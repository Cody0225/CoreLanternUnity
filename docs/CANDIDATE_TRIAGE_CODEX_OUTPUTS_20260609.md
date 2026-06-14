# Candidate Triage — Codex 出力候補（#1〜#10）（2026-06-09）

位置づけ: Codex が出力したキャラ候補素材の用途整理・分類。**採用確定ではない**。
状態: **本番PNG未上書き**。技術検収（数値）は**実ファイルパス入手後**に実施。
不変: 画像生成しない／PNG上書きしない／.meta・cs・Unity設定・Prefab/Scene・git 変更しない（本書は docs 保存のみ）。
基準: 品質ゲート13項目／`EVOLUTION_LINEAGE_DESIGN_S3PLUS_20260608.md`／各設計ブリーフ。

## 分類サマリ
| 分類 | 該当 |
|---|---|
| 強い採用候補 | #5, #7, #9 |
| 調整候補 | #1, #3, #4, #8 |
| 参考止まり | #2 |
| 将来保存 | #6 |
| 敵ボス候補 | #10 |
| NG（完全） | なし |

## 個別（#1〜#10）

### 強い採用候補
- **#5 青結晶の盾狼** → 仮割り当て: **S1 Cobalt Pup / F1 Nova Aegis 候補（最有力）**
  - 理由: 狼として明確・青結晶・前面の盾状クリスタル翼が本体接続・発光コア。Nova Aegis(前方反射盾)に合致。
  - 注意: リング無し。ほぼゲート適合。小表示識別の最終確認のみ。
- **#7 黒金城塞ベア** → 仮割り当て: **S6 Iron Bear GUARD L3 / F3 Core Bastion 候補**
  - 理由: 熊＋黒金装甲＋背の城塞＝要塞防御が噛み合う。色もIron Bear一致。
  - 注意: 背の塔が「建造物貼り付き」に寄らないか小表示で確認。
- **#9 紫猫** → 仮割り当て: **S4 Hex Cat 候補（最有力）**
  - 理由: 猫として明確（耳・ヒゲ・四脚・表情）、紫主役＋サイバー/魔術感。
  - 注意: 前回 S4 NG候補（`CODEX_PENDING_S4_HEXCAT_RETAKE_20260608.md`）の解決になり得る。採用時は同保留メモを更新。

### 調整候補
- **#1 青結晶の大型獣** → 仮割り当て: **S1 ULTIMATE系候補**
  - 注意: 胸の同心円発光＝魔法陣/UIリング風＋頭上ハロ＝ゲート「魔法陣・リング」抵触。**リング/ハロ除去が前提**。F3 Core Bastion(低重心・背甲の全身要塞)とは方向差→用途再検討。
- **#3 紫霊狼** → 仮割り当て: **F6 Photon Wraith 寄り**
  - 注意: 霊影・青紫のWraith感は強い。**S7 Wraith Lynx に使うなら山猫化必須**（房耳・短尾・山猫顔）。
- **#4 緑要塞狼** → 仮割り当て: **Core Bastion 方向**
  - 注意: 守護・要塞・獣は良いが**色が緑＝S1 Cobalt Pup(青)と不一致**。S1に使うなら青系へ調整、または緑系素体向けに整理。背の城＋側面の円形ギア(リング気味)も要整理。
- **#8 紫霊体獣** → 仮割り当て: **F6 Photon Wraith 寄り**
  - 注意: Wraith/Phase感は強いが**生体シルエットが弱い**＋炎ウィスプ/散り際の小四角ノイズが「粒子/効果」寄り。本体造形を起こす整理が必要。

### 参考止まり
- **#2 石壁ゴーレム** → Partner用途ではなく**要塞質感の参考**。
  - 注意: 人型・ロボット寄りで相棒（動物）感が薄い＋胸の同心円コア(リング)。本体採用は不適。

### 将来保存（preserved）
- **#6 白青の浮遊ビット体** → **S8 Genesis Core / S9 Halo Caster / 高次コア・ビット系の将来候補**
  - 注意: 多数の浮遊ダイヤ＋ハロリング＋中央の魔法陣風サークル＝ゲート「浮遊装飾/魔法陣/リング」に強く抵触。生き物=Corebornでない。採用には**本体接続のビット器官＋生体シルエットへ再設計**が前提。**今は初期6体に割り当てない・保存のみ**。

