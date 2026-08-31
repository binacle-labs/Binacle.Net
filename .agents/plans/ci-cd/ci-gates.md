---
description: Two gates the pull request does not have - the integration suites against the shipped module set, and a Sonar verdict without a button press
state: idea
waits-on: "nothing to gate yet"
paths:
  - ".github/workflows/**"
---

# Two gates the pull request does not have

**Gate 2.** The integration harnesses run core modules only, so every module combination the image ships is
untested end to end. Writing those tests belongs to the integration-test plan; this is only that they run on
every pull request once they exist.

**Gate 3.** `sonar-analysis.yml` is `workflow_dispatch` only, so analysis happens when somebody remembers,
which is never on the pull request that introduced the problem.

Two things that are easy to get wrong:

- **Neither goes in `shared-image-tests.yml`.** The release calls that file whole and takes no inputs, so
  every step added there is a step the release pays for. They belong in `pull-request.yml`.
- **Whether coverage blocks is a separate decision and not this plan's.** It was argued down on the grounds
  that the condition was red before anyone wrote a line; that stopped being true on 2026-08-31, when the
  fixed gate started passing on `main`. The numbers and the argument are in the CI/CD decisions ledger, in
  one place, because four files carried their own copy and three had gone stale.

## Done when

- [ ] `sonar-analysis.yml` runs without a button press.
      `grep -n 'pull_request' .github/workflows/sonar-analysis.yml` matches, or `pull-request.yml` calls it
      with a `uses:` line naming it.
- [ ] The integration suites run on every pull request, against the module set the image ships.
      **By eye** in `.github/workflows/pull-request.yml`: a job runs the all-modules suite. The harness TODOs
      the integration-test plan owns are gone first, so this box cannot close before that one does.
- [ ] Neither gate was added to `shared-image-tests.yml`.
      `grep -ci 'sonar\|coverage' .github/workflows/shared-image-tests.yml` returns 0.
