---
description: Session 8 - shape the summary of lib/results/README.md and vipaq/results/README.md with the maintainer, then write it from the filled results files
state: blocked
waits-on: "sessions 6 and 7 - every results file filled"
horizon: undecided
paths: ["lib/results/README.md", "vipaq/results/README.md"]
---

# 8 - The results READMEs

## Where it stands

Both READMEs are an index and a line `<!-- Summary: shape not decided yet. -->`. The maintainer has not decided
the summary's shape. Shape it with him first, one piece per turn, shown with made-up numbers; then write it.

## What is known so far

- **The README computes nothing.** Every number in it names the results file it came from.
- **Facts from two or more files go here**, and only here, since a results file reads one kind of raw result.
- **No other slice's numbers.** A comparison across slices has no home yet; it is an idea.
- The wording is revised later, by the maintainer.

Combinations seen so far, lib:

- fill bought against time paid, per option - `packing-efficiency-stats.md`, `algorithm-performance.md`,
  `parallel-racing.md`
- the pick is not worth optimizing: the slowest pick against the fastest pack - `result-selection.md`,
  `algorithm-performance.md`
- what a cost function can use - `packing-time-by-size.md`, `scaling.md`, `parallel-racing.md`, `parallel-bins.md`
- whether the direction is sound - the two version files, `packing-efficiency-stats.md`

Combinations seen so far, ViPaq:

- is the format worth having - `format-size.md` and the four codec files
- should the default be columnar - the row against the columnar files, `encode-cost.md`
- compress, and with what - the codec files, `encode-cost.md`
- is the speed acceptable - `encode-cost.md`, `decode-cost.md`

## Done when

- [ ] Both READMEs carry the summary in the shape the maintainer gave, and every number in it names its file.
      `! grep -l "shape not decided" lib/results/README.md vipaq/results/README.md`, then **by eye**.
