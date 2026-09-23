# Baseline - lib benchmarks

The first kept run of every lib bench class. A later run that moved goes in a dated folder beside this one,
holding only the reports that moved.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Copied 2026-09-23 and 2026-09-24, from runs made on those days on one machine:

- AMD Ryzen 9 9900X, 12 cores, Ubuntu 26.04.1
- .NET 10.0.12 (SDK 10.0.112), BenchmarkDotNet 0.15.8

Time only compares with a run from the same machine and .NET version. Ratio and Allocated compare anywhere.

## 📂 What was copied

| File | Recipe | Job | Cases |
|---|---|---|---|
| `algorithms/Smoke_FFD_Packing.md` | `just bench lib-algorithms-smoke` | short | 8 |
| `algorithms/Smoke_FFD_Fitting.md` | `just bench lib-algorithms-smoke` | short | 8 |
| `algorithms/Smoke_WFD_Packing.md` | `just bench lib-algorithms-smoke` | short | 8 |
| `algorithms/Smoke_WFD_Fitting.md` | `just bench lib-algorithms-smoke` | short | 8 |
| `algorithms/Smoke_BFD_Packing.md` | `just bench lib-algorithms-smoke` | short | 8 |
| `algorithms/Smoke_BFD_Fitting.md` | `just bench lib-algorithms-smoke` | short | 8 |
| `racing/Smoke_Packing.md` | `just bench lib-racing-smoke` | short | 8 |
| `racing/Sample_Packing_v1.md` | `just bench lib-racing-sample` | default | 20 |
| `racing/Sample_Packing_v2.md` | `just bench lib-racing-sample` | default | 20 |
| `threshold/Smoke_Algorithms_Packing.md` | `just bench lib-threshold-smoke` | short | 16 |
| `threshold/Smoke_Bins_Packing.md` | `just bench lib-threshold-smoke` | short | 48 |
| `threshold/Sample_Algorithms_Packing.md` | `just bench lib-threshold-sample` | default | 44 |
| `threshold/Sample_Bins_Packing.md` | `just bench lib-threshold-sample` | default | 84 |
| `scaling/Sample_Packing.md` | `just bench lib-scaling` | default | 66 |
| `result-selection/BestAlgorithm.md` | `just bench lib-result-selection` | short | 6 |
| `result-selection/BestBin.md` | `just bench lib-result-selection` | short | 8 |
| `result-selection/SmallestBin.md` | `just bench lib-result-selection` | short | 8 |

Not here yet: algorithms sample and full, and threshold full. Their first kept run joins this folder.
