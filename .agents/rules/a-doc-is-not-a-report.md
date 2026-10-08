---
description: A doc states what is true, not what was done to find it out. No session narrative in a file a human reads.
load: on-trigger
when: writing or editing any doc, README, results file or other public markdown
paths:
  - "**/README.md"
  - "lib/results/**"
  - "vipaq/results/**"
  - ".agents/docs/**"
  - ".agents/design/**"
---

# A doc is not a report

**State what is true. Never what you did to find it out.** A reader wants the fact, not the work behind it.

Wrong, and it reads as a session log:

```markdown
None. Swept both layouts, both codecs and all three algorithms.
```

Right, and it is the same fact:

```markdown
None, in either layout, either codec, or any of the three algorithms.
```

**Cut these words:** swept, checked, re-checked, verified, confirmed, measured just now, re-ran, we found, I
looked at, as of this session, recomputed. Each one tells the reader about the author instead of the subject.

**Cut the first person.** No "we", no "I". The file has no narrator.

## Where it does not apply

- **Provenance of a measured number is a fact about the number**, not a report. The machine, the runtime, the
  job and the date a report was copied all stay - a time means nothing without them.
- **Plans.** A plan is working state and says who suggested what and when, by its own rules.

**Why:** a results file and a README are read by someone deciding something, often months later. The author's
process is not part of the decision, and a file written as a report of a session ages into noise.
