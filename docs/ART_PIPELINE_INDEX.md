# Art Pipeline Index（キャラ素材パイプライン 索引）（2026-06-08）

目的: Codex 復帰後 / Claude 作業時に「どの doc を見ればよいか」で迷わないための入口。
役割分担: 生成＝Codex/外部、検収・反映＝Claude（合格後に同名上書き・.meta温存）。

> ⚠️ Codex は作業開始前に必ず `HANDOFF_FOR_CODEX.md` を全文読む（プロジェクト最優先ルール）。

## 🚩 Codex 復帰後の「最初の作業」
- **S3 Sage Hare の route L3 三分岐（R1/R2/R3 の L3 = 3枚）プレビューから開始。**
- **36枚一括生成・一括本番反映は禁止。** 素体単位・段階承認制。
- **L3 三分岐が合格したら、L2 → L1 を L3 から逆算**して連続性を担保。
- 準拠: `EVOLUTION_LINEAGE_DESIGN_S3PLUS_20260608.md` / `REGEN_PRIORITY_S3PLUS_20260608.md`。
- 候補は Temp 出力＋ size/四隅alpha/外周漏れ/透過漏れ を数値自己報告 → Claude が検収。

## 索引（用途別）
| 区分 | doc | 用途 / 現状 |
|---|---|---|
| 用語の正 | `docs/CODEX_TERMINOLOGY_COREBORN_20260604.md` | Coreborn / Link Module / 究極融合 の最上位用語 |
| S2 通常進化（現状） | （状態メモ）下記「S2通常進化の現状」参照 | 10枚反映済み。Unity一括確認は後日（チェックリスト参照） |
| S2 ULTIMATE 設計 | `docs/CODEX_S2_EMBERDRAKE_ULTIMATE_ART_BRIEF_20260608.md` | S2 F1–F6 融合アートの生成仕様＋受け入れ基準。Codex復帰後に生成 |
| S3以降 進化系統設計 | `docs/EVOLUTION_LINEAGE_DESIGN_S3PLUS_20260608.md` | テーマ/R1·R2·R3役割/L0–L3成長ルール（S3–S6確定・S7–S11拡張） |
| S7/S8 再設計ブリーフ | `docs/CODEX_S7_S8_REDESIGN_BRIEF_20260608.md` | S7 Wraith Lynx / S8 Genesis Core の512再生成前ブリーフ。S7=霊影の山猫、S8=生体コア万能型のテーマ・ルート分岐・成長ルール・NG要素 |
| S9–S11 新規設計ブリーフ | `docs/CODEX_S9_S10_S11_DESIGN_BRIEF_20260608.md` | S9 Halo Caster / S10 Pulse Hydra / S11 Solar Anchor の新規制作前ブリーフ。S9=接続ビット器官、S10=レーザー器官ヒドラ、S11=共鳴器官内包型Coreborn のテーマ・ルート分岐・成長ルール・NG要素 |
| 進化データ正本 | `docs/EVOLUTION_DATA_MASTER.md` | S1–S6 の54マトリクス（個別フォーム名）正本 |
| S3〜S6 QAレポート | `docs/QA_REPORT_S3_S6_PNG_20260608.md` | route36枚が256＝再生成対象。S5_L0は白毛で修正不要 |
| Partner 素材棚卸し | `docs/ASSET_INVENTORY_SKINS_20260608.md` | 総数296/512=38/256=258、素体別、再生成候補の全体像 |
| Unity 確認チェックリスト | `docs/UNITY_VISUAL_CHECK_CHECKLIST_20260608.md` | S1一括/S2 10枚/S3–S6再生成後の目視手順＋共通7項目 |
| 再生成 優先順位 | `docs/REGEN_PRIORITY_S3PLUS_20260608.md` | L3→L2→L1→L0→fusion→S7/S8→S9–S11 の順 |
| S3停止/S4移行メモ | `docs/S3_STOP_AND_S4_HANDOFF_20260612.md` | S3 Sage Hare は L2→L3差分が弱く試作停止。高品質S3 L3案はHOLD、L3+もHOLD、L2/L1は未作成。次はS4 Hex Catの候補整理へ移行 |
| S4 Hex Cat 保留 | `docs/CODEX_PENDING_S4_HEXCAT_RETAKE_20260608.md` | NG/保留。Codex復帰後にリテイク指示を渡す |
| Codex image_gen 不具合メモ | `docs/CODEX_IMAGEGEN_POSTER_FAILURE_20260611.md` | S3 L2生成で `PROBATION vs. PRISON` / `EUKARYOTIC ANIMAL CELL` など無関係ポスター化が複数回発生。Codex built-in image_gen は当面 Eggcore キャラスプライト生成に使わず、外部生成画像を受け取って Claude/Codex 側で透過512整形・技術検収する |
| 候補トリアージ | `docs/CANDIDATE_TRIAGE_CODEX_OUTPUTS_20260609.md` | Codex出力候補 #1〜#10 の用途整理・分類（強い採用/調整/参考止まり/将来保存/敵ボス候補）。技術検収は実ファイルパス入手後 |
| 候補 透過512 再書き出し依頼 | `docs/CODEX_CANDIDATE_TRANSPARENT_512_REEXPORT_REQUEST_20260609.md` | 候補 #1〜#10 の format NG 対応。1254×1254・不透明背景を 512×512/RGBA/透過/四隅alpha0/外周2px clear/中央配置 で再書き出し依頼 |
| 融合レシピ | `docs/CROSS_EVOLUTION_RECIPE_MASTER.md` | F1–F6 のリンク対・ゲート・ファンタジー |
| 図鑑データ | `docs/CODEX_DATA_MASTER.md` | 素体/ルート/融合の codex 仕様 |

## S2 通常進化の現状（状態メモ）
- 通常進化10枚（L0 / R1·R2·R3 × L1·L2·L3）は反映済み。
- R3_L1 は候補B（仮版）を反映済み。Unity 一括目視確認は後日（チェックリストの S2 節で実施）。
- S2 ULTIMATE（F1–F6）は未生成 → 上記 S2 ULTIMATE 設計ブリーフで Codex 復帰後に生成。

## 品質ゲート（全生成共通・要点）
- 512×512 / RGBA / 四隅alpha=0 / 外周透過漏れ0 / 中央配置 / 小表示で識別可能 / 256引き伸ばし禁止。
- 13項目NG（幼稚/見劣り/色替え/量産/図形貼付/UI装飾/浮遊装飾/粒子・線/魔法陣・リング/背景影地面/透過漏れ/小表示不可読）に1つでも該当＝不採用。
- ルート差は色でなく 体型・姿勢・武器/装甲器官・翼/尾/頭部シルエット。

## 反映ルール（Claude側）
- 検収合格のみ `Partner_S{n}_R{r}_L{l}.png` 等へ同名上書き。.meta は触らない。
- 上書き前に原本をバックアップ（`_backup_skins/<日付>/`）。
- CoreLanternGame.cs / Unity設定 / Prefab / Scene / git は別途指示があるまで触らない。
