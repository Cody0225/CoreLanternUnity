# 表情システム Unity組み込み 設計メモ — 2026-06-16

状態: **設計のみ（未実装）。** `CoreLanternGame.cs` 未変更 / git未操作。確定方式は `docs/VISUAL_STYLE_GUIDE.md` §11-2。

## 目的
プレイヤー（＝選択中パートナー）に、被弾/攻撃/進化で**表情フル絵の差し替え＋エフェクト**を出す。S1で確定した方式をゲーム本体へ。

## 現状の該当コード（実装時に再確認する）
- `playerRenderer`（SpriteRenderer, decl 259付近）＝プレイヤー本体スプライト。
- 表情なしの既存モーション: `UpdatePlayerVisualMotion`相当（~12620-12678）。`playerVisualBody` を **縦横非対称スクワッシュ**している → ⚠ **顔を歪ませる**ので、表情絵を出す間は非対称スクワッシュを抑える（同率スケール＋移動＋傾きに寄せる）。
- 被弾: `DamagePlayer`（10304） / `playerHitPulse`（set 10349、decay 15600）。大ダメージで既に `Flash(赤)`/`Shake` を呼んでいる。
- 攻撃: `playerAttackPulse`（12633 で減衰）。発射/攻撃時に立つ。
- 進化: `evolutionStage` 変化・進化演出（`Evolve` SFX 等）。
- 既存エフェクト資産: `Flash(color,dur)` / `Shake(amp,dur)` / `HitFreeze(sec)` / `SpawnSparks` 系 → **流用**。

## アセットモデル
- 1フォームにつき表情4枚: `NEUTRAL / HURT / ATTACK / EVOLVE`。
- 配置: `Assets/Resources/Skins/` に表情サフィックス付きで置く（例: `Partner_S{n}_R{r}_L{l}_HURT.png` 等）。NEUTRAL＝既存の本番スプライトを置換（1024・整列済み）。
- ロード: 既存 `LoadSkinSprite`/`skinSpriteCache` を表情キー対応に拡張（`..._HURT` を引けるように）。無い表情はNEUTRALにフォールバック（未整備フォームでも壊れない）。

## 表情ステートマシン（毎フレーム決定）
優先度（高→低）: **EVOLVE > HURT > ATTACK > NEUTRAL**
- EVOLVE: 進化イベント発火中（演出の数秒）。
- HURT: 被弾後 `hurtTimer`（例 0.35s）。`DamagePlayer` で `hurtTimer = 0.35` をセット。
- ATTACK: `playerAttackPulse > 0.15` の間（攻撃直後の短時間）。
- それ以外: NEUTRAL。
→ 毎フレーム現在の表情を決め、`playerRenderer.sprite` を該当表情スプライトに差し替えるだけ（絵まるごと差し替え＝お面/二重なし）。

## エフェクト（既存流用＋少し追加）
- 被弾: 既存 `Flash(赤)` ＋ `Shake` ＋（任意）`SpawnSparks` で衝撃。プレビューの「赤フラッシュ＋のけぞり」に相当。のけぞりは `playerVisualBody` に短時間の移動＋同率縮みで（**非対称スクワッシュは使わない**）。
- 攻撃: 既存の前のめり（`recoil`/`playerAttackPulse`）を活用。スピードライン等は任意。
- 進化: `Flash(白/cyan)` ＋ リング/粒子（`SpawnRingSparks`）。

## ⚠ 設計上の注意
1. **顔を歪ませない**: 表情絵表示中は `playerVisualBody` の縦横非対称スクワッシュを止める/弱める（同率スケール＋移動＋傾きのみ）。既存モーションの該当行を条件分岐。
2. **サイズ整合**: 表情4枚は同じ体サイズ・足位置で用意（取り込み時に揃える）。フォーム間でL0/L1/L2/L3のサイズ差は従来通り。
3. **未整備フォームの安全策**: 表情スプライトが無いフォームはNEUTRAL（＝従来挙動）にフォールバック。段階導入できる。

## 段階導入（ロールアウト）
1. まず **S1 の1フォーム**（L0 か L1 R1）で実装＋動作確認（`Tools/CheckCompileHealth.ps1 -Quick`／Play確認）。
2. OKなら表情ロード/ステートマシンを共通化し、用意できたフォームから順に表情スプライトを差す。
3. 全フォームは Codex の表情セット生成に追従（VISUAL_STYLE_GUIDE §11-2 のレシピ）。

## 実装時の手順（cs編集の作法・CLAUDE.md準拠）
- `EDIT_LOCK.md` を `LOCKED` に → Edit/Writeのみで編集 → `CheckCompileHealth.ps1`（brace/odd-quote/Roslyn 0）→ `RELEASED`。
- 影響範囲: 表情ロード（skin cache周辺）／表情ステート＋`playerRenderer.sprite`差し替え（visual update周辺）／`DamagePlayer`に`hurtTimer`セット。新規メソッド `UpdatePlayerExpression()` を visual update から呼ぶ想定。

## 未確定（実装前に決める）
- 表情スプライトの最終命名（`_HURT`サフィックス案）と、Resourcesへの本番配置タイミング。
- 既存スクワッシュをどこまで残すか（顔歪み回避との両立）。
