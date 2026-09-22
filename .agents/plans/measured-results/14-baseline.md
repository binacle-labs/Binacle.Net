---
description: Step 14 - the kept bench runs get their shape - a baseline folder holding the first kept run of every class, and a dated folder per later run that moved - their two READMEs, and how a run gets copied in
state: ready
waits-on: "nothing to start; how a run gets copied is open and answered after more runs are kept by hand"
horizon: next-release
paths: ["lib/results/benchmarks/**", "vipaq/results/benchmarks/**", "lib/results/README.md", "vipaq/results/README.md", "tooling/bench.just", "shared/test/Binacle.Benchmarking/**"]
---

# Step 14 - the baseline

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## The shape - the maintainer's, 2026-09-23

```
<slice>/results/benchmarks/
  README.md          how to read the reports; one row per entry
  baseline/          the first kept run of every bench class
    <family>/<Class>.md
  <date>/            a later run, only the reports that moved
    <family>/<Class>.md
```

- A report is copied exactly as BDN wrote it, renamed to its class: `Smoke_FFD_Packing.md`. No joining, no
  editing, so the record is what ran.
- Families: lib `algorithms/`, `racing/`, `threshold/`, `result-selection/`; ViPaq `encoding/`.
- A new class's first kept run joins `baseline/`. A dated folder holds only later runs that are significant,
  so the dates read as increments on the baseline.
- A run writes into the project's own `BenchmarkDotNet.Artifacts/results/`, which git ignores. The next run of
  the same class overwrites its report there, so a run worth keeping is copied before the next one.

## Done 2026-09-23

Every run the maintainer made that day was copied into `baseline/` by hand, because nothing was kept before:
every smoke recipe, `lib-result-selection`, and all of `vipaq-sample`. Each `baseline/README.md` lists what was copied: file, recipe, job, cases, and the machine.

## The step

- `lib/results/benchmarks/README.md` and `vipaq/results/benchmarks/README.md`: how to read the reports (Mean
  does not compare across files; Ratio and Allocated do, and Loop against Parallel only on the same core
  count), the shape above, and one row per entry: date, family, class, key ratio with its RatioSD, allocated.
  What the numbers say, as a sentence for a person, is not this step.
- A row for `benchmarks/` in `lib/results/README.md` and `vipaq/results/README.md`.
- `tooling/bench.just`'s header comment says where a kept run goes.

## Open

- **What counts as significant.** A rule a person can apply, for example "a ratio moved by more than its
  RatioSD, or Allocated changed". Without one, every run looks different.
- **How a run gets copied.** By hand today. The choices, answered after more runs are kept by hand:
  - a `keep` recipe, e.g. `just bench keep lib-algorithms`, that copies a project's latest reports; nothing
    lands unless asked. The leaning, because most runs are smoke checks nobody keeps.
  - automatic, in C#: `BenchmarkProgram.Run` is every bench project's `Main` and knows whether every case
    passed; after a good run it copies the reports. Every run lands, so git becomes the filter.
  - a second line in each `-run` recipe; five places instead of one.

## Known gap, not addressed for now

**The machine.** Time does not compare across machines or runtimes, so a dated folder only means something
against a baseline made on the same machine and .NET. The BDN header in every report names both, but nothing
yet says what happens when the machine changes - a new baseline, or a dated folder that says so. The
maintainer's call of 2026-09-23: noted, left for later.

## Done when

- [x] `ls lib/results/benchmarks/baseline vipaq/results/benchmarks/baseline` lists every family, and
      `ls vipaq/results/benchmarks/baseline/encoding` has `Sample_Encode.md`. Done 2026-09-23.
- [ ] Both `benchmarks/README.md` exist, with a row for every file in `baseline/`, or pointing at the list in `baseline/README.md`. That also brings to life the
      links in `lib/bench/README.md`, `vipaq/bench/README.md` and `tooling/bench.just`, dead until then.
      **By eye**, count the files and the rows.
- [ ] Both results READMEs have a `benchmarks/` row.
