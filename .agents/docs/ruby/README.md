---
id: ruby
description: Ruby gems under ruby/ — the Jekyll plugins the sites under sites/ load, which sites load which, and the one that belongs to a single site.
verified: 2026-08-24
check: Gem list, filter names and tag names match ruby/ source; every gem still has one entry file at lib/<gem>.rb and everything else under lib/<gem>/, one module inside Jekyll, and a frozen_string_literal line on every .rb; jekyll-page-meta still resolves the four page.meta keys in a :low priority generator and all three sites load it; ruby/Gemfile still names every gem under ruby/; the gtm tags still take the id as an argument; every site under sites/ still loads jekyll-filters and jekyll-gtm through its Gemfile :jekyll_plugins group and lists them under plugins: in _config.yml; all three sites still generate their sitemaps from a sitemaps: config block and write their Sitemap: lines with {% sitemap_links %}; all three sites render their link, script and prefetch elements with jekyll-resource-tags and none has a links, scripts or prefetch include; each site's _data/includes.yml still holds an icons: list
paths:
  - "ruby/**"
---

# Gems

Jekyll plugins for the three sites — `sites/docs/`, `sites/demo/` and `sites/www/`.

| Gem | What it adds | Loaded by |
|---|---|---|
| `jekyll-filters` | Three Liquid filters: `clean_content`, `capitalize_all`, `expand_year` | all three sites |
| `jekyll-gtm` | Two Liquid tags: `{% gtm_head %}`, `{% gtm_body %}` | all three sites |
| `jekyll-multi-sitemap` | A generator that writes the sitemap files, and three tags | all three sites |
| `jekyll-page-meta` | A generator that resolves four page keys, and `{% page_meta %}` | all three sites |
| `jekyll-resource-tags` | Three Liquid tags: `{% link_tags %}`, `{% script_tags %}`, `{% prefetch_tags %}` | all three sites |
| `jekyll-structured-data` | One Liquid tag: `{% structured_data %}` | all three sites |
| `binacle-docs-versions` | A generator that stamps two version keys, and `{% vlink %}` | **docs only** |

**Every gem has the same shape**: `<name>.gemspec`, one entry file at `lib/<name>.rb` that Jekyll requires by
gem name, and everything else under `lib/<name>/` with the gem prefix dropped from the filenames.

**A site loads a gem twice over, and both halves are needed.** Each site's `Gemfile` names the gem inside its
`group :jekyll_plugins` block with `path: "../../ruby/<gem>"` — that is what resolves the local directory,
since none of them is published. Each site's `_config.yml` then lists the gem under `plugins:`. Dropping either
half stops the plugin loading.

## jekyll-filters

**`clean_content(input, length = 160)`** — strips HTML tags, collapses newlines and runs of spaces, trims,
truncates to `length`, then trims again so a cut landing on a space leaves none. Used to generate meta description strings from page content.

**`capitalize_all(input)`** — capitalises every space-separated word. It lowercases the rest of each word,
so `API` becomes `Api`.

**`expand_year(input, placeholder = "{now}")`** — replaces the placeholder with the year of `site.time`.
Added 24 Aug 2026. **No site calls it yet** - all three footers still do the replace with
`{% assign %}` and `replace:`.

Source: `ruby/jekyll-filters/lib/jekyll-filters/` — `sanitization.rb`, `capitalization.rb` and `dates.rb`.
**The module is `Jekyll::SiteFilters`, not `Jekyll::Filters`** — that name is Jekyll's own, and is where
`markdownify` lives.

## jekyll-gtm

**Both tags take the Google Tag Manager id as an argument.** They are not configuration readers: the tag resolves
whatever it is handed as a Liquid variable, and falls back to the literal argument only when it has the shape of
a container id — `GTM-` and then letters and digits. The sites call them as `{% gtm_head site.gtm %}` and
`{% gtm_body site.gtm %}`, so the `gtm:` key in `_config.yml` is the *caller's* convention, not something the gem
knows about. A bare `{% gtm_head %}` resolves to an empty id and injects nothing.

**A variable that resolves to nothing renders nothing.** Before 24 Aug 2026 the fallback was unconditional, so a
misspelt variable name wrote itself into the snippet as the container id and the tag silently never loaded.

- **`{% gtm_head <id> %}`** — the GTM `<script>` snippet, for `<head>`.
- **`{% gtm_body <id> %}`** — the `<noscript>` fallback, for the top of `<body>`.

**An empty or missing id renders an empty string** rather than a broken snippet. All three sites currently set
`gtm: ''`, so GTM is off everywhere.

Source: `ruby/jekyll-gtm/lib/jekyll-gtm/` — `tag.rb` resolves the id, the two subclasses are one heredoc
each, registered in `jekyll-gtm.rb` under `Jekyll::GTM`.

