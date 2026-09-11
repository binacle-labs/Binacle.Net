---
description: Release - Binacle.Net v3.1.0. The demo UI release - the shipped clients move to v4, the demo gains two things and loses two faults.
---

# Release - Binacle.Net v3.1.0

**Status:** scope narrowed by the maintainer on 2026-09-07. **Two rows of work are left** - row 5, and the
six CI answers - and the maintainer set the order on 2026-09-11: **features first, then the changelog and
the docs, then the rest.** Branch `release/v3-1-0`. A `3.1.0-beta.1` from `main` proves the CI changes
before the real tag. **The steps from here to the tag are under *Before the tag*, and what happens after it
is `post-release-v3.1.0.md`.**

**What this release is.** The shipped UI still calls v3, and the demo has two faults nobody outside would
call a bug but everybody hits. **This release moves the clients to v4 and finishes the demo surface.** It is
UI work plus the fixes and maintenance that ride along with touching those files.

**It is a minor version, so nothing here reshapes an existing contract.** The packing-only image split does
not belong in it for that reason - removing the ServiceModule assemblies changes what a self-hoster pulls.

**v4 stays experimental in this release.** Moving our own UI onto it is the adoption that earns the stable
flip later; the flip itself needs an endpoint added to v4, and that is its own release. See *Not in this
release*.

**This file points; it does not hold work it can point at.** Each row names the plan and the slice this
release takes. The plan holds the work, the traps and the research. **This file holds the order and the
dependencies.** Where a row has no plan, it is release paperwork and lives here because nothing else owns it.

Companion: `post-release-v3.1.0.md` - the checks to run once the image is out, and the work the tag causes.

**The scope is the maintainer's.** Rows marked *(chosen by an agent)* are orderings or readiness calls written
to make the file legible - strike any of them.

---

## Before anything else - done

**`## [Unreleased]` is back in `CHANGELOG.md`**, landed with the first real entry rather than on its own, and
`just changelog check Unreleased` passes. Every row below logs into it as it lands.

---

## What the UI rows stand on

**Both landed 2026-09-09 and both are structural** - no endpoint moved and no page changed behaviour. They
are here because they ship in this version, not because anything is waiting on them.

| Plan | The slice this release takes | Landed |
|---|---|---|
| no plan - it came out of a decision taken in session | **`packages/binacle-net-client`, a private hand-written TypeScript client for v4.** No generator and no runtime dependency. It carries its own committed copy of the v4 OpenAPI document and a test that validates the hand-written types against it, so a contract change in the API fails a test rather than reaching a page. Covers `pack/compare-bins`. `just openapi check-site-copies` became `check-all-copies` and gained `sync-all-copies` to keep that copy in step | `35be5a5e`, `929612d1` |
| plan landed and deleted - the reasoning is in the packages decisions ledger | **`binacle-net-ui` split into `apps/`, `components/` and `shared/`. 19 of its 25 utils were visualizer internals sitting where any file could import them; the visualizer now owns them and exposes its component plus one contract type. No behaviour changed - 348 tests passed before and after with no assertion edited | `f3350689` |

**Both are ticked in *Done when* now.** They were proven by the same thing every UI row is: the two bundles
rebuilt and the pages exercised, done 2026-09-10.

## The UI, in this order

**The order is a dependency, not a preference.** The request panel prints an API version, so it cannot be
written before the clients have moved.

