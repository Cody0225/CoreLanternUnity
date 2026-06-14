# 【保留メモ】S4 Hex Cat 候補 リテイク指示（Codex 復帰後用）

状態: **NG / 未採用（保留）**。Codex は 6/11 まで不可のため、**Codex 復帰後に実行**する保留タスク。
本番PNG・.meta・CoreLanternGame.cs・Unity設定・git は変更しない（このメモは docs のみ）。

## 候補の問題点（整理）
1. 緑の発光が「ノイズ/汚れ」に見える（散っている・むら）。
2. 黒い胴体が潰れて面・装甲ラインが読めない。
3. 小さいUI表示で 顔・胴体・脚・尾 が判別しづらい。
4. 黄色い頬/尾先が強すぎ、Hex Cat の主役色（紫/黒/魔術感）が弱い。
5. 方向性はOKだが、S1/S2 基準の商用品質に未達。

## 目的
S4 Hex Cat の素体として、**かわいい魔猫感を残しつつ S1/S2 と同水準の商用品質**にする。

## Codex 修正依頼文（復帰後にそのまま渡す）
```
【依頼】S4 Hex Cat 候補のリテイク（方向性は維持・商用品質へ）
前提: HANDOFF_FOR_CODEX.md を読了。spec は
 docs/EVOLUTION_LINEAGE_DESIGN_S3PLUS_20260608.md（S4=連鎖型・紫の魔猫）/
 docs/EVOLUTION_DATA_MASTER.md / 品質ゲート13項目 に準拠。
目的: かわいい魔猫感を残しつつ S1/S2 と同水準の商用品質。

修正(必須):
1. 緑発光が汚れ/ノイズに見える→要所だけに整理し面で置く。にじみ・点ノイズ・むら除去。
2. 黒胴体の潰れ→陰影とハイライトで面・装甲ラインを起こし立体に。黒ベタ潰れ回避。
3. 小UI(〜76–136px)でも顔・胴体・脚・尾が判別できるシルエット/コントラスト。
4. 黄色い頬/尾先を弱め、主役を紫・黒・魔術感に。黄は弱い補助のみ。
5. 追加禁止: 背景・影・粒子・魔法陣・UIリング・浮遊装飾・スピード線。

技術: 512×512/RGBA/四隅alpha0/外周漏れ0/中央/256引き伸ばし禁止/種(頭・目・紫基調・発光コア)継承。
提出: まず修正版1枚→合格後 R1/R2/R3 の L3三分岐へ。候補はTempへ、size/四隅/漏れ を数値自己報告。
本番上書き・.meta・cs・Unity・Prefab・Scene・git は触らない（Claudeが合格後に同名上書き）。
```

## 再開トリガー
- Codex 復帰（6/11以降）→ 上記依頼文を渡す → 修正版候補のパスを Claude へ → Claude が品質ゲート＋技術検収 → 合格のみ反映。

## 2026-06-12 smoke test 記録（Codex built-in image_gen）

目的: Codex built-in image_gen が S4 Hex Cat の単体キャラスプライト候補を出せるか確認。

結果:
- 候補ID: **S4_R1_L3_RAW_VISUAL_20260612_01**
- 判定: **視覚HOLD / TECH_FIX_REQUIRED**
- 出力: `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d741c5e4a202530016a2c1d43a80c8198accc486325c04a3b.png`
- 視覚: ポスター化なし、1体キャラスプライト、S4 Hex Cat / R1 SPEED 方向としては良い。
- 技術: 1254x1254 / RGB / 非透明 / チェッカー背景焼き込み / 四隅 alpha 255 / 外周2px alpha 255。
- 本番影響: 本番PNG未変更、`Assets/Resources/Skins` 未保存、`.meta` 未編集、`CoreLanternGame.cs` 未編集、Unity設定・Prefab・Scene・git 未操作。

運用判断:
- 透明512本番PNGを直接作る用途ではまだ失敗扱い。
- ただしポスター化せず単体キャラスプライトを出せたため、**視覚候補作成用途では1回ずつ継続テスト可**。
- 良い見た目が出た場合は、サイズ/RGB/非透明/チェッカー背景焼き込みだけを理由に破棄せず、**raw visual candidate / 視覚HOLD / TECH_FIX_REQUIRED** として記録する。
- 本番反映・正式candidate保存は Claude の透過512整形・技術検収後。

## 次候補プロンプト方針（S4 R1 SPEED L3）

狙い:
- 前回の「猫として良いが技術NG」方向を維持しつつ、よりゲームスプライト候補に寄せる。
- ポスター化防止のため、画像内に文字・比較・資料・UI・説明要素を入れないことを強く指定する。
- 透明512は built-in では信用しきらず、見た目評価を優先する。

