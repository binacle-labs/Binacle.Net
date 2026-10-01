# Binacle.Lib.Benchmarks.ParallelAlgorithms

When `Best` races several algorithms on one bin, from what size is running them in parallel faster than one
after the other, and on how many cores? One class, on one bin and one item, both the same as in
`Binacle.Lib.Benchmarks.ParallelBins`. It runs the two sets production races (`FFD,BFD` on the multi-bin routes,
`FFD,WFD,BFD` on the single-bin ones), Loop against Parallel, v2, on each core count in `JobsByCoreCount`.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Identical_Packing.cs` | Every item the same and every item fits, over piece counts. Rows in three blocks: each race with its Loop as the baseline, then FFD, WFD and BFD each alone. Setup throws if an item does not fit |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |

The bin and the item live in `IdenticalCase` in `lib/test/Binacle.Lib.Testing`. The pinning itself,
`JobsByCoreCount`, lives in `shared/test/Binacle.Benchmarking`.

## 🛠️ How you use it

```
just bench lib-parallel-algorithms-identical dry    # each case once: checks the sizes
just bench lib-parallel-algorithms-identical        # the short job
```

`just bench` lists it with its cost. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

Read a Ratio only against the Loop in its own set, size and core count; that is the Loop it was taken
against. The alone rows show `?`: add them up and compare the sum with the Loop. The header still shows every CPU
the machine has; the `Cores` column is the real count.

The pinning is Linux only, and a class run outside `Program` fails at setup. A pinned run is kinder than a real
small machine: the OS and the BenchmarkDotNet host run on the spare CPUs.

The `short` job blurs ratios near 1.
