# jekyll-filters

Three Liquid filters for a Jekyll site: one that turns page content into a plain string you can put in a
`<meta>` tag, one that title-cases a phrase, and one that writes the build year into a copyright line.

There is nothing to configure. Each filter takes its input from the call.

## 🚀 Quick start

Add the gem, then list it under `plugins:`. Both lines are needed - miss the second and Liquid renders
nothing where the value should be.

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
  `{now}` does not suit you.

Write the placeholder into your data and let the filter fill it in:

```yaml
# _data/footer.yml
copyright: "(c) 2023-{now} Your Name"
```

## ⚠️ Gotchas

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

## 🧪 Tests

```bash
bundle exec rspec
```
