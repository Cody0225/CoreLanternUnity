# Eggcore Protocol — パッチノート テンプレート

EA 期間中の月次・随時アップデート用テンプレート。  
Steam Community / itch.io 掲示板 / X (Twitter) の 3 媒体に転用できるフォーマット。

参照: `docs/BACKLOG_FROM_DEMO_FEEDBACK.md` (反映元)、`docs/KPI_TRACKING.md` (効果計測)

最終更新: 2026-05-29

---

## バージョン命名規則

```
v0.9.X   — EA 期間中パッチ (X = 0 から始まり月次インクリメント)
v0.9.X.Y — ホットフィックス (Y = 緊急修正の連番)
v1.0.0   — 正式リリース (EA 終了)
```

例:
- `v0.9.0` — EA 初回リリース
- `v0.9.1` — 1 ヶ月後の月次パッチ
- `v0.9.1.1` — 緊急バグ修正

---

## Steam Community 投稿テンプレート (Japanese)

> **使い方:** 下記をコピー → `[内容]` 部分を実際の変更内容に書き換えて投稿。  
> Steam Community では `[b]`, `[i]`, `[u]`, `[list]` などの BBCode が使える。

```
[h1]パッチノート v0.9.X — [アップデート名] 🔥[/h1]

いつもプレイしてくれてありがとうございます！
v0.9.X をリリースしました。今回の主な変更点をお伝えします。

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]🐛 バグ修正[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [バグ内容] を修正しました。(#issue番号)
[*] [バグ内容] を修正しました。
[*] [バグ内容] を修正しました。
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]⚖️ バランス調整[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [キャラ/敵/スキル名]: [変更前] → [変更後]
[*] [キャラ/敵/スキル名]: [変更前] → [変更後]
[/list]

[i]開発メモ: [なぜこの調整をしたか、プレイヤーへの意図][/i]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]✨ QoL 改善[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [改善内容]
[*] [改善内容]
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]🆕 新コンテンツ[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [新機能/新キャラ/新ステージなど]
[*] [新機能/新キャラ/新ステージなど]
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]⚠️ 既知の問題[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [未修正の問題と回避策 (あれば)]
[/list]

次のパッチで対応予定です。

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]📢 開発者より[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[フリーコメント欄。今月の進捗・次回予告・感謝など。3〜5 文を目安に。]

フィードバックは Steam のディスカッション欄や X (@EmberStudio_dev) へどうぞ！
引き続き Eggcore Protocol をよろしくお願いします。

— Embercore Studio
```

---

## Steam Community 投稿テンプレート (English)

> 英語圏ユーザー向け。日本語版と同日投稿推奨。

```
[h1]Patch Notes v0.9.X — [Update Name] 🔥[/h1]

Thanks for playing Eggcore Protocol!
Here's what changed in v0.9.X.

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]🐛 Bug Fixes[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] Fixed [bug description]. (#issue)
[*] Fixed [bug description].
[*] Fixed [bug description].
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]⚖️ Balance[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [Character/Enemy/Skill]: [before] → [after]
[*] [Character/Enemy/Skill]: [before] → [after]
[/list]

[i]Dev note: [Intent behind the change][/i]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]✨ Quality of Life[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [Improvement]
[*] [Improvement]
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]🆕 New Content[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [New feature / character / stage]
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]⚠️ Known Issues[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[list]
[*] [Issue + workaround if any] — fix planned for next patch
[/list]

━━━━━━━━━━━━━━━━━━━━━━━━
[h2]📢 From the Developer[/h2]
━━━━━━━━━━━━━━━━━━━━━━━━

[Free comment: progress, next preview, thanks. 3–5 sentences.]

Leave feedback in Steam Discussions or on X (@EmberStudio_dev). Thanks for your support!

— Embercore Studio
```

---

## X (Twitter) 告知テンプレート

> 280 文字制限。スレッド形式で 2〜3 ツイートに分割可。

