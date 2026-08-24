# binacle-robots

One organisation's `robots.txt` body, held once so its sites cannot drift apart. `{% robots %}` writes it.

**This gem is not portable and is not meant to be.** What it carries is a rights reservation under Article 4
of EU directive 2019/790 and a set of content signals - a legal statement belonging to one publisher, not a
feature another site would want. The name says so, which is the point of the name.

## 📂 What is in it

| Surface | Does |
|---|---|
| `{% robots %}` | writes the body: the content-signal preamble, the rights reservation, and `User-Agent: *` |
| `lib/binacle-robots/robots.txt` | the text itself, as text |

It writes no `Sitemap:` lines and knows nothing about sitemaps. Those come from whatever writes them, on the
line after the tag.

## 🚀 Quick start

Both lines are needed. Miss the second and the tag is never registered, so Liquid raises
`Unknown tag 'robots'` and the build fails.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "binacle-robots", path: "path/to/binacle-robots"
```

```yaml
# _config.yml
plugins:
  - binacle-robots
```

Then the site's `robots.txt` is front matter and two lines:

```liquid
---
permalink: /robots.txt
layout: null
sitemap:
    exclude: true
nav:
    exclude: true
---

{% robots %}

{% sitemap_links %}
```

There is no config. A body that could be configured per site is a body that can differ per site, which is
the thing this exists to prevent.

## ⚠️ What will bite

**Call it on its own line with nothing after it.** The tag emits no trailing newline - the file's own
newline supplies it. `{%- robots -%}` eats that newline and the byte output changes.

**Do not reword the body when you move or edit it.** It is a legal statement. Change it deliberately, in
`lib/binacle-robots/robots.txt`, and know that the change reaches every site at once - which is the whole
reason it is here rather than in three files.

**The content signals are commented out.** `# Content-Signals: ai-train=no, search=yes, ai-input=no` sits
behind a `#`. Whether it should be live is a decision, not a typo, and a spec pins the current answer so
that turning it on has to be a choice someone makes.

**Nothing substitutes into the text.** It is read from disk once at load and written out unchanged; there is
no Liquid in it and none is rendered.
