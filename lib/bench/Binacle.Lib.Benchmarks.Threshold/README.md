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
| `Sample_Algorithms_Packing.cs`, `Sample_Bins_Packing.cs` | v2; the whole item ladder; items 3, 47, 79 at every bin count |
| `Full_Algorithms_Packing_v1.cs`, `_v2.cs`, `Full_Bins_Packing_v1.cs`, `_v2.cs` | v1 and v2; the whole ladder, bins 1 to 7 |
| `AlgorithmsBase.cs`, `BinsBase.cs` | One per family: builds both processors, loads the ladder step, holds the two rows |
| `Program.cs` | Runs the classes with the config from `shared/test/Binacle.Benchmarking`, and fails when nothing ran |

## 🛠️ How you use it

```
just bench lib-threshold-smoke     # 64 cases, about 8 minutes: did my change help or hurt
just bench lib-threshold-sample    # 128 cases at the default job; `quick` is about 11 minutes
just bench lib-threshold-full      # 704 cases at the short job, about 1 hour; `precise` is about 3
```

Full asks before it starts. The report header says which job ran. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

`Ratio` for Loop against Parallel only compares on the same core count; the header prints it. 1 bin is the
guard that wiring parallel up must not hurt a one-bin request, not a point on the curve. The ladder's steps
are not evenly spaced on purpose: 47 fits every bin, 59 overflows the smallest, 67 overflows bins 1-4, 79
fails the largest for all but BFD.
