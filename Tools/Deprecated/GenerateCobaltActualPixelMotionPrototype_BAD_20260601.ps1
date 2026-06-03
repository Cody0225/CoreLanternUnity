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

CELL = 96
SCALE = 3
BG = (5, 13, 18, 255)
PANEL = (4, 22, 30, 255)
GRID = (22, 74, 88, 255)
CYAN = (76, 230, 255, 255)
CYAN_DIM = (31, 139, 190, 255)
BLUE = (18, 105, 218, 255)
BLUE_DARK = (9, 45, 120, 255)
BLUE_MID = (33, 145, 235, 255)
WHITE = (228, 248, 255, 255)
ICE = (163, 235, 255, 255)
FUR_SHADOW = (95, 156, 198, 255)
ARMOR = (22, 38, 64, 255)
ARMOR_HI = (70, 118, 160, 255)
OUTLINE = (2, 8, 16, 255)
SHADOW = (0, 0, 0, 100)
YELLOW = (255, 215, 92, 255)
HIT_FLASH = (255, 255, 255, 180)

try:
    font = ImageFont.truetype("arial.ttf", 18)
    font_small = ImageFont.truetype("arial.ttf", 13)
except Exception:
    font = ImageFont.load_default()
    font_small = ImageFont.load_default()

def rect(d, xy, fill):
    d.rectangle([int(v) for v in xy], fill=fill)

def poly(d, pts, fill):
    d.polygon([(int(x), int(y)) for x, y in pts], fill=fill)

def line(d, pts, fill, width=1):
    d.line([(int(x), int(y)) for x, y in pts], fill=fill, width=width)

def ellipse(d, xy, fill, outline=None, width=1):
    d.ellipse([int(v) for v in xy], fill=fill, outline=outline, width=width)

def diamond(d, cx, cy, r, fill, outline=OUTLINE):
    poly(d, [(cx, cy-r), (cx+r, cy), (cx, cy+r), (cx-r, cy)], outline)
    poly(d, [(cx, cy-r+2), (cx+r-2, cy), (cx, cy+r-2), (cx-r+2, cy)], fill)

def draw_leg(d, x, y, ox, oy, far=False, lift=0, stretch=0):
    col = (21, 84, 158, 255) if far else BLUE_MID
    joint = CYAN_DIM if far else CYAN
    px = x + ox + stretch
    py = y + oy - lift
    # upper armor
    rect(d, (px-4, py-16, px+5, py-8), OUTLINE)
    rect(d, (px-3, py-15, px+4, py-9), col)
    # lower black cyber shin
    rect(d, (px-3, py-9, px+3, py-1), OUTLINE)
    rect(d, (px-2, py-8, px+2, py-2), ARMOR)
    # paw
    rect(d, (px-7, py-2, px+7, py+2), OUTLINE)
    rect(d, (px-5, py-2, px+5, py+1), WHITE)
    rect(d, (px-1, py-14, px+2, py-11), joint)

