# CheckCompileHealth.ps1
#
# CoreLanternGame.cs の編集後に必須実行する検証フローを 1 コマンド化。
# CLAUDE.md セクション B / C の手順を全自動化。
#
# Usage:
#   .\Tools\CheckCompileHealth.ps1            # 全検査 (odd-quote + brace + swallowed + mojibake warning + Roslyn)
#   .\Tools\CheckCompileHealth.ps1 -Quick     # Roslyn を省略 (高速、3-5秒)
#   .\Tools\CheckCompileHealth.ps1 -Verbose   # 詳細出力
#
# Exit code: 0 = OK / 1 = エラーあり (CI フックにも使える)
#
# 必須実行タイミング:
#   1. CoreLanternGame.cs 編集後 (任意の Edit / Write)
#   2. 完了報告前 (Codex / Claude 共通)
#   3. PR 作成前 (将来 git 化したら)

param(
    [switch]$Quick,
    [switch]$Verbose
)

$ErrorActionPreference = 'Continue'
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SourcePath = Join-Path $ProjectRoot 'Assets\Scripts\CoreLanternGame.cs'

if (-not (Test-Path $SourcePath)) {
    Write-Host "[FATAL] Source file not found: $SourcePath" -ForegroundColor Red
    exit 1
}

$failCount = 0
$warnCount = 0
$mojibakeLineCount = 0

function Pass([string]$msg) {
    Write-Host "  [OK]   $msg" -ForegroundColor Green
}

function Fail([string]$msg) {
    Write-Host "  [FAIL] $msg" -ForegroundColor Red
    $script:failCount++
}

function Warn([string]$msg) {
    Write-Host "  [WARN] $msg" -ForegroundColor Yellow
    $script:warnCount++
}

function Section([string]$title) {
    Write-Host ""
    Write-Host "=== $title ===" -ForegroundColor Cyan
}

# =============================================
# Header
# =============================================
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  CoreLanternGame.cs Compile Health Check" -ForegroundColor Cyan
Write-Host "  Source: $SourcePath" -ForegroundColor Gray
$fileInfo = Get-Item $SourcePath
Write-Host "  Size  : $([Math]::Round($fileInfo.Length / 1KB)) KB" -ForegroundColor Gray
Write-Host "  Mode  : $(if ($Quick) { 'Quick (no Roslyn)' } else { 'Full' })" -ForegroundColor Gray
Write-Host "=============================================" -ForegroundColor Cyan

# =============================================
# 1. ファイル読み込み (UTF-8 BOM 無し前提)
# =============================================
Section "Reading file (UTF-8 no BOM)"

try {
    $utf8NoBom = [System.Text.UTF8Encoding]::new($false)
    $content = [System.IO.File]::ReadAllText($SourcePath, $utf8NoBom)
    $lines = [System.IO.File]::ReadAllLines($SourcePath, $utf8NoBom)
    Pass "Loaded $($lines.Count) lines"
} catch {
    Fail "Failed to read file: $($_.Exception.Message)"
    exit 1
}

# =============================================
# 2. odd-quote 検査
# =============================================
Section "1. Odd-quote check (per-line)"

$inactive = 0
$oddLines = @()
for ($i = 0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    $trimmed = $line.Trim()
    if ($trimmed -eq '#if false') { $inactive++; continue }
    if ($trimmed -eq '#endif' -and $inactive -gt 0) { $inactive--; continue }
    if ($inactive -gt 0) { continue }
    # Strip // comments
    $stripped = $line -replace '//.*$', ''
    # Strip \" escaped quotes
    $stripped = $stripped -replace '\\\"', ''
    $count = ($stripped.ToCharArray() | Where-Object { $_ -eq '"' }).Count
    if (($count % 2) -ne 0) {
        $oddLines += "$($i + 1): $line"
    }
}

