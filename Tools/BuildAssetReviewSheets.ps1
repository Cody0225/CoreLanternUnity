Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Drawing

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$SkinDir = Join-Path $ProjectRoot "Assets\Resources\Skins"
$ArtDir = Join-Path $ProjectRoot "Assets\ArtSource"
$ReportPath = Join-Path $ProjectRoot "ASSET_REVIEW_REPORT.md"

New-Item -ItemType Directory -Force -Path $ArtDir | Out-Null

function New-Color {
    param([int]$R, [int]$G, [int]$B, [int]$A = 255)
    return [System.Drawing.Color]::FromArgb($A, $R, $G, $B)
}

function New-Canvas {
    param([int]$Width, [int]$Height)
    $bmp = [System.Drawing.Bitmap]::new($Width, $Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear((New-Color 2 12 18))
    return @{ Bitmap = $bmp; Graphics = $g }
}

function Save-Canvas {
    param($Canvas, [string]$Path)
    $Canvas.Bitmap.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $Canvas.Graphics.Dispose()
    $Canvas.Bitmap.Dispose()
}

function New-Font {
    param([float]$Size, [System.Drawing.FontStyle]$Style = [System.Drawing.FontStyle]::Regular)
    return [System.Drawing.Font]::new("Segoe UI", $Size, $Style, [System.Drawing.GraphicsUnit]::Pixel)
}

function Draw-Text {
    param($G, [string]$Text, [float]$X, [float]$Y, [float]$W, [float]$H, [System.Drawing.Color]$Color, [float]$Size = 14, [System.Drawing.StringAlignment]$Align = [System.Drawing.StringAlignment]::Near, [switch]$Bold)
    $font = New-Font $Size $(if ($Bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular })
    $brush = [System.Drawing.SolidBrush]::new($Color)
    $format = [System.Drawing.StringFormat]::new()
    $format.Alignment = $Align
    $format.LineAlignment = [System.Drawing.StringAlignment]::Center
    $G.DrawString($Text, $font, $brush, [System.Drawing.RectangleF]::new($X, $Y, $W, $H), $format)
    $format.Dispose()
    $brush.Dispose()
    $font.Dispose()
}

function Draw-Line {
    param($G, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2, [System.Drawing.Color]$Color, [float]$Width = 1.0)
    $pen = [System.Drawing.Pen]::new($Color, $Width)
    $G.DrawLine($pen, $X1, $Y1, $X2, $Y2)
    $pen.Dispose()
}

function Draw-Cell {
    param($G, [float]$X, [float]$Y, [float]$W, [float]$H, [string]$FileName, [string]$Label, [System.Drawing.Color]$Accent, [switch]$Missing)
    $back = [System.Drawing.SolidBrush]::new((New-Color 5 20 28 235))
    $G.FillRectangle($back, $X, $Y, $W, $H)
    $back.Dispose()

    $pen = [System.Drawing.Pen]::new($Accent, 1.6)
    $G.DrawRectangle($pen, $X, $Y, $W, $H)
    $pen.Dispose()

    $inner = [System.Drawing.RectangleF]::new($X + 7, $Y + 8, $W - 14, $H - 30)
    $path = Join-Path $SkinDir $FileName
    if ((-not $Missing) -and (Test-Path -LiteralPath $path)) {
        $img = [System.Drawing.Image]::FromFile($path)
        try {
            $scale = [Math]::Min($inner.Width / $img.Width, $inner.Height / $img.Height)
            $dw = $img.Width * $scale
            $dh = $img.Height * $scale
            $dx = $inner.X + ($inner.Width - $dw) / 2
            $dy = $inner.Y + ($inner.Height - $dh) / 2
            $G.DrawImage($img, [System.Drawing.RectangleF]::new($dx, $dy, $dw, $dh))
        }
        finally {
            $img.Dispose()
        }
    }
    else {
        $missingBrush = [System.Drawing.SolidBrush]::new((New-Color 20 4 8 210))
        $G.FillRectangle($missingBrush, $inner)
        $missingBrush.Dispose()
        Draw-Text $G "MISSING" $inner.X $inner.Y $inner.Width $inner.Height (New-Color 255 95 130) 13 ([System.Drawing.StringAlignment]::Center) -Bold
    }

    Draw-Text $G $Label ($X + 4) ($Y + $H - 23) ($W - 8) 18 (New-Color 205 245 245) 11 ([System.Drawing.StringAlignment]::Center) -Bold
}

function Get-Exists {
    param([string]$FileName)
    return Test-Path -LiteralPath (Join-Path $SkinDir $FileName)
}

function Build-PartnerRouteSheet {
    $speciesNames = @("Cobalt", "Ember", "Sage", "Hex", "Drift", "Iron", "Wraith", "Genesis")
    $routeNames = @("SPEED", "POWER", "GUARD")
    $routeAccent = @((New-Color 30 240 255), (New-Color 255 170 38), (New-Color 98 245 120))
    $cellW = 86
    $cellH = 94
    $leftW = 96
    $margin = 26
    $top = 74
    $width = $margin * 2 + $leftW + ($cellW * 13)
    $height = $top + ($cellH * $speciesNames.Count) + 38
    $canvas = New-Canvas $width $height
    $g = $canvas.Graphics

    Draw-Text $g "PARTNER ROUTE REVIEW" 0 14 $width 32 (New-Color 180 255 255) 28 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $g "Base + SPEED / POWER / GUARD stages. Use this to catch name/silhouette mismatches." 0 47 $width 20 (New-Color 120 200 210) 13 ([System.Drawing.StringAlignment]::Center)

    Draw-Text $g "Species" $margin $top $leftW 22 (New-Color 255 230 90) 13 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $g "Base" ($margin + $leftW) $top $cellW 22 (New-Color 255 230 90) 13 ([System.Drawing.StringAlignment]::Center) -Bold
    for ($r = 0; $r -lt 3; $r++) {
        $x = $margin + $leftW + $cellW + ($r * $cellW * 4)
        Draw-Text $g $routeNames[$r] $x $top ($cellW * 4) 22 $routeAccent[$r] 14 ([System.Drawing.StringAlignment]::Center) -Bold
        Draw-Line $g ($x + 8) ($top + 23) ($x + $cellW * 4 - 8) ($top + 23) $routeAccent[$r] 2
    }

    for ($s = 1; $s -le $speciesNames.Count; $s++) {
        $rowY = $top + 28 + (($s - 1) * $cellH)
        Draw-Text $g ("S{0}`n{1}" -f $s, $speciesNames[$s - 1]) $margin ($rowY + 6) $leftW ($cellH - 12) (New-Color 180 255 255) 14 ([System.Drawing.StringAlignment]::Center) -Bold
        Draw-Cell $g ($margin + $leftW) $rowY $cellW ($cellH - 8) ("Partner_S{0}_L0.png" -f $s) "BASE" (New-Color 70 235 235)
        for ($r = 1; $r -le 3; $r++) {
            for ($stage = 0; $stage -le 3; $stage++) {
                $x = $margin + $leftW + $cellW + ((($r - 1) * 4 + $stage) * $cellW)
                $name = "Partner_S{0}_R{1}_L{2}.png" -f $s, $r, $stage
                Draw-Cell $g $x $rowY $cellW ($cellH - 8) $name ("R{0} L{1}" -f $r, $stage) $routeAccent[$r - 1] -Missing:(-not (Get-Exists $name))
            }
        }
    }

    $path = Join-Path $ArtDir "AssetReview_PartnerRoutes.png"
    Save-Canvas $canvas $path
    return $path
}

function Build-FusionSheet {
    $speciesNames = @("Cobalt", "Ember", "Sage", "Hex", "Drift", "Iron", "Wraith", "Genesis")
    $fusionNames = @("Nova Aegis", "Photon Siphon", "Core Bastion", "Nova Phantom", "Aegis Drift", "Photon Wraith")
    $accent = @((New-Color 255 80 230), (New-Color 125 255 90), (New-Color 255 215 80), (New-Color 90 230 255), (New-Color 190 130 255), (New-Color 95 255 205))
    $cellW = 130
    $cellH = 116
    $leftW = 98
    $margin = 28
    $top = 86
    $width = $margin * 2 + $leftW + $cellW * 6
    $height = $top + $cellH * $speciesNames.Count + 34
    $canvas = New-Canvas $width $height
    $g = $canvas.Graphics

    Draw-Text $g "CROSS EVOLVE L3 REVIEW" 0 14 $width 32 (New-Color 255 155 255) 28 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $g "Species-specific fusion endpoints. L0-L2 are still intentionally missing for now." 0 48 $width 22 (New-Color 210 170 230) 13 ([System.Drawing.StringAlignment]::Center)
    for ($f = 1; $f -le 6; $f++) {
        $x = $margin + $leftW + (($f - 1) * $cellW)
        Draw-Text $g ("F{0}`n{1}" -f $f, $fusionNames[$f - 1]) $x $top $cellW 36 $accent[$f - 1] 12 ([System.Drawing.StringAlignment]::Center) -Bold
    }

    for ($s = 1; $s -le $speciesNames.Count; $s++) {
        $rowY = $top + 38 + (($s - 1) * $cellH)
        Draw-Text $g ("S{0}`n{1}" -f $s, $speciesNames[$s - 1]) $margin ($rowY + 10) $leftW 84 (New-Color 180 255 255) 14 ([System.Drawing.StringAlignment]::Center) -Bold
        for ($f = 1; $f -le 6; $f++) {
            $name = "Partner_S{0}_F{1}_L3.png" -f $s, $f
            $x = $margin + $leftW + (($f - 1) * $cellW)
            Draw-Cell $g $x $rowY ($cellW - 8) 104 $name ("S{0} F{1}" -f $s, $f) $accent[$f - 1] -Missing:(-not (Get-Exists $name))
        }
    }

    $path = Join-Path $ArtDir "AssetReview_FusionL3.png"
    Save-Canvas $canvas $path
    return $path
}

function Build-FusionStageSheet {
    $speciesNames = @("Cobalt", "Ember", "Sage", "Hex", "Drift", "Iron", "Wraith", "Genesis")
    $fusionNames = @("F1 Nova", "F2 Siphon", "F3 Bastion", "F4 Phantom", "F5 Drift", "F6 Wraith")
    $accent = @((New-Color 255 80 230), (New-Color 125 255 90), (New-Color 255 215 80), (New-Color 90 230 255), (New-Color 190 130 255), (New-Color 95 255 205))
    $cellW = 186
    $cellH = 128
    $leftW = 78
    $margin = 28
    $top = 92
    $width = $margin * 2 + $leftW + $cellW * 6
    $height = $top + $cellH * $speciesNames.Count + 34
    $canvas = New-Canvas $width $height
    $g = $canvas.Graphics

    Draw-Text $g "CROSS EVOLVE STAGE REVIEW" 0 14 $width 32 (New-Color 255 155 255) 28 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $g "Each cell shows L0 -> L1 -> L2 -> L3. Check whether the fusion reads as gradual absorption." 0 48 $width 22 (New-Color 210 170 230) 13 ([System.Drawing.StringAlignment]::Center)
    for ($f = 1; $f -le 6; $f++) {
        $x = $margin + $leftW + (($f - 1) * $cellW)
        Draw-Text $g $fusionNames[$f - 1] $x 72 $cellW 18 $accent[$f - 1] 12 ([System.Drawing.StringAlignment]::Center) -Bold
    }

    for ($s = 1; $s -le $speciesNames.Count; $s++) {
        $rowY = $top + (($s - 1) * $cellH)
        Draw-Text $g ("S{0}`n{1}" -f $s, $speciesNames[$s - 1]) $margin ($rowY + 18) $leftW 72 (New-Color 180 255 255) 13 ([System.Drawing.StringAlignment]::Center) -Bold
        for ($f = 1; $f -le 6; $f++) {
            $x = $margin + $leftW + (($f - 1) * $cellW)
            $back = [System.Drawing.SolidBrush]::new((New-Color 3 18 26 235))
            $g.FillRectangle($back, $x, $rowY, $cellW - 8, $cellH - 10)
            $back.Dispose()
            $pen = [System.Drawing.Pen]::new($accent[$f - 1], 1.4)
            $g.DrawRectangle($pen, $x, $rowY, $cellW - 8, $cellH - 10)
            $pen.Dispose()

            for ($stage = 0; $stage -le 3; $stage++) {
                $name = "Partner_S{0}_F{1}_L{2}.png" -f $s, $f, $stage
                $path = Join-Path $SkinDir $name
                if (Test-Path -LiteralPath $path) {
                    $img = [System.Drawing.Image]::FromFile($path)
                    try {
                        $slotX = $x + 8 + $stage * 42
                        $slotY = $rowY + 16
                        $g.DrawImage($img, [System.Drawing.RectangleF]::new($slotX, $slotY, 38, 38))
                        Draw-Text $g ("L{0}" -f $stage) ($slotX - 2) ($slotY + 44) 42 16 (New-Color 220 245 245) 10 ([System.Drawing.StringAlignment]::Center) -Bold
                    }
                    finally {
                        $img.Dispose()
                    }
                }
            }
            Draw-Text $g ("S{0} F{1}" -f $s, $f) $x ($rowY + $cellH - 30) ($cellW - 8) 16 (New-Color 210 245 245) 10 ([System.Drawing.StringAlignment]::Center)
        }
    }

    $path = Join-Path $ArtDir "AssetReview_FusionStages.png"
    Save-Canvas $canvas $path
    return $path
}

function Build-EnvironmentSheet {
    $assets = @(
        @{ Group = "Enemy"; Name = "Runner.png" },
        @{ Group = "Enemy"; Name = "Brute.png" },
        @{ Group = "Enemy"; Name = "Shooter.png" },
        @{ Group = "Enemy"; Name = "Dasher.png" },
        @{ Group = "Enemy"; Name = "Bomber.png" },
        @{ Group = "Enemy"; Name = "Phantom.png" },
        @{ Group = "Enemy"; Name = "Boss.png" },
        @{ Group = "Enemy"; Name = "Boss_Warning.png" },
        @{ Group = "Map"; Name = "Floor_TileA.png" },
        @{ Group = "Map"; Name = "Floor_TileB.png" },
        @{ Group = "Map"; Name = "Floor_TileC.png" },
        @{ Group = "Map"; Name = "Floor_DarkBase_A.png" },
        @{ Group = "Map"; Name = "Floor_GridOverlay_A.png" },
        @{ Group = "Map"; Name = "Floor_CoreMark_A.png" },
        @{ Group = "Map"; Name = "Boundary_Stone_A.png" },
        @{ Group = "Map"; Name = "Boundary_Stone_B.png" },
        @{ Group = "Prop"; Name = "Prop_ServerDebris_A.png" },
        @{ Group = "Prop"; Name = "Prop_ServerDebris_B.png" },
        @{ Group = "Prop"; Name = "Prop_NeonPylon_A.png" },
        @{ Group = "Prop"; Name = "Prop_DataTerminal_A.png" },
        @{ Group = "Core"; Name = "Core_Platform.png" },
        @{ Group = "Core"; Name = "Core_RingOuter.png" },
        @{ Group = "Core"; Name = "Core_RingInner.png" },
        @{ Group = "UI"; Name = "Card_Cyan.png" },
        @{ Group = "UI"; Name = "Card_Gold.png" },
        @{ Group = "UI"; Name = "Card_Green.png" },
        @{ Group = "UI"; Name = "Panel_FrameCyan.png" },
        @{ Group = "UI"; Name = "Panel_FrameGold.png" },
        @{ Group = "HUD"; Name = "HUD_Panel_HP.png" },
        @{ Group = "HUD"; Name = "HUD_Bar_EXP_Fill.png" },
        @{ Group = "HUD"; Name = "Minimap_Frame.png" },
        @{ Group = "HUD"; Name = "Minimap_Dot_Boss.png" }
    )

    $cols = 8
    $cellW = 148
    $cellH = 114
    $margin = 26
    $top = 82
    $rows = [Math]::Ceiling($assets.Count / $cols)
    $width = $margin * 2 + $cols * $cellW
    $height = $top + $rows * $cellH + 36
    $canvas = New-Canvas $width $height
    $g = $canvas.Graphics

    Draw-Text $g "ENEMY / MAP / UI REVIEW" 0 14 $width 32 (New-Color 180 255 255) 28 ([System.Drawing.StringAlignment]::Center) -Bold
    Draw-Text $g "Includes missing HUD targets so the next code hook work has a clear asset checklist." 0 48 $width 22 (New-Color 120 200 210) 13 ([System.Drawing.StringAlignment]::Center)

    for ($i = 0; $i -lt $assets.Count; $i++) {
        $col = $i % $cols
        $row = [Math]::Floor($i / $cols)
        $x = $margin + $col * $cellW
        $y = $top + $row * $cellH
        $asset = $assets[$i]
        $exists = Get-Exists $asset.Name
        $accent = if ($exists) { New-Color 40 230 240 } else { New-Color 255 80 120 }
        Draw-Cell $g $x $y ($cellW - 10) ($cellH - 10) $asset.Name ("{0}`n{1}" -f $asset.Group, ($asset.Name -replace "\.png$", "")) $accent -Missing:(-not $exists)
    }

    $path = Join-Path $ArtDir "AssetReview_EnemiesMapUi.png"
    Save-Canvas $canvas $path
    return $path
}

function Build-Report {
    param([string[]]$SheetPaths)

    $allPng = @(Get-ChildItem -LiteralPath $SkinDir -File -Filter "*.png")
    $disabled = @(Get-ChildItem -LiteralPath $SkinDir -File | Where-Object { $_.Name -like "*.disabled" })
    $partnerPng = @($allPng | Where-Object { $_.Name -match '^Partner_' })
    $routePng = @($allPng | Where-Object { $_.Name -match '^Route_' })
    $fusionPng = @($allPng | Where-Object { $_.Name -match '^Fusion_' })
    $enemyNames = @("Runner.png", "Brute.png", "Shooter.png", "Dasher.png", "Bomber.png", "Phantom.png", "Boss.png", "Boss_Warning.png")
    $hudTargets = @(
        "HUD_Panel_Wave.png", "HUD_Panel_HP.png", "HUD_Panel_Level.png", "HUD_Panel_ChipMini.png",
        "HUD_Panel_Status.png", "HUD_Panel_Loadout.png",
        "HUD_Bar_Back.png", "HUD_Bar_HP_PlayerFill.png", "HUD_Bar_HP_CoreFill.png", "HUD_Bar_HP_Lag.png", "HUD_Bar_EXP_Fill.png",
        "HUD_WaveProgress_Frame.png", "HUD_WaveProgress_Fill_Normal.png", "HUD_WaveProgress_Fill_Boss.png",
        "HUD_BossBar_Frame.png", "HUD_BossBar_Fill.png", "HUD_ChoiceDim.png", "HUD_DangerVignette_Player.png", "HUD_DangerVignette_Core.png",
        "HUD_BossWarning_Banner.png", "HUD_RelicSlot_Frame.png",
        "HUD_Icon_PlayerHP.png", "HUD_Icon_CoreHP.png", "HUD_Icon_EXP.png", "HUD_Icon_DataChip.png",
        "Relic_Magnet.png", "Relic_Deathless.png", "Relic_CorePulse.png", "Relic_Explosion.png", "Relic_Storm.png", "Relic_Mirror.png",
        "Minimap_Frame.png", "Minimap_Backplate.png", "Minimap_Grid.png", "Minimap_Dot_Player.png", "Minimap_Dot_Core.png", "Minimap_Dot_Enemy.png", "Minimap_Dot_Boss.png",
        "Minimap_Dot_Pickup.png", "Minimap_Ping_Wave.png"
    )

    $missingFusionStages = New-Object System.Collections.Generic.List[string]
    for ($s = 1; $s -le 8; $s++) {
        for ($f = 1; $f -le 6; $f++) {
            for ($l = 0; $l -le 2; $l++) {
                $name = "Partner_S{0}_F{1}_L{2}.png" -f $s, $f, $l
                if (-not (Get-Exists $name)) { $missingFusionStages.Add($name) }
            }
        }
    }

    $missingRoutes = New-Object System.Collections.Generic.List[string]
    for ($s = 1; $s -le 8; $s++) {
        for ($r = 1; $r -le 3; $r++) {
            for ($l = 0; $l -le 3; $l++) {
                $name = "Partner_S{0}_R{1}_L{2}.png" -f $s, $r, $l
                if (-not (Get-Exists $name)) { $missingRoutes.Add($name) }
            }
        }
    }

    $missingEnemies = @($enemyNames | Where-Object { -not (Get-Exists $_) })
    $missingHud = @($hudTargets | Where-Object { -not (Get-Exists $_) })

    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("# CoreLanternUnity Asset Review Report")
    $lines.Add("")
    $lines.Add("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
    $lines.Add("")
    $lines.Add('This pass intentionally avoids `Assets/Scripts/CoreLanternGame.cs` so Claude can keep working on gameplay/UI without file conflicts.')
    $lines.Add("")
    $lines.Add("## Generated Review Sheets")
    foreach ($sheet in $SheetPaths) {
        $rel = $sheet.Substring($ProjectRoot.Length + 1).Replace('\', '/')
        $lines.Add('- `' + $rel + '`')
    }
    $hudPreview = Join-Path $ArtDir "Generated_HudAsset_Preview.png"
    if (Test-Path -LiteralPath $hudPreview) {
        $rel = $hudPreview.Substring($ProjectRoot.Length + 1).Replace('\', '/')
        $lines.Add('- `' + $rel + '`')
    }
    $fusionStagePreview = Join-Path $ArtDir "Generated_FusionStage_Preview.png"
    if (Test-Path -LiteralPath $fusionStagePreview) {
        $rel = $fusionStagePreview.Substring($ProjectRoot.Length + 1).Replace('\', '/')
        $lines.Add('- `' + $rel + '`')
    }
    $newPartnerPreview = Join-Path $ArtDir "Generated_NewPartnerRelicUi_Preview.png"
    if (Test-Path -LiteralPath $newPartnerPreview) {
        $rel = $newPartnerPreview.Substring($ProjectRoot.Length + 1).Replace('\', '/')
        $lines.Add('- `' + $rel + '`')
    }
    $lines.Add("")
    $lines.Add("## Inventory Snapshot")
    $lines.Add("")
    $lines.Add("| Category | Count | Notes |")
    $lines.Add("|---|---:|---|")
    $lines.Add("| All active PNG | $($allPng.Count) | Assets/Resources/Skins |")
    $lines.Add("| Partner PNG | $($partnerPng.Count) | Includes route and fusion variants for species 1-8 |")
    $lines.Add("| Route PNG | $($routePng.Count) | Generic route sprites |")
    $lines.Add("| Fusion PNG | $($fusionPng.Count) | Generic fusion sprites |")
    $lines.Add("| Disabled/backup files | $($disabled.Count) | Kept for rollback/reference |")
    $lines.Add("| Relic PNG | $(@($allPng | Where-Object { $_.Name -match '^Relic_' }).Count) | Relic icons |")
    $lines.Add("| HUD PNG | $(@($allPng | Where-Object { $_.Name -match '^HUD_' }).Count) | HUD/UI hook targets |")
    $lines.Add("| Minimap PNG | $(@($allPng | Where-Object { $_.Name -match '^Minimap_' }).Count) | Minimap assets |")
    $lines.Add("")
    $lines.Add("## Missing / Next Targets")
    $lines.Add("")
    $lines.Add("- Partner route set missing: $($missingRoutes.Count) files.")
    if ($missingFusionStages.Count -eq 0) {
        $lines.Add("- Species-specific fusion L0-L2 missing: 0 files. Full L0-L3 temporary set exists for all 8 species x 6 fusions.")
    }
    else {
        $lines.Add("- Species-specific fusion L0-L2 missing: $($missingFusionStages.Count) files. L3 endpoints exist; intermediate stages still need generation.")
    }
    $lines.Add("- Enemy core set missing: $($missingEnemies.Count) files.")
    $lines.Add("- HUD/minimap dedicated asset targets missing: $($missingHud.Count) files.")
    $lines.Add("")
    $lines.Add("## Safe Parallel Work Queue")
    $lines.Add("")
    $lines.Add('1. Review `AssetReview_PartnerRoutes.png` for name/silhouette mismatches, especially Hex, Drift, and Iron.')
    $lines.Add('2. Review `AssetReview_FusionStages.png` for whether fusion reads as gradual absorption from L0 to L3.')
    $lines.Add('3. Review `AssetReview_FusionL3.png` for whether fusion endpoints look like the selected species absorbed the link, not generic icons.')
    $lines.Add("4. Review the generated HUD/minimap pack visually, then keep it asset-only until Claude finishes code-side hooks.")
    $lines.Add('5. Review `Generated_NewPartnerRelicUi_Preview.png` for Wraith/Genesis readability and relic icon clarity.')
    $lines.Add("6. After Claude's code changes settle, hook HUD/relic/minimap assets from " + '`Assets/Resources/Skins`' + " into panel/bar/minimap/relic UI creation.")
    $lines.Add("")
    $lines.Add("## HUD Targets Still Missing")
    if ($missingHud.Count -eq 0) {
        $lines.Add("- None for the current P1 target list.")
    }
    else {
        foreach ($name in $missingHud) { $lines.Add('- `' + $name + '`') }
    }

    Set-Content -LiteralPath $ReportPath -Value $lines -Encoding UTF8
    return $ReportPath
}

$sheets = @(
    (Build-PartnerRouteSheet),
    (Build-FusionSheet),
    (Build-FusionStageSheet),
    (Build-EnvironmentSheet)
)

$report = Build-Report $sheets

Write-Output "Generated review sheets:"
$sheets | ForEach-Object { Write-Output " - $_" }
Write-Output "Generated report:"
Write-Output " - $report"
