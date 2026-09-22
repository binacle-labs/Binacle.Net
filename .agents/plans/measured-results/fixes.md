---
description: Open fixes in the measure and bench tooling that wait on the maintainer's answer
state: proposed
waits-on: "the maintainer's answer on each item. horizon was set by an agent, strike it"
horizon: undecided
paths: ["shared/test/Binacle.Reporting/**", "lib/results/**", "vipaq/results/**"]
---

# Fixes waiting on an answer

- [ ] **The report writer leaves a dropped report's file behind.** `MarkdownFileWriter` deletes and rewrites
      only the files the code produces today. Remove a reporter and its old `.md` stays in `<slice>/results/`,
      never updated, and git shows no change. Making the writer delete every other `.md` would also delete
      files put there by hand.
      **Likely answer: do nothing.** Whatever reads the results reads files it knows by name, so a stale
      file nobody names is not read. Whoever removes a reporter deletes its file in the same change.
- [ ] **`vipaq/results/encoded-size.md` is too big to read.** 1 MB, 4,644 rows: 2,322 packs, once per layout.
      60% of it is the spaces that line up the columns, and the longest scenario name sets the width, so one
      longer name rewrites every row. GitHub does not render it (checked 2026-09-23 on the pushed branch:
      `richTextTruncated`, no table).
      **Idea: split it by set and by layout.** Measured 2026-09-23: Bischoff is 2,100 packs, about 450 KB per
      layout today; custom problems and demo samples together are 222, about 50 KB per layout. So Bischoff
      would still be too big and needs splitting again, one file per thpack (300 rows, about 65 KB).
