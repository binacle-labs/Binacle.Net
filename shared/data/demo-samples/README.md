# Demo Samples

The demo site's sample set, converted to the tests-kernel compact scenario format. One entry per bin per
sample — 20 files, 51 entries. Read by the **tests kernel**, and by ViPaq, which packs them into placed
results under `vipaq/data/packed/demo-samples/`.

**This folder is the source. The demo reads from it, not the other way round.** Each file is one sample: its
entries are that sample's bins, and every entry repeats the same items. The file name carries the order, and
`01-opening-set.json` is the one the demo page opens on.

`just regen demo-samples` reassembles these files into `packages/binacle-net-ui/src/utils/sampleData.ts`,
which the demo imports. A file whose entries disagree on their items fails that run rather than being taken
from the first entry.

**Hand-authored, and every `Result` was measured.** Each was run against the API on all three algorithms, on
both the packing and the fitting endpoint. Nothing here was guessed.

## 🧾 Format

Same compact format as the [Bischoff suite](../bischoff-suite/README.md) and
[custom problems](../custom-problems/README.md):

```json
{
  "Name": "DemoSample_03_ThreeAnswers_60x40x20",
  "Bin": "60x40x20",
  "Metrics": "23400 48000 13 48.75",
  "Result": {
    "FFD": "PartiallyPacked PartiallyPacked",
    "BFD": "FullyPacked FullyPacked",
    "WFD": "PartiallyPacked PartiallyPacked"
  },
  "Items": ["12x10x15 [5]", "18x10x12 [4]", "8x15x12 [4]"]
}
```

Half the point of this set is that the three algorithms disagree, so every entry names all three — including
the ones where they agree. Why `Result` is keyed by algorithm and never by version is in the
[parent README](../README.md).

## ⚠️ It is a baseline, not an oracle

`Metrics` is arithmetic over `Bin` + `Items`, so it needs no packer. `Result` is different: it is **the
packer's own output, recorded**. Nobody worked out by hand that BFD should fully pack that 60x40x20 bin — the
API said so and the answer was written down.

That makes this set a **regression baseline**. It catches a change in behaviour, and it proves nothing about
correctness. The other two sets are the independent check: the Bischoff suite's expected outcome comes from
the instance, and custom problems are hand-authored. Read a failure here as "something moved", then go and
decide whether the move was right.
