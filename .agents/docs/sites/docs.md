---
id: sites/docs
description: The published Jekyll documentation site at sites/docs/ — versioned API docs with Swagger UI embed. `$sites/docs` always means sites/docs/, never .agents/docs/.
verified: 2026-09-12
check: Collections, plugin list, and version folders match sites/docs/_config.yml and sites/docs/collections/_versions/ - one folder per major, named vN.x; every folder has an entry in sites/docs/_data/versions.yml carrying id, url_segment, label and version_tag, in the order the sidebar renders; collections/_common_pages/ holds version.html alone and pages/ holds 404.html and robots.txt alone; no file under collections/_versions/ carries a permalink; a built artifacts/docs renders the current folder at the root and every other under /version/<url_segment>/, has `noindex, follow` on every non-current version page and none on a root page, no sitemap listing a `noindex` URL, and a _redirects file at its root; the webpack entry, output and `clean` behaviour match sites/docs/webpack.config.js; sites/docs/_plugins/ still does not exist and every plugin the site loads except jekyll-tidy is a gem under ruby/, in the order _config.yml lists them; the sitemaps: block in _config.yml still writes pages.xml and version-current.xml under /sitemap/ with an index at /sitemap.xml
paths:
  - "sites/docs/**"
---

# Docs Site

**`$sites/docs` is the `sites/docs/` folder** — the published site, not `.agents/docs/` (the agent docs you
are reading). It is off limits from a coding session; see `.agents/README.md`.

Jekyll site at `sites/docs/`. The public API documentation for Binacle.Net users.
Built with Jekyll + webpack + TypeScript. Output goes to `../../artifacts/docs`.

Run locally, or build it once:

```bash
just serve docs   # jekyll serve (port 7195) + webpack watch, one Ctrl-C stops both
just build docs   # the same site built once, into artifacts/docs
```

## Content Structure

| Path | What it is |
|---|---|
| `collections/_versions/` | Every page. One folder per major line - `v1.x`, `v2.x`, `v3.x` |
| `collections/_common_pages/` | `version.html` alone - the list of lines at `/version/` |
| `pages/` | `404.html` and `robots.txt` |
| `_data/versions.yml` | The one knob: which folder is current, and the four keys of every folder |
| `_redirects` | Every old URL and where it went. Cloudflare reads it from the output root |

The current folder renders at the site root; every other folder under `/version/<url_segment>/`. See
"Versioning model" below.

### No common layer {#common-page-rule}

**Every page belongs to a line.** There is no page that renders once for every version. A page that is true
for every version - what an algorithm does, how configuration files are laid out - lives in the current folder
and is copied forward with it at the next major, like every other page. The six pages that used to sit outside
the versions moved in on 2026-09-12; the ledger entry `$sites/decisions#S11` holds why, with what each of them
had quietly come to say.

**Two consequences for a page in a version folder.** It may name real config keys, endpoint paths, API versions
and whether a feature is experimental - the folder says which release those hold for. And it links another page
with `{% vlink %}`, never with `{% link %}` into some shared place, because there is none.

## Page metadata

**Every page carries a written `description`** in its front matter - all 96 under `_versions/`, every line
included - and **no description names a version**; see below.
`jekyll-page-meta` still falls back to the excerpt and then the site description, cut at 160 characters,
which severs mid-word; that fallback is a safety net for a page that forgets, not the mechanism.

**`seo_title` overrides the composed title verbatim.** The composed form is
`<title> (<label>) - <site.title>` on a closed line and `<title> - <site.title>` on the current one; the
version half is the `title_suffix` that `binacle-docs-versions` stamps, and it stamps none on the current
line because its URL carries no version. A page that sets `seo_title` gets exactly that string and
**nothing is appended**, so a page using it writes its own suffix. `v3.x/index.md` does, for the root.

