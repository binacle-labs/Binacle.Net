# Binacle.Lib.Benchmarks.ParallelBins

When a request holds many bins, is packing them in parallel faster than packing them one after the other? It
runs Loop against Parallel, FFD v2, one class per kind of bin set.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Even_Packing.cs` | Identical bins and identical items, every item fits. The best case for Parallel |
| `Program.cs` | Takes `--job` out of the args, builds the core jobs from it, and runs with the config from `shared/test/Binacle.Benchmarking` |
