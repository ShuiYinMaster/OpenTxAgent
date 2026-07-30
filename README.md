# TxTools.Agent (TxAgent)

嵌入 **Siemens Tecnomatix Process Simulate 2402** 的 AI 助手插件——通过 LLM 调用 30+ 内置工具查询、操作 PS 场景,并把踩过的坑固化成记忆持久保留。

- **平台**:.NET Framework 4.8 / C# 7.3
- **运行环境**:作为 PS 插件加载到 PS 进程内
- **仓库**:[ShuiYinMaster/TxTools](https://github.com/ShuiYinMaster/TxTools)

---

## 快速定位

| 我想…… | 看这里 |
|---|---|
| 加新工具 | [工具系统](#工具系统) → 实现 `ITxAgentTool` 即可,`AutoRegisterTools` 反射注册 |
| 调 LLM | [多 LLM Provider](#多-llm-provider) → `DeepSeekClient` 是 OpenAI 兼容 |
| 改 UI | [WebView2 + chat.html](#前端-webview2--chathtml) → 单文件 HTML+CSS+JS |
| 加记忆条目 | [记忆系统](#记忆系统) → Snippet/Fact/Gotcha,一物一 Markdown 文件 |
| 换循环引擎 | [Agent Harness](#agent-harness-txagentcore) → `TxAgentForm.UseNewHarness` 一个常量切换 |
| 查 PS API 签名 | [API 知识库](#api-知识库) → `api_lookup` 直接反射,不用再 probe |
| 生成文档 | [文档生成](#文档生成-docxxlsxpptx) → docx/xlsx/pptx 均从零建 |
| 截图/相机 | [PS 视口 & 相机](#ps-视口--相机) → `capture_viewer_image` + `set_camera_view` |

---

## 架构总览

```
                    ┌─────────────────────────────────┐
                    │      TxAgentForm (WinForm)      │
                    │   ┌───────────────────────┐     │
                    │   │   WebView2 (Chromium) │     │
                    │   │      chat.html        │     │
                    │   └───────────────────────┘     │
                    └─────────────┬───────────────────┘
                                  │ postMessage / PostJs
                    ┌─────────────┴───────────────────┐
                    │           IAgentLoop            │
                    │  UI 只依赖接口,引擎可整体替换   │
                    └──┬───────────────────────────┬──┘
                       │                           │
          ┌────────────▼──────────┐   ┌────────────▼─────────────┐
          │  AgentLoop (旧·自研)  │   │  HarnessAgentLoop (新)   │
          └────────────┬──────────┘   └────────────┬─────────────┘
                       │                           │ 驱动
                       │                ┌──────────▼──────────────┐
                       │                │  TxAgent.Core.AgentLoop │
                       │                │  宿主无关的 harness 内核 │
                       │                └──────────┬──────────────┘
                       │                           │
                    ┌──┴───────────────────────────┴────────────┐
                    │                                           │
                       │             │            │
              ┌────────▼──────┐ ┌───▼────┐ ┌────▼─────┐
              │ DeepSeekClient│ │ Tools  │ │ Memory   │
              │ (LLM HTTP)    │ │ (30+)  │ │ Stores   │
              └───────────────┘ └───┬────┘ └──────────┘
                                    │
                            ┌───────▼───────┐
                            │ Tecnomatix    │
                            │ Engineering   │
                            │ SDK           │
                            └───────────────┘
```

**关键路径**(用户发消息 → AI 回复):

1. `chat.html` `postMessage` → `TxAgentForm.dispatchMessage`
2. → `_loop.SendAsync(userText)`(`_loop` 是 `IAgentLoop`,新旧引擎二选一)
3. → `DeepSeekClient.SendStreamAsync`,SSE 分片实时回调
4. LLM 返回工具调用 → 在后台线程执行,需要 PS SDK 的封送回主线程
5. 结果通过事件回调 → 前端流式渲染
6. 每步 history 变化 → `SaveCurrent` 增量落盘 + 重建 Markdown 摘要索引

---

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

现有 30+ 工具、`DeepSeekClient`、`PsContext`、`AuditLog` **全部不动**,
只在外面套三层适配器。UI 只认 `IAgentLoop` 接口,切换引擎改一个常量:

```csharp
// Agent/UI/TxAgentForm.cs
private const bool UseNewHarness = true;
```

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
| DeepSeek | `https://api.deepseek.com/v1` | DPAPI 加密 |
| Kimi (Moonshot) | `https://api.moonshot.cn/v1` | DPAPI 加密 |
| Qwen (Dashscope 兼容模式) | `https://dashscope.aliyuncs.com/compatible-mode/v1` | DPAPI 加密 |
| OpenAI | `https://api.openai.com/v1` | DPAPI 加密 |
| Ollama (本地) | `http://localhost:11434/v1` | 无 |

**核心机制**:

- **`DeepSeekClient.baseUrl` 参数化**,一个类通吃所有 provider
- **动态模型列表**:调 `/v1/models` 拿最新 model 列表,5 分钟节流缓存
- **Temperature 自动回退**:Kimi k2 / OpenAI o1 拒绝非默认 temperature 时,`LlmApiException` 里检测 `ShouldRetryWithoutTemperature`,去掉参数重试一次
- **DPAPI 加密**:`KeyStore.Load/Save(providerId)` 用 Windows DPAPI 加密到 `{plugin}\{provider}.key`,per-user unbind

**流式 SSE 解析**:`SendStreamOnceAsync` 用 `HttpCompletionOption.ResponseHeadersRead` 拿到 stream,逐行处理 `data: {...}` 帧,`content` 走 `AssistantDelta` 事件,`tool_calls` 累积后走 `ToolCalled`。

---

## 工具系统

**30+ 内置工具**,按类别:

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

四类知识 + 对话归档 + 一层跨对话检索。

### 存储介质:Markdown 而非 JSON

知识类存储从 JSON 换成了**一物一 Markdown 文件**:

```
memory/
  snippets/scan_physical_root_for_robots.md
  gotchas/CS1061_TxDocument_FullPath.md
  facts/用户偏好复用现有工具而非run_csharp.md
  api-notes/TxWeldOperation.md
```

换的理由:

- **可读可改** —— snippet 存的是 C# 代码,JSON 里是 `"var doc = ...\r\n  foreach ..."` 这种转义地狱;
  MD 里是围栏代码块,能直接看、直接改、直接复制。
- **Git 友好** —— JSON 里一条记录是一整行长字符串,改一个字符 diff 显示整行重写;MD 逐行 diff。
- **并发与体积** —— 写一条只动一个小文件,不用整份读-改-写。
- **注入省 token** —— 内容可原样拼进 prompt,不必反序列化后再格式化。

`MarkdownDoc` 是自己写的极简 frontmatter 解析器(五十行,零依赖)。
**没有引 YamlDotNet** —— 多一个第三方程序集就多一处和 PS 自带 DLL 撞版本的风险。

各 Store 的**公开 API 完全不变**,只是底下换了存储介质,所以调用点一行都不用改。
首次访问时若 MD 目录为空且找得到旧 JSON,自动逐条迁移,旧文件改名成 `*.migrated` 而非删除。

`RecipeStore` 保留 JSON —— 纯结构化步骤数据,没有代码块也没有散文,MD 化收益接近零。


### `SnippetStore` — 代码片段库
- 存 `run_csharp` 里跑通过的可复用代码
- **语义标签自动提取**:ExtractTags 扫描代码里的 API 关键词(`TxRobot` / `WeldPoint` / `AbsoluteLocation` 等)生成标签
- **相关性检索**:每轮 `SendAsync` 用当前 userText 检索 Top-3 最相关片段,作为 system 消息注入本轮(不进 history),仅本轮有效
- 使用计数:`get_snippet` 时 `IncrementUsage`,越用越靠前

### `FactsStore` — 事实/偏好
- 跨对话保留的常量(用户偏好、场景固定值、验证过的 SDK 事实)
- 每轮 system prompt 头部注入 Top-10

### `GotchasStore` — 踩坑记录
- 签名(问题模式) + 正解
- `run_csharp` 编译失败自动落库
- 每轮 system prompt 末尾注入 Top-15

### `RecipeStore` — 配方
- 用户可视化定义"多步流程 → 单个新工具"
- `RecipeTool` 是通用 wrapper,实际每次执行时按 recipe 定义的 step 序列调其他工具
- 加载时**注册成正式工具**,AI 视角跟内置工具无差

### `ConversationStore` — 对话持久化
- 每对话一个 JSON 文件在 `{plugin}\conversations\{id}.json`。
  **这一份必须保持 JSON** —— 它是协议原样存档,要能无损重放回 API:
  `tool_call_id` 配对、`arguments` 本身是 JSON 字符串、role 枚举、消息顺序。
- **增量保存**:`HistoryChanged` 事件在每次 `_fullHistory.Add` 后 raise → `SaveCurrent` 立即写盘
- **崩溃兜底**:`FormClosing` + `AppDomain.UnhandledException` 双保险
- PS 硬崩溃最多丢当前正在执行的**那一个**工具

**归档与工作记忆必须分开**:

| | 用途 | 可否压缩 |
|---|---|---|
| `_workingMemory` | 喂给 LLM 的上下文,可注入临时内容、可重建 | 可 |
| `_fullHistory` | 归档,`SaveCurrent` 落盘的就是它 | **只增不改** |

两者混用会出严重事故:harness 早期版本用压缩后的工作记忆整份覆盖归档,
于是每压缩一次,磁盘上的原始对话就被摘要覆盖一次——不是"检索不到",是内容真的没了。

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

摘要用规则拼装,**不调 LLM** —— 免费、零延迟;`messages` 数没变就跳过重写,每轮开销接近零。

检索是两段式:`search_past_conversations` 只扫索引,拿到 id 后用
`read_past_conversation(conv_id, query?)` 按需读原始 JSON 取细节。

---

## 前端 (WebView2 + chat.html)

**技术选型**:WebView2 (Chromium) 嵌入 WinForm,前端单文件 `chat.html` 包含所有 HTML/CSS/JS。

**核心 UI 特性**:

- **流式 markdown 渲染**:`renderMarkdownStreaming` 检测未闭合 ` ``` `,自动补尾,避免流式过程"代码块闪现"
- **合并气泡**:一整轮 AI 回复共用**一个"助手"标签**和大气泡容器,内部按时序交错文字段和工具卡片(`state.currentAssistantMsg` 生命周期到 `onBusy(false)` 才结束)
- **工具卡片折叠**:默认 `collapsed`,头部 `▸/▾` 箭头指示,状态徽章 `[已完成]/[失败]`
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

**对话持久化的四层保护**:

1. 每次 `_fullHistory.Add` (user/assistant/tool 三类) → `HistoryChanged` → `SaveCurrent` 立即写盘
2. 一轮 `SendAsync` finally 兜底 raise 一次
3. `FormClosing` 兜底
4. `AppDomain.CurrentDomain.UnhandledException` 兜底(.NET 侧未捕获异常)

PS 崩溃场景:

| 崩溃类型 | 覆盖情况 |
|---|---|
| .NET 侧未捕获异常 | ✅ AppDomain 兜底 |
| 用户 Alt+F4 / 正常退出 | ✅ FormClosing 兜底 |
| Task Manager 强杀 / native 崩溃 | ✅ 增量保存兜底(最多丢当前正在跑的**一个**工具) |
| 断电 | ✅ 增量保存兜底 |

---

## 部署 & 目录

**发布位置**:`{PS}\{version}\eMPower\CustomizedApps\TxTools\`

**运行时状态位置**:
- `{plugin}\{provider}.key` — DPAPI 加密 API Key
- `{plugin}\conversations\{id}.json` — 对话历史(协议原样存档)
- `{plugin}\conversations\index\{id}.md` — 对话摘要索引(检索用)
- `{plugin}\memory\snippets\*.md` / `gotchas\*.md` / `facts\*.md` / `api-notes\*.md` — 知识记忆
- `{plugin}\recipes.json` — 配方(结构化步骤,保留 JSON)
- `{plugin}\*.json.migrated` — 迁移到 MD 后保留的旧文件
- `{plugin}\prefs.json` — 用户偏好(provider/model/approvalMode)
- `{plugin}\audit.log` — 变更工具审计日志
- `%TEMP%\TxTools.Agent\uploads\{convId}\` — 上传文件(form 关闭时清理)
- `%USERPROFILE%\Desktop\TxTools_Exports\` — 导出的 docx/xlsx/pptx/png

**依赖**:
- Newtonsoft.Json 13.x —— **引用必须对齐 PS 自带的那一份**
  (`{PS}\eMPower\Newtonsoft.Json.dll`,`Copy Local = false`)。
  PS 进程已加载它,强名称绑定会顶掉插件引用的版本,签名对不上就 `MissingMethodException`。
- Microsoft.Web.WebView2
- DocumentFormat.OpenXml 2.20.0
- Microsoft.CSharp(主项目支持 dynamic,`run_csharp` 环境不含)

---

## 已知限制

- **`run_csharp` 无 `dynamic`** — 用反射代替(`CatiaBridge` 就是这个风格)
- **`GraphicViewer.GetImage` 是同步阻塞** — 抓大图(4K+)时 PS 视觉短暂无响应
- **对话历史无跨设备同步** — 只在本机 `{plugin}\conversations\` 下
- **未做回归测试** — prompt 或工具改动的验证只能"跑一遍看看"
- **harness 模式无 token 级流式的思考内容** — `reasoning_content` 只有 DeepSeek reasoner 系列返回,
  且需确认该模型支持 function calling
- **摘要索引是规则拼装** — 不调 LLM,语义概括能力有限;好处是免费且零延迟
- **上下文分项用量是估算值** — 按字符数折算,非 API 精确计数

---

## 扩展性设计的三个层次

**Layer 1 — 加工具**:写个 `ITxAgentTool` 实现,`AutoRegisterTools` 自动挑到,重启插件即可用。

**Layer 2 — 配方 (Recipe)**:稳定的多步工具编排,用户在 UI 或让 AI `save_recipe` 定义,`RecipeStore` 加载时注册成正式工具。AI 视角与内置工具无差。

**Layer 3 — 记忆学习**:AI 用 `run_csharp` 摸出新的 API 用法 → 系统自动 `SnippetStore.Upsert` 存下来 → 下轮遇到相似问题自动召回。踩过的坑 → `add_gotcha_correction` → 系统 prompt 末尾常驻。
