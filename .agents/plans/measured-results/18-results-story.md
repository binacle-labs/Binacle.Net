---
description: Step 18 - the results READMEs as a story for a human - "X is N% faster, cheaper or smaller than Y" - written from the raw files the measure and bench projects produce, in a session of its own; holds what the removed READMEs said and what the old vault could still prove
state: ready
waits-on: "a session of its own - the maintainer says when. horizon was set by an agent, strike it"
horizon: undecided
paths: ["lib/results/**", "vipaq/results/**"]
---

# Step 18 - the results story

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

`just measure` writes raw files: one row per scenario or pack, every number. That is evidence, not something
a person reads. What a person reads, and what we present as a win, is a sentence with a number:

- "BFD packs 8 points fuller than FFD on the 700 Bischoff problems."
- "A ViPaq token is 7% of the JSON that carries the same picture."
- "BFD v2 packs in under half the time of v1, with a third of the memory."

Each results folder's `README.md` should be that story, backed by the raw files beside it. The harness does
not write it; it would only ever print tables. The story is written from the raw files, perhaps by an AI
session reading them, and rewritten when the numbers move.

Until then, each results README says only what the files are. The harness READMEs were removed 2026-09-22.

## What the story compares

- **Fill.** Which algorithm packs fullest, by how much, and what racing buys the caller.
- **Size.** Four ways to store the same packing picture: a conventional JSON that holds just enough to
  redraw it, compact notation, protobuf, ViPaq. The JSON is a general guide, not the API's own response;
  its field names do not have to match the API.
- **Speed and memory**, from the kept runs in `<slice>/results/benchmarks/`. v2 against v1 per algorithm; parallel against one-at-a-time; how cost grows with
  item count and bin count. Ratios within one run only; see the next section.
- **What is missing.** Whether a claim we want to make has no measurement behind it yet. That gap is a
  finding for the measure or bench projects, not a sentence to write.

## Rules the story keeps

- A time claim is a ratio from one run on one machine. Mean never compares across machines or runtimes.
  Memory (Allocated) and fill compare anywhere.
- Every number in the README can be found in, or computed from, a raw file beside it.
- The story reads the raw files it knows by name, not whatever sits in the folder. A stale file left by a
  dropped reporter is never read (the maintainer, 2026-09-23).
- Name the version. The fill numbers are v2, the shipped version.
- Say how a number was made when it is not obvious: StdDev over all 700 (population); compact notation joins
  the bin and the items with `;` because it has no whole-pack form; JSON and compact are counted in text
  characters, the binary formats in base64 characters.

## What the removed READMEs said (2026-09-22)

**lib/results** - 700 Bischoff problems (thpack1..7), fill %, v2:

| Algorithm | Min | Mean | Median | Max |
|---|---|---|---|---|
| FFD | 56.18 | 73.41 | 73.47 | 87.90 |
| WFD | 49.15 | 69.21 | 69.05 | 87.90 |
| BFD | 62.08 | 81.26 | 81.48 | 90.66 |

- Best of FFD and BFD (what the API races): mean 81.30. Best of all three: 81.33. WFD adds 0.03 points.
- Best or tied: BFD 669 of 700, FFD 35, WFD 35.
- BFD leads in every set, BR1 (3 types) to BR7 (20 types); FFD and WFD lose more as item types grow
  (WFD 74.78 at BR1, 67.41 at BR7).
- v1 and v2 pack the same on 2,099 of 2,100 algorithm-problem pairs. The one: BFD on thpack7_45, 79.08 -> 79.73.

**vipaq/results** - 2,322 real packs, lengths in characters:

| Format | Mean | Max |
|---|---|---|
| JSON | 4,702 | 22,048 |
| Compact notation | 1,669 | 7,904 |
| Protobuf raw | 1,472 | 7,168 |
| Protobuf deflate | 529 | 1,656 |
| ViPaq deflate, row | 362 | 1,248 |
| ViPaq deflate, columnar | 304 | 704 |

- ViPaq deflate columnar as a share of: JSON 7% (1-22%), compact 21%, protobuf raw 24%, protobuf deflate 58%.
- ViPaq over protobuf under the same codec: raw 0.65, deflate row 0.68, deflate columnar 0.58, gzip 0.60-0.70.
- Deflate is smallest on 2,265 packs (row) and 2,275 (columnar); gzip never.
- Every real pack deflates to at most 1,248 base64 characters (row) or 704 (columnar).
- Compression pays from very small packs, but not always: raw still wins on some packs up to 6 items (row)
  and 2 (columnar). The removed README said "1 item", which was base64 rounding on one demo pack - do not
  repeat it.
