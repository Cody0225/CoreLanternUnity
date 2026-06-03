from __future__ import annotations

import json
import math
import shutil
from collections import Counter, deque
from datetime import datetime
from pathlib import Path
from typing import Iterable

from PIL import Image, ImageDraw, ImageFont


TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601"
SOURCE_DIR = ART / "color_identity_polish_20260602" / "backup_before_polish_20260602_143616"
OUT_DIR = ART / "hexcat_driftfox_baked_redraw_20260602"


SPRITES = {
    "S4": {
        "file": "Partner_S4_L0.png",
        "name": "Hex Cat",
        "family": "purple",
        "source": SOURCE_DIR / "Partner_S4_L0.png",
        "palette": {
            "outline": (20, 7, 35),
            "deep": (52, 22, 88),
            "shadow": (92, 45, 146),
            "base": (145, 78, 210),
            "mid": (190, 126, 250),
            "light": (226, 199, 255),
            "spark": (246, 238, 255),
            "eye": (250, 246, 255),
        },
    },
    "S5": {
        "file": "Partner_S5_L0.png",
        "name": "Drift Fox",
        "family": "pink",
        "source": SOURCE_DIR / "Partner_S5_L0.png",
        "palette": {
            "outline": (43, 9, 32),
            "deep": (91, 30, 70),
            "shadow": (138, 54, 106),
            "base": (214, 90, 155),
            "mid": (240, 142, 193),
            "light": (255, 208, 235),
            "spark": (255, 241, 250),
            "white_deep": (205, 189, 217),
            "white_shadow": (235, 219, 240),
            "white": (255, 244, 252),
            "gold_deep": (142, 81, 19),
            "gold": (235, 169, 51),
            "gold_light": (255, 226, 108),
        },
    },
}


def luminance(rgb: tuple[int, int, int]) -> float:
    r, g, b = rgb
    return 0.2126 * r + 0.7152 * g + 0.0722 * b


def saturation(rgb: tuple[int, int, int]) -> float:
    r, g, b = [v / 255.0 for v in rgb]
    mx = max(r, g, b)
    mn = min(r, g, b)
    if mx <= 0:
        return 0.0
    return (mx - mn) / mx


def percentile(values: list[float], pct: float) -> float:
    if not values:
        return 0.0
    ordered = sorted(values)
    k = (len(ordered) - 1) * pct / 100.0
    lo = int(math.floor(k))
    hi = int(math.ceil(k))
    if lo == hi:
        return ordered[lo]
    frac = k - lo
    return ordered[lo] * (1.0 - frac) + ordered[hi] * frac


def connected_components(img: Image.Image) -> list[dict[str, object]]:
    pix = img.load()
    w, h = img.size
    seen: set[tuple[int, int]] = set()
    comps: list[dict[str, object]] = []
    for y in range(h):
        for x in range(w):
            if (x, y) in seen or pix[x, y][3] <= ALPHA_THRESHOLD:
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen.add((x, y))
            coords: list[tuple[int, int]] = []
            while q:
                cx, cy = q.popleft()
                coords.append((cx, cy))
                for nx in (cx - 1, cx, cx + 1):
                    for ny in (cy - 1, cy, cy + 1):
                        if nx == cx and ny == cy:
                            continue
                        if 0 <= nx < w and 0 <= ny < h and (nx, ny) not in seen and pix[nx, ny][3] > ALPHA_THRESHOLD:
                            seen.add((nx, ny))
                            q.append((nx, ny))
            xs = [p[0] for p in coords]
            ys = [p[1] for p in coords]
            comps.append(
                {
                    "coords": coords,
                    "count": len(coords),
                    "bbox": (min(xs), min(ys), max(xs), max(ys)),
                }
            )
    comps.sort(key=lambda c: int(c["count"]), reverse=True)
    return comps


def is_edge_pixel(x: int, y: int, body: set[tuple[int, int]]) -> bool:
    for nx in (x - 1, x, x + 1):
        for ny in (y - 1, y, y + 1):
            if nx == x and ny == y:
                continue
            if (nx, ny) not in body:
                return True
    return False


def lerp(a: tuple[int, int, int], b: tuple[int, int, int], t: float) -> tuple[int, int, int]:
    t = max(0.0, min(1.0, t))
    return tuple(int(round(a[i] * (1.0 - t) + b[i] * t)) for i in range(3))


def shade_from_palette(pal: dict[str, tuple[int, int, int]], t: float, edge: bool) -> tuple[int, int, int]:
    if edge and t < 0.82:
        return pal["outline"] if t < 0.55 else pal["deep"]
    if t < 0.12:
        return pal["outline"]
    if t < 0.27:
        return pal["deep"]
    if t < 0.43:
        return pal["shadow"]
    if t < 0.60:
        return pal["base"]
    if t < 0.76:
        return pal["mid"]
    if t < 0.91:
        return pal["light"]
    return pal["spark"]


