// TxAgent / Ps / PsBridge.cs
// PS 场景访问门面：所有工具对 Tecnomatix.Engineering / PsReader 的调用都收敛到这里。
// 套路：dynamic + try/catch 兜 SDK 版本差异；经 PsContext.Current.Run(...) 路由回 PS 主线程。
//
// 依赖：引用 MyPlugin.ExportGun.PsReader / OperationInfo / PointType 等(仅本文件)。
// 物理树遍历照搬 PsReader.EnumDisplayableObjects 的健壮策略：
//   PhysicalRoot/ComponentRoot/ResourceRoot + GetAllDescendants(TxTypeFilter) + 无参回退。

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tecnomatix.Engineering;
using TxAgent.Core;
using MyPlugin.ExportGun;   // PsReader, OperationInfo, PointType, TcpOption ...

namespace TxAgent.Ps
{
    public static class PsBridge
    {
        private static readonly Action<string> Nolog = delegate (string s) { };

        // ───────── 基础查询 ─────────

        public static string GetSelectedObjectsSummary()
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var names = new List<string>();
                    dynamic sel = TxApplication.ActiveSelection;
                    dynamic items = sel.GetItems();
                    var en = items as IEnumerable;
                    if (en != null) foreach (var o in en) names.Add(SafeName(o));

