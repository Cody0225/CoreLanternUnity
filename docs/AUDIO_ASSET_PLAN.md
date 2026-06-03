# CoreLanternUnity Audio Asset Plan

Last updated: 2026-05-22

目的: 仮置きのプロシージャルBGM/SEを、商用前提で使える音素材へ置き換えるための制作方針。  
方針は「無料または無料枠で開始できる」「商用利用の根拠を残せる」「ゲーム全体で音色が散らからない」を優先する。

## 結論

最初のおすすめルート:

1. **SEはCC0/自作寄りで固める**
   - Kenney CC0、OpenGameArt CC0、Freesound CC0を第一候補にする。
   - 足りない変身音・融合音だけ Stable Audio 3.0 Small SFX などのAI生成を検討する。
2. **BGMはStable Audio 3.0を第一候補にする**
   - Stability AI公式情報では、Community License条件下で出力物の配布・商用化が可能。
   - ただし、組織の年間収益がUSD $1Mを超える場合はEnterprise条件になるため、ライセンスログへ必ず記録する。
3. **Eleven Musicは候補だが、無料商用の第一候補にはしない**
   - 公式APIページでは paid plans の商用利用を明記している。
   - Free planやゲーム用途の扱いは最新Terms確認が必要なので、無料商用だけで進めるなら保留。
4. **Suno Freeは使わない**
   - 公式Pricing上、Free PlanはNo commercial use。
   - 有料化後も、Free時代に作った曲が商用化できるとは限らないため混ぜない。

## 参照した公式情報

- Stability AI Stable Audio 3.0: https://stability.ai/news-updates/meet-stable-audio-3-the-model-family-built-for-artistic-experimentation-with-open-weight-models
- Stability AI License: https://stability.ai/license
- Eleven Music API: https://elevenlabs.io/music-api
- ElevenLabs publishing help: https://help.elevenlabs.io/hc/en-us/articles/13313564601361-Can-I-publish-the-content-I-generate-on-the-platform
- Suno Pricing: https://suno.com/pricing
- Kenney support/license: https://kenney.nl/support
- OpenGameArt FAQ: https://opengameart.org/content/faq
- Freesound FAQ: https://freesound.org/help/faq/
- Pixabay FAQ: https://pixabay.com/service/faq/
- DOVA-SYNDROME EN: https://dova-s.jp/en/

## 音のトンマナ

ゲーム全体:

- 暗いサイバー床、ネオン、進化、相棒、データコア防衛に合う音。
- かわいい相棒感は残しつつ、戦闘中は軽すぎない。
- 音色は「FMシンセ」「粒状ノイズ」「短いレーザー」「低いサブベース」「金属ではなくデジタル装甲」を中心にする。
- メロディは覚えやすくしすぎない。長時間プレイで邪魔にならないループ優先。

避ける:

- 既存IP名、アーティスト名、曲名、歌詞参照。
- 派手なボーカル曲。
- YouTube Content ID登録済みっぽいBGMを最終版に使うこと。
- SEの残響が長すぎて連射時に濁ること。

## 必要BGM

| ID | 用途 | 優先 | 長さ | 方針 |
|---|---:|---:|---:|---|
| `BGM_Title_Menu` | メインメニュー | P0 | 60-90秒 loop | 静かな期待感、低BPM、明るすぎない |
| `BGM_Stage1_Arena` | 通常アリーナ | P0 | 90-120秒 loop | 走り続けられる軽い緊張 |
| `BGM_Stage2_Lava` | 溶岩エリア | P0 | 90-120秒 loop | 熱、低音、煙、危険報酬感 |
| `BGM_Stage3_BrokenCore` | Broken Core Network | P1 | 90-150秒 loop | 不安定なデータ空間、壊れた信号 |
| `BGM_Boss_Pulswyrm` | 中ボス | P0 | 60-90秒 loop | Stage曲より強いが長すぎない |
| `BGM_Boss_Nullwyrm` | 最終ボス | P0 | 90-150秒 loop | 最終決戦、重いサブベース、Phase2対応 |
| `BGM_Victory` | クリア | P1 | 6-12秒 sting | 短い達成感 |
| `BGM_Defeat` | 敗北 | P1 | 6-12秒 sting | コア停止、余韻は短く |

