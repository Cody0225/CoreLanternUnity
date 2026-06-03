# CoreLanternUnity — キャラ画像生成プロンプト集

作成日: 2026-05-24
対象: Codex / Stable Diffusion / Midjourney / DALL-E など画像生成AI
用途: 11キャラ × 4段階 (L0素体 + L1/L2/L3) × 3ルート (SPEED/POWER/GUARD) + 6クロス進化 の最終素材生成

---

## 0. 共通スタイルガイド (全画像で固定)

### Positive Style Tokens (必ず含める)

```
neon cyberpunk creature, digital data lifeform, holographic shell,
clean rim light, emissive accents, transparent background preferred,
high contrast, signature glow, geometric mecha details,
full body, centered composition, character art, game asset,
3/4 top-down readable silhouette, crisp sprite-like digital illustration,
clean edges, readable at 48px, 512x512 to 1024x1024
```

### Negative Prompts (全画像で除外)

```
text, watermark, signature, multiple characters, low quality, blurry,
realistic photo, oil painting noise, anatomical errors, extra limbs,
background clutter, cluttered scene, ugly, deformed, fan art reference,
copyrighted character, established monster franchise, mascot franchise silhouette,
familiar IP look, direct parody, over-detailed noisy texture
```

### 出力規格

| 項目 | 値 |
|---|---|
| 解像度 | 1024×1024 (生成) → 512×512 (ゲーム配置) |
| 背景 | 透明 PNG (or 単色グレー後で除去) |
| ファイル名 | 素体 `Partner_S{species}_L0.png` / ルート `Partner_S{species}_R{route}_L{level}.png` / クロス `Partner_S{species}_F{fusion}_L{level}.png` |
| 配置先 | `Assets/Resources/Skins/` |
| ライセンス | 商用OK・オリジナル必須 (既存IPと類似禁止) |

### 進化段階の共通ルール

| Lv | 特徴 |
|---|---|
| **L0** | マスコット感、子供っぽい、シンプルな1モチーフ、装備なし |
| **L1** | ルート武装が初登場、サイズ +10%、発光部分が増える |
| **L2** | プロポーション変化、戦闘役割が明白、サイズ +25% |
| **L3** | ヒーロー形態、サイズ +40%、シグネチャシルエット確立 |

### ルート別シェイプ言語 (全キャラ共通)

| ルート | キーワード | 主色 |
|---|---|---|
| **SPEED** | slim body, boosters, blade fins, motion trails, agile pose | cyan / electric blue |
| **POWER** | wide shoulders, horns, cannons, furnace cores, heavy stance | orange / molten gold |
| **GUARD** | rounded armor, shields, rings, shell plates, defensive pose | green / mint |

### L2 生成ルール (個別L2プロンプトがない箇所の補完)

各キャラ本文は L1 と L3 が中心なので、L2 は以下を **L1プロンプト + キャラidentity + ルート別シェイプ言語** に追加して生成する:

```
mid evolution form, 25% larger than base, stronger silhouette than L1,
combat role clearly visible, more armor and emissive channels,
halfway between cute partner and hero form, no final-form excess,
same species identity preserved, clean readable game sprite
```

---

## 1. Cobalt Pup (style 1) — バランス型 / 青狼

### キャラ identity
小型サイバー狼。三角の大きな耳、データテール (尻尾がスカーフ状)。
チームのリーダー的存在で、シルエットは確信に満ちた小柄な姿。

### L0 (素体)
```
small cyber wolf pup, large triangular ears with glowing inner blue,
slim body, holographic data scarf-tail, soft cobalt blue and white,
confident standing pose, bright friendly eyes, mascot proportions,
clean simple design, signature color: cobalt blue (#1F77D6)
```

### L1 SPEED — スピード翼狼
```
cyber wolf pup with cyan booster fins on shoulders and tail,
small jet trails behind heels, slimmer silhouette, agile running pose,
glowing blue rim light, additional ear booster antennas,
signature color: bright cyan (#22D8FF)
```

### L1 POWER — パワー角狼
```
cyber wolf pup with twin orange horn-cannons on forehead,
heavier shoulders with vent plates, planted stance, growling expression,
furnace orange glow at chest, paw spikes,
signature color: molten orange (#FF8C24)
```

### L1 GUARD — ガード盾狼
```
cyber wolf pup with green hexagonal shield collar around neck,
ring of mint-green energy plates at hips, sitting alert defensive pose,
rounded armor plates on back, calm green glow,
signature color: mint green (#5BFF8E)
```

