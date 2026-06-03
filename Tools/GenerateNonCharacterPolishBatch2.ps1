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
$BackupDir = Join-Path $ArtDir "NonCharacterPolishBatch2Backup_$stamp"
$DidCreateBackup = $false
$BackedUp = [System.Collections.Generic.HashSet[string]]::new()
$GeneratedFiles = [System.Collections.Generic.List[string]]::new()

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
    $p.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $p.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $p.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
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

function Draw-Rect {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    $p = Pen $Color $Width
    $G.DrawRectangle($p, $X, $Y, $W, $H)
    $p.Dispose()
}

function Draw-Ellipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    $p = Pen $Color $Width
    $G.DrawEllipse($p, $X, $Y, $W, $H)
    $p.Dispose()
}

function Glow-Line {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    foreach ($m in 4.8, 2.6) {
        $p = Pen (A $Color ([Math]::Max(6, [int]($Color.A / ($m + 1.6))))) ($Width * $m)
        $G.DrawLine($p, $X1, $Y1, $X2, $Y2)
        $p.Dispose()
    }
    Draw-Line $G $X1 $Y1 $X2 $Y2 $Color $Width
}

function Glow-Ellipse {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    foreach ($m in 4.5, 2.4) {
        $p = Pen (A $Color ([Math]::Max(6, [int]($Color.A / ($m + 1.7))))) ($Width * $m)
        $G.DrawEllipse($p, $X, $Y, $W, $H)
        $p.Dispose()
    }
    Draw-Ellipse $G $X $Y $W $H $Color $Width
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
    [void]$GeneratedFiles.Add($Name)
    return $path
}

function Draw-IconShell {
    param($G, [int]$Size, [System.Drawing.Color]$Accent)
    $pad = [Math]::Max(5, [int]($Size * 0.07))
    Fill-Ellipse $G (C 0 13 20 218) $pad $pad ($Size - $pad * 2) ($Size - $pad * 2)
    Fill-Ellipse $G (A $Accent 20) ($pad + 5) ($pad + 5) ($Size - ($pad + 5) * 2) ($Size - ($pad + 5) * 2)
    Glow-Ellipse $G ($pad + 3) ($pad + 3) ($Size - ($pad + 3) * 2) ($Size - ($pad + 3) * 2) (A $Accent 155) ([Math]::Max(1.6, $Size / 48.0))
}

