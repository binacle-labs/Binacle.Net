# jekyll-filters

Three Liquid filters for a Jekyll site: one that turns page content into a plain string you can put in a
`<meta>` tag, one that title-cases a phrase, and one that writes the build year into a copyright line.

There is nothing to configure. Each filter takes its input from the call.

## 🚀 Quick start

Add the gem, then list it under `plugins:`. Both lines are needed - miss the second and nothing fails.
Liquid drops a filter it does not know and renders the value unfiltered, so the page ships
`(c) 2023-{now} Your Name` with the placeholder still in it.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-filters", path: "path/to/jekyll-filters"
```

```yaml
# _config.yml
plugins:
  - jekyll-filters
```

```liquid
<meta name="description" content="{{ page.content | clean_content }}">
<h1>{{ page.slug | capitalize_all }}</h1>
<p>{{ site.data.footer.copyright | expand_year }}</p>
```

## 🔤 The filters

- `clean_content` - removes HTML tags, turns newlines and runs of spaces into single spaces, trims the ends,
  then cuts to 160 characters. Pass a length to change the cut: `clean_content: 200`.
- `capitalize_all` - capitalizes every word.
- `expand_year` - replaces `{now}` with the year of the build. Pass your own placeholder as an argument if
  `{now}` does not suit you, but keep `}` out of it - see the gotchas.
- `doc_body` - turns a rendered docs page into the design's markup: drops the emoji that opens a heading, a
  bullet or a paragraph, makes callouts and code blocks, and puts a class on each block. Heading ids stay as
  kramdown made them. A code block with `data-call` on it, `{: data-call="POST /path" }` or `{: data-call="200" }`
  in the markdown, shows that in its head in place of the language. `data-name` shows a file or field name there
  instead, and `data-wrap=""` wraps one long string. In a `}` inside an attribute, write `\}`. A table cell that
  holds only code marked `{:.mono}` takes the class `mono` too.
- `doc_sections` - lists a page's `h2`s as `id` and `title`, for an outline. Give it `doc_body`'s result, so the
  titles carry no emoji.

Write the placeholder into your data and let the filter fill it in:

```yaml
# _data/footer.yml
copyright: "(c) 2023-{now} Your Name"
```

## ⚠️ What will bite you

- `clean_content` cuts characters, not words. It can stop mid-word and it adds no ellipsis.
- It removes tags with a regular expression, not a parser. Anything between a `<` and the next `>` goes, so
  prose that compares two things - `a < b > c` - comes out as `a c`.
- It does not decode entities. `&amp;` stays `&amp;`, which is right inside an attribute and wrong once you
  start counting characters.
- `capitalize_all` lowercases the rest of each word, so `API` becomes `Api`. Use it on slugs and path
  segments, not on a title someone wrote.
- All three return an empty string for nil, so a missing front matter key never fails a build.
- `expand_year` reads `site.time`, so every page of one build carries the same year even if the build runs
  over midnight. Called outside Jekyll it falls back to the clock.
- **A placeholder holding a `}` cannot be passed inside `{{ }}`.** `{{ x | expand_year: "{now}" }}` fails the
  build with `Variable was not properly terminated` - Liquid stops the output tag at the first `}` it meets,
  before the filter is ever reached. This is why `{now}` is the default: leave the argument off and the
  placeholder never appears in a template. If you must name it, do it in an `assign`, where the rule
  does not apply.

## 🧪 Tests

```bash
bundle exec rspec
```
