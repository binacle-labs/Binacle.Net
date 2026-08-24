---
description: Release - Binacle.Net v3.0.0
---

# Release - Binacle.Net v3.0.0

**Status:** In progress. Betas 1 to 4 published; `v3.0.0-beta.4` is the tag deployed on the test server. The
pipeline is rebuilt and proven end to end, the architecture branch is merged, the suite is green and the
OpenAPI contracts are proven unmoved - the top-line `info.description` changed on 2026-08-23, and no path,
schema or response did.

**The site and gem work landed on 2026-08-24 and it reaches this release in two places.** The theme switcher
is shared code that the UI module ships, so it goes into the image. The docs site is rebuilt on eight gems, so
it goes out with the docs deploy. **What is left is a browser pass over the switcher, one decision, a few
changelog lines, the last commit and the tag**, plus the immutability rule that holds nothing up.

**Betas 1 and 2 are deleted from Docker Hub.** Only `3.0.0-beta.3` and `3.0.0-beta.4` still resolve. Anything
that quotes a real tag or a real response has to name one of those two, or `3.0.0` once it exists.

**`3.0.0-beta.4` is the reference tag.** `just image verify 3.0.0-beta.4` passed all four checks on
2026-08-20 - tags, signature, attestations and metadata - under the `binacle-labs` certificate identity.
`3.0.0-beta.3` passed the same run on 2026-08-17 and is the fallback. **Both are safe to name in an example;
nothing else is.**

**Created:** 2026-07-16. **Rewritten for the GHCR pipeline:** 2026-08-11. **Scope reset:** 2026-08-14.
**Pruned to pending work only:** 2026-08-20. **Reorganised after the gem work landed:** 2026-08-24.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to
the reference rules: it may point at any file to coordinate the release, and **nothing points back at it**.
Delete it once v3.0.0 is out and verified.

Companion: `post-release-v3.0.0.md` - the checks to run once the release is out.

---

## How to work this file

**This file holds pending work only.** A finished item is a tick and a date, with the detail gone - what
outlives the release is in the docs and the decisions ledgers, not here.

**Where a plan does the work, this file names the slice it took and nothing more.** The plan file itself never
says what ships when. When a slice lands, cut that part out of the plan; when nothing is left in the plan,
delete it. Whatever the release did not take stays in the plan.

**Two lists.** The gate is what must be green before the tag. "Runs alongside" does **not** hold the tag - if
one is not ready, the release goes without it.

**How the pipeline works**, because it shapes every item below. Every build is staged on GHCR, smoked there,
and the smoked digest is **copied** to Docker Hub - so nothing unsmoked reaches the registry users pull from.
A prerelease gets its immutable tag only, never `3.0` or `latest`. The release body is extracted from
`CHANGELOG.md` by the workflow, and the last job writes the Docker Hub page.

**Three deploy workflows exist and this release owns one of them.** The docs deploy is release work and has
its own section below. The demo and www deploys are not, and **nothing in this release dispatches them.**

---

## The gate - all of this before the tag

| # | Item | State |
|---|---|---|
| 1 | Rate limiter tests | done - 2026-08-14 |
| 2 | Rate limiting owned by the ServiceModule | done - 2026-08-14 |
| 3 | The Azure Storage run | done - 2026-08-14, and now on the PR gate |
| 4 | Beta 3, and its three live checks | done - 2026-08-19 |
| 5 | Admin read endpoints | done - 2026-08-19 |
| 6 | Beta 4, deployed to the test server | done - 2026-08-19 |
| 7 | The site and gem work | landed 2026-08-24 - what it adds to this release is rows 8 to 10 |
| 8 | The theme switcher, read in a browser | open |
| 9 | The default theme the module ships | open - **a decision, not work** |
| 10 | The changelog lines the last two branches force | open |
| 11 | The description caption's comment | open |
| 12 | Every test leaf on the CI suite | open |
| 13 | The Docker Hub page's quick start | open |
| 14 | The last commit: pins, prose and the changelog rename | open |
| 15 | Tag `v3.0.0` | open |

