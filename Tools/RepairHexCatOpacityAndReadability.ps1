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
repair_dir = out_dir / "hex_cat_opacity_repair_20260602"
backup_dir = repair_dir / ("backup_before_hex_opacity_repair_" + time.strftime("%Y%m%d_%H%M%S"))
repair_dir.mkdir(parents=True, exist_ok=True)
backup_dir.mkdir(parents=True, exist_ok=True)

target = resource_dir / "Partner_S4_L0.png"
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

def remove_tiny_noise(im, min_area=160):
    pix = im.load()
    seen = set()
    removed = 0
    components = 0
    for sy in range(h):
        for sx in range(w):
            if (sx, sy) in seen:
                continue
            if pix[sx, sy][3] <= 8:
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
                        if pix[nx, ny][3] > 8:
                            q.append((nx, ny))
            components += 1
            xs = [p[0] for p in pts]
            ys = [p[1] for p in pts]
            # Keep body, eyes, bubbles, and tail ornaments; remove tiny chroma-key scraps.
            if len(pts) < min_area:
                for x, y in pts:
                    pix[x, y] = (0, 0, 0, 0)
                removed += len(pts)
    return {"components_seen": components, "pixels_removed": removed}

noise_report = remove_tiny_noise(img)
pix = img.load()
changed = 0
alpha_raised = 0

for y in range(h):
    for x in range(w):
        r, g, b, a = pix[x, y]
        if a <= 0:
            continue
        if a < 10:
            pix[x, y] = (0, 0, 0, 0)
            changed += 1
            continue

        lum = (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0
        mx = max(r, g, b)
        mn = min(r, g, b)

        # Keep black eyes readable, but make almost-transparent black scraps solid enough.
        if mx < 22 and a > 90:
            nr, ng, nb = r, g, b
        elif lum < 0.16:
            nr, ng, nb = blend((r, g, b), (72, 38, 116), 0.56)
            nr, ng, nb = clamp(nr + 8), clamp(ng + 5), clamp(nb + 14)
        elif lum < 0.38:
            nr, ng, nb = blend((r, g, b), (130, 82, 195), 0.50)
            nr, ng, nb = clamp(nr + 10), clamp(ng + 6), clamp(nb + 16)
        elif mx - mn < 42 and lum < 0.78:
            nr, ng, nb = blend((r, g, b), (176, 136, 238), 0.42)
        elif lum < 0.78:
            nr, ng, nb = blend((r, g, b), (156, 96, 226), 0.26)
        else:
            nr, ng, nb = blend((r, g, b), (224, 204, 255), 0.16)

        if a >= 64:
            na = 255
        elif a >= 28:
            na = max(180, int(a * 3.0))
        else:
            na = max(96, int(a * 4.0))
        na = clamp(na)

        if na != a:
            alpha_raised += 1
        if (nr, ng, nb, na) != (r, g, b, a):
            changed += 1
        pix[x, y] = (nr, ng, nb, na)

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
img.save(out_dir / "Partner_S4_L0.png")

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

def write_visibility_check(before_path, after_img):
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
    canvas.save(repair_dir / "HexCat_visibility_check_after.png")

write_previews(load_runtime_images())
write_visibility_check(backup_dir / target.name, img)

validation = []
for filename, label, im in load_runtime_images():
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

visible = [a for a in img.getchannel("A").getdata() if a > 0]
report = {
    "backup_dir": str(backup_dir),
    "file": str(target),
    "changed_pixels": changed,
    "alpha_raised_pixels": alpha_raised,
    "baseline_shift": baseline_shift,
    "noise_report": noise_report,
    "hex_alpha_min": min(visible) if visible else None,
    "hex_alpha_max": max(visible) if visible else None,
    "validation": validation,
    "meta_policy": "Only Partner_S4_L0.png was updated. Existing .meta files were not edited.",
}

with (repair_dir / "hex_cat_opacity_repair_report.json").open("w", encoding="utf-8", newline="\n") as f:
    json.dump(report, f, ensure_ascii=False, indent=2)

print("Repaired Hex Cat opacity/readability.")
print(f"backup_dir={backup_dir}")
print(f"changed_pixels={changed}")
print(f"alpha_raised_pixels={alpha_raised}")
print(f"baseline_shift={baseline_shift}")
print(f"noise={noise_report}")
print(f"hex_alpha_min={report['hex_alpha_min']} hex_alpha_max={report['hex_alpha_max']}")
for item in validation:
    if item["file"] == "Partner_S4_L0.png":
        print(f"{item['file']}: size={item['size']} bbox={item['bbox']} bottom_y={item['bottom_y']} height={item['height']} corners={item['corner_alpha']}")
'@

$script = $script.Replace("__OUT_DIR__", ($ResolvedOutDir -replace "\\", "\\"))
$script = $script.Replace("__RESOURCE_DIR__", ($ResourceDir -replace "\\", "\\"))

$tempScript = Join-Path $env:TEMP "repair_hex_cat_opacity_$(Get-Random).py"
[System.IO.File]::WriteAllText($tempScript, $script, [System.Text.UTF8Encoding]::new($false))
try {
    & $PythonPath $tempScript
} finally {
    Remove-Item -LiteralPath $tempScript -Force -ErrorAction SilentlyContinue
}
