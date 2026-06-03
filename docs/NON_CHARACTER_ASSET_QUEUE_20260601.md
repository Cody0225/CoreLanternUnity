# 非キャラ素材キュー 2026-06-01

目的: Claude と衝突せずに進められる、キャラ生成以外の残作業を固定する。

## ルール

- Codex は `Assets/Scripts/CoreLanternGame.cs` を触らない。
- Codex は素材生成、プレビュー、台帳、検証、Claude向け仕様書まで担当。
- Claude は `CoreLanternGame.cs` 接続、UI配置、ロジック、バランスを担当。
- 見た目に関わるものは、画像比率を変えた引き伸ばし禁止。Unity接続時は `preserveAspect = true` を基本にする。

## 現在の非キャラ残タスク

| 優先 | 項目 | 現状 | 次担当 |
|---|---|---|---|
| P0 | Aエッグのタイトル/リザルト接続 | 3素材生成済み、秘書レビューOK | Claude |
| P0 | HUD9 残りパネル/バー | 8素材生成済み、Wave進行バーのみ接続済み | Claude |
| P1 | Stage4/5サポート | 6素材生成済み、未接続 | Claude |
| P1 | V2 UI残り素材 | 14素材が未接続。全部採用ではなく選別が必要 | Claude + Codex検収 |
| P1 | 音素材の本番化 | 仮BGM/SEあり。旧ボスBGM fallback警告2件あり | Codex |
| P2 | Steam/itch/宣伝素材 | itch下書きとexeアイコンは済み。Steamは後半でよい | Codex |

## 今回作った確認物

- `Assets/ArtSource/NonCharacterQueue_20260601/NonCharacterAssetQueue_20260601.png`
- `Tools/GenerateNonCharacterQueuePreview.ps1`

このプレビューは、Aエッグ、HUD9、Stage4/5、V2 UI残りを一枚で確認するためのもの。

## Claudeへ渡すべき接続仕様

- `docs/CLAUDE_A_EGG_PRESENTATION_HOOK_SPEC.md`
- `docs/CLAUDE_HUD9SLICE_HOOK_SPEC.md`
- `docs/CLAUDE_STAGE45_SUPPORT_HOOK_SPEC.md`
- `docs/CLAUDE_TITLE_LOGO_HOOK_SPEC.md`
- `docs/CLAUDE_TITLE_SCREEN_SIMPLIFY_SPEC.md`

## Codexが次に安全に進められること

1. 音素材の棚卸しと、耳障りな上昇音対策の仕様化。
2. Steamストア画像の前準備だけ進める。ただしキャラ差し替え前なので最終生成は後回し。
3. HUD9/V2 UI素材の見た目だけを再検収し、不要・危険・採用候補に分類する。
4. Stage4/5の非キャラ環境素材を追加で作る。敵キャラやプレイヤーキャラは後回し。

## 注意

キャラ、進化後、クロス進化の大規模画像生成は最終タスク寄り。現在は `MOTION_REVIEW` までで、ユーザー承認前のUnity接続は禁止。
