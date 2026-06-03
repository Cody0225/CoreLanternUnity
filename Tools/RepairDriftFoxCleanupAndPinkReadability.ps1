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
from collections import deque
import json
import shutil
import time

out_dir = Path(r"__OUT_DIR__")
resource_dir = Path(r"__RESOURCE_DIR__")
repair_dir = out_dir / "drift_fox_cleanup_20260602"
backup_dir = repair_dir / ("backup_before_drift_fox_cleanup_" + time.strftime("%Y%m%d_%H%M%S"))
repair_dir.mkdir(parents=True, exist_ok=True)
backup_dir.mkdir(parents=True, exist_ok=True)

target = resource_dir / "Partner_S5_L0.png"
if not target.exists():
    raise FileNotFoundError(target)

shutil.copy2(target, backup_dir / target.name)

img = Image.open(target).convert("RGBA")
w, h = img.size

def clamp(v):
    return max(0, min(255, int(round(v))))

def blend(src, dst, t):
    return (
        clamp(src[0] + (dst[0] - src[0]) * t),
        clamp(src[1] + (dst[1] - src[1]) * t),
        clamp(src[2] + (dst[2] - src[2]) * t),
    )

def connected_components(im, alpha_threshold=8):
    pix = im.load()
    seen = set()
    components = []
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
                components.append(pts)
    components.sort(key=len, reverse=True)
    return components

before_components = connected_components(img)
component_summary_before = []
for pts in before_components:
    xs = [p[0] for p in pts]
    ys = [p[1] for p in pts]
    component_summary_before.append({
        "size": len(pts),
        "bbox": [min(xs), min(ys), max(xs) + 1, max(ys) + 1],
    })

pix = img.load()
main_pixels = set(before_components[0]) if before_components else set()
removed_pixels = 0

for pts in before_components[1:]:
    for x, y in pts:
        pix[x, y] = (0, 0, 0, 0)
        removed_pixels += 1

changed_pixels = 0
alpha_raised_pixels = 0
pink_adjusted_pixels = 0

