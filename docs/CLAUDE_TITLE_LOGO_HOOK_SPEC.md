# Claude Task: Title Screen Game Logo Hook

作成: 2026-05-31  
担当境界: Codex は素材と仕様のみ。`Assets/Scripts/CoreLanternGame.cs` の接続は Claude 担当。

## 目的

タイトル画面の live text `EGGCORE PROTOCOL` を、生成済みの透明PNGロゴ `GameLogo.png` に置き換える。

## 既存素材

| File | Size | Use |
|---|---:|---|
| `Assets/Resources/Skins/GameLogo.png` | 1024x512 | メインタイトルロゴ |
| `Assets/Resources/Skins/GameLogo_Mono.png` | 1024x512 | モノクロ版 |
| `Assets/Resources/Skins/GameLogo_Mark.png` | 512x512 | アイコン派生用マーク |
| `Assets/Resources/Skins/GameLogo_Mark_Mono.png` | 512x512 | モノクロマーク |

全て四隅 alpha=0 検証済み。

## 推奨表示

- 表示位置: `new Vector2(0, 268)`
- 表示サイズ: `new Vector2(560, 280)`
- `preserveAspect = true`
- `raycastTarget = false`
- 既存の `Main Menu Title` Text は削除せず、ロゴが無い場合の fallback として残す
- ロゴありの場合だけ `title.gameObject.SetActive(false)` にする

## CoreLanternGame.cs 変更案

### 1. フィールド追加

既存の title UI sprite フィールド群の近くに追加:

```csharp
Sprite gameLogoSprite;
```

### 2. CreateAssets などのロード箇所に追加

既存の `titleLogoUnderlineSprite = LoadOptionalSprite("Skins/Title_LogoUnderline_v2", null);` 付近に追加:

```csharp
gameLogoSprite = LoadOptionalSprite("Skins/GameLogo", null);
```

### 3. CreateMainMenuPanel のタイトル周辺を調整

現在:

```csharp
if (UseTitleUiV2 && titleLogoUnderlineSprite != null)
    CreateMenuSpriteImage("Menu Logo Underline V2", mainMenuPanel.transform, titleLogoUnderlineSprite, new Vector2(0, 238), new Vector2(620, 22), new Color(1f, 1f, 1f, 0.9f), false);

var title = CreateText("Main Menu Title", mainMenuPanel.transform, new Vector2(0, 268), TextAnchor.MiddleCenter, 54, new Color(0.68f, 1f, 1f));
title.text = "EGGCORE PROTOCOL";
```

推奨:

```csharp
if (UseTitleUiV2 && titleLogoUnderlineSprite != null)
    CreateMenuSpriteImage("Menu Logo Underline V2", mainMenuPanel.transform, titleLogoUnderlineSprite, new Vector2(0, 238), new Vector2(620, 22), new Color(1f, 1f, 1f, 0.9f), false);

if (gameLogoSprite != null)
    CreateMenuSpriteImage("Main Menu Game Logo", mainMenuPanel.transform, gameLogoSprite, new Vector2(0, 270), new Vector2(560, 280), new Color(1f, 1f, 1f, 0.98f), false);

var title = CreateText("Main Menu Title", mainMenuPanel.transform, new Vector2(0, 268), TextAnchor.MiddleCenter, 54, new Color(0.68f, 1f, 1f));
title.text = "EGGCORE PROTOCOL";
title.gameObject.SetActive(gameLogoSprite == null);
```

## 目視確認ポイント

1. ロゴがサブタイトル `相棒を進化させて、データエッグを守り抜く` に重ならない。
2. ロゴ下線 `Title_LogoUnderline_v2` がロゴの文字本体に刺さらない。
3. メニュー統計リボンや START RUN ボタンと縦方向に干渉しない。
4. 16:9 / Free Aspect / 1920x1080 でタイトル上部が切れない。
5. `GameLogo.png` が無い場合、従来のTextタイトルが表示される。

## 検証

接続後に実行:

```powershell
powershell -ExecutionPolicy Bypass -File .\Tools\CheckCompileHealth.ps1
powershell -ExecutionPolicy Bypass -File .\Tools\TestImageResources.ps1
```

Unity Editor の Game view でタイトル画面を目視確認する。
