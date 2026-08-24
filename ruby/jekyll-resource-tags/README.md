# jekyll-resource-tags

Three Liquid tags that render `<link>`, `<script>` and prefetch elements from a list in your site data or
page front matter. They replace the two-line `{% for %}` include that a Jekyll site otherwise copies once
per element type.

There is no config block. Everything a tag renders comes from the list you hand it, so the gem carries into
any Jekyll site unchanged.

## 📂 The three tags

| Tag | Writes | Reads the path from |
|---|---|---|
| `{% link_tags <list> %}` | one `<link ...>` per item | `href` |
| `{% script_tags <list> %}` | one `<script ...></script>` per item | `src` |
| `{% prefetch_tags <list> %}` | one `<link rel="prefetch" as="script" ...>` per item | `src` |

One list per call. How many lists you keep, and where in the page each is rendered, is yours to decide. The
gem never merges two lists and never picks an order between them.

Hand `prefetch_tags` the script list you later execute, and the two can never drift.

## 🚀 Quick start

Both lines are needed. Miss the second and the build fails: Liquid raises `Unknown tag 'link_tags'` on the
first page that uses one.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-resource-tags", path: "path/to/jekyll-resource-tags"
```

```yaml
# _config.yml
plugins:
  - jekyll-resource-tags
```

Then put a list somewhere Liquid can read it:

```yaml
# _data/includes.yml
links:
  - rel: stylesheet
    type: text/css
    href: /css/main.css

scripts_body_end:
  - type: text/javascript
    src: /js/main.js
    defer: true
```

```liquid
{% link_tags site.data.includes.links %}
{% script_tags site.data.includes.scripts_body_end %}
```

```html
<link rel="stylesheet" type="text/css" href="/css/main.css">
<script type="text/javascript" src="/js/main.js" defer></script>
```

A page's own front matter works the same way - `{% script_tags page.scripts %}`.

## ⚙️ How an item becomes an element

Every key on an item becomes an attribute, in the order the keys appear in the data. Rename a key and you
rename the attribute; move it and the attribute moves with it. There is no fixed attribute order, and the
only keys the gem knows by name are the path keys and the three below that `prefetch_tags` drops.

- `href` and `src` go through `relative_url`, so a site with a `baseurl` needs no thought. A value that
  already carries a scheme - `https://`, `data:` - or starts with `//` is written as it stands.
- A key with no value writes no attribute. A blank `type:` does not become `type=""`.
- `true` writes a bare attribute, `false` writes nothing. That is how you get `defer`.
- An item with no path writes no element at all. `<link rel="stylesheet">` with no `href` renders, loads
  nothing and reports nothing, so it is dropped instead.
- Every value is HTML-escaped, so a quote in your data cannot close an attribute early.
- A nil or empty list renders an empty string and does not raise. Most pages do not set `page.scripts`.
- `prefetch_tags` drops `async`, `defer` and `nomodule`. They are the only content attributes that exist on
  `<script>` and not on `<link>`, so on a prefetch element they do nothing and a validator flags them.
  Everything else on the item carries through - `crossorigin` in particular, without which a cross-origin
  script gets fetched twice.

## ⚠️ Gotchas

- Key order is load-bearing. It is what lets you move an existing site onto these tags with no change to its
  built pages: write the data keys in the order the old template emitted them. Ruby and the YAML parser both
  preserve insertion order, and a spec holds that down.
- The tags add no whitespace of their own. Items are joined with one newline and nothing is written before
  the first or after the last. The line break around the call site is yours to place.

## 🧪 Tests

```bash
bundle exec rspec
```

The specs build a real Jekyll site from `spec/fixtures/site` and read the pages it wrote.
