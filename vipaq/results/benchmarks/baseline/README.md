# Baseline - ViPaq benchmarks

The first kept run of every ViPaq bench class. A later run that moved goes in a dated folder beside this one,
holding only the reports that moved.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Copied 2026-09-23, from runs made that day on one machine:

- AMD Ryzen 9 9900X, 12 cores, Ubuntu 26.04.1
- .NET 10.0.12 (SDK 10.0.112), BenchmarkDotNet 0.15.8

Time only compares with a run from the same machine and .NET version. Ratio and Allocated compare anywhere.

## 📂 What was copied

| File | Recipe | Job | Cases |
|---|---|---|---|
| `encoding/Smoke_Encode.md` | `just bench vipaq-smoke` | short | 9 |
| `encoding/Smoke_Decode.md` | `just bench vipaq-smoke` | short | 9 |
| `encoding/Sample_Decode.md` | `just bench vipaq-sample` | default | 36 |
| `encoding/Sample_CompressionCost_Encode.md` | `just bench vipaq-sample` | default | 6 |
| `encoding/Sample_CompressionCost_Decode.md` | `just bench vipaq-sample` | default | 6 |

Not here yet: `encoding/Sample_Encode.md`, from the same `vipaq-sample` run, still running at the copy.
