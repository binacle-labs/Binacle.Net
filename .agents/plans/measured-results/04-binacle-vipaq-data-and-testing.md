---
description: Step 4 - the ViPaq kernel splits into Binacle.ViPaq.Data (the packs) and Binacle.ViPaq.Testing (the encoders, the picks, the generator)
state: ready
waits-on: "step 3's gate"
horizon: next-release
paths: ["vipaq/**", "Binacle.Net.slnx"]
---

# Step 4 - `Binacle.ViPaq.Data` and `Binacle.ViPaq.Testing`

Shape: [support-projects.md](support-projects.md). Protocol: the orchestrator beside this folder.

## The step

- `git mv vipaq/test/Binacle.ViPaq.TestsKernel vipaq/test/Binacle.ViPaq.Testing` - the bigger half keeps the
  history. Rename the csproj; namespaces.
- New `vipaq/data/Binacle.ViPaq.Data`. Into it: `Scenario`, `PackedDataReader`, the three data providers
  (`BischoffDataProvider`, `CustomProblemsDataProvider`, `DemoSamplesDataProvider`) and the embed of
  `vipaq/data/packed`. Its `Files/` goes; `Binacle.Data`'s reader takes over - the four-part manifest name
  ViPaq parses (`family.name.algorithm.json`) is the one thing that copy did differently, so the reader
  must serve both shapes or the parse moves to `PackedDataReader`.
- Stays in `Testing`: `ViPaqEncoder`, `ViPaqHeader`, `EncoderInfo`, `ProtobufEncoder`, `packing.proto`, the
  three curated providers, `SyntheticDataProvider`. `Testing` references `ViPaq.Data`.
- `Binacle.ViPaq`'s friend grant `$(ProjectName).TestsKernel` becomes `.Testing`.
- Consumers re-point: `Binacle.ViPaq.Benchmarks`, `Binacle.ViPaq.PerformanceTests`.
- `Binacle.Net.slnx`: a `/vipaq/data/` folder. Docs: the vipaq dependency doc and README, the ViPaq
  findings' `check:` line names the kernel; READMEs in both new folders; the `vipaq/data/packed` README.

## Open before starting

- `ScenarioComparison` - data (it compares two scenarios) or testing (the round-trip gates call it)? Read
  the callers and put it where they are.
- Whether `EncoderInfo` stays public; it wraps an internal enum.
- Whether this is one commit or two (the move, then the split). Both are reviewable; the maintainer picks.

## Done when

- [x] `test -d vipaq/data/Binacle.ViPaq.Data && test -d vipaq/test/Binacle.ViPaq.Testing && test ! -d vipaq/test/Binacle.ViPaq.TestsKernel`
- [x] `grep -rln "ViPaqEncoder\|Protobuf\|Curated\|Synthetic" vipaq/data/Binacle.ViPaq.Data --include=*.cs` is empty.
- [x] `grep -n "EmbeddedResource" vipaq/test/Binacle.ViPaq.Testing/*.csproj` is empty.
- [x] `test ! -d vipaq/data/Binacle.ViPaq.Data/Files`
- [x] `grep -n "InternalsVisibleTo" vipaq/src/Binacle.ViPaq/Binacle.ViPaq.csproj` names `.Testing`, not `.TestsKernel`.
- [x] `dotnet build vipaq/test/Binacle.ViPaq.Benchmarks` and `dotnet build vipaq/test/Binacle.ViPaq.PerformanceTests`
      succeed.
