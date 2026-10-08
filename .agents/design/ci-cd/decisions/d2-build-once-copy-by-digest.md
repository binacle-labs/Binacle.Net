---
id: ci-cd/decisions/D2
description: build once, smoke the registry copy, then copy by digest
status: pending
verified: 2026-09-29
check: D2/D3/D14 against release-docker-image.yml's publish job, which must carry `if: ${{ !contains(inputs.version, '-') }}`, and its release job, which must carry `if: ${{ !cancelled() && !contains(needs.*.result, 'failure') }}` and keep `publish` in its needs, with no condition on `page`
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/ci/copy-tags.sh"
---

# D2 — build once, smoke the registry copy, then copy by digest

Order: push the immutable tag to GHCR alone, pull it back from there and smoke it, then copy that digest to
Docker Hub with `docker buildx imagetools create` - the version tag first, the moving tags only after the sign
and the verify (`$ci-cd/decisions/D15`).

**Why not the tempting alternative.** Building with `load: true`, smoking the local image, then pushing sounds
equivalent and is not — it tests a local copy that *ought to be* identical to what lands in the registry.
Compression, manifest shape and attestation handling are precisely what a registry round trip changes, so the
only honest smoke target is what the registry actually serves.

**Promotion is a transfer, not a rebuild.** A manifest is content-addressed, so `imagetools create` preserves
the digest: Docker Hub serves the exact bytes that passed, not a rebuild that ought to match. The copy source
is the digest rather than the tag, so that holds even if something re-tagged staging in between. The moving tags
are aliases of the same manifest, so their copy moves no blobs.

**Verified across registries on 2026-08-11, not assumed.** The published `v3.0.0-beta.1` index - an amd64
manifest plus its attestation manifest - was copied by digest from Docker Hub into a scratch registry. It came
out on `sha256:c458644...`, the digest it went in with, and all three tags resolved to it. The attestation
entry survived the copy.

**Superseded 2026-08-11 — what changed and what did not.** This decision originally ran entirely on Docker Hub:
the immutable tag was pushed there, smoked there, and `docker buildx imagetools create` re-pointed `3.0` and
`latest`. The reasoning above survived intact; only the registry topology changed. What forced it was the one
cost the old shape accepted and should not have — an unsmoked artifact was briefly public on the registry users
pull from. It was argued as acceptable because nobody follows an exact pin on release day, and that is true, but
"true for the tag nobody watches" is a weaker claim than "never happens", and `$ci-cd/decisions/D14` makes it never happen for
free. The copy command did not change - `imagetools create` handles a cross-registry source as readily as a
local one, which is what kept a third-party tool out of the job that moves the artifact users pull.

**The copy needs no builder, so `publish` has no `docker/setup-buildx-action`.** `imagetools create` is a
registry operation: its sources must already exist in the registry, and it reads auth from the docker config the
login action writes. The runner's bundled buildx is enough. Dropped 2026-09-11, the maintainer's yes the same
day, and proved by the v3.1.0 run on 2026-10-08, whose copy and tag-move steps were green with no buildx setup
above them. One fewer third-party action in the job that holds the Docker Hub credential. The `build` job keeps
its `setup-buildx-action` - `provenance: mode=max` and `sbom: true` need the container driver it sets up.
