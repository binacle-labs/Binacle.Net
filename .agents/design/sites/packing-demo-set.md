---
id: sites/packing-demo-set
description: Why the packing demo sizes its items against the largest bin, and how sizingBin and addBin relate - the reasoning behind the numbers a visitor arrives to
verified: 2026-08-27
check: largestBin and randomItemFor in packages/binacle-net-ui/src/utils/samples.ts, and sizingBin and addBin in packages/binacle-net-ui/src/core/packingDemo.ts - the bin each one picks is the claim that moves; the module's _sass/_theme.scss still matches sites/demo/_sass/_theme.scss once whitespace is stripped; utils/_itemMaterial.ts is still one shared MeshNormalMaterial
paths:
  - "packages/binacle-net-ui/**"
---

# The set the packing demo hands the visitor

**Permanent.** Two arguments about the shared packing component that are not obvious from reading it, and
that a later session would otherwise re-decide.

## Items are sized against the largest bin

`utils/samples.ts` `largestBin` picks the biggest bin by volume, and `randomItemFor` sizes each rolled item at
half that bin's sides. **So the set always fits at least one candidate.**

**That is the point, not a safety margin.** The smaller bins are the interesting result - the visitor is
meant to see the same items land differently, or not at all, depending on which bin they are given. A set
where nothing fits anywhere shows nothing, and the comparison between candidates is what the page exists to
show.

## `sizingBin` and `addBin` do not pick the same bin

`core/packingDemo.ts` `sizingBin()` returns `largestBin(...)`, or a fresh roll when there are no bins, so
nothing downstream has to handle an empty list. `addBin` copies the **last** bin instead, through
`Bin.copyOf`, which keeps the dimensions and takes the next free copy number.

**The link is that a copy keeps the footprint.** Adding a bin therefore never changes which bin is largest,
so what items are sized against does not move under the visitor mid-edit.

**They are still not the same bin.** Samples list their bins largest first, so the last bin is usually the
smallest one. Any statement that the sizing bin is also the one a new bin is copied from is wrong, and one
was written in a comment and later removed. **Whether `addBin` should copy the largest instead is a behaviour
question nobody has answered** - not a bug.

## The module does not ship an old palette

**Checked 26 Aug 2026.** A review said `Binacle.Net.UIModule` was still on a pre-contrast-pass dark palette
while the sites had moved on. It is not. `api/src/Binacle.Net.UIModule/_sass/_theme.scss` is byte identical
to `sites/demo/_sass/_theme.scss` once whitespace is stripped, `#3c5d8b` is also the dark `--primary` in
`sites/www/_sass/_tokens.scss`, and the module's `_components.scss` carries the same four contrast overrides
the demo site's does.

**If a contrast number is ever in doubt, measure it.** Do not compare hex strings across files that were
always meant to match - that is what produced the false finding.

## The pink and teal faces are not item colours

`utils/_itemMaterial.ts` is a single shared `MeshNormalMaterial`, which colours each face by the direction
its normal points. There is one kind of item and no categories anywhere in the API.

**Any change to how items are coloured is design work.** It is not a bug and there is nothing to fix.
