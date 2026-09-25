# Binacle.ViPaq.EncodedSize

Encodes every frozen pack every way ViPaq can - each codec, each layout - and protobuf, JSON and compact
notation through the same codecs beside it, and writes the sizes as markdown into [`vipaq/results/measurements/encoded-size/`](../../results/measurements/encoded-size) - a
folder per algorithm, one file per layout per group inside it. Not a test:
nothing here passes or fails, and encoded size is deterministic, so the files are tracked and a change in the
format shows up as a diff.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Program.cs` | Wires the bag, the runner, the reporters and the writer, pointed at `vipaq/results/measurements/`; runs the gate first |
| `PreReportChecks/` | The gates: every curated benchmark pick still names a real pack, and every pack lands in a file. A failure stops the run |
| `EncodingRunner.cs` | Encodes every pack in every format and fills the bag |
| `EncodingBag.cs` | What the runner measured; every reporter reads from here |
| `ResultFiles.cs` | One file per group per algorithm per layout, and the header sentence each opens with |
| `Groups.cs` | Which file a pack goes in - the Bischoff suite splits by its seven sets, the other two families are one each |
| `Reporters/` | The per-pack rows, one table per codec. One instance per file, registered in `Program.cs` |

## 🛠️ How you use it

```
just measure vipaq
git diff vipaq/results/measurements
```

Run it after touching the encoder or the packed data. A diff is the finding; commit it with the change that
caused it.

## ⚠️ What will bite you

It overwrites the tracked files every run. Adding a Bischoff set means adding it to `Groups.All`, and a fourth algorithm means adding it to
`Algorithms.All`; the gate stops the run rather than dropping those packs silently. Timings are not measured here - they belong in
`vipaq/bench/Binacle.ViPaq.Benchmarks`.