function Draw-Relic {
    param([string]$Name, [System.Drawing.Color]$Accent, [string]$Kind)
    $s = 128
    $c = New-Canvas $s $s
    $g = $c.Graphics
    Draw-IconShell $g $s $Accent
    switch ($Kind) {
        "titan" {
            Fill-Poly $g @(@(64,22),@(100,42),@(92,91),@(64,110),@(36,91),@(28,42)) (A $Accent 138) (C 235 245 255 188)
            Glow-Line $g 44 55 84 55 (A $Accent 190) 4
            Glow-Line $g 46 78 82 78 (A $Accent 140) 3
        }
        "overdrive" {
            Fill-Poly $g @(@(25,74),@(53,25),@(63,57),@(100,38),@(74,82),@(65,54)) (A $Accent 170) (C 255 245 220 182)
            Glow-Ellipse $g 36 36 58 58 (A $Accent 145) 3
            Glow-Line $g 28 92 98 35 (A $Accent 130) 2.5
        }
        "data_surge" {
            for ($i = 0; $i -lt 3; $i++) {
                $y = 40 + $i * 18
                Glow-Line $g 26 $y 102 ($y + 8) (A $Accent (160 - $i * 22)) 3
            }
            Fill-Poly $g @(@(64,25),@(92,48),@(82,86),@(64,103),@(46,86),@(36,48)) (A $Accent 90) (C 225 255 238 160)
            Fill-Ellipse $g (C 236 255 236 190) 57 57 14 14
        }
        "apex" {
            Glow-Ellipse $g 28 28 72 72 (C 255 226 72 160) 3
            Fill-Poly $g @(@(64,18),@(82,45),@(112,54),@(88,74),@(92,106),@(64,90),@(36,106),@(40,74),@(16,54),@(46,45)) (A $Accent 130) (C 255 255 235 196)
            Fill-Ellipse $g (C 255 255 235 215) 51 51 26 26
            Glow-Line $g 64 28 64 100 (C 255 255 255 125) 2
        }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-ModuleIcon {
    param([string]$Name, [System.Drawing.Color]$Accent, [string]$Kind)
    $s = 96
    $c = New-Canvas $s $s
    $g = $c.Graphics
    Draw-IconShell $g $s $Accent
    switch ($Kind) {
        "bullet" { Fill-Poly $g @(@(20,55),@(59,25),@(78,39),@(37,68)) (A $Accent 205) (C 235 255 255 176); Glow-Line $g 19 72 51 56 (A $Accent 120) 3 }
        "firerate" { Glow-Ellipse $g 24 24 48 48 (A $Accent 155) 3; Fill-Poly $g @(@(48,16),@(61,44),@(81,48),@(61,55),@(48,82),@(35,55),@(15,48),@(35,44)) (A $Accent 160) (C 235 255 255 170) }
        "light" { Glow-Ellipse $g 22 22 52 52 (A $Accent 175) 4; Fill-Ellipse $g (C 248 255 220 190) 38 38 20 20 }
        "damage" { Fill-Poly $g @(@(48,12),@(62,35),@(86,42),@(66,58),@(70,84),@(48,68),@(26,84),@(30,58),@(10,42),@(34,35)) (A $Accent 172) (C 255 246 210 176) }
        "speed" { Fill-Poly $g @(@(17,52),@(49,17),@(79,52),@(56,47),@(49,80),@(42,47)) (A $Accent 188) (C 225 255 255 180) }
        "repair" { Fill-Poly $g @(@(48,21),@(68,41),@(60,69),@(48,78),@(36,69),@(28,41)) (A $Accent 168) (C 230 255 230 176); Fill-Rect $g (C 240 255 230 205) 44 32 8 34; Fill-Rect $g (C 240 255 230 205) 31 45 34 8 }
        "magnet" { Glow-Line $g 33 30 33 59 (A $Accent 210) 7; Glow-Line $g 63 30 63 59 (A $Accent 210) 7; Draw-Ellipse $g 31 26 34 42 (C 245 255 255 180) 3 }
        "explosion" { Fill-Poly $g @(@(48,16),@(58,38),@(80,32),@(63,49),@(80,66),@(58,60),@(48,82),@(38,60),@(16,66),@(33,49),@(16,32),@(38,38)) (A $Accent 172) (C 255 240 210 172) }
        "guard" { Fill-Poly $g @(@(48,15),@(70,29),@(65,66),@(48,82),@(31,66),@(26,29)) (A $Accent 170) (C 230 255 230 178); Glow-Ellipse $g 24 24 48 48 (A $Accent 118) 3 }
        "hp" { Fill-Poly $g @(@(48,21),@(70,40),@(62,68),@(48,80),@(34,68),@(26,40)) (A $Accent 175) (C 235 255 245 172) }
        "core" { Fill-Poly $g @(@(48,16),@(77,48),@(48,80),@(19,48)) (A $Accent 165) (C 255 255 220 176); Fill-Ellipse $g (C 255 255 230 190) 39 39 18 18 }
        "pierce" { Glow-Line $g 18 72 78 18 (A $Accent 215) 4; Fill-Poly $g @(@(72,15),@(83,13),@(81,25)) (A $Accent 188) (C 235 255 255 172) }
        "boss" { Fill-Poly $g @(@(48,15),@(76,33),@(72,71),@(48,84),@(24,71),@(20,33)) (A $Accent 138) (C 255 235 220 172); Fill-Ellipse $g (C 255 80 110 185) 34 43 10 10; Fill-Ellipse $g (C 255 80 110 185) 52 43 10 10 }
        "chain" { Glow-Ellipse $g 21 30 30 24 (A $Accent 170) 4; Glow-Ellipse $g 45 42 30 24 (A $Accent 170) 4 }
        "reflect" { Fill-Poly $g @(@(48,17),@(75,33),@(70,67),@(48,81),@(26,67),@(21,33)) (A $Accent 95) (C 240 232 255 180); Glow-Line $g 31 59 65 30 (A $Accent 180) 3 }
        "aura" { Glow-Ellipse $g 22 22 52 52 (A $Accent 165) 4; Glow-Ellipse $g 32 32 32 32 (A $Accent 135) 2 }
        "wave" { for ($i = 0; $i -lt 3; $i++) { Glow-Ellipse $g (36 - $i * 10) (36 - $i * 10) (24 + $i * 20) (24 + $i * 20) (A $Accent (170 - $i * 34)) 2.2 } }
        "lifesteal" { Fill-Poly $g @(@(48,17),@(66,38),@(60,65),@(48,80),@(36,65),@(30,38)) (A $Accent 160) (C 255 225 245 170); Glow-Line $g 37 31 59 69 (A $Accent 175) 2.8 }
        "data" { Fill-Poly $g @(@(48,17),@(76,34),@(76,63),@(48,80),@(20,63),@(20,34)) (A $Accent 140) (C 225 255 235 174); Fill-Ellipse $g (C 238 255 238 180) 39 39 18 18 }
        "skip" { Glow-Line $g 27 25 59 48 (A $Accent 180) 4; Glow-Line $g 27 71 59 48 (A $Accent 180) 4; Glow-Line $g 58 25 75 48 (A $Accent 126) 3; Glow-Line $g 58 71 75 48 (A $Accent 126) 3 }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-StageThumb {
    param([string]$Name, [System.Drawing.Color]$Accent, [System.Drawing.Color]$Accent2, [string]$Kind)
    $w = 512
    $h = 288
    $c = New-Canvas $w $h
    $g = $c.Graphics
    Fill-Rect $g (C 0 10 16 255) 0 0 $w $h
    Fill-Rect $g (A $Accent 18) 0 0 $w $h
    for ($x = 0; $x -le $w; $x += 48) { Draw-Line $g $x 0 $x $h (A $Accent 32) 1 }
    for ($y = 0; $y -le $h; $y += 48) { Draw-Line $g 0 $y $w $y (A $Accent 28) 1 }
    if ($Kind -eq "lava") {
        Fill-Rect $g (C 50 10 8 165) 0 190 $w 98
        for ($i = 0; $i -lt 8; $i++) { Glow-Line $g (($i * 73) % $w) (202 + (($i * 29) % 54)) (((($i * 73) % $w) + 90) % ($w + 40)) (218 + (($i * 37) % 42)) (A $Accent2 120) 3 }
    } elseif ($Kind -eq "broken") {
        for ($i = 0; $i -lt 4; $i++) {
            $x = 92 + $i * 104
            Glow-Ellipse $g ($x - 30) 96 60 60 (A $Accent2 120) 3
            Glow-Line $g $x 126 256 178 (A $Accent 78) 2
        }
        Fill-Rect $g (C 50 0 60 58) 0 0 $w $h
    } else {
        Glow-Ellipse $g 184 78 144 144 (A $Accent2 115) 3
        Fill-Rect $g (C 255 220 68 35) 208 112 96 52
    }
    Draw-Rect $g 2 2 ($w - 5) ($h - 5) (A $Accent 170) 2
    Save-Canvas $c $Name | Out-Null
}

function Draw-StageProp {
    param([string]$Name, [System.Drawing.Color]$Accent, [string]$Kind)
    $s = 256
    $c = New-Canvas $s $s
    $g = $c.Graphics
    switch ($Kind) {
        "lava_pool" {
            Fill-Ellipse $g (C 44 8 7 218) 25 54 206 148
            Glow-Ellipse $g 35 64 186 128 (A $Accent 120) 3
            for ($i = 0; $i -lt 7; $i++) { Glow-Line $g (50 + $i * 23) (98 + (($i * 19) % 44)) (92 + $i * 19) (118 + (($i * 23) % 32)) (A $Accent 128) 4 }
        }
        "heat_vent" {
            Fill-Poly $g @(@(128,42),@(204,84),@(204,172),@(128,214),@(52,172),@(52,84)) (C 9 23 28 210) (A $Accent 160)
            for ($i = 0; $i -lt 5; $i++) { Glow-Line $g 76 (88 + $i * 20) 180 (88 + $i * 20) (A $Accent (160 - $i * 16)) 3 }
        }
        "relay" {
            Glow-Ellipse $g 58 58 140 140 (A $Accent 95) 3
            Fill-Poly $g @(@(128,36),@(176,84),@(160,184),@(128,220),@(96,184),@(80,84)) (C 4 20 26 230) (A $Accent 170)
            Fill-Ellipse $g (A $Accent 190) 104 104 48 48
            Glow-Line $g 128 40 128 216 (A $Accent 120) 2
        }
        "corruption" {
            Fill-Ellipse $g (C 40 0 56 112) 28 48 198 154
            for ($i = 0; $i -lt 10; $i++) {
                $x = 42 + (($i * 31) % 166)
                $y = 65 + (($i * 47) % 112)
                Fill-Ellipse $g (A $Accent (72 + ($i % 3) * 24)) $x $y (18 + ($i % 4) * 7) (12 + ($i % 3) * 8)
            }
        }
    }
    Save-Canvas $c $Name | Out-Null
}

function Draw-PickupData64 {
    $c = New-Canvas 64 64
    $g = $c.Graphics
    $accent = C 96 255 172 235
    Fill-Poly $g @(@(32,7),@(55,20),@(55,44),@(32,57),@(9,44),@(9,20)) (A $accent 132) (C 224 255 235 182)
    Fill-Poly $g @(@(32,16),@(45,24),@(45,40),@(32,48),@(19,40),@(19,24)) (C 0 18 22 230) (A $accent 160)
    Fill-Ellipse $g (C 236 255 238 190) 26 26 12 12
    Glow-Ellipse $g 13 13 38 38 (A $accent 122) 2
    Save-Canvas $c "Pickup_Data_64.png" | Out-Null
}

Draw-Relic "Relic_Titan.png" (C 180 205 224 235) "titan"
Draw-Relic "Relic_Overdrive.png" (C 255 105 48 235) "overdrive"
Draw-Relic "Relic_DataSurge.png" (C 86 255 178 235) "data_surge"
Draw-Relic "Relic_Apex.png" (C 255 229 72 235) "apex"

Draw-ModuleIcon "ModuleIcon_Bullet.png" (C 70 225 255 235) "bullet"
Draw-ModuleIcon "ModuleIcon_FireRate.png" (C 92 240 255 235) "firerate"
Draw-ModuleIcon "ModuleIcon_CoreLight.png" (C 255 222 72 235) "light"
Draw-ModuleIcon "ModuleIcon_Damage.png" (C 255 176 48 235) "damage"
Draw-ModuleIcon "ModuleIcon_Speed.png" (C 68 218 255 235) "speed"
Draw-ModuleIcon "ModuleIcon_Repair.png" (C 112 255 142 235) "repair"
Draw-ModuleIcon "ModuleIcon_Magnet.png" (C 94 255 176 235) "magnet"
Draw-ModuleIcon "ModuleIcon_Explosion.png" (C 255 112 42 235) "explosion"
Draw-ModuleIcon "ModuleIcon_GuardRing.png" (C 96 255 124 235) "guard"
Draw-ModuleIcon "ModuleIcon_HP.png" (C 72 228 255 235) "hp"
Draw-ModuleIcon "ModuleIcon_CoreRepair.png" (C 255 220 68 235) "core"
Draw-ModuleIcon "ModuleIcon_Pierce.png" (C 78 230 255 235) "pierce"
Draw-ModuleIcon "ModuleIcon_Boss.png" (C 255 88 118 235) "boss"
Draw-ModuleIcon "ModuleIcon_Chain.png" (C 116 255 238 235) "chain"
Draw-ModuleIcon "ModuleIcon_Reflect.png" (C 196 136 255 235) "reflect"
Draw-ModuleIcon "ModuleIcon_Aura.png" (C 160 255 98 235) "aura"
Draw-ModuleIcon "ModuleIcon_Wave.png" (C 118 230 255 235) "wave"
Draw-ModuleIcon "ModuleIcon_Lifesteal.png" (C 255 92 190 235) "lifesteal"
Draw-ModuleIcon "ModuleIcon_DataForge.png" (C 92 255 168 235) "data"
Draw-ModuleIcon "ModuleIcon_Skip.png" (C 124 255 230 235) "skip"

Draw-StageThumb "StageThumb_Arena.png" (C 70 225 255 220) (C 255 220 72 180) "arena"
Draw-StageThumb "StageThumb_Lava.png" (C 255 92 42 220) (C 255 190 58 190) "lava"
Draw-StageThumb "StageThumb_BrokenCore.png" (C 150 80 255 220) (C 70 235 255 185) "broken"

Draw-StageProp "Stage2_LavaPool_A.png" (C 255 92 34 220) "lava_pool"
Draw-StageProp "Stage2_HeatVent_A.png" (C 255 154 56 220) "heat_vent"
Draw-StageProp "Stage3_RelayDevice_A.png" (C 78 235 255 220) "relay"
Draw-StageProp "Stage3_CorruptionPatch_A.png" (C 214 72 255 180) "corruption"

Draw-PickupData64

$previewPath = Join-Path $ArtDir "Generated_NonCharacterPolishBatch2_Preview.png"
$preview = New-Canvas 1320 920
$pg = $preview.Graphics
Fill-Rect $pg (C 0 8 13 255) 0 0 1320 920
$font = [System.Drawing.Font]::new("Arial", 16, [System.Drawing.FontStyle]::Bold)
$small = [System.Drawing.Font]::new("Arial", 8, [System.Drawing.FontStyle]::Bold)
$titleBrush = Brush (C 190 255 255 255)
$pg.DrawString("NON-CHARACTER POLISH BATCH 2", $font, $titleBrush, 24, 18)
$titleBrush.Dispose()

$files = @(
    "Relic_Titan.png","Relic_Overdrive.png","Relic_DataSurge.png","Relic_Apex.png",
    "ModuleIcon_Bullet.png","ModuleIcon_FireRate.png","ModuleIcon_CoreLight.png","ModuleIcon_Damage.png","ModuleIcon_Speed.png","ModuleIcon_Repair.png",
    "ModuleIcon_Magnet.png","ModuleIcon_Explosion.png","ModuleIcon_GuardRing.png","ModuleIcon_HP.png","ModuleIcon_CoreRepair.png","ModuleIcon_Pierce.png",
    "ModuleIcon_Boss.png","ModuleIcon_Chain.png","ModuleIcon_Reflect.png","ModuleIcon_Aura.png","ModuleIcon_Wave.png","ModuleIcon_Lifesteal.png",
    "ModuleIcon_DataForge.png","ModuleIcon_Skip.png","Pickup_Data_64.png","Stage2_LavaPool_A.png","Stage2_HeatVent_A.png","Stage3_RelayDevice_A.png","Stage3_CorruptionPatch_A.png",
    "StageThumb_Arena.png","StageThumb_Lava.png","StageThumb_BrokenCore.png"
)

for ($i = 0; $i -lt $files.Count; $i++) {
    $col = $i % 8
    $row = [Math]::Floor($i / 8)
    $x = 26 + $col * 160
    $y = 62 + $row * 190
    $path = Join-Path $SkinDir $files[$i]
    if (-not (Test-Path -LiteralPath $path)) { continue }
    $img = [System.Drawing.Image]::FromFile($path)
    $drawW = if ($files[$i].StartsWith("StageThumb_")) { 128 } else { 86 }
    $drawH = if ($files[$i].StartsWith("StageThumb_")) { 72 } else { 86 }
    $pg.DrawImage($img, $x, $y, $drawW, $drawH)
    $img.Dispose()
    $b = Brush (C 206 255 255 235)
    $pg.DrawString($files[$i], $small, $b, $x, $y + $drawH + 8)
    $b.Dispose()
}

$font.Dispose()
$small.Dispose()
$preview.Bitmap.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
$preview.Graphics.Dispose()
$preview.Bitmap.Dispose()

Write-Host "Generated $($GeneratedFiles.Count) non-character polish assets into $SkinDir"
Write-Host "Preview: $previewPath"
if ($DidCreateBackup) {
    Write-Host "Backup: $BackupDir"
}