| # | Plan | The slice this release takes |
|---|---|---|
| 1 | plan landed and deleted | **The shipped UI calls v4.** `pack/compare-bins`, through `packages/binacle-net-client` - the component keeps every bin's result and lets the visitor click between them, so the single-bin endpoints do not cover it. **Landed `33a4dfcc`.** Both bundles rebuilt clean and the pages exercised 2026-09-10 |
| 2 | no plan - release paperwork | **The `Best` algorithm, and the algorithm each result used.** `Best` is in the v4 enum and was unreachable from v3, so row 1 is what makes it offerable; `algorithmUsed` is on every v4 result and is rendered nowhere. **One feature, not two** - `Best` runs several heuristics and returns the winner, so without the display the visitor cannot tell what won. Both hosts' result rows. **Landed `a8050583`**, verified with the rebuild on 2026-09-10 |
| 3 | `plans/api/packing-demo-next.md` | **item 1, the unpacked-items tooltip. Landed on both hosts - UI module 2026-09-10, demo site 2026-09-11.** beercss's own `.tooltip`, on an info button inside the row the way the ViPaq delete button sits in its row. No directive and no TypeScript - one `:focus-within` rule covers the keyboard and touch, which beercss's hover-only tooltip does not. A first attempt built a native `popover` and rendered it at the top-left of the viewport; it was thrown away |
| 4 | plan landed and deleted - the reasoning is `D6` in the API decisions ledger | **The whole plan. Landed 2026-09-10.** A `Kernel/Instance/` slice now holds what the instance reports about itself; `FeatureOptions` moved in as `InstanceOptions` with a closed value hierarchy, so the presets sit beside the switched-on features without being counted as one. `_js/instance.js`, its webpack entry and its script tag are gone, and with them the last v4 call made from a browser inside the image |
| 5 | `plans/api/packing-demo-next.md` | **item 3, the request panel. In - the maintainer said so on 2026-09-11, and it gets a session of its own.** The plan carries five questions; the fourth, inside the component or in the Razor page around it, decides the cost and is answered first. The version question is answered by row 1 |

**Row 4 deleted `_js/instance.js`.** The warning that used to stand here - not to edit its v4 call on the
way past - is spent.

**Row 5 is the last feature.** Nothing under *Before the tag* past step 1 starts until it lands.

## The two fixes

| Plan | The slice this release takes |
|---|---|
| `plans/api/packing-demo-next.md` | **item 2, the submit button. Landed 2026-09-10.** `onSubmit` awaits the response and clears `submitting` in its own `finally` before dispatching `update-scene`, so the button no longer depends on a visualizer listening. The answer is written at the line it was taken |
| plan landed and deleted | **The whole plan. Landed `9f277ef6` on 2026-09-10.** A 429 now says the caller has been rate limited and to wait; every other empty body says the server sent no details. The parse-failure wording is gone, because an unparseable body throws out of the client before it reaches that branch and lands in the `catch` in `getResults`. **It is a demo-site fix, not an image one** - the limiter is
registered only by the Service Module, so an image running the shipped defaults never answers 429 |

**Both are in files this release already opens**, which is the whole reason they are here rather than in a
release of their own. *(chosen by an agent)*

## Maintenance riding along

| Plan | The slice this release takes |
|---|---|
| `plans/api/integration-tests-cover-shipped-modules.md` | **the CORS assertion. Landed 2026-09-10** in `api/test/Binacle.Net.IntegrationTests` - four tests: preflight and simple request from a configured origin carry the header, an unconfigured origin does not, and with no `Cors.json` no origin is allowed. Proven by breaking `app.UseCors()` and watching the right two fail. **Turning the optional modules on is the larger half and is still open** |
| `plans/ci-cd/ci-open-questions.md` | **the six findings he approved on 2026-09-11 - 1, 4, 5, 8, 10 and 12; 7 is rejected, `D29`.** The plan's answer table says what each yes takes. Two of them, 1 and 8, edit the release `publish` job, and **only a run proves that job**: after the merge, `3.1.0-beta.1` is dispatched from `main` and its run is the proof. Finding 1 first needs the org checked for an OIDC connection - a login-only fact |

**Why CORS is the one to take even if the rest slips.** `Program.cs` always registers the policy and every
core endpoint requires it, the origins come from an optional `Cors.json`, and with none present the fallback
allows nothing. **Nothing anywhere asserts any of it**, and it is the one thing in this project that has
actually broken in public: a preflight from the demo origin came back without the header, measured 2026-09-01,
fixed 2026-09-02.

**The CORS test does not need the optional modules.** The harnesses boot with an empty pre-build configuration
dictionary and every optional module off - checked 2026-09-07, and the three `// TODO: Run the tests with all
modules enabled` are still in place - but the CORS policy is registered unconditionally, so the assertion can
land before that gap is closed.

**Nothing here goes in `shared-image-tests.yml`.** The release calls that file whole and takes no inputs, so a
step added there is a step every release pays for.

## The docs site

