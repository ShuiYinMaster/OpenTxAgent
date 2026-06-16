// TxAgent / UI / TxAgentForm.cs
// PS 内嵌聊天窗口，驱动 AgentLoop (DeepSeek)。
//
// UI 框架与导插枪保持一致：
//   - 继承 TxForm，由 PS 框架托管窗口层级
//   - FormUiKit.InitStandardForm：类型全名持久化键 + AutoScaleMode.None + 关闭 flat 皮肤
//   - OnLoad 里 FormUiKit.ApplyDpiScaling：先重置设计尺寸再 Scale，防 TxForm 持久化尺寸叠加放大
//   - 控件用 FormUiKit.MkButton/MkLabel 自绘，绕过 PS flat 皮肤吃配色
//   - 构造签名 (SynchronizationContext psCtx, ...) 同 ExportGunForm
//
// 记忆：对话历史经 ConversationStore 落盘，开窗时恢复；"新对话"清空。

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using Tecnomatix.Engineering;
using Tecnomatix.Engineering.Ui;
using TxAgent.Core;
using TxTools.Common;          // FormUiKit
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using Label = System.Windows.Forms.Label;
using RichTextBox = System.Windows.Forms.RichTextBox;
// 工程同时引用了 WPF(部分插件用)，下列控件名在 WinForms/WPF 间会歧义(CS0104)。
// 本窗体是 WinForms，显式别名强制指向 System.Windows.Forms。
using TextBox = System.Windows.Forms.TextBox;

namespace TxAgent.UI
{
    public sealed class TxAgentForm : TxForm
    {
        private static readonly Size DesignSize = new Size(560, 640);

        // 配色（柔和中性 + 蓝色强调，卡片式留白）
        private static readonly Color UiBg = Color.FromArgb(245, 246, 248);
        private static readonly Color UiCard = Color.White;
        private static readonly Color UiBorder = Color.FromArgb(214, 218, 224);
        private static readonly Color UiAccent = Color.FromArgb(20, 110, 190);
        private static readonly Color UiMuted = Color.FromArgb(120, 128, 138);
        private static readonly Color UiUser = Color.FromArgb(36, 41, 47);
        private static readonly Color UiAsst = Color.FromArgb(20, 90, 160);

        private readonly ToolRegistry _tools;
        private DeepSeekClient _client;
        private AgentLoop _loop;
        private CancellationTokenSource _cts;
        private bool _dpiApplied;

        private RichTextBox _transcript;
        private TextBox _input;
        private Button _sendBtn;
        private Button _stopBtn;
        private Button _keyBtn;
        private Button _newBtn;
        private Button _historyBtn;
        private ComboBox _modelBox;
        private Label _status;

        private Conversation _current;   // 当前对话(多对话库中的一条)

        public TxAgentForm(SynchronizationContext psCtx, ToolRegistry tools)
        {
            _tools = tools;
            // psCtx 已在 TxAgentCommand 里设进 PsContext.Current；这里保留签名以与 ExportGunForm 一致。

            FormUiKit.InitStandardForm(this, "TxAgent — PDPS AI 助手 (DeepSeek)",
                                       DesignSize, new Size(420, 480), sizable: true);

            // TxForm 默认半模态，会挡住其它窗口的打开；关掉它才是真正的非模态。
            try { SemiModal = false; } catch { }
            try
            {
                var flatStyleProp = this.GetType().GetProperty("FlatStyleEnabled");
                if (flatStyleProp != null && flatStyleProp.CanWrite)
                {
                    flatStyleProp.SetValue(this, false, null);
                }
            }
            catch
            {
                // 反射失败时静默忽略，确保插件继续运行
            }
            BuildUi();

            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime) return;
        }

        public override void OnInitTxForm()
        {
            base.OnInitTxForm();
            try { SemiModal = false; } catch { }   // 双保险：防 TxForm 初始化阶段重置
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FormUiKit.ApplyDpiScaling(this, ref _dpiApplied, DesignSize);

            // 复用已保存的 key；没有则弹窗输入。
            var saved = KeyStore.Load();
            if (!string.IsNullOrWhiteSpace(saved))
            {
                ApplyKey(saved, persist: false);
                LoadMostRecentOrNew();   // 恢复最近一条对话(没有则开新的)
                SetStatus("已加载 API Key。工具 " + _tools.Count + " 个。");
            }
            else
            {
                SetStatus("尚未设置 API Key。");
                PromptForKey();
            }
        }

        // ---- UI 构建 (FormUiKit 同款配色/自绘控件) ----

