param(
    [string]$OutDir = "Assets\ArtSource\CharacterPixelArt_20260601",
    [string]$PythonPath = "C:\Users\kodai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe"
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ResolvedOutDir = if ([System.IO.Path]::IsPathRooted($OutDir)) { $OutDir } else { Join-Path $ProjectRoot $OutDir }
$ResourceDir = Join-Path $ProjectRoot "Assets\Resources\Skins"

$script = @'
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
from collections import deque, Counter
import json
import math
import shutil
import statistics
import time

out_dir = Path(r"__OUT_DIR__")
resource_dir = Path(r"__RESOURCE_DIR__")
redraw_dir = out_dir / "hexcat_driftfox_baked_redraw_20260602"
backup_dir = redraw_dir / ("backup_before_baked_redraw_" + time.strftime("%Y%m%d_%H%M%S"))
redraw_dir.mkdir(parents=True, exist_ok=True)
backup_dir.mkdir(parents=True, exist_ok=True)

source_dir = out_dir / "color_identity_polish_20260602" / "backup_before_polish_20260602_143616"

targets = {
    "Partner_S4_L0.png": {
        "name": "Hex Cat",
        "animal": "purple cat",
        "source": source_dir / "Partner_S4_L0.png",
        "palette": {
            "outline": (16, 7, 34),
            "deep": (38, 15, 74),
            "shadow": (67, 28, 118),
            "base": (111, 55, 180),
            "mid": (154, 91, 222),
            "highlight": (207, 170, 255),
            "spark": (246, 236, 255),
            "eye": (236, 222, 255),
            "gem": (224, 205, 255),
        },
        "sample_hint": (252, 286),
        "expected_family": "purple",
    },
    "Partner_S5_L0.png": {
        "name": "Drift Fox",
        "animal": "pink fox",
        "source": source_dir / "Partner_S5_L0.png",
        "palette": {
            "outline": (42, 9, 30),
            "deep": (91, 24, 64),
            "shadow": (143, 45, 103),
            "base": (216, 78, 151),
            "mid": (242, 126, 191),
            "highlight": (255, 198, 229),
            "spark": (255, 239, 248),
            "white": (255, 238, 248),
            "gold": (255, 177, 45),
            "gold_shadow": (151, 74, 21),
        },
        "sample_hint": (266, 311),
        "expected_family": "pink",
    },
}

names = [
    ("Partner_S1_L0.png", "Cobalt Pup"),
    ("Partner_S2_L0.png", "Ember Drake"),
    ("Partner_S3_L0.png", "Sage Hare"),
    ("Partner_S4_L0.png", "Hex Cat"),
    ("Partner_S5_L0.png", "Drift Fox"),
    ("Partner_S6_L0.png", "Iron Bear"),
]

def clamp(v):
    return max(0, min(255, int(round(v))))

def rgb_to_hex(c):
    return "#{:02X}{:02X}{:02X}".format(c[0], c[1], c[2])

def luma(rgb):
    r, g, b = rgb
    return 0.2126 * r + 0.7152 * g + 0.0722 * b

def saturation(rgb):
    r, g, b = [v / 255.0 for v in rgb]
    mx = max(r, g, b)
    mn = min(r, g, b)
    if mx <= 0:
        return 0.0
    return (mx - mn) / mx

def blend(a, b, t):
    return (
        clamp(a[0] + (b[0] - a[0]) * t),
        clamp(a[1] + (b[1] - a[1]) * t),
        clamp(a[2] + (b[2] - a[2]) * t),
    )

def connected_components(im, alpha_threshold):
    pix = im.load()
    w, h = im.size
    seen = set()
    comps = []
    for sy in range(h):
        for sx in range(w):
            if (sx, sy) in seen:
                continue
            if pix[sx, sy][3] <= alpha_threshold:
                seen.add((sx, sy))
                continue
            q = deque([(sx, sy)])
            seen.add((sx, sy))
            pts = []
            while q:
                x, y = q.popleft()
                pts.append((x, y))
                for nx in (x - 1, x, x + 1):
                    for ny in (y - 1, y, y + 1):
                        if nx == x and ny == y:
                            continue
                        if nx < 0 or ny < 0 or nx >= w or ny >= h:
                            continue
                        if (nx, ny) in seen:
                            continue
                        seen.add((nx, ny))
                        if pix[nx, ny][3] > alpha_threshold:
                            q.append((nx, ny))
            if pts:
                comps.append(pts)
    comps.sort(key=len, reverse=True)
    return comps

def percentile(values, pct):
    if not values:
        return 0
    ordered = sorted(values)
    idx = int(round((len(ordered) - 1) * pct))
    return ordered[max(0, min(len(ordered) - 1, idx))]

def is_boundary(mask, x, y, w, h):
    for nx in (x - 1, x, x + 1):
        for ny in (y - 1, y, y + 1):
            if nx == x and ny == y:
                continue
            if nx < 0 or ny < 0 or nx >= w or ny >= h:
                return True
            if (nx, ny) not in mask:
                return True
    return False

def component_bbox(points):
    xs = [p[0] for p in points]
    ys = [p[1] for p in points]
    return (min(xs), min(ys), max(xs) + 1, max(ys) + 1)

def choose_color(filename, src_rgb, t, boundary, palette):
    lum = luma(src_rgb)
    sat = saturation(src_rgb)
    r, g, b = src_rgb

    if filename == "Partner_S5_L0.png":
        # Keep the current gold pendant readable while rebaking all blue/cyan tech
        # into pink-family highlights.
        if r > 135 and g > 85 and b < 84:
            return palette["gold"] if t >= 0.45 else palette["gold_shadow"]
        if boundary and t < 0.88:
            return palette["outline"]
        if t < 0.10:
            return palette["outline"]
        if t < 0.26:
            return palette["deep"]
        if t < 0.43:
            return palette["shadow"]
        if t < 0.61:
            return palette["base"]
        if t < 0.78:
            return palette["mid"]
        # High-luma low-saturation areas are the white cheek/tail/fur accents.
        if t >= 0.78 and sat < 0.28:
            return palette["white"] if t > 0.90 else blend(palette["highlight"], palette["white"], 0.48)
        if t < 0.92:
            return palette["highlight"]
        return palette["spark"]

    # Hex Cat: keep bright gem/eye surfaces pale lavender-white, otherwise use
    # a purple cell-shaded ramp.
    if boundary and t < 0.88:
        return palette["outline"]
    if t < 0.11:
        return palette["outline"]
    if t < 0.27:
        return palette["deep"]
    if t < 0.44:
        return palette["shadow"]
    if t < 0.62:
        return palette["base"]
    if t < 0.79:
        return palette["mid"]
    if t < 0.92:
        return palette["highlight"]
    return palette["spark"]

def redraw_one(filename, cfg):
    runtime_path = resource_dir / filename
    source_path = cfg["source"]
    if not runtime_path.exists():
        raise FileNotFoundError(runtime_path)
    if not source_path.exists():
        raise FileNotFoundError(source_path)

    shutil.copy2(runtime_path, backup_dir / filename)
    shutil.copy2(source_path, redraw_dir / (filename.replace(".png", "_shade_source.png")))

    src = Image.open(source_path).convert("RGBA")
    w, h = src.size
    if (w, h) != (512, 512):
        raise RuntimeError(f"{filename}: source is {w}x{h}, expected 512x512")

    comps = connected_components(src, 8)
    if not comps:
        raise RuntimeError(f"{filename}: no source body component")
    body = set(comps[0])
    src_pix = src.load()
    lumas = [luma(src_pix[x, y][:3]) for x, y in body]
    lo = percentile(lumas, 0.04)
    hi = percentile(lumas, 0.96)
    if hi <= lo:
        hi = lo + 1

    out = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
    out_pix = out.load()
    shade_counter = Counter()

    for x, y in body:
        rgba = src_pix[x, y]
        src_rgb = rgba[:3]
        raw_t = (luma(src_rgb) - lo) / (hi - lo)
        # Preserve source lighting, but push the ramp slightly to increase depth.
        t = max(0.0, min(1.0, raw_t))
        t = math.pow(t, 0.92)
        boundary = is_boundary(body, x, y, 512, 512)
        color = choose_color(filename, src_rgb, t, boundary, cfg["palette"])
        out_pix[x, y] = (*color, 255)
        shade_counter[rgb_to_hex(color)] += 1

    # Keep only the largest visible component after drawing.
    out_comps = connected_components(out, 0)
    if len(out_comps) > 1:
        keep = set(out_comps[0])
        pix = out.load()
        for pts in out_comps[1:]:
            for x, y in pts:
                pix[x, y] = (0, 0, 0, 0)

    box = out.getchannel("A").getbbox()
    if not box:
        raise RuntimeError(f"{filename}: output is empty")
    baseline_shift = 448 - box[3]
    if baseline_shift:
        shifted = Image.new("RGBA", (512, 512), (0, 0, 0, 0))
        shifted.alpha_composite(out, (0, baseline_shift))
        out = shifted
        box = out.getchannel("A").getbbox()

    # Output must preserve the height band. If the original source is just outside
    # by a few pixels, keep the silhouette rather than resampling it.
    height = box[3] - box[1]
    if not (327 <= height <= 333):
        raise RuntimeError(f"{filename}: height {height} outside 327-333")

    out.save(runtime_path)
    out.save(out_dir / filename)
    out.save(redraw_dir / filename)

    final = Image.open(runtime_path).convert("RGBA")
    comps_final = connected_components(final, 0)
    final_box = final.getchannel("A").getbbox()
    pix = final.load()
    sample = sample_color(final, cfg["sample_hint"], cfg["expected_family"])
    lum_values = [luma(pix[x, y][:3]) for pts in comps_final[:1] for x, y in pts if pix[x, y][3] > 0]
    top_colors = shade_counter.most_common(12)

    return {
        "file": filename,
        "name": cfg["name"],
        "source": str(source_path),
        "backup": str(backup_dir / filename),
        "size": list(final.size),
        "bbox": list(final_box),
        "bottom_y": final_box[3],
        "height": final_box[3] - final_box[1],
        "corner_alpha": [
            final.getpixel((0, 0))[3],
            final.getpixel((511, 0))[3],
            final.getpixel((0, 511))[3],
            final.getpixel((511, 511))[3],
        ],
        "component_count_alpha_gt_0": len(comps_final),
        "sample": sample,
        "alpha_min": min([pix[x, y][3] for pts in comps_final[:1] for x, y in pts]) if comps_final else None,
        "alpha_max": max([pix[x, y][3] for pts in comps_final[:1] for x, y in pts]) if comps_final else None,
        "luma_min": round(min(lum_values), 2),
        "luma_max": round(max(lum_values), 2),
        "luma_range": round(max(lum_values) - min(lum_values), 2),
        "distinct_colors": len(shade_counter),
        "top_colors": [{"color": c, "pixels": n} for c, n in top_colors],
        "baseline_shift": baseline_shift,
    }

def sample_color(im, hint, expected_family):
    pix = im.load()
    hx, hy = hint
    best = None
    best_score = -10**9
    for radius in range(0, 90):
        for y in range(max(0, hy - radius), min(512, hy + radius + 1)):
            for x in range(max(0, hx - radius), min(512, hx + radius + 1)):
                if abs(x - hx) != radius and abs(y - hy) != radius:
                    continue
                r, g, b, a = pix[x, y]
                if a < 230:
                    continue
                if expected_family == "purple":
                    family = (b + r * 0.88) - g * 1.55
                    if not (b >= g + 25 and r >= g + 18):
                        family -= 200
                else:
                    family = (r + b * 0.78) - g * 1.35
                    if not (r >= g + 26 and b >= g + 10):
                        family -= 200
                dist = abs(x - hx) + abs(y - hy)
                score = family - dist * 1.8
                if score > best_score:
                    best_score = score
                    best = (x, y, r, g, b, a)
        if best and radius >= 8:
            break
    if not best:
        raise RuntimeError(f"Could not find sample color for {expected_family}")
    x, y, r, g, b, a = best
    return {
        "xy": [x, y],
        "color": rgb_to_hex((r, g, b)),
        "alpha": a,
    }

def bbox_image(im):
    box = im.getchannel("A").getbbox()
    return im.crop(box) if box else im

def load_runtime_images():
    images = []
    for filename, label in names:
        images.append((filename, label, Image.open(resource_dir / filename).convert("RGBA")))
    return images

def write_previews(images):
    preview_bg = (5, 12, 16, 255)
    cyan = (117, 239, 255, 255)
    muted = (130, 175, 178, 255)
    gold = (255, 214, 80, 255)
    try:
        small = ImageFont.truetype("arial.ttf", 15)
        medium = ImageFont.truetype("arial.ttf", 18)
    except Exception:
        small = ImageFont.load_default()
        medium = ImageFont.load_default()

    cell_w = 256
    cell_h = 276
    baseline = 224
    preview = Image.new("RGBA", (cell_w * 6, cell_h), preview_bg)
    draw = ImageDraw.Draw(preview)
    draw.line((16, baseline, preview.width - 16, baseline), fill=(20, 75, 82, 255), width=1)
    for idx, (_, label, im) in enumerate(images):
        sample = bbox_image(im)
        scale = min(180 / sample.width, 178 / sample.height)
        scaled = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.LANCZOS)
        x = idx * cell_w + (cell_w - scaled.width) // 2
        y = baseline - scaled.height
        preview.alpha_composite(scaled, (x, y))
        draw.text((idx * cell_w + 10, 236), label, font=small, fill=cyan)
        draw.rectangle((idx * cell_w + 4, 4, idx * cell_w + cell_w - 4, cell_h - 4), outline=(19, 94, 105, 160), width=1)
    preview.save(out_dir / "CharacterPixelArt_6partners_lineup_preview.png")
    preview.save(redraw_dir / "HexCat_DriftFox_baked_redraw_lineup_preview.png")

    clean = Image.new("RGBA", (512 * 6, 512), (0, 0, 0, 0))
    for idx, (_, _, im) in enumerate(images):
        clean.alpha_composite(im, (idx * 512, 0))
    clean.save(out_dir / "CharacterPixelArt_6partners_lineup_clean_transparent.png")

    sizes = [64, 96, 128]
    shrink = Image.new("RGBA", (180 * 6, 150 * len(sizes)), preview_bg)
    draw = ImageDraw.Draw(shrink)
    for r, size in enumerate(sizes):
        draw.text((8, r * 150 + 8), f"{size}px", font=small, fill=gold)
        for c, (_, label, im) in enumerate(images):
            sample = bbox_image(im)
            scale = size / max(sample.width, sample.height)
            scaled = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.NEAREST)
            x = c * 180 + (180 - scaled.width) // 2
            y = r * 150 + 32 + (94 - scaled.height) // 2
            shrink.alpha_composite(scaled, (x, y))
            draw.text((c * 180 + 8, r * 150 + 124), label.split()[0], font=small, fill=muted)
            draw.rectangle((c * 180 + 4, r * 150 + 24, c * 180 + 176, r * 150 + 146), outline=(19, 94, 105, 160), width=1)
    shrink.save(out_dir / "CharacterPixelArt_6partners_shrink_check.png")
    shrink.save(redraw_dir / "HexCat_DriftFox_baked_redraw_shrink_check.png")

    # Focused before/after and background visibility checks for S4/S5.
    bgs = [(5, 12, 16, 255), (31, 45, 63, 255), (92, 104, 112, 255), (230, 230, 230, 255)]
    focus = Image.new("RGBA", (512 * 4, 560 * 2), (0, 0, 0, 0))
    draw = ImageDraw.Draw(focus)
    focus_items = [
        ("Partner_S4_L0.png", "Hex Cat"),
        ("Partner_S5_L0.png", "Drift Fox"),
    ]
    runtime_by_file = {filename: im for filename, _, im in images}
    for row, (filename, label) in enumerate(focus_items):
        im = runtime_by_file[filename]
        for col, bg in enumerate(bgs):
            tile = Image.new("RGBA", (512, 512), bg)
            tile.alpha_composite(im, (0, 0))
            focus.alpha_composite(tile, (col * 512, row * 560))
            draw.text((col * 512 + 12, row * 560 + 520), f"{label} {bg[:3]}", font=medium, fill=(255, 255, 255, 255))
    focus.save(redraw_dir / "HexCat_DriftFox_baked_redraw_visibility_check.png")

