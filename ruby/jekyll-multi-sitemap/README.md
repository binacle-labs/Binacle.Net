# jekyll-multi-sitemap

Writes a Jekyll site's sitemap files, and the index over them, from one config block. It replaces the
`<url>` loop that sites otherwise copy into a template per sitemap.

This is not the published `jekyll-sitemap` gem. This one writes several files, keeps `changefreq` and
`priority`, filters a collection down to the entries you want, and writes the `Sitemap:` lines for your
`robots.txt`.

## 📂 What is in it

One renderer, three ways in. Each is a thinner wrapper than the one above it.

| Surface | Writes | For |
|---|---|---|
| the generator | every sitemap file, and the index | the normal case |
| `{% sitemap_urlset <list> %}` | one whole `<urlset>` document | a site that wants to own the file |
| `{% sitemap_url <document> %}` | one `<url>` element | a hand-written loop doing something odd |
| `{% sitemap_links %}` | the `Sitemap:` lines for `robots.txt` | every site |

## 🚀 Quick start

Both lines are needed. Miss the second and nothing fails - Jekyll prints the tag name as text and the
generator never runs.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-multi-sitemap", path: "path/to/jekyll-multi-sitemap"
```

```yaml
# _config.yml
plugins:
  - jekyll-multi-sitemap
```

A site with one sitemap needs three more lines:

```yaml
sitemaps:
  files:
    - name: sitemap
      include: [pages]
```

That writes `/sitemap.xml`, listing every page that opts in.

## ⚙️ Configuration

```yaml
sitemaps:
  path: /sitemap            # where the files go. Default "/"
  index: /sitemap.xml       # optional. A path, not a flag
  mode: opt-in              # opt-in (default) | opt-out
  files:
    - name: pages
      include: [pages, common_pages]
    - name: version-current
      include: [versions]
      where:
        version: $site.data.versions.current
```

`files` - one entry per sitemap. `name` becomes the filename, and `.xml` is added if you leave it off.

`include` - `pages` means `site.pages`. Every other name is a collection, and an unknown name fails the
build. Two names in one list are concatenated into one file.

`where` - a front matter key matched against a value. A value starting with `$` is a dotted path into site
data, read at build time; `$site.data.versions.current` and `$data.versions.current` mean the same thing.
Anything else is matched literally. That is the only form there is - for anything harder, use a tag.

`mode` - `opt-in` lists a document only when its front matter sets `sitemap.exclude` to literally `false`.
`opt-out` lists everything except `sitemap.exclude: true`, and is what most sites want. `opt-in` is the
default, and it is a trap: a `defaults:` scope that forgets the key drops its pages with no error.

`index` - a path. Set it and the gem writes a sitemap index there, naming every file it generated. An index
over a single sitemap buys nothing, so leave it out on a one-file site.

No `files` key at all is a site that only uses the tags. The generator then writes nothing, and `mode`
still applies to `{% sitemap_urlset %}`.

## 🧾 What one entry contains

Read from the document's own `sitemap:` block:

```yaml
sitemap:
  exclude   : false
  lastmod   : "current"    # or a date. "current" is the site build time
  changefreq: "monthly"
  priority  : 1.0
```

```xml
<url>
  <loc>https://example.com/about.html</loc>
  <lastmod>2024-03-04T00:00:00+02:00</lastmod>
  <changefreq>monthly</changefreq>
  <priority>1.0</priority>
</url>
```

`index.html` is dropped from a location, and the url is made absolute from `url` and `baseurl` in
`_config.yml`. A value that is not set writes no element, because an empty `<changefreq></changefreq>` is
not schema-valid.

## 🏷️ The tags

`{% sitemap_urlset <list> %}` writes a whole document - the XML declaration, the `<urlset>` and its urls -
from a list you hand it. It applies `mode` from your config block. Use it when the site wants to own the
file:

```liquid
{%- assign entries = site.versions | where: "version", site.data.versions.current -%}
{%- sitemap_urlset entries -%}
```

`{% sitemap_url <document> %}` writes one `<url>` element and nothing else, for a hand-written loop doing
something no config block can express. Treat it as a last resort: the site is then left to write the
`<urlset>`, the xmlns and the XML declaration, which is where sitemaps break.

`{% sitemap_links %}` writes the `Sitemap:` lines for a `robots.txt`. If you set `index`, it emits the index
alone; otherwise it emits every file. Pointing a crawler at both an index and its children is what a loop
over the files would do.

```liquid
User-Agent: *

{% sitemap_links %}
```

The same urls are readable as `site.sitemaps.urls`, for a site that wants its own line format.

## ⚠️ Gotchas

- A tag takes a variable name, not an expression. Liquid hands a tag its markup as plain text, so
  `{% sitemap_urlset site.versions | where: "version", "2.0" %}` never runs the filter. Assign first, then
  pass the name. The gem raises rather than letting that render as an empty file.
- The generator runs last, at Jekyll's lowest priority, so it sees pages other plugins created. A plugin
  that adds pages at an even lower priority is not in the sitemap, and nothing fails.
- An index path that is also a generated file fails the build. The two would fight over one path and the
  loser would be whichever ran last.
- `opt-in` mode drops a page that says nothing. See `mode` above. If you are adding this to a site with no
  `sitemap:` front matter at all, you want `opt-out`.
- Renaming a sitemap file changes a live url. Search engines fetch the path you submitted to them, and a
  renamed file 404s until you submit the new one.

## 🧪 Tests

```bash
bundle exec rspec
```

The specs build a real Jekyll site from `spec/fixtures/site` and read the files it writes.