改善プロンプト要点:
- exactly one standalone creature sprite
- one full-body purple cyber-mystic cat only
- S4 Hex Cat / R1 SPEED / L3 final normal evolution
- lean agile feline silhouette, long expressive tail, sharp cat ears, intelligent dangerous eyes
- purple / black / magenta palette, subtle integrated hex/chain motifs on the body
- light armor only, fast route, not bulky, not guard/power
- modern high-resolution pixel-art inspired indie game sprite
- no poster, no concept sheet, no comparison, no chart, no text, no labels, no UI, no background, no floor, no shadow, no multiple characters

次の判定:
- ポスター/教材図/比較表/文字入り/複数体/背景付きなら即NGで停止。
- 単体キャラとして良ければ、技術仕様NGでも **raw visual candidate / 視覚HOLD / TECH_FIX_REQUIRED** としてこのメモへ記録。

## 2026-06-13 次候補生成結果（S4 R1 SPEED L3）

目的: 6/12 smoke test を踏まえ、Codex built-in image_gen を「視覚候補作成」用途で1回だけ継続テスト。

出力:
- 候補ID: **S4_R1_L3_RAW_VISUAL_20260613_01**
- `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d741c5e4a202530016a2c1f8f857c8198b9fff0cc406dbe84.png`

視覚評価:
- ポスター化なし。
- 1体のみ。
- 文字・ラベル・UIなし。
- S4 Hex Cat / R1 SPEED / L3 として読める。
- 前回より前傾姿勢・細身・俊敏さが強く、SPEEDルート感は改善。
- 紫/黒/マゼンタの主役色が明確で、猫耳・猫顔・長い尾も維持。
- 目と顔の視認性は良好。小表示でもシルエットは比較的読みやすそう。

技術検収:
- size: 1254x1254（NG。512x512ではない）
- mode: RGB（NG。RGBAではない）
- corner alpha: [255,255,255,255]（NG。透明ではない）
- outer 2px max alpha: 255（NG。外周透明ではない）
- 背景: チェッカー焼き込み（NG）

判定:
- **視覚HOLD / TECH_FIX_REQUIRED**
- 本番反映不可。透明512化・背景除去・再検収は後工程。
- 本番PNG未変更、`Assets/Resources/Skins` 未保存、`.meta` 未編集、`CoreLanternGame.cs` 未編集、Unity設定・Prefab・Scene・git 未操作。

## 2026-06-13 追加候補生成結果（S4 R1 SPEED L3）

目的: 方針更新後、技術仕様NGだけでは破棄せず、S4 Hex Cat / R1 SPEED / L3 の raw visual candidate を追加確認。

出力:
- 候補ID: **S4_R1_L3_RAW_VISUAL_20260613_02**
- `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d741c5e4a202530016a2c214d74f88198ad6b63abf3e5f60b.png`

視覚評価:
- ポスター化なし。
- 1体のみ。
- 文字・ラベル・UIなし。
- S4 Hex Cat / R1 SPEED / L3 として読める。
- 前傾姿勢・細身・大きい尾で SPEED ルート感がある。
- 猫耳・猫顔・ひげ・目が読みやすく、S4の紫/黒/マゼンタ主役色も明確。
- 20260613_01 と近い方向だが、こちらは顔と猫らしさがやや強く、比較候補として保持価値あり。

技術検収:
- size: 1254x1254（TECH_FIX_REQUIRED）
- mode: RGB（TECH_FIX_REQUIRED）
- corner alpha: [255,255,255,255]（TECH_FIX_REQUIRED）
- outer 2px max alpha: 255（TECH_FIX_REQUIRED）
- 背景: チェッカー焼き込み（TECH_FIX_REQUIRED）

判定:
- **視覚HOLD / TECH_FIX_REQUIRED**
- 本番反映不可。Claude 側で 512x512 / RGBA / transparent background / alpha 検収が必要。
- 本番PNG未変更、`Assets/Resources/Skins` 未保存、`.meta` 未編集、`CoreLanternGame.cs` 未編集、Unity設定・Prefab・Scene・git 未操作。

## 2026-06-13 正式視覚HOLD候補（Claude後処理へ渡す1枚）

正式候補:
- 候補ID: **S4_R1_L3_RAW_VISUAL**
- 状態: **HOLD / TECH_FIX_REQUIRED**
- raw画像パス: `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d741c5e4a202530016a2c214d74f88198ad6b63abf3e5f60b.png`

採用理由:
- ポスター化なし。
- 1体キャラスプライト。
- 文字・ラベル・UIなし。
- S4 Hex Cat / R1 SPEED / L3 として読める。
- 猫耳・猫顔・ひげ・目・長い尾が明確で、20260613_01 より猫らしさが強い。
- 紫/黒/マゼンタの主役色が明確で、SPEEDルートの前傾・細身・高機動感がある。

技術状態:
- **TECH_FIX_REQUIRED**
- 1254x1254 / RGB / 非透明 / チェッカー背景焼き込み。
- サイズ変更、RGBA化、透明化、alpha検収は Claude 後処理対象。

