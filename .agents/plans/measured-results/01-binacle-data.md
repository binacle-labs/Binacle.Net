---
description: Step 1 - shared/test/Binacle.TestsKernel becomes shared/data/Binacle.Data with one reader that takes an Assembly; then demo-samples gets a provider and its own tests
state: ready
waits-on: "nothing - first step"
horizon: next-release
paths: ["shared/**", "api/test/**", "lib/test/**", "Binacle.Net.slnx", "Directory.Build.props", ".netconfig"]
---

# Step 1 - `Binacle.Data`

Shape: [support-projects.md](support-projects.md). Protocol: the orchestrator beside this folder.

Two commits. The first is the move; the second is demo-samples, because it adds tests that can fail and
each failure is a finding for the maintainer.

## Commit 1 - the move

- `git mv shared/test/Binacle.TestsKernel shared/data/Binacle.Data`; rename the csproj; every namespace
  `Binacle.TestsKernel*` becomes `Binacle.Data*`; one namespace per set (`BischoffSuite`, `CustomProblems`,
  `DemoSamples`).
- `Files/` takes an `Assembly` instead of calling `GetExecutingAssembly()`. Steps 2 and 4 delete their copies
  and call this one. **Read vipaq D10 first**: a shared reader was tried in 2026-07 and reverted, because the
  ViPaq manifest name has four parts (`family.name.algorithm.json`) and the shared one two. The reader
  either serves both shapes or hands the name back unsplit and lets each data project parse it.
- Everything else inside stays for now - the two `EvaluateResult` checks, `TestOperationParameters`,
  `TestAlgorithmFactory`, `AssertionMethodAttribute` all move in step 3. One idea per step.
- Consumers re-point: `Binacle.Net.IntegrationTests`, `Binacle.Lib.UnitTests`, `Binacle.Lib.Benchmarks`,
  `Binacle.Lib.PerformanceTests` - `ProjectReference` path and `using` lines.
- `Binacle.Net.slnx`: a `/shared/data/` folder. `Directory.Build.props`: `/data/` joins the Sonar path
  rule, and its comment names "three test kernels". `.netconfig`: `-*.Data;-*.Testing` join the coverage
  `assemblyfilters`, and the ci-cd design record D19, which quotes that line, is rewritten.
  `tooling/ci/sonar-analysis.xml` line 80 says "the two test kernels".
- Docs: the `shared/data` README gains a row and its three set READMEs say "tests kernel"; the shared
  dependency doc and README; the api dependency doc; the root README; the api integration-tests README;
  lib D3's reader paragraph and vipaq D10 are marked superseded in place; the memory about Sonar issue
  ignores names `shared/test/Binacle.TestsKernel/AssertionMethodAttribute.cs`; one other plan's `paths:`
  names the kernel folder; a README in the new folder. The gate is the list - grep before starting.

## Commit 2 - demo-samples

`All` is only a by-name lookup; every theory enumerates a set provider. So: a key set, a
`DemoSamples` provider, and a `PackingDemoSamplesTests` class in `Binacle.Lib.UnitTests` shaped like the
Bischoff and custom-problems ones. The api suite does not run it - it keys presets by `CustomProblems`. A
scenario that fails is reported to the maintainer as a finding, not dropped.

## Open before starting

- Whether the `Algorithms.` namespace segment goes. It is the only kind left, so it says nothing.
- The class names inside each set namespace. The rule is that the set is not repeated:
  `BischoffSuite.Scenarios`, not `BischoffSuiteScenarioProvider`.
- Whether `TestBin` and `TestItem` keep the `Test` prefix now that nothing else in the project is test-shaped.
- The embedded-resource `LogicalName` prefix: it is what the reader splits on, and a wrong one fails silently.
  Verify with `strings <dll> | grep <prefix>` after building.

## Done when

- [x] `test -d shared/data/Binacle.Data && test ! -d shared/test/Binacle.TestsKernel`
- [x] `grep -rn "Binacle\.TestsKernel" --include=*.cs --include=*.csproj --include=*.slnx --include=*.props --include=*.xml --include=*.md . | grep -v "^./.agents/plans/measured-results\|^./sites\|^./results"`
      is empty.
- [x] `grep -n "Contains('/data/')" Directory.Build.props` hits.
- [x] `grep -n "Data;" .netconfig` hits and `grep -n "\*.Data" .agents/design/ci-cd/decisions.md` hits.
- [x] `grep -rn "GetExecutingAssembly" shared/data/Binacle.Data` is empty.
- [x] `grep -rl "DemoSamples" lib/test/Binacle.Lib.UnitTests/Tests` is not empty.
- [x] `dotnet build api/test/Binacle.Net.IntegrationTests` and `dotnet build lib/test/Binacle.Lib.UnitTests` succeed;
      `just test cs_binacle-lib_unit` runs the demo-samples class.
- [x] `test -f shared/data/Binacle.Data/README.md`
