---
description: The docs site keeps one folder per major, renders the current one at the site root, and drops the common-page layer. A minor stops moving every URL.
state: ready
waits-on: "the docs deploy - everything on the branch landed 2026-09-12, both open questions answered the same day; what is left needs the deployed site (the redirect curls, the selector click, the 301 flip) or the release (the major tag manifest)"
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
_versions/v3.0.x/  renders at /version/v3.0.x/ _versions/v3.x/  renders at /                current, indexed
_versions/v2.1.x/  renders at /version/v2.1.x/ _versions/v2.x/  renders at /version/2.1.1/  noindex
_versions/v2.0.x/  renders at /version/v2.0.x/ (merged into v2.x)
_versions/v1.3.x/  renders at /version/v1.3.x/ _versions/v1.x/  renders at /version/1.3.0/  noindex
/version/latest/   meta-refresh page           (gone - the root is latest)
```

Three rules replace the old ones:

1. **One folder per major.** A minor appends to the release notes page and edits or adds pages, with an
   "added in 3.1.0" note where a reader on an older patch needs one. Semver says a minor only adds, so a
   reader never sees something their image lost, only something it does not have yet.
2. **The folder named by `current:` in `_data/versions.yml` renders with no prefix.** Every other folder
   renders at `/version/<url_segment>/`. Every list entry carries four keys and the build stops on a missing
   one: `id` (the folder, never in a URL), `url_segment` (the highest version the line shipped - `2.1.1`,
   `1.3.0`), `label` (what the selector and every page call it - `v2.1.1`), `version_tag` (what docker pulls).
   Nothing in a folder says where it renders. The maintainer set this on 2026-09-12: a closed line is named by
   what it shipped last, not by its folder.
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
commit; the end state is checked once, under *Done when*. "The output diff is empty" means: build
`sites/docs` before and after, then `diff -r -I '<lastmod>' <before> <after>` prints nothing - the sitemaps
carry the build time, nothing else does. **C** is a coding session, which may not touch `sites/`. **S** is a
site session, which touches only `sites/docs/`. Tick a step when its commit is in.

**Two facts set the order.** First: Jekyll takes a collection URL from the folder name, so a folder rename
moves URLs - unless the gem already decides every URL. So the gem's URL rule lands (step 7) before any folder
is renamed (steps 9, 10), and a rename moves nothing. Second: the gem's root rule and the common layer cannot
coexist - the collision check raises on `/`, `/quick-start/` and `/vipaq-protocol/`. So the site deletes the
common layer (step 13) before the current folder moves to the root (step 14). Between those two commits the
built site has no page at `/`. That is fine - the branch never deploys; `Deploy Site` is a dispatch.

**The only URLs that ever move are the ones this plan wants moved:** `/version/v1.3.x/` → `/version/1.3.0/`,
`/version/v2.1.x/` and `/version/v2.0.x/` → `/version/2.1.1/`, and `/version/v3.0.x/` and `/version/latest/`
→ `/`. **Every redirect is a 302 until step 16** - a browser caches a 301, so a wrong one cannot be taken back
while the moves are still being checked.

### Tooling and data - no URL moves

- [x] **1 - C.** `release-docker-image.yml` publishes a major tag next to the minor one:
      `type=semver,pattern={{major}}`. The docs then pull with `version_tag: "3"` and never name a minor. The
      moving-tag advice in `quick-start.md` and `samples/index.md` changes from "the minor tag" to "the
      major tag" in step 11. **`samples/` at the repo root pins `3` too - the maintainer decided on
      2026-09-11.** The pin, and the `"3"` in `versions.yml`, land only after the `3` tag exists - a pin on
      `main` must name an image that resolves. Both are in the post-release set.
      `grep -n 'pattern={{major}}' .github/workflows/release-docker-image.yml` matches.
- [x] **2 - C.** `tooling/openapi.just` stops carrying `current_docs_version` and reads `current:` from
      `sites/docs/_data/versions.yml`. Done before step 9, so the rename needs no tooling edit.
      `just openapi check-all-copies` passes, and `grep -n 'current_docs_version\|v3\.0' tooling/openapi.just`
      returns nothing.
- [x] **3 - S.** `_data/versions.yml` gains `version_tag` beside each `id` - the values the four `defaults`
      blocks in `_config.yml` carry today (`3.0`, `2.1.1`, `2.0.1`, `1.3.0`). Same commit: `sites/README.md`
      names `Deploy Site` and its `site` choice, not the three workflows that no longer exist.
      `grep -c '^ *version_tag:' sites/docs/_data/versions.yml` returns 4; nothing reads it yet, so the output
      diff is empty.
- [x] **4 - C.** The gem stamps `version` from the folder name and `version_tag` from `versions.yml`, and
      no longer needs a `defaults` block per folder. Spec for both.
      `just test rb_binacle-docs-versions_unit` passes, and the output diff is empty - the blocks still stamp the
      same values.
- [x] **5 - S.** `_config.yml`: the four per-version `defaults` blocks and the comment above them go. The
      `version-current` sitemap and the two `**/swagger/**` blocks stay.
      `grep -c 'v3\.' sites/docs/_config.yml` returns 0, and the output diff is empty.
- [x] **6 - C.** Three gem additions, each with its spec. **The collision check:** a page outside `_versions/`
      and a page in the current folder claiming the same URL raises and fails the build - Jekyll only warns,
      and a warning is how the wrong page ships. **The selector data:** for the page being rendered, the URL
      of the same path in every other version, or that version's index where the page does not exist. **The
      removed-page list**, printed at build: every page in the previous major's folder with no counterpart in
      the current one. That is the redirect list for a major - not needed for this release, needed the day
      `current` moves to `v4.x`, and cheap while the rule is fresh.
      `just test rb_binacle-docs-versions_unit` passes and `grep -c "^\s*it " ruby/binacle-docs-versions/spec/*_spec.rb`
      grew. The build prints the list and the output diff is empty.

### The gem decides every URL - still no URL moves

- [x] **7 - C.** The gem sets the URL of every file under `_versions/`, and for now every folder stays under
      `/version/`. **The rule:** strip the folder segment; the URL is `/version/<url_segment>/<rest>` where the
      segment is the list entry's `url_segment:`, or its `id:` when it has none. A document renders as `/<rest>/`
      (`index.md` maps to its parent path); a static file keeps its name and extension. **One shape for every
      page, so the seven swagger pages move from `swagger/v4.html` to `swagger/v4/`** - the only URL this
      step moves; they are noindex, nothing links the `.html` shape, and each old URL gets a redirect line
      when its folder's lines are written (steps 9, 10, 13). Set `doc.data['permalink']` before anything reads
      `doc.url` - Jekyll memoises the URL on first read. **Static files:** `Presets.json`, the compose and yaml
      files and `swagger/*.json` are `StaticFile`s. They are in `site.documents`, but `StaticFile#url` reads
      the collection template and never `data['permalink']`. Replace each one in `site.static_files` (and in
      its collection's `files`) with a subclass whose `url` applies the same rule. This is the fiddly part;
      write its spec first. `vlink` needs no change: it finds the file and returns whatever URL the file has.
      `just test rb_binacle-docs-versions_unit` passes, and the output diff shows only the seven swagger pages
      moving and the seven api pages that link them - with no `url_segment:` set, the rule reproduces every other
      URL exactly, hand-written permalinks included. Measured 2026-09-12: 44 specs, 21 output files differ.
- [x] **8 - S.** The 61 hand-written `permalink:` lines go. The gem overrides them since step 7, so this commit
      changes no output.
      `grep -rn '^permalink:' sites/docs/collections/_versions/` returns nothing, and the output diff is empty.

### Folders - renamed, and the closed lines take their final URLs

- [x] **9a - C.** Each entry in the `versions.yml` list is stamped with `url`, the URL of that version's index.
      `version.html` and the selector build `/version/<id>/` by hand today, which a `url_segment:` would break; they
      read `version.url` instead from step 9 on. Spec for it.
      `just test rb_binacle-docs-versions_unit` passes, and the output diff is empty.
- [x] **9 - S.** `v3.0.x` becomes `v3.x` and `v1.3.x` becomes `v1.x` (it only ever held 1.3). `versions.yml`:
      `current: v3.x`; the `v3.x` entry carries `url_segment: v3.0.x` **for now**, so its URLs do not move before
      the flip, `label: v3.0.0`; the `v1.x` entry carries `url_segment: 1.3.0`, `label: v1.3.0`. Every entry
      carries all four keys and the gem raises on a missing one. In `v3.x` the folder name is replaced in
      `menu_title` and in every description that names it; `v1.x` is not edited - its pages name the 1.3 line,
      which is still what they describe. `_redirects` is created at the site root and added to `include:`:
      ```
      /version/v1.3.x/swagger/v1.html   /version/1.3.0/swagger/v1/    302
      /version/v1.3.x/swagger/v2.html   /version/1.3.0/swagger/v2/    302
      /version/v1.3.x/swagger/v3.html   /version/1.3.0/swagger/v3/    302
      /version/v1.3.x/*                 /version/1.3.0/:splat         302
      ```
      Exact lines before the wildcard: Cloudflare matches an exact path first whatever the order, but a reader
      of the file should not have to know that.
      **Also in this step, because the segment and label break them otherwise:** every template and page that built
      `/version/<id>/` by hand reads the gem's keys instead - `version.html`, the selector and `main.js`
      (option value is the page's URL in that version, from `version_urls`), `sidebar.html`,
      `outdated-notice.html`, the breadcrumb, and the three common pages that linked "the page for
      {{ current }}". That was step 12; it is done here.
      `ls sites/docs/collections/_versions/` prints `v1.x v2.0.x v2.1.x v3.x`;
      `test -d artifacts/docs/version/1.3.0` and `test -d artifacts/docs/version/v3.0.x`;
      `test -f artifacts/docs/_redirects`; `grep -rn "'/version/'" sites/docs --exclude-dir=node_modules`
      returns nothing; every internal link in `artifacts/docs` resolves to a file (checked 2026-09-12: 0
      broken). The output diff, with `v1.3.x` substituted by `1.3.0` in the old output's paths and text, shows
      only the selector, the notice and the titles of the old line - all now saying `1.3.0`.
- [x] **10 - S.** `v2.0.x` and `v2.1.x` become one `v2.x`, `url_segment: 2.1.1`. Diffed 2026-09-11: they differ in the
      version label in every description, the image tag in the sample files (`2.0.1` against `2.1.1`), and
      `swagger/` exists only in 2.1. So `v2.x` is the 2.1.x content. The 2.0.0 and 2.0.1 sections of the 2.0.x
      release notes merge into the `v2.x` release notes, newest first, and anything the diff shows as
      2.1-only - the swagger pages, the `swagger:` key on the api pages - gets an "added in 2.1.0" note.
      **Read the diff; do not assume it is only those.** `_redirects` gains `/version/v2.1.x/*` and
      `/version/v2.0.x/*` to `/version/2.1.1/:splat`, and the two exact swagger lines
      (`/version/v2.1.x/swagger/v2.html` → `/version/2.1.1/swagger/v2/`, same for `v3`).
      **Also in this step, set by the maintainer on 2026-09-12:** no description names a version - the title
      suffix and the URL carry it - so 64 descriptions across the three folders lost their "v2.x"/"v1.3.x"/
      "v3.x"; prose says `{{ page.version_label }}`, not `{{ page.version }}` (13 places); and `vlink` takes a
      version id first (`{% vlink v2.x /index.md %}`) so the four cross-line links resolve a file instead of
      writing a `/version/` url.
      `grep -c '^## v2\.' sites/docs/collections/_versions/v2.x/release-notes.md` returns 4, the swagger
      pages say "added in 2.1.0", `test -d artifacts/docs/version/2.1.1`,
      `grep -rh -A2 '^description' sites/docs/collections/_versions --include=*.md | grep -E '\bv[123](\.[0-9]+)?\.x\b'`
      returns nothing, `grep -rn '{{ page.version }}' sites/docs/collections` returns nothing, and the build
      passes. **By eye** for the rest of the diff.

### The common pages - one commit per page

- [x] **11 - S. Six commits, one per page:** `quick-start`, `vipaq-protocol`, `core-concepts`,
      `configuration-basics`, `integration-guide`, `generate-a-client`. For each: **do not trust either copy.**
      Read it as it stood at `v3.0.0` (`git show v3.0.0:sites/docs/collections/_common_pages/<page>`), read it
      as it is now, diff them, and decide line by line what is a fix and what is v3.1 content. Then:
      - It moves into `v3.x/`. `quick-start.md` and `vipaq-protocol.md` merge into the versioned page of the
        same name. `core-concepts` gains `Best`. `generate-a-client` points at `/swagger/v4.json` and drops the
        "swap the version segment" advice. `quick-start` and `samples/index.md` keep saying "the minor tag" -
        the tag they print is `3.0` until the post-release set moves it to `3`, and the sentence moves with it.
      - **Backfill.** `core-concepts` (three algorithms - no `Best`), `configuration-basics` and
        `integration-guide` go into `v2.x` and `v1.x` too. Same rule: read the page at that line's tag
        (`v2.1.1`, `v1.3.0`), read the current one, carry over only what was true then plus wording fixes made
        since. `generate-a-client` is not backfilled - it did not exist for those lines, and v2.0 had no
        swagger JSON. **This edits old folders.** The freeze rule guards truth; a page that was true for that
        line and links that stop failing the build are both allowed under it.
      - Every `{% link _common_pages/<page> %}` in a versioned page becomes a `{% vlink %}` into its own
        folder. 49 files link to `_common_pages` today: 13 in v3.x, 12 in each v2 line, 17 in v1.x.
      - **The common copy stays** until step 13, so the root URL keeps serving and each commit is about one
        page's content.
      `v3.x/index.md` already carries the intro of `pages/index.md`; each commit adds the moved page's entry to
      it (Versions with the first), so step 13 can delete `pages/index.md`. Its `seo_title` and
      `breadcrumbs: false` move onto `v3.x/index.md` in step 13, when that page becomes the root.
      - [x] `quick-start` - 2026-09-12. The versioned page was already the better one; it gained the cloud
        platforms table and the pin-not-`latest` line from the common page. Nothing else on the common page was
        missing from it, and the common page had not changed since `v3.0.0` apart from the link fix in step 9.
      - [x] `vipaq-protocol` - 2026-09-12. Unchanged since `v3.0.0` but for the step-9 link fix. Its "what ViPaq
        is" intro (purpose, what a string carries) went into all three versioned pages in place of the sentence
        that pointed at the common page; the "strings do not move between versions" section was already on
        each versioned page as its warning block.
      - [x] `core-concepts` - 2026-09-12. Two wording fixes since `v3.0.0` (cartonization; what a heuristic
        does and does not promise), both kept in all three copies. `v3.x` gained a `Best` section and a note
        saying V3 has three heuristics and `Best` is V4; `v2.x` and `v1.x` say only V3 lets you choose. Both
        "consult the API documentation for your version" lines became `vlink`s. 11 links flipped.
      - [x] `configuration-basics` - 2026-09-12. Byte-identical at `v1.3.0`, `v2.1.1`, `v3.0.0` and now, so one
        copy in all three folders; the two "see the documentation for your version" sentences became
        `vlink`s to `/configuration/`. The three anchors the 28 links use are unchanged.
      - [x] `integration-guide` - 2026-09-12. Unchanged since `v1.3.0`. One copy in all three folders; the
        Presets sentence became a `vlink`, one spelling fix. Nothing linked it but `pages/index.md`.
      - [x] `generate-a-client` - 2026-09-12. Did not exist at `v3.0.0` - the whole page is post-tag, so it is
        `v3.x` only. Its document URLs are `{{ site.url }}{% vlink /swagger/v4.json %}` now, so they follow the
        page to the root at the flip without an edit; the "swap the version segment" paragraph went.
      `v3.x/index.md` lists all six. The old lines' index pages were not edited; the sidebar lists the new
      pages by `nav.order`.
      `grep -rn '_common_pages' sites/docs/collections/_versions/ | grep -v version.html` returns nothing (the
      two links to `version.html` stay - that page stays); the build passes; 0 broken internal links. What
      was kept from which side is written beside each page above.
- [x] **12 - S.** Includes and script - **done in step 9**, because the segment broke them there. Left for
      this step: nothing, unless step 13 shows a template still reading `page.version` for display; the
      reader-facing name is `page.url_segment`.
      `grep -rn "'/version/'" sites/docs --exclude-dir=node_modules` returns nothing. **By eye:** open
      `/version/v3.0.x/configuration/core/`, pick `v2.1.1`, land on `/version/2.1.1/configuration/core/`.

### The flip

- [x] **13 - S.** The common layer goes: the five pages in `_common_pages/` other than `version.html`,
      `version-latest.html`, and `pages/index.md`. The `v3.x` entry's `url_segment:` becomes `3.0.0` - unused
      while it is current, and where the line lands the day it closes.
      `_redirects` gains `/version/latest/*   /:splat   302`, `/version/v3.0.x/*   /:splat   302` and the two
      exact swagger lines (`/version/v3.0.x/swagger/v3.html` → `/swagger/v3/`, same for `v4`). The old root
      `/quick-start/` and `/vipaq-protocol/` need no redirect: after step 14 the same URL serves the real
      page.
      **Also in this step, because the deleted pages were linked:** both sidebars' home links (logo, title,
      "General Docs") pointed at `pages/index.md`; they point at the current line's index now, and the
      versioned sidebar shows its back button only on a page that is not current. `v3.x/index.md` carries
      `seo_title` and `breadcrumbs: false` from the deleted root page.
      `ls sites/docs/collections/_common_pages/` prints `version.html` alone, and the build passes. The site
      has no page at `/` until step 14 - expected; the breadcrumb home on `version.html` and `404.html` is the
      one link that does not resolve until then (checked 2026-09-12: that one, nothing else). Current renders
      under `/version/3.0.0/` for the same one commit.
- [x] **14 - C.** The current folder renders at the root: the rule from step 7 gives the folder named by
      `current:` no prefix at all - `/<rest>`. **`title_suffix`:** none for `current` - `Quick Start (v3.0.0) -
      Binacle.Net Docs` is noise on a URL that carries no version; keep it for the others, where it stops a
      title colliding with the root page of the same name. Spec for both. **Left over from step 13, for the
      maintainer:** `_layouts/redirect.html` and the gem's `redirect_to`/`canonical` stamps have no page left
      to serve - `version-latest.html` was their only user. Delete both here, or keep them for the next major.
      `just test rb_binacle-docs-versions_unit` passes, and after `bundle exec jekyll build` in `sites/docs`
      the first two *Done when* boxes hold. Done 2026-09-12: 51 specs; the redirect layout and stamps were
      kept, not deleted - the maintainer has not said.

### The agent docs

- [x] **15 - any session.** `docs/sites/docs.md`: the "one folder per minor" section, the "when a new line
      opens" rule, the common-page rule and the `current` table are rewritten to what the tree now does. The
      new-line rule becomes a new-major rule: copy the folder, remove what the major removed, print the
      removed-page list, write the redirects, give the closed line its `url_segment:`, move `current:`.
      `design/sites/decisions.md`: one entry with the reasoning under *Why* above, and the pages read on
      2026-09-11 as the evidence. The doc that describes `openapi.just`: it reads `current:` now.
      **By eye** - the last *Done when* box. Done 2026-09-12: `docs/sites/docs.md` rewritten (content
      structure, no common layer, the four keys, the new-major rule), `S11` in the sites ledger with the
      per-minor argument as the history it now is, `docs/tooling/README.md` since step 2, the gem README and
      `docs/ruby/README.md` step by step.

### Open - the maintainer's

- [x] **2026-09-12, the maintainer said no - it has no place now.** `version.html` is gone with the
      `common_pages` collection, its `defaults` block, `pages.xml`, the second layout, sidebar and menu, the
      `📚 Versions` entry on the landing page and the link on `generate-a-client.md`. `/version/` redirects to
      `/`. `design/sites/decisions.md#S12` holds it.
      `ls sites/docs/collections/` prints `_versions` alone, and `ls sites/docs/_layouts sites/docs/_includes`
      shows no `versions/` folder.
- [x] **2026-09-12, the maintainer said no.** The "Latest Version Docs" button is gone from both includes.
      `grep -rn latest_version_link_text sites/docs` returns nothing. `_data/sidebar.yml` held only that
      key and is empty of use - delete it.

### After the first deploy

- [ ] **16 - S.** Every line in `_redirects` becomes `301`, once each has been checked on the deployed site with
      the `curl -sI` calls under *Done when*. Until then they are `302`, so a wrong one is not cached.
      `grep -c 302 sites/docs/_redirects` returns 0.

**The sidebar order landed 2026-09-12**, in all three folders: Quick Start, Core Concepts, API, Generate a
Client, Configuration (Basics first inside it), Samples, Integration Guide, ViPaq Protocol, Verifying a
Release, Release Notes. Its plan, `plans/sites/docs-sidebar-order.md`, is ticked and waits for deletion.

**The 3.1.0 release notes are not a step here.** A `## v3.1.0` section at the top of `v3.x/release-notes.md`
names a date and a link that exist only after the run is green, so on `main` before the tag they would be
lies. It is in the post-release set, with the `version_tag: "3"`, `label: v3.1.0`, `url_segment: 3.1.0` and
`samples/` pin moves.

## Traps

- **`doc.url` is memoised.** If anything reads it before the generator sets `permalink`, the old URL sticks
  with no error. The spec asserts the rendered URL, not the data key.
- **Static files in a collection are in `site.documents`, but a `permalink` does nothing to them.** Jekyll 4
  merges `collection.files` into `site.documents`, so the stamps land on their `data` - but `StaticFile#url`
  reads the collection's template and the extension, never `data['permalink']`. Miss them and every download
  link on the current line 404s while the pages look right. Measured 2026-09-12 on the fixture site.
- **The swagger pages were `.html` permalinks** (`/version/v3.0.x/swagger/v4.html`) and are `/swagger/v4/`
  since step 7. A wildcard redirect sends the old `.html` URL to a 404, so every folder's redirect block
  carries an exact line per swagger page.
- **`{% link %}` raises on a missing target.** Delete a common page before the 49 files that link to it are
  rewritten and the build fails on the first one. That is why step 11 keeps the common copies and step 13 is
  the delete. Grep `_common_pages` across `collections/` before and after.
- **`_redirects` starts with an underscore.** Jekyll excludes it silently unless `include:` names it. Check
  `artifacts/docs/_redirects` exists after a build.
- **A `docs.binacle.net` URL written in prose does not move by itself.** Measured 2026-09-12: none is left in
  `_versions/` - `generate-a-client.md` builds its with `{{ site.url }}{% vlink … %}`, and
  `verifying-a-release.md` prints none. Grep before the flip anyway.
- **Breadcrumbs.** `breadcrumbs: exclude: ["version", "*.*"]` in `_config.yml` starts a versioned trail at its
  own version. A root page has no `version` segment, so its trail starts at home, which is right. Check one of
  each by eye.
- **`og_image` and canonical.** Check one root page and one `/version/` page by eye after step 14.

## Done when

- [x] **2026-09-12.** The current folder renders at the root and every closed line under `/version/<url_segment>/`.
      After `bundle exec jekyll build` in `sites/docs`: `test -f artifacts/docs/quick-start/index.html`,
      `test -f artifacts/docs/version/2.1.1/quick-start/index.html`,
      `test -f artifacts/docs/version/1.3.0/quick-start/index.html`, and `ls artifacts/docs/version/`
      prints `1.3.0 2.1.1 index.html` - no folder name, no `latest`.
- [x] **2026-09-12.** Static files follow the same rule.
      `test -f artifacts/docs/samples/docker/minimal/Presets.json` and
      `test -f artifacts/docs/swagger/v4.json`.
- [x] **2026-09-12.** No page in `collections/` decides its own URL.
      `grep -rn '^permalink:' sites/docs/collections/_versions/` returns nothing.
- [x] **2026-09-12.** `versions.yml` is the one knob.
      `grep -c 'v3\.' sites/docs/_config.yml` returns 0, `grep -n version_tag sites/docs/_data/versions.yml`
      lists one per folder, and so do `grep -n 'url_segment:'` and `grep -n 'label:'`, and
      `grep -n 'current_docs_version\|v3\.0' tooling/openapi.just` returns nothing.
- [x] **2026-09-12.** Three folders, no common layer.
      `ls sites/docs/collections/_versions/` prints `v1.x v2.x v3.x`, and `ls sites/docs/collections/` prints
      `_versions` alone.
- [x] **2026-09-12.** Every old-line link to a common page is a `vlink` into its own folder.
      `grep -rn '_common_pages' sites/docs/collections/_versions/` returns nothing, and the build passes.
- [x] **2026-09-12.** The six pages were merged from both copies, not taken from one.
      **By eye.** For each, `git diff v3.0.0 -- sites/docs/collections/_common_pages/<page>` was read and
      the session's notes say what was kept from which side. `core-concepts` names `Best` in `v3.x` and does
      not in `v2.x` or `v1.x`.
- [x] **2026-09-12.** `v2.x` carries the whole v2 line.
      `grep -c '^## v2\.' sites/docs/collections/_versions/v2.x/release-notes.md` returns 4, and the swagger
      pages say "added in 2.1.0". **By eye** for the rest of the diff.
- [x] **2026-09-12.** Indexing is unchanged in shape: current `✓`, everything else `✗`.
      `grep -L 'noindex' artifacts/docs/version/*/**/index.html` returns nothing;
      `grep -l 'noindex' artifacts/docs/quick-start/index.html` returns nothing;
      `artifacts/docs/sitemap/version-current.xml` lists root URLs only.
- [ ] Every old URL answers with a redirect, not a 404. `test -f artifacts/docs/_redirects` holds 2026-09-12; the
      rest is after deploy.
      `test -f artifacts/docs/_redirects`, and after deploy `curl -sI docs.binacle.net/version/v3.0.x/api/v3/`,
      `.../version/latest/`, `.../version/v2.0.x/quick-start/`, `.../version/v1.3.x/` and
      `.../version/v3.0.x/swagger/v4.html` each return `301` with the location `_redirects` says (`302` until
      step 16).
- [ ] The selector is on every page and lands on the same page. The option values were read off the built
      pages 2026-09-12 (`/configuration/core/` → `/version/2.1.1/configuration/core/`); the click is the
      maintainer's.
      **By eye.** Open `/configuration/core/`, pick `v2.1.1`, land on `/version/2.1.1/configuration/core/`.
      Pick `v1.3.0` on a page v1 does not have, land on `/version/1.3.0/`. The selector reads `v3.1.0 (current)`,
      `v2.1.1`, `v1.3.0`.
- [x] **2026-09-12.** The gem's spec suite covers the URL rule with and without `url_segment:`, the
      current-at-root rule, the static-file rule, the collision check and the removed-page list.
      `just test rb_binacle-docs-versions_unit` passes, and
      `grep -c "^\s*it " ruby/binacle-docs-versions/spec/*_spec.rb` grew.
- [ ] `release-docker-image.yml` publishes the major tag. The grep holds since step 1; the manifest is after
      the release.
      `grep -n 'pattern={{major}}' .github/workflows/release-docker-image.yml` matches, and after the next release
      `docker manifest inspect binacle/binacle-net:3` succeeds.
- [x] **2026-09-12.** The agent docs say what the tree does.
      **By eye.** `docs/sites/docs.md` has no "per minor" and no "when a new line opens"; its `verified:` is
      the day it was checked; `design/sites/decisions.md` has the entry.