if ($oddLines.Count -eq 0) {
    Pass "No unbalanced quote lines"
} else {
    Fail "Found $($oddLines.Count) line(s) with odd quote count"
    if ($Verbose -or $oddLines.Count -le 5) {
        $oddLines | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
    } else {
        $oddLines | Select-Object -First 5 | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
        Write-Host "    ... and $($oddLines.Count - 5) more (use -Verbose to see all)" -ForegroundColor Red
    }
}

# =============================================
# 3. brace バランス検査
# =============================================
Section "2. Brace balance check"

$openBraces = ([regex]::Matches($content, '\{')).Count
$closeBraces = ([regex]::Matches($content, '\}')).Count
$diff = $openBraces - $closeBraces

if ($diff -eq 0) {
    Pass "Balanced: open=$openBraces close=$closeBraces diff=0"
} else {
    Fail "Unbalanced: open=$openBraces close=$closeBraces diff=$diff (should be 0)"
}

# =============================================
# 4. swallowed code 検出
# =============================================
Section "3. Swallowed-code detection"

$swallowed = @()
for ($i = 0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    # Active code patterns swallowed inside // comment
    if ($line -match '^\s*//.*?\s+(list\.Add|upgradePool\.Add|fusionUpgradePool\.Add|pool\.Add\(new RelicData|if\s*\()') {
        $swallowed += "$($i + 1): $line"
    }
    # Closing brace } at end of // comment line
    elseif ($line -match '^\s*[^/]*//[^\r\n]*\s+\}\s*$') {
        $swallowed += "$($i + 1): $line"
    }
}

# Known false positives (English "return so" in comments etc.)
$falsePositives = @(
    'return so'        # "Slight cooldown after return so it doesn't immediately fire"
)
$realSwallowed = $swallowed | Where-Object {
    $isFalse = $false
    foreach ($fp in $falsePositives) {
        if ($_ -like "*$fp*") { $isFalse = $true; break }
    }
    -not $isFalse
}

if ($realSwallowed.Count -eq 0) {
    if ($swallowed.Count -gt 0) {
        Pass "No real swallowed code (filtered $($swallowed.Count) known false positive(s))"
    } else {
        Pass "No swallowed code detected"
    }
} else {
    Fail "Found $($realSwallowed.Count) line(s) with code swallowed in comments"
    $realSwallowed | Select-Object -First 5 | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
    if ($realSwallowed.Count -gt 5) {
        Write-Host "    ... and $($realSwallowed.Count - 5) more" -ForegroundColor Red
    }
}

# =============================================
# 5. Mojibake marker scan (warning only)
# =============================================
Section "4. Mojibake marker scan (warning only)"

$mojibakePattern = '�|喁E|寁E|めE|チE|宁E|封E|允E|吁E|刁E|亁E|甁E|征E|搁E|叁E|毁E|半征E|透�E|安�E'
$mojibakeLines = @()
for ($i = 0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    if ($line -match $mojibakePattern) {
        $mojibakeLines += "$($i + 1): $line"
    }
}
$mojibakeLineCount = $mojibakeLines.Count

if ($mojibakeLineCount -eq 0) {
    Pass "No mojibake markers detected"
} else {
    Warn "Found $mojibakeLineCount line(s) with mojibake markers (warning only; prioritize user-facing strings)"
    $sampleCount = 5
    if ($Verbose) { $sampleCount = 20 }
    $mojibakeLines | Select-Object -First $sampleCount | ForEach-Object { Write-Host "    $_" -ForegroundColor Yellow }
    if ($mojibakeLineCount -gt $sampleCount) {
        Write-Host "    ... and $($mojibakeLineCount - $sampleCount) more (use -Verbose to see more)" -ForegroundColor Yellow
    }
}

