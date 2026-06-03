# CoreLanternUnity — プロジェクト個別運用ルール

このファイルはこのプロジェクト固有の **絶対ルール**。
グローバル `~/.claude/CLAUDE.md` の上書き優先。

参考: 2026-05-27 の文字化け事故 (`docs/POSTMORTEM_20260528_MOJIBAKE.md`) の再発防止策を含む。

---

## 🔴 ハード制約 (絶対遵守)

### 0. Codex 作業開始ルール（最優先）

このプロジェクトで Codex が作業を始めるときは、**最初に必ず `HANDOFF_FOR_CODEX.md` を全文読む**。

- 「続きから」「進めて」「デバッグして」など短い依頼でも例外なし。
- Claude との同時作業後、レート制限明け、コンテキスト圧縮後、別スレッド再開後も必ず読み直す。
- 読めない場合は作業を始めず、ユーザーへ「`HANDOFF_FOR_CODEX.md` が読めない」と報告する。
- 読了後に `CoreLanternGame.cs` の担当境界を確認する。原則、**Codex は `CoreLanternGame.cs` を編集しない**。
- 完了報告には必要に応じて「`HANDOFF_FOR_CODEX.md` 読了済み」と明記する。

このルールは下記 A〜I より優先する。

### A. `CoreLanternGame.cs` の編集ルール

このファイルは 16,000+ 行の単一巨大ファイルで、一度壊すと復旧が困難。

1. **PowerShell の `Get-Content` / `Set-Content` でファイル本体を書き換えない**
   - 検査・スキャン用 (read-only) は OK
   - **書き戻し系は完全禁止**。`Set-Content -NoNewline` も含めて使わない。
   - 理由: Windows PowerShell 5.1 の既定エンコーディングが `Default` (= ANSI) で、UTF-8 ファイルが破壊される (`バージョン` → `バ�Eジョン` 等の mojibake が大量発生する)。

2. **編集は Edit / Write tool のみを使う**
   - Edit の `replace_all` で複数箇所一括置換は OK
   - 大量パターンを一気に処理したくても、Edit を複数回呼ぶ方が安全

3. **どうしても PowerShell で書き戻したい例外ケース**
   - 必須条件すべて満たす:
     - 事前バックアップ: `Copy-Item X X.bak_YYYYMMDD_HHmmss`
     - エンコーディング明示: `[System.IO.File]::ReadAllText($p, [System.Text.UTF8Encoding]::new($false))` + `WriteAllText` 同様
     - 編集後に検証フロー (下記 D) を実行
   - `Get-Content -Encoding UTF8` も同様 (ただし PowerShell 5.1 の `Set-Content -Encoding UTF8` は BOM 付きで Unity が嫌う場合あり、`[System.IO.File]::WriteAllText + UTF8Encoding(false)` 推奨)

### B. 編集後の検証フロー (必須)

`CoreLanternGame.cs` を編集した後、**必ず構文・飲み込み・文字化け警告の検査を実行**:

#### B-0. 推奨: ワンコマンド版 (本書セクション B/C を全自動化)

```powershell
.\Tools\CheckCompileHealth.ps1            # 全検査 (約 30 秒、Roslyn / mojibake warning 含む)
.\Tools\CheckCompileHealth.ps1 -Quick     # 高速 (約 1 秒、Roslyn 省略)
```

→ Exit code 0 なら OK、1 ならエラー。
→ 完了報告には末尾に出力される `Report for handoff:` の 1 行をコピペ。

下記 B-1 〜 B-3 と C は CheckCompileHealth.ps1 の中身。ツールが使えない環境では手動実行可。

#### B-1. 手動実行 (詳細)

