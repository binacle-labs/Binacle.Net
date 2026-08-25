---
description: Release - Binacle.Net v3.0.0
---

# Release - Binacle.Net v3.0.0

**Status:** In progress. Betas 1 to 4 published; `v3.0.0-beta.4` is the tag deployed on the test server. The
pipeline is rebuilt and proven end to end, the architecture branch is merged, the suite is green, and the
OpenAPI contracts are proven unmoved - the top-line `info.description` changed on 2026-08-23, and no path,
schema or response did. The site and gem work landed on 2026-08-24; the theme switcher is shared code the UI
module ships, so it goes into the image.

**What is left is a browser pass, one decision, the changelog, the Docker Hub page and the tag.**

**Betas 1 and 2 are deleted from Docker Hub.** Only `3.0.0-beta.3` and `3.0.0-beta.4` still resolve. Anything
that quotes a real tag or a real response has to name one of those two, or `3.0.0` once it exists.

**`3.0.0-beta.4` is the reference tag.** `just image verify 3.0.0-beta.4` passed all four checks on
2026-08-20 - tags, signature, attestations and metadata - under the `binacle-labs` certificate identity.
`3.0.0-beta.3` passed the same run on 2026-08-17 and is the fallback. **Both are safe to name in an example;
nothing else is.**

**Created:** 2026-07-16. **Rewritten for the GHCR pipeline:** 2026-08-11. **Scope reset:** 2026-08-14.
**Pruned to pending work only:** 2026-08-20. **Cut to the image and the tag:** 2026-08-25.

The orchestrator for v3.0.0 (drops v2, adds experimental v4, rebuilt ViPaq). This is the **one exception** to
the reference rules: it may point at any file to coordinate the release, and **nothing points back at it**.
Delete it once v3.0.0 is out and verified.

Companion: `post-release-v3.0.0.md` - the work v3.0.0 causes, and the checks to run once it is out.

---

## How to work this file

**A pointer surface. It holds no work it can point at.** Each row names the plan and the slice of it this
release takes. **The plan holds the work; this file holds the order and the gotchas.** Where a row has no
plan, it is release paperwork and it lives here because nothing else owns it.

**The test that put a row here, and it is not "is it important":**

- **It is in the image.** An image cannot be corrected after it is pushed.
- **Or the tag fires it and cannot fire it again** - the release body, the Docker Hub page.

**Everything else is post-release or a plan.** Something that can be fixed the day after costs a day, not a
release. **The two rows nobody would put here on merit are named at the bottom with the reason** - so they are
not mistaken for work that got dropped.

**A finished item is a tick and a date, with the detail gone.** What outlives the release is in the docs and
the decision ledgers, not here.

**How the pipeline works**, because it shapes every row. Every build is staged on GHCR, smoked there, and the
smoked digest is **copied** to Docker Hub - so nothing unsmoked reaches the registry users pull from. A
prerelease gets its immutable tag only, never `3.0` or `latest`. The release body is extracted from
`CHANGELOG.md` by the workflow, and the last job writes the Docker Hub page.

**Three deploy workflows exist and this release dispatches none of them.** The docs deploy is caused by the
tag and belongs to `post-release-v3.0.0.md`. The demo and www deploys are not release work at all.

---

## The gate - all of this before the tag

| # | Item | Where the work is | State |
|---|---|---|---|
| 1 | The theme switcher, read in a browser | here | done - 2026-08-25 |
| 2 | The default theme the module ships | here | done - 2026-08-25 |
| 3 | The changelog, and the `## 3.0.0` rename | here | **the rename only** |
| 4 | The Docker Hub page's quick start | `plans/ci-cd/dockerhub-overview.md` - section 1, the `curl` example | open |
| 5 | `ApiV4Document.IsExperimental` is still `true` | here | open - **a read** |
| 6 | Tag `v3.0.0` | here | open |

### 1. The theme switcher, read in a browser

**Why it is on the gate.** Every check a command can settle has been run. Those four cannot be, and the
switcher ships inside the image, so the tag is the last moment anyone can look.

- [x] **The four checks were read on all four hosts, 2026-08-25.** No wrong-theme flash, the machine is
      followed with JavaScript off, the switcher works from a keyboard and announces itself, and it sits right
      in each header.
- [x] **Re-read after the default changed to `"system"` - 2026-08-25.** No wrong-theme flash on the module's
      first paint with the `theme` cookie cleared.

### 2. The default theme the module ships

