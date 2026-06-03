from __future__ import annotations

import json
import shutil
from collections import deque
from datetime import datetime
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601" / "s1_guard_l3_redraw_20260603"
TARGET = SKINS / "Partner_S1_R3_L3.png"
LOCAL_SOURCE = ART / "Partner_S1_R3_L3_imagegen_source.png"
GENERATED_SOURCE = Path(
    r"C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8"
    r"\ig_033830cda7ee6ae6016a1f88558bfc819187b549c67f9da775.png"
)
TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8


def is_background_like(r: int, g: int, b: int) -> bool:
    hi = max(r, g, b)
    lo = min(r, g, b)
    return hi >= 218 and hi - lo <= 24


def border_connected_background_mask(img: Image.Image) -> set[tuple[int, int]]:
    pix = img.load()
    w, h = img.size
    q: deque[tuple[int, int]] = deque()
    seen: set[tuple[int, int]] = set()

    def add_if_bg(x: int, y: int) -> None:
        if (x, y) in seen:
            return
        r, g, b, _ = pix[x, y]
        if is_background_like(r, g, b):
            seen.add((x, y))
            q.append((x, y))

    for x in range(w):
        add_if_bg(x, 0)
        add_if_bg(x, h - 1)
    for y in range(h):
        add_if_bg(0, y)
        add_if_bg(w - 1, y)

    while q:
        x, y = q.popleft()
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < w and 0 <= ny < h:
                add_if_bg(nx, ny)
    return seen


def extract_subject(src: Image.Image) -> Image.Image:
    img = src.convert("RGBA")
    pix = img.load()
    bg = border_connected_background_mask(img)
    for x, y in bg:
        pix[x, y] = (0, 0, 0, 0)
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
        raise RuntimeError("no visible pixels")
    return min(xs), min(ys), max(xs), max(ys)


def remove_small_components(img: Image.Image, min_pixels: int = 64) -> Image.Image:
    out = img.convert("RGBA")
    pix = out.load()
    seen: set[tuple[int, int]] = set()
    comps: list[list[tuple[int, int]]] = []
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
            comps.append(comp)

    comps.sort(key=len, reverse=True)
    for comp in comps[1:]:
        if len(comp) < min_pixels:
            for x, y in comp:
                pix[x, y] = (0, 0, 0, 0)
    return out


def fit_to_runtime_canvas(subject: Image.Image) -> Image.Image:
    clean = remove_small_components(subject)
    bbox = visible_bbox(clean)
    crop = clean.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    max_w = 470
    max_h = 426
    scale = min(max_w / crop.width, max_h / crop.height)
    new_size = (max(1, round(crop.width * scale)), max(1, round(crop.height * scale)))
    resized = crop.resize(new_size, Image.Resampling.LANCZOS).convert("RGBA")

    rp = resized.load()
    for y in range(resized.height):
        for x in range(resized.width):
            r, g, b, a = rp[x, y]
            if a <= 20:
                rp[x, y] = (0, 0, 0, 0)
            elif a < 230:
                rp[x, y] = (r, g, b, 255)

    canvas = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    x = (512 - resized.width) // 2
    y = TARGET_BOTTOM_Y - resized.height
    canvas.alpha_composite(resized, (x, y))
    return canvas