### 7. What the site and gem work added to this release

**Landed 2026-08-24. Recorded here because two halves of it ship with v3.0.0 and neither was planned as
release work.**

- **The image half.** `packages/theme-switcher` was rewritten and the UI module takes it, along with changes to
  `_Layout.cshtml`, `_Header.cshtml` and two stylesheets. **This is user-visible in the shipped image.** It is
  why rows 8, 9 and 10 exist.
- **The docs half.** The docs site now builds on eight gems, and the change reaches all four version folders,
  not only `v3.0.x`. It goes out with the docs deploy below. **There is no separate deploy for it.**

**The three site plans it closed are deleted:** the robots tag, the redirect layout's own head, and the two
unreferenced android icons. Nothing is left of them to carry.

**Nothing else in that work touches the image.** The gems build Jekyll sites; the image builds from `npm` and
`dotnet`. `just build image` does not need Ruby.

### 8. The theme switcher, read in a browser

**The slice this release takes from [todos](plans/todos.md): the four checks in its Theme section, and the
`Dockerfile` comment under Comments.** Everything else in that file stays in it. **The checks themselves are
written there, once - do not copy them here.**

**Why they are on the gate.** Every check a command can settle has been run. These four cannot be, and the
switcher ships inside the image, so the tag is the last moment anyone can look. Read the UI module with
`UI_MODULE=True`, and the three built sites.

- [ ] **All four checks pass, on all four hosts.**

### 9. The default theme the module ships

- [ ] **Decide, then either change one line or record the answer.** `ModuleDefinition.cs` sets
      `DefaultTheme = "light"` for the UI module while all three Jekyll sites default to dark. **One of the two
      is what a first-time reader sees, and they disagree.** It is one line either way and it is baked into the
      image at the tag.

### 10. The changelog lines the last two branches force

Four edits to `CHANGELOG.md`, all in the `[Unreleased]` section.

- [ ] **A line for the theme switcher, or a recorded decision that it needs none.** The `🎨 UI Module` section
      was written on 2026-08-23, before the switcher was rewritten. The test is whether a self-hoster running
      `UI_MODULE=True` notices.
- [ ] **`/Error` is written as `/error`.** It is `/error/{errorCode?}`.
- [ ] **A `### 🧪 Diagnostics Module` bullet.** `/_health`'s `System` entry gained `ReservedPaths`, and
      `Features` now lists `HealthChecks`. `tooling/smoke/prod.hurl` moved from `count == 0` to `count == 1`
      for exactly this, so anyone parsing `/_health` sees it.
- [ ] **One sentence on the 2026-08-14 precedent**, in the release notes section below. That precedent says the
      restructure gets no changelog line because nothing user-observable moved. **The UI rebuild was tested
      against that bar and failed it** - moved routes, a dropped variable. Say so, or a later reader applies it
      to the wrong thing.

### 11. The description caption's comment

- [ ] **`Dockerfile:8` names two places; there are three.** The caption is pinned in the `Dockerfile` label,
      in `release-docker-image.yml`, and in `api/src/Binacle.Net.Kernel/Metadata.cs`, which is what reaches
      Swagger UI, Scalar and both published OpenAPI documents. Change two of the three and the image label and
      the API document disagree, silently. **One comment.** Do it while the reason is in front of someone,
      because the next person to change that string will read the comment and believe it.

### 12. Every test leaf on the CI suite

**The slice this release takes from
[ci-cd/test-leaves-reach-ci](plans/ci-cd/test-leaves-reach-ci.md): the whole plan.** Nothing is left in it, so
it is deleted when this lands.

**Why it is on the gate rather than alongside.** The release workflow calls `shared-test-suite.yml` as its
*"this commit passed CI"* proof. Ten of the twenty-six test leaves have no step there, so that proof is
currently incomplete - **the tag would be cut on a green check for suites nobody ran.** That is the argument;
placement is still yours to strike.

- [ ] **Give the ten Ruby leaves a step each**, and add `Setup - Ruby` pointed at `ruby/` - to the test suite
      and to the Sonar workflow, which runs the same leaves through `just coverage all` and today cannot start
      them at all.
