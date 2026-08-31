---
description: Release - Binacle.Net v3.0.0. A test release first, then the real one. Everything that goes inside the image is done before the test release.
---

# Release - Binacle.Net v3.0.0

**Status:** Betas 1 to 7 published. **Gate A is closed.** Gate B is where the work is: B1 and B3 are done,
B2 needs a browser, B4 and B5 are yours.

**`3.0.0-beta.7` was cut on 2026-08-31 from `ce9e4461` and its run was green.** Tag, GitHub prerelease,
signature, SBOM and provenance are all there - run `33374185508`. **It is the first image carrying AGPL-3.0**,
which is the one thing in this release that cannot be corrected after a push. *After beta.5* below has what
beta.6 was for and how it ended.

**beta.7 is the image every check now runs against**, and it replaces beta.6 everywhere this file names one.

**Why there was a beta.5, kept because the same argument now applies again.** `v3.0.0-beta.4` was tagged on
2026-08-20 and **247 files changed after it**, 176 of them under `api/`, +2378 / -13149. The UI module was
rebuilt, the beercss vendor assets left the tree, and the `cookies` and `theme-switcher` packages were
rewritten in TypeScript. Every statement in this file naming `3.0.0-beta.4` described an image the tree no
longer produced. **A test release is the rehearsal that makes the real tag a formality**, and *After beta.5*
below is the same test run against the current tree.

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

**Which published tags are safe to name in an example. Read the registry, not this file.** **Every beta from
1 to 6 resolves on Docker Hub - checked 2026-08-31.** This file said betas 1 to 4 were deleted; they are not,
the deletion was planned and never done, and the claim had reached three other files before anyone read the
registry. `latest` is still `2.1.1`. `3.0` does not exist.

**Resolving and verifying are not the same thing.** Betas 1 to 4 were signed under a tag ref, so the published
`cosign verify` command, anchored at `@refs/heads/main$`, **fails on them**. Only `3.0.0-beta.5`,
`3.0.0-beta.6` and `3.0.0-beta.7` pass it, which is why `README.md:20` names a beta - see the gotchas.

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
| A7 | Cut the test release - dispatch the release workflow with version `3.0.0-beta.5` | here | done - 2026-08-28. **Run `33127006852`, from `72015ff1`** |
| A8 | Move the code licence to AGPL-3.0 | here | done - 2026-08-31. **Landed in the image at `3.0.0-beta.7`** |

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

- [x] **Dispatched 2026-08-28, green in 6m 6s.** Run `33127006852`, from commit `72015ff1`. `CHANGELOG.md`
      stayed `## [Unreleased]`.
- [x] **Docker Hub got `3.0.0-beta.5` and nothing else - read off the registry, not off the run.** No `3.0`.
      `latest` is still the 2026-01-12 image, which is `v2.1.1`. The `page` job skipped itself: the overview
      still describes 2.1.

**The run is listed against `main`, not against a tag, and that is correct.** A dispatch has no tag ref to
carry, because the workflow makes the tag near the end. The thing it could have broken is the signature, and
B1 proves it did not. `run-name` now puts the version in the Actions list, since the ref cannot - see D1.

---

### A8. Move the code licence to AGPL-3.0

**It goes in Gate A because the licence is baked into the image**, in three places that no later commit can
reach: the `org.opencontainers.image.licenses` label, the `LICENSE.AGPL-3.0` file at `/app`, and the footer
link the UI module renders. An image already pulled keeps whatever it was built with.

- [x] **The label is right, read off the published image - 2026-08-31.**
      `org.opencontainers.image.licenses=AGPL-3.0-only AND CC-BY-4.0 AND Apache-2.0 AND MIT`.
- [x] **`/app/LICENSE.AGPL-3.0` is in the image and `/app/LICENSE.GPL-3.0` is gone - 2026-08-31.**
      That is GPL sections 4 and 5 satisfied for the conveyed binary.
- [x] **The footer links `blob/main/LICENSE.AGPL-3.0` and says `AGPL-3.0` - 2026-08-31**, read off a running
      beta.7 with `UI_MODULE=True`.
- [x] **GitHub reads the repository as AGPL-3.0 - 2026-08-31.** `spdx_id: AGPL-3.0` off the repository API.
      This was the one thing that could only be checked after a push.
