# CoreLanternUnity (Eggcore Protocol) リリース準備チェックリスト

作成: 2026-05-25
想定プラットフォーム: **Steam** (Windows 64-bit) / **itch.io** (任意)
想定言語: 日本語 + 英語 (将来)

凡例:
- 🤖 = **Codex に発注**できるタスク (素材生成系)
- 🛠 = **Claude に依頼**できるタスク (コード/ドキュメント)
- 👤 = **ユーザー本人**しかできないタスク (アカウント・契約・公開操作)
- ⏳ = 完了までの想定工数

---

## Phase 1: コンテンツ確定 (Content Lock) — 2-4 週間

販売前に絶対必要な「ゲーム本体の完成度」を上げる段階。

### 1-1. キャラクター素材確定 (最優先・最大ボリューム)

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🤖 **キャラ素体 11 体 × L0** | Codex | 大 | `docs/CHARACTER_IMAGE_PROMPTS.md` 既存。Cobalt Pup のみ L0 完成済 |
| 🤖 **進化ルート画像** (11 体 × 3 ルート × L1/L2/L3 = 99 枚) | Codex | 特大 | 段階投入推奨: まず L3 だけ全 33 枚 → 後で L1/L2 |
| 🤖 **クロス進化画像** (6 fusion × L0/L1/L2/L3 = 24 枚) | Codex | 大 | Fusion_1〜6 既存だが暫定 |
| 🛠 透過 PNG 化 & Resources/Skins/ 配置 | Claude | 小 | Codex 生成後の取り込み作業 |
| 👤 採用判断 (キャラデザの方向性 OK か) | ユーザー | - | デザイン承認 |

**Codex 発注時の注意:**
- 命名規則: `Partner_S{species}_L{level}.png` (素体) / `Partner_S{species}_R{route}_L{level}.png` (進化) / `Partner_S{species}_F{fusion}_L{level}.png` (クロス)
- サイズ: 512×512 透過 PNG (現状の Cobalt Pup と同じ)
- 背景: クロマキー → `remove_chroma_key.py` で透過化、または最初から透過
- スタイル: 既存 Cobalt Pup と統一感を保つこと

### 1-2. オーディオ確定

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🤖 **本番 BGM (5 曲程度)** | Codex / 外部 | 大 | 現状はビープ系プロシージャル。商用利用可能な素材必要 |
| - メインメニュー BGM | Codex / 外部 | 中 | 落ち着いた電子音楽 |
| - 通常 Wave BGM (Stage 1) | Codex / 外部 | 中 | 緊張感のあるドライブ感 |
| - Stage 2 (Lava) BGM | Codex / 外部 | 中 | 既存 BGM_Stage2.wav の差し替え |
| - Stage 3-5 BGM (各専用) | Codex / 外部 | 大 | 計 3 曲 |
| - ボス戦 BGM (1-2 曲) | Codex / 外部 | 中 | Pulswyrm / Nullwyrm 用 |
| - 勝利ジングル / 敗北ジングル | Codex / 外部 | 小 | 各 10-15 秒 |
| 🤖 **本番 SE 差し替え** | Codex / 外部 | 中 | 現 9 種を商用利用可能 SE に差し替え |
| 🛠 BGM/SE のループ調整 + 音量ミキシング | Claude | 小 | Unity import settings |
| 🛠 オーディオライセンス台帳更新 | Claude | 小 | `docs/AUDIO_LICENSE_LOG_TEMPLATE.md` 流用 |
| 👤 商用利用可能ライセンス確認 | ユーザー | - | 各素材のライセンス文書保管 |

