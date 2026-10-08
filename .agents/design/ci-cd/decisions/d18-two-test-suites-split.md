---
id: ci-cd/decisions/D18
description: two test suites, split by what ships
status: pending
verified: 2026-09-29
check: D18 against the test lists in tooling/tests.just and the steps in shared-image-tests.yml and shared-site-tests.yml, which must together name every test and share exactly the javascript ones that ship in both, and against deploy-site.yml's first job
paths:
  - ".github/workflows/shared-image-tests.yml"
  - ".github/workflows/shared-site-tests.yml"
  - "tooling/tests.just"
  - "tooling/ci/changed-paths.sh"
---

# D18 — two test suites, split by what ships

**Dated 2026-08-27.** `shared-image-tests.yml` runs the tests for what ends up in the Docker image.
`shared-site-tests.yml` runs the tests for what ends up in a Jekyll site. The release pipeline calls the first;
the site deploy calls the second; the pull request gate calls both.

**Why:** a release should run the tests for what it releases. The ten Jekyll plugins under `ruby/` cannot reach
the image, so a release paying for them buys nothing, and every step added to the suite the release calls is a
step every release pays for. The mirror was worse: the gems ran on no pipeline at all, so a broken plugin was
first seen half way through a deploy.

**The cut is one rule, not a judgement per test: the image gets the .NET tests plus the javascript tests, and
a site gets everything that is not .NET.** Checked against the manifests on 2026-08-27 — every site pulls all
five javascript packages, `demo` through `binacle-net-ui` to `binacle-vipaq` to `binacle-compact-notation`, all
three through `theme-switcher` to `cookies`. **`binacle-net-service-client` is the one on the site side only** -
added 2026-09-18, bundled by `sites/admin` alone, which is local only; the image never carries it.

**The javascript tests the image ships are in both files, and that is not duplication to remove.** They ship
in the image and they ship in the sites, so both sides prove them. Together the two files name every test in
`tooling/tests.just`.

**The pull request gate's path filter overlaps for the same reason — recorded 2026-08-28.** `just ci
changed-paths` prints `code=` and `site=`, and `packages/`, `shared/` and `vipaq/` set both. That is right:
those directories ship in the image and in the sites, so a change to one has to prove both.

**A site deploy runs the site suite as its own first job**, for the same reason the release runs the image
suite: a deploy is dispatched from any branch, so nothing guarantees the commit passed the gate.

**Steps, never a group recipe, in either file.** `just test image` and `just test sites` exist so one command
runs a slice on a laptop; a red check has to name the suite, which a group recipe cannot do.
The lists and the steps are written out by hand and nothing checks one against the other. That is the
trade: a list you can read, against a test that runs on a laptop and never in CI if somebody forgets the
step. `just check test-steps` did the checking until 2026-08-27, when it was deleted — a check about the
contents of another file in this repository is not a check on anything real.

**A step's `name:` is the assembly, package or gem in full; its `run:` is the test derived from that name.**
Renamed on 2026-08-27. Before, a step said `Test - API Kernel (Unit)` and ran `just test api-kernel-unit`,
which was a third name for `Binacle.Net.Kernel.UnitTests` — three names for one suite, none of them derivable
from another. The test is now `cs_binacle-net-kernel_unit`, and the rule for building it is at the top of
`tooling/tests.just`. The tests are `[private]`, so a laptop's completion offers the three groups; a
private recipe still runs by name, which is all a step needs.

**What this does not fix.** The shared javascript tests run twice on a pull request that touches
`packages/`. That is two jest runs of a few seconds each, against a suite in each file that is honest about
what it covers.
