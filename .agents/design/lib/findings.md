---
id: lib/findings
description: Lib findings — the measured evidence (fill, version gains, algorithm racing cost, parallel algorithm racing, parallel bin processing, parallel overhead) behind the decisions.
verified: 2026-10-04
check: the five problems of F1 and F2 (thpack1_7, thpack1_44, thpack2_30, thpack2_35, thpack7_56) are in the Bischoff suite, and CoresSet and Full_Packing are still in git at commit 16560b6f; 8a7580f3 is still the commit that added ThrowIfCancellationRequested to the lib processors; the fitting family under lib/src/Binacle.Lib/Fitting/ is still gone; the Full_Algorithms_ and Full_Bins_ reports at commit 59e9dfb3 still carry the item ladder and bins 1 to 7. F1, F2 and the notes are not re-checkable from the repo - see Environment. F2a and F4 are: their reports are in git at commit 59e9dfb3, under lib/results/benchmarks/baseline/threshold/. F6 to F11 re-read from the kept files: the two measures under lib/results/measurements/, the Full_<alg>_Packing reports under lib/results/benchmarks/baseline/algorithms/, the three reports under baseline/parallel-overhead/, and the two FFD reports under baseline/parallel-bins/. F8's mechanism is the OrderBy in the v1 BFD and WFD AlgorithmOperation against List.Sort in v2; F11's is CalculateVolume and CalculateLongestDimension sitting outside the quantity loop in the v2 Algorithm constructors.
also_update:
  - lib/decisions
paths:
  - "lib/**"
---

# Lib — findings (the measured evidence)

The measured truth behind `$lib/decisions`. **No session keeps its own numbers** — they live here.

Ranges show the effect, not a guarantee.

## Environment

Every run is on an AMD Ryzen 9 9900X with 12 physical cores, Linux Ubuntu 26.04, BenchmarkDotNet v0.15.8.
Times compare within one run only; ratios and `Allocated` compare across runs.

| Run | What it produced | Runtime |
|---|---|---|
| 2026-07-17, `AlgorithmRacing_Packing_v2` | F1, F2 | .NET 10.0.9 |
| 2026-09-25, `just bench lib-threshold-full precise` | F2a, F4 | .NET 10.0.12, default job |
| 2026-09-25, `just bench lib-algorithms-full` | F8 | .NET 10.0.12, short job |
| 2026-10-02, `just bench lib-parallel-overhead` | F9 | .NET 10.0.12, default job |
| 2026-10-03, `just bench lib-parallel-bins-identical FFD` | F10, F11 | .NET 10.0.12, short job |
| `just measure lib` | F6, F7 | no job — a measure has no noise |

A measure gives the same result on every run and every machine, so F6 and F7 carry no error bars and are not
dated: any change in them is a real change.

Within-run error is **0.3–1.0%** of the mean on the 2026-07-17 run, so the effects below are far outside the
noise.

**F1 and F2 cannot be re-checked from a clone.** BenchmarkDotNet writes to `BenchmarkDotNet.Artifacts/`, which
`.gitignore` excludes, and nobody kept those reports. The harness that ran them, `RacingSet` and the racing
`BenchmarkBase`, was deleted 2026-09-26; its five problems were in `CoresSet`, which `Full_Packing` in `Binacle.Lib.Benchmarks.ParallelAlgorithms` raced; both left the tree 2026-09-30 and are in git at commit `16560b6f`. **F2a and F4 were
kept**: every number in them can be read out of `lib/results/benchmarks/baseline/threshold/` at commit `59e9dfb3`. The
reports left the tree 2026-09-30 with the classes that made them.

**Some kept times may be slow processes.** Found 2026-09-26: BenchmarkDotNet runs each case in one process, and
about 1 in 10 BFD processes runs about 1.7× slow from start to end, with a tight StdDev. The three-algorithm
`Loop` runs 1.26× to 1.46× the sum of its parts in the threshold run. So a single BFD cell of F4, and every
FFD,WFD,BFD ratio of F2a, may move on a rerun - and with them "BFD crosses about two bins earlier".

