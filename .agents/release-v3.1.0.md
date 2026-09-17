---
description: Release - Binacle.Net v3.1.0. The demo UI release - the shipped clients move to v4, the demo gains two things and loses two faults.
---

# Release - Binacle.Net v3.1.0

**Status:** scope narrowed by the maintainer on 2026-09-07, three rows added on 2026-09-15. **Every row of
work has landed** - rows 5 to 8 on 2026-09-15. **Stage 1 and stage 2 closed on 2026-09-16**: the generator
run, both bundles rebuilt, `just test all` green across 28 suites, and the changelog read against every row.
**Every by-eye box passed on 2026-09-17** - rows 5, 6, 7 and 8 - **and row 8's two site halves landed the
same day.** What is left is stage 3. The six CI edits landed on
2026-09-11 and 2026-09-12; two of them are proved only by the `3.1.0` run. The maintainer set the order on
2026-09-11: **features first, then the changelog and the docs, then the rest.** Branch
`release/v3-1-0`. **Every beta is dispatched from this branch and stops at GHCR** - decided 2026-09-14 - so
nothing merges to `main` until the last beta is clean, and the changed `publish` job is first run by the real
tag. **The steps from here to the tag are under *Before the tag*, and what happens after it is
`post-release-v3.1.0.md`.**

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
| 3 | plan trimmed - the ledger has the reasoning | **The unpacked-items tooltip. Landed on both hosts - UI module 2026-09-10, demo site 2026-09-11.** beercss's own `.tooltip`, on an info button inside the row the way the ViPaq delete button sits in its row. No directive and no TypeScript - one `:focus-within` rule covers the keyboard and touch, which beercss's hover-only tooltip does not. A first attempt built a native `popover` and rendered it at the top-left of the viewport; it was thrown away |
| 4 | plan landed and deleted - the reasoning is `D6` in the API decisions ledger | **The whole plan. Landed 2026-09-10.** A `Kernel/Instance/` slice now holds what the instance reports about itself; `FeatureOptions` moved in as `InstanceOptions` with a closed value hierarchy, so the presets sit beside the switched-on features without being counted as one. `_js/instance.js`, its webpack entry and its script tag are gone, and with them the last v4 call made from a browser inside the image |
| 5 | plan landed and deleted - the reasoning is `P3` in the packages decisions ledger | **The request panel. Landed 2026-09-15, UI module only - the maintainer's call on 2026-09-14, reworked the next day to a panel on the right.** A `Request` button on the results card opens a right-side dialog with one `curl` line against the page's own origin and a Copy button where the clipboard API exists. The component exposes `lastRequest` and nothing else changed in the shared package; the dialog and the formatting are the module's own `_js/packing_demo.js`. **Rebuilt into both bundles 2026-09-16; the by-eye paste is still the maintainer's** |

| 6 | no plan - release paperwork, the maintainer added it on 2026-09-15 | **`Best` is the first algorithm in the list and the one selected on load**, both hosts. The UI moved to v4 for it, so it is what a visitor sees first. **Landed 2026-09-15** - the one table in `packingDemo.ts` reordered, two tests changed. **A third test was missed** and left `just test all` red until 2026-09-16; the suite is 387 green now |
| 7 | no plan - release paperwork, the maintainer added it on 2026-09-15 | **One more worked example, and it loads first. Source landed 2026-09-15** as `shared/data/demo-samples/00-two-winners.json` - `00` so it sorts first without renaming the other twenty. Two bins, one item set: `Best` picks FFD on `45x30x25` and BFD on `40x30x30`, each fully packed where the other heuristic is not, measured on the API. WFD cannot win on `compare-bins` - `Best` runs FFD and BFD only there. **Both generators have run** - `demo-samples` 2026-09-15, `vipaq-packed-data` 2026-09-16 - and both bundles carry it |
| 8 | no plan - release paperwork, the maintainer added it on 2026-09-15 | **Known-good sample strings on the ViPaq decoder. The component landed 2026-09-15**, the UI module page with it; the reasoning is `P4` in the packages decisions ledger. Five strings in `packages/binacle-net-ui/src/apps/protocolDecoder/sampleData.ts`, each a packed bin out of `vipaq/data/packed/demo-samples` and held to it by a test; every bin is at least three quarters full and no side is over 60. The component exposes `samples` and nothing else; on `Vipaq.cshtml` a full-width `Samples` button under the add button opens a right-side panel listing each name, its string and a Copy button - no add, on purpose: the visitor copies and pastes. **Chips, then a menu that decoded on click, then a menu that filled the input were each rejected the same day.** **Two halves are site sessions**: the demo site's `vipaq.html` needs the same button and panel, and the docs site's ViPaq protocol page carries the strings below with what each decodes to |

