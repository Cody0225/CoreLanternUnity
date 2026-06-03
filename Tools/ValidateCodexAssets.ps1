# ValidateCodexAssets.ps1
#
# Codex が生成した素材の規格適合を自動検証するスクリプト。
# Codex は素材完成後にこれを実行し、エラー 0 を確認してから完了報告する。
#
# Usage:
#   .\Tools\ValidateCodexAssets.ps1                    # 全カテゴリ検証
#   .\Tools\ValidateCodexAssets.ps1 -Category Logo      # ロゴだけ
#   .\Tools\ValidateCodexAssets.ps1 -Category Steam     # Steam カプセル
#   .\Tools\ValidateCodexAssets.ps1 -Category Character # キャラ画像
#   .\Tools\ValidateCodexAssets.ps1 -Category Audio     # BGM/SE
#
# Exit code: 0 = OK / 1 = エラーあり (CI フックにも使える)

param(
    [string]$Category = 'All'
)

$ErrorActionPreference = 'Continue'
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$ResourceDir = Join-Path $ProjectRoot 'Assets\Resources\Skins'
$AudioDir = Join-Path $ProjectRoot 'Assets\Resources\Audio'
$ArtSourceDir = Join-Path $ProjectRoot 'Assets\ArtSource'

$Errors = @()
$Warnings = @()

function Add-Error([string]$msg) {
    $script:Errors += $msg
    Write-Host "[ERROR] $msg" -ForegroundColor Red
}

function Add-Warning([string]$msg) {
    $script:Warnings += $msg
    Write-Host "[WARN] $msg" -ForegroundColor Yellow
}

function Add-Ok([string]$msg) {
    Write-Host "[OK] $msg" -ForegroundColor Green
}

# ── PNG メタ情報読み取り ──
function Get-PngInfo([string]$path) {
    if (-not (Test-Path $path)) { return $null }
    Add-Type -AssemblyName System.Drawing
    try {
        $img = [System.Drawing.Image]::FromFile($path)
        $info = [PSCustomObject]@{
            Width = $img.Width
            Height = $img.Height
            HasAlpha = $img.PixelFormat -match 'Argb|Indexed'
            FileSize = (Get-Item $path).Length
        }
        $img.Dispose()
        return $info
    } catch {
        return $null
    }
}

# ── 透過 PNG の四隅 alpha チェック ──
function Test-CornerAlpha([string]$path) {
    if (-not (Test-Path $path)) { return $false }
    Add-Type -AssemblyName System.Drawing
    try {
        $bmp = New-Object System.Drawing.Bitmap($path)
        $corners = @(
            $bmp.GetPixel(0, 0),
            $bmp.GetPixel($bmp.Width - 1, 0),
            $bmp.GetPixel(0, $bmp.Height - 1),
            $bmp.GetPixel($bmp.Width - 1, $bmp.Height - 1)
        )
        $bmp.Dispose()
        # 全て alpha=0 なら true
        $allTransparent = ($corners | Where-Object { $_.A -eq 0 }).Count -eq 4
        return $allTransparent
    } catch {
        return $false
    }
}

# ── .meta ファイル存在チェック ──
function Test-MetaFile([string]$path) {
    return Test-Path ($path + '.meta')
}

# ── カテゴリ別検証関数 ──

function Validate-Logo {
    Write-Host "`n=== Validating: Logo (P0-1) ===" -ForegroundColor Cyan
    $expectedLogos = @(
        @{ Path = 'GameLogo.png'; W = 1024; H = 512; Alpha = $true }
        @{ Path = 'GameLogo_Mono.png'; W = 1024; H = 512; Alpha = $true }
        @{ Path = 'StudioLogo_Embercore.png'; W = 512; H = 512; Alpha = $true }
        @{ Path = 'StudioLogo_Embercore_Horizontal.png'; W = 1024; H = 384; Alpha = $true }
        @{ Path = 'StudioLogo_Embercore_Mono.png'; W = 512; H = 512; Alpha = $true }
        @{ Path = 'StudioLogo_Embercore_64.png'; W = 64; H = 64; Alpha = $true }
    )
    foreach ($spec in $expectedLogos) {
        $fullPath = Join-Path $ResourceDir $spec.Path
        if (-not (Test-Path $fullPath)) {
            Add-Warning "Missing: $($spec.Path)"
            continue
        }
        $info = Get-PngInfo $fullPath
        if ($info.Width -ne $spec.W -or $info.Height -ne $spec.H) {
            Add-Error "$($spec.Path): Expected $($spec.W)x$($spec.H), got $($info.Width)x$($info.Height)"
        } elseif ($spec.Alpha -and -not (Test-CornerAlpha $fullPath)) {
            Add-Error "$($spec.Path): Corner alpha not 0 (transparent PNG requirement)"
        } elseif (-not (Test-MetaFile $fullPath)) {
            Add-Warning "$($spec.Path): .meta file missing"
        } else {
            Add-Ok "$($spec.Path): $($info.Width)x$($info.Height) OK"
        }
    }
}

