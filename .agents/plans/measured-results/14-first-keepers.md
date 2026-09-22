---
description: Step 14 - the scaling class and the JSON timing are written, then the first keepers - lib-algorithms-smoke, vipaq, the bin threshold once on a quiet machine - their benchmarks README, and the bin-threshold finding
state: ready
waits-on: "step 12's tooling rework, and a quiet machine for the threshold run"
horizon: next-release
paths: ["lib/bench/**", "vipaq/bench/**", "lib/results/benchmarks/**", "vipaq/results/benchmarks/**", ".agents/design/lib/findings.md", ".agents/docs/**"]
---

# Step 14 - the new classes and the first keepers

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## The step

- The scaling class in `Binacle.Lib.Benchmarks.Algorithms`: six versions over the 11-step item ladder
  already in `SpecializedScalingProblemsProvider` - the time-against-item-count curve no family gives today.
- `Json` joins protobuf as a row in `Binacle.ViPaq.Benchmarks` `Encode` (encode only - the encoder has no
  decode), using `JsonEncoder` from `Binacle.ViPaq.Testing`.
- `just bench lib-algorithms-smoke` and `just bench vipaq` - the maintainer's runs. The session copies each
  report into its family folder as `<date>.md`.
- `just bench lib-threshold` once, on a quiet machine, hours. The bin-parallelization question - does
  parallel bin processing pay, and from how many bins - gets its finding in the lib findings record with
  numbers, replacing "no finding yet". The algorithm-threshold family stays as the evidence for why parallel
  racing was not wired up.
- `lib/results/benchmarks/README.md` and `vipaq/results/benchmarks/README.md`: the rule (Mean does not compare
  across files; Ratio and Allocated do, and for Loop vs Parallel only on the same core count) and the trace -
  one row per keeper: date, ruler, family, key ratio with its RatioSD, allocated. What the keepers say, as a
  sentence for a person, is the results story, not this step.
- Keep a run when the ruler or the code changed, not because it ran. These are the first, so they are
  keepers by definition.
- Two commits: the two classes; the keepers, their READMEs and the finding.

## Open before starting

- Which machine is the ruler for the first keepers. Every BDN header records it, so the file does not; but
  the trace table's ruler column should say the same thing the same way for every row.
- The scaling class's name under the `<Family>_<Operation>_<Variant>` rule.
- Whether the 2026-07-17 racing reports still sit in a scratch folder on the maintainer's machine. If so,
  `racing/2026-07-17.md` is a keeper too.

## Done when

- [ ] `grep -rln "SpecializedScalingProblemsProvider" lib/bench/Binacle.Lib.Benchmarks.Algorithms --include=*.cs`
      names a class with an item-count `[Params]`.
- [ ] `grep -rn "JsonEncoder" vipaq/bench/Binacle.ViPaq.Benchmarks --include=*.cs` hits.
- [ ] `ls lib/results/benchmarks/algorithms/ vipaq/results/benchmarks/encoding/` each has a dated `.md`.
- [ ] `ls lib/results/benchmarks/algorithms/` has a file whose table has an item-count parameter column.
- [ ] `ls lib/results/benchmarks/threshold/` has a dated file for the bin family.
- [ ] `grep -n "Bins_Packing" .agents/design/lib/findings.md` is a section with numbers, not "no finding yet".
- [ ] Both `benchmarks/README.md` exist, and every keeper has a trace row.
      **By eye**, count the files and the rows.
- [ ] Then the maintainer deletes this folder and the orchestrator.
