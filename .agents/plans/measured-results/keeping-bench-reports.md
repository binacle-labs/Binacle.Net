---
description: How a bench run worth keeping gets from the project's gitignored artifacts folder into <slice>/results/benchmarks/, and in what shape
state: proposed
waits-on: "the maintainer's answer on how, likely after more keepers are made by hand; the shape is picked, the copy waits on the final shape. horizon was set by an agent, strike it"
horizon: undecided
paths: ["tooling/bench.just", "shared/test/Binacle.Benchmarking/**", "lib/results/benchmarks/**", "vipaq/results/benchmarks/**"]
---

# Keeping bench reports

A run writes its reports into the project's own `BenchmarkDotNet.Artifacts/results/`, which git ignores - one
GitHub markdown file per class (smoke Algorithms writes six). A kept run belongs in
`<slice>/results/benchmarks/<family>/`. Today nothing moves it: `tooling/bench.just` says "copied by hand".
Not every run is kept: keep one when the machine or the code changed, not because it ran.

## How it gets there

- **By hand**, as today. Nothing to build.
- **A `keep` recipe**, e.g. `just bench keep lib-algorithms`: copies that project's latest reports, dated.
  Nothing lands in `results/` unless asked.
- **Automatic, in C#.** `BenchmarkProgram.Run` is every bench project's `Main` and already knows whether every
  case passed; after a good run it copies or writes the reports there. One place for all five projects. Every
  run lands, so git becomes the filter: commit the keepers, delete the rest.
- A second line in each `-run` recipe could copy too; it stops before the copy when the run fails. Five
  places instead of one.

Leaning, 2026-09-23: the `keep` recipe, because most runs are smoke checks nobody keeps.

## Shape - picked 2026-09-23

A kept run is a dated folder of the reports exactly as BDN wrote them, each named after its class:
`lib/results/benchmarks/algorithms/2026-09-23/Smoke_FFD_Packing.md`, ... A plain copy, no joining, so the
record is what ran. A second kept run on the same day needs its own folder name.

Nothing is copied until the final shape of `results/benchmarks/` is in place (the maintainer, 2026-09-23).
The 2026-09-23 runs are wanted as the first baseline - every smoke run, `lib-result-selection` and
`vipaq-sample` - and they sit in each project's artifacts folder until then. The next run of the same class
overwrites its report there.
