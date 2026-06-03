# UploadToItch.ps1
#
# itch.io へ butler CLI でビルドをアップロードするラッパー。
# 事前に GenerateBuildPackage.ps1 でビルドを zip 化しておく。
#
# Usage:
#   .\Tools\UploadToItch.ps1                                     # 最新 zip を自動検出してアップロード
#   .\Tools\UploadToItch.ps1 -Version "0.9.0"                   # バージョン指定
#   .\Tools\UploadToItch.ps1 -Channel "demo"                     # チャンネル指定 (デフォルト: demo)
#   .\Tools\UploadToItch.ps1 -DryRun                             # アップロードせず確認のみ
#   .\Tools\UploadToItch.ps1 -CheckSetup                        # butler セットアップ確認だけ
#
# 前提:
#   - butler がインストールされていること (https://itch.io/docs/butler/)
#   - `butler login` で認証済みであること
#   - itch.io ゲームページが作成済みであること
#
# Exit code: 0 = OK / 1 = エラー

param(
    [string]$Version = '',
    [string]$Channel = 'demo',
    [string]$ItchUser = 'embercore-studio',
    [string]$ItchGame = 'eggcore-protocol',
    [string]$ZipPath = '',
    [switch]$DryRun,
    [switch]$CheckSetup
)

$ErrorActionPreference = 'Continue'
$ProjectRoot = Split-Path -Parent $PSScriptRoot

# ── ヘルパー ──
$failCount = 0
function Pass([string]$msg)    { Write-Host "  [OK]   $msg" -ForegroundColor Green }
function Fail([string]$msg)    { Write-Host "  [FAIL] $msg" -ForegroundColor Red; $script:failCount++ }
function Warn([string]$msg)    { Write-Host "  [WARN] $msg" -ForegroundColor Yellow }
function Info([string]$msg)    { Write-Host "  [INFO] $msg" -ForegroundColor Cyan }
function Section([string]$title) { Write-Host ""; Write-Host "=== $title ===" -ForegroundColor Cyan }

# ── ヘッダ ──
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  Eggcore Protocol - itch.io Uploader" -ForegroundColor Cyan
Write-Host "  User   : $ItchUser" -ForegroundColor Gray
Write-Host "  Game   : $ItchGame" -ForegroundColor Gray
Write-Host "  Channel: $Channel" -ForegroundColor Gray
if ($DryRun)     { Write-Host "  Mode   : DRY-RUN" -ForegroundColor Yellow }
if ($CheckSetup) { Write-Host "  Mode   : SETUP CHECK" -ForegroundColor Yellow }
Write-Host "=============================================" -ForegroundColor Cyan

# ── Step 1: butler インストール確認 ──
Section "1. butler installation check"

# butler の一般的なインストール先を探す
$butlerCmd = Get-Command 'butler' -ErrorAction SilentlyContinue
$butlerCandidates = @(
    $(if ($butlerCmd) { $butlerCmd.Source } else { $null }),
    "$env:USERPROFILE\.itch\bin\butler.exe",
    "$env:LOCALAPPDATA\itch\butler.exe",
    "$env:PROGRAMFILES\butler\butler.exe",
    "C:\butler\butler.exe"
)
$butlerPath = $null
foreach ($candidate in $butlerCandidates) {
    if ($candidate -and (Test-Path $candidate -ErrorAction SilentlyContinue)) {
        $butlerPath = $candidate; break
    }
}

if (-not $butlerPath) {
    if ($DryRun) {
        Warn "butler not found - DryRun will proceed without it."
        $butlerPath = "butler"  # placeholder for dry-run display
    } else {
        Fail "butler not found."
        Write-Host ""
        Write-Host "  Install butler:" -ForegroundColor Yellow
        Write-Host "  1. Download from https://itch.io/docs/butler/installing.html" -ForegroundColor Gray
        Write-Host "  2. Or: scoop install butler  (if Scoop is available)" -ForegroundColor Gray
        Write-Host "  3. Or: winget install Leafo.butler  (if winget is available)" -ForegroundColor Gray
        Write-Host "  4. After install, run: butler login" -ForegroundColor Gray
        Write-Host ""
        exit 1
    }
}

