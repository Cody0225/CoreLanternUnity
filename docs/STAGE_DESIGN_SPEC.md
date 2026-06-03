# CoreLanternUnity Stage Design Spec

Last updated: 2026-05-22

This document defines Stage 2 and Stage 3 for Claude-side implementation. It intentionally does not require changes from Codex in `Assets/Scripts/CoreLanternGame.cs`.

## Scope

- Stage 1 remains the normal arena baseline.
- Stage 2 adds a readable lava hazard layer and heat/visibility pressure.
- Stage 3 should feel like a different story chapter, not only a palette swap.
- Stage implementation should be deterministic enough for balancing, with small runtime variation for replay value.

## Review-Derived Design Targets

Review scan date: 2026-05-22.

The strongest survivor-like reviews consistently praise simple controls, fast power growth, clear risk/reward moments, and stage mechanics that give the player something meaningful to do besides only waiting out a timer. Common negative points are visual clutter, late-run performance drops, mandatory chores, unclear build value, and bosses or hazards that feel like stat checks rather than readable challenges.

Stage design should therefore follow these rules:

- Each stage adds one extra verb, not three. Stage 2 asks the player to route around danger. Stage 3 asks the player to decide whether to protect optional relays.
- Every stage hazard must have a reward hook nearby. Danger without upside feels punitive in this genre.
- Objectives should be optional advantages unless the stage is explicitly marketed as an objective mode.
- Hazards must be readable from shape and color before they deal damage.
- Avoid permanent full-screen overlays. Use local effects, edge haze, and one compact HUD line.
- Support multiple builds: SPEED reaches rewards faster, GUARD survives or blocks pressure, Siphon/collection builds profit from risky pickups.
- Keep the "one more run" loop fast. New mechanics should increase decisions, not menu or HUD time.

## Common Stage Architecture

Add stage behavior as a thin layer around the existing wave/enemy/boss systems rather than rewriting the arena loop.

Suggested fields:

- `int currentStageId`
- `string stageName`
- `string stageSubtitle`
- `float stageVisionMultiplier`
- `float stageHazardDamagePerSecond`
- `List<GameObject> stageObjects`
- `List<StageHazardZone> stageHazards`
- `List<StageDevice> stageDevices`
- `System.Random stageRandom`

Suggested methods:

- `ApplyStageConfig(int stageId)`
- `BuildStageEnvironment()`
- `ClearStageObjects()`
- `UpdateStageHazards(float dt)`
- `UpdateStageDevices(float dt)`
- `GetStageWaveTrait(int waveNumber)`
- `GetStageEnemySpawnWeights(int waveNumber)`
- `GetStageWaveHint(int waveNumber)`
- `ApplyStageBossModifiers(Enemy boss, bool isMidBoss)`

Main integration points:

- `BeginWave(...)`: apply stage-specific wave trait/hint and boss modifiers.
- `SpawnEnemy(...)`: alter spawn weights and spawn safety rules per stage.
- `UpdateEnemies(...)`: allow some enemies to avoid or exploit stage hazards.
- `UpdatePlayer(...)` or equivalent damage pass: apply hazard damage to the player only after a short grace time.
- `UpdateUi(...)`: show one compact stage hint, not a large permanent panel.
- Background/floor construction: use stage-specific floor sprites and props only when ratio-safe rendering is confirmed.

## Stage 2: Lava Area

Working name: `Lava Cache`

Theme:

- A corrupted thermal storage sector around the Data Egg.
- The arena is still circular, but the safe route changes because lava pools and heat vents split the floor.
- The player should read danger by color and shape immediately: orange glow = avoid unless the build can tolerate it.

### Hazard Layout

Recommendation: fixed anchor positions with light deterministic variation.

Reason:

- Fully random lava can create unfair damage and unreadable pathing.
- Fixed anchors let players learn the stage.
- Small seed variation keeps runs from feeling identical.

Default layout:

- 5 lava pools on a ring between radius `3.6` and `9.6`.
- 2 heat vents near the outer ring between radius `8.0` and `11.4`.
- Never place hazards inside radius `2.0` from the core.
- Never place hazards inside radius `1.6` from the player spawn.
- Keep at least `1.3` units of safe lane between adjacent hazard zones.

Suggested numeric values:

- `lavaPoolRadiusMin = 0.85f`
- `lavaPoolRadiusMax = 1.35f`
- `lavaPoolCountBase = 5`
- `heatVentCountBase = 2`
- `lavaDamagePerSecond = 0.24f`
- `lavaGraceSeconds = 0.55f`
- `heatVentPulseInterval = 14f`
- `heatVentPulseDuration = 3f`
- `heatVentTelegraphSeconds = 0.85f`
- `heatVentVisionMultiplier = 0.76f`
- `defaultStageVisionMultiplier = 0.9f`
- `minCombinedVisionMultiplier = 0.68f`
- `safeLaneMinWidth = 1.6f`
- `hazardRewardDataMultiplier = 1.25f`
- `hazardRewardPickupChance = 0.18f`

