# Codex 発注書: Embercore Studio リリース準備 (2026-05-25)

このドキュメントは Codex (素材生成エージェント) への発注リスト。
各セクションは独立して Codex に投げられる単位で整理してある。
1 ブロックずつコピペして Codex に渡せばよい。

---

## 📊 進捗ステータス (2026-05-31 更新)

| タスク | 状態 | 備考 |
|---|---|---|
| P0-1: スタジオロゴ + ゲームロゴ | 🟡 ゲームロゴドラフト生成済 / スタジオロゴ未 | `GameLogo*` 4枚生成済。`Embercore` は名称衝突注意のためスタジオ名確認待ち |
| P0-2: Steam ストア画像 8 種 | ⏳ 未着手 | 規定サイズ厳守 |
| P0-3: .exe アイコン | ✅ 完了 (2026-05-31 Codex) | `build/Icons/icon_*.png` + `EggcoreProtocol.ico` 生成済 |
| P1-1: キャラ画像 132 枚 | ⏳ Cobalt のみ完了 (4枚) | L1/L2 と他 10 体素体・進化・クロス進化未着手 |
| P1-2: 本番 BGM 7 曲 | 🟡 仮版生成済 | `BGM.wav` / `BGM_Stage2.wav` / `BGM_Boss_Pulswyrm.wav` / `BGM_Boss_Nullwyrm.wav` 等、商用素材差替は未 |
| P1-3: 本番 SE 9 種 | 🟡 仮版生成済 | procedural、商用素材差替は未 |
| P1-4: HUD 大型素材 9-slice | 🟡 一部接続済 (2026-06-01 Codex) | `HUD9_*` 11枚生成済。Wave進行バーのみ限定接続済、パネル/HP/EXPは目視承認後 |
| P0/P1 (新規): Stage/Boss enemy PNG ×7 | ✅ 完了 (2026-05-25 Codex) | LavaCrawler / MagmaTitan / CorruptionDrone / FrostKnight / VoltDasher / Boss_Pulswyrm / Boss_Nullwyrm |
| P1 (新規): Stage 4/5 支援素材 ×6 | ✅ 完了 (2026-05-25 Codex) | StageThumb_Frost/Storm, FrostCrystal, FrostPatch, LightningMarker, LightningStrike |
| P2-1: トレーラー素材 | ⏳ 未着手 | キャラ画像確定後 |
| P2-2: SNS 用クリップ | ⏳ 未着手 | トレーラー素材から派生 |
| P2-3: 英語ローカライズ | ⏳ 未着手 | 日本語版 EA リリース後 |
| P2-4: itch.io ページ画像 | 🟡 ドラフト生成済 (2026-05-31 Codex) | `marketing/ItchIo/` に header / cover / social card を生成。キャラ確定後に最終版へ |

凡例: ⏳ 未着手 / 🟡 仮版あり / ✅ 完了 / ❌ ブロック

---

## 🎯 プロジェクト基本情報 (全タスク共通の前提)

```
プロジェクト名: Eggcore Protocol
スタジオ名: Embercore Studio
ゲームジャンル: 防衛ローグライト サバイバー (TD + Vampire Survivors 系)
プラットフォーム: itch.io (デモ) → Steam (アーリーアクセス)
リリース目標: 2026年7-8月 (アーリーアクセス)
カラーパレット (ブランド共通):
  - 燻った赤/オレンジ: #C84A1F (ember 火種)
  - クールな青白: #8BC8FF (core 光)
  - 深い背景: #0B1418 (宇宙/深淵)
  - アクセントマゼンタ: #FF54F3 (ボス/危険)
  - 暖色イエロー: #FFCE3B (リワード/Elite)
作業前提:
  - Assets/Scripts/CoreLanternGame.cs は触らない (Claude が並行修正中)
  - 生成物は Assets/Resources/Skins/ または Assets/ArtSource/ に配置
  - 既存ファイル上書き時は Backup/ にバックアップ作成
  - 最終確認: Claude による Roslyn compile check 通過
```

