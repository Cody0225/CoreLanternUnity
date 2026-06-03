# itch.io デモ版公開手順書

**目的**: Steam EA リリース前に itch.io で先行デモ版を公開し、コミュニティからフィードバックを集める。
**想定タイムライン**: EA リリースの 2-4 週前
**所要時間**: 初回 2-4 時間 (アカウント作成・ページ作成含む)

---

## 0. なぜ itch.io デモから始めるか

| 観点 | 内容 |
|---|---|
| **登録費用** | 無料 (Steam は $100) |
| **公開速度** | 即時 (Steam は Valve 審査 1-2 週) |
| **フィードバック** | コアインディーゲーマー層から良質な意見 |
| **マーケ素材検証** | 本番 Steam 前にキャプセル画像・トレーラーの反応を見れる |
| **コミュニティ形成** | Steam ストアページ公開と同時に「フォロワー」を確保できる |

---

## Phase 1: 事前準備 (ユーザー作業、1-2 時間)

### 1-1. itch.io アカウント作成
1. https://itch.io/register でメール + パスワード登録
2. ログイン後、右上メニューから **「Dashboard」** に移動
3. プロフィール画像・カバー画像・自己紹介 (英日両方推奨) を設定

### 1-2. クリエイター情報入力
- **Display name**: `Embercore Studio`
- **URL**: `embercore` または `embercorestudio` (短い方推奨)
  → `https://embercore.itch.io` がプロフィール URL になる
- **About**: 「Solo dev creating defense-roguelike survivors. Currently working on **Eggcore Protocol**」(英語推奨)

### 1-3. 支払い設定 (有料公開する場合)
デモ版を無料公開なら不要。将来有料化する時:
- **Account → Edit account → Set up payments**
- PayPal または Stripe を連携 (タックスフォーム提出が必要)

---

## Phase 2: ビルド作成 (Claude 補助 + Unity 操作、30 分)

### 2-1. Unity の Build Settings

Unity Editor で:
1. **File → Build Settings**
2. **Platform**: PC, Mac & Linux Standalone を選択
3. **Target Platform**: Windows
4. **Architecture**: x86_64 (64-bit)
5. **Compression Method**: LZ4HC (圧縮率と読込速度のバランス)

### 2-2. Player Settings

1. **Player Settings → Other Settings**
2. **Product Name**: `Eggcore Protocol`
3. **Company Name**: `Embercore Studio`
4. **Version**: `0.9.0-demo`
5. **Bundle Identifier**: `studio.embercore.eggcoreprotocol`
6. **Default Screen Width**: 1920
7. **Default Screen Height**: 1080
8. **Fullscreen Mode**: `Fullscreen Window` (Windowed 切替可)
9. **Resizable Window**: ON
10. **Default Icon**: P0-3 で Codex が作る `icon_256.png` を設定 (現状は仮)
11. **Splash Image**: Unity デフォルトを使うか、シンプルな黒画面ロゴ表示

### 2-3. ビルド実行

1. **Build Settings → Build**
2. 出力先: `Builds/Demo_v0.9.0/`
3. 出力ファイル例:
   ```
   Builds/Demo_v0.9.0/
   ├── EggcoreProtocol.exe        (実行ファイル)
   ├── UnityCrashHandler64.exe
   ├── UnityPlayer.dll
   ├── MonoBleedingEdge/
   ├── EggcoreProtocol_Data/
   │   ├── Managed/
   │   ├── Resources/
   │   ├── StreamingAssets/
   │   └── ...
   └── ...
   ```

### 2-4. 動作確認 (ビルド前必須)

1. `EggcoreProtocol.exe` をダブルクリックで起動
2. メインメニュー表示確認
3. **1 ラン通しプレイ** (Wave 1→10)
4. 問題なければ Phase 3 へ進む

問題があれば即座に修正 (Claude に貼り付け)。

### 2-5. zip 圧縮

