```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method   | ScenarioName      | Set         | Mean      | Error     | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|--------- |------------------ |------------ |----------:|----------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| **Loop**     | **typical container** | **FFD,BFD**     |  **71.05 μs** |  **4.555 μs** | **0.250 μs** |  **1.00** |    **0.00** |  **7.3242** | **0.4883** |  **60.81 KB** |        **1.00** |
| Parallel | typical container | FFD,BFD     | 101.59 μs | 54.908 μs | 3.010 μs |  1.43 |    0.04 |  7.8125 | 0.7324 |  63.44 KB |        1.04 |
|          |                   |             |           |           |          |       |         |         |        |           |             |
| **Loop**     | **typical container** | **FFD,WFD,BFD** | **181.22 μs** | **36.224 μs** | **1.986 μs** |  **1.00** |    **0.01** | **10.9863** | **0.9766** |  **90.95 KB** |        **1.00** |
| Parallel | typical container | FFD,WFD,BFD | 133.17 μs | 88.325 μs | 4.841 μs |  0.73 |    0.02 | 11.4746 | 1.2207 |  93.85 KB |        1.03 |
|          |                   |             |           |           |          |       |         |         |        |           |             |
| **Loop**     | **BFD wins big**      | **FFD,BFD**     | **120.95 μs** | **14.564 μs** | **0.798 μs** |  **1.00** |    **0.01** |  **8.6670** | **0.6104** |   **71.3 KB** |        **1.00** |
| Parallel | BFD wins big      | FFD,BFD     |  99.77 μs | 18.354 μs | 1.006 μs |  0.82 |    0.01 |  9.0332 | 0.9766 |  73.94 KB |        1.04 |
|          |                   |             |           |           |          |       |         |         |        |           |             |
| **Loop**     | **BFD wins big**      | **FFD,WFD,BFD** | **299.87 μs** |  **7.460 μs** | **0.409 μs** |  **1.00** |    **0.00** | **13.1836** | **0.9766** | **109.02 KB** |        **1.00** |
| Parallel | BFD wins big      | FFD,WFD,BFD | 216.25 μs | 97.073 μs | 5.321 μs |  0.72 |    0.02 | 13.6719 | 1.9531 | 111.85 KB |        1.03 |
