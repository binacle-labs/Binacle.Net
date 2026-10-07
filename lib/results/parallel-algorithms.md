# Parallel algorithms

When several algorithms run on one bin, is running them in parallel faster than one after the other, from
what size, and on how many cores? Version 2, packing.

Every case packs one item size, 20x15x10, repeated from 1 to 256 pieces, into a 160x120x80 bin. There is no
fill to compare: for what running more than one algorithm buys in fill, see
[algorithm-cost.md](algorithm-cost.md).

## 📊 Parallel against one after the other

Parallel time divided by one-after-the-other time, at the same piece count and core count, ± how much that
ratio wobbled (RatioSD). Below 1 means parallel is faster.

### 📈 FFD and BFD

| Pieces | 2 cores | 4 cores | 8 cores | 12 cores |
|---|---|---|---|---|
| 1 | 1.84 ± 0.06 | 1.91 ± 0.07 | 1.83 ± 0.09 | 1.92 ± 0.04 |
| 2 | 1.53 ± 0.06 | 1.62 ± 0.13 | 1.63 ± 0.06 | 1.64 ± 0.07 |
| 4 | 1.33 ± 0.06 | 1.43 ± 0.07 | 1.42 ± 0.12 | 1.46 ± 0.07 |
| 8 | 1.10 ± 0.02 | 1.18 ± 0.05 | 1.15 ± 0.07 | 1.13 ± 0.06 |
| 16 | 0.92 ± 0.03 | 0.93 ± 0.03 | 0.96 ± 0.05 | 0.90 ± 0.04 |
| 32 | 0.75 ± 0.02 | 0.79 ± 0.04 | 0.80 ± 0.03 | 0.77 ± 0.02 |
| 64 | 0.72 ± 0.04 | 0.70 ± 0.03 | 0.68 ± 0.02 | 0.67 ± 0.02 |
| 128 | 0.67 ± 0.06 | 0.65 ± 0.04 | 0.66 ± 0.02 | 0.68 ± 0.05 |
| 256 | 0.63 ± 0.02 | 0.66 ± 0.07 | 0.72 ± 0.07 | 0.64 ± 0.10 |

### 📈 All three

| Pieces | 2 cores | 4 cores | 8 cores | 12 cores |
|---|---|---|---|---|
| 1 | 1.36 ± 0.08 | 1.56 ± 0.08 | 1.96 ± 0.10 | 1.97 ± 0.13 |
| 2 | 1.19 ± 0.04 | 1.42 ± 0.05 | 1.85 ± 0.10 | 1.89 ± 0.23 |
| 4 | 1.03 ± 0.02 | 1.20 ± 0.07 | 1.58 ± 0.11 | 1.41 ± 0.10 |
| 8 | 0.85 ± 0.03 | 0.99 ± 0.04 | 1.28 ± 0.11 | 1.16 ± 0.05 |
| 16 | 0.75 ± 0.02 | 0.80 ± 0.03 | 1.04 ± 0.08 | 1.02 ± 0.09 |
| 32 | 0.68 ± 0.02 | 0.78 ± 0.03 | 1.04 ± 0.14 | 0.97 ± 0.03 |
| 64 | 0.95 ± 0.09 | 1.11 ± 0.12 | 0.96 ± 0.07 | 1.33 ± 0.17 |
| 128 | 1.09 ± 0.24 | 1.19 ± 0.24 | 0.81 ± 0.14 | 0.90 ± 0.16 |
| 256 | 0.74 ± 0.14 | 0.74 ± 0.14 | 0.70 ± 0.14 | 0.90 ± 0.03 |

## 📊 Where parallel starts to win

The smallest piece count where parallel is faster even after its wobble: ratio plus RatioSD below 1.

| Race | 2 cores | 4 cores | 8 cores | 12 cores |
|---|---|---|---|---|
| FFD and BFD | 16 | 16 | 32 | 16 |
| All three | 8 | 16 | 128 | 256 |

Past that size parallel is not always faster. It loses or ties again at these piece counts:

- All three, 2 cores: 64, 128
- All three, 4 cores: 64, 128

## 📊 What going parallel costs on its own

An algorithm racing nothing, at 1 to 16 pieces: parallel time minus one-after-the-other time, lowest to highest
over those piece counts, in µs. Nothing is handed to another core, so this is only the cost of setting up.

| Algorithm | 2 cores | 4 cores | 8 cores | 12 cores |
|---|---|---|---|---|
| FFD | 0.25 to 0.28 | 0.27 to 0.29 | 0.25 to 0.27 | 0.23 to 0.28 |
| WFD | 0.23 to 0.28 | 0.23 to 0.28 | 0.21 to 0.29 | 0.26 to 0.28 |
| BFD | 0.22 to 0.25 | 0.23 to 0.25 | 0.26 to 0.31 | 0.26 to 0.28 |

## 📊 One after the other, measured against summed

A race's one-after-the-other time divided by the sum of each algorithm's time on its own, at the same piece
count and core count. Above 1 means running them in one call costs more than the separate runs add up to.
[algorithm-cost.md](algorithm-cost.md) uses the sum.

| Race | Min | Median | Max |
|---|---|---|---|
| FFD and BFD | 1.02 | 1.16 | 1.22 |
| All three | 1.14 | 1.25 | 1.57 |

The bench treats 1.5 or more as a slow process, not a real result. That is all three at 64 pieces on 8 cores (1.57).

## ⚠️ What will bite you

- **All three from 64 pieces up is noisy**: RatioSD reaches 0.14 to 0.24, and the one slow-process cell is
  in that range. Read those rows as rough.
- **All three is slower in parallel on more cores at small sizes**: at 1 piece it is 1.36 on
  2 cores and 1.97 on 12 cores. FFD and BFD stays between 1.83 and 1.92.
- **These runs used a short job** (3 iterations, 3 launches). It blurs ratios near 1.

## 📂 Where the numbers come from

The `Identical_*_Packing` reports in
[benchmarks/baseline/parallel-algorithms/](benchmarks/baseline/parallel-algorithms), BenchmarkDotNet on an AMD
Ryzen 9 9900X, .NET 10.0.12, each case pinned to its core count.
