# Steam ビルド / Steamworks 連携 ガイド

**目的**: itch.io デモ版でフィードバックを集めた後、Steam アーリーアクセス (EA) リリースに進むための実務手順。
**前提**: `docs/ITCH_IO_DEMO_RELEASE_GUIDE.md` の Phase 1-2 (Unity ビルド) は完了済み。

---

## 0. Steam リリースまでのフロー (全体像)

```
itch.io デモ公開
    ↓ (フィードバック収集 2-4 週間)
Steam Direct で App ID 申請 ($100)
    ↓ (税務書類対応 + 30 日間待機)
Steamworks Partner Portal 設定
    ↓
ストアページ作成 (Capsule 画像 + 説明文 + 価格)
    ↓
ビルドアップロード (Steamworks SDK 経由)
    ↓
Valve 審査 (1-5 営業日)
    ↓
リリース日設定 + 公開準備
    ↓
Steam EA リリース 🎉
```

所要時間目安: **最短 2 ヶ月、現実的に 2-3 ヶ月**

---

## Phase 1: Steam 開発者登録 (ユーザー作業、初回 30-60 分 + 待機期間)

### 1-1. Steamworks アカウント作成

1. https://partner.steamgames.com にアクセス
2. 既存の Steam アカウントでログイン (またはゲーム公開専用に新規作成推奨)
3. **「Get Started」 → 「Steam Direct」** を選択

### 1-2. Steam Direct 手数料 ($100/title) 支払い

- 1 タイトル = $100 (約 ¥15,000)
- クレジットカード決済 (個人カードでも可)
- 返金不可、ゲーム公開しなくても返金されない
- 一度支払えば永久に有効 (年間費用なし)

### 1-3. 税務書類提出 (W-8BEN 等)

米国非居住者の場合 (日本人):
- **W-8BEN フォーム** をオンラインで記入
- 所要時間: 15-30 分
- 必要情報:
  - 氏名 (パスポート表記の英字)
  - 住所
  - マイナンバー (日本居住者は「日本の納税者番号」として登録)

提出後、約 5-10 営業日で承認される。

### 1-4. 銀行口座登録 (収益振込先)

- SWIFT コード対応の日本の銀行口座 (PayPal でも可、ただし手数料高)
- 最低振込額 $100 から

### 1-5. 30 日間待機期間

Steam Direct の手数料を支払ってから **30 日間** はゲーム公開ができない仕様。
この間に以下を進める:
- ストアページ素材準備
- ビルド作成
- 法務書類 (利用規約・プライバシーポリシー) 整備

---

## Phase 2: App ID 申請とストアページ作成 (ユーザー + Claude 補助、3-7 日)

### 2-1. 新規 App 作成

1. Steamworks Partner Portal にログイン
2. **「Manage Steamworks」 → 「New Application」**
3. 入力項目:
   - **Game name**: `Eggcore Protocol`
   - **Genre**: `Action / Indie / Strategy`
   - **Platforms**: `Windows` (Mac/Linux は将来追加可)
   - **Release date**: 未定 OK (後で確定)

→ Steam が **App ID** (例: `2123456`) を発行。これがゲームの一意 ID になる。

### 2-2. ストアアセット アップロード

`docs/CODEX_RELEASE_ORDER.md` の **P0-2** で生成した 8 種の画像をアップロード:

| アセット | サイズ | Steamworks の項目名 |
|---|---|---|
| Library Capsule | 600×900 | Library Capsule |
| Library Hero | 3840×1240 | Library Hero |
| Library Logo | 1280×720 (透過) | Library Logo |
| Header Capsule | 460×215 | Header Capsule |
| Small Capsule | 462×174 | Small Capsule |
| Main Capsule | 920×430 | Main Capsule |
| Page Background | 1438×810 | Page Background |
| Animated Capsule (任意) | 600×900 mp4 | Animated Capsule |

各画像にはサムネ表示確認があるので、見え方を必ずチェック。

### 2-3. ストアページコピー

`docs/STEAM_STORE_COPY.md` の内容をそのまま貼り付け:
- **Short description** (300 字): 「中央のコア『データエッグ』を守りながら…」
- **About this game** (詳細説明): 機能リスト、EA について、開発者情報
- **System requirements**: 最低・推奨
- **Genres / Tags**: Survivors-like, Roguelite, Tower Defense, Action Roguelike, Indie

### 2-4. スクリーンショット (5-8 枚)

`docs/ITCH_IO_DEMO_RELEASE_GUIDE.md` の Phase 3-4 と同じ。Unity 内で F12 撮影。

推奨シーン:
- メインメニュー
- Wave 5 中ボス Pulswyrm enrage
- Wave 10 ラスボス Phase 2
- クロス進化カットイン
- データラボ (ショップ) UI
- Stage 4 / 5 (氷 / 嵐)

