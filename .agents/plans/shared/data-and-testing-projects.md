---
description: The three tests kernels dissolve into a Data project per data folder, a Testing project per slice, and Reporting - data knows no algorithm, shared references only shared
state: ready
waits-on: "nothing - the shape was agreed 2026-09-19. horizon copied from the results plan this is step 0 of; strike it if wrong"
horizon: next-release
paths:
  - "shared/**"
  - "lib/**"
  - "vipaq/**"
  - "api/test/**"
  - "Binacle.Net.slnx"
  - "Directory.Build.props"
---

# Data and Testing projects

Three projects are called a tests kernel today, and each mixes three kinds of thing: the JSON on disk and the
code that reads it, the scenario models, and harness code - assertion helpers, algorithm factories, the
encoders a benchmark drives. The name says which of the three is inside to nobody. The mix also costs real
copies: `Files/` (the embedded-resource reader) exists three times, `AlgorithmFactories.cs` three times byte
for byte, and the benchmark split in the results plan would make that five.

The shape below was agreed 2026-09-19. It is the shape, not the letter: class names and file layout inside a
project belong to the session doing the step, and a decision here that meets a problem in the code is
challenged with the evidence, not built around.

## The three rules

1. **`data/` holds what is on disk and the code that reads it into models.** Nothing in a `data/` project
   knows an algorithm, a benchmark pick or a generator.
2. **`Testing` holds what a slice's tests, measures and benchmarks need to drive its code.** It is the only
   support project that references the slice's `src`.
3. **Shared references shared.** `Binacle.Data` reaches `shared/src` only, and takes only interfaces, enums
   and one parser from it.

When the last step lands, these three lines go into the repo-wide design record and the three dependency
docs; the plan is deleted.

## The tree

Tags: `NEW`, `RENAMED`, `MOVED`, `SPLIT`. No tag, unchanged.

```
shared/
  src/    Geometry  CompactNotation  Packing  FluxResults
  data/
    or-library/  bischoff-suite/  custom-problems/  demo-samples/
    Binacle.Data/                       NEW, was shared/test/Binacle.TestsKernel
      Files/                            the one reader; takes an Assembly
      Scenario  ScenarioMetrics  ScenarioResult  AlgorithmResult  TestBin  TestItem  the parsers
      PercentageComparer
      BischoffSuite/  CustomProblems/  DemoSamples/    one namespace per set
      All                               every scenario, today's AllScenariosProvider, demo-samples included
                                        -> Geometry, CompactNotation, Packing
  test/
    Binacle.CompactNotation.UnitTests  Binacle.FluxResults.UnitTests
    Binacle.Reporting                   RENAMED, was Binacle.TestReporting     -> nothing
  tools/
    Binacle.OrLibrary.Converter         -> CompactNotation, Packing, Reporting

lib/
  data/
    result-selection/
    Binacle.Lib.Data/                   NEW, was lib/test/Binacle.Lib.TestsKernel
      ResultSelection/                  Scenario, the three sets      -> Binacle.Data, Packing (friend)
  src/    Binacle.Lib
  test/
    Binacle.Lib.UnitTests               -> Lib, Binacle.Data, Lib.Data, Lib.Testing
    Binacle.Lib.Testing                 NEW                          -> Lib, Binacle.Data
      AlgorithmFactories                the one copy
      TestAlgorithmFactory  TestOperationParameters
      ScenarioChecks                    the two EvaluateResult extensions, from the shared kernel
      Providers/                        BischoffCurated, CubeScaling, SpecializedScaling, Concurrency
      BenchmarkConfig                   the BDN config, copy 1 of 2
  measure/  bench/  results/            per the results plan; every project there -> Lib.Testing

vipaq/
  data/
    packed/
    Binacle.ViPaq.Data/                 NEW, the data half of vipaq/test/Binacle.ViPaq.TestsKernel
      Scenario                          bin + placed items, ushort
      PackedDataReader
      Packed/                           BischoffSuite, CustomProblems, DemoSamples
                                        -> Binacle.Data, Geometry, CompactNotation
  src/    Binacle.ViPaq                 friends: UnitTests, VectorGenerators, ViPaq.Testing
  test/
    Binacle.ViPaq.UnitTests             -> ViPaq, CompactNotation. No data project: the wall stays
    Binacle.ViPaq.Testing               NEW, the other half of the kernel
      ViPaqEncoder  ViPaqHeader  EncoderInfo  ProtobufEncoder  packing.proto  ScenarioComparison
      Curated/                          the benchmark picks - a choice, not data
      SyntheticScenarios                generated, not on disk, so not data
      BenchmarkConfig                   copy 2 of 2
                                        -> ViPaq (friend), ViPaq.Data, Google.Protobuf
  measure/  bench/  results/            per the results plan; every project there -> ViPaq.Testing
  tools/    PackedDataGenerator  VectorGenerators    -> Reporting, unchanged otherwise

api/test/Binacle.Net.IntegrationTests   -> Net, Packing, Binacle.Data
```

