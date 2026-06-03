# キャラクターピクセルアート制作フロー

最終更新: 2026-06-02

> ⚠ **2026-06-02 整合更新 — 必ず最初に読む**
>
> **1. 現在は「静止画運用」**: このゲームにはスプライトアニメ機構が無い（cs は静止画
> `Partner_S*_L{stage}.png` のみ使用）。本書のアニメ系QA（RUN/ATTACK/Idle/Hit/GIF/Animator/接地順）は
> **将来アニメ機構を入れたとき用に休眠**。現在のキャラ作業は **静止画1枚** で進める。
> アニメGIFは今は作らない（接続先が無く無駄になるため）。
>
> **2. 最優先は HANDOFF「素材品質ゲート」**（`HANDOFF_FOR_CODEX.md`）。本書より優先。
> 透過チェックは「四隅alpha=0」だけでは**不十分**（壊れた素材でも通るため）。必ず追加で:
> - 透過は色抜きchromakeyを使わず**透明キャンバスに直接描く**（色飛びの根本原因）
> - **本体ベタ部 alpha ≥ 230 / 内部に透け穴なし**
> - **陰影は3階調以上＝べた塗りでない**（単色塗り潰し禁止）
> - 本体中央の**色サンプル**で意図の色か検証
>
> 静止画に関わる既存QA（描き直し禁止・「修正済み」と偽らない・64/96/128pxシルエット・秘書レビュー）は
> 引き続き有効。

## 決定方針

キャラ画像は以下の順で進める。

1. AIでラフ案
2. ピクセル調に統一
3. 透過・縮小テスト
4. GIFでモーション確認
5. ゲーム内確認

目標は「現代寄りの高解像度ピクセルアート」。レトロ8bitではなく、輪郭・色数・発光・シルエットを整理した商用向けの読みやすいピクセルアートにする。

全キャラ共通のQAは `docs/CHARACTER_ANIMATION_GLOBAL_QA.md` を必ず優先する。  
この文書はCobalt Pup専用ではなく、今後の相棒、進化後、クロス進化すべてに適用する。

ユーザーへ提示する前に、`docs/CHARACTER_SECRETARY_REVIEW_PROTOCOL.md` の秘書レビューを通す。  
少しでも気になる点がある場合は、確認物として出す前に止める。

## 必須QA

- 良いラフ案を勝手に描き直さない。
- GIF確認用でも、見た目が崩れた素材は出さない。
- 攻撃は口・武器・発射器など、見た目上自然な位置から出す。
- 走りは足の接地順を確認する。同じ足だけが動いて見える場合は不採用。
- 透過PNGは四隅alpha 0を確認する。
- 64px、96px、128px相当でシルエットが読めるか確認する。
- UIやラベルがキャラに重ならないプレビューにする。
- 指摘された問題が変わっていないGIFを「修正済み」として提示しない。
- crop、re-export、label変更だけで動きが変わっていない場合は、必ず「未修正」と明記する。
- GIFをユーザーへ出す前に、RUN単体とATTACK単体を目視確認する。

## 修正報告ゲート

指摘対応として報告する前に、以下をすべて満たす。

| 項目 | 必須条件 |
|---|---|
| 見た目 | 承認済みラフと同等以上 |
| RUN | 前足/後足の接地が読み取れる |
| ATTACK | 弾/光の発生源が意図通り |
| 差分 | 前回NG素材から問題箇所が実際に変わっている |
| 表示 | GIF内で文字・UI・キャラが重ならない |
| 透過 | runtime候補PNGの四隅alphaが0 |

どれか1つでも満たさない場合は、`RUNTIME_CANDIDATE` にしない。

## Cobalt Pup 試作の扱い

> ⚠ **2026-06-02 更新: この節は旧アニメ方針の記録（現行では非現役）**。
> 実物の Cobalt Pup（`Assets/Resources/Skins/Partner_S1_L0.png`）は 2026-06-02 に
> **新しい静止画ピクセルアート**へ差し替え・ユーザー承認済み。
> 下記の `CobaltPixelAnim_20260601` アニメ試作（PixelAnimSheet / review_gifs）は、
> 現在の静止画運用では**使わない**（将来アニメ機構を入れる時の参考まで）。
> 進化形は静止画で `docs/CODEX_S1_EVOLUTION_SPEC_20260602.md` に沿って進行中。

採用候補:

- `Assets/ArtSource/CobaltPixelAnim_20260601/CobaltPup_PixelAnimSheet_transparent.png`
- `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_MotionReview_v1.gif`
- `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_Run_review_v1.gif`
- `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_Attack_review_v1.gif`
- `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/CobaltPup_AIrough_NormalizedReviewSheet_v1.png`

注意: 上記review_gifsは「元ラフの切り出し確認」であり、ユーザー指摘の足/攻撃モーション修正は未完了。現時点では `MOTION_REVIEW` であり、`RUNTIME_CANDIDATE` ではない。

非採用:

- `Assets/ArtSource/AnimationPrototypes/deprecated_bad_redraw_20260601/`
- `Tools/Deprecated/GenerateCobaltActualPixelMotionPrototype_BAD_20260601.ps1`

非採用理由: 元のAIラフの魅力を活かさずに再描画したため、実キャラの品質基準を満たさない。後足の動きも確認用として不十分。

## Claude への接続方針

Codexは当面、キャラ素材の生成・整理・GIF検収・ドキュメント化までを担当する。

Claudeが担当するのは、ユーザーが採用を確認した後のUnity接続、Animator設計、Pivot/PPU/Filter Mode設定、`CoreLanternGame.cs` 側の表示切り替え。

未承認の試作GIFや非採用フォルダをランタイムに接続しないこと。
