```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | ScenarioName      | Items | Widths  | Mean         | Error      | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |------------------ |------:|-------- |-------------:|-----------:|----------:|------:|-------:|-------:|----------:|------------:|
| **Protobuf**       | **one item**          |     **1** | **8/8/8**   |     **84.18 ns** |   **2.482 ns** |  **0.136 ns** |  **1.00** | **0.0564** |      **-** |     **472 B** |        **1.00** |
| ViPaq_Row      | one item          |     1 | 8/8/8   |     84.17 ns |  15.525 ns |  0.851 ns |  1.00 | 0.0362 |      - |     304 B |        0.64 |
| ViPaq_Columnar | one item          |     1 | 8/8/8   |     83.00 ns |   3.906 ns |  0.214 ns |  0.99 | 0.0362 |      - |     304 B |        0.64 |
|                |                   |       |         |              |            |           |       |        |        |           |             |
| **Protobuf**       | **typical container** |    **70** | **16/8/16** |  **1,852.10 ns** | **182.718 ns** | **10.015 ns** |  **1.00** | **0.8068** | **0.0172** |    **6760 B** |        **1.00** |
| ViPaq_Row      | typical container |    70 | 16/8/16 |  2,439.11 ns |  63.653 ns |  3.489 ns |  1.32 | 0.5875 | 0.0038 |    4936 B |        0.73 |
| ViPaq_Columnar | typical container |    70 | 16/8/16 |  2,504.66 ns | 412.051 ns | 22.586 ns |  1.35 | 0.5875 | 0.0038 |    4936 B |        0.73 |
|                |                   |       |         |              |            |           |       |        |        |           |             |
| **Protobuf**       | **largest FFD pack**  |   **365** | **16/8/16** | **10,423.12 ns** | **632.384 ns** | **34.663 ns** |  **1.00** | **3.7537** | **0.3052** |   **31400 B** |        **1.00** |
| ViPaq_Row      | largest FFD pack  |   365 | 16/8/16 | 12,279.79 ns | 426.167 ns | 23.360 ns |  1.18 | 2.9449 | 0.1373 |   24705 B |        0.79 |
| ViPaq_Columnar | largest FFD pack  |   365 | 16/8/16 | 12,675.39 ns | 562.690 ns | 30.843 ns |  1.22 | 2.9449 | 0.1373 |   24705 B |        0.79 |
