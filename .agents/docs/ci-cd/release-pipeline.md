---
id: ci-cd/release-pipeline
description: "The release pipeline in release-docker-image.yml — seven jobs from a dispatched version to a published GitHub release and the git tag it creates last, GHCR as the staging registry, the copy-to-Docker-Hub step every release reaches with a prerelease narrowed to its immutable tag, the CHANGELOG.md release body, and the Docker Hub page written last"
verified: 2026-08-28
check: "The trigger is workflow_dispatch alone with a required version input; the seven jobs, their needs: edges and job outputs match release-docker-image.yml; the gate job still carries the ref, semver and tag checks before the changelog one; the tag is pushed in the release job before gh release create; the concurrency block groups on github.workflow alone and still sets cancel-in-progress: false; `page` is still the only job carrying a prerelease condition and still the only one nothing needs, so no job above it is conditional; shared-image-tests.yml, shared-smoke-image.yml and shared-dockerhub-overview.yml still expose workflow_call; shared-image-tests.yml still names no gem test; `just changelog check` and `extract` still take a bare version or Unreleased"
also_update:
  - ci-cd
  - tooling
paths:
  - ".github/workflows/**"
---

# The release pipeline

`release-docker-image.yml`. One dispatch, with the version typed into it, produces a smoked image on GHCR, a
copy of that exact image on Docker Hub, a git tag and a GitHub release. The order is cheapest check first, so
nothing that cannot be undone happens until the things that can be checked cheaply have passed.

**The tag is made last, not first.** It is an output of a successful run rather than its trigger, so a run that
goes red leaves nothing at all behind — no tag, no release, nothing to delete before the version can be tried
again.

## The flow

```
Actions -> Build and Release Docker Image -> Run workflow -> version: 3.0.0
  |
  v  on: workflow_dispatch
gate      on main, semver, the tag is free, and the CHANGELOG.md section exists   (seconds)
test      every test the image ships plus the OpenAPI lint, by calling shared-image-tests.yml  (minutes)
build     just build publish, push the immutable tag to GHCR, capture the digest
smoke     pull that digest back from GHCR, structure check + all five profiles
publish   imagetools copy to Docker Hub - a prerelease gets its immutable tag only
release   git tag v3.0.0 on this run's commit, then gh release create, body from CHANGELOG.md
page      render .github/dockerhub-overview.md and write it to the Docker Hub page  (real releases only)
```

Each job `needs:` the ones before it, so a red anywhere leaves Docker Hub untouched and creates no release.
**`page` is the exception at the end**: nothing needs it, so it can go red on its own without holding anything
back, and it is skipped entirely for a prerelease.

**A release run is never cancelled.** The workflow declares `concurrency` grouped on
`${{ github.workflow }}` with **`cancel-in-progress: false`**. A stop between `build` and `publish` would
strand a staged image on GHCR or half-move the Docker Hub tags, so a second run queues behind the first rather
than replacing it. **The group is the workflow alone**: every dispatch is on `main`, so adding the ref would
split nothing, and two versions racing for `latest` is exactly what the group exists to prevent.

## The rule the shape exists to enforce

**Nothing unsmoked reaches Docker Hub.**

That comes from job ordering, not from the registry split: `smoke` runs against the staging copy and only a
digest that passed is ever copied across. GHCR is staging; Docker Hub is what users pull, and it carries every
tag the pipeline publishes — betas included, with the immutable tag only.

## The seven jobs

**`gate`** — checkout, `just`, then four checks, each its own step with its own failure message. All of them
cost seconds and everything waits on them; the alternative is finding out at the end, with the image already on
Docker Hub and `latest` already moved.

| Step | Passes when |
|---|---|
| Check the dispatch is on main | `github.ref` is `refs/heads/main` |
| Check the version is semver | `3.0.0` or `3.0.0-beta.1` — no leading `v`, no `+` build metadata, which a Docker tag cannot carry |
| Check the tag is free | `v<version>` exists on neither the local clone nor origin, **or already points at this run's commit** |
| Check the section exists | `just changelog check <section>` finds it and it is not empty |

**The tag check's exception is load-bearing.** A run that publishes the image and then fails at the release
step has to be dispatchable again; without it, the only way out would be deleting a tag.

**A fifth step sits between the tag check and the section check, and it is what names the section.** A
version containing a hyphen publishes `Unreleased`; any other publishes its own version. Job output:
`section`.

**`test`** — `uses: ./.github/workflows/shared-image-tests.yml`, no inputs. Nothing guarantees the commit
being released passed CI — a direct push to `main` never sees the pull request gate; this is that guarantee. It
runs after the gate job rather than beside it, so a bad version or a missing section is reported in seconds
instead of after a full suite. It takes that file whole, so the release also gets its OpenAPI lint step.

**The gems are not in it.** The ten Jekyll plugins under `ruby/` ship in the three sites and never in the
image, so they run in `shared-site-tests.yml`, which the three deploys call and this pipeline does not. Every
step added to the image suite is a step every release pays for — see `$ci-cd/decisions#D18`.

