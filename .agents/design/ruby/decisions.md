---
id: ruby/decisions
description: Ruby gem decisions ledger — why one computation feeds every gem, the one key whose generator order is load-bearing, how a gem's config names are chosen, what the gem name says about portability, why a portable gem may not name the product, and what decides a tag against a generator.
verified: 2026-08-24
check: R1 against the one place each key in the table is written, where a second writer is a bug; R2 against `priority :high` in binacle-docs-versions/generator.rb and `priority :low` in jekyll-page-meta/generator.rb; R5 by grepping every `ruby/jekyll-*` for the product and for its subject matter
paths:
  - "ruby/**"
---

# Ruby gems — decisions ledger

Why the gems under `ruby/` are shaped this way. `$ruby` says what each one *is*; this says why, so a later
pass does not undo a deliberate choice.

**This is what survives `site-gem-extraction.md`**, the plan that ran the extraction on 24 Aug 2026 and was
deleted when it finished. Its per-gem notes, its defect list and its plan roster all went; these six
decisions are the part that outlives the job.

## R1 — one computation, read as a plain key

**Compute once into page data. Render from page data.** A value two gems both need is written onto the page
by one generator, and every other gem reads it as an ordinary key. **No gem requires another, and no gem
guards on another's constant being defined.**

| Written by | Key | Read by |
|---|---|---|
| `jekyll-page-meta` | `page.meta.title` `.description` `.canonical` `.image` | its own tag, and structured data |
| `jekyll-breadcrumb-trail` | `page.breadcrumb_trail` | its own tag, and `jekyll-structured-data` |
| `binacle-docs-versions` | `page.title_suffix` | `jekyll-page-meta`'s resolver, into the title |
| `binacle-docs-versions`, or the page | `page.robots` | `jekyll-page-meta`'s tag, and `jekyll-structured-data` |
| `jekyll-multi-sitemap` | `site.sitemaps.urls` | its own `{% sitemap_links %}` |
| `jekyll-webmanifest` | `site.webmanifest.url` | its own link tag |

**Why.** A search engine compares the visible page against the markup. Two pieces of code computing one
value are free to disagree, and the failure is invisible — the page stays right while the markup quietly
stops matching it. One computation is the only fix that holds.

**The contract rots silently.** Rename a key in one gem and the other stops finding it with no error. That
is why every key above is named in both READMEs and asserted in both spec suites.

## R2 — generator order is load-bearing for exactly one key

`title_suffix` is stamped at `priority :high` and resolved into the title at `:low`. Get it backwards and
nothing errors: 118 documentation titles quietly lose their version.

**`robots`, `breadcrumb_trail` and everything else are read at render time and survive any order.** Only a
key read inside another *generator* is at risk. An audit found the rule written in three places as if it
bound every key; it binds one.

**Two generators of equal priority have no defined order at all.** Jekyll sorts on priority and stops. That
is not a thing to work around — it is a thing to design away from, which is R6.

## R3 — a gem's defaults are the words the ecosystem already uses

`description`, `og_image`, `robots`, `canonical`, `twitter_card`, `exclude`, `crumbtitle`. **A site whose own
vocabulary differs says so in config.** That line is the difference between a gem someone else can drop in
and a gem shaped around this repository.

Where nothing exists to copy, say so rather than inventing quietly: excluding a path *segment* from a
breadcrumb had no precedent anywhere, so `exclude` was chosen by analogy with Jekyll's own `exclude:`.

**Cost, paid once:** `jekyll-webmanifest` defaults `name` to `site.title` and not to `display_title`, because
`display_title` is this project's word. `sites/demo` therefore repeats its name in a config block. That is
the right trade — a shared gem carrying one site's vocabulary is the thing this whole split exists to avoid.

## R4 — the prefix says whether the gem is portable

**`jekyll-` means another site could load it.** `binacle-` means it carries something only this organisation
wants — `binacle-docs-versions` holds one site's version scheme, `binacle-robots` holds a rights reservation.
A `jekyll-` prefix on either would advertise something nobody else has a use for.

Avoid a name already on rubygems: `jekyll-sitemap`, `jekyll-seo-tag`, `jekyll-feed` and `jekyll-breadcrumbs`
are all taken, which is why the gems here are named after the job rather than the obvious noun. **The names
are not checked against rubygems.** As path gems a clash costs nothing, and the check happens before
anything is published.

## R5 — a portable gem may not refer to the product, and the name is the smaller half

Nothing in a `jekyll-*` gem names this product, in code, in a spec or in a README. Specs build a throwaway
site at `https://example.com`.

**Grepping the name is not the check.** A page meta fixture described bin packing and a structured data
fixture was called "Packing Demo" while `grep -i binacle` came back clean through all of it. Grep the
subject matter too, and read the fixture copy by eye. A gem carrying the product in its own name is exempt —
that is what the name is for.

## R6 — a tag when it needs another gem's output, a generator when it does not

Both write into a site. What decides between them is R2's second paragraph.

**`binacle-robots` is a tag.** A generator would own the whole `robots.txt` and the sites would keep nothing
— but it needs the `Sitemap:` urls, which exist only after `jekyll-multi-sitemap`'s generator has run at
`:lowest`. Two generators at `:lowest` would appear in either order, so the lines would come and go at
random, and a `robots.txt` without them still looks valid. Tags render after every generator, so the two
sit in one file with no ordering rule and neither gem requiring the other.

**`jekyll-webmanifest` is a generator.** Every value is config, `site.title` or `site.description`, so it
waits on nothing and owns the file outright — which is what lets a site delete its manifest page rather than
shorten it. It is the better end state, and it is only available when nothing else is in the way.

## R7 — three config names moved while the gems were built

Recorded because each was sketched one way and shipped another, and the sketch reads more obvious than the
answer.

**The breadcrumb config took the ecosystem's words**, not the ones first drafted: `hide` became `exclude`,
`label_from` became `title_from` defaulting to `[crumbtitle, title]`, `home_label`/`home_html` nested under
`home:` to match `page_meta`'s `description.from`, `nav_class` became `class`. **`separator` was dropped
outright** — the standard breadcrumb markup draws the separator in CSS, and both the ARIA pattern and
Bootstrap agree, so a separator setting would only enable markup nobody should emit.

**A page declares its structured data type as `structured_data: type:`, not `schema:` or `seo:`**, because a
key is named after what it produces and `seo:` produces nothing. `sites/docs` sets `default_type: WebPage`;
without it, 118 pages get no node at all.
