# Audio License Log — Eggcore Protocol

**最終更新**: 2026-06-01
**用途**: 商用版に入れる音素材の証跡管理 (法務対応)
**運用**: 採用判定前に Codex / ユーザーが本ログを必ず更新

商用版に入れる音素材は、必ずこの形式で証跡を残す。
AI 生成音もフリー素材も同じ粒度で管理する。

---

## 🎯 なぜ証跡が必要か

| リスク | 対策 |
|---|---|
| Steam Content ID クレーム (BGM 著作権) | ライセンス文書を保管、商用利用可を確認 |
| YouTube/Twitch での配信時 mute / 動画削除 | Royalty-Free / CC0 を選定 |
| 開発者の知らないところで第三者素材が混入 | 全 BGM/SE を本ログで管理 |
| 後で素材削除になった時の追跡 | Source URL とライセンス文を保管 |

---

## 📋 License Log (採用済み + 候補)

| FileName | IntendedUse | SourceType | SourceService | URL | Author | TrackOrPrompt | LicenseOrPlan | AttributionRequired | DateAcquired | ProofPath | Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `BGM.wav` | Main menu + Wave loop (default) | Procedural (自社合成) | `Tools/GenerateStarterAudio.ps1` | (none) | Embercore Studio | Deterministic sine/triangle synthesis | Self-generated | No | 2026-06-01 | `Assets/Resources/Audio/Licenses/StarterAudio_License.txt` | ✅ Approved (placeholder) | 2026-06-01 comfort pass 済。EA リリースまでに商用素材へ差替推奨 |
| `BGM_Stage2.wav` | Stage 2 (Lava Cache) | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | comfort pass 済。商用差替候補 |
| `BGM_Stage3.wav` | Stage 3 (Broken Core) | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | comfort pass 済。商用差替候補 |
| `BGM_Stage4.wav` | Stage 4 (Frost Vault) | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | comfort pass 済。商用差替候補 |
| `BGM_Stage5.wav` | Stage 5 (Storm Spire) | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | comfort pass 済。商用差替候補 |
| `BGM_Boss_Pulswyrm.wav` / `BGM_BossPulswyrm.wav` | Wave 5 mid-boss | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | 後者は旧名fallback。商用差替候補 |
| `BGM_Boss_Nullwyrm.wav` / `BGM_BossNullwyrm.wav` | Wave 10 final boss | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | 後者は旧名fallback。商用差替候補 |
| `Shoot.wav` | Player projectile | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | 多段ヒット対策で高域・ノイズを抑制 |
| `Hit.wav` 〜 `GameOver.wav` (8 種) | 各 SE 用途 | Procedural | 同上 | (none) | Embercore Studio | Same | Self-generated | No | 2026-06-01 | 同上 | ✅ Approved (placeholder) | comfort pass 済。商用差替候補 |

---

## 📁 Proof Files 保存場所と命名

```
Assets/Resources/Audio/Licenses/
├── StarterAudio_License.txt              (procedural 全体ライセンス)
├── {FileName}_License.txt                (各素材のライセンス文面)
├── {FileName}_Source_Screenshot.png      (DL ページのスクリーンショット)
├── {FileName}_AI_Prompt.txt              (AI 生成時のプロンプト)
└── {FileName}_TermsOfService.pdf         (サービスの規約 PDF 保存)
```

例: Pixabay から `BGM_Stage3_PixabayTrack.wav` を採用した場合:
```
BGM_Stage3_PixabayTrack_License.txt        (Pixabay License 全文をテキストで保存)
BGM_Stage3_PixabayTrack_Source_Screenshot.png (DL ページのスクショ)
BGM_Stage3_PixabayTrack_Author.txt         (作者名とクレジット表記要否)
```

---

## 🟢 推奨ソース (商用利用可・無料)

### BGM