| Plan | The slice this release takes |
|---|---|
| `plans/sites/docs-current-at-root.md` | **The whole plan - the maintainer put it in on 2026-09-11.** One folder per major, the current one at the site root, the common layer gone, `v2.0.x` and `v2.1.x` merged. The gem and tooling half is a coding session; the folder moves are a site session. Everything in it is true before the tag except the `## v3.1.0` release-notes section, which names a date and a link that exist only after the run - that one edit is in `post-release-v3.1.0.md` |
| no plan - two lines, a site session | **`sites/README.md:40-41` names `Deploy Docs Site`, `Deploy Demo Site` and `Deploy WWW Site`**, three workflows that no longer exist - finding 5 folded them into `deploy-site.yml` with the site chosen at dispatch, 2026-09-12. A coding session may not touch `sites/`; whichever site session comes first rewrites those two lines to name `Deploy Site` and its `site` choice |

**Why it rides in a UI release.** The old scheme opens a `v3.1.x` folder the day after the tag and moves every
indexed URL with it. Doing that once more and then restructuring would move the URLs twice. Landing the plan
first means 3.1.0 is the first minor that moves none.

---

## The row that was waiting on an answer - answered

**Sonar on pull request: in, reporting only. Landed 2026-09-10.** `sonar-analysis.yml` gained `workflow_call`
and `pull-request.yml` calls it as a parallel job off the same `code` path filter as the other code jobs. A
fork or Dependabot pull request skips it rather than failing on an empty token, and it is **not** in `gate`'s
`needs`, so it reports and never holds a merge. Whether coverage blocks stays the separate, still-open
question it was.

**Measured, not assumed:** the last nine dispatched runs took 260-357s against a 189s median for today's
longest pull request job, so it becomes the slowest job in the fan-out by roughly two minutes - which costs
no time to merge, because nothing waits on it.

**The reasoning is in the CI/CD decisions ledger as `D28`, not here**, which is what this row's *Done when*
box asked for.

---

## Not in this release, and why

**`plans/api/pack-first-bin-endpoint.md` and `plans/api/v4-stable.md`.** The flip needs an endpoint added to
v4 that reshapes no existing contract, and `pack/first-bin` is the only one costed. **Its selection-only
versus short-circuit question is a one-way door** - whatever ships is the response shape v4 then promises not
to reshape - so it wants a release where it is the subject, not a row at the end of a UI release.
*(reasoning chosen by an agent - strike it)*

**`plans/ci-cd/prerelease-staging-repository.md`.** It has to land before the first prerelease or it has
missed the thing it exists for, and it is still an idea with two unanswered questions, one of which has
already bitten: anything built from a branch signs under that branch's ref and fails the command printed in
`SECURITY.md`. Gating a UI release behind it buys nothing.

**`plans/api/packing-only-image.md` and `plans/api/servicemodule.md`.** Both answered 2026-08-31 and both
`proposed`. The image split changes what a self-hoster pulls, which a minor version may not do. They are the
next major, and the `// TODO` in `ApiUsageRateLimitingPolicy.cs:32` goes with them.

**`plans/api/integration-tests-cover-shipped-modules.md`, the optional-modules half.** It cannot close before
the harnesses have the optional modules on, and that is the half of the row above that is allowed to slip.
The pull-request plan that used to be named here was deleted on 2026-09-11 - the suites already ran on every
pull request, so the only thing left in it was this same half, and it now lives in one file.

**Everything with a `future`, `long`, `on-demand` or `undecided` horizon.** Read `plans/_index.md`.

---

## Before the tag

**Three stages, in the order the maintainer set on 2026-09-11: finish the features, then the changelog and
the docs, then the rest.** Each step needs the one above it. The release dispatches from `main` only, so the
first stage happens on the branch and everything from the merge on happens on `main`.

### Stage 1 - the features

- [ ] **Row 5, the request panel, landed.** A session of its own, started from
      `plans/api/packing-demo-next.md` item 3. The plan's *Done when* has the two checks.
- [ ] **The six CI findings landed** - 1, 4, 5, 8, 10 and 12 in `plans/ci-cd/ci-open-questions.md`, each
      ticked in that plan's *Done when*. Finding 1 starts with the org's Docker Hub settings: no OIDC
      connection, no finding 1. **What lands here is the workflow edit only** - the `publish` half is proved
      by the beta in stage 3, not by anything on the branch.
- [ ] `just test all` passes, and `just openapi check-all-copies` passes.

### Stage 2 - the changelog and the docs

- [ ] The `## [Unreleased]` section describes the whole release and nothing that did not ship - row 5 and
      the CI work included. **By eye.** Read it top to bottom against the rows above.