---

# 🔴 P0: 販売絶対必要 (リリース 4 週前までに完成)

## P0-1: スタジオロゴ + ゲームロゴ

⚠ **商標衝突チェック (Codex 側で実行推奨)**:
発注前に Codex は以下を確認:
- "Embercore" でWeb検索 → 既存大手スタジオ・有名ブランドなし
- "Eggcore Protocol" でゲーム検索 → 同名ゲーム既存なし
- 衝突疑いあれば、生成前にユーザーへ報告して別案検討

```
タスク名: Embercore Studio ロゴ + Eggcore Protocol ロゴ 生成
優先度: P0
推定工数: 2-3 時間
出力先: Assets/Resources/Skins/

仕様:
1. ゲームロゴ (Eggcore Protocol)
   ファイル名: GameLogo.png
   サイズ: 1024×512 px (透過 PNG)
   内容: "EGGCORE PROTOCOL" タイトルロゴ
   スタイル: シャープな現代的サンセリフ (Eurostile / Orbitron 系)
   配色: 主体 #8BC8FF (青白)、輪郭 #C84A1F (燻った赤)
   オプション: 文字の隙間に小さい光粒/グリッチノイズ
   バリエーション: モノクロ版 GameLogo_Mono.png も生成

2. スタジオロゴ (Embercore Studio)
   ファイル名: StudioLogo_Embercore.png
   サイズ: 512×512 px (透過 PNG)
   内容: 中央に小さい炎マーク + それを囲む円
       下に "EMBERCORE STUDIO" 文字
   スタイル: ミニマルマーク + 細いセリフフォント
   配色: 炎 #C84A1F、円 #8BC8FF、文字 white
   バリエーション:
     - StudioLogo_Embercore_Horizontal.png (横長 1024×384、マーク+文字を横並び)
     - StudioLogo_Embercore_Mono.png (モノクロ)
     - StudioLogo_Embercore_64.png (64×64、ファビコン用)

参考: 既存 ProjectRoot/CoreLanternUnity/Assets/Resources/Skins/ に置く
受け入れ基準:
  - 透過 PNG、コーナー alpha=0
  - 商標衝突なし (Codex 側でも軽く検索チェック)
  - Unity Inspector で sprite import 可能
```

---

## P0-2: Steam ストアページ用画像 8 種 (規定サイズ厳守)