```powershell
$p='Assets/Scripts/CoreLanternGame.cs'

# 1. odd-quote 検査
$lines = [System.IO.File]::ReadAllLines($p, [System.Text.UTF8Encoding]::new($false))
$inactive=0; $odd=0
foreach ($line in $lines) {
  $trimmed = $line.Trim()
  if ($trimmed -eq '#if false') { $inactive++; continue }
  if ($trimmed -eq '#endif' -and $inactive -gt 0) { $inactive--; continue }
  if ($inactive -gt 0) { continue }
  $stripped = ($line -replace '//.*$', '') -replace '\\\"', ''
  if ((($stripped.ToCharArray() | Where-Object { $_ -eq '"' }).Count % 2) -ne 0) { $odd++ }
}
"odd-quote lines: $odd"  # 0 必須

# 2. brace バランス検査
$content = [System.IO.File]::ReadAllText($p, [System.Text.UTF8Encoding]::new($false))
$o = ([regex]::Matches($content, '\{')).Count
$c = ([regex]::Matches($content, '\}')).Count
"braces: open=$o close=$c diff=$($o-$c)"  # diff=0 必須

# 3. Roslyn 構文チェック (コンパイルエラー 0 必須)
$mono='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\bin\mono.exe'
$csc='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\lib\mono\msbuild\Current\bin\Roslyn\csc.exe'
$unity='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\Managed\UnityEngine'
$netstd='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\NetStandard\ref\2.1.0'
if (!(Test-Path -LiteralPath 'Temp')) { New-Item -ItemType Directory -Path 'Temp' | Out-Null }
$args = @('/noconfig','/nostdlib+','/target:library','/langversion:preview','/out:Temp/syntax.dll')
$args += (Get-ChildItem -LiteralPath $netstd -Filter '*.dll' | ForEach-Object { '/r:' + $_.FullName })
$args += (Get-ChildItem -LiteralPath $unity -Filter 'UnityEngine*.dll' | ForEach-Object { '/r:' + $_.FullName })
$uiDll = 'Library/ScriptAssemblies/UnityEngine.UI.dll'
if (Test-Path $uiDll) { $args += '/r:' + (Resolve-Path $uiDll) }
$src = (Resolve-Path $p)
$args += $src
$out = & $mono $csc @args 2>&1
$errors = $out | Where-Object { $_ -match 'error CS' }
"Compile errors: $($errors.Count)"  # 0 必須
```

検査結果は必ず報告に含める (`brace diff=0 / odd-quote=0 / compile errors=0 ✓` のような形式)。

### C. swallowed code 検出 (Codex / Claude 共通)

過去事故では `}` や `list.Add(...)` が `//` コメント末尾に飲み込まれる症状があった。バルク編集後は以下も実行:

```powershell
$swallowed = 0
foreach ($line in $lines) {
  if ($line -match '^\s*//.*?\s+(list\.Add|upgradePool\.Add|fusionUpgradePool\.Add|pool\.Add\(new RelicData|if\s*\()' -or
      $line -match '^\s*[^/]*//[^\r\n]*\s+\}\s*$') {
    $swallowed++
  }
}
"Swallowed code lines: $swallowed"  # 0 必須 (英語コメント "return so" 等の誤検出があれば手動確認)
```

### D. バックアップ習慣

`CoreLanternGame.cs` を 500 行以上一気に編集する前 (例: ファイル分割、大型リファクタ) は:

```powershell
Copy-Item "Assets/Scripts/CoreLanternGame.cs" "Assets/Scripts/CoreLanternGame.cs.bak_$(Get-Date -Format yyyyMMdd_HHmmss)"
```

`.gitignore` に `*.bak_*` 追加で git を汚さない。

---

## 🟡 通常ルール (推奨遵守)

### E. Codex との同時編集回避（2026-05-30 強化）

2026-05-30 に Claude と Codex が `CoreLanternGame.cs` を同時編集する衝突が発生した
（被害は Claude 側編集の失敗のみで済んだが、文字化け事故の再来リスクがあった）。
再発防止のため、**役割の境界を恒久固定する**:

- **`CoreLanternGame.cs` の編集は Claude 専任**。Codex は cs を編集しない。
  - Codex = 素材生成（PNG/wav）・`Tools/`・docs 担当
  - Claude = cs を独占（ロジック / バランス / バグ / 素材のコード接続）
  - **「素材をコードに繋ぐ」工程まで Claude がやる。** Codex は素材を作って「繋いでね」と
    `REMAINING_TASKS.md` に書くだけ。これで cs を触るのが 1 人になり構造的に衝突しない。
- **ロック機構**: cs を触る前に `EDIT_LOCK.md` で `STATUS: RELEASED` を確認し、
  `LOCKED` を取得してから編集。完了後 `RELEASED` に戻す（保険。原則 Claude のみ取得）。
- **Claude 自身の自衛**: cs を Edit する直前に対象付近を Read で再確認し、
  直近に変更された痕跡（想定外の新メソッド等）があれば編集を中断して状況を報告する。
- 詳細な役割分担は `HANDOFF_FOR_CODEX.md`（Codex 起動時に読ませる）。
- 完了報告には必ず以下を明記:
  - `brace diff=0`
  - `compile errors=0` (Roslyn 通過)
  - 影響範囲 (どのメソッド・どの行)

### F. 大型 mojibake 復旧時の優先順位

万一また mojibake が発生した場合:

1. **コンパイル復旧最優先** (syntax errors を 0 にする)
2. **UI 文字列の修復** (ボタンラベル、メッセージ、description)
3. **コメントの修復** (実害なし、後回し)

### G. 進捗ドキュメントの更新

