using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TxAgent.Core
{
    public sealed class AgentLoopOptions
    {
        /// <summary>单次任务最多允许的模型往返轮数，防止死循环。</summary>
        public int MaxIterations { get; set; }

        /// <summary>
        /// 同一工具连续失败达到此次数则熔断中止。
        /// 注意别设太小：探查类工具失败是正常的试错过程，实测模型常在第 4~5 次才脱困，
        /// 设成 3 会误杀本来能成功的任务。真正起作用的是下面的提示阈值。
        /// </summary>
        public int MaxConsecutiveToolFailures { get; set; }

        /// <summary>
        /// 同一工具连续失败达到此次数后，在回灌文本里追加"换思路"提示。
        /// 模型脱困往往靠的是改变策略（先探查再写），而不是继续微调同一份代码。
        /// </summary>
        public int FailureHintThreshold { get; set; }

        /// <summary>LLM 调用层失败（网络/限流）的重试次数。</summary>
        public int MaxLlmRetries { get; set; }

        /// <summary>true 表示本轮只暴露只读工具（两阶段执行的分析阶段）。</summary>
        public bool ReadOnlyPhase { get; set; }

        /// <summary>首次写操作前是否自动建立回滚点。</summary>
        public bool AutoRestorePoint { get; set; }

        /// <summary>
        /// 是否启用流式。仅当 ILlmClient 同时实现 IStreamingLlmClient
        /// 且其 SupportsStreaming 为 true 时生效，否则自动退回非流式。
        /// </summary>
        public bool EnableStreaming { get; set; }

        public AgentLoopOptions()
        {
            MaxIterations = 25;
            MaxConsecutiveToolFailures = 6;
            FailureHintThreshold = 3;
            MaxLlmRetries = 2;
            ReadOnlyPhase = false;
            AutoRestorePoint = true;
            EnableStreaming = true;
        }
    }

    public sealed class AgentRunResult
    {
        public bool Completed { get; set; }

        /// <summary>
        /// 最后一轮的文本。注意：正文已通过 ContentDelta 事件实时发出，
        /// UI 不应再拿这个字段重复显示，它只用于日志与结果判定。
        /// </summary>
        public string FinalMessage { get; set; }

        public string StopReason { get; set; }
        public int Iterations { get; set; }
        public int ToolCallCount { get; set; }
        public int ToolFailureCount { get; set; }
        public bool SceneMutated { get; set; }
        public RestorePoint RestorePoint { get; set; }
        public int TotalPromptTokens { get; set; }
        public int TotalCompletionTokens { get; set; }
    }

    /// <summary>
    /// harness 核心循环。对 PS / CATIA 均无感知，全部能力经 IAgentHost 与 ITool 注入。
    /// 所有事件均在后台线程触发，UI 侧须自行封送。
    /// </summary>
    public sealed class AgentLoop
    {
        private readonly ILlmClient _llm;
        private readonly IStreamingLlmClient _streamLlm;
        private readonly ToolRegistry _registry;
        private readonly IAgentHost _host;
        private readonly AgentLoopOptions _options;

        public AgentLoop(ILlmClient llm, ToolRegistry registry, IAgentHost host, AgentLoopOptions options)
        {
            if (llm == null) throw new ArgumentNullException("llm");
            if (registry == null) throw new ArgumentNullException("registry");
            if (host == null) throw new ArgumentNullException("host");

            _llm = llm;
            _streamLlm = llm as IStreamingLlmClient;
            _registry = registry;
            _host = host;
            _options = options ?? new AgentLoopOptions();
        }

        /// <summary>本次运行是否会走真流式。</summary>
        public bool StreamingActive
        {
            get { return _options.EnableStreaming && _streamLlm != null && _streamLlm.SupportsStreaming; }
        }

        // ── 事件（全部在后台线程触发） ──

        /// <summary>
        /// 阶段性状态文本。这是"日志/状态栏"通道，不是聊天流 —— 转发到会话气泡会和
        /// ToolStarting 产生的工具卡片重复。
        /// </summary>
        public event Action<string> Progress;

        /// <summary>
        /// 正文增量。流式下是 token 级分片；非流式下每轮结束时把整段文本作为一个增量发出。
        /// UI 只需订阅这一个事件即可拿到全部助手正文。
        /// </summary>
        public event Action<string> ContentDelta;

        /// <summary>思考内容增量（推理模型的 reasoning_content）。非推理模型不会触发。</summary>
        public event Action<string> ReasoningDelta;

        /// <summary>
        /// LLM 重试导致已发出的增量作废。UI 收到后应清空当前这一轮的文本缓冲。
        /// </summary>
        public event Action ContentReset;

        /// <summary>一轮模型输出结束（无论有无工具调用）。参数为该轮完整正文，可能为空。</summary>
        public event Action<string> TurnCompleted;

        /// <summary>工具即将执行（已通过审批）。</summary>
        public event Action<ToolCall> ToolStarting;

        /// <summary>工具执行结束。</summary>
        public event Action<ToolCall, ToolResult> ToolFinished;

        /// <summary>单次 LLM 调用的 token 用量（prompt, completion）。</summary>
        public event Action<int, int> Usage;

        public async Task<AgentRunResult> RunAsync(AgentSession session, CancellationToken ct)
        {
            var result = new AgentRunResult();
            var failureCounter = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            RestorePoint restorePoint = null;

            var tools = _registry.ExportSchemas(_options.ReadOnlyPhase);

            for (int iter = 1; iter <= _options.MaxIterations; iter++)
            {
                ct.ThrowIfCancellationRequested();
                result.Iterations = iter;

                session.TrimToBudget();

                var response = await CallLlmWithRetryAsync(session, tools, ct).ConfigureAwait(false);

                if (response.IsError)
                {
                    result.Completed = false;
                    result.StopReason = "LLM 调用失败: " + response.ErrorMessage;
                    result.RestorePoint = restorePoint;
                    return result;
                }

                result.TotalPromptTokens += response.PromptTokens;
                result.TotalCompletionTokens += response.CompletionTokens;
                RaiseUsage(response.PromptTokens, response.CompletionTokens);

                // 非流式路径：整段正文在这里补发一次，让 UI 只需订阅 ContentDelta
                if (!response.AlreadyStreamed)
                {
                    if (!string.IsNullOrEmpty(response.ReasoningContent))
                        RaiseReasoningDelta(response.ReasoningContent);
                    if (!string.IsNullOrEmpty(response.Content))
                        RaiseContentDelta(response.Content);
                }

                RaiseTurnCompleted(response.Content);

                // 只在这一轮确实产出了内容或工具调用时才入会话。
                // 模型偶尔会返回"空内容 + 无 tool_calls"(上下文被撑爆时常见),
                // 这种消息一旦进了历史,下一轮原样发回去会被 API 拒:
                //   400 Invalid assistant message: content or tool_calls must be set
                bool hasPayload = !string.IsNullOrWhiteSpace(response.Content) || response.HasToolCalls;
                if (hasPayload)
                {
                    session.Add(ChatMessage.CreateAssistant(
                        response.Content,
                        response.ToolCalls == null ? null : new List<ToolCall>(response.ToolCalls)));
                }
                else
                {
                    _host.Log("warn", "模型返回空内容且无工具调用，已丢弃该轮消息以免污染历史");
                }

                // 没有工具调用 = 模型认为任务结束
                if (!response.HasToolCalls)
                {
                    result.Completed = hasPayload;
                    result.FinalMessage = response.Content;
                    result.StopReason = hasPayload
                        ? "正常结束"
                        : "模型返回空响应(无内容也无工具调用)，可能是上下文过长或触发了内容过滤";
                    result.RestorePoint = restorePoint;
                    return result;
                }

                foreach (var call in response.ToolCalls)
                {
                    ct.ThrowIfCancellationRequested();
                    result.ToolCallCount++;

                    var tool = _registry.Find(call.Name);

                    if (tool == null)
                    {
                        session.Add(ChatMessage.CreateToolResult(call.Id,
                            "错误：不存在名为 \"" + call.Name + "\" 的工具。可用工具：" +
                            _registry.DescribeAvailable() + "。请改用其中之一重试。"));
                        continue;
                    }

                    if (_options.ReadOnlyPhase && tool.IsWrite)
                    {
                        session.Add(ChatMessage.CreateToolResult(call.Id,
                            "错误：当前处于只读分析阶段，禁止调用写入类工具 \"" + tool.Name +
                            "\"。请先完成分析并输出变更清单，写入将在用户确认后的下一阶段执行。"));
                        continue;
                    }

                    // 首次写操作前建立回滚点
                    if (tool.IsWrite && _options.AutoRestorePoint && restorePoint == null)
                    {
                        RaiseProgress("正在建立回滚点…");
                        restorePoint = SafeCreateRestorePoint("即将执行写入工具: " + tool.Name);

                        RaiseProgress(restorePoint.Created
                            ? "回滚点已建立: " + restorePoint.HowToRollback
                            : "未能建立回滚点: " + restorePoint.HowToRollback);

                        if (!restorePoint.Created)
                        {
                            bool goOn = _host.Confirm(
                                "无法建立回滚点",
                                "原因：" + restorePoint.HowToRollback +
                                "\n继续执行将无法自动回退，是否继续？",
                                true);

                            if (!goOn)
                            {
                                session.Add(ChatMessage.CreateToolResult(call.Id,
                                    "用户拒绝在无回滚点的情况下执行写入操作。请停止修改，改为输出建议方案。"));
                                continue;
                            }
                        }
                    }

                    if (tool.IsDestructive)
                    {
                        bool ok = _host.Confirm(
                            "确认破坏性操作",
                            "工具：" + tool.Name + "\n参数：" + Truncate(call.ArgumentsJson, 800),
                            true);

                        if (!ok)
                        {
                            session.Add(ChatMessage.CreateToolResult(call.Id,
                                "用户否决了这次调用。请不要重复相同调用，改为说明意图或换一种非破坏性方式。"));
                            continue;
                        }
                    }

                    // 工具卡片在执行「前」就发出，UI 才能看到"正在执行"的中间态
                    RaiseToolStarting(call);
                    RaiseProgress("执行 " + tool.Name);

                    ToolResult toolResult = SafeExecute(tool, call.ArgumentsJson);

                    RaiseToolFinished(call, toolResult);

                    if (toolResult.MutatedScene) result.SceneMutated = true;

                    // 先更新连续失败计数，再据此决定回灌文本要不要带"换思路"提示
                    int consecutive = 0;
                    if (toolResult.Success)
                    {
                        failureCounter[tool.Name] = 0;
                    }
                    else
                    {
                        result.ToolFailureCount++;
                        failureCounter.TryGetValue(tool.Name, out consecutive);
                        consecutive++;
                        failureCounter[tool.Name] = consecutive;
                        _host.Log("warn", "工具 " + tool.Name + " 连续第 " + consecutive
                            + " 次失败: " + toolResult.ErrorKind);
                    }

                    session.Add(ChatMessage.CreateToolResult(call.Id,
                        FormatForModel(tool, toolResult, consecutive)));

                    if (!toolResult.Success && consecutive >= _options.MaxConsecutiveToolFailures)
                    {
                        result.Completed = false;
                        result.StopReason = "工具 " + tool.Name + " 连续失败 " + consecutive + " 次，已熔断中止";
                        result.FinalMessage = toolResult.Content;
                        result.RestorePoint = restorePoint;
                        return result;
                    }
                }
            }

            result.Completed = false;
            result.StopReason = "达到最大轮数 " + _options.MaxIterations + "，任务未完成";
            result.RestorePoint = restorePoint;
            return result;
        }

        private async Task<LlmResponse> CallLlmWithRetryAsync(
            AgentSession session, IList<ToolSchema> tools, CancellationToken ct)
        {
            LlmResponse last = null;
            bool useStream = StreamingActive;

            // 本轮是否已经往 UI 发过增量。重试前需要通知 UI 丢弃这些半截内容。
            bool emitted = false;

            var handlers = new LlmStreamHandlers
            {
                OnReasoningDelta = text => { emitted = true; RaiseReasoningDelta(text); },
                OnContentDelta = text => { emitted = true; RaiseContentDelta(text); }
            };

            for (int attempt = 0; attempt <= _options.MaxLlmRetries; attempt++)
            {
                if (attempt > 0)
                {
                    if (emitted)
                    {
                        RaiseContentReset();
                        emitted = false;
                    }
                    _host.Log("warn", "LLM 重试第 " + attempt + " 次");
                    RaiseProgress("模型调用失败，正在重试 (" + attempt + ")…");
                    await Task.Delay(500 * attempt, ct).ConfigureAwait(false);
                }

                var request = new LlmRequest
                {
                    Messages = new List<ChatMessage>(session.Messages),
                    Tools = tools
                };

                try
                {
                    last = useStream
                        ? await _streamLlm.CompleteStreamAsync(request, handlers, ct).ConfigureAwait(false)
                        : await _llm.CompleteAsync(request, ct).ConfigureAwait(false);

                    if (last != null && !last.IsError) return last;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _host.Log("error", "LLM 调用异常: " + ex);
                    last = LlmResponse.Error(ex.GetType().Name + ": " + ex.Message);
                }
            }

            return last ?? LlmResponse.Error("未知错误");
        }

        private ToolResult SafeExecute(ITool tool, string argumentsJson)
        {
            try
            {
                var r = tool.Execute(argumentsJson, _host);
                return r ?? ToolResult.Fail("host", "工具返回了 null 结果。");
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // 工具实现漏掉的异常在这里兜底，转成可回灌的文本而不是崩掉整个循环
                _host.Log("error", "工具 " + tool.Name + " 抛出未捕获异常: " + ex);
                return ToolResult.Fail("host",
                    "工具执行抛出异常：" + ex.GetType().Name + ": " + ex.Message +
                    "\n请检查参数是否符合 schema，或换一种实现方式重试。");
            }
        }

        private RestorePoint SafeCreateRestorePoint(string reason)
        {
            try
            {
                var rp = _host.CreateRestorePoint(reason);
                if (rp == null) return RestorePoint.None("宿主返回 null");

                if (rp.Created)
                    _host.Log("info", "已建立回滚点: " + rp.HowToRollback);
                else
                    _host.Log("warn", "未建立回滚点: " + rp.HowToRollback);

                return rp;
            }
            catch (Exception ex)
            {
                _host.Log("error", "建立回滚点失败: " + ex);
                return RestorePoint.None(ex.Message);
            }
        }

        /// <summary>
        /// 统一回灌格式。失败时显式标注，促使模型自修而不是继续往下走；
        /// 连续失败到阈值后追加换思路提示 —— 实测模型脱困靠的是改变策略，
        /// 而不是在同一份代码上继续微调。
        /// </summary>
        private string FormatForModel(ITool tool, ToolResult r, int consecutiveFailures)
        {
            if (r.Success) return r.Content ?? "(执行成功，无输出)";

            var sb = new StringBuilder();
            sb.Append("【执行失败】工具: ").Append(tool.Name);
            if (!string.IsNullOrEmpty(r.ErrorKind))
                sb.Append(" | 类型: ").Append(r.ErrorKind);
            sb.Append(" | 连续第 ").Append(consecutiveFailures).Append(" 次");
            sb.AppendLine();
            sb.AppendLine(r.Content ?? "(无错误详情)");

            if (consecutiveFailures >= _options.FailureHintThreshold)
            {
                sb.AppendLine("【停下来换思路】同一工具已连续失败 " + consecutiveFailures +
                    " 次，继续微调同一份代码大概率还会失败。请改变策略：");
                sb.AppendLine("  1. 先用只读方式把真实情况查清楚 —— 对象是否存在、真实类型名是什么、" +
                    "目标成员的确切签名是什么，不要凭记忆假设 API。");
                sb.AppendLine("  2. 把一次大动作拆成最小的一步先跑通，再逐步加回其余逻辑。");
                sb.AppendLine("  3. 若确认此路不通，换用其它工具，或向用户说明卡在哪里。");
                int left = _options.MaxConsecutiveToolFailures - consecutiveFailures;
                if (left > 0)
                    sb.AppendLine("  （该工具还剩 " + left + " 次失败机会，之后任务将被中止。）");
            }
            else
            {
                sb.Append("请根据以上错误修正后重试。");
            }

            return sb.ToString();
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max) + "…(已截断)";
        }

        // ── 事件触发（一律吞掉订阅方异常，UI 出错不应中断 agent 循环） ──

        private void RaiseProgress(string text)
        {
            var h = Progress;
            if (h != null) { try { h(text); } catch { } }
        }

        private void RaiseContentDelta(string text)
        {
            var h = ContentDelta;
            if (h != null && !string.IsNullOrEmpty(text)) { try { h(text); } catch { } }
        }

        private void RaiseReasoningDelta(string text)
        {
            var h = ReasoningDelta;
            if (h != null && !string.IsNullOrEmpty(text)) { try { h(text); } catch { } }
        }

        private void RaiseContentReset()
        {
            var h = ContentReset;
            if (h != null) { try { h(); } catch { } }
        }

        private void RaiseTurnCompleted(string text)
        {
            var h = TurnCompleted;
            if (h != null) { try { h(text); } catch { } }
        }

        private void RaiseToolStarting(ToolCall call)
        {
            var h = ToolStarting;
            if (h != null) { try { h(call); } catch { } }
        }

        private void RaiseToolFinished(ToolCall call, ToolResult r)
        {
            var h = ToolFinished;
            if (h != null) { try { h(call, r); } catch { } }
        }

        private void RaiseUsage(int prompt, int completion)
        {
            var h = Usage;
            if (h != null && (prompt > 0 || completion > 0)) { try { h(prompt, completion); } catch { } }
        }
    }
}