---
description: The scenario fixtures record which algorithms succeed, not how well - so a sample that exists because one algorithm packs better cannot say so
state: idea
waits-on: "nobody waiting"
paths:
  - "shared/data/**"
  - "shared/test/Binacle.TestsKernel/**"
---

# Fixtures record status, not how full the bin got

**A scenario's expected result is two statuses - packing and fitting.** Nothing in the format says how much
of the bin an algorithm actually filled.

**That is enough for most scenarios and not enough for some.** Where every algorithm packs the same set into
the same bin, the fill is the only thing separating them, and the fixture cannot see it.

## What it costs today

`shared/data/demo-samples/` has two entries that exist for a reason the data cannot express.

- The one built from `OrLibrary_thpack1_58`. All three algorithms return `PartiallyPacked`, so every result
  is identical. **It is in the set because WFD wins** - 84.33 against FFD's 80.43 and BFD's 77.83. In the file
  it is indistinguishable from any other partial pack.
- The one where BFD gets 96.22 where the others reach 77.94. Same shape of problem.

**Nothing catches a regression in either.** An algorithm could lose ten points of fill on both and every test
stays green.

## What it would take

A third value on the compact string, positional, the way `Metrics` already carries four:
`"{PackingStatus} {FittingStatus} {Fill}"`.

**Two fills exist and picking one is the decision, not the typing.** The API returns a bin fill and an items
fill. `results/lib/efficiency/` measures bin fill, and its README says to compare only within the same ruler -
so a second fill on a different ruler in the same repository is how a wrong comparison gets made later.

## What argues against it

**`results/lib/efficiency/` already tracks this, and over 700 scenarios rather than 20.** It holds the mean,
median and range per algorithm, plus the list of scenarios where BFD is at odds with the others. A fill in the
fixtures would be a regression pin on a handful of curated cases, not a new measurement.

**So the question is whether a pin is worth having when the ledger already exists.** The honest answer is that
they catch different things: the ledger says what the numbers are, and a pin says when one moved without
anyone meaning it to.

## Done when

- [ ] Either the fixtures carry a fill and the two samples above assert theirs, or there is a written decision
      that the efficiency ledger is the only place fill is tracked and this file is deleted.
