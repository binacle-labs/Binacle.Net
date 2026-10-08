---
name: sonar-scope-exclusions
description: sonar.exclusions and friends are scope exclusions, not issue ignores - they are allowed and already in use
type: convention
when: reading or editing the exclusion lists in tooling/ci/sonar-analysis.xml
paths:
  - "tooling/ci/sonar-analysis.xml"
  - "Directory.Build.props"
---

`tooling/ci/sonar-analysis.xml` **does** carry `sonar.exclusions`, `sonar.cpd.exclusions` and
`sonar.coverage.exclusions`. Do not read the ban on issue ignores as forbidding them - they are a different
thing. An issue ignore says "run this rule here, then hide what it finds". A scope exclusion says
"this is not our code, or not this metric's business".

- `sonar.exclusions` drops build output, vendored code such as `assets/lib/**`, and the fixture sets under
  `shared/data/` and the other data folders - the sets only, since the C# project beside each set is analysed.
  Nobody reviews or fixes these, so measuring them only moved the totals.
- `sonar.cpd.exclusions` covers `lib/src/Binacle.Lib/Algorithms/**`, where the algorithm versions are parallel
  implementations by design, and the frozen `api/src/Binacle.Net/v3/**`. **Every rule still runs on those
  files** — only duplication detection stops.
- Support projects are handled in `Directory.Build.props`, not here, via `SonarQubeTestProject` (`$build-topology`).
  That reclassifies them as test code rather than hiding anything.

**Why:** the two look alike in the same file, and reading the issue-ignore ban too broadly would get a
legitimate exclusion deleted.

**How to apply:** the test is whether a reader of the code would want to know. A hidden finding fails it; a file
that was never ours does not.
