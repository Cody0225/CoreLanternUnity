param(
    [switch]$NoOverwrite
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Split-Path -Parent $PSScriptRoot
$AudioDir = Join-Path $ProjectRoot "Assets\Resources\Audio"
$LicenseDir = Join-Path $AudioDir "Licenses"
$IncomingDir = Join-Path $AudioDir "_incoming"

New-Item -ItemType Directory -Force -Path $AudioDir, $LicenseDir, $IncomingDir | Out-Null

$SampleRate = 22050
$Manifest = @()

function Clamp01Signed([double]$v) {
    if ($v -gt 1.0) { return 1.0 }
    if ($v -lt -1.0) { return -1.0 }
    return $v
}

function Tone([double]$freq, [double]$t) {
    return [Math]::Sin(2.0 * [Math]::PI * $freq * $t)
}

function Tri([double]$freq, [double]$t) {
    $x = ($freq * $t) % 1.0
    if ($x -lt 0.5) { return (4.0 * $x - 1.0) }
    return (3.0 - 4.0 * $x)
}

function Noise([int]$i) {
    $v = [Math]::Sin($i * 127.1 + 0.3) * 43758.5453
    return 2.0 * ($v - [Math]::Floor($v)) - 1.0
}

function EnvAD([double]$t, [double]$duration, [double]$attack, [double]$release) {
    $a = if ($attack -le 0) { 1.0 } else { [Math]::Min(1.0, $t / $attack) }
    $r = if ($release -le 0) { 1.0 } else { [Math]::Min(1.0, ($duration - $t) / $release) }
    return [Math]::Max(0.0, [Math]::Min($a, $r))
}

function Write-Wav([string]$Path, [double[]]$Samples, [int]$Rate) {
    $sampleCount = $Samples.Length
    $dataSize = $sampleCount * 2
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter($ms)

    $bw.Write([Text.Encoding]::ASCII.GetBytes("RIFF"))
    $bw.Write([int](36 + $dataSize))
    $bw.Write([Text.Encoding]::ASCII.GetBytes("WAVE"))
    $bw.Write([Text.Encoding]::ASCII.GetBytes("fmt "))
    $bw.Write([int]16)
    $bw.Write([int16]1)
    $bw.Write([int16]1)
    $bw.Write([int]$Rate)
    $bw.Write([int]($Rate * 2))
    $bw.Write([int16]2)
    $bw.Write([int16]16)
    $bw.Write([Text.Encoding]::ASCII.GetBytes("data"))
    $bw.Write([int]$dataSize)

    for ($i = 0; $i -lt $sampleCount; $i++) {
        $v = Clamp01Signed $Samples[$i]
        $bw.Write([int16][Math]::Round($v * 32767.0))
    }

    [System.IO.File]::WriteAllBytes($Path, $ms.ToArray())
    $bw.Dispose()
    $ms.Dispose()
}

function Ensure-AudioMeta([string]$Path) {
    $metaPath = "$Path.meta"
    if (Test-Path -LiteralPath $metaPath) { return }

    $guid = [guid]::NewGuid().ToString("N")
    $meta = @"
fileFormatVersion: 2
guid: $guid
AudioImporter:
  externalObjects: {}
  serializedVersion: 8
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
    preloadAudioData: 0
  platformSettingOverrides: {}
  forceToMono: 0
  normalize: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"@
    Set-Content -LiteralPath $metaPath -Value $meta -Encoding UTF8
}

function Save-Clip([string]$Name, [scriptblock]$MakeSamples, [string]$Kind, [string]$Use) {
    $path = Join-Path $AudioDir ($Name + ".wav")
    if ($NoOverwrite -and (Test-Path -LiteralPath $path)) {
        Write-Host "Skip existing $Name.wav"
        $length = (Get-Item -LiteralPath $path).Length
        $duration = [Math]::Max(0.0, ($length - 44) / 2.0 / $SampleRate)
        $script:Manifest += [pscustomobject]@{
            file = "Assets/Resources/Audio/$Name.wav"
            id = $Name
            kind = $Kind
            use = $Use
            source = "Generated locally by Tools/GenerateStarterAudio.ps1"
            license = "Project-original deterministic synthesis; no third-party samples"
            sampleRate = $SampleRate
            durationSeconds = [Math]::Round($duration, 3)
        }
        return
    }

    $Samples = & $MakeSamples
    Write-Wav -Path $path -Samples $Samples -Rate $SampleRate
    Ensure-AudioMeta $path
    $script:Manifest += [pscustomobject]@{
        file = "Assets/Resources/Audio/$Name.wav"
        id = $Name
        kind = $Kind
        use = $Use
        source = "Generated locally by Tools/GenerateStarterAudio.ps1"
        license = "Project-original deterministic synthesis; no third-party samples"
        sampleRate = $SampleRate
        durationSeconds = [Math]::Round($Samples.Length / $SampleRate, 3)
    }
    Write-Host "Generated $Name.wav"
}

function New-Clip([double]$Duration) {
    return New-Object double[] ([int][Math]::Ceiling($SampleRate * $Duration))
}

function New-Shoot {
    $duration = 0.095
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $env = [Math]::Pow(1.0 - ($t / $duration), 2.2)
        $freq = 760.0 - 260.0 * ($t / $duration)
        $body = (Tone $freq $t) * 0.22 + (Tone ($freq * 0.5) $t) * 0.08
        $s[$i] = ($body + (Noise $i) * 0.012) * $env
    }
    return $s
}

