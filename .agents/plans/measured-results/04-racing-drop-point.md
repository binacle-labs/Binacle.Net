---
description: Session 4 - run the racing bench, keep it as the baseline, and read the algorithm parallelisation threshold - when racing algorithms on one bin in parallel beats a loop
state: ready
waits-on: "the maintainer picks between the analysis below and the full run as it stands"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/**", "lib/test/Binacle.Lib.Testing/CoresSet.cs", "lib/results/benchmarks/**"]
---

# 4 - The racing drop point

The bench is `lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms`: class `Full_Packing` (was `Cores_Packing`),
recipe `lib-parallel-algorithms-full`. No run of it is kept yet. `Smoke_Packing` has its first kept run, taken
2026-09-29 on the short job.

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

**Left from session 3:**

- The smoke and sample recipes in `tooling/bench.just` say "time not measured yet". Ask the maintainer how long
  each run took and put it in.

`parallel-algorithms.md` is the maintainer's to shape; session 6 builds it.

## What the sample run shows - suggested (agent, 2026-09-30)

Read from `baseline/parallel-algorithms/Sample_Packing.md`, the default job, the ladder from 3 to 79 items.

- **On 2 cores Parallel pays early.** FFD,BFD breaks even at 13 items (1.00) and wins from 17 on, down to 0.53×
  at 79. FFD,WFD,BFD wins from 7 items, except at 59 (1.14) and 67 (0.99).
- **More cores make Parallel slower, not faster.** Loop stays flat across core counts; Parallel at 3 items
  takes 1.5 µs on 2 cores and 2.7 µs on 12. On 8 and 12 cores it loses almost everywhere on the ladder.
- **So the drop point depends on the core count**, and on this machine it moves up, not down, as cores are
  added. A guess to test, not a finding: the 9900X has two six-core dies, so 8 and 12 cores may span both.
  This machine is a KVM guest, so the guess holds only if the host keeps each virtual CPU on one real core.
- **Some cells are a slow process, not the code.** Loop jumps in a few (47 items, FFD,WFD,BFD, 12 cores: 37 µs
  against 28 µs elsewhere), and the ratio beside it moves with it. The 4-core column swings the most.
- The ladder stops at 79 items. The full run's Bischoff picks start at 74, so it shows the larger jobs.

## Analysis before the full run - suggested (agent, 2026-09-30), pending

The maintainer, 2026-09-30: "yes write it into step 4 as suggestions i wil ltackle it in a new session". He
agreed to have it written down; whether the full run waits for it is still his call.

**Why the current benches cannot give the rule.** The ladder changes three things per step - item count, item
types and fill - and at 79 items all but BFD fail. A ratio that moves cannot say which of them moved it.
`Full_Packing`'s Bischoff picks are real and spread by time, which makes them a good check of a rule, not a
source of one. The same flaw sank the old bins grid; step 5 answered it with `Even`.

**Split the rule in two, as step 5 does:**

- **The machine part** - what Parallel costs to set up, per core count. Identical items that all fit, a sweep of
  piece counts, both sets. The algorithms take about the same time there, the best case for racing, so the
  break-even in pieces is the lowest any request can have. Step 5's `Even` idea, applied to racing.
- **The request part** - how uneven the algorithms are. Loop costs the sum of the algorithms; Parallel costs
  setup plus the slowest. The gain shrinks as one algorithm, usually BFD, takes most of the time. A sweep where
  one algorithm is made slow on purpose - a bin that fits only part of the items - measures it.

**The core puzzle.** Parallel gets slower as cores are added, while Loop stays flat. A race uses 2 or 3
threads, so the extra cores should cost nothing. Suggested checks, cheapest first:

- One small job on 2 CPUs next to each other against 2 CPUs far apart (0 and 6) - a job in `CoreJobs`, not a
  new class. On a KVM guest this says something only if the host pins virtual CPUs to real cores; ask the
  maintainer how the guest is set up.
- The thread pool's minimum threads rises with the CPU count; a run with it fixed would show whether the
  pool, not the CPUs, is the cost.

**Order the work needs:** the machine part first; it is short and needs nothing. The request part needs its
setup cost. The full run then checks the rule instead of being the only source of it.

## Reading it

This machine is a KVM guest; its CPUs are virtual, and the host decides where they run.

## Done when

- [ ] The precise run is kept.
      `ls lib/results/benchmarks/baseline/parallel-algorithms/Full_Packing.md`
- [ ] The threshold is read out of it, or the report shows there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
- [x] The sample run is kept.
      `ls lib/results/benchmarks/baseline/parallel-algorithms/Sample_Packing.md`
- [ ] Every `lib-parallel-algorithms-` recipe states its cost.
      `! grep -n "time not measured yet" tooling/bench.just`
