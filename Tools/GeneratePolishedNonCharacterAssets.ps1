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

$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$BackupDir = Join-Path $ArtDir "NonCharacterAssetBackup_$stamp"
$DidCreateBackup = $false
$BackedUp = [System.Collections.Generic.HashSet[string]]::new()

function C {
    param([int]$R, [int]$G, [int]$B, [int]$A = 255)
    [System.Drawing.Color]::FromArgb($A, $R, $G, $B)
}

function A {
    param([System.Drawing.Color]$Color, [int]$Alpha)
    [System.Drawing.Color]::FromArgb($Alpha, $Color.R, $Color.G, $Color.B)
}

function New-Canvas {
    param([int]$W, [int]$H)
    $bmp = [System.Drawing.Bitmap]::new($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
    $g.Clear([System.Drawing.Color]::Transparent)
    @{ Bitmap = $bmp; Graphics = $g }
}

function Brush {
    param([System.Drawing.Color]$Color)
    [System.Drawing.SolidBrush]::new($Color)
}

function Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 1.0)
    $p = [System.Drawing.Pen]::new($Color, $Width)
    $p.StartCap = [System.Drawing.Drawing2D.LineCap]::Square
    $p.EndCap = [System.Drawing.Drawing2D.LineCap]::Square
    $p.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Miter
    $p
}

function Fill-Rect {
    param($G, [System.Drawing.Color]$Color, [float]$X, [float]$Y, [float]$W, [float]$H)
    $b = Brush $Color
    $G.FillRectangle($b, $X, $Y, $W, $H)
    $b.Dispose()
}

function Fill-Ellipse {
    param($G, [System.Drawing.Color]$Color, [float]$X, [float]$Y, [float]$W, [float]$H)
    $b = Brush $Color
    $G.FillEllipse($b, $X, $Y, $W, $H)
    $b.Dispose()
}

function Draw-Line {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    $p = Pen $Color $Width
    $G.DrawLine($p, $X1, $Y1, $X2, $Y2)
    $p.Dispose()
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    foreach ($m in 4.0, 2.2) {
        $p = Pen (A $Color ([Math]::Max(8, [int]($Color.A / ($m + 2.0))))) ($Width * $m)
        $G.DrawLine($p, $X1, $Y1, $X2, $Y2)
        $p.Dispose()
    }
    Draw-Line $G $X1 $Y1 $X2 $Y2 $Color $Width
}

function Draw-Rect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    $p = Pen $Color $Width
    $G.DrawRectangle($p, $X, $Y, $W, $H)
    $p.Dispose()
}

function Draw-GlowRect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    foreach ($m in 4.0, 2.1) {
        $p = Pen (A $Color ([Math]::Max(8, [int]($Color.A / ($m + 2.1))))) ($Width * $m)
        $G.DrawRectangle($p, $X, $Y, $W, $H)
        $p.Dispose()
    }
    Draw-Rect $G $X $Y $W $H $Color $Width
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    foreach ($m in 4.0, 2.2) {
        $p = Pen (A $Color ([Math]::Max(8, [int]($Color.A / ($m + 2.0))))) ($Width * $m)
        $G.DrawEllipse($p, $X, $Y, $W, $H)
        $p.Dispose()
    }
    $core = Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-Poly {
    param($G, [object[]]$Pts, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke)
    $points = @()
    foreach ($pt in $Pts) {
        $points += [System.Drawing.PointF]::new([float]$pt[0], [float]$pt[1])
    }
    $b = Brush $Fill
    $G.FillPolygon($b, $points)
    $b.Dispose()
    $p = Pen $Stroke 1.5
    $G.DrawPolygon($p, $points)
    $p.Dispose()
}

function New-LinearBrush {
    param([System.Drawing.RectangleF]$Rect, [System.Drawing.Color]$A, [System.Drawing.Color]$B, [string]$Mode = "Horizontal")
    $modeValue = if ($Mode -eq "Vertical") {
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    } else {
        [System.Drawing.Drawing2D.LinearGradientMode]::Horizontal
    }
    [System.Drawing.Drawing2D.LinearGradientBrush]::new($Rect, $A, $B, $modeValue)
}

