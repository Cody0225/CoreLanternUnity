from __future__ import annotations

import json
import shutil
from collections import Counter
from datetime import datetime
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601" / "s1_speed_wave1_20260602"

SOURCE = ROOT / "Assets" / "ArtSource" / "CharacterDrafts_Cobalt_20260524" / "ready_512" / "Partner_S1_R1_L3.png"
TARGET = SKINS / "Partner_S1_R1_L3.png"
TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8

WAVE_TARGETS = [
    ("Partner_S1_R1_L1.png", "R1_L1", "L1 Pup Sprint"),
    ("Partner_S1_R1_L2.png", "R1_L2", "L2 Young Vector Wolf"),
    ("Partner_S1_R1_L3.png", "R1_L3", "L3 Clean Adult Strider"),
]


def visible_bbox(img: Image.Image) -> tuple[int, int, int, int]:
    pix = img.load()
    xs: list[int] = []
    ys: list[int] = []
    for y in range(img.height):
        for x in range(img.width):
            if pix[x, y][3] > ALPHA_THRESHOLD:
                xs.append(x)
                ys.append(y)
    if not xs:
        raise RuntimeError("empty image")
    return min(xs), min(ys), max(xs), max(ys)


def normalize_l3(src: Image.Image) -> Image.Image:
    img = src.convert("RGBA")
    bbox = visible_bbox(img)
    crop = img.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))

    # Keep the clean adult silhouette large, but avoid edge clipping.
    max_w = 500
    max_h = 420
    scale = min(max_w / crop.width, max_h / crop.height, 1.0)
    if scale != 1.0:
        crop = crop.resize((round(crop.width * scale), round(crop.height * scale)), Image.Resampling.LANCZOS)

    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    px = (512 - crop.width) // 2
    py = TARGET_BOTTOM_Y - crop.height
    out.alpha_composite(crop, (px, py))

    pix = out.load()
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)
    return out


def measure(path: Path) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    bbox = visible_bbox(img)
    pix = img.load()
    visible = [(x, y, pix[x, y]) for y in range(img.height) for x in range(img.width) if pix[x, y][3] > ALPHA_THRESHOLD]
    corners = [
        img.getpixel((0, 0))[3],
        img.getpixel((511, 0))[3],
        img.getpixel((0, 511))[3],
        img.getpixel((511, 511))[3],
    ]
    cx = (bbox[0] + bbox[2]) / 2
    cy = (bbox[1] + bbox[3]) / 2
    candidates = []
    for x, y, rgba in visible:
        r, g, b, a = rgba
        if a >= 230 and b > 100 and b >= r + 20 and b >= g - 40 and not (r > 220 and g > 220 and b > 225):
            candidates.append((abs(x - cx) + abs(y - cy), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - cx) + abs(y - cy), x, y, rgba) for x, y, rgba in visible if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda item: item[0])
    colors = Counter(rgba[:3] for _, _, rgba in visible if rgba[3] >= 230)
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
        "major_color_count": sum(1 for _, count in colors.items() if count >= 32),
    }


def meta_snapshot() -> dict[str, object]:
    meta = TARGET.with_name(TARGET.name + ".meta")
    data = meta.read_bytes() if meta.exists() else b""
    return {
        "exists": meta.exists(),
        "size": meta.stat().st_size if meta.exists() else None,
        "mtime": meta.stat().st_mtime if meta.exists() else None,
        "last_byte": data[-1] if data else None,
    }


