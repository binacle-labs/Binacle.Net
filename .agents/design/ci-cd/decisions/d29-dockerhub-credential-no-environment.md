---
id: ci-cd/decisions/D29
description: the Docker Hub credential is not scoped to an environment
status: decided
verified: 2026-09-29
paths:
  - ".github/workflows/release-docker-image.yml"
  - ".github/workflows/shared-dockerhub-overview.yml"
---

# D29 — the Docker Hub credential is not scoped to an environment

**Decided (the maintainer, 2026-09-11):** "Reject (Recommended)" - picked in a question prompt, to "Put `publish`
in a GitHub environment that admits `main` only, and move the Docker Hub secrets into it?"

The `publish` job stays out of a GitHub environment with a `main`-only branch policy, and the two Docker Hub values
stay repository secrets. `check-release-ref.sh` in `gate` remains the thing that keeps a release on `main`.

**Why not.** Two reasons, and the first is the one that decides it. The long-lived registry token is leaving
`publish` anyway: the Docker Hub login moves to an OIDC connection minted per run, so the credential the
environment would have fenced stops existing in that job. What would be left to scope is the page job's
token, which writes the repository description through the web API and has no OIDC path. Second, an
environment that admits `main` only refuses a prerelease dispatched from anywhere else, and that door is
worth keeping open while the prerelease staging repository is still undecided.

**What this does not decide.** Whether a prerelease may be dispatched from a branch. Since 2026-09-14 one may, from its own
`release/` branch (`$ci-cd/decisions/D3`) - a `main`-only environment would have refused it.