**Row 4 deleted `_js/instance.js`.** The warning that used to stand here - not to edit its v4 call on the
way past - is spent.

**Row 5 was reworked on 2026-09-15** - the panel on the right, not under the results. The `lastRequest` seam
and its tests stayed; the Razor markup and the ledger entry changed with it.

**Rows 5 to 8 are the last features.** Stage 2 under *Before the tag* waits on them.

**What the two site sessions for row 8 must write.** On `sites/demo/pages/vipaq.html`, the same thing that
sits under the add button on `Vipaq.cshtml`: a full-width `Samples` button opening a right `dialog` that lists
`samples` - name, the string in a `pre` the visitor can select, a Copy button - and a Close button. No add.
The component carries the strings; the page's own script holds the open state and the copy, the way
`_js/protocol_decoder.js` does. On the docs site's ViPaq
protocol page, a "Try it" section with these five strings, each with the name and what it decodes to. The
strings are in `sampleData.ts` - copy them from the file, never retype them.

| Name | Bin | Items | Full |
|---|---|---|---|
| Five boxes, packed full | 30x20x20 | one 20x20x20, four 10x10x10 | 100% |
| Ten boxes, two sizes | 60x35x25 | six 25x8x25, two 16x10x25, two 10x16x25 | 88% |
| Twenty-four cubes | 40x30x25 | twenty-four 10x10x10 | 80% |
| Thirteen boxes, mixed sizes | 35x30x25 | one 20x18x16, four 15x15x10, eight 10x10x8 | 81% |
| Eight flat items | 50x50x12 | eight 24x24x5 | 77% |

Each is the `viPaqData` field of a `pack` response for one of the demo's own worked examples, packed with
FFD, and the page must say so, and that the decoder page draws it.

## The two fixes

| Plan | The slice this release takes |
|---|---|
| plan trimmed - the ledger has the reasoning | **The submit button. Landed 2026-09-10.** `onSubmit` awaits the response and clears `submitting` in its own `finally` before dispatching `update-scene`, so the button no longer depends on a visualizer listening. The answer is written at the line it was taken |
| plan landed and deleted | **The whole plan. Landed `9f277ef6` on 2026-09-10.** A 429 now says the caller has been rate limited and to wait; every other empty body says the server sent no details. The parse-failure wording is gone, because an unparseable body throws out of the client before it reaches that branch and lands in the `catch` in `getResults`. **It is a demo-site fix, not an image one** - the limiter is
registered only by the Service Module, so an image running the shipped defaults never answers 429 |

**Both are in files this release already opens**, which is the whole reason they are here rather than in a
release of their own. *(chosen by an agent)*

## Maintenance riding along

| Plan | The slice this release takes |
|---|---|
| `plans/api/integration-tests-cover-shipped-modules.md` | **the CORS assertion. Landed 2026-09-10** in `api/test/Binacle.Net.IntegrationTests` - four tests: preflight and simple request from a configured origin carry the header, an unconfigured origin does not, and with no `Cors.json` no origin is allowed. Proven by breaking `app.UseCors()` and watching the right two fail. **Turning the optional modules on is the larger half and is still open** |
| `plans/ci-cd/ci-open-questions.md` | **the six findings he approved on 2026-09-11 - 1, 4, 5, 8, 10 and 12; 7 is rejected, `D29`.** The plan's answer table says what each yes takes. Two of them, 1 and 8, edit the release `publish` job, and **only a run proves that job** - and since the row below, only the `3.1.0` run itself reaches it. Finding 1 first needs the OIDC connection created on the Docker Hub org - the hand step under *Before the tag* |
| no plan - the maintainer decided it on 2026-09-14 | **A prerelease stops at staging. Landed 2026-09-14.** `publish` carries the one prerelease condition in the file, and `release` and `page` skip with it through `needs`, so a beta leaves a smoked image on GHCR and nothing on Docker Hub, no tag and no release. `D3` has the reasoning and the cost: `publish`, `release` and `page` are first run by the real tag |

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
| `plans/sites/docs-current-at-root.md` | **The whole plan - the maintainer put it in on 2026-09-11. Landed on the branch 2026-09-12**, steps 1-15 of 16. One folder per major, the current one at the site root, the common layer gone, `v2.0.x` and `v2.1.x` merged into `v2.x`, closed lines at `/version/1.3.0/` and `/version/2.1.1/`. Both questions for the maintainer were answered no on 2026-09-12: `version.html` and the sidebar button are gone, the layouts collapsed to one, Configuration Basics folded into Configuration, Integration Guide left the docs, CORS got its own page, and every folder has the same sidebar order. Open in the plan: the deployed-site checks and the 302→301 flip after them. The `## v3.1.0` release-notes section and the `versions.yml` bump are in `post-release-v3.1.0.md` |
| no plan - two lines | **`sites/README.md` names `Deploy Site` and its `site` choice. Landed 2026-09-12** - it had named three workflows that finding 5 folded into `deploy-site.yml` |

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

