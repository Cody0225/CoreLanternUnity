param(
    [int]$MaxExamples = 12
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$AudioDir = Join-Path $ProjectRoot "Assets\Resources\Audio"
$CoreScriptPath = Join-Path $ProjectRoot "Assets\Scripts\CoreLanternGame.cs"
$BacklogPath = Join-Path $ProjectRoot "IMAGE_ASSET_BACKLOG.md"

function New-Set {
    return New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)
}

function Add-ResourceName($Set, [string]$Name) {
    if ([string]::IsNullOrWhiteSpace($Name)) {
        return
    }
    if ($Name.EndsWith("_", [System.StringComparison]::OrdinalIgnoreCase)) {
        return
    }
    if (!$Name.EndsWith(".png", [System.StringComparison]::OrdinalIgnoreCase)) {
        $Name = "$Name.png"
    }
    [void]$Set.Add($Name)
}

function Write-List([string]$Label, $Items) {
    $array = @($Items)
    Write-Host "$Label ($($array.Count))"
    foreach ($item in ($array | Sort-Object | Select-Object -First $MaxExamples)) {
        Write-Host "  - $item"
    }
    if ($array.Count -gt $MaxExamples) {
        Write-Host "  ... $($array.Count - $MaxExamples) more"
    }
}

$skinFiles = @()
$skinNames = New-Set
if (Test-Path -LiteralPath $SkinDir) {
    $skinFiles = @(Get-ChildItem -LiteralPath $SkinDir -Filter "*.png" -File)
    foreach ($file in $skinFiles) {
        [void]$skinNames.Add($file.Name)
    }
}

$loadedSprites = New-Set
if (Test-Path -LiteralPath $CoreScriptPath) {
    $coreText = Get-Content -LiteralPath $CoreScriptPath -Encoding UTF8 -Raw
    $quote = [regex]::Escape([string][char]34)
    $loadPattern = "LoadOptionalSprite\(" + $quote + "Skins/([^" + $quote + "]+)" + $quote
    $resourcePattern = "Resources[.]Load<Sprite>\(" + $quote + "Skins/([^" + $quote + "]+)" + $quote

    foreach ($match in [regex]::Matches($coreText, $loadPattern)) {
        Add-ResourceName $loadedSprites $match.Groups[1].Value
    }
    foreach ($match in [regex]::Matches($coreText, $resourcePattern)) {
        Add-ResourceName $loadedSprites $match.Groups[1].Value
    }
}

$loadedExisting = @()
$loadedMissing = @()
foreach ($name in $loadedSprites) {
    if ($skinNames.Contains($name)) {
        $loadedExisting += $name
    } else {
        $loadedMissing += $name
    }
}

$unloadedSkins = @()
foreach ($name in $skinNames) {
    if (!$loadedSprites.Contains($name)) {
        $unloadedSkins += $name
    }
}

$hud9Files = @($skinFiles | Where-Object { $_.Name -like "HUD9_*.png" } | ForEach-Object { $_.Name })
$hud9Hooked = @($hud9Files | Where-Object { $loadedSprites.Contains($_) })
$hud9Waiting = @($hud9Files | Where-Object { !$loadedSprites.Contains($_) })

$stage45Support = @(
    "StageThumb_Frost.png",
    "StageThumb_Storm.png",
    "Stage4_FrostCrystal_A.png",
    "Stage4_FrostPatch_A.png",
    "Stage5_LightningMarker_A.png",
    "Stage5_LightningStrike_A.png"
)
$stage45Present = @($stage45Support | Where-Object { $skinNames.Contains($_) })
$stage45Hooked = @($stage45Support | Where-Object { $loadedSprites.Contains($_) })
$stage45Waiting = @($stage45Present | Where-Object { !$loadedSprites.Contains($_) })

$aEggPresentation = @(
    "Title_CoreEgg_ASelected_v1.png",
    "Result_CoreEgg_ASelected_v1.png",
    "Icon_CoreEgg_ASelected_v1.png"
)
$aEggPresent = @($aEggPresentation | Where-Object { $skinNames.Contains($_) })
$aEggHooked = @($aEggPresentation | Where-Object { $loadedSprites.Contains($_) })
$aEggWaiting = @($aEggPresent | Where-Object { !$loadedSprites.Contains($_) })

