---
id: ci-cd/decisions
description: CI/CD decisions ledger — why a release is dispatched with a version and tagged last, why the pipeline stages on GHCR and copies to Docker Hub by digest, why the prerelease guard is metadata-action's rather than a job-level skip, why the notes come from CHANGELOG.md, the pinning rules, why lychee is a pinned binary rather than its own action, why the test suite is split in two by what ships, why the gem sources need a built project and what a slnx project type decides, why a workflow step calls a just recipe rather than inlining shell, how CodeQL is configured, what `just image verify` checks and in what order, and the open questions about the PR gate and supply-chain attestation.
verified: 2026-08-31
check: Decisions still match .github/workflows/*.yml and tooling/build.just; D8's scope claims against tooling/ci/sonar-analysis.xml, whose exclusions must still name sites/*/js, sites/*/lib, the media folders and sites/**/*.html and must not exclude either site whole; D1 against release-docker-image.yml, whose trigger must be workflow_dispatch alone with a required version input and whose gate job must carry the ref, semver and tag checks; D2/D3/D14 against release-docker-image.yml's publish job, which must carry no prerelease condition, D7 against tooling/changelog.just, D6 against shared-smoke-image.yml's runs-on, D11 against .github/dependabot.yml, D12 against build.just's publish recipe, D14's STAGING_IMAGE against release-docker-image.yml, D15's identity regexp against SECURITY.md and tooling/image.just, D16 against .github/actions/install-lychee and the deploy workflows' link-check step, D17 against all three deploy workflows' triggers, which must stay workflow_dispatch only; D4 against tooling/ci.just and tooling/ci/*.sh, which must be shellcheck-clean and take their inputs as arguments; D18 against the test lists in tooling/tests.just and the steps in shared-image-tests.yml and shared-site-tests.yml, which must together name every test and share exactly the five javascript ones, and against the deploy workflows' first job; D20 against codeql-analysis.yml, whose matrix must stay four languages on build-mode none with a category per language, D22 against Binacle.Net.slnx, whose ruby.proj entry must carry a buildable Type and not Shared, and against tooling/ci/sonar-analysis.xml, whose ruby coverage path must stay relative to ruby/; and D21 against tooling/image.just, whose verify recipes must take a version with no default and reach no registry that needs a login
paths:
  - ".github/workflows/**"
  - "tooling/ci/**"
  - "tooling/image.just"
---

# CI/CD — decisions ledger

Why the workflows are shaped the way they are. What they *do* is `$ci-cd` and `$ci-cd/release-pipeline`; this
file is the reasoning, so a later session does not re-litigate a deliberate choice or "fix" one.

Several of these were argued out at length while the release pipeline was being rebuilt, in a working file that
does not outlive the work. This is where that reasoning lives now.

## Locked

### D1 — a release is dispatched with a version, and the tag and the release are its last job

`on: workflow_dispatch` with a required `version` input carrying no leading `v` — `3.0.0`, which is both the
`CHANGELOG.md` section name and the Docker tag. The workflow forms `v3.0.0` and, once the image is built,
smoked and published, `gh release create --target <commit>` makes the tag and the release in one call.

**Why the release is not the trigger.** With `release: published` the release is already public when the
workflow starts, so a failure leaves an announced release whose image never arrived. Creating it last inverts
that: a failure leaves nothing a user ever saw.

**Why nothing but the last job may create the tag.** A tag that is the trigger exists before a single test has
run, so a red suite leaves a tag — and often a GitHub release — to delete by hand before the version can be
tried again. Made last, a failed run leaves nothing at all: fix the cause and press the button again.

**One trigger, never two.** `workflow_dispatch` replaces the tag trigger rather than joining it. With both on,
the tag the last job pushes re-enters the workflow and builds everything a second time. A `GITHUB_TOKEN` push
does not trigger workflows today, which would hide it — that is exactly the coincidence this decision refuses
to depend on.

**The version is checked before anything is built.** The gate job proves the dispatch is on `main`, that the
version is semver shaped with no leading `v`, and that `v<version>` does not already exist on some other
commit. The semver check is also what keeps the input out of a shell whose meaning it could change; every
interpolation still goes through `env:` on top of that.

**The tag-exists check has an exception, and it is load-bearing.** A tag that already points at *this run's*
commit passes. Without that, a run that publishes the image and then fails at the release step could never be
dispatched again, and the only way out would be deleting a tag.

**What the tag check closes.** The old shape's recorded residual risk was re-pushing an old version tag:
`v1.0.0` matched the trigger, `CHANGELOG.md` carries a `## [1.0.0]` section so the notes gate passed it, and
`latest=auto` marks any non-prerelease semver as latest — so `latest` moved backwards. Now the gate stops it
in seconds. **The standing rule survives anyway: do not delete and re-push a released tag.**

**The residual risk that replaces it is the mirror image, and smaller.** Fail between `publish` and the
release and there is an image on Docker Hub with `latest` moved, no tag and no release. Recovery is a
re-dispatch, which rebuilds and re-copies the same digest under the same tags. There is no window inside the
release job itself: one API call makes both, so the image cannot be public with the tag missing because the
tag step failed.

**Amended 2026-08-28 — the tag is made by the release, not pushed before it.** `gh release create` creates a
missing tag itself and `--target` says on which commit, so the separate `just ci push-tag` step is gone from
this workflow. `push-tag.sh` stays for the three deploy marker tags. `--target` is ignored when the tag already
exists, so the load-bearing exception above — a re-dispatch on a run whose tag is already there — still works
exactly as written.

**Amended 2026-08-31 — `run-name` carries the version, because the run's ref cannot.** A dispatch is listed
against `main`, since the tag does not exist until the last job, so the Actions list showed the commit message
and nothing said which release a run was. `run-name: Release v${{ inputs.version }}` puts it in the list. The
branch chip still reads `main` and that is not fixable without dispatching on a tag, which D3 explains is the
weaker ref. **Nothing verifies against the run name** — the version that matters is the `version` input, which
the gate checks and every later job reads.

**The GitHub web release route stops working, and that is accepted.** *Draft a new release → create tag on
publish* now builds nothing, silently. The release job's create-or-edit branch is left in place: it costs
nothing and still covers a tag made by hand.

**No `dry_run` input.** The other repository that releases this way has one, because its release commits to
`main` and there is no cheaper rehearsal. Here a prerelease already runs every job including `publish`, so a
dry run would duplicate it — and making `publish` conditional would reintroduce the trap in D3, where `release`
then needs `if: ${{ !failure() && !cancelled() }}` or a beta silently gets no GitHub release.

**Superseded 2026-08-28 — what changed and what did not.** This ran on `on: push: tags: 'v[0-9]*'` from the
rebuild until then. Everything above about `release: published` and about creating the release last is the
original reasoning and is untouched; only the entry point moved. Two things went with it. The pattern was
`v[0-9]*` rather than `v*` because the three site deploys push `docs-<run>`, `demo-<run>` and `www-<run>` and
a release build must never fire on one — the release fires on no tag at all now, so that constraint on their
tag names is gone. And the certificate identity inside every signature changed with the ref; that is recorded
under D15, where the verify commands are listed.

### D2 — build once, smoke the registry copy, then copy by digest

Order: push the immutable tag to GHCR alone, pull it back from there and smoke it, then copy that digest to
Docker Hub with `docker buildx imagetools create`, under all three public tags at once.

**Why not the tempting alternative.** Building with `load: true`, smoking the local image, then pushing sounds
equivalent and is not — it tests a local copy that *ought to be* identical to what lands in the registry.
Compression, manifest shape and attestation handling are precisely what a registry round trip changes, so the
only honest smoke target is what the registry actually serves.