**`plans/ci-cd/prerelease-staging-repository.md`, what is left of it.** Its prerelease half landed in this
release - the row under *Maintenance riding along*. What remains is branch builds, still an idea with the
signing question open: anything built from a branch signs under that branch's ref and fails the command
printed in `SECURITY.md`. Gating a UI release behind it buys nothing.

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
the docs, then the rest.** Each step needs the one above it. A beta dispatches from this branch and a
release from `main` only, so everything up to and including the last beta happens here, and everything from
the merge on happens on `main`. **A beta can be dispatched at any point on the branch** - the run uses the
branch's own workflow file, so it is the cheap way to see a CI change or a feature on a real image before it
is finished; only the last one has to be clean.

### Stage 1 - the features

- [x] **2026-09-15. Row 5, the request panel, landed** - code, tests, docs, ledger `P3` and the changelog line,
      reworked the same day to a right-side dialog. The plan file is deleted. **The by-eye check is the
      *Done when* box below.**
- [x] **2026-09-15. Row 6, `Best` first**, landed in the shared package; both bundles carry it since 2026-09-16.
- [x] **2026-09-15. Row 7, the first worked example.** Source landed and `just regen demo-samples` run:
      `head -13 packages/binacle-net-ui/src/apps/packingDemo/sampleData.ts | grep -c 00-two-winners` returns 1.
      **`just regen vipaq-packed-data` run 2026-09-16** - `ls vipaq/data/packed/demo-samples | grep -c '^00-'`
      returns 3, one per algorithm, and the other 60 packed files came back byte-identical.
      **It needed a fix first.** The generator's demo-sample list is hardcoded in
      `vipaq/tools/Binacle.ViPaq.PackedDataGenerator/Program.cs`, and the new file was not in it.
- [x] **2026-09-15. Row 8, the ViPaq samples**, landed in the shared package and the module page; ledger `P4`.
      The by-eye check is the *Done when* box below.
- [x] **2026-09-16.** Both bundles rebuilt. **Git cannot see this.** Both output folders are gitignored -
      `.gitignore:73` for `api/src/Binacle.Net.UIModule/wwwroot` and `.gitignore:77` for `sites/demo/js` - so a
      `git diff` or `git status` over them is always empty and proves nothing either way. That is what the
      earlier reading of "no rebuild" was: the check, not the bundles.
      **Grep the built file instead.** `lastRequest`, `00-two-winners` and `Five boxes, packed full` each match
      once in `api/src/Binacle.Net.UIModule/wwwroot/js/binacle-net-ui.js` and once in
      `sites/demo/js/binacle-net-ui.js`. **All three are in both** - the two hosts build the shared package
      from the same source, so `lastRequest` ships in the demo chunk too. What is UI module only is the page
      markup that mounts the panel, not the code.
      Commands: `npm run copy-assets-to-uimodule && (cd api/src/Binacle.Net.UIModule && npm run build)`, and
      `npm run build` inside `sites/demo`. The demo bundle was three features behind - built 2026-09-10,
      before rows 7 and 8.
      The same box is under stage 3 - it has to hold at both points.
- [x] **2026-09-12. The six CI findings landed** - 1, 4, 5, 8, 10 and 12 in `plans/ci-cd/ci-open-questions.md`.
      `grep -c '^- \[x\]'` on that plan returns 12; the one open box, finding 8, closes on the `3.1.0` run.
      **What landed is the workflow edit** - the `publish` half is proved by the `3.1.0` run in stage 3, not
      by the beta and not by anything on the branch.
- [x] **2026-09-14, the maintainer's hand.** The OIDC connection exists on the `binacle` Docker Hub org
      with two rulesets on `binacle/binacle-net`, one per subject form - the immutable
      `repo:binacle-labs@189874141/Binacle.Net@607841255:ref:refs/heads/main` and the plain
      `repo:binacle-labs/Binacle.Net:ref:refs/heads/main` - and `DOCKERHUB_OIDC_CONNECTIONID` is set. The
      shape and why are `D33`. **Unproved until the `3.1.0` dispatch** - a prerelease stops before the
      login. `DOCKERHUB_TOKEN` stays as it is: the token screen offers no repository scoping.
