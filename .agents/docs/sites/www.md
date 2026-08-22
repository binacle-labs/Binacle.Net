---
id: sites/www
description: The published Jekyll marketing site at sites/www/ — four pages, no CSS framework, and the only site whose sass Jekyll does not compile.
verified: 2026-08-23
check: The page list and permalinks match sites/www/pages/; _config.yml still has no `sass:` block and package.json still carries build:css; the seo include still emits front-matter descriptions, og:type website and a default og:image; sitemap.xml still lists exactly the four pages; _data/exchange.yml still names v3 routes that exist in artifacts/openapi/Binacle.Net_v3.json; --action and --accent are still separate from --primary and --tertiary in _sass/_tokens.scss
paths:
  - "sites/www/**"
---

# WWW Site

Jekyll site at `sites/www/`, published at `www.binacle.net`. It is the marketing site — what Binacle.Net does
and who it is for. It is off limits from a coding session; see `.agents/README.md`.

Output goes to `../../artifacts/www`. Worker `binacle-net-www`.

```bash
just serve www    # jekyll serve (port 7197) + sass and webpack watches, one Ctrl-C stops all three
just build www    # the same site built once, into artifacts/www
```

## What is different from the other two sites

Three things, and each one is a decision rather than an oversight.

**Jekyll compiles no sass here.** `_config.yml` carries no `sass:` block. `npm run build:css` runs the sass
CLI over `_sass/main.scss` and writes `css/main.css` — the same pattern as `api/src/Binacle.Net.UIModule`,
which is the only other surface in the repo that compiles its own sass. Two consequences: putting the block
back gives two compilers a claim on the same files, and **`css/` is gitignored** here, because it is build
output. On `docs` and `demo` it is committed source.

**No CSS framework and no npm dependencies.** `docs` and `demo` run BeerCSS; this one is plain CSS. The gulp
asset copy skips `assets/lib/` entirely for this target, so BeerCSS and material-dynamic-colors never arrive.
There is no component library to reach for.

**`npm run watch` is itself a `concurrently` pair** — sass and webpack — so `just serve www` runs three
processes where the other two run two.

## Content structure

| Path | What it is |
|---|---|
| `pages/` | The four pages, plus `404.html` and `robots.txt` |
| `collections/_sitemaps/` | `sitemap.xml`, served at `/sitemap.xml` |
| `_data/exchange.yml` | Every request and response the site shows, as data |
| `_includes/` | `seo.html`, `schema.html`, header, footer, and the two conversion components |

### The URL map

| Path | Page |
|---|---|
| `/` | homepage — the problem, one exchange, the scope table, the command |
| `/cartonization/` | the trade's own word for choosing a box |
| `/parcel-lockers/` | whether an order fits a locker compartment |
| `/how-it-works/` | fit and pack, the three algorithms, what a yes and a no mean |
| `/404.html` | permalink kept at the root — Cloudflare's `not_found_handling` wants it there |
| `/robots.txt`, `/sitemap.xml` | per host |

`/plugins/` is reserved and deliberately unwritten. Which platform gets the first plugin is unanswered, and
that question decides the page's contents.

## The metadata include is new, not copied

`_includes/seo.html` was written for this site. **Do not replace it with the one from `docs` or `demo`** —
theirs carry five live defects and every one of them comes with the file:

| Defect | What this one does |
|---|---|
| description derived from the excerpt and truncated mid-word at 160 chars | `description` is front matter on every page; the excerpt is only a fallback |
| `og:type` hardcoded `article` | `website` |
| no `og:image` on any page, so every shared link renders a grey card | a site-wide default on every page |
| `twitter:card` never revisited | `summary`, because `og_image` is a square logo and a wide card crops one |
| `<title>` value wrapped in newlines and indentation | emitted with no whitespace inside the tag |

Separator is ` - `, not `|`. Every absolute URL comes from `absolute_url`, so `url` in `_config.yml` is the
only line in the build that names the host.

**The day a 1200x630 image exists**, `og_image` and `twitter:card` change together — a wide card and a square
image is worse than either.

## Structured data

Two JSON-LD blocks in `_includes/schema.html`, emitted only where a page sets `structured_data: true`, which
is `/` alone. `SoftwareApplication` carries `offers` at price `0` — the whole category is paid, so a free row
is what the directories lack. `Organization` carries `sameAs` pointing at the GitHub org and the Docker Hub
page, tying the site to the listing that currently outranks it.

`BreadcrumbList`, `FAQPage`, `WebSite` + `SearchAction` and `HowTo` were all considered and rejected. The
reasons are in `_includes/schema.html` so nobody adds them back.

## Every code block is a real response

`_data/exchange.yml` holds every request and response on the site, and the file says how to re-run them.
Nothing is hand-written. **A response that looks plausible but is not what the API returns is the one lie
this audience catches**, and they are the least likely to forgive it.

It is one data file and one include (`_includes/exchange.html`) rather than markup in each page, because the
card appears on all four pages: inline copies get re-run in one page and rot in the other three.

**Every example is v3.** v4 is experimental for the whole 3.0.x line and a marketing page must not hand a
stranger an unstable contract. The v3 routes are `fit/by-custom`, `fit/by-preset/{preset}`, `pack/by-custom`,
`pack/by-preset/{preset}` and `GET presets`. Anything named `fit/bin`, `fit/smallest-bin` or
`pack/smallest-bin` is v4. `just openapi generate` writes the current documents if you need to check.

**A `fit` response has no coordinates** — only `pack` does. Any sentence near a `fit` example that promises
placement is wrong.

**`parameters` is required on every fit and pack body.** A request without it returns 400, so it cannot be
trimmed out of an example to make one shorter.