$v2Ui = @($skinFiles | Where-Object { $_.Name -match "_v2[.]png$" } | ForEach-Object { $_.Name })
$v2Hooked = @($v2Ui | Where-Object { $loadedSprites.Contains($_) })
$v2Waiting = @($v2Ui | Where-Object { !$loadedSprites.Contains($_) })

$backlogDone = New-Set
if (Test-Path -LiteralPath $BacklogPath) {
    $backlogLines = @(Get-Content -LiteralPath $BacklogPath -Encoding UTF8)
    foreach ($line in $backlogLines) {
        if ($line -match "DONE" -or $line -match "HOOKED") {
            foreach ($match in [regex]::Matches($line, "\x60([^\x60]+[.]png)\x60")) {
                $leaf = Split-Path -Leaf ($match.Groups[1].Value.Replace([char]47, [char]92))
                [void]$backlogDone.Add($leaf)
            }
        }
    }
}

$wavFiles = @()
$manifestEntries = 0
if (Test-Path -LiteralPath $AudioDir) {
    $wavFiles = @(Get-ChildItem -LiteralPath $AudioDir -Filter "*.wav" -File)
    $manifestPath = Join-Path $AudioDir "AudioManifest.json"
    if (Test-Path -LiteralPath $manifestPath) {
        $manifest = Get-Content -LiteralPath $manifestPath -Encoding UTF8 | ConvertFrom-Json
        $manifestEntries = @($manifest).Count
    }
}

Write-Host "Asset status report"
Write-Host "Project: $ProjectRoot"
Write-Host ""
Write-Host "Totals"
Write-Host "  Skins PNG files       : $($skinFiles.Count)"
Write-Host "  Static code refs      : $($loadedSprites.Count)"
Write-Host "  Static refs present   : $($loadedExisting.Count)"
Write-Host "  Static refs missing   : $($loadedMissing.Count)"
Write-Host "  Skins not static refs : $($unloadedSkins.Count)"
Write-Host "  Backlog done/hooked   : $($backlogDone.Count)"
Write-Host "  WAV files             : $($wavFiles.Count)"
Write-Host "  Audio manifest entries: $manifestEntries"
Write-Host ""

Write-Host "HUD9 9-slice"
Write-Host "  Total   : $($hud9Files.Count)"
Write-Host "  Hooked  : $($hud9Hooked.Count)"
Write-Host "  Waiting : $($hud9Waiting.Count)"
Write-List "  Hooked files" $hud9Hooked
Write-List "  Waiting files" $hud9Waiting
Write-Host ""

Write-Host "Stage 4/5 support"
Write-Host "  Present : $($stage45Present.Count)"
Write-Host "  Hooked  : $($stage45Hooked.Count)"
Write-Host "  Waiting : $($stage45Waiting.Count)"
Write-List "  Waiting files" $stage45Waiting
Write-Host ""

Write-Host "A Egg presentation"
Write-Host "  Present : $($aEggPresent.Count)"
Write-Host "  Hooked  : $($aEggHooked.Count)"
Write-Host "  Waiting : $($aEggWaiting.Count)"
Write-List "  Waiting files" $aEggWaiting
Write-Host ""

Write-Host "V2 UI assets"
Write-Host "  Total   : $($v2Ui.Count)"
Write-Host "  Hooked  : $($v2Hooked.Count)"
Write-Host "  Waiting : $($v2Waiting.Count)"
Write-List "  Waiting examples" $v2Waiting
Write-Host ""

Write-List "Missing optional static refs" $loadedMissing
Write-Host ""
Write-Host "Suggested next safe tasks"
Write-Host "  1. Visually approve HUD9 wave progress in Unity before expanding HUD9 panels."
Write-Host "  2. Keep HUD9 panel/bar hook-up as a Claude/CoreLanternGame.cs task unless explicitly taking over."
Write-Host "  3. Stage 4/5 support assets are present but not statically loaded yet."
Write-Host "  4. A Egg presentation assets are present; Claude title/result hook-up is waiting."
if ($loadedMissing.Count -gt 0) {
    Write-Host "  5. Resolve missing optional sprite refs if they become visually important."
} else {
    Write-Host "  5. Static sprite refs are all present; no current image fallback warnings."
}
