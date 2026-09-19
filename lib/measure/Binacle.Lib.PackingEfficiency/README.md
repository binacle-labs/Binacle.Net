# Binacle.Lib.PackingEfficiency

Packs every scenario with every algorithm and writes what came out - fill rate, items placed, which version
won - as markdown into [`lib/results/`](../../results). Not a test: nothing here passes or fails, and the numbers
are deterministic, so the files are tracked and a change in the packer shows up as a diff.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Program.cs` | Wires the runner, points the writer at `lib/results/`, registers each report |
| `Tests/BischoffSuite/` | One class per report: packing efficiency, v1-vs-v2 regression, statistics per algorithm, comparison against BFD |
| `Results/` | The rows a report is made of |

## 🛠️ How you use it

```
just measure lib
git diff lib/results
```

Run it after touching an algorithm. A diff is the finding; commit it with the change that caused it.

## ⚠️ What will bite you

It overwrites the tracked files every run. Timings are not measured here on purpose - they vary by machine
and belong in `lib/test/Binacle.Lib.Benchmarks`.