Four exchanges are in the file, one per job: the homepage fit against two lockers, a `by-preset` call for
`/cartonization/`, a three-compartment call for `/parcel-lockers/`, and a `pack` call with coordinates for
`/how-it-works/`.

## The stylesheet, and the two tokens that are not the palette

Plain CSS, no framework, about 2.5 KB gzipped. `_sass/` is four partials plus `main.scss`: `_tokens`,
`_base`, `_layout`, `_code`, `_content`.

**Colour is rationed and the rationing is the design.** Blue is the answer - links, the primary button, one
3px structural rule per page. Orange is what you type, and only that: the `POST` chip and the `$` prompt, four
appearances on the whole site. Violet is accents only - the small-caps eyebrow above each `h2`, and the rule
on a caveat callout. **There is no coloured header bar and no coloured footer.** Do not repaint this; large
flat fills are why the other two sites read as framework demos.

**`--action` and `--accent` are deliberately not `--primary` and `--tertiary`.** The palette's dark variants
are darkened so they recede, which is right for a rule or a border and wrong for anything a reader has to read
or press: `#3c5d8b` as a button fill makes the call to action a dead grey-blue, and `#5b2d70` as label text on
`#101010` is unreadable. `--action` is `#448aff` at full strength in both themes; `--accent` is the violet
that carries a word. **The blue button takes a near-black label** because white on `#448aff` measures about
3.4:1 and fails; near-black gives 5.79:1.

**The neutrals are derived** with `color-mix` from `--bg` and `--fg`, which is what keeps the palette at four
hues while giving the site a full text ramp. No brand hue is ever the colour of running text.

**Prose runs 62ch inside a 1080px shell.** Documentation runs 90-100ch, and the difference is the cheapest
signal separating the two registers. **Never put `shell` and `prose` on the same element** - the narrower cap
wins and then `margin-inline: auto` centres it, so the column floats to the middle of the viewport while every
other band stays left. Nest them.

**The hero is one column.** It was specified as two at roughly 55/45 and that does not survive the payload: a
real `fit` response is 757px of monospace and a 45% column is about 517px, so the card chopped off mid-token.
Any grid holding a code pane also needs `min-width: 0` on its children - a grid item defaults to
`min-width: auto` and will grow its track to the intrinsic width of a `<pre>`.

## The JavaScript, and there is very little

`_js/main.ts`, about 770 bytes gzipped, two behaviours, both degrading cleanly:

- **A theme toggle.** The pre-paint read is *not* in this file — it has to run before first paint, so it is
  inlined in `_includes/theme-init.html`. Moving it to an external file reintroduces the flash.
- **Clipboard copy controls.** The `docker run` line is the primary conversion on three of the four pages.

Both controls are `hidden` in the markup and revealed by the script. **All four pages must be complete with
JavaScript off**, and a control that cannot work is worse than no control.

The consent banner has a reserved slot in `_data/includes.yml`. The maintainer will fill it in a dedicated
session.

## What will bite

**The Worker name is set once.** Renaming `binacle-net-www` after the first deploy creates a second Worker,
leaves the first running, and detaches the custom domain.

**The three-step build order is not optional**, and it matters more here than on the other two: Jekyll no
longer compiles the sass, so a clean `jekyll build` proves nothing about whether the site has styles.
`just build www` does all three steps.

**The exchange card is the most valuable element on the site, and it is one card.** One border, one radius,
five rows: the ask, the request, a `RESPONSE` seam, the response, and a plain-English verdict. The seam is
what makes it one exchange instead of two code blocks; the verdict row is the whole reason it is marketing
rather than documentation. Code is two-tone only - keys muted, values full text, punctuation lighter. **No
syntax rainbow.**

**`sitemap.xml` lists this site's four pages and nothing else.** The old `/apps/*` paths will be served from
here as redirects to the demo host; a sitemap listing them is a sitemap full of 301s, which Search Console
reports as an error.

**The `docker run` line names a tag that is not published yet.** As of 23 Aug 2026 Docker Hub's newest tags
are `3.0.0-beta.4` and `3.0.0-beta.3`, and `latest` still resolves to `2.1.1` from January. `binacle/binacle-net:3.0`
appears when 3.0.0 ships. **The site must not go live before it does** — that command is the primary
conversion on three pages and it fails with `manifest unknown` today.

**Three links point at hosts that do not answer yet.** `demo.binacle.net` has no DNS, and it is linked from
the nav, the footer and `/how-it-works/`. The link check runs offline and will not catch it.

**The code panes scroll on a phone and that is deliberate.** Wrapping them was tried and reverted: a wrapped
line starts at a different indent from the line it continues, so the JSON's own structure stops being
readable and the hero card grew to roughly 1900px tall. What helps instead is room and smaller type - the
card goes gutter to gutter below 720px and the code drops to 0.8125rem, which took the visible share of a
response line from 42% to 59% at 390px. **Do not "fix" this by wrapping it again.**

**The `docker run` line is one line in `_data/exchange.yml`, with no backslash continuations.** They wrap into
nonsense on a phone - `pre-wrap` keeps both the newlines and the continuation indents and then wraps on top of
them, which left a stray `-e` alone on a line in the middle of the primary conversion.

**The mobile header is an explicit grid, not the flex row.** Left to source order the nav spans both columns,
takes row 2, and pushes the theme control onto a row of its own - a four-row, 200px header before the reader
reaches anything. Placed explicitly it is 144px.

**The redirects are not written yet.** Until they are, `/apps/`, `/apps/packing-demo/` and
`/apps/protocol-decoder/` return 404 on a host that has answered them for three years. Do not tear down the
current marketing site before they exist.
