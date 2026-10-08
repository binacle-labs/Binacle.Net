# Encode and decode cost

What encoding and decoding a ViPaq token costs in time and memory against protobuf, and what compressing it
adds. Each case is hand-made; the tables show every case, not an average.

## 📊 Encode, against protobuf

Each format's mean time and allocated memory to encode the case, divided by protobuf's on the same case.
Below 1 means cheaper than protobuf.

| Case | Items | Widths | Protobuf time (ns) | ViPaq row time | ViPaq columnar time | JSON time | ViPaq row memory | ViPaq columnar memory | JSON memory |
|---|---|---|---|---|---|---|---|---|---|
| one item | 1 | 8/8/8 | 69 | 1.89 | 2.37 | 3.32 | 1.00 | 1.38 | 1.73 |
| small, all 16-bit | 4 | 16/16/16 | 156 | 1.73 | 2.03 | 3.09 | 0.97 | 1.23 | 1.90 |
| small, 8-bit | 16 | 8/8/8 | 368 | 1.94 | 2.17 | 3.95 | 0.53 | 0.63 | 1.86 |
| 100 cubes, 8-bit | 100 | 8/8/8 | 1,822 | 2.13 | 2.34 | 4.20 | 0.35 | 0.37 | 1.59 |
| typical container | 70 | 16/8/16 | 1,601 | 1.94 | 2.14 | 3.82 | 0.46 | 0.48 | 1.53 |
| largest FFD pack | 365 | 16/8/16 | 7,813 | 1.94 | 2.13 | 3.92 | 0.46 | 0.46 | 1.60 |
| 1000 items, 8-bit | 1000 | 8/8/8 | 33,987 | 1.09 | 1.30 | 3.14 | 0.31 | 0.31 | 1.66 |
| 5000 items, 8-bit | 5000 | 8/8/8 | 229,658 | 1.21 ⚠ | 0.96 | 2.19 | 0.28 | 0.28 | 1.51 |
| 65535 items, 8-bit | 65535 | 8/8/8 | 5,313,824 | 0.48 | 0.64 | 1.65 | 0.31 | 0.31 | 1.66 |
| 1000 items, 16-bit | 1000 | 16/16/16 | 45,687 | 0.96 | 1.12 | 2.34 | 0.53 | 0.53 | 1.68 |
| 5000 items, 16-bit | 5000 | 16/16/16 | 287,663 | 0.77 | 0.88 | 1.64 | 0.49 | 0.49 | 1.54 |
| 65535 items, 16-bit | 65535 | 16/16/16 | 5,553,120 | 0.59 | 0.75 | 1.42 | 0.53 | 0.53 | 1.68 |

⚠ Probably one slow process. The bench README records an earlier run of this case at 0.80, with no code
change between the two.

## 📊 Decode, against protobuf

Each format's mean time and allocated memory to decode the case, divided by protobuf's on the same case.
Below 1 means cheaper than protobuf. JSON has no decode in the test encoder, so it is not here.

| Case | Items | Widths | Protobuf time (ns) | ViPaq row time | ViPaq columnar time | ViPaq row memory | ViPaq columnar memory |
|---|---|---|---|---|---|---|---|
| one item | 1 | 8/8/8 | 89 | 0.94 | 0.95 | 0.64 | 0.64 |
| small, all 16-bit | 4 | 16/16/16 | 155 | 1.42 | 1.38 | 0.83 | 0.83 |
| small, 8-bit | 16 | 8/8/8 | 415 | 1.07 | 1.08 | 0.77 | 0.77 |
| 100 cubes, 8-bit | 100 | 8/8/8 | 2,550 | 0.94 | 0.98 | 0.71 | 0.71 |
| typical container | 70 | 16/8/16 | 1,999 | 1.21 | 1.27 | 0.73 | 0.73 |
| largest FFD pack | 365 | 16/8/16 | 10,938 | 1.12 | 1.15 | 0.79 | 0.79 |
| 1000 items, 8-bit | 1000 | 8/8/8 | 30,421 | 0.77 | 0.78 | 0.71 | 0.71 |
| 5000 items, 8-bit | 5000 | 8/8/8 | 211,932 | 0.56 | 0.56 | 0.64 | 0.64 |
| 65535 items, 8-bit | 65535 | 8/8/8 | 4,631,843 | 0.60 | 0.63 | 0.72 | 0.72 |
| 1000 items, 16-bit | 1000 | 16/16/16 | 42,692 | 0.90 | 0.88 | 0.85 | 0.85 |
| 5000 items, 16-bit | 5000 | 16/16/16 | 273,779 | 0.68 | 0.69 | 0.77 | 0.77 |
| 65535 items, 16-bit | 65535 | 16/16/16 | 4,697,748 | 0.78 | 0.83 | 0.86 | 0.86 |

## 📊 What compressing adds

Mean time and allocated memory with deflate or gzip, divided by the same encode or decode with no codec.
Row layout only: the bench has no columnar run.

| Case | Items | Encode, deflate | Encode, gzip | Decode, deflate | Decode, gzip | Encode memory, deflate | Encode memory, gzip | Decode memory, deflate | Decode memory, gzip |
|---|---|---|---|---|---|---|---|---|---|
| compression low win | 70 | 2.67 | 2.78 | 1.76 | 1.81 | 0.97 | 1.13 | 1.21 | 1.22 |
| compression high win | 108 | 2.79 | 2.82 | 1.49 | 1.54 | 0.85 | 0.96 | 1.14 | 1.15 |

## ⚠️ What will bite you

- **The smoke run disagrees with this one on the same case.** ViPaq row encode on "one item" is 1.63x protobuf
  there and 1.89x here, both at the default job. Every number in this file is from the sample run.
- **One process can run slow, and a report cannot show it.** Read a surprising row as possibly one slow process.

## 📂 Where the numbers come from

The `Sample_*` reports in [benchmarks/baseline/encoding/](benchmarks/baseline/encoding), default job,
BenchmarkDotNet on an AMD Ryzen 9 9900X, .NET 10.0.12. Every ratio is worked out from the Mean and Allocated
columns.
