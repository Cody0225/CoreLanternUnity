param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$skinsDir = Join-Path $ProjectRoot "Assets/Resources/Skins"
$outDir = Join-Path $ProjectRoot "marketing/ItchIo"
$artDir = Join-Path $ProjectRoot "Assets/ArtSource/ItchIo_20260531"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
New-Item -ItemType Directory -Force -Path $artDir | Out-Null

function New-Bitmap([int]$w, [int]$h) {
    $bmp = New-Object System.Drawing.Bitmap $w, $h, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    return $bmp
}

function New-Rect([float]$x, [float]$y, [float]$w, [float]$h) {
    return New-Object System.Drawing.RectangleF $x, $y, $w, $h
}

function Color-A([int]$a, [int]$r, [int]$g, [int]$b) {
    return [System.Drawing.Color]::FromArgb($a, $r, $g, $b)
}

function Draw-ImageFit($g, [System.Drawing.Image]$img, [System.Drawing.RectangleF]$box) {
    $scale = [Math]::Min($box.Width / $img.Width, $box.Height / $img.Height)
    $w = $img.Width * $scale
    $h = $img.Height * $scale
    $x = $box.X + ($box.Width - $w) / 2
    $y = $box.Y + ($box.Height - $h) / 2
    $g.DrawImage($img, (New-Rect $x $y $w $h))
}

function Draw-SoftBackground($g, [int]$w, [int]$h, [bool]$warm) {
    $top = if ($warm) { Color-A 255 15 20 22 } else { Color-A 255 6 18 23 }
    $bottom = if ($warm) { Color-A 255 37 24 15 } else { Color-A 255 9 12 18 }
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush (New-Rect 0 0 $w $h), $top, $bottom, 90
    $g.FillRectangle($brush, 0, 0, $w, $h)
    $brush.Dispose()

    $gridPen = New-Object System.Drawing.Pen (Color-A 34 90 220 235), 1
    for ($x = 0; $x -lt $w; $x += [Math]::Max(42, [int]($w / 12))) {
        $g.DrawLine($gridPen, $x, 0, $x, $h)
    }
    for ($y = 0; $y -lt $h; $y += [Math]::Max(42, [int]($h / 8))) {
        $g.DrawLine($gridPen, 0, $y, $w, $y)
    }
    $gridPen.Dispose()

    $edgePenCyan = New-Object System.Drawing.Pen (Color-A 115 91 232 245), 2
    $edgePenGold = New-Object System.Drawing.Pen (Color-A 115 255 205 55), 2
    $g.DrawLine($edgePenCyan, 18, 22, [int]($w * 0.36), 22)
    $g.DrawLine($edgePenGold, [int]($w * 0.62), $h - 30, $w - 24, $h - 30)
    $g.DrawLine($edgePenCyan, 24, $h - 44, [int]($w * 0.24), $h - 44)
    $g.DrawLine($edgePenGold, [int]($w * 0.70), 38, $w - 30, 38)
    $edgePenCyan.Dispose()
    $edgePenGold.Dispose()

    $rand = New-Object System.Random 31
    for ($i = 0; $i -lt 38; $i++) {
        $px = $rand.Next(0, $w)
        $py = $rand.Next(0, $h)
        $s = $rand.Next(2, 6)
        $c = if (($i % 3) -eq 0) { Color-A 80 255 205 55 } else { Color-A 66 88 232 245 }
        $b = New-Object System.Drawing.SolidBrush $c
        $g.FillRectangle($b, $px, $py, $s, $s)
        $b.Dispose()
    }
}

