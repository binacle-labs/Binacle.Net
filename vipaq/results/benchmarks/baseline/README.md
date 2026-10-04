# Baseline - ViPaq benchmarks

Every ViPaq bench class, at the shape the code has now.

`baseline/` is the only folder here while the benchmarking is still going on. Dated folders start once it
is finished.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Every report was replaced 2026-09-26 by a rerun that added the `Items` and `Widths` columns; the ViPaq code
did not change since the run before. All on one machine:

- AMD Ryzen 9 9900X, 12 cores, Ubuntu 26.04.1
- .NET 10.0.12 (SDK 10.0.112), BenchmarkDotNet 0.15.8

Time only compares with a run from the same machine and .NET version. Ratio and Allocated compare anywhere.

## 📂 What was copied

| File | Recipe | Job |
|---|---|---|
| `encoding/Smoke_Encode.md` | `just bench vipaq-smoke` | default |
| `encoding/Smoke_Decode.md` | `just bench vipaq-smoke` | default |
| `encoding/Sample_Encode.md` | `just bench vipaq-sample` | default |
| `encoding/Sample_Decode.md` | `just bench vipaq-sample` | default |
| `encoding/Sample_CompressionCost_Encode.md` | `just bench vipaq-sample` | default |
| `encoding/Sample_CompressionCost_Decode.md` | `just bench vipaq-sample` | default |

The encode classes carry a `Json` row that the decode twins do not - the test `JsonEncoder` has no decode.

## ⚠️ What will bite you

**One process can run slow, and a report cannot show it.** Each case runs in one process. With no code change,
`ViPaq_Row` on `5000 items, 8-bit` took 279 μs in the 2026-09-26 sample run against 183 μs in the one before it,
each with a tight StdDev, while protobuf stayed at 230 μs: 1.21× protobuf instead of 0.80×. `ViPaq_Columnar` on
`65535 items, 8-bit` went the other way, 0.77× to 0.64×. The protobuf baseline moved too: decode on `100 cubes, 8-bit`
went from 2,361 to 2,550 ns. Read a surprising row as possibly one slow process.
