---
description: Every suggestion for harder, more realistic bench cases for the two parallel bench projects - scenarios beyond Identical, bin-set categories, item mixes, a spin stub, and a Real set. Nothing here is figured out
state: idea
waits-on: "nobody - it is an idea. An agent picked the horizon to make the file legible; strike it"
horizon: undecided
paths:
  - "lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/**"
  - "lib/bench/Binacle.Lib.Benchmarks.ParallelBins/**"
  - "shared/test/Binacle.Benchmarking/**"
---

# Harder bench cases for the parallel benches

`Binacle.Lib.Benchmarks.ParallelAlgorithms` and `Binacle.Lib.Benchmarks.ParallelBins` both bench one scenario,
`Identical`: identical bins, identical items, everything packs. This file collects every suggestion for cases
harder than that - uneven algorithms, uneven bins, mixed item shapes, real box ranges.

**Nothing here is figured out.** Every line is a suggestion with its source and date. The suggestions disagree
with each other in places and are left that way. The maintainer, 2026-09-29: "reword the plans as not decided
but pending and just as suggestion".

## Beyond Identical, for racing algorithms

**Not touched** (the maintainer, 2026-10-03): "we wont toch the more advances this session".

**(the maintainer, 2026-10-03):** Identical is the ideal scenario, where every item packs. What makes a harder
one here is that one algorithm may fail to pack - "now the identical is the ideal scenario / all items pack /
but it may be that on algorithms 1 may bnot pack".

**Suggested (agent, 2026-10-03):** that is the one axis `ParallelAlgorithms` is missing, and it matters more
than another grid value. Parallel costs its setup plus the **slowest** algorithm; Loop costs their sum. In
Identical the racers sit close together, so Parallel's cost is setup plus about the average and the spread never
enters. A scenario where one algorithm does not pack puts the spread on the table, and the spread is what
decides whether racing pays.

It is also the reason `Algorithm.Best` races at all: a race is run because one algorithm may pack better, not
because racing is quick.

**Suggested (agent, 2026-10-03):** it gets its own folder beside `Identical/`, carrying the same five classes,
so `--filter` picks a whole scenario. The name is his; it has to say that not everything packs.

**Suggested (agent, 2026-09-30):** the request part of the rule - how uneven the algorithms are. Loop costs the
sum of the algorithms; Parallel costs setup plus the slowest. The gain shrinks as one algorithm takes most of
the time. A class where one algorithm is made slow on purpose - a bin that fits only part of the items -
measures it.

**Suggested (agent, 2026-09-30):** the order the work needs is the machine part, then the request part, which
needs its setup cost. A class on real problems then checks the rule; `Full_Packing` at commit 16560b6f is one
to start from.

## Beyond Identical, for bins

**Not touched** (the maintainer, 2026-10-03): "we wont toch the more advances this session".

**(the maintainer, 2026-10-03):** for bins, the harder scenario is the same items against different bins - "on
binms usually means same items but different bins".

**Suggested (agent, 2026-10-03):** this is the axis the Identical classes cannot show, and it is the `Spread`
category named further down. Every bin in Identical is a copy, so every bin takes the same time and Parallel's
cost comes out as its setup plus about the average bin. Parallel really costs its setup plus the **slowest**
bin. Bins of different sizes are what put those two apart, and that is what can stop parallel paying even when
there are plenty of bins and plenty of cores.

**Suggested (agent, 2026-10-03):** own folder beside `Identical/`, carrying the same six classes, so `--filter`
picks a whole scenario. The name is his.

## Split the rule in two - suggested (agent, 2026-09-30)

- **The machine part:** from the time one bin takes, the bins and the cores, does Parallel win? Shapes do not
  enter it. `Even` answers it.
- **The request part:** guess the time one bin takes, and how uneven it is across bins, from what the request
  shows. This is where the other categories come in.

## The bins steps after Identical - suggested (agent, 2026-09-30)

Ordered by what each needs measured before it. Identical needs nothing and answers what Parallel costs on its
own, and the fastest a bin can pack.

