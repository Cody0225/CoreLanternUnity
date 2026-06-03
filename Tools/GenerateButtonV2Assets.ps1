param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\Button_V2_20260530"
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
    foreach ($mul in 3.4, 2.0) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(4, [int]($Color.A / ($mul + 2.6))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Save-Canvas {
    param($Canvas, [string]$Name)
    $path = Join-Path $SkinDir $Name
    if ((Test-Path -LiteralPath $path) -and (-not $Force)) {
        Write-Host "Skip existing $Name"
    } else {
        $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
        Ensure-TextureMeta $path
        Write-Host "Generated $Name"
    }

    $artPath = Join-Path $ArtDir $Name
    $Canvas.Bitmap.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $artPath
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function Draw-Button {
    param(
        [string]$Name,
        [int]$W,
        [int]$H,
        [System.Drawing.Color]$Accent,
        [System.Drawing.Color]$Secondary,
        [string]$Variant = "regular"
    )

    $canvas = New-Canvas $W $H
    $g = $canvas.Graphics
    $radius = if ($H -le 34) { 4 } elseif ($H -le 56) { 5 } else { 6 }
    $path = New-RoundedRectPath 1 1 ($W - 2) ($H - 2) $radius
    $fillA = if ($Variant -eq "primary") { 238 } elseif ($Variant -eq "chip") { 218 } else { 224 }
    $fill = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb($fillA, 3, 14, 20))
    $g.FillPath($fill, $path)
    $fill.Dispose()

    $washAlpha = if ($Variant -eq "primary") { 46 } elseif ($Variant -eq "danger") { 42 } else { 24 }
    $wash = [System.Drawing.SolidBrush]::new((With-Alpha $Accent $washAlpha))
    $g.FillPath($wash, $path)
    $wash.Dispose()

    if ($Variant -eq "primary") {
        $band = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
            [System.Drawing.Rectangle]::new(2, 2, [Math]::Max(1, $W - 4), [Math]::Max(1, $H - 4)),
            (With-Alpha $Accent 16),
            (With-Alpha $Secondary 34),
            [System.Drawing.Drawing2D.LinearGradientMode]::Horizontal
        )
        $g.FillPath($band, $path)
        $band.Dispose()
    }

    $outline = New-Pen (With-Alpha $Accent 128) 1.2
    $g.DrawPath($outline, $path)
    $outline.Dispose()
    $path.Dispose()

    $m = if ($H -le 34) { 5 } elseif ($H -le 46) { 7 } else { 9 }
    $topLen = [Math]::Min([int]($W * 0.48), 170)
    $botLen = [Math]::Min([int]($W * 0.36), 132)
    Draw-GlowLine $g $m $m ($m + $topLen) $m (With-Alpha $Accent 195) 1.5
    Draw-GlowLine $g ($W - $m) ($H - $m) ($W - $m - $botLen) ($H - $m) (With-Alpha $Secondary 130) 1.25

    if ($Variant -eq "primary") {
        $railY = [Math]::Floor($H * 0.5)
        Draw-GlowLine $g 18 $railY ($W - 18) $railY (With-Alpha $Accent 42) 1.0
        $triBrush = [System.Drawing.SolidBrush]::new((With-Alpha $Accent 170))
        $tri = @(
            [System.Drawing.PointF]::new([float]($W * 0.14), [float]($H * 0.31)),
            [System.Drawing.PointF]::new([float]($W * 0.14), [float]($H * 0.69)),
            [System.Drawing.PointF]::new([float]($W * 0.20), [float]($H * 0.50))
        )
        $g.FillPolygon($triBrush, $tri)
        $triBrush.Dispose()
    } elseif ($Variant -eq "chip") {
        $cap = [System.Drawing.SolidBrush]::new((With-Alpha $Accent 28))
        $g.FillRectangle($cap, 10, 10, $W - 20, [Math]::Max(7, [int]($H * 0.18)))
        $cap.Dispose()
    } elseif ($Variant -eq "danger") {
        Draw-GlowLine $g 12 ($H - 10) ($W - 12) ($H - 10) (With-Alpha $Accent 88) 1.6
    }

    Save-Canvas $canvas $Name
}

