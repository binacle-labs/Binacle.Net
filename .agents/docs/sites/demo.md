---
id: sites/demo
description: The published Jekyll demo site at sites/demo/ — a chooser index and the two interactive demos, the packing demo and the ViPaq decoder. `$sites/demo` always means sites/demo/.
verified: 2026-08-24
check: Collections, JS bundles and plugin list match sites/demo/_config.yml and sites/demo/js/; sites/demo/_includes/ still has no seo.html and pages/index.html still prints item.summary on the cards; the demo/prefetch script split still matches sites/demo/_data/includes.yml; the sitemaps: block in _config.yml still writes one file and /sitemap.xml still lists the three pages; artifacts/demo/lib/ after `just build demo` holds exactly the vendor folders listed, and gulpfile.js's IGNORE map still explains what is missing
also_update:
  - packages
paths:
  - "sites/demo/**"
---

# Demo Site

**`$sites/demo` is the `sites/demo/` folder** — the published demo site. It is off limits from a coding
session; see `.agents/README.md`.

Jekyll site at `sites/demo/`. The public demo site for Binacle.Net.
Built with Jekyll + webpack + TypeScript. Output goes to `../../artifacts/demo`.

Run locally, or build it once:

```bash
just serve demo   # jekyll serve (port 7196) + webpack watch, one Ctrl-C stops both
just build demo   # the same site built once, into artifacts/demo
```

## Pages

| Page | Route | What it is |
|---|---|---|
| `pages/index.html` | `/` | The chooser — one card per tool, and the card is the link |
| `pages/packing.html` | `/packing/` | The packing demo |
| `pages/vipaq.html` | `/vipaq/` | The ViPaq decoder |
| `pages/404.html` | `/404.html` | Error page |

**There is no collection for the tools.** They are pages carrying `applet: true` and an `order`, and the
chooser, the two navs and the JSON-LD block all key off that flag.

**There is no sitemap file and no sitemaps collection.** `jekyll-multi-sitemap` generates `/sitemap.xml` from
the `sitemaps:` block in `_config.yml`, and `pages/robots.txt` is `{% robots %}` for the body and
`{% sitemap_links %}` for its `Sitemap:` line.

**Its `robots.txt` gained a `nav: exclude: true` line** on 24 Aug 2026, so the three sites' files are one
file. Only docs reads `nav.exclude`, and only for its sidebar, so the line does nothing here.

## JS Bundles

Webpack bundles from `sites/demo/_js/` and npm packages into `sites/demo/js/`:

| Bundle | What it is |
|---|---|
| `main.js` | Site-wide JS (theme, navigation) |
| `packing_demo.js` | Interactive packing demo — calls the Binacle.Net API |
| `protocol_decoder.js` | ViPaq protocol decoder — decodes pack results without calling the API |
| `binacle-net-ui.js` | Built from `packages/binacle-net-ui` — UI components and 3D visualizer |
| `binacle-vipaq.js` | Built from `vipaq/packages/binacle-vipaq` — TypeScript ViPaq decoder |
| `runtime.js` | Webpack runtime, loaded on every page |
| `vendors.js` | Shared npm dependencies, loaded on every page |
| `three.js` | Three.js on its own, **601 KiB** — demo pages only |

Only `main`, `packing_demo` and `protocol_decoder` are webpack entry points; the rest are split chunks or
package builds.

**The demo bundles are split out on purpose, and `sites/demo/_data/includes.yml` is the one list that decides
it.** `runtime.js`, `main.js` and `vendors.js` load on every page. `three.js`, `binacle-net-ui.js` and
`binacle-vipaq.js` load only where the front matter says `demo: true`; every other page **prefetches** that same
list, so arriving at a demo costs no download. Both halves read the one list, so they cannot drift apart.

**`{% prefetch_tags %}` writes the prefetch links and `{% script_tags %}` executes them**, off that one
`demo_scripts` list. The prefetch links carry the list's `type: text/javascript` as well, which the old
hand-written include dropped.

## Plugins

Same as the docs site: `jekyll-gtm`, `jekyll-filters`, `jekyll-multi-sitemap`, `jekyll-resource-tags`,
`jekyll-page-meta`, `jekyll-structured-data`, `jekyll-tidy` — everything but `binacle-docs-versions`, which
is the docs site's own.

**The head is `{% page_meta %}` then `{% structured_data %}`, and there is no seo include.** The two demo
pages name their node with `structured_data: type: WebApplication` in front matter; the `offers`,
`applicationCategory`, `operatingSystem` and `browserRequirements` under it come from
`structured_data: defaults:` in `_config.yml`, and the `organization:` block there is byte-identical to the
one in www and docs.

**`description` is the search sentence and `summary` is the card copy.** The chooser on `/` prints
`item.summary`; putting the card blurb back on `description` ships it as a search snippet.

## Vendor Libs

`sites/demo/lib/` holds two vendor folders — `beercss` and `material-dynamic-colors` — and **only BeerCSS is
loaded**, as a stylesheet and a module in `sites/demo/_data/includes.yml`.

- **material-dynamic-colors** is present and commented out there; uncomment it only when the site needs runtime
  theme switching.
- **`swagger-ui` is in `assets/` but never copied here.** `gulpfile.js` carries a per-target `IGNORE` map: the
  docs site's swagger layout is the only thing that loads it, so it reaches `sites/docs/` alone. That map is
  also what keeps it out of the image.

Alpine.js arrives as an npm dependency bundled by webpack, which is the copy the demo code uses — there is no
vendored copy any more.

**Three.js is not in `vendors.js`** — it is its own bundle, for the reason in the table above.

Built size is **3.0 MB**, of which 1.14 MB is three Material Symbols `woff2` files. `beer.min.css` declares
four `@font-face` families and a browser downloads only the one a page uses, so that weight is in the deploy
and not in the page load.