### 敵ボス候補
- **#10 黒い虚無コア体** → **Partner進化候補から除外。Void系/深淵系の敵ボス候補**。
  - 注意: 人型＋胸の黒い虚無コア＝相棒進化体に不適。敵ボスとしては良い。Partnerパイプライン外＝敵素材の命名/反映は別途設計が必要。

## 技術検収結果（2026-06-09 実施・読み取りのみ）

**判定: #1〜#10 全件 format NG（本番反映不可）。** 絵柄の用途分類は維持。

NG理由（全10枚共通）:
- サイズ **1254×1254**（規定 512×512 でない）
- **背景透過なし**（四隅 alpha=255 / 外周2px 不透明=10016 / 被覆100%）
- bbox 全面 → 背景を抜くまで中央配置/ズレは判定不能

用途分類（維持）:
- 強い採用候補: #5 / #7 / #9
- 調整候補: #1 / #3 / #4 / #8
- 参考止まり: #2
- 将来保存: #6
- 敵ボス候補: #10

対応:
- **採用前に Codex で「512×512 / RGBA / transparent background / 四隅alpha0 / 外周2px clear / 中央配置」を満たす再書き出しが必須**（依頼文: `docs/CODEX_CANDIDATE_TRANSPARENT_512_REEXPORT_REQUEST_20260609.md`）。
- **自動背景抜きは原則行わない**（縁残り・輪郭劣化・エフェクト境界破綻リスク）。Codex側で正規透明PNGとして再書き出し。
  - ※例外: **チェッカー背景焼き込み×高彩度素材**に限り、Claude側の自動透過（彩度+明度+外周フラッドフィル+最近傍インペイント）が綺麗に通る実績あり（S4紫猫=`Partner_S4_R1_L2`、本#5青結晶盾狼=`Partner_S1_F1_L3`）。**適用時は素材ごとに緑/黒/白合成＋高リスク部位の視覚検証を必須**とする。破綻が出る素材はCodex再書き出しに回す。
- 本番PNG上書きは「再書き出し or TECH_FIX_PASS→技術検収合格→採用確定」の後のみ（同名上書き・.meta温存・原本バックアップ）。

## 採用記録

### 2026-06-13 #5 採用 — S1 Cobalt Pup / F1 Nova Aegis / ULTIMATE 本番反映済み

候補: **#5 青結晶の盾狼**（`candidate_05_blue_crystal_shield_wolf.png`）→ 採用ID **`CODEX_05_BLUE_CRYSTAL_SHIELD_WOLF`**。

TECH_FIX（Claude後処理・自動透過）: **TECH_FIX_PASS**
- raw: 1254×1254 / RGBA だが全面不透明＝**チェッカー背景焼き込み**（四隅240–255灰）。
- 整形後: **512×512 / RGBA / transparent / 四隅alpha0 / 外周2px漏れ0(max0)**。
- 緑/黒/白合成でハロー・灰フチなし。**白い鬣・白い足先/爪・結晶盾翼・発光マゼンタコア・結晶冠すべて破綻なく保持**（内部白明部78,110px保護）。
- techfix版: `tmp_diag/candidates_hold/CODEX_05_BLUE_CRYSTAL_SHIELD_WOLF_TECHFIX_512.png`。

品質ゲート: **Adopt**。F1 Nova Aegis の「前方防御・盾・反射・青結晶」テーマが既存F1_L3より明確。ULTIMATEとしての完成度・荘厳さが高い。盾/結晶/装甲は本体統合（浮遊リング・背景・魔法陣・粒子なし）。13項目NG該当なし。
- **配色判断**: 金トリムは既存F1系統(L0–L3)に少ない新要素だが、**ULTIMATEの格上げ表現（盾・反射・荘厳さ強化）として許容**（ユーザー承認）。

