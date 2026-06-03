# Claude Task: Smart Title Screen Simplification

作成: 2026-05-31  
担当境界: Codex は素材と仕様のみ。`Assets/Scripts/CoreLanternGame.cs` の接続/レイアウト変更は Claude 担当。

## 目的

タイトル画面から余計な情報を削り、ゲームの顔としてスマートに見せる。

現在のタイトル画面は、ロゴ/サブタイトル/進行サマリー/ステージ・Danger概要/ヒーローデッキ/進化ルートバッジ/5ボタン/操作ガイド/クレジットが同時に出ていて、初見の視線が散りやすい。

## 方針

### 残す

1. ゲームロゴ
2. 短いサブコピー
3. `START RUN`
4. 下部の小さな補助ボタン群
5. 極小のバージョン表記

### 消す / タイトルから退避

1. `BEST / CLEARS / MISSIONS / DANGER / STAGE / COMBO` の進行サマリー
2. `Stage: ... Danger: ...` の選択概要
3. 中央の大型ヒーローデッキ
4. `SPEED / POWER / GUARD` の進化ルートバッジ
5. 操作ガイド `Enter / Space ... ESC ...`
6. `Embercore Studio` 表記。`Embercore` は名称衝突注意なので、スタジオ名確定まで表示しない

進行サマリーは `MISSION` / `図鑑` / `進化コンボ` 側で見られればよい。ステージとDangerは `START RUN` 後の `RUN SETUP` 画面で見せる。

## 新規/既存素材

| File | Size | Use |
|---|---:|---|
| `Assets/Resources/Skins/GameLogo_TitleCompact.png` | 1024x320 | タイトル画面用コンパクトロゴ |
| `Assets/Resources/Skins/GameLogo.png` | 1024x512 | 汎用/ストア用ロゴ |

`GameLogo_TitleCompact.png` はCodexが既存ロゴから上下余白を詰めて生成済み。四隅 alpha=0。

## 推奨レイアウト

1280x720 reference想定。

| Element | Position | Size | Notes |
|---|---:|---:|---|
| Logo | `(0, 210)` | `(620, 194)` | `GameLogo_TitleCompact`, preserveAspect |
| Subtitle | `(0, 82)` | `(620, 24)` | `相棒を進化させて、データエッグを守り抜く` |
| Start Button | `(0, -48)` | `(420, 60)` | 現行より上げる |
| Secondary Buttons | `y=-148` | `140x38` | 5個でもよいが薄め |
| Version | `(0, -332)` | `(640, 16)` | `Eggcore Protocol vX.X` のみ |

## CoreLanternGame.cs 実装案

### 1. ロゴsprite

`GameLogo_TitleCompact` を優先し、無ければ `GameLogo`、それも無ければ既存Textタイトルへfallback。

```csharp
Sprite gameLogoSprite;
Sprite gameLogoTitleCompactSprite;
```

```csharp
gameLogoSprite = LoadOptionalSprite("Skins/GameLogo", null);
gameLogoTitleCompactSprite = LoadOptionalSprite("Skins/GameLogo_TitleCompact", gameLogoSprite);
```

### 2. タイトル周辺

既存の `Main Menu Title` Text はfallbackとして残す。

```csharp
var logo = gameLogoTitleCompactSprite != null ? gameLogoTitleCompactSprite : gameLogoSprite;
if (logo != null)
    CreateMenuSpriteImage("Main Menu Game Logo", mainMenuPanel.transform, logo, new Vector2(0, 210), new Vector2(620, 194), new Color(1f, 1f, 1f, 0.98f), true);

var title = CreateText("Main Menu Title", mainMenuPanel.transform, new Vector2(0, 210), TextAnchor.MiddleCenter, 54, new Color(0.68f, 1f, 1f));
title.text = "EGGCORE PROTOCOL";
title.gameObject.SetActive(logo == null);
```

`Title_LogoUnderline_v2` はロゴ画像内にラインがあるため、タイトル画面では一旦使わない。

### 3. サブコピー

`Title_SubtitlePlate_v2` は使わず、テキストだけにする。

```csharp
var subtitle = CreateText("Main Menu Subtitle", mainMenuPanel.transform, new Vector2(0, 82), TextAnchor.MiddleCenter, 15, new Color(0.82f, 0.96f, 0.96f));
subtitle.text = "相棒を進化させて、データエッグを守り抜く";
subtitle.rectTransform.sizeDelta = new Vector2(620, 24);
```

### 4. 進行サマリーを非表示

以下はタイトルでは作らない、または `SetActive(false)`。

- `Menu Summary Plate`
- `Menu Stats Ribbon V2`
- `menuBestText`
- `mainMenuSelectionBriefText`

ただし `RefreshMainMenuSummary()` / `RefreshSelectionInfoLine()` が参照するので、最小リスクなら生成してすぐ非表示にする。

```csharp
menuBestText = CreateText(...);
menuBestText.gameObject.SetActive(false);

mainMenuSelectionBriefText = CreateText(...);
mainMenuSelectionBriefText.gameObject.SetActive(false);
```

### 5. ヒーローデッキを削除

`CreateMainMenuHeroVisual(mainMenuPanel.transform);` は呼ばない。

関数自体は残してよい。後で別画面やストア撮影用に使える。

### 6. START RUN を中央に寄せる

```csharp
startMenuButton = CreateWideButton("Start Run Button", mainMenuPanel.transform, new Vector2(0, -48), new Vector2(420, 60));
```

### 7. 補助ボタンを控えめに

機能は消さず、視覚の主役にしない。

推奨:

```csharp
const float secY = -148f;
const float secW = 136f;
const float secH = 38f;
const float secGap = 10f;
const float secFirstCenter = -292f;
```

`Title_NavRail_v2` は主張が出る場合はタイトルでは使わない。

### 8. ガイド文を削除

`Main Menu Guide` は作らない。

操作説明は必要になったらオプション/ヘルプ側へ。

### 9. クレジットを簡素化

`Embercore Studio` は未確定なので外す。

```csharp
credit.text = "Eggcore Protocol  v" + GameVersion;
credit.color = new Color(0.42f, 0.58f, 0.64f, 0.72f);
```

## 目視QA

必ずGame viewで確認:

1. ロゴ、サブコピー、STARTが重ならない。
2. 上部のロゴが画面外に切れない。
3. 5つの補助ボタンが主張しすぎない。
4. 進行サマリーとステージ/Danger情報がタイトルから消えている。
5. Run Setupを開けばステージ/Danger情報は確認できる。
6. 1920x1080 / Free Aspect / 可能なら1366x768でも破綻しない。

## 検証

```powershell
powershell -ExecutionPolicy Bypass -File .\Tools\CheckCompileHealth.ps1
powershell -ExecutionPolicy Bypass -File .\Tools\TestImageResources.ps1
```

必要ならUnity Editor上でスクショを撮り、文字被りと画像重なりを目視確認する。
