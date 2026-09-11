---
description: The docs site keeps one folder per major, renders the current one at the site root, and drops the common-page layer. A minor stops moving every URL.
state: ready
waits-on: "nothing - the maintainer said yes on 2026-09-11 and put it in the v3.1.0 release set"
horizon: now
paths:
  - "sites/docs/**"
  - "ruby/binacle-docs-versions/**"
  - "tooling/openapi.just"
  - ".github/workflows/release-docker-image.yml"
---

# The current docs line renders at the root, one folder per major

## What changes

Today every minor opens a new folder under `_versions/`, every indexed URL moves with it, and six pages sit
outside the versions as "common" because the root had to hold something. After this plan:

```
today                                          after

_common_pages/  6 pages, version-free, at /    _common_pages/  version.html only (the list at /version/)
_versions/v3.0.x/  renders at /version/v3.0.x/ _versions/v3.x/  renders at /            current, indexed
_versions/v2.1.x/  renders at /version/v2.1.x/ _versions/v2.x/  renders at /version/v2.x/  noindex
_versions/v2.0.x/  renders at /version/v2.0.x/ (merged into v2.x)
_versions/v1.3.x/  renders at /version/v1.3.x/ _versions/v1.x/  renders at /version/v1.x/  noindex
/version/latest/   meta-refresh page           (gone - the root is latest)
```

Three rules replace the old ones:

1. **One folder per major.** A minor appends to the release notes page and edits or adds pages, with an
   "added in 3.1.0" note where a reader on an older patch needs one. Semver says a minor only adds, so a
   reader never sees something their image lost, only something it does not have yet.
2. **The folder named by `current:` in `_data/versions.yml` renders with no prefix.** Every other folder
   renders at `/version/<folder>/`. Nothing in a folder says where it renders.
3. **No common layer.** Every page belongs to a line. A page that is true for every version lives in the
   current folder and is copied forward with it at the next major.

## Why

- **Search.** Only the current line is indexed. Today a minor moves `current`, so every URL Google ranks goes
  `noindex` and a set it has never seen goes live. It starts over, and every external link points at a dead
  page. Per major at the root, the indexed URLs never move - not at a minor, and not at a major either,
  because `/quick-start/` is still `/quick-start/` when it holds v4 content.
- **The common pages were not common.** Read on 2026-09-11: `core-concepts` lists three algorithms and V4
  has four (`Best` is in `api/v4.md`). `generate-a-client` names `v4.json`, `packCustomBin`,
  `Algorithm.Best` and "V4 is experimental". `configuration-basics` names `/app/Config_Files`,
  `.Production.json` and the `_CONNECTION_STRING` fallback, which are implementation choices. Only
  `integration-guide` is advice that survives any version. ViPaq had already shown it: its common page was
  stripped until it said nothing. Rule 3 ends the claim that any page is timeless.
- **The copy.** A minor copied 52 files, rewrote 18 permalinks, added a config block and edited two data
  files. A minor now edits one folder.
- **One knob stays one knob.** `current:` already moved the redirect and the sitemap. Now it also decides
  which folder has the root, and `tooling/openapi.just` reads it instead of carrying its own copy.

## The steps

**One step, one commit, and the build is green after each.** The check under a step is what reviews that
commit; the end state is checked once, under *Done when*. **C** is a coding session, which may not touch
`sites/`. **S** is a site session, which touches only `sites/docs/`. Tick a step when its commit is in.

**The order is set by one fact.** The gem's root rule and the common layer cannot coexist: with both, the
collision check of step 6 raises on `/`, `/quick-start/` and `/vipaq-protocol/`. So the site deletes the
common layer (step 11) before the gem moves the URLs (step 12). Between those two commits the built site has
no page at `/`. That is fine - the branch never deploys; `Deploy Site` is a dispatch.

Steps 1 to 6 change no URL. Steps 7 and 8 move URLs under `/version/` and cover every move with a redirect.
Steps 9 and 10 put the content where the flip needs it. Steps 11 to 13 are the flip and its cleanup.