**Nav labels and breadcrumbs use `menu_title` where a page sets one**, falling back to `title`
(`_includes/versions/menu.html`, and the `title_from` list in the site's `breadcrumbs:` config). It exists
so a page can carry a title that is unique across the site while the sidebar keeps a short label - two
sample pages named `Minimal` under different parents read fine in a tree and collide in a `<title>`.
**`nav.parent` still matches on `title`, not on `menu_title`**, so renaming a page that has children breaks the tree.

## Versioning model

**One folder per major, and the current one renders at the root.** Folders are `vMAJOR.x` - `v1.x`, `v2.x`,
`v3.x`. A minor edits the current folder in place: it appends to the release-notes page and edits or adds
pages, with an "added in 3.1.0" note where a reader on an older patch needs one. Semver says a minor only adds,
so a reader never sees something their image lost, only something it does not have yet. A major copies the
folder; the old one is never touched again. The reasoning is `$sites/decisions#S11`.

**Nothing in a folder says where it renders.** `binacle-docs-versions` decides every URL from
`_data/versions.yml`, static files included, and overwrites any `permalink` a page writes. The folder named by
`current` renders with no prefix - `/quick-start/`, `/swagger/v4.json`; every other folder renders at
`/version/<url_segment>/…`.

**Every folder has one entry in `_data/versions.yml`, and every entry carries four keys.** The build stops on
a folder with no entry or an entry missing one:

| Key | Is | `v3.x` today | `v2.x` |
|---|---|---|---|
| `id` | the folder. Never in a URL | `v3.x` | `v2.x` |
| `url_segment` | where a closed line renders: the highest version shipped, unused while current | `3.0.0` | `2.1.1` |
| `label` | what the selector and every page call the line | `v3.0.0` | `v2.1.1` |
| `version_tag` | what docker pulls - a closed line's newest patch, the current line's moving tag | `3.0` | `2.1.1` |

`current` is the one knob: it names the folder at the root, the line search engines may index, and the line
the selector marks. `list` is the rendered order - newest first, because it is read from the file rather than
sorted. A release edits the current entry's `label` (and `url_segment`, for the day it closes); nothing else.

**The gem also stamps what templates read.** On every page: `version` (the folder), `version_label`,
`version_tag`, and `version_urls` - this page's URL in every other line, or that line's index where the page is
missing, which is what the selector's options carry. On every list entry: `url`, the line's index URL. No
template builds a `/version/` URL by hand, and a `grep -rn "'/version/'" sites/docs` finding one is a bug.

### What `current` decides about search {#search-and-current}

Everything below reads `current`; nothing names a version.

| | Current version | Every other version |
|---|---|---|
| `<meta name="robots">` | none | `noindex, follow` |
| Listed in a sitemap | yes | no |

- **Neither value is written by a layout.** `binacle-docs-versions` stamps `robots` and `title_suffix` onto
  every page of a closed line at a high priority, and `{% page_meta %}` writes them out —
  `Quick Start (v2.1.1) - Binacle.Net Docs` — so an old page cannot collide with the same page at the root.
  The current line gets neither. `_layouts/versions/swagger.html` calls the same tag.
- **The sitemaps are generated, not written.** `jekyll-multi-sitemap` reads the `sitemaps:` block in
  `_config.yml`: `version-current.xml` selects the `versions` collection where `version` matches
  `site.data.versions.current` - root URLs only - and `pages.xml` covers `pages/` and `_common_pages/`, which
  is `/version/` and `/404.html`. Both are served under `/sitemap/`, with an index over them at `/sitemap.xml`.
- Swagger pages are `noindex, nofollow` in every version, current included. A `**/swagger/**` defaults block
  in `_config.yml` sets that `robots` value in page data, where the stamp leaves it alone, and keeps them out
  of the sitemap. A submitted `noindex` URL is a Search Console error.
- `robots.txt` is `{% robots %}` for the body and `{% sitemap_links %}` for the `Sitemap:` line, which
  emits the index alone, so no
  version-agnostic edit is needed there either.

**A description names no version.** The title carries it as the suffix the gem stamps (`V3 (v2.1.1) -
Binacle.Net Docs`) and the URL carries it too; a description that said "Binacle.Net v3.x" would have to change
at every release, and one that said "v3.0.0" would be wrong at the next minor. Prose that needs the version
writes `{{ page.version_label }}`, never `{{ page.version }}` - that is the folder id. A link into another
line is `{% vlink v2.x /path %}`, never a `/version/` url written by hand.

**Never derive a folder from an API tag.** The tree at a tag is whatever was in the repo that day — maybe
mid-edit. Copy the current folder the moment a new line opens; that is the only sound source.

**The `swagger/` json in a version folder is generated output**, not hand-written. `just openapi generate`
writes `artifacts/openapi/Binacle.Net_v3.json` and `_v4.json`; they are copied in as `swagger/v3.json` and
`swagger/v4.json`, so the rename is part of the copy. **Regenerate, never hand-edit** — a hand edit puts the
published spec out of step with what the code serves, and the diff hides inside whatever else was edited.

### When a new major opens (standing rule)

A line opens on every new **major** (`v3.x` → `v4.x`) and never on a minor:

1. `cp -r _versions/v3.x _versions/v4.x` — copy the folder the new line grows out of. Nothing inside names its
   URL, so there is no permalink to rewrite; fix `menu_title` on the index page.
2. Remove from `v4.x` what the major removed. Then build once: the gem prints every page the previous folder
   has that the current one lacks (`Docs versions: N pages in v3.x have no counterpart in v4.x`). That is the
   redirect list.
3. In `_data/versions.yml`: add the `v4.x` entry at the top of `list` with its four keys and point `current`
   at it. The line that just closed keeps its entry; its `url_segment` is where it renders from now on.
4. In `_redirects`: one line per removed page, from its old root URL to wherever it went. The pages that
   survived need none - `/quick-start/` is still `/quick-start/`. A line's old URLs never move again.
5. `bundle exec jekyll build` to confirm, and check every internal link resolves.
6. Edit only the new folder. **Never touch an old one** — that is what keeps it true.

**Watch out:**
- `vlink` (`ruby/binacle-docs-versions`) **raises and fails the build** on a missing target. Removing a page
  without removing its `vlink` references breaks the build — grep the page name before deleting.
- Selector order comes from `list` in `_data/versions.yml`, newest first. It is read, not sorted — Jekyll's
  own ordering is by path, which would put `v3.10.x` before `v3.2.x`.

## Plugins

Eleven, in `_config.yml` order. **This is the site that loads every gem** — the other two are flat, so
neither takes `jekyll-breadcrumb-trail`, and only this one is versioned, so only it takes
`binacle-docs-versions`.

| Plugin | Source |
|---|---|
| `jekyll-tidy` | gem |
| `jekyll-gtm` | `ruby/jekyll-gtm` |
| `jekyll-filters` | `ruby/jekyll-filters` |
| `binacle-robots` | `ruby/binacle-robots` |
| `jekyll-multi-sitemap` | `ruby/jekyll-multi-sitemap` |
| `jekyll-resource-tags` | `ruby/jekyll-resource-tags` |
| `jekyll-page-meta` | `ruby/jekyll-page-meta` |
| `jekyll-structured-data` | `ruby/jekyll-structured-data` |
| `jekyll-webmanifest` | `ruby/jekyll-webmanifest` |
| `jekyll-breadcrumb-trail` | `ruby/jekyll-breadcrumb-trail` |
| `binacle-docs-versions` | `ruby/binacle-docs-versions` |

**There is no `sites/docs/_plugins/` directory.** `_config.yml` still declares `plugins_dir: _plugins` and
Jekyll tolerates it being absent. VLink lived there and moved into `binacle-docs-versions` with the version
stamps, which is where it gets a spec suite; nothing under `sites/` has one.

**breadcrumbs** (`{% breadcrumbs %}`) — one call in `_includes/header.html` renders the trail for every
page, versioned or not. The two thirty-line includes and the branch that chose between them went on
24 Aug 2026. `breadcrumbs: exclude: ["version", "*.*"]` in `_config.yml` is what keeps a versioned trail
starting at its own version; **drop it and every breadcrumb on the site silently gains two crumbs.**
A page still turns its trail off with `breadcrumbs: false`, which now works everywhere rather than only on
versioned pages.

**`_layouts/redirect.html` has no page left.** It served `/version/latest/`, which went when the current line
moved to the root - `/version/latest/*` is a line in `_redirects` now. The layout and the gem's
`redirect_to`/`canonical` stamps are still there for a page that sets `layout: redirect`; none does.

**vlink** (`{% vlink path %}`) — resolves a path inside the current page's version folder to the URL the gem
gave that file; `{% vlink v2.x path %}` resolves it inside another line. Use it for every link inside
`_versions/`; a missing target fails the build.

## JS and Vendor Libs

Webpack bundles `sites/docs/_js/main.js` → `sites/docs/js/main.js` (entry `main`, ts-loader for `.ts`). A
`vendors` split chunk is configured but its `minSize` is 20000, so a built site is `main.js` alone. A second
config in the same file builds `_js/theme-init.ts` → `js/theme-init.js`, the pre-paint theme read loaded
blocking in `<head>`; it is its own config, not a second entry, because that script has to be one
self-contained file with no runtime or vendors chunk.

**`clean` is on only for a production build** (`env.build=dist`), not in watch mode. Watch shares
`sites/docs/js/` with a running jekyll, and deleting a file jekyll has already listed makes its next `File.stat`
raise `ENOENT` and kills `just serve docs`. So a watch run leaves stale bundles behind on purpose;
`just build docs` is what clears them. The production `clean` keeps `theme-init.js`, because the two configs
run in parallel into the one directory.

Vendor libs the docs site loads:
- BeerCSS — theming (`/lib/beercss/`, via `sites/docs/_data/includes.yml`)
- Swagger UI — embedded OpenAPI explorer, loaded in the `versions/swagger.html` layout

Note: docs does **not** use Alpine.js or material-dynamic-colors (neither is referenced anywhere under
`sites/docs/`). Don't assume they're available here.
