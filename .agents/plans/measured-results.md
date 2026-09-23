---
description: Orchestrator - steps 1 to 14 and 17 landed; left are 15 the new bench classes, 16 the bin-threshold finding, 18 the results story, and the provider names. The maintainer commits between steps
state: ready
waits-on: "the maintainer runs the three recipes step 15 needs, and the long run step 16 needs"
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

**Step 17 landed 2026-09-23** and its file is gone. `encoded-size.md` became 54 files under
`vipaq/results/encoded-size/<algorithm>/`, three tables each, every format through every codec. A dropped
reporter's old file is deleted by hand; the general design record says so, and wanting a check for it is an
idea in the shared plans.

**Step 14 landed 2026-09-23** and its file is gone. The kept-run shape, the rule for what is worth keeping,
and how a report is copied all live in the two `benchmarks/README.md`. Copying stays a hand job; the recipe
that would do it is an idea in the tooling plans.

- [`consistent-provider-names.md`](measured-results/consistent-provider-names.md) - one naming rule for every
  scenario provider in the Data and Testing projects. Not a numbered step: nothing depends on it, and it
  depends on nothing.

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

## The steps

| # | File | In one line | Gate |
|---|---|---|---|
| 15 | [15-new-bench-classes](measured-results/15-new-bench-classes.md) | the scaling class and the Json row, their first runs kept | `grep -rq JsonEncoder vipaq/bench --include=*.cs` |
| 16 | [16-bin-threshold-finding](measured-results/16-bin-threshold-finding.md) | the long threshold run, and the bins finding in the lib design record | `! grep -q "no finding yet" .agents/design/lib/findings.md` |
| 18 | [18-results-story](measured-results/18-results-story.md) | the results READMEs as a story, from everything above | **by eye** - the READMEs open with sentences and numbers |

The order is the work's: 15 and 16 keep their runs in the shape 14 set; 18 reads what 15 to 17 left.

## Done when

- [ ] Every step's last box is ticked, the provider names are done or dropped, and the maintainer has
      deleted the folder and this file.
      **By eye.** There is nothing left to check once the file is gone; the step files carried the checks.