1. `Builds/Demo_v0.9.0/` フォルダ全体を **zip 圧縮**
2. ファイル名: `EggcoreProtocol_Demo_v0.9.0_Windows.zip`
3. サイズ目安: 50-150 MB (Unity の標準ビルド)
4. 1 GB 超なら itch.io のアップロード上限要確認 (通常版 1GB / Pro 版で拡張可)

---

## Phase 3: itch.io ページ作成 (ユーザー作業、30 分)

### 3-1. 新規プロジェクト作成

1. ログイン状態で右上 **「Create new project」** をクリック
2. または直接 https://itch.io/game/new

### 3-2. 必須入力項目

| 項目 | 入力例 |
|---|---|
| **Title** | `Eggcore Protocol — Demo` |
| **Project URL** | `eggcore-protocol-demo` (英数字小文字、ハイフン可) |
| **Short description** | `Defend the core. Survive 10 waves. A defense-roguelike survivor by Embercore Studio.` (日本語版あれば併記) |
| **Classification** | Games |
| **Kind of project** | Downloadable |
| **Release status** | Prototype / In development |
| **Pricing** | `No payments` (無料デモ) または `$0 or donate` (任意投げ銭) |

### 3-3. アップロード

1. **Uploads** セクション
2. zip ファイルをドラッグ&ドロップ
3. **Windows** プラットフォームバッジを ON
4. **Display name**: `Windows Demo v0.9.0`
5. **Set as primary file**: ON

### 3-4. ストア画像

| 項目 | サイズ | ファイル |
|---|---|---|
| **Cover image** | 630×500 推奨 (最小 315×250) | `ItchIo_Cover.png` (Codex 待ち) |
| **Screenshots** | 1920×1080 推奨 (5-8 枚) | ゲーム内で F12 撮影 |
| **Trailer** | YouTube 等の動画 URL | (任意、Codex 後) |

### 3-5. 詳細説明文 (Body)

Markdown 対応。`docs/STEAM_STORE_COPY.md` の「詳細ディスクリプション」を流用 + itch.io 用にカスタマイズ。

例:
```markdown
# Eggcore Protocol

中央のコア「データエッグ」を守りながら 10 Wave を生き残る、
防衛 × サバイバー × ローグライト の融合作品。

## 主な機能
- 11 種の相棒キャラクター
- 3 ルート進化 + 6 種のクロス進化
- 5 ステージ × 専用敵
- ボス Phase 2 + 中ボス Enrage
- 60+ モジュール / 15+ レリック

## デモ版について
- 全 5 ステージ・全 11 相棒プレイ可能
- アーリーアクセス前の試遊版
- フィードバック歓迎 (コメント欄またはメール)

## 開発者
Embercore Studio (個人開発)

## 操作方法
- WASD / 矢印キー: 移動
- 自動射撃 (狙いは自動)
- 1/2/3 キー: モジュール選択
- Q/W/E: モジュールロック (リロール時保持)
- Z/X/C: モジュール除外
- ESC: 一時停止 / メニュー
- R: リザルト画面で同設定再戦

## システム要件
最低: Windows 10 64-bit / Intel HD Graphics 4000 / 4GB RAM
推奨: Windows 10/11 / GTX 1050+ / 8GB RAM

---
© 2026 Embercore Studio
```

### 3-6. メタデータ (タグ)

5-10 個選択 (検索ヒット率向上):
- `roguelike`
- `roguelite`
- `survivors-like`
- `tower-defense`
- `action`
- `top-down-shooter`
- `bullet-hell`
- `indie`
- `pixel-art` (キャラ画像のスタイル次第)
- `singleplayer`

### 3-7. コミュニティ機能

- **Community**: Comments を ON (フィードバック収集)
- **Pricing**: free or pay-what-you-want
- **Visibility**: 最初は `Draft` で内部確認 → OK なら `Public`

---

## Phase 4: 公開と告知 (ユーザー作業、即時〜数日)

### 4-1. 公開

1. プロジェクトページ右上 **「Edit game」 → Save**
2. プレビューで確認
3. **Visibility: Public** に変更 → 即時公開

### 4-2. SNS 告知

