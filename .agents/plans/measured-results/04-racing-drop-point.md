---
description: Session 4 - ParallelAlgorithms - find when racing algorithms on one bin in parallel beats a loop. Add and try, one class at a time, starting with the same bin and items as ParallelBins
state: ready
waits-on: "the maintainer re-runs Identical_Packing on the default job"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/**", "lib/test/Binacle.Lib.Testing/IdenticalCase.cs", "lib/results/benchmarks/**", "tooling/bench.just"]
---

# 4 - ParallelAlgorithms drop point

The project is `lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms`. `Identical_Packing` is its only class. The
ladder classes, `Full_Packing`, `CoresSet` and their reports are gone; they are in git at commit 16560b6f.

## Goal

**(the maintainer, 2026-09-29):** measure the algorithm parallelisation threshold, or its cost function - when
racing algorithms on one bin in parallel beats running them one after another. "one point is to measure the
algorithm paralelisationtheshold or its cost function".

**Decided (the maintainer, 2026-09-25):** "lets make the algo race first and have that run... disable the rest
and we decide later".

**(the maintainer, 2026-09-30):** the ladder numbers do not make sense; add and try instead - "we discussed how
theese numbers dont make sense instead we agreed to take a more simple add and try approach".

**Decided (the maintainer, 2026-09-30):** only the new classes remain, and the old reports go - "only theese
classes should remain and we build from there", "remove the results we added".

**Leaning yes (the maintainer, 2026-09-30):** the first step uses the same bin and the same items in
ParallelAlgorithms and ParallelBins - "the first step is to start with the same ladder? or same bin msame items on
both ideally this should give us the perfect results".

## What is left - suggested (agent, 2026-10-01)

`Identical_Packing` has one kept run, on the short job.

1. Add a `precise` mode to `lib-parallel-algorithms-identical` - the default job, as `lib-algorithms-full` has.
   The recipe takes only `dry` today.
2. The maintainer re-runs it with `precise`; keep it in a dated folder beside `baseline/`.
3. Read it with him: where Parallel starts to win, per core count.
4. Pick the next class with him from "The steps after". Each is built only after the one before has run.

`parallel-algorithms.md` is the maintainer's to shape; session 6 builds it.

## What the first run shows - agent, 2026-10-01

Read from `baseline/parallel-algorithms/Identical_Packing.md`, short job.

- **FFD,BFD: Parallel's best Ratio is 0.61** (2 cores, 256 pieces). The ideal for two algorithms that take the
  same time is 0.50. ParallelBins stops near 0.55 too (step 5).
- **FFD and BFD take the same time here; WFD does not.** At 256 pieces WFD takes about 7 times FFD, so the
  three-algorithm race waits on WFD.
- **More cores, later win.** Parallel starts to win at 16 pieces on 2 cores and only from 64 on 8 and 12.
- **The run is rough.** In half the Parallel rows the Error is more than half the Mean; in the other rows, almost
  none. That is why step 1 above re-runs it.

## The class

**Decided (the maintainer, 2026-09-30):** its shape - "But the shape is good" - and its name - "identical write it
into the plans". The docs describe it. Why it looks the way it does - agent, 2026-09-30:

- **The alone rows** show how far apart the algorithms are, and check that Loop is their sum.
- **The same bin, item and piece values as ParallelBins' `Identical_FFD_Packing`.** Its one-bin Loop row does the
  same work as the FFD alone row here, so the two reports check each other.
- **No lines sweep.** ParallelBins' lines class answers it.

## The steps after - suggested (agent, 2026-09-30), pending

**Split the rule in two, as step 5 does:**

- **The machine part** - what Parallel costs to set up, per core count. `Identical_Packing` answers it.
- **The request part** - how uneven the algorithms are. Loop costs the sum of the algorithms; Parallel costs
  setup plus the slowest. The gain shrinks as one algorithm takes most of the time. A class where one algorithm
  is made slow on purpose - a bin that fits only part of the items - measures it.

**The core puzzle.** Parallel starts to win later as cores are added, here and in ParallelBins. A race uses 2 or
3 threads, so the extra cores should cost nothing. Checks, cheapest first:

- One small job on 2 CPUs next to each other against 2 CPUs far apart (0 and 6) - a job in `CoreJobs`, not a
  new class. The 9900X has two six-core dies, so 8 and 12 cores may span both. This machine is a KVM guest, so
  this says something only if the host pins virtual CPUs to real cores; ask the maintainer how the guest is set up.
- The thread pool's minimum threads rises with the CPU count; a run with it fixed would show whether the
  pool, not the CPUs, is the cost.

**Order the work needs:** the machine part, then the request part, which needs its setup cost. A class on real
problems then checks the rule; `Full_Packing` at commit 16560b6f is one to start from.

## Done when

- [ ] `Identical_Packing` has a default-job run kept.
      **By eye.** A report of it in a dated folder under `lib/results/benchmarks/`.
- [ ] The threshold is read out of the runs, or they show there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
