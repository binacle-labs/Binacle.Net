---
description: Release - Binacle.Net v3.1.0. The demo UI release - the shipped clients move to v4, the demo gains two things and loses two faults.
---

# Release - Binacle.Net v3.1.0

**Status:** scope narrowed by the maintainer on 2026-09-07. **Rows 1 and 2 and one of the two fixes are in.**
No branch, no beta, no date.

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
| 3 | `plans/api/packing-demo-next.md` | **item 1, the unpacked-items tooltip. UI module landed 2026-09-10.** beercss's own `.tooltip`, on an info button inside the row the way the ViPaq delete button sits in its row. No directive and no TypeScript - one `:focus-within` rule covers the keyboard and touch, which beercss's hover-only tooltip does not. A first attempt built a native `popover` and rendered it at the top-left of the viewport; it was thrown away. **The `sites/demo/` markup is still to write** - a coding session may not touch `sites/`, so the plan carries the exact spec for a site session |
| 4 | plan landed and deleted - the reasoning is `D6` in the API decisions ledger | **The whole plan. Landed 2026-09-10.** A `Kernel/Instance/` slice now holds what the instance reports about itself; `FeatureOptions` moved in as `InstanceOptions` with a closed value hierarchy, so the presets sit beside the switched-on features without being counted as one. `_js/instance.js`, its webpack entry and its script tag are gone, and with them the last v4 call made from a browser inside the image |
| 5 | `plans/api/packing-demo-next.md` | **item 3, the request panel.** *(ordering chosen by an agent - it is last because it is the only row here that can be cut without leaving anything half-done)* |

**Row 4 deleted `_js/instance.js`.** The warning that used to stand here - not to edit its v4 call on the
way past - is spent.

**Row 5 has four questions left and one of them decides its cost** - whether the panel lives inside the shared
component or in the Razor page around it. The version question that used to gate it is answered by row 1.

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

## Done when

- [ ] `CHANGELOG.md` has an `## [Unreleased]` section describing this release.
      `just changelog check Unreleased` passes - it does today, and this box stays open until the last row
      below is in it, because the section has to describe the whole release and not the part that landed first.
- [ ] Every row above is either ticked with a date, or moved out of this file with a reason.
      **By eye.** A row that is neither is the state this file exists to refuse.
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
      growing. Hover, touch and keyboard all reach it.
      **Half done 2026-09-10.** On the UI module, **hover is confirmed by eye** - the panel floats above the
      row and the row keeps its height. Two things left there: tab to the button, and a touch device. Both
      ride on the `:focus-within` rule in `_sass/_components.scss`, because beercss reveals a tooltip on
      hover alone; if either fails, that rule is what is wrong. **The demo site has none of it yet** - the
      markup and the two style rules are specced in the plan for a site session.
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
