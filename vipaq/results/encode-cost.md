# Encode cost

What does a ViPaq encode cost in time and memory against protobuf, and what does compressing add?

> Every number in this file is fake. The tables show the shape only.

## 📊 Time against protobuf

<!--
Table: encode time as × of protobuf, per bench pack.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_Encode.md
Rows: one per pack, fewest items to most - the real packs `one item`, `small, all 16-bit`, `small, 8-bit`,
`typical container`, `100 cubes, 8-bit`, `largest FFD pack`, then the synthetic `<count> items, <width>-bit` packs.
Columns: Pack, Kind (real or synthetic), Items, Widths, ViPaq row, ViPaq columnar, JSON - each × of protobuf.
Notes: BenchmarkDotNet's own Ratio; Error and RatioSD dropped. Above 1.00× is bold. Items and Widths are read from
the report by header. The report has no Kind; it comes from the pack name.
Cost only: no size column, even for the same pack.
-->

| Pack | Kind | Items | Widths | ViPaq row | ViPaq columnar | JSON |
|---|---|---|---|---|---|---|
| one item | real | 1 | 8/8/8 | **1.99×** | **1.11×** | **1.22×** |
| small, all 16-bit | real | 4 | 16/16/16 | **1.33×** | **1.44×** | **1.55×** |
| small, 8-bit | real | 16 | 8/8/8 | **1.66×** | **1.77×** | **1.88×** |
| typical container | real | 70 | 16/8/16 | **1.99×** | **1.11×** | **1.22×** |
| 100 cubes, 8-bit | real | 100 | 8/8/8 | **1.33×** | **1.44×** | **1.55×** |
| largest FFD pack | real | 365 | 16/8/16 | **1.66×** | **1.77×** | **1.88×** |
| 1000 items, 8-bit | synthetic | 1000 | 8/8/8 | 0.99× | 0.11× | **2.22×** |
| 1000 items, 16-bit | synthetic | 1000 | 16/16/16 | 0.33× | 0.44× | **5.55×** |
| 5000 items, 8-bit | synthetic | 5000 | 8/8/8 | 0.66× | 0.77× | **8.88×** |
| 5000 items, 16-bit | synthetic | 5000 | 16/16/16 | 0.99× | 0.11× | **2.22×** |
| 65535 items, 8-bit | synthetic | 65535 | 8/8/8 | 0.33× | 0.44× | **5.55×** |
| 65535 items, 16-bit | synthetic | 65535 | 16/16/16 | 0.66× | 0.77× | **8.88×** |

## 📊 Memory against protobuf

<!--
Table: encode memory as × of protobuf, per bench pack.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_Encode.md
Rows: the same packs, in the same order.
Columns: Pack, Kind (real or synthetic), Items, Widths, ViPaq row, ViPaq columnar, JSON - each × of protobuf.
Notes: BenchmarkDotNet's own Alloc Ratio. Above 1.00× is bold.
-->

| Pack | Kind | Items | Widths | ViPaq row | ViPaq columnar | JSON |
|---|---|---|---|---|---|---|
| one item | real | 1 | 8/8/8 | **1.99×** | **1.11×** | **1.22×** |
| small, all 16-bit | real | 4 | 16/16/16 | **1.33×** | **1.44×** | **1.55×** |
| small, 8-bit | real | 16 | 8/8/8 | **1.66×** | **1.77×** | **1.88×** |
| typical container | real | 70 | 16/8/16 | **1.99×** | **1.11×** | **1.22×** |
| 100 cubes, 8-bit | real | 100 | 8/8/8 | **1.33×** | **1.44×** | **1.55×** |
| largest FFD pack | real | 365 | 16/8/16 | **1.66×** | **1.77×** | **1.88×** |
| 1000 items, 8-bit | synthetic | 1000 | 8/8/8 | 0.99× | 0.11× | **2.22×** |
| 1000 items, 16-bit | synthetic | 1000 | 16/16/16 | 0.33× | 0.44× | **5.55×** |
| 5000 items, 8-bit | synthetic | 5000 | 8/8/8 | 0.66× | 0.77× | **8.88×** |
| 5000 items, 16-bit | synthetic | 5000 | 16/16/16 | 0.99× | 0.11× | **2.22×** |
| 65535 items, 8-bit | synthetic | 65535 | 8/8/8 | 0.33× | 0.44× | **5.55×** |
| 65535 items, 16-bit | synthetic | 65535 | 16/16/16 | 0.66× | 0.77× | **8.88×** |

## 📊 What compressing adds

<!--
Table: what deflate and gzip add to one encode.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_CompressionCost_Encode.md
Rows: `compression low win`, `compression high win` - two real FFD packs, row layout, named as the report prints them.
Columns: Pack, Items, Widths, Deflate time, Gzip time, Deflate memory, Gzip memory - each × of no compression
(the NoOp row).
Notes: BenchmarkDotNet's own Ratio (time) and Alloc Ratio (memory). Above 1.00× is bold.
-->

| Pack | Items | Widths | Deflate time | Gzip time | Deflate memory | Gzip memory |
|---|---|---|---|---|---|---|
| compression low win | 70 | 16/8/16 | **1.99×** | **1.11×** | **1.22×** | **1.33×** |
| compression high win | 108 | 16/8/16 | **1.44×** | **1.55×** | **1.66×** | **1.77×** |

## Gaps and open questions

<!--
Question, under the compression table: two packs are not a curve. Is it worth measuring compression cost across pack
sizes, so the encoder can decide per pack whether to compress?
Gap: each case runs in one process, so a row can come from a slow or a fast process, and the report cannot show it.
-->
