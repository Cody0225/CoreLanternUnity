# Agents / Skills / Commands 棚卸し・スコアリング監査

作成日: 2026-06-03
対象: CoreLanternUnity (= Eggcore Protocol / "Lantern Core Defense")
ブランチ: `claude/agents-skills-commands-audit-jc1kU`

---

## 0. サマリ（最重要結論）

- **このリポジトリには Claude Code 用のカスタム定義が 1 つも無い。**
  `.claude/agents/` `.claude/skills/` `.claude/commands/` `.claude/settings.json` いずれも未作成。
- 現状動いているのは 3 層:
  1. **ビルトイン・サブエージェント**（`Agent` ツールの `subagent_type`）
  2. **グローバル/ビルトイン Skill**（session reminder に列挙された 14 個）
  3. **`Tools/` 配下のスクリプト群**（PowerShell / Python = 事実上の「コマンド」）
- CLAUDE.md には極めて厳格な運用ルール（mojibake 防止・`CoreLanternGame.cs` 編集規律・`CheckCompileHealth` 必須・Codex/Claude 分業・リリース手順）があるのに、**それらが Claude Code の hook / command / skill として全く自動化・配線されていない**。= 最大の「不足」。
- 環境注意: **この実行環境は Linux リモートコンテナ**。プロジェクトの中核ツール（PowerShell 5.1 / Unity 6000.4.7f1 = `C:\Program Files\Unity`）は **Windows 専用**。よって「アプリを起動して検証する」系 Skill はこの環境では実行不能。

スコア凡例: 5=必須 / 4=高 / 3=中 / 2=低 / 1=ほぼ不要 / 0=無関係

---

## 1. Agents（ビルトイン・サブエージェント）棚卸し

| Agent | 役割 | スコア | 評価 |
|---|---|---|---|
| **Explore** | read-only の広域検索 | **5** | 16,000 行の `CoreLanternGame.cs` + 60+ docs を抱える本プロジェクトに最適。位置特定を委譲してトークン節約。最重要。 |
| **general-purpose** | 多段タスク/検索 | **4** | 複合調査・横断検索の主力。 |
| **Plan** | 設計プラン作成 | **3** | cs ファイル分割・大型リファクタ前の設計に有効。 |
| **claude**（catch-all） | 汎用 | **2** | 既定。特化エージェントが無いとき。 |
| **claude-code-guide** | Claude Code 自体の質問 | **2** | メタ用途。プロジェクト直接の価値は低いが hook/command 整備時に有用。 |
| **statusline-setup** | ステータスライン設定 | **1** | 装飾。プロジェクト価値ほぼ無し。 |

**重複/重なり**: `Explore` / `general-purpose` / `claude`(検索委譲) は探索範囲が重複。使い分け（結論だけ欲しい→Explore、実装含む→general-purpose）を明文化すべき。

---

## 2. Skills 棚卸し（session reminder の 14 個）

| Skill | 用途 | スコア | 評価 / 本プロジェクトでの位置づけ |
|---|---|---|---|
| **code-review** | diff のバグ/簡素化レビュー | **4** | cs 変更の品質確認に直結。`/code-review` 主力。 |
| **session-start-hook** | SessionStart hook 整備 | **4** | 「HANDOFF_FOR_CODEX 読了」「EDIT_LOCK 確認」「CheckCompileHealth 案内」を自動表示できる。**未活用の宝**。 |
| **update-config** | settings.json/hook/permission 設定 | **3** | hook・permission・PowerShell allowlist を実際に配線する手段。下記「不足」の実装に必須。 |
| **fewer-permission-prompts** | read-only Bash/MCP の allowlist 生成 | **3** | 反復する PowerShell スキャンの許可プロンプト削減に有効。 |
| **simplify** | 変更コードの整理 | **2** | 有用だが巨大単一ファイルでは適用範囲を絞る必要。リスク管理前提。 |
| **review** | PR レビュー | **2** | `code-review` と用途重複（**重複**）。PR 単位 vs diff 単位の差のみ。 |
| **loop** | 定期実行/ポーリング | **2** | PR babysit 等に転用可。日常価値は限定的。 |
| **deep-research** | 多源 Web リサーチ | **1** | オフラインのゲーム開発では出番少。市場/競合調査の単発のみ。 |
| **verify** | アプリ起動して挙動確認 | **1** | **この Linux 環境では Unity 起動不能**＝実行不可。Windows 実機専用。 |
| **run** | アプリ起動/スクショ | **1** | 同上。`verify` と用途重複（**重複**）。 |
| **init** | CLAUDE.md 生成 | **1** | 既に精緻な CLAUDE.md が存在。再生成は上書き事故リスク＝むしろ有害。 |
| **security-review** | 差分のセキュリティレビュー | **1** | 単機オフラインゲームでは優先度低（安価なので保険程度）。 |
| **keybindings-help** | キーバインド編集 | **0** | プロジェクト価値なし。 |
| **claude-api** | Anthropic SDK 開発 | **0** | Unity ゲームに無関係。完全に対象外。 |

