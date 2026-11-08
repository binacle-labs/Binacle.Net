---
id: sites/docs
description: The published Jekyll documentation site at sites/docs/ — versioned API docs with Swagger UI embed. `$sites/docs` always means sites/docs/, never .agents/docs/.
verified: 2026-11-08
check: Collections, plugin list, and version folders match sites/docs/_config.yml and sites/docs/collections/_versions/ - one folder per major, named vN.x; every folder has an entry in sites/docs/_data/versions.yml carrying id, url_segment, label and version_tag, in the order the sidebar renders; collections/ holds _versions alone, pages/ holds 404.html and robots.txt alone, and _layouts and _includes have no versions/ subfolder; no file under collections/_versions/ carries a permalink; a built artifacts/docs renders the current folder at the root and every other under /version/<url_segment>/, has `noindex, follow` on every non-current version page and none on a root page, no sitemap listing a `noindex` URL, and a _redirects file at its root; the webpack entry, output, `clean` behaviour and splitChunks: false match sites/docs/webpack.config.js; sites/docs/_plugins/ still does not exist and every plugin the site loads except jekyll-tidy is a gem under ruby/ or sites/ruby/, in the order _config.yml lists them; the breadcrumbs: block's title_from is [menu_title, title] and _layouts/page.html renders page.breadcrumb_trail itself; the sitemaps: block in _config.yml writes version-current.xml alone under /sitemap/ with an index at /sitemap.xml; the top-level nav.order sequence is the same in every folder with Release Notes second
paths:
  - "sites/docs/**"
---

# Docs Site

**`$sites/docs` is the `sites/docs/` folder** — the published site, not `.agents/docs/` (the agent docs you
are reading). It is off limits from a coding session.

Jekyll site at `sites/docs/`. The public API documentation for Binacle.Net users.
Built with Jekyll, Sass and webpack + TypeScript. Output goes to `../../artifacts/docs`.

Run locally, or build it once:

```bash
just serve docs   # jekyll serve (port 7195) + sass and webpack watches, one Ctrl-C stops all three
just build docs   # the same site built once, into artifacts/docs
```

## Content Structure

| Path | What it is |
|---|---|
| `collections/_versions/` | Every page. One folder per major line - `v1.x`, `v2.x`, `v3.x` |
| `pages/` | `404.html` and `robots.txt` |
| `_data/versions.yml` | The one knob: which folder is current, and the four keys of every folder |
| `_redirects` | Every old URL and where it went. Cloudflare reads it from the output root |

The current folder renders at the site root; every other folder under `/version/<url_segment>/`. See
"Versioning model" below.

### No common layer {#common-page-rule}

**Every page belongs to a line.** There is no page that renders once for every version, and no collection
but `versions` - one layout (`default`, `swagger` for the embed), one sidebar, one menu. A page that is true
for every version - what an algorithm does, how configuration files are laid out - lives in the current folder
and is copied forward with it at the next major, like every other page. Why: `$sites/decisions#S11`. There is
no version list page; the selector in the sidebar is the list (`$sites/decisions#S12`).

**Two consequences for a page in a version folder.** It may name real config keys, endpoint paths, API versions
and whether a feature is experimental - the folder says which release those hold for. And it links another page
with `{% vlink %}`, never with `{% link %}` into some shared place, because there is none.

### One sidebar order, every folder {#sidebar-order}

The top level reads the same in every folder, minus the pages a line does not have: **Quick Start, Release
Notes, Core Concepts, API, Generate a Client, Configuration, Samples, ViPaq Protocol, Verifying a Release.**
Release Notes is always second. The maintainer, 2026-09-12: "rule release notes always after quick start".
`nav.order` in each page's front matter
is the number; a new top-level page is slotted in and the numbers after it move. Under Configuration the
Core page lists **one child page per file it reads** (`Presets.json`, `ForwardedHeaders.json`, `Cors.json`),
the way Diagnostics already does. An in-page contents list appears only on the API reference pages; the
sidebar is the contents for everything else.

## Page metadata

**Every page carries a written `description`** in its front matter, every line included, and **no description names a version**; see below.
`jekyll-page-meta` still falls back to the excerpt and then the site description, cut at 160 characters,
which severs mid-word; that fallback is a safety net for a page that forgets, not the mechanism.

**`seo_title` overrides the composed title verbatim.** The composed form is
`<title> (<label>) - <site.title>` on a closed line and `<title> - <site.title>` on the current one; the
version half is the `title_suffix` that `binacle-docs` stamps, and it stamps none on the current
line because its URL carries no version. A page that sets `seo_title` gets exactly that string and
**nothing is appended**, so a page using it writes its own suffix. `v3.x/index.md` does, for the root.

