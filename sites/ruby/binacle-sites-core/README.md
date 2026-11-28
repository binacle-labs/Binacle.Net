# binacle-sites-core

The markup jobs the Binacle site gems share. It is plain Ruby on a Nokogiri fragment. It registers no Liquid
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

## 🚀 Quick start

```ruby
# the site gem's gemspec
spec.add_dependency 'binacle-sites-core'
```

```ruby
require 'binacle-sites-core'

class Converter
  include Binacle::SitesCore::Markup
end
```

A site that loads a gem depending on this one also lists it in its `Gemfile` by path, or Bundler cannot find it:

```ruby
gem "binacle-sites-core", path: "../ruby/binacle-sites-core"
```

It goes outside `group :jekyll_plugins`, as it has no plugin to load.

## ⚠️ What will bite you

**A site's own choices stay in its gem.** Which `pre` and which tables a site takes, its extra classes, its code heads
and what it does to a cell that holds only code are the site's. The sites set a cell that is only code differently, so
that part is not here.

**A class set here changes on every site.** The design's Sass has to know it.

## 🧪 Tests

```bash
bundle exec rspec
```
