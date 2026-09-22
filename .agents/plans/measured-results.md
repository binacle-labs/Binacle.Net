---
description: Orchestrator - steps 1 to 11 landed; what is left is the bench tooling rework, the findings from the review of steps 1 to 12, and the first keepers. The maintainer commits between steps
state: ready
waits-on: "the maintainer picks the bench tooling shape in the findings file; the other findings can start now"
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
the four project folders and the one on measured numbers. Step 12 is built, bar its tooling. Step 13, which
converted the old `results/` folder, was dropped 2026-09-22 when the folder was deleted.

- [`findings.md`](measured-results/findings.md) - what the 2026-09-22 reviews of steps 1 to 12 found: the
  bench tooling the maintainer wants reworked, what broke or checks less, text the moves made false, and
  drift from the shape. A finding leaves it when fixed; the file goes when empty.

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
| 12 | [12-bench-split](measured-results/12-bench-split.md) | built: five benchmark projects, their tiers, the shared config, `bench.just`. Open: the tooling rework in the findings file | `test ! -f tooling/bench.run.sh` |
| 14 | [14-first-keepers](measured-results/14-first-keepers.md) | the scaling class and the JSON timing; then `lib-algorithms-smoke`, `vipaq`, the bin threshold once, their benchmarks READMEs, and the bin-threshold finding | `test -f lib/results/benchmarks/README.md` |

## Done when

- [ ] Step 14's last box is ticked, and the maintainer has deleted the folder and this file.
      **By eye.** There is nothing left to check once the file is gone; the step files carried the checks.
