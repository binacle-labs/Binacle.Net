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
