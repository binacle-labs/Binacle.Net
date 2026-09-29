---
description: Session 4 - run the racing bench, keep it as the baseline, and read the algorithm parallelisation threshold - when racing algorithms on one bin in parallel beats a loop
state: blocked
waits-on: "session 3 - the bench structure (the maintainer, 2026-09-29: 'yes 04 waits on step 3 too')"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/**", "lib/test/Binacle.Lib.Testing/CoresSet.cs", "lib/results/benchmarks/**"]
---

# 4 - The racing drop point

Session 3 moves the bench into `ParallelAlgorithms`: class `Full_Packing` (was `Cores_Packing`), recipe
`lib-parallel-algorithms-full`. No run of it is kept yet.

## Goal

**(the maintainer, 2026-09-29):** measure the algorithm parallelisation threshold, or its cost function - when
racing algorithms on one bin in parallel beats running them one after another. "one point is to measure the
algorithm paralelisationtheshold or its cost function".

**Decided (the maintainer, 2026-09-25):** "lets make the algo race first and have that run... disable the rest
and we decide later".

**Leaning yes (the maintainer, 2026-09-29):** the bench's shape is right, though the restructure may rename it -
"the restructure may rename thinfs i believe the shapoe for this might be rugiht".

## What the session does

1. The maintainer runs `just bench lib-parallel-algorithms-full precise` - about 3.5 hours.
2. Keep the report as `lib/results/benchmarks/baseline/parallel-algorithms/Full_Packing.md` - its first kept
   run.
3. Read the threshold: where Parallel starts to win, and whether from some point it always wins by a meaningful
   amount - what "meaningful" means is the maintainer's. If there is no such point, say so; do not force one.
4. Fix the line in the lib findings record on when racing pays.

**Suggested (agent, 2026-09-29):** run and keep `lib-parallel-algorithms-sample` too. The ladder covers jobs
smaller than the smallest Bischoff pick, so the drop point may sit there.

`parallel-algorithms.md` is the maintainer's to shape; session 6 builds it.

## Reading it

This machine is a KVM guest; its CPUs are virtual, and the host decides where they run.

## Done when

- [ ] The precise run is kept.
      `ls lib/results/benchmarks/baseline/parallel-algorithms/Full_Packing.md`
- [ ] The threshold is read out of it, or the report shows there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
