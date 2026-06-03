# CoreLanternUnity Asset Review Report

Generated: 2026-05-21 11:29:41  
Updated: 2026-06-02 (Codex partner L0 pixel-art redesign / Claude title lineup waiting)

Latest code hook update: 2026-05-22 (Codex background/cut-in placement)

## 2026-06-02 Partner L0 Pixel-Art Redesign

- Replaced the six base partner runtime PNGs with high-resolution pixel-art-style 512x512 transparent assets:
  `Partner_S1_L0`, `Partner_S2_L0`, `Partner_S3_L0`, `Partner_S4_L0`, `Partner_S5_L0`, `Partner_S6_L0`.
- Follow-up fix: `Partner_S4_L0` Hex Cat was too dark/semi-transparent on dark backgrounds, so Codex repaired opacity/readability, removed a detached chroma-key scrap, and restored its baseline to `bottom_y=448`.
- Follow-up fix: `Partner_S5_L0` Drift Fox had detached chroma-key scraps and weak pink readability. Codex removed detached artifacts, strengthened pink visibility, and preserved `bottom_y=448`.
- Asset identity check:
  Cobalt Pup = blue wolf pup, Ember Drake = orange drake, Sage Hare = green hare,
  Hex Cat = purple dark cat, Drift Fox = pink fox, Iron Bear = black armored bear.
- All six are normalized for title lineup use:
  `bottom_y=448`, similar body height, transparent corners, no baked ground shadow.
- Existing `.meta` files were intentionally preserved. PNGs only were overwritten.
- Generated review files:
  `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`,
  `CharacterPixelArt_6partners_shrink_check.png`,
  `CharacterPixelArt_6partners_lineup_clean_transparent.png`,
  `validation_report.json`.
- Added `Tools/PolishPartnerL0ColorIdentity.ps1` to correct S4/S5 color identity after secretary-style review found them slightly under-read as purple/pink.
- Added Claude hook-up guidance:
  `docs/CLAUDE_TITLE_6PARTNER_LINEUP_HOOK_SPEC.md`.
- Validation: image resources warnings 0 / errors 0; compile health quick passed; S1-S6 `.meta` files end with byte `0A`.
- `Assets/Scripts/CoreLanternGame.cs` was not edited in this pass.

## 2026-06-01 Audio Comfort Pass

- Regenerated all placeholder BGM/SE via `Tools/GenerateStarterAudio.ps1`.
- Reduced high-frequency/noise content in rapid-fire SE: `Shoot`, `Hit`, `Kill`.
- Reworked Stage1-5 and boss BGM to avoid obvious upward-moving phrases and sit lower/steadier for long-session play.
- Added legacy boss BGM aliases so older code paths no longer produce optional fallback warnings:
  `BGM_BossPulswyrm.wav`, `BGM_BossNullwyrm.wav`.
- Added Japanese review note: `docs/AUDIO_COMFORT_PASS_20260601.md`.
- Validation: `Tools/TestAudioResources.ps1` warnings 0 / errors 0; image resources warnings 0 / errors 0; compile health quick passed.
- `Assets/Scripts/CoreLanternGame.cs` was not edited in this pass.

## 2026-06-01 Non-Character Asset Queue

- Added `Tools/GenerateNonCharacterQueuePreview.ps1`.
- Generated `Assets/ArtSource/NonCharacterQueue_20260601/NonCharacterAssetQueue_20260601.png`.
- Added `docs/NON_CHARACTER_ASSET_QUEUE_20260601.md` in Japanese to separate safe Codex asset/doc work from Claude-owned code hook-up.
- Updated `Tools/ReportAssetStatus.ps1` so A egg presentation assets are counted explicitly:
  `Present 3 / Hooked 0 / Waiting 3`.
- Current non-character waiting snapshot:
  HUD9 waiting 8, Stage4/5 support waiting 6, V2 UI waiting 14, A egg presentation waiting 3.

## 2026-06-01 A Egg Presentation Assets

- Added title/result/icon presentation variants for the selected A egg direction:
  `Title_CoreEgg_ASelected_v1`, `Result_CoreEgg_ASelected_v1`, and `Icon_CoreEgg_ASelected_v1`.