**Fit run** - needs Identical, to compare against the floor.
- Identical bins where none, 10%, 50%, 90% and all of the items fit. One row set with fitting as well as packing.
- The bin must not be an exact multiple of the item, or pieces that do not fit cost nothing.
- Answers: time per piece for each kind of fit. The 90% bin should be the slow one.

**MixedItems** - needs Identical. Can run beside the fit run.
- Identical bins, several item shapes, fill kept about half to two-thirds - the bin grows with the pieces here.
- One main shape plus a few others, or 3 to 20 shapes like Bischoff.
- Answers: how far real time per piece sits above Even. Mixed shapes leave slivers every later piece scans past.

**Spread** - needs the fit run and MixedItems. Built from their times.
- Bins whose times differ by a known amount, up to one bin doing most of the work.
- Answers: does "Loop is the sum, Parallel is setup plus the slowest bin" hold. If it does, the rule can be drafted.

**ShopRange** - needs Spread, a drafted rule to check.
- 3, 5 and 8 boxes that nest, spaced like the DHL range; the largest fits everything.
- Answers: does the rule hold on the request shape shops send.

**Real** - needs ShopRange.
- The demo samples and presets. Scores the rule, never tunes it.

**Late checks** - need the machine part from Identical through Spread: a BFD row, a multi-algorithm row.

## Make the x-axis time, not pieces - suggested (agent, 2026-10-01)

Put to the maintainer on 2026-10-01 with two shape changes that were built. He said of this one and one other:
"i dont understand 2 and 4 but i believe we need to see". Kept unexplained, to be gone through with him before
anything is built.

`Pieces` is a stand-in for how long one algorithm takes, and a poor one: WFD at 64 pieces costs about what FFD
costs at 256, so the same column value means different work in different rows, and a drop point read off it is
FFD's drop point rather than the rule. A stub algorithm that spins for a set duration would sweep *work per
algorithm* straight, from nothing up to 100 us. The drop point then comes out as a curve instead of 9 points,
it is the same curve whichever algorithm is in front of it, and a spin loop has nothing for the run-time
optimization to get wrong, so the slow-process cells go away. Real algorithms become the check: does FFD at 256
pieces land where the curve says? It also gives unevenness without a contrived bin - spin two threads for 100
and 100, then 100 and 10, then 100 and 1.

## All suggestions for bins, by topic

Every line below is a suggestion. Three sources: the agent of 2026-09-29 (**old**), the agent of 2026-09-30
(**new**), and the session of 2026-09-30 (**idea**). The maintainer, 2026-09-30: "write them down to the plan as
suggestions all of them".

### 1. Categories

The examples use 32 items, `10x10x10 [16] 20x10x10 [8] 20x20x10 [8]`, volume 64,000.

**Old:**

| Name | What it is | Example bins |
|---|---|---|
| Even | One bin, copied. The ideal case, best for parallel | `80x60x40 [4]` |
| Similar | Sizes close together, all items fit | `60x60x40` `70x60x40` `80x60x40` `90x60x40` |
| SimilarTooSmall | Sizes close, one or more too small for all items | `40x40x35` `50x40x40` `60x40x40` `70x40x40` |
| Uneven | Sizes far apart, all items fit | `50x40x40` `80x60x40` `120x80x60` `200x100x100` |
| UnevenTooSmall | Sizes far apart, some too small. Closest to a real shop | `20x20x20` `40x30x30` `80x60x40` `200x100x100` |
| NothingFits | No item fits any bin. Parallel is only overhead; loop should always win | `60x60x8` `40x40x5` `100x9x9` `30x30x9` |
| OneHeavy | One bin does most of the work; parallel waits for it | `50x40x40 [3]` `200x100x100` |
| Real | Real box sets and items, never used to find the drop point. Checks the rule | Bischoff or the demo samples |

The maintainer named the first four on 2026-09-29. Categories 1 to 7 can be made up; Real has to be real.

**New** - a category matters only through the work per bin and how uneven it is across bins:

| Name | What it is | Why |
|---|---|---|
| Even | Identical bins and items, all fit. Built | The machine part |
| Spread | Bins whose work differs by a known amount; one bin doing most of it is the far end | The only thing that hurts Parallel beyond Even. Merges Uneven and OneHeavy |
| VariedItems | Identical bins, mixed item shapes and lines | Checks that time per piece from one shape holds for mixed ones |
| Real | Scores the rule, never tunes it | The check |

- Drop Similar: close sizes that all fit give about the same work.
- The three too-small sets become the fit run (topic 2) plus Spread. A too-small bin changes the work; it does
  not remove it.
- If Spread matches "Loop is the sum, Parallel is setup plus the slowest bin", no other category is needed.

**Idea** - only pieces and how many fit change a bin's time, so these are mixes of fit across the bins:

| Name | What it is | What it tests |
|---|---|---|
| FitsSomeMix | One bin fits part of the items, the rest fit all | The worst case for parallel: the part-fit bin is the slowest |
| FitsNoneMix | Most bins fit nothing, one fits all | Really one bin of work; loop should win |
| ShopRange | A carrier box range, small to large | All three fit kinds in one request; closest to real traffic |
| ManyBins | 24 or 32 identical bins | Bins waiting for a core |
| TinyRequest | 1 to 4 pieces, many bins | Parallel never wins |
| MixedItems | The ideal case with 3 item shapes | Whether time per piece from one shape carries over |
| OneBigItem | One item fits only the largest bin | A mild fits-some case shops hit often |

The session's pick: FitsSomeMix, ShopRange, MixedItems.

### 2. Items

- **Old:** the same three shapes at 2:1:1, only the count grows. `N` items is
  `10x10x10 [N/2] 20x10x10 [N/4] 20x20x10 [N/4]`. Items 4 to 128; 256 and up only if 128 still loses. The old
  grid had parallel winning at about 5 to 18 us per bin on 2 bins, 2 to 3 us on 4, 1.5 to 2 us on 7.
- **Old:** every case gets a one-bin row, like the alone rows in racing, so the work per bin is measured in the
  same run.
- **Built:** one shape `20x15x10`, one line, 1 to 256 pieces. Bins 1 to 32 - the one-bin row is `Bins = 1`.
- **New:** a lines run - the same pieces over 1, 4, 16 or 64 lines of one item. Should come out flat. If so, the
  rule should get pieces, not lines.
- **New:** a fit run - all fit, half fit, none fit, at a few piece and bin counts. Gives time per piece for each.

### 3. Bin size

- **Old:** for Even, one bin for every step, `160x120x80`, so only the item count changes:

  ```
  items  items (compact)                                fill
     4   10x10x10 [2]   20x10x10 [1]   20x20x10 [1]     <1%
    16   10x10x10 [8]   20x10x10 [4]   20x20x10 [4]      2%
    64   10x10x10 [32]  20x10x10 [16]  20x20x10 [16]     8%
   128   10x10x10 [64]  20x10x10 [32]  20x20x10 [32]    17%
  ```

  The cost: a few items in a near-empty bin is not a real request, and may pack faster than a real one. The Real
  set catches that.
- **Built:** `160x120x80` for every step, 8 by 8 by 8 of the `20x15x10` item. 256 fills half.
- **New:** a bin that grows with the pieces keeps the fill the same, but changes the bin too.

### 4. Layout and recipe

- **Old:**

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

  The lowercase word assumes `--filter` ignores case; unchecked (the maintainer, 2026-09-29: "ok dont care").
  `similar` does not catch `SimilarTooSmall_` - the underscore stops it.
- **Built:** one recipe, `lib-parallel-bins-identical`, takes the algorithm.

### 5. Run order

- **Old:** in stages, each deciding whether the next runs. All the categories on every setting is 5,120 cases -
  about 7 hours on the short job and 21 on the default one. The grid: FFD, v2, packing, loop against parallel;
  items 4 to 128; bins 2, 4, 8, 16; cores 2 and 12.

  | Stage | What | Cases | Short / default |
  |---|---|---|---|
  | A | Even | 96 | about 8 min / 25 min |
  | B | Similar and Uneven | 192 | about 16 min / 50 min |
  | C | SimilarTooSmall, UnevenTooSmall, NothingFits | 288 | about 25 min / 75 min |
  | D | cores 4 and 8, only where 2 and 12 disagree | depends | - |
  | E | a BFD check, OneHeavy, and the Real set | small | - |

