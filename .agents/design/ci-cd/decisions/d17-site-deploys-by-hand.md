---
id: ci-cd/decisions/D17
description: the site deploys are published by hand, and never on a push
status: pending
verified: 2026-09-29
check: D17 against deploy-site.yml's trigger, which must stay workflow_dispatch only
paths:
  - ".github/workflows/deploy-site.yml"
---

# D17 — the site deploys are published by hand, and never on a push

**Dated 2026-08-19, and it covered `deploy-www-site.yml` too, added after.** The site
deploy is `workflow_dispatch` and stays that way - `deploy-site.yml` since `$ci-cd/decisions/D32`, three files before it. No
`push` trigger on `sites/**`, and no scheduled run.

**Why:** publishing to the internet is a deliberate act, not a side effect of a commit. Those folders are
written in their own session, and pressing the button is part of how that session ends — a merge that happens
to touch a page is not a decision to put it live.

**Two mechanical consequences that make the same point.** The marker tag is numbered by `github.run_number`, so
a push trigger would produce a tag per commit and the tag would stop meaning "this is live". And the
concurrency group is never cancelled — `cancel-in-progress: false`, because a stopped run leaves the site
deployed with no marker tag — so a busy branch would queue rollouts behind each other rather than skip to the
last.

**This closes the question the workflow restructure left open.** It was not CI's to answer.
