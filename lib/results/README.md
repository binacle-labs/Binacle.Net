# Lib results

What the lib's measure and bench projects found.

<!-- Summary: shape not decided yet. -->

## 📂 What is in it

| Path | What it is |
|---|---|
| [measurements/](measurements) | Raw measurement results, written by `just measure lib`: fill per algorithm on every scenario, and where v1 and v2 pack differently |
| [benchmarks/](benchmarks) | Raw benchmark results: BenchmarkDotNet reports from `lib/bench`, copied by hand. Not written by `just measure` |
| [packing-efficiency-stats.md](packing-efficiency-stats.md) | How full each algorithm packs, per Bischoff set |
| [algorithm-performance.md](algorithm-performance.md) | BFD, WFD and the loop races against FFD: packing time |
| [version-differences-packing.md](version-differences-packing.md) | v2 against v1 when packing: time and memory |
| [version-differences-fitting.md](version-differences-fitting.md) | v2 against v1 when fitting: time and memory |
| [result-selection.md](result-selection.md) | What picking the result costs, v2 against v1 |
| [packing-time-by-size.md](packing-time-by-size.md) | How packing time grows with item count, over the 700 Bischoff problems |
| [scaling.md](scaling.md) | Packing time at the small end, 3 to 67 items |

Each file at this level answers one question from the two folders.

## 🛠️ How you use it

```
just measure lib      # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```
