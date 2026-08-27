---
description: The suite is split in two and every test now has a step. What is left needs a real run - a pull request that proves each half skips, a Sonar run that executes the gem tests, and the rubocop backlog to be decided on.
state: blocked
waits-on: "a pull request run, and one look at the gems in SonarCloud"
paths:
  - ".github/workflows/**"
  - "tooling/**"
  - "ruby/**"
---

# Every test reaches a pipeline

**Built.** All 26 tests have a step. `shared-image-tests.yml` runs the 16 the Docker image ships;
`shared-site-tests.yml` runs the 15 a Jekyll site ships; five javascript tests are in both because they ship
in both. The pull request gate calls both suites and builds all three sites, the release calls the image
suite, and each site deploy calls the site suite before it deploys. `just test image` and `just test sites`
are in, and the gems report coverage.

What runs where is in the CI/CD doc and why is in the CI/CD decisions ledger. The recipes are in the commands
and tooling docs. None of it is restated here.

**Everything below needs a run to confirm. Nothing below is more code.**

## The pull request halves have only been reasoned about

The filter was rewritten so `ruby/` no longer sets `code` and `sites/` now sets `site`. It moved out of the
workflow into `tooling/ci/changed-paths.sh` on 2026-08-28, and the move was proved by running the old and new
versions over forty commit ranges - they agreed on all forty. **That proves the move, not the filter.** The
filter itself has still only been read, never watched skipping a job.

Two runs settle it, and both are cheap: a branch touching one page under `sites/`, and a branch touching one
file under `ruby/`.

## Sonar ran, found a bug in the gem coverage, and it is fixed

The run of 2026-08-27 died on the first gem: CI installs the bundle into `ruby/vendor/bundle`, so simplecov
was not on the load path. Fixed on 2026-08-28 and all ten gems re-run green here.

**The gems are still at zero in SonarCloud, and the import is not the reason.** No `.rb` file has ever been
indexed, so there is nothing for the coverage to attach to.
`plans/ci-cd/sonar-scope-and-coverage.md` owns that; this file only needs to know the gems executed.

## Rubocop

**The backlog is 42 offences in 17 files**, measured 27 Aug 2026. All but the ten
`Gemspec/DevelopmentDependencies` are autocorrectable. The counts are in the Ruby doc.

**Nothing runs it.** `just check ruby` did until 2026-08-27, when it was deleted - no linting is set up in
this repository. Run it by hand with `cd ruby && bundle exec rubocop`. What is undecided is whether it
attaches to anything at all, and that decision needs the backlog cleared first - autocorrect is one command,
and reading what it changed is the actual work.

## Done when

- [ ] A change under `sites/` builds the sites and checks their links, and skips the image jobs.
      **By eye.** Open a pull request touching one page and read the gate's table.
- [ ] A change under `ruby/` runs the site tests and starts no Postgres.
      **By eye.** Open a pull request touching one gem file and read the gate's table.
- [x] The Sonar run executes the gem tests. First tried 2026-08-27, failed on the load path, fixed
      2026-08-28.
- [ ] The gems are not at zero in SonarCloud.
      **By eye.** Read them after a run. `plans/ci-cd/sonar-scope-and-coverage.md` is where that answer lands.
- [ ] Rubocop comes back clean, and whether it attaches to anything is decided.
      `cd ruby && bundle exec rubocop` exits 0, and a line in the Ruby doc or the CI/CD ledger says where it
      runs, or that it does not.