## 必要SE

| ID | 用途 | 優先 | 長さ | 方針 |
|---|---:|---:|---:|---|
| `SE_UI_Select` | ボタン決定 | P0 | 0.08-0.20秒 | 軽い電子クリック |
| `SE_UI_Back` | 戻る/閉じる | P0 | 0.08-0.20秒 | 低めのクリック |
| `SE_UI_Error` | 不可/不足 | P1 | 0.12-0.30秒 | 短い警告 |
| `SE_Shoot_Speed` | SPEED系射撃 | P0 | 0.06-0.14秒 | 高め、連射で濁らない |
| `SE_Shoot_Power` | POWER系射撃 | P0 | 0.10-0.22秒 | 低め、重い発射 |
| `SE_Shoot_Guard` | GUARD系射撃/リング | P1 | 0.10-0.25秒 | 防御的なパルス |
| `SE_Hit_Small` | 通常ヒット | P0 | 0.05-0.12秒 | 目立ちすぎない |
| `SE_Kill` | 通常撃破 | P0 | 0.08-0.22秒 | データ崩壊の短い粒 |
| `SE_Pickup_Data` | データチップ取得 | P0 | 0.05-0.12秒 | 小さく気持ちいい |
| `SE_Pickup_Heal` | 回復 | P1 | 0.12-0.35秒 | 柔らかい上昇音 |
| `SE_LevelUp` | レベルアップ | P0 | 0.40-0.90秒 | 明確な報酬音 |
| `SE_Evolve` | 進化 | P0 | 0.90-1.80秒 | 変身カットインの主役 |
| `SE_Fusion` | クロス進化 | P0 | 1.20-2.20秒 | 進化より重く、合体感 |
| `SE_RelicGet` | レリック獲得 | P1 | 0.50-1.20秒 | 希少感 |
| `SE_BossWarning` | ボス警告 | P0 | 0.50-1.20秒 | 警告だがうるさすぎない |
| `SE_CoreDamage` | コア被弾 | P0 | 0.18-0.45秒 | プレイヤーが即気づく |
| `SE_GameOver` | 敗北 | P1 | 1.00-2.00秒 | 低く短く |
| `SE_Clear` | クリア | P1 | 1.00-2.00秒 | 勝利ジングル |

## 推奨制作フロー

0. まず `Tools/GenerateStarterAudio.ps1` を実行して、第三者素材なしのスターター音源を自動生成する。
1. `docs/AUDIO_GENERATION_PROMPTS.md` のP0だけ生成または取得する。
2. 生成/取得したファイルを `Assets/Resources/Audio/_incoming/` に仮配置する。
3. 各ファイルごとに `docs/AUDIO_LICENSE_LOG_TEMPLATE.md` の行を埋める。
4. BGMはループ確認、SEは連射/重複再生確認をする。
5. 採用ファイルだけ `Assets/Resources/Audio/BGM/` と `Assets/Resources/Audio/SE/` へ移す。
6. Unity側で音量/ピッチ/同時発音数を調整する。

## ライセンス判断ルール

- **OK**: CC0、明示的に商用可の自作/生成音、Stable Audio 3.0 Community License条件内の生成物。
- **条件付きOK**: CC BY、DOVA-SYNDROME、Pixabay、Freesound CC BY。
- **基本NG**: NC、個人利用のみ、改変不可、Content IDが強いBGM、Suno Free、出所不明の転載音源。
- **記録必須**: URL、作者、ライセンス名、生成/ダウンロード日、プロンプト、利用サービス、証跡スクリーンショット。

## Codexでできること / できないこと

できる:

- 音素材の命名、仕様、プロンプト、ライセンス台帳の管理。
- ユーザーが置いた音声ファイルのUnity接続。
- 音量バランス、同時発音数、BGM切り替え、オプション保存の実装。
- CC0/商用可候補の調査と整理。

今すぐはできない:

- ログインが必要なAI音楽サービスでの直接生成。
- APIキーなしでStable AudioやEleven Musicを自動生成。
- 法的な保証。最終商用利用前には、各サービス/各素材ページのTerms保存が必要。
