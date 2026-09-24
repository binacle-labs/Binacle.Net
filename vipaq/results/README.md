# Encoded size results

What `just measure vipaq` writes: how long every real pack is as a ViPaq token in each codec and layout, and
as protobuf, JSON and compact notation beside it, each through the same three codecs so every number compares
with every other. The files are tracked, so a change in the format shows up as a diff. The rows are split
across 54 files because one table of 4,644 rows is more than GitHub will render, and because a file holding
one algorithm reads straight down.

## 📉 What the numbers say

A ViPaq token against the same packing stored another way. `Version = 0`, 2,322 real packings, both sides
through the same codec. Higher is better.

| ViPaq is smaller than | stored as is | compressed |
|---|---|---|
| JSON | **80%** | **54%** |
| compact notation | **42%** | **33%** |
| protobuf | **35%** | **42%** |

Compressed means deflate in the columnar layout - what the wire carries. Each figure is the average of the
per-pack figures, not a comparison of two averages.

- **Every packing fits in 704 characters.** The worst one, not the average. 98% fit in 512. The same packings
  as JSON reach 22,048.
- **The algorithm does not matter.** FFD, WFD and BFD move every figure above by less than two points.
- **The columnar layout is free.** Identical to row stored as is, 15% smaller once deflated. That is the only
  thing it is for.
- **Deflate is the right codec to have pinned.** It is smallest on all 2,322 packings. Gzip never is.

Two things that cut the other way. The JSON measured here is the readable kind - minified to one-letter keys
it would be 74%, and stripped to bare arrays it becomes compact notation, which is the 42% row. And the more
item shapes a packing holds the smaller the gap: against JSON, from 4.0% of its size on thpack1 to 9.0% on
thpack7.

## 📂 Files

| File | What it is |
|---|---|
| [encoded-size/](encoded-size) | Three tables per file - raw, deflate, gzip - one row per pack, with ViPaq, protobuf, JSON and compact side by side, and ViPaq against protobuf as a ratio. A folder per algorithm, and inside it one file per layout per group, named `<layout>-<group>.md` - the seven Bischoff sets, the custom problems and the demo samples |
| [benchmarks/](benchmarks) | Kept timing runs from `vipaq/bench`, and how to read them. Not written by `just measure` |

Speed and memory are not in this story yet. The kept runs in [benchmarks/](benchmarks) hold them.
