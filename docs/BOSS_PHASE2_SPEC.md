# CoreLanternUnity Boss Phase 2 Spec

Last updated: 2026-05-22

This document defines the intended second phase for Wave 10 boss `Nullwyrm`. It is a design handoff for Claude-side implementation and does not modify `Assets/Scripts/CoreLanternGame.cs`.

## Design Goal

The final boss should feel like the run's climax without becoming unreadable. Phase 2 should make the player change behavior:

- Phase 1: dodge bullet fans and manage boss pressure.
- Phase 2: protect the core while surviving a more aggressive pattern set.

## Review-Derived Boss Targets

Review scan date: 2026-05-22.

Strong survivor-like bosses are praised when they create a memorable pattern break, not when they simply become a larger HP sponge. The main review risks for this genre are unreadable screens, unfair damage during reward/choice states, late-run performance drops, and builds that cannot answer a boss mechanic.

Phase 2 should therefore follow these rules:

- Add new behavior before adding raw damage.
- Telegraph every new attack with shape, color, and sound.
- Give each major build type a useful answer: SPEED dodges, GUARD blocks, Siphon/collection builds recover, POWER burns windows.
- Use one signature mechanic players can describe after the run. For Nullwyrm, that should be `Core Mark`.
- Avoid long invulnerability. The transition should feel dramatic but not stall the run.
- Never stack spiral, summon, and core mark at the same moment.
- Do not fire dangerous attacks during evolution, card choice, result, pause, or cut-in states.

## Trigger

Trigger Phase 2 when `Nullwyrm` drops below 50% HP for the first time.

Suggested trigger rules:

- `enemy.isBoss == true`
- `enemy.isMidBoss == false`
- `enemy.phase2Triggered == false`
- `enemy.hp <= enemy.maxHp * 0.5f`

Important implementation caveat:

- Current code has places that infer boss identity from HP thresholds. That is fragile because `Pulswyrm` HP is now high enough to be misread. Add explicit fields such as `isMidBoss` or `bossKind` before Phase 2 logic.

## Suggested Enemy Fields

- `bool phase2Triggered`
- `bool isMidBoss`
- `string bossKind`
- `float phase2TransitionTimer`
- `float specialAttackCooldown`
- `float coreMarkTimer`
- `float spiralCooldown`
- `float summonCooldown`
- `float phase2ArmorFlashTimer`
- `Color phase1Color`
- `Color phase2Color`

## Phase 1 Baseline

Current final boss identity:

- Name: `Nullwyrm`
- Role: final boss
- Core pressure: indirect
- Current attacks: bullet fan and aggressive movement
- Current movement: targets the player

Phase 2 should build on this instead of replacing it.

## Phase 2 Changes

### Stats

Recommended values:

- Scale: `1.65 -> 1.88`
- Move speed multiplier: `1.08`
- Touch damage multiplier: `1.00`
- Bullet fan count: `5 -> 6`
- Bullet spread: `26 degrees -> 34 degrees`
- Shoot cooldown: `1.05f -> 0.92f` if current active cooldown allows it
- Ideal range: `3.2f -> 3.6f`
- Transition invulnerability: `0.75f`

Damage caution:

- Do not scale every number upward at once.
- Bullet count and core-direct pressure already increase difficulty.
- Keep bullet damage roughly similar to Phase 1 unless playtesting proves it too soft.
- Prefer more readable pattern variety over higher DPS.
- If recent difficulty changes are active, start Phase 2 softer and tune upward only after low-power builds can clear.

### Visuals

Phase 2 should be unmistakable at a glance.

Recommended visual changes:

- Armor plates crack or peel outward.
- Body scale increases about 14%.
- Main color shifts from magenta/orange to magenta/cyan-white corruption.
- Boss HP bar color changes to hot magenta.
- A short radial flash expands from the boss on transition.
- Existing boss sprite can be tinted/scaled first; bespoke Phase 2 sprite can come later.

Avoid:

- Full-screen tint that hides card selection or HUD.
- Too many particles around the player.
- Long invulnerable cutscene that interrupts flow.

## Phase 2 Attack Set

### 1. Spiral Corruption

Purpose:

- Forces circular movement and teaches the player that Phase 2 is different.

Behavior:

- Telegraph for `0.7f` seconds with a rotating ring.
- Fire 14 low-damage bullets in a spiral.
- Repeat every `5.5f` seconds.
- Bullets should be slower than normal fan bullets.
- Do not use Glitch Summon during Spiral Corruption.

Suggested values:

- `spiralBulletCount = 14`
- `spiralBulletSpeed = 2.8f`
- `spiralBulletDamage = 0.45f`
- `spiralRotationStep = 18f`
- `spiralCooldown = 5.5f`
- `spiralTelegraphSeconds = 0.75f`

### 2. Core Mark

Purpose:

- Brings the Data Egg back into the final battle.
- Gives defensive/guard builds a satisfying moment.

Behavior:

