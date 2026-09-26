# Algorithm performance

What do WFD, BFD and the two races cost in time against FFD, when packing? Version 2.

> Every number in this file is fake. The tables show the shape only.

## 📊 Packing time against FFD, mean per set

<!--
Table: packing time as × of FFD, per set.
Reads: lib/results/benchmarks/baseline/algorithms/Full_<FFD|WFD|BFD>_Packing.md - the v2 rows.
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, WFD, BFD, Race FFD+BFD (loop), Race all three (loop) - each × of FFD.
Notes: per problem, divide by FFD's time on that same problem, then take the mean per set. A loop race's time is the
sum of its members on that problem. The race columns are what a loop race should cost; a measured loop far from them
is a fault. Item types come from the inputs, shared/data/bischoff-suite/orlib_thpack<N>.json.
The file names the machine, runtime and job (short, three iterations), read from the report header.
Under it, one line: parallel race times are in parallel-racing.md.
-->

| Set | Item types | WFD | BFD | Race FFD+BFD (loop) | Race all three (loop) |
|---|---|---|---|---|---|
| thpack1 | 3 | 9.99× | 1.11× | 2.22× | 3.33× |
| thpack2 | 5 | 4.44× | 5.55× | 6.66× | 7.77× |
| thpack3 | 8 | 8.88× | 9.99× | 1.11× | 2.22× |
| thpack4 | 10 | 3.33× | 4.44× | 5.55× | 6.66× |
| thpack5 | 12 | 7.77× | 8.88× | 9.99× | 1.11× |
| thpack6 | 15 | 2.22× | 3.33× | 4.44× | 5.55× |
| thpack7 | 20 | 6.66× | 7.77× | 8.88× | 9.99× |
| All 700 | 3 to 20 | 1.11× | 2.22× | 3.33× | 4.44× |

<!--
Words, no table: one sentence on memory - BFD and WFD against FFD, over all 700.
-->

BFD allocates 1.11× of FFD's memory, WFD 2.22×, over all 700.

## 📊 BFD packing time against FFD, per set

<!--
Table: the spread of BFD time against FFD inside each set.
Reads: lib/results/benchmarks/baseline/algorithms/Full_<FFD|BFD>_Packing.md - the v2 rows.
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Min, Mean, Median, Max - each × of FFD.
Notes: per problem, divide by FFD's time on that same problem; then min, mean, median and max over the set.
-->

| Set | Item types | Min | Mean | Median | Max |
|---|---|---|---|---|---|
| thpack1 | 3 | 5.55× | 6.66× | 7.77× | 8.88× |
| thpack2 | 5 | 9.99× | 1.11× | 2.22× | 3.33× |
| thpack3 | 8 | 4.44× | 5.55× | 6.66× | 7.77× |
| thpack4 | 10 | 8.88× | 9.99× | 1.11× | 2.22× |
| thpack5 | 12 | 3.33× | 4.44× | 5.55× | 6.66× |
| thpack6 | 15 | 7.77× | 8.88× | 9.99× | 1.11× |
| thpack7 | 20 | 2.22× | 3.33× | 4.44× | 5.55× |
| All 700 | 3 to 20 | 6.66× | 7.77× | 8.88× | 9.99× |

## 📊 WFD packing time against FFD, per set

<!--
Table: the spread of WFD time against FFD inside each set.
Reads: lib/results/benchmarks/baseline/algorithms/Full_<FFD|WFD>_Packing.md - the v2 rows.
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Min, Mean, Median, Max - each × of FFD.
Notes: per problem, divide by FFD's time on that same problem; then min, mean, median and max over the set.
-->

| Set | Item types | Min | Mean | Median | Max |
|---|---|---|---|---|---|
| thpack1 | 3 | 1.11× | 2.22× | 3.33× | 4.44× |
| thpack2 | 5 | 5.55× | 6.66× | 7.77× | 8.88× |
| thpack3 | 8 | 9.99× | 1.11× | 2.22× | 3.33× |
| thpack4 | 10 | 4.44× | 5.55× | 6.66× | 7.77× |
| thpack5 | 12 | 8.88× | 9.99× | 1.11× | 2.22× |
| thpack6 | 15 | 3.33× | 4.44× | 5.55× | 6.66× |
| thpack7 | 20 | 7.77× | 8.88× | 9.99× | 1.11× |
| All 700 | 3 to 20 | 2.22× | 3.33× | 4.44× | 5.55× |

## Gaps and open questions

<!--
None set for this file yet.
-->
