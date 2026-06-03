# Asset Hookup Map — Codex 向けコード接続情報

**作成日**: 2026-05-28
**用途**: Codex が生成した素材が「コード上のどこから読まれるか」を一覧化。
**読者**: Codex (主) → どこに置けば自動で使われるかが一発で分かる

---

## 🎯 読み方

| ヘッダ | 意味 |
|---|---|
| **ファイル名** | Codex が生成すべきファイル名 (規則準拠) |
| **配置先** | このフォルダに置けば自動でロードされる |
| **接続コード** | CoreLanternGame.cs の該当行 (参考) |
| **フォールバック** | 未生成時に何が使われるか |

⚠ ファイル名規則を 1 文字でも間違えると **自動でロードされず、フォールバックが使われ続ける**。

---

## 1. キャラクター画像

### 素体 (L0)
| ファイル名 | 配置先 | 接続コード | フォールバック |
|---|---|---|---|
| `Partner_S1_L0.png` | `Assets/Resources/Skins/` | `LoadOptionalSprite("Skins/Partner_S1_L0", ...)` | procedural |
| `Partner_S2_L0.png` 〜 `Partner_S11_L0.png` | 同上 | 同 pattern | procedural |

### 進化ルート (L1-L3)
| ファイル名パターン | 例 | フォールバック |
|---|---|---|
| `Partner_S{n}_R{r}_L{l}.png` | `Partner_S1_R1_L3.png` (Cobalt SPEED L3) | 1 段下 L レベル |

n: 1-11 (species)、r: 1=SPEED, 2=POWER, 3=GUARD、l: 1, 2, 3

### クロス進化 (F1-F6)
| ファイル名パターン | 例 | フォールバック |
|---|---|---|
| `Partner_S{n}_F{f}_L{l}.png` | `Partner_S1_F1_L3.png` (Nova Aegis L3) | F{f} の素体 |

f: 1=Nova Aegis, 2=Photon Siphon, 3=Core Bastion, 4=Nova Phantom, 5=Aegis Drift, 6=Photon Wraith

---

## 2. 敵キャラ画像

| ファイル名 | 配置先 | フォールバック (procedural) |
|---|---|---|
| `Runner.png` | `Assets/Resources/Skins/` | `MakeRunnerSprite()` |
| `Brute.png` | 同上 | `MakeBruteSprite()` |
| `Shooter.png` | 同上 | `MakeShooterSprite()` |
| `Dasher.png` | 同上 | `MakeDasherSprite()` |
| `Bomber.png` | 同上 | `MakeBomberSprite()` |
| `Phantom.png` | 同上 | `MakePhantomSprite()` |
| `LavaCrawler.png` | 同上 | `MakeLavaCrawlerSprite()` ✅ Codex 完了 |
| `Enemy_MagmaTitan.png` | 同上 | `bruteSprite` ✅ Codex 完了 |
| `Enemy_CorruptionDrone.png` | 同上 | `phantomSprite` ✅ Codex 完了 |
| `Enemy_FrostKnight.png` | 同上 | `bruteSprite` ✅ Codex 完了 |
| `Enemy_VoltDasher.png` | 同上 | `dasherSprite` ✅ Codex 完了 |

## 3. ボス画像

| ファイル名 | 配置先 | 接続 | フォールバック |
|---|---|---|---|
| `Boss.png` | `Assets/Resources/Skins/` | 共通 `bossSprite` | `MakeBossSprite()` |
| `Boss_Pulswyrm.png` | 同上 | Wave 5 中ボス専用 ✅ Codex 完了 | `bossSprite` |
| `Boss_Nullwyrm.png` | 同上 | Wave 10 最終ボス専用 ✅ Codex 完了 | `bossSprite` |

⚠ Stage 1 のみ、Pulswyrm/Nullwyrm は共通 `bossSprite` を使用 (ユーザー判断、2026-05-28 Codex 対応済)

---

## 4. ステージハザード

