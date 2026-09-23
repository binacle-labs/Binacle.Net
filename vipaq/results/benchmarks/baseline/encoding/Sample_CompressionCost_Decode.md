```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method  | ScenarioName         | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **NoOp**    | **compression low win**  | **2.426 μs** | **0.0210 μs** | **0.0186 μs** |  **1.00** |    **0.01** | **0.5875** | **0.0038** |   **4.82 KB** |        **1.00** |
| Deflate | compression low win  | 4.212 μs | 0.0108 μs | 0.0090 μs |  1.74 |    0.01 | 0.7095 |      - |   5.84 KB |        1.21 |
| Gzip    | compression low win  | 4.505 μs | 0.0738 μs | 0.0690 μs |  1.86 |    0.03 | 0.7172 |      - |    5.9 KB |        1.22 |
|         |                      |          |           |           |       |         |        |        |           |             |
| **NoOp**    | **compression high win** | **3.672 μs** | **0.0161 μs** | **0.0151 μs** |  **1.00** |    **0.01** | **0.8926** | **0.0229** |   **7.31 KB** |        **1.00** |
| Deflate | compression high win | 5.498 μs | 0.0302 μs | 0.0268 μs |  1.50 |    0.01 | 1.0147 | 0.0076 |   8.34 KB |        1.14 |
| Gzip    | compression high win | 5.790 μs | 0.0550 μs | 0.0459 μs |  1.58 |    0.01 | 1.0223 | 0.0076 |   8.41 KB |        1.15 |
