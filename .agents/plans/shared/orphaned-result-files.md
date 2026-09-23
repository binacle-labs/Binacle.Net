---
description: "Nothing tells you a results file is stale - a dropped reporter's markdown stays on disk and git shows no change"
state: idea
waits-on: "nothing. Horizon picked by an agent to make the file legible; strike it if wrong"
horizon: undecided
paths: ["shared/test/Binacle.Reporting/**", "lib/results/**", "vipaq/results/**"]
---

# Nothing spots an orphaned results file

`MarkdownFileWriter` rewrites the files the reporters produce today and touches nothing else. Remove a reporter,
or rename what its files are called, and the old markdown stays in `<slice>/results/` forever: never updated,
never diffed, and read by anyone who opens it as if it were current. It happened twice on 2026-09-23 when
`encoded-size.md` was split, and both times the fix was a `git rm` typed by hand. Deleting every other `.md`
in the folder is not the answer, because READMEs and kept benchmark reports live there too. Something that
names what the run wrote and flags what it did not - a line at the end of the run, or a check in CI - would
turn a silent stale file into a sentence.
