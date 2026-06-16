# TxAgent

**一个嵌入西门子 Tecnomatix Process Simulate (PDPS) 的进程内 AI 助手**——用自然语言查询场景、统计导出、检查可达性、对齐设备，并在需要时探查 SDK API、即时编写并执行 C# 来完成没有现成工具的任务。

> 由 DeepSeek 驱动，运行在 PS 进程内，通过一套可扩展的工具集对接 `Tecnomatix.Engineering` SDK。

---

## ⚠️ 免责声明（请先读）

- 本项目**非西门子官方产品**，与 Siemens / Tecnomatix 无任何隶属关系。`Process Simulate`、`Tecnomatix` 等为其各自所有者的商标。
- 本插件包含 **`run_csharp`：在 PS 进程内即时编译并执行任意 C# 代码**的能力。这本质上是进程内任意代码执行，**唯一的安全边界是你在审批框里点“允许”前是否读懂了那段代码**。请务必逐段审阅后再批准。
- 所有改动场景的操作都需人工审批，并尽量包在 Undo 块里（可 Ctrl+Z）；但仍可能有未覆盖的副作用。**请在测试工程 / 有备份的前提下使用，风险自负。**
- 大语言模型会犯错。它给出的统计、可达性结论、生成的代码都可能不正确，**关键决策请自行复核**。
- 使用需要你自备 **DeepSeek API Key**（或任意 OpenAI 兼容端点），会产生相应的 API 费用。

---

## 这是什么

PS 二次开发里，很多重复性的查询/统计/校验任务都要写一次性脚本或点很多步。TxAgent 把这些能力封装成**工具**，让 LLM 用自然语言来编排：你说“检查 OP120 的可达性”“把所有机器人导出成清单”，它自己选工具、填参数、必要时探查 API、跑通后还能把方法记下来复用。

它**不是**“让 AI 接管 PS”，而是一个**工具编排器**：稳定能力以原生工具提供，AI 只负责理解意图和调度；`run_csharp` 是兜底的“开发期探针”，不是主力。

## 功能特性

- 🔍 **场景查询**：选中对象、文档信息、按类型统计/枚举、展开组件数子对象。
- 🧭 **双树感知**：区分物理树（设备/机器人/夹具）与操作树（机器人/焊接操作），各有专用查找工具。
- 🎯 **选中与操作**：按名查找并选中对象，打通“查到 → 选中 → 操作”。
- 📊 **统计导出**：焊点坐标、对象清单、任意汇总表 → Excel（手写 Open XML，无外部库）。
- 🤖 **可达性检查**：给操作名即可，逐点用 `GetPoseAtLocation` 判可达（快速摘要）。
- 🛠 **设备落地对齐**：选中设备最低点对齐到 Z=0，跳过末端工具，需审批、可撤销。
- 🧬 **API 探查 + 自写代码**：`list_types` / `inspect_type` / `inspect_object` 摸清 SDK，再用 `run_csharp` 写 C# 完成新需求。
- 🧠 **方法记忆**：把摸索出的可用代码存进**片段库**，把多步操作固化成**配方**，跨对话复用。
- 💬 **多对话 + 流式输出**：像常见 AI 工具一样保留历史对话、可回看；回答边收边显示。
- 🪟 **非模态窗口**：浮在 PS 上，主界面照常操作（选对象、点别的按钮）。

## 工作原理 / 架构

```
用户输入 ─▶ TxAgentForm (聊天窗, TxForm, 非模态)
              │
              ▼  (后台线程)
        AgentLoop ──▶ DeepSeekClient (SSE 流式, OpenAI 兼容)
              │            ▲
              │  tool_calls │ tool_results
              ▼            │
        ToolRegistry ─▶ 各工具 ─▶ PsBridge ─(PsContext 回主线程)─▶ Tecnomatix.Engineering SDK
                              └─▶ ApiInspector / CSharpRunner / DeviceZAlignService …
```

**线程模型（关键）**：PS SDK 单线程、非线程安全。Agent 循环跑在**后台线程**，所有 SDK 调用经 `PsContext`（`SynchronizationContext.Send`）**回到 PS 主线程**执行。这样网络/编译/思考期间窗口与 PS 保持响应，“停止”可在步骤之间生效。注意：单个重操作在主线程执行时 PS 仍会短暂无响应，这是 PS 单线程的固有特性。

`run_csharp` 的编译（CodeDom，纯 CPU）在后台线程进行，只有执行回主线程，避免编译冻结 UI。

## 环境要求 & 依赖

| 项 | 说明 |
|---|---|
| 平台 | Tecnomatix Process Simulate **2402**（其它版本多数 API 经 `dynamic`+反射兼容，但未逐一验证） |
| 框架 | .NET Framework **4.8**，C# **7.3** |
| LLM | DeepSeek API Key（base url `https://api.deepseek.com`，OpenAI 兼容；可改其它兼容端点） |

