# ViPaq results index

Every file in this folder, and what it holds.

## 📂 Raw results

| Path | What it is |
|---|---|
| [measurements/](measurements) | How long every real pack is as a ViPaq token, and as protobuf, JSON and compact notation, each raw, deflated and gzipped. Written by `just measure vipaq` |
| [benchmarks/](benchmarks) | BenchmarkDotNet reports from `vipaq/bench`, copied by hand |

## 📄 One question each

| File | The question |
|---|---|
| [format-size.md](format-size.md) | How long is a ViPaq token stored as is, against protobuf, JSON and compact notation? |
| [compressed-size.md](compressed-size.md) | Which layout and codec should ViPaq use, and how long is a compressed token against the others? |
| [encode-decode-cost.md](encode-decode-cost.md) | What do encoding and decoding cost against protobuf, and what does compressing add? |
