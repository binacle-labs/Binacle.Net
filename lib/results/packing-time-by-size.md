# Packing time by size

How does v2 packing time grow with the number of items, over the 700 Bischoff problems?

> Every number in this file is fake. The tables show the shape only.

## 📊 v2 packing time by item count

<!--
Table: median v2 packing time per item-count band, per algorithm.
Reads: lib/results/benchmarks/baseline/algorithms/Full_<FFD|WFD|BFD>_Packing.md - the v2 rows. Item counts come from
the inputs, shared/data/bischoff-suite/orlib_thpack<N>.json (third field of Metrics).
Rows: bands of 69 to 99, 100 to 119, 120 to 139, 140 to 159, 160 to 199, 200 to 476 items.
Columns: Items, Problems (count), FFD median, WFD median, BFD median - in μs.
Notes: medians, because times inside a band vary a lot. Times hold on one machine and runtime; the file names them,
read from the report header.
-->

| Items | Problems | FFD median (μs) | WFD median (μs) | BFD median (μs) |
|---|---|---|---|---|
| 69 to 99 | 333 | 44.4 | 55.5 | 66.6 |
| 100 to 119 | 777 | 88.8 | 99.9 | 11.1 |
| 120 to 139 | 222 | 33.3 | 44.4 | 55.5 |
| 140 to 159 | 666 | 77.7 | 88.8 | 99.9 |
| 160 to 199 | 111 | 22.2 | 33.3 | 44.4 |
| 200 to 476 | 555 | 66.6 | 77.7 | 88.8 |

## 📊 Spread inside each band

<!--
Table: how far BFD v2 time spreads inside each band.
Reads: lib/results/benchmarks/baseline/algorithms/Full_BFD_Packing.md - the v2 rows.
Item counts from the inputs, as above.
Rows: the same bands.
Columns: Items, Problems (count), Fastest, Median, Slowest - in μs; then Slowest as × of fastest.
Notes: it shows size alone cannot predict time.
-->

| Items | Problems | Fastest (μs) | Median (μs) | Slowest (μs) | Slowest as × fastest |
|---|---|---|---|---|---|
| 69 to 99 | 999 | 11.1 | 22.2 | 33.3 | 4.44× |
| 100 to 119 | 555 | 66.6 | 77.7 | 88.8 | 9.99× |
| 120 to 139 | 111 | 22.2 | 33.3 | 44.4 | 5.55× |
| 140 to 159 | 666 | 77.7 | 88.8 | 99.9 | 1.11× |
| 160 to 199 | 222 | 33.3 | 44.4 | 55.5 | 6.66× |
| 200 to 476 | 777 | 88.8 | 99.9 | 11.1 | 2.22× |

## Gaps and open questions

<!--
Question, under the spread table: if size alone cannot predict time, what does?
Open: do FFD and WFD spread differently inside a band? If they do, each gets a spread table like BFD's.
-->
