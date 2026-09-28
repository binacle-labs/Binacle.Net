---
id: ci-cd/decisions/D14
description: GHCR is staging, and only the release workflow touches it
status: pending
verified: 2026-09-29
check: D2/D3/D14 against release-docker-image.yml's publish job, which must carry `if: ${{ !contains(inputs.version, '-') }}`, and its release job, which must carry `if: ${{ !cancelled() && !contains(needs.*.result, 'failure') }}` and keep `publish` in its needs, with no condition on `page`; D14's STAGING_IMAGE against release-docker-image.yml
paths:
  - ".github/workflows/release-docker-image.yml"
  - ".github/workflows/shared-smoke-image.yml"
---

# D14 — GHCR is staging, and only the release workflow touches it

Everything built lands on `ghcr.io/binacle-labs/binacle-net` first. Docker Hub receives only what has been
smoked there.

**Why a second registry at all.** It buys the property `$ci-cd/decisions/D2` used to trade away: nothing unsmoked is ever visible
where users pull from - `smoke` runs against the staging copy, and only a smoked digest is ever copied across. **GHCR is staging; Docker Hub is what users pull, and it carries released
versions only - a prerelease stays on GHCR, `$ci-cd/decisions/D3`.**

**Only the release workflow writes GHCR - written 2026-08-15 as "touches", narrowed to "writes" on
2026-09-14.** The staging registry exists so the workflow can push an image, smoke it and copy the smoked
digest to Docker Hub. **No public surface names it and no deployment pulls from it. One image, one place anyone gets it from.** The
one reader outside the workflow is the person checking a beta, who pulls it from GHCR by hand and checks it with
`just image verify`, because since `$ci-cd/decisions/D3` a prerelease exists nowhere else.

**What that changed, on the day it was written.**

- `SECURITY.md` and `CHANGELOG.md` stopped naming it. The docs-site verification page is written the same way
  at the next deploy.
- **`just image verify` lost its `digest` check and was Docker Hub only** - until 2026-09-14, when it took
  the repository as an argument so a beta can be checked on GHCR (`$ci-cd/decisions/D21`). That check compared the tag across
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

**The consumer-side argument for a public package is back.** It was that a person could pull a beta from
GHCR with no `docker login`. It was spent between 2026-08-11 and 2026-09-14, while every beta reached Docker
Hub; since `$ci-cd/decisions/D3` a beta exists on GHCR only, so the package being public is what makes a beta checkable at all.

**Why GHCR specifically.** `GITHUB_TOKEN` is minted per run and expires with it, so staging needs no stored
credential and nothing to rotate. Keeping Docker Hub free of anything unsmoked or unreleased is what the second
registry buys, and it does that without adding a secret anywhere.

**The package is public, and that is wanted** - the maintainer, 2026-09-14: "and ghcr public yes thats
wanted". Measured the same day:
an anonymous token from `ghcr.io/token` lists the tags and pulls the `3.0.0-beta.8` manifest with a 200. This
entry said the package was private from the move on 2026-08-16 and that nothing minded; the pipeline still
does not - `build`, `publish` and `shared-smoke-image.yml` all log in with `GITHUB_TOKEN`. **Since `$ci-cd/decisions/D3` public
is load-bearing**: a beta exists on GHCR only and is pulled from there by the person checking it, with no
login. Do not flip it private. The tag list also still holds `3.0.0-beta.3` to `-beta.8`, `3.0.0` and a `latest`
that nothing in the current workflow writes; none of it is named anywhere.

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
