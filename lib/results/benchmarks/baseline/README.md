# Baseline - lib benchmarks

Every lib bench class, at the shape the code has now. The baseline skips nothing and carries every class at
its latest shape, so when a shape moves there are two acts and no third: a class or shape that is gone has its
report **removed**, and a class whose shape moved is **re-run and its report replaced**.

`baseline/` is the only folder here while the benchmarking is still going on. Dated folders start once it
is finished.

## 📂 What is in it

One row per recipe - the reports it writes, and the job they ran at. Every report is named for its class.

| Folder | Reports | Recipe | Job |
|---|---|---|---|
| `algorithms/` | `Smoke_*` | `just bench lib-algorithms-smoke` | short |
| `algorithms/` | `Sample_*` | `just bench lib-algorithms-sample` | default |
| `algorithms/` | `Full_*` | `just bench lib-algorithms-full` | short |
| `parallel-algorithms/` | `Identical_*` | `just bench lib-parallel-algorithms-identical` | short, 3 launches |
| `parallel-bins/` | `Identical_<alg>_*` | `just bench lib-parallel-bins-identical <alg>` | short |
| `parallel-overhead/` | `Fake_*` | `just bench lib-parallel-overhead` | default |
| `result-selection/` | every selector | `just bench lib-result-selection` | default |
| `scaling/` | `Sample_Packing` | `just bench lib-scaling` | default |

The job is the one the report's own header states, and it is what makes two files comparable.

## 🖥️ Where it ran

Runs made between 2026-09-23 and 2026-10-05, all on one machine:

- AMD Ryzen 9 9900X, 12 cores, Ubuntu 26.04.1
- .NET 10.0.12 (SDK 10.0.112), BenchmarkDotNet 0.15.8

Time only compares with a run from the same machine and .NET version. Ratio and Allocated compare anywhere.

## ⚠️ What will bite you

The `algorithms/Full_*` set ran the short job, about 16 hours. A short-job time is a rougher number than a
default-job one; the ratios still hold.
