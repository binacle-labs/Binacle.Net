---
description: Session 5 - ParallelBins - find where packing many bins at the same time starts to pay. Six Identical classes, a Pieces and a Lines one per algorithm, same params within a kind
state: ready
waits-on: "the maintainer runs the six classes - built and dry-checked 2026-10-03. The grid values are a suggestion until he picks them"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelBins/**", "lib/test/Binacle.Lib.Testing/**", "shared/test/Binacle.Benchmarking/**", "tooling/bench.just"]
---

# 5 - ParallelBins drop point

## What is left - agent, 2026-10-03

**The project has no kept run.** `Identical_FFD_Packing` and `Identical_FFD_Lines_Packing` had short-job runs;
both reports were removed on 2026-10-03 with the reshape, under the baseline rule in the orchestrator. What
they showed is written out below and is the only record of it.

**Read this before reading any number further down.** `ParallelBinProcessor` changed on 2026-10-02: it now caps
`maxDegreeOfParallelism` at `min(bins, cores)` by default instead of running unlimited. So `Bins = 1` resolves
to a degree of 1 and `Parallel.For` runs the body inline, where before it paid a dispatch and got nothing for
it - which is what "Parallel never wins on 1 bin" below was measuring. And 32 bins on the 2-core job is capped
at 2, where before the thread pool could inject more. **Those two cells of the old numbers are dead; from 2
bins up on 4 cores and above they still describe the code.**

**Open, and his call:** the classes call `ParallelBinProcessor` on its default cap, because this project asks
whether parallel pays in production and the cap is production now. Passing `ParallelLimits.NoLimit` instead
would measure the uncapped dispatch, which is what `ParallelOverhead` is for.

**The six classes are built and dry-checked** (2026-10-03). What is left here is the runs.

**The lines question is now asked on all three algorithms.** The FFD run was flat across 1 to 64 lines, so lines
cost nothing once they are pieces. The 2026-10-02 suggestion to drop the class is **superseded** - see the shape
section: he chose a Lines class per algorithm, to prove the negative rather than argue it.

What is left, in order:

1. The runs, one at a time with nothing else running: `just bench lib-parallel-bins-identical FFD`, then
   `WFD`, then `BFD`. Each report goes under `lib/results/benchmarks/baseline/parallel-bins/`, with a row in
   `lib/results/benchmarks/baseline/README.md`, and its time in the recipe's comment. The questions they
   answer: do WFD and BFD stop near the same Ratio as FFD, where do they start to win, and is each Lines class
   flat against its Pieces twin?
2. Read them with him, then pick the next step from "The steps" below. Each is built only after the one before
   has run, so only one can be built at a time.

## The shape - six classes

**Built and dry-checked 2026-10-03**, in `lib/bench/Binacle.Lib.Benchmarks.ParallelBins/Identical/`: a Pieces
and a Lines class for each of FFD, WFD and BFD, two methods each. The class set, one grid per kind, the folder,
and why the Lines classes measure an expected nothing are decided and written up in the lib decisions ledger
under `.agents/design/lib/`; what each class is now is in the lib tests doc under `.agents/docs/lib/`.
**Nothing about the shape is open here.**

What bites whoever runs this:

- **The grids are nobody's decision yet.** Pieces kind: bins 1 to 32 by pieces 1 to 256 on one line, 504
  cases. Lines kind: 4 bins, 64 and 256 pieces over 1, 4, 16 and 64 lines, 64 cases. Both carried over from
  the removed runs. **Suggested (agent, 2026-10-03):** leave them until a run says otherwise.
- **If a Lines class is not flat, there is one place to look.** The algorithm's constructor calls
  `CalculateVolume` and `CalculateLongestDimension` once per line, outside the quantity loop, so N lines of
  one pays N and one line of N pays one. Two multiplies and a comparison each, so it would only show at the
  small piece counts.
- **The call site uses the default cap**, because this project asks whether parallel pays in production.
  `ParallelLimits.NoLimit` would be asking `ParallelOverhead`'s question instead, and switching is his call.

## Beyond Identical - the next scenario

**Not touched this session** (the maintainer, 2026-10-03): "we wont toch the more advances this session". What
follows is written so the next session can pick it up.

**(the maintainer, 2026-10-03):** for bins, the harder scenario is the same items against different bins - "on
binms usually means same items but different bins".

**Suggested (agent, 2026-10-03):** this is the axis the Identical classes cannot show, and it is the `Spread`
category the suggestions further down already name. Every bin in Identical is a copy, so every bin takes the
same time and Parallel's cost comes out as its setup plus about the average bin. Parallel really costs its setup
plus the **slowest** bin. Bins of different sizes are what put those two apart, and that is what can stop
parallel paying even when there are plenty of bins and plenty of cores.

**Suggested (agent, 2026-10-03):** own folder beside `Identical/`, carrying the same six classes, so `--filter`
picks a whole scenario. The name is his.

## What the FFD run shows - agent, 2026-10-03

Read from `baseline/parallel-bins/Identical_FFD_Pieces_Packing.md` and
`Identical_FFD_Lines_Packing.md`, short job, run 2026-10-03 on the six-class shape. These supersede the
2026-10-01 reading of the removed pre-cap report. Ratio is BenchmarkDotNet's, Parallel against Loop.

### The drop point is a surface, and it moves both ways

The piece count where Parallel first wins, by bin count and core count:

| Bins | 2 cores | 4 cores | 8 cores | 12 cores |
|---|---|---|---|---|
| 1 | never | never | never | 128 |
| 2 | 32 | 32 | 32 | 32 |
| 4 | 8 | 16 | 16 | 32 |
| 8 | 4 | 4 | 16 | 32 |
| 16 | 2 | 2 | 8 | 16 |
| 24 | 1 | 1 | 4 | 8 |
| 32 | 1 | 1 | 4 | 4 |

**More bins win earlier; more cores win later.** Both directions are monotone with no exceptions, which is what
makes this readable as a rule rather than a table. The 1-bin row is the control: there is nothing to
parallelise, and the single cell at 0.97 is noise, not a win.

### Parallel never captures more than about 2x, however many bins

This is the finding to take seriously, and it is the old open question answered the hard way.

| Bins | best ratio | perfect scaling | what it got |
|---|---|---|---|
| 2 | 0.63 | 0.50 | 1.6x of a possible 2x |
| 4 | 0.61 | 0.25 | 1.6x of a possible 4x |
| 8 | 0.57 | 0.12 | 1.8x of a possible 8x |
| 16 | 0.54 | 0.08 | 1.9x of a possible 12x |
| 24 | 0.58 | 0.08 | 1.7x of a possible 12x |
| 32 | 0.53 | 0.08 | 1.9x of a possible 12x |

**The ceiling does not move with the bin count.** 2 bins and 32 bins both stop near 0.55, so going wider buys an
earlier drop point and nothing else. The 2026-10-01 reading already said "what stops Parallel near 0.55 is not
known"; it is now known not to be the bin count, the core count or the request size.

**Suggested (agent, 2026-10-03):** the likeliest cause is memory, not scheduling. Each bin builds its own piece
array, so 32 bins by 256 pieces allocates 32 copies, and `Fake_Bins` in `ParallelOverhead` already showed the
cost of handing a bin to another CPU growing far faster than the bin count once each bin walks 64 KB. Nothing
measures this yet.

### `Bins = 1` is the cap's receipt, and it reads in nanoseconds

`Parallel` sits a flat **230 to 307 ns** above `Loop` on every core count up to 32 pieces, which matches what
`ParallelOverhead` measures for `Parallel.For`'s own setup. The cap works: the step does not grow with the core
count, so nothing is being dispatched.

**Do not read that row as a ratio.** The same step is 1.79 to 1.98 at one piece, because the baseline there is
only about 290 ns. Past 64 pieces the step is smaller than the noise and goes negative. Read it at 1 to 32
pieces, in nanoseconds.

### Lines are flat, with one small real cost at the extreme

The negative the maintainer asked for, measured. `Loop` against the one-line case:

- **At 256 pieces: flat.** -0.9 to +1.8 per cent across 1, 4, 16 and 64 lines, which is inside the noise.
- **At 64 pieces over 64 lines: +3.6 to +10.3 per cent**, or +0.6 to +1.6 us on a 16 us baseline. `Loop` StdDev
  is 0.80 per cent of mean at the median and 2.0 at the 90th percentile, so this is above the noise.

**And it is exactly where this plan predicted it.** The algorithm's constructor calls `CalculateVolume` and
`CalculateLongestDimension` once per line, outside the quantity loop, so 64 lines of quantity 1 pays 64 of them
where one line of 64 pays one. It shows only at the small piece count, where there is nothing else to hide it.

**So the rule can read pieces** - lines do not change the shape of the answer. But lines are not free: a request
of many single-quantity lines costs a few per cent more than the same pieces in one line.

### Noise

`Loop` StdDev is 0.34 per cent of mean at the median on the Pieces class, 0.90 at the 90th percentile, worst
7.0. `Parallel` is 1.80, 5.5 and 14.0. The Lines class is 0.80 / 2.0 / 4.9 and 2.3 / 7.0 / 10.4. No cell looked
like the slow whole-process problem that spoils the WFD racing rows.

## What the overhead project measured for bins - agent, 2026-10-02

`Fake_Bins` in `Binacle.Lib.Benchmarks.ParallelOverhead` runs the bin processors with algorithms that do not
pack, so it gives the cost side of this plan's rule without any packing in it. Short job, 2026-10-02.

**What handing a bin to another CPU costs, 12 cores** - the gap between `Parallel` and `Parallel_OneThread`:

| Bins | walking nothing | walking 64 KB |
|---|---|---|
| 1 | 556 ns | 1,265 ns |
| 2 | 871 ns | 3,137 ns |
| 8 | 2,003 ns | 15,134 ns |
| 32 | 4,644 ns | 54,111 ns |

**The second column is the finding.** Most of the cost is the bin's own working set being fetched to whichever
CPU the bin landed on, not the dispatch. With nothing to fetch the cost is small and grows slowly; with a real
working set it grows much faster than the bin count.

Two cautions on that table. The 32-bin, 64 KB row allocates about 2 MB an operation and is where two cells came
back `NA`, so part of it is garbage collection rather than fetching - the racing plan carries the fix. And the
1-CPU rows of that run are not usable at all.

**What it means for the rule here:** `C` is not one number per machine. It rises with the bin count and with
how much memory a bin touches, so "Parallel costs its setup plus the slowest bin" understates it - the setup
term is itself a function of the request. That is a suggestion from one short run, not a settled rule.

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
Every class is pinned through `JobsByCoreCount` in `shared/test/Binacle.Benchmarking` ("yes common code to shared").

The project has `IdenticalBase` and a class per algorithm on it: identical bins and items from `IdenticalCase` in
`lib/test/Binacle.Lib.Testing`, Loop against Parallel, all four core counts `JobsByCoreCount` has. **The class
set and its grids are the shape section above.** Recipe `lib-parallel-bins-identical` takes the algorithm, and
`dry` to check first. Setup throws if any bin is not fully packed. The maintainer, 2026-09-30: "yes make sure they always pack everything fully meaning 256 item fit int box".
The suggestions below were written when it was called `Even`.

**Leaning yes (the maintainer, 2026-09-29):** one class per bin-set category, and one recipe that takes the
category. "i think so". A class each, because BenchmarkDotNet's `--filter` picks classes and methods, not parameter
values.

**How the cores are picked:** `JobsByCoreCount` runs every class on 2, 4, 8 and 12 cores. Running only some needs a
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

**1. Identical** (was `Even`) - needs nothing. FFD run and kept 2026-10-03; WFD and BFD built and waiting on a run.
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
  each piece a scan of the free spaces, up to 6 turns. **Measured 2026-10-03** - flat at 256 pieces, and up to
  10 per cent dearer at 64 pieces over 64 lines, which is the per-line work in the constructor.
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
- **Built:** all four core counts in one run; `JobsByCoreCount` has no way to pick some.
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

- **`MaxDegreeOfParallelism` as a knob the rule turns**, tested here. It does not imitate a smaller machine: the
  .NET documentation says it limits how many run at once, not which CPU they land on.
  **The constructor part is done** (agent, 2026-10-02): `ParallelBinProcessor` and
  `ParallelMultiAlgorithmBinProcessor` now take `maxDegreeOfParallelism` and pass it to `Parallel.For`, and the
  old `concurrencyLevel` argument is gone - the lock count is worked out as one writer per bin, capped by the
  threads that can run. Nothing turns the knob yet, and no bins run has measured it.

## Done when

- [ ] `ParallelBins` holds the six classes, each with `Loop` and `Parallel`, in a folder.
      `ls lib/bench/Binacle.Lib.Benchmarks.ParallelBins/Identical/` lists the six and their base.
- [x] `Identical_FFD_Packing` and its kept report are gone.
      `test ! -f lib/results/benchmarks/baseline/parallel-bins/Identical_FFD_Packing.md`
- [ ] The six have a kept baseline run.
      **By eye.** Six reports under `lib/results/benchmarks/baseline/parallel-bins/`, with a row each in that
      folder's README and the real time in the recipe's comment.
- [ ] Each Lines class is read against its Pieces twin: flat, or the difference is named and explained.
      **By eye.** Flat is the expected answer, and a negative is the point of running them.
- [ ] The drop point is read out of it, or the report shows there is none, and the lib findings record says so.
      **By eye.**