**Twitter / X**:
```
🎮 Demo released!

"Eggcore Protocol" — A defense-roguelike survivor where you protect the core.

- 11 partners × 3 routes × 6 cross-evolutions
- 5 stages × Boss Phase 2
- 60+ modules

Play free demo: https://embercore.itch.io/eggcore-protocol-demo

#indiegame #roguelike #survivors
```

**Note 等のブログ記事** (日本語):
```
タワーディフェンス × ヴァンサバ系の新作デモを公開しました。

防衛と進化と移動のバランスを楽しむ防衛ローグライト。
1 ラン 15-20 分で気軽に遊べます。

リンク: https://embercore.itch.io/eggcore-protocol-demo
```

### 4-3. コミュニティ巡回

itch.io には game jam / dev community があります。デモ公開後:
- https://itch.io/community でフィードバック投稿
- 関連タグの「Recently updated」スレッドに紹介

---

## Phase 5: フィードバック収集と反映 (継続)

### 5-1. コメント・メール対応

- itch.io のコメント欄を定期的に確認 (最低週 2 回)
- バグ報告は `REMAINING_TASKS.md` に追記
- 改善要望は `BACKLOG_FROM_DEMO_FEEDBACK.md` (要新規作成) に蓄積

### 5-2. アップデート公開

新しいビルドを上げる場合:
1. Unity で再ビルド (バージョン番号を上げる: v0.9.1 → v0.9.2)
2. zip を新規アップロード
3. **古いビルドは削除しない**: 自動的に "Latest version" が表示される
4. **Devlog** に変更内容を記載 (リリースノート)

---

## Phase 6: Steam EA 移行準備

itch.io デモで一定のフィードバックが集まったら:

1. デモプレイヤー数 / DL 数を `RELEASE_CHECKLIST.md` に記録
2. 主要バグ・バランス問題を反映した「製品版相当」のビルドを作成
3. Steam EA リリース手順 (`docs/RELEASE_CHECKLIST.md` の Phase 3 以降) に進む

**itch.io 版の扱い**:
- A. Steam EA 後も itch.io 版を継続公開 (デモとして残す)
- B. Steam EA リリース時に itch.io 版は撤去 / 「Steam に移行しました」と告知
- 推奨: **A**。itch.io は別の顧客層が居るので両方公開しておくのが良い

---

## 🎯 itch.io デモ公開の成功基準

最初の 2 週間で達成したい:
- [ ] **DL 数 100+** (告知のリーチ確認)
- [ ] **コメント 5+** (フィードバック獲得)
- [ ] **致命バグ報告 0**
- [ ] **アップデート 1 回**

達成できない場合:
- DL 数低い → 告知拡散 (X 広告 / Reddit / Discord 等)
- コメントゼロ → SNS でプレイヤーに直接フィードバック依頼
- 致命バグ → 即座にホットフィックス

---

## ⚠ 注意事項

1. **個人情報**: itch.io アカウントは個人名でも OK。スタジオ運営したい場合は法人化検討も。
2. **税務**: 収益が出たら確定申告対応必要。デモ無料なら影響なし。
3. **画像権利**: アップロードする画像は全て自分または Codex 生成のもの (商用利用可) のみ。Steam 用と同じガイドライン。
4. **EULA**: 利用規約 / プライバシーポリシーは itch.io のデフォルトで十分 (個人情報を収集していなければ)。

---

## 🚀 当面の進め方 (秘書からの推奨)

1. **本日 / 明日**: ユーザーが itch.io アカウント作成 (5 分)
2. **今週中**: Codex に P0-1 (ロゴ) + P0-3 (アイコン) + P2-4 (itch.io 画像) を発注
3. **2-3 週間後**: ロゴ・アイコンが揃ったら Phase 2 (Unity ビルド) 着手
4. **3-4 週間後**: itch.io 公開 → フィードバック収集開始
5. **その後 4-6 週間後**: Steam EA リリース

最終更新: 2026-05-28
担当: Claude (秘書)
対象: ユーザー (実行) + Codex (素材生成)
