---
id: sites/ruby
description: sites/ruby/ - the two gems only the Binacle sites read, binacle-robots and binacle-docs. What each stamps and writes, what fails the build, and how their bundle differs from ruby/.
verified: 2026-11-08
check: The two gems in sites/ruby/Gemfile match the folders; binacle-robots registers one robots tag and its spec still pins the content signals commented out; binacle-docs registers vlink, doc_body and doc_sections, and its generator still runs at priority :high and stamps version, version_tag, version_label, version_urls, prev_next, title_suffix and robots, the url on each versions list entry, and the three redirect keys; www, docs and demo load binacle-robots and only docs loads binacle-docs, each through its Gemfile with path ../ruby/<gem> and its plugins list; sites/ruby/ruby.rbproj still carries the ruby coverage report path; both locks hold the same Jekyll
also_update:
  - ruby
  - sites/docs
paths:
  - "sites/ruby/**"
---

# Site gems

`sites/ruby/README.md` says what the two gems are, how to install the bundle and run the specs. This page holds
what that does not.

**They are the gems that name the product.** The generic gems are in `ruby/`, and the rules they all follow -
one entry file, everything else under `lib/<gem>/`, `frozen_string_literal` on every file - are in `$ruby`.
These two sit in `Binacle::` rather than `Jekyll::`: `Binacle::Robots` and `Binacle::Docs`.

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
it is. `sites/docs/_layouts/page.html` pipes every page through it except the home page. It:

- names each top-level block with the design system's `doc__` classes, tables as `table doc__table`, and a
  table cell holding only code as `mono`;
- turns a quote marked `{: .block-warning }` or another kind, or opening with a GitHub `[!NOTE]` line, into a
  `callout` of that kind;
- rewraps every code block as a `code-block` with a head: the language, or the `data-name` the markdown gives,
  or the call in `data-call`. `data-wrap` makes one long line wrap. Only JSON keeps Rouge's token colours;
- names an example's head after the call: the method and path from the paragraph above a *Request Example*,
  `200` over a *Response Example*, and every JSON block after it up to the next heading;
- drops an opening emoji, and a rule straight before a section heading or at the end, since a section draws
  its own line.

**It writes the design system's class names**, so a class renamed in the Sass breaks a docs style with no error.
Which emoji the page keeps is in `$sites/docs`.

**`doc_sections`** lists the page's `h2` headings as `id` and `title`, for the *On this page* outline. It reads
them with a pattern, not a parser, because kramdown writes every `h2` the same way.

Source: `lib/binacle-docs/` - `versions/` (`generator.rb`, `urls.rb`, `pages.rb`, `versioned_file.rb`,
`vlink_tag.rb`), `pager.rb`, and `markup/` (`doc_body.rb`, `sections.rb`).

## The bundle

**`sites/ruby/Gemfile` is a second bundle**, apart from `ruby/Gemfile`, with its own lock. Both locks have to
hold the same Jekyll, or the two sets of gems are tested against different versions. The coverage gems and
Rubocop are in it too, and `.rubocop.yml` takes its rules from `ruby/.rubocop.yml`.

**The specs run the same way as the generic gems'** - `just test rb_binacle-docs_unit`, one step each in
`shared-site-tests.yml`, which sets up both bundles. Coverage is merged with theirs - `$ruby`, *Coverage*.
`sites/ruby/ruby.rbproj` offers the sources to Sonar and holds the report path for this folder.