- [x] **`tooling/smoke/structure.yaml` asserts the new label and passes - 2026-08-31.** 37 of 37 against the
      published beta.7. The assertion had never run against a real image before this.

**The boundary sentence is correct as written.** `CHANGELOG.md`, `NOTICE` and the v3 release notes all say
versions up to and including `3.0.0-beta.6` stay GPL-3.0. beta.7 is the first AGPL image, so that line holds.


## After beta.5 - beta.6 went red, beta.7 went green

**Eleven commits landed after `v3.0.0-beta.5`, six of them untested by any dispatch**, which is what beta.6
was cut for. The image changes were FluxResults vendored in-tree, the licence text and `NOTICE` shipping
inside the image, and the four-licence `licenses` label. The workflow changes were the anchored certificate
identity, `3.0` and `latest` waiting until the signature is checked, and the signed provenance from
`actions/attest-build-provenance`.

**`3.0.0-beta.6` was dispatched on 2026-08-30 from `1f6fbbe2` and the `publish` job went red.** Run
`33339765633`. It failed on `Verify the published signature`, the step added with the two-stage copy and
running for the first time.

**It was a race, not a signature problem.** cosign attaches the signature as an OCI referrer and Docker Hub's
referrers index is eventually consistent. The bundle was written at 22:46:03Z; the verify read it at
22:46:06.8 and got `no signatures found`. The same command passes now, under cosign `v3.0.6` - the exact CI
build - as well as `v3.1.3`. So not the format, the flags, the identity or the version.

**What is published, and it is complete.** `just image verify 3.0.0-beta.6 all` passes: signature under the
anchored identity, SPDX SBOM with 166 packages, provenance pointing at the run, revision `1f6fbbe2`, base
`aspnet:10.0`, runs as `app (1654)`. `Move the tags that move` printed `moving=` and would have skipped
anyway, and the `page` job skipped on its prerelease guard. **Nothing is half-published.**

**The fix is a retry in the workflow step, not in the recipe.** Five attempts, 15 seconds apart. The recipe
stays one shot because it is the command `SECURITY.md` hands readers, and a reader wants an answer rather
than a wait.

**beta.7 proved the retry, and it did it on the commit that mattered.** Dispatched 2026-08-31 from `ce9e4461`,
the licence commit. Run `33374185508`, green end to end. The `v3.0.0-beta.7` tag and the GitHub prerelease are
both there, so the `release` job ran this time. `v3.0.0-beta.6` also has its tag and prerelease.

**So the race is closed and beta.6 needs nothing.** beta.7 supersedes it as the image every check runs
against.

**What beta.7 was checked with, 2026-08-31, all against the published image and not a local build:**

| Check | Result |
|---|---|
| `just image verify 3.0.0-beta.7 all` | PASS - signature on `refs/heads/main`, SBOM 166 packages, provenance on run `33374185508`, revision `ce9e4461` |
| `container-structure-test` against `tooling/smoke/structure.yaml` | 37 of 37 |
| Five smoke profiles - minimal, quickstart, prod, service, full | 76 requests, 0 failures |
| `just test all` | 12 dotnet projects 10321 passed, 5 jest suites 854 passed, 10 rspec suites 0 failures |
| UI pages with `UI_MODULE=True` - `/`, `/packing`, `/vipaq`, `/instance` | 200, and all ten static assets 200 |

**The one thing on that list nobody had run before was the structure test.** It is what asserts the AGPL label,
and it had been written but never executed against a real image.

---

## Gate B - before the real release `v3.0.0`

**Nothing here can start until the test image is published.** That is the whole reason this is a second
list.

| # | Item | Where the work is | State |
|---|---|---|---|
| B1 | Check the test image is signed and complete | here | done - 2026-08-31, **re-run against beta.7** |
| B2 | Open the test image in a browser and use it like a visitor | here | open - **the browser half only**, against beta.7, see below |
| B3 | Run the first command on the Docker Hub page and paste in the real answer | `plans/ci-cd/dockerhub-overview.md` - section 1 | done - 2026-08-31, **no edit was needed** |
| B4 | Rename the changelog heading to `3.0.0` | here | open - **the last edit before the release** |
| B5 | Cut the real release - dispatch the release workflow with version `3.0.0` | here | open |

