```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  default : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=default  

```
| Method | ScenarioName         | Mean      | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
|------- |--------------------- |----------:|----------:|----------:|------:|-------:|----------:|------------:|
| **v1**     | **one full winner**      |  **3.996 ns** | **0.0112 ns** | **0.0088 ns** |  **1.00** | **0.0029** |      **24 B** |        **1.00** |
| v2     | one full winner      |  6.197 ns | 0.0218 ns | 0.0194 ns |  1.55 | 0.0029 |      24 B |        1.00 |
|        |                      |           |           |           |       |        |           |             |
| **v1**     | **all full, first wins** |  **3.537 ns** | **0.0176 ns** | **0.0164 ns** |  **1.00** | **0.0029** |      **24 B** |        **1.00** |
| v2     | all full, first wins |  5.931 ns | 0.0489 ns | 0.0433 ns |  1.68 | 0.0029 |      24 B |        1.00 |
|        |                      |           |           |           |       |        |           |             |
| **v1**     | **all partial**          | **25.378 ns** | **0.1386 ns** | **0.1158 ns** |  **1.00** | **0.0181** |     **152 B** |        **1.00** |
| v2     | all partial          |  6.358 ns | 0.0589 ns | 0.0522 ns |  0.25 | 0.0029 |      24 B |        0.16 |
