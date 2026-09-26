---
description: Step 18 - the lib results files and README, every table shape locked with the maintainer 2026-09-25 to 2026-09-26; what to build, from which raw files, in what order, and the history the story can draw on
state: ready
waits-on: "a session of its own - the maintainer says when. horizon was set by an agent, strike it"
horizon: undecided
paths: ["lib/results/**", ".agents/scripts/derive-lib-results.py"]
---

# Step 18 - the lib results files

Shape: the general design record, the decision on measured numbers. Protocol, and the rules every results file
keeps: the orchestrator, "The results files". This file says only what is particular to lib.

## How the maintainer wants it worked

- One table per turn: the pick, why, the trade-off, where it would be wrong - shown with example numbers,
  even made-up ones. Plain short English. No menus.
- Every number computed by script from the raw files. A session before this one printed three wrong numbers
  typed by hand.
- The shapes below are locked. Build them; argue with one only with evidence from the data, and stop to ask.

## The raw files

| File | Holds |
|---|---|
| `lib/results/measurements/packing-efficiency.md` | per problem: Types, Items, Ceiling % (the items' volume over the bin's), fill for FFD, WFD, BFD (v2), Best, Margin |
| `lib/results/measurements/version-parity.md` | the problems where v1 and v2 pack differently |
| `lib/results/benchmarks/baseline/algorithms/Full_<alg>_<Packing|Fitting>.md` | v1 and v2 time and Allocated on all 700, short job |
| `lib/results/benchmarks/baseline/result-selection/*.md` | the three selectors, v1 against v2 |
| `lib/results/benchmarks/baseline/scaling/Sample_Packing.md` | the item ladder, 3 to 79 items, default job |

## The root files

Status: **exists** is in the tree and accepted; **add** is agreed, not built; **new** is a file that does not
exist yet.

### `packing-efficiency-stats.md` - exists, add one table

How full each algorithm packs, per Bischoff set. Keeps its mean-fill table and one spread table per algorithm
(rows thpack1..7 then All 700; Set, Item types, Min, Mean, Median, Max).

**Add - headroom**, after the existing tables: rows thpack1..7 then All 700; columns Set, Item types, Ceiling,
Best of three, Left (points). Mean ceiling is 99.45 and best of three 81.33, so about 18 points are left. Under
it, a question: the ceiling is the items' volume, not the best packing possible, which nobody knows - so
"Left" is not reachable points. Is fill the next thing to improve, and how would we know how much is
reachable? Published results on the same problems would say; this plan leaves them out.

### `algorithm-performance.md` - exists, add three tables

Fill gained and time paid by BFD and WFD against FFD, per set, then one time spread table each.

**Add - what racing buys.** It serves a function the maintainer will write: an automatic pick of the best
balance of fill and cost for everyday use, with the caller keeping the choice. So it lays out every option
with its fill and cost and argues for none.

1. All 700, one row per option: FFD, WFD, BFD, Race FFD+BFD (loop), Race all three (loop). Columns Option,
   Mean fill, Best or tied on (of 700), Time as × FFD. A race's fill is the best of its members per problem;
   "best or tied" is fill equal to the best of all three; a loop race's time is the sum of its members per
   problem, divided by FFD on that problem, then averaged. One line under it: parallel lowers the race rows,
   see `parallel-racing.md`.
2. Fill per set: rows thpack1..7 then All 700; columns Set, Item types, FFD, WFD, BFD, Race FFD+BFD, Race all
   three.
3. Time per set as × FFD: the same rows; columns Set, Item types, WFD, BFD, Race FFD+BFD, Race all three.

Checked 2026-09-26: BFD alone 81.261 mean, best of FFD+BFD 81.297, best of three 81.33; FFD beats BFD on 23 of
700.

### `version-differences-packing.md`, `version-differences-fitting.md` - exist, no change

They show the direction is sound.

### `result-selection.md` - exists, rewritten in the script, not run

