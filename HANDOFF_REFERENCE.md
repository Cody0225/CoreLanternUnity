# CoreLanternUnity — Codex 不変リファレンス

`HANDOFF_FOR_CODEX.md` から分離した、**一度読めば十分な不変情報**。
プロジェクト概要・設計判断・ゲームデザイン要点・操作・コード地図など、頻繁には変わらない内容をまとめる。

- **毎回読む必要はない。** 現在の絶対ルール・役割分担・進行中タスク・残タスクは `HANDOFF_FOR_CODEX.md` 本体にある。
- 過去の作業履歴は `HANDOFF_ARCHIVE.md`。
- 内容は元の `HANDOFF_FOR_CODEX.md` から移設（情報は不変、削除なし）。

---

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
