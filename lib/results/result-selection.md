# Result selection

What picking the result costs once packing is done, v2 against v1. Each selector runs on a few hand-made
cases, not on the Bischoff problems.

## 📊 Time and memory

Mean time per pick, in ns. "v2 ÷ v1" is v2's mean time divided by v1's: below 1 means v2 is faster.

### 📈 Best algorithm

| Case | v1 (ns) | v2 (ns) | v2 ÷ v1 | v1 memory | v2 memory |
|---|---|---|---|---|---|
| one full winner | 4.0 | 6.2 | 1.55 | 24 B | 24 B |
| all full, first wins | 3.5 | 5.9 | 1.68 | 24 B | 24 B |
| all partial | 25.4 | 6.4 | 0.25 | 152 B | 24 B |

### 📈 Best bin

| Case | v1 (ns) | v2 (ns) | v2 ÷ v1 | v1 memory | v2 memory |
|---|---|---|---|---|---|
| one full winner | 32.2 | 6.0 | 0.19 | 208 B | 24 B |
| all full, fullest wins | 38.1 | 5.9 | 0.16 | 208 B | 24 B |
| all partial | 55.8 | 5.9 | 0.11 | 336 B | 24 B |
| 20 bins, half full | 91.4 | 24.6 | 0.27 | 208 B | 24 B |

### 📈 Smallest bin

| Case | v1 (ns) | v2 (ns) | v2 ÷ v1 | v1 memory | v2 memory |
|---|---|---|---|---|---|
| one full winner | 31.6 | 7.0 | 0.22 | 208 B | 24 B |
| two full, smallest wins | 32.2 | 7.0 | 0.22 | 208 B | 24 B |
| all partial, tie on volume | 68.7 | 7.3 | 0.11 | 528 B | 24 B |
| 20 bins, half full | 72.5 | 30.4 | 0.42 | 208 B | 24 B |

v2 is slower than v1 in two best-algorithm cases: "one full winner" by 2.2 ns and "all full, first wins" by
2.4 ns.

## 📂 Where the numbers come from

The reports in [benchmarks/baseline/result-selection/](benchmarks/baseline/result-selection), default job,
BenchmarkDotNet on an AMD Ryzen 9 9900X, .NET 10.0.12.
