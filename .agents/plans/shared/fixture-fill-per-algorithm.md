---
description: The scenario fixtures record which algorithms succeed, not how full the bin got
state: idea
waits-on: "nobody - wanted sometime"
paths:
  - "shared/data/**"
  - "shared/test/Binacle.TestsKernel/**"
---

# Fixtures record status, not how full the bin got

A scenario's expected result is two statuses, packing and fitting. Where every algorithm packs the same set
into the same bin, the fill is the only thing separating them and the fixture cannot see it.

**Two entries in `shared/data/demo-samples/` exist for exactly that reason** and read as identical to any
other partial pack: one where WFD wins on 84.33 against FFD 80.43 and BFD 77.83, and one where BFD gets 96.22
against 77.94. An algorithm could lose ten points on both and every test stays green.

A third positional value on the compact string would do it. **The decision is which fill**, since the API
returns a bin fill and an items fill, and `results/lib/efficiency/` measures bin fill over 700+ scenarios.

**What argues against it:** that ledger already tracks this, over far more scenarios. The counter is that they
catch different things - the ledger says what the numbers are, a pin says when one moved without anyone
meaning it to.
