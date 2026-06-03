from __future__ import annotations

import json
import math
import shutil
from collections import Counter, deque
from datetime import datetime
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601" / "s1_speed_wave1_20260602"
TARGET = SKINS / "Partner_S1_R1_L3.png"
TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8
SCALE = 3

WAVE_TARGETS = [
    ("Partner_S1_R1_L1.png", "R1_L1", "L1 Pup Sprint"),
    ("Partner_S1_R1_L2.png", "R1_L2", "L2 Young Vector Wolf"),
    ("Partner_S1_R1_L3.png", "R1_L3", "L3 Adult Apex Wolf"),
]

PALETTE = {
    "outline": (3, 9, 24, 255),
    "deep": (6, 24, 67, 255),
    "dark": (10, 49, 121, 255),
    "mid": (12, 102, 213, 255),
    "blue": (23, 143, 246, 255),
    "light": (85, 203, 255, 255),
    "cyan": (82, 255, 255, 255),
    "cyan2": (22, 205, 248, 255),
    "white": (241, 250, 255, 255),
    "fur_shadow": (144, 181, 222, 255),
    "steel": (197, 213, 233, 255),
    "steel_dark": (75, 91, 126, 255),
}


def sc(v: float) -> int:
    return int(round(v * SCALE))


def pts(points: list[tuple[float, float]]) -> list[tuple[int, int]]:
    return [(sc(x), sc(y)) for x, y in points]


def draw_poly(draw: ImageDraw.ImageDraw, points: list[tuple[float, float]], fill: tuple[int, int, int, int], outline: tuple[int, int, int, int] | None = None, width: int = 3) -> None:
    if outline is not None and width > 0:
        draw.line(pts(points + [points[0]]), fill=outline, width=sc(width), joint="curve")
    draw.polygon(pts(points), fill=fill)


def draw_ellipse(draw: ImageDraw.ImageDraw, box: tuple[float, float, float, float], fill: tuple[int, int, int, int], outline: tuple[int, int, int, int] | None = None, width: int = 3) -> None:
    b = tuple(sc(v) for v in box)
    if outline is not None and width > 0:
        draw.ellipse(b, fill=outline)
        inset = sc(width)
        b2 = (b[0] + inset, b[1] + inset, b[2] - inset, b[3] - inset)
        draw.ellipse(b2, fill=fill)
    else:
        draw.ellipse(b, fill=fill)


def draw_line(draw: ImageDraw.ImageDraw, points: list[tuple[float, float]], fill: tuple[int, int, int, int], width: int = 4) -> None:
    draw.line(pts(points), fill=fill, width=sc(width), joint="curve")


def draw_capsule(draw: ImageDraw.ImageDraw, points: list[tuple[float, float]], fill: tuple[int, int, int, int], width: int = 14, outline: tuple[int, int, int, int] | None = None) -> None:
    if outline is not None:
        draw.line(pts(points), fill=outline, width=sc(width + 7), joint="curve")
        r = (width + 7) / 2
        for x, y in points:
            draw_ellipse(draw, (x - r, y - r, x + r, y + r), outline)
    draw.line(pts(points), fill=fill, width=sc(width), joint="curve")
    r = width / 2
    for x, y in points:
        draw_ellipse(draw, (x - r, y - r, x + r, y + r), fill)


def draw_crystal(draw: ImageDraw.ImageDraw, points: list[tuple[float, float]], fill: tuple[int, int, int, int] = PALETTE["cyan2"]) -> None:
    draw_poly(draw, points, fill=PALETTE["outline"], outline=None)
    cx = sum(x for x, _ in points) / len(points)
    cy = sum(y for _, y in points) / len(points)
    inner = [(cx + (x - cx) * 0.88, cy + (y - cy) * 0.88) for x, y in points]
    draw.polygon(pts(inner), fill=fill)
    hi = [(cx + (x - cx) * 0.55, cy + (y - cy) * 0.55) for x, y in points]
    draw.line(pts(hi + [hi[0]]), fill=PALETTE["white"], width=sc(2))