### 1-3. UI / HUD 仕上げ

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🤖 **HUD 大型素材 9-slice 化** | Codex | 中 | HUD_Panel_*, HUD_Bar_* を高解像度化 + 9-slice 対応 |
| 🤖 メインメニュー背景の最終化 | Codex | 小 | 既存 Background_MainMenu の磨き込み |
| 🤖 リザルト背景 (Victory / Defeat) 最終化 | Codex | 小 | 既存 Background_Victory / Background_Defeat |
| 🤖 進化カットイン背景 | Codex | 小 | 既存 Background_Evolution の磨き込み |
| 🛠 9-slice の Unity import settings 設定 + 接続 | Claude | 中 | UseGeneratedHudBars / HudPanels フラグを true 化 |
| 🛠 タイトル画面のロゴ表示位置最終確認 | Claude | 小 | |
| 🤖 **ゲームロゴデザイン** | Codex / 外部 | 中 | "Eggcore Protocol" のロゴタイプ |

### 1-4. バランス・コンテンツ確定

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 👤 **実機 30 ラン以上の通しプレイ** | ユーザー | 大 | 各キャラ × 各ステージで通し |
| 🛠 Phase 2 / Pulswyrm enrage バランス調整 | Claude | 小 | playtest 結果ベース |
| 🛠 ステージごとの難易度カーブ調整 | Claude | 中 | Wave 1→10 で詰みすぎ/簡単すぎを是正 |
| 🛠 ボス HP 微調整 | Claude | 小 | 体感に合わせて |
| 🛠 モジュールバランス確認 (POWER 一強復帰してないか) | Claude | 小 | |
| 👤 「面白い瞬間」が毎ランあるか確認 | ユーザー | - | デザイン承認 |

### 1-5. ローカライゼーション (オプション、最初は日本語のみ可)

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🛠 全テキストを `Dictionary<string, Dictionary<string, string>>` に外出し | Claude | 大 | 後回し可能、初版は日本語のみ |
| 🤖 英語翻訳 (UI / モジュール名 / シナリオ) | Codex | 中 | 文字数: 約 5,000 字程度 |
| 👤 英訳のネイティブチェック | 外部 | - | 友人 or ローカライズサービス |

---

## Phase 2: 技術整理・品質保証 (QA) — 1-2 週間

### 2-1. ビルド設定

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🛠 Build Settings: Windows 64-bit only | Claude | 小 | Mac/Linux は後回し |
| 🛠 解像度対応 (1920×1080 / 2560×1440 / 3440×1440) | Claude | 中 | UGUI Stretch + Expand 既に対応 |
| 🛠 フルスクリーン / ウィンドウ切替 | Claude | 小 | F11 標準 |
| 🛠 セーブデータ保存先確認 (PlayerPrefs → AppData) | Claude | 小 | Unity デフォルト |
| 🛠 .exe アイコン設定 | Claude | 小 | Player Settings |
| 🤖 **.exe アイコン用 256×256 PNG** | Codex | 小 | ロゴから派生 |

### 2-2. パフォーマンス

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🛠 Profiler で Wave 10 ボス戦のフレーム計測 | Claude | 中 | 60 fps 維持確認 |
| 🛠 弾/敵/スパーク上限の最終調整 | Claude | 小 | EnforceBulletCap 等 |
| 🛠 メモリリーク確認 (10 ラン連続再戦) | Claude | 中 | RetryRun のコルーチン化済だが要検証 |

### 2-3. デバッグ・バグ修正

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 👤 バグレポート集約 | ユーザー | - | プレイテスター/自分でメモ |
| 🛠 致命バグ修正 | Claude | 中-大 | レポートに応じて |
| 🛠 警告 0 化 (Unity Console) | Claude | 小 | 既存未使用フィールド削除 |

### 2-4. アクセシビリティ

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🛠 Color blind 対応 (敵/弾色の差別化) | Claude | 中 | 後回し可 |
| 🛠 キーリマップ機能 | Claude | 中 | 後回し可 |
| 🛠 字幕速度オプション | Claude | 小 | ShowMessage の表示時間調整 |

---

## Phase 3: Steam ストアページ準備 — 1 週間

