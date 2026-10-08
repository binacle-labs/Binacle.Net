---
id: ci-cd/decisions/D33
description: the release logs into Docker Hub with the run's OIDC token
status: decided
verified: 2026-09-29
check: D33 against the same job's Docker Hub login, which must carry no password: and must read vars.DOCKERHUB_OIDC_CONNECTIONID
paths:
  - ".github/workflows/release-docker-image.yml"
  - ".github/workflows/shared-dockerhub-overview.yml"
---

# D33 — the release logs into Docker Hub with the run's OIDC token

**Decided (the maintainer, 2026-09-11):** "Approve (Recommended)" - picked in a question prompt, to "Docker Hub
OIDC login - drop the stored registry token from the release `publish` job?"

The `publish` job's `docker/login-action` step keeps `username:` and drops `password:`;
`DOCKERHUB_OIDC_CONNECTIONID` in its `env:` names the org's OIDC connection, and the job's `id-token: write` -
already there for cosign - is what the exchange needs.

**Why.** The credential that can push to the registry users pull from stops existing between runs. It is
minted per run and expires with it - the property that made GHCR the staging registry (`$ci-cd/decisions/D14`), applied to the
registry that matters. Nothing to rotate, nothing to leak from a repository setting. This is also most of the
reason `$ci-cd/decisions/D29` said no to an environment: the thing it would have fenced is gone.

**What it does not close.** `shared-dockerhub-overview.yml` writes the repository description through the
Docker Hub web API with `peter-evans/dockerhub-description`, which takes a username and password and has no
OIDC path. So `DOCKERHUB_USERNAME` and `DOCKERHUB_TOKEN` survive for the `page` job alone. The token can be
narrowed to what that job needs, which is not push.

**What it depends on outside the repository - in place since 2026-09-14.** An OIDC connection on the
`binacle` Docker Hub org and its id in the `DOCKERHUB_OIDC_CONNECTIONID` repository variable. Connections
are offered to Team, Business, Hardened Images and Sponsored Open Source orgs.

**The connection carries two rulesets, both on `binacle/binacle-net`, one per subject form.** This
repository was transferred after 15 July 2026, so GitHub issues the immutable subject
`repo:binacle-labs@189874141/Binacle.Net@607841255:ref:refs/heads/main` - the ids are the org's and the
repository's from the GitHub API. The plain `repo:binacle-labs/Binacle.Net:ref:refs/heads/main` is the second
ruleset. Whether Docker matches the immutable form, the plain one, or normalises between them is not
documented, and a rule that never matches costs nothing; two rules mean the release cannot fail on that
question. **Both are pinned to `main`** - the only ref `gate` lets through anyway.

**A prerelease never reaches the login (`$ci-cd/decisions/D3`), so the v3.1.0 release run is the first to exercise it.** A red
there is after the build and the smoke and before anything is copied; Docker Hub is untouched and the same
version is dispatched again.

**`DOCKERHUB_TOKEN` is not narrowed.** Docker Hub's access-token screen offers scopes but no repository
picker on this org, so the token the `page` job uses stays read/write/delete on the account. Do not go
looking for the narrowing; it is not there.