function Validate-Steam {
    Write-Host "`n=== Validating: Steam Capsules (P0-2) ===" -ForegroundColor Cyan
    $capsuleDir = Join-Path $ArtSourceDir 'SteamCapsules'
    if (-not (Test-Path $capsuleDir)) {
        Add-Warning "Directory not found: $capsuleDir"
        return
    }
    $expected = @(
        @{ Path = 'SteamCapsule_Library_600x900.png'; W = 600; H = 900; Alpha = $false }
        @{ Path = 'SteamCapsule_Hero_3840x1240.png'; W = 3840; H = 1240; Alpha = $false }
        @{ Path = 'SteamCapsule_LibraryLogo_1280x720.png'; W = 1280; H = 720; Alpha = $true }
        @{ Path = 'SteamCapsule_Header_460x215.png'; W = 460; H = 215; Alpha = $false }
        @{ Path = 'SteamCapsule_Small_462x174.png'; W = 462; H = 174; Alpha = $false }
        @{ Path = 'SteamCapsule_Main_920x430.png'; W = 920; H = 430; Alpha = $false }
        @{ Path = 'SteamCapsule_PageBg_1438x810.png'; W = 1438; H = 810; Alpha = $false }
    )
    foreach ($spec in $expected) {
        $fullPath = Join-Path $capsuleDir $spec.Path
        if (-not (Test-Path $fullPath)) {
            Add-Warning "Missing: $($spec.Path)"
            continue
        }
        $info = Get-PngInfo $fullPath
        if ($info.Width -ne $spec.W -or $info.Height -ne $spec.H) {
            Add-Error "$($spec.Path): Expected $($spec.W)x$($spec.H), got $($info.Width)x$($info.Height)"
        } elseif ($info.FileSize -gt 5MB) {
            Add-Warning "$($spec.Path): File size $([Math]::Round($info.FileSize/1MB, 1))MB exceeds 5MB recommendation"
        } else {
            Add-Ok "$($spec.Path): $($info.Width)x$($info.Height) ($([Math]::Round($info.FileSize/1KB))KB) OK"
        }
    }
}

function Validate-Icon {
    Write-Host "`n=== Validating: Exe Icons (P0-3) ===" -ForegroundColor Cyan
    $iconDir = Join-Path $ProjectRoot 'build\Icons'
    if (-not (Test-Path $iconDir)) {
        Add-Warning "Directory not found: $iconDir"
        return
    }
    $sizes = @(16, 32, 48, 64, 128, 256, 512)
    foreach ($size in $sizes) {
        $path = Join-Path $iconDir "icon_$size.png"
        if (-not (Test-Path $path)) {
            Add-Warning "Missing: icon_$size.png"
            continue
        }
        $info = Get-PngInfo $path
        if ($info.Width -ne $size -or $info.Height -ne $size) {
            Add-Error "icon_$size.png: Expected ${size}x${size}, got $($info.Width)x$($info.Height)"
        } else {
            Add-Ok "icon_$size.png: ${size}x${size} OK"
        }
    }
    $icoPath = Join-Path $iconDir 'EggcoreProtocol.ico'
    if (-not (Test-Path $icoPath)) {
        Add-Warning "Missing: EggcoreProtocol.ico (Windows icon format)"
    } else {
        Add-Ok "EggcoreProtocol.ico exists"
    }
}