def paint_hex_cat(rgb: tuple[int, int, int], t: float, edge: bool, pal: dict[str, tuple[int, int, int]]) -> tuple[int, int, int]:
    lum = luminance(rgb)
    sat = saturation(rgb)
    # Bright gems and eye whites stay readable instead of becoming flat purple.
    if lum > 170 and sat < 0.22:
        if edge:
            return pal["light"]
        return lerp(pal["light"], pal["spark"], (lum - 170) / 85.0)
    return shade_from_palette(pal, t, edge)


def paint_drift_fox(rgb: tuple[int, int, int], t: float, edge: bool, pal: dict[str, tuple[int, int, int]]) -> tuple[int, int, int]:
    r, g, b = rgb
    lum = luminance(rgb)
    sat = saturation(rgb)
    # Keep the chest core gold and give it enough contrast.
    if r > 130 and g > 85 and b < 90 and sat > 0.35:
        if edge or t < 0.35:
            return pal["gold_deep"]
        if t > 0.80:
            return pal["gold_light"]
        return pal["gold"]
    # Keep the muzzle, ear fluff and tail tip as warm white accents.
    if lum > 176 and sat < 0.30:
        if edge and t < 0.75:
            return pal["white_deep"]
        if t < 0.68:
            return pal["white_shadow"]
        return pal["white"]
    # Old cyan glow becomes pink-white energy, not blue.
    if b > 145 and g > 115 and r < 120:
        if edge:
            return pal["mid"]
        return lerp(pal["light"], pal["spark"], t)
    return shade_from_palette(pal, t, edge)


def redraw_sprite(spec: dict[str, object]) -> dict[str, object]:
    target = SKINS / str(spec["file"])
    source_path = Path(spec["source"])
    if not source_path.exists():
        source_path = target

    src = Image.open(source_path).convert("RGBA")
    comps = connected_components(src)
    if not comps:
        raise RuntimeError(f"No visible component in {source_path}")

    main_coords = set(comps[0]["coords"])
    min_x, min_y, max_x, max_y = comps[0]["bbox"]
    y_shift = TARGET_BOTTOM_Y - (max_y + 1)

    lums = [luminance(src.getpixel((x, y))[:3]) for x, y in main_coords]
    p04 = percentile(lums, 4)
    p96 = percentile(lums, 96)
    span = max(1.0, p96 - p04)

    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    family = str(spec["family"])
    pal = spec["palette"]  # type: ignore[assignment]
    assert isinstance(pal, dict)

    for x, y in main_coords:
        r, g, b, a = src.getpixel((x, y))
        if a <= ALPHA_THRESHOLD:
            continue
        ny = y + y_shift
        if not 0 <= ny < 512:
            continue
        lum = luminance((r, g, b))
        t = (lum - p04) / span
        t = max(0.0, min(1.0, t))
        edge = is_edge_pixel(x, y, main_coords)
        if family == "purple":
            nr, ng, nb = paint_hex_cat((r, g, b), t, edge, pal)  # type: ignore[arg-type]
        else:
            nr, ng, nb = paint_drift_fox((r, g, b), t, edge, pal)  # type: ignore[arg-type]
        out.putpixel((x, ny), (nr, ng, nb, 255))

    # The body is painted directly on transparent RGBA. Re-read bbox from output.
    target.parent.mkdir(parents=True, exist_ok=True)
    out.save(target)
    out_copy = OUT_DIR / f"{target.stem}_BAKED_REDRAW.png"
    out_copy.parent.mkdir(parents=True, exist_ok=True)
    out.save(out_copy)
    return measure_sprite(target, family)


def visible_pixels(img: Image.Image) -> list[tuple[int, int, tuple[int, int, int, int]]]:
    pix = img.load()
    w, h = img.size
    return [(x, y, pix[x, y]) for y in range(h) for x in range(w) if pix[x, y][3] > ALPHA_THRESHOLD]