### 3-1. Steamworks アカウント・登録

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 👤 **Steamworks 開発者登録 ($100 / 1 タイトル)** | ユーザー | - | 必須課金 |
| 👤 税務書類提出 (W-8BEN 等) | ユーザー | - | 米国税法対応 |
| 👤 ストアページ作成 (申請 → 承認 1-2 週間) | ユーザー | - | Steam バックエンド |

### 3-2. Steam ストア用画像 (規定サイズ厳守)

| タスク | サイズ | 担当 | 備考 |
|---|---|---|---|
| 🤖 **Library Capsule** | 600×900 | Codex | 縦長メインビジュアル |
| 🤖 **Library Hero** | 3840×1240 | Codex | ストアトップ用 ワイド |
| 🤖 **Library Logo** | 1280×720 (透過 PNG) | Codex | ロゴだけ、背景透明 |
| 🤖 **Header Capsule** | 460×215 | Codex | ストア検索結果のサムネ |
| 🤖 **Small Capsule** | 462×174 | Codex | フレンドリスト等 |
| 🤖 **Main Capsule** | 920×430 | Codex | ストアフロント特集用 |
| 🤖 **Page Background** | 1438×810 | Codex | ストアページ全体背景 |
| 🤖 **アニメーション Capsule (任意推奨)** | 600×900 mp4/webm | Codex | gif アニメ可 |

**Codex 発注時の注意:**
- 全画像は `Assets/ArtSource/SteamCapsules/` に生成
- フォーマット: PNG (透過対応のもののみ alpha)
- カラープロファイル: sRGB
- ロゴは「Eggcore Protocol」と読める形に
- 既存ゲーム画面のキャラビジュアル (Cobalt Pup + Data Egg) を活用

### 3-3. スクリーンショット (最低 5 枚、推奨 8 枚)

| タスク | 担当 | 備考 |
|---|---|---|
| 👤 ゲーム内で印象的なシーンを撮影 | ユーザー | F12 or Print Screen、解像度 1920×1080 |
| 推奨シーン: | | |
| - Wave 5 中ボス Pulswyrm 戦 | | enrage 中の派手な瞬間 |
| - Wave 10 ラスボス Phase 2 | | Spiral Corruption 中 |
| - クロス進化カットイン | | 派手な瞬間 |
| - データラボ (ショップ) UI | | システムの厚みを伝える |
| - Stage 4 / 5 (氷 / 嵐) | | バラエティを示す |
| - メインメニュー | | 顔となる UI |

### 3-4. トレーラー (60-90 秒、Steam 標準)

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🤖 トレーラー用ゲーム映像の撮影 (Unity Recorder 推奨) | Codex / ユーザー | 中 | 720p/1080p、複数シーン |
| 🤖 編集 + テロップ + BGM 合成 | Codex / 外部 | 中 | 60-90 秒に圧縮 |
| 👤 YouTube アップロード + Steam 連携 | ユーザー | - | Steam 動画は YouTube/Vimeo 経由 |

### 3-5. ストアコピー (テキスト)

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🛠 **ゲーム紹介文** (短文 + 長文) | Claude | 小 | 日本語版 |
| 🛠 機能リスト (5-7 個の bullet) | Claude | 小 | |
| 🛠 ジャンルタグ選定 (Survivor / Roguelike / Tower Defense) | Claude | 小 | |
| 🛠 推奨スペック表 | Claude | 小 | 最低/推奨 2 段階 |
| 👤 英語版コピー (将来) | 外部 | - | |

---

## Phase 4: 法務・契約 — 1 週間

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 👤 **個人事業主開業届 / 法人化検討** | ユーザー | - | 収益化前提 |
| 👤 利用規約 (Terms of Service) 整備 | ユーザー / 外部 | 中 | テンプレ + カスタマイズ |
| 👤 プライバシーポリシー整備 | ユーザー / 外部 | 中 | PlayerPrefs しか使ってないなら最小限 |
| 🛠 ゲーム内クレジット画面追加 | Claude | 小 | "Developed by ..." / 使用素材ライセンス |
| 👤 知財チェック (キャラデザインの類似作品確認) | ユーザー | - | 後でトラブル回避 |
| 👤 価格決定 ($5 / $10 / $15 のいずれか想定) | ユーザー | - | 競合分析必要 |

