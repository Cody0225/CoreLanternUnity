# Tools Directory

CoreLanternUnity プロジェクト用のユーティリティスクリプト集。
すべて PowerShell 5.1 / 7+ 対応。プロジェクトルートから実行する。

---

## 🟢 日常的に使うスクリプト (Claude / ユーザー両方)

### `CheckCompileHealth.ps1` ⭐ 必須
`CoreLanternGame.cs` 編集後に必ず実行する検証フロー (CLAUDE.md セクション B/C をワンコマンド化)。

```powershell
.\Tools\CheckCompileHealth.ps1            # 全検査 (約 30秒、Roslyn 含む)
.\Tools\CheckCompileHealth.ps1 -Quick     # 高速版 (約 1秒)
.\Tools\CheckCompileHealth.ps1 -Verbose   # 詳細出力
```

検査項目: odd-quote / brace balance / swallowed code / mojibake marker warning / Roslyn syntax
完了報告時の使用例: 末尾出力の `Report for handoff:` 1行をそのままコピペ

### `ValidateCodexAssets.ps1` ⭐ Codex 用
Codex が生成した素材の規格適合チェック (サイズ・透過・META)。

```powershell
.\Tools\ValidateCodexAssets.ps1                      # 全カテゴリ
.\Tools\ValidateCodexAssets.ps1 -Category Logo       # ロゴだけ
.\Tools\ValidateCodexAssets.ps1 -Category Steam      # Steam カプセル
.\Tools\ValidateCodexAssets.ps1 -Category Icon       # .exe アイコン
.\Tools\ValidateCodexAssets.ps1 -Category Character  # キャラ画像
.\Tools\ValidateCodexAssets.ps1 -Category Audio      # BGM/SE
.\Tools\ValidateCodexAssets.ps1 -Category Hud        # HUD 9-slice
```

### `GenerateBuildPackage.ps1` ⭐ リリース時
Unity ビルド後の zip 化 + ハッシュ生成 + アップロード前チェック。

```powershell
.\Tools\GenerateBuildPackage.ps1 -DryRun                 # 検証のみ
.\Tools\GenerateBuildPackage.ps1 -Version "0.9.0"        # demo 用
.\Tools\GenerateBuildPackage.ps1 -Version "0.9.0" -Channel "steam-ea"
```

出力: `Builds/_Packages/EggcoreProtocol_{channel}_v{version}_Windows.zip` + `.sha256.txt`

### `UploadToItch.ps1` ⭐ リリース時
`butler` CLI 経由で itch.io へビルドをアップロード。`GenerateBuildPackage.ps1` の次の工程。

```powershell
.\Tools\UploadToItch.ps1 -CheckSetup           # butler インストール・ログイン確認
.\Tools\UploadToItch.ps1 -DryRun               # アップロードせず確認のみ
.\Tools\UploadToItch.ps1                        # 最新 zip を自動検出してアップロード
.\Tools\UploadToItch.ps1 -Version "0.9.0"      # バージョン指定
.\Tools\UploadToItch.ps1 -Channel "steam-ea"   # チャンネル指定
```

前提: `butler login` 認証済み。未インストールの場合はスクリプトがインストール手順を案内。

---

## 🟡 検証スクリプト (Codex 完了確認用)

### `TestImageResources.ps1`
画像リソース台帳の整合性チェック (PNG / .meta / コード参照 / バックログ DONE 記載)。

```powershell
.\Tools\TestImageResources.ps1
```

### `TestAudioResources.ps1`
音声リソース台帳の整合性チェック (WAV / .meta / AudioManifest.json / コード参照)。

```powershell
.\Tools\TestAudioResources.ps1
```

### `ReportAssetStatus.ps1`
生成済み素材が「コード参照済み」か「接続待ち」かをざっくり棚卸しする読み取り専用レポート。

```powershell
.\Tools\ReportAssetStatus.ps1
```

HUD9、Stage 4/5 支援素材、V2 UI素材、任意fallbackの現在値をClaude/Codex間で確認するために使う。

---

## 🟠 素材生成スクリプト (Codex 担当、procedural / placeholder 用)

⚠ これらは **procedural (自社合成)** で placeholder を生成。商用 EA リリース前に商用素材へ差替推奨。

