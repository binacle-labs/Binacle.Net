```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | ScenarioName      | Mean         | Error        | StdDev     | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |------------------ |-------------:|-------------:|-----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **Protobuf**       | **one item**          |     **65.46 ns** |     **2.047 ns** |   **0.112 ns** |  **1.00** |    **0.00** | **0.0497** |      **-** |     **416 B** |        **1.00** |
| ViPaq_Row      | one item          |    131.75 ns |     6.757 ns |   0.370 ns |  2.01 |    0.01 | 0.0496 |      - |     416 B |        1.00 |
| ViPaq_Columnar | one item          |    160.18 ns |     7.372 ns |   0.404 ns |  2.45 |    0.01 | 0.0687 |      - |     576 B |        1.38 |
| Json           | one item          |    236.88 ns |    94.765 ns |   5.194 ns |  3.62 |    0.07 | 0.0861 |      - |     720 B |        1.73 |
|                |                   |              |              |            |       |         |        |        |           |             |
| **Protobuf**       | **typical container** |  **1,642.25 ns** |    **42.245 ns** |   **2.316 ns** |  **1.00** |    **0.00** | **0.9155** | **0.0095** |    **7664 B** |        **1.00** |
| ViPaq_Row      | typical container |  3,110.93 ns |   219.698 ns |  12.042 ns |  1.89 |    0.01 | 0.4196 |      - |    3536 B |        0.46 |
| ViPaq_Columnar | typical container |  3,450.47 ns |    34.915 ns |   1.914 ns |  2.10 |    0.00 | 0.4387 |      - |    3696 B |        0.48 |
| Json           | typical container |  6,171.76 ns |   575.482 ns |  31.544 ns |  3.76 |    0.02 | 1.3962 |      - |   11688 B |        1.53 |
|                |                   |              |              |            |       |         |        |        |           |             |
| **Protobuf**       | **largest real pack** |  **7,442.45 ns** |   **176.464 ns** |   **9.673 ns** |  **1.00** |    **0.00** | **4.3716** | **0.2518** |   **36592 B** |        **1.00** |
| ViPaq_Row      | largest real pack | 15,202.16 ns |   856.053 ns |  46.923 ns |  2.04 |    0.01 | 1.9989 | 0.0153 |   16816 B |        0.46 |
| ViPaq_Columnar | largest real pack | 16,841.26 ns |   655.493 ns |  35.930 ns |  2.26 |    0.00 | 2.0142 |      - |   16976 B |        0.46 |
| Json           | largest real pack | 30,807.89 ns | 4,461.826 ns | 244.568 ns |  4.14 |    0.03 | 6.9580 | 0.1831 |   58400 B |        1.60 |
