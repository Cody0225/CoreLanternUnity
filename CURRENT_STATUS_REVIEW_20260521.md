# CoreLanternUnity Current Status Review

確認日: 2026-05-21  
確認者: Codex  
対象: Claude引き継ぎ、現行ドキュメント、現行コード、生成済み画像レビューシート

## 確認したもの

- `HANDOFF_FOR_CODEX.md`
- `REMAINING_TASKS.md`
- `IMAGE_ASSET_BACKLOG.md`
- `ASSET_REVIEW_REPORT.md`
- `Assets/Scripts/CoreLanternGame.cs`
- `Assets/ArtSource/AssetReview_PartnerRoutes.png`
- `Assets/ArtSource/AssetReview_FusionStages.png`
- `Assets/ArtSource/AssetReview_EnemiesMapUi.png`
- `Assets/ArtSource/Generated_HudAsset_Preview.png`
- `Assets/ArtSource/Generated_MapAsset_Preview.png`
- `Assets/ArtSource/Generated_AllSpecies_Evolution_Preview.png`

## 現在できていること

- 現行 `CoreLanternGame.cs` はRoslynコンパイル成功。
- Claude側の2026-05-21変更はコードに反映済み。
  - SplashDamage再帰のStackOverflow対策。
  - GUARDノックバック強化。
  - SPEED/POWER倍率ナーフ。
  - 新キャラ `Wraith Lynx` と `Genesis Core` 追加。
  - パートナー選択画面を8体表示に拡張。
  - ボスHP 2倍化。
  - HUD/Loadout/Partner名など文字サイズ改善。
- オプション保存は実装済み。
  - `CoreLantern_Option_*` とHUD/カメラ倍率を `PlayerPrefs` に保存。
- 画像在庫は大きく増えている。
  - active PNG: 329
  - Partner系: 222
  - HUD専用: 13
  - Minimap: 7
- `Assets/Resources/Audio/` には本素材なし。
  - 現状はコード内プロシージャルBGM/SEが仮置き。

## コンパイル確認

結果: 成功。エラーなし。

残警告:

- `FindObjectOfType<T>()` obsolete warning が2件。
- `bossModuleReady` が未割当で常にfalse。
- `allyName` が代入のみで未使用。

注意:

- `bossModuleReady` はコード内に分岐が残っているが、コメントでは「Final boss reward module removed by user request」となっている。ボス後スペシャルモジュールを復活させるか、完全に削るか仕様確定が必要。

## 目視で気になったこと

### 1. キャラ進化の魅力はまだ足りない

`AssetReview_PartnerRoutes.png` と `Generated_AllSpecies_Evolution_Preview.png` を見た限り、名前と画像の不一致は以前より改善している。特にDrift Foxは鳥っぽさがかなり減った。

ただし、Lv3/Lv6/Lv9の変化がまだ「装飾が増える」寄りで、デジモン的な成長段階の驚きには届いていない。特にDrift/Hex/Ironは、遠目では段階差が弱い。

残タスク:

- 8体前提でキャラ大幅リデザイン仕様を更新する。
- Lv0 -> Lv3 -> Lv6 -> Lv9で、サイズ、姿勢、顔つき、装備、シルエットを変える。
- 色差ではなく形差でSPEED/POWER/GUARDを見分けられるようにする。

### 2. 新キャラ2体の追従

Claudeが `Wraith Lynx` style 7 と `Genesis Core` style 8 を追加済み。  
Codex側で図鑑・進化ツリー・画像台帳を8体対応に更新済み。

`Assets/Resources/Skins` に `Partner_S7*.png` / `Partner_S8*.png` も暫定生成済み。

確認箇所:

- `CreateCodexPanel` のPARTNERは8体対応。
- `CreateEvolutionTreePanel` 周辺の素体リストも8体対応。
- `IMAGE_ASSET_BACKLOG.md` の種族番号も1-8対応。

残タスク:

- 生成済み画像は暫定。大幅リデザイン時に作り直す。
- `Generated_NewPartnerRelicUi_Preview.png` を見て採用/差し戻し判断する。

### 3. クロス進化はまだ「吸収変身」よりエンブレム変化に見える

`AssetReview_FusionStages.png` では、L0-L3の段階が丸い紋章/リングの変化に寄っている。  
ユーザー方針は「リンク同士が合体」ではなく「メインキャラがリンクを吸収して見た目が変わる」なので、現状はまだ弱い。

