---
id: ci-cd/decisions/D26
description: Docker Hub tag immutability stays off
status: decided
verified: 2026-09-29
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/ci/moving-tags.sh"
---

# D26 — Docker Hub tag immutability stays off

**Decided (the maintainer, 2026-09-03):** "fotget immutability", and later that day: "...dockerhub
immutability...its no for now'"

**Answered 2026-09-04: no, for now.** The switch is off. Recorded as a decision rather than left as an open
question, because an open question with no value is worse than either answer.

**What would reopen it.** A Docker Hub tag actually being overwritten - the risk this was weighed against has
never happened, and one occurrence changes the arithmetic. Or a rehearsal on a throwaway repository proving a
rule scoped to released versions behaves as documented, which is the safe path this entry names and nobody
has walked.

**What it would have bought:** a published release tag that cannot be overwritten. **What it costs is the
reason not to.** There is no undo. An immutable tag cannot be deleted either, so a release tag pushed by
mistake is permanent, and turning it on safely means rehearsing it on a throwaway repository first.

**The stored rule is `.*`, and that is what makes the switch dangerous here rather than merely irreversible.**
`.*` matches `latest` and the other tags every release moves. Enabling it against that rule would freeze
the moving tags on the first release after it, which is exactly the mechanism `$ci-cd/decisions/D25` depends on. **If this is
ever reopened, the rule is the first thing to correct** - released versions only, never the moving tags.

**What already protects the release.** `$ci-cd/decisions/D24` - a tag ruleset that nobody bypasses, so a release tag cannot be
moved or deleted on the GitHub side. Immutability would have covered the registry side of the same risk, and
the registry side has never gone wrong.
