---
description: Coverage is 67.8% against a gate of 80% on new code - where the 2226 uncovered lines actually are, and the 212 of them that already have tests and only need the services started
state: ready
waits-on: "nothing"
paths:
  - "tooling/coverage.just"
  - "tooling/tests.just"
  - ".github/workflows/sonar-analysis.yml"
  - "api/src/**"
  - "packages/binacle-net-ui/**"
---

# Where the uncovered lines are

**Measured on the `ad2e96b8` run of 2026-08-27: 71.1% overall, 70.6% line. 1892 uncovered of 6429.** The gate
wants 80% on new code and reads **77.0%**. It cannot be lowered - custom quality gates start at the Team plan
and the project is on Free - so this closes by testing, not by configuring.

**334 lines were never coverable** and came out of the denominator through `sonar.coverage.exclusions`: the
python index generator, the three sites' bundles and webpack configs, and the typescript fixture-provider and
generator folders. That alone was 67.1% to 70.6%, with no test written. Everything below is the real
remainder, and the file-level numbers are from the run before the exclusions landed.

## 212 lines already have tests and are not being run

**The biggest single item, and it is not test-writing.** `just coverage all sonar` calls `just test all`, and
`all` runs the ServiceModule integration suite against Sqlite only - Postgres and AzureStorage need
`just serve services-up -d` first, and the Sonar workflow never starts it.

So four repositories sit at 0% while their tests exist and pass:

| File | Uncovered |
|---|---|
| `AzureTablesAccountRepository.cs` | 68 |
| `AzureTablesSubscriptionRepository.cs` | 63 |
| `NpgsqlAccountRepository.cs` | 41 |
| `NpgsqlSubscriptionRepository.cs` | 40 |

That is why `ServiceModule.Infrastructure` reads 25.5%, the worst directory in the repository. Starting the
services in the Sonar workflow and running the suite three times is the whole fix. **The workflow already has
a 45 minute budget and runs on ubuntu-latest, so azurite and postgres are containers it can start.**

## Then, in order of size

- **`packingVisualizer.ts` - 138 of 147 uncovered, 3.9%.** The single worst file. It is also where the
  typescript modernisation findings cluster, so each of those fixes currently costs new-code coverage. The UI
  test harness is what unblocks both.
- **OpenApi document generation - 173 of 194, 10.8%.** `Kernel/OpenApi/Transformers/` is 140 lines at exactly
  0%: the enum-strings, required-nullable and string-number-union transformers. These have one observable
  output - the generated document - and asserting on it is one test per transformer, not a harness.
- **`Kernel/Logs/` - 76 lines at 0%.** `LogsProcessor` and `LogsRetentionProcessor`, both background work.
- **`RequestDebugMiddleware.cs` - 66 lines at 0%**, the whole of `DiagnosticsModule/Middleware` being 50%.
- **The in-memory repositories and the two support models** - `InMemoryAccountRepository` (23),
  `InMemorySubscriptionRepository` (23), `ConcurrentSortedDictionary` (36), `FileHashStore` (25), all at 0%.
  `ConcurrentSortedDictionary` is the one to be careful with: a hand-written concurrent collection at 0% is
  the shape of bug that only appears under load.

## The ten gems contribute nothing either way

Their coverage has never reached SonarCloud, because no `.rb` file has ever been indexed. Until that is
settled the gem suites cannot move this number. `plans/ci-cd/sonar-scope-and-coverage.md` owns it.

## Done when

- [ ] The Sonar workflow runs the ServiceModule integration suite against all three backends.
      `AzureTablesAccountRepository.cs` is not 0% on the Code page after a run.
- [ ] `sonar.coverage.exclusions` covers the four non-product globs and the overall number moved with no test
      written.
      `grep 'coverage.exclusions' tooling/ci/sonar-analysis.xml` names tooling, sites, tests and tools.
- [ ] Each remaining block above is either covered or has a plan of its own naming why it is not.
      **By eye**, on the Code page sorted by uncovered lines.