```
タスク名: Steam ストア用カプセル画像 一式
優先度: P0
推定工数: 1-2 日
出力先: Assets/ArtSource/SteamCapsules/

⚠ Steam の規定サイズは絶対厳守。1px でもズレると Steam 側拒否。

必須生成リスト:

1. Library Capsule (縦長メインビジュアル)
   ファイル: SteamCapsule_Library_600x900.png
   サイズ: 600×900 px (PNG, 不透明)
   内容: Cobalt Pup (主人公) + Data Egg (コア) + 派手な光線
       タイトルロゴ上部または下部
   重要: Steam のライブラリで縦長表示される最重要画像

2. Library Hero (ストアトップワイド)
   ファイル: SteamCapsule_Hero_3840x1240.png
   サイズ: 3840×1240 px (PNG, 不透明)
   内容: 横ワイドの世界観ビジュアル
       中央〜右にロゴ、左に主人公やボス
   注意: 中央部分が中心構図 (左右にバッファ空間)

3. Library Logo (ロゴ単独)
   ファイル: SteamCapsule_LibraryLogo_1280x720.png
   サイズ: 1280×720 px (透過 PNG)
   内容: "EGGCORE PROTOCOL" ロゴのみ、背景透明
   注意: Steam がライブラリで背景の上に重ねるので透過必須

4. Header Capsule (ストア検索サムネ)
   ファイル: SteamCapsule_Header_460x215.png
   サイズ: 460×215 px (PNG, 不透明)
   内容: 主人公 + ロゴ + 雰囲気を 1 枚で伝える縮小版
   重要: Steam 検索結果で一番見られる画像

5. Small Capsule (フレンドリスト用)
   ファイル: SteamCapsule_Small_462x174.png
   サイズ: 462×174 px (PNG, 不透明)
   内容: Header より小さく、ロゴ判読性最優先

6. Main Capsule (ストアフロント特集用)
   ファイル: SteamCapsule_Main_920x430.png
   サイズ: 920×430 px (PNG, 不透明)
   内容: Header の高解像度版、Steam が大特集で使う

7. Page Background (ストアページ背景)
   ファイル: SteamCapsule_PageBg_1438x810.png
   サイズ: 1438×810 px (PNG, 不透明)
   内容: 抽象的な世界観背景、テキストが乗っても読めるよう低コントラスト

8. アニメーション Capsule (任意推奨)
   ファイル: SteamCapsule_Animated_600x900.webm (and .mp4)
   サイズ: 600×900 px、6-10 秒ループ、≤ 5MB
   内容: Library Capsule の静止画 + 光や粒子のループアニメ

スタイル統一指針:
  - カラー基調: 深い宇宙背景 + ネオン光 (青白 + マゼンタ)
  - 文字は最小限 (タイトルロゴのみ、説明は Steam 側に書く)
  - 主人公: Cobalt Pup の青/シアン色を強調
  - 雰囲気: 「ローグライト × タワーディフェンス」が一目で伝わる

参考: 競合タイトル (Vampire Survivors, Brotato, 20 Minutes Till Dawn) の Steam 画像
受け入れ基準:
  - 各サイズ完全一致 (1px ズレ NG)
  - PNG 圧縮済 (Library Hero は 5MB 以内推奨)
  - 不透明指定のものに透過チャンネルなし
```

---

## P0-3: .exe アイコン

```
タスク名: Windows .exe アイコン生成
優先度: P0
推定工数: 30 分
出力先: build/Icons/

仕様:
1. アイコン PNG セット
   icon_16.png   (16×16)
   icon_32.png   (32×32)
   icon_48.png   (48×48)
   icon_64.png   (64×64)
   icon_128.png  (128×128)
   icon_256.png  (256×256)
   icon_512.png  (512×512)

2. .ico ファイル
   EggcoreProtocol.ico (上記サイズすべて含む統合 ico)

内容: スタジオロゴの炎+円マークの簡略版
   - 小サイズ (16-32) は炎だけ強調 (円は省略可)
   - 大サイズ (128-512) はマーク + 微細なディテール
配色: 燻った赤 #C84A1F + 青白アクセント #8BC8FF + 深い黒背景 (#0B1418)

受け入れ基準:
  - 各サイズで識別可能 (16×16 でも何のゲームか分かる)
  - Windows タスクバー・スタートメニューで違和感なし
```

---

# 🟡 P1: 販売前に揃えたい (リリース 2-4 週前までに完成)

## P1-1: キャラクター画像 132 枚 (段階投入)

