---
description: Session 5 - find where packing many bins at the same time starts to pay. All pending - bin-set categories from even to real, an item sweep, and a run in stages, each a suggestion
state: blocked
waits-on: "sessions 3 and 4 - the bench structure, and the racing run and what it teaches"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelBins/**", "lib/test/Binacle.Lib.Testing/**", "shared/test/Binacle.Benchmarking/**", "tooling/bench.just"]
---

# 5 - The bins drop point

## Goal

**(the maintainer, 2026-09-29):** measure the bins parallelisation threshold, or its cost function - when
packing many bins in parallel beats packing them one after another. "the other the bins paralilizxation
thresohold of their cost function".

**Decided (the maintainer, 2026-09-25):** "lets make the algo race first".

His worry (2026-09-25): "it does worry me how we can test reliably with different bin shapes when i failed with
the ladder".

**(the maintainer, 2026-09-29):** this session finds the problems the bins bench runs - "step 5 is finding the
problems and keep suggestions will reinvestigate".

## The bench - picked in session 3

This session builds `lib/bench/Binacle.Lib.Benchmarks.ParallelBins/` - "then the 5th with discovery" (the
maintainer, 2026-09-29). Session 3 retired the old bins classes ("3 retire"); their reports left the baseline
(in git at commit 59e9dfb3, `lib/results/benchmarks/baseline/threshold/`). Every class is pinned through `CoreJobs` in
`shared/test/Binacle.Benchmarking` ("yes common code to shared").

`LadderGenerator.GetBins` in `lib/test/Binacle.Lib.Testing/` lost its last caller with the old bins classes.
Session 3 kept it for this one; delete it if this bench does not use it.

**Leaning yes (the maintainer, 2026-09-29):** one class per bin-set category, and one recipe that takes the
category. "i think so". A class each, because BenchmarkDotNet's `--filter` picks classes and methods, not
parameter values; one class could not run one stage alone.

**Suggested (agent, 2026-09-29)**, shown to the maintainer:

```
lib/bench/Binacle.Lib.Benchmarks.ParallelBins/
  Program.cs
  BinSetBase.cs        the Loop and Parallel rows, the item sweep, the bin counts
  Even_Packing.cs, Similar_Packing.cs, Uneven_Packing.cs, SimilarTooSmall_Packing.cs,
  UnevenTooSmall_Packing.cs, NothingFits_Packing.cs, OneHeavy_Packing.cs, Real_Packing.cs
  README.md
```

```just
# Loop against Parallel on one bin-set category, FFD, v2
[arg('category', pattern='even|similar|uneven|similartoosmall|uneventoosmall|nothingfits|oneheavy|real')]
[arg('mode', pattern='|quick')]
lib-parallel-bins category mode='':
    ...
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.ParallelBins -- --job "$job" --filter '*.{{ category }}_*'
```

- The lowercase word assumes `--filter` ignores case; unchecked. If not, the recipe takes `Even`. The
  maintainer, 2026-09-29: "ok dont care".
- `similar` does not catch `SimilarTooSmall_` - the underscore stops it.
- **How the recipe picks the cores is this session's to find.** The maintainer, 2026-09-29: "that is left to the
  session when it makes i dont know yet". The stages below run 2 and 12 first, 4 and 8 only where they disagree.

**Past this section, everything in this file is pending**, kept as suggestions to reinvestigate. The maintainer, 2026-09-29:
"reword the plans as not decided but pending and just as suggestion".

## What the session does - suggested

1. Go through the categories, the item sweep and the stages below with the maintainer. Each is pending.
2. Build `ParallelBins` and its recipe, pinned.
3. The maintainer runs stage A. The session reads it. He says whether the next stage runs.
4. Read the drop point, or say there is none.

`parallel-bins.md` is the maintainer's to shape; session 6 builds it.

## Why the kept bins run cannot answer it

`Full_Bins_Packing_v2` jumps around: FFD wins at 47 items on 2 bins (0.96) and loses at 59 (1.63). Every step
of that grid changed two things at once. One more bin was also a taller bin (60x40x10 up to 60x40x40), and from
59 items the ladder's total no longer fits the small bins. The grid stays as the record.

## What bites

- **Packing never stops early.** It tries every item. A bin too small for the items is packed partly or not at
  all; only fitting exits early. In the fixtures the first word of a result is packing, the second fitting -
  the shared slice doc has the format.
- **The rule cannot see time.** It sees the request. A case has to be described in what the request carries;
  time is only how the case is measured.
- **Real requests mix box sizes**, and some boxes are too small for the items. An all-equal set is the best
  case for parallel, not the usual one.

## The bin-set categories - suggested (agent, 2026-09-29), pending

Each category is one item list against a set of bins. The maintainer named the first four on 2026-09-29; the
agent added the rest. The examples use 32 items, `10x10x10 [16] 20x10x10 [8] 20x20x10 [8]`, volume 64,000.

| # | Name | What it is | Example bins |
|---|---|---|---|
| 1 | Even | One bin, copied. The ideal case, best for parallel. | `80x60x40 [4]` |
| 2 | Similar | Sizes close together, all items fit. | `60x60x40` `70x60x40` `80x60x40` `90x60x40` |
| 3 | SimilarTooSmall | Sizes close, one or more too small for all items. | `40x40x35` `50x40x40` `60x40x40` `70x40x40` |
| 4 | Uneven | Sizes far apart, all items fit. | `50x40x40` `80x60x40` `120x80x60` `200x100x100` |
| 5 | UnevenTooSmall | Sizes far apart, some too small. Closest to a real shop. | `20x20x20` `40x30x30` `80x60x40` `200x100x100` |
| 6 | NothingFits | No item fits any bin. Parallel is only overhead; loop should always win. | `60x60x8` `40x40x5` `100x9x9` `30x30x9` |
| 7 | OneHeavy | One bin does most of the work; parallel waits for it. | `50x40x40 [3]` `200x100x100` |
| 8 | Real | Real box sets and real items, never used to find the drop point. Checks the rule. | from Bischoff or the demo samples |

Categories 1 to 7 can be made up. Category 8 has to be real.

## The item sweep - suggested (agent, 2026-09-29), pending

The same three shapes at 2:1:1; only the count grows. `N` items is
`10x10x10 [N/2] 20x10x10 [N/4] 20x20x10 [N/4]`.

Each case also gets a one-bin row, like the alone rows in racing, so the work per bin is measured in the same
run instead of taken from another report.

For Even, one bin for every step, `160x120x80`, so only the item count changes:

```
items  items (compact)                                fill
   4   10x10x10 [2]   20x10x10 [1]   20x20x10 [1]     <1%
  16   10x10x10 [8]   20x10x10 [4]   20x20x10 [4]      2%
  64   10x10x10 [32]  20x10x10 [16]  20x20x10 [16]     8%
 128   10x10x10 [64]  20x10x10 [32]  20x20x10 [32]    17%
```

The cost: a few items in a near-empty bin is not a real request, and may pack faster than a real one. The Real
set is there to catch that. The other choice, a bin that grows with the items, keeps the fill level but changes
the bin too.

## The stages - suggested (agent, 2026-09-29), pending

All the categories on every setting is 5,120 cases - about 7 hours on the short job and 21 on the default one,
at the racing run's rate. Long runs have crashed the maintainer's machine. So each stage decides whether the next
one runs.

The grid: FFD, v2, packing, loop against parallel. Items 4, 8, 16, 32, 64, 128. Bins 2, 4, 8, 16. Cores 2 and 12.

| Stage | What | Cases | Short / default |
|---|---|---|---|
| A | Even | 96 | about 8 min / 25 min |
| B | Similar and Uneven | 192 | about 16 min / 50 min |
| C | SimilarTooSmall, UnevenTooSmall, NothingFits | 288 | about 25 min / 75 min |
| D | cores 4 and 8, only where 2 and 12 disagree | depends | - |
| E | a BFD check, OneHeavy, and the Real set | small | - |

- **Items stop at 128.** The old grid had parallel winning at about 5 to 18 us per bin on 2 bins, 2 to 3 us on
  4, 1.5 to 2 us on 7 - small item counts. 256 and up only if 128 still loses.
- **Cores 2 and 12 first.** If both give the same drop point, cores barely matter.
- **FFD only.** The algorithm is one more source of work. The BFD check shows whether the drop point, in us,
  lands in the same place.

## Open

- **16 bins in stage A, or cap it at 8?** 16 on 2 cores is the case most likely to show bins waiting for a core.
- **Does bin size change the work when all items fit?** Stages A and B answer it. If not, Similar and Uneven fold
  into Even, and OneHeavy into Uneven.
- **Does a set with too-small bins act like a set of fewer bins?** If so, the rule needs only a cheap count of
  the bins the items fit.
- **The bin sets have to grow with the items**, or "all items fit" stops being true past some count. How is not
  worked out.
- **Fitting.** This covers packing only. Fitting stops at the first item that does not fit, so its work varies by
  bin even more. If the bins processor also fits, that is its own drop point.
- **The multi-bin `Best` routes run `LoopMultiAlgorithmBinProcessor`**; its parallel twin is benched nowhere.
  Whether it belongs in `ParallelBins` is open.

## Candidates for the Real set - suggested (agent, 2026-09-28), pending

Picked to spread the work per bin, when the plan used real problems to find the drop point. Row 1 packs nothing -
no item fits the bin any way round; its `DemoSample_07_TallItems_20x20x60` does.

| # | Problem | Items | Time per bin |
|---|---|---|---|
| 1 | DemoSample_07_TallItems_40x40x30 | 4 | about 0.4 us |
| 2 | DemoSample_11_SevenTypes_40x30x30 | 13 | about 1 us |
| 3 | DemoSample_18_FourBinsBfdFullyPacks_45x35x30 | 20 | about 2 us |
| 4 | DemoSample_13_TwentyFourCubes_40x30x25 | 24 | about 2.5 us |
| 5 | Simple_5x5x5-50_FitIn_50x50x50 | 50 | about 4 us |
| 6 | th1_72 | 74 | 7 / 22 us |
| 7 | th4_22 | 111 | 16 / 60 us |
| 8 | th1_44 | 142 | 22 / 48 us |
| 9 | th3_67 | 169 | 33 / 61 us |
| 10 | th4_84 | 125 | 23 / 201 us |
| 11 | th7_56 | 162 | 39 / 145 us |
| 12 | th1_65 | 476 | 77 / 665 us |

Rows 1 to 5 are guessed from item count; the rest are FFD / BFD from the Full_ reports. A Real set is one item
list against several boxes, so each needs a box set as well.

## Also suggested (agent), pending

- **`MaxDegreeOfParallelism` as a knob the rule turns**, tested here. It does not imitate a smaller machine.
  `ParallelBinProcessor` takes a `concurrencyLevel` that only sizes its dictionary and never reaches
  `MaxDegreeOfParallelism`; fix that before any rule relies on it.

## Done when

- [ ] The bins bench runs the stages the maintainer picked, and he has run them and kept the reports.
      **By eye.** A report of a new class under `lib/results/benchmarks/*/parallel-bins/`.
- [ ] The drop point is read out of it, or the report shows there is none, and the lib findings record says so.
      **By eye.**