| ファイル名 | 配置先 | 用途 |
|---|---|---|
| `Floor_Hazard_Lava.png` | `Assets/Resources/Skins/` | Stage 1 溶岩プール ✅ |
| `Stage2_Lava_*` | 同上 | Stage 2 (Lava Cache 旧呼称) |
| `Stage2_HeatVent_*` | 同上 | Stage 2 熱噴出口 |
| `Stage3_CorruptionPatch_*` | 同上 | Stage 3 汚染パッチ |
| `Stage3_RelayDevice_*` | 同上 | Stage 3 リレー装置 |
| `Stage4_FrostCrystal_A.png` | 同上 | Stage 4 凍結クリスタル ✅ Codex 完了 |
| `Stage4_FrostPatch_A.png` | 同上 | Stage 4 結霜パッチ ✅ Codex 完了 |
| `Stage5_LightningMarker_A.png` | 同上 | Stage 5 落雷予告 ✅ Codex 完了 |
| `Stage5_LightningStrike_A.png` | 同上 | Stage 5 落雷発動 ✅ Codex 完了 |

---

## 5. 背景・カットイン

| ファイル名 | 配置先 | 用途 |
|---|---|---|
| `Background_MainMenu.png` | `Assets/Resources/Skins/` | メインメニュー (1920×1080) |
| `Background_Evolution.png` | 同上 | 進化カットイン |
| `Background_CrossEvolution.png` | 同上 | クロス進化カットイン |
| `Background_BossPulswyrm.png` | 同上 | Pulswyrm 出現カットイン |
| `Background_BossNullwyrm.png` | 同上 | Nullwyrm 出現カットイン |
| `Background_Victory.png` | 同上 | 勝利リザルト |
| `Background_Defeat.png` | 同上 | 敗北リザルト |
| `Cutin_Evolve_*.png` | 同上 | 進化エフェクト |

---

## 6. ステージサムネ (Stage 選択画面)

| ファイル名 | 配置先 | 用途 |
|---|---|---|
| `StageThumb_Arena.png` | `Assets/Resources/Skins/` | Stage 0 (Lantern Field) |
| `StageThumb_Lava.png` | 同上 | Stage 1 (Lava Cache) |
| `StageThumb_BrokenCore.png` | 同上 | Stage 2 (Broken Core Network) |
| `StageThumb_Frost.png` | 同上 | Stage 3 (Frost Vault) ✅ Codex 完了 |
| `StageThumb_Storm.png` | 同上 | Stage 4 (Storm Spire) ✅ Codex 完了 |

サイズ: 160×90 px

---

## 7. リンク・進化ルートアイコン

| ファイル名 | 配置先 | 用途 |
|---|---|---|
| `Link_Nova.png` | `Assets/Resources/Skins/` | Nova リンク |
| `Link_Bulwark.png` | 同上 | Bulwark リンク |
| `Link_Siphon.png` | 同上 | Siphon リンク |
| `Link_Phase.png` | 同上 | Phase リンク |
| `Route_SPEED.png` | 同上 | SPEED ルート |
| `Route_POWER.png` | 同上 | POWER ルート |
| `Route_GUARD.png` | 同上 | GUARD ルート |
| `Fusion_1.png` 〜 `Fusion_6.png` | 同上 | 6 種クロス進化 (Codex 既存仮版あり) |

---

## 8. HUD 大型素材 (9-slice)

未着手。`docs/VISUAL_STYLE_GUIDE.md` の HUD セクション参照。
コード側で `UseGeneratedHudBars = true` / `UseGeneratedHudPanels = true` で有効化。

| ファイル名 | サイズ | 9-slice border |
|---|---|---|
| `HUD_Panel_Wave.png` | 512×512 | 24, 24, 24, 24 |
| `HUD_Panel_HP.png` | 512×512 | 24, 24, 24, 24 |
| `HUD_Panel_Level.png` | 512×512 | 24, 24, 24, 24 |
| `HUD_Panel_ChipMini.png` | 512×512 | 24, 24, 24, 24 |
| `HUD_Bar_Back.png` | 256×64 | 8, 12, 8, 12 |
| `HUD_Bar_HP_PlayerFill.png` | 256×64 | 8, 12, 8, 12 |
| `HUD_Bar_HP_CoreFill.png` | 256×64 | 8, 12, 8, 12 |
| `HUD_Bar_EXP_Fill.png` | 256×64 | 8, 12, 8, 12 |
| `HUD_WaveProgress_Frame.png` | 512×48 | 8, 16, 8, 16 |
| `HUD_WaveProgress_Fill_Normal.png` | 512×48 | 8, 16, 8, 16 |
| `HUD_WaveProgress_Fill_Boss.png` | 512×48 | 8, 16, 8, 16 |

