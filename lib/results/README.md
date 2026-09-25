# Packing efficiency results

What `just measure lib` writes, into `measurements/`: how full `Binacle.Lib.PackingEfficiency` packs the 700
Bischoff suite problems (thpack1..7) with every algorithm. The files are tracked, so a change in a packer shows up
as a diff.

## 📉 What the numbers say

All 700 Bischoff problems, one bin, version 2 - the shipped one. Fill is the packed volume as a percentage of
the bin. Time and memory are one pack of that problem, from the kept run in [benchmarks/](benchmarks).

| Algorithm | Fill min | Fill mean | Fill max | Time | Memory |
|---|---|---|---|---|---|
| FFD | 56.18 | **73.41** | 87.90 | **22.6 μs** | 36.4 KB |
| WFD | 49.15 | **69.21** | 87.90 | **209.5 μs** | 36.9 KB |
| BFD | 62.08 | **81.26** | 90.66 | **94.2 μs** | 35.2 KB |

Three sentences carry it:

- **BFD packs 7.85 points fuller than FFD and costs 4.2 times the time.** It wins or ties on 669 of the 700
  problems. When FFD wins it wins by 1.11 points; when BFD wins it wins by 7.85.
- **WFD costs 8.9 times FFD and adds 0.03 points.** Racing FFD and BFD reaches 81.30 mean fill. Adding WFD
  makes it 81.33. WFD is the sole best on 14 problems out of 700.
- **Nothing is ever full.** The best of all three leaves **18.1 points** of the bin empty on average, and
  never less than 7.5. No problem in the suite is packed to its volume ceiling by any algorithm.

### 📊 Per Bischoff set

Each set holds 100 problems and differs only in how many item types they use. Fill is min / mean / max.

| Set | Types | Items | FFD fill | FFD time | WFD fill | WFD time | BFD fill | BFD time |
|---|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 150 | 61.9 / 75.7 / 87.9 | 20.3 μs | 60.4 / 74.8 / 87.9 | 157.3 μs | 62.1 / **80.7** / 90.4 | 95.2 μs |
| thpack2 | 5 | 137 | 59.8 / 74.6 / 87.8 | 19.5 μs | 49.1 / 70.4 / 87.4 | 176.1 μs | 68.1 / **81.6** / 90.7 | 81.8 μs |
| thpack3 | 8 | 134 | 60.7 / 73.8 / 84.8 | 21.6 μs | 51.0 / 68.2 / 84.8 | 214.0 μs | 71.7 / **82.1** / 88.9 | 88.5 μs |
| thpack4 | 10 | 133 | 59.2 / 73.3 / 84.0 | 22.1 μs | 52.3 / 68.2 / 84.0 | 220.3 μs | 73.2 / **81.6** / 87.4 | 91.2 μs |
| thpack5 | 12 | 133 | 56.2 / 72.1 / 80.8 | 23.8 μs | 53.7 / 67.7 / 78.8 | 232.0 μs | 73.0 / **81.7** / 87.5 | 94.9 μs |
| thpack6 | 15 | 131 | 61.6 / 72.7 / 80.6 | 24.7 μs | 54.8 / 67.8 / 77.1 | 232.1 μs | 69.7 / **80.8** / 87.4 | 100.1 μs |
| thpack7 | 20 | 130 | 59.0 / 71.6 / 80.4 | 26.1 μs | 57.9 / 67.4 / 77.9 | 235.0 μs | 73.4 / **80.2** / 86.1 | 107.6 μs |

- **BFD leads every set.** Its worst set still beats FFD's best.
- **More item types hurt FFD and WFD, not BFD.** From thpack1 to thpack7, FFD drops 4.1 points and WFD drops
  7.4. BFD drops 0.6.
- **The price stays the same.** BFD is 4.06 to 4.47 times FFD's time in every set, while the fill it buys
  grows from 5.03 points at three item types to 8.54 at twenty.

### 🔁 Version 2 against version 1

Every number here is all 700 problems, one row per problem, from the kept full runs.

**Fill did not change.** 2,099 of 2,100 algorithm-problem pairs pack to the same fill. The one that moved is
BFD on thpack7_45, 79.08 to 79.73. [version-parity.md](measurements/version-parity.md) is that list.

