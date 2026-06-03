# UI V2 目視QAチェックリスト

2026-05-30 時点で、Codex + Claude が大量の V2 UI 素材をコードに接続した。
これらは**寸法ベースで接続しただけで実機の見た目は未確認**。
このチェックリストに沿って Unity Editor で目視し、問題箇所を Claude に報告する。

## 使い方

1. Unity Editor を開く → Console を開く（Ctrl+Shift+C）
2. Play
3. 下記を上から順に確認。**OK なら ✅、問題があれば内容をメモ**
4. 問題箇所は「画面名 / 要素名 / 症状」を Claude に伝える（例:「メインメニュー / コーナー装飾 / タイトル文字に被ってる」）

報告を受けたら Claude が該当の座標・サイズ・アルファを調整、または該当フラグを false に戻す。

---

## 有効化されている V2 フラグ（全部 true）

| フラグ | 対象 | 行 |
|---|---|---|
| `UseWaveProgressHudV2` | ウェーブ進行バー | 34 |
| `UseHudBarV2` | HP/コアHP/EXP バー | 35 |
| `UseButtonV2` | 全ボタン背景（16種） | 39 |
| `UseTitleUiV2` | タイトル装飾（コア/下線/サブタイトル板/リボン/ナビレール/コーナー） | 40 |
| `UseResultUiV2` | リザルト（デッキ枠/ポートレート/statタイル/バッジ/MVP/サマリ/フッター/ランクメダル） | 41 |
| `UseCardPartsV2` | 強化カード部品 | 42 |

**問題が広範囲なら、該当フラグ1つを `false` に戻すだけで旧UIへ一括ロールバック可能**（Claude が対応）。

---

## ① 起動直後 / Console

- [ ] Console に**赤エラーが出ていない**（出たら全文を Claude へ）
- [ ] Console に大量の黄警告が出ていない（NullReference, Missing sprite 等）
- [ ] 起動時にフリーズ・無限ループしない

## ② メインメニュー画面

タイトル「EGGCORE PROTOCOL」周辺は装飾が最も密集。重なり要注意。

- [ ] タイトル文字が装飾（下線 `Title_LogoUnderline`）と**重なって読めなくなっていない**
- [ ] サブタイトル「相棒を進化させて〜」がプレート（`Title_SubtitlePlate`）からはみ出していない
- [ ] BEST/CLEARS の統計リボン（`Title_StatsRibbon`）が数字と被っていない
- [ ] 中央のコアエンブレム（`Title_CoreEmblem` 210×210）が**ボタンやテキストを覆っていない**
- [ ] 四隅のコーナー装飾（`Title_CornerAccent`）が画面外にはみ出していない / タイトルに被っていない
- [ ] ナビレール（`Title_NavRail`）が下部5ボタンの背面に正しく敷かれている
- [ ] START RUN ボタンの背景（`Button_Primary_Start`）が文字と合っている
- [ ] セカンダリ5ボタン（MISSION/進化ツリー/進化コンボ/図鑑/OPTIONS）の背景が揃っている
- [ ] ボタンを**ホバー/クリックして反応する**（V2スプライトが raycast を阻害していない）

## ③ RUN SETUP（ステージ/Danger選択）

- [ ] ステージチップ5個が正しく並ぶ。サムネ画像が潰れていない
- [ ] Danger チップ（`Button_DangerChip`）の背景が D0〜D5 で揃っている
- [ ] START / BACK ボタン（`Button_RunStart` / `Button_RunBack`）が機能する
- [ ] 選択中のチップがハイライトされる（色 tint が効いている）

## ④ ゲーム中 HUD

- [ ] HP バー / コアHP バー / EXP バー（HudBarV2）が枠内に収まっている
- [ ] ウェーブ進行バー（WaveProgressV2）が画面上部で正しく表示・減少する
- [ ] バー数値テキストがバー画像からはみ出していない

## ⑤ 強化モジュール選択（レベルアップ時）

- [ ] 3枚のカード部品（CardPartsV2）が崩れていない
- [ ] Reroll / Skip ボタン背景（`Button_Reroll` / `Button_SkipReward`）が機能する
- [ ] カードをクリックして選択できる

## ⑥ ポーズ画面（Esc / P）

- [ ] RESUME / OPTIONS / RESTART ボタン背景が揃っている
- [ ] 各ボタンが機能する

## ⑦ オプション画面

- [ ] CLOSE ボタン背景（`Button_CloseSmall`）が機能する
- [ ] ステッパー（±）ボタン（`Button_Stepper`）が小さすぎず機能する

## ⑧ リザルト画面（クリア / 敗北の両方）

最も多くの V2 素材が集中。要重点確認。

- [ ] デッキ枠（`Result_DeckFrame`）が画面中央に正しく表示
- [ ] ポートレート枠（`Result_PortraitFrame`）にキャラ画像が収まっている
- [ ] 6個の stat タイル（WAVE/TIME/KILLS/DATA/DAMAGE/RANK）が揃って読める
- [ ] **RANK タイルにランクメダル（`Result_RankMedal_S/A/B/C/D`）が重なって表示**される
  - メダルとランク文字（S/A/B/C/D）が**二重に見えて汚くないか**を特に確認
  - メダルが文字を隠していないか / 文字がメダルから外れていないか
- [ ] ルート/融合バッジ（`Result_BadgeStrip`）が読める
- [ ] MVP BUILD 3行（`Result_MvpRow`）が崩れていない
- [ ] サマリプレート（`Result_SummaryPlate`）の上にテキストが乗っている
- [ ] フッターガイド（`Result_FooterGuide`）の上に「R:再戦 M:メニュー」が乗っている
- [ ] 同設定で再戦 / メニュー ボタンが機能する

## ⑨ 1ラン通しプレイ

- [ ] Stage 1 を最後まで（Wave 10 + ボス）プレイできる
- [ ] Stage 別 BGM が切り替わる（Stage を変えて再確認推奨）
- [ ] 進化・クロス進化カットインが正しく出る
- [ ] クリア / 敗北でリザルトに正しく遷移する

---

## 報告テンプレート（Claude へ）

```
画面: （例: リザルト）
要素: （例: RANK メダル）
症状: （例: メダルの絵と S の文字が重なって両方汚く見える）
重要度: 高 / 中 / 低
```

重要度「高」が複数なら、該当 V2 フラグを一旦 false に戻して旧UIで EA を出し、
V2 は後日 Unity 上で詰める判断もアリ（Claude が提案する）。

最終更新: 2026-05-30
