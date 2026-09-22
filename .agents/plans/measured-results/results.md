---
description: What is left of the measured-results shape - benchmark keepers dated per family with a README each, the old vault converted in, the scaling class and the JSON timing, then root results/ goes
state: ready
waits-on: "the bench tooling rework in the findings file beside this one"
horizon: next-release
paths:
  - "results/**"
  - "lib/bench/**"
  - "vipaq/bench/**"
  - "lib/results/benchmarks/**"
  - "vipaq/results/benchmarks/**"
---

# What is left of the measured-results shape

Built by 2026-09-22 and recorded in the general design record (the decision on measured numbers): the two
kinds of number and their rules, the measure projects writing `<slice>/results/`, `just measure`, one
benchmark project per question under `<slice>/bench/`, `just bench`. What is left is the timing side's
record: keepers, their READMEs, the old vault converted in, and root `results/` gone.

**This is the shape, not the letter.** A step that finds a detail unsettled settles it; a step that finds
the shape wrong says so with the evidence and stops.

## Keepers

Timing runs go to scratch. A run worth keeping is copied by hand into its family folder:

```
lib/results/benchmarks/
  README.md
  algorithms/<date>.md     the scaling class, smoke, sample, full; and the 20 old Multiple* reports
  result-selection/
  racing/<date>.md
  threshold/
vipaq/results/benchmarks/
  README.md
  encoding/
```

**Keep a run when the ruler or the code changed, not because it ran.** The ruler is in every BDN header, so
the file does not carry it.

**Each `benchmarks/README.md` has three parts:** the rule (Mean does not compare across files; Ratio and
Allocated do, and for Loop vs Parallel only on the same core count); the trace - one row per keeper: date,
ruler, family, key ratio with its RatioSD, allocated; what they say - one line per family, the current
answer with its number. Parts two and three are read out of the keeper files, by a small script if the
implementer writes one, by hand on day one.

Not decided - the maintainer is unsure, so the shape stands until a session argues otherwise: part three as
its own section, where the newest trace row per family, marked, might do the job.

## Two classes still to write

- **Time against item count** in `Binacle.Lib.Benchmarks.Algorithms`: six versions over the 11-step ladder
  in `SpecializedScalingProblemsProvider`, minutes. No family gives that curve today, and 20 of the 50 old
  raw reports measured it (`MultipleItems_*`, `MultipleBins_*`); they land beside it.
- **`Json`** as a row in `Binacle.ViPaq.Benchmarks` `Encode`, using `JsonEncoder` from `Binacle.ViPaq.Testing`.
  Encode only - the encoder has no decode.

## Converting the old vault

Every old raw report is for a deleted class (`AlgorithmVersion_*`, `MultipleItems_*`, `MultipleBins_*`); each
lands under the nearest current family, dated from its BDN header and git, with a line saying which class it
really measured. The `AlgorithmVersion_*` files carry `FFD_v3` rows for a version that no longer exists; the
line says that too. One judgement per file. The hand-written dated summaries carry charts on GitHub
user-attachments and a "what changed" line; the line goes into the trace table, the rest does not. Then the
trace table is read as a whole and the progress story checked.

Root `results/` stays on disk, whole, until the new keepers exist - the maintainer's call, 2026-09-20: the
records are repurposed, not trusted to history. Then `git rm -r results` is his line.

## Not in this plan

- A docs-site page copied from the READMEs, and a www number.
- Comparison against published results on the Bischoff instances: "we do 81% at X ns, others 87% at Y ns".
  The fill half holds anywhere; the time half only if both ran on one machine.

## Research

### 2026-09-22 - what the vault can still say

87 files. Every number below was read out of the files by script.

What holds:
- **v2 against v1, in one run, on both machines and both runtimes** (cubes, 10 to 192 items).
  - BFD v2: 0.43-0.50x the time, 0.35-0.39x the memory.
  - FFD v2: 0.62-0.81x the time, 0.60-0.62x the memory.
  - WFD v2: 0.17-0.18x the memory at 192 items. The time is 0.60-1.03x, so it is not a steady gain.
  - Files: `results_*/…AlgorithmVersion_Packing_*`.
