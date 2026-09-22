```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method  | ScenarioName         | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------- |--------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **NoOp**    | **compression low win**  |  **3.144 μs** | **0.0279 μs** | **0.0233 μs** |  **1.00** |    **0.01** | **0.4196** |   **3.45 KB** |        **1.00** |
| Deflate | compression low win  |  8.585 μs | 0.0605 μs | 0.0566 μs |  2.73 |    0.03 | 0.3967 |   3.35 KB |        0.97 |
| Gzip    | compression low win  |  8.739 μs | 0.0586 μs | 0.0548 μs |  2.78 |    0.03 | 0.4730 |   3.91 KB |        1.13 |
|         |                      |           |           |           |       |         |        |           |             |
| **NoOp**    | **compression high win** |  **4.793 μs** | **0.0561 μs** | **0.0525 μs** |  **1.00** |    **0.01** | **0.6256** |   **5.13 KB** |        **1.00** |
| Deflate | compression high win | 13.194 μs | 0.0847 μs | 0.0792 μs |  2.75 |    0.03 | 0.5341 |   4.38 KB |        0.85 |
| Gzip    | compression high win | 13.393 μs | 0.0993 μs | 0.0929 μs |  2.79 |    0.04 | 0.5951 |   4.92 KB |        0.96 |
