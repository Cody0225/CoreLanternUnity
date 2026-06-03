# CoreLanternUnity — Codex 引き継ぎノート

## 🔴 Codex 起動時の絶対ルール（最優先）

Codex は、このプロジェクトで作業を始める前に **必ずこの `HANDOFF_FOR_CODEX.md` を全文読む**。

- 「続きから」「進めて」「確認して」など短い依頼でも例外なし。
- Claude との同時作業後、レート制限明け、コンテキスト圧縮後、別スレッド再開後も必ず読み直す。
- 読めない場合は作業を始めず、ユーザーに報告する。
- 読了後、まず現在の役割境界を確認する。特に **Codex は `Assets/Scripts/CoreLanternGame.cs` を編集しない**。
- 素材生成・検証・docs整理を優先し、cs 接続やロジック修正は Claude 接続待ちとして `REMAINING_TASKS.md` に残す。
- **「全文読む」対象はこの本体ファイルのみ。** 過去の作業履歴は `HANDOFF_ARCHIVE.md` に退避済みで、
  そちらは **必読ではない**（経緯を追うときだけ参照）。

このルールは、このファイル内の他のすべての手順より優先する。

## 🟢 連絡: `.claude/` 追加（2026-06-03・Claude / Codex への影響なし）

Claude Code 専用の `.claude/`（`/check-health`・`/validate-assets` コマンド、`cs-guardian` エージェント）を
追加した。**Codex の作業手順・既存スクリプト・パス・役割分担はすべて不変**（Codex は `.claude/` を参照しない）。
詳細は `docs/AGENTS_SKILLS_COMMANDS_AUDIT_20260603.md`。

## 🟢 進行中タスク: OSS公開（2026-06-04・Claude→Codex 引き継ぎ）

ユーザーは Eggcore Protocol を GitHub で OSS 公開中。Claude がレート制限のため Codex へ引き継ぎ。

### 確定した方針（変えない）
- ソースコード = **MIT**（`LICENSE`：`Copyright (c) 2026 Cody0225`）
- アセット（画像/音/ロゴ/キャラ/ブランド）= **All Rights Reserved・再利用不可**（`ASSET_LICENSE.md`）
- 公開範囲 = **ゲーム本体のみ**。`Assets/ArtSource/`（120MB・実験/バックアップ）は公開しない

### 完了済み（Claude）
- `LICENSE`（MIT, Cody0225）/ `README.md`（ライセンス記述を確定）/ `ASSET_LICENSE.md`（確定版）/ `.gitignore`（ArtSource＋秘密ファイル除外）
- 秘密スキャン: クリーン（APIキー/トークン/メール/パスワード無し）
- git: 旧サンドボックス .git は削除済み。**GitHub Desktop が新しい“ユーザー所有”の .git を作成・初回コミット済み**（追跡 1457 ファイル＝ArtSource/Library 除外・Skins 595枚含む・クリーン）
- GitHub アカウント: **Cody0225**（新規）

### 残り（ユーザーの GUI 操作）
- GitHub Desktop で **Publish repository（まず Private）** → `github.com/Cody0225/CoreLanternUnity` で確認 → OKなら Settings → Change visibility → **Public**

### 🔴 Codex への警告（厳守）
- **git 操作（init/commit/push 等）を Codex サンドボックスから実行しない。** 以前 Codex のサンドボックスが .git を作り、所有者不整合（dubious ownership）を起こして削除・再作成する羽目になった。git は今 **GitHub Desktop（ユーザー所有）**が管理。**触らない・git init しない。**
- **`Assets/ArtSource/` を公開リポジトリに戻さない**（意図的に .gitignore で除外）。
- `CoreLanternGame.cs` は従来通り **編集禁止**（Claude 専任）。
- Codex の画像生成は現在不調（無関係な教育図＝States of Matter 等を返す）。**画像生成は当てにしない**（復帰確認は英語最小プロンプトで）。

