---
description: CI - the branch protection change waiting on the maintainer, and the composite actions' shell, the one shell in CI that nothing lints
state: blocked
waits-on: "the maintainer - how to make the branch-protection change. The change itself is agreed"
paths:
  - ".github/**"
---

# CI - what is left after the workflow restructure

**Status: the build is done. Landed 2026-08-17 and 2026-08-18**, over two sittings - one review at a time, at
the maintainer's call. The run summaries were finished separately on 2026-08-19, across every workflow that
earns one.

**How it all works now lives in the CI/CD doc**, and why it is shaped that way in the CI/CD decisions ledger.
This file is down to what is *not* done, so it restates neither.

## The one thing needing hands, not code

**Agreed 27 Aug 2026 — this is being done.** What is missing is not the decision but how: the maintainer
wants guidance on making the change before anyone touches the protection settings.

**Point branch protection at `Pull Request / Gate`, and nothing else.** Until then every pull request waits on
a required check that no longer reports: the test suite lost its `pull_request` trigger when the gate started
calling it. **This is the last protection edit that should ever be needed** - every job under `gate` can be
renamed freely afterwards.

## The one real gap, for the workflows session

**`actionlint` cannot lint composite actions**, and no flag makes it: hand it an `action.yml` and it reports
`"jobs" section is missing`, because it treats every input as a workflow. Their **inputs** are still checked,
from the caller's side - a missing required input or a misspelled name is reported against the `uses:` line,
naming the action and listing what it accepts.

**Their shell is the only shell in CI that nothing lints: 36 lines.** Measured 2026-08-28, after the
workflows moved their logic into `tooling/ci/*.sh`. Four of the five blocks are the near-identical `install-*`
download-and-checksum scripts.

The rest is covered. 285 lines of script under `tooling/ci/` go through shellcheck directly, and actionlint
hands the 23 lines still inline in three workflows to shellcheck itself. So this is now the whole gap, not a
share of it.

**It costs nothing today.** All five blocks were extracted by hand on 2026-08-28 and shellcheck reported
nothing. The gap is that no one would know when that stops being true.

**One check already sits in `just check actions` by hand**: a grep for a `vars` or `secrets` expression in a
manifest. That is not hypothetical - it failed the first CI run on 2026-08-18, from an expression written
inside an input `description`, because the runner evaluates the whole manifest before any step runs.

## How to close it - decided 2026-08-28, not built

**No tool needed. The action calls a script in the repo.**

    run: tooling/ci/install-lychee.sh

**This works because a local action is read out of the working copy.** `./.github/actions/install-lychee` only
resolves after `actions/checkout`, so the repository is always there by the time the action runs - checked on
2026-08-28 across all 38 uses of a local action, every one of them after a checkout. Nothing has to be
installed first, and it does not go through `just`.

**The version and the checksum stay in the action's `env:`**, which is where a reader and Dependabot look for
them. Only the shell moves.

Then the gap closes itself: the shell is a `.sh` file that `shellcheck tooling/ci/*.sh` already covers, and
there is nothing left inside an action to extract. **The extractor this plan used to call for is not needed.**

**A second thing falls out.** `DEVELOPMENT.md` tells the maintainer to install lychee, hurl, actionlint and
container-structure-test by hand. Those four scripts are the same install CI does.

**Do the investigation first.** A separate pass is asking, for each of these, whether an official action or a
package manager already does the job, and whether any of the shell is doing more than it needs to. A script
written before that answer is a script deleted after it.

## Done when

- [ ] Branch protection requires `Pull Request / Gate` and nothing else.
      **By eye.** Open the branch protection settings for `main` and read the required-checks list.
- [ ] No pull request is waiting on a required check that cannot report.
      **By eye.** Open any open pull request; nothing sits pending forever.
- [ ] Every composite action's shell is a script under `tooling/ci/`, or a line here says why one is not.
      `grep -c 'run: |' .github/actions/*/action.yml` returns 0, and `shellcheck tooling/ci/*.sh` is clean.
- [ ] Each install is still the right way to install that tool.
      The investigation pass has reported, and anything it found a supported route for is using it.