function New-Preview {
    $previewPath = Join-Path $ArtDir "Button_V2_Preview.png"
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 6, 12, 16))
    $g.FillRectangle($bg, 0, 0, 1280, 720)
    $bg.Dispose()

    $title = [System.Drawing.Font]::new("Arial", 22, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 13, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(245, 184, 224, 255))
    $muted = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(178, 139, 200, 255))
    $g.DrawString("Button V2 exact-size draft", $title, $brush, 34, 24)
    $g.DrawString("Live text stays in Unity. These are native-size button plates, not stretch backgrounds.", $small, $muted, 34, 54)

    $items = @(
        @{File="Button_Primary_Start_v2.png"; X=42; Y=96},
        @{File="Button_MenuSecondary_v2.png"; X=42; Y=178},
        @{File="Button_RunStageChip_v2.png"; X=222; Y=178},
        @{File="Button_DangerChip_v2.png"; X=430; Y=178},
        @{File="Button_RunBack_v2.png"; X=42; Y=318},
        @{File="Button_RunStart_v2.png"; X=292; Y=318},
        @{File="Button_PauseResume_v2.png"; X=610; Y=96},
        @{File="Button_Wide_320x42_v2.png"; X=610; Y=164},
        @{File="Button_OptionsToggle_v2.png"; X=610; Y=226},
        @{File="Button_Stepper_v2.png"; X=890; Y=226},
        @{File="Button_Reroll_v2.png"; X=610; Y=288},
        @{File="Button_SkipReward_v2.png"; X=760; Y=434},
        @{File="Button_ResultRetry_v2.png"; X=42; Y=418},
        @{File="Button_ResultMenu_v2.png"; X=412; Y=418},
        @{File="Button_CloseSmall_v2.png"; X=42; Y=526},
        @{File="Button_CloseWide_v2.png"; X=318; Y=526}
    )

    foreach ($item in $items) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir $item.File))
        $g.DrawImage($img, $item.X, $item.Y, $img.Width, $img.Height)
        $label = $item.File -replace "Button_", "" -replace "_v2.png", ""
        $g.DrawString($label, $small, $muted, $item.X, $item.Y + $img.Height + 5)
        $img.Dispose()
    }

    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
    $title.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()
    Write-Host "Generated Button_V2_Preview.png"
}

$cyan = New-Color 139 240 255 215
$cyanSoft = New-Color 100 205 230 175
$gold = New-Color 255 214 86 205
$green = New-Color 91 220 124 190
$violet = New-Color 205 99 255 195
$red = New-Color 255 104 86 185

Draw-Button "Button_Primary_Start_v2.png" 420 60 $cyan $gold "primary"
Draw-Button "Button_MenuSecondary_v2.png" 150 44 $cyanSoft $cyan "regular"
Draw-Button "Button_RunStageChip_v2.png" 180 108 $cyan $green "chip"
Draw-Button "Button_DangerChip_v2.png" 80 56 $gold $red "danger"
Draw-Button "Button_RunBack_v2.png" 220 60 $cyanSoft $cyan "regular"
Draw-Button "Button_RunStart_v2.png" 280 60 $cyan $gold "primary"
Draw-Button "Button_PauseResume_v2.png" 320 48 $cyan $cyanSoft "regular"
Draw-Button "Button_Wide_320x42_v2.png" 320 42 $cyanSoft $cyan "regular"
Draw-Button "Button_OptionsToggle_v2.png" 250 36 $cyanSoft $green "regular"
Draw-Button "Button_Stepper_v2.png" 44 30 $cyanSoft $cyan "regular"
Draw-Button "Button_Reroll_v2.png" 440 52 $cyan $gold "regular"
Draw-Button "Button_SkipReward_v2.png" 300 52 $gold $cyan "regular"
Draw-Button "Button_ResultRetry_v2.png" 340 64 $green $cyan "primary"
Draw-Button "Button_ResultMenu_v2.png" 240 50 $cyanSoft $cyan "regular"
Draw-Button "Button_CloseSmall_v2.png" 240 32 $cyanSoft $violet "regular"
Draw-Button "Button_CloseWide_v2.png" 260 38 $cyanSoft $violet "regular"

New-Preview

Write-Host ""
Write-Host "Button v2 assets complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
