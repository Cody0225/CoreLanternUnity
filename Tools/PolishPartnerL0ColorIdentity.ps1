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
import colorsys
import json
import shutil
import time

out_dir = Path(r"__OUT_DIR__")
resource_dir = Path(r"__RESOURCE_DIR__")
polish_dir = out_dir / "color_identity_polish_20260602"
backup_dir = polish_dir / ("backup_before_polish_" + time.strftime("%Y%m%d_%H%M%S"))
polish_dir.mkdir(parents=True, exist_ok=True)
backup_dir.mkdir(parents=True, exist_ok=True)

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

def blend(src, dst, t):
    return (
        clamp(src[0] + (dst[0] - src[0]) * t),
        clamp(src[1] + (dst[1] - src[1]) * t),
        clamp(src[2] + (dst[2] - src[2]) * t),
    )

def recolor_hex_cat(img):
    px = img.load()
    changed = 0
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = px[x, y]
            if a < 8:
                continue
            mx = max(r, g, b)
            mn = min(r, g, b)
            if mx < 24:
                continue
            rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
            h, l, s = colorsys.rgb_to_hls(rf, gf, bf)
            target = (
                86 + 70 * l,
                45 + 58 * l,
                166 + 72 * l,
            )
            if mx > 224 and (mx - mn) < 36:
                t = 0.07
            elif mx < 58:
                t = 0.16
            elif s < 0.22:
                t = 0.46
            elif 0.58 <= h <= 0.82:
                t = 0.30
            else:
                t = 0.18
            nr, ng, nb = blend((r, g, b), target, t)
            if (nr, ng, nb) != (r, g, b):
                px[x, y] = (nr, ng, nb, a)
                changed += 1
    return changed

def recolor_drift_fox(img):
    px = img.load()
    changed = 0
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = px[x, y]
            if a < 8:
                continue
            mx = max(r, g, b)
            mn = min(r, g, b)
            if mx < 24:
                continue
            rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
            h, l, s = colorsys.rgb_to_hls(rf, gf, bf)
            # Keep gold core accents mostly intact.
            if r > 160 and g > 110 and b < 92:
                target = (255, 166, 86)
                t = 0.08
            # Warm white fur instead of blue-white.
            elif mx > 218 and (mx - mn) < 56:
                target = (255, 230, 244)
                t = 0.18
            # Convert cyan/blue tech glows into pink fox-phase glows.
            elif (b > r + 22 and g > r + 12) or (0.50 <= h <= 0.68 and s > 0.18):
                target = (255, 105, 194)
                t = 0.70
            # Gray armor/body becomes rose-plum, preserving value.
            elif s < 0.28:
                target = (
                    126 + 82 * l,
                    54 + 74 * l,
                    96 + 98 * l,
                )
                t = 0.48
            else:
                target = (238, 88, 166)
                t = 0.28
            nr, ng, nb = blend((r, g, b), target, t)
            if (nr, ng, nb) != (r, g, b):
                px[x, y] = (nr, ng, nb, a)
                changed += 1
    return changed

def save_png(path, img):
    img.save(path)

report = {"backup_dir": str(backup_dir), "changes": []}

for filename, label in names:
    src = resource_dir / filename
    if not src.exists():
        raise FileNotFoundError(src)
    if filename in ("Partner_S4_L0.png", "Partner_S5_L0.png"):
        shutil.copy2(src, backup_dir / filename)
        img = Image.open(src).convert("RGBA")
        if filename == "Partner_S4_L0.png":
            changed = recolor_hex_cat(img)
        else:
            changed = recolor_drift_fox(img)
        save_png(src, img)
        save_png(out_dir / filename, img)
        report["changes"].append({"file": filename, "label": label, "pixels_changed": changed})

def load_runtime_images():
    images = []
    for filename, label in names:
        img = Image.open(resource_dir / filename).convert("RGBA")
        images.append((filename, label, img))
    return images

def bbox_image(img):
    box = img.getchannel("A").getbbox()
    if not box:
        return img
    return img.crop(box)

