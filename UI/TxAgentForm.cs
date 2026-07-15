// TxTools.Agent / UI / TxAgentForm.cs  (v3 — 全 HTML UI)
//
// 从 1400 行的多控件自绘窗口简化为 ~500 行的"WebView2 壳":
//   • 窗口内只有一个填满的 WebView2,加载 Agent/UI/chat.html
//   • 顶栏、模型选择、按钮、输入区、附件卡片、状态栏、历史抽屉、API Key 输入
//     全部在 chat.html 内
//   • 审批弹窗(普通/代码)仍用原生 —— ApprovalRequest 委托签名是同步 bool,
//     改成异步会牵连 AgentLoop 深层重构;原生弹窗自带消息 pump,不阻塞。
//     ApiKeyDialog / ConversationListDialog 代码保留在项目里,但本 form 不再引用。
//
// WebView2 不可用时:弹 MessageBox 提示装 WebView2 Runtime,关闭窗口。
// 生产环境 Windows 10/11 系统级预装 Edge,基本不会走到这条路径。
//
// 通信协议 v2 见文件末尾 <PROTOCOL> 段的说明。

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tecnomatix.Engineering;
using Tecnomatix.Engineering.Ui;
using TxTools.Agent.Core;
using TxTools.Common;   // FormUiKit

namespace TxTools.Agent.UI
{
    public sealed class TxAgentForm : TxForm
    {
        private static readonly System.Drawing.Size DesignSize = new System.Drawing.Size(560, 720);
        private static readonly string[] AvailableModels = { "deepseek-v4-pro", "deepseek-v4-flash", "deepseek-chat" };
        private const string DefaultModel = "deepseek-v4-pro";

        // ── 依赖 ──
        private readonly ToolRegistry _tools;
        private DeepSeekClient _client;
        private AgentLoop _loop;
        private CancellationTokenSource _cts;
        private Conversation _current;
        private string _currentModel = DefaultModel;

        // ── WebView2 ──
        private WebView2 _webView;
        private bool _webViewReady;
        private bool _dpiApplied;

        public TxAgentForm(SynchronizationContext psCtx, ToolRegistry tools)
        {
            _tools = tools;

            FormUiKit.InitStandardForm(this,
                "TxTools.Agent \u2014 PDPS AI \u52a9\u624b (DeepSeek)",
                DesignSize, new System.Drawing.Size(420, 480), sizable: true);

            // TxForm 默认半模态,会挡住其它窗口;关掉才是真正的非模态
            try { SemiModal = false; } catch { }
            try
            {
                var flatStyleProp = this.GetType().GetProperty("FlatStyleEnabled");
                if (flatStyleProp != null && flatStyleProp.CanWrite)
                    flatStyleProp.SetValue(this, false, null);
            }
            catch { }

            _webView = new WebView2 { Dock = DockStyle.Fill };
            Controls.Add(_webView);
        }

        public override void OnInitTxForm()
        {
            base.OnInitTxForm();
            try { SemiModal = false; } catch { }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FormUiKit.ApplyDpiScaling(this, ref _dpiApplied, DesignSize);
            InitWebViewAsync();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            FireExtractLessons();                // 关窗前对当前对话跑一次经验萃取
            try { UploadStore.ClearAll(); } catch { }
            AgentLoop.Current = null;
            base.OnFormClosed(e);
        }

        // ─────────────────────────────────────────────────────
        //  WebView2 初始化 + HTML 加载
        // ─────────────────────────────────────────────────────

