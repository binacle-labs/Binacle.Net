---
description: Step 12 - one benchmark project per question, five of them, each with its tiers and scenarios settled, the config in shared/test/Binacle.Benchmarking, bench.just, the two benchmark scripts gone
state: ready
waits-on: "the maintainer runs the smoke recipes"
horizon: next-release
paths: ["lib/**", "vipaq/**", "tooling/**", "Binacle.Net.slnx", "justfile"]
---

# Step 12 - the bench split

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## The step

- `lib/test/Binacle.Lib.Benchmarks` becomes four projects under `lib/bench/`: `.Algorithms`, `.ResultSelection`,
  `.Racing`, `.Threshold`. `vipaq/test/Binacle.ViPaq.Benchmarks` moves to `vipaq/bench/Binacle.ViPaq.Benchmarks`,
  one project. The classes, tiers and scenarios of each are settled below; the scaling class and the JSON row
  are not written here.
- The BDN config, `AttributeOrderer` and `BenchmarkOrderAttribute` move into `shared/test/Binacle.Benchmarking`,
  the only project that references BenchmarkDotNet; every bench project references it and calls
  `BenchmarkConfig.Create()`. Not the `Testing` projects: a copy there pulls BenchmarkDotNet into every
  unit-test and measure restore. Decided 2026-09-20, built 2026-09-22.
- The friend grants: `Binacle.Lib` and `Binacle.ViPaq` each grant `$(ProjectName).Benchmarks` today, and
  `CompressionCost` constructs internal codecs. The lib grant becomes one per bench project that needs it;
  the build says which.
- Naming: the class names are in the settled sections; one namespace per project; `Generator.cs` is unused
  and goes; every `_v1` class carries one comment saying it is deleted with v1.
- `tooling/bench.just`: one recipe per binary and per tier, as settled below; `benchmarks.lib.sh` and
  `benchmarks.vipaq.sh` are absorbed and deleted. The root `justfile` gets the `mod` line and loses
  "benchmark and performance runs are still shell scripts".
- `Binacle.Net.slnx`: `/lib/bench/` and `/vipaq/bench/` folders. Docs: the lib tests doc (its class list
  and aliases), the tooling doc, the commands doc, both slice READMEs, a README in each `bench/` folder.
- The data edits the settled sections ask for travel with the binary that needs them: the curated key renames
  and `thpack1_65` in the providers, the result-selection JSON names and the two new scenarios, the vipaq
  provider comments.
- Commits: one per lib binary, one for vipaq, each with its recipes, and `bench.just` lands with the first.
  The tree builds after each.

## Settled 2026-09-21 - tiers and recipes

Three tier words, the same on every binary that has more than one: **smoke** (minutes: did my change help or
hurt), **sample** (a middle set, only where the full run is hours), **full** (everything the binary has).

- **Tiers and jobs - reworked 2026-09-22.** The shape above was replaced by the maintainer's. In short: a tier is a class-name prefix and a recipe, the
  job is the switch (`quick` on sample, `precise` on full), no plain-name aliases, no categories, no script.
  Measured 2026-09-21 on the BDN 0.15.8 source: the default job is 13-20 s per case whatever the method
  costs, `short` about 5 s. `short` keeps Mean and Ratio; it widens Error and RatioSD, so a finding that
  rests on a ratio under 1.1 needs the default job.
- **The five curated scenarios stay**, under their new names. The racing finding spreads them from 0.93x to 1.48x.
- **`ProcessorCount` goes.** The parallel processor runs `Parallel.For` over the algorithms, so a race of N
  algorithms uses at most N threads and the set already decides it. BDN's header prints the machine's cores.

Sample: 5 scenarios x 2 sets x 2 rows x 2 classes = 40 cases at the default job, about 10 minutes - the
findings rest on ratios like 1.08, which `short` would blur. Smoke: 2 scenarios, v2 only, 8 cases at `short`.

## Settled 2026-09-21 - `Binacle.Lib.Benchmarks.Threshold`

Two families, both `Loop` (baseline) vs `Parallel`, both on the synthetic ladder in
`SpecializedScalingProblemsProvider`. Production uses `Loop` everywhere; these are the evidence for whether the
parallel processors should be wired up, and from what size (the lib findings record on parallel racing, and the lib decision not to wire it up).

