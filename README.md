# TxAgent — PDPS 进程内 AI 助手插件 (DeepSeek)

在 Process Simulate (PS 2402) 内嵌一个 AI 智能体，通过工具调用查询/操作场景。
**原生 C# 进程内方案**：直连 DeepSeek (OpenAI 兼容) 的 tool-calling 循环，所有工具执行都落在 PS 的 UI 主线程上，
不依赖 Node、不跨进程，最稳。

**v3 里程碑**：UI 全 HTML 化（WebView2 承载）、记忆系统三层（片段/事实/踩坑）、文件上传解析（xlsx/csv/txt）、审批模式三档可切换。

## 架构一览

```
TxAgent/
  TxAgentCommand.cs          插件入口 (TxButtonCommand)，注册工具、开窗
  Core/                      与 PS 解耦的 agent 核心 (可独立编译/单测)
    LlmModels.cs             DeepSeek/OpenAI 的请求/响应 DTO (messages, tools, tool_calls, usage)
    DeepSeekClient.cs        直连 /chat/completions 的 HTTP 客户端 (Bearer + TLS1.2 + SSE 流式)
    KeyStore.cs              API key 的 DPAPI 加密落盘 (插件文件夹优先)
    PsContext.cs             把 PS 调用同步路由回主线程 (SynchronizationContext.Send)
    ITxAgentTool.cs          工具接口 + 可选基类
    ToolRegistry.cs          工具注册表 (v3 加 Tools 只读枚举供 UI 展示)
    AgentLoop.cs             编排循环 + 静态 Current 引用 + 经验萃取入口
    AuditLog.cs              变更类工具的审批/执行结果追加日志

    Recipe.cs                配方数据模型 (步骤 + 参数模板)
    RecipeStore.cs           配方持久化 (recipes.json)
    RecipeTool.cs            配方 → 工具的包装 (参数替换 + 只读继承 + 递归保护)

    ConversationStore.cs     多对话持久化 (conversations/{id}.json)

    ── 记忆系统 (v2) ──
    SnippetStore.cs          代码片段库 (snippets.json,自动存/按需注入)
    FactsStore.cs            跨对话事实/偏好 (facts.json,Jaccard 去重)
    GotchasStore.cs          踩坑清单 (gotchas.json,CS1061/CS0117 精准签名)
    LessonExtractor.cs       对话末经验萃取 (独立 LLM 调用产结构化 JSON)
    TaskPlan.cs              per-conversation 任务清单 (update_plan 工具)

    ── 文件上传 (v3) ──
    UploadStore.cs           内存字典 + %TEMP%\TxTools.Agent\uploads\{convId}\
    XlsxReader.cs            xlsx 读取 (基于 DocumentFormat.OpenXml)
    XlsxWriter.cs            xlsx 写出 (手写 Open XML, export_table 用)
    FileParserService.cs     按扩展名分发解析 xlsx/csv/tsv/txt/md/json/xml

  UI/
    TxAgentForm.cs           WebView2 壳 (500 行, JS↔C# 消息路由)
    chat.html                完整 UI (顶栏/消息/输入/附件/抽屉/modal, 内置简易 Markdown)
    ApiKeyDialog.cs          原生 API Key 弹窗 (v3 起 form 不再引用, 保留兜底)
    ConversationListDialog.cs 原生历史对话弹窗 (v3 起 form 不再引用, 保留兜底)
    CodeApprovalDialog.cs    run_csharp 代码审阅框 (v3 仍在用)

  Tools/                     具体工具实现 (见"工具清单"章节)
```

## 依赖与引用

