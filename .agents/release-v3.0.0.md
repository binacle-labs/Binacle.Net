---
description: Release - Binacle.Net v3.0.0. A test release first, then the real one. Everything that goes inside the image is done before the test release.
---

# Release - Binacle.Net v3.0.0

**Status:** Betas 1 to 4 published. **`v3.0.0-beta.5` has not been cut and is the next act.** The pipeline is
proven end to end, the architecture branch is merged, the suite is green, and the OpenAPI contracts are proven
unmoved.

**Why there is a beta.5 and it is not optional.** `v3.0.0-beta.4` was tagged on 2026-08-20 and **247 files
have changed since**, 176 of them under `api/`, +2378 / -13149. The UI module was rebuilt, the beercss vendor
assets left the tree, and the `cookies` and `theme-switcher` packages were rewritten in TypeScript. Every
statement in this file that names `3.0.0-beta.4` - the verify run, the Docker Hub response, the browser read -
describes an image the current tree no longer produces. **beta.5 is the rehearsal that makes the real tag a
formality.**

**Created:** 2026-07-16. **Rewritten for the GHCR pipeline:** 2026-08-11. **Scope reset:** 2026-08-14.
**Pruned to pending work only:** 2026-08-20. **Cut to the image and the tag:** 2026-08-25.
**Split into two gates around beta.5:** 2026-08-26.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to
the reference rules: it may point at any file to coordinate the release, and **nothing points back at it**.
Delete it once v3.0.0 is out and verified.

Companion: `post-release-v3.0.0.md` - the work v3.0.0 causes, and the checks to run once it is out.

---

## How to work this file

**This file points; it does not hold work it can point at.** Each row names the plan and the part of it this
release takes. **The plan holds the work; this file holds the order and the traps.** Where a row has no plan,
it is release paperwork and it lives here because nothing else owns it.

**Two gates, and the test for each is different.**

| Gate | The test that puts a row on it |
|---|---|
| **Gate A - before the test release `v3.0.0-beta.5`** | **It goes inside the image.** An image cannot be corrected once it is pushed, and the test release is the last cheap rehearsal of the one v3.0.0 publishes |
| **Gate B - before the real release `v3.0.0`** | **It needs the test image to exist first**, or **the tag does it once and cannot do it again** - the release notes, the Docker Hub page |

**Everything else is post-release or a plan.** Something that can be fixed the day after costs a day, not a
release.

**A finished item is a tick and a date, with the detail gone.** What outlives the release is in the docs and
the decision ledgers, not here.

**How the pipeline works**, because it shapes every row. Every build goes to a staging registry first, is run
and checked there, and only the copy that passed is **copied** to Docker Hub - so nothing unchecked reaches
the registry people pull from. A test release gets its own exact name only, never `3.0` or `latest`, and the
Docker Hub page job is skipped for it. The release notes are lifted out of `CHANGELOG.md` by the workflow.

**Three deploy workflows exist and this release dispatches none of them.** The docs deploy is caused by the
tag and belongs to `post-release-v3.0.0.md`. The demo and www deploys are not release work at all.

**Which published tags are safe to name in an example.** Betas 1 and 2 are deleted from Docker Hub. Only
`3.0.0-beta.3` and `3.0.0-beta.4` resolve today, `3.0.0-beta.5` once it publishes, `3.0.0` once it exists.
**Nothing else.**

---

## Gate A - before the test release `v3.0.0-beta.5`

| # | Item | Where the work is | State |
|---|---|---|---|
| A1 | Check the light and dark switch works, in a real browser | here | done - 2026-08-27. **All seven checks read in a browser, A4's four and A6's three** |
| A2 | Check the demo starts in the same mode as the visitor's computer | here | done - 2026-08-25 |
| A3 | Check version 4 of the API is still marked experimental | here | done - 2026-08-26 |
| A4 | Fix the packing demo bugs that go inside the image | `plans/api/packing-demo-bugs.md` - the part below | done - 2026-08-27. **Its four by-eye checks passed on A1 the same day** |
| A5 | Check the changelog names every image change since the last test release | here | done - 2026-08-26 |
| A6 | Swap the demo's random roll for a hand-picked sample set | here | done - 2026-08-26. **Its three by-eye checks passed on A1 on 2026-08-27** |
| A7 | Cut the test release - dispatch the release workflow with version `3.0.0-beta.5` | here | open |

