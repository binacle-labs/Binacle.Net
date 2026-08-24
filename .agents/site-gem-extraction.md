---
description: The one file behind the ruby/ gem extraction - what the three sites duplicate, the design every gem follows, and what each gem is. Names the plans it takes a slice of.
---

# Extracting the site Liquid into gems

**Read this before touching anything in `ruby/` or any site's Liquid.** It is the whole design in one
file, so a session can start cold.

**This file names plan files. A plan may not do that.** It sits at the root and is allowed to, the same
way the release set is - the maintainer granted it explicitly on 24 Aug 2026. Nothing points back at it.

## What was surveyed

Every include, layout, sitemap template, `robots.txt` and `_config.yml` in `sites/www`, `sites/demo` and
`sites/docs`, read on 24 Aug 2026.

**The scope is what a gem can carry unchanged into an unrelated Jekyll site.** A gem qualifies only if
someone can drop it in, write a config block, and have it work. **No product name appears in gem code, in a
spec or in a README.** Specs build a throwaway site at `https://example.com`.

## The design every gem follows

**Compute once into page data. Render from page data.**

A value that two gems both need is written onto the page by a generator, and every other gem reads it as a
plain key. **No gem requires another, and no gem guards on another's constant being defined.**

| Written by | Key | Read by |
|---|---|---|
| `jekyll-breadcrumb-trail` | `page.breadcrumb_trail` | its own tag, and `jekyll-structured-data` |
| `jekyll-page-meta` | `page.meta.title` `page.meta.description` `page.meta.canonical` `page.meta.image` | its own tag, and `jekyll-structured-data` |

`page.meta` is free - nothing on any of the three sites uses it.

**Why:** a search engine compares the visible page with the markup. Two pieces of code computing one value
are free to disagree, and the failure is invisible - the page stays right while the markup quietly stops
matching it. One computation is the only fix that holds.

**What this costs, and it is silent when wrong.** Generator order is now load-bearing. Anything that stamps
a key a resolver then reads - a version suffix, for instance - must run **before** that resolver. Stamping
generators run at a high priority, resolvers at a low one, and each gem's README says which it is. Get it
backwards and nothing errors; the value is simply missing from the resolved key.

**The contract can rot.** Rename a key in one gem and the other stops finding it, with no error. It is
written down in both READMEs and asserted in both spec suites.

## Naming

**A gem's defaults are the names the wider ecosystem already uses** - `description`, `og_image`, `robots`,
`canonical`, `twitter_card`. **A site whose own vocabulary differs says so in config.** That is the line
between a gem someone else can drop in and a gem shaped around this repository.

**Name the job, not the mechanism.** `jekyll-` prefix, because that is what a `plugins:` list expects.
Avoid a name already on rubygems - `jekyll-sitemap`, `jekyll-seo-tag`, `jekyll-feed` and
`jekyll-breadcrumbs` are all taken. **The names below are not checked against rubygems**; as path gems a
clash costs nothing, and the check happens before anything is published.

## Pass 1 - the gems that copy and paste

### `jekyll-multi-sitemap` [www, demo, docs] - **built and wired, 24 Aug 2026**

Nothing left. See `docs/ruby/README.md`.

### `jekyll-resource-tags` [www, demo, docs] - **built and wired, 24 Aug 2026**

Nothing left. See `docs/ruby/README.md`. The icons moved into an `icons:` list per site and `X-UA-Compatible`
came out with them, so no head tag is needed and none will be built.

### `jekyll-page-meta` [www, demo, docs] - **built and wired, 24 Aug 2026**

Nothing left. See `docs/ruby/README.md`. All five head implementations are gone; `docs/_layouts/redirect.html`
is the one head that still writes its own metadata, and a plan holds it.

### `jekyll-structured-data` [www, demo, docs] - **built and wired, 24 Aug 2026**