Dependencies, bottom up. Nothing points down or sideways.

```
Geometry -+- CompactNotation -+
          +- Packing ---------+- Binacle.Data -+- Lib.Data --+
                              |                |             +- Lib.Testing --+- Lib.UnitTests
Binacle.Lib ------------------+----------------+-------------+                +- lib measure, lib bench
                              |                |
Binacle.ViPaq ----------------+----------------+- ViPaq.Data -- ViPaq.Testing -+- vipaq measure, vipaq bench
                              |                |
Reporting (leaf) -------------+                +- Net.IntegrationTests
```

## Names, and why

| Was | Becomes | Why |
|---|---|---|
| `Binacle.TestsKernel` | `Binacle.Data` | "Kernel" said nothing; nothing in it is a test. `Data` is the folder it sits in. |
| `Binacle.Lib.TestsKernel` | `Binacle.Lib.Data` | Same; it never referenced `Binacle.Lib`, which the old name implied. |
| `Binacle.ViPaq.TestsKernel` | `Binacle.ViPaq.Data` + `Binacle.ViPaq.Testing` | Half data, half encoders; one name could not say both. |
| `Binacle.TestReporting` | `Binacle.Reporting` | Its `ITest`/`TestRunner` are not tests. It writes reports and finds the repo root; three tools use it for that. `Binacle.Results` shadows ASP.NET `Results`. |
| - | `Binacle.Lib.Testing`, `Binacle.ViPaq.Testing` | The `.Testing` suffix is the .NET convention for "helpers for testing X" (`Mvc.Testing`, `TimeProvider.Testing`). `Fixtures` collides with xunit; `Harness` is what the results plan calls the measure projects. |

Folder to namespace is mechanical: `bischoff-suite` -> `BischoffSuite`, `custom-problems` -> `CustomProblems`,
`demo-samples` -> `DemoSamples`, `result-selection` -> `ResultSelection`, `packed` -> `Packed`.

**The set is in the namespace, so the class does not repeat it.** `Binacle.Data.BischoffSuite.Scenarios`, not
`Binacle.Data.BischoffSuite.BischoffSuiteScenarioProvider`. The two ViPaq "Bischoff" providers are told apart
by where they sit: `ViPaq.Data.Packed.BischoffSuite` is every pack, `ViPaq.Testing.Curated.BischoffSuite` is
the picks.

**Why not a project per set** (`Binacle.Data.BischoffSuite`, ...). Every consumer of the shared sets reads
Bischoff and custom-problems together, mostly through `All`. Two projects would be two builds that no consumer
references apart. A project split earns its keep when consumers differ, and here they differ by slice. If a
set ever gets a consumer of its own, extract it then.

## What moves with it

- **`Directory.Build.props`** marks Sonar support code by path, `/test/` and `/tools/` only. `/data/` joins
  the list in step 1, or Sonar runs the product rules over the data projects.
- **`Binacle.Net.slnx`** gains a `/shared/data/`, `/lib/data/` and `/vipaq/data/` folder; the kernel entries
  leave `/test/`. CI builds the whole solution, so every project builds on every Sonar and image run.
- **Friend grants.** `Binacle.Packing` grants `Binacle.Lib.TestsKernel`; that becomes `Binacle.Lib.Data`.
  `Binacle.ViPaq` grants `$(ProjectName).TestsKernel`; that becomes `.Testing`.
