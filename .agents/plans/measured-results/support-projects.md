---
description: The three tests kernels dissolve into a Data project per data folder, a Testing project per slice, and Reporting - data knows no algorithm, shared references only shared
state: ready
waits-on: "nothing - the shape was agreed 2026-09-19. Steps 1 to 6 of the orchestrator beside this folder build it"
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

## The rules

Four folders, stated by the maintainer 2026-09-19. Sonar already draws the same line: `/test/` and `/tools/`
are support code, and `/data/`, `/measure/`, `/bench/` join that rule in steps 1 and 7.

| Folder | Holds | May reference |
|---|---|---|
| `src/` | the product, or support libraries about the product | `src` |
| `data/` | test data and the code that reads it into models | shared `src`, `data` |
| `test/` | tests of the product, or support libraries about tests | `src`, `data`, `test` support libraries |
| `measure/`, `bench/`, `tools/` | executables that produce something auxiliary - a report, a keeper, a data file | `src`, `data`, `test` support libraries |

Three sentences finish it:

1. **Nothing references a test project or an executable.** A project named `*.UnitTests` or
   `*.IntegrationTests` is a test project: it has the test SDK, `just test` runs it, and no
   `ProjectReference` points at it. Nothing under `measure/`, `bench/` or `tools/` is referenced either.
   Everything else under `test/` is a support library, named for what it holds - `Data`, `Testing`,
   `Reporting` - never for a test. The two `PerformanceTests` projects, which assert nothing, are the ones
   step 7 renames; `TestsKernel` and `TestReporting` said "test" and were not.
2. **A slice references itself and shared, never another slice.** `Binacle.ViPaq.UnitTests` may reference
   `Binacle.ViPaq.Data` and anything under `shared/`; never `lib/data` or lib's `Testing`. The one accepted
   exception is `Binacle.ViPaq.PackedDataGenerator`, which packs with `Binacle.Lib` to make vipaq's data;
   a lib tool writing into vipaq would be worse.
3. **Nothing in `data/` knows an algorithm, a benchmark pick or a generator.** `Binacle.Data` takes
   interfaces, enums and one parser from shared `src` and nothing else; a `Testing` project is the only
   support library that references its slice's `src`.

One slice adds a sentence of its own: **`Binacle.ViPaq.UnitTests` never references `Binacle.ViPaq.Testing`.**
The unit tests are the spec gate and must not lean on the harness's rival encoder. The vipaq doc says this
today as "UnitTests never references the kernel", which also shut out the data; the table lets the data in,
the sentence keeps the encoders out.

Greppable, three lines: no `ProjectReference` under `src/` or `data/` points into `test/`, `measure/`,
`bench/` or `tools/`; no `ProjectReference` anywhere ends in `Tests.csproj`; no `data/` project points
outside shared `src` and `data/`.

When the last step lands, the table and the three sentences go into the repo-wide design record and the
three dependency docs; this file is deleted.

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
    Binacle.ViPaq.UnitTests             -> ViPaq, CompactNotation, ViPaq.Data - never ViPaq.Testing
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
- **`demo-samples` is embedded and read by nothing.** No key, no provider, not in `All` - and `All` is only
  a by-name lookup; every theory enumerates a set provider. So step 1 gives it a key, a provider and a lib
  unit-test class of its own, like the other two sets. The api suite does not run it: it keys presets by
  `CustomProblems`. Any scenario that fails is a finding, not a reason to drop it.
- **`.netconfig`'s coverage filter** names the old assemblies: `-*.TestsKernel;-Binacle.TestReporting`.
  Step 1 adds `-*.Data;-*.Testing`, step 5 swaps in `-Binacle.Reporting`; the ci-cd design record (D19)
  quotes the line and is rewritten each time.
- **Superseded design decisions**, rewritten in place, not deleted: lib D3 "two tests kernels, split by who
  reads the fixtures" and its paragraph "each kernel owns its own embedded-resource reader"; vipaq D10 "ViPaq
  test kernel owns its file plumbing; no shared TestFiles", which records an earlier shared reader that was
  **reverted** - read it before writing the reader in step 1, the four-part manifest name is why it failed;
  vipaq D4 names the kernel's grant. `Directory.Build.props` and `tooling/ci/sonar-analysis.xml` describe
  the kernels by name in comments.
- **`AssertionMethodAttribute`** is a five-line Sonar marker. Each unit-test project keeps its own copy, as
  ViPaq's already does; it leaves the shared kernel in step 3.
- **The docs.** `shared/dependencies`, `lib/dependencies`, `vipaq/dependencies`, the shared README, the lib
  tests doc, the api dependency doc, the two slice READMEs, the root README, the api integration-tests README
  and the three `shared/data/*/README.md` all name the kernels. Each step rewrites the lines it makes false;
  its gate is the list - grep before starting, the lists here are not complete. The last step writes the
  rules into the design record. The note that "each tests kernel owns its own embedded-resource reader
  because `GetExecutingAssembly()`" is wrong and goes: pass the assembly in.
- **`shared/data/README.md`** gains a row for `Binacle.Data/`; the `every-folder-has-a-readme` rule applies to
  the three new project folders.
- One other plan file names `lib/test/Binacle.Lib.TestsKernel`; `grep -rl "Lib.TestsKernel" .agents/plans`
  finds it, and step 2 fixes the path.
- **`tooling/tests.just`** is untouched: no kernel is a test project.

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

## Done when

- [ ] No project or folder is called a tests kernel.
      `find . -iname "*TestsKernel*" -not -path "*/obj/*" -not -path "*/bin/*" -not -path "./.agents/*"` is empty.
- [ ] Every `data/` project references `shared/src` or a data project, never a `src/` outside shared.
      `grep -h "ProjectReference" shared/data/*/*.csproj lib/data/*/*.csproj vipaq/data/*/*.csproj` names no
      `Binacle.Lib.csproj` and no `Binacle.ViPaq.csproj`.
- [ ] One embedded-resource reader in the repo.
      `find . -name EmbeddedResourceFileProvider.cs -not -path "*/obj/*"` lists one file, under `shared/data/Binacle.Data`.
- [ ] No data project references a slice's `src`, and no support project other than `Testing` does.
      `grep -l "Binacle.Lib.csproj" lib/test/*/*.csproj lib/data/*/*.csproj` lists only `UnitTests`,
      `Testing` and, until the results steps move them, `Benchmarks` and `PerformanceTests`; same for ViPaq.
- [ ] The rules are in the design record and this file is gone.
      `test ! -f .agents/plans/measured-results/support-projects.md`
