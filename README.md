# TxTools.Agent (TxAgent)

嵌入 **Siemens Tecnomatix Process Simulate 2402** 的 AI 助手插件——通过 LLM 调用按组启用的内置工具查询、操作 PS 场景,并把踩过的坑固化成记忆持久保留。

- **平台**:.NET Framework 4.8；当前宿主工程使用 C# 8.0，`run_csharp` 的传统编译器限制另计
- **运行环境**:作为 PS 插件加载到 PS 进程内
- **仓库**:[ShuiYinMaster/OpenTxAgent](https://github.com/ShuiYinMaster/OpenTxAgent)
- **集成宿主**:[TxTools](https://github.com/ShuiYinMaster/TxTools)；本仓库是 Agent 源码子树，不含独立 `.csproj`、完整宿主或专有 SDK。
- **文档更新**:2026-09-04，按源码提交 `d7433b9` 核对。

## 当前进度

已实现：统一 Harness 循环、真实 SSE 流式、官方 DeepSeek V4 默认低强度思考、工具优先的查询/对比/验证流程、独立思考归档、原子保存与备份回退、记忆筛选和可恢复清理。

验证状态：在现有 TxTools 宿主工程中隔离编译通过，24 项离线回归及 HTML 内联 JavaScript 语法检查通过。新版本已在维护环境部署；未进行此次变更后的付费模型联调或 PS/CATIA 场景变更回归，不能把离线通过等同于真实场景验收。

维护工具与测试边界见 [maintenance/README.md](maintenance/README.md)。仓库不包含本机对话、运行记忆、API Key、偏好配置和部署备份。

---

## 快速定位

| 我想…… | 看这里 |
|---|---|
| 加新工具 | [工具系统](#工具系统) → 实现 `ITxAgentTool` 即可,`AutoRegisterTools` 反射注册 |
| 调 LLM | [多 LLM Provider](#多-llm-provider) → `DeepSeekClient` 是 OpenAI 兼容 |
| 改 UI | [WebView2 + chat.html](#前端-webview2--chathtml) → 单文件 HTML+CSS+JS |
| 加记忆条目 | [记忆系统](#记忆系统) → Snippet/Fact/Gotcha,一物一 Markdown 文件 |
| 改循环引擎 | [Agent Harness](#agent-harness-txagentcore) → 当前唯一入口为 `HarnessAgentLoop` |
| 查 PS API 签名 | [API 知识库](#api-知识库) → `api_lookup` 直接反射,不用再 probe |
| 挂本地资料 | [本地知识库](#本地知识库) → md 丢进 `memory/knowledge/`,支持语义检索 |
| 让 AI 看图 | [图像识别](#图像识别) → `analyze_image` / `analyze_viewport`,委托给视觉模型 |
| 改别的插件源码 | [源码工具](#源码工具) → 骨架/行段读 + 精确替换 + 编译验证 |
| 生成文档 | [文档生成](#文档生成-docxxlsxpptx) → docx/xlsx/pptx 均从零建 |
| 截图/相机 | [PS 视口 & 相机](#ps-视口--相机) → `capture_viewer_image` + `set_camera_view` |

---

## 架构总览

```text
chat.html / recipe-sidebar.js
  → TxAgentForm → IAgentLoop → HarnessAgentLoop
      → TxAgent.Core.AgentLoop
          → DeepSeekLlmClient → DeepSeekClient → 模型端点
          → TxAgentToolAdapter → PS 工具 / 宿主主线程
          → AgentSession（工作上下文）
      → ConversationStore（完整归档）+ ConversationIndex（检索索引）
      → SystemPromptBuilder / Memory Stores（按需经验）
```

用户输入经 WebView2 消息桥进入 `SendAsync`，模型 SSE 先产生思考/正文，再产生完整工具调用；工具结果回灌模型，直到结束或停止。工具结果先加入 session，再触发完成事件和增量归档；一轮结束和正常关闭另有保存兜底。

`Core/AgentLoop.cs` 目前保留 `AgentOptions` 与默认提示词，不是可切换的旧循环。实际循环位于 `Core/Harness/AgentLoop.cs`。

## Agent Harness (TxAgent.Core)

**智能体 = 模型 + harness**。模型决定能力上限,harness 决定这些能力怎么被组织成有意义的动作——
工具注册、循环调度、上下文管理、执行沙箱、错误反馈。同一个模型换个外壳,实际能干的活差很多。

`TxAgent.Core` 是抽出来的**宿主无关内核**:不引用任何 Tecnomatix / CATIA 类型,
理论上换个 `IAgentHost` 实现就能驱动 CATIA 或别的 CAD 宿主。

### 分层

| 层 | 内容 | 换宿主要改吗 |
|---|---|---|
| `TxAgent.Core` | `AgentLoop` / `AgentSession` / `ITool` / `IAgentHost` / `ILlmClient` / `ToolRegistry` | 否 |
| 适配层 | `PsAgentHost` / `DeepSeekLlmClient` / `TxAgentToolAdapter` | 是 |
| 桥接层 | `HarnessAgentLoop` — 用旧 UI 表面驱动新内核 | 是 |

当前 UI 的 `BuildLoop` 始终构造 `HarnessAgentLoop`，没有 `UseNewHarness` 切换开关。协议、工具、记忆与界面已持续迭代，不再属于“只加适配、原文件完全不动”的接入阶段。

入口和事件契约详见 [Harness 接入说明](Core/Harness/README_Harness接入.md)。

### 内核提供的能力

- **错误回灌自修** — 工具失败不抛异常,错误文本连同修复线索回灌给模型。
  关键在于识别**内联失败**:`run_csharp` 编译失败、`probe_python` 脚本异常时并不抛异常,
  而是把 `编译失败：` / `== 执行失败 ==` 当正常返回值吐出字符串。
  不识别的话整套错误反馈机制形同虚设——模型会在同一个错上反复打转而 harness 毫无察觉。
- **换思路提示 + 熔断** — 同一工具连续失败第 3 次起,回灌文本追加"停下来换思路"提示
  (先只读查清真实签名 / 把大动作拆成最小一步 / 换工具),第 6 次熔断中止。
  阈值不能设太小:实测模型常在第 4~5 次才脱困,设成 3 会误杀本来能成功的任务。
- **上下文预算裁剪** — `AgentSession` 按 token 预算裁剪,`PinTaskGoal` 固定的任务目标永不裁掉。
  裁剪保证带 `tool_calls` 的 assistant 与其 tool 结果**成对丢弃**,否则会破坏配对导致 400。
- **回滚点** — 首次写操作前自动建立。Standalone 模式下利用 PDPS 的保存机制
  (每次保存新建文件、旧文件进回收站)作天然还原点;建立失败会拦下来问用户,不静默继续。
- **两阶段只读** — `ReadOnlyPhase = true` 时写工具压根不导出给模型,只能分析并输出变更清单。
- **消息出口清洗** — `Sanitize` 在每次发送前挡掉 API 不接受的消息:
  空 assistant(既无 content 又无 tool_calls)、找不到配对的孤立 tool 消息、null 参数。
  一条脏消息会让**之后每一轮**都 400,而且报错不会告诉你是哪条。

### 线程模型

工具默认经 `host.Invoke`(`SynchronizationContext.Send`)封送回 PS 主线程——
Tecnomatix API 非线程安全。但**阻塞等待用户交互的工具是例外**:

```csharp
public interface ITxOffUiThreadTool { }   // 标记接口,禁止封送
```

`ask_user` 实现它。否则会死锁:主线程卡在 `Send` 里等用户点击,
而用户的点击需要主线程的消息循环才能派发——整个 PS 冻结,连对话框关闭按钮都点不动。

---

## 多 LLM Provider

5 个内置 provider,**都走 OpenAI 兼容协议** (`POST /v1/chat/completions`):

| Provider | Base URL | 密钥存储 |
|---|---|---|
| DeepSeek | `https://api.deepseek.com` | DPAPI 加密 |
| Kimi (Moonshot) | `https://api.moonshot.cn` | DPAPI 加密 |
| Qwen (Dashscope 兼容模式) | `https://dashscope.aliyuncs.com/compatible-mode` | DPAPI 加密 |
| OpenAI | `https://api.openai.com` | DPAPI 加密 |
| Ollama (本地) | `http://localhost:11434` | 无 |

上述是传给构造函数的 Base URL；客户端追加 `/v1/chat/completions` 和 `/v1/models`，不要重复附加 `/v1`。内置模型清单仅是兜底，实际可用性以端点返回及账户权限为准。

### 默认思考与输出预算

- `UserPrefs.ReasoningEffort` 默认为 `low`，允许 `high` / `max`；非法值归一为 `low`。配置在构造循环时读取，修改后重新打开智能体或重建循环。
- 仅当端点主机为 `api.deepseek.com`、模型名以 `deepseek-v4` 开头时发送 `reasoning_effort`。其他模型/代理不发送该专有配置，保留服务端默认。
- 官方 V4 携带工具时回传历史 `reasoning_content`；其他请求默认排除。历史没有思考时不会编造补写。协议依据见 [DeepSeek 文档](https://api-docs.deepseek.com/guides/thinking_mode/)。
- DeepSeek V4、Kimi K2/K3、Qwen3 的单次输出上限为 12,288 tokens，其余分支为 8,192；这是推理与正文共用的上限，不是单独思考预算。`AgentOptions.MaxTokens=4096` 不是当前 Harness 实际使用的预算。
- 宿主默认最多 50 次模型往返；裸 `AgentLoopOptions` 默认 25，桥接层会覆盖。低强度不保证固定响应时长，也不放宽审批或验证要求。
- 提示词要求先用专用工具/API 查询、脚本对比/差集筛查，再最小变更、读回复核，减少手算和反复猜测；不要为纯闲聊或已有答案额外调用工具。

**核心机制**:

- **`DeepSeekClient.baseUrl` 参数化**,一个类通吃所有 provider
- **动态模型列表**:调 `/v1/models` 拿最新 model 列表,5 分钟节流缓存
- **Temperature 自动回退**:Kimi k2 / OpenAI o1 拒绝非默认 temperature 时,`LlmApiException` 里检测 `ShouldRetryWithoutTemperature`,去掉参数重试一次
- **DPAPI 加密**:`KeyStore.Load/Save(providerId)` 用 Windows DPAPI 加密到 `{plugin}\{provider}.key`,per-user unbind

**流式 SSE 解析**:`SendStreamOnceAsync` 用 `HttpCompletionOption.ResponseHeadersRead` 拿到 stream,逐行处理 `data: {...}` 帧,`content` 走 `AssistantDelta` 事件,`tool_calls` 累积后走 `ToolCalled`。

---

## 工具系统

内置工具按类别组织，实际暴露集合由注册表与 `ToolGate` 决定，不固定写死数量。默认启用 `doc/view/catia/knowledge`，`code/cee` 默认关闭，未分组的核心工具始终启用；保存的用户配置可覆盖默认值，切换工具组在新建对话时生效。

### 场景查询(只读)
`count_objects` / `list_children` / `list_operations` / `find_objects` / `list_types` / `inspect_type` / `inspect_object` / `get_object_location` / `check_reachability`

### 机器人 / 焊接
`check_robot_base` / `inspect_robot_kinematics` / `find_robot_for_op` / `query_collision_sets`

### 位置对齐(变更 · 需审批)
`scan_devices_z` / `align_devices_z` / `set_object_location` / `batch_rename`

### 视图 & 相机
- `capture_viewer_image` — SDK 原生 `GraphicViewer.GetImage`,纯 3D 视图无 UI 污染
- `set_camera_view` — front/back/left/right/top/bottom/iso/custom 八向
- `set_view_to_object` — 选中 + `GraphicViewer.ZoomToSelection`
- `screenshot_window` — GDI 兜底(截整个客户区,抓 UI 时用)

### 文档生成
- `export_docx` / `export_pptx` / `export_table` — 从零建,零配置
- `render_pptx_template` / `inspect_pptx_template` — 自定义模板 + 占位符替换

### CATIA & PS 复合对象
- `catia_read_tree` / `import_catia_tree_to_parts` — CATIA V5 COM 桥接
- `create_compound_resource` / `create_compound_part`

### API 知识库
- `api_lookup` — **写代码前的第一站**。反射当前进程已加载的 Tecnomatix 程序集,
  给出可直接照抄的完整签名,并标出已废弃成员和历史踩坑注解。
- `api_note` — 把试错发现的运行期行为记回去,下次查同一类型自动带出。

### 本地知识库
- `search_knowledge` / `read_knowledge` — 检索与读取用户自备的 md 资料
- `knowledge_status` / `knowledge_reindex` — 查看索引状态、重建向量索引

### 图像识别
- `analyze_image` — 看已上传的图片
- `analyze_viewport` — 截当前 3D 视口并识别，一步完成

### 源码工具（改别的插件）
- `open_workspace` — 指定项目根目录，之后读写都限定在此
- `code_search` / `code_outline` / `code_read` — 定位 → 看骨架 → 读那一段
- `code_edit` / `code_create_file` / `code_revert` — 精确串替换、新建、回滚
- `code_build` — MSBuild 编译并回传诊断

### 交互 & 记忆
- `ask_user` — 弹窗提问,五种形态:confirm / choice / multi_choice / input / **form(混合表单)**
- `save_snippet` / `list_snippets` / `get_snippet` / `find_snippet`
- `add_fact` / `list_facts` / `add_gotcha_correction` / `list_gotchas`
- `save_recipe` / `delete_recipe` / `list_recipes`
- `search_past_conversations` / `read_past_conversation`

### 通用兜底
- `run_csharp` — 在 PS 主线程同步执行 C# 5 代码,需审批
- `list_uploaded_files` / `read_uploaded_file`

### 工具接口 & 自动注册

```csharp
public interface ITxAgentTool
{
    string Name { get; }
    string Description { get; }
    bool IsReadOnly { get; }        // false → 走 ApprovalRequest
    JObject InputSchema { get; }    // JSON Schema
    string Execute(JObject input);
}
```

**`AutoRegisterTools`** 反射扫描 assembly 里所有 `ITxAgentTool` 实现,有公共无参构造就自动注册——**新工具加 .cs 文件即可,无需改 command 类**。

---

## API 知识库

**别去解析 `tx_dir` 的输出。** 它只给"名字 + 类型种类"两列,拿到 `AddObject / builtin_function_or_method`
这种粒度还是不知道参数是什么,探完还得再探一次 `__doc__`。而 `Tecnomatix.Engineering.dll`
本来就在同一个进程里加载着,直接反射它,能一次拿到完整签名、返回类型、属性有没有 setter、有没有标 `Obsolete`。

分两层:

| 层 | 内容 | 是否落盘 |
|---|---|---|
| `ApiIndex` | 反射得到的类型/成员/签名 | **否** —— 反射一次几十毫秒,永远和实际 DLL 一致,不存在版本漂移 |
| `ApiNotesStore` | 反射看不出来的**运行期行为** | 是 —— 这才是真正值钱、需要跨会话积累的部分 |

反射拿不到的例子:`TxJoint.Name` 的 setter 会抛异常、某 API 在 IronPython 下不可用、
调用前必须先做某步准备。这些只能靠踩坑积累,所以 `api_lookup` 把注解放在成员列表**最前面**——
模型读到签名前先看到"这个方法已废弃"。

签名做了可读化:`void AddObject(ITxObject child)` 而不是
`System.Void AddObject(Tecnomatix.Engineering.ITxObject)`,可以直接照抄进代码。

**实测效果**:改造前"把选中焊点移到 WeldOperation_1"这个任务,模型连续 9 次 `probe_python`
才摸清 API(其中 4 次连续失败全在 IronPython 类型访问上);接入 `api_lookup` 后压到 1~2 次查询。

---

## 记忆系统

当前采用分类 Markdown 经验库、JSON 对话归档和派生检索索引，二者职责分开。

### 存储介质与默认注入

```text
memory/
  facts/       偏好、API 事实、场景快照、流程经验
  gotchas/     错误签名、触发代码、确认后的正解
  snippets/    C# / Python 可复用片段
  pending/     待验证片段
  recipes/     脚本配方及参数声明
  knowledge/   文档与向量索引
```

`MarkdownDoc` 处理轻量 frontmatter。部分旧 JSON 由对应 Store 迁移成 Markdown，原文件保留为 `*.migrated`；不要推断所有旧格式都已自动迁移。`api_note` 的运行期行为注释目前在 `%APPDATA%/TxTools/TxAgent/api_notes.json`，不是 `memory/api-notes`。

`SystemPromptBuilder` 会话级缓存基础提示；开新/切换对话时失效。默认最多注入 **6 条 preference/api_fact + 5 条有正解的 gotcha**，不再把场景快照和未解决错误作为全局前提。历史经验有冲突时，以当前工具实测为准。

### `SnippetStore` — 代码片段库

- 支持 C# / Python、标签检索及成功/失败/未决归因；`get_snippet` 只是登记候选，不能算成功执行。
- 每次 `SendAsync` 按输入检索最多 3 个片段，临时注入本轮，结束后移除，不把整库塞进上下文。
- `PendingSnippetStore.EnableAutoPromotion=false`：重复执行三次不再默认晋升。验证结果和适用范围后再手动固化。
- 过期候选读取时只过滤；需要物理清理时使用可恢复的维护流程。

### `FactsStore` / `GotchasStore`

事实库可保留流程线索，但场景 ID、数量、坐标不是跨工程常量。事实的默认注入仅取偏好/API 类别，并对完全相同内容去重。错误经验由工具输出记录；只有明确正解才进入默认避坑清单，其他记录仍可用于诊断。

### `RecipeStore` — 配方

当前配方是 **脚本 + 参数声明**，由 `memory/recipes/*.md` 读取，支持配方侧栏与工具调用。参数值按当前场景绑定，不写入通用配方文件；运行统计不等同于所有场景都已验证。

旧 `recipes.json` 是多步骤工具编排格式，仍可保留作历史资料，但当前 `RecipeStore.All()` 不读取它，也不自动迁移为脚本配方。`Core/Recipe.cs` 是遗留模型文件，与现行 `RecipeStore.cs` 内类型重名，不能盲目通配编译所有源码。

### `ConversationStore` — 对话持久化

- 每对话一个 `conversations/{id}.json`，当前写入 `SchemaVersion=2`，紧凑序列化；旧格式仍可读取，不批量改写旧历史。
- 存储专用 resolver 保存模型返回的 `reasoning_content`；网络序列化按端点/模型决定回传，归档不能直接原封不动作为请求发送。
- 正常完成、取消、最终调用失败及只返回思考的响应均有保留路径。历史 UI 默认折叠思考；旧版本未保存的思考无法补回。
- 同目录临时文件写入并 flush 后原子替换，保留一个 `.json.bak`；主文件损坏时尝试读取备份，保存失败写日志。
- 列表优先读取头部元数据，避免为显示标题反序列化整个消息数组。仍保留单文件快照方式，非逐 token WAL。
- 工具完成事件在结果入 session 后触发；归档用消息对象身份去重，避免工作上下文裁剪后索引错位。

| 数据 | 用途 | 处理方式 |
|---|---|---|
| `_workingMemory` | 本轮模型上下文 | 可裁剪、摘要、临时注入 |
| `_fullHistory` | 完整交互归档 | 常规同步追加，不被摘要替换；显式撤销/删除另行处理 |
| `reasoning_content` | 模型返回的过程记录 | 保存供回看，不自动视为事实或成功结论 |

原子替换降低文件截断风险，不保证强杀、断电或 native 崩溃时最后的流式片段一定保存。

### `ConversationIndex` — 对话摘要索引

原 `search_past_conversations` 每次搜索都把**全部对话的完整 JSON** 读进内存逐条扫,
既慢又只能做关键词 `Contains`——"插入焊枪 CGR"搜不到当初用 `AddComponentsFromFiles` 描述的那次操作。

现在每个对话额外生成一份 `conversations/index/{id}.md`,几百字,语义密度高:

```yaml
---
title: 测试askuser
tools: [ask_user, save_recipe, api_lookup, run_csharp]
types: [TxDocument, TxWeldOperation, TxStudyLoadMode]
files: [CatiaBridge.cs]
turns: 6
---
## 1. 尝试将这些选中的焊点都移到 WeldOperation_1 中
- 工具: query_scene → probe_python → run_csharp
- 结论: 61 个焊点已移入
```

**工具名和 PS 类型名是最强的语义锚点**,检索时 `tools`/`types` 权重 3、标题 5、正文 1。
搜 `TxWeldOperation` 就能捞出所有碰过焊接操作的对话,这是纯正文关键词做不到的。

摘要用规则拼装，**不调 LLM**，不产生额外模型调用费用，但仍有文件读写和处理开销；消息数未变时可跳过重写。

检索是两段式:`search_past_conversations` 只扫索引,拿到 id 后用
`read_past_conversation(conv_id, query?)` 按需读原始 JSON 取细节。

---

## 本地知识库

把 `.md` 丢进 `memory/knowledge/`，重开对话即生效，没有导入步骤。

### 目录常驻 + 按需取节

三种做法的取舍：

| 做法 | 问题 |
|---|---|
| 整篇塞系统提示词 | 文档一大就吃光上下文，且大部分内容与当前问题无关 |
| 只给检索工具 | **模型不知道有这东西，压根想不到去搜** —— 知识库建了没人用 |
| **目录常驻 + 按需取节** | 目录几百 token，模型看得见有什么；细节按需取 |

目录是静态的，常驻系统提示词对 prefix 缓存友好，这部分开销可以忽略。

### 分节

按 `#` / `##` / `###` 切块，每节记录**祖先路径**（面包屑）。
面包屑不是可选的：「已知问题」这种标题单看毫无信息量，
带上「工艺设计器接口」之后模型才知道它在讲什么，检索时也能命中上层主题词。
面包屑同时进入嵌入文本。

分节器还做了三件事，都是被实际文档逼出来的：

- **超长节按段落再切**（`MaxSectionChars` 默认 3000）—— 一节 100KB 的表格嵌入必被截断
- **剥离锚点/HTML/内部跳转链接** —— 纯导航结构，进向量只会稀释语义
- **跳过目录索引类小节** —— 它包含全文所有标题，任何关键词都能命中，
  会稳定挤掉真正有内容的小节。这是大部头知识库最典型的检索污染源

### 混合检索

向量擅长语义（「焊枪装在哪个法兰」↔「TCP 与 Toolframe 的挂接关系」），
但对精确串很弱 —— 搜 `TxWeldOperation`、`CS1061` 这类型号/API 名，关键字更准。
技术文档里两类查询各占一半，所以两路都跑，用 **RRF（Reciprocal Rank Fusion）** 融合：

```
score = Σ 1 / (60 + 该路名次)
```

只看名次不看分数。两路分值量纲完全不同（余弦相似度 vs 关键字计数），
直接加权是错的，归一化又易被离群值带偏。RRF 规避了这个问题，
而且**没有需要调的权重**。命中结果标 `vector` / `keyword` / `both`，调效果时一眼看出哪路在起作用。

### 向量化

`IEmbedder` 两套实现，接口相同可随时切换：

| | 云端 `DashScopeEmbedder` | 本地 `OnnxEmbedder` |
|---|---|---|
| 部署 | 零部署，复用百炼 key | 需放 model.onnx + vocab.txt |
| 成本 | 0.0007 元/千 token | 免费 |
| 模型 | text-embedding-v4 | bge-small-zh-v1.5（24M / ~90MB） |

三个做错了不报错但效果会差的细节：

- **向量必须 L2 归一化** —— 归一化后余弦相似度即点积，省掉每次算模长
- **嵌入文本要带标题和面包屑** —— 只嵌正文的话主题信息就丢了
- **换模型必须重建索引** —— 不同模型向量空间不通用，`EmbedderId` 不匹配时自动放弃向量路径

按内容 hash 增量记账，改一节不会触发全量重算。

本地 ONNX 路线有两个坑：分词器要自己写（ONNX Runtime 不含分词，
而引 BERT 分词器包又是一次版本冲突风险）；池化方式选错**不会报错**，
只是相似度整体失真 —— bge 用 CLS，m3e/text2vec 用 Mean。

---

## 图像识别

DeepSeek 系列不支持视觉。两条路：

| | 委托模式（本方案） | 直连（主模型换成 kimi-k3） |
|---|---|---|
| 图片在上下文 | 不驻留，只留文字描述 | 驻留，**每轮都要重发** |
| prefix 缓存 | 保持 | 换模型即重建 |
| 成本 | 主对话缓存价 + 看图约 1 分/张 | 全程按视觉模型价 |

委托的省钱幅度比表面更大：**每次委托都是全新的短上下文**，
而图片留在主对话里是每轮重发。

`ChatMessage.Content` 保持 `string` 属性不变，新增 `ContentParts`；
序列化时 **parts 为空发字符串、非空才发数组**。纯文本路径逐字节不变 ——
否则所有 provider 兼容性都要重测，prompt 前缀也会整体变化。

`ModelRouter` 按能力选模型，三条原则：主对话模型永远听用户的，
路由只管主模型干不了的活；找不到候选返回 null 让上层报错，不静默换模型；
没配 key 的 provider 直接跳过。

默认值都调成省钱档：provider 用千问、`detail=low`、视口截图 1024×576。
`high` 会把图切成多 tile 分别编码，通常贵几倍，而判断题 `low` 足够。

---

## 源码工具

用 TxAgent 改**其他插件**的源码。核心结论和 harness 那次一样：
**别整文件读，也别整文件改**。

### 读：三级递进

`code_search` 定位 → `code_outline` 看骨架 → `code_read` 读那一段。

一个 3000 行的 `.cs` 整读约 4 万 token，读两个文件上下文就废了；
骨架通常只有百来行。骨架解析用正则 + 花括号计数，**没引 Roslyn** ——
那是重依赖，而且遇到新语法特性时正则至少不会整个罢工。

### 改：精确串替换

`old_string` 必须在文件里**恰好出现一次**，0 次或多次一律拒绝。
让模型输出整个新文件有三个问题：几万 token 又慢又贵；
它会在无关处悄悄改动，review 时看不出来；长输出可能被 `max_tokens` 截断，
写出半个文件直接毁掉源码。

处理了两个容易忽略的细节：换行符归一化后再匹配（模型很难保证输出 `\r\n`），
保留原文件编码与 BOM（避免改一行把整个文件编码换掉）。

### 验：编译反馈闭环

**这是整套工具里最重要的一个。** 接上之前模型改 C# 一次成功率大概五成，
接上之后通常两三轮收敛 —— CS 错误码 + 文件 + 行号是极强的信号。
只回解析出的诊断行，不回整个构建日志（msbuild 一次输出几千行）。

### 两个编译器别混淆

| | `run_csharp` 沙箱 | `code_build` |
|---|---|---|
| 编译器 | `CSharpCodeProvider`（CodeDom） | MSBuild → Roslyn |
| 语法上限 | **C# 5** | 以目标工程 LangVersion 为准；当前宿主为 C# 8.0 |

系统提示词里那堆「C# 5 语法陷阱」**只对 `run_csharp` 成立**。

`FindMsBuild` 优先用 `vswhere.exe` 定位，并**故意排除**
`Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe` ——
那是 MSBuild 4.0，会退回传统编译器，遇到 C# 6+ 语法报一堆误导性的语法错误。
宁可明确报"找不到 MSBuild"，也不要给出会让人查错方向的错误。

---

## 前端 (WebView2 + chat.html)

**技术选型**:WebView2 (Chromium) 嵌入 WinForm,主体为 `UI/chat.html`，配方侧栏另有 `recipe-sidebar.css` / `recipe-sidebar.js`，由宿主嵌入资源加载。

**核心 UI 特性**:

- **流式 markdown 渲染**:`renderMarkdownStreaming` 检测未闭合 ` ``` `,自动补尾,避免流式过程"代码块闪现"
- **合并气泡**:一整轮 AI 回复共用**一个"助手"标签**和大气泡容器,内部按时序交错文字段和工具卡片(`state.currentAssistantMsg` 生命周期到 `onBusy(false)` 才结束)
- **工具卡片折叠**:默认 `collapsed`,头部 `▸/▾` 箭头指示,状态徽章 `[已完成]/[失败]`
- **思考记录**: SSE 推送 `reasoningStart/Delta/End`；保存后重开历史可折叠查看模型实际返回的内容。
- **思考动画**:空档期(`onCloseAssistant` 后 / `onToolResult` 后)都会重新显示,覆盖多轮 tool_call 之间的等待
- **侧滑 drawer**:历史对话 + 可用工具面板都用同款 drawer(右滑,420px),复用 `.drawer-head` 样式
- **HTML modal**:审批(allow/deny)+ 用户提问(五种形态),走 `TaskCompletionSource` 桥接同步阻塞
- **一体化输入区**:附件、文本框、上传、权限档位、token 用量、发送键全部收在同一张卡片里,
  聚焦时描边加在卡片上。权限按钮按风险配色(询问=灰 / 半自动=琥珀 / 全自动=红底),扫一眼知道放行到什么程度
- **上下文用量弹层**:百分比 + 分段进度条 + 按「系统提示词 / 工具定义 / 对话消息」三项拆分。
  分项是按字符数估算的(API 只返回总 `prompt_tokens`,不会告诉你其中多少是工具定义),
  用途是让你知道该压缩哪一块——比如工具定义占了 12%,就说明该精简工具描述了
- **附件回显**:C# 把文件摘要拼进用户消息正文再发给模型,重开对话时若直接渲染就是几千字的大气泡。
  前端解析 `[已附加文件]` 前缀还原成可折叠的附件卡片,正文只留用户真正打的字

**C# ↔ JS 通信**:

- C# → JS:`WebView.CoreWebView2.PostWebMessageAsString(json)` → JS 侧 `dispatchMessage(json)` switch case
- JS → C#:`window.chrome.webview.postMessage(JSON.stringify(msg))` → C# `WebMessageReceived` 事件

---

## 阻塞式用户交互 (Approval + AskUser)

工具在**后台线程**(Task.Run 线程池)执行,需要用户交互时用 `TaskCompletionSource<T>` 桥接同步等待:

```csharp
public bool AskApproval(...) {
    var tcs = new TaskCompletionSource<bool>();
    _pendingApproval = tcs;
    PostJs(new { type = "askApproval", ... });   // 显示 modal
    return tcs.Task.Result;                       // 后台线程阻塞
}

// UI 线程收到 approvalResult 消息 →
public void ReleasePendingApproval(bool allow) {
    _pendingApproval?.TrySetResult(allow);        // 后台线程解除阻塞
}
```

**关键安全前提**:工具跑在后台线程,不是 UI 线程——阻塞它不冻结界面。

新 harness 下这个前提一度被打破:`TxAgentToolAdapter` 为了 PS SDK 的线程安全,
把**每个**工具都用 `host.Invoke` 封送回主线程,`ask_user` 也不例外,于是主线程卡在
`Send` 里等用户点击、而点击又需要主线程的消息循环才能派发,整个 PS 冻死。
修法是给不碰 SDK 的工具加标记接口 `ITxOffUiThreadTool` 跳过封送。

宿主侧的等待也必须是这个结构:**`BeginInvoke` 异步投递显示 + 在调用线程等**。
写成 `Invoke` 同步调用后在 UI 线程等,同样死锁。

**`ask_user` 工具**用同款模式,AI 可主动弹窗问 confirm / choice / input,用户点击/输入即返回,省去"AI 说话→用户到输入框打字→按 Enter"三步。

---

## PS 视口 & 相机

关键 SDK API(从对话摸索得到):

```csharp
// 拿主视口
var viewer = TxApplication.ViewersManager.GraphicViewer;

// 抓 3D 视图 (WeldAnnotator 用的 API,不受窗口遮挡/DPI 影响)
Bitmap bmp = viewer.GetImage(size, transparent);   // size=Empty 会 NRE,须先读 viewer.ViewRectangle

// 相机读写 (通过 ITxGraphicDisplayer 接口)
TxCamera cam = ((ITxGraphicDisplayer)viewer).CurrentCamera;
((ITxGraphicDisplayer)viewer).CurrentCamera = new TxCamera(refPt, camPos, upVec);

// Zoom To Selection (命令 ID 必须带前缀 GraphicViewer.)
TxApplication.CommandsManager.ExecuteCommand("GraphicViewer.ZoomToSelection");
```

**`PsViewerHelper`** 封装这些原始 API,`CaptureToPng` 处理 Size.Empty 兜底 + `Application.DoEvents()` 让 SDK 完成异步渲染再抓图。

**`SetCameraViewTool`** 提供 8 种视角:

- 6 正交向 (front/back/left/right/top/bottom)
- iso (等轴)
- custom (三向量)

焦点解析多层兜底:`target` 参数 → 当前相机 ReferencePoint(非零时)→ ActiveSelection 首个对象 AbsoluteLocation → 原点。

---

## CATIA V5 集成

**技术方案**:反射 + `Type.InvokeMember` 显式调用 COM,**不用 `dynamic`**。

**为什么不用 dynamic**:`run_csharp` 环境(CodeDom 编译)不带 `Microsoft.CSharp.RuntimeBinder`,dynamic 用不了;主项目 dll 编译能用 dynamic,但代码风格保持一致更好——**同样的 CATIA 操作在两个环境里可以互通**。

**关键坑**(`CatiaBridge` 内已封装):

```csharp
// COM 集合的 Item(i) 必须用 InvokeMethod,不是 GetProperty
// (对话里踩过: GetProperty 抛 "Exception has been thrown by the target of an invocation")
var child = collection.GetType().InvokeMember(
    "Item", BindingFlags.InvokeMethod, null, collection, new object[] { i });
```

**`import_catia_tree_to_parts`** — 一键把 CATIA Product 树映射为 PS `TxCompoundPart` 空集合层级,TypeName=PartNumber,便于后续按 PartNumber 匹配零件填入。

---

## 文档生成 (docx/xlsx/pptx)

**统一原则**:三个都**从零建**、内置骨架、无需外部模板。用户体验一致。

### docx / xlsx
- `DocumentFormat.OpenXml` 直接构造 `WordprocessingDocument.Create` / `SpreadsheetDocument.Create`
- 骨架用 SDK API 快速搭

### pptx (技术亮点)
从零建 pptx 需要 SlideMaster + SlideLayout + Theme + N 个 Slide 各自的 XML 和关系,代码量大。**解决**:

**内嵌最小空白 pptx** 到 `BlankPptxData.cs`:
```
28 KB 空白 pptx → gzip 压缩 22 KB → base64 编码 29 KB → 硬编码常量
运行时: Convert.FromBase64String → GZipStream 解压 → 写目标路径 → 追加 slide
```

用户零配置,导出体验跟 docx 完全一致。

### PPT 模板引擎
- **`inspect_pptx_template`** — 扫描模板,列出所有 `{{TEXT}}` 占位符和 `IMG_xxx` 形状名(跨 Run 兼容——PowerPoint 常把 `{{TITLE}}` 拆成 `{{TIT` + `LE}}`)
- **`render_pptx_template`** — 克隆模板 slide N 次 → 替换文本 → 替换 `IMG_xxx` 形状为图片(继承原形状的 Transform2D 位置和尺寸)

---

## `run_csharp` 沙盒

**用途**:AI 兜底调用 PS SDK 的通用出口。

**执行方式**:`CSharpCodeProvider` 运行时编译成动态 assembly,反射调 `Run` 方法。执行在 PS 主线程同步(PS 会短暂无响应)。

**代码结构**(AI 生成的):
```csharp
public static object Run(Action<string, string> log)
{
    // AI 代码在这
    return (object)null;
}
```

**环境限制**(CodeDom 引用不全):

| 想用的 | 状态 | 替代 |
|---|---|---|
| `dynamic` | ❌ | 反射 `InvokeMember` |
| `System.IO.Packaging` | ❌ | 主项目 dll 里的封装工具 |
| `System.IO.Compression.ZipArchive` | ❌ | 同上 |
| `System.Xml` 全部 | ❌ | 同上 |
| C# 5 以上语法 (`$""` / `?.` / `=>` 表达式体) | ❌ | 老 C# 5 语法 |

**语法陷阱**(`DefaultSystemPrompt` 里已固化提示):
- 三元 null 必须转型:`flag ? (string)null : val`
- 无字符串插值:用 `+` 拼接
- 无 `?.`:`if(obj != null) { obj.Prop }`
- `TxSelection` 无索引器:`sel.GetItems()[0]`

---

## 文件解析 (`FileParserService`)

**30+ 扩展名支持**:

- **表格**:xlsx / csv / tsv → 结构化摘要(行列数、sheet 列表、前 200 行预览)
- **文档**:docx / pptx / xml / html → 结构提取
- **代码**:cs / py / js / ts / java / go / rs / cpp / ... → **语言感知摘要**
- **配置**:yml / toml / ini / env / properties → 纯文本读取

**代码文件的语言感知摘要**:

- 识别单行注释符(`//` / `#` / `--` / `REM`)→ 统计有效行数
- 扫描顶部 60 行 → 提取 `using`/`import`/`#include`/`require` 依赖列表
- 语言相关正则 → 提取顶层符号(`class` / `def` / `function` / `fn`)
- 前 30 行预览

**`read_uploaded_file`** 支持字符切片 + **行切片**(代码文件推荐):
```
read_uploaded_file(file_id="...", line_from=45, line_to=80)
→ 带行号输出:
   45: public static void CaptureToPng(...)
   46: {
   47:     var viewer = GetViewer();
```

---

## PS SDK 踩过的坑 (已固化到系统 prompt)

按频率排:

| 坑 | 正解 |
|---|---|
| `TxDocument.Name` 不存在 | `doc.CurrentStudy.Name` |
| `doc.Viewers` 不存在 | `TxApplication.ViewersManager.GraphicViewer` |
| `GetAllDescendants` 接口没暴露 | 只在具体类;`run_csharp` 里用 dynamic 或反射 |
| `TxTypeFilter(接口类型)` 返回空 | 传 `null`(全部)或具体类 |
| PS 命令 ID 无前缀报错 | 必须 `GraphicViewer.ZoomToSelection`(模块.命令) |
| `TxSelection.Add(list)` 不存在 | `SetItems` / `AddItems` |
| `TxRobot.Parent` 不存在 | `((ITxObject)o).LogicalParent` |
| `viewer.GetImage(Size.Empty)` NRE | 先从 `viewer.ViewRectangle` 拿真实尺寸 |
| `CreateSeamMfgFeature` 后 psz 保存失败 | `undoMgr.StartTransaction()` 包裹创建流程 |
| CATIA 集合 `Item(i)` | `BindingFlags.InvokeMethod`,不是 `GetProperty` |

---

## 可靠性

**对话持久化保护**：

1. 用户输入、工具结果及一轮结束的历史同步触发保存；不承诺每个 token 立即落盘。
2. `FormClosing` 和 .NET 未处理异常提供尽力而为的兜底。
3. 快照写入临时文件并 flush，再原子替换，保留一份备份。
4. 上下文裁剪不应覆盖完整归档，工具结果与调用保持配对。

强杀/native 崩溃可能跳过托管回调；断电和磁盘故障也不能保证最后一次提交成功。备份是恢复辅助手段，不是对工程、CATIA 文档或外部文件操作的统一撤销保证。

---

## 部署 & 目录

本仓库发布的是 **Agent 源码子树**，没有独立构建工程，也不包含 Siemens SDK、CATIA 互操作程序集或完整 TxTools 依赖模块。不要把仓库直接当作可独立编译的插件包。

已验证集成路径是在完整 TxTools 工程中编译（Windows、.NET Framework 4.8、Visual Studio/MSBuild）。宿主还依赖 PS SDK、公共 UI、导出等模块；具体引用及包版本以宿主 `TxTools.csproj` 为准，不在此硬编码成通用安装要求。

维护环境部署位置为 `{PS安装目录}/eMPower/DotNetCommands/TxTools/`。更新 DLL 前保存工程并关闭所有相关 PS 进程，备份原 DLL/PDB；界面资源内嵌于 DLL，改 HTML 后也要重新构建。

**运行状态**（插件目录优先；具体存储的回退位置见对应 Store）：

- `{provider}.key`：Windows DPAPI 加密 API Key。
- `prefs.json`：provider/model/ReasoningEffort/审批模式/工具组。
- `conversations/{id}.json` 及 `.json.bak`：对话和上一个快照。
- `conversations/index/{id}.md`：派生检索摘要。
- `memory/{facts,gotchas,snippets,pending,recipes,knowledge}/`：分类经验/文档。
- `recipes.json`、`*.migrated`：历史资料，不应被误认为当前活跃脚本配方。
- `maintenance-backup/<时间>/`：维护归档与恢复清单。
- `audit.log`：运行审计。
- `%TEMP%/TxTools.Agent/uploads/{convId}/`：附件暂存。
- `%APPDATA%/TxTools/TxAgent/api_notes.json`：运行期 API 注释。

Newtonsoft.Json、WebView2、Open XML、IronPython/嵌入相关组件的实际依赖以项目引用和宿主加载结果核对，尤其注意 PS 已加载程序集的版本绑定。仓库 `.gitignore` 排除构建产物、密钥、对话和运行记忆。

具体维护及测试步骤见 [维护说明](maintenance/README.md)。

---

## 已知限制

- **`run_csharp` 无 `dynamic`** — 用反射代替(`CatiaBridge` 就是这个风格)
- **`GraphicViewer.GetImage` 是同步阻塞** — 抓大图(4K+)时 PS 视觉短暂无响应
- **对话历史无跨设备同步** — 只在本机 `{plugin}\conversations\` 下
- **回归范围有限** — 已有 24 项离线存储/协议/循环测试，不能替代真实 PS/CATIA 场景测试或端点兼容验证
- **思考可用性取决于模型** — 支持真实思考流与归档；模型未返回的内容无法补写，强杀前尚未归档的片段仍可能丢失
- **摘要索引是规则拼装** — 不调 LLM，语义概括能力有限；没有额外模型费用，但仍有 I/O 开销
- **上下文分项用量是估算值** — 按字符数折算,非 API 精确计数
- **知识库检索质量取决于小节标题** — 标题写「约定三」而不是「焊枪坐标系约定」,
  向量和关键字都救不了
- **百炼代理版模型的 function calling 不可靠** — 同名模型在不同 provider 上表现不同,
  主对话建议走官方端点,代理版留给视觉/嵌入
- **`code_build` 需要装 VS 或 Build Tools** — 只有 .NET Framework 自带的 MSBuild 不够用

---

## 扩展性设计的三个层次

**Layer 1 — 加工具**:写个 `ITxAgentTool` 实现,`AutoRegisterTools` 自动挑到,重启插件即可用。

**Layer 2 — 配方 (Recipe)**:经验证的脚本与参数声明，保存为 Markdown，可由侧栏或工具调用；与历史 JSON 步骤格式区分。

**Layer 3 — 记忆学习**:代码执行形成待定候选，重复不自动视为成功；验证后固化片段，按任务检索。错误正解通过 `add_gotcha_correction` 补充，按筛选与数量上限注入。

---

## 一些经验教训

- **不要用 `screenshot_window` 抓 3D 视图** — 那是整个客户区(含 UI 面板)。用 `capture_viewer_image` (SDK `GetImage`)。
- **相机操作命令是异步的** — 调完 `ZoomToSelection` 立刻 `GetImage` 会撞未完成的渲染管线 NRE。`Application.DoEvents()` 一下。
- **PS 里创建的 mfg feature 必须在 `TxUndoTransactionManager.StartTransaction/EndTransaction` 里** — 否则 psz 保存失败("Failed to save data to the specified file location")。
- **`TxTypeFilter(接口类型)` 返回空集合** — SDK 内部只按具体类型匹配,接口传进去等于空过滤器。
- **写 `run_csharp` 前必须扫一遍系统 prompt 末尾的 Gotcha 清单** — 省下一次编译失败就省几千 token。
- **文档 API 上的 "MUST be null" 通常意味着"你调错重载了"** — 不是要你额外做什么。
- **`JToken.ToString(Formatting)` 在本环境必崩** — 编译期与运行期 Newtonsoft 签名不一致。
  紧凑序列化一律用 `JsonConvert.SerializeObject(token)`,缩进用无参 `token.ToString()`。
  根治办法是把引用对齐 PS 自带的那份 DLL,而不是靠"记得别写那个重载"。
- **模型偶尔返回"空内容 + 无 tool_calls"** — 上下文被撑爆时常见。这条消息一旦进了历史,
  下一轮原样发回去就是 `400 Invalid assistant message`,而且**之后每一轮都会 400**,
  报错还不告诉你是哪条。要在入会话和发送出口两处都挡。
- **工具的"内联失败"必须显式识别** — `run_csharp` 编译失败时不抛异常,而是把 `编译失败：`
  当正常返回值吐出来。不识别的话,错误回灌、失败计数、熔断三套机制会全部静默失效,
  表面上一切正常,实际模型在原地打转而框架毫无察觉。
- **连续失败别急着熔断,先给"换思路"提示** — 实测模型脱困靠的是改变策略
  (从猜 API 转向先探查),而不是在同一份代码上继续微调。阈值设成 3 会误杀本能成功的任务。
- **归档和工作记忆是两个东西** — 前者只增不改,后者可压缩可重建。混用会把原始对话物理销毁。
- **模型不知道的东西,框架帮不上** — harness 能让模型更快意识到自己错了并换策略,
  但消不掉"不知道 PDPS 的 IronPython 里 `typeof` 不可用"这个知识缺口。
  真正把 9 次探查压到 1 次的是 API 知识库,不是更好的循环。
- **静默的错误答案比明确的失败危险得多** — 这个坑连踩三次:
  同名对象取第一个、模糊匹配取第一个、内联失败当成功。
  最典型的一次是 `read_knowledge` 的模糊匹配 —— `#TCP` 返回了「TCPF Speed」,
  看起来像成功,模型就基于错的内容继续推理,连猜二十几轮。
  如果当时直接报"没有这个小节",它两轮就会停下来说工具坏了。
  **凡是"命中多个"的场合,一律返回候选列表让调用方重新指定,不要替它选。**
- **推理模型的思考链计入输出预算** — `max_tokens` 给小了会在思考中途被截断,
  返回 content 和 tool_calls 全空,表现为"任务莫名结束"。
  预算给大不花钱,只按实际生成量计费。
- **别让模型心算** — 手工展开旋转矩阵既慢又容易错,还会把输出预算烧光。
  能写进 `probe_python` 的计算就别在思考里做。
- **平台白名单管的是调用,不是列表** — 百炼 `/v1/models` 返回全量目录,
  业务空间授权只影响能不能调。客户端必须自己过滤,
  否则下拉里全是 embedding/rerank/日期快照和不支持工具调用的小模型。
