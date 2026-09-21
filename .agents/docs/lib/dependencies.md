---
id: lib/dependencies
description: Lib slice dependency tree — Binacle.Lib as the single src project, its own result-selection data project, who sees internals (IVT), and the composition-root rule (only Binacle.Net references the packer).
verified: 2026-09-22
check: ProjectReference and InternalsVisibleTo entries in lib/**/*.csproj match the graph below
paths:
  - "lib/**"
---

# Lib — project dependencies

The bin-packing algorithm layer. Which folder may reference which is the repo-wide rule, `$decisions#D9`; this
file is where the lib projects sit on it. **`lib/src` holds one project, `Binacle.Lib`.** There is no separate
abstractions assembly: the packing vocabulary that callers need moved down into `shared/src/Binacle.Packing`,
and the engine interfaces folded into `Binacle.Lib` itself, under its `Abstractions/` folder.

## The graph

Arrows point at what a project references. `[IVT]` marks who can see a project's internals.

```
Binacle.Geometry                         (shared leaf — see $shared/dependencies)
   ▲
Binacle.Packing ─────────────────────────┘   the packing vocabulary (shared/src)
   ▲   [IVT → Binacle.Lib, Binacle.Lib.Data]
   │
   ├── Binacle.Lib ──────────────────────┘   FFD/WFD/BFD algorithms, processors, result selection
   │      ▲   [IVT → UnitTests, Benchmarks, Testing, Lib.PackingEfficiency]
   │      │       only Binacle.Net references the packer (composition root)
   │      │
   │      ├── Binacle.Lib.Testing           library refs: Lib, Binacle.Data   the factories, checks, benchmark picks
   │      ├── Binacle.Lib.UnitTests         xUnit   refs: Lib, Lib.Testing, Binacle.Data, Lib.Data
   │      ├── Binacle.Lib.Benchmarks        BDN exe refs: Lib, Lib.Testing, Binacle.Data, Lib.Data, Benchmarking   (the classes not yet in lib/bench)
   │      ├── Binacle.Lib.Benchmarks.ResultSelection  BDN exe refs: Lib, Lib.Testing, Lib.Data, Benchmarking   (lib/bench)
   │      └── Binacle.Lib.PackingEfficiency exe     refs: Lib, Lib.Testing, Binacle.Data, Reporting   (lib/measure)
   │
   └── Binacle.Lib.Data ─────────────────┘   result-selection scenario hub (lib/data)
          refs: Binacle.Data (the reader), Binacle.Packing, Binacle.CompactNotation
          embeds lib/data/result-selection under the manifest prefix "ResultSelection."
```

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.Lib` | library | Packing | grants IVT to `Testing`, the two suites and the measure project | the algorithms, processors, result selection |
| `Binacle.Lib.Data` | library | Binacle.Data, Packing, CompactNotation | sees Packing's | result-selection scenarios + set classes |
| `Binacle.Lib.Testing` | library | Lib, Binacle.Data | yes | the one `AlgorithmFactories`, the scenario checks, the benchmark providers |
| `Binacle.Lib.UnitTests` | xUnit exe | Lib, Lib.Testing, Binacle.Data, Lib.Data | yes | algorithm/result unit tests |
| `Binacle.Lib.Benchmarks` | exe | Lib, Lib.Testing, Binacle.Data, Lib.Data, Benchmarking | yes | BenchmarkDotNet timings, the classes not yet in `lib/bench` |
| `Binacle.Lib.Benchmarks.ResultSelection` | exe (`lib/bench`) | Lib, Lib.Testing, Lib.Data, Benchmarking | no | the three result selectors, v1 against v2 |
| `Binacle.Lib.PackingEfficiency` | exe (`lib/measure`) | Lib, Lib.Testing, Binacle.Data, Reporting | yes | packs every scenario, writes `lib/results/` |

`Binacle.Data` above is the shared scenario project in `shared/data` and `Benchmarking` is `shared/test/Binacle.Benchmarking`;
`Lib.Data` and `Lib.Testing` are this slice's own.
`Lib.Testing` needs the friend grant because `AlgorithmFactories` constructs the internal algorithm classes.

## Notes

1. **Composition-root rule.** `Binacle.Net` is the only *application* project that references `Binacle.Lib`,
   and only to wire the packer up. The api `Kernel`, both modules and the integration suite are all off lib
   entirely — what they need is the result vocabulary, and that is `Binacle.Packing` in `shared/src`. Keep it
   that way: a new consumer should take `Binacle.Packing`, not `Binacle.Lib`.

   **Seven projects reference it in total, counted 2026-09-22**, and the other six are not consumers in the
   sense this rule is about: the three `lib/test/*` projects, `lib/bench/Binacle.Lib.Benchmarks.ResultSelection`,
   `lib/measure/Binacle.Lib.PackingEfficiency`, and `vipaq/tools/Binacle.ViPaq.PackedDataGenerator`,
   which is a generator run by hand rather than anything that ships - the one accepted cross-slice reference,
   `$decisions#D9`.

2. **Two data hubs, split by audience.** The shared `Binacle.Data` holds the algorithm scenarios, which the
   api integration suite reads too. `Binacle.Lib.Data` holds result selection, which nothing outside this
   slice reads — so its fixtures live in `lib/data` and it embeds them itself. It reads them with
   `Binacle.Data`'s embedded-resource reader, passing its own assembly, which is why it references
   `Binacle.Data` without reading any of its scenarios.

3. **The friend grant is what lets `Binacle.Lib.Data` fabricate results.** `Binacle.Packing`'s result models
   have internal constructors; `Binacle.Lib.Data` builds them from compact strings, and uses Packing's internal
   `Dimensions` struct to satisfy `PackedBin`. That grant annotates its existing reference to Packing —
   it does not add an edge, and nothing in `shared` depends on `lib` because of it.

4. **Nothing enforces the abstractions boundary.** Only convention stops an interface under
   `Binacle.Lib/Abstractions/` naming a concrete algorithm. That is the sharpest candidate rule for a
   type-level architecture check.
