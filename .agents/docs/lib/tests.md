---
id: lib/tests
description: lib/test projects — Binacle.Lib.Testing (the one AlgorithmFactories, the scenario checks, the benchmark providers), unit tests, the bench projects in lib/bench with their tiers, and the measure project in lib/measure; CommonTestingFixture, ResultSelectionTestingFixture, and run aliases
verified: 2026-10-03
check: Project list, AlgorithmFactories/CommonTestingFixture/ResultSelectionTestingFixture and what AssertResult calls, and the aliases, match lib/test/, lib/measure/, lib/bench/ and tooling/tests.just + tooling/measure.just + tooling/bench.just
also_update:
  - shared
  - lib/algorithm-factory
  - lib/result-selection
paths:
  - "lib/test/**"
  - "lib/measure/**"
  - "lib/bench/**"
  - "shared/test/Binacle.Benchmarking/**"

---

# Lib Tests

Two projects under `lib/test/`, one of them a support library rather than a suite, plus the measure project
in `lib/measure/` and the bench projects in `lib/bench/`. Algorithm scenario data
comes from the shared `Binacle.Data` project — see shared (`$shared`); the harness code every lib suite
shares comes from `Binacle.Lib.Testing`, below. The **result-selection** fixtures come from this
slice's own `lib/data/Binacle.Lib.Data`, which embeds `lib/data/result-selection` under the manifest prefix
`ResultSelection.` — it is here rather than in `shared` because nothing outside this slice reads it
(`$lib/dependencies`).

| Project | Kind | Run |
|---|---|---|
| `Binacle.Lib.Testing` | support library (no suite) | — |
| `Binacle.Lib.UnitTests` | xUnit | `just test cs_binacle-lib_unit` |
| `Binacle.Lib.PackingEfficiency` (`lib/measure/`) | console host (writes markdown reports) | `just measure lib` |
| `Binacle.Lib.Benchmarks.Algorithms` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-algorithms-smoke`, `-sample`, `-full` |
| `Binacle.Lib.Benchmarks.ParallelAlgorithms` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-parallel-algorithms-identical` |
| `Binacle.Lib.Benchmarks.ParallelBins` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-parallel-bins-identical FFD` (or `WFD`, `BFD`) |
| `Binacle.Lib.Benchmarks.ParallelOverhead` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-parallel-overhead` |
| `Binacle.Lib.Benchmarks.ResultSelection` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-result-selection` |
| `Binacle.Lib.Benchmarks.Scaling` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-scaling` |

## Binacle.Lib.Testing

The harness code the unit tests, the measure project and the bench projects share, imported globally
(`<Using Include="Binacle.Lib.Testing" />` in each csproj except `Binacle.Lib.Benchmarks.ResultSelection`, which
does not reference it). `Binacle.Lib` grants it friend access, because
it constructs the internal algorithm classes.

- `AlgorithmFactories.cs` defines one `TestAlgorithmFactory<IPackingAlgorithm>` static per version — `FFD_v1/_v2`,
  `WFD_v1/_v2/_v3`, `BFD_v1/_v2/_v3` — each constructing the algorithm directly
  (`new FirstFitDecreasing_v2<ScenarioBin, ScenarioItem>(bin, items)`), **not** through `IAlgorithmFactory`/DI.
  This keeps every version (including v1) under test without coupling it to the production factory.
- `TestAlgorithmFactory<TAlgorithm>` — `delegate TAlgorithm (ScenarioBin bin, List<ScenarioItem> items)` — and
  `TestOperationParameters`, the `IOperationParameters` a test hands to `Execute`.
- `ScenarioChecks` — the two `EvaluateResult` extensions, on `ScenarioMetrics` and on `AlgorithmResult`
  (**not** on `ScenarioResult`, which is the map). The `AlgorithmResult` one picks the packing or the fitting
  expected status by `result.AlgorithmOperation`, then throws on mismatch. `OperationResultExtensions` holds
  the volume and count totals they compare against.
- The benchmark picks, at the project root, each answering `Names` and `GetByName(name)`: `SmokeSet` (the
  smoke scenarios by name: `full bin, one type`, `small order`, `typical container`, `many item types`),
  `SampleSet` (picked Bischoff problems, name `<category> (<id>)`).
