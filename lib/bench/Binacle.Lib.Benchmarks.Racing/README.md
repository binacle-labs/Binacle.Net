# Binacle.Lib.Benchmarks.Racing

When `Best` races several algorithms on one bin, from what size is running them in parallel faster than one
after the other? One class, `Cores_Packing`: the two sets production races (`FFD,BFD` on the multi-bin routes,
`FFD,WFD,BFD` on the single-bin ones), Loop against Parallel, plus FFD, WFD and BFD each alone, v2, over 30
Bischoff problems from the smallest job to the largest, on 2, 4, 8 and 12 cores.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Cores_Packing.cs` | The class that runs. Seven rows in three blocks: each race with its Loop as the baseline, then the three algorithms alone |
| `CoreJobs.cs` | One job per core count, each pinned to the first N CPUs with `DOTNET_PROCESSOR_COUNT` set to N, and the `Cores` column |
| `CorePinning.cs` | Run first in each case: pins every thread to the job's CPUs, and fails the case if any thread or the CPU count is off |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |

## 🛠️ How you use it

```
just bench lib-racing-cores           # 840 cases at the short job, about 70 minutes; asks first
just bench lib-racing-cores precise   # the default job, about 3.5 hours: the one to keep
```

The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

Read a Ratio only against the Loop in its own block, problem and core count; that is the Loop it was taken
against. The alone rows show `?`: add them up and compare the sum with the Loop. The header still says 12
CPUs; the `Cores` column is the real count.

The pinning is Linux only. A pinned run is kinder than a real small machine: the OS and the BenchmarkDotNet
host run on the spare CPUs.

The findings here rest on ratios near 1, which the `short` job blurs - keep a `precise` run, not a short one.
