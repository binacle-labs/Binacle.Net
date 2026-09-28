---
id: ci-cd/decisions/D3
description: a prerelease stays on GHCR, and still gets its tag and a GitHub prerelease, through two job conditions
status: decided
verified: 2026-09-29
check: D2/D3/D14 against release-docker-image.yml's publish job, which must carry `if: ${{ !contains(inputs.version, '-') }}`, and its release job, which must carry `if: ${{ !cancelled() && !contains(needs.*.result, 'failure') }}` and keep `publish` in its needs, with no condition on `page`
paths:
  - ".github/workflows/release-docker-image.yml"
  - "tooling/ci/check-release-ref.sh"
  - "tooling/ci/github-release.sh"
  - "tooling/ci/changelog-section.sh"
  - "tooling/ci/release-summary.sh"
---

# D3 — a prerelease stays on GHCR, and still gets its tag and a GitHub prerelease

**Decided (the maintainer, 2026-09-18):** "should i publish beta versions to giuthub?", then, to the answer
yes: "do itand then scan what needs doing for release 3.1.0?"

**Decided 2026-09-18 by the maintainer, amending the 2026-09-14 rule.** `publish` carries
`if: ${{ !contains(inputs.version, '-') }}` and `page` skips with it through plain `needs`. `release` carries
`if: ${{ !cancelled() && !contains(needs.*.result, 'failure') }}`, so it runs past the skipped `publish` and
stops on a failed one. A beta runs `gate`, `test`, `build`, `smoke` and `release`, and leaves three things
behind: the smoked, signed image on `ghcr.io/binacle-labs/binacle-net` under its immutable tag, the git tag
on the run's commit, and a GitHub release flagged prerelease with the `[Unreleased]` section as its body. No
Docker Hub copy, no Docker Hub page.

**Why the tag and the release came back.** Four days under the 2026-09-14 rule produced `3.1.0-beta.1` and
`beta.2` with nothing in git naming them: no tag to check out, no diff between them for the notes, no
notification to anyone watching the repository, and the maintainer looking for tags that were never made.
Every `3.0.0-beta.*` had a prerelease page, so the convention was already what readers expected. The
plumbing was already there - `github-release.sh` flags a hyphenated tag `--prerelease` and
`changelog-section.sh` maps one to `Unreleased` - and only the `needs` edge stopped it running.

**The cost, and it is permanent.** `$ci-cd/decisions/D24`'s ruleset blocks deleting any `v*` tag, so every beta tag stays for
good, eight of them for the 3.0 line's worth. That is accepted: a tag that names what a run built is the
record, and the ruleset is what makes it trustworthy.

**What a beta is for under this shape.** It proves the build and the smoke against the exact commit, and it
gives a person an image to pull and click through before the real dispatch. Whoever checks it pulls from GHCR:
`just smoke all ghcr.io/binacle-labs/binacle-net:<version>` and `docker run` against the same reference. The
`release` job's run summary prints the reference, the digest, the release link and the GHCR form of the verify
command.

**What it does not prove, and the cost accepted with it.** `publish` and `page` never run for a prerelease,
so the first run of any change to them is the real release. That is survivable by design: a red `publish`
leaves Docker Hub untouched and creates no tag, and the tag check in `gate` lets the same version be
dispatched again once fixed. The cost is a red release run rather than a red beta.

**History - this is the fourth answer to the same question.** The skip was introduced on 2026-08-11 and
reversed the same day, on two arguments: it was never a safety rule, because *nothing unsmoked reaches Docker
Hub* comes from `smoke` running before `publish` and holds either way; and it cost deployability, because a
host that cannot route to GitHub's AS36459 could not pull a beta from GHCR. Between then and 2026-09-05 the
guard was metadata-action's alone - a prerelease reached Docker Hub with its immutable tag only, since
`{{major}}.{{minor}}` is skipped for one and `latest=auto` withholds `latest` - observed on Docker Hub on
2026-08-06 after `v3.0.0-beta.1`. **What the reversal underrated was the tag list**: eight `3.0.0-beta.*`
tags sat beside the release until they were deleted by hand (`$ci-cd/decisions/D27`), betas 1 to 4 failed the published verify
command for anyone who tried it, and every beta was a second answer to "which image do I pull". The
deployability argument still holds and is the price: a beta is pullable from GHCR only. **The 2026-09-14
answer** went one step further and dropped the git tag and the release too, on plain `needs`; that is the
step undone here.

**The trap the 2026-08-11 reversal warned about now applies, and is handled.** It said a conditional job
above `release` needs a status condition on `release` or a beta silently gets no GitHub release. That is
exactly the `if:` `release` carries. `!cancelled()` rather than `always()` so a cancelled run does not tag,
and `!contains(needs.*.result, 'failure')` rather than a check on `publish` alone so a red job anywhere above
holds it.

**A prerelease may be dispatched from a `release/*` branch** - the maintainer, 2026-09-14: "...any release in
main mut be beta or normal and any release from branch can only be beta (for now) and only from this pattern
release/*". The rule
in full: `main` may dispatch a release or a prerelease; a `release/*` branch may dispatch a prerelease only;
nothing else may dispatch anything. The release branch is where the betas of a version come from, and
nothing merges to `main` until the last one is clean. Because a dispatch runs the workflow file at its own
ref, the branch's CI changes are proved by its betas before they reach `main`. `check-release-ref.sh` takes
the version and is the whole check. **Since 2026-09-18 the branch must be named after the version** -
`release/v3-1-0` may dispatch `3.1.0-*` and nothing else, so a beta of one version cannot be built from
another version's branch, and a branch that is not `release/v<x>-<y>-<z>` is refused outright. The
maintainer, 2026-09-18: "if possible i'd like toalso gate the branch name to release/version so only those
branched publish beta images instead of all", then "do it".

**Branch builds with no version are not wanted - closed 2026-09-18.** Asked whether he needs them, the
maintainer, 2026-09-18: "no what i might need is enforcing the image on main to either be beta or normal and
then on brach only beta". The idea was an image from any branch under a slug-and-sha tag. A beta from the version's own `release/` branch is the way to try an image, and it
comes with a version, a tag and a verify command that passes. A branch build would sign under a ref no
published command names and leave an image on GHCR forever; the plan that held it was deleted the same day. "For now" on the branch half - a release from a branch is not ruled out
forever, only not today. The image signs under the branch's ref, so
checking it is `just image verify <version> all refs/heads/<branch> ghcr.io/...`; the published command,
anchored on `main`, does not cover it and nothing published names it.

**What the `gate` job still checks for a beta.** The ref, semver, the tag is free, and that
`## [Unreleased]` exists and is not empty. The section check is worth keeping even though nothing publishes
it: it fails in seconds when the changelog is not in shape, which is the cheapest place to learn that.

**The staging push carries the immutable tag only.** The moving tags - `3.1`, `3` and `latest` - are computed
in `publish`, the job that creates them. `3` was added to the tag list on 2026-09-12 so the samples can pin a
line that follows every minor; `$samples` has why.
