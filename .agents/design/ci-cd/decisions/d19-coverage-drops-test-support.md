---
id: ci-cd/decisions/D19
description: the merged coverage report drops the test-support assemblies
status: pending
verified: 2026-09-29
paths:
  - ".netconfig"
  - "tooling/coverage.just"
---

# D19 — the merged coverage report drops the test-support assemblies

`.netconfig` at the repo root, reportgenerator's own config file, carries
`assemblyfilters = "-*.Data;-*.Testing;-Binacle.Reporting;-*.UnitTests;-*.IntegrationTests"`.
Both `just coverage` recipes read it.

**Why:** a test helper scoring itself says nothing about shipped code. `*.Data` and `*.Testing` are patterns
rather than names, so adding a data project or a test support library needs no edit. `-*.TestsKernel` was on
the list until the last kernel went (2026-09-20).

**Why `Binacle.Reporting` is named even though it is not in the report today.** It only reaches it if a suite
that runs under coverage starts referencing it. Naming it now means that day is silent, instead of moving the
denominator with nobody noticing.