function Backup-Existing {
    param([string]$Path)
    if (-not $Force -or -not (Test-Path -LiteralPath $Path)) { return }
    if ($BackedUp.Contains($Path)) { return }
    if (-not $script:DidCreateBackup) {
        New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null
        $script:DidCreateBackup = $true
    }
    Copy-Item -LiteralPath $Path -Destination (Join-Path $BackupDir (Split-Path -Leaf $Path)) -Force
    $meta = "$Path.meta"
    if (Test-Path -LiteralPath $meta) {
        Copy-Item -LiteralPath $meta -Destination (Join-Path $BackupDir ((Split-Path -Leaf $Path) + ".meta")) -Force
    }
    [void]$BackedUp.Add($Path)
}

function Save-Canvas {
    param($Canvas, [string]$Name)
    $path = Join-Path $SkinDir $Name
    if ((Test-Path -LiteralPath $path) -and -not $Force) {
        $Canvas.Graphics.Dispose()
        $Canvas.Bitmap.Dispose()
        return $path
    }
    Backup-Existing $path
    $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
    return $path
}

function Draw-Corners {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [float]$L, [System.Drawing.Color]$Color, [float]$Width = 2.0)
    Draw-GlowLine $G $X $Y ($X + $L) $Y $Color $Width
    Draw-GlowLine $G $X $Y $X ($Y + $L) $Color $Width
    Draw-GlowLine $G ($X + $W) $Y ($X + $W - $L) $Y $Color $Width
    Draw-GlowLine $G ($X + $W) $Y ($X + $W) ($Y + $L) $Color $Width
    Draw-GlowLine $G $X ($Y + $H) ($X + $L) ($Y + $H) $Color $Width
    Draw-GlowLine $G $X ($Y + $H) $X ($Y + $H - $L) $Color $Width
    Draw-GlowLine $G ($X + $W) ($Y + $H) ($X + $W - $L) ($Y + $H) $Color $Width
    Draw-GlowLine $G ($X + $W) ($Y + $H) ($X + $W) ($Y + $H - $L) $Color $Width
}

function Draw-CardFrame {
    param([string]$Name, [System.Drawing.Color]$Accent)
    $w = 320
    $h = 400
    $c = New-Canvas $w $h
    $g = $c.Graphics
    Fill-Rect $g (C 1 14 20 220) 0 0 $w $h

    # Clean stretchable center: no pattern, only a soft vertical value shift.
    $rect = [System.Drawing.RectangleF]::new(30, 30, 260, 340)
    $grad = New-LinearBrush $rect (C 4 24 30 205) (C 1 10 15 205) "Vertical"
    $g.FillRectangle($grad, $rect)
    $grad.Dispose()

    Fill-Rect $g (A $Accent 26) 0 0 30 $h
    Fill-Rect $g (A $Accent 16) ($w - 30) 0 30 $h
    Fill-Rect $g (A $Accent 18) 0 0 $w 30
    Fill-Rect $g (A $Accent 12) 0 ($h - 30) $w 30

    Draw-GlowRect $g 1.5 1.5 ($w - 4) ($h - 4) (A $Accent 210) 1.6
    Draw-Rect $g 28 28 ($w - 57) ($h - 57) (A $Accent 82) 1
    Draw-Corners $g 18 18 ($w - 36) ($h - 36) 34 (A $Accent 238) 2.1

    Draw-GlowLine $g 35 30 130 30 (A $Accent 210) 2.4
    Draw-GlowLine $g ($w - 130) ($h - 30) ($w - 35) ($h - 30) (A $Accent 145) 2.4
    Fill-Rect $g (A $Accent 95) 0 152 8 54
    Fill-Rect $g (A $Accent 75) ($w - 8) 198 8 54
    Fill-Rect $g (C 255 255 255 28) 42 42 ($w - 84) 3

    Save-Canvas $c $Name | Out-Null
}

