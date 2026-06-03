param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource"
New-Item -ItemType Directory -Force -Path $SkinDir | Out-Null
New-Item -ItemType Directory -Force -Path $ArtDir | Out-Null

function C { param([int]$R,[int]$G,[int]$B,[int]$A=255) [System.Drawing.Color]::FromArgb($A,$R,$G,$B) }
function A { param([System.Drawing.Color]$Color,[int]$Alpha) [System.Drawing.Color]::FromArgb($Alpha,$Color.R,$Color.G,$Color.B) }

function New-Bmp {
    param([int]$W=256,[int]$H=256)
    $bmp = [System.Drawing.Bitmap]::new($W,$H,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    @{ Bitmap=$bmp; Graphics=$g }
}

function Save-Png {
    param([System.Drawing.Bitmap]$Bitmap,[string]$Path)
    if ((Test-Path -LiteralPath $Path) -and -not $Force) { return $false }
    $Bitmap.Save($Path,[System.Drawing.Imaging.ImageFormat]::Png)
    return $true
}

function Brush { param([System.Drawing.Color]$Color) [System.Drawing.SolidBrush]::new($Color) }
function Pen { param([System.Drawing.Color]$Color,[float]$W=2) $p=[System.Drawing.Pen]::new($Color,$W); $p.StartCap='Round'; $p.EndCap='Round'; $p.LineJoin='Round'; $p }

function Fill-Ellipse { param($G,[float]$X,[float]$Y,[float]$W,[float]$H,[System.Drawing.Color]$Color)
    $b=Brush $Color; $G.FillEllipse($b,$X,$Y,$W,$H); $b.Dispose()
}

function Draw-Ellipse { param($G,[float]$X,[float]$Y,[float]$W,[float]$H,[System.Drawing.Color]$Color,[float]$Width=2)
    $p=Pen $Color $Width; $G.DrawEllipse($p,$X,$Y,$W,$H); $p.Dispose()
}

function Draw-Line { param($G,[float]$X1,[float]$Y1,[float]$X2,[float]$Y2,[System.Drawing.Color]$Color,[float]$Width=2)
    $p=Pen $Color $Width; $G.DrawLine($p,$X1,$Y1,$X2,$Y2); $p.Dispose()
}

function Fill-Poly {
    param($G,[object[]]$Pts,[System.Drawing.Color]$Fill,[System.Drawing.Color]$Stroke)
    $points = @()
    foreach ($pt in $Pts) { $points += [System.Drawing.PointF]::new([float]$pt[0],[float]$pt[1]) }
    $b=Brush $Fill; $G.FillPolygon($b,$points); $b.Dispose()
    $p=Pen $Stroke 2; $G.DrawPolygon($p,$points); $p.Dispose()
}

function Glow-Ellipse {
    param($G,[float]$X,[float]$Y,[float]$W,[float]$H,[System.Drawing.Color]$Color,[float]$Width=2)
    foreach ($m in 5,3,2) {
        $p=Pen (A $Color ([Math]::Max(8,[int]($Color.A/($m+1))))) ($Width*$m)
        $G.DrawEllipse($p,$X,$Y,$W,$H); $p.Dispose()
    }
    Draw-Ellipse $G $X $Y $W $H $Color $Width
}

function Glow-Line {
    param($G,[float]$X1,[float]$Y1,[float]$X2,[float]$Y2,[System.Drawing.Color]$Color,[float]$Width=2)
    foreach ($m in 5,3,2) {
        $p=Pen (A $Color ([Math]::Max(8,[int]($Color.A/($m+1))))) ($Width*$m)
        $G.DrawLine($p,$X1,$Y1,$X2,$Y2); $p.Dispose()
    }
    Draw-Line $G $X1 $Y1 $X2 $Y2 $Color $Width
}

$RouteAccent = @{
    1 = C 52 230 255 235
    2 = C 255 160 38 235
    3 = C 96 255 118 235
}

$FusionAccent = @{
    1 = C 255 72 236 235
    2 = C 142 255 58 235
    3 = C 255 212 64 235
    4 = C 72 224 255 235
    5 = C 190 125 255 235
    6 = C 82 255 205 235
}

function Draw-RouteMotif {
    param($G,[int]$Route,[System.Drawing.Color]$Accent,[int]$Stage)
    if ($Route -le 0) { return }
    Glow-Ellipse $G 44 44 168 168 (A $Accent 80) 2
    if ($Route -eq 1) {
        Glow-Line $G 42 184 215 66 (A $Accent 150) 2.4
        Glow-Line $G 62 214 222 111 (A $Accent 95) 1.7
        Fill-Poly $G @(@(34,126),@(72,104),@(62,142),@(22,158)) (A $Accent 95) (A (C 235 255 255) 140)
        Fill-Poly $G @(@(222,126),@(184,104),@(194,142),@(234,158)) (A $Accent 95) (A (C 235 255 255) 140)
    }
    elseif ($Route -eq 2) {
        Glow-Ellipse $G 66 58 124 124 (A $Accent 105) 3
        Fill-Poly $G @(@(112,44),@(128,14),@(144,44),@(137,70),@(119,70)) (A $Accent 135) (A (C 255 245 210) 150)
        Fill-Poly $G @(@(44,112),@(80,94),@(88,135),@(50,148)) (A $Accent 105) (A (C 255 245 210) 120)
        Fill-Poly $G @(@(212,112),@(176,94),@(168,135),@(206,148)) (A $Accent 105) (A (C 255 245 210) 120)
    }
    else {
        Glow-Ellipse $G 30 35 196 196 (A $Accent 122) 3.3
        Glow-Ellipse $G 63 65 130 130 (A $Accent 95) 2
        Fill-Poly $G @(@(128,174),@(164,198),@(146,226),@(110,226),@(92,198)) (A $Accent 85) (A (C 235 255 235) 130)
    }
}

function Draw-FusionMotif {
    param($G,[int]$Fusion,[System.Drawing.Color]$Accent,[int]$Stage)
    if ($Fusion -le 0) { return }
    $r = 80 + $Stage * 8
    Glow-Ellipse $G (128-$r/2) (128-$r/2) $r $r (A $Accent 150) 3
    switch ($Fusion) {
        1 { Glow-Line $G 64 92 192 164 (A $Accent 165) 3; Glow-Line $G 192 92 64 164 (A $Accent 110) 2 }
        2 { Fill-Ellipse $G 55 112 22 22 (A $Accent 165); Fill-Ellipse $G 179 112 22 22 (A $Accent 165); Glow-Line $G 66 123 190 123 (A $Accent 130) 2 }
        3 { Fill-Poly $G @(@(128,48),@(184,95),@(164,178),@(92,178),@(72,95)) (A $Accent 90) (A $Accent 190) }
        4 { Glow-Line $G 52 128 204 128 (A $Accent 120) 2; Glow-Line $G 128 52 128 204 (A $Accent 120) 2 }
        5 { Glow-Ellipse $G 76 72 104 112 (A $Accent 115) 4; Glow-Line $G 86 186 170 72 (A $Accent 130) 2 }
        6 { Fill-Ellipse $G 108 46 40 40 (A $Accent 110); Glow-Line $G 128 66 128 204 (A $Accent 130) 2 }
    }
}

function Draw-NewPartner {
    param([int]$Species,[int]$Stage,[int]$Route,[int]$Fusion,[string]$OutPath)
    $c = New-Bmp 256 256
    $g = $c.Graphics
    $stageScale = 1.0 + $Stage * 0.1
    $accent = if ($Fusion -gt 0) { $FusionAccent[$Fusion] } elseif ($Route -gt 0) { $RouteAccent[$Route] } elseif ($Species -eq 8) { C 255 230 74 235 } else { C 160 236 255 235 }
    Draw-RouteMotif $g $Route $accent $Stage
    Draw-FusionMotif $g $Fusion $accent $Stage

    if ($Species -eq 7) {
        $fur = C 220 244 255 255
        $deep = C 48 65 92 255
        $shadow = C 95 132 165 210
        $cyan = C 126 232 255 235
        $bodyW = 82 * $stageScale
        $bodyH = 68 * $stageScale
        Fill-Poly $g @(@(66,168),@(24,190),@(62,128),@(95,150)) (A $cyan 75) (A $cyan 150)
        Fill-Poly $g @(@(190,168),@(232,190),@(194,128),@(161,150)) (A $cyan 75) (A $cyan 150)
        Fill-Ellipse $g (128-$bodyW/2) (133-$bodyH/2) $bodyW $bodyH $shadow
        Fill-Ellipse $g (128-$bodyW*0.42) (126-$bodyH*0.42) ($bodyW*0.84) ($bodyH*0.84) $fur
        Fill-Ellipse $g 91 60 74 70 $fur
        Fill-Ellipse $g 102 76 52 48 (C 235 254 255 255)
        Fill-Poly $g @(@(94,73),@(78,31),@(118,57)) $fur (A $cyan 190)
        Fill-Poly $g @(@(158,73),@(180,31),@(137,57)) $fur (A $cyan 190)
        Fill-Poly $g @(@(104,104),@(128,137),@(152,104),@(128,116)) (A $deep 70) (A $cyan 130)
        Fill-Ellipse $g 108 88 10 12 $deep
        Fill-Ellipse $g 138 88 10 12 $deep
        Fill-Ellipse $g 113 92 4 4 $cyan
        Fill-Ellipse $g 143 92 4 4 $cyan
        Fill-Poly $g @(@(122,106),@(134,106),@(128,116)) (C 60 78 100 255) (A $cyan 170)
        foreach ($sx in -1,1) {
            $baseX = 128 + $sx * (38 + $Stage * 3)
            Fill-Poly $g @(@($baseX,154),@(($baseX + $sx * 36),182),@(($baseX + $sx * 22),196),@(($baseX - $sx * 8),166)) (A $fur 215) (A $cyan 150)
            Glow-Line $g ($baseX + $sx * 14) 189 ($baseX + $sx * 38) 210 (A $cyan 165) 2.3
        }
        if ($Stage -ge 2) {
            Glow-Line $g 84 57 50 42 (A $cyan 190) 3
            Glow-Line $g 172 57 206 42 (A $cyan 190) 3
        }
        if ($Stage -ge 3 -or $Fusion -gt 0) {
            Glow-Ellipse $g 72 47 112 144 (A $accent 150) 2.5
            Fill-Ellipse $g 117 29 22 22 (A $accent 170)
        }
    }
    else {
        $gold = C 255 220 78 255
        $cyan = C 94 242 255 240
        $deep = C 24 86 112 255
        $coreSize = 72 + $Stage * 10
        Glow-Ellipse $g 54 54 148 148 (A $cyan 105) 2.5
        Draw-Line $g 128 34 128 222 (A $cyan 120) 2
        Draw-Line $g 34 128 222 128 (A $cyan 120) 2
        Fill-Poly $g @(@(128,42),@(168,82),@(128,122),@(88,82)) (A $gold 185) (A $cyan 150)
        Fill-Poly $g @(@(128,214),@(168,174),@(128,134),@(88,174)) (A $gold 150) (A $cyan 120)
        Fill-Poly $g @(@(42,128),@(82,88),@(122,128),@(82,168)) (A $cyan 95) (A $gold 130)
        Fill-Poly $g @(@(214,128),@(174,88),@(134,128),@(174,168)) (A $cyan 95) (A $gold 130)
        Fill-Ellipse $g (128-$coreSize/2) (128-$coreSize/2) $coreSize $coreSize $deep
        Fill-Ellipse $g (128-$coreSize*0.34) (128-$coreSize*0.34) ($coreSize*0.68) ($coreSize*0.68) $gold
        Fill-Ellipse $g 108 108 40 40 (C 250 255 230 255)
        Glow-Ellipse $g 96 96 64 64 (A $cyan 190) 3
        for ($i=0; $i -lt 4; $i++) {
            $ang = [Math]::PI/2*$i + 0.35
            $x = 128 + [Math]::Cos($ang)*(58+$Stage*6)
            $y = 128 + [Math]::Sin($ang)*(58+$Stage*6)
            Fill-Ellipse $g ($x-10) ($y-10) 20 20 (A $accent 150)
        }
        if ($Stage -ge 2) {
            Glow-Ellipse $g 42 42 172 172 (A $gold 145) 3
        }
        if ($Stage -ge 3 -or $Fusion -gt 0) {
            Glow-Ellipse $g 30 30 196 196 (A $accent 150) 3.5
            Fill-Poly $g @(@(128,12),@(148,44),@(128,66),@(108,44)) (A $accent 150) (A $cyan 180)
        }
    }

    Save-Png $c.Bitmap $OutPath | Out-Null
    $g.Dispose(); $c.Bitmap.Dispose()
}

function Draw-IconBase {
    param([string]$Name,[System.Drawing.Color]$Accent,[scriptblock]$Draw)
    $path = Join-Path $SkinDir $Name
    $c = New-Bmp 128 128
    $g = $c.Graphics
    Glow-Ellipse $g 18 18 92 92 (A $Accent 105) 2.5
    Fill-Ellipse $g 30 30 68 68 (A (C 4 18 28) 235)
    & $Draw $g $Accent
    Save-Png $c.Bitmap $path | Out-Null
    $g.Dispose(); $c.Bitmap.Dispose()
}

function Draw-PanelFrame {
    param([string]$Name,[int]$W,[int]$H,[System.Drawing.Color]$Accent,[int]$Alpha=175)
    $path = Join-Path $SkinDir $Name
    $c = New-Bmp $W $H
    $g = $c.Graphics
    $bg=Brush (C 0 12 18 120); $g.FillRectangle($bg,0,0,$W,$H); $bg.Dispose()
    $p=Pen (A $Accent $Alpha) 3; $g.DrawRectangle($p,4,4,$W-8,$H-8); $p.Dispose()
    Glow-Line $g 14 18 ($W*0.44) 18 (A $Accent 150) 2
    Glow-Line $g ($W*0.68) ($H-14) ($W-14) ($H-14) (A $Accent 100) 2
    Save-Png $c.Bitmap $path | Out-Null
    $g.Dispose(); $c.Bitmap.Dispose()
}

function Draw-Bar {
    param([string]$Name,[int]$W,[int]$H,[System.Drawing.Color]$Color)
    $path = Join-Path $SkinDir $Name
    $c = New-Bmp $W $H
    $g = $c.Graphics
    $b=Brush (A $Color 210); $g.FillRectangle($b,0,2,$W,$H-4); $b.Dispose()
    $hi=Brush (A (C 255 255 255) 72); $g.FillRectangle($hi,0,2,$W,[Math]::Max(2,[int]($H*0.28))); $hi.Dispose()
    Save-Png $c.Bitmap $path | Out-Null
    $g.Dispose(); $c.Bitmap.Dispose()
}

function Draw-Vignette {
    param([string]$Name,[System.Drawing.Color]$Color)
    $path = Join-Path $SkinDir $Name
    $c = New-Bmp 1280 720
    $g = $c.Graphics
    for ($i=0; $i -lt 26; $i++) {
        $a = [int](95 * (1 - $i / 26.0))
        $p = Pen (A $Color $a) (22 - $i*0.45)
        $g.DrawRectangle($p,$i*14,$i*8,1280-$i*28,720-$i*16)
        $p.Dispose()
    }
    Save-Png $c.Bitmap $path | Out-Null
    $g.Dispose(); $c.Bitmap.Dispose()
}

function Draw-Backdrop {
    param([string]$Name,[System.Drawing.Color]$Accent,[System.Drawing.Color]$Accent2)
    $path = Join-Path $SkinDir $Name
    $c = New-Bmp 1280 720
    $g = $c.Graphics
    $bg=Brush (C 0 9 15 255); $g.FillRectangle($bg,0,0,1280,720); $bg.Dispose()
    for ($x=0; $x -lt 1280; $x+=64) { Draw-Line $g $x 0 $x 720 (A $Accent 35) 1 }
    for ($y=0; $y -lt 720; $y+=64) { Draw-Line $g 0 $y 1280 $y (A $Accent 28) 1 }
    Glow-Ellipse $g 450 140 380 380 (A $Accent 95) 4
    Glow-Line $g 120 140 1160 140 (A $Accent2 115) 3
    Glow-Line $g 180 590 1100 590 (A $Accent 90) 2
    Save-Png $c.Bitmap $path | Out-Null
    $g.Dispose(); $c.Bitmap.Dispose()
}

$generated = 0

foreach ($species in 7,8) {
    $path = Join-Path $SkinDir "Partner_S${species}_L0.png"
    Draw-NewPartner $species 0 0 0 $path
    $generated++
    foreach ($route in 1..3) {
        foreach ($stage in 0..3) {
            $path = Join-Path $SkinDir "Partner_S${species}_R${route}_L${stage}.png"
            Draw-NewPartner $species $stage $route 0 $path
            $generated++
        }
    }
    foreach ($fusion in 1..6) {
        foreach ($stage in 0..3) {
            $path = Join-Path $SkinDir "Partner_S${species}_F${fusion}_L${stage}.png"
            Draw-NewPartner $species $stage 0 $fusion $path
            $generated++
        }
    }
}

Draw-IconBase "Relic_Magnet.png" (C 112 255 150 235) { param($g,$a) Glow-Line $g 42 44 42 74 $a 7; Glow-Line $g 86 44 86 74 $a 7; Draw-Ellipse $g 40 38 48 54 (A (C 255 255 255) 180) 4 }
Draw-IconBase "Relic_Deathless.png" (C 255 226 74 235) { param($g,$a) Fill-Poly $g @(@(64,24),@(96,44),@(86,92),@(64,106),@(42,92),@(32,44)) (A $a 125) (A (C 255 255 230) 180); Glow-Line $g 64 36 64 92 $a 3 }
Draw-IconBase "Relic_CorePulse.png" (C 100 235 255 235) { param($g,$a) Fill-Ellipse $g 50 50 28 28 (A $a 210); Glow-Ellipse $g 36 36 56 56 $a 3; Glow-Ellipse $g 24 24 80 80 (A $a 130) 2 }
Draw-IconBase "Relic_Explosion.png" (C 255 110 42 235) { param($g,$a) Fill-Poly $g @(@(64,20),@(76,50),@(108,42),@(82,64),@(108,86),@(74,78),@(64,110),@(52,78),@(20,86),@(46,64),@(20,42),@(52,50)) (A $a 155) (A (C 255 238 180) 180) }
Draw-IconBase "Relic_Storm.png" (C 88 225 255 235) { param($g,$a) Fill-Poly $g @(@(76,18),@(40,66),@(64,66),@(52,110),@(92,54),@(68,54)) (A $a 170) (A (C 230 255 255) 190) }
Draw-IconBase "Relic_Mirror.png" (C 190 135 255 235) { param($g,$a) Fill-Poly $g @(@(64,22),@(98,48),@(84,98),@(64,112),@(44,98),@(30,48)) (A $a 95) (A (C 245 235 255) 185); Glow-Line $g 44 48 84 92 $a 2.4 }

Draw-PanelFrame "HUD_Panel_Status.png" 384 160 (C 70 245 255 235)
Draw-PanelFrame "HUD_Panel_Loadout.png" 384 160 (C 255 210 70 235)
Draw-PanelFrame "HUD_BossBar_Frame.png" 512 48 (C 255 70 210 235)
Draw-Bar "HUD_BossBar_Fill.png" 512 24 (C 255 52 112 235)
Draw-PanelFrame "HUD_RelicSlot_Frame.png" 128 128 (C 255 220 80 235)
Draw-IconBase "HUD_Icon_PlayerHP.png" (C 70 220 255 235) { param($g,$a) Fill-Poly $g @(@(64,28),@(92,58),@(80,96),@(64,106),@(48,96),@(36,58)) (A $a 150) (A (C 230 255 255) 190) }
Draw-IconBase "HUD_Icon_CoreHP.png" (C 255 220 68 235) { param($g,$a) Fill-Poly $g @(@(64,24),@(100,64),@(64,104),@(28,64)) (A $a 165) (A (C 255 255 220) 190) }
Draw-IconBase "HUD_Icon_EXP.png" (C 152 255 80 235) { param($g,$a) Fill-Poly $g @(@(64,22),@(84,54),@(116,64),@(84,74),@(64,106),@(44,74),@(12,64),@(44,54)) (A $a 140) (A (C 230 255 220) 185) }
Draw-IconBase "HUD_Icon_DataChip.png" (C 95 255 170 235) { param($g,$a) Fill-Poly $g @(@(64,24),@(100,44),@(100,84),@(64,104),@(28,84),@(28,44)) (A $a 130) (A (C 220 255 235) 180); Fill-Ellipse $g 56 56 16 16 (A (C 255 255 255) 170) }
Draw-IconBase "Minimap_Dot_Pickup.png" (C 120 255 160 235) { param($g,$a) Fill-Poly $g @(@(64,30),@(92,64),@(64,98),@(36,64)) (A $a 190) (A (C 235 255 230) 180) }
Draw-IconBase "Minimap_Ping_Wave.png" (C 255 86 120 235) { param($g,$a) Glow-Ellipse $g 34 34 60 60 $a 3; Glow-Line $g 64 20 64 108 $a 2; Glow-Line $g 20 64 108 64 $a 2 }

Draw-PanelFrame "HUD_BossWarning_Banner.png" 768 96 (C 255 70 210 235)
Draw-Vignette "HUD_DangerVignette_Player.png" (C 255 58 88 235)
Draw-Vignette "HUD_DangerVignette_Core.png" (C 255 218 62 235)

$choice = New-Bmp 1280 720
$cg = $choice.Graphics
$b=Brush (C 0 8 14 185); $cg.FillRectangle($b,0,0,1280,720); $b.Dispose()
Glow-Ellipse $cg 440 150 400 400 (A (C 90 245 255 150) 135) 4
Save-Png $choice.Bitmap (Join-Path $SkinDir "HUD_ChoiceDim.png") | Out-Null
$cg.Dispose(); $choice.Bitmap.Dispose()

Draw-PanelFrame "Boundary_Gate_A.png" 256 256 (C 70 230 255 235)
Draw-Backdrop "MainMenu_Background.png" (C 80 230 255 235) (C 255 220 80 210)
Draw-Backdrop "Result_Clear_Background.png" (C 120 255 160 235) (C 255 220 80 210)
Draw-Backdrop "Result_GameOver_Background.png" (C 255 62 100 235) (C 190 120 255 210)
Draw-Backdrop "Cutin_Evolve_Speed.png" (C 70 232 255 235) (C 120 255 255 210)
Draw-Backdrop "Cutin_Evolve_Power.png" (C 255 154 40 235) (C 255 220 80 210)
Draw-Backdrop "Cutin_Evolve_Guard.png" (C 100 255 120 235) (C 255 220 80 210)
Draw-Backdrop "Cutin_Fusion.png" (C 255 80 235 235) (C 80 240 255 210)

$previewPath = Join-Path $ArtDir "Generated_NewPartnerRelicUi_Preview.png"
$preview = New-Bmp 1280 920
$pg = $preview.Graphics
$font = [System.Drawing.Font]::new("Arial",15,[System.Drawing.FontStyle]::Bold)
$smallFont = [System.Drawing.Font]::new("Arial",9,[System.Drawing.FontStyle]::Bold)
$titleBrush = Brush (C 185 255 255 255)
$pg.DrawString("NEW PARTNER / RELIC / UI ASSET PACK",$font,$titleBrush,24,18)
$titleBrush.Dispose()
$files = @(
    "Partner_S7_L0.png","Partner_S7_R1_L3.png","Partner_S7_R2_L3.png","Partner_S7_R3_L3.png","Partner_S7_F1_L3.png","Partner_S7_F4_L3.png",
    "Partner_S8_L0.png","Partner_S8_R1_L3.png","Partner_S8_R2_L3.png","Partner_S8_R3_L3.png","Partner_S8_F3_L3.png","Partner_S8_F6_L3.png",
    "Relic_Magnet.png","Relic_Deathless.png","Relic_CorePulse.png","Relic_Explosion.png","Relic_Storm.png","Relic_Mirror.png",
    "HUD_BossBar_Frame.png","HUD_RelicSlot_Frame.png","HUD_Icon_PlayerHP.png","HUD_Icon_CoreHP.png","HUD_Icon_EXP.png","HUD_Icon_DataChip.png",
    "HUD_BossWarning_Banner.png","HUD_DangerVignette_Player.png","MainMenu_Background.png","Cutin_Fusion.png"
)
for ($i=0; $i -lt $files.Count; $i++) {
    $col = $i % 4
    $row = [Math]::Floor($i / 4)
    $x = 24 + $col * 306
    $y = 62 + $row * 118
    $p = Join-Path $SkinDir $files[$i]
    $img = [System.Drawing.Image]::FromFile($p)
    $pg.DrawImage($img,$x,$y,96,96)
    $img.Dispose()
    $brush = Brush (C 205 255 255 255)
    $pg.DrawString($files[$i],$smallFont,$brush,$x+104,$y+35)
    $brush.Dispose()
}
$font.Dispose(); $smallFont.Dispose()
Save-Png $preview.Bitmap $previewPath | Out-Null
$pg.Dispose(); $preview.Bitmap.Dispose()

Write-Host "Generated new partner, relic, and UI/HUD assets into $SkinDir"
Write-Host "Preview: $previewPath"
