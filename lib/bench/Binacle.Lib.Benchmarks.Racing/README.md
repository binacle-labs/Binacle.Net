# Binacle.Lib.Benchmarks.Racing

When `Best` races several algorithms on one bin, is running them in parallel faster than one after the
other? Rows `Loop` (baseline) and `Parallel`, one class per algorithm factory version, over the two sets
production races (`FFD,BFD` on the multi-bin routes, `FFD,WFD,BFD` on the single-bin ones) and the five
curated Bischoff problems.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Packing_v1.cs`, `Packing_v2.cs` | One class per factory version; the columns are the scenario and the set |
| `BenchmarkBase.cs` | Builds both processors for the set, loads the scenario, and holds the two rows |
| `Program.cs` | The BenchmarkDotNet switcher with the config from `shared/test/Binacle.Benchmarking` |

## 🛠️ How you use it

```
just bench lib-racing-smoke     # 40 cases at the short job, about 2 minutes: did my change help or hurt
just bench lib-racing           # the same 40 at the default job, about 10 minutes: the one to keep
```

Both tiers run the same classes; only the job differs. The report lands in `BenchmarkDotNet.Artifacts/results/`,
gitignored.

## ⚠️ What will bite you

The findings here rest on ratios like 1.08, which the `short` job blurs - keep a default-job run, not a smoke
one. `Ratio` for Loop against Parallel only compares on the same core count; the header prints it.
`packing` is the one narrowing word that applies; the algorithm words select nothing here, since the
algorithms are a set, not a row.
