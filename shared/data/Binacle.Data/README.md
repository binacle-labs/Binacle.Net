# Binacle.Data

The scenario data of `shared/data` as C#. It embeds the three scenario sets next to it and reads them into
models the tests run against. Not a test project: nothing in it asserts, and no test SDK is referenced.

## 📂 What is in it

| Path | What it is |
|---|---|
| `BischoffSuite/`, `CustomProblems/`, `DemoSamples/` | One `Scenarios` class per set - its keys and every scenario by name; the first two also answer for their distinct bins |
| `All.cs` | Every scenario of every set, by name |
| `Scenario.cs`, `ScenarioBin.cs`, `ScenarioItem.cs`, `ScenarioMetrics.cs`, `ScenarioResult.cs`, `AlgorithmResult.cs` | The models a scenario reads into |
| `Helpers/` | The parsers for the compact `Metrics` and `Result` strings |
| `Files/` | The embedded-resource reader. It takes the assembly to read from, so other data projects use it too |
| `ExtensionMethods/`, `TestAlgorithmFactory.cs`, `TestOperationParameters.cs`, `PercentageComparer.cs` | Test helpers the lib and api suites share |

The set folders you see in the IDE are not on disk. The JSON lives in the sibling folders (`../bischoff-suite`,
`../custom-problems`, `../demo-samples`) and is linked in by the csproj; edit it there. A new file is embedded
on its own, but is not read until its key is in that set's `Scenarios.Keys`.

## 🛠️ How you use it

```csharp
using Binacle.Data.BischoffSuite;

foreach (var scenario in Scenarios.GetScenarios()) { ... }
Scenarios.GetScenarioByName("orlib_thpack1_1");
All.GetScenarioByName("baseline_1");   // any set
```

A file that reads one set imports that set's namespace and writes `Scenarios`. A file that reads two writes the
full name, `Binacle.Data.CustomProblems.Scenarios`.

## ⚠️ What will bite you

The manifest name of each embedded file is `Binacle.Data.<Set>.<name>.json`, set by `LogicalName` in the
csproj. The reader splits on it, and a wrong name fails silently - the set comes back empty. After touching the
csproj, check with `strings bin/Debug/net10.0/Binacle.Data.dll | grep Binacle.Data.BischoffSuite`.