残タスク:

- クロス進化は全体設計から作り直す。
- L0-L2は吸収途中、L3は完全変身として明確化する。
- 各種族の顔/体格を残しつつ、Nova/Bulwark/Siphon/Phaseの装備を乗せる。
- Fusion汎用アイコンではなく、種族別フォームとして読めるようにする。

### 4. HUD/Minimap/Relic/UI素材は生成済みだがコード未接続

`Generated_HudAsset_Preview.png` ではHUD/Minimap素材が揃っている。  
追加で `Generated_NewPartnerRelicUi_Preview.png` に、Relic、BossBar、DangerVignette、Menu/Result/Cutin背景なども生成済み。

ただし `IMAGE_ASSET_BACKLOG.md` 上も `HOOK` のままで、現行コードの `CreatePanel` / `CreateHudBar` / `CreateMinimap` / Relic表示にはまだ本格接続されていない。

残タスク:

- `CreatePanel` に任意スキン適用を入れる。
- `CreateHudBar` に背景/fill画像を適用する。
- HP減少が分かる遅延赤バー `HUD_Bar_HP_Lag.png` を実装する。
- `CreateMinimap` とdot生成に `Minimap_*` を接続する。
- レリック選択/表示に `Relic_*.png` と `HUD_RelicSlot_Frame.png` を接続する。
- メニュー/リザルト/進化カットイン背景に生成済み背景を接続する。
- 接続後、HUDが重なっていないか全画面/16:9で確認する。

### 5. マップ素材は整理されたが、実機密度は要確認

`Generated_MapAsset_Preview.png` 単体ではサイバー感がある。  
ただし床タイルA/B/Cは線密度が高めなので、実機で大量配置すると画面が汚くなる可能性がある。

残タスク:

- 実機でマップ密度を確認する。
- 床タイルの透明度/出現率/サイズを調整する。
- コア周辺とプレイヤー周辺は視認性優先で床ノイズを抑える。
- 敵弾、EXP、データチップの視認性を優先する。

### 6. 敵とボスは次の見た目強化が必要

敵の外部PNGは揃っているが、Elite系やBoss第2形態はまだ未作成。  
ゲームとしてWave後半の盛り上がりを作るなら、敵の個性とボスの段階演出が次の伸びしろ。

残タスク:

- Elite Runner / Elite Brute / Elite Shooter の追加。
- Boss第2形態の検討。
- 危険範囲や敵弾の見やすさ強化。
- 敵ごとの死亡演出差別化。

### 7. 音はまだ仮置き

`Assets/Resources/Audio/` にはREADMEだけで本素材なし。  
現在はプロシージャル音源なので、商用版としては差し替え前提。

残タスク:

- BGM本素材候補の選定。
- Shoot/Hit/Kill/Pickup/LevelUp/Evolve/Fusion/Boss/GameOverのSE整理。
- 音量設定の保存は既にあるので、実音源追加後にバランス調整。
- 連射SEの過密対策。

## 優先順

### P0: すぐやる

1. 新キャラ2体を図鑑/進化ツリー/画像台帳に反映する。
2. `bossModuleReady` の仕様を決める。
3. Wave1-10の実機通し確認。
4. HUD/Minimap素材のコード接続。

### P1: 次にやる

1. 8体前提のキャラ大幅リデザイン仕様書を作る。
2. クロス進化を「吸収変身」として設計し直す。
3. Elite敵とBoss第2形態を追加する。
4. マップ密度を実機で調整する。

### P2: 仕上げ

1. 本BGM/SE差し替え。
2. アニメーションフレーム対応。
3. メタ進行、解放演出、実績、難易度追加。
4. Build/Fullscreen/Input設定の整理。

## Claude/Codex同時作業の分担案

Claude向き:

- `CoreLanternGame.cs` のゲームロジック/UI接続。
- 図鑑/進化ツリー/ボス後報酬/HUD接続。
- Wave1-10実機確認とバグ修正。

Codex向き:

- キャラ大幅リデザイン仕様。
- 画像台帳更新。
- レビューシート更新。
- HUD/背景/カットイン/敵素材の生成。
- 音素材候補リスト作成。

## 今回の未確認

- Unity Editor上でのPlay実機操作は未実施。
- 実際のカード選択、進化、融合、リザルト遷移は未確認。
- 現行の全画面/16:9でのHUD重なりは未確認。