function Draw-HudPanel {
    param([string]$Name, [int]$W, [int]$H, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Warm = $(C 255 218 68))
    $c = New-Canvas $W $H
    $g = $c.Graphics
    Fill-Rect $g (C 0 12 18 215) 0 0 $W $H
    Fill-Rect $g (A $Accent 24) 0 0 ([Math]::Max(18, [int]($W * 0.18))) $H
    Fill-Rect $g (A $Warm 12) 0 0 4 $H
    Draw-GlowRect $g 1.5 1.5 ($W - 4) ($H - 4) (A $Accent 190) 1.2
    Draw-Corners $g 13 13 ($W - 26) ($H - 26) ([Math]::Min(28, [Math]::Min($W, $H) * 0.30)) (A $Accent 210) 1.8
    Draw-GlowLine $g 17 14 ([Math]::Min($W - 18, $W * 0.55)) 14 (A $Accent 160) 1.2
    Draw-GlowLine $g ([Math]::Max(18, $W * 0.66)) ($H - 12) ($W - 17) ($H - 12) (A $Accent 88) 1.2
    Save-Canvas $c $Name | Out-Null
}

function Draw-BarBack {
    $c = New-Canvas 132 20
    $g = $c.Graphics
    Fill-Rect $g (C 0 8 12 235) 0 0 132 20
    Fill-Rect $g (C 10 32 39 190) 3 3 126 14
    Draw-GlowRect $g 0.5 0.5 130 18 (C 65 218 238 130) 1
    Save-Canvas $c "HUD_Bar_Back.png" | Out-Null
}

function Draw-BarFill {
    param([string]$Name, [int]$W, [int]$H, [System.Drawing.Color]$Left, [System.Drawing.Color]$Right)
    $c = New-Canvas $W $H
    $g = $c.Graphics
    $rect = [System.Drawing.RectangleF]::new(0, 0, $W, $H)
    $br = New-LinearBrush $rect $Left $Right
    $g.FillRectangle($br, $rect)
    $br.Dispose()
    Fill-Rect $g (C 255 255 255 46) 0 1 $W ([Math]::Max(2, [int]($H * 0.25)))
    Fill-Rect $g (C 0 0 0 38) 0 ([int]($H * 0.72)) $W ([Math]::Max(2, [int]($H * 0.28)))
    Save-Canvas $c $Name | Out-Null
}

function Draw-WaveFrame {
    $c = New-Canvas 520 26
    $g = $c.Graphics
    Fill-Rect $g (C 0 10 16 220) 0 0 520 26
    Fill-Rect $g (C 12 36 44 165) 9 7 502 12
    Draw-GlowRect $g 0.5 0.5 518 24 (C 68 232 250 160) 1
    Draw-Corners $g 8 5 504 16 34 (C 145 255 255 165) 1.2
    Save-Canvas $c "HUD_WaveProgress_Frame.png" | Out-Null
}