**Promotion is a transfer, not a rebuild.** A manifest is content-addressed, so `imagetools create` preserves
the digest: Docker Hub serves the exact bytes that passed, not a rebuild that ought to match. The copy source
is the digest rather than the tag, so that holds even if something re-tagged staging in between. All three tags
go in one command - they are aliases of one manifest, so the blobs move once.

**Verified across registries on 2026-08-11, not assumed.** The published `v3.0.0-beta.1` index - an amd64
manifest plus its attestation manifest - was copied by digest from Docker Hub into a scratch registry. It came
out on `sha256:c458644...`, the digest it went in with, and all three tags resolved to it. The attestation
entry survived the copy.

**Superseded 2026-08-11 — what changed and what did not.** This decision originally ran entirely on Docker Hub:
the immutable tag was pushed there, smoked there, and `docker buildx imagetools create` re-pointed `3.0` and
`latest`. The reasoning above survived intact; only the registry topology changed. What forced it was the one
cost the old shape accepted and should not have — an unsmoked artifact was briefly public on the registry users
pull from. It was argued as acceptable because nobody follows an exact pin on release day, and that is true, but
"true for the tag nobody watches" is a weaker claim than "never happens", and D14 makes it never happen for
free. The copy command did not change - `imagetools create` handles a cross-registry source as readily as a
local one, which is what kept a third-party tool out of the job that moves the artifact users pull.

### D3 — the prerelease guard is metadata-action's, not an explicit skip

`publish` runs for every tag. A prerelease reaches Docker Hub with its **immutable tag only**, because
metadata-action skips `{{major}}.{{minor}}` for one and `latest=auto` withholds `latest` for the same reason.
So a beta can never move `3.0` or `latest`, and the guard is the action's rather than this workflow's.

**Observed, not assumed.** Checked on Docker Hub on 2026-08-06 after `v3.0.0-beta.1`: it published
`3.0.0-beta.1` and moved neither `latest` nor `3.0`, and no `3.0` tag existed at all.

**Reversed 2026-08-11, same day it was introduced.** For part of that day `publish` carried
`if: ${{ !contains(github.ref_name, '-') }}`, so a prerelease stopped after `smoke` and lived only on GHCR —
"Docker Hub carries releases only". Two things killed it:

- **It was never a safety rule.** The property that matters is *nothing unsmoked reaches Docker Hub*, and that
  comes from `smoke` running before `publish`. It holds identically with or without the skip. What the skip
  actually bought was a tidy tag list.
- **It cost real deployability.** A beta could then be pulled only from GHCR, and a host that cannot route to
  GitHub's AS36459 — which is not hypothetical — could not deploy the beta at all. Paying that for tidiness is
  the wrong trade.

**What the reversal takes back with it.** `release` no longer needs `if: ${{ !failure() && !cancelled() }}`;
with nothing conditional above it, plain `needs` says the same thing. Restore that condition if any job in the
chain ever becomes conditional again, or a beta will silently get no GitHub release.

No `{{major}}` tag is emitted on purpose — a bare `3` would cross minor lines.

### D14 — GHCR is staging, and only the release workflow touches it

Everything built lands on `ghcr.io/binacle-labs/binacle-net` first. Docker Hub receives only what has been
smoked there.

**Why a second registry at all.** It buys the property D2 used to trade away: nothing unsmoked is ever visible
where users pull from - `smoke` runs against the staging copy, and only a smoked digest is ever copied across. **GHCR is staging; Docker Hub is what users pull, and it carries every tag the
pipeline publishes, betas included.**

**Only the release workflow touches GHCR - decided 2026-08-15, and it is the strong form of the rule.** The
staging registry exists so the workflow can push an image, smoke it and copy the smoked digest to Docker Hub.
That is its whole job. **Nothing else reads it**: no public surface names it, no local recipe queries it, and
no deployment pulls from it. **One image, one place anyone gets it from.**

**What that changed, on the day it was decided.**

- `SECURITY.md` and `CHANGELOG.md` stopped naming it. The docs-site verification page is written the same way
  at the next deploy.
- **`just image verify` lost its `digest` check and is Docker Hub only.** That check compared the tag across
  the two registries to say Docker Hub serves what the smoke job passed.

  **That property is now the workflow's to keep, not a reader's to re-derive**, and it is not lost. `publish`
  copies by digest instead of rebuilding, so the copy cannot be a different artifact; the run log shows the
  digest at each step. From Docker Hub alone, the SLSA provenance names the run that built the image and
  `cosign verify` proves it came from this repository's release workflow - which is the question a reader
  actually has. What no longer has an outside witness is "this digest is the one `smoke` pulled", and that was
  only ever checkable by reading staging.
- The Docker Hub page must not name it - already the plan's own rule, but for a weaker reason.
- The deployment host is repointed at `binacle/binacle-net`.

**Why the rule is worth the check it cost.** A staging registry anyone reads is a second published registry
wearing a different word. It grows instructions, support questions and surfaces to keep true, all for bytes
identical to what Docker Hub already serves. The moment something outside the workflow depends on it, it is
not staging.

**The consumer-side argument for a public package is spent.** It was that a deployment host could pull a beta
from GHCR with no `docker login`. That held only while a prerelease stopped at staging, and it stopped being
true on 2026-08-11 when the prerelease skip was reversed - every beta now reaches Docker Hub under its
immutable tag. Nothing exists on GHCR that Docker Hub does not have.

**Why GHCR specifically.** `GITHUB_TOKEN` is minted per run and expires with it, so staging needs no stored
credential and nothing to rotate. Keeping Docker Hub free of anything unsmoked or unreleased is what the second
registry buys, and it does that without adding a secret anywhere.

**The package is private, and nothing in the pipeline minds.** Both jobs that reach GHCR log in with
`GITHUB_TOKEN` - `build` to push and `publish` to read the manifest it copies - and `shared-smoke-image.yml` logs in
the same way before pulling the staging tag. Public was only ever load-bearing for readers outside the
workflow, and there are none left. Private also removes the last public pointer at GHCR that this repo does
not control: the package's own page, which advertises a `docker pull ghcr.io/...` line.

**It arrived by the move rather than by a flip.** GHCR defaults a new package to private, and the package at
`ghcr.io/binacle-labs/binacle-net` was created fresh when the repository moved - see `$decisions#D1`. The old
`ghcr.io/chrismavrommatis/binacle-net`, which had been set public after its first run, was deleted on
2026-08-16. `3.0.0-beta.3` then ran `build`, `smoke` and `publish` green against the private package, so all
three jobs are proven against it.

**Nothing deletes the staging copy, and that is deliberate.** It is the rollback source if a Docker Hub tag is
ever found bad — the exact bits that were smoked, still addressable by digest. The second reason is failure
mode: a cleanup step inside the release path can fail, and a release that goes red *after* the image is
published is the worst outcome the ordering exists to avoid. If the package ever needs pruning, it happens on
its own schedule, not in this workflow.

**The workflow creates the package on its own** — `packages: write` is enough to create one in the repo's
namespace, and the `Dockerfile`'s `org.opencontainers.image.source` label is what links it back. An earlier
version of this decision claimed a manual first push was required; it is not. The `permission_denied` failure
that claim came from is real but narrower — it happens when a package already exists in the namespace
*unlinked*, from a personal token or a recreated repo.

