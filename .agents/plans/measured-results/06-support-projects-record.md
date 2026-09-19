---
description: Step 6 - the folder rules go into the design record, the dependency docs are redrawn end to end, the shape file is deleted
state: ready
waits-on: "step 5's gate"
horizon: next-release
paths: [".agents/design/**", ".agents/docs/**"]
---

# Step 6 - the record

Shape: [support-projects.md](support-projects.md). Protocol: the orchestrator beside this folder.

## The step

- The rules from the shape - the four-folder table, the three sentences, the ViPaq sentence - go into the
  repo-wide design record as one decision with its date and the reason (three copies of a reader, three of
  a factory, two projects named for tests that were not). The ViPaq sentence goes into the vipaq record
  beside it. The three grep lines from the shape go in as the check.
- Lib D3 and vipaq D10 are superseded in place if steps 1 and 2 did not finish the job.
- The shared, lib and vipaq dependency docs are read end to end against the shape's tree, not patched: every
  graph, every table row, every note. The lib tests doc, the shared README and both slice READMEs the same.
- The shape's own `Done when` is run in full and every box ticked; then the shape file is deleted. The
  maintainer runs `just agents all`.

## Open before starting

- Nothing. If a dependency-lint ever exists, the three grep lines are its first ruleset; that is a sentence for
  the design record, not work for this step.

## Done when

- [x] `grep -n "Testing" .agents/design/decisions.md` finds the decision with the folder table.
- [x] **By eye.** The three dependency docs draw the shape's graph and name no kernel.
- [x] `grep -rni "testskernel\|tests\? kernel" .agents/docs .agents/design` is empty, except lines that say
      "superseded".
- [x] `test ! -f .agents/plans/measured-results/support-projects.md`
