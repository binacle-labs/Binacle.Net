---
description: Step 10 - a JSON encoder joins protobuf, then the ViPaq runner and builder write encoded-size.md with its JSON column and the README with the largest-token line
state: ready
waits-on: "step 9's gate"
horizon: next-release
paths: ["vipaq/measure/**", "vipaq/results/**", "vipaq/test/Binacle.ViPaq.Testing/**"]
---

# Step 10 - `Binacle.ViPaq.EncodedSize`

Shape: [results.md](results.md), the `Binacle.ViPaq.EncodedSize` bullets. Protocol: the orchestrator.

## The step

- A `JsonEncoder` beside `ProtobufEncoder` in `Binacle.ViPaq.Testing`: the plain JSON body a user's token
  replaces. Protobuf stays the fair format comparison; "N% of the JSON body" is the user's number. Step 14
  puts it in the timing.
- One runner encodes every pack once per codec, layout and format - ViPaq, protobuf, JSON - and holds the
  base64 lengths; one builder writes two files. Uses the runner shape step 8 settled in `Binacle.Reporting`.
- `encoded-size.md`: Scenario, Algorithm, Items, Widths, ViPaq raw / deflate / gzip, protobuf raw / deflate /
  gzip, JSON, ratio, best codec, saved %. Base64 lengths only. Two sections, one per layout. Replaces
  sixteen tables in five files.
- Every file opens with the header step 7 settled (recipe, project, pack count, families; no date).
- `README.md`: codec x layout table of mean / min / max ViPaq-to-protobuf ratio; the same per algorithm;
  crossover item count per layout (today logged to the console and written nowhere); codec win-count; the
  JSON row; the largest token - "every real pack deflates to under N base64 characters"; links.
- The first run is the maintainer's.

## Open before starting

- What the JSON body looks like - the shape the API returns, or the minimal bin-plus-items object. The
  user's number is only honest if it is the body a user would actually send.
- `encoded-size.md` at 4,300 rows and ~500 KB. The shape leaves it whole; the alternative noted there is
  one algorithm's packs raw and per-algorithm means in the README. Decide before writing the builder.
- Whether bytes appear anywhere. The shape says base64 only, bytes are 3/4 of it.

## Done when

- [ ] `find vipaq/test/Binacle.ViPaq.Testing -name JsonEncoder.cs | grep -q .`
- [ ] `ls vipaq/results` lists exactly `README.md encoded-size.md` (plus `benchmarks/` once step 13 lands).
- [ ] `head -12 vipaq/results/encoded-size.md | grep -i json` hits.
- [ ] `grep -n "base64 characters" vipaq/results/README.md` hits, with a number.
- [ ] `grep -n -i "crossover" vipaq/results/README.md` hits, with a number per layout.
- [ ] A second run changes nothing: run, then `git status --short vipaq/results` is empty.