- [x] **Decided 2026-08-25: every surface follows the machine.** The module shipped `light` while all three
      Jekyll sites default to dark. `UIModuleOptions.cs` and `ModuleDefinition.cs` now both say `"system"`,
      which is what the switcher package already defaults to and what `_Layout.cshtml` was already written
      for - under `system` it emits no `data-theme` attribute and the stylesheet follows the machine.

**The three sites are not release work.** `default_theme: "dark"` in each `_config.yml` is a site session,
and `sites/` is off limits to a coding session. It is a row in `plans/todos.md`.

### 3. The changelog, and the rename

Release paperwork. All of it in `CHANGELOG.md`.

- [x] **The theme switcher earns a line - written 2026-08-25.** It goes in the UI Module section: the demo
      follows the machine on a first visit, the header switcher still overrides it, and the same `theme`
      cookie is kept, so anyone who already chose one is unaffected.
- [x] **`/Error` corrected to `/error/{errorCode?}`.** Verified against `Pages/Error.cshtml:1` and
      `ModuleDefinition.cs:48`.
- [x] **The Diagnostics bullet is written, and it is bigger than this row said.** At v2.1.1 `/_health`'s
      `System` entry returned **only** `Processors`. `Version`, `Environment`, `StartedAt`, `Uptime` and
      `Features` are all new as well, not just `ReservedPaths` and `HealthChecks`. **A bullet naming only the
      two would have misled anyone parsing the response.**
- [x] **Internal Work does not get the Razor Pages rebuild - decided 2026-08-25.** The framework swap and its
      only user-visible consequence are already stated in full in the UI Module section's first bullet, and
      Internal Work is for work with no user-facing bullet.
- [x] **The precedent sentence is written**, appended to the restructure bullet in Internal Work.
- [ ] **Rename `## [Unreleased]` to `## 3.0.0`.** The last change before the tag.
- [ ] **Preview the body:** `just changelog extract 3.0.0` after the rename. That is exactly what publishes.

### 4. The Docker Hub page's quick start

**Why it is on the gate rather than after.** The `page` job runs at the end of the release and publishes the
file as it stands, so there is no "goes without it". **The tag is the moment the page becomes public.**

- [ ] **Re-run the quick start against `3.0.0-beta.4` and paste the real response back.** The response in
      `.github/dockerhub-overview.md` came from a run against a tag that is now deleted. **A broken first
      command is the whole first impression.** The pinned tag in the page is a placeholder, so only the
      response body changes.
- [ ] **Read the rendered page locally:** `just image dockerhub-overview 3.0.0`. That is exactly what the
      release publishes.

**Do not run `Shared / Docker Hub Page` by hand before the tag - confirmed 2026-08-20, it has not been run.**
Both ways of running it publish something wrong. An empty version input takes the latest **non-prerelease**
release, which is still `v2.1.1`, so the page would describe 2.1. Typing `3.0.0` renders correctly but tells
every reader to pull `binacle/binacle-net:3.0`, which does not resolve yet - the recipe substitutes
placeholders, it does not check the tag exists. **The stale 2.x page is the lesser wrong until the tag.**

### 5. `IsExperimental`

- [ ] **Re-confirm `ApiV4Document.IsExperimental` is still `true`.** Read on 2026-08-24 and it is. **Shipping
      v4 as stable would lock contracts meant to keep moving**, and the flip is 3.1.0 work.

### 6. Tag

- [ ] **Tag `v3.0.0`.** The pipeline does the rest: the changelog gate, the suite, the GHCR build, the smoke,
      the Docker Hub copy under all three tags, the signature, the release created from the `3.0.0` section,
      and the Docker Hub page. **Nothing here is manual any more.** Watch the run, then open
      `post-release-v3.0.0.md`.

---

## The gotchas this release has to carry

Not duplicated from any plan. **They are here because they are properties of the release, not of the work.**

**The sample pins name an image that does not exist yet, and that is deliberate.** Six files sit on
`3.0.0-beta.4` through the tag. **The rule is that a pin on `main` must name an image that exists on Docker
Hub**, and `binacle/binacle-net:3.0` only starts resolving when the publish job finishes - so the bump is
post-release work and the tree at `v3.0.0` carries a beta pin for the length of one run. **They moved early
once before, on 2026-08-07, and sat on `main` naming an image nobody could pull.**

**`README.md` already names `binacle/binacle-net:3.0`.** Moved on 2026-08-17 when the beta names came off the
public surfaces, so **the most read file in the repo currently names an image nobody can pull.** Either revert
it until the tag or accept it - the same early-move trade was taken deliberately for `tooling/README.md` and
`tooling/smoke.just`, which read `3.0.0`. **Open, and it is a judgement call.**