候補整理:
- `S4_R1_L3_RAW_VISUAL_20260612_01`: 履歴・参考のみ。正式候補ではない。
- `S4_R1_L3_RAW_VISUAL_20260613_01`: 履歴・参考のみ。正式候補ではない。
- `S4_R1_L3_RAW_VISUAL_20260613_02`: **正式候補へ統合**し、以後は `S4_R1_L3_RAW_VISUAL` と呼ぶ。

次アクション:
- Codexでこれ以上連続生成しない。
- Claudeへ渡し、512x512 / RGBA / transparent background / alpha検収が可能か確認する。
- 本番反映は Claude の技術検収合格後のみ。

禁止・未実施:
- 本番PNG未変更。
- `Assets/Resources/Skins` 未保存。
- `.meta` 未編集。
- `CoreLanternGame.cs` 未編集。
- Unity設定・Prefab・Scene・git 未操作。

## 2026-06-13 Claude技術補正結果（S4_R1_L3_RAW_VISUAL）

判定: **TECH_FIX_PASS / candidate HOLD**（技術補正成功。ただし本番採用はまだしない）。

raw画像（入力）:
- `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d741c5e4a202530016a2c214d74f88198ad6b63abf3e5f60b.png`
- **1254×1254 / RGB / 非透過 / チェッカー背景焼き込み**（四隅≈241灰、alphaチャンネル無し）。

技術補正版（出力・candidate HOLD）:
- ファイル名: `S4_R1_L3_RAW_VISUAL_TECHFIX_512.png`
- 保存先（プロジェクト内 candidate HOLD 置き場）: `CoreLanternUnity/tmp_diag/candidates_hold/S4_R1_L3_RAW_VISUAL_TECHFIX_512.png`
- temp元: `%LOCALAPPDATA%\Temp\Eggcore_S4_TechFix_20260613\S4_R1_L3_RAW_VISUAL_TECHFIX_512.png`

Claude後処理での検収結果:
- サイズ: **512×512**（PASS）
- 形式: **RGBA**（PASS）
- 四隅alpha: **0 / 0 / 0 / 0**（PASS）
- 外周2px漏れ: **0px（alpha最大0）**（PASS）
- bbox: x[10..494] y[20..486]（w485×h467）／中心ズレ dx -4, dy -3px＝ほぼ中央（良好）
- エッジ: 半透明AA 13,206px、灰色ハロー・縁残りなし
- 視覚検証: 緑/黒/白の単色背景に合成して確認。**耳・房毛・ヒゲ・尾の刃/リング・足/爪・発光ライン（マゼンタ）すべて破綻なし**。

手法（再現用メモ）:
- 彩度＋明度で「白/灰チェッカー」候補を抽出 → 外周フラッドフィルで**外側背景のみ**除去（本体内の明部は保持）。
- 背景RGBを最近傍の本体色でインペイントしてから512へ縮小 → 縮小時の灰色ハローを防止。

状態:
- **candidate HOLD。本番反映はまだしていない。** 正式採用フロー（品質ゲート13項目＋設計ブリーフ照合→原本バックアップ→同名上書き）には未着手。
- 本番PNG未変更、`Assets/Resources/Skins` 未保存、`.meta` 未編集、`CoreLanternGame.cs` 未編集、Unity設定・Prefab・Scene・git 未操作。

## 自動透過（チェッカー背景除去）の運用メモ

- 今回の **S4 紫猫（高彩度・チェッカー背景焼き込み）** では、Claude側の自動透過処理が綺麗に成功した（ハローなし・細部破綻なし）。
- ただし**全素材に自動適用可能とはみなさない**。彩度の低い素体、白/灰系の本体、淡色エフェクトを多く含む素材では誤除去・縁残り・輪郭劣化のリスクが残る。
- 候補 **#1〜#10**（`CODEX_CANDIDATE_TRANSPARENT_512_REEXPORT_REQUEST_20260609.md`）等へ応用する場合も、**素材ごとに「緑/黒/白背景への合成」＋「高リスク部位（耳・ヒゲ・尾・足先・発光ライン・輪郭）の視覚検証」を必須**とする。
- 視覚検証で少しでも破綻が出る素材は自動透過を採用せず、Codex側での透明512再書き出しに回す（従来方針を維持）。

## 2026-06-13 本番反映（L2として採用・L3ではない）

> ⚠️ **後日訂正（2026-06-14）**: 本節で「S4 R1 SPEED L2」として採用した画像（`S4_R1_L3_RAW_VISUAL_TECHFIX_512.png` / hash `8617478F…`）は、その後のレビューで「低姿勢・装甲・機械尾の圧が強くPOWER寄り」と判断され、**R2 POWER L2 へ route 再分類**された（取り違えではなく意図した再配置）。現行の **R1 SPEED L2 は別の STANDING 版**（hash `A0706CE1…`）。確定マッピングは本書末尾「2026-06-14 S4 L2三分岐 最終整理（完了）」を参照。