        private async void InitWebViewAsync()
        {
            try
            {
                await _webView.EnsureCoreWebView2Async(null);
                try { _webView.CoreWebView2.Settings.AreDevToolsEnabled = true; } catch { }
                try { _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true; } catch { }
                try { _webView.CoreWebView2.Settings.IsWebMessageEnabled = true; } catch { }

                _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

                // chat.html 作为嵌入资源打包进 dll(Build Action = EmbeddedResource),
                // 不再依赖 bin\Agent\UI\chat.html 的物理复制。
                string html = ReadEmbeddedChatHtml();
                if (string.IsNullOrEmpty(html))
                {
                    ShowFatal(
                        "找不到嵌入资源 chat.html。请检查:" + Environment.NewLine +
                        "  1) 项目内 Agent\\UI\\chat.html 的\"生成操作(Build Action)\"= \"嵌入的资源(EmbeddedResource)\"" + Environment.NewLine +
                        "  2) 保存后已 Rebuild。" + Environment.NewLine +
                        "  3) 排查:所有嵌入资源名清单会打印到调试输出窗口。");
                    DumpResourceNames();
                    return;
                }

                // HTML 完整性自检 + Debug 输出(用户 F12 前先能在 VS 输出窗口看到基本诊断)
                var trimmed = html.TrimStart();
                System.Diagnostics.Debug.WriteLine("[TxAgent] chat.html loaded, length=" + html.Length
                    + ", startsWith=" + (trimmed.Length >= 15 ? trimmed.Substring(0, 15) : trimmed)
                    + ", endsWith=" + (html.Length >= 15 ? html.Substring(html.Length - 15) : html));
                if (!trimmed.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase)
                    && !trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("[TxAgent] WARN: chat.html 开头异常,可能被截断或编码错误");
                }

                // 用 WebResourceRequested 拦截 https://chathtml/* 请求。
                // 好处: 页面 URL 保持真实文件名(而非 about:blank),
                //       F12→源代码 能定位到 chat.html 真实行号,便于排错。
                var htmlBytes = System.Text.Encoding.UTF8.GetBytes(html);
                _webView.CoreWebView2.AddWebResourceRequestedFilter(
                    "https://chathtml/*", CoreWebView2WebResourceContext.All);
                _webView.CoreWebView2.WebResourceRequested += (s, ev) =>
                {
                    try
                    {
                        var uri = ev.Request.Uri ?? "";
                        if (uri.EndsWith("/chat.html", StringComparison.OrdinalIgnoreCase)
                            || uri.EndsWith("chathtml/", StringComparison.OrdinalIgnoreCase))
                        {
                            var stream = new MemoryStream(htmlBytes);
                            ev.Response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
                                stream, 200, "OK", "Content-Type: text/html; charset=utf-8");
                        }
                        else
                        {
                            // favicon.ico 等资源 —— 直接 204,消掉 Console 里的 404 噪音
                            ev.Response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(
                                null, 204, "No Content", "");
                        }
                    }
                    catch (Exception rex)
                    {
                        System.Diagnostics.Debug.WriteLine("[TxAgent] WebResourceRequested err: " + rex.Message);
                    }
                };
                _webView.CoreWebView2.Navigate("https://chathtml/chat.html");
            }
            catch (Exception ex)
            {
                ShowFatal(
                    "WebView2 初始化失败: " + ex.Message + Environment.NewLine + Environment.NewLine +
                    "本插件的 UI 需要 WebView2 Runtime。请到:" + Environment.NewLine +
                    "https://developer.microsoft.com/microsoft-edge/webview2/" + Environment.NewLine +
                    "下载 Evergreen Runtime 后重试。");
            }
        }

        /// <summary>
        /// 从当前程序集读取嵌入的 chat.html 文本(UTF-8,自动跳 BOM)。
        /// 用 EndsWith("chat.html") 模糊匹配资源名,避免根命名空间/子目录变化时改代码。
        /// </summary>
        private static string ReadEmbeddedChatHtml()
        {
            var asm = typeof(TxAgentForm).Assembly;
            string resName = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("chat.html", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(resName)) return null;

            using (var s = asm.GetManifestResourceStream(resName))
            {
                if (s == null) return null;
                // detectEncodingFromByteOrderMarks=true 会自动识别并跳过 UTF-8 BOM
                using (var r = new System.IO.StreamReader(s, System.Text.Encoding.UTF8, true))
                    return r.ReadToEnd();
            }
        }

        /// <summary>找不到资源时把全部资源名打到 VS 输出窗口,便于快速定位命名空间/大小写问题。</summary>
        private static void DumpResourceNames()
        {
            try
            {
                var names = typeof(TxAgentForm).Assembly.GetManifestResourceNames();
                System.Diagnostics.Debug.WriteLine("[TxAgent] 嵌入资源清单 (" + names.Length + " 条):");
                foreach (var n in names) System.Diagnostics.Debug.WriteLine("  - " + n);
            }
            catch { }
        }

        private void ShowFatal(string msg)
        {
            try { MessageBox.Show(this, msg, "TxTools.Agent", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            try { Close(); } catch { }
        }

        // ─────────────────────────────────────────────────────
        //  JS → C# 消息分派
        // ─────────────────────────────────────────────────────

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string raw = null;
            try { raw = e.TryGetWebMessageAsString(); }
            catch
            {
                // JS 端传的可能不是纯字符串,尝试从 JSON 读
                try { raw = e.WebMessageAsJson; } catch { }
            }
            if (string.IsNullOrEmpty(raw))
            {
                PostStatus("\u6536\u5230\u7a7a\u6d88\u606f\uff08\u53ef\u80fd\u662f WebView2 \u4f20\u9012\u5f02\u5e38\uff09");
                return;
            }

            JObject msg;
            try { msg = JObject.Parse(raw); }
            catch (Exception ex)
            {
                PostStatus("\u6d88\u606f\u89e3\u6790\u5931\u8d25: " + ex.Message);
                return;
            }

            var type = (string)msg["type"];
            try
            {
                switch (type)
                {
                    case "jsReady":
                        _webViewReady = true;
                        OnJsReady();
                        break;

                    case "setApiKey":
                        ApplyKey((string)msg["key"], persist: true);
                        break;

                    case "switchModel":
                        SwitchModel((string)msg["model"]);
                        break;

                    case "userSend":
                        _ = HandleUserSendAsync((string)msg["text"], msg["attachments"] as JArray);
                        break;

                    case "userStop":
                        try { if (_cts != null) _cts.Cancel(); } catch { }
                        break;

                    case "newConv":
                        NewConversation();
                        break;

                    case "listConvs":
                        PostConvList();
                        break;

                    case "listTools":
                        PostToolList();
                        break;

                    case "openConv":
                        OpenConversation((string)msg["id"]);
                        break;

                    case "deleteConv":
                        {
                            var delId = (string)msg["id"];
                            try { ConversationStore.Delete(delId); } catch { }
                            // 如果删的是当前对话,清空并切到新对话 —— 否则下次切换时 SaveCurrent
                            // 会把这条被删的对话原地写回,导致"删了又出现"的假象。
                            if (_current != null && string.Equals(_current.Id, delId, StringComparison.Ordinal))
                            {
                                _current = null;
                                StartFreshConversation();
                            }
                            PostConvList();
                            break;
                        }

                    case "uploadFile":
                        HandleUploadFile((string)msg["filename"], (string)msg["contentBase64"]);
                        break;

                    case "removeAttachment":
                        try { UploadStore.Remove((string)msg["id"]); } catch { }
                        break;

                    case "extractLessons":
                        FireExtractLessons();
                        PostStatus("\u5df2\u5c1d\u8bd5\u7ecf\u9a8c\u840c\u53d6\uff08\u540e\u53f0\uff09");
                        break;
                }
            }
            catch (Exception ex)
            {
                PostStatus("\u5904\u7406\u6d88\u606f\u5f02\u5e38: " + ex.Message);
            }
        }

        /// <summary>JS 侧完成初始化后调用。发初始化数据、恢复上次对话或触发 API Key 输入。</summary>
        private void OnJsReady()
        {
            // 1) 可选模型列表
            PostJs(new { type = "modelList", items = AvailableModels, current = _currentModel });

            // 2) 加载 API Key
            var saved = KeyStore.Load();
            if (!string.IsNullOrWhiteSpace(saved))
            {
                ApplyKey(saved, persist: false);
                LoadMostRecentOrNew();
                PostJs(new { type = "keyReady" });
                PostStatus("\u5df2\u52a0\u8f7d API Key\u3002\u5de5\u5177 " + _tools.Count + " \u4e2a\u3002");
            }
            else
            {
                PostStatus("\u5c1a\u672a\u8bbe\u7f6e API Key\u3002");
                PostJs(new { type = "askApiKey", reason = "\u9996\u6b21\u4f7f\u7528\u9700\u8981\u8bbe\u7f6e DeepSeek API Key\u3002" });
            }
        }

        // ─────────────────────────────────────────────────────
        //  C# → JS: 统一入口 PostJs + 各种便捷发送
        // ─────────────────────────────────────────────────────

        private void PostJs(object payload)
        {
            // 注意: 不能在这里判断 _webView.CoreWebView2 == null —— 那本身就是"访问 CoreWebView2",
            // 从后台线程(如 AgentLoop 事件回调 in Task.Run)进来会抛
            //   "CoreWebView2 can only be accessed from the UI thread"。
            // 所有 CoreWebView2 访问必须先跳到 UI 线程再做。
            if (!_webViewReady) return;

            string json;
            try
            {
                json = JsonConvert.SerializeObject(payload,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch { return; }

            var js = "try{dispatchMessage(" + JsonConvert.SerializeObject(json) + ");}catch(e){}";

            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(() => SafeExecuteScript(js))); }
                catch { /* form 已释放或 handle 未创建 */ }
            }
            else
            {
                SafeExecuteScript(js);
            }
        }

        /// <summary>在 UI 线程上安全执行 JS,访问 CoreWebView2 前统一 null 检查+异常兜底。</summary>
        private void SafeExecuteScript(string js)
        {
            try
            {
                if (_webView == null || _webView.CoreWebView2 == null) return;
                _webView.CoreWebView2.ExecuteScriptAsync(js);
            }
            catch { }
        }

        private void PostStatus(string text) { PostJs(new { type = "status", text }); }
        private void PostBusy(bool busy) { PostJs(new { type = "busy", value = busy }); }
        private void PostTokenUsage(int p, int c, int t) { PostJs(new { type = "tokenUsage", prompt = p, completion = c, total = t }); }

        private void PostConvList()
        {
            List<ConversationMeta> metas;
            try { metas = ConversationStore.List(); }
            catch { metas = new List<ConversationMeta>(); }

            var items = metas.Select(m => new
            {
                id = m.Id,
                title = string.IsNullOrEmpty(m.Title) ? "(\u65e0\u6807\u9898)" : m.Title,
                updated = m.UpdatedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
            }).ToList();
            PostJs(new { type = "convList", items });
        }

        /// <summary>把所有已注册工具的名字/描述/是否只读 发给 JS 端,供工具面板展示。</summary>
        private void PostToolList()
        {
            var items = new List<object>();
            try
            {
                foreach (var t in _tools.Tools)
                {
                    items.Add(new
                    {
                        name = t.Name,
                        description = t.Description ?? "",
                        isReadOnly = t.IsReadOnly
                    });
                }
            }
            catch { }
            // 按 只读优先 + 名称字母序 排,展示更清爽
            items.Sort((a, b) =>
            {
                dynamic da = a, db = b;
                int cmp = ((bool)db.isReadOnly).CompareTo((bool)da.isReadOnly);   // true(只读) 在前
                if (cmp != 0) return cmp;
                return string.Compare((string)da.name, (string)db.name, StringComparison.OrdinalIgnoreCase);
            });
            PostJs(new { type = "toolList", items });
        }

        // ─────────────────────────────────────────────────────
        //  上传文件
        // ─────────────────────────────────────────────────────

        private void HandleUploadFile(string filename, string contentBase64)
        {
            if (string.IsNullOrEmpty(contentBase64))
            {
                PostJs(new { type = "attachmentInfo", error = "\u4e0a\u4f20\u5185\u5bb9\u4e3a\u7a7a" });
                return;
            }
            byte[] bytes;
            try { bytes = Convert.FromBase64String(contentBase64); }
            catch (Exception ex)
            {
                PostJs(new { type = "attachmentInfo", error = "base64 \u89e3\u7801\u5931\u8d25: " + ex.Message });
                return;
            }

            var convId = _current != null ? _current.Id : "_default";
            UploadedFile uf;
            try { uf = UploadStore.Store(convId, filename, bytes); }
            catch (Exception ex)
            {
                PostJs(new { type = "attachmentInfo", error = "\u5b58\u50a8\u5931\u8d25: " + ex.Message });
                return;
            }

            try { FileParserService.Parse(uf); }
            catch (Exception ex) { uf.ParseError = ex.Message; }

            PostJs(new
            {
                type = "attachmentInfo",
                id = uf.Id,
                name = uf.OriginalName,
                extension = uf.Extension,
                size = uf.Size,
                sizeText = FileParserService.FormatBytes(uf.Size),
                rowCount = uf.RowCount,
                colCount = uf.ColCount,
                sheetCount = uf.SheetCount,
                summary = uf.ParsedSummary,
                error = uf.ParseError
            });
        }

        // ─────────────────────────────────────────────────────
        //  用户发送(含附件摘要注入)
        // ─────────────────────────────────────────────────────

        private async Task HandleUserSendAsync(string text, JArray attachments)
        {
            if (_loop == null)
            {
                PostStatus("\u8bf7\u5148\u8bbe\u7f6e API Key\u3002");
                PostJs(new { type = "askApiKey", reason = "\u5c1a\u672a\u8bbe\u7f6e API Key\u3002" });
                return;
            }

            var body = text ?? "";
            var prefix = BuildAttachmentPrefix(attachments);
            var finalText = string.IsNullOrEmpty(prefix) ? body : (prefix + "\n\n" + body);
            if (string.IsNullOrWhiteSpace(finalText)) return;

            PostBusy(true);
            _cts = new CancellationTokenSource();
            try
            {
                var token = _cts.Token;
                await Task.Run(() => _loop.SendAsync(finalText, token));
                PostStatus("\u5c31\u7eea\u3002");
            }
            catch (OperationCanceledException)
            {
                PostJs(new { type = "message", role = "\u7cfb\u7edf", text = "\u5df2\u53d6\u6d88\u672c\u6b21\u8bf7\u6c42\u3002" });
                PostStatus("\u5df2\u53d6\u6d88\u3002");
            }
            catch (LlmApiException apiEx)
            {
                PostJs(new { type = "message", role = "\u7cfb\u7edf", text = "API \u9519\u8bef: " + apiEx.Message });
                PostStatus("API \u9519\u8bef\u3002");
            }
            catch (Exception ex)
            {
                PostJs(new { type = "message", role = "\u7cfb\u7edf", text = "\u51fa\u9519: " + ex.Message });
                PostStatus("\u51fa\u9519\u3002");
            }
            finally
            {
                PostJs(new { type = "closeAssistant" });
                PostBusy(false);
                try { if (_cts != null) _cts.Dispose(); } catch { }
                _cts = null;
            }
        }

        /// <summary>把附件的摘要拼成一段前缀,注入到用户消息前面。让 AI 拿到即可判断能否直接答。</summary>
        private static string BuildAttachmentPrefix(JArray attachments)
        {
            if (attachments == null || attachments.Count == 0) return "";
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[\u5df2\u9644\u52a0\u6587\u4ef6]");
            int i = 0;
            foreach (var a in attachments)
            {
                i++;
                var id = (string)a["id"];
                if (string.IsNullOrEmpty(id)) continue;
                var uf = UploadStore.Get(id);
                if (uf == null) continue;

                sb.AppendLine();
                sb.AppendLine(i + ") " + uf.OriginalName + "  (id=" + uf.Id
                    + ", " + FileParserService.FormatBytes(uf.Size)
                    + (uf.SheetCount > 0 ? ", " + uf.SheetCount + " sheet" : "")
                    + (uf.RowCount > 0 ? ", " + uf.RowCount + "\u884c" + (uf.ColCount > 0 ? "\u00d7" + uf.ColCount + "\u5217" : "") : "")
                    + ")");
                if (!string.IsNullOrEmpty(uf.ParseError))
                    sb.AppendLine("   \u26a0 \u89e3\u6790\u8b66\u544a: " + uf.ParseError);
                if (!string.IsNullOrEmpty(uf.ParsedSummary))
                {
                    sb.AppendLine("   \u6458\u8981:");
                    foreach (var line in uf.ParsedSummary.Split(new[] { '\n' }, StringSplitOptions.None))
                        sb.AppendLine("     " + line.TrimEnd('\r'));
                }
            }
            sb.AppendLine();
            sb.AppendLine("[\u5982\u9700\u7cbe\u8bfb\u5b8c\u6574\u5185\u5bb9,\u8c03\u7528 read_uploaded_file(file_id=...)]");
            return sb.ToString();
        }

        // ─────────────────────────────────────────────────────
        //  API Key / 模型切换
        // ─────────────────────────────────────────────────────

        private void ApplyKey(string key, bool persist)
        {
            if (string.IsNullOrWhiteSpace(key)) { PostStatus("API Key \u4e3a\u7a7a\u3002"); return; }
            try
            {
                _client = new DeepSeekClient(key);
                _loop = BuildLoop(_client);
                if (_current != null) _loop.LoadHistory(_current.Messages);

                if (persist)
                {
                    try { KeyStore.Save(key); } catch { }
                    PostStatus("\u5df2\u4fdd\u5b58 API Key\u3002");
                }
                PostJs(new { type = "keyReady" });

                if (_current == null) LoadMostRecentOrNew();
            }
            catch (Exception ex)
            {
                PostStatus("\u8bbe\u7f6e Key \u5931\u8d25: " + ex.Message);
            }
        }

        private void SwitchModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model)) return;
            _currentModel = model;
            if (_client == null) { PostStatus("\u5df2\u9884\u9009\u6a21\u578b: " + model); return; }
            _loop = BuildLoop(_client);
            _loop.LoadHistory(_current != null ? _current.Messages : new List<ChatMessage>());
            if (_current != null) RestoreTranscriptToJs(_current.Messages);
            PostStatus("\u5df2\u5207\u6362\u6a21\u578b\u4e3a " + model + "\uff0c\u8bb0\u5fc6\u4fdd\u7559\u3002");
        }

        // ─────────────────────────────────────────────────────
        //  对话生命周期
        // ─────────────────────────────────────────────────────

        private void NewConversation()
        {
            SaveCurrent();
            FireExtractLessons();
            StartFreshConversation();
            PostStatus("\u5df2\u5f00\u59cb\u65b0\u5bf9\u8bdd\u3002\u65e7\u5bf9\u8bdd\u5df2\u4fdd\u7559\u3002");
        }

        private void OpenConversation(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            SaveCurrent();
            FireExtractLessons();
            LoadConversation(id);
            PostStatus("\u5df2\u6253\u5f00\u5386\u53f2\u5bf9\u8bdd\u3002");
        }

        private void LoadMostRecentOrNew()
        {
            var metas = ConversationStore.List();
            if (metas.Count > 0) LoadConversation(metas[0].Id);
            else StartFreshConversation();
        }

        private void StartFreshConversation()
        {
            _current = new Conversation { Id = ConversationStore.NewId(), CreatedUtc = DateTime.UtcNow };
            if (_loop != null)
            {
                _loop.SetConvId(_current.Id);
                _loop.Reset();
            }
            PostJs(new { type = "clear" });
            PostJs(new { type = "message", role = "\u7cfb\u7edf", text = "\u5df2\u5c31\u7eea\uff0c\u53ef\u4ee5\u5f00\u59cb\u5bf9\u8bdd\u3002" });
        }

        private void LoadConversation(string id)
        {
            var conv = ConversationStore.Load(id);
            if (conv == null) { StartFreshConversation(); return; }
            _current = conv;
            if (_loop != null)
            {
                _loop.SetConvId(id);
                _loop.LoadHistory(conv.Messages);
            }
            RestoreTranscriptToJs(conv.Messages);
        }

        private void SaveCurrent()
        {
            if (_loop == null || _current == null) return;
            if (!ConversationStore.HasUserContent(_loop.FullHistory)) return;
            _current.Messages = new List<ChatMessage>(_loop.FullHistory);
            try { ConversationStore.Save(_current); } catch { }
        }

        /// <summary>把消息数组序列化成 restore payload,交给 chat.html 一次性渲染。</summary>
        private void RestoreTranscriptToJs(IEnumerable<ChatMessage> messages)
        {
            if (messages == null) return;
            var msgList = new List<object>();
            var idToName = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var m in messages)
            {
                if (m == null || m.Role == "system") continue;
                if (m.Role == "user")
                {
                    msgList.Add(new { role = "user", text = m.Content ?? "" });
                }
                else if (m.Role == "assistant")
                {
                    var tcList = new List<object>();
                    if (m.ToolCalls != null)
                        foreach (var tc in m.ToolCalls)
                        {
                            var nm = tc.Function != null ? tc.Function.Name : "?";
                            if (tc.Id != null) idToName[tc.Id] = nm;
                            tcList.Add(new
                            {
                                id = tc.Id ?? "",
                                name = nm,
                                input = tc.Function != null ? Truncate(tc.Function.Arguments ?? "", 200) : ""
                            });
                        }
                    msgList.Add(new
                    {
                        role = "assistant",
                        text = m.Content ?? "",
                        toolCalls = tcList.Count > 0 ? tcList : null
                    });
                }
                else if (m.Role == "tool")
                {
                    string nm;
                    idToName.TryGetValue(m.ToolCallId ?? "", out nm);
                    bool isErr = m.Content != null && m.Content.StartsWith("Error", StringComparison.OrdinalIgnoreCase);
                    msgList.Add(new
                    {
                        role = "tool",
                        toolCallId = m.ToolCallId ?? "",
                        name = nm ?? "",
                        result = m.Content ?? "",
                        isErr = isErr
                    });
                }
            }
            PostJs(new { type = "restore", messages = msgList });
        }

        // ─────────────────────────────────────────────────────
        //  AgentLoop 构造 + 事件转发
        // ─────────────────────────────────────────────────────

        private AgentLoop BuildLoop(DeepSeekClient client)
        {
            var options = new AgentOptions { Model = _currentModel };

            // 记忆系统低风险写工具免弹窗
            options.AutoApproveTools.Add("add_fact");
            options.AutoApproveTools.Add("add_gotcha_correction");

            var loop = new AgentLoop(client, _tools, options);
            if (_current != null && !string.IsNullOrEmpty(_current.Id))
                loop.SetConvId(_current.Id);

            loop.AssistantDelta += frag => PostJs(new { type = "delta", text = frag });
            loop.Info += t =>
            {
                PostJs(new { type = "closeAssistant" });
                PostJs(new { type = "message", role = "\u7cfb\u7edf", text = t });
            };
            loop.ToolCalled += (name, input) =>
            {
                PostJs(new { type = "closeAssistant" });
                PostStatus("\u2699 " + name + "\u2026");
                PostJs(new { type = "toolCall", name = name, input = Compact(input) });
            };
            loop.ToolCompleted += (name, result, isErr) =>
            {
                PostJs(new { type = "toolResult", name = name, result = result, isErr = isErr });
                PostStatus("\u5c31\u7eea\u3002");
            };
            loop.ApprovalRequest = AskApprovalNative;
            loop.HistoryChanged += SaveCurrent;
            loop.TokenUsed += (p, c, t) => PostTokenUsage(loop.TotalPromptTokens, loop.TotalCompletionTokens, loop.TotalTokens);

            AgentLoop.Current = loop;
            return loop;
        }

        private void FireExtractLessons()
        {
            var loop = _loop;
            if (loop == null) return;
            if (loop.FullHistory == null || loop.FullHistory.Count < 4) return;
            Task.Run(async () =>
            {
                try { await loop.ExtractLessonsAsync(CancellationToken.None); }
                catch { }
            });
        }

        // ─────────────────────────────────────────────────────
        //  审批 —— 保留原生弹窗(ApprovalRequest 委托必须同步返回)
        // ─────────────────────────────────────────────────────

        private bool AskApprovalNative(ITxAgentTool tool, JObject input)
        {
            if (InvokeRequired)
                return (bool)Invoke(new Func<bool>(() => AskApprovalNative(tool, input)));

            // run_csharp:代码审阅框
            var codeTok = input != null ? input["code"] : null;
            if (codeTok != null && codeTok.Type == JTokenType.String)
                return CodeApprovalDialog.Show(this, tool.Name, (string)codeTok) == DialogResult.Yes;

            // 其它变更工具:简单确认框
            var msg = "\u52a9\u624b\u8bf7\u6c42\u6267\u884c\u4e00\u4e2a\u4f1a\u6539\u52a8\u573a\u666f\u7684\u64cd\u4f5c\uff1a\n\n" +
                      "\u5de5\u5177: " + tool.Name + "\n\u53c2\u6570: " + Compact(input) + "\n\n\u662f\u5426\u5141\u8bb8\uff1f";
            return MessageBox.Show(this, msg, "\u64cd\u4f5c\u786e\u8ba4", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                   == DialogResult.Yes;
        }

        // ─────────────────────────────────────────────────────
        //  小工具
        // ─────────────────────────────────────────────────────

        private static string Compact(JObject input)
        {
            if (input == null) return "{}";
            var s = JsonConvert.SerializeObject(input);
            return s.Length <= 200 ? s : s.Substring(0, 200) + "\u2026";
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max) + "\u2026";
        }

        // ─────────────────────────────────────────────────────
        //  <PROTOCOL> 通信协议 v2 参考
        //
        //  JS → C# (postMessage JSON string):
        //    { type:"jsReady" }
        //    { type:"setApiKey", key }
        //    { type:"switchModel", model }
        //    { type:"userSend", text, attachments:[{id,name}] }
        //    { type:"userStop" }
        //    { type:"newConv" }
        //    { type:"listConvs" }
        //    { type:"openConv", id }
        //    { type:"deleteConv", id }
        //    { type:"uploadFile", filename, contentBase64 }
        //    { type:"removeAttachment", id }
        //    { type:"extractLessons" }
        //
        //  C# → JS (dispatchMessage(...)):
        //    { type:"modelList", items, current }
        //    { type:"keyReady" }
        //    { type:"askApiKey", reason }
        //    { type:"convList", items:[{id,title,updated}] }
        //    { type:"clear" }
        //    { type:"restore", messages:[...] }
        //    { type:"message", role, text }
        //    { type:"delta", text }
        //    { type:"closeAssistant" }
        //    { type:"toolCall", name, input }
        //    { type:"toolResult", name, result, isErr }
        //    { type:"status", text }
        //    { type:"busy", value }
        //    { type:"tokenUsage", prompt, completion, total }
        //    { type:"attachmentInfo", id?, name?, extension?, size?, sizeText?, rowCount?, colCount?, sheetCount?, summary?, error? }
        // </PROTOCOL>
    }
}