- **目标框架**：.NET Framework 4.8，C# 7.3
- `Tecnomatix.Engineering`（PS SDK；主窗口继承 `TxForm`）
- `TxTools.Common`（`FormUiKit`；UI 框架与其他插件保持一致）
- `Newtonsoft.Json` 13.x —— 手工 `<Reference><HintPath>` 引 `libs\` 目录
- `Microsoft.Web.WebView2`（NuGet）—— UI 承载
- **`DocumentFormat.OpenXml` 2.20.0**（NuGet）—— xlsx 读取，2.20.0 明确支持 .NET Framework 4.6+
- `System.Net.Http`、`System.Windows.Forms`、`System.Drawing`、`System.Security`（DPAPI）

**运行前提**：
- **网络**：PS 工作站需放行到 `api.deepseek.com` 的出站 HTTPS
- **WebView2 Runtime**：Windows 10/11 一般系统级预装；缺失时弹 MessageBox 提示装 Evergreen Runtime
- **TLS**：客户端已在静态构造里启用 TLS1.2

## UI 架构 (v3)

**主窗口就是一个填满的 WebView2 控件**，其余全部在 `chat.html` 里：顶栏（品牌名 + 模型下拉 + **审批模式下拉** + 新对话/历史/工具/萃取/Key 按钮）、消息区（Markdown 渲染 + 工具卡片折叠 + 表格支持）、输入区（附件卡片 + textarea + 上传/发送/停止）、状态栏（左状态 · 右 token 计数）、三个 modal（历史对话抽屉 · 工具面板 · API Key 输入）。

**chat.html 部署方式**：作为**嵌入资源** (`Build Action = EmbeddedResource`) 打包进 dll，不再依赖 bin 目录物理复制。加载走 `WebResourceRequested` 拦截 `https://chathtml/*` 返回嵌入字节，页面 URL 保持 `chat.html` 便于 F12 调试报错定位。文件带 UTF-8 BOM 强制 Chromium 按 UTF-8 解析，绕开 Windows 中文系统的编码坑。

**通信协议 v2**：
- JS → C#：`jsReady / setApiKey / switchModel / setApprovalMode / userSend / userStop / newConv / listConvs / openConv / deleteConv / listTools / uploadFile / removeAttachment / extractLessons`
- C# → JS：`modelList / keyReady / askApiKey / convList / toolList / clear / restore / message / delta / closeAssistant / toolCall / toolResult / status / busy / tokenUsage / attachmentInfo`

**审批弹窗保留原生**：`ApprovalRequest` 委托签名同步 `bool`，改成异步会牵连 AgentLoop 深层重构；原生 `CodeApprovalDialog` / `MessageBox` 自带消息 pump，不阻塞主循环。

**关键线程坑**：所有对 `CoreWebView2` 的属性访问（包括 `== null` 判断）必须在 UI 线程。`AgentLoop.SendAsync` 在 `Task.Run` 里跑，事件回调（delta/toolCall/…）在线程池触发；`PostJs` 必须先 `BeginInvoke` marshal 到 UI 线程再摸 `CoreWebView2`，否则抛 "CoreWebView2 can only be accessed from the UI thread"。

## API Key

- 首次打开若无已保存的 key，前端弹 modal 输入；点确定后 **DPAPI 按当前 Windows 用户加密**，保存为插件文件夹下的 `deepseek.key`（Base64 密文）
- 之后每次打开自动解密复用；顶部"Key" 按钮可随时重输覆盖
- 插件目录不可写时（如 `Program Files`），自动回退到 `%LOCALAPPDATA%\TxTools.Agent\deepseek.key`
- 解密绑定当前用户：换 Windows 账户或换机器需重新输入

## 模型

窗口顶栏可切换（切换后 `_loop` 用新模型重建，`_current.Messages` 无损保留继续对话）：
- `deepseek-v4-pro`（默认）—— 复杂推理 / agent 工具循环
- `deepseek-v4-flash` —— 高并发、低成本；`LessonExtractor` 默认用它，成本低
- `deepseek-chat` —— 旧名，兼容用

## 审批模式 (v3 新增)

顶栏第二个下拉，session 级，关窗归位：