判定: **Adopt → 本番反映済み（L2）**。

ステージ再分類:
- 当初は **S4 R1 SPEED / L3 final** 候補として検討（`S4_R1_L3_RAW_VISUAL` → TECH_FIX_PASS）。
- 品質ゲート確認の結果、**L3 final としては完成感がやや弱い**が、**細身・俊敏・猫感・紫Hex Cat感があり R1 SPEED の中間進化（L2）として適切**と判断。
- よって **L3採用はせず、S4 R1 SPEED / L2 として採用**へ再分類。
- 候補ID（採用後）: **`S4_R1_L2_ADOPTED_FROM_RAW_VISUAL`**（元: `S4_R1_L3_RAW_VISUAL` / techfix版 `S4_R1_L3_RAW_VISUAL_TECHFIX_512.png`）。

今後のステージ設計方針:
- **S4 R1 L3** は今後、この L2 を起点に**さらに成熟・完成形へ上位設計**する（このL2より装甲・尾・発光の完成度を上げる）。
- **S4 R1 L1** はこの L2 から逆算し、**より小型・軽装・未完成**にする。

反映内容:
- 反映先: `Assets/Resources/Skins/Partner_S4_R1_L2.png`（同名上書き）
- 旧本番（256×256, 81,353 bytes）→ バックアップ `_backup_skins/20260613_S4/Partner_S4_R1_L2.png`（hash一致で退避確認）
- 上書き後検収: **512×512 / RGBA / 四隅alpha 0 / 外周2px漏れ 0(max0) / bbox x[10..494] y[20..486]・中心dx-4,dy-3 / sha256 8617478F…E483821（candidate一致）**
- `.meta` は**未編集**（hash・mtime ともに上書き前後で不変を確認）。

未変更（重要）:
- `Partner_S4_R1_L3.png` は**未変更**（256×256, 90,526 bytes のまま）。
- `Assets/Resources/Skins` の他ファイル・`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git は未変更。

## 2026-06-13 S4 R1 SPEED L3 設計（新L2からの上位進化）— プロンプト設計のみ

> 🚫 **不使用（DEPRECATED / 2026-06-14 確定）**: 本節（v1）は**旧R1想定**（起点L2＝旧8617478F、現在は R2 POWER L2）で書かれている。**今後このv1プロンプトは使用しない。** S4 R1 L3 を作る場合は、現行 **R1 SPEED L2＝STANDING版（A0706CE1…）を起点に新規設計**する。詳細は本書末尾「S4 L2三分岐 最終整理（完了）」を参照。

状態: **プロンプト設計・記録のみ。未生成・未反映・git操作なし。** Codex復帰後にこの依頼文を渡す。
準拠: `EVOLUTION_LINEAGE_DESIGN_S3PLUS_20260608.md`（S4=連鎖型・紫の魔猫）/ `EVOLUTION_DATA_MASTER.md` / 品質ゲート13項目。

### 起点（採用済み新L2の確定特徴＝継承の基準）
`Partner_S4_R1_L2.png`（= `S4_R1_L2_ADOPTED_FROM_RAW_VISUAL`）:
- 細身・前傾・低姿勢の俊敏な猫。長い機械尾（リング節＋刃先）。
- 猫耳・ヒゲ・大きいマゼンタの目・**額の発光ダイヤ型ジェム**。
- 紫/黒/マゼンタ主役色。胴体に**ヘックス装甲パネル**とマゼンタ発光ライン。軽装。

### L3で強める点（= 成熟・完成・高速特化・最終通常進化感）
**重要原則: 「完成度・成熟」は質の洗練で出す。サイズ・装甲量・質量では出さない。**
1. **高速特化を強調**: さらに引き締まった空力的シルエット、躍動・疾走を感じる前傾/踏み出しポーズ。脚は速さを示す細く力強いライン。
2. **造形の洗練**: L2の装甲・尾・発光を「散漫」でなく**統合・完成**させる。パネルの面を整え、ラインを一貫した回路として通す。新規パーツの追加より、既存要素の質を上げる。
3. **最終形の風格**: 自信ある/危険な眼差し、姿勢の決まり、毛/房のディテール密度を一段上げる。額ジェム・コアの発光を「より深く・制御された」輝きに。
4. **尾の上位化**: L2の尾を保ちつつ、より洗練された鞭的シルエット（速さの象徴）。**ただし巨大化・重量化はしない**。
5. **配色の深化**: 紫/黒の階調を深め、マゼンタ発光のコントラストで小表示でも映えるように。

### L2から必ず維持する種DNA
- 猫であること（耳・ヒゲ・猫顔・四脚・しなやかな胴）。
- 額のダイヤ型発光ジェム。紫/黒/マゼンタ。ヘックスモチーフ（本体統合）。
- R1 SPEED の細身・軽装・高機動（POWER/GUARDに寄せない）。

### NG（厳守・1つでも該当＝不採用）
- **POWER/GUARD化**（筋肉質な重量級、重装甲タンク化）。
- **過剰装甲・巨大化**（パーツ盛り、質量増、体格肥大）。
- **浮遊リング・ハロー・魔法陣**、**背景・地面・影**、**UI記号化/HUDボタン化したヘックス**。
- 粒子・火花・スピード線、文字・ラベル・比較・資料・複数体・ポスター化。
- 別キャラ化（人型/ロボット/結晶塊化）、色替えだけ、平面ベクター/ステッカー風、子供っぽい低密度。

### 技術仕様（必達）
512×512 / RGBA / transparent background / 四隅alpha0 / 外周2px clear / 中央配置 / 256引き伸ばし禁止 / 単体キャラのみ。

### Codex 生成プロンプト（コピペ用・英語）
```
exactly one standalone creature sprite, full body, single character only.
S4 Hex Cat / R1 SPEED route / L3 final normal evolution — the matured, refined final form of the L2.
A lean, agile, high-speed purple cyber-mystic CAT. Keep it unmistakably a cat: sharp cat ears, whiskers, feline face, four legs, supple slim body, long expressive tail.
Continuity from L2 (must inherit): glowing diamond gem on the forehead; large magenta eyes; purple / black / magenta palette; integrated hexagon armor-panel motif on the body; magenta circuit-like glow lines; long mechanical ringed tail with a bladed tip; light armor.
Make it read as a FINAL evolution through REFINEMENT, not bulk: more aerodynamic streamlined silhouette, confident dangerous expression, dynamic forward/lunging speed pose, cleaner unified armor surfaces, a single coherent glowing circuit running through the body, deeper purple/black shading with controlled magenta glow, higher fur/detail density.
Speed-specialized: slim, fast, agile, light — NOT bulky, NOT a tank, NOT muscular heavyweight.
modern high-resolution painterly pixel-art indie game creature sprite, crisp, readable at small size.

