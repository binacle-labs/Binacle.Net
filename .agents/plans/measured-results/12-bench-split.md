---
description: Step 12 - one benchmark project per question, five of them, each with its tiers and scenarios settled, the config in shared/test/Binacle.Benchmarking, bench.just, the two benchmark scripts gone
state: ready
waits-on: "step 11's gate"
horizon: next-release
paths: ["lib/**", "vipaq/**", "tooling/**", "Binacle.Net.slnx", "justfile"]
---

# Step 12 - the bench split

Shape: [results.md](results.md), "Benchmark projects" and "Recipes". Protocol: the orchestrator.

## The step

- `lib/test/Binacle.Lib.Benchmarks` becomes four projects under `lib/bench/`: `.Algorithms`, `.ResultSelection`,
  `.Racing`, `.Threshold`. `vipaq/test/Binacle.ViPaq.Benchmarks` moves to `vipaq/bench/Binacle.ViPaq.Benchmarks`,
  one project. The classes, tiers and scenarios of each are settled below; step 14's scaling class and JSON row
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

- **A tier is a recipe.** `lib-algorithms-smoke`, `lib-algorithms-sample`, `lib-algorithms-full`. The plain
  name (`lib-algorithms`) runs that binary's default tier, so it exists on every binary. A binary with one tier
  has only its plain name. `just` rejects a tier a binary does not have by itself.
- **Smoke and sample are their own classes**, so the tier is in the class name and so in the report file name.
  Full is the classes that exist today. Where a tier is only a cheaper job over the same classes (racing), no
  class is added.
- **Jobs.** No `Job` in code. Smoke and sample pass `--job short`; full runs BDN's default job and takes the
  job as its one argument for the cheap run (`just bench vipaq short` - not `job="short"`, which is only how
  `just --list` prints a default and cannot be typed). Full prints both estimates before it starts. Measured
  2026-09-21 on the BDN 0.15.8 source: the default job is 13-20 s per case whatever the method costs,
  `short` about 5 s, `short --iterationTime 100` about 2 s. `short` keeps Mean and Ratio; it widens Error
  and RatioSD, so a finding that rests on a ratio under 1.1 needs the default job.
- **Narrowing - open again since 2026-09-22.** As built, the tier and the words `ffd`, `bfd`, `wfd`,
  `packing`, `fitting` are `[BenchmarkCategory]` values passed as `--allCategories`, and a shell script
  (`tooling/bench.run.sh`) assembles the `dotnet run` line. The maintainer rejected both the script and the
  categories on review. [`findings.md`](findings.md) holds the objection, what is wrong with the built form
  (a `job=` recipe eats the first word as the job; every mistake exits 0), and a candidate shape - `--filter`
  on the class name, which already carries the tier, and the job from the environment. Nothing about it is
  settled until he picks.
- `--join` adds nothing with one project per family. JSON exporters come when a script needs them, not before.

## Settled 2026-09-21 - `Binacle.Lib.Benchmarks.Algorithms`

Every fill below is from `lib/results/packing-efficiency.md` on 2026-09-21; every timing claim from a file under
`results/lib/benchmarks/`. Re-run the table, do not trust this list, if either changes.

| tier | classes | cases | at `short` |
|---|---|---|---|
| smoke | `Smoke_Packing`, `Smoke_Fitting` - six rows (FFD, WFD, BFD x v1, v2), scenario column | 48 | ~4 min |
| sample | `Sample_<Alg>_<Op>` x 6 - rows v1, v2, 30 scenario columns | 360 | ~30 min (default) |
| full | `Full_<Alg>_<Op>` x 6 - rows v1, v2, 700 columns by thpack id | 8,400 | ~12 h; ~30 h default |

**Smoke - four scenarios.** The column shows the name; the id is in the provider.

| name | scenario | why |
|---|---|---|
| `full bin, one type` | Cube: 192 5x5x5 in 60x40x10, exactly 100% | the 192 column in every dated record since 2024-04. Where WFD v2 is a memory win, not a time win (v2/v1 1.03, allocation 0.18x) - keep `[MemoryDiagnoser]` |
| `small order` | Specialized: 3 types, 13 items | the small end. In the records the 10-item column moves in step with 192 for FFD and BFD; drop first if the budget is tight |
| `typical container` | thpack1_7 | realistic problem; its Fitting rows hit the does-not-fit exit, a ~5x cheaper path |
| `most item types` | thpack7_56, 20 types | item-type count is what separates the algorithms in time (WFD +131% on thpack1_7, +373% here). No other smoke scenario has more than 3 types |

**Sample - 30 scenarios, by category.** A stride sample (5 per group) gives 33 of 35 that all read "BFD wins by
8-22 points" and holds no WFD win, no FFD win, neither size end, and not thpack7_45. The column shows
`<category> (<id>)`.

