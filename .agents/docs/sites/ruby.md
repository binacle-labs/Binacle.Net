---
id: sites/ruby
description: sites/ruby/ - the gems only the Binacle sites read, binacle-robots, binacle-docs and binacle-www. What each stamps and writes, what fails the build, and how their bundle differs from ruby/.
verified: 2026-11-26
check: The gems in sites/ruby/Gemfile match the folders; binacle-www registers one filter, www_sections; binacle-robots registers one robots tag and its spec still pins the content signals commented out; binacle-docs registers vlink, doc_body, doc_doors and doc_sections, and its generator still runs at priority :high and stamps version, version_tag, version_label, version_urls, prev_next, title_suffix and robots, the url on each versions list entry, and the three redirect keys; www, docs and demo load binacle-robots and only docs loads binacle-docs, each through its Gemfile with path ../ruby/<gem> and its plugins list; sites/ruby/ruby.rbproj still carries the ruby coverage report path; both locks hold the same Jekyll
also_update:
  - ruby
  - sites/docs
paths:
  - "sites/ruby/**"
---

# Site gems

`sites/ruby/README.md` says what the gems are, how to install the bundle and run the specs. This page holds
what that does not.

**They are the gems that name the product.** The generic gems are in `ruby/`, and the rules they all follow -
one entry file, everything else under `lib/<gem>/`, `frozen_string_literal` on every file - are in `$ruby`.
These sit in `Binacle::` rather than `Jekyll::`: `Binacle::Robots`, `Binacle::Docs` and `Binacle::Www`. In `Binacle::Docs` the
folders are modules, `Versions` and `Markup`, with `Pager` beside them; a dash in a gem's name is a nested module,
as the RubyGems naming guide has it. **`binacle-docs` and `binacle-www` depend on Nokogiri**, for `doc_body` and
`www_sections`; the generic `jekyll-filters` does not, so `demo` has no Nokogiri in its lock.

**A site loads them the same two ways it loads the generic ones**, but its `Gemfile` path is
`../ruby/<gem>`, one level shorter.

## binacle-robots

**All three published sites load it.** `{% robots %}` writes the `robots.txt` body - the content-signal
preamble, the Article 4 rights reservation under EU directive 2019/790, and `User-Agent: *`.

**It was worth a gem for what the text is, not for its size.** Three hand-kept copies of a legal statement
drift, and the copy that drifts is the one that stops saying what was meant. Nobody would notice until they
read all three side by side.

**A tag, not a generator**, because it needs the sitemap urls and a generator could not be ordered after
the one that produces them. `$ruby/decisions#R6`.

**No config.** A body that can be configured per site is a body that can differ per site.

**The text is a data file**, `lib/binacle-robots/robots.txt`, read once at load and written out unchanged.
Nothing is substituted into it. The tag emits no trailing newline - the newline after it in the site's file
supplies that, and `{%- robots -%}` would eat it and change the bytes.

**The content signals are commented out**, and a spec pins that. Turning them on is a decision, and it would
reach all three sites at once.

Source: `lib/binacle-robots/` - `body.rb` reads the file, `tag.rb` writes it.

## binacle-docs

**`sites/docs` only.** Everything Ruby that only the docs site uses: its version scheme, the page before and
after each page, and two filters that turn a rendered page into the design system's markup.

### The versions

**The version scheme of `sites/docs` is hardcoded** - the front matter key `version`, the data path
`versions.current` and the collection folder `_versions` - because a shared gem holding one site's vocabulary
is the thing the split from `ruby/` exists to avoid.

**A generator decides every url under `_versions/`** - the `current` folder at the site root, every other at
`/version/<url_segment>/<rest>`, the segment read off `_data/versions.yml` beside the folder's id, a page as a
folder with an index, a static file under its own name. A `permalink` a page writes is overwritten. Static
files get a settable url through `VersionedFile`, a `StaticFile` subclass, because Jekyll's own ignores data.

**It also stamps plain keys**, all at `:high` priority. On every document under `_versions/<folder>/`:

- `version` - the folder name, so no config block has to say it;
- `version_tag` and `version_label` - from the list entry beside that id in `_data/versions.yml`. Every entry
  carries `url_segment`, `label` and `version_tag`; a folder with no entry, or an entry missing a key, stops the
  build;
- `version_urls` - this page's url in every other version, or that version's index where the page is missing.
  For the selector;
- `prev_next` - the page before and after it, walking that version's menu: the index first, as *Home*, then
  each entry by `nav.order` followed by its children. A page with `nav.exclude` is left out. Where two pages
  share a menu title, the link reads *Parent / Title*. Not `pager`: Jekyll asks every page for that, for its own
  pagination, and warns.

On every version that is not `current`: `title_suffix` (`(v2.1.1)`, from the label) and
`robots: noindex, follow`. The current version's url carries no version, so its title carries none, and it is
the one that is indexed. On every page whose layout is `redirect`: `redirect_to`, `canonical` and
`robots: noindex`. **Each entry in the versions list gets a `url`**, its index, so no template builds a url from
an id. `jekyll-page-meta` writes the head out and knows nothing about versions.

`site.documents` holds the collection's static files too, so the stamps land on their `data`. **It never
overwrites a key the page already set**, which is how the swagger pages keep `noindex, nofollow` from their
`defaults:` scope.

**The generator fails the build when two files render at one url** - Jekyll only warns - and **prints the pages
the previous version has that the current one lacks**, the redirect list for a major.

