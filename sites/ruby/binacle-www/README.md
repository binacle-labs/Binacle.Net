# binacle-www

One site's gem, on purpose. It holds the Ruby that only `sites/www` uses: the filter that turns a markdown page
into the design's sections. It writes the design's class names, such as `band__body` and `columns__column`, so it
changes when those do.

## 📂 What is in it

| Surface | Does |
|---|---|
| `www_sections` | turns a page kramdown has rendered into sections. A page written in HTML comes through as it is |

## 🚀 Quick start

Both lines are needed. Miss the second and Liquid leaves the page as it was, with no sections.

```ruby
# Gemfile, inside group :jekyll_plugins
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
- `.split--text-left` and the other `split--` classes put the words beside what follows them.
- `.columns` makes a column of every `###`. A `---` ends them, with a blank line above it: without one, markdown
  reads the line above as a heading. No `###`, or a `---` anywhere else, stops the build.
- In a list marked `{: .labelled-list }`, an item that opens in bold is the cost, and the bold is its label.
- A paragraph marked `{: .verdict }` goes into the picture or the request before it.

A mark on a heading that the filter does not know stops the build and is named. The list is at the top of
`lib/binacle-www/sections.rb`, and a spec checks each one is in the design system's Sass.

Words get the design's reading width by themselves. A page that starts with anything but a heading stops the
build.

## 🧪 Tests

```bash
bundle exec rspec
```
