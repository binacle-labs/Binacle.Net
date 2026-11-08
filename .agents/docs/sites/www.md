---
id: sites/www
description: The published Jekyll marketing site at sites/www/ - its pages, the look it takes from the design system, the exchange data every API sample comes from, and what bites.
verified: 2026-11-08
check: The page list and permalinks match sites/www/pages/; _sass/main.scss holds only @use lines into @binacle/design-system and _js/main.ts only calls it; _config.yml has no `sass:` block and sets theme binacle-design-system; _layouts/default.html still calls {% page_meta %} then {% structured_data %}; the structured_data: organization: block in _config.yml is byte-identical to the one in sites/docs and sites/demo; the sitemaps: block still writes one file from pages; _data/exchange.yml still names v3 routes that exist in artifacts/openapi/Binacle.Net_v3.json, and its command still names the major tag; the name in tooling/cloudflare/www.wrangler.jsonc is still binacle-net-www
paths:
  - "sites/www/**"
---

# WWW Site

Jekyll site at `sites/www/`, published at `www.binacle.net`. It is the marketing site — what Binacle.Net does
and who it is for. It is off limits from a coding session.

Output goes to `../../artifacts/www`. Worker `binacle-net-www`.

```bash
just serve www    # jekyll serve (port 7197) + sass and webpack watches, one Ctrl-C stops all three
just build www    # the same site built once, into artifacts/www
```

## The look comes from the design system

**The site holds no CSS and almost no script of its own.** `_sass/main.scss` is a list of `@use` lines, each
one part of `@binacle/design-system`; `_js/main.ts` calls the package's scripts. A part the site needs is added
to that list, not written here. Why the package is built this way is `$shared/design-system`.

**Some includes are not in `_includes/`.** `theme: binacle-design-system` in `_config.yml` makes Jekyll read the
package's `_includes/` after this site's. The copy button comes from there. An include here of the same name
wins.

## Content structure

| Path | What it is |
|---|---|
| `pages/` | The pages, plus `404.html` and `robots.txt` |
| `_data/exchange.yml` | Every API request and response the site shows, and the `docker run` command |
| `_data/header.yml`, `_data/footer.yml` | The nav and the footer links |
| `_includes/` | Header, footer, `exchange`, `command`, `trade-off`, and `drawings/`, one SVG per picture |

**There is no sitemap file.** `jekyll-multi-sitemap` generates `/sitemap.xml` from the `sitemaps:` block in
`_config.yml`, and `pages/robots.txt` is `{% robots %}` for the body and `{% sitemap_links %}` for its
`Sitemap:` line.

### The URL map

| Path | Page |
|---|---|
| `/` | homepage — the problem, what it answers, the trade-offs, one exchange, the command |
| `/cartonization/` | the trade's own word for choosing a box |
| `/parcel-lockers/` | whether an order fits a locker compartment |
| `/how-it-works/` | fit and pack, the three algorithms, what a yes and a no mean |
| `/self-hosting/` | running the image: the setups, `Presets.json`, changing a setting |
| `/verify/` | checking an image is the one the release workflow built |
| `/404.html` | permalink kept at the root — Cloudflare's `not_found_handling` wants it there |
| `/robots.txt`, `/sitemap.xml` | per host |

## The metadata comes from a gem

**There is no seo include on any of the three sites.** `{% page_meta %}` writes the head from
`jekyll-page-meta`, one implementation for all three.

`description` is front matter on every page here; the excerpt is only a fallback. Separator is ` - `, not
`|`, and it is `page_meta: title_separator:` in `_config.yml`. Every absolute URL is built from `url` in
`_config.yml`, so the host is named on one line.

**The day a 1200x630 image exists**, `og_image` and `twitter:card` change together — a wide card and a square
image is worse than either.

## Structured data

One JSON-LD graph per page, written by `{% structured_data %}`; its nodes are joined by `@id`.

`/` is the only page that names a type — `structured_data: type: SoftwareApplication` in its front matter,
with `name: Binacle.Net` because the node names the software, not the browser title. Its `offers` at price
`0`, `license` and `codeRepository` come from `structured_data: defaults: SoftwareApplication:` in
`_config.yml`. Every other page gets the organisation alone, which means no block at all.

