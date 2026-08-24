---
description: Ten of the twenty-six test leaves run on no pipeline. Give every leaf a step, group the leaves for a laptop, and add a check so the two lists cannot drift again.
state: ready
waits-on: "nothing"
paths:
  - ".github/workflows/**"
  - "tooling/**"
---

# Every test leaf reaches every pipeline

**Measured 24 Aug 2026.** `just test all` runs 26 leaves. `shared-test-suite.yml` has 18 steps. The
difference is the **ten Ruby gem leaves**, which run on a laptop and on nothing else.

Three pipelines are affected, because two of them call that file whole:

- the pull request gate,
- the release workflow, which calls it as its *"this commit passed CI"* proof,
- the Sonar run, which calls `just coverage all` and so runs the same leaves.

**A green check for a suite nobody executed is worse than a red one.** The release's own CI gate currently
proves less than it claims.

## The Sonar half fails differently, and worse

`sonar-analysis.yml` has no Ruby setup, so the ten leaves cannot even start there. They write no report, and
the coverage table builds its rows **from the reports that exist** - so a missing suite has no row at all.
Nothing turns red and nothing is listed as zero. **The suite is simply absent from the table**, and the run
publishes.

## What to build

**Group recipes in `tooling/tests.just`, and one leaf list per group.** `lib`, `shared`, `vipaq`, `packages`,
`ruby`, `api` - so `just test api` runs every API leaf and `just test ruby` runs all ten gems. `all` is the
sum of those lists, which makes the list of leaves exist **once**.

Every list stays the infra-free set. The `api` group carries ServiceModule on SQLite only; Postgres and
AzureStorage need `just serve services-up` and stay out of it.

**A step per leaf in `shared-test-suite.yml`, the ten Ruby ones included.** Not a step per group - a red check
has to name the suite. That is the maintainer's call, taken 24 Aug 2026, and the design record written when
this lands is what stops it being re-litigated.

**`Setup - Ruby` in `shared-test-suite.yml` and in `sonar-analysis.yml`.** The composite action already
exists; it takes the directory holding the `Gemfile`, which for the gems is `ruby/`, not a site.

**A check: `just check test-steps`.** Compare the leaf list against the workflow's `run: just test <leaf>`
lines and fail on a difference **in either direction**. Put it on the pull request gate's workflow-lint job,
beside the two checks already there.

## What will bite

**Both directions matter, and the second one bites at the worst moment.** A leaf with no step is the hole
above. A step naming a leaf that no longer exists is a `just` error inside the release's own gate, on a run
nobody wants to debug.

**A leaf can take an argument**, and one does - the ServiceModule integration leaf takes its backend. Whatever
carries the list has to pass that through, or a typo falls back to the harness default and reports green for a
backend nobody exercised. Prove it with a bad backend name: it must be rejected, not silently defaulted.

**A group recipe must not stop at the first failure.** One command should report everything that is broken,
the same as CI's `if: !cancelled()`.

**The check has to fail before it can be trusted.** Hide one step from it and confirm it names the leaf.
A check that has only ever passed is a check nobody has tested.

**Adding a group recipe does not settle whether a leaf belongs in `all`.** That is still a judgement per leaf
- it joins only once someone decides it needs nothing brought up.

## What lands where when this is done

- **The decision** - separate steps rather than group steps, and why - goes in the CI/CD decisions ledger.
- **The recipes** - the groups, and the new check - go in the commands doc and the tooling doc.
- **The gem leaves reaching CI** goes in the Ruby doc, which currently says they are leaves and stops there.
- **The line in `todos.md` saying no workflow runs the gem leaves** is deleted.

## Done when

- [ ] Every leaf in the test module has a step in the CI suite, and every step names a real leaf.
      `just check test-steps` passes, and it runs on the pull request gate.
- [ ] The check fails when a step is missing.
      Remove one step locally, run it, confirm it names that leaf, put the step back.
- [ ] The Sonar run can execute the Ruby leaves.
      `grep -c "setup-ruby" .github/workflows/sonar-analysis.yml` is 1.
- [ ] A group runs its slice and reports every failure, not just the first.
      `just test ruby` runs ten suites; break two and confirm both are named.
- [ ] The ServiceModule backend still reaches the leaf through the group machinery.
      Run the leaf with a backend that does not exist. It must be rejected, not defaulted.
- [ ] The leaf list exists in one place.
      **By eye.** Find the second copy. If the workflow is the only other list, that is the one the check covers.
- [ ] The decision and the docs are written, and the `todos.md` line is gone.
      **By eye.** The four files named above.
