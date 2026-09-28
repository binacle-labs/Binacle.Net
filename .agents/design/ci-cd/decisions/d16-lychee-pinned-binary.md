---
id: ci-cd/decisions/D16
description: lychee is installed as a pinned binary, not through its own action
status: pending
verified: 2026-09-29
check: D16 against .github/actions/install-lychee and deploy-site.yml's link-check step
paths:
  - ".github/actions/install-lychee/**"
  - "tooling/ci/install-lychee.sh"
  - "tooling/check.just"
  - "tooling/check.lychee.toml"
---

# D16 — lychee is installed as a pinned binary, not through its own action

`.github/actions/install-lychee` runs `tooling/ci/install-lychee.sh`, which downloads the release and checks
its SHA-256, and the workflow step is
`just check links <site>`. **`lycheeverse/lychee-action` exists and is maintained, and was still not used.**

**Why:** the action runs lychee itself from `args:` in YAML. The flags that decide what the check *is* —
`--offline`, `--root-dir`, `--config` — would then live in the workflow as well as in `tooling/check.just`, and
the two would drift. `just check links docs` on a laptop and the CI step would stop being the same check while
continuing to look like it. **This is D-nothing-new: it is the first convention in `$ci-cd`** — a step calls a
recipe — applied where the obvious answer pointed the other way.

**What it costs:** a download-and-checksum script, the same shape as hurl's, and lychee's version is bumped by
hand in `tooling/ci/install-lychee.sh` rather than by Dependabot. That is the
same trade already accepted for `hurl` and `container-structure-test`, and the same watch item applies.

**Where the action would win, if it is ever wanted:** a scheduled external run. `just check links-external` is
deliberately not a gate — it reports on other people's servers — and the useful shape for it is a monthly run
that opens an issue, which the action supports directly and a `run:` step does not. Different job, different
tool; that would be an addition, not a reversal of this.

**The check is `--offline` in CI, and that is not a preference either.** Every page carries a `canonical` and
an `og:url` pointing at where it *will* live. Run externally before the deploy, they 404 on every page the
deploy is about to create — 35 of 36 failures on the first real run, all of them self-references. A gate red
for that reason before anyone writes a line is a gate people learn to ignore.
