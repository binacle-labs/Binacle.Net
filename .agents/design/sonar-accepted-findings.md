---
id: sonar-accepted-findings
description: The Sonar findings answered with a reason rather than a code change, why each one stands, and why this register has to live in the repository rather than in the SonarCloud UI.
verified: 2026-08-27
check: S101 against GetAlgorithmIdentifierName() still emitting `FFD_v2` and the fixtures still parsing it; the five fixture methods against lib/test/Binacle.Lib.UnitTests, where they must still be instance methods reached as this.Fixture.X(...); S3458 against the six Item.Rotate switches; S1075 against the licence and GitHub URLs in the OpenAPI documents; the js-cookie entries against packages/cookies
paths:
  - "lib/**"
  - "packages/cookies/**"
  - "api/src/Binacle.Net.Kernel/OpenApi/**"
---

# The Sonar findings that are answered, not fixed

**This register is the only durable copy.** On the Free plan a finding is answered by marking it Accepted in
the SonarCloud UI - "Sonar way" is read-only, and `sonar.issue.ignore` rules are not allowed in
`tooling/sonar-analysis.xml`. **Those marks live in the project's database, not in this repository.**

**They have already been lost once.** The project was recreated on 2026-08-17 under the `binacle-labs`
organization, key `binacle-labs_Binacle.Net`, because a SonarQube Cloud org's binding to a GitHub account
cannot be changed once set. A new org was chosen over a rebind, so the old project and everything held in its
UI went with it. **The code fixes survived - they are commits. Only the accept-decisions were lost.**

So: mark them again from this list, and add to this list before marking anything new.

## The register

| Rule | Count | Why it stands |
|---|---|---|
| `S101` | 38 | `_v1` lowercase is the house style. `GetAlgorithmIdentifierName()` emits `FFD_v2` and the fixtures parse it, so the name is a **format**. Renamed and reverted on 2026-08-09. **Do not attempt the rename again.** |
| `S2325` / `CA1822` | 5 of them | `CommonTestingFixture.Run`, `.GetScenarioByName`, `.AssertResult`, `ResultSelectionTestingFixture.Select`, `.GetScenarioByName`. Static was applied and reverted: they are reached as `this.Fixture.X(...)` from sixty test bodies, and static forces `CommonTestingFixture.X(...)`, which stops the tests going through the fixture at all and breaks the arrange/act/assert convention. |
| `S3458` | 6 | `case 0: default:` in the six `Item.Rotate` switches. `case 0` documents the identity orientation. |
| `S1854` | 3 | `newAvailableSpaces[--newSpaces] = ...` - the decrement **is** the index. |
| `S1075` | 2 | The GPL licence and GitHub URLs in the OpenAPI documents are canonical constants, and a named `const` does not satisfy the rule anyway. |
| `javascript:S1874`, `S1121` | - | `packages/cookies` tracks upstream js-cookie v3.0.5 line for line, and the `escape` is deliberate - RFC 6265 `()` encoding. Mark with the reason that the file tracks upstream. |
| `S1135` | 2 | TODO comments, INFO severity. Leave them. |

**Every one of these is a reason, not a suppression.** The repository answers a finding where a reader can see
the answer; it never hides one with an ignore rule.

## A fix to `samples/` does not reach the frozen copies under `sites/docs/`

**This is the lesson the 2026-08-09 sweep actually produced**, and it is worth more than the findings.

Seven vulnerabilities appeared the moment the site exclusion was narrowed, all in
`sites/docs/collections/_versions/**` - the versioned sample files users download. They were invisible
because the site had been excluded whole, and **those two published sites are the only public attack surface
in the repository.**

Six of the seven were pure drift. `samples/kubernetes/minimal/binacle-deployment.yaml` already carried
`automountServiceAccountToken: false` and a full `resources:` block; the frozen copies under `v2.0.x` and
`v2.1.x` never got it, so a reader following the published instructions downloaded the unhardened manifest.
The seventh was a real GUID shipped as `PrimaryApiKey`, repeated in the matching compose file's
`OTEL_EXPORTER_OTLP_HEADERS` - **the two files must always agree, or the sample breaks.**

**Nothing enforces that a sample fix reaches its frozen copies.** Sweep them whenever a sample changes.

**The gate was never going to catch these.** They sat on lines a BOM commit had only touched at line 1, so
they counted as old code.

## The carve-out that let them be fixed at all

`d0150235` fixed all seven from a coding session, at the time against the rule that published sites are off
limits. **Settled 2026-08-10: that rule now carries a carve-out** for exactly this - a security fix to a
downloadable sample file under `sites/docs/collections/_versions/**`, touching no prose, no front matter and
no `.md`, matching what `samples/` already allows. It is narrower than "docs findings are fair game", and
every use is recorded in the plan that owns the work.
