---
id: vipaq/dependencies
description: ViPaq project dependency tree — who references whom, who can see internals, and the deliberate walls (UnitTests references ViPaq.Data, never Testing; no test project references a generator).
verified: 2026-09-22
check: ProjectReference and InternalsVisibleTo entries in vipaq/**/*.csproj match the graph and the boundary rules below; the pack count and the empty-pack count match the entries in vipaq/data/packed/**/*.json across all three families (bischoff-suite, custom-problems, demo-samples); the pre-report gate matches vipaq/measure/Binacle.ViPaq.EncodedSize/PreReportChecks/; the real-pack theories in vipaq/test/Binacle.ViPaq.UnitTests/Tests/Packed/ cover every family and the modes and codecs named below
paths:
  - "vipaq/**"
---

# ViPaq — project dependencies

One picture of how the ViPaq projects fit together, plus the boundaries that are easy to break by accident.
Which folder may reference which is the repo-wide rule, `$decisions#D9`; this file is where the ViPaq projects
sit on it.
The *why* of the format is in `$vipaq/architecture` and the design decisions behind it; this file is
just the wiring.

## The graph

Arrows point at what a project references. `[IVT]` marks a project that can see `Binacle.ViPaq` internals
(`Header`, `ProtocolEncoder`, `Width`, the codecs).

```
Binacle.Geometry                    leaf — geometry types + IWith[ReadOnly]Dimensions/Coordinates
   ▲   ▲   ▲
   │   │   └── Binacle.CompactNotation      "LxWxH (X,Y,Z)" parser/formatter
   │   │
   │   ├────── Binacle.ViPaq.Data  (no IVT)  library — the real packs
   │   │           refs: Binacle.Data (the embedded-resource reader), Geometry, CompactNotation
   │   │           owns: the 2,322 frozen packs, Scenario, one class per family under Packed/
   │   │              ▲
   │   └────── Binacle.ViPaq  [grants IVT]  the format (reference implementation)
   │              ▲   ▲   ▲   ▲
   │              │   │   │   │
   │              │   │   │   └── Binacle.ViPaq.UnitTests        [IVT]  xUnit — spec/correctness
   │              │   │   │           refs: ViPaq, ViPaq.Data, CompactNotation
   │              │   │   │           NO ref to Testing (deliberate)
   │              │   │   │
   │              │   │   └────── Binacle.ViPaq.Testing          [IVT]  library — the harness's encoders
   │              │   │               refs: ViPaq, ViPaq.Data, Geometry, CompactNotation
   │              │   │               owns: ViPaqEncoder/ViPaqHeader (drives ProtocolEncoder), protobuf, JSON,
   │              │   │                     compact, EncoderInfo, the curated and synthetic picks
   │              │   │                  ▲          ▲
   │              │   │                  │          └── Binacle.ViPaq.EncodedSize  [IVT]  exe (vipaq/measure)
   │              │   │                  │                  refs: Testing, ViPaq.Data, Reporting
   │              │   │                  │                  runs the curated-picks gate, writes vipaq/results/encoded-size.md
   │              │   │                  │
   │              │   │                  └───────────────── Binacle.ViPaq.Benchmarks    [IVT]  exe (vipaq/bench)
   │              │   │                                          refs: Testing, ViPaq.Data, Benchmarking
   │              │   │
   │              │   └── Binacle.ViPaq.VectorGenerators  [IVT]  tool exe — regenerates test-vectors/
   │              │           refs: ViPaq, CompactNotation, Reporting
   │              │
   │              └────── Binacle.ViPaq.PackedDataGenerator  (no IVT)  tool exe — freezes data/packed/
   │                          refs: Lib, Packing, ViPaq, CompactNotation, Geometry, Reporting
   │
   └── lib/src/Binacle.Lib                     the packing engine — reached only by PackedDataGenerator
```

`Binacle.Reporting` (a shared markdown-report writer) is referenced by EncodedSize and both generators.

## Projects at a glance

| Project | Kind | References | Sees internals | Role |
|---|---|---|---|---|
| `Binacle.ViPaq` | library | Geometry | grants IVT | the format; everything but the public surface is `internal` |
| `Binacle.ViPaq.UnitTests` | xUnit exe | ViPaq, ViPaq.Data, CompactNotation | yes | spec/correctness — vectors + curated inputs, plus every real pack round-tripped |
| `Binacle.ViPaq.Data` | library | Binacle.Data, Geometry, CompactNotation | **no** | the 2,322 real packs as scenarios, one class per family |
| `Binacle.ViPaq.Testing` | library | ViPaq, ViPaq.Data, Geometry, CompactNotation | yes | the harness's encoders - ViPaq, protobuf, JSON, compact - and the curated and synthetic picks |
| `Binacle.ViPaq.EncodedSize` | exe (`vipaq/measure`) | Testing, ViPaq.Data, Reporting | yes | the curated-picks gate, then `encoded-size.md` into `vipaq/results/` |
| `Binacle.ViPaq.Benchmarks` | exe (`vipaq/bench`) | Testing, ViPaq.Data, Benchmarking | yes | BenchmarkDotNet timings |
| `Binacle.ViPaq.VectorGenerators` | tool exe | ViPaq, CompactNotation, Reporting | yes | regenerates `test-vectors/` |
| `Binacle.ViPaq.PackedDataGenerator` | tool exe | Lib, Packing, ViPaq, CompactNotation, Geometry, Reporting | **no** | packs problems offline, freezes `data/packed/` |

