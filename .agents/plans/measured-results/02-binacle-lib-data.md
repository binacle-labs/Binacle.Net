---
description: Step 2 - lib/test/Binacle.Lib.TestsKernel becomes lib/data/Binacle.Lib.Data and uses the one reader
state: ready
waits-on: "step 1's gate"
horizon: next-release
paths: ["lib/**", "shared/src/Binacle.Packing/**", "Binacle.Net.slnx"]
---

# Step 2 - `Binacle.Lib.Data`

Shape: [support-projects.md](support-projects.md). Protocol: the orchestrator beside this folder.

## The step

- `git mv lib/test/Binacle.Lib.TestsKernel lib/data/Binacle.Lib.Data`; rename the csproj; namespaces
  `Binacle.Lib.TestsKernel*` become `Binacle.Lib.Data*`.
- Its `Files/` copy goes; it references `Binacle.Data` and calls that reader with its own assembly.
- `Binacle.Packing`'s friend grant `Binacle.Lib.TestsKernel` becomes `Binacle.Lib.Data`.
- Consumers re-point: `Binacle.Lib.UnitTests`, `Binacle.Lib.Benchmarks`.
- `Binacle.Net.slnx`: a `/lib/data/` folder.
- Docs: the lib dependency doc, the lib tests doc, the result-selection doc, `docs/build-topology.md`, lib
  D3 (superseded in place), `Directory.Build.props`'s comment; one other plan file names the old path -
  `grep -rl "Lib.TestsKernel" .agents/plans` finds it; a README in the new folder; the
  `lib/data/result-selection` README says "lib tests kernel". One plan file is *named* for the kernel
  (`ls .agents/plans/shared/ | grep -i kernel`); hand the maintainer a `git mv` to a name that says what it
  is about - growing the fixture cases - and fix its title. The gate is the list - grep before starting.

## Open before starting

- Whether the `ResultSelection.` namespace segment stays. It is the only set today; the folder says a second
  could come.

## Done when

- [x] `test -d lib/data/Binacle.Lib.Data && test ! -d lib/test/Binacle.Lib.TestsKernel`
- [x] `test ! -d lib/data/Binacle.Lib.Data/Files`
- [x] `grep -n "InternalsVisibleTo" shared/src/Binacle.Packing/Binacle.Packing.csproj` names `Binacle.Lib.Data`,
      not the kernel.
- [x] `grep -rn "Lib.TestsKernel" --include=*.cs --include=*.csproj --include=*.slnx --include=*.props --include=*.md . | grep -v "^./.agents/plans/measured-results\|^./sites"`
      is empty.
- [x] `ls .agents/plans/shared/ | grep -i kernel` is empty.
- [x] `dotnet build lib/test/Binacle.Lib.UnitTests` and `dotnet build lib/test/Binacle.Lib.Benchmarks` succeed;
      `just test cs_binacle-lib_unit` passes.