```
タスク名: キャラクター画像 全 132 枚 段階生成
優先度: P1
推定工数: 大 (2-4 週間、段階投入推奨)
出力先: Assets/Resources/Skins/

⚠ 大量タスク。以下の順序で段階投入。各段階完了後に Claude に取り込み依頼。

段階 1 (最優先): 進化 L3 全 33 枚 ← まずこれ完了
段階 2: 素体 L0 全 11 枚
段階 3: 進化 L1, L2 全 66 枚
段階 4: クロス進化 24 枚 (6 fusion × L0/L1/L2/L3)

詳細仕様:
  参照: docs/CHARACTER_IMAGE_PROMPTS.md
  サイズ: 512×512 透過 PNG
  命名:
    素体:       Partner_S{species}_L0.png       (例: Partner_S1_L0.png)
    進化ルート: Partner_S{species}_R{route}_L{lv}.png (例: Partner_S1_R1_L3.png)
    クロス進化: Partner_S{species}_F{fusion}_L{lv}.png (例: Partner_S1_F1_L0.png)
  species: 1-11 (Cobalt Pup ~ Solar Anchor)
  route: 1=SPEED, 2=POWER, 3=GUARD
  level: 0, 1, 2, 3
  fusion: 1-6 (Nova Aegis ~ Photon Wraith)

スタイル統一指針:
  - 既存 Partner_S1_L0.png / R1_L3 / R2_L3 / R3_L3 (Cobalt Pup) と統一感
  - クロマキー背景 → remove_chroma_key.py で透過化、または最初から透過
  - 進化段階 L0→L3 でシルエット・装備・発光が明確に強くなる
  - SPEED / POWER / GUARD が遠目でも区別できる色分け
  - クロス進化は「メインキャラがリンクを吸収した姿」(リング+本体融合)

参考: docs/CHARACTER_IMAGE_PROMPTS.md, Assets/ArtSource/CharacterDrafts_Cobalt_20260524/
受け入れ基準:
  - 透過 PNG、四隅 alpha=0
  - 512×512 厳守
  - 名前と見た目が一致 (Fox が鳥に見える等 NG)
  - 既存 Cobalt Pup のクオリティ以上
```

---

## P1-2: 本番 BGM 7 曲 (商用利用可能ライセンス必須)

```
タスク名: 本番 BGM 全 7 曲生成 (procedural → 商用素材へ差替)
優先度: P1
推定工数: 大 (1-2 週間)
出力先: Assets/Resources/Audio/

⚠ 商用利用可能な素材必須。生成ツール (Suno / Udio / AIVA) の場合、商用ライセンス購入要。
   フリー素材なら Pixabay Music / Free Music Archive 等から選定。

差替対象:
1. BGM.wav (メインメニュー + 通常 Wave)
   既存: 32秒ループのプロシージャル
   要件: 60-90秒ループ、落ち着いた電子音、緊張感あるドライブ感
   雰囲気: 「夜の防衛任務」、サイバー、ローバー

2. BGM_Stage2.wav (Lava Cache)
   既存: 32秒ループ
   要件: 60-90秒ループ、熱気・低音ドラム強め
   雰囲気: 「溶岩洞窟、危険地帯」

3. BGM_Stage3.wav (Broken Core Network) ← 新規追加
   要件: 60-90秒ループ、グリッチ系・ステルス感
   雰囲気: 「腐敗したネットワーク、デジタルノイズ」

4. BGM_Stage4.wav (Frost Vault) ← 新規追加
   要件: 60-90秒ループ、冷ややか・静寂感
   雰囲気: 「氷の保管庫、青白い空気」

5. BGM_Stage5.wav (Storm Spire) ← 新規追加
   要件: 60-90秒ループ、雷鳴・電撃感
   雰囲気: 「嵐の尖塔、緊迫感」

6. BGM_Boss.wav (ボス戦共通) ← 新規追加
   要件: 60秒ループ、高揚感・終盤感
   雰囲気: 「ラスボス級の戦闘音楽、ドラム強め」

7. BGM_Victory.wav / BGM_Defeat.wav (10-15秒ジングル) ← 新規追加
   勝利: 達成感、軽快な上昇音
   敗北: 沈鬱、静かに終わる

選定基準:
  - 商用利用可能 (Steam 販売 OK)
  - クレジット表記不要 or 必要なら方針確認
  - 44.1kHz / 48kHz, ステレオ, 16-bit以上
  - ループ部分が自然 (フェードなし接続)
  - ファイルサイズ: 各 ≤ 5MB

ライセンス情報:
  - 各曲のライセンス文書を Assets/Resources/Audio/Licenses/ に保存
  - docs/AUDIO_LICENSE_LOG_TEMPLATE.md に追記

受け入れ基準:
  - ループ違和感なし (Unity の Loop モードで確認)
  - 音量バランス均等 (-14 LUFS 程度)
  - 商用利用可能ライセンス文書が揃っている
```

