# Shared

Code and data that more than one slice uses. Nothing here is a product of its own - if only one slice reads
it, it belongs in that slice instead.

## 📂 What is in it

| Folder | What it is |
|---|---|
| `src/` | Four small libraries the whole repo compiles against - geometry, packing vocabulary, compact notation, result types |
| `data/` | The scenario corpus more than one slice reads, and `Binacle.Data`, which reads it - see [`data/README.md`](data/README.md) |
| `test/` | Test infrastructure shared by several suites - the report writer, the unit tests of the libraries |
| `tools/` | The OR-Library converter, which writes `data/bischoff-suite` |

## 📦 The libraries

Plain C# libraries, no framework and no I/O. They exist so `lib`, `api` and `vipaq` agree on the same types
rather than each defining their own.

| Project | What it is | Used by |
|---|---|---|
| `src/Binacle.Geometry` | Dimensions, coordinates, volume and quantity - the `IWith*` interfaces plus concrete `Dimensions`, `Coordinates`, `Item` | `Binacle.Packing`, `Binacle.CompactNotation`, ViPaq |
| `src/Binacle.Packing` | The packing vocabulary - `Algorithm`, `AlgorithmInfo`, `PackedBin`, `PackedItem`, `UnpackedItem`, `OperationResultStatus` | `Binacle.Lib`, the API, `Binacle.Data` and `Binacle.Lib.Data` |
| `src/Binacle.CompactNotation` | Parses and formats the terse strings used in fixtures and API payloads - `"60x40x30"`, `"108x76x30 [40]"` | The API, `Binacle.Data`, `Binacle.Lib.Data`, `Binacle.ViPaq.Data`, every generator |
| `src/Binacle.FluxResults` | Result and union types a repository or handler returns instead of throwing - `FluxUnion<T0, T1>`, `Success`, `NotFound`, `Conflict` | The service module - see [its README](src/Binacle.FluxResults/README.md) |

`Binacle.Packing` and `Binacle.CompactNotation` both build on `Binacle.Geometry`; nothing points the other way.
`Binacle.FluxResults` sits apart from all three and references nothing.

## 🧪 Test infrastructure

The scenario data and the code that reads it are `data/Binacle.Data` - see
[`data/Binacle.Data/README.md`](data/Binacle.Data/README.md).

### 🎯 `Binacle.FluxResults.UnitTests`

Union, extension-method and typed-result tests for `src/Binacle.FluxResults`. Runs with
`just test cs_binacle-flux-results_unit`.

### 📄 `Binacle.Reporting`

The runner-and-reporter loop behind the measure projects. Register a bag, `IRunner`s, `IReporter`s and an
`IFileWriter` in DI; `Measure` runs every runner, then every reporter, and writes one file per reporter.
Used by `lib/measure/Binacle.Lib.PackingEfficiency`, `vipaq/measure/Binacle.ViPaq.EncodedSize` and every data
generator under `shared/tools/` and `vipaq/tools/`. The reports it writes are the ones committed under [`results/`](../results).

### ⏱️ `Binacle.Benchmarking`

What every bench project runs with: `BenchmarkConfig.Create()` - the BenchmarkDotNet defaults, the GitHub
markdown report as the only export, reports pinned beside the calling project - and `[BenchmarkOrder(n)]`
with the orderer that sorts a group's rows by it. The only project that references BenchmarkDotNet, so the
unit tests and the measure projects never restore it.

### 🔤 `Binacle.CompactNotation.UnitTests`

Parse, format and round-trip tests for `src/Binacle.CompactNotation`. Runs with `just test cs_binacle-compact-notation_unit`.
The TypeScript twin of that parser lives in
[`packages/binacle-compact-notation`](../packages/binacle-compact-notation) and runs with `just test ts_binacle-compact-notation_unit`.

## 🛠️ Tools

`tools/Binacle.OrLibrary.Converter` turns the raw OR-Library text into the scenario JSON under
`data/bischoff-suite`.

```bash
just regen or-lib-scenarios
```

It takes no arguments and always runs every converter, so it cannot half-run and leave the fixtures
inconsistent. It rewrites committed files - run it only when you meant to.

## 📊 Data

[`data/`](data) holds the fixture corpus: the Bischoff suite, the custom problems, and the raw OR-Library
source they come from. Each folder has its own README with the file format.