        private void BuildUi()
        {
            BackColor = UiBg;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = UiBg,
                Padding = new Padding(8, 6, 8, 6)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // ── 顶部：左=模型选择，右=操作按钮 ──
            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                BackColor = UiBg,
                Margin = new Padding(2, 0, 2, 6)
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var modelGroup = new FlowLayoutPanel { AutoSize = true, BackColor = UiBg, Margin = new Padding(0) };
            var modelLbl = FormUiKit.MkLabel("模型", false);
            modelLbl.ForeColor = UiMuted;
            modelLbl.Margin = new Padding(0, 6, 6, 0);
            modelGroup.Controls.Add(modelLbl);
            _modelBox = new ComboBox { Width = 190, DropDownStyle = ComboBoxStyle.DropDownList, Font = FormUiKit.BaseFont };
            _modelBox.Items.AddRange(new object[] { "deepseek-v4-pro", "deepseek-v4-flash", "deepseek-chat" });
            _modelBox.SelectedIndex = 0;
            _modelBox.SelectedIndexChanged += ModelChanged;
            modelGroup.Controls.Add(_modelBox);
            header.Controls.Add(modelGroup, 0, 0);

            // 右对齐：RightToLeft 流，先加的在最右
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true, BackColor = UiBg };
            _keyBtn = FormUiKit.MkButton("设置 API Key…", false);
            _keyBtn.Click += (s, e) => PromptForKey();
            _historyBtn = FormUiKit.MkButton("历史对话", false);
            _historyBtn.Click += (s, e) => OpenHistory();
            _newBtn = FormUiKit.MkButton("新对话", false);
            _newBtn.Click += (s, e) => NewConversation();
            actions.Controls.Add(_keyBtn);
            actions.Controls.Add(_historyBtn);
            actions.Controls.Add(_newBtn);
            header.Controls.Add(actions, 1, 0);
            root.Controls.Add(header, 0, 0);