本番反映: **完了**
- 反映先: `Assets/Resources/Skins/Partner_S1_F1_L3.png`（同名上書き）。
- 旧F1_L3（512×512, 289,658 bytes, sha CCE41810…A7A4）→ バックアップ `_backup_skins/20260613_S1/Partner_S1_F1_L3.png`（hash一致で退避確認）。
- 上書き後検収: 512×512 / RGBA / 四隅alpha0 / 外周2px漏れ0(max0) / bbox x[7..505] y[29..487]・中心dx0,dy+2 / sha **E3630920…B69E**（candidate一致）。
- `.meta` 未編集（hash・mtime ともに上書き前後で不変を確認）。
- `Partner_S1_F1_L3.png` 以外の本番PNG・`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git は未変更／画像生成なし。

残課題（#5関連）:
- 今回 **F1 Nova Aegis ULTIMATE(F1_L3)** へ確定採用。S1素体側(L0–L2)への流用は別途検討（今回は未実施）。

### 2026-06-14 #7 採用 — S6 Iron Bear / F3 Core Bastion / ULTIMATE 本番反映済み

候補: **#7 黒金城塞ベア**（`candidate_07_black_gold_fortress_bear.png`）→ 採用ID **`CODEX_07_BLACK_GOLD_FORTRESS_BEAR`**。

route/stage 判定: **S6 R3 GUARD L3 ではなく S6 F3 Core Bastion / ULTIMATE**。
- 理由: 背中の**実体城塞**・黒金重装甲・防御要塞感は通常L3には豪華すぎ、**Core Bastion（城塞）テーマに直結**。現F3「金バリアドーム」より「城塞・防御・重装甲」の読みが強い。

TECH_FIX（Claude後処理・自動透過）: **TECH_FIX_PASS**
- raw: 1254×1254 / RGBA だが全面不透明＝**チェッカー背景焼き込み**（四隅≈236灰）。
- 整形後: **512×512 / RGBA / transparent / 四隅alpha0 / 外周2px漏れ0(max0)**。bbox x[3..499] y[15..476]・中心 dx-5,dy-10.5（城塞で上重心・被写体ほぼ全面。フレーミングは現版のまま採用＝一律縮小なし、ユーザー承認）。
- 緑/黒/白合成でハロー・灰フチなし。**金縁・城塞の発光窓・クレネル/細い凹凸・足先/金の爪・胸コア(amberヘックス)・肩/脚の紋章(H)・黒装甲の縁すべて破綻なく保持**（塔間ギャップも正しく透過）。
- techfix版: `tmp_diag/candidates_hold/CODEX_07_BLACK_GOLD_FORTRESS_BEAR_TECHFIX_512.png`。

品質ゲート: **Adopt**。13項目NG該当なし。荘厳・高密度でULTIMATE級。S6 Iron Bear系（黒+金+amber）として整合。

本番反映: **完了（方向転換: 金バリアドーム → 実体城塞）**
- 反映先: `Assets/Resources/Skins/Partner_S6_F3_L3.png`（同名上書き）。
- **方向転換理由**: Core Bastion の「城塞・防御・重装甲」テーマにより強く一致するため（ユーザー承認）。
- 旧F3_L3（256×256, 102,210 bytes, sha F690B89A…2E4A）→ バックアップ `_backup_skins/20260613_S6/Partner_S6_F3_L3.png`（hash一致で退避確認）。
- 上書き後検収: 512×512 / RGBA / 四隅alpha0 / 外周2px漏れ0(max0) / bbox x[3..499] y[15..476]・中心dx-5,dy-10.5 / sha **24169120…E70E**（candidate一致）。
- `.meta` 未編集（hash・mtime ともに上書き前後で不変を確認）。
- `Partner_S6_F3_L3.png` 以外の本番PNG・`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git は未変更／画像生成なし。

残課題（#7関連）:
- `Partner_S6_R3_L3`（GUARD L3）は **2026-06-14 に512化・採用済み**（下記参照）。
- 比較画像: `tmp_diag/S6_route_compare_07.png`。

### 2026-06-14 S6 R3 GUARD L3 採用 — 本番反映済み（#7 F3 ULTIMATE とは別枠）

候補ID: **`S6_R3_L3_RAW_VISUAL`**（Codex raw。#1〜#10 triage とは別系統）。
- raw: `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0e608735445d19f8016a2e752ccdd081919242c7a24df81f51.png`（**1402×1122 / RGB / 非透過＝チェッカー焼き込み・非正方形**）。

route/stage: **S6 Iron Bear / R3 GUARD / L3（通常進化最終）**。
- 黒鉄/濃灰の重装甲熊、肩・胸・前腕の防御プレート、低重心の守備姿勢、シアン発光。**城塞・塔なし＝通常L3として一段控えめ**。発光はシアンで、ULTIMATE #7（amber/金の城塞）と差別化。