def draw_tail(d, ox, oy, lift, wag):
    # segmented digital tail from actual Cobalt Pup, rear-left to upper-left.
    base = (33+ox, 49+oy)
    segs = [
        (25+ox, 42+oy-lift, 8, 5),
        (18+ox+wag, 34+oy-lift, 9, 5),
        (12+ox+wag, 25+oy-lift, 9, 6),
    ]
    line(d, [base, (28+ox, 44+oy-lift), (20+ox+wag, 36+oy-lift), (13+ox+wag, 27+oy-lift)], OUTLINE, 7)
    line(d, [base, (28+ox, 44+oy-lift), (20+ox+wag, 36+oy-lift), (13+ox+wag, 27+oy-lift)], CYAN_DIM, 4)
    for cx, cy, rw, rh in segs:
        rect(d, (cx-rw//2-1, cy-rh//2-1, cx+rw//2+1, cy+rh//2+1), OUTLINE)
        rect(d, (cx-rw//2, cy-rh//2, cx+rw//2, cy+rh//2), CYAN)
        rect(d, (cx-rw//2+2, cy-rh//2+1, cx+rw//2-2, cy-rh//2+1), WHITE)

def draw_cobalt(action="idle", frame=0):
    im = Image.new("RGBA", (CELL, CELL), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)

    bob = 0
    ox = 0
    recoil = 0
    ear = 0
    tail_lift = 0
    tail_wag = 0
    mouth_open = False
    muzzle_charge = 0
    projectile = 0
    hit_flash = False

    if action == "idle":
        bob = [0, -1, 0, 1, 0, -1][frame % 6]
        ear = [0, 0, -1, 0, 1, 0][frame % 6]
        tail_lift = [0, 1, 0, -1, 0, 1][frame % 6]
        tail_wag = [0, 1, 0, -1, 0, 1][frame % 6]
        gait = 1
    elif action == "move":
        gait = frame % 6
        bob = [0, -2, -1, 1, 0, -1][gait]
        ox = [0, 1, 2, 0, -1, -2][gait]
        tail_lift = [2, 1, -1, 2, 1, -1][gait]
        tail_wag = [-2, -1, 1, 2, 1, -1][gait]
    elif action == "attack":
        gait = 1
        recoil = [0, -1, -3, -4, -2, 0][frame % 6]
        bob = [0, 0, -1, -1, 0, 0][frame % 6]
        ear = [-1 if frame in (1, 2, 3) else 0][0]
        tail_lift = [0, 1, 2, 1, 0, 0][frame % 6]
        tail_wag = [-1, 0, 1, 0, -1, 0][frame % 6]
        mouth_open = frame in (1, 2, 3)
        muzzle_charge = [0, 3, 6, 3, 0, 0][frame % 6]
        projectile = [0, 0, 1, 2, 3, 0][frame % 6]
    elif action == "hit":
        gait = 1
        recoil = [-4, -2, 0, -1, -3, 0][frame % 6]
        bob = [0, 1, 0, -1, 1, 0][frame % 6]
        ear = 2
        tail_lift = -2
        tail_wag = 3
        hit_flash = frame in (0, 1, 4)
    else:
        gait = 1

    ox = ox + recoil
    oy = bob

    ellipse(d, (15+ox, 70, 78+ox, 84), SHADOW)

    # Diagonal gait, four separate feet. The named positions deliberately alternate.
    steps = [
        ((30,69,0,-2), (40,73,0,1), (57,73,0,1), (69,69,0,-2)),
        ((32,70,0,-1), (41,72,0,0), (58,72,0,0), (67,70,0,-1)),
        ((34,72,0,1), (39,68,0,-2), (61,68,0,-2), (66,73,0,1)),
        ((40,73,0,1), (30,69,0,-2), (69,69,0,-2), (57,73,0,1)),
        ((41,72,0,0), (32,70,0,-1), (67,70,0,-1), (58,72,0,0)),
        ((39,68,0,-2), (34,72,0,1), (66,73,0,1), (61,68,0,-2)),
    ]
    step = steps[gait % 6] if action == "move" else ((31,72,0,0), (41,73,0,0), (57,73,0,0), (68,72,0,0))

    draw_tail(d, ox, oy, tail_lift, tail_wag)

    # Far legs first, near legs last.
    draw_leg(d, step[0][0], step[0][1], ox, oy, True, -step[0][3], step[0][2])
    draw_leg(d, step[2][0], step[2][1], ox, oy, True, -step[2][3], step[2][2])
    draw_leg(d, step[1][0], step[1][1], ox, oy, False, -step[1][3], step[1][2])
    draw_leg(d, step[3][0], step[3][1], ox, oy, False, -step[3][3], step[3][2])

    # Body armor shell.
    ellipse(d, (27+ox, 42+oy, 69+ox, 68+oy), OUTLINE)
    ellipse(d, (30+ox, 44+oy, 67+ox, 66+oy), BLUE_DARK)
    ellipse(d, (34+ox, 46+oy, 60+ox, 63+oy), BLUE)
    rect(d, (40+ox, 50+oy, 57+ox, 58+oy), CYAN_DIM)
    rect(d, (44+ox, 52+oy, 54+ox, 55+oy), CYAN)
    diamond(d, 50+ox, 54+oy, 5, CYAN)
    # shoulder joint rings from actual image
    ellipse(d, (60+ox, 48+oy, 73+ox, 61+oy), OUTLINE)
    ellipse(d, (62+ox, 50+oy, 71+ox, 59+oy), ARMOR_HI)
    ellipse(d, (65+ox, 53+oy, 68+ox, 56+oy), CYAN)

    # Blue digital scarf/collar.
    rect(d, (35+ox, 38+oy, 66+ox, 47+oy), OUTLINE)
    rect(d, (37+ox, 39+oy, 64+ox, 45+oy), CYAN_DIM)
    for sx in range(40, 64, 6):
        rect(d, (sx+ox, 39+oy, sx+ox+1, 45+oy), CYAN)
    rect(d, (43+ox, 42+oy, 61+ox, 43+oy), ICE)

    # Head base: angular blue mask and white muzzle/chin.
    poly(d, [(44+ox,38+oy), (48+ox,24+oy), (63+ox,18+oy), (78+ox,25+oy), (82+ox,39+oy), (72+ox,52+oy), (55+ox,52+oy)], OUTLINE)
    poly(d, [(47+ox,38+oy), (51+ox,27+oy), (63+ox,22+oy), (75+ox,28+oy), (78+ox,39+oy), (70+ox,49+oy), (56+ox,49+oy)], BLUE)
    poly(d, [(56+ox,38+oy), (72+ox,35+oy), (78+ox,41+oy), (70+ox,50+oy), (57+ox,49+oy)], WHITE)
    poly(d, [(56+ox,47+oy), (70+ox,50+oy), (61+ox,55+oy)], WHITE)

    # Cheek fur tufts.
    poly(d, [(48+ox,45+oy), (37+ox,47+oy), (47+ox,50+oy)], OUTLINE)
    poly(d, [(50+ox,44+oy), (40+ox,47+oy), (49+ox,49+oy)], WHITE)
    poly(d, [(73+ox,43+oy), (84+ox,45+oy), (74+ox,49+oy)], OUTLINE)
    poly(d, [(72+ox,43+oy), (81+ox,45+oy), (73+ox,48+oy)], ICE)

    # Tall ears with white outer fur and cyan inner glow.
    poly(d, [(49+ox,28+oy), (42+ox,4+oy+ear), (59+ox,20+oy)], OUTLINE)
    poly(d, [(51+ox,27+oy), (45+ox,8+oy+ear), (58+ox,21+oy)], WHITE)
    poly(d, [(53+ox,25+oy), (48+ox,13+oy+ear), (58+ox,22+oy)], CYAN)
    poly(d, [(72+ox,27+oy), (83+ox,5+oy-ear), (67+ox,21+oy)], OUTLINE)
    poly(d, [(71+ox,27+oy), (80+ox,9+oy-ear), (68+ox,22+oy)], WHITE)
    poly(d, [(70+ox,24+oy), (77+ox,14+oy-ear), (69+ox,23+oy)], CYAN)

    # Forehead marks.
    diamond(d, 63+ox, 28+oy, 4, ICE)
    rect(d, (54+ox, 32+oy, 59+ox, 36+oy), WHITE)

    # Eye and muzzle details.
    poly(d, [(68+ox,34+oy), (78+ox,32+oy), (75+ox,37+oy), (69+ox,38+oy)], OUTLINE)
    poly(d, [(70+ox,34+oy), (76+ox,33+oy), (74+ox,36+oy), (70+ox,37+oy)], CYAN)
    rect(d, (75+ox, 34+oy, 77+ox, 35+oy), WHITE)
    # nose and mouth, facing right. Projectile is locked here.
    rect(d, (79+ox, 42+oy, 83+ox, 45+oy), OUTLINE)
    if mouth_open:
        rect(d, (80+ox, 45+oy, 86+ox, 49+oy), OUTLINE)
        rect(d, (83+ox, 46+oy, 86+ox, 48+oy), CYAN)
    else:
        rect(d, (80+ox, 46+oy, 85+ox, 47+oy), OUTLINE)

    # Cyan data squares near tail/body, like the actual asset but restrained.
    for idx, (sx, sy) in enumerate([(22,24), (18,20), (16,31)]):
        if (frame + idx) % 3 != 0:
            rect(d, (sx+ox, sy+oy, sx+ox+2, sy+oy+2), CYAN)

    muzzle = (86+ox, 47+oy)
    if muzzle_charge:
        ellipse(d, (muzzle[0]-muzzle_charge, muzzle[1]-muzzle_charge, muzzle[0]+muzzle_charge, muzzle[1]+muzzle_charge), (76,230,255,75))
        rect(d, (muzzle[0]-1, muzzle[1]-1, muzzle[0]+1, muzzle[1]+1), WHITE)
    if projectile:
        start = muzzle[0] + 2 + projectile * 5
        line(d, [(muzzle[0], muzzle[1]), (min(start, 95), muzzle[1])], (49, 180, 230, 185), 2)
        rect(d, (start, muzzle[1]-3, start+9, muzzle[1]+3), OUTLINE)
        rect(d, (start+1, muzzle[1]-2, start+8, muzzle[1]+2), CYAN)
        rect(d, (start+6, muzzle[1]-1, start+12, muzzle[1]+1), WHITE)

    if hit_flash:
        overlay = Image.new("RGBA", (CELL, CELL), (0, 0, 0, 0))
        od = ImageDraw.Draw(overlay)
        ellipse(od, (22+ox, 14+oy, 84+ox, 75+oy), HIT_FLASH)
        im.alpha_composite(overlay)

    return im

def upscale(im):
    return im.resize((CELL*SCALE, CELL*SCALE), Image.Resampling.NEAREST)

actions = [
    ("IDLE", "idle"),
    ("RUN - diagonal gait", "move"),
    ("ATTACK - muzzle shot", "attack"),
    ("HIT", "hit"),
]
durations = {"idle": 150, "move": 90, "attack": 95, "hit": 115}

# Individual previews on a dark background.
for label, key in actions:
    frames = []
    for i in range(6):
        bg = Image.new("RGBA", (CELL*SCALE, CELL*SCALE), BG)
        bg.alpha_composite(upscale(draw_cobalt(key, i)))
        frames.append(bg.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=180))
    frames[0].save(out_dir / f"CobaltPup_Actual_{key}_v1.gif", save_all=True, append_images=frames[1:], duration=durations[key], loop=0)

# Composite motion preview.
panel_w, panel_h = 360, 380
preview_frames = []
for i in range(6):
    canvas = Image.new("RGBA", (panel_w*2, panel_h*2), BG)
    cd = ImageDraw.Draw(canvas)
    for idx, (label, key) in enumerate(actions):
        px = (idx % 2) * panel_w
        py = (idx // 2) * panel_h
        rect(cd, (px+10, py+10, px+panel_w-10, py+panel_h-10), PANEL)
        cd.rectangle((px+10, py+10, px+panel_w-10, py+panel_h-10), outline=GRID, width=2)
        cd.text((px+22, py+18), label, fill=CYAN if key != "attack" else YELLOW, font=font)
        if key == "move":
            cd.text((px+22, py+44), "front/rear legs alternate, body bobs", fill=(155, 216, 224, 255), font=font_small)
        if key == "attack":
            cd.text((px+22, py+44), "bolt starts at mouth, no belly shots", fill=(226, 213, 132, 255), font=font_small)
        sprite_x = px + 40
        sprite_y = py + 80
        canvas.alpha_composite(upscale(draw_cobalt(key, i)), (sprite_x, sprite_y))
        cd.line((px+34, sprite_y+CELL*SCALE-38, px+330, sprite_y+CELL*SCALE-38), fill=(25, 92, 106, 255), width=1)
    preview_frames.append(canvas.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=220))

preview_frames[0].save(out_dir / "CobaltPup_Actual_MotionPreview_v1.gif", save_all=True, append_images=preview_frames[1:], duration=105, loop=0)

# Transparent runtime-oriented sheet.
sheet = Image.new("RGBA", (CELL*6, CELL*4), (0, 0, 0, 0))
for row, (_, key) in enumerate(actions):
    for col in range(6):
        sheet.alpha_composite(draw_cobalt(key, col), (col*CELL, row*CELL))
sheet.save(out_dir / "CobaltPup_Actual_SpriteSheet_v1.png")

# Transparent frame export for quick inspection/import experiments.
frames_dir = out_dir / "CobaltPup_Actual_frames_v1"
frames_dir.mkdir(exist_ok=True)
for _, key in actions:
    for col in range(6):
        draw_cobalt(key, col).save(frames_dir / f"CobaltPup_Actual_{key}_{col+1:02d}.png")

# Gait debug.
phase = [
    "FL + rear-right forward",
    "transition",
    "FR + rear-left forward",
    "alternate pair forward",
    "transition",
    "opposite pair forward",
]
debug_frames = []
for i in range(6):
    canvas = Image.new("RGBA", (420, 340), BG)
    d = ImageDraw.Draw(canvas)
    d.text((18, 14), f"RUN frame {i+1}: {phase[i]}", fill=CYAN, font=font)
    d.text((18, 42), "Check: no same-foot slide / no body-only drift", fill=(156, 218, 226, 255), font=font_small)
    canvas.alpha_composite(upscale(draw_cobalt("move", i)), (62, 52))
    d.line((52, 52+CELL*SCALE-38, 360, 52+CELL*SCALE-38), fill=(25, 92, 106, 255), width=1)
    debug_frames.append(canvas.convert("RGB").convert("P", palette=Image.Palette.ADAPTIVE, colors=220))
debug_frames[0].save(out_dir / "CobaltPup_Actual_MoveGait_Debug_v1.gif", save_all=True, append_images=debug_frames[1:], duration=300, loop=0)

print(out_dir / "CobaltPup_Actual_MotionPreview_v1.gif")
print(out_dir / "CobaltPup_Actual_MoveGait_Debug_v1.gif")
print(out_dir / "CobaltPup_Actual_SpriteSheet_v1.png")
'@

$script = $script.Replace("__OUT_DIR__", $OutDir.Replace("\", "\\"))
$tmpScript = Join-Path $OutDir "_generate_cobalt_actual_pixel_motion.py"
[System.IO.File]::WriteAllText($tmpScript, $script, [System.Text.Encoding]::UTF8)
& $PythonPath $tmpScript
