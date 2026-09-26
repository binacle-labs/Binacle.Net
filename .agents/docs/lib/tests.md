---
id: lib/tests
description: lib/test projects — Binacle.Lib.Testing (the one AlgorithmFactories, the scenario checks, the benchmark providers), unit tests, the five bench projects in lib/bench with their tiers, and the measure project in lib/measure; CommonTestingFixture, ResultSelectionTestingFixture, and run aliases
verified: 2026-09-26
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
in `lib/measure/` and the five bench projects in `lib/bench/`. Algorithm scenario data
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
| `Binacle.Lib.Benchmarks.Racing` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-racing-cores` |
| `Binacle.Lib.Benchmarks.ResultSelection` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-result-selection` |
| `Binacle.Lib.Benchmarks.Threshold` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-threshold-smoke`, `-sample`, `-full` |
| `Binacle.Lib.Benchmarks.Scaling` (`lib/bench/`) | BenchmarkDotNet, config from `shared/test/Binacle.Benchmarking` | `just bench lib-scaling` |

## Binacle.Lib.Testing

The harness code the unit tests, the measure project and the bench projects share, imported globally
(`<Using Include="Binacle.Lib.Testing" />` in each csproj except `Binacle.Lib.Benchmarks.ResultSelection`, which
does not reference it). `Binacle.Lib` grants it friend access, because
it constructs the internal algorithm classes.

- `AlgorithmFactories.cs` defines six `TestAlgorithmFactory<IPackingAlgorithm>` statics — `FFD_v1/_v2`,
  `WFD_v1/_v2`, `BFD_v1/_v2` — each constructing the algorithm directly
  (`new FirstFitDecreasing_v2<ScenarioBin, ScenarioItem>(bin, items)`), **not** through `IAlgorithmFactory`/DI.
  This keeps every version (including v1) under test without coupling it to the production factory.
- `TestAlgorithmFactory<TAlgorithm>` — `delegate TAlgorithm (ScenarioBin bin, List<ScenarioItem> items)` — and
  `TestOperationParameters`, the `IOperationParameters` a test hands to `Execute`.
- `ScenarioChecks` — the two `EvaluateResult` extensions, on `ScenarioMetrics` and on `AlgorithmResult`
  (**not** on `ScenarioResult`, which is the map). The `AlgorithmResult` one picks the packing or the fitting
  expected status by `result.AlgorithmOperation`, then throws on mismatch. `OperationResultExtensions` holds
  the volume and count totals they compare against.
- The benchmark picks, at the project root, each answering `Names` and `GetByName(name)`: `SmokeSet` (the four
  smoke scenarios by name: `full bin, one type`, `small order`, `typical container`, `many item types`),
  `SampleSet` (30 Bischoff problems, name `<category> (<id>)`), `CoresSet` (30 Bischoff problems spread by FFD+BFD time, name `<problem> (<items>i/<types>t)`,
  e.g. `th1_72 (74i/3t)`).
- The generators beside them, which build rather than pick: `CubeGenerator` (one cube baseline, `GetBaseline`)
  and `LadderGenerator` (the bin and item ladders the threshold and scaling projects climb).

## Binacle.Lib.UnitTests

Keeps its own `AssertionMethodAttribute`, the Sonar S2699 marker; ViPaq's unit tests carry a copy too.

Both fixtures split arrange, act and assert into separate members, so a test body shows all three steps
rather than handing them to one helper.

`CommonTestingFixture` holds all six factories in `AlgorithmsUnderTest[]` and exposes:

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
`PackingCustomProblemsTests`, `PackingDemoSamplesTests` (each a `[Theory]` × `[MemberData]` over all six versions), plus `CreationTests`,
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

Console host (not xUnit), in `lib/measure/`. `PackingRunner` (an `IRunner`) packs the 700 Bischoff-suite scenarios
with all six algorithm versions once and fills `PackingBag`; two `IReporter`s read the bag and each writes one
file under `lib/results/measurements/` through `Binacle.Reporting`'s `Measure` + `MarkdownFileWriter`: `PackingEfficiencyReporter` (`packing-efficiency.md`, one row per scenario with
the shipped fills, best and margin), `VersionParityReporter` (`version-parity.md`, only rows where v1 and v2
differ). `ResultFiles` holds the two `ResultFile`s and the shared header sentence. Nothing else under `lib/results/` is
written by the harness. Not pass/fail; a change is a diff.

## Binacle.Lib.Benchmarks.Algorithms

In `lib/bench/`. Three tiers, the tier in the class name. `BenchmarkBase` holds the scenario, loads it in
`[GlobalSetup]` through the abstract `Load`, and `Run(factory)` executes it with the abstract `Operation`.

- `Smoke_<FFD|WFD|BFD>_<Packing|Fitting>` (`SmokeBase`): rows `v1` (baseline) and `v2`, the column from
  `[ParamsSource]` over `SmokeSet.Names`. 48 cases, `short` job.
- `Sample_<FFD|WFD|BFD>_<Packing|Fitting>` (`SampleBase`): rows `v1` (baseline) and `v2`, the column over
  `SampleSet.Names`. 360 cases, default job, `short` with `quick`.
- `Full_<Alg>_<Op>` (`FullBase`): the same rows, the column over `Binacle.Data.BischoffSuite.DataProvider.Names`,
  all 700. 8,400 cases, `short` job, default with `precise`.

Every class is `[MemoryDiagnoser]`. The recipe picks a tier with `--filter '*.<Tier>_*'`. Every `v1` method carries the deleted-with-v1 comment.

## Binacle.Lib.Benchmarks.Racing

In `lib/bench/`. Loop against Parallel for `Best`'s race (`$lib/findings`), on 2, 4, 8 and 12 cores.
`Cores_Packing` names the lib's **internal** `AlgorithmFactory_v2()`
(`lib/src/Binacle.Lib/AlgorithmFactories/`), so `Binacle.Lib` grants the project friend access.

`Cores_Packing` is the one class that runs. v2, the column over `CoresSet.Names`, seven rows in three
`[BenchmarkCategory]` blocks: `Loop_FFD_BFD` (baseline) and `Parallel_FFD_BFD`; `Loop_FFD_WFD_BFD` (baseline)
and `Parallel_FFD_WFD_BFD`; and `FFD`, `WFD`, `BFD` alone, each a `LoopAlgorithmProcessor` of one, with no
baseline, so their Ratio is `?`. 30 x 7 x 4 core counts = 840 cases, `short` job, default with `precise`.
`[MemoryDiagnoser]`.

The core counts are four BDN jobs, built in `CoreJobs`. Each sets the affinity mask to the first N CPUs **and**
`DOTNET_PROCESSOR_COUNT=N`: BDN pins the child after it starts, and .NET reads its CPU count once at start-up.
BDN adds a CLI `--job` beside declared jobs instead of applying it, so `Program.cs` takes `--job` out of the
args and builds the four jobs from it; the recipe passes it as every other recipe does. The report has a
`Cores` column and hides `Job`, `Affinity` and `EnvironmentVariables`; the header still says 12 CPUs.

`CorePinning.PinAndCheck`, first in `[GlobalSetup]`, is Linux only. On Linux BDN's pin (`sched_setaffinity`
on the pid) reaches only the main thread, so it pins every thread in `/proc/self/task` to the mask, then fails
the case if `ProcessorCount` is not N or any thread's `Cpus_allowed` differs. A pinned run is kinder than a
real small VM: the OS and the BDN host run on the spare CPUs.

## Binacle.Lib.Benchmarks.ResultSelection

In `lib/bench/`. `BestAlgorithm`, `BestBin`, `SmallestBin` — one class per selector, `[MemoryDiagnoser]`, rows
`v1` (baseline) and `v2`, the scenario name as the column from `[ParamsSource]` over the set's
`DataProvider.Names`, aliased as `<Set>Data` because the bench class already carries the set's name. `BenchmarkBase` holds the name, loads the scenario in `[GlobalSetup]` through the
abstract `Load`, which each class points at its own set, and `Run(strategy)`. 11 scenarios, 22 cases, always
the `short` job.

## Binacle.Lib.Benchmarks.Scaling

In `lib/bench/`. Packing time against item count, on the item ladder in `LadderGenerator`.
One class, `Sample_Packing`, `[MemoryDiagnoser]`: `[Params]` over all 11 steps (3 to 79 items), the bin fixed
at `MaxSizeBin`, and six rows through the public `AlgorithmFactories` - `FFD_v1` (baseline), `FFD_v2`,
`WFD_v1`, `WFD_v2`, `BFD_v1`, `BFD_v2`. 66 cases at the default job, `short` with `quick`.

## Binacle.Lib.Benchmarks.Threshold

In `lib/bench/`. Loop against Parallel on the ladders in `LadderGenerator`, the evidence for
whether the parallel processors get wired up (`$lib/findings`). Two bases, each with rows `Loop` (baseline) and
`Parallel`, the lib's **internal** factories, and abstract `Items` (and `Bins`) params so each tier picks its
own steps:

- `AlgorithmsBase` — `LoopAlgorithmProcessor` / `ParallelAlgorithmProcessor`, param `Set` (`FFD,BFD`,
  `FFD,WFD,BFD`), one bin (`MaxSizeBin`). Classes `Smoke_Algorithms_Packing` (v2; items 3, 47, 67, 79),
  `Sample_Algorithms_Packing` (v2; all 11 items), `Full_Algorithms_Packing_v1` and `_v2` (all 11 items).
- `BinsBase` — `LoopBinProcessor` / `ParallelBinProcessor`, param `Algorithm` (FFD, BFD). Classes
  `Smoke_Bins_Packing` (v2; bins 2, 3, 7), `Sample_Bins_Packing` (v2; items 3, 47, 79; bins 1-7),
  `Full_Bins_Packing_v1` and `_v2` (all 11 items, bins 1-7).

Smoke 64 cases at `short`; sample 128 at the default job, `short` with `quick`; full 704 at `short`, default with
`precise`.
