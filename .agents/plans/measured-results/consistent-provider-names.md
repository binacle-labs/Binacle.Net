---
description: Three kinds with three endings - DataProvider holds a committed set, Set picks from one, Generator builds from a number - so a call site says which it got
state: ready
waits-on: "the maintainer says start. Every question is answered, 2026-09-24"
horizon: undecided
paths: ["lib/test/Binacle.Lib.Testing/**", "vipaq/test/Binacle.ViPaq.Testing/**", "shared/data/Binacle.Data/**", "lib/data/Binacle.Lib.Data/**", "vipaq/data/Binacle.ViPaq.Data/**", "lib/bench/**", "vipaq/bench/**", "vipaq/measure/**"]
---

# Consistent provider names

Three schemes are in use for the classes that hand out scenarios:

- lib's benchmark picks: `<Set><Kind>ProblemsProvider` - `BischoffSampleProblemsProvider`.
- ViPaq's: `<Set>CuratedProvider` - `BischoffCuratedProvider`.
- the data projects: a namespace per set and a class `Scenarios` (`Binacle.Data.BischoffSuite.Scenarios`), or a
  class per set (`Binacle.ViPaq.Data.Packed.BischoffSuite`).

The members differ too: `GetScenarioNames` / `GetScenarioByName`, `Names` / `GetByName` / `All`, and one
`GetBenchmarkScenarios`.

The general design record's decision on the four project folders already states the rule for the `Data`
projects: the set is in the namespace, so the class does not repeat it. It was never applied to the
`Testing` projects, and nobody wrote down why.

## The rule - settled 2026-09-23

**Three kinds of class, three endings, so a call site says which one it got.**

| Ending | What it is | Shape |
|---|---|---|
| `DataProvider` | holds a whole committed set, reads it once, hands it out by name. Picks nothing, builds nothing | folder and namespace name the set, the class is `DataProvider`: `BischoffSuite.DataProvider` |
| `Set` | names a handful of ids from a holder and gives each the column name a report prints | one class, `<Which>Set`: `SmokeSet` |
| `Generator` | builds scenarios from a number. Nothing exists until it is asked for | one class, `<Which>Generator`: `LadderGenerator` |

A `Set` and a `Generator` answer a benchmark the same two questions - what are the columns, give me this one -
so both expose `Names` and `GetByName`. A generator that is asked for a size rather than a column keeps its own
members (`GetItems`, `GetBins`). Nothing named `Provider` survives in the `Testing` projects; `DataProvider`
lives only in the `Data` projects.

**One class stays as it is:** `CubeGenerator`, one hand-written best case with one caller. The maintainer's
call of 2026-09-23 - folding it into `SmokeSet` would make that set a generator too, and the rule is worth
more than a 20-line file.

**`BischoffCuratedProvider` splits in two** - the maintainer's call of 2026-09-23. One class holds two
unrelated picks today: `TimingColumns` feeds Encode and Decode, `CompressionCostColumns` feeds CompressionCost,
which is not a timing question. Any single name lies about one half. `BischoffTimingSet` and
`CompressionCostSet` are each exactly true. `OrLibrary_thpack4_1.ffd` appears in both - the typical container
for timing, the low win for compression - which is the same pack being interesting twice, not duplication.
The curated gate then checks three sets instead of two.

**Why not one word.** Calling the synthetic class a set says its data exists somewhere. Its own comment says
the opposite - random packs report the reverse of real behaviour on size, and must never be used for it. The
noun has to carry that.

## Landed

**Slice 1, Bischoff in `Binacle.Data`, 2026-09-24.** `Scenarios` is `DataProvider`, with `Names`, `All`,
`GetByName`, `TheoryNames` (the xUnit `MemberData` wrapper), `Keys` and `GetDistinctBins` unchanged, and a new
`ByCollection(key)` that hands back one thpack. Nine call sites now write `BischoffSuite.DataProvider` and the
`using Binacle.Data.BischoffSuite` imports are gone. The plumbing became `ScenarioCollectionsReader` and
`MultipleScenarioCollectionsReader`, both `internal`, and `PackingRunner` stopped reaching past the holder.
`CollectionScenario.ConnectionKey` was a typo for `CollectionKey` and is fixed. Docs rewritten: the api tests
doc, the shared doc, and `shared/data/Binacle.Data/README.md`, all of which now say the sets are mid-rename.
Not built and not tested - a benchmark was running.

