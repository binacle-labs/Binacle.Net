```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method | ScenarioName               | Mean      | Error      | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
|------- |--------------------------- |----------:|-----------:|----------:|------:|-------:|----------:|------------:|
| **v1**     | **one full winner**            | **32.058 ns** |  **2.2321 ns** | **0.1223 ns** |  **1.00** | **0.0249** |     **208 B** |        **1.00** |
| v2     | one full winner            |  7.174 ns |  1.3977 ns | 0.0766 ns |  0.22 | 0.0029 |      24 B |        0.12 |
|        |                            |           |            |           |       |        |           |             |
| **v1**     | **two full, smallest wins**    | **32.825 ns** |  **3.3089 ns** | **0.1814 ns** |  **1.00** | **0.0249** |     **208 B** |        **1.00** |
| v2     | two full, smallest wins    |  7.054 ns |  1.4321 ns | 0.0785 ns |  0.21 | 0.0029 |      24 B |        0.12 |
|        |                            |           |            |           |       |        |           |             |
| **v1**     | **all partial, tie on volume** | **69.674 ns** |  **5.3614 ns** | **0.2939 ns** |  **1.00** | **0.0631** |     **528 B** |        **1.00** |
| v2     | all partial, tie on volume |  7.363 ns |  0.9640 ns | 0.0528 ns |  0.11 | 0.0029 |      24 B |        0.05 |
|        |                            |           |            |           |       |        |           |             |
| **v1**     | **20 bins, half full**         | **74.127 ns** | **14.2352 ns** | **0.7803 ns** |  **1.00** | **0.0248** |     **208 B** |        **1.00** |
| v2     | 20 bins, half full         | 30.641 ns |  1.4772 ns | 0.0810 ns |  0.41 | 0.0029 |      24 B |        0.12 |
