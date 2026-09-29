# Binacle.Lib.Benchmarks.ParallelAlgorithms

When `Best` races several algorithms on one bin, from what size is running them in parallel faster than one
after the other, and on how many cores? Every class runs the two sets production races (`FFD,BFD` on the
multi-bin routes, `FFD,WFD,BFD` on the single-bin ones), Loop against Parallel, v2, on each core count in
`CoreJobs`.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Smoke_Packing.cs` | A few steps of the synthetic ladder in `lib/test/Binacle.Lib.Testing/LadderGenerator.cs`: did my change help or hurt |
| `Sample_Packing.cs` | The whole ladder. It reaches jobs smaller than the smallest Bischoff pick |
| `Full_Packing.cs` | Bischoff problems picked from the smallest job to the largest. Rows in three blocks: each race with its Loop as the baseline, then FFD, WFD and BFD each alone |
| `LadderBase.cs` | The two ladder classes' rows: builds both processors, pins the case, loads the ladder step |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |

The pinning itself, `CoreJobs` and `CorePinning`, lives in `shared/test/Binacle.Benchmarking`.

## 🛠️ How you use it

```
just bench lib-parallel-algorithms-smoke            # the short job
just bench lib-parallel-algorithms-sample quick     # the default job unless `quick`
just bench lib-parallel-algorithms-full precise     # the short job unless `precise`: the one to keep; asks first
```

`just bench` lists each with its cost. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

Read a Ratio only against the Loop in its own set, size and core count; that is the Loop it was taken
against. The alone rows in `Full_Packing` show `?`: add them up and compare the sum with the Loop. The header
still shows every CPU the machine has; the `Cores` column is the real count.

The pinning is Linux only, and a class run outside `Program` fails at setup. A pinned run is kinder than a real
small machine: the OS and the BenchmarkDotNet host run on the spare CPUs.

The findings rest on ratios near 1, which the `short` job blurs - keep a `precise` run of `Full_Packing`, not a
short one.
