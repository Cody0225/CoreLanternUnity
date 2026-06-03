---
description: CoreLanternGame.cs 編集後の必須検証 (CheckCompileHealth.ps1 をラップ)
argument-hint: "[--quick | --verbose]"
allowed-tools: Bash(pwsh:*), Bash(powershell:*), Read
---

CLAUDE.md セクション B/C の必須検証を実行する。`Tools/CheckCompileHealth.ps1` のラッパー。
**スクリプト本体・既存ルールは一切変更しない。**

## 実行手順

1. プラットフォームを確認する。
   - **Windows (PowerShell 利用可)**: 以下を実行。
     ```
     .\Tools\CheckCompileHealth.ps1 $ARGUMENTS
     ```
     引数なし=全検査(約30秒, Roslyn含む) / `-Quick`=高速(約1秒) / `-Verbose`=詳細。
     ※ ユーザー入力 `--quick` は `-Quick` に読み替える。
   - **PowerShell が無い環境 (この Linux リモート等)**: スクリプトは実行できない。
     その旨を明示し、代替として `Tools/CheckCompileHealth.ps1` の検査項目
     (odd-quote / brace balance / swallowed code / mojibake marker / Roslyn) のうち
     Bash で代替可能な brace 数・odd-quote の簡易チェックのみ案内する。実機検証は
     ユーザー(Windows)に依頼する。

2. 出力末尾の `Report for handoff:` 行をそのまま完了報告へ貼る。

3. Exit code 0=OK / 1=エラー。エラー時は該当箇所を Read で特定し修正提案する。

## 注意 (CLAUDE.md 準拠)
- 検証対象 `CoreLanternGame.cs` は **Edit / Write tool のみ**で編集。PowerShell 書き戻し禁止。
- 完了報告には `brace diff=0 / odd-quote=0 / compile errors=0 ✓` 形式で結果を含める。