### D4 — a workflow step calls a `just` recipe, it does not inline the command

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
noticing is a rule nothing enforces.** It is now `tooling/ci/moving-tags.sh` and `tooling/ci/copy-tags.sh`, and
`release-docker-image.yml` has no inline `run: |` block left at all - `grep -c 'run: |'` returns 0.

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

**The release `publish` job is the exception and keeps its inline block.** It holds the Docker Hub credential
and deliberately never checks out, which is also why it gets no composite action. Moving its `imagetools`
copy into a script would mean checking out repository code beside that credential to save fifteen lines. Not
worth it; if it ever does check out for another reason, move the block then.

The corollary is that recipes must stay callable from CI as they stand — nothing interactive, no `sudo`, no
local-only paths. Directory preparation that needs `sudo` is a precondition of *running* a compose stack, not
of building anything, and is deliberately kept out of the build recipes.

### D5 — a repo variable may not duplicate a fact that lives in the repo

`API_PROJECT_PATH`, `BUILD_OUTPUT` and `BUILD_DOCKERFILE` were removed and replaced by the literal values.

**Why:** a repo setting is invisible to a reader of the repo, it is not versioned with the code, and it can only
drift from the fact it duplicates. One did — `API_PROJECT_PATH` still named the pre-move `src/` path after the
layout change and broke the publish. The project and output folder are decided in `tooling/build.just`, and
there is exactly one `Dockerfile`, at the repo root.

What legitimately stays a variable is a value with **no** home in the repo: the SDK version, the Docker Hub
coordinates, the Sonar project key.

### D6 — `shared-smoke-image.yml` pins `ubuntu-24.04`, everything else takes `ubuntu-latest`

**Why:** hurl links `libxml2.so.2`. Ubuntu 26.04 ships only `libxml2.so.16` and carries no compat package, so
hurl dies there with a missing-library error that reads like a hurl bug rather than a distro change.
`ubuntu-latest` will move to 26.04 eventually, and this workflow runs rarely enough that it would break on the
day it is needed most.

### D7 — `CHANGELOG.md` is the single source of release notes, and a missing section is fatal

`gh release create --notes-file`, with the body produced by `just changelog extract <section>`. The `notes` job
proves the section exists before anything is built. There is **no fallback to generated notes.**

**Why a changelog and not a per-release file.** The body used to come from `.agents/release-notes-<tag>.md`,
which was body-only so it could be published whole — that part was right and is kept. What was wrong is where
it lived: `.agents/` deletes a release's companions once the version ships, so the notes source was a file whose
own contract guaranteed it would disappear. A published release body is a permanent record, and it belongs in a
permanent file at the repo root where users read it.

**Why one section accumulates per cycle.** Betas publish `## [Unreleased]`; renaming that heading to the
version is the last edit before the real tag. A beta's notes are the in-progress notes at that moment, not a
version of their own, which is also why prereleases are excluded from the file — the GitHub releases stay as
the record of what each beta said.

**Why no fallback.** The old `--generate-notes` fallback existed because a prerelease normally had no written
body. Under the current shape a prerelease publishes `[Unreleased]`, which always exists mid-cycle, so the
fallback's only remaining effect would be to let a *real* release silently publish a commit list as its body.
Failing the build in seconds is the better outcome, and it is checked first for exactly that reason.

**Why the parsing is a `just` recipe and not inline YAML.** Same reason as D4 — CI and a laptop must read the
file the same way, and the exact body has to be previewable before the tag is pushed. A section terminates at
the next heading that *parses as a version*, not at the next `## `, because bodies carry their own subheadings
and stopping at those would truncate every section at its first one.

**Heading depth is normalised in the file and restored on the way out.** A release is `##` and its own sections
are `###`, so the file nests under a single `# Changelog`. `extract` shifts each section so its shallowest
heading returns to `##`, since a release body has no parent heading. Deriving the shift from the section's own
minimum keeps relative depth intact and means nothing has to be recorded anywhere.

**The docs site's release-notes page is hand-copied from this file, not generated from it — decided
2026-08-14.** Each version folder on the docs site carries its own `release-notes.md`, and a version's section
is copied into it by hand; a patch release appends a section rather than replacing the page. **The cost is
known and accepted: the same notes live in two places and they drift.** v3.0.0 shipped with three additions the
page never had, because the release body gained content after the page was written.

**So the docs handover is the control.** Every release's docs deploy checklist has to list what the changelog
gained since that page was last written. `just changelog extract <section>` prints the current text to compare
against. Generating the page instead was rejected: the two audiences differ — a GitHub release body is read
once at the tag, and the docs page is read by someone already on that version — and a generator would have to
own the site's front matter and heading style as well as the text.

### D8 — Sonar analysis is a CI run, and Automatic Analysis stays off

**Why:** coverage requires a build. Automatic Analysis only reads source, so it can never report coverage, and
the two fight if both are on. The build and the coverage run must sit between `Sonar begin` and `Sonar end` —
the scanner only sees projects compiled inside that pair — and a failing suite skips `Sonar end`, so a broken
run publishes nothing. That last part is deliberate, not a bug.

Two mechanical consequences: the checkout needs `fetch-depth: 0`, because a shallow clone makes all code look
new to the new-code comparison; and the scanner is a Java program whatever language it analyses, so the job sets
up a JDK.

**The trigger is `workflow_dispatch` and nothing else — recorded 2026-08-28.** No schedule: a nightly run
re-analyses a commit nothing changed and reports the same numbers, which teaches everyone that the run means
nothing. No `pull_request` either, for the reason in O2 — the coverage condition is red before anyone writes a
line.

**Scope and coverage paths live in `tooling/ci/sonar-analysis.xml`, not in the workflow.** The Scanner for .NET
ignores `sonar-project.properties`, so that XML is the file form it reads, and `/s:` needs an absolute path.
Only the key, org, token and host stay in the YAML.

#### The published sites are in scope, and that was a reversal {#sites-in-scope}

**Changed 2026-08-09.** `sonar.exclusions` used to drop both site directories whole, and the reason given was
that they are a separate deliverable written in their own session - **a workflow reason, not a scope reason.**
The test in the analysis xml is whether the code is ours to author, review and change. It is.

**The cost landed exactly where it hurt.** Those Jekyll sites are the only public attack surface in the
repository, `5e5f8c02` was an XSS fix in one of them, and the exclusion kept Sonar from looking for the next
one.

What is in scope is small - six hand-written js, fifteen scss, and the site yml and json. The generated and
vendored parts (`sites/*/js`, `sites/*/lib`, the two `media` folders) are named individually and stay out;
they are gitignored, so a CI checkout would not see them anyway. `sites/**/*.html` stays out too, because a
Jekyll template with `---` front matter and Liquid in its attributes is not an HTML document and Sonar's HTML
analyser can only misread it.

**Measuring and fixing are separate jobs, and only fixing was ever restricted.** A finding under `sites/` is
not fixed in a coding session; it becomes a row for the session that owns those files.

#### Four settings that cannot live in the repo {#sonar-ui-settings}

Scope, coverage paths and the test/product split are all in the repository. These four are only in the
SonarCloud UI, and they are what the gate actually hangs on.

- **New code period.** It was `previous_version`, and because the scanner is never passed `/v:` the project
  version never changed, so the period stayed pinned to the **first analysis, 2025-04-15**. Sixteen months of
  work counted as new code: 882 of 1059 code smells, and a gate asking 80% coverage on new code was really
  asking it of everything ever written. Set to **"Number of days = 30"** on 2026-08-09, and the new-code smell
  count fell from 882 to 8. **The textbook answer once Sonar runs on pull requests is "reference branch =
  main" - do not assume it applies here.** The Free plan analyses only `main` plus pull requests targeting it,
  and a pull request is already graded on its own diff whatever this says, so the setting may be unavailable
  or a no-op and "days = 30" may be permanent.
