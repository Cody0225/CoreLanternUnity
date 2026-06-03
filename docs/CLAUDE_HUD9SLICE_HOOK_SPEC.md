# Claude Task: HUD 9-slice Candidate Hook-Up

Date: 2026-05-31
Owner split: Codex generated assets; Claude decides whether/where to hook in `CoreLanternGame.cs`.

## Goal

Replace or test selected HUD panels/bars with cleaner 9-slice PNG candidates without reintroducing the previous problems:

- noisy HUD art
- stretched bitmap proportions
- text readability loss
- UI overlap from oversized ornamentation

## Assets Generated

Location:

- `Assets/Resources/Skins/`
- source copies and preview: `Assets/ArtSource/HUD_9Slice_20260531/`

Preview:

- `Assets/ArtSource/HUD_9Slice_20260531/HUD9_Preview.png`

## Files

### Panels

Use `Image.Type.Sliced`.

Border: left=24, bottom=24, right=24, top=24

- `HUD9_Panel_Wave.png` 512x512
- `HUD9_Panel_HP.png` 512x512
- `HUD9_Panel_Level.png` 512x512
- `HUD9_Panel_ChipMini.png` 512x512

### HP / EXP Bars

Use `Image.Type.Sliced`.

Border: left=12, bottom=8, right=12, top=8

- `HUD9_Bar_Back.png` 256x64
- `HUD9_Bar_HP_PlayerFill.png` 256x64
- `HUD9_Bar_HP_CoreFill.png` 256x64
- `HUD9_Bar_EXP_Fill.png` 256x64

### Wave Progress

Use `Image.Type.Sliced`.

Border: left=16, bottom=8, right=16, top=8

- `HUD9_WaveProgress_Frame.png` 512x48
- `HUD9_WaveProgress_Fill_Normal.png` 512x48
- `HUD9_WaveProgress_Fill_Boss.png` 512x48

## Hook-Up Recommendation

Do not replace the old `HUD_Panel_*` files yet.

Suggested safe implementation:

1. Add an opt-in flag such as `UseHud9SliceCandidates = false`.
2. Load these with `LoadOptionalSprite("Skins/HUD9_Panel_Wave", null)` etc.
3. Apply only to one HUD area first, ideally the top wave progress bar.
4. Set `Image.type = Image.Type.Sliced` for every frame/fill using these assets.
5. Keep text as live Unity text above the images.
6. Keep all text containers unchanged at first; image art should support the existing layout, not force a layout change.

## Visual QA Checklist

Check in Unity Game view at 16:9 and Free Aspect:

- No label clips outside a HUD panel.
- Bars do not blur when resized horizontally.
- Transparent corners remain clean.
- Text stays readable over panel centers.
- No HUD image appears over cards, pause, result, codex, or evolution selection.
- The HUD still feels quieter than the earlier rejected image-heavy pass.

## Validation Already Done By Codex

- 11 PNG files generated.
- Expected sizes verified.
- Four-corner alpha verified as `0`.
- `.meta` files created with sprite mode and borders.
- `Tools/TestImageResources.ps1` should remain at `Errors: 0`.

## Partial Hook-Up Done By Codex (2026-06-01)

Temporary exception: the user asked Codex to take over while Claude was rate-limited.

Implemented only the safest first slice:

- `UseHud9SliceCandidates = true`
- Loaded `HUD9_WaveProgress_Frame`, `HUD9_WaveProgress_Fill_Normal`, and `HUD9_WaveProgress_Fill_Boss`
- Added `ApplySlicedSprite()` and applied these sprites only to `CreateWaveProgressPanel()` / `UpdateWaveProgress()`
- Existing V2/procedural fallback is preserved if the HUD9 sprites are missing

Still pending:

- `HUD9_Panel_Wave`
- `HUD9_Panel_HP`
- `HUD9_Panel_Level`
- `HUD9_Panel_ChipMini`
- `HUD9_Bar_Back`
- `HUD9_Bar_HP_PlayerFill`
- `HUD9_Bar_HP_CoreFill`
- `HUD9_Bar_EXP_Fill`

Before expanding the hook-up, visually check the top Wave progress bar in Unity Game view and confirm it does not blur, clip text, or become louder than the rest of the HUD.