Pass "butler found: $butlerPath"

# butler バージョン確認 (DryRun で butler が placeholder の場合はスキップ)
if ($butlerPath -ne "butler" -or -not $DryRun) {
    $butlerVersion = & $butlerPath version 2>&1 | Select-Object -First 1
    Pass "butler version: $butlerVersion"
} else {
    Info "butler version check skipped (DryRun, butler not installed)"
}

if ($CheckSetup) {
    # ── ログイン状態確認 ──
    Section "2. butler login status"
    $statusOut = & $butlerPath status $ItchUser/$ItchGame 2>&1
    if ($LASTEXITCODE -eq 0) {
        Pass "Logged in and game accessible: $ItchUser/$ItchGame"
        $statusOut | Select-Object -First 5 | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }
    } else {
        Warn "Could not verify login. Run: butler login"
        $statusOut | Select-Object -First 3 | ForEach-Object { Write-Host "    $_" -ForegroundColor Yellow }
    }

    Write-Host ""
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host "  Setup check complete. Run without -CheckSetup to upload." -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host ""
    exit 0
}

# ── Step 2: zip ファイル解決 ──
Section "2. Zip file resolution"

$packageDir = Join-Path $ProjectRoot 'Builds\_Packages'

if ($ZipPath -and (Test-Path $ZipPath)) {
    Pass "Using specified zip: $ZipPath"
} elseif ($Version) {
    $ZipPath = Join-Path $packageDir "EggcoreProtocol_${Channel}_v${Version}_Windows.zip"
    if (Test-Path $ZipPath) {
        Pass "Found zip for v${Version}: $(Split-Path -Leaf $ZipPath)"
    } else {
        Fail "Zip not found for v${Version}: $ZipPath"
        Write-Host "  Run: .\Tools\GenerateBuildPackage.ps1 -Version '$Version' -Channel '$Channel'" -ForegroundColor Yellow
        exit 1
    }
} else {
    # 最新 zip を自動検出
    if (-not (Test-Path $packageDir)) {
        Fail "Package directory not found: $packageDir"
        Write-Host "  Run GenerateBuildPackage.ps1 first." -ForegroundColor Yellow
        exit 1
    }
    $latestZip = Get-ChildItem $packageDir -Filter "EggcoreProtocol_${Channel}_*.zip" -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $latestZip) {
        Fail "No zip found in: $packageDir"
        Write-Host "  Run: .\Tools\GenerateBuildPackage.ps1 -Channel '$Channel'" -ForegroundColor Yellow
        exit 1
    }
    $ZipPath = $latestZip.FullName
    Pass "Auto-detected latest zip: $($latestZip.Name)"
}

$zipFile = Get-Item $ZipPath
$zipSizeMB = [Math]::Round($zipFile.Length / 1MB, 1)
Pass "Zip size: ${zipSizeMB} MB"

# ── Step 3: SHA256 整合性確認 ──
Section "3. Checksum verification"

$hashFile = $ZipPath + '.sha256.txt'
if (Test-Path $hashFile) {
    $storedLine = Get-Content $hashFile -Raw -Encoding ascii
    $storedHash = ($storedLine -split '\s+')[0]
    $currentHash = (Get-FileHash $ZipPath -Algorithm SHA256).Hash
    if ($storedHash -eq $currentHash) {
        Pass "SHA256 match: $($storedHash.Substring(0,16))..."
    } else {
        Fail "SHA256 mismatch! Zip may be corrupted."
        Write-Host "  Stored : $storedHash" -ForegroundColor Red
        Write-Host "  Current: $currentHash" -ForegroundColor Red
        exit 1
    }
} else {
    Warn "No .sha256.txt found - skipping checksum verification"
    Warn "Regenerate with: .\Tools\GenerateBuildPackage.ps1"
}

