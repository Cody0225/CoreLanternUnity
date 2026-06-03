# Claude Task Progress

最終更新: 2026-05-24  
目的: Codex側の指示書・台帳・完了記録を元に、Claudeへ渡す実装進捗表を1枚にまとめる。

## 共有ルール

- `Assets/Scripts/CoreLanternGame.cs` はClaude側の主担当。Codexは同時作業中に触らない。
- 画像素材・台帳・設計MarkdownはCodex側の主担当。
- 大型UI/背景画像を有効化する場合は、9-sliceまたはアスペクト比維持を確認してから段階投入する。
- キャラ素体・進化・融合画像は作業量が非常に大きいため、最終フェーズまで生成/差し替えしない。現状PNGは暫定確認用として扱う。
- 商用前提なので、既存IPに寄せすぎた名前・見た目・音源は避ける。

## 優先度別タスク表

| 優先 | タスク | 現状 | Claude側の次アクション | 参照 | 競合/注意 |
|---|---|---|---|---|---|
| P0 | Stage 2 溶岩エリア実装 | レビュー分析反映済み | `currentStageId` / hazard zone / lava damage / smoke vision / enemy hazard behavior を実装 | `docs/STAGE_DESIGN_SPEC.md` | 溶岩は罰だけでなく危険報酬。局所アクセント、低ダメージ、明確な安全レーンを優先 |
| P0 | Stage 3 別ストーリー風ギミック | レビュー分析反映済み、推奨案は Broken Core Network | relay device 3基、安定化、敵の腐食、報酬処理を実装 | `docs/STAGE_DESIGN_SPEC.md` | Relayは任意の有利目標。無視してもクリア可能、HUD常駐パネルは増やさない |
| P0 | Nullwyrm 第2形態 | レビュー分析反映済み | HP50%以下でPhase2、Core Mark主役、Spiral/Summon/transition pulseを実装 | `docs/BOSS_PHASE2_SPEC.md` | 硬いだけにしない。まず `bossKind` / `isMidBoss` を明示。HP閾値でNullwyrm/Pulswyrm判定しない |
| P0 | Pulswyrm ミニエンレージ | 方針決定済み | Full Phase2ではなく50%で軽い速度/弾数強化だけ入れる | `docs/BOSS_PHASE2_SPEC.md` | Wave5を長くしすぎない |
| P0 | 背景画像 Stage1 再確認 | Codexで低密度版を有効化済み | Unity Playでメニュー/リザルト/進化/ボスカットインを目視QA | `HUD_UI_ASSET_REQUEST.md`, `ASSET_REVIEW_REPORT.md` | すべて比率維持。UI裏で主張が強ければ `UseGeneratedBackgrounds` / `UseGeneratedCutscenes` をOFF |
| P0 | 大型UI画像の段階投入 | 背景/カットインのみON、HUD/カード/警告帯はOFF | 次はHUDバーだけ単独で試すか、コード生成UIを維持するか判断 | `ASSET_REVIEW_REPORT.md` | 一括ON禁止。暗転板/危険vignette/警告帯は `UseGeneratedOverlayImages=false` のまま |
| P1 | Pulse Hydra 実装QA | Claude側で実装済み | 実機でDPS、射程外挙動、処理負荷、視覚ノイズを確認 | `REMAINING_TASKS.md` | PNGセット未整備。現状はコード表現/暫定見た目 |
| P1 | 難易度緩和後の全キャラQA | Codexで敵圧/育成速度を緩和済み | 弱めの素体(Cobalt/Ember/Hex/Drift/Iron)でWave7以降とPulswyrmを確認 | `REMAINING_TASKS.md` | Wraith/Pulse/Genesisが簡単になりすぎる可能性あり。必要なら強キャラ側を微ナーフ |
| P1 | シグネチャ強化モジュール10種QA | Claude側で実装済み | 取得条件、Lv表示、火力過多、Wraith/Pulse/Halo偏重を確認 | `REMAINING_TASKS.md` | build幅が狭くならないよう数値は要調整 |
| P1 | Halo Caster / Pulse Hydra 図鑑・進化ツリー・画像台帳追従 | 台帳上は正式PNG不足 | 図鑑/進化ツリーに名前・説明・ロック状態だけ合わせる | `IMAGE_ASSET_BACKLOG.md`, `docs/CHARACTER_REDESIGN_SPEC.md` | PNG最終化は最終フェーズまで触らない |
| FINAL | キャラ大幅リデザイン | ドラフト仕様書完成 | 今は実装しない。ゲーム機能/ステージ/ボス/UIが固まった後、最後に画像生成/差し替えへ | `docs/CHARACTER_REDESIGN_SPEC.md` | 既存10体目Pulse Hydraの正式採用もこの最終フェーズで確認 |
| P1 | 音素材導入 | 候補リスト + AI生成方針 + プロンプト + ライセンス台帳テンプレ + スターター音源自動生成スクリプト完成。通常BGM/Stage2 BGMは上昇感を抑えた24秒ループへ再生成済み | 実機で新BGM/Stage2 BGMの疲れにくさを確認。足りない音だけP0音源を生成/取得し、ライセンス証跡を同梱して差し替え | `docs/SOUND_ASSET_CANDIDATES.md`, `docs/AUDIO_ASSET_PLAN.md`, `docs/AUDIO_GENERATION_PROMPTS.md`, `docs/AUDIO_LICENSE_LOG_TEMPLATE.md`, `docs/AUDIO_AUTOMATION.md` | Freesound/Pixabayは個別ライセンス保存必須。Stable AudioはCommunity License条件、Eleven Musicはplan/game rightsを確認。Suno Freeは使わない |
| P1 | Pickup_Data 64x64問題 | 現状維持判断 | UI小アイコン用途が必要なら別名/別素材で64x64を作る | `IMAGE_ASSET_BACKLOG.md` | 既存116x112を即上書きしない |
| P2 | 画像台帳/レビュー更新 | Codex側で更新済み | 実装後に使った/無効化した素材ステータスだけ追記 | `IMAGE_ASSET_BACKLOG.md`, `ASSET_REVIEW_REPORT.md` | 同時編集時は先に差分確認 |
| P2 | 残タスク更新 | 冒頭直近完了に追記済み | Claude作業完了ごとに `REMAINING_TASKS.md` 冒頭へ1行追記 | `REMAINING_TASKS.md` | 長文化しすぎたら別セクションへ整理 |