## The walls (easy to break, deliberate)

1. **UnitTests references `ViPaq.Data`, never `Testing`.** UnitTests is the spec gate: it proves the code obeys
   `PROTOCOL.md` using the shared cross-language vectors, its own curated inputs and the real packs. The packs
   are inputs, not a rival encoder; `Testing` holds the harness's encoder, and the spec gate must not lean on
   it. The reasoning is `$vipaq/decisions#D18`.

2. **ViPaq.Data holds the real packs and nothing else; Testing holds the harness's encoders.** `ViPaq.Data`
   does not reference `Binacle.ViPaq` and has no internals grant - it is inputs only. `Testing` reaches the
   internal `ProtocolEncoder` through its own thin `ViPaqEncoder`/`ViPaqHeader`, so every mode (each codec,
   each layout) is forceable, and holds the three rival encoders the size file compares against: protobuf (the
   fair format comparison, same codec both sides), JSON and compact notation (the text a user's token replaces,
   bin and placed items only). Only Benchmarks and EncodedSize reference `Testing`.

3. **PackedDataGenerator has no internals grant.** It produces the frozen data through the public surface and the
   packing engine only. It must never reach into ViPaq internals — the data has to be generatable the way any
   caller would.

4. **Two doors into the internal `ProtocolEncoder`** — and they stay apart: the UnitTests fixture
   (`ProtocolTestingFixture` - curated and vector inputs, and the real packs in the forced-width theory) and
   `Testing`'s `ViPaqEncoder` (the harness's real-pack inputs).
   The UnitTests fixtures are split by which door a test goes through: `ProtocolTestingFixture` drives
   `ProtocolEncoder` (a header is an input, so a test can force a columnar or wider blob) and
   `ViPaqSerializerTestingFixture` drives the public `ViPaqSerializer` (which picks its own header).
   `BinContents` holds what both need — the bin/item builders, and the field-by-field `AssertSame`.

5. **No test project references a generator.** The generators are standalone tools: they write `test-vectors/`
   and `data/packed/`, and the suites read those files. A `ProjectReference` from a test project to a tool —
   or a TS test importing the generator's parser — drags a CLI tool into the test build and lets a broken tool
   fail the suite for a non-product reason. Shared grammar goes in the library both sides already reference
   (`Binacle.CompactNotation`), never across this line.

## The real-pack round trips, and the one gate left

Every real pack is round-tripped in `Binacle.ViPaq.UnitTests/Tests/Packed/PackedDataRoundTripTests.cs`, one
theory row per pack from `PackedScenarioProvider` (all three families through `ViPaq.Data`), so `just test`
runs it every time:

| Theory | What it sweeps |
|---|---|
| `Serializer_Round_Trips_In_Every_Mode` | all 2,322 packs through the public `ViPaqSerializer`, raw and deflate × both layouts - the four modes a caller can ask for |
| `Gzip_Round_Trips_In_Both_Layouts` | all 2,322 packs through `ProtocolEncoder` with the gzip codec × both layouts. The serializer never picks gzip, but the size report compares it, so it is tested |
| `Forced_Sixteen_Bit_Widths_Round_Trip` | the non-empty packs forced to 16-bit widths through `ProtocolTestingFixture`, which hands `ProtocolEncoder` a header, × raw, deflate and gzip × both layouts, so the 16-bit read path is exercised on real data the serializer would never widen |
| `Every_Family_Loads` | a fact, not a theory: each family has at least one pack. A family whose resources are misnamed loads empty and would add no rows to the theories above |

The forced-width theory **skips the nine empty packs** — six in custom-problems, three in demo-samples: §4 keeps
both item widths `Eight` for an empty pack, so a forced-wide empty blob is something `Encode` rejects by
design. Bischoff has none.

All three theories use one oracle: the two header bytes must decode back to the expected header - the one the
serializer must produce, or the forced one (`Header.FromBytes`, `Header.ByteCount` is 2) - **and** the pack must decode back to the input
(`BinContents.AssertSame`). Compressed bytes are never compared.

The one `IPreReportCheck` left in `Binacle.ViPaq.EncodedSize/PreReportChecks/` is `CuratedPicksCheck`: every
curated benchmark pick still resolves to a generated scenario, so a stale pick fails in one sentence instead of
deep inside a BenchmarkDotNet run. It stays in the measure project because the picks live in `Testing`, which
the unit tests never reference. `RunPreReportChecks()` runs it before `Measure` runs the reporters.
