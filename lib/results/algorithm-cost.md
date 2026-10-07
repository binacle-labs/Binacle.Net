# Algorithm cost

What WFD and BFD cost in time and memory against FFD, and what running more than one costs for the fill it
adds. Version 2, packing, on every Bischoff problem.

## 📊 Time against FFD

On each problem, the algorithm's mean time divided by FFD's. Min, mean, median and max are taken over those
per-problem ratios.

### 📈 WFD ÷ FFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 3.03 | 7.08 | 6.90 | 14.25 |
| thpack2 | 4.05 | 8.73 | 8.38 | 14.94 |
| thpack3 | 5.19 | 9.51 | 9.36 | 17.26 |
| thpack4 | 5.80 | 9.55 | 9.10 | 15.77 |
| thpack5 | 5.77 | 9.42 | 9.06 | 17.33 |
| thpack6 | 5.69 | 9.21 | 9.14 | 15.22 |
| thpack7 | 6.13 | 8.87 | 8.71 | 14.61 |
| **All sets** | 3.03 | 8.91 | 8.65 | 17.33 |

### 📈 BFD ÷ FFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 2.12 | 4.47 | 4.18 | 10.60 |
| thpack2 | 1.85 | 4.23 | 4.19 | 8.17 |
| thpack3 | 1.86 | 4.20 | 4.08 | 9.42 |
| thpack4 | 2.49 | 4.22 | 4.23 | 8.71 |
| thpack5 | 2.22 | 4.06 | 3.91 | 9.66 |
| thpack6 | 2.45 | 4.11 | 3.92 | 8.54 |
| thpack7 | 2.74 | 4.17 | 4.02 | 8.11 |
| **All sets** | 1.85 | 4.21 | 4.06 | 10.60 |

**Warning: Min and Max are not safe to quote.** They are single problems from a short job (3 iterations,
1 launch), and a single problem's time from that job can be off by more than 15%. Mean and median hold.

## 📊 Running more than one

On each problem, the algorithms' mean times added up, divided by BFD's own time. This is a sum of separate
runs, not a measured run of them together; measured, one after the other costs more than the sum, see
[parallel-algorithms.md](parallel-algorithms.md). "Fill gained" is the best fill of the algorithms run, minus
BFD's, on each problem, averaged. Fill comes from
[measurements/packing-efficiency.md](measurements/packing-efficiency.md).

### 📈 BFD and FFD

| Set | Time ÷ BFD, min | Time ÷ BFD, mean | Time ÷ BFD, median | Time ÷ BFD, max | Fill gained over BFD (mean points) | Problems where it fills more than BFD |
|---|---|---|---|---|---|---|
| thpack1 | 1.09 | 1.25 | 1.24 | 1.47 | 0.07 | 6 |
| thpack2 | 1.12 | 1.25 | 1.24 | 1.54 | 0.03 | 3 |
| thpack3 | 1.11 | 1.26 | 1.24 | 1.54 | 0.07 | 5 |
| thpack4 | 1.11 | 1.25 | 1.24 | 1.40 | 0.06 | 6 |
| thpack5 | 1.10 | 1.26 | 1.26 | 1.45 | 0.00 | 0 |
| thpack6 | 1.12 | 1.25 | 1.26 | 1.41 | 0.02 | 2 |
| thpack7 | 1.12 | 1.25 | 1.25 | 1.36 | 0.01 | 1 |
| **All sets** | 1.09 | 1.25 | 1.25 | 1.54 | 0.04 | 23 |

### 📈 BFD and WFD

