# lib/bench

The timings for `Binacle.Lib`: one BenchmarkDotNet project per question. Not tests - nothing here passes or
fails - and not measurements either: a timing is one machine's number, so nothing here is tracked. A run
worth keeping is copied into [`lib/results/benchmarks/`](../results/benchmarks) by hand: a class's first kept run
into `baseline/`, a later one into a dated folder.

## 📂 What is in it

| Project | The question |
|---|---|
| `Binacle.Lib.Benchmarks.Algorithms` | Is v2 of each packing algorithm still faster than v1, and on which problems? Three tiers - smoke, sample, full. See [its README](Binacle.Lib.Benchmarks.Algorithms/README.md). |
| `Binacle.Lib.Benchmarks.ParallelAlgorithms` | When `Best` races several algorithms on one bin, from what size is parallel faster than one after the other, and on how many cores? One class, on identical items, pinned to every core count. See [its README](Binacle.Lib.Benchmarks.ParallelAlgorithms/README.md). |
| `Binacle.Lib.Benchmarks.ParallelOverhead` | What does parallelising cost? No packing at all - the algorithms do not pack - so the rows measure what the processor costs around them. A class per race width, one for bins, each over a working set of 0 or 64 KB, pinned to every core count. See [its README](Binacle.Lib.Benchmarks.ParallelOverhead/README.md). |
| `Binacle.Lib.Benchmarks.ParallelBins` | When a request holds many bins, is packing them in parallel faster than one after the other? One class per algorithm, on identical bins and items, pinned to every core count. See [its README](Binacle.Lib.Benchmarks.ParallelBins/README.md). |
| `Binacle.Lib.Benchmarks.ResultSelection` | How does v2 of each result selector compare with v1, in time and allocations? See [its README](Binacle.Lib.Benchmarks.ResultSelection/README.md). |
| `Binacle.Lib.Benchmarks.Scaling` | How does packing time grow as the item count grows? One tier. See [its README](Binacle.Lib.Benchmarks.Scaling/README.md). |

## 🛠️ How you use it

```
just bench                          # every recipe with its cost, in tier order
just bench lib-algorithms-smoke     # one recipe; a project with tiers has one per tier
just bench lib-result-selection     # result selection and scaling have one tier and a plain name
just bench lib-scaling              # the growth curve
just bench lib-parallel-overhead    # what the parallel machinery costs, with no packing in it
```

Reports land in the project's `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

Every project takes its BenchmarkDotNet config from `shared/test/Binacle.Benchmarking`; there is no config
here to edit, and `BenchmarkConfig` pins the reports three folders up from the binary, which is the project's
own folder only when it was built there. A `Mean` does not compare across machines or across files - `Ratio` and `Allocated` do.
