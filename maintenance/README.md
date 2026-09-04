# Maintenance and regression checks

This directory contains source tools, not runtime conversation or memory data.

- `StorageRegression.cs`: 24 offline checks for reasoning serialization, provider-specific request fields, archive compatibility/recovery, cancellation and tool-result persistence. Compile against the built TxTools assembly and Newtonsoft.Json; run only in an isolated build directory because fixtures are written beside the assembly.
- `Optimize-DeployedMemory.ps1`: dry-run by default. Reviews scene-specific facts, unverified automatically promoted snippets and selected overlapping recipes. `-Apply` requires Process Simulate to be closed, snapshots each selected file, verifies SHA-256 before removal from active memory, and preserves a recovery manifest. It does not delete conversations, knowledge documents or the legacy recipe catalog.

The cleanup script targets the original deployment path explicitly and must be reviewed before adapting it to another installation. It sets `ReasoningEffort` to `low` without changing model selection or approval mode. Recovery consists of restoring archived files by manifest, with Process Simulate closed; compare any newer same-name files before restoring.

Official DeepSeek V4 uses `reasoning_effort=low` by default; `high` and `max` remain configuration overrides. Other providers retain their API defaults. Reasoning returned by the provider is persisted for history; old reasoning that was never recorded cannot be reconstructed. See [DeepSeek thinking mode documentation](https://api-docs.deepseek.com/guides/thinking_mode/).

This is the Agent source subtree of TxTools, not a standalone redistribution of the proprietary PS SDK or the surrounding TxTools modules. Build and integration require those existing dependencies.