### Tooling and data - no URL moves

- [x] **1 - C.** `release-docker-image.yml` publishes a major tag next to the minor one:
      `type=semver,pattern={{major}}`. The docs then pull with `version_tag: "3"` and never name a minor. The
      moving-tag advice in `quick-start.md` and `samples/index.md` changes from "the minor tag" to "the
      major tag" in step 9. **`samples/` at the repo root pins `3` too - the maintainer decided on
      2026-09-11.** The pin, and the `"3"` in `versions.yml`, land only after the `3` tag exists - a pin on
      `main` must name an image that resolves. Both are in the post-release set.
      `grep -n 'pattern={{major}}' .github/workflows/release-docker-image.yml` matches.
- [x] **2 - C.** `tooling/openapi.just` stops carrying `current_docs_version` and reads `current:` from
      `sites/docs/_data/versions.yml`. Done before step 7, so the rename needs no tooling edit.
      `just openapi check-all-copies` passes, and `grep -n 'current_docs_version\|v3\.0' tooling/openapi.just`
      returns nothing.
- [ ] **3 - S.** `_data/versions.yml` gains `version_tag` beside each `id` - the values the four `defaults`
      blocks in `_config.yml` carry today (`3.0`, `2.1.1`, `2.0.1`, `1.3.0`). Same commit: `sites/README.md`
      names `Deploy Site` and its `site` choice, not the three workflows that no longer exist.
      `grep -c version_tag sites/docs/_data/versions.yml` returns 4; nothing reads it yet, so
      `diff -r` of `artifacts/docs` before and after is empty.
- [ ] **4 - C.** The gem stamps `version` from the folder name and `version_tag` from `versions.yml`, and
      no longer needs a `defaults` block per folder. Spec for both.
      `just test rb_binacle-docs-versions_unit` passes, and `diff -r` of `artifacts/docs` before and after
      is empty - the blocks still stamp the same values.
- [ ] **5 - S.** `_config.yml`: the four per-version `defaults` blocks and the comment above them go. The
      `version-current` sitemap and the two `**/swagger/**` blocks stay.
      `grep -c 'v3\.' sites/docs/_config.yml` returns 0, and `diff -r` of `artifacts/docs` is empty.
- [ ] **6 - C.** Three gem additions, each with its spec. **The collision check:** a page outside `_versions/`
      and a page in the current folder claiming the same URL raises and fails the build - Jekyll only warns,
      and a warning is how the wrong page ships. **The selector data:** for the page being rendered, the URL
      of the same path in every other version, or that version's index where the page does not exist. **The
      removed-page list**, printed at build: every page in the previous major's folder with no counterpart in
      the current one. That is the redirect list for a major - not needed for this release, needed the day
      `current` moves to `v4.x`, and cheap while the rule is fresh.
      `just test rb_binacle-docs-versions_unit` passes and `grep -c "^\s*it " ruby/binacle-docs-versions/spec/*_spec.rb`
      grew. The build prints the list and `diff -r` of `artifacts/docs` is empty.

### Folders - URLs move under `/version/`, every move redirected

- [ ] **7 - S.** `v3.0.x` becomes `v3.x` and `v1.3.x` becomes `v1.x` (it only ever held 1.3). `current: v3.x`
      and the `list:` in `versions.yml`. The folder name is replaced in the 61 hand-written `permalink:`
      lines and in every description that names it - they are deleted in step 13, not here, so this commit
      moves URLs and nothing else. `_redirects` is created at the site root and added to `include:`:
      ```
      /version/v3.0.x/*   /version/v3.x/:splat    301
      /version/v1.3.x/*   /version/v1.x/:splat    301
      ```
      `ls sites/docs/collections/_versions/` prints `v1.x v2.0.x v2.1.x v3.x`;
      `test -f artifacts/docs/_redirects`; `grep -rn 'v3\.0\.x\|v1\.3\.x' sites/docs --exclude-dir=node_modules`
      matches only `_redirects` and release notes.