function New-Hit {
    $duration = 0.12
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $env = [Math]::Exp(-34.0 * $t)
        $s[$i] = ((Noise $i) * 0.13 + (Tone 220 $t) * 0.10 + (Tone 110 $t) * 0.05) * $env
    }
    return $s
}

function New-Kill {
    $duration = 0.24
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $p = $t / $duration
        $env = [Math]::Pow(1.0 - $p, 1.7)
        $s[$i] = ((Tone (360 - 180 * $p) $t) * 0.14 + (Tone (180 - 90 * $p) $t) * 0.08 + (Noise $i) * 0.035) * $env
    }
    return $s
}

function New-Pickup {
    $duration = 0.16
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $p = $t / $duration
        $env = EnvAD $t $duration 0.006 0.055
        $freq = if ($p -lt 0.5) { 660.0 } else { 554.37 }
        $s[$i] = ((Tone $freq $t) * 0.14 + (Tone ($freq * 0.5) $t) * 0.04) * $env
    }
    return $s
}

function New-LevelUp {
    $duration = 0.72
    $s = New-Clip $duration
    $notes = @(660.0, 554.37, 493.88, 440.0)
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $sum = 0.0
        for ($n = 0; $n -lt $notes.Count; $n++) {
            $start = $n * 0.12
            if ($t -ge $start) {
                $lt = $t - $start
                $env = [Math]::Exp(-5.5 * $lt) * (EnvAD $lt 0.5 0.012 0.12)
                $sum += (Tone $notes[$n] $lt) * $env * 0.10
            }
        }
        $s[$i] = $sum
    }
    return $s
}

function New-Evolve {
    $duration = 1.42
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $p = $t / $duration
        $charge = [Math]::Min(1.0, $p * 1.3)
        $sweepFreq = 420.0 - 150.0 * $p
        $burst = [Math]::Exp(-18.0 * [Math]::Max(0.0, $t - 1.02))
        $main = (Tone $sweepFreq $t) * 0.11 * $charge
        $ring = (Tone 330 $t) * 0.09 * $burst + (Tone 165 $t) * 0.07 * $burst
        $spark = (Noise $i) * 0.025 * $burst
        $s[$i] = ($main + $ring + $spark) * (EnvAD $t $duration 0.04 0.22)
    }
    return $s
}

