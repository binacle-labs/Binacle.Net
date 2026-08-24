# jekyll-webmanifest

Writes a site's web app manifest - the JSON file that lets a browser install the site to a home screen -
from one config block. It replaces the hand-written `site.webmanifest` page that a site otherwise keeps in
its source, with its own front matter and its own Liquid.

The gem owns the whole file, so the site keeps nothing.

## 📂 What is in it

| Surface | Writes | For |
|---|---|---|
| the generator | the whole manifest file | every site |
| `{% webmanifest_link %}` | `<link rel="manifest" href="...">` | a site with no head data list of its own |

**A generator, not a tag, and that is the design point.** Every value in a manifest is config, `site.title`
or `site.description` - it needs nothing another plugin worked out. A plugin that did need another
generator's output could not be a generator itself, because Jekyll puts no order on two generators of equal
priority; this one has nothing to wait for, so it can own the file outright.

## 🚀 Quick start

Both lines are needed. Miss the second and nothing loads, no manifest is written, and nothing fails.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-webmanifest", path: "path/to/jekyll-webmanifest"
```

```yaml
# _config.yml
plugins:
  - jekyll-webmanifest
```

Then one block. Every key in it has a default, so this is already a working manifest:

```yaml
webmanifest:
  theme_color: "#101010"
  background_color: "#101010"
```

That writes `/site.webmanifest`, named and described from the site.

## ⚙️ Configuration

```yaml
webmanifest:
  path: /site.webmanifest    # where the file goes
  name: My Site              # default: site.title
  description: What it is    # default: site.description
  start_url: /               # default: "/", through baseurl
  display: standalone        # default: standalone
  theme_color: "#101010"     # no default
  background_color: "#101010"  # no default
  icons:
    - { src: /android-chrome-192x192.png, sizes: 192x192, type: image/png }
```

`path` - a file, not a directory. Setting it moves the file and moves the published url with it.

`name` and `description` fall back to `title` and `description` in `_config.yml`. A value that is empty
after trimming writes no key rather than an empty one.

`theme_color` and `background_color` have no default. They are your colours and there is no sensible guess -
leave them out and the manifest simply has no such key. **They cannot be read from your stylesheet.** A
manifest is JSON served to a browser, so the two hexes exist in both places and drift silently. Change a
theme colour and change this block in the same commit.

`icons` - a list of maps, written out as given. `src` goes through `baseurl` unless it already has a scheme;
every other key is passed through in the order you wrote it. `icons: []` writes no `icons` key.

**There is no `short_name` key and no default for one.** Android truncates a home-screen label at about
twelve characters, and shortening your name is a decision about your name. Write `short_name:` in the block
and it goes out like any other key.

## 🧾 Any key you like

**Every key the gem does not know is written into the JSON untouched.** `orientation`, `categories`,
`shortcuts`, `screenshots`, `scope`, `id`, anything the specification grows next year:

```yaml
webmanifest:
  theme_color: "#101010"
  orientation: portrait
  categories: [utilities, productivity]
  shortcuts:
    - name: Second page
      url: /second/
```

The keys it does know come first, in the order above; the rest follow in the order the config declares them.
Only `path` and `url` are held back - `path` says where the file goes and `url` is what the gem publishes.

The JSON is built by Ruby's JSON library, so a quote or a backslash in a name is escaped rather than
breaking the file.

## 🏷️ The tag

After the generator runs, the built path is readable as `site.webmanifest.url`, carrying your `baseurl`.

`{% webmanifest_link %}` writes the head element from that key:

```liquid
<head>
  {% webmanifest_link %}
</head>
```

```html
<link rel="manifest" href="/site.webmanifest">
```

A site that already renders its head links from a data list does not need the tag - point one entry at
`/site.webmanifest` and leave it out. With no `webmanifest:` block the tag renders an empty string.

## ⚠️ Gotchas

**A beautifier will reformat it, and a recursive glob will not stop one.** The generator makes the page in
memory, so its path is the bare filename with no directory in front of it - `jekyll-tidy`'s
`File.fnmatch("**/*.webmanifest", "site.webmanifest")` is false. Exclude it as `*.webmanifest`, the way a
site already excludes a generated `*.xml` sitemap. Left running, a beautifier strips the indentation and the
trailing newline; the JSON stays valid and stops being readable.

- **No `webmanifest:` block means no file at all.** That is the off switch, and it is why an empty file is
  never written.
- **A path something else already writes fails the build.** Moving a site off a hand-written manifest means
  deleting that page in the same change as adding this block, or the two fight over one url.
- The generator runs at Jekyll's lowest priority, so the collision check above sees pages other plugins
  created. It reads nothing from them.
- **Renaming the file changes a live url.** A browser that already installed the site fetches the path it
  was given.

## 🧪 Tests

```bash
bundle exec rspec
```

The specs build a real Jekyll site from `spec/fixtures/site` and read the files it writes.
