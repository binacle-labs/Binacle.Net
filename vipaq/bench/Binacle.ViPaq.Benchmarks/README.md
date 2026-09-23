# Binacle.ViPaq.Benchmarks

Times ViPaq against protobuf on real packs and on a synthetic curve up to the format's limit. Rows `Protobuf`
(baseline), `ViPaq_Row`, `ViPaq_Columnar`, and `Json` on encode only - the production path is protobuf against
the row layout; whether columnar costs time is the one thing the size report cannot say. Every timing class runs the raw path; what
compressing costs is a class of its own.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Smoke_Encode.cs`, `Smoke_Decode.cs` | Three columns: one item, the typical container, the largest FFD pack |
| `Sample_Encode.cs`, `Sample_Decode.cs` | Twelve columns: six real packs by what each covers, then 1,000 / 5,000 / 65,535 items at 8 and 16 bit |
| `EncodeBase.cs`, `DecodeBase.cs` | The rows both tiers time - four on encode, three on decode; the test `JsonEncoder` has no decode |
| `Sample_CompressionCost_Encode.cs`, `_Decode.cs` | NoOp against Deflate and Gzip, row-major, on the low and the high end of deflate's win; each compares to its own NoOp |
| `CompressionCostBase.cs` | The two packs and the three codecs both classes use |
| `BenchmarkBase.cs` | Loads the column's scenario before the run |
| `Program.cs` | Runs the classes with the config from `shared/test/Binacle.Benchmarking`, and fails when nothing ran |

The picks are in `vipaq/test/Binacle.ViPaq.Testing/`, keyed by the column name the report prints: `TimingSet` joins `BischoffTimingSet`, `CustomProblemsTimingSet` and `SyntheticGenerator`; `CompressionCostSet` stands on its own.

## 🛠️ How you use it

```
just bench vipaq-smoke          # 18 cases, about 3 minutes: did my change help or hurt
just bench vipaq-sample         # 84 cases at the default job, about 20 minutes: the one to keep
just bench vipaq-sample quick   # the same at the short job, about 7 minutes
```

The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## ⚠️ What will bite you

The parity findings rest on ratios like 0.89 and 1.20, which the `short` job blurs - keep a default-job run.
The synthetic columns are for time and memory only: random data has nothing for a codec to grip, so never read
a size or a compression number off them. `Mean` is one machine's number; read `Ratio` and `Allocated`.