- **Two findings marked in the UI**, neither with an honest code fix: `S2245` on `getRandomInt.ts`, which
  picks demo data rather than secrets, and `S2068` on the `AccountGetResponse` OpenAPI example, where
  `PasswordHash` is the literal `"type::hash::salt"`.
- **Automatic Analysis stays off**, per this decision.
- **No source glob in the UI.** A leftover `sonar.inclusions` of `src/**/*` from a flat layout is what made
  the 2026-08-07 run index zero files **and still report success**. Scope is exclusions only, and they live in
  the xml.

### D9 — the Postgres service in `shared-image-tests.yml` carries no password

`POSTGRES_HOST_AUTH_METHOD: trust`, and no `POSTGRES_PASSWORD`.

**Why:** the container lives for one job, is reachable only from that job, and is thrown away after. There is
nothing for a password to protect. Under `trust` it accepts any password, so the integration tests connect
unchanged using the shared local-dev connection string.

**This removed the credential from that file, not from the repo.** The same local-dev password is still in
`tooling/serve.services.yml`, `tooling/image.full.yml`, the test default in `BinacleApi.cs` and
`Config_Files/ServiceModule/ConnectionStrings.Development.json`, where it is load-bearing. It is the same value
everywhere on purpose. Change it in all of them or none.

### D10 — `npm ci --ignore-scripts`

**Why:** an install-time lifecycle hook is arbitrary code execution from a dependency. Nothing here needs one —
no workspace declares `prepare` or `postinstall`, and the only dependency with an install script is `fsevents`,
which is darwin-only and never installed on a Linux runner. The flag costs nothing and closes the hole.

### D11 — every action is pinned by commit SHA, and Dependabot keeps the pins moving

With the version in a trailing comment, so the pin is readable. `.github/dependabot.yml` raises a weekly PR per
action, rewriting the SHA and the comment together.

**Why:** a mutable tag is a supply-chain hole — it can be re-pointed at any commit, including after review. The
trailing comment is what keeps the pin maintainable; a bare SHA tells a reader nothing about how far behind it
is.

**Why first-party actions too, as of 2026-08-11.** `actions/*` and `docker/setup-*` were left on major tags on
the grounds that the publisher is trusted. That is a weaker rule than it looks: the risk a SHA pin addresses is
the tag being re-pointed, and `actions/checkout@v4` is exactly as re-pointable as any other tag. Two rules also
meant every reader had to know which action fell under which. One rule, applied to all six workflows.

**The pin and the automation are one decision, not two.** A pinned action with nothing watching it stays on
whatever commit it was set to and stops receiving security fixes, which is worse than a floating tag because
nothing reports it. `docker/build-push-action` sat at v5.4.0, several majors behind, which is what made the
point concrete.

**The pins stayed in the composite actions; the config grew instead, on 2026-08-19.** Four outside SHAs moved
into `.github/actions/` with the workflow restructure, and Dependabot does not reach that folder from
`directory: /` — it covers `.github/workflows` and a root-level `action.yml`, and nothing else. The open
question was whether to answer that by pulling the four pins back out into a workflow file. **No:** the point
of the composite actions is that a setup step is written once, and undoing that to satisfy a config format is
the tail wagging the dog. `.github/dependabot.yml` carries one entry per action folder instead. The cost is
that adding an outside pin to a new action means remembering to add an entry — which is why the rule is
written down in `$ci-cd` beside the actions themselves, not only here.

### D12 — the image is framework-dependent, and the publish flag is spelled out

`--no-self-contained --runtime linux-x64`, written explicitly rather than left to the default of a bare
`--runtime`.

**Why:** the runtime comes from the `aspnet` base image, which is the whole point, and that has to be readable
on the line. The image was self-contained until 2026-08-10 while basing on `aspnet:10.0`, so it carried two
copies of .NET — the bundled one the app ran on, and the base image's, which nothing loaded.

**Measured before the change was kept:** image 150.2 MB to 103.2 MB, publish output 123 MB to 18 MB,
`System.*.dll` count 172 to 4. All structure assertions, all five smoke profiles and every test green on
the rebuilt image. The entrypoint did not change — `dotnet Binacle.Net.dll` was always the framework-dependent
idiom, which is what made the old pairing wrong in the first place.

The second reason is durability: framework-dependent means a .NET security fix reaches users by rebasing on a
newer `aspnet` tag rather than by republishing the app, which matters for a project that ships months apart.

**Written twice, on purpose.** The flags are in `tooling/build.just` and again in the publish step of
`release-docker-image.yml`. Nothing checks that the two agree, so changing one means changing the other.

### D13 — per-build OCI labels are applied at build time, never as `LABEL` fed by `ARG`

Version, revision and created are set with `--label` (locally) or by metadata-action (in CI). Constant labels
stay as `LABEL` lines in the `Dockerfile`.

**Why:** those three change on every build. As Dockerfile `LABEL`s fed by `ARG` they would invalidate the layer
cache from that point down, for metadata nothing executes. `--label` writes image-config metadata with no layer
and no cache cost.

metadata-action overrides three of the Dockerfile's constant labels on purpose — `licenses`, because
auto-detection returns `NOASSERTION` for a repo that declares more than one licence; `url`, which should be the landing site
rather than the repo; and `description`, which auto-fills from the GitHub repository blurb and silently beats
the `Dockerfile`'s caption. **The third was moved here on 2026-08-28** from a comment in the workflow, which
was the only place it was written down.

### D15 — the image carries an SBOM and provenance, and is signed keyless

`build-push-action` gets `provenance: mode=max` and `sbom: true`; `cosign sign` runs against the digest with
no key, using the job's OIDC token.

**Provenance was already being produced, and the ledger said the opposite.** Inspecting the published
`v3.0.0-beta.1` on 2026-08-11 showed an OCI **image index**: the amd64 manifest plus an `unknown/unknown`
manifest annotated `vnd.docker.reference.type: attestation-manifest`, carrying an in-toto document with
predicate type `https://slsa.dev/provenance/v1`. That is buildx's default in Actions. Nothing in this repo
asked for it, which is exactly how it came to be written down as absent. Stating it in the workflow makes it a
choice rather than a default that can change underneath us.

**`mode=max` over the default `min`** records the full build definition rather than just the materials. The
only build arg is `VERSION`, so nothing secret is captured — adding a secret-bearing build arg means revisiting
this.

**Why signing is separate from attestation, and why it happens twice.** The SBOM and provenance say how the
image was built; without a signature they do not prove the record itself was not altered. cosign closes that.
But a cosign signature is **not** a manifest inside the index — so unlike the attestations it does not travel
with the copy in D2. The staging image is signed on GHCR and the published image is signed again on Docker Hub,
so the copy users pull verifies. **The Docker Hub signature is the load-bearing one; since D14 nothing outside
the release workflow reads the staging signature at all.** Removing the `build` job's cosign step is a live
question and deliberately not release work - it deletes a step from the path a tag runs, for no gain beyond
tidiness. (The parenthetical here used to say GHCR was the only
place a beta ever exists; that stopped being true on 2026-08-11 when the prerelease skip was reversed, and
every beta now reaches Docker Hub under its immutable tag.) Signing the **digest** rather than a tag means one signature covers
`x.y.z`, `x.y` and `latest`, since all three are aliases of it.

