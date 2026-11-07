# Marketing site

The Jekyll site published at `www.binacle.net` - the pages a stranger lands on. The documentation is a
different site next door in [`../docs`](../docs), and the browser demos are in [`../demo`](../demo).

**It is built differently from the other two, in three ways that will catch you out.** They are the section
below.

## 📂 What is in it

| Path | What it is |
|---|---|
| `pages/` | The pages, plus `404.html` and `robots.txt` |
| `_sass/` | `main.scss`, which names the parts of `@binacle/design-system` this site uses. Compiled by the sass CLI, **not by Jekyll** |
| `_js/` | The TypeScript source: the parts of `@binacle/design-system` this site calls - the theme button, the menu drawer and the rest - and the pre-paint theme script |
| `_data/` | The header nav, the footer, and `exchange.yml` - every request and response the site shows |
| `_layouts/` | `default.html` is the bare document, `page.html` the header, main and footer around a page |
| `_includes/` | The header and footer, the exchange, the run command, the trade-off list, and `drawings/`, one SVG per picture |

## 🛠️ Building and serving

From the repo root:

```bash
just serve www                   # jekyll serve on :7197, sass and webpack watching beside it
just build www                   # the same site built once, into artifacts/www
just check links www             # the internal links in what was just built
```

`just build www` is three steps in a fixed order - copy the shared assets, compile the stylesheet and the
bundle, then `jekyll build`. **Running `jekyll build` on its own succeeds and ships a site with no styles at
all.** Use the recipes.

## ⚠️ What will bite you

**Jekyll compiles no sass here.** `_config.yml` carries no `sass:` block on purpose. `npm run build:css` runs
the sass CLI over `_sass/main.scss` and writes `css/main.css`, the same way `Binacle.Net.UIModule` does. Put
the block back and two compilers have a claim on the same files. It also means **`css/` is gitignored** - it
is build output, like `js/`.

**The look comes from `shared/packages/design-system`.** `build:css` reaches it by `pkg:`, so a part this site needs
goes in `_sass/main.scss`. The asset copy puts the package's `fonts/` in `fonts/`, which is gitignored like `css/`,
and skips everything under `assets/` this site does not read: `gulpfile.js` lists it.

**Some includes are not in `_includes/`.** `_config.yml` sets `theme: binacle-design-system`, a gem the
`Gemfile` loads from `shared/packages/design-system`, and Jekyll reads its `_includes/` after this site's. The copy
button comes from there. An include here of the same name wins.

**Every code block on the site is a real response.** They live in `_data/exchange.yml`, and that file says how
to re-run them. A response that looks plausible but is not what the API returns is the one thing this audience
checks.

**Every example is v3.** v4 is experimental. A marketing page must not hand a
stranger an unstable contract.

**`sitemap.xml` lists this site's pages and nothing else.** The old `/apps/*` paths move to the demo host and
will be served here as redirects; a sitemap that listed them would be a sitemap full of 301s, which is what
Search Console reports as an error.
