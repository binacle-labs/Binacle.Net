---
description: Where benchmark and performance results live and what shape they take - and only then, converting the last four `tooling/*.sh` scripts to `just` recipes
state: blocked
waits-on: "an unanswered question - where benchmark and performance results live and what shape they take. The maintainer keeps them in results/ and does not like them there"
paths:
  - "tooling/**"
  - "results/**"
---

# Where benchmark and performance results go, and only then the four scripts

**The blocker is not the conversion. It is the output.** Every one of the four remaining scripts is a
benchmark or performance harness, and **nobody has decided where their results are persisted or in what
shape.** They land in `results/` today and the maintainer does not want them there. Until that is answered,
converting the scripts moves the writing of results into a recipe and freezes the current answer by accident.

**So this is one question with a mechanical tail, not four mechanical conversions.** Answer the question
first. The conversion is the easy half and is written up below so it can be done in one sitting once the
question is closed.

## The question, stated

- **Where do benchmark and performance results live?** `results/` is a hand-curated vault: a keeper is copied
  in by hand, harnesses write to gitignored scratch. That works and the maintainer still dislikes the result
  sitting there.
- **What shape is a result?** One file per run, one ledger per suite, something dated, something diffable —
  undecided. The shape decides what a recipe can do on its own and what still needs a human.
- **What follows from the answer:** whether a recipe may write into the chosen place at all, or whether it
  keeps writing to scratch and a person promotes a keeper.

The same question blocks the plan to refresh the curated lib benchmark ledger. They are one decision.

## The four scripts

- `tooling/benchmarks.lib.sh`
- `tooling/benchmarks.vipaq.sh`
- `tooling/performance.lib.sh`
- `tooling/performance.vipaq.sh`

Split out of `ci-shared-scripts` on 2026-08-07, and deliberately not named `ci-` anything: **CI runs none of
these.** They gate nothing and no workflow calls them.

Every script CI cares about has already moved. Tests, coverage, the OpenAPI documents, the agent indexes,
running things from source, the build and the image stacks are `just` modules under `tooling/`; setup is
`just install` / `just assets` in the root justfile.

## Why convert them at all, given they work

`just --list` answers "what can I run here", and recipe names complete on tab. Nothing in `tooling/` completes
anything, so these four are findable only by knowing they exist. That is the whole benefit - it is real, but it
is small, and this plan should not be allowed to grow past it.

## How, from the moves that already landed

These are the lessons from converting everything else. They are worth following rather than rediscovering.

- **Absorbed, not wrapped.** The recipe runs the tool directly and the script is deleted. A recipe that only
  calls a script keeps the drift it was meant to remove. A script that is a program rather than a command line
  still counts as absorbed when it moves into a shebang recipe body whole - that is how the 103-line
  `agents-index.sh` moved.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** The launch profiles
  (`N|S|U|All`) went in that way, and `Encode|Decode` is the same shape. Without the reject, a typo falls
  through to the default and reports a green run for something nobody asked for.
- **One module per job, not per script.** Recipes that answer different questions do not share a module just
  because their scripts sat in the same folder. Where two modules need the same few lines, copy them - one
  reaching into another restores the coupling the split removed.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself (MSBuild
  resolves a relative output directory against the project, not the caller) needs an absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

## Watch out

**A recipe must not quietly change where output lands.** That is the whole reason this is blocked: today the
harnesses write to gitignored scratch and a keeper is copied into `results/` by hand. Whatever the answer
turns out to be, it is taken deliberately and not as a side effect of a conversion.

## Six stale references clear themselves when the conversion lands

`lib/README.md` (2), `results/lib/README.md` (2), `results/lib/benchmarks/README.md` and
`results/lib/efficiency/README.md` all give `./tooling/performance.lib.sh` or `./tooling/benchmarks.lib.sh` as
the command to run.

Those are the only places outside `tooling/`, `.agents/` and the repo's top-level docs that name the folder,
and the rule says nothing else should. They were left alone deliberately on 2026-08-12 rather than deleted:
each is the only place a reader of that slice learns how to run its benchmarks, so removing them costs
something real. Converting these scripts turns every one into a recipe name - `just bench lib` - which is not a
path into `tooling/` at all, so the violation disappears without anyone writing the prose twice.

**Update those four files as part of this work.** If the conversion is abandoned, the references need deciding
on their own terms instead.

## Done when

- [ ] Where benchmark and performance results live, and what shape they take, is written down where the
      results themselves are explained.
      **By eye.** Read `results/README.md`. If the answer is only in this plan, the box is open.
- [ ] Every `tooling/*.sh` a maintainer types is a `just` recipe, or says in one line at the top why it stayed
      a script.
      `ls tooling/*.sh` lists nothing, or every file listed carries that line.
- [ ] The six stale references name a recipe, not a path into `tooling/`.
      `grep -rn "tooling/performance\.\|tooling/benchmarks\." lib/README.md results/` returns nothing.
