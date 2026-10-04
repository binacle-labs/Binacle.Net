# Baseline - lib benchmarks

Every lib bench class, at the shape the code has now. The baseline skips nothing and carries every class at
its latest shape, so when a shape moves there are two acts and no third: a class or shape that is gone has its
report **removed**, and a class whose shape moved is **re-run and its report replaced**.

`baseline/` is the only folder here while the benchmarking is still going on. Dated folders start once it
is finished.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Copied 2026-09-23 to 2026-10-03, from runs made on those days on one machine:

- AMD Ryzen 9 9900X, 12 cores, Ubuntu 26.04.1
- .NET 10.0.12 (SDK 10.0.112), BenchmarkDotNet 0.15.8

Time only compares with a run from the same machine and .NET version. Ratio and Allocated compare anywhere.

## 📂 What was copied

| File | Recipe | Job |
|---|---|---|
| `algorithms/Smoke_FFD_Packing.md` | `just bench lib-algorithms-smoke` | short |
| `algorithms/Smoke_FFD_Fitting.md` | `just bench lib-algorithms-smoke` | short |
| `algorithms/Smoke_WFD_Packing.md` | `just bench lib-algorithms-smoke` | short |
| `algorithms/Smoke_WFD_Fitting.md` | `just bench lib-algorithms-smoke` | short |
| `algorithms/Smoke_BFD_Packing.md` | `just bench lib-algorithms-smoke` | short |
| `algorithms/Smoke_BFD_Fitting.md` | `just bench lib-algorithms-smoke` | short |
| `algorithms/Sample_FFD_Packing.md` | `just bench lib-algorithms-sample` | default |
| `algorithms/Sample_FFD_Fitting.md` | `just bench lib-algorithms-sample` | default |
| `algorithms/Sample_WFD_Packing.md` | `just bench lib-algorithms-sample` | default |
| `algorithms/Sample_WFD_Fitting.md` | `just bench lib-algorithms-sample` | default |
| `algorithms/Sample_BFD_Packing.md` | `just bench lib-algorithms-sample` | default |
| `algorithms/Sample_BFD_Fitting.md` | `just bench lib-algorithms-sample` | default |
| `algorithms/Full_FFD_Packing.md` | `just bench lib-algorithms-full` | short |
| `algorithms/Full_FFD_Fitting.md` | `just bench lib-algorithms-full` | short |
| `algorithms/Full_WFD_Packing.md` | `just bench lib-algorithms-full` | short |
| `algorithms/Full_WFD_Fitting.md` | `just bench lib-algorithms-full` | short |
| `algorithms/Full_BFD_Packing.md` | `just bench lib-algorithms-full` | short |
| `algorithms/Full_BFD_Fitting.md` | `just bench lib-algorithms-full` | short |
| `parallel-algorithms/Identical_FFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short, 3 launches |
| `parallel-algorithms/Identical_WFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short, 3 launches |
| `parallel-algorithms/Identical_BFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short, 3 launches |
| `parallel-algorithms/Identical_FFD_BFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short, 3 launches |
| `parallel-algorithms/Identical_FFD_WFD_BFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short, 3 launches |
| `parallel-bins/Identical_FFD_Pieces_Packing.md` | `just bench lib-parallel-bins-identical FFD` | short |
| `parallel-bins/Identical_FFD_Lines_Packing.md` | `just bench lib-parallel-bins-identical FFD` | short |
| `parallel-bins/Identical_WFD_Pieces_Packing.md` | `just bench lib-parallel-bins-identical WFD` | short |
| `parallel-bins/Identical_WFD_Lines_Packing.md` | `just bench lib-parallel-bins-identical WFD` | short |
| `parallel-overhead/Fake_TwoAlgorithms.md` | `just bench lib-parallel-overhead` | default |
| `parallel-overhead/Fake_ThreeAlgorithms.md` | `just bench lib-parallel-overhead` | default |
| `parallel-overhead/Fake_Bins.md` | `just bench lib-parallel-overhead` | default |
| `scaling/Sample_Packing.md` | `just bench lib-scaling` | default |
| `result-selection/BestAlgorithm.md` | `just bench lib-result-selection` | default |
| `result-selection/BestBin.md` | `just bench lib-result-selection` | default |
| `result-selection/SmallestBin.md` | `just bench lib-result-selection` | default |

Only classes that exist today have a report here.

## ⚠️ What will bite you

Algorithms full ran the short job, about 16 hours. A short-job time is a rougher number than a default-job
one; the ratios still hold.
