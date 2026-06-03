# Third Party Notices

Status: incomplete.

This file must be completed before public OSS submission.

## Unity

- Unity Engine and Unity packages are subject to Unity Technologies licensing terms.
- This repository does not redistribute the Unity Editor.

## Packages

See `Packages/manifest.json`.

| Package | Version | Notes |
|---|---:|---|
| `com.unity.ugui` | 2.0.0 | Unity UI package |
| `com.unity.2d.sprite` | 1.0.0 | Unity 2D sprite package |
| `com.unity.multiplayer.center` | 1.0.1 | Present in manifest; review whether required |
| `com.unity.modules.audio` | 1.0.0 | Built-in Unity module |
| `com.unity.modules.vectorgraphics` | 1.0.0 | Built-in Unity module |
| `com.unity.modules.accessibility` | 1.0.0 | Built-in Unity module |
| `com.unity.modules.adaptiveperformance` | 1.0.0 | Built-in Unity module |

## Audio

Runtime audio files are under `Assets/Resources/Audio/`.

| Asset | Source | License | Status |
|---|---|---|---|
| Placeholder BGM/SE | Local generated placeholder audio | Pending confirmation | Needs final review |

## Fonts

The project dynamically uses locally installed OS fonts where available.

| Font | Source | Notes |
|---|---|---|
| Yu Gothic UI / Yu Gothic / Meiryo / MS Gothic / Arial | User OS fonts | Not redistributed by this repository unless separately included |

## Generated Art

AI-assisted or procedurally generated art must be reviewed before public release.

| Category | Path | Status |
|---|---|---|
| Runtime skins | `Assets/Resources/Skins/` | Pending asset-rights review |
| Art source previews | `Assets/ArtSource/` | Pending asset-rights review |

## To Complete

- Confirm every bundled audio file source.
- Confirm every bundled non-generated third-party asset, if any.
- Decide whether AI-generated art is allowed for the target OSS submission.
- Remove any asset that cannot be documented.