- [x] **2026-09-16.** `just test all` passes - 28 suites, every one green - and `just openapi check-all-copies`
      passes.
      **It did not pass before that day, and the failure was rows 6 and 7's.**
      `tests/apps/packingDemo/samples.test.ts` still asserted `01-opening-set` as the first sample after
      `00-two-winners` took that place. The recipe stops on the first failure, so the 16 suites after it -
      every .NET integration suite and all ten Ruby ones - had not run at all since the beta. They run clean now.
      **A second thing was wrong and nothing had caught it:** the packed-data generator carries its
      demo-sample file list by hand, and `00-two-winners.json` was not in it, so `just regen vipaq-packed-data`
      would have written nothing however often it ran. Both fixed 2026-09-16.

### Stage 2 - the changelog and the docs

- [x] **2026-09-16.** The `## [Unreleased]` section describes the whole release and nothing that did not ship -
      row 5 and the CI work included. **By eye**, read top to bottom against every row above. All eight UI
      rows, both fixes, the CORS test, the five CI findings that were already in it, the `3` tag and the docs
      restructure are all there, and nothing in it names something that did not ship.
      **One line was missing and was added the same day:** finding 8, the builder setup dropped from the
      `publish` job. It is the last of the six CI findings to reach the changelog.
- [x] **2026-09-14.** No line reads as a contract change. *(chosen by an agent - strike it)* The Internal
      Work clause saying the health check payload "lists features by type now, not by key" read as one and is
      cut: the `Features` array is the same list of names it was, and the type filter is internal to
      `InstanceOptions`. The CI rows, the `3` tag and the docs restructure were logged the same day - they had
      landed without an entry.
- [x] **2026-09-16.** `.agents/docs/` says what the tree now does. The `verified:` and `check:` of every doc
      whose paths the release touched are current: `docs/api/modules/ui.md` and `docs/packages/binacle-net-ui.md`
      at 2026-09-15, the three `docs/ci-cd/*.md` and `docs/packages/binacle-net-client.md` at 2026-09-14.
      **By eye** against `git log --stat main..HEAD`. Row 5 is in `ui.md`'s `check:` and in the
      `packing_demo_app` row of `binacle-net-ui.md`; row 8 is in the `/vipaq` route row and the
      `protocol_decoder_app` row.
      **One number was stale and was fixed the same day:** `binacle-net-ui.md` said 384 tests; the suite is
      387, 72.03% of lines. The `verified:` date was left alone - only that one claim was re-measured.
      **The docs site restructure is its own row above and can land in this stage.** Only the `## v3.1.0`
      release-notes section waits for the tag - `post-release-v3.1.0.md` says why.
- [x] **2026-09-14.** *(chosen by an agent - strike it)* `.github/dockerhub-overview.md` describes the `3`
      tag - a row in the tag table, and `{{MAJOR}}` filled by `just image dockerhub-overview`.
      `just image dockerhub-overview 3.1.0 | grep -c '^| \`3\` |'` returns 1. **2026-09-15, the maintainer:**
      the quick start, the verify example and "Pin `{{MAJOR}}` for anything you keep" all say `3`; the minor
      row is for the Service Module, the one thing a minor may break.
- [x] **2026-09-12.** `sites/README.md` names `Deploy Site`, not three workflows.
      `grep -c 'Deploy Docs Site\|Deploy Demo Site\|Deploy WWW Site' sites/README.md` returns 0.

### Stage 3 - the rest

- [ ] Both bundles are rebuilt from the current sources, after the last source edit on the branch.
      `npm run copy-assets-to-uimodule && (cd api/src/Binacle.Net.UIModule && npm run build)`, and the demo
      site's own build for `sites/demo/js`. **Check it with the greps under stage 1, not with git** - both
      output folders are gitignored, so `git status` stays silent whether the build ran or not.