### 非OSSのペンディング（参考）
- `docs/PENDING_IMAGEGEN_TASKS.md`（S4/S5陰影・F2〜F6融合・HUD素材リニューアル＝全部 imagegen 復帰待ち）
- `docs/IMPROVEMENTS_BACKLOG.md`（HUD配置・レアリティ/★ 等のバッチ改善）

## 🔴 2026-06-02 改定: 素材品質ゲート（トークン浪費・後追い修正の撲滅）

2026-06-02、相棒6体ピクセルアート発注で「初回生成 → 色調整 → 不透明度修復 → ゴミ除去」と
**4回パス**を要した。後ろ3回は初回ミスの後追いで、トークンを大量に浪費した。
**今後は「一発で正しく出す」ことを最優先にする。以下を厳守。**

### A0. 既存画像の「編集・除去」を頼まない＝新規生成のみ（最重要・2026-06-02追加）

2026-06-02、Wave2のエフェクト除去で **Codex は約1時間に13回パスを繰り返しても効果を消せなかった**
（焼き込まれた装飾の除去は、描き直し以外にできないため）。
教訓: **Codex は「新規に描く」はできても「既存画像を編集・除去・修正」はできない。**

- ✕ NG な依頼の出し方: 「このエフェクトを消して」「色だけ直して」「ここを調整して」
  → 編集型は失敗 or 大量パスでトークンを溶かす。
- ◯ 正しい出し方: ダメなら毎回 **「ゼロから新規フルボディで描き直す」**。
  消したい要素は「最初から描かない」制約として与える（例: 「エフェクトを焼き込まない」）。
- 既存の良い部分を保ちたい場合も、編集ではなく「同じ方向性で新規に描く」と指示する。

### A. 透過処理ルール（色を殺さない・最重要）

色飛び事故の根本原因は「背景を色で抜く chromakey」方式。背景と近い色のキャラ画素まで
一緒に消えるため、濃い色のキャラ（紫の Hex Cat 等）が透けて色が飛ぶ。

- **原則: 最初から透明背景に描く**（背景を後から抜かない）。
- どうしても背景を抜くなら、**キャラのパレットに存在しないキー色**を使う
  （例: キャラに使っていない蛍光グリーン `#00FF00` / 蛍光マゼンタ `#FF00FF` を背景にして抜く）。
  キャラと同系色の背景でキーイングしない。
- 抜いた後、**本体の不透明部の alpha が落ちていないか必ず確認**
  （本体ベタ部は alpha ≥ 230 を目安。半透明スカスカ厳禁）。
- 「四隅 alpha=0」だけでなく、**本体内部に意図しない透け穴がないか**も確認する。

### A2. エフェクト重ね掛け禁止（静止画はクリーンに・全キャラ共通）

キャラ素材に「動きの筋/スピードライン/浮遊スパークル/散る粒子/平面的な幾何リング・矢印/
照準UI風オーバーレイ/余計なオーラ」を**焼き込まない**。
- 描くのは「キャラ本体＋実体のある装甲・武器・パーツ・コアの発光」だけ。
- 残像・グロウ・パーティクル・動きの演出は**後で Claude がコードで足す（Type A）**。
  静止画スプライトはクリーンに保つ。
- ルートや個性は「実体パーツ（翼・砲・盾板）」と「本体の発光」で表現。飛び散る装飾でごまかさない。

### B. 完了前セルフ検収ゲート（往復ループの撲滅・必須）

**「できた」と報告する前に、Codex 自身が以下を全部やってから報告する。
1つでも × なら、報告せずに直してから出す。** 不具合を知ったまま Claude/ユーザーに渡さない。

1. **検証スクリプトを実行**し、各ファイルの実測値を取る（既存の `validation_report.json` 方式）
2. **出力画像を実際に開いて目視**する（色・シルエット・ゴミ・透け穴）。
   横並びプレビューも開いて全体の統一感を見る
