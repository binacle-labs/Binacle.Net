---
description: One unanswered question - where benchmark and performance results are persisted and in what shape - and the two mechanical jobs waiting behind it
state: idea
waits-on: "a research session coming back with proposals. Nothing here can start until the maintainer picks one"
horizon: next-release
paths:
  - "tooling/**"
  - "results/**"
  - "lib/**"
---

# Where benchmark and performance results go

**The next action is a research session, not a build.** Send an agent to find the conventions other projects
use for persisting benchmark and performance results, and have it come back with proposals for the maintainer
to pick from. Nobody writes a recipe or moves a file until he has picked one, because both mechanical tails
below would freeze the current answer by accident if they went first.

The three questions to come back with proposals against, in the maintainer's words:

- **Do I publish them on any site?**
- **Only on repo?**
- **Do they need dated results?**

Behind those sits the one that decides what a recipe may do on its own: whether a recipe may write into the
chosen place at all, or whether it keeps writing to scratch and a person promotes a keeper.

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

## Research

### Date not recorded - `results/` is a hand-curated vault, and he dislikes the result sitting there

Harnesses write to gitignored scratch and a keeper is copied in by hand. That works. The shape of a result -
one file per run, one ledger per suite, something dated, something diffable - is undecided.

### 2026-08-07 - the four scripts, split out and deliberately not named `ci-` anything

`tooling/benchmarks.lib.sh`, `tooling/benchmarks.vipaq.sh`, `tooling/performance.lib.sh`,
`tooling/performance.vipaq.sh`. **CI runs none of them.** They gate nothing.

**The benefit of converting them is small and must not grow past it.** `just --list` answers "what can I run
here" and recipe names complete on tab; these four are findable only by knowing they exist.

Lessons from the conversions that already landed, worth following rather than rediscovering:

- **Absorbed, not wrapped - for a script that wraps a tool.** The recipe runs the tool and the script is
  deleted. These four are that shape: each is `dotnet run -c Release` with a path. **This does not extend to a
  program.** Since 2026-08-28 a program lives in its own file with a two-line recipe as its door, because
  shellcheck cannot read a `.just` body and a recipe body cannot be run on its own.
- **An alias list becomes a parameter whose `case` rejects an unknown value.** Without the reject a typo falls
  through to the default and reports a green run for something nobody asked for.
- **One module per job, not per script.** Where two modules need the same few lines, copy them.
- **Module recipes need `set working-directory := '..'`**, and a tool that resolves paths itself needs an
  absolute path passed in.
- **When the moved script is a generator, prove the move by diffing its output**, not by its exit code.

### Date not recorded - six stale references clear themselves

`lib/README.md` (2), `results/lib/README.md` (2), `results/lib/benchmarks/README.md` and
`results/lib/efficiency/README.md` all give `./tooling/performance.lib.sh` or `./tooling/benchmarks.lib.sh` as
the command. A recipe name - `just bench lib` - is not a path into `tooling/` at all. If the conversion is
abandoned, those references need deciding on their own terms.

### Date not recorded - the lib benchmark ledger is stale, not just old

`results/lib/benchmarks/` stops at 2025-02-10 while `lib/src` has moved on, including the geometry migration
that took `Dimensions` and `Coordinates` across an assembly boundary. **Those numbers describe code that no
longer exists - do not quote them.** `BestBin_v2` once measured 5-9x faster than v1, 24 B against 208-336 B
allocated; unconfirmed against current code.

Once the question is answered: re-run the lib benchmarks against current code and curate a keeper in.

### 2026-07-17 - algorithm racing was re-measured

The evidence is in the lib design findings. Its scratch reports are in `BenchmarkDotNet.Artifacts/` and a
keeper should be curated in too.
