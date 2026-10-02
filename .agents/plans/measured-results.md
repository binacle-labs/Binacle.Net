---
description: Orchestrator - fill every bench baseline and measure of lib and ViPaq, write one results file per question, then each slice's README; five sessions
state: ready
waits-on: "the maintainer says when each session starts"
horizon: next-release
paths:
  - "shared/**"
  - "lib/**"
  - "vipaq/**"
  - "tooling/**"
  - ".agents/**"
---

# Measured results

One plan in a topic folder, granted 2026-09-19. This file is the only one that points at the files in it.

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

## Where it stands

Every root results file is a placeholder: a comment per table saying what it holds, and a sample table with
fake numbers under it. Both results READMEs are an index.

## What has run, and with which job

Read out of each kept report's own header, not from the recipe comments: a short-job report carries
`IterationCount=3  LaunchCount=1`, a default-job one prints no characteristics line at all, and a dry one says
`RunStrategy=ColdStart`. Dates and case counts live in `lib/results/benchmarks/baseline/README.md` and
`vipaq/results/benchmarks/baseline/README.md`; this table does not repeat them.

**The times are the recipes' own estimates, not measured wall clock.** Nothing records how long a run actually
took.

| Recipe | Kept | Job | Time | No kept run |
|---|---|---|---|---|
| `lib-algorithms-smoke` | 6 classes | quick | about 6 min | - |
| `lib-algorithms-sample` | 6 classes | precise | about 30 min | - |
| `lib-algorithms-full` | 6 classes | quick | about 16 hrs | precise, about 30 hrs |
| `lib-parallel-algorithms-identical` | `Identical_Packing` | quick | 20 to 30 min | precise |
| `lib-parallel-bins-identical` | `Identical_FFD_Packing`, `Identical_FFD_Lines_Packing` | quick | about 1 hr for FFD | WFD, BFD |
| `lib-parallel-overhead` | nothing | - | about 40 min precise, 12 quick - an estimate, no run at this grid | all three classes |
| `lib-result-selection` | 3 classes | quick | about 3 min | precise |
| `lib-scaling` | `Sample_Packing` | precise | about 20 min | - |
| `vipaq-smoke` | 2 classes | quick | about 3 min | precise |
| `vipaq-sample` | 4 classes | precise | about 30 min | - |

Every kept report belongs to a class that still exists - checked file by file against `lib/bench/` and
`vipaq/bench/`. There are no orphans.

**The measures have all run, and they take no job.** A measure gives the same result on every run and every
machine, so there is no quick or precise for it and no noise to average out. `just measure lib` wrote both
files under `lib/results/measurements/`; `just measure vipaq` wrote all of `vipaq/results/measurements/`.
Neither recipe states a time.

## Kept reports that no longer match the code

The parallel processors changed on 2026-10-02: `concurrencyLevel` left the constructors and
`maxDegreeOfParallelism` now defaults to `ParallelLimits.Degree`. That reaches three kept reports.

- **`parallel-bins/Identical_FFD_Packing.md` - two cells are stale.** `Bins = 1` now resolves to a degree of 1,
  so `Parallel.For` runs the body inline where it used to pay a dispatch and get nothing for it. And 32 bins on
  the 2-core job is now capped at 2, where the thread pool could previously inject more. Everything from 2 bins
  up on 4 cores and above is unchanged. **Undecided:** the three options put to the maintainer on 2026-10-02 -
  delete the stale reports, overwrite them as new runs arrive, or move the parallel set to a dated folder and
  rebuild `baseline/`.
- **`parallel-bins/Identical_FFD_Lines_Packing.md` - its 2-core rows may have moved the same way**, since it
  holds 4 bins against a 2-CPU job. Its 4-, 8- and 12-core rows are unaffected. Suggested (agent, 2026-10-02):
  the class goes, because its question is answered - its kept run is flat across 1 to 64 lines, which was
  checked line by line (64 pieces reads 15.6 to 16.9 us across all four line counts).
- **`parallel-algorithms/Identical_Packing.md` - its `Allocated` column predates the lock-array change.** A
  race of two used to size its result dictionary for 12 writers and now sizes it for 2. The times are
  unaffected. The size of the drop is read from the plan's own research, about 500 B, and **has not been
  re-measured**; the next run of this class settles it.

**The three ParallelOverhead reports in the project's ignored artifacts folder are now of a shape that no
longer exists** - they carry the 32-bin rows that left the grid on 2026-10-02. They are the only evidence
behind this plan's memory findings, and `.gitignore` excludes the folder they sit in.

**Every future report's header will differ from every kept one**, because the job word is now part of the job
id - `short 02 cores` where a kept report reads `02 cores`. That is a label, not a measurement.

## Who runs what

- **The maintainer runs** every bench, every measure, and any build that starts the host. A session gives him
  the one-line command. A long run takes the whole machine while it runs. ("the first is true", 2026-09-29)

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
- **Nothing runs while a bench runs** ("i am waiting on the benchmark to complete so dont urn anything",
  2026-09-26).

## The sessions

| # | File | In one line | Gate |
|---|---|---|---|
| 4 | [04-racing-drop-point](measured-results/04-racing-drop-point.md) | `ParallelAlgorithms`: `Identical_Packing` is run on the short job; re-run it on the default job, then add and try; read where racing starts to pay | `ls lib/results/benchmarks/*/parallel-algorithms/Identical_Packing.md` |
| 5 | [05-bins-drop-point](measured-results/05-bins-drop-point.md) | `ParallelBins`: `Identical`, one class per algorithm - FFD is run, WFD and BFD next; then add and try; read the bins parallelisation threshold | **by eye** - a kept run of a new class under `lib/results/benchmarks/*/parallel-bins/` |
| 6 | [06-lib-results](measured-results/06-lib-results.md) | fill the lib results files by hand ("6 is manual for now", 2026-09-29) | `! grep -l "is fake" lib/results/*.md` |
| 7 | [07-vipaq-results](measured-results/07-vipaq-results.md) | fill the ViPaq results files the same way | `! grep -l "is fake" vipaq/results/*.md` |
| 8 | [08-results-readmes](measured-results/08-results-readmes.md) | shape and write the summary of both results READMEs | `! grep -l "shape not decided" lib/results/README.md vipaq/results/README.md` |

The order comes from the work. The results files read the kept runs, `ParallelAlgorithms` and `ParallelBins` included. The READMEs
read the results files. The sessions keep their numbers 4 to 8.

**Slow processes are not chased here** (the maintainer, 2026-09-28: "we wont chase that now its too much").

**Every results file that reads a BFD time names it as a gap** (the maintainer, 2026-09-29: "keep it, name it
as a gap and name explicitly for what it is upfront its the optimization acting up"). Up front, in plain words:
.NET's run-time optimization (tiered PGO) sometimes picks a worse version of the code and keeps it for a whole
process, so that process runs slow from start to end. It still gains over PGO off, only less ("we steill get
the boost from pgo-off but not as much", 2026-09-29).

## Done when

- [ ] Every bench class has a kept run in `baseline/`, `ParallelBins` included.
      **By eye.** Each class under `lib/bench/` and `vipaq/bench/` has a report under
      `<slice>/results/benchmarks/baseline/`.
- [ ] Every measure has written its file.
      **By eye.** Each measure under `lib/measure/` and `vipaq/measure/` has its file under
      `<slice>/results/measurements/`.
- [ ] Every root results file has real numbers.
      `! grep -l "is fake" lib/results/*.md vipaq/results/*.md`
- [ ] Each slice's README is written.
      `! grep -l "shape not decided" lib/results/README.md vipaq/results/README.md`
