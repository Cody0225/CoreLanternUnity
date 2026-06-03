# CoreLanternUnity — Codex 引き継ぎノート

## 🔴 Codex 起動時の絶対ルール（最優先）

Codex は、このプロジェクトで作業を始める前に **必ずこの `HANDOFF_FOR_CODEX.md` を全文読む**。

- 「続きから」「進めて」「確認して」など短い依頼でも例外なし。
- Claude との同時作業後、レート制限明け、コンテキスト圧縮後、別スレッド再開後も必ず読み直す。
- 読めない場合は作業を始めず、ユーザーに報告する。
- 読了後、まず現在の役割境界を確認する。特に **Codex は `Assets/Scripts/CoreLanternGame.cs` を編集しない**。
- 素材生成・検証・docs整理を優先し、cs 接続やロジック修正は Claude 接続待ちとして `REMAINING_TASKS.md` に残す。

このルールは、このファイル内の他のすべての手順より優先する。

## 🟢 連絡: agents/skills/commands 監査と `.claude/` 追加（2026-06-03・Claude）

**結論: Codex の作業手順は一切変更なし。** 下記の追加はすべて Claude Code 専用の `.claude/` 配下で、
Codex は参照不要・非干渉。Codex は今まで通り本ファイルを全文読んでから着手する。

### 追加したもの（ブランチ `claude/agents-skills-commands-audit-jc1kU`）
1. `docs/AGENTS_SKILLS_COMMANDS_AUDIT_20260603.md` — agents/skills/commands の棚卸し・スコアリング監査。
   重複/未使用/保守コスト高/本作に不要 を抽出（**削除はせず提案のみ**）。
2. `.claude/commands/check-health.md` — スラッシュコマンド `/check-health`。`Tools/CheckCompileHealth.ps1` のラッパー（本体不変）。
3. `.claude/commands/validate-assets.md` — スラッシュコマンド `/validate-assets`。`Tools/ValidateCodexAssets.ps1` のラッパー（本体不変）。
4. `.claude/agents/cs-guardian.md` — cs 編集の門番サブエージェント（Claude 用）。
5. `Tools/README.md` 追記 — 未記載だったワンショット py 8本を「実行済み履歴」として明示（**削除・移動なし。呼び出しパス不変**）。`/check-health` への導線追記。
6. `CLAUDE.md` セクション K 追記 — 新 `.claude` ツールの使い分け方針（既存 A〜J ルールは不変）。

### Codex への影響: なし
- 既存スクリプト本体・引数仕様・パスは全て不変。`ValidateCodexAssets.ps1` / `CheckCompileHealth.ps1` はそのまま手打ちで使える。
- 役割分担（cs=Claude 専任、素材=Codex、HANDOFF 必読）も変更なし。
- 自動実行 hook（SessionStart / PreToolUse）は **あえて未導入**（Windows/Linux 互換と「勝手に動く」リスク回避）。導入は要合意。

### 動作確認
- `/check-health` を Linux リモートで実行 → brace diff=0 / odd-quote=0。
  Roslyn/mojibake/swallowed の本検証は Windows 実機の `.ps1` が必要、という想定どおりの挙動。

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

最終更新: 2026-05-25
担当遷移: Claude Code → Codex

## このドキュメントの位置づけ

Codex が「コンテキストゼロから」このプロジェクトに入って、すぐに作業を続けられるように設計された引き継ぎ資料。プロジェクトの全体像、設計上の不変、直近の変更、未解決タスクをすべて含む。プロジェクトの README より詳細で、CLAUDE.md より実装寄り。

## プロジェクト概要

### 名称
- 開発名: CoreLanternUnity
- 内部タイトル: Eggcore Protocol
- ジャンル: ヴァンサバライク + ローグライク + コア防衛
- モチーフ: デジモン風の「進化・相棒・融合のワクワク感」を商用前提のオリジナルIPで再構築

### Unity 環境
- Unity Editor: `6000.4.7f1`
- レンダリング: 2D (orthographic camera)
- パッケージ: UGUI (`com.unity.ugui`), Multiplayer Center (未使用、PackageCache のみ)
- 配置: シーンに依存せず、Awake で全オブジェクトを生成

## ファイル構造

```
CoreLanternUnity/
├── Assets/
│   ├── Scripts/
│   │   └── CoreLanternGame.cs        ← メイン実装（5,500行超のシングルファイル）
│   └── Resources/
│       ├── Audio/                     ← BGM/SE 用 (空。なくても動く)
│       └── Skins/                     ← 生成PNG/オプション画像差し替え用 (procedural fallbackあり)
├── ProjectSettings/
├── Packages/
├── README.md                          ← ユーザー向け概要
├── DIGITAL_MONSTER_STYLE_PLAN.md      ← 旧スタイルプラン
└── HANDOFF_FOR_CODEX.md               ← この文書
```

### 重要な設計判断（変えないこと）

1. **シングルファイル設計**
   - `CoreLanternGame.cs` ひとつにゲームロジック・UI 構築・procedural sprite 生成・進行管理を集約
   - シーン上に何も置かない。`[RuntimeInitializeOnLoadMethod]` で起動時に自動セットアップ
   - 理由: プロトタイプ段階の即時起動性とコピペ移植性を優先。ファイル分割はまだしない方針

2. **Procedural sprite 生成**
   - `CreateAssets()` 内で `MakeColorSprite` / `MakeSprite` を使い、64〜144px のテクスチャを動的生成
   - `Resources/Skins/<name>.png` があればそちらを優先する `LoadOptionalSprite` パターン
   - 商用版に向けては外部スプライト差し替えで進化させる

3. **UI も全部スクリプトで構築**
   - Canvas → CanvasScaler (1280x720 reference) → GraphicRaycaster
   - パネルは `CreatePanel(name, parent, pos, size, anchor)` 経由で生成、ネオン枠と角飾りを自動追加
   - テキストは `CreateText(name, parent, pos, anchor, size, color)` 経由（Shadow 付き）
   - ボタンは `CreateButton` (カード型) と `CreateWideButton` (細長型) の 2 系統

4. **日本語フォント**
   - `GetUiFont()` で `Yu Gothic UI` / `Yu Gothic` / `Meiryo` / `MS Gothic` / `Arial` の優先で OS フォントを動的取得
   - Windows 11 環境前提
   - フォールバックは `LegacyRuntime.ttf`

5. **保存系**
   - `PlayerPrefs` を直接使用。データベースや外部保存はなし
   - Keys: `BestWave`, `Clears`, `Partner_<name>`, `Route_<SPEED|POWER|GUARD>`, `Fusion_<name>`, `CoreLantern_AutoStartRun`

## ゲームデザイン要点

### コアループ
1. メインメニュー → 相棒3択
2. ウェーブ1〜10。敵を倒し EXP/データチップ獲得
3. レベルアップで強化モジュール3択（リロールはデータチップ30）
4. Lv3/Lv6/Lv9 で進化（SPEED/POWER/GUARD ルート選択）
5. 仲間リンク（Nova/Bulwark/Siphon）を2種類取り、特定条件でクロス進化（融合）
6. ウェーブ10でボス
7. クリア/敗北 → リザルト → メインメニュー or もう一度

### 相棒 (プール6体、開始時にランダム3体提示)
名前は生成PNG (`Partner_S<species>_L0.png`) の見た目に揃えてある。speciesインデックスがPNGファイルにマッピングされる。
- `Cobalt Pup` (標準型, species=1, 青狼): 攻撃+8%、移動+6%
- `Ember Drake` (弾幕型, species=2, 橙竜): 連射+22%、弾数+1、自分HP-1
- `Sage Hare` (防衛型, species=3, 緑兎): コアHP+7、リング起動、移動-6%
- `Hex Cat` (連鎖型, species=4, 紫魔猫): チェイン+1、残像追撃ON、連射+6%
- `Drift Fox` (回避型, species=5, 桃狐): 移動+15%、Phaseリンクを最初から所持
- `Iron Bear` (装甲型, species=6, 黒熊): 自分HP+2、接触反撃+0.8、移動-8%

旧名→新名マッピング (PlayerPrefsキー含む):
- `Spark Kit` → `Ember Drake`
- `Guard Mole` → `Sage Hare`
- `Tech Owl` → `Hex Cat` (連鎖型に変更、`ハッカー型`は廃止)
- `Echo Cat` → `Iron Bear` (装甲型に変更、連鎖型はHex Catへ移動)

### 進化ルート (各ルートに4分岐、ランダム3提示)
- `SPEED` (formStyle=1): 連射・移動・貫通
  - 連射翼 / 貫通レーザー / ミラージュ / **ホーミングビット** (NEW)
- `POWER` (formStyle=2): 一撃火力・バースト
  - 爆裂コア / 巨弾アーム / ボスブレイカー / **シージモード** (NEW)
- `GUARD` (formStyle=3): 耐久・防衛・回復
  - コアシールド / 反撃装甲 / リカバリーコア / **鏡盾フォーム** (NEW)

### リンク (4種、最大2接続)
- `Nova` (攻撃補助): 連射+3%
- `Bulwark` (防衛補助): コアHP+2
- `Siphon` (回収補助): 回収範囲+8%、データ+4%
- `Phase` (回避補助): 移動+4%、近接敵弾を周期的に位相回避 (NEW)

### 融合（クロス進化、4C2=6パターン）
- `Nova Aegis` (Nova + Bulwark, 弾数3条件): 追撃と防衛
- `Photon Siphon` (Nova + Siphon, データ強化条件): 回収と連射
- `Core Bastion` (Bulwark + Siphon, コアHP25条件): コア防衛特化
- `Nova Phantom` (Nova + Phase, 移動5.4条件): 連射・連鎖・残像 (NEW)
- `Aegis Drift` (Bulwark + Phase, 自分HP8条件): 機動防衛、反射 (NEW)
- `Photon Wraith` (Siphon + Phase, 回収1.4条件): 回収特化、回復 (NEW)

### 融合スタイルコード (fusionStyle)
- 1: Nova Aegis (pink)
- 2: Photon Siphon (lime)
- 3: Core Bastion (gold)
- 4: Nova Phantom (sky blue)
- 5: Aegis Drift (lavender)
- 6: Photon Wraith (mint)

### モジュール段階
- `MaxModuleLevel = 3`。`仲間リンク` 系を除き Lv0→1→2→3
- カード表示は `Lv 0 > 1 / 3` の形式

## 操作

| 入力 | 動作 |
|------|------|
| WASD / 矢印 | 移動 |
| 左クリック長押し | マウス位置へ移動 |
| 攻撃 | 自動 |
| Esc / P | ポーズ |
| O | オプション |
| リザルト中 R | もう一度 |
| リザルト中 M / Esc | メインメニュー |
| タイトル Enter / Space | 開始 |

## コード上の重要ポイント（メソッド地図）

行番号は編集で容易に変わるので、grep でメソッド名を探すのが確実。代表メソッド一覧:

| メソッド | 役割 |
|---|---|
| `StartGame` | `[RuntimeInitializeOnLoadMethod]` 起動エントリ |
| `Awake` | アセット生成→ワールド構築→UI構築→アップグレードプール構築→タイトル表示 |
| `Update` | フレーム駆動 (タイトル/リザルト/ポーズ/カット中の分岐つき) |
| `CreateAssets` | procedural sprite 生成 |
| `MakePartnerSprite` / `MakeStyledPartnerSprite` / `MakeEvolvedPartnerSprite` | 段階別キャラスプライト |
| `CreateWorld` | 床/グリッド/コア/プレイヤー/オーラ生成 |
| `CreateUi` | Canvas, HUD, ポーズ, オプション, パートナー選択, リザルト, ボス, カットを構築 |
| `CreateMainMenuPanel` | メインメニュー全面化 |
| `CreateCodexPanel` / `CreateCodexSection` / `CreateCodexCard` / `RefreshCodexCards` | 図鑑パネル構築 (カード型) |
| `CreatePartnerSelectPanel` / `OpenPartnerSelect` | 相棒選択 |
| `CreateResultPanel` / `CreateBadgeStrip` / `CreateStatTile` / `CreateMvpRow` | リザルト画面構築 |
| `ShowResult` / `SetResultStat` / `BuildMvpList` / `BuildMvpEntry` | リザルト表示 |
| `CreateButton` | カード型ボタン（ヘッダー/アイコン/コア/リング/レール/グロウ層あり） |
| `BuildUpgrades` | モジュール定義一覧（28モジュール + 6 クロス専用） |
| `OpenUpgrade` | レベルアップ時のモジュール選択 |
| `OpenEvolution` | Lv3/6/9 進化選択 |
| `RecordModulePick` | MVPトラッキング用にピックを記録 |
| `ApplyCardVisual` | カードボタンの装飾を一括適用 |
| `UpdateChoiceCardEffects` / `AnimateCardButtons` | カードのパルスアニメ + 出現アニメ |
| `Evolve` | 進化適用 |
| `CheckFusion` / `ActivateFusion` | 融合判定と適用 |
| `QueueEvolutionCutscene` / `BeginQueuedCutscene` | カットシーン |
| `UpdateUi` | HUD 表示更新 |
| `LoadProgress` / `SaveProgress` | PlayerPrefs |
| `RetryRun` / `ReturnToMainMenu` | リザルトから遷移 |
| `GetUiFont` | 日本語対応フォント取得 (Yu Gothic UI / Meiryo / Noto Sans / BIZ UD / Hiragino) |
| `GetModuleTitleAccent` / `GetModuleTitleIcon` | モジュール名からアクセント色とアイコンを推定 |
| `GetRouteAccentColor` | 現在の進化ルートの基調色を返す |

## 状態フィールド早見表

| 種別 | 主なフィールド |
|---|---|
| プレイヤー | `playerHp`, `playerMaxHp`, `moveSpeed`, `aimDirection` |
| コア | `lanternHp`, `lanternMaxHp`, `lightRadius` |
| 弾 | `bulletCount`, `bulletDamage`, `bulletSpeed`, `bulletSize`, `bulletPierce`, `bulletLifeMultiplier`, `explodeChance` |
| 進化 | `evolutionStage` (0-3), `formStyle` (0=base,1=SPEED,2=POWER,3=GUARD), `formName` |
| 融合 | `fusionActive`, `fusionName`, `fusionStyle` (1=NovaAegis,2=PhotonSiphon,3=CoreBastion) |
| リンク | `allyNova`, `allyBulwark`, `allySiphon` |
| シナジー | `scatterSynergy`, `lanceSynergy`, `recoverySynergy`, `aegisSynergy` |
| 進行 | `wave`, `level`, `xp`, `xpToLevel`, `dataChips`, `elapsedTime`, `waveTrait` |
| ランタイム | `enemies`, `bullets`, `pickups`, `sparks`, `floatingTexts` |
| MVPトラッキング | `runDamageDealt`, `runEnemiesKilled`, `runBossKilled`, `runPickedModules` |

## ハード制約

- `.env` / `secrets/` は触らない（実際にはこのプロジェクトには存在しないが原則）
- ライブラリ追加禁止（Unity 標準と UGUI のみ）
- シーン依存禁止（コードのみで完結）
- 商用前提なので、IP を侵害する画像/音源を入れない（外部Skin差し替え時もユーザー責任を明記）

## 既知の警告

- `FindObjectOfType<T>()` が obsolete（Unity 6 で `FindFirstObjectByType<T>()` 推奨）→ 修正済み
- `allyName` フィールドが代入のみで未使用 → そのまま残存（リファクタ対象、削除可だが既存セーブとの整合のため残す）

## 直近のセッション履歴

### 前回 (Claude Code 時点まで)
- 進化/強化カード選択を画面中央配置
- カード選択中の背景HUD暗転とフラッシュ抑制
- HUD のActive Build/データチップ等を控えめに、詳細はEsc/HUD詳細で
- HP表示の隙間調整
- EXPバーが枠外に出る修正
- BUILD LINKパネル小型化
- ステータス上昇量を控えめに
- メインメニュー全面化、図鑑ボタン追加
- 図鑑にPartner/Route/Fusion/Record表示（テキスト）
- クリア/敗北リザルト追加（テキスト）

### 今回 (Claude Code セッション、2026-05-18)
- **HANDOFF_FOR_CODEX.md** をプロジェクトルートに作成（このファイル）
- `FindObjectOfType<T>()` を `#if UNITY_2023_1_OR_NEWER` で `FindFirstObjectByType<T>()` に分岐（Unity 6 / 旧バージョン両対応）
- **MVP トラッキング** 追加
  - 新フィールド: `runDamageDealt`, `runEnemiesKilled`, `runBossKilled`, `runPickedModules`, `runModulePicks`
  - `DamageEnemy` で総ダメージ加算
  - `KillEnemy` で kill / boss kill カウント
  - `OpenUpgrade` / `OpenEvolution` のonClick内で `RecordModulePick` 呼び出し