**Corrected 2026-08-11, against the real artifact.** This said the signature lands in a `sha256-<digest>.sig`
tag. That is the older cosign scheme and it is not what happens here. `sigstore/cosign-installer` v4.1.2
installs a cosign that attaches the signature as an **OCI 1.1 referrer**: a manifest whose `subject` is the
index digest, one layer of `artifactType` `application/vnd.dev.sigstore.bundle.v0.3+json`, reachable through
the referrers API and by the fallback tag `sha256-<digest>` with no suffix. Verified by walking the GHCR
manifests for `v3.0.0-beta.2`.

The correction does not move the decision — a referrer is still outside the index and still does not survive
`imagetools create`, so signing twice is still required. It matters because anyone auditing the registry for a
`.sig` tag will not find one and may conclude the image is unsigned.

**A second way to reach that wrong conclusion, found 2026-08-13 on the published beta 2.** Docker Hub answers
the referrers API for the signature; **GHCR answers it with a 404**, so the same query returns nothing there.
The signature is present - it is in the GHCR tag list as `sha256-<digest>` and `cosign verify` passes against
both registries. Only a failed verify is evidence of an unsigned image; an empty referrers response is not.

**The verify invocation is copied to several surfaces on purpose, and one thing changes it.** The same
`cosign verify` - identity regexp plus issuer - now lives in `CHANGELOG.md`, `SECURITY.md`, the `image.just`
recipe and the docs site, and is headed for the Docker Hub page. That repetition is deliberate: each audience
arrives somewhere different, and a link instead of the command defeats the point. **The only things that
change it are renaming `.github/workflows/release-docker-image.yml` or moving the repository** - both rare,
both visible in a diff. If either happens every copy changes together, and the certificate-identity regexp is
the part that breaks; the issuer flag never moves. `SECURITY.md` is the wording the others follow.

**The second of those happened on 2026-08-16**, when the repository moved to the `binacle-labs` organization,
and it played out as written: every copy of the regexp changed together, the issuer flag did not move, and
`SECURITY.md` led. It is worth reading as evidence rather than as prediction - the cost of the move was five
edits and one beta to prove them, because the copies were listed here before anyone needed the list.

**A third thing moved the identity on 2026-08-28, and it changed no command: the ref.** The identity ends
`release-docker-image.yml@<ref>`, and `<ref>` is whatever the run was dispatched on. Under the tag trigger that
was `refs/tags/v3.0.0`; the release is a `workflow_dispatch` from `main` now, so it is `refs/heads/main` — see
D1. **Every copy kept working unchanged**, because every one of them anchored at the `@` and constrained
nothing after it.

**That slack was closed on 2026-08-31, and the instruction it replaces was right about the wrong fix.** The
note here used to read *"the regexp must not be tightened"*, and its argument was that appending `refs/tags/`
would stop every image signed from 2026-08-28 onward from verifying. **That argument holds and is not what was
done.** The regexp now ends `@refs/heads/main$`. Anchoring on the branch keeps every image the surfaces
actually promise and drops the three that nobody was promised:

| | Under the anchored identity |
|---|---|
| `3.0.0` and later | passes - every release signs from `main`, because the release is a dispatch |
| `3.0.0-beta.5` | passes - the first release from `main`. Checked, not assumed |
| `3.0.0-beta.3`, `-beta.4` | **fail, and both still resolve.** The deletion recorded here on 2026-08-31 never happened - read the registry, not this row |
| `2.1.1` and earlier | unsigned, unchanged, still `no signatures found` |

**One published surface broke, found and fixed 2026-08-31.** The claim here was that every prerelease left
on Docker Hub signs from `main`. Betas 1 to 4 were never deleted, and `README.md:20` sent a reader to
`3.0.0-beta.4`, so the command in `SECURITY.md` failed on the one image the front page named. **The README now
names `3.0.0-beta.6`, which passes.** The betas that fail are still pullable and nothing points at them. `SECURITY.md` briefly carried a paragraph explaining how to check a tag-signed prerelease and it
was removed the same day - that removal was right for a different reason: nobody is asked to pull those
images.

**What the anchor buys.** A prefix accepts a signature from any ref in this repository, and a run on any ref
executes the workflow file **as it exists at that ref**. So anyone able to push a branch and start the workflow
could publish an image that passes the command we hand to users. **It needs write access, so it is not a way
in** - it is a way to make the documented check pass on something that is not a release. Dispatch alone does
not close it, because the dispatcher chooses the ref; the `$` is what closes it.

**The gate's *"Check the dispatch is on main"* step is not what closes it either, and it is worth saying so
before someone reads that step and removes the anchor.** The gate is a job inside the workflow file, so a run
on another ref executes that ref's copy of the file, gate included. It stops an accidental dispatch on a
branch. It cannot stop an edited one. **A check written in the thing being checked is not a control**; the
anchor sits in the verifier, which is the only place outside the attacker's reach.

**`just image verify` takes the ref and the repository as arguments**, defaulting to `refs/heads/main` and
`binacle/binacle-net`. **The ref is part of the identity, so it is an argument rather than a constant** - that
is what let the same recipe check images from either side of this change. Its first reason was that the old
betas stayed checkable, and that reason expired within a day when they were deleted; the shape is kept because
a constant would have to be edited to check anything else, and a recipe you edit to run is not a check.
**The repository argument is what the release workflow uses** to verify what it has just pushed, a scratch
repository included.

**The docs site's two copies are not done.** A coding session cannot write them. `plans/sites/docs-v3-deploy.md`
section 7 carries what they must say.

**Signing starts at `3.0.0-beta.2`**, along with the SBOM and the GHCR staging copy. Everything earlier
answers `no signatures found`, and that is history rather than a broken check. **Which images verify under
which identity is `$decisions#D3`** - the move split the signed images into two bands, and every example on
every surface has to name one that passes today.

**Keyless, so there is no key.** cosign exchanges the job's OIDC token for a short-lived certificate, which is
why both jobs need `id-token: write` and why this adds no secret to the repo. `sigstore/cosign-installer` comes
from the sigstore org itself rather than an individual, which is the standard this is adhering to in the first
place.

**Verified on 2026-08-11, not assumed.** A throwaway image built with both flags produced a single attestation
manifest carrying two in-toto layers — `https://spdx.dev/Document` and `https://slsa.dev/provenance/v1` — and a
cross-registry `imagetools create` of that index came out on the digest it went in with, attestations intact.

**Amended 2026-08-31 — a second provenance statement, and this one is signed.** `actions/attest-build-provenance`
runs in `build` against the pushed digest with `push-to-registry: true`. **Why, when buildkit already produces
provenance:** buildkit's is inlined in the index and is signed by nothing of its own. The cosign signature
covers it only because it covers the whole index, so a SLSA verifier has no signed provenance statement to
read and the build sits at SLSA build level 1. GitHub's statement is signed, which is level 2, and GitHub
says so outright. **It binds to the digest**, so the Docker Hub copy of that digest is covered without
attesting twice — unlike the cosign signature, which does not come across the copy. The action is a thin
wrapper over `actions/attest` and its own README points there for new work; the wrapper is used because its
name says what it produces and the generic one takes a predicate.

**Amended 2026-08-31 — `3.0` and `latest` do not move until the digest is signed and the signature checked.**
`publish` used to write all three tags in one `imagetools create` and sign afterwards. A failed sign left the
run red, no git tag and no GitHub release, all of which is correct — and `latest` already pointing at an
unsigned image on Docker Hub, with nothing published saying so. The copy is now two calls with the sign and
the verify between them. **Signing does fail often enough to plan for: Cilium wraps its `cosign sign` in a
retry with backoff**, which nobody writes otherwise. A guard fails the job if the version tag is missing from
metadata-action's list, because a silent mismatch there is what would send the moving tags out unsigned.