- These are asset-only and intentionally not connected to runtime code because Claude is available again and owns `CoreLanternGame.cs` hook-up.
- Preview: `Assets/ArtSource/EggCoreVisual_20260601/title_result_support_A_20260601/TitleResultCoreEgg_ASelected_v1_preview.png`.
- Validation: all three PNGs have transparent corners and `.meta` files.
- Hook-up guidance: `docs/CLAUDE_A_EGG_PRESENTATION_HOOK_SPEC.md`.
- Follow-up validation passed: image resources warnings 0 / errors 0, static sprite refs missing 0, compile health quick passed.
- Secretary review: OK for Claude handoff. Main caution is not placing title/progress text over the title asset's faint circular field.

## 2026-06-01 HUD9 Wave Progress Partial Hook + Asset Status Tool

- Added `Tools/ReportAssetStatus.ps1` as a read-only report for generated assets vs static code references.
- HUD9 9-slice candidates are no longer all purely pending: `HUD9_WaveProgress_Frame`, `HUD9_WaveProgress_Fill_Normal`, and `HUD9_WaveProgress_Fill_Boss` are hooked to the top Wave progress bar.
- Remaining HUD9 panel/bar assets are still candidate-only until the Wave progress bar is visually approved in Unity.
- Latest resource snapshot from validation: 591 active `Assets/Resources/Skins/*.png`, 180 static code sprite references, image resource errors 0.

## 2026-06-01 Player Form4 Fallback Cleanup

- Restored `Assets/Resources/Skins/Player_Form4.png` and `.meta` from the existing disabled backup files.
- This removes the recurring optional sprite fallback warning from `Tools/TestImageResources.ps1`.
- The disabled backup files remain in place for rollback/reference.
- Latest image validation after restore: 592 active skins, missing optional sprite refs 0, warnings 0, errors 0.

## 2026-06-01 Stage 4/5 Support Hook Spec

- Added `docs/CLAUDE_STAGE45_SUPPORT_HOOK_SPEC.md`.
- The six Stage 4/5 support PNGs are validated and present, but still not statically loaded by `CoreLanternGame.cs`.
- Hook-up is intentionally left to Claude/Core work because it touches stage thumbnails and hazard visuals.

## 2026-05-25 Stage/Boss Enemy Sprite Pack

- Added `Tools/GenerateStageBossEnemyAssets.ps1`.
- Generated 7 transparent 256x256 PNGs that are already referenced by existing `LoadOptionalSprite` paths:
  `LavaCrawler`, `Enemy_MagmaTitan`, `Enemy_CorruptionDrone`, `Enemy_FrostKnight`, `Enemy_VoltDasher`, `Boss_Pulswyrm`, `Boss_Nullwyrm`.
- Generated preview sheet: `Assets/ArtSource/Generated_StageBossEnemy_Preview.png`.
- Validation passed: all 7 PNGs are 256x256 and all four corners have alpha 0.
- No `Assets/Scripts/CoreLanternGame.cs` changes were needed.

## 2026-05-25 Stage 4/5 Support Asset Pack

- Added `Tools/GenerateStage45SupportAssets.ps1`.
- Generated 6 Stage 4/5 support PNGs:
  `StageThumb_Frost`, `StageThumb_Storm`, `Stage4_FrostCrystal_A`, `Stage4_FrostPatch_A`, `Stage5_LightningMarker_A`, `Stage5_LightningStrike_A`.
- Generated preview sheet: `Assets/ArtSource/Generated_Stage45Support_Preview.png`.
- Validation passed: thumbnails are 160x90 opaque PNGs; hazard/VFX files keep transparent corners.
- Debug update: generated missing `.meta` files for all Stage 4/5 support PNGs and the preview sheet; added meta backfill to the generator.
- No `Assets/Scripts/CoreLanternGame.cs` changes were made; these are prepared for the existing Stage 4/5 fallback replacement work and are not yet auto-displayed.

## 2026-05-22 Non-Character Polish Batch 2

