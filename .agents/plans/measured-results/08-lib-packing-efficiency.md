---
description: Step 8 - one run, many views - the lib runner packs every scenario once, a builder writes the four files, the README holds the summaries
state: ready
waits-on: "step 7's gate"
horizon: next-release
paths: ["lib/measure/**", "lib/results/**", "shared/test/Binacle.Reporting/**"]
---

# Step 8 - `Binacle.Lib.PackingEfficiency`

Shape: [results.md](results.md), "Deterministic projects" and the lib file list. Protocol: the orchestrator.

## The step

- Today four `ITest` classes each walk 700 scenarios and pack again - 12,600 packings for four views of
  4,200 numbers. Becomes: one runner packs every scenario once per shipped version, holds the results, and
  one builder writes the files from that set.
- The files, all under `lib/results/`, all harness-written, opening with the header step 7 settled:
  `README.md` (the summaries), `packing-efficiency.md` (700 rows: Scenario, Items, Types, Ceiling %, FFD,
  WFD, BFD), `algorithm-wins.md` (best per scenario and margin; replaces BaselineComparison, which hard-coded
  BFD), `version-parity.md` (only rows where v1 and v2 differ; replaces RegressionTests). `PackingTime.md`
  and `PackingEfficiencyComparison.md` are not written.
- The README: per version min / mean / median / max / stddev over 700; the same per set, rows labelled
  `BR1 (3 types)` .. `BR7 (20 types)`; the user's fill - mean of best-of-FFD-and-BFD and best-of-all-three;
  best-or-tied count per algorithm; v1/v2 agreement count; links to the three files.
- Unused Serilog packages go. `Binacle.Reporting`'s `ITest`/`TestRunner`/`TestResult` get names that say
  runner and builder, and `MarkdownFileWriter` stays.
- The first run is the maintainer's - 4,200 packings on a laptop. The session hands the command.

## Open before starting

- The runner and builder names, and whether the builder is one class per file or one class with four
  writers. The shape says the implementer names them.
- Column order in `packing-efficiency.md`, and whether `algorithm-wins.md` stays a file or becomes two
  columns there - the shape leaves it a file until a session argues otherwise.
- How `Binacle.Reporting` changes: the vipaq runner in step 10 reuses it, so the reshape is decided here
  for both.

## Done when

- [ ] `ls lib/results` lists exactly `README.md packing-efficiency.md algorithm-wins.md version-parity.md`
      (plus `benchmarks/` once step 13 lands).
- [ ] `head -30 lib/results/README.md` is summary tables, not rows; `grep -c "thpack" lib/results/packing-efficiency.md`
      is 700.
- [ ] `grep -n "BR1 (3 types)" lib/results/README.md` hits.
- [ ] `grep -rn "Serilog" lib/measure/*/*.csproj` is empty.
- [ ] `grep -n "12,600\|foreach.*scenarios" lib/measure/Binacle.Lib.PackingEfficiency -r --include=*.cs`
      **by eye** shows one pass over the scenarios, not four.
- [ ] A second run changes nothing: run, then `git status --short lib/results` is empty.
