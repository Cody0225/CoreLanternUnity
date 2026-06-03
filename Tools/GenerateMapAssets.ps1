Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$BackupDir = Join-Path $ProjectRoot "Assets\ArtSource\MapAssetBackup_20260519"

New-Item -ItemType Directory -Force -Path $SkinDir | Out-Null
New-Item -ItemType Directory -Force -Path $BackupDir | Out-Null

function New-Rgba {
    param([int]$A, [int]$R, [int]$G, [int]$B)
    return [System.Drawing.Color]::FromArgb(
        [Math]::Max(0, [Math]::Min(255, $A)),
        [Math]::Max(0, [Math]::Min(255, $R)),
        [Math]::Max(0, [Math]::Min(255, $G)),
        [Math]::Max(0, [Math]::Min(255, $B))
    )
}

function New-Bitmap {
    param([int]$W, [int]$H, [switch]$Transparent)
    $bmp = New-Object System.Drawing.Bitmap($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    if ($Transparent) {
        $g.Clear([System.Drawing.Color]::FromArgb(0, 0, 0, 0))
    } else {
        $g.Clear((New-Rgba 255 3 8 14))
    }
    return @{ Bitmap = $bmp; Graphics = $g }
}

function Save-Bitmap {
    param($Canvas, [string]$Name)
    $path = Join-Path $SkinDir $Name
    $backupPath = Join-Path $BackupDir $Name
    if ((Test-Path -LiteralPath $path) -and -not (Test-Path -LiteralPath $backupPath)) {
        Copy-Item -LiteralPath $path -Destination $backupPath
    }
    $Canvas.Bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
    return $path
}

function New-Pen {
    param([System.Drawing.Color]$Color, [float]$Width = 1)
    $pen = New-Object System.Drawing.Pen($Color, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    return $pen
}

function Fill-Polygon {
    param($G, [System.Drawing.Color]$Color, [System.Drawing.PointF[]]$Points)
    $brush = New-Object System.Drawing.SolidBrush($Color)
    $G.FillPolygon($brush, $Points)
    $brush.Dispose()
}

function Fill-Ellipse {
    param($G, [System.Drawing.Color]$Color, [float]$X, [float]$Y, [float]$W, [float]$H)
    $brush = New-Object System.Drawing.SolidBrush($Color)
    $G.FillEllipse($brush, $X, $Y, $W, $H)
    $brush.Dispose()
}

function Fill-Rect {
    param($G, [System.Drawing.Color]$Color, [float]$X, [float]$Y, [float]$W, [float]$H)
    $brush = New-Object System.Drawing.SolidBrush($Color)
    $G.FillRectangle($brush, $X, $Y, $W, $H)
    $brush.Dispose()
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in @(5, 3, 1)) {
        $alpha = [int]($Color.A / ($mul + 0.8))
        $pen = New-Pen (New-Rgba $alpha $Color.R $Color.G $Color.B) ($Width * $mul)
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2)
    foreach ($mul in @(5, 3, 1)) {
        $alpha = [int]($Color.A / ($mul + 0.8))
        $pen = New-Pen (New-Rgba $alpha $Color.R $Color.G $Color.B) ($Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = New-Pen $Color $Width
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Draw-CircuitGrid {
    param($G, [int]$Size, [int]$Seed, [System.Drawing.Color]$Accent)
    $rng = [System.Random]::new($Seed)
    for ($i = 0; $i -le $Size; $i += 16) {
        $major = ($i % 64) -eq 0
        $a = if ($major) { 42 } else { 18 }
        $width = if ($major) { 1.5 } else { 1 }
        $pen = New-Pen (New-Rgba $a 42 188 216) $width
        $G.DrawLine($pen, $i, 0, $i, $Size)
        $G.DrawLine($pen, 0, $i, $Size, $i)
        $pen.Dispose()
    }

    for ($i = 0; $i -lt 18; $i++) {
        $x = $rng.Next(8, $Size - 32)
        $y = $rng.Next(8, $Size - 32)
        $w = $rng.Next(18, 58)
        $h = $rng.Next(8, 28)
        $pen = New-Pen (New-Rgba $rng.Next(24, 72) $Accent.R $Accent.G $Accent.B) 2
        $G.DrawRectangle($pen, $x, $y, $w, $h)
        $pen.Dispose()
        if ($rng.NextDouble() -lt 0.5) {
            Draw-GlowLine $G ($x + $w * 0.5) ($y + $h * 0.5) ($x + $rng.Next(-28, 48)) ($y + $rng.Next(-28, 48)) (New-Rgba 80 $Accent.R $Accent.G $Accent.B) 1.2
        }
    }
}

function New-FloorTile {
    param([string]$Name, [int]$Seed, [System.Drawing.Color]$Accent)
    $c = New-Bitmap 128 128
    $bmp = $c.Bitmap
    for ($y = 0; $y -lt 128; $y++) {
        for ($x = 0; $x -lt 128; $x++) {
            $d = [Math]::Sqrt([Math]::Pow(($x - 64) / 64.0, 2) + [Math]::Pow(($y - 64) / 64.0, 2))
            $grain = (($x * 17 + $y * 31 + $Seed) % 23)
            $base = [int](12 + (1 - [Math]::Min(1, $d)) * 15 + $grain * 0.35)
            $bmp.SetPixel($x, $y, (New-Rgba 255 ([int]($base * 0.42)) ([int]($base * 0.8)) ([int]($base * 1.1))))
        }
    }
    Draw-CircuitGrid $c.Graphics 128 $Seed $Accent
    for ($i = 0; $i -lt 12; $i++) {
        $rng = [System.Random]::new($Seed + $i * 71)
        Draw-GlowLine $c.Graphics $rng.Next(8, 120) $rng.Next(8, 120) $rng.Next(8, 120) $rng.Next(8, 120) (New-Rgba 70 $Accent.R $Accent.G $Accent.B) 1.3
    }
    Save-Bitmap $c $Name
}

function New-DarkBase {
    $c = New-Bitmap 512 512
    $bmp = $c.Bitmap
    for ($y = 0; $y -lt 512; $y++) {
        for ($x = 0; $x -lt 512; $x++) {
            $nx = ($x - 256) / 256.0
            $ny = ($y - 256) / 256.0
            $d = [Math]::Sqrt($nx * $nx + $ny * $ny)
            $grid = if ((($x % 32) -eq 0) -or (($y % 32) -eq 0)) { 9 } else { 0 }
            $noise = (($x * 13 + $y * 29) % 19)
            $r = [int](2 + $grid + $noise * 0.25)
            $g = [int](8 + (1 - [Math]::Min(1, $d)) * 14 + $grid + $noise * 0.35)
            $b = [int](14 + (1 - [Math]::Min(1, $d)) * 22 + $grid * 2 + $noise * 0.45)
            $bmp.SetPixel($x, $y, (New-Rgba 255 $r $g $b))
        }
    }
    $g = $c.Graphics
    for ($i = 0; $i -lt 24; $i++) {
        $p = New-Pen (New-Rgba 24 38 210 238) 1
        $g.DrawLine($p, $i * 22, 0, $i * 22 - 180, 512)
        $p.Dispose()
    }
    Save-Bitmap $c "Floor_DarkBase_A.png"
}

function New-GridOverlay {
    $c = New-Bitmap 512 512 -Transparent
    $g = $c.Graphics
    for ($i = 0; $i -le 512; $i += 32) {
        $major = ($i % 128) -eq 0
        $color = if ($major) { New-Rgba 82 39 222 246 } else { New-Rgba 34 39 156 186 }
        $width = if ($major) { 2 } else { 1 }
        $pen = New-Pen $color $width
        $g.DrawLine($pen, $i, 0, $i, 512)
        $g.DrawLine($pen, 0, $i, 512, $i)
        $pen.Dispose()
    }
    for ($i = 0; $i -lt 42; $i++) {
        $x = (($i * 47) % 480) + 16
        $y = (($i * 83) % 480) + 16
        Fill-Rect $g (New-Rgba 46 44 229 255) $x $y 5 5
    }
    Save-Bitmap $c "Floor_GridOverlay_A.png"
}

function New-Crack {
    param([string]$Name, [int]$Seed)
    $c = New-Bitmap 192 192 -Transparent
    $g = $c.Graphics
    $rng = [System.Random]::new($Seed)
    for ($branch = 0; $branch -lt 8; $branch++) {
        $x = 96 + $rng.Next(-24, 24)
        $y = 96 + $rng.Next(-24, 24)
        for ($seg = 0; $seg -lt $rng.Next(3, 7); $seg++) {
            $nx = $x + $rng.Next(-44, 45)
            $ny = $y + $rng.Next(-44, 45)
            Draw-GlowLine $g $x $y $nx $ny (New-Rgba 72 38 229 255) 1.2
            $shadow = New-Pen (New-Rgba 105 0 6 10) 3
            $g.DrawLine($shadow, $x + 1, $y + 1, $nx + 1, $ny + 1)
            $shadow.Dispose()
            $x = $nx
            $y = $ny
        }
    }
    Save-Bitmap $c $Name
}

function New-Cable {
    param([string]$Name, [int]$Seed, [System.Drawing.Color]$Accent)
    $c = New-Bitmap 192 192 -Transparent
    $g = $c.Graphics
    $rng = [System.Random]::new($Seed)
    for ($i = 0; $i -lt 5; $i++) {
        $y = 28 + $i * 28 + $rng.Next(-8, 9)
        $x1 = $rng.Next(8, 38)
        $x2 = $rng.Next(150, 186)
        Draw-GlowLine $g $x1 $y (($x1 + $x2) / 2) ($y + $rng.Next(-18, 19)) (New-Rgba 100 $Accent.R $Accent.G $Accent.B) 3
        Draw-GlowLine $g (($x1 + $x2) / 2) ($y + $rng.Next(-18, 19)) $x2 ($y + $rng.Next(-14, 15)) (New-Rgba 80 $Accent.R $Accent.G $Accent.B) 2.4
        Fill-Ellipse $g (New-Rgba 190 $Accent.R $Accent.G $Accent.B) ($x1 - 4) ($y - 4) 8 8
        Fill-Ellipse $g (New-Rgba 150 $Accent.R $Accent.G $Accent.B) ($x2 - 4) ($y - 4) 8 8
    }
    Save-Bitmap $c $Name
}

function New-CoreMark {
    $c = New-Bitmap 256 256 -Transparent
    $g = $c.Graphics
    Draw-GlowEllipse $g 26 26 204 204 (New-Rgba 130 255 205 38) 2
    Draw-GlowEllipse $g 58 58 140 140 (New-Rgba 100 40 226 255) 1.8
    Draw-GlowEllipse $g 91 91 74 74 (New-Rgba 130 255 220 82) 1.4
    for ($i = 0; $i -lt 24; $i++) {
        $a = [Math]::PI * 2 * $i / 24
        $x1 = 128 + [Math]::Cos($a) * 72
        $y1 = 128 + [Math]::Sin($a) * 72
        $x2 = 128 + [Math]::Cos($a) * 103
        $y2 = 128 + [Math]::Sin($a) * 103
        Draw-GlowLine $g $x1 $y1 $x2 $y2 (New-Rgba 75 255 204 43) 1.1
    }
    Save-Bitmap $c "Floor_CoreMark_A.png"
}

function New-BoundaryStone {
    param([string]$Name, [int]$Seed, [System.Drawing.Color]$Accent)
    $c = New-Bitmap 128 128 -Transparent
    $g = $c.Graphics
    $pts = @(
        [System.Drawing.PointF]::new(28, 38),
        [System.Drawing.PointF]::new(82, 22),
        [System.Drawing.PointF]::new(112, 66),
        [System.Drawing.PointF]::new(73, 111),
        [System.Drawing.PointF]::new(21, 91)
    )
    Fill-Polygon $g (New-Rgba 238 19 29 36) $pts
    $pen = New-Pen (New-Rgba 170 $Accent.R $Accent.G $Accent.B) 3
    $g.DrawPolygon($pen, $pts)
    $pen.Dispose()
    Draw-GlowLine $g 41 58 88 47 (New-Rgba 100 $Accent.R $Accent.G $Accent.B) 2
    Draw-GlowLine $g 46 78 92 78 (New-Rgba 80 $Accent.R $Accent.G $Accent.B) 2
    Fill-Rect $g (New-Rgba 140 $Accent.R $Accent.G $Accent.B) 57 55 12 12
    Save-Bitmap $c $Name
}

function New-ServerDebris {
    param([string]$Name, [int]$Seed, [System.Drawing.Color]$Accent)
    $c = New-Bitmap 160 160 -Transparent
    $g = $c.Graphics
    $rng = [System.Random]::new($Seed)
    for ($i = 0; $i -lt 5; $i++) {
        $x = $rng.Next(18, 98)
        $y = $rng.Next(28, 115)
        $w = $rng.Next(38, 78)
        $h = $rng.Next(18, 42)
        Fill-Rect $g (New-Rgba 225 14 26 34) $x $y $w $h
        $pen = New-Pen (New-Rgba 120 70 100 110) 2
        $g.DrawRectangle($pen, $x, $y, $w, $h)
        $pen.Dispose()
        Draw-GlowLine $g ($x + 10) ($y + $h * 0.5) ($x + $w - 10) ($y + $h * 0.5) (New-Rgba 90 $Accent.R $Accent.G $Accent.B) 1.2
    }
    Save-Bitmap $c $Name
}

function New-NeonPylon {
    $c = New-Bitmap 128 128 -Transparent
    $g = $c.Graphics
    Fill-Ellipse $g (New-Rgba 130 0 0 0) 25 88 78 24
    Fill-Rect $g (New-Rgba 230 17 32 38) 44 28 40 66
    $pen = New-Pen (New-Rgba 180 72 241 255) 3
    $g.DrawRectangle($pen, 44, 28, 40, 66)
    $pen.Dispose()
    Draw-GlowLine $g 64 18 64 92 (New-Rgba 210 78 248 255) 2.4
    Draw-GlowEllipse $g 39 15 50 22 (New-Rgba 110 91 255 210) 2
    Save-Bitmap $c "Prop_NeonPylon_A.png"
}

function New-DataTerminal {
    $c = New-Bitmap 128 128 -Transparent
    $g = $c.Graphics
    Fill-Ellipse $g (New-Rgba 110 0 0 0) 22 90 84 20
    Fill-Rect $g (New-Rgba 230 13 28 34) 28 45 72 43
    $pen = New-Pen (New-Rgba 160 74 255 164) 2
    $g.DrawRectangle($pen, 28, 45, 72, 43)
    $pen.Dispose()
    Fill-Rect $g (New-Rgba 80 46 255 169) 41 54 44 7
    Fill-Rect $g (New-Rgba 70 46 215 255) 43 68 20 5
    Draw-GlowLine $g 64 88 64 104 (New-Rgba 100 46 255 169) 2
    Save-Bitmap $c "Prop_DataTerminal_A.png"
}

function New-Crate {
    $c = New-Bitmap 128 128 -Transparent
    $g = $c.Graphics
    Fill-Rect $g (New-Rgba 225 18 27 31) 28 31 72 66
    $pen = New-Pen (New-Rgba 120 107 128 126) 2
    $g.DrawRectangle($pen, 28, 31, 72, 66)
    $pen.Dispose()
    Draw-GlowLine $g 38 47 90 47 (New-Rgba 65 30 220 240) 1.2
    Draw-GlowLine $g 39 76 89 76 (New-Rgba 50 255 197 45) 1.2
    Save-Bitmap $c "Prop_Crate_A.png"
}

function New-CoreParts {
    $platform = New-Bitmap 256 256 -Transparent
    $g = $platform.Graphics
    Fill-Ellipse $g (New-Rgba 70 0 0 0) 34 160 188 44
    Draw-GlowEllipse $g 44 44 168 168 (New-Rgba 95 255 204 42) 2
    Draw-GlowEllipse $g 65 65 126 126 (New-Rgba 95 47 233 255) 2
    Fill-Ellipse $g (New-Rgba 72 255 204 42) 92 92 72 72
    Save-Bitmap $platform "Core_Platform.png"

    $outer = New-Bitmap 256 256 -Transparent
    Draw-GlowEllipse $outer.Graphics 23 23 210 210 (New-Rgba 190 255 202 37) 5
    Draw-GlowEllipse $outer.Graphics 43 43 170 170 (New-Rgba 85 42 220 255) 2
    Save-Bitmap $outer "Core_RingOuter.png"

    $inner = New-Bitmap 256 256 -Transparent
    Draw-GlowEllipse $inner.Graphics 72 72 112 112 (New-Rgba 175 255 223 86) 4
    Draw-GlowEllipse $inner.Graphics 95 95 66 66 (New-Rgba 120 45 230 255) 2
    Save-Bitmap $inner "Core_RingInner.png"
}

function New-DamageCrack {
    param([string]$Name, [int]$Seed, [int]$Alpha)
    $c = New-Bitmap 256 256 -Transparent
    $g = $c.Graphics
    $rng = [System.Random]::new($Seed)
    for ($i = 0; $i -lt 12; $i++) {
        $a = [Math]::PI * 2 * $i / 12 + $rng.NextDouble() * 0.3
        $x1 = 128 + [Math]::Cos($a) * $rng.Next(18, 45)
        $y1 = 128 + [Math]::Sin($a) * $rng.Next(18, 45)
        $x2 = 128 + [Math]::Cos($a) * $rng.Next(72, 117)
        $y2 = 128 + [Math]::Sin($a) * $rng.Next(72, 117)
        Draw-GlowLine $g $x1 $y1 $x2 $y2 (New-Rgba $Alpha 255 66 44) 1.5
    }
    Save-Bitmap $c $Name
}

function New-SpawnRing {
    param([string]$Name, [System.Drawing.Color]$Accent)
    $c = New-Bitmap 192 192 -Transparent
    $g = $c.Graphics
    Draw-GlowEllipse $g 34 34 124 124 (New-Rgba 150 $Accent.R $Accent.G $Accent.B) 3
    for ($i = 0; $i -lt 12; $i++) {
        $a = [Math]::PI * 2 * $i / 12
        $x = 96 + [Math]::Cos($a) * 70
        $y = 96 + [Math]::Sin($a) * 70
        Fill-Rect $g (New-Rgba 150 $Accent.R $Accent.G $Accent.B) ($x - 4) ($y - 4) 8 8
    }
    Save-Bitmap $c $Name
}

function New-PreviewSheet {
    $files = @(
        "Floor_TileA.png", "Floor_TileB.png", "Floor_TileC.png", "Floor_GridOverlay_A.png",
        "Floor_Crack_A.png", "Floor_Cable_A.png", "Floor_CoreMark_A.png", "Boundary_Stone_A.png",
        "Prop_ServerDebris_A.png", "Prop_NeonPylon_A.png", "Prop_DataTerminal_A.png", "Prop_Crate_A.png",
        "Core_Platform.png", "Core_RingOuter.png", "Core_RingInner.png", "Spawn_Ring_Normal.png"
    )

    $sheet = New-Bitmap 1024 720
    $g = $sheet.Graphics
    $g.Clear((New-Rgba 255 2 9 14))
    $titleFont = New-Object System.Drawing.Font("Arial", 18, [System.Drawing.FontStyle]::Bold)
    $font = New-Object System.Drawing.Font("Arial", 9, [System.Drawing.FontStyle]::Regular)
    $brush = New-Object System.Drawing.SolidBrush((New-Rgba 230 180 255 255))
    $g.DrawString("CoreLantern MAP Asset Preview", $titleFont, $brush, 24, 20)

    for ($i = 0; $i -lt $files.Count; $i++) {
        $file = $files[$i]
        $srcPath = Join-Path $SkinDir $file
        if (-not (Test-Path -LiteralPath $srcPath)) { continue }

        $col = $i % 4
        $row = [Math]::Floor($i / 4)
        $x = 34 + $col * 244
        $y = 72 + $row * 152
        Fill-Rect $g (New-Rgba 255 8 24 31) $x $y 208 118
        $pen = New-Pen (New-Rgba 170 31 214 238) 1
        $g.DrawRectangle($pen, $x, $y, 208, 118)
        $pen.Dispose()

        $img = [System.Drawing.Image]::FromFile($srcPath)
        $max = 86.0
        $scale = [Math]::Min($max / $img.Width, $max / $img.Height)
        $dw = [int]($img.Width * $scale)
        $dh = [int]($img.Height * $scale)
        $dx = [int]($x + 104 - $dw / 2)
        $dy = [int]($y + 10 + 46 - $dh / 2)
        $g.DrawImage($img, $dx, $dy, $dw, $dh)
        $img.Dispose()
        $g.DrawString($file, $font, $brush, $x + 10, $y + 96)
    }

    $previewPath = Join-Path $ProjectRoot "Assets\ArtSource\Generated_MapAsset_Preview.png"
    $sheet.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $font.Dispose()
    $titleFont.Dispose()
    $brush.Dispose()
    $sheet.Graphics.Dispose()
    $sheet.Bitmap.Dispose()
    return $previewPath
}

New-FloorTile "Floor_TileA.png" 1101 (New-Rgba 130 38 223 255)
New-FloorTile "Floor_TileB.png" 2211 (New-Rgba 120 94 255 180)
New-FloorTile "Floor_TileC.png" 3311 (New-Rgba 115 255 198 47)
New-DarkBase
New-GridOverlay
New-Crack "Floor_Crack_A.png" 404
New-Crack "Floor_Crack_B.png" 405
New-Cable "Floor_Cable_A.png" 501 (New-Rgba 140 38 223 255)
New-Cable "Floor_Cable_B.png" 502 (New-Rgba 135 90 255 180)
New-CoreMark
New-BoundaryStone "Boundary_Stone_A.png" 601 (New-Rgba 160 38 223 255)
New-BoundaryStone "Boundary_Stone_B.png" 602 (New-Rgba 150 255 198 47)
New-ServerDebris "Prop_ServerDebris_A.png" 701 (New-Rgba 110 38 223 255)
New-ServerDebris "Prop_ServerDebris_B.png" 702 (New-Rgba 120 255 198 47)
New-NeonPylon
New-DataTerminal
New-Crate
New-CoreParts
New-DamageCrack "Core_DamageCrack_1.png" 801 80
New-DamageCrack "Core_DamageCrack_2.png" 802 115
New-DamageCrack "Core_DamageCrack_3.png" 803 155
New-SpawnRing "Spawn_Ring_Normal.png" (New-Rgba 150 38 223 255)
New-SpawnRing "Spawn_Ring_Boss.png" (New-Rgba 170 255 48 221)
New-PreviewSheet

Get-ChildItem -LiteralPath $SkinDir -File |
    Where-Object { $_.Name -match '^(Floor|Boundary|Prop|Core|Spawn)' } |
    Sort-Object Name |
    Select-Object Name, Length