- **`demo-samples` is embedded and read by nothing.** No key, no provider, not in `All`. It gets a key and
  joins `All` in step 1, so the lib unit tests and the api suite run it like the other two sets. Any scenario
  that fails is a finding, not a reason to drop it.
- **`AssertionMethodAttribute`** is a five-line Sonar marker. Each unit-test project keeps its own copy, as
  ViPaq's already does; it leaves the shared kernel in step 3.
- **The docs.** `shared/dependencies`, `lib/dependencies`, `vipaq/dependencies`, the shared README, the lib
  tests doc and the two slice READMEs all name the kernels. Each step rewrites the lines it makes false; the
  last step writes the three rules into the design record. The note that "each tests kernel owns its own
  embedded-resource reader because `GetExecutingAssembly()`" is wrong and goes: pass the assembly in.
- **`shared/data/README.md`** gains a row for `Binacle.Data/`; the `every-folder-has-a-readme` rule applies to
  the three new project folders.
- **The plan about growing the fixture cases** names `lib/test/Binacle.Lib.TestsKernel`; step 2 fixes the path.
- **`tooling/tests.just`** is untouched: no kernel is a test project.

## How a session works this plan

- **One step per session, the maintainer commits between steps.** Nothing here is committed by a session. A
  step is sized to be one reviewable commit: a rename, a move, a split - never a line in three files, and
  never two ideas.
- **Pick up cold by running the checks.** Each step ends in a check. The first step whose check fails is the
  next step. No progress table; the tree is the state.
- **Settle the open details before touching a file.** Each step lists what it leaves open. The session works
  those out with the maintainer first, then does the step. A session that finds the shape wrong says so with
  the evidence and stops.
- **The sandbox denies `mv`, `rm` and `git mv`.** Hand the maintainer the `git mv` lines; edit after they
  land. History follows the bigger half of a split.
- **Build only what moved.** `dotnet build <csproj>` on the moved project and each consumer. A solution build
  is the maintainer's; it has crashed the machine.

## Work order

### 1. `Binacle.Data`

`git mv shared/test/Binacle.TestsKernel shared/data/Binacle.Data`; rename the csproj; namespaces
`Binacle.TestsKernel*` become `Binacle.Data*`; one namespace per set; `Files/` takes an `Assembly`;
demo-samples gets a key and joins `All`. Everything else inside stays for now - the checks and the lib-only
types move in step 3. Consumers re-point: api IntegrationTests, Lib.UnitTests, Lib.Benchmarks,
Lib.PerformanceTests (`ProjectReference` path and `using` lines). `.slnx`, the Sonar path rule, the
`shared/data` README row, the shared dependency doc and README.

Open before starting: whether the `Algorithms.` namespace segment goes (it is the only kind left, so it
carries nothing); the class names inside each set namespace; whether `TestBin`/`TestItem` keep the `Test`
prefix now that nothing else in the project is test-shaped.

- [ ] `test -d shared/data/Binacle.Data && test ! -d shared/test/Binacle.TestsKernel`
- [ ] `grep -rn "TestsKernel" --include=*.cs --include=*.csproj --include=*.slnx shared api lib/test/Binacle.Lib.UnitTests` is empty.
- [ ] `grep -n "/data/" Directory.Build.props` hits the path rule.
- [ ] `grep -rn "DemoSamples" shared/data/Binacle.Data --include=*.cs` shows a key set and a line in `All`.
- [ ] `dotnet build api/test/Binacle.Net.IntegrationTests` and `lib/test/Binacle.Lib.UnitTests` succeed.

### 2. `Binacle.Lib.Data`

`git mv lib/test/Binacle.Lib.TestsKernel lib/data/Binacle.Lib.Data`; rename; namespaces; drop its `Files/`
copy and use `Binacle.Data`'s reader with its own assembly; `Binacle.Packing`'s friend grant renamed.
Consumers: Lib.UnitTests, Lib.Benchmarks. `.slnx`, the lib dependency doc, the fixture-cases plan's path.

Open before starting: whether the `ResultSelection.` segment stays (it is the only set today, but the
folder says a second could come).

