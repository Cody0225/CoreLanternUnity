# CoreLanternUnity Audio Generation Prompts

Last updated: 2026-05-22

用途: Stable Audio 3.0 / Eleven Music / 他のAI音声生成ツール、またはCC0素材検索のためのプロンプト集。  
商用前提なので、既存IP名、アーティスト名、曲名、歌詞、固有キャラクター名は入れない。

## 共通プロンプト方針

Core style:

```text
original video game audio, dark cyber arena, neon data core, cute monster companion energy, clean mix, no vocals, no recognizable melody, no artist reference, seamless loop, game-ready, not cinematic trailer, not orchestral fantasy
```

Negative style:

```text
no vocals, no lyrics, no copyrighted character reference, no artist name, no existing song reference, no radio pop structure, no harsh high-frequency noise, no long reverb tail, no melody that dominates gameplay
```

SE common:

```text
short game sound effect, clean transient, synthetic digital texture, no background music, no voice, no reverb tail, no clipping, designed for repeated playback
```

## BGM Prompts

### BGM_Title_Menu

```text
Seamless loop for a dark neon cyber creature-raising roguelite main menu. Calm but exciting, soft FM synth pulses, warm sub bass, tiny data chimes, mysterious digital core ambience, 82 BPM, no vocals, no lyrics, clean game mix, central UI-safe mood, 75 seconds.
```

### BGM_Stage1_Arena

```text
Seamless loop for a top-down cyber arena survival game. Driving but not exhausting, pulsing synth bass, crisp digital percussion, light arpeggios, neon grid floor energy, 118 BPM, no vocals, no lyrics, clean loop for repeated combat, 100 seconds.
```

### BGM_Stage2_Lava

```text
Seamless loop for a cyber lava cache stage. Dark synthwave battle groove with heat distortion feeling, low analog bass, muted industrial percussion, glowing orange hazard energy, tense but readable, 112 BPM, no vocals, no lyrics, 100 seconds.
```

### BGM_Stage3_BrokenCore

```text
Seamless loop for a corrupted data network stage in a roguelite. Broken signal textures, glitchy FM bells, unstable sub pulses, restrained percussion, eerie but playable, 105 BPM, no vocals, no lyrics, no harsh noise, 120 seconds.
```

### BGM_Boss_Pulswyrm

```text
Seamless loop for a midboss battle against an unstable digital serpent. Faster cyber percussion, pulsing bass, alarm-like synth motif, energetic but not overwhelming, 132 BPM, no vocals, no lyrics, clean video game boss loop, 80 seconds.
```

### BGM_Boss_Nullwyrm

```text
Seamless loop for a final boss battle in a dark neon data-core arena. Heavy sub bass, aggressive digital drums, ominous synth brass, glitch pulses, phase two intensity, 140 BPM, no vocals, no lyrics, no orchestral fantasy, 120 seconds.
```

### BGM_Victory

```text
Short victory sting for a cyber monster evolution roguelite. Bright data chime, warm synth chord, satisfying clear result, clean ending, no vocals, no lyrics, 8 seconds.
```

### BGM_Defeat

```text
Short defeat sting for a digital core defense game. Low fading synth, corrupted data shutdown, soft impact, not horror, no vocals, no lyrics, 8 seconds.
```

## SE Prompts

### UI

| ID | Prompt |
|---|---|
| `SE_UI_Select` | `short clean cyber UI select click, tiny data chirp, soft transient, 0.12 seconds, no music, no voice` |
| `SE_UI_Back` | `short low cyber UI back click, descending tiny blip, 0.14 seconds, no music, no voice` |
| `SE_UI_Error` | `short soft error beep for cyber game UI, two quick muted pulses, not annoying, 0.25 seconds, no voice` |

### Combat

| ID | Prompt |
|---|---|
| `SE_Shoot_Speed` | `very short rapid plasma pellet shot, clean high synth zap, designed for repeated fire, 0.08 seconds, no tail` |
| `SE_Shoot_Power` | `short heavy digital cannon shot, warm low punch and bright edge, 0.18 seconds, no long reverb` |
| `SE_Shoot_Guard` | `short defensive energy ring pulse, rounded synth thump, shield-like, 0.20 seconds, no voice` |
| `SE_Hit_Small` | `tiny enemy hit tick, digital particle impact, very short, 0.07 seconds, no bass boom` |
| `SE_Kill` | `short digital enemy dissolve pop, data shards dispersing, satisfying but quiet, 0.18 seconds` |
| `SE_CoreDamage` | `urgent digital core damage impact, glassy shield crack plus low thump, clear warning, 0.35 seconds` |

### Pickup / Reward

| ID | Prompt |
|---|---|
| `SE_Pickup_Data` | `tiny data chip pickup, bright soft coin-like digital blip, 0.08 seconds, repeat friendly` |
| `SE_Pickup_Heal` | `short healing pickup sound, soft rising digital sparkle, warm and gentle, 0.30 seconds` |
| `SE_LevelUp` | `level up reward sound, rising digital arpeggio, bright but not loud, 0.75 seconds, no voice` |
| `SE_RelicGet` | `rare relic acquired sound, deep synth pulse then bright data ring, premium reward, 1.0 seconds` |

### Evolution / Boss

| ID | Prompt |
|---|---|
| `SE_Evolve` | `monster evolution transformation sound, charging digital energy, rising synth sweep, bright completion impact, cute but powerful, 1.4 seconds, no voice` |
| `SE_Fusion` | `cross evolution fusion sound, two energy cores merge, deep pulse, bright neon burst, stronger than normal evolution, 1.8 seconds, no voice` |
| `SE_BossWarning` | `boss warning alert, cyber siren pulse, short and serious, not painful, 0.9 seconds, no voice` |
| `SE_GameOver` | `digital core shutdown defeat sound, low synth fall, broken data tail, 1.5 seconds, no horror scream` |
| `SE_Clear` | `clear victory result sound, triumphant cyber synth chord and data sparkle, 1.6 seconds, no voice` |

## CC0素材検索キーワード

Kenney / OpenGameArt / Freesound CC0 で探す時の検索語:

```text
laser shot short cc0
sci fi blip cc0
digital pickup cc0
power up synth cc0
shield hit cc0
energy charge cc0
ui select click cc0
cyber loop cc0
synthwave game loop cc0
boss battle loop cc0
```

日本語素材サイトで探す時:

```text
サイバー BGM ループ
電子音 効果音 決定
レベルアップ 効果音
パワーアップ 効果音
ゲームオーバー ジングル
ボス 警告 効果音
```

## 生成後チェック

- BGMは最初と最後を繋げて、クリック音や急な音量差がないか確認する。
- SEは20回連続再生して耳が痛くないか確認する。
- Shoot/Hit/Pickupは音量を控えめにし、LevelUp/Evolve/Fusion/BossWarningだけ前に出す。
- AI生成物は、生成サービス名、アカウントプラン、生成日、プロンプト、利用ライセンスURLを記録する。

