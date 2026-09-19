---
description: Step 8 - one run, many views - the lib runner packs every scenario once into a bag, one reporter per file writes the three files, the README holds the summaries
state: ready
waits-on: "step 7's gate"
horizon: next-release
paths: ["lib/measure/**", "lib/results/**", "shared/test/Binacle.Reporting/**", "vipaq/measure/**"]
---

# Step 8 - `Binacle.Lib.PackingEfficiency`

Shape: [results.md](results.md), "Deterministic projects" and the lib file list. Protocol: the orchestrator.

## The step

- Today four `ITest` classes each walk 700 scenarios and pack again - 12,600 packings for four views of
  4,200 numbers. Becomes: `PackingRunner` packs every scenario with every version once and fills
  `PackingBag`; one reporter per file reads the bag.
- The files, all under `lib/results/`, all harness-written, opening with the header step 7 settled:
  `README.md` (the summaries), `packing-efficiency.md` (700 rows: Scenario, Types, Items, Ceiling %, FFD,
  WFD, BFD, Best, Margin - the last two replace BaselineComparison, which hard-coded BFD), `version-parity.md`
  (only rows where v1 and v2 differ; replaces RegressionTests). `PackingTime.md` and
  `PackingEfficiencyComparison.md` are not written.
- The README: per algorithm min / mean / median / max / stddev over 700; the same per set, rows labelled
  `BR1 (3 types)` .. `BR7 (20 types)`; the user's fill - best-of-FFD-and-BFD and best-of-all-three;
  best-or-tied count per algorithm; v1/v2 agreement count; links to the two files.
- Serilog goes from the lib project. `Binacle.Reporting` becomes the runner-and-reporter loop: `IRunner`,
  `IReporter`, `ReportSection`, `Measure`, `Statistics`, `Format`; `MarkdownFileWriter` stays. The bag is
  typed per slice, so it lives in the slice project and DI hands it to the runner and the reporters. The
  ViPaq project takes the rename in the same step so the tree keeps building; its reshape is step 10.
- The first run is the maintainer's - 4,200 packings on a laptop. The session hands the command.

## Open before starting

Settled 2026-09-20: one runner, one bag, one reporter class per file; the wins are two columns in
`packing-efficiency.md`, not a file; `Binacle.Reporting` holds the shared loop and never the bag.

## Done when

- [x] `ls lib/results` lists exactly `README.md packing-efficiency.md version-parity.md`
      (plus `benchmarks/` once step 13 lands).
- [x] `head -30 lib/results/README.md` is summary tables, not rows; `grep -c "OrLibrary_thpack" lib/results/packing-efficiency.md`
      is 700.
- [x] `grep -n "BR1 (3 types)" lib/results/README.md` hits.
- [x] `grep -rn "Serilog" lib/measure/*/*.csproj` is empty.
- [x] `grep -rn "Execute(" lib/measure/Binacle.Lib.PackingEfficiency --include=*.cs` hits one file,
      `PackingRunner.cs` - one pass over the scenarios, not four.
- [ ] A second run changes nothing: run, then `git status --short lib/results` is empty.
