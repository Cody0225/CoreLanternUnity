param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\Logo_Drafts_20260530"
New-Item -ItemType Directory -Force -Path $SkinDir, $ArtDir | Out-Null

function Ensure-TextureMeta {
    param([string]$AssetPath)
    $metaPath = $AssetPath + ".meta"
    if ((Test-Path -LiteralPath $metaPath) -and (-not $Force)) { return }

    $templatePath = Join-Path $SkinDir "Title_CoreEmblem_v2.png.meta"
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
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
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

function Get-FontFamily {
    param([string[]]$Names)
    foreach ($name in $Names) {
        try {
            return [System.Drawing.FontFamily]::new($name)
        } catch {
        }
    }
    return [System.Drawing.FontFamily]::GenericSansSerif
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 5.5, 3.0) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(3, [int]($Color.A / ($mul + 3.5))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 5.0, 2.6) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(3, [int]($Color.A / ($mul + 3.2))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-CoreMark {
    param($G, [float]$CX, [float]$CY, [float]$Scale, [bool]$Mono = $false)
    $cyan = if ($Mono) { New-Color 238 252 255 220 } else { New-Color 139 240 255 220 }
    $gold = if ($Mono) { New-Color 238 252 255 180 } else { New-Color 255 206 59 190 }
    $ember = if ($Mono) { New-Color 238 252 255 90 } else { New-Color 200 74 31 130 }

    Draw-GlowEllipse $G ($CX - 120 * $Scale) ($CY - 120 * $Scale) (240 * $Scale) (240 * $Scale) (With-Alpha $gold 92) (1.5 * $Scale)
    Draw-GlowEllipse $G ($CX - 88 * $Scale) ($CY - 88 * $Scale) (176 * $Scale) (176 * $Scale) (With-Alpha $cyan 132) (1.5 * $Scale)
    Draw-GlowEllipse $G ($CX - 50 * $Scale) ($CY - 50 * $Scale) (100 * $Scale) (100 * $Scale) (With-Alpha $gold 170) (2.0 * $Scale)
    Draw-GlowLine $G ($CX - 148 * $Scale) $CY ($CX + 148 * $Scale) $CY (With-Alpha $cyan 70) (1.2 * $Scale)
    Draw-GlowLine $G $CX ($CY - 148 * $Scale) $CX ($CY + 148 * $Scale) (With-Alpha $gold 70) (1.2 * $Scale)

    $points = @(
        [System.Drawing.PointF]::new($CX, $CY - 74 * $Scale),
        [System.Drawing.PointF]::new($CX + 48 * $Scale, $CY - 20 * $Scale),
        [System.Drawing.PointF]::new($CX + 28 * $Scale, $CY + 70 * $Scale),
        [System.Drawing.PointF]::new($CX, $CY + 96 * $Scale),
        [System.Drawing.PointF]::new($CX - 28 * $Scale, $CY + 70 * $Scale),
        [System.Drawing.PointF]::new($CX - 48 * $Scale, $CY - 20 * $Scale)
    )
    $rect = [System.Drawing.RectangleF]::new($CX - 52 * $Scale, $CY - 74 * $Scale, 104 * $Scale, 170 * $Scale)
    $fill = [System.Drawing.Drawing2D.LinearGradientBrush]::new($rect, $gold, $cyan, [System.Drawing.Drawing2D.LinearGradientMode]::Vertical)
    $G.FillPolygon($fill, $points)
    $fill.Dispose()
    $pen = New-Pen (With-Alpha $cyan 220) (2.4 * $Scale)
    $G.DrawPolygon($pen, $points)
    $pen.Dispose()

    for ($i = 0; $i -lt 18; $i++) {
        $a = ($i * [Math]::PI * 2.0) / 18.0
        $r1 = 110 * $Scale
        $r2 = if ($i % 3 -eq 0) { 140 * $Scale } else { 128 * $Scale }
        $x1 = $CX + [Math]::Cos($a) * $r1
        $y1 = $CY + [Math]::Sin($a) * $r1
        $x2 = $CX + [Math]::Cos($a) * $r2
        $y2 = $CY + [Math]::Sin($a) * $r2
        $col = if ($i % 4 -eq 0) { $ember } else { With-Alpha $gold 88 }
        Draw-GlowLine $G $x1 $y1 $x2 $y2 $col (1.0 * $Scale)
    }
}

function Add-LogoTextPath {
    param([string]$Text, [System.Drawing.FontFamily]$Family, [float]$Size)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $fmt = [System.Drawing.StringFormat]::new()
    $fmt.Alignment = [System.Drawing.StringAlignment]::Center
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
    $style = [System.Drawing.FontStyle]::Bold
    $path.AddString($Text, $Family, [int]$style, $Size, [System.Drawing.PointF]::new(0, 0), $fmt)
    $fmt.Dispose()
    return $path
}

function Draw-LogoText {
    param(
        $G,
        [string]$Text,
        [float]$Y,
        [float]$TargetWidth,
        [float]$Size,
        [System.Drawing.FontFamily]$Family,
        [System.Drawing.Color]$FillTop,
        [System.Drawing.Color]$FillBottom,
        [System.Drawing.Color]$Outline,
        [System.Drawing.Color]$Glow
    )

    $path = Add-LogoTextPath $Text $Family $Size
    $bounds = $path.GetBounds()
    $scale = [Math]::Min(1.0, $TargetWidth / [Math]::Max(1.0, $bounds.Width))
    $matrix = [System.Drawing.Drawing2D.Matrix]::new()
    $matrix.Scale($scale, $scale)
    $path.Transform($matrix)
    $matrix.Dispose()
    $bounds = $path.GetBounds()
    $matrix = [System.Drawing.Drawing2D.Matrix]::new()
    $matrix.Translate(512 - ($bounds.Left + $bounds.Width / 2), $Y - ($bounds.Top + $bounds.Height / 2))
    $path.Transform($matrix)
    $matrix.Dispose()
    $bounds = $path.GetBounds()

    foreach ($w in 18, 9) {
        $pen = New-Pen (With-Alpha $Glow ([Math]::Max(10, [int](90 / ($w / 6))))) $w
        $G.DrawPath($pen, $path)
        $pen.Dispose()
    }
    $outlinePen = New-Pen $Outline 5.5
    $G.DrawPath($outlinePen, $path)
    $outlinePen.Dispose()
    $shadowPen = New-Pen (New-Color 3 8 12 210) 2.2
    $G.DrawPath($shadowPen, $path)
    $shadowPen.Dispose()

    $brush = [System.Drawing.Drawing2D.LinearGradientBrush]::new($bounds, $FillTop, $FillBottom, [System.Drawing.Drawing2D.LinearGradientMode]::Vertical)
    $G.FillPath($brush, $path)
    $brush.Dispose()
    $path.Dispose()
}

function Save-Canvas {
    param($Canvas, [string]$Name, [bool]$Resource = $true)
    if ($Resource) {
        $path = Join-Path $SkinDir $Name
        if ((Test-Path -LiteralPath $path) -and (-not $Force)) {
            Write-Host "Skip existing $Name"
        } else {
            $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
            Ensure-TextureMeta $path
            Write-Host "Generated $Name"
        }
    }

    $artPath = Join-Path $ArtDir $Name
    $Canvas.Bitmap.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $artPath
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function New-GameLogo {
    param([bool]$Mono = $false)
    $canvas = New-Canvas 1024 512
    $g = $canvas.Graphics
    $family = Get-FontFamily @("Bahnschrift", "Segoe UI Semibold", "Arial")
    $cyan = if ($Mono) { New-Color 248 252 255 245 } else { New-Color 139 240 255 245 }
    $cyanDeep = if ($Mono) { New-Color 196 210 220 235 } else { New-Color 47 178 220 235 }
    $gold = if ($Mono) { New-Color 248 252 255 210 } else { New-Color 255 206 59 215 }
    $ember = if ($Mono) { New-Color 248 252 255 120 } else { New-Color 200 74 31 150 }
    $dark = New-Color 4 10 14 210

    Draw-CoreMark $g 512 242 0.74 $Mono
    Draw-GlowLine $g 172 118 852 118 (With-Alpha $cyan 120) 2.4
    Draw-GlowLine $g 236 390 788 390 (With-Alpha $gold 118) 2.2
    Draw-GlowLine $g 132 254 214 254 (With-Alpha $ember 104) 2.0
    Draw-GlowLine $g 810 254 892 254 (With-Alpha $ember 104) 2.0

    Draw-LogoText $g "EGGCORE" 213 810 126 $family (New-Color 230 255 255 250) $cyanDeep (With-Alpha $dark 230) (With-Alpha $cyan 160)
    Draw-LogoText $g "PROTOCOL" 318 680 72 $family $gold (With-Alpha $cyan 230) (With-Alpha $dark 235) (With-Alpha $gold 115)

    $dotBrush = [System.Drawing.SolidBrush]::new((With-Alpha $gold 170))
    foreach ($x in 190, 230, 794, 834) {
        $g.FillRectangle($dotBrush, $x, 412, 8, 8)
    }
    $dotBrush.Dispose()

    $fileName = if ($Mono) { "GameLogo_Mono.png" } else { "GameLogo.png" }
    Save-Canvas $canvas $fileName $true
}

function New-GameMark {
    param([bool]$Mono = $false)
    $canvas = New-Canvas 512 512
    $g = $canvas.Graphics
    Draw-CoreMark $g 256 256 1.0 $Mono
    $fileName = if ($Mono) { "GameLogo_Mark_Mono.png" } else { "GameLogo_Mark.png" }
    Save-Canvas $canvas $fileName $true
}

function New-LogoPreview {
    $canvas = New-Canvas 1400 860
    $g = $canvas.Graphics
    $bg = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
        [System.Drawing.Rectangle]::new(0, 0, 1400, 860),
        (New-Color 5 12 16 255),
        (New-Color 8 26 32 255),
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $g.FillRectangle($bg, 0, 0, 1400, 860)
    $bg.Dispose()
    $gridPen = New-Pen (New-Color 28 120 140 40) 1
    for ($x = 0; $x -le 1400; $x += 100) { $g.DrawLine($gridPen, $x, 0, $x, 860) }
    for ($y = 0; $y -le 860; $y += 100) { $g.DrawLine($gridPen, 0, $y, 1400, $y) }
    $gridPen.Dispose()

    $logo = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "GameLogo.png"))
    $mono = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "GameLogo_Mono.png"))
    $mark = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "GameLogo_Mark.png"))
    $markMono = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "GameLogo_Mark_Mono.png"))
    $g.DrawImage($logo, 188, 58, 1024, 512)
    $g.DrawImage($mono, 84, 604, 512, 256)
    $g.DrawImage($mark, 770, 610, 180, 180)
    $g.DrawImage($markMono, 1010, 610, 180, 180)
    $logo.Dispose()
    $mono.Dispose()
    $mark.Dispose()
    $markMono.Dispose()

    $family = Get-FontFamily @("Bahnschrift", "Segoe UI Semibold", "Arial")
    $font = [System.Drawing.Font]::new($family, 22, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new($family, 15, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new((New-Color 184 245 255 230))
    $muted = [System.Drawing.SolidBrush]::new((New-Color 255 206 59 205))
    $g.DrawString("GameLogo.png / GameLogo_Mono.png / mark variants", $font, $brush, 72, 22)
    $g.DrawString("Deterministic text logo draft. No AI text rendering; transparent PNG resources.", $small, $muted, 72, 50)
    $font.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()

    Save-Canvas $canvas "GameLogo_Preview.png" $false
}

New-GameLogo $false
New-GameLogo $true
New-GameMark $false
New-GameMark $true
New-LogoPreview

Write-Host ""
Write-Host "Logo assets complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
