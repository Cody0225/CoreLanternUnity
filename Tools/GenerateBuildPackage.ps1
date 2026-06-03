# GenerateBuildPackage.ps1
#
# Unity ビルドを itch.io / Steam 提出用に zip 化するスクリプト。
# Unity 自体のビルドは Editor で手動実行する前提。
# 本スクリプトはビルド後の検証 + zip 化 + ハッシュ生成を自動化。
#
# Usage:
#   .\Tools\GenerateBuildPackage.ps1                          # 既存ビルドを zip 化
#   .\Tools\GenerateBuildPackage.ps1 -Version "0.9.0"         # バージョン指定
#   .\Tools\GenerateBuildPackage.ps1 -Channel "demo"          # itch.io demo 用命名
#   .\Tools\GenerateBuildPackage.ps1 -Channel "steam-ea"      # Steam EA 用命名
#   .\Tools\GenerateBuildPackage.ps1 -DryRun                  # zip しない、検証のみ
#
# 前提:
#   - Unity Editor で File > Build Settings > Build を実行
#   - 出力先: Builds/{Version}/EggcoreProtocol.exe + Data/ フォルダ
#
# Exit code: 0 = OK / 1 = エラー

param(
    [string]$Version = '',
    [string]$Channel = 'demo',
    [switch]$DryRun,
    [string]$BuildDir = '',
    [string]$OutputDir = ''
)

$ErrorActionPreference = 'Continue'
$ProjectRoot = Split-Path -Parent $PSScriptRoot

# ── デフォルト値 ──
if (-not $Version) {
    # ProjectSettings からバージョンを推定 (Unity の ProjectVersion から)
    $projSettings = Join-Path $ProjectRoot 'ProjectSettings\ProjectSettings.asset'
    if (Test-Path $projSettings) {
        $content = Get-Content $projSettings -Raw
        if ($content -match 'bundleVersion:\s*(\S+)') {
            $Version = $Matches[1]
        }
    }
    if (-not $Version) { $Version = '0.9.0' }
}

if (-not $BuildDir) {
    $BuildDir = Join-Path $ProjectRoot "Builds\$Version"
}

if (-not $OutputDir) {
    $OutputDir = Join-Path $ProjectRoot 'Builds\_Packages'
}

# ── ヘッダ表示 ──
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  Build Package Generator" -ForegroundColor Cyan
Write-Host "  Version : $Version" -ForegroundColor Gray
Write-Host "  Channel : $Channel" -ForegroundColor Gray
Write-Host "  Build   : $BuildDir" -ForegroundColor Gray
Write-Host "  Output  : $OutputDir" -ForegroundColor Gray
if ($DryRun) {
    Write-Host "  Mode    : DRY-RUN (zip skipped)" -ForegroundColor Yellow
}
Write-Host "=============================================" -ForegroundColor Cyan

$failCount = 0

function Pass([string]$msg) { Write-Host "  [OK]   $msg" -ForegroundColor Green }
function Fail([string]$msg) { Write-Host "  [FAIL] $msg" -ForegroundColor Red; $script:failCount++ }
function Warn([string]$msg) { Write-Host "  [WARN] $msg" -ForegroundColor Yellow }
function Section([string]$title) { Write-Host ""; Write-Host "=== $title ===" -ForegroundColor Cyan }

# ── Step 1: ビルドフォルダ存在確認 ──
Section "1. Build directory check"