### A1. Check the light and dark switch works, in a real browser

- [x] **The four checks were read on all four hosts, 2026-08-25.** No wrong-theme flash, the machine is
      followed with JavaScript off, the switcher works from a keyboard and announces itself, and it sits right
      in each header.
- [x] **Re-read after the default changed to `"system"` - 2026-08-25.** No wrong-theme flash on the module's
      first paint with the `theme` cookie cleared.
- [x] **Re-read after A4 - 2026-08-27.** The read above was valid against commit `596b6130` and A4 changed
      the packing page. The page was loaded again and **all four checks A4 could not make without a browser
      pass**: add a bin and submit returns a result, clear all and submit is caught inline, a failed request
      empties the 3D view, and a result row is reached with Tab and activated with Enter.
- [x] **The page opens on the same sample every time - 2026-08-27.**
      **By eye:** reload a few times - it is always sample `01-opening-set`, three bins and two item types.
- [x] **Randomize moves to a different sample - 2026-08-27.**
      **By eye:** press it several times - it never lands on the sample already on screen.
- [x] **There is one Randomize button where there used to be two - 2026-08-27.**
      **By eye:** count them on the page.

### A2. Check the demo starts in the same mode as the visitor's computer

- [x] **Decided 2026-08-25: every surface follows the machine.** `UIModuleOptions.cs` and `ModuleDefinition.cs`
      both say `"system"`, which is what the switcher package already defaults to and what `_Layout.cshtml` was
      written for - under `system` it emits no `data-theme` attribute and the stylesheet follows the machine.

**The three Jekyll sites now say `default_theme: "system"` too - checked 2026-08-27.** `sites/www`,
`sites/docs` and `sites/demo` all follow the machine, so every surface agrees.

### A3. Check version 4 of the API is still marked experimental

- [x] **`api/src/Binacle.Net/v4/ApiV4Document.cs:14` returns `true` - read 2026-08-26.** **Shipping v4 as
      stable would lock contracts meant to keep moving**, and the flip is 3.1.0 work.

### A4. Fix the packing demo bugs that go inside the image

**`plans/api/packing-demo-bugs.md` holds all ten and what causes each. This row takes some of them.**

**Why it is on this list at all.** The shared package `packages/binacle-net-ui/` is used by both the demo
inside the image and the demo site, so these bugs are in the image. Three of them put a visitor two clicks
from an error box **on the page the image exists to show**. That meets the test for list A as written.

**The ones this release takes, and the file each fix lands in.** Fix it in the shared package where the plan
says so - one edit covers both the image and the demo site.

| What a visitor sees | Where the fix goes |
|---|---|
| A visitor types their own numbers and gets an error box - they arrive as text and the API refuses them | `packages/binacle-net-ui/src/core/packingDemo.ts` `onSubmit`. Adding `.number` to the `x-model` lines in `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` is cheap insurance on top |
| Press Add bin, then submit, and it fails - the copy gets the same name as the bin it was copied from | `packages/binacle-net-ui/src/core/packingDemo.ts` |
| Press Clear all, then submit, and it fails - an empty list passes the check that is meant to catch it | same file, `isValid()` |
| A failed request leaves the last result still drawn - the panel says nothing packed while the picture disagrees | same file, where it hands the scene over |
| The results panel prints a raw code name, such as `EarlyFail_ContainerDimensionExceeded` | the shared package's model layer |
| The error box is not opened as a real dialog - the keyboard walks the page behind it and Escape does nothing | `packages/binacle-net-ui/` |
| Result rows cannot be reached or pressed from a keyboard | both page templates |
| The 3D picture is stretched - it is sized from the window rather than from the box it sits in, and it is worst on a phone | `packages/binacle-net-ui/src/core/packingVisualizer.ts:45` and `:226` |
| Pressing the one button gives no sign it was pressed - nothing changes until the results arrive | `packages/binacle-net-ui/src/core/packingDemo.ts` `onSubmit`, plus the button bindings and the live region in both page templates |
| A partial result gives a percentage and never names the items that did not fit | **Not in the image. Helpers only.** The four strings are in `packages/binacle-net-ui/src/core/packingDemo.ts`; nothing renders them on either host - see below |

**The submit button was cut from this gate and is now in - 2026-08-27.** It was called polish; it was built
and it is **inside the image**, which is why it is on the table above rather than in a note under it. **Its
status line was trimmed the same day and that is in the image too**: "Results ready." is gone, and
`submitStatus` now clears on any change to the form, so a message cannot outlive the result it described.
"Packing..." and "No results." are the only two left.