- [ ] `test -d lib/data/Binacle.Lib.Data && test ! -d lib/test/Binacle.Lib.TestsKernel`
- [ ] `test ! -d lib/data/Binacle.Lib.Data/Files`
- [ ] `grep -n "InternalsVisibleTo" shared/src/Binacle.Packing/Binacle.Packing.csproj` names `Binacle.Lib.Data`.
- [ ] `dotnet build lib/test/Binacle.Lib.UnitTests` succeeds.

### 3. `Binacle.Lib.Testing`

New project at `lib/test/Binacle.Lib.Testing`, referencing `Binacle.Lib` and `Binacle.Data`. Into it:
`AlgorithmFactories.cs` (one copy; the other two deleted), `TestAlgorithmFactory`, `TestOperationParameters`,
the two `EvaluateResult` extensions and the `OperationResult` extensions they use (from `Binacle.Data`), the
four providers under `Binacle.Lib.Benchmarks/Providers/`. `AssertionMethodAttribute` leaves `Binacle.Data`
for `Binacle.Lib.UnitTests`. After this `Binacle.Data` names no result type. Consumers: Lib.UnitTests,
Lib.Benchmarks, Lib.PerformanceTests. `.slnx`, the lib dependency doc and tests doc.

Open before starting: the file the two checks land in; whether `Binacle.Lib` grants the new project friend
access (the three consumers have it today - find out if the moved code needs it).

- [ ] `test -d lib/test/Binacle.Lib.Testing`
- [ ] `find lib -name AlgorithmFactories.cs -not -path "*/obj/*"` lists one file.
- [ ] `grep -rn "OperationResult\|IOperationParameters\|AlgorithmOperation" shared/data/Binacle.Data --include=*.cs` is empty.
- [ ] `grep -rn "ProjectReference" shared/data/Binacle.Data/*.csproj` names Geometry, CompactNotation, Packing and nothing else.
- [ ] `dotnet build lib/test/Binacle.Lib.UnitTests` and `lib/test/Binacle.Lib.Benchmarks` succeed.

### 4. `Binacle.ViPaq.Data` and `Binacle.ViPaq.Testing`

`git mv vipaq/test/Binacle.ViPaq.TestsKernel vipaq/test/Binacle.ViPaq.Testing` (the bigger half keeps the
history); new `vipaq/data/Binacle.ViPaq.Data`, and into it `Scenario`, `PackedDataReader`, the three data
providers and the embed; its `Files/` goes, `Binacle.Data`'s reader takes over. Curated providers and the
synthetic generator stay in `Testing`. `Binacle.ViPaq`'s friend grant renamed. Consumers: ViPaq.Benchmarks,
ViPaq.PerformanceTests. `.slnx`, the vipaq dependency doc and README.

Open before starting: whether `ScenarioComparison` is data (compares two scenarios) or testing (used by
the round-trip gates) - read its callers; whether `EncoderInfo` stays public.

- [ ] `test -d vipaq/data/Binacle.ViPaq.Data && test -d vipaq/test/Binacle.ViPaq.Testing && test ! -d vipaq/test/Binacle.ViPaq.TestsKernel`
- [ ] `grep -rn "ViPaqEncoder\|Protobuf\|Curated\|Synthetic" vipaq/data/Binacle.ViPaq.Data --include=*.cs` is empty.
- [ ] `grep -rn "EmbeddedResource" vipaq/test/Binacle.ViPaq.Testing/*.csproj` is empty.
- [ ] `dotnet build vipaq/test/Binacle.ViPaq.Benchmarks` and `vipaq/test/Binacle.ViPaq.PerformanceTests` succeed.

### 5. `Binacle.Reporting`

`git mv shared/test/Binacle.TestReporting shared/test/Binacle.Reporting`; rename; namespace. Five consumers:
both PerformanceTests, both ViPaq generators, the OR-Library converter. `.slnx`, the shared dependency doc.
The `ITest`/`TestRunner`/`TestResult` names inside stay until the results plan reshapes the runner.

Small. If the maintainer would rather not commit it alone, it rides with step 4.

