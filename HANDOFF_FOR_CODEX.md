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
