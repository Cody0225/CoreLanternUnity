param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\UI_Polish_20260529"
New-Item -ItemType Directory -Force -Path $SkinDir, $ArtDir | Out-Null

function Ensure-TextureMeta {
    param([string]$AssetPath)
    $metaPath = $AssetPath + ".meta"
    if ((Test-Path -LiteralPath $metaPath) -and (-not $Force)) { return }

    $templatePath = Join-Path $SkinDir "HUD_WaveProgress_Frame_v2.png.meta"
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

function Save-Bitmap {
    param($Canvas, [string]$FileName, [switch]$AlsoArtSource)
    $path = Join-Path $SkinDir $FileName
    if ((Test-Path -LiteralPath $path) -and (-not $Force)) {
        Write-Host "Skip existing $FileName"
        $Canvas.Graphics.Dispose()
        $Canvas.Bitmap.Dispose()
        return
    }

    $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    Ensure-TextureMeta $path

    if ($AlsoArtSource) {
        $artPath = Join-Path $ArtDir $FileName
        $Canvas.Bitmap.Save($artPath, [System.Drawing.Imaging.ImageFormat]::Png)
        Ensure-TextureMeta $artPath
    }

    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
    Write-Host "Generated $FileName"
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 1.6))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowRect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 1.8))))) ($Width * $mul)
        $G.DrawRectangle($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawRectangle($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 3)
    foreach ($mul in 5, 3, 2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(5, [int]($Color.A / ($mul + 1.8))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    foreach ($mul in 1.9, 1.45, 1.15) {
        $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Color ([Math]::Max(4, [int]($Color.A / ($mul * 3.2))))))
        $cx = $X + $W / 2
        $cy = $Y + $H / 2
        $ww = $W * $mul
        $hh = $H * $mul
        $G.FillEllipse($brush, $cx - $ww / 2, $cy - $hh / 2, $ww, $hh)
        $brush.Dispose()
    }
    $core = [System.Drawing.SolidBrush]::new($Color)
    $G.FillEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-Poly {
    param($G, [object[]]$Points, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke, [float]$StrokeWidth = 3)
    $pts = @()
    foreach ($p in $Points) {
        $pts += [System.Drawing.PointF]::new([float]$p[0], [float]$p[1])
    }
    $brush = [System.Drawing.SolidBrush]::new($Fill)
    $G.FillPolygon($brush, $pts)
    $brush.Dispose()
    $pen = New-Pen $Stroke $StrokeWidth
    $G.DrawPolygon($pen, $pts)
    $pen.Dispose()
}

function Draw-CornerRails {
    param($G, [int]$W, [int]$H, [System.Drawing.Color]$Color)
    $l = 76
    $m = 16
    Draw-GlowLine $G $m $m ($m + $l) $m $Color 2
    Draw-GlowLine $G $m $m $m ($m + 34) (With-Alpha $Color 190) 2
    Draw-GlowLine $G ($W - $m) $m ($W - $m - $l) $m $Color 2
    Draw-GlowLine $G ($W - $m) $m ($W - $m) ($m + 34) (With-Alpha $Color 190) 2
    Draw-GlowLine $G $m ($H - $m) ($m + $l) ($H - $m) $Color 2
    Draw-GlowLine $G $m ($H - $m) $m ($H - $m - 34) (With-Alpha $Color 190) 2
    Draw-GlowLine $G ($W - $m) ($H - $m) ($W - $m - $l) ($H - $m) $Color 2
    Draw-GlowLine $G ($W - $m) ($H - $m) ($W - $m) ($H - $m - 34) (With-Alpha $Color 190) 2
}

function New-ResultRankBadge {
    param([string]$Rank, [System.Drawing.Color]$Accent)
    $canvas = New-Canvas 160 160
    $g = $canvas.Graphics
    Fill-GlowEllipse $g 23 23 114 114 (With-Alpha $Accent 115)
    Draw-GlowEllipse $g 25 25 110 110 (With-Alpha $Accent 220) 3
    Draw-GlowEllipse $g 48 48 64 64 (With-Alpha (New-Color 184 224 255 180) 150) 2
    Draw-Poly $g @(@(80,22),@(96,63),@(138,80),@(96,97),@(80,138),@(64,97),@(22,80),@(64,63)) (With-Alpha (New-Color 11 20 24 225) 225) (With-Alpha $Accent 235) 3

    $font = [System.Drawing.Font]::new("Arial", 56, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $format = [System.Drawing.StringFormat]::new()
    $format.Alignment = [System.Drawing.StringAlignment]::Center
    $format.LineAlignment = [System.Drawing.StringAlignment]::Center
    $shadow = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(170, 0, 0, 0))
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(245, 232, 255, 255))
    $rect = [System.Drawing.RectangleF]::new(0, 4, 160, 152)
    $shadowRect = [System.Drawing.RectangleF]::new(2, 7, 160, 152)
    $g.DrawString($Rank, $font, $shadow, $shadowRect, $format)
    $g.DrawString($Rank, $font, $brush, $rect, $format)
    $font.Dispose()
    $format.Dispose()
    $shadow.Dispose()
    $brush.Dispose()
    Save-Bitmap $canvas ("Result_RankBadge_" + $Rank + ".png") -AlsoArtSource
}