- [ ] **Group the leaves in the test module** - one list per slice, `all` their sum - so the list of leaves
      exists once.
- [ ] **Add `just check test-steps`** and put it on the pull request gate, so a leaf added after the tag
      cannot go missing the same way.

**Steps stay per leaf, not per group - decided 24 Aug 2026.** A red check has to name the suite. The plan
carries what that costs and why the check is the answer to it.

### 13. The Docker Hub page's quick start

**The slice this release takes from [ci-cd/dockerhub-overview](plans/ci-cd/dockerhub-overview.md): section 1,
the `curl` example.** Everything else in that plan stays in it.

**Why it is on the gate rather than alongside - state chosen by an agent, strike it if wrong.** The `page` job
runs at the end of the release and publishes the file as it stands, so there is no "goes without it". The tag
is the moment the page becomes public.

- [ ] **Re-run the quick start against `3.0.0-beta.4` and paste the real response back.** The response in
      `.github/dockerhub-overview.md` came from a run against a tag that is now deleted. A broken first command
      is the whole first impression. The pinned tag in the page is a placeholder, so only the response body
      changes.
- [ ] **Read the rendered page locally:** `just image dockerhub-overview 3.0.0`. That is exactly what the
      release publishes.

**Do not run `Shared / Docker Hub Page` by hand before the tag - confirmed 2026-08-20, it has not been run.**
Both ways of running it publish something wrong. An empty version input takes the latest **non-prerelease**
release, which is still `v2.1.1`, so the page would describe 2.1. Typing `3.0.0` renders correctly but tells
every reader to pull `binacle/binacle-net:3.0`, which does not resolve yet - the recipe substitutes
placeholders, it does not check the tag exists. **The stale 2.x page is the lesser wrong until the tag.** The
local render above is the pre-tag check.

### 14. The last commit before the tag - all in one

- [ ] **Rename `## [Unreleased]` to `## 3.0.0`** in `CHANGELOG.md`.
- [ ] **Move six pins from `3.0.0-beta.4` to `3.0`:**

  | File | What to change |
  |---|---|
  | `samples/docker/{minimal,quickstart,prod,service,full}/docker-compose.yml` | the `image:` line |
  | `samples/kubernetes/minimal/binacle-deployment.yaml` | the `image:` line |

- [ ] **Drop the expiring comment in the same six files.** Each `image:` line carries two extra lines - *"Pinned
      to the beta patch for now because `binacle/binacle-net:3.0` does not exist on Docker Hub yet - move to
      the 3.0 minor tag once v3.0.0 is published."* Delete those two, leaving only *"Pinned on purpose - a
      copied sample must not jump to a new major on the next pull."* **That reason expires the moment v3.0.0
      publishes.**
- [ ] **Rewrite the same reason in prose in `samples/README.md` and `samples/docker/README.md`.** Both name
      `3.0.0-beta.4` and explain why; both become `3.0` with the explanation cut.
- [ ] **Re-confirm `ApiV4Document.IsExperimental` is still `true`.** Read on 2026-08-24 and it is. Shipping v4
      as stable would lock contracts meant to keep moving. The flip is 3.1.0 work.
- [ ] **Preview the body:** `just changelog extract 3.0.0` after the rename. That is exactly what publishes.

**One decision, and it is open.** `README.md` was moved to `binacle/binacle-net:3.0` early, on 2026-08-17, when
the beta names came off the public surfaces. That tag does not exist on Docker Hub yet, so **the most read file
in the repo currently names an image nobody can pull**. Either revert it until the tag or accept it. The same
early-move trade was taken deliberately for `tooling/README.md` and `tooling/smoke.just`, which read `3.0.0`.

**The rule that drives the pin timing: a pin on `main` must name an image that exists on Docker Hub.** They
moved early once before, on 2026-08-07, and sat on `main` naming an image that did not exist. **Do not leave
the `3.0` bump on `main` long before tagging.**

### 15. Tag

