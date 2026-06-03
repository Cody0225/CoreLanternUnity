from __future__ import annotations

import json
import shutil
from collections import Counter, deque
from datetime import datetime
from pathlib import Path
from typing import Iterable

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601"
OUT_DIR = ART / "s1_speed_wave1_20260602"

BASE_FILE = SKINS / "Partner_S1_L0.png"
TARGETS = [
    ("Partner_S1_R1_L1.png", 354, "L1", "Aero Pup"),
    ("Partner_S1_R1_L2.png", 387, "L2", "Vector Pup"),
    ("Partner_S1_R1_L3.png", 426, "L3", "Cobalt Strider"),
]

TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8

BLUE_DARK = (7, 21, 52, 255)
BLUE_LINE = (16, 48, 110, 255)
CYAN_OUTLINE = (20, 204, 255, 255)
CYAN = (82, 255, 255, 255)
CYAN_LIGHT = (197, 255, 255, 255)
WHITE = (242, 252, 255, 255)
SHADOW = (6, 10, 22, 255)


def lerp_channel(a: int, b: int, t: float) -> int:
    t = max(0.0, min(1.0, t))
    return int(round(a * (1.0 - t) + b * t))


def lerp_rgba(a: tuple[int, int, int, int], b: tuple[int, int, int, int], t: float) -> tuple[int, int, int, int]:
    return (
        lerp_channel(a[0], b[0], t),
        lerp_channel(a[1], b[1], t),
        lerp_channel(a[2], b[2], t),
        lerp_channel(a[3], b[3], t),
    )


def visible_pixels(img: Image.Image) -> list[tuple[int, int, tuple[int, int, int, int]]]:
    pix = img.load()
    w, h = img.size
    return [(x, y, pix[x, y]) for y in range(h) for x in range(w) if pix[x, y][3] > ALPHA_THRESHOLD]


def bbox_of(img: Image.Image) -> tuple[int, int, int, int]:
    vis = visible_pixels(img)
    if not vis:
        raise RuntimeError("image has no visible pixels")
    xs = [v[0] for v in vis]
    ys = [v[1] for v in vis]
    return min(xs), min(ys), max(xs), max(ys)


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
            comps.append({"count": len(coords), "bbox": (min(xs), min(ys), max(xs), max(ys)), "coords": coords})
    comps.sort(key=lambda c: int(c["count"]), reverse=True)
    return comps


def crop_to_visible(img: Image.Image) -> Image.Image:
    x0, y0, x1, y1 = bbox_of(img)
    return img.crop((x0, y0, x1 + 1, y1 + 1))


def scale_base_to_height(base: Image.Image, target_h: int) -> Image.Image:
    crop = crop_to_visible(base)
    scale = target_h / crop.height
    target_w = max(1, int(round(crop.width * scale)))
    return crop.resize((target_w, target_h), Image.Resampling.NEAREST)


def draw_poly(draw: ImageDraw.ImageDraw, pts: Iterable[tuple[float, float]], fill: tuple[int, int, int, int], outline: tuple[int, int, int, int] = SHADOW, width: int = 4) -> None:
    pts_i = [(int(round(x)), int(round(y))) for x, y in pts]
    draw.polygon(pts_i, fill=outline)
    if width > 0:
        cx = sum(x for x, _ in pts_i) / len(pts_i)
        cy = sum(y for _, y in pts_i) / len(pts_i)
        inner = []
        for x, y in pts_i:
            inner.append((int(round(cx + (x - cx) * 0.88)), int(round(cy + (y - cy) * 0.88))))
        draw.polygon(inner, fill=fill)


