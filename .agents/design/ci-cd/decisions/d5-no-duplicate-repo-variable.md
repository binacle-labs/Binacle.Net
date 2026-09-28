---
id: ci-cd/decisions/D5
description: a repo variable may not duplicate a fact that lives in the repo
status: pending
verified: 2026-09-29
paths:
  - ".github/workflows/**"
---

# D5 — a repo variable may not duplicate a fact that lives in the repo

`API_PROJECT_PATH`, `BUILD_OUTPUT` and `BUILD_DOCKERFILE` were removed and replaced by the literal values.

**Why:** a repo setting is invisible to a reader of the repo, it is not versioned with the code, and it can only
drift from the fact it duplicates. One did — `API_PROJECT_PATH` still named the pre-move `src/` path after the
layout change and broke the publish. The project and output folder are decided in `tooling/build.just`, and
there is exactly one `Dockerfile`, at the repo root.

What legitimately stays a variable is a value with **no** home in the repo: the SDK version, the Docker Hub
coordinates, the Sonar project key.
