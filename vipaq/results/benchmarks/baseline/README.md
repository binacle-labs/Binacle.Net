# Baseline - ViPaq benchmarks

The first kept run of every ViPaq bench class. A later run that moved goes in a dated folder beside this one,
holding only the reports that moved.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Every report was replaced 2026-09-26 by a rerun that added the `Items` and `Widths` columns; the ViPaq code
did not change since the run before. All on one machine:

- AMD Ryzen 9 9900X, 12 cores, Ubuntu 26.04.1
- .NET 10.0.12 (SDK 10.0.112), BenchmarkDotNet 0.15.8

Time only compares with a run from the same machine and .NET version. Ratio and Allocated compare anywhere.

## 📂 What was copied

| File | Recipe | Job | Cases |
|---|---|---|---|
| `encoding/Smoke_Encode.md` | `just bench vipaq-smoke` | short | 12 |
| `encoding/Smoke_Decode.md` | `just bench vipaq-smoke` | short | 9 |
| `encoding/Sample_Encode.md` | `just bench vipaq-sample` | default | 48 |
| `encoding/Sample_Decode.md` | `just bench vipaq-sample` | default | 36 |
| `encoding/Sample_CompressionCost_Encode.md` | `just bench vipaq-sample` | default | 6 |
| `encoding/Sample_CompressionCost_Decode.md` | `just bench vipaq-sample` | default | 6 |

Every ViPaq bench class is here.

The encode classes carry a `Json` row, which is why they have more cases than their decode twins - the test
`JsonEncoder` has no decode.

## ⚠️ What will bite you

**One process can run slow, and a report cannot show it.** Each case runs in one process. With no code change,
`ViPaq_Row` on `5000 items, 8-bit` took 279 μs in the 2026-09-26 sample run against 183 μs in the one before it,
each with a tight StdDev, while protobuf stayed at 230 μs: 1.21× protobuf instead of 0.80×. `ViPaq_Columnar` on
`65535 items, 8-bit` went the other way, 0.77× to 0.64×. The protobuf baseline moved too: decode on `100 cubes, 8-bit`
went from 2,361 to 2,550 ns. Read a surprising row as possibly one slow process.
