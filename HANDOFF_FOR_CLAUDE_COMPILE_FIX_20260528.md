# CoreLanternUnity Compile Fix Handoff - 2026-05-28

## 2026-05-28 Codex Verification

Claude の修正後に Codex が再確認。

- active code の奇数ダブルクォート scan: no output
- Unity Roslyn syntax check: passed
- Remaining output is warnings only:
  - obsolete `Object.FindObjectOfType<T>()`
  - unused accessibility/danger HUD fields
  - `bossModuleReady` assigned nowhere

The original compile-blocking syntax errors are considered fixed.

## Purpose

Unity is currently blocked by compile errors in `Assets/Scripts/CoreLanternGame.cs`.
Codex started recovering mojibake/corrupted Japanese strings, but the file is not confirmed clean yet.
Use this note so Claude/Codex can continue without re-discovering the same damage.

## Current Symptom

Unity Console originally showed:

- `CoreLanternGame.cs(16238,60): error CS1056: Unexpected character '�'`
- `CoreLanternGame.cs(16571,1): error CS1026: ) expected`
- `CoreLanternGame.cs(16571,1): error CS1733: Expected expression`
- `CoreLanternGame.cs(16571,1): error CS1002: ; expected`
- `CoreLanternGame.cs(16571,1): error CS1513: } expected`

The visible line numbers may shift after fixes.

## Likely Cause

Several Japanese strings/comments in `CoreLanternGame.cs` were saved or transformed with bad encoding.
The damage pattern is:

- Japanese strings became mojibake such as `桁E��`, `裁E��`, `ガーチE`.
- Some closing quotes disappeared.
- Some active code was swallowed into `//` comments, for example `list.Add(new PartnerOption(`.
- Star rarity checks were corrupted, for example `stars[i] == '☁E`.
- Some `Contains("...")` checks lost closing quotes.

This is not a gameplay logic bug. It is source text corruption.

## Work Already Done By Codex

Codex repaired many active syntax breaks in `CoreLanternGame.cs`, including:

- rollout flags and generated sprite fields near the top of the file
- `BuildUpgrades()` by wrapping the old corrupt body in `#if false` and adding a clean replacement
- many module/card/evolution/fusion/UI Japanese strings
- many declarations that had been swallowed by comments
- boss alert/cutscene text and several result/options strings
- `GetGeneratedModuleIcon`, `GetModuleTitleAccent`, `GetModuleTitleIcon`
- `CanOfferUpgrade`, combo recipe names, fusion choice text, wave hint text

An active odd-quote scan had no output after those fixes, but compile still found remaining syntax errors.

## Remaining Known Hotspots

Inspect these areas first in `CoreLanternGame.cs`.
Line numbers are approximate and may move.

1. `BuildPartnerPool()` around the Wraith/Genesis/Halo/Pulse/Solar entries
   - Several comment lines still contain mojibake.
   - Check that every partner entry starts with active `list.Add(new PartnerOption(`.
   - Clean partner descriptions:
     - Drift Fox: `回避型\n移動が速く、Phase回避を最初から持つ桃色の狐。`
     - Iron Bear: `装甲型\nHP+2と接触反撃を持つ重装甲の黒熊。`
     - Wraith Lynx: `近接型\nオーラ・接触・吸血・KBの近接最強白狼。`
     - Genesis Core: `全種解放【隠し】\n4種リンクを全て解放した最強の総合素体。`
     - Halo Caster: `ファンネル型\n3機の自律ビットが敵を自動攻撃する。`
     - Pulse Hydra: `レーザー型\n直線上の敵を全員貫通ダメで焼く高火力型。`
     - Solar Anchor: `コア共鳴型\nコアの周りに常時ダメリング。コア近接で攻撃30%。`

2. Boss summon log around `SpawnBossMinions()`
   - Broken line currently resembles:
     `AddEventLog("GLITCH SUMMON: Runner × + runnerCount + (phantomCount > 0 ? " + Phantom × + phantomCount : ""));`
   - Replace with valid interpolation:
     `AddEventLog($"GLITCH SUMMON: Runner x{runnerCount}" + (phantomCount > 0 ? $" / Phantom x{phantomCount}" : ""));`

3. `FormatCardRarity(string stars)`
   - Broken char literal currently resembles `stars[i] == '☁E`.
   - Replace with:
     `if (stars[i] == '★')`

4. `GetUpgradeAccentColor(...)`
   - Broken `title.Contains(...)` strings still exist.
   - Replace corrupted checks with safe values:
     - Guard group: `ガード`, `GUARD`, `コア`, `反撃`, `リカバリー`
     - Power group: `パワー`, `バースト`, `ヘビー`, `炎`, `ボス`, `Break`
     - Defensive group: `ガード`, `HP`, `コア`, `修復`, `回復`, `リカバリー`, `リング`, `Sync`, `Data`

## Recommended Verification Commands

From repository root:

```powershell
$p='.\CoreLanternUnity\Assets\Scripts\CoreLanternGame.cs'
$i=1
$inactive=0
Get-Content -LiteralPath $p -Encoding UTF8 | ForEach-Object {
  $line=$_
  if ($line.Trim() -eq '#if false') { $inactive++ }
  elseif ($line.Trim() -eq '#endif' -and $inactive -gt 0) { $inactive-- }
  elseif ($inactive -eq 0) {
    $count=([regex]::Matches($line,'"')).Count
    if (($count % 2) -eq 1) { '{0}: {1}' -f $i, $line }
  }
  $i++
}
```

Then run the Unity Roslyn syntax check with preview language support:

```powershell
$mono='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\bin\mono.exe'
$csc='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\MonoBleedingEdge\lib\mono\msbuild\Current\bin\Roslyn\csc.exe'
$unity='C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Data\Managed\UnityEngine'
$refs = Get-ChildItem -LiteralPath $unity -Filter 'UnityEngine*.dll' | ForEach-Object { '/r:' + $_.FullName }
$refs += '/r:' + (Resolve-Path '.\CoreLanternUnity\Library\ScriptAssemblies\UnityEngine.UI.dll')
& $mono $csc /noconfig /target:library /langversion:preview /out:'.\CoreLanternUnity\Temp\CoreLanternSyntaxCheck.dll' $refs '.\CoreLanternUnity\Assets\Scripts\CoreLanternGame.cs'
```

If this passes, open Unity and let it recompile.

## Prevention After It Is Fixed

- Keep `CoreLanternGame.cs` encoded as UTF-8.
- Avoid large automated string rewrites on this file unless the script is UTF-8 safe.
- Prefer ASCII identifiers/comments when touching high-risk code paths; Japanese UI strings are fine but must be edited with UTF-8.
- After any bulk edit, run the odd-quote scan and Roslyn compile check before reporting completion.
- For future large Japanese text changes, edit smaller sections and verify immediately.
