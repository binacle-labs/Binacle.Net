---
id: ci-cd/decisions/D4
description: a workflow step calls a just recipe, it does not inline the command
status: pending
verified: 2026-09-29
check: D4 against tooling/ci.just and tooling/ci/*.sh, which must be shellcheck-clean and take their inputs as arguments, and against `grep -c 'run: |' .github/workflows/release-docker-image.yml`, which is 1 and must not grow
paths:
  - ".github/workflows/**"
  - ".github/actions/**"
  - "tooling/ci.just"
  - "tooling/ci/**"
---

# D4 — a workflow step calls a `just` recipe, it does not inline the command

**Why:** the release workflow used to inline `dotnet restore` + `dotnet publish` while `tooling/build.just`
published the same project to the same place. They matched by coincidence, and a coincidence is not a
guarantee — the project path, the output folder and the runtime identifier each had two homes that could drift.
Calling the recipe makes CI and a laptop build the same thing by construction.

It also makes a red step reproducible: the `run:` line is what you paste into a terminal.

**Made true on 2026-08-28, where it was not.** The rule held for the *commands* — `just build image`,
`just smoke test` — but roughly 240 lines of shell still sat in `run: |` blocks: the changed-paths filter, the
gate, three per deploy workflow, the Sonar gate poll, three in the release, two in the smoke, one in CodeQL.
Those moved to `tooling/ci/`, one file per operation, behind a new `ci` module.

**One file per operation, not a bigger `.just` module.** A `.just` recipe body can be neither shellchecked nor
run on its own. A `.sh` file is both — a real filename in a stack trace, a thing you can execute while you
debug it, and a file CodeQL's `actions` pack and shellcheck both read. `ci.just` stays a door: a one-line
description and the call, two lines per recipe.

**Corrected 2026-08-28.** This entry used to say a `run:` block cannot be handed to shellcheck either. That is
false — actionlint shellchecks every inline `run:` block when shellcheck is present, and both are installed in
the lint job. The decision stands on its other leg, which was always the stronger one: a `.sh` file can be run
on its own.

**And the shellcheck half is now true of the scripts themselves.** Until 2026-08-28 nothing ran shellcheck over
`tooling/ci/*.sh` — the reason those files exist was a habit, not a fact. `just check scripts` runs it, and the
lint job calls that recipe. shellcheck ships on the runners, so it installs nothing.

**Arguments in, no context read.** A script never reads `github.*` or the runner's own environment for an
input; the workflow puts the value in `env:` and the step passes it on the command line. That is what makes
every one of them runnable here, and it keeps the existing "an interpolated value goes through `env:`" rule
true by construction rather than by habit.

**A single command stays inline.** `dotnet dotnet-sonarscanner begin` and the one-line Docker Hub summary are
already readable, and wrapping them buys a file and loses nothing.

**It reaches recipe bodies too, and the first one moved on 2026-08-31.** The argument above was written about
`run:` blocks, but it never depended on where the shell sat - `just coverage table` was 60 lines of it, CI
calls it through `just coverage all sonar`, and nothing could check it. It is now `tooling/coverage.table.sh`
and the recipe is one line. `just check scripts` widened to `tooling/*.sh` in the same change, because a
script nothing checks is a recipe body with a longer name. The other modules still carry bodies; each moves
when it grows past a command or two, not in a sweep.

**One block was missed by the 2026-08-28 sweep, and it was found on 2026-08-31 by adding to it.** The release's
`publish` job still copied the image with 20 lines of `run: |`. A change that made it two copies with a sign
between them grew it rather than moving it, which is how the miss surfaced: **a rule you can add to without
noticing is a rule nothing enforces.** It is now `tooling/ci/moving-tags.sh` and `tooling/ci/copy-tags.sh`.

**And it happened again.** `grep -c 'run: |'` on `release-docker-image.yml` returns **1** as of 2026-09-04,
not 0 - a five-attempt retry loop around `just image verify`, added after this entry was written.
**The entry that says a rule you can add to without noticing is a rule nothing enforces was itself added to
without anyone noticing.** The block is small and it wraps a recipe rather than replacing one, so it is
recorded here rather than moved; **the count is the check, and it is 1.**

**Why there is a filter at all, since the simpler shape has none.** Copying the version tag, signing, then
copying the *whole* list including it again needs no filter, no guard and no step output. It is idempotent and
it works today. **It stops working the day Docker Hub immutable tags go on** - freeze the version tags with a
pattern, leave `latest` free, and the second write of the version tag is rejected and the release goes red at
its last step. The filter is what makes that a setting rather than a pipeline change.

**The guard is not a signing check, and the script used to say it was.** The signature is on the digest and
every tag points at that digest, so nothing goes out unsigned whatever the split does. What it catches is
`build`'s version and metadata-action's disagreeing, which would publish two version tags. **Corrected
2026-08-31** after the question "is this not just a filter" made the stated reason not survive being said out
loud.

**Splitting it in two rather than one bigger script** follows the one-file-per-operation half of this entry.
"Which public tags move" and "copy a digest to these tags" are separate questions, and both are now runnable
here: `just ci moving-tags` was checked against a real release, a prerelease and a deliberate mismatch before
the workflow ever saw it. That is the whole point of the decision and it is the first time it has paid out.

**Not every recipe a workflow calls is a `ci` one.** `publish` also runs `just image verify`, which belongs to
the `image` module because a person runs it too. **Do not move it into `tooling/ci/`** - the rule is that a
step calls a recipe, not that every recipe a step calls lives in one folder.

**Two jobs gained `contents: read` for this, and that is the price.** A script is read out of the working
copy, so `gate` in `pull-request.yml` and `summary` in `codeql-analysis.yml` now check out. Both had a
narrower permission set before. The trade is deliberate: the alternative is a hand-kept list of job names in
`gate` that a new job can be left out of, which is a failure nothing reports.

**The composite actions followed on 2026-08-28, and they call the script by path, not through `just`.** The
four `install-*` actions held 36 lines of download-and-checksum shell, the last shell in CI that nothing
checked. Each is now a door onto `tooling/ci/install-<tool>.sh`. No `just` in between: an action that installs
a tool must not need another tool installed first, and there is no recipe to call because nothing else calls
these. It works because a local action is read out of the working copy — `./.github/actions/install-lychee`
only resolves after `actions/checkout`, so the repository is always present.

**The version and the checksum moved into the script with the shell, against the earlier plan.** That plan said
to leave them in the action's `env:` "where a reader and Dependabot look". Dependabot never looked: it rewrites
`uses:` pins, and these four are hand-pinned binaries it has never touched. Leaving them behind would have made
two homes for one fact and left the script unrunnable without typing a checksum. One home, in the script, and
`tooling/ci/install-lychee.sh` is now the whole install on a laptop as well as in CI - `DEVELOPMENT.md` runs
these four rather than spelling out four `curl` blocks that repeated every version.

**All four checksums are upstream's.** hurl and lychee publish `<asset>.sha256`; actionlint and
container-structure-test publish a `checksums.txt`. Both of the ones this was not previously recorded for were
fetched and compared on 2026-08-28 and match byte for byte.

**`build-jekyll-site` keeps its two inline lines.** `npm ci --ignore-scripts` and `just build "$SITE"` are not
shell worth checking, and a file for each would be a file that says nothing.

**The `publish` job used to be the exception, and it is not any more.** This paragraph argued it never checks
out and therefore gets no composite action and keeps its inline copy. **All three are false as of
2026-09-04:** it has a `Checkout` step, it uses `./.github/actions/setup-just`, and its copy is
`just ci copy-tags`. The move is recorded above; this paragraph was the pre-move argument and was never
deleted. **It is kept as the record of what the objection was** - repository code beside the Docker Hub
credential - which is still the thing to weigh before adding anything else to that job.

The corollary is that recipes must stay callable from CI as they stand — nothing interactive, no `sudo`, no
local-only paths. Directory preparation that needs `sudo` is a precondition of *running* a compose stack, not
of building anything, and is deliberately kept out of the build recipes.
