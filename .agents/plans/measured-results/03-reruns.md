---
description: Session 3 - after the slow-run fix, the maintainer reruns the benches whose times are wrong or suspect, and the session keeps the reports
state: blocked
waits-on: "session 2 - the fix, proven by its small run"
horizon: undecided
paths: ["lib/results/benchmarks/**", "vipaq/results/benchmarks/**"]
---

# 3 - Reruns

Every kept report was checked against the code on 2026-09-26: no code change since a run made it stale. What
makes a report wrong is the slow-run fault of session 1. The racing run is not here; it is session 4.

## Must rerun - the times are wrong

| Recipe | Why | How long |
|---|---|---|
| `just bench lib-algorithms-full` | about 1 in 10 BFD rows are slow processes | the recipe says about 12 hours; the last run took about 16 |
| `just bench lib-scaling` | one process per case; BFD v2 jumps 1.45× from 67 to 79 items while FFD rises 1.2× | about 20 minutes |

## Suspect - the maintainer says rerun or keep

| Recipe | Why | How long |
|---|---|---|
| `just bench lib-algorithms-sample` | 8 of 60 BFD packing rows differ from the full run by more than 25% | about 2 hours at the default job |
| `just bench lib-threshold-sample` | the three-algorithm Loop is 1.18× to 1.85× its parts | about 1 hour at the default job |
| `just bench lib-threshold-full precise` | the same, 1.26× to 1.46×; it cannot run v2 alone, so v1 reruns too | the recipe says about 3 hours; the last run took about 5 |
| `just bench vipaq-sample` | rows moved with no code change, protobuf's too | about 30 minutes |

No results file reads the threshold reports; the maintainer keeps them as they are.

**If session 2 changed product code**, the smoke runs time the changed code too: `just bench lib-algorithms-smoke`
and `just bench lib-threshold-smoke`, a few minutes each.

## Fine as they are

- The lib and ViPaq measurements, unless session 2 moved them.
- `lib-result-selection` - it times only the pick from results already made, no packing.
- The smoke runs, if no product code changed.

## Keeping a report

How a report is kept, and when a run is worth keeping, is in `lib/results/benchmarks/README.md` and
`vipaq/results/benchmarks/README.md`. An old report stays until its replacement is kept.

## Done when

- [ ] Both "must" runs are kept, and their folder README gives a date after session 2's fix.
      **By eye.** `lib/results/benchmarks/*/algorithms/Full_*` and `*/scaling/Sample_Packing.md`, and the README beside them.
- [ ] Each suspect run is rerun and kept, or the maintainer's word to keep the old one is written here.
      **By eye.**
