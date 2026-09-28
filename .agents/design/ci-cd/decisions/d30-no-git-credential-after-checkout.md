---
id: ci-cd/decisions/D30
description: no job holds a git credential after checkout
status: decided
verified: 2026-09-29
paths:
  - ".github/workflows/**"
  - "tooling/ci/create-tag.sh"
---

# D30 — no job holds a git credential after checkout

**Decided (the maintainer, 2026-09-11):** "Approve both (Recommended)" - picked in a question prompt, to
"`push-tag.sh` - delete the two dead `git config` lines and replace the last `git push` with `gh api`, so every
checkout can carry `persist-credentials: false`?"

Every `actions/checkout` step in `.github/workflows/` - twenty-two then, eighteen once
`$ci-cd/decisions/D32` folded the deploys into one file - carries
`persist-credentials: false`, and nothing in CI runs `git push`. The last push, the deploy marker tag, became
one `gh api` call (`create-tag.sh`, `POST repos/{repo}/git/refs`) taking `GH_TOKEN` for that step alone.

**What it closes.** A compromised step inside any job could use the token checkout leaves behind. Checkout
v6 already moved that token out of `.git/config` into `$RUNNER_TEMP`, so the older leak - the config file
carried out inside an uploaded artifact - was closed before this; what this closes is use of the token by a
later step in the same job. The advice comes from workflow auditors (zizmor's `artipacked`), not from GitHub's
own pages, which do not mention the setting.

**What it costs.** One line per checkout, and the API's "Reference already exists" where git said "tag
already exists". `check-release-tag.sh` still reaches origin with `git ls-remote`, which works anonymously on
a public repository, and the deploy `tag` job still checks out because `deploy-summary.sh` reads the subject
with `git log`.