function New-MvpSlot {
    $canvas = New-Canvas 320 96
    $g = $canvas.Graphics
    $cyan = New-Color 139 200 255 190
    $yellow = New-Color 255 206 59 190
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(120, 11, 20, 24))
    $g.FillRectangle($bg, 10, 12, 300, 72)
    $bg.Dispose()
    Draw-GlowRect $g 12 12 296 72 (With-Alpha $cyan 140) 2
    Draw-GlowLine $g 28 20 132 20 (With-Alpha $cyan 210) 2
    Draw-GlowLine $g 212 76 294 76 (With-Alpha $yellow 150) 2
    Fill-GlowEllipse $g 31 28 40 40 (With-Alpha $cyan 160)
    Draw-Poly $g @(@(51,21),@(59,43),@(82,48),@(59,55),@(51,78),@(43,55),@(20,48),@(43,43)) (With-Alpha $yellow 185) (With-Alpha $yellow 230) 2
    Save-Bitmap $canvas "Result_MvpSlot_v2.png" -AlsoArtSource
}

function New-WarningFrame {
    param([string]$Name, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Secondary)
    $canvas = New-Canvas 960 220
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(95, 11, 20, 24))
    $g.FillRectangle($bg, 22, 24, 916, 172)
    $bg.Dispose()
    Draw-GlowRect $g 24 24 912 172 (With-Alpha $Accent 190) 3
    Draw-CornerRails $g 960 220 (With-Alpha $Secondary 210)
    Draw-GlowLine $g 84 58 876 58 (With-Alpha $Accent 130) 2
    Draw-GlowLine $g 160 168 800 168 (With-Alpha $Secondary 110) 2
    Draw-Poly $g @(@(480,28),@(504,74),@(558,88),@(516,122),@(528,176),@(480,148),@(432,176),@(444,122),@(402,88),@(456,74)) (With-Alpha $Accent 65) (With-Alpha $Accent 130) 2
    Save-Bitmap $canvas ($Name + ".png") -AlsoArtSource
}

function New-CoreMark {
    $canvas = New-Canvas 256 128
    $g = $canvas.Graphics
    $magenta = New-Color 255 84 243 215
    $yellow = New-Color 255 206 59 210
    Draw-GlowEllipse $g 64 0 128 128 (With-Alpha $magenta 155) 4
    Draw-GlowLine $g 0 64 256 64 (With-Alpha $magenta 150) 3
    Draw-GlowLine $g 128 0 128 128 (With-Alpha $yellow 130) 2
    Draw-Poly $g @(@(128,24),@(144,52),@(176,64),@(144,76),@(128,104),@(112,76),@(80,64),@(112,52)) (With-Alpha $yellow 180) (With-Alpha $yellow 240) 3
    Save-Bitmap $canvas "Warning_CoreMark_v2.png" -AlsoArtSource
}

function New-PreviewSheet {
    $previewPath = Join-Path $ArtDir "UI_Polish_Preview.png"
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 8, 15, 18))
    $g.FillRectangle($bg, 0, 0, 1280, 720)
    $bg.Dispose()

    $font = [System.Drawing.Font]::new("Arial", 20, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 14, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(245, 184, 224, 255))
    $muted = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(180, 139, 200, 255))
    $g.DrawString("UI Polish Pack 2026-05-29", $font, $brush, 40, 28)
    $g.DrawString("Transparent overlays/icons. Keep aspect ratio when hooked.", $small, $muted, 40, 58)

    $rankFiles = @("Result_RankBadge_S.png","Result_RankBadge_A.png","Result_RankBadge_B.png","Result_RankBadge_C.png","Result_RankBadge_D.png")
    for ($i = 0; $i -lt $rankFiles.Count; $i++) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir $rankFiles[$i]))
        $g.DrawImage($img, 58 + $i * 138, 106, 104, 104)
        $label = "Rank " + $rankFiles[$i].Substring(17, 1)
        $g.DrawString($label, $small, $muted, 84 + $i * 138, 222)
        $img.Dispose()
    }

    $mvp = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Result_MvpSlot_v2.png"))
    $g.DrawImage($mvp, 760, 116, 320, 96)
    $g.DrawString("Result_MvpSlot_v2.png", $small, $muted, 820, 222)
    $mvp.Dispose()

    $mid = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Warning_MidBoss_Frame_v2.png"))
    $g.DrawImage($mid, 72, 300, 540, 124)
    $g.DrawString("Warning_MidBoss_Frame_v2.png", $small, $muted, 226, 436)
    $mid.Dispose()

    $final = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Warning_FinalBoss_Frame_v2.png"))
    $g.DrawImage($final, 668, 300, 540, 124)
    $g.DrawString("Warning_FinalBoss_Frame_v2.png", $small, $muted, 806, 436)
    $final.Dispose()

    $core = [System.Drawing.Image]::FromFile((Join-Path $SkinDir "Warning_CoreMark_v2.png"))
    $g.DrawImage($core, 512, 505, 256, 128)
    $g.DrawString("Warning_CoreMark_v2.png", $small, $muted, 550, 646)
    $core.Dispose()

    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
    $font.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()
    Write-Host "Generated UI_Polish_Preview.png"
}

New-ResultRankBadge "S" (New-Color 255 206 59 220)
New-ResultRankBadge "A" (New-Color 139 200 255 220)
New-ResultRankBadge "B" (New-Color 91 200 94 220)
New-ResultRankBadge "C" (New-Color 255 157 44 220)
New-ResultRankBadge "D" (New-Color 200 74 31 220)
New-MvpSlot
New-WarningFrame "Warning_MidBoss_Frame_v2" (New-Color 200 74 31 205) (New-Color 255 206 59 185)
New-WarningFrame "Warning_FinalBoss_Frame_v2" (New-Color 255 84 243 205) (New-Color 139 200 255 185)
New-CoreMark
New-PreviewSheet

Write-Host ""
Write-Host "UI polish pack complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