- The generators beside them, which build rather than pick: `CubeGenerator` (one cube baseline, `GetBaseline`)
  and `LadderGenerator` (the item ladder the scaling project climbs, and the smoke set's small order).
- `IdenticalCase` — the bin `160x120x80` and item `20x15x10` of the Identical benchmarks, and `Items(pieces, lines)`.
  The parallel-algorithms and parallel-bins projects both read it, so the two cannot drift.

## Binacle.Lib.UnitTests

Keeps its own `AssertionMethodAttribute`, the Sonar S2699 marker; ViPaq's unit tests carry a copy too.

Both fixtures split arrange, act and assert into separate members, so a test body shows all three steps
rather than handing them to one helper.

`CommonTestingFixture` holds every factory in `AlgorithmsUnderTest[]` and exposes:

```csharp
Scenario GetScenarioByName(string scenarioName)
OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> factory, Scenario scenario, AlgorithmOperation operation)
void AssertResult(Scenario scenario, OperationResult result)
```

`GetScenarioByName` resolves from `Binacle.Data.All.GetByName`. `Run` builds the algorithm
and calls `Execute(parameters)`, checking nothing. `AssertResult` does both checks — `Metrics` pins how the
algorithm got there, `Result` pins where it landed — and is marked `[AssertionMethod]` so the analyser knows
where the assertion lives:

```csharp
scenario.Metrics.EvaluateResult(result);
scenario.Result.For(result.AlgorithmInfo.Algorithm).EvaluateResult(result);
```

**`scenario.Result` is a map keyed by algorithm**, so the assert picks the entry for whichever algorithm ran
(`$shared`). A test reads:

```csharp
var testScenario = this.Fixture.GetScenarioByName(scenario);

var result = this.Fixture.Run(AlgorithmFactories.FFD_v1, testScenario, AlgorithmOperation.Fitting);

this.Fixture.AssertResult(testScenario, result);
```

Test classes: `FittingBischoffSuiteTests`, `FittingCustomProblemsTests`, `PackingBischoffSuiteTests`,
`PackingCustomProblemsTests`, `PackingDemoSamplesTests` (each a `[Theory]` × `[MemberData]` over every version), plus `CreationTests`,
`SanityTests`, `ResultSelectionTests`, `BinProcessingCancellationTests`.

`ResultSelectionTestingFixture`:

```csharp
string Select(Scenario scenario, IResultSelectionStrategy strategy, Func<OperationResult, string> resultSelector)
```

Each test resolves its scenario itself, through its own set — `BestAlgorithm.GetByName(name)` with
the set's `DataProvider` aliased to the set name; the short names repeat across sets, so there is no all-sets lookup.
`Select` calls `strategy.Select(scenario.Results)` and applies `resultSelector`. There is no assert member here — the check
is a single comparison, so the test makes it itself with `selected.ShouldBe(scenario.ExpectedResult)`.
`ResultSelectionTests` runs both strategy versions: `BestAlgorithm_v1/v2` (selector
`x => x.AlgorithmInfo.GetAlgorithmIdentifierName()`), `BestBin_v1/v2` and `SmallestBin_v1/v2` (selector
`x => x.Bin.ID`). See `$lib/result-selection`.

## Binacle.Lib.PackingEfficiency

Console host (not xUnit), in `lib/measure/`. `PackingRunner` (an `IRunner`) packs every Bischoff-suite scenario
with every algorithm version once and fills `PackingBag`; two `IReporter`s read the bag and each writes one
file under `lib/results/measurements/` through `Binacle.Reporting`'s `Measure` + `MarkdownFileWriter`: `PackingEfficiencyReporter` (`packing-efficiency.md`, one row per scenario with
the shipped fills, best and margin), `VersionParityReporter` (`version-parity.md`, one column per version, only rows
where any version differs). `ResultFiles` holds the two `ResultFile`s and the shared header sentence. Nothing else under `lib/results/` is
written by the harness. Not pass/fail; a change is a diff.

## Binacle.Lib.Benchmarks.Algorithms

In `lib/bench/`. Three tiers, the tier in the class name. `BenchmarkBase` holds the scenario, loads it in
`[GlobalSetup]` through the abstract `Load`, and `Run(factory)` executes it with the abstract `Operation`.

- `Smoke_<FFD|WFD|BFD>_<Packing|Fitting>` (`SmokeBase`): rows `v1` (baseline) and `v2`, the column from
  `[ParamsSource]` over `SmokeSet.Names`. `short` job by default.