Nothing left. See `docs/ruby/README.md`. Two decisions moved while it was built: the page type is declared as
`structured_data: type:` rather than `schema:`, because a key is named after what it produces and `seo:`
produces nothing; and docs sets `default_type: WebPage`, without which its 118 pages get no node at all.

### `jekyll-breadcrumb-trail` [docs]

**Replaces** `docs/_includes/breadcrumbs.html` and `docs/_includes/versions/breadcrumbs.html` - the same
thirty lines of nested path-splitting Liquid, twice - and the branch in `docs/_includes/header.html` that
chooses between them.

A generator writes `page.breadcrumb_trail`; the tag renders the nav from it.

```yaml
breadcrumbs:
  hide: ["version", "*.*"]     # glob patterns. Default: hide nothing
  home_label: Home
  nav_class: tiny-space
  separator: "<span>/</span>"
  home_html: '<i class="small">home</i>'
  link_last: false
```

**The gem ships no hide list.** A default naming `version` is one site's vocabulary. **Leaving the block
out is silent** - every docs breadcrumb simply gains two crumbs.

**The markup strings are config with today's values as defaults.** The framework classes belong to the
site, not to a gem.

**The home crumb points at the deepest hidden prefix.** On a versioned docs page with
`["version", "*.*"]` hidden that is `/version/v3.0.x/`, which is what the versioned include resolves to
today. **Hidden segments stay in every URL**, so the trail a crawler follows is real.

**Three visible changes, all deliberate:** the four version index pages gain a crumb; `docs/pages/404.html`
finally gets its `breadcrumbs: false` honoured, which only the versioned copy ever did; and seven swagger
pages stop appending a slash to a `.html` file - invisible today, a 404 the moment it goes into markup.

**`capitalize_all` must produce the same strings as the filter of that name**, or every label on the site
shifts at once.

### One filter, no new gem [www, demo, docs]

Three footers do the same two lines: read `site.data.footer.copyright`, replace `{now}` with the build
year. It is a filter, and `ruby/jekyll-filters` already exists. `{{ ... | expand_year }}`. No `Gemfile`
line, no `plugins:` line.

## Not in pass 1

- **A head tag should not exist.** Its icons belong in the resource tags gem, and what remains is two static
  lines. `X-UA-Compatible` does not come along at all - it targets Internet Explorer, demo and docs emit it,
  www had already dropped it.
- **A robots gem is not worth pass 1.** The three files are byte identical, but what is duplicated is a
  thirty-line rights reservation under Article 4 of EU directive 2019/790 - **content, not gem code**. The
  generic half is a `Sitemap:` loop of three lines. Shipping it in pass 1 gets a gem and leaves the legal
  text in three files, which is the whole thing the work is for.
- **`{% vlink %}` and the version rules** are documentation-site vocabulary. They became
  `ruby/binacle-docs-versions` on 24 Aug 2026, named after the site rather than after Jekyll so nothing reads
  it as portable. It lives in `ruby/` rather than `sites/docs/_plugins/` because that is the only place a
  plugin gets a spec suite. **Wiring page meta into docs without it is a silent regression**: 74 pages lose
  their `noindex` and every versioned title loses its version.

## Defects the survey found

Each is in the sites today, and each is fixed by the gem that takes over that code.

- The link and script templates write `type="{{ item.type }}"` unconditionally. **Every item in all three
  `includes.yml` files sets `type`, so nothing renders empty today** - but the hardcoded icon links do not
  have one, so moving them into a data list with the current template would emit `type=""`.
- A page with `sitemap: {exclude: false}` and nothing else emits empty `<lastmod>`, `<changefreq>` and
  `<priority>` elements. Not schema-valid.
- `twitter_url` is set on no site, so `twitter:site` has never been emitted anywhere. **Emitting it for the
  first time is a change, not a fix.** Both keys came out with the includes; the gem writes `twitter:site`
  only when a `twitter_site` is configured, and none is.
- The per-page preview image override is dead and has two names: demo and docs read `main_img`, www reads
  `og_image`. **No page sets either.**