### B1. Check the test image is signed and complete

- [x] **All four checks pass - 2026-08-31.** One tag on Docker Hub and no `3.0` or `latest`; the signature
      verifies under the anchored identity; the SPDX SBOM and SLSA provenance are both attached; `revision` is
      `72015ff1`, base `aspnet:10.0`, runs as `app (1654)`, `/app/data` owned `app:app 755`, framework
      dependent.
- [x] **The recipe reported two false failures and both were this machine, not the image.** `cosign` was not
      installed, which it said. `docker buildx` was not installed, which it did **not** say - it swallowed the
      error and printed the attestations as MISSING. **That is fixed**: the check now names the missing tool,
      the way the signature check already did. The attestations were confirmed by reading the registry
      directly before the fix went in.

### B2. Open the test image in a browser and use it like a visitor

- [ ] **`docker run` `binacle/binacle-net:3.0.0-beta.7` with `UI_MODULE=True`** and open `/`, `/packing`,
      `/vipaq` and `/instance`. **A1 read a local build; this reads what the pipeline actually bakes.**

**What was checked on 2026-08-31 without a browser, so the browser pass is shorter.** The published image was
run and all four pages answered 200, as did `/swagger/`, `/scalar/` and `/error/404`. Every stylesheet and
script it asks for is served from `/_content/` and answered 200; **nothing on the page is fetched from the
internet**. The packing page as the pipeline baked it carries the seven `x-model.number` lines, one Randomize
button, the bound `submitStatus`, the error dialog and no `hasUnpackedItems` - so A4 and A6 are in the image.
**What is left is the part that needs javascript**, which is the box below.
- [ ] **Work the packing page like a visitor.** Type your own dimensions, press Add bin, press Clear all, then
      submit each time. **If A4 landed, none of those produces an error dialog.** If A4 was struck, confirm
      what a visitor sees so it is a known cost rather than a surprise.

**Re-read against `3.0.0-beta.7` on 2026-08-31.** Same result: `/`, `/packing`, `/vipaq` and `/instance` all
200, and all ten `/_content/` assets 200. The footer now says `AGPL-3.0` and links `blob/main/LICENSE.AGPL-3.0`.
**Only the javascript half is left**, and it is the box above.

### B3. Run the first command on the Docker Hub page and paste in the real answer

**Why it is on a gate rather than after.** The `page` job runs at the end of the **real** release and
publishes the file as it stands. There is no "goes without it", and **the tag is the moment the page becomes
public**.

- [x] **Ran against `3.0.0-beta.5` on 2026-08-31, and the response on the page was already right.** Every
      field matches byte for byte: `NotAllItemsFit` / `locker-S` / `60.0` / `72.97`, then `AllItemsFit` /
      `locker-M` / `30.83` / `100`. **No edit was needed.** This row said the response came from a deleted tag;
      whatever its source, it is correct. **Both sections of `plans/ci-cd/dockerhub-overview.md` are done** -
      section 2 on 2026-08-27, section 1 here. **It is deleted once the real release has published the page**,
      not now: nothing else records what that job is supposed to produce.
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

**The sample pins move to `3.0` at the tag, and they sit on `3.0.0-beta.6` until then - moved 2026-08-31.**
Six files, plus the two sample READMEs. **Two rules hold them there**: a pin on `main` must name an image that
exists on Docker Hub, and **no public surface may name a beta that fails the published verify command**. beta.4
satisfied the first and not the second. The bump to `3.0` is post-release work, and the tree at `v3.0.0`
carries a beta pin for the length of one run. **They moved early once before, on 2026-08-07, and sat on `main`
naming an image nobody could pull** - that is what the first rule exists for.

**beta.7 shipped and the pins still say beta.6.** Both rules still hold - beta.6 resolves and verifies - so
this is not a bug, and moving them buys one thing only: a reader who copies a sample today gets the AGPL
image rather than the last GPL one. **Weigh that against touching nine files twice**, since they all go to
`3.0` at the tag anyway. Leaving them is defensible; if any beta is deleted before the tag, it stops being.