Risk/reward rule:

- Put small data-chip clusters near lava edges, not in the center of lava.
- Heat vents can briefly reveal a bonus pickup after the pulse ends.
- Never make hazard rewards required for progression.
- If a lava zone blocks a route, place at least one visible alternate lane.

### Floor and Visual Elements

Use these assets only with aspect-ratio preservation:

- `Floor_Hazard_Lava`
- `Floor_DarkBase_A` or current active floor base
- `Arena_Boundary`
- small glow particles around vents

Visual rules:

- Lava zones should use clean silhouettes: round pools or broken plate shapes, not noisy texture overlays.
- The core area remains visually clean.
- Avoid full-screen orange tint. Use local glow and short warning rings.
- If the generated hazard image is too detailed at runtime scale, use it as a masked accent under a simpler procedural shape.

### Player Damage

Hazard damage should pressure movement but not delete runs.

Rules:

- Damage only the player, not the core.
- Core damage only occurs through enemy behavior or boss attacks.
- Apply damage after `lavaGraceSeconds` continuous contact.
- Flash the player outline and play a small heat tick effect at most 4 times per second.
- Do not stack multiple overlapping lava zones beyond `1.25x` damage.
- If player HP is `<= 35%`, lava damage ticks no faster than once every `0.55f` seconds.
- Lava damage never triggers during evolution, card choice, result, pause, or boss transition cut-ins.

### Enemy Behavior

Enemy stage behavior should create different wave texture without requiring new enemy classes.

Suggested per-type behavior:

- Runner: ignores lava and becomes more threatening through path pressure.
- Brute: avoids lava if a simple side route exists; otherwise crosses slowly.
- Shooter: tries to keep lava between itself and the player.
- Dasher: can cross lava freely and leaves a faint heat trail.
- Bomber: prefers lava-adjacent routes, making the hazard area feel hostile.
- Phantom: unaffected visually, but should not become invisible inside heat smoke.

Suggested fields:

- `bool ignoresStageHazards`
- `bool avoidsStageHazards`
- `float stageHazardSpeedMultiplier`
- `float stageHazardDamageMultiplier`

### Wave Traits

Stage 2 can reuse existing wave traits but rename some hints for flavor.

Suggested differences:

- Wave 2: `Berserk` becomes `Overheat`
- Wave 4: `Dark Field` becomes `Smoke Field`
- Wave 7: `Elite Swarm` becomes `Magma Swarm`
- Wave 10: `Boss` becomes `Nullwyrm: Thermal Break`

### Boss Changes

Clear condition:

- Same as Stage 1: clear Wave 10 by defeating the boss.

Pulswyrm:

- No full second form needed.
- At 50% HP, trigger one heat pulse and briefly increase movement speed.

Nullwyrm:

- If Phase 2 is implemented, Stage 2 Phase 2 should add lava vents during the transition.
- If Phase 2 is not ready, add a simple `Thermal Surge` every 8 seconds below 50% HP.

Suggested Phase 2 stage values:

- `bossThermalSurgeCooldown = 8f`
- `bossVentWarningSeconds = 0.75f`
- `bossVentDuration = 2.2f`
- `bossVentDamagePerSecond = 0.28f`

## Stage 3 Candidate Ideas

Stage 3 should introduce a new story feeling. The best direction is a mechanic that supports the core-defense identity instead of only making movement harder.

### Candidate A: Magnetic Ruins

Concept:

- The arena is a broken magnetic archive.
- Periodic polarity waves push or pull enemies, pickups, and some bullets.

Pros:

- Strong build synergy with pickup range, projectile count, and movement builds.
- Visually clear with cyan/magenta field rings.
- Can make the same enemy roster feel different.

Cons:

- Requires careful physics-like tuning to avoid nausea or unfair bullet hits.
- Can break pathing if implemented as raw position movement.

Implementation notes:

- Use velocity offsets, not instant position snaps.
- Limit player displacement to a mild nudge.
- Put polarity state in the HUD as a small icon only.

Suggested values:

- `polarityInterval = 12f`
- `polarityPulseDuration = 3.5f`
- `pickupPullMultiplier = 1.8f`
- `enemyPushForce = 0.55f`
- `playerNudgeForce = 0.22f`

### Candidate B: Time Fracture

Concept:

- Time distortion zones appear around broken clock-like devices.
- Enemies slow down in some zones and speed up in others.

Pros:

- High skill ceiling.
- Easy to create dramatic visual effects.
- Pairs well with boss warning telegraphs.

Cons:

- Can make the game feel inconsistent if projectile/update timing is not isolated.
- Needs very readable color language.

Implementation notes:

- Do not change global `Time.timeScale`.
- Apply multipliers to enemy speed, bullet speed, and cooldowns locally.

Suggested values:

