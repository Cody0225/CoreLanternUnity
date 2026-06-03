# CoreLanternUnity Audio Automation

Last updated: 2026-05-25

目的: 音素材準備の手作業を減らす。  
現時点では、ログイン不要・APIキー不要・第三者素材なしで使えるスターター音源を自動生成する。

## できること

`Tools/GenerateStarterAudio.ps1` は、Unityが現在読み込む名前でWAVを生成する。

```text
Assets/Resources/Audio/BGM.wav
Assets/Resources/Audio/BGM_Stage2.wav
Assets/Resources/Audio/BGM_Boss_Pulswyrm.wav
Assets/Resources/Audio/BGM_Boss_Nullwyrm.wav
Assets/Resources/Audio/Shoot.wav
Assets/Resources/Audio/Hit.wav
Assets/Resources/Audio/Kill.wav
Assets/Resources/Audio/Pickup.wav
Assets/Resources/Audio/LevelUp.wav
Assets/Resources/Audio/Evolve.wav
Assets/Resources/Audio/Fusion.wav
Assets/Resources/Audio/Boss.wav
Assets/Resources/Audio/GameOver.wav
```

同時に以下も作る。

```text
Assets/Resources/Audio/AudioManifest.json
Assets/Resources/Audio/Licenses/StarterAudio_License.txt
Assets/Resources/Audio/_incoming/
```

## 実行方法

PowerShell:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Tools\GenerateStarterAudio.ps1
```

既存ファイルを上書きしたくない場合:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Tools\GenerateStarterAudio.ps1 -NoOverwrite
```

## 検査方法

音源を追加・差し替えた後は、コード側の `LoadAudioClip()`、WAV、`.meta`、`AudioManifest.json` の整合を確認する。

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Tools\TestAudioResources.ps1
```

`BGM_BossPulswyrm` / `BGM_BossNullwyrm` は旧ファイル名の任意フォールバックなので、存在しなくても警告扱い。

## ライセンス方針

- このスターター音源はスクリプトの決定的シンセ生成で、第三者サンプルを含まない。
- AIサービス出力でもフリー素材でもないので、外部ライセンス証跡は不要。
- 最終製品で使うなら、プロジェクト内オリジナル素材として扱う。
- 後でStable Audio / Kenney / OpenGameArt等の音へ差し替える場合は、各ファイルごとにライセンス証跡を保存する。

## 限界

- これは「すぐ鳴る」「商用面で扱いやすい」スターター音源。
- 最終クオリティのBGM/SEではない。
- Stable AudioやEleven Musicの生成を完全自動化するには、アカウント/APIキー/利用規約確認が必要。

## BGM調整メモ

- 初期版は高いリード音が上昇気味で、長時間プレイ時に耳へ残りやすかった。
- 現行の通常BGMは24秒ループ、104 BPM、低めのベース/パッド中心。
- Stage2用に `BGM_Stage2.wav`、中ボス用に `BGM_Boss_Pulswyrm.wav`、最終ボス用に `BGM_Boss_Nullwyrm.wav` を生成する。
- ボスBGMも上昇リードを避け、低音パルスと暗いドローン中心にしている。
- リードは毎回上がる旋律ではなく、少ない下降気味パルスに抑えている。
- まだ仮BGMなので、最終版ではステージ3以降やタイトル専用BGMへさらに分ける想定。

## 次の自動化候補

1. Stable Audioで生成したファイルを `_incoming` へ置いたあと、正式名へリネームしてライセンスログを作るスクリプト。
2. `CoreLanternGame.cs` を拡張し、Stage3以降とタイトル専用BGM (`BGM_Title_Menu`, `BGM_Stage3_BrokenCore`, `BGM_Stage4_Frost`, `BGM_Stage5_Storm`) を切り替える。