- **Old:** cores 2 and 12 first - if both give the same drop point, cores barely matter. FFD only; the BFD check
  shows whether the drop point, in us, lands in the same place.
- **Built:** all four core counts in one run; `JobsByCoreCount` has no way to pick some. Running only some core
  counts needs a change in `shared/test/Binacle.Benchmarking`; not made.
- **New:** a dry run, then Even, then the lines run, the fit run, VariedItems, Spread, Real. Each only if the one
  before gave a clean answer. Real data never tunes the rule.

### 6. Open questions

- **Old:** 16 bins, or cap at 8? 16 on 2 cores is the case most likely to show bins waiting for a core.
- **Old:** does bin size change the work when all items fit? If not, Similar and Uneven fold into Even, and
  OneHeavy into Uneven.
- **Old:** does a set with too-small bins act like a set of fewer bins? If so, the rule needs only a cheap count of
  the bins the items fit.
- **Old:** the bin sets have to grow with the items, or "all items fit" stops being true past some count.
- **Old:** fitting. This covers packing only. Fitting stops at the first item that does not fit, so its work
  varies by bin even more. If the bins processor also fits, that is its own drop point.
- **Old:** the multi-bin `Best` routes run `LoopMultiAlgorithmBinProcessor`; its parallel twin is benched
  nowhere. Does it belong in `ParallelBins`?
- **New:** does the rule get the pieces? It changes what `BinProcessorFactory.Create` takes. Yes if the lines run
  is flat.
- **New:** Parallel on one bin may not spread across cores, so its cost there understates the setup cost at 4 or
  16 bins. The 2-bin row may be the better measure.
- **New:** the largest bin count real requests carry.
- **New:** short job to find the drop point, default only to confirm?
- **New:** may the rule do a cheap check first - bin volume against item volume - to guess fit and spread?

## Review of the suggestions - agent, 2026-09-30

Read-only review with research. Everything here is its suggestion.

### Categories

- **Even** - keep. The machine part, and the best case for Parallel.
- **VariedItems, MixedItems, the old 2:1:1 items** - merge into MixedItems. Even is the floor; mixed shapes are
  the real cost per piece.
- **Uneven, OneHeavy, Spread, FitsSomeMix, FitsNoneMix** - merge into Spread. Past Even, the only thing that hurts
  Parallel is one bin taking much longer than the rest; all five sit on that one scale. Why not as first
  written: a "known amount" of spread cannot be set before the time per bin is known.
- **Similar** - drop. Close sizes where all fit give about the same work: Even with a little spread.
- **SimilarTooSmall, UnevenTooSmall, OneBigItem** - merge into ShopRange. A real box range already has them.
- **ShopRange** - keep. It is what the presets, demo samples and shops look like.
- **NothingFits** - a row of the fit run, not a class. Cheap and easy to predict: its space list stays at one.
- **ManyBins, TinyRequest** - more values of Even, not classes.
- **Real** - keep, changed. No real request in the repo has several bins and many items: all 700 Bischoff
  problems use one container, `587x233x220`. Pairing them with shop boxes is made up. Use the demo samples and
  presets.

### Items and bin size

- **One shape, one bin for every step** - keep for Even. How full the bin is does not change Even's time.
- **Lines run** - values of Even's `Lines`, not a stage. FFD v2 uses lines only in its constructor.
- **Fit run** - keep, with 10%, 50% and 90%, and a bin that is not an exact multiple. A bin that fits most
  items is slow; one that fits few is cheap.
- **A bin that grows with the pieces** - drop for Even, keep for MixedItems, where slivers depend on the fill.

### Layout and run order

- **Eight classes** - drop. Four or five are enough. One recipe that takes the category - keep, once a second
  class exists; pull a base class out then, not before.
