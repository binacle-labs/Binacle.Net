# Binacle.Lib.Testing

What the three lib suites share and nothing else needs. Not a test project: nothing in it asserts on its own,
and no test SDK is referenced. Every project under `lib/test/` references it and imports it globally.

## 📂 What is in it

| Path | What it is |
|---|---|
| `AlgorithmFactories.cs` | Six `TestAlgorithmFactory` statics, one per algorithm version, each constructing the algorithm directly |
| `TestAlgorithmFactory.cs`, `TestOperationParameters.cs` | The delegate the factories are, and the parameters a test hands to `Execute` |
| `ScenarioChecks.cs` | `EvaluateResult` on a scenario's metrics and on its expected result - throws on mismatch |
| `OperationResultExtensions.cs` | Volume and count totals over an `OperationResult` |
| `Providers/` | The benchmark picks - the five curated Bischoff scenarios, the cube and specialized scaling baselines, the core count |

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