for x, y in main_pixels:
    r, g, b, a = pix[x, y]
    if a <= 0:
        continue

    # Fully drop barely visible anti-chroma haze, then strengthen real body pixels.
    if a < 10:
        pix[x, y] = (0, 0, 0, 0)
        changed_pixels += 1
        continue
    if a >= 58:
        na = 255
    elif a >= 28:
        na = max(184, int(a * 3.1))
    else:
        na = max(112, int(a * 4.0))
    na = clamp(na)

    # Restore the "pink fox" read without repainting the silhouette:
    # target only magenta/purple or low-saturation rose armor/body pixels.
    mx, mn = max(r, g, b), min(r, g, b)
    lum = (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0
    nr, ng, nb = r, g, b

    is_pink_family = (r > g + 18 and b > g + 8 and r > 88)
    is_rose_gray = (mx - mn < 58 and 0.16 < lum < 0.78 and r >= g - 4 and b >= g - 2)
    is_dark_plum = (b >= r - 18 and r >= g + 2 and lum < 0.34)
    is_white_fur = (mx > 220 and mx - mn < 72)
    is_gold_core = (r > 158 and g > 96 and b < 88)

    if is_gold_core:
        nr, ng, nb = r, g, b
    elif is_white_fur:
        nr, ng, nb = blend((r, g, b), (255, 222, 241), 0.10)
        pink_adjusted_pixels += 1
    elif is_pink_family:
        nr, ng, nb = blend((r, g, b), (255, 92, 183), 0.30)
        nr, ng, nb = clamp(nr + 8), clamp(ng + 1), clamp(nb + 8)
        pink_adjusted_pixels += 1
    elif is_rose_gray:
        nr, ng, nb = blend((r, g, b), (185, 92, 150), 0.26)
        pink_adjusted_pixels += 1
    elif is_dark_plum:
        nr, ng, nb = blend((r, g, b), (112, 45, 92), 0.30)
        pink_adjusted_pixels += 1

    if na != a:
        alpha_raised_pixels += 1
    if (nr, ng, nb, na) != (r, g, b, a):
        changed_pixels += 1
    pix[x, y] = (nr, ng, nb, na)

# Remove barely visible chroma-key haze outside the actual fox body. The main
# body is expanded a little so antialiased attached edges are preserved.
keep_pixels = set()
for x, y in main_pixels:
    for dx in range(-3, 4):
        for dy in range(-3, 4):
            nx, ny = x + dx, y + dy
            if 0 <= nx < w and 0 <= ny < h:
                keep_pixels.add((nx, ny))

low_alpha_outside_body_removed = 0
for y in range(h):
    for x in range(w):
        if pix[x, y][3] > 0 and (x, y) not in keep_pixels:
            pix[x, y] = (0, 0, 0, 0)
            low_alpha_outside_body_removed += 1

# Final hard gate: after the pink/readability pass, keep only the largest
# alpha>0 connected component. This removes any detached square scraps or hairline
# artifacts that are still too faint to be caught by the alpha>8 pass.
def all_alpha_components(im):
    pix2 = im.load()
    seen2 = set()
    comps2 = []
    for sy in range(h):
        for sx in range(w):
            if (sx, sy) in seen2:
                continue
            if pix2[sx, sy][3] <= 0:
                seen2.add((sx, sy))
                continue
            q = deque([(sx, sy)])
            seen2.add((sx, sy))
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
                        if (nx, ny) in seen2:
                            continue
                        seen2.add((nx, ny))
                        if pix2[nx, ny][3] > 0:
                            q.append((nx, ny))
            comps2.append(pts)
    comps2.sort(key=len, reverse=True)
    return comps2

detached_alpha_components_removed = 0
final_components = all_alpha_components(img)
for pts in final_components[1:]:
    for x, y in pts:
        pix[x, y] = (0, 0, 0, 0)
        detached_alpha_components_removed += 1

# Realign to the shared title baseline after detached artifacts are removed.
target_baseline = 448
box = img.getchannel("A").getbbox()
baseline_shift = 0
if box:
    baseline_shift = target_baseline - box[3]
    if baseline_shift:
        shifted = Image.new("RGBA", img.size, (0, 0, 0, 0))
        shifted.alpha_composite(img, (0, baseline_shift))
        img = shifted

img.save(target)
img.save(out_dir / "Partner_S5_L0.png")

names = [
    ("Partner_S1_L0.png", "Cobalt Pup"),
    ("Partner_S2_L0.png", "Ember Drake"),
    ("Partner_S3_L0.png", "Sage Hare"),
    ("Partner_S4_L0.png", "Hex Cat"),
    ("Partner_S5_L0.png", "Drift Fox"),
    ("Partner_S6_L0.png", "Iron Bear"),
]

def load_runtime_images():
    images = []
    for filename, label in names:
        im = Image.open(resource_dir / filename).convert("RGBA")
        images.append((filename, label, im))
    return images

def bbox_image(im):
    box = im.getchannel("A").getbbox()
    if not box:
        return im
    return im.crop(box)

def write_previews(images):
    preview_bg = (5, 12, 16, 255)
    cyan = (117, 239, 255, 255)
    muted = (130, 175, 178, 255)
    gold = (255, 214, 80, 255)
    try:
        small = ImageFont.truetype("arial.ttf", 15)
    except Exception:
        small = ImageFont.load_default()

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

def write_visibility_check(after_img):
    bgs = [(5, 12, 16, 255), (31, 45, 63, 255), (92, 104, 112, 255), (230, 230, 230, 255)]
    canvas = Image.new("RGBA", (512 * 4, 560), (0, 0, 0, 0))
    draw = ImageDraw.Draw(canvas)
    try:
        small = ImageFont.truetype("arial.ttf", 16)
    except Exception:
        small = ImageFont.load_default()
    for i, bg in enumerate(bgs):
        tile = Image.new("RGBA", (512, 512), bg)
        tile.alpha_composite(after_img, (0, 0))
        canvas.alpha_composite(tile, (i * 512, 0))
        draw.text((i * 512 + 12, 520), str(bg[:3]), font=small, fill=(255, 255, 255, 255))
    canvas.save(repair_dir / "DriftFox_visibility_check_after.png")

images = load_runtime_images()
write_previews(images)
write_visibility_check(Image.open(target).convert("RGBA"))

validation = []
for filename, label, im in images:
    box = im.getchannel("A").getbbox()
    validation.append({
        "file": filename,
        "label": label,
        "size": list(im.size),
        "bbox": list(box) if box else None,
        "bottom_y": box[3] if box else None,
        "height": (box[3] - box[1]) if box else None,
        "corner_alpha": [
            im.getpixel((0, 0))[3],
            im.getpixel((im.width - 1, 0))[3],
            im.getpixel((0, im.height - 1))[3],
            im.getpixel((im.width - 1, im.height - 1))[3],
        ],
    })

after = Image.open(target).convert("RGBA")
after_components = connected_components(after)
component_summary_after = []
for pts in after_components:
    xs = [p[0] for p in pts]
    ys = [p[1] for p in pts]
    component_summary_after.append({
        "size": len(pts),
        "bbox": [min(xs), min(ys), max(xs) + 1, max(ys) + 1],
    })
visible = [a for a in after.getchannel("A").getdata() if a > 0]

report = {
    "backup_dir": str(backup_dir),
    "file": str(target),
    "components_before": component_summary_before,
    "components_after": component_summary_after,
    "removed_pixels": removed_pixels,
    "changed_pixels": changed_pixels,
    "alpha_raised_pixels": alpha_raised_pixels,
    "pink_adjusted_pixels": pink_adjusted_pixels,
    "low_alpha_outside_body_removed": low_alpha_outside_body_removed,
    "detached_alpha_components_removed": detached_alpha_components_removed,
    "baseline_shift": baseline_shift,
    "final_alpha_min": min(visible) if visible else None,
    "final_alpha_max": max(visible) if visible else None,
    "validation": validation,
    "meta_policy": "Only Partner_S5_L0.png was updated. Existing .meta files were not edited.",
}

with (repair_dir / "drift_fox_cleanup_report.json").open("w", encoding="utf-8", newline="\n") as f:
    json.dump(report, f, ensure_ascii=False, indent=2)

print("Repaired Drift Fox cleanup/pink readability.")
print(f"backup_dir={backup_dir}")
print(f"removed_pixels={removed_pixels}")
print(f"changed_pixels={changed_pixels}")
print(f"alpha_raised_pixels={alpha_raised_pixels}")
print(f"pink_adjusted_pixels={pink_adjusted_pixels}")
print(f"low_alpha_outside_body_removed={low_alpha_outside_body_removed}")
print(f"detached_alpha_components_removed={detached_alpha_components_removed}")
print(f"baseline_shift={baseline_shift}")
for item in validation:
    if item["file"] == "Partner_S5_L0.png":
        print(f"{item['file']}: size={item['size']} bbox={item['bbox']} bottom_y={item['bottom_y']} height={item['height']} corners={item['corner_alpha']}")
'@

$script = $script.Replace("__OUT_DIR__", ($ResolvedOutDir -replace "\\", "\\"))
$script = $script.Replace("__RESOURCE_DIR__", ($ResourceDir -replace "\\", "\\"))

$tempScript = Join-Path $env:TEMP "repair_drift_fox_cleanup_$(Get-Random).py"
[System.IO.File]::WriteAllText($tempScript, $script, [System.Text.UTF8Encoding]::new($false))
try {
    & $PythonPath $tempScript
} finally {
    Remove-Item -LiteralPath $tempScript -Force -ErrorAction SilentlyContinue
}
