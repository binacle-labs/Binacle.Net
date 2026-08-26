# Packed placed-result data

Frozen, packed results read by the ViPaq test kernel's `BischoffDataProvider`, `CustomProblemsDataProvider`
and `DemoSamplesDataProvider` (the first two merged for curated runs by `CuratedScenarioProvider`). Each
sample is a bin plus the **placed** items a packing run produced — dimensions **and** coordinates
(`L x W x H (X,Y,Z)`) — which is what ViPaq serializes. The source problems carry only item *types* with a
quantity and no coordinates, so the coordinates only exist after packing.

## 🚫 Generated — do not hand-edit

These files are produced by `vipaq/tools/Binacle.ViPaq.PackedDataGenerator`. To change them, edit the source
problems (`shared/data/bischoff-suite`, `shared/data/custom-problems`, `shared/data/demo-samples`) or the
tool, then regenerate:

```
just regen vipaq-packed-data
```

`shared/data/bischoff-suite` is itself generated, so if you changed the OR-Library conversion run `just regen
all` instead - it does both, in that order.

The run is deterministic: a no-change re-run is byte-identical, so it produces no git noise. `just regen check`
regenerates everything and fails if any generated file moved.

## 📂 Layout

Split by source family, mirroring `shared/data`:

- `custom-problems/` — `baseline`, `complex`, `simple`.
- `bischoff-suite/` — `orlib_thpack1` .. `orlib_thpack7` (BR1–BR7).
- `demo-samples/` — the demo site's sample set, `01-opening-set` .. `20-wfd-wins`.

The **algorithm** rides on the file name as a `.<algo>` suffix, not a folder — e.g. `orlib_thpack1.ffd.json`.
Every algorithm the packer offers is generated: `.ffd.json`, `.wfd.json` and `.bfd.json`, side by side in the
same folder. The suffix names the family, not the implementation version — the tool packs through the same
factory the API uses, which is the v2 implementation of each. Different algorithms place items differently, so
their coordinates — and tokens — differ; the suffix keeps the sets apart without duplicating the folder tree.

The test kernel reads a sample's name as `<problem>.<algo>`, so the same problem under three algorithms is
three distinct scenarios.

The tool prints a per-file and total sample/item count on each run; that console summary is the run's report,
so there is no committed index file to keep in sync.

## 🧾 File format

Each problem file is a JSON array of samples. One sample:

```json
{
  "Name": "OrLibrary_thpack1_1",
  "WidthBits": 16,
  "Bin": "587x233x220",
  "Items": ["92x81x55 (0,0,0)", "92x81x55 (92,0,0)"]
}
```

- `Name` — the source problem's name.
- `WidthBits` — the width family: `8` if every bin dimension, item dimension and coordinate fits in a byte,
  else `16`. ViPaq still chooses the actual per-section width from the values at encode time; this is only a
  label for grouping.
- `Bin` — the container as `"LxWxH"`.
- `Items` — the placed items as `"LxWxH (X,Y,Z)"`. Bischoff instances are `PartiallyPacked` by design (they
  fill ~98%, never tessellate perfectly), so not every source box appears here — only the placed ones.

Only placed geometry is stored — no ViPaq token. The token is derivable from `Bin`+`Items`, and its compressed
bytes vary by gzip encoder/runtime, so committing it would churn the files on every regen. The kernel computes
the token itself when it benchmarks. Every sample is still round-tripped (encode → decode == input) at
generation time, or the run fails.
