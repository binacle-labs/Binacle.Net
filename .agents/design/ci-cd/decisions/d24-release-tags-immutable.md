---
id: ci-cd/decisions/D24
description: release tags cannot be moved or deleted, and nobody bypasses that
status: pending
verified: 2026-09-29
paths:
  - "tooling/ci/github-release.sh"
  - "tooling/ci/create-tag.sh"
---

# D24 — release tags cannot be moved or deleted, and nobody bypasses that

A tag ruleset on `refs/tags/v*` blocks **update** and **deletion**. It matches 48 tags, counted 2026-09-04,
every release back through v1 and v2. **Creation stays allowed**, or the release would break - `github-release.sh` makes the tag
itself.

**The bypass list is empty on purpose, including the maintainer.** A published image and a GitHub release
both point at a tag; moving one makes an artifact that has been public since January unverifiable, and there
is no undo. This is the only rule here guarding something irreversible, which is why it is the only one with
no way around it. Deleting a tag now means disabling the ruleset, deleting, and re-enabling - the friction is
the feature.

**`v*` and not everything.** The 20 deploy marker tags - `docs-6`, `web-release-5`, `www-1` - are created by
`create-tag.sh` on every site deploy and must stay free. Counted 2026-09-04; both numbers here grow on their
own, so read them rather than trusting them.