- **リザルト画面 豪華化** (`CreateResultPanel` フルリビルド + `ShowResult` 拡張)
  - デッキサイズ 720x440 → 960x580 に拡大
  - 左カラム: キャラポートレート (preserveAspect付き) + 進化ルートバッジ + 融合バッジ
  - 中央カラム: 3x2 ステータスグリッド (WAVE / TIME / KILLS / DATA / DAMAGE / BOSS)
  - 右カラム: MVP BUILD (取得モジュール上位3つをカード形式、アクセント色＋アイコン)
  - 下部: 1行サマリと既存ボタン2つ
  - 新ヘルパー: `CreateBadgeStrip`, `CreateStatTile`, `CreateMvpRow`, `SetResultStat`, `BuildMvpList`, `BuildMvpEntry`, `GetRouteAccentColor`, `GetModuleTitleAccent`, `GetModuleTitleIcon`
  - 新nested class: `MvpEntry`
- **図鑑 ビジュアル化** (`CreateCodexPanel` フルリビルド)
  - レイアウト: タイトル → BEST/CLEARS表示 → 3セクション (PARTNER / EVOLUTION ROUTE / CROSS EVOLVE) × 各3カード
  - 各カードに左側 64px サークル背景 + シルエット (ロック中) / アクセント色アイコン (解放後)
  - ロック中はカード全体をディム、解放後はアクセントOutline強化
  - 旧 `BuildCodexText` / `CodexLine` は dead code として残置（参考用）
  - 新ヘルパー: `CreateCodexSection`, `CreateCodexCard`, `GetCodexIconSprite`, `RefreshCodexCards`, `GetCodexDescriptionFor`
  - 新nested class: `CodexEntry`
  - `ToggleCodexPanel` を `RefreshCodexCards()` 呼び出しに切り替え
- **カード選択演出 強化** (`AnimateCardButtons` 拡張)
  - 出現時のEase-out アニメ (0.78→1.0 scale, per-card 80ms stagger)
  - 微小な浮遊感 (sin bob, 出現完了後に有効)
  - 新フィールド: `cardAppearStartTime`, `cardBasePositions`
  - `OpenUpgrade` / `OpenEvolution` / `OpenPartnerSelect` で開始時刻リセット
- **キャラ/敵の見た目 強化**
  - 敵スポーン時に色付きスパーク (Brute は12粒、その他は6粒)
  - ボス出現時に追加でリングスパーク x2 + 中央スパーク x48
- **日本語フォント フォールバック強化** (`GetUiFont`)
  - 候補拡張: Yu Gothic UI/Yu Gothic/Meiryo UI/Meiryo/BIZ UDGothic/BIZ UDPGothic/Noto Sans CJK JP/Noto Sans JP/Hiragino Sans/Hiragino Kaku Gothic ProN/MS Gothic/MS UI Gothic/Arial Unicode MS/Arial

### 今回 (Codex セッション、2026-05-18)
- **BEAST LINK の方針変更**
  - ユーザー要望: 最初の BEAST LINK は性能だけではなく見た目も変えたい
  - 採用方針: `BEAST LINK = 種族/素体`, `SPEED/POWER/GUARD = 進化方向`, `Lv3/6/9 = 成長段階`, `CROSS EVOLVE = メインキャラがリンクを吸収する専用フォーム`
  - クロス進化は「リンク同士の合体」ではなく「メインキャラがリンク装備/コアを取り込んで変身」する見せ方を優先
- **partnerStyle を追加**
  - 新フィールド: `partnerStyle`
  - `PartnerOption` に `style` を追加
  - `OpenPartnerSelect` の選択時に `partnerStyle = partner.style` をセットし、即 `ApplyPlayerVisuals(true)` で素体見た目を反映
- **相棒ごとのプロシージャル素体を追加**
  - 新メソッド: `MakePartnerVariantSprite(stage, routeStyle, fusedStyle, species)`
  - 新ヘルパー: `GetPartnerBaseColor`, `GetPartnerDeepColor`, `GetPartnerAccentColor`, `GetPartnerArmorColor`, `GetFusionAccentColor`
  - 6種の見た目差:
    - `Cobalt Pup`: 青系の犬型バランス素体
    - `Spark Kit`: オレンジ系のキツネ/弾幕素体、長耳とツインテール
    - `Guard Mole`: 緑系の低重心防衛素体、爪と装甲
    - `Tech Owl`: 青紫系のフクロウ素体、翼と visor
    - `Drift Fox`: 紫系の細身回避素体、長い尾と位相リング
    - `Echo Cat`: ピンク系の猫/連鎖素体、猫耳と波形リング
- **BEAST LINK カードの見た目プレビュー**
  - 新メソッド: `SetPartnerCardPreview`
  - カード内 `Card Icon` に `MakePartnerVariantSprite(0,0,0,style)` を表示
  - 選択前から「性能だけでなく素体が違う」ことが分かるようにした
- **CROSS EVOLVE 文言更新**
  - `ActivateFusion` のカットシーン説明を `メインキャラがリンクを取り込み、専用フォームへ` に変更
  - `ApplyPlayerVisuals` は融合時も `partnerStyle + formStyle + fusionStyle` でスプライト生成するため、素体を保ったまま融合装備が乗る
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex ブラッシュアップ、2026-05-18)
- **EXP / データチップ表示の修正**
  - `CreateHudBar` で `EXP` にも数値表示を追加
  - `UpdateUi` で EXP fill 色と数値を明示更新
  - `CollectPickupAt` 後に `UpdateUi()` を即呼び、拾った直後の HUD 反映を早めた
  - 小型の `Chip Mini Panel` を通常 HUD に追加し、データチップ所持数を常時表示
  - リロールボタンを `リロール 30   所持 X` 表示に変更
- **初期相棒 / クロス進化の見た目強化**
  - `MakePartnerVariantSprite` の素体シルエットを強化
  - 種族ごとの差分を拡大: 犬耳、長耳、低重心装甲、翼/visor、長い尾、波形リングなど
  - クロス進化装備を大型化: Nova砲、Bulwark盾、Siphonオーブ、Phaseリング、融合クレスト/コア
  - `ApplyPlayerVisuals` の融合スケールとオーラを強化
  - `UpdatePlayerEvolutionDecor` の融合時アクセントを大型化
- **進化 / 融合カットイン強化**
  - `evolutionCutsceneDuration` を 1.45 秒に延長
  - 中央ビーム、巨大リング、230pxポートレートを追加/拡大
  - `EVOLUTION` / `CROSS EVOLVE` 発生時に「変身した」ことが見える演出へ寄せた
- **図鑑更新**
  - PARTNER を6体すべて表示: Cobalt Pup / Spark Kit / Guard Mole / Tech Owl / Drift Fox / Echo Cat
  - CROSS EVOLVE を6種すべて表示: Nova Aegis / Photon Siphon / Core Bastion / Nova Phantom / Aegis Drift / Photon Wraith
  - 図鑑アイコンを丸/菱形から `MakePartnerVariantSprite` ベースの見た目プレビューへ変更
  - 旧 `BuildCodexText` の内容も6体/6融合へ追従
- **実装漏れ確認**
  - 実装済み: 6相棒、6クロス進化、Phaseリンク、3段階モジュール、強化カードLv表示、メインメニュー全面化、図鑑、リザルト、オプション、HUD整理、進化/融合カットイン、相棒ごとの初期見た目、通常HUDチップ表示
  - 未完/次候補: 商用品質の生成PNG/手描きスプライト差し替え、BGM/SE本素材、オプション保存、メタ進行、図鑑のスクロール/タブ化、Unity Editorでの実機目視調整
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 長期目標/ツリー追加、2026-05-18)
- **MISSION BOARD を追加**
  - メインメニューに `MISSION` ボタンを追加
  - 新パネル: `CreateMissionBoardPanel`
  - 8ミッションを実装:
    - 初回防衛成功
    - SPEED適性試験
    - POWER火力試験
    - GUARD防衛試験
    - 初クロス進化
    - データ収集班
    - コアキーパー
    - ハンターログ
  - 保存キー: `Mission_<id>`
  - `SaveProgress` で `EvaluateMissions(cleared)` を呼び、ラン終了時に達成判定
  - リザルトに新規達成ミッションを `MISSION CLEAR: ...` と表示
  - `LoadProgress` の `collectionSummary` に `Missions X / 8` を追加
- **進化ツリー画面を追加**
  - メインメニューに `進化ツリー` ボタンを追加
  - 新パネル: `CreateEvolutionTreePanel`
  - BEAST LINK 6素体、SPEED/POWER/GUARD の Lv0/Lv3/Lv6/Lv9 プレビュー、6クロス進化プレビューを表示
  - クロス進化は未発見なら `???` と暗色表示、発見済みはカラー表示
- **メニュー導線整理**
  - `MISSION`, `進化ツリー`, `図鑑`, `OPTIONS` が互いに重ならないよう、開く時に他パネルを閉じる
  - `StartRun` 時にも全メニュー系パネルを閉じる
- **次に着手しやすい候補**
  - ミッション報酬を実効果にする: スタートボーナス、初期データチップ、カード候補増加など
  - ステージ/難易度選択画面を追加
  - オプション永続化
  - パッド対応
  - カードレアリティ演出/SE差分
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 画像生成・差し替え、2026-05-18)
- **built-in imagegen で商用安全なオリジナル素材を生成**
  - 参照/原本:
    - `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_08ffdbef442aa774016a0b008ef8c0819186fd1607fece2a83.png` (キャラ/敵/コア素材)
    - `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d2eff41d8823c73016a0b04197d208191b362751206c54805.png` (床/UI/VFX素材)
  - プロジェクト内ソース:
    - `Assets/ArtSource/Generated_StyleSheet.png`
    - `Assets/ArtSource/Generated_Visual_Sheet.png`
    - `Assets/ArtSource/Generated_Visual_Sheet_Alpha.png`
    - `Assets/ArtSource/Generated_Visual_Asset_Preview.png`
- **キャラ/敵/リンク/融合フォームのPNG差し替え**
  - `Assets/Resources/Skins/` に `Partner_S*_L0`, `Route_*_Stage*`, `Fusion_*`, `Runner`, `Brute`, `Shooter`, `Boss`, `Lantern`, `Link_*` などを配置
  - `LoadOptionalSprite` を `Texture2D` fallback対応にし、UnityのSprite importer設定が未設定でも `Resources.Load<Texture2D>` から `Sprite.Create` で読めるようにした
  - `GetPartnerVariantSprite(stage, routeStyle, fusedStyle, species)` を追加し、素体/進化ルート/融合フォームを画像優先で解決
- **床/UI/VFXの画像差し替え**
  - 追加素材: `Floor_TileA/B/C`, `Card_Cyan/Gold/Green/Magenta/Red`, `Bullet_Speed/Power/Guard/Fusion`, `Pickup_Data`, `Evolution_Ring`, `Badge_*`, `Impact_*`, `Boss_Warning` など31点
  - `CreateBackground` で生成床タイルを敷き込み、暗いサイバー床の密度を上げた
  - `SpawnBullet`, Nova追撃, Mirage Bolt を生成弾スプライト優先に変更
  - `SpawnPickup` を `Pickup_Data` のデータチップ見た目へ変更
  - `ApplyCardVisual` で強化/進化/クロスカードに生成カードフレームを適用
  - 進化カットインのリングを `Evolution_Ring` に差し替え
  - LINKスロットのアイコンも丸/四角ではなく `Link_*` スプライト表示に変更
- **注意**
  - `Panel_FrameCyan/Gold`, `Badge_*`, `Impact_*`, `Boss_Warning`, `Heal_Orb`, `Heart_Orb`, `Pickup_Core/Bonus` は素材配置済みだが、まだ全箇所に接続していない。次のブラッシュアップ候補
  - AI生成のため一部クロップ/色味はUnity実機で目視して再調整推奨
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Claude Code クリーンアップ、2026-05-18)
- **クロス進化を「メインキャラがリンクを取り込んで変身」に統一**
  - `UpdateLinkCompanion`: 融合発動後はリンクコンパニオン (Nova/Bulwark/Siphon/Phaseの小型サイドキック) を非表示に → メインキャラ単体に
  - `ActivateFusion`: 融合発動前のリンク位置を記録し、各位置からプレイヤー位置へ吸収スパーク帯を展開
- **生成PNGの不良スプライトを無効化** (リネームで `.disabled` 付け、`Resources.Load` がフォールバックを使うように)
  - 「子キャラ+大型メカ獣」の融合構図が誤ってRoute/Partnerの Stage3 に割り当てられていた問題:
    - `Route_1_Stage3.png` / `Route_2_Stage3.png` / `Route_3_Stage3.png`
    - `Partner_S1_R1_L3.png` / `Partner_S2_R2_L3.png` / `Partner_S3_R3_L3.png`
    - `Player_Form4.png`
  - 他キャラ素体(緑クリスタル)が下端に映り込んでいた問題:
    - `Route_2_Stage1.png` / `Route_2_Stage2.png`
    - `Partner_S2_R2_L1.png` / `Partner_S2_R2_L2.png`
  - 無効化後はprocedural生成 (`MakePartnerVariantSprite`) または下段ステージのPNGがフォールバック
- **相棒名を画像の見た目に揃えてリネーム** (`BuildPartnerPool` + `CodexEntry` + `GetCodexDescriptionFor` + `BuildCodexText`):
  - `Spark Kit` → `Ember Drake` (S2 橙竜、弾幕型継続)
  - `Guard Mole` → `Sage Hare` (S3 緑兎、防衛型継続)
  - `Tech Owl` → `Hex Cat` (S4 紫猫、連鎖型に変更、`ハッカー型`は廃止)
  - `Echo Cat` → `Iron Bear` (S6 黒熊、装甲型に新規。連鎖型はHex Catへ)
  - PlayerPrefsキー (`Partner_<name>`) も新名に揃えたので、過去の解放記録はリセットされる (alpha段階の許容範囲)
- **コンパイル確認**
  - ブレースバランス OK (671/671)、6,517 → 7,563 行

### 今回 (Codex 進化分岐ブラッシュアップ、2026-05-19)
- **進化後に素体画像へ戻る問題を修正**
  - `GetPartnerVariantSprite(stage, routeStyle, fusedStyle, species, variantStyle)` に拡張
  - `stage > 0` で専用PNGが見つからない場合、`Partner_S*_L0` へ戻らず procedural 進化スプライトを生成するよう変更
  - PNG命名の将来拡張:
    - `Partner_S{species}_R{route}_V{variant}_L{stage}.png`
    - `Route_{route}_V{variant}_Stage{stage}.png`
    - `Partner_S{species}_F{fusion}_V{variant}_L{stage}.png`
- **進化カード選択を見た目に反映**
  - 新フィールド: `evolutionVariantStyle`
  - `Evolve(..., visualVariant)` を追加し、進化カードごとにvariantを保存
  - SPEED:
    - `1`: 連射翼 → 大きな左右ウィング
    - `2`: 貫通レーザー → レーザーフィン/砲身
    - `3`: ミラージュ → 残像リング
    - `4`: ホーミングビット → 周囲の小型ポッド
  - POWER:
    - `1`: 爆裂コア → 大型発光コア
    - `2`: 巨弾アーム → 巨腕
    - `3`: ボスブレイカー → 大型角
    - `4`: シージモード → 重装プレート
  - GUARD:
    - `1`: コアシールド → 大型シールドリング
    - `2`: 反撃装甲 → 反撃スパイク
    - `3`: リカバリーコア → 回復ハロー/十字
    - `4`: 鏡盾フォーム → 左右ミラーシールド
- **図鑑/進化ツリーの表示更新**
  - 進化ツリーの相棒短縮名を新名称に追従: `Cobalt / Ember / Sage / Hex / Drift / Iron`
  - 進化ツリーのルートプレビューでもvariant付きスプライトを使うよう更新
  - 図鑑のRouteアイコンをBASEではなく最終進化プレビューに変更
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex UIレイアウト調整、2026-05-19)
- **図鑑の詰まりを修正**
  - `CreateCodexPanel` のデッキを拡大し、タイトル/記録/セクション位置を再調整
  - PARTNER / EVOLUTION ROUTE / CROSS EVOLVE のカード幅・行間・見出し線を調整
  - カード内タイトル/OPEN表示/説明文のY位置と自動縮小を調整し、名前が上枠へ乗りにくくした
  - `CLOSE` ボタンを下げつつ小型化し、クロス進化カードとの重なりを解消
- **進化ツリーの下端クリップを修正**
  - `CreateEvolutionTreePanel` のデッキを拡大し、ツリー全体の横余白を増やした
  - BEAST LINK strip / 3ルートパネル / CROSS EVOLVE panel の配置を再調整
  - `CLOSE` ボタンを下げつつ小型化し、クロス進化プレビューとの重なりを解消
- **オプション画面を中央配置へ変更**
  - `Options Panel` を右寄せから中央配置に変更し、専用画面として読みやすくした
  - ボタン幅/間隔を拡大し、下部に `CLOSE` ボタンを追加
  - 開いた時に `SetAsLastSibling()` で最前面へ出すようにした
