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

    $templatePath = Join-Path $SkinDir "GameLogo.png.meta"
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

function Save-Png {
    param([System.Drawing.Bitmap]$Bitmap, [string]$Name)
    $resourcePath = Join-Path $SkinDir $Name
    if ((Test-Path -LiteralPath $resourcePath) -and (-not $Force)) {
        Write-Host "Skip existing $Name"
    } else {
        $Bitmap.Save($resourcePath, [System.Drawing.Imaging.ImageFormat]::Png)
        Ensure-TextureMeta $resourcePath
        Write-Host "Generated $Name"
    }

    $artPath = Join-Path $ArtDir $Name
    $Bitmap.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $artPath
}

$sourcePath = Join-Path $SkinDir "GameLogo.png"
if (-not (Test-Path -LiteralPath $sourcePath)) {
    throw "Missing source logo: $sourcePath"
}

$src = [System.Drawing.Bitmap]::FromFile($sourcePath)
$minX = $src.Width
$minY = $src.Height
$maxX = -1
$maxY = -1

for ($y = 0; $y -lt $src.Height; $y++) {
    for ($x = 0; $x -lt $src.Width; $x++) {
        if ($src.GetPixel($x, $y).A -gt 8) {
            if ($x -lt $minX) { $minX = $x }
            if ($y -lt $minY) { $minY = $y }
            if ($x -gt $maxX) { $maxX = $x }
            if ($y -gt $maxY) { $maxY = $y }
        }
    }
}

if ($maxX -lt 0 -or $maxY -lt 0) {
    $src.Dispose()
    throw "Logo has no visible pixels."
}

$padX = 24
$padY = 18
$minX = [Math]::Max(0, $minX - $padX)
$minY = [Math]::Max(0, $minY - $padY)
$maxX = [Math]::Min($src.Width - 1, $maxX + $padX)
$maxY = [Math]::Min($src.Height - 1, $maxY + $padY)

$cropW = $maxX - $minX + 1
$cropH = $maxY - $minY + 1
$crop = [System.Drawing.Rectangle]::new($minX, $minY, $cropW, $cropH)

$outW = 1024
$outH = 320
$dst = [System.Drawing.Bitmap]::new($outW, $outH, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$dst.SetResolution(96, 96)
$g = [System.Drawing.Graphics]::FromImage($dst)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
$g.Clear([System.Drawing.Color]::Transparent)

$scale = [Math]::Min(($outW - 32) / $cropW, ($outH - 24) / $cropH)
$drawW = [int][Math]::Round($cropW * $scale)
$drawH = [int][Math]::Round($cropH * $scale)
$drawX = [int][Math]::Round(($outW - $drawW) / 2)
$drawY = [int][Math]::Round(($outH - $drawH) / 2)
$g.DrawImage($src, [System.Drawing.Rectangle]::new($drawX, $drawY, $drawW, $drawH), $crop, [System.Drawing.GraphicsUnit]::Pixel)
$g.Dispose()
$src.Dispose()

Save-Png $dst "GameLogo_TitleCompact.png"
$dst.Dispose()

Write-Host ""
Write-Host "Title compact logo complete."
