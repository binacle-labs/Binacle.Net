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

Three things that are easy to get wrong:

- **Neither goes in `shared-image-tests.yml`.** The release calls that file whole and takes no inputs, so
  every step added there is a step the release pays for. They belong in `pull-request.yml`.
- **Coverage must not be blocking yet.** The numbers behind that are in the CI/CD decisions ledger; four
  files carried their own copy and three had gone stale.
- **Coverage sees one storage backend.** `sonar-analysis.yml` pins the service suite to SQLite, so the
  Postgres and Azure provider code is never covered.