function Validate-Character {
    Write-Host "`n=== Validating: Character Images (P1-1) ===" -ForegroundColor Cyan
    $checked = 0
    $missing = 0
    $invalidSize = 0
    $invalidAlpha = 0

    # 11 species × 4 levels (L0) + 11 × 3 routes × 3 (L1/L2/L3) + 6 fusions × 4 levels = 132
    for ($s = 1; $s -le 11; $s++) {
        # L0 (素体)
        $path = Join-Path $ResourceDir "Partner_S${s}_L0.png"
        $checked++
        if (-not (Test-Path $path)) { $missing++; continue }
        $info = Get-PngInfo $path
        if ($info.Width -ne 512 -or $info.Height -ne 512) { $invalidSize++ }
        elseif (-not (Test-CornerAlpha $path)) { $invalidAlpha++ }
    }
    for ($s = 1; $s -le 11; $s++) {
        for ($r = 1; $r -le 3; $r++) {
            for ($l = 1; $l -le 3; $l++) {
                $path = Join-Path $ResourceDir "Partner_S${s}_R${r}_L${l}.png"
                $checked++
                if (-not (Test-Path $path)) { $missing++; continue }
                $info = Get-PngInfo $path
                if ($info.Width -ne 512 -or $info.Height -ne 512) { $invalidSize++ }
                elseif (-not (Test-CornerAlpha $path)) { $invalidAlpha++ }
            }
        }
    }
    for ($f = 1; $f -le 6; $f++) {
        for ($l = 0; $l -le 3; $l++) {
            # Cross evolution uses different naming, e.g., Partner_S1_F1_L0.png
            # (any species can use any fusion, but Codex typically generates S1 base)
            $path = Join-Path $ResourceDir "Partner_S1_F${f}_L${l}.png"
            $checked++
            if (-not (Test-Path $path)) { $missing++; continue }
            $info = Get-PngInfo $path
            if ($info.Width -ne 512 -or $info.Height -ne 512) { $invalidSize++ }
            elseif (-not (Test-CornerAlpha $path)) { $invalidAlpha++ }
        }
    }
    $present = $checked - $missing
    Write-Host "  Total checked: $checked / 132 (sample slots)"
    Write-Host "  Present: $present"
    Write-Host "  Missing: $missing"
    if ($invalidSize -gt 0) { Add-Error "Wrong size (not 512x512): $invalidSize" }
    if ($invalidAlpha -gt 0) { Add-Error "Corner alpha not 0: $invalidAlpha" }
    if ($invalidSize -eq 0 -and $invalidAlpha -eq 0 -and $present -gt 0) {
        Add-Ok "$present character images valid"
    }
    if ($missing -gt 0) {
        Add-Warning "$missing character image slots not yet generated (segmented Codex delivery in progress)"
    }
}

function Validate-Audio {
    Write-Host "`n=== Validating: Audio (P1-2 / P1-3) ===" -ForegroundColor Cyan
    if (-not (Test-Path $AudioDir)) {
        Add-Warning "Audio directory not found"
        return
    }
    $expectedBgm = @('BGM.wav', 'BGM_Stage2.wav', 'BGM_Boss_Pulswyrm.wav', 'BGM_Boss_Nullwyrm.wav')
    $expectedSe = @('Shoot.wav', 'Hit.wav', 'Kill.wav', 'Pickup.wav', 'LevelUp.wav', 'Evolve.wav', 'Fusion.wav', 'Boss.wav', 'GameOver.wav')

    foreach ($name in $expectedBgm + $expectedSe) {
        $path = Join-Path $AudioDir $name
        if (-not (Test-Path $path)) {
            Add-Warning "Missing audio: $name"
            continue
        }
        $size = (Get-Item $path).Length
        if ($name -like 'BGM*' -and $size -gt 5MB) {
            Add-Warning "$name : $([Math]::Round($size/1MB, 1))MB exceeds 5MB BGM recommendation"
        } elseif (-not (Test-MetaFile $path)) {
            Add-Warning "$name : .meta file missing"
        } else {
            Add-Ok "$name OK ($([Math]::Round($size/1KB))KB)"
        }
    }

    # Check audio manifest
    $manifest = Join-Path $AudioDir 'AudioManifest.json'
    if (Test-Path $manifest) {
        Add-Ok "AudioManifest.json exists"
    } else {
        Add-Warning "AudioManifest.json missing"
    }

    # Check license dir
    $licDir = Join-Path $AudioDir 'Licenses'
    if (Test-Path $licDir) {
        $licCount = (Get-ChildItem $licDir -File).Count
        Add-Ok "Licenses directory: $licCount files"
    } else {
        Add-Warning "Licenses directory missing - commercial audio must include license docs"
    }
}

