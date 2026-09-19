# Packing efficiency results

Written by `just measure lib` from `Binacle.Lib.PackingEfficiency` over the 700 Bischoff suite scenarios (thpack1..7). Do not edit.

## 📊 Fill per algorithm
Fill as a percentage of the bin, over all 700 scenarios.

| Algorithm | Min   | Mean  | Median | Max   | StdDev |
|-----------|-------|-------|--------|-------|--------|
| FFD       | 56.18 | 73.41 | 73.47  | 87.90 | 5.35   |
| WFD       | 49.15 | 69.21 | 69.05  | 87.90 | 6.88   |
| BFD       | 62.08 | 81.26 | 81.48  | 90.66 | 3.78   |

## 📊 Fill per set
The same, per thpack file. Each set is 100 scenarios with the same number of item types.

| Set            | Algorithm | Min   | Mean  | Median | Max   | StdDev |
|----------------|-----------|-------|-------|--------|-------|--------|
| BR1 (3 types)  | FFD       | 61.87 | 75.70 | 75.78  | 87.90 | 6.62   |
| BR1 (3 types)  | WFD       | 60.42 | 74.78 | 75.12  | 87.90 | 7.19   |
| BR1 (3 types)  | BFD       | 62.08 | 80.73 | 81.14  | 90.37 | 5.39   |
| BR2 (5 types)  | FFD       | 59.82 | 74.56 | 75.03  | 87.75 | 5.87   |
| BR2 (5 types)  | WFD       | 49.15 | 70.43 | 71.29  | 87.40 | 8.20   |
| BR2 (5 types)  | BFD       | 68.06 | 81.62 | 81.94  | 90.66 | 4.52   |
| BR3 (8 types)  | FFD       | 60.67 | 73.81 | 74.32  | 84.81 | 5.62   |
| BR3 (8 types)  | WFD       | 51.04 | 68.23 | 68.05  | 84.76 | 6.81   |
| BR3 (8 types)  | BFD       | 71.71 | 82.12 | 82.70  | 88.91 | 3.47   |
| BR4 (10 types) | FFD       | 59.20 | 73.29 | 73.38  | 84.00 | 5.12   |
| BR4 (10 types) | WFD       | 52.26 | 68.19 | 68.12  | 84.05 | 6.35   |
| BR4 (10 types) | BFD       | 73.25 | 81.60 | 82.12  | 87.44 | 3.03   |
| BR5 (12 types) | FFD       | 56.18 | 72.14 | 72.88  | 80.82 | 4.61   |
| BR5 (12 types) | WFD       | 53.69 | 67.68 | 67.34  | 78.77 | 5.73   |
| BR5 (12 types) | BFD       | 73.03 | 81.74 | 82.23  | 87.53 | 3.00   |
| BR6 (15 types) | FFD       | 61.60 | 72.71 | 72.53  | 80.64 | 3.99   |
| BR6 (15 types) | WFD       | 54.76 | 67.76 | 68.40  | 77.13 | 5.57   |
| BR6 (15 types) | BFD       | 69.72 | 80.83 | 81.26  | 87.42 | 3.15   |
| BR7 (20 types) | FFD       | 59.02 | 71.63 | 71.73  | 80.38 | 3.89   |
| BR7 (20 types) | WFD       | 57.85 | 67.41 | 68.17  | 77.91 | 4.42   |
| BR7 (20 types) | BFD       | 73.36 | 80.17 | 80.33  | 86.12 | 2.72   |

## 📊 The user's fill
The fill a caller gets when the best of several algorithms is kept, from the same packings.

| Picks                                    | Min   | Mean  | Median | Max   | StdDev |
|------------------------------------------|-------|-------|--------|-------|--------|
| Best of FFD and BFD (what the API races) | 62.08 | 81.30 | 81.48  | 90.66 | 3.75   |
| Best of all three                        | 62.08 | 81.33 | 81.50  | 90.66 | 3.73   |

## 🔢 Wins
How many of the 700 scenarios each algorithm packed at least as well as the other two. A tie counts for every algorithm in it.

| Algorithm | Best or tied |
|-----------|--------------|
| FFD       | 35           |
| WFD       | 35           |
| BFD       | 669          |

## 🔢 Version parity
Scenarios where the shipped v2 packs to the same fill as v1.

| Algorithm | Same as v1 | Different |
|-----------|------------|-----------|
| FFD       | 700        | 0         |
| WFD       | 700        | 0         |
| BFD       | 699        | 1         |

## 📂 Files

| File                                           | What it is                                                           |
|------------------------------------------------|----------------------------------------------------------------------|
| [packing-efficiency.md](packing-efficiency.md) | One row per scenario: fill per algorithm, which won, and by how much |
| [version-parity.md](version-parity.md)         | Per algorithm, only the scenarios where v1 and v2 pack differently   |