### Audio
| スクリプト | 出力 |
|---|---|
| `GenerateStarterAudio.ps1` | `Assets/Resources/Audio/*.wav` (BGM 4 種 + SE 9 種) |

### Image Assets
| スクリプト | 用途 |
|---|---|
| `GenerateMapAssets.ps1` | マップ・床・背景 |
| `GenerateHudAssets.ps1` | HUD 旧版 |
| `GenerateHudBarV2Assets.ps1` | HUD バー V2 |
| `GenerateMissingVariantAssets.ps1` | 進化バリアント不足分 |
| `GenerateEnemyExternalAssets.ps1` | 敵キャラ外部素材 |
| `GenerateFusionStageAssets.ps1` | クロス進化ステージ素材 |
| `GenerateNewPartnerRelicUiAssets.ps1` | 新相棒・レリック UI |
| `GenerateNonCharacterPolishBatch2.ps1` | 非キャラ素材磨き込み |
| `GeneratePolishedNonCharacterAssets.ps1` | 同上 batch 1 |
| `GenerateStageBossEnemyAssets.ps1` | Stage 別敵・ボス sprite (2026-05-25 採用済) |
| `GenerateStage45SupportAssets.ps1` | Stage 4/5 サムネ・ハザード (2026-05-25 採用済) |

### Review Sheets
| スクリプト | 用途 |
|---|---|
| `BuildAssetReviewSheets.ps1` | 採用判定用のレビューシート生成 (キャラ一覧 PNG 等) |
| `GenerateNonCharacterQueuePreview.ps1` | 非キャラ未接続素材の作業キューを1枚のPNGで可視化 |

---

## 🛠 使用パターン例

### パターン A: Claude が CoreLanternGame.cs を編集した時
```powershell
# 編集後、必ず実行
.\Tools\CheckCompileHealth.ps1

# 結果が All checks PASSED なら完了報告に
# "odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)" と記載
```

### パターン B: Codex が新規素材を生成した時
```powershell
# 生成完了後、自己検証
.\Tools\ValidateCodexAssets.ps1 -Category Character

# Errors: 0 を確認してから完了報告
# REMAINING_TASKS.md の「直近完了」セクションに追記
```

### パターン C: ユーザーがリリース用ビルドを作成する時
```powershell
# 1. Unity Editor で File > Build Settings > Build
#    出力先: Builds/0.9.0/
# 2. zip + ハッシュ生成
.\Tools\GenerateBuildPackage.ps1 -Version "0.9.0" -Channel "demo"
# 3. 出力された zip を itch.io / Steam にアップロード
```

### パターン D: 全リソース整合性チェック
```powershell
.\Tools\TestImageResources.ps1
.\Tools\TestAudioResources.ps1
.\Tools\ValidateCodexAssets.ps1
.\Tools\CheckCompileHealth.ps1 -Quick
```

---

## ⚠ ハード制約 (CLAUDE.md より)

0. **Codex は作業開始前に `HANDOFF_FOR_CODEX.md` を全文読む**
   - 短い再開依頼、コンテキスト圧縮後、Claudeとの並行作業後も例外なし
   - 読めない場合は作業しない

1. **`CoreLanternGame.cs` を Get-Content / Set-Content で書き換えない**
   - 検査・スキャン用 (read-only) は OK
   - 書き戻し系は完全禁止 (UTF-8 文字化けリスク)

2. **編集後は `CheckCompileHealth.ps1` を必ず実行**
   - odd-quote / brace / swallowed code / mojibake marker warning / Roslyn を 1 コマンドで実行

3. **完了報告には検査結果を含める**
   - `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`

詳細: プロジェクト直下の `CLAUDE.md`

---

## 📦 既存スクリプトの今後の運用

### EA リリース時に整理
- `GenerateStarterAudio.ps1` → 商用 BGM/SE に差替後は archive ディレクトリへ
- `Generate*ExternalAssets.ps1` 系 → Codex 採用済の素材を生成した historical script として残置

### 新規追加候補
- `Tools/UploadToItch.ps1` — `butler` (itch.io CLI) を使った自動アップロード
- `Tools/UploadToSteam.ps1` — `steamcmd` ラッパー
- `Tools/RegenerateAllAssets.ps1` — 全 procedural 素材を一括再生成 (キャッシュ削除時)

最終更新: 2026-05-28