def write_previews(images):
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
    baseline = 224
    preview = Image.new("RGBA", (cell_w * 6, cell_h), preview_bg)
    draw = ImageDraw.Draw(preview)
    draw.line((16, baseline, preview.width - 16, baseline), fill=(20, 75, 82, 255), width=1)
    for idx, (_, label, image) in enumerate(images):
        sample = bbox_image(image)
        scale = min(180 / sample.width, 178 / sample.height)
        scaled = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.LANCZOS)
        x = idx * cell_w + (cell_w - scaled.width) // 2
        y = baseline - scaled.height
        preview.alpha_composite(scaled, (x, y))
        draw.text((idx * cell_w + 10, 236), label, font=small, fill=cyan)
        draw.rectangle((idx * cell_w + 4, 4, idx * cell_w + cell_w - 4, cell_h - 4), outline=(19, 94, 105, 160), width=1)
    preview.save(out_dir / "CharacterPixelArt_6partners_lineup_preview.png")

    clean = Image.new("RGBA", (512 * 6, 512), (0, 0, 0, 0))
    for idx, (_, _, image) in enumerate(images):
        clean.alpha_composite(image, (idx * 512, 0))
    clean.save(out_dir / "CharacterPixelArt_6partners_lineup_clean_transparent.png")

    sizes = [64, 96, 128]
    shrink = Image.new("RGBA", (180 * 6, 150 * len(sizes)), preview_bg)
    draw = ImageDraw.Draw(shrink)
    for r, size in enumerate(sizes):
        draw.text((8, r * 150 + 8), f"{size}px", font=small, fill=gold)
        for c, (_, label, image) in enumerate(images):
            sample = bbox_image(image)
            scale = size / max(sample.width, sample.height)
            scaled = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.NEAREST)
            x = c * 180 + (180 - scaled.width) // 2
            y = r * 150 + 32 + (94 - scaled.height) // 2
            shrink.alpha_composite(scaled, (x, y))
            draw.text((c * 180 + 8, r * 150 + 124), label.split()[0], font=small, fill=muted)
            draw.rectangle((c * 180 + 4, r * 150 + 24, c * 180 + 176, r * 150 + 146), outline=(19, 94, 105, 160), width=1)
    shrink.save(out_dir / "CharacterPixelArt_6partners_shrink_check.png")

write_previews(load_runtime_images())

validation = []
for filename, label, image in load_runtime_images():
    box = image.getchannel("A").getbbox()
    validation.append({
        "file": filename,
        "label": label,
        "size": list(image.size),
        "bbox": list(box) if box else None,
        "bottom_y": box[3] if box else None,
        "height": (box[3] - box[1]) if box else None,
        "corner_alpha": [
            image.getpixel((0, 0))[3],
            image.getpixel((image.width - 1, 0))[3],
            image.getpixel((0, image.height - 1))[3],
            image.getpixel((image.width - 1, image.height - 1))[3],
        ],
    })

report["validation"] = validation
report["meta_policy"] = "Only PNG files were updated. Existing .meta files were not edited."
with (polish_dir / "color_identity_polish_report.json").open("w", encoding="utf-8", newline="\n") as f:
    json.dump(report, f, ensure_ascii=False, indent=2)

print("Polished partner color identity.")
print(f"backup_dir={backup_dir}")
for item in report["changes"]:
    print(f"{item['file']}: pixels_changed={item['pixels_changed']}")
for item in validation:
    print(f"{item['file']}: size={item['size']} bbox={item['bbox']} bottom_y={item['bottom_y']} height={item['height']} corners={item['corner_alpha']}")
'@

$script = $script.Replace("__OUT_DIR__", ($ResolvedOutDir -replace "\\", "\\"))
$script = $script.Replace("__RESOURCE_DIR__", ($ResourceDir -replace "\\", "\\"))

$tempScript = Join-Path $env:TEMP "polish_partner_l0_color_identity_$(Get-Random).py"
[System.IO.File]::WriteAllText($tempScript, $script, [System.Text.UTF8Encoding]::new($false))
try {
    & $PythonPath $tempScript
} finally {
    Remove-Item -LiteralPath $tempScript -Force -ErrorAction SilentlyContinue
}