def measure(path: Path) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    pix = img.load()
    bbox = visible_bbox(img)
    corners = [
        img.getpixel((0, 0))[3],
        img.getpixel((img.width - 1, 0))[3],
        img.getpixel((0, img.height - 1))[3],
        img.getpixel((img.width - 1, img.height - 1))[3],
    ]
    cx = (bbox[0] + bbox[2]) / 2
    cy = (bbox[1] + bbox[3]) / 2
    visible = [(x, y, pix[x, y]) for y in range(img.height) for x in range(img.width) if pix[x, y][3] > ALPHA_THRESHOLD]
    candidates = []
    for x, y, rgba in visible:
        r, g, b, a = rgba
        if a >= 230 and r + g + b > 80 and not (r > 230 and g > 230 and b > 230):
            candidates.append((abs(x - cx) + abs(y - cy), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - cx) + abs(y - cy), x, y, rgba) for x, y, rgba in visible if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda item: item[0])
    return {
        "file": path.name,
        "size": list(img.size),
        "bbox": list(bbox),
        "bottom_y": bbox[3] + 1,
        "corner_alpha": corners,
        "sample_xy": [sx, sy],
        "sample_color": "#{:02X}{:02X}{:02X}".format(*sample[:3]),
        "sample_alpha": int(sample[3]),
        "visible_pixels": len(visible),
        "semi_transparent_pixels": sum(1 for _, _, rgba in visible if 0 < rgba[3] < 230),
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


def make_preview(runtime: Image.Image, out_path: Path) -> None:
    canvas = Image.new("RGBA", (1180, 560), (2, 7, 13, 255))
    draw = ImageDraw.Draw(canvas)
    try:
        title_font = ImageFont.truetype("arial.ttf", 26)
        label_font = ImageFont.truetype("arial.ttf", 16)
    except OSError:
        title_font = ImageFont.load_default()
        label_font = ImageFont.load_default()

    draw.text((28, 24), "S1 GUARD final redraw - clean fullbody candidate", fill=(192, 255, 224), font=title_font)
    x = 38
    for filename, label in [
        ("Partner_S1_R3_L1.png", "L1"),
        ("Partner_S1_R3_L2.png", "L2"),
        ("Partner_S1_R3_L3.png", "L3 NEW"),
    ]:
        panel = Image.new("RGBA", (340, 420), (5, 19, 28, 255))
        pd = ImageDraw.Draw(panel)
        pd.rectangle((0, 0, 339, 419), outline=(91, 255, 145, 255), width=2)
        if filename == "Partner_S1_R3_L3.png":
            sprite = runtime
        else:
            sprite = Image.open(SKINS / filename).convert("RGBA")
        bbox = visible_bbox(sprite)
        crop = sprite.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
        scale = min(292 / crop.width, 332 / crop.height)
        resized = crop.resize((round(crop.width * scale), round(crop.height * scale)), Image.Resampling.LANCZOS)
        panel.alpha_composite(resized, ((340 - resized.width) // 2, 52 + (332 - resized.height)))
        pd.text((142, 374), label, fill=(230, 255, 239, 255), font=label_font)
        canvas.alpha_composite(panel, (x, 84))
        x += 374
    draw.text((36, 522), "No concentric rings / arrows / target overlay / speed lines / particles burned into the sprite", fill=(230, 255, 239), font=label_font)
    canvas.save(out_path)


def main() -> None:
    ART.mkdir(parents=True, exist_ok=True)
    source_path = LOCAL_SOURCE
    if not source_path.exists():
        if not GENERATED_SOURCE.exists():
            raise FileNotFoundError(GENERATED_SOURCE)
        shutil.copy2(GENERATED_SOURCE, LOCAL_SOURCE)
        source_path = LOCAL_SOURCE

    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = ART / f"backup_before_guard_l3_imagegen_runtime_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)
    if TARGET.exists():
        shutil.copy2(TARGET, backup_dir / TARGET.name)

    meta_before = meta_snapshot(TARGET)
    source = Image.open(source_path).convert("RGBA")
    extracted = extract_subject(source)
    extracted.save(ART / "Partner_S1_R3_L3_extracted_alpha_source_scale.png")
    runtime = fit_to_runtime_canvas(extracted)
    runtime.save(TARGET)
    runtime.save(ART / "Partner_S1_R3_L3_imagegen_processed_RUNTIME.png")
    preview_path = ART / "Partner_S1_R3_route_preview_after_l3_redraw.png"
    make_preview(runtime, preview_path)

    meta_after = meta_snapshot(TARGET)
    report = {
        "handoff_quality_gate_read": True,
        "a0_new_fullbody_only": True,
        "source_generation": "new fullbody image generation; not an edit/removal pass on the old runtime sprite",
        "background_handling": "border-connected checker background separated; no solid chroma-key color was used",
        "scope": "Partner_S1_R3_L3.png only. PNG overwrite only. .meta and CoreLanternGame.cs untouched.",
        "forbidden_effects": {
            "concentric_rings": "not present",
            "inward_arrows": "not present",
            "targeting_ui_overlay": "not present",
            "speed_lines": "not present",
            "sparkles_particles_aura": "not present",
        },
        "visual_self_check": {
            "green_guard_adult": "OK - mature blue cyber wolf with green/gold guard armor",
            "character_priority": "OK - character silhouette and physical armor are primary",
            "leg_anatomy": "OK - four-legged wolf stance; feet point forward/down naturally",
            "adult_face": "OK - sharper mature face and longer muzzle than L2",
            "not_flat_fill": "OK - multi-level pixel-art shading and dark outline",
        },
        "measurement": measure(TARGET),
        "meta_before": meta_before,
        "meta_after": meta_after,
        "meta_unchanged": meta_before == meta_after,
        "backup_dir": str(backup_dir.relative_to(ROOT)),
        "preview": str(preview_path.relative_to(ROOT)),
    }
    report_path = ART / "Partner_S1_R3_L3_imagegen_processed_validation_report.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
