```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]  : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  default : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=default  

```
| Method | ScenarioName               | Mean      | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
|------- |--------------------------- |----------:|----------:|----------:|------:|-------:|----------:|------------:|
| **v1**     | **one full winner**            | **31.554 ns** | **0.2204 ns** | **0.1953 ns** |  **1.00** | **0.0249** |     **208 B** |        **1.00** |
| v2     | one full winner            |  7.044 ns | 0.0265 ns | 0.0248 ns |  0.22 | 0.0029 |      24 B |        0.12 |
|        |                            |           |           |           |       |        |           |             |
| **v1**     | **two full, smallest wins**    | **32.198 ns** | **0.1513 ns** | **0.1341 ns** |  **1.00** | **0.0249** |     **208 B** |        **1.00** |
| v2     | two full, smallest wins    |  6.958 ns | 0.0529 ns | 0.0495 ns |  0.22 | 0.0029 |      24 B |        0.12 |
|        |                            |           |           |           |       |        |           |             |
| **v1**     | **all partial, tie on volume** | **68.709 ns** | **0.4866 ns** | **0.4552 ns** |  **1.00** | **0.0631** |     **528 B** |        **1.00** |
| v2     | all partial, tie on volume |  7.251 ns | 0.0399 ns | 0.0354 ns |  0.11 | 0.0029 |      24 B |        0.05 |
|        |                            |           |           |           |       |        |           |             |
| **v1**     | **20 bins, half full**         | **72.540 ns** | **0.3345 ns** | **0.3129 ns** |  **1.00** | **0.0248** |     **208 B** |        **1.00** |
| v2     | 20 bins, half full         | 30.392 ns | 0.0753 ns | 0.0629 ns |  0.42 | 0.0029 |      24 B |        0.12 |
