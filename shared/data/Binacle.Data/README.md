# Binacle.Data

The scenario data of `shared/data` as C#. It embeds the three scenario sets next to it and reads them into
models the tests run against. Not a test project: nothing in it asserts, no test SDK is referenced, and it
names no packing result - the checks live with the lib tests.

## 📂 What is in it

| Path | What it is |
|---|---|
| `BischoffSuite/`, `CustomProblems/`, `DemoSamples/` | One class per set - its keys and every scenario by name; the first two also answer for their distinct bins. Bischoff is `DataProvider`, the other two are still `Scenarios` |
| `All.cs` | Every scenario of every set, by name |
| `Scenario.cs`, `ScenarioBin.cs`, `ScenarioItem.cs`, `ScenarioMetrics.cs`, `ScenarioResult.cs`, `AlgorithmResult.cs` | The models a scenario reads into |
| `Helpers/` | The parsers for the compact `Metrics` and `Result` strings |
| `Files/` | The embedded-resource reader. It takes the assembly to read from, so other data projects use it too |
| `PercentageComparer.cs` | The 0.1% tolerance the lib and api suites compare fill percentages with |

The set folders you see in the IDE are not on disk. The JSON lives in the sibling folders (`../bischoff-suite`,
`../custom-problems`, `../demo-samples`) and is linked in by the csproj; edit it there. A new file is embedded
on its own, but is not read until its key is in that set's `Keys`.

## 🛠️ How you use it

```csharp
using Binacle.Data;

foreach (var scenario in BischoffSuite.DataProvider.All) { ... }
BischoffSuite.DataProvider.GetByName("OrLibrary_thpack1_1");
BischoffSuite.DataProvider.ByCollection("BischoffSuite/orlib_thpack1");   // one thpack
All.GetScenarioByName("Complex_FitsInSmall_1");                           // any set
```

The sets are mid-rename. Bischoff is `DataProvider`: import `Binacle.Data` and write the set on the line, so
you can see which one you read. The other two are still `Scenarios`, read by importing that set's namespace
(`using Binacle.Data.CustomProblems;`) and writing `Scenarios`.

## ⚠️ What will bite you

The manifest name of each embedded file is `Binacle.Data.<Set>.<name>.json`, set by `LogicalName` in the
csproj. The reader splits on it, and a wrong name fails silently - the set comes back empty. After touching the
csproj, check with `strings bin/Debug/net10.0/Binacle.Data.dll | grep Binacle.Data.BischoffSuite`.
