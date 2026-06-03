param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$IconDir = Join-Path $ProjectRoot "build\Icons"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\Icon_Drafts_20260531"
New-Item -ItemType Directory -Force -Path $IconDir, $ArtDir | Out-Null

function New-Color {
    param([int]$R, [int]$G, [int]$B, [int]$A = 255)
    return [System.Drawing.Color]::FromArgb($A, $R, $G, $B)
}

function New-Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 2)
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    return $pen
}

function Draw-IconBackground {
    param($G, [int]$Size)
    $rect = [System.Drawing.Rectangle]::new(0, 0, $Size, $Size)
    $bg = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
        $rect,
        (New-Color 4 13 18 255),
        (New-Color 11 32 38 255),
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $G.FillEllipse($bg, 1, 1, $Size - 2, $Size - 2)
    $bg.Dispose()
}

function Draw-IconRings {
    param($G, [int]$Size)
    $cyan = New-Color 139 240 255 180
    $gold = New-Color 255 206 59 160
    $cx = $Size / 2.0
    $cy = $Size / 2.0
    foreach ($entry in @(@(0.78, $gold, 1.6), @(0.58, $cyan, 1.4), @(0.38, $gold, 1.2))) {
        $diam = $Size * $entry[0]
        $x = $cx - $diam / 2.0
        $y = $cy - $diam / 2.0
        $pen = New-Pen $entry[1] ([Math]::Max(1.0, $Size / 128.0 * $entry[2]))
        $G.DrawEllipse($pen, [float]$x, [float]$y, [float]$diam, [float]$diam)
        $pen.Dispose()
    }
}

function Draw-SmallGem {
    param($G, [int]$Size)
    $cx = $Size / 2.0
    $cy = $Size / 2.0
    $scale = $Size / 256.0
    $points = @(
        [System.Drawing.PointF]::new($cx, $cy - 76 * $scale),
        [System.Drawing.PointF]::new($cx + 50 * $scale, $cy - 22 * $scale),
        [System.Drawing.PointF]::new($cx + 30 * $scale, $cy + 66 * $scale),
        [System.Drawing.PointF]::new($cx, $cy + 94 * $scale),
        [System.Drawing.PointF]::new($cx - 30 * $scale, $cy + 66 * $scale),
        [System.Drawing.PointF]::new($cx - 50 * $scale, $cy - 22 * $scale)
    )
    $bounds = [System.Drawing.RectangleF]::new($cx - 52 * $scale, $cy - 76 * $scale, 104 * $scale, 170 * $scale)
    $fill = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
        $bounds,
        (New-Color 255 226 88 245),
        (New-Color 78 221 255 235),
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $G.FillPolygon($fill, $points)
    $fill.Dispose()
    $outline = New-Pen (New-Color 232 255 255 230) ([Math]::Max(1.0, $Size / 120.0))
    $G.DrawPolygon($outline, $points)
    $outline.Dispose()
}

