---
description: TestsKernel - grow the shared fixture cases
state: idea
waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
horizon: future
paths:
  - "shared/**"
---

# TestsKernel - grow the shared fixture cases

The data move is done and the fixtures are split by consumer. What is left is growing the cases: result
selection has the thinnest coverage in the repository, a single `baseline.json` per case, and `custom-problems`
is the only hand-authored set, so it is the only place a real 8-bit or uncompressed-16-bit problem can come
from. A new JSON file dropped into the right data folder is picked up automatically, so growing coverage is
authoring files - but each one is a piece of reasoning, not a paste, because the expected result is asserted.

## Research

### 2026-08-13 - the data move landed and the fixtures were split by consumer

The Bischoff suite and custom-problems stay in `shared/data/` because more than one slice reads them.
Result-selection moved to `lib/data/result-selection/` and is embedded by `lib/test/Binacle.Lib.TestsKernel`,
whose manifest prefix is `ResultSelection.`.

### 2026-08-26 - the demo's sample set is fixture data

`shared/data/demo-samples/` holds 20 files, one per sample, 51 entries. Every `Result` was measured against a
live API on all three algorithms and both endpoints. ViPaq packs them too, into
`vipaq/data/packed/demo-samples/`.

**The direction settled the other way from what was first proposed.** The data is the source and the demo reads
it: `just regen demo-samples` reassembles the files into
`packages/binacle-net-ui/src/utils/sampleData.ts`. Nothing reads the demo package to write the data.

### 2026-08-27 - the maintainer said "not yet" on growing the cases

Nothing here is wrong; it was deliberately not being done.

### Date not recorded - it collides with the heavy architecture tools

ArchUnitNET and its xunit v3 adapter both touch the tests, so whichever runs second reads the other's result,
and the `xunit.v3.mtp-v2` pin in `Directory.Packages.props` bites both.

### Date not recorded - what growing result selection would add

A single `baseline.json` per case today (BestAlgorithm, BestBin, SmallestBin).

- Add scenarios that exercise the tie-breaks and edge picks each selector is meant to make: equal-fit bins
  where the smallest wins; algorithms that tie on fit but differ on efficiency; a bin that only one algorithm
  can fill. Name cases so the intent is obvious.
- Cross-check against the selectors in `lib/src/Binacle.Lib/ResultSelection/` so every branch has a scenario.
  The fixtures and their providers are in `lib/data/result-selection/` and `lib/test/Binacle.Lib.TestsKernel/`.

### Date not recorded - what growing `custom-problems` would add

Bischoff is seven fixed instances, all 16-bit, all `PartiallyPacked`, so it cannot supply any of these. Adding
a problem here reaches lib's algorithm tests **and** ViPaq's packed data, regenerated from these definitions,
so one addition serves both.

- **8-bit coverage.** Every Bischoff pack is 16-bit, coordinates to ~587. The only 8-bit scenario is a custom
  pack, and ViPaq's curated Bischoff slice is all 16-bit, so a real, size-measured 8-bit problem has to come
  from here. Benchmarks get 8-bit from `SyntheticDataProvider`; this is about real, measured data.
- **Uncompressed 16-bit coverage.** ViPaq's uncompressed set is all 8-bit - every 16-bit problem is big enough
  that ViPaq compresses it, so there is no uncompressed-16-bit scenario to size or benchmark. Author a small
  16-bit problem: coordinates over 255 but few enough items to stay under the compression threshold. 16-bit
  body is `2 + 6 + items*(3*2 + 3*2)` bytes and must be under 255, so about 20 items.
- **A count ladder.** One problem family at ~5, ~13, ~50, ~200 items, with **only the item count changing**.
  This pins ViPaq's compression-crossover report, which is otherwise provisional - "8-bit crosses somewhere
  between 16 and 100 items".
- **Shape variety.** `simple`, `complex` and `baseline` are small and same-ish. Consider varied bin sizes, a
  single-item bin, and a near-perfect tessellation.

Consider whether Bischoff already covers the algorithm cases well enough that `custom-problems` can stay small
and targeted, growing only for the reasons above.

### Date not recorded - the cost, before you start

Each scenario carries `Metrics` and `Result`, and both are *asserted*:

```json
{ "Name": "...", "Bin": "60x40x10", "Metrics": "125 24000 1 0.5",
  "Result": { "FFD": "FullyPacked FullyPacked", "WFD": "FullyPacked FullyPacked",
              "BFD": "FullyPacked FullyPacked" },
  "Items": ["5x5x5 [1]"] }
```

`Metrics` is pure arithmetic - items volume, bin volume, item count, fill percent - and needs no packer.
`Result` is the **expected** outcome keyed by algorithm, and the tests run the real packer and check against
it. So you must know what each algorithm will do before you write the file.

### Date not recorded - keep manifest names exact

A fixture filename must have no extra dots beyond the extension. The embedder splits the dotted path on `.`,
so an extra dot corrupts the manifest name, and a broken manifest fails silently. Verify with
`strings <dll> | grep <prefix>` after building.

Provenance and the thpack1-7 versus thpack8/9 caveat live in the `README.md` beside each data folder. Read
those before touching the data.