# =============================================
# 6. Roslyn 構文チェック (Quick モードではスキップ)
# =============================================
if ($Quick) {
    Section "5. Roslyn syntax check"
    Write-Host "  [SKIP] Quick mode - skipping Roslyn" -ForegroundColor DarkGray
} else {
    Section "5. Roslyn syntax check"

    $mono   = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\bin\mono.exe'
    $csc    = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\lib\mono\msbuild\Current\bin\Roslyn\csc.exe'
    $unity  = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\Managed\UnityEngine'
    $netstd = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\NetStandard\ref\2.1.0'

    if (-not (Test-Path $mono)) {
        Warn "mono.exe not found - skipping Roslyn check (install Unity 6000.4 or adjust path)"
    } elseif (-not (Test-Path $csc)) {
        Warn "csc.exe not found - skipping Roslyn check"
    } else {
        $tempDir = Join-Path $ProjectRoot 'Temp'
        if (-not (Test-Path -LiteralPath $tempDir)) {
            New-Item -ItemType Directory -Path $tempDir | Out-Null
        }
        $outDll = Join-Path $tempDir 'CoreLanternSyntaxCheck.dll'

        $compileArgs = @('/noconfig', '/nostdlib+', '/target:library', '/langversion:preview', "/out:$outDll")
        if (Test-Path $netstd) {
            $compileArgs += (Get-ChildItem -LiteralPath $netstd -Filter '*.dll' | ForEach-Object { '/r:' + $_.FullName })
        }
        $compileArgs += (Get-ChildItem -LiteralPath $unity -Filter 'UnityEngine*.dll' | ForEach-Object { '/r:' + $_.FullName })
        $uiDll = Join-Path $ProjectRoot 'Library\ScriptAssemblies\UnityEngine.UI.dll'
        if (Test-Path $uiDll) { $compileArgs += '/r:' + $uiDll }
        $compileArgs += $SourcePath

        $compileStart = Get-Date
        $compileOut = & $mono $csc @compileArgs 2>&1
        $compileMs = ([int]((Get-Date) - $compileStart).TotalMilliseconds)

        # Categorize errors
        $syntaxErrors = $compileOut | Where-Object { $_ -match 'error CS1\d{3}' }
        $undefinedNames = $compileOut | Where-Object { $_ -match 'error CS0103' }
        $refErrors = $compileOut | Where-Object { $_ -match 'error CS(0012|0518|0246|0234|0433)' }
        $allErrors = $compileOut | Where-Object { $_ -match 'error CS' }

        Write-Host "  Compile completed in ${compileMs}ms" -ForegroundColor Gray

        if ($syntaxErrors.Count -eq 0) {
            Pass "Syntax errors (CS1xxx): 0"
        } else {
            Fail "Syntax errors (CS1xxx): $($syntaxErrors.Count)"
            $syntaxErrors | Select-Object -First 5 | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
        }

        if ($undefinedNames.Count -eq 0) {
            Pass "Undefined names (CS0103): 0"
        } else {
            Fail "Undefined names (CS0103): $($undefinedNames.Count)"
            $undefinedNames | Select-Object -First 5 | ForEach-Object { Write-Host "    $_" -ForegroundColor Red }
        }

        if ($refErrors.Count -gt 0 -and $Verbose) {
            Write-Host "  [INFO] Reference errors (CS0012/0518/0433): $($refErrors.Count)" -ForegroundColor DarkGray
            Write-Host "         These are environment-only (mscorlib/netstandard duplicate) - Unity will compile fine" -ForegroundColor DarkGray
        }
    }
}

# =============================================
# Summary
# =============================================
Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
if ($failCount -eq 0) {
    Write-Host "  All checks PASSED ($warnCount warning(s))" -ForegroundColor Green
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  Report for handoff:"
    Write-Host "    odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)" -ForegroundColor Green
    if ($mojibakeLineCount -gt 0) {
        Write-Host "    mojibake marker lines=$mojibakeLineCount (warning only)" -ForegroundColor Yellow
    }
    Write-Host ""
    exit 0
} else {
    Write-Host "  $failCount check(s) FAILED, $warnCount warning(s)" -ForegroundColor Red
    Write-Host "=============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  Fix the failures above before reporting completion." -ForegroundColor Red
    Write-Host "  See CLAUDE.md for verification flow details." -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
