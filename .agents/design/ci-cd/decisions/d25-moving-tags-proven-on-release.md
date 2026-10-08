---
id: ci-cd/decisions/D25
description: the moving tags were proven on the release itself, not on a scratch repository
status: pending
verified: 2026-09-29
check: D25 against release-docker-image.yml's `Move the tags that move` step, which must stay conditional on a non-empty moving list
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/ci/moving-tags.sh"
---

# D25 — the moving tags were proven on the release itself, not on a scratch repository

**Closed on 2026-09-01 by the v3.0.0 run.** Until then this was an open question: a prerelease produces only
its own immutable tag, so `{{major}}.{{minor}}` and `latest=auto` firing — and `imagetools create` being handed
three references instead of one — had never run. `Move the tags that move` had only ever skipped itself.

**The plan was a throwaway run against a scratch `DOCKERHUB_REPO`. It was never needed.** The real release
exercised the same step with nothing standing in for anything: `3.0.0`, `3.0` and `latest` were all written
after a green verify and all three resolve to `sha256:974f3dda3923`, read off the registry with
`just image verify 3.0.0` on 2026-09-02.

**Both traps that made a rehearsal awkward are also why the rehearsal was worth skipping.** A version
containing a hyphen is treated as a prerelease and proves nothing, and a clean `0.0.1` against the real
repository **would move `latest`**, because metadata-action never queries the registry and `latest=auto` marks
any non-prerelease semver as latest. A rehearsal that avoids both is a rehearsal of something else.

**The v3.1.0 run closed the rest on 2026-10-08.** It moved `latest` off `3.0.0`, wrote `3.1` and wrote `3`
for the first time. All four resolve to `sha256:2ca375b86e33`, and `3.0` still resolves to
`sha256:974f3dda3923`, read off the registry with `docker buildx imagetools inspect` the same day.
