# TxAgent.Core Harness 接入说明

更新：2026-09-04；依据源码提交 `d7433b9`。本目录已经是正式执行路径，不再是可选实验引擎。

## 当前架构

`TxAgentForm.BuildLoop` 始终创建 `HarnessAgentLoop`。旧执行循环已移除，没有 `UseNewHarness` 开关；`Core/AgentLoop.cs` 仅保留 `AgentOptions` 和系统提示词。

| 层 | 关键文件 | 职责 |
|---|---|---|
| 宿主无关循环 | `AgentLoop.cs`、`AgentSession.cs`、`Messages.cs` | 模型往返、工具执行、错误处理、工作上下文 |
| 契约 | `ILlmClient.cs`、`ITool.cs`、`IAgentHost.cs` | 模型、工具、主线程/审批/保存能力 |
| 协议适配 | `DeepSeekLlmClient.cs` | 请求转换、真实流式、端点兼容 |
| 宿主适配 | `PsAgentHost.cs`、`TxAgentToolAdapter.cs` | 封送 PS 调用、权限、工具结果转换 |
| UI/归档桥 | `HarnessAgentLoop.cs`、`IAgentLoop.cs` | 记忆注入、历史同步、事件转发 |

宿主无关内核使用 `TxAgent.Core` 命名空间；PS 适配使用 `TxTools.Agent.Harness`。并非整个目录都不依赖 PS SDK，也不是“原样引入、不能修改”的外部骨架。

## 模型与流式路径

```text
TxAgentForm → HarnessAgentLoop → TxAgent.Core.AgentLoop
  → DeepSeekLlmClient.CompleteStreamAsync
  → DeepSeekClient.SendStreamAsync
  → SSE reasoning/content → UI 事件
  → 完整 tool_calls → 工具执行 → 结果回灌 → 下一次模型调用
```

- `IStreamingLlmClient` 支持真实 SSE 边收边发，不是把完整回答切碎后的伪流式。不支持流式时退回 `CompleteAsync`。
- `LlmResponse.ReasoningContent` 经 session 与桥接保留到归档；历史页面默认折叠展示。
- 官方 DeepSeek V4 默认 `reasoning_effort=low`；配置允许 `high/max`。仅匹配官方域名与 V4 模型时发送参数，其他端点不套用。
- 官方 V4 带 tools 的请求回传历史思考；存储和网络序列化分离，不把所有模型的思考无条件发给所有端点。
- 协议转换中的两套 `ChatMessage/ToolCall` 类型不可混淆：使用别名或全限定名。

## 生效的默认值

| 设置 | 当前宿主路径 | 说明 |
|---|---:|---|
| 最大模型往返 | 50 | 来自 `AgentOptions`；裸内核默认 25 |
| 输出预算 | 12,288 / 8,192 | `OutputBudgetFor` 按模型选择；推理与正文共用 |
| 换思路提示阈值 | 连续失败 3 次 | 提示词还要求 2 次失败后主动换验证路径 |
| 工具失败熔断 | 连续失败 6 次 | 不是连续 3 次即中止 |
| 模型调用重试 | 2 次 | 首次调用之外的重试 |
| 流式 | 开启 | 还取决于客户端支持 |
| 回滚点 | 首次写操作前尝试建立 | 不是完整工程/外部系统事务 |
| 只读阶段 | 默认关闭 | `ReadOnlyPhase=true` 会限制暴露的工具 |

不要把 `AgentOptions.MaxTokens=4096`、裸内核的 `MaxTokens=16384` 当成宿主当前请求值：桥接层会传入模型对应的输出预算。

## 线程与审批

普通 PS 工具由适配器通过 `host.Invoke` 封送主线程；实现 `ITxOffUiThreadTool` 的工具在后台执行。尤其 `ask_user` 不能占住 UI 线程等待点击，否则会死锁。

适配器将 `!IsReadOnly` 作为写入/破坏性标记，但是否弹窗还受宿主审批模式控制：

- `ask`：询问模式。
- `auto_safe`：按宿主半自动规则处理，代码审阅仍受保护。
- `auto_all`：允许自动执行；不应在文档中承诺“每个写操作必有弹窗”。

模式与回滚实现以 `PsAgentHost` 为准。成功保存工程不等于可以自动撤销 CATIA、文件系统或所有外部效果；恢复失败需要明确反馈。

## 历史完整性

1. 模型响应携带正文、工具调用及返回的思考，加入 session。
2. 工具结果先加入 session，再发出 `ToolFinished`，保存订阅者才能看到已配对的结果。
3. 桥接以消息对象身份去重追加归档，不用裁剪后会错位的数值游标。
4. `_workingMemory` 可裁剪/摘要，不能用它替换 `_fullHistory` 的旧内容。
5. 取消、最终调用失败和只返回思考的响应也有保存路径；用未完成标记避免产生空 assistant 消息。
6. `ConversationStore` 使用紧凑 JSON、原子替换和一个旧快照备份。不是逐 token 日志；强杀时仍可能丢失最后的未归档片段。

重试时已作废的流式缓冲会清空，不将它冒充为最终回答。旧归档没有保存的思考无法从结果反推。

## 记忆与工具策略

基础提示由 `SystemPromptBuilder` 构建并缓存，最多注入 6 条偏好/API事实及 5 条有正解的错误经验。本轮相关片段最多 3 个，结束后清理临时注入。

工程任务优先：查相关经验 → API/场景只读查询 → 脚本对比筛查 → 小范围执行 → 读回复核。不要用冗长自述、手算坐标或连续微调失败代码代替验证。

`PendingSnippetStore.EnableAutoPromotion=false`，重复执行不再默认晋升。片段取出仅登记候选，复用成功应由真实执行结果归因。

## 验证状态与接入检查

已在完整 TxTools 宿主中编译通过，`maintenance/StorageRegression.cs` 的 24 项离线检查通过，覆盖协议隔离、归档读写、取消/失败、仅思考响应、备份恢复和工具结果保存顺序。前端内联 JavaScript 语法检查通过。

尚未完成此次版本的真实端点联调与 PS/CATIA 场景回归。建议启动后依次验证：

1. 只读查询 → 工具返回 → 总结，核对真实对象与数量。
2. 正常回答和取消生成后重新打开历史，检查正文、思考和工具配对。
3. 使用 `ask` 模式，在可恢复测试工程中验证一个小范围写操作及读回结果。
4. 检查长对话裁剪后旧归档仍存在；模拟损坏测试只能在隔离目录进行。
5. 切换 provider，确认不会误发官方 V4 专有字段。

本仓库没有独立项目文件；依赖和编译入口见[主 README](../../README.md)，测试说明见[维护目录](../../maintenance/README.md)。
