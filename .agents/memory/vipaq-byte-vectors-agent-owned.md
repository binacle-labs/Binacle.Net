---
name: vipaq-byte-vectors-agent-owned
description: ViPaq byte-exact golden vectors lay their bytes out by wire segment — a wall of hex nobody can check is not a test
type: convention
when: editing ViPaq byte-exact golden vectors
paths:
  - "vipaq/test-vectors/**"
---

ViPaq's hand-derived wire bytes — `vipaq/test-vectors/serialization/exact-bytes.json` and
`vipaq/test-vectors/protocol/little-endian/`, read by both suites — are written and maintained by the agent. They
are not hand-verified by the maintainer, so they have to justify themselves.

**Why:** byte-level tests are opaque to read. Without a derivation nobody can check them at all, and a wrong
vector locks in wrong behaviour for as long as it stays green.

**How to apply:** derive each vector from the spec, then lay it out by segment - header, count, bin, each item's
dims and coords, as `exact-bytes.json` does - so any single row can be spot-checked without recomputing the rest.
Never add a row you cannot explain. The wire is normative in `vipaq/PROTOCOL.md`; which vectors are generated and
which stay hand-authored is `$vipaq/decisions#D15`, still pending.
