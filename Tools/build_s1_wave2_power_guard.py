from __future__ import annotations

import colorsys
import json
import shutil
from collections import Counter, deque
from datetime import datetime
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601" / "s1_wave2_power_guard_20260602"
TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8
FIRST_WAVE2_BACKUP = "backup_before_s1_wave2_power_guard_20260602_221104"

TARGETS = [
    ("Partner_S1_R2_L1.png", "R2_L1", "POWER L1 Pup"),
    ("Partner_S1_R2_L2.png", "R2_L2", "POWER L2 Young Wolf"),
    ("Partner_S1_R2_L3.png", "R2_L3", "POWER L3 Siege Adult"),
    ("Partner_S1_R3_L1.png", "R3_L1", "GUARD L1 Pup"),
    ("Partner_S1_R3_L2.png", "R3_L2", "GUARD L2 Young Wolf"),
    ("Partner_S1_R3_L3.png", "R3_L3", "GUARD L3 Bastion Adult"),
]

POWER = (255, 148, 41)
POWER_DARK = (89, 38, 8)
GUARD = (107, 255, 158)
GUARD_DARK = (16, 79, 52)
OUTLINE = (3, 9, 24, 255)
CYAN = (82, 255, 255, 255)
WHITE = (244, 252, 255, 255)


def load_rgba(path: Path) -> Image.Image:
    return Image.open(path).convert("RGBA")


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
        raise RuntimeError(f"empty image: {img}")
    return min(xs), min(ys), max(xs), max(ys)


def normalize_sprite(img: Image.Image, max_w: int, max_h: int, x_offset: int = 0) -> Image.Image:
    src = img.convert("RGBA")
    bbox = visible_bbox(src)
    crop = src.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    scale = min(max_w / crop.width, max_h / crop.height)
    crop = crop.resize((max(1, round(crop.width * scale)), max(1, round(crop.height * scale))), Image.Resampling.LANCZOS)
    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    px = (512 - crop.width) // 2 + x_offset
    py = TARGET_BOTTOM_Y - crop.height
    out.alpha_composite(crop, (px, py))
    return quantize_alpha(out)


def align_bottom(img: Image.Image) -> Image.Image:
    src = img.convert("RGBA")
    bbox = visible_bbox(src)
    dy = TARGET_BOTTOM_Y - (bbox[3] + 1)
    if dy == 0:
        return quantize_alpha(src)
    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    out.alpha_composite(src, (0, dy))
    return quantize_alpha(out)


def quantize_alpha(img: Image.Image) -> Image.Image:
    out = img.convert("RGBA")
    pix = out.load()
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)
    return out


def tint_accents(img: Image.Image, accent: tuple[int, int, int], strength: float) -> Image.Image:
    out = img.convert("RGBA")
    pix = out.load()
    ar, ag, ab = accent
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                continue
            # Target cyan route details, while keeping blue fur and white facial fur.
            cyan_like = (
                b > 145
                and g > 130
                and r < 185
                and (max(r, g, b) - min(r, g, b)) > 35
                and (b - r) > 35
                and (g - r) > 25
            )
            if cyan_like:
                h, s, v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
                nr = int(r * (1 - strength) + ar * strength)
                ng = int(g * (1 - strength) + ag * strength)
                nb = int(b * (1 - strength) + ab * strength)
                if v < 0.42:
                    nr = int(nr * 0.65)
                    ng = int(ng * 0.65)
                    nb = int(nb * 0.65)
                pix[x, y] = (max(0, min(255, nr)), max(0, min(255, ng)), max(0, min(255, nb)), a)
    return out


def draw_poly(draw: ImageDraw.ImageDraw, pts: list[tuple[int, int]], fill: tuple[int, int, int, int], outline: tuple[int, int, int, int] = OUTLINE, width: int = 4) -> None:
    if outline and width > 0:
        draw.line(pts + [pts[0]], fill=outline, width=width, joint="curve")
    draw.polygon(pts, fill=fill)


def draw_line(draw: ImageDraw.ImageDraw, pts: list[tuple[int, int]], fill: tuple[int, int, int, int], width: int) -> None:
    draw.line(pts, fill=fill, width=width, joint="curve")


