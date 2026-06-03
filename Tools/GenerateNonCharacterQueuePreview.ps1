$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$OutDir = Join-Path $ProjectRoot "Assets\ArtSource\NonCharacterQueue_20260601"
$OutPath = Join-Path $OutDir "NonCharacterAssetQueue_20260601.png"

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
Add-Type -AssemblyName System.Drawing

function New-Color([int]$R, [int]$G, [int]$B, [int]$A = 255) {
    return [System.Drawing.Color]::FromArgb($A, $R, $G, $B)
}

function New-Font([float]$Size, [System.Drawing.FontStyle]$Style = [System.Drawing.FontStyle]::Regular) {
    return [System.Drawing.Font]::new("Segoe UI", $Size, $Style, [System.Drawing.GraphicsUnit]::Pixel)
}

function Draw-Text($G, [string]$Text, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Size = 14, [System.Drawing.StringAlignment]$Align = [System.Drawing.StringAlignment]::Near, [switch]$Bold) {
    $font = New-Font $Size $(if ($Bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular })
    $brush = [System.Drawing.SolidBrush]::new($Color)
    $format = [System.Drawing.StringFormat]::new()
    $format.Alignment = $Align
    $format.LineAlignment = [System.Drawing.StringAlignment]::Center
    $format.Trimming = [System.Drawing.StringTrimming]::EllipsisCharacter
    $G.DrawString($Text, $font, $brush, [System.Drawing.RectangleF]::new($X, $Y, $W, $H), $format)
    $format.Dispose()
    $brush.Dispose()
    $font.Dispose()
}

function Draw-AssetTile($G, [string]$FileName, [float]$X, [float]$Y, [float]$W, [float]$H, [string]$Status, [System.Drawing.Color]$Accent) {
    $panelBrush = [System.Drawing.SolidBrush]::new((New-Color 4 18 24 232))
    $G.FillRectangle($panelBrush, [System.Drawing.RectangleF]::new($X, $Y, $W, $H))
    $panelBrush.Dispose()

    $pen = [System.Drawing.Pen]::new($Accent, 1.4)
    $G.DrawRectangle($pen, $X, $Y, $W, $H)
    $pen.Dispose()

    $path = Join-Path $SkinDir $FileName
    $imageRect = [System.Drawing.RectangleF]::new($X + 10, $Y + 12, $W - 20, $H - 48)
    if (Test-Path -LiteralPath $path) {
        $img = [System.Drawing.Image]::FromFile($path)
        try {
            $scale = [Math]::Min($imageRect.Width / $img.Width, $imageRect.Height / $img.Height)
            $dw = [float]($img.Width * $scale)
            $dh = [float]($img.Height * $scale)
            $dx = [float]($imageRect.X + (($imageRect.Width - $dw) / 2))
            $dy = [float]($imageRect.Y + (($imageRect.Height - $dh) / 2))
            $G.DrawImage($img, [System.Drawing.RectangleF]::new($dx, $dy, $dw, $dh))
        } finally {
            $img.Dispose()
        }
    } else {
        $missingBrush = [System.Drawing.SolidBrush]::new((New-Color 50 8 12 235))
        $G.FillRectangle($missingBrush, $imageRect)
        $missingBrush.Dispose()
        Draw-Text $G "MISSING" $imageRect.X $imageRect.Y $imageRect.Width $imageRect.Height (New-Color 255 90 125) 16 ([System.Drawing.StringAlignment]::Center) -Bold
    }

    $statusBrush = [System.Drawing.SolidBrush]::new([System.Drawing.Color]::FromArgb(178, $Accent.R, $Accent.G, $Accent.B))
    $G.FillRectangle($statusBrush, [System.Drawing.RectangleF]::new($X + 8, $Y + $H - 40, $W - 16, 16))
    $statusBrush.Dispose()

    Draw-Text $G $Status ($X + 10) ($Y + $H - 41) ($W - 20) 16 (New-Color 2 16 20) 10 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $G $FileName ($X + 8) ($Y + $H - 22) ($W - 16) 18 (New-Color 205 245 245) 10 ([System.Drawing.StringAlignment]::Center)
}