- [ ] `test -d shared/test/Binacle.Reporting && test ! -d shared/test/Binacle.TestReporting`
- [ ] `grep -rn "TestReporting" --include=*.cs --include=*.csproj --include=*.slnx --include=*.md . | grep -v "^./.agents/plans\|^./sites"` is empty.

### 6. The record

The three rules go into the repo-wide design record; the three dependency docs and both slice READMEs are
read end to end against the tree, not patched; `just agents all` is the maintainer's. Then this file is
deleted.

- [ ] `grep -n "Testing" .agents/design/decisions.md` finds the three rules.
- [ ] **By eye.** `shared/dependencies`, `lib/dependencies`, `vipaq/dependencies` draw the graph above and no
      kernel.

## Done when

- [ ] No project or folder is called a tests kernel.
      `find . -iname "*TestsKernel*" -not -path "*/obj/*" -not -path "*/bin/*" -not -path "./.agents/*"` is empty.
- [ ] Every `data/` project references `shared/src` or a data project, never a `src/` outside shared.
      `grep -h "ProjectReference" shared/data/*/*.csproj lib/data/*/*.csproj vipaq/data/*/*.csproj` names no
      `Binacle.Lib.csproj` and no `Binacle.ViPaq.csproj`.
- [ ] One embedded-resource reader in the repo.
      `find . -name EmbeddedResourceFileProvider.cs -not -path "*/obj/*"` lists one file, under `shared/data/Binacle.Data`.
- [ ] Only a `Testing` project references a slice's `src` from the support side.
      `grep -l "Binacle.Lib.csproj" lib/test/*/*.csproj` lists UnitTests and Testing;
      `grep -l "Binacle.ViPaq.csproj" vipaq/test/*/*.csproj` lists UnitTests, Testing.
- [ ] The three rules are in the design record and the plan is gone.
      `test ! -f .agents/plans/shared/data-and-testing-projects.md`

## Research

### 2026-09-19 - what each kernel holds, counted

| Project | Lines | Data + reader | Models | Harness |
|---|---|---|---|---|
| `Binacle.TestsKernel` | 780 | 3 sets, `Files/`, providers, keys | `Scenario`, `TestBin`, `TestItem`, metrics, result | `PercentageComparer`, two `EvaluateResult`, `TestAlgorithmFactory`, `TestOperationParameters`, `AssertionMethod` |
| `Binacle.Lib.TestsKernel` | 494 | result-selection, `Files/` | `Scenario` of pre-computed results | - |
| `Binacle.ViPaq.TestsKernel` | 680 | packed, `Files/`, 3 data providers | `Scenario` (placed, ushort) | `ViPaqEncoder`, `ProtobufEncoder`, `.proto`, `ScenarioComparison`, `EncoderInfo`, 3 curated, synthetic |

`Files/` differs between copies only by namespace and, in ViPaq's, by the four-part manifest name it parses.

### 2026-09-19 - who reads which set

| Set | Files | Size | Readers |
|---|---|---|---|
| `bischoff-suite` | 7 | 396K | api integration, lib unit, lib bench, lib perf; ViPaq reads the packed copy |
| `custom-problems` | 3 | 24K | api integration (49 uses), lib unit |
| `demo-samples` | 21 | 92K | none in C# - embedded, no key, no provider |
| `result-selection` | 3 | 32K | lib unit, lib bench |
| `packed` | 93 | 5.3M | ViPaq perf, ViPaq bench |

### 2026-09-19 - what `Binacle.Data` takes from `Packing`, after step 3

`IWithID`, `IWithQuantity`, `OperationResultStatus`, `EarlyExitReason`. Interfaces and enums. The
`OperationResult` uses are all in the two `EvaluateResult` extensions and their helpers, called only from
`Binacle.Lib.UnitTests/CommonTestingFixture.cs`; the api suite has its own `EvaluateResult` over the response
contracts and uses `PercentageComparer` directly.

### 2026-09-19 - the `GetExecutingAssembly` note is a choice, not a constraint

The shared dependency doc says each kernel must own its reader because `Assembly.GetExecutingAssembly()`
resolves to the caller. A reader that takes `typeof(Marker).Assembly` serves any project. The three copies
exist because of that sentence.
