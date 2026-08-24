---
id: sites/docs
description: The published Jekyll documentation site at sites/docs/ — versioned API docs with Swagger UI embed. `$sites/docs` always means sites/docs/, never .agents/docs/.
verified: 2026-08-24
check: Collections, versions, plugin list, and version folders match sites/docs/_config.yml and sites/docs/collections/_versions/; `current` and `list` in sites/docs/_data/versions.yml match the folders and the order the sidebar renders; the common-page rule matches what is actually on collections/_common_pages/; the webpack entry, output and `clean` behaviour match sites/docs/webpack.config.js; a built artifacts/docs still has `noindex, follow` on every non-current version page, none on the current one, and no sitemap listing a `noindex` URL; sites/docs/_plugins/ is still empty and every plugin the site loads is a gem under ruby/; the sitemaps: block in _config.yml still writes pages.xml and version-current.xml under /sitemap/ with an index at /sitemap.xml
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
| `collections/_versions/` | Versioned docs — each subfolder is a version |
| `collections/_common_pages/` | Pages shared across all versions |
| `pages/` | Top-level pages (index, 404, robots.txt) |

Versioned docs are served at `/version/:path/`. See "Versioning model" below.

### What may go on a common page {#common-page-rule}

A `_common_pages/` page renders **once**, at `/<name>/`, and every version's reader sees that one copy. So it
carries only what is true for **all** of them: what a thing is, why it exists, and advice that does not turn on
the version. Anything that varies goes on the versioned page and is linked to.

**Never on a common page**, using the ViPaq pair as the worked example — these are the categories that have
already gone wrong or are reserved for change:

- The compression codec, or that compression happens at all.
- Base64, or any statement about the stored or text form.
- Integer encoding — fixed widths, width codes, "variable length encoding".
- The header, its size, or any byte or bit layout.
- Structure diagrams, field order, or the body layouts.
- **Whether a feature is experimental or stable.** ViPaq was experimental through v2.1.1 and is stable from
  v3.0.0, so the claim is version-varying like any other. Added 2026-08-07, when that flip left the general
  ViPaq page saying "experimental" to a v3.0.0 reader.
- A real config key, an endpoint path, or an API version number.
- Comparative performance claims — they describe an implementation, and implementations change.

`configuration-basics.md` is the model to copy: it teaches the mechanism with entirely invented names
(`AModule`, `Settings__Logs__Retention`), so it has nothing to go stale, and it defers on the specifics.

Two mechanical notes for a common page that needs to point at a versioned one. It has no `version` in its front
matter, so **`vlink` cannot be used** — build the URL from `site.data.versions.current` instead, the same way
`_layouts/redirect.html` and `_includes/sidebar.html` do, and it follows `current` with no edit. And never
hardcode a version or `latest` in prose or a command without saying what it tracks.

## Page metadata

**Every page carries a written `description`** in its front matter - all 118, every version line included.
`jekyll-page-meta` still falls back to the excerpt and then the site description, cut at 160 characters,
which severs mid-word; that fallback is a safety net for a page that forgets, not the mechanism.

**`seo_title` overrides the composed title verbatim.** The composed form is
`<title> (<version>) - <site.title>`, where the version half is the `title_suffix` that
`binacle-docs-versions` stamps; a page that sets `seo_title` gets exactly that string and **nothing is
appended**, so a page using it writes its own suffix.

