# Packing efficiency

How full each algorithm packs the bin on every Bischoff problem, and where BFD is not the fullest. Version 2.
Fill is the packed volume as a percentage of the bin.

Every number here comes from the fill table in [measurements/packing-efficiency.md](measurements/packing-efficiency.md).

## 📊 Fill per set

Mean fill per set, in % of the bin.

| Set | Item types | FFD | WFD | BFD | Best of BFD and FFD | Best of BFD and WFD | Best of all three | If every item fit |
|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 75.70 | 74.78 | 80.73 | 80.79 | 80.90 | 80.91 | 99.58 |
| thpack2 | 5 | 74.56 | 70.43 | 81.62 | 81.65 | 81.65 | 81.66 | 99.47 |
| thpack3 | 8 | 73.81 | 68.23 | 82.12 | 82.19 | 82.17 | 82.22 | 99.49 |
| thpack4 | 10 | 73.29 | 68.19 | 81.60 | 81.67 | 81.62 | 81.68 | 99.43 |
| thpack5 | 12 | 72.14 | 67.68 | 81.74 | 81.74 | 81.74 | 81.74 | 99.36 |
| thpack6 | 15 | 72.71 | 67.76 | 80.83 | 80.86 | 80.89 | 80.91 | 99.45 |
| thpack7 | 20 | 71.63 | 67.41 | 80.17 | 80.18 | 80.17 | 80.18 | 99.37 |
| **All sets** | 3 to 20 | 73.41 | 69.21 | 81.26 | 81.30 | 81.31 | 81.33 | 99.45 |

- **Best of ...**: the fill you would get if you ran those algorithms and kept the best result on each problem.
- **If every item fit**: the items' total volume as a percentage of the bin. No packing can go above it.

## 📊 Spread per algorithm

Fill per set, in % of the bin: the lowest problem, the mean, the median and the highest problem.

### 📈 FFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 61.87 | 75.70 | 75.78 | 87.90 |
| thpack2 | 59.82 | 74.56 | 75.03 | 87.75 |
| thpack3 | 60.67 | 73.81 | 74.32 | 84.81 |
| thpack4 | 59.20 | 73.29 | 73.38 | 84.00 |
| thpack5 | 56.18 | 72.14 | 72.88 | 80.82 |
| thpack6 | 61.60 | 72.71 | 72.53 | 80.64 |
| thpack7 | 59.02 | 71.63 | 71.73 | 80.38 |
| **All sets** | 56.18 | 73.41 | 73.47 | 87.90 |

### 📈 WFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 60.42 | 74.78 | 75.12 | 87.90 |
| thpack2 | 49.15 | 70.43 | 71.29 | 87.40 |
| thpack3 | 51.04 | 68.23 | 68.05 | 84.76 |
| thpack4 | 52.26 | 68.19 | 68.12 | 84.05 |
| thpack5 | 53.69 | 67.68 | 67.34 | 78.77 |
| thpack6 | 54.76 | 67.76 | 68.40 | 77.13 |
| thpack7 | 57.85 | 67.41 | 68.17 | 77.91 |
| **All sets** | 49.15 | 69.21 | 69.05 | 87.90 |

### 📈 BFD

| Set | Min | Mean | Median | Max |
|---|---|---|---|---|
| thpack1 | 62.08 | 80.73 | 81.14 | 90.37 |
| thpack2 | 68.06 | 81.62 | 81.94 | 90.66 |
| thpack3 | 71.71 | 82.12 | 82.70 | 88.91 |
| thpack4 | 73.25 | 81.60 | 82.12 | 87.44 |
| thpack5 | 73.03 | 81.74 | 82.23 | 87.53 |
| thpack6 | 69.72 | 80.83 | 81.26 | 87.42 |
| thpack7 | 73.36 | 80.17 | 80.33 | 86.12 |
| **All sets** | 62.08 | 81.26 | 81.48 | 90.66 |