**The cause is tiered PGO, and it is not fixed.** Measured 2026-09-27 and 2026-09-28. With `DOTNET_TieredPGO=0`,
80 BFD v2 processes and 80 test-only BFD v3 processes each ran at one speed, within 1.13×; the loop fault went
too. With it on, BFD v2 had 2 slow processes in 40 on the racing bench. BFD v3, which has no sort in its loop,
runs slow on 16 and 19 of 40 processes, 1.2× to 1.4× slower (thpack3_29: 32.5 or 41 us; thpack6_39: 24 or 29.5 us).
So the space sort is not the whole cause, and which code PGO tunes badly is not known. PGO off is no fix: every
time is 1.5× to 2× slower and allocates more. The maintainer set it aside on 2026-09-28: "we wont chase that
now its too much".

**The scenario names below are the ones the run printed.** The keys in `RacingSet` (deleted 2026-09-26) were
renamed on 2026-09-22 to say what each problem is for; the problems did not change. Baseline is now
`typical container` (thpack1_7), BFD dominance is `BFD wins big` (thpack1_44), High efficiency is `near tie`
(thpack2_30), WFD weakness is `WFD falls over` (thpack2_35), Max complexity is `many item types` (thpack7_56; called `most item types` until 2026-09-23).

Racing benchmarks run **one bin** and race algorithms against each other. They say nothing about running many
**bins** in parallel — that is a different axis, and it is F4 below.

## F1 — WFD roughly triples the cost of a race

Adding WFD to the FFD+BFD pair, single bin, `Loop`:

| Scenario | FFD,BFD | FFD,BFD,WFD | Cost of WFD |
|---|---|---|---|
| Baseline | 73.9μs | 170.7μs | **+131%** |
| BFD dominance | 119.5μs | 291.4μs | **+144%** |
| High efficiency | 353.0μs | 997.3μs | **+183%** |
| WFD weakness | 112.9μs | 357.3μs | **+217%** |
| Max complexity | 204.9μs | 968.8μs | **+373%** |

Roughly 2.3× to 4.7× the time for a third algorithm that rarely produces the winner. This is the evidence for
`$lib/decisions#D1`: the cost is paid **per bin**, so across a compare it multiplies by the bin count. One bin
can afford it; many bins cannot.

Reproduced 2026-07-17 against the 2026-07-14 run (which predates the cancellation-token commit): the effect
holds in the same shape and size on every scenario.

## F2 — parallel *algorithm* racing is marginal on the production set

`Parallel` vs `Loop`, racing FFD+BFD on one bin — the set production actually uses:

| Scenario | Loop | Parallel | Speedup |
|---|---|---|---|
| Baseline | 73.9μs | 79.2μs | **0.93× (slower)** |
| High efficiency | 353.0μs | 343.4μs | 1.03× |
| WFD weakness | 112.9μs | 104.7μs | 1.08× |
| Max complexity | 204.9μs | 186.7μs | 1.10× |
| BFD dominance | 119.5μs | 80.5μs | 1.48× |

**Two algorithms cap the win at 2× before any overhead**, and on the cheapest scenario the thread handoff
costs more than it saves. Parallel racing only clearly pays on `BFD dominance`, where the two algorithms take
very unequal time.

This is why `$lib/decisions#O1` is an open question rather than an obvious "wire it up": on the set production
uses, parallel racing is close to free but close to worthless. Wider sets look better (the discarded WFD-heavy
combinations gain more) — but those are the sets D1 rules out. **The decision that makes racing cheap is the
same decision that makes parallelising it pointless.**

Not measured here: `ParallelBinProcessor` (many bins at once), which scales with bin count rather than with
the number of algorithms. That one is F4 below.

### F2a — and the ladder says it never pays on FFD+BFD (2026-09-25)

`Full_Algorithms_Packing_v1` / `_v2`, the same Loop-against-Parallel question over the synthetic item ladder
instead of the curated problems. Report: `lib/results/benchmarks/baseline/threshold/` at commit `59e9dfb3`. `Parallel` ratio, v2:

| Items | 3 | 7 | 13 | 17 | 23 | 29 | 37 | 47 | 59 | 67 | 79 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| FFD,BFD | 2.55 | 1.87 | 1.54 | 1.54 | 1.36 | 1.32 | 1.64 | 1.11 | 1.16 | 1.46 | 1.34 |
| FFD,WFD,BFD | 2.14 | 1.54 | 1.21 | 1.13 | 1.26 | 1.42 | 1.15 | 1.14 | 0.96 | 1.08 | 1.01 |

**On the production set, `Parallel` loses at every step of the ladder** — 1.11× to 2.55×, and every ratio is
many times its own `RatioSD` (0.01 to 0.14). Three algorithms never do better than parity, and only at the top
of the ladder (0.96 to 1.08 from 59 items). Parallel also allocates more everywhere: 1.59× at 3 items down to
1.04–1.05× at 79.

This does not contradict F2. F2's curated problems start at 126 items and include `BFD dominance`, where the
two algorithms take very unequal time and parallel won 1.48×. The ladder has no such case, so the small end
shows the overhead with nothing to hide behind. Together: **parallel algorithm racing pays only on a problem
where one algorithm is much slower than the other, and costs on every other.** v1 behaves the same way
(1.16× to 2.41× on FFD,BFD), so this is not a v2 regression.

## F4 — parallel *bins* pays, above a surface rather than a threshold (2026-09-25)

`Full_Bins_Packing_v1` / `_v2`, one algorithm over bins 1 to 7 and the whole 11-step item ladder, 308 cases
each at the default job. Reports: `lib/results/benchmarks/baseline/threshold/` at commit `59e9dfb3`. This is the axis F2 could not
see, and it is the answer to `$lib/decisions#O1`.

`Parallel` ratio against `Loop`, v2 — **the shipped version**. Under 1.00 means parallel is faster:

FFD:

| Items \ Bins | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
|---|---|---|---|---|---|---|---|
| 3 | 4.52 | 3.09 | 2.51 | 2.11 | 1.96 | 1.89 | 1.86 |
| 7 | 2.90 | 2.24 | 1.84 | 1.65 | 1.33 | 1.47 | 1.35 |
| 13 | 1.90 | 1.70 | 1.51 | 1.30 | 1.14 | 1.21 | 1.13 |
| 17 | 1.61 | 1.65 | 1.38 | 1.22 | 1.10 | 1.09 | **0.98** |
| 23 | 1.46 | 1.38 | 1.21 | 1.14 | **0.97** | 0.94 | 0.90 |
| 29 | 1.41 | 1.25 | 1.13 | 1.02 | **0.92** | 0.91 | 0.83 |
| 37 | 1.27 | 1.05 | **0.92** | 0.89 | 0.80 | 0.80 | 0.76 |
| 47 | 1.22 | **0.96** | 0.90 | 0.79 | 0.74 | 0.72 | 0.68 |
| 59 | 1.25 | 1.63 | 1.19 | **0.93** | 0.84 | 0.76 | 0.70 |
| 67 | 1.23 | 1.50 | 1.27 | 1.04 | **0.87** | 0.78 | 0.68 |
| 79 | 1.16 | 1.17 | 1.09 | **0.86** | 0.75 | 0.67 | 0.60 |

BFD:

| Items \ Bins | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
|---|---|---|---|---|---|---|---|
| 3 | 4.60 | 2.71 | 2.40 | 2.15 | 1.89 | 1.91 | 1.71 |
| 7 | 2.84 | 1.91 | 1.76 | 1.53 | 1.38 | 1.38 | 1.23 |
| 13 | 1.64 | 1.53 | 1.39 | 1.19 | **0.99** | 1.03 | 0.92 |
| 17 | 1.45 | 1.51 | 1.13 | 1.06 | **0.87** | 0.87 | 0.84 |
| 23 | 1.02 | 1.36 | 1.02 | **0.93** | 0.83 | 0.75 | 0.71 |
| 29 | 1.50 | 1.32 | **0.97** | 0.80 | 0.71 | 0.76 | 0.60 |
| 37 | 1.52 | 1.32 | 1.21 | **0.84** | 0.54 | 0.48 | 0.58 |
| 47 | 1.11 | 1.14 | **0.85** | 0.71 | 0.68 | 0.59 | 0.54 |
| 59 | 1.13 | 1.38 | **1.00** | 0.78 | 0.82 | 0.64 | 0.63 |
| 67 | 1.39 | 1.21 | **0.76** | 0.97 | 0.71 | 0.72 | 0.55 |
| 79 | 1.46 | 0.87 | **0.84** | 0.70 | 0.74 | 0.56 | 0.52 |