NEGATIVE / do NOT include: no power/guard heavy body, no bulky armor, no oversized parts, no gigantism, no extra armor plates piled on, no floating rings, no halo, no magic circle, no glowing ground ring, no background, no floor, no shadow, no particles, no sparks, no speed lines, no UI icons, no HUD buttons, no text, no labels, no comparison sheet, no chart, no poster, no multiple characters, no humanoid, no robot, no crystal cluster, no flat vector sticker look, no childish low-detail style.
```

### 提出・検収フロー（生成は後日）
- まず **L3 単体1枚** をTempへ出力。`size / 四隅alpha / 外周漏れ / 透過有無` を数値自己報告。
- Claude が品質ゲート13項目＋技術検収（512/RGBA/四隅0/外周漏れ0/bbox/中央/小表示）＋ **L2との連続性（同一個体の成長に見えるか）** を確認。
- 合格のみ、別途「反映GO」後に `Partner_S4_R1_L3.png` へ同名上書き（原本バックアップ・.meta温存）。
- L3確定後、**L1はL2から逆算**（小型・軽装・未完成）して設計する。

### 本書での未実施（重要）
- 画像生成なし・本番PNG未変更・`Assets/Resources/Skins` 未保存・`.meta` 未編集・`CoreLanternGame.cs` 未編集・Unity設定/Prefab/Scene 未変更・git操作なし。

## 2026-06-13 S4 R1 SPEED L3 生成結果（L3失敗例・参考履歴）

判定: **NG / L3失敗例 / 参考履歴**。視覚HOLDではない。TECH_FIXには進めない。

raw画像:
- `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d741c5e4a202530016a2c29026be08198bd8f626812e81ecb.png`

NG理由:
- 採用済み `Partner_S4_R1_L2.png` と見た目が近すぎる。
- 体格、ポーズ、耳、胴体、尾の印象がほぼ同系差分。
- L3 final normal evolution としての進化差が不足。
- 問題は技術状態ではなく、**L2→L3のステージ差不足**。

技術状態（参考）:
- 1401x1123 / RGB / 非透過 / チェッカー背景焼き込み。
- ただし今回は技術補正対象にしない。

扱い:
- 本番反映なし。
- candidate正式保管なし。
- TECH_FIX対象外。
- 同じプロンプトでの連続生成は停止。

次にS4 R1 L3を作る場合の方針:
- L2と同じ前傾猫シルエットをそのまま使わない。
- L2より明確に成熟した体格にする。
- 頭身を少し大人寄りにする。
- 脚の完成度と高速機構を強める。
- 背中〜尾のシルエットをL2と明確に変える。
- ただし巨大化、重装甲化、POWER/GUARD化はしない。
- 「装飾を増やす」ではなく「形そのものを進化」させる。

未変更:
- 本番PNG未変更。
- `Assets/Resources/Skins` 未保存。
- `.meta` 未編集。
- `CoreLanternGame.cs` 未編集。
- Unity設定・Prefab・Scene・git 未操作。

## 2026-06-13 S4 R1 SPEED L3 プロンプト v2（L2との差分強化・未生成）

> 🚫 **不使用（DEPRECATED / 2026-06-14 確定）**: 本節（v2）も**旧R1想定**（起点L2＝旧8617478F、現在は R2 POWER L2）で書かれている。**今後このv2プロンプトは使用しない。** S4 R1 L3 は現行 **R1 SPEED L2＝STANDING版（A0706CE1…）を起点に新規設計**する。

状態: **プロンプト設計のみ / 未生成 / 未反映**。

目的:
- 採用済み `Partner_S4_R1_L2.png` との差を明確にする。
- 装飾追加ではなく、**体格・姿勢・脚・背中〜尾のシルエット・顔つき**で進化差を出す。
- L2の前傾猫ポーズをそのまま使わない。
- POWER/GUARD化、巨大化、重装甲化は禁止。

L2から変えるべき造形:
- 姿勢: L2の低い前傾待機姿勢ではなく、より完成された「疾走直前の伸びた構え」。肩から腰までのラインを長く、しなやかにする。
- 体格: 巨大化ではなく、若さが抜けた大人の細身。胴体は少し長く、胸・腰・脚の比率を洗練させる。
- 顔つき: かわいい猫顔から、鋭く自信のある成猫の表情へ。目は大きく読みやすいまま、視線を少し危険にする。
- 脚: L2より脚の完成度を上げ、細いだけでなく「高速で踏み込める」筋肉と機構を感じるラインにする。前脚/後脚のシルエットをL2と変える。
- 背中〜尾: L2の丸く上がる尾そのまま禁止。背中から尾へ流れる長いS字ライン、鞭のような尾、尾先は軽量ブレード状。尾の付け根〜先端で速度ルートの完成形を見せる。
- 装甲: 増やしすぎない。L2より「整理され統合された」ヘックス装甲にする。パーツ量ではなく面の美しさで完成感を出す。

Codex/外部生成プロンプト v2（コピペ用・英語）:
```
Create exactly ONE standalone game character sprite candidate.
Subject: S4 Hex Cat / R1 SPEED route / L3 final normal evolution.

