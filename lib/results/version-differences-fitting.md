# Version differences - fitting

Version 2 against version 1, fitting, per algorithm. Each problem is divided by v1 on that same problem, then those
are averaged, so v1 is 1.00× and under it v2 is faster or allocates less. Computed from the `Full_*_Fitting.md`
reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms).

Times come from one machine - an AMD Ryzen 9 9900X, 12 cores, .NET 10.0.12 - at the short job, three iterations per
problem. One problem's time is rough; an average over 100 is not. v1 and v2 of an algorithm ran side by side in one
class, so the ratio compares anywhere.

## 📊 v2 against v1, mean per set

| Set | Item types | FFD time | FFD memory | BFD time | BFD memory | WFD time | WFD memory |
|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.59× | 0.53× | 0.46× | 0.26× | 0.55× | 0.23× |
| thpack2 | 5 | 0.57× | 0.54× | 0.45× | 0.25× | 0.54× | 0.24× |
| thpack3 | 8 | 0.56× | 0.55× | 0.43× | 0.26× | 0.54× | 0.25× |
| thpack4 | 10 | 0.56× | 0.56× | 0.45× | 0.26× | 0.56× | 0.27× |
| thpack5 | 12 | 0.57× | 0.57× | 0.43× | 0.25× | 0.56× | 0.27× |
| thpack6 | 15 | 0.58× | 0.58× | 0.45× | 0.26× | 0.55× | 0.27× |
| thpack7 | 20 | 0.59× | 0.59× | 0.44× | 0.26× | 0.56× | 0.28× |
| **All 700** | 3 to 20 | **0.57×** | **0.56×** | **0.44×** | **0.26×** | **0.55×** | **0.26×** |

## 📊 FFD v2 against v1, per set

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.51× | 0.59× | 0.58× | 0.72× | 0.47× | 0.53× | 0.54× | 0.58× |
| thpack2 | 5 | 0.51× | 0.57× | 0.56× | 0.76× | 0.49× | 0.54× | 0.55× | 0.59× |
| thpack3 | 8 | 0.50× | 0.56× | 0.56× | 0.64× | 0.48× | 0.55× | 0.56× | 0.60× |
| thpack4 | 10 | 0.49× | 0.56× | 0.56× | 0.62× | 0.51× | 0.56× | 0.56× | 0.62× |
| thpack5 | 12 | 0.52× | 0.57× | 0.57× | 0.62× | 0.52× | 0.57× | 0.57× | 0.61× |
| thpack6 | 15 | 0.52× | 0.58× | 0.58× | 0.63× | 0.52× | 0.58× | 0.58× | 0.62× |
| thpack7 | 20 | 0.55× | 0.59× | 0.59× | 0.63× | 0.53× | 0.59× | 0.59× | 0.63× |
| **All 700** | 3 to 20 | **0.49×** | **0.57×** | **0.57×** | **0.76×** | **0.47×** | **0.56×** | **0.56×** | **0.63×** |

## 📊 BFD v2 against v1, per set

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.36× | 0.46× | 0.46× | 0.60× | 0.11× | 0.26× | 0.25× | 0.42× |
| thpack2 | 5 | 0.33× | 0.45× | 0.44× | 0.64× | 0.15× | 0.25× | 0.25× | 0.39× |
| thpack3 | 8 | 0.33× | 0.43× | 0.43× | 0.60× | 0.17× | 0.26× | 0.25× | 0.38× |
| thpack4 | 10 | 0.32× | 0.45× | 0.44× | 0.65× | 0.16× | 0.26× | 0.26× | 0.43× |
| thpack5 | 12 | 0.32× | 0.43× | 0.43× | 0.64× | 0.15× | 0.25× | 0.25× | 0.44× |
| thpack6 | 15 | 0.30× | 0.45× | 0.44× | 0.64× | 0.17× | 0.26× | 0.25× | 0.46× |
| thpack7 | 20 | 0.33× | 0.44× | 0.45× | 0.62× | 0.16× | 0.26× | 0.25× | 0.35× |
| **All 700** | 3 to 20 | **0.30×** | **0.44×** | **0.44×** | **0.65×** | **0.11×** | **0.26×** | **0.25×** | **0.46×** |

## 📊 WFD v2 against v1, per set

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.41× | 0.55× | 0.53× | 0.88× | 0.10× | 0.23× | 0.22× | 0.40× |
| thpack2 | 5 | 0.40× | 0.54× | 0.52× | 0.83× | 0.12× | 0.24× | 0.23× | 0.42× |
| thpack3 | 8 | 0.41× | 0.54× | 0.54× | 0.81× | 0.12× | 0.25× | 0.24× | 0.48× |
| thpack4 | 10 | 0.46× | 0.56× | 0.55× | 0.98× | 0.13× | 0.27× | 0.25× | 0.48× |
| thpack5 | 12 | 0.47× | 0.56× | 0.55× | 0.84× | 0.13× | 0.27× | 0.26× | 0.49× |
| thpack6 | 15 | 0.44× | 0.55× | 0.54× | 0.83× | 0.13× | 0.27× | 0.27× | 0.49× |
| thpack7 | 20 | 0.47× | 0.56× | 0.56× | 0.79× | 0.16× | 0.28× | 0.26× | 0.52× |
| **All 700** | 3 to 20 | **0.40×** | **0.55×** | **0.54×** | **0.98×** | **0.10×** | **0.26×** | **0.25×** | **0.52×** |
