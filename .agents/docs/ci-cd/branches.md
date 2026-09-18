---
id: ci-cd/branches
description: "Branch names — the three kinds in use, the snake_case subject, and the two constraints that are mechanical: Sonar analyses main and pull requests targeting main, and only main or the release/v<x>-<y>-<z> branch named after the version may dispatch the release workflow"
verified: 2026-09-18
check: "The kinds here match the branches on the remote and the merge commits in git log; pull-request.yml still triggers on pull_request with no branches: filter and calls sonar-analysis.yml; tooling/ci/check-release-ref.sh still admits refs/heads/main, and refs/heads/release/v<x>-<y>-<z> only for a prerelease of x.y.z, and nothing else; codeql-analysis.yml's branches: list still names main alone"
also_update:
  - ci-cd
paths:
  - ".github/workflows/**"
---

# Branch names

`main` is the only long-lived branch. Everything else is short and merges back into it.

```
<kind>/<subject>        features/post_release_v3
                        fixes/sonar_fixes
                        release/v3-1-0
```

**Three kinds.** `features/` for new work, `fixes/` for corrections, `release/` for the branch a version's
betas are dispatched from. Documentation and tooling work rides under `features/` - `features/docs_work` is
the precedent.

**The subject is `snake_case`**, lowercase, no second `/`, and short. Around forty characters is the practical
ceiling. A `release/` subject is the version with dots as hyphens - `release/v3-1-0` - so it reads as the
version it is.

**`release/` is the one kind a workflow matches on.** `tooling/ci/check-release-ref.sh` lets a prerelease
dispatch from `refs/heads/main` or from the `release/v<x>-<y>-<z>` branch named after it - `release/v3-1-0`
may dispatch `3.1.0-beta.*` and no other version, since 2026-09-18 - and a release from `main` only; a
dispatch on any other ref fails in the `gate` job. The kind was considered on 2026-09-05 and not adopted, because it was
legibility only; it was adopted on 2026-09-14 when the prerelease rule gave it a mechanical meaning.

## The one constraint that is not taste

**Every branch targets `main`, and nothing is long-lived.** SonarCloud's free plan analyses `main`, and
analyses a pull request *only if its target branch is `main`*. A branch opened against another branch would
get nothing.

**This is mechanical since 2026-09-10** - `pull-request.yml` calls `sonar-analysis.yml` on every pull request
that touches code. It triggers on `pull_request` with no `branches:` filter, so a pull request against the
wrong target would still go green with the Sonar half quietly missing.

That is why the arrangement is one long-lived branch rather than a develop-and-main pair. It is a consequence
of the plan the project is on, not a preference.

## `dependabot/**` is not ours

Dependabot creates and deletes its own branches under that prefix. **Never reuse it, never rename one.** A
`pull_request` from a Dependabot branch reads the Dependabot secret store rather than the Actions one, so
anything keyed to a secret behaves differently on those runs.

## Lowercase, and why it will matter more later

Nothing turns a branch name into an identifier today. **It stays lowercase anyway**, because the one thing
that would - building an image from a branch - cannot take an uppercase path or a `/`. GHCR rejects an
uppercase path, which is why `release-docker-image.yml` writes `STAGING_IMAGE` out longhand instead of
deriving it from the owner name. A branch named to survive that conversion costs nothing now.
