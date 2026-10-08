---
id: ci-cd/decisions/D31
description: the site half of the path filter names the .github/ files a site depends on
status: decided
verified: 2026-09-29
paths:
  - "tooling/ci/changed-paths.sh"
---

# D31 — the site half of the path filter names the `.github/` files a site depends on

**Decided (the maintainer, 2026-09-11):** "Approve, narrowed to site files (Recommended)" - picked in a question
prompt, to "Narrow the site half of the path filter so a workflow edit stops building all three Jekyll sites?"

`changed-paths.sh` used to set `site=yes` for anything under `.github/`, so a workflow-only pull request built all
three Jekyll sites and ran the site suite. It now matches `.github/actions/` and the workflows a site
build or test runs through - `pull-request.yml`, `shared-site-tests.yml` and `deploy-site.yml` - and nothing else
there.

**Why named files and not `.github/actions/` alone.** The narrower pattern leaves a hole: an edit to the site
test workflow, or to the pull request workflow whose `site-build` job does the building, would not run the
jobs it changed. Naming those files keeps the saving and closes the hole.

**What it saves, honestly.** Three Jekyll builds with link checks and one test suite on a workflow-only pull
request. The `code` half still matches every `.github/` file, so that pull request still runs the image tests,
the image build, the lint job and Sonar. It does not become cheap; it stops doing the one thing that could
not have found anything.
