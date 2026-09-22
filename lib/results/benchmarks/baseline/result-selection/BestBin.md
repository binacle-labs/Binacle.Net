```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method | ScenarioName           | Mean      | Error      | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------- |----------------------- |----------:|-----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **v1**     | **one full winner**        | **34.029 ns** | **29.2218 ns** | **1.6017 ns** |  **1.00** |    **0.06** | **0.0249** |     **208 B** |        **1.00** |
| v2     | one full winner        |  6.051 ns |  1.7740 ns | 0.0972 ns |  0.18 |    0.01 | 0.0029 |      24 B |        0.12 |
|        |                        |           |            |           |       |         |        |           |             |
| **v1**     | **all full, fullest wins** | **38.533 ns** |  **2.1737 ns** | **0.1191 ns** |  **1.00** |    **0.00** | **0.0249** |     **208 B** |        **1.00** |
| v2     | all full, fullest wins |  6.052 ns |  0.6062 ns | 0.0332 ns |  0.16 |    0.00 | 0.0029 |      24 B |        0.12 |
|        |                        |           |            |           |       |         |        |           |             |
| **v1**     | **all partial**            | **53.722 ns** |  **2.8178 ns** | **0.1545 ns** |  **1.00** |    **0.00** | **0.0401** |     **336 B** |        **1.00** |
| v2     | all partial            |  5.944 ns |  0.4944 ns | 0.0271 ns |  0.11 |    0.00 | 0.0029 |      24 B |        0.07 |
|        |                        |           |            |           |       |         |        |           |             |
| **v1**     | **20 bins, half full**     | **89.523 ns** |  **2.7996 ns** | **0.1535 ns** |  **1.00** |    **0.00** | **0.0248** |     **208 B** |        **1.00** |
| v2     | 20 bins, half full     | 25.251 ns |  4.7600 ns | 0.2609 ns |  0.28 |    0.00 | 0.0029 |      24 B |        0.12 |
