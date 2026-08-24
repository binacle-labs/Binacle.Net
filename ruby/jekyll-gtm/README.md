# jekyll-gtm

Two Liquid tags that write the Google Tag Manager snippets, so the vendor JavaScript is not sitting in your
layouts.

## 🚀 Quick start

Add the gem, list it under `plugins:`, and put your container id in the config. Both lines are needed - miss
the `plugins:` one and Jekyll prints the tag name as text.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-gtm", path: "path/to/jekyll-gtm"
```

```yaml
# _config.yml
plugins:
  - jekyll-gtm

gtm: GTM-XXXXXXX
```

```liquid
<head>
  {% gtm_head site.gtm %}
</head>
<body>
  {% gtm_body site.gtm %}
```

`gtm:` is your key, not the gem's. Name it whatever you like and hand it to the tag.

## 🏷️ The two tags

- `{% gtm_head <id> %}` - the GTM `<script>` block. Put it as high in `<head>` as you can.
- `{% gtm_body <id> %}` - the `<noscript>` iframe. Put it straight after `<body>`.

Both are needed for a working install. The tags do not check that you used both.

Each one looks its argument up as a Liquid variable first. If nothing is found, the argument is used as the
id itself, but only when it has the shape of one - `GTM-` and then letters and digits. So
`{% gtm_head GTM-XXXXXXX %}` works, and a variable name that resolves to nothing writes nothing.

To turn tracking off, set the id to an empty string. The tags then write nothing at all - no comment, no
script, no iframe.

## ⚠️ Gotchas

- Misspell the variable and you get an empty page section, with no warning. `{% gtm_head site.gtn %}` finds
  nothing, does not have the shape of an id, and renders nothing. Check the built page for the snippet
  after you first wire GTM up.
- The id goes into the snippet as it stands. It is not escaped, and the shape check is the only validation
  there is - a real container id that Google does not know still writes a live script.
- The gem does not decide whether to load GTM. Consent and environment checks go in the `{% if %}` around
  the tag.

## 🧪 Tests

```bash
bundle exec rspec
```
