---
description: Session 1 - some bench runs come out slow at random, a whole process at a time, and no report shows it; find why with a small test, then choose the fix with the maintainer
state: ready
waits-on: "the maintainer says when"
horizon: undecided
paths: ["lib/bench/**", "vipaq/bench/**", "shared/test/Binacle.Benchmarking/**", "lib/src/Binacle.Lib/Algorithms/**"]
---

# 1 - Slow runs

## The problem

BenchmarkDotNet runs each case in one process. Some processes run slow from start to end, at random, with a
tight StdDev inside them. One slow process is one wrong number, and nothing in the report shows it. Every
time a results file would read is open to it.

What the kept runs show (checked by agents 2026-09-26, from the reports):

- **Racing, the precise run of 2026-09-26.** Its report is in the gitignored
  `lib/bench/Binacle.Lib.Benchmarks.Racing/BenchmarkDotNet.Artifacts/kept-results/`. The same algorithm alone, on
  another core count, jumps more than 1.3×: BFD on 10 of 120 cases, WFD on 1, FFD on 0. th3_29 BFD is 215 us on
  4 and 12 cores, 370 us on 2 and 8. Slow and fast processes allocate the same bytes.
- **Racing, the loops.** FFD+BFD Loop is more than 1.3× the sum of FFD and BFD alone on 33 of 120 cases; it should
  be about 1.05×. The three-algorithm Loop is 1.15× to 2.2× its three alone rows on every case - steady, not
  random, so it may be a second cause.
- **Algorithms full.** Three BFD v2 times are 1.45× to 1.53× the fast racing value: th3_17 86.0 against 59.5 us,
  th4_2 122.8 against 80.5, th3_23 172.8 against 112.6. That rate, about 1 in 10, would put about 70 of the 700
  BFD v2 times on a slow process.
- **Algorithms sample.** 8 of 60 BFD packing rows differ from the full run by more than 25%. v1 is hit too:
  thpack2_33 v1 is 196 against 123 us.
- **Threshold.** The three-algorithm Loop is 1.18× to 1.85× its parts in the sample run, 1.26× to 1.46× in the
  full run.
- **ViPaq sample, rerun 2026-09-26 with no ViPaq code change.** `ViPaq_Row` on `5000 items, 8-bit` went from 183
  to 279 us; `ViPaq_Columnar` on `65535 items, 8-bit` from 4.20 to 3.41 ms. The protobuf decode baseline moved
  too: 2,361 to 2,550 ns on `100 cubes, 8-bit`. ViPaq has no sort anywhere.

## One guess, not proven

.NET tunes code while it runs (tiered PGO). The guess: it tunes the shared sort code for the comparison it sees
first. BFD re-sorts its free spaces on every placement
(`lib/src/Binacle.Lib/Algorithms/Best Fit Decreasing v2/AlgorithmOperation.cs:68`); if the tuning caught
another sort first, BFD is slow for the whole process. FFD never sorts in its loop and never jumps. It would
also explain the steady three-algorithm Loop: WFD and BFD share one process, so one of them is always slow.

The agent that made the guess put it at about 60%. **Against it:** ViPaq shows the same fault with no sort. If
tiered PGO is ruled out, code alignment is the next suspect.

## The small test - no code change, short job

Run it twice: as below, then with `DOTNET_TieredPGO=0` in front of `dotnet run` and `pgo-off` in place of `pgo-on`
in the folder. Child processes take the variable.

```
for i in 1 2 3 4 5; do dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Racing -- --job short --artifacts lib/bench/Binacle.Lib.Benchmarks.Racing/BenchmarkDotNet.Artifacts/pgo-on-$i --filter '*Cores_Packing.BFD(*"th3_29 *' '*Cores_Packing.BFD(*"th6_39 *'; done
```

That is 40 BFD processes per pass. PGO is the cause if:

- **PGO on:** a mix of about 215 and 370 us on th3_29, and about 118 and 175 to 213 us on th6_39.
- **PGO off:** no spread over 1.3×.

Then one PGO-off pass with `--filter '*"th3_13 *'`: if `Loop_FFD_WFD_BFD` is 1.1× or less of its three alone
rows, the loop fault has the same cause.

Then ViPaq, five launches with PGO on and five off:

```
for i in 1 2 3 4 5; do dotnet run -c Release --project vipaq/bench/Binacle.ViPaq.Benchmarks -- --job short --artifacts vipaq/bench/Binacle.ViPaq.Benchmarks/BenchmarkDotNet.Artifacts/pgo-on-$i --filter '*Sample_Encode.ViPaq_Row(*"5000 items, 8-bit"*'; done
```

If PGO off steadies BFD but not ViPaq, there are two causes, or it is alignment.

## The choice, the maintainer's

Two ways, not exclusive. The test says which are open.

1. **Fix the cause in the code.** For the guess above: take the shared lambda out of the hot sort, so each
   algorithm gets sort code of its own - `SpaceVolume` as a readonly struct, or one scan for the smallest (BFD)
   or largest (WFD) space instead of a sort. It changes the product. `List.Sort` is unstable, so it may change
   which of two equal spaces wins: the result tests must pass and the measurements must not move. Every v2 BFD
   and WFD time is retaken after it.
2. **Make the bench show it.** Run each case in several processes (BenchmarkDotNet's launch count), so a slow
   process shows as spread instead of hiding. No product change. Every run takes that many times longer:
   algorithms full is 12 to 16 hours at one launch.

## Done when

- [ ] The small test ran, and its outcome is written here: the cause, or "not found".
      **By eye.**
- [ ] The maintainer chose the fix, written here with his reason.
      **By eye.**
