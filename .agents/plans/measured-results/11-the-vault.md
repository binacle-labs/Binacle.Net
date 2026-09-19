---
description: Step 11 - root results/ goes, both slices are regrown with just measure, and every doc, record and config line that named the vault is rewritten
state: ready
waits-on: "step 10's gate"
horizon: next-release
paths: ["results/**", "tooling/ci/**", ".agents/**", ".gitignore"]
---

# Step 11 - the vault goes

Shape: [results.md](results.md), "Two kinds, two rules" and "What moves with it". Protocol: the orchestrator.

## The step

- `git rm -r results/` - the maintainer's line. The old keepers under `results/lib/benchmarks/` are needed
  in step 13; the session takes them from git history then, so nothing is kept aside.
- Both `<slice>/results/` regrown with `just measure all` - the maintainer's run - and `just measure check`
  passes on the result.
- The docs and records that name the vault: `design/vipaq/findings.md` (its `check:` line and the crossover
  reference), vipaq D5 and `docs/vipaq/architecture.md` (the compression report paths and the `check:`
  line), `design/lib/findings.md` (the vault path), `docs/README.md` (the `results/` row),
  `docs/build-topology.md` ("`results/` is deliberately not in the solution"), `.agents/README.md`
  ("`results/` by real path"), `docs/commands.md` (the benchmark script lines), the tooling doc. One line
  stays: the repo-wide design record's "the 2024 records under `results/lib/benchmarks/`" is history.
  `.agents/memory/_index.md` is generated; `just agents all` is the maintainer's.
- `tooling/ci/sonar-analysis.xml`: `results/**` in `sonar.exclusions` becomes `lib/results/**,vipaq/results/**`,
  and the comment on line 22 that calls `results/` build output. `.gitignore` loses
  `PerformanceTests.Artifacts` and `PerformanceTestsArtifacts`.

## Open before starting

- Whether `just measure check` belongs in a workflow. The shape says measurements do not gate. Leave it
  uncalled unless the maintainer says.

## Done when

- [ ] `test ! -d results && test -d lib/results && test -d vipaq/results`
- [ ] `just measure check` passes on a clean tree.
- [ ] `grep -rn "results/" --include=*.md .agents | grep -v "lib/results\|vipaq/results\|/plans/\|_index.md\|what was true then"`
      is empty.
- [ ] `grep -n "results" tooling/ci/sonar-analysis.xml` shows the two slice paths and no `results/**`;
      `grep -n "PerformanceTests" .gitignore` is empty.
