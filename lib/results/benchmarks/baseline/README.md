# Baseline - lib benchmarks

The first kept run of every lib bench class. A later run that moved goes in a dated folder beside this one,
holding only the reports that moved.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Copied 2026-09-23 to 2026-09-30, from runs made on those days on one machine:

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
| `algorithms/Sample_FFD_Packing.md` | `just bench lib-algorithms-sample` | default | 60 |
| `algorithms/Sample_FFD_Fitting.md` | `just bench lib-algorithms-sample` | default | 60 |
| `algorithms/Sample_WFD_Packing.md` | `just bench lib-algorithms-sample` | default | 60 |
| `algorithms/Sample_WFD_Fitting.md` | `just bench lib-algorithms-sample` | default | 60 |
| `algorithms/Sample_BFD_Packing.md` | `just bench lib-algorithms-sample` | default | 60 |
| `algorithms/Sample_BFD_Fitting.md` | `just bench lib-algorithms-sample` | default | 60 |
| `algorithms/Full_FFD_Packing.md` | `just bench lib-algorithms-full` | short | 1,400 |
| `algorithms/Full_FFD_Fitting.md` | `just bench lib-algorithms-full` | short | 1,400 |
| `algorithms/Full_WFD_Packing.md` | `just bench lib-algorithms-full` | short | 1,400 |
| `algorithms/Full_WFD_Fitting.md` | `just bench lib-algorithms-full` | short | 1,400 |
| `algorithms/Full_BFD_Packing.md` | `just bench lib-algorithms-full` | short | 1,400 |
| `algorithms/Full_BFD_Fitting.md` | `just bench lib-algorithms-full` | short | 1,400 |
| `parallel-algorithms/Smoke_Packing.md` | `just bench lib-parallel-algorithms-smoke` | short | 64 |
| `parallel-algorithms/Sample_Packing.md` | `just bench lib-parallel-algorithms-sample` | default | 88 |
| `scaling/Sample_Packing.md` | `just bench lib-scaling` | default | 66 |
| `result-selection/BestAlgorithm.md` | `just bench lib-result-selection` | short | 6 |
| `result-selection/BestBin.md` | `just bench lib-result-selection` | short | 8 |
| `result-selection/SmallestBin.md` | `just bench lib-result-selection` | short | 8 |

Only classes that exist today have a report here. A class with no row has no kept run yet.

Algorithms full ran the short job (8,400 cases, about 16 hours). A short-job time is a rougher number than a
default-job one; the ratios still hold.