## 📊 Algorithm against algorithm

On each problem, the first algorithm's fill minus the second's, in points. The mean, median, min and max are
taken over those per-problem differences. A positive number means the first algorithm packed fuller.

### 📈 BFD minus FFD

| Set | Mean | Median | Min | Max | Problems where BFD is fuller | Equal | Problems where FFD is fuller |
|---|---|---|---|---|---|---|---|
| thpack1 | 5.03 | 4.00 | -2.60 | 21.21 | 80 | 14 | 6 |
| thpack2 | 7.06 | 6.69 | -1.03 | 21.80 | 93 | 4 | 3 |
| thpack3 | 8.32 | 8.25 | -1.74 | 19.17 | 94 | 1 | 5 |
| thpack4 | 8.31 | 8.19 | -2.82 | 24.80 | 94 | 0 | 6 |
| thpack5 | 9.60 | 9.45 | 0.34 | 21.96 | 100 | 0 | 0 |
| thpack6 | 8.13 | 8.32 | -1.48 | 20.23 | 98 | 0 | 2 |
| thpack7 | 8.54 | 8.35 | -0.65 | 19.85 | 99 | 0 | 1 |
| **All sets** | 7.85 | 7.83 | -2.82 | 24.80 | 658 | 19 | 23 |

### 📈 BFD minus WFD

| Set | Mean | Median | Min | Max | Problems where BFD is fuller | Equal | Problems where WFD is fuller |
|---|---|---|---|---|---|---|---|
| thpack1 | 5.95 | 4.90 | -6.50 | 23.52 | 76 | 15 | 9 |
| thpack2 | 11.19 | 10.58 | -1.38 | 29.92 | 93 | 3 | 4 |
| thpack3 | 13.89 | 14.76 | -3.79 | 31.63 | 96 | 1 | 3 |
| thpack4 | 13.42 | 12.71 | -1.68 | 31.74 | 99 | 0 | 1 |
| thpack5 | 14.07 | 14.55 | 1.53 | 27.21 | 100 | 0 | 0 |
| thpack6 | 13.07 | 12.12 | -5.65 | 28.77 | 99 | 0 | 1 |
| thpack7 | 12.76 | 12.98 | 3.30 | 25.57 | 100 | 0 | 0 |
| **All sets** | 12.05 | 11.88 | -6.50 | 31.74 | 663 | 19 | 18 |

### 📈 FFD minus WFD

| Set | Mean | Median | Min | Max | Problems where FFD is fuller | Equal | Problems where WFD is fuller |
|---|---|---|---|---|---|---|---|
| thpack1 | 0.92 | 0.00 | -6.78 | 13.54 | 41 | 31 | 28 |
| thpack2 | 4.13 | 2.71 | -5.24 | 26.22 | 77 | 8 | 15 |
| thpack3 | 5.58 | 4.47 | -5.04 | 24.00 | 85 | 3 | 12 |
| thpack4 | 5.10 | 4.25 | -7.73 | 19.45 | 86 | 0 | 14 |
| thpack5 | 4.47 | 4.37 | -4.84 | 13.94 | 85 | 0 | 15 |
| thpack6 | 4.94 | 3.92 | -6.72 | 18.69 | 88 | 0 | 12 |
| thpack7 | 4.22 | 4.26 | -2.84 | 13.20 | 87 | 0 | 13 |
| **All sets** | 4.19 | 3.28 | -7.73 | 26.22 | 549 | 42 | 109 |

## 📊 Where BFD is not the fullest

Per set: how many problems BFD packs fuller than both others, ties with the fullest, or loses.
"Points behind" is the fullest fill minus BFD's, on the problems BFD loses.

