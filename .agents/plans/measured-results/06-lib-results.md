---
description: Session 6 - decide how the tables get their numbers, then fill the seven lib results files and the two parallel files from the kept runs; the table shapes are in the files, as comments over fake sample tables
state: blocked
waits-on: "sessions 3 to 5 - the reruns and the two drop points"
horizon: undecided
paths: ["lib/results/**"]
---

# 6 - The lib results files

## Where it stands

The seven files in `lib/results/` are placeholders. Above each table a comment is its spec - what it shows, the
raw file it reads, its rows, its columns, how a number is made. Under it, a sample table with fake numbers shows
the shape. The maintainer reviews the shapes before any number goes in.

`parallel-racing.md` and `parallel-bins.md` do not exist yet. The maintainer shapes them from the runs of
sessions 4 and 5; then they are built like the rest.

## First, the maintainer's call: how the numbers get in

Every number comes from the raw files by a program, never typed - a session once printed three wrong numbers
typed by hand. Where that program lives and how it runs is open. A script refreshed the tables between markers
until 2026-09-26; it was removed before anyone reviewed it. Put the choice to him with an
example, then build it.

**What the files are for:** the maintainer will write a function that picks the best balance of fill and cost
for everyday use, with the caller keeping the choice. `packing-efficiency-stats.md` gives the fill half,
`algorithm-performance.md` the time half; the README puts them side by side. Neither argues for dropping an
option.

## The raw files, by category

The inputs - `shared/data/bischoff-suite/orlib_thpack<N>.json`,
one entry per problem with `Items` (one line per item type, `[n]` its count) and `Metrics` (third field is the
item count) - are not results, and any file may read them.

| Category | Raw files | Root files |
|---|---|---|
| fill | `lib/results/measurements/packing-efficiency.md` (per problem: Types, Items, Ceiling %, fill for FFD, WFD, BFD, Best, Margin), `version-parity.md` | `packing-efficiency-stats.md` |
| algorithm time | `lib/results/benchmarks/baseline/algorithms/Full_<alg>_<Packing|Fitting>.md` (v1 and v2 time and Allocated on all 700, short job) | `algorithm-performance.md`, `version-differences-packing.md`, `version-differences-fitting.md`, `packing-time-by-size.md` |
| result selection | `lib/results/benchmarks/baseline/result-selection/*.md` | `result-selection.md` |
| ladder | `lib/results/benchmarks/baseline/scaling/Sample_Packing.md` (3 to 79 items, default job) | `scaling.md` |
| racing, threshold | the kept runs of sessions 4 and 5 | `parallel-racing.md`, `parallel-bins.md` - the maintainer's |

## Open points in the shapes, found 2026-09-26

Each is the maintainer's to settle, one per turn.

- **Bold in `algorithm-performance.md`.** Every value there is × FFD and above 1.00×. Bold every cell, or none?
- **Its memory sentence** has numbers no table holds, against "every number in the words is in a table".
- **It divides numbers from three reports**, run hours apart. That is weaker than a ratio taken inside one class.
- **The version files** have no loss count, though a per-problem maximum above 1.00× shows single losses happen.
- **`result-selection.md`** has no loss count, though its averages mix scenarios on both sides of 1.00×.
- **`version-parity.md`** is a raw file of the fill kind, but no table reads it. It stays as a correctness check.
- **Headroom:** Ceiling and Best of three per set are taken as means; the shape did not say.
- **"Best or tied"** is counted from the Best column, which holds exact values, not from the rounded fills.
- **`packing-efficiency.md` never says its fills are v2**; only the code does (`Algorithms.Shipped = 2`).
- **`scaling.md`** keeps 5 of the ladder's 11 steps (3, 13, 29, 47, 67) and gives no reason for dropping 7, 17, 23,
  37 and 59.
- **`packing-time-by-size.md`:** "do FFD and WFD spread differently inside a band" has no test yet.
- **The time files name no gaps yet.**
- **Unused raw columns:** Items and Margin in `packing-efficiency.md`, and the v1 rows of the ladder. They cost
  nothing and stay unless he says.

## Facts the words can use

- **Why v2 allocates 0.08× of v1 (BFD) and 0.05× (WFD).** v1 BFD and WFD pick a space with
  `availableSpace.OrderBy(...)`, which builds a new sorted copy of the free-space list for every item and every
  orientation; v2 sorts the list in place. FFD never sorted spaces, so it only drops to 0.38×. "Allocated" is
  memory handed out during one pack, garbage included - not peak memory.
- **Say how a number was made when it is not obvious**: StdDev over all 700 is population.
- **The dropped proposal:** one table of v2 packing times per algorithm (fastest, mean, median, slowest). The
  spread was too wide, 7 to 725 μs. `packing-time-by-size.md` shows that spread on purpose, as a finding.

## What the removed lib README said (2026-09-22)

**lib/results** - 700 Bischoff problems (thpack1..7), fill %, v2:

| Algorithm | Min | Mean | Median | Max |
|---|---|---|---|---|
| FFD | 56.18 | 73.41 | 73.47 | 87.90 |
| WFD | 49.15 | 69.21 | 69.05 | 87.90 |
| BFD | 62.08 | 81.26 | 81.48 | 90.66 |

