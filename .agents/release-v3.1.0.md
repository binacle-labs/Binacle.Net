---
description: Release - Binacle.Net v3.1.0. The checklist from the last beta to the tag.
---

# Release - Binacle.Net v3.1.0

The demo UI release: the shipped clients move to v4, the demo gains `Best` and the winning algorithm, a request
panel, a new first worked example and ViPaq sample strings, and loses two faults. Every row of work has landed;
what shipped is the `## [Unreleased]` section of `CHANGELOG.md`. The history of this file is in git.

Branch `release/v3-1-0`. A beta dispatches from the branch and stops at GHCR; the release dispatches from
`main` only. After the tag, open `post-release-v3.1.0.md`.

## Done

- [x] Every feature, fix and maintenance row landed, and the changelog and `.agents/docs/` describe them.
- [x] `3.1.0-beta.4`, dispatched 2026-10-07 from `f16a8904` and checked 2026-10-08. The run is green, with
      `publish` and `page` skipped. The tag and the prerelease exist, and the prerelease body is the
      `[Unreleased]` section. Docker Hub has no `beta` tag. `just image verify` passes, signed on
      `refs/heads/release/v3-1-0`. The four UI pages answer 200 and the bundle carries every feature.
      `just smoke all` is green - the maintainer, 2026-10-08.

## To the tag

- [ ] **The maintainer's call:** a changelog line for `ParallelLimits` and the two v3 algorithms, or none.
      Both are in `Binacle.Lib` since `beta.3`; the image runs neither.
- [ ] Pull request from `release/v3-1-0` to `main`, and `Gate` is green. **Watch the Sonar job** - it is the
      first real pull request since `D28`, nothing sets `sonar.pullrequest.*`, and a failed quality gate turns
      it red. Neither holds the merge; `sonar` is outside `gate`'s `needs`.
- [ ] Merged. `main` is now `f16a8904` plus agent files and the merge.
- [ ] `Deploy Site` dispatched from `main` with `demo`, green, and `demo.binacle.net/packing` packs with `Best`.
      **The first run of `deploy-site.yml` and of `create-tag.sh`.** Safe before the image: the v3.0.0 image
      already serves `pack/compare-bins`, `Best` and `algorithmUsed` on v4.
- [ ] `## [Unreleased]` is `## [3.1.0] - <date>` on `main`. **The last edit before the tag.**
      `just changelog check 3.1.0` passes, and `just changelog extract 3.1.0` prints the release body. Read it.
- [ ] Actions -> Build and Release Docker Image -> Run workflow, on `main`, version `3.1.0`, and the run is
      green. **The first run of the Docker Hub OIDC login and the copy without a builder** - findings 1 and 8
      of `plans/ci-cd/ci-open-questions.md`. A red `publish` leaves Docker Hub untouched and no tag: fix on
      `main` and dispatch again.
- [ ] Finding 8 ticked in `plans/ci-cd/ci-open-questions.md` - every box in that plan is ticked.
- [ ] `github.com/binacle-labs/Binacle.Net/releases/tag/v3.1.0` exists. Open `post-release-v3.1.0.md`.

**`3.1` and `3` are new tags and `latest` moves.** `3.0` stays on `3.0.0`, and every sample keeps naming it
until the post-release pin move: a pin on `main` must name an image that already exists.

**Delete this file once v3.1.0 is out and verified.**
