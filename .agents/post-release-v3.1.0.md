---
description: Post-release - what to look at once v3.1.0 is out, what the tag causes, and the plans it stops blocking. None of it holds the release up.
---

# Post-release - v3.1.0

**Status:** not started. `v3.1.0` is not tagged. **Nothing here can begin before the run is green.**

**A pointer surface.** Where a row names a plan, the plan holds the work. Where a row has no plan, it is a
single mechanical act with a known answer and it lives here.

**Three lists, in the order they should be worked.** Look first, because looking is cheap and it finds things.
The work comes second because it is the half that can be quietly skipped. **Delete this file once the first
two lists are clear** - the third is plans, and plans outlive it.

| List | What is in it | How to tell |
|---|---|---|
| **Things to look at** | you run it, read it, and it comes back yes or no | if it needs a decision, a password, a new file or a workflow, it is not one of these |
| **Things to do** | what the tag *caused* and would be wrong to leave | it is only true because v3.1.0 shipped |
| **Plans that stop being stuck** | plans that were waiting on the release and now are not | each has its own file, state and blocker |

---

## Things to look at

- [ ] `just smoke all binacle/binacle-net:3.1.0` - every profile green.
      **This confirms, it does not protect.** The pipeline smoked the staging copy already. What this buys is
      the one thing the pipeline cannot check: that the **copy** to Docker Hub landed something that runs.
- [ ] `3.1.0`, `3.1`, `3` and `latest` are one image, and `3.0` did not move.
      `docker buildx imagetools inspect` on all five; the first four share a digest and `3.0` still reports
      the `3.0.0` digest. **`3.1` and `3` are created for the first time by this release**, and `latest`
      moving off `3.0.0` is the one thing no local run rehearsed.
- [ ] `just image verify 3.1.0` - PASS. Signed on `refs/heads/main` by the release workflow, SBOM and
      provenance present. The identity is the whole value; anyone can sign anything.
- [ ] The Docker Hub page names `3.1.0`, and nowhere names `3.0.0` except where a version history should.
      **By eye**, on `hub.docker.com/r/binacle/binacle-net`. The `page` job wrote it last; it runs only on a
      real version, so this is its first run for this line.
- [ ] The GitHub release body is the `3.1.0` changelog section and nothing else.
      **By eye**, on `github.com/binacle-labs/Binacle.Net/releases/tag/v3.1.0`.
- [ ] The published image, in a browser, from Docker Hub.
      `docker run` `binacle/binacle-net:3.1.0` with `UI_MODULE=True`, open `/`, `/packing`, `/vipaq` and
      `/instance`. On `/packing`: `Best` is in the list, the winner shows on the row, `02-packs-nowhere`
      opens the unpacked list. On `/instance`: the presets show with no fetch - watch the network tab.
      **The local build covered the same code; this confirms the copy carried it.**
- [ ] The demo site packs against the public API with `Best`, from a phone as well as a desktop.
      **By eye** on `demo.binacle.net/packing`. The tooltip opens on tap - that is the `:focus-within` rule,
      and a phone is the one place nobody has checked it.
- [ ] Whichever site the request panel landed on prints a host a reader can paste. **By eye.** On the
      published image it is the container's own origin; on the demo site, if it is there at all, it is the
      public API, which the plan says is worth much less.

## Things to do

**In this order.** The release notes first, so the docs say 3.1.0 exists before anything points a reader at it.

### 1. The 3.1.0 release notes on the docs site - a site session

- [ ] `sites/docs/collections/_versions/v3.x/release-notes.md` opens with `## v3.1.0`, the release date and
      the GitHub release link, and the body is the `3.1.0` changelog section. No warning block; this line has
      no breaking change. **By eye.** It is the same text as the release body, so `just changelog extract 3.1.0`
      is the source.
- [ ] `bundle exec jekyll build` passes in `sites/docs`, and `Deploy Docs Site` is dispatched and green.
      `docs.binacle.net/release-notes/` shows 3.1.0 at the top, and `docs.binacle.net/version/v3.0.x/` answers
      `301` to `/`.

