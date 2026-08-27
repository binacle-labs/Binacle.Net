---
description: CI - one thing left, and it is a settings page: point branch protection at `Pull Request / Gate`. The composite actions' shell was the other half and it is done
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

## The gap that is now closed — done 2026-08-28

**`actionlint` cannot lint composite actions**, and no flag makes it: hand it an `action.yml` and it reports
`"jobs" section is missing`, because it treats every input as a workflow. Their **inputs** are still checked,
from the caller's side - a missing required input or a misspelled name is reported against the `uses:` line,
naming the action and listing what it accepts.

**Their shell was the only shell in CI that nothing lints: 36 lines.** Four of the five blocks were the
near-identical `install-*` download-and-checksum scripts. They now live in `tooling/ci/install-<tool>.sh` and
`just check scripts` covers them — a recipe that did not exist until the same day, and whose absence was the
larger half of this gap.

The fifth block is `build-jekyll-site`'s `npm ci --ignore-scripts` and `just build "$SITE"`. Both stay inline.
Neither is shell worth checking and a file for each would say nothing.

**One check already sits in `just check actions` by hand**: a grep for a `vars` or `secrets` expression in a
manifest. That is not hypothetical - it failed the first CI run on 2026-08-18, from an expression written
inside an input `description`, because the runner evaluates the whole manifest before any step runs.

## How it was closed

**No tool needed. The action calls a script in the repo.**

    run: tooling/ci/install-lychee.sh

**This works because a local action is read out of the working copy.** `./.github/actions/install-lychee` only
resolves after `actions/checkout`, so the repository is always there by the time the action runs - checked on
2026-08-28 across all 38 uses of a local action, every one of them after a checkout. Nothing has to be
installed first, and it does not go through `just`.

**The version and the checksum moved into the script too, against what this plan first said.** The reason
given here was "where a reader and Dependabot look". Dependabot never looked — it rewrites `uses:` pins, and
these four are hand-pinned binaries it has never touched. Leaving them in the action would have been two homes
for one fact, and a script you cannot run without typing a checksum.

Then the gap closes itself: the shell is a `.sh` file that `shellcheck tooling/ci/*.sh` already covers, and
there is nothing left inside an action to extract. **The extractor this plan used to call for is not needed.**

**A second thing fell out, and it is done too.** `DEVELOPMENT.md` used to spell out four `curl` blocks for
lychee, hurl, actionlint and container-structure-test, repeating every version. It now runs the four scripts,
so each version has one home. Two side effects worth having: the laptop install checks its checksum, which the
hand-written blocks never did, and hurl's checksum provenance - an open unknown until 2026-08-28 - is recorded,
because upstream does publish it and it matches.

**The investigation came back first, and it cleared this.** Nothing official replaces any of the four:
container-structure-test and hurl publish no action, actionlint's own advice is the download script this
already does by hand, and lychee's action is refused for a recorded reason. It also settled the shape — four
scripts, one per tool, not one script taking the tool as an argument. A repeated readable thing beats a shared
clever one.

## Done when

- [ ] Branch protection requires `Pull Request / Gate` and nothing else.
      **By eye.** Open the branch protection settings for `main` and read the required-checks list.
- [ ] No pull request is waiting on a required check that cannot report.
      **By eye.** Open any open pull request; nothing sits pending forever.
- [x] Every composite action's shell is a script under `tooling/ci/`, or a line here says why one is not.
      Done 2026-08-28. The four `install-*` are scripts; `build-jekyll-site`'s two lines stay inline and the
      reason is above. All four were run against a throwaway `HOME`: downloaded, checksum verified, installed.
      `grep -c 'run: |' .github/actions/*/action.yml` returns 0, and `shellcheck tooling/ci/*.sh` is clean.
- [ ] Each install is still the right way to install that tool.
      The investigation pass has reported, and anything it found a supported route for is using it.
