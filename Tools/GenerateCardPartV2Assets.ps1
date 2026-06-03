param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource\Card_Parts_V2_20260529"
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
    foreach ($mul in 4, 2.2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(4, [int]($Color.A / ($mul + 2.2))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4, 2.2) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(4, [int]($Color.A / ($mul + 2.2))))) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-Poly {
    param($G, [object[]]$Points, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke, [float]$StrokeWidth = 2)
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

function New-HeaderPlate {
    param([string]$Name, [System.Drawing.Color]$Accent, [int]$Glow)
    $canvas = New-Canvas 252 34
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 2 250 30 4
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(214, 4, 18, 24))
    $g.FillPath($bg, $path)
    $bg.Dispose()
    $washColor = With-Alpha $Accent ([int][Math]::Min(96, 20 + $Glow))
    $wash = [System.Drawing.SolidBrush]::new($washColor)
    $g.FillPath($wash, $path)
    $wash.Dispose()
    $pen = New-Pen (With-Alpha $Accent ([Math]::Min(235, 120 + $Glow))) 1.4
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
    Draw-GlowLine $g 16 8 114 8 (With-Alpha $Accent ([Math]::Min(235, 125 + $Glow))) 1.8
    Draw-GlowLine $g 168 26 236 26 (With-Alpha $Accent ([Math]::Min(210, 88 + $Glow))) 1.4
    Save-Canvas $canvas $Name
}

function New-TitlePlate {
    $canvas = New-Canvas 248 36
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 2 246 32 4
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(226, 3, 14, 20))
    $g.FillPath($bg, $path)
    $bg.Dispose()
    $wash = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(22, 139, 200, 255))
    $g.FillPath($wash, $path)
    $wash.Dispose()
    $pen = New-Pen (New-Color 139 200 255 126) 1.2
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
    Draw-GlowLine $g 18 7 230 7 (New-Color 139 200 255 58) 1
    Save-Canvas $canvas "Card_TitlePlate_v2.png"
}

function New-LevelPlate {
    $canvas = New-Canvas 240 22
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 2 238 18 3
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(186, 8, 25, 28))
    $g.FillPath($bg, $path)
    $bg.Dispose()
    $pen = New-Pen (New-Color 139 200 255 88) 1
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
    Draw-GlowLine $g 24 11 216 11 (New-Color 139 200 255 42) 1
    Save-Canvas $canvas "Card_LevelPlate_v2.png"
}

function New-RarityPlate {
    param([string]$Name, [System.Drawing.Color]$Accent, [int]$Pips, [int]$Glow)
    $canvas = New-Canvas 178 28
    $g = $canvas.Graphics
    $path = New-RoundedRectPath 1 2 176 24 4
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(222, 4, 18, 24))
    $g.FillPath($bg, $path)
    $bg.Dispose()
    $washColor = With-Alpha $Accent ([int][Math]::Min(120, 24 + $Glow))
    $wash = [System.Drawing.SolidBrush]::new($washColor)
    $g.FillPath($wash, $path)
    $wash.Dispose()
    $pen = New-Pen (With-Alpha $Accent ([Math]::Min(235, 115 + $Glow))) 1.2
    $g.DrawPath($pen, $path)
    $pen.Dispose()
    $path.Dispose()
    Draw-GlowLine $g 18 23 160 23 (With-Alpha $Accent ([Math]::Min(220, 90 + $Glow))) 1.4

    for ($i = 0; $i -lt 3; $i++) {
        $x = 74 + $i * 16
        $active = $i -lt $Pips
        $fill = if ($active) { With-Alpha $Accent 205 } else { New-Color 44 66 70 170 }
        $stroke = if ($active) { With-Alpha $Accent 245 } else { New-Color 110 130 135 135 }
        Draw-Poly $g @(@($x,7),@(($x + 5),13),@($x,19),@(($x - 5),13)) $fill $stroke 1.3
    }

    Save-Canvas $canvas $Name
}

function New-BottomRail {
    param([string]$Name, [System.Drawing.Color]$Accent)
    $canvas = New-Canvas 210 8
    $g = $canvas.Graphics
    Draw-GlowLine $g 12 4 198 4 (With-Alpha $Accent 165) 1.7
    Save-Canvas $canvas $Name
}

