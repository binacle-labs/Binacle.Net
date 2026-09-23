```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method  | ScenarioName         | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------- |--------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **NoOp**    | **compression low win**  |  **3.166 μs** | **0.0265 μs** | **0.0235 μs** |  **1.00** |    **0.01** | **0.4196** |   **3.45 KB** |        **1.00** |
| Deflate | compression low win  |  8.385 μs | 0.0349 μs | 0.0327 μs |  2.65 |    0.02 | 0.3967 |   3.35 KB |        0.97 |
| Gzip    | compression low win  |  8.718 μs | 0.0333 μs | 0.0312 μs |  2.75 |    0.02 | 0.4730 |   3.91 KB |        1.13 |
|         |                      |           |           |           |       |         |        |           |             |
| **NoOp**    | **compression high win** |  **4.758 μs** | **0.0245 μs** | **0.0229 μs** |  **1.00** |    **0.01** | **0.6256** |   **5.13 KB** |        **1.00** |
| Deflate | compression high win | 13.172 μs | 0.1067 μs | 0.0998 μs |  2.77 |    0.02 | 0.5341 |   4.38 KB |        0.85 |
| Gzip    | compression high win | 13.480 μs | 0.0643 μs | 0.0570 μs |  2.83 |    0.02 | 0.5951 |   4.92 KB |        0.96 |