- [ ] **Tag `v3.0.0`.** The pipeline does the rest: the changelog gate, the suite, the GHCR build, the smoke,
      the Docker Hub copy under all three tags, the signature, the release created from the `3.0.0` section,
      and the Docker Hub page. **Nothing here is manual any more.** Watch the run, then work
      `post-release-v3.0.0.md`.

---

## Runs alongside - does not hold the tag

**The Docker Hub logo and categories are not here.** They are the rest of the `ci-cd/dockerhub-overview` plan
and they stay in it, not in this release - the release only takes that plan's `curl` example. Recorded so
they are not mistaken for release work that got dropped.

### The pull request gate builds the image with no Node

**Verified 2026-08-24 and still true.** `release-docker-image.yml` has a Node step and an `npm ci`.
`pull-request.yml`'s `image` job does not - it sets up `just` and `.NET`, then runs `just build image`, whose
`publish` dependency runs `npm run copy-assets-to-uimodule` and `npm run build` inside the UI module. Both
resolve `gulp`, `sass` and `webpack` out of a `node_modules` nothing installed.

**It costs a merge, not an image**, which is why it is here and not on the gate. It is red on the required
check on every pull request that touches code.

- [ ] **Copy the two steps from the release workflow into `pull-request.yml`'s `image` job**, and prove it on
      a pull request.

### Ruby coverage - the decision only

**The slice this release takes from [ruby-gem-coverage](plans/ruby-gem-coverage.md): the answer, not the
build.** The ten gemspecs, the ten spec helpers and the Sonar property stay in the plan.

- [ ] **Decide whether the gems produce coverage at all.** They produce none today, and the coverage table
      builds its rows from the reports that exist - so they are absent from it rather than sitting at zero.
      **If the answer is no, write that down where the coverage pipeline's reasoning lives and drop the plan.**
      Leaving it open means the next reader of that table re-discovers ten missing suites from scratch.

**Placement chosen by an agent - strike it if wrong.** It is here rather than on the gate because nothing
about the image or the docs changes either way.

### Docker Hub tag immutability - the rule only

**The slice this release takes from
[ci-cd/dockerhub-tag-immutability](plans/ci-cd/dockerhub-tag-immutability.md): correct the rule, leave the
switch off.** The plan holds the trap, the regexp and why prereleases are excluded; the switch and the
scratch-repo test stay in it.

- [ ] **Correct the rule to released versions only, and read the value back from the API.** The stored value
      on 2026-08-13 was `".*"`, which would freeze `latest` and `3.0` - the two tags the release moves. **The
      switch is off, so nothing breaks either way; correcting it now is what makes the post-release decision a
      flip rather than a project.**

**Unclear, and it needs an answer:** `post-release-v3.0.0.md` was written as though this had already been done.
Nothing here records a run. Read the API and tick it or leave it.

---

## The release notes

**They live in `CHANGELOG.md`, in the `## [Unreleased]` section**, and the workflow extracts that section as
the release body. The content was checked byte-for-byte against the published beta 2 body on 2026-08-13 - all
four breaking changes, the six migration steps, the signing and SBOM bullet, the image-size drop, and
`RetentionDays`.

Three mechanics:

- **`[Unreleased]` is renamed to `## 3.0.0` as the last change before the tag.** Every beta publishes
  `[Unreleased]`. If you forget, the `notes` job fails in under a minute and nothing is built - which is why
  that gate runs first.
- **A section's own headings are `###`**, nested under the `##` version heading. `just changelog extract` shifts
  them back to `##` on the way out. **Do not "fix" the file to use `##` throughout** - that breaks the nesting
  under `# Changelog` and the extractor's terminator both.
- **The compare link at the bottom already reads `v2.1.1...v3.0.0`** - correct from the tag onward, and a 404
  on every beta release page until then. Left as it is deliberately.

**This section is also what the docs site copies.** The `## 3.0.0` body is hand-carried into the v3.0.x
release-notes page, and a v3.0.1 appends rather than replaces. The additions that page is missing are in the
docs deploy checklist below.

