---
description: One naming rule for every scenario provider in the Data and Testing projects - the namespace says what kind, the class says which one, every class has the same members
state: proposed
waits-on: "the maintainer picks the scope (the table only, or the data projects' members too); he wants consistent names, 2026-09-22"
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

## The rule

**The namespace says what kind of thing it is, the class says which one, and every class has the same
members: `Names`, `GetByName(name)`, `All`.** `Binacle.ViPaq.Data.Packed.BischoffSuite` already works this way.

## The renames

| Today | Proposed | Used by |
|---|---|---|
| `Binacle.Lib.Testing.Providers.SmokeProblemsProvider` | `Binacle.Lib.Testing.Picks.Smoke` | Algorithms smoke |
| `…Providers.BischoffSampleProblemsProvider` | `Binacle.Lib.Testing.Picks.Sample` | Algorithms sample |
| `…Providers.BischoffCuratedProblemsProvider` | `Binacle.Lib.Testing.Picks.Racing` | Racing |
| `…Providers.SpecializedScalingProblemsProvider` | `Binacle.Lib.Testing.Synthetic.Ladder` | Threshold, and the scaling class to come |
| `…Providers.CubeScalingProblemsProvider` | folded into `Picks.Smoke`, its only user | - |
| `Binacle.ViPaq.Testing.Providers.CuratedScenarioProvider` | `Binacle.ViPaq.Testing.Picks.Timing` | Encode, Decode |
| `…Providers.BischoffCuratedProvider` | `Binacle.ViPaq.Testing.Picks.BischoffSuite` | CompressionCost, `CuratedPicksCheck` |
| `…Providers.CustomProblemsCuratedProvider` | `Binacle.ViPaq.Testing.Picks.CustomProblems` | `CuratedPicksCheck` |
| `…Providers.SyntheticDataProvider` | `Binacle.ViPaq.Testing.Synthetic.Scale` | Encode, Decode |

`GetBenchmarkScenarios` on the Racing picks becomes `Names`.

## Open

- **Scope.** The data projects use `GetScenarioNames` and `GetScenarioByName` on every `Scenarios` class in
  `Binacle.Data` and `Binacle.Lib.Data`. Renaming them to `Names` / `GetByName` / `All` makes one vocabulary
  across the repo. It also touches `Binacle.Lib.UnitTests` and `Binacle.Net.IntegrationTests`, which call
  them. The table alone touches only the bench projects, the ViPaq measure project and the two `Testing`
  projects.
- **Class per set, or namespace per set.** `Binacle.Data.BischoffSuite.Scenarios` and
  `Binacle.ViPaq.Data.Packed.BischoffSuite` follow the rule in two shapes. Pick one, or keep both and say why.
- **The largest real pack.** The timing column `largest real pack` is the FFD pack of thpack1_65 (365 items);
  a comment in `SyntheticDataProvider.cs` says the largest is 371 (the BFD pack of the same problem). Pick one
  pack, and name the column for what it is.

## What will bite

- Files move: `git mv` is the maintainer's; hand him the lines, edit after they land, so history follows.
- A kept bench report is named after its bench class, which this rename does not touch. A renamed column
  label (the largest real pack) does change the tables, so a kept run from before reads differently.
- Docs to rewrite in the same change: the lib tests doc, the vipaq dependencies doc, the design record's
  folder decision (the sentence on the curated picks), and the READMEs of both `Testing` projects and the
  bench projects.

## Done when

- [ ] No class under `lib/test/Binacle.Lib.Testing` or `vipaq/test/Binacle.ViPaq.Testing` ends in `Provider`.
      `grep -rn "class [A-Za-z]*Provider\b" lib/test/Binacle.Lib.Testing vipaq/test/Binacle.ViPaq.Testing --include=*.cs`
      is empty.
- [ ] No `GetBenchmarkScenarios` anywhere.
      `grep -rn GetBenchmarkScenarios --include=*.cs .` is empty.
- [ ] Every scenario class in scope exposes `Names` and `GetByName`.
      **By eye**, one class per folder in the table.
- [ ] The design record's folder decision states the rule for `Testing` as well as `Data`.
      **By eye.**
- [ ] Every project that moved builds; `just test cs_binacle-lib_unit` and `just test cs_binacle-vipaq_unit` pass.
