from __future__ import annotations

import json
import shutil
import sys
from collections import Counter, deque
from datetime import datetime
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
OUT_DIR = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601" / "s1_speed_wave1_20260602"

TARGETS = [
    ("Partner_S1_R1_L1.png", "R1_L1", "SPEED initial: cyber wolf pup"),
    ("Partner_S1_R1_L2.png", "R1_L2", "SPEED mid: young standing wolf"),
    ("Partner_S1_R1_L3.png", "R1_L3", "SPEED final: adult crystal wing wolf"),
]

TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8


def is_checker_bg(px: tuple[int, int, int, int]) -> bool:
    r, g, b, a = px
    if a == 0:
        return True
    return min(r, g, b) >= 224 and max(r, g, b) - min(r, g, b) <= 22


def remove_generated_checkerboard(src: Image.Image) -> Image.Image:
    img = src.convert("RGBA")
    pix = img.load()
    w, h = img.size
    seen: set[tuple[int, int]] = set()
    q: deque[tuple[int, int]] = deque()

    for x in range(w):
        for y in (0, h - 1):
            if (x, y) not in seen and is_checker_bg(pix[x, y]):
                seen.add((x, y))
                q.append((x, y))
    for y in range(h):
        for x in (0, w - 1):
            if (x, y) not in seen and is_checker_bg(pix[x, y]):
                seen.add((x, y))
                q.append((x, y))

    while q:
        cx, cy = q.popleft()
        for nx, ny in ((cx - 1, cy), (cx + 1, cy), (cx, cy - 1), (cx, cy + 1)):
            if 0 <= nx < w and 0 <= ny < h and (nx, ny) not in seen and is_checker_bg(pix[nx, ny]):
                seen.add((nx, ny))
                q.append((nx, ny))

    out = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    out_pix = out.load()
    for y in range(h):
        for x in range(w):
            if (x, y) in seen:
                continue
            r, g, b, a = pix[x, y]
            out_pix[x, y] = (r, g, b, 255 if a > 0 else 0)
    return out


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
                    "count": len(coords),
                    "bbox": (min(xs), min(ys), max(xs), max(ys)),
                    "coords": coords,
                    "cx": sum(xs) / len(xs),
                }
            )
    comps.sort(key=lambda c: int(c["count"]), reverse=True)
    return comps


def group_stage_canvases(sheet: Image.Image) -> list[Image.Image]:
    w, h = sheet.size
    stage_centers = [w * 0.18, w * 0.50, w * 0.82]
    groups = [Image.new("RGBA", sheet.size, (0, 0, 0, 0)) for _ in range(3)]
    src_pix = sheet.load()
    group_pix = [g.load() for g in groups]

    for comp in connected_components(sheet):
        if int(comp["count"]) < 10:
            continue
        cx = float(comp["cx"])
        idx = min(range(3), key=lambda i: abs(stage_centers[i] - cx))
        for x, y in comp["coords"]:  # type: ignore[assignment]
            group_pix[idx][x, y] = src_pix[x, y]
    return groups


def visible_bbox(img: Image.Image) -> tuple[int, int, int, int]:
    pix = img.load()
    w, h = img.size
    xs: list[int] = []
    ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if pix[x, y][3] > ALPHA_THRESHOLD:
                xs.append(x)
                ys.append(y)
    if not xs:
        raise RuntimeError("empty sprite")
    return min(xs), min(ys), max(xs), max(ys)


def fit_stage(group: Image.Image, index: int) -> Image.Image:
    x0, y0, x1, y1 = visible_bbox(group)
    crop = group.crop((x0, y0, x1 + 1, y1 + 1))
    target_heights = [286, 392, 424]
    max_widths = [410, 506, 506]
    scale = min(target_heights[index] / crop.height, max_widths[index] / crop.width, 1.25)
    resized = crop.resize((max(1, round(crop.width * scale)), max(1, round(crop.height * scale))), Image.Resampling.LANCZOS)

    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    px = (512 - resized.width) // 2
    py = TARGET_BOTTOM_Y - resized.height
    out.alpha_composite(resized, (px, py))
    return quantize_alpha(out)