| サービス | ライセンス | コスト | 注意点 |
|---|---|---|---|
| [Pixabay Music](https://pixabay.com/music/) | Pixabay License (商用可、クレジット不要) | 無料 | 一部楽曲は CC0 ではないので個別確認 |
| [Free Music Archive](https://freemusicarchive.org/) | 楽曲ごと異なる (CC0 / CC-BY / etc) | 無料 | 商用可・帰属不要のものを選ぶ |
| [Incompetech](https://incompetech.com/) | CC-BY (要クレジット) | 無料 | クレジット表記必須 |
| [BGMer](https://bgmer.net/) | 無料商用可 (帰属不要) | 無料 | 日本産、和テイストもあり |
| [効果音ラボ](https://soundeffect-lab.info/) | 無料商用可 (帰属不要) | 無料 | 主に SE |
| [Suno AI](https://suno.com/) | サブスク (Pro 月 $8 / Premier 月 $24) | 有料 | Pro 以上で商用利用権利 |
| [Udio](https://www.udio.com/) | 同様にサブスク | 有料 | 商用利用は Pro プラン |

### SE

| サービス | ライセンス | 備考 |
|---|---|---|
| [Pixabay Sound Effects](https://pixabay.com/sound-effects/) | Pixabay License | BGM と同じ |
| [Freesound](https://freesound.org/) | CC0 / CC-BY / etc | 個別確認必須 |
| [効果音ラボ](https://soundeffect-lab.info/) | 無料商用可 | 日本産、UI 系豊富 |
| [Kenney Game Audio](https://kenney.nl/assets/category:Audio) | CC0 | ゲーム向け |

---

## 🚫 避けるべきソース

| ソース | 理由 |
|---|---|
| YouTube から音声抽出 | 著作権違反 |
| ジングルベル等のスタンダードナンバー | 元曲の権利は切れていても演奏者の著作隣接権が有効 |
| 「フリー BGM」と書いてあるだけのブログ | 出典・規約が不明、リスク高 |
| 個人の Twitter / SoundCloud | 商用利用条件が不明 |
| 同人音楽サイト (一部) | 商用配布禁止のことが多い |
| Spotify / Apple Music DRM 解除 | 違法 |

---

## ✅ 選定基準

新規 BGM/SE を採用する前に以下を全てチェック:

- [ ] ライセンスが **商用利用可** (Royalty-Free / CC0 / Pixabay License 等)
- [ ] **クレジット表記** が必要か任意か明確
- [ ] **改変可** (ループ加工・ピッチ変更が許可されているか)
- [ ] **再配布禁止条項なし** (ゲーム内に含めて配布可能)
- [ ] **YouTube/Twitch 配信で問題なし** (Content ID 対策)
- [ ] **DL 元のスクショ + 規約 PDF** を Licenses/ に保存済
- [ ] **本ログに追加** + Status を `Candidate` → `Approved`

---

## 🔄 採用ワークフロー

### Step 1: 候補発見 (Codex / ユーザー)
1. 推奨ソースから候補曲をピックアップ
2. 本ログに新規行追加、Status = `Candidate`
3. Proof File を Licenses/ に保存

### Step 2: 試聴・検証 (ユーザー)
1. ゲームに仮実装して試聴
2. 雰囲気がゲームに合うか確認
3. 他の SE/BGM とのバランスチェック (-14 LUFS / -6 dBFS)

### Step 3: ライセンス確認 (Claude 補助)
1. ライセンス文を全文確認 (Google 翻訳でも OK)
2. 商用利用条件・改変条件・帰属条件を本ログに記録
3. グレー部分があればソースに問い合わせ (英語メール送信は Claude 補助可)

### Step 4: 採用判定 (ユーザー)
- 全条件 OK → Status = `Approved`、`Assets/Resources/Audio/` に正式ファイル名で配置
- 問題あり → Status = `Rejected`、Notes に理由

### Step 5: 配信時のクレジット表記
- クレジット必要 → `Embercore Studio` の Web サイト + ゲーム内クレジット画面に作者名・楽曲名・ライセンス記載
- クレジット不要 → 任意で記載 (作者への敬意としてあると良い)

---

## 📝 Status コード

| Status | 意味 |
|---|---|
| `Candidate` | 候補。まだゲーム採用しない。ライセンス確認中 |
| `Approved` | ライセンス確認済み。ゲームへ接続してよい |
| `Rejected` | 品質 / ライセンス / Content ID リスクで不採用 |
| `Replaced` | 以前使ったが別素材に差し替え済み (履歴として残す) |
| `Pending` | 未調査・選定待ち |
| `(placeholder)` | procedural 仮版、商用差替予定 |

---

## 🏷 ゲーム内クレジット表記テンプレ

`Embercore Studio` の Web サイト + ゲーム内クレジット画面に以下記載:

```
─────────────────────────
MUSIC
─────────────────────────

"Track Name 1" by Author Name
  Source: pixabay.com/music/...
  License: Pixabay License

"Track Name 2" by Another Artist
  Source: freemusicarchive.org/...
  License: CC-BY 4.0
  (※ クレジット表記必要)

─────────────────────────
SOUND EFFECTS
─────────────────────────

Sound effects by 効果音ラボ
  https://soundeffect-lab.info/
  License: 無料商用利用可

Additional SFX:
  - "footstep.wav" by Author (Freesound, CC0)
  - "explosion.wav" by Author (Kenney, CC0)
─────────────────────────
```

---

## 🛡 法務リスク対応

### Steam Content ID 警告が来たら
1. 警告内容を確認 (どの曲が問題か特定)
2. 本ログで該当曲のライセンスを再確認
3. 正当な権利なら反論 (DL ページ URL + 規約 PDF を提示)
4. グレーなら即差し替え (本ログで Status = `Replaced` に)

### YouTube/Twitch 配信者から「ミュートされた」報告
1. 該当 BGM を特定
2. ライセンス再確認
3. 必要なら次パッチで差替

### 万一裁判沙汰になったら
本ログ + Proof Files (Assets/Resources/Audio/Licenses/) を証拠提示。

---

## 🎓 簡単な FAQ

### Q. Pixabay License って何?
A. Pixabay 独自のライセンス。商用利用 OK、改変 OK、帰属不要、ただし楽曲そのものの再配布禁止。
ゲームに組み込んで配布するのは「再配布」ではなく「使用」扱いで OK。

### Q. CC0 と Royalty-Free の違いは?
- **CC0**: パブリックドメイン同等、何でもしてよい
- **Royalty-Free**: 一度買えば追加ロイヤリティなし、ただし規約はあり (再配布禁止等)

### Q. AI 生成 (Suno/Udio) は安全?
A. **サブスク Pro 以上のプランなら商用利用 OK** (利用規約で明示)。
無料プランは個人利用のみのケースが多い。要確認。

### Q. クレジット表記は本当に書かないとダメ?
A. ライセンスによる。「Attribution Required」とあるなら必須。違反すると訴訟リスクあり。
省略可能でも書いておくと作者への敬意 + 関係構築になる。

### Q. 同じ Pixabay 曲を 2 つのゲームに使ってよい?
A. OK (Pixabay License は複数プロジェクト使用を制限していない)。

---

## 📊 採用済み素材サマリ (定期更新)

**最終更新**: 2026-06-01

| 種類 | Approved | Candidate | Rejected | Placeholder |
|---|---|---|---|---|
| BGM (主要/ボス/旧名alias含む) | 0 | 0 | 0 | 9 (procedural) |
| SE | 0 | 0 | 0 | 9 (procedural) |
| **合計** | **0** | **0** | **0** | **18** |

EA リリースまでに **少なくとも 3-5 件の商用 BGM 差替** を目標 (主要 4 + ボス戦)。

---

## 🔗 関連ドキュメント

- `docs/AUDIO_ASSET_PLAN.md` — 音素材の全体方針
- `docs/AUDIO_GENERATION_PROMPTS.md` — AI 生成プロンプト集
- `docs/SOUND_ASSET_CANDIDATES.md` — 候補リスト
- `docs/AUDIO_AUTOMATION.md` — procedural 自動生成手順
- `Assets/Resources/Audio/AudioManifest.json` — Unity 読み込み用マニフェスト

---

最終更新: 2026-06-01
担当: Claude (秘書) — 雛形作成 / Codex — comfort pass 更新
運用: Codex (候補追加) + ユーザー (採用判定)
