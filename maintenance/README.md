# 维护、归档清理与回归测试

更新：2026-09-04；对应源码提交 `d7433b9`。本目录只存源代码与操作说明，不包含运行记忆、对话、密钥或本机清理报告。

## 已完成与未验证

- 官方 DeepSeek V4 默认低强度思考，工具查询/对比/读回复核优先。
- 思考内容独立归档；紧凑 JSON、原子替换、一个旧快照备份、元数据列表读取。
- 修复工具完成事件早于结果入库，以及上下文裁剪导致归档索引错位的问题。
- 默认记忆注入最多 6 条偏好/API事实 + 5 条有正解的避坑；自动片段晋升默认关闭。
- 完整 TxTools 宿主隔离编译通过；24 项离线回归和 HTML 内联 JavaScript 语法检查通过。
- 已在维护环境应用清理并部署。清理数量属于那次本机快照，不是项目的通用默认规模。
- 未做此次版本的付费模型联调或真实 PS/CATIA 场景变更回归。低强度并不保证固定响应时长。

## Optimize-DeployedMemory.ps1

默认只预览，不改数据：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\maintenance\Optimize-DeployedMemory.ps1
```

脚本明确锁定原部署路径。虽然暴露 `-Root` 参数，内部还会检查固定的允许路径；用于其他安装前必须同时审查路径校验和清理规则，不能只传一个新目录。

当前规则：

| 类别 | 规则 |
|---|---|
| facts | 归档 `scene_constant`，避免把旧工程快照当作全局事实；完全相同的规范化内容保留一份 |
| snippets | 归档 `origin=auto-promoted` 且 `success_count=0` 的候选，不代表永久判定其代码无效 |
| pending | 根据 `last_seen` 归档超过 30 天的条目 |
| recipes | 仅在综合颜色导出配方存在时，归档两个指定、无运行记录的重叠配方；保留输出结构不同的按关节拆分配方 |

所有对话、知识文档、gotchas 和旧 `recipes.json` 保留。没有用模糊相似度批量合并 API 经验；名称相近的对象或参考系可能语义不同。

应用前保存工程并关闭所有 Process Simulate（Tune）进程，审核预览后再运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\maintenance\Optimize-DeployedMemory.ps1 -Apply
```

该操作需要部署目录写权限。它先记录清单、复制原文件并核对 SHA-256，再逐个移出活动目录；副本保留在 `maintenance-backup/<UTC时间>/`。同时备份 `prefs.json/recipes.json`，将 `ReasoningEffort` 设为 `low`，保留原模型、provider 与审批模式。

这不是跨文件事务。中途失败应按 manifest 核查备份与原文件后恢复/续做，不能假设所有条目都已处理。不要让 PS 在应用过程中重新启动。

### 恢复

关闭 PS，按 `manifest.json` 的相对路径将备份复制回活动目录，并核对 SHA-256。若原位置已经出现新文件，先比较再决定，不覆盖新的有效内容。偏好文件有单独原始备份。

清理脚本不替换 DLL，也不执行场景配方。DLL/PDB 部署须单独备份和验证；UI 是内嵌资源，修改后需要重新构建宿主。

## StorageRegression.cs

在已构建的隔离目录中编译测试程序，引用同目录的 `TxTools.dll` 与 `Newtonsoft.Json.dll`。示例需在包含编译器的开发者命令行中运行：

```text
csc /nologo /target:exe /out:artifacts/review-build/StorageRegression.exe /reference:artifacts/review-build/TxTools.dll /reference:artifacts/review-build/Newtonsoft.Json.dll maintenance/StorageRegression.cs
artifacts\review-build\StorageRegression.exe
```

这不是本仓库独立构建命令：先在完整 TxTools 工程中产生 DLL，并确保其依赖可解析。

**不要在正式部署目录运行测试**。测试会在被测程序集旁创建 `conversations/storage_regression.json` 等夹具，并故意写入损坏 JSON 验证备份恢复。它不发起真实模型请求、不调用 PS 场景工具，但会写测试文件。

24 项断言覆盖：

- 普通请求排除思考，存储保留并可 round-trip；旧格式兼容。
- 官方 V4 的 low/max/非法配置处理、工具请求回传、代理隔离及无工具请求。
- 上下文预算计算包含思考内容。
- 磁盘读写、原子备份、元数据列表及主文件损坏回退。
- 正常、取消、失败、仅思考响应保存，避免空 assistant 消息。
- 工具完成事件触发时，session 已有匹配的工具结果。

测试不覆盖真实 COM 互操作、工程回滚、模型行为质量、所有 provider 或硬崩溃中最后几个流式片段。

## 协议与数据边界

`prefs.json` 可设置 `ReasoningEffort: low/high/max`，构造循环时加载；只对官方 DeepSeek V4 发送该控制字段，其他 provider 保持默认。官方带工具请求的思考回传依据见 [DeepSeek Thinking Mode](https://api-docs.deepseek.com/guides/thinking_mode/)。

思考仅记录模型实际返回内容，旧版本没有保存的内容不能恢复。归档是过程记录，不自动视为正确经验。快照保存并非逐 token WAL，已作废的重试缓冲也不会作为最终回答保留。

备份与对话同样可能包含敏感工程信息，应按本地数据管理，不上传到源码仓库。