| 模式 | 值 | 行为 |
|---|---|---|
| **审批·询问** | `ask` | 默认。所有变更工具都弹 dialog |
| **审批·半自动** | `auto_safe` | `run_csharp` 仍弹代码审阅框，其他变更工具自动通过 |
| **审批·全自动** | `auto_all` | 所有变更工具（含 `run_csharp`）自动通过；切换时前端弹 confirm 二次确认 |

**永久白名单**（`AgentOptions.AutoApproveTools`）与模式独立：`add_fact` / `add_gotcha_correction` 只写自家 json 库不动 PS 场景，任何模式下都直接通过，`AuditLog` 记 `AUTO-OK`。

**AuditLog 完整覆盖**：所有变更类工具的每次审批/执行都写一行 `audit.log`——`APPLIED` / `FAILED` / `DENIED` / `AUTO-OK` / `AUTO-SAFE` / `AUTO-ALL` / `APPROVAL-MODE = xxx`，可追溯。

## 线程模型（关键，别踩坑）

`AgentLoop.SendAsync` 内部 `await` 网络 I/O 时**没用 `ConfigureAwait(false)`**，续延回 UI 同步上下文——于是 `tool.Execute(...)` 天然在 UI 主线程运行，可安全调用 PS SDK。**切勿在工具内另起线程去碰 PS 对象**。

`TxAgentForm.HandleUserSendAsync` 里 `await Task.Run(() => _loop.SendAsync(...))` 把整个 agent 循环丢到线程池，UI 线程保持响应"停止"按钮，PS 界面不冻。工具执行内部若需调 PS SDK，通过 `PsContext.Run(Action)` 同步路由回主线程（`SynchronizationContext.Send`）。

**固有限制**：单个重操作（如遍历所有机器人）在主线程执行时，PS 必然短暂无响应——原生命令也一样。所以系统提示要求 `run_csharp` 写**有界代码**、大批量**分批并 log 进度**；审批框也提示"执行期 PS 会无响应"。真遇到生成代码里的死循环，只能结束 PS 进程，**批准前务必读一遍代码**是最后防线。

## DeepSeek 工具调用回合结构

1. 请求带 `tools`（`{type:"function", function:{name, description, parameters(JSON Schema)}}`）
2. 响应 `choices[0].message.tool_calls[]`，每项含 `id` 与 `function.arguments`（**JSON 字符串**，需 `JObject.Parse`）
3. 把该 assistant 消息**原样**追加回去（含 tool_calls），再为每次调用追加一条 `role:"tool"` 消息（带 `tool_call_id` + 结果文本）
4. 再次请求，直到模型不再返回 tool_calls

流式：`DeepSeekClient.SendStreamAsync` 解析 SSE，边收边把文本分片回调 `AssistantDelta`，并增量累积 `tool_calls` 分片；前端开一行【助手】→追加分片→工具调用或回合结束时闭行。

## 记忆系统 (三层)

### 1. 情景记忆 —— 多对话保留

`ConversationStore` 每条对话存成 `conversations/{id}.json`（含标题/时间/messages）。
- 每轮 `HistoryChanged` 触发 `SaveCurrent`，写回当前对话文件（空对话不落盘）
- 开窗时加载**最近一条**继续；顶部"新对话" = 存好当前 + 开新的；"历史"抽屉可打开/删除任意过往
- **删除当前对话的坑**已修：删的若是 `_current`，立即清空并 `StartFreshConversation()`，否则下次切换时 `SaveCurrent()` 会把 `_current` 原地写回，出现"删了又出现"
- `AgentLoop._messages` 每轮压缩（保留最近 `MaxTurnsToKeep=3` 个 user 回合，其余提炼成摘要），`_fullHistory` 全量保留供持久化和萃取

### 2. 方法记忆 —— 代码片段