**Slice 2, custom problems in `Binacle.Data`, 2026-09-24.** Same shape, no `ByCollection` - nothing walks its
collections. `GetDistinctBins`, `GetDistinctBinIds` and `GetSmallestBin` keep their names, since they answer
about bins. 26 files swept, 24 of them API integration tests, plus the two lib unit test files and `All.cs`.
Docs: the api tests doc, the shared doc, and the lib design record's `check:` line, which named the two files
by path.

**Slice 3, demo samples in `Binacle.Data`, 2026-09-24.** Two callers only. With it, every set in that project
speaks one vocabulary.

**`Binacle.Data.All`, 2026-09-24.** The aggregate follows the sets - `Names`, `GetByName`, `TheoryNames` - with
one difference the maintainer settled: the values are `All.Scenarios`, because the class is already called
`All` and `All.All` reads as a mistake. 19 files swept.

**Slice 4, the three result-selection sets in `Binacle.Lib.Data`, 2026-09-24.** Taken together: they share a
reader, which became `ScenarioCollectionsReader` and `MultipleScenarioCollectionsReader`, both `internal`, the
same as the shared pair. Callers use a `using` alias, because the bench class and the data namespace both carry
the set's name - `BestBinData` in the bench, the plain set name in `ResultSelectionTests`.

**Slice 5, the three ViPaq packed sets, 2026-09-24.** These already spoke the target vocabulary - they are
where the rule came from - so only the class and the namespace moved: a folder per set under `Packed/`, each
holding a `DataProvider`. Eight files swept across the ViPaq measure project, the unit tests and the two
`Testing` picks. `PackedDataReader` stays where it is; it sits in an ancestor namespace, so the holders still
see it.

**Slice 6, `Binacle.Lib.Testing`, 2026-09-24.** `SmokeSet`, `SampleSet`, `RacingSet`, `CubeGenerator`,
`LadderGenerator`, all at the project root - the `Providers/` folder is gone, and with it the
`Binacle.Lib.Testing.Providers` global using in four bench csprojs. `RacingSet` gained `GetByName(column)` and
made its column-to-id map private, so the Racing benchmark stopped doing the lookup itself.

**Slice 7, `Binacle.ViPaq.Testing`, 2026-09-24.** `TimingSet`, `BischoffTimingSet`, `CustomProblemsTimingSet`,
`CompressionCostSet` (the split) and `SyntheticGenerator`. One shape across all of them: `Names` is the report
columns, `GetByName(column)` the scenario, `PackNames` the picks behind the columns for the curated gate -
which now checks three sets. The `largest real pack` column became `largest FFD pack` in both smoke classes.

**The alias, 2026-09-24.** The first build after slice 7 failed with `CS0103` in every consumer: a using
directive imports a namespace's types, not its nested namespaces, so `using Binacle.Data;` never reached
`BischoffSuite`. The data projects compiled because they sit inside that namespace. Fixed by aliasing the
holder at the top of each caller - `using BischoffSuite = Binacle.Data.BischoffSuite.DataProvider;` - which
also reads better: the line is `BischoffSuite.GetByName(...)`, not `BischoffSuite.DataProvider.GetByName(...)`.
Every project builds.

Everything in this plan has landed, builds and passes. The file can go.

## The renames

| Today | Becomes | Kind | Used by |
|---|---|---|---|
| `Binacle.Lib.Testing.Providers.SmokeProblemsProvider` | `SmokeSet` | set | Algorithms smoke |
| `…Providers.BischoffSampleProblemsProvider` | `SampleSet` | set | Algorithms sample |
| `…Providers.BischoffCuratedProblemsProvider` | `RacingSet` | set | Racing |
| `…Providers.SpecializedScalingProblemsProvider` | `LadderGenerator` | generator | Threshold and Scaling |
| `…Providers.CubeScalingProblemsProvider` | `CubeGenerator` | generator | `SmokeSet` only |
| `Binacle.ViPaq.Testing.Providers.CuratedScenarioProvider` | `TimingSet` | set (joins the two below) | Encode, Decode |
| `…Providers.BischoffCuratedProvider` | split: `BischoffTimingSet` + `CompressionCostSet` | two sets | Encode/Decode through `TimingSet`; CompressionCost; the curated gate |
| `…Providers.CustomProblemsCuratedProvider` | `CustomProblemsTimingSet` | set | Encode/Decode through `TimingSet`; the curated gate |
| `…Providers.SyntheticDataProvider` | `SyntheticGenerator` | generator | Encode, Decode |

