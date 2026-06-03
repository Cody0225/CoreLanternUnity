param(
    [string]$PythonPath = "C:\Users\kodai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe"
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$Source = Join-Path $ProjectRoot "Assets\ArtSource\CobaltPixelAnim_20260601\CobaltPup_PixelAnimSheet_transparent.png"
$OutDir = Join-Path $ProjectRoot "Assets\ArtSource\CobaltPixelAnim_20260601\review_gifs"
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$script = @'
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

source_path = Path(r"__SOURCE__")
out_dir = Path(r"__OUT_DIR__")
out_dir.mkdir(parents=True, exist_ok=True)

src = Image.open(source_path).convert("RGBA")
alpha = src.getchannel("A")

BG = (5, 12, 16, 255)
PANEL = (3, 20, 27, 255)
GRID = (29, 93, 105, 255)
CYAN = (94, 236, 255, 255)
YELLOW = (255, 215, 92, 255)
TEXT = (190, 235, 240, 255)

try:
    font = ImageFont.truetype("arial.ttf", 18)
    small = ImageFont.truetype("arial.ttf", 13)
except Exception:
    font = ImageFont.load_default()
    small = ImageFont.load_default()

def content_bbox(region):
    box = region.getchannel("A").getbbox()
    return box

def crop_frame(raw_box, pad=10):
    x0, y0, x1, y1 = raw_box
    x0 = max(0, x0 - pad)
    y0 = max(0, y0 - pad)
    x1 = min(src.width, x1 + pad)
    y1 = min(src.height, y1 + pad)
    region = src.crop((x0, y0, x1, y1))
    box = content_bbox(region)
    if not box:
        return None
    bx0, by0, bx1, by1 = box
    bx0 = max(0, bx0 - pad)
    by0 = max(0, by0 - pad)
    bx1 = min(region.width, bx1 + pad)
    by1 = min(region.height, by1 + pad)
    return region.crop((bx0, by0, bx1, by1))

def save_gif(name, frames, duration, mode="center", canvas=(360, 270), label=None):
    rendered = []
    for idx, fr in enumerate(frames):
        bg = Image.new("RGBA", canvas, BG)
        d = ImageDraw.Draw(bg)
        if label:
            d.text((16, 12), label, font=font, fill=CYAN if "ATTACK" not in label else YELLOW)
        ground = canvas[1] - 36
        d.line((24, ground, canvas[0]-24, ground), fill=(22, 80, 92, 255), width=1)
        if mode == "left":
            x = 32
        else:
            x = (canvas[0] - fr.width) // 2
        y = ground - fr.height + 8
        bg.alpha_composite(fr, (x, y))
        rendered.append(bg.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=220))
    rendered[0].save(out_dir / name, save_all=True, append_images=rendered[1:], duration=duration, loop=0)

def save_transparent_sheet(name, actions):
    cell_w, cell_h = 340, 250
    cols = max(len(v) for v in actions.values())
    rows = len(actions)
    sheet = Image.new("RGBA", (cell_w * cols, cell_h * rows), (0, 0, 0, 0))
    for r, (action, frames) in enumerate(actions.items()):
        for c, fr in enumerate(frames):
            if action == "attack":
                x = c * cell_w + 24
            else:
                x = c * cell_w + (cell_w - fr.width) // 2
            y = r * cell_h + (cell_h - fr.height) // 2
            sheet.alpha_composite(fr, (x, y))
    sheet.save(out_dir / name)

# These boxes are intentionally based on visible frame clusters in the approved AI rough sheet.
# We do not redraw the character here; we only crop and preview the existing art.
boxes = {
    "idle": [
        (190, 50, 382, 270),
        (440, 50, 650, 270),
        (695, 50, 890, 270),
        (922, 50, 1120, 270),
    ],
    "run": [
        (78, 318, 292, 505),
        (304, 318, 535, 505),
        (548, 318, 765, 505),
        (792, 318, 1006, 505),
        (1036, 318, 1250, 505),
        (1270, 318, 1502, 505),
    ],
    "attack": [
        (160, 558, 390, 750),
        (424, 558, 655, 750),
        (690, 558, 1086, 750),
        (1088, 558, 1300, 750),
    ],
    "hit": [
        (184, 790, 396, 980),
        (432, 790, 648, 980),
    ],
}

frames = {}
for action, raw_boxes in boxes.items():
    frames[action] = []
    for box in raw_boxes:
        fr = crop_frame(box)
        if fr is not None:
            frames[action].append(fr)

save_gif("CobaltPup_AIrough_Idle_review_v1.gif", frames["idle"], 160, label="IDLE / source sheet crop")
save_gif("CobaltPup_AIrough_Run_review_v1.gif", frames["run"], 90, label="RUN / source sheet crop")
save_gif("CobaltPup_AIrough_Attack_review_v1.gif", frames["attack"], 110, mode="left", canvas=(500, 270), label="ATTACK / source core shot")
save_gif("CobaltPup_AIrough_Hit_review_v1.gif", frames["hit"], 130, label="HIT / source sheet crop")

panel_w, panel_h = 500, 300
sequence_count = 12
preview = []
labels = [("IDLE", "idle"), ("RUN", "run"), ("ATTACK", "attack"), ("HIT", "hit")]
for i in range(sequence_count):
    canvas = Image.new("RGBA", (panel_w * 2, panel_h * 2), BG)
    d = ImageDraw.Draw(canvas)
    for idx, (label, action) in enumerate(labels):
        px = (idx % 2) * panel_w
        py = (idx // 2) * panel_h
        d.rectangle((px+10, py+10, px+panel_w-10, py+panel_h-10), fill=PANEL, outline=GRID, width=2)
        d.text((px+22, py+20), label, font=font, fill=CYAN if action != "attack" else YELLOW)
        if action == "run":
            d.text((px+22, py+46), "check rear legs carefully before runtime adoption", font=small, fill=TEXT)
        if action == "attack":
            d.text((px+22, py+46), "source art uses a chest-core shot", font=small, fill=TEXT)
        fr = frames[action][i % len(frames[action])]
        ground = py + panel_h - 44
        d.line((px+34, ground, px+panel_w-34, ground), fill=(22, 80, 92, 255), width=1)
        if action == "attack":
            x = px + 46
        else:
            x = px + (panel_w - fr.width) // 2
        y = ground - fr.height + 8
        canvas.alpha_composite(fr, (x, y))
    preview.append(canvas.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=220))
preview[0].save(out_dir / "CobaltPup_AIrough_MotionReview_v1.gif", save_all=True, append_images=preview[1:], duration=95, loop=0)

save_transparent_sheet("CobaltPup_AIrough_NormalizedReviewSheet_v1.png", frames)

print(out_dir / "CobaltPup_AIrough_MotionReview_v1.gif")
print(out_dir / "CobaltPup_AIrough_Run_review_v1.gif")
print(out_dir / "CobaltPup_AIrough_Attack_review_v1.gif")
print(out_dir / "CobaltPup_AIrough_NormalizedReviewSheet_v1.png")
'@

$script = $script.Replace("__SOURCE__", $Source.Replace("\", "\\"))
$script = $script.Replace("__OUT_DIR__", $OutDir.Replace("\", "\\"))
$tmpScript = Join-Path $OutDir "_generate_cobalt_sheet_review_gifs.py"
[System.IO.File]::WriteAllText($tmpScript, $script, [System.Text.Encoding]::UTF8)
& $PythonPath $tmpScript
