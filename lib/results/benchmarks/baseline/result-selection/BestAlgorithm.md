```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method | ScenarioName         | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------- |--------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **v1**     | **one full winner**      |  **4.076 ns** | **0.9254 ns** | **0.0507 ns** |  **1.00** |    **0.02** | **0.0029** |      **24 B** |        **1.00** |
| v2     | one full winner      |  6.164 ns | 1.3152 ns | 0.0721 ns |  1.51 |    0.02 | 0.0029 |      24 B |        1.00 |
|        |                      |           |           |           |       |         |        |           |             |
| **v1**     | **all full, first wins** |  **3.537 ns** | **0.5141 ns** | **0.0282 ns** |  **1.00** |    **0.01** | **0.0029** |      **24 B** |        **1.00** |
| v2     | all full, first wins |  5.954 ns | 0.8330 ns | 0.0457 ns |  1.68 |    0.02 | 0.0029 |      24 B |        1.00 |
|        |                      |           |           |           |       |         |        |           |             |
| **v1**     | **all partial**          | **25.450 ns** | **1.2287 ns** | **0.0673 ns** |  **1.00** |    **0.00** | **0.0181** |     **152 B** |        **1.00** |
| v2     | all partial          |  6.029 ns | 0.5352 ns | 0.0293 ns |  0.24 |    0.00 | 0.0029 |      24 B |        0.16 |