def quantize_alpha(img: Image.Image) -> Image.Image:
    out = img.convert("RGBA")
    pix = out.load()
    w, h = out.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            else:
                pix[x, y] = (r, g, b, 255)
    return out


def measure(path: Path) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    bbox = visible_bbox(img)
    corners = [
        img.getpixel((0, 0))[3],
        img.getpixel((511, 0))[3],
        img.getpixel((0, 511))[3],
        img.getpixel((511, 511))[3],
    ]
    pix = img.load()
    visible = [(x, y, pix[x, y]) for y in range(512) for x in range(512) if pix[x, y][3] > ALPHA_THRESHOLD]
    center_x = (bbox[0] + bbox[2]) / 2
    center_y = (bbox[1] + bbox[3]) / 2
    candidates = []
    for x, y, rgba in visible:
        r, g, b, a = rgba
        if a >= 230 and b > 100 and b >= r + 20 and b >= g - 30 and not (r > 220 and g > 220 and b > 225):
            candidates.append((abs(x - center_x) + abs(y - center_y), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - center_x) + abs(y - center_y), x, y, rgba) for x, y, rgba in visible if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda item: item[0])
    colors = Counter(rgba[:3] for _, _, rgba in visible if rgba[3] >= 230)
    major_color_count = sum(1 for _, count in colors.items() if count >= 32)
    return {
        "file": path.name,
        "size": list(img.size),
        "bbox": list(bbox),
        "bottom_y": bbox[3] + 1,
        "height": bbox[3] - bbox[1] + 1,
        "corner_alpha": corners,
        "sample_xy": [sx, sy],
        "sample_color": "#{:02X}{:02X}{:02X}".format(*sample[:3]),
        "sample_alpha": int(sample[3]),
        "visible_pixels": len(visible),
        "semi_transparent_pixels": sum(1 for _, _, rgba in visible if 0 < rgba[3] < 230),
        "major_color_count": major_color_count,
    }


def meta_snapshot() -> dict[str, dict[str, object]]:
    snap: dict[str, dict[str, object]] = {}
    for filename, _, _ in TARGETS:
        path = SKINS / f"{filename}.meta"
        data = path.read_bytes() if path.exists() else b""
        snap[path.name] = {
            "exists": path.exists(),
            "size": path.stat().st_size if path.exists() else None,
            "mtime": path.stat().st_mtime if path.exists() else None,
            "last_byte": data[-1] if data else None,
        }
    return snap