- [ ] **8 - S.** `v2.0.x` and `v2.1.x` become one `v2.x`. Diffed 2026-09-11: they differ in the version label in
      every description, the image tag in the sample files (`2.0.1` against `2.1.1`), and `swagger/` exists
      only in 2.1. So `v2.x` is the 2.1.x content. The 2.0.0 and 2.0.1 sections of the 2.0.x release notes
      merge into the `v2.x` release notes, newest first, and anything the diff shows as 2.1-only - the swagger
      pages, the `swagger:` key on the api pages - gets an "added in 2.1.0" note. **Read the diff; do not
      assume it is only those.** Two more `_redirects` lines: `/version/v2.1.x/*` and `/version/v2.0.x/*` to
      `/version/v2.x/:splat`.
      `grep -c '^## v2\.' sites/docs/collections/_versions/v2.x/release-notes.md` returns 4, the swagger
      pages say "added in 2.1.0", and the build passes. **By eye** for the rest of the diff.

### The common pages - one commit per page

- [ ] **9 - S. Six commits, one per page:** `quick-start`, `vipaq-protocol`, `core-concepts`,
      `configuration-basics`, `integration-guide`, `generate-a-client`. For each: **do not trust either copy.**
      Read it as it stood at `v3.0.0` (`git show v3.0.0:sites/docs/collections/_common_pages/<page>`), read it
      as it is now, diff them, and decide line by line what is a fix and what is v3.1 content. Then:
      - It moves into `v3.x/`. `quick-start.md` and `vipaq-protocol.md` merge into the versioned page of the
        same name. `core-concepts` gains `Best`. `generate-a-client` points at `/swagger/v4.json` and drops the
        "swap the version segment" advice. `quick-start` and `samples/index.md` say "the major tag".
      - **Backfill.** `core-concepts` (three algorithms - no `Best`), `configuration-basics` and
        `integration-guide` go into `v2.x` and `v1.x` too. Same rule: read the page at that line's tag
        (`v2.1.1`, `v1.3.0`), read the current one, carry over only what was true then plus wording fixes made
        since. `generate-a-client` is not backfilled - it did not exist for those lines, and v2.0 had no
        swagger JSON. **This edits old folders.** The freeze rule guards truth; a page that was true for that
        line and links that stop failing the build are both allowed under it.
      - Every `{% link _common_pages/<page> %}` in a versioned page becomes a `{% vlink %}` into its own
        folder. 49 files link to `_common_pages` today: 13 in v3.x, 12 in each v2 line, 17 in v1.x.
      - **The common copy stays** until step 11, so the root URL keeps serving and each commit is about one
        page's content.
      With the first of the six: the text of `pages/index.md` moves into `v3.x/index.md` or goes, so step 11
      can delete the file.
      `grep -rln '_common_pages' sites/docs/collections/_versions/ | wc -l` shrinks with each commit and is 0
      after the sixth; the build passes after each. The session's notes say what was kept from which side.
- [ ] **10 - S.** Includes and script. `_includes/sidebar.html`, `_includes/versions/outdated-notice.html`
      and `_includes/versions/sidebar.html` build `'/version/' + current` by hand; they use what the gem
      stamps. The selector renders on every page, since every page is versioned after step 11. `_js/main.js`
      navigates to the URL the gem stamped for the chosen version, so the reader lands on the same page and
      not the index.
      `grep -rn "'/version/'" sites/docs/_includes sites/docs/_js` returns nothing. **By eye:** open
      `/version/v3.x/configuration/core/`, pick `v2.x`, land on `/version/v2.x/configuration/core/`.

### The flip