TECH_FIX: **TECH_FIX_PASS**
- **非正方形のためアスペクト比保持で中央配置**（512へ直接ストレッチせず bbox crop→fit→center。歪み回避）。
- 検収: 512×512 / RGBA / 四隅alpha0 / 外周2px漏れ0(max0) / bbox x[10..501] y[86..424]・中心dx-0.5,dy-1.0。
- 緑/黒/白合成でハロー・灰フチなし。**耳・顔・シアン目・胸コア・肩/前腕装甲・金縁・爪・足先・黒毛の縁・シアン発光ラインすべて健在**。
- techfix版: `tmp_diag/candidates_hold/S6_R3_L3_RAW_VISUAL_TECHFIX_512.png`。

本番反映: **完了**
- 反映先: `Assets/Resources/Skins/Partner_S6_R3_L3.png`（同名上書き）。
- 旧本番（256×256, 93,363 bytes, sha BEFEC0F0…8126）→ `_backup_skins/20260613_S6/Partner_S6_R3_L3.png`（hash一致で退避）。
- 上書き後 sha **2F9A5661…78F8**（candidate一致）。`.meta` 未編集（hash・mtime不変）。
- **`Partner_S6_F3_L3.png` は未変更**（hash `24169120…E70E` のまま、別枠ULTIMATE）。R3_L3 以外の本番PNG・`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git は未変更／画像生成なし。

### 2026-06-14 S6 R1 SPEED L3 採用 — 本番反映済み

候補ID: **`S6_R1_L3_RAW_VISUAL`**（Codex raw。#1〜#10 triage とは別系統）。
- raw: `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0e608735445d19f8016a2e7a5a484881918f7019c8bdd1cc4c.png`（**1402×1122 / RGB / 非透過＝チェッカー焼き込み・非正方形**）。

route/stage: **S6 Iron Bear / R1 SPEED / L3（通常進化最終）**。
- 黒鉄装甲の熊＋**青発光ライン＋後方空力ブレード＋低い前傾の突進姿勢＋長い青爪**。R3 GUARD（重防御・安定姿勢・ティール発光）と姿勢/装甲量で明確に差別化。F3 Core Bastion ULTIMATE（城塞・黒金・巨躯）より明確に格下＝通常L3として成立。

TECH_FIX: **TECH_FIX_PASS**
- **非正方形のためアスペクト比保持で中央配置**（512へ直接ストレッチせず bbox crop→fit→center。歪み回避）。
- 検収: 512×512 / RGBA / 四隅alpha0 / 外周2px漏れ0(max0) / bbox x[10..501] y[91..420]・中心dx-0.5,dy-0.5。
- 緑/黒/白合成でハロー・灰フチなし。**黒毛外縁・青発光ライン・背中ブレード・前脚装甲・爪・顔まわりすべて健在**。
- techfix版: `tmp_diag/candidates_hold/S6_R1_L3_RAW_VISUAL_TECHFIX_512.png`。差別化比較: `tmp_diag/S6_R1_vs_R3_vs_F3.png`。

本番反映: **完了**
- 反映先: `Assets/Resources/Skins/Partner_S6_R1_L3.png`（同名上書き）。
- 旧本番（93,904 bytes）→ `_backup_skins/20260613_S6/Partner_S6_R1_L3.png`（hash一致で退避）。
- 上書き後 sha **83F7B97C…6745**（candidate一致）。`.meta` 未編集（hash・mtime不変）。
- **`Partner_S6_R3_L3.png`（GUARD）・`Partner_S6_F3_L3.png`（ULTIMATE）は未変更**（hash不変で確認）。R1_L3 以外の本番PNG・`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git は未変更／画像生成なし。

#### S6 Iron Bear ルート整理（2026-06-14 時点）
- **R1 SPEED L3** = 黒鉄・青発光・空力ブレードの突進型（512・採用済み）。
- **R3 GUARD L3** = 黒鉄・ティール発光の重防御型（512・採用済み）。
- **F3 Core Bastion / ULTIMATE** = 黒金・城塞の要塞型（512・採用済み）。
- 3枠とも512化済みで route/tier 差別化が成立。

## 関連
- `docs/ART_PIPELINE_INDEX.md` / `docs/EVOLUTION_LINEAGE_DESIGN_S3PLUS_20260608.md`
- `docs/CODEX_S7_S8_REDESIGN_BRIEF_20260608.md` / `docs/CODEX_S9_S10_S11_DESIGN_BRIEF_20260608.md`
- `docs/CODEX_PENDING_S4_HEXCAT_RETAKE_20260608.md`（#9 で解決見込み）
- `docs/REGEN_PRIORITY_S3PLUS_20260608.md`
