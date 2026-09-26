---
description: Orchestrator - steps 1 to 17 landed, and the provider names; left are 18 the lib results files, 19 the racing drop point, 20 the bins one and 21 the ViPaq results files, plus the rules both results slices share. The maintainer commits between steps
state: ready
waits-on: "a session of its own for step 18 and one for step 21 - the maintainer says when"
horizon: next-release
paths:
  - "shared/**"
  - "lib/**"
  - "vipaq/**"
  - "tooling/**"
  - "results/**"
  - ".agents/**"
---

# Measured results, and the projects that feed them

One plan, too big for one file. The maintainer granted the topic folder on 2026-09-19; this file is the only
one that points at every file in it.

**Steps 1 to 11 landed 2026-09-20 to 2026-09-22 and their files are gone.** The three tests kernels became
`Data` and `Testing` projects and `Binacle.Reporting`; both measure projects write `<slice>/results/`; the
old vault stopped being current. Their lasting rules are in the general design record - the decision on
the four project folders and the one on measured numbers. Step 12, the bench split, landed 2026-09-23; its
reasons are in the lib and ViPaq design records. Step 13, which converted the old `results/` folder, was
dropped 2026-09-22 when the folder was deleted.

**Step 15 landed 2026-09-24** and its file is gone. `Binacle.Lib.Benchmarks.Scaling` is a project of its own
with one class, `Sample_Packing`, 66 cases over the item ladder; `Json` is a row in ViPaq's two encode
classes. All three runs are kept in `baseline/`.

**Step 17 landed 2026-09-23** and its file is gone. `encoded-size.md` became 54 files under
`vipaq/results/measurements/encoded-size/<algorithm>/`, three tables each, every format through every codec. A dropped
reporter's old file is deleted by hand; the general design record says so, and wanting a check for it is an
idea in the shared plans.

**Step 16 landed 2026-09-25.** The maintainer ran `lib-threshold-full precise` and `lib-algorithms-full`; the
ten reports are the first kept run of both full tiers, in `baseline/threshold/` and `baseline/algorithms/`. The
answer is F4 in the lib findings record - parallel bin processing pays above a surface of bin count and item
count, and loses badly below it - with F2a settling parallel algorithm racing against. O1 in the lib decisions
record carries both, and what is still undecided.

**Step 14 landed 2026-09-23** and its file is gone. The kept-run shape, the rule for what is worth keeping,
and how a report is copied all live in the two `benchmarks/README.md`. Copying stays a hand job; the recipe
that would do it is an idea in the tooling plans.

**The provider names landed** and their file is gone.

## How a session works this plan

- **A session takes one step or several; the maintainer commits between steps.** No session commits. A step
  is sized to be one reviewable commit - a rename, a move, a split, a harness - never a line in three files
  and never two ideas. Where a step says it is small, the maintainer may take it with the one before.
- **A session says when it should stop.** Its judgement, not a count of steps: when its context is heavy, or
  when the next step deserves a cold read. It says so at a step boundary, never in the middle of one, and
  names the step a fresh session picks up. The maintainer decides whether to start a new session or go on.
- **Pick up cold by running the gates.** Every row below has a gate: one command that is true once the step
  landed. Run them top to bottom; the first that fails is the next step. There is no progress table - the
  tree is the state, and a tick nobody verified is a claim.
- **Settle the open details first.** Each step file lists what it leaves open. Work those out with the
  maintainer, then start. A session that finds the shape wrong when it meets the code says so with the
  evidence and stops; a wrong decision is cheaper to change than to obey.
- **The sandbox denies `mv`, `rm` and `git mv`.** Hand the maintainer the lines; edit after they land.
  History follows the bigger half of a split.
- **Build only what moved.** `dotnet build <csproj>` on the project and each consumer. A solution build or a
  long run is the maintainer's; both have crashed the machine.
- **Every step leaves a working tree**: every project builds, every script or recipe that exists still runs.
  A step that has to break something fixes it in the same step.
- **Every step rewrites the doc lines it makes false**, in the same step. `just agents all` is the
  maintainer's; say when it is due.

## The results files - rules for steps 18 and 21