- [ ] No line reads as a contract change. *(chosen by an agent - strike it)* The Internal Work line saying
      the health check payload "lists features by type now, not by key" reads as one. It is not: the
      `Features` array is the same list of names it was, and the type filter is internal to `InstanceOptions`.
      In a minor release that sentence sends a reader looking for a break that is not there. Cut the clause.
- [ ] `.agents/docs/` says what the tree now does. The `verified:` and `check:` of every doc whose paths the
      release touched are current: `docs/api/modules/ui.md`, `docs/ci-cd/*.md`, `docs/packages/*.md`.
      **By eye** - `git log --stat main..HEAD` lists the paths; each doc's `paths:` says which one owns it.
      **The docs site restructure is its own row above and can land in this stage.** Only the `## v3.1.0`
      release-notes section waits for the tag - `post-release-v3.1.0.md` says why.
- [ ] `sites/README.md` names `Deploy Site`, not three workflows.
      `grep -c 'Deploy Docs Site\|Deploy Demo Site\|Deploy WWW Site' sites/README.md` returns 0.

### Stage 3 - the rest

- [ ] Both bundles are rebuilt from the current sources and the rebuilt output is in the tree.
      `npm run copy-assets-to-uimodule && (cd api/src/Binacle.Net.UIModule && npm run build)`, then
      `git status` shows nothing new under `api/src/Binacle.Net.UIModule/wwwroot/`.
- [ ] Pull request from `release/v3-1-0` to `main`, and `Gate` is green. **Watch the Sonar job** - it is the
      first real pull request since `D28`, nothing sets `sonar.pullrequest.*`, and with finding 10 in it goes
      red on a failed quality gate. Neither holds the merge; `sonar` is outside `gate`'s `needs`.
- [ ] Merged.
- [ ] **`3.1.0-beta.1` dispatched from `main`, run green.** This is what proves findings 1 and 8 - the
      changed `publish` job runs for real, signs on `refs/heads/main`, and copies to Docker Hub under its own
      immutable tag. The `page` job skips on the hyphen. Then, against the published beta:
      `just image verify 3.1.0-beta.1` passes, `just smoke all binacle/binacle-net:3.1.0-beta.1` is green,
      and the four UI pages open from it with `UI_MODULE=True` - on `/packing`: pick `Best`, read the winner
      off the row, randomize to `02-packs-nowhere` and open the unpacked list, and the request panel prints
      a call that answers when pasted. **A red run here is the cheap place to find out** - fix on `main`,
      dispatch `beta.2`.
      **The beta lands in `binacle/binacle-net` and stays** - `D27` says a published version is never
      deleted, and the staging repository that would take it instead is still an idea.
- [ ] `Deploy Site` dispatched from `main` with `demo`, green, and `demo.binacle.net/packing` packs with `Best`.
      **This is also the first run of `deploy-site.yml` and of `create-tag.sh`** - the fold and the API tag
      are both unproved until it.
      **Safe before the real image** - the v3.0.0 image already serves `pack/compare-bins`, `Best` and
      `algorithmUsed` on v4, read off the committed `v3.0.x` swagger copy on 2026-09-11. It is also the
      first time the v4 client runs against the public API.
- [ ] `## [Unreleased]` is `## [3.1.0] - <date>` on `main`. **The last edit before the tag.**
      `just changelog check 3.1.0` passes, and `just changelog extract 3.1.0` prints what will be the release
      body. Read it once.
- [ ] Actions -> Build and Release Docker Image -> Run workflow, on `main`, version `3.1.0`. Everything after
      the dispatch is automatic: gate, tests, build, smoke on staging, copy to Docker Hub under `3.1.0`,
      `3.1`, `3` and `latest`, signature, the GitHub release built from the `3.1.0` section, the tag, and the
      Docker Hub page. Watch the run, then open `post-release-v3.1.0.md`.

**`3.1` and `3` are new tags and `latest` moves.** `3.0` keeps pointing at `3.0.0` and every sample and README still
names it until the post-release pin move - which is deliberate: a pin on `main` must name an image that
already exists, so the pin follows the publish and never precedes it.

---

## Done when

- [ ] `CHANGELOG.md` has an `## [Unreleased]` section describing this release.
      `just changelog check Unreleased` passes - it does today, and this box stays open until the last row
      below is in it, because the section has to describe the whole release and not the part that landed first.
