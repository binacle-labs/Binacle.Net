---
description: Session 5 - ParallelBins - find where packing many bins at the same time starts to pay. Add and try, starting with Identical - one class per algorithm, FFD run first; the other categories are pending suggestions
state: ready
waits-on: "the maintainer runs WFD and BFD"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelBins/**", "lib/test/Binacle.Lib.Testing/**", "shared/test/Binacle.Benchmarking/**", "tooling/bench.just"]
---

# 5 - ParallelBins drop point

## What is left - suggested (agent, 2026-10-01)

`Identical_FFD_Packing` and `Identical_FFD_Lines_Packing` are run and kept, short job. WFD and BFD passed `dry`
only.

1. The maintainer runs `just bench lib-parallel-bins-identical WFD`, then `BFD` - one at a time, nothing else
   running meanwhile. Keep each report under `lib/results/benchmarks/baseline/parallel-bins/`, with a row in
   `lib/results/benchmarks/baseline/README.md`, and put its time in the recipe's comment.
2. Read them with him: do WFD and BFD stop near the same Ratio as FFD, and where do they start to win?
3. Pick the next step with him from "The steps" - each is built only after the one before has run.

## What the FFD run shows - agent, 2026-10-01

Read from `baseline/parallel-bins/Identical_FFD_Packing.md`, short job. Ratio is BenchmarkDotNet's, Parallel
against Loop.

- **Parallel's best Ratio is about 0.55, on every core count.** On the big jobs - 4 or more bins, Loop at 30 µs or
  more - the median is 0.65 on 2 cores, 0.61 on 4, 0.61 on 8 and 0.63 on 12. Most cells sit between 0.55 and
  0.70, RatioSD at most 0.07. A few reach 0.83 and 0.87, on 8 and 12 cores.
- **One cell shows 0.43** - 32 bins, 32 pieces, 2 cores. Its Loop took 98 µs against 67 µs on the other core
  counts: a slow Loop process, not a gain.
- **The ideal is far lower** - near 0.13 for 8 bins on 8 cores. What stops Parallel near 0.55 is not known.
- **Loop is the sum of the bins**, and lines cost nothing: the lines class is flat across 1 to 64 lines.
- **Parallel never wins on 1 bin.** It starts to win once Loop would take about 4 to 8 µs on 2 and 4 cores, and
  about 16 µs on 12. More cores need a bigger request.

## Goal

**(the maintainer, 2026-09-29):** measure the bins parallelisation threshold, or its cost function - when
packing many bins in parallel beats packing them one after another. "the other the bins paralilizxation
thresohold of their cost function".

**Decided (the maintainer, 2026-09-25):** "lets make the algo race first".

His worry (2026-09-25): "it does worry me how we can test reliably with different bin shapes when i failed with
the ladder".

**(the maintainer, 2026-09-29):** this session finds the problems ParallelBins runs - "step 5 is finding the
problems and keep suggestions will reinvestigate".

**(the maintainer, 2026-09-30):** add and try - "we agreed to take a more simple add and try approach".

**Leaning yes (the maintainer, 2026-09-30):** the first step uses the same bin and the same items here and in
ParallelAlgorithms - "same bin msame items on both ideally this should give us the perfect results".

**Decided (the maintainer, 2026-09-30):** one class per algorithm - "for bins we need 3 one for each algotirhm to
see how algorithms change behaviour". The shape, with FFD run first - "But the shape is good... we will initially
choose to run FFD only". The name - "identical write it into the plans".

## The project

`lib/bench/Binacle.Lib.Benchmarks.ParallelBins/` - "then the 5th with discovery" (the maintainer, 2026-09-29).
Every class is pinned through `CoreJobs` in `shared/test/Binacle.Benchmarking` ("yes common code to shared").

The project has `IdenticalBase` and a class per algorithm on it: identical bins and items from `IdenticalCase` in
`lib/test/Binacle.Lib.Testing`, Loop against Parallel, all four core counts `CoreJobs` has.
`Identical_FFD_Packing`, `Identical_WFD_Packing`, `Identical_BFD_Packing`: bins 1 to 32, pieces 1 to 256, one line.
`Identical_FFD_Lines_Packing`: 4 bins, 64 and 256 pieces over 1, 4, 16 or 64 lines. Recipe
`lib-parallel-bins-identical` takes the algorithm, and `dry` to check first. Setup throws if any bin is not fully
packed. The maintainer, 2026-09-30: "yes make sure they always pack everything fully meaning 256 item fit int box".
The suggestions below were written when it was called `Even`.

**Leaning yes (the maintainer, 2026-09-29):** one class per bin-set category, and one recipe that takes the
category. "i think so". A class each, because BenchmarkDotNet's `--filter` picks classes and methods, not parameter
values.

**How the cores are picked:** `CoreJobs` runs every class on 2, 4, 8 and 12 cores. Running only some needs a
change in `shared/test/Binacle.Benchmarking`; not made.

**(the maintainer, 2026-09-30)** - what each case records: "cores... bins count and items count ... flattened
count". Items count is the item lines; flattened count is read here as the pieces, every quantity added up.
`BinProcessorFactory.Create` sees only bins count and items count today.

**Leaning yes (the maintainer, 2026-09-30):** synthetic cases first - "i believe if we make synthetic we can
derive the rest". Start with the ideal case, identical bins and identical items - "we can start with idea
scenario identical bins nad items?". The other categories are to be reviewed - "we havent figured out the rest".

**(the maintainer, 2026-09-30):** one step at a time - "i say we measure 1 step at a time and build the next...
perhaps we can get some insight so prioritize them by what they need to run before them". Each step is built only
after the one before it has run.

**Past this section, everything in this file is pending**, kept as suggestions to reinvestigate. The maintainer, 2026-09-29:
"reword the plans as not decided but pending and just as suggestion".

## The steps, in the order the work needs - suggested (agent, 2026-09-30), pending

Ordered by what each needs measured before it. From the review below.

**1. Identical** (was `Even`) - needs nothing. FFD run and kept; WFD and BFD to run.
- Answers: what Parallel costs on its own, and the fastest a bin can pack.
- Its box is an exact multiple of the item, so every piece takes the first free space. Its time per piece is the
  lowest any request can have - never read a piece count off it.

**2. Fit run** - needs 1, to compare against the floor.
- Identical bins where none, 10%, 50%, 90% and all of the items fit. One row set with fitting as well as packing.
- The bin must not be an exact multiple of the item, or pieces that do not fit cost nothing.
- Answers: time per piece for each kind of fit. The 90% bin should be the slow one.

**3. MixedItems** - needs 1. Can run beside 2.
- Identical bins, several item shapes, fill kept about half to two-thirds - the bin grows with the pieces here.
- One main shape plus a few others, or 3 to 20 shapes like Bischoff.
- Answers: how far real time per piece sits above Even. Mixed shapes leave slivers every later piece scans past.

**4. Spread** - needs 2 and 3. Built from their times.
- Bins whose times differ by a known amount, up to one bin doing most of the work.
- Answers: does "Loop is the sum, Parallel is setup plus the slowest bin" hold. If it does, the rule can be drafted.

**5. ShopRange** - needs 4, a drafted rule to check.
- 3, 5 and 8 boxes that nest, spaced like the DHL range; the largest fits everything.
- Answers: does the rule hold on the request shape shops send.

**6. Real** - needs 5.
- The demo samples and presets. Scores the rule, never tunes it.

**Late checks** - need the machine part from 1 to 4: a BFD row, a multi-algorithm row.

`parallel-bins.md` is the maintainer's to shape; session 6 builds it.

## What bites

- **Each bin is packed on its own, with the whole item list.** Loop costs the sum of the bins. Parallel costs its
  setup plus the slowest bin, while there are enough cores.
- **Lines only matter while they are turned into pieces.** After that FFD v2 works per piece: a sort, then for
  each piece a scan of the free spaces, up to 6 turns. Read from the code, not measured.
- **A bin that fits only some of the items is the slowest.** A piece that does not fit scans every space 6
  times. A bin that fits nothing is the fastest - its space list stays at one. Packing never stops early; only
  fitting does. In the fixtures the first word of a result is packing, the second fitting - the shared slice doc
  has the format.
- **Identical items are the cheapest work per piece.** A drop point read in pieces from them sits too low for mixed
  items. Read it in time per bin, then map time to pieces from mixed items.
- **The rule cannot see time.** It sees the request. A case has to be described in what the request carries;
  time is only how the case is measured.
- **Each bin builds its own piece array**, so memory grows with bins times pieces.

## Split the rule in two - suggested (agent, 2026-09-30), pending

- **The machine part:** from the time one bin takes, the bins and the cores, does Parallel win? Shapes do not
  enter it. `Even` answers it.
- **The request part:** guess the time one bin takes, and how uneven it is across bins, from what the request
  shows. This is where the other categories come in.

## All suggestions, by topic - pending

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
- **Built:** all four core counts in one run; `CoreJobs` has no way to pick some.
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

## Review of the suggestions - agent, 2026-09-30, pending

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
- **Cores 2 and 12 first** - needs a `CoreJobs` change. Not needed now: Even runs all four core counts on the short job.
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

- [ ] ParallelBins runs the stages the maintainer picked, and he has run them and kept the reports.
      **By eye.** A report of a new class under `lib/results/benchmarks/*/parallel-bins/`.
- [ ] The drop point is read out of it, or the report shows there is none, and the lib findings record says so.
      **By eye.**
