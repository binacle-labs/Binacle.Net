---
description: Prereleases and branch builds go to a public staging repository, so the repository users pull from only ever receives a final release
state: idea
waits-on: "whether this replaces the GHCR staging step, and what the published verify command covers. horizon: undecided - chosen by an agent, strike it if wrong"
horizon: undecided
paths:
  - ".github/workflows/**"
  - "tooling/ci/**"
  - "tooling/image.just"
---

# Prereleases and branch builds go to a staging repository

Today every tag the pipeline publishes lands in `binacle/binacle-net`, betas included - D14 says so. That left
eight `3.0.0-beta.*` tags sitting beside the release until they were deleted by hand. **A second public
repository takes them instead, and it takes branch builds too**, so the repository users pull from receives
final releases and nothing else, and there is nothing to clean up afterwards.

**That is the rule worth having, and it is simpler than a retention policy:** a tag in the release repository
is a release. No betas, no branch builds, no exceptions.

**Public, not private.** A beta or a branch image is something a person is asked to try, so it has to be
pullable without a login.

## Branch builds

**Dispatch only, on any ref. Never on push - answered 2026-09-05.** No branch pattern, no `push:` key, no
automatic trigger of any kind. A build happens because somebody asked for one, which is also what keeps
`dependabot/**` out without having to name it: those branches are pushed, never dispatched.

The release workflow is `workflow_dispatch` on `main` only - `gate` runs `just ci check-release-ref`. A branch
build has to get past that check, **for the staging path only. The `main` check stays exactly as it is on the
path that reaches Docker Hub.**

**A staging tag must not look like a release.** Slug the ref - `/` and `_` to `-`, lowercased - and add the
short sha: `features/release_v3-1` becomes `release-v3-1-<sha>`. A Docker tag takes no `/`.

## What has to be worked out at pickup

- **Whether this replaces the GHCR staging step or sits after it.** D14 has GHCR as staging that no public
  surface names and nothing else reads. A public repository is a different job - a surface people are pointed
  at - and the two can coexist or collapse into one. That choice is the whole plan.
- **The signing story, and this one has already bitten.** The published verify command is anchored at
  `@refs/heads/main$` in `SECURITY.md`, on the Docker Hub page and on the docs verifying page. **Anything
  built from a branch signs under that branch's ref and fails it.** That is precisely why betas 1 to 4 read as
  tampered - they were signed under a tag ref. Either the staging repository says plainly that the published
  command does not cover it, or it carries its own identity line. **Do not publish one command that fails on
  half the images it appears to describe.**
- The repository name, and whether its Docker Hub page says anything at all.
- Whether staging images are ever deleted. **A separate repository makes the question cheap rather than
  answering it.**

## Done when

- [ ] A prerelease and a branch build both publish to the staging repository and to nothing else.
      `docker buildx imagetools inspect binacle/binacle-net:<next prerelease>` fails, and the same reference
      against the staging repository succeeds.
- [ ] The release repository holds released versions and the moving tags, and nothing else.
      `curl -s "https://hub.docker.com/v2/repositories/binacle/binacle-net/tags?page_size=100" | jq -r '.results[].name'`
      matches no prerelease and no branch pattern.
- [ ] The `main` check still gates everything that reaches Docker Hub.
      `grep -n check-release-ref .github/workflows/release-docker-image.yml` still matches on the publish path.
- [ ] A reader can tell which images the published verify command covers.
      **By eye** in `SECURITY.md` and on both Docker Hub pages. If a staging image fails the command a page
      prints, the box is open.
- [ ] D14 and D27 say what the shape actually is.
      **By eye** in `design/ci-cd/decisions.md`.
