# Result selection

What does picking the result cost, and is v2 cheaper than v1 at it?

> Every number in this file is fake. The tables show the shape only.

## 📊 What a pick costs

<!--
Table: the cost of each selector, v2 against v1.
Reads: lib/results/benchmarks/baseline/result-selection/<BestBin|SmallestBin|BestAlgorithm>.md
Rows: Best bin, Smallest bin, Best algorithm.
Columns: Selector, Scenarios (count), Slowest v2 pick (ns), Time v2 against v1, Memory v2 against v1 - the last
two × of v1.
Notes: the last two are the v2 row's Ratio and Alloc Ratio, then the mean over the selector's scenarios. An average
above 1.00× is bold. Where one selector's scenarios differ widely, the words under the table say why.
The pick time holds on one machine and runtime; the file names them, read from the report header.
-->

| Selector | Scenarios | Slowest v2 pick (ns) | Time v2 against v1 | Memory v2 against v1 |
|---|---|---|---|---|
| Best bin | 999 | 111 | 0.22× | 0.33× |
| Smallest bin | 444 | 555 | 0.66× | 0.77× |
| Best algorithm | 888 | 999 | **1.11×** | 0.22× |

## Gaps and open questions

<!--
None set for this file yet.
-->