function Validate-Hud {
    Write-Host "`n=== Validating: HUD 9-slice (P1-4) ===" -ForegroundColor Cyan
    $expected = @(
        @{ Path = 'HUD_Panel_Wave.png'; W = 250; H = 74 }
        @{ Path = 'HUD_Panel_HP.png'; W = 270; H = 104 }
        @{ Path = 'HUD_Panel_Level.png'; W = 270; H = 76 }
        @{ Path = 'HUD_Panel_ChipMini.png'; W = 190; H = 42 }
        @{ Path = 'HUD_Bar_Back.png'; W = 132; H = 20 }
        @{ Path = 'HUD_Bar_HP_PlayerFill.png'; W = 132; H = 16 }
        @{ Path = 'HUD_Bar_HP_CoreFill.png'; W = 132; H = 16 }
        @{ Path = 'HUD_Bar_EXP_Fill.png'; W = 132; H = 16 }
        @{ Path = 'HUD_Bar_Back_v2.png'; W = 132; H = 20 }
        @{ Path = 'HUD_Bar_HP_PlayerFill_v2.png'; W = 128; H = 16 }
        @{ Path = 'HUD_Bar_HP_CoreFill_v2.png'; W = 128; H = 16 }
        @{ Path = 'HUD_Bar_EXP_Fill_v2.png'; W = 128; H = 16 }
        @{ Path = 'HUD_Bar_HP_Lag_v2.png'; W = 128; H = 16 }
        @{ Path = 'HUD_WaveProgress_Frame.png'; W = 520; H = 26 }
        @{ Path = 'HUD_WaveProgress_Fill_Normal.png'; W = 520; H = 22 }
        @{ Path = 'HUD_WaveProgress_Fill_Boss.png'; W = 520; H = 22 }
        @{ Path = 'HUD_WaveProgress_Frame_v2.png'; W = 512; H = 48 }
        @{ Path = 'HUD_WaveProgress_Fill_Normal_v2.png'; W = 512; H = 12 }
        @{ Path = 'HUD_WaveProgress_Fill_Boss_v2.png'; W = 512; H = 12 }
    )
    foreach ($spec in $expected) {
        $fullPath = Join-Path $ResourceDir $spec.Path
        if (-not (Test-Path $fullPath)) {
            Add-Warning "Missing: $($spec.Path)"
            continue
        }
        $info = Get-PngInfo $fullPath
        if ($info.Width -ne $spec.W -or $info.Height -ne $spec.H) {
            Add-Error "$($spec.Path): Expected $($spec.W)x$($spec.H), got $($info.Width)x$($info.Height)"
        } elseif (-not (Test-CornerAlpha $fullPath)) {
            Add-Warning "$($spec.Path): corners are not fully transparent; okay for native-size HUD, regenerate before strict 9-slice use"
        } else {
            Add-Ok "$($spec.Path): $($info.Width)x$($info.Height) OK"
        }
    }
}

# ── 実行 ──

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Codex Asset Validator - Eggcore Protocol" -ForegroundColor Cyan
Write-Host "Category: $Category" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan

switch ($Category) {
    'Logo'      { Validate-Logo }
    'Steam'     { Validate-Steam }
    'Icon'      { Validate-Icon }
    'Character' { Validate-Character }
    'Audio'     { Validate-Audio }
    'Hud'       { Validate-Hud }
    'All' {
        Validate-Logo
        Validate-Steam
        Validate-Icon
        Validate-Character
        Validate-Audio
        Validate-Hud
    }
    default {
        Write-Host "Unknown category: $Category" -ForegroundColor Red
        Write-Host "Valid: Logo, Steam, Icon, Character, Audio, Hud, All"
        exit 1
    }
}

# ── サマリ ──
Write-Host "`n===============================================" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Errors: $($Errors.Count)" -ForegroundColor $(if ($Errors.Count -eq 0) { 'Green' } else { 'Red' })
Write-Host "Warnings: $($Warnings.Count)" -ForegroundColor $(if ($Warnings.Count -eq 0) { 'Green' } else { 'Yellow' })

if ($Errors.Count -gt 0) {
    Write-Host "`n--- Errors detail ---" -ForegroundColor Red
    $Errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    exit 1
}
exit 0
