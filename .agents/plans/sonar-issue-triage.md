---
description: Reliability is A and HIGH is zero as of 2026-08-31. What is left is 288 code smells and one red gate condition, new_coverage, which belongs to the coverage plans
state: mostly done
waits-on: "a scan. The workflow is dispatch-only, so nothing after fab50ba9 is in the numbers"
paths:
  - "api/src/Binacle.Net.UIModule/Pages/**"
  - "api/src/Binacle.Net.UIModule/_js/instance.js"
  - "packages/binacle-net-ui/src/utils/samples.ts"
  - "ruby/*/spec/**"
  - "tooling/ci/sonar-analysis.xml"
---

# Sonar - getting reliability to A

## Done - 2026-08-31

**Reliability A, Security A, Maintainability A. Zero bugs. Zero HIGH findings.** Read off the API after the
scan on `fab50ba9`.

| | before | after |
|---|---|---|
| Reliability | C | **A** |
| Bugs | 6 | **0** |
| HIGH findings | 27 | **0** |
| Open issues | 324 | 288 |

**What cleared it.** The five code edits below, plus the `_Navbar.cshtml:1` accept, plus the 27 `ruby:S1192`
literals hoisted to constants across 16 spec files. `Web:S6850` and `csharpsquid:S125` were accepted too.

**Then 30 more MEDIUM findings went in the same day** - the "genuinely a small edit" set below, across 22 files.
**Those are not in the numbers above**: they landed after `fab50ba9` and the scan is dispatch-only.

**Three of them did not go the way this file said.** `toSorted` is ES2023 and both tsconfigs target `es2016`,
so `javascript:S4043` was fixed by splitting the sort onto its own statement. `rubydre:S7840` in `resolver.rb`
needed `lazy` - the obvious `filter_map.first` evaluates every source and each one runs a markdown conversion.
And `CA1869` had two `JsonSerializerOptions` in that file, not one.

**`CA1850` came with a test the code never had.** `Sha256PasswordHasher` had no coverage at all, and the
login tests hash and compare in one run, so they pass whatever the algorithm is. `Sha256PasswordHasherTests`
now pins known answers computed outside C#. A round trip would not have caught the hash changing, and every
stored password would have stopped matching.

**What is left is one red gate condition and it is not this file's work.**

| | |
|---|---|
| `new_coverage` | **40.5% against 80%** - 916 of 1396 new lines uncovered. 637 more lines have to be covered |
| Everything else | passing |

**Start with `ruby/`: 749 lines, 0.0% coverage, while all ten gem suites pass with 232 examples.** That is a
reporting gap, not missing tests. `sonar.ruby.coverage.reportPaths` is wired in `tooling/ci/sonar-analysis.xml`
with the `../` prefix the ruby sensor needs, so the question is whether the report is being written at all.
It is the largest single block of uncovered lines in the project and the cheapest to move.

After that, by uncovered lines: `ServiceModule.Infrastructure` 432, `Kernel` 357 (of which `Kernel/OpenApi`
is 173 at 10.8%, and its `Transformers` folder is 140 at 0%), `Binacle.Net` 344, `binacle-net-ui` 201,
`DiagnosticsModule` 171.

---

## The run this was read from

