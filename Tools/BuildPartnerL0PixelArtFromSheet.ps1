param(
    [Parameter(Mandatory = $true)]
    [string]$Source,
    [string]$OutDir = "Assets\ArtSource\CharacterPixelArt_20260601",
    [string]$PythonPath = "C:\Users\kodai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe"
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ResolvedOutDir = if ([System.IO.Path]::IsPathRooted($OutDir)) { $OutDir } else { Join-Path $ProjectRoot $OutDir }
$ResourceDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ChromaHelper = "C:\Users\kodai\.codex\skills\.system\imagegen\scripts\remove_chroma_key.py"

New-Item -ItemType Directory -Force -Path $ResolvedOutDir | Out-Null

$ChromaCopy = Join-Path $ResolvedOutDir "CharacterPixelArt_6partners_source_chromakey.png"
$TransparentSheet = Join-Path $ResolvedOutDir "CharacterPixelArt_6partners_source_transparent.png"

Copy-Item -LiteralPath $Source -Destination $ChromaCopy -Force

& $PythonPath $ChromaHelper `
    --input $ChromaCopy `
    --out $TransparentSheet `
    --key-color "#ff00ff" `
    --soft-matte `
    --transparent-threshold 30 `
    --opaque-threshold 180 `
    --edge-contract 1 `
    --despill `
    --force | Out-Null

$script = @'
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import json
import shutil
import time

project_root = Path(r"__PROJECT_ROOT__")
out_dir = Path(r"__OUT_DIR__")
resource_dir = Path(r"__RESOURCE_DIR__")
source = Path(r"__TRANSPARENT_SHEET__")

names = [
    ("Partner_S1_L0.png", "Cobalt Pup", "blue cyber wolf pup"),
    ("Partner_S2_L0.png", "Ember Drake", "orange cyber drake"),
    ("Partner_S3_L0.png", "Sage Hare", "green cyber hare"),
    ("Partner_S4_L0.png", "Hex Cat", "purple cyber cat"),
    ("Partner_S5_L0.png", "Drift Fox", "pink cyber fox"),
    ("Partner_S6_L0.png", "Iron Bear", "black armored bear"),
]

out_dir.mkdir(parents=True, exist_ok=True)
resource_dir.mkdir(parents=True, exist_ok=True)

src = Image.open(source).convert("RGBA")
w, h = src.size
alpha = src.getchannel("A")

counts = []
for x in range(w):
    counts.append(sum(1 for y in range(h) if alpha.getpixel((x, y)) > 12))
active = [i for i, c in enumerate(counts) if c > 4]
if not active:
    raise RuntimeError("No active pixels found after chroma-key removal.")

intervals = []
start = prev = active[0]
for x in active[1:]:
    if x - prev <= 18:
        prev = x
    else:
        intervals.append([start, prev])
        start = prev = x
intervals.append([start, prev])
intervals = [iv for iv in intervals if iv[1] - iv[0] > 25]

while len(intervals) > 6:
    # Merge closest intervals. This preserves small attached magic/glitch bits.
    merge_i = min(range(len(intervals) - 1), key=lambda i: intervals[i + 1][0] - intervals[i][1])
    intervals[merge_i][1] = intervals[merge_i + 1][1]
    intervals.pop(merge_i + 1)

if len(intervals) != 6:
    raise RuntimeError(f"Expected 6 character intervals, found {len(intervals)}: {intervals}")

characters = []
for idx, iv in enumerate(intervals):
    x0 = max(0, iv[0] - 24)
    x1 = min(w, iv[1] + 24)
    cell = src.crop((x0, 0, x1, h))
    box = cell.getchannel("A").getbbox()
    if not box:
        raise RuntimeError(f"Empty character crop for {names[idx][1]}")
    bx0, by0, bx1, by1 = box
    pad_x = 24
    pad_y = 24
    crop = cell.crop((
        max(0, bx0 - pad_x),
        max(0, by0 - pad_y),
        min(cell.width, bx1 + pad_x),
        min(cell.height, by1 + pad_y),
    ))
    tight = crop.getchannel("A").getbbox()
    if not tight:
        raise RuntimeError(f"Empty tight crop for {names[idx][1]}")
    characters.append((crop, tight))

target_canvas = 512
target_baseline = 448
target_height = 350
rendered = []
metrics = []

for idx, (crop, tight) in enumerate(characters):
    _, _, _, tight_h = tight
    scale = target_height / max(1, tight_h)
    max_w = 430
    if crop.width * scale > max_w:
        scale = max_w / crop.width
    new_size = (max(1, int(round(crop.width * scale))), max(1, int(round(crop.height * scale))))
    resized = crop.resize(new_size, Image.Resampling.LANCZOS)
    rb = resized.getchannel("A").getbbox()
    if not rb:
        raise RuntimeError(f"Empty resized crop for {names[idx][1]}")
    rx0, ry0, rx1, ry1 = rb
    canvas = Image.new("RGBA", (target_canvas, target_canvas), (0, 0, 0, 0))
    x = (target_canvas - resized.width) // 2
    y = target_baseline - ry1
    y = max(12, min(target_canvas - resized.height - 4, y))
    canvas.alpha_composite(resized, (x, y))
    rendered.append(canvas)
    final_box = canvas.getchannel("A").getbbox()
    metrics.append({
        "file": names[idx][0],
        "name": names[idx][1],
        "description": names[idx][2],
        "canvas": [target_canvas, target_canvas],
        "bbox": list(final_box) if final_box else None,
        "bottom_y": final_box[3] if final_box else None,
        "height": (final_box[3] - final_box[1]) if final_box else None,
        "corner_alpha": [
            canvas.getpixel((0, 0))[3],
            canvas.getpixel((target_canvas - 1, 0))[3],
            canvas.getpixel((0, target_canvas - 1))[3],
            canvas.getpixel((target_canvas - 1, target_canvas - 1))[3],
        ],
    })

backup_dir = out_dir / ("backup_before_overwrite_" + time.strftime("%Y%m%d_%H%M%S"))
backup_dir.mkdir(exist_ok=True)

for (filename, display_name, _), image in zip(names, rendered):
    dest = resource_dir / filename
    if dest.exists():
        shutil.copy2(dest, backup_dir / filename)
    image.save(dest)
    art_copy = out_dir / filename
    image.save(art_copy)

preview_bg = (5, 12, 16, 255)
cyan = (117, 239, 255, 255)
muted = (130, 175, 178, 255)
gold = (255, 214, 80, 255)
try:
    font = ImageFont.truetype("arial.ttf", 20)
    small = ImageFont.truetype("arial.ttf", 15)
except Exception:
    font = ImageFont.load_default()
    small = ImageFont.load_default()

cell_w = 256
cell_h = 276
preview = Image.new("RGBA", (cell_w * 6, cell_h), preview_bg)
draw = ImageDraw.Draw(preview)
baseline = 224
draw.line((16, baseline, preview.width - 16, baseline), fill=(20, 75, 82, 255), width=1)
for idx, ((_, display_name, _), image) in enumerate(zip(names, rendered)):
    box = image.getchannel("A").getbbox()
    sample = image.crop(box) if box else image
    scale = min(180 / sample.width, 178 / sample.height)
    scaled = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.LANCZOS)
    x = idx * cell_w + (cell_w - scaled.width) // 2
    y = baseline - scaled.height
    preview.alpha_composite(scaled, (x, y))
    draw.text((idx * cell_w + 10, 236), display_name, font=small, fill=cyan)
    draw.rectangle((idx * cell_w + 4, 4, idx * cell_w + cell_w - 4, cell_h - 4), outline=(19, 94, 105, 160), width=1)
preview.save(out_dir / "CharacterPixelArt_6partners_lineup_preview.png")

clean = Image.new("RGBA", (512 * 6, 512), (0, 0, 0, 0))
for idx, image in enumerate(rendered):
    clean.alpha_composite(image, (idx * 512, 0))
clean.save(out_dir / "CharacterPixelArt_6partners_lineup_clean_transparent.png")

sizes = [64, 96, 128]
shrink = Image.new("RGBA", (180 * 6, 150 * len(sizes)), preview_bg)
draw = ImageDraw.Draw(shrink)
for r, size in enumerate(sizes):
    draw.text((8, r * 150 + 8), f"{size}px", font=small, fill=gold)
    for c, ((_, display_name, _), image) in enumerate(zip(names, rendered)):
        box = image.getchannel("A").getbbox()
        sample = image.crop(box) if box else image
        scale = size / max(sample.width, sample.height)
        scaled = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.NEAREST)
        x = c * 180 + (180 - scaled.width) // 2
        y = r * 150 + 32 + (94 - scaled.height) // 2
        shrink.alpha_composite(scaled, (x, y))
        draw.text((c * 180 + 8, r * 150 + 124), display_name.split()[0], font=small, fill=muted)
        draw.rectangle((c * 180 + 4, r * 150 + 24, c * 180 + 176, r * 150 + 146), outline=(19, 94, 105, 160), width=1)
shrink.save(out_dir / "CharacterPixelArt_6partners_shrink_check.png")

with (out_dir / "validation_report.json").open("w", encoding="utf-8", newline="\n") as f:
    json.dump({
        "source": str(source),
        "source_size": [w, h],
        "intervals": intervals,
        "backup_dir": str(backup_dir),
        "metrics": metrics,
        "meta_policy": "PNG files overwritten only; existing .meta files intentionally untouched.",
    }, f, ensure_ascii=False, indent=2)

print("Built partner L0 pixel art assets.")
print(f"source_size={w}x{h}")
print(f"out_dir={out_dir}")
print(f"backup_dir={backup_dir}")
for m in metrics:
    print(f"{m['file']}: bbox={m['bbox']} bottom_y={m['bottom_y']} height={m['height']} corners={m['corner_alpha']}")
'@

$script = $script.Replace("__PROJECT_ROOT__", ($ProjectRoot -replace "\\", "\\"))
$script = $script.Replace("__OUT_DIR__", ($ResolvedOutDir -replace "\\", "\\"))
$script = $script.Replace("__RESOURCE_DIR__", ($ResourceDir -replace "\\", "\\"))
$script = $script.Replace("__TRANSPARENT_SHEET__", ($TransparentSheet -replace "\\", "\\"))

$tempScript = Join-Path $env:TEMP "build_partner_l0_pixelart_$(Get-Random).py"
[System.IO.File]::WriteAllText($tempScript, $script, [System.Text.UTF8Encoding]::new($false))
try {
    & $PythonPath $tempScript
} finally {
    Remove-Item -LiteralPath $tempScript -Force -ErrorAction SilentlyContinue
}