# ── Step 4: アップロード前チェックリスト ──
Section "4. Pre-upload verification"

Write-Host ""
Write-Host "  Upload target:" -ForegroundColor Cyan
Write-Host "    itch.io game : https://itch.io/game/$ItchGame" -ForegroundColor Gray
Write-Host "    channel      : $Channel" -ForegroundColor Gray
Write-Host "    zip          : $(Split-Path -Leaf $ZipPath)" -ForegroundColor Gray
Write-Host "    size         : ${zipSizeMB} MB" -ForegroundColor Gray
Write-Host ""
Write-Host "  Pre-upload checklist (confirm manually):" -ForegroundColor Cyan
Write-Host "    [ ] .exe plays without errors on a clean machine" -ForegroundColor Gray
Write-Host "    [ ] Version matches itch.io page description" -ForegroundColor Gray
Write-Host "    [ ] itch.io game page is set to 'Restrict to registered users' or 'Public'" -ForegroundColor Gray

if ($DryRun) {
    Write-Host ""
    Write-Host "  [DRY-RUN] Skipping actual upload." -ForegroundColor Yellow
    Write-Host "  Command that would run:" -ForegroundColor Yellow
    Write-Host "    $butlerPath push '$ZipPath' $ItchUser/${ItchGame}:$Channel" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host "  Dry-run complete - no upload performed." -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host ""
    exit 0
}

# ── Step 5: アップロード ──
Section "5. Upload to itch.io via butler"

$target = "$ItchUser/${ItchGame}:$Channel"
Write-Host "  Uploading to: $target" -ForegroundColor Gray
Write-Host "  (This may take several minutes depending on file size and network speed)" -ForegroundColor Gray
Write-Host ""

$uploadStart = Get-Date
$uploadOutput = & $butlerPath push $ZipPath $target --userversion-file $hashFile 2>&1
$uploadExitCode = $LASTEXITCODE
$uploadMs = [int]((Get-Date) - $uploadStart).TotalMilliseconds
$uploadSec = [Math]::Round($uploadMs / 1000, 1)

$uploadOutput | ForEach-Object { Write-Host "    $_" -ForegroundColor Gray }

if ($uploadExitCode -eq 0) {
    Pass "Upload completed in ${uploadSec}s"
} else {
    Fail "Upload failed (exit code $uploadExitCode)"
    Write-Host ""
    Write-Host "  Common causes:" -ForegroundColor Yellow
    Write-Host "    - Not logged in: run 'butler login'" -ForegroundColor Yellow
    Write-Host "    - Wrong game URL: check -ItchUser and -ItchGame params" -ForegroundColor Yellow
    Write-Host "    - Network timeout: retry" -ForegroundColor Yellow
    exit 1
}

# ── Step 6: アップロード後の確認 ──
Section "6. Post-upload checklist"

Write-Host ""
Write-Host "  After upload, verify on itch.io:" -ForegroundColor Cyan
Write-Host "    1. Login at https://itch.io" -ForegroundColor Gray
Write-Host "    2. Go to Dashboard > $ItchGame > Edit > Uploads" -ForegroundColor Gray
Write-Host "    3. Confirm the new build appears with the correct channel tag" -ForegroundColor Gray
Write-Host "    4. Set platform: Windows (if not auto-detected)" -ForegroundColor Gray
Write-Host "    5. Download and test on a clean machine" -ForegroundColor Gray
Write-Host ""
Write-Host "  Game page: https://$ItchUser.itch.io/$ItchGame" -ForegroundColor Cyan

# ── Summary ──
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
if ($failCount -eq 0) {
    Write-Host "  Upload: SUCCESS" -ForegroundColor Green
    Write-Host "  Target: $target" -ForegroundColor Green
    Write-Host "  Time  : ${uploadSec}s" -ForegroundColor Green
} else {
    Write-Host "  Upload: FAILED ($failCount error(s))" -ForegroundColor Red
}
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

if ($failCount -gt 0) { exit 1 } else { exit 0 }
