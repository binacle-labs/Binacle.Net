# Binacle.Lib.Benchmarks.ParallelOverhead

What does parallelising **cost**? Three classes, and none of them pack: the algorithms do not pack, so what a
row measures is what the processor costs around them.

The other two parallel projects ask whether parallel **pays** on real work. This one asks only what it costs,
which is the other half of that question and the only number both of them need.

It exists because the cost used to be read as the gap between two packing rows, and subtracting two numbers of
similar size is the least reliable way to get one. Here it is measured straight.

## 📂 What is in it

| Path | What it is |
|---|---|
| `FakeRaceBase.cs` | The three rows, and the setup the race classes share |
| `Fake_TwoAlgorithms.cs` | A two-algorithm race, the one the multi-bin routes run |
| `Fake_ThreeAlgorithms.cs` | A three-algorithm race, the one the single-bin routes run |
| `Fake_Bins.cs` | 1, 2 or 8 bins through the bin processors |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it over 1, 2, 4, 8 and 12 CPUs, and runs with the config from `shared/test/Binacle.Benchmarking` |

`FakeAlgorithmFactory`, whose algorithms do not pack, lives in `lib/test/Binacle.Lib.Testing` with the other
doubles. The bin and the item come from `IdenticalCase` there too, the same ones the packing projects use -
nothing here packs them, the processors only carry them through.

A race's width is fixed by production, 2 or 3, so it is a class. A request carries any number of bins, so that
is a parameter.

## 🛠️ How you use it

```
just bench lib-parallel-overhead dry      # each case once: checks the pinning
just bench lib-parallel-overhead quick    # the short job
just bench lib-parallel-overhead          # the default job
just bench lib-parallel-overhead precise 3  # the default job, each case in 3 processes
```

`just bench` lists it with its cost. The report lands in `BenchmarkDotNet.Artifacts/results/`, gitignored.

## 📊 The three rows, and what the gaps mean

| Row | What it runs |
|---|---|
| `Loop` | the work with no `Parallel.For` in it at all |
| `Parallel_OneThread` | `Parallel.For` capped to one thread, so nothing is handed to another CPU |
| `Parallel` | `Parallel.For` as production runs it |

The gaps are the answer, not the rows:

- **`Loop` to `Parallel_OneThread`** is what `Parallel.For` costs to set up. It does not move with the core
  count - measured flat across all five, which is what makes it the reference line.
- **`Parallel_OneThread` to `Parallel`** is what handing the work to another CPU costs. This is the one that
  moves with the machine.

## 📏 `Bytes` - the working set

`Bytes` is how much memory each fake algorithm walks before returning, a cache line at a time. Each instance
gets its own buffer, as a real algorithm builds its own piece array.

- **`Bytes = 0`** is the pure machinery cost, and nothing but the machinery.
- **`Bytes` above 0** stands in for a real algorithm's working set. A thread on a far CPU has to fetch data its
  cache does not hold, and that cost cannot appear at all when the algorithms touch nothing.

**Why it is here:** the machinery alone came out at about a third of what the real drop points imply. The rest
is the work itself being slower away from home. If no byte size lines up with what the packing projects show,
then the cost cannot be separated from the work and this project only ever gives a floor.

## ⚠️ What will bite you

**A `Mean` here is not a packing time.** It is what the scaffolding costs around work that does not exist, so it
is only read against the `Loop` row of its own core count.

**1 CPU is the dear end, not the cheap one.** `Parallel.For` still hands the work to a worker thread, and with
one CPU that worker waits for the caller to be taken off it. Measured dearer than any other core count.

`Parallel_OneThread` is not a production path. It passes `maxDegreeOfParallelism: 1`, which makes `Parallel.For`
run the body on the calling thread.

The pinning is Linux only, and a class run outside `Program` fails at setup. The 2, 4, 8 and 12 CPU jobs are the
set every other project runs, so the three reports can be read side by side; this one adds 1 CPU.

**The 1-CPU rows of `Fake_Bins` are not usable, and the race rows are.** At one CPU the thread pool and the
garbage collector contend with the benchmark for it, which a 2- or 3-wide race walking nothing does not
provoke. A race row there is tight. A bins row there can read `Loop` slower than `Parallel_OneThread`, which
cannot happen, or carry a StdDev over half its own mean. Read a 1-CPU bins row only as a sign the case ran.