**The redirect stamps are why no head under `sites/` composes its own.** `sites/docs/_layouts/redirect.html`
would otherwise build its title, canonical and `robots` inline, because its canonical points at the current
version's root rather than at itself, and front matter cannot hold a value that moves. The generator computes
that url once; the layout reads `page.redirect_to` for the script, the meta refresh and the visible link, and
`jekyll-page-meta` reads `canonical` for the head. **Stamping only the canonical would have left the layout
computing the same url a second time.**

**`versions.current` has to name a version the site has, or the build stops.** A `current` that matches
nothing would noindex every page while the sitemap still lists them, and nothing else would say so. A
missing `current` stops the build too, for the mirror reason: it would leave every old version indexable.
The suffix is stamped from the page's own `version` and does not read `current` at all.

**`{% vlink /path %}`** takes an optional version id first (`{% vlink v2.x /index.md %}`), so a page links
another line's page by file, and the build fails if the file is not there. The first word counts as a version
only if the versions list knows it, because a path may carry Liquid with spaces in it.

**Removing this gem is a silent regression**: every old-version page loses its `noindex`, every versioned
title loses its version, and the pager disappears.

### The markup

**`doc_body` runs on the rendered page**, so kramdown's heading ids are already made and the markdown stays as
it is. `sites/docs/_layouts/page.html` pipes every page through it except the home
page, where `home.html` calls `doc_doors`. It:

- names each top-level block with the design system's `doc__` classes, tables as `table doc__table` inside a
  `doc__table-scroll` box, a table cell holding only code as `mono`, a right-aligned one as `table__number`, and one that
  is only emphasis marked `table__off` as `table__off`;
- turns a quote marked `{: .block-warning }` or another kind, or opening with a GitHub `[!NOTE]` line, into a
  `callout` of that kind;
- rewraps every code block as a `code-block` with a head: the language, or the `data-name` the markdown gives,
  or the call in `data-call`. `data-wrap` makes one long line wrap. Only JSON keeps Rouge's token colours;
- turns a list marked `{: .doors }` into a `doors` nav, labelled by the heading above it, a grid with `.doors--grid`. An item that is not
  `[Title](link) - text` stops the build;
- in a list marked `{: .labelled-list }`, makes the item that opens in bold the `labelled-list__cost`, the bold its
  label;
- adds `doc__steps` to a numbered list marked `{: .steps }`;
- turns code blocks in a row marked `data-tab` into `tabs`, labelled by the first one's `data-tabs`. A stray or
  unnamed tab stops the build;
- turns a paragraph marked `{: .http-line }` into an `http-line`, a badge and a path. One that is not a call stops
  the build;
- puts a code block marked `data-fold` in a closed `disclosure`, under that text;
- turns a table marked `{: .result-list }` into a `result-list`, one `result-row` per row. A row without its five
  columns, or with a fill that is not a number, stops the build;
- turns a table marked `{: .bit-diagram }` into a `bit-diagram`, one box per row, its `--cols` the sum of the widths. A
  row without its four columns, or a table with no `aria-label`, stops the build;
- names an example's head after the call: the method and path from the paragraph above a *Request Example*,
  `200` over a *Response Example*, and every JSON block after it up to the next heading;
- drops an opening emoji, and a rule straight before a section heading or at the end, since a section draws
  its own line.

**`doc_doors`** does only the `doors` step, so the home page keeps its emoji and its plain blocks.

**It writes the design system's class names**, so a class renamed in the Sass breaks a docs style with no error.
Which emoji the page keeps is in `$sites/docs`.

**`doc_sections`** lists the page's `h2` headings as `id` and `title`, for the *On this page* outline. It reads
them with a pattern, not a parser, because kramdown writes every `h2` the same way.

Source: `lib/binacle-docs/` - `versions/` (`generator.rb`, `urls.rb`, `pages.rb`, `versioned_file.rb`,
`vlink_tag.rb`), `pager.rb`, and `markup/` (`doc_body.rb`, `sections.rb`).

## binacle-www

**`sites/www` only.** `www_sections` runs on the rendered page, in `_layouts/page.html`. Every `h1` and `h2` starts a
section; the kramdown attributes on the heading say how it looks. `sites/ruby/binacle-www/README.md` lists them. A look that is
not in the filter's list stops the build, and a spec checks the list against the design system's Sass.

**It draws code blocks, link tiles and tables too, not the way `binacle-docs` does.** A code block has a head only when the markdown
names it, and never carries `data-lang`, which would make a bash block wrap at every width. Tiles are a plain `div`. A table's
cells carry their column's name, which a phone shows; a docs table scrolls instead.

**A section with no label column has no grid** unless its band has one column, or its body would sit in the label's place.

**It needs three kramdown settings in `sites/www/_config.yml`:** `auto_ids: false`, since a heading's id goes on its
section; `parse_block_html: false`, since with it on kramdown reads the indented HTML inside `exchange.html` and
`command.html` as code and prints the tags; `smart_quotes` as straight quotes.

**It is apart from `binacle-docs` on purpose**, though both draw labelled lists. The maintainer, 2026-11-26: "lets go
separate gem for www and we see if we need to join them later".

Source: `lib/binacle-www/sections.rb`.

## The bundle

**`sites/ruby/Gemfile` is a second bundle**, apart from `ruby/Gemfile`, with its own lock. Both locks have to
hold the same Jekyll, or the two sets of gems are tested against different versions. The coverage gems and
Rubocop are in it too, and `.rubocop.yml` takes its rules from `ruby/.rubocop.yml`.

**The specs run the same way as the generic gems'** - `just test rb_binacle-docs_unit`, one step each in
`shared-site-tests.yml`, which sets up both bundles. Coverage is merged with theirs - `$ruby`, *Coverage*.
`sites/ruby/ruby.rbproj` offers the sources to Sonar and holds the report path for this folder.