**Amended 2026-08-31 — the workflow runs the command the docs publish.** `just image verify <version>
signature refs/heads/main <repo>` runs in `publish` straight after the sign. **The recipe holds the only copy
of that invocation**, so `SECURITY.md` and the registry cannot drift apart without the release going red. The
repository is an argument so the check follows a scratch `DOCKERHUB_REPO` rather than checking the real one by
accident. Kyverno, Cilium and Flux all publish a verify command and none of them run it in their own release,
so this is ahead of the norm rather than catching up.

**The standing maintenance cost of keyless, recorded 2026-08-31.** The published command runs on other
people's machines, with their cosign. Sigstore's transparency log is moving to Rekor v2, generally available
2025-10-10, and only cosign 2.6.0+ or 3.0.1+ can verify entries in it. Rekor v1 runs in parallel and freezes
with a year's notice. **Sigstore's own advice is to update verification before signing**, which for this repo
means the version floor now in `SECURITY.md` matters before anything in the workflow does. Nothing to change
in the pipeline; `sigstore/cosign-installer` is pinned by SHA and Dependabot moves it.

**What this obliges.** Users have no way to verify what they are not told about. Publishing signed images
without a documented `cosign verify` invocation, including the certificate identity and OIDC issuer to match
against, is decoration. That page is owed and is not written yet.

### D16 — lychee is installed as a pinned binary, not through its own action

`.github/actions/install-lychee` curls the release and checks its SHA-256, and the workflow step is
`just check links <site>`. **`lycheeverse/lychee-action` exists and is maintained, and was still not used.**

**Why:** the action runs lychee itself from `args:` in YAML. The flags that decide what the check *is* —
`--offline`, `--root-dir`, `--config` — would then live in the workflow as well as in `tooling/check.just`, and
the two would drift. `just check links docs` on a laptop and the CI step would stop being the same check while
continuing to look like it. **This is D-nothing-new: it is the first convention in `$ci-cd`** — a step calls a
recipe — applied where the obvious answer pointed the other way.

**What it costs:** about twenty lines of curl and checksum, copied from `install-hurl`, and lychee's version
now lives in two places (that action and `DEVELOPMENT.md`) rather than being bumped by Dependabot. That is the
same trade already accepted for `hurl` and `container-structure-test`, and the same watch item applies.

**Where the action would win, if it is ever wanted:** a scheduled external run. `just check links-external` is
deliberately not a gate — it reports on other people's servers — and the useful shape for it is a monthly run
that opens an issue, which the action supports directly and a `run:` step does not. Different job, different
tool; that would be an addition, not a reversal of this.

**The check is `--offline` in CI, and that is not a preference either.** Every page carries a `canonical` and
an `og:url` pointing at where it *will* live. Run externally before the deploy, they 404 on every page the
deploy is about to create — 35 of 36 failures on the first real run, all of them self-references. A gate red
for that reason before anyone writes a line is a gate people learn to ignore.

### D17 — the site deploys are published by hand, and never on a push

**Decided by the maintainer, 2026-08-19, and it covers `deploy-www-site.yml` too, added after.**
`deploy-docs-site.yml`, `deploy-demo-site.yml` and `deploy-www-site.yml` are `workflow_dispatch` and stay that
way. No `push` trigger on `sites/**`, and no scheduled run.

**Why:** publishing to the internet is a deliberate act, not a side effect of a commit. Those folders are
written in their own session, and pressing the button is part of how that session ends — a merge that happens
to touch a page is not a decision to put it live.

**Two mechanical consequences that make the same point.** The marker tag is numbered by `github.run_number`, so
a push trigger would produce a tag per commit and the tag would stop meaning "this is live". And the
concurrency group is never cancelled — `cancel-in-progress: false`, because a stopped run leaves the site
deployed with no marker tag — so a busy branch would queue rollouts behind each other rather than skip to the
last.

**This closes the question the workflow restructure left open.** It was not CI's to answer.

### D18 — two test suites, split by what ships

**Decided by the maintainer, 2026-08-27.** `shared-image-tests.yml` runs the sixteen tests that end up in the
Docker image. `shared-site-tests.yml` runs the fifteen that end up in a Jekyll site. The release pipeline calls
the first; the three site deploys call the second; the pull request gate calls both.

**Why:** a release should run the tests for what it releases. The ten Jekyll plugins under `ruby/` cannot reach
the image, so a release paying for them buys nothing, and every step added to the suite the release calls is a
step every release pays for. The mirror was worse: the gems ran on no pipeline at all, so a broken plugin was
first seen half way through a deploy.

**The cut is one rule, not a judgement per test: the image gets the .NET tests plus the javascript tests, and
a site gets everything that is not .NET.** Checked against the manifests on 2026-08-27 — every site pulls all
five javascript packages, `demo` through `binacle-net-ui` to `binacle-vipaq` to `binacle-compact-notation`, all
three through `theme-switcher` to `cookies`.

**Five tests are in both files, and that is not duplication to remove.** They ship in the image and they ship
in the sites, so both sides prove them. 16 plus 15 less the 5 shared is 26, the whole list.

