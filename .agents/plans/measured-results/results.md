---
description: Measured results get a home in each slice, a harness that writes the verdict, benchmark projects split by question, two just recipes, and the old vault converted in
state: ready
waits-on: "nothing - the reviewer pass landed 2026-09-19; the support projects (steps 1 to 6 of the orchestrator) go first"
horizon: next-release
paths:
  - "tooling/**"
  - "results/**"
  - "lib/**"
  - "vipaq/**"
  - "shared/test/**"
---

# Where measured results live, and what produces them

The repo has two kinds of measured number and treats them as one. **Deterministic** numbers - how full a
bin gets, how many bytes a token takes - are the same on any machine, so a change in them is a change in
the code. **Timing** numbers from BenchmarkDotNet move with the machine and the runtime; only the ratio
between rows in one run is stable. The old `results/` vault mixed both under one rule, was copied by hand,
and went stale: every raw report in it is for classes that no longer exist, and the one folder that looked
current predates the data it claims to cover.

The decisions below were taken 2026-09-18 and 2026-09-19. The evidence is under Research.

**They are the shape, not the letter.** Each piece of work will find details this plan does not settle -
internal names, column order, how a tier is picked - and those belong to whoever does that work. And if a
decision here turns out to have a problem when it meets the code, the session doing the work says so and
challenges it, with the evidence, rather than building around it. A decision that was wrong is cheaper to
change than to obey.

## The shape

### Two kinds, two rules

| | Deterministic | Timing |
|---|---|---|
| Produced by | `<slice>/measure/` project | `<slice>/bench/` projects |
| Recipe | `just measure <slice>` | `just bench <slice>-<project>` |
| Written to | `<slice>/results/` by the harness, tracked, overwritten every run | scratch; a keeper is copied by hand |
| Comparison | `git diff` | Ratio and Allocated columns across keepers; never Mean. Ratio crosses machines only for same-thread rows; Loop vs Parallel carries the core count in its ruler |
| Runs when | something changed - not a test, not a gate | on demand; the slow ones only when named |

### Where

Storage is in the slice, beside `data/`, `src/` and `test/`. Root `results/` is deleted. Storing and showing
are different questions: GitHub renders the folder today; a docs-site page is a later copy of the README,
the same shape as the OpenAPI sync; www carries at most one number. No new site. Whatever is shown later,
only deterministic numbers are published - timing from one desktop is not a claim.

```
lib/
  data/  src/  test/
  measure/  Binacle.Lib.PackingEfficiency
  bench/    Binacle.Lib.Benchmarks.Algorithms
            Binacle.Lib.Benchmarks.ResultSelection
            Binacle.Lib.Benchmarks.Racing
            Binacle.Lib.Benchmarks.Threshold
  results/
    README.md                 harness-written: the summaries, nothing else
    packing-efficiency.md     700 rows, shipped versions, ceiling column, best and margin
    version-parity.md         only rows where v1 and v2 differ
    benchmarks/
      README.md               rule, trace table, what they say - see below
      algorithms/2026-09-20.md
      result-selection/
      racing/2026-07-17.md
      threshold/

vipaq/
  data/  src/  test/  test-vectors/  tools/
  measure/  Binacle.ViPaq.EncodedSize
  bench/    Binacle.ViPaq.Benchmarks.Encoding
            Binacle.ViPaq.Benchmarks.Scale
  results/
    README.md
    encoded-size.md           one row per pack per layout, six sizes, base64 only
    benchmarks/
      README.md
      encoding/
      scale/
```

Neither project set lives under `test/`: they assert nothing, and `test/` is what `just test` runs. What a
slice's measure and bench projects share - factories, curated picks, the BDN config, the encoders - is its
`Testing` project; what both slices share is `Binacle.Reporting`. No `shared/measure/` or `shared/bench/`.

### Deterministic projects - one run, many views

Each project packs or encodes every scenario once, holds the results, and a builder writes the files from
that set. Today each report re-runs the work (lib: 12,600 packings for 4,200 numbers) and computes its own
truth. The implementer names the runner and builder.

**Raw files stay raw. The README is the presentation** - short summaries backed by the detail, every number
computed by the harness. Nothing hand-written lands in it: a hand-written summary goes stale silently and is
the one thing people quote. The why lives in `.agents/design/<slice>/findings.md`. Header on every file is
fixed text - tool, scenario count, data set - never a date or commit, which git already has.