function New-Fusion {
    $duration = 1.88
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $p = $t / $duration
        $left = 300.0 - 60.0 * $p
        $right = 420.0 - 110.0 * $p
        $merge = [Math]::Exp(-12.0 * [Math]::Abs($p - 0.72))
        $tail = [Math]::Exp(-8.0 * [Math]::Max(0.0, $t - 1.35))
        $s[$i] = ((Tone $left $t) * 0.10 + (Tone $right $t) * 0.08 + (Tone 330 $t) * 0.08 * $merge + (Noise $i) * 0.025 * $tail) * (EnvAD $t $duration 0.05 0.32)
    }
    return $s
}

function New-Boss {
    $duration = 0.92
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $pulse = ([Math]::Floor($t * 6.0) % 2)
        $env = if ($pulse -eq 0) { 0.9 } else { 0.35 }
        $s[$i] = ((Tone 96 $t) * 0.16 + (Tone 288 $t) * 0.06 + (Noise $i) * 0.02) * $env * (EnvAD $t $duration 0.02 0.18)
    }
    return $s
}

function New-GameOver {
    $duration = 1.45
    $s = New-Clip $duration
    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $p = $t / $duration
        $freq = 220.0 - 110.0 * $p
        $s[$i] = ((Tone $freq $t) * 0.25 + (Tone ($freq * 0.5) $t) * 0.18 + (Noise $i) * 0.025) * (1.0 - $p) * (EnvAD $t $duration 0.02 0.35)
    }
    return $s
}

function New-Bgm {
    $duration = 24.0
    $s = New-Clip $duration
    $bpm = 104.0
    $stepLen = 60.0 / $bpm / 2.0
    # Keep the placeholder BGM steady and dark. Avoid ascending lead lines because
    # they become tiring during long survival runs.
    $bass = @(98.0, 0.0, 82.41, 0.0, 73.42, 0.0, 65.41, 0.0, 87.31, 0.0, 73.42, 0.0, 65.41, 0.0, 55.0, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $bassFreq = $bass[$step]
        $value = 0.0

        if ($bassFreq -gt 0.0) {
            $env = [Math]::Exp(-2.0 * $local)
            $value += (Tri $bassFreq $t) * 0.10 * $env
            $value += (Tone ($bassFreq * 0.5) $t) * 0.08 * $env
        }

        $kickPhase = $t % ((60.0 / $bpm) * 2.0)
        if ($kickPhase -lt 0.08) {
            $kEnv = [Math]::Exp(-34.0 * $kickPhase)
            $value += (Tone (60.0 - 16.0 * ($kickPhase / 0.08)) $t) * 0.14 * $kEnv
        }

        $hatPhase = $t % ($stepLen)
        if ($hatPhase -lt 0.026 -and ($step % 2 -eq 1)) {
            $value += (Noise $i) * 0.007 * [Math]::Exp(-82.0 * $hatPhase)
        }

        $padFreq = if (($bar % 8) -lt 2) { 82.41 } elseif (($bar % 8) -lt 4) { 73.42 } elseif (($bar % 8) -lt 6) { 65.41 } else { 55.0 }
        $padBreath = 0.68 + 0.32 * [Math]::Sin(2.0 * [Math]::PI * 0.0625 * $t)
        $value += ((Tri $padFreq $t) * 0.014 + (Tone ($padFreq * 0.75) $t) * 0.007) * $padBreath

        $fade = EnvAD $t $duration 0.08 0.08
        $s[$i] = $value * $fade
    }

    return $s
}