| category | scenarios | why |
|---|---|---|
| `typical container` | 1_7 | the baseline every finding quotes |
| `BFD wins big` | 4_77, 2_51, 5_26, 6_39, 7_48, 1_44 | biggest BFD margins (24.8 down to 14.4); 4_77 is also the biggest spread, 31.7 |
| `WFD falls over` | 2_59, 3_98, 6_78, 2_35 | WFD's lowest fills (49.2 to 56.8) |
| `FFD falls over` | 5_47 | FFD's lowest fill, 56.2 |
| `WFD wins` | 6_93, 1_58, 3_43 | WFD is best alone on 14 of 700; these are its three biggest margins |
| `FFD wins` | 4_25, 3_35, 4_93 | FFD is best alone on 14 of 700; biggest margin, and 4_93 where FFD and BFD sit 0.03 apart |
| `all three tie` | 1_39, 3_17 | 17 three-way ties in the suite; 1_39 is the highest fill among them, 3_17 the only one past thpack2 |
| `near tie` | 2_30, 5_29 | margins 0.42 and 1.53 |
| `tightest fit` | 1_54, 7_4 | the only two with ceiling 100.00 |
| `loosest fit` | 4_23 | ceiling 97.40, the lowest |
| `most items` | 1_65 | 476 items |
| `fewest items` | 1_84 | 69 items, and a three-way tie at 62.08 |
| `BFD best fill` | 2_33 | 90.66, the highest fill in the suite |
| `most item types` | 7_56 | 20 types |
| `v1 and v2 differ` | 7_45 | the one scenario where BFD v2 packs differently from v1 (79.08 -> 79.73) |

**Renames that fall out.** The keys in `BischoffCuratedProblemsProvider.ScenarioDescriptions` become the names
above: `Baseline` -> `typical container`, `BFD dominance` -> `BFD wins big`, `High efficiency` -> `near tie`,
`WFD weakness` -> `WFD falls over`, `Max complexity` -> `most item types`. Racing reads the same keys.

**Seen in the records, not measured by any tier.** Cost steps at each bin's worth of items (192 -> 193 cubes:
266 -> 452 us). An unshipped WFD variant from 2024-11 ran 2.5-3x faster and was never adopted. Both are notes
for `design/lib/findings.md`, not cases.

## Settled 2026-09-21 - `Binacle.Lib.Benchmarks.ResultSelection`

One tier, one recipe, `--job short` - the methods are sub-microsecond, the default job's extra iterations buy
nothing. Three classes as today, rows v1 and v2, column = scenario. Class names `BestAlgorithm`, `BestBin`,
`SmallestBin` - the project name already says ResultSelection.

What the bench guards: v1 is LINQ (`Where().OrderBy().First()`, and a second pass when nothing is fully
packed); v2 is one loop. The memory column and Ratio catch a v2 that grows an iterator or a second pass back.

Candidate counts in real use, from the code: BestAlgorithm sees 3 on the single-bin path
(`AlgorithmProcessorFactory.cs`) and 2 per bin on the multi-bin path (`BinProcessorFactory.cs`) - fixed, so 3 is
the real number. BestBin and SmallestBin see the request's bin count; the api validator sets no maximum
(`v4/Contracts/IWithBins.cs`), the shipped presets carry 3 each. So those two get one bigger count.

| class | scenario (column name) | candidates | why |
|---|---|---|---|
| BestAlgorithm | `one full winner` | 3 | v1 stops at the first full result; v2 loops |
| BestAlgorithm | `all full, first wins` | 3 | the tie; v2's strict `>` keeps the first |
| BestAlgorithm | `all partial` | 3 | the only path where v1 sorts |
| BestBin | `one full winner` | 3 | v1 filters to 1 |
| BestBin | `all full, fullest wins` | 3 | v1 filters to 3, then sorts |
| BestBin | `all partial` | 3 | v1 runs both passes |
| BestBin | `20 bins, half full` | 20 | new - the uncapped user count |
| SmallestBin | `one full winner` | 3 | |
| SmallestBin | `two full, smallest wins` | 3 | what "Multiple Fully Packed" is today: 60x40x10 is partial there |
| SmallestBin | `all partial, tie on volume` | 3 | rewritten - today's bins are 60x40x10/20/30, so the volume tie-break never decides; two bins share a volume (60x40x10 and 40x30x20, both 24000) |
| SmallestBin | `20 bins, half full` | 20 | new |

11 scenarios x 2 rows = 22 cases, about 2 minutes at `short`.

**Names.** The JSON `Name` in `lib/data/result-selection/*/baseline.json` becomes the short name above and the
column shows it unchanged - one string for data, bench and unit tests. The `Best Bin - ` prefix exists only
because `All.cs` keys every set in one dictionary; the bench base and the unit-test fixture resolve through the
per-set `Scenarios.GetScenarioByName` instead, and `All` goes or keys by `<set>/<name>`. The compact result
format and `ExpectedResult` do not change. The unit tests read the same files and get the new names.