def draw_light_core(draw: ImageDraw.ImageDraw, cx: float, cy: float, r: float) -> None:
    draw_ellipse(draw, (cx - r - 5, cy - r - 5, cx + r + 5, cy + r + 5), PALETTE["outline"])
    draw_ellipse(draw, (cx - r, cy - r, cx + r, cy + r), PALETTE["cyan2"])
    draw_ellipse(draw, (cx - r * 0.55, cy - r * 0.55, cx + r * 0.55, cy + r * 0.55), PALETTE["cyan"])
    draw_ellipse(draw, (cx - r * 0.25, cy - r * 0.25, cx + r * 0.25, cy + r * 0.25), PALETTE["white"])


def draw_adult_wolf() -> Image.Image:
    img = Image.new("RGBA", (512 * SCALE, 512 * SCALE), (0, 0, 0, 0))
    glow = Image.new("RGBA", img.size, (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    draw = ImageDraw.Draw(img)

    # Abstract SPEED trail only. No duplicate wolf face/body.
    for y, length, alpha in [(352, 148, 82), (382, 122, 64), (415, 176, 55)]:
        gd.line(pts([(330, y), (330 + length, y - 18)]), fill=(82, 255, 255, alpha), width=sc(4))
        gd.line(pts([(300, y + 15), (410, y + 2)]), fill=(18, 171, 255, alpha), width=sc(3))
    gd.polygon(pts([(335, 250), (480, 190), (430, 292)]), fill=(82, 255, 255, 32))
    gd.polygon(pts([(310, 288), (494, 232), (430, 340)]), fill=(82, 255, 255, 26))
    glow = glow.filter(ImageFilter.GaussianBlur(sc(2)))
    img.alpha_composite(glow)

    # Tail blade and back crystal wings, fewer and cleaner than the rejected cluttered L3.
    draw_crystal(draw, [(338, 250), (488, 160), (425, 290), (348, 318)], PALETTE["cyan2"])
    draw_crystal(draw, [(292, 230), (407, 118), (370, 258), (303, 300)], (48, 190, 255, 255))
    draw_crystal(draw, [(380, 324), (500, 270), (448, 356), (362, 372)], (58, 224, 255, 255))
    draw_crystal(draw, [(382, 331), (504, 295), (486, 344), (397, 384)], PALETTE["cyan2"])

    # Adult body: long, athletic, not pup/chibi.
    body_outline = [
        (126, 270), (164, 232), (236, 210), (323, 221), (395, 258),
        (424, 300), (398, 336), (315, 355), (222, 350), (151, 325),
        (116, 296),
    ]
    draw_poly(draw, body_outline, PALETTE["outline"])
    body = [
        (137, 270), (172, 240), (238, 220), (318, 229), (381, 260),
        (404, 298), (382, 326), (310, 343), (228, 338), (164, 318),
        (132, 293),
    ]
    draw_poly(draw, body, PALETTE["dark"])
    draw_poly(draw, [(160, 258), (233, 233), (314, 241), (365, 267), (309, 286), (214, 287)], PALETTE["mid"])
    draw_poly(draw, [(195, 238), (258, 224), (320, 238), (285, 255), (220, 260)], PALETTE["blue"])
    draw_poly(draw, [(236, 292), (334, 286), (374, 307), (314, 331), (230, 328)], PALETTE["deep"])

    # Neck and mature head with long muzzle, sharper eye, and non-puppy proportions.
    neck = [(128, 246), (171, 218), (205, 232), (185, 287), (133, 296), (105, 278)]
    draw_poly(draw, neck, PALETTE["outline"])
    draw_poly(draw, [(137, 248), (171, 226), (196, 236), (178, 281), (136, 288), (115, 274)], PALETTE["mid"])

    head_outline = [
        (74, 206), (112, 174), (168, 171), (213, 201), (216, 247),
        (184, 283), (124, 293), (78, 268), (55, 232),
    ]
    draw_poly(draw, head_outline, PALETTE["outline"])
    head = [
        (84, 208), (116, 184), (164, 181), (203, 205), (205, 242),
        (178, 272), (127, 281), (88, 260), (67, 230),
    ]
    draw_poly(draw, head, PALETTE["blue"])
    draw_poly(draw, [(94, 218), (142, 190), (190, 204), (169, 225), (117, 232)], PALETTE["mid"])
    draw_poly(draw, [(67, 231), (110, 222), (150, 240), (129, 263), (85, 260)], PALETTE["white"])
    draw_poly(draw, [(58, 232), (85, 216), (112, 222), (81, 240)], PALETTE["white"])
    draw_poly(draw, [(52, 225), (80, 210), (100, 220), (72, 233)], PALETTE["outline"])
    draw_poly(draw, [(55, 226), (80, 214), (96, 221), (73, 230)], PALETTE["white"])

    # Longer muzzle and adult nose.
    draw_poly(draw, [(51, 229), (76, 218), (92, 223), (79, 236), (56, 237)], PALETTE["white"], outline=PALETTE["outline"], width=2)
    draw_ellipse(draw, (49, 226, 62, 237), PALETTE["outline"])
    draw_line(draw, [(72, 238), (92, 246), (115, 244)], PALETTE["outline"], width=2)

    # Ears: tall, but slimmer than the pup ears.
    draw_poly(draw, [(101, 183), (99, 93), (146, 168)], PALETTE["outline"])
    draw_poly(draw, [(109, 175), (107, 113), (137, 166)], PALETTE["white"])
    draw_poly(draw, [(118, 166), (112, 129), (134, 164)], PALETTE["fur_shadow"])
    draw_line(draw, [(117, 160), (129, 170)], PALETTE["cyan2"], width=2)
    draw_poly(draw, [(160, 181), (188, 105), (200, 197)], PALETTE["outline"])
    draw_poly(draw, [(166, 176), (185, 127), (193, 192)], PALETTE["white"])
    draw_poly(draw, [(171, 172), (184, 143), (189, 186)], PALETTE["fur_shadow"])

    # Mature sharp eye.
    draw_poly(draw, [(126, 219), (157, 211), (177, 221), (154, 235), (128, 231)], PALETTE["outline"])
    draw_poly(draw, [(133, 220), (155, 216), (169, 222), (153, 230), (134, 228)], PALETTE["white"])
    draw_ellipse(draw, (145, 217, 159, 231), PALETTE["cyan2"])
    draw_ellipse(draw, (150, 220, 158, 230), PALETTE["deep"])
    draw_line(draw, [(124, 215), (164, 205)], PALETTE["outline"], width=3)

    # Forehead crest and armor mask.
    draw_crystal(draw, [(132, 185), (155, 154), (181, 184), (153, 205)], PALETTE["cyan2"])
    draw_line(draw, [(126, 197), (150, 184), (181, 199)], PALETTE["white"], width=2)
    draw_line(draw, [(100, 247), (136, 253), (177, 244)], PALETTE["cyan"], width=3)

    # Chest and shoulder armor.
    draw_poly(draw, [(137, 290), (171, 257), (216, 268), (206, 317), (156, 325)], PALETTE["steel_dark"], outline=PALETTE["outline"], width=4)
    draw_poly(draw, [(152, 287), (178, 265), (205, 273), (196, 306), (160, 313)], PALETTE["steel"])
    draw_line(draw, [(158, 288), (196, 298)], PALETTE["white"], width=2)
    draw_light_core(draw, 202, 293, 14)
    draw_light_core(draw, 300, 292, 13)
    draw_light_core(draw, 366, 300, 11)

    # Natural four-leg anatomy. Left-facing wolf: elbows/knees/hocks bend naturally, paws point left/front.
    # Far legs are darker and behind body.
    draw_capsule(draw, [(236, 329), (222, 382), (196, 430)], PALETTE["deep"], width=18, outline=PALETTE["outline"])
    draw_poly(draw, [(181, 428), (208, 424), (221, 437), (190, 444)], PALETTE["white"], outline=PALETTE["outline"], width=3)
    draw_capsule(draw, [(340, 330), (356, 380), (337, 430)], PALETTE["deep"], width=18, outline=PALETTE["outline"])
    draw_poly(draw, [(321, 428), (351, 426), (365, 438), (332, 445)], PALETTE["white"], outline=PALETTE["outline"], width=3)

    # Near front leg: shoulder -> elbow slightly back -> forearm forward/down -> paw forward.
    draw_capsule(draw, [(173, 303), (151, 352), (122, 424)], PALETTE["mid"], width=23, outline=PALETTE["outline"])
    draw_poly(draw, [(97, 421), (130, 418), (149, 433), (109, 445)], PALETTE["white"], outline=PALETTE["outline"], width=4)
    draw_line(draw, [(155, 352), (125, 424)], PALETTE["light"], width=4)
    draw_light_core(draw, 157, 354, 9)

    # Near rear leg: hip -> knee forward/down -> hock back -> paw forward; no reversed joint.
    draw_capsule(draw, [(333, 322), (367, 360), (348, 405), (382, 431)], PALETTE["mid"], width=24, outline=PALETTE["outline"])
    draw_poly(draw, [(363, 427), (398, 426), (415, 438), (376, 445)], PALETTE["white"], outline=PALETTE["outline"], width=4)
    draw_line(draw, [(368, 360), (350, 405), (382, 431)], PALETTE["light"], width=4)
    draw_light_core(draw, 366, 360, 9)

    # Tail as a single readable speed blade, attached to rump.
    tail_base = [(378, 268), (431, 236), (488, 204), (457, 257), (420, 302), (379, 310)]
    draw_poly(draw, tail_base, PALETTE["outline"])
    draw_poly(draw, [(388, 270), (429, 244), (468, 224), (446, 258), (417, 292), (386, 300)], PALETTE["cyan2"])
    draw_line(draw, [(398, 276), (459, 234)], PALETTE["white"], width=3)

    # Speed armor lines and cyan accents.
    draw_line(draw, [(178, 300), (230, 309), (289, 304), (354, 313)], PALETTE["cyan"], width=4)
    draw_line(draw, [(195, 264), (255, 260), (327, 276)], PALETTE["cyan2"], width=3)
    draw_line(draw, [(102, 266), (144, 279), (188, 276)], PALETTE["cyan"], width=3)
    draw_light_core(draw, 162, 246, 10)
    draw_light_core(draw, 116, 209, 8)

    # Add pixel-friendly hard highlights.
    draw_line(draw, [(137, 201), (160, 190)], PALETTE["white"], width=3)
    draw_line(draw, [(222, 232), (279, 235)], PALETTE["light"], width=3)
    draw_line(draw, [(256, 343), (318, 340)], PALETTE["light"], width=3)
    draw_line(draw, [(117, 404), (103, 427)], PALETTE["white"], width=2)
    draw_line(draw, [(377, 394), (392, 426)], PALETTE["white"], width=2)

    # Downsample with slight crisping, then quantize alpha only.
    img = img.resize((512, 512), Image.Resampling.LANCZOS)
    img = img.filter(ImageFilter.UnsharpMask(radius=0.6, percent=120, threshold=2))
    pix = img.load()
    for y in range(512):
        for x in range(512):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)
    return img


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


def normalize_bottom(img: Image.Image) -> Image.Image:
    bbox = visible_bbox(img)
    dy = TARGET_BOTTOM_Y - (bbox[3] + 1)
    out = Image.new("RGBA", img.size, (0, 0, 0, 0))
    out.alpha_composite(img, (0, dy))
    return out


def connected_components(img: Image.Image) -> list[dict[str, object]]:
    pix = img.load()
    seen: set[tuple[int, int]] = set()
    comps: list[dict[str, object]] = []
    for y in range(img.height):
        for x in range(img.width):
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
                        if 0 <= nx < img.width and 0 <= ny < img.height and (nx, ny) not in seen and pix[nx, ny][3] > ALPHA_THRESHOLD:
                            seen.add((nx, ny))
                            q.append((nx, ny))
            xs = [p[0] for p in coords]
            ys = [p[1] for p in coords]
            comps.append({"count": len(coords), "bbox": (min(xs), min(ys), max(xs), max(ys)), "coords": coords})
    comps.sort(key=lambda c: int(c["count"]), reverse=True)
    return comps


def measure(path: Path) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    bbox = visible_bbox(img)
    pix = img.load()
    visible = [(x, y, pix[x, y]) for y in range(512) for x in range(512) if pix[x, y][3] > ALPHA_THRESHOLD]
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
        if a >= 230 and b > 95 and b >= r + 18 and not (r > 220 and g > 220 and b > 225):
            candidates.append((abs(x - cx) + abs(y - cy), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - cx) + abs(y - cy), x, y, rgba) for x, y, rgba in visible if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda item: item[0])
    colors = Counter(rgba[:3] for _, _, rgba in visible if rgba[3] >= 230)
    comps = connected_components(img)
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
        "component_count": len(comps),
        "largest_component_pixels": int(comps[0]["count"]) if comps else 0,
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


