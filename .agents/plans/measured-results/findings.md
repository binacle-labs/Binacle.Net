---
description: Findings from the 2026-09-22 reviews of steps 1 to 12 as landed - the bench tooling the maintainer wants reworked, what broke or lost coverage, what text is false, where the build drifted from the shape. A finding leaves here when it is fixed.
state: ready
waits-on: "the maintainer picks the bench tooling shape; everything else can start now"
horizon: now
paths: ["shared/**", "lib/**", "vipaq/**", "tooling/**", "results/**", "artifacts/README.md", ".agents/docs/**", ".agents/design/**", ".agents/memory/**", "Directory.Build.props", "Directory.Packages.props"]
---

# Steps 1 to 12 - findings

Two reviews on 2026-09-22: one of step 12 alone, then one of steps 1 to 12 against the shapes as first
written (`git show f7a5698c:.agents/plans/measured-results/results.md`, and the support-projects shape before
step 6 deleted it). Every claim was checked by grep, `just -n` or a script over the committed files. Nothing
was built, run or benchmarked.

**Was the idea carried out? Yes.** Every step's gate passes. The project graph matches the support-projects
shape and its three grep lines come back empty. Every number in `lib/results/README.md` and
`vipaq/results/README.md` recomputes from the detail files. Every benchmark case count matches the step 12
tables. What is wrong is below: one thing that stopped working, test coverage that got thinner, and a lot of
text that the moves made false.

## 1. The bench tooling gets reworked - the maintainer's call, 2026-09-22

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
  `.agents/docs/commands.md`, `.agents/docs/tooling/README.md`, `.agents/docs/lib/tests.md`,
  `tooling/bench.just` itself. Only `vipaq/bench/Binacle.ViPaq.Benchmarks/README.md` writes the working form,
  `just bench vipaq short`.
- **Every mistake is silent.** BDN exits 0 on an invalid job ("The provided base job "ffd" is invalid"), on a
  filter that matches nothing, and on a parse error. `Program.cs` in all five projects drops what `Run`
  returns. So a wrong word runs nothing and the recipe reports success.
- **The algorithm words select nothing on four of five binaries.** `ffd`, `bfd`, `wfd` are categories only in
  Algorithms; in Racing and Threshold the algorithm is a column, in ResultSelection and vipaq there is none.
  `just bench lib-racing bfd` runs nothing, exits 0.
- **`just bench` lists alphabetically**, so `lib-algorithms` (the alias, no cost in its comment) sits above
  `-full`, `-sample`, `-smoke`, and `default` shows as a recipe. `just measure` has the same fault.
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
- `default` lists with `--unsorted`, so the file order is the tier order. Same for `measure.just`.
- `Program.cs` exits 1 when `Run` returns nothing, so an invalid job or an empty filter fails the recipe.
- Lessons from the earlier script-to-recipe conversions: a script that only wraps a tool is absorbed into
  the recipe, not kept beside it; one module per job, and where two need the same few lines, copy them;
  module recipes need `set working-directory := '..'`.
- A misspelled curated id throws a `KeyNotFoundException` in `[GlobalSetup]`, in BDN's child process. BDN
  marks the case NA and goes on, so this is silent today too. The exit code above should catch it: fail
  when any report has a failed case, not only when there is no report.

## 2. Broken, or checking less than before

Fixed 2026-09-22, and the ViPaq unit tests pass (7,379): the Sonar data exclusion; gzip and the compressed
forced-width case in the real-pack round trip; a test that every pack family loads. `just measure check` was
removed - `just measure` is a local tool, not a guard. The harness no longer writes the results READMEs; the
story they should tell is written apart from it. No check was added for the report's own encoder or for the
lib benchmark picks - the maintainer's call: no tests of the test harness. A misspelled pick shows through
the exit code in section 1.

Nothing is open here.

## 3. Text the moves made false

The `.agents` docs, design records, READMEs and comments were fixed 2026-09-22, and root `results/` was
deleted with its READMEs. What is left is in the code.

### Left in the code

- [ ] The largest real pack is 365 items in `BischoffCuratedProvider.cs` (the FFD pack of thpack1_65) and 371
      in `SyntheticDataProvider.cs` (the BFD pack). Pick one pack; name the column for what it is.
- [ ] `ViPaqHeader.Read`, `IsCompressed` and `UncompressedByteCount` have no callers.
      `grep -rn "IsCompressed\|UncompressedByteCount\|ViPaqHeader.Read" vipaq --include=*.cs` hits only the definitions today.
- [ ] `BischoffCuratedProblemsProvider.GetBenchmarkScenarios` - every sibling is `GetScenarioNames`.

