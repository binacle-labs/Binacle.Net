# ViPaq results

What ViPaq's measure and bench projects found.

<!-- Summary: shape not decided yet. -->

## 📂 What is in it

| Path | What it is |
|---|---|
| [measurements/](measurements) | Raw measurement results, written by `just measure vipaq`: how long every real pack is as a ViPaq token, and as protobuf, JSON and compact notation, each raw, deflated and gzipped |
| [benchmarks/](benchmarks) | Raw benchmark results: BenchmarkDotNet reports from `vipaq/bench`, copied by hand. Not written by `just measure` |
| [format-size.md](format-size.md) | ViPaq against JSON, compact notation and protobuf, uncompressed |
| [row-deflate.md](row-deflate.md), [row-gzip.md](row-gzip.md) | Row layout, compressed, against protobuf and JSON |
| [columnar-deflate.md](columnar-deflate.md), [columnar-gzip.md](columnar-gzip.md) | Columnar layout, compressed, against protobuf and JSON |
| [encode-cost.md](encode-cost.md) | Encode time and memory against protobuf, and what compressing adds |
| [decode-cost.md](decode-cost.md) | Decode time and memory against protobuf, and what decompressing adds |

Each file at this level answers one question from the two folders.

## 🛠️ How you use it

```
just measure vipaq    # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```

## ⚠️ What will bite you

Every size is a character count, not bytes: ViPaq and protobuf as base64, JSON and compact notation as text,
because text is their own stored form.
