# Binacle.Lib.Benchmarks.ParallelBins

When a request holds many bins, is packing them in parallel faster than packing them one after the other? It
runs Loop against Parallel, v2, on identical bins and identical items: a piece sweep and a line sweep for each
algorithm.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Identical/IdenticalBase.cs` | The `Loop` and `Parallel` rows and the setup they all share. Identical bins and items, every item fits - the best case for Parallel. Setup throws if a bin is not fully packed |
| `Identical/PiecesBase.cs` | The grid the piece classes run: bin counts by piece counts, one line |
| `Identical/LinesBase.cs` | The grid the line classes run: the same pieces over more lines |
| `Identical/Identical_FFD_Pieces_Packing.cs`, `..._WFD_...`, `..._BFD_...` | One algorithm each |
| `Identical/Identical_FFD_Lines_Packing.cs`, `..._WFD_...`, `..._BFD_...` | One algorithm each |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |

The grids sit on the two kind bases, not on the classes, so the reports of a kind stay comparable.

The bin and the item live in `IdenticalCase` in `lib/test/Binacle.Lib.Testing`.

## 📏 What the line classes are for

The same pieces handed over two ways: one line of many, then many lines of one. They should come out flat -
lines become pieces in the algorithm's constructor, and both forms build the same array. **Flat is the answer
they exist to prove**, not a reason to skip running them.

## 🛠️ How you use it

```
just bench lib-parallel-bins-identical FFD dry    # each case once: checks the sizes
just bench lib-parallel-bins-identical FFD        # the short job; also WFD and BFD
```
