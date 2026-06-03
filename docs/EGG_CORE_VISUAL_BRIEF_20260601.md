# EGG / CORE Visual Brief 2026-06-01

Status: `VISUAL_BRIEF`

This is a visual planning document only. Do not hook generated files into
`Assets/Scripts/CoreLanternGame.cs` until the user approves a direction.

## Goal

The egg is the face of **Eggcore Protocol**. It should feel like the thing the
whole game is about, not just a yellow HP objective.

Current direction to preserve:

- dark cyber arena
- gold and cyan protocol rings
- cute partner contrast around a sacred central core
- readable top-down gameplay silhouette

Problems to improve:

- the current egg/core reads more like a generic glowing gem than a signature
  mascot object
- title-screen identity needs a stronger "data egg" symbol
- gameplay scale must stay readable without adding noisy detail
- map/center floor should support the egg visually instead of competing with it

## Design Pillars

1. **Sacred Data Egg**
   - golden translucent shell
   - cyan-white living data light inside
   - circuit-like cracks and soft inner glow
   - elegant and iconic rather than busy

2. **Protocol Core**
   - the egg is also a machine core
   - ring halo, socket, and pedestal show that it powers the arena
   - it should look defensible and important

3. **Readable At Game Size**
   - must read at 96px, 128px, and 192px
   - no tiny text, no dense panels, no small facial details
   - clear silhouette first, surface detail second

4. **Original IP**
   - avoid existing monster/IP motifs
   - no direct Digimon, Pokemon, or specific franchise cues
   - "cyber egg shrine" rather than "monster egg from another series"

## Candidate Directions

### A. Sacred Data Egg

Most title-friendly. A polished gold egg with soft cyan inner life, elegant
circuit cracks, and restrained halo rings.

Best use:

- title screen
- main in-game core
- result/victory emblem

Risk:

- can become too decorative if ring details are too dense

### B. Armored Core Egg

More battle-readable. A slightly heavier shell with segmented armor plates,
small shield ribs, and a stable pedestal.

Best use:

- gameplay core objective
- damaged-state variants
- boss-warning contrast

Risk:

- can lose the "egg" feeling if armor becomes too angular

### C. Hatch Protocol Egg

Most emotional. Semi-transparent shell with a small abstract life/data seed
inside, implying evolution and future partners.

Best use:

- title hero
- evolution/cross-evolution cut-in
- archive/codex motif

Risk:

- may become too soft or hard to read at 64-96px

## Required Review Outputs

Before showing the user as a candidate, prepare:

- candidate board with A/B/C side by side
- 96px / 128px / 192px shrink check
- title-scale preview mock on dark background
- transparent or chroma-key removable source if the candidate is meant to be
  used as an asset
- secretary review summary

## Secretary Review Checklist

Review before user presentation:

- Does it still read as an egg, not just a gem or crystal?
- Does it feel important enough to be the title object?
- Is the silhouette readable at 96px?
- Are rings/glow supporting the egg instead of hiding it?
- Is there any tiny detail that will turn into noise in gameplay?
- Is the art original enough for commercial use?
- Does it match the existing gold/cyan cyber floor tone?
- Is it clearly marked `VISUAL_REVIEW`, not `RUNTIME_CANDIDATE`?

If any major issue remains, do not call it final. Show it only as a direction
check or regenerate.

## Map Tie-In Direction

The map should frame the egg without visual clutter:

- central circular socket/rune under the egg
- thin gold/cyan rings around the egg
- darker floor within the central arena to improve character readability
- stage-specific accents only at the edges or hazard zones
- no dense line patterns across the center 60% of the screen

Future map pass should be separate from this egg candidate pass. The first
priority is choosing the egg/core identity.

## Claude Handoff Notes

If the user approves a candidate:

- Codex may place final PNGs under `Assets/Resources/Skins/` with versioned
  filenames such as `CoreEgg_v3.png`, `Title_CoreEgg_v3.png`, and
  `CoreEgg_Damaged_v3.png`.
- Codex must update `IMAGE_ASSET_BACKLOG.md` and `REMAINING_TASKS.md`.
- Claude owns the runtime hook-up in `Assets/Scripts/CoreLanternGame.cs`.
- Do not connect `VISUAL_REVIEW` files to runtime.
