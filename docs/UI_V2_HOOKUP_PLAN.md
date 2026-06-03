# UI V2 Hookup Plan — 2026-05-30

Purpose: connect Codex-generated UI v2 assets without repeating the old stretched-image problem.

Scope: guidance only. This document does not require code changes by itself.

## Core Rules

- Keep gameplay text as live Unity `Text`; do not bake labels into PNGs.
- Prefer exact-size placement. If scaling is needed, scale uniformly only.
- Set `Image.preserveAspect = true` for decorative non-fill images.
- Put decorative images behind text/buttons by sibling order.
- Do not replace every panel globally in one pass. Hook one screen, test, then continue.

## Recommended Hook Order

1. Main menu title polish
   - Use `Title_CoreEmblem_v2.png` behind the title/start cluster.
   - Use `Title_StatsRibbon_v2.png` behind the main menu progress row.
   - Use `Title_NavRail_v2.png` behind secondary menu buttons.
   - Use `Button_Primary_Start_v2.png` for START RUN.
   - Use `Button_MenuSecondary_v2.png` for MISSION / evolution tree / combo / codex / options.

2. Result screen cleanup
   - Use `Button_ResultRetry_v2.png` and `Button_ResultMenu_v2.png` for bottom actions.
   - Use `Result_DeckFrame_v2.png` only at the current `960x580` deck size.
   - Use `Result_StatTile_v2.png` for run stats (`420x56` wide candidate; keep aspect ratio or slice, do not non-uniformly stretch).
   - Use `Result_MvpRow_v2.png` for the current `230x68` MVP rows.
   - DONE: `Result_RankMedal_*.png` is connected as a subtle background medal inside the RANK stat tile, keeping the result screen compact.
   - Use `Result_BadgeStrip_Route_v2.png` and `Result_BadgeStrip_Fusion_v2.png` for route/cross strips.
   - Keep long module/build text constrained or clipped; do not force all text into one line.

3. Upgrade card readability
   - DONE: `Card_Header_*_v2.png`, `Card_TitlePlate_v2.png`, and `Card_LevelPlate_v2.png` are connected as exact-size internal parts.
   - DONE: `Card_RarityPlate_*_v2.png`, `Card_BottomRail_*_v2.png`, and `Card_SpecialCorner_*_v2.png` are connected with rarity/special-state selection.
   - Keep rarity visible, but secondary to module name and effect.

4. HUD panels
   - Use `HUD_Panel_*_v2.png` only where the current rect size matches the asset.
   - Do not enable old 512x512 generated panels globally unless visual QA passes.

## Main Menu Visual QA Checklist

- Title text does not touch screen edges or decorative rails.
- Stats row does not collide with stage/danger text.
- START RUN is visually dominant but not so bright that secondary buttons disappear.
- Secondary buttons have equal spacing.
- No decorative corner/rail sits on top of readable text.
- At 16:9 and Free Aspect, no image is stretched wider or taller than intended.

## Result Screen Visual QA Checklist

- Retry and Main Menu buttons are always visible.
- MVP list does not overlap run stats.
- Long module list wraps or truncates cleanly.
- Rank badge is readable at a glance.
- Background gameplay is dimmed enough that result text is readable.

## Claude/Codex Coordination Notes

- If Claude is actively editing `Assets/Scripts/CoreLanternGame.cs`, Codex should avoid code hookup and continue asset/doc work.
- When code hookup begins, edit with patch-style changes and run the project `CLAUDE.md` verification flow.
- Report at minimum: visual target, files hooked, odd-quote count, brace diff, swallowed-code scan, compile/Roslyn result.
