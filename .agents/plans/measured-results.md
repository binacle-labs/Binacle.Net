---
description: Orchestrator - the benchmarks, measurements and results files of lib and ViPaq, left as eight sessions in order - slow runs, their fix, the reruns, the racing and bins drop points, the lib and ViPaq results files, and the results READMEs
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

## What landed

The bench and measure projects, their recipes, and the kept runs under `lib/results/` and `vipaq/results/`
landed 2026-09-20 to 2026-09-26. Their lasting rules are in the general design record (the four project
folders, measured numbers) and in the lib and ViPaq design records. The racing bench `Cores_Packing` is built;
no run of it is kept.

On 2026-09-26 the plan was reset. The table scripts were removed. Every results file became a placeholder: a
comment per table saying what it holds, and a sample table with fake numbers under it. The two results READMEs
were cut to an index.

## How a session works this plan

- **One session, one file, in order.** Run the gates in the table below top to bottom; the first that fails is
  the next session. The maintainer commits between sessions. No session commits.
- **Do only what the file says.** A problem found on the way: say it in one plain sentence and stop. New work
  becomes an idea plan, never a new step.
- **No scripts in the repo** until session 6 decides how the tables get their numbers.
- **The maintainer runs** every bench, every measure, and any build that starts the host. A session gives him
  the one-line command. Long runs have crashed his machine.
- **The sandbox denies `rm`, `mv` and `git rm`.** Hand the maintainer one line with every path on it.
- **Every session rewrites the doc lines it makes false**, in the same session. `just agents all` is his.
- **Plain short words**, in chat and in files. One decision per turn, shown with an example.

## The results files - rules

Settled with the maintainer 2026-09-25 and 2026-09-26. Both slices keep them.

**What they are for.** They show the maintainer how the work is going and give him data to decide on: v2
against v1 says whether the direction is sound, a drop point feeds a cost function, a gap says what to
improve. A file that drives no decision and shows no progress is a candidate to cut.

| Path | Holds |
|---|---|
| `<slice>/results/measurements/` | raw results from `just measure` |
| `<slice>/results/benchmarks/` | raw results - BenchmarkDotNet reports, copied by hand |
| `<slice>/results/<file>.md` | one file per question, read from the raw files |
| `<slice>/results/README.md` | the index, and a summary written last |

- **Each slice stays apart.** No file reads the other slice's numbers.
- **A file reads one kind of raw result**: a slice's measurements, or one bench family (lib: algorithms,
  result-selection, scaling, racing, threshold; ViPaq: encoding). Inputs - the problems under `shared/data/`,
  the ladder in `LadderGenerator.cs` - are not results; any file may read them.
- **Never average two algorithms together.** A race - the better of its members on each problem - is an option
  of its own, not an average, and stays.
- **A difference is "×" the baseline**: v2 at 0.46× of v1. A share of a whole may be a percentage.
- **A ratio is per problem, then averaged.** Never mean A ÷ mean B. BenchmarkDotNet's own Ratio is used where one
  run holds both rows.
- **A loss must show.** Where above 1.00× is the bad side the average goes bold, and where an average can hide
  single losses a count column says how many.
- **Every cell one number, every column labelled.**
- **Each file ends with its gaps and open questions.**
- **v2 everywhere, except where v1 against v2 is the question.**
- **A time holds on one machine and runtime only; the file says which.** Ratios, memory, fill and size compare
  anywhere.
- **The comment above each table is its spec**, and the sample table under it shows the shape. Filling a table
  replaces the fake numbers; the comment stays. The line "Every number in this file is fake" goes when the last
  table of the file is filled.
- **The README computes nothing.** Every number in it names the file it came from.
- **Nothing runs while a bench runs** - it disturbs the run.

## The sessions

| # | File | In one line | Gate |
|---|---|---|---|
| 1 | [01-slow-runs](measured-results/01-slow-runs.md) | find why some runs come out slow at random; choose the fix with the maintainer | **by eye** - the file records the cause, or "not found", and the fix he chose |
| 2 | [02-fix-slow-runs](measured-results/02-fix-slow-runs.md) | build that fix; a small run proves it | **by eye** - the small run's numbers are in the file and show no slow run |
| 3 | [03-reruns](measured-results/03-reruns.md) | the maintainer reruns the benches whose times are wrong, and keeps them | **by eye** - every run on its list is kept, or his word to keep the old one is written |
| 4 | [04-racing-drop-point](measured-results/04-racing-drop-point.md) | run the racing bench, keep it, read where racing starts to pay | `ls lib/results/benchmarks/*/racing/Cores_Packing.md` |
| 5 | [05-bins-drop-point](measured-results/05-bins-drop-point.md) | build and run the bins bench, read where packing bins at once starts to pay | **by eye** - a kept run of the new bins class under `lib/results/benchmarks/` |
| 6 | [06-lib-results](measured-results/06-lib-results.md) | decide how the tables get their numbers; fill the lib results files | `! grep -l "is fake" lib/results/*.md` |
| 7 | [07-vipaq-results](measured-results/07-vipaq-results.md) | fill the ViPaq results files the same way | `! grep -l "is fake" vipaq/results/*.md` |
| 8 | [08-results-readmes](measured-results/08-results-readmes.md) | shape and write the summary of both results READMEs | `! grep -l "shape not decided" lib/results/README.md vipaq/results/README.md` |

The order comes from the work. A rerun before the fix hits the same fault. Racing and bins read the reruns.
The results files read the kept runs. The READMEs read the results files.

Kept as ideas, outside this plan: MessagePack, CBOR and a columnar protobuf; a results story for others; a home
for comparisons across slices.

## Done when

- [ ] Every session file's boxes are ticked, and the maintainer has deleted the folder and this file.
      **By eye.** There is nothing left to check once the file is gone; the session files carried the checks.
