# Binacle.ViPaq results

Measured output for the ViPaq wire format. Two kinds, one per folder, the same split as `results/lib/`:

| Folder | What it measures | Written by |
|---|---|---|
| [benchmarks/](benchmarks/) | Encode/decode speed and allocation, per mode | `vipaq/test/Binacle.ViPaq.Benchmarks` (BenchmarkDotNet) |
| [compression/](compression/) | Encoded size vs protobuf, and where compression starts to pay | the old performance-tests project, since replaced |

Nothing writes here any more. Size and crossover are now `vipaq/results/encoded-size.md` and its README,
written by `just measure vipaq`; the `compression/` files are the last hand-copied reports, kept until the
benchmark projects have run. `benchmarks/` is empty.
