---
description: The testing techniques this repo does not use - property-based, fuzzing, load, mutation - and the four yes-or-no answers
state: idea
waits-on: "nobody - it is an idea"
horizon: undecided
---

# Testing techniques not in use

The suite is a real pyramid and classicist in style - real objects, real Azurite, mock almost nothing. What is
missing is a *technique* axis, not a scope one: property-based testing, fuzzing the ViPaq decoder, load
testing, and mutation testing on result selection. Each is a separate yes or no, and the first is the strong
one, because 3D packing has invariants no example test can carry.

## Research

### Date not recorded - property-based testing, the strong one

Assert an invariant, let the tool generate inputs trying to break it. CsCheck or FsCheck; Bogus already
generates data here. 3D packing is close to the ideal domain: no two packed items overlap, every item lies
inside the bin, `packed + unpacked == input`, same input gives the same result. **An overlap bug is silent** -
the API returns 200 with a plausible packing and it surfaces as a complaint months later.

**Open:** which invariants hold for *fit* as well as *pack*, since fit exits early.

### Date not recorded - fuzzing the ViPaq decoder

`ViPaqSerializer.deserialize` parses untrusted base64 and is the only real attack surface here. The property is
simple: any garbage throws `ViPaqFormatError`, never hangs, never OOMs. SharpFuzz.

**Open:** is it reachable from a public endpoint, which decides whether this is security or hygiene.

### Date not recorded - load testing

A 400-bin `pack/compare-bins` measured **31.5s** single-threaded and nothing is known under concurrency.

**Open:** there is no stated target, and without one this measures without deciding anything.

### Date not recorded - mutation testing on `ResultSelection/`

Coverage says a line ran; mutation says whether a test would notice it being wrong. `BestBin_v2.cs:24` adds a
magic `1000` to rank fully-packed bins first - that constant is a business rule, and a wrong bin choice returns
200 looking fine. Start there, five small files, and cost one run: the lib suite is ~8,679 tests and Stryker
reruns them per mutant. **Do not chase the score** and do not point it at the UI or Diagnostics modules. If it
scores well the idea closes as "checked, not needed".

### Date not recorded - three rejected

Playwright E2E, for brittleness - the same reasoning that leaves `UIModule` at 0% on purpose. Pact contract
testing, because both clients are in this repo. Verify snapshots, because the ViPaq vectors already do this,
better.
