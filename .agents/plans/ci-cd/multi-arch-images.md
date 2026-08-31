---
description: CI - publish the image for arm64 as well as amd64
state: idea
waits-on: "someone asking for ARM - nobody has"
horizon: on-demand
paths:
  - ".github/workflows/**"
---

# Publish the image for arm64 as well as amd64

The published image is `linux/amd64` only. Building it for arm64 too is weeks of work with no evidence anyone
wants it, so the question to answer first is whether anyone runs Binacle.Net on ARM. If nobody asks, amd64 is
defensible and the useful action is writing that down as a decision rather than leaving it looking incidental.

## Research

### Date not recorded - amd64 only, confirmed

By inspecting the manifest of `3.0.0-beta.1`.

### Date not recorded - five things that would bite, so they are not re-derived

- **The naming trap.** Docker's `TARGETARCH` is `amd64`/`arm64`; .NET's runtime identifiers are
  `linux-x64`/`linux-arm64`. They do not match, and the mapping has to live in exactly one place or the image
  gets the wrong binaries and still builds.
- **`.dockerignore` allowlists `artifacts/binacle-net`.** Renaming the publish directories per architecture
  means updating it, or the image builds empty.
- **Start with QEMU in one job**, not a runner matrix. The release runs rarely and slow minutes on release day
  cost nothing. Move to a matrix only when that becomes annoying.
- **Test on a native ARM runner.** `shared-smoke-image.yml` already does the whole job, so making its
  `runs-on` a matrix over `[ubuntu-24.04, ubuntu-24.04-arm]` gives the full suite on both. Its tool install
  hardcodes x86_64 URLs and needs to pick by `$(uname -m)`; both tools publish arm64 builds.
- **Re-check signing.** `cosign sign` runs against the digest of the index. Confirm that covers every platform
  manifest under it before claiming a multi-arch image is signed.
