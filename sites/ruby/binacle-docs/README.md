# binacle-docs

One site's gem, on purpose. It holds the Ruby that only `sites/docs` uses: its version scheme, and the
filters that turn a rendered page into the docs' markup. Nothing here is portable, and nothing here should be
made portable. The markup jobs `www` writes the same way come from [`binacle-sites-core`](../binacle-sites-core): the
link tiles, the wrapper of a code block and the base of a table. The moment these become settings, every site loading the gem carries a vocabulary only one of
them uses.

The version scheme rests on the front matter key `version`, the data path `versions.current`, and the
collection folder `_versions`. The filters write the design's class names, such as `doc__text` and
`code-block__pre`, so they change when those do.

## 📂 What is in it

| Surface | Does |
|---|---|
| the generator | decides the url of every file under `_versions/`; stamps `version`, `version_tag`, `title_suffix`, `robots`, `version_urls` and `prev_next`, the page before and after in the menu; fails the build on two files at one url; prints the pages the previous version had that the current one lacks |
| `{% vlink /path %}` | links to a file inside the current page's version, or inside the version named first |
| `doc_body` | turns a rendered page into the design's markup |
| `doc_doors` | turns only the lists marked `doors`, and keeps the emoji |
| `doc_sections` | lists a page's `h2`s, for an outline |

## 🚀 Quick start

Both lines are needed. Miss the second and the build fails on the first `{% vlink %}` - Liquid raises
`Unknown tag 'vlink'`. The generator just never runs, so the stamps go missing without a word.

```ruby
# Gemfile
gem "binacle-sites-core", path: "../ruby/binacle-sites-core"   # outside the group; it has no plugin

group :jekyll_plugins do
  gem "binacle-docs", path: "../ruby/binacle-docs"
end
```

```yaml
# _config.yml
plugins:
  - binacle-docs
```

One thing has to be true of the site - every folder under `_versions/` is in the list, and every entry carries
all four keys. Nothing is derived; a missing one stops the build.

```yaml
# _data/versions.yml
current: v3.x
list:
  - id: v3.x            # the folder under _versions/ - never in a url
    url_segment: 3.0.0  # where it renders, /version/<url_segment>/
    label: v3.0.0       # what the selector and every page call it
    version_tag: "3.0"  # what docker pulls
  - id: v2.x
    url_segment: 2.1.1
    label: v2.1.1
    version_tag: "2.1.1"
```

## 🌐 The url

Nothing in a folder says where it renders, and a `permalink` a page writes is overwritten. The folder named
by `current` renders at the site root - `api/v3.md` → `/api/v3/`, `index.md` → `/`, `swagger/v3.json` →
`/swagger/v3.json`. Every other folder renders at `/version/<url_segment>/<rest>` - a page as a folder with an
index inside (`api/v3.md` → `/version/2.1.1/api/v3/`), a static file under its own name. Static files get this
through `VersionedFile`, a `StaticFile` whose url can be set - Jekyll's own reads the collection template and
ignores data. Moving `current` moves which folder is at the root; nothing else changes.

## 🏷️ The stamps

On every document under `_versions/<folder>/`:

- `version` - the folder name. Nothing in the folder and no config block has to say it.
- `version_tag` - the tag listed beside that id in `_data/versions.yml`, for the pull commands on the page.
  A folder with no tag in the list stops the build: the page would print a pull command with nothing after
  the colon.
- `version_label` - the list entry's `label`, what the page calls its own version.
- `title_suffix` - `(v2.1.1)`, from the label, for whatever writes the page title. Not on the current version:
  its url carries no version, so its title carries none either. On the others it keeps `Quick Start (v2.1.1)`
  apart from the root page `Quick Start`.
- `version_urls` - a map from every version to the url of this same page in that version, or to that
  version's index where the page does not exist. For a version selector that lands on the same page.
- `robots` - `noindex, follow` on every version that is not `current`.

On every entry of the `list` in `_data/versions.yml`:

- `url` - where that version's index renders, so a template lists versions without building a url from an id.

On every page whose layout is `redirect`:

- `redirect_to` - the url of the current version's index, for the layout to send a reader to.
- `canonical` - the same url, so a crawler is told the redirect and its target are one destination.
- `robots` - `noindex`.

**A redirect page's canonical points at the page it redirects to, not at itself.** That is the whole
purpose of the page, and it is why the value cannot live in front matter: it moves when `current` moves.
Both keys carry one computed url, so the head and the redirect can never disagree.

**The current version has to have an index for a redirect to point at.** Where it does not and a redirect
page exists, the build stops.

`current` is the one knob. It names the version search engines may index; move it at release time and
nothing else changes. A document with no `version` is left alone, and so is a site with no versioned
documents at all.

