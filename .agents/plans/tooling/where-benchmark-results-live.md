---
description: One unanswered question - where benchmark and performance results are persisted and in what shape - and the two mechanical jobs waiting behind it
state: blocked
waits-on: "the maintainer answering where benchmark and performance results live and what shape they take. They sit in results/ today and he does not want them there"
paths:
  - "tooling/**"
  - "results/**"
  - "lib/**"
---

# Where benchmark and performance results go

**One decision, then two mechanical tails.** Nothing here can start until the decision lands, and both tails
would freeze the current answer by accident if they went first.

## The question

- **Where do benchmark and performance results live?** `results/` is a hand-curated vault: harnesses write to
  gitignored scratch and a keeper is copied in by hand. That works, and the maintainer still dislikes the
  result sitting there.
- **What shape is a result?** One file per run, one ledger per suite, something dated, something diffable -
  undecided. The shape decides what a recipe may do on its own and what still needs a person.
- **What follows:** whether a recipe may write into the chosen place at all, or whether it keeps writing to
  scratch and a person promotes a keeper.

## Tail one - the four scripts become recipes

`tooling/benchmarks.lib.sh`, `tooling/benchmarks.vipaq.sh`, `tooling/performance.lib.sh`,
`tooling/performance.vipaq.sh`. Split out on 2026-08-07 and deliberately not named `ci-` anything: **CI runs
none of them.** They gate nothing.

**The benefit is small and this must not grow past it.** `just --list` answers "what can I run here" and
recipe names complete on tab; these four are findable only by knowing they exist.

Lessons from the conversions that already landed, worth following rather than rediscovering:

- **Absorbed, not wrapped.** The recipe runs the tool and the script is deleted. A recipe that only calls a
  script keeps the drift it was meant to remove. A script that is a program moves into a shebang recipe body
  whole - that is how the 103-line `agents-index.sh` moved.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** Without the reject a typo
  falls through to the default and reports a green run for something nobody asked for.
- **One module per job, not per script.** Where two modules need the same few lines, copy them.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself needs an
  absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

**Six stale references clear themselves.** `lib/README.md` (2), `results/lib/README.md` (2),
`results/lib/benchmarks/README.md` and `results/lib/efficiency/README.md` all give
`./tooling/performance.lib.sh` or `./tooling/benchmarks.lib.sh` as the command. A recipe name - `just bench
lib` - is not a path into `tooling/` at all. Update those four files as part of this. If the conversion is
abandoned, the references need deciding on their own terms.

## Tail two - the lib benchmark ledger is stale, not just old

`results/lib/benchmarks/` stops at 2025-02-10 while `lib/src` has moved on, including the geometry migration
that took `Dimensions` and `Coordinates` across an assembly boundary. **Those numbers describe code that no
longer exists - do not quote them.** (`BestBin_v2` once measured 5-9x faster than v1, 24 B against 208-336 B
allocated. Unconfirmed against current code.)

Once the question is answered: re-run the lib benchmarks against current code and curate a keeper in.
Algorithm racing was re-measured on 2026-07-17 and the evidence is in the lib design findings; its scratch
reports are in `BenchmarkDotNet.Artifacts/` and a keeper should be curated in too.

## Done when

- [ ] Where benchmark and performance results live, and what shape they take, is written down where the
      results themselves are explained.
      **By eye.** Read `results/README.md`. If the answer is only in this plan, the box is open.
- [ ] Every `tooling/*.sh` a maintainer types is a `just` recipe, or says in one line at the top why it
      stayed a script.
      `ls tooling/*.sh` lists nothing, or every file listed carries that line.
- [ ] The six stale references name a recipe, not a path into `tooling/`.
      `grep -rn "tooling/performance\.\|tooling/benchmarks\." lib/README.md results/` returns nothing.
- [ ] The committed lib benchmark numbers were produced by current code.
      **By eye.** The newest file under `results/lib/benchmarks/` is dated after the geometry migration.