This is the matured final form evolved from the adopted L2 purple cyber-mystic cat, but it must NOT reuse the same low forward-leaning cat pose.

Core identity:
A purple cyber-mystic hex cat companion, unmistakably feline: sharp cat ears, whiskers, feline face, bright magenta eyes, slim agile four-legged cat body, cat paws, long expressive tail. Purple and black are dominant, with controlled magenta/violet glow integrated into the body.

L3 evolution goal:
Show a clear evolution from L2 through BODY SHAPE, POSTURE, LEGS, BACK-TO-TAIL SILHOUETTE, and FACE.
Do not show evolution by simply adding more ornaments.

Pose / silhouette:
Use a more mature high-speed stance than L2: a long, elegant, stretched sprinter-cat posture, like the moment before a high-speed dash.
The shoulder-to-hip line should be longer and sleeker than L2.
The body should feel adult, refined, and complete, not larger or heavier.
Do NOT copy the L2 low crouching forward pose.

Head / face:
Make the face clearly more mature and dangerous than L2.
Keep the cat face readable and cute-cool, but sharpen the eyes, muzzle, ear shape, and brow line.
The expression should feel confident, intelligent, and fast.

Legs:
Make the legs more developed than L2: slim but powerful, with clean high-speed mechanical/feline structure.
The front and rear leg silhouettes should be different from L2, showing a completed speed evolution.
Do not make the legs bulky.

Back and tail:
The biggest silhouette change should be the back-to-tail flow.
Create a long S-curve from back to tail, a whip-like aerodynamic tail that feels evolved for speed.
Tail tip may be a light bladed cyber-mystic shape, but not oversized.
Do NOT reuse the same circular raised tail silhouette from L2.

Armor / markings:
Light armor only. Integrated hex armor panels and magenta circuit lines should look cleaner, more unified, and more complete than L2.
Do not add random floating parts.
Do not make it heavy, tanky, or fortress-like.

Style:
commercial indie game creature sprite quality, modern high-resolution pixel-art inspired style, clean dark outline, strong cel shading, crisp readable silhouette, detailed but not noisy, cute but cool, mature final normal evolution.

