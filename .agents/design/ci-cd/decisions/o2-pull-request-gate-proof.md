---
id: ci-cd/decisions/O2
description: open question: how much the pull-request gate should prove
status: open
verified: 2026-09-29
paths:
  - ".github/workflows/pull-request.yml"
  - "tooling/ci/sonar-analysis.xml"
---

# O2 — how much the pull-request gate should prove

A PR now runs both test suites, an image build, the three site builds with their link checks, the `.github/`
lints, and - as of `$ci-cd/decisions/D28` - Sonar analysis on a code change from this repository. What is still missing: the
integration suites cover core modules only. That is a known gap rather than an oversight, and the shape of the
fix is not settled — one folded job or three workflows, and what the runtime budget allows.

**Whether coverage becomes a blocking check is open, and it is the maintainer's call.** The project runs the
read-only "Sonar way" gate, which asks 80% on new code; custom gates need a paid plan. This was argued
against until 2026-08-31, on one argument: the condition was red before anyone wrote a line, so it would block
every pull request for a reason none of them caused and be waived within a week. **The condition passes on
`main` as of 2026-08-31**, so that argument is spent. **The shortcut stays rejected** — excluding the untested
areas was considered and turned down, because it moves the number without changing anything true.

**This is the one place the coverage numbers are recorded.** They moved a long way in two weeks and were
being re-derived in four files, which is how three of them ended up disagreeing.

| Measured | Overall | The four areas that were at 0% |
|---|---|---|
| 2026-08-08, from Sonar | 53.3%, 31.4% on new code | the Blazor UI module and three TypeScript packages — 1571 lines, 22.5% of the denominator |
| 2026-08-22, local cobertura | 56.6% — 9511 of 16785 lines, 20 assemblies | all four now have suites; see the table below |
| 2026-08-27, from Sonar — run `ad2e96b8` | 71.1%, 70.6% line — 1892 uncovered of 6429. New code reads **77.0%** against the 80% gate | none. All four suites reached Sonar, which is what the UI harness was waiting to see |
| 2026-08-31, from Sonar | **over 80% on new code — the fixed gate passes on `main`.** The exact percentage was not read; the maintainer confirmed the crossing. Four ServiceModule repositories left 0% because the Sonar workflow ran Sqlite only, and it now starts azurite and postgres | the three below |

**The 2026-08-22 detail**, hand-written code only:

| Area | Lines | Covered | |
|---|---|---|---|
| `api/src/Binacle.Net.UIModule` (C#) | 232 | 216 | 93.1% |
| `packages/binacle-net-ui/src` | 631 | 440 | 69.7% |
| `packages/cookies/src` | 48 | 47 | 97.9% |
| `packages/theme-switcher/src` | 40 | 39 | 97.5% |

**Two caveats on the first row, and both matter to whoever sets a floor.** The assembly reports **35.4%**,
because two generated namespaces land inside it — `Microsoft.AspNetCore.OpenApi.Generated` and
`System.Runtime.CompilerServices`, both at 0% and neither written by anyone here. And 216 of those 232 lines
need both UIModule suites together; the unit one alone does not reach `ModuleDefinition`.

**The old first row counted 959 lines of Blazor that no longer exist.** What replaced it is a tenth of the
size and nearly covered.

**The middle row is local cobertura, the outer two are Sonar's**, and Sonar counts coverable lines its own
way — the shape held and the digits moved, which is why both are kept.

**334 lines came out of the denominator through `sonar.coverage.exclusions` before that run**, and none of
them was ever coverable: the python index generator, the three sites' bundles and webpack configs, and the
typescript fixture-provider and generator folders. That alone moved 67.1% to 70.6% with no test written.
**It is not the same act as excluding untested code**, which stays rejected above.

**What is left uncovered on purpose**, so it is not re-opened as a gap:

- `Kernel/Logs/LogsRetentionProcessor` — the `catch` around `File.Delete`, because there is no portable way to
  force a delete to fail (Linux unlinks open files, Windows does not), and the second turn of the retention
  loop, because the `PeriodicTimer` is built with no `TimeProvider` so the day between sweeps cannot be faked.
  Passing a `TimeProvider` in would make the second one testable, and that is a change to the code.
- `InMemoryAccountRepository`, `InMemorySubscriptionRepository` and `FileHashStore` sit at 0%. The two
  repositories hold their `ConcurrentSortedDictionary` in a static field, so state is shared across the whole
  process and a test that writes to one has to account for every other test that did.

**Still uncovered and not on purpose:** OpenApi document generation, around 130 lines —
`Kernel/OpenApi/ExtensionsMethods/OpenApiOptionsExtensions.cs` and `OpenApiServiceCollectionExtensions.cs`,
`Kernel/OpenApi/Helpers/OpenApiValidationProblemExample.cs`, and `Binacle.Net/v3/ApiV3Document.cs` /
`v4/ApiV4Document.cs` with their example-response classes. Those last need a host with the document endpoint
mapped, which is gated on `SWAGGER_UI` or `SCALAR_UI`, so they wait on the integration-harness question. The
transformers under `Kernel/OpenApi/Transformers/` are at 100% from `api/test/Binacle.Net.Kernel.UnitTests/OpenApi/`,
against hand-built transformer contexts and no host.

**`binacle-net-ui`'s uncovered third is the Three.js half** — `core/packingVisualizer.ts` and the scene
helpers in `utils/`. They need a WebGL context, so a test there could only assert that a call happened. They
stay in the denominator: excluding them would be the same act as the Sonar coverage exclusions rejected
above, one layer down. The only exclusion anywhere is `.d.ts`, which carries no runtime code.
