---
description: Step 10 - JSON and compact-notation encoders join protobuf, then the ViPaq runner and reporters write encoded-size.md with its text columns and the README with the largest-token line
state: ready
waits-on: "step 9's gate"
horizon: next-release
paths: ["vipaq/measure/**", "vipaq/results/**", "vipaq/test/Binacle.ViPaq.Testing/**"]
---

# Step 10 - `Binacle.ViPaq.EncodedSize`

Shape: [results.md](results.md), the `Binacle.ViPaq.EncodedSize` bullets. Protocol: the orchestrator.

## The step

- `JsonEncoder` and `CompactEncoder` beside `ProtobufEncoder` in `Binacle.ViPaq.Testing`: the bin and the
  placed items as JSON and as compact notation - the text a user's token replaces. Protobuf stays the fair
  format comparison; "N% of the JSON" is the user's number. Step 14 puts JSON in the timing.
- `EncodingRunner` encodes every pack once per codec, layout and format and fills `EncodingBag`; one reporter
  per file. The runner shape step 8 settled in `Binacle.Reporting`.
- `encoded-size.md`: Scenario, Algorithm, Items, Widths, ViPaq raw / deflate / gzip, protobuf raw / deflate /
  gzip, JSON, Compact, ViPaq/Proto (under deflate), best codec, saved %. Base64 lengths for the binary
  formats, text lengths for JSON and compact. Two sections, one per layout, every pack. Replaces sixteen
  tables in five files.
- Every file opens with the header step 7 settled (recipe, project, pack count, families; no date).
- `README.md`: codec x layout table of mean / min / max ViPaq-to-protobuf ratio; the same per algorithm under
  deflate; stored length per format; the user's number - ViPaq deflate columnar as a share of JSON, compact
  and protobuf; crossover per layout (today logged to the console and written nowhere); codec win-count; the
  largest token - "every real pack deflates to at most N base64 characters"; links.
- The first run is the maintainer's.

## Open before starting

Settled 2026-09-20: the text bodies are the geometry only - bin and placed items, no IDs - counted as text,
not base64, and compact notation joins the comparison; `encoded-size.md` is whole, every pack, both layouts;
no byte counts anywhere.

## Done when

- [x] `find vipaq/test/Binacle.ViPaq.Testing -name JsonEncoder.cs -o -name CompactEncoder.cs | wc -l` is 2.
- [x] `ls vipaq/results` lists exactly `README.md encoded-size.md` (plus `benchmarks/` once step 13 lands).
- [x] `head -12 vipaq/results/encoded-size.md | grep -i json` hits, and so does `grep -i compact`.
- [x] `grep -n "base64 characters" vipaq/results/README.md` hits, with a number.
- [x] `grep -n -i "crossover" vipaq/results/README.md` hits, with a number per layout.
- [ ] A second run changes nothing: run, then `git status --short vipaq/results` is empty.
