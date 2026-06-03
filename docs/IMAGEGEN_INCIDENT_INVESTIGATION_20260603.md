# Image Generation Incident Investigation - 2026-06-03

## 結論

2026-06-03 時点の異常生成は、プロンプト単体の失敗ではなく、Codex/image generation ツール経路または OpenAI 側サービス状態の影響が強い。

根拠:

- 最小プロンプト「青い狼のキャラを1枚だけ生成して。文字なし。」でも、実結果は英文法ポスターになった。
- 直近の複数生成が、青狼/サイバー狼/キャラ素材とは無関係な教育ポスターや政治ポスターになっている。
- 生成ごとに新しい PNG ファイルは保存されているため、単なる古い画像の見間違いではない。
- OpenAI 公式ステータスで、同日に Codex / ChatGPT / Responses API 系の障害が確認された。

## ローカル証拠

保存先:

`C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8`

直近の異常例:

| JST時刻 | ファイル傾向 | 実際の内容 |
|---|---|---|
| 2026-06-03 16:01:31 | 新規 PNG | `PARTS OF SPEECH` 英文法ポスター |
| 2026-06-03 12:15:20 | 新規 PNG | `Life Cycle of a Butterfly` 教育ポスター |
| 2026-06-03 11:55:34 | 新規 PNG | `States of Matter` 教育ポスター |
| 2026-06-03 11:52:56 | 新規 PNG | 政治キャンペーン風ポスター |

寸法検証:

- 上記3枚はいずれも `1536x1024 / Format24bppRgb`
- 透明キャラ素材で期待する `512x512 / alphaあり` ではない

## プロンプト影響の判定

プロンプトが複雑すぎた可能性は低い。

理由:

- 長いキャラ仕様プロンプトだけでなく、最小テストプロンプトでも破綻した。
- 失敗内容が「狼の造形が悪い」「透明でない」ではなく、「題材そのものが完全に別物」になっている。
- ローカルの prompt history には、直近ユーザー指示として青狼/進化/F1究極フォーム/テスト指示が残っており、教育ポスターを依頼した形跡は見当たらない。

## 推定原因

可能性が高い順:

1. OpenAI 側の画像生成または Responses/Codex 経路の障害
2. Codex image generation ツールの request/result 取り違え、またはルーティング異常
3. このスレッド/ローカル生成セッションの一時的な状態破損
4. プロンプト品質の問題

4 は今回の主因ではない可能性が高い。

## 推奨対応

当面、 production 用キャラ画像生成は停止する。

再開条件:

1. OpenAI status の Codex / ChatGPT / Responses API 障害が解消済みになる。
2. Codex アプリを再起動、または新規スレッドで image generation を試す。
3. 最小 sanity test を1回だけ実行する。

Sanity test:

`A single blue wolf character. No text. No diagram. Transparent background.`

合格条件:

- 青い狼キャラが出る
- 文字や説明図が出ない
- 画像の題材がプロンプトと一致する

失敗した場合:

- 追加生成しない
- キャラ画像タスクは停止
- Claude には「画像生成ツール側がまだ不安定。接続作業のみ継続可」と共有する

## 参照

- OpenAI Status: https://status.openai.com/
- Incident: https://status.openai.com/incidents/01KT5XJ5ATD6RMYP908WS69FVD
