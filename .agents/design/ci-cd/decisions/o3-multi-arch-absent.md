---
id: ci-cd/decisions/O3
description: open question: multi-arch is still absent
status: open
verified: 2026-09-29
paths:
  - ".github/workflows/release-docker-image.yml"
  - "Dockerfile"
---

# O3 — multi-arch is still absent

`linux/amd64` only. No second architecture is built, and nothing asks for one yet.

It stays out because it **changes the artifact** and roughly doubles build time, and because there is no
evidence of demand. Attestation and signing, which used to share this entry, are now done — see `$ci-cd/decisions/D15`.
