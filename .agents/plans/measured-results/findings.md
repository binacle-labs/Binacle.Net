---
description: Findings from the 2026-09-22 reviews of steps 1 to 12 as landed - the bench tooling the maintainer wants reworked, what broke or lost coverage, what text is false, where the build drifted from the shape. A finding leaves here when it is fixed.
state: ready
waits-on: "nothing - the bench tooling shape was picked and built 2026-09-22; the old files wait on the maintainer"
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

## 1. The bench tooling - settled and built 2026-09-22

The maintainer's shape: tiers are classes, the job is the switch.

- **Smoke** - minutes, takes no word. **Sample** - up to about an hour, the default job, `quick` for `short`
  where it runs long. **Full** - hours, `[confirm]` first, `short` by default, `precise` for the default job.
  A project under five minutes has one recipe and no tiers (`lib-result-selection`).
- The tier is the first word of the class name, picked with `--filter '*.<Tier>_*'`. No categories, no
  narrowing words, no free BDN flags.
- Every recipe starts with `lib-` or `vipaq-`. One private `<name>-run` recipe per project holds its
  `dotnet run` line. `[arg(..., pattern=...)]` checks the word; a recipe that takes one sets the job in a
  short bash body on its own lines, then calls `-run`. `default` lists with `--unsorted`.
- `BenchmarkProgram.Run` in `Binacle.Benchmarking` is every `Main`; it exits 1 when nothing ran or a case failed.
- New classes: Racing `Smoke_Packing`, Threshold `Sample_Algorithms_Packing`, ViPaq `Smoke_Encode` and
  `Smoke_Decode`. Renamed to carry the tier: Racing `Sample_Packing_v1`/`_v2`, Threshold `Full_*`, ViPaq
  `Sample_Encode`, `Sample_Decode`, `Sample_CompressionCost` over `EncodeBase` and `DecodeBase`.

Left:

- [ ] The maintainer deletes the old files: `tooling/bench.run.sh`, Racing `Packing_v1.cs` and `Packing_v2.cs`
      (Racing does not build until they go), Threshold `Algorithms_Packing_v1.cs`, `_v2.cs`,
      `Bins_Packing_v1.cs`, `_v2.cs`, ViPaq `Encode.cs`, `Decode.cs`, `CompressionCost.cs`.
- [ ] The maintainer runs each smoke recipe once; a wrong word fails; `just bench lib-algorithms-full` asks.

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

Nothing.

### Found on the way, outside this plan

- [ ] `.agents/docs/api/tests.md` has about ten claims the code does not back (one-file folders, the v3
      ByPreset tests, which test asserts the special presets, `InitializeAsync`, `Kernel.UnitTests` folders,
      `NegativeRequest`'s signature). Its `verified:` was left at 2026-09-19 so it still reads as unchecked.
      Needs its own pass against `api/test/**`.

## 4. Where the build drifted from the shape

Nothing.

## 5. Small

- [ ] Step 12's namespace check prints `bench` for every project (the awk takes the folder, not the
      project). It still shows a sixth namespace, but never says which project holds it.
- [ ] `version-parity.md` prints an empty table for FFD and WFD. Fixed in the code 2026-09-23: a section's
      table is optional, and parity leaves it out and says "All 700 scenarios pack to the same fill". Ticks
      when the maintainer reruns `just measure lib`.
- [ ] `packing-efficiency.md` says Margin is "top fill minus the next one"; on a two-way tie it is the gap to
      the third (thpack1_4: 0.19). The code comment in `Wins.cs` says it right; copy that wording.
      Fixed in the code 2026-09-23; ticks when the maintainer reruns `just measure lib`.
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
      Lib smoke done 2026-09-23: one class per algorithm, `Smoke_<Alg>_<Op>`, like Sample and Full. The
      maintainer deletes `Smoke_Packing.cs` and `Smoke_Fitting.cs` under `lib/bench/Binacle.Lib.Benchmarks.Algorithms`.
      ViPaq done 2026-09-23: `Sample_CompressionCost_Encode` and `_Decode`, each with its own NoOp and row
      order. The maintainer deletes `vipaq/bench/Binacle.ViPaq.Benchmarks/Sample_CompressionCost.cs`; until
      then `vipaq-sample` runs its 12 cases twice.
- [ ] `lib/bench/Binacle.Lib.Benchmarks.ResultSelection` references `Binacle.Lib.Testing` and uses nothing from
      it. Removing it makes two lines false: `lib/test/Binacle.Lib.Testing/README.md` ("every project under
      `lib/bench/` references it") and `.agents/docs/lib/dependencies.md`.
      `grep -c Binacle.Lib.Testing lib/bench/Binacle.Lib.Benchmarks.ResultSelection/*.csproj` is 0.
      Done 2026-09-23; builds without it.
- [ ] `Properties/launchSettings.json` - four copies under `lib/bench/*` and one under
      `lib/measure/Binacle.Lib.PackingEfficiency`, all old profiles. One `git rm` line, the maintainer's.
      `ls lib/bench/*/Properties lib/measure/*/Properties 2>/dev/null | wc -l` is 0.
- [ ] `Binacle.Lib`'s grant to `Binacle.Lib.PackingEfficiency` may be unused; only a build says.
- [ ] `lib/bench/README.md`, `vipaq/bench/README.md` and a comment in `tooling/bench.just` link to
      `../results/benchmarks`, which step 14 creates. Dead until then.
- [ ] `shared/test/Binacle.Reporting` and `shared/test/Binacle.Benchmarking` have no README; every sibling
      support project has one. The shared README covers both, which the rule allows. Judgement.

## Done when

- [ ] Every box above is ticked or its line has moved into the plan it belongs to, and this file is deleted.
      **By eye.**
