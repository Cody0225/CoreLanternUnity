param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$skinsDir = Join-Path $ProjectRoot "Assets/Resources/Skins"
$artDir = Join-Path $ProjectRoot "Assets/ArtSource/HUD_9Slice_20260531"
New-Item -ItemType Directory -Force -Path $skinsDir | Out-Null
New-Item -ItemType Directory -Force -Path $artDir | Out-Null

function Color-A([int]$a, [int]$r, [int]$g, [int]$b) {
    return [System.Drawing.Color]::FromArgb($a, $r, $g, $b)
}

function New-Rect([float]$x, [float]$y, [float]$w, [float]$h) {
    return New-Object System.Drawing.RectangleF $x, $y, $w, $h
}

function New-Bmp([int]$w, [int]$h) {
    return New-Object System.Drawing.Bitmap $w, $h, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
}

function New-RoundedPath([System.Drawing.RectangleF]$r, [float]$radius) {
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $d = $radius * 2
    $path.AddArc($r.X, $r.Y, $d, $d, 180, 90)
    $path.AddArc($r.Right - $d, $r.Y, $d, $d, 270, 90)
    $path.AddArc($r.Right - $d, $r.Bottom - $d, $d, $d, 0, 90)
    $path.AddArc($r.X, $r.Bottom - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function Save-Png([System.Drawing.Bitmap]$bmp, [string]$path) {
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
}

function New-GuidString {
    return ([guid]::NewGuid().ToString("N"))
}

function Ensure-Meta([string]$pngPath, [int]$borderL, [int]$borderB, [int]$borderR, [int]$borderT) {
    $metaPath = "$pngPath.meta"
    if (Test-Path $metaPath) { return }
    $guid = New-GuidString
    $text = @"
fileFormatVersion: 2
guid: $guid
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsToUnits: 100
  spriteBorder: {x: $borderL, y: $borderB, z: $borderR, w: $borderT}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {}
  spritePackingTag:
  pSDRemoveMatte: 0
  pSDShowRemoveMatteOption: 0
  userData:
  assetBundleName:
  assetBundleVariant:
"@
    [System.IO.File]::WriteAllText($metaPath, $text, [System.Text.Encoding]::UTF8)
}

function Draw-Panel([string]$name, [System.Drawing.Color]$accent, [System.Drawing.Color]$glow) {
    $w = 512; $h = 512
    $bmp = New-Bmp $w $h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    $outer = New-Rect 24 24 464 464
    $inner = New-Rect 40 40 432 432
    $outerPath = New-RoundedPath $outer 20
    $innerPath = New-RoundedPath $inner 14

    $center = New-Object System.Drawing.SolidBrush (Color-A 205 4 22 27)
    $g.FillPath($center, $outerPath)
    $center.Dispose()

    $soft = New-Object System.Drawing.Drawing2D.LinearGradientBrush $inner, (Color-A 48 120 255 245), (Color-A 8 4 18 24), 90
    $g.FillPath($soft, $innerPath)
    $soft.Dispose()

    for ($i = 0; $i -lt 4; $i++) {
        $pen = New-Object System.Drawing.Pen (Color-A (70 - $i * 12) $glow.R $glow.G $glow.B), (8 - $i)
        $g.DrawPath($pen, $outerPath)
        $pen.Dispose()
    }

    $edge = New-Object System.Drawing.Pen $accent, 3
    $dimEdge = New-Object System.Drawing.Pen (Color-A 120 $accent.R $accent.G $accent.B), 1
    $g.DrawPath($edge, $outerPath)
    $g.DrawPath($dimEdge, $innerPath)

    $gold = New-Object System.Drawing.Pen (Color-A 160 255 206 58), 3
    $cyan = New-Object System.Drawing.Pen (Color-A 170 90 236 248), 3

    $g.DrawLine($cyan, 54, 52, 190, 52)
    $g.DrawLine($cyan, 52, 54, 52, 132)
    $g.DrawLine($gold, 54, 460, 132, 460)
    $g.DrawLine($gold, 52, 382, 52, 460)
    $g.DrawLine($dimEdge, 322, 52, 460, 52)
    $g.DrawLine($dimEdge, 460, 54, 460, 132)
    $g.DrawLine($dimEdge, 322, 460, 460, 460)
    $g.DrawLine($dimEdge, 460, 382, 460, 460)

    $edge.Dispose(); $dimEdge.Dispose(); $gold.Dispose(); $cyan.Dispose()
    $outerPath.Dispose(); $innerPath.Dispose()

    $path = Join-Path $skinsDir "$name.png"
    Save-Png $bmp $path
    Copy-Item -LiteralPath $path -Destination (Join-Path $artDir "$name.png") -Force
    Ensure-Meta $path 24 24 24 24
    $g.Dispose(); $bmp.Dispose()
}

function Draw-Bar([string]$name, [System.Drawing.Color]$fill, [bool]$isBack) {
    $w = 256; $h = 64
    $bmp = New-Bmp $w $h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    $r = New-Rect 8 14 240 36
    $path = New-RoundedPath $r 12
    if ($isBack) {
        $b = New-Object System.Drawing.SolidBrush (Color-A 210 3 14 18)
        $g.FillPath($b, $path)
        $b.Dispose()
        $p = New-Object System.Drawing.Pen (Color-A 140 82 219 232), 2
        $g.DrawPath($p, $path)
        $p.Dispose()
    } else {
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush $r, (Color-A 255 $fill.R $fill.G $fill.B), (Color-A 180 ([Math]::Max(0,$fill.R-35)) ([Math]::Max(0,$fill.G-35)) ([Math]::Max(0,$fill.B-35))), 0
        $g.FillPath($brush, $path)
        $brush.Dispose()
        $shine = New-Object System.Drawing.SolidBrush (Color-A 62 255 255 255)
        $g.FillRectangle($shine, 18, 18, 218, 6)
        $shine.Dispose()
    }
    $path.Dispose()

    $pathOut = Join-Path $skinsDir "$name.png"
    Save-Png $bmp $pathOut
    Copy-Item -LiteralPath $pathOut -Destination (Join-Path $artDir "$name.png") -Force
    Ensure-Meta $pathOut 12 8 12 8
    $g.Dispose(); $bmp.Dispose()
}

function Draw-Wave([string]$name, [System.Drawing.Color]$fill, [bool]$isFrame) {
    $w = 512; $h = 48
    $bmp = New-Bmp $w $h
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    if ($isFrame) {
        $back = New-Object System.Drawing.SolidBrush (Color-A 190 3 14 20)
        $g.FillRectangle($back, 8, 10, 496, 28)
        $back.Dispose()
        $p1 = New-Object System.Drawing.Pen (Color-A 180 88 232 245), 2
        $p2 = New-Object System.Drawing.Pen (Color-A 130 255 205 55), 1
        $g.DrawRectangle($p1, 8, 10, 496, 28)
        $g.DrawLine($p2, 22, 14, 166, 14)
        $g.DrawLine($p2, 346, 34, 488, 34)
        $p1.Dispose(); $p2.Dispose()
    } else {
        $r = New-Rect 8 16 496 16
        $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush $r, (Color-A 255 $fill.R $fill.G $fill.B), (Color-A 220 ([Math]::Max(0,$fill.R-45)) ([Math]::Max(0,$fill.G-45)) ([Math]::Max(0,$fill.B-45))), 0
        $g.FillRectangle($brush, $r)
        $brush.Dispose()
        $shine = New-Object System.Drawing.SolidBrush (Color-A 52 255 255 255)
        $g.FillRectangle($shine, 16, 18, 480, 3)
        $shine.Dispose()
    }

    $pathOut = Join-Path $skinsDir "$name.png"
    Save-Png $bmp $pathOut
    Copy-Item -LiteralPath $pathOut -Destination (Join-Path $artDir "$name.png") -Force
    Ensure-Meta $pathOut 16 8 16 8
    $g.Dispose(); $bmp.Dispose()
}

Draw-Panel "HUD9_Panel_Wave" (Color-A 230 82 235 248) (Color-A 255 82 235 248)
Draw-Panel "HUD9_Panel_HP" (Color-A 230 82 235 248) (Color-A 255 82 235 248)
Draw-Panel "HUD9_Panel_Level" (Color-A 230 126 255 92) (Color-A 255 126 255 92)
Draw-Panel "HUD9_Panel_ChipMini" (Color-A 230 255 206 58) (Color-A 255 255 206 58)

Draw-Bar "HUD9_Bar_Back" (Color-A 255 42 82 90) $true
Draw-Bar "HUD9_Bar_HP_PlayerFill" (Color-A 255 64 214 248) $false
Draw-Bar "HUD9_Bar_HP_CoreFill" (Color-A 255 255 206 58) $false
Draw-Bar "HUD9_Bar_EXP_Fill" (Color-A 255 129 245 77) $false

Draw-Wave "HUD9_WaveProgress_Frame" (Color-A 255 72 220 244) $true
Draw-Wave "HUD9_WaveProgress_Fill_Normal" (Color-A 255 56 204 236) $false
Draw-Wave "HUD9_WaveProgress_Fill_Boss" (Color-A 255 246 76 235) $false

$preview = New-Bmp 1180 900
$pg = [System.Drawing.Graphics]::FromImage($preview)
$pg.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$pg.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$pg.Clear([System.Drawing.Color]::FromArgb(255, 5, 12, 16))
$font = New-Object System.Drawing.Font "Arial", 20, ([System.Drawing.FontStyle]::Bold), ([System.Drawing.GraphicsUnit]::Pixel)
$brush = New-Object System.Drawing.SolidBrush (Color-A 255 164 255 255)
$pg.DrawString("HUD 9-slice candidates - native ratio preview", $font, $brush, 30, 24)
$names = @(
    "HUD9_Panel_Wave","HUD9_Panel_HP","HUD9_Panel_Level","HUD9_Panel_ChipMini",
    "HUD9_Bar_Back","HUD9_Bar_HP_PlayerFill","HUD9_Bar_HP_CoreFill","HUD9_Bar_EXP_Fill",
    "HUD9_WaveProgress_Frame","HUD9_WaveProgress_Fill_Normal","HUD9_WaveProgress_Fill_Boss"
)
$x = 34; $y = 70
foreach ($n in $names) {
    $img = [System.Drawing.Image]::FromFile((Join-Path $skinsDir "$n.png"))
    $scale = if ($img.Width -eq 512 -and $img.Height -eq 512) { 0.34 } elseif ($img.Width -eq 256) { 0.9 } else { 1.0 }
    $dw = $img.Width * $scale; $dh = $img.Height * $scale
    if ($x + $dw -gt 1140) { $x = 34; $y += 210 }
    $pg.DrawImage($img, (New-Rect $x $y $dw $dh))
    $pg.DrawString($n, $font, $brush, $x, ($y + $dh + 8))
    $x += $dw + 34
    $img.Dispose()
}
$font.Dispose(); $brush.Dispose()
Save-Png $preview (Join-Path $artDir "HUD9_Preview.png")
$pg.Dispose(); $preview.Dispose()

Write-Host "Generated HUD 9-slice candidates:"
Get-ChildItem -LiteralPath $skinsDir -Filter "HUD9_*.png" | Sort-Object Name | ForEach-Object { Write-Host "  $($_.FullName)" }
Write-Host "Preview: $(Join-Path $artDir 'HUD9_Preview.png')"