## 完了済みタスク

| タスク | 完了内容 | 成果物 |
|---|---|---|
| HUD/UI素材指示書作成 | 非キャラ素材の命名、サイズ、9-sliceルールを整理 | `HUD_UI_ASSET_REQUEST.md` |
| 非キャラ素材生成 | Card/HUD/Background/Bullet/Effect/Icon/Floor系を生成 | `Tools/GeneratePolishedNonCharacterAssets.ps1`, `Assets/Resources/Skins/` |
| 背景画像低密度再生成 | 全面グリッド、赤縦縞、中央リング、斜め帯を削除 | `Background_*.png`, `MainMenu_Background.png`, `Cutin_*`, `Result_*` |
| 背景/カットイン画像配置 | メニュー/結果/進化/クロス進化/ボス背景を比率維持で有効化。HUD/カード大型画像は継続OFF | `Assets/Scripts/CoreLanternGame.cs` |
| 難易度緩和 | 初期耐久/育成速度UP、Wave敵数/HP/速度/被ダメ低下、Pulswyrm中ボス判定修正 | `Assets/Scripts/CoreLanternGame.cs` |
| Stage2選択/レリック再調整 | Stage2を古い/新規セーブでも選択可能にし、レリック性能差を大幅圧縮 | `Assets/Scripts/CoreLanternGame.cs` |
| Stage2マップデザイン差別化 | Stage2専用の熱ウォッシュ/焼け床/導熱ライン/外周マーカー/溶岩テクスチャを追加 | `Assets/Scripts/CoreLanternGame.cs` |
| リザルト復帰再修正 | リザルト/ポーズ復帰で `LoadScene` を使わず、生成済みルート破棄→次フレーム再生成へ変更 | `Assets/Scripts/CoreLanternGame.cs` |
| リザルト復帰後の音消え修正 | 生成型リロード後に自前生成したMain CameraへAudioListenerを付与し、メニュー復帰後の再スタートでもBGM/SEが聞こえるように修正 | `Assets/Scripts/CoreLanternGame.cs` |
| 全体デバッグ/ジャンル差分反映 | メニュー/レリック/リザルト復帰の導線バグを修正し、通常強化にスキップ報酬と候補枯渇時の緊急補給を追加。Stage2溶岩縁にリスク報酬データも配置。ジャンル差分分析も作成 | `Assets/Scripts/CoreLanternGame.cs`, `docs/DEBUG_AND_GENRE_GAP_ANALYSIS_20260522.md` |
| 相棒選択UI修正 | BEAST LINK画面を3列スクロール化し、戻るボタン/Esc復帰を追加。カード内の発光円・文字・ステータスバーの重なりを調整 | `Assets/Scripts/CoreLanternGame.cs` |
| 非キャラ画像 Batch 2 | 追加レリック4種、モジュールアイコン20種、Stage2/Stage3小物、ステージサムネ、Pickup_Data_64を生成。追加レリックと強化カードアイコンはコード接続済み | `Tools/GenerateNonCharacterPolishBatch2.ps1`, `Assets/Resources/Skins/`, `Assets/Scripts/CoreLanternGame.cs` |
| レビュー分析反映 | 同ジャンルレビュー傾向からStage/Boss仕様を再調整。危険報酬、任意目標、視認性、Core Mark主役、控えめなPhase2数値を追加 | `docs/STAGE_DESIGN_SPEC.md`, `docs/BOSS_PHASE2_SPEC.md` |
| Stage設計書 | Stage2溶岩、Stage3候補と推奨案を文書化 | `docs/STAGE_DESIGN_SPEC.md` |
| Boss Phase2設計書 | Nullwyrm Phase2とPulswyrm mini-enrageを文書化 | `docs/BOSS_PHASE2_SPEC.md` |
| Character Redesign草案 | 8素体 + Halo Caster + Pulse Hydra provisional を整理 | `docs/CHARACTER_REDESIGN_SPEC.md` |
| 音源候補リスト | 商用利用前提の候補とライセンス確認リンクを整理 | `docs/SOUND_ASSET_CANDIDATES.md` |
| 音源制作方針/生成プロンプト | 無料/商用可優先のAI音源方針、P0/P1音源一覧、生成プロンプト、ライセンス台帳テンプレを追加 | `docs/AUDIO_ASSET_PLAN.md`, `docs/AUDIO_GENERATION_PROMPTS.md`, `docs/AUDIO_LICENSE_LOG_TEMPLATE.md` |
| 音源自動生成 | 第三者素材なしのスターターWAVをUnity読込名で自動生成し、manifest/licenseも作るスクリプトを追加 | `Tools/GenerateStarterAudio.ps1`, `docs/AUDIO_AUTOMATION.md` |
| BGM上昇感の抑制 | BGMの高い上昇リードを廃止し、低め・少なめ・下降気味の32秒ループへ再生成 | `Tools/GenerateStarterAudio.ps1`, `Assets/Resources/Audio/BGM.wav` |
| SE多段ヒット抑制 | `Hit` / `Shoot` / `Kill` / `Pickup` などに再生間隔と音量係数を追加し、連続発音の濁りを軽減 | `Assets/Scripts/CoreLanternGame.cs` |
| 強化カード押せない問題 | Data Lab/ショップで無効化された共通カードを通常強化/レリック/進化表示時に再有効化。スキップも表示時に有効化 | `Assets/Scripts/CoreLanternGame.cs` |
| 体験面ブラッシュアップ | OptionsにBGM/SE音量スライダー、リザルトにRUN RANK/NEXT目標、強化カードにBASIC/RARE/EPIC併記と説明文はみ出し抑制を追加 | `Assets/Scripts/CoreLanternGame.cs` |
| 最終ボス/リザルト/Options/BGM修正 | ボス多段ヒット時のエフェクト負荷を間引き、リザルトを2行サマリ中心へ整理。Optionsからゲーム中難易度変更項目を外し、通常/Stage2 BGMと上昇系SEを再生成。Stage2専用敵LavaCrawlerも追加 | `Assets/Scripts/CoreLanternGame.cs`, `Tools/GenerateStarterAudio.ps1`, `Assets/Resources/Audio/BGM_Stage2.wav` |
| Pickup_Data判断 | 116x112は現状維持、64x64は別途必要時作成 | `IMAGE_ASSET_BACKLOG.md` |