Bold is the first bin count on each row where parallel wins. `RatioSD` is 0.08 or less on every cell at 3 bins
and up from 7 items, and up to 0.13 at 3 items, so a ratio outside 0.95–1.05 is a real effect; the 1-bin column wobbles more (up to 0.17) because the
parallel overhead is most of the measurement there.

**There is one threshold, not two, and it is a surface.** Parallel's cost is a fixed thread setup of roughly
1.3μs plus a share of the slowest bin; `Loop`'s cost is the sum of all bins. So parallel needs enough bins to
divide *and* enough work per bin to be worth dividing, and either one alone is not enough:

- **1 bin is always a loss** — 1.02× to 4.60×, worst on the smallest requests. This is the guard the harness
  was built for, and it fires: wiring parallel up unconditionally would make every `fit/bin` and `pack/bin`
  request slower.
- **3 and 7 items never win, at any bin count up to 7.** At 3 items parallel is still 1.7× to 1.9× slower with
  7 bins to divide.
- **From 13 items the crossover walks in**: FFD wins from 7 bins at 17 items, 5 at 23, 3 at 37, 2 at 47. BFD,
  the more expensive algorithm, crosses about two bins earlier — 5 bins at 13 items, 3 at 29, 2 at 79.
- **At the top corner the win is real**: 0.60 (FFD) and 0.52 (BFD) at 79 items over 7 bins, on 12 cores.

**Allocation is the price and it is small at size.** Parallel allocates more in every one of the 308 cases,
from **2.15× (3 items, 1 bin) down to 1.03× (79 items, 5 to 7 bins)**. It falls along both axes: at 3 items,
2.09× at 1 bin to 1.27× at 7; at 79 items, 1.13× at 1 bin to 1.03× at 7. The overhead is per run, not per
item, so it disappears into a big request and dominates a tiny one — the same shape as the time. **On the 67 of
308 cases where parallel wins on time, it costs at most 1.12× the allocation.**

**The faster the algorithm gets, the further out the threshold moves.** At the small end v1 crosses over
sooner: FFD at 13 items wins from 7 bins in v1 and never in v2; FFD at 17 wins from 5 bins in v1 and 7 in v2;
BFD at 7 items reaches 0.97 at 7 bins in v1 and 1.23 in v2. v2 roughly halved the per-bin work, so the same
fixed overhead now covers more of the run. **At the top corner the two land together** — 0.61 (v1) against 0.60
(v2) for FFD at 79 items over 7 bins, 0.50 against 0.52 for BFD — because there the per-bin work dwarfs the
overhead in both. Above 37 items the crossover column is not stable enough to rank the versions: it moves by a
bin or two either way, which is the ladder's uneven bins showing through. A future speedup pushes the small end
of the threshold further out.

**The bins are not identical.** Each added bin is taller than the last (60x40x10 up to 60x40x40), so bin count
and total work grow together and the parallel run is bounded by the largest bin. A request with seven bins of
one size would divide better than this grid shows.

**What the prior got wrong.** The November 2025 records below said `Parallel` was under 1.0 from 2 bins up on
every machine, about 0.85 at 2 bins. The top end holds — 0.65 at 8 bins then is 0.52–0.70 at 7 bins from 47 items now — but
the small end does not. **At 2 bins parallel wins only at 47 items for FFD and only at 79 for BFD**,
and **3 and 7 items lose at every bin count**. The old runs used one fixed item set, and it must have been a
heavy one.

Not covered: more than 7 bins, item counts above 79, and a machine with fewer cores. Loop-against-Parallel
only compares on the same core count.

