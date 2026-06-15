# -*- coding: utf-8 -*-
"""
build_expression_preview.py — Reusable expression preview builder.

Takes a character's 4 expression illustrations (neutral/hurt/attack/evolve, transparent PNG, any size)
and produces an animated preview GIF + a labeled keyframe strip:
  idle/walk (bounce) -> ATTACK (speed lines + lunge) -> DAMAGE (red flash + impact + recoil)
  -> EVOLVE (white flash + ring) -> settle.

Method (locked in docs/VISUAL_STYLE_GUIDE.md §11-2):
  - whole-illustration swap per expression (NO overlay / NO part-rig).
  - normalize the 4 to the SAME character height + feet baseline (uniform scale only).
  - animate with translation + uniform scale + small tilt + drawn FX. NEVER distort the face non-uniformly.

Run with the real Python (py), e.g.:
  py Tools/build_expression_preview.py ^
     --neutral PATH_NEUTRAL.png --hurt PATH_HURT.png --attack PATH_ATTACK.png --evolve PATH_EVOLVE.png ^
     --out OUTPUT_DIR --name s1_R1_L1
Outputs: OUTPUT_DIR/<name>_fx.gif and OUTPUT_DIR/<name>_strip.png
"""
import os, math, argparse
import numpy as np
from PIL import Image, ImageDraw

CAN = 1024
TARGET_H = 740
FEET_Y = 880
CX = 512


def normalize(path):
    im = Image.open(path).convert("RGBA")
    a = np.asarray(im)[:, :, 3]
    ys, xs = np.where(a > 0)
    crop = im.crop((xs.min(), ys.min(), xs.max() + 1, ys.max() + 1))
    cw, ch = crop.size
    s = TARGET_H / ch
    nw, nh = max(1, int(cw * s)), max(1, int(ch * s))
    cv = Image.new("RGBA", (CAN, CAN), (0, 0, 0, 0))
    cv.alpha_composite(crop.resize((nw, nh), Image.LANCZOS), (int(CX - nw / 2), int(FEET_Y - nh)))
    return cv


WIN = (CX - 400, FEET_Y - TARGET_H - 150, CX + 400, FEET_Y + 60)
CW, CH = WIN[2] - WIN[0], WIN[3] - WIN[1]
CXL = CX - WIN[0]
CYL = (FEET_Y - TARGET_H / 2) - WIN[1]


def char(IM, name, dy=0.0, dx=0.0, scale=1.0, ang=0.0, tint=0.0, bright=0.0):
    img = IM[name]
    if abs(scale - 1.0) > 1e-3:
        s = img.resize((int(CAN * scale), int(CAN * scale)), Image.LANCZOS)
        layer = Image.new("RGBA", (CAN, CAN), (0, 0, 0, 0))
        layer.alpha_composite(s, (int(CX - CX * scale), int(FEET_Y - FEET_Y * scale)))
        img = layer
    if abs(ang) > 1e-3:
        img = img.rotate(ang, resample=Image.BICUBIC, center=(CX, FEET_Y), expand=False)
    if abs(dx) > 1e-3 or abs(dy) > 1e-3:
        b = Image.new("RGBA", (CAN, CAN), (0, 0, 0, 0))
        b.alpha_composite(img, (int(dx), int(dy)))
        img = b
    crop = img.crop(WIN)
    if tint > 0 or bright > 0:
        arr = np.asarray(crop).astype(np.float32)
        al = arr[:, :, 3:4] / 255.0
        if tint > 0:
            arr[:, :, :3] = arr[:, :, :3] * (1 - tint * al) + np.array([255, 50, 40], np.float32) * (tint * al)
        if bright > 0:
            arr[:, :, :3] = arr[:, :, :3] * (1 - bright * al) + 255 * (bright * al)
        crop = Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")
    return crop


def _bg():
    return Image.new("RGBA", (CW, CH), (26, 28, 36, 255))


def comp(c, behind=None, front=None):
    cv = _bg()
    if behind:
        cv.alpha_composite(behind)
    cv.alpha_composite(c)
    if front:
        cv.alpha_composite(front)
    return cv.convert("RGB")


def _speed(k):
    fx = Image.new("RGBA", (CW, CH), (0, 0, 0, 0)); d = ImageDraw.Draw(fx)
    for i in range(4):
        y = CYL - 70 + i * 46; x2 = CXL - 70 - i * 16
        d.line([(x2 - 120 * k, y), (x2, y)], fill=(180, 230, 255, int(150 * k)), width=5)
    return fx


def _impact(k):
    fx = Image.new("RGBA", (CW, CH), (0, 0, 0, 0)); d = ImageDraw.Draw(fx)
    for i in range(10):
        a = i / 10 * 2 * math.pi; r1 = 30 + 60 * k
        d.line([(CXL + math.cos(a) * r1, CYL + math.sin(a) * r1),
                (CXL + math.cos(a) * (r1 + 70), CYL + math.sin(a) * (r1 + 70))],
               fill=(255, 90, 70, int(220 * (1 - k))), width=6)
    return fx


