---
id: api/tests
description: api/test integration tests — layout, v3/v4 HTTP conventions, validBinId, preset keys, special bins, base-class asserts, and test host config
verified: 2026-08-31
check: Test folders mirror api/src/Binacle.Net/v{3,4}/Endpoints/ exactly, and the only folders with a single file are the three Presets ones; validBinId, PresetKeys, special bins, base-class asserts, and the ServiceModule fixture's seeding helpers match api/test/ source
also_update:
  - shared
paths:
  - "api/test/**"

---

# API Tests

Eight projects under `api/test/` — three integration suites, which this doc is about, and five unit suites:

| Project | Covers | Run |
|---|---|---|
| `Binacle.Net.IntegrationTests` | v3 + v4 HTTP endpoints (fit, pack, presets) | `just test cs_binacle-net_integration` |
| `Binacle.Net.ServiceModule.IntegrationTests` | auth token, admin account/subscription (ServiceModule on), rate limiting both ways | `just test cs_binacle-net-service-module_integration` |
| `Binacle.Net.UIModule.IntegrationTests` | which routes answer with a web page, with the demo on and off | `just test cs_binacle-net-ui-module_integration` |
| `Binacle.Net.UnitTests` | `Binacle.Net`'s own options validators, and the forwarded-headers middleware over the options they produce | `just test cs_binacle-net_unit` |
| `Binacle.Net.Kernel.UnitTests` | Kernel features, one folder each (`Network/`, `OpenApi/`, `Paths/`, `Serialization/`) | `just test cs_binacle-net-kernel_unit` |
| `Binacle.Net.DiagnosticsModule.UnitTests` | health check allow-list, middleware, config validators | `just test cs_binacle-net-diagnostics-module_unit` |
| `Binacle.Net.UIModule.UnitTests` | the applet list, the four page models, the error page | `just test cs_binacle-net-ui-module_unit` |
| `Binacle.Net.ServiceModule.UnitTests` | ServiceModule config validators and policies, the password hasher, and `ConcurrentSortedDictionary` | `just test cs_binacle-net-service-module_unit` |

The unit suites need no host and nothing brought up. `Binacle.Net.Kernel.UnitTests` is split by Kernel feature,
each folder holding its own `Tests/` and `Providers/`.

The OpenAPI transformers are tested against a hand-built context rather than a generated document: all three
`OpenApi*TransformerContext` types are sealed with init-only properties and a public parameterless constructor,
so `OpenApi/TransformerContexts.cs` builds them. A failure then names one transformer. The transformers are
`internal`, so the Kernel carries `InternalsVisibleTo`.

`Binacle.Net.UIModule.UnitTests` reaches internal types, because Razor generates internal page classes and the
whole module follows them. The module carries `InternalsVisibleTo`, the same as `Binacle.Net`, the
ServiceModule and the DiagnosticsModule. `FakeWebHostEnvironment` is the one helper: the page models read
`EnvironmentName` and nothing else, so a stub is smaller than a mocking library for one interface.

**Three things in that module are not unit tested, because they need a host.** The two middlewares in
`ModuleDefinition.UseUIModule` are inline lambdas and cannot be constructed on their own; routing and
`.cshtml` rendering need a running app; and whether a reserved path escapes the error page is only observable
end to end. All three are covered by `Binacle.Net.UIModule.IntegrationTests` instead. The prefix matching
underneath them is `ReservedPathOptions`, unit tested in `Binacle.Net.Kernel.UnitTests/Paths/`.

`Binacle.Net.UIModule.IntegrationTests` builds its own `WebApplicationFactory` per feature configuration —
`UIModuleBinacleApi` with `Features:UI_MODULE` on, `UIModuleOffBinacleApi` with it off, both three lines over
a shared `UIModuleApi` base — rather than using the core harness, which runs with every module off. **`Feature.Manager` is process-global static state**, set while
a host builds, so two hosts that disagree about a flag cannot be alive at once: the second to boot answers for
both. Every class sits in one collection with `DisableParallelization = true`. The ServiceModule's
`RateLimiting/` suite is the same arrangement for a different reason, and both are worth reading together
before adding a third module harness.

**Those tests never fetch a bundle or a stylesheet.** `wwwroot/` is generated and gitignored, so a clone that
has not run the javascript build has nothing to serve and every such assertion would be red for the wrong
reason. They assert the page asks for the right path; `tooling/smoke` asserts the file exists, against a built
image.