3. **発注書の検収基準を1項目ずつ実測値付きで確認**する
4. 完了報告に**実測値（size / bbox / bottom_y / 四隅alpha / 本体中央の色サンプル・alpha）**を必ず貼る。
   「できました」だけの報告は不可。数値で示す

### C. 報告テンプレ（これを埋めて報告）

```
対象: <ファイル名>
HANDOFF_FOR_CODEX.md 読了済み: はい
実測: size=512x512 / bottom_y=448 / 四隅alpha=[0,0,0,0] / 本体中央 color=#RRGGBB alpha=NNN
目視: 色◯ シルエット◯ ゴミ無◯ 透け穴無◯ / 横並びプレビュー統一感◯
発注基準: 1.◯ 2.◯ 3.◯ …（× があれば直してから報告）
meta: PNG同名上書きのみ / .meta未編集（末尾0a維持）
```

このゲートを通さずに出した素材で後追い修正が発生した場合、それは「やり直し」であって
新規作業ではない。**最初の1回で通すことがトークン節約に直結する。**

**このゲートが最上位。** キャラ制作の各QA文書
（`docs/CHARACTER_PIXEL_ART_PIPELINE.md` / `CHARACTER_ANIMATION_GLOBAL_QA.md` /
`CHARACTER_SECRETARY_REVIEW_PROTOCOL.md`）はこのゲートに従属する。
また**現在はスプライトアニメ機構が無いため静止画運用**。これら文書のアニメ系QA
（RUN/ATTACK/GIF/Animator/接地順）は将来用に休眠で、今はキャラを静止画1枚で作る。

## 🔴 2026-05-30 改定: 同時編集衝突の恒久防止（最重要・他のすべてに優先）

2026-05-30、Claude と Codex が `Assets/Scripts/CoreLanternGame.cs` を同タイミングで編集する衝突が発生した。
今回は Claude 側の編集が失敗しただけで済んだが、2026-05-27 の大規模文字化け事故の再来リスクがあった。
**再発防止のため、役割の境界を以下に固定する。**

### 役割分担（恒久ルール）

| 担当 | やること | 触ってよいファイル |
|---|---|---|
| **Codex** | 素材生成（PNG / wav）、`Tools/Generate*.ps1`、検証スクリプト実行、docs 整備 | `Assets/Resources/Skins/`、`Assets/Resources/Audio/`、`Tools/`、`docs/`、`*.md` |
| **Claude** | 素材を cs に繋ぐ、ロジック / バランス / バグ修正、UI 接続 | **`CoreLanternGame.cs` を独占** |

- **`CoreLanternGame.cs` は Codex 編集禁止。** 素材を「コードに繋ぐ」工程まで含めて Claude が担当する。
- Codex が PNG/wav を「作る」だけなら別ファイルなので絶対に衝突しない。衝突したのは
  Codex が素材を cs に「繋ぐ（= cs を編集する）」ところまでやったから。
- **Codex は素材を作って `REMAINING_TASKS.md` に「Claude 接続待ち」と書くだけでよい。**
  接続（cs 編集）は Claude が行う。

### Codex が素材を作ったときの手順

1. `Assets/Resources/Skins/`（音声は `Audio/`）に PNG/wav + `.meta` を置く
2. `IMAGE_ASSET_BACKLOG.md`（音声は `AudioManifest.json`）に**寸法・用途・接続先メソッド名**を記載
3. `REMAINING_TASKS.md` の「直近完了」に「**Claude 接続待ち**」と明記
4. **cs は触らない。**

### ロック機構（保険）

万一 cs を触る必要が生じたら、`EDIT_LOCK.md` で `STATUS: RELEASED` を確認し
`LOCKED` を取得してから編集、完了後 `RELEASED` に戻す。原則 Claude のみが取得する。

### Claude 接続待ちの素材（2026-05-30 時点）

