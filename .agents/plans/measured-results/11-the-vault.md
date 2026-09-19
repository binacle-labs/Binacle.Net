---
description: Step 11 - every doc, record and config line that named the vault is rewritten; results/ itself stays on disk until the new benchmarks have run
state: ready
waits-on: "step 10's gate"
horizon: next-release
paths: ["results/**", "tooling/ci/**", ".agents/**", ".gitignore"]
---

# Step 11 - the vault stops being current

Shape: [results.md](results.md), "Two kinds, two rules" and "What moves with it". Protocol: the orchestrator.

## The step

- Nothing is deleted. `results/` stays on disk, whole, until step 14 has run the new benchmarks - the
  maintainer's call, 2026-09-20: the records are repurposed, not trusted to history. Its READMEs say so:
  old records, nothing writes here, filed or dropped by steps 13 and 14.
- Both `<slice>/results/` already hold what `just measure all` writes (steps 8 and 10); `just measure check`
  passes on the tree.
- The docs and records that name the vault: `design/vipaq/findings.md` (its `check:` line and the crossover
  reference), vipaq D5 and `docs/vipaq/architecture.md` (the compression report paths and the `check:`
  line), `design/lib/findings.md` (the vault path), `docs/README.md` (the `results/` row),
  `docs/build-topology.md` ("`results/` is deliberately not in the solution"), `.agents/README.md`
  ("`results/` by real path"), `docs/commands.md` (the benchmark script lines), the tooling doc. One line
  stays: the repo-wide design record's "the 2024 records under `results/lib/benchmarks/`" is history.
  `.agents/memory/_index.md` is generated; `just agents all` is the maintainer's. Other plans name
  `results/lib/efficiency/` and the like - `grep -rn "results/lib\|results/vipaq" .agents/plans` finds them;
  the new path is `lib/results/packing-efficiency.md` and so on.
- `tooling/ci/sonar-analysis.xml`: `lib/results/**,vipaq/results/**` join `results/**` in `sonar.exclusions`
  (`results/**` goes with the folder in step 14), and the comment that called `results/` build output says
  measured results. `.gitignore` already lost its `PerformanceTests` lines in step 7.

## Open before starting

Settled 2026-09-20: `just measure check` is not called from any workflow. Measurements do not gate; the
check is for the maintainer before a commit that touches a packer or the encoder. A separate plan wants a
workflow to call `just regen check` and says regen covers "data generated into the repository, and nothing
else"; `measure` is not regen, and that plan's sentence stands.

## Done when

- [x] `test -d results && test -d lib/results && test -d vipaq/results`
- [x] `just measure check` passes on a clean tree.
- [x] `grep -rn "results/vipaq\|results/lib/efficiency\|curated vault\|hand-curated" --include=*.md .agents | grep -v "_index.md\|measured-results"`
      is empty - plans included. Lines naming `results/lib/benchmarks` and the `docs/README.md` row for the
      old records stay until step 14.
- [x] `grep -n "results" tooling/ci/sonar-analysis.xml` shows the two slice paths beside `results/**`;
      `grep -n "PerformanceTests" .gitignore` is empty.
