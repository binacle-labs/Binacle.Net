# Decode cost

What does a ViPaq decode cost in time and memory against protobuf, and what does decompressing add?

## 📝 What was observed

- Decode is where ViPaq is strong. It beats protobuf on most packs and never costs more than 1.42×.
- It always allocates less, 0.64× to 0.86× protobuf.
- Row and columnar decode alike, within a few per cent. The layout only matters when encoding.
- The 16-bit widths are the dearest case, in time and in memory.
- Decompressing adds about half again to three quarters again on time, and about a fifth on memory.

## 📊 Time against protobuf

<!--
Decode time as × of protobuf, per bench pack. BenchmarkDotNet's own Ratio. No JSON column: the test JSON encoder
has no decoder, so the report has no JSON row.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_Decode.md
-->

| Pack | Kind | Items | Widths | ViPaq row | ViPaq columnar |
|---|---|---|---|---|---|
| one item | real | 1 | 8/8/8 | 0.94× | 0.95× |
| small, all 16-bit | real | 4 | 16/16/16 | 1.42× | 1.38× |
| small, 8-bit | real | 16 | 8/8/8 | 1.07× | 1.08× |
| typical container | real | 70 | 16/8/16 | 1.21× | 1.27× |
| 100 cubes, 8-bit | real | 100 | 8/8/8 | 0.94× | 0.98× |
| largest FFD pack | real | 365 | 16/8/16 | 1.12× | 1.15× |
| 1000 items, 8-bit | synthetic | 1000 | 8/8/8 | 0.77× | 0.78× |
| 1000 items, 16-bit | synthetic | 1000 | 16/16/16 | 0.90× | 0.88× |
| 5000 items, 8-bit | synthetic | 5000 | 8/8/8 | 0.56× | 0.56× |
| 5000 items, 16-bit | synthetic | 5000 | 16/16/16 | 0.68× | 0.69× |
| 65535 items, 8-bit | synthetic | 65535 | 8/8/8 | 0.60× | 0.63× |
| 65535 items, 16-bit | synthetic | 65535 | 16/16/16 | 0.78× | 0.83× |

## 📊 Memory against protobuf

<!--
Decode memory as × of protobuf, per bench pack. BenchmarkDotNet's own Alloc Ratio.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_Decode.md
-->

| Pack | Kind | Items | Widths | ViPaq row | ViPaq columnar |
|---|---|---|---|---|---|
| one item | real | 1 | 8/8/8 | 0.64× | 0.64× |
| small, all 16-bit | real | 4 | 16/16/16 | 0.83× | 0.83× |
| small, 8-bit | real | 16 | 8/8/8 | 0.77× | 0.77× |
| typical container | real | 70 | 16/8/16 | 0.73× | 0.73× |
| 100 cubes, 8-bit | real | 100 | 8/8/8 | 0.71× | 0.71× |
| largest FFD pack | real | 365 | 16/8/16 | 0.79× | 0.79× |
| 1000 items, 8-bit | synthetic | 1000 | 8/8/8 | 0.71× | 0.71× |
| 1000 items, 16-bit | synthetic | 1000 | 16/16/16 | 0.85× | 0.85× |
| 5000 items, 8-bit | synthetic | 5000 | 8/8/8 | 0.64× | 0.64× |
| 5000 items, 16-bit | synthetic | 5000 | 16/16/16 | 0.77× | 0.77× |
| 65535 items, 8-bit | synthetic | 65535 | 8/8/8 | 0.72× | 0.72× |
| 65535 items, 16-bit | synthetic | 65535 | 16/16/16 | 0.86× | 0.86× |

## 📊 What decompressing adds

<!--
What deflate and gzip add to one ViPaq decode, as × of the same decode uncompressed. The baseline is the NoOp
row, so every ratio here is ViPaq against itself - no protobuf is in this class. Ratio and Alloc Ratio.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_CompressionCost_Decode.md
-->

| Pack | Items | Widths | Deflate time | Gzip time | Deflate memory | Gzip memory |
|---|---|---|---|---|---|---|
| compression low win | 70 | 16/8/16 | 1.76× | 1.81× | 1.21× | 1.22× |
| compression high win | 108 | 16/8/16 | 1.49× | 1.54× | 1.14× | 1.15× |
