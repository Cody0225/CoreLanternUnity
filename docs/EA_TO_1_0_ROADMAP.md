# EA → 1.0 ロードマップ — Eggcore Protocol

**作成日**: 2026-05-28
**用途**: Steam アーリーアクセス申請時の「Why Early Access」回答ソース + 開発計画
**対象期間**: 2026-08 (EA リリース) 〜 2027-02 (1.0 リリース予定、約 6 ヶ月)

---

## 🎯 1.0 リリースのビジョン

「EA で集めたフィードバックを反映し、コンテンツ量・難易度幅・国際対応を強化した完成版」

EA 開始時点の状態 (現在)
- 11 相棒、5 ステージ、10 Wave、全ボス戦実装済
- 60+ モジュール、15+ レリック
- Reroll/Lock/Banish カード管理
- Danger Level 0-5
- 日本語のみ

1.0 リリース時点の目標
- 14+ 相棒 (3 種追加)、6-7 ステージ (1-2 種追加)、エンドレスモード追加
- 80+ モジュール、20+ レリック
- 実績システム (Steam Achievements)
- 英語ローカライズ
- キーリマップ
- バランス調整完了 (EA フィードバック反映)

---

## 📅 ロードマップ (3 ヶ月単位)

### Phase A: EA 初動 (2026-08 〜 2026-10、3 ヶ月)

**テーマ**: 安定化 + コミュニティ形成

| マイルストーン | 内容 | 担当 |
|---|---|---|
| **v0.9.0 (EA Launch)** | 2026-08-15 リリース | All |
| v0.9.1 Hotfix | 致命バグ修正 (24-72 時間以内) | Claude |
| v0.9.2 Balance | ボス難易度・ストリーク報酬 調整 | Claude |
| v0.9.3 QoL | UI 改善、キーリマップ追加 | Claude |
| v0.10.0 Content | **新相棒 1 体追加** + 新モジュール 5 種 | Claude + Codex |

**Codex 並行作業**:
- 残キャラ画像の段階投入 (L0/L1/L2)
- BGM 商用素材選定 + ライセンス取得
- itch.io 版アップデート

**KPI 目標** (3 ヶ月終了時):
- Steam DL: 500+
- Steam レビュー: 30+ (Mostly Positive)
- Discord メンバー: 50+

### Phase B: EA 中期 (2026-11 〜 2027-01、3 ヶ月)

**テーマ**: コンテンツ拡張 + 国際化準備

| マイルストーン | 内容 | 担当 |
|---|---|---|
| v0.11.0 | **新ステージ 1 種追加** (Stage 6: 仮称 Genesis Vault) | Claude + Codex |
| v0.12.0 | **新相棒 2 体追加** (累計 13 → 14 体) | Claude + Codex |
| v0.13.0 | **エンドレスモード** 追加 (Wave 10 以降ループ、難易度自動上昇) | Claude |
| v0.14.0 | **実績システム** (Steam Achievements 30+ 項目) | Claude |
| v0.15.0 | **英語ローカライズβ** (テキスト外出し + 翻訳適用) | Claude + Codex |

**Codex 並行作業**:
- 新キャラ画像生成
- 新ステージ素材
- Steam 1.0 用ストア画像更新
- トレーラー素材撮影開始

**KPI 目標** (Phase B 終了時):
- Steam DL: 2,000+
- Steam レビュー: 70+ (Mostly Positive 以上)
- Discord メンバー: 150+

### Phase C: 1.0 リリース (2027-02、1 ヶ月)

**テーマ**: 完成版リリース

| マイルストーン | 内容 | 担当 |
|---|---|---|
| v1.0.0-rc1 | リリース候補ビルド + ベータテスター招待 (Discord) | All |
| v1.0.0-rc2 | 致命バグ修正、最終バランス調整 | Claude |
| **v1.0.0 (Full Release)** | 2027-02-04 予定 | All |
| v1.0.1 Hotfix | 1.0 リリース直後の小修正 | Claude |
| 1.0 ローンチ価格 | $4.99 (EA $2.99 から値上げ) | ユーザー |
| 1.0 ローンチセール | 1 週間 $2.99 割引 | ユーザー |

**KPI 目標** (1.0 リリース 1 ヶ月後):
- Steam DL (1.0 後追加): 1,000+
- Steam レビュー (累計): 100+ (Very Positive 目標)
- 売上累計: $5,000+

---

## 🆕 計画中の新コンテンツ詳細

### 新相棒キャラ (Phase A-B で 3 体追加)

| 候補 | コンセプト | 実装フェーズ |
|---|---|---|
| **Magnet Mole** (style 12) | データ回収特化、磁力ピックアップ範囲 ×3 | Phase A (v0.10.0) |
| **Echo Bat** (style 13) | ソナー型、敵を視覚化、暗闇ステージで強い | Phase B (v0.12.0) |
| **Glitch Phoenix** (style 14) | 復活ギミック、Last Stand 強化、Phase 回避強 | Phase B (v0.12.0) |

各キャラの仕様詳細は `docs/CHARACTER_REDESIGN_SPEC.md` (拡充予定) に記載。

### 新ステージ (Phase B で 1 種追加)

