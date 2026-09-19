---
description: Step 14 - the scaling class and the JSON timing are written, then the first keepers - lib-fast, vipaq-encoding, the bin threshold once on a quiet machine - and its finding
state: ready
waits-on: "step 13's gate, and a quiet machine for the threshold run"
horizon: next-release
paths: ["lib/bench/**", "vipaq/bench/**", "lib/results/benchmarks/**", "vipaq/results/benchmarks/**", ".agents/design/lib/findings.md"]
---

# Step 14 - the new classes and the first keepers

Shape: [results.md](results.md), "Benchmark projects" and "Two kinds, two rules". Protocol: the orchestrator.

## The step

- The scaling class in `Binacle.Lib.Benchmarks.Algorithms`: six versions over the 11-step item ladder
  already in `SpecializedScalingProblemsProvider`, the time-against-item-count curve no family gives today,
  and the family the 20 old `Multiple*` reports sit under since step 13.
- JSON joins protobuf as a baseline row in `Binacle.ViPaq.Benchmarks.Encoding`, using step 10's encoder.
- `just bench lib-fast` and `just bench vipaq-encoding` - the maintainer's runs. The session copies each
  report into its family folder as `<date>.md`, adds the trace row and the "what they say" line.
- `just bench lib-threshold` once, on a quiet machine, hours. The bin-parallelization question - does
  parallel bin processing pay, and from how many bins - gets its finding in the lib findings record with
  numbers, replacing "no finding yet". The algorithm-threshold family stays as the evidence for why parallel
  racing was not wired up.
- Keep a run when the ruler or the code changed, not because it ran. These are the first, so they are
  keepers by definition.
- Two commits: the two classes; then the keepers and the finding.

## Open before starting

- Which machine is the ruler for the first keepers. Every BDN header records it, so the file does not; but
  the trace table's ruler column should say the same thing the same way for every row.
- The scaling class's name under the `<Family>_<Operation>_<Variant>` rule.

## Done when

- [ ] `grep -rln "SpecializedScalingProblemsProvider" lib/bench/Binacle.Lib.Benchmarks.Algorithms --include=*.cs`
      names a class with an item-count `[Params]`.
- [ ] `grep -rn "JsonEncoder" vipaq/bench/Binacle.ViPaq.Benchmarks.Encoding --include=*.cs` hits.
- [ ] `ls lib/results/benchmarks/algorithms/ lib/results/benchmarks/result-selection/ vipaq/results/benchmarks/encoding/`
      each has a `.md` dated after step 13's conversion.
- [ ] `ls lib/results/benchmarks/algorithms/` has a file whose table has an item-count parameter column.
- [ ] `ls lib/results/benchmarks/threshold/` has a dated file for the bin family.
- [ ] `grep -n "BinParallelizationThreshold" .agents/design/lib/findings.md` is a section with numbers, not
      "no finding yet".
- [ ] Every keeper has a trace row: **by eye**, count the files and the rows.
- [ ] The results shape's `Done when` list is all ticked. Then the maintainer deletes this folder and the
      orchestrator.