**Skill の重複**:
- `code-review` ≒ `review` ≒ `security-review`（+ GitHub MCP `request_copilot_review`）… 「レビュー」系が4系統。
- `verify` ≒ `run`… どちらも「アプリ起動して観察」。しかも当環境では両方実行不能。

---

## 3. Commands 棚卸し（`Tools/` = 事実上のコマンド層）

> 注: 正式な `.claude/commands/*.md` スラッシュコマンドは **0 個**。以下は PowerShell/Python スクリプトとして存在する「手動コマンド」。

| スクリプト | 用途 | スコア | 評価 |
|---|---|---|---|
| **CheckCompileHealth.ps1** | cs 編集後の必須検証（odd-quote/brace/swallowed/mojibake/Roslyn） | **5** | CLAUDE.md B/C を 1 コマンド化。**最重要。スラッシュコマンド化すべき筆頭**。 |
| **ValidateCodexAssets.ps1** | Codex 生成素材の規格チェック | **4** | 素材検収の中核。`/validate-assets` 化候補。 |
| **GenerateBuildPackage.ps1** | ビルド zip 化+ハッシュ | **4** | リリース工程の中核。 |
| **UploadToItch.ps1** | itch.io アップロード | **4** | リリース後工程。 |
| **ReportAssetStatus.ps1 / TestImageResources / TestAudioResources** | 素材ステータス点検 | **3** | 日常点検。 |
| `Generate*Assets.ps1`（30+ 本） | 個別素材生成 | **2** | 一発生成系。多くは生成済みで再利用頻度低。アーカイブ候補多数。 |
| `redraw_* / process_* / build_s1_*`（py 個別） | 特定キャラ素材の一回限り処理 | **1** | 使い捨て。`Tools/Deprecated/` へ寄せるべき。 |
| Tools/Deprecated/...BAD_20260601.ps1 | 既に廃止済み | — | 整理済み（良い前例）。 |

**重複/肥大**: `Tools/` は 50 本近く。大半が「特定の素材を一度生成しただけ」のワンショット。`CheckCompileHealth` 等の常用 5〜6 本と、ワンショット生成系（アーカイブ対象）が混在＝可読性低下。

---

## 4. 抽出: 重複 / 未使用 / 保守コスト高 / 本作に不要

> 方針: **削除は行わない**。以下は「最適化のための分類」であり、対処は §5〜§7 の **追加・統合・改善提案のみ**。既存構成との互換性を最優先する。

### 4-A. 本作（Lantern Core Defense / Eggcore Protocol）に不要

| 区分 | 項目 | 理由 | 提案 |
|---|---|---|---|
| Skill | **claude-api** | Unity ゲームに無関係（0） | 運用文書で「非対象」明記のみ。削除不要（グローバル提供で物理削除不可） |
| Skill | **keybindings-help** | プロジェクト価値なし（0） | 同上 |
| Skill | **init** | 精緻な CLAUDE.md が既存。再生成は上書き事故リスク | 「使用禁止」を CLAUDE.md に 1 行明記 |

