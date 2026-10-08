---
id: ci-cd/decisions/D11
description: every action is pinned by commit SHA, and Dependabot keeps the pins moving
status: pending
verified: 2026-09-29
check: D11 against .github/dependabot.yml
paths:
  - ".github/workflows/**"
  - ".github/actions/**"
  - ".github/dependabot.yml"
---

# D11 — every action is pinned by commit SHA, and Dependabot keeps the pins moving

With the version in a trailing comment, so the pin is readable. `.github/dependabot.yml` raises a weekly PR per
action, rewriting the SHA and the comment together.

**Why:** a mutable tag is a supply-chain hole — it can be re-pointed at any commit, including after review. The
trailing comment is what keeps the pin maintainable; a bare SHA tells a reader nothing about how far behind it
is.

**Why first-party actions too, as of 2026-08-11.** `actions/*` and `docker/setup-*` were left on major tags on
the grounds that the publisher is trusted. That is a weaker rule than it looks: the risk a SHA pin addresses is
the tag being re-pointed, and `actions/checkout@v4` is exactly as re-pointable as any other tag. Two rules also
meant every reader had to know which action fell under which. One rule, applied to every workflow.

**The pin and the automation are one decision, not two.** A pinned action with nothing watching it stays on
whatever commit it was set to and stops receiving security fixes, which is worse than a floating tag because
nothing reports it. `docker/build-push-action` sat at v5.4.0, several majors behind, which is what made the
point concrete.

**The pins stayed in the composite actions; the config grew instead, on 2026-08-19.** Four outside SHAs moved
into `.github/actions/` with the workflow restructure, and Dependabot does not reach that folder from
`directory: /` — it covers `.github/workflows` and a root-level `action.yml`, and nothing else. The open
question was whether to answer that by pulling the four pins back out into a workflow file. **No:** the point
of the composite actions is that a setup step is written once, and undoing that to satisfy a config format is
the tail wagging the dog. `.github/dependabot.yml` carries one entry per action folder that pins an outside action instead. The cost is
that adding an outside pin to a new action means remembering to add an entry — which is why the rule is
written down in `$ci-cd` beside the actions themselves, not only here.