The stop sign: picking a result is not worth optimizing. One table, one row per selector (Best bin, Smallest
bin, Best algorithm): Scenarios, Slowest v2 pick, Share of the fastest pack (a percentage), Time v2 against v1,
Memory v2 against v1 - the last two BenchmarkDotNet's own ratios, averaged. The fastest pack is the fastest v2
pack of any algorithm over the 700 (FFD on thpack1_72, 7.0 μs). The maintainer accepted it printed on
2026-09-26, with one caveat noted: Best algorithm's 1.14× averages 1.51×, 1.68× and 0.24×. The file in the tree
is still the old three-table version.

### `scaling.md` - new, three tables

How time grows with size - an input to the cost function.

1. v2 packing time by item-count band over the 700: columns Items, Problems, FFD median, WFD median, BFD median,
   in μs, this machine only. Medians, because times inside a band vary a lot. The script picks band edges that
   keep enough problems in each; items run 69 to 476, median 131.
2. "Spread inside each band": BFD v2 only, the same bands; columns Items, Problems, Fastest, Median, Slowest,
   Slowest as × fastest. It shows size alone cannot predict time; a question under it asks what does (th4_84
   has 125 items and a long job). If FFD or WFD spread differently, the script says so and they get tables too.
3. The small end, from the ladder run: rows 3, 13, 29, 47, 67 items; columns Items, Item types, FFD, WFD, BFD
   (v2, μs). 79 is left out with a note - FFD and WFD no longer fit every item there and stop early. The file
   says each ladder step adds items and an item type at once, so the ladder cannot separate the two.

The scaling bench itself is sound (default job, tight error); it just cannot answer the cost question alone.

### `parallel-racing.md`, `parallel-bins.md` - the maintainer's

The tipping points, from steps 19 and 20. He is making them.

### `README.md` - last

Written once every root file above exists, the two parallel files included. A short summary of all, then the
combinations, then open questions and gaps, then the index (which is what the tree holds today). An example
shown 2026-09-26, wording to be revised: "Direction: v2 is sound" (version files, packing-efficiency-stats),
"What to run by default" (algorithm-performance, parallel-racing), "What a cost function can use" (scaling,
parallel-racing, parallel-bins), "What not to chase" (result-selection), then open questions (fill headroom,
what predicts time).

## Build order

1. **Change the script to write tables only.** `.agents/scripts/derive-lib-results.py` writes whole files today,
   sentences included, and the words are now written by hand. How it refreshes a table without touching the
   words is open - settle it with the maintainer first. Then add the tables above.
2. **The maintainer runs it**, never during a bench run. Then write the words for each changed or new file from
   what its tables show: few words, the gaps and questions at the end.
3. **The README**, once steps 19 and 20 have given the two parallel files.

## Facts the words can use

- **Why v2 allocates 0.08× of v1 (BFD) and 0.05× (WFD).** v1 BFD and WFD pick a space with
  `availableSpace.OrderBy(...)`, which builds a new sorted copy of the free-space list for every item and every
  orientation; v2 sorts the list in place. FFD never sorted spaces, so it only drops to 0.38×. "Allocated" is
  memory handed out during one pack, garbage included - not peak memory.
- **Say how a number was made when it is not obvious**: StdDev over all 700 is population.
- **The dropped proposal:** one table of v2 packing times per algorithm (fastest, mean, median, slowest). The
  spread was too wide, 7 to 725 μs. `scaling.md` shows that spread on purpose, as a finding.

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
so a single problem's mean is rough; an average over 700 is not. Beside it, `baseline/threshold/Full_*` holds
the parallel numbers, already read out as F4 and F2a in the lib findings record - the story does not need to
re-derive them.

## Not in this plan

- A docs-site page copied from the READMEs, and a number on www.
- Comparison against published results on the Bischoff problems: "we fill 81% at X ns, others 87% at Y ns".
  The fill half holds anywhere; the time half only if both ran on one machine.

## Done when

- [ ] `lib/results/` holds every root file listed above with its locked tables, and its `README.md` opens
      with the summary, then the combinations, then the open questions, then the index.
      **By eye.** Every table is generated; every number in the words is in a table.
- [ ] The history above is either used in the story or dropped with a reason.
      **By eye.**
