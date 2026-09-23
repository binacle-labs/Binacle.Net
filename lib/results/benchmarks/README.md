# lib benchmark runs

Timing runs worth keeping, copied by hand from `lib/bench`. A run itself is not a measurement - it is one
machine's number on one day - so only runs that say something are kept, and each is kept whole.

Every file is a BenchmarkDotNet report, copied unchanged and renamed to the class that produced it.

## 📂 What is in it

| Folder | What it is |
|---|---|
| [baseline/](baseline) | The first kept run of every bench class. [Its README](baseline/README.md) lists each file with its recipe, job and case count, and the machine it ran on. |
| `<date>/` | A later run, named for the day it was kept. Only the reports that moved, so a date reads as a change against the baseline. |

Inside either, a report sits under its family:

| Family | What it compares |
|---|---|
| `algorithms/` | v2 of each packing algorithm against v1, packing and fitting |
| `racing/` | Racing several algorithms on one bin: one after the other against in parallel |
| `threshold/` | The same two, over a grid of item and bin counts, to find where parallel starts to pay |
| `result-selection/` | v2 of each result selector against v1 |
| `scaling/` | Packing time against item count, every algorithm in both versions |

## 📊 How to read a report

- **`Mean` compares nothing outside its own file.** Another file, another machine or another .NET version
  makes it a different number. The report header names the machine and the runtime.
- **`Ratio` and `Allocated` compare anywhere.** `Ratio` is each row against the baseline row of its group,
  which is the bold one. `Allocated` is bytes and does not care which machine ran it.
- **`RatioSD` is how much that ratio wobbled.** A ratio that moved by less than its own `RatioSD` did not move.
- **Loop against Parallel only holds on the same core count.** A 12-core result says nothing about 4 cores.

## 🛠️ How a run gets kept

By hand. A run writes into the project's own `BenchmarkDotNet.Artifacts/results/`, which git ignores, and the
next run of the same class overwrites it there - so copy it before running again.

```
just bench lib-algorithms-smoke    # run it; the report lands in the project's artifacts folder
```

Copy the report into `<date>/<family>/`, named for its class, and add a line to that folder's README saying
what ran.

## ⚠️ What will bite you

**Keep a run only when it changed something.** A ratio counts as moved when it moves by more than its own
`RatioSD` and by at least 5 percent, or when `Allocated` changes. Anything smaller is the machine breathing.
The 5 percent is a working number and open to argument.

**A new machine invalidates the times, not the ratios.** Nothing here yet says what to do when the machine
changes - the times in `baseline/` stop being a fair target, while every `Ratio` and `Allocated` still holds.
