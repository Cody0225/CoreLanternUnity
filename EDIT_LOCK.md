# EDIT LOCK — CoreLanternGame.cs 編集ロック

このファイルは `Assets/Scripts/CoreLanternGame.cs` の同時編集を防ぐためのロック台帳。
**cs を編集する前に必ずここを確認し、ロックを取得すること。**

参照: `HANDOFF_FOR_CODEX.md`（役割分担）、`CLAUDE.md` セクション E（衝突回避）

---

## 🔒 現在のロック状態

```
STATUS: RELEASED
HOLDER: -
SINCE:  -
SCOPE:  -
```

- `RELEASED` = 誰も編集していない。取得してよい。
- `LOCKED`   = 誰かが編集中。取得者が `RELEASED` にするまで待つ。

---

## ロックの取り方（cs を編集する直前）

1. 上の `STATUS` が `RELEASED` であることを確認
2. 以下のように書き換える:
   ```
   STATUS: LOCKED
   HOLDER: Claude        (または Codex)
   SINCE:  2026-05-30 14:30
   SCOPE:  Button V2 接続 (CreateMainMenuPanel 周辺)
   ```
3. 編集 + 検証（`Tools/CheckCompileHealth.ps1`）完了後、`STATUS: RELEASED` に戻す

## 原則

- **cs を編集するのは原則 Claude のみ**（`HANDOFF_FOR_CODEX.md` の役割分担）。
  Codex は素材生成に専念し、このロックを取る必要は通常ない。
- ロックは「念のための可視化」。役割分担を守っていれば発動しない。

---

## ロック履歴（任意・追記式）

| 日時 | 取得者 | 範囲 | 解放 |
|---|---|---|---|
| 2026-05-30 | Claude | Button/Result V2 接続, Stage BGM, バグ修正 | 済 (RELEASED) |
| 2026-05-30 | Claude | Result_RankMedal 接続予定で取得 → Codex が既に実装済みと判明、cs 未編集で即解放 | 済 (RELEASED) |
| 2026-05-31 | Claude | タイトル画面スマート化 (GameLogo接続 / 進行サマリー・ヒーローデッキ・ガイド退避 / クレジット簡素化) | 済 (RELEASED) |
| 2026-06-01 | Codex | HUD9 Wave進行バー限定接続 (Claudeレート制限中の一時巻き取り) | 済 (RELEASED) |
| 2026-06-02 | Claude | タイトル6体横一列レイアウト実装 (CreateMainMenuHeroArt 全面書き換え, Roslyn 0err) | 済 (RELEASED) |
| 2026-06-02 | Claude | HUD9 9-slice バー4枚接続 (Back/HP/CoreHP/EXP, CreateHudBar, Roslyn 0err) | 済 (RELEASED) |
| 2026-06-02 | Claude | HUD9 9-slice パネル4枚接続 (Wave/HP/Level/ChipMini, ApplyGeneratedPanelSkin, Roslyn 0err) | 済 (RELEASED) |
| 2026-06-03 | Claude | クロス進化の表示を常にL3固定 (融合素材は F*_L3 のみで足りる, Roslyn 0err) | 済 (RELEASED) |
| 2026-06-03 | Claude | HUD9パネルの二重枠解消 (Outline/ネオン装飾線をHUD9時のみ省略, Roslyn 0err) | 済 (RELEASED) |
| 2026-06-03 | Claude | Stage4/5支援素材6枚接続 (サムネFrost/Storm, Frost Patch/Crystal, Lightning Marker/Strike, ReportAssetStatus Hooked 6/6, Roslyn 0err) | 済 (RELEASED) |
| 2026-06-03 | Claude | A Egg presentation 3枚接続 (Title背後エンブレム/Result透かし/Codexアイコン, Hooked 3/3, Roslyn 0err) | 済 (RELEASED) |