### L3 SPEED 最終 — Ion Lance Pup
```
sleek cyber wolf in full battle form, long ion blade-tail extended,
quad cyan booster wings, racing stance about to dash,
dramatic motion blur lines, bright cyan halo at heels,
35% larger than L0, hero silhouette
```

### L3 POWER 最終 — Furnace Pup
```
heavy cyber wolf with twin shoulder cannons firing orange plasma,
massive horns curved forward, lava-vein glow on body,
planted battle stance with cracks of light, intimidating
```

### L3 GUARD 最終 — Bastion Pup
```
fortress cyber wolf with rotating green energy rings around body,
heavy hexagonal shield armor on back and shoulders,
calm guardian pose, green data shield projected in front
```

---

## 2. Ember Drake (style 2) — 弾幕型 / 橙竜

### キャラ identity
小型サイバードラゴン。短い羽根、長い火炎テール。
連射特化、シルエットは流線型。

### L0 (素体)
```
small cyber dragon hatchling, short flame-feather wings,
slender body with orange and dark-red plates,
long tail with three small fire-data crystals,
playful flying pose, signature color: ember orange (#FF7032)
```

### L1 SPEED — 連射翼ドラゴン
```
cyber dragon with cyan-tinted flame wings, dual barrel chest cannons,
long flowing tail with motion trail, alert hovering pose,
mixed cyan and orange glow, dive-bombing silhouette
```

### L1 POWER — 重砲ドラゴン
```
heavy cyber dragon with massive shoulder cannons, twin chest furnaces,
larger horns with molten cracks, grounded stance breathing fire,
deep orange and crimson glow, intimidating
```

### L1 GUARD — 龍鱗ドラゴン
```
armored cyber dragon with green-tinted scale plates,
rounded shoulder shields with mint runes, defensive coiled pose,
flame core reduced, scales emphasized, signature ember-mint mix
```

### L3 SPEED 最終 — Tempest Wyrm
```
sleek cyber wyrm with quad cyan-flame wings, multiple chest cannon barrels,
racing serpentine pose, motion trails of orange embers and cyan sparks,
hero form 40% larger, double tail-trail
```

### L3 POWER 最終 — Inferno Drake
```
massive cyber dragon with chest furnace exposed, twin shoulder mortars,
crown of horns with molten gold, roaring stance,
lava-vein body, fire halo at feet
```

### L3 GUARD 最終 — Aegis Wyrm
```
fortified cyber dragon with full scale armor, green energy shields,
serpentine guardian pose around an invisible core,
flame muted to green, scales prominent, calm protector
```

---

## 3. Sage Hare (style 3) — 防衛型 / 緑兎賢者

### キャラ identity
小型サイバーラビット賢者。長い耳、ローブ風プレート。
コア防衛特化、シルエットは静かで賢明。

### L0 (素体)
```
small cyber rabbit sage, long upright ears with mint runes,
hooded data robe over slim body, calm meditative standing pose,
green and white palette, holds a tiny floating orb,
signature color: sage mint (#5BD89A)
```

### L1 SPEED — 韋駄天兎
```
cyber rabbit with cyan booster bands on legs, robe shortened for speed,
quick racing stance, ear-mounted antennas, mint+cyan glow,
trails of mint sparks
```

### L1 POWER — 賢者杖兎
```
cyber rabbit holding a glowing orange staff, robe with furnace seams,
casting pose with raised paw, intense stare,
mint robe with orange accents on staff
```

### L1 GUARD — 守護兎
```
cyber rabbit with full mint-green guardian armor, rune-engraved chest plate,
defensive crossed-arm pose, halo of small green orbs orbiting,
expanded ears with shield panels
```

### L3 SPEED 最終 — Cycle Hare
```
sleek cyber rabbit in racing form, full body cyan-mint trail,
quad booster jets, dashing forward pose, robe replaced by flight suit,
hero proportions
```

### L3 POWER 最終 — Sage Conjurer Hare
```
cyber rabbit master with twin floating staves orbiting body,
robe with massive sleeves emitting orange data spells,
levitating pose, halo of golden runes
```

### L3 GUARD 最終 — Guardian Hare
```
fortress cyber rabbit with full mint armor, large kite shield,
guardian stance protecting an unseen core,
ears swept back like helm wings, calm protector
```

---

## 4. Hex Cat (style 4) — 連鎖型 / 紫魔猫

### キャラ identity
小型サイバー猫の魔法使い。三日月マーク、長い尻尾。
チェイン連鎖と残像追撃が得意、シルエットは細長く神秘的。

### L0 (素体)
```
small cyber sorcerer cat, slender body with long tail,
purple and magenta palette with crescent moon mark on forehead,
mystical poised stance, glowing purple eyes,
floating data sparkles around paws,
signature color: hex magenta (#A647FF)
```

