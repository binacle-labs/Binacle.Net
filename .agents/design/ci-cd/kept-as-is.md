---
id: ci-cd/kept-as-is
description: CI pieces checked against an official or first-party alternative and kept, so the next review does not check them again
verified: 2026-09-11
check: each named file, action or flag still exists in .github/ or tooling/ci/; the reason given for each still holds against the upstream it names
paths:
  - ".github/**"
  - "tooling/ci/**"
---

# CI - what was checked and kept

Each of these was checked against an official or first-party alternative, in the platform sweep of 2026-08-28
and again on 2026-09-11, and the alternative is worse. Listed so the next review does not check them again.

- **`changed-paths.sh` and its full-history checkout.** The API route is `repos/{repo}/compare/{base}...{head}`,
  and its `basehead` really is the three-dot merge-base comparison. But **the file list caps at 300 and does
  not paginate** - GitHub's own docs say so - and a truncated list reads as "nothing changed", which is the
  worst possible failure for this job. `fetch-depth: 0` costs nothing here either: the packed history is
  11 MiB.
- **`actions/checkout` sparse-checkout and `filter: blob:none` for the small jobs.** Same measurement. There is
  no time to save.
- **`gate.sh`.** GitHub provides no built-in "did every job pass" gate, and feeding it `toJSON(needs)` is
  already the only shape in which the job list cannot drift from `needs:`. Re-checked 2026-09-11: GitHub's own
  docs still say a workflow skipped by a `paths:` filter leaves its checks pending and blocks the merge, and
  still recommend one always-running workflow instead. A skipped *job* reports success. That is exactly the
  shape here - one workflow, a `changes` job, per-job `if:`, one collector - so this stands.
- **`check-version.sh`.** `workflow_dispatch` inputs have no pattern or regex validation; a `type: choice` would
  need every version enumerated. A check in the run is the only option.
- **`peter-evans/dockerhub-description`.** Docker publishes no action for the repository description, and the
  API route needs a JWT exchange - more hand-rolled code than the action replaces.
- **`cloudflare/wrangler-action` and `sigstore/cosign-installer`.** Both first-party, from the org that owns
  the tool. Nothing to change.
- **`extractions/setup-just`.** No first-party alternative exists; the just project points at this one.
- **`npm ci --ignore-scripts` repeated in six places.** Folding it into `setup-node` would make an action named
  for setup also install, which is the surprise the split exists to avoid.
- **`if: ${{ !cancelled() }}` on thirty-odd test steps.** GitHub has no job-level default for `if:`. Repeating
  it is the only way to express it.
- **`jq`, `gh`, `git` and `docker`.** All preinstalled on the runners, and no workflow installs any of them.
  Nothing to delete - this was checked because it is a common waste, and it is not happening here.
- **The staging reference assembled by hand in the `build` job.** `metadata-action`'s `tags` output would give
  the same string today, but only while exactly one tag rule is configured; it is newline-separated and would
  break as a job output the moment a second rule is added. Building it from the image and the version survives
  that.
- **`actions/attest-build-provenance`.** This is the first-party mechanism for what the cosign steps hand-roll,
  and it has one genuine advantage: attestations are stored by GitHub against the digest, so they survive the
  copy to Docker Hub and the image would not need signing twice. **It is still not worth it.** Adopting it
  means rewriting the verify command users are given on four public surfaces at once, and a failed verify after
  such a change reads to a user as tampering. Adding it *beside* cosign is worse again - two verification
  stories for one image. Written down so this does not get proposed as an easy win.
- **`.github/dependabot.yml`'s five near-identical blocks.** A glob in `directories:` for the github-actions
  ecosystem is still not supported and is reported to produce duplicate pull requests; explicit entries per
  action folder remain the recommended shape. Checked 2026-08-28.
- **`just check workflows` printing its file list and count.** actionlint would find the files on its own, but
  it says nothing on a clean run, so the count is what distinguishes a pass from a run that never started.