- `Result_StatTile_v2`: 2026-05-31 Codex が **420×56** に再生成済み。必要ならClaude側で現行リザルトUIに合わせて採用確認する
- `Result_RankMedal_{S/A/B/C/D}_v2`: ランク専用枠が UI に無い。Claude が枠追加後に接続

### タイトル画面スマート化に伴う注意（2026-05-31 Claude）

`docs/CLAUDE_TITLE_SCREEN_SIMPLIFY_SPEC.md` に沿ってタイトルを簡素化した。これにより
以下の Title V2 スプライトは **ロードはするが現在タイトルでは描画していない**（宙ぶらりん状態）:

- `Title_SubtitlePlate_v2` / `Title_StatsRibbon_v2` / `Title_LogoUnderline_v2` / `Title_NavRail_v2`

→ 動作に害はない（フィールド代入済みなので未使用警告も出ない）。**Codex は削除しないこと。**
将来タイトルに戻す/別画面で使う可能性があるため残置。タイトルで再利用したい場合は
Claude に接続依頼（cs 編集は Claude 担当）。

`Title_CoreEmblem_v2` はヒーローデッキ専用で、ヒーローデッキ自体がタイトル非表示になったため
現在は出ていない（`CreateMainMenuHeroVisual` 内で生成されるが、この関数はタイトルから呼ばれない）。
`Title_CornerAccent_v2`（四隅装飾）はタイトルに残置している。

### タイトル6体横一列レイアウト（2026-06-02 Claude 実装済み）

タイトル中央に **初期相棒6体（Partner_S1..S6_L0）を横一列・均等配置** で実装済み
（`CreateMainMenuHeroArt`、Roslyn 0err）。各キャラはアクセント色のグロウ＋足元グロウ付き。
ユーザー確定方針「6体全員・ピクセルアート風」を満たした構成。

- 6体ピクセルアートは Codex が生成・色復旧・ゴミ除去まで完了（2026-06-02）。
- **cs 接続は不要**: 既存の `Partner_S{n}_L0.png` を同名上書きすれば自動反映される。
  Codex がキャラを再修正する場合も**同名 PNG 上書き + 既存 .meta 維持**でよい（cs は触らない）。
- 関連 docs: `docs/CODEX_CHARACTER_PIXELART_SPEC_20260601.md`（生成仕様）、
  `docs/CODEX_PIXELART_CLEANUP_20260602.md`（ゴミ除去）。

---

## ⚠ 2026-05-28 現在の最優先

Unity が `Assets/Scripts/CoreLanternGame.cs` の構文エラーで Play できない状態。
まず `HANDOFF_FOR_CLAUDE_COMPILE_FIX_20260528.md` を読み、コンパイル復旧を完了してから他タスクに戻ること。

> ※ 追記（2026-06-03）: その後 2026-06-01 までの複数セッションで `compile errors=0` を継続確認済み
> （履歴ログ参照）。本項は**解決済み**で、現在の最優先ではない。経緯参照用として残置（削除しない）。

最終更新: 2026-05-25
担当遷移: Claude Code → Codex

## 📖 リファレンス・履歴の所在（毎回読まなくてよい）

- **不変リファレンス**（プロジェクト概要 / 設計判断 / ゲームデザイン要点 / 操作 /
  メソッド地図 / 状態フィールド早見表 / ハード制約 / 既知の警告）→ **`HANDOFF_REFERENCE.md`**。
  一度読めば十分。
- **過去の全セッション履歴** → **`HANDOFF_ARCHIVE.md`**（必読外・経緯参照用）。
- **Claude 専用ルール**（cs 編集規律 / mojibake 防止 / `.claude/` ツール）→ **`CLAUDE.md`**（A〜K）。

## 直近のセッション履歴