function New-BgmStage2 {
    $duration = 24.0
    $s = New-Clip $duration
    $bpm = 96.0
    $stepLen = 60.0 / $bpm / 2.0
    $bass = @(65.41, 0.0, 55.0, 0.0, 49.0, 0.0, 55.0, 0.0, 61.74, 0.0, 49.0, 0.0, 43.65, 0.0, 49.0, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $freq = $bass[$step]
        $value = 0.0

        if ($freq -gt 0.0) {
            $env = [Math]::Exp(-1.8 * $local)
            $value += (Tri $freq $t) * 0.105 * $env
            $value += (Tone ($freq * 0.5) $t) * 0.075 * $env
        }

        $kickPhase = $t % ((60.0 / $bpm) * 2.0)
        if ($kickPhase -lt 0.11) {
            $kEnv = [Math]::Exp(-25.0 * $kickPhase)
            $value += (Tone (48.0 - 10.0 * ($kickPhase / 0.11)) $t) * 0.13 * $kEnv
        }

        $ventPhase = $t % 2.8
        if ($ventPhase -lt 0.22) {
            $value += (Noise $i) * 0.014 * [Math]::Exp(-7.0 * $ventPhase)
            $value += (Tone 92.5 $t) * 0.011 * [Math]::Exp(-4.2 * $ventPhase)
        }

        $heatFreq = if (($bar % 8) -lt 4) { 55.0 } else { 43.65 }
        $heat = 0.64 + 0.36 * [Math]::Sin(2.0 * [Math]::PI * 0.047 * $t)
        $value += ((Tri $heatFreq $t) * 0.018 + (Tone ($heatFreq * 1.25) $t) * 0.006 + (Noise $i) * 0.004) * $heat

        $fade = EnvAD $t $duration 0.08 0.08
        $s[$i] = $value * $fade
    }

    return $s
}

function New-BgmStage3 {
    $duration = 28.0
    $s = New-Clip $duration
    $bpm = 92.0
    $beat = 60.0 / $bpm
    $stepLen = $beat / 2.0
    $bass = @(65.41, 0.0, 55.0, 0.0, 49.0, 0.0, 55.0, 0.0, 61.74, 0.0, 51.91, 0.0, 49.0, 0.0, 46.25, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $freq = $bass[$step]
        $value = 0.0

        if ($freq -gt 0.0) {
            $env = [Math]::Exp(-1.9 * $local)
            $value += (Tri $freq $t) * 0.095 * $env
            $value += (Tone ($freq * 0.5) $t) * 0.075 * $env
        }

        $pulsePhase = ($t + 0.04) % ($beat * 2.0)
        if ($pulsePhase -lt 0.09) {
            $pEnv = [Math]::Exp(-28.0 * $pulsePhase)
            $value += (Tone (56.0 - 12.0 * ($pulsePhase / 0.09)) $t) * 0.115 * $pEnv
        }

        $glitchPhase = ($t + 0.17) % ($beat * 5.0)
        if ($glitchPhase -lt 0.11) {
            $value += ((Noise ($i * 3)) * 0.010 + (Tone 82.41 $t) * 0.010) * [Math]::Exp(-18.0 * $glitchPhase)
        }

        $padFreq = if (($bar % 8) -lt 3) { 73.42 } elseif (($bar % 8) -lt 5) { 61.74 } else { 55.0 }
        $pad = 0.55 + 0.45 * [Math]::Sin(2.0 * [Math]::PI * 0.035 * $t)
        $value += ((Tri $padFreq $t) * 0.014 + (Tone ($padFreq * 0.75) $t) * 0.005 + (Noise $i) * 0.003) * $pad

        $fade = EnvAD $t $duration 0.10 0.12
        $s[$i] = $value * $fade
    }

    return $s
}

