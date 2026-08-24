# jekyll-page-meta

Resolves a page's title, description, canonical url and preview image, then writes the head elements from
them. It replaces the `_includes/seo.html` that a Jekyll site otherwise copies from site to site and then
edits in one place only.

This is not the published `jekyll-seo-tag`. This one trims a description on characters, the way a search
result cuts it, and it takes a per-page `robots` value and a title suffix another plugin stamps.

The resolving and the writing are two halves. A generator resolves four values onto every page; a tag writes
the elements. That is what lets a second plugin - structured data, say - read the same title the head
carries instead of working out its own.

## 📂 What is in it

| Surface | Does |
|---|---|
| the generator | resolves `page.meta.title`, `.description`, `.canonical` and `.image` |
| `{% page_meta %}` | writes the title, description, canonical, robots, OpenGraph and Twitter card elements |

## 🚀 Quick start

Both lines are needed. Miss the second and the build fails on the first `{% page_meta %}` - Liquid raises
`Unknown tag 'page_meta'`. The generator just never runs, so nothing publishes the four keys.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-page-meta", path: "path/to/jekyll-page-meta"
```

```yaml
# _config.yml
plugins:
  - jekyll-page-meta
```

Then call the tag once, in your head:

```liquid
<head>
  {% page_meta %}
</head>
```

There is no config to write. Every setting below has a default.

## ⚙️ Configuration

```yaml
page_meta:
  title_separator: " - "
  description:
    from: [description, excerpt, site]
    truncate: 160
  twitter_card: summary
  twitter_site: ""
```

`title_separator` - what sits between the page title and the site title.

`description.from` - where a description comes from, first hit wins. Each name is a front matter key, plus
`content` for the page body and the two reserved words `excerpt` and `site`. `site` means
`site.description`.

Never list a key your templates *print*. A blurb written for a card is not a search snippet, and putting it
in this list ships it as one.

`description.truncate` - the character count to cut at. `0` or `false` cuts nothing. It cuts on characters,
not on words, because that is what a search result does. There is no ellipsis.

`twitter_card` - the card type. `summary` unless you say otherwise.

`twitter_site` - the site's Twitter handle. Unset by default, and nothing is written without it. A leading
`@` is added if you leave it off.

## 🏷️ Front matter

Every key is named after the element it becomes, so nothing here is a word this gem invented.

| Key | Becomes |
|---|---|
| `title` | the page half of `<title>` |
| `seo_title` | `<title>`, on its own. Beats everything below it |
| `title_suffix` | appended to the title, before the separator. Meant for a plugin to stamp, not for a person to type |
| `description` | `<meta name="description">`, `og:description`, `twitter:description` |
| `og_image` | `og:image` and `twitter:image`. Falls back to `site.og_image` |
| `robots` | `<meta name="robots">`. Written only when the page has one |
| `canonical` | `<link rel="canonical">` and `og:url`, overriding the derived url |
| `locale` | `og:locale`. Falls back to `site.locale` |

The title, in order: `seo_title` wins outright; otherwise the page title, plus the suffix if there is one,
plus the separator and the site title; with no page title, the site title alone.

## 🔑 The four keys it publishes

```
page.meta.title   page.meta.description   page.meta.canonical   page.meta.image
```

They are the contract. Read them from a template, or from another plugin that needs the same values. A
title resolved twice is a title free to disagree with itself, and what notices is a search engine comparing
your markup with your visible page.

A key it cannot resolve is not written. A page with no url has no `canonical`, and a site with no image
anywhere has no `image`. Anything reading these omits the field rather than writing an empty one.

## ⚠️ Gotchas

- The generator runs at Jekyll's `:low` priority, so anything stamping a key it reads must run higher. A
  plugin that writes `title_suffix` after this one has resolved the title leaves the suffix out of
  `page.meta.title`, and nothing errors.
- One description pipeline, whatever the source. Markdown first, then tags out, then whitespace collapsed,
  then the cut. A `description` written as plain prose comes out as it went in; an excerpt written in
  markdown comes out as sentences rather than asterisks.
- A description is built before Liquid has run. The generator sees raw page content, so `{% ... %}` and
  `{{ ... }}` are dropped from the text rather than rendered into it. A page whose first paragraph is mostly
  Liquid gets a thin description - write a `description` for it.
- An empty canonical is worse than none. `<link rel="canonical" href="">` self-references the wrong page, so
  a page with no url gets neither a canonical nor an `og:url`.
- `twitter:site` is not written unless you configure it. Adding a handle for the first time is a change to
  every page, not a fix.
- A description that came from markdown carries typographic quotes. Kramdown converts them, and the cut
  happens after that.

## 🧪 Tests

```bash
bundle exec rspec
```

The specs build a real Jekyll site from `spec/fixtures/site`. A hash pretending to be a page never
exercises the excerpt, which is the link that breaks.
