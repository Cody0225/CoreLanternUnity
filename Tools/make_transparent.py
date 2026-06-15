# -*- coding: utf-8 -*-
"""
make_transparent.py — Batch background removal + optional resize for character PNGs.

Removes a SOLID-color background (magenta / white / any) by flooding from the 4 corners
(border-connected only, so same-color areas INSIDE the character are protected). Optionally
fits the character into a square canvas (aspect-preserving, centered, transparent).
Already-transparent inputs are passed through (and only resized if --size is given).

Run with the real Python (py):
  # just make transparent (keep original size), output to <in>_transparent:
  py Tools/make_transparent.py --in "C:\\path\\to\\folder"
  # transparent + fit into 512x512 centered:
  py Tools/make_transparent.py --in "C:\\path\\to\\folder" --out "C:\\path\\out" --size 512
  # single file:
  py Tools/make_transparent.py --in "C:\\path\\img.png" --size 1024
"""
import os, sys, argparse
import numpy as np
from PIL import Image
from scipy import ndimage

BG_TOL = 42.0      # per-pixel RGB distance treated as background
EDGE_ERODE = 1     # px, eat anti-aliased fringe (anti-halo)


def corner_bg(rgb):
    h, w, _ = rgb.shape; s = 6
    patches = [rgb[:s, :s], rgb[:s, w - s:], rgb[h - s:, :s], rgb[h - s:, w - s:]]
    return np.median(np.concatenate([p.reshape(-1, 3) for p in patches], axis=0), axis=0)


def remove_bg(im):
    """Return RGBA with solid background made transparent. Position/size preserved."""
    if im.mode in ("RGBA", "LA"):
        a = np.asarray(im.convert("RGBA"))[:, :, 3]
        if a.min() < 250 and max(a[0, 0], a[0, -1], a[-1, 0], a[-1, -1]) == 0:
            return im.convert("RGBA"), "already-transparent"
    rgb = np.asarray(im.convert("RGB")).astype(np.float32)
    bg = corner_bg(rgb)
    dist = np.sqrt(((rgb - bg) ** 2).sum(axis=2))
    bgmask = dist < BG_TOL
    lbl, _ = ndimage.label(bgmask)
    border = set(np.unique(np.concatenate([lbl[0, :], lbl[-1, :], lbl[:, 0], lbl[:, -1]]))); border.discard(0)
    fg = ~np.isin(lbl, list(border))
    if EDGE_ERODE > 0:
        fg = ndimage.binary_erosion(fg, iterations=EDGE_ERODE, border_value=0)
    alpha = np.where(fg, 255, 0).astype(np.uint8)
    return Image.fromarray(np.dstack([rgb.astype(np.uint8), alpha]), "RGBA"), "colorkey(bg=%d,%d,%d)" % tuple(int(x) for x in bg)


def fit_square(im, size, margin=12):
    """Crop to alpha bbox, scale to fit (size-2*margin) preserving aspect, center on transparent square."""
    a = np.asarray(im)[:, :, 3]
    ys, xs = np.where(a > 0)
    if xs.size == 0:
        return im.resize((size, size))
    crop = im.crop((xs.min(), ys.min(), xs.max() + 1, ys.max() + 1))
    cw, ch = crop.size
    sc = (size - 2 * margin) / max(cw, ch)
    nw, nh = max(1, round(cw * sc)), max(1, round(ch * sc))
    rs = crop.resize((nw, nh), Image.LANCZOS)
    cv = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    cv.paste(rs, ((size - nw) // 2, (size - nh) // 2), rs)
    return cv


def process(path_in, path_out, size):
    im = Image.open(path_in)
    out, mode = remove_bg(im)
    if size:
        out = fit_square(out, size)
    out.save(path_out)
    a = np.asarray(out)[:, :, 3]
    corners = (int(a[0, 0]), int(a[0, -1]), int(a[-1, 0]), int(a[-1, -1]))
    print("%-44s | %s | %dx%d corners=%s" % (os.path.basename(path_out), mode, out.size[0], out.size[1], corners))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--in", dest="inp", required=True, help="folder or single .png")
    ap.add_argument("--out", dest="out", default=None, help="output folder (default: <in>_transparent)")
    ap.add_argument("--size", type=int, default=0, help="fit into NxN square (omit to keep original size)")
    a = ap.parse_args()
    inp = a.inp
    if os.path.isdir(inp):
        out_dir = a.out or (inp.rstrip("/\\") + "_transparent")
        os.makedirs(out_dir, exist_ok=True)
        files = sorted(f for f in os.listdir(inp) if f.lower().endswith(".png"))
        print("== %d PNG -> %s ==" % (len(files), out_dir))
        for f in files:
            process(os.path.join(inp, f), os.path.join(out_dir, f), a.size)
    else:
        out_dir = a.out or os.path.dirname(inp)
        os.makedirs(out_dir, exist_ok=True)
        process(inp, os.path.join(out_dir, os.path.basename(inp)), a.size)
    print("DONE")


if __name__ == "__main__":
    main()