function New-BgmStage4 {
    $duration = 32.0
    $s = New-Clip $duration
    $bpm = 84.0
    $beat = 60.0 / $bpm
    $stepLen = $beat / 2.0
    $bass = @(65.41, 0.0, 49.0, 0.0, 55.0, 0.0, 41.2, 0.0, 65.41, 0.0, 46.25, 0.0, 55.0, 0.0, 49.0, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $freq = $bass[$step]
        $value = 0.0

        if ($freq -gt 0.0) {
            $env = [Math]::Exp(-1.45 * $local)
            $value += (Tri $freq $t) * 0.095 * $env
            $value += (Tone ($freq * 0.5) $t) * 0.095 * $env
        }

        $icePhase = ($t + 0.22) % ($beat * 4.0)
        if ($icePhase -lt 0.08) {
            $value += ((Tone 164.81 $t) * 0.010 + (Noise ($i * 5)) * 0.006) * [Math]::Exp(-22.0 * $icePhase)
        }

        $droneFreq = if (($bar % 8) -lt 4) { 65.41 } else { 49.0 }
        $breath = 0.62 + 0.38 * [Math]::Sin(2.0 * [Math]::PI * 0.026 * $t)
        $value += ((Tri $droneFreq $t) * 0.016 + (Tone ($droneFreq * 0.75) $t) * 0.005 + (Noise $i) * 0.002) * $breath

        $fade = EnvAD $t $duration 0.16 0.18
        $s[$i] = $value * $fade
    }

    return $s
}

function New-BgmStage5 {
    $duration = 24.0
    $s = New-Clip $duration
    $bpm = 112.0
    $beat = 60.0 / $bpm
    $stepLen = $beat / 2.0
    $bass = @(73.42, 0.0, 61.74, 0.0, 55.0, 0.0, 49.0, 0.0, 65.41, 0.0, 55.0, 0.0, 49.0, 0.0, 43.65, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $freq = $bass[$step]
        $value = 0.0

        if ($freq -gt 0.0) {
            $env = [Math]::Exp(-2.25 * $local)
            $value += (Tri $freq $t) * 0.10 * $env
            $value += (Tone ($freq * 0.5) $t) * 0.075 * $env
        }

        $kickPhase = $t % ($beat * 2.0)
        if ($kickPhase -lt 0.07) {
            $kEnv = [Math]::Exp(-34.0 * $kickPhase)
            $value += (Tone (62.0 - 16.0 * ($kickPhase / 0.07)) $t) * 0.13 * $kEnv
        }

        $sparkPhase = ($t + 0.09) % ($beat * 3.0)
        if ($sparkPhase -lt 0.045) {
            $value += ((Tone 123.47 $t) * 0.010 + (Noise ($i * 7)) * 0.009) * [Math]::Exp(-36.0 * $sparkPhase)
        }

        $stormFreq = if (($bar % 8) -lt 4) { 61.74 } else { 49.0 }
        $storm = 0.56 + 0.44 * [Math]::Sin(2.0 * [Math]::PI * 0.041 * $t)
        $value += ((Tri $stormFreq $t) * 0.014 + (Tone ($stormFreq * 0.75) $t) * 0.004 + (Noise $i) * 0.003) * $storm

        $fade = EnvAD $t $duration 0.08 0.10
        $s[$i] = $value * $fade
    }

    return $s
}

function New-BgmBossPulswyrm {
    $duration = 24.0
    $s = New-Clip $duration
    $bpm = 112.0
    $beat = 60.0 / $bpm
    $stepLen = $beat / 2.0
    $bass = @(73.42, 0.0, 65.41, 0.0, 55.0, 0.0, 65.41, 0.0, 73.42, 0.0, 55.0, 0.0, 65.41, 0.0, 49.0, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $value = 0.0
        $freq = $bass[$step]

        if ($freq -gt 0.0) {
            $env = [Math]::Exp(-2.4 * $local)
            $value += (Tri $freq $t) * 0.115 * $env
            $value += (Tone ($freq * 0.5) $t) * 0.085 * $env
        }

        $kickPhase = $t % ($beat * 2.0)
        if ($kickPhase -lt 0.09) {
            $kEnv = [Math]::Exp(-30.0 * $kickPhase)
            $value += (Tone (62.0 - 14.0 * ($kickPhase / 0.09)) $t) * 0.14 * $kEnv
        }

        $clankPhase = ($t + 0.12) % ($beat * 4.0)
        if ($clankPhase -lt 0.055 -and (($bar % 4) -ne 3)) {
            $value += ((Tone 123.47 $t) * 0.020 + (Noise $i) * 0.009) * [Math]::Exp(-42.0 * $clankPhase)
        }

        $droneFreq = if (($bar % 8) -lt 4) { 73.42 } else { 65.41 }
        $value += ((Tri $droneFreq $t) * 0.014 + (Tone ($droneFreq * 0.75) $t) * 0.005) * (0.62 + 0.38 * [Math]::Sin(2.0 * [Math]::PI * 0.055 * $t))

        $fade = EnvAD $t $duration 0.08 0.12
        $s[$i] = $value * $fade
    }

    return $s
}