---

## P1-3: 本番 SE 9 種差し替え

```
タスク名: 本番 SE 9 種差し替え (procedural → 商用素材へ)
優先度: P1
推定工数: 中 (3-5 日)
出力先: Assets/Resources/Audio/

差替対象:
1. Shoot.wav      (プレイヤー射撃、0.05-0.1秒)
2. Hit.wav        (敵命中、0.1-0.2秒)
3. Kill.wav       (敵撃破、0.2-0.3秒)
4. Pickup.wav     (データ拾得、0.15-0.2秒)
5. LevelUp.wav    (レベルアップ、0.5-1.0秒、上昇音控えめ)
6. Evolve.wav     (進化、1.0-1.5秒、派手だがうるさくない)
7. Fusion.wav     (クロス進化、1.5-2.0秒、最も派手)
8. Boss.wav       (ボス出現/警告、0.5-1.0秒、低音強調)
9. GameOver.wav   (敗北、1.0-1.5秒、沈痛)

選定基準:
  - 商用利用可能 (Steam 販売 OK)
  - 同ジャンル (cyber / electronic / sci-fi) で統一
  - 音量レベル -6 dBFS 程度 (クリップしない範囲で大きめ)
  - 重複再生でも煩くならない短さ
  - ピッチが既存のプロシージャルと近い (急に違和感を出さない)

参考: 既存 Tools/GenerateStarterAudio.ps1 で生成されたピッチ/長さを参照
受け入れ基準:
  - Unity の AudioSource で再生可能
  - SfxCooldown (連続再生間隔) と整合性あり
  - 商用利用可能ライセンス文書付き
```

---

## P1-4: HUD 大型素材 9-slice 化

```
タスク名: HUD 大型素材を 9-slice 対応の高解像度版へ
優先度: P1
推定工数: 中 (3-5 日)
出力先: Assets/Resources/Skins/

⚠ Unity の 9-slice (Sprite Border) 機能を活用するため、四隅と中央を分離設計

差替対象:

1. HUD パネル背景 (4 種)
   ファイル: HUD_Panel_Wave.png, HUD_Panel_HP.png, HUD_Panel_Level.png, HUD_Panel_ChipMini.png
   サイズ: 512×512 px (透過 PNG)
   9-slice ボーダー: top=24, right=24, bottom=24, left=24
   内容: 角に微細な装飾 + 中央は単色 or 微弱グラデ
   配色: 暗い紺色 (#0B1418) + 縁にネオン青 #8BC8FF

2. HUD バー (HP / EXP) (4 種)
   ファイル: HUD_Bar_Back.png, HUD_Bar_HP_PlayerFill.png, HUD_Bar_HP_CoreFill.png, HUD_Bar_EXP_Fill.png
   サイズ: 256×64 px (透過 PNG)
   9-slice ボーダー: top=8, right=12, bottom=8, left=12
   内容: 横長バーの背景 / 塗り
   配色:
     - Back: 暗いグレー
     - HP Player: 赤系 (#FF3D5C)
     - HP Core: 黄系 (#FFCE3B)
     - EXP: 緑系 (#3DFF7A)

3. Wave 進捗バー (3 種)
   ファイル: HUD_WaveProgress_Frame.png, HUD_WaveProgress_Fill_Normal.png, HUD_WaveProgress_Fill_Boss.png
   サイズ: 512×48 px (透過 PNG)
   9-slice ボーダー: top=8, right=16, bottom=8, left=16
   内容: 細長バー、フレームと塗り分離
   配色:
     - Frame: 暗い紺 + 縁ネオン
     - Fill Normal: シアン
     - Fill Boss: マゼンタ

受け入れ基準:
  - 9-slice の境界線が滑らかにつながる (角張りなし)
  - 拡大時 (4倍) に縁がぼやけない
  - 既存ロジック側で UseGeneratedHudBars / UseGeneratedHudPanels フラグを true 化したとき
    Claude による Unity Inspector での確認で破綻なし
```

