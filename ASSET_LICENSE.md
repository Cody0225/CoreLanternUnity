# Asset License

Status: FINALIZED 2026-06-03 (owner-approved direction).

This file defines the license boundary for art, audio, logos, character designs, screenshots, and brand assets.

**The source code is open (MIT). The assets are NOT.** The MIT license in `LICENSE` covers
the source code only. All art/audio/character/brand assets are All Rights Reserved and are
included for reference and to make the game runnable — **not** for reuse.

## Policy (finalized)

- **Source code**: MIT License (see `LICENSE`).
- **Original game art (sprites, UI, logo, etc.)**: © All Rights Reserved. Not licensed for reuse.
- **Original audio**: © All Rights Reserved. Not licensed for reuse.
- **Character designs, names, game title ("Eggcore Protocol"), studio/brand identity**:
  © All Rights Reserved. Trademark/brand rights retained. Do not reuse or create derivatives.
- **Third-party assets**: listed in `THIRD_PARTY_NOTICES.md` (none beyond what is noted there).

You may read, study, fork, and run the code under MIT. You may **not** redistribute or reuse the
art/audio/brand assets, or ship a game using them. Replace the assets in `Assets/Resources/`
with your own if you build on the code.

## Asset Categories To Review

| Category | Path | Status |
|---|---|---|
| Runtime sprites | `Assets/Resources/Skins/` | Pending review |
| Runtime audio | `Assets/Resources/Audio/` | Pending review |
| Art source and previews | `Assets/ArtSource/` | Pending review |
| Marketing images | `marketing/` | Pending review |
| Documentation images | `docs/` | Pending review |

## Required Before Public OSS Submission

1. Decide which assets are included in the public repo.
2. Decide which assets are reusable by others.
3. Mark non-reusable brand assets clearly.
4. Remove or replace any asset whose origin/license cannot be verified.
5. Fill `THIRD_PARTY_NOTICES.md`.
