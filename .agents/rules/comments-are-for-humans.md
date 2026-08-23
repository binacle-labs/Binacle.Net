---
description: A comment carries the one thing that is not obvious from the code. Short. The reasoning goes in design/, never in both.
load: on-trigger
when: writing or editing a code comment
paths:
  - "**/*.cs"
  - "**/*.ts"
  - "**/*.js"
  - "**/*.csproj"
  - "**/*.props"
  - "**/*.just"
  - "**/*.yml"
---

# Comments are for humans, and they are thin

**A comment carries the one thing that is not obvious from the code.** That the path must be absolute. That
there is no `--` before the runner options. That the catch is empty on purpose. **Short. One line where one
line does it.**

**The test is the line below it.** If a person reading that line would already know it, cut the comment. If
they would not, that is the comment - and only that. "Why" is the wrong word for this test, because why
invites the reasoning; the reasoning is not the comment.

**The reasoning goes in `design/`.** That layer exists for exactly this - why it was built this way, and what
proved it. Background, the options that were rejected, task history, "keep this in step with X", and anything
that reads like a briefing goes there, not above the line.

**Never both.** A fact written in a comment and in a doc will disagree within a release. If the comment and
the design record would say the same thing, the comment is the copy that goes.

**Write them thin.** Cut the connective grammar first: "This is required because it throws due to X and Y"
is "without this it throws". Cut the restatement of the line below it. Cut the essay - if the reasoning is
worth keeping it goes in `design/`.

A table of widths, a byte layout, a measured number - those stay. A reader cannot recover them.

**A surviving agent comment is not damage.** When a review pass strips agent-written comments, the test is the
one above, not who typed it. Several were kept on purpose because they were better than the line they replaced -
the unchecked-multiply overflow note on the packing algorithms, the empty-catch explanation in
`ConnectionString.cs`, the curated-scenario table in `BischoffCuratedProblemsProvider.cs`.
