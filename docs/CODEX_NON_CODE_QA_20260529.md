# Codex Non-Code QA / Handoff — 2026-05-29

## Scope

Claude is checking code-side issues in parallel, so this pass intentionally avoids editing:

- `Assets/Scripts/CoreLanternGame.cs`
- gameplay balance code
- UI layout code

Codex handled asset/resource hygiene and prepared a Claude-friendly non-image task queue.

## Completed In This Pass

### 2026-05-30 addendum: Result UI v2 exact-size draft prepared

Added result-screen layout pieces to reduce clutter without stretching generic panel images.

Generated files:

- `Assets/Resources/Skins/Result_DeckFrame_v2.png` — 960x580
- `Assets/Resources/Skins/Result_PortraitFrame_v2.png` — 220x220
- `Assets/Resources/Skins/Result_StatTile_v2.png` — 112x76
- `Assets/Resources/Skins/Result_BadgeStrip_Route_v2.png` — 220x44
- `Assets/Resources/Skins/Result_BadgeStrip_Fusion_v2.png` — 220x44
- `Assets/Resources/Skins/Result_MvpRow_v2.png` — 230x68
- `Assets/Resources/Skins/Result_SummaryPlate_v2.png` — 520x44
- `Assets/Resources/Skins/Result_FooterGuide_v2.png` — 520x28
- `Assets/Resources/Skins/Result_RankMedal_S_v2.png` — 128x128
- `Assets/Resources/Skins/Result_RankMedal_A_v2.png` — 128x128
- `Assets/Resources/Skins/Result_RankMedal_B_v2.png` — 128x128
- `Assets/Resources/Skins/Result_RankMedal_C_v2.png` — 128x128
- `Assets/Resources/Skins/Result_RankMedal_D_v2.png` — 128x128

Tool:

- `Tools/GenerateResultV2Assets.ps1`

Art-source copies:

- `Assets/ArtSource/Result_V2_20260530/`
- Preview: `Assets/ArtSource/Result_V2_20260530/Result_V2_Preview.png`

Validation:

- All thirteen files match intended dimensions.
- All thirteen files have transparent corners (`alpha=0`).
- No text is baked into the images.

Design note:

- Best hookup target is MVP row/stat tile first, not the whole result deck.
- Keep long module summaries clipped/wrapped; image plates should organize the screen, not force dense text into a single line.

### 2026-05-30 addendum: Title UI v2 non-text draft prepared

Added decorative title-screen assets that can make the main menu feel more like the game's face without baking in title text.

Generated files:

- `Assets/Resources/Skins/Title_CoreEmblem_v2.png` — 512x512
- `Assets/Resources/Skins/Title_SubtitlePlate_v2.png` — 580x38
- `Assets/Resources/Skins/Title_StatsRibbon_v2.png` — 820x54
- `Assets/Resources/Skins/Title_LogoUnderline_v2.png` — 620x22
- `Assets/Resources/Skins/Title_NavRail_v2.png` — 960x24
- `Assets/Resources/Skins/Title_CornerAccent_v2.png` — 160x160

Tool:

- `Tools/GenerateTitleUiV2Assets.ps1`

Art-source copies:

- `Assets/ArtSource/Title_UI_V2_20260530/`
- Preview: `Assets/ArtSource/Title_UI_V2_20260530/Title_UI_V2_Preview.png`

Validation:

- All six files match intended dimensions.
- All six files have transparent corners (`alpha=0`).
- `Title_CornerAccent_v2.png` was revised from a filled plate to line-only decoration to avoid covering or competing with title text.

Design note:

- Keep game title and subtitle as live Unity text.
- Layer `Title_CoreEmblem_v2.png` behind the title/start cluster at native size or uniform scale only.
- Use the rail/ribbon pieces as low-alpha accents; avoid full-screen stretching.

### 2026-05-30 addendum: Button v2 exact-size draft prepared

Added a button plate pack for later menu/result/setup polish. These are exact-size transparent PNGs, not full-screen panels or stretched generic frames.

Generated files:

