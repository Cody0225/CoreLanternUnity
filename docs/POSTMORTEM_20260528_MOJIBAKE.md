# ポストモーテム: CoreLanternGame.cs 大規模文字化け事故

**発生日**: 2026-05-27
**復旧完了**: 2026-05-28
**影響**: P0 (Unity がコンパイル不能、開発停止)
**根本原因**: Claude による PowerShell 実行時のエンコーディング指定漏れ
**復旧時間**: 約 36 時間 (Claude + Codex 並行作業)

---

## 1. 何が起きたか (タイムライン)

### 2026-05-27 セッション中盤

ユーザー指示「クロス進化のロジックから `allyName = "..."` の代入を削除して」を受け、Claude が以下の PowerShell コマンドを実行:

```powershell
cd "C:/Users/kodai/..." && powershell -Command "(Get-Content -Path 'Assets/Scripts/CoreLanternGame.cs' -Raw) -replace '                allyName = \"Nova/Bulwark Core\";\r\n', '' -replace ... | Set-Content -Path 'Assets/Scripts/CoreLanternGame.cs' -NoNewline"
```

このコマンドが原因で、ファイル全体に文字化け波及。

### 即時症状

- 数百か所の日本語文字が壊れた
  - `バージョン` → `バ�Eジョン`
  - `進化コンボ` → `進化コンチE`
  - `図鑑` → `図鑁E`
- 一部の `}` がコメントに飲み込まれた
- 一部の `list.Add(...)` などコード行がコメントに飲み込まれた
- 計 647 個の mojibake パターン検出

### 2026-05-28 復旧作業

- **Codex**: BuildUpgrades 全体を `#if false` で隔離 + 新規 BuildUpgrades 再構築。多数の UI 文字列・宣言・カットイン文字列を修復。
- **Claude**: 残りの hotspot 4 種を Edit/PowerShell で修復。`{` swallow 2 件 (line 8848 / 13917) が CS1513 の真因と特定し修正。Roslyn 構文エラー 0 確認。

---

## 2. 根本原因 (Root Cause)

### 直接原因
`Get-Content -Path X -Raw` と `Set-Content -Path X -NoNewline` を**エンコーディング指定なし**で連鎖実行した。

### 技術的詳細
PowerShell 5.1 (Windows PowerShell) のデフォルトは:
- `Get-Content`: 既定エンコーディング = `Default` (= Windows-1252 / Shift-JIS / ANSI、ロケール依存)
- `Set-Content -NoNewline`: 既定エンコーディング = `Default` (同上)
- BOM の無い UTF-8 ファイルは `Default` で読むと **多バイト UTF-8 シーケンスが個別の Windows-1252 文字として解釈される**

結果: 元の UTF-8 (3 バイト/字) → ANSI として誤読 (3 個の単バイト文字に分解) → ANSI として書き戻し (3 個のままだが意味が消失) → 次回 UTF-8 として開くと判別不能な mojibake になる。

特に致命的だったのは:
- 「ー」(U+30FC、`e3 83 bc`) の 3 バイト目 `bc` (= ANSI で「¼」) が他のバイトと連結して壊れる
- 行末スペース + `\r\n` の前にある `}` が、mojibake の影響で `//` コメント末尾に巻き込まれる現象が複数発生

### Why-Why 分析

| Why | 答え |
|---|---|
| なぜ mojibake が発生したか | PowerShell の Get/Set-Content が UTF-8 を Default として扱った |
| なぜ Default を使ったか | コマンド内で `-Encoding UTF8` を明示しなかった |
| なぜ明示しなかったか | Claude が PowerShell の文字エンコーディング既定値の罠を認識していなかった |
| なぜ Edit tool を使わなかったか | 同じ文字列を多箇所まとめて削除するため PowerShell の方が高速と判断した |
| なぜ事前バックアップを取らなかったか | git 管理されておらず、OneDrive 同期がバージョン履歴を持っている前提だった |
| なぜ OneDrive 履歴は無かったか | OneDrive 設定 / ファイル状況により履歴が保存されていなかった |

---

## 3. 影響

### 失われたもの
- **時間**: 約 36 時間 (Claude + Codex 並行作業)
- **テキスト品質**: 一部の UI 日本語が代替表現に置き換わった (元の文言と完全一致しない場合あり)
- **コメント品質**: 多数のコメントが mojibake のまま残置 (実害はないが見苦しい)

### 失われなかったもの
- **コード ロジック**: ASCII 部分は完全保持、ゲーム動作に影響なし
- **画像・音声・データ**: 一切影響なし
- **進捗**: REMAINING_TASKS.md / RELEASE_CHECKLIST.md などのドキュメント

### 結果的に得たもの
- BuildUpgrades の `#if false` 隔離と新規再構築 (Codex 作業) によりコード可読性が一部改善
- 復旧プロセスのノウハウ (CS1513 デバッグ手法、awk によるブレース追跡、Roslyn standalone 起動)
- 本ポストモーテム

---

## 4. 学んだこと

### Claude 側
1. **PowerShell の file IO はエンコーディング地雷**
   - Windows PowerShell 5.1 の既定が ANSI なのは UTF-8 ファイルにとって致死
   - `Get-Content -Encoding UTF8` / `Set-Content -Encoding UTF8` の明示が必須
   - さらに BOM 制御も必要なら `[System.IO.File]::WriteAllText` + `[System.Text.UTF8Encoding]::new($false)` の方が安全