## Settled 2026-09-21 - `Binacle.Lib.Benchmarks.Racing`

The question: when `Best` races several algorithms on one bin, is parallel faster than one after the other
(`design/lib/findings.md` F2, decision O1). Rows `Loop` (baseline) and `Parallel`. Classes `Packing_v1` and
`Packing_v2` (one per algorithm factory; v1 carries the deleted-with-v1 comment). No Fitting class in this step.

- **Two algorithm sets, not four.** D1 says production races exactly `FFD,BFD` (multi-bin routes) and
  `FFD,WFD,BFD` (single-bin routes). `BFD,WFD` and `FFD,WFD` never run in production and cannot change the
  answer; their side note is already in F2.
- **The five curated scenarios stay**, under their new names. F2 spreads them from 0.93x to 1.48x.
- **`ProcessorCount` goes.** The parallel processor runs `Parallel.For` over the algorithms, so a race of N
  algorithms uses at most N threads and the set already decides it. BDN's header prints the machine's cores.

5 scenarios x 2 sets x 2 rows x 2 classes = 40 cases. Two tiers over the same classes: `lib-racing-smoke` at
`short`, about 2 minutes; `lib-racing` = `lib-racing-full` at the default job, about 10 minutes - the
findings rest on ratios like 1.08, which `short` would blur.

## Settled 2026-09-21 - `Binacle.Lib.Benchmarks.Threshold`

Two families, both `Loop` (baseline) vs `Parallel`, both on the synthetic ladder in
`SpecializedScalingProblemsProvider`. Production uses `Loop` everywhere; these are the evidence for whether the
parallel processors should be wired up, and from what size (`design/lib/findings.md` F2, decision O1).

What the old `MultipleBins` records show (`results/lib/benchmarks/results_net*/`, ratios recomputed per
algorithm by script): Parallel is under 1.0 from 2 bins up on every machine - about 0.85 at 2 bins, 0.65 at 8,
flat after. They measured 2, 8, 14 .. 38 bins on one fixed item set; never 1 bin, never 3-7, never a change in
per-bin weight. The ladder covers exactly that gap. Per-bin weight moves the low end (2 bins: FFD 0.82, WFD
0.60), so the bins family needs every bin count and only three item levels.

- **Algorithm sets: `FFD,BFD` and `FFD,WFD,BFD` only** - the two production races (D1).
- **Bins family: FFD and BFD only** - the multi-bin routes never run WFD (D1).
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
thing size cannot say. Step 14's `Json` row is encode only (`JsonEncoder` has no decode).

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
- [x] `grep -r "^namespace" lib/bench vipaq/bench --include=*.cs | awk -F: '{split($1,p,"/"); print p[2], $2}' | sort -u`
      shows one namespace per project.
- [x] `grep -l "ManualConfig" lib/bench/*/Program.cs vipaq/bench/*/Program.cs` is empty, and
      `grep -l BenchmarkDotNet */test/*/*.csproj` lists only `shared/test/Binacle.Benchmarking`.
- [x] `grep -rh "class Smoke_\|class Sample_\|class Full_" lib/bench --include=*.cs | wc -l` is 2 + 6 + 6 + 2 + 1 = 17,
      and `grep -rn "\[Params\|ParamsSource" lib/bench/Binacle.Lib.Benchmarks.Threshold --include=*.cs | grep -c ProcessorCount` is 0.
- [x] `grep -c "Benchmarks\"" lib/src/Binacle.Lib/Binacle.Lib.csproj` is 0 and `grep -c "Benchmarks" vipaq/src/Binacle.ViPaq/Binacle.ViPaq.csproj` is 1.
- [x] `test -f tooling/bench.just && test ! -f tooling/benchmarks.lib.sh && test ! -f tooling/benchmarks.vipaq.sh`
- [ ] `just bench` lists every binary and tier with its cost; `just bench lib-result-selection` runs to a report;
      `just bench lib-algorithms-full` prints the case count and both estimates before it starts.
      **By eye** for the last: the maintainer runs it and stops it.
- [x] `grep -n "still shell scripts" justfile` is empty; `grep -n "benchmarks\." tooling/README.md` is empty.
- [ ] The tooling shape in `findings.md` section 1 is settled and built: no `tooling/bench.run.sh`, no
      `[BenchmarkCategory]`, and a wrong job or an empty filter fails the recipe.
      `test ! -f tooling/bench.run.sh && ! grep -rq BenchmarkCategory lib/bench vipaq/bench --include=*.cs`,
      and `just bench lib-result-selection nothing-matches-this` exits non-zero.
- [x] Every scenario name in the settled tables appears in a provider or a data file:
      `grep -rn "typical container\|BFD wins big\|largest real pack\|one full winner\|all partial, tie on volume" lib vipaq --include=*.cs --include=*.json | wc -l` is at least 5.
