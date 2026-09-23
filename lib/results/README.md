# Packing efficiency results

What `just measure lib` writes: how full `Binacle.Lib.PackingEfficiency` packs the 700 Bischoff suite
problems (thpack1..7) with every algorithm. The files are tracked, so a change in a packer shows up as a diff.

## 📂 Files

| File | What it is |
|---|---|
| [packing-efficiency.md](packing-efficiency.md) | One row per scenario: fill per algorithm, which won, and by how much |
| [version-parity.md](version-parity.md) | Per algorithm, only the scenarios where v1 and v2 pack differently |
| [benchmarks/](benchmarks) | Kept timing runs from `lib/bench`, and how to read them. Not written by `just measure` |

The summary of what these numbers say is not written yet.
