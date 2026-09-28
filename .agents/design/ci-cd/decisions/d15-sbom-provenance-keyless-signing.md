---
id: ci-cd/decisions/D15
description: the image carries an SBOM and provenance, and is signed keyless
status: pending
verified: 2026-09-29
check: D15's identity regexp against SECURITY.md and tooling/image/verify-signature.sh
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/image/verify-signature.sh"
  - "SECURITY.md"
---

# D15 — the image carries an SBOM and provenance, and is signed keyless

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
with the copy in `$ci-cd/decisions/D2`. The staging image is signed on GHCR and the published image is signed again on Docker Hub,
so the copy users pull verifies. **The Docker Hub signature is the load-bearing one for a release. The staging signature is the only one a
prerelease has** - since `$ci-cd/decisions/D3` a beta stops on GHCR, and `just image verify` checks it there (`$ci-cd/decisions/D21`) - so the
`build` job's cosign step stays; the question of removing it, open while nothing read that signature, is
closed. (GHCR is the only place a beta exists - true until 2026-08-11, false while every beta reached
Docker Hub, and true again since `$ci-cd/decisions/D3` on 2026-09-14.) Signing the **digest** rather than a tag means one signature covers
every public tag, since they are all aliases of it.

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
`cosign verify` - identity regexp plus issuer - now lives in `CHANGELOG.md`, `SECURITY.md`,
`tooling/image/verify-signature.sh`, the docs site and the Docker Hub page. That repetition is deliberate: each audience
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
`$ci-cd/decisions/D1`. **Every copy kept working unchanged**, because every one of them anchored at the `@` and constrained
nothing after it.

**That slack was closed on 2026-08-31, and the instruction it replaces was right about the wrong fix.** The
note here used to read *"the regexp must not be tightened"*, and its argument was that appending `refs/tags/`
would stop every image signed from 2026-08-28 onward from verifying. **That argument holds and is not what was
done.** The regexp now ends `@refs/heads/main$`. Anchoring on the branch keeps every image the surfaces
actually promise and drops the three that nobody was promised:

| | Under the anchored identity |
|---|---|
| `3.0.0` and later | passes - every release signs from `main`, because the release is a dispatch. Checked on the published `3.0.0`, 2026-09-01 |
| `3.0.0-beta.5` | passes - the first release from `main`. Checked, not assumed |
| `3.0.0-beta.3`, `-beta.4` | **fail, and both still resolve.** The deletion recorded here on 2026-08-31 never happened - read the registry, not this row |
| `2.1.1` and earlier | unsigned, unchanged, still `no signatures found` |

**One published surface broke, found and fixed 2026-08-31.** The claim here was that every prerelease left
on Docker Hub signs from `main`. Betas 1 to 4 were never deleted, and `README.md:20` sent a reader to
`3.0.0-beta.4`, so the command in `SECURITY.md` failed on the one image the front page named. **The README named
`3.0.0-beta.6` until the tag, and from 2026-09-01 it names `3.0` alone.** The betas that fail are still pullable and nothing points at them. `SECURITY.md` briefly carried a paragraph explaining how to check a tag-signed prerelease and it
was removed the same day - that removal was right for a different reason: nobody is asked to pull those
images.

**What the anchor buys.** A prefix accepts a signature from any ref in this repository, and a run on any ref
executes the workflow file **as it exists at that ref**. So anyone able to push a branch and start the workflow
could publish an image that passes the command we hand to users. **It needs write access, so it is not a way
in** - it is a way to make the documented check pass on something that is not a release. Dispatch alone does
not close it, because the dispatcher chooses the ref; the `$` is what closes it.

**The gate's *"Check the dispatch is on a ref this version may run from"* step is not what closes it either, and it is worth saying so
before someone reads that step and removes the anchor.** The gate is a job inside the workflow file, so a run
on another ref executes that ref's copy of the file, gate included. It stops an accidental dispatch on a
branch. It cannot stop an edited one. **A check written in the thing being checked is not a control**; the
anchor sits in the verifier, which is the only place outside the attacker's reach.

**`just image verify` takes the ref and the repository as arguments**, defaulting to `refs/heads/main` and
`binacle/binacle-net`. **The ref is part of the identity, so it is an argument rather than a constant** - that
is what let the same recipe check images from either side of this change. Its first reason was that the old
betas stayed checkable, and that reason still holds - read off the registry 2026-09-04, all eight
`3.0.0-beta.*` tags resolve, 23 tags in the repository. The shape would be kept anyway, because a constant
would have to be edited to check anything else, and a recipe you edit to run is not a check.
**The repository argument is what the release workflow uses** to verify what it has just pushed, a scratch
repository included.

**The docs site's two copies are not a coding session's to write.** They are
`sites/docs/collections/_versions/v3.x/release-notes.md` and `verifying-a-release.md`, and each must end
`yml@refs/heads/main$` literally, the same as `SECURITY.md`, `CHANGELOG.md` and the Docker Hub page.
`verify-signature.sh` builds the same ending from its ref argument, `refs/heads/main` by default.

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
against, is decoration. `SECURITY.md` and the docs site's verifying-a-release page are where it is told.