**The sidebar, the header's menu button and the breadcrumbs use `menu_title` where a page sets one**, falling
back to `title` (`_includes/menu.html`, `_includes/header.html`, and the `title_from` list in the site's
`breadcrumbs:` config). It exists so a page can carry a title that is unique across the site while the sidebar
keeps a short label - two sample pages named `Minimal` under different parents read fine in a tree and collide
in a `<title>`.
**`nav.parent` still matches on `title`, not on `menu_title`**, so renaming a page that has children breaks the tree.

## Icons {#icons}

**The markdown keeps its emoji; the page shows only the page's own.** `doc_body`, in `sites/ruby/binacle-docs`,
drops the emoji that opens a heading, a paragraph, a list item or a list item's first link. A page's `nav.icon`
goes on its title (`_layouts/page.html`) and, on a top-level page, on its sidebar entry (`_includes/menu.html`)
and the header's menu button (`_includes/header.html`).
A keycap (`1️⃣`) numbers a menu entry and never goes on a title. The home page, `home: true`, skips `doc_body`
and keeps every emoji it is written with. The design system's `emoji` block tones them down per theme.
Why: `$sites/decisions#S13`.

## Versioning model

**One folder per major, and the current one renders at the root.** Folders are `vMAJOR.x` - `v1.x`, `v2.x`,
`v3.x`. A minor edits the current folder in place: it appends to the release-notes page and edits or adds
pages, with an "added in 3.1.0" note where a reader on an older patch needs one. Semver says a minor only adds,
so a reader never sees something their image lost, only something it does not have yet. A major copies the
folder; the old one is never touched again. The reasoning is `$sites/decisions#S11`.

**An old line takes the new look, never new words.** Every folder shares the layouts, includes and styles, so a
restyle reaches them all; the markdown of a closed line stays as it was. The `v2.x` and `v1.x` home pages are
lists of headings that are only links, and `.doc__heading--2:has(> a)` in the design system's `layout/doc` sets
them small and close. Why: `$sites/decisions#S15`.

**Nothing in a folder says where it renders.** `binacle-docs` decides every URL from
`_data/versions.yml`, static files included, and overwrites any `permalink` a page writes. The folder named by
`current` renders with no prefix - `/quick-start/`, `/swagger/v4.json`; every other folder renders at
`/version/<url_segment>/…`.

**Every folder has one entry in `_data/versions.yml`, and every entry carries four keys.** The build stops on
a folder with no entry or an entry missing one:

| Key | Is | `v3.x` today | `v2.x` |
|---|---|---|---|
| `id` | the folder. Never in a URL | `v3.x` | `v2.x` |
| `url_segment` | where a closed line renders: the highest version shipped, unused while current | `3.1.0` | `2.1.1` |
| `label` | what the selector and every page call the line | `v3.1.0` | `v2.1.1` |
| `version_tag` | what docker pulls - a closed line's newest patch, the current line's major tag | `3` | `2.1.1` |

`current` is the one knob: it names the folder at the root, the line search engines may index, and the line
the selector marks. `list` is the rendered order - newest first, because it is read from the file rather than
sorted. A release edits the current entry's `label` (and `url_segment`, for the day it closes); nothing else.

**The gem also stamps what templates read** - the version keys on every page, `prev_next` for the pager, and a
`url` on every list entry; the list is `$sites/ruby`. The selector's options carry `version_urls`. No
template builds a `/version/` URL by hand, and a `grep -rn "'/version/'" sites/docs` finding one is a bug.

### What `current` decides about search {#search-and-current}

Everything below reads `current`; nothing names a version.

| | Current version | Every other version |
|---|---|---|
| `<meta name="robots">` | none | `noindex, follow` |
| Listed in a sitemap | yes | no |

- **Neither value is written by a layout.** `binacle-docs` stamps `robots` and `title_suffix` onto
  every page of a closed line at a high priority, and `{% page_meta %}` writes them out —
  `Quick Start (v2.1.1) - Binacle.Net Docs` — so an old page cannot collide with the same page at the root.
  The current line gets neither. `_layouts/swagger.html` calls the same tag.
- **The sitemaps are generated, not written.** `jekyll-multi-sitemap` reads the `sitemaps:` block in
  `_config.yml`: one file, `version-current.xml`, selects the `versions` collection where `version` matches
  `site.data.versions.current` - root URLs only. It is served under `/sitemap/`, with an index over it at
  `/sitemap.xml`. There is no `pages.xml`: `404.html` is excluded and nothing else lives outside the
  collection.
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
writes `artifacts/openapi/Binacle.Net_v3.json` and `_v4.json`; `just openapi sync-all-copies` copies them into
the current folder as `swagger/v3.json` and `swagger/v4.json`, and `check-all-copies` fails on drift. **Regenerate, never hand-edit** — a hand edit puts the
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
- `vlink` (`sites/ruby/binacle-docs`) **raises and fails the build** on a missing target. Removing a page
  without removing its `vlink` references breaks the build — grep the page name before deleting.