**`build`** — checkout, .NET, `just`, then `just build publish`. One `docker/metadata-action` step, a GHCR
login with `GITHUB_TOKEN`, buildx, and one `docker/build-push-action` that pushes the immutable tag to GHCR
with `provenance: mode=max` and `sbom: true`. `VERSION` is passed as a build arg from the metadata step's
`version` output rather than from the input, so `BINACLE_VERSION` inside the container and the image tag cannot
disagree. It ends by signing the pushed digest with cosign.

**Both metadata steps — this one and `publish`'s — carry `value=${{ inputs.version }}` on every `type=semver`
line.** That rule takes its version from the tag ref, and a dispatch has no tag ref, so without `value=` both
steps emit no tags at all.

Job outputs: `staging` (the full `ghcr.io/...:tag` the smoke job pulls), `version` and `digest`.

**`smoke`** — `uses: ./.github/workflows/shared-smoke-image.yml` with the `staging` output. It calls the same workflow
a maintainer runs by hand, rather than copying its steps, so the release path and a manual check are the same
thing. See `$ci-cd` for that workflow's runner pin.

**`publish`** — the only job that touches Docker Hub and the only place the stored Docker Hub credential is
used. A `metadata-action` step computes the public tag set, then one `docker buildx imagetools create` moves
the manifest **by digest** from GHCR under all three public tags at once, and cosign signs the result. It never
checks out, so it holds no `contents` permission. Job output: `tags`, the public tag set, read by `release` for
the run summary.

**`release`** — checkout, `just`, then one call that makes the tag and the release together, with the body from
`just changelog extract <section>`. It `needs:` the `gate` job because it reads that job's `section` output, and
the prerelease flag is set explicitly either way from whether the tag contains a hyphen.

**The tag is made by the release, not pushed separately.** `gh release create` creates a missing tag itself, and
`--target <commit>` says which commit to put it on. The job passes `github.sha` — the commit the button was
pressed against, which is also what every other job checked out — so the tag and the image cannot describe
different code. `main` may have moved on by now and the tag ignores that. Because it is one API call there is no
window at all in which the image is public and the tag is missing.

**It also writes the run summary** — version, digest, every public tag, the release link and the verify
command. This job rather than `publish` because the release URL does not exist until the release step has run.

**It creates the release, or edits one that already exists.** The dispatch route normally finds nothing there.
The edit branch is kept for a tag made by hand: GitHub's web UI cannot create a bare tag — the only way to tag
from the site is to publish a release, which makes both at once. A plain `gh release create` would then fail
after every other job had succeeded, leaving the image published and one red job. Editing instead means the body
comes from `CHANGELOG.md` whichever way the tag was made, and a release marked prerelease by hand is corrected
for a real version. `--target` is ignored when the tag already exists, so a re-dispatch after a half-finished
run still works.

**`page`** — `uses: ./.github/workflows/shared-dockerhub-overview.yml` with the `build` job's `version`, and the
two Docker Hub secrets passed by name. That workflow renders `.github/dockerhub-overview.md` through
`just image dockerhub-overview <version>` and PATCHes the result onto the repository page.

**Last, and nothing needs it.** Everything irreversible has already happened by the time it runs, so a page
that failed to update is one red job on a release that shipped a correct image — never a release held up by a
paragraph.

**The one job with a prerelease condition**, `if: ${{ !contains(inputs.version, '-') }}`. The page describes
the stable line, and a beta moves neither the minor tag nor `latest`, so every tag the page names would be one
a beta did not create. Because nothing needs this job, the condition skips it alone and nothing downstream.

## Copy, never rebuild

`imagetools create` transfers a manifest, and a manifest is content-addressed, so the digest is preserved:
what Docker Hub serves is bit for bit what the smoke job passed. The copy source is the digest rather than the
tag, so the guarantee holds even if something re-tagged staging in between. All three tags go in one command,
because they are aliases of one manifest and the blobs need moving only once.

A second build in the publish job would ship an image nothing tested, however identical the inputs looked.

Smoking the registry copy rather than a locally loaded image is the point of the shape: compression, manifest
shape and attestation handling are exactly what a registry round trip changes.

## What ships alongside the image

The pushed artifact is an OCI **image index**, not a single manifest: the `linux/amd64` manifest plus an
`unknown/unknown` attestation manifest carrying two in-toto documents.

| | Predicate | Produced by |
|---|---|---|
| SBOM | `https://spdx.dev/Document` | `sbom: true` on `build-push-action` |
| Provenance | `https://slsa.dev/provenance/v1` | `provenance: mode=max` on the same step |

Both are manifests inside the index, so they travel with the copy to Docker Hub.

**The cosign signature does not.** It is a separate manifest rather than a child of the index, so the image is
signed twice — once on GHCR in `build`, once on Docker Hub in `publish`.

Specifically, cosign attaches it as an **OCI 1.1 referrer**: a manifest whose `subject` points at the index
digest, carrying one layer of `artifactType`
`application/vnd.dev.sigstore.bundle.v0.3+json`, discoverable through the registry's referrers API and
addressable by the fallback tag `sha256-<digest>` — **no `.sig` suffix**. Observed on
`v3.0.0-beta.2`, 2026-08-11. The older cosign scheme put signatures in a `sha256-<digest>.sig` tag instead;
this repo does not use it, so do not go looking for one.