**Speed and memory changed a lot, and packing and fitting changed differently** - they answer different
questions, so they are measured apart. Ratio is v2 as a share of v1 in the same run; under 1.00 means v2 is
better.

| | Packing time | Packing memory | Fitting time | Fitting memory |
|---|---|---|---|---|
| FFD | 0.67 | 0.38 | 0.57 | 0.56 |
| WFD | 0.53 | **0.05** | 0.55 | 0.26 |
| BFD | 0.46 | **0.08** | 0.44 | 0.26 |

In absolute terms, per pack:

| Algorithm | Packing memory v1 | v2 | Fitting time v1 | v2 |
|---|---|---|---|---|
| FFD | 95.1 KB | **36.4 KB** | 16.4 μs | 9.4 μs |
| WFD | 792.9 KB | **36.9 KB** | 50.9 μs | 28.0 μs |
| BFD | 468.6 KB | **35.2 KB** | 54.0 μs | 23.8 μs |

- **Memory no longer depends on which algorithm you pick.** All three now allocate about 36 KB. Under v1,
  choosing WFD cost 8 times FFD's memory. That reason to avoid it is gone.
- **v2 is faster on every problem but one.** FFD packing is slower on 1 of 700; WFD and BFD on none.
- **Fitting is cheaper than packing, by very different amounts.** FFD fits at 0.44 of its packing time, BFD at
  0.26, WFD at 0.15. WFD's early exit saves it from most of its own cost.

### 📈 Cost against size

From the scaling and threshold runs in [benchmarks/](benchmarks), on the synthetic item ladder.

**Time against item count**, 3 items to 79, version 2:

| Algorithm | 3 items | 79 items | Growth | Per item at 79 |
|---|---|---|---|---|
| FFD | 0.38 μs | 6.24 μs | 16× | 0.079 μs |
| BFD | 0.35 μs | 11.42 μs | 33× | 0.145 μs |
| WFD | 0.42 μs | 21.60 μs | 51× | 0.273 μs |

WFD grows worst as well as costing most.

**Time against bin count** grows roughly in step with the bin count - each bin is packed in turn. On the
ladder used for that run each added bin is also taller than the last, so bin count and total work grow
together.

## 📂 Files

| File | What it is |
|---|---|
| [measurements/packing-efficiency.md](measurements/packing-efficiency.md) | One row per scenario: fill per algorithm, which won, and by how much |
| [measurements/version-parity.md](measurements/version-parity.md) | Per algorithm, only the scenarios where v1 and v2 pack differently |
| [benchmarks/](benchmarks) | Kept timing runs from `lib/bench`, and how to read them. Not written by `just measure` |

## 🛠️ How you use it

```
just measure lib      # rewrites packing-efficiency.md and version-parity.md
just bench            # the list of timing runs, with what each one costs
```

## ⚠️ What will bite you

**A time only compares inside its own run.** Every time on this page comes from one machine - an AMD Ryzen 9
9900X, 12 cores, .NET 10.0.12 - on 2026-09-23 to 2026-09-25. Another machine or another runtime makes a
different number. **Fill, memory and every ratio compare anywhere.**

**The 700-problem times ran at the short job**, three iterations per case. One problem's time is rough; an
average over 700 is not. The ladder runs used the default job. Fill does not care either way - it is
deterministic.

**Fill is measured on one bin.** Nothing on this page says what a request against many bins costs.

### 🕳️ What is not measured yet

These are open, and naming them here is the point - do not read a number off this page for a case it does not
cover.

- **Core count is not an axis anywhere.** Every timing run used 12 cores. Whether packing many bins in
  parallel pays depends on core count, and the sign of that answer changes with it, so no rule on this page
  can be turned into a switch for a machine with a different core count.
- **Running bins in parallel** has one grid, on one machine, over bins 1 to 7 and 3 to 79 items. Enough to see
  the shape, not enough to ship a threshold.
- **Racing algorithms in parallel** has two runs that disagree with each other on two of five problems.
  Unsettled.
- **Racing across many bins at the same time** is measured by nothing.
- **The edge cases are all missing.** Every timing run uses either the Bischoff suite, where 69 to 476 items
  always pack at least 56% of the bin, or the synthetic ladder of 3 to 79 items. Nothing times a request
  where no item fits, where there are no items, where there is one item, where far more items are sent than
  fit, or where the item count runs past 476. The demo samples and custom problems hold cases like that and
  are never timed.
