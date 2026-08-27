---
description: The three release checks that only a real dispatch can prove - a prerelease run, the moving tags now that they come from an explicit value=, and cosign verify against what it publishes
state: blocked
waits-on: "a real run of the release workflow. Everything else this plan asked for is built"
paths:
  - ".github/workflows/release-docker-image.yml"
---

# Prove the dispatch release on a real run

The release pipeline is dispatched with a version and makes its git tag last, after the image is built, smoked
and published. Everything that can be checked without releasing has been. Three things can only be seen on a
run.

## Done when

- [ ] A prerelease dispatch produces what a beta tag produced before - immutable tag only, no `latest`, no
      Docker Hub page. **By eye**, on the run: `publish` green, one tag in the copy step's output, `page`
      skipped.
- [ ] `latest=auto` and the `{{major}}.{{minor}}` tag still fire, now that both metadata steps take their
      version from an explicit `value=` rather than from a tag ref. **By eye**, against a scratch
      `DOCKERHUB_REPO`, on a version with no hyphen. Three tags in the copy step, all resolving to one digest.
- [ ] `cosign verify` passes against the published image with the command as written in `SECURITY.md`.
      `just image verify <version>`. The certificate identity now ends `@refs/heads/main` rather than
      `@refs/tags/v3.0.0`, and every published command anchors at the `@` and constrains nothing after it - a
      green verify here is what proves that reading.

## The two traps in the second one

A version containing a hyphen is treated as a prerelease and proves nothing. A clean `0.0.1` against the real
Docker Hub repository **would move `latest`**, because metadata-action never queries the registry and
`latest=auto` marks any non-prerelease semver as latest. Point `DOCKERHUB_REPO` at a scratch repository.
