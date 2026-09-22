---
description: Step 17 - what the measure tools write - encoded-size.md is too big to read and gets split; a dropped report's old file stays behind, likely left as is
state: proposed
waits-on: "the maintainer's answer on both items. horizon was set by an agent, strike it"
horizon: undecided
paths: ["shared/test/Binacle.Reporting/**", "vipaq/measure/Binacle.ViPaq.EncodedSize/**", "lib/results/**", "vipaq/results/**"]
---

# Step 17 - the measure output

Shape: the general design record, the decision on measured numbers. Protocol: the orchestrator.

## `vipaq/results/encoded-size.md` is too big to read

1 MB, 4,644 rows: 2,322 packs, once per layout. 60% of it is the spaces that line up the columns, and the
longest scenario name sets the width, so one longer name rewrites every row. GitHub does not render it
(checked 2026-09-23 on the pushed branch: `richTextTruncated`, no table).

**Idea: split it by set and by layout.** Measured 2026-09-23: Bischoff is 2,100 packs, about 450 KB per
layout today; custom problems and demo samples together are 222, about 50 KB per layout. So Bischoff would
still be too big and needs splitting again, one file per thpack (300 rows, about 65 KB).

## A dropped report leaves its old file behind

`MarkdownFileWriter` deletes and rewrites only the files the code produces today. Remove a reporter and its
old `.md` stays in `<slice>/results/`, never updated, and git shows no change. Making the writer delete every
other `.md` would also delete files put there by hand.

**Likely answer: do nothing.** Whatever reads the results reads files it knows by name, so a stale file
nobody names is not read. Whoever removes a reporter deletes its file in the same change.

## Done when

- [ ] No file under `vipaq/results/` is over 100 KB: `find vipaq/results -name '*.md' -size +100k` is empty.
- [ ] The dropped-report item is answered; if the answer is "do nothing", it is written where the reporter
      code is described.