- **ポーズ中HUDの情報過多を整理**
  - `HUD 詳細: OFF` のとき、ポーズ中でも `ACTIVE BUILD` / `LINKS` 詳細パネルを出さないよう変更
  - 重複していた大きい `Data Panel` は非表示固定にし、小型 `Chip Mini Panel` のみで所持データチップを表示
  - Link slot の `BULWARK` / `SIPHON` 表示を `BULW` / `SIPH` に短縮し、文字折り返しを抑制
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 進化ツリー見出し/プレビュー修正、2026-05-19)
- **進化ツリーの見出しはみ出しを修正**
  - `BEAST LINK 素体` と `CROSS EVOLVE` の見出し座標を内側へ戻し、パネル外へ突き抜けないよう調整
  - 原因: `CreateText` は `TextAnchor.MiddleLeft` の場合も中央pivotになるため、テキスト矩形の半分が左へ飛び出していた
- **進化プレビューの視認性を改善**
  - ルート内の `BASE / Lv3 / Lv6 / Lv9` 小カードを少し拡大
  - `CreateTreeIconCard` のアイコンサイズ計算を見直し、進化絵が小さくなりすぎないよう変更
  - 進化ツリー上のルートプレビューは `GetEvolutionTreePreviewSprite` 経由で procedural 進化スプライトを優先
  - 理由: 現状の一部PNGは段階差・素体差が揃っておらず、ツリー上で「同じキャラの成長」に見えにくいため
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex メタ進行/オプション保存、2026-05-19)
- **オプション永続化を実装**
  - 新規キーprefix: `CoreLantern_Option_`
  - `LoadOptions` / `SaveOptions` を追加
  - 保存対象: HUD詳細、イベントログ、操作表示、ミニマップ、強化ビジュアル、画面揺れ、画面フラッシュ、BGM、SE
  - オプションボタン押下時に `PlayerPrefs` へ保存し、次回起動/リトライ後も設定を維持
- **ミッション報酬を実効果化**
  - `ApplyMetaProgressionRewards` を追加し、ラン開始時に解放済みミッション報酬を適用
  - 報酬内容:
    - `first_clear`: 初期データチップ +10
    - `speed_clear`: 移動 +3%、連射 +3%
    - `power_clear`: 攻撃力 +0.08
    - `guard_clear`: コアHP +2
    - `first_fusion`: 回収範囲 +5%
    - `data_120`: リロール費用 30 → 25
    - `core_keeper`: 自分HP +1
    - `hunter_180`: バースト率 +2%
  - `BuildMissionDefinitions` の報酬文言を実効果に合わせて更新
  - メインメニューのミッション欄に `解放済み報酬` の短縮サマリを表示
  - リロールボタン表示/消費を `GetRerollCost()` 経由に変更
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 進化グラフィック統一、2026-05-19)
- **全素体の進化表示を統一**
  - `GetPartnerVariantSprite` の解決順を変更
  - `stage > 0` かつ `routeStyle/fusedStyle` がある場合、古い汎用 `Route_*_Stage*` や一部だけ存在する旧PNGへ落とさず、基本は `MakePartnerVariantSprite` の統一生成進化を使う
  - 将来の高品質差し替えは `Partner_S{species}_R{route}_V{variant}_L{stage}.png` / `Partner_S{species}_F{fusion}_V{variant}_L{stage}.png` を置けば優先される
  - 目的: 「素体はあるのに進化がない」「相棒ごとに進化後の絵柄がバラつく」問題を抑える
- **種族ごとの生成見た目を現名称に追従**
  - `Hex Cat` (species=4): 旧Tech Owl系の青/翼/visor寄りから、紫魔猫・猫耳・波形/ウィスプ系へ寄せた
  - `Drift Fox` (species=5): 桃狐の色味へ更新
  - `Iron Bear` (species=6): 旧Echo Cat系のピンク猫寄りから、黒熊・重装・爪/装甲プレート系へ変更
  - `GetPartnerBaseColor` / `GetPartnerDeepColor` / `GetPartnerAccentColor` / `GetPartnerArmorColor` を新名称に合わせて更新
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 進化素材復旧、2026-05-19)
- **「全然ダメ」だった進化ツリーの丸いコード絵を撤回**
  - ユーザー確認で、プロシージャル統一絵は進化の魅力と種族差を潰していると判断
  - `GetPartnerVariantSprite` を再調整し、進化/融合は高品質PNG (`Route_*_Stage*`, `Fusion_*`) を優先、存在しない時だけ `MakePartnerVariantSprite` に落とすよう変更
  - `GetEvolutionTreePreviewSprite` も `GetPartnerVariantSprite` 経由に戻し、進化ツリーが既存の高品質素材を使うよう修正
- **無効化されていた良い素材を復旧**
  - 復旧: `Route_1_Stage3.png`, `Route_2_Stage3.png`, `Route_3_Stage3.png`, `Partner_S5_L0.png`
  - 汚れを除去して復旧: `Route_2_Stage1.png`, `Route_2_Stage2.png`, `Fusion_2.png`
  - 確認用プレビュー: `Assets/ArtSource/Generated_Route_Fix_Preview.png`
- **今後の注意**
  - 現状は「高品質汎用ルート素材」を優先しているため、全6素体それぞれの専用進化ラインはまだ未完成
  - 次の本命は `Partner_S{species}_R{route}_V{variant}_L{stage}.png` を生成/配置し、素体ごとに進化シルエットを作ること
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex Drift Fox/ツリー見出し修正、2026-05-19)
- **Drift Fox が鳥に見える問題を応急修正**
  - `Partner_S5_L0.png` は羽根が強く、`Drift Fox` の名前と噛み合っていなかった
  - 羽根部分を除去し、狐らしい尻尾を追加した差し替えPNGに更新
  - 注意: これは暫定修正。最終的には `Drift Fox` 専用の高品質進化ラインを新規生成する方がよい
- **進化ツリー左上見出しの埋まりを修正**
  - `BEAST LINK 素体` の位置をカード上端から上へ逃がし、pivotを左寄せに変更
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 全相棒進化ツリー切替、2026-05-19)
- **Hex / Drift / Iron が進化しないように見える問題を修正**
  - 実ゲームでは全6相棒が進化できるが、ツリーが代表3体だけを表示していたため誤解を生んでいた
  - 進化ツリー上部の相棒カードを選択可能にし、下の `SPEED / POWER / GUARD` プレビューが選択中の相棒へ切り替わるよう変更
  - 選択中表示: `選択中: <相棒>    SPEED / POWER / GUARD すべて進化可能`
- **全6相棒 x 3ルート x Lv3/Lv6/Lv9 の暫定PNGを生成**
  - 追加命名: `Partner_S{species}_R{route}_L{stage}.png`
  - 専用生成PNGがない組み合わせも、素体シルエットを保ったままルート別エフェクトを重ねて表示する
  - 確認用プレビュー: `Assets/ArtSource/Generated_AllSpecies_Evolution_Preview.png`
  - 注意: これはUI/理解用の暫定素材。商用品質では各相棒ごとの完全専用進化デザインを生成し直すのが本命
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex Drift Fox 作り直し、2026-05-19)
- **Drift Fox の鳥要素を完全撤去**
  - ユーザー指摘: 羽は消えたが、嘴とトサカがあり鳥にしか見えない
  - `Partner_S5_L0.png` を既存の高品質な哺乳類素体ベースで桃狐カラーへ再加工し、嘴/トサカ/羽のない狐系シルエットに差し替え
  - `Partner_S5_R1_L1` 〜 `Partner_S5_R3_L3` も同じ狐素体ベース + ルート別エフェクトへ再生成
  - SPEED最終などに残っていた羽根系パーツも使わないようにした
  - 確認用プレビュー: `Assets/ArtSource/Generated_DriftFox_Redo_Preview.png`
  - 全体確認用プレビュー: `Assets/ArtSource/Generated_AllSpecies_Evolution_Preview.png`
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

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

### 今回 (Claude Code 大規模ブラッシュアップ、2026-05-19)

#### 音響システム（プロシージャル合成）
- **BGM を起動時に自動生成** (`MakeBgmClip`)
  - 120 BPM / Aマイナーペンタトニック / 8秒シームレスループ
  - 4レイヤー: キック（周波数ドロップ）+ ハイハット + ベース（三角波）+ メロディー（矩形波）+ パッドコード
- **SE 9種をプロシージャル合成** (`MakeSeClip`)
  - Shoot/Hit/Kill/Pickup/LevelUp/Evolve/Fusion/Boss/GameOver
  - `GenerateProceduralAudio()` が LoadAudioClip 後に呼ばれ、未ロードキーのみ生成
  - `Resources/Audio/` に同名ファイルを置けば自動優先

#### ゲームフィール改善
- **移動トレイル**: `trailTimer`（0.038s 間隔）で、動き中にプレイヤー位置へ小スパーク生成
- **ヒットスパーク強化**: 2粒→5粒、力0.32→0.5、敵タイプ別カラー（Runner:赤/Brute:紫/Shooter:黄/Boss:ピンク）
- **Brute 撃破エフェクト**: Flash + Shake + 紫スパーク 20粒追加
- **スパーク改善**: maxLife=life で全スパークが最大輝度から uniform に減衰、sqrt フェードカーブ
- **ウェーブ開始リング**: `BeginWave` で外周から収縮する光輪（通常シアン 24粒 / ボスウェーブ マゼンタ 36粒）
- **真空吸引サージ**: レベルアップ時 1.8 秒間全ピックアップが 32 unit/sec で吸引。VSの「真空」に相当
- **Eggcore 損傷ビジュアル**: HP 割合に応じてグローが 金→オレンジ→赤 へ変色、パルス速度も変化

#### プロシージャルスプライト改善
- **Runner**: 両サイドに速度ストリーク + 胴体グラデーション
- **Brute**: 内側にグローリング + X字装甲ライン + グラデーション
- **Shooter**: 両側に砲台ノブ + コアグラデーション
- **Boss**: 中央にダイヤモンドシジル + 翼に静脈ライン + グラデーション
- **Eggcore**: 同心リング + 十字 + 中央ダイヤモンドコア + グラデーション
- **背景タイル**: 150個の床タイルの 22% をダイヤ形、38% を円形に変更

#### バランス調整
- `fireRate` 2.6 → 2.8（序盤テンポ改善）
- Phase dodge クールダウン 1.4s → 1.1s（回避リンクの実用性向上）
- Boss HP: `46 + level*3.2` → `40 + level*3.5`（初回易化）
- **パワーカーブ急峻化** (Wave 7+)
  - 敵数: `+14*(wave-6)` の追加
  - スポーン間隔: `Mathf.Max(0.15f, ...)` に短縮
  - HP 倍率: Wave 7=×1.1 / Wave 8=×1.25 / Wave 9=×1.45
  - 速度倍率: Wave 7=×1.06 / Wave 8+=×1.12

#### 競合分析から実装した機能
分析対象: Vampire Survivors / Brotato / Halls of Torment / 20 Minutes Till Dawn / Dome Keeper
- **Wave 予告テキスト** (Brotato 参考): モジュール選択パネルのタイトルが「次 Wave N: Shooter急増」等を表示
- **ボス後スペシャルモジュール** (HoT ドロップ参考): Wave 10 ボス撃破後、高レアモジュールを 2 択から 1 つ追加選択
- **ラン後スタッツ強化**: BOSS タイルを「TOP W」（最多撃破 Wave）に変更。結果画面下部に「コア残体力 / BOSS撃破 / 最多撃破Wave」1行サマリを追加
- **ダメージ数字サイズ拡大**: 0.095→0.105、ボス 0.12→0.13

#### レリックシステム（新規）
Wave 3・Wave 6 クリア時に 2 択から 1 つ選択するパッシブアイテム。
- `relicSet` (HashSet<string>) で管理
- `choosingRelic` フラグで UpdateWave の二重起動を防止
- **レリック 6 種**:
  - `磁力核`: pickupRange += 2.2
  - `不滅の証`: deathShieldActive = true（1回死亡回避）
  - `コアの鼓動`: 5秒ごとに Eggcore 周囲 3.2 に 2.5 ダメージ AoE
  - `爆炎の紋章`: explodeChance を 0.52 以上に設定
  - `嵐の心臓`: fireRate ×1.35 + bulletCount +1
  - `鏡の誓約`: reflectShield = true
- **死亡シールド発動演出**: Flash + Shake + 金リング + 「DEATHLESS」テキスト

#### UI 改善
- **CanvasScaler を Expand モードに変更**: ウルトラワイド等で図鑑/ツリーの CLOSE ボタンが押せなかった問題を根本解決
- **タイトル画面の ESC 対応**: 図鑑/ツリー/ミッション/オプションを ESC で閉じられるように
- **レリック表示 UI**: ステータスパネルを 198→228px に拡張、下部に取得済みレリック名を琥珀色で常時表示（◆ 磁力核  ◆ 不滅の証）
- **ESC フィードバック**: モジュール/レリック選択中に ESC を押すとパネルタイトルが赤くなり「選択必須」を示す
- **PNG 無効化 (追加)**:
  - `Partner_S5_L0.png`: Drift Fox に鳥が出ていたため disabled → procedural fallback
  - `Fusion_2.png`: クロップ失敗のため disabled → procedural fallback
- **図鑑の Route アイコン修正**: stage=3 → stage=0 で各ルートが固有アイコンを表示

#### 新フィールド（重要なもの）
| フィールド | 型 | 用途 |
|---|---|---|
| `relicSet` | HashSet<string> | 取得済みレリック管理 |
| `choosingRelic` | bool | レリック選択中フラグ |
| `deathShieldActive` | bool | 不滅の証発動フラグ |
| `corePulseRelicActive` | bool | コアの鼓動レリック |
| `corePulseTimer` | float | コアパルスの間隔タイマー |
| `bossModuleReady` | bool | ボス後モジュール待機フラグ |
| `vacuumSurgeTimer` | float | LVアップ後の真空サージ残時間 |
| `runMaxKillWave` | int | 最多撃破Waveトラッキング |
| `runMaxWaveKills` | int | その最多撃破数 |
| `trailTimer` | float | 移動トレイルのインターバル |
| `relicDisplayText` | Text | ステータスパネルのレリック表示 |

#### 新クラス
- `RelicData`: id / title / description / apply を持つレリック定義クラス（Upgrade クラスの直前に定義）

#### 新メソッド（主要なもの）
- `BuildRelicPool()` / `OpenRelicSelect()` / `GetWaveHint(int)` / `MakeBgmClip()` / `MakeSeClip(string)` / `GenerateProceduralAudio()` / `PcmSin/PcmTri/PcmSq/PcmNoise`

### 今回 (Codex モーション改善、2026-05-19)
- **Claude 追加分の確認**
  - プロシージャル BGM/SE、レリック、ボス後スペシャルモジュール、真空吸引サージ、移動トレイル、Wave予告、CanvasScaler Expand などが `CoreLanternGame.cs` に実装済みであることを確認
- **1枚絵が滑っているように見える問題への短期対応**
  - プレイヤーと敵の本体スプライトをルート直描画から子オブジェクト `Player Body` / `<Enemy> Body` へ分離
  - 当たり判定・移動・HPバーは親Transformのまま、本体絵だけに移動方向のflip、微小な傾き、上下ボブ、潰れ/伸びを付与
  - 敵のフェード/Phantom透過はルートSpriteRendererではなく本体SpriteRendererへ適用するよう変更
- **今後の本命**
  - 今回はコードだけでの応急改善。本質的には相棒/敵ごとに `idle / walk / attack / hit` の数フレームPNGを生成して差し替えると、商用品質の「生きている感」に近づく
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 追加モーション演出、2026-05-19)
- **移動の生物感を追加**
  - `SpriteGhost` を追加し、プレイヤー/高速敵/ボスに薄い残像を出せるようにした
  - プレイヤー移動中は足元に小さな stride spark を発生
  - Runner/Brute/Bomber/Boss も移動に合わせて接地スパークを出し、滑って見える印象を軽減
- **射撃の手応えを追加**
  - プレイヤー射撃時に `playerAttackPulse` による反動、マズルフラッシュ、前方スパークを追加
  - Shooter/Boss の敵弾発射時にも `attackPulse` と発射フラッシュを追加
  - Dasher の突進開始時にも軽い予備動作を追加
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex カード可読性調整、2026-05-19)
- **強化カードの文字整理**
  - カード本文から `★★☆` などのレアリティ行を分離し、下部の専用 `Card Rarity Text` / `Card Rarity Plate` に表示
  - 上部 `Card Badge` は種別とLvだけを表示する方針にし、`CROSS RARE` は `CROSS`、`LINK MODULE` は `LINK` に短縮
  - `Card Header Plate` の背景を濃くして、緑/金など明るいカードフレーム上でも読めるようにした
  - 本文のフォントサイズと表示領域を調整し、カードごとの文字サイズ差を抑制
- **選択パネル上部の情報整理**
  - `panelSubtitleText` を追加
  - タイトルは「進化モジュールを選ぶ」などの主題だけにし、`次 Wave ...` や補足情報は小さなサブタイトルへ移動
- **レアリティ方針**
  - レアリティはビルド判断の補助になるため残す。ただし本文内ではなく、独立した下部バッジとして控えめに表示
- **コンパイル確認**
  - `CoreLanternGame.cs` の Roslyn コンパイルチェック通過
  - 残警告: `FindObjectOfType<T>()` と `allyName` 未使用のみ

