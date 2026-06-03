param(
    [string]$PythonPath = "C:\Users\kodai\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe"
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$OutDir = Join-Path $ProjectRoot "Assets\ArtSource\AnimationPrototypes"
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$script = @'
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

out_dir = Path(r"__OUT_DIR__")
out_dir.mkdir(parents=True, exist_ok=True)

CELL = 64
SCALE = 3
PREVIEW_PANEL_W = 260
PREVIEW_PANEL_H = 220
BG = (7, 14, 18, 255)
GRID = (20, 65, 76, 255)
CYAN = (91, 231, 255, 255)
CYAN2 = (44, 164, 214, 255)
WHITE = (220, 248, 255, 255)
BLUE = (69, 162, 217, 255)
DARK = (8, 24, 34, 255)
OUTLINE = (3, 10, 16, 255)
SHADOW = (0, 0, 0, 90)
HIT = (255, 255, 255, 190)
YELLOW = (255, 214, 82, 255)

try:
    font = ImageFont.truetype("arial.ttf", 16)
    font_small = ImageFont.truetype("arial.ttf", 12)
except Exception:
    font = ImageFont.load_default()
    font_small = ImageFont.load_default()

def rect(d, xy, fill):
    d.rectangle([int(v) for v in xy], fill=fill)

def poly(d, pts, fill):
    d.polygon([(int(x), int(y)) for x, y in pts], fill=fill)

def line(d, pts, fill, width=1):
    d.line([(int(x), int(y)) for x, y in pts], fill=fill, width=width)

def draw_pup(action="idle", frame=0):
    im = Image.new("RGBA", (CELL, CELL), (0,0,0,0))
    d = ImageDraw.Draw(im)

    # Per-action motion controls. The gait uses diagonal pairs:
    # phase 0: front-left + rear-right forward
    # phase 3: front-right + rear-left forward
    bob = 0
    body_x = 0
    recoil = 0
    mouth_open = False
    muzzle_charge = 0
    projectile = 0
    flash = 0

    if action == "idle":
        bob = [0, -1, 0, 1][frame % 4]
        tail_lift = [0, 1, 0, -1][frame % 4]
        ear_twitch = [0, 0, -1, 0][frame % 4]
        gait = 1
    elif action == "move":
        gait = frame % 6
        bob = [0, -1, 0, 1, 0, -1][gait]
        body_x = [0, 1, 1, 0, -1, -1][gait]
        tail_lift = [1, 0, -1, 1, 0, -1][gait]
        ear_twitch = 0
    elif action == "attack":
        gait = 1
        bob = 0
        recoil = [0, 0, -1, -2, -1, 0][frame % 6]
        mouth_open = frame in (1,2,3)
        muzzle_charge = [0, 2, 4, 2, 0, 0][frame % 6]
        projectile = [0, 0, 1, 2, 3, 0][frame % 6]
        tail_lift = [0, 1, 1, 0, 0, 0][frame % 6]
        ear_twitch = -1 if frame in (1,2) else 0
    elif action == "hit":
        gait = 1
        bob = 0
        recoil = [-2, -1, 0, -1, -2, 0][frame % 6]
        flash = 1 if frame in (0,1,4) else 0
        tail_lift = -2
        ear_twitch = 1
    else:
        gait = 1
        tail_lift = 0
        ear_twitch = 0

    ox = body_x + recoil
    oy = bob

    # ground contact shadow
    d.ellipse((15+ox, 45, 54+ox, 54), fill=SHADOW)

    # legs: four distinct paws. Facing right.
    # Front = near head/right. Rear = near tail/left.
    # We draw far legs first, near legs last for readable overlap.
    if action == "move":
        # positions are intentionally different for each frame.
        # format: rear_far, rear_near, front_far, front_near
        step = [
            ((22,43),(27,46),(41,46),(47,43)),
            ((23,44),(28,45),(42,45),(46,44)),
            ((24,45),(29,43),(43,43),(45,46)),
            ((27,46),(22,43),(47,43),(41,46)),
            ((28,45),(23,44),(46,44),(42,45)),
            ((29,43),(24,45),(45,46),(43,43)),
        ][gait]
    else:
        step = ((23,44),(29,45),(41,45),(47,44))

    leg_far = (26, 102, 132, 255)
    leg_near = (65, 190, 232, 255)
    paw = (210, 248, 255, 255)
    for (x,y), col in [(step[0], leg_far), (step[2], leg_far), (step[1], leg_near), (step[3], leg_near)]:
        rect(d, (x+ox-2, y+oy-9, x+ox+2, y+oy), OUTLINE)
        rect(d, (x+ox-1, y+oy-8, x+ox+1, y+oy-1), col)
        rect(d, (x+ox-3, y+oy-1, x+ox+3, y+oy+1), OUTLINE)
        rect(d, (x+ox-2, y+oy-1, x+ox+2, y+oy), paw)

    # tail: cyan cyber tail, attached to rear/left
    tail_base = (22+ox, 33+oy)
    tail_tip = (10+ox, 27+oy-tail_lift)
    line(d, [tail_base, (15+ox, 28+oy-tail_lift), tail_tip], OUTLINE, 5)
    line(d, [tail_base, (15+ox, 28+oy-tail_lift), tail_tip], CYAN2, 3)
    rect(d, (8+ox, 25+oy-tail_lift, 12+ox, 29+oy-tail_lift), CYAN)

    # body outline and fill
    d.ellipse((18+ox, 27+oy, 47+ox, 45+oy), fill=OUTLINE)
    d.ellipse((20+ox, 28+oy, 45+ox, 44+oy), fill=BLUE)
    d.ellipse((25+ox, 30+oy, 43+ox, 43+oy), fill=(92, 213, 240, 255))
    rect(d, (31+ox, 32+oy, 36+ox, 37+oy), CYAN)
    rect(d, (33+ox, 34+oy, 34+ox, 35+oy), WHITE)

    # head
    d.ellipse((38+ox, 18+oy, 57+ox, 37+oy), fill=OUTLINE)
    d.ellipse((40+ox, 20+oy, 55+ox, 36+oy), fill=(111, 220, 245, 255))
    d.ellipse((43+ox, 23+oy, 55+ox, 34+oy), fill=(186, 245, 255, 255))

    # ears
    poly(d, [(41+ox,21+oy), (36+ox,10+oy+ear_twitch), (46+ox,17+oy)], OUTLINE)
    poly(d, [(43+ox,21+oy), (39+ox,13+oy+ear_twitch), (47+ox,18+oy)], CYAN2)
    poly(d, [(52+ox,21+oy), (57+ox,10+oy-ear_twitch), (49+ox,18+oy)], OUTLINE)
    poly(d, [(51+ox,21+oy), (55+ox,13+oy-ear_twitch), (49+ox,18+oy)], CYAN2)

    # face and eye
    rect(d, (51+ox, 26+oy, 53+ox, 28+oy), OUTLINE)
    rect(d, (52+ox, 26+oy, 53+ox, 27+oy), WHITE)
    rect(d, (56+ox, 30+oy, 59+ox, 32+oy), OUTLINE)
    if mouth_open:
        rect(d, (56+ox, 31+oy, 60+ox, 34+oy), OUTLINE)
        rect(d, (58+ox, 32+oy, 60+ox, 33+oy), CYAN)
    else:
        rect(d, (56+ox, 31+oy, 59+ox, 32+oy), OUTLINE)

    # forehead/core gem
    rect(d, (47+ox, 21+oy, 50+ox, 24+oy), OUTLINE)
    rect(d, (48+ox, 22+oy, 49+ox, 23+oy), CYAN)

    # attack: projectile always originates from mouth/muzzle only
    muzzle = (60+ox, 32+oy)
    if muzzle_charge:
        d.ellipse((muzzle[0]-muzzle_charge, muzzle[1]-muzzle_charge, muzzle[0]+muzzle_charge, muzzle[1]+muzzle_charge), fill=(91,231,255,90))
        rect(d, (muzzle[0]-1, muzzle[1]-1, muzzle[0]+1, muzzle[1]+1), WHITE)
    if projectile:
        start = muzzle[0] + 2 + projectile * 4
        rect(d, (start, muzzle[1]-2, start+8, muzzle[1]+2), CYAN)
        rect(d, (start+8, muzzle[1]-1, start+12, muzzle[1]+1), WHITE)
        # small trail connected back toward mouth
        line(d, [(muzzle[0]+1, muzzle[1]), (start, muzzle[1])], (42, 174, 220, 180), 1)

    if flash:
        # hit flash overlay, but not hiding silhouette completely
        overlay = Image.new("RGBA", (CELL, CELL), (0,0,0,0))
        od = ImageDraw.Draw(overlay)
        od.ellipse((17+ox, 17+oy, 58+ox, 48+oy), fill=HIT)
        im.alpha_composite(overlay)

    return im

def upscale(im):
    return im.resize((CELL*SCALE, CELL*SCALE), Image.Resampling.NEAREST)

actions = [
    ("IDLE", "idle"),
    ("MOVE", "move"),
    ("ATTACK - mouth only", "attack"),
    ("HIT", "hit"),
]

# Save individual animation GIFs
durations = {"idle": 150, "move": 95, "attack": 95, "hit": 120}
for label, key in actions:
    frames = []
    for i in range(6):
        bg = Image.new("RGBA", (CELL*SCALE, CELL*SCALE), BG)
        bg.alpha_composite(upscale(draw_pup(key, i)))
        frames.append(bg.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=128))
    frames[0].save(out_dir / f"CobaltPup_{key}_v2.gif", save_all=True, append_images=frames[1:], duration=durations[key], loop=0)

