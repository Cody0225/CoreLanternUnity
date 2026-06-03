# CoreLanternUnity Image Asset Backlog

## 2026-06-02 Codex Partner L0 Pixel-Art Redesign

Status: RUNTIME_CANDIDATE / hooked by existing resource filenames. `CoreLanternGame.cs` was not edited.

The six base partner L0 assets were redesigned as high-resolution pixel-art-style
sprites for the future title lineup. Existing `.meta` files were preserved by
overwriting PNGs only.

| Status | File | Size | Use |
|---|---|---:|---|
| HOOKED | `Assets/Resources/Skins/Partner_S1_L0.png` | 512x512 | Cobalt Pup / blue cyber wolf pup |
| HOOKED | `Assets/Resources/Skins/Partner_S2_L0.png` | 512x512 | Ember Drake / orange cyber drake |
| HOOKED | `Assets/Resources/Skins/Partner_S3_L0.png` | 512x512 | Sage Hare / green cyber hare |
| HOOKED | `Assets/Resources/Skins/Partner_S4_L0.png` | 512x512 | Hex Cat / purple dark cyber cat |
| HOOKED | `Assets/Resources/Skins/Partner_S5_L0.png` | 512x512 | Drift Fox / pink cyber fox |
| HOOKED | `Assets/Resources/Skins/Partner_S6_L0.png` | 512x512 | Iron Bear / black armored bear |
| REVIEW | `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png` | 1536x276 | Title-lineup visual check |
| REVIEW | `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png` | 1080x450 | 64/96/128px readability check |
| REVIEW | `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_clean_transparent.png` | 3072x512 | Clean transparent lineup |

Validation: all runtime PNGs have transparent corners and normalized foot
baseline (`bottom_y=448`). S1-S6 `.meta` files still end with byte `0A`.
Claude title-layout guidance: `docs/CLAUDE_TITLE_6PARTNER_LINEUP_HOOK_SPEC.md`.

## 2026-06-01 Codex Non-Character Asset Queue Preview

Status: REVIEW / planning-only. `CoreLanternGame.cs` was not edited.

Created a visual queue sheet for non-character assets that are already generated
or waiting for selective hook-up. This is not a runtime asset.

| Status | File | Size | Use |
|---|---|---:|---|
| REVIEW | `Assets/ArtSource/NonCharacterQueue_20260601/NonCharacterAssetQueue_20260601.png` | 1600x1442 | Preview of A egg, HUD9, Stage4/5, and V2 UI waiting queues |

Related doc: `docs/NON_CHARACTER_ASSET_QUEUE_20260601.md`.

## 2026-06-01 Codex A Egg/Core Presentation Assets

Status: BUILT / Claude hook-up waiting. `CoreLanternGame.cs` was not edited.

Created separate A-selected egg/core presentation resources for title, result,
and small-icon placement. These are not hooked yet because Claude owns code
changes now that Claude is available again.

| Status | File | Size | Use |
|---|---|---:|---|
| BUILT | `Assets/Resources/Skins/Title_CoreEgg_ASelected_v1.png` | 768x768 | Title hero/emblem candidate |
| BUILT | `Assets/Resources/Skins/Result_CoreEgg_ASelected_v1.png` | 512x512 | Result/victory emblem candidate |
| BUILT | `Assets/Resources/Skins/Icon_CoreEgg_ASelected_v1.png` | 256x256 | Small icon/codex/setup mark candidate |
| REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/title_result_support_A_20260601/TitleResultCoreEgg_ASelected_v1_preview.png` | 1280x720 | Layout and 64/96/128px shrink preview |

Validation: all three PNGs have transparent corners (`alpha=0`) and `.meta`
files. Hook-up spec: `docs/CLAUDE_A_EGG_PRESENTATION_HOOK_SPEC.md`.
Secretary review: OK for Claude handoff. Do not place title/progress text
directly over the faint circular field in the title asset.

## 2026-06-01 Codex A Egg/Core Runtime Candidate

Status: RUNTIME_CANDIDATE / hooked by existing resource filename. `CoreLanternGame.cs` was not edited.

The user selected candidate A as the current egg/core direction. Codex replaced
the existing resource image at `Assets/Resources/Skins/Lantern.png` with a
384x384 transparent A-selected version, while preserving the old image as a
rollback backup in ArtSource.

| Status | File | Size | Use |
|---|---|---:|---|
| HOOKED | `Assets/Resources/Skins/Lantern.png` | 384x384 | Current in-game data egg/core sprite |
| BACKUP | `Assets/ArtSource/EggCoreVisual_20260601/selected_A_runtime_candidate_20260601/Lantern_PRE_ASelected_20260601.png` | 384x384 | Previous runtime Lantern backup |
| RUNTIME_CANDIDATE | `Assets/ArtSource/EggCoreVisual_20260601/selected_A_runtime_candidate_20260601/Lantern_ASelected_v1_RUNTIME_CANDIDATE.png` | 384x384 | Same A-selected runtime source |
| RUNTIME_CANDIDATE | `Assets/ArtSource/EggCoreVisual_20260601/selected_A_runtime_candidate_20260601/Title_CoreEgg_ASelected_v1_RUNTIME_CANDIDATE.png` | 512x512 | Title/icon-size candidate source |
| REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/selected_A_runtime_candidate_20260601/Lantern_ASelected_v1_comparison_shrink_check.png` | 1100x720 | Previous/new + 64/96/128/192px comparison |

Secretary review passed. Remaining caveat: tiny shell/pedestal details compress
at 64px, but the gold egg silhouette and cyan center light remain readable.

## 2026-06-01 Codex A Egg/Core Map Support

Status: RUNTIME_CANDIDATE / hooked by existing resource filenames. `CoreLanternGame.cs` was not edited.

Updated the existing center-ring support sprites to better match the selected
A egg/core direction. Previous runtime versions are backed up under
`Assets/ArtSource/EggCoreVisual_20260601/core_map_support_A_20260601/`.