### 今回 (Codex 画像素材台帳、2026-05-19)
- **画像制作管理ファイルを追加**
  - `IMAGE_ASSET_BACKLOG.md` を作成
  - 現在の `Assets/Resources/Skins/` 在庫をカテゴリ別に整理
  - コードが読む命名規則、相棒/ルート/融合番号、作成優先順を明文化
- **優先作成対象**
  - ルートL0不足分 15枚: `Partner_S{species}_R{route}_L0.png`
  - 種族別クロス進化L3 36枚: `Partner_S{1-6}_F{1-6}_L3.png`
  - 既存ルート進化54枚の品質レビュー
  - Dasher/Bomber/Phantom の外部PNG化はコードHOOKが必要
- **方針**
  - まず静止画の統一感を作り、その後 `idle / move / attack / hit` のフレームアニメへ進む

### 今回 (Codex HUD/MAP素材台帳追記、2026-05-19)
- **MAP/HUDの必要画像も台帳へ統合**
  - `IMAGE_ASSET_BACKLOG.md` に `P1: HUD・MAP専用素材` を追加
  - HUD常時表示、戦闘進行、ミニマップ、ワールド/マップ、サイズ目安を分けて整理
  - 既存の `Floor_TileA/B/C` と `Pickup_Data` は `DONE`、未ロードのパネル/バー/ミニマップ/小物は `HOOK` として管理
- **生成バッチ順を更新**
  - `Batch 4: HUD可読性パック`
  - `Batch 5: MAP密度パック`
  - 既存の敵外部化/アニメーション/背景系は後ろへ移動
- **次の実装候補**
  - `CreatePanel` / `CreateHudBar` / `CreateMinimap` / `CreateWorld` に画像差し替え用の `LoadOptionalSprite` 対応を入れる
  - HP/EXP/ミニマップ、床密度、コア台座から優先すると画面全体の完成度が上がる

### 今回 (Codex MAP素材生成/実装、2026-05-19)
- **マップ素材を先行生成**
  - 生成スクリプト: `Tools/GenerateMapAssets.ps1`
  - 出力先: `Assets/Resources/Skins/`
  - 既存 `Floor_TileA/B/C` は上書きし、初回上書き前の画像は `Assets/ArtSource/MapAssetBackup_20260519/` に退避
  - プレビュー: `Assets/ArtSource/Generated_MapAsset_Preview.png`
- **生成した主な素材**
  - 床: `Floor_TileA/B/C`, `Floor_DarkBase_A`, `Floor_GridOverlay_A`, `Floor_Crack_A/B`, `Floor_Cable_A/B`, `Floor_CoreMark_A`
  - 外周/小物: `Boundary_Stone_A/B`, `Prop_ServerDebris_A/B`, `Prop_NeonPylon_A`, `Prop_DataTerminal_A`, `Prop_Crate_A`
  - コア/演出: `Core_Platform`, `Core_RingOuter`, `Core_RingInner`, `Core_DamageCrack_1/2/3`, `Spawn_Ring_Normal`, `Spawn_Ring_Boss`
- **コード側フック**
  - `CreateAssets` で上記マップ素材を `LoadOptionalSprite` 対応
  - `CreateBackground` で床ベース、グリッド、ひび/配線、外周石、サーバー破片、小物、コア台座/リングを配置
  - `BeginWave` のWaveリングに `Spawn_Ring_Normal/Boss` を使用
  - コアHP割合に応じて `Core_DamageCrack_1/2/3` を表示
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete と `allyName` 未使用のみ
  - Unity Editor は同プロジェクトが既に開いていたため batchmode 起動確認はスキップ

### 今回 (Codex カード文字階層の再整理、2026-05-19)
- **Claude 変更分の確認**
  - プロシージャルBGM/SE、レリック、真空吸引、モーション演出、カード可読性調整、MAP生成/フックが `CoreLanternGame.cs` / `Tools/GenerateMapAssets.ps1` に入っていることを確認
  - Git管理外フォルダのため `git diff` ではなく、HANDOFFと現行コードベースで確認
- **「何のモジュールかわからない」問題を修正**
  - カード内に `Card Title Plate` / `Card Title Text` / `Card Level Text` を追加
  - 既存 `Label` は説明文専用に変更し、カードの情報階層を `上:種別` / `中央:モジュール名` / `下:Lv・説明・レアリティ` に分離
  - `SetCardText` を追加し、強化・進化・レリック・相棒選択カードすべてでタイトル/レベル/説明を明示表示
  - `Card Header Plate` のアニメ中アルファを上げ、上部バッジがカード背景に沈まないよう調整
- **マップ**
  - ユーザーより Claude 側で解決済みとの連絡があったため、今回の Codex 作業では追加変更なし
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete、`allyName` 未使用、`bossModuleReady` 未割当
  - `bossModuleReady` はコード内コメントで「Final boss reward module removed」とされており、ボス後スペシャルモジュールは現在無効化状態の可能性あり

### 今回 (Codex 画像生成タスク Batch 1/2、2026-05-20)
- **生成スクリプト追加**
  - `Tools/GenerateMissingVariantAssets.ps1` を追加
  - 既存の相棒/ルートPNGをベースに、ルート別L0と種族別クロス進化L3を生成するプロジェクト内バッチ
  - `-Force` 指定で再生成可能。確認用プレビューも同時に生成する
- **Batch 1: ルートL0**
  - `Partner_S{1-6}_R{1-3}_L0.png` 全18枚を生成/再生成
  - 以前不足していた15枚は解消済み
- **Batch 2: 種族別クロス進化L3**
  - `Partner_S{1-6}_F{1-6}_L3.png` 36枚を生成
  - これにより、クロス進化時に汎用 `Fusion_*.png` へ落ちる前に、相棒種族ごとの専用L3見た目が使われる
- **確認用プレビュー**
  - `Assets/ArtSource/Generated_MissingVariant_Preview.png`
  - 目視でF3/Core Bastion系の黄色盾が強かったため、オーバーレイを薄めに調整済み
- **画像台帳更新**
  - `IMAGE_ASSET_BACKLOG.md` の Partner数を114へ更新
  - ルートL0全18枚と種族別クロス進化L3全36枚を `TEMP` 扱いへ更新
  - 完全な種族別クロス進化セットの残りは `Partner_S{1-6}_F{1-6}_L{0-2}.png` の108枚
- **敵3種の外部PNG化**
  - `Tools/GenerateEnemyExternalAssets.ps1` を追加
  - `Dasher.png`, `Bomber.png`, `Phantom.png` を生成
  - `CreateAssets` で `LoadOptionalSprite("Skins/Dasher" / "Bomber" / "Phantom", fallback)` 対応へ変更
  - 確認用プレビュー: `Assets/ArtSource/Generated_EnemyExternal_Preview.png`
- **次候補**
  - 生成プレビューを見て、採用/作り直し候補を選ぶ
  - 既存ルート進化54枚の品質レビュー
  - HUD/ミニマップ画像HOOK、またはエリート敵/ボス第2形態の追加素材化

### 今回 (Codex ボス追跡修正、2026-05-20)
- **ボスの移動ターゲットをプレイヤーへ変更**
  - `UpdateEnemies` のボス分岐が `lantern.position` を直接追っていたため、Eggcoreへ向かって重なり、消えたように見える挙動が起きていた
  - `EnemyType.Boss` のターゲットを常に `player.position` にし、移動方向も共通 `target` から計算するよう変更
  - 射撃方向は従来通りプレイヤー狙いのまま維持
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete、`allyName` 未使用、`bossModuleReady` 未割当

### 今回 (Codex 残タスク整理、2026-05-20)
- **残タスク一覧を追加**
  - `REMAINING_TASKS.md` を作成
  - Claude と Codex が同時作業する前提で、`CoreLanternGame.cs` に触る作業と素材/台帳作業を分けて整理
  - キャラ素体・進化・融合は「現状維持、後で大幅リデザイン予定」と明記
- **画像台帳を更新**
  - `IMAGE_ASSET_BACKLOG.md` にキャラ素体・進化・融合の大幅リデザイン項目を追加
  - 現状の `Partner_*` / `Route_*` / `Fusion_*` 系素材は暫定確認用として扱う方針に変更
- **注意**
  - 今回は `CoreLanternGame.cs` は編集していない

### 今回 (Codex 現状確認/残タスク洗い出し、2026-05-21)
- **Claude引き継ぎと現行状態を確認**
  - `HANDOFF_FOR_CODEX.md`, `REMAINING_TASKS.md`, `IMAGE_ASSET_BACKLOG.md`, `ASSET_REVIEW_REPORT.md` を確認
  - `CoreLanternGame.cs` の主要キーワードを確認し、Claude側の2026-05-21変更が入っていることを確認
  - `CURRENT_STATUS_REVIEW_20260521.md` を追加
- **目視確認**
  - `AssetReview_PartnerRoutes.png`
  - `AssetReview_FusionStages.png`
  - `AssetReview_EnemiesMapUi.png`
  - `Generated_HudAsset_Preview.png`
  - `Generated_MapAsset_Preview.png`
  - `Generated_AllSpecies_Evolution_Preview.png`
- **新たに洗い出した重要残タスク**
  - `Wraith Lynx` / `Genesis Core` はコード上に追加済みだが、図鑑・進化ツリー・画像台帳はまだ6体前提が残っていた（次セッションで対応済み）
  - `Partner_S7*.png` / `Partner_S8*.png` の外部PNGも次セッションで暫定生成済み
  - クロス進化段階素材は、吸収変身よりエンブレム変化に見えやすい
  - HUD/Minimap素材は生成済み。後続セッションでMAP以外のHUD/UI/Relic/背景HOOKは対応済み、Minimap追加素材のみ未接続
  - `bossModuleReady` は未割当で、ボス後報酬を復活させるか削除しきるか仕様確定が必要
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete x2、`allyName` 未使用、`bossModuleReady` 未割当
- **注意**
  - 今回は `CoreLanternGame.cs` は編集していない

### 今回 (Codex 新キャラ/画像追従、2026-05-21)
- **図鑑を8体対応**
  - `CreateCodexPanel` のPARTNERに `Wraith Lynx` / `Genesis Core` を追加
  - 8体表示用に `CreateCodexSection` を4列x2段レイアウト対応
  - `GetCodexDescriptionFor` と旧 `BuildCodexText` に新キャラ説明を追加
- **進化ツリーを8体対応**
  - `CreatePartnerPreviewStrip` を `Cobalt / Ember / Sage / Hex / Drift / Iron / Wraith / Genesis` の8体表示へ変更
  - `evolutionTreeSelectedSpecies` のclampを1-8へ拡張
  - クロス進化プレビューも選択中素体で更新されるよう `treeCrossPreviewIcons` を追加
- **新規画像生成**
  - 生成スクリプト: `Tools/GenerateNewPartnerRelicUiAssets.ps1`
  - `Partner_S7_L0.png`, `Partner_S8_L0.png`
  - `Partner_S{7-8}_R{1-3}_L{0-3}.png` 全24枚
  - `Partner_S{7-8}_F{1-6}_L{0-3}.png` 全48枚
  - レリック6種: `Relic_Magnet`, `Relic_Deathless`, `Relic_CorePulse`, `Relic_Explosion`, `Relic_Storm`, `Relic_Mirror`
  - HUD/UI追加素材: BossBar, RelicSlot, HP/Core/EXP/DataChip icons, DangerVignette, ChoiceDim, BossWarning, menu/result/cutin backgrounds など
  - 確認用プレビュー: `Assets/ArtSource/Generated_NewPartnerRelicUi_Preview.png`
- **画像台帳更新**
  - `IMAGE_ASSET_BACKLOG.md`: Partner 296 / HUD 26 / Minimap 9 へ更新
  - `ASSET_REVIEW_REPORT.md`: active PNG 432 へ更新
  - `REMAINING_TASKS.md` と `CURRENT_STATUS_REVIEW_20260521.md` も新キャラ追従済みへ更新
  - `Tools/BuildAssetReviewSheets.ps1` を8体対応に更新し、レビューシートを再生成
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete x2、`allyName` 未使用、`bossModuleReady` 未割当

### 今回 (Codex UI/HUD画像HOOK、2026-05-21)
- **MAP以外のUI/HUD画像をコードへ接続**
  - `CoreLanternGame.cs` に HUD/UI 用Spriteフィールドとロード処理を追加
  - `CreatePanel` で `HUD_Panel_*` / `Panel_Frame*` を自動適用。ただし `Minimap Panel` は対象外
  - `CreateHudBar` に `HUD_Bar_Back`、HP/EXP fill、遅延HPバー、Player/Core/EXPアイコンを適用
  - Wave進行バーに `HUD_WaveProgress_Frame` / Normal/Boss fill を適用
  - Boss HPバーに `HUD_BossBar_Frame` / `HUD_BossBar_Fill` を適用
  - 選択集中用の暗転に `HUD_ChoiceDim` を適用
  - 低HP警告に `HUD_DangerVignette_Player/Core` を適用
  - メインメニュー、リザルト、進化/クロス進化カットイン背景を生成PNGに接続
  - レリック選択カードと右HUDのレリックスロットに `Relic_*` アイコンと `HUD_RelicSlot_Frame` を表示
  - Boss cutscene slash に `HUD_BossWarning_Banner` を割り当て
- **意図的に未変更**
  - ユーザー指定通り、MAP本体・床・ミニマップ素材の追加HOOKは今回触っていない
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete x2、`allyName` 未使用、`bossModuleReady` 未割当
  - Unity Play実機確認は未実施。次にGameビューでHUDの重なり/見え方を目視確認する

### 今回 (Codex UI/HUD品質差し戻し、2026-05-21)
- **ユーザー目視フィードバック**
  - 生成UI画像が荒く、情報量が多すぎ、パネル/カード/背景を引き延ばしているため品質が落ちている
  - プロ目線では「派手な画像を貼る」より、スマートな余白・線・色面を優先する方がよい
- **対応**
  - `UseGeneratedUiSkins = false` を追加し、大型UI画像の自動適用を停止
  - 停止対象: HUDパネル画像、カードフレーム画像、メニュー/リザルト/カットイン背景画像、Boss/Wave/HP/EXPバーのビットマップ、選択暗転画像、危険Vignette画像、Boss警告帯画像
  - 維持対象: データチップ/HP/EXP/レリックなどの小アイコン。これらは `preserveAspect = true` で比率維持
  - これにより、UI骨格はコード生成のクリーンなネオン線/色面に戻り、低解像度画像の非等倍引き伸ばしを避ける
- **今後**
  - 大型UI画像を復活させるなら、高解像度かつ用途ごとのアスペクト比固定、または9-slice対応が必要
  - 現段階では「小アイコン + procedural panel」が最もスマートで安全
- **検証**
  - Roslyn compile check 通過
  - 残警告: `FindObjectOfType<T>()` obsolete x2、`allyName` 未使用、`bossModuleReady` 未割当

### 今回 (Codex 非キャラ素材再生成、2026-05-21)
- **対象**
  - ユーザー要望: `HUD_UI_ASSET_REQUEST.md` の指示書に基づき、キャラ以外の画像を生成
  - キャラ素体/進化/融合は触らず、UI/HUD/背景/VFX/小アイコン/一部P3素材のみ対象
- **生成スクリプト追加**
  - `Tools/GeneratePolishedNonCharacterAssets.ps1`
  - 実行時に既存同名PNGを `Assets/ArtSource/NonCharacterAssetBackup_20260521_233106/` へ退避
  - 確認用プレビュー: `Assets/ArtSource/Generated_PolishedNonCharacterAsset_Preview.png`
- **生成/更新した主な素材**
  - P0: `Card_Cyan/Gold/Green/Magenta/Red`, `HUD_Panel_Wave/HP/Level/ChipMini/Loadout`, `HUD_Bar_*`, `HUD_WaveProgress_*`
  - P1: `Background_MainMenu/Evolution/CrossEvolution/Victory/Defeat/BossPulswyrm/BossNullwyrm`
  - 既存コード互換: `MainMenu_Background`, `Cutin_Evolve_Speed/Power/Guard`, `Cutin_Fusion`, `Result_Clear_Background`, `Result_GameOver_Background`
  - P2/P3: `Bullet_*`, `Pickup_Heal`, `Effect_*`, `Icon_Link_*`, `Icon_Stat_*`, `Floor_DarkBase`, `Floor_Hazard_Lava`, `Arena_Boundary`
- **品質方針**
  - 前回の失敗対策として、カード/パネル/バーは中央をほぼ無装飾にし、外周と角だけで密度を出す
  - 背景は16:9固定かつ低コントラスト。UIに貼っても文字を邪魔しない前提
  - 今回は `CoreLanternGame.cs` を編集していない。大型UI画像は引き続き `UseGeneratedUiSkins = false` のため未使用で、復帰させるなら9-slice/比率維持確認が必要
- **検証**
  - PNG生成完了
  - 主要対象の寸法確認済み
  - コード変更なしのためRoslyn compile checkは未実施

