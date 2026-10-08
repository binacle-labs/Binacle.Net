---
id: ci-cd/decisions/D1
description: a release is dispatched with a version, and the tag and the release are its last job
status: pending
verified: 2026-09-29
check: D1 against release-docker-image.yml, whose trigger must be workflow_dispatch alone with a required version input and whose gate job must carry the ref, semver and tag checks
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/ci/check-release-ref.sh"
  - "tooling/ci/check-version.sh"
  - "tooling/ci/check-release-tag.sh"
  - "tooling/ci/github-release.sh"
---

# D1 — a release is dispatched with a version, and the tag and the release are its last job

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

**The version is checked before anything is built.** The gate job proves the dispatch is on a ref the
version may run from - `main` for a release, `main` or `release/*` for a prerelease since `$ci-cd/decisions/D3` - that the version is
semver shaped with no leading `v`, and that `v<version>` does not already exist on some other commit. The semver check is also what keeps the input out of a shell whose meaning it could change; every
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
this workflow. The deploy marker tags are made by `create-tag.sh`, `$ci-cd/decisions/D30`. `--target` is ignored when the tag already
exists, so the load-bearing exception above — a re-dispatch on a run whose tag is already there — still works
exactly as written.

**Amended 2026-08-31 — `run-name` carries the version, because the run's ref cannot.** A dispatch is listed
against `main`, since the tag does not exist until the last job, so the Actions list showed the commit message
and nothing said which release a run was. `run-name: Release v${{ inputs.version }}` puts it in the list. The
branch chip still reads `main` and that is not fixable without dispatching on a tag - the ref the rebuild
moved off, for the reasons at the top of this entry. **Nothing verifies against the run name** — the version that matters is the `version` input, which
the gate checks and every later job reads.

**The GitHub web release route stops working, and that is accepted.** *Draft a new release → create tag on
publish* now builds nothing, silently. The release job's create-or-edit branch is left in place: it costs
nothing and still covers a tag made by hand.

**No `dry_run` input.** The other repository that releases this way has one, because its release commits to
`main` and there is no cheaper rehearsal. Here a prerelease is the rehearsal: it runs `gate`, `test`, `build`,
`smoke` and `release` and skips the Docker Hub jobs (`$ci-cd/decisions/D3`), and a dry run would duplicate that. What a
prerelease does not rehearse is `publish` and `page` - a real release is their first run, and a red there
leaves Docker Hub untouched and no tag, so the same version can be dispatched again.

**Superseded 2026-08-28 — what changed and what did not.** This ran on `on: push: tags: 'v[0-9]*'` from the
rebuild until then. Everything above about `release: published` and about creating the release last is the
original reasoning and is untouched; only the entry point moved. Two things went with it. The pattern was
`v[0-9]*` rather than `v*` because the three site deploys push `docs-<run>`, `demo-<run>` and `www-<run>` and
a release build must never fire on one — the release fires on no tag at all now, so that constraint on their
tag names is gone. And the certificate identity inside every signature changed with the ref; that is recorded
under `$ci-cd/decisions/D15`, where the verify commands are listed.