function Draw-Backdrop {
    param([string]$Name, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Accent2, [switch]$MagentaBurst, [switch]$Defeat, [switch]$Boss)
    $w = 1920
    $h = 1080
    $c = New-Canvas $w $h
    $g = $c.Graphics

    $top = C 0 14 20 155
    $bottom = C 0 5 11 155
    if ($Defeat) {
        $top = C 24 4 14 155
        $bottom = C 6 1 8 155
    } elseif ($Boss) {
        $top = C 5 8 18 158
        $bottom = C 0 4 10 158
    }

    $rect = [System.Drawing.RectangleF]::new(0, 0, $w, $h)
    $bg = [System.Drawing.Drawing2D.LinearGradientBrush]::new(
        $rect,
        $top,
        $bottom,
        [System.Drawing.Drawing2D.LinearGradientMode]::Vertical
    )
    $g.FillRectangle($bg, $rect)
    $bg.Dispose()

    Fill-Rect $g (A $Accent 9) 0 0 $w $h
    Fill-Rect $g (A $Accent2 5) 0 0 $w $h

    if ($Boss) {
        Fill-Rect $g (A $Accent 8) 0 0 $w $h
    }
    if ($MagentaBurst) {
        Fill-Rect $g (A $Accent 7) 0 0 $w $h
    }
    if ($Defeat) {
        Fill-Rect $g (C 140 18 54 10) 0 0 $w $h
    }

    Fill-Rect $g (C 0 0 0 28) 0 0 $w 120
    Fill-Rect $g (C 0 0 0 32) 0 ($h - 150) $w 150

    $edgeAlpha = 34
    Draw-GlowLine $g 90 84 470 84 (A $Accent2 $edgeAlpha) 2
    Draw-GlowLine $g ($w - 470) 84 ($w - 90) 84 (A $Accent $edgeAlpha) 2
    Draw-GlowLine $g 90 ($h - 84) 520 ($h - 84) (A $Accent $edgeAlpha) 2
    Draw-GlowLine $g ($w - 520) ($h - 84) ($w - 90) ($h - 84) (A $Accent2 $edgeAlpha) 2

    Draw-Line $g 122 108 122 238 (A $Accent 24) 1
    Draw-Line $g ($w - 122) 108 ($w - 122) 238 (A $Accent2 24) 1
    Draw-Line $g 122 ($h - 238) 122 ($h - 108) (A $Accent2 22) 1
    Draw-Line $g ($w - 122) ($h - 238) ($w - 122) ($h - 108) (A $Accent 22) 1

    for ($i = 0; $i -lt 10; $i++) {
        $x = if (($i % 2) -eq 0) { 72 + (($i * 67) % 360) } else { $w - 432 + (($i * 83) % 360) }
        $y = 150 + (($i * 211) % 780)
        $len = 28 + (($i * 13) % 54)
        Draw-Line $g $x $y ($x + $len) $y (A $Accent2 18) 1
    }

    Save-Canvas $c $Name | Out-Null
}