- Missing from the removed README: gzip rows in the per-format table.

## History - what the old `results/` folder proved

Removed from the tree 2026-09-22. The files are in git before that date. Every number below was read out of
them by script.

**v2 against v1, packing, one bin 60x40x10 filled with 5x5x5 cubes.** The Ratio column is v2's time as a
share of v1's in the same run. Memory is identical on both machines at the same runtime.

| Alg | Items | i5 .NET 9 | i5 .NET 10 | i7 .NET 9 | i7 .NET 10 | Memory v1 -> v2 (.NET 10) |
|---|---|---|---|---|---|---|
| FFD | 10 | 0.68 | 0.70 | 0.65 | 0.68 | 6.46 -> 3.85 KB |
| FFD | 192 | 0.65 | 0.81 | 0.64 | 0.69 | 83.21 -> 50.07 KB |
| WFD | 10 | 0.67 | 0.72 | 0.60 | 0.67 | 11.15 -> 4.72 KB |
| WFD | 192 | 0.71 | 1.03 | 0.66 | 0.72 | 285.57 -> 51.24 KB |
| BFD | 10 | 0.46 | 0.47 | 0.50 | 0.50 | 8.40 -> 3.31 KB |
| BFD | 192 | 0.45 | 0.45 | 0.43 | 0.48 | 130.96 -> 49.67 KB |

i5 is an i5-4570 on Debian 12; i7 an i7-14700 on Windows 11; runs of November 2025. What holds:

- BFD v2 takes under half the time of v1, with a third of the memory.
- FFD v2 takes about a third less time, with 40% less memory.
- WFD v2 uses a sixth of the memory at 192 items. Its time gain is not steady: 1.03 in one run.

**Other things that hold:**
- FFD v2 memory at 192 cubes held for a year: 50.38-50.41 KB from 2024-11 to 2025-11.
- Fill has not moved since 2024-11: the same FFD 73.41, WFD 69.21, BFD 81.26 as today.
- .NET 8 to .NET 9 in one run (2024-11-15, 2024-11-22): FFD v2 packing 0.85-0.92x the time.
- Cost against size, in one run: FFD and BFD grow about in line with item count (13-17x the time for 19x the
  items); WFD much faster (42-65x). Time grows in line with bin count.
- Parallel bins against a loop, in one run: 0.63-0.76x at 8-38 bins on the 2-core i5, 0.20-0.49x on the
  20-core i7; 1.3-2.6x slower at 2 bins with one item type. A hint for the bin-threshold question, not an
  answer - the code has changed since.

**What did not hold:**
- "N% faster from 2024 to 2025". The OS, runtime, benchmark tool and scenario all changed. The same code
  also drifts between runs: FFD v2 at 192 cubes ranged 171-197 us on one fixed setup.

**A way to make the speed story current:** the smoke case `full bin, one type` is the old 192-cube row. The
2026-09-23 `lib-algorithms-smoke` run, kept in `lib/results/benchmarks/baseline/algorithms/`, gives the v2/v1
ratios and memory on .NET 10 to set beside the table above, and shows whether memory moved since November 2025.

**A better source landed 2026-09-25.** The `lib-algorithms-full` run - 1,400 cases per class, all 700 Bischoff
problems, v1 against v2, packing and fitting - is in the same folder as `Full_<alg>_<op>.md`. It carries the
v2/v1 story over the whole suite instead of one synthetic case, so the story's speed numbers come from there
and the smoke case is only the bridge back to the November table. It ran at the **short** job (3 iterations),
so a single problem's mean is rough; an average over 700 is not. Beside it, `baseline/threshold/Full_*` holds
the parallel numbers, already read out as F4 and F2a in the lib findings record - the story does not need to
re-derive them.

## Not in this plan

- A docs-site page copied from the READMEs, and a number on www.
- Comparison against published results on the Bischoff problems: "we fill 81% at X ns, others 87% at Y ns".
  The fill half holds anywhere; the time half only if both ran on one machine.

## Done when

- [ ] `lib/results/README.md` and `vipaq/results/README.md` each open with the story: sentences with numbers,
      each naming what it compares.
      **By eye.** Every number in the story is in, or computed from, a raw file in the same folder.
- [ ] The history above is either used in the story or dropped with a reason.
      **By eye.**
