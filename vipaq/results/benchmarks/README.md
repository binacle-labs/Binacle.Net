# ViPaq benchmark runs

Timing runs worth keeping, copied by hand from `vipaq/bench`. A run itself is not a measurement - it is one
machine's number on one day - so only runs that say something are kept, and each is kept whole.

Every file is a BenchmarkDotNet report, copied unchanged and renamed to the class that produced it.

Size is a different thing and is not here: `just measure vipaq` writes it to
[`vipaq/results/measurements`](../measurements).

## 📂 What is in it

| Folder | What it is |
|---|---|
| [baseline/](baseline) | Every bench class at the shape the code has now. [Its README](baseline/README.md) lists each file with its recipe and job, and the machine it ran on. |

A report sits under its family. ViPaq has one:

| Family | What it compares |
|---|---|
| `encoding/` | ViPaq against protobuf, encoding and decoding, in both the row and the columnar layout. The `CompressionCost` pair compares compressing the token with leaving it alone. |

## 📊 How to read a report

- **`Mean` compares nothing outside its own file.** Another file, another machine or another .NET version
  makes it a different number. The report header names the machine and the runtime.
- **`Ratio` and `Allocated` compare anywhere.** `Ratio` is each row against the baseline row of its group,
  which is the bold one - protobuf in the encode and decode files, no compression in the cost files.
  `Allocated` is bytes and does not care which machine ran it.
- **`RatioSD` is how much that ratio wobbled.** A ratio that moved by less than its own `RatioSD` did not move.

## 🛠️ How a run gets kept

By hand. A run writes into the project's own `BenchmarkDotNet.Artifacts/results/`, which git ignores, and the
next run of the same class overwrites it there - so copy it before running again.

```
just bench vipaq-sample    # run it; the reports land in the project's artifacts folder
```

Copy the report into `baseline/encoding/`, named for its class, and add a line to that folder's README saying
what ran. `baseline/` is the only folder while the benchmarking is still going on; dated folders, each
holding only the reports that moved, start once it is finished.

## ⚠️ What will bite you

**Keep a run only when it changed something.** A ratio counts as moved when it moves by more than its own
`RatioSD` and by at least 5 percent, or when `Allocated` changes. Anything smaller is the machine breathing.
The 5 percent is a working number.
A report that gained a column a results file reads counts as changed too; say so in the folder's README.

**A new machine invalidates the times, not the ratios.** The times in `baseline/` stop being a fair target,
while every `Ratio` and `Allocated` still holds.
