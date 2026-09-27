---
description: Session 5 - build and run a bins bench that finds where packing many bins at the same time starts to pay, on bins of one size, 2 to 16 bins, 2 to 12 cores; shape agreed, not built
state: blocked
waits-on: "session 4 - the racing run and what it teaches"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.Threshold/**", "lib/bench/Binacle.Lib.Benchmarks.Racing/**", "lib/test/Binacle.Lib.Testing/**", "shared/test/Binacle.Benchmarking/**"]
---

# 5 - The bins drop point

Same aim as the racing drop point, for `ParallelBinProcessor`: where packing the bins at the same time starts to
win, and the point from which it always wins by a meaningful amount. Built after racing - **decided (the
maintainer, 2026-09-25):** "lets make the algo race first".

## What the session does

1. Agree the 12 problems with the maintainer.
2. Build the bins bench, pinned as the racing bench is.
3. A small run first; then the maintainer runs the full one and the session keeps it.
4. Read the drop point, or say there is none.

`parallel-bins.md` is the maintainer's to shape; session 6 builds it.

## Why the kept bins run cannot answer it

`Full_Bins_Packing_v2` jumps around: FFD wins at 47 items on 2 bins (0.96) and loses at 59 (1.63). Every step
of that grid changed two things at once. One more bin was also a taller bin (60x40x10 up to 60x40x40), and from
59 items the ladder's total no longer fits the small bins, so some bins stop early and some do not. The grid
stays as the record; the new test changes one thing at a time.

## The shape - leaning yes

**(the maintainer, 2026-09-25):** "yes i think yes... but it does worry me how we can test reliably with
different bin shapes when i failed with the ladder".

- FFD and BFD, loop against parallel, v2 only, packing.
- On 2, 4, 8 and 12 cores, pinned as the racing bench is.
- 2, 4, 8 and 16 bins, **all one size**: the problem's own bin, copied. No 1 bin: the rule returns loop. 16 is
  more bins than cores.
- Over 12 problems. **Suggested (agent, 2026-09-25):** 12 of the racing 30, so the two results sit side by
  side. Dropped by the proposal below: the 30 are too big to show where parallel starts to win.
- 12 x 4 bin counts x 2 algorithms x 2 x 4 core counts = 768 cases: about 1 hour at the short job, 3 to 4 at
  the default one.

## The 12 problems - suggested (agent, 2026-09-28), not agreed

The racing 30 cannot find the drop point alone. The old bins grid had parallel winning at about 5 to 18 us per
bin on 2 bins, 2 to 3 us on 4 bins, 1.5 to 2 us on 7 bins. The smallest of the 30 is 7 us. So five smaller
real problems come in from the demo samples and custom problems, and seven stay from the racing 30.

Each bin gets the whole item list and, in packing, no bin stops early, so the work per bin is the problem's
one-bin time. Rows 1 to 5 are estimated from item count; the rest are FFD / BFD from the Full_ reports.

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

## Open - asked by the maintainer, not answered

- **Does the new bins class replace the old bins classes and their kept reports?** He asked on 2026-09-28 why
  the threshold bench changes, and whether the new is added to the old. The old are `Smoke_Bins_Packing`,
  `Sample_Bins_Packing`, `Full_Bins_Packing_v1` and `Full_Bins_Packing_v2`.

## Suggested (agent), not agreed

- **A mix test after it:** same problems, bin count and cores; only the bin sizes change - all one size, or
  mixed (full, three-quarters, half). Parallel pays for the slowest bin, loop for all of them, so a mix comes
  down to how much of the work sits in the biggest bin. If the mix moves the drop point little, the rule
  ignores bin sizes.
- **`MaxDegreeOfParallelism` as a knob the rule turns**, tested here. It does not imitate a smaller machine.
  `ParallelBinProcessor` takes a `concurrencyLevel` that only sizes its dictionary and never reaches
  `MaxDegreeOfParallelism`; fix that before any rule relies on it.

## Done when

- [ ] The bins bench runs the shape above, and the maintainer has run it and kept the report.
      **By eye.** A report of the new class under `lib/results/benchmarks/*/threshold/`.
- [ ] The drop point is read out of it, or the report shows there is none, and the lib findings record says so.
      **By eye.**
