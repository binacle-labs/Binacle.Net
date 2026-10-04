---
description: Fill every bench baseline and measure of lib and ViPaq, write one results file per question, then each slice's README summary
state: ready
waits-on: "the maintainer runs the parallel bench classes that have no kept report, and pins each results file's shape with his own yes"
horizon: next-release
paths:
  - "shared/**"
  - "lib/**"
  - "vipaq/**"
  - "tooling/**"
  - ".agents/**"
---

# Measured results

One plan. The work is: get a kept run for every bench class, fill the root results file of each question from
those runs, then write the summary at the top of each slice's results README.

## Goal

**(the maintainer, 2026-09-29)**, in his words, lightly joined:

- `results/` is per slice. The repo-root `results/` is gone.
- Each slice owns its `results/`. Benches live in `<slice>/bench/`, measures in `<slice>/measure/`.
- A measure writes to `results/measurements/`. It gives the same result on every run and every machine, so any
  change matters. It is not dated.
- A benchmark has noise, and we accept it. `results/benchmarks/baseline/` is filled first. Dated snapshots come
  after, and one can hold a single class instead of the whole suite.
- One file per question at the root of `results/`. The README summarises them.
- Compare per problem, then average: problem 1 in v1 against v2, then the mean. Never v1's mean against v2's.
- Benchmarks and measures must answer questions and show how well we do.
- Automation is not needed.

## Who runs what

- **The maintainer runs** every bench, every measure, and any build that starts the host. A session gives him
  the one-line command. A long run takes the whole machine while it runs. ("the first is true", 2026-09-29)

**Nothing runs while a bench runs** ("i am waiting on the benchmark to complete so dont urn anything",
2026-09-26).

## The results files - rules

Every line here quotes the maintainer.

- **What they are for** ("iot needs to show me what we are doing has value and benefit", 2026-09-25).
- **Each slice stays apart** ("each result needs to be isolated ideally", 2026-09-26).
- **Never average two algorithms together** ("no mixing algorithms.... separate etiher more table or more
  ggiles", 2026-09-26).
- **A difference is "×" the baseline**: v2 at 0.46× of v1 ("i prefer 0.46", 2026-09-25).
- **BenchmarkDotNet's own Ratio** where it fits ("use BenchmarkDotNet's own Ratio sometimes is true",
  2026-09-29).