Strict negatives:
no poster, no infographic, no concept sheet, no lineup, no comparison chart, no evolution chart, no labels, no text, no UI, no background scene, no floor, no ground, no shadow, no magic circle, no floating rings, no particles, no speed lines, no multiple characters.
no giant form, no bulky armor, no heavy tank silhouette, no power route, no guard route, no fortress design, no muscular heavyweight, no dog, no fox, no wolf, no rabbit, no dragon, no humanoid, no robot.
```

生成後の判定ポイント:
- L2と並べて、体格・姿勢・脚・背中〜尾のシルエットが明確に違うか。
- L2より成猫・成熟・完成形に見えるか。
- R1 SPEEDの細身・高速・軽装感が維持されているか。
- POWER/GUARD化、巨大化、重装甲化していないか。
- 装飾差分ではなく、形そのものが進化しているか。

未実施:
- 画像生成なし。
- 本番PNG未変更。
- `Assets/Resources/Skins` 未保存。
- `.meta` 未編集。
- `CoreLanternGame.cs` 未編集。
- Unity設定・Prefab・Scene・git 未操作。

## 2026-06-14 S4 R3 GUARD L2 raw visual candidate

状態: **R3 GUARD L2 採用フロー候補 / TECH_FIX_REQUIRED**。

候補ID:
- **S4_R3_L2_RAW_VISUAL**

raw画像パス:
- `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d7cb0ae9f7faed6016a2e657fb9cc81919a237b10c9c8cf02.png`

視覚評価:
- ポスター化なし。
- 1体のみ。
- 文字・ラベル・UIなし。
- 紫のサイバー魔法猫として読める。
- 猫耳・猫顔・長い機械尾が残っている。
- 低く安定した構え、胸・肩・前脚の防御感、守るような尾の形があり、S4 Hex Cat / R3 GUARD / L2 の採用フロー候補として扱える。

R1/R2との差分:
- R1 SPEED L2 より低重心で守備的。
- R2 POWER L2 より攻撃圧ではなく保護・安定感が強い。
- POWERの筋肉型ではなく、GUARDの守備・安定・保護感に寄っている。

技術状態:
- **TECH_FIX_REQUIRED**
- Codex raw生成のため、512x512 / RGBA / transparent background / alpha検収は Claude 後処理対象。
- サイズ/RGB/非透過/チェッカー背景焼き込み等がある場合でも、視覚品質はHOLDとして保持する。

Claudeへ渡すTECH_FIX依頼文:
```
S4 Hex Cat / R3 GUARD / L2 の raw visual candidate を TECH_FIX してください。

候補ID:
S4_R3_L2_RAW_VISUAL

raw画像:
C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d7cb0ae9f7faed6016a2e657fb9cc81919a237b10c9c8cf02.png

目的:
S4 Hex Cat / R3 GUARD / L2 採用フロー候補として、512x512 / RGBA / transparent background へ技術補正できるか確認してください。

視覚方針:
- 紫のサイバー魔法猫。
- R3 GUARD向け。
- R1 SPEEDより低重心で守備的。
- R2 POWERより攻撃圧ではなく保護・安定感が強い。
- 猫感、低く安定した構え、胸・肩・前脚の防御感、守るような尾の形は維持。

TECH_FIX要件:
- 512x512 PNG
- RGBA
- transparent background
- 四隅alpha 0
- 外周2px透過漏れ 0
- 中央配置
- 灰色/白色/チェッカー背景の焼き込み除去
- 耳・ひげ・尾・足先・発光ラインの破綻なし

禁止:
- 本番PNGへ上書きしない
- Partner_S4_R3_L2.png へ反映しない
- .metaを触らない
- CoreLanternGame.csを触らない
- Unity設定 / Prefab / Scene / git操作をしない

完了後:
- TECH_FIX済みcandidateの保存先
- size / mode / 四隅alpha / 外周2px漏れ / bbox
- 視覚破綻の有無
- TECH_FIX_PASS / TECH_FIX_FAILED
を報告してください。
```

未変更:
- 本番PNG未変更。
- `Assets/Resources/Skins` 未保存。
- `.meta` 未編集。
- `CoreLanternGame.cs` 未編集。
- Unity設定・Prefab・Scene・git 未操作。

## 2026-06-14 Claude技術補正＋本番反映（S4 R3 GUARD L2）

判定: **TECH_FIX_PASS → 本番反映済み（R3 GUARD L2）**。採用ID `S4_R3_L2_ADOPTED_FROM_RAW_VISUAL`。

raw画像（入力）:
- `C:\Users\kodai\.codex\generated_images\019e3668-9b71-7e42-91f4-6483282ef9b8\ig_0d7cb0ae9f7faed6016a2e657fb9cc81919a237b10c9c8cf02.png`
- 1254×1254 / RGB / 非透過 / チェッカー背景焼き込み（四隅≈236灰）。

TECH_FIX出力（candidate）:
- `tmp_diag/candidates_hold/S4_R3_L2_RAW_VISUAL_TECHFIX_512.png`（temp元 `%LOCALAPPDATA%\Temp\Eggcore_S4R3_TechFix_20260613\`）。

Claude後処理検収:
- **512×512 / RGBA / 四隅alpha0 / 外周2px漏れ0(max0) / bbox x[20..479] y[70..435]・中心dx-6.5,dy-3.5**。
- 緑/黒/白合成でハロー・灰フチなし。**耳・房毛・ヒゲ・足先/爪・尾刃/尾節/中央オーブ・マゼンタ発光すべて健在**。
- GUARD読み: 低重心・胸/肩/前脚の防御プレート・盾状尾先で R1 SPEED L2 より守備的。POWERの筋肉型ではない。

本番反映: **完了**
- 反映先: `Assets/Resources/Skins/Partner_S4_R3_L2.png`（同名上書き）。
- 旧本番（256×256, 81,748 bytes, sha D696FD89…2ACF）→ `_backup_skins/20260613_S4/Partner_S4_R3_L2.png`（hash一致で退避）。
- 上書き後 sha **ECB75F41…B636**（candidate一致）。`.meta` 未編集（hash・mtime 不変）。
- **L3には反映していない**: `Partner_S4_R3_L3.png` 未変更（256×256, 90,773 bytes）。
- `Partner_S4_R3_L2.png` 以外の本番PNG・`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git は未変更／画像生成なし。

