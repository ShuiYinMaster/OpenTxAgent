// TxTools.Agent / Core / GotchasStore.cs
// 踩坑记录：run_csharp 报错时自动摘录的 "反面教材"。
// 目的：把散在人脑/多次对话里的 API 陷阱、语法错误、TxNotImplementedException 沉淀成
// 结构化清单，作为 system prompt 的一部分注入(避坑清单)，模型不再重复踩同一坑。
//
// 数据流：
//   AgentLoop.RunOneTool → run_csharp 输出含错误 → GotchasStore.Record → 落 gotchas.json
//   BuildSystemPromptWithMemory → TopN → 每轮注入到 system prompt "避坑清单"
//   AI 学到正解 → add_gotcha_correction 工具 → GotchasStore.AddCorrection
//   对话末萃取 → LessonExtractor 补充 Correction
//
// 路径策略与 KeyStore/RecipeStore 一致(优先插件目录，回退 LocalAppData)。

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TxTools.Agent.Core
{
    public sealed class Gotcha
    {
        public string Id { get; set; }
        /// <summary>去重签名。例：CS0117:TxCollisionRoot.CollisionPairs 或 TxNotImplementedException:TxJoint.Name</summary>
        public string Signature { get; set; }
        /// <summary>错误分类：CS0117 / TxNotImplementedException / 编译失败 / runtime</summary>
        public string ErrorType { get; set; }
        /// <summary>完整错误消息(截 300 字)。</summary>
        public string ErrorMessage { get; set; }
        /// <summary>触发的代码片段(截 500 字)，尽量定位到出错行附近。</summary>
        public string CodeSnippet { get; set; }
        /// <summary>正确用法(初始空，AI 学到后通过 add_gotcha_correction 或萃取补充)。</summary>
        public string Correction { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastHitUtc { get; set; }
        /// <summary>被踩次数。用于排序注入优先级。</summary>
        public int HitCount { get; set; }
        public string ConvId { get; set; }
    }

    public static class GotchasStore
    {
        private const string FileName = "gotchas.json";

        public static List<Gotcha> All()
        {
            foreach (var path in CandidatePaths())
            {
                try
                {
                    if (!File.Exists(path)) continue;
                    var list = JsonConvert.DeserializeObject<List<Gotcha>>(File.ReadAllText(path, Encoding.UTF8));
                    if (list != null) return list;
                }
                catch { }
            }
            return new List<Gotcha>();
        }

        /// <summary>
        /// 从错误输出+代码里自动提炼一条 Gotcha 并落库。
        /// 已存在同签名 → HitCount++ 并更新 LastHitUtc；新签名 → 新增一条。
        /// 提取不出签名(如输出根本不像错误)则不入库,返回 null。
        /// </summary>
        public static Gotcha Record(string code, string errorOutput, string convId)
        {
            var sig = ExtractSignature(errorOutput, code);
            if (string.IsNullOrEmpty(sig)) return null;

            var all = All();
            var existing = all.FirstOrDefault(g => string.Equals(g.Signature, sig, StringComparison.Ordinal));
            if (existing != null)
            {
                existing.HitCount++;
                existing.LastHitUtc = DateTime.UtcNow;
                SaveAll(all);
                return existing;
            }

            var g = new Gotcha
            {
                Id = "gc_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                Signature = sig,
                ErrorType = ExtractErrorType(errorOutput),
                ErrorMessage = Truncate(errorOutput, 300),
                CodeSnippet = Truncate(ExtractOffendingLines(code, errorOutput), 500),
                Correction = "",
                CreatedUtc = DateTime.UtcNow,
                LastHitUtc = DateTime.UtcNow,
                HitCount = 1,
                ConvId = convId
            };
            all.Add(g);
            SaveAll(all);
            return g;
        }

        /// <summary>为已存在签名补充正确用法。返回是否找到并更新。</summary>
        public static bool AddCorrection(string signature, string correction)
        {
            if (string.IsNullOrWhiteSpace(signature)) return false;
            var all = All();
            var g = all.FirstOrDefault(x =>
                string.Equals(x.Signature, signature, StringComparison.OrdinalIgnoreCase));
            if (g == null) return false;
            g.Correction = correction ?? "";
            SaveAll(all);
            return true;
        }

        public static bool Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            var all = All();
            int n = all.RemoveAll(g => string.Equals(g.Id, id, StringComparison.OrdinalIgnoreCase));
            if (n == 0) return false;
            SaveAll(all);
            return true;
        }

        /// <summary>
        /// 取"最值得注入 prompt"的 Top-N：
        /// HitCount 高 → 常踩，必须记住；有 Correction → 能给出正解，价值 +5；
        /// 最近 30 天有被踩过 → 仍活跃 +2。
        /// </summary>
        public static List<Gotcha> TopN(int n)
        {
            return All()
                .OrderByDescending(g =>
                    g.HitCount * 3.0
                    + (!string.IsNullOrEmpty(g.Correction) ? 5.0 : 0.0)
                    + ((DateTime.UtcNow - g.LastHitUtc).TotalDays < 30 ? 2.0 : 0.0))
                .Take(n)
                .ToList();
        }

        // ── 签名与分类的启发式提取 ──

        private static readonly Regex CsErrorPattern =
            new Regex(@"CS(\d{4})", RegexOptions.Compiled);
        private static readonly Regex TypeMemberPattern =
            new Regex(@"([A-Z][A-Za-z0-9_]+)\.([A-Za-z_][A-Za-z0-9_]+)", RegexOptions.Compiled);
        private static readonly Regex TxNotImplPattern =
            new Regex(@"TxNotImplementedException", RegexOptions.Compiled);
        private static readonly Regex ExceptionTypePattern =
            new Regex(@"([A-Z][A-Za-z0-9_]+Exception)", RegexOptions.Compiled);
        private static readonly Regex LineColPattern =
            new Regex(@"\((\d+),\d+\)", RegexOptions.Compiled);

        /// <summary>提取错误签名。抽不出时返回 null,表示这条错误不值得入库。</summary>
        public static string ExtractSignature(string errorOutput, string code)
        {
            if (string.IsNullOrEmpty(errorOutput)) return null;

            // 1) TxNotImplementedException → 找目标 API
            if (TxNotImplPattern.IsMatch(errorOutput))
            {
                var tm = TypeMemberPattern.Match(errorOutput);
                var target = tm.Success ? tm.Value : FirstTypeMemberInCode(code);
                if (string.IsNullOrEmpty(target)) target = "unknown";
                return "TxNotImplementedException:" + target;
            }

            // 2) C# 编译错误 CSxxxx
            var cs = CsErrorPattern.Match(errorOutput);
            if (cs.Success)
            {
                var csCode = cs.Value;
                // 优先从错误上下文抠 "Type.Member" 作为签名的可读部分
                var tm = TypeMemberPattern.Match(errorOutput);
                if (tm.Success) return csCode + ":" + tm.Value;
                // 退化：CS 号 + 首行 hash
                return csCode + ":" + ShortHash(FirstLine(errorOutput));
            }

            // 3) 其他运行时异常
            var exMatch = ExceptionTypePattern.Match(errorOutput);
            if (exMatch.Success)
            {
                return exMatch.Value + ":" + ShortHash(FirstLine(errorOutput));
            }

            return null;
        }

        private static string ExtractErrorType(string errorOutput)
        {
            if (string.IsNullOrEmpty(errorOutput)) return "unknown";
            var cs = CsErrorPattern.Match(errorOutput);
            if (cs.Success) return cs.Value;
            if (TxNotImplPattern.IsMatch(errorOutput)) return "TxNotImplementedException";
            var ex = ExceptionTypePattern.Match(errorOutput);
            if (ex.Success) return ex.Value;
            if (errorOutput.IndexOf("编译失败", StringComparison.Ordinal) >= 0) return "编译失败";
            return "runtime";
        }

        /// <summary>从代码里抠出出错行附近的片段(带行号则精准定位;否则前 8 行)。</summary>
        private static string ExtractOffendingLines(string code, string errorOutput)
        {
            if (string.IsNullOrEmpty(code)) return "";
            var lines = code.Split(new[] { '\n' }, StringSplitOptions.None);

            var m = LineColPattern.Match(errorOutput ?? "");
            if (m.Success)
            {
                int ln;
                if (int.TryParse(m.Groups[1].Value, out ln) && ln > 0 && ln <= lines.Length)
                {
                    int from = Math.Max(0, ln - 2);
                    int to = Math.Min(lines.Length - 1, ln + 1);
                    var sb = new StringBuilder();
                    for (int i = from; i <= to; i++) sb.AppendLine(lines[i].TrimEnd('\r'));
                    return sb.ToString().TrimEnd();
                }
            }

            var head = new StringBuilder();
            for (int i = 0; i < Math.Min(lines.Length, 8); i++)
                head.AppendLine(lines[i].TrimEnd('\r'));
            return head.ToString().TrimEnd();
        }

        private static string FirstTypeMemberInCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var m = TypeMemberPattern.Match(code);
            return m.Success ? m.Value : null;
        }

        private static string FirstLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var nl = s.IndexOf('\n');
            return nl >= 0 ? s.Substring(0, nl).Trim() : s.Trim();
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max) + "…";
        }

        private static string ShortHash(string s)
        {
            if (string.IsNullOrEmpty(s)) return "0";
            unchecked
            {
                int h = 5381;
                foreach (var c in s) h = ((h << 5) + h) ^ c;
                return h.ToString("x8");
            }
        }

        // ── 持久化 ──

        private static void SaveAll(List<Gotcha> all)
        {
            var json = JsonConvert.SerializeObject(all ?? new List<Gotcha>(), Formatting.Indented);
            foreach (var path in CandidatePaths())
            {
                try
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    File.WriteAllText(path, json, Encoding.UTF8);
                    return;
                }
                catch { }
            }
        }

        private static string[] CandidatePaths()
        {
            string pluginDir = null;
            try { pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
            catch { }

            var localDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TxTools.Agent");

            if (string.IsNullOrEmpty(pluginDir))
                return new[] { Path.Combine(localDir, FileName) };

            return new[]
            {
                Path.Combine(pluginDir, FileName),
                Path.Combine(localDir, FileName)
            };
        }
    }
}
