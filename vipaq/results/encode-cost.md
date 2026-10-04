# Encode cost

What does a ViPaq encode cost in time and memory against protobuf, and what does compressing add?

## 📝 What was observed

- ViPaq trades time for memory. On the real packs it takes about 2× protobuf's time and under half its memory.
- Row is faster than columnar on every pack.
- Above about a thousand items ViPaq turns faster than protobuf too, down to 0.48× at the top of the range.
- JSON is the slowest everywhere, 1.42× to 4.20× protobuf, and the only format that allocates more.
- Compressing roughly triples the encode and costs no extra memory.

## 📊 Time against protobuf

<!--
Encode time as × of protobuf, per bench pack. BenchmarkDotNet's own Ratio.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_Encode.md
-->

| Pack | Kind | Items | Widths | ViPaq row | ViPaq columnar | JSON |
|---|---|---|---|---|---|---|
| one item | real | 1 | 8/8/8 | 1.89× | 2.37× | 3.32× |
| small, all 16-bit | real | 4 | 16/16/16 | 1.73× | 2.03× | 3.09× |
| small, 8-bit | real | 16 | 8/8/8 | 1.94× | 2.17× | 3.96× |
| typical container | real | 70 | 16/8/16 | 1.94× | 2.14× | 3.82× |
| 100 cubes, 8-bit | real | 100 | 8/8/8 | 2.13× | 2.34× | 4.20× |
| largest FFD pack | real | 365 | 16/8/16 | 1.94× | 2.13× | 3.92× |
| 1000 items, 8-bit | synthetic | 1000 | 8/8/8 | 1.09× | 1.30× | 3.14× |
| 1000 items, 16-bit | synthetic | 1000 | 16/16/16 | 0.96× | 1.12× | 2.34× |
| 5000 items, 8-bit | synthetic | 5000 | 8/8/8 | 1.21× | 0.96× | 2.19× |
| 5000 items, 16-bit | synthetic | 5000 | 16/16/16 | 0.77× | 0.88× | 1.64× |
| 65535 items, 8-bit | synthetic | 65535 | 8/8/8 | 0.48× | 0.64× | 1.65× |
| 65535 items, 16-bit | synthetic | 65535 | 16/16/16 | 0.59× | 0.75× | 1.42× |

## 📊 Memory against protobuf

<!--
Encode memory as × of protobuf, per bench pack. BenchmarkDotNet's own Alloc Ratio.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_Encode.md
-->

| Pack | Kind | Items | Widths | ViPaq row | ViPaq columnar | JSON |
|---|---|---|---|---|---|---|
| one item | real | 1 | 8/8/8 | 1.00× | 1.38× | 1.73× |
| small, all 16-bit | real | 4 | 16/16/16 | 0.97× | 1.23× | 1.90× |
| small, 8-bit | real | 16 | 8/8/8 | 0.53× | 0.63× | 1.86× |
| typical container | real | 70 | 16/8/16 | 0.46× | 0.48× | 1.53× |
| 100 cubes, 8-bit | real | 100 | 8/8/8 | 0.35× | 0.37× | 1.59× |
| largest FFD pack | real | 365 | 16/8/16 | 0.46× | 0.46× | 1.60× |
| 1000 items, 8-bit | synthetic | 1000 | 8/8/8 | 0.31× | 0.31× | 1.66× |
| 1000 items, 16-bit | synthetic | 1000 | 16/16/16 | 0.53× | 0.53× | 1.68× |
| 5000 items, 8-bit | synthetic | 5000 | 8/8/8 | 0.28× | 0.28× | 1.51× |
| 5000 items, 16-bit | synthetic | 5000 | 16/16/16 | 0.49× | 0.49× | 1.54× |
| 65535 items, 8-bit | synthetic | 65535 | 8/8/8 | 0.31× | 0.31× | 1.66× |
| 65535 items, 16-bit | synthetic | 65535 | 16/16/16 | 0.53× | 0.53× | 1.68× |

## 📊 What compressing adds

<!--
What deflate and gzip add to one ViPaq encode, as × of the same encode uncompressed. The baseline is the NoOp
row, so every ratio here is ViPaq against itself - no protobuf is in this class. Ratio and Alloc Ratio.
Reads: vipaq/results/benchmarks/baseline/encoding/Sample_CompressionCost_Encode.md
-->

| Pack | Items | Widths | Deflate time | Gzip time | Deflate memory | Gzip memory |
|---|---|---|---|---|---|---|
| compression low win | 70 | 16/8/16 | 2.67× | 2.78× | 0.97× | 1.13× |
| compression high win | 108 | 16/8/16 | 2.79× | 2.82× | 0.85× | 0.96× |