**The `Organization` block is identical in all three sites' `_config.yml`, `@id` and all.** That pinned
`@id` — `https://www.binacle.net/#organization` — is what makes www, docs and demo one publisher rather than
three, and it is never derived from `url`. `sameAs` points at the GitHub org and the Docker Hub page.

`BreadcrumbList`, `FAQPage`, `WebSite` + `SearchAction` and `HowTo` were all considered and rejected - the
reasons are `$sites/www-design#W8`.

## Every API example is a real response

`_data/exchange.yml` holds every request and response on the site. How to re-run them is in that file. Nothing
is hand-written. **A response that looks plausible but is not what the API returns is the one lie this audience
catches**, and they are the least likely to forgive it.

It is one data file and one include (`_includes/exchange.html`) rather than markup in each page, because the
same card appears on several pages: inline copies get re-run in one page and rot in the others. The other code
blocks - the `cosign` and `docker buildx` lines on `/verify/`, the settings on `/self-hosting/` - are commands
and config, not API answers, and sit in their pages.

**Every example is v3.** v4 is marked experimental (`ApiV4Document.IsExperimental`), and a marketing page must
not hand a stranger an unstable contract. The v3 routes are `fit/by-custom`, `fit/by-preset/{preset}`, `pack/by-custom`,
`pack/by-preset/{preset}` and `GET presets`. Anything named `fit/bin`, `fit/smallest-bin` or
`pack/smallest-bin` is v4. `just openapi generate` writes the current documents if you need to check.

**A `fit` response has no coordinates** — only `pack` does. Any sentence near a `fit` example that promises
placement is wrong.

**`parameters` is required on every fit and pack body.** A request without it returns 400, so it cannot be
trimmed out of an example to make one shorter.

One exchange per job: the homepage fit against two lockers, a `by-preset` call for `/cartonization/`, a
three-compartment call for `/parcel-lockers/`, and a `pack` call with coordinates for `/how-it-works/`.

## The JavaScript

`_js/main.ts`, built as `js/main.js`, starts the design system's theme button, menu drawer, code wrap and copy
buttons. The pre-paint theme read is its own bundle, `js/theme-init.js`, loaded
blocking in `<head>` - `$sites/www-design#W2`.

**Every page must be complete with JavaScript off.** The copy buttons are `hidden` in the markup and shown by
the script, and the drawer's burger is made by the script. A control that cannot work is worse than no control.

## What will bite

**The Worker name is set once.** Renaming `binacle-net-www` after the first deploy creates a second Worker,
leaves the first running, and detaches the custom domain.

**The three-step build order is not optional.** Jekyll does not compile the sass (`$sites`), so a clean
`jekyll build` proves nothing about whether the site has styles. `just build www` does all three steps.

**The exchange card is one card**: the request, the response, and an optional plain-English verdict. With
`fold=true` the response sits closed behind *Response*. The verdict is what makes it marketing rather than
documentation.

**`sitemap.xml` lists this site's pages and nothing else.** The old `/apps/*` paths are served from here
as redirects to the demo host; a sitemap listing them is a sitemap full of 301s, which Search Console
reports as an error.

**The `docker run` line names `binacle/binacle-net:3`, the major tag.** It follows every minor and patch in
the line, so a minor release does not touch it. That command is the primary conversion on the site; a tag
that does not exist fails it with `manifest unknown`.

**The `docker run` line is one line in `_data/exchange.yml`, with no backslash continuations.** The command
block keeps newlines, so a continuation would show as a broken line on a phone. The block breaks it between
words instead.

**`demo.binacle.net` answers**, and it is linked from the nav, the footer, the home hero and `/how-it-works/`.
The link check runs offline and will not catch a host that stops answering.

**Below 800px the nav folds into a drawer** behind a burger the script adds. With JavaScript off the nav stays
in the header as its own row, which scrolls sideways.

**The old `/apps/*` paths redirect from `sites/www/_redirects`.** Each is a 301 to the demo host:
`/apps/packing-demo` to `/packing/`, `/apps/protocol-decoder` to `/vipaq/`, `/apps` to the demo root, each
with and without the trailing slash. `_redirects` is in the `include:` list in `_config.yml`, so it ships
with the build.
