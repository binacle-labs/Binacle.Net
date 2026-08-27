---
description: The testing techniques this repo does not use - property-based, fuzzing, load, mutation - what each buys, and the four yes-or-no answers
state: idea
waits-on: "nobody waiting - future"
---

# Idea: testing techniques not in use

**Status:** Unvetted. A survey of what exists, what is already here, and what might be worth adopting.
Nothing decided. Kept because the reasoning is easy to lose and expensive to re-derive.

## What is already here

More than it looks like from the outside:

| Technique | Where |
|---|---|
| Unit (example-based) | `lib/test` ~8,679 tests · xunit.v3 + Shouldly |
| Integration | `api/test` 622 + 106 · `WebApplicationFactory`, real Azurite |
| Golden / vector | `vipaq/test-vectors/` — byte-exact, committed |
| Cross-language interop | the same vectors replayed in C# **and** TS — genuinely rare |
| Benchmarks | BenchmarkDotNet (the lib design findings) |
| Performance harness | `Binacle.ViPaq.PerformanceTests` — custom, Serilog-reported |
| Fake data generation | Bogus (C#) · faker-js (TS) |

Shape is a real **test pyramid** — many unit, fewer integration, no E2E. Most projects drift the other way
(the "ice cream cone": mostly manual and E2E, slow, flaky, distrusted). Nothing to fix here.

The style is **classicist** ("Chicago"), not **mockist** ("London"): real objects, real Azurite, mock almost
nothing. That is why `ServiceModule.Domain` gets 83.7% line coverage *through* the API tests instead of
needing its own mocked suite. It is also why the suite fails when Azurite is down — the cost of realism, and
the right trade.

## The axis that matters: technique, not scope

Scope (unit → integration → E2E) is the familiar axis, and it is a straight cost curve: going up buys
**realism** and pays in **speed, flakiness, and vague failures**. That one is settled here.

The interesting axis is *how* a test decides what is right:

**Example-based** (have) — pick inputs, assert outputs.
*Good:* concrete, readable, debuggable. *Bad:* only tests the cases someone thought of, and bugs live in the
others. *Trade:* precision vs imagination.

**Golden / vector** (have) — freeze known-good output, compare exactly.
*Good:* unbeatable for wire formats and cross-language agreement. *Bad:* says output *changed*, never why it
is wrong; re-blessing a broken file is one command. *Trade:* exactness vs blind acceptance.

**Property-based** — assert an invariant, generate inputs trying to break it.
*Good:* finds what nobody would think of; shrinks a failure to a minimal case. *Bad:* invariants are hard to
write; failures can be cryptic; slower. *Trade:* concrete examples vs coverage of the input *space*.

**Fuzzing** — throw malformed input at a parser.
*Good:* finds crashes, hangs, OOMs on hostile input. *Bad:* finds crashes only, not wrong answers, unless
assertions are added. *Trade:* cheap to run, narrow in what it catches.

**Mutation** — break the code, see if a test notices.
*Good:* the only one that measures *assertions*. *Bad:* slow; equivalent mutants cap the score.

## Candidates

### 1. Property-based testing — the strong one

Assert an **invariant**, let the tool generate hundreds of random inputs trying to break it. Tools: CsCheck
or FsCheck. Bogus already generates data here, so the step is small: assert a *rule* instead of an expected
value.

3D bin packing is close to the ideal domain, because the invariants are hard and no example test can carry
them:

- **No two packed items overlap.** ~8,679 example tests cannot prove this. One property can.
- **Every packed item lies entirely inside the bin.**
- **Conservation:** `packed + unpacked == input` — no item invented, none lost.
- **Determinism:** same input + same algorithm → same result.
- **Necessary condition:** if fully packed, `sum(item volumes) <= bin volume`.

Why it beats writing more examples: an overlap bug is **silent**. The API returns 200 with a plausible
packing, and it surfaces months later as a customer complaint. Example tests check the answers someone
predicted; a property checks the ones nobody did.

**Open:** which invariants are actually true for *fit* vs *pack* (fit exits early — the concepts doc)? Is
generating a valid random bin+items set cheap enough to be worth it? Do failures shrink to something
readable, or to noise?

### 2. Fuzzing the ViPaq decoder

`ViPaqSerializer.deserialize` parses **untrusted input** — a base64 token handed over by a user. It is the
only real attack surface in the repo. The `DecodeInvalid` vectors are hand-authored, so they cover the
malformations someone imagined.

The property is simple: **any** garbage must throw `ViPaqFormatError` — never hang, never OOM, never throw
something else. A decoder that hangs on a crafted token is a denial of service. Tool: SharpFuzz.

**Open:** is this reachable from a public endpoint today, or only from stored tokens? That decides whether it
is security work or hygiene.

### 3. Load testing the API

The pitch says "real time"; that claim is untested under concurrency. A 400-bin `pack/compare-bins` measured
**31.5s** single-threaded. Nothing is known about it under parallel load. Tools: NBomber, k6.

**Open:** is there a stated target to test against? Without one this measures without deciding anything.

### 4. Mutation testing on `ResultSelection/`

Coverage answers "did this line run?". Mutation testing answers "if this line were **wrong**, would any test
fail?" It flips `>` to `>=`, `&&` to `||`, deletes a statement, reruns the suite, and reports each mutant as
**killed** (a test failed, good) or **survived** (covered but never actually checked). Tool:
`dotnet-stryker`.

`lib/src/Binacle.Lib/ResultSelection/` is the natural first target, and the only one worth trying:

- It reports ~88% line and ~88% branch. Mutation testing says whether that is real.
- `BestBin_v2.cs:24` adds a magic `1000` to rank fully-packed bins first. That constant **is** a business
  rule. Coverage proves the line runs; it cannot prove a test would notice if the rule inverted.
- Its failures are **silent**. A wrong bin choice returns 200 and looks fine.

The manual version already works: deleting the cancellation guard in `LoopBinProcessor` failed exactly two
tests, which is what proved the guard was tested rather than merely covered.

**Open:** the lib suite is ~8,679 tests and Stryker reruns them per mutant, so whole-repo is likely hours -
start at `ResultSelection/` (five small files) and see what one run costs. If it scores well the idea closes
as "checked, not needed", which is a fine outcome. `.config/dotnet-tools.json` already pins ReportGenerator
and the Sonar scanner, so a local pin would match - only worth it if it is run more than once. Not in CI.

**Do not chase the score.** Equivalent mutants cap it, and it games the same way coverage does. Do not point
it at `UIModule` or `DiagnosticsModule` - loud failures, low value, the same reasoning that leaves their
coverage alone.

## Probably not worth it

- **E2E (Playwright).** Aimed at the demo UI. Brittle, slow, breaks on markup changes — the same
  reasoning that leaves `UIModule` at 0% coverage on purpose.
- **Contract testing (Pact).** It earns its keep when *other teams* consume the API on their own release
  cycle. Both clients are in this repo; the integration tests already cover the contract.
- **Snapshot testing (Verify).** The same idea as golden vectors, aimed at objects. The vectors already do
  this where it matters, and better.

## Done when

Each candidate ends in a yes or a no. Nothing here is started until one gets a yes.

- [ ] Property-based testing has an answer.
      **By eye.** Either an invariant suite exists under `lib/test/`, or this section says it was tried and
      what it cost.
- [ ] Fuzzing the ViPaq decoder has an answer, and the security question under it is settled first.
      **By eye.** Whether a decoder token reaches a public endpoint is written down, then a yes or a no.
- [ ] Load testing has an answer, or a stated target to test against.
      **By eye.** Without a target there is nothing to pass or fail, so the target is the first half.
- [ ] Mutation testing has an answer.
      **By eye.** Either one Stryker run over `lib/src/Binacle.Lib/ResultSelection/` has been costed, or this
      section says why it was dropped.