---

## 8.5. Codex V2 実寸UI素材 (2026-05-29/30)

旧 512×512 汎用パネルを広範囲に引き伸ばすと画質・比率が崩れるため、Codex は現在のUI Rectに合わせた実寸PNGを追加している。

⚠ これらは 9-slice 汎用素材ではなく、原則 **Native Size / 均一スケールのみ** で使う。横だけ・縦だけの比率変更は禁止。

### HUD panel v2

| ファイル名 | サイズ | 想定用途 |
|---|---:|---|
| `HUD_Panel_Wave_v2.png` | 250×74 | Top-left wave HUD |
| `HUD_Panel_HP_v2.png` | 270×104 | Player/core HP HUD |
| `HUD_Panel_Level_v2.png` | 270×76 | Level/EXP HUD |
| `HUD_Panel_ChipMini_v2.png` | 190×42 | Compact data chip HUD |
| `HUD_Panel_Data_v2.png` | 250×54 | Data panel HUD |
| `HUD_Panel_Loadout_v2.png` | 296×92 | Active build/loadout HUD |
| `HUD_Panel_Stats_v2.png` | 286×156 | Combat stats HUD |
| `HUD_Panel_Links_v2.png` | 360×228 | Links/status HUD |
| `HUD_Panel_EventLog_v2.png` | 520×70 | Event log HUD |

生成: `Tools/GenerateHudPanelV2Assets.ps1`
プレビュー: `Assets/ArtSource/HUD_Panel_V2_20260529/HUD_Panel_V2_Preview.png`

### Card part v2

| ファイル名 | サイズ | 想定用途 |
|---|---:|---|
| `Card_Header_Basic_v2.png` | 252×34 | Basic card header plate |
| `Card_Header_Rare_v2.png` | 252×34 | Rare card header plate |
| `Card_Header_Epic_v2.png` | 252×34 | Epic/special card header plate |
| `Card_TitlePlate_v2.png` | 248×36 | Card title background |
| `Card_LevelPlate_v2.png` | 240×22 | Card level/progression background |
| `Card_RarityPlate_Basic_v2.png` | 178×28 | Basic rarity plate |
| `Card_RarityPlate_Rare_v2.png` | 178×28 | Rare rarity plate |
| `Card_RarityPlate_Epic_v2.png` | 178×28 | Epic rarity plate |
| `Card_BottomRail_Cyan_v2.png` | 210×8 | Card bottom rail |
| `Card_BottomRail_Gold_v2.png` | 210×8 | Special card bottom rail |
| `Card_SpecialCorner_Epic_v2.png` | 40×40 | Epic marker |
| `Card_SpecialCorner_Cross_v2.png` | 40×40 | Cross/fusion marker |

生成: `Tools/GenerateCardPartV2Assets.ps1`
プレビュー: `Assets/ArtSource/Card_Parts_V2_20260529/Card_Parts_V2_Preview.png`

### Button v2

