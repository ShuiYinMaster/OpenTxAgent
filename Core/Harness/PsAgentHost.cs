// TxTools.Agent / Core / Harness / PsAgentHost.cs
// IAgentHost 的 Process Simulate 实现。
// 把"主线程封送 / 审批 / 回滚点 / 日志"这套宿主能力接到 TxAgent.Core 的 harness 上。
// 这是 readme「三步接入」第 1 步的落地:TxAgent 不需要改任何 26 个工具,
// 只要提供一个 IAgentHost 适配即可被新 harness 驱动。

using System;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Tecnomatix.Engineering;
using TxAgent.Core;            // IAgentHost / HostMode / RestorePoint
using TxTools.Agent.Core;     // PsContext / AuditLog

namespace TxTools.Agent.Harness
{
    public sealed class PsAgentHost : IAgentHost
    {
        private readonly SynchronizationContext _ctx;

        /// <summary>
        /// 审批回调: (工具名, 参数) -> 是否放行。由 UI 层(DriverLoop 桥)注入,
        /// 复用现有 HTML 审批弹窗(含 auto_safe / auto_all 模式)。
        /// 未设置时退回 WinForms MessageBox。
        /// </summary>
        public Func<string, JObject, bool> ConfirmRequest;

        /// <summary>变更类工具自动通过白名单(对应旧 AgentOptions.AutoApproveTools)。
        /// 仅作为 ConfirmRequest 未注入时的 fallback。</summary>
        public System.Collections.Generic.HashSet<string> AutoApproveTools { get; private set; }

        public PsAgentHost(SynchronizationContext ctx)
        {
            _ctx = ctx ?? SynchronizationContext.Current ?? new SynchronizationContext();
            AutoApproveTools = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
        }

        // ── 主线程封送 ──

        public void Invoke(Action action)
        {
            Exception captured = null;
            _ctx.Send(delegate (object s) {
                try { action(); }
                catch (Exception e) { captured = e; }
            }, null);
            if (captured != null) throw captured;
        }

        public T Invoke<T>(Func<T> func)
        {
            T value = default(T);
            Exception captured = null;
            _ctx.Send(delegate (object s) {
                try { value = func(); }
                catch (Exception e) { captured = e; }
            }, null);
            if (captured != null) throw captured;
            return value;
        }

        // ── 运行模式 ──

        public HostMode Mode
        {
            get { return IsConnectedToServer() ? HostMode.Connected : HostMode.Standalone; }
        }

        private bool IsConnectedToServer()
        {
            // 在线模式(Teamcenter / eMServer)的回滚走 Undo Checkout;
            // 这里只做尽力探测,失败一律返回 Standalone(本地文件模式)。
            try
            {
                var t = typeof(TxApplication);
                var prop = t.GetProperty("IsTeamcenterConnected") ?? t.GetProperty("TeamcenterConnected");
                if (prop != null && prop.PropertyType == typeof(bool))
                    return (bool)prop.GetValue(null, null);
            }
            catch { }
            return false;
        }

        // ── 用户确认 ──

        public bool Confirm(string title, string detail, bool destructive)
        {
            string name, argsJson;
            ParseConfirmDetail(detail, out name, out argsJson);

            JObject input = null;
            if (!string.IsNullOrEmpty(argsJson))
            {
                try { input = JObject.Parse(argsJson); }
                catch { input = new JObject(); }
            }

            if (ConfirmRequest != null)
                return ConfirmRequest(name, input);

            // fallback: 原生弹窗
            var kind = destructive ? "破坏性" : "变更";
            var msg = "助手请求执行一个" + kind + "操作：\n\n" + (detail ?? "") +
                      "\n\n是否允许？";
            return MessageBox.Show(msg, "操作确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                   == DialogResult.Yes;
        }

        /// <summary>harness 的确认 detail 固定为 "工具：&lt;name&gt;\n参数：&lt;json&gt;" 格式,这里解析出工具名与参数。</summary>
        private static void ParseConfirmDetail(string detail, out string name, out string argsJson)
        {
            name = null;
            argsJson = null;
            if (string.IsNullOrEmpty(detail)) return;
            var lines = detail.Split(new[] { '\n' }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.StartsWith("工具：")) name = line.Substring(3).Trim();
                else if (line.StartsWith("参数：")) argsJson = line.Substring(3).Trim();
            }
        }

        // ── 回滚点 ──

        public RestorePoint CreateRestorePoint(string reason)
        {
            try
            {
                // Standalone: 保存当前工程(PDPS 保存会生成新文件,旧文件进回收站)。
                var t = typeof(TxApplication);
                var docProp = t.GetProperty("ActiveDocument");
                if (docProp == null) return RestorePoint.None("无法获取活动文档,未建立回滚点。");

                var doc = docProp.GetValue(null, null);
                if (doc == null) return RestorePoint.None("当前没有打开的工程,未建立回滚点。");

                var saveMethod = doc.GetType().GetMethod("Save", Type.EmptyTypes);
                if (saveMethod != null) saveMethod.Invoke(doc, null);

                string path = null;
                var pathProp = doc.GetType().GetProperty("Path")
                            ?? doc.GetType().GetProperty("FileName")
                            ?? doc.GetType().GetProperty("Name");
                if (pathProp != null)
                {
                    try { path = pathProp.GetValue(doc, null) as string; } catch { }
                }

                return new RestorePoint
                {
                    Created = true,
                    Mode = this.Mode,
                    TimeUtc = DateTime.UtcNow,
                    FilePath = path,
                    HowToRollback = string.IsNullOrEmpty(path)
                        ? "已保存当前工程(PDPS Save),可在文件历史/回收站找回旧版本。"
                        : "已保存当前工程: " + path + " (旧版本在回收站/文件历史)"
                };
            }
            catch (Exception ex)
            {
                return RestorePoint.None("保存失败,未建立回滚点: " + ex.Message);
            }
        }

        // ── 日志 ──

        public void Log(string level, string message)
        {
            try { AuditLog.Write("[" + (level ?? "info") + "] " + (message ?? "")); }
            catch { }
            System.Diagnostics.Debug.WriteLine("[TxAgent.Harness:" + (level ?? "info") + "] " + (message ?? ""));
        }
    }
}