## 2026-06-14 S4 L2三分岐 最終整理（完了）

状態: **S4 Hex Cat の L2 三分岐（R1 SPEED / R2 POWER / R3 GUARD）＝完了**。ハッシュ照合で実体確定。本節を**確定マッピングの正**とする（過去節で route が異なる記述があれば本節が優先）。

### 確定マッピング（実体ハッシュ）
| スロット | 実体ファイル(hash) | バイト数 | 由来 / 採用 | 技術 |
|---|---|---|---|---|
| **R1 SPEED L2** `Partner_S4_R1_L2.png` | `A0706CE1…` | 225,513 | **STANDING版**（candidate: `S4_R1_L2_STANDING_RAW_VISUAL_TECHFIX_512.png`）。SPEEDルートとして採用 | 512×512 / RGBA / 四隅0 / 外周漏れ0 / .meta未編集 |
| **R2 POWER L2** `Partner_S4_R2_L2.png` | `8617478F…` | 257,658 | **低姿勢版**（candidate: `S4_R1_L3_RAW_VISUAL_TECHFIX_512.png`）。当初SPEED候補→**レビューで「低姿勢・装甲・機械尾の圧が強くPOWER寄り」と判断しR2 POWER L2へ route再分類**（取り違えではなく意図した配置） | 512×512 / RGBA / 四隅0 / 外周漏れ0 / .meta未編集 |
| **R3 GUARD L2** `Partner_S4_R3_L2.png` | `ECB75F41…` | 226,194 | **GUARD版**（candidate: `S4_R3_L2_RAW_VISUAL_TECHFIX_512.png`）。GUARDルートとして採用 | 512×512 / RGBA / 四隅0 / 外周漏れ0 / .meta未編集 |

### 採用記録（route別）
- **R1 SPEED L2 = STANDING版（A0706CE1）**: 現行のSPEEDルートL2として採用・反映済み。
- **R2 POWER L2 = 低姿勢版（8617478F）**: route再分類後のPOWERルートL2として採用・反映済み（採用記録は本節で確定。06-13節の「R1 SPEED L2」表記はこの再分類で上書き）。
- **R3 GUARD L2 = GUARD版（ECB75F41）**: GUARDルートL2として採用・反映済み（06-14反映節と整合）。

### 留意（今後の設計時・確定ルール）
- **既存の「S4 R1 SPEED L3 設計 プロンプト v1／v2」節は使用しない（DEPRECATED）。** 旧R1想定（起点L2＝旧8617478F／現R2 POWER）で書かれているため。
- **L3 は各ルートとも、現行L2を起点に新規設計する**:
  - **S4 R1 L3** → 現行 **R1 SPEED L2＝STANDING版（A0706CE1…）** を起点に新規設計。
  - **S4 R2 L3** → 現行 **R2 POWER L2＝低姿勢版（8617478F…）** を起点に新規設計。
  - **S4 R3 L3** → 現行 **R3 GUARD L2＝GUARD版（ECB75F41…）** を起点に新規設計。
- 各L1（R1/R2/R3）は対応する現行L2から逆算（小型・軽装・未完成）して設計する。

### 完了ステータス
- **S4 L2三分岐（R1 SPEED / R2 POWER / R3 GUARD）＝完了（クローズ）。** 本最終整理を「正」とする。以降、追加の本番PNG変更・PNG入れ替え・.meta変更・Unity設定・Prefab/Scene・git操作は行わない（別途の明示指示があるまで）。

### 本整理での変更範囲
- **docs更新のみ**。本番PNGは追加変更なし／PNG入れ替えなし／`.meta`・`CoreLanternGame.cs`・Unity設定・Prefab・Scene・git・画像生成すべて未実施。
