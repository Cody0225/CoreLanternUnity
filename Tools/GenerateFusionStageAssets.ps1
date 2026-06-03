param(
    [switch]$Force
)

Set-StrictMode -Version Latest
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
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    return @{ Bitmap = $bmp; Graphics = $g }
}

function Load-Image {
    param([string]$Name)
    $path = Join-Path $SkinDir $Name
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing source image: $Name"
    }
    return [System.Drawing.Image]::FromFile($path)
}

function Draw-ImageAlpha {
    param($G, [System.Drawing.Image]$Image, [System.Drawing.RectangleF]$Rect, [double]$Alpha = 1.0)
    if ($Alpha -ge 0.999) {
        $G.DrawImage($Image, $Rect)
        return
    }

    $dest = [System.Drawing.Rectangle]::new(
        [int][Math]::Round($Rect.X),
        [int][Math]::Round($Rect.Y),
        [int][Math]::Round($Rect.Width),
        [int][Math]::Round($Rect.Height)
    )
    $cm = [System.Drawing.Imaging.ColorMatrix]::new()
    $cm.Matrix33 = [single]$Alpha
    $ia = [System.Drawing.Imaging.ImageAttributes]::new()
    $ia.SetColorMatrix($cm)
    $G.DrawImage($Image, $dest, 0, 0, $Image.Width, $Image.Height, [System.Drawing.GraphicsUnit]::Pixel, $ia)
    $ia.Dispose()
}

function Draw-Centered {
    param($G, [System.Drawing.Image]$Image, [float]$Scale, [double]$Alpha = 1.0, [float]$OffsetX = 0, [float]$OffsetY = 0)
    $w = 176 * $Scale
    $h = 176 * $Scale
    $rect = [System.Drawing.RectangleF]::new(128 - $w / 2 + $OffsetX, 128 - $h / 2 + $OffsetY, $w, $h)
    Draw-ImageAlpha $G $Image $rect $Alpha
}