def measure_sprite(path: Path, family: str) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    vis = visible_pixels(img)
    if not vis:
        raise RuntimeError(f"No visible pixels: {path}")
    xs = [p[0] for p in vis]
    ys = [p[1] for p in vis]
    bbox = (min(xs), min(ys), max(xs), max(ys))
    bottom_y = bbox[3] + 1
    height = bottom_y - bbox[1]
    corners = [img.getpixel((0, 0))[3], img.getpixel((511, 0))[3], img.getpixel((0, 511))[3], img.getpixel((511, 511))[3]]

    # Pick a representative colored body sample close to the sprite center.
    center_x = (bbox[0] + bbox[2]) / 2.0
    center_y = (bbox[1] + bbox[3]) / 2.0
    candidates = []
    for x, y, rgba in vis:
        r, g, b, a = rgba
        if a < 230:
            continue
        if family == "purple":
            ok = b >= r >= g and (r + b) > 120
        else:
            ok = r >= b >= g and (r + b) > 150
        if ok:
            candidates.append((abs(x - center_x) + abs(y - center_y), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - center_x) + abs(y - center_y), x, y, rgba) for x, y, rgba in vis if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda v: v[0])
    sample_rgb = sample[:3]
    sample_hex = "#{:02X}{:02X}{:02X}".format(*sample_rgb)

    comps = connected_components(img)
    luma_values = [luminance(rgba[:3]) for _, _, rgba in vis if rgba[3] >= 230]
    unique_colors = Counter([rgba[:3] for _, _, rgba in vis if rgba[3] >= 230])
    major_colors = sum(1 for _, count in unique_colors.items() if count >= 64)
    semi_transparent = sum(1 for _, _, rgba in vis if 0 < rgba[3] < 230)

    return {
        "file": path.name,
        "size": list(img.size),
        "bbox": list(bbox),
        "bottom_y": bottom_y,
        "height": height,
        "corner_alpha": corners,
        "component_count": len(comps),
        "largest_component_pixels": int(comps[0]["count"]) if comps else 0,
        "semi_transparent_pixels": semi_transparent,
        "sample_xy": [sx, sy],
        "sample_color": sample_hex,
        "sample_alpha": int(sample[3]),
        "luma_range": [round(min(luma_values), 2), round(max(luma_values), 2)],
        "major_color_count": major_colors,
    }


def backup_targets() -> Path:
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = OUT_DIR / f"backup_before_baked_redraw_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)
    for spec in SPRITES.values():
        src = SKINS / str(spec["file"])
        if src.exists():
            shutil.copy2(src, backup_dir / src.name)
    return backup_dir


def paste_fit(dst: Image.Image, sprite: Image.Image, box: tuple[int, int, int, int]) -> None:
    x0, y0, x1, y1 = box
    vis = visible_pixels(sprite)
    if not vis:
        return
    xs = [p[0] for p in vis]
    ys = [p[1] for p in vis]
    bbox = (min(xs), min(ys), max(xs), max(ys))
    crop = sprite.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    max_w = x1 - x0
    max_h = y1 - y0
    scale = min(max_w / crop.width, max_h / crop.height)
    new_size = (max(1, int(crop.width * scale)), max(1, int(crop.height * scale)))
    resized = crop.resize(new_size, Image.Resampling.NEAREST)
    px = x0 + (max_w - new_size[0]) // 2
    py = y1 - new_size[1]
    dst.alpha_composite(resized, (px, py))