## F5 — test-only v3 of BFD and WFD: one scan instead of a sort (2026-09-27)

v3 differs from v2 in one method: it finds the smallest (BFD) or largest (WFD) space that fits in one pass,
where v2 sorts the free spaces on every placement. It fills all 700 Bischoff problems exactly as v2
(`lib/results/measurements/version-parity.md`) and allocates the same.

Smoke run, short job, one process per case - v3 time as × v2:

| | full bin, one type | small order | typical container | many item types |
|---|---|---|---|---|
| BFD packing | 0.85× | 0.73× | 0.21× | 0.24× |
| WFD packing | 0.21× | 0.70× | 0.31× | 0.16× |

The gain grows with the number of free spaces. On packing, BFD v3 now takes 0.94× to 1.09× of FFD v2's time and WFD v3 1.07×
to 1.65×. Four scenarios only; the full run over 700 is not done.

## F6 — fill per algorithm over the 700 Bischoff problems

`lib/results/measurements/packing-efficiency.md`, written by `just measure lib`. Fill is the packed volume as
a share of the bin, v2.

| Algorithm | Min | Mean | Median | Max |
|---|---|---|---|---|
| FFD | 56.18 | 73.41 | 73.47 | 87.90 |
| WFD | 49.15 | 69.21 | 69.05 | 87.90 |
| BFD | 62.08 | 81.26 | 81.48 | 90.66 |

- **Best of FFD and BFD — the pair the API races — is 81.30 mean. Best of all three is 81.33.** WFD adds
  0.03 points. This is the fill side of `$lib/decisions#D1`: F1 says WFD multiplies the cost of a race by 2.3
  to 4.7, and this says it buys three hundredths of a point.
- **Best or tied: BFD on 669 of the 700, FFD on 35, WFD on 35.**
- **BFD leads in every set**, BR1 (3 item types) through BR7 (20): 80.17 to 82.12. FFD and WFD lose ground as
  item types grow — FFD 75.70 at BR1 down to 71.63 at BR7, WFD 74.78 down to 67.41.

## F7 — v1 and v2 pack the same, with one exception

`lib/results/measurements/version-parity.md`: all 700 problems pack to the same fill under FFD v1 and v2, and
under WFD v1, v2 and v3. BFD differs on one — `OrLibrary_thpack7_45`, 79.08 in v1 against 79.73 in v2 and v3.
So the v2 rewrite is a speed and memory change, not a packing change.

## F8 — v2's memory gain is the free-space sort (2026-09-25)

`lib/results/benchmarks/baseline/algorithms/Full_FFD_Packing.md`, `Full_WFD_Packing.md` and
`Full_BFD_Packing.md` — every Bischoff problem, v1 against v2, packing, short job. Mean of BenchmarkDotNet's
per-problem `Alloc Ratio`:

| Algorithm | v2 allocation as × v1 |
|---|---|
| FFD | 0.38 |
| WFD | 0.05 |
| BFD | 0.08 |

**WFD and BFD gain twenty times what FFD gains, and the reason is one line.** BFD v1 and WFD v1 pick a space
with `availableSpace.OrderBy(...)` in
`lib/src/Binacle.Lib/Algorithms/Best Fit Decreasing v1/AlgorithmOperation.cs` and
`lib/src/Binacle.Lib/Algorithms/Worst Fit Decreasing v1/AlgorithmOperation.cs`, which builds a new sorted copy
of the free-space list for every item and every orientation. v2 sorts the list in place with `List.Sort`. FFD
never sorted spaces at all, so it only drops to 0.38×.

`Allocated` is memory handed out during one pack, garbage included — not peak memory.

## F9 — what `Parallel.For` costs around a race (2026-10-02)

`Binacle.Lib.Benchmarks.ParallelOverhead` runs the processors with algorithms that take no measurable time, so
a row is what the processor costs around them rather than the gap between two packing rows of similar size.
Built on the maintainer's "lets make the first and 3rd theese we can measure cleanly and get them down
properly" (2026-10-01). Reports: `lib/results/benchmarks/baseline/parallel-overhead/`, default job.