**`Binacle.Lib.PackingEfficiency`** (its original 2024 name minus `Tests`):

- `README.md` - per shipped version min / mean / median / max / stddev over 700; the same per set, rows
  labelled `BR1 (3 types)` .. `BR7 (20 types)` - that is what thpack1..7 are, and how the papers name them;
  **the user's fill** - mean of best-of-FFD-and-BFD (what the API races) and best-of-all-three, from the
  same packings, which prices the racing decision in fill; best-or-tied count per algorithm; v1/v2 agreement
  count; links.
- `packing-efficiency.md` - Scenario, Types, Items, Ceiling %, FFD, WFD, BFD, Best, Margin. The last two replace
  BaselineComparison, which hard-coded BFD as the answer; settled 2026-09-20 as columns, not a file.
- `version-parity.md` - replaces RegressionTests, which listed any difference, not only worse. v1 code stays
  for the speed benchmarks; in the fill files it appears here only. It reports where v2 packs differently -
  the old vault shows at least one such scenario (thpack7_45, BFD 79.08 vs 79.73) - so it is a count, not a
  proof of sameness. Deleted with v1.
- Not added: timing, fitting (yes/no), custom problems, the adjusted (fill / ceiling) table - ceilings are
  97-100%, it moves numbers by a point.

**`Binacle.ViPaq.EncodedSize`**:

- `README.md` - codec x layout table of mean / min / max ViPaq-to-protobuf ratio; the same per algorithm's
  packs; crossover item count per layout; codec win-count; the JSON row; **the largest token** - one line,
  "every real pack deflates to under N base64 characters" (1,248 over 1,400 packs in the old data), the
  fits-a-URL claim; links.
- `encoded-size.md` - Scenario, Algorithm, Items, Widths, ViPaq raw / deflate / gzip, protobuf raw /
  deflate / gzip, ratio, best codec, saved %. Base64 lengths only - the stored form; bytes are 3/4 of it.
  Two sections, one per layout. Replaces sixteen tables in five files.
- **A JSON baseline is added**, a small encoder beside `ProtobufEncoder` in `Binacle.ViPaq.Testing`.
  Protobuf is the fair format comparison; JSON is what a user's token replaces, and "N% of the JSON body"
  is the user's number.
- Two of the three pre-report gates move to `Binacle.ViPaq.UnitTests`: they round-trip every real pack in
  every codec and layout, one at a forced 16-bit width, and are the only round trip of every real pack;
  today they run only when someone runs the report. The third checks that every curated pick still names a
  real scenario; the picks live in `Binacle.ViPaq.Testing`, which the unit tests never reference, so it
  stays in the measure project's startup.

### Benchmark projects - one project per question

| Project | Holds | Cost |
|---|---|---|
| `Binacle.Lib.Benchmarks.Algorithms` | FastValidation; **a scaling class** - six versions over the 11-step item ladder already in `SpecializedScalingProblemsProvider`, the time-against-item-count curve no family gives today; BischoffSuite in tiers - quick validation, perhaps a curated sample, and the full 700 which runs only when named | minutes to a day |
| `Binacle.Lib.Benchmarks.ResultSelection` | the three strategy benchmarks - different code, different data, so its own project though only 18 cases | seconds |
| `Binacle.Lib.Benchmarks.Racing` | AlgorithmRacing v1, v2 | ~30 min |
| `Binacle.Lib.Benchmarks.Threshold` | both parallelization families. AlgorithmParallelizationThreshold stays as the evidence for why parallel racing was not wired up | hours |
| `Binacle.ViPaq.Benchmarks.Encoding` | curated encode, curated decode, CompressionCost | minutes |
| `Binacle.ViPaq.Benchmarks.Scale` | synthetic encode and decode at 2,000 and 5,000 items | longer |

The project is the category; no BenchmarkDotNet categories needed. The 20-line BDN config lives in each
slice's `Testing` project - two copies, which is the "copy the few lines" lesson below, not a project for 20
lines. One class-name rule, `<Family>_<Operation>_<Variant>`, one namespace per project; fix
`Benchmarks/Fitting/FastValidation/FastValidation_SpecializedBaseline_Packing.cs`, which holds the `_Fitting` class;
delete the unused `Generator.cs`. JSON joins the protobuf baseline in the vipaq timing once the encoder
exists. Every `_v1` baseline is marked in one comment as deleted with v1.

