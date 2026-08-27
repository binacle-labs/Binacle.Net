# Ruby Gems

Custom Jekyll plugins for the sites in `sites/`. Each site loads them from its own `Gemfile` as a local
path dependency, so none of them is published anywhere.

Two lines wire a gem into a site: one in that site's `Gemfile`, inside `group :jekyll_plugins`, and one
under `plugins:` in its `_config.yml`. Drop either and the plugin does not load.

Each gem has the same shape: a `<name>.gemspec`, one entry file at `lib/<name>.rb` that Jekyll requires by
gem name, and everything else under `lib/<name>/`.

## 📦 Gems

### 🧭 `jekyll-breadcrumb-trail`

Works out a page's breadcrumb trail from its url and publishes it as `page.breadcrumb_trail`, a list of
`name` and `url`; `{% breadcrumbs %}` renders the nav landmark, ordered list and `aria-current` from that
key and works nothing out for itself. That is what lets the structured data gem build a `BreadcrumbList`
from the same values the visible trail shows.

`exclude` suppresses a segment's label without taking it out of any url, and the home crumb points at the
deepest excluded segment rather than the site root. `jekyll-breadcrumb-trail/README.md` has the config and
the rules.

### 🔤 `jekyll-filters`

Custom Liquid filters:
- `capitalize_all` - capitalizes every word in a string
- `clean_content` - strips HTML tags, collapses whitespace, truncates to a given length (default 160 chars)
- `expand_year` - replaces `{now}` with the year of the build

`jekyll-filters/README.md` says what each one does to its input, and where each one bites.

### 📊 `jekyll-gtm`

Liquid tags for Google Tag Manager:
- `{% gtm_head <id> %}` - the GTM `<script>` block, for `<head>`
- `{% gtm_body <id> %}` - the GTM `<noscript>` block, for the start of `<body>`

Both take the container id as an argument. `jekyll-gtm/README.md` says how that id is resolved and how to
turn GTM off.

### 🗺️ `jekyll-multi-sitemap`

Writes a site's sitemap files, the index over them, and the `Sitemap:` lines for `robots.txt`, from one
config block. It also ships tags for a site that wants to write a sitemap itself. All three sites load it
and none of them has a sitemap template any more. `jekyll-multi-sitemap/README.md` has the config and the
tags.

### 🔎 `jekyll-page-meta`

Resolves a page's title, description, canonical url and preview image, then writes the head elements from
them. A generator publishes the four values as `page.meta.title`, `page.meta.description`,
`page.meta.canonical` and `page.meta.image`; `{% page_meta %}` writes the title, description, canonical,
robots, OpenGraph and Twitter card elements from those keys and works nothing out for itself.

Every setting has a default, so a site can load it and write no config at all. All three sites load it and
none of them has an seo include any more. `jekyll-page-meta/README.md` has the config and the front matter
keys.

### 🔗 `jekyll-resource-tags`

Liquid tags that render `<link>`, `<script>` and prefetch elements from a data list, one element per item:
- `{% link_tags <list> %}`
- `{% script_tags <list> %}`
- `{% prefetch_tags <list> %}`

Every key on an item becomes an attribute, in the order the data declares it. There is no config block -
everything comes from the list handed to the tag. `jekyll-resource-tags/README.md` has the rules.

### 🧾 `jekyll-structured-data`

Writes one JSON-LD block per page - the hidden graph that says what the page is, who published it and where
it sits. It reads the keys other plugins publish (`page.meta.*`, `page.breadcrumb_trail`, `page.robots`) and
works nothing out for itself, so the markup can never disagree with the head.

The organisation lives in one config block with a pinned `@id`, written identically in all three sites, which
is what makes three hosts one publisher. A page names its node with `structured_data: type:` in front matter.
All three sites load it. `jekyll-structured-data/README.md` has the config and the front matter.

### 📱 `jekyll-webmanifest`

Writes a site's web app manifest - the JSON file a browser reads to install the site to a home screen - from
one config block, so the site keeps no `site.webmanifest` page of its own. Every key it does not know is
written into the JSON untouched, so a site can add `orientation` or `shortcuts` without the gem learning what
they are.

The built path is readable as `site.webmanifest.url`, and `{% webmanifest_link %}` writes the head element
from it. `jekyll-webmanifest/README.md` has the config and the tag.

### 🤖 `binacle-robots`

The body of `robots.txt`, held once instead of three times. `{% robots %}` writes the content-signal
preamble, the Article 4 rights reservation and `User-Agent: *`; the `Sitemap:` lines come from
`{% sitemap_links %}` on the next line. There is no config - a body that can be configured per site is a
body that can differ per site.

Like `binacle-docs-versions`, the name says it is not portable. `binacle-robots/README.md` has the file
shape and the traps.

### 📚 `binacle-docs-versions`

The one gem here written for a single site. It carries the version scheme of `sites/docs` - a generator
stamps `title_suffix` and `robots` onto every versioned document, and `{% vlink %}` links to a file inside
the version the page belongs to.

The vocabulary is hardcoded on purpose: making it configurable would put a scheme only one site uses in
front of all of them. `binacle-docs-versions/README.md` has the two site keys it needs.

## 🧪 Tests

Every gem here shares one bundle, `ruby/Gemfile`. Install it once:

```bash
cd ruby && bundle install
```

Then run a gem's specs from its own folder. Bundler walks up and finds the shared `Gemfile` on its own:

```bash
cd ruby/jekyll-multi-sitemap && bundle exec rspec
```

From the repo root each gem is a test, so `just test rb_jekyll-gtm_unit` runs one and `just test all` runs
them with everything else. Coverage comes with them, through simplecov.

For style there is `.rubocop.yml`, covering all ten gems. Nothing runs it - no recipe, no pipeline step. Run
it by hand from here:

```bash
cd ruby && bundle exec rubocop
```

Adding a gem means one `gemspec path:` line in `ruby/Gemfile` and one recipe in the test module.