- `Assets/Resources/Skins/Button_Primary_Start_v2.png` — 420x60
- `Assets/Resources/Skins/Button_MenuSecondary_v2.png` — 150x44
- `Assets/Resources/Skins/Button_RunStageChip_v2.png` — 180x108
- `Assets/Resources/Skins/Button_DangerChip_v2.png` — 80x56
- `Assets/Resources/Skins/Button_RunBack_v2.png` — 220x60
- `Assets/Resources/Skins/Button_RunStart_v2.png` — 280x60
- `Assets/Resources/Skins/Button_PauseResume_v2.png` — 320x48
- `Assets/Resources/Skins/Button_Wide_320x42_v2.png` — 320x42
- `Assets/Resources/Skins/Button_OptionsToggle_v2.png` — 250x36
- `Assets/Resources/Skins/Button_Stepper_v2.png` — 44x30
- `Assets/Resources/Skins/Button_Reroll_v2.png` — 440x52
- `Assets/Resources/Skins/Button_SkipReward_v2.png` — 300x52
- `Assets/Resources/Skins/Button_ResultRetry_v2.png` — 340x64
- `Assets/Resources/Skins/Button_ResultMenu_v2.png` — 240x50
- `Assets/Resources/Skins/Button_CloseSmall_v2.png` — 240x32
- `Assets/Resources/Skins/Button_CloseWide_v2.png` — 260x38

Tool:

- `Tools/GenerateButtonV2Assets.ps1`

Art-source copies:

- `Assets/ArtSource/Button_V2_20260530/`
- Preview: `Assets/ArtSource/Button_V2_20260530/Button_V2_Preview.png`

Validation:

- All sixteen files match intended dimensions.
- All sixteen files have transparent corners (`alpha=0`).
- No text is baked into the images.

Design note:

- Keep Unity text live on top.
- Use native/exact-size drawing first. Avoid stretching these as large shared backgrounds.
- Good first hook targets: main menu start/secondary buttons and result retry/menu buttons.

### Audio resource gap fixed

`Tools/TestAudioResources.ps1` found that current code references these files, but they were missing:

- `Assets/Resources/Audio/BGM_Stage3.wav`
- `Assets/Resources/Audio/BGM_Stage4.wav`
- `Assets/Resources/Audio/BGM_Stage5.wav`

Fix:

- Extended `Tools/GenerateStarterAudio.ps1` with `New-BgmStage3`, `New-BgmStage4`, and `New-BgmStage5`.
- Generated the three missing WAV files with `-NoOverwrite`, so existing BGM/SE files were not replaced.
- Updated `Assets/Resources/Audio/AudioManifest.json`.
- Updated `Assets/Resources/Audio/Licenses/StarterAudio_License.txt`.

Design note:

- Stage 3: broken-core/glitch loop, low and steady.
- Stage 4: frost/ice loop, sparse and cold.
- Stage 5: storm loop, pulsed but not ascending.
- All three avoid the uncomfortable rising-tone pattern the user disliked.

### UI polish pack prepared

Added a small transparent PNG pack that can be hooked later without using stretched full-panel art:

- `Assets/Resources/Skins/Result_RankBadge_S.png`
- `Assets/Resources/Skins/Result_RankBadge_A.png`
- `Assets/Resources/Skins/Result_RankBadge_B.png`
- `Assets/Resources/Skins/Result_RankBadge_C.png`
- `Assets/Resources/Skins/Result_RankBadge_D.png`
- `Assets/Resources/Skins/Result_MvpSlot_v2.png`
- `Assets/Resources/Skins/Warning_MidBoss_Frame_v2.png`
- `Assets/Resources/Skins/Warning_FinalBoss_Frame_v2.png`
- `Assets/Resources/Skins/Warning_CoreMark_v2.png`

Tool:

- `Tools/GenerateUiPolishPack.ps1`

Art-source copies:

- `Assets/ArtSource/UI_Polish_20260529/`
- Preview: `Assets/ArtSource/UI_Polish_20260529/UI_Polish_Preview.png`

Validation:

- Rank badges: 160x160, corners alpha 0.
- MVP slot: 320x96, corners alpha 0.
- Boss warning frames: 960x220, corners alpha 0.
- Core mark: 256x128, corners alpha 0.

Design note:

- These are overlays/icons, not large baked UI screens.
- They are safe to review visually before any code hookup.
- Boss warning images intentionally contain no baked text.

### HUD panel v2 exact-size draft prepared

Added exact-size HUD panel frames based on current UI rect sizes read from `CoreLanternGame.cs`.

Generated files:

- `Assets/Resources/Skins/HUD_Panel_Wave_v2.png` — 250x74
- `Assets/Resources/Skins/HUD_Panel_HP_v2.png` — 270x104
- `Assets/Resources/Skins/HUD_Panel_Level_v2.png` — 270x76
- `Assets/Resources/Skins/HUD_Panel_ChipMini_v2.png` — 190x42
- `Assets/Resources/Skins/HUD_Panel_Data_v2.png` — 250x54
- `Assets/Resources/Skins/HUD_Panel_Loadout_v2.png` — 296x92
- `Assets/Resources/Skins/HUD_Panel_Stats_v2.png` — 286x156
- `Assets/Resources/Skins/HUD_Panel_Links_v2.png` — 360x228
- `Assets/Resources/Skins/HUD_Panel_EventLog_v2.png` — 520x70

