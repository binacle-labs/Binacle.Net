# Binacle.Lib.PackingEfficiency

Packs every Bischoff suite scenario with every algorithm version, once, and writes what came out - fill per
algorithm, which won, where v1 and v2 differ - as markdown into [`lib/results/`](../../results). Not a test:
nothing here passes or fails, and the numbers are deterministic, so the files are tracked and a change in the
packer shows up as a diff.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Program.cs` | Wires the bag, the runner, the reporters and the writer, pointed at `lib/results/` |
| `PackingRunner.cs` | Packs 700 scenarios with six algorithm versions and fills the bag |
| `PackingBag.cs` | What the runner measured; every reporter reads from here |
| `ResultFiles.cs` | The three files and the header sentence they open with |
| `Reporters/` | One class per file: the README summaries, the per-scenario rows, the v1/v2 differences |

## 🛠️ How you use it

```
just measure lib
git diff lib/results
```

Run it after touching an algorithm. A diff is the finding; commit it with the change that caused it.

## ⚠️ What will bite you

It overwrites the tracked files every run. Timings are not measured here on purpose - they vary by machine
and belong in `lib/test/Binacle.Lib.Benchmarks`.
