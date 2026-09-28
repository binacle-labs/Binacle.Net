---
id: ci-cd/decisions/D23
description: one required check, and the maintainer bypasses it
status: decided
verified: 2026-09-29
paths:
  - ".github/workflows/pull-request.yml"
  - "tooling/ci/gate.sh"
---

# D23 — one required check, and the maintainer bypasses it

**Decided (the maintainer, 2026-08-31):** "yes record all three" - answering whether to write down the admin
bypass on `Gate` as a deliberate choice, with the reason.

**Branch protection on `main` requires exactly one status check, `Gate`.** That is the job name, and the job
name is the whole context - not `Pull Request / Gate`. `pull-request.yml` says so above the job: *"`gate` is
the only name branch protection holds."* Every job under it can be renamed freely; this is the last
protection edit that should ever be needed.

**`gate` reports whatever happens.** It is `if: always()` with `needs:` on every other job but `sonar` (`$ci-cd/decisions/D28`), so a skipped half
still produces a verdict. A required check that can silently never report is what leaves a pull request
pending forever, and that shape is designed out rather than watched for.

**The repository-admin role bypasses it, deliberately.** `pull-request.yml` triggers on `pull_request` only.
A commit pushed straight to `main` therefore has no `Gate` check and never will - not a slow one, an absent
one. Without the bypass the maintainer's own push is rejected with nothing to wait for. **The bypass is not a
weakening of a check that was working; it is an admission the check was never going to run for that path.**

What the check actually binds is Dependabot, which opens up to ten pull requests per entry in
`.github/dependabot.yml`. External pull requests are closed (`CONTRIBUTING.md`), so there is nobody else to bind.

**The alternative, rejected for now:** add `push: branches: [main]` and drop the bypass. That makes the
requirement honest for every commit, and costs a full CI run on every push while the workflow stops being
about pull requests. **Revisit it when a second person can commit** - at that point the bypass is protecting
a habit rather than describing a gap.

**`strict_required_status_checks_policy` is off.** A pull request can merge without being up to date with
`main`. With one committer the alternative forces a rebase before every Dependabot merge and buys little.
