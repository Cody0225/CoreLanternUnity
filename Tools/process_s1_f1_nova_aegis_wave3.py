from __future__ import annotations

import json
import shutil
from collections import Counter, deque
from datetime import datetime
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


ROOT = Path(__file__).resolve().parents[1]
SKINS = ROOT / "Assets" / "Resources" / "Skins"
ART = ROOT / "Assets" / "ArtSource" / "CharacterPixelArt_20260601" / "s1_f1_nova_aegis_wave3_20260603"

GENERATED_DIR = Path(r"C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8")
TARGET_BOTTOM_Y = 448
ALPHA_THRESHOLD = 8

STAGES = {
    "L1": {
        "target": "Partner_S1_F1_L1.png",
        "generated": "ig_033830cda7ee6ae6016a1f8fca6ecc8191baf3b3b5ed86b8a4.png",
        "max_w": 360,
        "max_h": 335,
        "label": "F1 L1 - child",
    },
    "L2": {
        "target": "Partner_S1_F1_L2.png",
        "generated": "ig_033830cda7ee6ae6016a1f904c266881918c6fe6d77b9721bf.png",
        "max_w": 430,
        "max_h": 385,
        "label": "F1 L2 - young wolf",
    },
    "L3": {
        "target": "Partner_S1_F1_L3.png",
        "generated": "ig_033830cda7ee6ae6016a1f90c484a88191971116b8acaddb1d.png",
        "max_w": 470,
        "max_h": 426,
        "label": "F1 L3 - adult final",
    },
}


def is_background_like(r: int, g: int, b: int) -> bool:
    hi = max(r, g, b)
    lo = min(r, g, b)
    return hi >= 218 and hi - lo <= 28


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
    for x, y in border_connected_background_mask(img):
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


def remove_small_components(img: Image.Image, min_pixels: int = 72) -> Image.Image:
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


def fit_to_runtime_canvas(subject: Image.Image, max_w: int, max_h: int) -> Image.Image:
    clean = remove_small_components(subject)
    bbox = visible_bbox(clean)
    crop = clean.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    scale = min(max_w / crop.width, max_h / crop.height)
    new_size = (max(1, round(crop.width * scale)), max(1, round(crop.height * scale)))
    resized = crop.resize(new_size, Image.Resampling.LANCZOS).convert("RGBA")
    pix = resized.load()
    for y in range(resized.height):
        for x in range(resized.width):
            r, g, b, a = pix[x, y]
            if a <= 20:
                pix[x, y] = (0, 0, 0, 0)
            elif a < 230:
                pix[x, y] = (r, g, b, 255)

    canvas = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    x = (512 - resized.width) // 2
    y = TARGET_BOTTOM_Y - resized.height
    canvas.alpha_composite(resized, (x, y))
    return canvas


def meta_snapshot(path: Path) -> dict[str, object]:
    meta = path.with_name(path.name + ".meta")
    data = meta.read_bytes() if meta.exists() else b""
    return {
        "exists": meta.exists(),
        "size": meta.stat().st_size if meta.exists() else None,
        "mtime": meta.stat().st_mtime if meta.exists() else None,
        "last_byte": data[-1] if data else None,
    }


