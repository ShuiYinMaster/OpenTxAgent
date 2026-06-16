// TxAgent / TxAgentCommand.cs
// PS 插件入口。注意 TxButtonCommand 在 PS 2402 中真正的抽象成员只有 Name / Category / Execute(object)，
// 不要声明 DisplayName / InternalName / Tooltip (会触发 CS0115/CS0534)。

using System;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Tecnomatix.Engineering;
using TxAgent.Core;
using TxAgent.Tools;
using TxAgent.UI;

namespace TxAgent
{
    public sealed class TxAgentCommand : TxButtonCommand
    {
        private static TxAgentForm _form;

        public override string Name { get { return "TxAgent"; } }
        public override string Category { get { return "TxTools"; } }

        public override void Execute(object cmdParams)
        {
            // 在 PS 主线程捕获 SynchronizationContext，供所有工具把 PS 调用路由回主线程
            // (对齐 ExportGunCmd/ExportService 的做法)。
            var psCtx = SynchronizationContext.Current ?? new SynchronizationContext();
            PsContext.Current = new PsContext(psCtx);

            if (_form != null && !_form.IsDisposed)
            {
                // 已打开就前置激活、并从最小化还原，不重复创建。
                if (_form.WindowState == FormWindowState.Minimized)
                    _form.WindowState = FormWindowState.Normal;
                _form.BringToFront();
                _form.Activate();
                _form.Focus();
                return;
            }

            var tools = BuildToolRegistry();
            _form = new TxAgentForm(psCtx, tools);
            _form.Name = _form.GetType().FullName;   // 跨插件窗口尺寸串扰修复(双保险)
            _form.FormClosed += (s, e) => _form = null;

            IWin32Window owner = TryGetPsMainWindow();
            if (owner != null) _form.Show(owner);
            else _form.Show();

            try { TxApplication.StatusBarMessage = "TxAgent 已启动"; } catch { }
        }

        /// <summary>注册全部工具：原子工具 + 配方机制 + 已保存的配方。</summary>
        private static ToolRegistry BuildToolRegistry()
        {
            var reg = new ToolRegistry();

            // 1) 原子工具 (能力的基本积木；越细，配方能组合出的解法越多)
            reg.Register(new SceneQueryTool());     // 只读：查询选中对象 / 当前文档
            reg.Register(new CountObjectsTool());    // 只读：全场景按类型统计/枚举(数机器人等)
            reg.Register(new ListChildrenTool());    // 只读：展开组件数子对象(数 CD_L 下设备)
            reg.Register(new ListOperationsTool());  // 只读：选中操作 名称/类型/工具
            reg.Register(new CountPointsTool());     // 只读：按类型统计点数
            reg.Register(new ListTcpOptionsTool());  // 只读：操作的 TCP 选项
            reg.Register(new CheckReachabilityTool()); // 只读：快速可达性摘要
            reg.Register(new GetReferenceFrameTool()); // 只读：当前参考坐标系
            reg.Register(new ListTypesTool());       // 只读：搜 SDK 类型
            reg.Register(new InspectTypeTool());     // 只读：列类型成员
            reg.Register(new InspectObjectTool());   // 只读：探查活动对象
            reg.Register(new UpdatePlanTool());      // 任务：维护多步计划
            reg.Register(new SaveSnippetTool());     // 记忆：存可复用代码片段
            reg.Register(new ListSnippetsTool());    // 记忆：列片段
            reg.Register(new GetSnippetTool());      // 记忆：取片段代码
            reg.Register(new ExportTableTool());     // 导出：把汇总信息写成 xlsx
            reg.Register(new ExportPointsExcelTool()); // 导出：选中操作的焊点坐标(ExportGun 同款)
            reg.Register(new ExportObjectListTool());  // 导出：遍历→对象清单一步导出(机器人/设备清单)
            reg.Register(new SelectObjectsTool());   // 动作：按名选中(查到→选中→操作)
            reg.Register(new AlignDevicesZTool());  // 变更：Z 对齐 (需审批, 可撤销)
            reg.Register(new RunCSharpTool());       // 变更：写 C# 在 PS 内执行 (兜底, 强制审批, 可撤销, 审计)
            // reg.Register(new RunReachabilityTool());  // TODO: 包装 RobotReachabilityChecker
            // reg.Register(new ExportGunTool());        // TODO: 包装 ExportService

            // 2) 配方机制：让 agent 自己把多步操作存成可复用工具
            reg.Register(new ListRecipesTool());
            reg.Register(new SaveRecipeTool(reg));
            reg.Register(new DeleteRecipeTool(reg));

            // 3) 加载已保存的配方，注册成工具 (启动即可用)
            foreach (var recipe in RecipeStore.Load())
                reg.Register(new RecipeTool(recipe, reg));

            // 4) 预置内置配方(仅注册到内存，不写盘；启动即在，可被 list_recipes 看到)
            SeedDefaultRecipes(reg);

            return reg;
        }

        /// <summary>预置几条开箱即用的只读配方(都由现有只读工具组合，免审批)。</summary>
        private static void SeedDefaultRecipes(ToolRegistry reg)
        {
            // 导出前点检：列操作 + 焊点数 + 参考系
            SeedIfAbsent(reg, "preflight_check", "导出前点检：列出选中操作、统计焊点数、确认参考坐标系。",
                new[]
                {
                    Step("list_operations", null),
                    Step("count_points", new JObject { ["point_type"] = "WeldPoint" }),
                    Step("get_reference_frame", null)
                });

            // 场景概览：全场景类型直方图 + 当前文档
            SeedIfAbsent(reg, "scene_overview", "场景概览：全场景对象类型直方图 + 当前文档信息。",
                new[]
                {
                    Step("count_objects", null),
                    Step("query_scene", new JObject { ["scope"] = "document" })
                });
        }

        private static RecipeStep Step(string tool, JObject input)
        {
            return new RecipeStep { Tool = tool, Input = input ?? new JObject() };
        }

        private static void SeedIfAbsent(ToolRegistry reg, string name, string desc, RecipeStep[] steps)
        {
            ITxAgentTool existing;
            if (reg.TryGet(name, out existing)) return; // 用户已有同名(配方或工具)则不覆盖
            var recipe = new Recipe
            {
                Name = name,
                Description = desc,
                Parameters = new System.Collections.Generic.List<RecipeParam>(),
                Steps = new System.Collections.Generic.List<RecipeStep>(steps)
            };
            reg.Register(new RecipeTool(recipe, reg));
        }

        private static IWin32Window TryGetPsMainWindow()
        {
            // 反射容忍 SDK 版本差异 (静态成员 dynamic 仍在编译期解析，故必须反射)。
            // 优先 MainForm(IWin32Window)，回退 MainScreenHandle(IntPtr) 包一层。
            try
            {
                var t = typeof(TxApplication);
                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;

                var mainForm = t.GetProperty("MainForm", flags);
                if (mainForm != null)
                {
                    var w = mainForm.GetValue(null, null) as IWin32Window;
                    if (w != null) return w;
                }

                var handleProp = t.GetProperty("MainScreenHandle", flags);
                if (handleProp != null)
                {
                    var v = handleProp.GetValue(null, null);
                    if (v is IntPtr) return new Win32Window((IntPtr)v);
                }
            }
            catch { }
            return null;
        }

        /// <summary>把一个窗口句柄包成 IWin32Window，供 Form.Show(owner) 让窗口归属 PS 主窗口。</summary>
        private sealed class Win32Window : IWin32Window
        {
            public Win32Window(IntPtr handle) { Handle = handle; }
            public IntPtr Handle { get; private set; }
        }
    }
}