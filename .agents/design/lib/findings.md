---
id: lib/findings
description: Lib findings — the measured evidence (algorithm racing cost, parallel algorithm racing, parallel bin processing) behind the decisions.
verified: 2026-09-25
check: the five keys in RacingSet (typical container, BFD wins big, near tie, WFD falls over, many item types) still exist, and BenchmarkBase in lib/bench/Binacle.Lib.Benchmarks.Racing still races the quoted algorithm sets over them (the classes that ran it were deleted 2026-09-26; their reports are in lib/results/benchmarks/baseline/racing/); Cores_Packing still races both sets and each algorithm alone on the four core jobs in CoreJobs; 8a7580f3 is still the commit that added ThrowIfCancellationRequested to the lib processors; the fitting family under lib/src/Binacle.Lib/Fitting/ is still gone; BinsBase and the four Full_ classes in Binacle.Lib.Benchmarks.Threshold still carry the item ladder and bins 1 to 7, and LadderGenerator still grows the bin taller per step. F1, F2 and the notes are not re-checkable from the repo - see Environment. F2a and F4 are: their reports are in lib/results/benchmarks/baseline/threshold/.
also_update:
  - lib/decisions
paths:
  - "lib/**"
---

# Lib — findings (the measured evidence)

The measured truth behind `$lib/decisions`. **No session keeps its own numbers** — they live here.

Ranges show the effect, not a guarantee.

## Environment

Two runs, both on an AMD Ryzen 9 9900X with 12 physical cores, Linux Ubuntu 26.04, BenchmarkDotNet v0.15.8.
Times compare within one run only; ratios and `Allocated` compare across both.

| Run | What it produced | Runtime |
|---|---|---|
| 2026-07-17, `AlgorithmRacing_Packing_v2` | F1, F2 | .NET 10.0.9 |
| 2026-09-25, `just bench lib-threshold-full precise` | F2a, F4 | .NET 10.0.12, default job |

Within-run error is **0.3–1.0%** of the mean on the 2026-07-17 run, so the effects below are far outside the
noise.

**F1 and F2 cannot be re-checked from a clone.** BenchmarkDotNet writes to `BenchmarkDotNet.Artifacts/`, which
`.gitignore` excludes, and nobody kept those reports - so what a later session can confirm is that the harness
still races what is quoted here, not that a re-run would land on the same microseconds. **F2a and F4 were
kept**: every number in them can be read out of `lib/results/benchmarks/baseline/threshold/`.

**The scenario names below are the ones the run printed.** The keys in `RacingSet` were
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
instead of the curated problems. Report: `lib/results/benchmarks/baseline/threshold/`. `Parallel` ratio, v2:

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
each at the default job. Reports: `lib/results/benchmarks/baseline/threshold/`. This is the axis F2 could not
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

Bold is the first bin count on each row where parallel wins. `RatioSD` is 0.01 to 0.07 on every cell at 3 bins
and up, so a ratio outside 0.95–1.05 is a real effect; the 1-bin column wobbles more (up to 0.17) because the
parallel overhead is most of the measurement there.

**There is one threshold, not two, and it is a surface.** Parallel's cost is a fixed thread setup of roughly
1.3μs plus a share of the slowest bin; `Loop`'s cost is the sum of all bins. So parallel needs enough bins to
divide *and* enough work per bin to be worth dividing, and either one alone is not enough:

- **1 bin is always a loss** — 1.16× to 4.60×, worst on the smallest requests. This is the guard the harness
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
every machine, about 0.85 at 2 bins. The top end holds — 0.65 at 8 bins then is 0.60–0.68 at 7 bins now — but
the small end does not. **At 2 bins parallel loses at every item count below 47 for FFD and below 79 for BFD**,
and **3 and 7 items lose at every bin count**. The old runs used one fixed item set, and it must have been a
heavy one.

Not covered: more than 7 bins, item counts above 79, and a machine with fewer cores. Loop-against-Parallel
only compares on the same core count.

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
- **Threshold keeps the algorithms family** though it is lighter than Racing everywhere (ladder max 79 items;
  the lightest curated problem is 126), and old records over 10 to 202 items were flat at 0.85 to 1.0. It
  covers the small-request end (demo samples: median 13 items) and the 67 -> 79 step, where the algorithms
  take unequal time. 3 items is the one place `Parallel` loses: thread cost dominates.
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
(1.74×). With no full result v1 sorts and v2 wins: `all partial` 24.9 ns vs 6.1 ns. v2 allocates 24 B on every
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