- Nullwyrm marks a line from itself to the core.
- Telegraph for `1.2f` seconds.
- If the player stands in the line, damage is reduced or redirected to the player shield.
- If not blocked, the core takes moderate damage.

Suggested values:

- `coreMarkCooldown = 10f`
- `coreMarkTelegraphSeconds = 1.35f`
- `coreMarkDamageToCore = 1.8f`
- `coreMarkBlockedDamageToPlayer = 0.45f`
- `coreMarkBlockWidth = 0.65f`
- `coreMarkGuardReduction = 0.35f`
- `coreMarkLowCoreHpSkipThreshold = 0.35f`

Notes:

- This should never one-shot the core.
- Add an obvious line telegraph and warning sound.
- If UI is already busy, show only the line, not a new text panel.
- If the core is below `35%` HP, skip Core Mark and use a normal bullet fan instead.
- If the player blocks Core Mark successfully, show a short positive feedback text such as `BLOCKED`.

### 3. Glitch Summon

Purpose:

- Stops the player from tunneling only on the boss.
- Makes crowd-control builds matter in the finale.

Behavior:

- Spawn a small pack every `10f` seconds in Phase 2.
- Prefer weak enemies over high-HP bodies.
- Spawn outside the core's immediate safe radius.
- First summon should be light so players learn the phase before being crowded.

Suggested pack:

- First summon: 2 Runner
- Later summon: 2 Runner, with 1 Phantom only if enemy pressure is low

Suggested values:

- `summonCooldown = 11f`
- `summonMinRadiusFromCore = 4.5f`
- `summonMaxAliveBonus = 3`
- `summonSkipIfEnemyCountAbove = 14`
- `phantomSummonDelayAfterPhase2 = 20f`

### 4. Transition Pulse

Purpose:

- Sells the phase change with a memorable moment.

Behavior:

- At Phase 2 start, fire one expanding warning ring.
- Player can dodge outward or inward depending on ring timing.
- Damage should be low. The point is spectacle and repositioning.

Suggested values:

- `transitionPulseDelay = 0.55f`
- `transitionPulseDamage = 0.45f`
- `transitionPulseKnockback = 1.4f`
- `transitionPulseRingSpeed = 7.5f`
- `transitionNoDamageGrace = 0.65f`

### Phase 2 Mercy Rules

These guards keep the boss exciting without creating unavoidable losses:

- Do not start Core Mark if the core is below `35%` HP.
- Do not summon if total enemy count is above `14`.
- Do not use Spiral Corruption while Core Mark is telegraphing.
- Do not use any Phase 2 special while the player is in card choice, evolution choice, result, pause, or cut-in states.
- For the first `3f` after transition, allow normal movement and low-pressure bullets only.
- If the player has not evolved beyond stage 1 by Wave 10, reduce Phase 2 special cooldown pressure by `15%`.

## Pulswyrm Decision

Recommendation: do not give `Pulswyrm` a full Phase 2 yet.

Reason:

- Wave 5 is a pacing spike, not the finale.
- A full second form could make mid-run feel too long.
- The player should learn "bosses can change" without exhausting the mechanic early.

Recommended alternative:

- Add a mini-enrage at 50% HP.
- No transition cutscene.
- No new core-mark attack.

Suggested mini-enrage:

- Move speed multiplier: `1.12`
- Bullet fan count: `3 -> 4`
- One short warning pulse
- Duration: until death

## Suggested Methods

- `TryTriggerBossPhase2(Enemy enemy)`
- `TriggerBossPhase2(Enemy enemy)`
- `UpdateBossPhase2(Enemy enemy, float dt, Vector2 dirToPlayer, float distToPlayer)`
- `FireBossSpiral(Enemy enemy)`
- `BeginCoreMarkAttack(Enemy enemy)`
- `ResolveCoreMarkAttack(Enemy enemy)`
- `SpawnBossMinions(Enemy enemy, int runnerCount, int phantomCount)`
- `ApplyBossPhaseVisuals(Enemy enemy)`
- `BeginBossPhase2Cutin(Enemy enemy)`

## HUD and Feedback

Recommended:

- Boss HP bar briefly flashes.
- Show one short center text: `NULLWYRM PHASE 2`.
- Keep the text under `1.2f` seconds.
- Use sound and screen shake if options allow them.

Avoid:

- Permanent extra HUD panels.
- Covering evolution/card selection if a selection appears at the same time.
- Using the same color overlay as evolution choice screens.

## Balance Test Cases

- Can a low-speed build dodge Spiral Corruption without unavoidable damage?
- Can a guard/core-defense build meaningfully reduce Core Mark pressure?
- Does Phase 2 stay readable when many enemies are alive?
- Does the transition interrupt victory or evolution choice states?
- Does the boss still target the player after Phase 2?
- Does killing `Pulswyrm` still start the correct midboss victory flow?
- Can a weak non-meta partner survive the first 10 seconds of Phase 2?
- Does Core Mark feel like a signature moment instead of random core damage?
- Does Phase 2 add decisions without becoming a damage sponge?
- Does Phase 2 keep stable performance when summons, bullets, and player effects overlap?
