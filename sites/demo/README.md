# Binacle.Net Demo Site

The demo site, built with [Jekyll](https://jekyllrb.com/). A chooser index and the three interactive demos.
Its host is <https://demo.binacle.net>.

## 📂 What is in it

| Path | What it is |
|---|---|
| `pages/` | The whole site - the chooser `index.html`, `items.html`, `packing.html`, `vipaq.html`, `404.html`, `robots.txt` |
| `collections/_sitemaps/` | The sitemap source. One file, one collection |
| `_data/` | The header's links, the footer, and the script and style includes |
| `_layouts/`, `_includes/` | Templates and partials. `default.html` is the bare document and `page.html` the wrapper every page gets |
| `_includes/drawings/` | The box cards and the home page's packed box, as SVG. A page pulls one in as an include, so its CSS reaches it |
| `_sass/` | `main.scss`, which names the parts of [`packages/design-system`](../../packages/design-system) this site uses |
| `_js/` | The webpack entry points - `main.js`, `everyday_items.js`, `packing_demo.js`, `protocol_decoder.js`, and `theme-init.ts` for the head |

`css/`, `js/`, `fonts/`, `media/` and the favicons at the root are **generated or copied in** and gitignored -
the sass CLI writes the first, webpack the second, `just assets` the rest. Nothing there is edited by hand.

## 🚀 Develop

From the repo root:

```bash
just serve demo
```

That runs `jekyll serve` and the sass and webpack watches together in one terminal, and one Ctrl-C stops all
three. **Use it rather than `jekyll serve` on its own** - Jekyll alone does not rebuild `_js/` or `_sass/`, so a
script or style change appears to do nothing.

Run `just install` once on a fresh clone, and `just assets` after changing anything under the repo-root
[`assets/`](../../assets) folder - the site serves its own copy, so a new logo does not show up until that runs.

## 🧩 The three demos

Each is a page in `pages/` marked `demo: true`, with their behaviour in a `_js/` entry point that imports the
shared packages from [`packages/`](../../packages) - `@binacle/apps` for all three,
`design-system` for the site chrome. They are resolved through the npm workspace, not copied, so editing a
package changes the site on the next webpack pass.

*Everyday Items* and *Custom Sizes* call a running API. `api_url` in `_config.yml` points it at the local one; `_config.prod.yml`
overrides that for a deploy.

## ⚙️ One thing that bites

`pages/404.html` carries `permalink: /404.html`. Cloudflare is configured with `not_found_handling:
"404-page"`, which looks for that exact name in the site root.

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../../ruby),
wired up through this site's `Gemfile`.
