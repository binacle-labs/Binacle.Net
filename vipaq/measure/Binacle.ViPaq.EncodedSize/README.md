# Binacle.ViPaq.EncodedSize

Encodes every frozen pack every way ViPaq can - each codec, each layout - and protobuf, JSON and compact
notation beside it, and writes the sizes as markdown into [`vipaq/results/`](../../results). Not a test:
nothing here passes or fails, and encoded size is deterministic, so the files are tracked and a change in the
format shows up as a diff.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Program.cs` | Wires the bag, the runner, the reporters and the writer, pointed at `vipaq/results/`; runs the gate first |
| `PreReportChecks/` | The gate: every curated benchmark pick still names a real pack. A failure stops the run |
| `EncodingRunner.cs` | Encodes every pack in every format and fills the bag |
| `EncodingBag.cs` | What the runner measured; every reporter reads from here |
| `ResultFiles.cs` | The file and the header sentence it opens with |
| `Reporters/` | One class per file: the per-pack rows |

## 🛠️ How you use it

```
just measure vipaq
git diff vipaq/results
```

Run it after touching the encoder or the packed data. A diff is the finding; commit it with the change that
caused it.

## ⚠️ What will bite you

It overwrites the tracked files every run, except `README.md`, which is written by hand. Timings are not measured here - they belong in
`vipaq/bench/Binacle.ViPaq.Benchmarks`.