- **Stages A to E** - drop; built on the old category list.
- **Cores 2 and 12 first** - needs a `JobsByCoreCount` change. Not needed now: Even runs all four core counts
  on the short job.
- **BFD check** - keep, late. BFD v2 re-sorts the space list on every try, so its time per piece grows faster.

### Open questions - its answers

- **16 bins or 8?** 16, and add 32. Real ranges have 5 to 10 boxes.
- **Does bin size change the work when all fit?** Not for identical items in an exact grid. It does for mixed
  items, through slivers.
- **Do too-small bins act like fewer bins?** Fitting: nearly - `Execute` returns at once when the items' volume
  is more than the bin's, or an item is longer than its longest side. Packing: no - a bin that fits most items
  can be the slowest.
- **Must the bin sets grow with the items?** For ShopRange and MixedItems, yes. Check in setup that the largest
  bin fits everything.
- **Fitting?** One row set in the fit run before deciding it needs its own rule. Its early exits likely favour
  Loop.
- **The multi-algorithm route?** Later, one check row. Same machine rule, more time per bin.
- **Should the rule get pieces?** Yes, from the code: 1 line of 500 and 1 line of 5 look the same to `Create`
  today. Changing its signature is the maintainer's call.
- **Short job to find, default to confirm?** Yes.
- **A cheap volume check first?** Yes. It counts the bins clearly too small. It cannot predict how slow a
  part-fit bin is.

### Research

- Demo samples: 1 to 5 bins, 2 to 24 pieces (one has 92), largest to smallest bin volume usually 2 to 9 times.
  Usually only the largest fits everything. `shared/data/demo-samples/*.json`.
- Presets: 3 bins each, one footprint with growing height, or cubes 10, 20, 30.
  `api/src/Binacle.Net/Config_Files/Presets.json`.
- Custom problems use the 60x40 footprint family, `shared/data/custom-problems/*.json`. The Bischoff suite is
  one container, `shared/data/bischoff-suite/`.
- Monaci: 3 or 5 bin types, sizes within 1.5 to 2.5 times.
  https://www.researchgate.net/publication/222384866_Heuristics_for_the_variable_sized_bin-packing_problem
- Pisinger and Sigurd: 5 bin types, each side from half to full, one type forced to full size so a solution
  exists. https://www.sciencedirect.com/science/article/pii/S1572528605000216
- Martello, Pisinger and Vigo: identical bins; most classes are one main item shape plus others.
  https://hjemmesider.diku.dk/~pisinger/codes.html
- Ivancic: 47 instances with one container type, 17 with 2 or 3 types.
  https://eprints.soton.ac.uk/364226/1/A_20Comparative_20Review_20of_203D_20Container_20Loading_20Algorithms.pdf
- Shops use 5 to 10 box sizes, 8 is common.
  https://hub.shipium.com/playbook/how-to-decide-packaging-options-and-sizes-per-fulfillment-center/
- DHL Express boxes 1 to 8: about 1 to 98 litres, neighbours 1.3 to 3 times apart, the first three flat.
  https://www.easyship.com/blog/dhl-boxes-101

### Unsure

- Every cost claim is read from the code, not measured.
- In Even, memory and garbage collection may cost more than the scan, and threads may compete for them.
- The Crainic, Perboli and Tadei papers and the Ivancic multi-type instances could not be opened.
- The shop numbers come from vendor blogs, not studies.

## Candidates for the Real set - suggested (agent, 2026-09-28)

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

## The parallelism cap as a knob - suggested (agent)

- **`MaxDegreeOfParallelism` as a knob the rule turns**, tested in `ParallelBins`. It does not imitate a smaller
  machine: the .NET documentation says it limits how many run at once, not which CPU they land on.
  **The constructor part is done** (agent, 2026-10-02): `ParallelBinProcessor` and
  `ParallelMultiAlgorithmBinProcessor` now take `maxDegreeOfParallelism` and pass it to `Parallel.For`, and the
  old `concurrencyLevel` argument is gone - the lock count is worked out as one writer per bin, capped by the
  threads that can run. Nothing turns the knob yet, and no bins run has measured it.