**The unfitted items are the one row on this table with no visible feature behind it. Helpers only.**
`hasUnpackedItems`, `unpackedItemsOf`, `unpackedItemsTitle` and `unpackedItemText` are in
`packages/binacle-net-ui/src/core/packingDemo.ts` and therefore in the image, with unit tests. **Nothing calls
them.** An inline block shipped on both hosts on 2026-08-27 and was pulled the same day because it changes the
height of the result row; the markup is out of both templates and the answer will be a tooltip, which is a new
interaction and is not in this release. **So the image carries four unused strings and a visitor sees
nothing**, which is what it did before 27 Aug.
`grep -c hasUnpackedItems sites/demo/pages/packing.html api/src/Binacle.Net.UIModule/Pages/Packing.cshtml`
returns 0 for each - that is the row's state, not a failure.
**Nine of the plan's ten are in the image; this one is not.**

**Both hosts match otherwise - checked 2026-08-27.** `sites/demo/pages/packing.html` and
`api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` each carry the seven `x-model.number` lines, the error
block, the keyboard-reachable result rows, the readable status text, and the submit-button bindings with the
live region. The demo site's body copy matches `AppletsService.cs` as well. **The unfitted-items removal was
the last site edit this gate needed, and it landed on both hosts on 2026-08-27. No site work is left here.**

**One claim from the 2026-08-25 review does not hold, so nobody chases it.** "The module ships the pre-pass
dark palette" is wrong. `api/src/Binacle.Net.UIModule/_sass/_theme.scss` is byte identical to
`sites/demo/_sass/_theme.scss` once whitespace is stripped, `#3c5d8b` is also `sites/www`'s dark `--primary`,
and the module carries the same four contrast overrides in its `_components.scss` that the site does. **There
is no palette edit on this gate.**

### A5. Check the changelog names every image change since the last test release

**Release paperwork. All of it in `CHANGELOG.md`, in the `## [Unreleased]` section.** Do this **after** A4
lands, so the audit covers the final tree.

- [x] **The theme switcher line - written 2026-08-25.**
- [x] **`/Error` corrected to `/error/{errorCode?}`.**
- [x] **The Diagnostics bullet is written**, and it names all six new `System` keys, not two.
- [x] **Internal Work does not get the Razor Pages rebuild - decided 2026-08-25.**
- [x] **The precedent sentence is written**, appended to the restructure bullet in Internal Work.
- [x] **Walked, 2026-08-26. Ten bullets were missing.** The one that mattered: **no
      `Strict-Transport-Security` header is sent any more.** `UseHsts()` exists at `v2.1.1` and at
      `v3.0.0-beta.4` and greps to nothing now - a deliberate deletion, recorded as D2 in the API decisions
      ledger, and a self-hoster relying on that header would have found out by not finding out. The other nine
      are static files moving under `/_content/`, API paths no longer answering with the demo's error page,
      the one-line footer, no page fetching anything from the internet, Add bin / Add item behaviour, the
      empty validation dialog, `0%` instead of `NaN%`, the error page naming the problem, and the theme
      resetting on every page load over plain http.
- [x] **Re-measured 2026-08-26: 108 MB, against 158 MB built the old way.** The file said 103 and 150. Those
      were right in **MiB** and the file says MB, which is what `docker image ls` and Docker Hub both report -
      so anyone checking would have seen a different number. Both corrected. "About a third smaller" still
      holds, at 31.6%. The old-way figure was measured by actually building it, not estimated.
- [x] **A4 gets no changelog lines - settled 2026-08-26, and the earlier instruction here was wrong.** This
      file documents v2.1.1 to v3.0.0. Every bug A4 fixed was introduced by the demo rebuild inside this same
      cycle and never reached a released image, so "no longer fails" would describe a failure no released
      version ever had. **A fix only earns a line when the broken behaviour shipped.**
- [x] **Re-audited 2026-08-28 after `d0ba7823` changed image source, and one line was added.** The commit
      landed after this row was first ticked. Applying the same rule: the bad-enum answer moved twice, and
      only one move shipped. `"algorithm": "FFDD"` was read as null at v2.1.1 and caught by validation as
      absent, which is a 422 - unchanged, so no line. **`"algorithm": 1`, `true` or a list answered 400
      `Invalid JSON Format` at v2.1.1 and answers 422 now** - that shipped, so it earns one, in Core Changes
      under the V3 stability bullet. Checked by running v2.1.1's converter, not by reading it.