### 4-B. 未使用 / この環境で実行不能

| 区分 | 項目 | 理由 | 提案 |
|---|---|---|---|
| Skill | **verify / run** | 当 Linux リモートでは Unity(Windows専用) 起動不能＝事実上未使用 | 「Windows 実機専用」と注記。当環境では呼ばない運用に |
| Skill | **deep-research** | オフライン開発で出番ほぼ無し | 市場/競合調査の単発時のみ、と用途限定を明記 |
| Commands | `Tools/` ワンショット py（`redraw_*` `process_s1_*` `build_s1_*` `replace_s1_*` `extract_*` `generate_s1_*`） | 一回限りの素材処理。再実行されていない | **削除せず** README で「ワンショット（実行済み）」とタグ付け。任意で `Tools/_archive/` への移動は将来提案（今は非実施） |

### 4-C. 保守コストが高いもの

| 区分 | 項目 | コスト要因 | 提案 |
|---|---|---|---|
| Commands | `Tools/` 50 本近い PowerShell/py | 常用 5〜6 本と使い捨て 40+ 本が混在し可読性低下。Windows/PowerShell 依存で当環境から検証不可 | README に「🟢常用 / ⚪ワンショット」の二分表を追加（移動はしない＝パス互換維持） |
| Skill | **simplify** | 16,000 行単一ファイルに広域適用すると差分が巨大化・レビュー困難・mojibake 誘発リスク | 適用範囲を「直近編集メソッドのみ」に限定するルールを明記 |
| ルール | CLAUDE.md の検証フロー（B/C 手動 PowerShell） | 毎回コピペ実行で手間。漏れると事故 | `/check-health` コマンド化で保守コスト恒久削減（§6-1） |

### 4-D. 重複 → §5 で詳述

---

## 5. 重複（統合/使い分け明文化）

| 重複グループ | 含まれるもの | 対処 |
|---|---|---|
| レビュー系 | `code-review` / `review` / `security-review` / GitHub MCP `request_copilot_review` | `code-review` を主軸に。`review`=PR 単位、`security-review`=任意の保険、と役割を 1 行で明文化。 |
| アプリ起動系 | `verify` / `run` | どちらも当環境で不可。Windows 実機運用ルールへ集約。 |
| 探索系 | `Explore` / `general-purpose` / `claude`(検索) | 「結論だけ→Explore / 実装含む→general-purpose」と CLAUDE.md に追記。 |
| 素材生成スクリプト | `Tools/Generate*Assets.ps1` 多数 | カテゴリ別に統合 or アーカイブ。 |

---

## 6. 不足（新規作成を強く推奨）★最重要セクション

CLAUDE.md の厳格ルールが「人手の口約束」で止まっており、Claude Code の自動化機構（hook/command/skill/settings）に **全く配線されていない**。以下を作るべき。

### 6-1. スラッシュコマンド `.claude/commands/*.md`（現状 0 個）

| 新規コマンド | 中身 | 優先度 |
|---|---|---|
| **`/check-health`** | `CheckCompileHealth.ps1` を実行し `Report for handoff:` を貼る | ★★★ 必須 |
| **`/validate-assets`** | `ValidateCodexAssets.ps1`（カテゴリ引数） | ★★ |
| **`/release`** | `GenerateBuildPackage.ps1` → `UploadToItch.ps1` の連結 | ★★ |
| **`/codex-handoff`** | `HANDOFF_FOR_CODEX.md` の要点 + `REMAINING_TASKS.md` 追記テンプレ | ★ |

### 6-2. Hooks（`.claude/settings.json` 経由、現状 0 個）