**`README.md:20` named an image the published verify command rejects, and it now names `3.0.0-beta.6` -
moved 2026-08-31.** It pointed at `3.0.0-beta.4`, which resolves but was signed under a tag ref, so
`cosign verify` anchored at `@refs/heads/main$` failed on it, two lines under a sentence telling the reader to
run that command. beta.6 passes it, checked rather than assumed, and so does beta.7. **The line is still
deleted at the tag** - `post-release-v3.0.0.md` carries that.

**The six sample pins and the two sample READMEs moved with it**, same day, same reason. `tooling/image.just:9`
named `3.0.0-beta.4` as the example of a **tag-signed** release needing its ref; **the example was cut and the
sentence kept**, because every tag-signed image is a beta and the betas are deleted once `3.0.0` is live.

**`README.md` named `binacle/binacle-net:3.0` alone, and that was reverted on 2026-08-27.** The name was moved
on 2026-08-17 when the beta names came off the public surfaces, so the most read file in the repo pointed at
an image nobody can pull, and no test release changes that - a test release creates no `3.0` name. The pin
warning now carries a second line naming `3.0.0-beta.6` until the real tag, and `post-release-v3.0.0.md`
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
2026-08-28** - three drifts were fixed on the page and one of them, the release pipeline
sentence, was wrong here too and was fixed here. **The FluxResults bullet, added here on 2026-08-29, went on
the page the same day** - `plans/sites/docs-v3-deploy.md` section 2 records it and the override it needed.
**The error page's status line did the same on 2026-08-31.** **The page still lacks the release date and the
release link, which need the tag.** `plans/sites/docs-v3-deploy.md` carries that and records what was changed.

---

## The sequence

1. **A4** - fix the packing demo bugs that go inside the image. **Done 2026-08-26.**
2. **A6** - the hand-picked sample set. The last code work.
3. **A1** - the browser read, which A4 and A6 both invalidated. **Done 2026-08-27** - A4's four by-eye
   checks and A6's three sample checks all passed.
4. **A5** - check the changelog, after A4 and A6 so it covers the finished tree.
5. **Cut the test release, `v3.0.0-beta.5`.** **Done 2026-08-28.**
6. **B1** - check the test image. **Done 2026-08-31.**
7. **B3** - the Docker Hub first command. **Done 2026-08-31, and the page needed no edit.**
8. **A8** - move the code licence to AGPL-3.0. **Done 2026-08-31**, and it had to be in an image before the
   tag because an image cannot be relicensed after a push.
9. **Cut `3.0.0-beta.7`** from the licence commit, for the green run and to prove the retry.
   **Done 2026-08-31** - run `33374185508`, tag and prerelease both there.
10. **B2** - open the beta.7 image and use the packing page like a visitor. **The one thing left that needs a
    human.** Everything on that page that can be checked without javascript already has been.
11. **B4** - rename the changelog heading to `3.0.0`. The last edit.
12. **Cut the real release, `v3.0.0`.** Everything after this is automatic, the Docker Hub page included.
13. **Open `post-release-v3.0.0.md`.**

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

**Also not here, and not new:** rubocop, which lands red before it lands green. **The Ruby coverage import
landed on 2026-08-31, outside the release** - the gems read 98.8%. **The Docker Hub logo and categories - the rest of
`plans/ci-cd/dockerhub-overview.md` - were done outside the release on 2026-08-27.**

**Held back on 2026-08-14, with reasons that still hold:**

| Item | The blocker |
|---|---|
| **The architecture checks** | The heavy tools - ArchUnitNET, dependency-cruiser, lychee - need a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose. dependency-cruiser has no root `tsconfig.json`; there are seven, and `sites/demo/` has none. **The three lighter checks joined them on 2026-08-17.** |
| **CI gates 2 and 3** | Gate 2 runs the all-modules integration tests, which are not being written here. Gate 3 is Sonar and coverage, and its own plan says do not make coverage blocking yet. Gate 1 ships; these two have nothing to gate. |
| **Raising test coverage** | **Decided 2026-08-14: do not test the UIModule until it is rebuilt**, so the tests are not written twice in two languages. The rebuild landed on 2026-08-21 and the suites followed on 2026-08-22 - all four, plus an integration suite for the module. |
| **The demo and www deploys** | Neither site is part of this release and neither has a row here. **Do not dispatch either workflow as part of it.** |
