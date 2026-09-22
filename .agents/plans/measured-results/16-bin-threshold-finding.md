---
description: Step 16 - one long threshold run on a quiet machine answers whether packing bins in parallel pays, and from how many bins; the answer becomes a finding in the lib design record
state: ready
waits-on: "a quiet machine for hours, the maintainer's"
horizon: next-release
paths: ["lib/bench/Binacle.Lib.Benchmarks.Threshold/**", "lib/results/benchmarks/**", ".agents/design/lib/findings.md", ".agents/design/lib/decisions.md"]
---

# Step 16 - the bin-threshold finding

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## The step

- `just bench lib-threshold-full precise` once, on a quiet machine: 704 cases, about 3 hours. The maintainer's
  run.
- The session copies the reports into the kept runs: `baseline/threshold/` for a class that has no kept run
  yet, a dated folder otherwise.
- The question - does parallel bin processing pay, and from how many bins - gets its finding in
  `.agents/design/lib/findings.md` with numbers, replacing "no finding yet". The old November 2025 records
  there (about 0.85 at 2 bins, 0.65 at 8) are the prior to check against.
- `.agents/design/lib/decisions.md` O1 says whether the finding changes the decision not to wire the parallel
  processors up. The algorithms half of the run stays as the evidence for why parallel racing was not wired up.

## Open before starting

- Whether the 2026-07-17 racing reports still sit in a scratch folder on the maintainer's machine. If so, they
  are kept too, as the racing family's oldest run.

## Done when

- [ ] `ls lib/results/benchmarks/` has the `Full_Bins_Packing_v1` and `_v2` reports.
- [ ] `grep -n "no finding yet" .agents/design/lib/findings.md` is empty, and the bins section has numbers.
- [ ] O1 in the lib decisions record names the finding.
      **By eye.**