if (-not (Test-Path $BuildDir)) {
    Fail "Build directory not found: $BuildDir"
    Write-Host ""
    Write-Host "  Did you run Unity > File > Build Settings > Build?" -ForegroundColor Yellow
    Write-Host "  Expected output path: $BuildDir" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
Pass "Build directory exists"

# ── Step 2: 必須ファイル確認 ──
Section "2. Required files check"

$requiredFiles = @(
    'EggcoreProtocol.exe',
    'UnityPlayer.dll'
)
$requiredDirs = @(
    'EggcoreProtocol_Data',
    'MonoBleedingEdge'
)

foreach ($f in $requiredFiles) {
    $p = Join-Path $BuildDir $f
    if (Test-Path $p) {
        $size = (Get-Item $p).Length
        Pass "$f ($([Math]::Round($size/1KB)) KB)"
    } else {
        Fail "Missing required file: $f"
    }
}

foreach ($d in $requiredDirs) {
    $p = Join-Path $BuildDir $d
    if (Test-Path $p) {
        $count = (Get-ChildItem $p -Recurse -File).Count
        Pass "$d/ ($count files)"
    } else {
        Fail "Missing required directory: $d/"
    }
}

if ($failCount -gt 0) {
    Write-Host ""
    Write-Host "  Build is incomplete. Re-run Unity build." -ForegroundColor Red
    exit 1
}

# ── Step 3: ビルドサイズ計算 ──
Section "3. Build size calculation"

$totalSize = (Get-ChildItem $BuildDir -Recurse -File | Measure-Object Length -Sum).Sum
$totalSizeMB = [Math]::Round($totalSize / 1MB, 1)
$totalFiles = (Get-ChildItem $BuildDir -Recurse -File).Count

Pass "Total files: $totalFiles"
Pass "Total size : ${totalSizeMB} MB"

if ($totalSizeMB -gt 1000) {
    Warn "Build is over 1 GB. Check itch.io upload limit (1 GB on free plan)"
} elseif ($totalSizeMB -gt 500) {
    Warn "Build is over 500 MB. Long upload time expected."
}

# ── Step 4: 不要ファイル検出 ──
Section "4. Unwanted file check"

$unwantedPatterns = @(
    '*.pdb',
    '*.mdb',
    '*.bak',
    '*.bak_*',
    'Thumbs.db',
    'Desktop.ini',
    '.DS_Store'
)

$unwanted = @()
foreach ($pattern in $unwantedPatterns) {
    $found = Get-ChildItem $BuildDir -Recurse -File -Filter $pattern -ErrorAction SilentlyContinue
    $unwanted += $found
}

if ($unwanted.Count -eq 0) {
    Pass "No unwanted files"
} else {
    Warn "Found $($unwanted.Count) unwanted file(s)"
    $unwanted | Select-Object -First 5 | ForEach-Object {
        Write-Host "    $($_.FullName.Replace($BuildDir, ''))" -ForegroundColor Yellow
    }
    if ($unwanted.Count -gt 5) {
        Write-Host "    ... and $($unwanted.Count - 5) more" -ForegroundColor Yellow
    }
    Write-Host "  Consider cleaning before packaging." -ForegroundColor Yellow
}

# ── Step 5: zip 化 ──
Section "5. Package as zip"

if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
    Pass "Created output directory: $OutputDir"
}

# 命名規則: EggcoreProtocol_{channel}_v{version}_Windows.zip
$zipName = "EggcoreProtocol_${Channel}_v${Version}_Windows.zip"
$zipPath = Join-Path $OutputDir $zipName

if ($DryRun) {
    Write-Host "  [DRY-RUN] Would create: $zipPath" -ForegroundColor Yellow
} else {
    if (Test-Path $zipPath) {
        Warn "Zip already exists, overwriting: $zipName"
        Remove-Item $zipPath -Force
    }
    Write-Host "  Compressing... (this may take a few minutes)" -ForegroundColor Gray
    $zipStart = Get-Date

    try {
        Add-Type -AssemblyName 'System.IO.Compression.FileSystem'
        # Compress: src dir → zip, with build folder as root inside zip
        # Create at parent level so zip contains all files at root (not nested)
        [System.IO.Compression.ZipFile]::CreateFromDirectory(
            $BuildDir,
            $zipPath,
            [System.IO.Compression.CompressionLevel]::Optimal,
            $false  # Don't include base directory (files at zip root)
        )
        $zipMs = ([int]((Get-Date) - $zipStart).TotalMilliseconds)
        $zipSize = (Get-Item $zipPath).Length
        $zipSizeMB = [Math]::Round($zipSize / 1MB, 1)
        $compressionRatio = [Math]::Round((1 - ($zipSize / $totalSize)) * 100, 1)
        Pass "Created: $zipName (${zipSizeMB} MB, ${compressionRatio}% compression, ${zipMs}ms)"
    } catch {
        Fail "Zip creation failed: $($_.Exception.Message)"
        exit 1
    }
}