function Draw-CoreHalo($g, [float]$cx, [float]$cy, [float]$r) {
    for ($i = 0; $i -lt 5; $i++) {
        $alpha = 72 - ($i * 10)
        $pen = New-Object System.Drawing.Pen (Color-A $alpha 92 235 246), (2 + $i)
        $rr = $r + ($i * 18)
        $g.DrawEllipse($pen, $cx - $rr, $cy - $rr, $rr * 2, $rr * 2)
        $pen.Dispose()
    }

    $gold = New-Object System.Drawing.Pen (Color-A 185 255 205 55), 3
    $g.DrawEllipse($gold, $cx - $r * 0.72, $cy - $r * 0.72, $r * 1.44, $r * 1.44)
    $gold.Dispose()

    $coreBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush (New-Rect ($cx - $r*0.48) ($cy - $r*0.50) ($r*0.96) ($r*0.96)), (Color-A 255 255 246 138), (Color-A 255 255 150 28), 90
    $g.FillEllipse($coreBrush, $cx - $r*0.48, $cy - $r*0.50, $r*0.96, $r*0.96)
    $coreBrush.Dispose()

    $inner = New-Object System.Drawing.SolidBrush (Color-A 235 120 255 245)
    $g.FillEllipse($inner, $cx - $r*0.19, $cy - $r*0.20, $r*0.38, $r*0.38)
    $inner.Dispose()
}

function Draw-Tag($g, [string]$text, [System.Drawing.RectangleF]$rect) {
    $bg = New-Object System.Drawing.SolidBrush (Color-A 190 8 18 24)
    $line = New-Object System.Drawing.Pen (Color-A 190 255 205 55), 1.5
    $g.FillRectangle($bg, $rect)
    $g.DrawRectangle($line, $rect.X, $rect.Y, $rect.Width, $rect.Height)
    $font = New-Object System.Drawing.Font "Arial", 13, ([System.Drawing.FontStyle]::Bold), ([System.Drawing.GraphicsUnit]::Pixel)
    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = [System.Drawing.StringAlignment]::Center
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
    $txt = New-Object System.Drawing.SolidBrush (Color-A 255 255 226 94)
    $g.DrawString($text, $font, $txt, $rect, $fmt)
    $txt.Dispose(); $fmt.Dispose(); $font.Dispose(); $line.Dispose(); $bg.Dispose()
}

function Draw-CenteredLine($g, [string]$text, [float]$cx, [float]$y, [float]$maxW, [float]$size, [System.Drawing.Color]$color) {
    $fontSize = $size
    $font = $null
    do {
        if ($font -ne $null) { $font.Dispose() }
        $font = New-Object System.Drawing.Font "Arial", $fontSize, ([System.Drawing.FontStyle]::Regular), ([System.Drawing.GraphicsUnit]::Pixel)
        $measured = $g.MeasureString($text, $font)
        if ($measured.Width -le $maxW -or $fontSize -le 8) { break }
        $fontSize -= 1
    } while ($true)

    $fmt = New-Object System.Drawing.StringFormat
    $fmt.Alignment = [System.Drawing.StringAlignment]::Center
    $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
    $brush = New-Object System.Drawing.SolidBrush $color
    $g.DrawString($text, $font, $brush, (New-Rect ($cx - $maxW / 2) ($y - $fontSize * 0.7) $maxW ($fontSize * 1.4)), $fmt)
    $brush.Dispose(); $fmt.Dispose(); $font.Dispose()
}

function Save-Png([System.Drawing.Bitmap]$bmp, [string]$path) {
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
}