**Keepers are dated, one folder per family.** The ruler is in every BDN header, so the file does not carry
it. Keep a run when the ruler or the code changed, not because it ran.

**`<slice>/results/benchmarks/README.md`** has three parts: the rule (Mean does not compare across files;
Ratio and Allocated do, and for Loop vs Parallel only on the same core count); the trace - one row per
keeper: date, ruler, family, key ratio with its RatioSD, allocated; what they say - one line per family,
the current answer with its number. Parts two and three are read out of the
keeper files, by a small script if the implementer writes one, by hand on day one.

### Recipes

Two modules at the tooling root, following `tests.just`: `set working-directory := '..'`,
`set no-exit-message := true`, a `default` that lists, one recipe per project. With one project per family
the family is the recipe name, so `just` rejects an unknown one by itself - no alias table, no `case`. The
four `tooling/*.sh` scripts are absorbed - each is `dotnet run -c Release` with a path, the shape the earlier
conversions absorbed rather than wrapped. The project list is the recipe list; nothing else holds it. The
BDN config is C# in the two `Testing` projects, which is BDN's own config, so no file under `tooling/` is needed.

```
just measure                      the list
just measure lib                  Binacle.Lib.PackingEfficiency -> lib/results/
just measure vipaq                Binacle.ViPaq.EncodedSize     -> vipaq/results/
just measure all
just measure check                all, then fail if git status under either results/ is dirty - the golden-file check,
                                  the same ten lines as `just regen check`

just bench                        the list, each project with its cost
just bench lib-algorithms         default tier; `just bench lib-algorithms -- --filter '*Bischoff*'` passes BDN flags
just bench lib-racing | lib-threshold | lib-result-selection
just bench lib-fast               lib-algorithms then lib-result-selection - the quick run
just bench lib-all                says "hours" first, then every lib project
just bench vipaq-encoding | vipaq-scale | vipaq-all
```

How the full 700-scenario tier is named inside `lib-algorithms` is the implementer's - a BDN filter, a
second recipe, or an environment variable; whichever it is, the full run never happens without being asked.

The root `justfile` gets two `mod` lines and loses the comment "benchmark and performance runs are still
shell scripts". `tooling/README.md` rows for `performance.<slice>.sh` and `benchmarks.<slice>.sh` become the
two modules. `just check scripts` is a glob and loses the four files by itself. `regen.just` opens with
"Regenerates the data that is committed to the repository" and "Every tool here rewrites committed files";
once `measure` exists it is no longer the only module that does, and its header says so.

### What moves with it

Files that name the old layout by path or by rule. Each one is wrong the moment its step lands.

- **`Directory.Build.props`** marks a project as Sonar support code when its path holds `/test/` or
  `/tools/`. `measure/` and `bench/` join that list, or Sonar runs the product rule set over the benchmarks
  again - S101 on `BestBin_ResultSelection` is why the rule exists.
- **`tooling/ci/sonar-analysis.xml`** excludes `results/**`; it becomes `lib/results/**,vipaq/results/**`.
- **`Binacle.Net.slnx`** lists the four projects under `/lib/test/` and `/vipaq/test/`. New folders for
  `measure/` and `bench/` in each slice. CI builds the whole solution twice per run, so every new project is
  built on every Sonar and image run.
- **`.gitignore`** loses `PerformanceTests.Artifacts` and `PerformanceTestsArtifacts` once nothing writes
  there. `BenchmarkDotNet.Artifacts` stays.
- **`AlgorithmFactories.cs`** was one file copied into three lib projects; the support-projects shape gives
  it one home, `Binacle.Lib.Testing`, before the bench split would make it five.
- **`.agents/memory/results-curated.md`** says the opposite of this plan - "never point a harness's file
  writer at `results/`". It is deleted the day the writers move (step 7), or the next agent obeys it.