def make_preview(measurements: dict[str, dict[str, object]]) -> dict[str, str]:
    sprites = [Image.open(SKINS / filename).convert("RGBA") for filename, _, _ in TARGETS]

    preview = Image.new("RGBA", (1500, 700), (3, 10, 18, 255))
    draw = ImageDraw.Draw(preview)
    try:
        title_font = ImageFont.truetype("arial.ttf", 34)
        label_font = ImageFont.truetype("arial.ttf", 23)
        small_font = ImageFont.truetype("arial.ttf", 17)
    except OSError:
        title_font = ImageFont.load_default()
        label_font = ImageFont.load_default()
        small_font = ImageFont.load_default()

    draw.text((44, 26), "Cobalt Pup SPEED Evolution - rebuilt full-body Wave 1", fill=(176, 248, 255), font=title_font)
    names = ["L1 Pup Sprint", "L2 Young Vector Wolf", "L3 Adult Crystal Strider"]
    for i, sprite in enumerate(sprites):
        x = 60 + i * 470
        draw.rectangle((x, 88, x + 390, 596), outline=(82, 255, 255, 255), width=2)
        draw.ellipse((x + 80, 162, x + 316, 398), fill=(30, 175, 255, 28))
        preview.alpha_composite(sprite, (x + (390 - 512) // 2, 52))
        draw.rectangle((x + 16, 606, x + 374, 686), fill=(3, 10, 18, 230))
        draw.text((x + 64, 616), names[i], fill=(235, 253, 255), font=label_font)
        draw.text((x + 68, 650), TARGETS[i][0], fill=(140, 215, 224), font=small_font)
        if i < 2:
            draw.line((x + 410, 342, x + 450, 342), fill=(82, 255, 255, 255), width=4)
            draw.polygon([(x + 450, 342), (x + 436, 332), (x + 436, 352)], fill=(82, 255, 255, 255))
    preview_path = OUT_DIR / "S1_SPEED_Wave1_L1_L2_L3_preview.png"
    preview.save(preview_path)

    transparent = Image.new("RGBA", (1536, 512), (0, 0, 0, 0))
    for i, sprite in enumerate(sprites):
        transparent.alpha_composite(sprite, (i * 512, 0))
    transparent_path = OUT_DIR / "S1_SPEED_Wave1_L1_L2_L3_transparent_strip.png"
    transparent.save(transparent_path)

    dark = Image.new("RGBA", (1240, 620), (2, 8, 13, 255))
    draw = ImageDraw.Draw(dark)
    draw.text((36, 24), "Dark background and shrink-readability check", fill=(176, 248, 255), font=title_font)
    for i, sprite in enumerate(sprites):
        x = 48 + i * 395
        draw.rectangle((x, 78, x + 360, 540), outline=(40, 170, 220, 255), width=1)
        dark.alpha_composite(sprite, (x + (360 - 512) // 2, 42))
        draw.rectangle((x + 76, 552, x + 284, 594), fill=(2, 8, 13, 230))
        draw.text((x + 112, 560), TARGETS[i][1], fill=(235, 253, 255), font=label_font)
    dark_path = OUT_DIR / "S1_SPEED_Wave1_dark_readability_check.png"
    dark.save(dark_path)

    return {
        "preview": str(preview_path.relative_to(ROOT)),
        "transparent_strip": str(transparent_path.relative_to(ROOT)),
        "dark_check": str(dark_path.relative_to(ROOT)),
    }


def backup_targets() -> Path:
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = OUT_DIR / f"backup_before_s1_speed_wave1_fullbody_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)
    for filename, _, _ in TARGETS:
        target = SKINS / filename
        if target.exists():
            shutil.copy2(target, backup_dir / filename)
    return backup_dir


def main() -> None:
    if len(sys.argv) < 2:
        raise SystemExit("Usage: python extract_s1_speed_wave1_from_generated_sheet.py <generated_sheet.png>")

    source = Path(sys.argv[1])
    if not source.exists():
        raise FileNotFoundError(source)

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    backup_dir = backup_targets()
    meta_before = meta_snapshot()

    source_copy = OUT_DIR / "S1_SPEED_Wave1_fullbody_generated_source.png"
    shutil.copy2(source, source_copy)
    sheet = Image.open(source).convert("RGBA")
    transparent_sheet = remove_generated_checkerboard(sheet)
    transparent_sheet_path = OUT_DIR / "S1_SPEED_Wave1_fullbody_generated_source_alpha.png"
    transparent_sheet.save(transparent_sheet_path)

    stage_groups = group_stage_canvases(transparent_sheet)
    measurements: dict[str, dict[str, object]] = {}
    for i, (filename, _, _) in enumerate(TARGETS):
        fitted = fit_stage(stage_groups[i], i)
        target = SKINS / filename
        fitted.save(target)
        fitted.save(OUT_DIR / f"{Path(filename).stem}_FULLBODY_WAVE1.png")
        measurements[filename] = measure(target)

    previews = make_preview(measurements)
    meta_after = meta_snapshot()
    report = {
        "wave": 1,
        "handoff_quality_gate_read": True,
        "method": "imagegen full-body source, generated checkerboard removed by edge flood-fill only; no chroma-key color; PNG overwrite only",
        "source": str(source),
        "source_copy": str(source_copy.relative_to(ROOT)),
        "transparent_source": str(transparent_sheet_path.relative_to(ROOT)),
        "backup_dir": str(backup_dir.relative_to(ROOT)),
        "targets": [str((SKINS / t[0]).relative_to(ROOT)) for t in TARGETS],
        "measurements": measurements,
        "previews": previews,
        "meta_before": meta_before,
        "meta_after": meta_after,
        "meta_unchanged": meta_before == meta_after,
    }
    report_path = OUT_DIR / "s1_speed_wave1_validation_report.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
