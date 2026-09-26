# Lib results

What the lib's measure and bench projects found, as raw files and as the story read out of them.

## 📂 What is in it

| Path | What it is |
|---|---|
| [measurements/](measurements) | Raw measurement results, written by `just measure lib`: fill per algorithm on every scenario, and where v1 and v2 pack differently |
| [benchmarks/](benchmarks) | Raw benchmark results: BenchmarkDotNet reports from `lib/bench`, copied by hand. Not written by `just measure` |
| [packing-efficiency-stats.md](packing-efficiency-stats.md) | How full each algorithm packs, per Bischoff set |
| [algorithm-performance.md](algorithm-performance.md) | BFD and WFD against FFD: fill gained, time paid |
| [version-differences-packing.md](version-differences-packing.md) | v2 against v1 when packing: time and memory |
| [version-differences-fitting.md](version-differences-fitting.md) | v2 against v1 when fitting: time and memory |
| [result-selection.md](result-selection.md) | What picking the result costs next to a pack, and why it is not worth optimizing |

The files at this level are derived from the two folders. Each tells one part of the story with numbers, and is
rewritten when the raw files move. Do not edit them by hand.

## 🛠️ How you use it

```
just measure lib      # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```