| Status | File | Size | Use |
|---|---|---:|---|
| HOOKED | `Assets/Resources/Skins/Floor_CoreMark_A.png` | 256x256 | Subtle floor protocol ring |
| HOOKED | `Assets/Resources/Skins/Core_Platform.png` | 256x256 | Dark core socket/pedestal |
| HOOKED | `Assets/Resources/Skins/Core_RingOuter.png` | 256x256 | Thin outer gold/cyan ring |
| HOOKED | `Assets/Resources/Skins/Core_RingInner.png` | 256x256 | Compact inner focus ring |
| BACKUP | `Assets/ArtSource/EggCoreVisual_20260601/core_map_support_A_20260601/*_PRE_ASelected_20260601.png` | 256x256 | Previous center support sprites |
| REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/core_map_support_A_20260601/CoreMapSupport_ASelected_v1_preview.png` | 1280x720 | Individual and combined preview |

Secretary review passed. Remaining caveat: the combined outer ring may still
draw attention in Play mode if Unity's existing glow/tint stack makes it too
bright, so visual QA in the editor is recommended.

## 2026-06-01 Codex Egg/Core Visual Review v1

Status: VISUAL_REVIEW / ArtSource only. `CoreLanternGame.cs` was not edited.

Created three candidate directions for the central egg/core identity. These are
not runtime candidates yet and must not be copied to `Assets/Resources/Skins/`
or connected in code until the user approves a direction.

| Status | File | Size | Use |
|---|---|---:|---|
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_candidates_chromakey.png` | 1819x865 | Original A/B/C candidate board |
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_candidates_transparent.png` | 1819x865 | Transparent candidate board |
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_A_SacredDataEgg_transparent.png` | 539x615 | Candidate A isolated crop |
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_B_ArmoredCoreEgg_transparent.png` | 522x620 | Candidate B isolated crop |
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_C_HatchProtocolEgg_transparent.png` | 506x599 | Candidate C isolated crop |
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_shrink_check.png` | 900x820 | 96/128/192px readability check |
| VISUAL_REVIEW | `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/EggCore_VISUAL_REVIEW_v1_title_scale_preview.png` | 1280x720 | Title-scale dark preview |

Secretary review: A `SacredDataEgg` is the recommended base for the game face.
B is useful as combat-core inspiration but too armored/noisy for the main icon.
C has good hatch/evolution emotion but weakens at 96px.

## 2026-06-01 Codex Cobalt Pup Pixel Animation Prototype

Status: PROTOTYPE / ArtSource only. `CoreLanternGame.cs` was not edited.

Created one pixel-art animation prototype sheet for Cobalt Pup to test whether the character direction should move away from AI-painted still images and toward game-readable pixel sprites.

| Status | File | Size | Use |
|---|---|---:|---|
| PROTOTYPE | `Assets/ArtSource/CobaltPixelAnim_20260601/CobaltPup_PixelAnimSheet_chromakey.png` | 1536x1024 | Original chroma-key sheet |
| PROTOTYPE | `Assets/ArtSource/CobaltPixelAnim_20260601/CobaltPup_PixelAnimSheet_transparent.png` | 1536x1024 | Transparent preview sheet |
| PROTOTYPE | `Assets/ArtSource/CobaltPixelAnim_20260601/frames/CobaltPup_idle_00.png` etc. | 256x256 | Cropped animation frames |
| PROTOTYPE | `Assets/ArtSource/CobaltPixelAnim_20260601/CobaltPup_*_preview.gif` | 256x256 | Motion preview GIFs |

Frame layout: idle 4 / run 6 / attack 4 / hit 2. This is not yet connected to `Assets/Resources/Skins/`.

## 2026-06-01 Codex Player Form4 Fallback Cleanup

Status: DONE / restored existing disabled asset. `CoreLanternGame.cs` was not edited.

`Player_Form4.png` was the only optional sprite reference missing from image validation. An existing disabled backup was already present in `Assets/Resources/Skins/`, so Codex restored it without deleting the disabled backup files.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `Player_Form4.png` | 256x256 | Optional final player/evolution form sprite |

Validation: `Tools/TestImageResources.ps1` now reports `Warnings: 0` and `Errors: 0`.

## 2026-05-31 Codex HUD 9-slice Candidates

Status: PARTIAL HOOK / Wave progress only. `CoreLanternGame.cs` was temporarily edited by Codex on 2026-06-01 while Claude was rate-limited.

Generated by `Tools/GenerateHud9SliceAssets.ps1` as a quieter replacement candidate for the old rejected HUD panels. These use new `HUD9_*` names. Only the top Wave progress bar is currently hooked; panel/HP/EXP assets remain waiting for visual approval and a later hook-up pass.

| Status | File | Size | Use |
|---|---|---:|---|
| BUILT | `HUD9_Panel_Wave.png` | 512x512 | 9-slice Wave HUD panel candidate |
| BUILT | `HUD9_Panel_HP.png` | 512x512 | 9-slice HP HUD panel candidate |
| BUILT | `HUD9_Panel_Level.png` | 512x512 | 9-slice Level/EXP HUD panel candidate |
| BUILT | `HUD9_Panel_ChipMini.png` | 512x512 | 9-slice data chip mini panel candidate |
| BUILT | `HUD9_Bar_Back.png` | 256x64 | 9-slice HP/EXP bar back candidate |
| BUILT | `HUD9_Bar_HP_PlayerFill.png` | 256x64 | 9-slice player HP fill candidate |
| BUILT | `HUD9_Bar_HP_CoreFill.png` | 256x64 | 9-slice core HP fill candidate |
| BUILT | `HUD9_Bar_EXP_Fill.png` | 256x64 | 9-slice EXP fill candidate |
| HOOKED | `HUD9_WaveProgress_Frame.png` | 512x48 | 9-slice top wave progress frame candidate |
| HOOKED | `HUD9_WaveProgress_Fill_Normal.png` | 512x48 | 9-slice normal wave progress fill candidate |
| HOOKED | `HUD9_WaveProgress_Fill_Boss.png` | 512x48 | 9-slice boss wave progress fill candidate |

Art-source copies and preview: `Assets/ArtSource/HUD_9Slice_20260531/`.

Hook-up spec: `docs/CLAUDE_HUD9SLICE_HOOK_SPEC.md`.

Validation: all 11 PNG files match intended dimensions, have transparent corners (`alpha=0`), and include `.meta` files with sprite borders. 2026-06-01 full compile/image checks also passed after the Wave progress hook-up.

## 2026-05-31 Codex itch.io Page Assets

Status: BUILT / marketing-only. `CoreLanternGame.cs` was not edited.

Generated by `Tools/GenerateItchIoPageAssets.ps1` using the existing Eggcore logo and core-mark assets. These are character-independent draft assets for the demo page, so they should survive the later character redesign pass better than character-heavy store art.

| Status | File | Size | Use |
|---|---|---:|---|
| BUILT | `marketing/ItchIo/ItchIo_Header_630x500.png` | 630x500 | itch.io header image |
| BUILT | `marketing/ItchIo/ItchIo_Cover_315x250.png` | 315x250 | itch.io cover image |
| BUILT | `marketing/ItchIo/ItchIo_SocialCard_1200x630.png` | 1200x630 | social/link preview draft |

Art-source copies and preview: `Assets/ArtSource/ItchIo_20260531/`.

Validation: all marketing PNG files match intended dimensions and are fully opaque at all four corners (`alpha=255`). Text copy was manually checked in the preview and split into two lines to avoid clipping.

## 2026-05-31 Codex Windows Exe Icon Set

Status: BUILT / asset-only. `CoreLanternGame.cs` was not edited.

Generated by `Tools/GenerateExeIconAssets.ps1` from the Eggcore core-mark direction.

| Status | File | Size | Use |
|---|---|---:|---|
| BUILT | `build/Icons/icon_16.png` | 16x16 | Windows icon source |
| BUILT | `build/Icons/icon_32.png` | 32x32 | Windows icon source |
| BUILT | `build/Icons/icon_48.png` | 48x48 | Windows icon source |
| BUILT | `build/Icons/icon_64.png` | 64x64 | Windows icon source |
| BUILT | `build/Icons/icon_128.png` | 128x128 | Windows icon source |
| BUILT | `build/Icons/icon_256.png` | 256x256 | Windows icon source |
| BUILT | `build/Icons/icon_512.png` | 512x512 | High-res source / store-adjacent use |
| BUILT | `build/Icons/EggcoreProtocol.ico` | multi-size | Windows `.exe` icon |

Art-source copies and preview: `Assets/ArtSource/Icon_Drafts_20260531/`.

Validation: all PNG files match intended dimensions and have transparent corners (`alpha=0`). The `.ico` contains standard Windows icon sizes 16 through 256; the 512px PNG is kept as a high-resolution source.

## 2026-05-30 Codex Game Logo Draft

Status: DONE / asset-only. `CoreLanternGame.cs` was not edited.

Generated by `Tools/GenerateLogoAssets.ps1` as deterministic text-rendered PNGs. This avoids AI text misspelling and keeps the game title exact.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `GameLogo.png` | 1024x512 | Main game title logo, transparent PNG |
| DONE | `GameLogo_Mono.png` | 1024x512 | Monochrome/overlay title logo, transparent PNG |
| DONE | `GameLogo_Mark.png` | 512x512 | Data egg/core mark for icon derivation, transparent PNG |
| DONE | `GameLogo_Mark_Mono.png` | 512x512 | Monochrome mark for press/overlay use, transparent PNG |
| DONE | `GameLogo_TitleCompact.png` | 1024x320 | Compact title-screen logo with reduced vertical padding |

Art-source copies and preview: `Assets/ArtSource/Logo_Drafts_20260530/`.

Validation: all logo resource files match intended dimensions and have transparent corners (`alpha=0`).

Note: `StudioLogo_Embercore*` was not generated. A quick name collision check found an existing Steam game named `Embercore`, so the studio name should be confirmed or renamed before final studio-logo production.

## 2026-05-30 Codex Result UI V2 Exact-Size Draft

Status: DONE / hooked in `CoreLanternGame.cs`.

Generated by `Tools/GenerateResultV2Assets.ps1` after reading the current result screen rect sizes from `CoreLanternGame.cs`.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `Result_DeckFrame_v2.png` | 960x580 | Result main deck frame |
| DONE | `Result_PortraitFrame_v2.png` | 220x220 | Final partner portrait frame |
| DONE | `Result_StatTile_v2.png` | 420x56 | Wide Wave/time/kills/data/damage/rank stat tile |
| DONE | `Result_BadgeStrip_Route_v2.png` | 220x44 | Result route badge strip |
| DONE | `Result_BadgeStrip_Fusion_v2.png` | 220x44 | Result fusion/cross badge strip |
| DONE | `Result_MvpRow_v2.png` | 230x68 | MVP build row at current result size |
| DONE | `Result_SummaryPlate_v2.png` | 520x44 | Compact run summary backing plate |
| DONE | `Result_FooterGuide_v2.png` | 520x28 | Keyboard guide/footer backing plate |
| DONE | `Result_RankMedal_S_v2.png` | 128x128 | Result S rank medal |
| DONE | `Result_RankMedal_A_v2.png` | 128x128 | Result A rank medal |
| DONE | `Result_RankMedal_B_v2.png` | 128x128 | Result B rank medal |
| DONE | `Result_RankMedal_C_v2.png` | 128x128 | Result C rank medal |
| DONE | `Result_RankMedal_D_v2.png` | 128x128 | Result D rank medal |

Art-source copies: `Assets/ArtSource/Result_V2_20260530/`.
Preview: `Assets/ArtSource/Result_V2_20260530/Result_V2_Preview.png`.

Validation: all thirteen files match intended dimensions and have transparent corners (`alpha=0`). No text is baked into the images.

## 2026-05-30 Codex Title UI V2 Non-Text Draft

Status: DONE / hooked in `CoreLanternGame.cs`.

Generated by `Tools/GenerateTitleUiV2Assets.ps1` as title-screen decorative parts. These avoid baked title text and should be layered behind live Unity text/buttons.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `Title_CoreEmblem_v2.png` | 512x512 | Main menu central data-egg/core emblem |
| DONE | `Title_SubtitlePlate_v2.png` | 580x38 | Subtitle/supporting-copy backing plate |
| DONE | `Title_StatsRibbon_v2.png` | 820x54 | Best wave / clears / mission / stage stats backing ribbon |
| DONE | `Title_LogoUnderline_v2.png` | 620x22 | Thin title underline / logo accent rail |
| DONE | `Title_NavRail_v2.png` | 960x24 | Navigation button rail accent |
| DONE | `Title_CornerAccent_v2.png` | 160x160 | Lightweight corner accent lines |

Art-source copies: `Assets/ArtSource/Title_UI_V2_20260530/`.
Preview: `Assets/ArtSource/Title_UI_V2_20260530/Title_UI_V2_Preview.png`.

Validation: all six files match intended dimensions and have transparent corners (`alpha=0`). The corner accent was revised from a filled plate to line-only decoration so it does not cover title text.

## 2026-05-30 Codex Button V2 Exact-Size Draft

Status: DONE / hooked in `CoreLanternGame.cs`.

Generated by `Tools/GenerateButtonV2Assets.ps1` after reading the current menu/result/setup button rect sizes from `CoreLanternGame.cs`.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `Button_Primary_Start_v2.png` | 420x60 | Main menu large start button |
| DONE | `Button_MenuSecondary_v2.png` | 150x44 | Main menu secondary navigation buttons |
| DONE | `Button_RunStageChip_v2.png` | 180x108 | Run setup stage selection chip |
| DONE | `Button_DangerChip_v2.png` | 80x56 | Run setup danger selection chip |
| DONE | `Button_RunBack_v2.png` | 220x60 | Run setup back button |
| DONE | `Button_RunStart_v2.png` | 280x60 | Run setup start button |
| DONE | `Button_PauseResume_v2.png` | 320x48 | Pause resume button |
| DONE | `Button_Wide_320x42_v2.png` | 320x42 | Pause/options wide command button |
| DONE | `Button_OptionsToggle_v2.png` | 250x36 | Options toggle button |
| DONE | `Button_Stepper_v2.png` | 44x30 | Options plus/minus button |
| DONE | `Button_Reroll_v2.png` | 440x52 | Upgrade reroll button |
| DONE | `Button_SkipReward_v2.png` | 300x52 | Upgrade skip/reward button |
| DONE | `Button_ResultRetry_v2.png` | 340x64 | Result retry button |
| DONE | `Button_ResultMenu_v2.png` | 240x50 | Result main menu button |
| DONE | `Button_CloseSmall_v2.png` | 240x32 | Codex/tree close button |
| DONE | `Button_CloseWide_v2.png` | 260x38 | Wider close/back button |

Art-source copies: `Assets/ArtSource/Button_V2_20260530/`.
Preview: `Assets/ArtSource/Button_V2_20260530/Button_V2_Preview.png`.

Validation: all sixteen files match intended dimensions and have transparent corners (`alpha=0`). No text is baked into the images; Unity text should remain live for localization and sizing.

## 2026-05-29 Codex UI Polish Pack

Status: DONE / prepared for future hook-up.

Generated by `Tools/GenerateUiPolishPack.ps1`:

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `Result_RankBadge_S.png` | 160x160 | Result rank badge |
| DONE | `Result_RankBadge_A.png` | 160x160 | Result rank badge |
| DONE | `Result_RankBadge_B.png` | 160x160 | Result rank badge |
| DONE | `Result_RankBadge_C.png` | 160x160 | Result rank badge |
| DONE | `Result_RankBadge_D.png` | 160x160 | Result rank badge |
| DONE | `Result_MvpSlot_v2.png` | 320x96 | MVP build slot frame |
| DONE | `Warning_MidBoss_Frame_v2.png` | 960x220 | Mid boss warning overlay frame |
| DONE | `Warning_FinalBoss_Frame_v2.png` | 960x220 | Final boss warning overlay frame |
| DONE | `Warning_CoreMark_v2.png` | 256x128 | Core Mark warning icon/overlay |

Art-source copies: `Assets/ArtSource/UI_Polish_20260529/`.
Preview: `Assets/ArtSource/UI_Polish_20260529/UI_Polish_Preview.png`.

Validation: all files have transparent corners (`alpha=0`) and are not intended to be stretched into full-screen panels. Boss warning frames contain no baked text.

## 2026-05-29 Codex HUD Panel V2 Exact-Size Draft

Status: DONE / prepared for future hook-up.

Generated by `Tools/GenerateHudPanelV2Assets.ps1` after reading the current HUD rect sizes from `CoreLanternGame.cs`.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `HUD_Panel_Wave_v2.png` | 250x74 | Top-left wave HUD |
| DONE | `HUD_Panel_HP_v2.png` | 270x104 | Player/core HP HUD |
| DONE | `HUD_Panel_Level_v2.png` | 270x76 | Level/EXP HUD |
| DONE | `HUD_Panel_ChipMini_v2.png` | 190x42 | Compact data chip HUD |
| DONE | `HUD_Panel_Data_v2.png` | 250x54 | Data panel HUD |
| DONE | `HUD_Panel_Loadout_v2.png` | 296x92 | Active build/loadout HUD |
| DONE | `HUD_Panel_Stats_v2.png` | 286x156 | Combat stats HUD |
| DONE | `HUD_Panel_Links_v2.png` | 360x228 | Links/status HUD |
| DONE | `HUD_Panel_EventLog_v2.png` | 520x70 | Event log HUD |

Art-source copies: `Assets/ArtSource/HUD_Panel_V2_20260529/`.
Preview: `Assets/ArtSource/HUD_Panel_V2_20260529/HUD_Panel_V2_Preview.png`.

Validation: all nine files match current UI rect dimensions and have transparent corners (`alpha=0`). These are not yet code-hooked. They are intended for exact-size/native use to avoid the old stretched-panel quality problem.

## 2026-05-29 Codex Card Part V2 Exact-Size Draft

Status: DONE / hooked in `CoreLanternGame.cs`.

Generated by `Tools/GenerateCardPartV2Assets.ps1` after reading the current `CreateButton()` child rect sizes from `CoreLanternGame.cs`.

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `Card_Header_Basic_v2.png` | 252x34 | Basic card header plate |
| DONE | `Card_Header_Rare_v2.png` | 252x34 | Rare card header plate |
| DONE | `Card_Header_Epic_v2.png` | 252x34 | Epic/special card header plate |
| DONE | `Card_TitlePlate_v2.png` | 248x36 | Card title background |
| DONE | `Card_LevelPlate_v2.png` | 240x22 | Card level/progression background |
| DONE | `Card_RarityPlate_Basic_v2.png` | 178x28 | Basic rarity plate |
| DONE | `Card_RarityPlate_Rare_v2.png` | 178x28 | Rare rarity plate |
| DONE | `Card_RarityPlate_Epic_v2.png` | 178x28 | Epic rarity plate |
| DONE | `Card_BottomRail_Cyan_v2.png` | 210x8 | Card bottom rail |
| DONE | `Card_BottomRail_Gold_v2.png` | 210x8 | Special card bottom rail |
| DONE | `Card_SpecialCorner_Epic_v2.png` | 40x40 | Epic/special corner marker |
| DONE | `Card_SpecialCorner_Cross_v2.png` | 40x40 | Cross/fusion corner marker |

Art-source copies: `Assets/ArtSource/Card_Parts_V2_20260529/`.
Preview: `Assets/ArtSource/Card_Parts_V2_20260529/Card_Parts_V2_Preview.png`.

Validation: all twelve files match intended dimensions and have transparent corners (`alpha=0`). No text is baked into the images; Unity text stays live for localization and resizing. Hook-up uses exact-size internal parts only; the whole-card generated frame remains disabled to avoid stretching.

## 2026-05-27 Codex Image Resource Validation

Status: DONE / tooling added.

Added `Tools/TestImageResources.ps1` for quick image resource validation:

- Checks every `Assets/Resources/Skins/*.png` for missing `.meta` and zero-byte files.
- Checks static `LoadOptionalSprite("Skins/...")` and `Resources.Load<Sprite>("Skins/...")` references.
- Checks DONE entries in this backlog against files in `Assets/Resources/Skins/`.

Latest result:

- Skins PNG files: 502
- Code sprite references: 121
- Backlog PNG references: 138
- Backlog DONE PNG references: 29
- Errors: 0
- Warning: optional `Player_Form4.png` is absent and currently uses procedural fallback.

## 2026-05-25 Codex Stage/Boss Enemy PNGs

Status: DONE / hooked by existing resource names.

Generated by `Tools/GenerateStageBossEnemyAssets.ps1`:

| Status | File | Use |
|---|---|---|
| DONE | `LavaCrawler.png` | Stage lava crawler variant |
| DONE | `Enemy_MagmaTitan.png` | Lava stage heavy enemy |
| DONE | `Enemy_CorruptionDrone.png` | Broken/corruption stage specialist enemy |
| DONE | `Enemy_FrostKnight.png` | Frost stage heavy enemy |
| DONE | `Enemy_VoltDasher.png` | Storm stage fast enemy |
| DONE | `Boss_Pulswyrm.png` | Mid boss sprite |
| DONE | `Boss_Nullwyrm.png` | Final boss sprite |

Preview: `Assets/ArtSource/Generated_StageBossEnemy_Preview.png`.

Validation: all files are 256x256 transparent PNGs, corner alpha 0.

Note: partner/character/evolution/fusion redesign remains final-phase work and was not changed in this pass.

## 2026-05-25 Codex Stage 4/5 Support PNGs

Status: DONE / prepared for future hook-up.

Generated by `Tools/GenerateStage45SupportAssets.ps1`:

| Status | File | Size | Use |
|---|---|---:|---|
| DONE | `StageThumb_Frost.png` | 160x90 | Frost Vault stage select thumbnail |
| DONE | `StageThumb_Storm.png` | 160x90 | Storm Spire stage select thumbnail |
| DONE | `Stage4_FrostCrystal_A.png` | 96x96 | Frost crystal hazard/reward object |
| DONE | `Stage4_FrostPatch_A.png` | 256x256 | Frost slow-zone decal |
| DONE | `Stage5_LightningMarker_A.png` | 256x256 | Lightning telegraph marker |
| DONE | `Stage5_LightningStrike_A.png` | 128x512 | Vertical lightning VFX |

Preview: `Assets/ArtSource/Generated_Stage45Support_Preview.png`.

Validation: thumbnails are opaque 160x90; hazard/VFX PNGs have transparent corners.

Debug note: `.meta` files are now generated/backfilled by `Tools/GenerateStage45SupportAssets.ps1`. These assets are placed and import-safe, but `CoreLanternGame.cs` does not yet load `StageThumb_Frost`, `StageThumb_Storm`, `Stage4_FrostCrystal_A`, `Stage4_FrostPatch_A`, `Stage5_LightningMarker_A`, or `Stage5_LightningStrike_A`.

最終更新: 2026-05-27

このファイルは、今後生成・差し替えが必要な画像素材の台帳です。  
目的は「何を作るべきか」「どの名前で置くべきか」「どれを優先するか」を迷わないようにすることです。

## 基本方針

- 商用前提なので、既存IPの丸コピーはしない。方向性は「暗いサイバー床、ネオン発光、可愛い相棒、進化でシルエットが変わる」。
- まずは静止画の統一感を作る。その後に idle / walk / attack / hit の数フレーム化へ進む。
- 画像は `Assets/Resources/Skins/` に置くとコードが優先ロードする。
- キャラ/敵は透過PNG推奨。UIカードや床は不透過でもよい。
- 既存の `.disabled` は過去素材の退避。復活させる前に見た目を確認する。
- **キャラ素体・進化・融合は大幅リデザイン予定だが、作業量が大きいため最終フェーズまで生成/差し替えしない。現状素材は暫定の確認用として保持する。**

## 追記: Cobalt Pup 進化画像 試作 (2026-05-24)

- Cobalt Pup 1体分の試作として `Partner_S1_L0`, `Partner_S1_R1_L3`, `Partner_S1_R2_L3`, `Partner_S1_R3_L3` を生成。
- 生成元はクロマキー背景、`remove_chroma_key.py` で透過済み。4隅alphaは0確認済み。
- レビュー用プレビュー: `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/Cobalt_Evolution_Transparent_Preview.png`
- 差し替え候補512版: `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/ready_512/`
- ユーザー指示により、上記4枚は `Assets/Resources/Skins/` に反映済み。
- 既存画像バックアップ: `Assets/ArtSource/CharacterBackup_CobaltBeforeImplement_20260524_041559/`
- L1/L2は未生成のため、Cobaltルートの中間進化統一は `NEED`。

## 追記: 非キャラ素材再生成 (2026-05-21)

- `HUD_UI_ASSET_REQUEST.md` に基づく非キャラ素材パックを `Tools/GeneratePolishedNonCharacterAssets.ps1` で再生成。
- プレビュー: `Assets/ArtSource/Generated_PolishedNonCharacterAsset_Preview.png`
- バックアップ: `Assets/ArtSource/NonCharacterAssetBackup_20260521_233106/`
- 対象は `Card_*`, `HUD_*`, `Background_*`, `MainMenu_Background`, `Cutin_*`, `Result_*`, `Bullet_*`, `Pickup_Heal`, `Effect_*`, `Icon_Link_*`, `Icon_Stat_*`, `Floor_DarkBase`, `Floor_Hazard_Lava`, `Arena_Boundary`。
- 大型UI画像は品質改善版を作成済み。現在は背景/カットイン/ボス背景のみ比率維持で採用し、HUD/カード/警告帯は未使用。採用判断は実機目視と9-slice/比率維持対応後に行う。

## 追記: 非キャラ素材ステータス整理 (2026-05-22)

- 旧Codex生成の大型UI画像は、引き伸ばし・画質荒れ・情報量過多が出たため `DEPRECATED` 扱い。直接復活させず、必要なら9-slice/比率固定版として再作成する。
- `Tools/GeneratePolishedNonCharacterAssets.ps1` で作成した新Codex非キャラ素材は `DONE / REVIEW`。Claude側の報告では Stage 1 (背景) 有効化済みとして扱う。Codexローカル側にフラグ差分が残る場合は、Claude作業中の `CoreLanternGame.cs` を優先確認する。
- 新素材の採用方針は「小アイコンは積極採用、大型パネル/カード/背景は9-slice中央単色 + 角装飾 + アスペクト比維持を確認してから段階投入」。
- `Pickup_Data.png` は仕様発注64×64に対して現状 `116×112`。現時点ではゲーム内ピックアップとして視認性があり、即差し替えのリスクが高いため `KEEP / REVIEW`。UI小アイコンで使う時だけ `Pickup_Data_64` 相当を作る。

## 追記: 背景画像 低密度再生成 (2026-05-22)

- `Draw-Backdrop` を再設計し、背景画像の全面グリッド、赤縦縞、中央リング、斜め帯を削除。
- 中央60%はほぼ単色グラデのみ。画面端にだけ低アルファの細いラインを残す。
- 再生成対象: `Background_MainMenu`, `Background_Evolution`, `Background_CrossEvolution`, `Background_Victory`, `Background_Defeat`, `Background_BossPulswyrm`, `Background_BossNullwyrm`。
- コード互換alias: `MainMenu_Background`, `Cutin_Evolve_*`, `Cutin_Fusion`, `Result_Clear_Background`, `Result_GameOver_Background` も同じ方針で再生成済み。
- 最新バックアップ: `Assets/ArtSource/NonCharacterAssetBackup_20260522_003259/`

## 追記: 背景/カットイン画像配置 (2026-05-22)

- `CoreLanternGame.cs` で低密度背景を段階的に有効化。
- メインメニュー、リザルト、進化/クロス進化カットイン、ボスカットインは `preserveAspect = true` で表示し、画像比率を変えない。
- `Background_BossPulswyrm` / `Background_BossNullwyrm` は今回初めてコード側へ接続済み。
- HUDバー/パネル、カードフレーム、暗転板、危険vignette、警告帯は引き伸ばし/ノイズ再発防止のため未採用を継続。

## 追記: 非キャラ画像 Batch 2 (2026-05-22)

- `Tools/GenerateNonCharacterPolishBatch2.ps1` を追加し、非キャラ素材32枚を生成。
- プレビュー: `Assets/ArtSource/Generated_NonCharacterPolishBatch2_Preview.png`
- 追加レリック: `Relic_Titan`, `Relic_Overdrive`, `Relic_DataSurge`, `Relic_Apex`。
- モジュール識別アイコン: `ModuleIcon_*` 20種。強化カードへコード接続済み。
- ステージ/小物: `StageThumb_Arena`, `StageThumb_Lava`, `StageThumb_BrokenCore`, `Stage2_LavaPool_A`, `Stage2_HeatVent_A`, `Stage3_RelayDevice_A`, `Stage3_CorruptionPatch_A`, `Pickup_Data_64`。
- 今回も中央領域に細かい装飾を入れすぎない方針。小アイコンは採用済み、大型ステージサムネ/小物は今後のUI/Stage3実装用ストック。

## ステータス定義

| 状態 | 意味 |
|---|---|
| `DONE` | 現在PNGが存在し、ひとまず使える |
| `TEMP` | PNGはあるが暫定・統一感や名前一致に不安あり |
| `NEED` | 作成が必要 |
| `HOOK` | 画像だけでなくコード側のロード対応も必要 |
| `REVIEW` | 目視確認して採用/差し戻しを決める |

## 現在のざっくり在庫

| カテゴリ | 現在のPNG数 | コメント |
|---|---:|---|
| Partner | 296 | 既存6素体 + 新規style7/8 + 全ルートL0-3 + 種族別クロス進化L0-3。style7/8は暫定生成済み |
| Route | 12 | 汎用SPEED/POWER/GUARDのStage0-3 |
| Fusion | 6 | 汎用クロス進化。種族別L3は `Partner_S*_F*_L3` 側で追加済み |
| Enemy | 8 | Runner/Brute/Shooter/Boss/Boss Warning + Dasher/Bomber/Phantom。追加3種は暫定外部PNG |
| Floor | 10 | 床タイルA/B/C + ベース/グリッド/ひび/配線/コア印 |
| UI | 15 | カード、パネル、バッジ系 |
| HUD専用 | 26 | HP/EXP/Wave/パネル素材に加え、BossBar/RelicSlot/HP/EXP/DataChipアイコン/警告系を生成済み。ただし大型HUD/警告画像は画質/引き伸ばし問題で不採用。小アイコンのみ使用 |
| Relic | 10 | 既存6種 + 追加4種。全レリックカード/HUDへ接続済み |
| ModuleIcon | 20 | 強化カードの種別識別用。コード接続済み |
| Minimap | 9 | 背景/枠/グリッド/dot素材に加え、Pickup/Pingを生成済み。コード側の適用は `HOOK` |
| Map Props | 19 | 外周石、サーバー破片、ネオン小物、コア台座/リング、スポーンリング、Stage2/Stage3小物 |
| VFX | 7 | 弾、Impact、Evolution Ring系 |
| Pickup | 5 | Data/Core/Bonus/Heal系 + `Pickup_Data_64`。`Pickup_Data.png` は 116×112 のため現状維持 |

## 命名ルール

### 相棒素体

| 用途 | ファイル名 |
|---|---|
| 種族ごとの素体 | `Partner_S{species}_L0.png` |
| 種族 + ルート + 段階 | `Partner_S{species}_R{route}_L{stage}.png` |
| 種族 + ルート + バリアント + 段階 | `Partner_S{species}_R{route}_V{variant}_L{stage}.png` |
| 種族 + 融合 + 段階 | `Partner_S{species}_F{fusion}_L{stage}.png` |
| 種族 + 融合 + バリアント + 段階 | `Partner_S{species}_F{fusion}_V{variant}_L{stage}.png` |

### 番号対応

| 種族番号 | 名前 | 方向性 |
|---:|---|---|
| 1 | Cobalt Pup | 青い犬/狼系、標準型 |
| 2 | Ember Drake | 橙の小竜、弾幕型 |
| 3 | Sage Hare | 緑の兎/賢者、防衛型 |
| 4 | Hex Cat | 紫の猫/魔術、連鎖型 |
| 5 | Drift Fox | 桃/白の狐、回避型 |
| 6 | Iron Bear | 黒/金の熊、装甲型 |
| 7 | Wraith Lynx | 白/銀の山猫・狼寄り、近接型。暫定外部PNG生成済み |
| 8 | Genesis Core | 金/シアンの隠し総合素体。暫定外部PNG生成済み |
| 9 | Halo Caster | ファンネル/ビット型。コード実装済みだがPNG台帳は未整備 |
| 10 | Pulse Hydra | 持続レーザー型。Claude側追加中。最終採用とPNGセットは要確認 |

| ルート番号 | 名前 | 方向性 |
|---:|---|---|
| 1 | SPEED | 細身、高速、翼ではなくブースター/刃/光跡 |
| 2 | POWER | 大型、火力、角/砲/重装 |
| 3 | GUARD | 装甲、盾、リング、守護 |

| 融合番号 | 名前 | リンク |
|---:|---|---|
| 1 | Nova Aegis | Nova + Bulwark |
| 2 | Photon Siphon | Nova + Siphon |
| 3 | Core Bastion | Bulwark + Siphon |
| 4 | Nova Phantom | Nova + Phase |
| 5 | Aegis Drift | Bulwark + Phase |
| 6 | Photon Wraith | Siphon + Phase |

## FINAL: 最後にまとめて作る画像

### 0. キャラ素体・進化・融合の大幅リデザイン

現状の相棒/進化/融合素材は、ゲーム内表示と図鑑の確認用として一旦維持する。  
ただしユーザー方針として、キャラ素体・進化・融合は後で大幅に作り直す。これは作業量が非常に大きいため、ステージ/ボス/UI/ゲーム機能が固まった後の最終フェーズに回す。

| 状態 | 内容 |
|---|---|
| REVIEW | 6素体 `Partner_S{1-6}_L0.png` |
| TEMP | 新規2素体 `Partner_S{7-8}_L0.png` |
| NEED | Halo Caster / Pulse Hydra の正式PNGセット。現状はコード側表現・暫定扱い |
| REVIEW | 全相棒ルート進化 `Partner_S{1-6}_R{1-3}_L{0-3}.png` |
| TEMP | 新規2素体のルート進化 `Partner_S{7-8}_R{1-3}_L{0-3}.png` |
| REVIEW | 種族別クロス進化 `Partner_S{1-6}_F{1-6}_L{0-3}.png` |
| TEMP | 新規2素体のクロス進化 `Partner_S{7-8}_F{1-6}_L{0-3}.png` |

作り直し時の基準:

- Cobalt / Ember / Sage / Hex / Drift / Iron / Wraith / Genesis の種族差を遠目でも分かるようにする
- Lv0 -> Lv3 で体格、装備、発光、シルエットが明確に進化する
- SPEED / POWER / GUARD の違いを、色だけでなく形でも出す
- クロス進化は「メインキャラがリンクを吸収した姿」に見えるようにする
- 現状の暫定素材に引っ張られすぎず、商用オリジナルIPとして再設計する

### 1. ルートL0不足分

進化ツリーやルートプレビューで、選択中の素体がルート別に見えるようにするための素材。  
`Tools/GenerateMissingVariantAssets.ps1` で全18枚を暫定生成済み。

| 状態 | 枚数 | 内容 |
|---|---:|---|
| TEMP | 18 | `Partner_S{1-6}_R{1-3}_L0.png` |

### 2. 種族別クロス進化

今は `Fusion_1.png` から `Fusion_6.png` の汎用絵があるだけ。  
ユーザー方針は「リンク同士ではなく、メインキャラがリンクを吸収して見た目が変わる」なので、種族別クロス進化が本命。

最初に作る最小セット:

| 状態 | 枚数 | 内容 |
|---|---:|---|
| TEMP | 36 | `Partner_S{1-6}_F{1-6}_L3.png` |

余力があれば作る完全セット:

| 状態 | 枚数 | 内容 |
|---|---:|---|
| TEMP | 108 | `Partner_S{1-6}_F{1-6}_L{0-2}.png` |

`L0-L3` の全144枚は暫定生成済み。次にやるなら、`AssetReview_FusionStages.png` で段階変化が気持ちよく見えるか確認し、作り直し候補を選ぶ。

### 3. 全相棒ルート進化の品質統一

現在 `Partner_S{1-6}_R{1-3}_L{1-3}.png` は大部分が存在するが、暫定生成が混ざっている。  
次の品質基準で全54枚を見直す。

| 状態 | 枚数 | 内容 |
|---|---:|---|
| REVIEW | 54 | `Partner_S{1-6}_R{1-3}_L{1-3}.png` |

チェック観点:

- 名前とシルエットが一致しているか。Foxが鳥に見える、Catが鳥に見える、Bearが猫に見える等はNG。
- Lv1 -> Lv2 -> Lv3で明確に大きく/強く/派手になっているか。
- SPEED/POWER/GUARDの差が遠目でも分かるか。
- 背景なし、透過、ゲーム画面で小さくしても読めるか。

## P1: 敵・ボス画像

### 既存外部PNG

| 状態 | ファイル | コメント |
|---|---|---|
| DONE | `Runner.png` | 外部PNGあり |
| DONE | `Brute.png` | 外部PNGあり |
| DONE | `Shooter.png` | 外部PNGあり |
| DONE | `Boss.png` | 外部PNGあり |
| DONE | `Boss_Warning.png` | 演出用 |

### まだ外部PNGとして読んでいない敵

現在コード内では procedural 生成。画像差し替えしたい場合は、先に `LoadOptionalSprite` 対応を追加する。

| 状態 | ファイル | 必要対応 |
|---|---|---|
| TEMP | `Dasher.png` | `CreateAssets()` で `LoadOptionalSprite("Skins/Dasher", MakeDasherSprite())` 対応済み |
| TEMP | `Bomber.png` | `CreateAssets()` で `LoadOptionalSprite("Skins/Bomber", MakeBomberSprite())` 対応済み |
| TEMP | `Phantom.png` | `CreateAssets()` で `LoadOptionalSprite("Skins/Phantom", MakePhantomSprite())` 対応済み |

追加で欲しい敵バリエーション:

| 優先 | 状態 | 内容 |
|---|---|---|
| P1 | NEED | Elite Runner / Elite Brute / Elite Shooter |
| P1 | NEED | Wave 10 Boss 第2形態 |
| P2 | NEED | 小型雑魚2種、遠距離雑魚1種、トラップ敵1種 |

## P1: アニメーション用フレーム

今は単一PNGをコード演出で揺らしている。  
次の段階は本物のフレームアニメ。まだコード側の命名ロードは未実装なので `HOOK` 扱い。

推奨命名:

| 用途 | ファイル名 |
|---|---|
| 待機 | `{BaseName}_Idle_0.png` から `{BaseName}_Idle_3.png` |
| 移動 | `{BaseName}_Move_0.png` から `{BaseName}_Move_3.png` |
| 攻撃 | `{BaseName}_Attack_0.png` から `{BaseName}_Attack_3.png` |
| 被弾 | `{BaseName}_Hit_0.png` から `{BaseName}_Hit_1.png` |

最初に作るべき対象:

| 優先 | 状態 | 対象 |
|---|---|---|
| P1 | HOOK | プレイヤー現在フォーム用 `Player_*` |
| P1 | HOOK | 6素体 `Partner_S{1-6}_L0` |
| P1 | HOOK | 敵 `Runner`, `Brute`, `Shooter`, `Dasher`, `Bomber`, `Phantom` |
| P2 | HOOK | 全進化/融合フォーム |

## P1: HUD・MAP専用素材

HUD/MAP は常に画面に出るので、キャラ画像と同じくらい売り物感に効く。  
今はコード生成のネオン枠や単色バーが中心なので、画像化すると「前に送ってくれた理想画像」の密度にかなり近づけられる。

### HUD: 常時表示

| 優先 | 状態 | ファイル | 用途 | 必要対応 |
|---|---|---|---|---|
| P1 | REVIEW | `HUD_Panel_Wave.png` | 左上 Wave/相棒名パネル | 不採用中。低解像度/引き伸ばし問題。再利用は9-sliceか専用比率で |
| P1 | REVIEW | `HUD_Panel_HP.png` | 自分HP/コアHPパネル | 不採用中。コード生成パネルを使用 |
| P1 | REVIEW | `HUD_Panel_Level.png` | レベル/EXPパネル | 不採用中。コード生成パネルを使用 |
| P1 | REVIEW | `HUD_Panel_ChipMini.png` | 小型データチップ表示 | 不採用中。アイコンのみ使用 |
| P1 | REVIEW | `HUD_Panel_Links.png` | LINKS表示 | 不採用中。コード生成パネルを使用 |
| P2 | REVIEW | `HUD_Panel_Status.png` | ESC中の詳細ステータス | 不採用中。コード生成パネルを使用 |
| P2 | REVIEW | `HUD_Panel_Loadout.png` | ACTIVE BUILD/取得ビルド詳細 | 不採用中。コード生成パネルを使用 |
| P1 | REVIEW | `HUD_Bar_Back.png` | HP/EXP共通バー背景 | 不採用中。コード生成バーを使用 |
| P1 | REVIEW | `HUD_Bar_HP_PlayerFill.png` | 自分HPの水色fill | 不採用中。単色fillを使用 |
| P1 | REVIEW | `HUD_Bar_HP_CoreFill.png` | コアHPの黄色fill | 不採用中。単色fillを使用 |
| P1 | REVIEW | `HUD_Bar_HP_Lag.png` | 被弾時の遅延赤バー | 不採用中。単色lag fillを使用 |
| P1 | REVIEW | `HUD_Bar_EXP_Fill.png` | EXPの黄緑fill | 不採用中。単色fillを使用 |
| P2 | DONE | `HUD_Icon_PlayerHP.png` | 自分HPアイコン | HPラベル横に配置済み |
| P2 | DONE | `HUD_Icon_CoreHP.png` | コアHPアイコン | HPラベル横に配置済み |
| P2 | DONE | `HUD_Icon_EXP.png` | EXPアイコン | EXPラベル横に配置済み |
| P2 | DONE | `Pickup_Data.png` | データチップアイコン流用 | 既存素材をHUDへ流用可能 |
| P2 | DONE | `HUD_Icon_DataChip.png` | HUD専用チップアイコン | Chip/Dataパネルに配置済み |

### HUD: 戦闘進行・警告

| 優先 | 状態 | ファイル | 用途 | 必要対応 |
|---|---|---|---|---|
| P1 | REVIEW | `HUD_WaveProgress_Frame.png` | 上部Wave進行バー枠 | 不採用中。コード生成バーを使用 |
| P1 | REVIEW | `HUD_WaveProgress_Fill_Normal.png` | 通常Wave進行fill | 不採用中。単色fillを使用 |
| P1 | REVIEW | `HUD_WaveProgress_Fill_Boss.png` | Boss Wave進行fill | 不採用中。単色fillを使用 |
| P1 | REVIEW | `HUD_BossBar_Frame.png` | Boss HPバー枠 | 不採用中。コード生成バーを使用 |
| P1 | REVIEW | `HUD_BossBar_Fill.png` | Boss HPバーfill | 不採用中。単色fillを使用 |
| P1 | REVIEW | `HUD_ChoiceDim.png` | カード選択中の暗転板 | 不採用中。単色暗転を使用 |
| P1 | REVIEW | `HUD_DangerVignette_Player.png` | 自分HP低下時の画面端警告 | 不採用中。単色警告を使用 |
| P1 | REVIEW | `HUD_DangerVignette_Core.png` | コアHP低下時の画面端警告 | 不採用中。単色警告を使用 |
| P2 | REVIEW | `HUD_BossWarning_Banner.png` | Boss/大型Waveの警告帯 | 不採用中。コード生成slashを使用 |
| P2 | REVIEW | `HUD_RelicSlot_Frame.png` | レリック表示枠 | 不採用中。コード生成枠を使用 |
| P2 | DONE | `Relic_Magnet.png` | 磁力核アイコン | レリックカード/HUDに適用済み |
| P2 | DONE | `Relic_Deathless.png` | 不滅の証アイコン | レリックカード/HUDに適用済み |
| P2 | DONE | `Relic_CorePulse.png` | コアの鼓動アイコン | レリックカード/HUDに適用済み |
| P2 | DONE | `Relic_Explosion.png` | 爆炎の紋章アイコン | レリックカード/HUDに適用済み |
| P2 | DONE | `Relic_Storm.png` | 嵐の心臓アイコン | レリックカード/HUDに適用済み |
| P2 | DONE | `Relic_Mirror.png` | 鏡の誓約アイコン | レリックカード/HUDに適用済み |

### ミニマップ

Status: REMOVED / do not hook unless the minimap feature is revived.

| 優先 | 状態 | ファイル | 用途 | 必要対応 |
|---|---|---|---|---|
| - | REMOVED | `Minimap_Frame.png` | 旧ミニマップ外枠 | ミニマップ機能撤去済み。復活時のみ再評価 |
| - | REMOVED | `Minimap_Backplate.png` | 旧ミニマップ背景 | 同上 |
| - | REMOVED | `Minimap_Grid.png` | 旧レーダー線 | 同上 |
| - | REMOVED | `Minimap_Dot_Player.png` | 旧プレイヤー点 | 同上 |
| - | REMOVED | `Minimap_Dot_Core.png` | 旧コア点 | 同上 |
| - | REMOVED | `Minimap_Dot_Enemy.png` | 旧敵点 | 同上 |
| - | REMOVED | `Minimap_Dot_Boss.png` | 旧ボス点 | 同上 |
| - | REMOVED | `Minimap_Dot_Pickup.png` | 旧ピックアップ点 | 同上 |
| - | REMOVED | `Minimap_Ping_Wave.png` | 旧Wave Ping | 同上 |

### ワールド/マップ

| 優先 | 状態 | ファイル | 用途 | 必要対応 |
|---|---|---|---|---|
| P1 | TEMP | `Floor_TileA.png` | 床タイル | 2026-05-19 生成v1。`CreateAssets` で優先ロード |
| P1 | TEMP | `Floor_TileB.png` | 床タイル | 2026-05-19 生成v1。`CreateAssets` で優先ロード |
| P1 | TEMP | `Floor_TileC.png` | 床タイル | 2026-05-19 生成v1。`CreateAssets` で優先ロード |
| P1 | TEMP | `Floor_DarkBase_A.png` | 床の大判ベース | `CreateWorld` で背景スプライト化済み |
| P1 | TEMP | `Floor_GridOverlay_A.png` | ネオン格子/細かい床線 | 床タイル上に薄く重ねる対応済み |
| P1 | TEMP | `Floor_Crack_A.png` | 破損床 | ランダム装飾配置済み |
| P1 | TEMP | `Floor_Crack_B.png` | 破損床バリエーション | ランダム装飾配置済み |
| P1 | TEMP | `Floor_Cable_A.png` | サイバー配線 | ランダム装飾配置済み |
| P1 | TEMP | `Floor_Cable_B.png` | 配線バリエーション | ランダム装飾配置済み |
| P1 | TEMP | `Floor_CoreMark_A.png` | コア周辺の円形印 | コア足元に固定配置済み |
| P1 | TEMP | `Boundary_Stone_A.png` | 外周石/瓦礫 | `Boundary Stone` 差し替え済み |
| P1 | TEMP | `Boundary_Stone_B.png` | 外周石バリエーション | 外周の単調さ軽減に使用中 |
| P2 | HOOK | `Boundary_Gate_A.png` | ボス/次Wave方向のゲート | Wave演出用 |
| P1 | TEMP | `Prop_ServerDebris_A.png` | 壊れたサーバー破片 | マップ密度アップに使用中 |
| P1 | TEMP | `Prop_ServerDebris_B.png` | 破片バリエーション | マップ密度アップに使用中 |
| P1 | TEMP | `Prop_NeonPylon_A.png` | 発光オブジェクト | ランドマークとして配置済み |
| P1 | TEMP | `Prop_DataTerminal_A.png` | 小型端末/祭壇 | データ世界感の補強として配置済み |
| P2 | TEMP | `Prop_Crate_A.png` | 低い障害物風装飾 | 見た目のみ。衝突なしで配置済み |
| P1 | TEMP | `Core_Platform.png` | コア台座 | `Lantern` 足元を強化済み |
| P1 | TEMP | `Core_RingOuter.png` | コア外周リング | コアの重要感を上げる配置済み |
| P1 | TEMP | `Core_RingInner.png` | コア内周リング | 同上 |
| P2 | TEMP | `Core_DamageCrack_1.png` | コア軽ダメージ床 | コアHP割合で表示済み |
| P2 | TEMP | `Core_DamageCrack_2.png` | コア中ダメージ床 | コアHP割合で表示済み |
| P2 | TEMP | `Core_DamageCrack_3.png` | コア重ダメージ床 | コアHP割合で表示済み |
| P2 | TEMP | `Spawn_Ring_Normal.png` | 通常敵スポーン予告 | Wave開始リングに使用中 |
| P2 | TEMP | `Spawn_Ring_Boss.png` | ボス出現予告 | Boss Waveリングに使用中 |

### サイズ目安

| 種類 | 推奨サイズ | メモ |
|---|---:|---|
| HUDパネル | 256x96 / 384x128 | 9-slice未実装なので、最初は固定サイズ用で作る |
| HUDバー | 256x24 | 背景/fillを分ける |
| ミニマップ | - | 機能撤去済み。復活判断までは新規生成/接続しない |
| ミニマップdot | - | 機能撤去済み。復活判断までは新規生成/接続しない |
| 床タイル | 128x128 | 今の床密度に合う |
| 床デカール | 128x128 / 256x256 | 透過PNGでランダム回転 |
| 外周/小物 | 96x96 / 128x128 | 上から見ても形が読めるシルエット |
| コア台座/リング | 256x256 | `Lantern.png` と重ねる前提 |

## P2: UI・演出素材

### 既存

| 状態 | 内容 |
|---|---|
| DONE | `Card_Cyan`, `Card_Gold`, `Card_Green`, `Card_Magenta`, `Card_Red` |
| DONE | `Panel_FrameCyan`, `Panel_FrameGold` |
| DONE | `Badge_*` 系 |
| DONE | `Bullet_*` 系 |
| DONE | `Impact_*` 系 |
| DONE | `Evolution_Ring` |
| DONE | `Pickup_*`, `Heal_Orb`, `Heart_Orb` |

### 追加候補

| 優先 | 状態 | ファイル | 用途 |
|---|---|---|---|
| P2 | NEED | `Cutin_Evolve_Speed.png` | SPEED進化カットイン背景 |
| P2 | NEED | `Cutin_Evolve_Power.png` | POWER進化カットイン背景 |
| P2 | NEED | `Cutin_Evolve_Guard.png` | GUARD進化カットイン背景 |
| P2 | NEED | `Cutin_Fusion.png` | クロス進化カットイン背景 |
| P2 | NEED | `MainMenu_Background.png` | メインメニュー専用背景 |
| P2 | NEED | `Result_Clear_Background.png` | クリアリザルト背景 |
| P2 | NEED | `Result_GameOver_Background.png` | 敗北リザルト背景 |

## P2: 背景・マップ素材

詳細な必要素材は `P1: HUD・MAP専用素材` に昇格済み。  
ここでは、追加候補として特に目立つものだけを残す。既存床タイルはあるが、マップが狭く単調に見えやすいので床の密度とランドマークを増やす。

| 優先 | 状態 | ファイル | 用途 |
|---|---|---|---|
| P2 | TEMP | `Floor_TileA.png` | 床 |
| P2 | TEMP | `Floor_TileB.png` | 床 |
| P2 | TEMP | `Floor_TileC.png` | 床 |
| P2 | TEMP | `Floor_Crack_A.png` | 破損床 |
| P2 | TEMP | `Floor_Cable_A.png` | サイバー配線 |
| P2 | TEMP | `Floor_CoreMark_A.png` | コア周辺の円形印 |
| P2 | TEMP | `Prop_ServerDebris_A.png` | 壊れたサーバー破片 |
| P2 | TEMP | `Prop_NeonPylon_A.png` | 発光オブジェクト |
| P2 | TEMP | `Boundary_Stone_A.png` | 外周石の高品質版 |

## 生成バッチ順

### Batch 1: ルートL0不足分

完了。`Tools/GenerateMissingVariantAssets.ps1` で全18枚を暫定生成済み。  
確認用: `Assets/ArtSource/Generated_MissingVariant_Preview.png`

### Batch 2: 種族別クロス進化 L3

完了。`Tools/GenerateMissingVariantAssets.ps1` で36枚を暫定生成済み。  
確認用: `Assets/ArtSource/Generated_MissingVariant_Preview.png`

### Batch 3: 既存ルート進化54枚の品質レビュー

作り直しが必要なものだけ差し替える。  
特に `Hex Cat`, `Drift Fox`, `Iron Bear` は名前とシルエット一致を厳しく見る。

### Batch 4: HUD可読性パック

画像生成は完了。`Tools/GenerateHudAssets.ps1` で再生成可能。  
確認用: `Assets/ArtSource/Generated_HudAsset_Preview.png`  
コード側のHOOKは未適用なので、Claude側の `CoreLanternGame.cs` 作業が落ち着いてから接続する。

ゲーム中ずっと見えるので、キャラ差し替えと同じくらい優先度が高い。  
先に `CreatePanel`, `CreateHudBar`, `CreateWaveProgressPanel`, `CreateMinimap` の画像HOOKを入れる。

生成済み対象:

- `HUD_Panel_Wave.png`, `HUD_Panel_HP.png`, `HUD_Panel_Level.png`, `HUD_Panel_ChipMini.png`
- `HUD_Bar_Back.png`, `HUD_Bar_HP_PlayerFill.png`, `HUD_Bar_HP_CoreFill.png`, `HUD_Bar_EXP_Fill.png`
- `HUD_WaveProgress_Frame.png`, `HUD_WaveProgress_Fill_Normal.png`, `HUD_WaveProgress_Fill_Boss.png`
- `Minimap_Frame.png`, `Minimap_Backplate.png`, `Minimap_Grid.png`, `Minimap_Dot_Player.png`, `Minimap_Dot_Core.png`, `Minimap_Dot_Enemy.png`

### Batch 5: MAP密度パック

「暗いサイバー床、ネオン発光、情報密度高め」に寄せるための環境素材。  
先に `CreateWorld` で床デカール/小物/コア台座の任意ロードと配置を作る。

作成対象:

- `Floor_DarkBase_A.png`, `Floor_GridOverlay_A.png`, `Floor_Crack_A.png`, `Floor_Cable_A.png`, `Floor_CoreMark_A.png`
- `Boundary_Stone_A.png`, `Boundary_Stone_B.png`
- `Prop_ServerDebris_A.png`, `Prop_ServerDebris_B.png`, `Prop_NeonPylon_A.png`, `Prop_DataTerminal_A.png`
- `Core_Platform.png`, `Core_RingOuter.png`, `Core_RingInner.png`

### Batch 6: 敵3種の外部化

完了。`Tools/GenerateEnemyExternalAssets.ps1` で `Dasher.png`, `Bomber.png`, `Phantom.png` を暫定生成済み。  
コードも `LoadOptionalSprite` 対応済み。確認用: `Assets/ArtSource/Generated_EnemyExternal_Preview.png`

### Batch 7: クロス進化 L0-L2 段階素材

完了。`Tools/GenerateFusionStageAssets.ps1` で `Partner_S{1-6}_F{1-6}_L{0-2}.png` 全108枚を暫定生成済み。  
確認用: `Assets/ArtSource/Generated_FusionStage_Preview.png`, `Assets/ArtSource/AssetReview_FusionStages.png`

### Batch 8: アニメーションフレーム

まずプレイヤー/敵の少数から。  
単一PNGの揺れ演出から、フレームアニメへ移行する。

### Batch 9: メニュー/リザルト/カットイン背景

売り物感を強化する最後の仕上げ。

完了。`Tools/GenerateNewPartnerRelicUiAssets.ps1` で以下を暫定生成済み。

- `MainMenu_Background.png`
- `Result_Clear_Background.png`
- `Result_GameOver_Background.png`
- `Cutin_Evolve_Speed.png`
- `Cutin_Evolve_Power.png`
- `Cutin_Evolve_Guard.png`
- `Cutin_Fusion.png`

確認用: `Assets/ArtSource/Generated_NewPartnerRelicUi_Preview.png`

### Batch 10: 新規style7/8 + レリック/HUD追加

完了。`Tools/GenerateNewPartnerRelicUiAssets.ps1` で暫定生成済み。

- `Partner_S7_L0.png`, `Partner_S8_L0.png`
- `Partner_S{7-8}_R{1-3}_L{0-3}.png` 全24枚
- `Partner_S{7-8}_F{1-6}_L{0-3}.png` 全48枚
- `Relic_Magnet.png`, `Relic_Deathless.png`, `Relic_CorePulse.png`, `Relic_Explosion.png`, `Relic_Storm.png`, `Relic_Mirror.png`
- `HUD_Panel_Status.png`, `HUD_Panel_Loadout.png`, `HUD_BossBar_Frame.png`, `HUD_BossBar_Fill.png`, `HUD_ChoiceDim.png`, `HUD_DangerVignette_Player.png`, `HUD_DangerVignette_Core.png`, `HUD_BossWarning_Banner.png`, `HUD_RelicSlot_Frame.png`
- `HUD_Icon_PlayerHP.png`, `HUD_Icon_CoreHP.png`, `HUD_Icon_EXP.png`, `HUD_Icon_DataChip.png`
- `Minimap_Dot_Pickup.png`, `Minimap_Ping_Wave.png`, `Boundary_Gate_A.png`

注意: MAP以外のHUD/Relic/背景系はコード側のロード・表示HOOK自体はある。低密度背景/カットイン/ボス背景は採用済みだが、HUD/カード/警告帯などの大型UI画像はカテゴリ別フラグOFFのまま。小アイコンは採用している。

## 画像生成用テンプレ

```text
Original monster partner sprite for a commercial indie roguelike survivor game.
Dark cyber dungeon tone, neon rim light, cute but cool creature, compact readable silhouette, transparent background.
Do not copy any existing franchise character. No text, no logo.
Top-down/isometric 2D game sprite, centered, full body visible, high contrast, readable at small size.
Species: {species name and animal motif}
Evolution route: {SPEED / POWER / GUARD / FUSION name}
Stage: {L0/L1/L2/L3}
Color accents: {cyan/gold/green/magenta etc}
```

## 次にやるなら

1. `AssetReview_PartnerRoutes.png`, `AssetReview_FusionStages.png`, `Generated_NewPartnerRelicUi_Preview.png` を見て、作り直し候補を決める。
2. 8素体前提で、既存ルート進化と新規style7/8の品質レビューを進める。
3. 大型UI画像を使うなら、先に高解像度/比率固定/9-slice前提で作り直す。現状はコード生成UI + 小アイコンが推奨。
4. 敵の次段階として Elite Runner / Elite Brute / Elite Shooter や Boss第2形態を検討する。
5. その後、idle / move / attack / hit のフレームアニメへ進む。