### 今回 (Codex 設計/台帳更新、2026-05-22)
- **設計ドキュメント追加**
  - `docs/STAGE_DESIGN_SPEC.md`: Stage 2 溶岩エリア、Stage 3 候補3案と推奨案を整理
  - `docs/BOSS_PHASE2_SPEC.md`: Nullwyrm HP50%以下の第2形態、Pulswyrmはミニエンレージ推奨
  - `docs/CHARACTER_REDESIGN_SPEC.md`: 8素体 + Halo Caster、Claude側追加中の Pulse Hydra を含む大幅リデザイン草案
  - `docs/SOUND_ASSET_CANDIDATES.md`: Kenney / OpenGameArt / DOVA / Freesound / Pixabay の候補とライセンス確認リンク
- **台帳更新**
  - `IMAGE_ASSET_BACKLOG.md`: 旧Codex生成大型UI画像を `DEPRECATED` 扱い、新非キャラ素材を `DONE / REVIEW` として整理
  - `ASSET_REVIEW_REPORT.md`: 68件の非キャラ素材、Stage1背景有効化状況、`Pickup_Data.png` サイズ判断を追記
  - `REMAINING_TASKS.md`: 冒頭の直近完了に今回のMarkdown/台帳更新を追記
- **判断**
  - `Pickup_Data.png` は 116×112 のまま現状維持。64×64版はUI小アイコンで必要になった時に別途作る
  - 大型UI画像は9-slice/比率維持なしで再有効化しない
- **注意**
  - 今回は `Assets/Scripts/CoreLanternGame.cs` を編集していない
  - Claudeが同時作業中のため、コード側フラグ差分はClaude側の最新状態を優先確認する

### 今回 (Codex 背景画像低密度再生成、2026-05-22)
- **背景画像の再生成**
  - `Tools/GeneratePolishedNonCharacterAssets.ps1` の `Draw-Backdrop` を修正
  - `Background_MainMenu`, `Background_Evolution`, `Background_CrossEvolution`, `Background_Victory`, `Background_Defeat`, `Background_BossPulswyrm`, `Background_BossNullwyrm` を再生成
  - コード互換aliasの `MainMenu_Background`, `Cutin_Evolve_*`, `Cutin_Fusion`, `Result_Clear_Background`, `Result_GameOver_Background` も同じ関数で再生成
- **品質方針**
  - 全面グリッド、赤縦縞、中央リング、斜め帯を削除
  - 中央60%はほぼ単色グラデのみ
  - 装飾は画面端の細い低アルファラインだけ
  - 平均アルファはおおむね160前後で、背景単体の主張を抑制
- **検証**
  - `Generated_PolishedNonCharacterAsset_Preview.png` を目視確認
  - `Background_Defeat.png` の赤縦縞が消えていることを確認
  - 最新バックアップ: `Assets/ArtSource/NonCharacterAssetBackup_20260522_003259/`
- **注意**
  - 今回は `Assets/Scripts/CoreLanternGame.cs` を編集していない

### 今回 (Codex Claude進捗表作成、2026-05-22)
- **Claude共有用の進捗表を追加**
  - `CLAUDE_TASK_PROGRESS.md` を新規作成
  - 指示書、台帳、直近完了を元に、P0/P1/P2のタスク状態・Claude次アクション・参照ファイル・競合注意を表形式で整理
  - `Pulse Hydra` / シグネチャ強化モジュールなどClaude側で進んだ項目もQA待ちとして反映
- **注意**
  - 今回は `Assets/Scripts/CoreLanternGame.cs` を編集していない

### 今回 (Codex キャラ画像最終フェーズ明記、2026-05-22)
- **Claude向け方針の明確化**
  - `CLAUDE_TASK_PROGRESS.md` のキャラ大幅リデザインを `P1` から `FINAL` 扱いへ変更
  - キャラ素体・進化後・クロス進化画像は作業量が大きいため、ステージ/ボス/UI/ゲーム機能が固まった後の最後にまとめて実施する方針を明記
  - Claude側は当面、正式PNG生成ではなく、名前/説明/図鑑/進化ツリー表示の整合確認だけを行う
- **画像台帳の明確化**
  - `IMAGE_ASSET_BACKLOG.md` のキャラ大幅リデザイン章を `FINAL` 扱いに変更
- **注意**
  - 今回は `Assets/Scripts/CoreLanternGame.cs` を編集していない

### 今回 (Codex 背景/カットイン画像配置、2026-05-22)
- **低密度背景を段階的に有効化**
  - `UseGeneratedBackgrounds = true`
  - `UseGeneratedCutscenes = true`
  - `UseGeneratedOverlayImages = false` を追加し、暗転板/警告帯/危険vignetteは手描きUIのまま維持
- **配置した画像**
  - `Background_MainMenu` をメインメニューへ接続
  - `Background_Victory` / `Background_Defeat` をリザルトへ接続
  - `Background_Evolution` / `Cutin_Evolve_*` / `Background_CrossEvolution` を進化・クロス進化カットインへ接続
  - 未接続だった `Background_BossPulswyrm` / `Background_BossNullwyrm` をボスカットイン背景へ接続
- **品質対策**
  - 全画面背景は `ApplyScreenBackgroundSprite` 経由で `preserveAspect = true` にし、画像比率を変えない
  - HUDバー/パネル/カードフレームは引き伸ばし品質リスクが残るため引き続きOFF
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`allyName` 未使用

### 今回 (Codex 難易度緩和、2026-05-22)
- **目的**
  - 特定キャラでしかクリアしづらい状態を改善し、非メタ相棒でも育成が間に合うようにする
- **プレイヤー側の土台強化**
  - 初期HP: `5 -> 6`
  - コアHP: `18 -> 24`
  - 初期EXP必要量: `8 -> 7`
  - EXPカーブ: `*1.35 + 2 -> *1.28 + 1.6`
  - 回収範囲: `1.1 -> 1.25`
- **敵圧の緩和**
  - Wave湧き数、後半追加湧き、湧き速度を全体的に低下
  - 後半HP倍率/速度倍率を緩和
  - Wave4はDark Field中にDasherを出さず、Shooter混成までに抑制
  - 高妨害のBomber/Phantom比率を低下
- **被ダメージ緩和**
  - 通常敵接触ダメージを15%軽減
  - Berserk接触倍率を `1.5 -> 1.2`
  - Shooter弾とボス弾の威力/頻度を低下
  - コア接触ダメージを低下
  - Dark Fieldの視界低下を `38% -> 24%`
- **ボス調整**
  - PulswyrmをHP80以下に収め、中ボスが最終ボス扱いの5way弾幕になる問題を回避
  - NullwyrmのHP/速度/接触威力も軽減
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`allyName` 未使用

### 今回 (Codex レビュー分析反映、2026-05-22)
- **目的**
  - 同ジャンルのユーザーレビュー/業界レビューで見える売れた理由と不満点を、Stage/Boss仕様に反映する
- **Stage設計更新**
  - `docs/STAGE_DESIGN_SPEC.md` にレビュー由来の設計原則を追加
  - Stage2は溶岩/煙を控えめな数値へ調整し、危険地帯の近くに任意報酬を置く方針へ変更
  - Stage3 Broken Core Networkは必須目標ではなく任意の有利目標として明確化
  - SPEED/GUARD/Siphon/Bulwark/PhaseがStage3 relayにそれぞれ役割を持つように設計追記
- **Boss Phase2設計更新**
  - `docs/BOSS_PHASE2_SPEC.md` にレビュー由来のボス設計原則を追加
  - Nullwyrm Phase2は「硬いだけ」ではなく `Core Mark` を主役にする方向へ整理
  - 移動速度/弾数/弾幕/召喚/ダメージの数値を、直近の難易度緩和に合わせて控えめに再調整
  - 進化/カード選択/ポーズ/カットイン中に特殊攻撃を重ねない mercy rule を追加
- **注意**
  - 今回はMarkdown設計のみ更新。`Assets/Scripts/CoreLanternGame.cs` は編集していない
  - `REMAINING_TASKS.md` と `CLAUDE_TASK_PROGRESS.md` にも反映済み

### 今回 (Codex Stage2選択/レリック再調整、2026-05-22)
- **Stage2選択修正**
  - `LoadProgress()` の Stage 解放読み込みを修正
  - `Stage_MaxUnlocked` が未作成の古い/新規セーブでも `MaxStageId` まで選べるようにし、開発中に Stage2 を直接テスト可能にした
  - 既存の `Clears > 0` または `BestWave >= MaxWave` からも Stage2 解放を補正
- **レリック性能差の圧縮**
  - `磁力核`: 回収範囲 `+2.2 -> +1.15`
  - `不滅の証`: 死亡回避に小さな最大HP補助を追加
  - `コアの鼓動`: 周期 `5s -> 6.5s`、範囲/ダメージ/粒子数を低下
  - `爆炎の紋章`: 爆発率固定高値をやめ、控えめな加算/上限に変更
  - `嵐の心臓`: 弾数+1を削除し、攻撃速度/弾速/寿命の軽め強化へ変更
  - `鋼鉄の盟約`, `オーバードライブ`, `データサージ`, `APEX CORE` も全体的に数値を抑制
  - HUDのレリック名辞書に追加レリック4種を追従し、ID表示にならないよう修正
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex Stage2マップデザイン差別化、2026-05-22)
- **目的**
  - Stage2 `Lava Cache` がStage1と同じ床に見える問題を改善し、選んだ瞬間に別ステージだと分かるようにする
- **実装**
  - `floorHazardLavaSprite` を追加し、`Skins/Floor_Hazard_Lava` をロード
  - `BuildStageEnvironment()` のStage2構築時に `CreateStage2MapSkin()` を呼ぶように変更
  - `CreateStage2MapSkin()` で以下を生成:
    - 赤黒い低密度の熱ウォッシュ
    - 焼けた床プレート
    - オレンジの導熱ラインとコアライン
    - 外周ヒートマーカー
    - 中央付近の控えめなクーラント安全プレート
  - すべて `stageObjects` 管理にして、Stage1へ戻る時や再構築時に `ClearStageObjects()` で消える
  - 溶岩プールは `Floor_Hazard_Lava` があれば使用し、なければ従来の円形スプライトへフォールバック
- **デザイン方針**
  - 派手な全面画像ではなく、読みやすさを残した色温度差と低密度装飾で差別化
  - 敵弾・HPバー・カード選択を邪魔しないよう、床装飾は低alphaで床レイヤーに限定
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex リザルト復帰修正、2026-05-22)
- **原因**
  - リザルトの `もう一度` / `メインメニュー` とポーズの `RESTART RUN` が `SceneManager.LoadScene(activeScene.buildIndex)` に依存していた
  - Unity Editorで未保存の `Untitled` シーンを使っている場合、`activeScene.buildIndex == -1` になり、LoadSceneが失敗する
- **対応**
  - `ReloadRuntime(bool autoStartRun)` を追加
  - Build Settings入りのシーンでは従来通り `LoadScene(activeScene.buildIndex)`
  - `buildIndex < 0` の未保存シーンでは `SoftReloadGeneratedRuntime()` で生成済みルートオブジェクトを破棄し、新しい `Core Lantern Game` を作って起動し直す
  - `RetryRun()`, `ReturnToMainMenu()`, ポーズ画面の `RESTART RUN` を共通処理へ接続
- **意図**
  - Editorでテスト中でも、リザルトからそのまま再戦/メニュー復帰できるようにする
  - 実ビルドやBuild Settings入りシーンでは通常のシーンリロードを維持
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex リザルト復帰再修正、2026-05-22)
- **追加原因**
  - 保存済み/Build Settings入りシーンでも、実行中の `SceneManager.LoadScene(...)` では `[RuntimeInitializeOnLoadMethod]` が毎回再発火するとは限らず、空のUnityシーンだけが表示されるケースが出た
  - このプロジェクトはシーンにオブジェクトを置かず、コード生成でゲームを構築するため、リザルト復帰でシーンリロードへ依存するのは不安定
- **対応**
  - `ReloadRuntime(autoStartRun)` から `SceneManager.LoadScene` 分岐を撤去
  - `ReloadGeneratedRuntimeRoutine()` を追加し、現在の生成済みルートオブジェクトを破棄した次フレームに新しい `Core Lantern Game` を作成
  - 新しい `CoreLanternGame.Awake()` が `LoadProgress()` / `CreateAssets()` / `CreateWorld()` / `CreateUi()` / `BuildUpgrades()` / `ShowTitle()` を通常起動と同じ順序で実行する
  - `AutoStartRunKey` は維持し、`もう一度` なら即相棒選択へ、`メインメニュー` ならタイトルへ戻る
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex リザルト復帰後の音消え修正、2026-05-22)
- **原因**
  - 通常起動時のUnity標準 `Main Camera` には `AudioListener` が付いている
  - ただしリザルト復帰の生成型リロード後は、`CreateWorld()` が自前で `Main Camera` を作るため `AudioListener` が付かない
  - その結果、`AudioSource` / BGM / SE の初期化自体は通っていても、音を聞く出力先がなく無音になっていた
- **対応**
  - `CreateWorld()` のカメラ初期化後に `AudioListener` の有無を確認
  - なければ `mainCamera.gameObject.AddComponent<AudioListener>()` で追加する
  - 初回起動の既存カメラでは重複追加しない
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`FindFirstObjectByType<T>()` obsolete x2、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 全体デバッグ/ジャンル差分反映、2026-05-22)
- **デバッグ範囲**
  - bootstrap / generated runtime reload / title overlays / result retry / main menu return / pause / options / upgrade / evolution / relic / wave completion / audio recovery を静的確認
  - Roslyn compile check と直近ログ確認を実施
- **修正**
  - メニュー上で Options / Mission / Codex / Evolution Tree が開いている時、Enter/Space でラン開始しないようにした
  - レリック選択中に `O` でOptionsが重なる導線をブロック
  - `ReloadRuntime` に `runtimeReloading` ガードを追加し、結果/ポーズ復帰ボタン連打で複数再生成される可能性を潰した
  - リロード開始後は result/restart ボタンを一時的に `interactable=false` にする
  - Unity 6でobsolete警告が出ていた `FindFirstObjectByType` を `FindAnyObjectByType` に置換
- **システム改善**
  - 通常強化カード画面に `スキップ +データ X` を追加
  - ボス報酬/レリック/進化はスキップ不可のまま
  - 強化候補が枯渇した場合は `緊急補給` を提示し、ソフトロックを防止
  - Stage2 `Lava Cache` の溶岩縁に少量のデータを配置し、危険地帯へ寄るリスク/報酬を追加
- **分析資料**
  - `docs/DEBUG_AND_GENRE_GAP_ANALYSIS_20260522.md` を追加
  - Vampire Survivors / Brotato / Halls of Torment / 20 Minutes Till Dawn の公開情報とレビュー傾向から、未実装の検討候補を整理
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 相棒選択UI修正、2026-05-22)
- **原因**
  - BEAST LINK画面で10体を固定配置していたため、上部タイトル/説明とカードが近く、カード内でも説明/ステータスが重なりやすかった
  - 相棒選択からメインメニューへ戻る明示的な導線がなかった
- **対応**
  - 相棒カードを3列の縦スクロールコンテンツへ変更
  - カード数に応じてスクロール領域の高さを自動調整
  - `戻る` ボタンとEscキーでメインメニューへ戻れるようにした
  - カード内の発光円/アイコン/名前/特性/説明/ステータスバーの位置とサイズを再調整
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 非キャラ画像 Batch 2、2026-05-22)
- **生成**
  - `Tools/GenerateNonCharacterPolishBatch2.ps1` を追加
  - `Assets/Resources/Skins/` に非キャラ素材32枚を生成
  - プレビュー: `Assets/ArtSource/Generated_NonCharacterPolishBatch2_Preview.png`
- **内容**
  - 追加レリック4種: `Relic_Titan`, `Relic_Overdrive`, `Relic_DataSurge`, `Relic_Apex`
  - 強化カード識別用: `ModuleIcon_*` 20種
  - 今後のステージ/UI用: `StageThumb_*`, `Stage2_LavaPool_A`, `Stage2_HeatVent_A`, `Stage3_RelayDevice_A`, `Stage3_CorruptionPatch_A`, `Pickup_Data_64`
- **コード接続**
  - 追加レリック4種を `GetRelicSprite()` に接続
  - 強化カードにタイトル内容に応じた `ModuleIcon_*` を表示する `ApplyModuleCardIcon()` を追加
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 音素材制作方針、2026-05-22)
- **目的**
  - ユーザー要望: ゲーム用SE/BGMをAI生成も含めて準備したい。ただし無料・商用利用可能を優先したい
- **追加資料**
  - `docs/AUDIO_ASSET_PLAN.md`: Stable Audio / Eleven Music / Suno Free / CC0素材の扱い、BGM/SE必要一覧、制作フロー、ライセンス判断ルール
  - `docs/AUDIO_GENERATION_PROMPTS.md`: BGM 8種、SE 18種の生成プロンプトとCC0検索キーワード
  - `docs/AUDIO_LICENSE_LOG_TEMPLATE.md`: AI生成音/フリー素材共通のライセンス証跡テンプレ
- **既存資料更新**
  - `docs/SOUND_ASSET_CANDIDATES.md` にAI生成ポリシーを追記
  - `REMAINING_TASKS.md` と `CLAUDE_TASK_PROGRESS.md` に音素材制作方針の完了を追記
