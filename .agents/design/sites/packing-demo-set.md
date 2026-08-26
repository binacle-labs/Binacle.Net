---
id: sites/packing-demo-set
description: Why the packing demo sizes its items against the largest bin, and how sizingBin and addBin relate - the reasoning behind the numbers a visitor arrives to
verified: 2026-08-27
check: largestBin and randomItemFor in packages/binacle-net-ui/src/utils/samples.ts, and sizingBin and addBin in packages/binacle-net-ui/src/core/packingDemo.ts - the bin each one picks is the claim that moves
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