- [ ] **11 - S.** The common layer goes: the five pages in `_common_pages/` other than `version.html`,
      `version-latest.html`, and `pages/index.md`. `_redirects` gains `/version/latest/*   /:splat   301` and
      the `v3.0.x` line now points at `/:splat`. The old root `/quick-start/` and `/vipaq-protocol/` need no
      redirect: after step 12 the same URL serves the real page.
      `ls sites/docs/collections/_common_pages/` prints `version.html` alone, and the build passes. The site
      has no page at `/` until step 12 - expected.
- [ ] **12 - C.** The gem decides every URL. **The permalink rule:** for every document in `_versions/`, strip
      the folder segment; if the folder is `current`, `permalink` is `/<rest>/`, else `/version/<folder>/<rest>/`.
      `index.md` maps to its parent path; a `.html` document stays a file, so `swagger/v4.html` keeps its
      shape. Set `doc.data['permalink']` before anything reads `doc.url` - Jekyll memoises the URL on first
      read. **Static files:** `Presets.json`, the compose and yaml files and `swagger/*.json` are `StaticFile`s.
      They take their URL from the collection's `/version/:path/` template and have no `permalink` to
      override. Replace each one in `site.static_files` with a subclass whose `url` applies the same rule.
      This is the fiddly part; write its spec first. **`title_suffix`:** none for `current` - `Quick Start
      (v3.x) - Binacle.Net Docs` is noise on a URL that carries no version; keep it for the others, where it
      stops a title colliding with the root page of the same name. `vlink` needs no change: it finds the file
      and returns whatever URL the file has.
      `just test rb_binacle-docs-versions_unit` passes, and after `bundle exec jekyll build` in `sites/docs`
      the first two *Done when* boxes hold.
- [ ] **13 - S.** The 61 hand-written `permalink:` lines go. The gem overrode them in step 12, so this commit
      changes no output.
      `grep -rn '^permalink:' sites/docs/collections/_versions/` returns nothing, and `diff -r` of
      `artifacts/docs` before and after is empty.

### The agent docs

- [ ] **14 - any session.** `docs/sites/docs.md`: the "one folder per minor" section, the "when a new line
      opens" rule, the common-page rule and the `current` table are rewritten to what the tree now does. The
      new-line rule becomes a new-major rule: copy the folder, remove what the major removed, print the
      removed-page list, write the redirects, move `current:`. `design/sites/decisions.md`: one entry with the
      reasoning under *Why* above, and the pages read on 2026-09-11 as the evidence. The doc that describes
      `openapi.just`: it reads `current:` now.
      **By eye** - the last *Done when* box.

**The 3.1.0 release notes are not a step here.** A `## v3.1.0` section at the top of `v3.x/release-notes.md`
names a date and a link that exist only after the run is green, so on `main` before the tag they would be
lies. It is in the post-release set, with the `version_tag: "3"` and `samples/` pin moves.

## Traps

- **`doc.url` is memoised.** If anything reads it before the generator sets `permalink`, the old URL sticks
  with no error. The spec asserts the rendered URL, not the data key.
- **Static files in a collection are not documents.** `site.documents` does not contain them;
  `site.static_files` does, with `collection` set. Miss them and every download link on the current line
  404s while the pages look right.
- **The swagger pages are `.html` permalinks** (`/version/v3.0.x/swagger/v4.html`). A rule that turns every
  document into `/<rest>/` moves them to `/swagger/v4/`, and `/version/v3.0.x/* -> /:splat` then lands the old
  URL on a 404. Keep a `.html` document a file, or add a redirect line per swagger page.
- **`{% link %}` raises on a missing target.** Delete a common page before the 49 files that link to it are
  rewritten and the build fails on the first one. That is why step 9 keeps the common copies and step 11 is
  the delete. Grep `_common_pages` across `collections/` before and after.
- **`_redirects` starts with an underscore.** Jekyll excludes it silently unless `include:` names it. Check
  `artifacts/docs/_redirects` exists after a build.
- **`verifying-a-release.md` and `generate-a-client.md` print `docs.binacle.net` URLs in prose.** They are
  not `vlink`s and do not move by themselves.
