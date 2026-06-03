param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\Title_UI_V2_20260530"
New-Item -ItemType Directory -Force -Path $SkinDir, $ArtDir | Out-Null

function Ensure-TextureMeta {
    param([string]$AssetPath)
    $metaPath = $AssetPath + ".meta"
    if ((Test-Path -LiteralPath $metaPath) -and (-not $Force)) { return }

    $templatePath = Join-Path $SkinDir "HUD_Bar_Back_v2.png.meta"
    if (-not (Test-Path -LiteralPath $templatePath)) {
        $templatePath = Join-Path $SkinDir "Runner.png.meta"
    }

    if (Test-Path -LiteralPath $templatePath) {
        $text = [System.IO.File]::ReadAllText($templatePath, [System.Text.UTF8Encoding]::new($false))
        $guid = [guid]::NewGuid().ToString("N")
        $text = [regex]::Replace($text, "guid: [0-9a-fA-F]+", "guid: $guid", 1)
        [System.IO.File]::WriteAllText($metaPath, $text, [System.Text.UTF8Encoding]::new($false))
    }
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

function New-RoundedRectPath {
    param([float]$X, [float]$Y, [float]$W, [float]$H, [float]$R)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $d = $R * 2
    $path.AddArc($X, $Y, $d, $d, 180, 90)
    $path.AddArc($X + $W - $d, $Y, $d, $d, 270, 90)
    $path.AddArc($X + $W - $d, $Y + $H - $d, $d, $d, 0, 90)
    $path.AddArc($X, $Y + $H - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4, 2.2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(3, [int]($Color.A / ($mul + 2.8))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 5, 2.6) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(3, [int]($Color.A / ($mul + 3.4))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Save-Canvas {
    param($Canvas, [string]$Name)
    $path = Join-Path $SkinDir $Name
    if ((Test-Path -LiteralPath $path) -and (-not $Force)) {
        Write-Host "Skip existing $Name"
    } else {
        $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
        Ensure-TextureMeta $path
        Write-Host "Generated $Name"
    }

    $artPath = Join-Path $ArtDir $Name
    $Canvas.Bitmap.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $artPath
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function New-CoreEmblem {
    $canvas = New-Canvas 512 512
    $g = $canvas.Graphics
    $cyan = New-Color 139 240 255 190
    $gold = New-Color 255 206 59 170
    $ember = New-Color 200 74 31 140

    for ($i = 0; $i -lt 5; $i++) {
        $size = 378 - ($i * 46)
        $x = (512 - $size) / 2
        $alpha = 90 - ($i * 10)
        $col = if ($i % 2 -eq 0) { With-Alpha $gold $alpha } else { With-Alpha $cyan $alpha }
        Draw-GlowEllipse $g $x $x $size $size $col 1.6
    }

    Draw-GlowLine $g 256 42 256 470 (With-Alpha $cyan 60) 1.1
    Draw-GlowLine $g 42 256 470 256 (With-Alpha $gold 56) 1.1

    for ($i = 0; $i -lt 24; $i++) {
        $a = ($i * [Math]::PI * 2.0) / 24.0
        $r1 = 174
        $r2 = if ($i % 3 -eq 0) { 205 } else { 190 }
        $x1 = 256 + [Math]::Cos($a) * $r1
        $y1 = 256 + [Math]::Sin($a) * $r1
        $x2 = 256 + [Math]::Cos($a) * $r2
        $y2 = 256 + [Math]::Sin($a) * $r2
        $col = if ($i % 4 -eq 0) { With-Alpha $ember 95 } else { With-Alpha $gold 68 }
        Draw-GlowLine $g $x1 $y1 $x2 $y2 $col 1.2
    }

    $corePath = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $corePath.AddEllipse(198, 198, 116, 116)
    $glow = [System.Drawing.Drawing2D.PathGradientBrush]::new($corePath)
    $glow.CenterColor = (New-Color 255 244 158 185)
    $glow.SurroundColors = @([System.Drawing.Color]::Transparent)
    $g.FillEllipse($glow, 198, 198, 116, 116)
    $glow.Dispose()
    $corePath.Dispose()

    $gem = @(
        [System.Drawing.PointF]::new(256, 164),
        [System.Drawing.PointF]::new(316, 220),
        [System.Drawing.PointF]::new(292, 322),
        [System.Drawing.PointF]::new(256, 356),
        [System.Drawing.PointF]::new(220, 322),
        [System.Drawing.PointF]::new(196, 220)
    )
    $gemFill = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
        [System.Drawing.Rectangle]::new(196, 164, 120, 192),
        (New-Color 255 231 104 216),
        (New-Color 76 224 255 182),
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $g.FillPolygon($gemFill, $gem)
    $gemFill.Dispose()
    $gemPen = New-Pen (New-Color 255 250 178 230) 2.2
    $g.DrawPolygon($gemPen, $gem)
    $gemPen.Dispose()

    Draw-GlowEllipse $g 176 176 160 160 (With-Alpha $cyan 165) 2.2
    Draw-GlowEllipse $g 212 212 88 88 (With-Alpha $gold 185) 2.0

    for ($i = 0; $i -lt 14; $i++) {
        $a = ($i * [Math]::PI * 2.0) / 14.0
        $r = 223
        $x = 256 + [Math]::Cos($a) * $r
        $y = 256 + [Math]::Sin($a) * $r
        $sparkColor = if ($i % 2 -eq 0) { With-Alpha $cyan 80 } else { With-Alpha $gold 86 }
        $b = [System.Drawing.SolidBrush]::new($sparkColor)
        $g.FillRectangle($b, [float]($x - 3), [float]($y - 3), 6, 6)
        $b.Dispose()
    }

    Save-Canvas $canvas "Title_CoreEmblem_v2.png"
}

function New-Plate {
    param([string]$Name, [int]$W, [int]$H, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Secondary)
    $canvas = New-Canvas $W $H
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 1 ($W - 2) ($H - 2) 6
    $fill = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(132, 4, 14, 20))
    $g.FillPath($fill, $path)
    $fill.Dispose()
    $wash = [System.Drawing.SolidBrush]::new((With-Alpha $Accent 20))
    $g.FillPath($wash, $path)
    $wash.Dispose()
    $pen = New-Pen (With-Alpha $Accent 86) 1.2
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
    Draw-GlowLine $g 18 9 ([Math]::Min($W - 18, 230)) 9 (With-Alpha $Accent 160) 1.6
    Draw-GlowLine $g ($W - 18) ($H - 10) ([Math]::Max(18, $W - 230)) ($H - 10) (With-Alpha $Secondary 96) 1.3
    Save-Canvas $canvas $Name
}

function New-Rail {
    param([string]$Name, [int]$W, [int]$H, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Secondary)
    $canvas = New-Canvas $W $H
    $g = $canvas.Graphics
    $y = [Math]::Floor($H / 2)
    Draw-GlowLine $g 26 $y ($W - 26) $y (With-Alpha $Accent 82) 2.2
    Draw-GlowLine $g 82 ($y - 5) ([Math]::Min($W - 82, 360)) ($y - 5) (With-Alpha $Secondary 108) 1.4
    Draw-GlowLine $g ($W - 82) ($y + 5) ([Math]::Max(82, $W - 360)) ($y + 5) (With-Alpha $Accent 82) 1.3
    Save-Canvas $canvas $Name
}

function New-CornerAccent {
    param([string]$Name, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Secondary)
    $canvas = New-Canvas 160 160
    $g = $canvas.Graphics
    Draw-GlowLine $g 12 12 108 12 (With-Alpha $Accent 150) 1.7
    Draw-GlowLine $g 12 12 12 108 (With-Alpha $Accent 132) 1.7
    Draw-GlowLine $g 30 28 96 28 (With-Alpha $Accent 82) 1.2
    Draw-GlowLine $g 28 30 28 96 (With-Alpha $Accent 72) 1.2
    Draw-GlowLine $g 54 134 144 134 (With-Alpha $Secondary 105) 1.4
    Draw-GlowLine $g 134 54 134 144 (With-Alpha $Accent 72) 1.2
    Save-Canvas $canvas $Name
}

function New-Preview {
    $previewPath = Join-Path $ArtDir "Title_UI_V2_Preview.png"
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 7, 15, 18))
    $g.FillRectangle($bg, 0, 0, 1280, 720)
    $bg.Dispose()
    $gridPen = New-Pen (New-Color 24 96 112 70) 1
    for ($x = 0; $x -lt 1280; $x += 96) { $g.DrawLine($gridPen, $x, 0, $x, 720) }
    for ($y = 0; $y -lt 720; $y += 96) { $g.DrawLine($gridPen, 0, $y, 1280, $y) }
    $gridPen.Dispose()

    $emblem = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Title_CoreEmblem_v2.png"))
    $g.DrawImage($emblem, 384, 104, 512, 512)
    $emblem.Dispose()

    $items = @(
        @{File="Title_LogoUnderline_v2.png"; X=330; Y=108},
        @{File="Title_SubtitlePlate_v2.png"; X=350; Y=194},
        @{File="Title_StatsRibbon_v2.png"; X=230; Y=286},
        @{File="Title_NavRail_v2.png"; X=160; Y=560},
        @{File="Title_CornerAccent_v2.png"; X=54; Y=56},
        @{File="Title_CornerAccent_v2.png"; X=1066; Y=56},
        @{File="Title_CornerAccent_v2.png"; X=54; Y=504},
        @{File="Title_CornerAccent_v2.png"; X=1066; Y=504}
    )
    foreach ($item in $items) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir $item.File))
        $g.DrawImage($img, $item.X, $item.Y, $img.Width, $img.Height)
        $img.Dispose()
    }

    $font = [System.Drawing.Font]::new("Arial", 21, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 13, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(235, 184, 224, 255))
    $muted = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(170, 139, 200, 255))
    $g.DrawString("Title UI V2 non-text asset preview", $font, $brush, 34, 638)
    $g.DrawString("Decorative pieces only. Keep title/buttons as live Unity text.", $small, $muted, 34, 666)
    $font.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()

    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
    Write-Host "Generated Title_UI_V2_Preview.png"
}

$cyan = New-Color 139 240 255 195
$gold = New-Color 255 206 59 150
$ember = New-Color 200 74 31 128
$violet = New-Color 255 84 243 120

New-CoreEmblem
New-Plate "Title_SubtitlePlate_v2.png" 580 38 $cyan $gold
New-Plate "Title_StatsRibbon_v2.png" 820 54 $gold $cyan
New-Rail "Title_LogoUnderline_v2.png" 620 22 $cyan $gold
New-Rail "Title_NavRail_v2.png" 960 24 $cyan $ember
New-CornerAccent "Title_CornerAccent_v2.png" $cyan $violet

New-Preview

Write-Host ""
Write-Host "Title UI v2 assets complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