### 2-5. トレーラー (任意推奨、60-90 秒)

YouTube に動画アップロード → URL を Steamworks に貼る。
動画は `docs/CODEX_RELEASE_ORDER.md` の **P2-1** で Codex に発注予定。

### 2-6. ストアページ レビュー申請

すべて入力完了後:
**「Edit Store Page」 → 右上「Save and Publish for Review」**

Valve のレビュー: **1-5 営業日**。
通常パスする項目:
- 説明文に NG ワードなし
- 画像サイズ正確
- 利用規約のリンクあり
- ジャンルタグが妥当

リジェクト時はメールで通知 → 修正して再申請。

---

## Phase 3: Steamworks SDK 連携 (Claude 補助、2-4 時間)

### 3-1. SDK ダウンロード

1. Steamworks Partner Portal 内 **「Get Steamworks SDK」**
2. 最新版を DL (現在 v1.59 想定)
3. 展開先: `C:\SteamworksSDK\` (絶対パス推奨)

### 3-2. Unity プロジェクトに steam_appid.txt 配置

`C:\Users\kodai\OneDrive\デスクトップ\claudecode-app\CoreLanternUnity\steam_appid.txt` を作成し、App ID 1 行のみ:
```
2123456
```
**`.gitignore` には追加しない** (公開しても問題ない情報)。

### 3-3. Steamworks.NET 統合 (任意、実績/クラウドセーブ使う場合)

オープンソースの C# wrapper を使う:
- https://steamworks.github.io/
- Unity Package で `Steamworks.NET` を追加

最小限の実装 (Optional、Claude 担当):
```csharp
// Awake() で初期化
void Awake() {
    if (!SteamAPI.Init()) {
        Debug.LogError("Steamworks init failed");
    }
}

