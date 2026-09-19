---
description: Step 7 - both PerformanceTests projects move to <slice>/measure/ under their new names, point their writer at <slice>/results/, and get measure.just; the memory and D3 that said otherwise go the same day
state: ready
waits-on: "step 6's gate"
horizon: next-release
paths: ["lib/**", "vipaq/**", "tooling/**", ".agents/**", "Binacle.Net.slnx", "Directory.Build.props", "justfile"]
---

# Step 7 - the measure projects and their recipe

Shape: [results.md](results.md), "Two kinds, two rules" and "Recipes". Protocol: the orchestrator.

## The step

- `git mv lib/test/Binacle.Lib.PerformanceTests lib/measure/Binacle.Lib.PackingEfficiency` and
  `git mv vipaq/test/Binacle.ViPaq.PerformanceTests vipaq/measure/Binacle.ViPaq.EncodedSize`; rename the
  csproj files and namespaces. `Binacle.Lib`'s and `Binacle.ViPaq`'s friend grants follow the names.
- Each writer points at `<slice>/results/` through `RepositoryRoot.Bind().Find(...)`, creating the folder
  when it is missing, not at the `PerformanceTests.Artifacts` scratch. What they write does not change
  yet - that is steps 8 and 10 - so **do not run them in this step**; a run now would commit the old-named
  reports. Root `results/` still exists; it goes in step 11.
- `tooling/measure.just`: `set working-directory := '..'`, `set no-exit-message := true`, `default` lists,
  `lib`, `vipaq`, `all`, `check` (run all, then fail if `git status` under either `results/` is dirty - the
  ten lines `regen check` has). The root `justfile` gets its `mod` line. `performance.lib.sh` and
  `performance.vipaq.sh` are absorbed and deleted - each was `dotnet run -c Release` with a path.
  `regen.just`'s header says every tool there rewrites committed files; it is no longer alone, and says so.
- `Directory.Build.props`: `/measure/` and `/bench/` join the Sonar path rule, both now, so step 12 does not
  touch it.
- The same day, because the writers now say the opposite: `.agents/memory/results-curated.md` is deleted;
  vipaq D3's "writes to scratch, copied by hand" paragraph is rewritten - the harness writes the tracked
  file, a win is a diff; `docs/commands.md`'s "gitignored scratch folder" sentence and its two performance
  script lines; the tooling doc's two rows.
- `Binacle.Net.slnx`: `/lib/measure/` and `/vipaq/measure/` folders. Docs: the lib tests doc, both slice
  READMEs; READMEs in both new folders. The gate is the list.

## Open before starting

- Settled 2026-09-20, both slices use it. Every written file opens with its title as the `#` line, then one
  sentence, for lib: Written by `just measure lib` from `Binacle.Lib.PackingEfficiency` over the 700 Bischoff
  suite scenarios (thpack1..7). Do not edit. - tool, count, data set; never a date or a commit. The 700 are the
  Bischoff suite alone; custom problems and demo samples are not in the run (corrected 2026-09-20, step 8).
  ViPaq's names its own recipe, project, pack count and families.

## Done when

- [x] `test -d lib/measure/Binacle.Lib.PackingEfficiency && test -d vipaq/measure/Binacle.ViPaq.EncodedSize`
- [x] `ls lib/test vipaq/test` lists no `PerformanceTests`.
- [x] `grep -rn "Find(" lib/measure/*/Program.cs vipaq/measure/*/Program.cs` shows the results path;
      `grep -rn "PerformanceTests.Artifacts" lib vipaq --include=*.cs` is empty.
- [x] `grep -n "Contains('/measure/')" Directory.Build.props` and `Contains('/bench/')` both hit.
- [x] `test -f tooling/measure.just && test ! -f tooling/performance.lib.sh && test ! -f tooling/performance.vipaq.sh`;
      `just measure` lists four recipes.
- [x] `test ! -f .agents/memory/results-curated.md`; `grep -n "scratch" .agents/design/vipaq/decisions.md .agents/docs/commands.md`
      names no `PerformanceTests.Artifacts`.
- [x] `dotnet build` on both projects succeeds. No run.