| 新規 hook | 目的 | 優先度 |
|---|---|---|
| **PreToolUse（Edit/Write が `CoreLanternGame.cs`）** | PowerShell 書き戻し禁止・Edit/Write 限定・EDIT_LOCK 確認を強制 | ★★★ mojibake 事故再発防止 |
| **PostToolUse（同上）** | 自動で brace/odd-quote/swallowed 検査を走らせ警告 | ★★★ |
| **SessionStart** | 「HANDOFF_FOR_CODEX 読了」「cs は Claude 専任」「CheckCompileHealth 必須」をバナー表示 | ★★ |

→ 実装手段は **`session-start-hook`** および **`update-config`** Skill が担える（だから上で高スコア）。

### 6-3. カスタム・サブエージェント `.claude/agents/*.md`（現状 0 個）

| 新規エージェント | 役割 | 優先度 |
|---|---|---|
| **`cs-guardian`** | `CoreLanternGame.cs` 編集の門番。Read 再確認→Edit→CheckCompileHealth まで一貫実行。PowerShell 書き戻し厳禁を内蔵 | ★★ |
| **`asset-codex-liaison`** | Claude/Codex 分業境界をエンコード。素材を「繋ぐ」工程の定型化 | ★ |

### 6-4. Permissions（`.claude/settings.json`、現状 0 個）

- read-only な PowerShell スキャン（`CheckCompileHealth -Quick`、`ReportAssetStatus` 等）を **allowlist** 化し許可プロンプトを削減。
- 手段: **`fewer-permission-prompts`** Skill。

---

## 7. アクション提案（優先順 / すべて「追加・統合・改善」のみ・削除なし）

> 互換性最優先: 既存ファイルのパス・スクリプト名・CLAUDE.md ルールは変更しない。
> 提案はすべて **新規追加（`.claude/` 配下）** か **文書への追記** のみで、既存挙動を壊さない。

1. **`.claude/settings.json` + cs 用 PreToolUse/PostToolUse hook を新設**（mojibake 事故の構造的再発防止／最優先）。新規ファイルのため既存に非干渉。
2. **`/check-health` スラッシュコマンド追加**（`CheckCompileHealth.ps1` をラップするだけ。スクリプト本体は不変）。
3. **SessionStart hook 追加**で HANDOFF/分業/検証ルールを毎回明示。
4. **`Tools/README.md` に「🟢常用 / ⚪ワンショット」二分表を追記**（ファイル移動はしない＝呼び出しパス互換を維持）。
5. **CLAUDE.md に「エージェント/Skill 使い分け」セクションを追記**（Explore vs general-purpose、レビュー系の役割、verify/run は Windows 専用、claude-api/keybindings-help/init は非対象）。
6. いずれも段階導入可能。まず §6-1 の `/check-health` と §6-2 の SessionStart から着手すると低リスク。

---

## 8. 実装済み (2026-06-03 / Codex 連携非干渉を確認の上)

Codex は `HANDOFF_FOR_CODEX.md`(markdown)で駆動し `.claude/` を参照しないため、以下の追加は
Codex ワークフローと完全に直交する。**自動実行する hook は使わず、純マークダウン定義のみ**で
着手(何も自動実行しない=既存挙動ゼロ干渉)。既存ファイル・スクリプト・ルールは未変更。

| 追加ファイル | 種別 | 内容 | 既存への影響 |
|---|---|---|---|
| `.claude/commands/check-health.md` | スラッシュコマンド | `CheckCompileHealth.ps1` のラッパー (`/check-health`) | なし(新規) |
| `.claude/commands/validate-assets.md` | スラッシュコマンド | `ValidateCodexAssets.ps1` のラッパー (`/validate-assets`) | なし(新規) |
| `.claude/agents/cs-guardian.md` | サブエージェント | cs 安全編集の門番。EDIT_LOCK 確認→Edit/Write 限定→検証を内蔵 | なし(新規) |

未実装(将来オプション・要合意): SessionStart hook / cs 用 PreToolUse hook / Tools README 二分表追記 /
CLAUDE.md 使い分けセクション。hook 系は Windows/Linux クロス互換と「自動実行」の性質上、
導入は段階的・要合意とする。

---

最終更新: 2026-06-03
