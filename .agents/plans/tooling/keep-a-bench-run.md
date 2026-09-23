---
description: "A recipe that copies the reports of the last bench run into the kept-runs folder, instead of doing it by hand"
state: idea
waits-on: "nothing. Horizon picked by an agent to make the file legible; strike it if wrong"
horizon: undecided
paths: ["tooling/bench.just", "lib/results/benchmarks/**", "vipaq/results/benchmarks/**"]
---

# A recipe that keeps a bench run

A benchmark run writes into the project's own `BenchmarkDotNet.Artifacts/results/`, which git ignores, and the
next run of the same class overwrites it. A run worth keeping is copied out by hand today: into
`<slice>/results/benchmarks/<date>/<family>/`, renamed to the class, with a line added to that folder's README.
A recipe would do it - `just bench keep lib-scaling` - copying that project's latest reports and leaving
everything else alone, so nothing lands unless it is asked for. Most runs are quick checks nobody keeps, which
is why it has to be asked for rather than automatic. The other shape considered was doing it in C#, since
`BenchmarkProgram.Run` already knows whether every case passed and could copy after a good run; that makes git
the filter instead of the person.