### L1 SPEED — 残像猫
```
cyber cat with cyan motion trail, multiple after-images visible,
slim body about to teleport, glowing crescent intensified,
purple+cyan glow, agile pose
```

### L1 POWER — 呪術砲猫
```
cyber cat wielding a magenta orb cannon on shoulder,
robe with arcane runes, casting pose with floating sigil,
deeper purple, gold accents on cannon
```

### L1 GUARD — 結界猫
```
cyber cat with mint barrier dome around body, defensive crouching pose,
guardian sigils orbiting, mixed purple-mint glow,
calm protective expression
```

### L3 SPEED 最終 — Phantom Hex
```
sleek cyber cat with permanent after-image train (3-4 copies),
quad-trail dashing pose, transparent chains of magenta energy,
hero form, ethereal
```

### L3 POWER 最終 — Hex Mage
```
cyber cat in mage form, twin orbital sigils, robe expanded into wings,
casting massive purple beam pose, floating regally,
crown of crescent moons
```

### L3 GUARD 最終 — Hex Warden
```
cyber cat with full barrier dome, multiple shield orbs orbiting,
seated meditation guardian pose, calm magenta-mint aura,
crescent mark amplified
```

---

## 5. Drift Fox (style 5) — 回避型 / 桃狐

### キャラ identity
小型サイバー狐、Phase回避能力。スリムでアジャイル。
桃ピンクとマゼンタ、シルエットは流れるような。

### L0 (素体)
```
small cyber fox kit, slim body with flowing fur-data tail,
bright pink and magenta palette, mischievous grin,
agile poised stance ready to dodge, holographic ear tips,
signature color: drift pink (#FF66B8)
```

### L1 SPEED — 風影狐
```
cyber fox with cyan-pink trail, mid-leap dodging pose,
double tail with motion blur, eye glints with cyan,
agile twisting silhouette
```

### L1 POWER — 影爪狐
```
cyber fox with orange chest core, twin clawed gauntlets glowing,
aggressive lunging pose, pink fur with orange accents,
power core visible in chest
```

### L1 GUARD — 鏡反狐
```
cyber fox with mint reflective armor panels, defensive sidestep pose,
glass-like shield around shoulders, calm focused expression,
pink-mint mix
```

### L3 SPEED 最終 — Mirage Fox
```
sleek cyber fox phasing in and out, transparent body parts,
multiple after-images, cyan-pink dual trail,
graceful aerial dodge pose, hero form ethereal
```

### L3 POWER 最終 — Striker Fox
```
muscular cyber fox with chest furnace and claw weapons,
aggressive pouncing pose, orange-pink flame trail,
prominent fangs, predator silhouette
```

### L3 GUARD 最終 — Aegis Fox
```
cyber fox with mirrored mint armor, defensive guardian stance,
reflective shield surface, calm protective expression,
pink+mint guardian aura
```

---

## 6. Iron Bear (style 6) — 装甲型 / 黒熊

### キャラ identity
中型サイバーベア、重装甲特化。短い四肢、大きな胸部。
最高 HP、シルエットはどっしりと安定。

### L0 (素体)
```
small cyber bear cub, stocky body with thick armor plates,
dark grey and gold palette, calm grounded stance,
small visor over eyes, signature color: iron grey (#5A6878) with gold accents
```

### L1 SPEED — 高速ベア
```
cyber bear with cyan booster jets on back, slimmer profile,
charging forward pose with motion blur, agile despite bulk,
mixed grey-cyan glow
```

### L1 POWER — 鋼鉄ベア
```
heavy cyber bear with massive shoulder cannons, gold furnace chest,
aggressive standing pose, gold-orange accents on armor,
muscular silhouette
```

### L1 GUARD — 重装ベア
```
fortress cyber bear with full mint guardian armor, large pauldrons,
defensive crouching pose blocking attack, mint shield aura,
calm immovable stance
```

### L3 SPEED 最終 — Velocity Bear
```
sleek cyber bear with quad jets, charging stampede pose,
cyan trail behind, hero form retains bulk but fast,
intimidating speed predator
```

### L3 POWER 最終 — Titan Bear
```
massive cyber bear with twin gatling cannons, gold chest furnace,
roaring battle stance, lava cracks in armor,
king-sized hero proportions
```

### L3 GUARD 最終 — Bastion Bear
```
fortress cyber bear with mountain-sized mint armor,
guardian stance with green energy shield projected,
calm immovable protector, hero proportions
```

---

