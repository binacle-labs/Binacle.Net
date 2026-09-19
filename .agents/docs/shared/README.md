---
id: shared
description: Shared slice — Binacle.Data (algorithm scenario data, compact-string formats, the set classes, the one embedded-resource reader) and shared/data (the fixture corpus more than one slice reads)
verified: 2026-09-19
check: Key arrays, compact-string parsers (Result is a per-algorithm map, not a bare string), and the set class names and methods match shared/data/Binacle.Data; the embedded-resource folders in Binacle.Data.csproj match the folders under shared/data and the Keys arrays in BischoffSuite/Scenarios.cs and CustomProblems/Scenarios.cs; OR-Library files match shared/data
also_update:
  - lib/tests
  - api/tests
paths:
  - "shared/**"
---

# Shared

`shared/` holds code used across more than one slice. Two parts:

- `shared/data/Binacle.Data` — the scenario data and the code that reads it (reader, parsers, models, set classes)
- `shared/data` — the scenario JSON that more than one slice reads (`bischoff-suite/`, `custom-problems/`,
  `demo-samples/`), plus the raw OR-Library benchmark data

## Who uses Binacle.Data

Four project references, all support or test projects:

- `lib/test/Binacle.Lib.UnitTests`
- `lib/test/Binacle.Lib.PerformanceTests`
- `lib/test/Binacle.Lib.Benchmarks`
- `api/test/Binacle.Net.IntegrationTests`

`Binacle.Net.ServiceModule.IntegrationTests` and `vipaq/test/Binacle.ViPaq.UnitTests` do **not** use it —
they have their own self-contained fixtures (ServiceModule) or use Bogus fakers (ViPaq). Nothing in `src`
references it, in any slice.

## One area

This project holds the algorithm scenarios only. **Result selection lives in `lib/test/Binacle.Lib.TestsKernel`**,
because nothing outside the lib slice reads it — see `$lib/dependencies`.

The project root also holds what both audiences share: `ScenarioBin`, `ScenarioItem`, `TestOperationParameters`,
`TestAlgorithmFactory`, `PercentageComparer`, `AssertionMethodAttribute` and the `Files/` reader.

## Scenario sets

One namespace per set, one static `Scenarios` class in each, and the set's keys are on that class:

| Class | `Keys` | Count |
|---|---|---|
| `Binacle.Data.BischoffSuite.Scenarios` | `BischoffSuite/orlib_thpack1` … `orlib_thpack7` | 7 |
| `Binacle.Data.CustomProblems.Scenarios` | `CustomProblems/baseline`, `/simple`, `/complex` | 3 |

Data is embedded JSON, loaded by resource prefix. The manifest name is `Binacle.Data.<Set>.<name>.json`, and
`ScenarioCollectionsProvider` splits it into the collection key `<set>/<name>` lowercased.

**A third folder is embedded and has no key set.** `shared/data/demo-samples/` comes in under
`Binacle.Data.DemoSamples.`, so `ScenarioCollectionsProvider.Collections` holds it under keys like
`demosamples/01-opening-set` — but no `Scenarios` class names it and `All` does not include it, so no C# test
reads it today. Its consumers are the demo component in `packages/binacle-net-ui/` and ViPaq, which reads the
*packed* form from `vipaq/data/packed/demo-samples/` through its own kernel.

**The set folders in the IDE are not on disk.** Every scenario JSON lives under `shared/data/` and is pulled in
as an `EmbeddedResource` with a `<Link>`, so it only *looks* like `BischoffSuite/…` in the IDE. To edit a
scenario, open `shared/data/bischoff-suite/` or `shared/data/custom-problems/`. The csproj sets `LogicalName` so
the manifest name stays what the reader expects, one flat entry per folder — a `**` wildcard corrupts that name.

**Why these two sets are here and result-selection is not.** A fixture set lives in `shared/data` when more than
one slice reads it. Bischoff and custom-problems qualify twice over: the api integration suite and the lib tests
both read them through this project, and the ViPaq packed-data generator reads the same files by path at run
time. Result-selection had one consumer, so it lives in `lib/data`.

## The embedded-resource reader

`Files/EmbeddedResourceFileProvider.ByPrefix(assembly, prefix)` lists the resources of the assembly it is
given whose manifest name starts with the prefix. Each comes back as an `EmbeddedResourceFile` with the name
after the prefix unsplit, and `OpenRead()` reads from that same assembly. **The caller names the assembly**
(`typeof(SomeTypeInThatProject).Assembly`) and splits the name its own way, so one reader serves every data
project however its manifest names are shaped.