`SnippetStore` 把摸索出的可用 `run_csharp` 代码持久化到 `snippets.json`。
- **AutoSnippet**：`run_csharp` 成功后自动存（带 `auto_` 前缀 + 语义标签 + `HasSimilarCode` Jaccard 去重）
- **按需注入 (v2 关键改动)**：每轮 `SendAsync` 前根据用户消息即时召回 Top-3 相关片段（含**完整代码**），作为独立 system 消息插入本轮工作记忆，轮末 `finally` 里移除。命中就直接引用/改写，不用再调 `find_snippet` 二次拉取
- `find_snippet` / `get_snippet` / `save_snippet` 手动接口保留

### 3. 语义记忆 —— 事实 + 踩坑

**FactsStore**（`facts.json`）：跨对话保留的用户偏好、场景常量、验证过的 SDK 事实、稳定工作流。
- 类别：`preference` / `scene_constant` / `api_fact` / `workflow` / `misc`
- Jaccard≥0.7 自动去重，仅刷新 `LastConfirmedUtc`
- `BuildSystemPromptWithMemory` 每轮把 Top-10 注入 system prompt 头部，视为对话默认前提

**GotchasStore**（`gotchas.json`）：`run_csharp` 报错的反面教材。
- **AutoGotcha**：`RunOneTool` 里 `run_csharp` 输出含 CS0xxx/TxNotImplementedException 等特征时自动 `Record`
- **精准签名 (v3 关键改动)**：`ExtractSignature` 用专用正则识别 CS1061/CS0117/CS0246 的"XX 不包含 YY 的定义"消息，从引号内抠出真正的 `Type.Member`，覆盖 6 种引号变体（U+0022/U+0027/U+201C/U+201D/U+2018/U+2019）。同时黑名单跳过 `Tecnomatix.Engineering` / `System.*` 等命名空间前缀。避免"所有 CS1061 归到同一个笼统签名"的痛点
- 系统提示注入 Top-15，`{Type.Member → 正解}` 精确到 API
- AI 学到解法后应主动调 `add_gotcha_correction` 补 `Correction`

**LessonExtractor**（对话末萃取）：
- 触发时机：`NewConversation` / `OpenConversation` / `OnFormClosed` 之前，`FullHistory.Count >= 4` 才跑
- fire-and-forget，用便宜模型（`deepseek-v4-flash`）独立一次 LLM 调用，产结构化 JSON `{facts, gotchas}`
- 分别落 `FactsStore.Add` 与 `GotchasStore.AddCorrection`

**`search_past_conversations`** 工具：跨对话按关键字搜索历史消息，遇到"上次那个方案""我之前是不是处理过 X"时 AI 主动调用。

## 文件上传/解析 (v3)

