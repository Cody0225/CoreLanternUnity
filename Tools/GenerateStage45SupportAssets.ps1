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

    $templatePath = Join-Path $SkinDir "StageThumb_Arena.png.meta"
    if (-not (Test-Path -LiteralPath $templatePath)) {
        $templatePath = Join-Path $SkinDir "Runner.png.meta"
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
    param([int]$W, [int]$H)
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
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 1.4))))) ($Width * $mul)
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
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 1.5))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    foreach ($mul in 2.0, 1.45, 1.15) {
        $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Color ([Math]::Max(6, [int]($Color.A / ($mul * 3.0))))))
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

function Save-Bitmap {
    param($Canvas, [string]$FileName)
    $path = Join-Path $SkinDir $FileName
    $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $path
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function Set-BitmapOpaque {
    param([System.Drawing.Bitmap]$Bitmap)
    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            $c = $Bitmap.GetPixel($x, $y)
            if ($c.A -lt 255) {
                $Bitmap.SetPixel($x, $y, [System.Drawing.Color]::FromArgb(255, $c.R, $c.G, $c.B))
            }
        }
    }
}

function Fill-ThumbBackground {
    param($G, [int]$W, [int]$H, [System.Drawing.Color]$Top, [System.Drawing.Color]$Bottom)
    $rect = [System.Drawing.Rectangle]::new(0, 0, $W, $H)
    $brush = [System.Drawing.Drawing2D.LinearGradientBrush]::new($rect, $Top, $Bottom, 90)
    $G.FillRectangle($brush, $rect)
    $brush.Dispose()
}

function New-StageThumbFrost {
    $canvas = New-Canvas 160 90
    $g = $canvas.Graphics
    Fill-ThumbBackground $g 160 90 (New-Color 8 24 38 255) (New-Color 31 70 96 255)
    $ice = New-Color 116 230 255 210
    $white = New-Color 230 255 255 225
    Draw-GlowLine $g 0 65 160 54 (With-Alpha $ice 90) 1.8
    Draw-GlowLine $g 15 72 145 70 (With-Alpha $white 80) 1.3
    Draw-Poly $g @(@(18,82),@(47,31),@(75,82)) (New-Color 22 62 88 215) (With-Alpha $ice 160) 2
    Draw-Poly $g @(@(58,83),@(100,18),@(145,83)) (New-Color 26 74 106 220) (With-Alpha $white 155) 2
    Draw-Poly $g @(@(92,83),@(126,37),@(161,83)) (New-Color 18 54 83 215) (With-Alpha $ice 145) 2
    Draw-GlowLine $g 44 32 52 76 (With-Alpha $white 105) 1
    Draw-GlowLine $g 101 18 111 78 (With-Alpha $white 120) 1
    Draw-GlowEllipse $g 52 23 58 58 (With-Alpha $ice 50) 2
    Set-BitmapOpaque $canvas.Bitmap
    Save-Bitmap $canvas "StageThumb_Frost.png"
}

function New-StageThumbStorm {
    $canvas = New-Canvas 160 90
    $g = $canvas.Graphics
    Fill-ThumbBackground $g 160 90 (New-Color 18 8 37 255) (New-Color 52 25 83 255)
    $violet = New-Color 166 66 255 210
    $bolt = New-Color 255 235 71 230
    $cyan = New-Color 70 226 255 170
    Draw-GlowEllipse $g -18 23 82 82 (With-Alpha $violet 75) 2.5
    Draw-GlowEllipse $g 96 -14 78 78 (With-Alpha $cyan 65) 2
    Draw-GlowLine $g 25 20 65 56 (With-Alpha $violet 130) 2
    Draw-GlowLine $g 106 18 145 50 (With-Alpha $cyan 120) 2
    Draw-Poly $g @(@(86,7),@(64,42),@(82,41),@(62,83),@(109,33),@(90,35)) (With-Alpha $bolt 230) (With-Alpha $bolt 245) 2
    Draw-GlowLine $g 0 71 160 70 (With-Alpha $violet 60) 1
    Draw-GlowLine $g 18 79 143 77 (With-Alpha $bolt 60) 1
    Set-BitmapOpaque $canvas.Bitmap
    Save-Bitmap $canvas "StageThumb_Storm.png"
}

