# binacle-www

One site's gem, on purpose. It holds the Ruby that only `sites/www` uses: the filter that turns a markdown page
into the design's sections. It writes the design's class names, such as `band__body` and `columns__column`, so it
changes when those do. The jobs both site gems write the same way come from
[`binacle-sites-core`](../binacle-sites-core).

## 📂 What is in it

| Surface | Does |
|---|---|
| `www_sections` | turns a page kramdown has rendered into sections |

## 🚀 Quick start

Both lines are needed. Miss the second and Liquid leaves the page as it was, with no sections.

```ruby
# Gemfile, binacle-www inside group :jekyll_plugins
gem "binacle-sites-core", path: "../ruby/binacle-sites-core"   # outside the group; it has no plugin
gem "binacle-www", path: "../ruby/binacle-www"
```

```yaml
# _config.yml
plugins:
  - binacle-www
```

```liquid
{{ content | www_sections }}
```

Three kramdown settings matter to it:

```yaml
kramdown:
  auto_ids: false             # or every section takes an id from its heading
  parse_block_html: false     # or kramdown reads an include's HTML as markdown and shows its tags
  smart_quotes: apos,apos,quot,quot
```

## 🧱 The markdown

Every `#` and `##` starts a section. The line under it says how the section looks:

```markdown
## The three algorithms
{: .band--wide .columns #algorithms data-label="Algorithms" }
```

- `.band--wide` and the other `band--` classes go on the section, and so does the id.
- `data-label` is the small label beside the section. Left empty, its column stays and is blank.
- With neither a label nor `band--split`, `band--mirrored` or `band--wide`, the section has no grid. In a
  `band--mirrored` one the label sits over the heading.
- `.split--text-left` and the other `split--` classes put the words beside what follows them.
- `.band__body--code-left`, in a wide section, puts a short code block under the left column of text.
- `.statement` is one line at display size and the words under it, with no label. Each sentence of the heading
  is kept whole, and the heading comes out as a paragraph.
- `.prose--one-line` is for a wide section with one short line under its heading, which then sits below the heading
  and not beside it.
- `.hero` is the top of the home page, with no label: the words and the actions on one side, the picture on the
  other. The actions start at the paragraph marked `{: .actions }`. One marked `{: .hero__source }` is the line under
  them. It takes one picture.
- `.columns` makes a column of every `###`. A `---` ends them, with a blank line above it: without one, markdown
  reads the line above as a heading. No `###`, or a `---` anywhere else, stops the build.
- In a list marked `{: .labelled-list }`, an item that opens in bold is the cost, and the bold is its label.
- A paragraph marked `{: .verdict }` goes into the picture or the request before it. In a fit-row it is the caption.
- A table with no class is drawn as the design's. A cell that is only `*text*{: .table__off }` is greyed out, and one
  that is only code is set as a value.
- A list marked `{: .doors }` becomes link tiles in a `nav`, named by the nearest heading above it. An item is `[Title](link) - text`; anything else stops the build.
  A picture may come before the link, and the list is then marked `.doors--pictured` too.
- A numbered list marked `{: .path aria-label="On this page" }` is the row of steps. It comes out after the section it
  is written in. An item is only a link.
- A code block marked `{: data-name="Signature" }` has a head with that name. Without it there is only the copy
  button.

A mark on a heading that the filter does not know stops the build and is named. The list is at the top of
`lib/binacle-www/sections.rb`, and a spec checks each one is in the design system's Sass.

Words get the design's reading width by themselves. A page that starts with anything but a heading stops the
build.

## 🧪 Tests

```bash
bundle exec rspec
```
