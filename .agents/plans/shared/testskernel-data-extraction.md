---
description: TestsKernel - grow the shared fixture cases
state: deferred
waits-on: "the maintainer - he said \"not yet\" on 2026-08-27; it revives when he says the fixture cases are worth growing"
paths:
  - "shared/**"
---

# TestsKernel — grow the shared fixture cases

**Not yet - 2026-08-27.** The maintainer's answer on growing the cases was *"not yet"*. Nothing below is
wrong; it is deliberately not being done. **What revives it is him saying so.**

**Status (2026-08-13):** The data move is **done**, and the fixtures have since been split by consumer. Bischoff
suite and custom-problems stay in `shared/data/` because more than one slice reads them; result-selection moved to
`lib/data/result-selection/` and is embedded by the new `lib/test/Binacle.Lib.TestsKernel`, whose manifest prefix
is `ResultSelection.`. Only the "review and grow the cases" work below remains. Delete this file when nothing
pending is left.

**It collides with the heavy architecture tools** - ArchUnitNET and its xunit v3 adapter. Both touch the test
leaves, so whichever runs second reads the other's result, and the `xunit.v3.mtp-v2` pin in
`Directory.Packages.props` bites both.

A new JSON file dropped into the right data folder is picked up automatically (each `.csproj` embeds its set with a
`*.json` glob), so growing coverage is just authoring files. Provenance and the thpack1–7 vs thpack8/9 caveat live
in the `README.md` beside each data folder — read those before touching the data.

## Done 2026-08-26 — the demo's sample set is fixture data

`shared/data/demo-samples/` holds 20 files, one per sample, 51 entries. Every `Result` was measured against a
live API on all three algorithms and both endpoints. ViPaq packs them too, into
`vipaq/data/packed/demo-samples/`.

**The direction settled the other way from what this section first proposed.** The data is the source and the
demo reads it: `just regen demo-samples` reassembles the files into
`packages/binacle-net-ui/src/utils/sampleData.ts`. Nothing reads the demo package to write the data.

**Nothing under this heading is open. It is here so the next reader knows it landed** rather than re-deriving
it, and it goes when this file does.

## Pending — review and grow result-selection

Result selection has the thinnest coverage: a single `baseline.json` per case (BestAlgorithm, BestBin, SmallestBin).

- Add scenarios that exercise the tie-breaks and edge picks each selector is meant to make (e.g. equal-fit bins
  where the smallest wins; algorithms that tie on fit but differ on efficiency; a bin that only one algorithm can
  fill). Name cases so the intent is obvious.
- Cross-check against the selectors in `lib/src/Binacle.Lib/ResultSelection/` so every branch has a scenario.
  The fixtures and their providers are in `lib/data/result-selection/` and `lib/test/Binacle.Lib.TestsKernel/`.

## Pending — add more problems to `custom-problems`

Bischoff is seven fixed instances, all 16-bit, all `PartiallyPacked`, so it cannot supply these. `custom-problems`
is the only hand-authored set, and adding a problem here reaches lib's algorithm tests **and** ViPaq's packed data
(regenerated from these definitions), so one addition serves both.

- **8-bit coverage.** Every Bischoff pack is 16-bit (coordinates to ~587). The only 8-bit scenario is a custom
  pack, and ViPaq's curated Bischoff slice is all 16-bit, so a real, size-measured 8-bit problem has to come from
  here. (Benchmarks get 8-bit from `SyntheticDataProvider`; this is about real, measured data.)
- **Uncompressed 16-bit coverage.** ViPaq's uncompressed set is all 8-bit — every 16-bit problem (Bischoff) is big
  enough that ViPaq compresses it, so there is no uncompressed-16-bit scenario to size or benchmark. Author a small
  16-bit problem — coordinates over 255 but few enough items to stay under the compression threshold (16-bit body:
  `2 + 6 + items*(3*2 + 3*2)` bytes ≤ 255 → ~20 items). See the ViPaq design findings.
- **A count ladder.** One problem family at ~5, ~13, ~50, ~200 items, with **only the item count changing**. This
  pins ViPaq's compression-crossover report, which is otherwise provisional ("8-bit crosses somewhere between 16
  and 100 items").
- **Shape variety.** `simple`/`complex`/`baseline` are small and same-ish. Consider varied bin sizes, a single-item
  bin, and a near-perfect tessellation, so the algorithm tests exercise more than one regime.

**The cost, before you start.** Each scenario carries `Metrics` and `Result`, and both are *asserted*:

```json
{ "Name": "...", "Bin": "60x40x10", "Metrics": "125 24000 1 0.5",
  "Result": { "FFD": "FullyPacked FullyPacked", "WFD": "FullyPacked FullyPacked",
              "BFD": "FullyPacked FullyPacked" },
  "Items": ["5x5x5 [1]"] }
```

`Metrics` is pure arithmetic (items volume, bin volume, item count, fill %) — computable, no packer needed.
`Result` is the **expected** outcome, keyed by algorithm, and the tests run the real packer and check against it.
So you must know what each algorithm will do before you write the file — a new problem is a small piece of
reasoning, not a paste.

Consider whether Bischoff already covers the algorithm cases well enough that `custom-problems` can stay small and
targeted, growing only for the reasons above.

## Watch out

- **Keep manifest names exact.** A fixture filename must have no extra dots beyond the extension — the embedder
  splits the dotted path on `.`, so an extra dot corrupts the manifest name. Verify with
  `strings <dll> | grep <prefix>` after building.
- **Never commit** — leave changes in the working tree for the human.
- Trim this plan as each item lands; delete the file when nothing pending remains.
