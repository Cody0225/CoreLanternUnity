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
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    return @{ Bitmap = $bmp; Graphics = $g }
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = [System.Drawing.Pen]::new((With-Alpha $Color ([Math]::Max(8, [int]($Color.A / ($mul + 1))))), $Width * $mul)
        $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
        $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = [System.Drawing.Pen]::new($Color, $Width)
    $core.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $core.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = [System.Drawing.Pen]::new((With-Alpha $Color ([Math]::Max(6, [int]($Color.A / ($mul + 1.2))))), $Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = [System.Drawing.Pen]::new($Color, $Width)
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    foreach ($mul in 2.2, 1.55, 1.15) {
        $a = [Math]::Max(7, [int]($Color.A / ($mul * 3.2)))
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
    $pen = [System.Drawing.Pen]::new($Stroke, $StrokeWidth)
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $G.DrawPolygon($pen, $pts)
    $pen.Dispose()
}

function Save-Bitmap {
    param([System.Drawing.Bitmap]$Bitmap, [string]$Path)
    $Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
}

function New-Dasher {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $accent = New-Color 60 230 255 230
    Draw-GlowLine $g 57 214 184 62 (With-Alpha $accent 90) 4
    Draw-GlowLine $g 77 226 207 92 (With-Alpha $accent 62) 2
    Draw-Poly $g @(@(128,28),@(204,166),@(157,153),@(128,222),@(99,153),@(52,166)) (New-Color 9 58 78 238) (With-Alpha $accent 210) 4
    Draw-Poly $g @(@(128,48),@(171,145),@(140,137),@(128,182),@(116,137),@(85,145)) (New-Color 33 188 230 218) (New-Color 220 255 255 185) 2.4
    Fill-GlowEllipse $g 116 109 24 24 (New-Color 226 255 255 220)
    Draw-GlowLine $g 88 176 49 210 (With-Alpha $accent 150) 4
    Draw-GlowLine $g 168 176 207 210 (With-Alpha $accent 150) 4
    Save-Bitmap $canvas.Bitmap (Join-Path $SkinDir "Dasher.png")
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

function New-Bomber {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $accent = New-Color 255 128 28 235
    Draw-GlowEllipse $g 46 45 164 164 (With-Alpha $accent 128) 5
    Fill-GlowEllipse $g 51 53 154 154 (New-Color 82 39 12 238)
    Fill-GlowEllipse $g 67 67 122 122 (New-Color 255 126 25 235)
    Fill-GlowEllipse $g 88 86 82 82 (New-Color 255 205 65 210)
    Draw-GlowLine $g 72 84 181 191 (New-Color 36 16 8 176) 8
    Draw-GlowLine $g 181 84 72 191 (New-Color 36 16 8 176) 8
    Draw-Poly $g @(@(161,37),@(199,47),@(187,81),@(151,70)) (New-Color 26 82 38 232) (New-Color 142 255 92 190) 3
    Draw-GlowLine $g 181 42 217 24 (New-Color 255 230 83 210) 2.4
    Fill-GlowEllipse $g 210 16 15 15 (New-Color 255 235 92 220)
    Save-Bitmap $canvas.Bitmap (Join-Path $SkinDir "Bomber.png")
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

function New-Phantom {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $accent = New-Color 172 88 255 230
    Draw-GlowEllipse $g 58 42 140 164 (With-Alpha $accent 130) 4
    Fill-GlowEllipse $g 78 50 100 132 (New-Color 45 14 88 226)
    Fill-GlowEllipse $g 91 66 75 92 (New-Color 123 58 210 190)
    Draw-Poly $g @(@(93,151),@(111,224),@(128,166),@(146,224),@(165,151),@(144,181),@(128,156),@(111,181)) (New-Color 38 9 82 210) (With-Alpha $accent 155) 2.4
    Fill-GlowEllipse $g 94 100 18 14 (New-Color 220 168 255 230)
    Fill-GlowEllipse $g 144 100 18 14 (New-Color 220 168 255 230)
    Draw-GlowLine $g 59 130 23 160 (With-Alpha $accent 130) 3
    Draw-GlowLine $g 197 130 233 160 (With-Alpha $accent 130) 3
    Draw-GlowEllipse $g 70 63 116 116 (New-Color 88 245 255 72) 2
    Save-Bitmap $canvas.Bitmap (Join-Path $SkinDir "Phantom.png")
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

function Draw-Preview {
    $canvas = New-Canvas 720 280
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new((New-Color 2 10 15 255))
    $g.FillRectangle($bg, 0, 0, 720, 280)
    $bg.Dispose()
    $titleFont = [System.Drawing.Font]::new("Segoe UI", 18, [System.Drawing.FontStyle]::Bold)
    $labelFont = [System.Drawing.Font]::new("Segoe UI", 12, [System.Drawing.FontStyle]::Bold)
    $titleBrush = [System.Drawing.SolidBrush]::new((New-Color 150 255 255 255))
    $g.DrawString("Generated Enemy External Sprites", $titleFont, $titleBrush, 24, 16)
    $titleBrush.Dispose()
    $titleFont.Dispose()
    $names = @("Dasher", "Bomber", "Phantom")
    $colors = @((New-Color 60 230 255 210), (New-Color 255 148 38 210), (New-Color 172 88 255 210))
    for ($i = 0; $i -lt $names.Count; $i++) {
        $x = 46 + $i * 220
        $y = 62
        $pen = [System.Drawing.Pen]::new($colors[$i], 2)
        $g.DrawRectangle($pen, $x, $y, 180, 190)
        $pen.Dispose()
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir ($names[$i] + ".png")))
        try {
            $g.DrawImage($img, [System.Drawing.RectangleF]::new($x + 24, $y + 12, 132, 132))
        }
        finally {
            $img.Dispose()
        }
        $brush = [System.Drawing.SolidBrush]::new((New-Color 218 250 255 255))
        $sf = [System.Drawing.StringFormat]::new()
        $sf.Alignment = [System.Drawing.StringAlignment]::Center
        $g.DrawString($names[$i], $labelFont, $brush, [System.Drawing.RectangleF]::new($x, $y + 150, 180, 28), $sf)
        $sf.Dispose()
        $brush.Dispose()
    }
    $labelFont.Dispose()
    Save-Bitmap $canvas.Bitmap (Join-Path $ArtDir "Generated_EnemyExternal_Preview.png")
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

$targets = @("Dasher.png", "Bomber.png", "Phantom.png")
if (-not $Force) {
    $existing = $targets | Where-Object { Test-Path -LiteralPath (Join-Path $SkinDir $_) }
    if ($existing.Count -gt 0) {
        Write-Host "Existing enemy PNGs found. Use -Force to overwrite:"
        $existing | ForEach-Object { Write-Host (" - " + $_) }
        return
    }
}

New-Dasher
New-Bomber
New-Phantom
Draw-Preview

Write-Host "Generated enemy external sprites:"
$targets | ForEach-Object { Write-Host (" - " + $_) }
Write-Host ("Preview: " + (Join-Path $ArtDir "Generated_EnemyExternal_Preview.png"))
