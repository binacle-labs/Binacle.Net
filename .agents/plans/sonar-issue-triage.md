---
description: Sonar re-read against the current project on 2026-08-28 - what the scope fix and the high-severity pass cleared, the 299 findings left, and the answer on whether test and tooling code stays in scope
state: ready
waits-on: "nothing. Every item below is work, and the two that are decisions are recommended in place"
paths:
  - "tooling/ci/sonar-analysis.xml"
  - "api/src/Binacle.Net.UIModule/**"
  - "packages/**"
---

# Sonar - what is left

**Read on 2026-08-28 through the SonarCloud API, against the 2026-08-27 run of `binacle-labs_Binacle.Net`.**
Every count here is open issues only. The dashboard's own totals mix in closed ones and read about 12% high.

## What the run said, and what most of it was

**384 open findings. 65 of them were other people's gems.** CI installs the bundle into `ruby/vendor/bundle`
before the scan, and nothing excluded it, so kramdown's html fixtures and four gems' own workflow files were
being analysed as if we had written them.

**All 8 open security findings were in that folder.** Not one was in our code. The security rating of C, and
the gate's `new_security_rating` failure, were both entirely vendored gems - unpinned action tags in
`http_parser.rb`'s CI, and `bundle install` without a lockfile in `terminal-table`'s.

| | Open | Security | Reliability | Maintainability | High or worse |
|---|---|---|---|---|---|
| As measured | 384 | 8 | 81 | 311 | 34 |
| Less `ruby/vendor/**` | 319 | 0 | 27 | 295 | 22 |
| Less the high-severity pass | 299 | 0 | 12 | 290 | 2 |

The two remaining high findings are `typescript:S1186` on `packages/binacle-net-ui/tests/stubs/orbitControls.ts` -
empty `update()` and `dispose()` on a stub whose whole point is to do nothing. **Mark both Accepted in the
SonarCloud UI with "deliberate no-op stub".** The file comment already says why; repeating it inside the
braces to satisfy the rule would be the comment rule broken to please an analyser.

## Test and tooling code stays in scope

**114 of the remaining 299 sit in test or tooling code, and they should stay visible.** Three reasons, and
they are not the same reason:

- **The costly half is already handled.** `SonarQubeTestProject` in `Directory.Build.props` takes the .NET
  support projects out of the coverage denominator and off the product rule set. The globs added to
  `sonar.coverage.exclusions` do the same for the python, site and typescript files MSBuild cannot see. What
  is left is test-scope rules on test code, which is what they are for.
- **Maintainability is rated A and the gate does not read these.** They cost nothing at the gate, so
  excluding them buys nothing to offset going blind.
- **The top rules are about test correctness, not tidiness.** `xUnit1042` and `xUnit1050` (33 between them)
  are untyped `MemberData`/`ClassData`. `CA1816` (14) is the dispose pattern on fixtures. A fixture that
  disposes wrongly is a flaky suite, and that is exactly the failure nobody debugs from the symptom.

**`tooling/ci/` is the strongest case for keeping, not the weakest.** Its 15 high-severity findings were the
whole of the reliability rating - fourteen `[` tests and a `case` with no default, in the scripts that decide
whether a release publishes. Excluding the folder would have hidden them. They are fixed instead.

## What is actually left, biggest first

- **xUnit1042 (22) + xUnit1050 (11)** - `TheoryData<T>` is a real improvement to the ViPaq and Kernel suites,
  but it is a rewrite per data source rather than an edit. **Biggest single item.**
- **Six accessibility findings on the shipped UI**, and these are the ones worth doing next: three tables
  without `<th>` in `Instance.cshtml`, an unlabelled input in `Packing.cshtml`, an empty heading in
  `_ErrorsDialog.cshtml`, and an `<Applet>` element in `_Navbar.cshtml` that html5 does not have. Real users,
  real pages.
- **S101 (38)** - `BestAlgorithm_v1` and its siblings. The identifier is a published format, not a name, so
  this is an Accept-with-reason sweep rather than a rename.
- **CA1873 (13)** guard expensive log arguments · **CA1859 (9)** concrete return types · **CA2208 (9)**
  exception `paramName` used as a message, which needs an exception-type decision rather than a swap.
- **S1192 (11)** - `box_1/2/3` in both `ExampleData.cs`, `first`/`previous`/`repeat` in `PackingVisualizer`
  (dictionary keys, so a const prevents a runtime `KeyNotFoundException`), and two canonical URLs.
- **S2325 (10) + CA1822 (19)** - the residue of the make-static sweep, minus the five that are answered.
- **~45 TypeScript and JavaScript** modernisation items in `packages/` and `vipaq/packages/`, nearly all in
  files with little coverage, so each fix costs new-code coverage and buys style.

**`lib/data/**` no longer needs excluding.** The earlier plan asked for it. The 2026-08-27 run indexed no json
at all - the language breakdown is cs, web, ts, yaml, css, js, shell, py, docker and nothing else - so the
entry would be dead weight.

## CA1816 on ten test fixtures

Three of thirteen are done. The other ten are xunit `IAsyncLifetime` classes whose `DisposeAsync` ends in
`await base.DisposeAsync()`. `GC.SuppressFinalize(this)` on a fixture that will never have a finalizer is
ceremony. **Accept the ten with "test fixture, no finalizer"** rather than adding the line ten times.

## The gate

`new_coverage` is the only condition that this pass does not move: **63.0% against 80%**. The other two
failures - `new_security_rating` and `new_reliability_rating` - were the vendored gems and the shell scripts,
and both are cleared.

**The 80% cannot be lowered.** Custom quality gates need the Team plan and the project is on Free. The gate
goes green when the untested code gets tested. `plans/ci-cd/sonar-coverage-gap.md` says where that code is.

## Done when

- [ ] The two `orbitControls.ts` stub findings are Accepted with a reason in SonarCloud.
      **By eye**, on the issue list filtered to Accepted.
- [ ] The six accessibility findings on the UIModule pages are fixed or answered.
      Re-read the project filtered to `Web:` rules on `api/src/Binacle.Net.UIModule/**`; the list is empty or
      every entry is Accepted.
- [ ] The findings answered with a reason rather than a code change are marked Accepted again in the current
      project. The marks live in SonarCloud's database and were lost once when the project was recreated.
      **By eye.** Open the issue list filtered to Accepted and compare it to the design register.