## 7. Wraith Lynx (style 7) — 近接型 / 白狼

### キャラ identity
高速白狼、近接接触戦闘特化。シルバーシアン、半透明な部分。
オーラ・吸血・接触ダメージ。

### L0 (素体)
```
ghostly white cyber lynx, semi-transparent body parts (legs/tail),
silver-cyan palette with violet glowing eyes,
agile hunting pose, sharp ear tufts,
ghost-like translucent fur trail,
signature color: wraith silver (#C7E5FF)
```

### L1 SPEED — 影狼
```
wraith lynx with full motion blur, multiple after-images,
sprint pose with cyan trail, transparent body parts intensified,
ethereal speed silhouette
```

### L1 POWER — 牙狼
```
wraith lynx with extended glowing claws and fangs,
violet orange chest core, predator pouncing pose,
solid body with selective transparency
```

### L1 GUARD — 守魂狼
```
wraith lynx with mint translucent aura, defensive guardian pose,
ghost shield around body, calm sentinel expression,
silver-mint glow
```

### L3 SPEED 最終 — Phantom Lynx
```
sleek wraith lynx fully ghostly, body partly transparent,
quad ghost trails, ethereal sprint pose,
violet-cyan aura, hero proportions
```

### L3 POWER 最終 — Hunter Lynx
```
muscular wraith lynx with massive glowing claws,
violet chest core fully exposed, savage pouncing pose,
prominent fangs, predator silhouette
```

### L3 GUARD 最終 — Sentinel Lynx
```
wraith lynx with full mint guardian aura, large translucent shield,
calm protective stance, ghost armor plates,
silver-mint hero form
```

---

## 8. Genesis Core (style 8) — 全種解放型 / 隠し総合

### キャラ identity
全 4 種リンク (Nova/Bulwark/Siphon/Phase) を内蔵した「すべての色を持つ」素体。
ハイブリッド見た目、複数色のグラデーション、最も大きい素体。

### L0 (素体)
```
prismatic data creature, body color shifts between orange/green/yellow/pink in gradient,
four small orbital sigils around body (one each color),
majestic standing pose, larger than other base partners,
crystalline central core visible in chest,
signature color: prismatic gold-cyan (#FFE852 / #5DDCFF)
```

### L1 SPEED — 神速ジェネシス
```
genesis creature with cyan dominant glow, all 4 orbital sigils form a ring,
agile dash pose, prismatic trails,
unified hero silhouette
```

### L1 POWER — 神域ジェネシス
```
genesis creature with orange dominant glow, twin shoulder cannons,
all 4 sigils orbiting as combat support,
king-sized power pose
```

### L1 GUARD — 神聖ジェネシス
```
genesis creature with green dominant glow, full guardian armor,
4 sigils form protective barrier ring,
imposing defensive hero pose
```

### L3 SPEED 最終 — Aurora Genesis
```
hero genesis form with prismatic aurora trail,
all 4 sigil colors blended into rainbow light,
swift hero pose, transcendent
```

### L3 POWER 最終 — Solar Genesis
```
hero genesis form with chest sun-core blazing,
all 4 sigils transformed into solar plates,
divine king pose
```

### L3 GUARD 最終 — Eternal Genesis
```
hero genesis form with full prismatic guardian armor,
all 4 sigils form a halo, sacred protector pose,
serene divine guardian
```

---

## 9. Halo Caster (style 9) — ファンネル型

### キャラ identity
中型ヒューマノイドメカ、自律ビットを操る術者。
深紺紺色 + シアンハロー。ビット (小型衛星) が周囲を周回。

### L0 (素体)
```
small humanoid cyber mage in dark navy robe, hooded silhouette,
3 small cyan funnel bits orbiting around shoulders,
floating pose with arms relaxed at sides,
glowing cyan halo above head, signature color: navy + cyan halo (#2E66FF)
```

### L1 SPEED — 高速ファンネル
```
halo caster with extended booster jets, all funnel bits emit motion trails,
agile floating pose, racing forward, cyan-blue speed lines,
swift silhouette
```

### L1 POWER — 強砲ファンネル
```
halo caster with heavy shoulder cannons, funnel bits enlarged with twin barrels,
casting pose with arms raised summoning bits,
orange-cyan power glow
```

### L1 GUARD — 守護ファンネル
```
halo caster with mint guardian aura, funnel bits forming defensive ring,
calm sentinel pose, large halo with shield panels,
mint-cyan glow
```

### L3 SPEED 最終 — Velocity Caster
```
sleek halo caster with 6 funnel bits in trail formation,
flight pose with arms outstretched, cyan-blue dash,
hero form, master conductor of swarm
```

