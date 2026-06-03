param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms

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
    param([int]$W = 256, [int]$H = 256)
    $bmp = [System.Drawing.Bitmap]::new($W, $H, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    return @{ Bitmap = $bmp; Graphics = $g }
}

function Load-BitmapClone {
    param([string]$Path)
    $img = [System.Drawing.Image]::FromFile($Path)
    try {
        $clone = [System.Drawing.Bitmap]::new($img.Width, $img.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $g = [System.Drawing.Graphics]::FromImage($clone)
        $g.DrawImage($img, 0, 0, $img.Width, $img.Height)
        $g.Dispose()
        return $clone
    }
    finally {
        $img.Dispose()
    }
}

function Draw-ImageAlpha {
    param($G, [System.Drawing.Image]$Image, [System.Drawing.RectangleF]$Rect, [double]$Alpha = 1.0)
    if ($Alpha -ge 0.999) {
        $G.DrawImage($Image, $Rect)
        return
    }

    $dest = [System.Drawing.Rectangle]::new(
        [int][Math]::Round($Rect.X),
        [int][Math]::Round($Rect.Y),
        [int][Math]::Round($Rect.Width),
        [int][Math]::Round($Rect.Height)
    )
    $cm = [System.Drawing.Imaging.ColorMatrix]::new()
    $cm.Matrix33 = [single]$Alpha
    $ia = [System.Drawing.Imaging.ImageAttributes]::new()
    $ia.SetColorMatrix($cm)
    $G.DrawImage($Image, $dest, 0, 0, $Image.Width, $Image.Height, [System.Drawing.GraphicsUnit]::Pixel, $ia)
    $ia.Dispose()
}

function Draw-GlowLine {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 2.0)
    foreach ($mul in 5, 3, 2) {
        $alpha = [Math]::Max(8, [int]($Color.A / ($mul + 1.1)))
        $pen = [System.Drawing.Pen]::new((With-Alpha $Color $alpha), $Width * $mul)
        $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
        $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
        $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
        $pen.Dispose()
    }
    $core = [System.Drawing.Pen]::new($Color, $Width)
    $core.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $core.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $G.DrawLine($core, $X1, $Y1, $X2, $Y2)
    $core.Dispose()
}

function Draw-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 2.0)
    foreach ($mul in 5, 3, 2) {
        $alpha = [Math]::Max(6, [int]($Color.A / ($mul + 1.4)))
        $pen = [System.Drawing.Pen]::new((With-Alpha $Color $alpha), $Width * $mul)
        $G.DrawEllipse($pen, $X, $Y, $W, $H)
        $pen.Dispose()
    }
    $core = [System.Drawing.Pen]::new($Color, $Width)
    $G.DrawEllipse($core, $X, $Y, $W, $H)
    $core.Dispose()
}

function Fill-GlowEllipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color)
    foreach ($mul in 2.0, 1.45, 1.1) {
        $a = [Math]::Max(8, [int]($Color.A / ($mul * 3.2)))
        $brush = [System.Drawing.SolidBrush]::new((With-Alpha $Color $a))
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
    param($G, [object[]]$Points, [System.Drawing.Color]$Fill, [System.Drawing.Color]$Stroke)
    $pts = @()
    foreach ($p in $Points) {
        $pts += [System.Drawing.PointF]::new([float]$p[0], [float]$p[1])
    }
    $brush = [System.Drawing.SolidBrush]::new($Fill)
    $G.FillPolygon($brush, $pts)
    $brush.Dispose()
    $pen = [System.Drawing.Pen]::new($Stroke, 2.2)
    $G.DrawPolygon($pen, $pts)
    $pen.Dispose()
}

function Save-Bitmap {
    param([System.Drawing.Bitmap]$Bitmap, [string]$Path)
    $Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
}

$SpeciesNames = @{
    1 = "Cobalt Pup"
    2 = "Ember Drake"
    3 = "Sage Hare"
    4 = "Hex Cat"
    5 = "Drift Fox"
    6 = "Iron Bear"
}

$RouteNames = @{
    1 = "SPEED"
    2 = "POWER"
    3 = "GUARD"
}

