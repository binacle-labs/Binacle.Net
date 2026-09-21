---
description: Findings from the 2026-09-22 review of step 12 as landed - the tooling the maintainer wants reworked, what must be fixed before the commit, and what can wait. A finding leaves here when it is fixed.
state: ready
waits-on: "the maintainer picks the tooling shape; the before-commit list can start now"
horizon: now
paths: ["tooling/bench.just", "tooling/bench.run.sh", "lib/bench/**", "vipaq/bench/**", "shared/test/Binacle.Benchmarking/**", ".agents/docs/**"]
---

# Step 12 - findings

Every claim below was confirmed with `just -n` or a `--list` run on 2026-09-22; nothing was benchmarked.

## 1. The tooling gets reworked - the maintainer's call, 2026-09-22

He does not want **the shell script** (`tooling/bench.run.sh`) and does not want **BenchmarkDotNet categories**
(`[BenchmarkCategory]` on the classes, `--allCategories` from the recipes) as the way a tier or a word selects
its cases. Both go. The plan's "Narrowing" paragraph (settled again 2026-09-22 in `12-bench-split.md`) is
therefore open again.

What the review found wrong with the shape as built, each one a thing a user will hit:

- **A `job=` recipe eats the first word as the job.** `just bench lib-algorithms-full ffd packing` runs
  `--job ffd`. `just bench vipaq --iterationTime 100` runs `--job --iterationTime`. Three recipes have this:
  `lib-algorithms-full`, `lib-threshold-full`, `vipaq`.
