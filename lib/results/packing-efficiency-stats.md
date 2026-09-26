# Packing efficiency stats

How full does each algorithm, and each race of them, pack the 700 Bischoff problems? Version 2.

> Every number in this file is fake. The tables show the shape only.

## 📊 Mean fill per set

<!--
Table: mean fill per Bischoff set, per algorithm and per race.
Reads: lib/results/measurements/packing-efficiency.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, FFD, WFD, BFD, Race FFD+BFD, Race all three - fill in % of the bin.
Notes: a race's fill on one problem is the best fill of its members; then the mean per set.
Item types come from the Types column (3, 5, 8, 10, 12, 15, 20 for thpack1 to thpack7; All 700 reads 3 to 20).
-->

| Set | Item types | FFD (%) | WFD (%) | BFD (%) | Race FFD+BFD (%) | Race all three (%) |
|---|---|---|---|---|---|---|
| thpack1 | 3 | 11.1 | 22.2 | 33.3 | 44.4 | 55.5 |
| thpack2 | 5 | 66.6 | 77.7 | 88.8 | 99.9 | 11.1 |
| thpack3 | 8 | 22.2 | 33.3 | 44.4 | 55.5 | 66.6 |
| thpack4 | 10 | 77.7 | 88.8 | 99.9 | 11.1 | 22.2 |
| thpack5 | 12 | 33.3 | 44.4 | 55.5 | 66.6 | 77.7 |
| thpack6 | 15 | 88.8 | 99.9 | 11.1 | 22.2 | 33.3 |
| thpack7 | 20 | 44.4 | 55.5 | 66.6 | 77.7 | 88.8 |
| All 700 | 3 to 20 | 99.9 | 11.1 | 22.2 | 33.3 | 44.4 |

## 📊 The options, all 700

<!--
Table: each option over all 700 problems.
Reads: lib/results/measurements/packing-efficiency.md
Rows: FFD, WFD, BFD, Race FFD+BFD, Race all three.
Columns: Option, Mean fill (%), Best or tied on (count, of 700).
Notes: "best or tied" means its fill equals the best of all three on that problem.
-->

| Option | Mean fill (%) | Best or tied on (of 700) |
|---|---|---|
| FFD | 55.5 | 666 |
| WFD | 77.7 | 888 |
| BFD | 99.9 | 111 |
| Race FFD+BFD | 22.2 | 333 |
| Race all three | 44.4 | 555 |

## 📊 Headroom

<!--
Table: how far the best fill sits below the ceiling, per set.
Reads: lib/results/measurements/packing-efficiency.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Ceiling (%), Best of three (%), Left (points).
Notes: Ceiling is the Ceiling % column - the items' volume as a share of the bin. Best of three is the best fill of
FFD, WFD and BFD on each problem. Both are the mean per set; Left is Ceiling minus Best of three.
Under it, the question in the last section.
-->

| Set | Item types | Ceiling (%) | Best of three (%) | Left (points) |
|---|---|---|---|---|
| thpack1 | 3 | 66.6 | 77.7 | 88.8 |
| thpack2 | 5 | 99.9 | 11.1 | 22.2 |
| thpack3 | 8 | 33.3 | 44.4 | 55.5 |
| thpack4 | 10 | 66.6 | 77.7 | 88.8 |
| thpack5 | 12 | 99.9 | 11.1 | 22.2 |
| thpack6 | 15 | 33.3 | 44.4 | 55.5 |
| thpack7 | 20 | 66.6 | 77.7 | 88.8 |
| All 700 | 3 to 20 | 99.9 | 11.1 | 22.2 |

## 📊 FFD fill per set

<!--
Table: the spread of FFD fill inside each set.
Reads: lib/results/measurements/packing-efficiency.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Min, Mean, Median, Max - fill in %.
-->

| Set | Item types | Min (%) | Mean (%) | Median (%) | Max (%) |
|---|---|---|---|---|---|
| thpack1 | 3 | 33.3 | 44.4 | 55.5 | 66.6 |
| thpack2 | 5 | 77.7 | 88.8 | 99.9 | 11.1 |
| thpack3 | 8 | 22.2 | 33.3 | 44.4 | 55.5 |
| thpack4 | 10 | 66.6 | 77.7 | 88.8 | 99.9 |
| thpack5 | 12 | 11.1 | 22.2 | 33.3 | 44.4 |
| thpack6 | 15 | 55.5 | 66.6 | 77.7 | 88.8 |
| thpack7 | 20 | 99.9 | 11.1 | 22.2 | 33.3 |
| All 700 | 3 to 20 | 44.4 | 55.5 | 66.6 | 77.7 |

## 📊 BFD fill per set

<!--
Table: the spread of BFD fill inside each set.
Reads: lib/results/measurements/packing-efficiency.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Min, Mean, Median, Max - fill in %.
-->

| Set | Item types | Min (%) | Mean (%) | Median (%) | Max (%) |
|---|---|---|---|---|---|
| thpack1 | 3 | 88.8 | 99.9 | 11.1 | 22.2 |
| thpack2 | 5 | 33.3 | 44.4 | 55.5 | 66.6 |
| thpack3 | 8 | 77.7 | 88.8 | 99.9 | 11.1 |
| thpack4 | 10 | 22.2 | 33.3 | 44.4 | 55.5 |
| thpack5 | 12 | 66.6 | 77.7 | 88.8 | 99.9 |
| thpack6 | 15 | 11.1 | 22.2 | 33.3 | 44.4 |
| thpack7 | 20 | 55.5 | 66.6 | 77.7 | 88.8 |
| All 700 | 3 to 20 | 99.9 | 11.1 | 22.2 | 33.3 |

## 📊 WFD fill per set

<!--
Table: the spread of WFD fill inside each set.
Reads: lib/results/measurements/packing-efficiency.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Min, Mean, Median, Max - fill in %.
-->

| Set | Item types | Min (%) | Mean (%) | Median (%) | Max (%) |
|---|---|---|---|---|---|
| thpack1 | 3 | 44.4 | 55.5 | 66.6 | 77.7 |
| thpack2 | 5 | 88.8 | 99.9 | 11.1 | 22.2 |
| thpack3 | 8 | 33.3 | 44.4 | 55.5 | 66.6 |
| thpack4 | 10 | 77.7 | 88.8 | 99.9 | 11.1 |
| thpack5 | 12 | 22.2 | 33.3 | 44.4 | 55.5 |
| thpack6 | 15 | 66.6 | 77.7 | 88.8 | 99.9 |
| thpack7 | 20 | 11.1 | 22.2 | 33.3 | 44.4 |
| All 700 | 3 to 20 | 55.5 | 66.6 | 77.7 | 88.8 |

## Gaps and open questions

<!--
Question, under the headroom table: the ceiling is the items' volume, not the best packing possible, which nobody
knows. So "Left" is not points that can be reached. Is fill the next thing to improve, and how would we know how
much is reachable?
Gap: no comparison against published results on the Bischoff problems.
-->