- Added `Tools/GenerateNonCharacterPolishBatch2.ps1`.
- Generated 32 additional non-character assets and preview sheet `Assets/ArtSource/Generated_NonCharacterPolishBatch2_Preview.png`.
- Added 4 relic icons: `Relic_Titan`, `Relic_Overdrive`, `Relic_DataSurge`, `Relic_Apex`.
- Added 20 module-card icons: `ModuleIcon_*`.
- Added stage/support assets: `StageThumb_*`, `Stage2_LavaPool_A`, `Stage2_HeatVent_A`, `Stage3_RelayDevice_A`, `Stage3_CorruptionPatch_A`, `Pickup_Data_64`.
- Hooked the 4 new relic icons into relic card/HUD display and hooked module-card icons into upgrade card display.
- Roslyn compile check passed. Remaining warnings are existing analyzer load warnings and unused fields.

## 2026-05-22 Background/Cut-in Placement Pass

- Enabled low-density screen backgrounds in `Assets/Scripts/CoreLanternGame.cs`.
- Hooked `Background_MainMenu`, `Background_Victory`, `Background_Defeat` into main menu and result screens with `preserveAspect = true`.
- Hooked `Background_Evolution` / `Cutin_Evolve_*` / `Background_CrossEvolution` into evolution and cross-evolution cut-ins with `preserveAspect = true`.
- Added previously unused `Background_BossPulswyrm` and `Background_BossNullwyrm` to the boss cutscene.
- Kept `UseGeneratedHudBars`, `UseGeneratedHudPanels`, `UseGeneratedCards`, and `UseGeneratedOverlayImages` disabled to avoid stretched/noisy HUD, card, warning, and vignette images.
- Roslyn compile check passed. Remaining warnings are existing analyzer load warnings, obsolete `FindFirstObjectByType`, and unused `allyName`.

## 2026-05-22 Documentation Pass

- Added `docs/STAGE_DESIGN_SPEC.md` for Stage 2 lava area and Stage 3 story-gimmick candidates.
- Added `docs/BOSS_PHASE2_SPEC.md` for Nullwyrm Phase 2 and Pulswyrm mini-enrage guidance.
- Added `docs/CHARACTER_REDESIGN_SPEC.md` as a draft for 9 partner silhouettes, L0-L3 evolution direction, and cross-evolution identity.
- Added `docs/SOUND_ASSET_CANDIDATES.md` with commercial-safe candidate sources and license reference links.
- No code files were changed in this pass.

## 2026-05-22 Background Regeneration Pass

- Updated `Tools/GeneratePolishedNonCharacterAssets.ps1` background section only.
- Regenerated low-density 1920x1080 backgrounds: `Background_MainMenu`, `Background_Evolution`, `Background_CrossEvolution`, `Background_Victory`, `Background_Defeat`, `Background_BossPulswyrm`, `Background_BossNullwyrm`.
- Regenerated code-compatible aliases with the same backdrop function: `MainMenu_Background`, `Cutin_*`, `Result_*`.
- Removed full-screen grids, red vertical stripes, central rings, and diagonal banding.
- Latest backup: `Assets/ArtSource/NonCharacterAssetBackup_20260522_003259/`.
- No `Assets/Scripts/CoreLanternGame.cs` changes.

## Generated Review Sheets
- `Assets/ArtSource/AssetReview_PartnerRoutes.png`
- `Assets/ArtSource/AssetReview_FusionL3.png`
- `Assets/ArtSource/AssetReview_FusionStages.png`
- `Assets/ArtSource/AssetReview_EnemiesMapUi.png`
- `Assets/ArtSource/Generated_HudAsset_Preview.png`
- `Assets/ArtSource/Generated_FusionStage_Preview.png`
- `Assets/ArtSource/Generated_NewPartnerRelicUi_Preview.png`
- `Assets/ArtSource/Generated_PolishedNonCharacterAsset_Preview.png`
- `Assets/ArtSource/Generated_NonCharacterPolishBatch2_Preview.png`
- `Assets/ArtSource/Generated_StageBossEnemy_Preview.png`
- `Assets/ArtSource/Generated_Stage45Support_Preview.png`

## Inventory Snapshot

