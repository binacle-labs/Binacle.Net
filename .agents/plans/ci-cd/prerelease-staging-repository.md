---
description: Branch builds go to the staging registry on dispatch, the way a prerelease now does - so an image from a branch can be tried without ever reaching the repository users pull from
state: idea
waits-on: "the signing story for a branch-built image, and whether staging images are ever deleted. horizon: undecided - chosen by an agent, strike it if wrong"
horizon: undecided
paths:
  - ".github/workflows/**"
  - "tooling/ci/**"
  - "tooling/image.just"
  - "tooling/image/**"
---

# Branch builds go to the staging registry

**The prerelease half of this plan landed on 2026-09-14.** A prerelease now stops at `ghcr.io/binacle-labs/binacle-net`
after the smoke - no Docker Hub copy, no tag, no GitHub release - and the reasoning is in the CI/CD decisions
ledger. GHCR is the staging repository; the second public Docker Hub repository this plan used to propose is
not needed, because GHCR is already public and needs no new credential. **What is left is branch builds.**

**The rule worth having, and it is simpler than a retention policy:** a tag in the release repository is a
release. No betas, no branch builds, no exceptions. Prereleases now honour it; branch builds cannot be made at
all yet.

## Branch builds

**Dispatch only, on any ref. Never on push - answered 2026-09-05.** No branch pattern, no `push:` key, no
automatic trigger of any kind. A build happens because somebody asked for one, which is also what keeps
`dependabot/**` out without having to name it: those branches are pushed, never dispatched.

**Since 2026-09-14 a prerelease already dispatches from a `release/*` branch** - `check-release-ref.sh`
lets a hyphenated version through from `main` or `release/*` and keeps a release on `main`. So a release
branch can already be tried by giving it a prerelease version. What this plan still adds is a build from any
other branch, carrying no version at all.

**A staging tag must not look like a release.** Slug the ref - `/` and `_` to `-`, lowercased - and add the
short sha: `features/release_v3-1` becomes `release-v3-1-<sha>`. A Docker tag takes no `/`.

## What has to be worked out at pickup

- **The signing story, and this one has already bitten.** The published verify command is anchored at
  `@refs/heads/main$` in `SECURITY.md`, on the Docker Hub page and on the docs verifying page. **Anything
  built from a branch signs under that branch's ref and fails it.** That is precisely why the v3.0.0 betas 1
  to 4 read as tampered - they were signed under a tag ref. A prerelease from `main` is fine; a branch build
  is not. Either the branch build is unsigned and says so, or the run summary prints the identity that does
  verify it. **Do not publish one command that fails on half the images it appears to describe.**
- **Whether staging images are ever deleted.** GHCR still holds `3.0.0-beta.3` to `-beta.8`, `3.0.0` and a
  stale `latest`, read on 2026-09-14. Nothing names them and nothing costs while they sit there; a branch
  build per dispatch changes the rate.

## Done when

- [ ] A branch build publishes to the staging registry and to nothing else.
      `docker buildx imagetools inspect binacle/binacle-net:<slug>-<sha>` fails, and the same reference
      against `ghcr.io/binacle-labs/binacle-net` succeeds.
- [ ] The `main` check still gates everything that reaches Docker Hub.
      `grep -n check-release-ref .github/workflows/release-docker-image.yml` still matches on the publish path.
- [ ] A reader can tell which images the published verify command covers.
      **By eye** in `SECURITY.md` and on the Docker Hub page. If a staging image fails the command a page
      prints and nothing says so, the box is open.