# Composite preview: four panels animate simultaneously.
preview_frames = []
for i in range(6):
    canvas = Image.new("RGBA", (PREVIEW_PANEL_W*2, PREVIEW_PANEL_H*2), BG)
    cd = ImageDraw.Draw(canvas)
    for idx, (label, key) in enumerate(actions):
        px = (idx % 2) * PREVIEW_PANEL_W
        py = (idx // 2) * PREVIEW_PANEL_H
        rect(cd, (px+8, py+8, px+PREVIEW_PANEL_W-8, py+PREVIEW_PANEL_H-8), (4, 20, 28, 255))
        cd.rectangle((px+8, py+8, px+PREVIEW_PANEL_W-8, py+PREVIEW_PANEL_H-8), outline=GRID, width=2)
        cd.text((px+16, py+14), label, fill=CYAN if key != "attack" else YELLOW, font=font)
        if key == "move":
            cd.text((px+16, py+36), "diagonal foot pairs alternate", fill=(150,210,220,255), font=font_small)
        if key == "attack":
            cd.text((px+16, py+36), "projectile origin locked to muzzle", fill=(225,210,130,255), font=font_small)
        sprite = upscale(draw_pup(key, i))
        canvas.alpha_composite(sprite, (px+34, py+52))
        # contact baseline
        cd.line((px+30, py+52+CELL*SCALE-28, px+222, py+52+CELL*SCALE-28), fill=(25,90,104,255), width=1)
    preview_frames.append(canvas.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=160))

