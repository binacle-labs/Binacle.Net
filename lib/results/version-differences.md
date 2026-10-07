# Version differences

How v2 differs from v1 for each algorithm, in fill, time and memory, when packing and when fitting.

## 📊 Same fill

From [measurements/version-parity.md](measurements/version-parity.md), over every Bischoff problem.

| Algorithm | Versions | Problems with a different fill |
|---|---|---|
| FFD | v1, v2 | 0 |
| WFD | v1, v2, v3 | 0 |
| BFD | v1, v2, v3 | 1 (thpack7_45: v1 79.08, v2 79.73, v3 79.73) |

v3 has no timing runs, so the sections below compare v1 and v2 only.

## 📊 Time, v2 against v1

On each problem, v2's mean time divided by v1's. Below 1 means v2 is faster. Min, mean, median and max are
taken over those per-problem ratios, across every Bischoff problem.

| Algorithm | Operation | Min | Mean | Median | Max | Problems where v2 is slower |
|---|---|---|---|---|---|---|
| FFD | Packing | 0.50 | 0.67 | 0.67 | 1.36 | 1 |
| FFD | Fitting | 0.49 | 0.57 | 0.57 | 0.76 | 0 |
| WFD | Packing | 0.38 | 0.53 | 0.53 | 0.91 | 0 |
| WFD | Fitting | 0.40 | 0.55 | 0.54 | 0.98 | 0 |
| BFD | Packing | 0.26 | 0.46 | 0.46 | 0.87 | 0 |
| BFD | Fitting | 0.30 | 0.44 | 0.44 | 0.65 | 0 |

**Warning: Min, Max and "Problems where v2 is slower" are not safe to quote.** These runs used a short job
(3 iterations, 1 launch). The Sample runs repeat 30 of these problems with the default job. On those 30, the
mean and median ratios agree within 0.02, but a single problem's ratio differs by more than 15% on up to 9
of them. Mean and median hold; one problem's number does not.

Where v2 is slower: FFD packing on thpack7_54, 1.36.

### 📈 Median time ratio per set

| Algorithm | Operation | thpack1 | thpack2 | thpack3 | thpack4 | thpack5 | thpack6 | thpack7 | **All sets** |
|---|---|---|---|---|---|---|---|---|---|
| FFD | Packing | 0.63 | 0.64 | 0.66 | 0.66 | 0.68 | 0.70 | 0.70 | 0.67 |
| FFD | Fitting | 0.58 | 0.56 | 0.56 | 0.56 | 0.57 | 0.58 | 0.59 | 0.57 |
| WFD | Packing | 0.53 | 0.53 | 0.53 | 0.53 | 0.54 | 0.53 | 0.52 | 0.53 |
| WFD | Fitting | 0.53 | 0.52 | 0.54 | 0.55 | 0.55 | 0.54 | 0.56 | 0.54 |
| BFD | Packing | 0.48 | 0.48 | 0.48 | 0.46 | 0.45 | 0.46 | 0.45 | 0.46 |
| BFD | Fitting | 0.46 | 0.44 | 0.43 | 0.44 | 0.43 | 0.44 | 0.45 | 0.44 |

## 📊 Memory, v2 against v1

On each problem, the memory v2 allocates divided by what v1 allocates. Below 1 means v2 allocates less.
Min, mean, median and max are taken over those per-problem ratios, across every Bischoff problem.

| Algorithm | Operation | Min | Mean | Median | Max |
|---|---|---|---|---|---|
| FFD | Packing | 0.30 | 0.38 | 0.39 | 0.48 |
| FFD | Fitting | 0.47 | 0.56 | 0.56 | 0.63 |
| WFD | Packing | 0.03 | 0.05 | 0.05 | 0.16 |
| WFD | Fitting | 0.10 | 0.26 | 0.25 | 0.52 |
| BFD | Packing | 0.04 | 0.08 | 0.08 | 0.17 |
| BFD | Fitting | 0.11 | 0.26 | 0.25 | 0.46 |

## 📂 Where the numbers come from

Time and memory: the `Full_*` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms),
BenchmarkDotNet on an AMD Ryzen 9 9900X, .NET 10.0.12. Each ratio is worked out from the Mean and Allocated
columns, not read from the reports' rounded Ratio column.