$RouteAccents = @{
    1 = (New-Color 55 225 255 230)
    2 = (New-Color 255 154 34 230)
    3 = (New-Color 105 255 116 230)
}

$FusionAccents = @{
    1 = (New-Color 255 76 238 235)
    2 = (New-Color 150 255 60 235)
    3 = (New-Color 255 210 58 235)
    4 = (New-Color 72 224 255 235)
    5 = (New-Color 190 120 255 235)
    6 = (New-Color 88 255 200 235)
}

function Draw-RouteUnderlay {
    param($G, [int]$Route, [System.Drawing.Color]$Accent)
    Draw-GlowEllipse $G 45 48 166 166 (With-Alpha $Accent 70) 2.5
    if ($Route -eq 1) {
        Draw-GlowLine $G 31 188 216 63 (With-Alpha $Accent 116) 2.6
        Draw-GlowLine $G 41 210 224 94 (With-Alpha $Accent 82) 1.7
        Draw-GlowLine $G 23 151 194 35 (With-Alpha $Accent 72) 1.4
    }
    elseif ($Route -eq 2) {
        Fill-GlowEllipse $G 89 34 78 78 (With-Alpha $Accent 45)
        Draw-GlowEllipse $G 69 63 118 118 (With-Alpha $Accent 86) 3.1
        Draw-GlowLine $G 78 213 178 213 (With-Alpha $Accent 80) 3.4
    }
    else {
        Draw-GlowEllipse $G 35 38 186 186 (With-Alpha $Accent 105) 3.2
        Draw-GlowEllipse $G 63 66 130 130 (With-Alpha $Accent 74) 2.0
    }
}

function Draw-RouteTop {
    param($G, [int]$Route, [System.Drawing.Color]$Accent)
    $white = New-Color 235 255 255 165
    if ($Route -eq 1) {
        Draw-Poly $G @(@(35,112),@(82,96),@(62,126),@(22,144)) (With-Alpha $Accent 95) (With-Alpha $white 135)
        Draw-Poly $G @(@(221,112),@(174,96),@(194,126),@(234,144)) (With-Alpha $Accent 95) (With-Alpha $white 135)
        Draw-GlowLine $G 68 151 31 184 (With-Alpha $Accent 190) 2.2
        Draw-GlowLine $G 188 151 225 184 (With-Alpha $Accent 190) 2.2
        Fill-GlowEllipse $G 77 184 13 13 (With-Alpha $Accent 185)
        Fill-GlowEllipse $G 166 184 13 13 (With-Alpha $Accent 185)
    }
    elseif ($Route -eq 2) {
        Draw-Poly $G @(@(111,39),@(128,12),@(145,39),@(136,65),@(120,65)) (With-Alpha $Accent 128) (With-Alpha $white 140)
        Draw-Poly $G @(@(42,104),@(76,91),@(86,130),@(48,142)) (With-Alpha $Accent 105) (With-Alpha $white 110)
        Draw-Poly $G @(@(214,104),@(180,91),@(170,130),@(208,142)) (With-Alpha $Accent 105) (With-Alpha $white 110)
        Draw-GlowLine $G 62 106 42 88 (With-Alpha $Accent 180) 4.5
        Draw-GlowLine $G 194 106 214 88 (With-Alpha $Accent 180) 4.5
        Fill-GlowEllipse $G 93 202 19 19 (With-Alpha $Accent 175)
        Fill-GlowEllipse $G 144 202 19 19 (With-Alpha $Accent 175)
    }
    else {
        Draw-Poly $G @(@(42,118),@(72,99),@(78,158),@(46,170)) (With-Alpha $Accent 92) (With-Alpha $white 115)
        Draw-Poly $G @(@(214,118),@(184,99),@(178,158),@(210,170)) (With-Alpha $Accent 92) (With-Alpha $white 115)
        Draw-GlowEllipse $G 62 50 132 155 (With-Alpha $Accent 160) 3.0
        Draw-Poly $G @(@(128,175),@(161,194),@(146,221),@(110,221),@(95,194)) (With-Alpha $Accent 88) (With-Alpha $white 130)
        Fill-GlowEllipse $G 117 111 22 22 (With-Alpha $Accent 180)
    }
}

