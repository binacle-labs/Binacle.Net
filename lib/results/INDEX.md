# Lib results index

Every file in this folder, and what it holds.

## 📂 Raw results

| Path | What it is |
|---|---|
| [measurements/](measurements) | Fill per algorithm on every scenario, and where v1 and v2 pack differently. Written by `just measure lib` |
| [benchmarks/](benchmarks) | BenchmarkDotNet reports from `lib/bench`, copied by hand |

## 📄 One question each

| File | The question |
|---|---|
| [packing-efficiency-stats.md](packing-efficiency-stats.md) | How full does each algorithm pack, per Bischoff set? |
| [algorithm-performance.md](algorithm-performance.md) | How do BFD, WFD and the loop races compare against FFD on packing time? |
| [version-differences-packing.md](version-differences-packing.md) | What did v2 change against v1 when packing, in time and memory? |
| [version-differences-fitting.md](version-differences-fitting.md) | What did v2 change against v1 when fitting, in time and memory? |
| [result-selection.md](result-selection.md) | What does picking the result cost, v2 against v1? |
| [packing-time-by-size.md](packing-time-by-size.md) | How does packing time grow with item count? |
| [scaling.md](scaling.md) | How does packing time behave at the small end? |

Each one reads the raw results above. Nothing at this level is generated.