**The restructure gets no changelog line - decided 2026-08-14.** No user-observable behaviour changes, nothing
is published to NuGet, and no contract moves - the OpenAPI diff proves the last one. The four breaking changes
stay four. **Row 10 above adds the sentence saying the UI rebuild was measured against this bar and failed it.**

---

## The docs deploy - after the tag

**The config half is done:** `main` carries `current: v3.0.x`, `- id: v3.0.x` back at the top of `list`, and
the version sitemap restored - all verified 2026-08-14. What is left is the deploy plus the edits that must go
out with it.

**`sites/docs/` is off limits to a coding session.** This is the docs session's work, written here for it.

- [ ] **Read the rebuilt docs site before dispatching anything.** The gem work changed pages in all four
      version folders - `v1.3.x`, `v2.0.x`, `v2.1.x` and `v3.0.x` - plus the landing, the 404, the sitemaps,
      `robots.txt`, the typography and the code-block styles. **This deploy publishes every one of those,
      not only the `v3.0.x` edits below.** Build it and read it. **Placement chosen by an agent - strike it if
      you would rather this were its own pass.**

      Four things a build settles, and nothing else does:

  - Every version's pages still carry a title, a description and a canonical.
  - `/version/latest/` still redirects, is still `noindex`, and its canonical still points at
    `/version/<current>/` rather than at itself.
  - The three sites' `robots.txt` files are byte identical to each other apart from the host in the
    `Sitemap:` lines, and each is byte identical to what it was before.
  - The site serves and links a web app manifest, and the icon paths in it resolve. **Check it in a
    browser's application panel** - a wrong icon path fails with no console error.

- [ ] **Re-cut the worked example in `v3.0.x/verifying-a-release.md` against the real `3.0.0`.** It is the
      **last place any public surface still names a beta image**, and it cannot be fixed before the tag because
      it quotes real output. Run `just image verify 3.0.0` and replace three things with what it prints: the
      Docker Hub digest, the package count, and the provenance run URL.

      **It is now broken, not merely stale.** The example verifies `3.0.0-beta.2`, which is **deleted from
      Docker Hub** and was signed under the old owner anyway - so the block asks the reader to run a command
      that cannot succeed. **This page must not deploy before this row is done.**

      **The rule that decided this, worth keeping for future releases: name a version where the version is the
      fact, never as a floor or an example.** A floor or a sample tag goes stale on its own; a record of what
      was signed does not.

- [ ] **Put the real date and release link in `v3.0.x/release-notes.md`.** The `## v3.0.0` section carries
      interim wording because the tag did not exist when the pages were written. Swap the italic line for
      *"Released &lt;date&gt; - [release on GitHub](.../releases/tag/v3.0.0)"*, matching every other version
      folder.
- [ ] **Carry the additions from `CHANGELOG.md` into `v3.0.x/release-notes.md`.** Same notes in two places,
      and the release body gained content on 2026-08-10, 2026-08-23 and again at row 10 above that the page
      does not have.

      **Decided 2026-08-14: this page stays hand-copied.** It is not generated from `CHANGELOG.md`. The drift is
      the accepted cost, so **this checklist is the control** - every future release's docs handover has to list
      what the changelog gained since the page was last written. Run `just changelog extract Unreleased` to see
      the current text. **Row 10 above adds to this list, so read the changelog rather than only this page.**
      The first four go in the `## v3.0.0` section, in the page's plain-ASCII style; the fifth is a migration
      step:
  - **Overview**, one bullet after the health check line: the image creates `/app/data` and gives it to the app
    user, so a volume mounted there is writable.
  - **Core Changes**, replacing *"The `Dockerfile` and existing environment variables are unchanged"* - which is
    false, the Dockerfile changed three times this release. Spell out the `/app/data` fix (docker created the
    mount point as root, the app does not run as root, so packing logs and the SQLite database could not be
    written to a fresh named volume); `libgssapi-krb5-2` now shipping, so Npgsql stops printing "Cannot load
    library libgssapi_krb5.so.2" at every start, which was harmless but read as fatal; OCI labels on the image;
    and only then "existing environment variables are unchanged".
  - **A `🔌 Service Module` section**, between Diagnostics and UI Module: the auth token rate limit partitions
    on the connection's remote address instead of a caller-supplied header, so varying the header no longer
    resets your own login throttle.
  - **A `🧪 Diagnostics Module` section**, carrying the `/_health` bullet from row 10.
  - **The whole `🎨 UI Module` section, plus one Overview bullet and one Core Changes bullet.** Rewritten in
    `CHANGELOG.md` on 2026-08-23: the section it replaced said "neither tool changed" and was written before
    the 21-22 Aug rebuild. What the page needs is the rebuild itself (Blazor to Razor Pages, no SignalR
    circuit), the route changes `/PackingDemo` -> `/packing` and `/ProtocolDecoder` -> `/vipaq`, the Protocol
    Decoder renamed to ViPaq Decoder, the new `/instance` page, the randomizer, that the module reads no
    configuration, and whatever row 10 settled about the theme switcher. The Core Changes bullet is the removal
    of `BINACLEAPI_CONNECTION_STRING` - it replaces "existing environment variables are unchanged", which was
    false.
  - **Migration step 7, `Drop BINACLEAPI_CONNECTION_STRING`**, which pushed the `cosign verify` step to 8.
    The docs page numbers its own steps, so renumber there too.
