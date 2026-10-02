---
description: Session 4 - ParallelAlgorithms - find when racing algorithms on one bin in parallel beats a loop. Add and try, one class at a time, starting with the same bin and items as ParallelBins
state: ready
waits-on: "the maintainer decides the two fixes below, then runs ParallelOverhead on the default job"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/**", "lib/bench/Binacle.Lib.Benchmarks.ParallelOverhead/**", "shared/test/Binacle.Benchmarking/**", "lib/test/Binacle.Lib.Testing/IdenticalCase.cs", "lib/results/benchmarks/**", "tooling/bench.just"]
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

`Identical_Packing` has one kept run, on the short job, and is unchanged. The question of why starting a race
gets dearer with cores moved to its own project - see "Why the win comes later on more cores" below.

Five overhead runs are done and read - see "What the overhead runs showed". **The missing cost is found**: it
is the work being fetched to a cold CPU, and the `Bytes` axis measures it. That closes the question this plan
opened. The rig is settled as of 2026-10-02; what is left is one clean run of it.

**Both rig faults are settled (the maintainer, 2026-10-02).** They were faults in the rig, not findings.

1. **The allocation is capped - done.** `Fake_Bins` now sweeps 1, 2 and 8 bins, on his "yes do it". At 32 bins
   by 64 KB each operation walked about 2 MB, which measured garbage collection rather than dispatch, and two
   cells came back `NA`. 8 bins by 64 KB is 512 KB and still shows the effect. The grid is 150 cases.
2. **The 1-CPU job stays - he said keep it**: "2 keep it we need a baseline maybe the run was noisy maybe it was
   a dry run" (2026-10-02).

   **And this plan's reason for dropping it was too broad.** It said the 1-CPU rows "cannot be trusted" as a
   whole. Read row by row, they split by class. `Fake_TwoAlgorithms` at 0 bytes is tight - Loop 75.6 ns,
   `Parallel_OneThread` 301 ns, `Parallel` 2,666 ns, StdDev at 0.5, 0.5 and 4.8 per cent of mean - and that is
   the row carrying the finding, 2,666 ns against about 750 at 2 CPUs. What is broken is `Fake_Bins`: at 1 bin
   and 64 KB, `Loop` reads 3,504 ns against `Parallel_OneThread` at 1,393, which is impossible; at 8 bins and 0
   bytes, `Loop` has StdDev at 58 per cent of its mean. With many bins or 64 KB to walk, the thread pool and the
   garbage collector fight over the single CPU; with a 2- or 3-wide race and nothing to walk they do not. The
   run that produced those cells was the short job, not dry - its header reads `IterationCount=3 LaunchCount=1
   WarmupCount=3`.

   It is also the only row that shows the case `ParallelLimits.Degree` exists to prevent, so dropping it would
   have cost the proof behind the cap.

Then: one run on the default job, kept under `lib/results/benchmarks/baseline/parallel-overhead/`, with a row
in `lib/results/benchmarks/baseline/README.md` and its real time in the recipe's comment.

**Undecided, and waiting on him**: what to do with `lib/results/benchmarks/baseline/parallel-bins/`. Two of its
cells no longer describe the code - see the record in the bins plan. Three options were put to him on
2026-10-02 - delete the stale reports, overwrite them as new runs arrive, or move the parallel set to a dated
folder and rebuild `baseline/` - and he has picked none. The agent's preference was the dated folder, because
the existing bins run is the only measurement of the uncapped behaviour and the findings quote it.

`parallel-algorithms.md` is the maintainer's to shape; session 6 builds it.

## What the first run shows - agent, 2026-10-01

Read from `baseline/parallel-algorithms/Identical_Packing.md`, short job.

- **FFD,BFD: Parallel's best Ratio is 0.61** (2 cores, 256 pieces). The ideal for two algorithms that take the
  same time is 0.50. ParallelBins stops near 0.55 too (step 5).
- **FFD and BFD take the same time here; WFD does not.** At 256 pieces WFD takes about 7 times FFD, so the
  three-algorithm race waits on WFD.
- **More cores, later win.** Parallel starts to win at 16 pieces on 2 cores and only from 64 on 8 and 12.
- **The run is tight, not rough** - corrected (agent, 2026-10-01). An earlier reading called it rough because
  the Error column is more than half the Mean in half the Parallel rows. That column carries no information
  here: Error is a flat 18 times StdDev in every single row of the report, because the short job's 3 iterations
  fix the multiplier. Read StdDev over Mean instead - median 0.3% in the Loop rows, 2.8% in the Parallel rows.
  RatioSD is 0.01 at the median.
- **What is wrong instead: a few whole processes ran slow.** A Loop row does the same work whatever core count
  it is pinned to, so any spread across core counts in one Loop row is pure noise and is free to read. The
  median spread is 1.04. But `Loop_FFD_WFD_BFD` at 128 pieces reads 68, 69, 69 and 101 us - 1.48 - and that
  slow process is the whole reason the 12-core 128-piece ratio reads 0.73. `Loop_FFD_BFD` never spreads past
  1.05 at any cell. **Every bad cell is on the WFD path**, which is the same whole-process optimization problem
  this plan already names for BFD. So the FFD and BFD numbers hold; the three-algorithm ones have holes, and
  their apparent wins are slow Loops rather than Parallel gains.

## Why the win comes later on more cores - agent, 2026-10-01

Read off `baseline/parallel-algorithms/Identical_Packing.md`. **The drop point moving is not itself a puzzle - it
follows from one number.** Starting a race costs more the more cores the process can see, while the packing
itself does not change at all.

At 1 piece there is almost no real work, so what is left is the cost of starting the race:

| Cores | Loop_FFD_BFD | Parallel_FFD_BFD | the gap |
|---|---|---|---|
| 2 | 0.68 us | 1.31 us | 0.63 us |
| 4 | 0.68 us | 1.64 us | 0.96 us |
| 8 | 0.69 us | 2.17 us | 1.48 us |
| 12 | 0.67 us | 2.46 us | 1.78 us |

FFD alone over the same four core counts is 295, 299, 298 and 293 ns - flat. So the algorithm does not care
about the core count; only starting the race does, and that nearly triples from 2 cores to 12.

The drop point then falls out as arithmetic. A race of two saves roughly the time of the second algorithm, so it
pays once one algorithm takes longer than the gap above. On 2 cores that is 0.63 us, which FFD reaches at about
16 pieces. On 12 cores it is 1.78 us, which FFD reaches between 16 and 64 pieces. Both match the rows.

**So the open question is one step back: why does starting a race get dearer with cores, when a race only ever
uses 2 or 3 threads?** `Binacle.Lib.Benchmarks.ParallelOverhead` was built to answer it, a project of its own,
on the maintainer's "lets make the first and 3rd theese we can measure cleanly and get them down properly" and
"i do agree its too much packed" (2026-10-01). Its README holds the detail. What it does differently is measure
the cost straight: its algorithms take no measurable time, so a row is what the processor costs around them,
rather than the gap between two packing rows of similar size.

## What the overhead runs showed - agent, 2026-10-02

Three runs over 2026-10-01 and 02, on jobs that have since been cut. Those jobs moved apart two things
`JobsByCoreCount` always moves together - the CPUs a thread may really run on, and the count the runtime is told - and
added pairs that held the count still while putting the CPUs on opposite dies. **The findings below are why the
current shape is what it is**; the jobs that produced them are gone, so none of this is reproducible from the
project as it stands.

**The whole cost is handing work to another CPU.** Capping the race to one thread, so nothing is dispatched,
gives 235 ns and it is flat across every job - deaf to the CPU count and to what the runtime believes. That
splits the cost in three:

| | 1 algorithm | moves with the core count? |
|---|---|---|
| the loop itself | 45 ns | no |
| `Parallel.For`'s own setup | +190 ns | no |
| handing the work to another CPU | +571 ns | **yes - this is all of it** |

This is what the three rows of the current shape measure, and it is the reason a race of one algorithm is not
worth a class: `Parallel.For` runs one item inline, so there is no second gap.

**Both levers matter, and together they cost more than the sum.** For a two-algorithm race: telling the runtime
12 while pinned to 2 cost +10 per cent, giving 12 CPUs while telling it 2 cost +18, and both together +63. An
earlier reading of this plan called the two "one number, the smaller of them" - **that was wrong**, and the
6-CPU job is what broke it: the honest 2, 6 and 12 jobs came out at 708, 838 and 1,152 ns, a curve rather than a
step.

**The result dictionary is ruled out.** Holding its lock array at the race width saved about 500 B and no time:
the differences scattered both ways with no pattern. Measured, not argued. The code was still changed, because
the parameter was wrong for other reasons - see the record below.

**A cap below the race width is a real knob, and a trap.** Capping a three-algorithm race at 2 took it from
1,441 to 871 ns, down to the cheapest figure the full machine ever reaches, in two runs that agreed. But it
serialises the third algorithm, so it buys about 570 ns of setup for that algorithm's whole runtime - fine on a
tiny request, ruinous when the third is WFD on 256 pieces. It is a threshold of its own, not a setting to turn
on, which is why no row measures it any more.

**The dies could not be measured.** Two CPUs on opposite dies against two neighbours came out +2.7, +5.8, +2.2
per cent in one run and +2.6, -2.3, -2.7 in the next. The signs flip, every value sits inside the noise, and the
jobs were dropped: this rig cannot answer it at its precision.

**Loop is clean and is the sum**: about 27.5 ns per algorithm, flat within 6 per cent across every job, deaf to
both levers. It is the row that says whether a run is sound.

**The machinery does not explain the drop point at 12 cores.** Taking the measured gap and asking when FFD grows
past it:

| | measured gap | predicts first win | the base run wins at | so the gap must be |
|---|---|---|---|---|
| 2 cores | 650 ns | 8 pieces | 16 pieces | 712 to 1,165 ns |
| 12 cores | 1,135 ns | 16 pieces | 64 pieces | 2,099 to 4,027 ns |

At 2 cores it holds, one step out. At 12 it accounts for about a third. **One to three microseconds at 12 cores
is not machinery.** The likeliest remainder is the packing work itself running slower on a distant CPU, whose
cache holds none of the bin or item data - and this project cannot see that, because stubs touch no memory. So
the negative is clean rather than inconclusive.

Noise: StdDev over Mean ran 1.4 to 2.7 per cent at the median across the three runs, with a handful of cells
above 6 and one at 30. `Error` was again a flat 15.7 to 18.4 times StdDev, so that column still carries nothing.
**Every run was one launch per case**, so none of them could see process-to-process variance - which is what
produces the slow Loop cells and the flipping die signs. Any reading under about 20 per cent needs more
launches before it is safe.

## The missing cost, found - agent, 2026-10-02

Two runs of the settled shape: `Fake_TwoAlgorithms`, `Fake_ThreeAlgorithms` and `Fake_Bins`, each row over a
working set of 0 or 64 KB, on 1, 2, 4, 8 and 12 CPUs. The first was the default job before the `Bytes` axis
existed; the second the short job with it.

**The cost of handing work to another CPU is mostly the work's memory, not the dispatch.** Two algorithms, the
gap between `Parallel` and `Parallel_OneThread`:

| Cores | walking nothing | walking 64 KB | |
|---|---|---|---|
| 2 | 443 ns | 1,381 ns | +212% |
| 4 | 521 ns | 2,383 ns | +357% |
| 8 | 890 ns | 3,452 ns | +288% |
| 12 | 840 ns | 3,870 ns | +361% |

**That closes the gap this plan could not account for.** The drop point at 12 cores needs the cost to be 2,100
to 4,000 ns; the machinery alone gave 1,135; with a 64 KB working set it measures 3,870. The remainder was
never machinery - it is the second thread fetching data the CPU it landed on does not hold.

**And 64 KB is not an arbitrary size.** FFD on the identical case allocates 1.66 KB at 1 piece rising to 69.02
KB at 256, read from `baseline/parallel-algorithms/Identical_Packing.md`. The size that reproduces the real
drop point is the size the real algorithm uses, which is what makes the axis worth having rather than a knob
that happens to fit.

**So the three projects can divide cleanly** - which is what the maintainer asked for on 2026-10-02 ("i aim to
keep a simple earchitecture and clean separation"). `ParallelOverhead` owns the whole cost, not a floor, because
`Bytes` reaches the part no-work algorithms cannot show. The two packing projects are left with their own
question: whether parallel pays, and where it does not.

**With bins it grows the same way and faster**, 12 cores, same gap:

| Bins | walking nothing | walking 64 KB |
|---|---|---|
| 1 | 556 ns | 1,265 ns |
| 2 | 871 ns | 3,137 ns |
| 8 | 2,003 ns | 15,134 ns |
| 32 | 4,644 ns | 54,111 ns |

Read the 32-bin row knowing it is also the row that produced the `NA` cells: 32 buffers of 64 KB is about 2 MB
an operation, so part of what it measures is garbage collection rather than fetching.

**A prediction that was wrong, recorded so it is not made twice.** This plan said 1 CPU would be the cheap end -
a race has nowhere to hand the work, so `Parallel` should land on `Parallel_OneThread`, and a run where it did
not was a broken run. The default-job run shows 1 CPU is the **dearest**: 1,231 ns against 752 at 2 CPUs, a
U-shaped curve with its floor at 2. `Parallel.For` still hands the work to a worker thread, and with one CPU
that worker waits for the caller to be taken off it. The claim had been written into the project README, its
`Program.cs` and the lib tests doc as a soundness check; all three are corrected.

**What held up across every run**: `Parallel_OneThread` is flat. 290 to 300 ns on all five core counts at 0
bytes, StdDev under 1 per cent of Mean. No dispatch means nothing for the machine to change, which is what
makes it the reference line the other two rows are read against.

## What changed in the code afterwards - agent, 2026-10-02

Researched with the maintainer on 2026-10-02 and changed on his "ok refactor". Both are in
`ParallelLimits`, read by all three parallel processors.

- **`concurrencyLevel` is gone from the three constructors** and worked out instead: one writer per work item,
  never more than the threads that can run. It was defaulting to `Environment.ProcessorCount`, so a race of two
  asked for a 12-lock array. Measured on .NET 10.0.12: a lock array costs about 44 B per lock, so 712 B for a
  3-entry dictionary where 240 B would do. **And passing the argument at all switches off lock-array growth** -
  an explicit 12 stays at 12 locks after 5,000 inserts where the default grows to 1,536. So the old code was not
  merely restating the default, it was quietly picking something worse, and the matching number hid it.
- **`maxDegreeOfParallelism` is the one optional argument now**, on all three. It was set nowhere in the
  repository, so `Parallel.For` ran unlimited.
- **It now defaults to a cap** (2026-10-02, on his "can u fix"): `ParallelLimits.Degree`, which is
  `min(work items, Environment.ProcessorCount)` and never below 1. The algorithm processor resolves it in its
  constructor; the two bin processors resolve it per call, because the bin count only exists then. Pass
  `ParallelLimits.NoLimit` to lift it.

  **It is a cap, not a decision.** Whether to parallelise at all stays with the factory, in one place, and no
  processor branches to a loop on its own - his framing on 2026-10-02: "the factory will choose but that we
  better guard the processors". What the cap prevents is the pathological case: on one CPU, an uncapped race
  measured 1,231 ns against the loop's 74, so parallel was 16 times dearer than not bothering. Capped it is
  296 ns - still dearer than looping, which is why the factory and not the cap is what keeps production off
  that path.

  **Two consequences.** `ParallelOverhead` now passes `NoLimit` explicitly, because an uncapped dispatch is
  what it measures and the cap would erase its 1-CPU and 1-bin rows. And `ParallelAlgorithms` is unaffected -
  2 or 3 algorithms on 2 to 12 cores means the cap always equals the race width - while `ParallelBins` is
  affected at two cells, recorded in the bins plan.

What the research settled, against claims found while searching: the `concurrencyLevel` constructors are **not**
obsolete and the argument is **not** ignored on .NET 10 - that was an unresolved 2020 proposal, dotnet/runtime
issue 33131, parked and never shipped. .NET 8 and up do accept -1 for "give me the default".

**What `maxDegreeOfParallelism` cannot do**, from the .NET documentation: "changing MaxDegreeOfParallelism from
the default only limits how many concurrent tasks will be used". How many, not which. It caps the count and
does not place the work, and .NET has no managed way to place it - affinity is per OS thread, and these run on
pooled threads, so pinning one reaches everything that later uses it. That is why the jobs pin the whole process
and production cannot. A cap of 2 is therefore not expected to reach the cheap case; the row is there to find
out, not to confirm.

**One loose end, a different problem.** At 256 pieces the gap between the race and its slowest algorithm is 6.1,
8.7, 12.7 and 15.6 us by core count - far more than the setup figures above, and setup should not care how many
pieces there are. Each algorithm builds its own piece array, so this looks like the threads getting in each
other's way over memory. Not the same question.

## The shape ParallelOverhead settled on - the maintainer, 2026-10-02

His words: "paralel ovehear = fake algoirithms + bins we test both... we start with no work and some work?",
and on the naming, "make sure u use names that are clear and people undersntand what theu do".

| Class | What it is |
|---|---|
| `Fake_TwoAlgorithms` | the race the multi-bin routes run |
| `Fake_ThreeAlgorithms` | the race the single-bin routes run |
| `Fake_Bins` | 1, 2 or 8 bins through the bin processors |

Three rows on each: `Loop`, `Parallel_OneThread` (`maxDegreeOfParallelism: 1`), `Parallel`. `Bytes` is 0 and
65,536. Core counts 1, 2, 4, 8, 12 - the set every other project runs, plus 1, which stays on his word.

**Read the gaps, not the rows.** `Loop` to `Parallel_OneThread` is what `Parallel.For` costs to set up and does
not move with the machine; `Parallel_OneThread` to `Parallel` is what handing work to another CPU costs, and
that one does.

**Why a class per race width but a parameter for bins:** a race is only ever 2 or 3 wide in production, and
BenchmarkDotNet's `--filter` picks classes rather than parameter values, so each can be run alone. A request
carries any number of bins, so that has to be a parameter.

**Names that were rejected on the way**, so they are not revisited: `Stub_` (jargon, and it describes how the
double is built rather than what the case is), `NoPacking_` (collides with `OperationResultStatus.NotPacked`,
a real result), `Empty_` (reads as an empty bin or an empty request), `NoWork_` (accurate until the algorithms
started walking memory, which made no-work one value of a sweep rather than the case).

## The class

**Decided (the maintainer, 2026-09-30):** its shape - "But the shape is good" - and its name - "identical write it
into the plans". The docs describe it. Why it looks the way it does - agent, 2026-09-30:

- **The alone rows** show how far apart the algorithms are, and check that Loop is their sum.
- **The same bin, item and piece values as ParallelBins' `Identical_FFD_Packing`.** Its one-bin Loop row does the
  same work as the FFD alone row here, so the two reports check each other.
- **No lines sweep.** ParallelBins' lines class answers it.

## Two more shape changes - suggested (agent, 2026-10-01), pending

Put to the maintainer on 2026-10-01 with the two that were built. He took those two and said of these: "i dont
understand 2 and 4 but i believe we need to see". Kept here unexplained work, to be gone through with him
before anything is built.

**Make the x-axis time, not pieces.** `Pieces` is a stand-in for how long one algorithm takes, and a poor one:
WFD at 64 pieces costs about what FFD costs at 256, so the same column value means different work in different
rows, and a drop point read off it is FFD's drop point rather than the rule. A stub algorithm that spins for a
set duration would sweep *work per algorithm* straight, from nothing up to 100 us. The drop point then comes out
as a curve instead of 9 points, it is the same curve whichever algorithm is in front of it, and a spin loop has
nothing for the run-time optimization to get wrong, so the slow-process cells go away. Real algorithms become
the check: does FFD at 256 pieces land where the curve says? It also gives unevenness without a contrived bin -
spin two threads for 100 and 100, then 100 and 10, then 100 and 1.

**Spend the run budget on launches rather than piece counts.** Every case is one process, 3 iterations, one
launch, so the report cannot see process-to-process variance at all - which is the one thing that is actually
wrong with the base run. 5 piece values at 3 launches costs about what 9 at 1 launch costs, and BenchmarkDotNet's
own StdDev would then include the variance instead of hiding it.

**The mechanism exists as of 2026-10-02:** every bench recipe takes an optional launch count after the word, so
`just bench lib-parallel-algorithms-identical quick 3` runs each case in three processes. It is ours, not
BenchmarkDotNet's `--launchCount`. What is still open is only whether to spend this project's budget that way,
and at what piece counts.

## The steps after - suggested (agent, 2026-09-30), pending

**Split the rule in two, as step 5 does:**

- **The machine part** - what Parallel costs to set up, per core count. `Identical_Packing` answers it.
- **The request part** - how uneven the algorithms are. Loop costs the sum of the algorithms; Parallel costs
  setup plus the slowest. The gain shrinks as one algorithm takes most of the time. A class where one algorithm
  is made slow on purpose - a bin that fits only part of the items - measures it.

**The core puzzle** - part of it is now read off the base run; see "Why the win comes later on more cores".
Checks that are still open, cheapest first:

- **Tried and dropped**: 2 CPUs next to each other against 2 far apart. Ran twice in ParallelOverhead, signs
  flipped, everything inside the noise. The 9900X has two six-core dies, so the comparison says something only
  where the CPUs a run is pinned to are known to map to real cores, which the bench cannot establish on its own.
- **Answered**: whether the cost follows the real CPUs or what the runtime believes. Both, and together they
  cost more than the sum. See "What the overhead runs showed".

**Order the work needs:** the machine part, then the request part, which needs its setup cost. A class on real
problems then checks the rule; `Full_Packing` at commit 16560b6f is one to start from.

## Done when

- [ ] `Identical_Packing` has a default-job run kept.
      **By eye.** A report of it in a dated folder under `lib/results/benchmarks/`.
- [ ] The threshold is read out of the runs, or they show there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
