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
| **Protobuf**       | **one item**          |     **72.57 ns** |     **7.274 ns** |   **0.399 ns** |  **1.00** |    **0.01** | **0.0497** |      **-** |     **416 B** |        **1.00** |
| ViPaq_Row      | one item          |    135.67 ns |    66.544 ns |   3.648 ns |  1.87 |    0.04 | 0.0496 |      - |     416 B |        1.00 |
| ViPaq_Columnar | one item          |    162.80 ns |    26.151 ns |   1.433 ns |  2.24 |    0.02 | 0.0687 |      - |     576 B |        1.38 |
|                |                   |              |              |            |       |         |        |        |           |             |
| **Protobuf**       | **typical container** |  **1,657.42 ns** |   **107.765 ns** |   **5.907 ns** |  **1.00** |    **0.00** | **0.9155** | **0.0095** |    **7664 B** |        **1.00** |
| ViPaq_Row      | typical container |  3,181.04 ns |   294.611 ns |  16.149 ns |  1.92 |    0.01 | 0.4196 |      - |    3536 B |        0.46 |
| ViPaq_Columnar | typical container |  3,474.58 ns |   326.182 ns |  17.879 ns |  2.10 |    0.01 | 0.4387 |      - |    3696 B |        0.48 |
|                |                   |              |              |            |       |         |        |        |           |             |
| **Protobuf**       | **largest real pack** |  **7,599.38 ns** | **1,327.477 ns** |  **72.764 ns** |  **1.00** |    **0.01** | **4.3716** | **0.2518** |   **36592 B** |        **1.00** |
| ViPaq_Row      | largest real pack | 15,448.77 ns | 2,899.083 ns | 158.909 ns |  2.03 |    0.02 | 1.9836 |      - |   16816 B |        0.46 |
| ViPaq_Columnar | largest real pack | 17,062.15 ns | 1,138.993 ns |  62.432 ns |  2.25 |    0.02 | 2.0142 |      - |   16976 B |        0.46 |
