---
description: Step 5 - Binacle.TestReporting becomes Binacle.Reporting; small, may ride with step 4
state: ready
waits-on: "step 4's gate"
horizon: next-release
paths: ["shared/test/**", "lib/test/**", "vipaq/**", "shared/tools/**", "Binacle.Net.slnx"]
---

# Step 5 - `Binacle.Reporting`

Shape: [support-projects.md](support-projects.md). Protocol: the orchestrator beside this folder.

## The step

`git mv shared/test/Binacle.TestReporting shared/test/Binacle.Reporting`; rename the csproj; the namespace.
Five consumers: `Binacle.Lib.PerformanceTests`, `Binacle.ViPaq.PerformanceTests`, both ViPaq generators,
`Binacle.OrLibrary.Converter`. `Binacle.Net.slnx`. `.netconfig`: `-Binacle.TestReporting` becomes
`-Binacle.Reporting`, and the ci-cd design record D19 that quotes it. Docs: the shared dependency doc, the
lib and vipaq dependency docs, `docs/README.md`, `docs/build-topology.md`, `shared/README.md`, the tooling
doc. The gate is the list.

The `ITest`, `TestRunner`, `TestResult` names inside stay. Step 8 reshapes the runner and renames them then.

Small. If the maintainer would rather not commit it alone, it rides with step 4.

## Open before starting

- Nothing. It stays in `shared/test/`: `tools/` holds executables only - the maintainer's rule, 2026-09-19.

## Done when

- [ ] `test -d shared/test/Binacle.Reporting && test ! -d shared/test/Binacle.TestReporting`
- [ ] `grep -n "Binacle.Reporting" .netconfig` hits.
- [ ] `grep -rn "TestReporting" --include=*.cs --include=*.csproj --include=*.slnx --include=*.md . | grep -v "^./.agents/plans/measured-results\|^./sites"`
      is empty.
- [ ] `dotnet build shared/tools/Binacle.OrLibrary.Converter` succeeds.