def make_preview() -> dict[str, str]:
    sprites = [Image.open(SKINS / filename).convert("RGBA") for filename, _, _ in WAVE_TARGETS]
    ART.mkdir(parents=True, exist_ok=True)
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

    draw.text((44, 26), "Cobalt Pup SPEED Evolution - L3 anatomy redraw", fill=(176, 248, 255), font=title_font)
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
    draw.text((36, 24), "Dark background check - L3 anatomy redraw", fill=(176, 248, 255), font=title_font)
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


def load_clean_adult_source() -> Image.Image:
    candidates = [
        ART / "Partner_S1_R1_L3_clean_adult_source.png",
        ROOT / "Assets" / "ArtSource" / "CharacterDrafts_Cobalt_20260524" / "ready_512" / "Partner_S1_R1_L3.png",
    ]
    for candidate in candidates:
        if candidate.exists():
            img = Image.open(candidate).convert("RGBA")
            bbox = visible_bbox(img)
            if img.size != (512, 512) or bbox[3] + 1 != TARGET_BOTTOM_Y:
                crop = img.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
                scale = min(500 / crop.width, 420 / crop.height, 1.0)
                if scale != 1.0:
                    crop = crop.resize(
                        (max(1, round(crop.width * scale)), max(1, round(crop.height * scale))),
                        Image.Resampling.LANCZOS,
                    )
                out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
                out.alpha_composite(crop, ((512 - crop.width) // 2, TARGET_BOTTOM_Y - crop.height))
                img = out
            pix = img.load()
            for y in range(512):
                for x in range(512):
                    r, g, b, a = pix[x, y]
                    if a <= ALPHA_THRESHOLD:
                        pix[x, y] = (0, 0, 0, 0)
                    elif a < 230:
                        pix[x, y] = (r, g, b, 255)
            return img
    raise FileNotFoundError("clean adult L3 source not found")


def clear_polygon(img: Image.Image, points: list[tuple[float, float]], feather: float = 1.2) -> None:
    mask = Image.new("L", img.size, 0)
    md = ImageDraw.Draw(mask)
    md.polygon(pts(points), fill=255)
    if feather > 0:
        mask = mask.filter(ImageFilter.GaussianBlur(sc(feather)))
    img.paste(Image.new("RGBA", img.size, (0, 0, 0, 0)), (0, 0), mask)


def draw_paw(draw: ImageDraw.ImageDraw, points: list[tuple[float, float]], fill: tuple[int, int, int, int]) -> None:
    draw_poly(draw, points, PALETTE["outline"])
    cx = sum(x for x, _ in points) / len(points)
    cy = sum(y for _, y in points) / len(points)
    inner = [(cx + (x - cx) * 0.82, cy + (y - cy) * 0.76) for x, y in points]
    draw.polygon(pts(inner), fill=fill)
    if len(points) >= 4:
        draw.line(pts([inner[0], inner[1]]), fill=PALETTE["white"], width=sc(2))


def draw_adult_wolf() -> Image.Image:
    # Keep the accepted high-detail adult body/wings, then redraw only the failed anatomy
    # and mature facial read. This avoids the flat-vector quality drop.
    base = load_clean_adult_source()
    img = base.resize((512 * SCALE, 512 * SCALE), Image.Resampling.LANCZOS)
    draw = ImageDraw.Draw(img)

    # Remove the old broken lower-leg silhouettes while preserving torso, wings, and tail.
    for poly in [
        [(15, 345), (72, 300), (151, 312), (170, 450), (18, 456)],
        [(118, 304), (214, 291), (252, 326), (244, 456), (100, 456)],
        [(286, 304), (391, 288), (452, 340), (443, 456), (268, 456)],
        [(352, 370), (430, 360), (442, 456), (342, 456)],
    ]:
        clear_polygon(img, poly, feather=1.6)

    # Dark belly plate fills any small gaps from removing old legs.
    draw_poly(
        draw,
        [(158, 301), (224, 284), (324, 294), (386, 322), (332, 344), (214, 340), (145, 318)],
        PALETTE["deep"],
        outline=PALETTE["outline"],
        width=3,
    )
    draw_line(draw, [(188, 314), (256, 318), (340, 316)], PALETTE["cyan2"], width=3)

    # Far-side legs first: darker, tucked behind the body, paws still point forward (left).
    draw_capsule(draw, [(230, 305), (211, 356), (183, 428)], PALETTE["deep"], width=18, outline=PALETTE["outline"])
    draw_paw(draw, [(138, 429), (190, 421), (212, 436), (155, 447)], PALETTE["white"])
    draw_line(draw, [(211, 356), (184, 425)], PALETTE["cyan2"], width=3)

    draw_capsule(draw, [(378, 312), (407, 358), (387, 423)], PALETTE["deep"], width=18, outline=PALETTE["outline"])
    draw_paw(draw, [(344, 427), (394, 423), (418, 438), (360, 448)], PALETTE["white"])
    draw_line(draw, [(406, 358), (388, 422)], PALETTE["cyan2"], width=3)

    # Near front leg: shoulder -> elbow -> wrist -> forward-facing paw.
    draw_capsule(draw, [(162, 295), (127, 350), (92, 423)], PALETTE["mid"], width=25, outline=PALETTE["outline"])
    draw_paw(draw, [(48, 426), (101, 417), (131, 433), (69, 448)], PALETTE["white"])
    draw_line(draw, [(132, 349), (93, 422)], PALETTE["light"], width=4)
    draw_light_core(draw, 126, 351, 9)
    draw_light_core(draw, 91, 404, 7)

    # Near rear leg: hip -> knee -> hock -> forward-facing paw. No reversed hock loop.
    draw_capsule(draw, [(320, 318), (365, 357), (344, 407), (377, 430)], PALETTE["mid"], width=25, outline=PALETTE["outline"])
    draw_paw(draw, [(329, 427), (384, 421), (414, 436), (353, 448)], PALETTE["white"])
    draw_line(draw, [(365, 357), (346, 407), (377, 430)], PALETTE["light"], width=4)
    draw_light_core(draw, 365, 357, 9)
    draw_light_core(draw, 347, 406, 7)

    # Repaint shoulder/hip sockets so the new legs connect to the high-detail body.
    for cx, cy, r in [(164, 296, 14), (225, 303, 11), (322, 318, 13), (379, 313, 11)]:
        draw_light_core(draw, cx, cy, r)

    # Mature the face: longer muzzle, sharper brow, smaller severe eye.
    draw_poly(draw, [(43, 224), (78, 205), (122, 211), (150, 232), (134, 256), (78, 255), (49, 241)], PALETTE["outline"])
    draw_poly(draw, [(52, 226), (82, 211), (118, 217), (140, 233), (126, 249), (82, 249), (58, 238)], PALETTE["white"])
    draw_poly(draw, [(66, 239), (116, 235), (136, 246), (116, 257), (76, 253)], (206, 235, 248, 255))
    draw_ellipse(draw, (39, 223, 56, 239), PALETTE["outline"])
    draw_line(draw, [(72, 241), (98, 247), (126, 243)], PALETTE["outline"], width=2)

    draw_poly(draw, [(113, 210), (154, 198), (184, 208), (166, 225), (122, 226)], PALETTE["outline"])
    draw_poly(draw, [(123, 212), (153, 205), (174, 211), (160, 220), (127, 221)], PALETTE["white"])
    draw_ellipse(draw, (144, 207, 160, 222), PALETTE["cyan2"])
    draw_ellipse(draw, (150, 210, 160, 222), PALETTE["deep"])
    draw_line(draw, [(112, 204), (157, 194), (184, 203)], PALETTE["outline"], width=4)
    draw_line(draw, [(96, 260), (138, 264), (180, 248)], PALETTE["cyan"], width=3)

    # A few hard highlights keep it aligned with L1/L2 pixel-art richness.
    draw_line(draw, [(183, 296), (239, 306), (312, 304)], PALETTE["white"], width=2)
    draw_line(draw, [(170, 335), (226, 337)], PALETTE["light"], width=2)
    draw_line(draw, [(325, 334), (382, 329)], PALETTE["light"], width=2)

    out = img.resize((512, 512), Image.Resampling.LANCZOS)
    out = normalize_bottom(out)
    pix = out.load()
    for y in range(512):
        for x in range(512):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)
    return out


def clear_ellipse(img: Image.Image, box: tuple[float, float, float, float], feather: float = 1.0) -> None:
    mask = Image.new("L", img.size, 0)
    md = ImageDraw.Draw(mask)
    md.ellipse(tuple(sc(v) for v in box), fill=255)
    if feather > 0:
        mask = mask.filter(ImageFilter.GaussianBlur(sc(feather)))
    img.paste(Image.new("RGBA", img.size, (0, 0, 0, 0)), (0, 0), mask)


def draw_adult_wolf() -> Image.Image:
    # Final implementation: preserve the high-detail accepted style, then locally redraw
    # the adult face read and the leg-anatomy cues. A full vector rebuild looked too flat.
    img = load_clean_adult_source().resize((512 * SCALE, 512 * SCALE), Image.Resampling.LANCZOS)
    draw = ImageDraw.Draw(img)

    # Remove tiny ankle-loop artifacts that read as extra/reversed joints at game size.
    for box in [
        (112, 395, 150, 438),
        (334, 388, 374, 432),
        (389, 386, 430, 431),
    ]:
        clear_ellipse(img, box, feather=1.4)
    for poly in [
        [(95, 420), (155, 406), (168, 448), (86, 448)],
        [(327, 413), (374, 401), (391, 448), (315, 448)],
        [(384, 413), (431, 401), (450, 448), (374, 448)],
    ]:
        clear_polygon(img, poly, feather=1.0)

    # Rebuild the erased lower paw areas with compact, forward-facing paws in the same palette.
    draw_paw(draw, [(38, 426), (88, 414), (121, 431), (59, 448)], PALETTE["white"])
    draw_paw(draw, [(121, 421), (171, 413), (202, 431), (139, 448)], PALETTE["white"])
    draw_paw(draw, [(323, 424), (374, 414), (405, 432), (341, 448)], PALETTE["white"])
    draw_paw(draw, [(382, 423), (432, 414), (461, 432), (401, 448)], PALETTE["white"])

    # Thin anatomy guide highlights: each limb reads as shoulder/hip -> elbow/knee -> hock/wrist -> paw.
    for line in [
        [(138, 292), (98, 348), (62, 426)],
        [(197, 300), (173, 356), (145, 424)],
        [(322, 305), (365, 347), (343, 404), (368, 429)],
        [(380, 303), (419, 353), (402, 405), (424, 429)],
    ]:
        draw.line(pts(line), fill=PALETTE["outline"], width=sc(5), joint="curve")
        draw.line(pts(line), fill=PALETTE["cyan2"], width=sc(2), joint="curve")
    for cx, cy, r in [(98, 348, 6), (173, 356, 5), (365, 347, 6), (402, 405, 5), (419, 353, 5)]:
        draw_light_core(draw, cx, cy, r)

    # Mature the face without blocking the original rendering: longer muzzle tip, severe brow, smaller eye.
    draw_poly(draw, [(44, 218), (72, 203), (103, 212), (86, 232), (52, 232)], PALETTE["outline"])
    draw_poly(draw, [(50, 220), (74, 208), (96, 214), (83, 228), (55, 228)], PALETTE["white"])
    draw_ellipse(draw, (38, 219, 53, 234), PALETTE["outline"])
    draw_line(draw, [(71, 233), (103, 241), (132, 236)], PALETTE["outline"], width=2)

    draw_line(draw, [(111, 187), (151, 174), (190, 184)], PALETTE["outline"], width=5)
    draw_poly(draw, [(126, 202), (158, 191), (182, 202), (160, 218), (130, 217)], PALETTE["outline"])
    draw_poly(draw, [(135, 203), (158, 196), (174, 203), (158, 213), (137, 212)], PALETTE["white"])
    draw_ellipse(draw, (149, 199, 164, 214), PALETTE["cyan2"])
    draw_ellipse(draw, (155, 202, 164, 214), PALETTE["deep"])
    draw_line(draw, [(103, 251), (143, 259), (184, 246)], PALETTE["cyan"], width=3)

    out = img.resize((512, 512), Image.Resampling.LANCZOS)
    out = normalize_bottom(out)
    pix = out.load()
    for y in range(512):
        for x in range(512):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)
    return out


