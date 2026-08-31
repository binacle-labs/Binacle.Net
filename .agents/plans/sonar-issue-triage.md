---
description: Reliability A, zero bugs, zero HIGH, and the gate green as of 2026-08-31. What is left is 288 code smells nobody has to clear, triaged here by rule, and the accept marks to be read back
state: ready
waits-on: "nothing"
paths:
  - "api/src/Binacle.Net.UIModule/Pages/**"
  - "api/src/Binacle.Net.UIModule/_js/instance.js"
  - "packages/binacle-net-ui/src/utils/samples.ts"
  - "ruby/*/spec/**"
  - "tooling/ci/sonar-analysis.xml"
---

# Sonar - getting reliability to A

## Where it stands - 2026-08-31

**Reliability A, Security A, Maintainability A. Zero bugs, zero HIGH findings, zero ruby findings of any
rule.** The gate reads OK and `new_coverage` is 89.3% against its threshold of 80.

**What is left is 288 code smells, and clearing them is not urgent.** Old issues do not fail the gate, and
editing an untested file to fix one costs new-code coverage - see the memory on touching untested code.
Sections below are the triage: which rules, which files, and what the honest answer to each is.


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

- [ ] The six accept marks are read back against SonarCloud.
      **By eye.** The list is in the memory on Sonar issue ignores - that is the only copy, because the marks
      live in SonarCloud's database and have been lost once before.
- [ ] `impactSeverities=HIGH` returns 0 again.
      It read 7 on 2026-08-31, all `shelldre:S7688` in the two new `tooling/coverage.*.sh` files. Fixed in the
      tree the same day; unproven until the next scan.