## Claudeのおすすめ着手順

1. まず現行 `CoreLanternGame.cs` のコンパイルとPlay確認。
2. 低密度背景のStage1有効化を1回だけ試す。ダメならフラグOFFに戻す。
3. `BOSS_PHASE2_SPEC.md` の前提として `bossKind` / `isMidBoss` を追加し、HP閾値判定を避ける。
4. Nullwyrm Phase2を最小構成で入れる: transition visual、7way shot、Core Markの順。
5. Stage2はハザード配置とプレイヤーダメージだけ先に入れ、敵AI変化とボス変化は後続。
6. Stage3はrelay deviceの見た目/当たり/安定化だけ先に入れ、報酬と敵腐食は後続。
7. Pulse Hydra / Halo Caster / Wraith Lynx系の数値QAを行い、専用ビルドが強すぎないか調整。
8. キャラ素体・進化後・クロス進化の正式画像生成は、上記のゲーム機能とUI方針が固まった後の最後に回す。

## Codex側に戻すとよいタスク

| タスク | 理由 |
|---|---|
| 新キャラ正式PNG生成 | 作業量が大きいため最終フェーズ。ゲーム機能/UIが固まるまでは触らない |
| HUD/背景の再生成 | 見た目の調整はCodex側でスクリプト/台帳を保つ方が安全 |
| 音源の候補追加 | ライセンス調査と候補整理はコード実装と分離できる |
| レビューシート再生成 | `Assets/ArtSource` 周辺の確認資料は素材台帳とセットで管理する |

## 要ユーザー確認

- Pulse Hydraを正式10体目として採用するか。
- キャラ正式リデザイン最終フェーズで、10体制にするか9体制に戻すか。
- Stage3は推奨案 `Broken Core Network` で進めるか。
- Nullwyrm Phase2のCore Markを「プレイヤーが射線に入って防ぐ」仕様にするか。
- 大型UI画像を最終的に使うか、コード生成UI + 小アイコン中心で進めるか。
