---
id: sites/demo
description: The published Jekyll demo site at sites/demo/ - a box picker home and the three interactive demos, Everyday Items, Custom Sizes and the ViPaq decoder. The look it takes from the design system, its bundles and its assets. `$sites/demo` always means sites/demo/.
verified: 2026-11-08
check: The page list and permalinks match sites/demo/pages/; _sass/main.scss holds only @use lines into @binacle/design-system; _config.yml has no `sass:` block, sets theme binacle-design-system and lists the same plugins as below; the nav is _data/header.yml; the bundle table matches the entries and cacheGroups in webpack.config.js; the demo/prefetch split still matches _data/includes.yml and _layouts/default.html; the sitemaps: block still writes one file from pages; gulpfile.js still gives demo no assets/lib; the name in tooling/cloudflare/demo.wrangler.jsonc is still binacle-net-demo
also_update:
  - shared/apps
paths:
  - "sites/demo/**"
---

# Demo Site

**`$sites/demo` is the `sites/demo/` folder** - the published demo site, at `demo.binacle.net`. It is off
limits from a coding session.

Output goes to `../../artifacts/demo`. Worker `binacle-net-demo`.

```bash
just serve demo   # jekyll serve (port 7196) + sass and webpack watches, one Ctrl-C stops all three
just build demo   # the same site built once, into artifacts/demo
```

## The look comes from the design system

**The site holds no CSS of its own.** `_sass/main.scss` is a list of `@use` lines, each one part of
`@binacle/design-system`. A part the site needs is added to that list, not written here. Why the package is
built this way is `$shared/design-system`.

**Some includes are not in `_includes/`.** `theme: binacle-design-system` in `_config.yml` makes Jekyll read the
package's `_includes/` after this site's. The copy button comes from there. An include here of the same name
wins.

## Pages

| Page | Route | What it is |
|---|---|---|
| `pages/index.html` | `/` | Pick a box: three box cards, each opening *Everyday Items* with `?box=` set, and tiles to the other two demos |
| `pages/items.html` | `/items/` | *Everyday Items* - pick a box, add things, see where each lands |
| `pages/packing.html` | `/packing/` | *Custom Sizes* - your own bins and items, and the algorithm |
| `pages/vipaq.html` | `/vipaq/` | The ViPaq decoder |
| `pages/404.html` | `/404.html` | Error page |

**The nav is `_data/header.yml`**, written by hand. A new page is not in it until it is added there.

The three demo pages carry `demo: true` and name their own entry script under `scripts:` in front matter. The
apps behind them, and the names their markup must use, are `$shared/apps`.

**There is no sitemap file and no sitemaps collection.** `jekyll-multi-sitemap` generates `/sitemap.xml` from
the `sitemaps:` block in `_config.yml`, and `pages/robots.txt` is `{% robots %}` for the body and
`{% sitemap_links %}` for its `Sitemap:` line.

**Its `robots.txt` carries `nav: exclude: true`** so the three sites' files are the same file. Only docs reads
`nav.exclude`, and only for its sidebar, so the line does nothing here.

## JS Bundles

Webpack bundles from `sites/demo/_js/` and npm packages into `sites/demo/js/`:

| Bundle | What it is |
|---|---|
| `main.js` | Site-wide: the design system's theme button, menu drawer, code wrap, copy buttons and lightbox |
| `everyday_items.js`, `packing_demo.js`, `protocol_decoder.js` | One per demo page. Each starts Alpine with its app's plugin; the decoder's also holds the *Samples* drawer |
| `binacle-net-ui.js` | `@binacle/apps`, the API client and the design system's 3D view, in one chunk. Why it keeps that name is `$shared/apps` |
| `binacle-vipaq.js` | `vipaq/packages/vipaq`, the TypeScript ViPaq decoder |
| `three.js` | Three.js on its own, by far the largest - demo pages only |
| `runtime.js`, `vendors.js` | The webpack runtime and the other npm dependencies, Alpine among them. Every page |
| `theme-init.js` | The pre-paint theme read, loaded blocking in `<head>` |

The four page scripts are the entry points of the main webpack config; the rest of that config's output is
split chunks. `theme-init` is a second config in the same file, built from `_js/theme-init.ts`, which calls the
design system's `theme-init` entry: the head script has to be one self-contained file with no runtime or vendors
chunk. The main config's `clean` keeps `theme-init.js` so the two do not delete each other.

**The demo bundles are split out on purpose, and `sites/demo/_data/includes.yml` is the one list that decides
it.** `runtime.js`, `main.js` and `vendors.js` load on every page. `three.js`, `binacle-net-ui.js` and
`binacle-vipaq.js` load only where the front matter says `demo: true`; every other page **prefetches** that same
list, so arriving at a demo costs no download. Both halves read the one list, so they cannot drift apart.

**`{% prefetch_tags %}` writes the prefetch links and `{% script_tags %}` executes them**, off that one
`demo_scripts` list. The prefetch links carry the list's `type: text/javascript` as well.

## Plugins

Nine, in `_config.yml` order: `jekyll-tidy`, `jekyll-gtm`, `jekyll-filters`, `binacle-robots`,
`jekyll-multi-sitemap`, `jekyll-resource-tags`, `jekyll-page-meta`, `jekyll-structured-data`,
`jekyll-webmanifest`. The two the docs site loads and this one does not are `jekyll-breadcrumb-trail` and
`binacle-docs` - a flat site has no trail and no versions. `binacle-robots` is one of the site gems,
`$sites/ruby`.

**The head is `{% page_meta %}` then `{% structured_data %}`, and there is no seo include.** Each demo page
names its node with `structured_data: type: WebApplication` in front matter; the `offers`,
`applicationCategory`, `operatingSystem` and `browserRequirements` under it come from
`structured_data: defaults:` in `_config.yml`, and the `organization:` block there is byte-identical to the
one in www and docs.

## Fonts and assets

**No vendored library reaches this site.** `gulpfile.js` gives `demo` nothing from `assets/lib/`; Alpine and
Three.js come from npm through webpack. `just assets` copies the favicons and the link preview from `assets/`,
the two fonts from the design system into `fonts/`, and the design system's mark to
`media/logo/binacle-logo.svg`, the address `logo.src` in `_config.yml` names. `_data/includes.yml` preloads both
fonts.
