# Binacle.Lib.Benchmarks.ParallelBins

When a request holds many bins, is packing them in parallel faster than packing them one after the other? It
runs Loop against Parallel, v2, one class per algorithm, on identical bins and identical items.

## 📂 What is in it

| Path | What it is |
|---|---|
| `IdenticalBase.cs` | Identical bins and identical items, every item fits. The best case for Parallel. Takes the algorithm. Setup throws if a bin is not fully packed |
| `Identical_FFD_Packing.cs`, `Identical_WFD_Packing.cs`, `Identical_BFD_Packing.cs` | One per algorithm, over bin counts and piece counts, one line |
| `Identical_FFD_Lines_Packing.cs` | FFD, the same pieces over more lines |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |

The bin and the item live in `IdenticalCase` in `lib/test/Binacle.Lib.Testing`.

## 🛠️ How you use it

```
just bench lib-parallel-bins-identical FFD dry    # each case once: checks the sizes
just bench lib-parallel-bins-identical FFD        # the short job; also WFD and BFD
```