function New-SpecialCorner {
    param([string]$Name, [System.Drawing.Color]$Accent)
    $canvas = New-Canvas 40 40
    $g = $canvas.Graphics
    Draw-GlowEllipse $g 5 5 30 30 (With-Alpha $Accent 160) 2
    Draw-Poly $g @(@(20,4),@(26,15),@(37,20),@(26,25),@(20,36),@(14,25),@(3,20),@(14,15)) (With-Alpha $Accent 125) (With-Alpha $Accent 220) 2
    Save-Canvas $canvas $Name
}

function New-Preview {
    $previewPath = Join-Path $ArtDir "Card_Parts_V2_Preview.png"
    $canvas = New-Canvas 1280 720
    $g = $canvas.Graphics
    $bg = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(255, 8, 15, 18))
    $g.FillRectangle($bg, 0, 0, 1280, 720)
    $bg.Dispose()
    $font = [System.Drawing.Font]::new("Arial", 20, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Arial", 14, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(245, 184, 224, 255))
    $muted = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(180, 139, 200, 255))
    $g.DrawString("Card Parts V2 exact-size draft", $font, $brush, 36, 28)
    $g.DrawString("No baked UI text. Unity text should sit on top.", $small, $muted, 36, 58)

    $items = @(
        @{File="Card_Header_Basic_v2.png"; Label="Header Basic"; X=52; Y=104},
        @{File="Card_Header_Rare_v2.png"; Label="Header Rare"; X=52; Y=164},
        @{File="Card_Header_Epic_v2.png"; Label="Header Epic"; X=52; Y=224},
        @{File="Card_TitlePlate_v2.png"; Label="Title"; X=360; Y=112},
        @{File="Card_LevelPlate_v2.png"; Label="Level"; X=360; Y=172},
        @{File="Card_RarityPlate_Basic_v2.png"; Label="Rarity Basic"; X=360; Y=232},
        @{File="Card_RarityPlate_Rare_v2.png"; Label="Rarity Rare"; X=600; Y=232},
        @{File="Card_RarityPlate_Epic_v2.png"; Label="Rarity Epic"; X=840; Y=232},
        @{File="Card_BottomRail_Cyan_v2.png"; Label="Rail Cyan"; X=630; Y=112},
        @{File="Card_BottomRail_Gold_v2.png"; Label="Rail Gold"; X=630; Y=150},
        @{File="Card_SpecialCorner_Epic_v2.png"; Label="Corner"; X=860; Y=104},
        @{File="Card_SpecialCorner_Cross_v2.png"; Label="Cross"; X=920; Y=104}
    )

    foreach ($item in $items) {
        $img = [System.Drawing.Image]::FromFile((Join-Path $SkinDir $item.File))
        $g.DrawImage($img, $item.X, $item.Y, $img.Width, $img.Height)
        $g.DrawString($item.Label, $small, $muted, $item.X, $item.Y + $img.Height + 7)
        $img.Dispose()
    }

    $canvas.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
    $font.Dispose()
    $small.Dispose()
    $brush.Dispose()
    $muted.Dispose()
    Write-Host "Generated Card_Parts_V2_Preview.png"
}

$cyan = New-Color 139 200 255 210
$yellow = New-Color 255 206 59 218
$green = New-Color 91 200 94 210
$magenta = New-Color 255 84 243 225

New-HeaderPlate "Card_Header_Basic_v2.png" $cyan 12
New-HeaderPlate "Card_Header_Rare_v2.png" $green 42
New-HeaderPlate "Card_Header_Epic_v2.png" $magenta 76
New-TitlePlate
New-LevelPlate
New-RarityPlate "Card_RarityPlate_Basic_v2.png" $cyan 1 14
New-RarityPlate "Card_RarityPlate_Rare_v2.png" $green 2 44
New-RarityPlate "Card_RarityPlate_Epic_v2.png" $yellow 3 78
New-BottomRail "Card_BottomRail_Cyan_v2.png" $cyan
New-BottomRail "Card_BottomRail_Gold_v2.png" $yellow
New-SpecialCorner "Card_SpecialCorner_Epic_v2.png" $yellow
New-SpecialCorner "Card_SpecialCorner_Cross_v2.png" $magenta
New-Preview

Write-Host ""
Write-Host "Card part v2 assets complete."
Write-Host "Resources: $SkinDir"
Write-Host "Art source copies: $ArtDir"