What the old `MultipleBins` records showed (the November 2025 runs, since deleted; ratios recomputed per
algorithm by script): Parallel is under 1.0 from 2 bins up on every machine - about 0.85 at 2 bins, 0.65 at 8,
flat after. They measured 2, 8, 14 .. 38 bins on one fixed item set; never 1 bin, never 3-7, never a change in
per-bin weight. The ladder covers exactly that gap. Per-bin weight moves the low end (2 bins: FFD 0.82, WFD
0.60), so the bins family needs every bin count and only three item levels.

- **Algorithm sets: `FFD,BFD` and `FFD,WFD,BFD` only** - the two production races.
- **Bins family: FFD and BFD only** - the multi-bin routes never run WFD.
- **`ProcessorCount` goes from both.** In `ParallelBinProcessor` it only sizes a dictionary and never reaches
  `Parallel.For`; the old param changed nothing.
- **The ladder's steps**, from the provider's volume table: 47 items fits every bin, 59 overflows the smallest,
  67 overflows bins 1-4, 79 is the top and fails the max bin for all but BFD. 3 items is where thread cost
  dominates - the one place Parallel loses.
- **1 bin is a fixed-cost row, not shape.** It stays in full as the guard that wiring parallel up must not hurt
  a 1-bin `compare-bins` request; smoke starts at 2, the first count that can win. 3 is every preset.
- **The algorithms family is lighter than Racing everywhere** (ladder max 79 items; lightest curated problem
  126) and old records over 10-202 items were flat at 0.85-1.0. It stays for the small-request end (demo
  samples: median 13 items) and the 67 -> 79 step where the algorithms take unequal time.
- **Param names are what BDN prints** - there is no display attribute. `Items`, `Bins`, `Set`; the lists become
  private fields. Rows `Loop` and `Parallel`.

| tier | classes | params | cases | at `short` |
|---|---|---|---|---|
| smoke | `Smoke_Algorithms_Packing`, `Smoke_Bins_Packing` (v2) | Items 3, 47, 67, 79; Set x2; Bins 2, 3, 7; FFD, BFD | 16 + 48 = 64 | ~5 min |
| sample | `Algorithms_Packing_v2` (a filter, all 11 items), `Sample_Bins_Packing` (Items 3, 47, 79 x Bins 1-7) | | 44 + 84 = 128 | ~11 min (default tier) |
| full | `Algorithms_Packing_v1`, `_v2`, `Bins_Packing_v1`, `_v2` | all 11 items; Bins 1-7 | 88 + 616 = 704 | ~1 h; ~3 h default job |

The extra item points in full only interpolate; full keeps the whole ladder once so a record of the curve
exists. Sample's algorithms half is a filter on the v2 full class, not its own class - the one place the
tier is not in the file name, and the report header says which job ran.

## Settled 2026-09-21 - `Binacle.ViPaq.Benchmarks`

**One vipaq binary, not two.** Scale is 24 cases today, about 2 minutes at `short`; the shape file's "longer"
was a guess. The wall argument fails: size is written by `vipaq/measure/Binacle.ViPaq.EncodedSize`, which never
touches the synthetic provider, and BDN can only write ns and bytes to its own artifacts folder. Both bench
projects would reference `Binacle.ViPaq.Testing` anyway, so a split builds no wall. `vipaq/bench/` holds one
project; the recipe is `vipaq`.

Two facts from the code and `vipaq/results/encoded-size.md`:

- **Every timing class runs the raw path** (`NoOpCodec`), so the curated provider's "raw vs compressed" split
  does not exist in the timing tables; the packs differ only by item count and width. Compression time is
  measured in `CompressionCost` alone.
- **The provider's "uncompressed" picks are a leftover** of a threshold that is gone: deflate wins on three of
  the four (37-45% saved); only the 1-item pack is raw. Fix the comments when the class is touched.
- The api only encodes, row-major, uncompressed (`v4/Contracts/BinResponseBase.cs`); decoding happens in the
  browser. The production path is the rows `Protobuf` vs `ViPaq_Row`.

Real packs, scripted over the 2,322 rows: median 79 items, max 371 (`OrLibrary_thpack1_65`), 2,103 rows at
width 16/8/16. The encoder is a per-item loop, so cost is linear by construction; one synthetic curve past the
real data is enough, and it reads in the same table as the real packs.

**Three classes:** `Encode`, `Decode`, `CompressionCost`. The synthetic scenarios are columns in `Encode` and
`Decode`; both `Synthetic*` classes and `SyntheticBenchmarkBase` go. Rows `Protobuf` (baseline), `ViPaq_Row`,
`ViPaq_Columnar` - the size report's own words; today's `ViPaq_Col` / `ViPaq_Column` mismatch goes. Both
layouts stay: the size README headlines columnar as the user's number, and whether it costs time is the one
thing size cannot say. A `Json` row would be encode only (`JsonEncoder` has no decode).