function New-BgmBossNullwyrm {
    $duration = 32.0
    $s = New-Clip $duration
    $bpm = 88.0
    $beat = 60.0 / $bpm
    $stepLen = $beat / 2.0
    $bass = @(55.0, 0.0, 41.2, 0.0, 49.0, 0.0, 41.2, 0.0, 51.91, 0.0, 38.89, 0.0, 49.0, 0.0, 41.2, 0.0)

    for ($i = 0; $i -lt $s.Length; $i++) {
        $t = $i / $SampleRate
        $step = [int][Math]::Floor($t / $stepLen) % $bass.Count
        $local = ($t % $stepLen) / $stepLen
        $bar = [int][Math]::Floor($t / ($stepLen * 8.0))
        $value = 0.0
        $freq = $bass[$step]

        if ($freq -gt 0.0) {
            $env = [Math]::Exp(-1.55 * $local)
            $value += (Tri $freq $t) * 0.125 * $env
            $value += (Tone ($freq * 0.5) $t) * 0.095 * $env
        }

        $kickPhase = $t % ($beat * 2.0)
        if ($kickPhase -lt 0.13) {
            $kEnv = [Math]::Exp(-22.0 * $kickPhase)
            $value += (Tone (44.0 - 9.0 * ($kickPhase / 0.13)) $t) * 0.15 * $kEnv
        }

        $riftPhase = ($t + 0.08) % ($beat * 6.0)
        if ($riftPhase -lt 0.28) {
            $value += ((Noise $i) * 0.012 + (Tone 82.41 $t) * 0.014) * [Math]::Exp(-5.8 * $riftPhase)
        }

        $padFreq = if (($bar % 8) -lt 2) { 55.0 } elseif (($bar % 8) -lt 4) { 49.0 } elseif (($bar % 8) -lt 6) { 46.25 } else { 41.2 }
        $pad = 0.58 + 0.42 * [Math]::Sin(2.0 * [Math]::PI * 0.039 * $t)
        $value += ((Tri $padFreq $t) * 0.017 + (Tone ($padFreq * 0.75) $t) * 0.004 + (Noise $i) * 0.003) * $pad

        $phasePulse = $t % ($beat * 8.0)
        if ($phasePulse -lt 0.18) {
            $value += (Tone 98.0 $t) * 0.018 * [Math]::Exp(-10.0 * $phasePulse)
        }

        $fade = EnvAD $t $duration 0.10 0.16
        $s[$i] = $value * $fade
    }

    return $s
}

