```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method  | ScenarioName         | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| **NoOp**    | **compression low win**  | **2.462 μs** | **0.0252 μs** | **0.0223 μs** |  **1.00** |    **0.01** | **0.5875** | **0.0038** |   **4.82 KB** |        **1.00** |
| Deflate | compression low win  | 4.292 μs | 0.0387 μs | 0.0362 μs |  1.74 |    0.02 | 0.7095 |      - |   5.84 KB |        1.21 |
| Gzip    | compression low win  | 4.492 μs | 0.0301 μs | 0.0282 μs |  1.82 |    0.02 | 0.7172 |      - |    5.9 KB |        1.22 |
|         |                      |          |           |           |       |         |        |        |           |             |
| **NoOp**    | **compression high win** | **3.717 μs** | **0.0149 μs** | **0.0132 μs** |  **1.00** |    **0.00** | **0.8926** | **0.0229** |   **7.31 KB** |        **1.00** |
| Deflate | compression high win | 5.617 μs | 0.0505 μs | 0.0447 μs |  1.51 |    0.01 | 1.0147 | 0.0076 |   8.34 KB |        1.14 |
| Gzip    | compression high win | 5.822 μs | 0.0495 μs | 0.0439 μs |  1.57 |    0.01 | 1.0223 | 0.0076 |   8.41 KB |        1.15 |