**外部依赖（需自备 / 一并纳入你的解决方案）**：

- `Tecnomatix.Engineering`（PS 安装自带的 SDK 程序集）。
- `Newtonsoft.Json` —— **务必引用 PS 自带的那个版本**（`Copy Local=false`），版本不一致会在运行时报 `MissingMethodException`。
- `TxTools.Common`（`FormUiKit`）—— 本仓库代码引用了它做 DPI/皮肤/控件自绘。**本仓库未包含**，请用你自己的 `FormUiKit`，或把 `TxAgentForm` 改成普通 `TxForm` 自行布局。
- `MyPlugin.ExportGun`（`PsReader` / `OperationInfo` / `ExcelExporter` 等）—— 焊点导出、操作识别、参考系等依赖它。**本仓库未包含**，需替换为你自己的等价实现。

> 这两个外部命名空间是把 TxAgent 接到既有插件工程上的“胶水”。开源使用时，最省事的做法是把 TxAgent 作为一个模块放进你已有的 TxTools 解决方案；独立使用则需自行提供 `FormUiKit` 与 `PsReader` 的等价实现（依赖都收敛在 `UI/` 与 `Ps/PsBridge.cs`）。

## 构建 & 安装

1. 把 `TxAgent/` 下全部 `.cs` 加入你的 PS 插件工程（老式 `.csproj` 记得“包括在项目中”，每个文件都要）。
2. 添加引用：`Tecnomatix.Engineering`、PS 自带的 `Newtonsoft.Json`（`Copy Local=false`）、`System`（CodeDom 在内）、`System.Drawing`、`System.Windows.Forms`；以及你的 `FormUiKit` / `PsReader` 所在工程。
3. 在你的插件注册入口把 `TxAgentCommand` 挂到工具栏（`TxButtonCommand`，按你现有插件的方式注册）。
4. 编译为 DLL，按 PS 加载自定义命令的方式部署。

> 想用现代 C# 语法写 `run_csharp`？引用 NuGet 包 `Microsoft.CodeDom.Providers.DotNetCompilerPlatform`（默认用 .NET 自带编译器，仅 C# 5 语法）。

## 配置

- 首次打开窗口会提示输入 **DeepSeek API Key**；Key 经 **Windows DPAPI 加密**保存到插件目录（回退 `%LOCALAPPDATA%\TxAgent`）。随时可点“设置 API Key…”更改。
- 模型下拉：`deepseek-v4-pro`（默认）/ `deepseek-v4-flash`（更快更省）/ `deepseek-chat`。
- 端点/默认模型等在 `Core/DeepSeekClient.cs` 与 `Core/AgentLoop.cs`（`AgentOptions`）里调整。

## 使用

打开窗口后直接用自然语言下指令，例如：

- `场景概览` / `场景里有多少机器人？`
- `CD_L 下有多少设备？`
- `检查 OP120 的可达性`
- `把所有机器人导出成清单`
- `把选中操作的焊点坐标导出 Excel`
- `把选中的设备落地对齐到 Z=0`（会弹审批 + 可撤销）

复杂任务它会先列计划（`update_plan`）；没有现成工具时会探查 API 并写 `run_csharp`（弹代码审批框，读懂再批）；跑通有价值的做法会用 `save_snippet` / `save_recipe` 记下来下次复用。

## 工具一览

| 类别 | 工具 | 作用 |
|---|---|---|
| 只读·场景 | `query_scene` | 当前选中对象 / 文档信息 |
| 只读·场景 | `count_objects` | 全场景按类型统计/枚举（数机器人等） |
| 只读·场景 | `list_children` | 展开组件，按类型数子对象 |
| 只读·操作 | `find_operations` | 在**操作树**里按名查找操作 |
| 只读·操作 | `list_operations` | 选中操作的名称/类型/工具 |
| 只读·操作 | `count_points` | 按类型统计点数 |
| 只读·操作 | `list_tcp_options` | 操作的 TCP 选项 |
| 只读·操作 | `check_reachability` | 给操作名即做可达性摘要 |
| 只读·操作 | `get_reference_frame` | 当前参考坐标系 |
| 只读·API | `list_types` / `inspect_type` / `inspect_object` | 反射探查 SDK 类型与活动对象 |
| 导出 | `export_table` | 任意汇总表 → xlsx |
| 导出 | `export_points_excel` | 选中操作焊点坐标 → xlsx（含参考系转换） |
| 导出 | `export_object_list` | 遍历匹配对象 → xlsx（机器人/设备清单） |
| 动作 | `select_objects` | 按名选中物理树对象 |
| 变更 | `align_devices_z` | 选中设备落地 Z=0（审批 + 撤销 + 审计） |
| 变更 | `run_csharp` | 即时编译执行 C#（审批 + 撤销 + 审计） |
| 记忆·配方 | `save_recipe` / `list_recipes` / `delete_recipe` | 把多步工具序列固化为可调用工具 |
| 记忆·片段 | `save_snippet` / `list_snippets` / `get_snippet` | 持久化可复用的 C# 代码 |
| 任务 | `update_plan` | 维护多步任务清单 |