**`current` has to name a version the site has.** Where it does not, the build stops and names both the
value it was given and the versions it found. A `current` nobody notices is wrong would put `noindex` on
every page of the site while the sitemap still lists them.

## 🛑 The collision check

Two files rendering at one url is a Jekyll warning, and a warning is how the wrong page ships. Here it stops the
build and names both files. It covers every page and document that writes output, so a root page claiming a
versioned url fails, and so would two pages in one version.

## 🗑️ The removed-page list

At build, the generator prints every page and file in the previous version that has no counterpart at the
same path in the current one. The previous version is the entry listed right after `current` in
`_data/versions.yml`. It is printed and written nowhere: it is the redirect list whoever opens a new major
has to write.

No key knows why it is set. Whatever renders them reads ordinary values and needs to know nothing about
versions.

## 🔗 The tag

```liquid
{% vlink /swagger/v3.json %}
{% vlink /swagger/{{ page.swagger }}.json %}
{% vlink v2.x /configuration/service-module/index.md %}
```

The path is resolved inside `_versions/<the page's version>/`, against documents and static files alike, and
comes back as a url. A first word that is a version id in `versions.yml` picks that version's folder instead -
the way a page links its counterpart in another line without knowing where that line renders. Liquid inside
the argument is rendered first, so a page can build the path from its own front matter. A path that resolves
to nothing fails the build rather than writing a link to a 404.

## 🧱 The markup

`doc_body` runs on the page after kramdown and Rouge, so heading ids stay as kramdown made them.

- It drops the emoji that opens a heading, a bullet or a paragraph, makes callouts and code blocks, and puts a
  class on each block.
- A code block with `data-call` on it, `{: data-call="POST /path" }` or `{: data-call="200" }` in the markdown,
  shows that in its head in place of the language. `data-name` shows a file or field name there instead, and
  `data-wrap=""` wraps one long string. In a `}` inside an attribute, write `\}`.
- A list marked `{: .doors }` becomes links to pages. Each item is `[Title](link) - text`; anything else stops the
  build. The nearest heading above it names the list. `{: .doors .doors--grid }` makes a grid of them; any other is full width.
  The links and the label are the shared code's. The grid or full-width class is the docs'.
- In a list marked `{: .labelled-list }`, an item that opens in bold is the cost, and the bold is its label.
- A numbered list marked `{: .steps }` becomes the steps. Indent what belongs to a step by three spaces.
- Code blocks in a row, each with `{: data-tab="Windows" }`, become tabs. The first also carries
  `data-tabs="Operating system"`, what they choose between. A `data-tab` with no `data-tabs` before it stops the
  build.
- A paragraph that is one call, `` `GET /path` ``, marked `{: .http-line }`, shows the method as a badge. Anything
  else stops the build.
- A code block with `{: data-fold="Response" }` starts closed, under that text.
- A table marked `{: .result-list aria-label="The response" }` becomes result rows with a fill bar. Its columns are
  the bin, its size, the fill as a bare number, the result and the items. A row that does not fit stops the build.
- A table marked `{: .bit-diagram aria-label="..." }` becomes a row of boxes. Its columns are the size, the name in
  code, the note and the width. A name marked `{:.reserved}` or `{:.more}` is drawn faint. A row that does not fit, or no
  `aria-label`, stops the build.
- Every table takes the class `doc__table` and a box that scrolls, on top of the shared table.
- A table cell that holds only code marked `{:.mono}` takes the class `mono` too. A right-aligned column (`--:`)
  holds numbers. A cell that is only `*text*{:.table__off}` is greyed out.
- A code block after a paragraph that starts *Request Example* or *Response Example* names the call or the
  status in its head. Every response is shown as a 200.

`doc_sections` lists each `h2` as `id` and `title`. Give it `doc_body`'s result, so the titles carry no emoji.

```liquid
{% assign body = content | doc_body %}
{% assign sections = body | doc_sections %}
```

## ⚠️ What will bite you

- The generator never overwrites a key the page already set. That is how a swagger page keeps
  `noindex, nofollow` while the rest of its version is `noindex, follow`.
- Only `title_suffix` is order-sensitive. It runs at Jekyll's `:high` priority because a generator resolves
  the title from it; resolve the title first and the suffix is silently missing. `robots` is read at render,
  so nothing that reads it can run too early.
- A generator runs once, before render. A document another plugin creates later is not stamped.
- `{% vlink %}` walks every file in the site per call. Fine at this size; it is a linear scan.

## 🧪 Tests

```bash
bundle exec rspec
```

The version specs build real Jekyll sites from `spec/fixtures` - one with two versions, one whose only page
links at a file that is not there, one with no index page, and one where two pages collide. The markup specs
feed the filters HTML strings.
