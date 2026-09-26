```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4


```
| Method  | ScenarioName         | Items | Widths  | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------- |--------------------- |------:|-------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **NoOp**    | **compression low win**  |    **70** | **16/8/16** |  **3.148 μs** | **0.0217 μs** | **0.0203 μs** |  **1.00** |    **0.01** | **0.4196** |   **3.45 KB** |        **1.00** |
| Deflate | compression low win  |    70 | 16/8/16 |  8.415 μs | 0.0577 μs | 0.0540 μs |  2.67 |    0.02 | 0.3967 |   3.35 KB |        0.97 |
| Gzip    | compression low win  |    70 | 16/8/16 |  8.762 μs | 0.0990 μs | 0.0926 μs |  2.78 |    0.03 | 0.4730 |   3.91 KB |        1.13 |
|         |                      |       |         |           |           |           |       |         |        |           |             |
| **NoOp**    | **compression high win** |   **108** | **16/8/16** |  **4.724 μs** | **0.0247 μs** | **0.0231 μs** |  **1.00** |    **0.01** | **0.6256** |   **5.13 KB** |        **1.00** |
| Deflate | compression high win |   108 | 16/8/16 | 13.195 μs | 0.0912 μs | 0.0809 μs |  2.79 |    0.02 | 0.5341 |   4.38 KB |        0.85 |
| Gzip    | compression high win |   108 | 16/8/16 | 13.302 μs | 0.0790 μs | 0.0700 μs |  2.82 |    0.02 | 0.5951 |   4.92 KB |        0.96 |
