# Scaling

How does v2 packing time grow at the small end, from 3 to 67 items?

> Every number in this file is fake. The tables show the shape only.

## 📊 v2 packing time up the item ladder

<!--
Table: v2 packing time per step of the ladder.
Reads: lib/results/benchmarks/baseline/scaling/Sample_Packing.md - the FFD_v2, WFD_v2 and BFD_v2 rows.
Rows: 3, 13, 29, 47, 67 items.
Columns: Items, Item types, FFD, WFD, BFD - in μs.
Notes: item types (1, 3, 6, 8, 10) are fixed by lib/test/Binacle.Lib.Testing/LadderGenerator.cs; the report has no
types column. 79 items is left out, with a note: FFD and WFD no longer fit every item there and stop early.
Times hold on one machine and runtime (default job); the file names them, read from the report header.
-->

| Items | Item types | FFD (μs) | WFD (μs) | BFD (μs) |
|---|---|---|---|---|
| 3 | 1 | 33.3 | 44.4 | 55.5 |
| 13 | 3 | 66.6 | 77.7 | 88.8 |
| 29 | 6 | 99.9 | 11.1 | 22.2 |
| 47 | 8 | 33.3 | 44.4 | 55.5 |
| 67 | 10 | 66.6 | 77.7 | 88.8 |

## Gaps and open questions

<!--
Gap: each step adds items and an item type at once, so the ladder cannot separate the two.
-->