**Analysis of 2026-08-30 00:02 UTC on revision `06f18da4` ("Merge pull request #17 from
binacle-labs/features/results_vendoring").** Read on 2026-08-31 through the public SonarCloud API, so the data
was one day old at reading. **The scan workflow is `workflow_dispatch` only**, so these numbers do not move
until someone runs it, and anything committed after `06f18da4` is not in them. **Seven commits are on `main`
past that revision**, but none of them touches any of the six files below, so every line number here still
resolves in the working tree.

Counts are open issues only. The dashboard mixes in closed ones.

## The numbers

| | |
|---|---|
| Reliability | **C** |
| Security | A |
| Maintainability | A |
| Bugs | **6** |
| Vulnerabilities | 0 |
| Security hotspots | 0 |
| Code smells | 318 |
| Coverage | 64.3% |
| Duplicated lines | 3.9% |
| Lines of code | 29381 |

**Reliability is C, not B.** One MAJOR bug is enough to drop it to C, and all six are MAJOR. The rating is the
worst single bug, so six MAJOR bugs and one MAJOR bug give the same letter.

Open issues total **324**, by impact severity: **27 HIGH, 177 MEDIUM, 120 LOW, 1 INFO.** Those sum to one
more than the total, because an issue can carry more than one impact and the facet counts it under each.

**The gate is red on two conditions**, both against new code (a rolling 30 days):

- `new_reliability_rating` **C against A** - all six bugs are in the new-code window.
- `new_coverage` **40.4% against 80%**. Not this plan's work; the coverage gap is planned on its own.

The other four conditions pass, including `new_security_rating` and `new_maintainability_rating` at A.

## Every open bug

Six, all MAJOR, all reliability-impact MEDIUM, all created 2026-08-21 or 2026-08-22.

| Rule | Where | What is actually wrong | Verdict |
|---|---|---|---|
| `Web:UnsupportedTagsInHtml5Check` | `api/src/Binacle.Net.UIModule/Pages/Shared/_Navbar.cshtml:1` | Line 1 is `@model IReadOnlyList<Applet>`. Sonar's HTML analyser reads `<Applet>` as the deprecated html element. `Applet` is a C# class in `api/src/Binacle.Net.UIModule/Models/Applet.cs`. | **False positive. Accept.** |
| `Web:S5256` | `Pages/Instance.cshtml:14` | The Build table is `<tbody>` rows of label + value with no `<th>`. A screen reader gets six unlabelled cells. | **Fix** |
| `Web:S5256` | `Pages/Instance.cshtml:34` | Same shape - the Switches table, name / state / path, no header cell anywhere. | **Fix** |
| `Web:S5256` | `Pages/Instance.cshtml:73` | The Presets table ships with an empty `<tbody data-presets-body>`; `_js/instance.js` fills it with three cells per row (name, box count, dimensions) and never adds a header. | **Fix** |
| `Web:InputWithoutLabelCheck` | `Pages/Packing.cshtml:150` | `<select x-model="model.algorithm" name="algorithm" id="algorithm">` has no `<label for="algorithm">`. The only thing next to it is an `<i>arrow_drop_down</i>` icon. | **Fix** |
| `typescript:S6959` | `packages/binacle-net-ui/src/utils/samples.ts:25` | `bins.reduce((largest, bin) => ...)` with no seed. `Array.reduce` on an empty array with no initial value throws `TypeError`. | **Fix** |

### The four Razor findings are one root cause

All four are the HTML analyser reading `.cshtml`. Three are real - the tables genuinely have no header cells,
and the analyser is right about a page that ships to users. One is the analyser mistaking Razor syntax for
html, and no edit to the file would be an improvement.

**The three tables want `<th scope="row">` on the first cell of each row**, not a `<thead>`, because Build and
Switches are label/value lists rather than columnar data. The Presets table is the exception: its rows come
from javascript and it has three real columns, so it wants a `<thead>` with three `<th>` in the `.cshtml`.

### The `reduce()` finding is real but narrow

**The one production caller already guards it.** `packages/binacle-net-ui/src/core/packingDemo.ts:89` reads
`this.model.bins.length > 0 ? largestBin(this.model.bins) : randomBin()`. So nothing in the demo can reach the
throw today.

**It is still worth fixing, because `largestBin` is exported from the package's public surface**
(`packages/binacle-net-ui/src/utils/index.ts`), and an external caller has no guard. Decide the empty case in
the function rather than seeding the reduce: seeding with `bins[0]` makes an empty array return `undefined`
silently, which is worse than the throw.

## The shortest path to Reliability A

**All six. There is no shorter set.** The rating is the worst single bug, so every one of them has to be gone
or accepted before the letter moves.

That is **five code edits across three files, plus one accept**:

1. `Pages/Instance.cshtml` - `<th scope="row">` in the Build and Switches tables, a `<thead>` on Presets. One file, three findings.
2. `Pages/Packing.cshtml:150` - a `<label for="algorithm">`.
3. `packages/binacle-net-ui/src/utils/samples.ts:25` - handle the empty array explicitly.
4. `Pages/Shared/_Navbar.cshtml:1` - **Accept**, reason "Razor `@model` type, not an html element".

**The same set also clears the gate's `new_reliability_rating`**, because all six are inside the new-code
window. Nothing else has to happen for that condition to go green.

**Two things to know about accepting.** It needs issue-administration on the project, and the mark lives in
SonarCloud's database rather than in the repository - the marks were lost once already when the project was
recreated, so an accept is not a durable answer the way a code edit is.

**Effort: small.** Three files, an afternoon at most, and every edit is local.

## High severity: 27 findings, one rule, and they are ours

**All 27 HIGH findings are `ruby:S1192`, "String literals should not be duplicated", and every one is in a
`spec/*_spec.rb` file of one of our own Jekyll gems.** Nothing else in the project is HIGH.

**They did not arrive with a vendored gem.** `tooling/ci/sonar-analysis.xml` already excludes
`ruby/vendor/**` and `ruby/*/spec/fixtures/**`. What is left is `ruby/binacle-docs-versions`,
`ruby/jekyll-breadcrumb-trail`, `ruby/jekyll-filters`, `ruby/jekyll-gtm`, `ruby/jekyll-multi-sitemap`,
`ruby/jekyll-page-meta`, `ruby/jekyll-structured-data` and `ruby/jekyll-webmanifest` - all tracked, all ours.
An earlier reading of this project recorded these as gem findings; that was wrong and it is worth not
repeating.

**The rule fires at three repeats and Ruby rates it CRITICAL / maintainability HIGH.** The identical rule on
C# code is rated far lower, which is the whole reason this group dominates the HIGH count while
`csharpsquid:S1192` (8 findings) does not appear in it.

**27 findings over 14 files.** The heaviest are `jekyll-structured-data/spec/tag_spec.rb:7` ("index.html", 12
times) and `jekyll-multi-sitemap/spec/generator_spec.rb:9` ("sitemap.xml", 12 times). The rest are three to
seven repeats of a fixture filename or a fixture string.

**Verdict: fix.** Each file gets a handful of constants at the top and the literals become names. It is
mechanical, it is confined to spec files, and the specs fail loudly if a rename goes wrong. The alternative
is turning the rule off for `ruby/**/spec/**` in the quality profile, which is defensible - a fixture filename
repeated in a spec is not the duplication the rule is aimed at - but it buys nothing that fixing does not, and
it needs a profile change rather than a repository change.

**Effort: medium and boring.** One pass per file, 14 files, no cross-file reasoning.

There are **no HIGH findings outside this group**, so clearing it takes the project to zero HIGH.

## Medium severity: what is genuinely a small edit

177 MEDIUM findings. These are the ones that are a real edit and nothing more.

**Two are analyser mistakes and want an accept rather than a change:**

- `Web:S6850` on `Pages/Shared/_ErrorsDialog.cshtml:4` - "headings must have content". The element is
  `<h5 x-text="title">`, filled by Alpine at runtime from the `errors_dialog('One or more Errors occured!')`
  initialiser on line 1. **Accept.**
- `csharpsquid:S125` on `vipaq/src/Binacle.ViPaq/ViPaqBase64Extensions.cs:6` - "remove this commented out
  code". It is the two-line usage example inside the file's header comment. **Accept.**

**Small edits, worth doing:**

| Rule | Count | Where | Why it is small |
|---|---|---|---|
| `typescript:S6557` | 5 | `packages/binacle-compact-notation/src/compactNotation.ts:26,102`, `packages/cookies/src/converter.ts:4` | `text[0] !== "("` becomes `!text.startsWith("(")`. Same line, same meaning. |
| `Web:S5255` | 4 | `Pages/Packing.cshtml:63,134`, `Pages/Shared/_Navbar.cshtml:7,22` | An `aria-label` on four `<nav>` elements. The two navbar ones need different labels - one is the left rail, one is the bottom bar, and they render the same list. |
| `typescript:S7772` | 6 | `packages/binacle-net-ui/tools/generateSamples.ts`, `vipaq/packages/binacle-vipaq/tools/interopArtifactGenerator.ts`, `vipaq/packages/binacle-vipaq/tests/support/vectorReader.ts` | `node:` prefix on `fs` and `path` imports. |
| `shelldre:S7679` | 3 | `tooling/ci/changed-paths.sh:15,16`, `tooling/ci/codeql-summary.sh:20` | Assign `$1`/`$2` to a named local at the top of the function. |
| `typescript:S2933` | 2 | `vipaq/packages/binacle-vipaq/src/ProtocolWriter.ts:6`, `ProtocolReader.ts:5` | Add `readonly` to one field each. |
| `external_roslyn:CA1850` | 2 | `api/src/Binacle.Net.ServiceModule.Infrastructure/Services/Sha256PasswordHasher.cs:29,36` | `ComputeHash` becomes the static `SHA256.HashData`. Drops the disposable too. |
| `rubydre:S7840` | 2 | `ruby/jekyll-breadcrumb-trail/lib/jekyll-breadcrumb-trail/labels.rb:13`, `ruby/jekyll-page-meta/lib/jekyll-page-meta/resolver.rb:43` | `each` with an early return becomes `find`. |
| `rubydre:S8418` | 1 | `ruby/jekyll-multi-sitemap/lib/jekyll-multi-sitemap/renderer.rb:20` | Rename the unused `site` parameter to `_site`. |
| `javascript:S4043` | 1 | `api/src/Binacle.Net.UIModule/_js/instance.js:26` | `names.sort(...)` becomes `names.toSorted(...)`. `names` is a fresh `Object.keys` result so the mutation is harmless today; the swap is one word. |
| `external_roslyn:CA1854` | 1 | `lib/test/Binacle.Lib.Benchmarks/Providers/SpecializedScalingProblemsProvider.cs:89` | `ContainsKey` then indexer becomes `TryGetValue`. |
| `external_roslyn:CA1869` | 1 | `vipaq/tools/Binacle.ViPaq.VectorGenerators/InteropArtifactGenerator.cs:29` | Hoist the `JsonSerializerOptions` to a static field. |

**That is 30 findings across 11 rules, two of them accepts.** None of them changes a signature or a shape.
Effort: small, and they can be taken one rule at a time.

**One `javascript:S7772` finding is out of reach.** Three of the four sit in `sites/www/webpack.config.js`,
`sites/demo/webpack.config.js` and `sites/docs/webpack.config.js`. Published sites are not edited from a
coding session. Only `api/src/Binacle.Net.UIModule/webpack.config.js` is in scope, and it is in the row above.

## What is being left, on purpose

**The remaining ~145 MEDIUM findings stay open.** Not because they are wrong, but because each is a rewrite or
a design decision rather than an edit, and none of them moves the reliability rating.

- **`external_roslyn:xUnit1042` (22) and `xUnit1050` (11).** Untyped `MemberData` / `ClassData`. `TheoryData<T>`
  is a real improvement, but it is a rewrite per data source. **Biggest single group left.**
- **`external_roslyn:CA1816` (14).** The dispose pattern on test fixtures. These are `IAsyncLifetime` classes
  that will never have a finalizer, so `GC.SuppressFinalize(this)` is ceremony fourteen times over. The right
  answer is an accept sweep with a reason, not code.
- **`external_roslyn:CA1873` (13).** Guard expensive log arguments. Each one needs a judgement about whether
  the argument is actually expensive.
- **`external_roslyn:CA1822` (13) and `csharpsquid:S2325` (8).** Make members static. A sweep, and some of
  these members are deliberately instance members for an override.
- **`external_roslyn:CA1859` (10).** Return concrete types. Changes public signatures in places.
- **`csharpsquid:S1192` (8).** The C# side of the duplicate-literal rule, rated MEDIUM here rather than HIGH.
  Some are dictionary keys where a constant would prevent a runtime `KeyNotFoundException`, so it is worth
  doing eventually, just not as part of this.
- **`external_roslyn:CA2208` (8).** `ArgumentNullException` constructed with a sentence in the `paramName`
  slot, in `lib/test/Binacle.Lib.TestsKernel/ResultSelection/ScenarioReader.cs` and
  `shared/test/Binacle.TestsKernel/Algorithms/ScenarioReader.cs`. The fix is picking a different exception
  type for "this scenario file is malformed", not swapping an argument.
- **`typescript:S5976` (7).** Parameterise three-to-five near-identical tests. Test rewrites.
- **`typescript:S1444` (9) and `external_roslyn:CA2211` (5).** Mutable public/static fields, mostly in test
  key holders. Making them immutable changes what test code may reference.
- **`csharpsquid:S1117` (5).** A parameter named `value` shadowing a field, all five in
  `api/src/Binacle.Net.Kernel/Configuration/Models/ConnectionString.cs`. It reads like a rename, but I have
  not read the type, so I am not calling it easy.
- **`csharpsquid:S3881` (3).** The dispose pattern on `vipaq/src/Binacle.ViPaq/ProtocolReader.cs`,
  `ProtocolWriter.cs` and `api/src/Binacle.Net.Kernel/Logging/TimedOperation.cs`. Shipped types, real pattern
  change.
- **`csharpsquid:S1854` (3).** `newAvailableSpaces[--newSpaces]` where the final decrement is never read, once
  each in the Best Fit, Worst Fit and First Fit Decreasing v2 algorithm operations. The edit is one character
  of arithmetic, but it is in the packing hot path and each of the three wants reading in full before
  touching.
- **The long tail.** `csharpsquid:S3928` (3), `S3442` (3), `S2326` (2), `S4144`, `S3246`, `S2589`, `S2629` and
  `external_roslyn:CA2254` (the same line of `TimedOperation.cs:32`), `ASP0025`, `CA1861`, `xUnit1045`,
  `typescript:S1121`, `S4624` (2), `rubydre:S7815` (4). One to four each, and each needs a decision about a
  type or an API rather than an edit.

**The 120 LOW findings are out of scope here.** The largest is `csharpsquid:S101` (38), the `BestAlgorithm_v1`
naming family, where the identifier is a published format string rather than a name - an accept-with-reason
sweep, not a rename.

## Done when

- [ ] `api/issues/search?componentKeys=binacle-labs_Binacle.Net&resolved=false&types=BUG` returns 0. Five edits
      in three files and one accept on `_Navbar.cshtml`.
- [ ] The reliability rating reads A and the gate's `new_reliability_rating` condition passes.
      **A scan has to run first** - the workflow is dispatch-only, so nothing above shows up until it does.
- [ ] `api/issues/search?...&impactSeverities=HIGH` returns 0, which means the 27 `ruby:S1192` findings in the
      gem specs are gone.
- [ ] The two medium accepts are marked: `Web:S6850` on `_ErrorsDialog.cshtml:4` and `csharpsquid:S125` on
      `ViPaqBase64Extensions.cs:6`.
- [ ] The accept marks are checked by eye against the list above. They live in SonarCloud's database, not in
      the repository, and they have been lost once before.