> 過去の全セッション履歴（2026-05-18〜05-25 の Codex/Claude 作業ログ）は
> **`HANDOFF_ARCHIVE.md`** に退避済み。経緯を追う必要があるときだけ参照すればよく、
> **起動時の必読対象ではない**。
> 最新の動きは本ファイル冒頭の「🟢 連絡」「🟢 進行中タスク」セクションを参照。

## 残タスク（Codex が引き継ぐ場合の優先順）

### 必須（プレイ前確認）
1. **Unity Editor で起動して目視確認**
   - メインメニューが日本語含めて正しく表示されるか
   - 図鑑のカード配置と色が想定通りか
   - リザルト画面のMVPビルド表示が崩れないか
   - 進化カットシーンとカード選択の重なりがないか
2. **日本語文字化けがあれば修正**
   - フォントが見つからない場合のフォールバック確認
   - TextMesh が混在する FloatingText も確認

### 高優先
3. **BGM/SE の本実装**
   - `Assets/Resources/Audio/` に WAV/MP3 を置けば自動ロード
   - 候補: BGM, Shoot, Hit, Kill, Pickup, LevelUp, Evolve, Fusion, Boss, GameOver
4. **キャラスプライトの差し替え**
   - `Assets/Resources/Skins/` に `Player_Form1.png` 〜 `Player_Form4.png` などを置く
   - 商用品質のキャラデザインを別途用意

### 中優先
5. **シングルファイルの分割計画**
   - 5,500行超で IDE のリロードが重い
   - 候補分割: `CoreLanternGame.cs` (本体) / `CoreLanternGame.Sprites.cs` (procedural) / `CoreLanternGame.UI.cs` (Canvas構築) / `CoreLanternGame.Upgrades.cs` (モジュール定義)
   - **注意**: 1ファイル維持の方針があるので、分割前にユーザーに確認すること
6. **オプション保存の永続化**
   - 現在オプション (HUD詳細/画面揺れ/BGM等) は PlayerPrefs に保存していない。次起動時にリセットされる
7. **メタ進行の追加**
   - クリア回数でスタートボーナスや新パートナー解放

### 低優先
8. **コード上の `_var` 未使用警告掃除**
9. **`allyName` の用途確定または削除**
10. **シーン化（Prefab/Scene への移行）** — 商用配布段階での話、急がない


> 上記より下にあった完了済みの作業ログ（2026-05-19〜05-25 の各セッション詳細＝
> 音響/スプライト/レリック/UI/バランス等の実装記録）は **`HANDOFF_ARCHIVE.md`** に
> 退避済み。アクティブな残タスクは上記「必須/高/中/低優先」のみ。

## Codex が作業を始める時のチェックリスト

1. **絶対ルール: まずこの `HANDOFF_FOR_CODEX.md` を全文読む**
   - 短い再開依頼、コンテキスト圧縮後、Claudeとの並行作業後も必ず読み直す
   - 読めない場合は作業せずユーザーに報告する
2. `CoreLanternUnity/Assets/Scripts/CoreLanternGame.cs` を読む（最低でも `Awake`, `Update`, `BuildUpgrades`, `OpenUpgrade`, `OpenEvolution`, `ShowResult`, `BuildCodexText` 周辺）
3. Unity Editor で開いて Play → 1ラン通してプレイし、現状把握
4. このドキュメントの「残タスク」から着手項目を選ぶ
5. 編集後は **ビルドが通ること** と **Play でクラッシュしないこと** を必ず確認
6. 変更内容をこのドキュメントの「直近のセッション履歴」に追記

## 連絡先・参考

- プロジェクトルート (ユーザー端末): `C:\Users\kodai\OneDrive\デスクトップ\claudecode-app\CoreLanternUnity`
- ユーザー方針: 「ヴァンサバ的気持ちよさ + 進化のワクワク感 + 売れるレベルまで磨く」
- 既存資料: `README.md`, `DIGITAL_MONSTER_STYLE_PLAN.md`
