# Debug And Genre Gap Analysis 2026-05-22

## Debug Coverage

- Roslyn compile check passed after changes.
- Checked high-risk runtime flows in `Assets/Scripts/CoreLanternGame.cs`:
  - bootstrap / generated runtime reload
  - title menu overlays
  - result retry / main menu return
  - pause / options
  - upgrade / evolution / relic selection
  - wave completion and victory
  - audio listener recovery after generated reload
- Checked recent Unity logs for current compile/runtime errors. Existing log noise is mainly Package Manager launched with `-noUpm`, old import worker assertions, and Unity analyzer warnings. No new C# compile errors were produced.

## Fixed Now

- Prevented title `Enter` / `Space` from starting a run while Options / Mission / Evolution Tree / Codex overlays are open.
- Prevented Options from opening over relic selection.
- Added a generated-runtime reload guard so result retry / main menu / pause restart cannot launch multiple reload coroutines from repeated clicks.
- Disabled result/restart buttons after reload starts, then re-enabled result buttons when a result screen is shown again.
- Replaced Unity 6-obsolete `FindFirstObjectByType` calls with `FindAnyObjectByType`.
- Added normal upgrade `Skip` conversion:
  - Normal module choices now include `スキップ +データ X`.
  - Boss reward, relic, and evolution choices remain mandatory.
  - If the module pool ever runs dry, `緊急補給` is offered instead of soft-locking.
- Added Stage2 risk/reward data:
  - Lava edges now seed small data pickups so the hazard is not only a punishment.

## Genre Findings

Sources checked:

- Vampire Survivors Steam: minimal controls, snowballing choices, gold/meta upgrades, broad controller/touch support.
  - https://store.steampowered.com/app/1794680/Vampire_Survivors/
- Brotato Steam: fast waves, many characters/items, auto-fire plus manual aiming, shop between waves, accessibility difficulty tweaks.
  - https://store.steampowered.com/app/1942280/Brotato
- Halls of Torment Steam: quest meta progression, many abilities/items, unique bosses, multiple stages, rare item variants, class marks.
  - https://store.steampowered.com/app/2218750/Halls_of_Torment/
- 20 Minutes Till Dawn Steam: dynamic builds, rune meta progression, boss tomes, distinct characters/weapons, active aim.
  - https://store.steampowered.com/app/1966900/20_Minutes_Till_Dawn/
- PC Gamer Halls of Torment review: strong appeal comes from feeling like you are following and testing a plan.
  - https://www.pcgamer.com/games/action/halls-of-torment-review/
- Game Informer Vampire Survivors review: compelling progression and learning even on failure are major strengths.
  - https://gameinformer.com/review/vampire-survivors/single-stick-masterpiece

## What Eggcore Already Has

- Auto-fire and mouse movement.
- Short wave-based runs.
- Evolution route identity.
- Fusion/cross-evolve conditions.
- Relics and rerolls.
- Missions, codex, stage selection, danger level.
- Core defense twist that differentiates it from pure survival clones.

## System Gaps To Consider

### P0 Candidate

- Boss mechanics need unique readable patterns.
  - Current boss identity is still mostly pressure/stat based. Nullwyrm Phase2 spec exists; implement next.
- Stage identity should alter decisions, not only visuals.
  - Stage2 now has lava-edge rewards, but safe-lane readability and wave-specific reward patterns still need playtesting.
- Character goals and unlock hooks need stronger run-to-run direction.
  - Missions exist, but more "try this build" goals would make each partner feel worth replaying.

### P1 Candidate

- Build agency beyond reroll.
  - `Skip` is now implemented as a first step.
  - Later options: banish/seal, lock one card, or "favor route" currency.
- Rare variants for modules/relics.
  - Halls of Torment-style rarity variants would make repeated modules feel less flat.
- Partner-specific signature upgrades.
  - Some are present, but every partner should have at least one identifiable build payoff.
- In-run shop or between-wave exchange.
  - Brotato's shop is a major buildcraft driver. Eggcore could use a smaller "Data Lab" between key waves.

### P2 Candidate

- Accessibility sliders.
  - Brotato explicitly exposes enemy health/damage/speed tuning. Eggcore has options, but not gameplay accessibility.
- Post-run recommendation.
  - Result screen could say "次はSPEED + Siphonを試す" based on failed build.
- Enemy encyclopedia.
  - Useful once enemy roles get more distinct.
- More stage modifiers after clear.
  - Danger level exists, but modifiers could change rules instead of just pressure.

## Immediate Design Recommendation

The next highest-value system work is:

1. Implement Nullwyrm Phase2 with clear telegraphs and one named mechanic.
2. Playtest Stage2 lava-edge rewards and tune pickup value / density.
3. Add one partner-specific signature module per partner that changes behavior, not only stats.
4. Add a lightweight "Data Lab" at Wave 3/6/9 only if current choice pressure still feels too random after `Skip`.