**The scope decision was taken on 2026-08-27: both non-image sections stay.** **📚 Versioned Docs** and
**🏗️ Internal Work** are not cut. Each now opens with one line saying what the section is and what it does
to the image. Versioned Docs says nothing in it reaches the image. **Internal Work names the items that do** -
the dependency patches, the Binacle.Geometry extraction and the packing log rework, all compiled into the
image - and says nothing else there reaches it. A blanket "none of this reaches the image" would have told a
self-hoster a security patch had not shipped, and **the three are named rather than counted** because A5 is
still open and a bullet added at the top would break "the first three". That one line replaces the mid-list
sentence the section used to carry.

### A6. Swap the demo's random roll for a hand-picked sample set

**Decided 2026-08-26. It goes inside the image, so it is on this list.**

The packing demo used to roll its opening numbers at random. It now carries **20 hand-picked samples**, one
JSON per sample in `shared/data/demo-samples/`, generated into `packages/binacle-net-ui/src/utils/sampleData.ts`
by `just regen demo-samples`. Both the demo in the image and the demo site roll from that one set.
**The first sample is pinned and always loads.** Randomize picks a different sample from the set rather than
rolling numbers.

**Why it is worth doing rather than leaving.** A picked sample can be checked against all three algorithms
once and then always behaves; a rolled one is re-rolled in front of every visitor. It also removes the last
way the page can produce an error a visitor did not cause - Randomize can currently roll two identical bins
or two identical items, and the API rejects the duplicate id.

- [x] **20 samples, every one run against all three algorithms - 2026-08-26.** 60 requests, all 200, no
      duplicate-id rejections. Bin counts run one to five. The pinned sample is three bins and two item types.
- [x] **No scenario names and no unit labels.** Numbers only. What to call a sample is a wording decision and
      it is not settled here.
- [x] **The demo site's page template needs nothing for this** - checked 2026-08-26. It binds `randomize` and
      reads `model.bins` / `model.items`, all unchanged, so the site picks the set up on its next build. **Its
      A4 markup fixes landed in the same commit** - see A4.
- [x] **The changelog line is corrected - 2026-08-26.** It used to say the demo starts from a random sample
      rather than a fixed one. It now names the 20 examples, the pinned one, and what Randomize does.

### A7. Cut the test release

- [ ] **Tag `v3.0.0-beta.5`.** `CHANGELOG.md` stays `## [Unreleased]` - a test release reads that section, and
      renaming it now fails the `notes` job.
- [ ] **Watch the run.** All seven jobs run; the Docker Hub page job skips itself because this is a test
      release. Docker Hub gets `3.0.0-beta.5` and nothing else - no `3.0`, no `latest`.

---

## Gate B - before the real release `v3.0.0`

**Nothing here can start until the test image is published.** That is the whole reason this is a second
list.

| # | Item | Where the work is | State |
|---|---|---|---|
| B1 | Check the test image is signed and complete | here | open |
| B2 | Open the test image in a browser and use it like a visitor | here | open |
| B3 | Run the first command on the Docker Hub page and paste in the real answer | `plans/ci-cd/dockerhub-overview.md` - section 1 | open |
| B4 | Rename the changelog heading to `3.0.0` | here | open - **the last edit before the release** |
| B5 | Cut the real release - dispatch the release workflow with version `3.0.0` | here | open |

### B1. Check the test image is signed and complete

- [ ] **`just image verify 3.0.0-beta.5`** - tags, signature, attestations and metadata, under the
      `binacle-labs` certificate identity. This ran green on `3.0.0-beta.3` (2026-08-17) and `3.0.0-beta.4`
      (2026-08-20). **What it proves that those runs no longer do is that the current tree still signs and
      attests correctly** after the package rewrites.

### B2. Open the test image in a browser and use it like a visitor

- [ ] **`docker run` `binacle/binacle-net:3.0.0-beta.5` with `UI_MODULE=True`** and open `/`, `/packing`,
      `/vipaq` and `/instance`. **A1 read a local build; this reads what the pipeline actually bakes.**