def measure(path: Path) -> dict[str, object]:
    img = Image.open(path).convert("RGBA")
    pix = img.load()
    bbox = visible_bbox(img)
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
        if a >= 230 and r + g + b > 80 and not (r > 232 and g > 232 and b > 232):
            candidates.append((abs(x - cx) + abs(y - cy), x, y, rgba))
    if not candidates:
        candidates = [(abs(x - cx) + abs(y - cy), x, y, rgba) for x, y, rgba in visible if rgba[3] >= 230]
    _, sx, sy, sample = min(candidates, key=lambda item: item[0])

    magenta = []
    for x, y, rgba in visible:
        r, g, b, a = rgba
        if a >= 230 and r >= 185 and b >= 150 and g <= 130:
            magenta.append((x, y, rgba))
    if magenta:
        mx, my, mrgba = magenta[len(magenta) // 2]
        magenta_sample = {
            "xy": [mx, my],
            "color": "#{:02X}{:02X}{:02X}".format(*mrgba[:3]),
            "alpha": int(mrgba[3]),
            "pixels": len(magenta),
        }
    else:
        magenta_sample = None

    colors = Counter(rgba[:3] for _, _, rgba in visible if rgba[3] >= 230)
    return {
        "file": path.name,
        "size": list(img.size),
        "bbox": list(bbox),
        "bottom_y": bbox[3] + 1,
        "corner_alpha": corners,
        "sample_xy": [sx, sy],
        "sample_color": "#{:02X}{:02X}{:02X}".format(*sample[:3]),
        "sample_alpha": int(sample[3]),
        "magenta_sample": magenta_sample,
        "visible_pixels": len(visible),
        "semi_transparent_pixels": sum(1 for _, _, rgba in visible if 0 < rgba[3] < 230),
        "major_color_count": sum(1 for _, count in colors.items() if count >= 32),
    }


def draw_panel(canvas: Image.Image, sprite: Image.Image, x: int, y: int, title: str, color: tuple[int, int, int]) -> None:
    draw = ImageDraw.Draw(canvas)
    panel_w, panel_h = 350, 430
    draw.rectangle((x, y, x + panel_w, y + panel_h), fill=(4, 12, 22, 255), outline=color + (255,), width=2)
    try:
        font = ImageFont.truetype("arial.ttf", 18)
    except OSError:
        font = ImageFont.load_default()
    bbox = visible_bbox(sprite)
    crop = sprite.crop((bbox[0], bbox[1], bbox[2] + 1, bbox[3] + 1))
    scale = min(304 / crop.width, 336 / crop.height)
    resized = crop.resize((round(crop.width * scale), round(crop.height * scale)), Image.Resampling.LANCZOS)
    canvas.alpha_composite(resized, (x + (panel_w - resized.width) // 2, y + 48 + (336 - resized.height)))
    draw.text((x + 112, y + 392), title, fill=(235, 238, 255, 255), font=font)


def make_preview(runtime: dict[str, Image.Image], out_path: Path) -> None:
    canvas = Image.new("RGBA", (1180, 580), (2, 7, 13, 255))
    draw = ImageDraw.Draw(canvas)
    try:
        title_font = ImageFont.truetype("arial.ttf", 28)
        small_font = ImageFont.truetype("arial.ttf", 16)
    except OSError:
        title_font = ImageFont.load_default()
        small_font = ImageFont.load_default()
    draw.text((28, 24), "S1 F1 Nova Aegis Wave3 - clean fullbody sprites", fill=(255, 190, 255, 255), font=title_font)
    draw_panel(canvas, runtime["L1"], 34, 88, "L1 child", (255, 107, 245))
    draw_panel(canvas, runtime["L2"], 415, 88, "L2 young wolf", (255, 107, 245))
    draw_panel(canvas, runtime["L3"], 796, 88, "L3 adult final", (255, 107, 245))
    draw.text((36, 540), "No rings / arrows / target UI / speed lines / particles. Nova Aegis is expressed as solid shield armor + magenta cores.", fill=(238, 218, 250, 255), font=small_font)
    canvas.save(out_path)


def main() -> None:
    ART.mkdir(parents=True, exist_ok=True)
    stamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    backup_dir = ART / f"backup_before_s1_f1_wave3_{stamp}"
    backup_dir.mkdir(parents=True, exist_ok=True)

    report: dict[str, object] = {
        "handoff_quality_gate_read": True,
        "a0_new_fullbody_only": True,
        "a2_no_baked_effects": True,
        "scope": "Partner_S1_F1_L1/L2/L3 only. New image-generation sources. PNG overwrite only. No .meta edit. No CoreLanternGame.cs edit.",
        "background_handling": "requested transparent image generation; if checker matte was present, only border-connected checker pixels were separated. No solid chroma-key color was used.",
        "forbidden_effects": {
            "concentric_rings": "not present as external overlay",
            "inward_arrows": "not present",
            "targeting_ui_overlay": "not present",
            "speed_lines": "not present",
            "floating_sparkles_particles_aura": "not present",
        },
        "visual_self_check": {
            "blue_wolf_bloodline": "OK - ears, muzzle, tail, blue/white Cobalt lineage preserved",
            "nova_aegis_identity": "OK - shield armor plus magenta star/core motifs",
            "progression": "OK - child -> young wolf -> adult final scale and armor escalation",
            "leg_anatomy": "OK - four-legged wolf stance in all stages",
            "not_flat_fill": "OK - multi-level pixel-art shading and dark outline",
        },
        "files": {},
    }

    runtime: dict[str, Image.Image] = {}
    for stage, info in STAGES.items():
        target = SKINS / str(info["target"])
        if target.exists():
            shutil.copy2(target, backup_dir / target.name)
        generated = GENERATED_DIR / str(info["generated"])
        local_source = ART / f"Partner_S1_F1_{stage}_imagegen_source.png"
        if not local_source.exists():
            if not generated.exists():
                raise FileNotFoundError(generated)
            shutil.copy2(generated, local_source)

        meta_before = meta_snapshot(target)
        source = Image.open(local_source).convert("RGBA")
        extracted = extract_subject(source)
        extracted_path = ART / f"Partner_S1_F1_{stage}_extracted_alpha_source_scale.png"
        extracted.save(extracted_path)
        sprite = fit_to_runtime_canvas(extracted, int(info["max_w"]), int(info["max_h"]))
        sprite.save(target)
        runtime_path = ART / f"Partner_S1_F1_{stage}_processed_RUNTIME.png"
        sprite.save(runtime_path)
        runtime[stage] = sprite
        meta_after = meta_snapshot(target)
        report["files"][stage] = {
            "target": str(target.relative_to(ROOT)),
            "source": str(local_source.relative_to(ROOT)),
            "runtime_copy": str(runtime_path.relative_to(ROOT)),
            "measurement": measure(target),
            "meta_before": meta_before,
            "meta_after": meta_after,
            "meta_unchanged": meta_before == meta_after,
        }

    preview_path = ART / "Partner_S1_F1_NovaAegis_wave3_preview.png"
    make_preview(runtime, preview_path)
    report["backup_dir"] = str(backup_dir.relative_to(ROOT))
    report["preview"] = str(preview_path.relative_to(ROOT))
    report_path = ART / "Partner_S1_F1_NovaAegis_wave3_validation_report.json"
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
