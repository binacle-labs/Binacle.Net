---
id: ci-cd/decisions/D21
description: just image verify is what a user runs, and it must stay that way
status: pending
verified: 2026-09-29
check: D21 against tooling/image.just and tooling/image/verify*.sh, whose verify recipe must take a version with no default and whose scripts reach no registry that needs a login
paths:
  - "tooling/image.just"
  - "tooling/image/**"
---

# D21 — `just image verify` is what a user runs, and it must stay that way

**Recorded 2026-08-28**, from the comments in `tooling/image.just` that were the only copy of it. Four checks
against a published image: tags, signature, attestations, metadata. **Scripts since 2026-09-14**, one per
check under `tooling/image/`, for the reason `$ci-cd/decisions/D4` gives - a recipe body can be neither run alone nor
shellchecked - and `image.just` is a door the way `ci.just` is.

**It runs against GHCR too - added 2026-09-14, the day a prerelease's image started stopping there (`$ci-cd/decisions/D3`).** A beta is
an image a person is asked to try, so it gets the same command a release does; the repository is the fourth
argument. Three checks were registry-neutral already. `tags` was Docker Hub's web API and gained a GHCR path
through the OCI registry API - anonymous pull token, tag list, a HEAD per tag - which keeps the no-login rule
below because the package is public (`$ci-cd/decisions/D14`). Proven the same day: all four checks pass on
`ghcr.io/binacle-labs/binacle-net:3.0.0-beta.8`, and `3.0.0` there lists `3.0.0` and `latest` on the release
digest `974f3dda3923`, the one Docker Hub serves.

**No `docker login`, ever.** These are the commands a user runs against a public artifact, and a check that
only passes with a credential is not checking a public artifact. Nothing in this recipe may grow one.

**The version argument has no default.** A default rots into a tag nobody meant to check, and green against
last release is worse than no output.

**The order is deliberate: each check answers something the next one assumes.** Which tags are this image,
then whether it is signed, then what is attached to it, then what it says about itself.

**Every check prints what it found before it says pass or fail**, and no check aborts the others — the recipe
runs without `set -e` and OR-s the exit codes, so the first failure cannot hide the answers that explain it.
A check whose only output is "ok" cannot be read over someone's shoulder.

**Nothing older than `3.0.0-beta.5` can pass**, per `$ci-cd/decisions/D15` - earlier images are unsigned or
signed under a tag ref.