| Category | Count | Notes |
|---|---:|---|
| All active PNG | 502 | Assets/Resources/Skins |
| Partner PNG | 296 | Includes route and fusion variants for species 1-8. Halo Caster / Pulse Hydra PNG sets are not yet represented in this inventory |
| Route PNG | 12 | Generic route sprites |
| Fusion PNG | 6 | Generic fusion sprites |
| Disabled/backup files | 26 | Kept for rollback/reference |
| Relic PNG | 10 | Relic icons; all current relic IDs have a dedicated image |
| ModuleIcon PNG | 20 | Upgrade-card category icons |
| HUD PNG | 26 | HUD/UI hook targets |
| Minimap PNG | 9 | Minimap assets |
| Polished non-character targets | 100 | Original 68 + Batch2 32 small/utility assets |

## Non-Character Asset Status

| Group | Count / status | Current decision |
|---|---:|---|
| Active PNG in `Assets/Resources/Skins` | 502 | Includes the 2026-05-25 Stage/Boss enemy and Stage 4/5 support packs |
| Polished non-character targets | 68 | Generated and awaiting staged adoption |
| Old stretched large UI images | Deprecated | Do not re-enable directly; recreate or use only with 9-slice/aspect-safe hooks |
| New polished Stage 1 background | Generated / Codex-enabled | Menu/result/boss backgrounds now use aspect-preserving hooks |
| Low-density background pass | Generated / Review | Backgrounds now use quiet tone fields with edge-only decoration |
| `Pickup_Data.png` | 116x112 | Keep for now; create a 64x64 icon variant only if UI needs it |

## Generated Non-Character File Groups

- Cards: `Card_Cyan`, `Card_Gold`, `Card_Green`, `Card_Magenta`, `Card_Red`
- HUD panels/bars: `HUD_Panel_*`, `HUD_Bar_*`, `HUD_WaveProgress_*`, `HUD_BossBar_*`, `HUD_RelicSlot_Frame`, `HUD_ChoiceDim`, `HUD_DangerVignette_*`, `HUD_BossWarning_Banner`
- Background/cut-in/result: `Background_*`, `MainMenu_Background`, `Cutin_*`, `Result_*`
- VFX/pickups/icons: `Bullet_*`, `Pickup_Heal`, `Effect_*`, `Icon_Link_*`, `Icon_Stat_*`
- Stage support: `Floor_DarkBase`, `Floor_Hazard_Lava`, `Arena_Boundary`
- Batch2 utility: `Relic_Titan`, `Relic_Overdrive`, `Relic_DataSurge`, `Relic_Apex`, `ModuleIcon_*`, `StageThumb_*`, `Stage2_*`, `Stage3_*`, `Pickup_Data_64`

## Missing / Next Targets

- Partner route set missing: 0 files.
- Species-specific fusion L0-L2 missing: 0 files. Full L0-L3 temporary set exists for all 8 species x 6 fusions.
- Enemy core set missing: 0 files.
- HUD/minimap dedicated asset targets missing: 0 files.

## Safe Parallel Work Queue

1. Review `AssetReview_PartnerRoutes.png` for name/silhouette mismatches, especially Hex, Drift, and Iron.
2. Review `AssetReview_FusionStages.png` for whether fusion reads as gradual absorption from L0 to L3.
3. Review `AssetReview_FusionL3.png` for whether fusion endpoints look like the selected species absorbed the link, not generic icons.
4. Review `Generated_PolishedNonCharacterAsset_Preview.png`, then keep large UI images asset-only until 9-slice/ratio-preserving hooks are confirmed.
5. Review `Generated_NewPartnerRelicUi_Preview.png` for Wraith/Genesis readability and relic icon clarity.
6. After Claude's code changes settle, hook HUD/relic/minimap assets from `Assets/Resources/Skins` into panel/bar/minimap/relic UI creation.
7. If Stage 2 work starts, use `docs/STAGE_DESIGN_SPEC.md` and treat `Floor_Hazard_Lava` as a local hazard accent, not a stretched full-screen texture.
8. If boss work starts, add explicit boss kind/phase fields before relying on HP thresholds.

## HUD Targets Still Missing
- None for the current P1 target list.