function New-IconBitmap {
    param([int]$Size)
    $bmp = [System.Drawing.Bitmap]::new($Size, $Size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $bmp.SetResolution(96, 96)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    Draw-IconBackground $g $Size
    if ($Size -ge 32) {
        Draw-IconRings $g $Size
    }
    Draw-SmallGem $g $Size

    if ($Size -ge 64) {
        $pen = New-Pen (New-Color 200 74 31 128) ([Math]::Max(1.0, $Size / 128.0))
        $g.DrawLine($pen, [float]($Size * 0.16), [float]($Size * 0.50), [float]($Size * 0.30), [float]($Size * 0.50))
        $g.DrawLine($pen, [float]($Size * 0.70), [float]($Size * 0.50), [float]($Size * 0.84), [float]($Size * 0.50))
        $pen.Dispose()
    }

    $g.Dispose()
    return $bmp
}

function Save-IconPngs {
    param([int[]]$Sizes)
    foreach ($size in $Sizes) {
        $bmp = New-IconBitmap $size
        $name = "icon_$size.png"
        $path = Join-Path $IconDir $name
        $artPath = Join-Path $ArtDir $name
        if ((Test-Path -LiteralPath $path) -and (-not $Force)) {
            Write-Host "Skip existing $name"
        } else {
            $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
            Write-Host "Generated $name"
        }
        $bmp.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    }
}

function New-IcoFile {
    param([int[]]$Sizes, [string]$OutputPath)
    $entries = @()
    foreach ($size in $Sizes) {
        $pngPath = Join-Path $IconDir "icon_$size.png"
        $bytes = [System.IO.File]::ReadAllBytes($pngPath)
        $entries += [pscustomobject]@{ Size = $size; Bytes = $bytes }
    }

    $fs = [System.IO.File]::Open($OutputPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::Write)
    $bw = [System.IO.BinaryWriter]::new($fs)
    $bw.Write([UInt16]0)
    $bw.Write([UInt16]1)
    $bw.Write([UInt16]$entries.Count)

    $offset = 6 + (16 * $entries.Count)
    foreach ($entry in $entries) {
        $dim = if ($entry.Size -ge 256) { 0 } else { [byte]$entry.Size }
        $bw.Write([byte]$dim)
        $bw.Write([byte]$dim)
        $bw.Write([byte]0)
        $bw.Write([byte]0)
        $bw.Write([UInt16]1)
        $bw.Write([UInt16]32)
        $bw.Write([UInt32]$entry.Bytes.Length)
        $bw.Write([UInt32]$offset)
        $offset += $entry.Bytes.Length
    }

    foreach ($entry in $entries) {
        $bw.Write($entry.Bytes)
    }
    $bw.Dispose()
    $fs.Dispose()
}

function New-Preview {
    $canvas = [System.Drawing.Bitmap]::new(900, 420, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $canvas.SetResolution(96, 96)
    $g = [System.Drawing.Graphics]::FromImage($canvas)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
    $g.Clear((New-Color 5 12 16 255))
    $gridPen = New-Pen (New-Color 24 100 120 44) 1
    for ($x = 0; $x -le 900; $x += 60) { $g.DrawLine($gridPen, $x, 0, $x, 420) }
    for ($y = 0; $y -le 420; $y += 60) { $g.DrawLine($gridPen, 0, $y, 900, $y) }
    $gridPen.Dispose()

    $font = [System.Drawing.Font]::new("Arial", 20, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 13, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new((New-Color 185 245 255 235))
    $g.DrawString("Eggcore Protocol icon set", $font, $brush, 30, 24)
    $g.DrawString("PNG sizes + Windows .ico generated from original core mark style", $small, $brush, 30, 52)

    $xPos = 42
    foreach ($size in @(16, 32, 48, 64, 128, 256)) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $IconDir "icon_$size.png"))
        $draw = if ($size -lt 64) { $size * 3 } elseif ($size -eq 64) { 96 } elseif ($size -eq 128) { 128 } else { 160 }
        $g.DrawImage($img, $xPos, 118, $draw, $draw)
        $g.DrawString("${size}px", $small, $brush, $xPos, 292)
        $xPos += $draw + 44
        $img.Dispose()
    }

    $brush.Dispose()
    $font.Dispose()
    $small.Dispose()
    $previewPath = Join-Path $ArtDir "IconSet_Preview.png"
    $canvas.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $g.Dispose()
    $canvas.Dispose()
    Write-Host "Generated IconSet_Preview.png"
}

$pngSizes = @(16, 32, 48, 64, 128, 256, 512)
$icoSizes = @(16, 32, 48, 64, 128, 256)

Save-IconPngs $pngSizes
New-IcoFile $icoSizes (Join-Path $IconDir "EggcoreProtocol.ico")
Copy-Item -LiteralPath (Join-Path $IconDir "EggcoreProtocol.ico") -Destination (Join-Path $ArtDir "EggcoreProtocol.ico") -Force
Write-Host "Generated EggcoreProtocol.ico"
New-Preview

Write-Host ""
Write-Host "Exe icon assets complete."
Write-Host "Icons: $IconDir"
Write-Host "Art source copies: $ArtDir"