| Set | BFD alone fullest | BFD tied fullest | BFD behind | Mean points behind | Most points behind |
|---|---|---|---|---|---|
| thpack1 | 76 | 14 | 10 | 1.84 | 6.50 |
| thpack2 | 91 | 4 | 5 | 0.80 | 1.38 |
| thpack3 | 93 | 1 | 6 | 1.59 | 3.79 |
| thpack4 | 94 | 0 | 6 | 1.26 | 2.82 |
| thpack5 | 100 | 0 | 0 | - | - |
| thpack6 | 97 | 0 | 3 | 2.67 | 5.65 |
| thpack7 | 99 | 0 | 1 | 0.65 | 0.65 |
| **All sets** | 650 | 19 | 31 | 1.55 | 6.50 |

Every problem where BFD is behind, most points behind first.

| Problem | Item types | Items | BFD | Fullest | Reached by | Points behind |
|---|---|---|---|---|---|---|
| thpack1_58 | 3 | 92 | 77.83 | 84.33 | WFD | 6.50 |
| thpack6_93 | 15 | 86 | 69.72 | 75.37 | WFD | 5.65 |
| thpack1_40 | 3 | 107 | 78.78 | 83.82 | WFD | 5.04 |
| thpack3_43 | 8 | 92 | 76.22 | 80.01 | WFD | 3.79 |
| thpack4_25 | 10 | 133 | 76.84 | 79.66 | FFD | 2.82 |
| thpack4_35 | 10 | 151 | 81.55 | 84.00 | FFD | 2.45 |
| thpack3_28 | 8 | 143 | 71.71 | 73.45 | FFD | 1.74 |
| thpack4_63 | 10 | 100 | 82.37 | 84.05 | WFD | 1.68 |
| thpack1_23 | 3 | 128 | 80.08 | 81.74 | WFD | 1.66 |
| thpack3_69 | 8 | 162 | 78.27 | 79.86 | FFD | 1.59 |
| thpack6_89 | 15 | 100 | 73.72 | 75.20 | FFD | 1.48 |
| thpack2_58 | 5 | 114 | 82.36 | 83.74 | WFD | 1.38 |
| thpack1_45 | 3 | 140 | 75.66 | 76.86 | WFD | 1.20 |
| thpack3_58 | 8 | 87 | 79.92 | 81.02 | FFD | 1.10 |
| thpack2_72 | 5 | 88 | 79.62 | 80.61 | FFD | 0.99 |
| thpack3_35 | 8 | 149 | 82.10 | 83.06 | FFD | 0.96 |
| thpack6_98 | 15 | 147 | 74.77 | 75.65 | FFD | 0.88 |
| thpack2_12 | 5 | 86 | 68.06 | 68.94 | FFD, WFD | 0.88 |
| thpack1_37 | 3 | 99 | 81.88 | 82.75 | WFD | 0.87 |
| thpack1_17 | 3 | 213 | 83.38 | 84.21 | WFD | 0.83 |
| thpack1_41 | 3 | 109 | 84.43 | 85.26 | FFD | 0.83 |
| thpack1_12 | 3 | 104 | 65.88 | 66.63 | FFD, WFD | 0.75 |
| thpack7_45 | 20 | 140 | 79.73 | 80.38 | FFD | 0.65 |
| thpack2_65 | 5 | 187 | 81.91 | 82.54 | WFD | 0.63 |
| thpack1_29 | 3 | 109 | 78.21 | 78.71 | WFD | 0.50 |
| thpack3_87 | 8 | 126 | 81.49 | 81.86 | WFD | 0.37 |
| thpack4_38 | 10 | 115 | 77.75 | 78.11 | FFD | 0.36 |
| thpack4_27 | 10 | 119 | 77.06 | 77.28 | FFD | 0.22 |
| thpack1_4 | 3 | 197 | 79.83 | 80.02 | FFD, WFD | 0.19 |
| thpack2_68 | 5 | 130 | 86.53 | 86.64 | WFD | 0.11 |
| thpack4_93 | 10 | 78 | 79.81 | 79.84 | FFD | 0.03 |
