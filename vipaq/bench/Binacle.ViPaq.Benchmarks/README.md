# Binacle.ViPaq.Benchmarks

Times ViPaq against protobuf on real packs and on a synthetic curve up to the format's limit. Rows `Protobuf`
(baseline), `ViPaq_Row`, `ViPaq_Columnar` - the production path is protobuf against the row layout; whether
columnar costs time is the one thing the size report cannot say. Every timing class runs the raw path; what
compressing costs is a class of its own.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Encode.cs`, `Decode.cs` | Twelve columns: six real packs by what each covers, then 1,000 / 5,000 / 65,535 items at 8 and 16 bit |
| `CompressionCost.cs` | NoOp against Deflate and Gzip, encode and decode, row-major, on the low and the high end of deflate's win |
| `BenchmarkBase.cs` | Loads the column's scenario before the run |
| `Program.cs` | The BenchmarkDotNet switcher with the config from `shared/test/Binacle.Benchmarking` |

The picks are in `vipaq/test/Binacle.ViPaq.Testing/Providers/`, keyed by the column name the report prints.

## 🛠️ How you use it

```
just bench vipaq              # 84 cases at the default job, about 20 minutes
just bench vipaq short        # the same at the short job, about 7 minutes
```

The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

The parity findings rest on ratios like 0.89 and 1.20, which the `short` job blurs - keep a default-job run.
The synthetic columns are for time and memory only: random data has nothing for a codec to grip, so never read
a size or a compression number off them. `Mean` is one machine's number; read `Ratio` and `Allocated`.