`ForwardedHeadersPipelineTests` needs framework code to run: `ConfigureForwardedHeaders` only writes options,
and whether a caller actually gets resolved is `ForwardedHeadersMiddleware` acting on them. It calls
`ForwardedHeadersExtensions.Apply(configured, options)` — the mapping split out of the extension for exactly
this — then constructs the framework middleware over a `DefaultHttpContext`, the same `MiddlewareWith` /
`ContextWith` shape the other middleware suites use. No host, no test server, no HTTP client.
`Connection.RemoteIpAddress` is set by hand, where Kestrel would have set it.

`CapturingLogger<T>` in `Binacle.Net.DiagnosticsModule.UnitTests` plays the equivalent role for middleware whose
whole job is to log something: `NullLogger` cannot answer "what did it say", and a mocking library would be a
dependency for one interface.

Both are xUnit over `WebApplicationFactory<IApiMarker>`, registered with `[assembly: AssemblyFixture(...)]`.
The core fixture uses `UseEnvironment("Test")`, a camelCase `JsonSerializerOptions` with `JsonStringEnumConverter`,
and a null logger factory.

`RateLimiting/` in the ServiceModule suite is the exception to both fixtures. Each test builds and disposes its
own `WebApplicationFactory` — `RateLimitedBinacleApi` with the module on and small limits, or
`ServiceModuleOffBinacleApi` with it off — and the three classes share a `RateLimiterCollection` that runs
non-parallel. The routes under test are derived from the route table — the POSTs under `/api/v3` and `/api/v4`,
which must all be limited, and the GETs, which are the preset lists and must not be — with `{preset}` and
`{bin}` filled from the host's own preset options. That factory pins SQLite with its own `DataSource` on every
backend leg.

> `Binacle.Net.ServiceModule.IntegrationTests` picks its database backend from **`BINACLE_TEST_INFRA`** —
> `AzureStorage`, `Postgres`, or `Sqlite`. Unset, it **falls back to SQLite**, so a bare `dotnet test` runs with
> no external service. Only the first two need something up (Azurite on `127.0.0.1:10002`, Postgres on `5432`);
> pick one with `just test cs_binacle-net-service-module_integration [Sqlite|Postgres|AzureStorage]`, which rejects a
> misspelled backend instead of silently falling back.
>
> Each backend has a localhost default connection string, overridden by the production env name
> `AZURESTORAGE_` / `POSTGRES_` / `SQLITE_CONNECTION_STRING` — the same keys the app reads, no test-only
> mechanism. The backend and whether it came from an override are printed to the console on every run, so a
> green run never hides which one it used. **CI runs the suite three times, one step per backend**
> (`.github/workflows/shared-image-tests.yml`), against a Postgres and an Azurite service container that stay up for the
> whole job. `sonar-analysis.yml` runs the same three through `just coverage all sonar all-with-services`,
> against the same two service containers. The defaults match the CI service containers, so CI sets no
> connection string. Locally, `just test all` runs the SQLite test only — it is the set that needs nothing
> brought up. The other two come from `just test all-with-services`, or one at a time with
> `just test cs_binacle-net-service-module_integration <backend>`, after `just serve services-up -d`.

> **Each backend writes its own coverage and result files**, named `Binacle.Net.ServiceModule.IntegrationTests.<backend>`.
> Without that the three runs overwrite each other and only the last one counts.

## Layout — one folder per endpoint

`Tests/v{3,4}/Endpoints/` mirrors `api/src/Binacle.Net/v{3,4}/Endpoints/` exactly: a folder per endpoint class,
named after it, holding that endpoint's tests.

```
api/src/Binacle.Net/v4/Endpoints/Fit/CustomBin.cs
api/test/…/Tests/v4/Endpoints/Fit/CustomBin/FitCustomBinBehavior.cs
                                            /FitCustomBinScenario.cs
```

Each endpoint has up to two files. **Behavior** covers status codes, validation, and the endpoint's own rules.
**Scenario** replays the shared fixture cases (`$shared`) through the route and asserts the algorithm's answer.
**The three `Presets` endpoints have a behavior file only** — `v3 Presets/List`, `v4 Presets/List` and
`v4 Presets/Get`. They run no algorithm, so there is nothing for a scenario to assert, and they are the only
three folders in the tree with one file.

**Namespaces track the folders**, with `Tests/` elided:
`Tests/v4/Endpoints/Fit/CustomBin/` → `Binacle.Net.IntegrationTests.v4.Endpoints.Fit.CustomBin`. `Abstractions/`
and `ExtensionMethods/` sit outside `Endpoints/`, at the version root, and follow the same rule.

Two consequences worth knowing:

- `BinacleApi` and `PresetKeys` live in the root namespace, which is an ancestor of every test namespace, so
  they still resolve with no `using`.
- `ScenarioResultExtensions` does **not** — it sits in `v{3,4}.ExtensionMethods`, a sibling. Every scenario
  test needs `using Binacle.Net.IntegrationTests.v{3,4}.ExtensionMethods;` for `EvaluateResult`. **It is a
  different extension from the shared kernel's**: this one takes the request parameters as well, because the
  scenario's expected result is a map keyed by algorithm and the parameters say which algorithm ran
  (`$shared`). A test that needs the entry itself reaches it as
  `scenario.Result.For(request.Parameters.GetAlgorithm()!.Value)`.

Scenario tests for the multi-bin endpoints send the scenario's single bin as a one-element list, so selection
cannot change the answer and the result must match the single-bin endpoint's. The exceptions are the preset
selecting endpoints (`pack/smallest-bin/{preset}`, `pack/best-bin/{preset}`, `fit/smallest-bin/{preset}`), whose bins
come from config and cannot be reduced to one: they assert the selection invariant instead — if the scenario's
own bin packs fully, the endpoint must return a fully packed result, and for smallest, in a bin no larger.

## Constants

- `validBinId = "60x40x10"` — declared in **v4 only**: `Tests/v4/Endpoints/Fit/PresetBin/FitPresetBinBehavior.cs`
  and `Tests/v4/Endpoints/Pack/PresetBin/PackPresetBinBehavior.cs`. v3 preset tests don't substitute a bin id
  into the route.
- `PresetKeys` (`PresetKeys.cs`): `BiscoffSuite = "biscoff-suite"` (note the misspelling, in both the C# symbol
  and the value), `CustomProblems = "custom-problems"`, `SpecialSet = "special"`.

## Preset bins come from the scenario providers

The `custom-problems` and `biscoff-suite` presets are **not written down anywhere** — `BinacleApi` registers
whatever bins the scenarios use. That set is owned by the scenario data (`$shared`) and grows whenever a
scenario introduces a new bin, so the providers answer for it:

| Call | Gives |
|---|---|
| `CustomProblemsScenarioProvider.GetDistinctBins()` | The bins, one per ID, in the order scenarios introduce them |
| `CustomProblemsScenarioProvider.GetDistinctBinIds()` | Just the IDs — for asserting a preset's contents |
| `CustomProblemsScenarioProvider.GetSmallestBin()` | The least roomy bin; an item that fits it fits them all |
| `BischoffSuiteScenarioProvider.GetDistinctBins()` | Same, for `biscoff-suite` |

`BinacleApi` builds both presets from `GetDistinctBins()`, so a test asserting on a preset reads the same
source it was registered from and the two cannot drift.

**Never write the bin list into a test.** `custom-problems` holds more than the three `60x40x…` bins —
`600x400x300`, `50x50x50`, and others — which is easy to get wrong.

## Special bins

The registered `special` preset has three bins, 60×40 footprint, heights 10/11/12:
`special_bin_1` 60×40×10, `special_bin_2` 60×40×11, `special_bin_3` 60×40×12.
`ListPresets` tests assert the preset contains these three.

`CreateSpecialRequest` request bodies differ by version (because the request shapes differ):

- **v3** sends a `Bins` array of three: `special_bin_1` 10×40×60, `_2` 11×40×60, `_3` 12×40×60.
- **v4** sends a single `Bin`: `{ ID = "special_bin", 10×40×60 }`.
- v4's custom multi-bin endpoints instead take a `Bins` array (`bin_small` 10×40×60 / `bin_medium` 20×40×60 /
  `bin_large` 30×40×60).

Items in both: `special_box_1` 8×40×60 + `box_1` 5×5×5.

## Base-class asserts (v3 vs v4)

Response shapes differ: **v3** wraps results in a `.Data` collection (multi-bin); **v4** returns one bin result
directly, or a `Results` list from the compare endpoints. Both base classes
(`Tests/v{3,4}/Abstractions/BehaviourTestsBase.cs`) provide
`Request_Returns_200Ok / _422UnprocessableContent / _404NotFound`.

| | v3 (`*_ValidateBasedOnParameters`) | v4 (`*_Validate`) |
|---|---|---|
| Fit | 200; `FitResponse` not null; `.Data` not empty; each `Bin` not null | 200; `Bin` not null; `AlgorithmUsed` not empty; `PackedItems` and `UnpackedItems` only `ShouldNotBeNull()` |
| Pack | 200; `.Data` not empty; per bin: if `FullyPacked` → packed not empty + unpacked empty, else unpacked not empty | 200; `Bin` not null; `AlgorithmUsed` not empty; if `FullyPacked` → packed not empty + unpacked empty, else unpacked not empty |