- `Sample_<FFD|WFD|BFD>_<Packing|Fitting>` (`SampleBase`): rows `v1` (baseline) and `v2`, the column over
  `SampleSet.Names`. Default job by default.
- `Full_<Alg>_<Op>` (`FullBase`): the same rows, the column over `Binacle.Data.BischoffSuite.DataProvider.Names`,
  every Bischoff problem. `short` job by default.

Every recipe takes `dry`, `quick` or `precise`: the dry job, the short job, the default job.
Every class is `[MemoryDiagnoser]`. The recipe picks a tier with `--filter '*.<Tier>_*'`. Every `v1` method carries the deleted-with-v1 comment.

## Binacle.Lib.Benchmarks.ParallelAlgorithms

In `lib/bench/`. Loop against Parallel for `Best`'s race (`$lib/findings`), on every core count in `JobsByCoreCount`.
Every class names the lib's **internal** `AlgorithmFactory_v2()` (`lib/src/Binacle.Lib/AlgorithmFactories/`),
so `Binacle.Lib` grants the project friend access. Every class is `[MemoryDiagnoser]`.

`Identical/` holds `IdenticalBase` and five classes on it. The base holds the rows `Loop` (baseline) and
`Parallel`, the param `Pieces`, and the bin and item from `IdenticalCase`; setup throws unless every algorithm
packs every piece. The namespace stays at the project root. Each class names only the algorithms it races:

- `Identical_FFD_Packing`, `Identical_WFD_Packing`, `Identical_BFD_Packing` — one algorithm each. `Parallel`
  there is not a race: the default cap resolves to 1, so `Parallel.For` runs the body inline.
- `Identical_FFD_BFD_Packing` — the race the multi-bin routes run.
- `Identical_FFD_WFD_BFD_Packing` — the race the single-bin routes run.

The `Pieces` values live on the base, so all five sweep one grid and their reports can be read side by side.
The recipe filters all five in, because `Loop` against the sum of the alone classes only holds within one
invocation.

The short job by default.

Each core count is a BDN job, built in `JobsByCoreCount` (`shared/test/Binacle.Benchmarking`). Each sets the affinity
mask to the first N CPUs **and** `DOTNET_PROCESSOR_COUNT=N`: BDN pins the child after it starts, and .NET reads
its CPU count once at start-up. BDN adds a CLI `--job` beside declared jobs instead of applying it, so
`JobOption.Extract` takes `--job` and `--launches` out of the args and returns the job they name, and the word
it resolved; `Program.cs` builds the core jobs from that. Each job id is the word then the core count, as
`short 02 cores`, so the report header says which job made it. `JobOption.CreateConfig` does the same for the
projects that do not pin: the id is the word. `--launches N` is ours, not BDN's `--launchCount`: every job runs
one process per case, so `StdDev` only covers that process, and more launches is the only way to see a case
whose process started badly. It is applied with `WithLaunchCount` only when given, and a non-numeric or
non-positive value is rejected as an unknown `--job` is. The report has a `Cores` column, read from the affinity
mask, and hides `Job`, `Affinity` and `EnvironmentVariables`; the header still shows every CPU the machine has.

`SkipAttribute` marks a bench class whose question is answered: `[Skip("why")]`, the reason a required
argument. Classes only, and not inherited, so a mark on a base does not reach the classes on it. **Nothing
reads it** - no run honours it and no class carries one, so marking a class today changes nothing.

`JobsByCoreCount.SetCoreCountAndCheck`, first in every class's `[GlobalSetup]`, is Linux
only. On Linux BDN's pin (`sched_setaffinity` on the pid) reaches only the main thread, so it pins every thread
in `/proc/self/task` to the mask, then fails the case if `ProcessorCount` is not N or any thread's
`Cpus_allowed` differs. A pinned run is kinder than a real small VM: the OS and the BDN host run on the spare
CPUs.

## Binacle.Lib.Benchmarks.ParallelBins

In `lib/bench/`. Loop against Parallel for a request of many bins, on every core count in `JobsByCoreCount`, pinned as
above. `Identical/` holds `IdenticalBase`, two bases for the grids, and six classes. `IdenticalBase` takes the
algorithm and holds the rows `Loop` (baseline) and `Parallel`, the abstract params `Bins`, `Lines` and `Pieces`,
and the bin and item from `IdenticalCase`; setup throws unless every bin is fully packed. `PiecesBase` sweeps
bins and pieces on one line, `LinesBase` sweeps the same pieces over more lines, and the grid sits on the base
so the three classes of a kind cannot drift apart. `Identical_FFD_Pieces_Packing`, `Identical_WFD_Pieces_Packing`
and `Identical_BFD_Pieces_Packing` sit on the first; `Identical_FFD_Lines_Packing`,
`Identical_WFD_Lines_Packing` and `Identical_BFD_Lines_Packing` on the second. The namespace stays at the
project root. The recipe `lib-parallel-bins-identical` takes the algorithm, then the word, and its filter picks
both kinds for that algorithm. Short job by default.

