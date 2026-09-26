# Binacle.Lib.Benchmarks.Algorithms

Times the three packing algorithms, v1 against v2, on scenarios from the shared data. Three tiers; the tier
is the first word of the class name, so it is in the report file name too.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Smoke_<Alg>_<Op>.cs` (six) | Rows v1 and v2; the column is one of four scenarios: full bin, one type / small order / typical container / many item types |
| `Sample_<Alg>_<Op>.cs` (six) | Rows v1 and v2; the column is one of 30 Bischoff problems, named `<category> (<id>)` |
| `Full_<Alg>_<Op>.cs` (six) | Rows v1 and v2; the column is every one of the 700 Bischoff problems |
| `SmokeBase.cs`, `SampleBase.cs`, `FullBase.cs` | Where each tier gets its scenarios; the picks are `SmokeSet`, `SampleSet` and the Bischoff data provider |
| `BenchmarkBase.cs` | Loads the scenario before the run and runs one algorithm on it |
| `Program.cs` | Runs the classes with the config from `shared/test/Binacle.Benchmarking`, and fails when nothing ran |

## 🛠️ How you use it

```
just bench lib-algorithms-smoke            # 48 cases, about 6 minutes: did my change help or hurt
just bench lib-algorithms-sample           # 360 cases at the default job
just bench lib-algorithms-sample quick     # the same at the short job, about 30 minutes
just bench lib-algorithms-full             # 8,400 cases at the short job, about 16 hours
just bench lib-algorithms-full precise     # the same at the default job, about 30 hours
```

Full asks before it starts. The report lands in `BenchmarkDotNet.Artifacts/results/`,
gitignored.

## ⚠️ What will bite you

`short` widens Error and RatioSD; a finding that rests on a ratio under 1.1 needs the default job. `Mean` is
one machine's number - read `Ratio` and `Allocated`. The full tier is a day of your machine; run it when the
code or the machine changed, not to see.
