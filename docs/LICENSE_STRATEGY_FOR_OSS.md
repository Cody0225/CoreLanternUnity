# License Strategy For OSS Publication

Last updated: 2026-06-03

This project is being prepared for an OSS submission. The README now states the intended model: MIT for source code, with separate asset and trademark terms.

Do not publish as a finalized open-source release until the `LICENSE`, `ASSET_LICENSE.md`, and `THIRD_PARTY_NOTICES.md` files are completed.

## Recommended Model

For a game project, the safest practical split is:

1. **Source code**: MIT License
2. **Original game assets**: separate asset license
3. **Game name, logos, character names, and brand marks**: trademark/brand rights retained
4. **Third-party assets**: listed individually in `NOTICE` or `THIRD_PARTY_NOTICES.md`

## Selected Direction: MIT For Code, Assets Retained

Best if the goal is to show engineering and allow forks while protecting commercial identity.

Suggested files:

- `LICENSE` with MIT License for source code
- `ASSET_LICENSE.md` stating that art, audio, logos, character designs, and marketing assets are not covered by MIT unless explicitly noted
- `THIRD_PARTY_NOTICES.md` listing audio, fonts, tools, and external assets

Pros:

- Simple and familiar to OSS reviewers.
- Friendly to contributors.
- Keeps commercial game assets under control.

Cons:

- Some OSS contests may expect all bundled repo assets to be openly reusable.

## Alternative: Apache-2.0 For Code, Assets Retained

Best if patent language is desired.

Pros:

- More explicit patent grant.
- Still accepted broadly as open source.

Cons:

- Longer and less approachable than MIT for a small game prototype.

## Option C: Fully Open Project

Source code and original assets are released under permissive/open licenses.

Possible split:

- Code: MIT
- Art/audio/docs: CC BY 4.0 or CC0 where possible

Pros:

- Strongest OSS story.
- Easier for judges and contributors to understand.

Cons:

- Gives up much more control over the game's visual identity.
- Risky if the plan is commercial Steam/itch.io release using the same assets.

## Recommended Decision

Use **MIT for source code** unless the target OSS application explicitly requires open licensing for all art/audio.

Before finalizing, the owner should answer:

1. Is the submission about the source code, or the full game including assets?
2. Can other developers legally reuse the character art and logos?
3. Are AI-generated images acceptable for the target application?
4. Are placeholder BGM/SE assets replaced or documented clearly enough?

## README Wording Until Final License

Use wording like:

> The source code is planned to be released under the MIT License. Art, audio, logos, names, and character designs are planned to be managed under separate asset/trademark terms.

This is honest and avoids falsely licensing assets under MIT before the asset terms are actually selected.