- **A loss must show** - leaning yes ("yes both.,,, i think for now", 2026-09-26).
- **v2 everywhere, except where v1 against v2 is the question** ("the canonical is v2 everywhere else...
  unless we do a version comparison", 2026-09-25).
- **Placeholders are sample tables with fake numbers** ("the placegolder should be sample tables with face
  looking data so i review the sahe easitly", 2026-09-26). The line "Every number in this file is fake" goes
  when the last table of the file is filled.
- **The README computes nothing; every number in it names its file** - leaning yes ("perhaps the read me",
  2026-09-29).

**Slow processes are not chased here** (the maintainer, 2026-09-28: "we wont chase that now its too much").

**Every results file that reads a BFD time names it as a gap** (the maintainer, 2026-09-29: "keep it, name it
as a gap and name explicitly for what it is upfront its the optimization acting up"). Up front, in plain words:
.NET's run-time optimization (tiered PGO) sometimes picks a worse version of the code and keeps it for a whole
process, so that process runs slow from start to end. It still gains over PGO off, only less ("we steill get
the boost from pgo-off but not as much", 2026-09-29).

## Where it stands

Every root results file is a placeholder: a comment per table saying what it holds, and a sample table with
fake numbers under it. Both results READMEs are an index and nothing else.

**The order comes from the work.** The results files read the kept runs, `ParallelAlgorithms` and `ParallelBins`
included. The READMEs read the results files.

## What has run, and with which job

Read out of each kept report's own header, not from the recipe comments: a short-job report carries
`IterationCount=3  LaunchCount=1`, a default-job one prints no characteristics line at all, and a dry one says
`RunStrategy=ColdStart`. Dates and case counts live in `lib/results/benchmarks/baseline/README.md` and
`vipaq/results/benchmarks/baseline/README.md`; this table does not repeat them.

**Most times are the recipes' own estimates, not measured wall clock.** Two are measured and say so.

| Recipe | Kept | Job | Time | No kept run |
|---|---|---|---|---|
| `lib-algorithms-smoke` | 6 classes | quick | about 6 min | - |
| `lib-algorithms-sample` | 6 classes | precise | about 30 min | - |
| `lib-algorithms-full` | 6 classes | quick | about 16 hrs | precise, about 30 hrs |
| `lib-parallel-algorithms-identical` | 5 classes | quick, 3 launches | about 2 hrs, measured 2026-10-04 | precise |
| `lib-parallel-bins-identical` | FFD, 2 classes | quick | about 70 min, measured 2026-10-03 | WFD and BFD, 4 classes |
| `lib-parallel-overhead` | 3 classes | precise | 58 min, measured 2026-10-02 | - |
| `lib-result-selection` | 3 classes | precise | about 5 min | - |
| `lib-scaling` | `Sample_Packing` | precise | about 20 min | - |
| `vipaq-smoke` | 2 classes | precise | about 4 min | - |
| `vipaq-sample` | 4 classes | precise | about 30 min | - |

Every kept report belongs to a class that still exists - checked file by file against `lib/bench/` and
`vipaq/bench/`. There are no orphans.

**The measures have all run, and they take no job.** A measure gives the same result on every run and every
machine, so there is no quick or precise for it and no noise to average out. `just measure lib` wrote both
files under `lib/results/measurements/`; `just measure vipaq` wrote all of `vipaq/results/measurements/`.
Neither recipe states a time.

**Four bins classes still have no kept report** - WFD and BFD, Pieces and Lines.

## The three old parallel packing reports are gone

**All three were removed on 2026-10-03** - `parallel-algorithms/Identical_Packing.md`,
`parallel-bins/Identical_FFD_Packing.md` and `parallel-bins/Identical_FFD_Lines_Packing.md`. Two things had
made them unreadable: the processors changed on 2026-10-02, when `concurrencyLevel` left the constructors and
`maxDegreeOfParallelism` began defaulting to `ParallelLimits.Degree`; and the two projects were reshaped on
2026-10-03 into five and six classes. The runs of the new shape landed later the same day, and the table above
says which.

**What was read off them is not kept** (the maintainer, 2026-10-04: "delete them we go by what we have"). The
new runs re-measure it on the current code.

## The baseline folder, and skip

Both rules are settled and both have left this plan. **The baseline rule** - every class at the current shape,
a gone shape removed, a moved shape re-run and replaced, no dated folder - is stated at the top of
`lib/results/benchmarks/baseline/README.md`, which is where anyone copying a report will be. **The skip mark**
and the reasoning behind both are in the lib decisions ledger under `.agents/design/lib/`, and what the
attribute is today is in the lib tests doc under `.agents/docs/lib/`.

What is left here is only the judgement: **nothing is marked skip, and no class has earned a mark yet**,
because a mark says a run proved an answer.

## ParallelAlgorithms - the racing drop point

The project is `lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms`. It holds five `Identical` classes on a
shared base - the shape section below. The ladder classes, `Full_Packing`, `CoresSet` and their reports are
gone; they are in git at commit 16560b6f.

### Its goal

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

### The shape - five classes

**Built and dry-checked 2026-10-03**, in `lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/Identical/`:
each algorithm alone, then `FFD,BFD` and `FFD,WFD,BFD`, two methods each. The class set, the single grid, the
folder, and why the split was worth making are decided and written up in the lib decisions ledger under
`.agents/design/lib/`; what each class is now is in the lib tests doc under `.agents/docs/lib/`. **Nothing
about the shape is open here.**

Three things from it bite whoever runs this, so they stay:

- **Run all five in one invocation**, and read the sum check as a ratio, not an equality. In the kept run
  `Loop` on `FFD,BFD` is 1.06 to 1.23 times the sum of the alone classes, and allocation is additive within a
  few per cent. So about 1.2 is normal and a cell well above it is a slow process: `FFD,WFD,BFD` reaches 1.67
  at 256 pieces on 12 cores, and every cell above 1.5 is on the WFD path. The recipe's filter takes all five.
- **An alone class's `Parallel` row is not a race** - the cap resolves to 1 and `Parallel.For` runs the body
  inline. Read it as the cap's receipt: a flat step above `Loop` on every core count, and something is wrong
  with the cap if it is not.
- **The grid is nobody's decision yet.** `Pieces` 1 to 256 on 2, 4, 8 and 12 cores, 360 cases, carried over
  from the removed run. **Suggested (agent, 2026-10-03):** leave it until a run says otherwise.

### What is left here - agent, 2026-10-03

The five classes ran on the quick job on 2026-10-03 and their reports are kept under
`lib/results/benchmarks/baseline/parallel-algorithms/`. What is left is reading them, and the precise job and
the extra launches for the three-algorithm class, which have no kept run.

The question of why starting a race gets dearer with cores moved to its own project, `ParallelOverhead`. Its
runs are done and read, and the missing cost is found: it is the work being fetched to a cold CPU, which the
`Bytes` axis measures. That closes the question this plan opened.

**Both rig faults are settled (the maintainer, 2026-10-02).** They were faults in the rig, not findings.

1. **The allocation is capped - done.** `Fake_Bins` now sweeps 1, 2 and 8 bins, on his "yes do it". The old
   32-bin grid walked so much memory per operation that it measured garbage collection rather than dispatch,
   and two cells came back `NA`. The grid is 150 cases.
2. **The 1-CPU job stays - he said keep it**: "2 keep it we need a baseline maybe the run was noisy maybe it was
   a dry run" (2026-10-02). It is the only row that shows the case `ParallelLimits.Degree` exists to prevent,
   so dropping it would have cost the proof behind the cap.

**That run is done - 2026-10-02, the default job, 150 cases in 58 minutes.** All three reports are kept under
`lib/results/benchmarks/baseline/parallel-overhead/`, with their rows in
`lib/results/benchmarks/baseline/README.md` and the real time in the recipe's comment. **It is the only
overhead run that can be re-read from the repository**, so it is the one to quote.

**Keeping the 1-CPU job was right.** Not one `NA` cell, and every row on every core count has `Loop` below
`Parallel_OneThread` below `Parallel`, which is the ordering that has to hold. The two cells once called
untrustworthy are both clean.

**The run has been read out and its findings are recorded.** In short: starting a race is dearest on one CPU
and cheapest on two, only the hand-off to another CPU moves with the core count, and most of that hand-off is
the work's memory rather than the dispatch.

**Answered**: what to do with `lib/results/benchmarks/baseline/parallel-bins/`. Two of its cells no longer
describe the code. Three options were put to him on 2026-10-02 - delete the stale reports, overwrite them as
new runs arrive, or move the parallel set to a dated folder and rebuild `baseline/`. He took none of them; the
bins section below holds the decision and what follows from it.

`parallel-algorithms.md` is the maintainer's to shape; it is built with the other lib results files.

### Why `ParallelOverhead` exists - agent, 2026-10-01

The first racing run showed the drop point moving out as the core count rises, which raised a question one step
back: **why does starting a race get dearer with cores, when a race only ever uses 2 or 3 threads?**
`Binacle.Lib.Benchmarks.ParallelOverhead` was built to answer it, a project of its own, on the maintainer's
"lets make the first and 3rd theese we can measure cleanly and get them down properly" and "i do agree its too
much packed" (2026-10-01). Its README holds the detail. What it does differently is measure the cost straight:
its algorithms take no measurable time, so a row is what the processor costs around them, rather than the gap
between two packing rows of similar size. That question is now answered and recorded.

The run those readings came from was removed on 2026-10-03. The five-class run that replaced it has not been
read out yet.

### What the earlier overhead runs settled about the shape - agent, 2026-10-02

Three runs over 2026-10-01 and 02, on jobs that have since been cut. Their numbers are not kept (the
maintainer, 2026-10-04: "delete them we go by what we have"); what they settled about the project's shape is:

- **The three rows of the current shape are the right three.** Capping the race to one thread, so nothing is
  dispatched, is flat across every job, which is what makes it the reference line. A race of one algorithm is
  not worth a class: `Parallel.For` runs one item inline, so there is no second gap.
- **Both levers matter** - the CPUs a thread may really run on and the count the runtime is told - and
  together they cost more than the sum. An earlier reading called them "one number, the smaller of them";
  that was wrong, and the jobs that showed it are gone.
- **The result dictionary is ruled out.** Holding its lock array at the race width saved a little memory and no
  time. The code was still changed, because the parameter was wrong for other reasons - see the record below.
- **A cap below the race width is a real knob, and a trap.** It serialises the extra algorithm, buying setup for
  that algorithm's whole runtime - fine on a tiny request, ruinous when the third is WFD on 256 pieces. It is a
  threshold of its own, not a setting to turn on, which is why no row measures it any more.
- **The dies could not be measured.** Opposite dies against neighbours flipped sign between runs and sat inside
  the noise. This rig cannot answer it at its precision, so those jobs were dropped.
- **Every run was one launch per case**, so none could see process-to-process variance - which is what produces
  the slow Loop cells and the flipping die signs. Any reading under about 20 per cent needs more launches
  before it is safe.

**A prediction that was wrong, recorded so it is not made twice.** It was once held that 1 CPU would be the
cheap end - a race has nowhere to hand the work, so `Parallel` should land on `Parallel_OneThread`, and a run
where it did not was a broken run. The kept run shows 1 CPU is the **dearest**. `Parallel.For` still hands the
work to a worker thread, and with one CPU that worker waits for the caller to be taken off it. The claim had
been written into the project README, its `Program.cs` and the lib tests doc as a soundness check; all three
are corrected.

**Why the `Bytes` axis is there, and why 64 KB.** The machinery alone could not account for the drop point at
12 cores; the gap closes once the work has a real working set to fetch, and 64 KB is the order of what FFD
actually allocates at the top of the identical grid. The axis is what lets `ParallelOverhead` own the whole
cost rather than a floor - "i aim to keep a simple earchitecture and clean separation" (the maintainer,
2026-10-02). The two packing projects are left with their own question: whether parallel pays, and where it
does not.

### What changed in the code afterwards - agent, 2026-10-02

Researched with the maintainer on 2026-10-02 and changed on his "ok refactor". Both are in
`ParallelLimits`, read by all three parallel processors.

- **`concurrencyLevel` is gone from the three constructors** and worked out instead: one writer per work item,
  never more than the threads that can run. It was defaulting to `Environment.ProcessorCount`, so a race of two
  asked for a 12-lock array. Passing the argument at all also switches off lock-array growth, where the default
  grows the array as the dictionary fills. So the old code was not merely restating the default, it was quietly
  picking something worse, and the matching number hid it.
- **`maxDegreeOfParallelism` is the one optional argument now**, on all three. It was set nowhere in the
  repository, so `Parallel.For` ran unlimited.
- **It now defaults to a cap** (2026-10-02, on his "can u fix"): `ParallelLimits.Degree`, which is
  `min(work items, Environment.ProcessorCount)` and never below 1. The algorithm processor resolves it in its
  constructor; the two bin processors resolve it per call, because the bin count only exists then. Pass
  `ParallelLimits.NoLimit` to lift it.

  **It is a cap, not a decision.** Whether to parallelise at all stays with the factory, in one place, and no
  processor branches to a loop on its own - his framing on 2026-10-02: "the factory will choose but that we
  better guard the processors". What the cap prevents is the pathological case: in the kept overhead run an
  uncapped two-algorithm race on one CPU reads 1,244 ns against the loop's 76, so parallel is 16 times dearer
  than not bothering. Capping it does not make it cheaper than looping, which is why the factory and not the
  cap is what keeps production off that path.

  **Two consequences.** `ParallelOverhead` now passes `NoLimit` explicitly, because an uncapped dispatch is
  what it measures and the cap would erase its 1-CPU and 1-bin rows. And `ParallelAlgorithms` is unaffected -
  2 or 3 algorithms on 2 to 12 cores means the cap always equals the race width - while `ParallelBins` is
  affected at two cells, recorded in the bins section below.

What the research settled, against claims found while searching: the `concurrencyLevel` constructors are **not**
obsolete and the argument is **not** ignored on .NET 10 - that was an unresolved 2020 proposal, dotnet/runtime
issue 33131, parked and never shipped. .NET 8 and up do accept -1 for "give me the default".

**What `maxDegreeOfParallelism` cannot do**, from the .NET documentation: "changing MaxDegreeOfParallelism from
the default only limits how many concurrent tasks will be used". How many, not which. It caps the count and
does not place the work, and .NET has no managed way to place it - affinity is per OS thread, and these run on
pooled threads, so pinning one reaches everything that later uses it. That is why the jobs pin the whole process
and production cannot. A cap of 2 is therefore not expected to reach the cheap case; the row is there to find
out, not to confirm.

**One loose end, a different problem.** At the top of the piece ladder the gap between a race and its slowest
algorithm was far wider than any setup figure, and setup should not care how many pieces there are. Each
algorithm builds its own piece array, so this looks like the threads getting in each other's way over memory.
The run that showed it is gone; the kept racing reports can be read for it. Not the same question.

### ParallelOverhead, and the case both projects share

Both are built, run and documented, so only the live parts stay here.

`ParallelOverhead`'s shape and the reasoning behind it are in the lib decisions ledger under
`.agents/design/lib/`; what it is now is in the lib tests doc and the project's own README. **Read its rows as
gaps**: `Loop` to `Parallel_OneThread` is `Parallel.For`'s setup and is flat across core counts;
`Parallel_OneThread` to `Parallel` is the cost of handing work to another CPU, and that one moves.

**The case there is the same bin, item and piece values as ParallelBins' `Identical_FFD_Pieces_Packing`.** Its
one-bin `Loop` row does the same work as the `FFD` alone class in `ParallelAlgorithms`, so the two reports check
each other - that is the one cross-project reading to make. There is no lines sweep in `ParallelAlgorithms`; the
`ParallelBins` Lines classes answer it.

### Spend the run budget on launches - suggested (agent, 2026-10-01), pending

Put to the maintainer on 2026-10-01 with two shape changes that were built. He took those two and said of this
one and one other: "i dont understand 2 and 4 but i believe we need to see".

**Spend the run budget on launches rather than piece counts.** Every case is one process, 3 iterations, one
launch, so the report cannot see process-to-process variance at all - which is the one thing that is actually
wrong with the base run. 5 piece values at 3 launches costs about what 9 at 1 launch costs, and BenchmarkDotNet's
own StdDev would then include the variance instead of hiding it.

**The mechanism exists as of 2026-10-02:** every bench recipe takes an optional launch count after the word, so
`just bench lib-parallel-algorithms-identical quick 3` runs each case in three processes. It is ours, not
BenchmarkDotNet's `--launchCount`. What is still open is only whether to spend this project's budget that way,
and at what piece counts.

### The machine part, and the core puzzle - suggested (agent, 2026-09-30), pending

**The machine part** - what Parallel costs to set up, per core count. The five `Identical` classes answer it.

**The core puzzle** - part of it is now read off the base run; see "Why the win comes later on more cores".
Checks that are still open, cheapest first:

- **Tried and dropped**: 2 CPUs next to each other against 2 far apart. Ran twice in ParallelOverhead, signs
  flipped, everything inside the noise. The 9900X has two six-core dies, so the comparison says something only
  where the CPUs a run is pinned to are known to map to real cores, which the bench cannot establish on its own.
- **Answered**: whether the cost follows the real CPUs or what the runtime believes. Both, and together they
  cost more than the sum. See "What the overhead runs showed".

## ParallelBins - the bins drop point

### Its goal

**(the maintainer, 2026-09-29):** measure the bins parallelisation threshold, or its cost function - when
packing many bins in parallel beats packing them one after another. "the other the bins paralilizxation
thresohold of their cost function".

**Decided (the maintainer, 2026-09-25):** "lets make the algo race first".

His worry (2026-09-25): "it does worry me how we can test reliably with different bin shapes when i failed with
the ladder".

**(the maintainer, 2026-09-29):** this work finds the problems ParallelBins runs - "step 5 is finding the
problems and keep suggestions will reinvestigate".

**(the maintainer, 2026-09-30):** add and try - "we agreed to take a more simple add and try approach".

**Leaning yes (the maintainer, 2026-09-30):** the first step uses the same bin and the same items here and in
ParallelAlgorithms - "same bin msame items on both ideally this should give us the perfect results".

**Decided (the maintainer, 2026-09-30):** one class per algorithm - "for bins we need 3 one for each algotirhm to
see how algorithms change behaviour". The shape, with FFD run first - "But the shape is good... we will initially
choose to run FFD only". The name - "identical write it into the plans".

### The project

`lib/bench/Binacle.Lib.Benchmarks.ParallelBins/` - "then the 5th with discovery" (the maintainer, 2026-09-29).
Every class is pinned through `JobsByCoreCount` in `shared/test/Binacle.Benchmarking` ("yes common code to shared").

The project has `IdenticalBase` and a class per algorithm on it: identical bins and items from `IdenticalCase` in
`lib/test/Binacle.Lib.Testing`, Loop against Parallel, all four core counts `JobsByCoreCount` has. **The class
set and its grids are the shape section below.** Recipe `lib-parallel-bins-identical` takes the algorithm, and
`dry` to check first. Setup throws if any bin is not fully packed. The maintainer, 2026-09-30: "yes make sure they always pack everything fully meaning 256 item fit int box".

**Leaning yes (the maintainer, 2026-09-29):** one class per bin-set category, and one recipe that takes the
category. "i think so". A class each, because BenchmarkDotNet's `--filter` picks classes and methods, not parameter
values.

**How the cores are picked:** `JobsByCoreCount` runs every class on 2, 4, 8 and 12 cores.

**(the maintainer, 2026-09-30)** - what each case records: "cores... bins count and items count ... flattened
count". Items count is the item lines; flattened count is read here as the pieces, every quantity added up.
`BinProcessorFactory.Create` sees only bins count and items count today.

**Leaning yes (the maintainer, 2026-09-30):** synthetic cases first - "i believe if we make synthetic we can
derive the rest". Start with the ideal case, identical bins and identical items - "we can start with idea
scenario identical bins nad items?". The other categories are to be reviewed - "we havent figured out the rest".

**(the maintainer, 2026-09-30):** one step at a time - "i say we measure 1 step at a time and build the next...
perhaps we can get some insight so prioritize them by what they need to run before them". Each step is built only
after the one before it has run.

### The shape - six classes

**Built and dry-checked 2026-10-03**, in `lib/bench/Binacle.Lib.Benchmarks.ParallelBins/Identical/`: a Pieces
and a Lines class for each of FFD, WFD and BFD, two methods each. The class set, one grid per kind, the folder,
and why the Lines classes measure an expected nothing are decided and written up in the lib decisions ledger
under `.agents/design/lib/`; what each class is now is in the lib tests doc under `.agents/docs/lib/`.
**Nothing about the shape is open here.**

What bites whoever runs this:

- **The grids are nobody's decision yet.** Pieces kind: bins 1 to 32 by pieces 1 to 256 on one line, 504
  cases. Lines kind: 4 bins, 64 and 256 pieces over 1, 4, 16 and 64 lines, 64 cases. Both carried over from
  the removed runs. **Suggested (agent, 2026-10-03):** leave them until a run says otherwise.
- **If a Lines class is not flat, there is one place to look.** The algorithm's constructor calls
  `CalculateVolume` and `CalculateLongestDimension` once per line, outside the quantity loop, so N lines of
  one pays N and one line of N pays one. Two multiplies and a comparison each, so it would only show at the
  small piece counts.
- **The call site uses the default cap**, because this project asks whether parallel pays in production.
  `ParallelLimits.NoLimit` would be asking `ParallelOverhead`'s question instead, and switching is his call.

### What is left here - agent, 2026-10-03

**Read this before reading any number further down.** `ParallelBinProcessor` changed on 2026-10-02: it now caps
`maxDegreeOfParallelism` at `min(bins, cores)` by default instead of running unlimited. So `Bins = 1` resolves
to a degree of 1 and `Parallel.For` runs the body inline, where before it paid a dispatch and got nothing for
it - which is what "Parallel never wins on 1 bin" in the old reading was measuring. And 32 bins on the 2-core
job is capped at 2, where before the thread pool could inject more. **Those two cells of the old numbers are
dead; from 2 bins up on 4 cores and above they still describe the code.**

**Open, and his call:** the classes call `ParallelBinProcessor` on its default cap, because this project asks
whether parallel pays in production and the cap is production now. Passing `ParallelLimits.NoLimit` instead
would measure the uncapped dispatch, which is what `ParallelOverhead` is for.

**The lines question is now asked on all three algorithms.** The FFD run was flat across 1 to 64 lines, so lines
cost nothing once they are pieces. The 2026-10-02 suggestion to drop the class is **superseded** - see the shape
section: he chose a Lines class per algorithm, to prove the negative rather than argue it.

What is left, in order:

1. The WFD and BFD runs, one at a time with nothing else running: `just bench lib-parallel-bins-identical WFD`,
   then `BFD`. Each report goes under `lib/results/benchmarks/baseline/parallel-bins/`, with a row in
   `lib/results/benchmarks/baseline/README.md`, and its time in the recipe's comment. The questions they
   answer: do WFD and BFD stop near the same Ratio as FFD, where do they start to win, and is each Lines class
   flat against its Pieces twin?
2. Read them with him, then pick the next step with him. Each is built only after the one before has run, so
   only one can be built at a time.

**Identical** (was `Even`) - needs nothing. FFD run and kept 2026-10-03; WFD and BFD built and waiting on a run.
- Answers: what Parallel costs on its own, and the fastest a bin can pack.
- Its box is an exact multiple of the item, so every piece takes the first free space. Its time per piece is the
  lowest any request can have - never read a piece count off it.

`parallel-bins.md` is the maintainer's to shape; it is built with the other lib results files.

### What the FFD run shows - agent, 2026-10-03

**The FFD run has been read out and its findings are recorded.** In short: the drop point is a surface - more
bins win earlier, more cores win later - and Parallel never captures more than about 2x however many bins
there are. The 1-bin row reads as the cap's receipt, and lines cost almost nothing once they are pieces. The
reading supersedes the 2026-10-01 one taken off the removed pre-cap report.

What stops Parallel near 0.55 is still not known. It is not the bin count, the core count or the request size.

**Suggested (agent, 2026-10-03):** the likeliest cause is memory, not scheduling. Each bin builds its own piece
array, so 32 bins by 256 pieces allocates 32 copies, and `Fake_Bins` in `ParallelOverhead` already showed the
cost of handing a bin to another CPU growing far faster than the bin count once each bin walks 64 KB. Nothing
measures this yet.

#### Lines

The negative the maintainer asked for is measured and recorded: lines are flat at 256 pieces and cost a few
per cent at 64 pieces over 64 lines, which is the per-line work in the constructor. So the rule can read
pieces.

#### Noise

No cell of either FFD report looked like the slow whole-process problem that spoils the WFD racing rows.

### What the overhead project measured for bins - agent, 2026-10-02

`Fake_Bins` in `Binacle.Lib.Benchmarks.ParallelOverhead` runs the bin processors with algorithms that do not
pack, so it gives the cost side of the bins rule without any packing in it. The kept run has been read out and
its findings are recorded.

**What it means for the rule here:** `C` is not one number per machine. It rises with the bin count and with
how much memory a bin touches, so "Parallel costs its setup plus the slowest bin" understates it - the setup
term is itself a function of the request.

### What bites

- **Each bin is packed on its own, with the whole item list.** Loop costs the sum of the bins. Parallel costs its
  setup plus the slowest bin, while there are enough cores.
- **Lines only matter while they are turned into pieces.** After that FFD v2 works per piece: a sort, then for
  each piece a scan of the free spaces, up to 6 turns. **Measured 2026-10-03** - flat at 256 pieces, and up to
  10 per cent dearer at 64 pieces over 64 lines, which is the per-line work in the constructor.
- **A bin that fits only some of the items is the slowest.** A piece that does not fit scans every space 6
  times. A bin that fits nothing is the fastest - its space list stays at one. Packing never stops early; only
  fitting does. In the fixtures the first word of a result is packing, the second fitting - the shared slice doc
  has the format.
- **Identical items are the cheapest work per piece.** A drop point read in pieces from them sits too low for mixed
  items. Read it in time per bin, then map time to pieces from mixed items.
- **The rule cannot see time.** It sees the request. A case has to be described in what the request carries;
  time is only how the case is measured.
- **Each bin builds its own piece array**, so memory grows with bins times pieces.

## The lib results files

The files in `lib/results/` are placeholders. Above each table a comment is its spec - what it shows, the
raw file it reads, its rows, its columns, how a number is made. Under it, a sample table with fake numbers shows
the shape. The maintainer reviews the shapes before any number goes in.

`parallel-algorithms.md` and `parallel-bins.md` do not exist yet. They read the kept runs of the two parallel
projects. The maintainer shapes them from those runs; then they are built like the rest.

**When a file is filled, two things come out of it**: the `<!-- Table: ... -->` spec comment above each table,
and the "Every number in this file is fake" banner. The maintainer, 2026-10-04: the table specs stay for now,
but what remains after the work is done is in scope.

### Its goal

**(the maintainer, 2026-09-29):** fill the lib results files by hand - "6 is manual for now".

**(the maintainer, 2026-09-29):** the shapes are not pinned down yet - "some sample files exist in /lib/results/
but not pinned down same holsds true for vipaq". Pin each file's shape first, then fill it.

**What the files are for** (the maintainer, 2026-09-26): "i aim to cvreate a functio nthat will optiomaly
determine...but i also aim to provide choice".

### Open points in the lib shapes - suggested (agent, 2026-09-26), to reinvestigate

Each is the maintainer's to settle, one per turn.

- **Bold in `algorithm-performance.md`.** Every value there is × FFD and above 1.00×. Bold every cell, or none?
- **Its memory sentence** has numbers no table holds, against "every number in the words is in a table".
- **It divides numbers from three reports**, run hours apart. That is weaker than a ratio taken inside one class.
- **The version files** have no loss count, though a per-problem maximum above 1.00× shows single losses happen.
- **`result-selection.md`** has no loss count, though its averages mix scenarios on both sides of 1.00×.
- **`version-parity.md`** is a raw file of the fill kind, but no table reads it. Suggested: keep it as a
  correctness check.
- **Headroom:** Ceiling and Best of three per set are taken as means; the shape did not say.
- **"Best or tied"** is counted from the Best column, which holds exact values, not from the rounded fills.
- **`packing-efficiency.md` never says its fills are v2**; only the code does (`Algorithms.Shipped = 2`).
- **`scaling.md`** keeps 5 of the ladder's 11 steps (3, 13, 29, 47, 67) and gives no reason for dropping 7, 17, 23,
  37 and 59.
- **`packing-time-by-size.md`:** "do FFD and WFD spread differently inside a band" has no test yet.
- **The time files name no gaps yet.**
- **Unused raw columns:** Items and Margin in `packing-efficiency.md`, and the v1 rows of the ladder. Suggested:
  keep them; they cost nothing.

### Facts the lib words can use - suggested (agent), to reinvestigate

- **Why v2 allocates so much less than v1 on BFD and WFD, and much less so on FFD.** Measured and recorded;
  the cause is the free-space sort. "Allocated" is memory handed out during one pack, garbage included - not
  peak memory.
- **Say how a number was made when it is not obvious**: StdDev over all 700 is population.
- **The dropped proposal:** one table of v2 packing times per algorithm (fastest, mean, median, slowest). The
  spread was too wide, 7 to 725 μs. `packing-time-by-size.md` shows that spread on purpose, as a finding.

### Where the v1-against-v2 speed story comes from

**The `lib-algorithms-full` run of 2026-09-25** - every Bischoff problem, v1 against v2, packing and fitting -
is kept under `lib/results/benchmarks/baseline/algorithms/` as `Full_<alg>_<op>.md`. It carries the v2/v1 story
over the whole suite, so every speed and memory number in the words comes from there. It ran at the **short**
job (3 iterations), so a single problem's mean is rough; an average over 700 is not. Some of its BFD times are
slow processes, and are not retaken.

The old repo-root `results/` folder and its November 2025 runs are gone, and what was read off them is not kept
(the maintainer, 2026-10-04: "delete them we go by what we have"). The kept run replaces all of it.

### Not in this plan

- A docs-site page copied from the READMEs, and a number on www.
- Comparison against published results on the Bischoff problems: "we fill 81% at X ns, others 87% at Y ns".
  The fill half holds anywhere; the time half only if both ran on one machine.

## The ViPaq results files

**Done.** `format-size.md`, `compressed-size.md`, `encode-cost.md`, `decode-cost.md`, and the README summary.
Seven files became four: the four layout-and-codec size files were one 4-row table, so they are
`compressed-size.md`. `INDEX.md` holds the file list, the README holds the story.

### The shape, settled 2026-10-05 - it holds for the lib files too

Every line is the maintainer's.

- **A results file is consolidated data and nothing else** - "purre consoilidated data ... thats the purpose of
  those result fuiles". No words under a table. A short "What was observed" goes **on top**, as bullets -
  "the words go on top... as a summary... or in the readme.md summary".
- **No gaps section.** What a file does not measure was cut from every one of them, and it does not come back -
  "1 an idea exists".
- **The losses get their own table** - "now the scenarions where it looses mucst be on a table". Named rows
  while they are few, counted per group once they are many. No bold above 1.00× and no loss-count column: the
  table is the catch.
- **Per case, then averaged** - the ratio on one pack, then the mean per set, never a mean divided by a mean,
  "cause it contains varried things".
- **One table per axis, not one per algorithm.** The algorithm gets a single table, a row each, never averaged
  together.
- **Equal comparisons only** - "i want equal comparisons fair equitable". A ratio is taken inside one report.
  No dividing a number in one report by a number in another, and no claim a run did not make: the compression
  tables are ViPaq against itself, because that class has no protobuf row.
- **Each table keeps a two-line comment** - what it says, and the raw file it reads. Nothing more.

### Still open - the measure output, not the results files

- **Raw row and raw columnar are the same length**, character for character, on every pack of every algorithm
  (checked 2026-10-05 by hashing the tables). So the Raw table of every `columnar-*` measurement file is an
  exact copy of the row file's. Drop it from the measure output, or keep it so each layout has its own file?
- **Unused measure columns:** ViPaq/Proto everywhere, a rounded copy of what the tables compute. Drop, or keep?
  Items is unused and stays as context.

## The results READMEs

**`vipaq/results/README.md` carries its summary.** The index moved to `INDEX.md` beside it, so the README holds
the story and nothing else. The shape is a table - the question, the answer, and the files the answer reads -
then the one fact that crosses two files, then a line stating nothing on the page is computed. `lib/results/`
is split the same way, and its README still needs the summary.

A README is a public file, so it carries no planning marker to grep for. Whether the summary is there is read by
eye.

**(the maintainer, 2026-09-29):** each slice's README sums up its results files - "the readme will smarize
them". Every number in it names the file it came from - leaning yes ("perhaps the read me", 2026-09-29).

### Suggested (agent), to reinvestigate

- **Facts from two or more files go in the README**, and only there, since a results file reads one kind of raw
  result.

Combinations seen so far, lib:

- fill bought against time paid, per option - `packing-efficiency-stats.md`, `algorithm-performance.md`,
  `parallel-algorithms.md`
- the pick is not worth optimizing: the slowest pick against the fastest pack - `result-selection.md`,
  `algorithm-performance.md`
- what a cost function can use - `packing-time-by-size.md`, `scaling.md`, `parallel-algorithms.md`, `parallel-bins.md`
- whether the direction is sound - the two version files, `packing-efficiency-stats.md`

ViPaq is done. The one fact that crosses two files and lives only in its README: what the wire win costs in
encode time.

## Done when

**The runs**

- [ ] Every bench class has a kept run in `baseline/`, `ParallelBins` included.
      **By eye.** Each class under `lib/bench/` and `vipaq/bench/` has a report under
      `<slice>/results/benchmarks/baseline/`.
- [x] Every measure has written its file.
      **By eye.** Each measure under `lib/measure/` and `vipaq/measure/` has its file under
      `<slice>/results/measurements/`.
- [ ] `ParallelAlgorithms` holds the five classes, each with `Loop` and `Parallel`, in a folder.
      `ls lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/Identical/` lists the five and their base.
- [x] `Identical_Packing` and its kept report are gone.
      `test ! -f lib/results/benchmarks/baseline/parallel-algorithms/Identical_Packing.md`
- [ ] The five have a kept baseline run, made in one invocation.
      **By eye.** Five reports under `lib/results/benchmarks/baseline/parallel-algorithms/`, every one carrying
      the same job header, and a row each in that folder's README.
- [ ] `Loop` is the sum of its algorithms, checked across the alone and race reports of that one run.
      **By eye.** It is the test of whether the run is clean, so a failing check reopens the run, not the plan.
- [ ] The racing threshold is read out of the runs, or they show there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
- [ ] `ParallelBins` holds the six classes, each with `Loop` and `Parallel`, in a folder.
      `ls lib/bench/Binacle.Lib.Benchmarks.ParallelBins/Identical/` lists the six and their base.
- [x] `Identical_FFD_Packing` and its kept report are gone.
      `test ! -f lib/results/benchmarks/baseline/parallel-bins/Identical_FFD_Packing.md`
- [ ] The six have a kept baseline run.
      **By eye.** Six reports under `lib/results/benchmarks/baseline/parallel-bins/`, with a row each in that
      folder's README and the real time in the recipe's comment.
- [ ] Each Lines class is read against its Pieces twin: flat, or the difference is named and explained.
      **By eye.** Flat is the expected answer, and a negative is the point of running them.
- [ ] The bins drop point is read out of it, or the report shows there is none, and the lib findings record says so.
      **By eye.**

**The results files**

- [ ] The maintainer pinned the shape of each lib results file and each ViPaq results file.
      **By eye.** His yes for each file, quoted and dated, in this file.
- [ ] `parallel-algorithms.md` and `parallel-bins.md` exist in the shape the maintainer gave.
      `ls lib/results/parallel-algorithms.md lib/results/parallel-bins.md`
- [ ] Every lib results file has real numbers, filled by hand, and its words and gaps are written from them.
      `! grep -l "is fake" lib/results/*.md`, then **by eye**.
- [x] Every ViPaq results file has real numbers and a summary written from them.
      `! grep -l "is fake" vipaq/results/*.md`, then **by eye**. Done 2026-10-05. Seven files became four.
- [ ] Each filled file has lost its table specs and its fake banner.
      `! grep -l "^Table:" lib/results/*.md vipaq/results/*.md` and
      `! grep -l "is fake" lib/results/*.md vipaq/results/*.md`
- [x] The old lib and ViPaq numbers kept in this file are used to check the new ones, or dropped with a reason.
      Done 2026-10-04 on the maintainer's "delete them we go by what we have": every old number a kept file
      could check was checked and recorded, and the rest is gone.

**The READMEs**

- [ ] Both READMEs carry the summary in the shape the maintainer gave, and every number in it names its file.
      **By eye.** Open `lib/results/README.md` and `vipaq/results/README.md`. Under the index there is a summary
      section, and every number in it says which results file it came from. No summary, or a bare number, and
      the box stays open.