---

# 🟢 P2: 販売後 OK (リリース 1 ヶ月後までに完成)

## P2-1: トレーラー素材 (60-90 秒)

```
タスク名: Steam ストア用トレーラー (60-90 秒)
優先度: P2
推定工数: 大 (1-2 週間)
出力先: marketing/Trailer/

工程:
1. ゲーム内録画 (Unity Recorder 推奨、1080p/60fps)
   - 各シーン 5-15 秒、計 5-8 分の素材
   - 推奨シーン:
     a. メインメニュー → START 押下 (3-5秒)
     b. Wave 1-3 通常プレイ (10秒)
     c. レベルアップ + モジュール選択 (8秒)
     d. クロス進化カットイン (8秒)
     e. Wave 5 Pulswyrm enrage (10秒)
     f. Wave 10 Nullwyrm Phase 2 + Spiral Corruption (12秒)
     g. ボス撃破クライマックス (5秒)
     h. リザルト画面 (3秒)

2. 編集 (DaVinci Resolve / Premiere)
   - 60-90 秒に圧縮、テンポ良くカット
   - テロップ: 主要機能を 3-5 個 (例: "11 PARTNERS / 5 STAGES / EVOLVE OR EXTINCT")
   - BGM 合成: ゲーム内 BGM_Boss.wav が候補
   - エフェクト: 控えめなトランジション (フェード/カット中心)

3. 出力
   ファイル: Trailer_Main_1920x1080.mp4 (H.264, 60fps, ≤ 250MB)
        Trailer_Short_1080x1080.mp4 (Twitter 用、30秒、≤ 50MB)
        Trailer_Vertical_1080x1920.mp4 (TikTok/Reels 用、15秒)

受け入れ基準:
  - 1080p 60fps
  - 最初の 3 秒で「何のゲームか」が伝わる
  - Steam の動画フォーマット規定に準拠
```

---

## P2-2: SNS 用 GIF/動画クリップ

```
タスク名: SNS 投稿用クリップ ×5
優先度: P2
推定工数: 中 (3-5 日)
出力先: marketing/SocialClips/

仕様:
各クリップ 10-30 秒、横 1280×720 mp4 + gif 両方:
1. クロス進化の派手な瞬間 (10秒)
2. Spiral Corruption 弾幕回避 (15秒)
3. Elite 敵連続撃破 + STREAK 表示 (20秒)
4. Pulswyrm enrage 突入 + 突進 (15秒)
5. ラスボス撃破クライマックス (10秒)

形式:
  - mp4: H.264, 1280×720, 24-60fps, ≤ 8MB
  - gif: 同内容を gif 化、≤ 5MB (256 色)

受け入れ基準:
  - Twitter/X タイムラインで再生される
  - 音声なし or 控えめ (タイムラインは無音再生がデフォルト)
```

---

## P2-3: 英語ローカライズ

