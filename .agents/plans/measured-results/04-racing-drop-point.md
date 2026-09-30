---
description: Session 4 - ParallelAlgorithms - find when racing algorithms on one bin in parallel beats a loop. Add and try, one class at a time, starting with the same bin and items as ParallelBins
state: ready
waits-on: "the maintainer reads the first runs and picks the next class"
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

## What is left

Both projects are rebuilt and the first runs are kept - `Identical_Packing` here, step 5's FFD classes there.

1. Read the runs with the maintainer: what Parallel costs on each core count, and the piece count where it
   breaks even.
2. Pick the next class with him from "The steps after". Each is built only after the one before has run.

`parallel-algorithms.md` is the maintainer's to shape; session 6 builds it.

## The first class

**Decided (the maintainer, 2026-09-30):** the shape below - "But the shape is good". The name - "identical
write it into the plans". "Identical": every bin and every item is the same.

- **`Identical_Packing` in ParallelAlgorithms, shaped like `Full_Packing`:** Loop and Parallel for `FFD,BFD` and for
  `FFD,WFD,BFD`, each race with its own Loop baseline, plus FFD, WFD and BFD alone.
- **One parameter, `Pieces`**, with the values of ParallelBins' `Identical_FFD_Packing`. One line of one item.
- **Bin `160x120x80`, item `20x15x10`**, held in one small class in `lib/test/Binacle.Lib.Testing`. Both projects
  already reference it; ParallelBins' `IdenticalBase` reads the same class, so the two cannot drift.
- **Setup throws** unless every algorithm packs every piece.
- **Recipe `lib-parallel-algorithms-identical`**, short job, with `dry` - the shape of ParallelBins' recipe.

Why each part - agent, 2026-09-30:

- **The alone rows.** WFD v2 and BFD v2 sort the free-space list for every piece; FFD v2 does not. So even here
  the three do not take the same time. The alone rows show by how much, and check that Loop is their sum.
- **The same piece values.** ParallelBins' one-bin Loop row of `Identical_FFD_Packing` is one FFD pack of the same pieces - the same work as
  the FFD alone row here. The two reports check each other.
- **No lines sweep.** ParallelBins' lines class answers whether lines cost anything.
- **The piece range** covers the demo samples, 2 to 24 pieces with one at 92, and runs past them.
- **Short job**, as ParallelBins: find the point here, confirm on the default job later.

## What the sample run showed - suggested (agent, 2026-09-30)

Read from `baseline/parallel-algorithms/Sample_Packing.md` (in git at commit 16560b6f), the default job, the ladder from 3 to 79 items.

- **On 2 cores Parallel pays early.** FFD,BFD breaks even at 13 items (1.00) and wins from 17 on, down to 0.53×
  at 79. FFD,WFD,BFD wins from 7 items, except at 59 (1.14) and 67 (0.99).
- **For FFD,BFD, more cores make Parallel slower.** At 3 items it takes 1.5 µs on 2 cores and 2.7 µs on 12. On 8
  cores it wins only at 47 items (0.91); on 12 it never wins.
- **FFD,WFD,BFD is mixed on 8 and 12 cores.** It loses up to 47 items, then wins in some cells: 0.86 at 59 on 12
  cores, 0.91 and 0.90 at 67, 0.73 at 79 on 8 cores.
- **Loop is not flat for FFD,WFD,BFD.** It moves with the core count: at 47 items 28 to 37 µs, at 79 items 51 to
  68 µs. FFD,BFD's Loop stays close. Some cells are a slow process, not the code, and the ratio beside them moves
  with them.
- **So the drop point depends on the core count** and the set. A guess to test, not a finding: the 9900X has two
  six-core dies, so 8 and 12 cores may span both. This machine is a KVM guest, so the guess holds only if the
  host keeps each virtual CPU on one real core.

## The steps after - suggested (agent, 2026-09-30), pending

**Why the ladder cannot give the rule.** It changes three things per step - item count, item types and fill -
and at 79 items all but BFD fail. A ratio that moves cannot say which of them moved it. Real problems, like the
`CoresSet` picks, are a good check of a rule, not a source of one.

**Split the rule in two, as step 5 does:**

- **The machine part** - what Parallel costs to set up, per core count. The first class above answers it.
- **The request part** - how uneven the algorithms are. Loop costs the sum of the algorithms; Parallel costs
  setup plus the slowest. The gain shrinks as one algorithm, usually BFD, takes most of the time. A class where
  one algorithm is made slow on purpose - a bin that fits only part of the items - measures it.

**The core puzzle.** For FFD,BFD, Parallel gets slower as cores are added. A race uses 2 or 3 threads, so the
extra cores should cost nothing. Checks, cheapest first:

- One small job on 2 CPUs next to each other against 2 CPUs far apart (0 and 6) - a job in `CoreJobs`, not a
  new class. On a KVM guest this says something only if the host pins virtual CPUs to real cores; ask the
  maintainer how the guest is set up.
- The thread pool's minimum threads rises with the CPU count; a run with it fixed would show whether the
  pool, not the CPUs, is the cost.

**Order the work needs:** the machine part, then the request part, which needs its setup cost. A class on real
problems then checks the rule; `Full_Packing` at commit 16560b6f is one to start from.

## Reading it

This machine is a KVM guest; its CPUs are virtual, and the host decides where they run.

## Done when

- [x] Only `Identical_Packing` is left in ParallelAlgorithms, beside `Program.cs`.
      `ls lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/*.cs`
- [x] The ladder reports and their rows are gone.
      `! ls lib/results/benchmarks/baseline/parallel-algorithms/S*_Packing.md` and
      `! grep -n "parallel-algorithms/S" lib/results/benchmarks/baseline/README.md`
- [x] Nothing outside git history names a removed class or recipe.
      `grep -rn --exclude-dir=.git -e Full_Packing -e LadderBase -e lib-parallel-algorithms-full .` finds only this
      plan and the lib findings record's history, then **by eye**.
- [x] `Identical_Packing`'s run is kept.
      `ls lib/results/benchmarks/baseline/parallel-algorithms/Identical_Packing.md`
- [ ] The threshold is read out of the runs, or they show there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
- [x] Every `lib-parallel-` recipe states its cost.
      `! grep -n "time not measured yet" tooling/bench.just`