void OnApplicationQuit() {
    SteamAPI.Shutdown();
}
```

⚠ Steamworks SDK 統合は MVP では **任意**。初回 EA リリースは Steamworks 統合なしでもよい (DRM フリー扱いになる)。

---

## Phase 4: ビルドアップロード (ユーザー + Claude 補助、初回 1-2 時間)

### 4-1. SteamPipe (steamcmd) ビルドアップロードツール準備

SteamworksSDK 内に `tools\ContentBuilder\` フォルダがある。これを `C:\SteamworksContent\` にコピー (作業用に複製)。

### 4-2. App Build Script 作成

`C:\SteamworksContent\scripts\app_build_2123456.vdf` (App ID で置換):
```
"AppBuild"
{
    "AppID" "2123456"
    "Desc" "Eggcore Protocol v0.9.0 EA Initial"
    "BuildOutput" "C:\SteamworksContent\output\"
    "ContentRoot" "C:\SteamworksContent\content\"
    "SetLive" ""
    "Depots"
    {
        "2123457"
        {
            "FileMapping"
            {
                "LocalPath" "*"
                "DepotPath" "."
                "Recursive" "1"
            }
        }
    }
}
```

`2123457` は Depot ID (Steamworks Portal で確認、通常 App ID + 1)。

### 4-3. ビルド配置

Unity でビルドした実行ファイルとリソースを `C:\SteamworksContent\content\` にコピー:
```
C:\SteamworksContent\content\
├── EggcoreProtocol.exe
├── UnityPlayer.dll
├── EggcoreProtocol_Data\
│   ├── ...
└── ...
```

### 4-4. アップロード実行

PowerShell:
```powershell
cd C:\SteamworksContent\
.\builder\steamcmd.exe +login YOUR_STEAM_USERNAME +run_app_build scripts\app_build_2123456.vdf +quit
```

- 初回ログインで Steam Guard コード入力 (メールで届く)
- アップロード時間: 100-500 MB のビルドで 5-30 分

### 4-5. ビルド確認

Steamworks Portal の **「Builds」** タブに新しいビルドが表示される。
- BuildID 控える
- 「Default」ブランチに割り当てる

---

## Phase 5: リリース日設定と公開 (ユーザー作業、最終 1-2 日)

### 5-1. 価格設定

1. Steamworks Portal **「Set up your Pricing」**
2. **アーリーアクセス価格**: $2.99 (約 ¥450)
3. **Regional Pricing**:
   - 日本: ¥450
   - 米国: $2.99
   - 欧州: €2.79
   - 中国: ¥18
   - ロシア: ₽149

参考: VS と同等または以下にする (ユーザーの方針)。

### 5-2. リリース日確定

- **最低 2 週間前** にリリース日を確定 (Steam の規定)
- 例: 2026-08-15 をリリース日にする場合、7月末までに設定
- 確定後はストアページに「Coming Soon: 2026-08-15」と表示される

### 5-3. アーリーアクセス情報

EA 限定の追加情報を入力:
- **Why Early Access?**: 「フィードバックを反映してフル版を作るため」
- **Approximately how long will this game be in Early Access?**: 「6-12 ヶ月」
- **How is the full version planned to differ from the Early Access version?**: 「追加キャラ、追加ステージ、難易度、英語ローカライズ」
- **What is the current state of the Early Access version?**: 「全 5 ステージ・全 11 相棒・10 Wave 完全プレイ可能」
- **Will the game be priced differently during and after Early Access?**: 「Yes, $2.99 (EA) → $4.99 (1.0)」
- **How are you planning on involving the Community in your development process?**: 「Steam フォーラム、Discord、月次アップデート」

### 5-4. 法務書類

1. **利用規約 (Terms of Service)**
2. **プライバシーポリシー** (PlayerPrefs しか使わないなら最小限)
3. **クレジット表記** (使用 BGM / SE / フォント等)

ストアページに URL リンクを記載。

### 5-5. リリース当日

1. **「Release Game」** ボタンを押す
2. ストアページが公開される
3. 自動的にフォロワーへメール通知
4. **SNS 告知**:
   - Twitter / X
   - itch.io ページに「Steam で公開しました」追記
   - Reddit (`r/IndieGaming`, `r/Survivorslike` 等)
5. **配信者対応**:
   - Keymailer に登録 (任意、無料)
   - 個別招待コード配布

---

## Phase 6: ローンチ後対応

### 6-1. 初週

- **Steam フォーラム巡回** (1 日 2-3 回)
- **バグ報告に即対応** (致命バグは hotfix を 1-3 日以内)
- **レビュー対応**: 良いレビューには「いいね」、否定的レビューには返信は控えめ

### 6-2. 月次アップデート

EA 期間中の更新:
- バランス調整
- バグフィックス
- 新コンテンツ (新キャラ・新ステージ)
- 各更新時に **Patch Notes** を Steam Community に投稿

### 6-3. 1.0 リリースへ

EA 期間 6-12 ヶ月で:
- 新コンテンツ追加完了
- 主要バグなし
- レビュー 100+ 件、Mostly Positive 以上

これらが揃ったら 1.0 リリース:
- 価格を $4.99 に変更 (Regional Pricing も同時)
- ストアページから「Early Access」バッジ除去
- Launch Discount $2.99 で 1 週間セール

---

## 🎯 Steam EA 成功基準 (初月)

| 指標 | 目標 |
|---|---|
| **販売本数** | 100+ |
| **レビュー数** | 10+ |
| **レビュー評価** | Mostly Positive 以上 |
| **致命バグ** | 0 (即修正済) |
| **アップデート** | 2 回 |

---

## ⚠ 注意事項

1. **税務**: Steam の収益は 米国で 30% 源泉徴収 (W-8BEN 提出済なら 0%)。日本で確定申告必要。
2. **チャージバック**: クレジットカード決済の不正利用に注意。Steam は対応してくれる。
3. **無料配布キー**: EA 価格 $2.99 でも、年間 500 キーまで無料発行可能 (Keymailer / 配信者対応用)。
4. **DRM**: Steamworks SDK 統合なしなら DRM フリー扱い。違法配布リスクあるが、インディー初期は気にしなくて良い。
5. **言語**: ストアページは日本語のみで OK (英語版は後追加可能)。
6. **アダルト**: Eggcore Protocol は対象外だが、暴力表現の自己申告は必要。

---

## ⚙ Claude が補助できるタスク

| タスク | 担当 |
|---|---|
| Unity Build Settings の確認 | 🛠 Claude |
| Player Settings の値検証 | 🛠 Claude |
| Steamworks.NET 統合コード追加 (任意) | 🛠 Claude |
| 実績システム実装 (任意) | 🛠 Claude |
| クラウドセーブ対応 (任意) | 🛠 Claude |
| ストアコピーの英訳 | 🛠 Claude (素案) → 外部レビュー |
| Patch Notes 文面作成 | 🛠 Claude |
| バグ修正パッチ | 🛠 Claude |

---

## 🚀 当面の進め方

1. **今月**: itch.io デモ公開 + フィードバック収集
2. **来月**: Steam Direct 課金 + 30 日間待機開始
3. **30 日後**: ストアページ作成 + ビルドアップロード
4. **その 1-2 週後**: Steam EA リリース 🎉

最終更新: 2026-05-28
担当: Claude (秘書) + ユーザー (実行)
参照: `docs/RELEASE_CHECKLIST.md`, `docs/STEAM_STORE_COPY.md`, `docs/ITCH_IO_DEMO_RELEASE_GUIDE.md`