- `docs/_layouts/versions/swagger.html` branches on `title` where the include branches on `seo_title`, so a
  swagger page setting `seo_title` is silently ignored.
- docs derives a description from page content, demo does not. Nothing chose that.
- www writes the preview image unguarded; demo and docs guard it. Only one can survive.
- A canonical with no page url renders as `href=""`, which is worse than none - it self-references the wrong
  thing.

## The plans this takes a slice of

All are in `plans/sites/`. **Where one contradicts this file, this file is the design.**

**The resource tags gem landed on 24 Aug 2026 and both its plans are deleted.** `head-tag.md` was folded
into them; the one question it carried that no gem answers - the two android icons nothing references - is
`plans/sites/android-icons.md`.

**The page meta and structured data work is three files** - `plans/sites/page-meta-gem.md` and
`plans/sites/structured-data-gem.md` build the two gems, and `plans/sites/page-meta-wiring.md` moves the
sites onto both **in one change**, because deleting a site's `seo.html` deletes the JSON-LD living inside
it. `seo-tag-gem.md` was folded into all three and deleted.

**The breadcrumb work is `plans/sites/breadcrumb-trail-gem.md` and `plans/sites/breadcrumb-trail-wiring.md`.**
`breadcrumbs-tag.md` was folded into them and deleted.

**Every wiring plan is site-session work.** The standing rule keeps `sites/` out of an ordinary coding
session, and its answer is a written plan carried out by the session that owns that content.

**The sitemap gem landed on 24 Aug 2026 and both its plans are deleted.** `ruby/jekyll-multi-sitemap` is
built, all three sites generate their sitemaps from a `sitemaps:` config block, `sites/docs` writes an index
at `/sitemap.xml`, and every `robots.txt` gets its `Sitemap:` lines from `{% sitemap_links %}`. What it is
and how it is configured is in `docs/ruby/README.md` and the gem's own README.

| Plan | What is still worth reading | What it gets wrong |
|---|---|---|
| `robots-tag.md` | what the body is and why it must not be reworded | nothing. It is simply not pass 1 |
| `docs-version-rules.md` | the rules, and the swagger and redirect `robots` values that must move into page data | its generator runs at a low priority. Under this design a stamping generator runs high, or the suffix never reaches the resolved title |
| `ruby-plugin-tidy.md` | all of it | nothing. Untouched by this |

## Done when

- [ ] Every pass 1 gem exists in `ruby/` with a gemspec, a `README.md`, a `Gemfile` and a passing spec suite.
      `ls ruby/*/[a-z]*.gemspec` lists one per gem, and `rspec` passes in each
- [ ] No gem names the product, in code, spec or README.
      `grep -rni "binacle" ruby/jekyll-multi-sitemap ruby/jekyll-resource-tags ruby/jekyll-page-meta ruby/jekyll-structured-data ruby/jekyll-breadcrumb-trail` finds nothing
- [ ] No site holds a copy the gems replaced.
      `ls sites/*/_includes/seo.html sites/*/_includes/links.html sites/*/_includes/scripts.html` finds nothing;
      `grep -rn "page_last_mod\|entry_last_mod\|split: '/'" sites/` finds nothing
- [ ] A value read by two gems is computed by one.
      **By eye.** Find the one place each key in the contract table is written. A second writer is a bug
- [ ] The `Sitemap:` lines are still in all three built `robots.txt`.
      `grep -c "^Sitemap:" artifacts/*/robots.txt`
- [ ] Each site's built output is unchanged except where this file says it changes.
      Build from the previous commit and `diff`. Expected deltas: empty sitemap elements gone, docs'
      `robots.txt` listing one url instead of two, docs gaining `/sitemap.xml`, the icon links demo and docs
      gain, the breadcrumb changes named above, and the structured data that did not exist
- [ ] The head of every built page is checked on all three sites.
      **By eye**, one page per layout. This is the top of every page on all three sites - wrong once is wrong
      everywhere