# ── Step 6: ハッシュ生成 (verification 用) ──
Section "6. Hash generation"

if ($DryRun) {
    Write-Host "  [DRY-RUN] Skipping hash generation" -ForegroundColor Yellow
} elseif (Test-Path $zipPath) {
    $hashStart = Get-Date
    $sha256 = (Get-FileHash $zipPath -Algorithm SHA256).Hash
    $md5 = (Get-FileHash $zipPath -Algorithm MD5).Hash
    $hashMs = ([int]((Get-Date) - $hashStart).TotalMilliseconds)
    Pass "SHA256: $sha256 (${hashMs}ms)"
    Pass "MD5   : $md5"

    # ハッシュをテキストファイルにも保存 (リリース時の改ざん検証用)
    $hashFile = $zipPath + '.sha256.txt'
    "$sha256  $zipName" | Out-File $hashFile -Encoding ascii -NoNewline
    Pass "Hash saved: $hashFile"
}

# ── Step 7: アップロード前チェックリスト出力 ──
Section "7. Pre-upload checklist"

Write-Host ""
Write-Host "  Before uploading, verify:" -ForegroundColor Cyan
Write-Host "    [ ] Played the .exe yourself (1 run through)" -ForegroundColor Gray
Write-Host "    [ ] No errors in Unity Console during play" -ForegroundColor Gray
Write-Host "    [ ] All audio plays correctly" -ForegroundColor Gray
Write-Host "    [ ] All character images load (Cobalt Pup at minimum)" -ForegroundColor Gray
Write-Host "    [ ] Version number matches Steam page / itch.io page" -ForegroundColor Gray

if ($Channel -eq 'demo') {
    Write-Host ""
    Write-Host "  For itch.io demo:" -ForegroundColor Cyan
    Write-Host "    1. Login to itch.io" -ForegroundColor Gray
    Write-Host "    2. Go to Project > Edit > Uploads" -ForegroundColor Gray
    Write-Host "    3. Drag $zipName" -ForegroundColor Gray
    Write-Host "    4. Set as primary file, Windows platform tag ON" -ForegroundColor Gray
    Write-Host "    5. Save and test download" -ForegroundColor Gray
} elseif ($Channel -like 'steam*') {
    Write-Host ""
    Write-Host "  For Steam upload:" -ForegroundColor Cyan
    Write-Host "    1. Copy build folder to C:\SteamworksContent\content\" -ForegroundColor Gray
    Write-Host "    2. Run: steamcmd +login USER +run_app_build scripts\app_build_XXXXX.vdf +quit" -ForegroundColor Gray
    Write-Host "    3. Verify in Steamworks Portal > Builds" -ForegroundColor Gray
    Write-Host "    Note: Steam does NOT use this zip directly. The zip is for distribution outside Steam." -ForegroundColor Yellow
}

# ── Summary ──
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
if ($failCount -eq 0) {
    Write-Host "  Package generation: SUCCESS" -ForegroundColor Green
    if (-not $DryRun) {
        Write-Host "  Output: $zipPath" -ForegroundColor Green
    }
} else {
    Write-Host "  Package generation: FAILED ($failCount error(s))" -ForegroundColor Red
}
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

if ($failCount -gt 0) { exit 1 } else { exit 0 }
