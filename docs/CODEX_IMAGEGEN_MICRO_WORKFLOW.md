# Codex Image Generation Micro Workflow

目的: レート制限と品質ブレを抑えながら、画像生成タスクを小さく安全に進める。

## 基本方針

- 1回の生成で大量差し替えしない。
- 既存の `Assets/Resources/Skins/*.png` はすぐ上書きしない。
- まず `Assets/ArtSource/HUD_V2/` などのプレビュー置き場に保存する。
- 1枚ごとに寸法、四隅 alpha、中央の静けさ、テキスト可読性を確認する。
- 合格した素材だけ本番名へ昇格し、Unity 側の参照と import 設定を調整する。

## 優先順

1. HUD/UI v2 パイロット
2. メインメニュー装飾
3. リザルト画面装飾
4. ボス警告/カットイン
5. キャラ/進化画像

キャラ/進化画像は作業量が大きく、品質基準が固まるまで最後に回す。

## HUD/UI v2 パイロット

現状:

- `UseGeneratedHudPanels = false`
- `UseGeneratedHudBars = false`
- 既存 HUD PNG は本番 UI では無効化中。
- 現行 PNG はネイティブ実寸で作られており、9-slice 前提の四隅透過素材ではない。

最初に生成する候補:

1. `HUD_WaveProgress_Frame_v2.png`
2. `HUD_WaveProgress_Fill_Normal_v2.png`
3. `HUD_WaveProgress_Fill_Boss_v2.png`

理由:

- 表示位置が画面上部で確認しやすい。
- 画面全体を壊しにくい。
- バー系のトーンが決まると HP/EXP へ横展開しやすい。

## HUD v2 受け入れ基準

- 中央 70% はほぼ単色または微弱グラデ。
- 装飾は端と角だけ。細かい線や模様を中央に入れない。
- 文字やバーの上に画像の強い発光が乗らない。
- 比率変更で伸ばしても違和感が出ない構造にする。
- 四隅は完全透明 alpha 0。
- 角装飾は小さく、外枠は細め。
- 色は暗い青緑ベース、アクセントはシアン/金/マゼンタ/緑の機能色だけ。

## 生成プロンプトの原則

避ける:

- 密集した回路模様
- 太い発光ライン
- 中央に大きいシンボル
- 派手な縦縞、横縞
- 写真風、金属板の過剰ディテール
- UI文字を画像内に含める

入れる:

- clean sci-fi HUD frame
- dark transparent center
- thin cyan neon edge
- small corner brackets
- minimal detail
- game UI sprite sheet asset
- transparent background

## 1回ごとの作業単位

### Step A: 生成前

- 対象ファイルを1つだけ選ぶ。
- 既存の用途、表示サイズ、必要な9-slice borderを確認する。
- 生成先はプレビュー名にする。

### Step B: 生成

- 1枚だけ生成する。
- 生成結果を表示して、うるささと比率を目視確認する。

### Step C: 検査

- 寸法が仕様通りか。
- 四隅 alpha が 0 か。
- 中央が静かか。
- 本番名へ昇格してよいか。

### Step D: 配置

- 合格後に `Assets/Resources/Skins/` へコピーする。
- Unity import の `textureType=Sprite`, `alphaIsTransparency=1`, `spriteBorder` を設定する。
- その後にだけ `UseGeneratedHudBars` / `UseGeneratedHudPanels` の有効化を検討する。

## 次の推奨タスク

`HUD_WaveProgress_Frame_v2.png` を1枚だけ生成し、保存せずプレビュー確認する。

合格したら `HUD_WaveProgress_Fill_Normal_v2.png` と `HUD_WaveProgress_Fill_Boss_v2.png` に進む。