Settled with the maintainer on 2026-09-25 and 2026-09-26. Both slices build to them; the two step files say
only what is particular to each.

**What they are for.** The story is for the maintainer first: it shows him how the work is going and gives him
data to decide on - v2 against v1 says whether the direction is sound, a tipping point feeds a cost function, a
gap says what to improve. A file that drives no decision and shows no progress is a candidate to cut. The story
for the outside world comes after, from the same files.

**The layout, per slice:**

| Path | Holds |
|---|---|
| `<slice>/results/measurements/` | raw results from `just measure` |
| `<slice>/results/benchmarks/` | raw results - BenchmarkDotNet reports, copied by hand |
| `<slice>/results/<file>.md` | derived: one file per question, which may read several raw files and supports decisions inside that area only |
| `<slice>/results/README.md` | a short summary of every root file, then the combinations - facts from two or more root files, grouped by the decisions only a combination can make - then the open questions and gaps gathered from the root files. The folder index goes below. Written last, once every root file of the slice is done. Its wording is revised later |

- **Each slice stays isolated.** No root file or README reads another slice's numbers. A decision that spans
  slices - ViPaq's encode time against lib's pack time, what one request costs end to end - has no home yet;
  the maintainer does not know where it goes, only that it is in neither slice.
- **A script makes the tables; a person writes the words.** Every table is generated from the raw files, never
  typed. Everything else is written by hand from what the tables show, and rewritten when they move. Root
  files carry few words; the README may carry more. How the script refreshes tables without touching the
  words is open - markers around each table are one way.
- **The README computes nothing.** Every number in it comes from a root file and names it. When a root file is
  rewritten, the README is reread.
- **A difference is "×" the baseline**: v2 at 0.46× of v1, BFD at 4.2× of FFD. A share of a whole may be a
  percentage.
- **A ratio is per problem or per pack, then averaged.** Divide by the baseline on that same problem, then take
  the mean (and min, median, max where the table says). Never mean A ÷ mean B, which lets big cases outweigh
  small ones. BenchmarkDotNet's own Ratio is used where one run holds both rows.
- **A loss must show.** Where 1.00× or more is the bad side, the average goes bold, and where an average can
  hide single losses, a count column says how many.
- **Every cell one number, every column labelled.** No blank cells, no "a / b" cells.
- **Each file names its gaps and its open questions** in a line or two at the end. A gap is written down so it
  can be added later; filling it is new measurement, not a table.
- **v2 everywhere, except where v1 against v2 is the question.** Name the version.
- **A time holds on one machine and runtime only; the file says which.** Ratios, memory, fill and size compare
  anywhere.
- **Build only when the maintainer says**, and run no script while a bench run is going - it disturbs the run.
- **A script reads the raw files it knows by name**, never whatever sits in the folder: a stale file left by a
  dropped reporter is never read.
- **Open, his call:** whether the tables later come from the measure harness instead of a plan script.

## The steps

| # | File | In one line | Gate |
|---|---|---|---|
| 18 | [18-results-story](measured-results/18-results-story.md) | the lib results files and README, shapes locked | **by eye** - `lib/results/` holds every file in the step and the README opens with the summary |
| 19 | [19-racing-drop-point](measured-results/19-racing-drop-point.md) | the racing drop point, on 2 to 12 cores, over 30 locked problems | `ls lib/results/benchmarks/*/racing/` shows the new report |
| 20 | [20-bins-drop-point](measured-results/20-bins-drop-point.md) | the bins drop point, bins of one size, shape agreed, waits on 19 | `ls lib/results/benchmarks/*/threshold/` shows the new report |
| 21 | [21-vipaq-results](measured-results/21-vipaq-results.md) | the ViPaq results files and README, shapes locked; a bench column and a rerun first | **by eye** - `vipaq/results/` holds every file in the step and the README opens with the summary |

18 reads what 15 to 17 left. `python3 .agents/scripts/derive-lib-results.py`, run from the repo root, writes its
derived result files.

## Done when

- [ ] Every step's last box is ticked, the provider names are done or dropped, and the maintainer has
      deleted the folder and this file.
      **By eye.** There is nothing left to check once the file is gone; the step files carried the checks.