- **`job="short"` cannot be typed.** It is `just`'s display form of a positional default; `just bench vipaq
  job=short` passes the literal string `job=short`. Every doc that shows `job="short"` teaches the broken form:
  `lib/bench/Binacle.Lib.Benchmarks.Algorithms/README.md`, `.../Threshold/README.md`, `tooling/README.md`,
  `.agents/docs/commands.md`, `.agents/docs/tooling/README.md`, `tooling/bench.just` itself. Only
  `vipaq/bench/Binacle.ViPaq.Benchmarks/README.md` writes the working form, `just bench vipaq short`.
- **Every mistake is silent.** BDN exits 0 on an invalid job ("The provided base job "ffd" is invalid"), on a
  filter that matches nothing, and on a parse error. `Program.cs` in all five projects drops what `Run`
  returns. So a wrong word runs nothing and the recipe reports success.
- **The algorithm words select nothing on four of five binaries.** `ffd`, `bfd`, `wfd` are categories only in
  Algorithms; in Racing and Threshold the algorithm is a column, in ResultSelection and vipaq there is none.
  `just bench lib-racing bfd` runs nothing, exits 0.
- **`just bench` lists alphabetically**, so `lib-algorithms` (the alias, no cost in its comment) sits above
  `-full`, `-sample`, `-smoke`, and `default` shows as a recipe.
- Three recipe shapes for one job (`*words`, `job="default" *words`, an alias), one inline path where the
  rest use variables, 25 lines of bash to put `--allCategories` in front of a list, and the full recipe's
  `&&` body echoing its own comment.

Candidate shape, not decided. It keeps what is settled (a recipe per binary and tier, the plain name as the
default tier, the tier in the class name) and drops the script and the categories:

- The tier is selected by **`--filter` on the class name**, which already carries it: `'*.Smoke_*'`,
  `'*.Sample_*'`, `'*.Full_*'`; Threshold's sample tier is two globs (`'*.Algorithms_Packing_v2.*' '*.Sample_Bins_*'`).
  BDN ORs its globs, which is what a tier wants. The `[BenchmarkCategory]` attributes go.
- The narrowing words go, or exist only on Algorithms as a documented `--filter` the user types himself.
- No positional job. The job and any extra BDN flag come from the environment, the `DOTNET_TEST_ARGS`
  precedent in `tests.just`: `BDN_JOB=short just bench vipaq`, `BDN_ARGS='--iterationTime 100' ...`.
- Each recipe is one `dotnet run -c Release --project <path> -- --job <job> --filter <globs>` line, the way
  `measure.just` writes its runs. No script.
- `default` lists with `--unsorted`, so the file order is the tier order.
- `Program.cs` exits 1 when `Run` returns nothing, so an invalid job or an empty filter fails the recipe.

## 2. Before the commit

- [ ] `lib/bench/README.md` says ResultSelection v2 is "allocation-free"; the report from 2026-09-22 shows
      24 B on every v2 case. Drop the word.
      `grep -n "allocation-free" lib/bench/README.md` is empty.
- [ ] `.agents/docs/build-topology.md` was re-verified today but says 51 projects and "three benchmark
      projects"; the slnx has 56 and there are five. Fix the counts or put `verified:` back to 2026-09-20.
      `grep -c "<Project " Binacle.Net.slnx` matches the number in the doc.
- [ ] `.agents/docs/shared/dependencies.md` lists `Binacle.Benchmarking`'s consumers as "Lib.Benchmarks,
      Lib.Benchmarks.ResultSelection, ViPaq.Benchmarks" - the first does not exist and three are missing.
      **By eye**, against `grep -rl Binacle.Benchmarking --include=*.csproj lib vipaq`.
- [ ] `.agents/docs/README.md` says thirteen just modules and "the benchmark scripts", and that benchmarks
      live under `lib/test/` and `vipaq/test/`. Fourteen, no scripts, `bench/`.
      `grep -n "thirteen\|benchmark scripts\|lib/test/Binacle.Lib.Benchmarks" .agents/docs/README.md` is empty.
- [ ] `.agents/design/vipaq/findings.md` still says in the present tense that the benchmarks fan out over
      `UncompressedNames` and names `CuratedEncodeBenchmarks`; the history went into `check:` instead of the
      body. A dated note in the body; `check:` says only what to confirm now.
      `grep -n "UncompressedNames\|CuratedEncodeBenchmarks" .agents/design/vipaq/findings.md` hits only inside a dated note.
- [ ] `lib/test/Binacle.Lib.Testing/README.md` says a provider gives "the core count"; `ConcurrencyProvider`
      is gone.
      `grep -n "core count" lib/test/Binacle.Lib.Testing/README.md` is empty.
- [ ] Every `job="short"` in a doc or recipe comment becomes the form that works, or goes with the rework
      (section 1).
      `grep -rn 'job="short"' lib/bench vipaq/bench tooling .agents/docs` is empty.
- [ ] `.agents/docs/commands.md` and `tooling/README.md` claim a `-flag` passes through on every recipe; it is
      eaten on the three `job=` recipes. Say so, or fix it with the rework.
      **By eye.**
- [ ] `lib/bench/*/Properties/launchSettings.json` - four copies of the old project's profile, named
      `Benchmarks`; the vipaq project has none. One `git rm` line for the four, the maintainer's.
      `ls lib/bench/*/Properties 2>/dev/null | wc -l` is 0.
- [ ] `lib/bench/Binacle.Lib.Benchmarks.ResultSelection/Binacle.Lib.Benchmarks.ResultSelection.csproj`
      references `Binacle.Lib.Testing` and uses nothing from it.
      `grep -c Binacle.Lib.Testing lib/bench/Binacle.Lib.Benchmarks.ResultSelection/*.csproj` is 0, and the project builds.

## 3. Later

- [ ] The smoke classes have one baseline, `FFD_v1`, so every row's Ratio is against FFD and "did my WFD
      change help" is two Means read by eye. `CompressionCost` has one baseline, `Encode_NoOp`, so the
      `Decode_*` rows get a Ratio against an encode. Fix: the orderer groups by params plus a key, and each
      algorithm or direction carries its own baseline (BDN allows one per logical group).
      **By eye** in a smoke report: a Ratio of 1.00 on `WFD_v1` and `BFD_v1` as well as `FFD_v1`.
- [ ] `vipaq/test/Binacle.ViPaq.Testing/Providers/BischoffCuratedProvider.cs` calls the FFD pack of
      thpack1_65 (365 items) `largest real pack`; the BFD pack of the same problem has 371. Rename the column
      or pick the BFD pack.
- [ ] `shared/test/Binacle.Benchmarking/AttributeOrderer.cs` has a comment naming `NoOfItems`, a param that
      exists nowhere, and two that restate the code.
- [ ] `.agents/docs/vipaq/dependencies.md` says two things about what the bench references: the tree line
      says `Testing, ViPaq.Data (BenchmarkDotNet)`, the table says `Benchmarking`.
- [ ] `Directory.Build.props` and `tooling/ci/sonar-analysis.xml` carry history comments naming "twelve
      projects", `BestBin_ResultSelection`, "performance suites".
- [ ] `vipaq/test/Binacle.ViPaq.Testing/README.md` says "benchmarks and performance tests"; the measure
      project has had its name since step 7.
- [ ] `BischoffCuratedProblemsProvider.GetBenchmarkScenarios` - every sibling is `GetScenarioNames`.
- [ ] `just agents all` - `.agents/docs/_index.md` still describes lib/tests with the old wording. The
      maintainer's.
- [ ] Not a bug, a finding for `design/lib/findings.md`: the 2026-09-22 ResultSelection report shows
      BestAlgorithm v2 1.4-1.7x slower than v1 on the two scenarios where v1 stops at the first full result.
      Nanoseconds at three candidates; worth one line.

## Done when

- [ ] Every box above is ticked or its line has moved into the plan it belongs to, and this file is deleted.
      **By eye.**