---

## Phase 5: マーケティング準備 — 並行で進める

### 5-1. オンラインプレゼンス

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 👤 Twitter / X アカウント作成 (開発進捗ポスト) | ユーザー | - | 早めに開始推奨 |
| 👤 Discord サーバー (任意) | ユーザー | - | コミュニティ用 |
| 👤 開発ブログ / Note / Hatena Blog | ユーザー | - | SEO 効果 |
| 🤖 SNS 用 GIF/動画クリップ (10-30秒) | Codex | 中 | プレイ映像から切り出し |

### 5-2. プレスキット (任意推奨)

| タスク | 担当 | 工数 | 備考 |
|---|---|---|---|
| 🛠 presskit() フォーマットの press kit 作成 | Claude | 中 | dopresskit.com 形式 |
| 内容: | | | |
| - 開発者プロフィール | 👤 | | |
| - ゲーム概要 | 🛠 | | |
| - 高解像度スクリーンショット (3-5 枚) | 👤 + 🤖 | | |
| - GIF 動画 (3-5 個) | 🤖 | | |
| - ロゴ素材 | 🤖 | | |
| - 開発エピソード | 👤 | | |

### 5-3. 露出機会

| タスク | 担当 | 備考 |
|---|---|---|
| 👤 Steam Next Fest 申請 (期間限定デモ公開) | ユーザー | 露出効果大 |
| 👤 自作ゲーム展示会 (BitSummit / TGS インディー) | ユーザー | 任意 |
| 👤 ゲームメディアへのプレスリリース | ユーザー | 任意 |
| 👤 配信者向けキー配布 (Keymailer) | ユーザー | 任意 |

---

## Phase 6: ローンチ前最終チェック — リリース 1 週間前

| タスク | 担当 | 備考 |
|---|---|---|
| 🛠 最終ビルド作成 + 試遊 | Claude + 👤 | 致命バグなし確認 |
| 🛠 Steam Build 申請 (Build Tools) | Claude + 👤 | Steamworks 経由 |
| 🛠 実績 (Achievements) 実装 | Claude | 任意、後追加可 |
| 🛠 クラウドセーブ対応 | Claude | 任意、PlayerPrefs ベース |
| 👤 ストアページレビュー → 公開申請 | ユーザー | Valve 承認 1-2 週間 |
| 👤 発売日確定 + Twitter 告知 | ユーザー | |
| 👤 価格・地域別価格設定 (Regional Pricing) | ユーザー | Steamworks 設定 |
| 👤 アーリーアクセス か フル版 か決定 | ユーザー | |

---

## Phase 7: ローンチ当日 + ローンチ後

| タスク | 担当 | 備考 |
|---|---|---|
| 👤 SNS 告知 + Discord 開放 | ユーザー | |
| 👤 配信者対応 (招待コード) | ユーザー | |
| 👤 Steam フォーラム巡回 (バグ報告対応) | ユーザー | |
| 🛠 バグ修正パッチ (1.0.1, 1.0.2…) | Claude | レビュー反応見て |
| 🛠 バランス調整パッチ | Claude | プレイデータ収集後 |
| 🛠 新キャラ/新ステージ DLC 計画 (任意) | Claude + 👤 | 長期 |

---

## 🤖 Codex 発注リスト サマリー (まとめて投げる用)

以下を Codex に渡す想定。優先度順:

### P0 (販売に絶対必要)

1. **Steam ストア用画像 8 種** (`Assets/ArtSource/SteamCapsules/`)
   - Library Capsule 600×900
   - Library Hero 3840×1240
   - Library Logo 1280×720 透過
   - Header Capsule 460×215
   - Small Capsule 462×174
   - Main Capsule 920×430
   - Page Background 1438×810
   - アニメ Capsule 600×900 (mp4/webm/gif)

