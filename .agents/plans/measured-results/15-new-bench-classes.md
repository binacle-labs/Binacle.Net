---
description: Step 15 - two new bench classes - lib's time against item count, and a Json row beside protobuf in ViPaq's encode - and their first runs kept in the baseline
state: ready
waits-on: "the maintainer runs lib-scaling once"
horizon: next-release
paths: ["lib/bench/Binacle.Lib.Benchmarks.Scaling/**", "vipaq/bench/Binacle.ViPaq.Benchmarks/**", "lib/test/Binacle.Lib.Testing/**", "lib/results/benchmarks/**", "vipaq/results/benchmarks/**"]
---

# Step 15 - the new bench classes

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## The step

- **The scaling class.** Done 2026-09-23, in its own project `lib/bench/Binacle.Lib.Benchmarks.Scaling`
  rather than in Algorithms - the maintainer's call: one project per question, and its own recipe means the
  curve runs without the sample tier behind it. One class `Sample_Packing`, no smoke tier: a curve read from
  three points is not worth having. Six rows over the 11-step ladder, 66 cases. Recipe `just bench lib-scaling`.
- **`Json` joins protobuf** as a row in `Binacle.ViPaq.Benchmarks`' encode classes - encode only, because the
  test `JsonEncoder` has no decode. Done 2026-09-23, in `EncodeBase`, so both smoke and sample carry it.
- The maintainer runs each once; the session copies the reports into `baseline/`, since a new class's first
  kept run goes there. ViPaq's two ran 2026-09-24 and are copied. **Still to run:** `just bench lib-scaling`.
  The scaling recipe's comment says "timed on its first run" - replace that with the measured time.
- The docs that list the bench classes. Done 2026-09-23: the lib tests doc, both bench READMEs, the new
  project README, the vipaq design decision on one synthetic curve, and the benchmarks README's family list.
  The ViPaq README's bench row names no rows, so nothing to change there.

## Done when

- [x] `grep -rln "LadderGenerator" lib/bench/Binacle.Lib.Benchmarks.Scaling --include=*.cs`
      names a class with an item-count `[Params]`. Done 2026-09-23.
- [x] `grep -rn "JsonEncoder" vipaq/bench/Binacle.ViPaq.Benchmarks --include=*.cs` hits. Done 2026-09-23.
- [ ] `ls lib/results/benchmarks/baseline/scaling/` has `Sample_Packing.md`, with an item-count column.
- [x] `vipaq/results/benchmarks/baseline/encoding/` has an encode report with a `Json` row.
      Done 2026-09-24: both smoke and sample reruns copied in, 12 and 48 cases.
- [ ] Both projects build; the runs above finished with no failed case.
      `dotnet build -c Release` on both passed 2026-09-23; the runs have not happened.
