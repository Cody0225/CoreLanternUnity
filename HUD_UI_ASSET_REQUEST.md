# HUD / UI 素材 発注リスト

最終更新: 2026-05-24

このドキュメントは **Codex** 向けの HUD / UI 画像素材の発注書です。
キャラ画像（素体・進化・融合）は別途「キャラ大幅リデザイン仕様書」で扱うため、ここには含めません。

---

## ⚡ 最新の追加発注 (2026-05-24 v2)

各 Stage 専用敵 4種 + ボス専用 sprite 2種を追加発注。
**現状はコード側でフォールバック動作中** (既存敵 sprite / Boss sprite 流用)。

### 🔴 P0 — 各ステージ専用 雑魚敵 sprite 4種

VSのようにステージごとに敵の見た目を変えるため。
コード側は EnemyType に追加済、sprite が来たら自動切り替わる。

| ファイル名 | サイズ | ステージ | 元 / モチーフ |
|---|---|---|---|
| `Enemy_MagmaTitan.png` | 96×96 | Stage 1 (Lava) | Brute 派生、橙の重装、燃焼トレイル感 |
| `Enemy_CorruptionDrone.png` | 96×96 | Stage 2 (Broken Core) | Phantom 派生、紫マゼンタの高速ステルス |
| `Enemy_FrostKnight.png` | 96×96 | Stage 3 (Frost) | Brute 派生、青白の氷装甲 |
| `Enemy_VoltDasher.png` | 96×96 | Stage 4 (Storm) | Dasher 派生、紫黄の電撃トレイル |

### 🔴 P0 — ボス専用 sprite 2種

中ボスと最終ボスを個別化 (現状 Boss.png 共通)。
コード側で `pulswyrmSprite` / `nullwyrmSprite` フィールド追加済、独自攻撃機能も実装済:
- **Pulswyrm**: 5秒毎にプレイヤーへ突進 (0.45秒、命中で 2.2dmg + knockback)
- **Nullwyrm**: 8秒毎にコアを狙う Core Mark (1秒予告 → 直線ダメ。コアに 2.5dmg / プレイヤー直線上 1.5dmg)

| ファイル名 | サイズ | 用途 | モチーフ |
|---|---|---|---|
| `Boss_Pulswyrm.png` | 256×256 | 中ボス (Wave 5) | 機動型、オレンジ&赤、突進する蛇竜感 |
| `Boss_Nullwyrm.png` | 256×256 | 最終ボス (Wave 10) | コア狙い、マゼンタ&紫、威圧的な大型ヘビ |

### 既存追加発注 (Stage 4/5 関連、再掲)

| ファイル名 | サイズ | 用途 |
|---|---|---|
| `Stage4_FrostCrystal_A.png` | 96×96 | Stage 4 凍結クリスタル |
| `Stage4_FrostPatch_A.png` | 256×256 | Stage 4 結霜パッチ |
| `Stage5_LightningMarker_A.png` | 256×256 | Stage 5 落雷警告 |
| `Stage5_LightningStrike_A.png` (任意) | 256×256 | Stage 5 雷撃 VFX |
| `StageThumb_Frost.png` | 16:9 | Stage 4 サムネ |
| `StageThumb_Storm.png` | 16:9 | Stage 5 サムネ |

---

## ⚡ 追加発注 (2026-05-24)

ステージ拡張 (4, 5 追加) + 新キャラ Solar Anchor 実装に伴い、以下の素材を追加で発注。

### 🔴 P0 — Stage 4 / 5 専用ハザード sprite

| ファイル名 | サイズ | 用途 | 説明 |
|---|---|---|---|
| `Stage4_FrostCrystal_A.png` | 96×96 | Stage 4 凍結クリスタル | ダイヤ型結晶、青白〜シアン、HP がある破壊可能オブジェクト感 |
| `Stage4_FrostPatch_A.png` | 256×256 | Stage 4 結霜パッチ | 円形、薄い氷の張りつき、移動低下ゾーン (罰は弱め) |
| `Stage5_LightningMarker_A.png` | 256×256 | Stage 5 落雷警告マーカー | 円形、テレグラフ中は黄→白点滅対応 (Unity 側で alpha 変化、テクスチャは単色推奨) |
| `Stage5_LightningStrike_A.png` (任意) | 256×256 | Stage 5 雷撃 VFX | 縦線の白/紫の閃光、雷撃発動時の追加演出 (なくても動作OK) |

### 🟡 P1 — Stage 4 / 5 サムネ (Run Setup 画面)

| ファイル名 | サイズ | モチーフ |
|---|---|---|
| `StageThumb_Frost.png` | 16:9 (例 384×216) | 青白の氷の風景、凍結クリスタルが並ぶ |
| `StageThumb_Storm.png` | 16:9 (例 384×216) | 紫の嵐、雷撃が落ちる夜空 |

### 🟡 P1 — Solar Anchor 専用 Core Aura ring sprite (任意)