def validate_all():
    validation = []
    for filename, label in names:
        im = Image.open(resource_dir / filename).convert("RGBA")
        box = im.getchannel("A").getbbox()
        comps = connected_components(im, 0)
        validation.append({
            "file": filename,
            "label": label,
            "size": list(im.size),
            "bbox": list(box) if box else None,
            "bottom_y": box[3] if box else None,
            "height": (box[3] - box[1]) if box else None,
            "corner_alpha": [
                im.getpixel((0, 0))[3],
                im.getpixel((511, 0))[3],
                im.getpixel((0, 511))[3],
                im.getpixel((511, 511))[3],
            ],
            "component_count_alpha_gt_0": len(comps),
        })
    return validation

reports = []
for filename, cfg in targets.items():
    reports.append(redraw_one(filename, cfg))

images = load_runtime_images()
write_previews(images)
all_validation = validate_all()

gate_failures = []
for item in reports:
    if item["size"] != [512, 512]:
        gate_failures.append(f"{item['file']} size")
    if item["bottom_y"] != 448:
        gate_failures.append(f"{item['file']} bottom_y={item['bottom_y']}")
    if not (327 <= item["height"] <= 333):
        gate_failures.append(f"{item['file']} height={item['height']}")
    if item["corner_alpha"] != [0, 0, 0, 0]:
        gate_failures.append(f"{item['file']} corner_alpha={item['corner_alpha']}")
    if item["sample"]["alpha"] < 230:
        gate_failures.append(f"{item['file']} sample alpha={item['sample']['alpha']}")
    if item["component_count_alpha_gt_0"] != 1:
        gate_failures.append(f"{item['file']} component_count={item['component_count_alpha_gt_0']}")
    if item["distinct_colors"] < 7 or item["luma_range"] < 120:
        gate_failures.append(f"{item['file']} shading weak colors={item['distinct_colors']} luma_range={item['luma_range']}")

