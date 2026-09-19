# Binacle.ViPaq.EncodedSize

Encodes every frozen pack every way ViPaq can - each codec, each layout - and protobuf beside it, and writes the
sizes as markdown into [`vipaq/results/`](../../results). Not a test: nothing here passes or fails, and encoded
size is deterministic, so the files are tracked and a change in the format shows up as a diff.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Program.cs` | Wires the runner, points the writer at `vipaq/results/`, runs the gates, then the reports |
| `PreReportChecks/` | The gates that run first: every pack round-trips through every mode, every curated pick still exists. A failure stops the run |
| `Tests/` | The two reports: ViPaq against protobuf, and where compression starts to pay |
| `ExtensionMethods/` | The registrations |

## 🛠️ How you use it

```
just measure vipaq
git diff vipaq/results
```

Run it after touching the encoder or the packed data. A diff is the finding; commit it with the change that
caused it.

## ⚠️ What will bite you

It overwrites the tracked files every run. The gates sweep all the packs through every mode before any report
is written, so a run takes a while and a wiring bug fails early with a named scenario. Timings are not measured
here - they belong in `vipaq/test/Binacle.ViPaq.Benchmarks`.