Save-Clip "BGM" { New-Bgm } "BGM" "Current global battle/menu loop loaded by CoreLanternGame"
Save-Clip "BGM_Stage2" { New-BgmStage2 } "BGM" "Stage 2 Lava Cache loop loaded when Lava Cache is selected"
Save-Clip "BGM_Stage3" { New-BgmStage3 } "BGM" "Stage 3 Broken Core Network placeholder loop"
Save-Clip "BGM_Stage4" { New-BgmStage4 } "BGM" "Stage 4 Frost Vault placeholder loop"
Save-Clip "BGM_Stage5" { New-BgmStage5 } "BGM" "Stage 5 Storm Spire placeholder loop"
Save-Clip "BGM_Boss_Pulswyrm" { New-BgmBossPulswyrm } "BGM" "Mid-boss Pulswyrm loop loaded during boss encounter"
Save-Clip "BGM_Boss_Nullwyrm" { New-BgmBossNullwyrm } "BGM" "Final boss Nullwyrm loop loaded during boss encounter"
Save-Clip "BGM_BossPulswyrm" { New-BgmBossPulswyrm } "BGM" "Legacy mid-boss Pulswyrm fallback alias loaded by older CoreLanternGame paths"
Save-Clip "BGM_BossNullwyrm" { New-BgmBossNullwyrm } "BGM" "Legacy final boss Nullwyrm fallback alias loaded by older CoreLanternGame paths"
Save-Clip "Shoot" { New-Shoot } "SE" "Player projectile"
Save-Clip "Hit" { New-Hit } "SE" "Enemy hit"
Save-Clip "Kill" { New-Kill } "SE" "Enemy destroyed"
Save-Clip "Pickup" { New-Pickup } "SE" "Data/health pickup and UI confirm"
Save-Clip "LevelUp" { New-LevelUp } "SE" "Level up and normal upgrade reward"
Save-Clip "Evolve" { New-Evolve } "SE" "Evolution cut-in"
Save-Clip "Fusion" { New-Fusion } "SE" "Cross evolution fusion cut-in"
Save-Clip "Boss" { New-Boss } "SE" "Boss warning and boss emphasis"
Save-Clip "GameOver" { New-GameOver } "SE" "Defeat result"

$licenseText = @"
CoreLanternUnity Starter Audio License
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss zzz")

Files:
- Assets/Resources/Audio/BGM.wav
- Assets/Resources/Audio/BGM_Stage2.wav
- Assets/Resources/Audio/BGM_Stage3.wav
- Assets/Resources/Audio/BGM_Stage4.wav
- Assets/Resources/Audio/BGM_Stage5.wav
- Assets/Resources/Audio/BGM_Boss_Pulswyrm.wav
- Assets/Resources/Audio/BGM_Boss_Nullwyrm.wav
- Assets/Resources/Audio/BGM_BossPulswyrm.wav
- Assets/Resources/Audio/BGM_BossNullwyrm.wav
- Assets/Resources/Audio/Shoot.wav
- Assets/Resources/Audio/Hit.wav
- Assets/Resources/Audio/Kill.wav
- Assets/Resources/Audio/Pickup.wav
- Assets/Resources/Audio/LevelUp.wav
- Assets/Resources/Audio/Evolve.wav
- Assets/Resources/Audio/Fusion.wav
- Assets/Resources/Audio/Boss.wav
- Assets/Resources/Audio/GameOver.wav

Source:
Generated locally by Tools/GenerateStarterAudio.ps1 using deterministic synthesis.
No third-party samples, melodies, stems, recordings, or AI service outputs are included.

Project use:
Created for the CoreLanternUnity project as project-original placeholder audio.
Commercial use in this project is intended to be permitted. No attribution required.

Notes:
This is a starter audio kit, not final production sound design.
2026-06-01 comfort pass: reduced obvious upward BGM phrases and softened rapid-fire hit/shoot/kill SE for long-session play.
If replaced by downloaded or AI-generated audio, keep each source license beside the file.
"@

Set-Content -LiteralPath (Join-Path $LicenseDir "StarterAudio_License.txt") -Value $licenseText -Encoding UTF8

$manifestPath = Join-Path $AudioDir "AudioManifest.json"
($Manifest | ConvertTo-Json -Depth 5) | Set-Content -LiteralPath $manifestPath -Encoding UTF8

Write-Host ""
Write-Host "Starter audio complete."
Write-Host "Audio: $AudioDir"
Write-Host "License: $(Join-Path $LicenseDir 'StarterAudio_License.txt')"
Write-Host "Manifest: $manifestPath"