- [ ] **`3.1.0-beta.<n>` dispatched from `release/v3-1-0`, run green - `gate`, `test`, `build` and `smoke`;
      `publish`, `release` and `page` show as skipped.** **`beta.1` ran green on 2026-09-14** from `c0355b26`
      - the branch's own workflow, signed on the branch ref, nothing on Docker Hub. Against it: verify PASS on
      all four checks, all five smoke profiles green, the four pages answer 200 with `UI_MODULE=True`, and the
      bundle carries `compare-bins`, `Try all, keep the best` and `algorithmUsed`. **The box stays open** -
      row 5 is not in it, and the last beta is the one that has to be clean. It proves the branch builds, signs on
      `refs/heads/release/v3-1-0` and passes the smoke; it does not touch Docker Hub, and it makes no tag.
      Then, against the staged image - it is public, no login:
      `just image verify 3.1.0-beta.<n> all refs/heads/release/v3-1-0 ghcr.io/binacle-labs/binacle-net` passes,
      `just smoke all ghcr.io/binacle-labs/binacle-net:3.1.0-beta.<n>` is green, and the four UI pages open
      from `docker run ghcr.io/binacle-labs/binacle-net:3.1.0-beta.<n>` with `UI_MODULE=True` - on `/packing`:
      pick `Best`, read the winner off the row, randomize to `02-packs-nowhere` and open the unpacked list,
      and the request panel prints a call that answers when pasted. **A red run here is the cheap place to
      find out** - fix on the branch, dispatch the next number. **What a beta cannot prove is findings 1 and
      8** - the Docker Hub login and the buildx-less copy run for the first time on the `3.1.0` dispatch
      below. A red `publish` there leaves Docker Hub untouched and no tag; fix on `main` and dispatch `3.1.0`
      again.
- [ ] Pull request from `release/v3-1-0` to `main`, and `Gate` is green. **Watch the Sonar job** - it is the
      first real pull request since `D28`, nothing sets `sonar.pullrequest.*`, and with finding 10 in it goes
      red on a failed quality gate. Neither holds the merge; `sonar` is outside `gate`'s `needs`.
- [ ] Merged. `main` is now the commit the last clean beta was built from, plus the merge.
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

- [x] **2026-09-16.** `CHANGELOG.md` has an `## [Unreleased]` section describing this release.
      `just changelog check Unreleased` passes - 47 lines - and the section now covers every row, rows 5 to 8
      and all six CI findings included. **The read against the rows is the stage 2 box**, which holds the
      detail; this box is the command.
      **It fails by design after the rename to `## [3.1.0]`** - that is the last edit before the tag, under
      stage 3.
- [ ] Every row above is either ticked with a date, or moved out of this file with a reason.
      **By eye.** A row that is neither is the state this file exists to refuse.
- [ ] Every box under *Before the tag* is ticked, in order, and the `3.1.0` run is green.
      **By eye**, and the release page at `github.com/binacle-labs/Binacle.Net/releases/tag/v3.1.0` exists.
- [x] **2026-09-17, confirmed by the maintainer.** Row 8 is in on the UI module: the ViPaq decoder offers
      samples, and each one draws. **By eye** on `/vipaq` - press `Samples`, Copy a string, paste it into the
      input, add, the bin draws. `just test ts_binacle-net-ui_unit` passes the `sampleData` suite inside the
      387-test suite. The demo site's own panel is the box below.
- [x] **2026-09-17.** Row 8's two site halves are in, one site session each.
      `grep -c Samples sites/demo/pages/vipaq.html` returns 2 and
      `grep -c 'Five boxes, packed full' sites/docs/collections/_versions/v3.x/vipaq-protocol.md` returns 1;
      both returned 0 the day before. **No string was retyped on either page** - the demo page loops `samples`
      off the component and holds no base64 of its own, and all five strings on the docs page byte-match
      `sampleData.ts`, checked pair by pair.
      The demo page copies `Vipaq.cshtml` including the nesting - `samples_panel` sits inside
      `protocol_decoder_app`'s scope, which is how `samples` resolves - and its own `_js/protocol_decoder.js`
      holds the open state and the copy. The docs page gained a `## Try it` section between *Example* and
      *Full Specification*: five blocks of name, bin, items and fill, each string in a `text` fence, and a line
      saying each is the `viPaqData` of an FFD `pack` response for one of the demo's worked examples.
      `npm run build` in `sites/demo` and `bundle exec jekyll build` in `sites/docs` both pass.
      **Neither page has been opened in a browser** - the demo panel is the same component and markup the
      module page already passed by eye.
- [x] **2026-09-17, confirmed by the maintainer.** Row 5 is in: the request panel prints the call that was
      sent, against the host the page is served from. **By eye** on the UI module's packing page.
- [x] **2026-09-17, confirmed by the maintainer.** Rows 6 and 7 are in: the page opens on
      `Try all, keep the best` with two result rows naming two winners. **By eye.**
- [x] **2026-09-16.** The docs site renders the current line at the root, with no `v3.1.x` folder opened.
      `ls sites/docs/collections/_versions/` prints `v1.x v2.x v3.x`.
      **The deployed-site half is not a box here.** The three that are still open in
      `plans/sites/docs-current-at-root.md` *Done when* - the redirect checks, the selector click and the
      major-tag manifest - each need the site deployed or the `3` tag published, so none of them can be
      ticked before the tag. `post-release-v3.1.0.md` owns all three.
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
