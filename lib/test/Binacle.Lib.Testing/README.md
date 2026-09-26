# Binacle.Lib.Testing

What the lib suites, the measure project and the bench projects share and nothing else needs. Not a test
project: nothing in it asserts on its own, and no test SDK is referenced. Every other project under
`lib/test/`, `lib/measure/` and `lib/bench/` references it and imports it globally, except
`Binacle.Lib.Benchmarks.ResultSelection`, which needs none of it.

## 📂 What is in it

| Path | What it is |
|---|---|
| `AlgorithmFactories.cs` | Six `TestAlgorithmFactory` statics, one per algorithm version, each constructing the algorithm directly |
| `TestAlgorithmFactory.cs`, `TestOperationParameters.cs` | The delegate the factories are, and the parameters a test hands to `Execute` |
| `ScenarioChecks.cs` | `EvaluateResult` on a scenario's metrics and on its expected result - throws on mismatch |
| `OperationResultExtensions.cs` | Volume and count totals over an `OperationResult` |
| `SmokeSet.cs`, `SampleSet.cs`, `RacingSet.cs`, `CoresSet.cs` | The benchmark picks - the smoke four, the 30-problem Bischoff sample, the five curated Bischoff scenarios the old racing classes used (only the racing `BenchmarkBase` still reads it), and the 30 Bischoff problems the racing bench runs on every core count. Each answers `Names` and `GetByName(name)` |
| `CubeGenerator.cs`, `LadderGenerator.cs` | Scenarios built in code rather than picked - one cube baseline, and the bin and item ladders the threshold and scaling projects climb |

## 🛠️ How you use it

```csharp
var result = AlgorithmFactories.FFD_v2(scenario.Bin, scenario.Items)
    .Execute(new TestOperationParameters { Operation = AlgorithmOperation.Packing });

scenario.Metrics.EvaluateResult(result);
scenario.Result.For(Algorithm.FFD).EvaluateResult(result);
```

## ⚠️ What will bite you

The algorithm classes are internal to `Binacle.Lib`, so this project is a friend of it. A new project that
wants the factories references this one; it does not need a grant of its own.
