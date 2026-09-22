# Binacle.Lib.Benchmarks.ResultSelection

Times the three result selectors, v1 against v2, on the scenarios in
[`lib/data/result-selection/`](../../data/result-selection). v1 is LINQ - a filter, a sort, and a second pass
when nothing packed fully; v2 is one loop. `Ratio` and `Allocated` are what to read: a v2 that grows an
iterator or a second pass back shows there first.

## 📂 What is in it

| Path | What it is |
|---|---|
| `BestAlgorithm.cs`, `BestBin.cs`, `SmallestBin.cs` | One class per selector; the scenario is the column, v1 and v2 the rows |
| `BenchmarkBase.cs` | Loads the named scenario from its own set before the run |
| `Program.cs` | Runs the classes with the config from `shared/test/Binacle.Benchmarking`, and fails when nothing ran |

## 🛠️ How you use it

```
just bench lib-result-selection
```

22 cases, about two minutes at the `short` job - the methods are sub-microsecond, so a longer job buys
nothing. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

The unit tests read the same JSON files, so a scenario added or renamed here is tested there too. Scenario
names repeat across the three sets; each class resolves its own.