### L3 POWER 最終 — Aegis Caster
```
heavy halo caster with massive bit cannons, robe replaced by combat armor,
commanding cast pose, all bits firing simultaneously,
orange-cyan firepower
```

### L3 GUARD 最終 — Sanctum Caster
```
halo caster with full mint guardian robe, bits forming shield barrier,
serene protector stance, halo expanded with rune plates,
mint-cyan sacred glow
```

---

## 10. Pulse Hydra (style 10) — 持続レーザー型

### キャラ identity
中型サイバー多頭ヘビ。深紅マゼンタ、ピンクの発光チャネル。
体から長いビームを発射する持続砲台型。

### L0 (素体)
```
small cyber multi-head serpent, deep crimson body with magenta glow channels,
3 small heads, coiled poised pose, glowing pink eyes,
energy vein on body, signature color: pink magenta (#FF73DA)
```

### L1 SPEED — 高速ハイドラ
```
pulse hydra with cyan beam channels, slimmer coil,
all 3 heads dashing forward, motion trail of pink-cyan,
agile serpentine pose
```

### L1 POWER — 重砲ハイドラ
```
pulse hydra with thick beam emitters on each head, massive chest core,
all heads charging beams simultaneously, intimidating stance,
crimson-magenta power
```

### L1 GUARD — 反射ハイドラ
```
pulse hydra with mint reflective scales, defensive coiled pose,
shield panels on shoulders, beams diffused into shields,
crimson-mint mix
```

### L3 SPEED 最終 — Velocity Hydra
```
sleek pulse hydra with 5 heads, all firing thin precision beams,
agile dashing serpentine pose, pink-cyan light trails,
hero form fast assassin
```

### L3 POWER 最終 — Inferno Hydra
```
massive pulse hydra with 5 heads, each with thick laser cannon,
chest core exposed and blazing, dominating stance,
crimson-magenta hero form, all beams charged
```

### L3 GUARD 最終 — Aegis Hydra
```
pulse hydra fully armored in mint reflective shells,
all heads defensive guardian stance, beams form shield wall,
calm protective pose, hero form
```

---

## 11. Solar Anchor (style 11) — コア共鳴型

### キャラ identity
中型サイバー柱モチーフ生物。コアに「碇」のように繋がる存在。
ゴールド + シアンエネルギー、シルエットは安定した三角形ベース。

### L0 (素体)
```
cyber tower-creature, golden body with cyan energy channels,
anchor-shaped chest core glowing, stable triangular silhouette,
short stout legs, no wings (designed to stay grounded),
floating data tether trail behind, signature color: solar gold (#FFD744) + cyan
```

### L1 SPEED — 機動アンカー
```
solar anchor with cyan booster fins, slightly slimmer profile,
running pose but still grounded-feeling, gold-cyan trail,
agile guardian
```

### L1 POWER — 重砲アンカー
```
heavy solar anchor with massive shoulder turrets, gold flames at base,
planted aggressive stance firing twin cannons,
orange-gold furnace core exposed
```

### L1 GUARD — 守護アンカー
```
fortress solar anchor with full mint guardian armor,
ring of gold-cyan shield around body, calm sentinel pose,
expanded silhouette protecting core
```

### L3 SPEED 最終 — Pulse Anchor
```
sleek solar anchor with quad cyan jets, swift guardian pose,
gold-cyan aura trail, hero form retains stable silhouette,
swift but rooted
```

### L3 POWER 最終 — Solar Bastion
```
massive solar anchor with twin orbital cannons, chest sun-core blazing,
dominating king pose, gold-orange furnace flames,
hero god form
```

### L3 GUARD 最終 — Eternal Anchor
```
fortress solar anchor with halo of gold-mint shield rings,
serene guardian stance protecting core, full guardian armor,
hero protector form, large stable silhouette
```

---

## 12. クロス進化 (Fusion) — 6種 × L0〜L3 フル展開

### 設計原則

メインキャラがリンクを **吸収して同化** した姿。
「キャラ + リンク」ではなく「キャラ自身が変質した」見た目に。

下記の各 fusion プロンプトは **メインキャラのシルエットを保ちつつ fusion 色/モチーフを足す** 用途で、ベースキャラ名と組み合わせて使う想定:

```
[Base Partner full prompt] + [Fusion Add-on prompt]
例: Cobalt Pup base + Nova Aegis L2 → Cobalt Pup の体に Nova Aegis L2 のモチーフ
```

ただし「単独 fusion 形態」としても通用するプロンプトを記載 (キャラ非依存生成も可能)。