2. **ゲームロゴ** (`Assets/Resources/Skins/GameLogo.png`)
   - "Eggcore Protocol" のロゴタイプ
   - 透過 PNG 1024×512

3. **.exe アイコン** (`build/icon_256.png`)
   - 256×256 PNG + ico
   - ロゴから派生

### P1 (販売前に揃えたい)

4. **キャラ画像 全 132 枚** (`Assets/Resources/Skins/Partner_*.png`)
   - `docs/CHARACTER_IMAGE_PROMPTS.md` 参照
   - 段階投入: まず L3 全 33 枚 (11 体 × 3 ルート) → L0 11 枚 → L1/L2 残り → クロス進化 24 枚

5. **本番 BGM 7 曲** (`Assets/Resources/Audio/`)
   - メインメニュー (60秒ループ)
   - 通常 Wave (90秒ループ)
   - Stage 2/3/4/5 (各 90秒ループ)
   - ボス戦 (60秒ループ、高揚感)
   - 商用利用可能ライセンス必須

6. **本番 SE 9 種差し替え** (既存 Shoot/Hit/Kill/Pickup/LevelUp/Evolve/Fusion/Boss/GameOver)
   - 商用利用可能ライセンス必須

7. **HUD 大型素材 9-slice 化**
   - HUD_Panel_Wave / HP / Level / ChipMini (各 9-slice 高解像度 512×512)
   - HUD_Bar_Back / HP_PlayerFill / HP_CoreFill / EXP_Fill
   - HUD_WaveProgress_Frame / Fill_Normal / Fill_Boss

### P2 (販売後の追加要素)

8. **トレーラー素材**
   - ゲーム内録画 (1080p, 3-5 分の素材)
   - 編集・テロップ・BGM 合成 (60-90 秒に圧縮)

9. **SNS 用 GIF/動画クリップ** (`marketing/`)
   - 10-30 秒の派手なプレイ瞬間 ×5
   - Twitter/X 投稿用

10. **英語ローカライズ**
    - 全テキスト翻訳 (約 5,000 字)
    - ネイティブチェック必要

---

## 📋 工数感 (おおまかな目安)

| Phase | 期間 |
|---|---|
| Phase 1 (コンテンツ確定) | 2-4 週間 (Codex 並列で短縮可) |
| Phase 2 (QA) | 1-2 週間 |
| Phase 3 (Steam ページ) | 1 週間 + Valve 承認待ち 1-2 週間 |
| Phase 4 (法務) | 1 週間 |
| Phase 5 (マーケ) | 並行進行 |
| Phase 6 (ローンチ前最終) | 1 週間 |
| **合計** | **6-10 週間** (約 2-3 ヶ月) |

---

## 💰 想定コスト

| 項目 | 金額 |
|---|---|
| Steamworks 開発者登録 | $100 (約 ¥15,000) |
| BGM / SE 素材ライセンス | $0-$300 (Free 素材なら 0、Suno/Udio 課金なら $30/月) |
| 法務サービス (任意) | ¥10,000-¥50,000 |
| プレスキットサイト (任意、dopresskit.com) | 無料 |
| **合計最低限** | **約 ¥15,000-¥30,000** |

---

## 🎯 次の動き (秘書からの推奨)

1. **Phase 1-1 キャラ画像生成を Codex に発注開始** (最重量、並列処理で時短)
2. **Phase 1-2 商用 BGM 候補リサーチ** (Suno / Udio / フリー音源)
3. **Phase 3-1 Steamworks 登録** ($100 課金、要ユーザー判断)
4. **Phase 4 価格戦略決定** ($5/$10/$15 のどこを狙うか)

確認したい点:
- Steam 以外のプラットフォーム (itch.io / Switch / Mobile) も視野に入れるか
- アーリーアクセスでリリースするか、フル版で出すか
- 日本のみリリース か グローバル同時 か
- 開発者名・ブランド名 (個人名 or スタジオ名)
