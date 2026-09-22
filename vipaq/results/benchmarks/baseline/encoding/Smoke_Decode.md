```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | ScenarioName      | Mean         | Error      | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |------------------ |-------------:|-----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **Protobuf**       | **one item**          |     **88.30 ns** |  **25.281 ns** |  **1.386 ns** |  **1.00** |    **0.02** | **0.0564** |      **-** |     **472 B** |        **1.00** |
| ViPaq_Row      | one item          |     85.48 ns |  10.815 ns |  0.593 ns |  0.97 |    0.01 | 0.0362 |      - |     304 B |        0.64 |
| ViPaq_Columnar | one item          |     84.78 ns |   1.250 ns |  0.069 ns |  0.96 |    0.01 | 0.0362 |      - |     304 B |        0.64 |
|                |                   |              |            |           |       |         |        |        |           |             |
| **Protobuf**       | **typical container** |  **2,046.22 ns** | **355.583 ns** | **19.491 ns** |  **1.00** |    **0.01** | **0.8049** | **0.0153** |    **6760 B** |        **1.00** |
| ViPaq_Row      | typical container |  2,438.59 ns | 568.991 ns | 31.188 ns |  1.19 |    0.02 | 0.5875 | 0.0038 |    4936 B |        0.73 |
| ViPaq_Columnar | typical container |  2,569.58 ns | 280.185 ns | 15.358 ns |  1.26 |    0.01 | 0.5875 | 0.0038 |    4936 B |        0.73 |
|                |                   |              |            |           |       |         |        |        |           |             |
| **Protobuf**       | **largest real pack** | **10,947.62 ns** | **998.621 ns** | **54.738 ns** |  **1.00** |    **0.01** | **3.7537** | **0.3052** |   **31400 B** |        **1.00** |
| ViPaq_Row      | largest real pack | 12,385.08 ns | 594.318 ns | 32.577 ns |  1.13 |    0.01 | 2.9449 | 0.1373 |   24705 B |        0.79 |
| ViPaq_Columnar | largest real pack | 12,154.21 ns | 781.094 ns | 42.814 ns |  1.11 |    0.01 | 2.9449 | 0.1373 |   24705 B |        0.79 |