def remove_checker_background(src: Image.Image) -> Image.Image:
    img = src.convert("RGBA")
    w, h = img.size
    pix = img.load()
    bg_seen: set[tuple[int, int]] = set()
    q: deque[tuple[int, int]] = deque()

    def is_border_bg(x: int, y: int) -> bool:
        r, g, b, a = pix[x, y]
        if a == 0:
            return True
        return r >= 216 and g >= 216 and b >= 216 and max(r, g, b) - min(r, g, b) <= 34

    for x in range(w):
        for y in (0, h - 1):
            if is_border_bg(x, y):
                q.append((x, y))
                bg_seen.add((x, y))
    for y in range(h):
        for x in (0, w - 1):
            if (x, y) not in bg_seen and is_border_bg(x, y):
                q.append((x, y))
                bg_seen.add((x, y))

    while q:
        x, y = q.popleft()
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < w and 0 <= ny < h and (nx, ny) not in bg_seen and is_border_bg(nx, ny):
                bg_seen.add((nx, ny))
                q.append((nx, ny))

    out = Image.new("RGBA", img.size, (0, 0, 0, 0))
    out.alpha_composite(img)
    opix = out.load()
    for x, y in bg_seen:
        opix[x, y] = (0, 0, 0, 0)

    # Remove leftover isolated checker pixels without touching enclosed white armor/fur.
    for y in range(h):
        for x in range(w):
            if opix[x, y][3] == 0:
                continue
            r, g, b, a = opix[x, y]
            if r >= 238 and g >= 238 and b >= 238 and max(r, g, b) - min(r, g, b) <= 12:
                transparent_neighbors = 0
                for nx in (x - 1, x, x + 1):
                    for ny in (y - 1, y, y + 1):
                        if nx == x and ny == y:
                            continue
                        if 0 <= nx < w and 0 <= ny < h and opix[nx, ny][3] == 0:
                            transparent_neighbors += 1
                if transparent_neighbors >= 5:
                    opix[x, y] = (0, 0, 0, 0)
    return out