function New-RouteL0 {
    param([int]$Species, [int]$Route)
    $outName = "Partner_S${Species}_R${Route}_L0.png"
    $outPath = Join-Path $SkinDir $outName
    if ((Test-Path -LiteralPath $outPath) -and -not $Force) {
        return $false
    }

    $basePath = Join-Path $SkinDir "Partner_S${Species}_L0.png"
    if (-not (Test-Path -LiteralPath $basePath)) {
        throw "Missing base species sprite: $basePath"
    }

    $accent = $RouteAccents[$Route]
    $canvas = New-Canvas 256 256
    $base = Load-BitmapClone $basePath
    try {
        Draw-RouteUnderlay $canvas.Graphics $Route $accent
        Draw-ImageAlpha $canvas.Graphics $base ([System.Drawing.RectangleF]::new(0, 0, 256, 256)) 1.0
        Draw-RouteTop $canvas.Graphics $Route $accent
        Save-Bitmap $canvas.Bitmap $outPath
    }
    finally {
        $base.Dispose()
        $canvas.Graphics.Dispose()
        $canvas.Bitmap.Dispose()
    }
    return $true
}

function Get-FusionBaseRoute {
    param([int]$Fusion)
    switch ($Fusion) {
        1 { return 3 }
        2 { return 1 }
        3 { return 3 }
        4 { return 1 }
        5 { return 3 }
        6 { return 1 }
        default { return 1 }
    }
}

function Draw-FusionUnderlay {
    param($G, [int]$Fusion, [System.Drawing.Color]$Accent)
    Draw-GlowEllipse $G 22 20 212 212 (With-Alpha $Accent 82) 3.2
    Draw-GlowEllipse $G 48 46 160 160 (With-Alpha $Accent 64) 2.1
    if ($Fusion -eq 4 -or $Fusion -eq 6) {
        Draw-GlowLine $G 40 208 216 48 (With-Alpha $Accent 92) 2.2
        Draw-GlowLine $G 26 166 184 32 (With-Alpha $Accent 70) 1.6
    }
    elseif ($Fusion -eq 3 -or $Fusion -eq 5) {
        Draw-GlowEllipse $G 54 58 148 148 (With-Alpha $Accent 112) 4.2
    }
    else {
        Draw-GlowLine $G 54 62 202 196 (With-Alpha $Accent 76) 2.2
        Draw-GlowLine $G 202 62 54 196 (With-Alpha $Accent 76) 2.2
    }
}