function New-FrostCrystal {
    $canvas = New-Canvas 96 96
    $g = $canvas.Graphics
    $ice = New-Color 116 235 255 225
    $white = New-Color 238 255 255 235
    Draw-GlowEllipse $g 12 12 72 72 (With-Alpha $ice 70) 3
    Draw-Poly $g @(@(48,5),@(76,31),@(64,84),@(32,84),@(20,31)) (New-Color 48 102 135 215) (With-Alpha $ice 230) 3
    Draw-Poly $g @(@(48,14),@(62,35),@(55,75),@(41,75),@(34,35)) (New-Color 156 236 255 178) (With-Alpha $white 210) 2
    Draw-GlowLine $g 48 7 48 85 (With-Alpha $white 140) 1.5
    Draw-GlowLine $g 24 33 72 33 (With-Alpha $white 110) 1.2
    Save-Bitmap $canvas "Stage4_FrostCrystal_A.png"
}

function New-FrostPatch {
    $canvas = New-Canvas 256 256
    $g = $canvas.Graphics
    $ice = New-Color 112 230 255 118
    $white = New-Color 230 255 255 145
    Fill-GlowEllipse $g 36 42 184 170 (With-Alpha $ice 72)
    Draw-GlowEllipse $g 43 49 170 154 (With-Alpha $white 120) 3
    Draw-GlowEllipse $g 69 70 118 112 (With-Alpha $ice 88) 2
    foreach ($line in @(
        @(56,122,202,122), @(92,72,158,186), @(70,176,186,82),
        @(128,56,128,206), @(52,94,204,160)
    )) {
        Draw-GlowLine $g $line[0] $line[1] $line[2] $line[3] (With-Alpha $white 95) 1.6
    }
    Save-Bitmap $canvas "Stage4_FrostPatch_A.png"
}

function New-LightningMarker {
    $canvas = New-Canvas 256 256
    $g = $canvas.Graphics
    $warn = New-Color 255 216 60 160
    $white = New-Color 255 255 230 175
    Fill-GlowEllipse $g 42 42 172 172 (With-Alpha $warn 58)
    Draw-GlowEllipse $g 46 46 164 164 (With-Alpha $warn 190) 4
    Draw-GlowEllipse $g 79 79 98 98 (With-Alpha $white 115) 2
    Draw-GlowLine $g 128 25 128 63 (With-Alpha $white 150) 3
    Draw-GlowLine $g 128 193 128 231 (With-Alpha $white 150) 3
    Draw-GlowLine $g 25 128 63 128 (With-Alpha $white 150) 3
    Draw-GlowLine $g 193 128 231 128 (With-Alpha $white 150) 3
    Draw-Poly $g @(@(139,73),@(107,130),@(127,128),@(109,186),@(153,115),@(132,117)) (With-Alpha $white 185) (With-Alpha (New-Color 255 238 78 220) 220) 2
    Save-Bitmap $canvas "Stage5_LightningMarker_A.png"
}

function New-LightningStrike {
    $canvas = New-Canvas 128 512
    $g = $canvas.Graphics
    $bolt = New-Color 255 244 92 235
    $white = New-Color 255 255 240 245
    Draw-GlowLine $g 76 14 45 151 (With-Alpha $bolt 210) 8
    Draw-GlowLine $g 45 151 78 236 (With-Alpha $white 230) 10
    Draw-GlowLine $g 78 236 39 364 (With-Alpha $bolt 225) 8
    Draw-GlowLine $g 39 364 69 498 (With-Alpha $white 220) 7
    Draw-GlowLine $g 30 195 91 221 (With-Alpha $bolt 120) 3
    Draw-GlowLine $g 31 334 92 314 (With-Alpha $bolt 120) 3
    Save-Bitmap $canvas "Stage5_LightningStrike_A.png"
}

