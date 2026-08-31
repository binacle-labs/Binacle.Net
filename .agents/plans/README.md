# Plans

Work not yet done — designs, migrations, TODOs, deferred decisions, and the rough maybes nobody has committed
to. **A maybe belongs here.** There is no bar to clear and no separate layer for the unvetted ones; how
settled it is rides in `state:` below, not in where the file sits.

Group plans by slice, mirroring the repo — a plan that clearly belongs to one area lives in that slice
folder (e.g. `plans/lib/`, `plans/shared/`); anything that maps to no slice stays at the root.

## Every plan declares where it stands

Two keys in the front matter, on top of the `description:` and `paths:` every file carries:

```yaml
state: idea | proposed | ready | blocked | deferred
waits-on: what it is waiting for, in plain words
```

| State | Means |
|---|---|
| `idea` | Rough, and nobody is waiting. It can sit for a year without that being wrong. |
| `proposed` | An argument addressed to the maintainer, waiting on a yes or a no. |
| `ready` | Nothing blocks it. It can start today. |
| `blocked` | `waits-on:` names what. |
| `deferred` | Deliberately not now. The file says what revives it. |

**There is no `doing` state.** The working tree says that.

**A state is not a priority and not an order** — those are the maintainer's, and an agent never writes one in.
If you have to pick a state to make a file legible, say so in `waits-on:` so it can be struck.

## Rules

- **One item per plan file.** A session should be able to open one file, do the whole thing, and delete it —
  without pulling in three unrelated topics. Something that needs a decision, research, or more than one sitting
  gets its own file. A single mechanical act with a known answer does not: just do it, or make it a checkbox on
  the release file if it gates a release.
- **One master plan per topic**, holding what is done and what is left. When a review turns up issues, put them
  in **one findings file** beside it; a finding lives there until it is fixed, then moves into the master and is
  deleted from findings. Delete findings when it's empty. Don't let a topic sprawl into four overlapping plans,
  and don't keep a session log inside a plan — history belongs in git.
- **One item per file means facts get repeated. Keep the repeat to a sentence.** A fact two plans both need
  (a breaking change, a config shape) is stated in each. That's the accepted cost — what isn't accepted is a
  shared background section growing inside several plans at once.
- **Delete it when it lands** — or trim it down to only the part that remains. A plan and a doc should never
  describe the same finished thing. When a plan's content is done, its lasting facts move into a doc (what it
  is now) or design (why it was built that way).

## Done when - tick boxes, each with its check

**Every plan ends in a `## Done when` section, and every clause is a checkbox.** Same shape the release set
already uses.

```markdown
- [ ] `_js/instance.js` is gone, along with its webpack entry.
      `test ! -f api/src/.../instance.js` and no `instance:` line in `webpack.config.js`.
- [ ] The reload question is answered in code, readable at the point it was taken.
      **By eye.** Find the answer in the file that takes it. If it is only in this plan, the box is open.
```

**A tick on its own is somebody's claim.** The line under it is what makes the tick worth reading, and it is
the whole point: **not trust, verify.** Two forms and no third:

- **A command or a path** - something that can be run or looked at, and comes back yes or no.
- **`By eye`, plus what to look at.** Some clauses are judgement and no grep settles them. **Say so.** A
  judgement clause with a fake command beside it is worse than one that admits what it is.

**Write the check when you write the clause, not when you tick it.** A clause nobody can say how to check is
not a done-when clause - it is a hope, and it goes or gets rewritten until it is checkable.

**Ticks are working state, not history.** A plan is still deleted when it lands - all boxes ticked means
delete the file, not archive it. **Nothing here is a log of what happened**; that is git.

**Half-ticked is the useful state.** It is what tells the next session where to start, and it is why a plan
that half-shipped gets trimmed to the open boxes rather than rewritten.

## Index

The manifest is generated — see [`_index.md`](_index.md). Regenerate with `just agents all` after adding,
renaming or re-describing a plan.