- **方針**
  - SEはKenney/OpenGameArt/Freesound CC0と自作寄りを第一候補
  - BGM/不足SEはStable Audio 3.0を第一AI候補。ただしCommunity License条件と収益条件の記録必須
  - Eleven Musicは商用候補だが、無料商用の第一候補にはせず、plan/game rights確認後に限定
  - Suno Freeは公式Pricing上No commercial useなので商用ビルドでは使わない
- **注意**
  - 今回はMarkdownのみ更新。`Assets/Scripts/CoreLanternGame.cs` は編集していない
  - 実際のAI生成はサービスログイン/APIキー/生成結果のユーザー確認が必要。生成後はライセンス証跡を必ず保存する

### 今回 (Codex 音素材スターター自動生成、2026-05-22)
- **目的**
  - ユーザー要望: 音素材の生成/配置/ライセンス記録を自動化したい
- **追加**
  - `Tools/GenerateStarterAudio.ps1`: 第三者素材なしの決定的シンセ生成でスターターWAVを作成
  - `docs/AUDIO_AUTOMATION.md`: 実行方法、生成物、限界、次の自動化候補を記載
- **生成済み**
  - `Assets/Resources/Audio/BGM.wav`
  - `Assets/Resources/Audio/Shoot.wav`
  - `Assets/Resources/Audio/Hit.wav`
  - `Assets/Resources/Audio/Kill.wav`
  - `Assets/Resources/Audio/Pickup.wav`
  - `Assets/Resources/Audio/LevelUp.wav`
  - `Assets/Resources/Audio/Evolve.wav`
  - `Assets/Resources/Audio/Fusion.wav`
  - `Assets/Resources/Audio/Boss.wav`
  - `Assets/Resources/Audio/GameOver.wav`
  - `Assets/Resources/Audio/AudioManifest.json`
  - `Assets/Resources/Audio/Licenses/StarterAudio_License.txt`
- **設計**
  - 現行 `CoreLanternGame.cs` が読む `Resources.Load<AudioClip>("Audio/" + name)` の名前に合わせて生成
  - そのままUnity Editorで再生すればプロシージャルフォールバックではなくWAVが鳴る
  - 外部AI/API/素材サイトを使わないため、第三者ライセンス混入なし
- **注意**
  - 今回は `CoreLanternGame.cs` を編集していない
  - 音はスターター品質。最終版はStable AudioやCC0素材で置き換え、同じファイル名またはコード拡張で管理する

### 今回 (Codex SE多段ヒット抑制、2026-05-22)
- **原因**
  - 外部WAVを読み込むようになったことで、`Hit` / `Shoot` / `Kill` などが多段ヒットや連射ごとにすべて鳴り、音が重なりすぎてうるさくなっていた
- **対応**
  - `sfxLastPlayedAt` を追加し、`PlaySfx()` にSE名ごとの最短再生間隔を導入
  - `Hit`: 0.065秒、`Shoot`: 0.035秒、`Kill`: 0.045秒、`Pickup`: 0.028秒などで短時間の重複発音を間引く
  - 外部WAV再生時も `PlayOneShot(clip, volumeScale)` を使うようにし、SE名ごとの音量係数を追加
  - `Hit` / `Shoot` を控えめ、`Evolve` / `Fusion` は演出用に残すバランスへ調整
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 強化カード押せない問題修正、2026-05-22)
- **原因**
  - Data Lab / ショップで買えないカードを `interactable = false` にしていた
  - `upgradeButtons` は通常強化、レリック、進化、ショップで共通利用しているため、その無効状態が次の通常強化カードへ残っていた
- **対応**
  - `RenderUpgradeChoices()` で通常強化カード表示時に `upgradeButtons[i].interactable = true`
  - `OpenRelicSelect()` でレリックカード表示時に `upgradeButtons[i].interactable = true`
  - `OpenEvolution()` で進化カード表示時に `upgradeButtons[i].interactable = true`
  - 通常強化/ショップのスキップボタンも表示時に `interactable = true`
  - `RepairActiveChoiceButtonInteractivity()` を追加し、通常強化/進化画面の表示中は既に開いているカードも毎フレーム再有効化する保険を入れた
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex BGM上昇感の抑制、2026-05-22)
- **原因**
  - スターターBGMのリード音が高めで、A→C→D方向に上がるパターンと高い倍音が目立っていた
  - サバイバル系の長時間ループでは、常に煽られるように聞こえて疲れやすい
- **対応**
  - `Tools/GenerateStarterAudio.ps1` の `New-Bgm` を調整
  - 24秒/118 BPM から 32秒/104 BPM へ変更
  - 上昇リードを廃止し、低めのベース/パッド中心、少ない下降気味パルスのループに変更
  - `Assets/Resources/Audio/BGM.wav` を再生成し、`AudioManifest.json` / `StarterAudio_License.txt` も更新
- **検証**
  - `Tools/GenerateStarterAudio.ps1` 実行成功
  - `AudioManifest.json` で `BGM.wav` が `durationSeconds: 32` として出力されていることを確認
  - コード編集なしのためRoslyn compile checkは未実施

### 今回 (Codex 体験面ブラッシュアップ、2026-05-22)
- **目的**
  - 音量バランス、リザルトの再挑戦動機、強化カードの瞬間判断を改善
- **Options**
  - `BGM 音量` / `SE 音量` のUGUI Sliderを追加
  - `bgmVolume` / `sfxVolume` を `PlayerPrefs` に保存
  - `ApplyOptions()` で既存ON/OFFと音量スライダーを掛け合わせて `AudioSource.volume` に反映
- **リザルト**
  - 6番目の統計を `RANK` に変更し、Wave/コアHP/進化/リンク/融合/ボス撃破/クリア状況から S〜D を算出
  - コア残体力行の下に `NEXT:` を追加し、次ステージ/Danger/クロス進化/防衛/育成など次に狙う行動を表示
- **強化カード**
  - レアリティ表記を `★☆☆ BASIC` / `★★☆ RARE` / `★★★ EPIC` に変更
  - レアリティ枠を横に拡げ、説明文はbest-fit + truncateにして下段レアリティやボタン領域へはみ出しにくくした
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity analyzer load warning、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 最終ボス/リザルト/音/Stage2改善、2026-05-24)
- **最終ボス負荷対策**
  - `MaxBullets 180`, `MaxEnemies 80`, `MaxPickups 60`, `MaxSpriteGhosts 40`, `MaxFloatingTexts 36`, `MaxSparks 280` に圧縮
  - ボス連続ヒット時のスパーク/SE/ダメージ数字/ヒットフリーズを時間間引きし、多段ヒットビルドで重くなりにくくした
- **リザルト整理**
  - `Result Deep Scrim` を追加し、裏のHUD/戦闘画面が読めすぎる問題を抑制
  - 下部の長い取得モジュール羅列を廃止し、MVP BUILD + 2行サマリ中心へ整理
- **Options整理**
  - ゲーム中に敵HP/火力/速度を変更できるアクセシビリティ項目をUIから撤去
  - 保存値も1.0固定へ戻し、Danger/Stage以外で難度が変わらないようにした
  - パネル高さを600に縮め、Close周辺の枠重なりを緩和
- **画面端対策**
  - `mainCamera.clearFlags = SolidColor` を固定
  - 床のベーススケールをワイド画面向けに拡大済み
- **音**
  - `Tools/GenerateStarterAudio.ps1` を更新し、`BGM.wav` を24秒/22050Hzの低音寄りループへ再生成
  - `BGM_Stage2.wav` を追加し、`currentStageId == 1` のStage2専用BGMとして読み込む
  - `Pickup` / `LevelUp` / `Evolve` / `Fusion` の上昇音型を下降・低音寄りへ変更
- **Stage2専用敵**
  - `EnemyType.LavaCrawler` と `MakeLavaCrawlerSprite()` を追加
  - Stage2 `Lava Cache` の敵組成にLavaCrawlerを混ぜ、ステージごとの敵差別化を開始
- **検証**
  - `Tools/GenerateStarterAudio.ps1` 実行成功
  - Roslyn compile check 通過
  - 残警告: Unity SourceGenerator警告、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex 図鑑/進化ツリーUI整理、2026-05-24)
- **競合回避**
  - Claudeが同時対応中の Solar Anchor シグネチャ / Stage4-5敵組成 / Frost Crystal判定 / メインメニューコンボ進捗 / Last Stand / Resonance Wave は触らず、図鑑・進化ツリーの表示品質に限定
- **図鑑パネル**
  - `CreateCodexPanel` に `ScrollRect` / viewport / content を追加し、11体PARTNERが後続セクションに重なる問題を解消
  - `CreateCodexSection` の4列カード幅と間隔を調整し、狭いカードではタイトル/ステータス/説明文を小さめのbest-fitに変更
  - `CodexEntry.description` / `accent` をカードごとに保持し、Halo Caster / Pulse Hydra / Solar Anchor の説明が `GetCodexDescriptionFor` 未追従で空にならないよう修正
  - 図鑑を開いた時はスクロール位置を上に戻す
- **進化ツリー**
  - 素体選択列を Cobalt / Ember / Sage / Hex / Drift / Iron / Wraith / Genesis / Halo / Pulse / Solar の11体対応へ更新
  - 選択speciesのclampを 1..11 に拡張し、新キャラでもSPEED/POWER/GUARDとクロスプレビューを確認できるようにした
  - 左上の `BEAST LINK` ラベルがカードに埋まりにくい位置/サイズへ調整
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity SourceGenerator警告、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex キャラ画像プロンプト確認、2026-05-24)
- **確認対象**
  - `docs/CHARACTER_IMAGE_PROMPTS.md`
  - 参照照合: `docs/CHARACTER_REDESIGN_SPEC.md`, `IMAGE_ASSET_BACKLOG.md`, `GetPartnerVariantSprite` の読み込み命名
- **修正**
  - 共通スタイルに「3/4 top-down」「48pxで読める」「crisp sprite-like」「transparent background preferred」を追加
  - Negative promptから既存IP名の直接誘導を弱め、`established monster franchise` / `familiar IP look` など商用安全寄りに整理
  - 出力規格と命名規約をコード互換に修正: 素体 `Partner_S{n}_L0`, ルート `Partner_S{n}_R{route}_L{lv}`, クロス `Partner_S{n}_F{fusion}_L{lv}`
  - L2個別プロンプト不足に備え、L1とL3の中間形を作る共通L2生成ルールを追加
  - Phase枚数の矛盾、`全 keras` typo、クロス進化のOption計算を修正
- **次に生成するなら**
  - まず22枚 (L0 11 + 代表L3 11) でシルエット審査
  - 通ったら44枚 (L0 11 + 各ルートL3 33) でルート差を審査
  - その後110枚の本編差し替えへ進む
- **検証**
  - ドキュメント修正のみ。コード変更なし、コンパイル確認なし

### 今回 (Codex Cobalt Pup 進化画像試作、2026-05-24)
- **目的**
  - ユーザー要望: まず1体だけ、進化パターンも含めて生成し、透過を安定させたい
- **生成**
  - Cobalt Pup `L0`
  - Cobalt Pup `R1 SPEED L3`
  - Cobalt Pup `R2 POWER L3`
  - Cobalt Pup `R3 GUARD L3`
- **透過処理**
  - built-in image generationで単色クロマキー背景を指定
  - `remove_chroma_key.py` で透過PNG化
  - 512x512版の4隅alphaが0であることを確認
- **保存先**
  - 元画像/透過原寸: `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/`
  - 差し替え候補512版: `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/ready_512/`
  - プレビュー: `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/Cobalt_Evolution_Transparent_Preview.png`
- **注意**
  - ユーザー指示で `Partner_S1_L0.png`, `Partner_S1_R1_L3.png`, `Partner_S1_R2_L3.png`, `Partner_S1_R3_L3.png` は `Assets/Resources/Skins/` に反映済み
  - 既存画像バックアップ: `Assets/ArtSource/CharacterBackup_CobaltBeforeImplement_20260524_041559/`
  - L1/L2は未生成のため、Cobaltを本採用するなら次に `Partner_S1_R1_L1/L2`, `Partner_S1_R2_L1/L2`, `Partner_S1_R3_L1/L2` を生成する

### 今回 (Codex メインメニュー顔作り、2026-05-24)
- **目的**
  - ユーザー要望: タイトル画面はゲームの顔なので、もっとかっこよくしたい
- **対応**
  - `CreateMainMenuPanel()` を再構成
  - 中央に `Main Menu Hero Deck` を追加し、Cobalt Pup SPEED L3、Data Egg Core、SPEED/POWER/GUARDのルートバッジを表示
  - タイトル/サブタイトル/進行サマリー/ラン設定サマリー/START RUN/サブボタンのY配置を整理
  - 背景に `Menu Deep Scrim` とステータスリボンを追加し、背景の主張を抑えて視線を中央に集める
  - `RefreshMainMenuSummary()` の表記を短縮し、`BEST | CLEARS | MISSIONS | DANGER | STAGE | COMBO` 形式へ変更
- **新規ヘルパー**
  - `CreateMainMenuHeroVisual`
  - `CreateMenuSpriteImage`
  - `CreateMainMenuRouteBadge`
- **検証**
  - Roslyn compile check 通過
  - 残警告: Unity SourceGenerator警告、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

### 今回 (Codex タイトル重なり/白点修正、2026-05-25)
- **目的**
  - ユーザー指摘: キャラ中央の白い点が不要。タイトル画面の文字はみ出し/画像重なりは仕様ではないので、報告前に必ず確認する
- **対応**
  - プレイヤー通常射撃時の `SpawnAttackFlash(player.position, ...)` を停止し、キャラ絵中央に白い発射フラッシュが乗らないようにした
  - 不要になっていた `playerCoreBadge` / `playerCoreBadgeRenderer` と代入を削除
  - メインメニューのタイトル/サブタイトル/ステータスリボン/選択概要/ヒーローデッキ/START/下部ボタン/ガイドのY配置を再調整
  - ヒーローデッキ内の `BEAST LINK ONLINE` / `DATA EGG CORE` ラベルを撤去し、文字と画像が競合しない構成へ変更
  - Cobalt Pup、Data Egg、SPEED/POWER/GUARDバッジを縮小・再配置して、見せ場は残しつつ重なりを避けた
- **検証**
  - 静的矩形チェック `MAIN_MENU_LAYOUT_CHECK: OK (no disallowed overlaps)`
  - `playerCoreBadge` / `Evolution Core Badge` / `SpawnAttackFlash(player.position` / hero label 残存なしを `rg` で確認
  - Roslyn compile check 通過
  - 残警告: Unity SourceGenerator警告、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用

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
### Recent session (Codex Stage/Boss Enemy PNGs, 2026-05-25)
- Read this handoff before work.
- User asked to continue available image-generation tasks.
- Did not touch character/evolution/fusion assets because the backlog says those are final-phase redesign work.
- Added `Tools/GenerateStageBossEnemyAssets.ps1`.
- Generated transparent 256x256 resource PNGs into `Assets/Resources/Skins/`: `LavaCrawler.png`, `Enemy_MagmaTitan.png`, `Enemy_CorruptionDrone.png`, `Enemy_FrostKnight.png`, `Enemy_VoltDasher.png`, `Boss_Pulswyrm.png`, `Boss_Nullwyrm.png`.
- Existing `CoreLanternGame.cs` already calls `LoadOptionalSprite` for these names, so no code hook change was needed.
- Preview sheet: `Assets/ArtSource/Generated_StageBossEnemy_Preview.png`.
- Verification: all 7 PNGs are 256x256 and all four corners have alpha 0.
### Recent session (Codex Stage 4/5 Support PNGs, 2026-05-25)
- Added `Tools/GenerateStage45SupportAssets.ps1`.
- Generated Stage 4/5 support assets into `Assets/Resources/Skins/`: `StageThumb_Frost.png`, `StageThumb_Storm.png`, `Stage4_FrostCrystal_A.png`, `Stage4_FrostPatch_A.png`, `Stage5_LightningMarker_A.png`, `Stage5_LightningStrike_A.png`.
- Preview sheet: `Assets/ArtSource/Generated_Stage45Support_Preview.png`.
- Verification: thumbnails are 160x90 opaque PNGs; hazard/VFX files have transparent corners. No `CoreLanternGame.cs` changes were made.

### Debug session (Codex generated image assets, 2026-05-25)
- Re-ran both generators successfully.
- Added `.meta` backfill helpers to `Tools/GenerateStageBossEnemyAssets.ps1` and `Tools/GenerateStage45SupportAssets.ps1`.
- Verified the 13 generated gameplay/support PNGs have expected dimensions and alpha behavior.
- Verified the 7 enemy/boss resources are loaded by existing `CoreLanternGame.cs` paths.
- Found that Stage 4/5 support assets are not referenced by `CoreLanternGame.cs` yet; they are import-safe and ready, but not displayed until a hook-up pass adds resource fields/usage.
- Roslyn compile check passed. Remaining warnings are existing Unity SourceGenerator analyzer warnings plus unused fields.