preview_frames[0].save(out_dir / "CobaltPup_MotionPreview_v2.gif", save_all=True, append_images=preview_frames[1:], duration=105, loop=0)

# Sprite sheet with transparent frames: 4 rows x 6 columns
sheet = Image.new("RGBA", (CELL*6, CELL*4), (0,0,0,0))
for row, (_, key) in enumerate(actions):
    for col in range(6):
        sheet.alpha_composite(draw_pup(key, col), (col*CELL, row*CELL))
sheet.save(out_dir / "CobaltPup_SpriteSheet_v2.png")

# Debug strip for gait only with foot phase markers.
debug_frames = []
phase_text = [
    "FL+RR forward",
    "transition",
    "FR+RL forward",
    "FL+RR back",
    "transition",
    "FR+RL back",
]
for i in range(6):
    canvas = Image.new("RGBA", (330, 240), BG)
    d = ImageDraw.Draw(canvas)
    d.text((16, 12), f"MOVE frame {i+1}: {phase_text[i]}", fill=CYAN, font=font)
    d.text((16, 34), "Check: feet alternate, body bobs, no same-foot slide", fill=(160,220,225,255), font=font_small)
    canvas.alpha_composite(upscale(draw_pup("move", i)), (70, 54))
    d.line((58, 54+CELL*SCALE-28, 260, 54+CELL*SCALE-28), fill=(25,90,104,255), width=1)
    debug_frames.append(canvas.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=160))
debug_frames[0].save(out_dir / "CobaltPup_MoveGait_Debug_v2.gif", save_all=True, append_images=debug_frames[1:], duration=300, loop=0)

print(out_dir / "CobaltPup_MotionPreview_v2.gif")
print(out_dir / "CobaltPup_MoveGait_Debug_v2.gif")
print(out_dir / "CobaltPup_SpriteSheet_v2.png")
'@

$script = $script.Replace("__OUT_DIR__", $OutDir.Replace("\", "\\"))
$tmpScript = Join-Path $OutDir "_generate_cobalt_pixel_motion.py"
[System.IO.File]::WriteAllText($tmpScript, $script, [System.Text.Encoding]::UTF8)
& $PythonPath $tmpScript
