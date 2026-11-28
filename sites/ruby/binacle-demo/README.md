# binacle-demo

One site's gem, on purpose. It holds the Ruby that only `sites/demo` uses: the filter that turns its markdown home
page into the design's sections. The jobs it needs come from [`binacle-sites-core`](../binacle-sites-core): its
section builder and the link tiles. `Converter` adds one look, the hero.

## 📂 What is in it

| Surface | Does |
|---|---|
| `demo_sections` | turns a page kramdown has rendered into sections |

## 🚀 Quick start

Both lines are needed. Miss the second and Liquid leaves the page as it was, with no sections.

```ruby
# Gemfile, binacle-demo inside group :jekyll_plugins
gem "binacle-sites-core", path: "../ruby/binacle-sites-core"   # outside the group; it has no plugin
gem "binacle-demo", path: "../ruby/binacle-demo"
```

```yaml
# _config.yml
plugins:
  - binacle-demo
```

```liquid
{{ content | demo_sections }}
```

Three kramdown settings matter to it:

```yaml
kramdown:
  auto_ids: false             # or every section takes an id from its heading
  parse_block_html: false     # or kramdown reads an include's HTML as markdown and shows its tags
  smart_quotes: apos,apos,quot,quot
```

## 🧱 The markdown

Every `#` and `##` starts a section. The line under it says how the section looks. The `band--`, `split--`,
`band__body--code-left` and `prose--one-line` looks, the label, link tiles, code blocks and tables are explained in
[`binacle-sites-core`](../binacle-sites-core). This site adds one look, `hero`, and a mark on a heading that neither list knows stops the build and is named.

`hero` marks the `#` at the top of the home page. It takes no other look and no label. The words under it become the
marketing prose. The box cards (a `nav`) and the `p.actions` follow them in the copy. One `figure` is the picture, and
the `dialog` that opens it goes after it.

```markdown
# {{ page.heading }}
{: .hero }

Words about the page.

{% include box-cards.html %}

[Pack a box](/items/){: .button .button--primary }
{: .actions }

{% include home-still.html %}
```

## 🧪 Tests

```bash
bundle exec rspec
```
