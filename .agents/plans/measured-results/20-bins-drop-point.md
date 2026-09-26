---
description: Step 20 - a bins bench that finds where packing many bins at the same time starts to pay, on bins of one size, 2 to 16 bins, 2 to 12 cores; shape agreed, not built
state: blocked
waits-on: "step 19's run and what it teaches - the maintainer says when"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.Threshold/**", "lib/test/Binacle.Lib.Testing/**"]
---

# Step 20 - the bins drop point

Same aim as step 19, for `ParallelBinProcessor`: where packing the bins at the same time starts to win, and the
point from which it always wins by a meaningful amount. Built after racing (the maintainer, 2026-09-25).

## Why the kept bins run cannot answer it

`Full_Bins_Packing_v2` jumps around: FFD wins at 47 items on 2 bins (0.96) and loses at 59 (1.63). Every step
of that grid changed two things at once. One more bin was also a taller bin (60x40x10 up to 60x40x40), and from
59 items the ladder's total no longer fits the small bins, so some bins stop early and some do not. The grid
stays as the record; the new test changes one thing at a time.

## The shape agreed (2026-09-25)

- FFD and BFD, loop against parallel, v2 only, packing.
- On 2, 4, 8 and 12 cores, pinned as step 19 does.
- 2, 4, 8 and 16 bins, **all one size**: the problem's own bin, copied. No 1 bin: the rule returns loop. 16 is
  more bins than cores.
- Over 12 of step 19's 30 problems, so a racing and a bins result sit side by side. Which 12 is not picked.
- 12 x 4 bin counts x 2 algorithms x 2 x 4 core counts = 768 cases: about 1 hour at the short job, 3 to 4 at
  the default one.

## Proposed, not agreed

- **A mix test after it:** same problems, bin count and cores; only the bin sizes change - all one size, or
  mixed (full, three-quarters, half). Parallel pays for the slowest bin, loop for all of them, so a mix comes
  down to how much of the work sits in the biggest bin. If the mix moves the drop point little, the rule
  ignores bin sizes.
- **`MaxDegreeOfParallelism` as a knob the rule turns**, tested here. It does not imitate a smaller machine.
  `ParallelBinProcessor` takes a `concurrencyLevel` that only sizes its dictionary and never reaches
  `MaxDegreeOfParallelism`; fix that before any rule relies on it.

## Done when

- [ ] The bins bench runs the shape above, and the maintainer has run it and kept the report.
      `ls lib/results/benchmarks/*/threshold/`
- [ ] The drop point is read out of it, or the report shows there is none, and the lib findings record says so.
      **By eye.**
