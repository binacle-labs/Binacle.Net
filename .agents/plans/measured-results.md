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

## Who runs what

- **The maintainer runs** every bench, every measure, and any build that starts the host. A session gives him
  the one-line command. Long runs have crashed his machine. ("the first is true", 2026-09-29)

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
