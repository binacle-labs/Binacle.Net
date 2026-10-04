# Binacle.Lib.Benchmarks.ParallelAlgorithms

When `Best` races several algorithms on one bin, from what size is running them in parallel faster than one
after the other, and on how many cores? One class per race that production runs, plus one per algorithm on its
own, all on the same bin and item as `Binacle.Lib.Benchmarks.ParallelBins`. Loop against Parallel, v2, on each
core count in `JobsByCoreCount`.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Identical/IdenticalBase.cs` | The `Loop` and `Parallel` rows, the piece sweep, and the setup they all share. Setup throws if an item does not fit |
| `Identical/Identical_FFD_Packing.cs`, `..._WFD_...`, `..._BFD_...` | One algorithm each, racing nothing |
| `Identical/Identical_FFD_BFD_Packing.cs` | The race the multi-bin routes run |
| `Identical/Identical_FFD_WFD_BFD_Packing.cs` | The race the single-bin routes run |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |

Every class sweeps the same piece counts, so the reports can be read side by side. The grid lives on the
base for that reason.

The bin and the item live in `IdenticalCase` in `lib/test/Binacle.Lib.Testing`. The pinning itself,
`JobsByCoreCount`, lives in `shared/test/Binacle.Benchmarking`.

## 🛠️ How you use it

```
just bench lib-parallel-algorithms-identical dry    # each case once: checks the sizes
just bench lib-parallel-algorithms-identical        # the short job
```

`just bench` lists it with its cost. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

**Run them together, and read the sum check as a ratio.** `Loop` on a race class comes out at about 1.2
times the sum of the alone classes at the same piece count - not 1.0. The allocation is additive, so it is the
same work: an alone class's process constructs one algorithm and its call sites get devirtualized, which the
race's cannot. **About 1.2 is normal; 1.5 or more is a slow process**, and that is how a clean run is told from
a dirty one. It only holds while they all come from one invocation.

**A one-algorithm class's `Parallel` row is not a race.** The default cap resolves to 1, so `Parallel.For` runs
the body inline and nothing is handed to another CPU: the row is what `Parallel.For` costs to set up.
`Binacle.Lib.Benchmarks.ParallelOverhead` measures that straight. **Read it at 1 to 16 pieces**, where it is a
flat 243 to 267 ns above `Loop` on every core count; at 256 pieces the same step is a few hundred ns on a
15 to 100 us measurement and drowns.

Read a Ratio only against the Loop in its own class, size and core count. The header still shows every CPU the
machine has; the `Cores` column is the real count.

The pinning is Linux only, and a class run outside `Program` fails at setup. A pinned run is kinder than a real
small machine: the OS and the BenchmarkDotNet host run on the spare CPUs.

The `short` job blurs ratios near 1.