function Draw-Bullet {
    param([string]$Name, [System.Drawing.Color]$Accent, [string]$Kind)
    $c = New-Canvas 64 64
    $g = $c.Graphics
    Fill-Ellipse $g (A $Accent 30) 6 6 52 52
    switch ($Kind) {
        "speed" {
            Fill-Poly $g @(@(12,34),@(42,15),@(55,30),@(28,42)) (A $Accent 218) (C 235 255 255 190)
            Draw-GlowLine $g 8 45 35 34 (A $Accent 170) 2.5
        }
        "power" {
            Fill-Ellipse $g (A $Accent 220) 18 18 28 28
            Draw-GlowEllipse $g 14 14 36 36 (A $Accent 190) 3
            Fill-Ellipse $g (C 255 248 190 200) 25 20 10 10
        }
        "guard" {
            Draw-GlowEllipse $g 14 14 36 36 (A $Accent 210) 4
            Fill-Poly $g @(@(32,17),@(46,28),@(41,48),@(23,48),@(18,28)) (A $Accent 88) (C 235 255 235 180)
        }
        default {
            Fill-Poly $g @(@(32,8),@(49,24),@(42,49),@(16,49),@(15,22)) (A $Accent 135) (C 255 225 255 170)
            Draw-GlowEllipse $g 15 15 34 34 (A $Accent 210) 3
        }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-PickupHeal {
    $c = New-Canvas 64 64
    $g = $c.Graphics
    Fill-Ellipse $g (C 80 255 135 45) 7 7 50 50
    Draw-GlowEllipse $g 12 12 40 40 (C 116 255 150 185) 2.8
    Fill-Poly $g @(@(32,16),@(45,28),@(40,46),@(32,52),@(24,46),@(19,28)) (C 105 255 150 205) (C 230 255 230 190)
    Fill-Rect $g (C 235 255 230 205) 29 24 6 24
    Fill-Rect $g (C 235 255 230 205) 21 33 22 6
    Save-Canvas $c "Pickup_Heal.png" | Out-Null
}

function Draw-EffectRing {
    param([string]$Name, [int]$Size, [System.Drawing.Color]$Accent, [string]$Kind)
    $c = New-Canvas $Size $Size
    $g = $c.Graphics
    $mid = $Size / 2
    $r1 = $Size * 0.34
    $r2 = $Size * 0.46
    Draw-GlowEllipse $g ($mid - $r2) ($mid - $r2) ($r2 * 2) ($r2 * 2) (A $Accent 145) ([Math]::Max(2, $Size / 92))
    Draw-GlowEllipse $g ($mid - $r1) ($mid - $r1) ($r1 * 2) ($r1 * 2) (A $Accent 160) ([Math]::Max(2, $Size / 112))
    if ($Kind -eq "flash") {
        Fill-Poly $g @(@($mid, 8), @(($mid + 20), ($mid - 18)), @(($Size - 8), $mid), @(($mid + 20), ($mid + 18)), @($mid, ($Size - 8)), @(($mid - 20), ($mid + 18)), @(8, $mid), @(($mid - 20), ($mid - 18))) (A $Accent 120) (A (C 255 255 255) 160)
    } elseif ($Kind -eq "explosion") {
        Fill-Poly $g @(@($mid, 12), @(($mid + 30), ($mid - 26)), @(($Size - 12), $mid), @(($mid + 28), ($mid + 26)), @($mid, ($Size - 12)), @(($mid - 28), ($mid + 26)), @(12, $mid), @(($mid - 30), ($mid - 26))) (A $Accent 130) (A (C 255 245 210) 150)
    } else {
        for ($i = 0; $i -lt 8; $i++) {
            $a0 = [Math]::PI * 2 * $i / 8.0
            $x1 = $mid + [Math]::Cos($a0) * ($Size * 0.22)
            $y1 = $mid + [Math]::Sin($a0) * ($Size * 0.22)
            $x2 = $mid + [Math]::Cos($a0) * ($Size * 0.47)
            $y2 = $mid + [Math]::Sin($a0) * ($Size * 0.47)
            Draw-GlowLine $g $x1 $y1 $x2 $y2 (A $Accent 96) ([Math]::Max(1.2, $Size / 180))
        }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-LinkIcon {
    param([string]$Name, [System.Drawing.Color]$Accent, [string]$Kind)
    $c = New-Canvas 64 64
    $g = $c.Graphics
    Fill-Ellipse $g (C 1 14 20 220) 4 4 56 56
    Draw-GlowEllipse $g 8 8 48 48 (A $Accent 170) 2
    switch ($Kind) {
        "nova" { Fill-Poly $g @(@(32,13),@(40,29),@(57,32),@(40,36),@(32,51),@(24,36),@(7,32),@(24,29)) (A $Accent 190) (C 255 246 210 170) }
        "bulwark" { Fill-Poly $g @(@(32,11),@(49,21),@(45,45),@(32,55),@(19,45),@(15,21)) (A $Accent 165) (C 225 255 230 180) }
        "siphon" { Draw-GlowEllipse $g 19 19 26 26 (A $Accent 210) 4; Fill-Ellipse $g (A $Accent 170) 28 12 8 40 }
        "phase" { Fill-Poly $g @(@(34,12),@(50,24),@(42,36),@(52,51),@(30,43),@(16,52),@(23,35),@(13,22)) (A $Accent 175) (C 255 220 255 170) }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-StatIcon {
    param([string]$Name, [System.Drawing.Color]$Accent, [string]$Kind)
    $c = New-Canvas 32 32
    $g = $c.Graphics
    Fill-Ellipse $g (C 0 13 18 210) 2 2 28 28
    switch ($Kind) {
        "hp" { Fill-Poly $g @(@(16,9),@(24,15),@(21,24),@(16,28),@(11,24),@(8,15)) (A $Accent 190) (C 230 255 245 170) }
        "atk" { Draw-GlowLine $g 9 23 22 8 (A $Accent 220) 3; Fill-Poly $g @(@(21,6),@(26,5),@(25,11)) (A $Accent 190) (C 255 245 210 170) }
        "spd" { Fill-Poly $g @(@(7,19),@(16,6),@(25,19),@(18,17),@(16,27),@(14,17)) (A $Accent 178) (C 230 255 255 170) }
        "fire" { Fill-Poly $g @(@(8,18),@(18,8),@(26,13),@(22,23),@(12,24)) (A $Accent 185) (C 255 244 205 170) }
        "spc" { Fill-Poly $g @(@(16,5),@(20,13),@(29,16),@(20,19),@(16,27),@(12,19),@(3,16),@(12,13)) (A $Accent 176) (C 240 255 255 170) }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-FloorDarkBase {
    $c = New-Canvas 256 256
    $g = $c.Graphics
    Fill-Rect $g (C 1 12 18 255) 0 0 256 256
    for ($i = 0; $i -le 256; $i += 32) {
        Draw-Line $g $i 0 $i 256 (C 45 200 220 34) 1
        Draw-Line $g 0 $i 256 $i (C 45 200 220 30) 1
    }
    Fill-Rect $g (C 255 215 70 18) 64 64 32 32
    Fill-Rect $g (C 70 235 255 18) 160 96 64 32
    Save-Canvas $c "Floor_DarkBase.png" | Out-Null
}

function Draw-HazardLava {
    $c = New-Canvas 256 256
    $g = $c.Graphics
    Fill-Rect $g (C 18 4 5 255) 0 0 256 256
    for ($i = 0; $i -lt 12; $i++) {
        $x = (($i * 71) % 240)
        $y = (($i * 43) % 240)
        Draw-GlowLine $g $x $y ($x + 46) ($y + 18) (C 255 74 32 105) 3
    }
    for ($i = 0; $i -le 256; $i += 32) {
        Draw-Line $g $i 0 $i 256 (C 255 140 45 26) 1
        Draw-Line $g 0 $i 256 $i (C 255 140 45 22) 1
    }
    Save-Canvas $c "Floor_Hazard_Lava.png" | Out-Null
}

function Draw-ArenaBoundary {
    $c = New-Canvas 512 512
    $g = $c.Graphics
    Draw-GlowEllipse $g 18 18 476 476 (C 65 232 255 180) 4
    Draw-GlowEllipse $g 42 42 428 428 (C 255 218 70 105) 2.2
    for ($i = 0; $i -lt 32; $i++) {
        $a0 = [Math]::PI * 2 * $i / 32.0
        $x1 = 256 + [Math]::Cos($a0) * 206
        $y1 = 256 + [Math]::Sin($a0) * 206
        $x2 = 256 + [Math]::Cos($a0) * 236
        $y2 = 256 + [Math]::Sin($a0) * 236
        Draw-GlowLine $g $x1 $y1 $x2 $y2 (C 75 235 255 90) 2
    }
    Save-Canvas $c "Arena_Boundary.png" | Out-Null
}

function Draw-Preview {
    $items = @(
        "Card_Cyan.png","Card_Gold.png","Card_Green.png","Card_Magenta.png","Card_Red.png",
        "HUD_Panel_Wave.png","HUD_Panel_HP.png","HUD_Panel_Level.png","HUD_Panel_ChipMini.png","HUD_Panel_Loadout.png",
        "HUD_Bar_Back.png","HUD_Bar_HP_PlayerFill.png","HUD_Bar_HP_CoreFill.png","HUD_Bar_EXP_Fill.png","HUD_WaveProgress_Frame.png",
        "MainMenu_Background.png","Cutin_Evolve_Speed.png","Cutin_Fusion.png","Result_Clear_Background.png","Result_GameOver_Background.png",
        "Bullet_Speed.png","Bullet_Power.png","Bullet_Guard.png","Bullet_Fusion.png","Pickup_Heal.png",
        "Effect_LevelUp.png","Effect_Evolution_Ring.png","Effect_HitFlash.png","Effect_Explosion.png","Effect_Knockback_Ring.png",
        "Icon_Link_Nova.png","Icon_Link_Bulwark.png","Icon_Link_Siphon.png","Icon_Link_Phase.png",
        "Icon_Stat_HP.png","Icon_Stat_ATK.png","Icon_Stat_SPD.png","Icon_Stat_FIRE.png","Icon_Stat_SPC.png"
    )
    $cols = 5
    $cellW = 250
    $cellH = 168
    $titleH = 72
    $rows = [Math]::Ceiling($items.Count / $cols)
    $w = $cols * $cellW + 40
    $h = $titleH + $rows * $cellH + 32
    $c = New-Canvas $w $h
    $g = $c.Graphics
    Fill-Rect $g (C 1 10 16 255) 0 0 $w $h
    $font = [System.Drawing.Font]::new("Segoe UI", 26, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $small = [System.Drawing.Font]::new("Segoe UI", 11, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $b = Brush (C 190 255 255 255)
    $g.DrawString("POLISHED NON-CHARACTER ASSET PACK", $font, $b, 22, 20)
    $b.Dispose()

    for ($i = 0; $i -lt $items.Count; $i++) {
        $col = $i % $cols
        $row = [Math]::Floor($i / $cols)
        $x = 20 + $col * $cellW
        $y = $titleH + $row * $cellH
        Fill-Rect $g (C 3 22 29 230) $x $y ($cellW - 14) ($cellH - 14)
        Draw-Rect $g $x $y ($cellW - 14) ($cellH - 14) (C 55 220 235 90) 1
        $path = Join-Path $SkinDir $items[$i]
        if (Test-Path -LiteralPath $path) {
            $img = [System.Drawing.Image]::FromFile($path)
            try {
                $maxW = $cellW - 40
                $maxH = $cellH - 46
                $scale = [Math]::Min($maxW / $img.Width, $maxH / $img.Height)
                $dw = $img.Width * $scale
                $dh = $img.Height * $scale
                $dx = $x + ($cellW - 14 - $dw) / 2
                $dy = $y + 12 + ($maxH - $dh) / 2
                $g.DrawImage($img, [System.Drawing.RectangleF]::new($dx, $dy, $dw, $dh))
            } finally {
                $img.Dispose()
            }
        }
        $lb = Brush (C 210 246 246 255)
        $fmt = [System.Drawing.StringFormat]::new()
        $fmt.Alignment = [System.Drawing.StringAlignment]::Center
        $fmt.LineAlignment = [System.Drawing.StringAlignment]::Center
        $g.DrawString(($items[$i] -replace "\.png$", ""), $small, $lb, [System.Drawing.RectangleF]::new($x + 6, $y + $cellH - 42, $cellW - 26, 28), $fmt)
        $fmt.Dispose()
        $lb.Dispose()
    }

    $preview = Join-Path $ArtDir "Generated_PolishedNonCharacterAsset_Preview.png"
    $c.Bitmap.Save($preview, [System.Drawing.Imaging.ImageFormat]::Png)
    $font.Dispose()
    $small.Dispose()
    $c.Graphics.Dispose()
    $c.Bitmap.Dispose()
    $preview
}

$cyan = C 50 229 255 245
$gold = C 255 205 52 245
$green = C 105 255 126 245
$magenta = C 255 84 238 245
$red = C 255 75 92 245

Draw-CardFrame "Card_Cyan.png" $cyan
Draw-CardFrame "Card_Gold.png" $gold
Draw-CardFrame "Card_Green.png" $green
Draw-CardFrame "Card_Magenta.png" $magenta
Draw-CardFrame "Card_Red.png" $red

Draw-HudPanel "HUD_Panel_Wave.png" 250 74 $cyan $gold
Draw-HudPanel "HUD_Panel_HP.png" 270 104 $cyan $gold
Draw-HudPanel "HUD_Panel_Level.png" 270 76 $green $cyan
Draw-HudPanel "HUD_Panel_ChipMini.png" 190 42 (C 86 255 185) $gold
Draw-HudPanel "HUD_Panel_Loadout.png" 296 92 $cyan $magenta
Draw-HudPanel "HUD_Panel_Status.png" 296 120 $cyan $gold
Draw-HudPanel "HUD_Panel_Links.png" 330 110 $cyan $magenta

Draw-BarBack
Draw-BarFill "HUD_Bar_HP_PlayerFill.png" 132 16 (C 42 210 255 245) (C 88 245 255 245)
Draw-BarFill "HUD_Bar_HP_CoreFill.png" 132 16 (C 255 196 42 245) (C 255 232 74 245)
Draw-BarFill "HUD_Bar_EXP_Fill.png" 132 16 (C 110 240 70 245) (C 190 255 92 245)
Draw-BarFill "HUD_Bar_HP_Lag.png" 132 16 (C 255 70 82 220) (C 255 138 92 220)
Draw-WaveFrame
Draw-BarFill "HUD_WaveProgress_Fill_Normal.png" 520 22 (C 35 190 255 245) (C 72 245 255 245)
Draw-BarFill "HUD_WaveProgress_Fill_Boss.png" 520 22 (C 255 54 216 245) (C 255 82 112 245)

Draw-Backdrop "Background_MainMenu.png" $cyan $gold
Draw-Backdrop "Background_Evolution.png" $cyan (C 160 255 255 210)
Draw-Backdrop "Background_CrossEvolution.png" $magenta $cyan -MagentaBurst
Draw-Backdrop "Background_Victory.png" $green $gold
Draw-Backdrop "Background_Defeat.png" $red (C 190 120 255 210) -Defeat
Draw-Backdrop "Background_BossPulswyrm.png" (C 255 145 38 235) $gold -Boss
Draw-Backdrop "Background_BossNullwyrm.png" $magenta $cyan -Boss -MagentaBurst

# Code-compatible aliases currently used by CoreLanternGame.cs.
Draw-Backdrop "MainMenu_Background.png" $cyan $gold
Draw-Backdrop "Cutin_Evolve_Speed.png" $cyan (C 160 255 255 210)
Draw-Backdrop "Cutin_Evolve_Power.png" (C 255 150 42 235) $gold
Draw-Backdrop "Cutin_Evolve_Guard.png" $green $gold
Draw-Backdrop "Cutin_Fusion.png" $magenta $cyan -MagentaBurst
Draw-Backdrop "Result_Clear_Background.png" $green $gold
Draw-Backdrop "Result_GameOver_Background.png" $red (C 190 120 255 210) -Defeat

Draw-Bullet "Bullet_Speed.png" $cyan "speed"
Draw-Bullet "Bullet_Power.png" (C 255 158 42 245) "power"
Draw-Bullet "Bullet_Guard.png" $green "guard"
Draw-Bullet "Bullet_Fusion.png" $magenta "fusion"
Draw-PickupHeal

Draw-EffectRing "Effect_LevelUp.png" 256 $gold "ring"
Draw-EffectRing "Effect_Evolution_Ring.png" 512 $cyan "ring"
Draw-EffectRing "Effect_HitFlash.png" 128 (C 240 255 255 235) "flash"
Draw-EffectRing "Effect_Explosion.png" 256 (C 255 120 42 235) "explosion"
Draw-EffectRing "Effect_Knockback_Ring.png" 256 $green "ring"

Draw-LinkIcon "Icon_Link_Nova.png" (C 255 150 42 245) "nova"
Draw-LinkIcon "Icon_Link_Bulwark.png" $green "bulwark"
Draw-LinkIcon "Icon_Link_Siphon.png" (C 255 226 72 245) "siphon"
Draw-LinkIcon "Icon_Link_Phase.png" (C 255 115 225 245) "phase"

Draw-StatIcon "Icon_Stat_HP.png" (C 92 235 255 245) "hp"
Draw-StatIcon "Icon_Stat_ATK.png" (C 255 160 50 245) "atk"
Draw-StatIcon "Icon_Stat_SPD.png" $cyan "spd"
Draw-StatIcon "Icon_Stat_FIRE.png" (C 255 215 74 245) "fire"
Draw-StatIcon "Icon_Stat_SPC.png" (C 190 135 255 245) "spc"

Draw-FloorDarkBase
Draw-HazardLava
Draw-ArenaBoundary

$preview = Draw-Preview
Write-Output "Generated polished non-character assets in $SkinDir"
Write-Output "Preview: $preview"
if ($DidCreateBackup) {
    Write-Output "Backup: $BackupDir"
}
