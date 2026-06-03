# Digital Monster Style Plan

This is a prep note for turning Core Lantern into an original digital-monster-style roguelike.

Do not use official character names, copied artwork, ripped sprites, logos, music, or sound effects if the project will ever be shared. For private local play, keep any copyrighted replacements on this PC only.

## Theme Direction

Working title: `Eggcore Protocol`

Premise:

A small partner monster protects a data egg in a broken cyber arena. Glitched virus creatures attack in waves. Each level-up gives the partner a new evolution module.

## Visual Replacements

Current object names and planned theme:

- `Player.png`: original partner monster
- `Lantern.png`: data egg / core egg
- `Runner.png`: small glitch bug
- `Brute.png`: heavy virus beast
- `Shooter.png`: ranged virus imp

Recommended sprite style:

- transparent PNG
- square canvas, ideally 512 x 512
- character centered
- thick readable silhouette
- no official logos or text in the image
- bright edge highlights so it reads on the dark arena

## Original Character Ideas

Player options:

- `Bitmew`: small blue fox/cat-like cyber partner with antenna ears
- `Cobaltpup`: compact dinosaur-puppy partner with cyan claws
- `Sparkkit`: yellow electric lizard partner with data fins

Enemy options:

- `Glitchbit`: tiny red corrupted data bug
- `Crashhorn`: bulky purple virus beetle
- `Pinghex`: yellow floating ranged imp that shoots hex bolts

Boss option:

- `Nullwyrm`: large black-and-violet virus dragon with broken data wings

## Gameplay Changes To Add Tomorrow

High-impact tasks:

1. Add a title screen.
2. Add a wave 10 boss.
3. Add hit, pickup, upgrade, and game-over sounds.
4. Add more upgrade cards with evolution-style names.
5. Improve sprite scaling so imported character images fit cleanly.
6. Add a simple boss HP bar.
7. Rename UI from lantern fantasy terms to digital monster terms.

Suggested UI rename:

- `コア・ランタン` -> `コアテイマー`
- `ランタン HP` -> `データエッグ HP`
- `ランタンの加護を選ぶ` -> `進化モジュールを選ぶ`
- `経験値` -> `データ`
- `ウェーブ` -> `侵食`

## Prompt Ideas For Original Sprite Generation

Use these only for original characters, not official characters.

Player prompt:

`transparent background sprite of an original small cobalt cyber monster partner, cute brave dinosaur puppy silhouette, cyan claws, digital ear fins, thick outline, readable top-down roguelike game sprite, no text, no logo, not an existing franchise character`

Runner prompt:

`transparent background sprite of an original tiny red glitch data bug monster, angular corrupted pixels, simple readable enemy silhouette, thick outline, top-down roguelike game sprite, no text, no logo`

Brute prompt:

`transparent background sprite of an original bulky purple virus beetle monster, heavy armor shell, glowing magenta cracks, thick outline, readable game enemy sprite, no text, no logo`

Shooter prompt:

`transparent background sprite of an original yellow floating virus imp monster, hexagonal core, small horns, shoots energy, thick outline, readable game enemy sprite, no text, no logo`

Boss prompt:

`transparent background sprite of an original black and violet virus dragon boss monster, broken data wings, glowing cyan and magenta cracks, imposing silhouette, thick outline, readable top-down roguelike boss sprite, no text, no logo`

## Next Session Checklist

1. Stop Unity Play mode.
2. Generate or import original PNGs into `Assets/Resources/Skins`.
3. Set each imported PNG to `Sprite (2D and UI)`.
4. Patch `CoreLanternGame.cs` for boss wave and title screen.
5. Test in Unity.
6. Balance enemy HP, player speed, and upgrade strength.
