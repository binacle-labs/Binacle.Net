# Binacle.Lib.Benchmarks.Threshold

Should the parallel processors be wired up, and from what size? Production runs `Loop` everywhere; this is
the evidence. Two families, both rows `Loop` (baseline) and `Parallel`, on the synthetic ladder in
`lib/test/Binacle.Lib.Testing/Providers/SpecializedScalingProblemsProvider.cs`:

- **Algorithms** - one bin, the two sets production races (`FFD,BFD`, `FFD,WFD,BFD`), the item ladder.
- **Bins** - one algorithm (FFD or BFD, the multi-bin routes never run WFD), bins 1 to 7, the item ladder.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Smoke_Algorithms_Packing.cs`, `Smoke_Bins_Packing.cs` | v2 only; items 3, 47, 67, 79; bins 2, 3, 7 |
| `Sample_Bins_Packing.cs` | v2; items 3, 47, 79; every bin count |
| `Algorithms_Packing_v1.cs`, `_v2.cs`, `Bins_Packing_v1.cs`, `_v2.cs` | The whole ladder, bins 1 to 7 |
| `AlgorithmsBase.cs`, `BinsBase.cs` | One per family: builds both processors, loads the ladder step, holds the two rows |
| `Program.cs` | The BenchmarkDotNet switcher with the config from `shared/test/Binacle.Benchmarking` |

## 🛠️ How you use it

```
just bench lib-threshold-smoke     # 64 cases, about 5 minutes: did my change help or hurt
just bench lib-threshold           # the sample: 128 cases, about 11 minutes
just bench lib-threshold-full      # 704 cases, about 3 hours; job="short" is about 1
```

The sample tier is `Sample_Bins_Packing` plus the v2 algorithms class, which is in both `sample` and `full`;
the report header says which job ran. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

`Ratio` for Loop against Parallel only compares on the same core count; the header prints it. 1 bin is the
guard that wiring parallel up must not hurt a one-bin request, not a point on the curve. The ladder's steps
are not evenly spaced on purpose: 47 fits every bin, 59 overflows the smallest, 67 overflows bins 1-4, 79
fails the largest for all but BFD.
