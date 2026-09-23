# Encoded size results

What `just measure vipaq` writes: how long every real pack is as a ViPaq token in each codec and layout, and
as protobuf, JSON and compact notation beside it, each through the same three codecs so every number compares
with every other. The files are tracked, so a change in the format shows up as
a diff. The rows are split across 54 files because one table of 4,644 rows is more than GitHub will render, and
because a file holding one algorithm reads straight down.

## 📂 Files

| File | What it is |
|---|---|
| [encoded-size/](encoded-size) | Three tables per file - raw, deflate, gzip - one row per pack, with ViPaq, protobuf, JSON and compact side by side, and ViPaq against protobuf as a ratio. A folder per algorithm, and inside it one file per layout per group, named `<layout>-<group>.md` - the seven Bischoff sets, the custom problems and the demo samples |
| [benchmarks/](benchmarks) | Kept timing runs from `vipaq/bench`, and how to read them. Not written by `just measure` |

The summary of what these numbers say is not written yet.
