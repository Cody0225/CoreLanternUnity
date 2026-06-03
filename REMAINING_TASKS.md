# CoreLanternUnity Remaining Tasks

## 直近対応 (2026-06-02 Codex Drift Fox ゴミピクセル/桃色修正)

- `HANDOFF_FOR_CODEX.md` を読了し、`docs/CODEX_PIXELART_CLEANUP_20260602.md` のS5指示を確認。
- `Assets/Scripts/CoreLanternGame.cs` は編集していない。`Partner_S4_L0.png` も触っていない。
- 対象は `Assets/Resources/Skins/Partner_S5_L0.png` のみ。
- 左側の短い縦線アーティファクト、しっぽ上/周辺の本体から分離した白/ピンク四角片、低alphaの残りを除去。
- 本体のシルエットを保ったまま、透過で弱くなっていた桃色の視認性を補正。
- 既存 `.meta` は編集せず、PNGのみ同名上書き。
- 追加ツール:
  - `Tools/RepairDriftFoxCleanupAndPinkReadability.ps1`
- バックアップ:
  - `Assets/ArtSource/CharacterPixelArt_20260601/drift_fox_cleanup_20260602/backup_before_drift_fox_cleanup_20260602_151155/`
- 検収画像/ログ:
  - `Assets/ArtSource/CharacterPixelArt_20260601/drift_fox_cleanup_20260602/DriftFox_visibility_check_after.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/drift_fox_cleanup_20260602/drift_fox_cleanup_report.json`
- 検証:
  - `Partner_S5_L0.png`: 512x512 / bbox `[124,120,386,448]` / `bottom_y=448` / 四隅alpha 0
  - `Partner_S5_L0.png`: alpha>0連結成分 1個のみ
  - `Partner_S5_L0.png.meta`: 末尾byte `0A`、`EndsWithLF=True`
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`

## 直近対応 (2026-06-02 Codex Hex Cat 透過抜け修正)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- ユーザー指摘「紫のキャラが透過で色が消えてる」に対応。
- `Partner_S4_L0.png` の半透明寄りだった紫ピクセルを不透明化し、暗背景でも消えないよう紫の明度/彩度を補正。
- 右側に残っていた小さな分離ノイズも削除し、足元基準は `bottom_y=448` に再調整。
- 既存 `.meta` は編集せず、PNGのみ更新。
- 追加ツール:
  - `Tools/RepairHexCatOpacityAndReadability.ps1`
- 検収画像:
  - `Assets/ArtSource/CharacterPixelArt_20260601/hex_cat_opacity_repair_20260602/HexCat_visibility_check_after.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
- バックアップ:
  - `Assets/ArtSource/CharacterPixelArt_20260601/hex_cat_opacity_repair_20260602/backup_before_hex_opacity_repair_20260602_145358/`
- 検証:
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
  - `Partner_S4_L0.png.meta`: 末尾byte `0A`、`EndsWithLF=True`

## 直近対応 (2026-06-02 Codex 相棒6体 L0 ピクセルアート刷新)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `docs/CODEX_CHARACTER_PIXELART_SPEC_20260601.md` に沿って、素体L0の6体を高解像度ピクセルアート風に同名差し替え:
  - `Partner_S1_L0.png` Cobalt Pup / 青狼
  - `Partner_S2_L0.png` Ember Drake / 橙竜
  - `Partner_S3_L0.png` Sage Hare / 緑兎
  - `Partner_S4_L0.png` Hex Cat / 紫猫
  - `Partner_S5_L0.png` Drift Fox / 桃狐
  - `Partner_S6_L0.png` Iron Bear / 黒熊
- 6体すべて 512x512、足元基準 `bottom_y=448`、四隅alpha 0。既存 `.meta` は編集せず、PNGのみ上書き。
- S4/S5は秘書レビューで「紫猫/桃狐の読み」が弱いと判断し、シルエットを残したまま色識別だけ補正。
- 出力/検収:
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_preview.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_shrink_check.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/CharacterPixelArt_6partners_lineup_clean_transparent.png`
  - `Assets/ArtSource/CharacterPixelArt_20260601/validation_report.json`
  - `Assets/ArtSource/CharacterPixelArt_20260601/color_identity_polish_20260602/color_identity_polish_report.json`
- 旧PNGバックアップ:
  - `Assets/ArtSource/CharacterPixelArt_20260601/backup_before_overwrite_20260602_142911/`
  - `Assets/ArtSource/CharacterPixelArt_20260601/color_identity_polish_20260602/backup_before_polish_20260602_143616/`
- 追加ツール:
  - `Tools/BuildPartnerL0PixelArtFromSheet.ps1`
  - `Tools/PolishPartnerL0ColorIdentity.ps1`
- Claude向けタイトル配置仕様:
  - `docs/CLAUDE_TITLE_6PARTNER_LINEUP_HOOK_SPEC.md`
- 検証:
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
  - `Partner_S1_L0.png.meta` 〜 `Partner_S6_L0.png.meta`: 末尾byte `0A`、`EndsWithLF=True`
- 次: Claude がタイトル画面に6体横並びを配置。`preserveAspect = true`、文字/ボタン重なりなし、16:9確認必須。

## 直近対応 (2026-06-01 Codex 音まわり快適化)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- ユーザー指摘「多段ヒットSEがうるさい」「BGM/SEの音階が上がっていく感じが不快」に対し、仮音の快適化を実施。
- `Tools/GenerateStarterAudio.ps1` を調整し、BGM/SEを再生成:
  - 連射/多段ヒット系: `Shoot.wav`, `Hit.wav`, `Kill.wav` の高域・ノイズ・音量を抑制。
  - 取得/レベルアップ/演出系: `Pickup.wav`, `LevelUp.wav`, `Evolve.wav`, `Fusion.wav`, `Boss.wav` の上昇感と耳への刺さりを軽減。
  - BGM: Stage1-5 + Boss2種を低域中心・下降/安定寄りのループへ調整。
- 旧ボスBGM fallback名も追加:
  - `Assets/Resources/Audio/BGM_BossPulswyrm.wav`
  - `Assets/Resources/Audio/BGM_BossNullwyrm.wav`
- 変更メモ:
  - `docs/AUDIO_COMFORT_PASS_20260601.md`
- 検証:
  - `Tools/TestAudioResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- 次の確認: Unity Playで Stage1/2 と Wave10 ボスを試聴し、まだ不快な上昇音が残る場合は Claude 側で SE 再生頻度制御を入れる。

## 直近対応 (2026-06-01 Codex 非キャラ素材キュー整理)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- キャラ生成以外の衝突しない残作業を整理するため、日本語の作業キューを追加:
  - `docs/NON_CHARACTER_ASSET_QUEUE_20260601.md`
- 未接続の非キャラ素材を一枚で見える化するプレビュー生成ツールを追加:
  - `Tools/GenerateNonCharacterQueuePreview.ps1`
  - 出力: `Assets/ArtSource/NonCharacterQueue_20260601/NonCharacterAssetQueue_20260601.png`
- `Tools/ReportAssetStatus.ps1` を更新し、Aエッグ派生素材の `Present / Hooked / Waiting` も表示するようにした。
- 現在の非キャラ接続待ち:
  - Aエッグ presentation: `Present 3 / Hooked 0 / Waiting 3`
  - HUD9: `Total 11 / Hooked 3 / Waiting 8`
  - Stage4/5 support: `Present 6 / Hooked 0 / Waiting 6`
  - V2 UI: `Total 68 / Hooked 54 / Waiting 14`
- 検証:
  - `Tools/ReportAssetStatus.ps1`: static sprite refs missing 0
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`

## 直近対応 (2026-06-01 Codex Aエッグ派生素材 / Claude接続待ち)

- Claude が復帰したため、制限を通常運用に戻した。Codex は `Assets/Scripts/CoreLanternGame.cs` を編集していない。
- Aエッグをタイトル/リザルト/小アイコンでも使えるよう、別リソース名で3点を追加:
  - `Assets/Resources/Skins/Title_CoreEgg_ASelected_v1.png` (768x768)
  - `Assets/Resources/Skins/Result_CoreEgg_ASelected_v1.png` (512x512)
  - `Assets/Resources/Skins/Icon_CoreEgg_ASelected_v1.png` (256x256)
- ArtSource/検収プレビュー:
  - `Assets/ArtSource/EggCoreVisual_20260601/title_result_support_A_20260601/TitleResultCoreEgg_ASelected_v1_preview.png`
  - `Assets/ArtSource/EggCoreVisual_20260601/title_result_support_A_20260601/validation_report.txt`
- 検証: 3点すべて四隅alpha 0、`.meta` あり。
- 秘書レビュー: NGなし。64/96/128pxでも金色エッグ+青コアは読める。注意点として、タイトル用は透明余白と薄い円が大きいのでタイトル文字や実績テキストの背面に重ねない。
- Claude接続仕様書を追加:
  - `docs/CLAUDE_A_EGG_PRESENTATION_HOOK_SPEC.md`
- 検証結果:
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/TestAudioResources.ps1`: `Errors: 0`。警告2件は任意の旧ボスBGM fallback名のみ。
  - `Tools/ReportAssetStatus.ps1`: static sprite refs missing 0。HUD9 waiting 8、Stage4/5 support waiting 6 は継続タスク。
  - `Tools/CheckCompileHealth.ps1 -Quick`: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
- 状態: `Claude接続待ち`。タイトル/リザルトに配置する場合は `preserveAspect = true`、テキストやボタンと重ねないこと。

## 直近完了 (2026-06-01 Codex Aエッグ反映 / デバッグ)