| ファイル名 | サイズ | 想定用途 |
|---|---:|---|
| `Button_Primary_Start_v2.png` | 420×60 | Main menu large start button |
| `Button_MenuSecondary_v2.png` | 150×44 | Main menu secondary nav |
| `Button_RunStageChip_v2.png` | 180×108 | Run setup stage chip |
| `Button_DangerChip_v2.png` | 80×56 | Run setup danger chip |
| `Button_RunBack_v2.png` | 220×60 | Run setup back |
| `Button_RunStart_v2.png` | 280×60 | Run setup start |
| `Button_PauseResume_v2.png` | 320×48 | Pause resume |
| `Button_Wide_320x42_v2.png` | 320×42 | Pause/options wide button |
| `Button_OptionsToggle_v2.png` | 250×36 | Options toggle |
| `Button_Stepper_v2.png` | 44×30 | Options +/- |
| `Button_Reroll_v2.png` | 440×52 | Upgrade reroll |
| `Button_SkipReward_v2.png` | 300×52 | Upgrade skip |
| `Button_ResultRetry_v2.png` | 340×64 | Result retry |
| `Button_ResultMenu_v2.png` | 240×50 | Result main menu |
| `Button_CloseSmall_v2.png` | 240×32 | Codex/tree close |
| `Button_CloseWide_v2.png` | 260×38 | Wider close/back |

生成: `Tools/GenerateButtonV2Assets.ps1`
プレビュー: `Assets/ArtSource/Button_V2_20260530/Button_V2_Preview.png`

### Title UI v2

| ファイル名 | サイズ | 想定用途 |
|---|---:|---|
| `Title_CoreEmblem_v2.png` | 512×512 | Main menu central core emblem |
| `Title_SubtitlePlate_v2.png` | 580×38 | Subtitle backing plate |
| `Title_StatsRibbon_v2.png` | 820×54 | Main menu stat ribbon backing |
| `Title_LogoUnderline_v2.png` | 620×22 | Title underline accent |
| `Title_NavRail_v2.png` | 960×24 | Menu nav rail accent |
| `Title_CornerAccent_v2.png` | 160×160 | Lightweight corner accent |

生成: `Tools/GenerateTitleUiV2Assets.ps1`
プレビュー: `Assets/ArtSource/Title_UI_V2_20260530/Title_UI_V2_Preview.png`

### Result UI v2

| ファイル名 | サイズ | 想定用途 |
|---|---:|---|
| `Result_DeckFrame_v2.png` | 960×580 | Result main deck frame |
| `Result_PortraitFrame_v2.png` | 220×220 | Final partner portrait frame |
| `Result_StatTile_v2.png` | 420×56 | Result stat tile (wide replacement candidate) |
| `Result_BadgeStrip_Route_v2.png` | 220×44 | Route badge strip |
| `Result_BadgeStrip_Fusion_v2.png` | 220×44 | Fusion/cross badge strip |
| `Result_MvpRow_v2.png` | 230×68 | MVP build row |
| `Result_SummaryPlate_v2.png` | 520×44 | Compact run summary plate |
| `Result_FooterGuide_v2.png` | 520×28 | Keyboard guide/footer plate |
| `Result_RankMedal_S_v2.png` | 128×128 | S rank medal |
| `Result_RankMedal_A_v2.png` | 128×128 | A rank medal |
| `Result_RankMedal_B_v2.png` | 128×128 | B rank medal |
| `Result_RankMedal_C_v2.png` | 128×128 | C rank medal |
| `Result_RankMedal_D_v2.png` | 128×128 | D rank medal |

生成: `Tools/GenerateResultV2Assets.ps1`
プレビュー: `Assets/ArtSource/Result_V2_20260530/Result_V2_Preview.png`

実装注意:
- テキストは画像に焼き込まず、Unity側の `Text` を上に置く。
- `Image.preserveAspect = true` を優先。
- 既存テキスト/ボタンの上に装飾を置かない。装飾は原則 sibling index を下げる。
- Unity Editor 実機確認で 16:9 / Free Aspect の文字はみ出し・重なりを必ず見る。

---

## 9. ピックアップ・モジュールアイコン

| ファイル名 | 配置先 | 用途 |
|---|---|---|
| `Pickup_Data.png` | `Assets/Resources/Skins/` | データチップ |
| `Pickup_Data_64.png` | 同上 | UI 用小サイズ |
| `Pickup_Heal.png` | 同上 | HP 回復ピックアップ |
| `ModuleIcon_*.png` (20 種) | 同上 | モジュール識別アイコン |
| `HUD_Icon_DataChip.png` | 同上 | HUD のデータチップ表示 |

---

## 10. オーディオ

