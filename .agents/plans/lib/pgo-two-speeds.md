---
description: Some bench processes run slow from start to end because of tiered PGO, and one process per case hides it; a code fix was tried and did not hold
state: idea
waits-on: "nobody - it is an idea"
horizon: undecided
paths:
  - "lib/src/Binacle.Lib/Algorithms/**"
  - "lib/bench/**"
  - "vipaq/bench/**"
---

# Slow processes under tiered PGO

BenchmarkDotNet runs each case in one process. Now and then a whole process runs slow, with a tight StdDev, and
the report cannot show it. Every kept BFD time may be one. Taken out of the measured-results plan on 2026-09-28:
too much to chase now.

## What is known - measured 2026-09-27 and 2026-09-28

- **The cause is tiered PGO.** With `DOTNET_TieredPGO=0` every process runs at one speed. PGO off is no fix:
  every time is about twice as slow and allocates more.
- **BFD v2** on the racing bench: about 1 process in 20 slow, about 1.7× slower. The three-algorithm `Loop`
  runs slow on every case, and PGO off removes that too.
- **The sort is not the whole cause.** The test-only BFD v3 has no sort in its loop and still splits: about 2
  processes in 5 run 1.2× to 1.4× slower.
- **ViPaq** once moved with no code change; 20 processes with PGO on did not repeat it.

The numbers and the reports are in the lib findings record, and in the gitignored `BenchmarkDotNet.Artifacts/`
folders `pgo-*`, `v3-proof` and `v3-spread*` of the racing, algorithms and ViPaq benches.

## Two ways, not exclusive

- **Find the code PGO tunes badly.** Compare the JIT output of a fast and a slow process
  (`DOTNET_JitStdOutFile`, `DOTNET_JitDisasmSummary`). Left in a v3 process that a guess could get wrong: the
  item `Array.Sort` with a lambda, `List.Remove` on a class, the LINQ `Where` over the items.
- **Make the benches show it.** Run each case in several processes (`--launchCount`), so a slow process shows
  as disagreement. Every run takes that many times longer.

It matters most where a small difference decides something: a racing or bins drop point, a cost function.

## Done when

- [ ] The code PGO tunes badly is found and fixed, or the bench recipes run each case in several processes.
      **By eye.** A run of one BFD case in many processes splits no more than 1.3×, or the recipes pass
      `--launchCount`.
