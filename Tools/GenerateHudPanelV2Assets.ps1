param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\HUD_Panel_V2_20260529"
New-Item -ItemType Directory -Force -Path $SkinDir, $ArtDir | Out-Null

function Ensure-TextureMeta {
    param([string]$AssetPath)
    $metaPath = $AssetPath + ".meta"
    if ((Test-Path -LiteralPath $metaPath) -and (-not $Force)) { return }

    $templatePath = Join-Path $SkinDir "HUD_Bar_Back_v2.png.meta"
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

function New-Color {
    param([int]$R, [int]$G, [int]$B, [int]$A = 255)
    return [System.Drawing.Color]::FromArgb($A, $R, $G, $B)
}

function With-Alpha {
    param([System.Drawing.Color]$Color, [int]$A)
    return [System.Drawing.Color]::FromArgb($A, $Color.R, $Color.G, $Color.B)
}

function New-Canvas {
    param([int]$W, [int]$H)
    $bmp = [System.Drawing.Bitmap]::new($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $bmp.SetResolution(96, 96)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    return @{ Bitmap = $bmp; Graphics = $g }
}

function New-Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 2)
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    return $pen
}

function New-RoundedRectPath {
    param([float]$X, [float]$Y, [float]$W, [float]$H, [float]$R)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $d = $R * 2
    $path.AddArc($X, $Y, $d, $d, 180, 90)
    $path.AddArc($X + $W - $d, $Y, $d, $d, 270, 90)
    $path.AddArc($X + $W - $d, $Y + $H - $d, $d, $d, 0, 90)
    $path.AddArc($X, $Y + $H - $d, $d, $d, 90, 90)
    $path.CloseFigure()
    return $path
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4, 2.5) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(4, [int]($Color.A / ($mul + 2.0))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-Panel {
    param(
        [string]$Name,
        [int]$W,
        [int]$H,
        [System.Drawing.Color]$Accent,
        [System.Drawing.Color]$Secondary
    )

    $canvas = New-Canvas $W $H
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 1 ($W - 2) ($H - 2) 5
    $fill = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(214, 7, 22, 28))
    $g.FillPath($fill, $path)
    $fill.Dispose()

    $inner = New-RoundedRectPath 5 5 ($W - 10) ($H - 10) 4
    $wash = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(24, $Accent.R, $Accent.G, $Accent.B))
    $g.FillPath($wash, $inner)
    $wash.Dispose()

    $outline = New-Pen (With-Alpha $Accent 125) 1.4
    $g.DrawPath($outline, $path)
    $outline.Dispose()
    $path.Dispose()
    $inner.Dispose()

    $m = 10
    $topLen = [Math]::Min([int]($W * 0.48), 150)
    $bottomLen = [Math]::Min([int]($W * 0.34), 112)
    $sideLen = [Math]::Min([int]($H * 0.42), 68)
    Draw-GlowLine $g $m $m ($m + $topLen) $m (With-Alpha $Accent 205) 1.8
    Draw-GlowLine $g $m $m $m ($m + $sideLen) (With-Alpha $Secondary 155) 1.5
    Draw-GlowLine $g ($W - $m) $m ($W - $m - [Math]::Min(52, $topLen)) $m (With-Alpha $Accent 150) 1.5
    Draw-GlowLine $g ($W - $m) $m ($W - $m) ($m + [Math]::Min(34, $sideLen)) (With-Alpha $Accent 130) 1.4
    Draw-GlowLine $g $m ($H - $m) ($m + [Math]::Min(42, $bottomLen)) ($H - $m) (With-Alpha $Secondary 150) 1.5
    Draw-GlowLine $g $m ($H - $m) $m ($H - $m - [Math]::Min(28, $sideLen)) (With-Alpha $Secondary 120) 1.3
    Draw-GlowLine $g ($W - $m) ($H - $m) ($W - $m - $bottomLen) ($H - $m) (With-Alpha $Accent 125) 1.5

    $scan = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(18, 139, 200, 255))
    for ($y = 14; $y -lt $H - 10; $y += 18) {
        $g.FillRectangle($scan, 14, $y, $W - 28, 1)
    }
    $scan.Dispose()

    $pathOut = Join-Path $SkinDir $Name
    if ((Test-Path -LiteralPath $pathOut) -and (-not $Force)) {
        Write-Host "Skip existing $Name"
    } else {
        $canvas.Bitmap.Save($pathOut, [System.Drawing.Imaging.ImageFormat]::Png)
        Ensure-TextureMeta $pathOut
        Write-Host "Generated $Name"
    }

    $artPath = Join-Path $ArtDir $Name
    $canvas.Bitmap.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $artPath
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

function New-Preview {
    $previewPath = Join-Path $ArtDir "HUD_Panel_V2_Preview.png"
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 8, 15, 18))
    $g.FillRectangle($bg, 0, 0, 1280, 720)
    $bg.Dispose()
    $font = [System.Drawing.Font]::new("Arial", 20, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 14, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(245, 184, 224, 255))
    $muted = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(180, 139, 200, 255))
    $g.DrawString("HUD Panel V2 exact-size draft", $font, $brush, 34, 26)
    $g.DrawString("Generated at current UI rect sizes. Do not stretch; use preserve/native size.", $small, $muted, 34, 56)

    $items = @(
        @{File="HUD_Panel_Wave_v2.png"; X=42; Y=96},
        @{File="HUD_Panel_HP_v2.png"; X=42; Y=190},
        @{File="HUD_Panel_Level_v2.png"; X=42; Y=318},
        @{File="HUD_Panel_Data_v2.png"; X=42; Y=414},
        @{File="HUD_Panel_ChipMini_v2.png"; X=330; Y=96},
        @{File="HUD_Panel_Loadout_v2.png"; X=330; Y=158},
        @{File="HUD_Panel_Stats_v2.png"; X=330; Y=272},
        @{File="HUD_Panel_Links_v2.png"; X=640; Y=96},
        @{File="HUD_Panel_EventLog_v2.png"; X=640; Y=352}
    )

    foreach ($item in $items) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir $item.File))
        $g.DrawImage($img, $item.X, $item.Y, $img.Width, $img.Height)
        $g.DrawString(($item.File -replace "HUD_Panel_", "" -replace "_v2.png", ""), $small, $muted, $item.X, $item.Y + $img.Height + 6)
        $img.Dispose()
    }

    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
    $font.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()
    Write-Host "Generated HUD_Panel_V2_Preview.png"
}

$cyan = New-Color 139 200 255 205
$gold = New-Color 255 206 59 170
$green = New-Color 91 200 94 175
$violet = New-Color 165 91 255 175

Draw-Panel "HUD_Panel_Wave_v2.png" 250 74 $cyan $gold
Draw-Panel "HUD_Panel_HP_v2.png" 270 104 $cyan $gold
Draw-Panel "HUD_Panel_Level_v2.png" 270 76 $green $cyan
Draw-Panel "HUD_Panel_ChipMini_v2.png" 190 42 $cyan $green
Draw-Panel "HUD_Panel_Data_v2.png" 250 54 $cyan $green
Draw-Panel "HUD_Panel_Loadout_v2.png" 296 92 $gold $cyan
Draw-Panel "HUD_Panel_Stats_v2.png" 286 156 $cyan $gold
Draw-Panel "HUD_Panel_Links_v2.png" 360 228 $cyan $violet
Draw-Panel "HUD_Panel_EventLog_v2.png" 520 70 $violet $cyan
New-Preview

Write-Host ""
Write-Host "HUD panel v2 assets complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