- **`.agents/design/vipaq/decisions.md` D3** says the perf test writes to scratch and the vault is copied by
  hand. That paragraph and the scratch sentence in `docs/commands.md` are rewritten in step 7 with the
  memory. When the vault goes (step 11): `design/vipaq/findings.md` (its `check:` line and the crossover
  reference), vipaq D5 and `docs/vipaq/architecture.md` (the compression report paths),
  `design/lib/findings.md` (the vault path), `docs/README.md` (the `results/` row), `docs/build-topology.md`
  ("`results/` is deliberately not in the solution"), `.agents/README.md` ("`results/` by real path"),
  `docs/commands.md` (the four scripts). One line stays: the repo-wide design record names "the 2024 records
  under `results/lib/benchmarks/`" as a record of what was true then; it is history, not a pointer.
- **`shared/data/README.md`** and the slice READMEs name `results/` at the root nowhere, but
  `lib/README.md` and `vipaq/README.md` list `test/` projects and gain `measure/`, `bench/` and `results/`.

### Converting the old vault

Done last, once the new folders exist. Every old raw report is for a deleted class (`AlgorithmVersion_*`,
`MultipleItems_*`, `MultipleBins_*`); each lands under the nearest current family - the 20 scaling reports
under `algorithms/` beside the new scaling class - dated from its BDN header and git, with a line saying
which class it really measured. The `AlgorithmVersion_*` files carry `FFD_v3` rows for a version that no
longer exists; the line says that too. One judgement per file. The hand-written dated
summaries carry charts on GitHub user-attachments and a "what changed" line; the line is worth keeping in
the trace table, the rest is not. Then the trace table is read as a whole and the progress story checked.

### Not in this plan

- A docs-site page copied from the READMEs, and a www number.
- Comparison against published results on the Bischoff instances: "we do 81% at X ns, others 87% at Y ns".
  The fill half holds anywhere; the time half only if both ran on one machine.

## Work order

Steps 7 to 14 of the orchestrator beside this folder, one file each. The support projects go first, because
the bench split multiplies every copy they remove.

## Research

### 2026-09-19 - reviewer pass against the shape

Four errors, fixed above: ResultSelection is 18 cases; ratio does not cross machines for Loop vs Parallel
(`ProcessorCount` is a `[ParamsSource]` from `Environment.ProcessorCount`, 2 on one machine and 12 on the
other); v2 does not pack identically to v1 (thpack7_45); 20 of the 50 old raw reports are "time against item
count" (`MultipleItems_*`, `MultipleBins_*`) and no current family measures that.

**Added to the shape 2026-09-19, all four:** **time against item count** - one class, six versions over the existing
11-step ladder in `SpecializedScalingProblemsProvider`, minutes, and the family the 20 old files land under;
**the user's fill** - `max(FFD,BFD)` and `max(all three)` per scenario from the same packings, which prices
D1 in fill; **"every real pack fits a URL"** - largest deflated token over 1,400 packs is 1,248 base64 chars,
one README line; **RatioSD beside every ratio** in the trace table.

**Not decided - the maintainer is unsure, so the shape above stands until a session argues otherwise:**
`encoded-size.md` at 4,300 rows and ~500 KB (one algorithm's packs raw, per-algorithm means in the
README); README part 3 as a separate section (the newest trace row per family, marked, does the job).

Conventions, noted and not acted on: no well-known project keeps dated per-family benchmark folders in the source tree - the .NET
norm is one table in the README refreshed at release, and `benchmark-action/github-action-benchmark` on a
gh-pages branch is the known tool for charts over time; keepers were chosen knowing that. `just measure
check` - run, then fail if `git status` is dirty - is the ten lines `regen check` already has, and it is what
makes a golden file one. BDN mechanics to name in the implementation: `--join`, `--exporters json` for
anything a script reads, no `Job` is set in either project today. Label per-set rows `BR1 (3 types)` so a
reader of the papers recognises them.

Recipe shape, simpler than first drawn: with one project per family the family is the recipe name and
`just` rejects an unknown one itself - `just bench lib-algorithms`, `lib-fast`, `lib-all`, `vipaq-encoding`;
`just measure lib`, `vipaq`, `all`, `check`. `*args` passes BDN flags after `--`. `regen.just`'s header
says every tool there rewrites committed files; once `measure` does too, the header says it is not alone.

### 2026-09-19 - what survives a ruler change

Same code, `AlgorithmVersion_Fitting_FFD`, `FFD_v1` at 10 items, three committed rulers:

| Run | Machine | Runtime | Mean | v2 / v1 | Allocated |
|---|---|---|---|---|---|
| results_net9 | i5-4570 Linux | .NET 9 | 18.7 us | 0.64 | 5.6 KB |
| results_net10 | i5-4570 Linux | .NET 10 | 14.2 us | 0.69 | 5.56 KB |
| results_net9_windows | i7-14700 Windows | .NET 9 | 1.18 us | 0.66 | 5.6 KB |

No algorithm code changed between them. Read as a timeline, Mean says "24% faster" (the runtime) then "12x
faster" (the machine). Ratio and Allocated hold across all three. This is why keepers are dated but the
README says Mean does not compare, and why a two-ruler comparison is a design finding, not a folder.

### 2026-09-19 - the run cost of each family, from the `[Params]`

| Family | Cases | Cost |
|---|---|---|
| FastValidation (6 classes) | 36 | minutes |
| ResultSelection (3) | 18 | seconds |
| AlgorithmRacing (2) | 80 | ~30 min |
| AlgorithmParallelizationThreshold (2) | 176 | ~1 h |
| BinParallelizationThreshold (2) | 924 | hours |
| BischoffSuite (6) | 8,400 | a day |

`benchmarks.lib.sh` with no argument ran all six. ViPaq's five classes total under an hour.

### 2026-09-19 - the vault, measured

- Every `results_net*` report names `AlgorithmVersion_*`, `MultipleItems_*` or `MultipleBins_*`. No such
  class exists (`grep -rl AlgorithmVersion lib/test` is empty). By their .NET versions the runs are from
  November 2025; the classes were renamed in the May 2026 restructure; the folders were committed July 2026.
- `results/lib/efficiency/` holds `PackingEfficiencyComparison.md` and `PackingTime.md`, which the harness
  no longer writes, and lacks three of the four files it does write.
- `results/vipaq/compression/` has 700 Bischoff rows per layout named `OrLibrary_thpack1_1`. The packed data
  since 2026-08 has 2,100 per set, named `...thpack1_1.ffd`.
- The dated summaries were assembled by hand - contents list, charts drawn on chartbenchmark.net, tables
  pasted in. An hour each; none after 2025-02.
- Aggregates nobody wrote down: mean ViPaq/protobuf base64 over Bischoff is 0.65 raw, 0.65 deflate, 0.66
  gzip; deflate 0.38-0.75; deflate smallest on 714 of 721 packs, raw on the 7 tiniest custom ones.
- thpack1..7 are 100 scenarios each with 3, 5, 8, 10, 12, 15, 20 item types; every ceiling is 97.4-100%.

### 2026-09-19 - what each harness does, read not run

`Binacle.Lib.PerformanceTests`: four `ITest` classes each walk 700 scenarios and pack again - 12,600
packings for four views of 4,200 numbers. No summary table; `RegressionTests` lists any v1/v2 difference;
`BaselineComparison` hard-codes BFD and gives no count; per-set rows say `thpack3`, not "8 item types"; no
header; a Serilog bootstrap logger with two packages nothing uses (`Enrichers.Process`, `Sinks.File`);
`AlgorithmFactories` copied in three projects. Born
`Binacle.Net.Lib.PackingEfficiencyTests` 2024-10, renamed 2024-11 when timing was added; timing is gone.

`Binacle.Lib.Benchmarks`: four class-name patterns; five namespaces including
`Binacle.Lib.Benchmarks.Benchmarks.ResultSelection`, which BDN puts in the report file name; one file holds
the wrong class; `Generator.cs` unused; every baseline is `_v1`; the `Parallelization` alias matched two
families.

`Binacle.ViPaq.PerformanceTests`: three gates round-trip every real pack in every codec and layout plus a
forced 16-bit pass - the only place that happens, and CI never runs it. Then 12 size tables in 3 files and 4
crossover tables in 2 files; the crossover answer is logged to the console and written nowhere; bytes and
base64 on every row.

`Binacle.ViPaq.Benchmarks`: clean. Aliases `Encode`/`Decode` each match two classes; CompressionCost needs
the raw filter; `results/vipaq/benchmarks/` is empty.

### 2026-09-18 - rejected shapes, and why

- **The harness as a test that fails on a changed number.** Rejected: measurements run when something
  changed, on demand; they do not gate.
