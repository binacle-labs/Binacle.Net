# Baseline - ViPaq benchmarks

The first kept run of every ViPaq bench class. A later run that moved goes in a dated folder beside this one,
holding only the reports that moved.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Copied 2026-09-24. The encode and decode reports are from reruns made that day, after `Json` joined the
encode classes; the rest are from 2026-09-23. All on one machine:

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

The encode classes gained a `Json` row on 2026-09-24, which is why they carry more cases than their decode
twins - the test `JsonEncoder` has no decode. These runs printed the column `largest real pack`; it was
renamed to `largest FFD pack` after they ran, so the next kept run reads differently in that one cell.
