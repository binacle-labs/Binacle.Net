# Baseline - lib benchmarks

Every lib bench class, at the shape the code has now. The maintainer, 2026-10-03: "in baseline we do no
skip bnaseline must carryuy all and updated to the latest shapoe". So when a shape moves there are two acts and
no third - a class or shape that is gone has its report **removed**, and a class whose shape moved is **re-run
and its report replaced**. There is no dated folder.

Each file is a BenchmarkDotNet report copied unchanged from the project's `BenchmarkDotNet.Artifacts/results/`,
renamed to its class.

## 🖥️ Where it ran

Copied 2026-09-23 to 2026-10-02, from runs made on those days on one machine:

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
| `parallel-algorithms/Identical_FFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short | 72 |
| `parallel-algorithms/Identical_WFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short | 72 |
| `parallel-algorithms/Identical_BFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short | 72 |
| `parallel-algorithms/Identical_FFD_BFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short | 72 |
| `parallel-algorithms/Identical_FFD_WFD_BFD_Packing.md` | `just bench lib-parallel-algorithms-identical` | short | 72 |
| `parallel-overhead/Fake_TwoAlgorithms.md` | `just bench lib-parallel-overhead` | default | 30 |
| `parallel-overhead/Fake_ThreeAlgorithms.md` | `just bench lib-parallel-overhead` | default | 30 |
| `parallel-overhead/Fake_Bins.md` | `just bench lib-parallel-overhead` | default | 90 |
| `scaling/Sample_Packing.md` | `just bench lib-scaling` | default | 66 |
| `result-selection/BestAlgorithm.md` | `just bench lib-result-selection` | short | 6 |
| `result-selection/BestBin.md` | `just bench lib-result-selection` | short | 8 |
| `result-selection/SmallestBin.md` | `just bench lib-result-selection` | short | 8 |

Only classes that exist today have a report here. A class with no row has no kept run yet.

Algorithms full ran the short job (8,400 cases, about 16 hours). A short-job time is a rougher number than a
default-job one; the ratios still hold.

## ⚠️ ParallelBins has no kept run, and the WFD rows of the racing run are dirty

Both packing projects were reshaped on 2026-10-03, into five and six classes, and every report they had was
removed under the rule above. `parallel-algorithms/` was refilled the same day; **`parallel-bins/` is still
empty, so nothing here measures Loop against Parallel over many bins.**

Two things to know before reading the racing reports:

- **The alone classes are not a decomposition of the race.** `Loop` on a race class runs 1.10 to 1.21 times the
  sum of the alone classes, while the allocation is additive to the kilobyte - same work, more time. An alone
  class's process only ever sees one algorithm, so its call sites get devirtualized where the race's cannot.
  Read the sum as a **relative** check: about 1.2 is normal, and a cell at 1.5 or more is a slow process.
- **`Identical_FFD_WFD_BFD_Packing` has five such cells**, all on the WFD path - 2 cores at 64 and 128 pieces,
  4 cores at 128, 8 cores at 128, 12 cores at 256. Its best ratio reads 0.60 where the slowest algorithm puts
  the floor near 0.77, so that figure is an inflated `Loop`, not a win. **The two-algorithm race is sound; the
  three-algorithm one needs more launches before its drop point is worth quoting.**

The three `parallel-overhead/` reports are current: that project kept its shape, and it is the only kept
measurement of what parallelising costs.

