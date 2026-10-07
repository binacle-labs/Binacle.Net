# Packing time by size

How v2 packing time grows with the number of items and item types, for FFD, WFD and BFD.

## 📊 By item count

Every Bischoff problem, grouped by how many items it has. Median time per group, in µs.

| Items | Problems | FFD | WFD | BFD |
|---|---|---|---|---|
| 69 to 99 | 74 | 11.6 | 80.5 | 52.9 |
| 100 to 124 | 210 | 17.2 | 138.0 | 70.0 |
| 125 to 149 | 240 | 23.6 | 214.7 | 94.7 |
| 150 to 199 | 145 | 30.1 | 318.4 | 117.0 |
| 200 to 476 | 31 | 39.5 | 400.5 | 149.8 |

### 📈 What doubling the items does

A fit over every problem: when the items double, the time is multiplied by about this much.

| Algorithm | Time multiplied by |
|---|---|
| FFD | 2.5 |
| WFD | 3.5 |
| BFD | 2.4 |

Item count explains only part of the time. Problems with the same item count differ in time by up to 3x for
FFD, 6x for WFD and 5x for BFD.

## 📊 By set

Each Bischoff set has a fixed number of item types. Median time per set, in µs.

| Set | Item types | Items, median | FFD | WFD | BFD |
|---|---|---|---|---|---|
| thpack1 | 3 | 133 | 16.9 | 112.5 | 71.5 |
| thpack2 | 5 | 132.5 | 17.7 | 158.7 | 73.8 |
| thpack3 | 8 | 132 | 19.6 | 187.9 | 80.3 |
| thpack4 | 10 | 132 | 21.0 | 200.6 | 85.5 |
| thpack5 | 12 | 132.5 | 22.5 | 207.5 | 88.9 |
| thpack6 | 15 | 131 | 24.0 | 227.0 | 97.8 |
| thpack7 | 20 | 129 | 24.0 | 218.4 | 100.8 |
| **All sets** | 3 to 20 | 131 | 21.4 | 192.5 | 86.7 |

## 📊 At the small end

A ladder of made-up orders from 3 to 79 items, packed into the largest bin. Each step adds one more item
type, so items and item types grow together. Mean time, in µs.

| Items | Item types | FFD | WFD | BFD |
|---|---|---|---|---|
| 3 | 1 | 0.38 | 0.42 | 0.35 |
| 7 | 2 | 0.63 | 0.84 | 0.63 |
| 13 | 3 | 1.04 | 1.74 | 1.15 |
| 17 | 4 | 1.35 | 2.46 | 1.51 |
| 23 | 5 | 1.69 | 3.84 | 1.95 |
| 29 | 6 | 2.12 | 5.73 | 2.74 |
| 37 | 7 | 2.73 | 8.39 | 4.30 |
| 47 | 8 | 3.84 | 14.93 | 4.75 |
| 59 | 9 | 4.89 | 17.59 | 6.15 |
| 67 | 10 | 5.11 | 15.84 | 7.88 |
| 79 | 11 | 6.24 * | 21.60 * | 11.42 |

\* At 79 items FFD and WFD do not pack every item. The time is real, but it is not the same work as the
steps before it.

## 📂 Where the numbers come from

- Bischoff times: the `Full_*_Packing` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms),
  short job. Each algorithm comes from its own report, so FFD, WFD and BFD are compared across files from the
  same machine, runtime and job.
- Items and item types per problem: [measurements/packing-efficiency.md](measurements/packing-efficiency.md).
- The ladder: [benchmarks/baseline/scaling/Sample_Packing.md](benchmarks/baseline/scaling/Sample_Packing.md),
  default job, every algorithm in one report.
- Both ran on an AMD Ryzen 9 9900X, .NET 10.0.12.
