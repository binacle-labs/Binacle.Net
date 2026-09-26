# Version differences - fitting

Is v2 faster and lighter than v1 when fitting, per algorithm?

> Every number in this file is fake. The tables show the shape only.

## 📊 v2 against v1, mean per set

<!--
Table: v2 time and memory as × of v1, per set, per algorithm.
Reads: lib/results/benchmarks/baseline/algorithms/Full_<FFD|BFD|WFD>_Fitting.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, FFD time, FFD memory, BFD time, BFD memory, WFD time, WFD memory - each × of v1.
Notes: time is the v2 row's Ratio, memory its Alloc Ratio, per problem; then the mean per set. An average above 1.00×
is bold. Item types come from the inputs, shared/data/bischoff-suite/orlib_thpack<N>.json.
v1 and v2 ran side by side in one class, so the ratios compare on any machine; the file says so.
-->

| Set | Item types | FFD time | FFD memory | BFD time | BFD memory | WFD time | WFD memory |
|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.33× | 0.44× | 0.55× | 0.66× | 0.77× | 0.88× |
| thpack2 | 5 | 0.99× | 0.11× | 0.22× | 0.33× | 0.44× | 0.55× |
| thpack3 | 8 | 0.66× | 0.77× | 0.88× | 0.99× | 0.11× | 0.22× |
| thpack4 | 10 | 0.33× | 0.44× | 0.55× | 0.66× | 0.77× | 0.88× |
| thpack5 | 12 | 0.99× | 0.11× | 0.22× | 0.33× | 0.44× | 0.55× |
| thpack6 | 15 | 0.66× | 0.77× | 0.88× | 0.99× | **1.11×** | 0.22× |
| thpack7 | 20 | 0.33× | 0.44× | 0.55× | 0.66× | 0.77× | 0.88× |
| All 700 | 3 to 20 | 0.99× | 0.11× | 0.22× | 0.33× | 0.44× | 0.55× |

## 📊 FFD v2 against v1, per set

<!--
Table: the spread of FFD v2 time and memory against v1 inside each set.
Reads: lib/results/benchmarks/baseline/algorithms/Full_FFD_Fitting.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Time min, Time mean, Time median, Time max, Memory min, Memory mean, Memory median,
Memory max - each × of v1.
Notes: the v2 row's Ratio (time) and Alloc Ratio (memory) per problem; then min, mean, median and max over the set.
A mean above 1.00× is bold.
-->

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.66× | 0.77× | 0.88× | 9.99× | 0.11× | 0.22× | 0.33× | 0.44× |
| thpack2 | 5 | 0.55× | 0.66× | 0.77× | 8.88× | 0.99× | 0.11× | 0.22× | 0.33× |
| thpack3 | 8 | 0.44× | 0.55× | 0.66× | 7.77× | 0.88× | 0.99× | 0.11× | 0.22× |
| thpack4 | 10 | 0.33× | 0.44× | 0.55× | 6.66× | 0.77× | 0.88× | 0.99× | 0.11× |
| thpack5 | 12 | 0.22× | **1.33×** | 0.44× | 5.55× | 0.66× | 0.77× | 0.88× | 0.99× |
| thpack6 | 15 | 0.11× | 0.22× | 0.33× | 4.44× | 0.55× | 0.66× | 0.77× | 0.88× |
| thpack7 | 20 | 0.99× | 0.11× | 0.22× | 3.33× | 0.44× | 0.55× | 0.66× | 0.77× |
| All 700 | 3 to 20 | 0.88× | 0.99× | 0.11× | 2.22× | 0.33× | 0.44× | 0.55× | 0.66× |

## 📊 BFD v2 against v1, per set

<!--
Table: the spread of BFD v2 time and memory against v1 inside each set.
Reads: lib/results/benchmarks/baseline/algorithms/Full_BFD_Fitting.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Time min, Time mean, Time median, Time max, Memory min, Memory mean, Memory median,
Memory max - each × of v1.
Notes: the v2 row's Ratio (time) and Alloc Ratio (memory) per problem; then min, mean, median and max over the set.
A mean above 1.00× is bold.
-->

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.77× | 0.88× | 0.99× | 1.11× | 0.22× | 0.33× | 0.44× | 0.55× |
| thpack2 | 5 | 0.66× | 0.77× | 0.88× | 9.99× | 0.11× | 0.22× | 0.33× | 0.44× |
| thpack3 | 8 | 0.55× | 0.66× | 0.77× | 8.88× | 0.99× | 0.11× | 0.22× | 0.33× |
| thpack4 | 10 | 0.44× | 0.55× | 0.66× | 7.77× | 0.88× | 0.99× | 0.11× | 0.22× |
| thpack5 | 12 | 0.33× | **1.44×** | 0.55× | 6.66× | 0.77× | 0.88× | 0.99× | 0.11× |
| thpack6 | 15 | 0.22× | 0.33× | 0.44× | 5.55× | 0.66× | 0.77× | 0.88× | 0.99× |
| thpack7 | 20 | 0.11× | 0.22× | 0.33× | 4.44× | 0.55× | 0.66× | 0.77× | 0.88× |
| All 700 | 3 to 20 | 0.99× | 0.11× | 0.22× | 3.33× | 0.44× | 0.55× | 0.66× | 0.77× |

## 📊 WFD v2 against v1, per set

<!--
Table: the spread of WFD v2 time and memory against v1 inside each set.
Reads: lib/results/benchmarks/baseline/algorithms/Full_WFD_Fitting.md
Rows: thpack1 to thpack7, then All 700.
Columns: Set, Item types, Time min, Time mean, Time median, Time max, Memory min, Memory mean, Memory median,
Memory max - each × of v1.
Notes: the v2 row's Ratio (time) and Alloc Ratio (memory) per problem; then min, mean, median and max over the set.
A mean above 1.00× is bold.
-->

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.88× | 0.99× | 0.11× | 2.22× | 0.33× | 0.44× | 0.55× | 0.66× |
| thpack2 | 5 | 0.77× | 0.88× | 0.99× | 1.11× | 0.22× | 0.33× | 0.44× | 0.55× |
| thpack3 | 8 | 0.66× | 0.77× | 0.88× | 9.99× | 0.11× | 0.22× | 0.33× | 0.44× |
| thpack4 | 10 | 0.55× | 0.66× | 0.77× | 8.88× | 0.99× | 0.11× | 0.22× | 0.33× |
| thpack5 | 12 | 0.44× | **1.55×** | 0.66× | 7.77× | 0.88× | 0.99× | 0.11× | 0.22× |
| thpack6 | 15 | 0.33× | 0.44× | 0.55× | 6.66× | 0.77× | 0.88× | 0.99× | 0.11× |
| thpack7 | 20 | 0.22× | 0.33× | 0.44× | 5.55× | 0.66× | 0.77× | 0.88× | 0.99× |
| All 700 | 3 to 20 | 0.11× | 0.22× | 0.33× | 4.44× | 0.55× | 0.66× | 0.77× | 0.88× |

## Gaps and open questions

<!--
None set for this file yet.
-->
