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
| [packing-efficiency.md](packing-efficiency.md) | How full does each algorithm pack, alone and combined, and where is BFD not the fullest? |
| [version-differences.md](version-differences.md) | How does v2 differ from v1 in fill, time and memory, packing and fitting? |
| [algorithm-cost.md](algorithm-cost.md) | What do WFD and BFD cost against FFD in time and memory, and what does running more than one cost for the fill it adds? |
| [packing-time-by-size.md](packing-time-by-size.md) | How does packing time grow with the number of items and item types? |
| [parallel-algorithms.md](parallel-algorithms.md) | When several algorithms run on one bin, is parallel faster than one after the other, from what size and on how many cores? |
| [parallel-bins.md](parallel-bins.md) | When one algorithm packs several bins, is parallel faster than one after the other, from what size and on how many cores? |
| [result-selection.md](result-selection.md) | What does picking the result cost, v2 against v1? |
