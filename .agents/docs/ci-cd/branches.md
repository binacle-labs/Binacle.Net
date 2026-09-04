---
id: ci-cd/branches
description: "Branch names — the two kinds in use, the snake_case subject, and the one constraint that is mechanical today: Sonar analyses main and pull requests targeting main, and nothing else"
verified: 2026-09-05
check: "The kinds here match the branches on the remote and the merge commits in git log; pull-request.yml still triggers on pull_request with no branches: filter; sonar-analysis.yml is still workflow_dispatch only; codeql-analysis.yml's branches: list still names main alone"
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
```

**Two kinds, and there is no third.** `features/` for new work, `fixes/` for corrections. Documentation and
tooling work rides under `features/` - `features/docs_work` is the precedent.

**The subject is `snake_case`**, lowercase, no second `/`, and short. Around forty characters is the practical
ceiling.

**A `release/` kind was considered on 2026-09-05 and not adopted.** It would have been legibility only:
release-prep branches have used `features/release_*` and nothing matches on the prefix, so a third kind buys a
tidier `git branch` listing and nothing else.

## The one constraint that is not taste

**Every branch targets `main`, and nothing is long-lived.** SonarCloud's free plan analyses `main`, and
analyses a pull request *only if its target branch is `main`*. A branch opened against another branch would
get nothing.

**Today that costs nothing, because no pull request is analysed at all** - `sonar-analysis.yml` is
`workflow_dispatch` only. It becomes real the moment that trigger lands, and `pull-request.yml` triggers on
`pull_request` with no `branches:` filter, so a run against the wrong target would still go green with the
Sonar half quietly missing.

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
