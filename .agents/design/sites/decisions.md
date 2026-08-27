---
id: sites/decisions
description: Decisions behind the demo and documentation sites — the link-preview pair, title order, what the demo host calls itself, why the demo has no collections, and the two footer calls. What a review would otherwise re-litigate.
verified: 2026-08-27
check: S1 against the og_image in both sites' _config.yml and the twitter_card default in jekyll-page-meta, which must still agree; S2 against the page_meta title_separator in both _config.yml files; S3 against display_title in sites/demo/_config.yml and its use in _includes/header.html; S4 against sites/demo/_config.yml, which must declare no collections: key at all, and against sites/docs/_config.yml, whose collections are versions and common_pages
paths:
  - "sites/demo/**"
  - "sites/docs/**"
---

# Sites — decisions ledger

Why the two published sites are the way they are. `$sites/demo` and `$sites/docs` say what they *are*; this
says why, so a later pass does not undo a deliberate choice.

**`sites/www` is not covered here.** It is being built in its own session and owns its own record.

## Locked

### S1 — the link preview is a square logo and a small card, and the two change together

Every page on both sites emits `og:image` pointing at `media/logo/binacle-logo-512x512.png`, with
`twitter:card` set to `summary`.

**`summary` is not an oversight.** `summary_large_image` renders a wide card, and a wide card crops a square
image. The pair is only correct together: the day a 1200x630 image exists, `og_image` in each site's
`_config.yml` moves to it **and** `twitter:card` becomes `summary_large_image`. Changing either alone is worse
than changing neither.

**Why ship a logo rather than wait.** A missing `og:image` renders a bare grey card in Slack, Discord and
anywhere else a link is pasted. A small square card is worse than a designed one and better than none, and the
wiring is what takes the time — swapping the file later is one line per site.

### S2 — page first, brand last, separated by ` - `

`Packing Demo - Binacle.Net`, not `Binacle.Net | Packing Demo`.

**Tabs and search results truncate at the end.** Brand-first makes every tab and every search row open with the
same characters, so a reader with several open can tell none of them apart. The distinguishing words have to
lead. A page may override the whole string with `seo_title` when the composed one would stutter.

### S3 — the demo host calls itself Binacle.Net Demo, and `site.title` stays the brand

`sites/demo/_config.yml` carries both: `title` is `Binacle.Net`, `display_title` is `Binacle.Net Demo`. The
header bar and the index `h1` use `display_title`; the `<title>` suffix and `og:site_name` use `title`.

**Two different jobs.** A visitor needs to know which host they are on, because the demo and the marketing site
otherwise wear the same name and the nav's exit link off the demo is meaningless. A `<title>` suffix needs the
brand — `Packing Demo - Binacle.Net Demo` stutters, and the page half already says Demo where it matters.

### S4 — the demo site has no collections at all

The two tool pages are pages in `sites/demo/pages/` carrying `applet: true` and an `order`. The chooser, both
navs and the JSON-LD block all select on that flag.

**They were an `apps` collection while the URLs were `/apps/:name/`.** Once the host became the index and the
tools moved to `/packing/` and `/vipaq/`, a collection expressed nothing a front-matter flag does not.

**A `sitemaps` collection went the same way**, and later than the first. A sitemap page under `pages/`
inherited the `pages/**` defaults — a layout and a sitemap entry — and needed two overrides to undo them,
which was the argument for keeping it a collection. `jekyll-multi-sitemap` removed the page entirely: the
`sitemaps:` block in `_config.yml` names the files and what each includes, and the gem generates them, so
there is nothing on disk to inherit a default. `sites/demo/_config.yml` declares no `collections:` key.

**`sites/docs` is the only site with collections** — `versions` and `common_pages` — and neither is a
sitemap.

### S5 — legacy swagger pages keep `nofollow`; every other legacy page gets `follow`

De-indexed version pages are served `noindex, follow` so their links still lead somewhere worth crawling.
The swagger pages are the exception and stay `noindex, nofollow`, which is what they already were: a Swagger
shell has no links a crawler benefits from following.

**The inconsistency is deliberate and it is the smaller cost.** Making them uniform would mean either
following links that go nowhere, or dropping `follow` from seventy-four pages that have real ones.

### S6 — the demo footer carries a version badge and no stars badge

`sites/demo/_data/footer.yml` fetches one `img.shields.io` badge, for the published image version. The GitHub
stars badge that sat beside it is gone.

**A third-party badge is a request on every page**, and it rendered as the last thing a visitor read. The version badge is the opposite case: it is a fact that stays true without anyone maintaining it,
which is the whole argument for a badge.

**Rendering it at build time was considered and rejected.** Nothing in the site knows the published version —
there is no version field in `_config.yml` — so a build-time badge would be a hand-maintained string, which is
the one thing a badge is supposed not to be. The cost is one third-party request per page, and it is the only
one on either site.

### S7 — the demo's way out is Docs in the rail, the website in the top bar, and both in the footer

Someone who has just watched a box get packed has no path to the documentation or to the product page, and
leaves through the back button. The exits are in three places on purpose: the left rail carries Docs alone,
because it is the exit a developer mid-demo actually wants; the top bar carries the website beside the GitHub
and Docker marks, because that bar is already where you leave for another property; the footer carries all
four, because that is where someone who has finished looks.

**The rail carries one exit, not two, for a measured reason** — the second label overflowed an 80px rail.