Read the rows as gaps: `Loop` to `Parallel_OneThread` is `Parallel.For`'s own setup; `Parallel_OneThread` to
`Parallel` is the cost of handing the work to another CPU.

**The curve is U-shaped and its floor is 2 cores.** `Parallel`, nothing to walk:

| Cores | Two algorithms | Three algorithms |
|---|---|---|
| 1 | 1,244 ns | 1,292 ns |
| 2 | 693 ns | 827 ns |
| 4 | 781 ns | 1,019 ns |
| 8 | 1,078 ns | 1,381 ns |
| 12 | 1,131 ns | 1,442 ns |

**One CPU is the dearest, not the cheapest.** `Parallel.For` still hands the work to a worker thread, and with
one CPU that worker waits for the caller to be taken off it. The maintainer kept the 1-CPU job for this —
"2 keep it we need a baseline maybe the run was noisy maybe it was a dry run" (2026-10-02) — and it is the one
row that shows the case the degree cap exists to prevent: there an uncapped two-algorithm race costs 1,244 ns
against the loop's 76.

**The cost splits three ways**, two algorithms: the loop itself about 37 ns per algorithm, `Parallel.For`'s own
setup +218 ns, and handing the work to another CPU +400 ns at 2 cores rising to +834 ns at 12. **Only the third
moves with the core count.**

**`Parallel_OneThread` and `Loop` are both deaf to the core count** — 290, 292, 291, 290, 297 ns and 76, 73,
73, 73, 74 ns across the five counts. That is what makes `Parallel_OneThread` the reference line the other two
rows are read against.

**Most of the hand-off is the work's memory, not the dispatch.** With a 64 KB working set the
`Parallel_OneThread`-to-`Parallel` gap goes from 401 to 1,368 ns at 2 cores, and from 834 to 2,994 ns at 12.
With bins it grows faster still — `Fake_Bins` at 12 cores reads 635 ns at 1 bin and 2,167 ns at 8 bins with
nothing to walk, against 1,227 ns and 13,684 ns with 64 KB per bin. **So the setup term is a function of the
request**, not one number per machine: "parallel costs its setup plus the slowest bin" understates it.

`Loop` is below `Parallel_OneThread` is below `Parallel` in every row of all three reports, which is the
ordering that has to hold, and no cell came back `NA`.

Noise: the worst StdDev is 5.6 per cent of mean in `Fake_TwoAlgorithms`, 7.3 in `Fake_ThreeAlgorithms` and 8.6
in `Fake_Bins` — all of them `Parallel` rows, which is the row that varies. `Loop` and `Parallel_OneThread` sit
near 1 per cent throughout.

Not covered: where the work lands. `maxDegreeOfParallelism` caps how many concurrent tasks run, not which CPU
runs them, and .NET has no managed way to place work that runs on pooled threads.

## F10 — parallel *bins*: the drop point is a surface and the ceiling is about 2× (2026-10-03)

`Identical_FFD_Pieces_Packing` — identical bins, identical items, FFD, bins 1 to 32 by pieces 1 to 256, on 2,
4, 8 and 12 cores. Report: `lib/results/benchmarks/baseline/parallel-bins/Identical_FFD_Pieces_Packing.md`,
short job. Ratio is BenchmarkDotNet's, `Parallel` against `Loop`. This is the axis `$lib/decisions#O1` asks
about, measured on a synthetic case instead of F4's item ladder.

The piece count where `Parallel` first wins:

| Bins | 2 cores | 4 cores | 8 cores | 12 cores |
|---|---|---|---|---|
| 1 | never | never | never | 128 |
| 2 | 32 | 32 | 32 | 32 |
| 4 | 8 | 16 | 16 | 32 |
| 8 | 4 | 4 | 16 | 32 |
| 16 | 2 | 2 | 8 | 16 |
| 24 | 1 | 1 | 4 | 8 |
| 32 | 1 | 1 | 4 | 4 |

**More bins win earlier; more cores win later**, with no exception in either direction. The 1-bin row is the
control — there is nothing to parallelise, and its single cell at 0.97 is noise.

**`Parallel` never captures more than about 2×, however many bins.** Best ratio at 12 cores:

