# OSS Application Readiness

Last updated: 2026-06-03

This document tracks what is ready, what is risky, and what must be finished before submitting Eggcore Protocol to an OSS-focused event, award, or public GitHub showcase.

## Current Repository Snapshot

Measured locally on 2026-06-03:

| Item | Value |
|---|---:|
| Total files, including Unity generated folders | 6,810 |
| Publish-candidate files, excluding `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `build/` | 3,087 |
| Git tracked files | 1,852 |
| Git commits | 12 |
| Modified tracked files | 48 |
| Untracked files | 1,235 |
| Main implementation file | `Assets/Scripts/CoreLanternGame.cs` |
| Main implementation size | About 16,031 lines |

## Application Positioning

Recommended one-line pitch:

> Eggcore Protocol is a Unity survivor-roguelite prototype that combines automatic combat, central-core defense, partner evolution, cross-evolution builds, and stage-specific hazards in a fully script-generated 2D game.

Recommended technical angle:

- A compact Unity prototype architecture where the game can boot from generated runtime objects.
- A clear example of survivor-style combat mixed with defense objectives.
- Documented AI-assisted asset iteration, validation scripts, and postmortems.
- A game-design sandbox for evolution routes, partner identities, cross-evolution unlocks, and build synergy.

## Strengths For OSS Review

- The core gameplay loop is understandable and playable: choose partner, survive waves, collect upgrades, evolve, fight bosses, see result.
- The project includes design specs, release planning, asset maps, visual style notes, audio notes, and incident postmortems.
- The optional `Resources/Skins` override pattern makes it possible to replace art without touching game logic.
- Tooling exists for image/audio validation and compile-health checks.
- The project has a clear development story: solo indie prototype, heavy iteration, documented failures, and recovery practices.

## Current Risks

| Risk | Severity | Why it matters |
|---|---|---|
| License not finalized | Blocker | README now states MIT is planned for source code, but final `LICENSE` and asset terms still need owner approval. |
| Huge single C# file | High | External contributors will find 16k lines hard to review and modify. |
| Dirty Git working tree | High | 48 modified tracked files and 1,235 untracked files make the public state hard to trust. |
| Asset licensing ambiguity | High | AI-generated images, placeholder audio, and generated art need a clear permission record. |
| No CI | Medium | Reviewers cannot quickly verify compile/resource health. |
| README lacks visual proof | Medium | OSS judges and GitHub visitors need screenshots/GIFs immediately. |
| No public issue templates until now | Medium | Makes contribution flow unclear. |

## Minimum Before Applying

These are the minimum tasks before an OSS application:

1. Confirm MIT License for source code.
2. Add final `LICENSE` and complete asset-specific license/notice files.
3. Clean Git status: decide which untracked files belong in the repo, which are generated source archives, and which should be ignored.
4. Add screenshots/GIFs to README.
5. Add a short architecture section explaining the single-file prototype decision.
6. Add validation commands and expected output.
7. Run Unity once and confirm no console errors.
8. Create a clean commit tagged as the application snapshot.

## Recommended Application Score After Cleanup

Current estimated status:

- Game prototype: 8 / 10
- OSS repository readiness: 4 / 10
- Expected readiness after the minimum tasks above: 7 / 10

The main improvement lever is not more features. It is trust: license clarity, clean repo state, verification steps, and visual proof.
