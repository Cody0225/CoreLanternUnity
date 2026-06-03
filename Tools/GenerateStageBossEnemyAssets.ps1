param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource"
New-Item -ItemType Directory -Force -Path $SkinDir | Out-Null
New-Item -ItemType Directory -Force -Path $ArtDir | Out-Null

function Ensure-TextureMeta {
    param([string]$AssetPath)
    $metaPath = $AssetPath + ".meta"
    if (Test-Path -LiteralPath $metaPath) {
        return
    }

    $templatePath = Join-Path $SkinDir "Runner.png.meta"
    if (-not (Test-Path -LiteralPath $templatePath)) {
        $templatePath = Join-Path $SkinDir "StageThumb_Arena.png.meta"
    }
    if (-not (Test-Path -LiteralPath $templatePath)) {
        Write-Warning ("No texture meta template found for " + $AssetPath)
        return
    }

    $text = [System.IO.File]::ReadAllText($templatePath)
    $guid = [guid]::NewGuid().ToString("N")
    $text = [regex]::Replace($text, "guid: [0-9a-fA-F]+", "guid: $guid", 1)
    [System.IO.File]::WriteAllText($metaPath, $text, [System.Text.UTF8Encoding]::new($false))
}

function New-Color {
    param([int]$R, [int]$G, [int]$B, [int]$A = 255)
    return [System.Drawing.Color]::FromArgb($A, $R, $G, $B)
}

function With-Alpha {
    param([System.Drawing.Color]$Color, [int]$A)
    return [System.Drawing.Color]::FromArgb($A, $Color.R, $Color.G, $Color.B)
}

function New-Canvas {
    param([int]$W = 256, [int]$H = 256)
    $bmp = [System.Drawing.Bitmap]::new($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $bmp.SetResolution(96, 96)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    return @{ Bitmap = $bmp; Graphics = $g }
}

function New-Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 2)
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    return $pen
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(6, [int]($Color.A / ($mul + 1.4))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 1.6))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    foreach ($mul in 2.1, 1.55, 1.18) {
        $a = [Math]::Max(8, [int]($Color.A / ($mul * 3.0)))
        $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Color $a))
        $cx = $X + $W / 2
        $cy = $Y + $H / 2
        $ww = $W * $mul
        $hh = $H * $mul
        $G.FillEllipse($brush, $cx - $ww / 2, $cy - $hh / 2, $ww, $hh)
        $brush.Dispose()
    }
    $core = [System.Drawing.SolidBrush]::new($Color)
    $G.FillEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-Poly {
    param($G, [object[]]$Points, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke, [float]$StrokeWidth = 3)
    $pts = @()
    foreach ($p in $Points) {
        $pts += [System.Drawing.PointF]::new([float]$p[0], [float]$p[1])
    }
    $brush = [System.Drawing.SolidBrush]::new($Fill)
    $G.FillPolygon($brush, $pts)
    $brush.Dispose()
    $pen = New-Pen $Stroke $StrokeWidth
    $G.DrawPolygon($pen, $pts)
    $pen.Dispose()
}

function Draw-GlowRect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4, 2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 2))))) ($Width * $mul)
        $G.DrawRectangle($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawRectangle($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-RoundRect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [float]$Radius, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke, [float]$StrokeWidth = 2)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $d = $Radius * 2
    $path.AddArc($X, $Y, $d, $d, 180, 90)
    $path.AddArc($X + $W - $d, $Y, $d, $d, 270, 90)
    $path.AddArc($X + $W - $d, $Y + $H - $d, $d, $d, 0, 90)
    $path.AddArc($X, $Y + $H - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    $brush = [System.Drawing.SolidBrush]::new($Fill)
    $G.FillPath($brush, $path)
    $brush.Dispose()
    $pen = New-Pen $Stroke $StrokeWidth
    $G.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
}

function Draw-EnergyCore {
    param($G, [float]$X, [float]$Y, [float]$Size, [System.Drawing.Color]$Color)
    Fill-GlowEllipse $G ($X - $Size / 2) ($Y - $Size / 2) $Size $Size (With-Alpha $Color 220)
    Draw-GlowEllipse $G ($X - $Size * 0.72) ($Y - $Size * 0.72) ($Size * 1.44) ($Size * 1.44) (With-Alpha $Color 150) 2
    Fill-GlowEllipse $G ($X - $Size * 0.18) ($Y - $Size * 0.18) ($Size * 0.36) ($Size * 0.36) (New-Color 242 255 255 230)
}

function Draw-Segment {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke, [float]$StrokeWidth = 3)
    Fill-RoundRect $G ($X - $W / 2) ($Y - $H / 2) $W $H ([Math]::Min($W, $H) * 0.24) $Fill $Stroke $StrokeWidth
}