- [ ] Every row above is either ticked with a date, or moved out of this file with a reason.
      **By eye.** A row that is neither is the state this file exists to refuse.
- [ ] Every box under *Before the tag* is ticked, in order, and the `3.1.0` run is green.
      **By eye**, and the release page at `github.com/binacle-labs/Binacle.Net/releases/tag/v3.1.0` exists.
- [ ] Row 5 is in: the request panel prints the call that was sent, against the host the page is served from.
      **By eye.** Open the packing page on a running container, submit, paste what the panel prints into a
      terminal. It answers.
- [ ] The docs site renders the current line at the root, with no `v3.1.x` folder opened.
      Every box in `plans/sites/docs-current-at-root.md` *Done when* is ticked except the release-notes one,
      and `ls sites/docs/collections/_versions/` prints `v1.x v2.x v3.x`.
- [ ] The six CI findings are in and the seventh is recorded.
      `grep -c '^- \[x\]' .agents/plans/ci-cd/ci-open-questions.md` returns 13 - every box in that plan.
- [x] **2026-09-10.** The demo bundles the client, and no chunk is published that nothing loads.
      `ls api/src/Binacle.Net.UIModule/wwwroot/js` lists no `binacle-net-client*` file, because both hosts
      list their chunks by hand and the client rides in the `binacle-net-ui` chunk.
- [x] **2026-09-10.** The restructured package still exports exactly the two plugins.
      `cat packages/binacle-net-ui/index.ts` shows both `export` lines and both `/// <reference` lines.
- [x] **2026-09-10.** The shipped UI calls v4 on both hosts.
      `grep -n 'api/v3' packages/binacle-net-ui/src/apps/packingDemo/packingDemo.ts` returns nothing, with both bundles
      rebuilt.
- [x] **2026-09-11, confirmed by the maintainer.** A partial result names the items it could not fit, and the result row keeps its height.
      **By eye.** Randomize to `02-packs-nowhere` and reach the unpacked items from the row without the row
      growing. Hover, touch and keyboard all reach it. Both hosts - `grep -c hasUnpackedItems` returns 1 on
      `sites/demo/pages/packing.html` and 1 on `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml`.
- [x] **2026-09-10.** The demo offers `Best`, and it is not confusable with `Best Fit Decreasing`.
      **By eye** in the algorithm dropdown. Two entries a visitor cannot tell apart is the failure here.
- [x] **2026-09-10.** Every result says which algorithm actually ran, by name and not by code.
      **By eye.** Pack with `Best` selected and read the winner off the result row. `BFD` on the page rather
      than a friendly name means the box is open.
- [x] **2026-09-10.** The instance page renders its presets without a browser fetch.
      `test ! -f api/src/Binacle.Net.UIModule/_js/instance.js`, and no `instance:` entry in that module's
      `webpack.config.js`. Both hold, the bundle was rebuilt so `wwwroot/js` no longer emits it, and
      `just test all` passes. **The presets are a startup snapshot** - deliberate, and written at the fill
      point in `Program.cs` as well as in the ledger.
- [x] **2026-09-11, confirmed by the maintainer.** The submit button cannot stay disabled when no visualizer is listening.
      **By eye.** Render the demo component on a page with no visualizer, submit, and the button comes back.
      **Code landed 2026-09-10** with tests covering it; the by-eye pass is what is left.
- [x] **2026-09-10.** A rate-limited request does not tell the visitor the reply could not be parsed.
      **By eye** in the errors dialog against a forced 429 - on a host with the Service Module on, because
      the shipped image defaults register no limiter and never answer 429.
- [x] **2026-09-10.** CORS is asserted in a test.
      `grep -rn "Access-Control-Allow-Origin" api/test` matches. `just test cs_binacle-net_integration` - 662 passed.
- [x] **2026-09-10.** Sonar on pull request is answered yes or no, and the answer is somewhere other than
      this file. Answered in, reporting only; recorded as `D28` in the CI/CD decisions ledger.
      **One thing is unproven until the first real pull request** - nothing sets `sonar.pullrequest.*` and the
      run relies on the scanner reading the pull request context itself. Watch the first run.

**Delete this file once v3.1.0 is out and verified.** What outlives it goes to the docs and the decision
ledgers, not here.
