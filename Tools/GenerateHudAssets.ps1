param(
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource"
New-Item -ItemType Directory -Force -Path $SkinDir | Out-Null
New-Item -ItemType Directory -Force -Path $ArtDir | Out-Null

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
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    return @{ Bitmap = $bmp; Graphics = $g }
}

function Save-Canvas {
    param($Canvas, [string]$Name)
    $path = Join-Path $SkinDir $Name
    if ((Test-Path -LiteralPath $path) -and -not $Force) {
        $Canvas.Graphics.Dispose()
        $Canvas.Bitmap.Dispose()
        return $path
    }
    $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
    return $path
}

function New-Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 1)
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Square
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Square
    return $pen
}

function Fill-Rect {
    param($G, [System.Drawing.Color]$Color, [float]$X, [float]$Y, [float]$W, [float]$H)
    $brush = [System.Drawing.SolidBrush]::new($Color)
    $G.FillRectangle($brush, $X, $Y, $W, $H)
    $brush.Dispose()
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4, 2.5) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(12, [int]($Color.A / ($mul + 1.4))))) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowRect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in 4, 2.4) {
        $pen = New-Pen (With-Alpha $Color ([Math]::Max(10, [int]($Color.A / ($mul + 1.8))))) ($Width * $mul)
        $G.DrawRectangle($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawRectangle($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-CyberCorners {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    $l = [Math]::Min($W, $H) * 0.24
    Draw-GlowLine $G $X $Y ($X + $l) $Y $Color 2
    Draw-GlowLine $G $X $Y $X ($Y + $l) $Color 2
    Draw-GlowLine $G ($X + $W) $Y ($X + $W - $l) $Y $Color 2
    Draw-GlowLine $G ($X + $W) $Y ($X + $W) ($Y + $l) $Color 2
    Draw-GlowLine $G $X ($Y + $H) ($X + $l) ($Y + $H) $Color 2
    Draw-GlowLine $G $X ($Y + $H) $X ($Y + $H - $l) $Color 2
    Draw-GlowLine $G ($X + $W) ($Y + $H) ($X + $W - $l) ($Y + $H) $Color 2
    Draw-GlowLine $G ($X + $W) ($Y + $H) ($X + $W) ($Y + $H - $l) $Color 2
}

function Draw-PanelSkin {
    param([string]$Name, [int]$W, [int]$H, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Secondary)
    $c = New-Canvas $W $H
    $g = $c.Graphics
    Fill-Rect $g (New-Color 2 16 22 215) 0 0 $W $H
    Fill-Rect $g (With-Alpha $Accent 22) 0 0 ([int]($W * 0.36)) $H
    Fill-Rect $g (With-Alpha $Secondary 18) ([int]($W * 0.66)) 0 ([int]($W * 0.34)) $H
    Draw-GlowRect $g 2 2 ($W - 5) ($H - 5) (With-Alpha $Accent 205) 1.4
    Draw-CyberCorners $g 12 12 ($W - 24) ($H - 24) (With-Alpha $Accent 235)
    Draw-GlowLine $g 18 18 ([int]($W * 0.52)) 18 (With-Alpha $Accent 180) 1.2
    Draw-GlowLine $g ([int]($W * 0.66)) ($H - 16) ($W - 20) ($H - 16) (With-Alpha $Secondary 145) 1.2
    Save-Canvas $c $Name | Out-Null
}

function Draw-BarBack {
    $c = New-Canvas 256 24
    $g = $c.Graphics
    Fill-Rect $g (New-Color 1 8 12 230) 0 0 256 24
    Draw-GlowRect $g 1 1 253 21 (New-Color 55 225 245 180) 1
    Fill-Rect $g (New-Color 10 32 40 175) 5 5 246 14
    Save-Canvas $c "HUD_Bar_Back.png" | Out-Null
}

function Draw-BarFill {
    param([string]$Name, [System.Drawing.Color]$Left, [System.Drawing.Color]$Right)
    $c = New-Canvas 256 24
    $g = $c.Graphics
    $rect = [System.Drawing.RectangleF]::new(0, 0, 256, 24)
    $brush = [System.Drawing.Drawing2D.LinearGradientBrush]::new($rect, $Left, $Right, [System.Drawing.Drawing2D.LinearGradientMode]::Horizontal)
    $g.FillRectangle($brush, $rect)
    $brush.Dispose()
    Fill-Rect $g (New-Color 255 255 255 45) 0 2 256 4
    Fill-Rect $g (New-Color 0 0 0 55) 0 18 256 6
    Save-Canvas $c $Name | Out-Null
}

function Draw-WaveFrame {
    $c = New-Canvas 512 48
    $g = $c.Graphics
    Fill-Rect $g (New-Color 1 12 18 220) 0 0 512 48
    Draw-GlowRect $g 1 1 509 45 (New-Color 48 235 255 190) 1.3
    Draw-CyberCorners $g 13 9 486 30 (New-Color 80 245 255 225)
    Fill-Rect $g (New-Color 12 38 48 190) 24 29 464 9
    Save-Canvas $c "HUD_WaveProgress_Frame.png" | Out-Null
}

function Draw-MinimapBackplate {
    $c = New-Canvas 256 256
    $g = $c.Graphics
    $center = [System.Drawing.PointF]::new(128, 128)
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $path.AddEllipse(10, 10, 236, 236)
    $brush = [System.Drawing.Drawing2D.PathGradientBrush]::new($path)
    $brush.CenterColor = New-Color 8 58 66 220
    $brush.SurroundColors = @((New-Color 1 12 18 230))
    $brush.CenterPoint = $center
    $g.FillPath($brush, $path)
    $brush.Dispose()
    $path.Dispose()
    Save-Canvas $c "Minimap_Backplate.png" | Out-Null
}

function Draw-MinimapFrame {
    $c = New-Canvas 256 256
    $g = $c.Graphics
    Draw-GlowRect $g 20 20 216 216 (New-Color 42 230 245 150) 1.2
    foreach ($r in 118, 104) {
        $pen = New-Pen (New-Color 80 245 255 150) 2
        $g.DrawEllipse($pen, 128 - $r, 128 - $r, $r * 2, $r * 2)
        $pen.Dispose()
    }
    Draw-CyberCorners $g 24 24 208 208 (New-Color 150 255 255 230)
    Save-Canvas $c "Minimap_Frame.png" | Out-Null
}

function Draw-MinimapGrid {
    $c = New-Canvas 256 256
    $g = $c.Graphics
    for ($i = 32; $i -le 224; $i += 32) {
        Draw-GlowLine $g $i 20 $i 236 (New-Color 55 205 220 65) 0.8
        Draw-GlowLine $g 20 $i 236 $i (New-Color 55 205 220 65) 0.8
    }
    $pen = New-Pen (New-Color 100 255 255 92) 1.4
    $g.DrawEllipse($pen, 46, 46, 164, 164)
    $g.DrawLine($pen, 128, 28, 128, 228)
    $g.DrawLine($pen, 28, 128, 228, 128)
    $pen.Dispose()
    Save-Canvas $c "Minimap_Grid.png" | Out-Null
}

function Draw-Dot {
    param([string]$Name, [System.Drawing.Color]$Color, [switch]$Diamond, [switch]$Boss)
    $c = New-Canvas 24 24
    $g = $c.Graphics
    if ($Boss) {
        Draw-GlowRect $g 5 5 13 13 $Color 1.8
        Fill-Rect $g $Color 8 8 8 8
    }
    elseif ($Diamond) {
        $points = @(
            [System.Drawing.PointF]::new(12, 3),
            [System.Drawing.PointF]::new(21, 12),
            [System.Drawing.PointF]::new(12, 21),
            [System.Drawing.PointF]::new(3, 12)
        )
        $brush = [System.Drawing.SolidBrush]::new($Color)
        $g.FillPolygon($brush, $points)
        $brush.Dispose()
    }
    else {
        foreach ($mul in 2.7, 1.8) {
            $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Color ([int](95 / $mul))))
            $size = 9 * $mul
            $g.FillEllipse($brush, 12 - $size / 2, 12 - $size / 2, $size, $size)
            $brush.Dispose()
        }
        $brush = [System.Drawing.SolidBrush]::new($Color)
        $g.FillEllipse($brush, 6, 6, 12, 12)
        $brush.Dispose()
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-Preview {
    $names = @(
        "HUD_Panel_Wave.png", "HUD_Panel_HP.png", "HUD_Panel_Level.png", "HUD_Panel_ChipMini.png",
        "HUD_Bar_Back.png", "HUD_Bar_HP_PlayerFill.png", "HUD_Bar_HP_CoreFill.png", "HUD_Bar_HP_Lag.png", "HUD_Bar_EXP_Fill.png",
        "HUD_WaveProgress_Frame.png", "HUD_WaveProgress_Fill_Normal.png", "HUD_WaveProgress_Fill_Boss.png",
        "Minimap_Backplate.png", "Minimap_Frame.png", "Minimap_Grid.png",
        "Minimap_Dot_Player.png", "Minimap_Dot_Core.png", "Minimap_Dot_Enemy.png", "Minimap_Dot_Boss.png"
    )
    $cols = 4
    $cellW = 280
    $cellH = 170
    $titleH = 62
    $rows = [Math]::Ceiling($names.Count / $cols)
    $c = New-Canvas ($cols * $cellW + 44) ($titleH + $rows * $cellH + 28)
    $g = $c.Graphics
    Fill-Rect $g (New-Color 2 10 16 255) 0 0 ($cols * $cellW + 44) ($titleH + $rows * $cellH + 28)
    $font = [System.Drawing.Font]::new("Segoe UI", 28, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new((New-Color 185 255 255))
    $g.DrawString("HUD / MINIMAP ASSET PACK", $font, $brush, 22, 18)
    $brush.Dispose()
    $font.Dispose()

    for ($i = 0; $i -lt $names.Count; $i++) {
        $col = $i % $cols
        $row = [Math]::Floor($i / $cols)
        $x = 22 + $col * $cellW
        $y = $titleH + $row * $cellH
        Fill-Rect $g (New-Color 3 18 26 230) $x $y ($cellW - 14) ($cellH - 14)
        Draw-GlowRect $g $x $y ($cellW - 14) ($cellH - 14) (New-Color 45 230 245 120) 1
        $path = Join-Path $SkinDir $names[$i]
        if (Test-Path -LiteralPath $path) {
            $img = [System.Drawing.Image]::FromFile($path)
            try {
                $maxW = $cellW - 42
                $maxH = $cellH - 54
                $scale = [Math]::Min($maxW / $img.Width, $maxH / $img.Height)
                $dw = $img.Width * $scale
                $dh = $img.Height * $scale
                $dx = $x + ($cellW - 14 - $dw) / 2
                $dy = $y + 15 + ($maxH - $dh) / 2
                $g.DrawImage($img, [System.Drawing.RectangleF]::new($dx, $dy, $dw, $dh))
            }
            finally {
                $img.Dispose()
            }
        }
        $labelFont = [System.Drawing.Font]::new("Segoe UI", 12, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
        $labelBrush = [System.Drawing.SolidBrush]::new((New-Color 210 245 245))
        $fmt = [System.Drawing.StringFormat]::new()
        $fmt.Alignment = [System.Drawing.StringAlignment]::Center
        $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
        $g.DrawString(($names[$i] -replace "\.png$", ""), $labelFont, $labelBrush, [System.Drawing.RectangleF]::new($x + 5, $y + $cellH - 42, $cellW - 24, 24), $fmt)
        $fmt.Dispose()
        $labelBrush.Dispose()
        $labelFont.Dispose()
    }

    $path = Join-Path $ArtDir "Generated_HudAsset_Preview.png"
    $c.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $c.Graphics.Dispose()
    $c.Bitmap.Dispose()
    return $path
}

Draw-PanelSkin "HUD_Panel_Wave.png" 384 128 (New-Color 42 230 245) (New-Color 245 210 62)
Draw-PanelSkin "HUD_Panel_HP.png" 384 160 (New-Color 42 230 245) (New-Color 255 205 50)
Draw-PanelSkin "HUD_Panel_Level.png" 384 128 (New-Color 130 255 85) (New-Color 42 230 245)
Draw-PanelSkin "HUD_Panel_ChipMini.png" 220 72 (New-Color 75 255 190) (New-Color 245 220 80)
Draw-PanelSkin "HUD_Panel_Links.png" 420 176 (New-Color 42 230 245) (New-Color 255 95 235)

Draw-BarBack
Draw-BarFill "HUD_Bar_HP_PlayerFill.png" (New-Color 42 218 255 245) (New-Color 72 245 255 245)
Draw-BarFill "HUD_Bar_HP_CoreFill.png" (New-Color 255 205 45 245) (New-Color 255 235 80 245)
Draw-BarFill "HUD_Bar_HP_Lag.png" (New-Color 255 70 78 210) (New-Color 255 135 95 210)
Draw-BarFill "HUD_Bar_EXP_Fill.png" (New-Color 110 245 70 245) (New-Color 190 255 95 245)
Draw-WaveFrame
Draw-BarFill "HUD_WaveProgress_Fill_Normal.png" (New-Color 38 200 255 245) (New-Color 62 245 255 245)
Draw-BarFill "HUD_WaveProgress_Fill_Boss.png" (New-Color 255 55 210 245) (New-Color 255 90 95 245)

Draw-MinimapBackplate
Draw-MinimapFrame
Draw-MinimapGrid
Draw-Dot "Minimap_Dot_Player.png" (New-Color 48 230 255 255)
Draw-Dot "Minimap_Dot_Core.png" (New-Color 255 215 45 255) -Diamond
Draw-Dot "Minimap_Dot_Enemy.png" (New-Color 255 65 80 255)
Draw-Dot "Minimap_Dot_Boss.png" (New-Color 255 55 225 255) -Boss

$preview = Draw-Preview

Write-Output "Generated HUD/minimap assets in $SkinDir"
Write-Output "Preview: $preview"
