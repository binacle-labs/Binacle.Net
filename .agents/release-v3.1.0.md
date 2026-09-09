---
description: Release - Binacle.Net v3.1.0. The demo UI release - the shipped clients move to v4, the demo gains two things and loses two faults.
---

# Release - Binacle.Net v3.1.0

**Status:** scope narrowed by the maintainer on 2026-09-07. **Nothing started.** No branch, no beta, no date.

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

**The scope is the maintainer's.** Rows marked *(chosen by an agent)* are orderings or readiness calls written
to make the file legible - strike any of them.

---

## Before anything else

**`## [Unreleased]` has to go back into `CHANGELOG.md`.** The v3.0.0 release renamed it. Nothing can be logged
for this version until it is there, and `just changelog check Unreleased` fails on a missing section and on an
empty one alike, so it goes in with the first real entry rather than on its own.

---

## What the UI rows stand on

**Both landed 2026-09-09 and both are structural** - no endpoint moved and no page changed behaviour. They
are here because they ship in this version, not because anything is waiting on them.

| Plan | The slice this release takes | Landed |
|---|---|---|
| no plan - it came out of a decision taken in session | **`packages/binacle-net-client`, a private hand-written TypeScript client for v4.** No generator and no runtime dependency. It carries its own committed copy of the v4 OpenAPI document and a test that validates the hand-written types against it, so a contract change in the API fails a test rather than reaching a page. Covers `pack/compare-bins`. `just openapi check-site-copies` became `check-all-copies` and gained `sync-all-copies` to keep that copy in step | `35be5a5e`, `929612d1` |
| plan landed and deleted - the reasoning is in the packages decisions ledger | **`binacle-net-ui` split into `apps/`, `components/` and `shared/`. 19 of its 25 utils were visualizer internals sitting where any file could import them; the visualizer now owns them and exposes its component plus one contract type. No behaviour changed - 348 tests passed before and after with no assertion edited | `f3350689` |

**Neither is ticked in *Done when* yet.** Both are proven by the same thing every UI row is: the two bundles
rebuilt and the pages exercised.

## The UI, in this order

**The order is a dependency, not a preference.** The request panel prints an API version, so it cannot be
written before the clients have moved.

| # | Plan | The slice this release takes |
|---|---|---|
| 1 | plan landed and deleted | **The shipped UI calls v4.** `pack/compare-bins`, through `packages/binacle-net-client` - the component keeps every bin's result and lets the visitor click between them, so the single-bin endpoints do not cover it. Landed `33a4dfcc`. **Both bundles still have to be rebuilt**, which is what this row waits on |
| 2 | no plan - release paperwork | **The `Best` algorithm, and the algorithm each result used.** `Best` is in the v4 enum and was unreachable from v3, so row 1 is what makes it offerable; `algorithmUsed` is on every v4 result and is rendered nowhere. **One feature, not two** - `Best` runs several heuristics and returns the winner, so without the display the visitor cannot tell what won. Both hosts' result rows. *(placed after row 1 by an agent because it depends on it - strike the placement, not the row)* |
| 3 | `plans/api/packing-demo-next.md` | **item 1, the unpacked-items tooltip.** Needs no decision - the four helpers and their ten tests already exist |
| 4 | `plans/api/uimodule-instance-presets.md` | the whole plan. It deletes `_js/instance.js` and its webpack entry, and removes the last v4 call made from a browser inside the image |
| 5 | `plans/api/packing-demo-next.md` | **item 3, the request panel.** *(ordering chosen by an agent - it is last because it is the only row here that can be cut without leaving anything half-done)* |

**Do not edit the v4 call in `_js/instance.js` on the way past.** Row 4 deletes the file.

**Row 5 has four questions left and one of them decides its cost** - whether the panel lives inside the shared
component or in the Razor page around it. The version question that used to gate it is answered by row 1.

## The two fixes

| Plan | The slice this release takes |
|---|---|
| `plans/api/packing-demo-next.md` | **item 2, the submit button.** Confirmed live 2026-09-07: `submitting` is cleared only in the `finally` inside the thunk handed to `$dispatch('update-scene', …)`, and nothing runs that thunk unless a visualizer is listening |
| `plans/api/rate-limit-error-is-unreadable.md` | the whole plan. A 429 tells the visitor the reply could not be parsed, which is not what happened |

