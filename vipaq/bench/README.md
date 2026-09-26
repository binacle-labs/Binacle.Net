# vipaq/bench

The timings for `Binacle.ViPaq`: one BenchmarkDotNet project. Not tests - nothing here passes or fails - and
not measurements either: a timing is one machine's number, so nothing here is tracked. Size is measured by
[`vipaq/measure`](../measure) and written to [`vipaq/results`](../results); a timing run worth keeping is copied
into [`vipaq/results/benchmarks/`](../results/benchmarks) by hand: a class's first kept run into `baseline/`, a later
one into a dated folder.

## 📂 What is in it

| Project | The question |
|---|---|
| `Binacle.ViPaq.Benchmarks` | Does ViPaq's encode and decode cost more time than protobuf's, in either layout, and what does compressing cost? See [its README](Binacle.ViPaq.Benchmarks/README.md). |

## 🛠️ How you use it

```
just bench vipaq-smoke          # 21 cases, about 3 minutes: did my change help or hurt
just bench vipaq-sample         # 96 cases at the default job, about 30 minutes: the one to keep
just bench vipaq-sample quick   # the same at the short job, about 7 minutes
```

The report lands in the project's `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

The BenchmarkDotNet config comes from `shared/test/Binacle.Benchmarking`; there is no config here to edit.
`Mean` does not compare across machines or across files - `Ratio` and `Allocated` do.
