# lib/bench

The timings for `Binacle.Lib`: one BenchmarkDotNet project per question. Not tests - nothing here passes or
fails - and not measurements either: a timing is one machine's number, so nothing here is tracked. A run
worth keeping is copied into [`lib/results/benchmarks/`](../results/benchmarks) by hand, dated.

## 📂 What is in it

| Project | The question |
|---|---|
| `Binacle.Lib.Benchmarks.Algorithms` | Is v2 of each packing algorithm still faster than v1, and on which problems? Three tiers - smoke, sample, full. See [its README](Binacle.Lib.Benchmarks.Algorithms/README.md). |
| `Binacle.Lib.Benchmarks.Racing` | When `Best` races several algorithms on one bin, is parallel faster than one after the other? See [its README](Binacle.Lib.Benchmarks.Racing/README.md). |
| `Binacle.Lib.Benchmarks.Threshold` | Should the parallel processors be wired up, and from what size? Two families, three tiers. See [its README](Binacle.Lib.Benchmarks.Threshold/README.md). |
| `Binacle.Lib.Benchmarks.ResultSelection` | How does v2 of each result selector compare with v1, in time and allocations? See [its README](Binacle.Lib.Benchmarks.ResultSelection/README.md). |

## 🛠️ How you use it

```
just bench                          # every recipe with its cost
just bench lib-algorithms-smoke     # one recipe; a project with tiers has one per tier
just bench lib-result-selection     # a project with one tier has its plain name
```

Reports land in the project's `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

Every project takes its BenchmarkDotNet config from `shared/test/Binacle.Benchmarking`; there is no config
here to edit, and `BenchmarkConfig` pins the reports three folders up from the binary, which is the project's
own folder only when it was built there. A `Mean` does not compare across machines or across files - `Ratio` and `Allocated` do.