- ユーザー判断により、エッグ/コア候補は A `SacredDataEgg` を現行採用。B/Cは削除せず `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/` に保存。
- `Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `Assets/Resources/Skins/Lantern.png` を A ベースの 384x384 透過PNGに更新。
- Aエッグに合わせて、中心周りの既存Resourcesも同名・同サイズで静かめに更新:
  - `Floor_CoreMark_A.png`
  - `Core_Platform.png`
  - `Core_RingOuter.png`
  - `Core_RingInner.png`
- 元の `Lantern.png` は `Assets/ArtSource/EggCoreVisual_20260601/selected_A_runtime_candidate_20260601/Lantern_PRE_ASelected_20260601.png` にバックアップ済み。
- 元の中心リング/台座4点は `Assets/ArtSource/EggCoreVisual_20260601/core_map_support_A_20260601/*_PRE_ASelected_20260601.png` にバックアップ済み。
- 生成/保存:
  - `EggCore_ASelected_v1_clean_source.png`
  - `Lantern_ASelected_v1_RUNTIME_CANDIDATE.png`
  - `Title_CoreEgg_ASelected_v1_RUNTIME_CANDIDATE.png`
  - `Lantern_ASelected_v1_comparison_shrink_check.png`
  - `CoreMapSupport_ASelected_v1_preview.png`
  - `validation_report.txt`
- 秘書レビュー結果:
  - 中心コアとして成立。前版より重要オブジェクト感が強い。
  - 64pxでは土台/卵殻細部は潰れるが、金色シルエットと青い中心光は読める。
  - 比率崩れ、引き伸ばし、白点ノイズ、既存IPらしさの重大懸念なし。
  - 中心リング4点もAエッグとトーン一致。ゲーム内で外周リングが少し目立つ可能性だけPlay目視で確認。
- デバッグ結果:
  - `Tools/CheckCompileHealth.ps1` full: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`
  - `Tools/TestImageResources.ps1`: `Warnings: 0` / `Errors: 0`
  - `Tools/TestAudioResources.ps1`: `Errors: 0`。警告2件は任意の旧ボスBGM fallback名のみ。
  - `Tools/ReportAssetStatus.ps1`: static sprite refs missing 0。HUD9 waiting 8、Stage4/5 support waiting 6 は継続タスク。

## 直近対応 (2026-06-01 Codex Egg/Core VISUAL_REVIEW v1生成)

- `HANDOFF_FOR_CODEX.md` と imagegen skill を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- ユーザー要望「エッグはタイトルにもあるくらいだからデザインにこだわりたい」に対し、差し替え前の方向確認として `docs/EGG_CORE_VISUAL_BRIEF_20260601.md` を追加。
- 生成先: `Assets/ArtSource/EggCoreVisual_20260601/visual_review_20260601/`
- 主な出力:
  - `EggCore_VISUAL_REVIEW_v1_candidates_chromakey.png`
  - `EggCore_VISUAL_REVIEW_v1_candidates_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_A_SacredDataEgg_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_B_ArmoredCoreEgg_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_C_HatchProtocolEgg_transparent.png`
  - `EggCore_VISUAL_REVIEW_v1_shrink_check.png`
  - `EggCore_VISUAL_REVIEW_v1_title_scale_preview.png`
- 透過: 候補ボードとA/B/C単体の四隅alpha 0 確認済み。
- 秘書レビュー結果:
  - 3案とも96/128/192pxで「金の卵 + シアンのコア」として読める。
  - 第一候補は A `SacredDataEgg`。ゲームの顔/タイトル/象徴性のバランスが最も良い。
  - B `ArmoredCoreEgg` は装甲過多で、名前含め既存IP連想リスクがあるためメイン顔としては止め寄り。
  - C `HatchProtocolEgg` は孵化感が魅力だが、96pxで内部モチーフが潰れやすい。
  - 次は A ベースでリング/粒子を減らした低ノイズ実装候補を作るのが安全。
- 状態: `VISUAL_REVIEW`。ユーザー承認前のためUnity接続禁止。Claudeへ渡す場合も `CoreLanternGame.cs` 接続不可として共有する。

## 直近対応 (2026-06-01 Codex Ember Drake MOTION_REVIEW v1生成)

- `HANDOFF_FOR_CODEX.md` と imagegen skill を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- Cobaltで通した制作フローを別体型で試すため、Ember Drake の `MOTION_BRIEF` を `docs/CHARACTER_BRIEF_EMBER_MOTION_20260601.md` に追加。
- 生成先: `Assets/ArtSource/EmberPixelAnim_20260601/motion_review_20260601/`
- 生成ツール: `Tools/BuildCharacterMotionReviewFromGeneratedSheet.ps1`
- 主な出力:
  - `EmberDrake_MOTION_REVIEW_v1_RUN.gif`
  - `EmberDrake_MOTION_REVIEW_v1_ATTACK.gif`
  - `EmberDrake_MOTION_REVIEW_v1_sheet_transparent.png`
  - `EmberDrake_MOTION_REVIEW_v1_shrink_check.png`
  - `EmberDrake_MOTION_REVIEW_v1_before_after.png`
- 透過: 四隅alpha 0 確認済み。
- 秘書レビュー結果:
  - 橙色の小型竜、黒い角、背中棘、緑の目は維持。
  - ATTACKの火弾は口先から出ており、胸・爪・画面中央発生には見えない。
  - RUNは低い姿勢の跳ね走りとして読める。
  - 致命NGなし。
  - 残る懸念: 64pxでは細部が潰れる。静止版より横長で印象が少し変わる。
- 状態: `MOTION_REVIEW`。ユーザー承認前のためUnity接続禁止。

## 直近対応 (2026-06-01 Codex Cobalt MOTION_REVIEW v1生成)

- `Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `docs/CHARACTER_BRIEF_COBALT_MOTION_20260601.md` に沿って、Cobalt Pup の `MOTION_REVIEW` v1 を生成。
- 生成先: `Assets/ArtSource/CobaltPixelAnim_20260601/motion_review_20260601/`
- 主な出力:
  - `CobaltPup_MOTION_REVIEW_v1_RUN.gif`
  - `CobaltPup_MOTION_REVIEW_v1_ATTACK.gif`
  - `CobaltPup_MOTION_REVIEW_v1_sheet_transparent.png`
  - `CobaltPup_MOTION_REVIEW_v1_shrink_check.png`
  - `CobaltPup_MOTION_REVIEW_v1_before_after.png`
- 生成ツール: `Tools/BuildCobaltMotionReviewFromGeneratedSheet.ps1`
- 透過: 四隅alpha 0 確認済み。
- 初回切り出しでATTACKに隣フレーム混入があったため、ツールを修正して再生成済み。
- 秘書レビュー結果:
  - RUN: 前後脚の位置差が読め、同じ足だけが動いている印象は前回より薄い。
  - ATTACK: 火球・ビームとも口元に寄って見え、胸コア/前脚発射には見えにくい。
  - 致命NGなし。
  - 残る懸念: 64pxではATTACK chargeの発光球が顔下に広がり、胸コア近くの光にも少し見える可能性あり。
- 状態: `MOTION_REVIEW`。ユーザー承認前のためUnity接続禁止。

## 直近対応 (2026-06-01 Codex Cobaltモーションブリーフ作成)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- Cobalt Pup の次回生成前ブリーフを `docs/CHARACTER_BRIEF_COBALT_MOTION_20260601.md` に追加。
- 目的を「見た目の作り直し」ではなく、ユーザー指摘の **RUN後ろ足** と **ATTACK口元発射** の修正に限定。
- 秘書レビューを実施し、指摘に基づいて以下を追記:
  - RUN 6フレームの接地足/浮遊足設計
  - ATTACK 5フレームの溜め/発射/反動/戻り設計
  - Before比較対象GIF
  - 保存先候補 `Assets/ArtSource/CobaltPixelAnim_20260601/motion_review_20260601/`
  - `MOTION_REVIEW` ラベルと、ユーザー承認前のUnity接続禁止
- 次にユーザーへ出す確認物は、Cobalt Pupの見た目を維持した **RUN単体GIF** と **ATTACK単体GIF** だけに絞る。
- 同じ問題で内部NGが2回続いた場合は、生成を止めて短く相談する。

## 直近対応 (2026-06-01 Codex 秘書レビュー運用追加)

- ユーザー意向: やり取り回数と無駄を減らす。少しでも気になる点は事前に確認し、チーム間で指摘が来そうな場所を先に潰して秘書に報告させる。
- `docs/CHARACTER_SECRETARY_REVIEW_PROTOCOL.md` を追加。
- `docs/CHARACTER_ANIMATION_GLOBAL_QA.md` に秘書レビューゲートを追加。
- 秘書レビューで必須確認するもの: RUN/MOVE単体GIF、ATTACK単体GIF、64/96/128px縮小確認、Before/After、透過、接続可否ラベル。
- 同じ問題で内部NGが2回続いたら生成を止め、ユーザーへ短く相談するルールを追加。
- Claude共有時は `RUNTIME_CANDIDATE` の接続可ファイル名と、`REJECTED`/未承認素材の接続禁止ファイル名を明記する。

## 直近対応 (2026-06-01 Codex キャラアニメ全体QAへ拡張)

- ユーザー指摘: 再発防止策がCobalt Pup局所に寄りすぎており、他キャラで同じ失敗が起きる可能性がある。
- `docs/CHARACTER_ANIMATION_GLOBAL_QA.md` を追加。
- 今後の全キャラ・進化後・クロス進化に対して、キャラ別モーションブリーフ、RUN/ATTACK単体GIF、Before/After証明、自己検収ゲートを必須化。
- `docs/CHARACTER_PIXEL_ART_PIPELINE.md` と `docs/POSTMORTEM_20260601_CHARACTER_ANIMATION_QA_FAILURE.md` から全体QA文書を参照するよう更新。
- 結論: Cobalt専用ではなく、キャラ画像/モーション作業全体のルールとして扱う。

## 直近対応 (2026-06-01 Codex キャラアニメーションQA失敗報告)

- ユーザー指摘: Cobalt Pupの足/攻撃モーションが変わっていないにもかかわらず、修正扱いで報告してしまった。
- `docs/POSTMORTEM_20260601_CHARACTER_ANIMATION_QA_FAILURE.md` を作成。
- `docs/CHARACTER_PIXEL_ART_PIPELINE.md` に修正報告ゲートを追加。
- 現状の `review_gifs` は「元ラフ切り出し確認」であり、足/攻撃修正済みではないと明記。
- 次にキャラアニメへ戻る場合は、新規または編集済みフレームでRUN/ATTACKの問題箇所そのものを変える。crop/re-export/label変更だけは禁止。

## 直近対応 (2026-06-01 Codex Cobalt Pupピクセルアニメ方針修正)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- ユーザー指摘により、手描き再描画版 `CobaltPup_Actual_*` は品質基準を満たさないため非採用化。
- 非採用素材を `Assets/ArtSource/AnimationPrototypes/deprecated_bad_redraw_20260601/` へ移動。
- 非採用ツールを `Tools/Deprecated/GenerateCobaltActualPixelMotionPrototype_BAD_20260601.ps1` へ移動。
- 採用候補のAIラフシート `Assets/ArtSource/CobaltPixelAnim_20260601/CobaltPup_PixelAnimSheet_transparent.png` から、再描画なしでレビュー用GIFを生成。
- 追加ツール: `Tools/GenerateCobaltPixelSheetReviewGifs.ps1`
- 生成先: `Assets/ArtSource/CobaltPixelAnim_20260601/review_gifs/`
- 生成物: `CobaltPup_AIrough_MotionReview_v1.gif`, `CobaltPup_AIrough_Run_review_v1.gif`, `CobaltPup_AIrough_Attack_review_v1.gif`, `CobaltPup_AIrough_NormalizedReviewSheet_v1.png`
- `docs/CHARACTER_PIXEL_ART_PIPELINE.md` を追加し、Claude共有用に「AIラフ案 → ピクセル調統一 → 透過・縮小テスト → GIF確認 → ゲーム内確認」を明文化。

## 直近完了 (2026-06-01 Codex Cobalt Pupピクセルアニメ試作)

- Cobalt Pupのピクセルアート版・アニメ試作シートを生成。
- 保存先: `Assets/ArtSource/CobaltPixelAnim_20260601/`
- 生成物: クロマキー元画像、透過PNG、256x256切り出しフレーム、Idle/Run/Attack/HitのプレビューGIF。
- これは試作確認用で、`Assets/Resources/Skins/` には未反映。採用なら次にUnity用のPivot/Point Filter/Animator設計へ進む。

## 直近完了 (2026-06-01 Codex 素材接続状況レポート追加)

- `HANDOFF_FOR_CODEX.md` を読了し、今回は `Assets/Scripts/CoreLanternGame.cs` を編集していない。
- `Tools/ReportAssetStatus.ps1` を追加。HUD9 / Stage4-5支援素材 / V2 UI / 任意fallbackの「接続済み・接続待ち」を読み取り専用で棚卸しできる。
- `IMAGE_ASSET_BACKLOG.md` のHUD9ステータスを更新し、Wave進行バー3枚だけ `HOOKED`、その他HUD9は候補/待機のままと明記。
- `Tools/README.md`, `ASSET_REVIEW_REPORT.md`, `docs/CODEX_RELEASE_ORDER.md` を現状に追従。
- 検証: `ReportAssetStatus` 実行OK、`TestImageResources` Errors 0、`TestAudioResources` Errors 0、`CheckCompileHealth -Quick` OK。

## 直近完了 (2026-06-01 Codex Player_Form4警告解消)

- `Assets/Scripts/CoreLanternGame.cs` は編集していない。
- 既存の退避ファイル `Player_Form4.png.disabled` と `Player_Form4.png.meta.disabled` から、通常ファイル `Player_Form4.png` / `.meta` を復元。
- 退避ファイルは削除せず残置。
- 検証: `Tools/TestImageResources.ps1` が `Warnings: 0` / `Errors: 0` に改善。

## 直近対応 (2026-06-01 Codex Stage4/5支援素材 接続仕様)

- `Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `docs/CLAUDE_STAGE45_SUPPORT_HOOK_SPEC.md` を追加。
- 目的: 生成済みだが未接続の `StageThumb_Frost`, `StageThumb_Storm`, `Stage4_FrostCrystal_A`, `Stage4_FrostPatch_A`, `Stage5_LightningMarker_A`, `Stage5_LightningStrike_A` をClaudeが迷わず接続できるようにする。
- 現状: `ReportAssetStatus` では Stage4/5 support `Present: 6 / Hooked: 0 / Waiting: 6`。

## 直近完了 (2026-06-01 Codex HUD9 Wave進行バー限定接続)

- `HANDOFF_FOR_CODEX.md` を読了し、Claudeレート制限中の今回だけ例外として `EDIT_LOCK.md` でロック取得後に `Assets/Scripts/CoreLanternGame.cs` を編集。
- `HUD9_WaveProgress_Frame.png` / `HUD9_WaveProgress_Fill_Normal.png` / `HUD9_WaveProgress_Fill_Boss.png` をWave進行バーだけに限定接続。
- 新フラグ `UseHud9SliceCandidates` と `ApplySlicedSprite()` を追加し、HUD9素材が無い場合は既存V2/従来表示へfallbackする構成にした。
- パネル/HP/EXPなど他HUD9素材は未接続。実機目視でWaveバーが問題なければ段階的に広げる。
- 検証: `Tools/CheckCompileHealth.ps1` full OK、`Tools/TestImageResources.ps1` Errors 0。任意fallback `Player_Form4.png` 警告のみ。

## 直近完了 (2026-05-31 Codex Result_StatTile_v2再生成)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- 引き継ぎに明記されていた `Result_StatTile_v2` のサイズ不一致を対応。
- `Tools/GenerateResultV2Assets.ps1` を更新し、`Result_StatTile_v2.png` を `112x76` から `420x56` の横長候補に再生成。
- プレビュー: `Assets/ArtSource/Result_V2_20260530/Result_V2_Preview.png`。
- 検証: `Result_StatTile_v2.png` は `420x56`、四隅 `alpha=0`、`.meta` あり。
- 関連ドキュメント更新: `IMAGE_ASSET_BACKLOG.md`, `docs/ASSET_HOOKUP_MAP.md`, `docs/UI_V2_HOOKUP_PLAN.md`。

## 直近完了 (2026-05-31 Codex HUD 9-slice候補生成)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `Tools/GenerateHud9SliceAssets.ps1` を追加し、旧HUD画像の「うるさい/引き伸ばし」問題を避けるため、別名 `HUD9_*` で9-slice候補を11枚生成。
- 生成先: `Assets/Resources/Skins/HUD9_Panel_*`, `HUD9_Bar_*`, `HUD9_WaveProgress_*`。
- ArtSourceコピー/プレビュー: `Assets/ArtSource/HUD_9Slice_20260531/HUD9_Preview.png`。
- Claude向け接続仕様: `docs/CLAUDE_HUD9SLICE_HOOK_SPEC.md`。
- 検証: 全11ファイルの寸法一致、四隅 `alpha=0`、`.meta` 生成済み。現行HUD挙動は未変更。
- 状態: P1-4 `HUD 大型素材 9-slice` は候補生成完了。採用/接続はClaude側タスク。

## 直近完了 (2026-05-31 Codex itch.io ページ画像ドラフト)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `Tools/GenerateItchIoPageAssets.ps1` を追加し、キャラ最終絵に依存しにくいロゴ/コア中心の販促画像を生成。
- 生成先: `marketing/ItchIo/ItchIo_Header_630x500.png`, `ItchIo_Cover_315x250.png`, `ItchIo_SocialCard_1200x630.png`。
- ArtSourceコピー/プレビュー: `Assets/ArtSource/ItchIo_20260531/ItchIo_PageAssets_Preview.png`。
- 検証: 3ファイルとも寸法一致、四隅 `alpha=255`。プレビューで文字のはみ出しがないよう英語コピーを2行化。
- 状態: P2-4 `itch.io ページ画像` はドラフト完了。キャラ画像確定後に最終ビジュアルへ差し替える。

## 直近対応 (2026-05-31 Codex .exe アイコンUnity設定仕様)

- Claude/Unity向けに `docs/CLAUDE_EXE_ICON_PLAYER_SETTINGS_SPEC.md` を追加。
- Codex生成済みの `build/Icons/EggcoreProtocol.ico` と `Assets/ArtSource/Icon_Drafts_20260531/icon_*.png` の使い分けを整理。
- 状態: Unity Player Settingsへの設定はClaude/Unity側タスク。

## 直近完了 (2026-05-31 Claude タイトル画面スマート化)

- `docs/CLAUDE_TITLE_SCREEN_SIMPLIFY_SPEC.md` / `docs/CLAUDE_TITLE_LOGO_HOOK_SPEC.md` に沿って `CreateMainMenuPanel` を簡素化。
- **ロゴ接続**: `GameLogo_TitleCompact` を優先表示 (無ければ `GameLogo`、それも無ければ live text `EGGCORE PROTOCOL` へ fallback)。位置 (0,210) / size (620,194) / preserveAspect。
- **タイトルから退避**: 進行サマリー (BEST/CLEARS/Stage/Danger) は生成のみで `SetActive(false)`、ヒーローデッキ (`CreateMainMenuHeroVisual` 呼ばない)、ガイド文削除、StatsRibbon/SubtitlePlate/LogoUnderline/NavRail V2 はタイトルでは不使用。
- **クレジット簡素化**: `Embercore Studio` 表記を外し `Eggcore Protocol v0.9.0 (EA)` のみ (名称未確定のため)。
- **レイアウト調整**: サブコピー (0,82) / START (0,-48) / 補助ボタン控えめ (secY=-148, 136x38) / version (0,-332)。
- Stage/Danger は従来通り `RUN SETUP` (RunConfigPanel) で確認可能 (変更なし)。
- 検証: `CheckCompileHealth.ps1` full `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)` (17047行)、`TestImageResources.ps1` `Errors: 0`。
- ⚠ **実機目視TODO (ユーザー)**: Game view でロゴ・サブコピー・START が重ならないか、ロゴ上端が切れないか、16:9/Free Aspect/1920x1080 で破綻しないか確認。コーナー装飾 (`Title_CornerAccent_v2`) は残置 (ロゴと水平方向に非干渉)。

## 直近完了 (2026-05-31 Codex .exe アイコン生成)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集していない。
- `Tools/GenerateExeIconAssets.ps1` を追加し、ゲームロゴのコアマーク方向からWindows用アイコン一式を生成。
- 生成先: `build/Icons/icon_16.png`, `icon_32.png`, `icon_48.png`, `icon_64.png`, `icon_128.png`, `icon_256.png`, `icon_512.png`, `EggcoreProtocol.ico`。
- ArtSourceコピー/プレビュー: `Assets/ArtSource/Icon_Drafts_20260531/IconSet_Preview.png`。
- 検証: PNG全サイズ一致、四隅 `alpha=0`。`.ico` はWindows標準向けに16-256pxを収録、512pxは高解像度ソースとして保持。
- 状態: P0-3 `.exe` アイコン素材は完了。Unity Player Settingsへの設定はClaude/Unity側接続タスク。

## 直近対応 (2026-05-31 Codex タイトル画面スマート化仕様)

- `HANDOFF_FOR_CODEX.md` を読了し、現行ルール通り `Assets/Scripts/CoreLanternGame.cs` は編集していない。
- タイトル画面用に上下余白を詰めた `GameLogo_TitleCompact.png` を生成。配置先: `Assets/Resources/Skins/`、ArtSourceコピー: `Assets/ArtSource/Logo_Drafts_20260530/`。
- Claude向けに `docs/CLAUDE_TITLE_SCREEN_SIMPLIFY_SPEC.md` を追加。
- 方針: タイトルから `BEST/CLEARS/MISSIONS/DANGER/STAGE/COMBO`、Stage/Danger概要、ヒーローデッキ、進化ルートバッジ、操作ガイド、未確定スタジオ名を削除。ロゴ/サブコピー/START/控えめな補助ボタン/バージョンのみ残す。
- 状態: **Claude 接続待ち**。

## 直近対応 (2026-05-31 Codex タイトルロゴ接続仕様)

- `HANDOFF_FOR_CODEX.md` を読了し、現行ルール通り `Assets/Scripts/CoreLanternGame.cs` は編集していない。
- 既存コード確認: 現在のタイトルは `CreateMainMenuPanel()` 内の live text `EGGCORE PROTOCOL` で、`GameLogo.png` はまだロード/表示されていない。
- Claude向け接続仕様を `docs/CLAUDE_TITLE_LOGO_HOOK_SPEC.md` に作成。
- 推奨: `GameLogo.png` を `CreateMenuSpriteImage("Main Menu Game Logo", ..., new Vector2(0, 270), new Vector2(560, 280), ...)` で表示し、既存Textタイトルはロゴ未読込時のfallbackにする。
- 状態: **Claude 接続待ち**。

## 直近完了 (2026-05-30 Codex ゲームロゴドラフト生成)

- `HANDOFF_FOR_CODEX.md` を読了し、`Assets/Scripts/CoreLanternGame.cs` は編集せずに素材生成のみ実施。
- `Tools/GenerateLogoAssets.ps1` を追加し、文字崩れを避けるため画像AIではなく決定的なテキスト描画でゲームロゴを生成。
- 生成先: `Assets/Resources/Skins/GameLogo.png`, `GameLogo_Mono.png`, `GameLogo_Mark.png`, `GameLogo_Mark_Mono.png`。
- プレビュー: `Assets/ArtSource/Logo_Drafts_20260530/GameLogo_Preview.png`。
- 検証: 4つのResource PNGは想定サイズ一致、四隅 `alpha=0`。
- `Embercore` はSteam上に同名ゲームが見つかったため、`StudioLogo_Embercore*` は未生成。スタジオ名はユーザー確認/リネーム検討待ち。

## 直近完了 (2026-05-30 Codex 運用ルール更新)

- Claude/Codex衝突対策として、Codex作業開始時の絶対ルールを追加。
- Codexは短い再開依頼・コンテキスト圧縮後・Claude並行作業後でも、最初に `HANDOFF_FOR_CODEX.md` を全文読む。
- `CLAUDE.md` / `HANDOFF_FOR_CODEX.md` / `Tools/README.md` に同ルールを明記。
- 今回はドキュメントのみ更新。`Assets/Scripts/CoreLanternGame.cs` は編集していない。

## 直近完了 (2026-05-30 Codex UI V2 追加接続)

- `Title_*_v2` をメインメニューへ追加接続。タイトル下線、サブタイトル板、進捗リボン、ナビレール、角アクセント、中央コア装飾を既存レイアウトに重ねた。
- `Result_*_v2` をリザルト画面へ追加接続。デッキ枠、ポートレート枠、ステータスタイル、RANKメダル、ルート/融合バッジ、MVP行、サマリ板、フッターガイドを接続。
- V2リザルト画像の二重枠を軽減。`Result_StatTile_v2` / `Result_MvpRow_v2` / バッジ帯を使う場合は、Unity側の追加アウトラインと細い上線を控えめにした。
- `Card_*_v2` を強化カード内部へ追加接続。カード全体フレームは従来のまま、ヘッダー/タイトル板/レベル板/レアリティ板/下レール/特殊角だけ実寸PNGを使用。
- ボタン接続は既存実装を保持。文字は画像に焼き込まず、Unity側 `Text` のまま。
- `Tools/CheckCompileHealth.ps1` に文字化けマーカー警告スキャンを追加。構文エラーではない表示文言事故を今後見落としにくくした。
- 検証: `Tools/CheckCompileHealth.ps1` full `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`。
- 追加検証: `Tools/TestImageResources.ps1` `Errors: 0`、`Tools/TestAudioResources.ps1` `Errors: 0`。
- 実機目視TODO: Unity Editorでタイトル/リザルトを開き、文字被り・装飾の前面化・ボタン押下阻害がないか確認。

## 直近完了 (2026-05-30 Claude Button/Result/Title V2 コード接続 / itch.io アップローダ)

- **Button V2 素材をコードに接続**: Codex が生成した `Button_*_v2.png` 16枚を `SetButtonSpriteV2()` ヘルパー経由で各ボタンに接続。
  - フラグ `UseButtonV2 = true` で一括 ON/OFF 可能。素材が無ければ自動で従来の塗りボタンに fallback。
  - 接続先: メインメニュー START / セカンダリ5個 (Mission/Tree/Combo/Codex/Options) / RunConfig (Start/Back/DangerChip) / Reroll / SkipReward / Result (Retry/Menu) / Pause (Resume/Options/Restart) / Close各種 (Codex/Mission/Tree/Options) / Stepper (±)。
  - hover=淡シアン / pressed=暖黄 / disabled=グレーで状態フィードバック維持。`RefreshDangerChips()` の色付けは sprite tint として共存。
- **`Tools/UploadToItch.ps1` を新規作成**: `butler` CLI ラッパー。`GenerateBuildPackage.ps1` の次工程。
  - 機能: butler 自動検出 → zip 自動検出/バージョン指定 → SHA256 照合 → アップロード前チェックリスト → `butler push` → アップロード後ガイド。
  - `-CheckSetup` / `-DryRun` モード対応。PowerShell 5.1 互換 (null条件演算子不使用)。DryRun でスクリプト動作確認済み。
- **Result V2 素材を接続済み**: `Result_DeckFrame_v2` / `Result_PortraitFrame_v2` / `Result_StatTile_v2` / `Result_RankMedal_*_v2` / `Result_BadgeStrip_*_v2` / `Result_MvpRow_v2` / `Result_SummaryPlate_v2` / `Result_FooterGuide_v2` を接続。素材が無ければ従来の塗り+アクセント枠に fallback。
  - `Result_RankMedal_*_v2` は大型枠ではなく、RANK stat tile 内の薄い背景メダルとして接続。文字可読性優先。
- ⚠ **Title V2 はこの後 Codex が別実装で全接続** (`UseTitleUiV2` + `CreateMenuSpriteImage`、CoreEmblem 含む6素材全部)。Claude の Title V2 接続編集 (`UseTitleV2`) は同時編集衝突で全失敗 → 重複なし。上の「Codex UI V2 追加接続」セクション参照。
- ⚠ **同時編集衝突発生** (CLAUDE.md セクション E 抵触): Claude と Codex が `CoreLanternGame.cs` を同タイミングで編集。今回は被害なし (Claude の Title 編集が失敗しただけ) だが、今後は片方ずつにする運用を厳守。
- 検証 (最終): `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)` (Roslyn フルチェック通過、17026行)。Claude の全作業 (Button V2 / Result DeckFrame / Stage3-5 BGM / Stageスケーリング / バグ3件 / 侵食mojibake) は重複・欠落なく残存を確認済み。

## 直近検証 (2026-05-30 Codex)

- `Tools/TestImageResources.ps1`: `Errors: 0` / `Warnings: 1`。警告は任意フォールバック `Player_Form4.png` のみ。
- `Tools/TestAudioResources.ps1`: `Errors: 0` / `Warnings: 2`。警告は任意の旧ボスBGM fallback 名のみ。
- `Tools/CheckCompileHealth.ps1` full: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)`。
- その後のCodex追加接続で `Assets/Scripts/CoreLanternGame.cs` を編集済み。最新検証は上の「Codex UI V2 追加接続」を参照。

## 直近完了 (2026-05-30 Codex Result UI V2 素材追加)

- `CoreLanternGame.cs` は触らず、リザルト画面のごちゃつき軽減用に実寸UI素材を13枚作成。
- `Tools/GenerateResultV2Assets.ps1` を追加。デッキ枠、ポートレート枠、ステータスタイル、バッジ帯、MVP行、サマリ板、ランクメダルを生成。
- 生成先: `Assets/Resources/Skins/Result_*_v2.png`、プレビュー: `Assets/ArtSource/Result_V2_20260530/Result_V2_Preview.png`。
- 検証: 全13ファイルの想定サイズ一致、四隅 `alpha=0`。台帳は `IMAGE_ASSET_BACKLOG.md` に追記済み。

## 直近完了 (2026-05-30 Codex Title UI V2 素材追加)

- `CoreLanternGame.cs` は触らず、タイトル画面の顔を強化する非テキスト装飾素材を6枚作成。
- `Tools/GenerateTitleUiV2Assets.ps1` を追加。中央コア、統計リボン、サブタイトル板、ロゴ下線、ナビレール、角アクセントを生成。
- 生成先: `Assets/Resources/Skins/Title_*_v2.png`、プレビュー: `Assets/ArtSource/Title_UI_V2_20260530/Title_UI_V2_Preview.png`。
- 角アクセントは当初の薄い四角パネルから線だけの装飾に修正済み。文字やUIを覆わない前提。
- 検証: 全6ファイルの想定サイズ一致、四隅 `alpha=0`。台帳は `IMAGE_ASSET_BACKLOG.md` に追記済み。

## 直近完了 (2026-05-30 Codex Button V2 素材追加)

- `CoreLanternGame.cs` は触らず、メニュー/リザルト/ポーズ/オプション/強化画面で使う実寸ボタン素材を16枚作成。
- `Tools/GenerateButtonV2Assets.ps1` を追加。文字は画像に焼き込まず、Unity側テキストを重ねる前提。
- 生成先: `Assets/Resources/Skins/Button_*_v2.png`、プレビュー: `Assets/ArtSource/Button_V2_20260530/Button_V2_Preview.png`。
- 検証: 全16ファイルの想定サイズ一致、四隅 `alpha=0`。台帳は `IMAGE_ASSET_BACKLOG.md` に追記済み。

## 直近完了 (2026-05-30 Claude コード・バランス・バグ修正)

- **Stage 別 BGM 差別化** (完全対応): Stage 3 (Broken Core Network) / Stage 4 (Frost Vault) / Stage 5 (Storm Spire) にそれぞれ独自のプロシージャル BGM を追加。`MakeStage3/4/5BgmClip()` を実装し、`PlayBgm()` が `currentStageId` に応じて自動選択するよう拡張。
  - Stage 3: グリッチ・ノイズバースト電子音、Stage 4: クリスタルベル静寂音、Stage 5: 電撃クラッキング激しいリズム
  - 外部 WAV ファイルがあれば優先、なければプロシージャル fallback

- **ステージ別難易度スケーリング** (新規): `GetStageHpMultiplier()` / `GetStageSpeedMultiplier()` を追加。Stage 0 は等倍、Stage 4 (Storm Spire) は HP × 1.30 / 速度 × 1.10 に。Runner/Brute 等の汎用敵がどのステージでも同一スペックだった問題を解消。

- **Data Lab ヒントテキスト修正** (バグ): Wave 4 で Data Lab が開いても「ラン最後のショップ」と表示していた問題を修正。`wave == 2` → `wave == 4` の typo。

- **"侵食" mojibake 修正** (バグ): `ShowMessage("侵飁E" + wave)` の mojibake を `"侵食"` に修正 (2箇所)。

- **ミッション報酬テキスト修正** (バグ): "data_120" ミッションの報酬表示「リロール費用 30→5」が実際のコード (30→25) と不一致。`30→25` に修正。

- **docs/PATCH_NOTES_TEMPLATE.md** を新規作成。Steam Community / itch.io / X 向け三媒体対応バイリンガルテンプレート。

- 検証: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 (OK)` (Roslyn フルチェック通過)

## 直近完了 (2026-05-29 Codex 非コードQA / Stage3-5仮BGM追加)

- Claude側のコード確認と衝突しないよう `CoreLanternGame.cs` は触らず、素材/音/台帳系だけを確認。
- `Tools/TestAudioResources.ps1` で不足していた `BGM_Stage3.wav` / `BGM_Stage4.wav` / `BGM_Stage5.wav` を検出し、`Tools/GenerateStarterAudio.ps1` を拡張して仮BGMを追加。
- 既存音源は `-NoOverwrite` で保持し、`AudioManifest.json` と `StarterAudio_License.txt` も更新。
- `Tools/GenerateUiPolishPack.ps1` を追加し、リザルトランクバッジ5枚、MVP枠、ボス警告枠2枚、Core Mark警告の透明PNGを作成。すべて四隅alpha=0。
- `Tools/GenerateHudPanelV2Assets.ps1` を追加し、現行HUD実寸に合わせた `HUD_Panel_*_v2` 9枚を作成。旧大型パネル画像の引き伸ばし問題を避けるため、まだコード接続はしていない。
- `Tools/GenerateCardPartV2Assets.ps1` を追加し、強化カードのヘッダー/タイトル/レベル/レアリティ/レール用の小型透明PNGを12枚作成。文字は焼き込まず、Unity側テキストを重ねる前提。
- 検証: image resource `errors=0`, audio resource `errors=0`, HUD validator `errors=0 / warnings=11`。詳細は `docs/CODEX_NON_CODE_QA_20260529.md`。

## ✅ コンパイル復旧 完了 (2026-05-28)

- Claude / Codex 連携で構文エラー (CS1xxx) を完全解消。Unity Editor で動作確認可能な状態。
- 事故詳細: `docs/POSTMORTEM_20260528_MOJIBAKE.md` (PowerShell エンコーディング指定漏れによる大規模 mojibake)
- 再発防止ルール: `CLAUDE.md` (プロジェクト直下、絶対遵守)
- ハンドオフ書類: `HANDOFF_FOR_CLAUDE_COMPILE_FIX_20260528.md` (履歴目的で残置)

### 残課題 (低優先・段階的に対応)
- 残存 mojibake コメント (実害なし、コード動作問題なし)
- partner description の文言修正 (例: `桁E��の狐` → `桃色の狐`)
- 表示崩れる UI 文字列があれば随時 Edit ツールで修正

## 直近完了 (2026-05-28 Codex デバッグ pass / swallowed code 復旧)

- 文字化けコメントに飲み込まれていた実コードを復旧: `stageChipButtons.Clear()`, `img.raycastTarget=false`, `sideRect.anchoredPosition`, `UpdateLockVisual`, `ShowEvolutionComboBanner`, `TrackAmbientNeon`, `dangerMaxCleared`, quick retry flag など。
- クリック阻害・同設定リトライ・Danger 解放値・進化コンボ演出・ボス召喚再試行に関わる低リスク不具合を修正。
- `Tools/ValidateCodexAssets.ps1` を PowerShell 5.1 でも動く ASCII 出力に修正し、HUD の現行実寸に合わせて検査可能化。
- 検証: Roslyn `compile errors=0 / warnings=0`, image/audio resource tests `errors=0`, HUD validator `errors=0 / warnings=11`。
- レート制限対策として `docs/CODEX_IMAGEGEN_MICRO_WORKFLOW.md` を追加。次は `HUD_WaveProgress_Frame_v2.png` を1枚だけプレビュー生成する。
- `Assets/ArtSource/HUD_V2/` に Wave progress v2 パイロットを追加: frame は imagegen + chroma key、normal/boss fill はローカル生成。全 preview/composite は 512×48 / 四隅 alpha 0。
- `Assets/Resources/Skins/HUD_WaveProgress_*_v2.png` を追加し、Wave進行バーだけに限定してコード接続。フレームは同率拡大、fillは512×12化して比率崩れを避ける。検証: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 / image/audio resource errors=0`。
- `HUD_Bar_*_v2.png` をローカル生成し、HP/コアHP/EXPバーだけに限定してコード接続。背景132×20、fill128×16で現行Rectに合わせ、引き伸ばしを避ける。検証: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0 / image resource errors=0 / HUD validator errors=0`。

## 直近完了 (2026-05-28 Codex Stage 1 ボス画像復帰 / 表示文字化け修正)

- Stage 1 (`Lantern Field`) の中ボス/最終ボスは、生成済み `Boss_Pulswyrm.png` / `Boss_Nullwyrm.png` ではなく元の共通 `bossSprite` を使うように変更。
- Stage 2 以降は引き続き専用ボス画像を使うため、ステージ差別化は維持。
- 図鑑の相棒説明でプレイヤーに見える mojibake を修正: Ember / Sage / Drift / Genesis / Halo / Pulse。
- 進化ルート説明の mojibake を修正: SPEED / GUARD。
- 相棒選択カードの Drift Fox / Iron Bear / Genesis Core / Pulse Hydra の説明文を正規日本語に修正。
- ボス出現メッセージを `中ボス出現` / `ボス出現` に修正。
- 検証: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0`。

## 直近完了 (2026-05-28 Codex 表示文字列 mojibake 追加修復)

- Active code の表示用文字列を再スキャンし、文字化けが残っていたミッション/相棒選択/HUD/レリック/進化カード/進化コンボ/Data Lab/リザルト次目標を修正。
- 主な修正: `データチップ`, `ウェーブ`, `レリックを選ぶ`, `コアキーパー`, `不滅の証`, `最強進化 / APEX CORE`, `ボス撃破ボーナス`, `クロス進化見送り`。
- `HasPickedModule("スプリットビーム")` の文字化けも修正し、PHOTON LANCE コンボ条件が正しいモジュール名を参照するように戻した。
- Active string mojibake scan: no output。
- 検証: `odd-quote=0 / brace diff=0 / swallowed=0 / compile errors=0`。

## ⚠ 後日対応メモ (2026-05-25)

- **Stage 1 ボス画像を元 (共通 `bossSprite`) に戻したい**
  - 2026-05-28 対応済み: Stage 1 のみ共通 `bossSprite` へ戻した。履歴として項目は残置。
  - 現状: Codex が生成した `Boss_Pulswyrm.png` / `Boss_Nullwyrm.png` が自動採用されている (Resources/Skins 配下に置かれているため)
  - ユーザー判断: Stage 1 (Lantern Field) のボスは「元のデザインの方が良かった」
  - 待機理由: Codex が他の素材を引き続き生成中なので、終わったタイミングで一括対応
  - 対応案:
    - A. Stage 1 だけ別フォールバックに切替 (`SpawnBoss` 内で `currentStageId == 0` のとき `bossSprite` を使う)
    - B. `Boss_Pulswyrm.png` / `Boss_Nullwyrm.png` を削除して全体フォールバック
    - C. Codex に「元のボスデザインに寄せた」リテイク依頼

## Recent Done (2026-05-27 Codex Image Resource Validation)

- Added `Tools/TestImageResources.ps1` to validate PNG resource hygiene without touching `CoreLanternGame.cs`.
- The tool checks `Assets/Resources/Skins/*.png`, Unity `.meta` files, static `LoadOptionalSprite("Skins/...")` / `Resources.Load<Sprite>("Skins/...")` references, and `IMAGE_ASSET_BACKLOG.md` DONE entries.
- Validation result: 502 Skins PNGs, 121 code sprite references, 138 backlog PNG references, 29 DONE backlog PNG references, 0 errors.
- Remaining warning: optional `Player_Form4.png` is not present, so the existing procedural fallback is used. This is not a runtime blocker.
- Cleaned the active backlog wording for minimap assets: the minimap feature was already removed, so `Minimap_*` PNGs are now marked REMOVED instead of HOOK.

## Recent Done (2026-05-25 Codex Boss Alert Sprite Link)

- Boss alert/cutscene portrait now uses the same `GetBossEncounterSprite()` / `GetBossEncounterTint()` resolver as the spawned boss.
- `Pulswyrm` and `Nullwyrm` warning visuals should now match the actual boss image/tint that appears after the alert.
- Boss intro/victory cutscenes now derive name, subtitle, accent, warning color, portrait, background, and optional boss BGM from `isMidBoss` instead of passing separate strings.
- Optional boss BGM hook now matches audio docs: `BGM_Boss_Pulswyrm` / `BGM_Boss_Nullwyrm`, with legacy `BGM_BossPulswyrm` / `BGM_BossNullwyrm` fallback.
- Generated starter boss BGM files `BGM_Boss_Pulswyrm.wav` and `BGM_Boss_Nullwyrm.wav`; both are deterministic local synthesis with `.meta`, manifest, and license entries.
- Improved `Tools/GenerateStarterAudio.ps1` so `-NoOverwrite` skips existing WAV synthesis before doing expensive generation, while still keeping existing files in `AudioManifest.json`.
- Mid-boss victory cutscene now restores the current stage BGM after the victory cutscene ends, so `BGM_Boss_Pulswyrm` does not keep playing through the rest of Wave 5.
- Added `Tools/TestAudioResources.ps1` to validate `LoadAudioClip()` names, WAV files, `.meta` files, and `AudioManifest.json` consistency.
- `Tools/TestAudioResources.ps1` passes with 0 errors; only warnings are optional legacy boss BGM fallback names not being present.
- Roslyn compile check passed; remaining warnings are existing Unity SourceGenerator analyzer warnings plus unused fields.

## Recent Done (2026-05-25 Codex Stage/Boss Enemy PNGs)

- Generated and placed `LavaCrawler`, `Enemy_MagmaTitan`, `Enemy_CorruptionDrone`, `Enemy_FrostKnight`, `Enemy_VoltDasher`, `Boss_Pulswyrm`, and `Boss_Nullwyrm` transparent PNGs in `Assets/Resources/Skins/`.
- Added reusable generator `Tools/GenerateStageBossEnemyAssets.ps1` and preview `Assets/ArtSource/Generated_StageBossEnemy_Preview.png`.
- Verified all 7 PNGs are 256x256 and corner alpha is 0. No `CoreLanternGame.cs` edits were needed because the resource hooks already exist.
- Character/evolution/fusion redesign is still tracked as final-phase work and was not changed.

## Recent Done (2026-05-25 Codex Stage 4/5 Support PNGs)

- Generated and placed `StageThumb_Frost`, `StageThumb_Storm`, `Stage4_FrostCrystal_A`, `Stage4_FrostPatch_A`, `Stage5_LightningMarker_A`, and `Stage5_LightningStrike_A` in `Assets/Resources/Skins/`.
- Added reusable generator `Tools/GenerateStage45SupportAssets.ps1` and preview `Assets/ArtSource/Generated_Stage45Support_Preview.png`.
- Verified thumbnails are 160x90 opaque PNGs; hazard/VFX assets keep transparent corners.
- Debugged and backfilled `.meta` files for all Stage 4/5 support PNGs and the preview sheet.
- No `CoreLanternGame.cs` edits were made. These assets are ready for Claude or a later hook-up pass, but they are not yet auto-displayed by the current code.
- Roslyn compile check passed after the image-asset debug pass; remaining warnings are existing Unity SourceGenerator analyzer warnings plus unused fields.

最終更新: 2026-05-27

Claude と Codex が同時に作業する前提の残タスク一覧。  
`CoreLanternGame.cs` は競合しやすいので、同時作業中は担当を明確に分ける。

## 直近完了 (2026-05-25 Codex タイトル重なり/白点修正)

- プレイヤー発射時にキャラ中央へ白い点のように見えていた `SpawnAttackFlash(player.position, ...)` を停止
- 未使用化していた進化コアバッジ用フィールド/代入を削除し、キャラ絵の上に不要な中心装飾が乗らない状態に整理
- タイトル画面のタイトル/サブタイトル/サマリー/ヒーローデッキ/START/下部ボタンのY配置を再調整
- ヒーローデッキ内の説明ラベルを削除し、Cobalt / Data Egg / ルートバッジを縮小・再配置して画像同士の重なりを回避
- `MAIN_MENU_LAYOUT_CHECK` で不許可の矩形重なりなし、Roslyn compile check 通過

## 直近完了 (2026-05-24 Codex メインメニュー顔作り)

- タイトル画面をゲームの顔として再構成し、中央に `Cobalt Pup SPEED L3 + Data Egg Core + 3ルートバッジ` のキービジュアルを追加
- 背景に深いスクリーンとヒーローデッキを重ね、既存背景の大きな黄色円がUIを邪魔しにくいよう調整
- 進行情報を1行のコンパクトなステータスリボンに整理 (`BEST / CLEARS / MISSIONS / DANGER / STAGE / COMBO`)
- `START RUN` ボタンと下部サブボタンの配置を下げ、中央の見せ場と操作導線を分離
- Roslyn compile check 通過。残警告はUnity SourceGenerator警告と既存未使用フィールドのみ

## 直近完了 (2026-05-24 Codex Cobalt進化画像試作)

- Cobalt Pup の試作として `L0 / SPEED L3 / POWER L3 / GUARD L3` の4枚を生成
- 元画像は単色クロマキー背景で作成し、`remove_chroma_key.py` で透過PNG化
- 512x512版を `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/ready_512/` に保存
- プレビューは `Assets/ArtSource/CharacterDrafts_Cobalt_20260524/Cobalt_Evolution_Transparent_Preview.png`
- ユーザー指示で `Assets/Resources/Skins/Partner_S1_L0.png`, `Partner_S1_R1_L3.png`, `Partner_S1_R2_L3.png`, `Partner_S1_R3_L3.png` に反映済み
- 既存画像バックアップ: `Assets/ArtSource/CharacterBackup_CobaltBeforeImplement_20260524_041559/`
- 注意: L1/L2は今回未生成のため既存素材のまま。Cobaltを本採用するなら次に各ルートL1/L2を生成して中間進化も統一する

## 直近完了 (2026-05-24 Codex キャラ画像プロンプト確認)

- `docs/CHARACTER_IMAGE_PROMPTS.md` を確認し、全体方針は採用可能と判断
- Unity側の読み込み命名に合わせ、ルート画像は `Partner_S{n}_R{route}_L{lv}.png`、クロス進化は `Partner_S{n}_F{fusion}_L{lv}.png` へ整理
- L2個別プロンプト不足を補う共通生成ルールを追加
- Phase枚数とクロス進化Optionの計算ミス/typoを修正し、22枚→44枚→110枚→クロス段階投入の順番に整理
- ドキュメント修正のみ。コード変更・コンパイル確認なし

## 直近完了 (2026-05-24 Claude ステージ別敵 + ボス機能)

VS 風の「ステージごとに敵を変える」を実装 (見た目のみ) + ボス 2体を個別化 (見た目+機能)。

### Stage 別専用雑魚 4種追加 (見た目のみ、挙動は派生)

| 敵 | ステージ | 派生元 | 特徴 |
|---|---|---|---|
| `MagmaTitan` | Stage 1 (Lava) | Brute | HP↑、橙発光 |
| `CorruptionDrone` | Stage 2 (Broken Core) | Phantom | 高速、紫マゼンタ |
| `FrostKnight` | Stage 3 (Frost) | Brute | 重装、青白氷装甲 |
| `VoltDasher` | Stage 4 (Storm) | Dasher | 超高速、紫黄電撃 |

- `EnemyType` enum に 4種追加
- `SpawnEnemy` の switch に 4ケース追加 (HP/速度/触ダメ/sprite で個性付け)
- 各 Stage の出現確率テーブルに新敵を混入 (60% 共通 + 40% 専用敵)
- `heavyKind` フラグで Brute / MagmaTitan / FrostKnight をまとめて鎧トリム + HP バー幅 1.0 適用
- sprite ロードは既存敵フォールバック (Codex 生成後に自動切替)

### ボス 2体個別化 (見た目+機能)

#### Pulswyrm (中ボス, Wave 5) — 機動型
- **専用 sprite**: `pulswyrmSprite` (フォールバックは共通 bossSprite)
- **シグネチャ攻撃: 突進アタック**
  - 5秒毎にプレイヤーへ向かって 0.45秒ダッシュ (速度 8.5/s)
  - 命中で 2.2 dmg + Shake 0.30 + HitFreeze 0.08
  - 1.8〜8m の中距離で発動 (近すぎ/遠すぎは不発)
  - 発動前に attack flash + ring sparks で予告
  - Event log: `PULSWYRM 突進!`

#### Nullwyrm (最終ボス, Wave 10) — コア狙い型
- **専用 sprite**: `nullwyrmSprite`
- **シグネチャ攻撃: Core Mark**
  - 8秒毎にコアを狙うライン攻撃
  - 1.0秒予告 (ボス→コアの間に sparks 散布、中央 "⚠ CORE MARK" 表示)
  - 発動: コアに 2.5 dmg / 直線上のプレイヤーに 1.5 dmg / 直線上の敵に 2.0 dmg
  - コア HP < 35% の時は発動スキップ (詰みを回避)
  - 発動演出: Shake 0.35 + HitFreeze 0.10 + Boss SFX

### 共通対応

- `Enemy` クラスに ボス専用フィールド 5個追加 (`bossSpecialCooldown` / `bossChargeTimer` / `bossChargeDir` / `bossCoreMarkTimer` / `bossCoreMarkTarget`)
- `SpawnBoss` に初回クール設定 (mid 4.5s / final 6.0s)
- 新メソッド `UpdateBossSpecialAttack(Enemy, Vector2 dir, float dist)` 追加
- 既存 bullet fan ロジック直後に `UpdateBossSpecialAttack` を呼ぶ統合

### Codex 素材依頼 (新規)

| ファイル名 | 優先度 | 用途 |
|---|---|---|
| `Enemy_MagmaTitan.png` | P0 | Stage 1 専用敵 |
| `Enemy_CorruptionDrone.png` | P0 | Stage 2 専用敵 |
| `Enemy_FrostKnight.png` | P0 | Stage 3 専用敵 |
| `Enemy_VoltDasher.png` | P0 | Stage 4 専用敵 |
| `Boss_Pulswyrm.png` | P0 | 中ボス |
| `Boss_Nullwyrm.png` | P0 | 最終ボス |

ブレース差分 0、15,693 行。

## 直近完了 (2026-05-24 Codex 図鑑/進化ツリーUI整理)

- Claude作業中の Solar Anchor シグネチャ / Stage4-5敵組成 / Frost Crystal判定 / メインメニューコンボ進捗 / Last Stand / Resonance Wave は触らず、図鑑と進化ツリーの表示崩れを修正
- 図鑑をスクロールビュー化し、PARTNER 11体がEVOLUTION ROUTE/CROSS EVOLVEへ重ならないようカード配置・幅・文字サイズを調整
- 図鑑カードの説明文を `CodexEntry.description` から直接表示するようにし、Halo Caster / Pulse Hydra / Solar Anchor も説明欠落しないよう整理
- 進化ツリー上部の素体選択を11体対応に更新し、左上ラベルの埋まりと新キャラ未表示を改善
- Roslyn compile check 通過。残警告は Unity SourceGenerator と既存未使用フィールドのみ

## 直近完了 (2026-05-24 Claude プロデザイナー全体ブラッシュアップ)

プロ視点で気付いた7点を一括修正:

- **Solar Anchor 専用シグネチャ「コアシンク」追加** — 他キャラと公平に固有モジュール提供
  - Resonance Wave dmg +50% / 範囲 +0.8m / クール -25%
  - 新フィールド `resonanceWaveDamageBonus` / `resonanceWaveRadiusBonus` / `resonanceWaveCooldownMul`
  - `IsUpgradeUnlocked` で `solarAnchorActive` ゲート
- **Stage 4 (Frost Vault) 敵組成差別化** — Brute 多め (氷の世界に重戦士)、Bomber 少々 (クリスタル誘発)
- **Stage 5 (Storm Spire) 敵組成差別化** — Dasher 主役 + Shooter 増 (高速回避、雷誘導戦術性)
- **Frost Crystal の武器種対応拡張** — 弾だけでなく、オーラ / レーザー / 接触ダメージでも壊せるように
  - 「全プレイスタイルでクリスタル報酬を取れる」公平性確保
- **メインメニュー Archive にコンボ進捗追加** — `COMBO N / 8` 形式、図鑑要素の可視化
- **Last Stand 発動時の中央メッセージ** — `ShowMessage("LAST STAND")` で派手な瞬間を強化、Ring sparks 二重 + 二重 SFX
- **Stage 4/5 サムネ無し時のフォールバック色を識別性向上** — Frost Vault は氷の青、Storm Spire は嵐の紫
- **リザルト summary に Last Stand 使用状況追加** — `LAST STAND使用済` / `温存` 表示
- **Resonance Wave 発動時の floating text 追加** — `RESONANCE` で Solar Anchor の核機構を可視化

ブレース差分 0、15,336 行。

## 直近完了 (2026-05-24 Codex 最終ボス/リザルト/音/Stage2)

- **最終ボス重さ対策**: 弾/残像/スパーク/浮遊テキストの上限を圧縮し、ボス連続ヒット時のスパーク・SE・ヒットフリーズを間引き。多段ヒット型ビルドでもEditorが重くなりにくい方向へ調整
- **リザルト整理**: 背景に濃いスクリーンを重ねて裏HUDを見えにくくし、下部の長いモジュール羅列を廃止。MVP BUILD + 2行サマリ中心の読みやすい構成へ変更
- **Options整理**: ゲーム中に敵HP/火力/速度を変えられるアクセシビリティ項目を非表示化し、保存値も1.0固定に戻す。パネル高さと配置を再調整
- **画面端対策**: Camera clearFlagsをSolidColorに固定済み、床をワイド画面でも届くサイズへ拡張済み
- **音の上昇感抑制**: `Tools/GenerateStarterAudio.ps1` を更新し、BGM/LevelUp/Pickup/Evolve/Fusionの上昇音型を下降・低音寄りへ再生成。`BGM_Stage2.wav` を追加
- **Stage2専用要素**: Stage2 `Lava Cache` 専用敵 `LavaCrawler` を追加し、Stage2の敵組成に混入。専用BGMは `currentStageId == 1` で自動選択
- **検証**: Roslyn compile check 通過。残警告はUnity SourceGenerator警告、`stageSubtitle` / `stageVisionMultiplier` / `allyName` 未使用のみ

## 直近完了 (2026-05-25 Claude P1 系統 5項目連続実装)

「順番にお願い」を受けて 🟢 P1 5 項目を一括処理。

### #1 SPEED / GUARD ビルド強化 (POWER 一強解消)
通常 upgradePool に新規 12 個を追加。SPEED 5 / GUARD 5 / 汎用 2。

- **SPEED**: ターボブースト, クイックリロード, マルチチェイン, SPEED専用ベロシティチャージ, SPEED専用ミラージュバースト
- **GUARD**: コアキャパシティ, リング拡張, スパイクシェル, GUARD専用トライリング, GUARD専用ガーディアンウェーブ
- **汎用**: ステディハート, オーラブースト

既存 `IsUpgradeUnlocked` の "SPEED専用" / "GUARD専用" prefix 検査でゲート済み。

### #2 Elite 敵
- `Enemy.isElite` フラグ追加 (Runner/Brute/Shooter のみ対象)
- 出現率: Wave 5 = 4% / 7 = 8% / 9+ = 12% (Elite Swarm wave は +6%)
- ステ補正: HP ×1.75, 速度 ×1.06, 接触 ×1.10, スケール ×1.18
- 見た目: 金色 glow + Elite Ring + spawn sparks
- 撃破時: データ ×2.5 + ELITE+ floating text + 金色 sparks

### #3 ボス攻撃パターン追加 (Pulswyrm mini-enrage)
spec の「中ボスは Phase 2 にせず軽い enrage」を実装。

- `TriggerMidBossEnrage`: HP <= 50% で発動 (`phase2Triggered` を共通フラグ再利用)
- 移動速度 ×1.12, 弾幕 3→4 発, 拡散 18°→22°, クール 1.18→1.02s
- 警告 pulse (橙色) + Flash + Shake + Boss SFX + `PULSWYRM ENRAGE` 表示

### #4 レリック種類増 (10 → 15)
- **電流の輪**: チェイン +2 hop / 攻撃 +5% (SPEED 系)
- **共鳴の翼**: コアオーラ DPS +3 / 半径 +1m (Solar Anchor 系汎用化)
- **覚醒の刻印**: 現レベル XP 半分即チャージ + データ +8% (テンポ系)
- **回収の祝福**: 回収範囲 +50% / 拾い回復 +15% (回収特化)
- **過充電の核**: 弾数 +1 / 連射 +12% / 単発 -6% (攻撃テンポ)

### #5 Wave 編成・予告の個性化
既存 `waveTrait` が stat multiplier しか効かなかった問題を解消。spawn 時に type バイアスを追加:

- **Shooter Raid** (W5): Shooter 55%
- **Rush** (W3): Runner 55% / Dasher 20%
- **Iron Skin** (W6): Brute 50%
- **Berserk** (W2/8): Dasher 35% / Runner 20%
- **Dark Field** (W4/9): Phantom 45%
- **Elite Swarm** (W7): Brute 32% / Shooter 28%

Stage 別専用敵 (MagmaTitan/CorruptionDrone/FrostKnight/VoltDasher) は対象外で維持。

ブレース差分 0、16,102 行 (15,693 → +409)。

---

## 直近完了 (2026-05-25 Claude Boss Phase 2 実装)

`docs/BOSS_PHASE2_SPEC.md` 準拠で Nullwyrm Phase 2 を実装。

### Enemy 拡張
- `isMidBoss` 明示フラグ追加 (HP閾値推定の脆弱性解消)
- Phase 2 フィールド 6個: `phase2Triggered` / `phase2TransitionGrace` / `spiralCooldown` / `spiralTelegraphTimer` / `summonCooldown` / `firstSummonDone`
- 既存 `maxHp > 80f` 判定 2箇所 (`UpdateBossSpecialAttack` / ボス勝利) を `isMidBoss` に置換

### Phase 2 トリガー
- `TryTriggerBossPhase2`: HP <= 50% + 非ミッドボス + 未トリガー + UI 非選択中
- `TriggerBossPhase2`: scale +14%, HP バー hot magenta, Transition Pulse (リング + 軽ダメ 0.45), grace 3s, `NULLWYRM PHASE 2` 表示

### Phase 2 攻撃
- **通常弾幕強化**: 弾 5→6, 拡散 26°→34°, クール 1.05→0.92s, 速度 ×1.08, 理想距離 3.2→3.6m
- **Spiral Corruption** (`FireBossSpiral`): 14発スパイラル, 0.75s 予告, 5.5s クール
- **Glitch Summon** (`SpawnBossMinions`): 11s 毎、初回 Runner ×2 / 後 Runner ×2 + Phantom ×1, コアから 4.5m 以上、敵総数 >14 でスキップ
- **Core Mark 調整**: cooldown 8→10s, telegraph 1.0→1.35s

### Mercy
- コア HP < 35% で Core Mark スキップ (既存)
- 敵総数 > 14 で summon スキップ
- Core Mark テレグラフ中は spiral 控え (重ね回避)
- grace 3s 中は射撃停止 (低圧力)
- Phase 2 中は Thermal Surge 無効化 (重複圧力回避)

### SpawnEnemy 拡張
- `SpawnEnemy(EnemyType?, Vector2?)` overload 追加 (forceType/forcePos)
- 既存 `SpawnEnemy()` は内部で `(null, null)` を呼ぶ薄いラッパー

### HUD/演出
- `ShowMessage("NULLWYRM PHASE 2")` + Flash + Shake 0.45 + HitFreeze 0.12 + Boss SFX
- HP バー fill 色を `(1, 0.18, 0.92)` に変更、新ボス出現時にリセット
- Spiral テレグラフ中は回転リング sparks、発動時に派手な ring sparks

ブレース差分 0、15,908 行。

## 🟡 保留タスク (完成が近づいたら再検討)

ユーザー判断で「今は実装しない、完成前に再評価」と決まったもの。  
削除でなく **意図的な保留** なので、最終QAフェーズ前に1件ずつ要否再決定する。

### 死亡時の難度下げリンク (B7 改)
- 内容: リザルト画面 RETRY 横に「Danger を1下げて RETRY」ボタン
- ユーザー指示: **B7 (初回死亡 Accessibility 提案) と同じ理由で要検討** = patronizing 認定リスクをどう扱うか方針確定してから

### プロ目線レビューで「削った」候補 10件
完成が近づいたら 1件ずつ再評価。現時点では「やらない」と決めた:

- A1 シグネチャ クールタイマー HUD (自動発動なので不要判定)
- A3 重たいチュートリアル (ジャンル文化に反する)
- B2 stat delta floating text (画面ノイズ)
- B3 Wave 予告テロップ (waveTrait 表示で代替済)
- C1 動的カメラズーム (酔い・狙いブレリスク)
- C2 Heat mode パーティクル culling (性能問題未確認のため早すぎる)
- C4 ヒット表現の敵タイプ別差別化 (体感差小)
- C5 ノックバック表現強化 (既に十分)
- C7 ダメージ数字強化 (既に十分)
- E1〜E5 技術リファクタ全般 (ユーザー無関係)

## 直近完了 (2026-05-22 Claude Stage 4/5 追加)

- **Stage 4 (Frost Vault) 実装** — 氷の保管庫
  - **凍結クリスタル × 4**: 弾で破壊可能 (HP 12)、破壊時に半径1.5m範囲を凍結 (敵 knockback + 2dmg) + データ +15
  - **結霜パッチ × 3**: ダメージなし、接触中 移動 -25% (純粋な減速ゾーン)
  - マップスキン: 青白の冷気 wash
  - 敵組成: 既存テーブル流用 (今後調整余地)
  - Wave trait リネーム: Cold Snap / Blizzard / Frost Pack / Glacier Hide / Snow Drift
- **Stage 5 (Storm Spire) 実装** — 雷の尖塔
  - **ランダム雷撃**: 12秒毎に random 地点に落雷予告 → 1.5秒テレグラフ後発動
  - 落雷範囲 2.5m: 敵に **12dmg + knockback (1.5秒スタン代替)** / プレイヤーに **3dmg**
  - テレグラフ中は黄→白の点滅加速、発動で Flash 0.35 + Shake 0.45 + HitFreeze 0.10
  - マップスキン: 紫の嵐 wash
  - Wave trait リネーム: Static Surge / Thunder Shroud / Storm Swarm / Charged Shell / Voltage Rush
- **共通対応**
  - MaxStageId 2 → 4
  - StageHazardKind enum: FrostCrystal / FrostPatch / LightningMarker 追加
  - StageHazardZone: crystalHp / crystalMaxHp / lightningStrikeAt / lightningResolved 追加
  - Run Config Stage chip: 5個対応 (180×108、フォント 16、Stage 3/4 はサムネ無しで色のみフォールバック)
  - GetStageDisplayName / GetStageDisplayTrait / RefreshSelectionInfoLine が全 5 stage 対応
  - ClearStageObjects に frostPatchSlowActive / lightningNextStrikeTimer リセット追加
  - メインメニュー Archive 表示で "STAGE 解放 SX / S5" 形式 (上限明示)

### Codex 素材依頼 (Stage 4/5 用、現状はフォールバック動作中)

優先度 P1:
- DONE `StageThumb_Frost.png` (160×90, 青白の氷の風景)
- DONE `StageThumb_Storm.png` (160×90, 紫の嵐 + 雷撃の風景)
- DONE `Stage4_FrostCrystal_A.png` (96×96 推奨、ダイヤ型結晶)
- DONE `Stage4_FrostPatch_A.png` (256×256, 円形 薄い氷)
- DONE `Stage5_LightningMarker_A.png` (256×256, 黄→白点滅対応の円形警告)
- DONE `Stage5_LightningStrike_A.png` (任意, 雷撃の縦線VFX)

## 直近完了 (2026-05-22 Claude 新キャラ⑤ + Stage 3)

- **新キャラ⑤ Solar Anchor (style 11) 実装** — コア共鳴型、TD 本質を補強
  - コア半径 3m 以内: 全 dmg +25% + 移動 +25%
  - コア半径 6m 以遠: HP 回復停止 (実装は player update 側でフラグ提供)
  - 8秒毎にコアから半径 3m 範囲ダメージ「Resonance Wave」
  - ベース HP +2, コア HP +4, bulletDamage ×0.95 (共鳴ボーナス前提)
  - 識別色: ゴールド (1, 0.86, 0.28)
- **Stage 3 (Broken Core Network) 実装**
  - 汚染パッチ × 4 (Lava より弱罰、grace 0.85s, DPS 0.10, 接触中 移動 -25%)
  - リレー装置 × 3 (2秒立ち続けで起動 → +25 データ / +2 HP / +2 コアHP / 1回限り)
  - Stage 3 専用マップスキン (紫マゼンタ wash + outer haze)
  - 敵組成: Phantom 多め + Dasher 増 (ステルス&高速)
  - Wave trait リネーム: Berserk→Glitch Rage, Dark Field→Static Veil, Elite Swarm→Corruption Swarm, Iron Skin→Hardened Shell
  - Stage 2 クリアで Stage 3 解禁 (既存 stageMaxUnlocked ロジック流用)
- **共通対応**
  - species clamp 1-10 → 1-11
  - 4つの色関数 (Glow/Deep/Accent/Armor) に case 11 追加
  - GetPartnerShortName "Solar" 追加
  - Run Config Panel の Stage chip 3個対応 (S1/S2/S3, サイズ 230×130)
  - RefreshStageChips の thumb 判定 3段階対応
  - RefreshSelectionInfoLine / GetStageDisplayName が Stage 3 対応
  - StageHazardKind enum 拡張 (CorruptionPatch, RelayDevice 追加)
  - StageHazardZone に relayActivated/relayChargeTimer/relayChargeRing 追加
  - Codex 図鑑に Halo Caster / Pulse Hydra / Solar Anchor の3エントリー追加 (8→11)
  - UpdateSolarAnchor を Update loop に統合
  - DamageEnemy に coreBondNearActive 補正 (+25%)

## 直近完了 (2026-05-22 Claude プレイヤー必須項目スプリント)

- **Codex 体験面ブラッシュアップ**: OptionsにBGM/SE音量スライダーを追加。リザルトにRUN RANKとNEXT目標を表示。強化カードはレアリティを `BASIC/RARE/EPIC` 併記にし、説明文のはみ出しを抑制
- **Codex BGM上昇感の抑制**: `Tools/GenerateStarterAudio.ps1` の `New-Bgm` を調整。高い上昇リードを廃止し、32秒の低め・少なめ・下降気味パルスのループへ変更。`Assets/Resources/Audio/BGM.wav` と manifest/license を再生成
- **Codex 強化カード押せない問題修正**: Data Lab/ショップで `interactable=false` になった共通カードを通常強化/レリック/進化表示時に `true` へ戻すよう修正。スキップボタンも表示時に有効化
- **Codex SE多段ヒット抑制**: `PlaySfx` にSE別の最短再生間隔と音量係数を追加。`Hit` / `Shoot` / `Kill` / `Pickup` などが多段ヒット時に重なりすぎてうるさくなる問題を軽減
- **Codex Audio Automation**: `Tools/GenerateStarterAudio.ps1` を追加。Unityが現在読む `BGM/Shoot/Hit/Kill/Pickup/LevelUp/Evolve/Fusion/Boss/GameOver.wav` を第三者素材なしで自動生成し、ライセンスtxt/manifestも出力する仕組みを用意。手順は `docs/AUDIO_AUTOMATION.md`
- **Codex Audio Plan**: 無料/商用可を優先する音素材方針を `docs/AUDIO_ASSET_PLAN.md` に整理し、AI生成プロンプト集 `docs/AUDIO_GENERATION_PROMPTS.md` とライセンス台帳テンプレ `docs/AUDIO_LICENSE_LOG_TEMPLATE.md` を追加。`SOUND_ASSET_CANDIDATES.md` にStable Audio / Eleven Music / Suno Freeの扱いも追記
- **A5 Last Stand**: ラン1回限り、致死ダメージを 1HP まで救済 + 1.4秒無敵。`deathShield` レリックと独立 (両方持てば2回救済)
- **B4 Stage 2 敵組成差別化**: Stage 2 では Bomber/Phantom 出現率↑、Dasher やや減 (熱で機動性低下イメージ)。Wave 4 (Smoke Field) で Phantom 早期登場
- **B6 進化コンボ カットイン強化**: 専用バナー `ShowEvolutionComboBanner` 新設、1.6秒フェード in/out、Shake/Flash/HitFreeze/Spark/Ring 全部盛り、Evolve SFX 追加
- **C3 モジュールアイコン表示**: Data Lab (ショップ) カードに `ApplyModuleCardIcon` 呼び出し追加 (通常モジュール / レリックは既に接続済)
- **C6 進化カットイン強化**: 既存 evolutionCutsceneTimer を 1.45→2.10s に延長、開幕に Flash + Shake + 2層 RingSparks + Evolve/Hit 二重SFX 追加
- **A2 Build Report 軽量版**: リザルト summary に取得モジュール全一覧を 1行で集約 (`BuildResultModulesLine`、重複 ×N 表記、8件超で省略)。STAGE/Danger 情報も summary に追加

## 直近完了 (2026-05-22 Codex 相棒選択UI修正)

- BEAST LINK相棒選択を固定詰め込みから、3列カード + 縦スクロールへ変更
- 上部に `戻る` ボタンを追加し、Escキーでもメインメニューへ戻れるようにした
- 人数が増えても無理に1画面へ押し込まず、カード数に応じてスクロールコンテンツ高さを自動調整
- カード内の発光円、アイコン、名前、特性、説明、ステータスバーの位置/サイズを再調整し、文字重なりを軽減
- Roslyn compile check 通過。残警告は既存の analyzer / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex 非キャラ画像 Batch 2)

- キャラ/進化素材には触れず、非キャラ素材32枚を `Tools/GenerateNonCharacterPolishBatch2.ps1` で生成
- 追加レリック4種 `Relic_Titan`, `Relic_Overdrive`, `Relic_DataSurge`, `Relic_Apex` を作成し、コード側のレリックカード/HUD表示へ接続
- モジュール識別用アイコン20種 `ModuleIcon_*` を作成し、強化カードのアイコン表示へ接続
- Stage選択/今後のStage実装向けに `StageThumb_*`, `Stage2_*`, `Stage3_*`, `Pickup_Data_64` を作成
- プレビュー `Assets/ArtSource/Generated_NonCharacterPolishBatch2_Preview.png` を作成
- Roslyn compile check 通過。残警告は既存の analyzer / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex 全体デバッグ/ジャンル差分反映)

- コンパイル、主要フロー、リザルト復帰、メニューオーバーレイ、強化/進化/レリック選択、ログを広めに確認
- メニュー上の図鑑/ミッション/進化ツリー/オプション表示中にEnter/Spaceでラン開始してしまう導線をブロック
- レリック選択中にOキーでオプションが重なる導線をブロック
- リザルト/ポーズのリロード処理に連打ガードを追加し、多重ランタイム再生成を防止
- Unity 6でobsolete化していた `FindFirstObjectByType` を `FindAnyObjectByType` に置換
- 同ジャンル分析を元に、通常強化へ `スキップ +データ` を追加。候補枯渇時は `緊急補給` を出してソフトロックを防ぐ
- Stage2の溶岩縁に少量のデータを配置し、危険地帯へ近づくリスク/報酬を追加
- 分析と検討リストを `docs/DEBUG_AND_GENRE_GAP_ANALYSIS_20260522.md` に作成
- Roslyn compile check 通過。残警告は既存の analyzer / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex リザルト復帰後の音消え修正)

- メインメニュー復帰後に再スタートするとBGM/SEが聞こえなくなる問題に対応
- 原因は、生成型リロード後に自前生成した `Main Camera` へ `AudioListener` が付かず、AudioSourceは再生していても出力先がない状態になっていたこと
- `CreateWorld()` でカメラ初期化後、`AudioListener` がなければ追加するようにした
- Roslyn compile check 通過。残警告は既存の analyzer / obsolete / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex リザルト復帰再修正)

- リザルト復帰時にUnityの空シーン画面へ戻ってしまう問題に再対応
- 原因は、保存済み/Build Settings入りシーンでも `LoadScene` では `[RuntimeInitializeOnLoadMethod]` が再発火せず、生成型ゲームが再構築されないこと
- リザルト復帰/リスタートでは `SceneManager.LoadScene` を使わない方針に変更
- `ReloadRuntime(autoStartRun)` からコルーチン `ReloadGeneratedRuntimeRoutine()` を起動し、生成済みルートを破棄した次フレームに新しい `Core Lantern Game` を作る方式へ変更
- リザルトの `もう一度` / `メインメニュー` と、ポーズ中の `RESTART RUN` も同じ共通処理へ接続
- Roslyn compile check 通過。残警告は既存の analyzer / obsolete / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex Stage2マップデザイン差別化)

- Stage2 `Lava Cache` 専用のマップスキンを追加し、Stage1と床の印象が同じ問題に対応
- `CreateStage2MapSkin()` を追加し、赤黒い熱ウォッシュ、焼けた床プレート、導熱ライン、外周ヒートマーカー、中央安全プレートを生成
- Stage2開始時にだけ専用スキンを `stageObjects` として追加し、Stage1へ戻すと `ClearStageObjects()` で消える構造にした
- `Floor_Hazard_Lava` をロードし、溶岩プールの見た目へ反映。なければ従来の円形表示にフォールバック
- Roslyn compile check 通過。残警告は既存の analyzer / obsolete / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex Stage2選択/レリック再調整)

- Stage2が選べない問題に対応。`Stage_MaxUnlocked` が未作成の古い/新規セーブでも Stage2 を選択可能にした
- 既存クリア済みデータ (`Clears` / `BestWave`) からもStage解放状態を補正するようにした
- レリック間の性能差を大幅調整。攻撃系、耐久系、回収系、隠しAPEXの突出を抑え、選択差を圧縮
- `Core Pulse` の周期/範囲/ダメージも下げ、放置防衛性能が強くなりすぎないよう調整
- HUDのレリック名表示も、追加レリックがID表示にならないよう追従
- Roslyn compile check 通過。残警告は既存の analyzer / obsolete / 未使用フィールドのみ

## 直近完了 (2026-05-22 Codex レビュー分析反映)

- 同ジャンルのユーザーレビュー/業界レビュー傾向をもとに `docs/STAGE_DESIGN_SPEC.md` を更新
- Stage2は「溶岩で罰する」より「危険地帯の近くに報酬を置く」方針へ寄せ、溶岩/煙の数値を控えめに調整
- Stage3はBroken Core Networkを「任意の有利目標」として明確化し、無視してもクリア可能な設計に整理
- `docs/BOSS_PHASE2_SPEC.md` にレビュー由来のボス設計原則を追加
- Nullwyrm Phase2は硬いだけにせず、Core Markを主役にしつつ、弾幕/召喚/移動強化の数値を控えめに再調整
- 今回はMarkdown設計のみ更新。`Assets/Scripts/CoreLanternGame.cs` は未変更

## 直近完了 (2026-05-22 Codex 難易度緩和)

- 特定キャラ依存を下げるため、全キャラ共通で序盤耐久と育成速度を上げた
- 初期HP 5→6、コアHP 18→24、初期EXP必要量 8→7、回収範囲 1.1→1.25
- Wave全体の敵数、後半追加湧き、湧き速度、HP倍率、速度倍率を緩和
- Shooter弾、中ボス/ボス弾、接触ダメージ、コア接触ダメージを軽減
- 中ボスPulswyrmのHPを80以下に収め、最終ボス扱いの5way弾幕にならないよう調整
- Dark Fieldの視界低下を38%→24%へ緩和
- Roslyn compile check 通過。残警告は既存の analyzer / obsolete / `allyName` 未使用のみ

## 直近完了 (2026-05-22 Codex 背景/カットイン画像配置)

- `CoreLanternGame.cs` で低密度版の全画面背景を有効化
- `Background_MainMenu`, `Background_Victory`, `Background_Defeat` をメニュー/リザルトに比率維持で配置
- `Background_Evolution` / `Cutin_Evolve_*` / `Background_CrossEvolution` を進化・クロス進化カットインに比率維持で配置
- 未接続だった `Background_BossPulswyrm` / `Background_BossNullwyrm` をボスカットインへ追加
- 暗転板、危険vignette、警告帯、HUDバー/パネル、カードフレームは引き伸ばし品質リスクがあるため引き続きOFF
- Roslyn compile check 通過。残警告は既存の analyzer / obsolete / `allyName` 未使用のみ

## 直近完了 (2026-05-22 Codex キャラ画像最終フェーズ明記)

- `CLAUDE_TASK_PROGRESS.md` でキャラ素体・進化後・クロス進化画像を `FINAL` 扱いに変更
- `IMAGE_ASSET_BACKLOG.md` に「作業量が大きいため最終フェーズまで生成/差し替えしない」と明記
- Claude側は当面、名前/説明/図鑑/進化ツリー表示の整合だけ見て、正式PNG生成は後回しにする方針へ整理
- 今回も `Assets/Scripts/CoreLanternGame.cs` は未変更

## 直近完了 (2026-05-22 Codex Claude進捗表作成)

- 指示書・台帳・直近完了を元に `CLAUDE_TASK_PROGRESS.md` を新規作成
- Claude向けにP0/P1/P2の実装優先度、現状、次アクション、参照資料、競合注意を表形式で整理
- Pulse Hydra / シグネチャ強化モジュールなどClaude側で進んだ項目もQA待ちとして反映
- 今回も `Assets/Scripts/CoreLanternGame.cs` は未変更

## 直近完了 (2026-05-22 Codex 背景画像低密度再生成)

- `Tools/GeneratePolishedNonCharacterAssets.ps1` の `Draw-Backdrop` を低密度背景方針へ修正
- `Background_MainMenu/BossPulswyrm/BossNullwyrm/Evolution/CrossEvolution/Victory/Defeat` を1920×1080で再生成
- コード互換aliasの `MainMenu_Background`, `Result_Clear_Background`, `Result_GameOver_Background`, `Cutin_*` も同じ低密度方針で再生成
- 中央装飾・グリッド・赤縦縞・リング状グローを削除し、中央60%はほぼ単色グラデのみへ変更
- 確認用プレビュー `Assets/ArtSource/Generated_PolishedNonCharacterAsset_Preview.png` を更新
- バックアップ作成: `Assets/ArtSource/NonCharacterAssetBackup_20260522_003259/`
- 今回も `Assets/Scripts/CoreLanternGame.cs` は未変更

## 直近完了 (2026-05-22 Codex 設計/台帳更新)

- 新ステージ②③仕様書を `docs/STAGE_DESIGN_SPEC.md` に作成
- ボス第2形態仕様書を `docs/BOSS_PHASE2_SPEC.md` に作成
- キャラ大幅リデザイン仕様書ドラフトを `docs/CHARACTER_REDESIGN_SPEC.md` に作成
- 音素材候補リストを公式ライセンス確認リンク付きで `docs/SOUND_ASSET_CANDIDATES.md` に作成
- `Pickup_Data.png` は現状 116×112 を維持し、64×64再作成は小アイコン化フェーズの別タスクとして記録
- `IMAGE_ASSET_BACKLOG.md` に旧Codex生成画像の廃止/再作成済み扱いと新非キャラ素材の状態を追記
- `ASSET_REVIEW_REPORT.md` に本日生成済み非キャラ素材とMarkdown設計資料の棚卸しを追記
- 今回は `Assets/Scripts/CoreLanternGame.cs` を触らず、Markdown/台帳のみ更新

## 直近完了 (2026-05-22 シグネチャ強化モジュール 10種追加)

- 各キャラ専用機構を伸ばすモジュール (アプローチA) を 10件追加、すべて `IsUpgradeUnlocked()` ゲート付き
  - **ファンネル系**: ビット増設 / ビット連射 / 突撃連打 / 突撃強化 (Halo Caster)
  - **レーザー系**: レーザー増幅 / レーザー射程 / 貫通効率UP (Pulse Hydra)
  - **オーラ系**: オーラ拡大 / オーラ強化 (Wraith Lynx + プラズマオーラ)
  - **吸血系**: 吸血増幅 (Wraith Lynx + HP吸収弾)
- 既存 const → field 化 (モジュールで変更可能に): `funnelDetachInterval`, `funnelDetachDamageMultiplier`, `laserChainDamageMul`, `lifestealAmount`
- Pulse Hydra 射程外バグ修正 (FindNearestEnemy → laserMaxLength 以内検索に変更)
- Pulse Hydra 移動速度ペナルティ復活 (`moveSpeed *= 0.92f`)

## 直近完了 (2026-05-22 第3弾キャラ④ Pulse Hydra)

- **新キャラ ⑩ Pulse Hydra (持続レーザー型) 実装**
  - 最寄り敵に常時ビーム接続、`0.15秒` ごとに tick ダメージ
  - ビーム視覚化: squareSprite を stretch + rotate (LineRenderer 不使用、既存パターン踏襲)
  - ダメージ: `bulletDamage × 5.0 × tickInterval` (≈ 通常DPSの2.7倍相当)
  - 本体メイン弾は控えめ (fireRate ×0.25, bulletDamage ×0.55)
- Laser system 追加: `EnsureLaser` / `UpdateLaser`、フィールド 6 + 定数 1
- species clamp 1-9 → 1-10 拡張 (4箇所)、short name "Pulse" 追加
- パートナー UI は既に 4×3 = 12 スロットなので追加レイアウト変更不要

## 直近完了 (2026-05-21 段階的UI素材有効化 Stage1 + 即ロールバック)

- Codex 生成画像の検収完了: 全カテゴリ仕様通り (Card×5、HUDPanel×5、HUDBar×7、Background×8、Bullet×4、Effect×5、Icon×9、Floor×3)
- `UseGeneratedUiSkins` フラグを 5カテゴリ別に分割: `UseGeneratedBackgrounds` / `UseGeneratedHudBars` / `UseGeneratedHudPanels` / `UseGeneratedCutscenes` / `UseGeneratedCards`
- **Stage 1 (背景) を試験有効化 → 即ロールバック**
  - 実機確認で「線が多い・縦縞が UI 圧迫・全体ノイズが多い」と判明
  - `UseGeneratedBackgrounds = false` に戻して procedural 表示へ
  - Codex に「シンプル・低密度・低コントラスト」で背景画像再生成を依頼予定 (`HUD_UI_ASSET_REQUEST.md` 改訂)
- 弾/ピックアップ/進化リング/カードルートPNG は元から自動適用 (LoadOptionalSprite フォールバック方式)
- Stage 2-5 は順次有効化予定 (HUDバー / HUDパネル / カットイン / カードフレーム)

## 直近完了 (2026-05-21 Codex 非キャラ素材)

- `HUD_UI_ASSET_REQUEST.md` に基づき、キャラ以外の画像素材をスマート寄りに再生成
- 生成スクリプト: `Tools/GeneratePolishedNonCharacterAssets.ps1`
- 確認用プレビュー: `Assets/ArtSource/Generated_PolishedNonCharacterAsset_Preview.png`
- 旧素材バックアップ: `Assets/ArtSource/NonCharacterAssetBackup_20260521_233106/`
- 生成/更新対象: `Card_*`, `HUD_Panel_*`, `HUD_Bar_*`, `HUD_WaveProgress_*`, `Background_*`, `MainMenu_Background`, `Cutin_*`, `Result_*`, `Bullet_*`, `Pickup_Heal`, `Effect_*`, `Icon_Link_*`, `Icon_Stat_*`, `Floor_DarkBase`, `Floor_Hazard_Lava`, `Arena_Boundary`
- 注意: `CoreLanternGame.cs` は触っていない。大型UI画像はまだ `UseGeneratedUiSkins = false` でゲーム内未使用のため、次は比率維持/9-slice前提で段階的に有効化するか判断する

## 直近完了 (2026-05-21 第3弾キャラ開始)

- **新キャラ ⑨ Halo Caster (ファンネル型) 実装** — 自律ビット3機が周回し最寄り敵を自動攻撃。本人連射は控えめ
- **Halo Caster ビット強化 (A1 離脱突撃)** — 5秒毎にビット1機が周回離脱して敵に突撃、通常弾の4.5倍ダメ + ヒットエフェクト
- Funnel system 追加: `EnsureFunnelBits` / `UpdateFunnelBits` / `SpawnFunnelBullet`、最大6機まで対応
- パートナー選択UI を 4×2 → 4×3 グリッドに拡張 (cardH 290→200、合計12スロット、9キャラ使用)
- カード内部レイアウト全面再計算: Badge y=+82, Icon y=+30, Title y=-2, Trait y=-22, Desc y=-42, Stat y=-72
- species clamp 1-8 → 1-9 拡張 (sprite/color/short name)

## 直近完了 (2026-05-21 後半)

- パートナー選択画面: 凡例バーが下段カードの SPC バーを覆っていた致命問題を修正（凡例バー撤去）
- パートナー選択画面のレイアウト数値を厳密化（cardH 290、stat container y=-90、SPC まで card 内に完全収容）
- カード描画系の `SetCardText` / `ApplyCardVisual` で隠れた font/位置の強制上書きを削除（CreateButton の大きい値が活きる）
- ボス優先オートエイム削除: ボス戦中も雑魚を狙えるように `score -= 9999f` 撤去
- Wraith Lynx 大幅強化: 弾は撃たない前提で近接ステ全方位強化（オーラ 5→9、接触 2.4→4.0、KB 2.5→3.5、移動 1.32→1.45、HP +3→+5、連射 ×0.10）
- ノックバック「瞬間移動」感を解消: Enemy に `knockbackVelocity` 追加、`position +=` 一発から 0.22秒指数減衰へ
- ミニマップ機能を完全撤去（コード・オプション・PlayerPrefs キー含む全削除）
- HUD/UI 素材発注ドキュメント `HUD_UI_ASSET_REQUEST.md` を Codex 向けに作成

## 直近完了 (2026-05-21)

- SplashDamage → DamageEnemy → SplashDamage の再帰でWave 9頃にStackOverflowを起こす問題を `splashDamageDepth` ガードで修正
- GUARDノックバックを「半径AoEバースト + 瞬時押し戻し + クールダウン (0.18秒)」方式に書き換え
- GUARD進化4種のノックバック値を 1.0~2.2 → 2.2~4.0 に大幅増、全ルートで`playerKnockback`が積み上がる
- SPEED/POWER系の進化倍率を全体的にナーフ（GUARDが弱すぎる問題の解消）
- 新キャラ追加: **Wraith Lynx (近接型, style 7)** — オーラ近接ダメ＋接触バースト＋吸血＋移動速度
- 新キャラ追加: **Genesis Core (全リンク所持・隠し最強, style 8)** — Nova/Bulwark/Siphon/Phase 全フラグONでスタート
- パートナー選択画面を 3×2=6 → 4×2=8 グリッドに拡張、新キャラ2人を表示
- 図鑑に **Wraith Lynx / Genesis Core** を追加し、8体表示に対応
- 進化ツリーの素体ストリップを8体対応にし、選択中素体でルート/クロス進化プレビューを更新
- style7/8 の暫定PNGを生成: 素体2枚 + ルート24枚 + クロス進化48枚
- レリック6種、HUD/UI追加素材、ミニマップ追加素材、メニュー/リザルト/カットイン背景を生成
- ボスHP 2倍化 (Pulswyrm 80→160, Nullwyrm 190→380)
- UI読みやすさ底上げ: HUDバーラベル 13→15、Loadout 13→15、モジュールスロット 12→14、リンクヒント 11→13、Wave進捗 16→18、Partner名 14→16
- MAP以外のUI/HUD画像を一度接続したが、品質低下のため大型画像は無効化。小アイコンのみ維持し、パネル/カード/背景はスマートなコード生成UIへ戻した

## Codex現状確認で追加した残タスク (2026-05-21)

詳細: `CURRENT_STATUS_REVIEW_20260521.md`

- 新キャラ `Wraith Lynx` / `Genesis Core` の図鑑・進化ツリー・画像台帳対応は完了。品質は暫定なので後で大幅リデザイン対象
- `Partner_S7*.png` / `Partner_S8*.png` は暫定生成済み
- `AssetReview_FusionStages.png` 上ではクロス進化が「メインキャラがリンクを吸収した姿」より、丸いエンブレム変化に見えやすい
- `bossModuleReady` は未割当で常にfalse。コード分岐は残っているが、コメント上はボス後報酬削除済みなので仕様確定が必要
- HUD/Relic/UI追加素材のうち、小アイコンのみコード使用中。大型UI画像は引き伸ばし・ノイズ問題により無効化。Minimap追加素材は未接続
- マップ素材は整理されたが、実機では床線密度が高くなりすぎる可能性があるので再確認が必要

## まだ残っている第3弾キャラ (実装難度: 中-高)

- ~~新キャラ③ **ファンネル型**~~ → **Halo Caster** 実装完了 (2026-05-21)
- ~~新キャラ④ **持続レーザー型**~~ → **Pulse Hydra** 実装完了 (2026-05-22)
- 新キャラ⑤ **コア合体型** — プレイヤーがコアと一体化する（タワーディフェンス本質に影響 / 要慎重）

## 新ステージ (残)

- ステージ②: 溶岩エリア (床ダメージ / 視界制限ギミック)
- ステージ③: 別ストーリー風ギミック

## 最重要方針

### キャラ素体・進化・融合の見た目は大幅変更予定

現状の `Partner_S*` / `Route_*` / `Fusion_*` / `Partner_S*_F*_L*` は、プレイ確認や図鑑確認用の暫定素材として扱う。  
今すぐ細かく直し続けるより、後で方向性を決め直して大幅に作り替える。

やり直し時の対象:

- 8素体: `Cobalt Pup`, `Ember Drake`, `Sage Hare`, `Hex Cat`, `Drift Fox`, `Iron Bear`, `Wraith Lynx`, `Genesis Core`
- 3進化ルート: `SPEED`, `POWER`, `GUARD`
- 各ルートの段階: `L0/L1/L2/L3`
- 6クロス進化: `Nova Aegis`, `Photon Siphon`, `Core Bastion`, `Nova Phantom`, `Aegis Drift`, `Photon Wraith`
- クロス進化の段階: `L0/L1/L2/L3`
- 最終的には idle / move / attack / hit のフレームアニメ

判断基準:

- 名前とシルエットが一致する。Foxが鳥に見える、Bearが猫に見える等はNG
- L0からL3でサイズ、シルエット、装備、発光が明確に強くなる
- SPEED / POWER / GUARD が遠目でも区別できる
- クロス進化は「リンク同士」ではなく「メインキャラがリンクを吸収した姿」に見える
- 商用前提のオリジナルIPとして成立する

## P0: 同時作業の運用

- Claude が `CoreLanternGame.cs` を触っている間、Codex は素材・台帳・レビュー画像・設計メモ中心に作業する
- `CoreLanternGame.cs` の編集は、片方が終わってから行う
- コード変更後は Roslyn compile check と Unity Editor のPlay確認を行う
- 変更したら `HANDOFF_FOR_CODEX.md` に追記する

## P1: プレイ確認・バグ修正

- ボスがプレイヤーを追う挙動の実機確認
- Wave 1から10まで通しプレイして、進行停止や二重選択がないか確認
- レベルアップ、進化、レリック、クロス進化、リザルトの遷移確認
- 低HP時に自分HP/コアHPが見えるか確認
- カード選択中に背景色や警告色が選択肢を邪魔しないか確認
- 図鑑、進化ツリー、ミッション、オプションがESCで自然に閉じるか確認

## P1: UI / HUD

- 生成済みHUD素材は小アイコンのみ使用。大型パネル/バー/背景画像は、画質と引き伸ばし問題が解決するまで使わない
  - `HUD_Panel_Wave.png`
  - `HUD_Panel_HP.png`
  - `HUD_Panel_Level.png`
  - `HUD_Panel_ChipMini.png`
  - `HUD_Bar_Back.png`
  - `HUD_Bar_HP_PlayerFill.png`
  - `HUD_Bar_HP_CoreFill.png`
  - `HUD_Bar_EXP_Fill.png`
  - `HUD_WaveProgress_Frame.png`
  - `HUD_WaveProgress_Fill_Normal.png`
  - `HUD_WaveProgress_Fill_Boss.png`
- ~~ミニマップ素材接続~~ → ミニマップ機能自体を撤去済 (2026-05-21)
- HUD詳細、データチップ、Active Build の表示タイミングを再整理する
- カード画面の情報階層をさらに整理する
- メインメニュー、図鑑、進化ツリー、リザルトを全面画面として磨く
- オプション設定を PlayerPrefs に保存する

## P1: ゲーム内容・ビルド幅

- Power以外のビルド魅力を増やす
- SPEED専用の移動・回避・貫通・連射ギミックを増やす
- GUARD専用のコア防衛・反射・回復・範囲制圧ギミックを増やす
- クロス進化はどれか一つを選ぶ重みを強める
- リンクが強すぎる場合の再ナーフ
- ボス後スペシャルモジュールの現在仕様を確認し、必要なら復活/整理する
- レリックの種類を増やす
- Waveごとの敵編成や予告をもっと個性的にする

## P1: 敵・ボス

- Elite Runner / Elite Brute / Elite Shooter を追加する
- Wave 10 Boss 第2形態を検討する
- ボスの攻撃パターンを増やす
- 敵ごとの見た目、移動、攻撃、死亡演出を差別化する
- 敵弾や危険範囲を見やすくする

## P1: グラフィック

- キャラ素体・進化・融合は大幅リデザイン予定として保留
- 現状素材はレビュー用に保持する
  - `AssetReview_PartnerRoutes.png`
  - `AssetReview_FusionStages.png`
  - `AssetReview_FusionL3.png`
- HUD素材は生成済みだが、大型画像は不採用。再利用するなら高解像度化/用途別比率固定/9-slice対応が必要
  - `Generated_HudAsset_Preview.png`
- マップ素材は一旦生成済みだが、画面密度・汚さ・視認性は実機で再確認する
- メインメニュー背景を作る
- 進化カットイン背景を作る
- クロス進化カットイン背景を作る
- クリア/敗北リザルト背景を作る
- 敵のアニメーションフレームを作る

## P2: 音

- 現在のプロシージャルBGM/SEは仮置き
- 商用利用可能なBGM/SEに差し替える
- 音量設定を保存する
- BGM、Shoot、Hit、Kill、Pickup、LevelUp、Evolve、Fusion、Boss、GameOverを整理する
- ループの違和感、音割れ、連射時のSE過密を確認する

## P2: メタ進行・長期目標

- クリア回数やミッションでパートナー/進化/レリックを解放する
- 図鑑に発見演出と報酬を追加する
- ラン後の成長要素を追加する
- 難易度追加: Normal以降の高難度
- Steam向けを意識したリザルト、実績、設定、タイトル導線を検討する

## P2: 技術整理

- `CoreLanternGame.cs` の分割は後回し。今はプロトタイプ速度を優先
- 分割するならユーザー確認後に行う
  - `CoreLanternGame.Sprites.cs`
  - `CoreLanternGame.UI.cs`
  - `CoreLanternGame.Upgrades.cs`
  - `CoreLanternGame.Audio.cs`
- ~~画像アセットの命名検証ツールを追加する~~ → `Tools/TestImageResources.ps1` 追加済み (2026-05-27)
- Unity import settings の確認
- ビルド設定、解像度、全画面、入力設定を整理する

## Codex向け安全タスク

Claude がコードを触っている間に進めやすいもの:

- `IMAGE_ASSET_BACKLOG.md` の更新
- `ASSET_REVIEW_REPORT.md` の更新
- レビューシート生成
- HUD/背景/カットインの画像生成
- 音素材候補リストの作成
- キャラ大幅リデザイン用の仕様書作成

## 次のおすすめ

1. Claude側の `CoreLanternGame.cs` 作業が落ち着くまで、Codexは「カットイン背景」「メニュー/リザルト背景」「キャラ大幅リデザイン仕様書」を進める
2. Claude側の作業が終わったら、HUD素材やStage 4/5サポート素材を必要な範囲だけコードに接続する
3. その後、実機プレイでUIとゲームバランスをまとめて確認する