**Asymmetry:** pack verifies the packed/unpacked split keyed on `FullyPacked`; fit only checks presence, not the
emptiness split.

v4 adds `FitCompareRequest_Validate` / `PackCompareRequest_Validate` for the compare endpoints: 200, `Results`
not empty, then the same per-entry asserts across every result.

## Negative tests start from the typed request

A negative test written as an anonymous object - `new { Algorithm = "invalid" }` - keeps compiling after the
contract is renamed, still gets its 422, and passes for the wrong reason from then on. A test that sends the
typed request has the opposite problem: the C# type cannot express a wire value the enum forbids, so that path
goes untested.

Both base classes take the middle. Serialize the typed request, override the one field as raw JSON, post that:

```csharp
var payload = JsonSerializer.SerializeToNode(request, this.Sut.JsonSerializerOptions)!.AsObject();
payload["parameters"]!["algorithm"] = algorithm;
```

Neither suite writes a wire name down. Both take the field as `nameof`, convert it through the serializer's
naming policy, and assert the key is on the payload before replacing it:

| Suite | Helper | Shape |
|---|---|---|
| core, `NegativeRequest.cs` at the project root | `Post_WithFieldValue_Returns_422UnprocessableContent` | `params string[]` of `nameof` segments, so any depth works - `nameof(FitPresetBinRequest.Parameters), nameof(OperationParameters.Algorithm)` |
| ServiceModule, `Endpoints/Admin/AdminEndpointsTestsBase.cs` | `SendWithFieldValueAsync` | one `nameof` and the HTTP verb; the contracts there are flat |

Each version's `BehaviourTestsBase` forwards to `NegativeRequest` rather than holding its own copy, because v3
and v4 differ in the request shape and not in how a field is broken.

Both helpers were verified by breaking a contract on purpose. Adding `[JsonPropertyName]` to
`OperationParameters.Algorithm` - which production compiles straight through - turns 28 v4 tests red with
`'algorithm' is not on the serialized request`. Renaming a property fails the build, though in the core suite
the endpoint handlers fail first, so there the `nameof` is the second line rather than the first.

**`params` takes an empty array happily**, and a call with no segments would post an untouched valid request
and assert nothing. `NegativeRequest` refuses one. That is not hypothetical: the bulk edit that introduced
these call sites missed five files, and this is what caught them.

**Assert the body, not only the status.** Every negative test here has more than one route to its status code,
so the helpers check the error key too - the 422 was right for a while when the key was not (`$api/decisions#D4`).

**The set of enum fields is not written down anywhere.** `RequestEnumConverterTests`, one copy in
`Binacle.Net.UnitTests` and one in `Binacle.Net.ServiceModule.UnitTests`, finds every nullable enum on every
contract by reflection and fails if one does not carry `JsonStringNullableEnumConverter`. A new field is
covered the day it is added; nobody has to remember.

## Test host config

- `BinacleApi.cs` — `ConfigureTestServices` clears `BinPresetOptions.Presets`, then registers `custom-problems`
  (bins from `CustomProblemsScenarioProvider`), `biscoff-suite` (from `BischoffSuiteScenarioProvider`), and
  `special` (the three special bins). Presets come from the shared kernel — see shared (`$shared`).
  Runs with default modules (ServiceModule off), carrying a `// TODO: Run the tests with all modules enabled`.
- `BinacleApiWithoutPresets.cs` — same shape but only clears presets (no registration); tests the no-presets path.
- `Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs` — `IAsyncLifetime`; enables ServiceModule via
  in-memory config (`SERVICE_MODULE=true`, an `AuthToken=NoLimiter::0` rate-limit rule, a connection string
  chosen by `ResolveTestInfrastructure()` from `BINACLE_TEST_INFRA`, JWT issuer and audience `"ForTestsOnly"`
  with a separate 70-plus-character `TokenSecret`). `InitializeAsync` seeds an admin
  (`DefaultAdminAccount`) and a known user; `NonExistentId = EF81C267-A003-44B8-AD89-4B48661C4AA5` is hard-coded.
  Carries the same all-modules TODO. Tests that need their own account seed it per class through
  `EnsureAccountExists`, which takes an optional `AccountStatus` (default `Active`) so a suspended or inactive
  account can be seeded, and drop it again through `EnsureAccountDoesNotExist`.
