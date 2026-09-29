---
description: Session 5 - find where packing many bins at the same time starts to pay. The ideal case is built and not run; the other categories are pending suggestions
state: ready
waits-on: "session 4's racing run would teach it; the maintainer starts 5 anyway (2026-09-30: 'i will open a session to do #5')"
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

## The bench

This session builds `lib/bench/Binacle.Lib.Benchmarks.ParallelBins/` - "then the 5th with discovery" (the
maintainer, 2026-09-29). Session 3 retired the old bins classes ("3 retire"); their reports left the baseline
(in git at commit 59e9dfb3, `lib/results/benchmarks/baseline/threshold/`). Every class is pinned through `CoreJobs` in
`shared/test/Binacle.Benchmarking` ("yes common code to shared").

The project has one class, `Even_Packing`: identical bins `160x120x80`, one line of `20x15x10` items, FFD v2,
Loop against Parallel. Bins 1, 2, 4, 8, 16; pieces 8 to 256; all four core counts `CoreJobs` has. Recipe
`lib-parallel-bins-even`, `dry` to check first. Its setup throws if any bin is not fully packed. The maintainer,
2026-09-30: "yes make sure they always pack everything fully meaning 256 item fit int box". The other classes
were removed - "remove the classes we havent decided anything". Not run yet.

`LadderGenerator.GetBins` in `lib/test/Binacle.Lib.Testing/` lost its last caller with the old bins classes.
Session 3 kept it for this one; delete it if this bench does not use it.

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

**Past this section, everything in this file is pending**, kept as suggestions to reinvestigate. The maintainer, 2026-09-29:
"reword the plans as not decided but pending and just as suggestion".

## Next

1. The maintainer runs `just bench lib-parallel-bins-even dry`, then `just bench lib-parallel-bins-even`.
2. Read it: does Loop grow with the bins, what Parallel costs on its own, where Parallel starts to win on each
   core count.
3. Pick the next categories from what it shows.

`parallel-bins.md` is the maintainer's to shape; session 6 builds it.

## Why the kept bins run cannot answer it

`Full_Bins_Packing_v2` jumps around: FFD wins at 47 items on 2 bins (0.96) and loses at 59 (1.63). Every step
of that grid changed two things at once. One more bin was also a taller bin (60x40x10 up to 60x40x40), and from
59 items the ladder's total no longer fits the small bins. The grid stays as the record.

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
- **Built:** one shape `20x15x10`, one line, 8 to 256 pieces. Bins 1 to 16 - the one-bin row is `Bins = 1`.
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
- **Built:** one recipe per class so far, `lib-parallel-bins-even`.

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