function Draw-FusionTop {
    param($G, [int]$Fusion, [System.Drawing.Color]$Accent)
    $white = New-Color 235 255 255 175
    switch ($Fusion) {
        1 {
            Draw-Poly $G @(@(29,108),@(72,86),@(89,117),@(48,148)) (With-Alpha $Accent 118) (With-Alpha $white 145)
            Draw-Poly $G @(@(227,108),@(184,86),@(167,117),@(208,148)) (With-Alpha $Accent 118) (With-Alpha $white 145)
            Draw-GlowEllipse $G 66 66 124 124 (With-Alpha (New-Color 92 240 255 235) 150) 3.0
            Fill-GlowEllipse $G 120 118 16 16 (With-Alpha $Accent 210)
        }
        2 {
            foreach ($p in @(@(62,84),@(194,84),@(62,177),@(194,177),@(128,42),@(128,214))) {
                Fill-GlowEllipse $G ($p[0]-8) ($p[1]-8) 16 16 (With-Alpha $Accent 180)
                Draw-GlowLine $G 128 128 $p[0] $p[1] (With-Alpha $Accent 105) 1.6
            }
            Draw-GlowEllipse $G 83 83 90 90 (With-Alpha $Accent 135) 2.5
        }
        3 {
            Draw-Poly $G @(@(56,63),@(200,63),@(219,129),@(188,205),@(68,205),@(37,129)) (With-Alpha $Accent 32) (With-Alpha $Accent 155)
            Draw-Poly $G @(@(128,87),@(167,111),@(154,158),@(102,158),@(89,111)) (With-Alpha $Accent 62) (With-Alpha $white 118)
            Fill-GlowEllipse $G 116 116 24 24 (With-Alpha $Accent 215)
        }
        4 {
            Draw-GlowLine $G 29 201 218 78 (With-Alpha $Accent 170) 2.6
            Draw-GlowLine $G 41 220 229 111 (With-Alpha (New-Color 190 95 255 220) 138) 1.9
            Draw-GlowEllipse $G 51 48 154 154 (With-Alpha (New-Color 190 95 255 220) 135) 2.2
            Fill-GlowEllipse $G 116 116 24 24 (With-Alpha $Accent 190)
        }
        5 {
            Draw-GlowEllipse $G 44 54 168 148 (With-Alpha $Accent 160) 3.4
            Draw-Poly $G @(@(44,133),@(82,74),@(111,111),@(77,177)) (With-Alpha $Accent 82) (With-Alpha $white 130)
            Draw-Poly $G @(@(212,133),@(174,74),@(145,111),@(179,177)) (With-Alpha (New-Color 255 218 60 210) 82) (With-Alpha $white 130)
            Draw-GlowLine $G 81 209 181 47 (With-Alpha $Accent 92) 2.0
        }
        6 {
            Draw-GlowEllipse $G 58 40 140 172 (With-Alpha $Accent 145) 3.0
            Draw-GlowLine $G 68 70 31 116 (With-Alpha (New-Color 160 90 255 220) 125) 2.5
            Draw-GlowLine $G 188 70 225 116 (With-Alpha (New-Color 160 90 255 220) 125) 2.5
            foreach ($p in @(@(78,188),@(128,205),@(178,188))) {
                Fill-GlowEllipse $G ($p[0]-7) ($p[1]-7) 14 14 (With-Alpha $Accent 170)
            }
        }
    }
}

function New-FusionL3 {
    param([int]$Species, [int]$Fusion)
    $outName = "Partner_S${Species}_F${Fusion}_L3.png"
    $outPath = Join-Path $SkinDir $outName
    if ((Test-Path -LiteralPath $outPath) -and -not $Force) {
        return $false
    }

    $route = Get-FusionBaseRoute $Fusion
    $basePath = Join-Path $SkinDir "Partner_S${Species}_R${route}_L3.png"
    if (-not (Test-Path -LiteralPath $basePath)) {
        $basePath = Join-Path $SkinDir "Partner_S${Species}_L0.png"
    }
    if (-not (Test-Path -LiteralPath $basePath)) {
        throw "Missing fusion source sprite: $basePath"
    }

    $accent = $FusionAccents[$Fusion]
    $canvas = New-Canvas 256 256
    $base = Load-BitmapClone $basePath
    try {
        Draw-FusionUnderlay $canvas.Graphics $Fusion $accent
        if ($Fusion -eq 4 -or $Fusion -eq 6) {
            Draw-ImageAlpha $canvas.Graphics $base ([System.Drawing.RectangleF]::new(-10, 7, 256, 256)) 0.18
            Draw-ImageAlpha $canvas.Graphics $base ([System.Drawing.RectangleF]::new(10, -5, 256, 256)) 0.16
        }
        Draw-ImageAlpha $canvas.Graphics $base ([System.Drawing.RectangleF]::new(0, 0, 256, 256)) 1.0
        Draw-FusionTop $canvas.Graphics $Fusion $accent
        Save-Bitmap $canvas.Bitmap $outPath
    }
    finally {
        $base.Dispose()
        $canvas.Graphics.Dispose()
        $canvas.Bitmap.Dispose()
    }
    return $true
}

