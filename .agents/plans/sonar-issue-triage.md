---
description: Sonar - re-read the open findings against the current project, then the one-line exclusion fix and the two rewrites behind it
state: blocked
waits-on: "a re-read against SonarCloud - every count in this file predates the current project and the maintainer wants it revisited before the v3.0.0 tag"
paths:
  - "tooling/sonar-analysis.xml"
---

# Sonar - what is left

**Every number below is stale, and that is this plan's first step.** The project was recreated on 2026-08-17
under `binacle-labs_Binacle.Net` and nobody has read the findings since. **Do not act on a count here.**

**The findings answered with a reason rather than a code change are not in this file.** They are a design
record, because the Accept marks live in SonarCloud's database and were lost once already when the project was
recreated. Re-mark them from there, not from here.

Read the memory on touching untested code before fixing anything, and the one on the algorithm identifier
being a format before renaming anything.

## Step 1 - re-read, and write the counts back here

The last read was the `016d7478` run on 2026-08-09: **509 open issues down to 305**, 24 of the 305 being new
arrivals from the two sites coming back into scope. Seven vulnerabilities were found and all seven fixed the
same day in `d0150235`, so the rating should read A again — **unverified.**

## Step 2 - the one-line exclusion fix

**`lib/data/**` is missing from `sonar.exclusions`.** Found 2026-08-13. The line names `shared/data/**` and
`vipaq/test-vectors/**` as the fixture corpora; the tests-kernel split moved the result-selection fixtures to
`lib/data/result-selection/`, which no entry covers, so those json files are indexed where they used to be
skipped.

Three files, so it is small — **but it is the same class as the `shared/data` entry that turned out to be 28%
of the project measured as data.** A new fixture folder needs a new entry, and this is the first time that has
come up, so the line becomes a list rather than a pair. **Two other places repeat the list in prose and drift
with it**: the comment above the line, and the no-sonar-issue-ignores memory.

## Step 3 - what is left, biggest first

- **xUnit1042 (22) + xUnit1050 (10)** — `MemberData`/`ClassData` returning untyped `object[]`. `TheoryData<T>`
  is a real improvement to the ViPaq and Kernel suites, but it is a rewrite per data source rather than an
  edit. **Biggest remaining item.**
- **CA1873 (13)** guard expensive log arguments · **CA1859 (9)** concrete return types, mostly in test helpers
  · **CA2208 (9)** exception `paramName` used as a message, which needs an exception-type decision rather than
  a swap.
- **S1192 (11)** — the media-type half is done. Left: `box_1/2/3` in both `ExampleData.cs` (file-private
  consts, values unchanged so the OpenAPI examples stay identical), `first`/`previous`/`repeat` in
  `PackingVisualizer` (dictionary keys, so a const prevents a runtime `KeyNotFoundException` — but UIModule is
  0% covered), and two canonical URLs.
- **S2325 (8) + CA1822 (13)** — the residue of the make-static sweep, minus the five that are answered.
- **S3776 (2)** cognitive complexity 17 against 15 in `Auth/Token.cs` and `Program.cs` · **S2365 (1)**
  `Navbar.MenuItems` rebuilding its list on each of four reads per render · **ASP0025**, **CA1869**, and a
  thin tail of one-liners.
- **~45 TypeScript and JavaScript** modernisation items in `packages/` and `vipaq/packages/`, nearly all in
  0%-coverage files, so each fix costs new-code coverage and buys style. **Do these with the UI harness.**

## Two questions nobody has answered

**CA1816 on ten test fixtures.** Three of thirteen are done. The other ten are xunit `IAsyncLifetime` classes
whose `DisposeAsync` ends in `await base.DisposeAsync()`. `GC.SuppressFinalize(this)` on a fixture that will
never have a finalizer is ceremony. **Add the line ten times, or accept the ten with "test fixture, no
finalizer".**

**The prose on the frozen versioned sample pages.** The sample *files* are fixed. The `v2.0.x` and `v2.1.x`
pages now ship corrected manifests with nothing on the page saying so, and the resource values are starting
points rather than a recommendation. The `v3.0.x` page got a note under "Customize" on 2026-08-10; the two
frozen pages were left alone pending this. **A docs decision, not a coding one:**

- **Correct the frozen copies whenever the current one is corrected, and say nothing.** Cheapest, but it
  silently rewrites what a released version shipped.
- **Annotate the frozen pages as historical**, pointing at the current sample. Honest, but it leaves a reader
  on an old page holding a file we know is worse.

Either way the answer wants writing down somewhere durable, because the failure is silent: nobody diffs a
four-version-old manifest. **Nothing here blocks a release** — the current version's manifest ships limits and
its page explains them.

## The gate

`new_coverage` is the only failing condition — **45.4% against 80%** at the last read. Nothing else fails, and
the open issues block nothing; `new_maintainability_rating` is A.

**The 80% cannot be lowered.** Custom quality gates need the Team plan and the project is on Free. So the gate
goes green when the UI gets tested, not by configuration.

## Done when

- [ ] The counts in this file came from a run someone opened.
      **By eye.** Step 1's numbers name a revision analysed under `binacle-labs_Binacle.Net`.
- [ ] `lib/data/**` is excluded, and the two prose copies of the list match the line.
      `grep -c 'lib/data' tooling/sonar-analysis.xml` returns at least 1, and the comment above it and the
      no-sonar-issue-ignores memory name the same folders.
- [ ] The answered findings are marked Accepted again in the current project.
      **By eye.** Open the project's issue list filtered to Accepted and compare it to the design register.
- [ ] Each of the two open questions has an answer written down.
      **By eye.** Not in this file — in the code, or on the page, or in a design record.