Tool:

- `Tools/GenerateHudPanelV2Assets.ps1`

Art-source copies:

- `Assets/ArtSource/HUD_Panel_V2_20260529/`
- Preview: `Assets/ArtSource/HUD_Panel_V2_20260529/HUD_Panel_V2_Preview.png`

Validation:

- All nine files match the intended pixel dimensions.
- All nine files have transparent corners (`alpha=0`).

Design note:

- These are not currently hooked into `CoreLanternGame.cs`.
- They are intended for exact-size/native usage, not 512x512 stretching.
- If code hookup happens later, prefer a dedicated v2 flag and do not flip the old `UseGeneratedHudPanels` globally without in-Editor visual QA.

### Card part v2 exact-size draft prepared

Added small card UI parts for later readability polish. These target the current `CreateButton()` child rect sizes, but are not code-hooked yet.

Generated files:

- `Assets/Resources/Skins/Card_Header_Basic_v2.png` — 252x34
- `Assets/Resources/Skins/Card_Header_Rare_v2.png` — 252x34
- `Assets/Resources/Skins/Card_Header_Epic_v2.png` — 252x34
- `Assets/Resources/Skins/Card_TitlePlate_v2.png` — 248x36
- `Assets/Resources/Skins/Card_LevelPlate_v2.png` — 240x22
- `Assets/Resources/Skins/Card_RarityPlate_Basic_v2.png` — 178x28
- `Assets/Resources/Skins/Card_RarityPlate_Rare_v2.png` — 178x28
- `Assets/Resources/Skins/Card_RarityPlate_Epic_v2.png` — 178x28
- `Assets/Resources/Skins/Card_BottomRail_Cyan_v2.png` — 210x8
- `Assets/Resources/Skins/Card_BottomRail_Gold_v2.png` — 210x8
- `Assets/Resources/Skins/Card_SpecialCorner_Epic_v2.png` — 40x40
- `Assets/Resources/Skins/Card_SpecialCorner_Cross_v2.png` — 40x40

Tool:

- `Tools/GenerateCardPartV2Assets.ps1`

Art-source copies:

- `Assets/ArtSource/Card_Parts_V2_20260529/`
- Preview: `Assets/ArtSource/Card_Parts_V2_20260529/Card_Parts_V2_Preview.png`

Validation:

- All twelve files match intended dimensions.
- All twelve files have transparent corners (`alpha=0`).
- No UI text is baked into the images.

Design note:

- These are meant to support card readability without replacing the full card frame.
- Best first hook targets are `Card Rarity Plate`, `Card Header Plate`, and `Card Title Plate`.
- Keep Unity text on top so localization and font sizing remain controllable.

## Validation Results

### 2026-05-30 UI v2 hookup validation

Codex connected the generated Title/Result v2 assets in `Assets/Scripts/CoreLanternGame.cs` after the non-code asset pass:

- Title screen: `Title_CoreEmblem_v2`, `Title_SubtitlePlate_v2`, `Title_StatsRibbon_v2`, `Title_LogoUnderline_v2`, `Title_NavRail_v2`, `Title_CornerAccent_v2`
- Result screen: `Result_DeckFrame_v2`, `Result_PortraitFrame_v2`, `Result_StatTile_v2`, `Result_BadgeStrip_Route_v2`, `Result_BadgeStrip_Fusion_v2`, `Result_MvpRow_v2`, `Result_SummaryPlate_v2`, `Result_FooterGuide_v2`

Validation:

- `Tools/CheckCompileHealth.ps1`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- `Tools/TestImageResources.ps1`: `Errors: 0`
- `Tools/TestAudioResources.ps1`: `Errors: 0`

Manual QA still required in Unity Editor:

- Title screen: check text overlap around logo/subtitle/stats row and ensure corner accents stay behind readable text.
- Result screen: check MVP rows, stat tiles, route/fusion strips, and bottom action buttons for overlap and clickability.

### 2026-05-30 latest validation

Commands:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\Tools\TestImageResources.ps1'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\Tools\TestAudioResources.ps1'
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\Tools\CheckCompileHealth.ps1'
```

Results:

- Image resource errors: 0
- Audio resource errors: 0
- Compile health: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`

Notes:

- Image warning: optional `Player_Form4.png` falls back to procedural.
- Audio warnings: optional legacy boss fallback names only.
- Codex did not edit `Assets/Scripts/CoreLanternGame.cs` in the 2026-05-30 asset pass.

### Image resources

Command:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\Tools\TestImageResources.ps1'
```

Result:

- Skins PNG files: 540
- Code sprite references: 129
- Missing DONE backlog sprites: 0
- Errors: 0
- Warning: optional `Player_Form4.png` is absent and falls back to procedural.

### Audio resources

Command:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\Tools\TestAudioResources.ps1'
```