                    if (names.Count == 0) return "当前没有选中任何对象。";
                    var sb = new StringBuilder();
                    sb.AppendLine("当前选中 " + names.Count + " 个对象：");
                    for (int i = 0; i < names.Count; i++) sb.AppendLine((i + 1) + ". " + names[i]);
                    return sb.ToString();
                }
                catch (Exception ex) { return "读取选中对象失败: " + ex.Message; }
            });
        }

        public static string GetActiveDocumentSummary()
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    dynamic doc = TxApplication.ActiveDocument;
                    if (doc == null) return "当前没有打开的文档。";
                    string name = "未知";
                    try { name = (string)doc.Name; } catch { }
                    return "当前文档: " + name;
                }
                catch (Exception ex) { return "读取文档信息失败: " + ex.Message; }
            });
        }

        // ───────── 场景树遍历 / 统计 (信息汇总核心) ─────────

        /// <summary>
        /// 按类型统计场景对象。typeKeyword 为空 -> 输出整场景类型直方图；
        /// 非空 -> 列出类型名包含该关键字的对象(机器人额外用 is TxRobot 兜)。
        /// 用它精确回答"场景里有多少机器人/夹具/…"，不要从操作列表推断。
        /// </summary>
        public static string CountObjectsByType(string typeKeyword)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var all = CollectScene(true);
                    if (all.Count == 0)
                        return "未能遍历到场景对象(可能没有打开文档，或该 SDK 版本根不可用)。";

                    if (string.IsNullOrWhiteSpace(typeKeyword))
                    {
                        var hist = Histogram(all);
                        var sb = new StringBuilder();
                        sb.AppendLine("场景对象类型统计 (共 " + all.Count + " 个)：");
                        foreach (var kv in hist.OrderByDescending(k => k.Value).Take(25))
                            sb.AppendLine("• " + kv.Key + ": " + kv.Value);
                        if (hist.Count > 25) sb.AppendLine("…(其余类型省略)");
                        return sb.ToString();
                    }

                    var key = typeKeyword.Trim();
                    bool wantRobot = key.IndexOf("robot", StringComparison.OrdinalIgnoreCase) >= 0
                                     || key.Contains("机器人");
                    var matches = new List<string>();
                    foreach (var o in all)
                    {
                        var tn = o.GetType().Name;
                        bool hit = tn.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0;
                        if (!hit && wantRobot && o is TxRobot) hit = true;
                        if (hit) matches.Add(SafeName(o) + " [" + tn + "]");
                    }

                    var sb2 = new StringBuilder();
                    sb2.AppendLine("匹配 \"" + key + "\" 的对象 " + matches.Count + " 个：");
                    int cap = Math.Min(matches.Count, 40);
                    for (int i = 0; i < cap; i++) sb2.AppendLine((i + 1) + ". " + matches[i]);
                    if (matches.Count > cap) sb2.AppendLine("…(其余 " + (matches.Count - cap) + " 个省略)");
                    return sb2.ToString();
                }
                catch (Exception ex) { return "统计场景对象失败: " + ex.Message; }
            });
        }

        /// <summary>
        /// 展开一个组件(按 name 查找，缺省用当前选中第一个)，按类型统计其子对象数量。
        /// 用它回答"CD_L 下有多少设备"这类层级问题。recursive=true 递归到底，false 仅直接子级。
        /// </summary>
        public static string ListChildren(string name, bool recursive)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    ITxObject target = null;
                    if (!string.IsNullOrWhiteSpace(name)) target = FindByName(name);
                    if (target == null) target = FirstSelected();
                    if (target == null)
                        return string.IsNullOrWhiteSpace(name)
                            ? "请先选中一个对象，或提供 name 参数。"
                            : "未找到名为 " + name + " 的对象。";

                    var kids = DescendantsOf(target, recursive);
                    if (kids == null || kids.Count == 0)
                        return SafeName(target) + " 下没有可枚举的子对象。";

                    var hist = new Dictionary<string, int>(StringComparer.Ordinal);
                    var sample = new List<string>();
                    foreach (ITxObject o in kids)
                    {
                        if (o == null) continue;
                        var tn = o.GetType().Name;
                        int c; hist[tn] = hist.TryGetValue(tn, out c) ? c + 1 : 1;
                        if (sample.Count < 40) sample.Add(SafeName(o) + " [" + tn + "]");
                    }

                    var sb = new StringBuilder();
                    sb.AppendLine(SafeName(target) + " 下" + (recursive ? "(递归)" : "(直接子级)")
                                  + "共 " + kids.Count + " 个对象，按类型：");
                    foreach (var kv in hist.OrderByDescending(k => k.Value))
                        sb.AppendLine("• " + kv.Key + ": " + kv.Value);
                    sb.AppendLine("示例：");
                    foreach (var n in sample) sb.AppendLine("  - " + n);
                    if (kids.Count > sample.Count) sb.AppendLine("  …(其余省略)");
                    return sb.ToString();
                }
                catch (Exception ex) { return "列出子对象失败: " + ex.Message; }
            });
        }

        // ───────── PsReader 支撑的只读原语 ─────────

        public static string ListOperations()
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var ops = PsReader.GetOperationsFromSelection(Nolog);
                    if (ops == null || ops.Count == 0) return "当前选择里没有可识别的操作。";

                    var sb = new StringBuilder();
                    sb.AppendLine("选中操作 " + ops.Count + " 个：");
                    int i = 0;
                    foreach (var op in ops)
                    {
                        i++;
                        string tool = "";
                        try { tool = PsReader.GetToolNameFromOperation(op); } catch { }
                        sb.Append(i).Append(". ").Append(op.Name);
                        if (!string.IsNullOrEmpty(op.TypeLabel)) sb.Append(" [").Append(op.TypeLabel).Append("]");
                        if (!string.IsNullOrEmpty(tool)) sb.Append("  工具=").Append(tool);
                        sb.AppendLine();
                    }
                    return sb.ToString();
                }
                catch (Exception ex) { return "读取操作失败: " + ex.Message; }
            });
        }

        public static string CountPoints(string pointType, bool useMfgName)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var ops = PsReader.GetOperationsFromSelection(Nolog);
                    if (ops == null || ops.Count == 0) return "当前选择里没有可识别的操作。";

                    var pt = ParsePointType(pointType);
                    int total = 0;
                    var sb = new StringBuilder();
                    sb.AppendLine("点统计 (类型=" + pt + ", useMfgName=" + useMfgName + ")：");
                    foreach (var op in ops)
                    {
                        op.Points.Clear();
                        PsReader.FillPoints(op, pt, useMfgName, Nolog);
                        total += op.Points.Count;
                        sb.AppendLine("• " + op.Name + ": " + op.Points.Count + " 点");
                    }
                    sb.Append("合计: ").Append(total).Append(" 点");
                    return sb.ToString();
                }
                catch (Exception ex) { return "统计点失败: " + ex.Message; }
            });
        }

        public static string ListTcpOptions()
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var ops = PsReader.GetOperationsFromSelection(Nolog);
                    if (ops == null || ops.Count == 0) return "当前选择里没有可识别的操作。";
                    var op = ops[0];
                    var tcps = PsReader.EnumerateTcpOptions(op, Nolog);
                    if (tcps == null || tcps.Count == 0) return "操作 " + op.Name + " 没有可用的 TCP 选项。";

                    var sb = new StringBuilder();
                    sb.AppendLine("操作 " + op.Name + " 的 TCP 选项 " + tcps.Count + " 个：");
                    foreach (var t in tcps) sb.AppendLine("• " + t.Name + (t.IsDefault ? "  (默认)" : ""));
                    return sb.ToString();
                }
                catch (Exception ex) { return "读取 TCP 选项失败: " + ex.Message; }
            });
        }

        public static string GetReferenceFrameSummary()
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var rf = PsReader.GetReferenceFrame();
                    if (rf == null || rf.Item2 == null) return "未取得参考坐标系，按世界坐标系处理。";
                    bool isWorld = PsReader.IsIdentity(rf.Item2);
                    return "参考坐标系: " + (rf.Item1 ?? "未知") + (isWorld ? " (等同世界系)" : "");
                }
                catch (Exception ex) { return "读取参考坐标系失败: " + ex.Message; }
            });
        }

        // ───────── 动作：选中 / 真实焊点导出 ─────────

        /// <summary>
        /// 快速可达性摘要：对选中操作的各点位用 robot.GetPoseAtLocation 判定可达(只读，不驱动机器人)。
        /// 不含 RobotReachabilityChecker 的关节余量/奇异/碰撞等完整分析。
        /// </summary>
        /// <summary>
        /// 快速可达性摘要。operationName 非空时在操作树(OperationRoot)里按名查找该操作并检查其下机器人点位；
        /// 为空时回退到当前选中的操作。逐点用 GetPoseAtLocation 判定(只读、不驱动机器人)。
        /// </summary>
        public static string CheckReachability(string operationName)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var roots = new List<ITxObject>();
                    if (!string.IsNullOrWhiteSpace(operationName))
                    {
                        var key = operationName.Trim();
                        foreach (var o in CollectOperations())
                        {
                            var n = SafeName(o);
                            if (n != null && n.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) roots.Add(o);
                        }
                        if (roots.Count == 0) return "在操作树里未找到名称含 \"" + key + "\" 的操作。";
                    }
                    else
                    {
                        var sel = PsReader.GetOperationsFromSelection(Nolog);
                        if (sel == null || sel.Count == 0)
                            return "未提供操作名，且当前没有选中操作。请给出 operation 名(如 OP120)，或先在操作树里选中操作。";
                        foreach (var op in sel) if (op.PsObject != null) roots.Add(op.PsObject);
                    }

                    // 去重并尽量只保留"最外层"匹配(避免父子重复统计)：简单去重即可。
                    var seen = new HashSet<int>();
                    var sb = new StringBuilder();
                    int gReach = 0, gTotal = 0, checkedOps = 0;
                    foreach (var root in roots)
                    {
                        if (!seen.Add(RuntimeHelpers.GetHashCode(root))) continue;
                        var locs = EnumerateLocationOps(root);
                        if (locs.Count == 0) continue;

                        var robot = FindRobotForOps(root, locs);
                        var rootName = SafeName(root);
                        checkedOps++;
                        if (robot == null) { sb.AppendLine("• " + rootName + ": " + locs.Count + " 点，未找到绑定机器人"); continue; }

                        int reach = 0;
                        var bad = new List<string>();
                        foreach (var loc in locs)
                        {
                            bool ok = false;
                            try { ok = robot.GetPoseAtLocation(loc) != null; } catch { ok = false; }
                            if (ok) reach++;
                            else if (bad.Count < 10) bad.Add(SafeName(loc));
                        }
                        gReach += reach; gTotal += locs.Count;
                        sb.Append("• ").Append(rootName).Append(" [机器人 ").Append(SafeName(robot)).Append("]: 可达 ")
                          .Append(reach).Append("/").Append(locs.Count);
                        if (bad.Count > 0) sb.Append("  不可达: ").Append(string.Join(", ", bad));
                        sb.AppendLine();
                    }

                    if (checkedOps == 0) return "匹配到的操作下没有机器人点位可检查(可能不是机器人操作)。";
                    sb.AppendLine("合计: 可达 " + gReach + "/" + gTotal + " 点");
                    sb.Append("(快速摘要：GetPoseAtLocation 判定，不含关节余量/奇异/碰撞分析)");
                    return sb.ToString();
                }
                catch (Exception ex) { return "可达性检查失败: " + ex.Message; }
            });
        }

        /// <summary>在操作树(OperationRoot)里按名查找操作。回答"OP120 这个操作在哪/是什么"。</summary>
        public static string FindOperations(string keyword)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var ops = CollectOperations();
                    if (ops.Count == 0) return "未能遍历到操作树(OperationRoot)。";

                    var key = (keyword ?? "").Trim();
                    var matches = new List<ITxObject>();
                    foreach (var o in ops)
                    {
                        var n = SafeName(o);
                        if (key.Length == 0 || (n != null && n.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0))
                            matches.Add(o);
                    }
                    if (matches.Count == 0) return "操作树里没有名称含 \"" + key + "\" 的操作。";

                    var sb = new StringBuilder();
                    sb.AppendLine("匹配操作 " + matches.Count + " 个：");
                    int cap = Math.Min(matches.Count, 40);
                    for (int i = 0; i < cap; i++)
                        sb.AppendLine((i + 1) + ". " + SafeName(matches[i]) + " [" + matches[i].GetType().Name + "]");
                    if (matches.Count > cap) sb.AppendLine("…(其余 " + (matches.Count - cap) + " 个省略)");
                    return sb.ToString();
                }
                catch (Exception ex) { return "查找操作失败: " + ex.Message; }
            });
        }

        /// <summary>遍历操作树(OperationRoot)的全部后代操作。</summary>
        private static List<ITxObject> CollectOperations()
        {
            var list = new List<ITxObject>();
            try
            {
                dynamic doc = TxApplication.ActiveDocument;
                if (doc == null) return list;
                var opRoot = doc.OperationRoot as ITxObjectCollection; // TxOperationRoot : ITxObjectCollection
                if (opRoot != null)
                {
                    var all = opRoot.GetAllDescendants(new TxTypeFilter(typeof(ITxObject)));
                    if (all != null) foreach (ITxObject o in all) if (o != null) list.Add(o);
                }
            }
            catch { }
            return list;
        }

        private static TxRobot FindRobotForOps(ITxObject root, List<ITxRoboticLocationOperation> locs)
        {
            try { var r = PsReader.FindRobotForOperation(root); if (r != null) return r; } catch { }
            if (locs != null)
                foreach (var loc in locs)
                {
                    try { var r = PsReader.FindRobotForOperation(loc); if (r != null) return r; } catch { }
                }
            return null;
        }

        private static List<ITxRoboticLocationOperation> EnumerateLocationOps(ITxObject operation)
        {
            var list = new List<ITxRoboticLocationOperation>();
            if (operation == null) return list;
            var f = new TxTypeFilter(typeof(ITxRoboticLocationOperation));

            if (operation is ITxCompoundOperation comp)
            {
                try
                {
                    var objs = comp.GetAllDescendants(f);
                    if (objs != null) foreach (ITxObject o in objs) if (o is ITxRoboticLocationOperation l) list.Add(l);
                }
                catch { }
            }
            if (list.Count == 0)
            {
                try
                {
                    dynamic d = operation;
                    TxObjectList objs = d.GetAllDescendants(f);
                    if (objs != null) foreach (ITxObject o in objs) if (o is ITxRoboticLocationOperation l) list.Add(l);
                }
                catch { }
            }
            if (list.Count == 0 && operation is ITxRoboticLocationOperation self) list.Add(self);
            return list;
        }

        /// <summary>按名称在场景里查找并设为当前选中(替换)。打通"查到 -> 选中 -> 操作"。</summary>
        public static string SelectObjects(IList<string> names)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    if (names == null || names.Count == 0) return "未提供要选中的名称。";

                    var all = CollectScene(true);
                    var map = new Dictionary<string, ITxObject>(StringComparer.Ordinal);
                    foreach (var o in all) { var n = SafeName(o); if (n != null && !map.ContainsKey(n)) map[n] = o; }

                    var list = new TxObjectList();
                    var found = new List<string>();
                    var missing = new List<string>();
                    foreach (var nm in names)
                    {
                        ITxObject o;
                        if (map.TryGetValue(nm, out o)) { list.Add(o); found.Add(nm); }
                        else
                        {
                            var c = all.FirstOrDefault(x =>
                            {
                                var n = SafeName(x);
                                return n != null && n.IndexOf(nm, StringComparison.OrdinalIgnoreCase) >= 0;
                            });
                            if (c != null) { list.Add(c); found.Add(SafeName(c)); }
                            else missing.Add(nm);
                        }
                    }

                    if (found.Count == 0)
                        return "没有匹配到任何对象。未找到: " + string.Join(", ", missing);

                    try { var sel = TxApplication.ActiveSelection; sel.Clear(); sel.AddItems(list); }
                    catch
                    {
                        try { TxApplication.ActiveSelection.SetItems(list); }
                        catch (Exception ex) { return "设置选中失败: " + ex.Message; }
                    }

                    var msg = "已选中 " + found.Count + " 个对象: " + string.Join(", ", found.Take(20));
                    if (missing.Count > 0) msg += "；未找到: " + string.Join(", ", missing);
                    return msg;
                }
                catch (Exception ex) { return "选中对象失败: " + ex.Message; }
            });
        }

        /// <summary>遍历场景，把匹配 typeKeyword 的对象清单(名称/类型/父级)一步导出为 xlsx。真实数据流。</summary>
        public static string ExportObjectList(string typeKeyword, string folder)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var all = CollectScene(true);
                    if (all.Count == 0) return "未能遍历到场景对象。";

                    var key = (typeKeyword ?? "").Trim();
                    bool wantRobot = key.IndexOf("robot", StringComparison.OrdinalIgnoreCase) >= 0
                                     || key.Contains("机器人");
                    var matched = new List<ITxObject>();
                    foreach (var o in all)
                    {
                        var tn = o.GetType().Name;
                        bool hit = key.Length == 0 || tn.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0
                                   || (wantRobot && o is TxRobot);
                        if (hit) matched.Add(o);
                    }
                    if (matched.Count == 0) return "没有匹配 \"" + key + "\" 的对象，未导出。";

                    var headers = new List<string> { "#", "名称", "类型", "父级" };
                    var rows = new List<IList<string>>();
                    int i = 0;
                    foreach (var o in matched)
                    {
                        i++;
                        rows.Add(new List<string> { i.ToString(), SafeName(o), o.GetType().Name, ParentName(o) });
                    }

                    var fld = string.IsNullOrWhiteSpace(folder)
                        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TxAgentExport")
                        : folder;
                    var tag = SanitizeTag(key.Length > 0 ? key : "objects");
                    var fname = tag + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
                    var path = XlsxWriter.Write(Path.Combine(fld, fname), tag, headers, rows);
                    return "已导出 " + matched.Count + " 个对象到: " + path;
                }
                catch (Exception ex) { return "导出对象清单失败: " + ex.Message; }
            });
        }

        /// <summary>把当前选中操作的焊点/路径点坐标导出为 Excel(复用你的 ExcelExporter，含参考系转换)。</summary>
        public static string ExportPointsExcel(string pointType, bool useMfg, string folder)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    var ops = PsReader.GetOperationsFromSelection(Nolog);
                    if (ops == null || ops.Count == 0) return "当前选择里没有可识别的操作。";

                    var pt = ParsePointType(pointType);
                    int total = 0;
                    foreach (var op in ops)
                    {
                        op.Points.Clear();
                        PsReader.FillPoints(op, pt, useMfg, Nolog);
                        total += op.Points.Count;
                    }
                    if (total == 0) return "未找到符合条件的点，未导出。";

                    double[] refMatrix = null;
                    try { var rf = PsReader.GetReferenceFrame(); if (rf != null) refMatrix = rf.Item2; }
                    catch { }

                    var path = ExcelExporter.Export(ops, refMatrix, folder, Nolog);
                    return path != null
                        ? "已导出 " + total + " 个点到: " + path
                        : "导出失败(无数据或写入错误)。";
                }
                catch (Exception ex) { return "导出焊点 Excel 失败: " + ex.Message; }
            });
        }

        // ───────── 变更操作 (待接入) ─────────

        /// <summary>把当前选中设备最低点对齐到世界 Z=0 (变更，可 Ctrl+Z 撤销)。</summary>
        public static string AlignSelectedDevicesToFloor()
        {
            return PsContext.Current.Run<string>(delegate
            {
                try { return DeviceZAlignService.AlignSelection(); }
                catch (Exception ex) { return "对齐失败: " + ex.Message; }
            });
        }

        // ───────── API 探查 / 动态代码 ─────────

        /// <summary>探查一个活动对象(按 name 或当前选中第一个)的运行时类型与成员取值。</summary>
        public static string InspectObject(string name)
        {
            return PsContext.Current.Run<string>(delegate
            {
                try
                {
                    ITxObject target = !string.IsNullOrWhiteSpace(name) ? FindByName(name) : FirstSelected();
                    if (target == null)
                        return string.IsNullOrWhiteSpace(name) ? "请先选中一个对象，或提供 name。" : ("未找到 " + name);
                    return ApiInspector.InspectObjectLive(target);
                }
                catch (Exception ex) { return "探查对象失败: " + ex.Message; }
            });
        }

        /// <summary>编译(后台)+执行(主线程, 包 Undo)用户 C# 代码。调用方负责审批与审计。</summary>
        public static string RunCSharp(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "未提供代码。";

            // 1) 编译：纯 CPU，不碰 PS —— 在调用线程(后台)进行，不冻结 UI。
            string compileError;
            var assembly = CSharpRunner.Compile(code, out compileError);
            if (assembly == null) return compileError;

            // 2) 执行：碰 PS，必须主线程，包在 Undo 块里(可撤销)。
            return PsContext.Current.Run<string>(delegate
            {
                var log = new StringBuilder();
                Action<string> logfn = delegate (string s) { if (s != null) log.AppendLine(s); };

                TxDocument doc = null;
                try { doc = TxApplication.ActiveDocument; } catch { }
                bool undo = doc != null && BeginUndo(doc, "run_csharp");

                string result;
                try { result = CSharpRunner.Invoke(assembly, logfn); }
                catch (Exception ex) { result = "执行异常: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }
                finally { if (undo) EndUndo(doc); }

                try { TxApplication.RefreshDisplay(); } catch { }

                var sb = new StringBuilder();
                if (log.Length > 0) sb.Append("日志:\n").Append(log.ToString());
                sb.Append("结果: ").Append(result);
                return sb.ToString();
            });
        }

        private static bool BeginUndo(TxDocument doc, string desc)
        {
            try { dynamic d = doc; dynamic ur = d.UndoRedo; if (ur != null) { ur.BeginCommand(desc); return true; } } catch { }
            try { dynamic d = doc; dynamic ctx = d.UndoContext; if (ctx != null) { ctx.Open(desc); return true; } } catch { }
            try { dynamic d = doc; dynamic um = d.UndoManager; if (um != null) { um.BeginUndoStep(desc); return true; } } catch { }
            return false;
        }

        private static void EndUndo(TxDocument doc)
        {
            try { dynamic d = doc; d.UndoRedo.EndCommand(); return; } catch { }
            try { dynamic d = doc; d.UndoContext.Close(); return; } catch { }
            try { dynamic d = doc; d.UndoManager.EndUndoStep(); return; } catch { }
        }

        // ───────── 内部：遍历辅助 (均在 PsContext.Run 内被调用，不再二次路由) ─────────

        private static List<object> SceneRoots()
        {
            var roots = new List<object>();
            try
            {
                dynamic dDoc = TxApplication.ActiveDocument;
                if (dDoc == null) return roots;
                foreach (var rn in new[] { "PhysicalRoot", "ComponentRoot", "ResourceRoot" })
                {
                    object root = null;
                    try
                    {
                        switch (rn)
                        {
                            case "PhysicalRoot": root = dDoc.PhysicalRoot; break;
                            case "ComponentRoot": root = dDoc.ComponentRoot; break;
                            case "ResourceRoot": root = dDoc.ResourceRoot; break;
                        }
                    }
                    catch { }
                    if (root != null) roots.Add(root);
                }
            }
            catch { }
            return roots;
        }

        private static TxObjectList DescendantsOf(object node, bool recursive)
        {
            var f = new TxTypeFilter(typeof(ITxObject));
            try
            {
                dynamic d = node;
                var r = (recursive ? d.GetAllDescendants(f) : d.GetDirectDescendants(f)) as TxObjectList;
                if (r != null) return r;
            }
            catch { }
            try
            {
                dynamic d = node;
                var r = (recursive ? d.GetAllDescendants() : d.GetDirectDescendants()) as TxObjectList;
                if (r != null) return r;
            }
            catch { }
            return null;
        }

        private static List<ITxObject> CollectScene(bool recursive)
        {
            var list = new List<ITxObject>();
            var seen = new HashSet<int>();
            foreach (var root in SceneRoots())
            {
                var kids = DescendantsOf(root, recursive);
                if (kids == null) continue;
                foreach (ITxObject o in kids)
                {
                    if (o == null) continue;
                    if (seen.Add(RuntimeHelpers.GetHashCode(o))) list.Add(o);
                }
            }
            return list;
        }

        private static Dictionary<string, int> Histogram(List<ITxObject> objs)
        {
            var hist = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var o in objs)
            {
                var tn = o.GetType().Name;
                int c; hist[tn] = hist.TryGetValue(tn, out c) ? c + 1 : 1;
            }
            return hist;
        }

        private static ITxObject FindByName(string name)
        {
            ITxObject contains = null;
            foreach (var o in CollectScene(true))
            {
                var n = SafeName(o);
                if (string.Equals(n, name, StringComparison.Ordinal)) return o;
                if (contains == null && n != null
                    && n.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0) contains = o;
            }
            return contains;
        }

        private static ITxObject FirstSelected()
        {
            try
            {
                dynamic sel = TxApplication.ActiveSelection;
                dynamic items = sel.GetItems();
                var en = items as IEnumerable;
                if (en != null) foreach (var o in en) return o as ITxObject;
            }
            catch { }
            return null;
        }

        private static PointType ParsePointType(string s)
        {
            if (string.Equals(s, "WeldPoint", StringComparison.OrdinalIgnoreCase)) return PointType.WeldPoint;
            if (string.Equals(s, "PathPoint", StringComparison.OrdinalIgnoreCase)) return PointType.PathPoint;
            if (string.Equals(s, "ContinuousPoint", StringComparison.OrdinalIgnoreCase)) return PointType.ContinuousPoint;
            return PointType.All;
        }

        private static string ParentName(object o)
        {
            try { dynamic d = o; dynamic p = d.Parent; if (p != null) return (string)p.Name ?? ""; } catch { }
            return "";
        }

        private static string SanitizeTag(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
                sb.Append(char.IsLetterOrDigit(c) ? c : '_');
            var r = sb.ToString().Trim('_');
            return r.Length == 0 ? "objects" : r;
        }

        private static string SafeName(object o)
        {
            try
            {
                dynamic d = o;
                string n = (string)d.Name;
                return string.IsNullOrEmpty(n) ? o.ToString() : n;
            }
            catch { return o == null ? "<null>" : o.ToString(); }
        }
    }
}