### BGM (`Assets/Resources/Audio/`)
| ファイル名 | 接続 | 用途 |
|---|---|---|
| `BGM.wav` | `LoadAudioClip("BGM")` | メインメニュー + 通常 Wave (Stage 0) |
| `BGM_Stage2.wav` | `LoadAudioClip("BGM_Stage2")` | Lava Cache |
| `BGM_Boss_Pulswyrm.wav` | `LoadAudioClip("BGM_Boss_Pulswyrm")` | Wave 5 中ボス戦 ✅ Codex 仮版 |
| `BGM_Boss_Nullwyrm.wav` | `LoadAudioClip("BGM_Boss_Nullwyrm")` | Wave 10 ラスボス戦 ✅ Codex 仮版 |

将来追加候補:
- `BGM_Stage3.wav` (Broken Core)
- `BGM_Stage4.wav` (Frost Vault)
- `BGM_Stage5.wav` (Storm Spire)
- `BGM_Victory.wav` / `BGM_Defeat.wav` (ジングル)

### SE (`Assets/Resources/Audio/`)
| ファイル名 | 接続 | 用途 |
|---|---|---|
| `Shoot.wav` | `PlaySfx("Shoot", ...)` | プレイヤー射撃 |
| `Hit.wav` | `PlaySfx("Hit", ...)` | 敵命中 |
| `Kill.wav` | `PlaySfx("Kill", ...)` | 敵撃破 |
| `Pickup.wav` | `PlaySfx("Pickup", ...)` | データ拾得 / UI 確定 |
| `LevelUp.wav` | `PlaySfx("LevelUp", ...)` | レベルアップ |
| `Evolve.wav` | `PlaySfx("Evolve", ...)` | 進化カットイン |
| `Fusion.wav` | `PlaySfx("Fusion", ...)` | クロス進化カットイン |
| `Boss.wav` | `PlaySfx("Boss", ...)` | ボス警告・出現 |
| `GameOver.wav` | `PlaySfx("GameOver", ...)` | 敗北 |

---

## 11. Steam ストア用カプセル画像

`Assets/ArtSource/SteamCapsules/` に配置 (Resources 配下ではない)。
Unity 内では使われない。Steamworks Portal にアップロードする素材。

詳細は `docs/CODEX_RELEASE_ORDER.md` の P0-2 参照。

---

## 12. itch.io ページ画像

`Assets/ArtSource/ItchIo/` に配置。

| ファイル名 | サイズ | 用途 |
|---|---|---|
| `ItchIo_Cover.png` | 630×500 | itch.io ヘッダー |
| `ItchIo_Screenshot_01-08.png` | 1920×1080 | ストアページスクショ |

---

## 🔄 ロード処理の流れ (Codex 向け理解)

1. ゲーム起動時に `LoadOptionalSprite("Skins/X.png", fallback)` が呼ばれる
2. `Assets/Resources/Skins/X.png` が存在すれば、それを使う
3. 存在しなければ `fallback` (procedural または別の sprite) を使う
4. つまり、**ファイル名さえ正しければ、コード変更なしで自動切替される**

オーディオも同様: `Assets/Resources/Audio/X.wav` を置けば `LoadAudioClip("X")` が拾う。

---

## 📋 Codex 作業時のチェックリスト

各アセット完成時に Codex は:

1. [ ] ファイル名が本マップの規則通りか
2. [ ] 配置先が `Assets/Resources/Skins/` または `Assets/Resources/Audio/` か (`ArtSource/` は最終配置先ではない)
3. [ ] サイズが規定通りか (`docs/VISUAL_STYLE_GUIDE.md` 参照)
4. [ ] 透過 PNG なら四隅 alpha=0 か
5. [ ] `.meta` ファイル添付済みか (Unity の import settings)
6. [ ] `Tools/ValidateCodexAssets.ps1 -Category {対応カテゴリ}` で検証 0 エラー
7. [ ] `REMAINING_TASKS.md` の「直近完了」に記録

これに沿えば Claude / ユーザーは追加修正なしで採用できる。

---

最終更新: 2026-05-28
担当: Claude (秘書)
読者: Codex (素材生成)