### Fix session (Codex boss alert sprite link, 2026-05-25)
- User reported that the boss alert image and the spawned boss image were not linked.
- Updated `BeginBossCutscene()` to use boss-specific encounter sprite/tint via `GetBossEncounterSprite()` and `GetBossEncounterTint()`.
- Updated `SpawnBoss()` to use the same resolver, so alert/cutscene and actual spawned boss stay synchronized for `Pulswyrm` and `Nullwyrm`.
- Follow-up pass removed separate boss name/subtitle parameters from the intro flow: `BeginBossCutscene(bool isMidBoss)` now derives name, intro text, accent, warning color, portrait, background, and optional BGM from one boss-type flag.
- Updated boss victory cutscene to derive its name/portrait/tint from the killed boss flag too.
- Added optional boss BGM loading for `BGM_Boss_Pulswyrm` and `BGM_Boss_Nullwyrm`, matching the audio docs, with old no-underscore names as fallback.
- Extended `Tools/GenerateStarterAudio.ps1` to generate `BGM_Boss_Pulswyrm.wav` and `BGM_Boss_Nullwyrm.wav` as low, non-ascending placeholder loops.
- Generated both boss BGM files into `Assets/Resources/Audio/` with `.meta`, `AudioManifest.json`, and `Licenses/StarterAudio_License.txt` entries.
- Improved `-NoOverwrite` so existing WAVs are skipped before expensive synthesis but still written to the manifest.
- Added a mid-boss victory BGM restore: after the `Pulswyrm` kill cutscene ends, `PlayBgm()` resumes the current stage loop instead of leaving boss music active.
- Added `Tools/TestAudioResources.ps1` to validate `LoadAudioClip()` names against WAV files, `.meta` files, and `AudioManifest.json`.
- `Tools/TestAudioResources.ps1` passes with 0 errors; expected warnings only for optional legacy boss BGM fallback names.
- Roslyn compile check passed. Remaining warnings are existing Unity SourceGenerator analyzer warnings plus unused fields.

### Tooling session (Codex image resource validation, 2026-05-27)
- Read this handoff before work.
- Added `Tools/TestImageResources.ps1`.
- The tool validates `Assets/Resources/Skins/*.png`, missing Unity `.meta` files, static sprite resource references in `CoreLanternGame.cs`, and DONE entries in `IMAGE_ASSET_BACKLOG.md`.
- Ran `powershell -ExecutionPolicy Bypass -File .\Tools\TestImageResources.ps1`.
- Result: 502 Skins PNGs, 121 static code sprite references, 138 backlog PNG references, 29 DONE backlog PNG references, 0 errors.
- Remaining warning: optional `Player_Form4.png` is missing and currently falls back to procedural generation. This is not a blocker.
- Cleaned active backlog wording for `Minimap_*` assets: minimap was already removed, so those PNGs are marked REMOVED instead of HOOK and should not be connected unless the feature is revived.
- No `CoreLanternGame.cs` changes were made in this pass.

### Recent session (Codex game logo draft, 2026-05-30)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `Tools/GenerateLogoAssets.ps1`.
- Generated deterministic text-rendered logo PNGs in `Assets/Resources/Skins/`: `GameLogo.png`, `GameLogo_Mono.png`, `GameLogo_Mark.png`, `GameLogo_Mark_Mono.png`.
- Art-source copies and preview: `Assets/ArtSource/Logo_Drafts_20260530/GameLogo_Preview.png`.
- Verified the four resource PNGs match intended dimensions and have transparent corners.
- `StudioLogo_Embercore*` was not generated because a quick name collision check found an existing Steam game named `Embercore`; studio name needs user confirmation or rename before final studio logo work.

### Recent session (Codex title logo hook spec, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Checked current title implementation: `CreateMainMenuPanel()` still renders `EGGCORE PROTOCOL` as live text and does not load `GameLogo.png` yet.
- Added `docs/CLAUDE_TITLE_LOGO_HOOK_SPEC.md` with exact hook-up instructions for Claude.
- Recommended placement: `GameLogo.png` at `new Vector2(0, 270)` with size `new Vector2(560, 280)`, `preserveAspect=true`, existing title text kept as fallback only.
- Status: Claude hook-up pending.

### Recent session (Codex smart title simplification spec, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Generated `Assets/Resources/Skins/GameLogo_TitleCompact.png` from `GameLogo.png` with reduced vertical padding for cleaner title-screen placement.
- Added `Tools/GenerateTitleLogoCompact.ps1`.
- Added `docs/CLAUDE_TITLE_SCREEN_SIMPLIFY_SPEC.md`.
- Simplification direction: keep logo, short subtitle, START RUN, subtle secondary buttons, tiny version; remove title progress stats, Stage/Danger line, hero deck, route badges, operation guide, and unconfirmed studio name.
- Status: Claude hook-up pending.

### Recent session (Codex exe icon set, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `Tools/GenerateExeIconAssets.ps1`.
- Generated Windows icon source PNGs in `build/Icons/`: `icon_16.png`, `icon_32.png`, `icon_48.png`, `icon_64.png`, `icon_128.png`, `icon_256.png`, `icon_512.png`.
- Generated `build/Icons/EggcoreProtocol.ico` with standard Windows sizes 16 through 256.
- Art-source copies and preview: `Assets/ArtSource/Icon_Drafts_20260531/IconSet_Preview.png`.
- Verified all PNG files have expected dimensions and transparent corners.
- Status: P0-3 `.exe` icon asset complete. Unity Player Settings hook-up remains a Claude/Unity-side task.

### Recent session (Codex itch.io page asset draft, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `Tools/GenerateItchIoPageAssets.ps1`.
- Generated marketing PNG drafts into `marketing/ItchIo/`: `ItchIo_Header_630x500.png`, `ItchIo_Cover_315x250.png`, `ItchIo_SocialCard_1200x630.png`.
- Art-source copies and preview: `Assets/ArtSource/ItchIo_20260531/ItchIo_PageAssets_Preview.png`.
- Verified all three marketing PNG files match intended dimensions and are opaque at the corners.
- Preview was visually checked; tagline text was split into two lines so it does not clip or collide.
- Status: P2-4 `itch.io` page image draft complete. Final character-heavy page art should wait until the character redesign pass is settled.

### Recent session (Codex exe icon Unity hook spec, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `docs/CLAUDE_EXE_ICON_PLAYER_SETTINGS_SPEC.md`.
- The spec tells Claude/Unity to use imported PNGs under `Assets/ArtSource/Icon_Drafts_20260531/` for Player Settings and keep `build/Icons/EggcoreProtocol.ico` for installer/shortcut packaging.

### Recent session (Codex HUD 9-slice candidates, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `Tools/GenerateHud9SliceAssets.ps1`.
- Generated quieter HUD 9-slice candidate PNGs with new `HUD9_*` names so current HUD behavior is unchanged until Claude hooks them:
  - `HUD9_Panel_Wave.png`, `HUD9_Panel_HP.png`, `HUD9_Panel_Level.png`, `HUD9_Panel_ChipMini.png`
  - `HUD9_Bar_Back.png`, `HUD9_Bar_HP_PlayerFill.png`, `HUD9_Bar_HP_CoreFill.png`, `HUD9_Bar_EXP_Fill.png`
  - `HUD9_WaveProgress_Frame.png`, `HUD9_WaveProgress_Fill_Normal.png`, `HUD9_WaveProgress_Fill_Boss.png`
- Art-source copies and preview: `Assets/ArtSource/HUD_9Slice_20260531/HUD9_Preview.png`.
- Added `docs/CLAUDE_HUD9SLICE_HOOK_SPEC.md` with sprite border and QA instructions.
- Verified all 11 PNGs have expected dimensions, transparent corners, and `.meta` files with sprite borders.

### Recent session (Codex Result_StatTile_v2 resize, 2026-05-31)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Updated `Tools/GenerateResultV2Assets.ps1` so `Result_StatTile_v2.png` is generated as `420x56` instead of the old `112x76`.
- Re-ran the generator with `-Force`, regenerating Result V2 art-source preview.
- Verified `Assets/Resources/Skins/Result_StatTile_v2.png` is `420x56`, has transparent corners, and has a `.meta` file.
- Updated `IMAGE_ASSET_BACKLOG.md`, `docs/ASSET_HOOKUP_MAP.md`, and `docs/UI_V2_HOOKUP_PLAN.md` to reflect the new size.

### Recent session (Codex HUD9 Wave progress limited hook-up, 2026-06-01)
- Read this handoff before work.
- User explicitly asked Codex to take over this time because Claude was rate-limited, so Codex temporarily edited `Assets/Scripts/CoreLanternGame.cs`.
- Acquired `EDIT_LOCK.md` before editing and released it after validation.
- Hooked only the top Wave progress HUD to the new quiet HUD9 9-slice candidates:
  - `HUD9_WaveProgress_Frame`
  - `HUD9_WaveProgress_Fill_Normal`
  - `HUD9_WaveProgress_Fill_Boss`
- Added `UseHud9SliceCandidates` and `ApplySlicedSprite()` so the change is opt-in, isolated, and falls back to the existing V2/procedural HUD if assets are missing.
- Did not hook the HUD9 panel/HP/EXP/chip assets yet. Those remain pending until the Wave bar is visually approved in Unity.
- Validation passed:
  - `Tools/CheckCompileHealth.ps1` full: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
  - `Tools/TestImageResources.ps1`: `Errors: 0`; only optional `Player_Form4.png` fallback warning remains.