**Any label the release workflow also emits is a label the `Dockerfile` does not own.** `metadata-action`
auto-fills `description` from the GitHub repo blurb and silently overrode the `Dockerfile`'s, and `publish`
copies by digest - so what it baked was what shipped. The caption is now pinned in the workflow as well.

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
- **A section's own headings are `###`**, nested under the `##` version heading. `just changelog extract`
  shifts them back to `##` on the way out. **Do not "fix" the file to use `##` throughout** - that breaks the
  nesting under `# Changelog` and the extractor's terminator both.
- **The compare link at the bottom already reads `v2.1.1...v3.0.0`** - correct from the tag onward, and a 404
  on every beta release page until then. Left as it is deliberately.

**The restructure gets no changelog line - decided 2026-08-14.** No user-observable behaviour changes, nothing
is published to NuGet, and no contract moves - the OpenAPI diff proves the last one. The four breaking changes
stay four. **Row 3 above adds the sentence saying the UI rebuild was measured against this bar and failed it.**

**This section is also what the docs site copies**, by hand. The docs plan carries what the page is missing.

---

## The sequence

1. **The theme switcher in a browser**, and the default-theme decision.
2. **The changelog**, including the rename - it is the last edit before the tag.
3. **The Docker Hub page's quick start**, read locally.
4. **The `IsExperimental` read.**
5. **Tag `v3.0.0`.** The pipeline does the rest, page included.
6. **Open `post-release-v3.0.0.md`.** The docs deploy and the sample pins are the first two rows in it.

---

## Not in this release

Everything here has a plan of its own, with its state and its blocker named there. **Do not pull any of it
in.**

**Taken off the gate on 2026-08-25, and these two are the ones that were on it.** The test is at the top of
this file. **Placement chosen by an agent - strike either one and it comes back.**

| Item | Why not | Where it went |
|---|---|---|
| **Every test leaf on the CI suite** | Ten of the twenty-six leaves are Ruby, and **Ruby does not build the image** - it builds the Jekyll sites. A leaf nobody runs cannot ship a broken image, and adding the steps the day after costs nothing. The argument for the gate was that the release workflow uses `shared-test-suite.yml` as its *"this commit passed CI"* proof and that proof is incomplete. **That is true and it is still not an image risk.** | `plans/ci-cd/test-leaves-reach-ci.md` |
| **Docker Hub tag immutability - the rule** | The switch is off, so the rule's value changes nothing about this release either way. Correcting it is what makes the post-release decision a flip rather than a project - and that is a post-release sentence, not a gate. **The stored value on 2026-08-13 was `".*"`**, which would freeze `latest` and `3.0`. | `plans/ci-cd/dockerhub-tag-immutability.md` |

**Also not here, and not new:** the Ruby coverage decision (`plans/ruby-gem-coverage.md`), the pull request
gate's missing Node steps and the description caption's comment (both `plans/todos.md`), the Docker Hub logo
and categories (the rest of `plans/ci-cd/dockerhub-overview.md`), and rubocop, which lands red before it lands
green.

**Held back on 2026-08-14, with reasons that still hold:**

| Item | The blocker |
|---|---|
| **The architecture checks** | The heavy tools - ArchUnitNET, dependency-cruiser, lychee - need a new toolchain: ArchUnitNET wants a new test project that becomes a node in the graph it inspects, and `.xUnitV3` may drag in plain `xunit.v3` when this repo pins `xunit.v3.mtp-v2` on purpose. dependency-cruiser has no root `tsconfig.json`; there are seven, and `sites/demo/` has none. **The three lighter checks joined them on 2026-08-17**, when a better design turned a ready item into a fresh one. |
| **CI gates 2 and 3** | Gate 2 runs the all-modules integration tests, which are not being written here. Gate 3 is Sonar and coverage, and its own plan says do not make coverage blocking yet. Gate 1 ships; these two have nothing to gate. |
| **Raising test coverage** | **Decided 2026-08-14: do not test the UIModule until it is rebuilt**, so the tests are not written twice in two languages. The rebuild landed on 2026-08-21 and the suites followed on 2026-08-22 - all four, plus an integration suite for the module. None of that shipped here: what did is the modest bump the rate limiter tests brought, and nothing more. |
| **The demo and www deploys** | Neither site is part of this release and neither has a row here. **Do not dispatch either workflow as part of it.** |