### Found on the way, outside this plan

- [ ] `.agents/docs/api/tests.md` has about ten claims the code does not back (one-file folders, the v3
      ByPreset tests, which test asserts the special presets, `InitializeAsync`, `Kernel.UnitTests` folders,
      `NegativeRequest`'s signature). Its `verified:` was left at 2026-09-19 so it still reads as unchecked.
      Needs its own pass against `api/test/**`.

## 4. Where the build drifted from the shape

- [ ] **The loader above the shared reader is still two copies.** `ScenarioCollectionsProvider.cs` and
      `MultipleScenarioCollectionsProvider.cs` in `shared/data/Binacle.Data` and in
      `lib/data/Binacle.Lib.Data/ResultSelection` have the same code over different types: each set's own
      `Scenario`, `CollectionScenario` and `ScenarioReader`. One copy needs a generic loader in `Binacle.Data`
      that takes the prefix and a read function. Merge, or record why two.
      `find shared/data lib/data -name ScenarioCollectionsProvider.cs -not -path "*/obj/*" | wc -l` is 1.
- [ ] **The set-in-the-namespace rule was not applied to the curated picks.** The shape wanted
      `Curated.BischoffSuite`; the tree has `BischoffCuratedProvider`, `CustomProblemsCuratedProvider`,
      `BischoffCuratedProblemsProvider`. D9 now says only "the curated picks are in `ViPaq.Testing`", with no
      reason for dropping the rule. The maintainer wants consistent names, 2026-09-22: rename, once the names
      are agreed. **By eye.**
## 5. Small

- [ ] Step 12's namespace check prints `bench` for every project (the awk takes the folder, not the
      project). It still shows a sixth namespace, but never says which project holds it.
- [ ] `version-parity.md` prints an empty table for FFD and WFD. A line saying "no difference" reads better.
- [ ] `packing-efficiency.md` says Margin is "top fill minus the next one"; on a two-way tie it is the gap to
      the third (thpack1_4: 0.19). The code comment in `Wins.cs` says it right; copy that wording.
- [ ] `MarkdownFileWriter` only overwrites. A dropped reporter leaves its old file behind.
- [ ] `vipaq/results/encoded-size.md` is 1 MB, about half of it column padding; one longer scenario name
      rewrites all 4,644 rows. Whether GitHub renders it is not checked.
- [ ] `lib-racing-smoke` says about 2 minutes; 40 cases at `short` is about 3.
- [ ] Smoke's `most item types` pick (thpack7_56, 20 types) ties with all 100 thpack7 problems. The label
      is true; the "why" should say it stands for the set.
- [ ] The smoke classes have one baseline, `FFD_v1`, so every row's Ratio is against FFD. `CompressionCost`
      has one baseline, `Encode_NoOp`, so the `Decode_*` rows get a Ratio against an encode; it also has no
      `[BenchmarkOrder]`. Fix: each algorithm or direction carries its own baseline.
      **By eye** in a smoke report: a Ratio of 1.00 on `WFD_v1` and `BFD_v1` as well as `FFD_v1`.
- [ ] `lib/bench/Binacle.Lib.Benchmarks.ResultSelection` references `Binacle.Lib.Testing` and uses nothing from
      it. Removing it makes two lines false: `lib/test/Binacle.Lib.Testing/README.md` ("every project under
      `lib/bench/` references it") and `.agents/docs/lib/dependencies.md`.
      `grep -c Binacle.Lib.Testing lib/bench/Binacle.Lib.Benchmarks.ResultSelection/*.csproj` is 0.
- [ ] `Properties/launchSettings.json` - four copies under `lib/bench/*` and one under
      `lib/measure/Binacle.Lib.PackingEfficiency`, all old profiles. One `git rm` line, the maintainer's.
      `ls lib/bench/*/Properties lib/measure/*/Properties 2>/dev/null | wc -l` is 0.
- [ ] `Binacle.Lib`'s grant to `Binacle.Lib.PackingEfficiency` may be unused; only a build says.
- [ ] `lib/bench/README.md`, `vipaq/bench/README.md` and a comment in `tooling/bench.just` link to
      `../results/benchmarks`, which step 14 creates. Dead until then.
- [ ] `shared/test/Binacle.Reporting` and `shared/test/Binacle.Benchmarking` have no README; every sibling
      support project has one. The shared README covers both, which the rule allows. Judgement.
- [ ] `just agents all` - `.agents/plans/_index.md` has no row for this file, and `.agents/docs/_index.md`
      still describes lib/tests with the old wording. The maintainer's.

## Done when

- [ ] Every box above is ticked or its line has moved into the plan it belongs to, and this file is deleted.
      **By eye.**