function Draw-Preview {
    $canvas = New-Canvas 960 520
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new((New-Color 3 10 16 255))
    $g.FillRectangle($bg, 0, 0, 960, 520)
    $bg.Dispose()
    $titleFont = [System.Drawing.Font]::new("Segoe UI", 22, [System.Drawing.FontStyle]::Bold)
    $labelFont = [System.Drawing.Font]::new("Segoe UI", 12, [System.Drawing.FontStyle]::Bold)
    $brush = [System.Drawing.SolidBrush]::new((New-Color 180 255 255 255))
    $g.DrawString("Stage 4/5 Support Asset Pack", $titleFont, $brush, 28, 22)
    $items = @(
        @{ Name = "StageThumb_Frost"; File = "StageThumb_Frost.png"; X = 35; Y = 90; W = 240; H = 135 },
        @{ Name = "StageThumb_Storm"; File = "StageThumb_Storm.png"; X = 315; Y = 90; W = 240; H = 135 },
        @{ Name = "FrostCrystal"; File = "Stage4_FrostCrystal_A.png"; X = 595; Y = 80; W = 130; H = 130 },
        @{ Name = "FrostPatch"; File = "Stage4_FrostPatch_A.png"; X = 35; Y = 285; W = 160; H = 160 },
        @{ Name = "LightningMarker"; File = "Stage5_LightningMarker_A.png"; X = 295; Y = 285; W = 160; H = 160 },
        @{ Name = "LightningStrike"; File = "Stage5_LightningStrike_A.png"; X = 600; Y = 238; W = 80; H = 250 }
    )
    foreach ($item in $items) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir $item.File))
        try {
            $g.DrawImage($img, [System.Drawing.RectangleF]::new($item.X, $item.Y, $item.W, $item.H))
        }
        finally {
            $img.Dispose()
        }
        $sf = [System.Drawing.StringFormat]::new()
        $sf.Alignment = [System.Drawing.StringAlignment]::Center
        $labelW = [Math]::Max([float]($item.W + 36), 190.0)
        $labelX = [float]$item.X + ([float]$item.W / 2) - ($labelW / 2)
        $g.DrawString($item.Name, $labelFont, $brush, [System.Drawing.RectangleF]::new($labelX, $item.Y + $item.H + 8, $labelW, 26), $sf)
        $sf.Dispose()
    }
    $brush.Dispose()
    $titleFont.Dispose()
    $labelFont.Dispose()
    $previewPath = Join-Path $ArtDir "Generated_Stage45Support_Preview.png"
    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $previewPath
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

$targets = @(
    "StageThumb_Frost.png",
    "StageThumb_Storm.png",
    "Stage4_FrostCrystal_A.png",
    "Stage4_FrostPatch_A.png",
    "Stage5_LightningMarker_A.png",
    "Stage5_LightningStrike_A.png"
)

if (-not $Force) {
    $existing = $targets | Where-Object { Test-Path -LiteralPath (Join-Path $SkinDir $_) }
    if ($existing.Count -gt 0) {
        Write-Host "Existing Stage 4/5 support PNGs found. Use -Force to overwrite:"
        $existing | ForEach-Object { Write-Host (" - " + $_) }
        return
    }
}

New-StageThumbFrost
New-StageThumbStorm
New-FrostCrystal
New-FrostPatch
New-LightningMarker
New-LightningStrike
Draw-Preview

Write-Host "Generated Stage 4/5 support assets:"
$targets | ForEach-Object { Write-Host (" - " + $_) }
Write-Host ("Preview: " + (Join-Path $ArtDir "Generated_Stage45Support_Preview.png"))
