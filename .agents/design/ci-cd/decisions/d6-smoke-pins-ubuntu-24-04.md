---
id: ci-cd/decisions/D6
description: shared-smoke-image.yml pins ubuntu-24.04, everything else takes ubuntu-latest
status: pending
verified: 2026-09-29
check: D6 against shared-smoke-image.yml's runs-on
paths:
  - ".github/workflows/shared-smoke-image.yml"
---

# D6 — `shared-smoke-image.yml` pins `ubuntu-24.04`, everything else takes `ubuntu-latest`

**Why:** hurl links `libxml2.so.2`. Ubuntu 26.04 ships only `libxml2.so.16` and carries no compat package, so
hurl dies there with a missing-library error that reads like a hurl bug rather than a distro change.
`ubuntu-latest` will move to 26.04 eventually, and this workflow runs rarely enough that it would break on the
day it is needed most.
