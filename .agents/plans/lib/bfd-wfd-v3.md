---
description: A test-only v3 of BFD and WFD finds a space in one scan instead of a sort - same fill as v2, far faster on the few cases timed; held for more testing before it could ship
state: idea
waits-on: "the maintainer - held for more testing"
horizon: undecided
paths:
  - "lib/src/Binacle.Lib/Algorithms/**"
  - "lib/src/Binacle.Lib/AlgorithmFactory.cs"
---

# BFD and WFD v3

Built 2026-09-27 while chasing slow processes. v3 differs from v2 in one method: `FindAvailableSpace` scans the
free spaces once for the smallest (BFD) or largest (WFD) that fits, where v2 sorts them on every placement. A
tie goes to the earliest space in the list, as in v1.

It lives in `lib/src/Binacle.Lib/Algorithms/<Heuristic> v3/`. The unit tests and `just measure lib` run it. No
factory under `lib/src/` creates it, so the API runs v2. No bench runs it; it was added to the algorithms bench
by hand for the runs below and taken out.

The maintainer, 2026-09-27: "no we hold v3 we need more testing", then "it needs to exist in lib and unit tests
and in version parity... nmo benchmarks for now".

**What is known.** It fills every Bischoff problem exactly as v2 (`lib/results/measurements/version-parity.md`)
and allocates the same. On the four smoke scenarios, packing, it took well under half of v2's time where there
are many free spaces; BFD v3 took about FFD's time. The numbers are in the lib findings record.

**What is not known yet.**

- Its time over every Bischoff problem - the full algorithms bench with v3 added.
- Whether it puts every item where v2 does, not just the same fill.
- Its processes still split into two speeds under tiered PGO.

Shipping it means `AlgorithmFactory` hands out BFD v3 and WFD v3, and the results files move from v2 to v3.

## Done when

- [ ] v3 is timed over every Bischoff problem against v2, and the maintainer has said ship or drop.
      **By eye.** A kept full run with v3 rows under `lib/results/benchmarks/`, and his word written here.