report = {
    "created_at": time.strftime("%Y-%m-%d %H:%M:%S"),
    "mode": "transparent-canvas local redraw; no chroma-key; PNG overwrite only",
    "backup_dir": str(backup_dir),
    "redraw_reports": reports,
    "all_partner_validation": all_validation,
    "gate_failures": gate_failures,
    "meta_policy": "Only Partner_S4_L0.png and Partner_S5_L0.png were overwritten. Existing .meta files were not edited.",
}
with (redraw_dir / "hexcat_driftfox_baked_redraw_report.json").open("w", encoding="utf-8", newline="\n") as f:
    json.dump(report, f, ensure_ascii=False, indent=2)

if gate_failures:
    raise RuntimeError("Gate failures: " + "; ".join(gate_failures))

print("Redrew Hex Cat and Drift Fox with baked palettes.")
print(f"backup_dir={backup_dir}")
for item in reports:
    print(f"{item['file']}: size={item['size']} bbox={item['bbox']} bottom_y={item['bottom_y']} height={item['height']} corners={item['corner_alpha']} sample={item['sample']['color']} alpha={item['sample']['alpha']} colors={item['distinct_colors']} luma_range={item['luma_range']} components={item['component_count_alpha_gt_0']}")
'@

$script = $script.Replace("__OUT_DIR__", ($ResolvedOutDir -replace "\\", "\\"))
$script = $script.Replace("__RESOURCE_DIR__", ($ResourceDir -replace "\\", "\\"))

$tempScript = Join-Path $env:TEMP "redraw_hexcat_driftfox_baked_palette_$(Get-Random).py"
[System.IO.File]::WriteAllText($tempScript, $script, [System.Text.UTF8Encoding]::new($false))
try {
    & $PythonPath $tempScript
} finally {
    Remove-Item -LiteralPath $tempScript -Force -ErrorAction SilentlyContinue
}
