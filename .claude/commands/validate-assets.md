---
description: Codex 生成素材の規格適合チェック (ValidateCodexAssets.ps1 をラップ)
argument-hint: "[Logo|Steam|Icon|Character|Audio|Hud] (省略=全カテゴリ)"
allowed-tools: Bash(pwsh:*), Bash(powershell:*), Read
---

`Tools/ValidateCodexAssets.ps1` のラッパー。素材のサイズ・透過・META 規格を検査する。
**スクリプト本体・既存ルールは一切変更しない。**

## 実行手順

1. **Windows (PowerShell 利用可)**:
   ```
   .\Tools\ValidateCodexAssets.ps1 $ARGUMENTS
   ```
   - 引数なし = 全カテゴリ
   - `-Category Logo|Steam|Icon|Character|Audio|Hud` = 個別
   - ユーザーが `Logo` 等のカテゴリ名のみ渡したら `-Category Logo` に読み替える。

2. **PowerShell が無い環境**: 実行不可。`validation_report.json` が既にあればそれを Read して要約する
   (CLAUDE.md セクション J: Claude は素材PNGを極力開かず、数値レポートで判断)。

## 注意 (CLAUDE.md 準拠)
- 素材 PNG を Read で開くのは原則しない。Codex の数値報告 / `validation_report.json` を優先。
- これは検収補助。視覚チェックはユーザー(Unity)に寄せる。