The `Providers/` folder in both `Testing` projects goes; the classes sit at the project root, since the ending
already says what each is. `GetBenchmarkScenarios` on the Racing set becomes `Names`.

## The holders - surveyed 2026-09-23

**The rule, the maintainer's call of 2026-09-23:** the folder and namespace name the set, the class is
`DataProvider`. `BischoffSuite.DataProvider.GetScenarioByName(name)` at the call site, with `using
Binacle.Data`. The set is then visible on every line that takes a scenario, which is the point - today the
set lives only in a `using` and the class is called `Scenarios`, so two files away the same word means a
different set. Not started yet.

Nine classes hold data. They take the rule in two amounts of work:

| Class today | Under the rule | Work |
|---|---|---|
| `Binacle.Data.BischoffSuite.Scenarios` | `.BischoffSuite.DataProvider` | rename the class, rename the file |
| `Binacle.Data.CustomProblems.Scenarios` | `.CustomProblems.DataProvider` | same |
| `Binacle.Data.DemoSamples.Scenarios` | `.DemoSamples.DataProvider` | same |
| `Binacle.Lib.Data.ResultSelection.BestBin.Scenarios` | `.BestBin.DataProvider` | same |
| `…ResultSelection.BestAlgorithm.Scenarios` | `.BestAlgorithm.DataProvider` | same |
| `…ResultSelection.SmallestBin.Scenarios` | `.SmallestBin.DataProvider` | same |
| `Binacle.ViPaq.Data.Packed.BischoffSuite` | `.Packed.BischoffSuite.DataProvider` | a folder per set, so the file moves and the namespace grows a level |
| `…Packed.CustomProblems` | `.Packed.CustomProblems.DataProvider` | same |
| `…Packed.DemoSamples` | `.Packed.DemoSamples.DataProvider` | same |

The first six are already folder-per-set, so only the class name changes. The ViPaq three are class-per-set in
one `Packed` namespace, so each needs its own folder - `git mv`, the maintainer's.

**Three classes are not sets and must not take the name:**

- `Binacle.Data.All` - every scenario of every set, by name. It is an aggregate over the three, not a set, and
  it has no folder of its own. It keeps its name. It does change inside: it reads
  `BischoffSuite.Scenarios.Keys` today and would read `BischoffSuite.DataProvider.Keys`.
- `Binacle.Data.ScenarioCollectionsProvider` and `Binacle.Lib.Data.ResultSelection.ScenarioCollectionsProvider`
  - the embedded-resource readers the holders sit on, plus the two internal `MultipleScenarioCollectionsProvider`
  classes beside them. **All four become `...Reader`, and both readers go internal** - the maintainer's call of
  2026-09-23. `Reader` is already this repo's word for the layer: `ScenarioReader` sits next to them and ViPaq
  has `PackedDataReader`.

  The lib reader has no caller outside its project. The shared one does, and it is the only place in the repo
  that goes around a holder: `PackingRunner` in `lib/measure` walks `BischoffSuite.Scenarios.Keys` for the
  collection keys, then asks the reader for each collection's scenarios, because it labels every row with the
  thpack it came from - and the holder flattens all 700 into one dictionary by name, losing the grouping. **The
  holder gains a member that answers it** (a scenarios-by-collection-key lookup), `PackingRunner` uses that, and
  the shared reader goes internal with the other.
- `Binacle.Data.Files.EmbeddedResourceFileProvider` and ViPaq's `PackedDataReader` are plumbing, untouched.

**Members are part of the same rename, sliced by set** - the maintainer's call of 2026-09-23. The six
`Binacle.Data` and `Binacle.Lib.Data` holders expose `GetScenarioNames`, `GetScenarios`, `GetScenarioByName`,
`Keys` and `ScenarioNames`; the ViPaq three already expose `Names`, `All`, `GetByName`, which is the target.
One set per commit: rename the class, rename its members, fix its callers, done. No class is ever
half-converted, and each commit reads. Measured 2026-09-23: 46 files in the repo call these members, 18 of
them in `Binacle.Net.IntegrationTests` and 7 in `Binacle.Lib.UnitTests`.

`Keys` and `ScenarioNames` exist for their consumers, not for the vocabulary - the resource reader and xUnit's
`MemberData`. They keep their jobs whatever they end up called.

`Binacle.Data.All` reads all three shared sets, so the first three slices each touch it - three lines in its
constructor.

