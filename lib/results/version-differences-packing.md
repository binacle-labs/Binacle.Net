# Version differences - packing

Version 2 against version 1, packing, per algorithm. Each problem is divided by v1 on that same problem, then those
are averaged, so v1 is 1.00× and under it v2 is faster or allocates less. Computed from the `Full_*_Packing.md`
reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms).

Times come from one machine - an AMD Ryzen 9 9900X, 12 cores, .NET 10.0.12 - at the short job, three iterations per
problem. One problem's time is rough; an average over 100 is not. v1 and v2 of an algorithm ran side by side in one
class, so the ratio compares anywhere.

## 📊 v2 against v1, mean per set

| Set | Item types | FFD time | FFD memory | BFD time | BFD memory | WFD time | WFD memory |
|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.64× | 0.38× | 0.48× | 0.09× | 0.53× | 0.07× |
| thpack2 | 5 | 0.65× | 0.38× | 0.48× | 0.09× | 0.54× | 0.06× |
| thpack3 | 8 | 0.66× | 0.38× | 0.48× | 0.08× | 0.53× | 0.05× |
| thpack4 | 10 | 0.66× | 0.38× | 0.46× | 0.08× | 0.53× | 0.05× |
| thpack5 | 12 | 0.69× | 0.38× | 0.44× | 0.08× | 0.55× | 0.05× |
| thpack6 | 15 | 0.70× | 0.39× | 0.45× | 0.07× | 0.53× | 0.05× |
| thpack7 | 20 | 0.71× | 0.39× | 0.44× | 0.07× | 0.53× | 0.05× |
| **All 700** | 3 to 20 | **0.67×** | **0.38×** | **0.46×** | **0.08×** | **0.53×** | **0.05×** |

## 📊 FFD v2 against v1, per set

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.56× | 0.64× | 0.63× | 0.87× | 0.30× | 0.38× | 0.38× | 0.48× |
| thpack2 | 5 | 0.57× | 0.65× | 0.64× | 0.78× | 0.32× | 0.38× | 0.38× | 0.45× |
| thpack3 | 8 | 0.58× | 0.66× | 0.66× | 0.80× | 0.32× | 0.38× | 0.38× | 0.45× |
| thpack4 | 10 | 0.59× | 0.66× | 0.66× | 0.74× | 0.34× | 0.38× | 0.38× | 0.47× |
| thpack5 | 12 | 0.62× | 0.69× | 0.68× | 0.83× | 0.34× | 0.38× | 0.38× | 0.44× |
| thpack6 | 15 | 0.62× | 0.70× | 0.70× | 0.84× | 0.34× | 0.39× | 0.39× | 0.44× |
| thpack7 | 20 | 0.50× | 0.71× | 0.70× | 1.36× | 0.35× | 0.39× | 0.39× | 0.44× |
| **All 700** | 3 to 20 | **0.50×** | **0.67×** | **0.67×** | **1.36×** | **0.30×** | **0.38×** | **0.39×** | **0.48×** |

## 📊 BFD v2 against v1, per set

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.30× | 0.48× | 0.48× | 0.71× | 0.04× | 0.09× | 0.09× | 0.15× |
| thpack2 | 5 | 0.32× | 0.48× | 0.48× | 0.77× | 0.05× | 0.09× | 0.09× | 0.17× |
| thpack3 | 8 | 0.31× | 0.48× | 0.48× | 0.87× | 0.05× | 0.08× | 0.08× | 0.14× |
| thpack4 | 10 | 0.28× | 0.46× | 0.46× | 0.70× | 0.05× | 0.08× | 0.08× | 0.12× |
| thpack5 | 12 | 0.29× | 0.44× | 0.45× | 0.56× | 0.04× | 0.08× | 0.07× | 0.13× |
| thpack6 | 15 | 0.27× | 0.45× | 0.46× | 0.60× | 0.04× | 0.07× | 0.07× | 0.10× |
| thpack7 | 20 | 0.26× | 0.44× | 0.45× | 0.74× | 0.05× | 0.07× | 0.07× | 0.10× |
| **All 700** | 3 to 20 | **0.26×** | **0.46×** | **0.46×** | **0.87×** | **0.04×** | **0.08×** | **0.08×** | **0.17×** |

## 📊 WFD v2 against v1, per set

| Set | Item types | Time min | Time mean | Time median | Time max | Memory min | Memory mean | Memory median | Memory max |
|---|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 0.40× | 0.53× | 0.53× | 0.84× | 0.03× | 0.07× | 0.06× | 0.16× |
| thpack2 | 5 | 0.41× | 0.54× | 0.53× | 0.90× | 0.03× | 0.06× | 0.05× | 0.11× |
| thpack3 | 8 | 0.45× | 0.53× | 0.53× | 0.65× | 0.03× | 0.05× | 0.05× | 0.09× |
| thpack4 | 10 | 0.41× | 0.53× | 0.53× | 0.90× | 0.03× | 0.05× | 0.05× | 0.09× |
| thpack5 | 12 | 0.42× | 0.55× | 0.54× | 0.91× | 0.03× | 0.05× | 0.05× | 0.08× |
| thpack6 | 15 | 0.38× | 0.53× | 0.53× | 0.89× | 0.03× | 0.05× | 0.04× | 0.07× |
| thpack7 | 20 | 0.44× | 0.53× | 0.52× | 0.91× | 0.03× | 0.05× | 0.05× | 0.07× |
| **All 700** | 3 to 20 | **0.38×** | **0.53×** | **0.53×** | **0.91×** | **0.03×** | **0.05×** | **0.05×** | **0.16×** |