```
【パッチノート】Eggcore Protocol v0.9.X リリース！

主な変更:
・[変更点 1]
・[変更点 2]
・[変更点 3]

詳細はSteamパッチノートへ↓
[Steam コミュニティ URL]

#EggcoreProtocol #IndieGame #EmberStudio
```

---

## itch.io 掲示板テンプレート (簡易版)

> itch.io のデモ版用。Steam より短くてよい。

```
## Patch v0.9.X — [Update Name]

**Bug Fixes**
- [fix]
- [fix]

**Balance**
- [change]

**Notes**
[Short dev comment. Thank you for trying the demo!]

Full notes: [Steam Community link]
```

---

## パッチノート記入ガイド

### セクション優先度

| セクション | 必須？ | 目安件数 |
|---|---|---|
| バグ修正 | 必須 (0件でも「なし」と明記) | 3〜10件 |
| バランス調整 | EA 期間は必須 | 1〜5件 |
| QoL 改善 | 推奨 | 1〜3件 |
| 新コンテンツ | あれば | 0〜3件 |
| 既知の問題 | 推奨 | 1〜3件 |
| 開発者より | 必須 | 3〜5文 |

### 書き方のルール

1. **バグ修正**: 「〜を修正」で統一。技術的すぎる説明は避ける
   - OK: `ステージ 3 のボスが出現しないことがある問題を修正`
   - NG: `GetEnemySpawnList() の NullReferenceException を修正`

2. **バランス**: 必ず数値を明記する
   - OK: `Cobalt Pup の攻撃速度: 1.2 → 1.0 (強化)`
   - NG: `Cobalt Pup を少し強くした`

3. **QoL**: プレイヤー視点の言葉で
   - OK: `ランク選択画面でスキル詳細をホバー確認できるようになった`
   - NG: `ToolTipManager の表示タイミングを修正`

4. **開発者より**: EA の「なぜ今公開したか」文脈を忘れずに。フィードバックへの感謝を必ず含める

### バランス調整の方向感を示す記号

```
(強化)   — バフ。プレイヤーに有利な変更
(弱体化) — ネーフ。ゲームバランス上必要な調整
(変更)   — 方向性変更。強弱は文脈次第
(修正)   — 意図しない挙動の是正
```

---

## 月次リリースチェックリスト

パッチ投稿前に必ず確認:

```
[ ] BACKLOG_FROM_DEMO_FEEDBACK.md で DONE になった項目を全部反映した
[ ] バランス変更は KPI_TRACKING.md の目標値と整合している
[ ] 既知の問題セクションに未修正の重要バグを記載した
[ ] 日本語版と英語版の内容が一致している
[ ] X 用の短縮ツイートも用意した
[ ] Steam コミュニティ投稿の URL を X ツイートに貼った
[ ] itch.io にも投稿した (デモ版が存在する場合)
[ ] 投稿後に KPI_TRACKING.md の「パッチ履歴」に記録した
```

---

## 過去パッチアーカイブ (記録欄)

| バージョン | リリース日 | テーマ | 主要変更 | Steam URL |
|---|---|---|---|---|
| v0.9.0 | (EA リリース日) | 初回 EA | 初リリース | — |
| v0.9.1 | | | | |
| v0.9.2 | | | | |

---

## 関連ドキュメント

| ドキュメント | 役割 |
|---|---|
| `docs/BACKLOG_FROM_DEMO_FEEDBACK.md` | フィードバック反映元。DONE になったものをパッチノートに書く |
| `docs/KPI_TRACKING.md` | バランス変更の根拠数値。プレイ時間・クリア率・離脱率を参照 |
| `docs/EA_TO_1_0_ROADMAP.md` | マイルストーン確認。パッチが v1.0 計画と整合しているか |
| `docs/POST_RELEASE_OPS_PLAYBOOK.md` | パッチ後の運用フロー (レビュー対応、緊急 hotfix 手順) |
| `docs/MARKETING_QUOTES.md` | 好意的レビューをパッチノートの冒頭引用に使う場合はここから |
