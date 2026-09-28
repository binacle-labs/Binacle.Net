---
id: ci-cd/decisions/D10
description: npm ci --ignore-scripts
status: pending
verified: 2026-09-29
paths:
  - ".github/workflows/**"
  - ".github/actions/**"
---

# D10 — `npm ci --ignore-scripts`

**Why:** an install-time lifecycle hook is arbitrary code execution from a dependency. Nothing here needs one —
no workspace declares `prepare` or `postinstall`, and the only dependency with an install script is `fsevents`,
which is darwin-only and never installed on a Linux runner. The flag costs nothing and closes the hole.
