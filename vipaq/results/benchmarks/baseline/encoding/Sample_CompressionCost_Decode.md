```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method  | ScenarioName         | Items | Widths  | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------- |--------------------- |------:|-------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **NoOp**    | **compression low win**  |    **70** | **16/8/16** | **2.420 μs** | **0.0318 μs** | **0.0266 μs** |  **1.00** |    **0.01** | **0.5875** | **0.0038** |   **4.82 KB** |        **1.00** |
| Deflate | compression low win  |    70 | 16/8/16 | 4.248 μs | 0.0308 μs | 0.0273 μs |  1.76 |    0.02 | 0.7095 |      - |   5.84 KB |        1.21 |
| Gzip    | compression low win  |    70 | 16/8/16 | 4.389 μs | 0.0529 μs | 0.0495 μs |  1.81 |    0.03 | 0.7172 |      - |    5.9 KB |        1.22 |
|         |                      |       |         |          |           |           |       |         |        |        |           |             |
| **NoOp**    | **compression high win** |   **108** | **16/8/16** | **3.680 μs** | **0.0196 μs** | **0.0164 μs** |  **1.00** |    **0.01** | **0.8926** | **0.0229** |   **7.31 KB** |        **1.00** |
| Deflate | compression high win |   108 | 16/8/16 | 5.482 μs | 0.0260 μs | 0.0244 μs |  1.49 |    0.01 | 1.0147 | 0.0076 |   8.34 KB |        1.14 |
| Gzip    | compression high win |   108 | 16/8/16 | 5.685 μs | 0.0519 μs | 0.0460 μs |  1.54 |    0.01 | 1.0223 | 0.0076 |   8.41 KB |        1.15 |
