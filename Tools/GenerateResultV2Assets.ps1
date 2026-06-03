param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\Result_V2_20260530"
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
    foreach ($mul in 3.6, 2.0) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(3, [int]($Color.A / ($mul + 2.8))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4.6, 2.4) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(3, [int]($Color.A / ($mul + 3.2))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
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

function New-Panel {
    param([string]$Name, [int]$W, [int]$H, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Secondary, [int]$FillAlpha = 214)
    $canvas = New-Canvas $W $H
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 1 ($W - 2) ($H - 2) 6
    $fill = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb($FillAlpha, 4, 15, 20))
    $g.FillPath($fill, $path)
    $fill.Dispose()
    $wash = [System.Drawing.SolidBrush]::new((With-Alpha $Accent 20))
    $g.FillPath($wash, $path)
    $wash.Dispose()
    $pen = New-Pen (With-Alpha $Accent 112) 1.2
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
    $m = if ($H -le 48) { 7 } else { 12 }
    Draw-GlowLine $g $m $m ([Math]::Min($W - $m, $m + [Math]::Min(220, $W * 0.44))) $m (With-Alpha $Accent 170) 1.5
    Draw-GlowLine $g ($W - $m) ($H - $m) ([Math]::Max($m, $W - $m - [Math]::Min(220, $W * 0.38))) ($H - $m) (With-Alpha $Secondary 105) 1.25
    Save-Canvas $canvas $Name
}

function New-PortraitFrame {
    $canvas = New-Canvas 220 220
    $g = $canvas.Graphics
    $cyan = New-Color 139 240 255 170
    $gold = New-Color 255 206 59 116
    New-Panel "Result_PortraitFrame_v2.png" 220 220 $cyan $gold 188
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

function New-RankMedal {
    param([string]$Name, [System.Drawing.Color]$Accent)
    $canvas = New-Canvas 128 128
    $g = $canvas.Graphics
    Draw-GlowEllipse $g 18 18 92 92 (With-Alpha $Accent 168) 2.4
    Draw-GlowEllipse $g 34 34 60 60 (With-Alpha $Accent 128) 1.8
    $diamond = @(
        [System.Drawing.PointF]::new(64, 24),
        [System.Drawing.PointF]::new(104, 64),
        [System.Drawing.PointF]::new(64, 104),
        [System.Drawing.PointF]::new(24, 64)
    )
    $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Accent 172))
    $g.FillPolygon($brush, $diamond)
    $brush.Dispose()
    $pen = New-Pen (With-Alpha $Accent 235) 2
    $g.DrawPolygon($pen, $diamond)
    $pen.Dispose()
    Save-Canvas $canvas $Name
}

function New-Preview {
    $previewPath = Join-Path $ArtDir "Result_V2_Preview.png"
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 6, 13, 17))
    $g.FillRectangle($bg, 0, 0, 1280, 720)
    $bg.Dispose()

    $deck = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_DeckFrame_v2.png"))
    $g.DrawImage($deck, 160, 70, $deck.Width, $deck.Height)
    $deck.Dispose()
    $portrait = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_PortraitFrame_v2.png"))
    $g.DrawImage($portrait, 230, 210, $portrait.Width, $portrait.Height)
    $portrait.Dispose()
    $badgeA = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_BadgeStrip_Route_v2.png"))
    $badgeB = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_BadgeStrip_Fusion_v2.png"))
    $g.DrawImage($badgeA, 230, 444, $badgeA.Width, $badgeA.Height)
    $g.DrawImage($badgeB, 230, 496, $badgeB.Width, $badgeB.Height)
    $badgeA.Dispose()
    $badgeB.Dispose()
    $tile = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_StatTile_v2.png"))
    for ($row = 0; $row -lt 3; $row++) {
        $g.DrawImage($tile, 500, 220 + $row * 72, $tile.Width, $tile.Height)
    }
    $tile.Dispose()
    $mvp = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_MvpRow_v2.png"))
    for ($i = 0; $i -lt 3; $i++) {
        $g.DrawImage($mvp, 910, 220 + $i * 82, $mvp.Width, $mvp.Height)
    }
    $mvp.Dispose()
    $line = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_SummaryPlate_v2.png"))
    $g.DrawImage($line, 474, 516, $line.Width, $line.Height)
    $line.Dispose()
    $footer = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_FooterGuide_v2.png"))
    $g.DrawImage($footer, 388, 604, $footer.Width, $footer.Height)
    $footer.Dispose()
    $rank = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_RankMedal_S_v2.png"))
    $g.DrawImage($rank, 94, 96, $rank.Width, $rank.Height)
    $rank.Dispose()

    $font = [System.Drawing.Font]::new("Arial", 21, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 13, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(235, 184, 224, 255))
    $muted = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(170, 139, 200, 255))
    $g.DrawString("Result UI V2 exact-size layout pieces", $font, $brush, 32, 24)
    $g.DrawString("No baked labels. Use to simplify the result screen without stretching.", $small, $muted, 32, 52)
    $font.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()

    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
    Write-Host "Generated Result_V2_Preview.png"
}

$cyan = New-Color 139 240 255 190
$gold = New-Color 255 206 59 170
$green = New-Color 115 255 170 190
$magenta = New-Color 255 84 243 170

New-Panel "Result_DeckFrame_v2.png" 960 580 $cyan $gold 218
New-Panel "Result_PortraitFrame_v2.png" 220 220 $cyan $gold 188
New-Panel "Result_StatTile_v2.png" 420 56 $cyan $gold 225
New-Panel "Result_BadgeStrip_Route_v2.png" 220 44 $cyan $gold 226
New-Panel "Result_BadgeStrip_Fusion_v2.png" 220 44 $magenta $cyan 226
New-Panel "Result_MvpRow_v2.png" 230 68 $gold $cyan 226
New-Panel "Result_SummaryPlate_v2.png" 520 44 $cyan $green 172
New-Panel "Result_FooterGuide_v2.png" 520 28 $cyan $magenta 172
New-RankMedal "Result_RankMedal_S_v2.png" $green
New-RankMedal "Result_RankMedal_A_v2.png" $cyan
New-RankMedal "Result_RankMedal_B_v2.png" $gold
New-RankMedal "Result_RankMedal_C_v2.png" (New-Color 170 185 198 150)
New-RankMedal "Result_RankMedal_D_v2.png" (New-Color 128 128 140 130)

New-Preview

Write-Host ""
Write-Host "Result UI v2 assets complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
