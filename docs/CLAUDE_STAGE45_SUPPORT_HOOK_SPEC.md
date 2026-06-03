# Claude Task: Stage 4/5 Support Asset Hook-Up

Date: 2026-06-01  
Owner split: Codex generated and validated PNGs; Claude handles `CoreLanternGame.cs` hook-up.

## Goal

Stage 4 `Frost Vault` and Stage 5 `Storm Spire` already have mechanics, enemies, and BGM, but several Codex-generated visual support assets are still not statically loaded. Hook them in without changing balance.

## Assets Ready

Location: `Assets/Resources/Skins/`

| File | Size | Intended use |
|---|---:|---|
| `StageThumb_Frost.png` | 160x90 | Run setup stage chip thumbnail for Stage 4 / Frost Vault |
| `StageThumb_Storm.png` | 160x90 | Run setup stage chip thumbnail for Stage 5 / Storm Spire |
| `Stage4_FrostCrystal_A.png` | 96x96 | Frost crystal hazard/reward object |
| `Stage4_FrostPatch_A.png` | 256x256 | Frost slow-zone decal |
| `Stage5_LightningMarker_A.png` | 256x256 | Lightning warning marker |
| `Stage5_LightningStrike_A.png` | 128x512 | Lightning strike visual |

Validation already passed: files exist, dimensions are correct, `.meta` files exist, hazard/VFX corners are transparent.

## Recommended Code Hook Points

### 1. Sprite fields

Add fields near the existing stage thumbnail and stage hazard sprites:

```csharp
Sprite stageThumbFrostSprite;
Sprite stageThumbStormSprite;
Sprite stage4FrostCrystalSprite;
Sprite stage4FrostPatchSprite;
Sprite stage5LightningMarkerSprite;
Sprite stage5LightningStrikeSprite;
```

### 2. `CreateAssets()`

Load after current stage thumbnail and stage hazard loads:

```csharp
stageThumbFrostSprite = LoadOptionalSprite("Skins/StageThumb_Frost", null);
stageThumbStormSprite = LoadOptionalSprite("Skins/StageThumb_Storm", null);
stage4FrostCrystalSprite = LoadOptionalSprite("Skins/Stage4_FrostCrystal_A", null);
stage4FrostPatchSprite = LoadOptionalSprite("Skins/Stage4_FrostPatch_A", null);
stage5LightningMarkerSprite = LoadOptionalSprite("Skins/Stage5_LightningMarker_A", null);
stage5LightningStrikeSprite = LoadOptionalSprite("Skins/Stage5_LightningStrike_A", null);
```

### 3. Run setup stage chips

Current code uses thumbnails for stage index 0-2 and fallback colors for 3-4. Update both stage chip creation and `RefreshStageChips()`:

```csharp
if (s == 0) thumb = stageThumbArenaSprite;
else if (s == 1) thumb = stageThumbLavaSprite;
else if (s == 2) thumb = stageThumbBrokenCoreSprite;
else if (s == 3) thumb = stageThumbFrostSprite;
else if (s == 4) thumb = stageThumbStormSprite;
```

Keep the fallback colors for missing sprites.

### 4. Frost Vault hazards

`CreateFrostPatch(Vector2 pos, float radius)` currently uses `circleSprite`. Prefer:

```csharp
var patchSprite = stage4FrostPatchSprite != null ? stage4FrostPatchSprite : circleSprite;
```

Use white tint with controlled alpha when the PNG is present; keep the old color when falling back.

`CreateFrostCrystal(Vector2 pos)` currently uses `diamondSprite`. Prefer:

```csharp
var crystalSprite = stage4FrostCrystalSprite != null ? stage4FrostCrystalSprite : diamondSprite;
```

Do not enlarge the crystal too much. It needs to read as a target, not a full hazard zone.

### 5. Storm Spire lightning

`CreateLightningMarker(Vector2 pos)` currently uses `circleSprite`. Prefer `Stage5_LightningMarker_A` for the telegraph marker.

For the strike moment, add a short-lived visual object using `Stage5_LightningStrike_A` at the marker position. If there is already a strike resolution branch in `UpdateStageHazards`, spawn there and destroy it after roughly `0.18f` to `0.28f` seconds.

Keep damage timing unchanged. This is only visual polish.

## Visual QA

Check in Unity Game view:

- Stage 4/5 stage select chips show thumbnails and remain readable when locked/unlocked/selected.
- Frost patch does not hide enemies or pickups.
- Frost crystal is clickable/damageable-looking and not confused with data pickups.
- Lightning marker is visible before damage, but not so bright that it hides projectiles.
- Lightning strike does not persist after the hazard resolves.
- No asset is stretched into a non-native full-screen panel.

## Verification

After hook-up:

```powershell
.\Tools\CheckCompileHealth.ps1
.\Tools\TestImageResources.ps1
.\Tools\ReportAssetStatus.ps1
```

Expected after a complete hook-up:

- Stage 4/5 support `Hooked: 6`
- `TestImageResources.ps1` errors 0