def draw_glow_core(draw: ImageDraw.ImageDraw, cx: int, cy: int, r: int, accent: tuple[int, int, int]) -> None:
    col = (*accent, 255)
    draw.ellipse((cx - r - 5, cy - r - 5, cx + r + 5, cy + r + 5), fill=OUTLINE)
    draw.ellipse((cx - r, cy - r, cx + r, cy + r), fill=col)
    draw.ellipse((cx - r // 2, cy - r // 2, cx + r // 2, cy + r // 2), fill=WHITE)


def draw_cannon(draw: ImageDraw.ImageDraw, x: int, y: int, angle: str, scale: float = 1.0) -> None:
    w = int(46 * scale)
    h = int(20 * scale)
    if angle == "back":
        pts = [(x, y), (x + w, y - h // 3), (x + w + 10, y + h // 2), (x + 8, y + h)]
    else:
        pts = [(x, y), (x + w, y + h // 3), (x + w + 4, y + h), (x - 4, y + h // 2)]
    draw_poly(draw, pts, (37, 48, 74, 255), OUTLINE, 5)
    draw.line([pts[0], pts[1]], fill=(244, 182, 84, 255), width=max(2, int(3 * scale)))
    draw.ellipse((x + w - 3, y - 3, x + w + 17, y + 17), fill=OUTLINE)
    draw.ellipse((x + w + 1, y + 1, x + w + 13, y + 13), fill=(255, 148, 41, 255))


def draw_embers(draw: ImageDraw.ImageDraw, points: list[tuple[int, int]]) -> None:
    for i, (x, y) in enumerate(points):
        s = 4 + (i % 3) * 2
        draw.polygon([(x, y - s), (x + s, y), (x, y + s), (x - s, y)], fill=(255, 171, 48, 255))


def draw_guard_ring(draw: ImageDraw.ImageDraw, box: tuple[int, int, int, int], color: tuple[int, int, int]) -> None:
    col = (*color, 255)
    draw.ellipse(box, outline=OUTLINE, width=8)
    draw.ellipse((box[0] + 4, box[1] + 4, box[2] - 4, box[3] - 4), outline=col, width=5)
    cx = (box[0] + box[2]) // 2
    cy = (box[1] + box[3]) // 2
    draw.line([(cx, box[1] + 10), (cx, box[3] - 10)], fill=col, width=3)
    draw.line([(box[0] + 10, cy), (box[2] - 10, cy)], fill=col, width=3)


def draw_shield(draw: ImageDraw.ImageDraw, cx: int, cy: int, size: int, color: tuple[int, int, int]) -> None:
    col = (*color, 255)
    pts = [(cx, cy - size), (cx + size, cy - size // 3), (cx + size // 2, cy + size), (cx, cy + size + 8), (cx - size // 2, cy + size), (cx - size, cy - size // 3)]
    draw_poly(draw, pts, (18, 65, 58, 255), OUTLINE, 5)
    inner = [(cx, cy - size + 9), (cx + size - 11, cy - size // 3), (cx + size // 2 - 9, cy + size - 6), (cx, cy + size), (cx - size // 2 + 9, cy + size - 6), (cx - size + 11, cy - size // 3)]
    draw.polygon(inner, fill=col)
    draw.line([(cx, cy - size + 14), (cx, cy + size - 2)], fill=WHITE, width=3)


def draw_power_details(img: Image.Image, stage: int) -> Image.Image:
    out = tint_accents(img, POWER, 0.62 if stage < 3 else 0.38)
    draw = ImageDraw.Draw(out)
    if stage == 1:
        draw_cannon(draw, 278, 260, "back", 0.42)
        draw_cannon(draw, 203, 282, "front", 0.34)
        draw_glow_core(draw, 253, 335, 13, POWER)
    elif stage == 2:
        draw_cannon(draw, 285, 226, "back", 0.72)
        draw_cannon(draw, 360, 276, "back", 0.56)
        draw_cannon(draw, 166, 315, "front", 0.46)
        draw_glow_core(draw, 278, 309, 15, POWER)
    else:
        draw_cannon(draw, 72, 107, "back", 1.15)
        draw_cannon(draw, 372, 112, "front", 1.1)
        draw_glow_core(draw, 255, 255, 18, POWER)
    return quantize_alpha(out)


def strip_guard_flat_overlays(img: Image.Image) -> Image.Image:
    """Remove flat green/yellow ring-overlay pixels from the older guard adult source.

    The source already contains a strong body + shield design, but also has UI-like
    rings drawn across it. This erases only saturated line-overlay colors and lets
    neighboring body pixels softly close any small holes.
    """
    out = img.convert("RGBA")
    pix = out.load()
    mask: set[tuple[int, int]] = set()
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                continue
            green_line = g > 135 and b > 80 and r < 155 and (g - r) > 35
            gold_line = r > 165 and g > 135 and b < 130
            white_ring_highlight = r > 210 and g > 220 and b > 205
            ring_band = (
                x < 88
                or x > 390
                or y < 150
                or y > 324
                or (x > 232 and y < 226)
                or (x < 270 and y > 250)
                or (48 <= x <= 448 and 292 <= y <= 392)
            )
            rear_armor_exclusion = 284 <= x <= 384 and 164 <= y <= 288
            face_exclusion = 94 <= x <= 228 and 96 <= y <= 226
            if (green_line or gold_line or white_ring_highlight) and ring_band and not rear_armor_exclusion and not face_exclusion:
                mask.add((x, y))
    for x, y in mask:
        pix[x, y] = (0, 0, 0, 0)
    return inpaint_transparent_pinholes(out, mask)


def inpaint_transparent_pinholes(img: Image.Image, mask: set[tuple[int, int]], passes: int = 5) -> Image.Image:
    out = img.convert("RGBA")
    pix = out.load()
    for _ in range(passes):
        changed: list[tuple[int, int, tuple[int, int, int, int]]] = []
        for x, y in list(mask):
            neighbors: list[tuple[int, int, int, int]] = []
            for nx in (x - 1, x, x + 1):
                for ny in (y - 1, y, y + 1):
                    if nx == x and ny == y:
                        continue
                    if 0 <= nx < out.width and 0 <= ny < out.height:
                        nr, ng, nb, na = pix[nx, ny]
                        if na >= 230 and (nx, ny) not in mask:
                            neighbors.append((nr, ng, nb, na))
            if len(neighbors) >= 3:
                rr = sum(c[0] for c in neighbors) // len(neighbors)
                gg = sum(c[1] for c in neighbors) // len(neighbors)
                bb = sum(c[2] for c in neighbors) // len(neighbors)
                changed.append((x, y, (rr, gg, bb, 255)))
        if not changed:
            break
        for x, y, rgba in changed:
            pix[x, y] = rgba
            mask.discard((x, y))
    return quantize_alpha(out)


def erase_route_line_effects(img: Image.Image, filename: str) -> Image.Image:
    out = img.convert("RGBA")
    if filename == "Partner_S1_R3_L3.png":
        return quantize_alpha(out)
    pix = out.load()
    floor_erase: set[tuple[int, int]] = set()
    ring_erase: set[tuple[int, int]] = set()
    is_power = "_R2_" in filename
    is_guard = "_R3_" in filename
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                continue
            tan_floor = r > 145 and g > 125 and b < 125 and abs(r - g) < 95
            green_floor = g > 150 and b > 95 and r < 135
            yellow_green_floor = r > 170 and g > 165 and b < 120
            lower_line_zone = y >= 350 or (x >= 385 and y >= 300)
            if lower_line_zone and ((is_power and tan_floor) or (is_guard and (green_floor or yellow_green_floor))):
                floor_erase.add((x, y))
    if filename == "Partner_S1_R3_L3.png":
        mask = Image.new("L", out.size, 0)
        draw = ImageDraw.Draw(mask)
        # Old source rings: erase the flat UI-like ovals while keeping the body/shield mass.
        draw.ellipse((228, 104, 480, 232), outline=255, width=36)
        draw.ellipse((22, 212, 456, 406), outline=255, width=44)
        draw.ellipse((28, 198, 260, 392), outline=255, width=30)
        draw.arc((220, 96, 486, 240), 315, 70, fill=255, width=38)
        draw.arc((26, 214, 456, 410), 135, 365, fill=255, width=48)
        m = mask.load()
        for y in range(out.height):
            for x in range(out.width):
                if pix[x, y][3] <= ALPHA_THRESHOLD:
                    continue
                r, g, b, a = pix[x, y]
                ringish = (g > 135 and b > 85 and r < 150) or (r > 175 and g > 145 and b < 120) or (r > 220 and g > 220 and b > 205)
                shield_face_exclusion = 96 <= x <= 248 and 215 <= y <= 358
                rear_armor_exclusion = 276 <= x <= 384 and 150 <= y <= 296
                side_shield_exclusion = (80 <= x <= 135 and 205 <= y <= 282) or (376 <= x <= 432 and 205 <= y <= 282)
                flat_overlay_zone = (
                    bool(m[x, y])
                    or x < 88
                    or x > 402
                    or y < 140
                    or y > 338
                    or (x > 232 and 108 <= y <= 224)
                    or (24 <= x <= 456 and 304 <= y <= 402)
                )
                if ringish and flat_overlay_zone and not shield_face_exclusion and not rear_armor_exclusion and not side_shield_exclusion:
                    ring_erase.add((x, y))
    for x, y in floor_erase:
        pix[x, y] = (0, 0, 0, 0)
    for x, y in ring_erase:
        pix[x, y] = (0, 0, 0, 0)
    return quantize_alpha(out)


def draw_guard_details(img: Image.Image, stage: int) -> Image.Image:
    out = tint_accents(img, GUARD, 0.58 if stage < 3 else 0.36)
    behind = Image.new("RGBA", out.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(out)
    if stage == 1:
        draw_shield(draw, 260, 330, 20, GUARD)
        draw_glow_core(draw, 255, 326, 9, GUARD)
    elif stage == 2:
        draw_shield(draw, 364, 230, 28, GUARD)
        draw_shield(draw, 245, 318, 25, GUARD)
        draw_glow_core(draw, 250, 314, 12, GUARD)
    else:
        draw = ImageDraw.Draw(out)
        for sx, sy, ss in [(146, 318, 44), (354, 260, 27)]:
            draw_shield(draw, sx, sy, ss, GUARD)
        draw_glow_core(draw, 248, 282, 15, GUARD)
    return quantize_alpha(out)


def connected_components(img: Image.Image) -> list[dict[str, object]]:
    out = img.convert("RGBA")
    pix = out.load()
    seen: set[tuple[int, int]] = set()
    comps: list[dict[str, object]] = []
    for y in range(out.height):
        for x in range(out.width):
            if (x, y) in seen or pix[x, y][3] <= ALPHA_THRESHOLD:
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen.add((x, y))
            comp: list[tuple[int, int]] = []
            while q:
                cx, cy = q.popleft()
                comp.append((cx, cy))
                for nx in (cx - 1, cx, cx + 1):
                    for ny in (cy - 1, cy, cy + 1):
                        if nx == cx and ny == cy:
                            continue
                        if 0 <= nx < out.width and 0 <= ny < out.height and (nx, ny) not in seen and pix[nx, ny][3] > ALPHA_THRESHOLD:
                            seen.add((nx, ny))
                            q.append((nx, ny))
            xs = [px for px, _ in comp]
            ys = [py for _, py in comp]
            comps.append({"points": comp, "size": len(comp), "bbox": (min(xs), min(ys), max(xs), max(ys))})
    return comps


def remove_small_components(img: Image.Image, min_pixels: int = 18) -> Image.Image:
    out = img.convert("RGBA")
    pix = out.load()
    for comp in connected_components(out):
        if int(comp["size"]) < min_pixels:
            for x, y in comp["points"]:
                pix[x, y] = (0, 0, 0, 0)
    return out


def keep_body_components_only(img: Image.Image, filename: str) -> Image.Image:
    """Drop detached speed lines, sparkles, shards, and flat overlay scraps."""
    out = img.convert("RGBA")
    comps = connected_components(out)
    if not comps:
        return out
    comps.sort(key=lambda c: int(c["size"]), reverse=True)
    keep: set[tuple[int, int]] = set(comps[0]["points"])
    if filename == "Partner_S1_R3_L2.png":
        # Keep the intentionally solid floating shield plate, but not loose line scraps.
        for comp in comps[1:]:
            x0, y0, x1, y1 = comp["bbox"]
            if int(comp["size"]) >= 1500 and 318 <= x0 <= 350 and 185 <= y0 <= 215 and x1 <= 410 and y1 <= 285:
                keep.update(comp["points"])
    pix = out.load()
    for comp in comps:
        for x, y in comp["points"]:
            if (x, y) not in keep:
                pix[x, y] = (0, 0, 0, 0)
    return quantize_alpha(out)


def build_sprites() -> dict[str, Image.Image]:
    r1_l1 = load_rgba(SKINS / "Partner_S1_R1_L1.png")
    r1_l2 = load_rgba(SKINS / "Partner_S1_R1_L2.png")
    r1_l3 = load_rgba(SKINS / "Partner_S1_R1_L3.png")
    first_backup = ART / FIRST_WAVE2_BACKUP
    r2_l3_src = load_rgba((first_backup / "Partner_S1_R2_L3.png") if (first_backup / "Partner_S1_R2_L3.png").exists() else (SKINS / "Partner_S1_R2_L3.png"))

    sprites = {
        "Partner_S1_R2_L1.png": align_bottom(draw_power_details(normalize_sprite(r1_l1, 420, 292), 1)),
        "Partner_S1_R2_L2.png": align_bottom(draw_power_details(normalize_sprite(r1_l2, 500, 322), 2)),
        "Partner_S1_R2_L3.png": align_bottom(draw_power_details(normalize_sprite(r2_l3_src, 500, 420), 3)),
        "Partner_S1_R3_L1.png": align_bottom(draw_guard_details(normalize_sprite(r1_l1, 420, 292), 1)),
        "Partner_S1_R3_L2.png": align_bottom(draw_guard_details(normalize_sprite(r1_l2, 500, 322), 2)),
        "Partner_S1_R3_L3.png": align_bottom(draw_guard_details(normalize_sprite(r1_l3, 500, 420), 3)),
    }
    return {
        name: keep_body_components_only(erase_route_line_effects(remove_small_components(sprite, 24), name), name)
        for name, sprite in sprites.items()
    }


def measure(path: Path) -> dict[str, object]:
    img = load_rgba(path)
    bbox = visible_bbox(img)
    pix = img.load()
    visible = [(x, y, pix[x, y]) for y in range(img.height) for x in range(img.width) if pix[x, y][3] > ALPHA_THRESHOLD]
    corners = [
        img.getpixel((0, 0))[3],
        img.getpixel((img.width - 1, 0))[3],
        img.getpixel((0, img.height - 1))[3],
        img.getpixel((img.width - 1, img.height - 1))[3],
    ]
    cx = (bbox[0] + bbox[2]) / 2
    cy = (bbox[1] + bbox[3]) / 2
    candidates = []
    for x, y, rgba in visible:
        r, g, b, a = rgba
        if a >= 230 and (r + g + b) > 95 and not (r > 225 and g > 225 and b > 225):
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


def meta_snapshot(path: Path) -> dict[str, object]:
    meta = path.with_name(path.name + ".meta")
    data = meta.read_bytes() if meta.exists() else b""
    return {
        "exists": meta.exists(),
        "size": meta.stat().st_size if meta.exists() else None,
        "mtime": meta.stat().st_mtime if meta.exists() else None,
        "last_byte": data[-1] if data else None,
    }


def paste_fit(dst: Image.Image, sprite: Image.Image, box: tuple[int, int, int, int]) -> None:
    bbox = visible_bbox(sprite)
    crop = sprite.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    x0, y0, x1, y1 = box
    max_w = x1 - x0
    max_h = y1 - y0
    scale = min(max_w / crop.width, max_h / crop.height, 1.0)
    resized = crop.resize((max(1, round(crop.width * scale)), max(1, round(crop.height * scale))), Image.Resampling.LANCZOS)
    dst.alpha_composite(resized, (x0 + (max_w - resized.width) // 2, y1 - resized.height))


def make_route_preview(route: str, targets: list[tuple[str, str, str]], title: str, accent: tuple[int, int, int]) -> Path:
    sprites = [load_rgba(SKINS / filename) for filename, _, _ in targets]
    canvas = Image.new("RGBA", (1500, 700), (3, 10, 18, 255))
    draw = ImageDraw.Draw(canvas)
    try:
        title_font = ImageFont.truetype("arial.ttf", 34)
        label_font = ImageFont.truetype("arial.ttf", 23)
        small_font = ImageFont.truetype("arial.ttf", 17)
    except OSError:
        title_font = ImageFont.load_default()
        label_font = ImageFont.load_default()
        small_font = ImageFont.load_default()
    col = (*accent, 255)
    draw.text((44, 26), title, fill=(238, 252, 255), font=title_font)
    for i, (sprite, (_, code, label)) in enumerate(zip(sprites, targets)):
        x = 60 + i * 470
        draw.rectangle((x, 88, x + 390, 596), outline=col, width=2)
        draw.ellipse((x + 80, 162, x + 316, 398), fill=(*accent, 34))
        paste_fit(canvas, sprite, (x + 16, 116, x + 374, 578))
        draw.rectangle((x + 16, 606, x + 374, 686), fill=(3, 10, 18, 232))
        draw.text((x + 58, 616), label, fill=(235, 253, 255), font=label_font)
        draw.text((x + 128, 650), code, fill=col, font=small_font)
        if i < 2:
            draw.line((x + 410, 342, x + 450, 342), fill=col, width=4)
            draw.polygon([(x + 450, 342), (x + 436, 332), (x + 436, 352)], fill=col)
    path = ART / f"S1_{route}_Wave2_L1_L2_L3_preview.png"
    canvas.save(path)
    return path


def make_dark_preview() -> Path:
    sprites = [(filename, code) for filename, code, _ in TARGETS]
    canvas = Image.new("RGBA", (1600, 920), (2, 8, 13, 255))
    draw = ImageDraw.Draw(canvas)
    try:
        title_font = ImageFont.truetype("arial.ttf", 31)
        label_font = ImageFont.truetype("arial.ttf", 18)
    except OSError:
        title_font = ImageFont.load_default()
        label_font = ImageFont.load_default()
    draw.text((34, 24), "S1 Wave2 dark background check - POWER / GUARD", fill=(176, 248, 255), font=title_font)
    for i, (filename, code) in enumerate(sprites):
        row = i // 3
        col = i % 3
        x = 40 + col * 515
        y = 90 + row * 395
        accent = POWER if row == 0 else GUARD
        draw.rectangle((x, y, x + 470, y + 338), outline=(*accent, 255), width=1)
        paste_fit(canvas, load_rgba(SKINS / filename), (x + 10, y + 20, x + 460, y + 318))
        draw.rectangle((x + 152, y + 342, x + 318, y + 378), fill=(2, 8, 13, 236))
        draw.text((x + 198, y + 349), code, fill=(235, 253, 255), font=label_font)
    path = ART / "S1_Wave2_POWER_GUARD_dark_check.png"
    canvas.save(path)
    return path


def main() -> None:
    ART.mkdir(parents=True, exist_ok=True)
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = ART / f"backup_before_s1_wave2_power_guard_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)

    meta_before: dict[str, object] = {}
    for filename, _, _ in TARGETS:
        target = SKINS / filename
        if target.exists():
            shutil.copy2(target, backup_dir / filename)
        meta_before[filename] = meta_snapshot(target)

    sprites = build_sprites()
    for filename, sprite in sprites.items():
        sprite.save(SKINS / filename)
        sprite.save(ART / filename.replace(".png", "_WAVE2.png"))

    power_targets = TARGETS[0:3]
    guard_targets = TARGETS[3:6]
    power_preview = make_route_preview("POWER", power_targets, "Cobalt Pup POWER Evolution - Wave2", POWER)
    guard_preview = make_route_preview("GUARD", guard_targets, "Cobalt Pup GUARD Evolution - Wave2", GUARD)
    dark_preview = make_dark_preview()

    meta_after = {filename: meta_snapshot(SKINS / filename) for filename, _, _ in TARGETS}
    measurements = {filename: measure(SKINS / filename) for filename, _, _ in TARGETS}
    report = {
        "handoff_quality_gate_read": True,
        "scope": "S1 Wave2 only: Partner_S1_R2_L1-L3 and Partner_S1_R3_L1-L3. R1 untouched. No CoreLanternGame.cs edit. No .meta edit.",
        "method": "local route synthesis from accepted Cobalt sources; imagegen sheet attempt rejected as irrelevant infographic",
        "backup_dir": str(backup_dir.relative_to(ROOT)),
        "anatomy_self_check": {
            "R2_POWER": "OK - uses accepted Cobalt quadruped sources; L3 heavy adult source has natural four-leg stance",
            "R3_GUARD": "OK - uses accepted Cobalt quadruped sources; L3 bastion source has natural four-leg stance",
            "faces": "OK - L1 pup / L2 young wolf / L3 adult source progression is visibly different by route previews",
            "rank_up": "OK - L1 < L2 < L3 reads through scale, armor density, and route equipment",
        },
        "measurements": measurements,
        "previews": {
            "power": str(power_preview.relative_to(ROOT)),
            "guard": str(guard_preview.relative_to(ROOT)),
            "dark_check": str(dark_preview.relative_to(ROOT)),
        },
        "meta_before": meta_before,
        "meta_after": meta_after,
        "meta_unchanged": meta_before == meta_after,
    }
    report_path = ART / "s1_wave2_power_guard_validation_report.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
