# Algorithm performance

What BFD and WFD give and cost against FFD, packing, version 2. Fill gain is how many points fuller they pack than
FFD. Time is how many times as long they take: each problem is divided by FFD on that same problem, then those are
averaged, so FFD is 1.00×. Fill comes from [measurements/packing-efficiency.md](measurements/packing-efficiency.md),
time from the `Full_*_Packing.md` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms).

Times come from one machine - an AMD Ryzen 9 9900X, 12 cores, .NET 10.0.12 - at the short job, three iterations per
problem. One problem's time is rough; an average over 100 is not. The three algorithms ran as separate classes in one
run of `just bench lib-algorithms-full`, which is why their times can be divided.

## 📊 Fill gained and time paid, mean per set

| Set | Item types | BFD fill gain | BFD time | WFD fill gain | WFD time |
|---|---|---|---|---|---|
| thpack1 | 3 | +5.03 | 4.47× | -0.92 | 7.08× |
| thpack2 | 5 | +7.06 | 4.23× | -4.13 | 8.73× |
| thpack3 | 8 | +8.32 | 4.20× | -5.58 | 9.51× |
| thpack4 | 10 | +8.31 | 4.22× | -5.10 | 9.55× |
| thpack5 | 12 | +9.60 | 4.06× | -4.47 | 9.42× |
| thpack6 | 15 | +8.13 | 4.11× | -4.94 | 9.21× |
| thpack7 | 20 | +8.54 | 4.17× | -4.22 | 8.87× |
| **All 700** | 3 to 20 | **+7.85** | **4.21×** | **-4.19** | **8.91×** |

Memory is about the same for all three: over all 700, BFD uses 0.97× of FFD's memory and WFD 1.01×.

## 📊 BFD packing time against FFD, per set

| Set | Item types | Min | Mean | Median | Max |
|---|---|---|---|---|---|
| thpack1 | 3 | 2.12× | 4.47× | 4.18× | 10.60× |
| thpack2 | 5 | 1.85× | 4.23× | 4.19× | 8.17× |
| thpack3 | 8 | 1.86× | 4.20× | 4.08× | 9.42× |
| thpack4 | 10 | 2.49× | 4.22× | 4.23× | 8.71× |
| thpack5 | 12 | 2.22× | 4.06× | 3.91× | 9.66× |
| thpack6 | 15 | 2.45× | 4.11× | 3.92× | 8.54× |
| thpack7 | 20 | 2.74× | 4.17× | 4.02× | 8.11× |
| **All 700** | 3 to 20 | **1.85×** | **4.21×** | **4.06×** | **10.60×** |

## 📊 WFD packing time against FFD, per set

| Set | Item types | Min | Mean | Median | Max |
|---|---|---|---|---|---|
| thpack1 | 3 | 3.03× | 7.08× | 6.90× | 14.25× |
| thpack2 | 5 | 4.05× | 8.73× | 8.38× | 14.94× |
| thpack3 | 8 | 5.19× | 9.51× | 9.36× | 17.26× |
| thpack4 | 10 | 5.80× | 9.55× | 9.10× | 15.77× |
| thpack5 | 12 | 5.77× | 9.42× | 9.06× | 17.33× |
| thpack6 | 15 | 5.69× | 9.21× | 9.14× | 15.22× |
| thpack7 | 20 | 6.13× | 8.87× | 8.71× | 14.61× |
| **All 700** | 3 to 20 | **3.03×** | **8.91×** | **8.65×** | **17.33×** |