| 候補 | コンセプト | 実装フェーズ |
|---|---|---|
| **Stage 6: Genesis Vault** (隠し) | Genesis Core 解放後にアクセス可能、全難度ミックス | Phase B (v0.11.0) |
| **Stage 7: Endless Spire** (エンドレス専用) | Wave 制限なし、無限スケーリング | Phase B (v0.13.0) |

### 新モジュール / レリック (Phase A-B で +20 種)

- 新キャラのシグネチャ強化
- フィードバックで要望多い汎用枠
- 新ステージ向けの専用枠

### エンドレスモード仕様 (Phase B)

- Wave 10 撃破後、自動継続
- 11 Wave 以降は敵 HP/速度/数 が指数増加
- 100 Wave 突破で隠しエンディング
- スコアリーダーボード (Steam ランキング)

### 実績システム (Phase B)

代表的な 30 項目案:
- Wave 5 / 10 / 20 / 50 / 100 クリア
- 各キャラで 1 回クリア (11 種)
- 全クロス進化達成 (6 種)
- 全ステージクリア (5 種)
- Danger Level 5 でクリア
- 死因記録: 各死因タイプで 1 回
- 完璧クリア (ノーダメ Wave 5 突破等)

---

## 📊 マイルストーン管理

各マイルストーンの達成判定:

| マイルストーン | 完了条件 | KPI |
|---|---|---|
| v0.9.0 EA Launch | Steam ページ公開、レビュー受付開始 | DL 50+ (1 週間) |
| v0.10.0 First Content | 新キャラ 1 体 + モジュール 5 種実装、Roslyn 通過 | レビュー Mostly Positive 維持 |
| v0.13.0 Endless Mode | エンドレス 100 Wave テストプレイ完走 | 平均プレイ時間 +50% |
| v1.0.0 Full Release | RC2 で致命バグ 0、英語完訳 | DL 2,000+ (累計) |

---

## ⚠ リスク管理

### リスク 1: コンテンツ追加が間に合わない
- **緩和策**: 新キャラ追加を 3 → 2 に減らす、新ステージ追加を後回し
- **影響**: 1.0 リリース内容が EA + 限定追加のみになる

### リスク 2: ネガレビュー集中
- **緩和策**: フィードバックを最優先で反映、毎週パッチ
- **影響**: レビュー評価が回復しない場合、Steam 露出が下がる

### リスク 3: 開発リソース枯渇
- **緩和策**: 1.0 リリースを延期、EA 期間延長 (12 ヶ月以内ならペナルティなし)
- **影響**: 価格据置で長く EA 継続

### リスク 4: 英語翻訳遅延
- **緩和策**: 機械翻訳 + コミュニティ翻訳併用、1.0 時点では日本語のみ受け入れ可
- **影響**: 海外ユーザー獲得遅延

---

## 🎯 Steam EA リリース時の必須回答 (Why Early Access)

Steamworks Portal で以下の英文質問に回答:

### Q1: Why Early Access?
```
We chose Early Access because player feedback is critical to shaping
the final game balance and content. Our defense-roguelite-survivor
fusion is a unique genre blend, and we want to refine it based on
how real players experience the synergies between defending the core,
managing wave pressure, and building roguelite synergies.
```

### Q2: Approximately how long will this game be in Early Access?
```
6-12 months. Targeted full release: 2027-02-04.
```

### Q3: How is the full version planned to differ from the Early Access version?
```
The 1.0 full release will include:
- 3 additional partner characters (14 total, up from 11)
- 1-2 additional stages (6-7 total, up from 5)
- An Endless Mode with infinite scaling
- Steam Achievements system (30+ achievements)
- Full English localization
- Key remapping
- Balance refined based on community feedback
- Additional modules and relics
```

### Q4: What is the current state of the Early Access version?
```
The Early Access version is fully playable:
- 11 partner characters with unique signature mechanics
- 5 stages with unique enemies and hazards
- 10 wave run structure with mid-boss (Pulswyrm) and final boss (Nullwyrm) Phase 2
- 60+ modules and 15+ relics
- Reroll/Lock/Banish card management system
- Danger Level 0-5 difficulty options
- Death cause tracking and run summary

Average run: 15-20 minutes. All content can be experienced from day one.
```

### Q5: Will the game be priced differently during and after Early Access?
```
Yes. Early Access price: $2.99. Full version 1.0 price: $4.99.
Players who purchase during Early Access receive the full version
at no additional charge.
```

### Q6: How are you planning on involving the Community in your development process?
```
We will:
- Post monthly development updates on Steam Community
- Monitor and respond to Steam Forum discussions (daily)
- Maintain a Discord server for direct community interaction
- Run quarterly polls on feature priorities
- Provide a public roadmap that updates as plans evolve
- Credit major contributors in the in-game credits
```

---

## 📋 関連ドキュメント

- `docs/RELEASE_CHECKLIST.md` — リリース全体チェックリスト
- `docs/STEAM_BUILD_GUIDE.md` — Steam ビルド・連携手順
- `docs/CHARACTER_REDESIGN_SPEC.md` — キャラ追加仕様 (拡張中)
- `docs/STAGE_DESIGN_SPEC.md` — ステージ追加仕様
- `docs/BOSS_PHASE2_SPEC.md` — ボス強化仕様

最終更新: 2026-05-28
担当: Claude (秘書)
レビュー: ユーザー (内容承認 + Steamworks 提出時に流用)
