---
description: Two things a pull request does not run - the integration suites against the shipped module set, and Sonar, which is dispatch-only
state: idea
waits-on: "nobody - it is an idea. horizon: near - chosen by an agent, strike it if wrong"
horizon: near
paths:
  - ".github/workflows/**"
---

# What the pull request does not run

Two things a pull request does not check. The integration harnesses run core modules only, so every module
combination the image ships is untested end to end on a pull request - and writing those tests is separate
work; this is only that they run once they exist. And `sonar-analysis.yml` is `workflow_dispatch` only, so
analysis happens when somebody remembers, which is never on the pull request that introduced the problem.

Two things that are easy to get wrong:

- **Neither goes in `shared-image-tests.yml`.** The release calls that file whole and takes no inputs, so every
  step added there is a step the release pays for. They belong in `pull-request.yml`.
- **Whether coverage blocks is a separate decision and not this file's.** The numbers and the argument are in
  the CI/CD decisions ledger, in one place, because four files carried their own copy and three had gone stale.

## Done when

- [ ] `sonar-analysis.yml` runs without a button press.
      `grep -n 'pull_request' .github/workflows/sonar-analysis.yml` matches, or `pull-request.yml` calls it
      with a `uses:` line naming it.
- [ ] The integration suites run on every pull request, against the module set the image ships.
      **By eye** in `.github/workflows/pull-request.yml`: a job runs the all-modules suite. The harnesses have
      to have the optional modules on first, so this box cannot close before that work does.
- [ ] Neither gate was added to `shared-image-tests.yml`.
      `git diff` on that file shows no new step.

## Research

### 2026-09-01 - the free plan does not block Sonar on a pull request

The worry was that the SonarCloud free tier cannot analyse a pull request. **It can, for this repository.**
Sonar's subscription-plans page lists pull request analysis on the Free plan as available *"only if the target
branch is the main branch"*, and the free plan covers unlimited analysis of public projects. `pull-request.yml`
triggers on `pull_request` with no branch filter and `main` is the only long-lived branch here, so every pull
request that matters targets `main` and qualifies.

**What the free tier actually restricts is custom quality gates and branch analysis.** Custom gates start at
the Team plan, which is why this project runs the read-only "Sonar way" gate asking 80% on new code. Branch
analysis is main-only, so a pull request targeting anything other than `main` would get nothing. Neither
restriction touches whether analysis runs on a pull request.

**Two things to plan for before adding the trigger, and neither is a Sonar limit:**

- **`secrets.SONAR_TOKEN` is not handed to a `pull_request` run from a fork**, and a Dependabot pull request
  reads from the Dependabot secret store rather than the Actions one. Both cases need answering, or those runs
  fail on an empty token.
- **The recorded reason for no `pull_request` trigger was the coverage condition being red**, and it stopped
  being red on 2026-08-31 when the fixed gate started passing on `main`. That argument is spent.
