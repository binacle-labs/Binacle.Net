# ViPaq results

What ViPaq's measure and bench projects found, as raw files and as the story read out of them.

## 📂 What is in it

| Path | What it is |
|---|---|
| [measurements/](measurements) | Raw measurement results, written by `just measure vipaq`: how long every real pack is as a ViPaq token, and as protobuf, JSON and compact notation, each raw, deflated and gzipped |
| [benchmarks/](benchmarks) | Raw benchmark results: BenchmarkDotNet reports from `vipaq/bench`, copied by hand. Not written by `just measure` |

Files at this level are derived from the two folders. Each tells one part of the story with numbers, and is
rewritten when the raw files move. Do not edit them by hand.

## 🛠️ How you use it

```
just measure vipaq    # rewrites measurements/
just bench            # the list of timing runs, with what each one costs
```

## ⚠️ What will bite you

Every size is a character count, not bytes: ViPaq and protobuf as base64, JSON and compact notation as text,
because text is their own stored form.
