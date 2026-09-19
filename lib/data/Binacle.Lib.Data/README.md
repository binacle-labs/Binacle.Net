# Binacle.Lib.Data

The result-selection scenarios of `lib/data/result-selection` as C#. It embeds them and reads them into models
the lib unit tests and benchmarks run against. Not a test project: nothing in it asserts, and no test SDK is
referenced. Nothing outside the lib slice reads it.

## 📂 What is in it

| Path | What it is |
|---|---|
| `ResultSelection/BestAlgorithm/`, `BestBin/`, `SmallestBin/` | One `Scenarios` class per set - its keys and every scenario by name |
| `ResultSelection/All.cs` | Every scenario of every set, by name |
| `ResultSelection/Scenario.cs` | A named case: the candidate results and which one should win |
| `ResultSelection/Helpers/` | The parsers for the compact `OperationResult` and `AlgorithmInfo` strings |

The set folders you see in the IDE are not on disk. The JSON lives in `../result-selection` and is linked in by
the csproj; edit it there. A new file is embedded on its own, but is not read until its key is in that set's
`Scenarios.Keys`.

The embedded-resource reader is `Binacle.Data`'s; this project passes its own assembly to it.

## 🛠️ How you use it

```csharp
using Binacle.Lib.Data.ResultSelection.BestBin;

foreach (var scenario in Scenarios.GetScenarios()) { ... }
```

A file that reads one set imports that set's namespace and writes `Scenarios`. A file that reads several gives
each an alias: `using BestBin = Binacle.Lib.Data.ResultSelection.BestBin.Scenarios;`.

## ⚠️ What will bite you

The manifest name of each embedded file is `ResultSelection.<Set>.<name>.json`, set by `LogicalName` in the
csproj. The reader splits on it, and a wrong name fails silently - the set comes back empty. After touching the
csproj, check with `strings bin/Debug/net10.0/Binacle.Lib.Data.dll | grep ResultSelection`.
