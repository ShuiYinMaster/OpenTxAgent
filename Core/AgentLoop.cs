// TxTools.Agent / Core / AgentLoop.cs
// 进程内 agent 编排循环 (DeepSeek / OpenAI 兼容)。
//
// v2 记忆系统升级:
//   [P0-1] SetConvId / LoadHistory 时切换 TaskPlan 上下文,修 per-conversation 隔离 bug
//   [P1-1] 每轮 SendAsync 按需注入相关 Snippet(完整代码)为本轮临时 system 消息,轮末移除
//   [P1-3] BuildSystemPromptWithMemory 常驻注入 FactsStore + GotchasStore 的 TopN
//   [P1-3] ExtractLessonsAsync 供 UI 层在对话末调用,萃取 facts / gotcha 正解落库
//   [P1-4] RunOneTool 里 run_csharp 输出含错误时自动 GotchasStore.Record
//
// 线程模型:
//   SendAsync 从 WinForms UI 线程的 async void 事件发起。await 的网络 I/O 在线程池完成,
//   但没有用 ConfigureAwait(false),续延回到 UI 同步上下文 —— 于是 tool.Execute(...)
//   天然在 UI 主线程上运行,可安全调用 Tecnomatix.Engineering。切勿在工具内另起线程碰 PS 对象。
//   ExtractLessonsAsync 同样从 UI 触发,SendAsync 完成后异步跑,不阻塞对话。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TxTools.Agent.Core
{
    public sealed class AgentOptions
    {
        public string Model { get; set; }
        public int MaxTokens { get; set; }
        public double Temperature { get; set; }
        public string SystemPrompt { get; set; }
        public int MaxIterations { get; set; }

        /// <summary>保留的最近完整对话回合数。超出则将老回合压缩为摘要注入上下文。0=不压缩。</summary>
        public int MaxTurnsToKeep { get; set; }

        /// <summary>
        /// 自动审批工具名白名单:在此列表内的工具调用跳过用户弹窗、直接执行(写 AUTO-OK 审计)。
        /// 建议加入的低风险高频工具:simulate_operation / add_fact / add_gotcha_correction。
        /// 注意:run_csharp 不应加入,它有专属 CodeApprovalDialog 必须经过代码审阅。
        /// </summary>
        public HashSet<string> AutoApproveTools { get; private set; }

        public AgentOptions()
        {
            Model = "deepseek-v4-pro";
            MaxTokens = 4096;
            Temperature = 0.3;
            MaxIterations = 12;
            MaxTurnsToKeep = 3;
            SystemPrompt = DefaultSystemPrompt;
            AutoApproveTools = new HashSet<string>(StringComparer.Ordinal);
        }

        public const string DefaultSystemPrompt =
            "你是嵌入 Process Simulate (PDPS) 内部的 AI 助手,通过调用工具来查询和操作当前 PS 场景。\n" +
            "原则:\n" +
            "1. 用中文简洁作答。\n" +
            "2. 行动前先用只读工具了解场景状态,不要凭空假设对象名或参数。\n" +
            "3. 会改动场景的操作(建几何、对齐、导出等)由系统在执行前请用户确认;你只需正常调用,被拒绝时换个思路或向用户解释。\n" +
            "4. 只依据工具实际返回的结果作答,绝不编造工具输出或场景内容。\n" +
            "5. 一次只做一件清晰的事,必要时分步调用工具。\n" +
            "6. 需要场景对象的数量、清单或层级时,用 count_objects / list_children 等遍历工具取真实数据;绝不能从操作列表(list_operations)推断对象类型或数量。\n" +
            "7. 汇总好的信息(设备/机器人清单、点数统计等)可用 export_table 导出成 Excel。\n" +
            "8. 当没有合适的现成工具时,可先用 list_types/inspect_type/inspect_object 探查 PS 的真实 API,再用 run_csharp 写 C# 代码完成(C# 5 语法;在 PS 进程内执行;改动可撤销;执行前需用户确认)。run_csharp 是兜底,优先用现成工具。注意:代码在 PS 主线程同步执行,期间 PS 会无响应——务必避免无界循环与超重操作;大批量操作要分批处理并用 log 输出进度,单次代码只做有限、可预期的工作量。\n" +
            "9. 方法记忆(重要):\n" +
            "   • 每轮系统会自动检索并注入与你当前用户问题最相关的 Snippet(完整代码,标记为『本轮相关代码片段』);先扫一眼——命中就直接引用/改写,不要从零摸索。\n" +
            "   • 主动搜索: find_snippet 按语义关键字搜、get_snippet 按名称取。\n" +
            "   • run_csharp 执行成功后系统自动存片段(带 auto_ 前缀+语义标签);需覆盖或补说明用 save_snippet。\n" +
            "   • 稳定多步流程用 save_recipe 固化成可一键调用的工具,先 list_recipes 看有没有现成的,优先复用。\n" +
            "10. 踩坑避免(重要):\n" +
            "   • 系统提示末尾会列出常踩清单(签名+正解);写 run_csharp 前先扫一遍,遇到相同签名直接用正解写法。\n" +
            "   • 全表用 list_gotchas 查看。\n" +
            "   • run_csharp 若失败,系统自动落库;当你后来学到正解时,请主动调用 add_gotcha_correction 补充,让下一次能避坑。\n" +
            "11. 事实记忆:\n" +
            "   • 系统提示头部列出的『已知事实』是跨对话保留的用户偏好/场景常量/API 事实,应视为默认前提。\n" +
            "   • 用户明确表达偏好、给出场景常量、或你验证了一条 SDK 事实时,主动调用 add_fact 存档。全表用 list_facts 查看。\n" +
            "12. 跨对话回忆:遇到“我之前是不是处理过 X”/“上次那个方案”等需要历史信息时,用 search_past_conversations 搜索所有历史对话。\n" +
            "13. 复杂的多步任务:先用 update_plan 列出计划,每完成一步就更新清单状态,再继续。\n" +
            "14. 当你用现有工具跑通了一段值得复用的多步操作,可用 save_recipe 把它保存成新工具(引用现有工具 + {{参数}} 模板),之后直接调用;先用 list_recipes 看有没有现成的,优先复用而非重复创建。\n" +
            "15. 机器人基座校验:用 check_robot_base 校验场景内所有机器人的 BASE0 是否与期望一致。\n" +
            "16. 机器人运动学:用 inspect_robot_kinematics 查询一台机器人的关节数、各关节名称和当前值、TCP 数量。\n" +
            "17. 操作→机器人:用 find_robot_for_op 查找操作绑定的机器人。\n" +
            "18. 对象位姿查询:用 get_object_location 查询对象的世界坐标 XYZ(mm) 和旋转角。\n" +
            "19. 设备 Z 对齐扫描:用 scan_devices_z 先检查哪些设备需要落地(Z≠0),再决定是否用 align_devices_z 执行对齐。\n" +
            "20. 设置对象位置:用 set_object_location 设置对象的世界坐标 XYZ(mm) 和可选旋转(度)。需审批,可 Ctrl+Z 撤销。\n" +
            "21. 仿真播放:用 simulate_operation 播放/重置/回退一个操作的仿真。需审批。\n" +
            "22. C# 5 语法陷阱(避免编译失败):\n" +
            "   • 三元 null 必须转型:var x = flag ? (string)null : val;\n" +
            "   • 无字符串插值 $「...」:用 「...」 + var 或 string.Format(...)\n" +
            "   • 无 ?. 空条件:用 if(obj!=null){obj.Prop} 模式\n" +
            "   • 无 => 表达式体:用完整 { return ...; }\n" +
            "   • TxSelection 无索引器:用 sel.GetItems()[0]\n" +
            "   • 花括号必须配对:每写一个 { 立刻写对应 }\n" +
            "   • var 不能推断 null:var x = null; ← 错误,需 var x = (string)null;\n" +
            "23. PS SDK 速查:\n" +
            "   • 选中对象:var items = TxApplication.ActiveSelection.GetItems();\n" +
            "   • 按名查找:var list = TxApplication.ActiveDocument.GetObjectsByName(「name」);\n" +
            "   • 场景根:doc.PhysicalRoot / doc.OperationRoot / doc.MfgRoot\n" +
            "   • 类型遍历:var f = new TxTypeFilter(typeof(TxWeldPoint)); var pts = doc.MfgRoot.GetAllDescendants(f);\n" +
            "   • 读坐标:obj.AbsoluteLocation.Translation → TxVector(mm); .RotationRPY_ZYX → 弧度\n" +
            "   • 读关节:robot.DrivingJoints → TxObjectList; joint.CurrentValue/.Type/.Name\n" +
            "   • 设坐标(需 Undo):obj.AbsoluteLocation = new TxTransformation(...);\n" +
            "   • ITxLeadingPart 无 Name:需 ((ITxObject)wp.LeadingPart).Name\n" +
            "   • log() 和 return 在方法体内直接可用\n" +
            "24. 写 run_csharp 代码纪律:先在脑中把完整逻辑想清楚再写,争取一次编译通过。每次编译失败=浪费一轮迭代+大量 token。提交前对照规则 22 逐条检查。\n" +
            "25. 批量操作:find_objects 按名称/类型关键字搜;batch_rename 三种模式(prefix_replace/suffix_replace/regex_replace),需审批。\n" +
            "26. 碰撞检测:query_collision_sets 列出场景配置的碰撞组;若 SDK 版本无此 API,用 list_types('Collision') 探索后再 run_csharp。";
    }

    public sealed class AgentLoop
    {
        /// <summary>
        /// 当前活动的 AgentLoop 实例(单窗口应用,单例)。
        /// 供 TxAgentCommand 里注册 SearchPastConversationsTool / AddFactTool 时的
        /// lambda 使用: () => AgentLoop.Current?.CurrentConvId
        /// TxAgentForm 在 BuildLoop 后设置, OnFormClosed 时清空。
        /// </summary>
        public static AgentLoop Current { get; set; }

        private readonly DeepSeekClient _client;
        private readonly ToolRegistry _tools;
        private readonly AgentOptions _options;
        private readonly List<ChatMessage> _messages = new List<ChatMessage>();      // API 工作记忆(可裁剪)
        private readonly List<ChatMessage> _fullHistory = new List<ChatMessage>();   // 完整对话(永不裁剪,供持久化)

        private LessonExtractor _lessonExtractor;

        public event Action<string> AssistantText;
        public event Action<string> AssistantDelta;
        public event Action<string, JObject> ToolCalled;
        public event Action<string, string, bool> ToolCompleted;
        public event Action<string> Info;
        public event Action HistoryChanged;
        public event Action<int, int, int> TokenUsed;

        public int TotalPromptTokens { get; private set; }
        public int TotalCompletionTokens { get; private set; }
        public int TotalTokens { get { return TotalPromptTokens + TotalCompletionTokens; } }

        /// <summary>变更类工具执行前的审批回调;返回 true 放行。未设置时默认拒绝所有变更。</summary>
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
            _fullHistory.Clear();
            TotalPromptTokens = 0;
            TotalCompletionTokens = 0;
            ResetWithSystem();
        }

        private void ResetWithSystem()
        {
            var prompt = BuildSystemPromptWithMemory();
            if (!string.IsNullOrEmpty(prompt))
            {
                var sysMsg = new ChatMessage("system", prompt);
                _messages.Add(sysMsg);
                _fullHistory.Add(sysMsg);
            }
        }

        public IReadOnlyList<ChatMessage> FullHistory { get { return _fullHistory; } }
        public IReadOnlyList<ChatMessage> WorkingMemory { get { return _messages; } }

        public void LoadHistory(IEnumerable<ChatMessage> msgs)
        {
            _messages.Clear();
            _fullHistory.Clear();
            ResetWithSystem();
            if (msgs == null) return;
            foreach (var m in msgs)
                if (m != null && m.Role != "system")
                {
                    _messages.Add(m);
                    _fullHistory.Add(m);
                }
        }

        public async Task SendAsync(string userText, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userText)) return;
            var userMsg = new ChatMessage("user", userText);
            _messages.Add(userMsg);
            _fullHistory.Add(userMsg);

            // [P1-1] 按需 Snippet 注入:根据本轮用户问题即时召回 Top-3 完整代码,
            // 作为独立 system 消息插入到工作记忆,仅本轮有效,finally 里移除。
            ChatMessage snippetSysMsg = InjectRelevantSnippets(userText);

            try
            {
                for (int iter = 0; iter < _options.MaxIterations; iter++)
                {
                    ct.ThrowIfCancellationRequested();
                    CompressHistory();

                    var request = new ChatRequest
                    {
                        Model = _options.Model,
                        MaxTokens = _options.MaxTokens,
                        Temperature = _options.Temperature,
                        Stream = false,
                        Messages = _messages,
                        Tools = _tools.ToToolDefs()
                    };

                    var assistant = await _client.SendStreamAsync(request,
                        frag => { if (AssistantDelta != null) AssistantDelta(frag); }, ct,
                        usage =>
                        {
                            if (usage != null)
                            {
                                TotalPromptTokens += usage.PromptTokens;
                                TotalCompletionTokens += usage.CompletionTokens;
                                if (TokenUsed != null)
                                    TokenUsed(usage.PromptTokens, usage.CompletionTokens, usage.TotalTokens);
                            }
                        });

                    assistant.Role = "assistant";
                    _messages.Add(assistant);
                    _fullHistory.Add(assistant);

                    var calls = assistant.ToolCalls;
                    if (calls == null || calls.Count == 0) return;

                    foreach (var tc in calls)
                    {
                        var name = tc.Function != null ? tc.Function.Name : null;
                        var input = ParseArguments(tc.Function != null ? tc.Function.Arguments : null);

                        if (ToolCalled != null) ToolCalled(name, input);

                        string output;
                        bool isError;
                        RunOneTool(name, input, out output, out isError);

                        if (ToolCompleted != null) ToolCompleted(name, output, isError);
                        var toolMsg = new ChatMessage("tool", output) { ToolCallId = tc.Id };
                        _messages.Add(toolMsg);
                        _fullHistory.Add(toolMsg);
                    }
                }

                if (Info != null)
                    Info("已达到最大工具调用轮数,已停止。可重述需求或拆成更小的步骤。");
            }
            finally
            {
                // [P1-1] 移除本轮临时注入的 Snippet 消息 —— 只影响工作记忆,不进历史
                if (snippetSysMsg != null) _messages.Remove(snippetSysMsg);
                if (HistoryChanged != null) HistoryChanged();
            }
        }

        // ── 工具执行 ──

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
                bool autoApproved = _options.AutoApproveTools != null
                                    && _options.AutoApproveTools.Contains(name);
                bool approved = autoApproved
                    || (ApprovalRequest != null && ApprovalRequest(tool, input));
                if (!approved)
                {
                    output = "用户拒绝执行该变更操作。";
                    isError = true;
                    AuditLog.Write("DENIED  tool=" + name + "  input=" + Compact(input));
                    return;
                }
                if (autoApproved)
                    AuditLog.Write("AUTO-OK tool=" + name + "  input=" + Compact(input));
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

            // AutoSnippet: run_csharp 成功后自动存片段
            if (!isError && name == "run_csharp")
            {
                try { AutoSaveSnippet(input, output); }
                catch { }
            }

            // [P1-4] AutoGotcha: run_csharp 输出含错误特征时自动落库
            // 注意: run_csharp 编译失败通常 isError=false,只是把错误作为文本返回,所以看 output 而非 isError
            if (name == "run_csharp" && IsGotchaWorthy(output))
            {
                try
                {
                    var code = GetStringFromInput(input, "code");
                    GotchasStore.Record(code, output, _currentConvId);
                }
                catch { }
            }
        }

        private static bool IsGotchaWorthy(string output)
        {
            if (string.IsNullOrEmpty(output)) return false;
            // 编译错误 CSxxxx / TxNotImplementedException / 明显的异常关键字
            if (output.IndexOf("CS0", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("CS1", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("编译失败", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("TxNotImplementedException", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("MissingMemberException", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("MissingMethodException", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("NullReferenceException", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("未知成员", StringComparison.Ordinal) >= 0) return true;
            if (output.IndexOf("找不到方法", StringComparison.Ordinal) >= 0) return true;
            return false;
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
            catch { return new JObject(); }
        }

        // ── [P1-1] 按需 Snippet 注入 ──

        /// <summary>
        /// 根据本轮用户消息即时召回 Top-3 相关 Snippet,以独立 system 消息插入工作记忆。
        /// 返回插入的消息对象,供 SendAsync finally 里移除。无命中返回 null。
        /// 注意:只加到 _messages,不加 _fullHistory —— 本轮临时上下文,不进永久历史。
        /// </summary>
        private ChatMessage InjectRelevantSnippets(string userText)
        {
            List<Snippet> snippets;
            try { snippets = SnippetStore.FindByTagOrKeyword(userText).Take(3).ToList(); }
            catch { return null; }

            if (snippets == null || snippets.Count == 0) return null;

            var sb = new StringBuilder();
            sb.AppendLine("【本轮相关代码片段】以下是与用户当前问题匹配的已验证 run_csharp 代码。" +
                          "若需求相近可直接引用或改写,不必再从零摸索:");
            foreach (var s in snippets)
            {
                var tagStr = s.Tags != null && s.Tags.Count > 0
                    ? "[" + string.Join(",", s.Tags) + "]" : "";
                sb.AppendLine();
                sb.AppendLine("--- " + s.Name + " " + tagStr
                    + " (复用 " + s.SuccessCount + " 次) ---");
                if (!string.IsNullOrEmpty(s.Description)) sb.AppendLine(s.Description);
                sb.AppendLine("```csharp");
                sb.AppendLine(s.Code);
                sb.AppendLine("```");
            }

            var msg = new ChatMessage("system", sb.ToString());
            _messages.Add(msg);
            return msg;
        }

        // ── 回合级压缩 + 摘要注入 ──

        private const string SUMMARY_PREFIX = "[前序对话摘要] ";

        private void CompressHistory()
        {
            int keepTurns = _options.MaxTurnsToKeep;
            if (keepTurns <= 0) return;
            if (_messages.Count <= 2) return;

            bool hasSys = _messages.Count > 0 && _messages[0].Role == "system";
            int startIdx = hasSys ? 1 : 0;

            string prevSummary = "";
            int summaryUserIdx = -1;
            int summaryAsstIdx = -1;
            for (int i = startIdx; i < _messages.Count - 1; i++)
            {
                if (_messages[i].Role == "user"
                    && _messages[i].Content != null
                    && _messages[i].Content.StartsWith(SUMMARY_PREFIX))
                {
                    summaryUserIdx = i;
                    prevSummary = _messages[i].Content.Substring(SUMMARY_PREFIX.Length);
                    if (i + 1 < _messages.Count && _messages[i + 1].Role == "assistant")
                        summaryAsstIdx = i + 1;
                    break;
                }
            }

            var clean = new List<ChatMessage>();
            if (hasSys) clean.Add(_messages[0]);
            for (int i = startIdx; i < _messages.Count; i++)
            {
                if (i == summaryUserIdx || i == summaryAsstIdx) continue;
                clean.Add(_messages[i]);
            }

            var turnStarts = new List<int>();
            int cleanStart = hasSys ? 1 : 0;
            for (int i = cleanStart; i < clean.Count; i++)
                if (clean[i].Role == "user") turnStarts.Add(i);

            if (turnStarts.Count <= keepTurns) return;

            int keepFrom = turnStarts[turnStarts.Count - keepTurns];

            var toCompress = new List<ChatMessage>();
            for (int i = cleanStart; i < keepFrom; i++)
                toCompress.Add(clean[i]);

            string newPart = GenerateTurnSummary(toCompress);
            string merged = prevSummary.Length > 0
                ? prevSummary + "\n---\n(后续) " + newPart
                : newPart;
            if (merged.Length > 1200)
                merged = merged.Substring(0, 1200) + "\n...(更多历史省略)";

            var rebuilt = new List<ChatMessage>();
            if (hasSys) rebuilt.Add(_messages[0]);
            rebuilt.Add(new ChatMessage("user", SUMMARY_PREFIX + merged));
            rebuilt.Add(new ChatMessage("assistant", "[确认] 已了解前序对话内容,基于以上上下文继续当前任务。"));
            for (int i = keepFrom; i < clean.Count; i++)
                rebuilt.Add(clean[i]);

            _messages.Clear();
            _messages.AddRange(rebuilt);
        }

        private string GenerateTurnSummary(List<ChatMessage> msgs)
        {
            if (msgs == null || msgs.Count == 0) return "";
            var sb = new StringBuilder();

            var subTurns = new List<List<ChatMessage>>();
            var cur = new List<ChatMessage>();
            foreach (var m in msgs)
            {
                if (m.Role == "user" && cur.Count > 0)
                {
                    subTurns.Add(cur);
                    cur = new List<ChatMessage>();
                }
                cur.Add(m);
            }
            if (cur.Count > 0) subTurns.Add(cur);

            foreach (var sub in subTurns)
            {
                var userMsg = sub.FirstOrDefault(m2 => m2.Role == "user");
                if (userMsg != null && userMsg.Content != null)
                    sb.AppendLine("用户: " + Truncate(userMsg.Content, 100));

                var calledTools = new List<string>();
                foreach (var m in sub)
                    if (m.Role == "assistant" && m.ToolCalls != null)
                        foreach (var tc in m.ToolCalls)
                            calledTools.Add(tc.Function != null ? tc.Function.Name : "?");
                if (calledTools.Count > 0)
                    sb.AppendLine("  调用: " + string.Join(" -> ", calledTools));

                var results = new List<ChatMessage>();
                foreach (var m in sub)
                    if (m.Role == "tool" && m.Content != null) results.Add(m);
                if (results.Count > 0)
                {
                    var keyInfo = new List<string>();
                    int take = Math.Min(results.Count, 5);
                    for (int ri = 0; ri < take; ri++)
                        keyInfo.Add(Truncate(ExtractKeyInfo(results[ri].Content), 70));
                    sb.AppendLine("  结果: " + string.Join("; ", keyInfo));
                }

                var conclusions = new List<ChatMessage>();
                foreach (var m in sub)
                    if (m.Role == "assistant"
                        && (m.ToolCalls == null || m.ToolCalls.Count == 0)
                        && m.Content != null && m.Content.Length > 0)
                        conclusions.Add(m);
                if (conclusions.Count > 0)
                    sb.AppendLine("  结论: " + Truncate(conclusions[conclusions.Count - 1].Content, 120));
            }

            return sb.ToString();
        }

        private static string Truncate(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Replace("\r", "").Replace("\n", " ").Trim();
            if (s.Length <= maxLen) return s;
            return s.Substring(0, maxLen) + "...";
        }

        private static string ExtractKeyInfo(string content)
        {
            if (string.IsNullOrEmpty(content)) return "";
            content = content.Replace("\r", "");
            int nl = content.IndexOf('\n');
            if (nl < 0) return content.Trim();
            var first = content.Substring(0, nl).Trim();
            if (first.Length < 30)
            {
                int nl2 = content.IndexOf('\n', nl + 1);
                if (nl2 > nl)
                    return first + " " + content.Substring(nl + 1, nl2 - nl - 1).Trim();
            }
            return first;
        }

        // ── AutoSnippet: run_csharp 成功后自动存片段 ──

        private void AutoSaveSnippet(JObject input, string output)
        {
            var code = GetStringFromInput(input, "code");
            if (string.IsNullOrWhiteSpace(code)) return;
            var lines = code.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 3) return;
            if (output != null && (output.IndexOf("编译失败", StringComparison.OrdinalIgnoreCase) >= 0
                || output.IndexOf("CS0", StringComparison.OrdinalIgnoreCase) >= 0
                || output.IndexOf("异常", StringComparison.OrdinalIgnoreCase) >= 0))
                return;
            if (output != null && output.Trim().Length < 20) return;
            if (SnippetStore.HasSimilarCode(code, 0.6)) return;

            var tags = SnippetStore.ExtractTags(code);
            var autoName = SnippetStore.AutoName(code);
            var desc = SnippetStore.AutoDescription(code, tags);

            var savedCode = code.Length > 2000 ? code.Substring(0, 2000) + "\n// …(截断)" : code;

            SnippetStore.Upsert(new Snippet
            {
                Name = autoName,
                Description = desc,
                Code = savedCode,
                Tags = tags,
                Origin = "auto",
                ConvId = _currentConvId
            });
        }

        private static string GetStringFromInput(JObject input, string key)
        {
            if (input == null) return null;
            var val = input[key];
            if (val == null) return null;
            return val.ToString();
        }

        // ── [P1-3/P1-4] 系统提示构建:注入 Facts + Gotchas (不再静态注入 Snippet) ──

        /// <summary>
        /// 构建含记忆的系统提示 = DefaultSystemPrompt + FactsStore.TopN + GotchasStore.TopN。
        /// Snippet 改为每轮 SendAsync 里按需注入(完整代码),此处不再列名单,避免双重注入。
        /// </summary>
        public static string BuildSystemPromptWithMemory()
        {
            var prompt = AgentOptions.DefaultSystemPrompt;
            var sb = new StringBuilder();

            // 事实记忆 (Facts) —— 用户偏好/场景常量/API事实/流程
            var facts = FactsStore.TopN(10);
            if (facts.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("【已知事实】(跨对话保留,视为对话默认前提):");
                foreach (var f in facts)
                    sb.AppendLine("  • [" + f.Category + "] " + f.Content);
            }

            // 踩坑清单 (Gotchas) —— 已知报错的签名与正解
            var gotchas = GotchasStore.TopN(15);
            if (gotchas.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("【避坑清单】(写 run_csharp 前核对,遇到相同签名直接用正解写法):");
                foreach (var g in gotchas)
                {
                    var fix = string.IsNullOrEmpty(g.Correction) ? "(暂无正解)" : g.Correction;
                    sb.AppendLine("  • [" + g.Signature + "] " + fix);
                }
            }

            return prompt + sb.ToString();
        }

        // ── 当前对话 ID + TaskPlan 切换 ──

        private string _currentConvId;

        public string CurrentConvId { get { return _currentConvId; } }

        /// <summary>
        /// 设置当前对话 ID。同时切换 TaskPlan 的活动对话(P0-1: 修 per-conversation 隔离 bug)。
        /// 外部在切换对话时应先 SetConvId,再 LoadHistory。
        /// </summary>
        public void SetConvId(string convId)
        {
            _currentConvId = convId;
            TaskPlan.SetActiveConversation(convId);   // [P0-1]
        }

        // ── [P1-3] 对话末经验萃取 ──

        /// <summary>
        /// 对当前对话跑一次经验萃取:提取 facts 落入 FactsStore,补充 gotchas 正解到 GotchasStore。
        /// 独立一次 LLM 调用,建议在 UI 层"结束对话/切换对话前"或对话消息数超阈值时触发。
        /// 不阻塞对话主循环,可 fire-and-forget。
        /// </summary>
        public async Task<LessonExtractor.ExtractResult> ExtractLessonsAsync(CancellationToken ct)
        {
            if (_lessonExtractor == null)
                _lessonExtractor = new LessonExtractor(_client, "deepseek-v4-flash");
            return await _lessonExtractor.ExtractAsync(_currentConvId, _fullHistory, ct);
        }

        /// <summary>兼容旧调用点(如 UI 层已有代码调用了这个名字)。等价于 BuildSystemPromptWithMemory。</summary>
        [Obsolete("改用 BuildSystemPromptWithMemory")]
        public static string BuildSystemPromptWithSnippets()
        {
            return BuildSystemPromptWithMemory();
        }
    }
}
