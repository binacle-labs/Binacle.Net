---
description: Step 3 - new Binacle.Lib.Testing takes the factories, the checks and the benchmark providers; Binacle.Data names no result type after it
state: ready
waits-on: "step 2's gate"
horizon: next-release
paths: ["lib/**", "shared/data/Binacle.Data/**", "Binacle.Net.slnx"]
---

# Step 3 - `Binacle.Lib.Testing`

Shape: [support-projects.md](support-projects.md). Protocol: the orchestrator beside this folder.

## The step

New project `lib/test/Binacle.Lib.Testing`, referencing `Binacle.Lib` and `Binacle.Data`. Into it:

- `AlgorithmFactories.cs` - one copy; the copies in `UnitTests`, `Benchmarks` and `PerformanceTests` go.
- `TestAlgorithmFactory`, `TestOperationParameters` - from `Binacle.Data`; lib is their only user.
- The two `EvaluateResult` extensions and the `OperationResult` extensions they call - from `Binacle.Data`.
  After this `Binacle.Data` names no result type, and its references are Geometry, CompactNotation, Packing.
- The four providers under `Binacle.Lib.Benchmarks/Providers/` - `BischoffCurated`, `CubeScaling`,
  `SpecializedScaling`, `Concurrency`. They are benchmark choices and the bench split in step 12 needs them
  in one place.
- `AssertionMethodAttribute` leaves `Binacle.Data` for `Binacle.Lib.UnitTests`, its only lib user; ViPaq's
  unit tests already have their own.

Consumers re-point: `Binacle.Lib.UnitTests`, `Binacle.Lib.Benchmarks`, `Binacle.Lib.PerformanceTests`.
`Binacle.Net.slnx`. Docs: the lib dependency doc and tests doc; the shared dependency doc's table row for
`Binacle.Data` loses the harness column; a README in the new folder.

## Open before starting

- The file the two checks land in (`ScenarioChecks.cs` is the shape's guess).
- Whether `Binacle.Lib` grants the new project friend access. The three consumers have it today; find out
  whether any moved line needs it before adding a grant.
- Whether `PercentageComparer` stays in `Binacle.Data`. The api suite uses it directly and it has no
  dependency, so the shape says stay.

## Done when

- [x] `test -d lib/test/Binacle.Lib.Testing`
- [x] `find lib -name AlgorithmFactories.cs -not -path "*/obj/*"` lists one file.
- [x] `grep -rnw "OperationResult\|IOperationParameters\|AlgorithmOperation\|AssertionMethodAttribute" shared/data/Binacle.Data --include=*.cs`
      is empty (`-w`, so `OperationResultStatus`, which stays, does not match).
- [x] `grep -o 'Include="[^"]*csproj"' shared/data/Binacle.Data/*.csproj` names Packing and CompactNotation
      and nothing else; Geometry comes through them.
- [x] `test ! -d lib/test/Binacle.Lib.Benchmarks/Providers`
- [x] `dotnet build` on `Binacle.Lib.UnitTests`, `Binacle.Lib.Benchmarks`, `Binacle.Lib.PerformanceTests`
      succeeds; `just test cs_binacle-lib_unit` passes.