- `timeZoneCount = 3`
- `timeZoneRadius = 2.2f`
- `slowZoneMultiplier = 0.72f`
- `hasteZoneMultiplier = 1.18f`
- `timeZoneRelocateInterval = 20f`

### Candidate C: Broken Core Network

Concept:

- The Data Egg is connected to damaged relay devices around the arena.
- The player can stabilize relays for temporary benefits while enemies try to corrupt them.

Pros:

- Most distinct from Stage 1 and Stage 2.
- Reinforces the game's identity: protecting the core, not only surviving.
- Gives defensive builds and link roles more value.

Cons:

- Requires extra UI and object state.
- Needs careful tuning so it does not become busywork.

Implementation notes:

- Devices should be optional advantages, not mandatory chores.
- One compact objective line is enough: `Relay 1/3 stable`.
- Ignoring all relays must still allow a normal Stage 3 clear.
- Relays should create a comeback route, not a failure spiral.

Suggested values:

- `relayCount = 3`
- `relayHp = 18f`
- `relayStabilizeSeconds = 3.2f`
- `relayCorruptDamagePerSecond = 0.75f`
- `relayRewardCoreShield = 4f`
- `relayRewardDataChips = 8`
- `relayWaveSpawnBonus = 0.0f` until balance is proven
- `relayRewardRerollDiscount = 5`
- `relayChoiceRarityBonus = 0.12f`
- `relayEnemyPressureCap = 6`

## Recommended Stage 3

Choose Candidate C: `Broken Core Network`.

Reason:

- It feels like a story chapter instead of just another hazard floor.
- It gives the core-defense premise more bite.
- It creates natural build choices: mobility can stabilize relays quickly, guard can hold a relay, siphon builds can profit from stable lanes.

Secondary flavor:

- Add a light time-fracture pulse around unstable relays only.
- Avoid full magnetic physics in the first pass.

Recommended Stage 3 fields:

- `int stableRelayCount`
- `float nextRelayPulseTime`
- `float relayPulseDuration`
- `bool relayRewardClaimedThisWave`
- `float relayObjectiveHintTimer`
- `int corruptedRelayPressureCount`

Recommended Stage 3 methods:

- `SpawnStageRelays()`
- `UpdateRelayState(float dt)`
- `DamageRelay(StageDevice relay, float amount)`
- `StabilizeRelay(StageDevice relay, float dt)`
- `ApplyRelayReward(StageDevice relay)`
- `GetNearestRelay(Vector2 position)`
- `GetRelayBuildInteractionBonus(string linkName)`

Build interaction targets:

- SPEED route: `relayStabilizeSeconds * 0.85f` while moving near a relay.
- GUARD route: relays take `15%` less corruption damage while the player is nearby.
- Siphon link: data-chip reward from stabilized relays increases by `25%`.
- Bulwark link: first relay stabilized each wave grants `+1` temporary core shield.
- Phase link: once per wave, crossing an unstable relay pulse does not damage the player.

Reward ladder:

- 1 relay stable: immediate data burst.
- 2 relays stable: small core shield pulse.
- 3 relays stable: next card choice gets a minor rarity weight boost or reroll discount.

Failure rule:

- A corrupted relay creates pressure but does not end the run.
- Relay enemies should stop spawning when the stage enemy count is already high.

## Claude Implementation Checklist

- Add stage state without changing existing enum-like route/build structures.
- Keep Stage 1 behavior unchanged.
- Start with Stage 2 hazard visuals disabled behind a flag if needed.
- Implement Stage 2 damage and enemy behavior before adding boss-specific Stage 2 attacks.
- Implement Stage 3 relay devices without adding new permanent HUD blocks.
- Use local effects and compact text; avoid bringing back visual clutter.
- Verify wide 16:9 and narrow Game view layouts before enabling new large assets.

## Review-Informed Acceptance Tests

Stage 2:

- A low-speed build can always see a safe lane around lava.
- A low-HP build can touch lava briefly, react, and recover without losing the run instantly.
- Lava rewards are tempting but never mandatory.
- Heat/smoke effects do not hide player HP, core HP, enemy danger circles, or card choices.
- Enemy count plus hazard particles does not cause late-wave frame drops.

Stage 3:

- Ignoring relays is viable but less rewarding.
- Stabilizing one relay feels immediately useful within the same wave.
- Relay UI fits in one compact objective line.
- Defensive builds get a clear reason to stand ground.
- Collection/Siphon builds get a clear reason to route through relay rewards.
- No relay mechanic interrupts evolution, cross evolution, result, pause, or boss cut-in states.

## Open Balance Questions

- Should Stage 2 unlock after one Stage 1 clear, or rotate randomly after Wave 10 clears?
- Should Stage 3 be selectable from the main menu, or appear only after mission progression?
- Should stage-specific rewards exist, or should stages only alter risk and flavor?
- Should Stage 3 relays influence evolution/fusion unlocks later?
