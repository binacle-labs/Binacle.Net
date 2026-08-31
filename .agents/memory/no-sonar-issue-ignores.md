---
name: no-sonar-issue-ignores
description: Sonar findings are answered in code, never with a sonar.issue.ignore rule in tooling/ci/sonar-analysis.xml
type: decision
when: answering a Sonar finding
paths:
  - "tooling/ci/sonar-analysis.xml"
  - "Directory.Build.props"
---

`tooling/ci/sonar-analysis.xml` carries no `sonar.issue.ignore.multicriteria` entries, and none should be added.
A suppression there hides a finding from everyone reading the code, in a file nobody opens.

**Two honest answers to a rule you disagree with:**

1. **Change the code** so the analyser reads it correctly.
2. **Mark the individual finding** Accepted or False Positive in the SonarCloud UI, with a reason. It is
   per-finding rather than per-path, so the rule stays armed for the next occurrence and the reason is on the
   record.

Worked examples of the first:

- **S2699 "tests should include assertions"** — restructure so the assert sits in the test body, or mark the
  helper that does the asserting with `[AssertionMethod]`. The analyser matches that attribute **by name
  alone**: no package, any namespace. It is declared twice, in
  `shared/test/Binacle.TestsKernel/AssertionMethodAttribute.cs` and
  `vipaq/test/Binacle.ViPaq.UnitTests/AssertionMethodAttribute.cs`, because ViPaq.UnitTests deliberately does
  not reference TestsKernel (`$vipaq/dependencies`). C# S2699 has no rule parameters, so the Java
  `customAssertionMethods` advice found in Sonar community threads does not transfer.
- **S6418 "hard-coded secret"** on a dev placeholder — change the value so it stops looking like a credential.
  A path ignore would also blind that file to a real secret pasted in later.

**Two things `[AssertionMethod]` cannot reach**, worth knowing before reworking a test: it does not survive a
delegate hop (a test reaching its assert through a `Dictionary<Type, Action>` shows the analyser only
`Action.Invoke`), and there is no code fix for the jwt.io sample JWT in the `TokenResponse` OpenAPI example.

**Some findings survive with nothing honest to change** and are marked in the SonarCloud UI - the table below
is the count, this paragraph is the reasoning. That jwt.io JWT;
**S2245 "use a cryptographically strong RNG"** on `getRandomInt.ts`, not a security context, where swapping in
`RandomNumberGenerator` to pick a demo box would be cargo cult (the rule is `scope: MAIN`, so the same finding
in a benchmark or test kernel disappears once that project is marked as test code) - it was also marked on
`SampleDataService`, which the UIModule rebuild deleted; and **S2068 "hard-coded credential"** on
`AccountGetResponse`'s OpenAPI example, where `PasswordHash` is the literal `"type::hash::salt"` - it documents
the *shape* of a stored hash, and the rule fires on the property name, so any literal there would trip it.

**Eight findings are marked, under two different statuses** - read back from the API on 2026-08-31. A
listing that asks for one status silently misses the other:

| Status | Rule | File | Line |
|---|---|---|---|
| Accepted | `csharpsquid:S6418` | `ServiceModule/v0/Contracts/Auth/TokenResponse.cs` | 35 |
| Accepted | `csharpsquid:S2068` | `ServiceModule/v0/Contracts/Admin/AccountGetResponse.cs` | 94 |
| Accepted | `typescript:S2245` | `packages/binacle-net-ui/src/utils/getRandomInt.ts` | 4 |
| Accepted | `Web:S6850` | `UIModule/Pages/Shared/_ErrorsDialog.cshtml` | 4 |
| Accepted | `csharpsquid:S125` | `vipaq/src/Binacle.ViPaq/ViPaqBase64Extensions.cs` | 6 |
| False positive | `Web:UnsupportedTagsInHtml5Check` | `UIModule/Pages/Shared/_Navbar.cshtml` | 1 |
| False positive | `typescript:S7758` | `packages/binacle-net-ui/src/core/protocolDecoder.ts` | 30 |
| False positive | `typescript:S7758` | `packages/binacle-net-ui/src/core/protocolDecoder.ts` | 87 |

**Accepted and False positive are not the same claim.** Accepted says the rule is right and we are keeping
the code anyway. False positive says the rule is wrong about this line. Use the second only when it is, or
the next reader cannot tell a judgement from a defect.

**This table is the only copy.** The marks live in SonarCloud's database, not in the repository - they do not
survive the project being recreated, and they have been lost once already. Read all eight back after anything
that touches the project's settings, **asking for both statuses**:

    curl -s "https://sonarcloud.io/api/issues/search?componentKeys=binacle-labs_Binacle.Net&branch=main&issueStatuses=ACCEPTED,FALSE_POSITIVE&ps=100"

**Verify against the API, never against a plan's prose.** A plan claimed a sixth accept on `_Navbar.cshtml`;
an agent copied that here unchecked, then "corrected" it off a listing that had only asked for `ACCEPTED`.
Both were wrong. It is marked, under the other status.

**Why:** a finding answered in code stays reviewable and keeps the rule armed for the next occurrence; a
finding answered in config is invisible and switches the rule off for everything matching the path.

**How to apply:** never add an ignore rule to `tooling/ci/sonar-analysis.xml`. Fix the code, mark the assertion
helper, or mark the individual finding in the SonarCloud UI with a reason.
