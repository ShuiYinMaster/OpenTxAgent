// TxTools.Agent / Core / Multi / PsInstanceRegistry.cs
//
// 多 PDPS 实例的发现与角色仲裁。
//
// ── 为什么必须"脑出去、执行器留下" ──
//   Tecnomatix API 只能在进程内主线程调用，没有任何办法从外部驱动。
//   所以插件必须留在每个 PDPS 里；能搬出去的只是 agent 的决策部分。
//
//        TxAgent 主控(脑) —— 全局只有一份
//           ├─ 命名管道 ─→ PDPS #1 插件(执行器)
//           └─ 命名管道 ─→ PDPS #2 插件(执行器)
//
//   主控自己也宿在某个 PDPS 里，不需要单独做个应用。
//
// ── 角色仲裁 ──
//   先启动的那个抢到主控，后启动的检测到已有主控就退化成纯执行器。
//   这顺带解决了"两个窗口各开一个 agent 各写各的"那个问题:
//   全局只有一个脑，对话数据自然只有一份。
//
// ── 为什么用文件注册而不是服务发现 ──
//   同机、少量实例、无网络。文件注册零依赖、可肉眼排查，
//   进程崩了留下的残留项靠"进程是否存活"判掉即可。

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TxTools.Agent.Core
{
    public sealed class PsInstanceInfo
    {
        /// <summary>PDPS 进程号。同时也是管道名的后缀。</summary>
        public int Pid { get; set; }

        /// <summary>用户可读的环境名。默认取 study 名，重名时补序号。</summary>
        public string Name { get; set; }

        /// <summary>当前打开的 study。</summary>
        public string Study { get; set; }

        /// <summary>是否为主控(脑)。全局应只有一个 true。</summary>
        public bool IsBrain { get; set; }

        public DateTime HeartbeatUtc { get; set; }

        [JsonIgnore]
        public string PipeName { get { return PsInstanceRegistry.PipeNameFor(Pid); } }

        [JsonIgnore]
        public bool IsSelf { get { return Pid == PsInstanceRegistry.SelfPid; } }

        /// <summary>进程还在 且 心跳没过期。两条都要查:强杀会留下未过期的死记录。</summary>
        [JsonIgnore]
        public bool IsAlive
        {
            get
            {
                if (DateTime.UtcNow - HeartbeatUtc > PsInstanceRegistry.Ttl) return false;
                try { Process.GetProcessById(Pid); return true; }
                catch { return false; }
            }
        }

        public override string ToString()
        {
            return Name + " [pid " + Pid + "]" + (IsBrain ? " (主控)" : "");
        }
    }

    public static class PsInstanceRegistry
    {
        public static readonly int SelfPid = Process.GetCurrentProcess().Id;

        /// <summary>心跳过期时间。超过即认为该实例已不可用。</summary>
        public static TimeSpan Ttl = TimeSpan.FromSeconds(45);

        public static string PipeNameFor(int pid) { return "TxAgent_PS_" + pid; }

        private static string Dir()
        {
            var d = Path.Combine(Path.GetTempPath(), "TxAgent.Instances");
            Directory.CreateDirectory(d);
            return d;
        }

        private static string PathFor(int pid)
        {
            return Path.Combine(Dir(), pid + ".json");
        }

        // ── 注册与心跳 ──

        /// <summary>
        /// 注册本实例。返回本实例最终的角色 —— 已有活着的主控时自动退为执行器。
        /// 【每次心跳都要重新判定】主控进程崩掉后，剩下的执行器应该有人顶上。
        /// </summary>
        public static PsInstanceInfo Register(string study, bool wantBrain)
        {
            var all = All();
            var existingBrain = all.FirstOrDefault(x => x.IsBrain && x.IsAlive && !x.IsSelf);

            var me = all.FirstOrDefault(x => x.IsSelf) ?? new PsInstanceInfo { Pid = SelfPid };
            me.Study = study;
            me.HeartbeatUtc = DateTime.UtcNow;
            me.IsBrain = wantBrain && existingBrain == null;

            if (string.IsNullOrWhiteSpace(me.Name))
                me.Name = MakeName(study, all);

            try
            {
                File.WriteAllText(PathFor(SelfPid),
                    JsonConvert.SerializeObject(me, Formatting.Indented), Encoding.UTF8);
            }
            catch { }

            return me;
        }

        /// <summary>心跳。定期调用，否则别的实例会认为本进程已死。</summary>
        public static void Heartbeat(string study)
        {
            Register(study, IsSelfBrain());
        }

        public static void Unregister()
        {
            try { var p = PathFor(SelfPid); if (File.Exists(p)) File.Delete(p); }
            catch { }
        }

        // ── 查询 ──

        /// <summary>全部注册项(含已死的，调用方按需过滤)。</summary>
        public static List<PsInstanceInfo> All()
        {
            var list = new List<PsInstanceInfo>();
            try
            {
                foreach (var f in Directory.GetFiles(Dir(), "*.json"))
                {
                    try
                    {
                        var info = JsonConvert.DeserializeObject<PsInstanceInfo>(
                            File.ReadAllText(f, Encoding.UTF8));
                        if (info != null && info.Pid > 0) list.Add(info);
                    }
                    catch { }
                }
            }
            catch { }
            return list;
        }

        /// <summary>活着的实例，按名称排序。顺手清掉死记录。</summary>
        public static List<PsInstanceInfo> Live()
        {
            var all = All();
            var live = new List<PsInstanceInfo>();

            foreach (var i in all)
            {
                if (i.IsAlive) { live.Add(i); continue; }
                try { File.Delete(PathFor(i.Pid)); } catch { }   // 顺手打扫
            }

            return live.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static PsInstanceInfo Self()
        {
            return All().FirstOrDefault(x => x.IsSelf);
        }

        public static bool IsSelfBrain()
        {
            var me = Self();
            return me != null && me.IsBrain;
        }

        public static PsInstanceInfo Brain()
        {
            return Live().FirstOrDefault(x => x.IsBrain);
        }

        /// <summary>按名称或 pid 找一个实例。</summary>
        public static PsInstanceInfo Find(string nameOrPid)
        {
            if (string.IsNullOrWhiteSpace(nameOrPid)) return null;
            var live = Live();

            int pid;
            if (int.TryParse(nameOrPid, out pid))
            {
                var byPid = live.FirstOrDefault(x => x.Pid == pid);
                if (byPid != null) return byPid;
            }

            return live.FirstOrDefault(x =>
                       string.Equals(x.Name, nameOrPid, StringComparison.OrdinalIgnoreCase))
                ?? live.FirstOrDefault(x => x.Name != null &&
                       x.Name.IndexOf(nameOrPid, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>重命名本实例。study 同名时靠它区分。</summary>
        public static void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) return;
            var me = Self();
            if (me == null) return;
            me.Name = newName.Trim();
            try
            {
                File.WriteAllText(PathFor(SelfPid),
                    JsonConvert.SerializeObject(me, Formatting.Indented), Encoding.UTF8);
            }
            catch { }
        }

        /// <summary>取 study 名作环境名;重名时补序号，否则模型没法区分两个环境。</summary>
        private static string MakeName(string study, List<PsInstanceInfo> others)
        {
            var b = string.IsNullOrWhiteSpace(study) ? "PS" : study.Trim();
            var used = new HashSet<string>(
                others.Where(x => !x.IsSelf && x.IsAlive).Select(x => x.Name ?? ""),
                StringComparer.OrdinalIgnoreCase);

            if (!used.Contains(b)) return b;
            for (int i = 2; i < 20; i++)
                if (!used.Contains(b + "#" + i)) return b + "#" + i;

            return b + "#" + SelfPid;
        }
    }
}