| Bins | best ratio | perfect scaling | what it got |
|---|---|---|---|
| 2 | 0.63 | 0.50 | 1.6× of a possible 2× |
| 4 | 0.61 | 0.25 | 1.6× of a possible 4× |
| 8 | 0.57 | 0.12 | 1.8× of a possible 8× |
| 16 | 0.54 | 0.08 | 1.9× of a possible 12× |
| 24 | 0.58 | 0.08 | 1.7× of a possible 12× |
| 32 | 0.53 | 0.08 | 1.9× of a possible 12× |

Across all four core counts the best cell of each bin count runs 0.50 to 0.62. **The ceiling does not move with
the bin count**: 2 bins and 32 bins both stop near 0.55, so going wider buys an earlier drop point and nothing
else. It is not the bin count, the core count or the request size that stops it there; nothing measures the
cause yet.

**`Bins = 1` is the degree cap's receipt.** With one bin the cap resolves to 1 and `Parallel.For` runs the body
inline, so `Parallel` sits a small step above `Loop` that does not grow with the core count — **230 to 302 ns
up to 16 pieces**. Read it only there. At 32 pieces the step already scatters from 98 to 338 ns because it is
becoming small next to the baseline, and past 64 pieces it is under the noise and goes negative. Do not read
the row as a ratio either: the same step is 1.79 to 1.98 at one piece, where the baseline is only 264 to
294 ns.

Noise: `Loop` StdDev is 0.33 per cent of mean at the median, 0.89 at the 90th percentile, worst 6.99.
`Parallel` is 1.72, 5.49 and 14.00.

**Read the drop point in time per bin, not in pieces.** The case's box is an exact multiple of the item, so
every piece takes the first free space — the cheapest work per piece any request can have. A piece count read
off it sits too low for mixed items.

Not covered: WFD and BFD, more than 32 bins, more than 256 pieces, and anything but identical bins and items.

## F11 — lines cost almost nothing once they are pieces (2026-10-03)

`Identical_FFD_Lines_Packing` — 4 bins, 64 and 256 pieces spread over 1, 4, 16 and 64 lines. Report:
`lib/results/benchmarks/baseline/parallel-bins/Identical_FFD_Lines_Packing.md`, short job. A class per
algorithm exists to prove this negative rather than argue it — the maintainer, 2026-09-30: "for bins we need 3
one for each algotirhm to see how algorithms change behaviour".

`Loop`, 64 lines against the same pieces in one line:

- **At 256 pieces: flat** — −0.9 to +1.8 per cent across the four core counts, inside the noise.
- **At 64 pieces: +3.6 to +10.3 per cent**, or +0.6 to +1.6 μs on a 16 μs baseline. `Loop` StdDev is 0.8 per
  cent of mean at the median and 2.0 at the 90th percentile, so this one is above the noise.

**It is exactly where it was predicted.** The v2 algorithm constructor calls `CalculateVolume` and
`CalculateLongestDimension` once per line, outside the quantity loop (`Algorithm.cs` in each v2 family under
`lib/src/Binacle.Lib/Algorithms/`), so 64 lines of quantity 1 pays 64 of them where one line of 64 pays one.
It shows only at the small piece count, where there is nothing else to hide it.

So a rule can read pieces — lines do not change the shape of the answer. But lines are not free: a request of
many single-quantity lines costs a few per cent more than the same pieces in one line.

## Note — the old `MultipleBins` records (November 2025, records deleted)

Kept here because the files are gone. Ratios were recomputed per algorithm by script on 2026-09-21.
`Parallel` was under 1.0 from 2 bins up on every machine: about 0.85 at 2 bins, 0.65 at 8, flat after. Per-bin
weight moves the low end (2 bins: FFD 0.82, WFD 0.60). The runs measured 2, 8, 14 .. 38 bins on one fixed item
set - never 1 bin, never 3 to 7, never a change in per-bin weight. F4 above closed that gap and
found the small end is a loss, so these numbers are superseded; they are kept only because they are the only
record above 7 bins.

## Note — why the Threshold and Racing benches are shaped as they are (2026-09-21)

