---
id: ci-cd/decisions/D27
description: a released version is never deleted, and prereleases stop reaching the public repository
status: pending
verified: 2026-09-29
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/image/verify-tags.sh"
  - ".github/dockerhub-overview.md"
---

# D27 — a released version is never deleted, and prereleases stop reaching the public repository

**A released version stays, whatever happens to its line.**
`.github/dockerhub-overview.md` tells a reader that an exact version never moves, which they read as a promise
that it is there tomorrow. Deleting one breaks the user who took that advice and pinned. A line that is no
longer patched is retired in words, not by removing the images.

**The eight `3.0.0-beta.*` tags were deleted by hand on 2026-09-05** - read off the registry the same day, no
beta in the tag list and `docker buildx imagetools inspect` failing on `beta.1`, `beta.5` and `beta.8`. Two
reasons they went: betas 1 to 4 fail the published verify command, so anyone following `SECURITY.md` against
one sees what reads as tampering, and a test build kept forever is a second answer to "which image do I pull".

**That was a one-off cleanup, not a policy - and since 2026-09-14 the policy exists.** A prerelease's image
stops at GHCR (`$ci-cd/decisions/D3`), so nothing ever lands in `binacle/binacle-net` that is not a release, and there is nothing to
delete afterwards. A second public Docker Hub repository for prereleases was the direction until then; GHCR
already does that job, is public, and needs no new credential, so it is the staging repository. **Nothing
about prerelease lifetime goes on the Docker Hub page** - the page describes the repository users pull from,
and prereleases are no longer in it. **Old prerelease tags on GHCR stay.** The maintainer, 2026-09-14:
"as for the beta tags agreed as long as it is documented thats ok" - what it answered is not on record.
Nothing deletes a staging image.

**Docker Hub has no lifecycle rules**, so no cleanup happens on its own. Deleting is the Hub API,
`DELETE /v2/repositories/{repo}/tags/{tag}/` with a JWT - the sibling of the tag list `tooling/image/verify-tags.sh`
already reads. The registry delete verb is not accepted.

**This is a second reason the `$ci-cd/decisions/D26` rule would have to be corrected before immutability is ever enabled.** An
immutable tag cannot be deleted, so a rule of `.*` freezes any prerelease that does reach the repository.