```
タスク名: 全テキスト英語翻訳
優先度: P2
推定工数: 中 (1 週間)
出力先: Assets/Resources/Localization/en.json (Claude が後で実装)

⚠ 翻訳作業のみ。Claude 側で多言語対応コードを後実装する想定。

翻訳対象:
1. メインメニュー (5-10 項目)
2. モジュール名 + 説明 (約 60 個 × 各 2-3 文)
3. キャラ名 + 紹介 (11 個 × 3 文)
4. レリック名 + 説明 (15 個 × 2 文)
5. Wave 開始メッセージ
6. リザルト UI
7. オプション項目
8. シナリオ的なフレーバーテキスト
合計: 約 5,000-7,000 文字

翻訳指針:
  - ゲーム業界用語は標準訳 (e.g., "弾数" → "Bullets", "連射" → "Fire Rate")
  - 固有名詞 (Cobalt Pup / Pulswyrm 等) はそのまま
  - 攻撃名 (突進 / Core Mark) は意訳可
  - 60 文字以内のラベルは英語でも 60 字以内に収める
  - ネイティブ感より「ゲームらしさ」優先

出力形式:
  ja.json / en.json の対構造
  例:
  {
    "module.dual_bit.title": "デュアルビット" / "Dual Bit",
    "module.dual_bit.desc": "弾を1発追加する。" / "Add 1 bullet."
  }

受け入れ基準:
  - 全テキスト網羅 (漏れなし)
  - 英語版でレイアウト崩れない (長すぎる訳に注意)
  - ネイティブチェックは後でユーザー側で実施
```

---

## P2-4: itch.io ページ用画像

```
タスク名: itch.io デモページ用画像
優先度: P0 ← itch.io デモ先行公開のため実は P0
推定工数: 1 日
出力先: marketing/ItchIo/

仕様:
1. ヘッダー画像
   ファイル: ItchIo_Header_630x500.png
   サイズ: 630×500 px (PNG, 不透明)
   内容: Steam Library Capsule と同方向性、itch.io 用にトリミング

2. カバー画像 (任意)
   ファイル: ItchIo_Cover_315x250.png
   サイズ: 315×250 px (PNG)

3. スクリーンショット 5-8 枚
   ファイル: ItchIo_Screenshot_01-08.png
   サイズ: 1920×1080 px (PNG)
   ユーザー側でゲーム内撮影 (Codex は加工のみ)

スタイル:
  - Steam ストア用と同じビジュアル方向性
  - 「DEMO」ラベルをヘッダーに追加 (任意)

受け入れ基準:
  - itch.io アップロード可能なサイズ
  - 「Embercore Studio」スタジオロゴ含む
```

---

# 📋 発注順序 (推奨スケジュール)

```
Week 1 (今すぐ):
  ★ P0-1: スタジオロゴ + ゲームロゴ (最優先、他タスクの基礎)
  ★ P0-3: .exe アイコン (ロゴから派生、すぐ完了可)
  ★ P2-4: itch.io ページ用画像 (デモ先行公開のため P0 扱い)

Week 2-3:
  ★ P0-2: Steam ストア用画像 8 種 (キャラ画像と並列)
  ★ P1-1: キャラ画像 段階 1 (進化 L3 全 33 枚) ← 最重量

Week 3-4:
  ★ P1-1: キャラ画像 段階 2 (素体 L0 全 11 枚)
  ★ P1-2: BGM 7 曲 (商用素材選定 + ライセンス確認)

Week 4-5:
  ★ P1-1: キャラ画像 段階 3 (進化 L1, L2)
  ★ P1-3: SE 9 種差替
  ★ P1-4: HUD 9-slice 素材

Week 5-6:
  ★ P1-1: キャラ画像 段階 4 (クロス進化 24 枚)
  ★ P2-1: トレーラー素材 (録画 + 編集)

Week 7-8 (リリース直前):
  ★ P2-2: SNS クリップ
  ★ P2-3: 英語ローカライズ (将来用)
```

---

# 🔄 進捗管理

各タスク完了時:
1. Codex 側でプレビューを `Assets/ArtSource/{TaskName}_Preview.png` に出力
2. Codex 側で `REMAINING_TASKS.md` の「Recent Done」セクションに完了記載
3. Claude が次回起動時にプレビュー確認 + Resources/Skins/ への配置
4. ユーザー判断 → 採用 or リテイク依頼

問題があれば:
- Codex への再依頼時は本ドキュメントの該当セクションをそのまま引用 + 修正点追記
- 仕様変更時は本ドキュメント側を先に更新

---

最終更新: 2026-05-25
担当: Claude (秘書)
発注先: Codex
承認: ユーザー