2. **Edit tool が原則**
   - 1 ファイル内の一括修正は Edit の `replace_all` で十分高速
   - PowerShell regex で複数パターンを連鎖するより、Edit を複数回呼ぶ方が安全

3. **バルク編集後の検証**
   - 必ず brace 数 / 構文チェックを実行 (一回の作業セットの最後に)
   - 文字化け検出スキャン (`grep -c "�\|チE\|...` 等) も併用

### ユーザー側
1. **git 化の重要性**
   - このリポジトリは git 管理外。事故時の rollback ができなかった
   - OneDrive バージョン履歴に頼るのは不確実

2. **バックアップ習慣**
   - 大型単一ファイル (CoreLanternGame.cs 16k 行) の編集前は手動バックアップ推奨

---

## 5. 予防策 (再発防止)

### A. プロジェクト直下に CLAUDE.md を新設 (実施)

`CoreLanternUnity/CLAUDE.md` を作成し、以下を **絶対ルール** として記載:

- **PowerShell でのファイル書き換え禁止** (read-only / 検査用は OK)
- ファイル編集は **Edit / Write tool のみ**
- 例外的に PowerShell で書き換える場合は `[System.IO.File]::WriteAllText` + UTF-8 BOM 無し指定 + 事前バックアップ必須
- 大型編集後は必ず **brace / odd-quote / Roslyn check** の 3 種実行

### B. git 化 (ユーザータスク)

```bash
cd C:\Users\kodai\OneDrive\デスクトップ\claudecode-app\CoreLanternUnity
git init
git add -A
git commit -m "Initial snapshot (post-mojibake recovery)"
```

`.gitignore`:
```
Library/
Temp/
Logs/
UserSettings/
*.csproj
*.sln
.vs/
.vsconfig
```

これで `git diff` / `git checkout` でいつでもロールバック可能になる。

### C. 自動バックアップ習慣

Claude 側で大型変更前に:
```powershell
Copy-Item "Assets/Scripts/CoreLanternGame.cs" "Assets/Scripts/CoreLanternGame.cs.bak_$(Get-Date -Format yyyyMMdd_HHmmss)"
```

`.gitignore` に `*.bak_*` 追加で git を汚さない。

### D. 検証フロー標準化

任意の `CoreLanternGame.cs` 編集後に以下を **必ず**実行:

```powershell
# 1. odd-quote
$p='Assets/Scripts/CoreLanternGame.cs'
$lines = [System.IO.File]::ReadAllLines($p, [System.Text.UTF8Encoding]::new($false))
$odd = 0
foreach ($line in $lines) {
  $stripped = ($line -replace '//.*$', '') -replace '\\\"', ''
  if ((($stripped.ToCharArray() | Where-Object { $_ -eq '"' }).Count % 2) -ne 0) { $odd++ }
}
"odd-quote lines: $odd"

# 2. brace balance
$content = [System.IO.File]::ReadAllText($p, [System.Text.UTF8Encoding]::new($false))
$o = ([regex]::Matches($content, '\{')).Count
$c = ([regex]::Matches($content, '\}')).Count
"braces: open=$o close=$c diff=$($o-$c)"

# 3. Roslyn syntax check (optional, slower)
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\bin\mono.exe' `
  'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\lib\mono\msbuild\Current\bin\Roslyn\csc.exe' `
  /noconfig /target:library /langversion:preview `
  /out:Temp/syntax.dll $p 2>&1 | Select-String 'error CS1'
```

E. 受け入れ基準: 全 3 つで 0 件 / diff=0 / 構文エラーなし

### F. Codex との運用ルール再徹底

- 同時編集を避ける (片方が `CoreLanternGame.cs` を触っている間、もう片方は素材・ドキュメント)
- 完了時に「brace diff=0 / 構文エラー 0」をハンドオフ書類に明記する義務

---

## 6. アクション (担当・期限)

| # | アクション | 担当 | 期限 | 状態 |
|---|---|---|---|---|
| 1 | `CoreLanternUnity/CLAUDE.md` 作成 (本ポストモーテム後) | Claude | 即時 | ✅ 本日実施 |
| 2 | git 初期化 + 初回 commit | ユーザー | 1 週以内 | ⏳ |
| 3 | 大型編集前バックアップ習慣化 | Claude | 即時 | ✅ 本ルール追加 |
| 4 | 検証フロー (brace / odd-quote / Roslyn) を編集後ルーチン化 | Claude | 即時 | ✅ |
| 5 | 残存 mojibake コメントの最終修復 (低優先) | Claude/Codex | 任意 | ⏳ |
| 6 | partner description の文言を正規日本語化 | Claude/Codex | リリース前 | ⏳ |

---

## 7. 教訓 (One-liner)

> **「Windows PowerShell で UTF-8 ファイルを `Get/Set-Content` するときは、必ず `-Encoding UTF8` を付ける。さもなくば崩壊する」**

そして:

> **「Edit tool が使えるなら Edit を使え。PowerShell は最後の手段」**

---

最終更新: 2026-05-28
作成: Claude (秘書)
レビュー: ユーザー