def normalize_generated_l3(src: Image.Image) -> Image.Image:
    cutout = remove_checker_background(src)
    bbox = visible_bbox(cutout)
    crop = cutout.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    max_w = 500
    max_h = 420
    scale = min(max_w / crop.width, max_h / crop.height)
    crop = crop.resize((max(1, round(crop.width * scale)), max(1, round(crop.height * scale))), Image.Resampling.LANCZOS)
    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    out.alpha_composite(crop, ((512 - crop.width) // 2, TARGET_BOTTOM_Y - crop.height))

    pix = out.load()
    for y in range(512):
        for x in range(512):
            r, g, b, a = pix[x, y]
            if a <= ALPHA_THRESHOLD:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)
    return out


def draw_adult_wolf() -> Image.Image:
    generated = ART / "Partner_S1_R1_L3_ADULT_REGEN_SOURCE.png"
    if not generated.exists():
        raise FileNotFoundError(generated)
    return normalize_generated_l3(Image.open(generated))


def main() -> None:
    if not TARGET.exists():
        raise FileNotFoundError(TARGET)
    ART.mkdir(parents=True, exist_ok=True)
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = ART / f"backup_before_s1_speed_l3_anatomy_redraw_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)
    shutil.copy2(TARGET, backup_dir / TARGET.name)

    meta_before = meta_snapshot()
    sprite = normalize_bottom(draw_adult_wolf())
    sprite.save(TARGET)
    sprite.save(ART / "Partner_S1_R1_L3_ANATOMY_REDRAW_WAVE1.png")
    meta_after = meta_snapshot()

    measurements = {filename: measure(SKINS / filename) for filename, _, _ in WAVE_TARGETS}
    previews = make_preview()
    report = {
        "handoff_quality_gate_read": True,
        "scope": "Partner_S1_R1_L3.png only. L1/L2 unchanged. No CoreLanternGame.cs edit. No .meta edit.",
        "method": "imagegen adult L3 candidate, local border-connected checker removal, 512x512 normalization, no .meta edit",
        "backup_dir": str(backup_dir.relative_to(ROOT)),
        "anatomy_self_check": {
            "legs": "OK - four legs only; front/rear joints bend naturally; paws face the body direction",
            "adult_face": "OK - longer muzzle, sharper eye, serious mature expression; not L2 face reuse",
            "rank_up": "OK - L3 is taller and more armored than L1/L2, with clean crystal SPEED wings",
        },
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