function New-ItchAsset([int]$w, [int]$h, [string]$path, [bool]$compact) {
    $logoPath = Join-Path $skinsDir "GameLogo_TitleCompact.png"
    if (-not (Test-Path $logoPath)) { $logoPath = Join-Path $skinsDir "GameLogo.png" }
    $markPath = Join-Path $skinsDir "GameLogo_Mark.png"

    $logo = [System.Drawing.Image]::FromFile($logoPath)
    $mark = if (Test-Path $markPath) { [System.Drawing.Image]::FromFile($markPath) } else { $null }
    $bmp = New-Bitmap $w $h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit

    Draw-SoftBackground $g $w $h $false
    Draw-CoreHalo $g ($w * 0.50) ($h * 0.53) ([Math]::Min($w, $h) * 0.18)

    if ($mark -ne $null) {
        Draw-ImageFit $g $mark (New-Rect ($w*0.39) ($h*0.35) ($w*0.22) ($h*0.22))
        $mark.Dispose()
    }

    if ($compact) {
        Draw-ImageFit $g $logo (New-Rect ($w*0.08) ($h*0.08) ($w*0.84) ($h*0.25))
        Draw-CenteredLine $g "EVOLVE YOUR PARTNER" ($w*0.50) ($h*0.74) ($w*0.78) ([Math]::Max(10, [int]($h * 0.052))) (Color-A 235 210 252 255)
        Draw-CenteredLine $g "DEFEND THE DATA EGG" ($w*0.50) ($h*0.80) ($w*0.78) ([Math]::Max(10, [int]($h * 0.052))) (Color-A 235 210 252 255)
        Draw-Tag $g "DEMO BUILD" (New-Rect ($w*0.34) ($h*0.86) ($w*0.32) ($h*0.07))
    } else {
        Draw-ImageFit $g $logo (New-Rect ($w*0.07) ($h*0.08) ($w*0.86) ($h*0.24))
        Draw-CenteredLine $g "EVOLVE YOUR PARTNER" ($w*0.50) ($h*0.72) ($w*0.82) ([Math]::Max(13, [int]($h * 0.043))) (Color-A 235 215 255 255)
        Draw-CenteredLine $g "DEFEND THE DATA EGG" ($w*0.50) ($h*0.775) ($w*0.82) ([Math]::Max(13, [int]($h * 0.043))) (Color-A 235 215 255 255)
        Draw-Tag $g "FREE DEMO" (New-Rect ($w*0.40) ($h*0.84) ($w*0.20) ($h*0.07))
    }

    Save-Png $bmp $path
    $g.Dispose()
    $bmp.Dispose()
    $logo.Dispose()
}

$header = Join-Path $outDir "ItchIo_Header_630x500.png"
$cover = Join-Path $outDir "ItchIo_Cover_315x250.png"
$social = Join-Path $outDir "ItchIo_SocialCard_1200x630.png"
New-ItchAsset 630 500 $header $false
New-ItchAsset 315 250 $cover $true
New-ItchAsset 1200 630 $social $false

Copy-Item -LiteralPath $header -Destination (Join-Path $artDir "ItchIo_Header_630x500.png") -Force
Copy-Item -LiteralPath $cover -Destination (Join-Path $artDir "ItchIo_Cover_315x250.png") -Force
Copy-Item -LiteralPath $social -Destination (Join-Path $artDir "ItchIo_SocialCard_1200x630.png") -Force

$preview = New-Bitmap 1280 720
$pg = [System.Drawing.Graphics]::FromImage($preview)
$pg.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$pg.Clear([System.Drawing.Color]::FromArgb(255, 8, 14, 18))
$hImg = [System.Drawing.Image]::FromFile($header)
$cImg = [System.Drawing.Image]::FromFile($cover)
$sImg = [System.Drawing.Image]::FromFile($social)
Draw-ImageFit $pg $hImg (New-Rect 52 70 504 400)
Draw-ImageFit $pg $cImg (New-Rect 606 70 315 250)
Draw-ImageFit $pg $sImg (New-Rect 606 370 600 315)
$labelFont = New-Object System.Drawing.Font "Arial", 20, ([System.Drawing.FontStyle]::Bold), ([System.Drawing.GraphicsUnit]::Pixel)
$labelBrush = New-Object System.Drawing.SolidBrush (Color-A 255 164 255 255)
$pg.DrawString("itch.io page asset preview", $labelFont, $labelBrush, 52, 24)
$pg.DrawString("header 630x500", $labelFont, $labelBrush, 52, 486)
$pg.DrawString("cover 315x250", $labelFont, $labelBrush, 606, 332)
$pg.DrawString("social card 1200x630", $labelFont, $labelBrush, 606, 690)
$labelBrush.Dispose(); $labelFont.Dispose()
$hImg.Dispose(); $cImg.Dispose(); $sImg.Dispose()
Save-Png $preview (Join-Path $artDir "ItchIo_PageAssets_Preview.png")
$pg.Dispose()
$preview.Dispose()

Write-Host "Generated itch.io page assets:"
Write-Host "  $header"
Write-Host "  $cover"
Write-Host "  $social"
Write-Host "  Preview: $(Join-Path $artDir 'ItchIo_PageAssets_Preview.png')"