- **FFD v2 memory held for a year:** 50.38-50.41 KB at 192 cubes, 2024-11 to 2025-11.
- **Fill has not moved since 2024-11:** FFD 73.41, WFD 69.21, BFD 81.26 then and in `lib/results/README.md`.
- **.NET 8 to .NET 9 in one run:** FFD v2 packing 0.85-0.92x the time (`2024-11-15.md`, `2024-11-22.md`).
- **Parallel bins in one run:** 0.63-0.76x of Loop at 8-38 bins on the 2-core machine, 0.20-0.49x on the
  20-core one; 1.3-2.6x slower at 2 bins with one item type. A prior for the bin threshold, not a finding.

What does not hold:
- "N% faster from 2024 to 2025" by Mean. The OS, the runtime, the BDN version and the scenario all changed.
  Unchanged code also drifts: FFD v2 at 192 cubes ranges 171-197 us on one fixed setup.

Worth nothing today:
- `results/lib/efficiency/` and `results/vipaq/`: their numbers match `lib/results/` and `vipaq/results/`
  exactly.
- The 40 charts, which sit on external attachments.
- The 2024 runs of deleted code.

The link to today's benchmarks: smoke's `full bin, one type` is the old 192-cube row, same bin. A first
`lib-algorithms-smoke` run on .NET 10 compares against the Nov 2025 ratios and allocations directly:
FFD 83.21/50.07 KB, BFD 130.96/49.67, WFD 285.57/51.24 (v1/v2).

Proposed, not decided:
- Keep the five Nov 2025 folders as keepers, one file each.
- Put the stories above into the lib findings record as one history table.
- Drop the rest.

This narrows "every old report becomes a keeper" above.

### 2026-09-19 - the vault, measured

- Every `results_net*` report names `AlgorithmVersion_*`, `MultipleItems_*` or `MultipleBins_*`. No such
  class exists. By their .NET versions the runs are from November 2025; the classes were renamed in the May
  2026 restructure; the folders were committed July 2026.
- `results/lib/efficiency/` holds `PackingEfficiencyComparison.md` and `PackingTime.md`, which no harness
  writes; `lib/results/` replaced it.
- `results/vipaq/compression/` has 700 Bischoff rows per layout named `OrLibrary_thpack1_1`; `vipaq/results/`
  replaced it.
- The dated summaries were assembled by hand - contents list, charts drawn on chartbenchmark.net, tables
  pasted in. An hour each; none after 2025-02.

### 2026-07-17 - algorithm racing was re-measured

The numbers are in the lib findings record, the sections on WFD's cost and on parallel algorithm racing. The
reports behind them were never copied in; `racing/2026-07-17.md` is the keeper to recover if the scratch
folder still exists on that machine.

### 2026-08-07 - lessons from the script-to-recipe conversions

For the bench tooling rework:

- **Absorbed, not wrapped, for a script that wraps a tool.** A bench run is `dotnet run -c Release` with a
  path. A program keeps its own file with a two-line recipe as its door, because shellcheck cannot read a
  `.just` body.
- **One module per job.** Where two need the same few lines, copy them.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself needs an
  absolute path.

## Done when

- [ ] `results/` at the root is gone.
      `test ! -d results`
- [ ] Every old keeper sits under a family folder with a date and a line naming its real class.
      **By eye.** Open each file under `lib/results/benchmarks/*/`; the first lines say class and ruler.
- [ ] Both `benchmarks/README.md` have the rule, the trace and what they say.
      **By eye.** Three headings, and every number in the trace appears in a keeper file.
- [ ] The scaling curve exists and has a keeper.
      `ls lib/results/benchmarks/algorithms/` has a file whose table has an item-count parameter column.
- [ ] The bin-threshold question has a finding.
      `grep -n "BinParallelizationThreshold\|Bins_Packing" .agents/design/lib/findings.md` is a section with numbers, not "no finding yet".
- [ ] No doc, README or memory names the root vault as current.
      `grep -rn "results/" --include=*.md .agents | grep -v "lib/results\|vipaq/results\|/plans/\|_index.md\|what was true then"` returns nothing.
