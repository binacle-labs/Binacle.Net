---
description: Prereleases go to a public staging repository instead of the one users pull from
state: idea
waits-on: "nobody - it is an idea. horizon: undecided - chosen by an agent, strike it if wrong"
horizon: undecided
paths:
  - ".github/workflows/**"
  - "tooling/image.just"
---

# Prereleases go to a public staging repository

Today every tag the pipeline publishes lands in `binacle/binacle-net`, betas included - D14 says so. That is
what left eight `3.0.0-beta.*` tags sitting beside the release until they were deleted by hand on
2026-09-05, and nothing stops the next set doing the same. **A second public repository takes them instead**,
so the repository users pull from carries released versions only and there is nothing to clean up.

**Public, not private.** A beta is something a person is asked to try, so it has to be pullable without a
login.

## What has to be worked out at pickup

- **Whether this replaces the GHCR staging step or sits after it.** D14 has GHCR as staging that no public
  surface names and nothing else reads. A public prerelease repository is a different job - a surface people
  are pointed at - and the two can coexist or collapse into one. That choice is the whole plan.
- The repository name, and whether its Docker Hub page says anything at all.
- Whether a prerelease is still signed under the same identity, and what `just image verify` does with it.
  `SECURITY.md` and the docs site both print a verify command naming one repository.
- Whether prereleases in the new repository are deleted, kept, or left to accumulate. **A separate repository
  makes the question cheap rather than answering it.**

## Done when

- [ ] A prerelease publishes to the staging repository and to nothing else.
      `docker buildx imagetools inspect binacle/binacle-net:<next prerelease>` fails, and the same reference
      against the staging repository succeeds.
- [ ] The release repository contains released versions and the moving tags, and nothing else.
      `curl -s "https://hub.docker.com/v2/repositories/binacle/binacle-net/tags?page_size=100" | jq -r '.results[].name'`
      matches no prerelease pattern.
- [ ] D14 and D27 say what the shape actually is, and the verify command names the right repository for each.
      **By eye** in `design/ci-cd/decisions.md`, `SECURITY.md` and the docs verifying page.
