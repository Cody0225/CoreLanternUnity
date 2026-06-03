# Claude Task: Windows Exe Icon Hook-Up

Date: 2026-05-31
Owner split: Codex generated assets; Claude/Unity handles Player Settings.

## Goal

Use the Codex-generated Eggcore Protocol icon as the Unity application icon for Windows builds.

## Assets Already Generated

Codex generated the Windows icon set:

- `build/Icons/icon_16.png`
- `build/Icons/icon_32.png`
- `build/Icons/icon_48.png`
- `build/Icons/icon_64.png`
- `build/Icons/icon_128.png`
- `build/Icons/icon_256.png`
- `build/Icons/icon_512.png`
- `build/Icons/EggcoreProtocol.ico`

Art-source/importable copies:

- `Assets/ArtSource/Icon_Drafts_20260531/icon_16.png`
- `Assets/ArtSource/Icon_Drafts_20260531/icon_32.png`
- `Assets/ArtSource/Icon_Drafts_20260531/icon_48.png`
- `Assets/ArtSource/Icon_Drafts_20260531/icon_64.png`
- `Assets/ArtSource/Icon_Drafts_20260531/icon_128.png`
- `Assets/ArtSource/Icon_Drafts_20260531/icon_256.png`
- `Assets/ArtSource/Icon_Drafts_20260531/icon_512.png`
- `Assets/ArtSource/Icon_Drafts_20260531/EggcoreProtocol.ico`

Preview:

- `Assets/ArtSource/Icon_Drafts_20260531/IconSet_Preview.png`

## Recommended Unity Setup

1. In Unity, select `Assets/ArtSource/Icon_Drafts_20260531/icon_256.png`.
2. In Inspector import settings:
   - Texture Type: `Default`
   - Alpha Source: `Input Texture Alpha`
   - Alpha Is Transparency: enabled if available
   - Compression: `None` or `High Quality`
3. Open `Edit > Project Settings > Player`.
4. Under `Icon`, set the default application icon to `icon_256.png`.
5. If Unity exposes per-size Windows icons, assign:
   - 16: `icon_16.png`
   - 32: `icon_32.png`
   - 48: `icon_48.png`
   - 64: `icon_64.png`
   - 128: `icon_128.png`
   - 256: `icon_256.png`

## Notes

- Unity usually wants imported Texture2D assets, not the `.ico` file directly. Use the PNG files inside `Assets/ArtSource/Icon_Drafts_20260531/` for Player Settings.
- `build/Icons/EggcoreProtocol.ico` is still useful for installers, Windows shortcuts, press kit packaging, or manual executable metadata workflows.
- Do not move the generated source icons unless docs and `IMAGE_ASSET_BACKLOG.md` are updated.

## Verification

After setting the icon:

1. Build a Windows player.
2. Check the generated `.exe` in File Explorer at small and large icon sizes.
3. Pin/run it and confirm the taskbar icon is readable.
4. If Windows caches the old icon, rename the build folder or clear icon cache before judging.
