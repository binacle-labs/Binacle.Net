# Encoded size results

What `just measure vipaq` writes: how long every real pack is as a ViPaq token in each codec and layout, and
as protobuf, JSON and compact notation beside it. The files are tracked, so a change in the format shows up as
a diff.

## 📂 Files

| File | What it is |
|---|---|
| [encoded-size.md](encoded-size.md) | One row per pack per layout: every format's size, the ratio, the best codec |
| [benchmarks/](benchmarks) | Kept timing runs from `vipaq/bench`, and how to read them. Not written by `just measure` |

The summary of what these numbers say is not written yet.
