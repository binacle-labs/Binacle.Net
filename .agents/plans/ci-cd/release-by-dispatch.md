---
description: The three release checks that only a real dispatch can prove - a prerelease run, the moving tags now that they come from an explicit value=, and cosign verify against what it publishes
state: blocked
waits-on: "a scratch-repo run for the moving tags - a non-prerelease version against a scratch DOCKERHUB_REPO"
paths:
  - ".github/workflows/release-docker-image.yml"
---

# Prove the dispatch release on a real run

The release pipeline is dispatched with a version and makes its git tag last, after the image is built, smoked
and published. Everything that can be checked without releasing has been. Three things can only be seen on a
run.

## Done when

- [x] **`3.0.0-beta.6`, run `33339765633`, 2026-08-30.** One tag in the copy step, `moving=` empty, `page`
      skipped. `publish` went red **after** the copy, on the verify - see the fourth box.
- [ ] `latest=auto` and the `{{major}}.{{minor}}` tag still fire, now that both metadata steps take their
      version from an explicit `value=` rather than from a tag ref. **By eye**, against a scratch
      `DOCKERHUB_REPO`, on a version with no hyphen. Three tags in the copy step, all resolving to one digest.
- [x] **The identity reading is proven - 2026-08-30.** The command as written in `SECURITY.md` passes against
      the published `3.0.0-beta.6`, so `@refs/heads/main` is what a dispatch signs under.
- [x] **The step passes inside the run - 2026-08-31.** It did not on 2026-08-30: Docker Hub's referrers index
      is eventually consistent and the verify read it three seconds after the sign, so it said
      `no signatures found` on an image that verified from a laptop under the same cosign build. A
      five-attempt retry went into the workflow step at `release-docker-image.yml:281-286` and the recipe was
      left alone, because it is what `SECURITY.md` hands readers. **`3.0.0-beta.7`, run `33374185508`,
      dispatched from `ce9e4461`, finished green.**
- [ ] The two-stage copy behaves. **The prerelease half is done** - the copy listed the version tag on its
      own and `Move the tags that move` skipped itself with nothing to move. **What is untested is the half
      that matters**: `3.0` and `latest` written after a green verify, which only a non-prerelease run does.
      **Added 2026-08-31 with the change it checks.**

## The two traps in the second one

A version containing a hyphen is treated as a prerelease and proves nothing. A clean `0.0.1` against the real
Docker Hub repository **would move `latest`**, because metadata-action never queries the registry and
`latest=auto` marks any non-prerelease semver as latest. Point `DOCKERHUB_REPO` at a scratch repository.
