# Packing Efficiency Results

How well each algorithm fills a bin, and how long it took, as the old performance-tests project wrote them.
Nothing writes here any more: fill is now `lib/results/packing-efficiency.md`, written by `just measure lib`.
These stay only until the timing rows have a home under `lib/results/benchmarks/`.

| File | What it shows |
|---|---|
| `PackingEfficiency.md` | Volume used per algorithm - min, mean, median, max, as a percentage |
| `PackingEfficiencyComparison.md` | The same measure, one row per scenario, one column per algorithm |
| `PackingTime.md` | Time per algorithm - min, mean, median, max, in microseconds |
| `2024-11-24.md` ... `2025-02-10.md` | Dated snapshots. A suffix names the algorithm that changed (`2024-11-27_WFD.md`, `2024-12-09_BFD.md`). |

Efficiency is stable across machines; the times in `PackingTime.md` are not. Compare times only against a run
on the same hardware.