**The two registries expose it differently, and one of them looks broken.** Docker Hub serves the signature
through the referrers API; **GHCR answers `/v2/.../referrers/<digest>` with a 404**, so a referrers query there
returns nothing at all. The signature is present either way — on GHCR it is visible in the tag list as
`sha256-<digest>`, and `cosign verify` passes against both, checked on the published `3.0.0-beta.2`
(2026-08-13). An empty referrers response from GHCR is not evidence of a missing signature; only a failed
`cosign verify` is.

Either way the point stands: a referrer is not inside the index, so `imagetools create` does not carry it, and
the published image must be signed where it lands.

Signing is keyless — cosign exchanges the job's OIDC token for a short-lived certificate, which is why both
jobs declare `id-token: write` and why no signing key exists to store. The signature is made against the
**digest**, so one signature covers `x.y.z`, `x.y` and `latest` alike.

What that certificate's identity names, and why tightening the regexp users verify with would break every
published command at once, is `$ci-cd/decisions#D15`.

## How a prerelease differs

A hyphen in the version is the prerelease marker. Every job runs either way; what changes is the tag set.

| | `3.0.0` | `3.0.0-beta.3` |
|---|---|---|
| Section the `gate` job checks | `3.0.0` | `Unreleased` |
| Pushed to GHCR | `3.0.0` | `3.0.0-beta.3` |
| `publish` job | runs | runs |
| Docker Hub tags | `3.0.0`, `3.0`, `latest` | `3.0.0-beta.3` only |
| Git tag created | `v3.0.0` | `v3.0.0-beta.3` |
| GitHub release | normal | marked `--prerelease` |
| `page` job | runs | **skipped** |

**One job is conditional, and it is the last one.** For the six that build and publish, the narrowing is
entirely `metadata-action`'s: it withholds `{{major}}.{{minor}}` and `latest` for a prerelease, so a beta can
never move a tag anyone is following. `page` is the exception — it does not go through `metadata-action`, so
its skip is a job condition. That is safe only because nothing needs it; a condition on any job above would
skip everything downstream of it.

**The consequence for testing:** a prerelease now exercises every job, `publish` included. What it still does
not cover is the *moving-tag* half — creating `3.0` and `latest` — since a beta produces neither. That is one
extra argument to the same `imagetools create` call, so the residual gap is much smaller than it was, but it
is not nothing: `latest=auto` firing correctly is first proven on a real release, and it is fed by the
`value=` on the semver rules. `DOCKERHUB_REPO` pointed at a scratch repo is how that is rehearsed without
moving a tag anyone follows.

## Where the release body comes from

`CHANGELOG.md` at the repo root, newest version first, Keep a Changelog shape. One section accumulates per
cycle: betas publish `## [Unreleased]`, and renaming that heading to the version is the last edit before the
real release.

The parsing lives in `tooling/changelog.just`, not in the workflow, so CI and a laptop read the file the same
way and the exact body can be previewed before the release is dispatched. See `$tooling` for the module.

Inside the file a release is `##` and its own sections are `###`, nesting under the single `# Changelog`.
`just changelog extract` shifts each section so its shallowest heading returns to `##`, because a release body
has no such parent. Relative depth inside the body is preserved and nothing has to be recorded anywhere.

A real release whose section is missing fails the `gate` job. There is no fallback to generated notes — that
would silently publish a commit list as the release body.

## Labels

Three sources, and they do not collide.

- **Constant labels are `LABEL` lines in the `Dockerfile`** — title, description, source, url, documentation,
  vendor, licenses, base.name.
- **Per-build labels are applied at build time**, never as `LABEL` fed by `ARG`: version, revision and created
  change every build, so as Dockerfile layers they would bust the cache from that point down. `--label` sets
  image-config metadata with no layer.
- **The `build` job's metadata step overrides two** that metadata-action gets wrong on its own: `licenses`,
  because auto-detection returns `NOASSERTION` for a dual-licensed repo, and `url`, which should be the landing
  site rather than the repo.

`tooling/build.just` does the same three per-build labels for a local `just build image`, so a locally built
image carries the same metadata shape a pushed one does.

## What still happens by hand

- **Deciding the version and pressing the button.** *Actions → Build and Release Docker Image → Run workflow*,
  on `main`, with the version typed in and no leading `v`. That is the only entry point.
- **Writing the `[Unreleased]` section of `CHANGELOG.md`** as the work lands, and renaming that heading to the
  version before the real release.
- **The moving-tag check on a scratch repository.** A prerelease reaches the `publish` job but produces only
  its immutable tag, so `3.0` and `latest` are first created on a real release.
- **Deploying the docs site**, which is its own `workflow_dispatch` workflow and is not chained to a release.

**What no longer happens by hand: the tag.** `git tag v3.0.0 && git push origin v3.0.0` builds nothing now, and
neither does *Releases → Draft a new release → Create new tag on publish* — that route makes a tag and a
release and no image, silently. A tag made either way is not an error to clean up; it just is not a release.
