---
description: Refresh the curated lib benchmark ledger
state: blocked
waits-on: "an unanswered question - where benchmark and performance results live and what shape they take. The maintainer keeps them in results/ and does not like them there"
paths:
  - "lib/**"
  - "results/lib/**"
---

# Refresh the curated lib benchmark ledger

**Status:** Blocked, not started. The committed lib benchmark results are stale, not just old.

**The blocker is the same one that holds the `tooling/*.sh` conversion plan**, and it is one decision, not
two: **where benchmark and performance results are persisted, and what shape they take.** They sit in
`results/` today and the maintainer does not want them there. Re-running and curating a keeper into
`results/lib/benchmarks/` under the current arrangement would add to the pile he is trying to move.

## Why
`results/lib/benchmarks/` stops at 2025-02-10 while `lib/src` has moved on — including the geometry migration,
which moved `Dimensions` / `Coordinates` across an assembly boundary. Those numbers describe code that no longer
exists; do not quote them until they are re-run. (`BestBin_v2` once measured 5–9× faster than v1, 24 B vs
208–336 B allocated — unconfirmed against current code.)

## What, once the question is answered
- Re-run the lib benchmarks against current code.
- Curate a keeper into `results/lib/benchmarks/`. The vault is hand-curated — harnesses write to gitignored
  scratch, never straight into `results/`; diff the scratch against the committed files and copy the keeper in
  by hand.
- Algorithm racing was re-measured 2026-07-17 and the evidence lives in the lib design findings; its scratch reports are
  in `BenchmarkDotNet.Artifacts/` and a keeper should be curated in.