- **Racing had no `ProcessorCount` parameter.** The parallel processor runs `Parallel.For` over the
  algorithms, so a race of N algorithms uses at most N threads; the set already decides it. BDN's header
  prints the machine's cores.
- **Racing varies the core count as jobs (2026-09-26).** At or above N cores the count should matter little;
  below it - three algorithms on 2 cores - it does. `Cores_Packing` runs on 2, 4, 8 and 12. A parameter cannot
  do it: .NET reads its CPU count once at start-up, so each count is a BDN job with an affinity mask and
  `DOTNET_PROCESSOR_COUNT`. On Linux BDN's mask reaches only the main thread, so the case pins every thread
  itself before it checks.
- **Threshold keeps the algorithms family** though it overlaps Racing only at its top (ladder max 79 items;
  the lightest racing problem is 74), and old records over 10 to 202 items were flat at 0.85 to 1.0. It
  covers the small-request end (demo samples: median 13 items) and the 67 -> 79 step, where the algorithms
  take unequal time. In the kept run `Parallel` loses at every step for FFD,BFD (1.11× to 2.55×), and
  FFD,WFD,BFD wins only at 59 items (0.96).
- **Full keeps the whole item ladder** though the extra points only interpolate, so one record of the curve
  exists.
- **Param names are what BDN prints** - there is no display attribute.

## Note — the cancellation-token guard has no measurable cost

`8a7580f3` added `cancellationToken.ThrowIfCancellationRequested()` to each loop iteration in
`LoopAlgorithmProcessor` and friends — the exact hot path these benchmarks measure.

Comparing FFD,BFD `Loop` before (2026-07-14) and after (2026-07-17): **−2.0%, −1.4%, −0.7%, +2.0%, +6.2%**
across the five scenarios. **The signs are mixed**, which is the point: a real per-iteration tax would push
every scenario the same way. This is run-to-run variance, not a regression.

**Caveat:** these are two separate runs on different days, not a controlled A/B, and the machine was in use
during the second. Within-run error is ~1%, so the ±2% moves are between-run variance. If a definitive answer
is ever needed, stash the guard and run both back to back.

## Note — BestAlgorithm v2 is slower than v1 where v1 stops early (2026-09-22)

`Binacle.Lib.Benchmarks.ResultSelection`, ShortRun job, three candidates per scenario, same machine as above
on .NET 10.0.12. v1 returns the first `FullyPacked` result; v2 scores every candidate. So where a full result
exists v1 is faster: `one full winner` 4.4 ns vs 6.0 ns (1.37×), `all full, first wins` 3.6 ns vs 6.2 ns
(1.74×). With no full result v1 sorts and v2 wins: `all partial` 24.9 ns vs 6.1 ns. The kept run of 2026-09-23
(`lib/results/benchmarks/baseline/result-selection/BestAlgorithm.md`) reads 4.08 vs 6.16 ns (1.51×), 3.54 vs
5.95 ns (1.68×) and 25.45 vs 6.03 ns. v2 allocates 24 B on every
case. These are nanoseconds on three candidates, noise next to a packing run. In the same run BestBin and
SmallestBin v2 are 2.4-9× faster than v1.

## F3 — v3 fitting on the packing lineage matches the old fitting family

The v3.0.0 release unified fitting and packing onto one algorithm: fitting stopped running its own family
(`Binacle.Lib/Fitting/Algorithms/*/`, version 3) and now runs the packing lineage (version 2) with early exit.
Fitting answers a yes/no question with a heuristic, so a different heuristic could disagree on edge cases —
worth confirming because it sits inside the **frozen v3 contract**.

Differential-tested 2026-07-19 against the real `binacle/binacle-net:2.1.1` image: **~5,400 fit requests** (random
bins/items, all three algorithms, weighted to the near-full boundary where heuristics diverge) ran old vs new
side by side — **identical answers every time, zero disagreements**. No behaviour change; no release-notes caveat
needed. (Old ViPaq tokens rejecting loudly is the other v3 verification — that one lives in `vipaq/PROTOCOL.md`
plus the committed regression vectors, not here.)