**It propagates, one set at a time.** No file in the repo uses two of the nine, so the classes can be renamed
in any order without anyone having to qualify a name. Bischoff first was the maintainer's pick; stopping
part-way leaves `BischoffSuite.DataProvider` beside `CustomProblems.Scenarios`, which is a pause, not a
destination.

## The sets - surveyed 2026-09-23

Nine classes in the two `Testing` projects. They are not one kind of thing; they are three, and one word
cannot cover them honestly.

**Five pick from a holder.** They name a handful by id and give each a column name the report prints.

| Class | Picks | From |
|---|---|---|
| `SmokeProblemsProvider` | 4 columns | two ids out of Bischoff, two from the generators below |
| `BischoffSampleProblemsProvider` | 30 ids | Bischoff |
| `BischoffCuratedProblemsProvider` | 5 ids, with the fill each algorithm reaches | Bischoff, resolved by the caller |
| `BischoffCuratedProvider` (ViPaq) | 3 packs | ViPaq packed Bischoff |
| `CustomProblemsCuratedProvider` (ViPaq) | 4 packs | ViPaq packed custom problems |

**Three build scenarios from a number.** Nothing is picked; the data does not exist until it is asked for.

| Class | Builds |
|---|---|
| `CubeScalingProblemsProvider` | one cube baseline |
| `SpecializedScalingProblemsProvider` | the item and bin ladders, by count |
| `SyntheticDataProvider` (ViPaq) | deterministic random packs at a given item count |

**One joins the others.** `CuratedScenarioProvider` (ViPaq) puts the two curated sets and the synthetic curve
in report order; it holds no ids of its own.

**What this means for the name.** A picked set and a generated one answer the same question to a benchmark -
give me the columns, give me this one - so they can share the member names. They cannot share a noun: a
generator picks nothing, and calling it a set says the data exists somewhere, which is the mistake the
synthetic class's own comment warns about. Two nouns, one for each, with the joiner taking the picked one.

## Settled, nothing open

**The `largest real pack` column becomes `largest FFD pack`** - the maintainer's call of 2026-09-24. The label
points at `OrLibrary_thpack1_65.ffd`, 365 items. Measured from the size results on 2026-09-23: the same problem
packed by BFD holds 371 items and is the largest pack in the data, which is what the synthetic generator's
comment means when it says 371. The pick stays FFD, because every ViPaq timing column is an FFD pack and
mixing one in would stop the columns comparing; only the label was loose. It is one dictionary key, and it
changes the column a kept run prints.

## What will bite

- Files move: `git mv` is the maintainer's; hand him the lines, edit after they land, so history follows.
- A kept bench report is named after its bench class, which this rename does not touch. A renamed column
  label (the largest real pack) does change the tables, so a kept run from before reads differently.
- Docs to rewrite in the same change: the lib tests doc, the vipaq dependencies doc, the design record's
  folder decision (the sentence on the curated picks), and the READMEs of both `Testing` projects and the
  bench projects.

## Done when

- [x] No class under `lib/test/Binacle.Lib.Testing` or `vipaq/test/Binacle.ViPaq.Testing` ends in `Provider`;
      every one ends in `Set` or `Generator`.
      `grep -rn "class [A-Za-z]*Provider\b" lib/test/Binacle.Lib.Testing vipaq/test/Binacle.ViPaq.Testing --include=*.cs`
      is empty.
- [x] Every holder is called `DataProvider`, and no `Scenarios` class is left in the three data projects.
      `grep -rn "class Scenarios\b" shared/data lib/data vipaq/data --include=*.cs` is empty.
- [x] No `GetBenchmarkScenarios` anywhere.
      `grep -rn GetBenchmarkScenarios --include=*.cs .` is empty.
- [x] Every `Set` and every `Generator` that answers columns exposes `Names` and `GetByName`.
      **By eye**, one class per row in the renames table.
- [x] Every set names its holder on the line that takes a scenario - no set reads a holder through a `using`
      that hides which set it is.
      **By eye**, read the dictionary at the top of each set.
- [x] The design record's folder decision states the rule for `Testing` as well as `Data`.
      **By eye.**
- [x] Every project that moved builds; `just test cs_binacle-lib_unit` and `just test cs_binacle-vipaq_unit` pass.
      Done 2026-09-24: every project built one at a time, lib unit passed 9,001 tests, the maintainer confirmed
      the ViPaq suite passed.
