# Claude Hook Spec: A Egg Presentation Assets

Date: 2026-06-01  
Owner: Claude for code hook-up / Codex for assets and validation  
Scope: title, result, small icon placement only

## Purpose

The user selected the A egg/core direction as the current game-face motif. Codex has already replaced the runtime core sprite by updating existing resource filenames, without editing `CoreLanternGame.cs`.

This spec covers three additional A egg presentation resources that are prepared but not connected yet. Hook-up requires `CoreLanternGame.cs`, so Claude owns this step.

## Prepared Resources

| Resource path | Size | Intended use |
|---|---:|---|
| `Assets/Resources/Skins/Title_CoreEgg_ASelected_v1.png` | 768x768 | Title hero/emblem candidate |
| `Assets/Resources/Skins/Result_CoreEgg_ASelected_v1.png` | 512x512 | Result/victory core emblem candidate |
| `Assets/Resources/Skins/Icon_CoreEgg_ASelected_v1.png` | 256x256 | Small mark for codex/archive/setup or future icon slots |

Art-source preview and validation:

`Assets/ArtSource/EggCoreVisual_20260601/title_result_support_A_20260601/TitleResultCoreEgg_ASelected_v1_preview.png`

Validation result:

- all three PNGs have transparent corners (`alpha=0`)
- all three PNGs have `.meta` files
- preview includes 64px / 96px / 128px shrink checks

## Hook-Up Guidance

Use optional loading with fallback:

```csharp
var titleEgg = LoadOptionalSprite("Skins/Title_CoreEgg_ASelected_v1", null);
var resultEgg = LoadOptionalSprite("Skins/Result_CoreEgg_ASelected_v1", null);
var iconEgg = LoadOptionalSprite("Skins/Icon_CoreEgg_ASelected_v1", null);
```

Suggested placement:

- Title: use `Title_CoreEgg_ASelected_v1` as a subtle center hero mark behind or near the logo only if it does not collide with the logo, stats, or primary `START RUN` button.
- Result: use `Result_CoreEgg_ASelected_v1` as the clear/result emblem or portrait-side core mark. Keep the existing character portrait readable if both are shown.
- Icon: use `Icon_CoreEgg_ASelected_v1` only for compact places such as archive/setup progress marks where 64-96px readability is enough.

Secretary review note:

- `Title_CoreEgg_ASelected_v1` has large transparent padding and a faint circular field. Do not put title text or progress text directly on top of that circle. Use it as a separated central ornament, or keep enough vertical distance from text blocks.
- At 64px, shell and pedestal detail compress, but the gold egg silhouette and cyan core remain readable enough for icon use.

## Non-Negotiable Visual Rules

- Do not stretch these sprites. Set `preserveAspect = true` for `Image` components.
- Do not place large UI text directly over the egg.
- Do not reintroduce the old busy title layout. The user asked for a smart, reduced title screen.
- Do not remove the B/C visual-review candidates. They may be reused later.
- Do not expose the label `ArmoredCoreEgg` publicly. If candidate B is ever reused, rename it to something original such as `FortressDataEgg`.

## QA Checklist Before Reporting Done

Test in Game view:

- Free Aspect
- 16:9 / 1920x1080
- 1280x720 reference scale

Verify:

- no text clipping
- no image/text overlap
- no pointer-blocking decorative images over buttons
- title logo and `START RUN` remain the first read
- result buttons remain clickable
- egg does not look like a white dot or unreadable blob at smaller size
- title egg circle does not sit behind title/progress text in a way that reduces readability

## Status

`Claude接続待ち`. Codex intentionally did not edit `Assets/Scripts/CoreLanternGame.cs` for this pass because Claude is available again and owns code hook-up.