            // ── 对话区（白底卡片 + 细边 + 留白）──
            var tBorder = new System.Windows.Forms.Panel { Dock = DockStyle.Fill, BackColor = UiBorder, Padding = new Padding(1), Margin = new Padding(2, 0, 2, 6) };
            var tCard = new System.Windows.Forms.Panel { Dock = DockStyle.Fill, BackColor = UiCard, Padding = new Padding(8, 6, 6, 6) };
            _transcript = new System.Windows.Forms.RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = FormUiKit.BaseFont,
                BackColor = UiCard,
                BorderStyle = BorderStyle.None
            };
            tCard.Controls.Add(_transcript);
            tBorder.Controls.Add(tCard);
            root.Controls.Add(tBorder, 0, 1);

            // ── 输入区（卡片：左输入框 + 右发送/停止）──
            var iBorder = new System.Windows.Forms.Panel { Dock = DockStyle.Fill, BackColor = UiBorder, Padding = new Padding(1), Margin = new Padding(2, 0, 2, 4) };
            var iCard = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = UiCard, ColumnCount = 2, RowCount = 1, Padding = new Padding(8, 6, 6, 6) };
            iCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            iCard.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _input = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsReturn = false,
                Font = FormUiKit.BaseFont,
                BorderStyle = BorderStyle.None,
                BackColor = UiCard
            };
            _input.KeyDown += Input_KeyDown;
            iCard.Controls.Add(_input, 0, 0);

            var btnCol = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, BackColor = UiCard, Margin = new Padding(8, 0, 0, 0) };
            _sendBtn = FormUiKit.MkButton("发送", true, 84);
            _sendBtn.Click += async (s, e) => await SendCurrentInput();
            _stopBtn = FormUiKit.MkButton("停止", false, 84);
            _stopBtn.Enabled = false;
            _stopBtn.Margin = new Padding(_stopBtn.Margin.Left, 4, _stopBtn.Margin.Right, _stopBtn.Margin.Bottom);
            _stopBtn.Click += (s, e) => { if (_cts != null) _cts.Cancel(); };
            btnCol.Controls.Add(_sendBtn);
            btnCol.Controls.Add(_stopBtn);
            iCard.Controls.Add(btnCol, 1, 0);
            root.Controls.Add(iBorder, 0, 2);
            iBorder.Controls.Add(iCard);

            // ── 状态行 ──
            _status = FormUiKit.MkLabel("", false);
            _status.Dock = DockStyle.Fill;
            _status.ForeColor = UiMuted;
            _status.Margin = new Padding(4, 2, 4, 0);
            root.Controls.Add(_status, 0, 3);

            Controls.Add(root);
        }

        // ---- API Key / Loop / 记忆 ----

        private void PromptForKey()
        {
            using (var dlg = new ApiKeyDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                ApplyKey(dlg.ApiKey, persist: true);
                if (_current == null) LoadMostRecentOrNew();
            }
        }

        private void ApplyKey(string key, bool persist)
        {
            if (string.IsNullOrWhiteSpace(key)) { SetStatus("API Key 为空。"); return; }
            try
            {
                _client = new DeepSeekClient(key);
                _loop = BuildLoop(_client);
                if (_current != null) _loop.LoadHistory(_current.Messages); // 重设 key 时保留当前对话
                if (persist)
                {
                    var path = KeyStore.Save(key);
                    SetStatus("已保存并加密 API Key 到: " + path);
                }
            }
            catch (Exception ex) { SetStatus("设置失败: " + ex.Message); }
        }

        private AgentLoop BuildLoop(DeepSeekClient client)
        {
            var options = new AgentOptions { Model = (string)_modelBox.SelectedItem };
            var loop = new AgentLoop(client, _tools, options);
            loop.AssistantDelta += OnAssistantDelta;
            loop.Info += t => { CloseAssistant(); AppendLine("系统", t, SystemColors.GrayText); };
            loop.ToolCalled += (name, input) =>
            { CloseAssistant(); AppendLine("工具", "调用 " + name + " " + Compact(input), Color.FromArgb(120, 90, 0)); };
            loop.ToolCompleted += (name, result, isErr) =>
                AppendLine("工具", (isErr ? "✗ " : "✓ ") + name + " -> " + Truncate(result, 400),
                           isErr ? Color.Firebrick : Color.FromArgb(0, 120, 80));
            loop.ApprovalRequest = AskApproval;
            loop.HistoryChanged += SaveCurrent;   // 每轮结束持久化到当前对话
            return loop;
        }

        private void ModelChanged(object sender, EventArgs e)
        {
            if (_client == null) return;
            _loop = BuildLoop(_client);
            _loop.LoadHistory(_current != null ? _current.Messages : new List<ChatMessage>()); // 保留当前对话
            SetStatus("已切换模型为 " + _modelBox.SelectedItem + "，记忆保留。");
        }

        private void NewConversation()
        {
            SaveCurrent();                 // 先存好当前对话(旧对话保留)
            StartFreshConversation();
            SetStatus("已开始新对话。旧对话已保留，可在「历史对话」里回看。");
        }

        private void OpenHistory()
        {
            var id = ConversationListDialog.Pick(this);
            if (string.IsNullOrEmpty(id)) return;
            SaveCurrent();                 // 切换前存好当前
            LoadConversation(id);
            SetStatus("已打开历史对话。");
        }

        /// <summary>持久化当前对话(空对话不落盘，避免产生大量无内容文件)。</summary>
        private void SaveCurrent()
        {
            if (_loop == null || _current == null) return;
            if (!ConversationStore.HasUserContent(_loop.History)) return;
            _current.Messages = new List<ChatMessage>(_loop.History);
            ConversationStore.Save(_current);
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
            if (_loop != null) _loop.Reset();
            _transcript.Clear();
        }

        private void LoadConversation(string id)
        {
            var conv = ConversationStore.Load(id);
            if (conv == null) { StartFreshConversation(); return; }
            _current = conv;
            if (_loop != null) _loop.LoadHistory(conv.Messages);
            RestoreTranscript(conv.Messages);
        }

        private void RestoreTranscript(IEnumerable<ChatMessage> messages)
        {
            _transcript.Clear();
            if (messages == null) return;
            var idToName = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var m in messages)
            {
                if (m == null || m.Role == "system") continue;
                if (m.Role == "user")
                {
                    if (!string.IsNullOrEmpty(m.Content))
                        AppendLine("你", m.Content, UiUser);
                }
                else if (m.Role == "assistant")
                {
                    if (!string.IsNullOrEmpty(m.Content))
                        AppendLine("助手", m.Content, UiAsst);
                    if (m.ToolCalls != null)
                        foreach (var tc in m.ToolCalls)
                        {
                            var nm = tc.Function != null ? tc.Function.Name : "?";
                            if (tc.Id != null) idToName[tc.Id] = nm;
                            AppendLine("工具", "调用 " + nm, Color.FromArgb(120, 90, 0));
                        }
                }
                else if (m.Role == "tool")
                {
                    string nm;
                    idToName.TryGetValue(m.ToolCallId ?? "", out nm);
                    AppendLine("工具", (nm ?? "") + " -> " + Truncate(m.Content, 400),
                               Color.FromArgb(0, 120, 80));
                }
            }
        }

        // ---- 交互 ----

        private async void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                e.SuppressKeyPress = true;
                await SendCurrentInput();
            }
            else if (e.KeyCode == Keys.Enter && e.Control)
            {
                int pos = _input.SelectionStart;
                _input.Text = _input.Text.Insert(pos, Environment.NewLine);
                _input.SelectionStart = pos + Environment.NewLine.Length;
            }
        }

        private async System.Threading.Tasks.Task SendCurrentInput()
        {
            if (_loop == null) { SetStatus("请先设置 API Key。"); PromptForKey(); return; }

            var text = _input.Text.Trim();
            if (text.Length == 0) return;

            AppendLine("你", text, UiUser);
            _input.Clear();
            SetBusy(true);
            _cts = new CancellationTokenSource();

            try
            {
                // 关键：循环放到后台线程跑。PS SDK 调用由 PsContext 统一回主线程；
                // 这样网络/编译/思考期间 UI 与 PS 保持响应，“停止”也能在步骤之间生效。
                var token = _cts.Token;
                await System.Threading.Tasks.Task.Run(() => _loop.SendAsync(text, token));
                SetStatus("就绪。");
            }
            catch (OperationCanceledException)
            {
                AppendLine("系统", "已取消本次请求。", SystemColors.GrayText);
            }
            catch (LlmApiException apiEx)
            {
                AppendLine("系统", "API 错误: " + apiEx.Message, Color.Firebrick);
            }
            catch (Exception ex)
            {
                AppendLine("系统", "出错: " + ex.Message, Color.Firebrick);
            }
            finally
            {
                CloseAssistant();
                SetBusy(false);
                _cts.Dispose();
                _cts = null;
            }
        }

        private bool AskApproval(ITxAgentTool tool, JObject input)
        {
            // 循环在后台线程，弹窗须回 UI 线程；Invoke 同步返回用户选择。
            if (InvokeRequired)
                return (bool)Invoke(new Func<bool>(() => AskApproval(tool, input)));

            var codeTok = input != null ? input["code"] : null;
            if (codeTok != null && codeTok.Type == JTokenType.String)
            {
                // 代码类操作：用可滚动、尺寸受限的专用对话框完整展示，避免顶穿屏幕。
                return CodeApprovalDialog.Show(this, tool.Name, (string)codeTok) == DialogResult.Yes;
            }

            var msg = "助手请求执行一个会改动场景的操作：\n\n" +
                      "工具: " + tool.Name + "\n参数: " + Compact(input) + "\n\n是否允许？";
            return MessageBox.Show(this, msg, "操作确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                   == DialogResult.Yes;
        }

        // ---- 辅助 ----

        private void SetBusy(bool busy)
        {
            _sendBtn.Enabled = !busy;
            _input.Enabled = !busy;
            _stopBtn.Enabled = busy;
            if (busy) SetStatus("思考中…");
        }

        private void SetStatus(string text)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => _status.Text = text)); return; }
            _status.Text = text;
        }

        // 流式渲染：开行(打一次【助手】前缀) -> 追加分片 -> 闭行(换行)
        private bool _assistantOpen;

        private void OnAssistantDelta(string frag)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => OnAssistantDelta(frag))); return; }
            if (!_assistantOpen)
            {
                _transcript.SelectionStart = _transcript.TextLength;
                _transcript.SelectionColor = UiAsst;
                _transcript.SelectionFont = new Font(_transcript.Font, FontStyle.Bold);
                _transcript.AppendText("【助手】 ");
                _assistantOpen = true;
            }
            _transcript.SelectionStart = _transcript.TextLength;
            _transcript.SelectionColor = UiAsst;
            _transcript.SelectionFont = new Font(_transcript.Font, FontStyle.Regular);
            _transcript.AppendText(frag);
            _transcript.SelectionStart = _transcript.TextLength;
            _transcript.ScrollToCaret();
        }

        private void CloseAssistant()
        {
            if (InvokeRequired) { BeginInvoke(new Action(CloseAssistant)); return; }
            if (!_assistantOpen) return;
            _transcript.AppendText(Environment.NewLine);
            _assistantOpen = false;
            _transcript.SelectionStart = _transcript.TextLength;
            _transcript.ScrollToCaret();
        }

        private void AppendLine(string who, string text, Color color)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => AppendLine(who, text, color))); return; }

            _transcript.SelectionStart = _transcript.TextLength;
            _transcript.SelectionColor = color;
            _transcript.SelectionFont = new Font(_transcript.Font, FontStyle.Bold);
            _transcript.AppendText("【" + who + "】 ");
            _transcript.SelectionFont = new Font(_transcript.Font, FontStyle.Regular);
            _transcript.AppendText(text + Environment.NewLine);
            _transcript.SelectionStart = _transcript.TextLength;
            _transcript.ScrollToCaret();
        }

        private static string Compact(JObject input)
        {
            if (input == null) return "{}";
            return Truncate(Newtonsoft.Json.JsonConvert.SerializeObject(input), 200);
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Length <= max ? s : s.Substring(0, max) + "…";
        }
    }
}