| ファイル名 | サイズ | 用途 |
|---|---|---|
| `Stage_CoreAura_Ring.png` (任意) | 512×512 | Solar Anchor のコア周りに常時光るゴールドリング (現状は `circleSprite` フォールバック、専用素材があれば打感↑) |

### Stage 別フォールバック動作中の状態

- Stage 4 (Frost Vault) / Stage 5 (Storm Spire): サムネ素材なし、Run Setup チップは青/紫の色フォールバックで動作中
- Stage 4 ハザード: `circleSprite` / `diamondSprite` フォールバック中
- Stage 5 落雷マーカー: `circleSprite` フォールバック中

---

## ✅ 既に生成済 (Codex 2026-05-22 Batch 2)

これらは既にコードに接続済み:

- **Stage 2 (Lava Cache)**: `Stage2_LavaPool_A.png` / `Stage2_HeatVent_A.png` / `StageThumb_Lava.png`
- **Stage 3 (Broken Core)**: `Stage3_CorruptionPatch_A.png` / `Stage3_RelayDevice_A.png` / `StageThumb_BrokenCore.png`
- **Stage 1 サムネ**: `StageThumb_Arena.png`
- **モジュールアイコン 20種**: `ModuleIcon_HP/Repair/CoreRepair/Damage/FireRate/Speed/Magnet/Bullet/Pierce/Aura/Chain/Explosion/Reflect/GuardRing/Lifesteal/CoreLight/Wave/DataForge/Boss/Skip.png`
- **追加レリック 4種**: `Relic_Titan/Overdrive/DataSurge/Apex.png`
- **64×64 ピックアップ**: `Pickup_Data_64.png` (HUD 用フォールバック)

---

## 🔴 P0 既存依頼 (まだ無効化中、9-slice 再制作待ち)

### 1. カードフレーム (5色)
進化モジュール・パートナー選択・レリック選択画面で使用。

| ファイル名 | サイズ | 色 |
|---|---|---|
| `Card_Cyan.png` | 320×400 | シアン (SPEED) |
| `Card_Gold.png` | 320×400 | ゴールド (POWER) |
| `Card_Green.png` | 320×400 | グリーン (GUARD) |
| `Card_Magenta.png` | 320×400 | マゼンタ (CROSS) |
| `Card_Red.png` | 320×400 | レッド (HAZARD) |

**仕様:**
- コーナー装飾エリア: 各角 30×30 px
- 内側使用領域: 中央 260×340 px (テキスト配置エリア、ここに装飾禁止)
- ネオン・サイバーパンクスタイル
- **現状**: `UseGeneratedCards = false` で無効化中

### 2. HUD パネルフレーム (5種)
ゲーム画面のステータス表示パネル枠。

| ファイル名 | サイズ | 用途 |
|---|---|---|
| `HUD_Panel_Wave.png` | 250×74 | ウェーブ表示 |
| `HUD_Panel_HP.png` | 270×104 | HP表示 (自分+コア) |
| `HUD_Panel_Level.png` | 270×76 | レベル+EXP表示 |
| `HUD_Panel_ChipMini.png` | 190×42 | データチップ数表示 |
| `HUD_Panel_Loadout.png` | 296×92 | ACTIVE BUILD表示 |

**仕様**: 9-slice 必須、コーナー装飾は 16×16 px
**現状**: `UseGeneratedHudPanels = false` で無効化中

### 3. HUD バー素材

| ファイル名 | サイズ | 色 |
|---|---|---|
| `HUD_Bar_Back.png` | 132×20 | ダーク背景 (9-slice) |
| `HUD_Bar_HP_PlayerFill.png` | 132×16 | シアン グラデ |
| `HUD_Bar_HP_CoreFill.png` | 132×16 | ゴールド グラデ |
| `HUD_Bar_EXP_Fill.png` | 132×16 | グリーン グラデ |
| `HUD_WaveProgress_Frame.png` | 520×26 | フレーム (9-slice) |
| `HUD_WaveProgress_Fill_Normal.png` | 520×22 | シアン |
| `HUD_WaveProgress_Fill_Boss.png` | 520×22 | マゼンタ |

**現状**: `UseGeneratedHudBars = false` で無効化中

---

## 🟡 P1 — 背景画像 (静止画、現状プロシージャル)

### 4. メインメニュー背景
- `Background_MainMenu.png` (1920×1080)
- **モチーフ**: データセンター内部・神秘の光る卵を中心に、青緑のネオン回路

### 5. 進化カットイン背景
- `Background_Evolution.png` (1920×1080)
- **モチーフ**: 進化リング、光のシャワー、エネルギー爆発
- **要件**: 中央に被写体配置用の暗いスポット (シルエットでも可)

### 6. クロス進化カットイン背景
- `Background_CrossEvolution.png` (1920×1080)
- **モチーフ**: 紫マゼンタの大爆発、光の粒子
- **要件**: 進化カットインより派手に

### 7. リザルト背景
- `Background_Victory.png` (1920×1080) - 勝利、明るい青緑系
- `Background_Defeat.png` (1920×1080) - 敗北、暗赤系

