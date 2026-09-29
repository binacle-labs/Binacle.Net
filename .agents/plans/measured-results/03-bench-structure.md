---
description: Session 3 - find the structure of the racing and bins benches - the project names, and which class lives where - before sessions 4 and 5 use them
state: ready
waits-on: "the maintainer says when - state set by an agent 2026-09-29, strike it if wrong"
horizon: undecided
paths: ["lib/bench/Binacle.Lib.Benchmarks.Racing/**", "lib/bench/Binacle.Lib.Benchmarks.Threshold/**", "tooling/bench.just", "lib/results/benchmarks/**"]
---

# 3 - The structure of the racing and bins benches

## Goal

**(the maintainer, 2026-09-29):** "step 3 is finding the structure for both 4 and 5". The names are bad - "its
a bad name both" - and the structure may change: "dont care if we have to change structure".

Session 4 measures the algorithm parallelisation threshold, session 5 the bins one. The structure serves both.

## Open - the names and where each class lives

Racing asks when racing algorithms in parallel on one bin pays; Threshold asks when packing bins in parallel pays. An agent read both projects on 2026-09-29.

**Suggested (agent, 2026-09-29)**, four options, the first its pick:

| # | Names | For | Against |
|---|---|---|---|
| 1 | `ParallelAlgorithms` (or `ParallelRacing`) and `ParallelBins` | says what runs in parallel; one word from project to recipe to results folder to results file | `ParallelAlgorithms` beside `Algorithms` can read as "algorithms that are parallel" |
| 2 | `AlgorithmProcessing` and `BinProcessing` | the folder names in `lib/src/Binacle.Lib/`, as `Algorithms` and `ResultSelection` do | names what is measured, not the question |
| 3 | one project, `Parallel`, two families | cheapest; the core pinning stays in one place | against the split the maintainer wants; "Parallel" alone is vague |
| 4 | `Pinned` and `Ladder` - by how it is measured | one setup per project | splits each question over two projects; `Ladder` clashes with `Scaling` |

What the agent found:

- The ladder `*_Algorithms_Packing` classes ask racing's question but sit in Threshold. They are kept because
  they cover jobs smaller than racing's lightest problem.
- `Cores_` is used like a tier, but it is a setting. If the bins bench is pinned too, it could mean "pinned" in
  both projects.
- The bins classes bench `LoopBinProcessor` against `ParallelBinProcessor`. The multi-bin `Best` routes run
  `LoopMultiAlgorithmBinProcessor`, and its parallel twin is benched nowhere.
- A rename touches the project folders and namespaces, `Binacle.Net.slnx`, the `InternalsVisibleTo` lines in
  `lib/src/Binacle.Lib/Binacle.Lib.csproj` (miss them and the benches stop compiling), `tooling/bench.just`,
  the kept reports under `lib/results/benchmarks/baseline/threshold/`, and the lib docs.
- Renaming `Cores_Packing` and `lib-racing-cores` reopens the maintainer's 2026-09-25 choice.
- Whether the new bins class replaces the old bins classes shapes what the bins project holds.
- Do not rename the racing folder while a run is going: its gitignored `BenchmarkDotNet.Artifacts/` does not
  move with it.

## Done when

- [ ] The maintainer picked the structure: the names, and which class lives where.
      **By eye.** His words, quoted and dated, in this file.
- [ ] It is built: projects, recipes, kept reports and lib docs carry the picked names.
      `ls lib/bench/` shows them, and `just bench` lists the recipes under them.
