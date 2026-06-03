param(
    [Parameter(Mandatory = $true)]
    [string]$Source,
    [Parameter(Mandatory = $true)]
    [string]$Slug,
    [Parameter(Mandatory = $true)]
    [string]$DisplayName,
    [Parameter(Mandatory = $true)]
    [string]$OutDir,
    [string]$BeforeImage = "",
    [string]$PythonPath = "C:\Users\kodai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe"
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ResolvedOutDir = if ([System.IO.Path]::IsPathRooted($OutDir)) { $OutDir } else { Join-Path $ProjectRoot $OutDir }
New-Item -ItemType Directory -Force -Path $ResolvedOutDir | Out-Null

$ChromaCopy = Join-Path $ResolvedOutDir "$($Slug)_MOTION_REVIEW_v1_sheet_chromakey.png"
$CleanSource = Join-Path $ResolvedOutDir "$($Slug)_MOTION_REVIEW_v1_sheet_transparent_clean.png"
$ChromaHelper = "C:\Users\kodai\.codex\skills\.system\imagegen\scripts\remove_chroma_key.py"

Copy-Item -LiteralPath $Source -Destination $ChromaCopy -Force
& $PythonPath $ChromaHelper `
    --input $ChromaCopy `
    --out $CleanSource `
    --key-color "#ff00ff" `
    --soft-matte `
    --transparent-threshold 34 `
    --opaque-threshold 180 `
    --edge-contract 1 `
    --despill `
    --force | Out-Null

$script = @'
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

source = Path(r"__SOURCE__")
out_dir = Path(r"__OUT_DIR__")
project_root = Path(r"__PROJECT_ROOT__")
before_image = Path(r"__BEFORE_IMAGE__") if r"__BEFORE_IMAGE__" else None
slug = "__SLUG__"
display_name = "__DISPLAY_NAME__"
out_dir.mkdir(parents=True, exist_ok=True)

src = Image.open(source).convert("RGBA")
w, h = src.size
transparent_sheet = out_dir / f"{slug}_MOTION_REVIEW_v1_sheet_transparent.png"
src.save(transparent_sheet)

frames_dir = out_dir / "frames"
frames_dir.mkdir(exist_ok=True)

BG = (5, 12, 16, 255)
PANEL = (3, 20, 27, 255)
GRID = (32, 104, 118, 255)
CYAN = (94, 236, 255, 255)
YELLOW = (255, 215, 92, 255)
TEXT = (190, 235, 240, 255)

try:
    font = ImageFont.truetype("arial.ttf", 18)
    small = ImageFont.truetype("arial.ttf", 13)
except Exception:
    font = ImageFont.load_default()
    small = ImageFont.load_default()

def detect_intervals(row_index, expected):
    row_h = h / 2.0
    y0 = int(row_index * row_h)
    y1 = int((row_index + 1) * row_h)
    alpha = src.getchannel("A")
    counts = []
    for x in range(w):
        counts.append(sum(1 for y in range(y0, y1) if alpha.getpixel((x, y)) > 20))
    active = [i for i, c in enumerate(counts) if c > 3]
    if not active:
        raise RuntimeError(f"no active pixels for row {row_index}")
    intervals = []
    s = prev = active[0]
    for x in active[1:]:
        if x - prev <= 12:
            prev = x
        else:
            intervals.append([s, prev])
            s = prev = x
    intervals.append([s, prev])
    intervals = [iv for iv in intervals if iv[1] - iv[0] + 1 > 10]

    while len(intervals) > expected:
        small_i = min(range(len(intervals)), key=lambda i: intervals[i][1] - intervals[i][0] + 1)
        small_w = intervals[small_i][1] - intervals[small_i][0] + 1
        if small_w < 70:
            if small_i == 0:
                intervals[1][0] = min(intervals[1][0], intervals[0][0])
                intervals.pop(0)
            elif small_i == len(intervals) - 1:
                intervals[-2][1] = max(intervals[-2][1], intervals[-1][1])
                intervals.pop()
            else:
                gap_prev = intervals[small_i][0] - intervals[small_i - 1][1]
                gap_next = intervals[small_i + 1][0] - intervals[small_i][1]
                if gap_prev <= gap_next:
                    intervals[small_i - 1][1] = max(intervals[small_i - 1][1], intervals[small_i][1])
                    intervals.pop(small_i)
                else:
                    intervals[small_i + 1][0] = min(intervals[small_i + 1][0], intervals[small_i][0])
                    intervals.pop(small_i)
        else:
            pair_i = min(range(len(intervals) - 1), key=lambda i: intervals[i + 1][0] - intervals[i][1])
            intervals[pair_i][1] = intervals[pair_i + 1][1]
            intervals.pop(pair_i + 1)
    if len(intervals) != expected:
        raise RuntimeError(f"row {row_index} expected {expected} intervals, found {len(intervals)}: {intervals}")
    return [(max(0, s - 10), min(w, e + 10), y0, y1) for s, e in intervals]

def crop_interval(interval, pad=14):
    x0, x1, y0, y1 = interval
    cell = src.crop((x0, y0, x1, y1))
    box = cell.getchannel("A").getbbox()
    if not box:
        raise RuntimeError(f"empty interval {interval}")
    bx0, by0, bx1, by1 = box
    bx0 = max(0, bx0 - pad)
    by0 = max(0, by0 - pad)
    bx1 = min(cell.width, bx1 + pad)
    by1 = min(cell.height, by1 + pad)
    return cell.crop((bx0, by0, bx1, by1))

run_frames = [crop_interval(iv) for iv in detect_intervals(0, 6)]
attack_frames = [crop_interval(iv) for iv in detect_intervals(1, 5)]

for i, fr in enumerate(run_frames, 1):
    fr.save(frames_dir / f"{slug}_MOTION_REVIEW_v1_RUN_{i:02}.png")
for i, fr in enumerate(attack_frames, 1):
    fr.save(frames_dir / f"{slug}_MOTION_REVIEW_v1_ATTACK_{i:02}.png")

def render_gif(frames, out_name, label, duration=95, canvas=(440, 300), mode="center"):
    rendered = []
    for fr in frames:
        bg = Image.new("RGBA", canvas, BG)
        d = ImageDraw.Draw(bg)
        d.text((16, 12), label, font=font, fill=CYAN if "RUN" in label else YELLOW)
        ground = canvas[1] - 38
        d.line((28, ground, canvas[0] - 28, ground), fill=(22, 80, 92, 255), width=1)
        x = 30 if mode == "left" else (canvas[0] - fr.width) // 2
        y = ground - fr.height + 8
        bg.alpha_composite(fr, (x, y))
        rendered.append(bg.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=220))
    rendered[0].save(out_dir / out_name, save_all=True, append_images=rendered[1:], duration=duration, loop=0)

render_gif(run_frames, f"{slug}_MOTION_REVIEW_v1_RUN.gif", f"{display_name} / MOTION REVIEW / RUN", 90)
render_gif(attack_frames, f"{slug}_MOTION_REVIEW_v1_ATTACK.gif", f"{display_name} / MOTION REVIEW / ATTACK", 110, canvas=(560, 300), mode="left")

def shrink_check():
    samples = [run_frames[1], run_frames[3], attack_frames[1], attack_frames[2]]
    labels = ["RUN push A", "RUN land B", "ATK charge", "ATK fire"]
    sizes = [64, 96, 128]
    cell_w, cell_h = 190, 165
    sheet = Image.new("RGBA", (cell_w * len(samples), cell_h * len(sizes)), BG)
    d = ImageDraw.Draw(sheet)
    for r, size in enumerate(sizes):
        d.text((8, r * cell_h + 8), f"{size}px check", font=small, fill=TEXT)
        for c, sample in enumerate(samples):
            scale = size / max(sample.width, sample.height)
            resized = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.NEAREST)
            x = c * cell_w + (cell_w - resized.width) // 2
            y = r * cell_h + 34 + (cell_h - 44 - resized.height) // 2
            sheet.alpha_composite(resized, (x, y))
            d.rectangle((c * cell_w + 8, r * cell_h + 28, c * cell_w + cell_w - 8, r * cell_h + cell_h - 8), outline=GRID, width=1)
            d.text((c * cell_w + 12, r * cell_h + cell_h - 26), labels[c], font=small, fill=TEXT)
    sheet.save(out_dir / f"{slug}_MOTION_REVIEW_v1_shrink_check.png")

def before_after():
    if before_image is None or not before_image.exists():
        return
    old = Image.open(before_image).convert("RGBA")
    box = old.getchannel("A").getbbox()
    if box:
        pad = 8
        x0, y0, x1, y1 = box
        old = old.crop((max(0, x0 - pad), max(0, y0 - pad), min(old.width, x1 + pad), min(old.height, y1 + pad)))
    new_samples = [run_frames[1], run_frames[3], attack_frames[1], attack_frames[2]]
    labels = ["RUN A", "RUN B", "ATK charge", "ATK fire"]
    cell_w, cell_h = 260, 230
    canvas = Image.new("RGBA", (cell_w * 4, cell_h * 2 + 50), BG)
    d = ImageDraw.Draw(canvas)
    d.text((16, 12), "BEFORE: current static skin / AFTER: new motion review", font=font, fill=TEXT)
    for c in range(4):
        d.text((c * cell_w + 16, 42), labels[c], font=small, fill=TEXT)
        for r, sample in enumerate([old, new_samples[c]]):
            scale = min(1.0, 190 / max(sample.width, sample.height))
            img = sample.resize((max(1, int(sample.width * scale)), max(1, int(sample.height * scale))), Image.Resampling.NEAREST)
            yoff = 62 + r * cell_h
            x = c * cell_w + (cell_w - img.width) // 2
            y = yoff + (cell_h - img.height) // 2
            canvas.alpha_composite(img, (x, y))
            d.rectangle((c * cell_w + 8, yoff + 8, c * cell_w + cell_w - 8, yoff + cell_h - 8), outline=GRID, width=1)
            d.text((c * cell_w + 12, yoff + 12), "BEFORE" if r == 0 else "AFTER", font=small, fill=YELLOW if r == 1 else TEXT)
    canvas.save(out_dir / f"{slug}_MOTION_REVIEW_v1_before_after.png")

shrink_check()
before_after()

alpha = src.getchannel("A")
corners = [alpha.getpixel((0,0)), alpha.getpixel((w-1,0)), alpha.getpixel((0,h-1)), alpha.getpixel((w-1,h-1))]
coverage = sum(1 for v in alpha.getdata() if v > 0)

print(f"source={source}")
print(f"out_dir={out_dir}")
print(f"sheet={transparent_sheet}")
print(f"size={w}x{h}")
print(f"corners_alpha={corners}")
print(f"opaque_pixels={coverage}")
print("run_gif=" + str(out_dir / f"{slug}_MOTION_REVIEW_v1_RUN.gif"))
print("attack_gif=" + str(out_dir / f"{slug}_MOTION_REVIEW_v1_ATTACK.gif"))
print("shrink=" + str(out_dir / f"{slug}_MOTION_REVIEW_v1_shrink_check.png"))
'@

$script = $script.Replace("__SOURCE__", $CleanSource.Replace("\", "\\"))
$script = $script.Replace("__OUT_DIR__", $ResolvedOutDir.Replace("\", "\\"))
$script = $script.Replace("__PROJECT_ROOT__", $ProjectRoot.Replace("\", "\\"))
$script = $script.Replace("__BEFORE_IMAGE__", $BeforeImage.Replace("\", "\\"))
$script = $script.Replace("__SLUG__", $Slug)
$script = $script.Replace("__DISPLAY_NAME__", $DisplayName)
$tmpScript = Join-Path ([System.IO.Path]::GetTempPath()) "build_character_motion_review.py"
[System.IO.File]::WriteAllText($tmpScript, $script, [System.Text.Encoding]::UTF8)
& $PythonPath $tmpScript