## jekyll-multi-sitemap

**All three sites use it, and none of them has a sitemap template any more.** Each declares a `sitemaps:`
block in `_config.yml`; `sites/docs` also sets `index:`, which writes `/sitemap.xml` over its two files.
Every `robots.txt` gets its `Sitemap:` lines from `{% sitemap_links %}`, which emits the index alone where
there is one.

A generator writes one file per entry in a `sitemaps:` config block, an index over them when `index:` is set,
and it fails the build on a bad config rather than writing a wrong sitemap. Three tags come with it —
`{% sitemap_urlset %}`, `{% sitemap_url %}` and `{% sitemap_links %}`, the last writing the `Sitemap:` lines
for a `robots.txt`. The generated urls are readable as `site.sitemaps.urls`.

**The gem names no product and its specs build a throwaway site at `https://example.com`**, so it can be
dropped into an unrelated Jekyll site. `ruby/jekyll-multi-sitemap/README.md` is written for that reader and
holds the config keys and the tags.

Source: `ruby/jekyll-multi-sitemap/lib/jekyll-multi-sitemap/`.

## jekyll-page-meta

**All three sites load it and no site has an seo include any more.**

**Two halves.** A generator resolves four values onto every page and every document —
`page.meta.title`, `page.meta.description`, `page.meta.canonical` and `page.meta.image`. The
`{% page_meta %}` tag writes the head elements from those keys and resolves nothing itself. **A value two
plugins need is computed once**, which is why the resolver is a generator rather than logic in the tag.

**It runs at `:low` priority**, so a plugin stamping `title_suffix` or `robots` must run higher. Stamp after
the resolver and the suffix is silently missing from the title.

Config is `page_meta:` and every key has a default: `title_separator`, `description.from`,
`description.truncate`, `twitter_card`, `twitter_site`. The description chain is
`[description, excerpt, site]` by default, cut at 160 characters — **characters, not words**, because that is
what a search result cuts. `twitter:site` is written only when a handle is configured, and none is today.

**One description pipeline for every source**: markdown, then tags out, then whitespace collapsed, then the
cut. The generator sees content before Liquid has run, so Liquid tags are dropped from the text rather than
rendered into it.

**The gem names no product and its specs build a throwaway site at `https://example.com`.**
`ruby/jekyll-page-meta/README.md` is written for that reader and holds the config and the front matter keys.

Source: `ruby/jekyll-page-meta/lib/jekyll-page-meta/` — `resolver.rb` holds the four values, `head.rb`
writes the elements, `text.rb` is the description pipeline.

## jekyll-resource-tags

**All three sites load it, and none of them has a links, scripts or prefetch include any more.** The favicon
links, hardcoded in five layouts before, come from an `icons:` list in each site's `_data/includes.yml`.

Three tags, each taking one list and writing one element per item — `{% link_tags %}` reads `href`,
`{% script_tags %}` reads `src`, `{% prefetch_tags %}` reads `src` and writes it as the `href` of a
`<link rel="prefetch" as="script">`. **Every other key on an item becomes an attribute, in the order the data
declares it**, which is what lets a site move onto the tags with no change to its built pages. `href` and `src`
go through `relative_url` unless they already carry a scheme.

**`prefetch_tags` drops `async`, `defer` and `nomodule`** — the only content attributes that exist on `<script>`
and not on `<link>`. Without that, a site prefetching the same list it executes puts `defer` on a `<link>`.
Everything else carries through, `crossorigin` included.

**There is no config block and it names no product**, so it drops into an unrelated Jekyll site.
`ruby/jekyll-resource-tags/README.md` is written for that reader.

Source: `ruby/jekyll-resource-tags/lib/jekyll-resource-tags/` — `resource_tag.rb` holds the rendering,
`element.rb` writes the attributes, and the three subclasses are three constants each.

## jekyll-structured-data

**All three sites load it.** `sites/www/_includes/schema.html` is gone, demo's block moved out of its seo
include, and docs gained one on all 118 pages through `default_type: WebPage`.

**One tag, no generator, and it computes nothing.** `{% structured_data %}` reads `page.meta.title`,
`page.meta.description`, `page.meta.canonical`, `page.meta.image`, `page.breadcrumb_trail` and `page.robots`,
and writes one `@graph` holding the organisation, the page node and the breadcrumb list, joined by `@id`.
**It requires no other gem and guards on no other gem's constant** — a key it cannot find is a field it
leaves out.

**The organisation `@id` is pinned in config**, never derived from `site.url`. That is what makes three hosts
one publisher rather than three.

