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
| [format-size.md](format-size.md) | How much smaller is a ViPaq token than JSON, compact notation and protobuf, stored as is? |
| [compressed-size.md](compressed-size.md) | Compressed, how much smaller is it, and which layout and codec should be used? |
| [encode-cost.md](encode-cost.md) | What does encoding cost in time and memory against protobuf, and what does compressing add? |
| [decode-cost.md](decode-cost.md) | What does decoding cost in time and memory against protobuf, and what does decompressing add? |

Each one reads the raw results above. Nothing at this level is generated.