### Recent session (Codex asset status reporting, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `Tools/ReportAssetStatus.ps1`, a read-only report that compares `Assets/Resources/Skins/*.png` against static sprite loads in `CoreLanternGame.cs`.
- Updated HUD9 backlog status so the three Wave progress files are marked `HOOKED`, while the remaining HUD9 panels/bars stay candidate-only.
- Updated `Tools/README.md`, `ASSET_REVIEW_REPORT.md`, `docs/CODEX_RELEASE_ORDER.md`, and `REMAINING_TASKS.md` to reflect the partial HUD9 hook-up.
- Validation:
  - `Tools/ReportAssetStatus.ps1`: 591 skins, 180 static refs, HUD9 hooked 3 / waiting 8, Stage4/5 support waiting 6
  - `Tools/TestImageResources.ps1`: `Errors: 0`
  - `Tools/TestAudioResources.ps1`: `Errors: 0`
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`

### Recent session (Codex Player_Form4 fallback cleanup, 2026-06-01)
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Restored `Assets/Resources/Skins/Player_Form4.png` and `.meta` from the existing disabled backup files.
- Left the `.disabled` backups in place.
- `Tools/TestImageResources.ps1` now reports `Warnings: 0` and `Errors: 0`.

### Recent session (Codex Stage4/5 support hook spec, 2026-06-01)
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `docs/CLAUDE_STAGE45_SUPPORT_HOOK_SPEC.md`.
- Purpose: help Claude hook the six generated but currently unreferenced Stage 4/5 support assets:
  - `StageThumb_Frost`, `StageThumb_Storm`
  - `Stage4_FrostCrystal_A`, `Stage4_FrostPatch_A`
  - `Stage5_LightningMarker_A`, `Stage5_LightningStrike_A`
- `Tools/ReportAssetStatus.ps1` currently reports Stage 4/5 support `Present: 6 / Hooked: 0 / Waiting: 6`.

### Recent session (Codex Cobalt Pup pixel animation prototype, 2026-06-01)
- User asked for a Cobalt Pup pixel-art animation prototype sheet.
- Used built-in image generation, then chroma-key removal for a transparent sheet.
- Saved outputs under `Assets/ArtSource/CobaltPixelAnim_20260601/`.
- Generated:
  - `CobaltPup_PixelAnimSheet_chromakey.png`
  - `CobaltPup_PixelAnimSheet_transparent.png`
  - cropped `frames/CobaltPup_{idle/run/attack/hit}_NN.png`
  - `CobaltPup_idle_preview.gif`, `CobaltPup_run_preview.gif`, `CobaltPup_attack_preview.gif`, `CobaltPup_hit_preview.gif`
- This is ArtSource/prototype only. It is not connected to `Assets/Resources/Skins/` or runtime animation yet.

### Recent session (Codex Cobalt Pup pixel animation correction, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User rejected the hand-drawn/redrawn `CobaltPup_Actual_*` prototype because it lost the quality of the AI rough and had poor rear-leg motion.
- Moved rejected outputs to `Assets/ArtSource/AnimationPrototypes/deprecated_bad_redraw_20260601/`.
- Moved rejected generator to `Tools/Deprecated/GenerateCobaltActualPixelMotionPrototype_BAD_20260601.ps1`.
- Added `Tools/GenerateCobaltPixelSheetReviewGifs.ps1`.
- Generated review GIFs by cropping the approved AI rough sheet directly, without redrawing:
  - `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_MotionReview_v1.gif`
  - `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_Run_review_v1.gif`
  - `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_Attack_review_v1.gif`
  - `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_NormalizedReviewSheet_v1.png`
- Added `docs/CHARACTER_PIXEL_ART_PIPELINE.md` for Claude/Codex shared rules:
  AI rough -> pixel-style unification -> transparency/shrink test -> GIF motion check -> in-game check.
- Important: do not connect deprecated redraw outputs to runtime. Only connect character animation after user approves the review GIFs.

### Recent session (Codex character-animation QA failure postmortem, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User correctly pointed out that the follow-up output still did not fix rear-leg movement or attack motion; it only re-exported/cropped existing art and changed presentation.
- Added `docs/POSTMORTEM_20260601_CHARACTER_ANIMATION_QA_FAILURE.md`.
- Updated `docs/CHARACTER_PIXEL_ART_PIPELINE.md` with a stricter correction gate:
  - do not present crop/re-export/relabel as a fix
  - confirm RUN and ATTACK visually before reporting
  - require actual visible behavioral change before saying fixed
- Current Cobalt `review_gifs` are not fixed runtime candidates. Treat them only as rough-review material until a fresh AI/manual frame pass changes the rear-leg and attack problems.

### Recent session (Codex global character-animation QA expansion, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User correctly pointed out that the postmortem countermeasures were too local to Cobalt Pup and might not prevent the same failure on other characters.
- Added `docs/CHARACTER_ANIMATION_GLOBAL_QA.md`.
- The new global QA applies to all partners, evolutions, and cross evolutions.
- Required before any future character animation generation:
  - character-specific motion brief
  - body type / locomotion / moving parts / fixed parts / attack origin / NG conditions
  - separate RUN and ATTACK GIF review
  - before/after proof for any claimed fix
  - no runtime candidate status until user approval
- Updated `docs/CHARACTER_PIXEL_ART_PIPELINE.md`, `docs/POSTMORTEM_20260601_CHARACTER_ANIMATION_QA_FAILURE.md`, and `REMAINING_TASKS.md` to reference the global QA.

### Recent session (Codex secretary-review workflow for character assets, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User clarified the intent: reduce back-and-forth and wasted outputs; if anything is concerning, confirm before presenting; preempt likely team/Claude feedback and have a secretary reviewer report it.
- Spawned a secretary-review sub-agent to critique the current QA docs. Key feedback:
  - current docs had direction but not enough stopping rules
  - concern remaining -> do not present as fixed
  - require RUN/ATTACK/smaller-size/Before-After review
  - define secretary review before user presentation
  - stop after repeated internal NG instead of generating endless weak GIFs
- Added `docs/CHARACTER_SECRETARY_REVIEW_PROTOCOL.md`.
- Updated character QA docs so future character work must pass:
  - character motion brief
  - RUN/MOVE single GIF
  - ATTACK single GIF
  - 64/96/128px shrink test
  - Before/After proof
  - transparency check
  - secretary review
  - explicit `ROUGH_REVIEW` / `MOTION_REVIEW` / `RUNTIME_CANDIDATE` / `REJECTED` status
- Rule: if the same issue gets two internal NGs, stop generation and ask briefly rather than spending more tokens.

### Recent session (Codex Cobalt motion brief and secretary review, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added `docs/CHARACTER_BRIEF_COBALT_MOTION_20260601.md`.
- Scope is deliberately narrow: preserve the existing good Cobalt Pup pixel-art look, and fix only RUN rear-leg readability plus ATTACK mouth-origin readability.
- Ran a secretary review before continuing. Review passed the direction but requested more concrete frame planning.
- Updated the brief with:
  - RUN 6-frame foot-contact plan
  - ATTACK 5-frame charge/fire/recoil/return plan
  - exact Before GIF references
  - `MOTION_REVIEW` output naming rule
  - save path candidate `Assets/ArtSource/CobaltPixelAnim_20260601/motion_review_20260601/`
  - explicit reminder that this brief is not a Unity hook-up instruction
- Next Cobalt output should be only:
  - RUN single GIF with diagonal rear-leg gait visible
  - ATTACK single GIF with projectile origin touching the mouth/muzzle
- Even if the next output passes internal review, it remains `MOTION_REVIEW` until the user approves. Do not connect it to runtime.

### Recent session (Codex Cobalt MOTION_REVIEW v1 generation, 2026-06-01)
- User said there would be no Claude conflict until 19:00, but Codex still did not edit `Assets/Scripts/CoreLanternGame.cs` in this pass.
- Used the image generation skill to create a new Cobalt Pup sprite-sheet review with:
  - top row: 6 RUN frames
  - bottom row: 5 ATTACK frames
  - magenta chroma-key background
  - attack aimed from mouth/muzzle
- Added `Tools/BuildCobaltMotionReviewFromGeneratedSheet.ps1`.
- Built review outputs under `Assets/ArtSource/CobaltPixelAnim_20260601/motion_review_20260601/`:
  - `CobaltPup_MOTION_REVIEW_v1_RUN.gif`
  - `CobaltPup_MOTION_REVIEW_v1_ATTACK.gif`
  - `CobaltPup_MOTION_REVIEW_v1_sheet_transparent.png`
  - `CobaltPup_MOTION_REVIEW_v1_sheet_chromakey.png`
  - `CobaltPup_MOTION_REVIEW_v1_shrink_check.png`
  - `CobaltPup_MOTION_REVIEW_v1_before_after.png`
  - `frames/CobaltPup_MOTION_REVIEW_v1_RUN_*.png`
  - `frames/CobaltPup_MOTION_REVIEW_v1_ATTACK_*.png`
  - `README.md`
- Validation:
  - transparent sheet corners alpha `[0, 0, 0, 0]`
  - initial ATTACK crop had neighbor-frame contamination; crop detection was fixed and outputs regenerated
  - secretary review found no fatal stop condition
- Secretary review caveat:
  - RUN is improved and rear/front leg position difference is visible
  - ATTACK reads as muzzle-origin rather than chest/paw-origin
  - At 64px, ATTACK charge may still read slightly face-lower/chest-near because the glow expands downward
- Status: `MOTION_REVIEW` only. It is not a runtime candidate and must not be copied to `Assets/Resources/Skins/` until the user approves.

### Recent session (Codex Ember Drake MOTION_REVIEW v1 generation, 2026-06-01)
- Read this handoff and the imagegen skill before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User liked the Cobalt review and asked whether the workflow could be tried with other characters.
- Added `docs/CHARACTER_BRIEF_EMBER_MOTION_20260601.md`.
- Added generalized review builder `Tools/BuildCharacterMotionReviewFromGeneratedSheet.ps1`.
- Generated Ember Drake review assets under `Assets/ArtSource/EmberPixelAnim_20260601/motion_review_20260601/`:
  - `EmberDrake_MOTION_REVIEW_v1_RUN.gif`
  - `EmberDrake_MOTION_REVIEW_v1_ATTACK.gif`
  - `EmberDrake_MOTION_REVIEW_v1_sheet_transparent.png`
  - `EmberDrake_MOTION_REVIEW_v1_sheet_chromakey.png`
  - `EmberDrake_MOTION_REVIEW_v1_sheet_transparent_clean.png`
  - `EmberDrake_MOTION_REVIEW_v1_shrink_check.png`
  - `EmberDrake_MOTION_REVIEW_v1_before_after.png`
  - `frames/EmberDrake_MOTION_REVIEW_v1_RUN_*.png`
  - `frames/EmberDrake_MOTION_REVIEW_v1_ATTACK_*.png`
  - `README.md`
- Validation:
  - transparent sheet corners alpha `[0, 0, 0, 0]`
  - secretary review found no fatal stop condition
- Secretary review:
  - orange small drake, black horns/back spikes, and green eyes are preserved
  - ATTACK fireball reads as mouth/nose-origin, not chest/claw/screen-origin
  - RUN reads as a low bouncing drake run
  - caveat: 64px is suitable for motion review, but not fine-detail review
  - caveat: compared with the current static skin, the new version is more horizontal/long-bodied
- Status: `MOTION_REVIEW` only. Do not connect to runtime until the user approves.

### Recent session (Codex Egg/Core VISUAL_REVIEW v1, 2026-06-01)
- Read this handoff and the imagegen skill before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User asked to make the egg/core more important because it is the face of the game.
- Added `docs/EGG_CORE_VISUAL_BRIEF_20260601.md`.
- Generated A/B/C egg-core visual candidates under `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/`:
  - `EggCore_VISUAL_REVIEW_v1_candidates_chromakey.png`
  - `EggCore_VISUAL_REVIEW_v1_candidates_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_A_SacredDataEgg_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_B_ArmoredCoreEgg_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_C_HatchProtocolEgg_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_shrink_check.png`
  - `EggCore_VISUAL_REVIEW_v1_title_scale_preview.png`
- Validation:
  - transparent board and A/B/C crops have corner alpha `[0, 0, 0, 0]`
  - shrink check covers 96px / 128px / 192px
- Secretary review:
  - A `SacredDataEgg` is the recommended base for the game face/title identity
  - B is visually strong but too armored/noisy and the `ArmoredCoreEgg` label risks existing-IP association
  - C has hatch/evolution emotion but internal motif weakens at 96px
  - next pass should create an A-based low-noise implementation candidate with reduced rings/particles
- Status: `VISUAL_REVIEW` only. Do not copy to `Assets/Resources/Skins/` or connect to runtime until the user approves.

### Recent session (Codex A Egg/Core selected and runtime image replaced, 2026-06-01)
- User selected candidate A for now and asked to keep B/C data for possible later use.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Updated `Assets/Resources/Skins/Lantern.png` to the A-selected 384x384 transparent data egg/core image.
- Updated existing center support resources, also without code edits:
  - `Assets/Resources/Skins/Floor_CoreMark_A.png`
  - `Assets/Resources/Skins/Core_Platform.png`
  - `Assets/Resources/Skins/Core_RingOuter.png`
  - `Assets/Resources/Skins/Core_RingInner.png`
- Backed up the previous runtime image to:
  - `Assets/ArtSource/EggCoreVisual_20260601/selected_A_runtime_candidate_20260601/Lantern_PRE_ASelected_20260601.png`
- Backed up previous center support sprites to:
  - `Assets/ArtSource/EggCoreVisual_20260601/core_map_support_A_20260601/*_PRE_ASelected_20260601.png`
- Kept B/C candidates under:
  - `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/`
- Added selected-A outputs:
  - `EggCore_ASelected_v1_clean_source.png`
  - `Lantern_ASelected_v1_RUNTIME_CANDIDATE.png`
  - `Title_CoreEgg_ASelected_v1_RUNTIME_CANDIDATE.png`
  - `Lantern_ASelected_v1_comparison_shrink_check.png`
  - `README.md`
- Added center-support preview:
  - `Assets/ArtSource/EggCoreVisual_20260601/core_map_support_A_20260601/CoreMapSupport_ASelected_v1_preview.png`
- Secretary review passed:
  - central core identity is stronger than the previous green-ring version
  - 64px loses small pedestal/shell details, but gold egg silhouette and cyan center light remain readable
  - no stop condition for stretching, ratio distortion, white-dot noise, or existing-IP likeness
  - center support sprites match A's gold/cyan tone; only caveat is that the outer ring may draw too much attention if existing Unity glow/tint stacking is strong in Play mode
- Debug/validation:
  - `Tools/CheckCompileHealth.ps1` full: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/TestAudioResources.ps1`: errors 0; only optional legacy boss BGM fallback warnings remain
  - `Tools/ReportAssetStatus.ps1`: static sprite refs missing 0
- Rollback if user dislikes it: copy `Lantern_PRE_ASelected_20260601.png` back to `Assets/Resources/Skins/Lantern.png`.
  For the center support sprites, copy the matching `*_PRE_ASelected_20260601.png` files back to `Assets/Resources/Skins/`.

### Recent session (Codex A Egg/Core title-result support, 2026-06-01)
- Read this handoff before work.
- User said Claude is available again, so the normal restriction is restored: Codex must not edit `Assets/Scripts/CoreLanternGame.cs`.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Added three selected-A egg presentation resources:
  - `Assets/Resources/Skins/Title_CoreEgg_ASelected_v1.png` (768x768)
  - `Assets/Resources/Skins/Result_CoreEgg_ASelected_v1.png` (512x512)
  - `Assets/Resources/Skins/Icon_CoreEgg_ASelected_v1.png` (256x256)
- Art-source copies and preview:
  - `Assets/ArtSource/EggCoreVisual_20260601/title_result_support_A_20260601/`
  - `TitleResultCoreEgg_ASelected_v1_preview.png`
  - `validation_report.txt`
- Validation:
  - all three PNGs have transparent corners (`alpha=0`)
  - all three PNGs have `.meta` files
- Secretary review:
  - OK for Claude handoff
  - no ratio distortion or stretched look
  - 64/96/128px still reads as "gold egg + cyan core"
  - caution: do not put title/progress text directly over the title asset's faint circular field
- Follow-up validation:
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/TestAudioResources.ps1`: errors 0; only optional legacy boss BGM fallback warnings remain
  - `Tools/ReportAssetStatus.ps1`: static sprite refs missing 0; HUD9 waiting 8; Stage4/5 support waiting 6
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- Added `docs/CLAUDE_A_EGG_PRESENTATION_HOOK_SPEC.md`.
- Status: `Claude接続待ち`. Claude should hook these with `preserveAspect = true`, avoid text/button overlap, and keep the reduced title-screen direction intact.

### Recent session (Codex non-character asset queue, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User asked for non-conflicting work and confirmed there were tasks besides character generation.
- Added `docs/NON_CHARACTER_ASSET_QUEUE_20260601.md` to summarize safe non-character remaining work in Japanese.
- Added `Tools/GenerateNonCharacterQueuePreview.ps1`.
- Generated visual queue preview:
  - `Assets/ArtSource/NonCharacterQueue_20260601/NonCharacterAssetQueue_20260601.png`
  - `Assets/ArtSource/NonCharacterQueue_20260601/README.md`
- Updated `Tools/ReportAssetStatus.ps1` so it now explicitly reports A Egg presentation assets:
  - `Present 3 / Hooked 0 / Waiting 3`
- Latest non-character waiting snapshot:
  - HUD9 waiting 8
  - Stage4/5 support waiting 6
  - A Egg presentation waiting 3
  - V2 UI waiting 14
- Validation:
  - `Tools/ReportAssetStatus.ps1`: static sprite refs missing 0
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`

### Recent session (Codex audio comfort pass, 2026-06-01)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User asked to handle remaining non-character work; Codex prioritized the audio issue because the user had reported noisy multi-hit SE and an uncomfortable rising-tone BGM/SE feeling.
- Updated `Tools/GenerateStarterAudio.ps1` and regenerated all placeholder audio under `Assets/Resources/Audio/`.
- Comfort changes:
  - lowered high-frequency/noise content in `Shoot`, `Hit`, and `Kill`
  - made `Pickup`, `LevelUp`, `Evolve`, `Fusion`, and `Boss` less piercing/upward
  - reworked Stage1-5 and boss BGM toward lower, steadier, non-ascending loops
- Added legacy fallback boss BGM aliases:
  - `Assets/Resources/Audio/BGM_BossPulswyrm.wav`
  - `Assets/Resources/Audio/BGM_BossNullwyrm.wav`
- Updated `Assets/Resources/Audio/AudioManifest.json` to 18 entries and refreshed `Assets/Resources/Audio/Licenses/StarterAudio_License.txt`.
- Added Japanese note: `docs/AUDIO_COMFORT_PASS_20260601.md`.
- Updated `docs/AUDIO_LICENSE_LOG_TEMPLATE.md`, `ASSET_REVIEW_REPORT.md`, and `REMAINING_TASKS.md`.
- Validation:
  - `Tools/TestAudioResources.ps1`: warnings 0 / errors 0
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- Next audio check: user should try Stage1/2 and Wave10 boss. If rapid-hit sound is still fatiguing, Claude should add SE playback throttling/priority in `CoreLanternGame.cs`; Codex should not do that code hook-up while Claude owns the file.

### Recent session (Codex partner L0 pixel-art redesign, 2026-06-02)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Read `docs/CODEX_CHARACTER_PIXELART_SPEC_20260601.md` and used the imagegen workflow for a new high-resolution pixel-art-style base partner set.
- Replaced only PNG files at the existing runtime paths:
  - `Assets/Resources/Skins/Partner_S1_L0.png` Cobalt Pup / blue cyber wolf pup
  - `Assets/Resources/Skins/Partner_S2_L0.png` Ember Drake / orange cyber drake
  - `Assets/Resources/Skins/Partner_S3_L0.png` Sage Hare / green cyber hare
  - `Assets/Resources/Skins/Partner_S4_L0.png` Hex Cat / purple dark cyber cat
  - `Assets/Resources/Skins/Partner_S5_L0.png` Drift Fox / pink cyber fox
  - `Assets/Resources/Skins/Partner_S6_L0.png` Iron Bear / black armored bear
- Existing `.meta` files were intentionally preserved. PNGs only were overwritten to avoid the previous `.meta` corruption issue.
- Built assets with:
  - `Tools/BuildPartnerL0PixelArtFromSheet.ps1`
  - `Tools/PolishPartnerL0ColorIdentity.ps1`
- ArtSource and review outputs:
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_clean_transparent.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/validation_report.json`
  - `Assets/ArtSource/CharacterPixelArt_20260601/color_identity_polish_20260602/color_identity_polish_report.json`
- Backups:
  - `Assets/ArtSource/CharacterPixelArt_20260601/backup_before_overwrite_20260602_142911/`
  - `Assets/ArtSource/CharacterPixelArt_20260601/color_identity_polish_20260602/backup_before_polish_20260602_143616/`
- Secretary-style review:
  - S1/S2/S3/S6 passed identity on first visual check.
  - S4 and S5 were readable but slightly weak as "purple cat" and "pink fox", so Codex color-polished them before reporting.
  - Drift Fox now reads as fox, not bird; no beak/crest/wing/talon issue.
  - At 64px, Hex Cat is intentionally dark but still cat-shaped.
- Added Claude title hook-up guidance:
  - `docs/CLAUDE_TITLE_6PARTNER_LINEUP_HOOK_SPEC.md`
- Validation:
  - all six runtime PNGs are 512x512, transparent corners `[0,0,0,0]`, normalized foot baseline `bottom_y=448`
  - S1-S6 `.meta` files end with byte `0A` / `EndsWithLF=True`
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- Status: runtime base partner images are replaced through existing filenames. Title screen six-partner layout remains Claude hook-up work.

### Recent session (Codex Hex Cat opacity/readability fix, 2026-06-02)
- Read this handoff before work.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- User reported that the purple character looked like its color disappeared through transparency.
- Fixed `Assets/Resources/Skins/Partner_S4_L0.png` only:
  - raised visible alpha so the purple body no longer fades out on dark backgrounds
  - increased purple readability while preserving the cat silhouette
  - removed a small detached chroma-key/noise component near the right edge
  - realigned the foot baseline to `bottom_y=448`
- Existing `.meta` was not edited. `Partner_S4_L0.png.meta` still ends with byte `0A`.
- Added `Tools/RepairHexCatOpacityAndReadability.ps1`.
- Review outputs:
  - `Assets/ArtSource/CharacterPixelArt_20260601/hex_cat_opacity_repair_20260602/HexCat_visibility_check_after.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/hex_cat_opacity_repair_20260602/hex_cat_opacity_repair_report.json`
- Backup:
  - `Assets/ArtSource/CharacterPixelArt_20260601/hex_cat_opacity_repair_20260602/backup_before_hex_opacity_repair_20260602_145358/`
- Validation:
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- Status: Hex Cat PNG is updated through the existing resource filename. Title six-partner layout remains Claude hook-up work.

### Recent session (Codex Drift Fox cleanup/pink readability fix, 2026-06-02)
- Read this handoff before work.
- Read `docs/CODEX_PIXELART_CLEANUP_20260602.md` and followed the S5-only scope.
- Did not edit `Assets/Scripts/CoreLanternGame.cs`.
- Did not edit `Partner_S4_L0.png`; Hex Cat was already fixed and left untouched.
- Fixed `Assets/Resources/Skins/Partner_S5_L0.png` only:
  - removed the left short vertical artifact
  - removed detached white/pink square scraps around the tail/body
  - removed remaining low-alpha detached haze
  - restored the pink fox read by strengthening body alpha and pink-family color visibility
  - preserved the main silhouette and shared title baseline
- Existing `.meta` was not edited. `Partner_S5_L0.png.meta` still ends with byte `0A`.
- Added `Tools/RepairDriftFoxCleanupAndPinkReadability.ps1`.
- Review outputs:
  - `Assets/ArtSource/CharacterPixelArt_20260601/drift_fox_cleanup_20260602/DriftFox_visibility_check_after.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/drift_fox_cleanup_20260602/drift_fox_cleanup_report.json`
- Backup:
  - `Assets/ArtSource/CharacterPixelArt_20260601/drift_fox_cleanup_20260602/backup_before_drift_fox_cleanup_20260602_151155/`
- Validation:
  - `Partner_S5_L0.png`: 512x512 / bbox `[124,120,386,448]` / `bottom_y=448` / corner alpha `[0,0,0,0]`
  - `Partner_S5_L0.png`: alpha>0 connected component count 1
  - `Tools/TestImageResources.ps1`: warnings 0 / errors 0
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- Status: Drift Fox PNG is updated through the existing resource filename. Title six-partner layout remains Claude hook-up work.
