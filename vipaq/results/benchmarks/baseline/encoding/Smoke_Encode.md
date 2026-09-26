```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | ScenarioName      | Items | Widths  | Mean         | Error        | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |------------------ |------:|-------- |-------------:|-------------:|----------:|------:|-------:|-------:|----------:|------------:|
| **Protobuf**       | **one item**          |     **1** | **8/8/8**   |     **66.56 ns** |     **1.504 ns** |  **0.082 ns** |  **1.00** | **0.0497** |      **-** |     **416 B** |        **1.00** |
| ViPaq_Row      | one item          |     1 | 8/8/8   |    131.82 ns |     3.688 ns |  0.202 ns |  1.98 | 0.0496 |      - |     416 B |        1.00 |
| ViPaq_Columnar | one item          |     1 | 8/8/8   |    159.80 ns |    12.556 ns |  0.688 ns |  2.40 | 0.0687 |      - |     576 B |        1.38 |
| Json           | one item          |     1 | 8/8/8   |    229.95 ns |    10.785 ns |  0.591 ns |  3.45 | 0.0861 |      - |     720 B |        1.73 |
|                |                   |       |         |              |              |           |       |        |        |           |             |
| **Protobuf**       | **typical container** |    **70** | **16/8/16** |  **1,635.99 ns** |    **18.522 ns** |  **1.015 ns** |  **1.00** | **0.9155** | **0.0095** |    **7664 B** |        **1.00** |
| ViPaq_Row      | typical container |    70 | 16/8/16 |  3,098.21 ns |   107.349 ns |  5.884 ns |  1.89 | 0.4196 |      - |    3536 B |        0.46 |
| ViPaq_Columnar | typical container |    70 | 16/8/16 |  3,440.72 ns |   122.691 ns |  6.725 ns |  2.10 | 0.4387 |      - |    3696 B |        0.48 |
| Json           | typical container |    70 | 16/8/16 |  6,070.46 ns |   423.021 ns | 23.187 ns |  3.71 | 1.3962 |      - |   11688 B |        1.53 |
|                |                   |       |         |              |              |           |       |        |        |           |             |
| **Protobuf**       | **largest FFD pack**  |   **365** | **16/8/16** |  **7,379.10 ns** |   **492.090 ns** | **26.973 ns** |  **1.00** | **4.3716** | **0.2518** |   **36592 B** |        **1.00** |
| ViPaq_Row      | largest FFD pack  |   365 | 16/8/16 | 15,161.64 ns | 1,016.172 ns | 55.700 ns |  2.05 | 1.9836 |      - |   16816 B |        0.46 |
| ViPaq_Columnar | largest FFD pack  |   365 | 16/8/16 | 16,621.32 ns |   426.013 ns | 23.351 ns |  2.25 | 2.0142 |      - |   16976 B |        0.46 |
| Json           | largest FFD pack  |   365 | 16/8/16 | 30,311.15 ns |   457.436 ns | 25.074 ns |  4.11 | 6.9580 | 0.1831 |   58400 B |        1.60 |
