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
      moving off `3.0.0` is the one thing no local run rehearsed. **This also closes the last box of
      `plans/sites/docs-current-at-root.md`** - `docker manifest inspect binacle/binacle-net:3` succeeds -
      so tick it there in the same sitting.
- [ ] No prerelease reached Docker Hub.
      `curl -s "https://hub.docker.com/v2/repositories/binacle/binacle-net/tags?page_size=100" | jq -r '.results[].name'`
      lists no `3.1.0-beta.*`. **The first release under the prerelease stop** - `D3` in the CI/CD ledger -
      and the tag list is the proof it held.
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
- [ ] The `v3.x` entry in `sites/docs/_data/versions.yml` says what the line has shipped: `label: v3.1.0`,
      `url_segment: 3.1.0`, `version_tag: "3"` (the major tag - it exists once this release publishes it, and
      then every page pulls with it and no later minor edits the line).
      `grep -A3 'id: v3.x' sites/docs/_data/versions.yml` shows the three values. Same commit: the sentence
      under the `docker run` in `v3.x/quick-start.md` and in `v3.x/samples/index.md` says "the major tag", not
      "the minor tag" - true only once the tag printed is `3`.
      `grep -n 'minor tag' sites/docs/collections/_versions/v3.x/quick-start.md sites/docs/collections/_versions/v3.x/samples/index.md`
      returns nothing.
- [ ] `bundle exec jekyll build` passes in `sites/docs`, and `Deploy Site` is dispatched with `docs` and green.
      `docs.binacle.net/release-notes/` shows 3.1.0 at the top, and `docs.binacle.net/version/v3.0.x/` answers
      `301` to `/`.
- [ ] The two deploy-only boxes in `plans/sites/docs-current-at-root.md` are ticked from the live site -
      every old URL redirects, and the selector lands on the same page - and the 302 to 301 flip is done.
      **By eye**, with the checks written under each box in that plan. They straddled the tag and would
      otherwise be lost the way the v3.0.0 post-release boxes were.

**Why this is after the tag, not before.** The page names a release date and a release link that do not
exist until the run is green. On `main` before that they would be lies for the length of the gap.

**No folder opens.** `plans/sites/docs-current-at-root.md` landed in the release: one folder per major,
the current one at the root. A minor is this section and nothing else.

### 2. Move the pins to `3` - a coding session

**`3`, not `3.1` - the maintainer decided on 2026-09-11 and confirmed on 2026-09-15.** The major tag follows
every minor and patch in the line, so this move happens once and no later minor repeats it. An old minor tag
gets no patches - `3.0` stays at `3.0.0` - so a sample pinned to a minor is a sample that stops getting fixes
the day the next minor ships. **One exception, decided 2026-09-15: the `service` sample pins `3.1`**, because
the Service Module is the one thing a minor may break, and that sample is for people who turned it on. It moves
by hand at each minor. The release workflow publishes `3` for the first time with this release, which is why
the move still waits for the run.

- [ ] Five samples pin `binacle/binacle-net:3`, and `samples/docker/service/docker-compose.yml` pins `3.1`.
      `grep -rln 'binacle-net:3\.' samples/` returns only the service compose file, and that file says `3.1`.
      The five are `samples/docker/{minimal,quickstart,prod,full}/docker-compose.yml` and
      `samples/kubernetes/minimal/binacle-deployment.yaml`.
- [ ] The three files that carry the tag in prose moved with them: `README.md:21`, `samples/README.md:28`,
      `samples/docker/README.md:36` - each says `3`, and the two sample READMEs say why `service` is the
      exception. The two that name it only as an example, `tooling/README.md` and `tooling/smoke.just`, may
      stay.
      `grep -rn 'binacle-net:3\.[0-9]' README.md samples/README.md samples/docker/README.md` returns nothing.
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
      and `Deploy Site` is dispatched with `www` and green.
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

**None.** Nothing was held back for the tag: row 5 and the six CI findings ship in it, everything up to the
smoke was proved by the betas from the branch, and the release `publish` changes by the `3.1.0` run itself. The one plan the release touches without closing
is `plans/api/integration-tests-cover-shipped-modules.md`, whose optional-modules half was never tied to a
version.

**What the beta leaves behind.** `ghcr.io/binacle-labs/binacle-net:3.1.0-beta.<n>` stays on GHCR - a
prerelease's image stops there since 2026-09-14, `D3`. `beta.1` and `beta.2` have no git tag; from `beta.3`
on, each has its tag and a GitHub prerelease, amended 2026-09-18. **The image stays there** - the
maintainer said so on 2026-09-14; nothing deletes a staging image.

**Delete this file once the first two lists are clear.** What outlives it goes to the docs and the decision
ledgers, not here.
