```

BenchmarkDotNet v0.15.8, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD Ryzen 9 9900X 4.39GHz, 12 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.112
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method         | ScenarioName      | Mean         | Error      | StdDev    | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------- |------------------ |-------------:|-----------:|----------:|------:|-------:|-------:|----------:|------------:|
| **Protobuf**       | **one item**          |     **86.26 ns** |   **3.126 ns** |  **0.171 ns** |  **1.00** | **0.0564** |      **-** |     **472 B** |        **1.00** |
| ViPaq_Row      | one item          |     83.27 ns |  26.180 ns |  1.435 ns |  0.97 | 0.0362 |      - |     304 B |        0.64 |
| ViPaq_Columnar | one item          |     83.62 ns |   2.222 ns |  0.122 ns |  0.97 | 0.0362 |      - |     304 B |        0.64 |
|                |                   |              |            |           |       |        |        |           |             |
| **Protobuf**       | **typical container** |  **1,988.59 ns** |  **87.700 ns** |  **4.807 ns** |  **1.00** | **0.8049** | **0.0153** |    **6760 B** |        **1.00** |
| ViPaq_Row      | typical container |  2,403.66 ns |  23.269 ns |  1.275 ns |  1.21 | 0.5875 | 0.0038 |    4936 B |        0.73 |
| ViPaq_Columnar | typical container |  2,520.92 ns |  88.035 ns |  4.825 ns |  1.27 | 0.5875 | 0.0038 |    4936 B |        0.73 |
|                |                   |              |            |           |       |        |        |           |             |
| **Protobuf**       | **largest real pack** | **10,886.49 ns** | **764.222 ns** | **41.890 ns** |  **1.00** | **3.7537** | **0.3052** |   **31400 B** |        **1.00** |
| ViPaq_Row      | largest real pack | 12,235.89 ns | 368.230 ns | 20.184 ns |  1.12 | 2.9449 | 0.1373 |   24705 B |        0.79 |
| ViPaq_Columnar | largest real pack | 12,642.17 ns | 139.312 ns |  7.636 ns |  1.16 | 2.9449 | 0.1373 |   24705 B |        0.79 |
