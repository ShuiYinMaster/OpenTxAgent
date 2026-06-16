// TxAgent / Core / AgentLoop.cs
// 进程内 agent 编排循环 (DeepSeek / OpenAI 兼容)：
//   发请求 -> 拿到 assistant.tool_calls -> 执行工具 -> 以 role:"tool" 追加结果 -> 再发，直到模型不再调用工具。
//
// 线程模型 (关键)：
//   SendAsync 从 WinForms UI 线程的 async void 事件发起。await 的网络 I/O 在线程池完成，
//   但没有用 ConfigureAwait(false)，续延会回到 UI 同步上下文 —— 于是 tool.Execute(...)
//   天然在 UI 主线程上运行，可安全调用 Tecnomatix.Engineering。切勿在工具内另起线程碰 PS 对象。

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TxAgent.Core
{
    public sealed class AgentOptions
    {
        public string Model { get; set; }
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public string SystemPrompt { get; set; }
        public int MaxIterations { get; set; }
        /// <summary>发送前保留的最大历史消息数(不含 system)，超出从最早的用户回合边界裁掉，控 token。0=不裁。</summary>
        public int MaxHistoryMessages { get; set; }

        public AgentOptions()
        {
            Model = "deepseek-v4-pro";   // 适合 agent / 工具循环；高并发省钱可换 deepseek-v4-flash
            MaxTokens = 4096;
            Temperature = 0.3;
            MaxIterations = 12;
            MaxHistoryMessages = 40;
            SystemPrompt = DefaultSystemPrompt;
        }

        public const string DefaultSystemPrompt =
            "你是嵌入 Process Simulate (PDPS) 内部的 AI 助手，通过调用工具来查询和操作当前 PS 场景。\n" +
            "原则：\n" +
            "1. 用中文简洁作答。\n" +
            "2. 行动前先用只读工具了解场景状态，不要凭空假设对象名或参数。\n" +
            "3. 会改动场景的操作 (建几何、对齐、导出等) 由系统在执行前请用户确认；你只需正常调用，被拒绝时换个思路或向用户解释。\n" +
            "4. 只依据工具实际返回的结果作答，绝不编造工具输出或场景内容。\n" +
            "5. 一次只做一件清晰的事，必要时分步调用工具。\n" +
            "6. 需要场景对象的数量、清单或层级时，用 count_objects / list_children 等遍历工具取真实数据；绝不能从操作列表(list_operations)推断对象类型或数量。\n" +
            "7. 汇总好的信息(设备/机器人清单、点数统计等)可用 export_table 导出成 Excel。\n" +
            "8. 当没有合适的现成工具时，可先用 list_types/inspect_type/inspect_object 探查 PS 的真实 API，再用 run_csharp 写 C# 代码完成(C# 5 语法；在 PS 进程内执行；改动可撤销；执行前需用户确认)。run_csharp 是兜底，优先用现成工具。注意：代码在 PS 主线程同步执行，期间 PS 会无响应——务必避免无界循环与超重操作；大批量操作要分批处理并用 log 输出进度，单次代码只做有限、可预期的工作量。\n" +
            "9. 方法记忆：遇到需要写代码的新需求时，先用 list_snippets/get_snippet 查片段库有没有现成可复用的做法；用 run_csharp 摸索出一个有价值且可复用的做法后，主动用 save_snippet 把可用代码存下来(name+description+code)，必要时再用 save_recipe 固化成可一键调用的工具，避免下次从零重来。\n" +
            "10. 复杂的多步任务：先用 update_plan 列出计划，每完成一步就更新清单状态，再继续。\n" +
            "11. 当你用现有工具跑通了一段值得复用的多步操作，可用 save_recipe 把它保存成新工具(引用现有工具 + {{参数}}模板)，之后直接调用；先用 list_recipes 看有没有现成的，优先复用而非重复创建。";
    }

    public sealed class AgentLoop
    {
        private readonly DeepSeekClient _client;
        private readonly ToolRegistry _tools;
        private readonly AgentOptions _options;
        private readonly List<ChatMessage> _messages = new List<ChatMessage>();

        public event Action<string> AssistantText;
        /// <summary>流式文本分片(边收边显示)。</summary>
        public event Action<string> AssistantDelta;
        public event Action<string, JObject> ToolCalled;
        public event Action<string, string, bool> ToolCompleted;
        public event Action<string> Info;
        /// <summary>对话历史发生变更(一轮结束)后触发，供外部持久化记忆。</summary>
        public event Action HistoryChanged;

        /// <summary>变更类工具执行前的审批回调；返回 true 放行。未设置时默认拒绝所有变更，安全失效。</summary>
        public Func<ITxAgentTool, JObject, bool> ApprovalRequest;

        public AgentLoop(DeepSeekClient client, ToolRegistry tools, AgentOptions options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tools = tools ?? throw new ArgumentNullException(nameof(tools));
            _options = options ?? new AgentOptions();
            ResetWithSystem();
        }

        public void Reset()
        {
            _messages.Clear();
            ResetWithSystem();
        }

        private void ResetWithSystem()
        {
            if (!string.IsNullOrEmpty(_options.SystemPrompt))
                _messages.Add(new ChatMessage("system", _options.SystemPrompt));
        }

        /// <summary>当前对话历史(只读)，供持久化。</summary>
        public System.Collections.Generic.IReadOnlyList<ChatMessage> History
        {
            get { return _messages; }
        }

        /// <summary>用持久化的历史恢复对话。系统提示用当前配置刷新，其余消息照原样接续。</summary>
        public void LoadHistory(System.Collections.Generic.IEnumerable<ChatMessage> msgs)
        {
            _messages.Clear();
            ResetWithSystem();
            if (msgs == null) return;
            foreach (var m in msgs)
                if (m != null && m.Role != "system")
                    _messages.Add(m);
        }

        public async Task SendAsync(string userText, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userText)) return;
            _messages.Add(new ChatMessage("user", userText));

            try
            {
            for (int iter = 0; iter < _options.MaxIterations; iter++)
            {
                ct.ThrowIfCancellationRequested();
                TrimHistory();

                var request = new ChatRequest
                {
                    Model = _options.Model,
                    MaxTokens = _options.MaxTokens,
                    Temperature = _options.Temperature,
                    Stream = false,
                    Messages = _messages,
                    Tools = _tools.ToToolDefs()
                };

                // 流式：边收边回调文本分片；结束后拿到拼好的 assistant 消息(含 tool_calls)。
                var assistant = await _client.SendStreamAsync(request,
                    frag => AssistantDelta?.Invoke(frag), ct);

                // 原样回写 assistant 这一轮 (含 tool_calls，否则下一轮 API 报错)。
                assistant.Role = "assistant";
                _messages.Add(assistant);

                // 没有工具调用则本轮结束。
                var calls = assistant.ToolCalls;
                if (calls == null || calls.Count == 0) return;

                // 4) 逐个执行，以 role:"tool" 追加结果。
                foreach (var tc in calls)
                {
                    var name = tc.Function != null ? tc.Function.Name : null;
                    var input = ParseArguments(tc.Function != null ? tc.Function.Arguments : null);

                    ToolCalled?.Invoke(name, input);

                    string output;
                    bool isError;
                    RunOneTool(name, input, out output, out isError);

                    ToolCompleted?.Invoke(name, output, isError);
                    _messages.Add(new ChatMessage("tool", output) { ToolCallId = tc.Id });
                }
            }

            Info?.Invoke("已达到最大工具调用轮数，已停止。可重述需求或拆成更小的步骤。");
            }
            finally
            {
                HistoryChanged?.Invoke();
            }
        }

        private void RunOneTool(string name, JObject input, out string output, out bool isError)
        {
            isError = false;

            ITxAgentTool tool;
            if (string.IsNullOrEmpty(name) || !_tools.TryGet(name, out tool))
            {
                output = "未知工具: " + (name ?? "<null>");
                isError = true;
                return;
            }

            if (!tool.IsReadOnly)
            {
                bool approved = ApprovalRequest != null && ApprovalRequest(tool, input);
                if (!approved)
                {
                    output = "用户拒绝执行该变更操作。";
                    isError = true;
                    AuditLog.Write("DENIED  tool=" + name + "  input=" + Compact(input));
                    return;
                }
            }

            try
            {
                output = tool.Execute(input) ?? string.Empty;
            }
            catch (Exception ex)
            {
                output = "工具执行异常: " + ex.Message;
                isError = true;
            }

            if (!tool.IsReadOnly)
                AuditLog.Write((isError ? "FAILED  " : "APPLIED ") + "tool=" + name
                               + "  input=" + Compact(input) + "  result=" + FirstLine(output));
        }

        private static string Compact(JObject input)
        {
            if (input == null) return "{}";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            return s.Length <= 300 ? s : s.Substring(0, 300) + "…";
        }

        private static string FirstLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            int nl = s.IndexOf('\n');
            var line = nl >= 0 ? s.Substring(0, nl) : s;
            return line.Length <= 200 ? line : line.Substring(0, 200) + "…";
        }

        private static JObject ParseArguments(string argumentsJson)
        {
            if (string.IsNullOrWhiteSpace(argumentsJson)) return new JObject();
            try { return JObject.Parse(argumentsJson); }
            catch { return new JObject(); } // 容忍模型偶发的非法 JSON
        }

        /// <summary>
        /// 把历史裁剪到"系统提示 + 最近 MaxHistoryMessages 条"。
        /// 切点对齐到 user 消息边界，避免把 assistant.tool_calls 和它的 tool 结果拆散(否则 API 报错)。
        /// </summary>
        private void TrimHistory()
        {
            int max = _options.MaxHistoryMessages;
            if (max <= 0 || _messages.Count <= max + 1) return; // +1 给 system

            int hasSystem = (_messages.Count > 0 && _messages[0].Role == "system") ? 1 : 0;
            int cut = _messages.Count - max;
            if (cut <= hasSystem) return;

            // 切点前移到下一个 user 边界，保证 tool 配对完整
            while (cut < _messages.Count && _messages[cut].Role != "user") cut++;
            if (cut >= _messages.Count || cut <= hasSystem) return;

            var kept = new List<ChatMessage>();
            if (hasSystem == 1) kept.Add(_messages[0]);
            for (int i = cut; i < _messages.Count; i++) kept.Add(_messages[i]);
            _messages.Clear();
            _messages.AddRange(kept);
        }
    }
}