新增工具：实现 `ITxAgentTool`（或继承 `TxAgentToolBase`），在 `TxAgentCommand.BuildToolRegistry()` 里 `reg.Register(...)` 即可。

## 记忆机制

- **多对话**：每条对话存为 `conversations/{id}.json`（标题/时间/消息）。“新对话”保留旧对话、开新的；“历史对话”可回看或删除。旧版单文件 `conversation.json` 自动迁移。
- **配方**（`recipes.json`）：参数化的固定工具序列，固化稳定可复用的多步操作。仅参数替换，不支持分支/循环。
- **片段库**（`snippets.json`）：持久化可复用的 `run_csharp` 代码，给 codegen 路径的“方法记忆”——摸清一次 API、存下代码，跨对话复用。
- **上下文裁剪**：`AgentOptions.MaxHistoryMessages`（默认 40）每轮把喂给模型的上下文裁到“系统提示 + 最近 N 条”，切点对齐到 user 边界以保 tool_call 配对；只裁上下文，不删磁盘记录。

## 安全模型

- 改动场景的工具（`run_csharp`、`align_devices_z`）**强制人工审批**；`run_csharp` 的审批框可滚动展示**完整代码**供审阅。
- 变更尽量包在 **Undo 块**里，可 Ctrl+Z 撤销。
- 变更结果写入 `audit.log`（APPLIED / DENIED / FAILED + 工具 + 入参 + 结果）。
- **`run_csharp` 是进程内任意代码执行，无法沙箱化**——审批前读代码是最后防线。编译出的程序集无法卸载，宜偶发使用、跑通后固化为片段/配方/原生工具。

## 已知限制

- 重操作在主线程执行时 PS 会短暂无响应，且一旦进入主线程无法安全中断（PS 单线程固有）。
- `run_csharp` 默认仅 **C# 5** 语法（无字符串插值 / `?.` / 表达式体）。
- 仅在 PS **2402** 上验证；其它版本 SDK 签名可能不同。
- 是否把成功做法存成片段/配方依赖模型自觉（可直接命令它“存成片段”强制沉淀）。
- LLM 输出需自行复核。

## 目录结构

```
TxAgent/
├─ TxAgentCommand.cs         # 插件入口：注册工具、开窗（非模态）
├─ Core/                     # Agent 核心（与 PS 无关）
│  ├─ AgentLoop.cs           #   工具循环 + 系统提示 + 历史裁剪
│  ├─ DeepSeekClient.cs      #   SSE 流式客户端（OpenAI 兼容）
│  ├─ ToolRegistry.cs / ITxAgentTool.cs / LlmModels.cs
│  ├─ ConversationStore.cs   #   多对话持久化
│  ├─ RecipeStore.cs / Recipe.cs / RecipeTool.cs   # 配方
│  ├─ SnippetStore.cs        #   代码片段库
│  ├─ TaskPlan.cs            #   任务计划
│  ├─ KeyStore.cs            #   API Key（DPAPI）
│  ├─ AuditLog.cs            #   变更审计
│  ├─ XlsxWriter.cs          #   通用 xlsx 写出
│  └─ PsContext.cs           #   主线程路由
├─ Ps/                       # 对接 PS SDK
│  ├─ PsBridge.cs            #   所有 SDK 调用的门面
│  ├─ ApiInspector.cs        #   反射式 API 探查
│  ├─ CSharpRunner.cs        #   in-process C# 编译执行
│  └─ DeviceZAlignService.cs #   无界面 Z 对齐
├─ Tools/                    # 各工具（实现 ITxAgentTool）
└─ UI/                       # 窗口（TxForm + FormUiKit）
   ├─ TxAgentForm.cs / ApiKeyDialog.cs
   ├─ CodeApprovalDialog.cs / ConversationListDialog.cs
```

## 贡献

欢迎 Issue / PR：新工具、其它 PS 版本适配、其它 LLM 端点、把成熟能力拆成独立插件命令等。提交前请确保所有改动文件都已纳入工程，并通过编译。

## 许可证

建议 **MIT**（在仓库根目录放 `LICENSE` 文件即可），你也可换成更合适的协议。注意：本项目链接的 `Tecnomatix.Engineering` 等为西门子专有 SDK，使用者需自行持有合法的 Process Simulate 授权——本项目仅为源代码，不分发任何西门子组件。

## 致谢

- 基于 DeepSeek（OpenAI 兼容 API）。
- 运行于 Siemens Tecnomatix Process Simulate（非官方、无隶属）。