def _ring(k):
    fx = Image.new("RGBA", (CW, CH), (0, 0, 0, 0)); d = ImageDraw.Draw(fx)
    r = 40 + 250 * k; al = int(200 * (1 - k))
    d.ellipse([CXL - r, CYL - r, CXL + r, CYL + r], outline=(180, 240, 255, al), width=10)
    d.ellipse([CXL - r, CYL - r, CXL + r, CYL + r], outline=(255, 255, 255, al), width=4)
    for i in range(12):
        a = i / 12 * 2 * math.pi; x = CXL + math.cos(a) * r * 0.9; y = CYL + math.sin(a) * r * 0.9; s = 6 * (1 - k) + 2
        d.ellipse([x - s, y - s, x + s, y + s], fill=(255, 255, 255, al))
    return fx


def build(IM, out_dir, name):
    n = lambda nm, **kw: char(IM, nm, **kw)
    F = []
    for i in range(8): F.append(comp(n("NEUTRAL", dy=-math.sin(i * 0.5) * 4)))
    for i in range(14): F.append(comp(n("NEUTRAL", dy=-abs(math.sin(i * 0.55)) * 13)))
    for i in range(5):
        k = (i + 1) / 5.0; F.append(comp(n("ATTACK", dx=14 * k, ang=4 * k), behind=_speed(k)))
    for i in range(3): F.append(comp(n("ATTACK", dx=14, ang=4), behind=_speed(1.0)))
    for i in range(4):
        k = 1 - (i + 1) / 4.0; F.append(comp(n("ATTACK", dx=14 * k, ang=4 * k)))
    for i in range(5):
        k = (i + 1) / 5.0; F.append(comp(n("HURT", dy=10 * k, dx=-8 * k, ang=-6 * k, tint=0.6 * (1 - k * 0.3)), front=_impact(k)))
    for i in range(4):
        k = 1 - (i + 1) / 4.0; F.append(comp(n("HURT", dy=10 * k, dx=-8 * k, ang=-6 * k, tint=0.15 * k)))
    for i in range(9):
        k = (i + 1) / 9.0; F.append(comp(n("EVOLVE", dy=-16 * k, scale=1 + 0.06 * k, bright=0.7 * k), front=_ring(k)))
    for i in range(4):
        k = 1 - (i + 1) / 4.0; F.append(comp(n("EVOLVE", dy=-16 * (0.4 + 0.6 * k), scale=1 + 0.06 * k, bright=0.6 * k)))
    for i in range(7): F.append(comp(n("NEUTRAL", dy=-math.sin(i * 0.5) * 4)))
    os.makedirs(out_dir, exist_ok=True)
    gp = os.path.join(out_dir, name + "_fx.gif")
    F[0].save(gp, save_all=True, append_images=F[1:], duration=72, loop=0, optimize=True)
    picks = [3, 15, 25, 37, 48, 59]
    labels = ["idle", "walk", "ATTACK", "DAMAGE", "EVOLVE", "settle"]
    cell = 200
    strip = Image.new("RGB", (cell * 6, cell + 18), (250, 250, 250)); d = ImageDraw.Draw(strip)
    for j, p in enumerate(picks):
        th = F[min(p, len(F) - 1)].copy(); th.thumbnail((cell - 8, cell - 8), Image.LANCZOS)
        strip.paste(th, (j * cell + 4, 16)); d.text((j * cell + 6, 3), labels[j], fill=(0, 0, 0))
    sp = os.path.join(out_dir, name + "_strip.png")
    strip.save(sp)
    return gp, sp


def _find_in_dir(folder):
    """Find exactly one PNG per expression by suffix (..._NEUTRAL.png etc.) at the top level."""
    found = {}
    for f in sorted(os.listdir(folder)):
        if not f.lower().endswith(".png"):
            continue
        up = f.upper()
        for key in ("NEUTRAL", "HURT", "ATTACK", "EVOLVE"):
            if up.endswith(key + ".PNG"):
                found.setdefault(key, []).append(os.path.join(folder, f))
    paths = {}
    for key in ("NEUTRAL", "HURT", "ATTACK", "EVOLVE"):
        hits = found.get(key, [])
        if len(hits) != 1:
            raise SystemExit("ERROR: need exactly one *%s.png in folder, found %d: %s" % (key, len(hits), hits))
        paths[key] = hits[0]
    return paths


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--dir", dest="dir", default=None, help="folder containing the 4 *_NEUTRAL/_HURT/_ATTACK/_EVOLVE.png")
    ap.add_argument("--neutral")
    ap.add_argument("--hurt")
    ap.add_argument("--attack")
    ap.add_argument("--evolve")
    ap.add_argument("--out", required=True)
    ap.add_argument("--name", required=True)
    a = ap.parse_args()
    if a.dir:
        p = _find_in_dir(a.dir)
        IM = {k: normalize(p[k]) for k in p}
    else:
        if not all([a.neutral, a.hurt, a.attack, a.evolve]):
            raise SystemExit("ERROR: give --dir, or all of --neutral/--hurt/--attack/--evolve")
        IM = {"NEUTRAL": normalize(a.neutral), "HURT": normalize(a.hurt),
              "ATTACK": normalize(a.attack), "EVOLVE": normalize(a.evolve)}
    gp, sp = build(IM, a.out, a.name)
    print("gif   =", gp)
    print("strip =", sp)
    print("DONE")


if __name__ == "__main__":
    main()