def make_preview() -> dict[str, str]:
    sprites = [Image.open(SKINS / filename).convert("RGBA") for filename, _, _ in WAVE_TARGETS]
    ART.mkdir(parents=True, exist_ok=True)

    def crop_visible(sprite: Image.Image) -> Image.Image:
        x0, y0, x1, y1 = visible_bbox(sprite)
        return sprite.crop((x0, y0, x1 + 1, y1 + 1))

    def paste_fit(dst: Image.Image, sprite: Image.Image, box: tuple[int, int, int, int]) -> None:
        x0, y0, x1, y1 = box
        crop = crop_visible(sprite)
        max_w = x1 - x0
        max_h = y1 - y0
        scale = min(max_w / crop.width, max_h / crop.height, 1.0)
        resized = crop.resize((max(1, round(crop.width * scale)), max(1, round(crop.height * scale))), Image.Resampling.LANCZOS)
        px = x0 + (max_w - resized.width) // 2
        py = y1 - resized.height
        dst.alpha_composite(resized, (px, py))

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

    draw.text((44, 26), "Cobalt Pup SPEED Evolution - L3 cleaned", fill=(176, 248, 255), font=title_font)
    for i, (sprite, (_, code, label)) in enumerate(zip(sprites, WAVE_TARGETS)):
        x = 60 + i * 470
        draw.rectangle((x, 88, x + 390, 596), outline=(82, 255, 255, 255), width=2)
        draw.ellipse((x + 80, 162, x + 316, 398), fill=(30, 175, 255, 28))
        paste_fit(preview, sprite, (x + 16, 116, x + 374, 578))
        draw.rectangle((x + 16, 606, x + 374, 686), fill=(3, 10, 18, 230))
        draw.text((x + 58, 616), label, fill=(235, 253, 255), font=label_font)
        draw.text((x + 128, 650), code, fill=(140, 215, 224), font=small_font)
        if i < 2:
            draw.line((x + 410, 342, x + 450, 342), fill=(82, 255, 255, 255), width=4)
            draw.polygon([(x + 450, 342), (x + 436, 332), (x + 436, 352)], fill=(82, 255, 255, 255))
    preview_path = ART / "S1_SPEED_Wave1_L1_L2_L3_preview.png"
    preview.save(preview_path)

    transparent = Image.new("RGBA", (1536, 512), (0, 0, 0, 0))
    for i, sprite in enumerate(sprites):
        transparent.alpha_composite(sprite, (i * 512, 0))
    transparent_path = ART / "S1_SPEED_Wave1_L1_L2_L3_transparent_strip.png"
    transparent.save(transparent_path)

    dark = Image.new("RGBA", (1240, 620), (2, 8, 13, 255))
    draw = ImageDraw.Draw(dark)
    draw.text((36, 24), "Dark background check - L3 clean adult", fill=(176, 248, 255), font=title_font)
    for i, (sprite, (_, code, _)) in enumerate(zip(sprites, WAVE_TARGETS)):
        x = 48 + i * 395
        draw.rectangle((x, 78, x + 360, 540), outline=(40, 170, 220, 255), width=1)
        paste_fit(dark, sprite, (x + 8, 100, x + 352, 528))
        draw.rectangle((x + 76, 552, x + 284, 594), fill=(2, 8, 13, 230))
        draw.text((x + 112, 560), code, fill=(235, 253, 255), font=label_font)
    dark_path = ART / "S1_SPEED_Wave1_dark_readability_check.png"
    dark.save(dark_path)

    return {
        "preview": str(preview_path.relative_to(ROOT)),
        "transparent_strip": str(transparent_path.relative_to(ROOT)),
        "dark_check": str(dark_path.relative_to(ROOT)),
    }


def main() -> None:
    if not SOURCE.exists():
        raise FileNotFoundError(SOURCE)
    if not TARGET.exists():
        raise FileNotFoundError(TARGET)

    ART.mkdir(parents=True, exist_ok=True)
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = ART / f"backup_before_s1_speed_l3_clean_adult_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)
    shutil.copy2(TARGET, backup_dir / TARGET.name)

    source_copy = ART / "Partner_S1_R1_L3_clean_adult_source.png"
    shutil.copy2(SOURCE, source_copy)

    meta_before = meta_snapshot()
    clean = normalize_l3(Image.open(SOURCE))
    clean.save(TARGET)
    clean.save(ART / "Partner_S1_R1_L3_CLEAN_ADULT_WAVE1.png")
    meta_after = meta_snapshot()

    measurements = {filename: measure(SKINS / filename) for filename, _, _ in WAVE_TARGETS}
    previews = make_preview()
    report = {
        "handoff_quality_gate_read": True,
        "scope": "Replace only Partner_S1_R1_L3.png. L1/L2 unchanged. No CoreLanternGame.cs edit. No .meta edit.",
        "reason": "Previous L3 had overlapping afterimage wolf/wing clutter that read as creepy. Clean adult source has one readable body silhouette.",
        "source": str(SOURCE.relative_to(ROOT)),
        "source_copy": str(source_copy.relative_to(ROOT)),
        "backup_dir": str(backup_dir.relative_to(ROOT)),
        "measurements": measurements,
        "previews": previews,
        "meta_before": meta_before,
        "meta_after": meta_after,
        "meta_unchanged": meta_before == meta_after,
    }
    report_path = ART / "s1_speed_wave1_validation_report.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
