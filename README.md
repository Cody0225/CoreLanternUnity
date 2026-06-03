# Eggcore Protocol

A defense-roguelite-survivor fusion where you protect a central core through 10 waves of escalating chaos.

**Developer**: Embercore Studio (Solo indie)
**Engine**: Unity 6000.4
**Platform**: Windows 64-bit (Steam EA + itch.io demo)
**Target Release**: Steam Early Access 2026-08 → Full Release 2027-02

> **License**: Source code is released under the **MIT License** (see [`LICENSE`](LICENSE)). Art, audio, logos, names, and character designs are **© All Rights Reserved** and are not licensed for reuse (see [`ASSET_LICENSE.md`](ASSET_LICENSE.md)).

---

## 🎮 What is Eggcore Protocol?

Eggcore Protocol blends the **survivor-roguelite replayability** of Vampire Survivors and Brotato with the **defense tension** of tower defense games.

You play as an autonomous-firing partner character that must protect a central "Data Egg" core while pushing back waves of enemies. Each run takes 15-20 minutes and combines:

- **Movement strategy**: Move around the core, but don't stray too far
- **Build construction**: Roguelite module/relic combos with Reroll/Lock/Banish
- **Evolution**: 11 partners × 3 routes × 4 levels + 6 cross-evolutions
- **Boss mastery**: Read patterns, exploit Phase 2 windows

## 💡 What Makes It Different

Most survivor-likes focus on keeping the player alive. Eggcore Protocol adds a second pressure point: the central **Data Egg** must survive too.

The core identity is:

- **Core defense + survivor movement**: You must kite enemies, collect data, and stay close enough to protect the Eggcore.
- **Partner evolution as build expression**: A run is not only stats; your partner changes through SPEED / POWER / GUARD routes and cross-evolutions, making build identity visible instead of purely numerical.
- **Two-layer failure pressure**: Losing player HP and losing Eggcore HP are separate threats, so greedy collection routes can punish the core even when the player survives.
- **Cross-evolution goals**: Ally links and route conditions create mid-run build targets that feel closer to creature evolution than a normal upgrade list.
- **Readable prototype architecture**: The game boots from runtime-generated objects, with optional PNG/WAV overrides through `Resources`.
- **Documented iteration process**: Visual direction, asset validation, audio notes, stage specs, postmortems, and release plans are kept in `docs/`.

---

## 🚀 How to Play

### Quick Start
1. Open this folder with **Unity Hub** (Unity 6000.4.7f1 or later)
2. Open the default scene and press **Play**
3. Select a partner from the partner selection screen
4. Survive 10 waves while protecting the Eggcore

### Developer Validation

Before opening a pull request or preparing a public build, run the available validation scripts from the project root:

```powershell
.\Tools\CheckCompileHealth.ps1 -Quick
.\Tools\TestImageResources.ps1
.\Tools\TestAudioResources.ps1
```

Expected result: no compile-health errors, no missing required image resources, and no required audio errors.

### Build

Recommended local build flow:

1. Open the project in **Unity 6000.4.7f1 or later**.
2. Open **File > Build Profiles** or **File > Build Settings**.
3. Select **Windows** as the target platform.
4. Use the default scene included in the project.
5. Build to a local folder outside tracked source files, for example `Builds/Windows/`.
6. Run the generated executable and complete a smoke test: title screen, partner select, one wave, pause menu, result path.

For release packaging notes, see [`docs/STEAM_BUILD_GUIDE.md`](docs/STEAM_BUILD_GUIDE.md) and [`docs/ITCH_IO_DEMO_RELEASE_GUIDE.md`](docs/ITCH_IO_DEMO_RELEASE_GUIDE.md).

### Controls
| Action | Key |
|---|---|
| Move | WASD or Arrow keys |
| Pause / Menu | ESC or P |
| Options | O |
| Module Lock (during selection) | Q / W / E |
| Module Banish (during selection) | Z / X / C |
| Retry with same setup (result screen) | R |
| Return to main menu (result screen) | M |

### Build Philosophy
- **SPEED**: Mobility + bullet count + chain hops
- **POWER**: Single-shot damage + boss damage + critical
- **GUARD**: Core HP + orbit shield + reflect + knockback
- **Cross Evolution**: Combine route + ally link to unlock fusion forms

---

## ✨ Features

- 11 unique partner characters
- 3 evolution routes (SPEED / POWER / GUARD) × 4 stages each
- 6 cross-evolutions (Nova Aegis, Photon Siphon, Core Bastion, Nova Phantom, Aegis Drift, Photon Wraith)
- 5 stages with unique enemies + hazards (Lava / Corruption / Frost / Storm)
- Boss Phase 2 + mid-boss Enrage
- 60+ upgrade modules + 15+ relics
- Brotato-style Reroll / Lock / Banish card management
- Danger Level 0-5 difficulty scaling
- Kill streak tracking + death cause analysis
- Same-settings rematch button for fast iteration

