---
description: Session 3 - restructure the parallel benches, first part - the core pinning moves to shared, Racing and the ladder classes become ParallelAlgorithms, Threshold is retired. ParallelBins is session 5's
state: ready
waits-on: "the maintainer starts it (2026-09-29: 'i wil lspawn the first to restructure common and 4')"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.Racing/**", "lib/bench/Binacle.Lib.Benchmarks.Threshold/**", "lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/**", "shared/test/Binacle.Benchmarking/**", "tooling/bench.just", "lib/results/benchmarks/**"]
---

# 3 - Restructure: shared pinning and ParallelAlgorithms

## Goal

**(the maintainer, 2026-09-29):** "step 3 is finding the structure for both 4 and 5". The names are bad - "its
a bad name both" - and the structure may change: "dont care if we have to change structure".

**(the maintainer, 2026-09-29):** it happens in parts - "the restructure will happen gradfually.. commen then
the 1st to completion then the 5th with discovery". This session builds the common part and
`ParallelAlgorithms`. Session 4 runs it. Session 5 builds `ParallelBins` as it finds its problems.

## Picked

**Decided (the maintainer, 2026-09-29):** two projects, one per question - `ParallelAlgorithms` and
`ParallelBins` - with a full restructure of both. "on 1 yeah i believe a full restructure of both binaries is in
order".

**Decided (the maintainer, 2026-09-29):** asked "pin every class in both projects?", he said "yes common code
to shared". `CoreJobs.cs` and `CorePinning.cs` move to `shared/test/Binacle.Benchmarking`; they use only
BenchmarkDotNet and `System`.

**Decided (the maintainer, 2026-09-29):** drop `Full_Algorithms_Packing_v1` and `_v2` - they ask v1 against v2,
the `Algorithms` project's question. "2 yes".

**Decided (the maintainer, 2026-09-29):** retire the old bins classes - `Smoke_Bins_Packing`,
`Sample_Bins_Packing`, `Full_Bins_Packing_v1`, `Full_Bins_Packing_v2` and `BinsBase`. "3 retire". Their kept
reports stay as the record.

## What this session builds - suggested (agent, 2026-09-29)

Shown to the maintainer 2026-09-29; he answered the open points on it, not each line.

```
shared/test/Binacle.Benchmarking/
  CoreJobs.cs          <- Racing
  CorePinning.cs       <- Racing

lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/
  Program.cs           Racing's, reading --job for the core jobs
  LadderBase.cs        <- Threshold AlgorithmsBase.cs
  Smoke_Packing.cs     <- Threshold Smoke_Algorithms_Packing    ladder 3, 47, 67, 79
  Sample_Packing.cs    <- Threshold Sample_Algorithms_Packing   the whole ladder
  Full_Packing.cs      <- Racing Cores_Packing                  the 30 Bischoff picks, code unchanged
  README.md

lib/bench/Binacle.Lib.Benchmarks.Racing/       gone
lib/bench/Binacle.Lib.Benchmarks.Threshold/    gone

lib/results/benchmarks/baseline/
  parallel-algorithms/   <- threshold/*_Algorithms_Packing*.md, old names kept
  parallel-bins/         <- threshold/*_Bins_Packing*.md, old names kept
  racing/, threshold/    gone
```

- **Every class is pinned** through `CoreJobs`, on the core counts it uses today. The ladder classes were
  unpinned; the ladder covers jobs smaller than the smallest Bischoff pick, so the drop point may sit there.
- **The kept reports keep their old names.** They are unpinned runs, the record - not the baseline of the
  pinned class with a similar name. A pinned class's first kept run is its baseline.
- **Recipes**, the same shape as every other project:

```just
# Loop against Parallel, both races, ladder 3, 47, 67, 79, on 2 to 12 cores: about N minutes
lib-parallel-algorithms-smoke: (lib-parallel-algorithms-run 'short' 'Smoke')

# The whole ladder, on 2 to 12 cores: about N minutes with `quick`
[arg('mode', pattern='|quick')]
lib-parallel-algorithms-sample mode='':
    ...
    just bench lib-parallel-algorithms-run "$job" Sample

# Both races and each algorithm alone, the 30 Bischoff picks, on 2 to 12 cores: about 70 minutes; `precise` about 3.5 hours
[arg('mode', pattern='|precise')]
[confirm('lib-parallel-algorithms-full: about 70 minutes (precise: about 3.5 hours). Run?')]
lib-parallel-algorithms-full mode='':
    ...
    just bench lib-parallel-algorithms-run "$job" Full

[private]
lib-parallel-algorithms-run job tier:
    dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms -- --job {{ job }} --filter '*.{{ tier }}_*'
```

The `about N minutes` are unknown until a pinned run: pinning multiplies the ladder cases by the core counts.
The `bench.just` header loses its `Cores_ for racing` exception.

## What bites the build

- The rename touches the project folders and namespaces, `Binacle.Net.slnx`, the `InternalsVisibleTo` lines in
  `lib/src/Binacle.Lib/Binacle.Lib.csproj` (miss them and the benches stop compiling), `tooling/bench.just`,
  `lib/bench/README.md`, the kept reports, and the lib docs.
- Retiring the bins classes may leave `LadderGenerator.GetBins` in `lib/test/Binacle.Lib.Testing/` unused.
  Check before deleting it; session 5 may want it.
- Do not move the racing folder while a run is going: its gitignored `BenchmarkDotNet.Artifacts/` does not
  move with it.
- The maintainer runs every bench and any build that starts the host. A plain `dotnet build` of the bench
  project is not one.

## Done when

- [ ] The maintainer picked the structure: the names, and which class lives where.
      **By eye.** His words, quoted and dated, in this file.
- [ ] The pinning code lives in shared.
      `test -f shared/test/Binacle.Benchmarking/CoreJobs.cs && test -f shared/test/Binacle.Benchmarking/CorePinning.cs`
- [ ] `ParallelAlgorithms` holds the three classes, each pinned, and builds.
      `ls lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms/{Smoke,Sample,Full}_Packing.cs` and
      `dotnet build -c Release lib/bench/Binacle.Lib.Benchmarks.ParallelAlgorithms`. Pinned: **by eye**, each
      class runs through `CoreJobs`.
- [ ] Racing and Threshold are gone, and nothing names them.
      `test ! -d lib/bench/Binacle.Lib.Benchmarks.Racing && test ! -d lib/bench/Binacle.Lib.Benchmarks.Threshold`
      and `! grep -rn "Benchmarks\.Racing\|Benchmarks\.Threshold\|lib-racing\|lib-threshold\|Cores_Packing" Binacle.Net.slnx lib tooling .agents/docs`
- [ ] The kept reports moved.
      `test ! -d lib/results/benchmarks/baseline/threshold && ls lib/results/benchmarks/baseline/parallel-algorithms lib/results/benchmarks/baseline/parallel-bins`
- [ ] `just bench` lists the three `lib-parallel-algorithms-` recipes.
      `just --list bench | grep -c "lib-parallel-algorithms-"` prints 3.