### L0 → L3 共通進化ルール

| Lv | fusion 要素の見え方 |
|---|---|
| **L0** | リンク色が体に芽生え始める (10% モチーフ、控えめ) |
| **L1** | リンク装備が初登場 (30% モチーフ、身体の一部に) |
| **L2** | 体が変質、戦闘役割が明白 (60% モチーフ、装甲化) |
| **L3** | 完全融合ヒーロー形態 (90% モチーフ、主役のシルエット痕跡のみ残る) |

---

### 12-1. Nova Aegis (Nova + Bulwark)

**コンセプト**: 攻撃と防衛の両立。「燃える盾」の戦士。
**シグネチャカラー**: orange (#FF8C24) + mint green (#5BFF8E)
**シルエット**: 炎核を中央に、緑の盾リングが周回

#### L0 (素体に芽生え)
```
base partner silhouette with subtle orange ember spots on chest,
small green shield fragment hovering near shoulder,
hint of fusion forming, color shifts to warm orange tint,
fusion add-on: 10% intensity, original creature still dominant
```

#### L1 (初期装備)
```
partner with orange chest core forming, single green shield ring around hip,
small flame trail behind, defensive-ready combat stance,
orange + mint balanced glow, fusion partially formed
```

#### L2 (中期 — 役割確立)
```
partner with full orange furnace chest, twin green shield rings rotating around body,
heavier shoulders with flame vents, planted offensive-defensive pose,
mint shield projecting forward when needed, hero proportions emerging,
clear "burning shield warrior" silhouette
```

#### L3 (最終ヒーロー)
```
hero fusion creature with massive chest furnace core blazing orange,
quad rotating green-mint shield rings forming a protective sphere,
twin shoulder cannons combining flame and shield motifs,
imposing balanced pose: one hand raised for shield, one for attack,
dramatic dual-color aura (orange flame + mint shield wall),
king of offense-defense, 40% larger than L0,
signature silhouette: chest sun-core inside rotating ring fortress
```

---

### 12-2. Photon Siphon (Nova + Siphon)

**コンセプト**: 火力 + データ回収。「黄金の追跡者」。
**シグネチャカラー**: orange (#FF8C24) + lime yellow (#FFE852)
**シルエット**: 黄金核 + 周回データクリスタル

#### L0 (素体に芽生え)
```
base partner with subtle golden data sparkles around body,
small yellow data crystal hovering near tail/head,
orange-yellow shimmer in eyes, fusion forming gently
```

#### L1 (初期装備)
```
partner with golden chest core glowing, data collector hooks at shoulders,
yellow-orange light trail, agile data-hunting pose,
small orbiting data crystals (2-3), graceful flight
```

#### L2 (中期 — 役割確立)
```
partner with bright golden body channels, 4-6 data crystals orbiting,
expanded collector arms, flame-data hybrid wings,
levitating with golden aura, hero form emerging,
clear "golden data hunter" silhouette
```

#### L3 (最終ヒーロー)
```
hero fusion creature with massive golden sun-core at chest,
8 data crystals orbiting in elegant pattern, flame-data wings spread wide,
levitating majestically with golden-orange aurora,
twin energy collectors on shoulders, divine harvester pose,
hero form 40% larger, golden halo above head,
signature silhouette: sun-core radiating data crystal orbits
```

---

### 12-3. Core Bastion (Bulwark + Siphon)

**コンセプト**: コア防衛特化。「動く要塞」。
**シグネチャカラー**: green (#5BD89A) + gold-yellow (#FFCC44)
**シルエット**: 重装甲ベース + 黄金データシールド

#### L0 (素体に芽生え)
```
base partner with subtle green armor plates forming, small gold data tendrils,
slightly heavier silhouette, calm grounded stance,
green-gold hint, fusion gently appearing
```

#### L1 (初期装備)
```
partner with green chest plate, gold data shield ring at hip,
defensive crouching pose, expanded shoulder pauldrons,
mint-gold mix glow, calm sentinel feel
```

#### L2 (中期 — 役割確立)
```
heavily armored partner with full green guardian plates,
3-4 gold data shield rings orbiting body, fortress crouch,
expanded silhouette, gold data tendrils acting as auxiliary defense,
clear "moving fortress" identity
```

#### L3 (最終ヒーロー)
```
hero fusion creature as a walking citadel,
massive green guardian armor covering body,
6 gold shield rings forming a defensive sphere around an inner core,
seated guardian pose like a monk-fortress,
gold-green aura, immovable presence,
hero form 45% larger, hexagonal energy shields projecting in all directions,
signature silhouette: kneeling armored colossus inside ring fortress
```

---

### 12-4. Nova Phantom (Nova + Phase)

**コンセプト**: 火力 + 残像。「炎の幻影軍団」。
**シグネチャカラー**: orange (#FF7032) + violet (#B080FF)
**シルエット**: 炎核 + 複数の紫残像

#### L0 (素体に芽生え)
```
base partner with subtle orange ember at chest, faint violet ghost trail,
slightly transparent edges, alert pose, fusion forming,
warm orange + cool violet hint
```

#### L1 (初期装備)
```
partner with orange chest core forming, single violet ghost copy behind,
flame trail with phase-shift effect, agile dashing pose,
orange-violet dual glow
```

#### L2 (中期 — 役割確立)
```
partner with bright orange core, 2-3 violet after-images visible,
flame wings with phase-shift, lunging attack pose,
body partially transparent, hero proportions,
clear "phantom flame striker" silhouette
```

#### L3 (最終ヒーロー)
```
hero fusion creature with massive chest flame core,
4-5 violet phase clones surrounding main body in attack formation,
flame wings each with phase trail, multi-image strike pose,
dramatic orange-violet aurora, transcendent ghost-fire form,
hero form 40% larger, leader of phantom army,
signature silhouette: blazing core with ring of ghost flames
```

---

### 12-5. Aegis Drift (Bulwark + Phase)

**コンセプト**: 反射 + 回避。「鏡の戦士」。
**シグネチャカラー**: mint green (#5BFF8E) + drift pink (#FF66B8)
**シルエット**: 鏡面装甲 + ピンク残像

#### L0 (素体に芽生え)
```
base partner with subtle mint plates forming, faint pink ghost trail,
slightly mirror-like surface on shoulders, agile alert pose,
mint-pink hint, fusion gently emerging
```

#### L1 (初期装備)
```
partner with mint reflective shoulder plates, pink phase trail behind,
defensive sidestep pose, glass-like mirror panels,
mint-pink balanced glow, agile guardian feel
```

#### L2 (中期 — 役割確立)
```
partner with full mirror armor, 2-3 pink ghost copies dodging in sync,
reflective body surface visible, gracefully evading pose,
mint shield panels with pink phase shift, hero proportions,
clear "mirror dancer warrior" silhouette
```

#### L3 (最終ヒーロー)
```
hero fusion creature with full mirror-mint armor,
4 pink phase copies dancing around in defensive ring,
reflective shield body that bounces light, ethereal warrior pose,
mint-pink aurora trail, transcendent dancer-guardian,
hero form 40% larger, blade dancer of light,
signature silhouette: chrome warrior at center of pink ghost ring
```

---

### 12-6. Photon Wraith (Siphon + Phase)

**コンセプト**: データ回収 + 回避。「黄金の死神」。
**シグネチャカラー**: gold-yellow (#FFCC44) + ghost white (#C7E5FF)
**シルエット**: 半透明黄金体 + データクリスタル幻影

#### L0 (素体に芽生え)
```
base partner with subtle golden data motes, slight body transparency at edges,
faint ghost trail in white-gold, mysterious pose,
fusion forming with ethereal hint
```

#### L1 (初期装備)
```
partner with semi-transparent golden body, data crystal collector floating beside,
ghostly white trail, gliding pose, mysterious hooded feel,
gold-white dual glow
```

#### L2 (中期 — 役割確立)
```
partner mostly transparent gold body, 3-4 data crystals orbiting in ghost wake,
reaper-like silhouette with collector scythe forming,
gold-white aurora, gliding stance, hero proportions emerging,
clear "data reaper" identity
```

#### L3 (最終ヒーロー)
```
hero fusion creature as ethereal data reaper,
fully translucent golden body with internal data flow visible,
massive ghost-data scythe weapon, 6 orbital crystals in death-procession,
gliding majestic pose, gold-white aurora wrapping body,
transcendent harvester king, 40% larger,
signature silhouette: translucent reaper with crystal halo
```

---

### クロス進化のキャラ別差別化のヒント

11キャラ × 6 fusion = 66通りすべて作るのは膨大なので、3パターンで運用可:

**Option A (省力)**: fusion archetype 24枚のみ (上記 6×4)。カードに「Cobalt Pup as Nova Aegis」として共通スプライト使用
**Option B (中間)**: fusion archetype + キャラ別 L3 のみ (24 + 11×6 = 90枚)。L3 だけキャラ感を残す
**Option C (フル)**: 全キャラ × fusion × level = 11×6×4 = 264枚 (理想だが膨大)

**推奨**: Phase 1 は Option A、Phase 2 で Option B にアップグレード。Option C は商用ローンチ後の DLC レベル。

---

## 13. 生成優先順位 (実装計画)

### Phase 1 — 必須最小セット

**推奨 1A: L0素体 + 各ルートL3**  
最低限「素体の魅力」と「進化後の派手さ」を確認するセット。

- L0素体 × 11 = 11枚
- L3最終ヒーロー × 11キャラ × 3ルート = 33枚
- **合計 44枚**

**削減 1B: L0素体 + 代表L3 1枚**  
まずシルエット審査だけを速く回す場合。

- L0素体 × 11 = 11枚
- 代表L3 × 11 = 11枚
- **合計 22枚**

### Phase 2 — フル進化セット
- L0 × 11 = 11枚
- R1/R2/R3 の L1-L3 × 11 × 3 × 3 = 99枚
- 必要ならルート別L0 `Partner_S{n}_R{route}_L0` × 11 × 3 = 33枚 (進化ツリー用。省略時は `Partner_S{n}_L0` にフォールバック)
- **合計 110枚 (ルート別L0なし) / 143枚 (ルート別L0あり)**

### Phase 3 — クロス進化
- **Option A**: fusion archetype 6種 × L0-L3 = 24枚
- **Option B**: Option A + キャラ別L3 11×6 = 90枚
- **Option C**: キャラ別フル 11×6×4 = 264枚

### 推奨進行

**22枚でシルエット審査 → 44枚でルート審査 → 110枚で本編差し替え → クロス進化はOption Aから段階投入。**

---

## 14. クオリティチェック (各画像の合格基準)

- ✅ 名前を見ずにシルエットでキャラ判別可能
- ✅ ルート (S/P/G) が遠目でも区別可能 (色 + 形状)
- ✅ 進化段階 L0→L3 でサイズ・装甲・発光が明確に強化
- ✅ クロス進化は「メインキャラがリンクを吸収した姿」に見える (リンク同士の合体ではない)
- ✅ 既存IPと類似していない (Pokemon/Digimon/モンハン等)
- ✅ 商用利用可能なオリジナル
- ✅ 透明PNG、エッジクリーン

---

## 15. ファイル命名規約

| パターン | 例 |
|---|---|
| `Partner_S{n}_L0.png` | `Partner_S1_L0.png` (素体) |
| `Partner_S{n}_L{lv}.png` | `Partner_S1_L2.png` (ルートなし汎用進化。必要時のみ) |
| `Partner_S{n}_R{route}_L{lv}.png` | `Partner_S1_R1_L3.png` (S1 SPEEDルート L3) |
| `Partner_S{n}_R{route}_V{variant}_L{lv}.png` | `Partner_S1_R1_V2_L3.png` (ルート + バリアント。必要時のみ) |
| `Partner_S{n}_F{fusion}_L{lv}.png` | `Partner_S1_F4_L3.png` (S1 Nova Aegis クロス L3) |
| `Partner_S{n}_F{fusion}_V{variant}_L{lv}.png` | `Partner_S1_F4_V2_L3.png` (クロス + バリアント。必要時のみ) |

**Route インデックス:**
- R1 = SPEED
- R2 = POWER
- R3 = GUARD

**Fusion インデックス:**
- F1 = Nova Aegis (Nova + Bulwark)
- F2 = Photon Siphon (Nova + Siphon)
- F3 = Core Bastion (Bulwark + Siphon)
- F4 = Nova Phantom (Nova + Phase)
- F5 = Aegis Drift (Bulwark + Phase)
- F6 = Photon Wraith (Siphon + Phase)

### species index
- S1 Cobalt Pup
- S2 Ember Drake
- S3 Sage Hare
- S4 Hex Cat
- S5 Drift Fox
- S6 Iron Bear
- S7 Wraith Lynx
- S8 Genesis Core
- S9 Halo Caster
- S10 Pulse Hydra
- S11 Solar Anchor

---

## 担当分け

- **Claude**: プロンプト設計、命名規約、生成画像のコード接続
- **Codex (or AI生成担当)**: プロンプトを使った画像生成、最適化、配置
- **ユーザー**: シルエット読みやすさ・既存IP類似性のレビュー、最終承認

## 進捗管理

- 完成画像は `Assets/Resources/Skins/Partner_S{n}_L0.png` / `Partner_S{n}_R{route}_L{lv}.png` / `Partner_S{n}_F{fusion}_L{lv}.png` に配置
- バックアップは `Assets/ArtSource/CharacterBackup_YYYYMMDD/` に保管
- 不採用画像も「rejected」フォルダに保管 (再利用検討用)
