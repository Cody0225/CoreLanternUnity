param(
    [switch]$Quiet,
    [int]$MaxWarningsToShow = 40
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$CoreScriptPath = Join-Path $ProjectRoot "Assets\Scripts\CoreLanternGame.cs"
$BacklogPath = Join-Path $ProjectRoot "IMAGE_ASSET_BACKLOG.md"

$errors = New-Object System.Collections.Generic.List[string]
$warnings = New-Object System.Collections.Generic.List[string]

function Add-Error([string]$Message) {
    $script:errors.Add($Message)
}

function Add-Warning([string]$Message) {
    $script:warnings.Add($Message)
}

function Get-SkinPathLabel([string]$FileName) {
    return "Assets/Resources/Skins/$FileName"
}

function Get-NormalizedLeaf([string]$PathLike) {
    $normalized = $PathLike.Replace([char]92, [char]47)
    return Split-Path -Leaf $normalized
}

if (!(Test-Path -LiteralPath $SkinDir)) {
    Add-Error "Skins directory missing: $SkinDir"
}

if (!(Test-Path -LiteralPath $CoreScriptPath)) {
    Add-Error "CoreLanternGame.cs missing: $CoreScriptPath"
}

if (!(Test-Path -LiteralPath $BacklogPath)) {
    Add-Warning "IMAGE_ASSET_BACKLOG.md missing: $BacklogPath"
}

$skinFiles = @()
$skinFileNames = New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)
if (Test-Path -LiteralPath $SkinDir) {
    $skinFiles = @(Get-ChildItem -LiteralPath $SkinDir -Filter "*.png" -File)
    foreach ($png in $skinFiles) {
        [void]$skinFileNames.Add($png.Name)

        if ($png.Length -le 0) {
            Add-Error "PNG has zero bytes: $(Get-SkinPathLabel $png.Name)"
        }

        if (!(Test-Path -LiteralPath "$($png.FullName).meta")) {
            Add-Error "Unity .meta missing for PNG: $(Get-SkinPathLabel $png.Name)"
        }
    }
}

$loadedSpriteNames = New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)
if (Test-Path -LiteralPath $CoreScriptPath) {
    $coreText = Get-Content -LiteralPath $CoreScriptPath -Encoding UTF8 -Raw
    $quote = [regex]::Escape([string][char]34)
    $loadPattern = "LoadOptionalSprite\(" + $quote + "Skins/([^" + $quote + "]+)" + $quote
    $resourcePattern = "Resources[.]Load<Sprite>\(" + $quote + "Skins/([^" + $quote + "]+)" + $quote

    foreach ($match in [regex]::Matches($coreText, $loadPattern)) {
        $name = $match.Groups[1].Value
        if ($name.EndsWith("_", [System.StringComparison]::OrdinalIgnoreCase)) {
            continue
        }
        if (!$name.EndsWith(".png", [System.StringComparison]::OrdinalIgnoreCase)) {
            $name = "$name.png"
        }
        [void]$loadedSpriteNames.Add($name)
    }

    foreach ($match in [regex]::Matches($coreText, $resourcePattern)) {
        $name = $match.Groups[1].Value
        if ($name.EndsWith("_", [System.StringComparison]::OrdinalIgnoreCase)) {
            continue
        }
        if (!$name.EndsWith(".png", [System.StringComparison]::OrdinalIgnoreCase)) {
            $name = "$name.png"
        }
        [void]$loadedSpriteNames.Add($name)
    }

    foreach ($name in ($loadedSpriteNames | Sort-Object)) {
        if (!$skinFileNames.Contains($name)) {
            Add-Warning "Code optional sprite is not present and will use procedural fallback: $(Get-SkinPathLabel $name)"
            continue
        }

        $fullPath = Join-Path $SkinDir $name
        if (!(Test-Path -LiteralPath "$fullPath.meta")) {
            Add-Error "Unity .meta missing for loaded sprite: $(Get-SkinPathLabel $name)"
        }
    }
}

$backlogPngNames = New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)
$doneBacklogNames = New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)
if (Test-Path -LiteralPath $BacklogPath) {
    $backlogLines = @(Get-Content -LiteralPath $BacklogPath -Encoding UTF8)
    $backlogPattern = "\x60([^\x60]+[.]png)\x60"

    foreach ($line in $backlogLines) {
        foreach ($match in [regex]::Matches($line, $backlogPattern)) {
            $fileName = Get-NormalizedLeaf $match.Groups[1].Value
            if ([string]::IsNullOrWhiteSpace($fileName)) {
                continue
            }

            [void]$backlogPngNames.Add($fileName)
            if ($line -match "\|\s*DONE\s*\|" -or $line -match "DONE") {
                [void]$doneBacklogNames.Add($fileName)
            }
        }
    }

    foreach ($name in ($doneBacklogNames | Sort-Object)) {
        if (!$skinFileNames.Contains($name)) {
            Add-Warning "Backlog marks DONE but file is not in Skins: $(Get-SkinPathLabel $name)"
        }
    }
}

$loadedMissing = 0
foreach ($name in $loadedSpriteNames) {
    if (!$skinFileNames.Contains($name)) {
        $loadedMissing++
    }
}

$doneBacklogMissing = 0
foreach ($name in $doneBacklogNames) {
    if (!$skinFileNames.Contains($name)) {
        $doneBacklogMissing++
    }
}

if (!$Quiet) {
    Write-Host "Image resource check"
    Write-Host "Project: $ProjectRoot"
    Write-Host "Skins PNG files: $($skinFiles.Count)"
    Write-Host "Code sprite references: $($loadedSpriteNames.Count)"
    Write-Host "Backlog PNG references: $($backlogPngNames.Count)"
    Write-Host "Backlog DONE PNG references: $($doneBacklogNames.Count)"
    Write-Host "Missing optional code sprites: $loadedMissing"
    Write-Host "Missing DONE backlog sprites: $doneBacklogMissing"
    Write-Host "Warnings: $($warnings.Count)"

    $shown = 0
    foreach ($warning in $warnings) {
        if ($shown -ge $MaxWarningsToShow) {
            $remaining = $warnings.Count - $shown
            if ($remaining -gt 0) {
                Write-Host "WARN: ... $remaining more warnings omitted. Re-run with -MaxWarningsToShow 999 to list all."
            }
            break
        }

        Write-Host "WARN: $warning"
        $shown++
    }

    Write-Host "Errors: $($errors.Count)"
    foreach ($errorItem in $errors) {
        Write-Host "ERROR: $errorItem"
    }
}

if ($errors.Count -gt 0) {
    exit 1
}

exit 0