- [ ] **Work the packing page like a visitor.** Type your own dimensions, press Add bin, press Clear all, then
      submit each time. **If A4 landed, none of those produces an error dialog.** If A4 was struck, confirm
      what a visitor sees so it is a known cost rather than a surprise.

### B3. Run the first command on the Docker Hub page and paste in the real answer

**Why it is on a gate rather than after.** The `page` job runs at the end of the **real** release and
publishes the file as it stands. There is no "goes without it", and **the tag is the moment the page becomes
public**.

- [ ] **Re-run the quick start against `3.0.0-beta.5` and paste the real response back** into
      `.github/dockerhub-overview.md`. The response there came from a run against a deleted tag. **The pinned
      tag on the page is a placeholder, so only the response body changes.** A broken first command is the
      whole first impression.
- [ ] **Read the rendered page locally:** `just image dockerhub-overview 3.0.0`. That is exactly what the
      release publishes. The recipe refuses a version with a suffix, so it takes `3.0.0` even though the
      response was captured off beta.5.

**Do not run `Shared / Docker Hub Page` by hand before the tag - confirmed 2026-08-20, it has not been run.**
Both ways of running it publish something wrong. Leaving the version blank takes the latest **real** release,
still `v2.1.1`, so the page would describe 2.1. Typing `3.0.0` renders correctly but tells every
reader to pull `binacle/binacle-net:3.0`, which does not resolve until the real tag - the recipe substitutes
placeholders, it does not check the tag exists. **The stale 2.x page is the lesser wrong until then.**

### B4. Rename the changelog heading

- [ ] **Rename `## [Unreleased]` to `## 3.0.0`.** `CHANGELOG.md:3`, verified still `[Unreleased]` on
      2026-08-26. **The last change before the tag.**
- [ ] **Preview the body:** `just changelog extract 3.0.0`. That is exactly what publishes.

### B5. Cut the real release

- [ ] **Tag `v3.0.0`.** Everything after this is automatic: the changelog check, the test suite, the build,
      running and checking it in staging, the copy to Docker Hub under all three names, the signature, the
      release page built from the `3.0.0` section, and the Docker Hub page. **Nothing here is done by hand any
      more.** Watch the run, then open `post-release-v3.0.0.md`.

---

## The gotchas this release has to carry

Not duplicated from any plan. **They are here because they are properties of the release, not of the work.**

**The sample pins move to `3.0`, not to `3.0.0-beta.5`.** Six files sit on `3.0.0-beta.4` and stay there
through both tags. **The rule is that a pin on `main` must name an image that exists on Docker Hub**, and
beta.4 still resolves - so leaving them is correct and cheaper than moving them twice. The bump is
post-release work and the tree at `v3.0.0` carries a beta pin for the length of one run. **They moved early
once before, on 2026-08-07, and sat on `main` naming an image nobody could pull.**

**`README.md` named `binacle/binacle-net:3.0` alone, and that was reverted on 2026-08-27.** The name was moved
on 2026-08-17 when the beta names came off the public surfaces, so the most read file in the repo pointed at
an image nobody can pull, and beta.5 does not change that - a test release creates no `3.0` name. The pin
warning now carries a second line naming `3.0.0-beta.4` until the real tag, and `post-release-v3.0.0.md`
carries its removal. `tooling/README.md` and `tooling/smoke.just` still take the opposite trade deliberately.

**Any label the release workflow also emits is a label the `Dockerfile` does not own.** `metadata-action`
auto-fills `description` from the GitHub repo blurb and silently overrode the `Dockerfile`'s, and `publish`
copies by exact image ID - so what it baked was what shipped. The caption is now pinned in the workflow too.

**The test release does not exercise the names that move.** It produces its own exact name only, so `3.0`
and `latest` are still created for the first time by the real release. That is one extra argument to the same `imagetools
create` call, and it is the residual gap beta.5 cannot close. `post-release-v3.0.0.md` checks it.

---

## The release notes

**They live in `CHANGELOG.md`, in the `## [Unreleased]` section**, and the workflow extracts that section as
the release body. **There is no separate notes file.** The content was checked byte for byte against the
published beta 2 body on 2026-08-13 - all four breaking changes, the six migration steps, the signing and SBOM
bullet, the image-size drop, and `RetentionDays`. **A5 re-audits it against the tree, which has moved a long
way since.**

Three mechanics:

- **`[Unreleased]` is renamed to `## 3.0.0` as the last change before the real tag, and not before beta.5.**
  Every beta publishes `[Unreleased]`. If you rename early, the beta's `notes` job fails in under a minute and
  nothing is built - which is why that gate runs first.
- **A section's own headings are `###`**, nested under the `##` version heading. `just changelog extract`
  shifts them back to `##` on the way out. **Do not "fix" the file to use `##` throughout** - that breaks the
  nesting under `# Changelog` and the extractor's terminator both.
- **The compare link at the bottom already reads `v2.1.1...v3.0.0`** - correct from the real tag onward, and a
  404 on every beta release page until then. Left as it is deliberately.

**The restructure gets no changelog line of its own - decided 2026-08-14.** No user-observable behaviour
changes, nothing is published to NuGet, and no contract moves - the OpenAPI diff proves the last one. The four
breaking changes stay four.

**This section is also what the docs site copies**, by hand. **The two were diffed bullet for bullet on
2026-08-28 and are in step** - three drifts were fixed on the page and one of them, the release pipeline
sentence, was wrong here too and was fixed here. **The page still lacks the release date and the release
link, which need the tag.** `plans/sites/docs-v3-deploy.md` carries that and records what was changed.

---

## The sequence

1. **A4** - fix the packing demo bugs that go inside the image. **Done 2026-08-26.**
2. **A6** - the hand-picked sample set. The last code work.
3. **A1** - the browser read, which A4 and A6 both invalidated. **Done 2026-08-27** - A4's four by-eye
   checks and A6's three sample checks all passed.
4. **A5** - check the changelog, after A4 and A6 so it covers the finished tree.
5. **Cut the test release, `v3.0.0-beta.5`.** Watch the run.
6. **B1 and B2** - check the test image, then open it in a browser.
7. **B3** - the Docker Hub first command, answered from the test image, read on this machine.
8. **B4** - rename the changelog heading to `3.0.0`. The last edit.
9. **Cut the real release, `v3.0.0`.** Everything after this is automatic, the Docker Hub page included.
10. **Open `post-release-v3.0.0.md`.**

---

## Not in this release

Everything here has a plan of its own, with its state and its blocker named there. **Do not pull any of it
in.**

**Taken off the list on 2026-08-25, and these two are the ones that were on it.** **An agent decided where
they went - strike either one and it comes back.**

| Item | Why not | Where it went |
|---|---|---|
| **Every test on the CI suite** | Ten of the twenty-six tests are Ruby, and **Ruby does not build the image** - it builds the Jekyll sites. A test nobody runs cannot ship a broken image. The argument for the gate was that the release workflow uses its shared test suite as its *"this commit passed CI"* proof and that proof is incomplete. **That is true and it is still not an image risk.** | `plans/ci-cd/tests-reach-ci.md` |
| **Docker Hub tag immutability - the rule** | The switch is off, so the rule's value changes nothing about this release either way. Correcting it is what makes the post-release decision a flip rather than a project. **The stored value on 2026-08-13 was `".*"`**, which would freeze `latest` and `3.0`. | `plans/ci-cd/dockerhub-tag-immutability.md` |

**Also not here, and not new:** the Ruby coverage decision (`plans/ci-cd/sonar-scope-and-coverage.md`) and rubocop, which
lands red before it lands green. **The Docker Hub logo and categories - the rest of
`plans/ci-cd/dockerhub-overview.md` - were done outside the release on 2026-08-27.**

**Held back on 2026-08-14, with reasons that still hold:**

| Item | The blocker |
|---|---|
| **The architecture checks** | The heavy tools - ArchUnitNET, dependency-cruiser, lychee - need a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose. dependency-cruiser has no root `tsconfig.json`; there are seven, and `sites/demo/` has none. **The three lighter checks joined them on 2026-08-17.** |
| **CI gates 2 and 3** | Gate 2 runs the all-modules integration tests, which are not being written here. Gate 3 is Sonar and coverage, and its own plan says do not make coverage blocking yet. Gate 1 ships; these two have nothing to gate. |
| **Raising test coverage** | **Decided 2026-08-14: do not test the UIModule until it is rebuilt**, so the tests are not written twice in two languages. The rebuild landed on 2026-08-21 and the suites followed on 2026-08-22 - all four, plus an integration suite for the module. |
| **The demo and www deploys** | Neither site is part of this release and neither has a row here. **Do not dispatch either workflow as part of it.** |