### 8. ボスカットイン背景
- `Background_BossPulswyrm.png` (1920×1080) - 中ボス、オレンジ系
- `Background_BossNullwyrm.png` (1920×1080) - ラスボス、マゼンタ系

---

## 🟢 P2 — VFX / 装飾

### 9. 弾スプライト (差し替え)

| ファイル名 | サイズ | スタイル |
|---|---|---|
| `Bullet_Speed.png` | 64×64 | シアン、矢/光線型 |
| `Bullet_Power.png` | 64×64 | オレンジ、ヘビーオーブ型 |
| `Bullet_Guard.png` | 64×64 | グリーン、シールドリング型 |
| `Bullet_Fusion.png` | 64×64 | マゼンタ、複合型 |

各透明背景、中央に弾本体、外側に発光オーラ。

### 10. ピックアップ

| ファイル名 | サイズ | 用途 |
|---|---|---|
| `Pickup_Data.png` | 64×64 | データチップ (既存、ワールド配置用) |
| `Pickup_Data_64.png` | 64×64 | データチップ HUD 用 (既存、Codex 生成済) |
| `Pickup_Heal.png` | 64×64 | HP回復 (新規) |

### 11. エフェクト

| ファイル名 | サイズ | 用途 |
|---|---|---|
| `Effect_LevelUp.png` | 256×256 | 黄リング |
| `Effect_Evolution_Ring.png` | 512×512 | 進化光輪 |
| `Effect_HitFlash.png` | 128×128 | 汎用ヒット閃光 |
| `Effect_Explosion.png` | 256×256 | 爆発 |
| `Effect_Knockback_Ring.png` | 256×256 | GUARDノックバック半径波紋 (Wraith Lynx演出兼用) |

---

## 🟢 P2 — 小アイコン

### 12. リンク仲間アイコン

| ファイル名 | サイズ | 色 / モチーフ |
|---|---|---|
| `Icon_Link_Nova.png` | 64×64 | オレンジ / 火力 |
| `Icon_Link_Bulwark.png` | 64×64 | グリーン / 防衛 |
| `Icon_Link_Siphon.png` | 64×64 | イエロー / 回収 |
| `Icon_Link_Phase.png` | 64×64 | ピンク / 回避 |

### 13. ステータスアイコン

| ファイル名 | サイズ | モチーフ |
|---|---|---|
| `Icon_Stat_HP.png` | 32×32 | ハート |
| `Icon_Stat_ATK.png` | 32×32 | 剣 / 火 |
| `Icon_Stat_SPD.png` | 32×32 | 翼 / 矢印 |
| `Icon_Stat_FIRE.png` | 32×32 | 弾丸 / 連射 |
| `Icon_Stat_SPC.png` | 32×32 | 星 / オーラ |

---

## 🔵 P3 — マップ素材

### 14. 床テクスチャ
- `Floor_DarkBase.png` (256×256, タイル可能) ✅ 生成済
- `Floor_Hazard_Lava.png` (256×256, タイル可能) ✅ 生成済 (Stage 2 で使用中)

### 15. アリーナ縁
- `Arena_Boundary.png` (任意サイズ) - 円周のネオン表現

---

## 含めない項目 (理由付き)

| 項目 | 理由 |
|---|---|
| ミニマップ素材一式 | ミニマップ機能自体を撤去済 (2026-05-21) |
| キャラ素体・進化・融合画像 | 別ドキュメント「キャラ大幅リデザイン仕様書」で扱う (Solar Anchor [style 11] 含む 11キャラ分すべて) |
| BGM / SE | 別カテゴリ (Codex 側で `Tools/GenerateStarterAudio.ps1` で自動生成中) |

---

## 進捗管理

- 完了したファイルは `Assets/Resources/Skins/` に配置
- 配置後、Claude 側で `LoadOptionalSprite` 経由の動作確認
- 問題があれば本ドキュメントの該当項目にメモを追記

## 担当分け

- **Codex**: 画像生成・最適化・配置
- **Claude**: コード側の接続確認・実機での見た目確認・問題報告

---

## 2026-05-24 追加発注の優先順位整理

最終ローンチ向けの優先度。本日 (2026-05-24) 時点の Stage/キャラ拡張で必要な順:

| 順位 | 項目 | 重要度 |
|---|---|---|
| 1 | `Stage4_FrostCrystal_A.png` | Stage 4 の核 (破壊報酬の見た目) |
| 2 | `Stage5_LightningMarker_A.png` | Stage 5 の核 (テレグラフが視認できないと事故死) |
| 3 | `StageThumb_Frost.png` / `StageThumb_Storm.png` | Run Setup でのステージ判別性 |
| 4 | `Stage4_FrostPatch_A.png` | あれば氷の質感UP、なくても動作OK |
| 5 | `Stage_CoreAura_Ring.png` | Solar Anchor 専用、贅沢素材 |
| 6 | `Stage5_LightningStrike_A.png` | 雷撃発動時のVFX、なくても動作OK |