def draw_capsule(draw: ImageDraw.ImageDraw, box: tuple[float, float, float, float], fill: tuple[int, int, int, int], outline: tuple[int, int, int, int] = SHADOW, width: int = 3) -> None:
    x0, y0, x1, y1 = [int(round(v)) for v in box]
    draw.rounded_rectangle((x0, y0, x1, y1), radius=max(2, (y1 - y0) // 2), fill=outline)
    draw.rounded_rectangle((x0 + width, y0 + width, x1 - width, y1 - width), radius=max(2, (y1 - y0) // 2 - width), fill=fill)


def draw_ring(draw: ImageDraw.ImageDraw, center: tuple[float, float], r: float, fill: tuple[int, int, int, int] = CYAN, outline: tuple[int, int, int, int] = SHADOW) -> None:
    cx, cy = center
    box = (int(cx - r), int(cy - r), int(cx + r), int(cy + r))
    draw.ellipse(box, fill=outline)
    inset = max(3, int(r * 0.28))
    draw.ellipse((box[0] + inset, box[1] + inset, box[2] - inset, box[3] - inset), fill=fill)
    inset2 = max(inset + 4, int(r * 0.58))
    draw.ellipse((box[0] + inset2, box[1] + inset2, box[2] - inset2, box[3] - inset2), fill=BLUE_DARK)


def paste_nontransparent(dst: Image.Image, src: Image.Image, xy: tuple[int, int]) -> None:
    dst.alpha_composite(src, xy)


def speed_tint_sprite(sprite: Image.Image, level_index: int) -> Image.Image:
    """Bake route color into the Cobalt body while preserving the original L0 shading."""
    out = sprite.copy().convert("RGBA")
    pix = out.load()
    intensity = [0.08, 0.20, 0.36][level_index]
    w, h = out.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                continue
            # Keep white fur and black linework readable; tint blue fur/armor and neutral metal.
            if r > 218 and g > 218 and b > 224:
                continue
            if r < 24 and g < 30 and b < 42:
                continue
            if b >= r and b >= g:
                target = (30, 210, 255, a)
                pix[x, y] = lerp_rgba((r, g, b, a), target, intensity)
            elif abs(r - g) < 35 and abs(g - b) < 45:
                target = (70, 175, 210, a)
                pix[x, y] = lerp_rgba((r, g, b, a), target, intensity * 0.55)
    return out


def make_afterimage(sprite: Image.Image, level_index: int) -> Image.Image:
    """Create baked opaque residual silhouettes for the final SPEED form."""
    out = Image.new("RGBA", sprite.size, (0, 0, 0, 0))
    src = sprite.convert("RGBA")
    src_pix = src.load()
    out_pix = out.load()
    w, h = src.size
    trail_colors = [
        (10, 74, 104, 255),
        (14, 96, 132, 255),
        (16, 116, 148, 255),
    ]
    color = trail_colors[min(level_index, len(trail_colors) - 1)]
    for y in range(h):
        for x in range(w):
            r, g, b, a = src_pix[x, y]
            if a <= ALPHA_THRESHOLD:
                continue
            # Residuals should trail from the body/tail, not duplicate the face and tall ears as a bright blob.
            if y < h * 0.34:
                continue
            if x > w * 0.70 and y < h * 0.64:
                continue
            if r > 220 and g > 220 and b > 225 and y < h * 0.58:
                continue
            out_pix[x, y] = color
    return out


def force_opaque_visible(img: Image.Image) -> Image.Image:
    pix = img.load()
    w, h = img.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = pix[x, y]
            if a > ALPHA_THRESHOLD:
                pix[x, y] = (r, g, b, 255)
            else:
                pix[x, y] = (0, 0, 0, 0)
    return img


def remove_tiny_components(img: Image.Image, min_pixels: int = 32) -> Image.Image:
    comps = connected_components(img)
    if not comps:
        return img
    keep: set[tuple[int, int]] = set()
    for comp in comps:
        if int(comp["count"]) >= min_pixels:
            keep.update(comp["coords"])  # type: ignore[arg-type]
    pix = img.load()
    w, h = img.size
    for y in range(h):
        for x in range(w):
            if pix[x, y][3] > ALPHA_THRESHOLD and (x, y) not in keep:
                pix[x, y] = (0, 0, 0, 0)
    return img


def draw_speed_features(canvas: Image.Image, body_bbox: tuple[int, int, int, int], level_index: int, phase: str) -> None:
    draw = ImageDraw.Draw(canvas)
    x0, y0, x1, y1 = body_bbox
    w = x1 - x0 + 1
    h = y1 - y0 + 1

    def rx(v: float) -> float:
        return x0 + w * v

    def ry(v: float) -> float:
        return y0 + h * v

    if phase == "behind":
        # Attached speed fins: L1 small pair, L2 larger plates, L3 jet-wing silhouette.
        fin_count = [1, 3, 4][level_index]
        for i in range(fin_count):
            y_base = 0.34 + i * (0.10 if level_index < 2 else 0.075)
            length = 0.24 + level_index * 0.11 + i * (0.018 + level_index * 0.012)
            height = 0.11 + level_index * 0.040 + i * 0.006
            attach_x = 0.43 - i * (0.020 if level_index < 2 else 0.035)
            pts = [
                (rx(attach_x), ry(y_base)),
                (rx(attach_x - length), ry(y_base - height)),
                (rx(attach_x - length * 0.55), ry(y_base + height * 0.92)),
            ]
            fill = CYAN if i % 2 == 0 else (34, 180, 255, 255)
            draw_poly(draw, pts, fill=fill, outline=SHADOW, width=5)
            inner = [
                (rx(attach_x - 0.03), ry(y_base + 0.005)),
                (rx(attach_x - length * 0.73), ry(y_base - height * 0.42)),
                (rx(attach_x - length * 0.42), ry(y_base + height * 0.40)),
            ]
            draw_poly(draw, inner, fill=CYAN_LIGHT, outline=CYAN_OUTLINE, width=2)

        if level_index >= 1:
            # Shoulder crest: L2+ gets a clear new protrusion above the back line.
            crest = [
                (rx(0.54), ry(0.31)),
                (rx(0.34 - level_index * 0.06), ry(0.19 - level_index * 0.03)),
                (rx(0.43), ry(0.40)),
            ]
            draw_poly(draw, crest, fill=(24, 165, 242, 255), outline=SHADOW, width=5)
            draw.line((rx(0.49), ry(0.32), rx(0.36 - level_index * 0.05), ry(0.23 - level_index * 0.02)), fill=CYAN_LIGHT, width=3 + level_index)

        if level_index >= 1:
            # Low rear jet rail, connected under the tail/body.
            draw_capsule(draw, (rx(0.03), ry(0.64), rx(0.39), ry(0.70)), fill=(30, 190, 255, 255), outline=SHADOW, width=4)
            draw.line((rx(0.08), ry(0.67), rx(0.35), ry(0.67)), fill=CYAN_LIGHT, width=3)

        if level_index >= 2:
            # L3 has jet-wing blades and a tail thruster fan. These make the final silhouette read instantly.
            for k, off in enumerate([0.0, 0.11, 0.22]):
                blade = [
                    (rx(0.56 - off * 0.35), ry(0.28 + off)),
                    (rx(-0.06 - off * 0.20), ry(0.03 + off * 0.55)),
                    (rx(0.18 - off * 0.10), ry(0.40 + off * 0.70)),
                ]
                draw_poly(draw, blade, fill=(18 + k * 18, 158 + k * 32, 255, 255), outline=SHADOW, width=7)
                inner = [
                    (rx(0.48 - off * 0.25), ry(0.29 + off)),
                    (rx(0.03 - off * 0.10), ry(0.10 + off * 0.54)),
                    (rx(0.22 - off * 0.08), ry(0.35 + off * 0.65)),
                ]
                draw_poly(draw, inner, fill=CYAN_LIGHT, outline=CYAN_OUTLINE, width=2)
            draw_capsule(draw, (rx(-0.04), ry(0.76), rx(0.28), ry(0.83)), fill=(82, 255, 255, 255), outline=SHADOW, width=5)
            draw.line((rx(-0.02), ry(0.80), rx(0.25), ry(0.80)), fill=WHITE, width=3)

    if phase == "front":
        # Forehead speed crest.
        crest_scale = 1.0 + level_index * 0.18
        crest = [
            (rx(0.66), ry(0.18)),
            (rx(0.78 + level_index * 0.03), ry(0.25)),
            (rx(0.64), ry(0.32)),
            (rx(0.55 - level_index * 0.02), ry(0.23)),
        ]
        draw_poly(draw, crest, fill=CYAN, outline=SHADOW, width=max(3, int(4 * crest_scale)))
        draw.line((rx(0.61), ry(0.23), rx(0.72 + level_index * 0.03), ry(0.26)), fill=CYAN_LIGHT, width=3 + level_index)

        # Angular chest speed armor. V-shape keeps the mouth clear and reads as aerodynamic plating.
        left_plate = [
            (rx(0.44), ry(0.61)),
            (rx(0.57), ry(0.58)),
            (rx(0.55), ry(0.67)),
            (rx(0.42), ry(0.69)),
        ]
        right_plate = [
            (rx(0.58), ry(0.58)),
            (rx(0.72 + level_index * 0.03), ry(0.61)),
            (rx(0.70 + level_index * 0.03), ry(0.69)),
            (rx(0.56), ry(0.67)),
        ]
        draw_poly(draw, left_plate, fill=(26, 165, 255, 255), outline=SHADOW, width=4)
        draw_poly(draw, right_plate, fill=(42, 220, 255, 255), outline=SHADOW, width=4)
        draw.line((rx(0.48), ry(0.635), rx(0.66 + level_index * 0.03), ry(0.635)), fill=CYAN_LIGHT, width=2 + level_index)
        draw.line((rx(0.71), ry(0.41), rx(0.86), ry(0.38)), fill=CYAN, width=3 + level_index)
        draw.line((rx(0.73), ry(0.435), rx(0.83), ry(0.415)), fill=CYAN_LIGHT, width=2)

        # Leg boosters.
        draw_ring(draw, (rx(0.34), ry(0.70)), 9 + level_index)
        draw_ring(draw, (rx(0.58), ry(0.72)), 10 + level_index)
        if level_index == 0:
            draw.line((rx(0.36), ry(0.79), rx(0.20), ry(0.76)), fill=CYAN, width=3)
            draw.line((rx(0.58), ry(0.82), rx(0.75), ry(0.78)), fill=CYAN_LIGHT, width=2)
        if level_index >= 1:
            draw_ring(draw, (rx(0.76), ry(0.62)), 9 + level_index)
            draw.line((rx(0.32), ry(0.75), rx(0.18), ry(0.72)), fill=CYAN, width=3 + level_index)
            draw.line((rx(0.65), ry(0.78), rx(0.85), ry(0.74)), fill=CYAN_LIGHT, width=3 + level_index)
            draw_capsule(draw, (rx(0.28), ry(0.87), rx(0.46), ry(0.92)), fill=(35, 205, 255, 255), outline=SHADOW, width=3)
            draw_capsule(draw, (rx(0.61), ry(0.87), rx(0.82), ry(0.92)), fill=(35, 205, 255, 255), outline=SHADOW, width=3)

        if level_index >= 2:
            # Final-stage aerodynamic cheek and tail light rails.
            draw.line((rx(0.68), ry(0.36), rx(0.89), ry(0.32), rx(0.97), ry(0.38)), fill=CYAN, width=5)
            draw.line((rx(0.68), ry(0.39), rx(0.88), ry(0.36)), fill=CYAN_LIGHT, width=2)
            draw.line((rx(0.26), ry(0.54), rx(0.02), ry(0.47)), fill=CYAN, width=5)
            draw.line((rx(0.28), ry(0.58), rx(0.03), ry(0.55)), fill=CYAN_LIGHT, width=3)
            for yy in [0.48, 0.57, 0.68]:
                draw.line((rx(0.78), ry(yy), rx(1.02), ry(yy - 0.04)), fill=CYAN_LIGHT, width=3)


def generate_stage(base: Image.Image, filename: str, target_height: int, level_index: int) -> dict[str, object]:
    scaled = speed_tint_sprite(scale_base_to_height(base, target_height), level_index)
    x_offsets = [0, 8, 28]
    x = int(round((512 - scaled.width) / 2 + x_offsets[level_index]))
    y = TARGET_BOTTOM_Y - scaled.height
    body_bbox = (x, y, x + scaled.width - 1, y + scaled.height - 1)

    canvas = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    if level_index >= 2:
        ghost = make_afterimage(scaled, level_index)
        # Multiple residuals are deliberately overlapped with the body so they are not isolated debris.
        for ox, oy in [(-38, -15), (-20, -6)]:
            paste_nontransparent(canvas, ghost, (x + ox, y + oy))
            draw = ImageDraw.Draw(canvas)
            draw.line((x + 38 + ox, y + scaled.height * 0.48 + oy, x + 158, y + scaled.height * 0.53), fill=(32, 190, 230, 255), width=4)
    draw_speed_features(canvas, body_bbox, level_index, "behind")
    paste_nontransparent(canvas, scaled, (x, y))
    draw_speed_features(canvas, body_bbox, level_index, "front")
    force_opaque_visible(canvas)
    remove_tiny_components(canvas)

    target = SKINS / filename
    canvas.save(target)
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    canvas.save(OUT_DIR / f"{Path(filename).stem}_SPEED_WAVE1.png")
    return measure(target)


def luminance(rgb: tuple[int, int, int]) -> float:
    r, g, b = rgb
    return 0.2126 * r + 0.7152 * g + 0.0722 * b


def measure(path: Path) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    vis = visible_pixels(img)
    bbox = bbox_of(img)
    corners = [img.getpixel((0, 0))[3], img.getpixel((511, 0))[3], img.getpixel((0, 511))[3], img.getpixel((511, 511))[3]]
    center_x = (bbox[0] + bbox[2]) / 2
    center_y = (bbox[1] + bbox[3]) / 2
    candidates = []
    for x, y, rgba in vis:
        r, g, b, a = rgba
        if a < 230:
            continue
        # Representative Cobalt/SPEED body sample, not white fur or black outline.
        if b >= r and (b - r) > 35 and b > 120 and r > 35 and not (r > 200 and g > 200 and b > 220):
            candidates.append((abs(x - center_x) + abs(y - center_y), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - center_x) + abs(y - center_y), x, y, rgba) for x, y, rgba in vis if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda v: v[0])

    luma_values = [luminance(rgba[:3]) for _, _, rgba in vis if rgba[3] >= 230]
    major_colors = sum(1 for _, count in Counter(rgba[:3] for _, _, rgba in vis if rgba[3] >= 230).items() if count >= 64)
    comps = connected_components(img)
    return {
        "file": path.name,
        "size": list(img.size),
        "bbox": list(bbox),
        "bottom_y": bbox[3] + 1,
        "height": bbox[3] - bbox[1] + 1,
        "corner_alpha": corners,
        "component_count": len(comps),
        "largest_component_pixels": int(comps[0]["count"]) if comps else 0,
        "semi_transparent_pixels": sum(1 for _, _, rgba in vis if 0 < rgba[3] < 230),
        "sample_xy": [sx, sy],
        "sample_color": "#{:02X}{:02X}{:02X}".format(*sample[:3]),
        "sample_alpha": int(sample[3]),
        "luma_range": [round(min(luma_values), 2), round(max(luma_values), 2)],
        "major_color_count": major_colors,
    }


def paste_fit(dst: Image.Image, sprite: Image.Image, box: tuple[int, int, int, int]) -> None:
    x0, y0, x1, y1 = box
    crop = crop_to_visible(sprite)
    max_w = x1 - x0
    max_h = y1 - y0
    scale = min(max_w / crop.width, max_h / crop.height)
    resized = crop.resize((max(1, int(crop.width * scale)), max(1, int(crop.height * scale))), Image.Resampling.NEAREST)
    px = x0 + (max_w - resized.width) // 2
    py = y1 - resized.height
    dst.alpha_composite(resized, (px, py))


def paste_uniform_crop(dst: Image.Image, sprite: Image.Image, x_center: int, y_bottom: int, scale: float) -> None:
    crop = crop_to_visible(sprite)
    resized = crop.resize((max(1, int(crop.width * scale)), max(1, int(crop.height * scale))), Image.Resampling.NEAREST)
    px = x_center - resized.width // 2
    py = y_bottom - resized.height
    dst.alpha_composite(resized, (px, py))


def make_preview() -> dict[str, str]:
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    sprites = [Image.open(SKINS / filename).convert("RGBA") for filename, _, _, _ in TARGETS]

    preview = Image.new("RGBA", (1500, 560), (3, 10, 18, 255))
    draw = ImageDraw.Draw(preview)
    try:
        title_font = ImageFont.truetype("arial.ttf", 32)
        label_font = ImageFont.truetype("arial.ttf", 24)
        small_font = ImageFont.truetype("arial.ttf", 18)
    except OSError:
        title_font = ImageFont.load_default()
        label_font = ImageFont.load_default()
        small_font = ImageFont.load_default()

    draw.text((42, 28), "Cobalt Pup SPEED Evolution - Wave 1 Pilot", fill=(176, 248, 255), font=title_font)
    cell_w = 430
    start_x = 72
    for i, (sprite, target) in enumerate(zip(sprites, TARGETS)):
        filename, _, level, codename = target
        x = start_x + i * cell_w
        draw.rectangle((x, 92, x + 330, 488), outline=(82, 255, 255, 255), width=2)
        draw.ellipse((x + 58, 138, x + 266, 346), fill=(30, 175, 255, 35))
        paste_uniform_crop(preview, sprite, x + 165, 418, 0.72)
        draw.text((x + 105, 428), f"{level}  {codename}", fill=(232, 252, 255), font=label_font)
        draw.text((x + 79, 462), filename, fill=(140, 215, 224), font=small_font)
        if i < len(sprites) - 1:
            draw.line((x + 357, 282, x + 405, 282), fill=(82, 255, 255, 255), width=4)
            draw.polygon([(x + 405, 282), (x + 390, 271), (x + 390, 293)], fill=(82, 255, 255, 255))
    preview_path = OUT_DIR / "S1_SPEED_Wave1_L1_L2_L3_preview.png"
    preview.save(preview_path)

    transparent = Image.new("RGBA", (1536, 512), (0, 0, 0, 0))
    for i, sprite in enumerate(sprites):
        transparent.alpha_composite(sprite, (512 * i, 0))
    transparent_path = OUT_DIR / "S1_SPEED_Wave1_L1_L2_L3_transparent_strip.png"
    transparent.save(transparent_path)

    dark_check = Image.new("RGBA", (1240, 500), (2, 8, 13, 255))
    draw = ImageDraw.Draw(dark_check)
    draw.text((36, 24), "In-game dark background check", fill=(176, 248, 255), font=title_font)
    for i, sprite in enumerate(sprites):
        x = 78 + i * 390
        draw.rectangle((x, 82, x + 300, 454), outline=(40, 170, 220, 255), width=1)
        draw.ellipse((x + 82, 132, x + 220, 270), fill=(82, 255, 255, 28))
        paste_uniform_crop(dark_check, sprite, x + 150, 400, 0.70)
        draw.text((x + 105, 410), TARGETS[i][2], fill=(232, 252, 255), font=label_font)
    dark_path = OUT_DIR / "S1_SPEED_Wave1_dark_readability_check.png"
    dark_check.save(dark_path)

    return {
        "preview": str(preview_path.relative_to(ROOT)),
        "transparent_strip": str(transparent_path.relative_to(ROOT)),
        "dark_check": str(dark_path.relative_to(ROOT)),
    }


def backup_targets() -> Path:
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = OUT_DIR / f"backup_before_s1_speed_wave1_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)
    for filename, _, _, _ in TARGETS:
        src = SKINS / filename
        if src.exists():
            shutil.copy2(src, backup_dir / filename)
    return backup_dir


def meta_snapshot() -> dict[str, dict[str, object]]:
    snap: dict[str, dict[str, object]] = {}
    for filename, _, _, _ in TARGETS:
        meta = SKINS / f"{filename}.meta"
        if not meta.exists():
            continue
        data = meta.read_bytes()
        snap[meta.name] = {
            "mtime": meta.stat().st_mtime,
            "size": meta.stat().st_size,
            "last_byte": data[-1] if data else None,
        }
    return snap


def main() -> None:
    if not BASE_FILE.exists():
        raise FileNotFoundError(BASE_FILE)
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    backup_dir = backup_targets()
    meta_before = meta_snapshot()
    base = Image.open(BASE_FILE).convert("RGBA")

    measurements = {}
    for index, (filename, target_h, level, codename) in enumerate(TARGETS):
        measurements[filename] = generate_stage(base, filename, target_h, index)

    previews = make_preview()
    meta_after = meta_snapshot()
    report = {
        "wave": 1,
        "handoff_quality_gate_read": True,
        "method": "direct transparent RGBA composition from new Partner_S1_L0 with baked SPEED fins/armor; no chromakey; PNG overwrite only",
        "base": str(BASE_FILE.relative_to(ROOT)),
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