| class | column | pack | why |
|---|---|---|---|
| Encode, Decode | `one item` | Baseline_5x5x5-1 | the fixed cost |
| | `small, all 16-bit` | Simple_16bit-4 | the only all-16-bit real pack (2 of 2,322 rows) |
| | `small, 8-bit` | Complex_FitsInMedium_1, 16 items | the 8-item pack says the same one step apart; dropped |
| | `100 cubes, 8-bit` | Simple_5x5x5-100 | the big 8-bit end |
| | `typical container` | OrLibrary_thpack4_1, 70 items | Bischoff median is 81; width 16/8/16 |
| | `largest real pack` | OrLibrary_thpack1_65, 365 items | new - the tail no pick reaches; joins `BischoffCuratedProvider`, an FFD pack like the others |
| | `1000 items, 8-bit` .. `65535 items, 16-bit` | synthetic 1,000 / 5,000 / 65,535 x 8 / 16 bit | the curve to the format's limit (`Limits.cs` throws above 65,535); 2,000 dropped |
| CompressionCost | `compression low win` | OrLibrary_thpack4_1 | 54% saved - the low end of deflate's win |
| | `compression high win` | OrLibrary_thpack1_2 | 69% saved |

Encode 12 x 3 = 36, Decode 36, CompressionCost 2 x 6 = 12: **84 cases**, about 7 minutes at `short`, about
20 at the default job. One tier: `vipaq` at the default job - the parity findings rest on ratios like 0.89 and
1.20 - with `short` as its one argument for a did-it-help run.

## Done when

- [x] `ls lib/bench` lists `Binacle.Lib.Benchmarks.Algorithms Binacle.Lib.Benchmarks.ResultSelection Binacle.Lib.Benchmarks.Racing Binacle.Lib.Benchmarks.Threshold`;
      `ls vipaq/bench` lists `Binacle.ViPaq.Benchmarks` alone.
- [x] `ls lib/test vipaq/test` lists no `Benchmarks`.
- [x] `grep -r "^namespace" lib/bench vipaq/bench --include=*.cs | awk -F: '{split($1,p,"/"); print p[3], $2}' | sort -u`
      shows one namespace per project.
- [x] `grep -l "ManualConfig" lib/bench/*/Program.cs vipaq/bench/*/Program.cs` is empty, and
      `grep -l BenchmarkDotNet */test/*/*.csproj` lists only `shared/test/Binacle.Benchmarking`.
- [x] `grep -rh "class Smoke_\|class Sample_\|class Full_" lib/bench --include=*.cs | wc -l` is 2 + 6 + 6 + 2 + 1 = 17,
      and `grep -rn "\[Params\|ParamsSource" lib/bench/Binacle.Lib.Benchmarks.Threshold --include=*.cs | grep -c ProcessorCount` is 0.
- [x] `grep -c "Benchmarks\"" lib/src/Binacle.Lib/Binacle.Lib.csproj` is 0 and `grep -c "Benchmarks" vipaq/src/Binacle.ViPaq/Binacle.ViPaq.csproj` is 1.
- [x] `test -f tooling/bench.just && test ! -f tooling/benchmarks.lib.sh && test ! -f tooling/benchmarks.vipaq.sh`
- [ ] `just bench` lists every binary and tier with its cost; `just bench lib-result-selection` runs to a report;
      `just bench lib-algorithms-full` asks before it starts.
      **By eye** for the last: the maintainer runs it and says no.
- [x] `grep -n "still shell scripts" justfile` is empty; `grep -n "benchmarks\." tooling/README.md` is empty.
- [x] The tooling shape is settled and built: no `tooling/bench.run.sh`, no
      `[BenchmarkCategory]`, and a wrong job or an empty filter fails the recipe.
      `test ! -f tooling/bench.run.sh && ! grep -rq BenchmarkCategory lib/bench vipaq/bench --include=*.cs`,
      and `just bench lib-threshold-sample nothing-matches-this` exits non-zero.
- [x] Every scenario name in the settled tables appears in a provider or a data file:
      `grep -rn "typical container\|BFD wins big\|largest real pack\|one full winner\|all partial, tie on volume" lib vipaq --include=*.cs --include=*.json | wc -l` is at least 5.