**Why this is after the tag, not before.** The page names a release date and a release link that do not
exist until the run is green. On `main` before that they would be lies for the length of the gap.

**No folder opens.** `plans/sites/docs-current-at-root.md` landed in the release: one folder per major,
the current one at the root. A minor is this section and nothing else.

### 2. Move the pins to `3` - a coding session

**`3`, not `3.1` - the maintainer decided on 2026-09-11.** The major tag follows every minor and patch in
the line, so this move happens once and no later minor repeats it. The release workflow publishes `3` for the
first time with this release, which is why the move still waits for the run.

- [ ] The six samples pin `binacle/binacle-net:3`.
      `grep -rln 'binacle-net:3\.' samples/` returns nothing. That is `samples/docker/*/docker-compose.yml`
      (five) and `samples/kubernetes/minimal/binacle-deployment.yaml`.
- [ ] The three files that carry the tag in prose moved with them: `README.md:21`, `samples/README.md:28`,
      `samples/docker/README.md:36`. The two that name it only as an example, `tooling/README.md` and
      `tooling/smoke.just`, may stay.
      `grep -rn 'binacle-net:3\.[0-9]' README.md samples/` returns nothing.
- [ ] `.agents/docs/samples/README.md` still says "read the value out of the sample files" and names no
      version of its own. **By eye.** If it names `3.0` anywhere, that sentence was not honoured.
- [ ] The docs site's copies of the six sample files match `samples/` again - a site session, since
      the copies are under `sites/`.
      `for f in $(cd samples && find docker kubernetes -name '*.yml' -o -name '*.yaml' -o -name '*.json'); do
      diff -q samples/$f sites/docs/collections/_versions/v3.x/samples/$f; done` prints nothing.
      They pin `3` with the rest, and never move again at a minor.

**The rule that governs the move:** a pin on `main` must name an image that already exists, so this cannot
land before the *Things to look at* list confirms `3` resolves. `3.0` stays valid forever - the move is
about which line a new reader is handed, not about anything breaking. **What `3` promises a self-hoster:**
a restart picks up the next minor, and a minor may break the Service Module. That is the trade the
maintainer took.

### 3. The www `docker run` line - a site session

- [ ] `sites/www/_data/exchange.yml` names `binacle/binacle-net:3` in `command.tag` and `command.text`,
      and `Deploy WWW Site` is dispatched and green.
      `grep -n '3\.0' sites/www/_data/exchange.yml` matches only the `verified:` lines, which record what was
      pulled on a date and are history.
- [ ] The four `verified:` lines are re-run against `3.1.0` or left dated as they are. **His call**; the
      examples are v3 calls and v3 did not change, so leaving them is defensible.

### 4. Close the release set

- [ ] `release-v3.1.0.md` is deleted, once every box in it is ticked and the image is verified.
      `test ! -f .agents/release-v3.1.0.md`.
- [ ] The `verified:` line of `.agents/docs/sites/www.md` and `.agents/docs/sites/docs.md` is refreshed if
      the edits above touched what they describe.
- [ ] This file is deleted when *Things to look at* and *Things to do* are clear.

## Plans that stop being stuck

**None.** Nothing was held back for the tag: row 5 and the six CI findings ship in it, and the release
`publish` changes were proved by `3.1.0-beta.1` before it. The one plan the release touches without closing
is `plans/api/integration-tests-cover-shipped-modules.md`, whose optional-modules half was never tied to a
version.

**One thing the beta leaves behind.** `binacle/binacle-net:3.1.0-beta.1` (and any `beta.2`) stays on Docker
Hub - `D27`. The plan that would have sent it elsewhere is
`plans/ci-cd/prerelease-staging-repository.md`, still an idea. **If that plan is ever picked up, this
release is the second time a beta sat beside the release it rehearsed**, which is the argument for it.

**Delete this file once the first two lists are clear.** What outlives it goes to the docs and the decision
ledgers, not here.