## Binacle.Lib.Benchmarks.ParallelOverhead

In `lib/bench/`. The only bench project that does not pack: what parallelising costs, measured straight rather
than as the gap between two packing rows. No friend access - the one real result it needs comes from the public
`AlgorithmFactories.FFD_v2` in `Binacle.Lib.Testing`, run once in setup, because `OperationResult` has an
internal constructor.

All three classes are `[MemoryDiagnoser]` and run the same rows: `Loop` (baseline), `Parallel_OneThread`
(`maxDegreeOfParallelism: 1`, so `Parallel.For` runs on the calling thread) and `Parallel`. Both have a `Bytes`
param, `0` and `65536`.

- `FakeRaceBase` — abstract, the algorithm processors. Subclasses give only the `Algorithms` array. Setup
  throws unless the race returns one result per algorithm.
- `Fake_TwoAlgorithms` and `Fake_ThreeAlgorithms` — `FFD,BFD` and `FFD,WFD,BFD`, the races the multi-bin and
  single-bin routes run. One class each rather than a param, because a race's width is fixed by production.
- `Fake_Bins` — the bin processors, param `Bins` 1, 2 or 8. It stops at 8: at 32 bins of 64 KB one operation allocates and walks about 2 MB,
  which measured garbage collection and came back `NA` in two cells. A param rather than a class because a request
  carries any number. Setup throws unless one result comes back per bin.
- `FakeAlgorithmFactory` (in `lib/test/Binacle.Lib.Testing`) — an `IAlgorithmFactory` whose algorithms return
  the result passed to it. `Create` still allocates, `Execute` is still a virtual call and the identifier name
  is still built, so only the packing is absent. Its `bytes` argument is how much memory each instance walks,
  a cache line at a time, from a buffer of its own - the stand-in for a real algorithm's working set.

The default job, not the short one: the gaps it measures are small enough that a short run blurs them. Recipe
`lib-parallel-overhead` takes the word, then an optional launch count (`just bench lib-parallel-overhead precise 3`),
the only recipe that does.

Its `Program` passes `JobsByCoreCount.CreateConfig` an explicit `[1, 2, 4, 8, 12]`, so it adds a 1-CPU job to
the set every other project runs. `JobsByCoreCount.Counts` stays `[2, 4, 8, 12]`, so no other project's kept
runs move.

**Read the gaps, not the rows.** `Loop` to `Parallel_OneThread` is what `Parallel.For` costs to set up, measured
flat across every core count; `Parallel_OneThread` to `Parallel` is what handing work to another CPU costs, and
that one moves with the machine. **1 CPU is the dear end, not the cheap one** - `Parallel.For` still hands the
work to a worker, which then waits for the caller to come off the single CPU.

## Binacle.Lib.Benchmarks.ResultSelection

In `lib/bench/`. `BestAlgorithm`, `BestBin`, `SmallestBin` — one class per selector, `[MemoryDiagnoser]`, rows
`v1` (baseline) and `v2`, the scenario name as the column from `[ParamsSource]` over the set's
`DataProvider.Names`, aliased as `<Set>Data` because the bench class already carries the set's name. `BenchmarkBase` holds the name, loads the scenario in `[GlobalSetup]` through the
abstract `Load`, which each class points at its own set, and `Run(strategy)`. `short` job by default.

## Binacle.Lib.Benchmarks.Scaling

In `lib/bench/`. Packing time against item count, on the item ladder in `LadderGenerator`.
One class, `Sample_Packing`, `[MemoryDiagnoser]`: `[Params]` over every step (3 to 79 items), the bin fixed
at `MaxSizeBin`, and v1 and v2 of each algorithm as rows, through the public `AlgorithmFactories` - `FFD_v1`
(baseline), `FFD_v2`, `WFD_v1`, `WFD_v2`, `BFD_v1`, `BFD_v2`. Default job by default.
