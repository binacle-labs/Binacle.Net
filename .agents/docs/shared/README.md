---
id: shared
description: Shared slice — Binacle.TestsKernel (algorithm scenario data, compact-string formats, providers, fixtures) and shared/data (the fixture corpus more than one slice reads)
verified: 2026-09-04
check: Collection keys, compact-string parsers (Result is a per-algorithm map, not a bare string), and provider class names and methods match shared/test/Binacle.TestsKernel; the embedded-resource folders in Binacle.TestsKernel.csproj match the folders under shared/data and the key sets in Algorithms/CollectionKeys.cs; OR-Library files match shared/data
also_update:
  - lib/tests
  - api/tests
paths:
  - "shared/**"
---

# Shared

`shared/` holds code used across more than one slice. Two parts:

- `shared/test/Binacle.TestsKernel` — algorithm test scenario infrastructure (data, parsers, providers, models)
- `shared/data` — the scenario JSON that more than one slice reads (`bischoff-suite/`, `custom-problems/`,
  `demo-samples/`), plus the raw OR-Library benchmark data

## Who uses Binacle.TestsKernel

Four project references, all test projects:

- `lib/test/Binacle.Lib.UnitTests`
- `lib/test/Binacle.Lib.PerformanceTests`
- `lib/test/Binacle.Lib.Benchmarks`
- `api/test/Binacle.Net.IntegrationTests`

`Binacle.Net.ServiceModule.IntegrationTests` and `vipaq/test/Binacle.ViPaq.UnitTests` do **not** use it —
they have their own self-contained fixtures (ServiceModule) or use Bogus fakers (ViPaq). Nothing in `src`
references it, in any slice.

## One area

This kernel holds the algorithm side only. **Result selection lives in `lib/test/Binacle.Lib.TestsKernel`**,
because nothing outside the lib slice reads it — see `$lib/dependencies`. What is here:

- `Binacle.TestsKernel.Algorithms.*` — fit/pack scenarios (bin + items + expected metrics + expected result)

The kernel root still holds what both audiences share: `TestBin`, `TestItem`, `TestOperationParameters`,
`TestAlgorithmFactory`, `PercentageComparer`, `AssertionMethodAttribute` and the `Files/` reader.

## Scenario suites

Algorithms (`Algorithms/CollectionKeys.cs`):

| Key set | Members | Count |
|---|---|---|
| `BischoffSuite` | `BischoffSuite/orlib_thpack1` … `orlib_thpack7` | 7 |
| `CustomProblems` | `CustomProblems/baseline`, `/simple`, `/complex` | 3 |

Data is embedded JSON under `Algorithms/Data/<suite>/`, loaded by resource prefix. The collection key is
`{folder}/{name}` lowercased.

**A third folder is embedded and has no key set.** `shared/data/demo-samples/` comes in under
`Algorithms/Data/DemoSamples/`, so `ScenarioCollectionsProvider.Collections` holds it under keys like
`demosamples/01-opening-set` — but `CollectionKeys.cs` names no set for it and `AllScenariosProvider` does not
include it, so no C# test reads it through this kernel today. Its consumers are the demo component in
`packages/binacle-net-ui/` and ViPaq, which reads the *packed* form from `vipaq/data/packed/demo-samples/`
through its own kernel.

**That `Data/` folder is not on disk.** Every scenario JSON lives under `shared/data/` and is pulled in as an
`EmbeddedResource` with a `<Link>`, so it only *looks* like `…/Data/…` in the IDE. To edit a scenario, open
`shared/data/bischoff-suite/` or `shared/data/custom-problems/`. The csproj sets `LogicalName` so the manifest
name stays what the readers expect, one flat entry per folder — a `**` wildcard corrupts that name.

**Why these two sets are here and result-selection is not.** A fixture set lives in `shared/data` when more than
one slice reads it. Bischoff and custom-problems qualify twice over: the api integration suite and the lib tests
both read them through this kernel, and the ViPaq packed-data generator reads the same files by path at run time.
Result-selection had one consumer, so it lives in `lib/data`.

## Compact-string formats

Scenario JSON keeps values terse. Each field has its own parser. Verify against the parser, not by guessing.

| Field | Parser | Rule | Real example |
|---|---|---|---|
| Dimensions | `TestBin.FromCompactString` / `TestItem.FromCompactString` (in `Models/`) via the shared `Binacle.CompactNotation` parser | `"LxWxH"` or `"LxWxH [Q]"` — `TestBin` calls `CompactNotationParser.ParseDimensions<int>` (3 ints L,W,H); `TestItem` calls `CompactNotationParser.ParseDimensionsAndQuantity<int>`, which splits off the optional `[Q]` (quantity, default 1) | `"108x76x30 [40]"`, `"60x40x10"` |
| Metrics (4) | `Algorithms/Helpers/ScenarioMetricsHelper.cs` | exactly 4 space-separated: `ItemsVolume BinVolume ItemsCount Percentage` (first 3 int, last decimal, trailing `%` trimmed) | `"29736390 30089620 112 98.83"` |
| Result (a map) | `Algorithms/Helpers/ScenarioResultHelper.cs` | a JSON object keyed by algorithm name, and **it must name every one** — `FFD`, `WFD`, `BFD`. Each value is 2 space-separated statuses: **`parts[0]` = packing, `parts[1]` = fitting** | `{"FFD": "PartiallyPacked PartiallyPacked", "WFD": …, "BFD": …}` |

The result-selection formats (the 5-part `OperationResult` and the `"Name_vN"` `AlgorithmInfo`) moved with their
parsers to `lib/test/Binacle.Lib.TestsKernel/ResultSelection/Helpers/`.

