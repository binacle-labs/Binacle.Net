# Lib results

How full each packing algorithm fills a bin, what it costs in time and memory, and what changed between v1
and v2.

[INDEX.md](INDEX.md) lists every file here and the question it answers.

## 📝 What the results say

Every number here is quoted from the file beside it. Nothing on this page is computed.

| The question | The answer | Read |
|---|---|---|
| How full does each algorithm pack? | BFD fills 81.26% of the bin on average, FFD 73.41%, WFD 69.21%. Running all three and keeping the best on each problem gives 81.33%. | `packing-efficiency.md` |
| Is v2 better than v1? | Same fill on every problem but one, in BFD. Median time is 0.44 to 0.67 of v1's, median memory 0.05 to 0.56. | `version-differences.md` |
| What does BFD's fill cost? | 4.06x FFD's time, median per problem. Running all three costs 3.43x BFD alone, for 0.07 points of fill. | `algorithm-cost.md` |
| How does time grow with size? | Doubling the items multiplies the time by about 2.5 for FFD, 3.5 for WFD and 2.4 for BFD. | `packing-time-by-size.md` |
| When is racing algorithms in parallel faster? | FFD and BFD in parallel beats one after the other from 16 pieces, 32 on 8 cores. | `parallel-algorithms.md` |
| When is packing bins in parallel faster? | From 1 to 32 pieces per bin, depending on bins and cores. The best is 0.25 of the time, WFD on 12 cores; FFD and BFD stay near 0.5. | `parallel-bins.md` |
| What does picking the result cost? | v2 takes 0.11 to 0.42 of v1's time to pick a bin. Picking the best algorithm is about 2 ns slower in v2 in two cases. | `result-selection.md` |

## 🛠️ How you use it

```
just measure lib      # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```

## ⚠️ What will bite you

The `Full_*` timing reports ran a short job, so one problem's time is rough. Means and medians hold; the
files that use them say where a single number does not.
