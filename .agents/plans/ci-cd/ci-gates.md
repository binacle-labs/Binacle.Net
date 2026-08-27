---
description: Two gates the pull request still does not have - the integration suites against the module set the image ships, and a Sonar verdict without anyone pressing a button
state: idea
waits-on: "nothing to gate yet. The maintainer called both a future idea on 2026-08-27"
paths:
  - ".github/workflows/**"
---

# Two gates the pull request does not have

**Gate 1 landed on 2026-08-18** - the workflow shape, the `changes`/`gate` pair, and the image build on every
pull request. How that works is in the CI/CD reference doc and why it is shaped that way is in the CI/CD
decisions ledger; neither is restated here.

**Both gates below are a future idea, and neither has anything to gate yet.** They are written down so the
reason is not re-derived.

## What they share

- **A gate that does not match what ships proves nothing.** The shipped module set and the coverage floor both
  have to be the real ones.
- **Runtime is the shared budget.** The integration suite is already the long pole, and all-modules plus
  coverage each make it longer. Know the total before adding the second gate.
- **Neither goes in `shared-test-suite.yml`.** The release calls that file whole and takes no inputs, so every
  step added there is a step the release pays for. They belong in `pull-request.yml`, beside the image build.

## Gate 2 - the integration suites with all modules on

The harnesses run **core modules only**, so every module combination the image ships is untested end to end.

**Writing those tests is not this.** The integration-test plan owns the decisions - one run with everything
on or a matrix, where the rate-limit tests live, and what breaks when the modules go on. What belongs here is
only that once those leaves exist they run on every pull request like the rest. If the answer is a matrix,
the runtime budget above is what decides how wide.

## Gate 3 - a Sonar verdict without a button press

`sonar-analysis.yml` is `workflow_dispatch` only, so analysis happens when somebody remembers, which is never
on the pull request that introduced the problem.

**Coverage must not be blocking yet, and that half is already decided** - the ledger's open entry on the
pull-request gate carries it, with the numbers. Do not re-derive them here; four files were carrying their own
copy and three had gone stale.

**When the floor is finally set, set it from a run that has settled.** No Sonar run has happened since the UI
suites landed, so nobody has seen the new numbers arrive there. A floor nobody agreed on gets waived the first
time it blocks something.

**Coverage sees one storage backend.** `sonar-analysis.yml` pins the service suite to SQLite, so its coverage
never reaches the Postgres or Azure provider code. Covering those means running that leaf per backend, which
the coverage recipes do not do.

## Done when

- [ ] The integration suites run against the module set the image ships, and the three harness TODOs are gone.
      `grep -rn "Run the tests with all modules enabled" api/test` returns nothing, and those leaves have a
      step in `pull-request.yml`.
- [ ] A pull request gets a coverage number and a Sonar verdict without anyone pressing a button.
      **By eye.** `sonar-analysis.yml` carries a `pull_request` trigger, or `pull-request.yml` calls it.
- [ ] The coverage floor, when one is set, came from a settled run.
      **By eye.** The number is written in the ledger beside the run it came from, not chosen.
