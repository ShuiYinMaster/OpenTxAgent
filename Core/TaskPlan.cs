// TxAgent / Core / TaskPlan.cs
// 轻量任务计划：让 agent 把复杂多步任务拆成带状态的清单并随进度更新。
// 计划随对话存活(也随 conversation 记忆持久化，因为它以工具结果形式进入历史)。

using System.Collections.Generic;
using System.Text;

namespace TxAgent.Core
{
    public sealed class TaskItem
    {
        public string Text { get; set; }
        public bool Done { get; set; }
    }

    public static class TaskPlan
    {
        private static readonly List<TaskItem> _items = new List<TaskItem>();

        public static string Update(IEnumerable<TaskItem> items)
        {
            _items.Clear();
            if (items != null)
                foreach (var it in items)
                    if (it != null && !string.IsNullOrWhiteSpace(it.Text))
                        _items.Add(new TaskItem { Text = it.Text.Trim(), Done = it.Done });
            return Render();
        }

        public static string Render()
        {
            if (_items.Count == 0) return "（当前无计划）";
            var sb = new StringBuilder();
            sb.AppendLine("当前计划：");
            int done = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                var it = _items[i];
                if (it.Done) done++;
                sb.AppendLine((i + 1) + ". [" + (it.Done ? "x" : " ") + "] " + it.Text);
            }
            sb.Append("进度: " + done + "/" + _items.Count);
            return sb.ToString();
        }
    }
}
