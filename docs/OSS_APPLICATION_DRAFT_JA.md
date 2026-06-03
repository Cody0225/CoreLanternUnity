# OSS応募用ドラフト

最終更新: 2026-06-03

## プロジェクト名

Eggcore Protocol

## 概要

Eggcore Protocol は、ヴァンサバライクな自動戦闘、ローグライトのビルド構築、中央コア防衛、相棒キャラクターの進化・クロス進化を組み合わせた Unity 製 2D プロトタイプです。

プレイヤーは自動攻撃する相棒を操作し、中央の Data Egg を守りながら 10 ウェーブを生き抜きます。レベルアップでモジュールを選び、Lv3/Lv6/Lv9 で進化ルートを選択し、リンク条件を満たすとクロス進化が発動します。

## 独自性

- 生存だけでなく、中央の Data Egg を守る二重目標がある。
- SPEED / POWER / GUARD の進化ルートが、性能だけでなく相棒の見た目とビルド方針に紐づく。
- リンクとクロス進化により、通常のステータス強化より強い「ビルドの完成形」が見える。
- Unity シーンやPrefabに依存せず、起動時にランタイム生成されるプロトタイプ構成を採用している。
- 画像・音声・UI素材の生成、検証、失敗事例、改善方針をドキュメント化している。

## OSSとしての価値

- Unity で survivor-like / defense roguelite を試作する実例として読める。
- `Resources/Skins` と `Resources/Audio` による差し替え構造があり、コードを触らず素材変更を試せる。
- 検証スクリプトにより、文字化け・括弧崩れ・画像欠落・音声欠落を確認できる。
- AI支援開発における失敗と再発防止策をポストモーテムとして残しており、制作プロセスの学習価値がある。

## 技術構成

- Engine: Unity 6000.4
- Language: C#
- UI: UGUI
- Rendering: 2D orthographic
- Main file: `Assets/Scripts/CoreLanternGame.cs`
- Assets: `Assets/Resources/Skins`, `Assets/Resources/Audio`
- Tools: PowerShell validation and generation scripts under `Tools/`

## 現在の規模

- 総ファイル数: 6,810
- 公開候補ファイル数: 3,087
- Git管理済みファイル数: 1,852
- コミット数: 12
- メインC#実装: 約16,031行

## 現在の注意点

- ライセンスは未確定。OSS応募前に `LICENSE`, `ASSET_LICENSE.md`, `THIRD_PARTY_NOTICES.md` を確定する必要がある。
- `CoreLanternGame.cs` は現時点で巨大な単一ファイル。プロトタイプ都合の設計であり、将来的には段階的な分割を予定する。
- AI生成素材と仮BGM/SEのライセンス・使用可否を公開前に棚卸しする必要がある。

## 応募時の短い説明

Eggcore Protocol is a Unity survivor-roguelite prototype about protecting a living Data Egg with an evolving partner. It combines automatic combat, central-core defense, route-based evolution, cross-evolution builds, and documented AI-assisted asset iteration.

## 審査で見てほしいポイント

1. コア防衛とヴァンサバライク移動の緊張感
2. 相棒進化とクロス進化によるビルド表現
3. Unityプロトタイプを短期間で検証・改善するためのドキュメントとツール
4. 失敗事例を隠さずポストモーテム化して再発防止に落とし込んだ開発プロセス
