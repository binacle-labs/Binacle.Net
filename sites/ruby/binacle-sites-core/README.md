# binacle-sites-core

The markup and the section builder the Binacle site gems share. It is plain Ruby on a Nokogiri fragment. It registers no Liquid
filter, tag or generator: a site gem calls it from its own filter.

## 📂 What is in it

| Method | Does |
|---|---|
| `add_class(element, name)` | adds a class to an element and keeps the ones it has |
| `name_labelled_lists(fragment)` | in a `ul.labelled-list`, gives each item its class, and makes an opening bold the cost's label |
| `grey_out(cell)` | marks a table cell that is only `*text*{: .table__off }` as greyed out, and drops the emphasis |
| `name_doors(fragment)` | turns each `ul.doors` into a `nav` of links, and gives back the navs |
| `name_code_blocks(fragment, selector:, plain:)` | wraps each code block in the design's `code-block`, with the copy button |
| `name_tables(fragment, selector:, cell:)` | gives each table the design's class and markup, and gives back the tables |

The methods are module functions of `Binacle::SitesCore::Markup`. A converter includes the module and calls them
as its own.

`Binacle::SitesCore::Sections.new(html).to_html` turns a page into the design's sections. A site subclasses it.

| Hook | A site overrides it to |
|---|---|
| `site_looks` | list the looks it adds to the shared ones |
| `site_section(blocks, looks, label, id)` | build a section its own way, or give back `nil` for the shared one |
| `prepare` | work on the whole page before it is split |
| `part(blocks)` | add what comes after a section |
| `divide(blocks, looks)` and `draw_columns(blocks)` | cut out and draw a part inside the body |

## 🧱 The shared jobs

**Link tiles.** An item is `[Title](link) - text`; anything else raises `ArgumentError`. A picture may come before the
link. The list becomes a `nav` that keeps its classes, and takes its `aria-label` from the nearest heading above it, even with words between. It
gives back the navs, so a site can set the classes it wants on them.

**Code blocks.** The `selector` says which `pre` elements to wrap, as each site matches its own. A block becomes
`div.code-block[data-code-block]` with `pre.code-block__pre`, and the empty last line is dropped. With `data-name` on the
block the markdown gets a head with that name, escaped. Without it the copy button floats in the corner.

- `plain: true` turns the highlighter's spans into text.
- A block given to the method runs on each `pre` with the `pre`, its `code` and the outer element, before the code's
  class is removed. What it returns is used as the head, or `nil` keeps the shared one. `code_head(inner_html)` and
  `code_label(name)` build one.

**Tables.** The `selector` says which tables to take. Each gets the class `table` and `data-table`, `scope="col"` on its
header cells, and a `data-label` on each body cell from its column. A right-aligned cell loses its style and takes
`table__number`. A cell that is only `*text*{: .table__off }` is greyed out.

- `cell:` is called on each body cell first, for what a site does to a cell itself.

## 📑 The sections

Every `h1` and `h2` starts a section, and the attributes on the heading say how it looks. A page that starts with
anything but a heading stops the build, and so does a `---` that no part of the site takes.

- `band--` classes go on the section, and so does the id. `data-label` is the small label beside it; left empty, its
  column stays and is blank.
- With neither a label nor `band--split`, `band--mirrored` or `band--wide`, the section has no grid. In a
  `band--mirrored` one the label sits over the heading.
- `split--` classes put the words beside what follows them.
- `band__body--code-left`, in a wide section, puts a short code block under the left column of text.
- `prose--one-line` is for a wide section with one short line under its heading, which then sits below the heading
  and not beside it.

Words get the design's reading width by themselves. A mark the class and the site do not know stops the build and is
named. A spec checks each shared look is in the design system's Sass.

## 🚀 Quick start

```ruby
# the site gem's gemspec
spec.add_dependency 'binacle-sites-core'
```

```ruby
require 'binacle-sites-core'

class Converter < Binacle::SitesCore::Sections
  private

  def site_looks = ['hero']
end
```

A site that loads a gem depending on this one also lists it in its `Gemfile` by path, or Bundler cannot find it:

```ruby
gem "binacle-sites-core", path: "../ruby/binacle-sites-core"
```

It goes outside `group :jekyll_plugins`, as it has no plugin to load.

## ⚠️ What will bite you

**A site's own choices stay in its gem.** Its extra looks, with the parts they draw, are its own. Which `pre` and which tables a site takes, its extra classes, its code heads
and what it does to a cell that holds only code are the site's. The sites set a cell that is only code differently, so
that part is not here.

**A class set here changes on every site.** The design's Sass has to know it.

## 🧪 Tests

```bash
bundle exec rspec
```
