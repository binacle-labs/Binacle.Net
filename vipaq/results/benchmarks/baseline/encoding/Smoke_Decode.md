```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  default : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=default  

```
| Method         | ScenarioName      | Items | Widths  | Mean         | Error      | StdDev     | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |------------------ |------:|-------- |-------------:|-----------:|-----------:|------:|-------:|-------:|----------:|------------:|
| **Protobuf**       | **one item**          |     **1** | **8/8/8**   |     **88.95 ns** |   **0.552 ns** |   **0.516 ns** |  **1.00** | **0.0564** |      **-** |     **472 B** |        **1.00** |
| ViPaq_Row      | one item          |     1 | 8/8/8   |     83.76 ns |   0.251 ns |   0.196 ns |  0.94 | 0.0362 |      - |     304 B |        0.64 |
| ViPaq_Columnar | one item          |     1 | 8/8/8   |     83.77 ns |   0.377 ns |   0.334 ns |  0.94 | 0.0362 |      - |     304 B |        0.64 |
|                |                   |       |         |              |            |            |       |        |        |           |             |
| **Protobuf**       | **typical container** |    **70** | **16/8/16** |  **1,988.90 ns** |  **11.577 ns** |  **10.829 ns** |  **1.00** | **0.8049** | **0.0153** |    **6760 B** |        **1.00** |
| ViPaq_Row      | typical container |    70 | 16/8/16 |  2,424.21 ns |   9.676 ns |   9.051 ns |  1.22 | 0.5875 | 0.0038 |    4936 B |        0.73 |
| ViPaq_Columnar | typical container |    70 | 16/8/16 |  2,542.54 ns |   8.464 ns |   7.918 ns |  1.28 | 0.5875 | 0.0038 |    4936 B |        0.73 |
|                |                   |       |         |              |            |            |       |        |        |           |             |
| **Protobuf**       | **largest FFD pack**  |   **365** | **16/8/16** | **11,088.67 ns** |  **50.879 ns** |  **47.592 ns** |  **1.00** | **3.7537** | **0.3052** |   **31400 B** |        **1.00** |
| ViPaq_Row      | largest FFD pack  |   365 | 16/8/16 | 12,301.22 ns |  62.160 ns |  58.145 ns |  1.11 | 2.9449 | 0.1373 |   24705 B |        0.79 |
| ViPaq_Columnar | largest FFD pack  |   365 | 16/8/16 | 12,913.26 ns | 161.614 ns | 151.174 ns |  1.16 | 2.9449 | 0.1373 |   24705 B |        0.79 |