- [ ] **Replace the two swagger documents under `sites/docs/collections/_versions/v3.0.x/swagger/`.** Copy
      `artifacts/openapi/Binacle.Net_v3.json` -> `swagger/v3.json` and `artifacts/openapi/Binacle.Net_v4.json`
      -> `swagger/v4.json`; the generator's file names differ from what the site expects, so the rename is part
      of the handover.

      **Re-measured 2026-08-23, and the 2026-08-14 figure no longer applies.** The `servers` entry and the
      removal of the `429` responses are **already in the committed files** - both carry `servers: [{"url":
      "/"}]` and zero `429`. Against a fresh `just openapi generate`, sorted-key diffs of both documents show
      **one changed line each: `info.description`.** Nothing else moves, in either document.

      **The description is what moved, and it moved on purpose** - the caption replacing *"an API created to
      address the 3D Bin Packing Problem in real time"*. It is still a visible change to the published spec,
      so mention it wherever the update is described.
- [ ] **Write the client-generation page.** **Pulled in on 2026-08-14 at the maintainer's call.**

      A short page with copy-paste commands that generate a client from the published per-version spec -
      `hey-api` for TypeScript, `kiota` for C#, and whatever else is worth naming. Today the spec is published
      and nothing tells anyone they can do this.

      **It applies to every version, not just v3.0.x** - the maintainer's call. Each version folder publishes
      its own `swagger/v3.json` and `swagger/v4.json`, so the commands work against v1.3.x, v2.0.x, v2.1.x and
      v3.0.x alike. **Write it so the version is a placeholder the reader substitutes**, rather than four
      near-identical pages that drift apart - which means deciding once where a cross-version page lives on
      that site.

      **Do not publish SDKs to close this.** The deliverable is a spec plus a generation guide, not shipped
      packages.
- [ ] **The UI module no longer reads any configuration.** The rebuild deleted the `BinacleApi` connection
      string and `Config_Files/UiModule/` with it, so five files under `v3.0.x` describe a file the image does
      not ship. All five are wrong for 3.0.0, not merely stale. **Confirmed by grep on 2026-08-24.**
  - **`configuration/ui-module/index.md`** - delete the `## ⚙️ Configuration` block (the
    `/app/Config_Files/UiModule` sentence and the directory tree) and the whole `## 🛠️ Configuration`
    section at the bottom - the auto-detect paragraph, the `BinacleApi` JSON sample, and the link to
    *Configuration Basics > Connection String Fallbacks*. **What replaces them:** the module reads no
    configuration at all. Both demos run in the browser and call the API they are served from, over relative
    URLs, so there is nothing to point anywhere. `UI_MODULE=True` is the whole setup.
  - **`configuration/index.md`** - drop the `UiModule` branch and its `ConnectionStrings.json` leaf from the
    directory tree, so it matches the image.
  - **The three sample compose files** under `v3.0.x/samples/docker/` - `quickstart`, `full` and `service` -
    each set `BINACLEAPI_CONNECTION_STRING`. Delete that line, and in `service` the two comment lines above it.
    Repo-root `samples/` has already had this done; copy from there.

      **Leave `v1.3.x`, `v2.0.x` and `v2.1.x` alone.** Those versions really did read the file. Only `v3.0.x`
      is wrong.