- **Breadcrumbs.** `breadcrumbs: exclude: ["version", "*.*"]` in `_config.yml` starts a versioned trail at its
  own version. A root page has no `version` segment, so its trail starts at home, which is right. Check one of
  each by eye.
- **`og_image` and canonical.** Check one root page and one `/version/` page by eye after step 12.

## Done when

- [ ] The current folder renders at the root and every other folder under `/version/<folder>/`.
      After `bundle exec jekyll build` in `sites/docs`: `test -f artifacts/docs/quick-start/index.html`,
      `test -f artifacts/docs/version/v2.x/quick-start/index.html`, and
      `test ! -d artifacts/docs/version/v3.x`.
- [ ] Static files follow the same rule.
      `test -f artifacts/docs/samples/docker/minimal/Presets.json` and
      `test -f artifacts/docs/swagger/v4.json`.
- [ ] No page in `collections/` decides its own URL.
      `grep -rn '^permalink:' sites/docs/collections/_versions/` returns nothing.
- [ ] `versions.yml` is the one knob.
      `grep -c 'v3\.' sites/docs/_config.yml` returns 0, `grep -n version_tag sites/docs/_data/versions.yml`
      lists one per folder, and `grep -n 'current_docs_version\|v3\.0' tooling/openapi.just` returns nothing.
- [ ] Three folders, no common layer.
      `ls sites/docs/collections/_versions/` prints `v1.x v2.x v3.x`, and
      `ls sites/docs/collections/_common_pages/` prints `version.html` alone.
- [ ] Every old-line link to a common page is a `vlink` into its own folder.
      `grep -rn '_common_pages' sites/docs/collections/_versions/` returns nothing, and the build passes.
- [ ] The six pages were merged from both copies, not taken from one.
      **By eye.** For each, `git diff v3.0.0 -- sites/docs/collections/_common_pages/<page>` was read and
      the session's notes say what was kept from which side. `core-concepts` names `Best` in `v3.x` and does
      not in `v2.x` or `v1.x`.
- [ ] `v2.x` carries the whole v2 line.
      `grep -c '^## v2\.' sites/docs/collections/_versions/v2.x/release-notes.md` returns 4, and the swagger
      pages say "added in 2.1.0". **By eye** for the rest of the diff.
- [ ] Indexing is unchanged in shape: current `✓`, everything else `✗`.
      `grep -L 'noindex' artifacts/docs/version/*/**/index.html` returns nothing;
      `grep -l 'noindex' artifacts/docs/quick-start/index.html` returns nothing;
      `artifacts/docs/sitemap/version-current.xml` lists root URLs only.
- [ ] Every old URL answers with a redirect, not a 404.
      `test -f artifacts/docs/_redirects`, and after deploy `curl -sI docs.binacle.net/version/v3.0.x/api/v3/`,
      `.../version/latest/`, `.../version/v2.0.x/quick-start/` and `.../version/v3.0.x/swagger/v4.html` each
      return `301` with the location `_redirects` says.
- [ ] The selector is on every page and lands on the same page.
      **By eye.** Open `/configuration/core/`, pick `v2.x`, land on `/version/v2.x/configuration/core/`.
      Pick `v1.x` on a page v1 does not have, land on `/version/v1.x/`.
- [ ] The gem's spec suite covers the permalink rule, the static-file rule, the collision check and the
      removed-page list.
      `just test rb_binacle-docs-versions_unit` passes, and
      `grep -c "^\s*it " ruby/binacle-docs-versions/spec/*_spec.rb` grew.
- [ ] `release-docker-image.yml` publishes the major tag.
      `grep -n 'pattern={{major}}' .github/workflows/release-docker-image.yml` matches, and after the next release
      `docker manifest inspect binacle/binacle-net:3` succeeds.
- [ ] The agent docs say what the tree does.
      **By eye.** `docs/sites/docs.md` has no "per minor" and no "when a new line opens"; its `verified:` is
      the day it was checked; `design/sites/decisions.md` has the entry.
