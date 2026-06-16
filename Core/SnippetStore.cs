// TxAgent / Core / SnippetStore.cs
// 代码片段库：把摸索出的可用 run_csharp 代码持久化到 snippets.json，跨对话检索复用。
// 这是给 codegen 路径的“方法记忆”——摸清一次 API、存下可用代码，以后先查库、命中就直接用。
// 路径策略与 RecipeStore/KeyStore 一致(优先插件目录，回退 LocalAppData)。

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace TxAgent.Core
{
    public sealed class Snippet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime CreatedUtc { get; set; }
    }

    public static class SnippetStore
    {
        private const string FileName = "snippets.json";

        public static List<Snippet> All()
        {
            foreach (var path in CandidatePaths())
            {
                try
                {
                    if (!File.Exists(path)) continue;
                    var list = JsonConvert.DeserializeObject<List<Snippet>>(File.ReadAllText(path, Encoding.UTF8));
                    if (list != null) return list;
                }
                catch { }
            }
            return new List<Snippet>();
        }

        public static Snippet Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return All().FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public static List<Snippet> Find(string keyword)
        {
            var all = All();
            if (string.IsNullOrWhiteSpace(keyword)) return all;
            var k = keyword.Trim();
            return all.Where(s =>
                       (s.Name != null && s.Name.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0) ||
                       (s.Description != null && s.Description.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                      .ToList();
        }

        /// <summary>按名新增或覆盖一条片段。</summary>
        public static void Upsert(Snippet snippet)
        {
            if (snippet == null || string.IsNullOrWhiteSpace(snippet.Name)) return;
            var all = All();
            all.RemoveAll(s => string.Equals(s.Name, snippet.Name, StringComparison.OrdinalIgnoreCase));
            if (snippet.CreatedUtc == default(DateTime)) snippet.CreatedUtc = DateTime.UtcNow;
            all.Add(snippet);
            SaveAll(all);
        }

        public static bool Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var all = All();
            int n = all.RemoveAll(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (n == 0) return false;
            SaveAll(all);
            return true;
        }

        private static void SaveAll(List<Snippet> all)
        {
            var json = JsonConvert.SerializeObject(all ?? new List<Snippet>(), Formatting.Indented);
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
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TxAgent");

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
