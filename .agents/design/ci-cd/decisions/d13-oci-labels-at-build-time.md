---
id: ci-cd/decisions/D13
description: per-build OCI labels are applied at build time, never as LABEL fed by ARG
status: pending
verified: 2026-09-29
paths:
  - "Dockerfile"
  - "tooling/build.just"
  - ".github/workflows/release-docker-image.yml"
---

# D13 — per-build OCI labels are applied at build time, never as `LABEL` fed by `ARG`

Version, revision and created are set with `--label` (locally) or by metadata-action (in CI). Constant labels
stay as `LABEL` lines in the `Dockerfile`.

**Why:** those three change on every build. As Dockerfile `LABEL`s fed by `ARG` they would invalidate the layer
cache from that point down, for metadata nothing executes. `--label` writes image-config metadata with no layer
and no cache cost.

metadata-action overrides three of the Dockerfile's constant labels on purpose — `licenses`, because
auto-detection returns `NOASSERTION` for a repo that declares more than one licence; `url`, which should be the landing site
rather than the repo; and `description`, which auto-fills from the GitHub repository blurb and silently beats
the `Dockerfile`'s caption. **The third was moved here on 2026-08-28** from a comment in the workflow, which
was the only place it was written down.