---

## 🛠 Project Structure

```
CoreLanternUnity/
├── Assets/
│   ├── Scripts/
│   │   └── CoreLanternGame.cs        # Main game (16k+ lines, single file)
│   ├── Resources/
│   │   ├── Skins/                    # Optional sprite overrides (PNG)
│   │   └── Audio/                    # BGM and SFX (WAV)
│   └── ArtSource/                    # Dev-only art source (excluded from the public repo)
├── Tools/                            # Asset generation + validation scripts
│   ├── GenerateStarterAudio.ps1
│   ├── CheckCompileHealth.ps1
│   ├── TestImageResources.ps1
│   ├── TestAudioResources.ps1
│   └── ...
├── docs/                             # Full documentation (see below)
├── CLAUDE.md                         # Project-specific Claude rules
├── CONTRIBUTING.md                   # Community contribution guide
└── README.md                         # (this file)
```

---

## 📚 Documentation Index

### For Players
- [`CONTRIBUTING.md`](CONTRIBUTING.md) — How to report bugs / request features / contribute translations
- (Steam page) — Coming 2026-08

### For Developers
- [`CLAUDE.md`](CLAUDE.md) — **Hard rules** for working with CoreLanternGame.cs (must read before editing)
- [`REMAINING_TASKS.md`](REMAINING_TASKS.md) — Running task list with completion history
- [`docs/OSS_APPLICATION_READINESS.md`](docs/OSS_APPLICATION_READINESS.md) — OSS submission readiness snapshot
- [`docs/OSS_APPLICATION_DRAFT_JA.md`](docs/OSS_APPLICATION_DRAFT_JA.md) — Japanese OSS application draft
- [`docs/LICENSE_STRATEGY_FOR_OSS.md`](docs/LICENSE_STRATEGY_FOR_OSS.md) — Recommended public license strategy

### Release Planning
- [`docs/RELEASE_CHECKLIST.md`](docs/RELEASE_CHECKLIST.md) — Full release prep checklist
- [`docs/EA_TO_1_0_ROADMAP.md`](docs/EA_TO_1_0_ROADMAP.md) — EA → 1.0 6-month plan
- [`docs/ITCH_IO_DEMO_RELEASE_GUIDE.md`](docs/ITCH_IO_DEMO_RELEASE_GUIDE.md) — itch.io demo publishing
- [`docs/STEAM_BUILD_GUIDE.md`](docs/STEAM_BUILD_GUIDE.md) — Steam build + Steamworks integration

### Content / Assets
- [`docs/CODEX_RELEASE_ORDER.md`](docs/CODEX_RELEASE_ORDER.md) — Codex asset generation orders
- [`docs/VISUAL_STYLE_GUIDE.md`](docs/VISUAL_STYLE_GUIDE.md) — Brand colors, fonts, character art style
- [`docs/ASSET_HOOKUP_MAP.md`](docs/ASSET_HOOKUP_MAP.md) — Where each asset is loaded in code
- [`docs/CHARACTER_IMAGE_PROMPTS.md`](docs/CHARACTER_IMAGE_PROMPTS.md) — Character art generation prompts
- [`docs/AUDIO_LICENSE_LOG_TEMPLATE.md`](docs/AUDIO_LICENSE_LOG_TEMPLATE.md) — Audio license tracking

### Marketing / Operations
- [`docs/STEAM_STORE_COPY.md`](docs/STEAM_STORE_COPY.md) — Steam store page copy (Japanese)
- [`docs/STEAM_STORE_COPY_EN.md`](docs/STEAM_STORE_COPY_EN.md) — Steam store page copy (English draft)
- [`docs/PRESS_KIT_TEMPLATE.md`](docs/PRESS_KIT_TEMPLATE.md) — Press kit template
- [`docs/MARKETING_QUOTES.md`](docs/MARKETING_QUOTES.md) — Player quote repository
- [`docs/POST_RELEASE_OPS_PLAYBOOK.md`](docs/POST_RELEASE_OPS_PLAYBOOK.md) — Post-release ops playbook
- [`docs/KPI_TRACKING.md`](docs/KPI_TRACKING.md) — KPI tracking template
- [`docs/BACKLOG_FROM_DEMO_FEEDBACK.md`](docs/BACKLOG_FROM_DEMO_FEEDBACK.md) — Feedback backlog

### Game Design
- [`docs/STAGE_DESIGN_SPEC.md`](docs/STAGE_DESIGN_SPEC.md) — Stage specifications
- [`docs/BOSS_PHASE2_SPEC.md`](docs/BOSS_PHASE2_SPEC.md) — Boss Phase 2 mechanics
- [`docs/CHARACTER_REDESIGN_SPEC.md`](docs/CHARACTER_REDESIGN_SPEC.md) — Character redesign notes