- Best of FFD and BFD (what the API races): mean 81.30. Best of all three: 81.33. WFD adds 0.03 points.
- Best or tied: BFD 669 of 700, FFD 35, WFD 35.
- BFD leads in every set, BR1 (3 types) to BR7 (20 types); FFD and WFD lose more as item types grow
  (WFD 74.78 at BR1, 67.41 at BR7).
- v1 and v2 pack the same on 2,099 of 2,100 algorithm-problem pairs. The one: BFD on thpack7_45, 79.08 -> 79.73.

## History - what the old `results/` folder proved

Removed from the tree 2026-09-22. The files are in git before that date. Every number below was read out of
them by script.

**v2 against v1, packing, one bin 60x40x10 filled with 5x5x5 cubes.** The Ratio column is v2's time as a
share of v1's in the same run. Memory is identical on both machines at the same runtime.

| Alg | Items | i5 .NET 9 | i5 .NET 10 | i7 .NET 9 | i7 .NET 10 | Memory v1 -> v2 (.NET 10) |
|---|---|---|---|---|---|---|
| FFD | 10 | 0.68 | 0.70 | 0.65 | 0.68 | 6.46 -> 3.85 KB |
| FFD | 192 | 0.65 | 0.81 | 0.64 | 0.69 | 83.21 -> 50.07 KB |
| WFD | 10 | 0.67 | 0.72 | 0.60 | 0.67 | 11.15 -> 4.72 KB |
| WFD | 192 | 0.71 | 1.03 | 0.66 | 0.72 | 285.57 -> 51.24 KB |
| BFD | 10 | 0.46 | 0.47 | 0.50 | 0.50 | 8.40 -> 3.31 KB |
| BFD | 192 | 0.45 | 0.45 | 0.43 | 0.48 | 130.96 -> 49.67 KB |

i5 is an i5-4570 on Debian 12; i7 an i7-14700 on Windows 11; runs of November 2025. What holds:

- BFD v2 takes under half the time of v1, with a third of the memory.
- FFD v2 takes about a third less time, with 40% less memory.
- WFD v2 uses a sixth of the memory at 192 items. Its time gain is not steady: 1.03 in one run.

**Other things that hold:**
- FFD v2 memory at 192 cubes held for a year: 50.38-50.41 KB from 2024-11 to 2025-11.
- Fill has not moved since 2024-11: the same FFD 73.41, WFD 69.21, BFD 81.26 as today.
- .NET 8 to .NET 9 in one run (2024-11-15, 2024-11-22): FFD v2 packing 0.85-0.92x the time.
- Cost against size, in one run: FFD and BFD grow about in line with item count (13-17x the time for 19x the
  items); WFD much faster (42-65x). Time grows in line with bin count.
- Parallel bins against a loop, in one run: 0.63-0.76x at 8-38 bins on the 2-core i5, 0.20-0.49x on the
  20-core i7; 1.3-2.6x slower at 2 bins with one item type. A hint for the bin-threshold question, not an
  answer - the code has changed since.

**What did not hold:**
- "N% faster from 2024 to 2025". The OS, runtime, benchmark tool and scenario all changed. The same code
  also drifts between runs: FFD v2 at 192 cubes ranged 171-197 us on one fixed setup.

**A way to make the speed story current:** the smoke case `full bin, one type` is the old 192-cube row. The
2026-09-23 `lib-algorithms-smoke` run, kept in `lib/results/benchmarks/baseline/algorithms/`, gives the v2/v1
ratios and memory on .NET 10 to set beside the table above, and shows whether memory moved since November 2025.

**A better source landed 2026-09-25.** The `lib-algorithms-full` run - 1,400 cases per class, all 700 Bischoff
problems, v1 against v2, packing and fitting - is in the same folder as `Full_<alg>_<op>.md`. It carries the
v2/v1 story over the whole suite instead of one synthetic case, so the story's speed numbers come from there
and the smoke case is only the bridge back to the November table. It ran at the **short** job (3 iterations),
so a single problem's mean is rough; an average over 700 is not. Its BFD times hold slow processes and are
retaken in session 3. Beside it, `baseline/threshold/Full_*` holds
the parallel numbers, already read out as F4 and F2a in the lib findings record - the story does not need to
re-derive them.

## Not in this plan

- A docs-site page copied from the READMEs, and a number on www.
- Comparison against published results on the Bischoff problems: "we fill 81% at X ns, others 87% at Y ns".
  The fill half holds anywhere; the time half only if both ran on one machine.

## Done when

- [ ] The maintainer chose how the numbers get in, written here, and it is built.
      **By eye.**
- [ ] Every lib results file has real numbers, and its words and gaps are written from them.
      `! grep -l "is fake" lib/results/*.md`, then **by eye** - every number in the words is in a table.
- [ ] `parallel-racing.md` and `parallel-bins.md` exist in the shape the maintainer gave.
      `ls lib/results/parallel-racing.md lib/results/parallel-bins.md`
- [ ] The history above is used in the words or dropped with a reason.
      **By eye.**