| Set | Time ÷ BFD, min | Time ÷ BFD, mean | Time ÷ BFD, median | Time ÷ BFD, max | Fill gained over BFD (mean points) | Problems where it fills more than BFD |
|---|---|---|---|---|---|---|
| thpack1 | 1.73 | 2.76 | 2.56 | 6.04 | 0.18 | 9 |
| thpack2 | 1.84 | 3.26 | 3.05 | 9.07 | 0.03 | 4 |
| thpack3 | 1.65 | 3.47 | 3.35 | 6.70 | 0.05 | 3 |
| thpack4 | 1.96 | 3.39 | 3.36 | 5.35 | 0.02 | 1 |
| thpack5 | 1.99 | 3.45 | 3.40 | 6.40 | 0.00 | 0 |
| thpack6 | 1.95 | 3.34 | 3.36 | 6.17 | 0.06 | 1 |
| thpack7 | 1.97 | 3.23 | 3.16 | 6.04 | 0.00 | 0 |
| **All sets** | 1.65 | 3.27 | 3.17 | 9.07 | 0.05 | 18 |

### 📈 All three

| Set | Time ÷ BFD, min | Time ÷ BFD, mean | Time ÷ BFD, median | Time ÷ BFD, max | Fill gained over BFD (mean points) | Problems where it fills more than BFD |
|---|---|---|---|---|---|---|
| thpack1 | 1.88 | 3.01 | 2.81 | 6.51 | 0.18 | 10 |
| thpack2 | 1.97 | 3.51 | 3.31 | 9.61 | 0.04 | 5 |
| thpack3 | 1.77 | 3.73 | 3.62 | 7.23 | 0.10 | 6 |
| thpack4 | 2.08 | 3.64 | 3.60 | 5.71 | 0.08 | 6 |
| thpack5 | 2.09 | 3.71 | 3.68 | 6.71 | 0.00 | 0 |
| thpack6 | 2.12 | 3.59 | 3.61 | 6.51 | 0.08 | 3 |
| thpack7 | 2.10 | 3.47 | 3.39 | 6.39 | 0.01 | 1 |
| **All sets** | 1.77 | 3.52 | 3.43 | 9.61 | 0.07 | 31 |

The same warning applies to Min and Max.

## 📊 Memory against FFD

On each problem, the memory the algorithm allocates divided by what FFD allocates.

### 📈 WFD ÷ FFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 0.92 | 1.01 | 1.00 | 1.08 |
| thpack2 | 0.90 | 1.01 | 1.01 | 1.09 |
| thpack3 | 0.91 | 1.03 | 1.01 | 1.33 |
| thpack4 | 0.93 | 1.02 | 1.01 | 1.21 |
| thpack5 | 0.89 | 1.01 | 1.00 | 1.20 |
| thpack6 | 0.90 | 1.00 | 1.00 | 1.09 |
| thpack7 | 0.91 | 1.01 | 1.01 | 1.10 |
| **All sets** | 0.89 | 1.01 | 1.01 | 1.33 |

### 📈 BFD ÷ FFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 0.87 | 0.99 | 0.99 | 1.07 |
| thpack2 | 0.90 | 0.99 | 1.00 | 1.12 |
| thpack3 | 0.85 | 0.99 | 0.99 | 1.11 |
| thpack4 | 0.84 | 0.97 | 0.98 | 1.08 |
| thpack5 | 0.86 | 0.96 | 0.96 | 1.06 |
| thpack6 | 0.87 | 0.94 | 0.94 | 1.02 |
| thpack7 | 0.86 | 0.94 | 0.94 | 1.04 |
| **All sets** | 0.84 | 0.97 | 0.97 | 1.12 |

## 📂 Where the numbers come from

The `Full_*_Packing` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms),
BenchmarkDotNet on an AMD Ryzen 9 9900X, .NET 10.0.12. Each algorithm's times come from its own report,
so every ratio here compares two files from the same machine, runtime and job.

As a check, the `Sample_*_Packing` reports repeat some of these problems with the default job. On those problems:

| Ratio | Median, Full runs | Median, Sample runs |
|---|---|---|
| WFD ÷ FFD | 8.82 | 8.94 |
| BFD ÷ FFD | 4.26 | 4.17 |
| BFD and FFD ÷ BFD | 1.23 | 1.24 |
| BFD and WFD ÷ BFD | 3.12 | 3.10 |
| All three ÷ BFD | 3.38 | 3.34 |