### Legal
- [`docs/LEGAL_TEMPLATES.md`](docs/LEGAL_TEMPLATES.md) — Terms of Service / Privacy Policy / EULA drafts
- [`ASSET_LICENSE.md`](ASSET_LICENSE.md) — Pending asset-license boundary
- [`THIRD_PARTY_NOTICES.md`](THIRD_PARTY_NOTICES.md) — Third-party notice inventory template

### Postmortem
- [`docs/POSTMORTEM_20260528_MOJIBAKE.md`](docs/POSTMORTEM_20260528_MOJIBAKE.md) — Major encoding incident retrospective

---

## 🎨 Optional Local Character Skins

You can replace the simple shapes with your own local sprites without touching code.

Put PNG files in `Assets/Resources/Skins/` with names like:
- `Player.png` / `Player_Form1.png` etc.
- `Lantern.png` (core)
- `Runner.png` / `Brute.png` / `Shooter.png` / `Dasher.png` / `Bomber.png` / `Phantom.png`
- `Boss.png` / `Boss_Pulswyrm.png` / `Boss_Nullwyrm.png`
- `Partner_S{n}_L{l}.png`, `Partner_S{n}_R{r}_L{l}.png`, `Partner_S{n}_F{f}_L{l}.png`
- `Enemy_MagmaTitan.png` etc. (stage-specific enemies)

In Unity, select each PNG and set **Texture Type** to `Sprite (2D and UI)`.

For audio, drop WAV files into `Assets/Resources/Audio/` matching names like `BGM.wav`, `BGM_Stage2.wav`, `Shoot.wav`, `Hit.wav`, etc.

Full hookup map: [`docs/ASSET_HOOKUP_MAP.md`](docs/ASSET_HOOKUP_MAP.md)

---

## 🛡 Project Rules

⚠ **Before editing `Assets/Scripts/CoreLanternGame.cs`, read [`CLAUDE.md`](CLAUDE.md)** — this is a 16k+ line single file and has hard rules to prevent recurrence of past encoding disasters.

### Architecture Note

The main game currently lives in one large C# file by design. This keeps the prototype easy to launch and move between local AI-assisted workflows, but it is not the final architecture. Public contributors should treat it as a prototype core until a staged refactor plan is opened.

Planned split, after gameplay stabilizes:

1. `GameState` and run progression
2. Player, partner, enemy, projectile, and boss systems
3. Upgrade/module/relic definitions
4. UI construction and screen flow
5. Asset loading and procedural fallback generation
6. Save data and unlock tracking

The current rule is intentionally conservative: avoid large refactors while the prototype is still changing quickly.

Key rules:
- Use **Edit / Write tools only** (never `Get-Content` / `Set-Content` for write-back)
- After bulk edits, run the **3-step verification flow** (odd-quote / brace / Roslyn)
- **Don't edit concurrently with Codex** — coordinate via `REMAINING_TASKS.md`
- Maintain UTF-8 encoding (no BOM)

---

## 🤝 Contributing

Bug reports, feature requests, translation help, and streaming/playthroughs are all welcome!

See [`CONTRIBUTING.md`](CONTRIBUTING.md) for details.

### Good First Issue Candidates

Good first contributions should avoid large gameplay rewrites and avoid editing `CoreLanternGame.cs` unless a maintainer has confirmed the scope.

- Improve README screenshots/GIF placement after public media is selected.
- Add or refine module/relic descriptions in documentation.
- Expand `THIRD_PARTY_NOTICES.md` with verified asset/audio sources.
- Improve Japanese/English wording in docs and UI copy lists.
- Add small validation checks to `Tools/` for assets, metadata, or documentation consistency.
- Create issue labels and triage categories for bugs, balance, UI, docs, assets, and accessibility.
- Add stage/enemy design notes to existing docs without changing runtime behavior.

---

## 📜 License

**Source code: [MIT License](LICENSE).** You may use, modify, and redistribute the code under MIT.

**Assets are NOT open.** All art, audio, logos, character designs, names, and the title
"Eggcore Protocol" are **© All Rights Reserved** and are included only to make the game runnable —
not for reuse. If you build on this code, replace the assets in `Assets/Resources/` with your own.

See [`ASSET_LICENSE.md`](ASSET_LICENSE.md) and [`THIRD_PARTY_NOTICES.md`](THIRD_PARTY_NOTICES.md).

---

## 📧 Contact

- **Website**: https://embercore.studio (planned)
- **Twitter / X**: @embercore_studio (planned)
- **Discord**: (coming with EA release)
- **Press inquiries**: press@embercore.studio (planned)

---

最終更新: 2026-05-28
Status: 🔨 In active development (pre-EA)
