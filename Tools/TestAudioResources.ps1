param(
    [switch]$Quiet
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$AudioDir = Join-Path $ProjectRoot "Assets\Resources\Audio"
$ManifestPath = Join-Path $AudioDir "AudioManifest.json"
$CoreScriptPath = Join-Path $ProjectRoot "Assets\Scripts\CoreLanternGame.cs"

$errors = New-Object System.Collections.Generic.List[string]
$warnings = New-Object System.Collections.Generic.List[string]

function Add-Error([string]$Message) {
    $script:errors.Add($Message)
}

function Add-Warning([string]$Message) {
    $script:warnings.Add($Message)
}

if (!(Test-Path -LiteralPath $AudioDir)) {
    Add-Error "Audio directory missing: $AudioDir"
}

if (!(Test-Path -LiteralPath $ManifestPath)) {
    Add-Error "AudioManifest.json missing: $ManifestPath"
}

if (!(Test-Path -LiteralPath $CoreScriptPath)) {
    Add-Error "CoreLanternGame.cs missing: $CoreScriptPath"
}

$manifest = @()
if ($errors.Count -eq 0) {
    $manifest = Get-Content -LiteralPath $ManifestPath -Encoding UTF8 | ConvertFrom-Json
}

$manifestIds = @{}
$manifestFiles = @{}
foreach ($entry in $manifest) {
    if ([string]::IsNullOrWhiteSpace($entry.id)) {
        Add-Error "Manifest entry has empty id."
        continue
    }

    if ($manifestIds.ContainsKey($entry.id)) {
        Add-Error "Duplicate manifest id: $($entry.id)"
    } else {
        $manifestIds[$entry.id] = $true
    }

    if ([string]::IsNullOrWhiteSpace($entry.file)) {
        Add-Error "Manifest entry '$($entry.id)' has empty file path."
        continue
    }

    $manifestFiles[$entry.file.Replace("/", "\").ToLowerInvariant()] = $true
    $absoluteFile = Join-Path $ProjectRoot ($entry.file.Replace("/", "\"))
    if (!(Test-Path -LiteralPath $absoluteFile)) {
        Add-Error "Manifest file missing: $($entry.file)"
        continue
    }

    if (!(Test-Path -LiteralPath "$absoluteFile.meta")) {
        Add-Error "Unity .meta missing for manifest file: $($entry.file)"
    }

    if ($entry.file -notmatch "\.wav$") {
        Add-Warning "Manifest entry is not a .wav file: $($entry.file)"
    }
}

if (Test-Path -LiteralPath $CoreScriptPath) {
    $coreText = Get-Content -LiteralPath $CoreScriptPath -Encoding UTF8 -Raw
    $loadMatches = [regex]::Matches($coreText, 'LoadAudioClip\("([^"]+)"\)')
    $loadedNames = New-Object System.Collections.Generic.HashSet[string]
    foreach ($match in $loadMatches) {
        [void]$loadedNames.Add($match.Groups[1].Value)
    }

    foreach ($name in ($loadedNames | Sort-Object)) {
        $isLegacyFallback = $name -eq "BGM_BossPulswyrm" -or $name -eq "BGM_BossNullwyrm"
        $wavPath = Join-Path $AudioDir ($name + ".wav")
        if (!(Test-Path -LiteralPath $wavPath)) {
            if ($isLegacyFallback) {
                Add-Warning "Optional legacy boss BGM fallback not present: $name.wav"
            } else {
                Add-Error "CoreLanternGame.cs loads missing audio file: $name.wav"
            }
            continue
        }

        if (!(Test-Path -LiteralPath "$wavPath.meta")) {
            Add-Error "Unity .meta missing for loaded audio file: $name.wav"
        }

        if (!$manifestIds.ContainsKey($name)) {
            Add-Warning "Loaded audio is missing from AudioManifest.json: $name"
        }
    }
}

if (Test-Path -LiteralPath $AudioDir) {
    foreach ($wav in Get-ChildItem -LiteralPath $AudioDir -Filter "*.wav" -File) {
        $relative = "Assets\Resources\Audio\$($wav.Name)".ToLowerInvariant()
        if (!$manifestFiles.ContainsKey($relative)) {
            Add-Warning "WAV exists but is not listed in AudioManifest.json: $($wav.Name)"
        }
        if (!(Test-Path -LiteralPath "$($wav.FullName).meta")) {
            Add-Error "Unity .meta missing for WAV: $($wav.Name)"
        }
    }
}

if (!$Quiet) {
    Write-Host "Audio resource check"
    Write-Host "Project: $ProjectRoot"
    Write-Host "Manifest entries: $($manifest.Count)"
    Write-Host "Warnings: $($warnings.Count)"
    foreach ($warning in $warnings) {
        Write-Host "WARN: $warning"
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