大型変更後は以下のいずれかを更新:

- `REMAINING_TASKS.md` の「直近完了」セクションに 5-15 行のサマリ追加
- 重大な事故・方針変更は `docs/POSTMORTEM_*.md` または `docs/*_SPEC.md` を新規作成

---

## 🟢 推奨ベストプラクティス

### H. テキスト編集の優先度

| ツール | 用途 | 安全度 |
|---|---|---|
| **Edit (`replace_all`)** | 同一文字列の複数箇所置換 | ◎ |
| **Edit (単発)** | ピンポイント置換 | ◎ |
| **Write** | ファイル全体書き換え (新規 or 完全置換) | ○ (UTF-8 BOM 無しで保存) |
| PowerShell read-only | スキャン・検査・report 生成 | ○ (書き込まない) |
| PowerShell write | 原則 NG (大量置換等の必須ケースのみ) | × (要 UTF-8 明示) |
| `sed` / `awk` via Bash | スキャン用 | ○ (書き込まない) |

### I. Unity 復帰時の自己点検

ローカル変更を加えた後、Unity Editor で確認するときは:

1. Console を開く (Ctrl+Shift+C)
2. 赤エラーが出ていないか確認
3. 出ていたら即座に Claude に貼り付けて修復依頼
4. Play で起動 → メインメニュー → 1 ラン通しプレイ

### J. Claude のレート制限を抑える検収分担（2026-06-02 ユーザー合意）

Claude が素材PNGを毎回開いて目視検収するとトークン消費が激しく、レート制限が早く来る。
**視覚チェックはユーザー（Unityで見る）に寄せ、Claude は画像を極力開かない。**

- **Claude**: ①Codex への指示文を書く ②cs コード（Claude 専任）③ユーザーが問題報告したときだけ原因診断
- **ユーザー**: ①Codex 指示を貼る ②Unity で見て OK/NG 判断 ③NG なら「何が変か＋スクショ」を Claude に渡す
- **Codex**: 生成＋自己検収（数値で報告。HANDOFF「素材品質ゲート」準拠）

Claude 側の節約ルール:
- **素材PNGを Read で開くのは原則しない。** Codex の数値報告 / 小さい `validation_report.json` で足りる
- ユーザーがスクショを貼ったら、それを見て診断する（メッセージ内画像はタダ。PNGを開き直さない）
- どうしても本体を開く必要があるのは「数値とスクショでも原因が特定できない」例外時のみ
- 応答は簡潔に。冗長な再説明をしない

---

### K. Claude Code カスタム構成 (agents / skills / commands) の使い分け（2026-06-03 追加）

棚卸し結果は `docs/AGENTS_SKILLS_COMMANDS_AUDIT_20260603.md`。`.claude/` 配下は新規追加のみで、
既存スクリプト・ルールは未変更（Codex は `HANDOFF_FOR_CODEX.md` 駆動で `.claude/` を参照しないため非干渉）。

**プロジェクト固有のスラッシュコマンド** (`.claude/commands/`):
- `/check-health` — `Tools/CheckCompileHealth.ps1` のラッパー。cs 編集後の必須検証。
- `/validate-assets [カテゴリ]` — `Tools/ValidateCodexAssets.ps1` のラッパー。

**プロジェクト固有のサブエージェント** (`.claude/agents/`):
- `cs-guardian` — `CoreLanternGame.cs` 編集の門番。EDIT_LOCK 確認→Edit/Write 限定→検証を内蔵。
  cs のバグ修正・バランス調整・素材のコード接続を依頼するときに使う。

**ビルトインの使い分け**:
- 探索: 結論だけ欲しい広域検索→`Explore` / 実装を含む多段タスク→`general-purpose`。
- レビュー: diff の品質→`/code-review`（主軸）。`review`=PR 単位、`security-review`=任意の保険。
- `verify` / `run` は **Windows 実機専用**（Unity 6000.4.7f1 は Windows のみ）。Linux リモートでは使わない。
- `claude-api` / `keybindings-help` / `init` は本プロジェクト非対象（`init` は本 CLAUDE.md 上書きリスクのため使用禁止）。

---

## 参照

- `docs/AGENTS_SKILLS_COMMANDS_AUDIT_20260603.md` — agents/skills/commands 棚卸し監査
- `docs/POSTMORTEM_20260528_MOJIBAKE.md` — 大規模文字化け事故の詳細
- `HANDOFF_FOR_CLAUDE_COMPILE_FIX_20260528.md` — 復旧作業の引継ぎ書
- `~/.claude/CLAUDE.md` — グローバル運用ルール (秘書/部署制)

最終更新: 2026-05-28
