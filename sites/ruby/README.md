# Site gems

The Jekyll plugins only the Binacle sites read. They carry the sites' own words and markup, so no other site
could load them. The generic ones, which any Jekyll site could use, are in [`ruby/`](../../ruby).

Each site loads them from its own `Gemfile` as a local path dependency, inside `group :jekyll_plugins`, and
names them under `plugins:` in its `_config.yml`. Drop either line and the plugin does not load. `binacle-sites-core` has no plugin, so it goes in the `Gemfile` outside
the group and is not named under `plugins:`.

## 📦 Gems

| Gem | What it is | Loaded by |
|---|---|---|
| [`binacle-docs`](binacle-docs) | Everything Ruby that only `docs` uses: the version scheme - a generator that stamps `title_suffix` and `robots` onto every versioned document, and `{% vlink %}` - and the `doc_body` and `doc_sections` filters that turn a rendered page into the docs' markup. It uses `binacle-sites-core` for the tiles, the code block wrapper and the table base | `docs` |
| [`binacle-robots`](binacle-robots) | The body of `robots.txt`, held once instead of three times. `{% robots %}` writes the content-signal preamble, the rights reservation and `User-Agent: *`; the `Sitemap:` lines come from `{% sitemap_links %}` | `www`, `docs`, `demo` |
| [`binacle-sites-core`](binacle-sites-core) | The markup jobs the site gems write the same way, as plain Ruby they call: link tiles, code blocks, tables and a few smaller jobs. It has no filter or tag of its own | `www`, `docs` |
| [`binacle-www`](binacle-www) | The `www_sections` filter, which turns a markdown page into the design's sections. It uses `binacle-sites-core` for the shared markup | `www` |

Each gem has the shape `bundle gem` writes: a `<name>.gemspec`, one entry file at `lib/<name>.rb`, the rest
under `lib/<name>/`, and the specs in `spec/`. All are MIT.

## 🧪 Tests

The gems share one bundle, `sites/ruby/Gemfile`, apart from the one in `ruby/`. Install it once:

```bash
cd sites/ruby && bundle install
```

Then run a gem's specs from its own folder, or from the repo root:

```bash
cd sites/ruby/binacle-docs && bundle exec rspec
just test rb_binacle-docs_unit
just test rb_binacle-robots_unit
just test rb_binacle-www_unit
```

`.rubocop.yml` takes the rules from `ruby/.rubocop.yml`. Nothing runs it; run it by hand from here with
`bundle exec rubocop`.

Adding a gem means one `gemspec path:` line in `sites/ruby/Gemfile` and one recipe in the test module.

## ⚠️ What will bite you

**`doc_body` and `www_sections` write the design system's class names.** Rename a class in the Sass and these gems
have to change with it, or the site loses that style without any error. The section looks are the exception: a spec
in `binacle-www` checks them against the Sass.

**The bundle has its own lock.** Raise Jekyll in `ruby/Gemfile.lock` and raise it here too, or the two sets of
gems are tested against different versions.