- [ ] **Two framework defaults nobody overrode. Placement chosen by an agent - strike it if you would rather
      it waited.** Both are on the docs site and both go out with this deploy or not at all until the next one.
  - **Code samples render in the body sans-serif.** They need a monospace stack.
  - **Wide tables are clipped rather than scrolled.** They need a scrolling container.
        Checked on 2026-08-24: no `overflow-x` exists anywhere in `sites/docs/_sass/`.

- [ ] **The ViPaq rename on the docs landing.** The landing still calls it the Protocol Decoder. The image and
      the changelog both call it the ViPaq Decoder from 3.0.0. **Placement chosen by an agent - strike it if
      wrong.** The rest of that page's rewrite is not release work and is not here.

- [ ] **Deploy.** It is `workflow_dispatch` only.

**This is the single most losable item in the release** - nothing fails if the deploy is skipped, the site just
quietly keeps serving v2.1.x as current. **It has to run after the tag**, because the notes need the date and
the `releases/tag/v3.0.0` link, and `main` already says v3.0.x is current, so deploying earlier presents an
unreleased version as current. It has to land before anything is announced.

**One deliberate 404, do not "fix" it.** The `v3.0.x` ViPaq page links the wire spec at
`github.com/binacle-labs/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md`, which 404s until the tag is pushed. A
versioned page should pin the spec it describes; do not repoint it at `main`.

---

## The sequence

1. **The theme switcher in a browser**, the default-theme decision, and the changelog lines those force.
2. **The caption comment, and every test leaf on the CI suite.** The immutability rule, the Ruby coverage
   answer and the pull request gate's Node steps whenever they suit.
3. **The Docker Hub page's quick start.**
4. **The last commit:** changelog rename, six pins, six comment blocks, two READMEs, and the `IsExperimental`
   re-confirm.
5. **Tag `v3.0.0`.** The pipeline does the rest, page included.
6. **Deploy the docs**, with the edits above.
7. **Work `post-release-v3.0.0.md`**, then delete both files.

---

## Not in this release

Everything else has a plan of its own, with its state and its blocker named there.
**Do not pull any of it in.**

**Held back on 2026-08-14, with reasons that still hold:**

| Item | The blocker |
|---|---|
| **The architecture checks** | The heavy tools - ArchUnitNET, dependency-cruiser, lychee - need a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose. dependency-cruiser has no root `tsconfig.json`; there are seven, and `sites/demo/` has none. **The three lighter checks joined them on 2026-08-17**, when a better design turned a ready item into a fresh one. |
| **CI gates 2 and 3** | Gate 2 runs the all-modules integration tests, which are not being written here. Gate 3 is Sonar and coverage, and its own plan says do not make coverage blocking yet. Gate 1 ships; these two have nothing to gate. |
| **Raising test coverage** | **Decided 2026-08-14: do not test the UIModule until it is rebuilt**, so the tests are not written twice in two languages. The rebuild landed on 2026-08-21 and the suites followed on 2026-08-22 - all four of them, plus an integration suite for the module. None of that shipped here: what did is the modest bump the rate limiter tests brought, and nothing more. |
| **The workflow restructure's last item** | The branch protection edit. It landed 2026-08-18 and left the release on 2026-08-19 - it gates nothing here. |

**Held back on 2026-08-24, when the gem work landed:**

| Item | Why not |
|---|---|
| **Rubocop** | `ruby/.rubocop.yml` exists, rubocop is in no `Gemfile` and no recipe calls it. It lands red before it lands green. |
| **The demo and www deploys** | Neither site is part of this release and neither has a row here. **Do not dispatch either workflow as part of it.** |
