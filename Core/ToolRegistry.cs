// TxAgent / Core / ToolRegistry.cs
// 工具注册表：按名注册/查找，并构造发给 API 的工具声明列表。

using System;
using System.Collections.Generic;

namespace TxAgent.Core
{
    public sealed class ToolRegistry
    {
        private readonly Dictionary<string, ITxAgentTool> _map =
            new Dictionary<string, ITxAgentTool>(StringComparer.Ordinal);

        public int Count { get { return _map.Count; } }

        public void Register(ITxAgentTool tool)
        {
            if (tool == null) throw new ArgumentNullException(nameof(tool));
            if (string.IsNullOrWhiteSpace(tool.Name))
                throw new ArgumentException("工具必须有非空的 Name。");
            _map[tool.Name] = tool; // 同名后注册者覆盖
        }

        public bool TryGet(string name, out ITxAgentTool tool)
        {
            return _map.TryGetValue(name ?? string.Empty, out tool);
        }

        public bool Remove(string name)
        {
            return _map.Remove(name ?? string.Empty);
        }

        public List<ToolDef> ToToolDefs()
        {
            var list = new List<ToolDef>(_map.Count);
            foreach (var t in _map.Values)
            {
                list.Add(new ToolDef
                {
                    Type = "function",
                    Function = new FunctionDef
                    {
                        Name = t.Name,
                        Description = t.Description,
                        Parameters = t.InputSchema
                    }
                });
            }
            return list;
        }
    }
}
