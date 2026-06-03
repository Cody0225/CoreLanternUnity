# Claude Title 6 Partner Lineup Hook Spec

Date: 2026-06-02  
Owner boundary: Claude owns `Assets/Scripts/CoreLanternGame.cs`. Codex prepared the PNG assets only.

## Purpose

Title screen should show the six base partners as a polished "game face" lineup after the L0 pixel-art redesign.

This is a code/UI hook-up task. Codex must not edit `CoreLanternGame.cs`.

## Prepared Runtime Assets

All six images are already placed at the existing resource paths, so normal `LoadOptionalSprite` calls can reuse them:

| Partner | Resource path | Notes |
|---|---|---|
| Cobalt Pup | `Assets/Resources/Skins/Partner_S1_L0.png` | Blue cyber wolf pup |
| Ember Drake | `Assets/Resources/Skins/Partner_S2_L0.png` | Orange cyber drake |
| Sage Hare | `Assets/Resources/Skins/Partner_S3_L0.png` | Green cyber hare |
| Hex Cat | `Assets/Resources/Skins/Partner_S4_L0.png` | Purple dark cyber cat |
| Drift Fox | `Assets/Resources/Skins/Partner_S5_L0.png` | Pink fox; no bird/beak/crest/wings |
| Iron Bear | `Assets/Resources/Skins/Partner_S6_L0.png` | Black armored bear |

All files are 512x512 transparent PNGs. The `.meta` files were intentionally not edited.

## Layout Rules

- Display all six partners on the title screen in one horizontal lineup.
- Do not stretch. Use `preserveAspect = true` or equivalent aspect-safe sizing.
- Keep visual scale consistent across all six. They were normalized with the same foot baseline (`bottom_y = 448`) and similar height.
- Do not draw extra baked shadow under the characters. The PNGs intentionally have no shadow.
- Keep the title clean: do not overlap characters with the logo, START RUN button, nav buttons, or progress text.
- If the six-character row competes with the START RUN button, place the row behind/above the button with reduced alpha/glow rather than covering UI.
- Decorative character images should have `raycastTarget = false`.

## QA Checklist Before Reporting

- 1280x720 and 1920x1080 16:9 Game view:
  - no text overlap
  - no text overflow
  - no character cropped by screen edge
  - no UI button blocked by character image
  - no side-world/Unity default skybox visible at the edges
- Free Aspect quick check:
  - characters remain aspect-safe
  - title still reads first
  - START RUN remains the clearest action

## ArtSource Review Files

Use these for placement judgement:

- `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
- `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
- `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_clean_transparent.png`

## Validation From Codex

- `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
- `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- Partner `.meta` files for S1-S6: last byte `0A`, `EndsWithLF=True`

Status: Claude title hookup waiting.
