---
description: The release set holds what must ship with a version. An agent maintains it and never decides priority.
load: on-trigger
when: touching a release file, or deciding what to work on next
paths:
  - ".agents/release-v*.md"
  - ".agents/post-release-v*.md"
---

# The release set

## What it is

**A pointer surface.** It holds references to plans, with the order and the dependencies between them. It
never holds the work itself - that stays in the plan file.

| | Holds | Lives |
|---|---|---|
| `release-v<version>.md` | everything that **must ship with** that version | until the tag, then deleted |
| `post-release-v<version>.md` | everything that must happen **because** that version shipped | until its own items are done |

**Everything not tied to a release is a plan under `plans/` and nothing else.** The plan's own `state:` and
`waits-on:` say where it stands, and `plans/_index.md` lists them all.

**A release file may take a slice of a plan and leave the rest.** The plan file stays under `plans/` for what
is left; the release row names the slice it took. A plan file is deleted only when nothing is left in it.

**The slice is written once, in the release file.** Never also in the plan - see
[plans-do-not-schedule-themselves](plans-do-not-schedule-themselves.md). Two copies of one scheduling decision
disagree within a release, and neither announces which is stale.

## Who decides what

**An agent maintains the file. A human directs it.**

- **Recording a row you were told to record is the agent's job**, and so is keeping it current: when a slice
  lands, tick the row and drop the link in the same change. Otherwise the file rots into dead links, which is
  exactly what it exists to prevent.
- **Placement, readiness and priority are the maintainer's call.** Deciding something is "ready", where it
  sits in an order, how urgent it is, or whether it belongs in a release - **ask.** Do not judge it and write
  it in.
- **If you must state a readiness to make a row legible, say that you chose it** so it can be struck.

## Why

**When and whether something ships is not the agent's to decide.** It is judged with everything else in view.
An agent working one task sees one corner of it.