function Save-Canvas {
    param($Canvas, [string]$FileName)
    $path = Join-Path $SkinDir $FileName
    $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $path
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function New-LavaCrawler {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $lava = New-Color 255 116 22 235
    $hot = New-Color 255 218 78 225
    $dark = New-Color 33 18 14 242
    Draw-GlowLine $g 52 168 22 206 (With-Alpha $lava 140) 4
    Draw-GlowLine $g 204 168 234 206 (With-Alpha $lava 140) 4
    Draw-GlowLine $g 65 124 26 112 (With-Alpha $lava 120) 3
    Draw-GlowLine $g 191 124 230 112 (With-Alpha $lava 120) 3
    Draw-Poly $g @(@(44,144),@(73,103),@(118,86),@(172,98),@(210,139),@(194,181),@(132,205),@(70,182)) $dark (With-Alpha $lava 210) 4
    Draw-Poly $g @(@(76,130),@(105,104),@(146,108),@(178,134),@(165,166),@(122,178),@(83,160)) (New-Color 80 36 20 232) (With-Alpha $hot 210) 2.8
    Draw-GlowLine $g 93 128 118 165 $hot 3
    Draw-GlowLine $g 148 113 128 176 $hot 3
    Draw-GlowLine $g 168 138 104 144 (With-Alpha $lava 210) 2.5
    Draw-EnergyCore $g 126 142 30 $hot
    Fill-GlowEllipse $g 71 89 24 22 (New-Color 255 182 55 230)
    Fill-GlowEllipse $g 164 91 24 22 (New-Color 255 182 55 230)
    Draw-GlowEllipse $g 45 96 166 118 (With-Alpha $lava 105) 3
    Save-Canvas $canvas "LavaCrawler.png"
}

function New-MagmaTitan {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $lava = New-Color 255 118 24 238
    $hot = New-Color 255 221 76 226
    $basalt = New-Color 27 29 31 245
    Draw-Poly $g @(@(38,93),@(72,58),@(105,80),@(94,136),@(55,143)) (New-Color 40 38 35 242) (With-Alpha $lava 180) 4
    Draw-Poly $g @(@(218,93),@(184,58),@(151,80),@(162,136),@(201,143)) (New-Color 40 38 35 242) (With-Alpha $lava 180) 4
    Draw-Poly $g @(@(87,58),@(128,28),@(169,58),@(177,141),@(145,207),@(111,207),@(79,141)) $basalt (With-Alpha $lava 210) 5
    Fill-GlowEllipse $g 101 78 54 54 (New-Color 72 35 22 235)
    Draw-EnergyCore $g 128 107 34 $hot
    Draw-GlowLine $g 103 68 97 177 (With-Alpha $lava 220) 3
    Draw-GlowLine $g 153 68 159 177 (With-Alpha $lava 220) 3
    Draw-GlowLine $g 94 148 163 151 (With-Alpha $hot 200) 2.5
    Draw-Poly $g @(@(90,203),@(115,203),@(112,236),@(75,238)) (New-Color 35 35 35 242) (With-Alpha $lava 180) 3
    Draw-Poly $g @(@(141,203),@(166,203),@(181,238),@(144,236)) (New-Color 35 35 35 242) (With-Alpha $lava 180) 3
    Draw-GlowLine $g 66 83 31 49 (With-Alpha $hot 170) 3
    Draw-GlowLine $g 190 83 225 49 (With-Alpha $hot 170) 3
    Draw-GlowEllipse $g 63 35 130 188 (With-Alpha $lava 90) 4
    Save-Canvas $canvas "Enemy_MagmaTitan.png"
}

function New-CorruptionDrone {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $mag = New-Color 245 58 255 232
    $cyan = New-Color 55 232 255 210
    $dark = New-Color 22 12 38 236
    Draw-GlowLine $g 23 128 233 128 (With-Alpha $mag 110) 3
    Draw-GlowLine $g 128 27 128 229 (With-Alpha $cyan 90) 2
    Draw-Poly $g @(@(128,39),@(180,91),@(180,165),@(128,217),@(76,165),@(76,91)) $dark (With-Alpha $mag 210) 4
    Draw-Poly $g @(@(128,67),@(157,105),@(147,153),@(128,181),@(109,153),@(99,105)) (New-Color 88 28 120 222) (With-Alpha $cyan 210) 3
    Draw-EnergyCore $g 128 128 36 $mag
    Draw-Poly $g @(@(55,96),@(14,118),@(55,139),@(83,121)) (New-Color 20 31 48 218) (With-Alpha $cyan 170) 3
    Draw-Poly $g @(@(201,96),@(242,118),@(201,139),@(173,121)) (New-Color 20 31 48 218) (With-Alpha $cyan 170) 3
    Draw-GlowLine $g 79 84 43 55 (With-Alpha $mag 170) 3
    Draw-GlowLine $g 177 172 216 204 (With-Alpha $mag 170) 3
    Draw-GlowRect $g 190 49 15 15 (With-Alpha $mag 185) 2
    Draw-GlowRect $g 48 189 11 11 (With-Alpha $cyan 165) 2
    Draw-GlowEllipse $g 57 57 142 142 (With-Alpha $mag 86) 3
    Save-Canvas $canvas "Enemy_CorruptionDrone.png"
}

function New-FrostKnight {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $ice = New-Color 126 236 255 228
    $white = New-Color 231 255 255 230
    $deep = New-Color 26 47 72 242
    Draw-Poly $g @(@(128,22),@(158,70),@(144,112),@(112,112),@(98,70)) (New-Color 187 235 255 206) (With-Alpha $ice 210) 3
    Draw-Poly $g @(@(61,87),@(100,69),@(109,132),@(75,168),@(45,137)) (New-Color 31 62 92 238) (With-Alpha $ice 190) 4
    Draw-Poly $g @(@(195,87),@(156,69),@(147,132),@(181,168),@(211,137)) (New-Color 31 62 92 238) (With-Alpha $ice 190) 4
    Draw-Poly $g @(@(128,56),@(175,98),@(164,178),@(128,218),@(92,178),@(81,98)) $deep (With-Alpha $ice 220) 5
    Draw-EnergyCore $g 128 126 34 $white
    Draw-Poly $g @(@(107,144),@(149,144),@(139,188),@(117,188)) (New-Color 167 231 255 170) (With-Alpha $white 185) 2.5
    Draw-GlowLine $g 71 72 38 34 (With-Alpha $white 165) 3
    Draw-GlowLine $g 185 72 218 34 (With-Alpha $white 165) 3
    Draw-GlowLine $g 86 172 56 221 (With-Alpha $ice 165) 3
    Draw-GlowLine $g 170 172 200 221 (With-Alpha $ice 165) 3
    Draw-GlowEllipse $g 57 41 142 178 (With-Alpha $ice 80) 4
    Save-Canvas $canvas "Enemy_FrostKnight.png"
}

function New-VoltDasher {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $volt = New-Color 255 238 64 235
    $purple = New-Color 162 66 255 220
    $dark = New-Color 28 23 47 235
    Draw-GlowLine $g 39 198 92 143 $volt 4
    Draw-GlowLine $g 164 112 221 50 $volt 4
    Draw-GlowLine $g 69 74 120 119 (With-Alpha $purple 190) 3
    Draw-Poly $g @(@(35,159),@(105,82),@(183,80),@(220,117),@(159,132),@(116,188)) $dark (With-Alpha $purple 210) 4
    Draw-Poly $g @(@(93,126),@(142,69),@(134,111),@(199,103),@(134,136),@(125,184)) (New-Color 255 214 42 224) (With-Alpha $volt 240) 3
    Draw-EnergyCore $g 151 112 22 $volt
    Draw-GlowLine $g 71 164 29 200 (With-Alpha $volt 180) 3
    Draw-GlowLine $g 172 134 229 152 (With-Alpha $purple 175) 3
    Fill-GlowEllipse $g 194 101 18 18 (New-Color 255 253 170 230)
    Draw-GlowEllipse $g 45 60 165 126 (With-Alpha $volt 70) 3
    Save-Canvas $canvas "Enemy_VoltDasher.png"
}

function New-Pulswyrm {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $amber = New-Color 255 174 45 232
    $cyan = New-Color 69 232 255 205
    $body = New-Color 45 36 52 242
    Draw-Segment $g 66 166 54 38 $body (With-Alpha $amber 170) 3
    Draw-Segment $g 98 149 61 44 (New-Color 50 39 55 242) (With-Alpha $amber 190) 3
    Draw-Segment $g 132 128 70 51 (New-Color 57 43 58 242) (With-Alpha $amber 210) 4
    Draw-Segment $g 168 104 82 62 (New-Color 65 46 58 245) (With-Alpha $amber 230) 5
    Draw-Poly $g @(@(179,66),@(219,90),@(220,132),@(183,160),@(143,136),@(144,92)) (New-Color 70 47 61 244) (With-Alpha $amber 235) 5
    Draw-EnergyCore $g 184 112 34 $amber
    Draw-GlowLine $g 140 84 95 49 (With-Alpha $cyan 180) 3
    Draw-GlowLine $g 151 161 106 210 (With-Alpha $cyan 180) 3
    Draw-GlowLine $g 208 78 231 40 (With-Alpha $amber 175) 3
    Draw-GlowLine $g 210 146 238 174 (With-Alpha $amber 175) 3
    Fill-GlowEllipse $g 196 96 16 16 (New-Color 238 255 255 235)
    Fill-GlowEllipse $g 196 125 16 16 (New-Color 238 255 255 235)
    Draw-GlowEllipse $g 31 48 196 166 (With-Alpha $cyan 75) 3
    Save-Canvas $canvas "Boss_Pulswyrm.png"
}

function New-Nullwyrm {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $mag = New-Color 245 54 255 235
    $cyan = New-Color 64 230 255 210
    $gold = New-Color 255 211 75 205
    $dark = New-Color 17 16 31 246
    Draw-Segment $g 55 166 48 34 $dark (With-Alpha $mag 150) 3
    Draw-Segment $g 83 139 60 40 (New-Color 22 18 40 244) (With-Alpha $mag 170) 3
    Draw-Segment $g 118 113 72 46 (New-Color 27 20 45 245) (With-Alpha $mag 190) 4
    Draw-Segment $g 158 91 78 52 (New-Color 33 22 50 245) (With-Alpha $mag 210) 4
    Draw-Poly $g @(@(183,43),@(229,69),@(226,126),@(189,167),@(145,145),@(136,87)) $dark (With-Alpha $mag 235) 5
    Draw-Poly $g @(@(173,64),@(211,82),@(208,116),@(183,143),@(157,128),@(153,91)) (New-Color 67 29 91 225) (With-Alpha $cyan 205) 3
    Draw-EnergyCore $g 188 104 42 $mag
    Draw-GlowLine $g 144 75 92 34 (With-Alpha $cyan 185) 3
    Draw-GlowLine $g 144 148 88 211 (With-Alpha $cyan 185) 3
    Draw-GlowLine $g 205 61 242 29 (With-Alpha $gold 185) 3
    Draw-GlowLine $g 213 130 247 151 (With-Alpha $gold 185) 3
    Draw-GlowLine $g 105 113 178 111 (With-Alpha $cyan 200) 2.6
    Draw-GlowLine $g 78 143 154 91 (With-Alpha $mag 180) 2.4
    Fill-GlowEllipse $g 204 87 17 17 (New-Color 250 255 255 235)
    Fill-GlowEllipse $g 198 124 13 13 (New-Color 250 255 255 220)
    Draw-GlowEllipse $g 22 34 214 190 (With-Alpha $mag 80) 4
    Draw-GlowEllipse $g 52 59 155 132 (With-Alpha $cyan 55) 3
    Save-Canvas $canvas "Boss_Nullwyrm.png"
}

function Draw-Preview {
    $targets = @(
        @{ Name = "LavaCrawler"; File = "LavaCrawler.png"; Color = New-Color 255 132 36 230 },
        @{ Name = "MagmaTitan"; File = "Enemy_MagmaTitan.png"; Color = New-Color 255 132 36 230 },
        @{ Name = "CorruptionDrone"; File = "Enemy_CorruptionDrone.png"; Color = New-Color 235 64 255 230 },
        @{ Name = "FrostKnight"; File = "Enemy_FrostKnight.png"; Color = New-Color 120 235 255 230 },
        @{ Name = "VoltDasher"; File = "Enemy_VoltDasher.png"; Color = New-Color 255 236 68 230 },
        @{ Name = "Pulswyrm"; File = "Boss_Pulswyrm.png"; Color = New-Color 255 174 45 230 },
        @{ Name = "Nullwyrm"; File = "Boss_Nullwyrm.png"; Color = New-Color 245 54 255 230 }
    )
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new((New-Color 3 10 16 255))
    $g.FillRectangle($bg, 0, 0, 1280, 560)
    $bg.Dispose()
    $titleFont = [System.Drawing.Font]::new("Segoe UI", 22, [System.Drawing.FontStyle]::Bold)
    $labelFont = [System.Drawing.Font]::new("Segoe UI", 13, [System.Drawing.FontStyle]::Bold)
    $subFont = [System.Drawing.Font]::new("Segoe UI", 9, [System.Drawing.FontStyle]::Regular)
    $titleBrush = [System.Drawing.SolidBrush]::new((New-Color 168 255 255 255))
    $g.DrawString("Stage/Boss Enemy Sprite Pack", $titleFont, $titleBrush, 32, 24)
    $g.DrawString("Transparent 256x256 PNGs / loaded by existing Skins resource paths", $subFont, $titleBrush, 34, 55)
    $titleBrush.Dispose()

    for ($i = 0; $i -lt $targets.Count; $i++) {
        $col = $i % 4
        $row = [Math]::Floor($i / 4)
        $x = 34 + $col * 306
        $y = 105 + $row * 270
        $color = $targets[$i].Color
        $panel = [System.Drawing.SolidBrush]::new((New-Color 7 22 30 255))
        $g.FillRectangle($panel, $x, $y, 270, 236)
        $panel.Dispose()
        $pen = New-Pen $color 2
        $g.DrawRectangle($pen, $x, $y, 270, 236)
        $pen.Dispose()
        Draw-GlowLine $g ($x + 16) ($y + 15) ($x + 132) ($y + 15) (With-Alpha $color 190) 2
        $path = Join-Path $SkinDir $targets[$i].File
        $img = [System.Drawing.Image]::FromFile($path)
        try {
            $g.DrawImage($img, [System.Drawing.RectangleF]::new($x + 58, $y + 18, 154, 154))
        }
        finally {
            $img.Dispose()
        }
        $brush = [System.Drawing.SolidBrush]::new((New-Color 225 249 255 255))
        $sf = [System.Drawing.StringFormat]::new()
        $sf.Alignment = [System.Drawing.StringAlignment]::Center
        $g.DrawString($targets[$i].Name, $labelFont, $brush, [System.Drawing.RectangleF]::new($x + 8, $y + 180, 254, 30), $sf)
        $sf.Dispose()
        $brush.Dispose()
    }
    $titleFont.Dispose()
    $labelFont.Dispose()
    $subFont.Dispose()
    $previewPath = Join-Path $ArtDir "Generated_StageBossEnemy_Preview.png"
    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $previewPath
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

$targets = @(
    "LavaCrawler.png",
    "Enemy_MagmaTitan.png",
    "Enemy_CorruptionDrone.png",
    "Enemy_FrostKnight.png",
    "Enemy_VoltDasher.png",
    "Boss_Pulswyrm.png",
    "Boss_Nullwyrm.png"
)

if (-not $Force) {
    $existing = $targets | Where-Object { Test-Path -LiteralPath (Join-Path $SkinDir $_) }
    if ($existing.Count -gt 0) {
        Write-Host "Existing stage/boss enemy PNGs found. Use -Force to overwrite:"
        $existing | ForEach-Object { Write-Host (" - " + $_) }
        return
    }
}

New-LavaCrawler
New-MagmaTitan
New-CorruptionDrone
New-FrostKnight
New-VoltDasher
New-Pulswyrm
New-Nullwyrm
Draw-Preview

Write-Host "Generated stage/boss enemy sprites:"
$targets | ForEach-Object { Write-Host (" - " + $_) }
Write-Host ("Preview: " + (Join-Path $ArtDir "Generated_StageBossEnemy_Preview.png"))
