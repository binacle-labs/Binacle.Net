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

## The work

Three kinds of session. A coding session may not touch `sites/`; a site session touches only `sites/docs/`.

### 1. The gem - `ruby/binacle-docs-versions` - coding session

The generator already stamps every versioned document at high priority. It gains:

- **The permalink rule.** For every document in `_versions/`: strip the folder segment; if the folder is
  `current`, `permalink` is `/<rest>/`, else `/version/<folder>/<rest>/`. `index.md` maps to its parent
  path. Set `doc.data['permalink']` before anything reads `doc.url` - Jekyll memoises the URL on first read.
  The 18 hand-written `permalink:` lines in the folders go, so the gem is the only place a URL is decided.
- **Static files.** `Presets.json`, the compose and yaml files and `swagger/*.json` are `StaticFile`s. They
  take their URL from the collection's `/version/:path/` template and have no `permalink` to override. Replace
  each one in `site.static_files` with a subclass whose `url` applies the same rule. This is the fiddly part;
  write its spec first.
- **`version` from the folder name** instead of a `defaults` block per folder. **`version_tag` from
  `versions.yml`**, one field beside each `id`.
- **A collision check.** A page outside `_versions/` and a page in the current folder claiming the same URL
  raises and fails the build. Jekyll only warns, and a warning is how the wrong page ships.
- **The selector data.** For the page being rendered, the URL of the same path in every other version, or
  that version's index if the page does not exist there. The script then lands on the same page instead of
  the index.
- **A removed-page list**, printed at build: every page in the previous major's folder with no counterpart
  in the current one. That is the redirect list for a major. Not needed for this release; needed the day
  `current` moves to `v4.x`, and cheap to write while the rule is fresh.

`vlink` needs no change: it finds the file and returns whatever URL the file has.

### 2. Config, tooling, workflow - coding session

- `sites/docs/_config.yml` is under `sites/` - the four per-version `defaults` blocks are removed by the
  site session in step 3, once the gem stamps `version` itself.
- `tooling/openapi.just:46` stops carrying `current_docs_version`; it reads `current:` from
  `sites/docs/_data/versions.yml`.
- `release-docker-image.yml` publishes a major tag next to the minor one: `type=semver,pattern={{major}}`.
  The docs pull with `version_tag: "3"` and never name a minor. The moving-tag advice in `quick-start.md:37` and
  `samples/index.md:39` changes from "the minor tag" to "the major tag". **`samples/` at the repo root pins
  `3` too - the maintainer decided on 2026-09-11.** A minor then changes nothing in `samples/`, in the docs
  copies of them, or in the READMEs that name the tag. The pin lands only after the `3` tag exists, which
  the first release with the workflow change creates - a pin on `main` must name an image that resolves.

### 3. The site - `sites/docs/` - site session

**Folders.**
- `v3.0.x` becomes `v3.x`. `current: v3.x`.
- `v2.0.x` and `v2.1.x` become one `v2.x`. Diffed 2026-09-11: they differ in the version label in every
  description, the image tag in the sample files (`2.0.1` against `2.1.1`), and `swagger/` exists only in
  2.1. So `v2.x` is the 2.1.x content. The 2.0.0 and 2.0.1 sections of the 2.0.x release notes merge into
  the `v2.x` release notes, newest first, and anything the diff shows as 2.1-only - the swagger pages, the
  `swagger:` key on the api pages - gets an "added in 2.1.0" note. **Read the diff; do not assume it is only
  those.**
- `v1.3.x` becomes `v1.x`. It only ever held 1.3.
- Every hand-written `permalink:` line in the folders is deleted.

**The six common pages.** Each moves into `v3.x/`; `quick-start.md` and `vipaq-protocol.md` merge into the
versioned page of the same name, and `version-latest.html` is deleted. **Do not trust either copy.** For each
page: read it as it stood at `v3.0.0` (`git show v3.0.0:sites/docs/collections/_common_pages/<page>`), read it
as it is now, diff them, and decide line by line what is a fix and what is v3.1 content. `core-concepts` gains
`Best`. `generate-a-client` points at `/swagger/v4.json` and drops the "swap the version segment" advice.

**Backfill.** `core-concepts` (three algorithms - no `Best`), `configuration-basics` and `integration-guide`
go into `v2.x` and `v1.x` too. 38 pages in the old lines link to `_common_pages/` today (16 in v1.3.x, 11 in
each v2 line), so without the pages those links fail the build. Same rule: read each page at the tag of that
line (`v2.1.1`, `v1.3.0`), read the current one, and carry over only what was true then plus wording fixes
made since. `generate-a-client` is not backfilled - it did not exist for those lines, and v2.0.x has no
swagger JSON. Every `{% link _common_pages/... %}` in a versioned page becomes a `{% vlink %}` into its own
folder. **This edits old folders.** The freeze rule guards truth; a page that was true for that line and
links that stop failing the build are both allowed under it.

**Redirects.** The site is Cloudflare Workers static assets (`tooling/cloudflare/docs.wrangler.jsonc`), which
reads a `_redirects` file at the output root. Jekyll drops underscore files unless they are in `include:`.
The file:

```
/version/latest/*   /:splat                 301
/version/v3.0.x/*   /:splat                 301
/version/v2.1.x/*   /version/v2.x/:splat    301
/version/v2.0.x/*   /version/v2.x/:splat    301
/version/v1.3.x/*   /version/v1.x/:splat    301
```

The old root `/quick-start/` and `/vipaq-protocol/` need no redirect: the same URL now serves the real page.

**Includes and script.**
- `_includes/sidebar.html:13`, `_includes/versions/outdated-notice.html:6` and
  `_includes/versions/sidebar.html:14` build `'/version/' + current` by hand. They use what the gem stamps.
- The selector renders on every page, since every page is versioned now. `_js/main.js` navigates to the URL
  the gem stamped for the chosen version.
- `pages/index.md` at `/` is replaced by the current folder's `index.md`. Its text moves or goes.
- `_config.yml`: the four per-version `defaults` blocks go; `_redirects` is added to `include:`; the
  `version-current` sitemap and the `**/swagger/**` block stay as they are.

**The 3.1.0 release notes.** A `## v3.1.0` section at the top of `v3.x/release-notes.md`, from
`just changelog extract 3.1.0`, with the release date and the GitHub release link. **This one edit waits for
the tag** - the date and the link do not exist before the run is green, so on `main` they would be lies for
the length of the gap. Everything else in this plan is true before the tag and can land before it.

### 4. The agent docs - any session

- `docs/sites/docs.md`: the "one folder per minor" section, the "when a new line opens" rule, the
  common-page rule and the `current` table are rewritten to what the tree now does. The new-line rule becomes
  a new-major rule: copy the folder, remove what the major removed, print the removed-page list, write the
  redirects, move `current:`.
- `design/sites/decisions.md`: one entry with the reasoning under *Why* above, and the pages read on
  2026-09-11 as the evidence.
- `docs/tooling.md` or wherever `openapi.just` is described: it reads `current:` now.

## Traps

- **`doc.url` is memoised.** If anything reads it before the generator sets `permalink`, the old URL sticks
  with no error. The spec asserts the rendered URL, not the data key.
- **Static files in a collection are not documents.** `site.documents` does not contain them;
  `site.static_files` does, with `collection` set. Miss them and every download link on the current line
  404s while the pages look right.
- **`{% link %}` raises on a missing target.** Move a common page before the 43 pages that link to it are
  rewritten and the build fails on the first one. Grep `_common_pages` across `collections/` before and after.
- **`_redirects` starts with an underscore.** Jekyll excludes it silently unless `include:` names it. Check
  `artifacts/docs/_redirects` exists after a build.
- **`verifying-a-release.md` and `generate-a-client.md` print `docs.binacle.net` URLs in prose.** They are
  not `vlink`s and do not move by themselves.
- **Breadcrumbs.** `breadcrumbs: exclude: ["version", "*.*"]` in `_config.yml` starts a versioned trail at its
  own version. A root page has no `version` segment, so its trail starts at home, which is right. Check one of
  each by eye.
- **`og_image`, canonical and `title_suffix`.** The gem stamps `title_suffix: (v3.x)` from `version`. On the
  current line at the root that suffix is noise - `Quick Start (v3.x) - Binacle.Net Docs` for a page whose
  URL carries no version. Drop it for `current`; keep it for the others, where it stops a title colliding
  with the root page of the same name.

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
      lists one per folder, and `grep -n 'current_docs_version\|v3\.' tooling/openapi.just` returns nothing.
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
      `test -f artifacts/docs/_redirects`, and after deploy `curl -sI docs.binacle.net/version/v3.0.x/api/v3/`
      and `.../version/latest/` and `.../version/v2.0.x/quick-start/` each return `301` with the location the
      table above says.
- [ ] The selector is on every page and lands on the same page.
      **By eye.** Open `/configuration/core/`, pick `v2.x`, land on `/version/v2.x/configuration/core/`.
      Pick `v1.x` on a page v1 does not have, land on `/version/v1.x/`.
- [ ] The gem's spec suite covers the permalink rule, the static-file rule, the collision check and the
      removed-page list.
      `just test rb_binacle-docs-versions_unit` passes, and
      `grep -c "^\s*it " ruby/binacle-docs-versions/spec/*_spec.rb` grew.
- [ ] `release.yml` publishes the major tag.
      `grep -n 'pattern={{major}}' .github/workflows/release-docker-image.yml` matches, and after the next release
      `docker manifest inspect binacle/binacle-net:3` succeeds.
- [ ] `v3.x/release-notes.md` opens with `## v3.1.0`, a date and the release link, and the body is the
      `3.1.0` changelog section. **By eye** against `just changelog extract 3.1.0`.
- [ ] The agent docs say what the tree does.
      **By eye.** `docs/sites/docs.md` has no "per minor" and no "when a new line opens"; its `verified:` is
      the day it was checked; `design/sites/decisions.md` has the entry.