- **Scratch plus `diff` and `promote` recipes.** Rejected: keeps the copy step that already went stale once.
- **Root `results/` mirroring the slices.** Rejected: a mirror tree three folders from its writer, held
  together by a README sentence - that is how it went stale.
- **Dated copies of the deterministic files.** Rejected: git is the history; every copy can go stale.
- **A new results site.** Rejected: a fourth Jekyll site for a few tables, and no data for charts yet.
- **BDN categories in one assembly.** Rejected for projects per question: the tree says what exists, no
  attribute to forget, two problems cannot share a name.

### 2026-08-07 - lessons from the script-to-recipe conversions that already landed

- **Absorbed, not wrapped, for a script that wraps a tool.** These four are `dotnet run -c Release` with a
  path. A program keeps its own file with a two-line recipe as its door, because shellcheck cannot read a
  `.just` body.
- **An alias list becomes a parameter whose `case` rejects an unknown value.**
- **One module per job.** Where two need the same few lines, copy them.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself needs an
  absolute path.
- **When the moved script is a generator, prove the move by diffing its output.**

### 2026-07-17 - algorithm racing was re-measured

The numbers are in `.agents/design/lib/findings.md` (F1, F2). The reports behind them were never curated
in; `racing/2026-07-17.md` is the keeper to recover if the scratch folder still exists on that machine.

## Done when

- [ ] `results/` at the root is gone and each slice has its own.
      `test ! -d results && test -d lib/results && test -d vipaq/results`
- [ ] The deterministic harnesses write into the slice, and the README they write holds the summaries.
      `just measure lib && git status --short lib/results` shows only files the run changed;
      `head -30 lib/results/README.md` is a summary table, not 700 rows.
- [ ] Nothing under `<slice>/test/` produces a report.
      `ls lib/test vipaq/test` lists no `PerformanceTests` and no `Benchmarks`.
- [ ] One benchmark project per question, in `bench/`.
      `ls lib/bench vipaq/bench` lists the six projects named above.
- [ ] The round-trip of every real pack runs under `just test`.
      `grep -l "Binacle.ViPaq.Data" vipaq/test/Binacle.ViPaq.UnitTests/*.csproj` is not empty.
- [ ] `encoded-size.md` carries a JSON column.
      `head -12 vipaq/results/encoded-size.md | grep -i json`
- [ ] The four scripts are gone and two modules exist.
      `ls tooling/benchmarks.*.sh tooling/performance.*.sh` lists nothing; `test -f tooling/measure.just && test -f tooling/bench.just`
- [ ] No doc, README or memory names a path into `tooling/` for these runs, or the root vault.
      `grep -rn "tooling/performance\.\|tooling/benchmarks\.\|results-curated" --include=*.md . | grep -v "^./sites"`
      returns nothing, and so does
      `grep -rn "results/" --include=*.md .agents | grep -v "lib/results\|vipaq/results\|/plans/\|_index.md\|what was true then"`.
- [ ] Sonar still sees the moved projects as support code.
      `grep -n "Contains('/measure/')" Directory.Build.props` and `Contains('/bench/')` both hit;
      `grep -n "results" tooling/ci/sonar-analysis.xml` shows the two slice paths, not `results/**`.
- [ ] One `AlgorithmFactories.cs` in the lib slice.
      `find lib -name AlgorithmFactories.cs -not -path "*/obj/*"` lists one file.
- [ ] Every old keeper sits under a family folder with a date and a line naming its real class.
      **By eye.** Open each file under `lib/results/benchmarks/*/`; the first lines say class and ruler.
- [ ] Both `benchmarks/README.md` have the rule, the trace and what they say.
      **By eye.** Three headings, and every number in the trace appears in a keeper file.
- [ ] The scaling curve exists and has a keeper.
      `ls lib/results/benchmarks/algorithms/` has a file whose table has an item-count parameter column.
- [ ] The bin-threshold question has a finding.
      `grep -n "BinParallelizationThreshold" .agents/design/lib/findings.md` is a section with numbers, not "no finding yet".
- [ ] The doc for lib tests, the tooling doc, D3 and the lib and vipaq READMEs describe the new layout.
      `just agents all` regenerates clean; **by eye** the files name `measure/`, `bench/`, `results/`.