## Compact-string formats

Scenario JSON keeps values terse. Each field has its own parser. Verify against the parser, not by guessing.

| Field | Parser | Rule | Real example |
|---|---|---|---|
| Dimensions | `ScenarioBin.FromCompactString` / `ScenarioItem.FromCompactString` via the shared `Binacle.CompactNotation` parser | `"LxWxH"` or `"LxWxH [Q]"` — `ScenarioBin` calls `CompactNotationParser.ParseDimensions<int>` (3 ints L,W,H); `ScenarioItem` calls `CompactNotationParser.ParseDimensionsAndQuantity<int>`, which splits off the optional `[Q]` (quantity, default 1) | `"108x76x30 [40]"`, `"60x40x10"` |
| Metrics (4) | `Helpers/ScenarioMetricsHelper.cs` | exactly 4 space-separated: `ItemsVolume BinVolume ItemsCount Percentage` (first 3 int, last decimal, trailing `%` trimmed) | `"29736390 30089620 112 98.83"` |
| Result (a map) | `Helpers/ScenarioResultHelper.cs` | a JSON object keyed by algorithm name, and **it must name every one** — `FFD`, `WFD`, `BFD`. Each value is 2 space-separated statuses: **`parts[0]` = packing, `parts[1]` = fitting** | `{"FFD": "PartiallyPacked PartiallyPacked", "WFD": …, "BFD": …}` |

The result-selection formats (the 5-part `OperationResult` and the `"Name_vN"` `AlgorithmInfo`) live with their
parsers in `lib/test/Binacle.Lib.TestsKernel/ResultSelection/Helpers/`.

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

## The set classes

Static, lazily built, keyed by scenario `Name`. Each exposes `GetScenarioNames()`, `ScenarioNames`
(`IEnumerable<object[]>` for xUnit `[MemberData]`), `GetScenarios()`, `GetScenarioByName(name)`.

- `Binacle.Data.All` (Bischoff + Custom, the by-name lookup), `Binacle.Data.BischoffSuite.Scenarios`,
  `Binacle.Data.CustomProblems.Scenarios`

A file that reads one set imports its namespace and writes `Scenarios`; a file that reads two writes the full
name. The result-selection providers in `lib/test/Binacle.Lib.TestsKernel` follow the same shape.

### The bins a set runs against

The two set classes also answer for their **bins**, because the API tests register exactly those as a preset
and must not restate the list (see `$api/tests`):

- `BischoffSuite.Scenarios.GetDistinctBins()` and `CustomProblems.Scenarios.GetDistinctBins()` — one
  `ScenarioBin` per ID, in the order the scenarios introduce them.
- `CustomProblems.Scenarios` adds `GetDistinctBinIds()` and `GetSmallestBin()` (least volume).

Add a scenario with a new bin and the set grows on its own — nothing else needs editing.

**The two classes do not match, on purpose.** Each carries only what something calls: both presets are
registered from `GetDistinctBins()`, but only `custom-problems` tests ask for the ids or the smallest bin. Add
the missing pair to Bischoff when a caller needs it, not to even the two up.

## Models and helpers

Models: `ScenarioBin` (`IWithID, IWithDimensions`), `ScenarioItem` (`IWithID, IWithDimensions, IWithQuantity`),
`TestOperationParameters`. `ScenarioBin`/`ScenarioItem` each expose a `FromCompactString` factory (and a
`Binacle.Geometry.IWithDimensions<int>` ctor) that parse via the shared notation. `Scenario` carries `Name` +
bin + items + `ScenarioMetrics` + `ScenarioResult`, and `Scenario.Create` is what parses all three from their
compact forms.

**`ScenarioResult` is a map, and `AlgorithmResult` is one entry in it.** `scenario.Result.For(algorithm)`
returns the `AlgorithmResult` for that algorithm and throws if the scenario does not name it.
`AlgorithmResult` carries `PackingStatus`, `PackingEarlyExitReason`, `FittingStatus` and
`FittingEarlyExitReason`.

The project defines **no xUnit fixtures** — those live in the test projects (see lib tests (`$lib/tests`)).
It provides:

- `TestAlgorithmFactory<TAlgorithm>` — `delegate TAlgorithm (ScenarioBin bin, List<ScenarioItem> items)`
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
result vocabulary, `Binacle.Data` the scenario hub — is in `$shared/dependencies`. Nothing in this slice
references `lib`, `api` or `vipaq`.