**The pull request gate's path filter overlaps for the same reason — recorded 2026-08-28.** `just ci
changed-paths` prints `code=` and `site=`, and `packages/`, `shared/` and `vipaq/` set both. That is right:
those directories ship in the image and in the sites, so a change to one has to prove both.

**A site deploy runs the site suite as its own first job**, for the same reason the release runs the image
suite: a deploy is dispatched from any branch, so nothing guarantees the commit passed the gate.

**Steps, never a group recipe, in either file.** `just test image` and `just test sites` exist so one command
runs a slice on a laptop; a red check has to name the suite, which a group recipe cannot do.
The lists and the steps are written out by hand and nothing checks one against the other. That is the
trade: a list you can read, against a test that runs on a laptop and never in CI if somebody forgets the
step. `just check test-steps` did the checking until 2026-08-27, when it was deleted — a check about the
contents of another file in this repository is not a check on anything real.

**A step's `name:` is the assembly, package or gem in full; its `run:` is the test derived from that name.**
Renamed on 2026-08-27. Before, a step said `Test - API Kernel (Unit)` and ran `just test api-kernel-unit`,
which was a third name for `Binacle.Net.Kernel.UnitTests` — three names for one suite, none of them derivable
from another. The test is now `cs_binacle-net-kernel_unit`, and the rule for building it is in the
`test-naming` memory. The tests are `[private]`, so a laptop's completion offers the three groups; a
private recipe still runs by name, which is all a step needs.

**What this does not fix.** The five javascript tests run twice on a pull request that touches
`packages/`. That is two jest runs of a few seconds each, against a suite in each file that is honest about
what it covers.

### D19 — the merged coverage report drops the test-support assemblies

`just coverage report` passes `-assemblyfilters:'-*.TestsKernel;-Binacle.TestReporting;-*.UnitTests;-*.IntegrationTests'`
to reportgenerator.

**Why:** a test helper scoring itself says nothing about shipped code. `*.TestsKernel` is a pattern rather
than three names, so adding a kernel needs no edit.

**Why `Binacle.TestReporting` and the ViPaq kernel are named even though neither is in the report today.**
They only reach it if a suite that runs under coverage starts referencing them. Naming them now means that
day is silent, instead of moving the denominator with nobody noticing.

### D20 — CodeQL runs buildless, on merge only, and reports nothing on a check

**Recorded 2026-08-28**, from the comments in `codeql-analysis.yml` that were the only copy of it. One job per
language: `actions`, `csharp`, `javascript-typescript`, `ruby`. Findings land in the repository's Security
tab.

**Every language runs `build-mode: none`**, so no job installs a toolchain. If C# extraction is ever found to
miss code, that one matrix entry becomes `manual` with a `dotnet build` step — **never `autobuild`**, which
guesses at a solution this repository builds through `just`.

**On `push` to `main`, not on `pull_request`.** The gate is the only required check and code scanning is
advisory. A second pull request check that never blocks trains people to ignore it.

**The schedule earns its place here, unlike Sonar's.** The query packs change, so the same commit reports new
findings weeks later. That is a reason a Sonar schedule does not have — see D8.

**`security-extended`, and quality queries stay off.** Sonar already reports quality, and two tools
disagreeing on the same line is how a finding stops being read.

**`category: /language:<name>` is load-bearing.** Without it, each upload replaces the last, so the job that
finishes last clears the other three languages' findings.

**The permissions are the smallest set that works.** `security-events: write` for the upload. GitHub's
template also lists `actions: read` and `packages: read`; both are for private repositories and private query
packs, and this one is public and uses neither.

**The `summary` job exists because no page in the run says how many alerts are open** once the matrix has
finished. It does not repeat the per-language results — the job list already shows those.

### D21 — `just image verify` is what a user runs, and it must stay that way

**Recorded 2026-08-28**, from the comments in `tooling/image.just` that were the only copy of it. Four checks
against a published image: tags, signature, attestations, metadata.

**No `docker login`, ever.** These are the commands a user runs against a public artifact, and a check that
only passes with a credential is not checking a public artifact. Nothing in this recipe may grow one.

**The version argument has no default.** A default rots into a tag nobody meant to check, and green against
last release is worse than no output.

**The order is deliberate: each check answers something the next one assumes.** Which tags are this image,
then whether it is signed, then what is attached to it, then what it says about itself.

**Every check prints what it found before it says pass or fail**, and no check aborts the others — the recipe
runs without `set -e` and OR-s the exit codes, so the first failure cannot hide the answers that explain it.
A check whose only output is "ok" cannot be read over someone's shoulder.

**Only `3.0.0` and later can pass**, per D15 — signing and the SBOM start there.

### D22 — the gems reach Sonar through a built project, and the project type is what decides that

`ruby/ruby.proj` exists for one reason: to list the 102 gem sources as `Content` so the scanner is offered
them. `Microsoft.Build.NoTargets`, so it compiles nothing. It excludes `vendor/**`, the bundle CI installs.

**Without it Sonar saw no Ruby at all.** The run of 2026-08-27 23:08 reported `10 languages detected` and no
`Sensor Ruby` line anywhere; the run after it, with the project, reported `11` and analysed 99 `.rb` files.
The scanner's own walk of the repository root reaches `ruby/` — it indexed 240 files under `ruby/vendor`
before that folder was excluded — so it sees the `.rb` files and does not claim them. Being listed in a built
project is what makes the difference.

**The extension is declared in `Binacle.Net.slnx`, so the project entry carries no `Type`:**

```xml
<ProjectType Name="Ruby" Extension="rbproj" BasedOn="C#" />
```

A `.slnx` rejects any project whose type it cannot infer — `ProjectType '' not found` makes the whole solution
unloadable — and it infers nothing from `.proj`. Declaring the extension answers that once. The mapping is
scoped to `rbproj`, so the five `Type="Shared"` content projects are untouched.

**It is based on `C#` and not on `Shared`, and `IsBuildable` does not bridge the two.** Measured on
2026-08-28, each from a deleted `ruby/obj` and a full `dotnet build Binacle.Net.slnx`:

| Declaration | Built by the solution |
|---|---|
| `.csproj`, no type | yes |
| `.proj`, `Type="C#"` | yes |
| `.rbproj`, `BasedOn="C#"` | yes |
| `.rbproj`, `BasedOn="Shared" IsBuildable="true"` | **no** |

`IsBuildable="true"` on a type based on `Shared` does not make it build. An unbuilt project never reaches the
scanner, which is this one's whole job — which is also why `tooling/obj` and `assets/obj` are stale from
6 Aug 2026 and none of those projects has ever been analysed.

**The consequence for the others is worth knowing.** `tooling.proj`, `assets.proj` and the three site
projects are all `Type="Shared"`, so none of them has ever been built or appeared as a Sonar module. Their
files are analysed anyway, through the scanner's root walk — which is why the `.sh` and `.py` findings exist
while the projects that list them do not.

**The type names are not documented, so they were tested.** Accepted: `C#`, `VB`, `F#`, `Website`, `Shared`,
`WiX`, `Docker`, `Folder`, `JavaScript`, and a raw GUID. Rejected: `Classic C#`, `MSBuild`, `Web`,
`Solution Folder`, `Python`, `Node`, `TypeScript`, `Custom`, `None`.

**The scanner does not care what the file is called.** Its targets pick files by MSBuild item type -
`SQAnalysisFileItemTypes` names `Content` among others - and the only projects it skips are Microsoft Fakes
projects, `_wpftmp`/`WorkerExtensions` temp projects, and anything setting `SonarQubeExclude`. Nothing keys
off the extension, and `ProjectLanguage` comes from `$(Language)`, which is empty for a NoTargets project
whatever it is named. So the extension is a readability choice, not a functional one.

**One knock-on: the ruby coverage path is the odd one out.** A report path is resolved against the base
directory of the module the sensor runs in. The C# and javascript sensors run in the root module; the
SimpleCov sensor runs in `ruby/`, this project's folder, and said so —
`SimpleCov report not found: 'artifacts/coverage/sonar/*.json'`. So that one property opens with `../` while
the other two do not. Every report is still written to `artifacts/coverage/sonar`.

### D23 — one required check, and the maintainer bypasses it

**Branch protection on `main` requires exactly one status check, `Gate`.** That is the job name, and the job
name is the whole context - not `Pull Request / Gate`. `pull-request.yml` says so above the job: *"`gate` is
the only name branch protection holds."* Every job under it can be renamed freely; this is the last
protection edit that should ever be needed.

**`gate` reports whatever happens.** It is `if: always()` with `needs:` on every other job, so a skipped half
still produces a verdict. A required check that can silently never report is what leaves a pull request
pending forever, and that shape is designed out rather than watched for.

**The repository-admin role bypasses it, deliberately.** `pull-request.yml` triggers on `pull_request` only.
A commit pushed straight to `main` therefore has no `Gate` check and never will - not a slow one, an absent
one. Without the bypass the maintainer's own push is rejected with nothing to wait for. **The bypass is not a
weakening of a check that was working; it is an admission the check was never going to run for that path.**

What the check actually binds is Dependabot, which opens up to ten pull requests per ecosystem across five of
them. External pull requests are closed (`CONTRIBUTING.md`), so there is nobody else to bind.

**The alternative, rejected for now:** add `push: branches: [main]` and drop the bypass. That makes the
requirement honest for every commit, and costs a full CI run on every push while the workflow stops being
about pull requests. **Revisit it when a second person can commit** - at that point the bypass is protecting
a habit rather than describing a gap.

**`strict_required_status_checks_policy` is off.** A pull request can merge without being up to date with
`main`. With one committer the alternative forces a rebase before every Dependabot merge and buys little.

### D24 — release tags cannot be moved or deleted, and nobody bypasses that

A tag ruleset on `refs/tags/v*` blocks **update** and **deletion**. It matches 46 tags, every release back
through v1 and v2. **Creation stays allowed**, or the release would break - `github-release.sh` makes the tag
itself.

**The bypass list is empty on purpose, including the maintainer.** A published image and a GitHub release
both point at a tag; moving one makes an artifact that has been public since January unverifiable, and there
is no undo. This is the only rule here guarding something irreversible, which is why it is the only one with
no way around it. Deleting a tag now means disabling the ruleset, deleting, and re-enabling - the friction is
the feature.

**`v*` and not everything.** The 14 deploy marker tags - `docs-6`, `web-release-5`, `www-1` - are created by
`push-tag.sh` on every site deploy and must stay free.


## Open

### O1 — a prerelease cannot test the publish step, and this got worse

**Mostly closed by the D3 reversal on 2026-08-11.** For part of that day `publish` was skipped entirely for a
prerelease, which meant the Docker Hub login, the copy and the release-side signature were all first exercised
by a real release. That is no longer true: `publish` runs for every release, so a beta proves the job's
credentials, its wiring, the cross-registry copy and the signature.

**What is still untested is narrower: the moving tags.** A prerelease produces only its immutable tag, so
`{{major}}.{{minor}}` and `latest=auto` firing correctly — and `imagetools create` being handed three
references instead of one — are first proven on the release itself. That is one extra argument to a command
that will have run several times by then.

**D1 fed this on 2026-08-28.** Both metadata steps take the version from `value=${{ inputs.version }}` now,
because a dispatch has no tag ref for `type=semver` to read, and that is the input to exactly the two tag rules
nothing has proven yet.

Whether that residual deserves a throwaway run is a judgement call rather than an obvious yes. If it is
done, note the two traps: a version containing a hyphen is treated as a prerelease and proves nothing, and a
clean `0.0.1` against the real repo **would move `latest`**, because metadata-action never queries the registry
and `latest=auto` marks any non-prerelease semver as latest. Point `DOCKERHUB_REPO` at a scratch repo instead.

### O2 — how much the pull-request gate should prove

A PR now runs both test suites, an image build, the three site builds with their link checks, and the
`.github/` lints. What is still missing: the integration suites cover core modules only, and Sonar runs when
somebody presses a button. Both are known gaps rather than oversights, and the shape of the fix is not settled
— one folded job or three workflows, and what the runtime budget allows.

**Whether coverage becomes a blocking check is open, and it is the maintainer's call.** The project runs the
read-only "Sonar way" gate, which asks 80% on new code; custom gates need a paid plan. This was settled
against until 2026-08-31, on one argument: the condition was red before anyone wrote a line, so it would block
every pull request for a reason none of them caused and be waived within a week. **The condition passes on
`main` as of 2026-08-31**, so that argument is spent. **The shortcut stays rejected** — excluding the untested
areas was considered and turned down, because it moves the number without changing anything true.

**This is the one place the coverage numbers are recorded.** They moved a long way in two weeks and were
being re-derived in four files, which is how three of them ended up disagreeing.

| Measured | Overall | The four areas that were at 0% |
|---|---|---|
| 2026-08-08, from Sonar | 53.3%, 31.4% on new code | the Blazor UI module and three TypeScript packages — 1571 lines, 22.5% of the denominator |
| 2026-08-22, local cobertura | 56.6% — 9511 of 16785 lines, 20 assemblies | all four now have suites; see the table below |
| 2026-08-27, from Sonar — run `ad2e96b8` | 71.1%, 70.6% line — 1892 uncovered of 6429. New code reads **77.0%** against the 80% gate | none. All four suites reached Sonar, which is what the UI harness was waiting to see |
| 2026-08-31, from Sonar | **over 80% on new code — the fixed gate passes on `main`.** The exact percentage was not read; the maintainer confirmed the crossing. Four ServiceModule repositories left 0% because the Sonar workflow ran Sqlite only, and it now starts azurite and postgres | the three below |

**The 2026-08-22 detail**, hand-written code only:

| Area | Lines | Covered | |
|---|---|---|---|
| `api/src/Binacle.Net.UIModule` (C#) | 232 | 216 | 93.1% |
| `packages/binacle-net-ui/src` | 631 | 440 | 69.7% |
| `packages/cookies/src` | 48 | 47 | 97.9% |
| `packages/theme-switcher/src` | 40 | 39 | 97.5% |

**Two caveats on the first row, and both matter to whoever sets a floor.** The assembly reports **35.4%**,
because two generated namespaces land inside it — `Microsoft.AspNetCore.OpenApi.Generated` and
`System.Runtime.CompilerServices`, both at 0% and neither written by anyone here. And 216 of those 232 lines
need both UIModule suites together; the unit one alone does not reach `ModuleDefinition`.

**The old first row counted 959 lines of Blazor that no longer exist.** What replaced it is a tenth of the
size and nearly covered.

**The middle row is local cobertura, the outer two are Sonar's**, and Sonar counts coverable lines its own
way — the shape held and the digits moved, which is why both are kept.

**334 lines came out of the denominator through `sonar.coverage.exclusions` before that run**, and none of
them was ever coverable: the python index generator, the three sites' bundles and webpack configs, and the
typescript fixture-provider and generator folders. That alone moved 67.1% to 70.6% with no test written.
**It is not the same act as excluding untested code**, which stays rejected above.

**What is left uncovered on purpose**, so it is not re-opened as a gap:

- `Kernel/Logs/LogsRetentionProcessor` — the `catch` around `File.Delete`, because there is no portable way to
  force a delete to fail (Linux unlinks open files, Windows does not), and the second turn of the retention
  loop, because the `PeriodicTimer` is built with no `TimeProvider` so the day between sweeps cannot be faked.
  Passing a `TimeProvider` in would make the second one testable, and that is a change to the code.
- `InMemoryAccountRepository`, `InMemorySubscriptionRepository` and `FileHashStore` sit at 0%. The two
  repositories hold their `ConcurrentSortedDictionary` in a static field, so state is shared across the whole
  process and a test that writes to one has to account for every other test that did.

**Still uncovered and not on purpose:** OpenApi document generation, around 130 lines —
`Kernel/OpenApi/ExtensionsMethods/OpenApiOptionsExtensions.cs` and `OpenApiServiceCollectionExtensions.cs`,
`Kernel/OpenApi/Helpers/OpenApiValidationProblemExample.cs`, and `Binacle.Net/v3/ApiV3Document.cs` /
`v4/ApiV4Document.cs` with their example-response classes. Those last need a host with the document endpoint
mapped, which is gated on `SWAGGER_UI` or `SCALAR_UI`, so they wait on the integration-harness question. The
transformers under `Kernel/OpenApi/Transformers/` are at 100% from `api/test/Binacle.Net.Kernel.UnitTests/OpenApi/`,
against hand-built transformer contexts and no host.

**`binacle-net-ui`'s uncovered third is the Three.js half** — `core/packingVisualizer.ts` and the scene
helpers in `utils/`. They need a WebGL context, so a test there could only assert that a call happened. They
stay in the denominator: excluding them would be the same act as the Sonar coverage exclusions rejected
above, one layer down. The only exclusion anywhere is `.d.ts`, which carries no runtime code.

### O3 — multi-arch is still absent

`linux/amd64` only. No second architecture is built, and nothing asks for one yet.

It stays out because it **changes the artifact** and roughly doubles build time, and because there is no
evidence of demand. Attestation and signing, which used to share this entry, are now done — see D15.