- Selector order comes from `list` in `_data/versions.yml`, newest first. It is read, not sorted — Jekyll's
  own ordering is by path, which would put `v3.10.x` before `v3.2.x`.

## Plugins

In `_config.yml` order. **It loads every plugin `www` and `demo` do, and two more**: the other two are flat, so
neither takes `jekyll-breadcrumb-trail`, and only this one is versioned, so only it takes `binacle-docs`. It
takes no Jekyll theme; its layouts and includes are its own.

| Plugin | Source |
|---|---|
| `jekyll-tidy` | gem |
| `jekyll-gtm` | `ruby/jekyll-gtm` |
| `jekyll-filters` | `ruby/jekyll-filters` |
| `binacle-robots` | `sites/ruby/binacle-robots` |
| `jekyll-multi-sitemap` | `ruby/jekyll-multi-sitemap` |
| `jekyll-resource-tags` | `ruby/jekyll-resource-tags` |
| `jekyll-page-meta` | `ruby/jekyll-page-meta` |
| `jekyll-structured-data` | `ruby/jekyll-structured-data` |
| `jekyll-webmanifest` | `ruby/jekyll-webmanifest` |
| `jekyll-breadcrumb-trail` | `ruby/jekyll-breadcrumb-trail` |
| `binacle-docs` | `sites/ruby/binacle-docs` |

**There is no `sites/docs/_plugins/` directory.** `_config.yml` still declares `plugins_dir: _plugins` and
Jekyll tolerates it being absent. Every plugin is a gem; the site holds no Ruby.

**breadcrumbs** — the gem writes `page.breadcrumb_trail` and `_layouts/page.html` renders it in the design
system's `breadcrumbs` markup; the `{% breadcrumbs %}` tag is not called. A page with `nav.exclude`, the home
page, shows *Docs* alone. `breadcrumbs: exclude: ["version", "*.*"]` in `_config.yml` is what keeps a versioned
trail starting at its own version; **drop it and every breadcrumb on the site silently gains two crumbs.**
`breadcrumbs: false` empties a page's trail.

**`_layouts/redirect.html` has no page left.** `/version/latest/*` is a line in `_redirects`. The layout and the gem's
`redirect_to`/`canonical` stamps are still there for a page that sets `layout: redirect`; none does.

**vlink** (`{% vlink path %}`) — resolves a path inside the current page's version folder to the URL the gem
gave that file; `{% vlink v2.x path %}` resolves it inside another line. Use it for every link inside
`_versions/`; a missing target fails the build.

## JS and Vendor Libs

Webpack bundles `sites/docs/_js/main.ts` → `sites/docs/js/main.js` (entry `main`, ts-loader for `.ts`). It takes
its behaviour from the design system. **There is no chunk split**, as on `www`: the page loads `main.js` alone,
and a `vendors` chunk the page never loads stops `main.js` from running. A watch build keeps three.js, which the
design system's index names, so a split would make one. A second
config in the same file builds `_js/theme-init.ts` → `js/theme-init.js`, the pre-paint theme read loaded
blocking in `<head>`; it is its own config, not a second entry, because that script has to be one
self-contained file with no runtime or vendors chunk.

**`clean` is on only for a production build** (`env.build=dist`), not in watch mode. Watch shares
`sites/docs/js/` with a running jekyll, and deleting a file jekyll has already listed makes its next `File.stat`
raise `ENOENT` and kills `just serve docs`. So a watch run leaves stale bundles behind on purpose;
`just build docs` is what clears them. The production `clean` keeps `theme-init.js`, because the two configs
run in parallel into the one directory.

**Swagger UI is the one vendor library**, and only `_layouts/swagger.html` loads it. Docs does **not** use
Alpine.js or material-dynamic-colors. Don't assume they're available here.

**There is no live 3D view.** The home page's picture is a still inside the design system's `pack-view` frame,
and the site loads no three.js.

## Swagger UI {#swagger}

**A page with `swagger:` in its front matter gets a *Swagger* button.** It opens a wide `panel`, the drawer from
the right, with the Swagger page in an iframe (`_layouts/page.html`). The page around it takes the design
system; the frame keeps Swagger UI's own look, and `_sass/_swagger.scss` sets only its size. The vendored
version is in `assets/README.md`.

**The frame follows the site's theme.** A page in a frame cannot read the site's theme, so `initSwaggerTheme`
in `_js/main.ts` sets Swagger's own `dark-mode` class on the frame's root, on load and on every open. Opened
on its own, at `/swagger/v3/`, the page stays light: Swagger UI does not read the system theme.
`_layouts/swagger.html` paints the light ground and the dark scrollbar Swagger leaves out. Why:
`$sites/decisions#S14`.
