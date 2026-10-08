---
name: oidc-subject-is-the-immutable-form
description: This repository's OIDC token carries GitHub's immutable subject claim, which breaks nothing today but is what a cloud trust policy keyed on `sub` would have to match
type: gotcha
when: wiring OIDC trust to a cloud provider, or debugging a trust policy that will not match
paths:
  - ".github/workflows/**"
---

GitHub gives repositories transferred after 15 July 2026 the new immutable `sub` claim, and this one has it.
The subject is built from numeric ids rather than names, so it survives a rename or another organisation move:

```
repo:binacle-labs@<org id>/Binacle.Net@<repo id>:ref:refs/heads/main
```

**It does not break signing.** Fulcio builds the certificate identity from `job_workflow_ref`, not from
`sub`, so cosign and `just image verify` never see this claim.

**It matters the first time OIDC trust is wired to a provider that keys on `sub`.** A policy written from the
old `repo:<org>/<repo>:ref:...` form silently never matches, and the error says only that the token was
rejected. The Docker Hub connection carries a ruleset for each form for exactly this reason.

**Why:** the one place it bites is a place nobody is looking, long after the move that caused it.

**How to apply:** when a trust policy will not match, print the token's `sub` before assuming the policy is
wrong.
