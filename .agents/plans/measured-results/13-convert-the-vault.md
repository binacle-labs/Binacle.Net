---
description: Step 13 - every old keeper lands under its family folder with a date and a line naming its real class; both benchmarks/README.md are written
state: ready
waits-on: "step 12's gate"
horizon: next-release
paths: ["lib/results/benchmarks/**", "vipaq/results/benchmarks/**"]
---

# Step 13 - convert the old vault

Shape: [results.md](results.md), "Converting the old vault" and the README paragraph. Protocol: the orchestrator.

## The step

- The old files are still on disk under `results/lib/`; the folder goes at the end of step 14, once the new
  benchmarks have run. Every raw report
  there names a deleted class: `AlgorithmVersion_*`, `MultipleItems_*`, `MultipleBins_*`. Each lands under
  the nearest current family - the 20 `Multiple*` reports under `algorithms/` beside the new scaling class,
  the `AlgorithmVersion_*` ones too - dated from its BDN header and git, with a first line saying which class
  it really measured and, for the `FFD_v3` rows, that the version no longer exists. One judgement per file.
- The hand-written dated summaries (`2024-02-20.md` .. `2025-02-10.md`) carry charts on GitHub attachments
  and a "what changed" line. The line goes into the trace table; the rest does not come over.
- `lib/results/benchmarks/README.md` and `vipaq/results/benchmarks/README.md`, three parts each: the rule
  (Mean does not compare across files; Ratio and Allocated do, and for Loop vs Parallel only on the same
  core count); the trace - one row per keeper: date, ruler, family, key ratio with its RatioSD, allocated;
  what they say - one line per family with the current answer and its number. Parts two and three by hand
  on day one, by a small script if the session writes one.
- Then the trace is read as a whole and the progress story checked. What it says goes to the lib findings
  record if it is a finding.
- ViPaq's `benchmarks/README.md` has the rule and empty parts two and three: no ViPaq keeper exists.

## Open before starting

- Whether the five raw folders (`results_net9`, `_net10`, `_net9_windows`, `_net10_windows`,
  `_net9_10Installed`) each become a dated file, or only the three the ruler-change research used. Keep all
  five unless two say the same thing on the same ruler.
- Whether `racing/2026-07-17.md` can be recovered from the scratch folder on the maintainer's machine. Ask;
  if not, the trace row for racing cites the findings record and no keeper.

## Done when

- [ ] `test -f lib/results/benchmarks/README.md && test -f vipaq/results/benchmarks/README.md`
- [ ] `ls lib/results/benchmarks/` lists `algorithms` and `README.md`; `racing` too if the 2026-07-17 report
      was recovered. The other family folders are step 14's - git holds no empty folder.
- [ ] **By eye.** Open each file under `lib/results/benchmarks/*/`; the first lines say the class it really
      measured and the ruler.
- [ ] **By eye.** Each README has three headings, and every number in the trace appears in a keeper file.
- [ ] `grep -c "RatioSD\|Ratio SD" lib/results/benchmarks/README.md` is not 0.
