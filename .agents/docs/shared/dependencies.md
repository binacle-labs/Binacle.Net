---
id: shared/dependencies
description: Shared slice dependency tree — Geometry (the BCL-only leaf everything geometric bottoms out on), CompactNotation, Packing, FluxResults, TestReporting, and the algorithm TestsKernel; who references them and who sees internals.
verified: 2026-08-30
check: ProjectReference and InternalsVisibleTo entries in shared/**/*.csproj match the graph and notes below; Binacle.FluxResults carries its own MIT LICENSE and Binacle.Geometry and Binacle.CompactNotation each carry an Apache-2.0 one, and NOTICE names all three; nothing Apache-2.0 here may take a ProjectReference on anything under the repository's code licence
paths:
  - "shared/**"
---

# Shared — project dependencies

The foundation slice. **Nothing here references `api`, `lib` or `vipaq`.** There is no exception: the slice is
the bottom of the stack in both directions, production and test.

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals.

```
Binacle.Geometry                 leaf — BCL only, no Binacle deps
   ▲                             geometry types + IWith[ReadOnly]Dimensions/Coordinates<T>
   │                             (consumed by CompactNotation, Packing, ViPaq)
   │
   ├── Binacle.CompactNotation ──┘   "LxWxH (X,Y,Z) [Q]" parser/formatter
   │      ▲   [IVT → CompactNotation.UnitTests]
   │      └── Binacle.CompactNotation.UnitTests   xUnit
   │
   └── Binacle.Packing ──────────┘   the packing vocabulary: results, identity, status enums
          [IVT → Binacle.Lib, Binacle.Lib.TestsKernel]
          consumers: Binacle.Lib, api DiagnosticsModule + IntegrationTests, Binacle.TestsKernel,
                     Binacle.Lib.TestsKernel, OrLibrary.Converter, ViPaq.PackedDataGenerator
                     (Binacle.Net reaches it transitively and imports it globally - $api/dependencies)

Binacle.FluxResults              leaf — BCL only, no Binacle deps
   ▲                             result/union types: FluxUnion<T0, T1>, the TypedResult structs
   │                             consumers: api ServiceModule.Domain — Infrastructure, ServiceModule and
   │                             SM.IntegrationTests reach it transitively and import it globally
   └── Binacle.FluxResults.UnitTests   xUnit

Binacle.TestsKernel              algorithm fixture hub — Bischoff + custom-problems scenarios
   refs: Binacle.Packing, Binacle.CompactNotation
   consumers: api IntegrationTests, Binacle.Lib.UnitTests/Benchmarks/PerformanceTests

Binacle.TestReporting            leaf — markdown report writer, no Binacle deps
   consumers: Binacle.Lib.PerformanceTests, ViPaq.PerformanceTests, both ViPaq generators, OrLibrary.Converter

shared/tools/Binacle.OrLibrary.Converter   exe tool
   refs: Binacle.CompactNotation, Binacle.Packing, Binacle.TestReporting
```

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.Geometry` | library | — (BCL only) | — | the geometry leaf: `IWith[ReadOnly]*` + `Dimensions<T>`/`Coordinates<T>`/`Item<T>` (Apache-2.0, see note 8) |
| `Binacle.CompactNotation` | library | Geometry | grants IVT to its UnitTests | parses/formats the `LxWxH (X,Y,Z)` compact string |
| `Binacle.CompactNotation.UnitTests` | xUnit exe | CompactNotation | yes | notation tests |
| `Binacle.Packing` | library | Geometry | grants IVT to `Binacle.Lib`, `Binacle.Lib.TestsKernel` | packing result models, identity, status enums |
| `Binacle.FluxResults` | library | — (BCL only) | — | result/union types: `FluxUnion<T0, T1>` + the `TypedResult` structs (see note 7) |
| `Binacle.FluxResults.UnitTests` | xUnit exe | FluxResults | — (public surface only) | union, extension and typed-result units |
| `Binacle.TestReporting` | library | — | — | markdown report writer for the perf harnesses |
| `Binacle.TestsKernel` | library | Packing, CompactNotation | — | algorithm fixtures + providers (see note 3) |
| `Binacle.OrLibrary.Converter` | exe tool | CompactNotation, Packing, TestReporting | — | converts OR-Library benchmark data |

## Notes

1. **`Binacle.Geometry` is the true leaf.** It has no Binacle dependency and only the BCL. Every geometric type in
   the repo (lib, api, vipaq) bottoms out here, which is why it can be shared without dragging anything along.

2. **`Binacle.Packing` is the vocabulary, not the engine.** It holds what a packing *result* is written in —
   `OperationResult`, `PackedBin`, `PackedItem`, the status enums, `IWithID`. The engine interfaces and the
   algorithms live in `Binacle.Lib`, one slice up. The split is what lets the api integration suite and both
   tests kernels assert on results without referencing the packer.

3. **`Binacle.TestsKernel` holds the algorithm fixtures only.** Bischoff suite and custom-problems, embedded by
   link from `shared/data`. It is here rather than in a slice because two slices read it: the api integration
   suite and the lib tests. The result-selection fixtures went the other way — one consumer, so they live in
   `lib/data` and are embedded by `Binacle.Lib.TestsKernel` (see `$lib/dependencies`). Not to be confused with
   `Binacle.ViPaq.TestsKernel`, a separate ViPaq-only hub — see `$vipaq/dependencies`.

4. **Each tests kernel owns its own embedded-resource reader.** `Assembly.GetExecutingAssembly()` resolves to
   the assembly holding the data, so a shared reader would look in the wrong assembly and find nothing. The
   three kernels have deliberately divergent `IFile` shapes for the same reason.

5. **An `InternalsVisibleTo` grant is not a dependency edge.** It annotates one the grantee's `ProjectReference`
   already declares — `Binacle.Packing` granting to `Binacle.Lib.TestsKernel` records that the kernel leans on
   Packing's internals, not that Packing leans on the kernel.

6. **`Binacle.TestReporting` has no Binacle deps** — a plain writer, safe for any harness to reference. It owns
   `RepositoryRoot`/`RepositoryRootLocator`, the repo-root locator the tools and perf harnesses use.

7. **`Binacle.FluxResults` came in from the retired FluxResults NuGet package**, v1.0.0. Same copyright
   holder, and **it keeps that package's MIT licence rather than the repository's** — it carries its own
   `LICENSE` and `NOTICE` names it. Only what the API uses came over: the `FluxResult<T>` wrapper and the
   three- and four-arm unions were dropped. The namespace was renamed to `Binacle.FluxResults`; the type names
   were not. `shared/src/Binacle.FluxResults/README.md` records the provenance.

8. **`Binacle.Geometry` is Apache-2.0, and that is a dependency fact rather than a preference.**
   `Binacle.CompactNotation` and `Binacle.ViPaq` are Apache-2.0 so a third party can read the two data formats
   without taking on the code licence, and both `ProjectReference` this project. An Apache grant over a library
   that cannot be built without a copyleft dependency is not usable, so the leaf moved with them.
   **`Binacle.Packing` referencing it is unaffected** - Apache-2.0 into GPL-3.0 is the permitted direction, and
   the combined binary is still conveyed under the code licence.

9. **The obvious name for it, `Binacle.Results`, does not compile.** Inside `Binacle.Net.ServiceModule` the
   identifier `Results` would then bind to that namespace and shadow ASP.NET's
   `Microsoft.AspNetCore.Http.Results`, breaking every `Results.Ok(...)` in the endpoints. `Binacle.TypedResults`
   collides the same way with `Microsoft.AspNetCore.Http.TypedResults`. Keep `Flux` in the name.
