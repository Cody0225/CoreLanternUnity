param(
    [switch]$NoOverwrite
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ResourceDir = Join-Path $ProjectRoot 'Assets\Resources\Skins'
$ArtSourceDir = Join-Path $ProjectRoot 'Assets\ArtSource\HUD_V2'

New-Item -ItemType Directory -Force -Path $ResourceDir | Out-Null
New-Item -ItemType Directory -Force -Path $ArtSourceDir | Out-Null

function New-Rgba([int]$r, [int]$g, [int]$b, [int]$a) {
    return [System.Drawing.Color]::FromArgb($a, $r, $g, $b)
}

function Get-RoundedAlpha([int]$x, [int]$y, [int]$w, [int]$h, [int]$radius) {
    $cx = if ($x -lt $radius) { $radius } elseif ($x -ge $w - $radius) { $w - $radius - 1 } else { $x }
    $cy = if ($y -lt $radius) { $radius } elseif ($y -ge $h - $radius) { $h - $radius - 1 } else { $y }
    $dx = $x - $cx
    $dy = $y - $cy
    if (($dx * $dx + $dy * $dy) -le ($radius * $radius)) { return 255 }
    return 0
}

function Save-Png([System.Drawing.Bitmap]$Bitmap, [string]$Path) {
    if ($NoOverwrite -and (Test-Path -LiteralPath $Path)) {
        Write-Host "SKIP existing $Path"
        $Bitmap.Dispose()
        return
    }
    $Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $Bitmap.Dispose()
    Write-Host "WROTE $Path"
}

function Write-TextureMeta([string]$PngPath) {
    $metaPath = "$PngPath.meta"
    if (Test-Path -LiteralPath $metaPath) { return }
    $guid = [guid]::NewGuid().ToString('N')
    $content = @"
fileFormatVersion: 2
guid: $guid
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 1
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
    wrapU: 0
    wrapV: 0
    wrapW: 0
  nPOTScale: 1
  lightmap: 0
  compressionQuality: 50
  spriteMode: 0
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsToUnits: 100
  spriteBorder: {x: 0, y: 0, z: 0, w: 0}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 0
  spriteTessellationDetail: -1
  textureType: 0
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
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
    customData:
    physicsShape: []
    bones: []
    spriteID:
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    spriteCustomMetadata:
      entries: []
    nameFileIdTable: {}
  mipmapLimitGroupName:
  pSDRemoveMatte: 0
  userData:
  assetBundleName:
  assetBundleVariant:
"@
    [System.IO.File]::WriteAllText($metaPath, $content, [System.Text.UTF8Encoding]::new($false))
}

function New-BarBack([string]$Path) {
    $w = 132
    $h = 20
    $bmp = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    for ($y = 0; $y -lt $h; $y++) {
        for ($x = 0; $x -lt $w; $x++) {
            $mask = Get-RoundedAlpha $x $y $w $h 5
            if ($mask -eq 0) {
                $bmp.SetPixel($x, $y, (New-Rgba 0 0 0 0))
                continue
            }
            $edge = ($x -lt 2 -or $x -ge $w - 2 -or $y -lt 2 -or $y -ge $h - 2)
            $innerEdge = ($x -lt 5 -or $x -ge $w - 5 -or $y -lt 5 -or $y -ge $h - 5)
            if ($edge) {
                $bmp.SetPixel($x, $y, (New-Rgba 139 200 255 150))
            } elseif ($innerEdge) {
                $bmp.SetPixel($x, $y, (New-Rgba 44 62 71 190))
            } else {
                $shade = [int](18 + ($y / [Math]::Max(1, $h - 1)) * 12)
                $bmp.SetPixel($x, $y, (New-Rgba 11 $shade 24 235))
            }
        }
    }
    Save-Png $bmp $Path
    Write-TextureMeta $Path
}

function New-BarFill([string]$Path, [int]$r, [int]$g, [int]$b) {
    $w = 128
    $h = 16
    $bmp = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    for ($y = 0; $y -lt $h; $y++) {
        for ($x = 0; $x -lt $w; $x++) {
            $mask = Get-RoundedAlpha $x $y $w $h 4
            if ($mask -eq 0) {
                $bmp.SetPixel($x, $y, (New-Rgba 0 0 0 0))
                continue
            }
            $t = $x / [Math]::Max(1, $w - 1)
            $highlight = if ($y -le 3) { 34 } elseif ($y -ge $h - 4) { -24 } else { 0 }
            $pulse = [int](18 * [Math]::Sin($t * [Math]::PI))
            $rr = [Math]::Min(255, [Math]::Max(0, $r + $highlight + $pulse))
            $gg = [Math]::Min(255, [Math]::Max(0, $g + $highlight + $pulse))
            $bb = [Math]::Min(255, [Math]::Max(0, $b + $highlight + $pulse))
            $bmp.SetPixel($x, $y, (New-Rgba $rr $gg $bb 240))
        }
    }
    Save-Png $bmp $Path
    Write-TextureMeta $Path
}

function New-Preview([string]$Path) {
    $w = 420
    $h = 132
    $bmp = New-Object System.Drawing.Bitmap($w, $h, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear((New-Rgba 8 15 18 255))
    $font = New-Object System.Drawing.Font('Arial', 10, [System.Drawing.FontStyle]::Bold)
    $brush = New-Object System.Drawing.SolidBrush((New-Rgba 184 224 255 255))
    $labels = @('PLAYER HP', 'CORE HP', 'EXP', 'LAG')
    $fills = @(
        (Join-Path $ResourceDir 'HUD_Bar_HP_PlayerFill_v2.png'),
        (Join-Path $ResourceDir 'HUD_Bar_HP_CoreFill_v2.png'),
        (Join-Path $ResourceDir 'HUD_Bar_EXP_Fill_v2.png'),
        (Join-Path $ResourceDir 'HUD_Bar_HP_Lag_v2.png')
    )
    $back = [System.Drawing.Image]::FromFile((Join-Path $ResourceDir 'HUD_Bar_Back_v2.png'))
    for ($i = 0; $i -lt $labels.Length; $i++) {
        $y = 14 + $i * 28
        $g.DrawString($labels[$i], $font, $brush, 14, $y + 2)
        $g.DrawImage($back, 110, $y, 132, 20)
        $fill = [System.Drawing.Image]::FromFile($fills[$i])
        $fillW = if ($i -eq 0) { 96 } elseif ($i -eq 1) { 112 } elseif ($i -eq 2) { 72 } else { 52 }
        $g.DrawImage($fill, 112, $y + 2, $fillW, 16)
        $fill.Dispose()
    }
    $back.Dispose()
    $font.Dispose()
    $brush.Dispose()
    $g.Dispose()
    Save-Png $bmp $Path
    Write-TextureMeta $Path
}

New-BarBack (Join-Path $ResourceDir 'HUD_Bar_Back_v2.png')
New-BarFill (Join-Path $ResourceDir 'HUD_Bar_HP_PlayerFill_v2.png') 80 224 255
New-BarFill (Join-Path $ResourceDir 'HUD_Bar_HP_CoreFill_v2.png') 255 206 59
New-BarFill (Join-Path $ResourceDir 'HUD_Bar_EXP_Fill_v2.png') 123 237 82
New-BarFill (Join-Path $ResourceDir 'HUD_Bar_HP_Lag_v2.png') 255 72 56

Copy-Item -LiteralPath (Join-Path $ResourceDir 'HUD_Bar_Back_v2.png') -Destination (Join-Path $ArtSourceDir 'HUD_Bar_Back_v2_preview.png') -Force
Copy-Item -LiteralPath (Join-Path $ResourceDir 'HUD_Bar_HP_PlayerFill_v2.png') -Destination (Join-Path $ArtSourceDir 'HUD_Bar_HP_PlayerFill_v2_preview.png') -Force
Copy-Item -LiteralPath (Join-Path $ResourceDir 'HUD_Bar_HP_CoreFill_v2.png') -Destination (Join-Path $ArtSourceDir 'HUD_Bar_HP_CoreFill_v2_preview.png') -Force
Copy-Item -LiteralPath (Join-Path $ResourceDir 'HUD_Bar_EXP_Fill_v2.png') -Destination (Join-Path $ArtSourceDir 'HUD_Bar_EXP_Fill_v2_preview.png') -Force
Copy-Item -LiteralPath (Join-Path $ResourceDir 'HUD_Bar_HP_Lag_v2.png') -Destination (Join-Path $ArtSourceDir 'HUD_Bar_HP_Lag_v2_preview.png') -Force

Write-TextureMeta (Join-Path $ArtSourceDir 'HUD_Bar_Back_v2_preview.png')
Write-TextureMeta (Join-Path $ArtSourceDir 'HUD_Bar_HP_PlayerFill_v2_preview.png')
Write-TextureMeta (Join-Path $ArtSourceDir 'HUD_Bar_HP_CoreFill_v2_preview.png')
Write-TextureMeta (Join-Path $ArtSourceDir 'HUD_Bar_EXP_Fill_v2_preview.png')
Write-TextureMeta (Join-Path $ArtSourceDir 'HUD_Bar_HP_Lag_v2_preview.png')

New-Preview (Join-Path $ArtSourceDir 'HUD_Bar_v2_composite_preview.png')

Write-Host 'HUD bar v2 assets generated.'