**Nav labels and breadcrumbs use `menu_title` where a page sets one**, falling back to `title`
(`_includes/versions/menu.html`, and the `title_from` list in the site's `breadcrumbs:` config). It exists
so a page can carry a title that is unique across the site while the sidebar keeps a short label - two
sample pages named `Minimal` under different parents read fine in a tree and collide in a `<title>`.
**`nav.parent` still matches on `title`, not on `menu_title`**, so renaming a page that has children breaks the tree.

## Versioning model

**Every folder is a version; there is no moving folder.** Folders are `vMAJOR.MINOR.x` — one per minor line
(`v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x`). The current line is edited in place; when a new line opens, its folder
is copied and the old one is never touched again. `/version/latest/` survives only as a **redirect** to
`current`, holding no content.

**The one knob:** `current` in `sites/docs/_data/versions.yml` says which folder is current, where the
`latest` redirect points, and which folder search engines may index. One edit per new line. That file also
carries `list`, the rendered version order — newest first, because the order is read from the file rather
than sorted.

### What `current` decides about search {#search-and-current}

Everything below reads `current`; nothing names a version.

| | Current version | Every other version |
|---|---|---|
| `<meta name="robots">` | none | `noindex, follow` |
| Listed in a sitemap | yes | no |

- **Neither value is written by a layout any more.** `binacle-docs-versions` stamps `robots` and
  `title_suffix` onto every versioned document at a high priority, and `{% page_meta %}` writes them out —
  `Quick Start (v3.0.x) - Binacle.Net Docs` — so a versioned page cannot collide with the same page at the
  site root or with another version of itself. `_layouts/versions/swagger.html` calls the same tag.
- **The sitemaps are generated, not written.** `jekyll-multi-sitemap` reads the `sitemaps:` block in
  `_config.yml`: `version-current.xml` selects the `versions` collection where `version` matches
  `site.data.versions.current`, and `pages.xml` covers `pages/` and `_common_pages/`. Both are served under
  `/sitemap/`, with an index over them at `/sitemap.xml`.
- Swagger pages are `noindex, nofollow` in every version, current included. A `**/swagger/**` defaults block
  in `_config.yml` sets that `robots` value in page data, where the stamp leaves it alone, and keeps them out
  of the sitemap. A submitted `noindex` URL is a Search Console error.
- `robots.txt` writes its `Sitemap:` line with `{% sitemap_links %}`, which emits the index alone, so no
  version-agnostic edit is needed there either.

**Why per-minor, not per-major.** A folder answers "what does my image do", and the API set is what changes:
versions are **added at minors** (v1.2.0 added API v3) and **removed at majors** (v2.0.0 removed v1, v3.0.0
removes v2). Per-major would show a v3 to a v1.1.4 image that never had it. Per-minor also caught the swagger UI:
`v2.0.x` has no `swagger/` while `v1.3.x`, `v2.1.x` and `v3.0.x` all do, so the folder tree records that it was
there, went away, and came back — which a per-major tree could not have shown. Patches never move the docs
(every patch pair in history is byte-identical across `sites/docs/`). This makes the freeze **structural** — an
old folder is frozen because nothing edits it, not because someone remembered to snapshot it. That discipline
is exactly what failed before: four releases (v2.0.0 → v2.1.1) shipped with no snapshot, and only one folder
was ever authored.

**Never derive a folder from an API tag.** The tree at a tag is whatever was in the repo that day — maybe
mid-edit. Copy the current folder the moment a new line opens; that is the only sound source.

**The `swagger/` json in a version folder is generated output**, not hand-written. `just openapi generate`
writes `artifacts/openapi/Binacle.Net_v3.json` and `_v4.json`; they are copied in as `swagger/v3.json` and
`swagger/v4.json`, so the rename is part of the copy. **Regenerate, never hand-edit** — a hand edit puts the
published spec out of step with what the code serves, and the diff hides inside whatever else was edited.

### When a new line opens (standing rule)

A line opens on every new **minor** (`v3.0.x` → `v3.1.x`, or `v3.1.x` → `v4.0.x`):

1. `cp -r _versions/v3.0.x _versions/v3.1.x` — copy the folder the new line grows out of.
2. Rewrite every `permalink`/`menu_title`:
   `grep -rl "/version/v3\.0\.x/" v3.1.x/ | xargs sed -i 's|/version/v3\.0\.x/|/version/v3.1.x/|g'`
3. Add the folder's `defaults` block in `sites/docs/_config.yml`, or it is invisible in the selector.
4. Add it to the top of `list` in `_data/versions.yml` and point `current` at it (also moves the `latest`
   redirect).
5. `bundle exec jekyll build` to confirm.
6. Edit only the new folder. **Never touch an old one** — that is what keeps it true.

**Watch out:**
- `vlink` (`ruby/binacle-docs-versions`) **raises and fails the build** on a missing target. Removing a page
  without removing its `vlink` references breaks the build — grep the page name before deleting.
- Selector order comes from `list` in `_data/versions.yml`, newest first. It is read, not sorted — Jekyll's
  own ordering is by path, which would put `v3.10.x` before `v3.2.x`.

## Plugins

| Plugin | Source |
|---|---|
| `jekyll-gtm` | `ruby/jekyll-gtm` |
| `jekyll-filters` | `ruby/jekyll-filters` |
| `jekyll-multi-sitemap` | `ruby/jekyll-multi-sitemap` |
| `jekyll-resource-tags` | `ruby/jekyll-resource-tags` |
| `jekyll-page-meta` | `ruby/jekyll-page-meta` |
| `jekyll-structured-data` | `ruby/jekyll-structured-data` |
| `jekyll-breadcrumb-trail` | `ruby/jekyll-breadcrumb-trail` |
| `binacle-docs-versions` | `ruby/binacle-docs-versions` |
| `jekyll-tidy` | gem |

**`sites/docs/_plugins/` is empty.** VLink lived there and moved into `binacle-docs-versions` with the
version stamps, which is where it gets a spec suite; nothing under `sites/` has one.

**breadcrumbs** (`{% breadcrumbs %}`) — one call in `_includes/header.html` renders the trail for every
page, versioned or not. The two thirty-line includes and the branch that chose between them went on
24 Aug 2026. `breadcrumbs: exclude: ["version", "*.*"]` in `_config.yml` is what keeps a versioned trail
starting at its own version; **drop it and every breadcrumb on the site silently gains two crumbs.**
A page still turns its trail off with `breadcrumbs: false`, which now works everywhere rather than only on
versioned pages.

**vlink** (`{% vlink path %}`) — resolves a relative path to the correct versioned URL based on the
current page's `version` front matter. Use it instead of plain links inside `_versions/` pages
so links stay correct across versions.

## JS and Vendor Libs

Webpack bundles `sites/docs/_js/main.js` → `sites/docs/js/main.js` (entry `main`, ts-loader for `.ts`).

**`clean` is on only for a production build** (`env.build=dist`), not in watch mode. Watch shares
`sites/docs/js/` with a running jekyll, and deleting a file jekyll has already listed makes its next `File.stat`
raise `ENOENT` and kills `just serve docs`. So a watch run leaves stale bundles behind on purpose;
`just build docs` is what clears them.

Vendor libs the docs site loads:
- BeerCSS — theming (`/lib/beercss/`, via `sites/docs/_data/includes.yml`)
- Swagger UI — embedded OpenAPI explorer, loaded in the `versions/swagger.html` layout

Note: docs does **not** use Alpine.js or material-dynamic-colors (neither is referenced anywhere under
`sites/docs/`). Don't assume they're available here.
