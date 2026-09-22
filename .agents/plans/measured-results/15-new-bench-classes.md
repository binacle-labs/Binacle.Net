---
description: Step 15 - two new bench classes - lib's time against item count, and a Json row beside protobuf in ViPaq's encode - and their first runs kept in the baseline
state: ready
waits-on: "the scaling class's name; the kept-run shape in place"
horizon: next-release
paths: ["lib/bench/Binacle.Lib.Benchmarks.Algorithms/**", "vipaq/bench/Binacle.ViPaq.Benchmarks/**", "lib/test/Binacle.Lib.Testing/Providers/**", "lib/results/benchmarks/**", "vipaq/results/benchmarks/**"]
---

# Step 15 - the new bench classes

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## The step

- **The scaling class** in `Binacle.Lib.Benchmarks.Algorithms`: six versions over the 11-step item ladder
  already in `SpecializedScalingProblemsProvider` - the time-against-item-count curve no class gives today.
  66 cases.
- **`Json` joins protobuf** as a row in `Binacle.ViPaq.Benchmarks`' encode classes - encode only, because the
  test `JsonEncoder` has no decode.
- The maintainer runs each once; the session copies the reports into `baseline/`, since a new class's first
  kept run goes there.
- The docs that list the bench classes: the lib tests doc, the ViPaq README's bench row, both bench READMEs,
  and the vipaq design decision on one synthetic curve (it says there is no Json row yet).

## Open before starting

- **The scaling class's tier and name.** A class name starts with its tier (`Smoke_`, `Sample_`, `Full_`),
  so the recipe that runs it follows from the name.
- **Which ViPaq tiers get the Json row** - smoke, sample, or both.

## Done when

- [ ] `grep -rln "SpecializedScalingProblemsProvider" lib/bench/Binacle.Lib.Benchmarks.Algorithms --include=*.cs`
      names a class with an item-count `[Params]`.
- [ ] `grep -rn "JsonEncoder" vipaq/bench/Binacle.ViPaq.Benchmarks --include=*.cs` hits.
- [ ] `ls lib/results/benchmarks/baseline/algorithms/` has the scaling class's report, with an item-count
      column; `vipaq/results/benchmarks/` has a report with a `Json` row.
- [ ] Both projects build; the runs above finished with no failed case.