Result after fix:

- Manifest entries: 16
- Errors: 0
- Warnings: 2

Warnings are optional legacy boss BGM fallback names:

- `BGM_BossNullwyrm.wav`
- `BGM_BossPulswyrm.wav`

These are not blockers because the current underscore names exist.

### HUD assets

Command:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File '.\Tools\ValidateCodexAssets.ps1' -Category Hud
```

Result:

- Errors: 0
- Warnings: 11

Meaning:

- Old HUD assets still warn because they are not strict 9-slice safe.
- V2 assets pass:
  - `HUD_Bar_Back_v2.png`
  - `HUD_Bar_HP_PlayerFill_v2.png`
  - `HUD_Bar_HP_CoreFill_v2.png`
  - `HUD_Bar_EXP_Fill_v2.png`
  - `HUD_Bar_HP_Lag_v2.png`
  - `HUD_WaveProgress_Frame_v2.png`
  - `HUD_WaveProgress_Fill_Normal_v2.png`
  - `HUD_WaveProgress_Fill_Boss_v2.png`

## Claude Task Queue: Non-Image / Code-Side

Give Claude these tasks while Codex continues asset work.

1. Full playthrough regression
   - Check Title -> Stage select/setup -> Run -> Level-up card -> Evolution -> Boss -> Result -> Retry -> Main Menu.
   - Confirm no blank screen after result transitions.
   - Confirm BGM restarts correctly after returning to menu and starting again.

2. UI overlap sweep
   - Title screen: no text overflow, no hero image overlap, no stat ribbon clipping.
   - Result screen: summary text must not collide with buttons or MVP list.
   - Codex/evolution tree: if entries exceed space, use scroll instead of shrinking into unreadable text.
   - Upgrade cards: title, rarity, module level, description, and skip/reroll buttons must not overlap.

3. Stage audio routing check
   - Confirm Stage 3 uses `BGM_Stage3`.
   - Confirm Stage 4 uses `BGM_Stage4`.
   - Confirm Stage 5 uses `BGM_Stage5`.
   - If stage numbering changed internally, update the audio mapping so names and selected stage match.

4. Runtime performance hotspots
   - Re-test final boss with high-shot/high-link builds.
   - Watch bullet count, spark count, floating text count, and hit freeze triggers.
   - Keep visual impact, but clamp repeated effects before frame drops.

5. Accessibility/options cleanup
   - Do not allow mid-run difficulty changes.
   - Keep useful options: BGM/SE volume, HUD detail, screen shake, flash, hit freeze, controls display.
   - Remove or hide options that break run fairness.

6. Visible text mojibake sweep
   - Search only active string literals first.
   - Comments can stay lower priority unless they swallow code or affect documentation.

## Codex Task Queue: Asset / Image / Documentation

Keep these with Codex to avoid code conflicts.

1. HUD panel v2 assets
   - Exact-size draft prepared as `HUD_Panel_*_v2`.
   - Do not reuse old stretched panel art.
   - Prefer code-native panels plus small decorative corner/rail overlays.

2. Card readability pack
   - Exact-size draft prepared as `Card_*_v2`.
   - Start with rarity/header/title plates only, not the full card frame.
   - Keep text live in Unity and use best-fit/truncate rules.

3. Result screen polish pack
   - Small rank badge icons. Prepared as `Result_RankBadge_*`.
   - MVP slot icon/frame. Prepared as `Result_MvpSlot_v2`.
   - Victory/defeat subtle background accents.
   - No full-screen busy panel image unless preserve-aspect is guaranteed.

4. Boss warning polish pack
   - `Warning_MidBoss_Frame_v2`. Prepared.
   - `Warning_FinalBoss_Frame_v2`. Prepared.
   - `Warning_CoreMark_v2`. Prepared.
   - No text baked into images; text stays in Unity UI.

5. Store/demo art later
   - Steam capsule and itch.io cover should wait until title screen and character direction are stable.

6. Character/evolution redesign later
   - This remains final-phase work.
   - Do not generate the full 100+ character set until the user approves a small style sample.

## Visual QA Checklist Before Reporting "Done"

Use this after every visual/UI pass:

- No text is clipped at 1280x720.
- No text overlaps another text block or button.
- Important buttons remain clickable.
- Images keep their aspect ratio.
- Large images are not stretched into blurry panels.
- Background center area stays quiet enough for UI.
- Character sprites do not have unwanted center dots, white flashes, or baked backgrounds.
- HUD does not show off-map Unity sky/edge artifacts.
- Rarity/module level/card title are readable at gameplay scale.
- Japanese text uses stable labels, not mojibake.