`structured_data: type:` in front matter picks the node type and every other key under it passes through untouched.
`structured_data: defaults:` holds per-type keys and the page wins over them. A `noindex` page gets no
breadcrumb list, and a page with nothing to say gets no block at all rather than one holding only the
publisher.

**The JSON comes from a JSON library and every `</` is escaped**, so a `</script>` in a title cannot break
out of the block.

Source: `ruby/jekyll-structured-data/lib/jekyll-structured-data/` — `graph.rb` assembles the nodes,
`config.rb` holds the organisation, `json.rb` serialises and escapes.

## The shape every gem has

Settled 24 Aug 2026, when the two oldest gems were moved onto what the other four already did.

- **One entry file, named after the gem** — `lib/jekyll-gtm.rb`. Not a choice: a `plugins:` list runs
  `require "jekyll-gtm"`, so the name has to match.
- **Everything else under `lib/<gem-name>/`**, required from the entry file with `require_relative`. A flat
  `lib/element.rb` would sit on the shared load path where another gem's file of that name could shadow it.
- **One module per gem, inside `Jekyll`** — `Jekyll::MultiSitemap`, `Jekyll::GTM`, `Jekyll::PageMeta`,
  `Jekyll::ResourceTags`, `Jekyll::StructuredData`, `Jekyll::SiteFilters`.
- **`# frozen_string_literal: true` at the top of every `.rb`**, gemspecs and specs included.
- **`ruby/.rubocop.yml` covers all seven.** It disables `Metrics`, allows any hyphenated entry filename and
  sets the line length to 120. **Rubocop is not in `ruby/Gemfile`** — it needs `gem install rubocop`, then
  `rubocop` from `ruby/`. First run was 24 Aug 2026: everything is clean except `Gemspec/RequireMFA` and
  `Gemspec/DevelopmentDependencies`, which fire on every gem and nobody has decided on.

## binacle-docs-versions

**The one gem here written for a single site**, and named for it rather than for Jekyll. It carries the
version scheme of `sites/docs` — the front matter key `version`, the data path `versions.current` and the
collection folder `_versions` — all hardcoded, because a shared gem holding one site's vocabulary is the
thing this split exists to avoid.

**A generator stamps two plain keys** onto every versioned document at `:high` priority: `title_suffix`
(`(v2.1.x)`) and, on every version that is not `current`, `robots: noindex, follow`. `jekyll-page-meta`
writes both out and knows nothing about versions. **It never overwrites a key the page already set**, which
is how the swagger pages keep `noindex, nofollow` from their `defaults:` scope.

**`{% vlink /path %}` moved here from `sites/docs/_plugins/VLink.rb`** unchanged in behaviour, and gained a
spec suite in the move — nothing runs a `.rb` under `sites/`. `sites/docs/_plugins/` is now empty.

**Deleting the include without this gem is a silent regression**: 74 pages lose their `noindex` and every
versioned title loses its version. That is what the wiring pass measured before and after.

Source: `ruby/binacle-docs-versions/lib/binacle-docs-versions/` — `generator.rb` and `vlink_tag.rb`.

## Running the specs

Each gem has an RSpec suite under `spec/` and declares `rspec` as a development dependency in its gemspec.
All seven pass. The counts move as the gems grow, so they are not written down here.

**One `Gemfile` covers every gem, at `ruby/`, and no gem has one of its own.** It names each gem with
`gemspec path:`, which also pulls in that gemspec's development dependencies. `ruby/Gemfile.lock` is
committed, so every spec run uses the same Jekyll — 4.4.1 — rather than whatever the machine has installed.

**Bundler walks up from the current directory**, so `bundle exec rspec` inside a gem folder finds `ruby/Gemfile`
by itself. Plain `rspec` also works and ignores the lock. `bundle exec rspec` from `ruby/` itself does not:
each `spec/spec_helper.rb` is only on the load path when rspec runs from inside that gem.

**Each gem is a test leaf**, added 24 Aug 2026 — `just test ruby-gtm-unit`, and all seven are in `just test all`
because they need nothing brought up. The leaf runs `bundle exec rspec` from inside the gem folder, which is the
only place a `spec_helper` is on the load path.

**Style is `ruby/.rubocop.yml`, one config for all seven.** Rubocop is not in the bundle: `gem install rubocop`,
then run `rubocop` from `ruby/`. Two gemspec cops fire on every gem — `Gemspec/RequireMFA` and
`Gemspec/DevelopmentDependencies` — and nothing else does.

**They carry no coverage.** Ruby's collector is simplecov and it is not in the bundle, so a `just coverage` run
executes them and writes no file for them — they are absent from the table rather than sitting at zero.

**No workflow runs them.** The only workflow that reads `ruby/` is CodeQL, which analyses rather than tests.