function Draw-PreviewCell {
    param($G, [string]$Path, [int]$X, [int]$Y, [string]$Label, [System.Drawing.Color]$Accent)
    $bg = [System.Drawing.SolidBrush]::new((New-Color 4 18 24 255))
    $G.FillRectangle($bg, $X, $Y, 156, 176)
    $bg.Dispose()
    $pen = [System.Drawing.Pen]::new((With-Alpha $Accent 180), 2)
    $G.DrawRectangle($pen, $X, $Y, 156, 176)
    $pen.Dispose()
    if (Test-Path -LiteralPath $Path) {
        $img = Load-BitmapClone $Path
        try {
            Draw-ImageAlpha $G $img ([System.Drawing.RectangleF]::new($X + 18, $Y + 8, 120, 120)) 1.0
        }
        finally {
            $img.Dispose()
        }
    }
    $font = [System.Drawing.Font]::new("Segoe UI", 8.5, [System.Drawing.FontStyle]::Bold)
    $brush = [System.Drawing.SolidBrush]::new((New-Color 212 250 255 255))
    $sf = [System.Drawing.StringFormat]::new()
    $sf.Alignment = [System.Drawing.StringAlignment]::Center
    $G.DrawString($Label, $font, $brush, [System.Drawing.RectangleF]::new($X + 5, $Y + 132, 146, 36), $sf)
    $sf.Dispose()
    $brush.Dispose()
    $font.Dispose()
}

function New-PreviewSheet {
    $cols = 6
    $cellW = 170
    $cellH = 190
    $headerH = 64
    $routeRows = 3
    $fusionRows = 6
    $w = $cols * $cellW + 40
    $h = $headerH + ($routeRows + $fusionRows) * $cellH + 86
    $canvas = New-Canvas $w $h
    try {
        $bg = [System.Drawing.SolidBrush]::new((New-Color 2 10 15 255))
        $canvas.Graphics.FillRectangle($bg, 0, 0, $w, $h)
        $bg.Dispose()

        $titleFont = [System.Drawing.Font]::new("Segoe UI", 20, [System.Drawing.FontStyle]::Bold)
        $subFont = [System.Drawing.Font]::new("Segoe UI", 11, [System.Drawing.FontStyle]::Bold)
        $cyan = New-Color 145 255 255 255
        $gold = New-Color 255 220 86 255
        $brushCyan = [System.Drawing.SolidBrush]::new($cyan)
        $brushGold = [System.Drawing.SolidBrush]::new($gold)
        $canvas.Graphics.DrawString("Generated Missing Variant Assets", $titleFont, $brushCyan, 22, 14)
        $canvas.Graphics.DrawString("Route L0 gaps + species-specific Cross Evolve L3", $subFont, $brushGold, 24, 44)

        $y = $headerH
        foreach ($route in 1..3) {
            foreach ($species in 1..6) {
                $x = 20 + ($species - 1) * $cellW
                $path = Join-Path $SkinDir "Partner_S${species}_R${route}_L0.png"
                $label = "S$species $($RouteNames[$route]) L0"
                Draw-PreviewCell $canvas.Graphics $path $x $y $label $RouteAccents[$route]
            }
            $y += $cellH
        }

        foreach ($fusion in 1..6) {
            foreach ($species in 1..6) {
                $x = 20 + ($species - 1) * $cellW
                $path = Join-Path $SkinDir "Partner_S${species}_F${fusion}_L3.png"
                $label = "S$species F$fusion L3"
                Draw-PreviewCell $canvas.Graphics $path $x $y $label $FusionAccents[$fusion]
            }
            $y += $cellH
        }

        $titleFont.Dispose()
        $subFont.Dispose()
        $brushCyan.Dispose()
        $brushGold.Dispose()
        Save-Bitmap $canvas.Bitmap (Join-Path $ArtDir "Generated_MissingVariant_Preview.png")
    }
    finally {
        $canvas.Graphics.Dispose()
        $canvas.Bitmap.Dispose()
    }
}

$created = New-Object System.Collections.Generic.List[string]

foreach ($species in 1..6) {
    foreach ($route in 1..3) {
        if (New-RouteL0 $species $route) {
            $created.Add("Partner_S${species}_R${route}_L0.png")
        }
    }
}

foreach ($species in 1..6) {
    foreach ($fusion in 1..6) {
        if (New-FusionL3 $species $fusion) {
            $created.Add("Partner_S${species}_F${fusion}_L3.png")
        }
    }
}

New-PreviewSheet

Write-Host ("Created or overwritten: " + $created.Count)
$created | Sort-Object | ForEach-Object { Write-Host (" - " + $_) }
Write-Host ("Preview: " + (Join-Path $ArtDir "Generated_MissingVariant_Preview.png"))