**Keyed by algorithm, never by version.** Every version of an algorithm must produce the same result, so
there is deliberately no way to name `FFD_v1` and `FFD_v2` apart. `ParseFromMap` rejects an unknown name, a
name given twice, and a map missing any algorithm — the round trip through `Enum.TryParse` is what rejects
`"0"` and `"ffd"`. **A bare string is rejected**: a scenario whose algorithms all agree repeats the same pair
under each key rather than collapsing.

Both halves of each **result** string parse as the `Binacle.Packing` enum `OperationResultStatus`
(`Unknown=-1, FullyPacked, PartiallyPacked, NotPacked, EarlyExit`) — not the API fit/pack enums, so there is no
`AllItemsFit`/`NotAllItemsFit` here. A half may carry an early-exit reason as `Status-EarlyExitReason`
(`EarlyExitReason`: `None`, `ContainerVolumeExceeded`, `ContainerDimensionExceeded`); in real data only the
fitting half early-exits (pack never does), e.g. `"NotPacked EarlyExit-ContainerDimensionExceeded"`.

Metrics check: `ItemsVolume`, `BinVolume`, `ItemsCount` must match exactly; `Percentage` is an **upper bound** —
the actual `PackedBinVolumePercentage` must be ≤ expected, within a 0.1% tolerance (`PercentageComparer`).

## Providers

Static, lazily built, keyed by scenario `Name`. Each exposes `GetScenarioNames()`, `ScenarioNames`
(`IEnumerable<object[]>` for xUnit `[MemberData]`), `GetScenarios()`, `GetScenarioByName(name)`.

- Algorithms: `AllScenariosProvider` (Bischoff + Custom), `BischoffSuiteScenarioProvider`, `CustomProblemsScenarioProvider`

The result-selection providers moved to `lib/test/Binacle.Lib.TestsKernel` and follow the same shape.

### The bins a suite runs against

The two algorithm suite providers also answer for their **bins**, because the API tests register exactly those
as a preset and must not restate the list (see `$api/tests`):

- `BischoffSuiteScenarioProvider.GetDistinctBins()` and `CustomProblemsScenarioProvider.GetDistinctBins()` —
  one `TestBin` per ID, in the order the scenarios introduce them.
- `CustomProblemsScenarioProvider` adds `GetDistinctBinIds()` and `GetSmallestBin()` (least volume).

Add a scenario with a new bin and the set grows on its own — nothing else needs editing.

**The two providers do not match, on purpose.** Each carries only what something calls: both presets are
registered from `GetDistinctBins()`, but only `custom-problems` tests ask for the ids or the smallest bin. Add
the missing pair to Bischoff when a caller needs it, not to even the two up.

## Models and helpers

Models: `TestBin` (`IWithID, IWithDimensions`), `TestItem` (`IWithID, IWithDimensions, IWithQuantity`),
`TestOperationParameters`. `TestBin`/`TestItem` each expose a `FromCompactString` factory (and a
`Binacle.Geometry.IWithDimensions<int>` ctor) that parse via the shared notation. Algorithms `Scenario`
carries `Name` + bin + items + `ScenarioMetrics` + `ScenarioResult`, and `Scenario.Create` is what parses all
three from their compact forms.

**`ScenarioResult` is a map, and `AlgorithmResult` is one entry in it.** `scenario.Result.For(algorithm)`
returns the `AlgorithmResult` for that algorithm and throws if the scenario does not name it.
`AlgorithmResult` carries `PackingStatus`, `PackingEarlyExitReason`, `FittingStatus` and
`FittingEarlyExitReason`.

The kernel defines **no xUnit fixtures** — those live in the test projects (see lib tests (`$lib/tests`)).
It provides:

- `TestAlgorithmFactory<TAlgorithm>` — `delegate TAlgorithm (TestBin bin, List<TestItem> items)`
- `EvaluateResult` extensions on `ScenarioMetrics` and on `AlgorithmResult` — **not on `ScenarioResult`**, which
  is the map. The `AlgorithmResult` one picks the packing or the fitting expected status by
  `result.AlgorithmOperation`, then throws on mismatch. A caller reaches it as
  `scenario.Result.For(result.AlgorithmInfo.Algorithm).EvaluateResult(result)`.
- `OperationResultExtensions` (volume/count totals) and `PercentageComparer` (0.1% tolerance)

## shared/data — OR-Library

`shared/data/or-library/` holds the raw OR-Library container-loading text files
(`thpack1.txt` … `thpack9.txt`, plus `thpack9-fixed.txt` and a `README.md`). Source: OR-Library (J.E. Beasley);
`thpack1–7` are Bischoff & Ratcliff (1995). These raw files are the upstream origin that was converted into the
embedded `BischoffSuite/orlib_thpack1..7.json`. Only `thpack1–7` map to `BischoffSuite`; `thpack8/9` are not in
the embedded suite. `thpack9-fixed.txt` patches a missing indicator in thpack9 problems 18–20.

**The converter carries the published result as a fixed baseline, so it never runs the packer.** That is why
`Binacle.OrLibrary.Converter` needs no dependency on `lib` - it reads and writes `shared/` and nothing else.
Recomputing the baseline instead of copying it would put the packer in the bottom slice.

## Dependencies

How the shared projects reference each other and who sees internals — `Geometry` the leaf, `Binacle.Packing` the
result vocabulary, `TestsKernel` the algorithm fixture hub — is in `$shared/dependencies`. Nothing in this slice
references `lib`, `api` or `vipaq`.