function New-Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 2)
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    return $pen
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 4, 2.4) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(10, [int]($Color.A / ($mul + 1.4))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 4, 2.2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(9, [int]($Color.A / ($mul + 1.5))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    foreach ($mul in 2.2, 1.55) {
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

function Draw-FusionMotif {
    param($G, [int]$Fusion, [int]$Stage, [System.Drawing.Color]$Accent)
    $alpha = 85 + $Stage * 42
    $accentA = With-Alpha $Accent ([Math]::Min(245, $alpha))
    $r = 58 + $Stage * 10
    Draw-GlowEllipse $G (128 - $r) (128 - $r) ($r * 2) ($r * 2) $accentA (1.8 + $Stage * 0.5)

    if ($Fusion -eq 1) {
        Draw-GlowLine $G 68 166 188 92 $accentA (2.2 + $Stage)
        Fill-GlowEllipse $G 74 64 24 24 $accentA
        Fill-GlowEllipse $G 160 166 28 28 $accentA
    }
    elseif ($Fusion -eq 2) {
        Draw-GlowLine $G 70 128 186 128 $accentA (2.3 + $Stage)
        Draw-GlowLine $G 128 70 128 186 $accentA (1.7 + $Stage * 0.6)
        Fill-GlowEllipse $G 104 104 48 48 (With-Alpha $Accent (45 + $Stage * 18))
    }
    elseif ($Fusion -eq 3) {
        $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Accent (34 + $Stage * 18)))
        $points = @(
            [System.Drawing.PointF]::new(128, 50 - $Stage * 3),
            [System.Drawing.PointF]::new(194 + $Stage * 3, 96),
            [System.Drawing.PointF]::new(178, 188 + $Stage * 4),
            [System.Drawing.PointF]::new(128, 212 + $Stage * 3),
            [System.Drawing.PointF]::new(78, 188 + $Stage * 4),
            [System.Drawing.PointF]::new(62 - $Stage * 3, 96)
        )
        $G.FillPolygon($brush, $points)
        $brush.Dispose()
        Draw-GlowLine $G 78 188 178 188 $accentA (2.2 + $Stage * 0.4)
    }
    elseif ($Fusion -eq 4) {
        Draw-GlowLine $G 62 90 198 170 $accentA (2.4 + $Stage)
        Draw-GlowLine $G 66 170 190 80 (With-Alpha (New-Color 90 230 255) ([Math]::Min(230, $alpha))) (1.8 + $Stage * 0.7)
        Fill-GlowEllipse $G 108 108 40 40 $accentA
    }
    elseif ($Fusion -eq 5) {
        Draw-GlowEllipse $G 70 52 116 150 $accentA (2.4 + $Stage * 0.5)
        Draw-GlowLine $G 82 160 174 96 $accentA (2.2 + $Stage * 0.5)
        Draw-GlowLine $G 174 160 82 96 $accentA (2.2 + $Stage * 0.5)
    }
    else {
        Draw-GlowLine $G 68 94 188 94 $accentA (1.8 + $Stage * 0.6)
        Draw-GlowLine $G 68 162 188 162 $accentA (1.8 + $Stage * 0.6)
        Draw-GlowLine $G 84 64 172 192 $accentA (2.0 + $Stage * 0.8)
        Fill-GlowEllipse $G 94 94 68 68 (With-Alpha $Accent (35 + $Stage * 18))
    }
}

function Save-Canvas {
    param($Canvas, [string]$Path)
    $Canvas.Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function Get-FusionAccent {
    param([int]$Fusion)
    switch ($Fusion) {
        1 { return New-Color 255 78 228 }
        2 { return New-Color 112 255 86 }
        3 { return New-Color 255 210 55 }
        4 { return New-Color 62 225 255 }
        5 { return New-Color 186 116 255 }
        default { return New-Color 80 255 205 }
    }
}

function Generate-FusionStages {
    $created = New-Object System.Collections.Generic.List[string]
    for ($s = 1; $s -le 6; $s++) {
        $base = Load-Image ("Partner_S{0}_L0.png" -f $s)
        try {
            for ($f = 1; $f -le 6; $f++) {
                $l3Name = "Partner_S{0}_F{1}_L3.png" -f $s, $f
                $final = Load-Image $l3Name
                try {
                    $accent = Get-FusionAccent $f
                    for ($stage = 0; $stage -le 2; $stage++) {
                        $name = "Partner_S{0}_F{1}_L{2}.png" -f $s, $f, $stage
                        $path = Join-Path $SkinDir $name
                        if ((Test-Path -LiteralPath $path) -and -not $Force) {
                            continue
                        }

                        $canvas = New-Canvas 256 256
                        $g = $canvas.Graphics
                        $baseScale = 0.86 + $stage * 0.035
                        $finalAlpha = 0.18 + $stage * 0.20
                        $finalScale = 0.78 + $stage * 0.09

                        Draw-FusionMotif $g $f $stage (With-Alpha $accent (105 + $stage * 35))
                        Draw-Centered $g $base $baseScale 1.0 0 2
                        Draw-Centered $g $final $finalScale $finalAlpha 0 (-2 - $stage)
                        Draw-FusionMotif $g $f $stage (With-Alpha $accent (72 + $stage * 30))

                        Save-Canvas $canvas $path
                        $created.Add($name)
                    }
                }
                finally {
                    $final.Dispose()
                }
            }
        }
        finally {
            $base.Dispose()
        }
    }
    return $created
}

function Draw-Text {
    param($G, [string]$Text, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Size = 12, [System.Drawing.StringAlignment]$Align = [System.Drawing.StringAlignment]::Center, [switch]$Bold)
    $style = if ($Bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular }
    $font = [System.Drawing.Font]::new("Segoe UI", $Size, $style, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new($Color)
    $fmt = [System.Drawing.StringFormat]::new()
    $fmt.Alignment = $Align
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
    $G.DrawString($Text, $font, $brush, [System.Drawing.RectangleF]::new($X, $Y, $W, $H), $fmt)
    $fmt.Dispose()
    $brush.Dispose()
    $font.Dispose()
}

function Build-Preview {
    $speciesNames = @("Cobalt", "Ember", "Sage", "Hex", "Drift", "Iron")
    $fusionNames = @("F1 Nova", "F2 Siphon", "F3 Bastion", "F4 Phantom", "F5 Drift", "F6 Wraith")
    $cellW = 186
    $cellH = 128
    $margin = 30
    $top = 92
    $leftW = 72
    $width = $margin * 2 + $leftW + $cellW * 6
    $height = $top + $cellH * 6 + 34
    $canvas = New-Canvas $width $height
    $g = $canvas.Graphics
    $g.Clear((New-Color 2 10 16))

    Draw-Text $g "CROSS EVOLVE STAGE PROGRESSION" 0 14 $width 34 (New-Color 255 155 255) 28 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $g "Each cell shows L0 -> L1 -> L2 -> L3. Asset-only; CoreLanternGame.cs untouched." 0 50 $width 24 (New-Color 205 190 240) 13 ([System.Drawing.StringAlignment]::Center)

    for ($f = 1; $f -le 6; $f++) {
        $x = $margin + $leftW + (($f - 1) * $cellW)
        Draw-Text $g $fusionNames[$f - 1] $x 72 $cellW 18 (Get-FusionAccent $f) 12 ([System.Drawing.StringAlignment]::Center) -Bold
    }

    for ($s = 1; $s -le 6; $s++) {
        $y = $top + (($s - 1) * $cellH)
        Draw-Text $g ("S{0}`n{1}" -f $s, $speciesNames[$s - 1]) $margin ($y + 18) $leftW 72 (New-Color 180 255 255) 13 ([System.Drawing.StringAlignment]::Center) -Bold
        for ($f = 1; $f -le 6; $f++) {
            $x = $margin + $leftW + (($f - 1) * $cellW)
            $back = [System.Drawing.SolidBrush]::new((New-Color 3 18 26 235))
            $g.FillRectangle($back, $x, $y, $cellW - 8, $cellH - 10)
            $back.Dispose()
            $pen = New-Pen (Get-FusionAccent $f) 1.4
            $g.DrawRectangle($pen, $x, $y, $cellW - 8, $cellH - 10)
            $pen.Dispose()

            for ($stage = 0; $stage -le 3; $stage++) {
                $name = "Partner_S{0}_F{1}_L{2}.png" -f $s, $f, $stage
                $path = Join-Path $SkinDir $name
                $img = [System.Drawing.Image]::FromFile($path)
                try {
                    $slotX = $x + 8 + $stage * 42
                    $slotY = $y + 16
                    $rect = [System.Drawing.RectangleF]::new($slotX, $slotY, 38, 38)
                    Draw-ImageAlpha $g $img $rect 1.0
                    Draw-Text $g ("L{0}" -f $stage) ($slotX - 2) ($slotY + 44) 42 16 (New-Color 220 245 245) 10 ([System.Drawing.StringAlignment]::Center) -Bold
                }
                finally {
                    $img.Dispose()
                }
            }
            Draw-Text $g ("S{0} F{1}" -f $s, $f) $x ($y + $cellH - 30) ($cellW - 8) 16 (New-Color 210 245 245) 10 ([System.Drawing.StringAlignment]::Center)
        }
    }

    $path = Join-Path $ArtDir "Generated_FusionStage_Preview.png"
    Save-Canvas $canvas $path
    return $path
}

$created = Generate-FusionStages
$preview = Build-Preview

Write-Output ("Generated/updated fusion stage assets: {0}" -f $created.Count)
Write-Output "Preview: $preview"
