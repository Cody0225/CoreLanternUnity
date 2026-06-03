# Audio Comfort Pass 2026-06-01

## 目的

ユーザー指摘:

- 多段ヒット時に SE が重なってうるさい。
- BGM か SE か不明だが、音階がどんどん上がる感じが不快。
- Stage ごとの音の差はほしいが、長時間プレイで耳に刺さらない仮音にしたい。

今回の対応は **仮音の快適化**。本番BGM/SEの最終差し替えではない。

## 変更ファイル

- `Tools/GenerateStarterAudio.ps1`
- `Assets/Resources/Audio/*.wav`
- `Assets/Resources/Audio/AudioManifest.json`
- `Assets/Resources/Audio/Licenses/StarterAudio_License.txt`

`Assets/Scripts/CoreLanternGame.cs` は編集していない。

## 実施内容

### SE

- `Shoot.wav`: 高域とノイズ量を下げ、連射時の耳への刺さりを軽減。
- `Hit.wav`: ノイズと打撃音量を下げ、多段ヒットで団子になっても暴れにくく調整。
- `Kill.wav`: 高域倍音と破裂ノイズを抑え、連続撃破時の圧を軽減。
- `Pickup.wav`: ピッチを下げ、拾得音を少し落ち着かせた。
- `LevelUp.wav`: 高音から始まる印象を弱め、短めの下降寄りフレーズに変更。
- `Evolve.wav` / `Fusion.wav` / `Boss.wav`: 上昇感とノイズを抑え、演出音として残しつつ疲れにくくした。

### BGM

- `BGM.wav` / `BGM_Stage2.wav` / `BGM_Stage3.wav` / `BGM_Stage4.wav` / `BGM_Stage5.wav` を再生成。
- 露骨な上昇フレーズ、明るい高域パッド、鋭い装飾音を減らした。
- Stage差は残しつつ、低域中心・横に流れるループへ寄せた。
- `BGM_Boss_Pulswyrm.wav` / `BGM_Boss_Nullwyrm.wav` も同じ方針で再生成。

### 旧名フォールバック

以下を追加し、旧コードパスが参照しても警告が出ないようにした。

- `BGM_BossPulswyrm.wav`
- `BGM_BossNullwyrm.wav`

どちらも内容は現行の `BGM_Boss_Pulswyrm.wav` / `BGM_Boss_Nullwyrm.wav` と同系統の生成音。

## 検証

- `Tools/TestAudioResources.ps1`
  - Manifest entries: 18
  - Warnings: 0
  - Errors: 0
- `Tools/TestImageResources.ps1`
  - Warnings: 0
  - Errors: 0
- `Tools/CheckCompileHealth.ps1 -Quick`
  - odd-quote=0
  - brace diff=0
  - swallowed=0
  - compile errors=0 (Quick/Roslyn skip)

## 次に目視・試聴してほしいところ

- Stage 1/2 の通常戦闘を 2-3 分ずつプレイして、まだ「上がっていく音」が気になるか。
- Wave 10 ボスで弾数が多いビルドを使い、`Shoot` / `Hit` / `Kill` が重なっても不快でないか。
- LevelUp / Evolve / Fusion の演出音が地味になりすぎていないか。

不快感がまだ残る場合は、次は **音量ではなく再生頻度側** を Claude が `CoreLanternGame.cs` で制御するのが本筋。
例: hit SE のクールダウン、同一フレーム内の SE 集約、距離や重要度による発音優先度。