**上传流程**：
1. 前端 drag-drop / paste / 点击附件按钮 → `FileReader.readAsDataURL` → base64 postMessage 给 C#
2. `UploadStore.Store(convId, filename, bytes)` 落到 `%TEMP%\TxTools.Agent\uploads\{convId}\`
3. `FileParserService.Parse` 按扩展名分发解析，产 500-2000 字符摘要
4. 摘要通过 `attachmentInfo` 回传前端；前端在附件卡片里显示可折叠展开
5. 用户发送时，C# 把附件摘要拼到用户消息前缀，AI 直接看到即可判断能否答

**支持格式**：`.xlsx / .csv / .tsv / .txt / .md / .log / .json / .xml`。
- **xlsx**：基于 **DocumentFormat.OpenXml SDK**（不再是自己手写的 XmlReader），正确处理 SharedString / InlineString / Boolean / 命名空间前缀（如 `x:sheet`）等所有边界
- **csv/tsv**：自动检测分隔符（`, ; \t` 频次统计），支持 quoted "..." + 双引号转义
- **文本类**：UTF-8 优先，失败回退 GBK

**按需精读**：`read_uploaded_file(file_id, sheet?, row_from?, row_to?, char_from?, char_to?)` 让 AI 分片读大文件，单次上限 12000 字符，防止塞爆 token。

**清理**：切对话不清（用户可能切回来引用同一附件），关窗时 `UploadStore.ClearAll()` 统一清理临时目录。

## 任务机制：计划/待办

`update_plan(items)` 让 agent 把复杂多步任务拆成带状态的清单（整表替换，每项 `text`+`done`）。
- **v2 修复**：`TaskPlan` 改为 **per-conversation**（原全局静态单例会跨对话污染），`AgentLoop.SetConvId` 时同步 `TaskPlan.SetActiveConversation(convId)`
- 系统提示要求：多步任务前先列计划、每完成一步更新状态

## 动态能力：探查 API + 自写代码 (兜底)

现成工具覆盖不到时，agent 可以"从内部读懂 API、再据此写代码"：

- **API 探查（只读）**：
  - `list_types(keyword)` 在已加载程序集（优先 Tecnomatix）搜类型
  - `inspect_type(type_name)` 反射列出某类型的公共属性/方法/事件签名
  - `inspect_object(name?)` 探查活动对象的运行时类型与各属性取值

- **自写代码**：`run_csharp(code)` 用 .NET 自带 CodeDom 在 **PS 进程内**编译执行。代码作为方法体注入（已 `using Tecnomatix.Engineering`，可用 `TxApplication.ActiveDocument`、`log("…")`、`return` 结果）。

  **安全门控**：
  - `run_csharp` 是变更工具，`ask` / `auto_safe` 模式下每次强制审批，`CodeApprovalDialog` 展示完整代码供审阅
  - 执行包在 **Undo 块**里（可 Ctrl+Z 撤销）
  - 审批/结果写入 `audit.log`；编译/运行异常都回传（AI 借此自我修正）
  - 失败自动 `GotchasStore.Record`；成功自动 `SnippetStore.Upsert`
  - **C# 5 语法**（无字符串插值 / `?.` / 表达式体）；编译出的程序集无法卸载，宜偶发使用

  典型链路：`list_types("Collision")` → `inspect_type("TxCollisionRoot")` → `run_csharp(...)` 完成一个没有现成工具的操作。

## 配方机制：自写工具 (无代码)

配方 = 一串对现有工具的调用 + `{{参数}}` 模板。能力完全被原子工具集合框死，无编译器、无新依赖，也不会引入超出现有工具的破坏面。

**工作流**：
1. agent 用现有工具跑通一个多步任务
2. `save_recipe(name, description, parameters, steps)`，`steps[].tool` 必须是已存在的工具名，`steps[].input` 模板里用 `{{参数名}}` 引用 `parameters`
3. 保存校验步骤工具都存在 → 写入 `recipes.json` → **即时注册成新工具**
4. 启动时 `RecipeStore.Load()` 把已存配方注册回来；`list_recipes` 查、`delete_recipe(name)` 删

**安全**：`IsReadOnly` 按步骤继承——全只读免审批；任一步会改场景则整条配方执行前一次性确认，内部步骤不二次弹窗。`save_recipe` 本身只读，但不允许用配方名遮蔽内置原语；配方间递归有 8 层深度上限。

**局限**（有意）：只支持"参数化的固定序列"，无分支/循环/条件——那类逻辑由 agent 在对话里直接编排工具。

**预置配方**（`SeedDefaultRecipes`，只入内存不写盘）：
- `preflight_check` 列操作 + 焊点数 + 参考系
- `scene_overview` 类型直方图 + 当前文档
- `robot_audit` BASE0 校验 + 类型统计
- `weld_preflight` 焊接前置检查

用户同名配方优先，不覆盖。

## 工具清单

**只读原语**：
- 场景：`query_scene` / `count_objects` / `list_children` / `list_operations` / `count_points` / `list_tcp_options` / `check_reachability` / `get_reference_frame` / `find_objects`
- 反射探查：`list_types` / `inspect_type` / `inspect_object`
- 机器人：`check_robot_base` / `inspect_robot_kinematics` / `find_robot_for_op`
- 位姿：`get_object_location` / `scan_devices_z`
- 碰撞：`query_collision_sets`

**变更原语**（需审批 / 可 Ctrl+Z 撤销）：
- 位置：`set_object_location` / `align_devices_z`
- 命名：`batch_rename`（prefix_replace / suffix_replace / regex_replace 三模式）
- 仿真：`simulate_operation`
- 选中：`select_objects`
- 兜底：`run_csharp`（专属代码审阅框）

**导出**：`export_table` / `export_points_excel` / `export_object_list`

**记忆系统工具**：
- 片段：`save_snippet` / `list_snippets` / `get_snippet` / `find_snippet`
- 事实：`add_fact`（自动通过） / `list_facts`
- 踩坑：`add_gotcha_correction`（自动通过） / `list_gotchas`
- 跨对话：`search_past_conversations`

**上传解析**：`list_uploaded_files` / `read_uploaded_file`

**任务/配方**：`update_plan` / `list_recipes` / `save_recipe` / `delete_recipe`

## 加一个工具

1. 在 `Tools/` 下实现 `ITxAgentTool`（或继承 `TxAgentToolBase`）：`Name` / `Description` / `IsReadOnly` / `InputSchema` / `Execute`。工具名限 a-z A-Z 0-9 `_` `-`，最长 64
2. PS 实际调用直接用 `Tecnomatix.Engineering` API；如需路由到主线程，用 `PsContext.Current.Run(...)`
3. `TxAgentCommand.BuildToolRegistry()` 里 `reg.Register(new YourTool())`
4. 需要拿当前 convId 的工具（如 AddFactTool），构造函数注入 `() => AgentLoop.Current?.CurrentConvId`

## 目录与持久化

| 路径 | 内容 |
|---|---|
| `{插件目录}\deepseek.key` | DPAPI 加密的 API Key |
| `{插件目录}\conversations\{id}.json` | 每条对话 |
| `{插件目录}\recipes.json` | 已保存配方 |
| `{插件目录}\snippets.json` | 代码片段库 |
| `{插件目录}\facts.json` | 跨对话事实/偏好 |
| `{插件目录}\gotchas.json` | 踩坑清单 |
| `{插件目录}\audit.log` | 变更操作审计 |
| `%TEMP%\TxTools.Agent\uploads\{convId}\` | 上传附件（关窗清理） |

插件目录不可写时（如部署在 `Program Files`），自动回退到 `%LOCALAPPDATA%\TxTools.Agent\`。

## 调试与排错

- **F12 打开 DevTools**：Console 看 JS 错误、Sources 看 chat.html 真实行号、Network 看拦截请求
- **VS 输出窗口** 会打印 `[TxAgent] chat.html loaded, length=..., startsWith=..., endsWith=...` 和 `[XlsxReader] sharedStrings loaded / 找到 N 个 sheet` 等诊断
- 前端 JS 崩了会通过 `window.onerror` 显示在状态栏（不再静默"初始化中"）
- 前端 3 秒未收到 C# 的 `modelList` 会在页面顶部弹红色横幅提示按 F12
- **审计文件 `audit.log`** 记录所有变更操作，追溯出问题的时机与工具名

## 已知限制

- **`run_csharp` 用 C# 5 语法**（自带 CodeDom 编译器）：无字符串插值 `$"..."`、无 `?.`、无表达式体、`var x = null` 不合法
- **`run_csharp` 编译出的程序集不能卸载**，宜偶发使用；不建议高频调用
- **文件上传上限 20 MB**（JS 端硬阻），更大文件通过 `read_uploaded_file` 分片读
- **WebView2 CoreWebView2 严格 UI 线程 affinity**：所有属性访问必须在 UI 线程，别的线程读 `== null` 都会抛
