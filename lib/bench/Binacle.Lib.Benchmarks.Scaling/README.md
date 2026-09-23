# Binacle.Lib.Benchmarks.Scaling

How does packing time grow as the order grows? One class, six rows - FFD, WFD and BFD in v1 and v2 - run over
the 11-step item ladder in `SpecializedScalingProblemsProvider`, from 3 items to 79. The bin is the largest one
in that provider and never changes, so the item count is the only thing moving.

Read it down the `Items` column: that is the curve. `Ratio` reads across, against FFD v1 at the same item count.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Sample_Packing.cs` | The whole benchmark: the ladder as a parameter, and one row per algorithm and version |
| `Program.cs` | Runs the class with the config from `shared/test/Binacle.Benchmarking`, and fails when nothing ran |

## 🛠️ How you use it

```
just bench lib-scaling          # 66 cases at the default job
just bench lib-scaling quick    # the same at the short job
```

The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

At 79 items everything but BFD fails to pack them all. The timing is still real - a failed pack is work the
algorithm did - but it is not the same work, so do not read the last step as more of the same curve.
