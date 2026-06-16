// TxAgent / Tools / SnippetTools.cs
// 代码片段库工具：让 AI 把摸索出的可用 run_csharp 代码存下来，并在新需求时检索复用。
// 都是只读(只读写本地文本，不改场景)。

using Newtonsoft.Json.Linq;
using System.Text;
using TxAgent.Core;

namespace TxAgent.Tools
{
    /// <summary>保存一段可复用的 C# 代码到片段库。</summary>
    public sealed class SaveSnippetTool : TxAgentToolBase
    {
        public override string Name { get { return "save_snippet"; } }

        public override string Description
        {
            get
            {
                return "把一段验证可用的 run_csharp 代码存入片段库以便日后复用(按 name 覆盖)。" +
                       "当你通过探查 API + run_csharp 跑通了一个有价值的做法，应当用它存下来：" +
                       "name 简短可检索，description 说明用途/前提，code 为可直接交给 run_csharp 执行的方法体。";
            }
        }

        public override bool IsReadOnly { get { return true; } }

        public override JObject InputSchema
        {
            get
            {
                return JObject.Parse(@"{
                    ""type"": ""object"",
                    ""properties"": {
                        ""name"": { ""type"": ""string"", ""description"": ""片段名(唯一, 简短可检索)"" },
                        ""description"": { ""type"": ""string"", ""description"": ""用途/前提/注意事项"" },
                        ""code"": { ""type"": ""string"", ""description"": ""可直接交给 run_csharp 的 C# 5 方法体"" }
                    },
                    ""required"": [""name"", ""code""]
                }");
            }
        }

        public override string Execute(JObject input)
        {
            var name = GetString(input, "name", null);
            var code = GetString(input, "code", null);
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
                return "name 和 code 不能为空。";
            SnippetStore.Upsert(new Snippet
            {
                Name = name.Trim(),
                Description = GetString(input, "description", ""),
                Code = code
            });
            return "已保存片段: " + name.Trim();
        }
    }

    /// <summary>列出片段库(可按关键字过滤)，只给名称与说明，不含代码。</summary>
    public sealed class ListSnippetsTool : TxAgentToolBase
    {
        public override string Name { get { return "list_snippets"; } }

        public override string Description
        {
            get { return "列出已保存的代码片段(name + description，可按 keyword 过滤)。遇到新需求先查这里有没有现成可复用的做法。"; }
        }

        public override bool IsReadOnly { get { return true; } }

        public override JObject InputSchema
        {
            get
            {
                return JObject.Parse(@"{
                    ""type"": ""object"",
                    ""properties"": {
                        ""keyword"": { ""type"": ""string"", ""description"": ""按名称/说明模糊过滤, 留空列全部"" }
                    }
                }");
            }
        }

        public override string Execute(JObject input)
        {
            var list = SnippetStore.Find(GetString(input, "keyword", null));
            if (list.Count == 0) return "片段库为空（或无匹配）。";
            var sb = new StringBuilder();
            sb.AppendLine("片段 " + list.Count + " 条：");
            foreach (var s in list)
                sb.AppendLine("• " + s.Name + " — " + (string.IsNullOrEmpty(s.Description) ? "(无说明)" : s.Description));
            return sb.ToString();
        }
    }

    /// <summary>取出某片段的完整代码，用于直接交给 run_csharp。</summary>
    public sealed class GetSnippetTool : TxAgentToolBase
    {
        public override string Name { get { return "get_snippet"; } }

        public override string Description
        {
            get { return "按 name 取出某片段的完整代码(可直接或稍改后交给 run_csharp 执行)。"; }
        }

        public override bool IsReadOnly { get { return true; } }

        public override JObject InputSchema
        {
            get
            {
                return JObject.Parse(@"{
                    ""type"": ""object"",
                    ""properties"": {
                        ""name"": { ""type"": ""string"", ""description"": ""片段名"" }
                    },
                    ""required"": [""name""]
                }");
            }
        }

        public override string Execute(JObject input)
        {
            var name = GetString(input, "name", null);
            var snip = SnippetStore.Get(name);
            if (snip == null) return "未找到片段: " + name;
            var sb = new StringBuilder();
            sb.AppendLine("片段: " + snip.Name);
            if (!string.IsNullOrEmpty(snip.Description)) sb.AppendLine("说明: " + snip.Description);
            sb.AppendLine("--- 代码 ---");
            sb.Append(snip.Code);
            return sb.ToString();
        }
    }
}