$sections = @(
    @{
        Title = "A EGG PRESENTATION - Claude hook waiting"
        Note = "Title / result / small icon. Preserve aspect; do not place text over the faint circle."
        Accent = (New-Color 255 214 72)
        Status = "CLAUDE HOOK"
        Files = @("Title_CoreEgg_ASelected_v1.png", "Result_CoreEgg_ASelected_v1.png", "Icon_CoreEgg_ASelected_v1.png")
    },
    @{
        Title = "HUD9 REMAINING - panels and bars"
        Note = "Wave progress is already hooked. These 8 files wait for staged HUD hookup."
        Accent = (New-Color 75 235 255)
        Status = "WAITING"
        Files = @("HUD9_Panel_Wave.png", "HUD9_Panel_HP.png", "HUD9_Panel_Level.png", "HUD9_Panel_ChipMini.png", "HUD9_Bar_Back.png", "HUD9_Bar_HP_PlayerFill.png", "HUD9_Bar_HP_CoreFill.png", "HUD9_Bar_EXP_Fill.png")
    },
    @{
        Title = "STAGE 4/5 SUPPORT - stage identity assets"
        Note = "Frost and storm stage support assets exist but are not statically loaded yet."
        Accent = (New-Color 145 215 255)
        Status = "WAITING"
        Files = @("StageThumb_Frost.png", "StageThumb_Storm.png", "Stage4_FrostCrystal_A.png", "Stage4_FrostPatch_A.png", "Stage5_LightningMarker_A.png", "Stage5_LightningStrike_A.png")
    },
    @{
        Title = "V2 UI LEFTOVERS - selective hook only"
        Note = "Do not hook everything blindly. Prioritize warning frames, result slot, and HUD panels after visual QA."
        Accent = (New-Color 255 120 235)
        Status = "REVIEW"
        Files = @("Button_OptionsToggle_v2.png", "HUD_Panel_ChipMini_v2.png", "HUD_Panel_Data_v2.png", "HUD_Panel_EventLog_v2.png", "HUD_Panel_HP_v2.png", "HUD_Panel_Level_v2.png", "HUD_Panel_Links_v2.png", "HUD_Panel_Loadout_v2.png", "HUD_Panel_Stats_v2.png", "HUD_Panel_Wave_v2.png", "Result_MvpSlot_v2.png", "Warning_CoreMark_v2.png", "Warning_FinalBoss_Frame_v2.png", "Warning_MidBoss_Frame_v2.png")
    }
)

$canvasW = 1600
$tileW = 190
$tileH = 142
$gap = 20
$cols = 7
$margin = 48
$headerH = 78
$sectionGap = 34
$height = 78
foreach ($section in $sections) {
    $rows = [Math]::Ceiling($section.Files.Count / [double]$cols)
    $height += $headerH + ($rows * $tileH) + (($rows - 1) * $gap) + $sectionGap
}
$canvasH = [int]($height + 24)

$bmp = [System.Drawing.Bitmap]::new($canvasW, $canvasH, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
$g.Clear((New-Color 3 10 15))

$gridPen = [System.Drawing.Pen]::new((New-Color 15 84 96 100), 1)
for ($x = 0; $x -lt $canvasW; $x += 80) {
    $g.DrawLine($gridPen, $x, 0, $x, $canvasH)
}
for ($y = 0; $y -lt $canvasH; $y += 80) {
    $g.DrawLine($gridPen, 0, $y, $canvasW, $y)
}
$gridPen.Dispose()

Draw-Text $g "NON-CHARACTER ASSET QUEUE" 0 18 $canvasW 34 (New-Color 180 255 255) 30 ([System.Drawing.StringAlignment]::Center) -Bold
Draw-Text $g "Generated by Codex without CoreLanternGame.cs edits. Code hook-up remains Claude-owned." 0 54 $canvasW 22 (New-Color 125 205 215) 14 ([System.Drawing.StringAlignment]::Center)

$y = 90
foreach ($section in $sections) {
    $accent = $section.Accent
    $sectionPen = [System.Drawing.Pen]::new($accent, 2)
    $g.DrawLine($sectionPen, $margin, $y + 10, $canvasW - $margin, $y + 10)
    $sectionPen.Dispose()

    Draw-Text $g $section.Title $margin ($y + 18) ($canvasW - ($margin * 2)) 26 $accent 22 ([System.Drawing.StringAlignment]::Near) -Bold
    Draw-Text $g $section.Note $margin ($y + 48) ($canvasW - ($margin * 2)) 20 (New-Color 175 220 225) 13 ([System.Drawing.StringAlignment]::Near)

    $tileY = $y + $headerH
    for ($i = 0; $i -lt $section.Files.Count; $i++) {
        $col = $i % $cols
        $row = [Math]::Floor($i / $cols)
        $tileX = $margin + ($col * ($tileW + $gap))
        $curY = $tileY + ($row * ($tileH + $gap))
        Draw-AssetTile $g $section.Files[$i] $tileX $curY $tileW $tileH $section.Status $accent
    }

    $rows = [Math]::Ceiling($section.Files.Count / [double]$cols)
    $y = $tileY + ($rows * $tileH) + (($rows - 1) * $gap) + $sectionGap
}

try {
    $bmp.Save($OutPath, [System.Drawing.Imaging.ImageFormat]::Png)
} finally {
    $g.Dispose()
    $bmp.Dispose()
}

$readme = @"
# Non-Character Asset Queue Preview

Generated: 2026-06-01

This folder is a visual queue for non-character assets that can be worked on
without touching `Assets/Scripts/CoreLanternGame.cs`.

- `NonCharacterAssetQueue_20260601.png`: grouped preview sheet.
- Code hook-up remains Claude-owned.
- Codex can regenerate/validate assets and update docs in this area.
"@

[System.IO.File]::WriteAllText((Join-Path $OutDir "README.md"), $readme, [System.Text.UTF8Encoding]::new($false))

Write-Host "Generated: $OutPath"