def make_previews() -> dict[str, str]:
    names = [f"Partner_S{i}_L0.png" for i in range(1, 7)]
    sprites = [Image.open(SKINS / name).convert("RGBA") for name in names]

    transparent = Image.new("RGBA", (512 * 6, 512), (0, 0, 0, 0))
    for i, sprite in enumerate(sprites):
        transparent.alpha_composite(sprite, (512 * i, 0))
    transparent_path = ART / "CharacterPixelArt_6partners_lineup_clean_transparent.png"
    transparent.save(transparent_path)

    preview = Image.new("RGBA", (1920, 560), (3, 10, 16, 255))
    draw = ImageDraw.Draw(preview)
    try:
        font_title = ImageFont.truetype("arial.ttf", 34)
        font_label = ImageFont.truetype("arial.ttf", 24)
    except OSError:
        font_title = ImageFont.load_default()
        font_label = ImageFont.load_default()
    draw.text((40, 28), "Partner L0 lineup baked recolor check", fill=(170, 245, 245), font=font_title)
    labels = ["Cobalt", "Ember", "Sage", "Hex", "Drift", "Iron"]
    colors = [(80, 220, 255), (255, 190, 45), (110, 240, 120), (195, 120, 255), (255, 150, 215), (210, 205, 170)]
    cell_w = 300
    start_x = 72
    for i, sprite in enumerate(sprites):
        x = start_x + cell_w * i
        col = colors[i]
        draw.rectangle((x, 92, x + 240, 486), outline=col + (255,), width=2)
        draw.ellipse((x + 52, 140, x + 188, 276), fill=col + (42,))
        paste_fit(preview, sprite, (x + 25, 118, x + 215, 420))
        draw.text((x + 76, 438), labels[i], fill=(225, 244, 246), font=font_label)
    preview_path = ART / "CharacterPixelArt_6partners_lineup_preview.png"
    preview.save(preview_path)

    shrink = Image.new("RGBA", (1600, 760), (4, 8, 12, 255))
    draw = ImageDraw.Draw(shrink)
    draw.text((36, 28), "Shrink readability check: 128 / 96 / 64 px", fill=(170, 245, 245), font=font_title)
    sizes = [128, 96, 64]
    for row, sz in enumerate(sizes):
        y = 118 + row * 200
        draw.text((42, y + 40), f"{sz}px", fill=(220, 230, 230), font=font_label)
        for i, sprite in enumerate(sprites):
            x = 180 + i * 220
            vis = visible_pixels(sprite)
            xs = [p[0] for p in vis]
            ys = [p[1] for p in vis]
            crop = sprite.crop((min(xs), min(ys), max(xs) + 1, max(ys) + 1))
            scale = sz / max(crop.width, crop.height)
            resized = crop.resize((int(crop.width * scale), int(crop.height * scale)), Image.Resampling.NEAREST)
            draw.rectangle((x - 18, y - 10, x + 150, y + 168), outline=colors[i] + (180,), width=1)
            shrink.alpha_composite(resized, (x + (132 - resized.width) // 2, y + 126 - resized.height))
            draw.text((x + 38, y + 138), labels[i], fill=(225, 244, 246), font=font_label)
    shrink_path = ART / "CharacterPixelArt_6partners_shrink_check.png"
    shrink.save(shrink_path)

    visibility = Image.new("RGBA", (1180, 520), (0, 0, 0, 0))
    draw = ImageDraw.Draw(visibility)
    backgrounds = [
        ((4, 8, 12), "dark"),
        ((39, 52, 63), "blue gray"),
        ((230, 232, 228), "light"),
    ]
    target_sprites = [Image.open(SKINS / "Partner_S4_L0.png").convert("RGBA"), Image.open(SKINS / "Partner_S5_L0.png").convert("RGBA")]
    target_labels = ["Hex Cat", "Drift Fox"]
    for bi, (bg, label) in enumerate(backgrounds):
        x0 = 20 + bi * 380
        draw.rectangle((x0, 20, x0 + 350, 500), fill=bg + (255,), outline=(80, 230, 230, 255), width=1)
        draw.text((x0 + 18, 34), label, fill=(180, 245, 245), font=font_label)
        for si, sprite in enumerate(target_sprites):
            paste_fit(visibility, sprite, (x0 + 40 + si * 155, 86, x0 + 175 + si * 155, 382))
            draw.text((x0 + 68 + si * 155, 410), target_labels[si], fill=(230, 245, 245), font=font_label)
    visibility_path = OUT_DIR / "HexCat_DriftFox_visibility_check_after.png"
    visibility.save(visibility_path)

    return {
        "lineup_preview": str(preview_path.relative_to(ROOT)),
        "shrink_check": str(shrink_path.relative_to(ROOT)),
        "transparent_lineup": str(transparent_path.relative_to(ROOT)),
        "visibility_check": str(visibility_path.relative_to(ROOT)),
    }


def main() -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    backup_dir = backup_targets()
    meta_before = {}
    for spec in SPRITES.values():
        meta = SKINS / f"{str(spec['file'])}.meta"
        if meta.exists():
            meta_before[meta.name] = {
                "mtime": meta.stat().st_mtime,
                "size": meta.stat().st_size,
                "last_byte": meta.read_bytes()[-1] if meta.stat().st_size else None,
            }

    measurements: dict[str, object] = {}
    for key, spec in SPRITES.items():
        measurements[key] = redraw_sprite(spec)

    previews = make_previews()

    meta_after = {}
    for spec in SPRITES.values():
        meta = SKINS / f"{str(spec['file'])}.meta"
        if meta.exists():
            meta_after[meta.name] = {
                "mtime": meta.stat().st_mtime,
                "size": meta.stat().st_size,
                "last_byte": meta.read_bytes()[-1] if meta.stat().st_size else None,
            }

    report = {
        "handoff_quality_gate_read": True,
        "method": "direct transparent RGBA repaint from preserved silhouette/luminance source; no chromakey; no meta write",
        "backup_dir": str(backup_dir.relative_to(ROOT)),
        "source_dir": str(SOURCE_DIR.relative_to(ROOT)),
        "outputs": {
            "S4": str((SKINS / "Partner_S4_L0.png").relative_to(ROOT)),
            "S5": str((SKINS / "Partner_S5_L0.png").relative_to(ROOT)),
        },
        "measurements": measurements,
        "previews": previews,
        "meta_before": meta_before,
        "meta_after": meta_after,
        "meta_unchanged": meta_before == meta_after,
    }
    report_path = OUT_DIR / "hexcat_driftfox_baked_redraw_report.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