**Both are in files this release already opens**, which is the whole reason they are here rather than in a
release of their own. *(chosen by an agent)*

## Maintenance riding along

| Plan | The slice this release takes |
|---|---|
| `plans/api/integration-tests-cover-shipped-modules.md` | **the CORS assertion at minimum.** A configured origin comes back in `Access-Control-Allow-Origin`, an unconfigured one does not. Turning the optional modules on is the larger half and can slip *(split chosen by an agent)* |
| `plans/ci-cd/ci-open-questions.md` | **a part of it, his pick.** Seven of its twelve items are open, six of those close on a sentence and need no work; one needs a dispatch |

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

---

## One row waiting on an answer

**Sonar on pull request - in or out?** `sonar-analysis.yml` is `workflow_dispatch:` only, and the recorded
reason for it - a red coverage condition - stopped being true on 2026-08-31. It was called in and then called
out in the same sitting, so it is written here rather than assumed either way.

**What it costs, so the answer is made against the real number:** that workflow does a full build plus every
suite under coverage, with a 45-minute timeout. It is the slowest thing in the repository and every pull
request would pay it. Two other things need answering before it could be wired at all - a run from a fork is
not handed `SONAR_TOKEN`, and a Dependabot run reads from the Dependabot secret store rather than the Actions
one. `plans/ci-cd/what-the-pull-request-does-not-run.md` holds the detail.

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

**`plans/ci-cd/what-the-pull-request-does-not-run.md`, the integration-suite half.** It cannot close before
the harnesses have the optional modules on, and that is the half of the row above that is allowed to slip.

**Everything with a `future`, `long`, `on-demand` or `undecided` horizon.** Read `plans/_index.md`.

---

## Done when

- [ ] `CHANGELOG.md` has an `## [Unreleased]` section describing this release.
      `just changelog check Unreleased` passes.
- [ ] Every row above is either ticked with a date, or moved out of this file with a reason.
      **By eye.** A row that is neither is the state this file exists to refuse.
- [ ] The demo bundles the client, and no chunk is published that nothing loads.
      `ls api/src/Binacle.Net.UIModule/wwwroot/js` lists no `binacle-net-client*` file, because both hosts
      list their chunks by hand and the client rides in the `binacle-net-ui` chunk.
- [ ] The restructured package still exports exactly the two plugins.
      `cat packages/binacle-net-ui/index.ts` shows both `export` lines and both `/// <reference` lines.
- [ ] The shipped UI calls v4 on both hosts.
      `grep -n 'api/v3' packages/binacle-net-ui/src/apps/packingDemo/packingDemo.ts` returns nothing, with both bundles
      rebuilt.
- [ ] A partial result names the items it could not fit, and the result row keeps its height.
      **By eye.** Randomize to `02-packs-nowhere` and reach the unpacked items from the row without the row
      growing. Hover, touch and keyboard all reach it.
- [ ] The demo offers `Best`, and it is not confusable with `Best Fit Decreasing`.
      **By eye** in the algorithm dropdown. Two entries a visitor cannot tell apart is the failure here.
- [ ] Every result says which algorithm actually ran, by name and not by code.
      **By eye.** Pack with `Best` selected and read the winner off the result row. `BFD` on the page rather
      than a friendly name means the box is open.
- [ ] The instance page renders its presets without a browser fetch.
      `test ! -f api/src/Binacle.Net.UIModule/_js/instance.js`, and no `instance:` entry in that module's
      `webpack.config.js`.
- [ ] The submit button cannot stay disabled when no visualizer is listening.
      **By eye.** Render the demo component on a page with no visualizer, submit, and the button comes back.
- [ ] A rate-limited request does not tell the visitor the reply could not be parsed.
      **By eye** in the errors dialog against a forced 429.
- [ ] CORS is asserted in a test.
      `grep -rn "Access-Control-Allow-Origin" api/test` matches.
- [ ] Sonar on pull request is answered yes or no, and the answer is somewhere other than this file.
      **By eye.** If it is only here, it dies with this file at the tag.

**Delete this file once v3.1.0 is out and verified.** What outlives it goes to the docs and the decision
ledgers, not here.
