# jekyll-structured-data

Writes one JSON-LD block per page - the hidden graph that tells a search engine what the page is, who
published it and where it sits.

It works nothing out. Every value comes from config, from front matter, or from a key another plugin has
already put on the page. A title resolved twice is a title free to disagree with itself, and what notices is
a search engine comparing your markup against your visible page.

## 📂 What it reads

| Key on the page | Becomes | Usually written by |
|---|---|---|
| `page.meta.title` | the node's `name` | your page meta plugin |
| `page.meta.description` | its `description` | the same |
| `page.meta.canonical` | its `url`, and the `@id` of every node | the same |
| `page.meta.image` | its `image` | the same |
| `page.breadcrumb_trail` | the `BreadcrumbList` | your breadcrumb plugin |
| `page.robots` | nothing, except that `noindex` drops the trail | the page, or a plugin |

It requires none of those plugins and checks for none of them. Write the keys in front matter yourself if
you like. A key it cannot find is a field it leaves out.

`breadcrumb_trail` is a list of `name` and `url`, in order, the first crumb first.

## 🚀 Quick start

Both lines are needed. Miss the second and nothing fails - Jekyll prints the tag name as text.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-structured-data", path: "path/to/jekyll-structured-data"
```

```yaml
# _config.yml
plugins:
  - jekyll-structured-data
```

Then call the tag once, in your head:

```liquid
<head>
  {% structured_data %}
</head>
```

## ⚙️ Configuration

```yaml
structured_data:
  default_type: WebPage        # unset. A page with no structured_data.type gets no page node
  organization:
    "@id": https://www.example.com/#organization
    name: Example Ltd
    url: https://www.example.com
    logo: /media/logo.png
    same_as: [https://github.com/example]
  defaults:
    WebApplication:
      applicationCategory: DeveloperApplication
      operatingSystem: Any
```

Pin the organisation `@id` and never derive it. One absolute url, written the same way in every site you
run, is what makes a crawler treat several hosts as one publisher. A gem deriving it from `site.url` would
claim a different organisation on each of them.

Every key under `organization:` is written out as you type it. `same_as` is the one exception, because
schema.org spells it `sameAs`. `@type` is `Organization` unless you set another. A relative `logo` is
resolved against the organisation's own `url`, not against the page.

`defaults:` is per type. Its keys go under the page node, and anything the page says wins.

`default_type` is the type for a page that names none. Leave it unset and those pages get no node.

## 🏷️ Front matter

```yaml
structured_data:
  type: WebApplication
  name: Packing Demo      # optional. The resolved page title otherwise
```

`type` picks the node's `@type`. Every other key under `structured_data:` is written into the node as it
stands, so a page can carry any schema.org property without this gem learning about it.

## 🧾 What comes out

```json
{
  "@context": "https://schema.org",
  "@graph": [
    { "@type": "Organization", "@id": "https://www.example.com/#organization" },
    { "@type": "WebApplication", "@id": "https://example.com/demo/#page",
      "publisher":  { "@id": "https://www.example.com/#organization" },
      "breadcrumb": { "@id": "https://example.com/demo/#breadcrumbs" } },
    { "@type": "BreadcrumbList", "@id": "https://example.com/demo/#breadcrumbs" }
  ]
}
```

One block, with the parts joined by `@id` rather than nested. Nothing is written unless there is something
to say about the page: with no page node and no trail, the tag renders an empty string rather than a graph
holding only the publisher.

## ⚠️ Gotchas

- A `</script>` in a title would close the block early and render the rest of your graph as text on the
  page. Every `</` is escaped on the way out. The JSON comes from a JSON library, never from a template, so
  a quote, a newline or a non-Latin character in a title cannot break it either.
- A `noindex` page gets no `BreadcrumbList`, and its page node gets no `breadcrumb` link. A trail on a page
  nothing will crawl claims a hierarchy nothing will follow. On a documentation site that is most of the
  pages.
- Every `@id` is built from the canonical url, so a page without one gets nodes with no `@id` and no link
  between them. Fix the canonical, not this.
- Nothing here invents a value. No rating, no review count, no date you cannot prove. If you want a field,
  put it in `structured_data:` or in `defaults:`.
- An organisation block with no `@id` fails the build. It is the one key that cannot be guessed at.

## 🧪 Tests

```bash
bundle exec rspec
```

The specs build a real Jekyll site from `spec/fixtures/site` and parse the block out of the pages it writes.
