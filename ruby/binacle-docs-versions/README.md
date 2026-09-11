# binacle-docs-versions

One site's gem, on purpose. It carries the version scheme of `sites/docs`: the front matter key `version`,
the data path `versions.current`, and the collection folder `_versions`. Nothing here is portable, and
nothing here should be made portable. The moment these become settings, every site loading the gem carries a
vocabulary only one of them uses.

Two pieces, both keyed on the version the page belongs to.

## 📂 What is in it

| Surface | Does |
|---|---|
| the generator | decides the url of every file under `_versions/`; stamps `version`, `version_tag`, `title_suffix`, `robots` and `version_urls`; fails the build on two files at one url; prints the pages the previous version had that the current one lacks |
| `{% vlink /path %}` | links to a file inside the current page's version, or inside the version named first |

## 🚀 Quick start

Both lines are needed. Miss the second and the build fails on the first `{% vlink %}` - Liquid raises
`Unknown tag 'vlink'`. The generator just never runs, so the stamps go missing without a word.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "binacle-docs-versions", path: "../../ruby/binacle-docs-versions"
```

```yaml
# _config.yml
plugins:
  - binacle-docs-versions
```

One thing has to be true of the site - every folder under `_versions/` is in the list, and every entry carries
all four keys. Nothing is derived; a missing one stops the build.

```yaml
# _data/versions.yml
current: v3.x
list:
  - id: v3.x            # the folder under _versions/ - never in a url
    url_segment: 3.0.0  # where it renders, /version/<url_segment>/
    label: v3.0.0       # what the selector and every page call it
    version_tag: "3.0"  # what docker pulls
  - id: v2.x
    url_segment: 2.1.1
    label: v2.1.1
    version_tag: "2.1.1"
```

## 🌐 The url

Nothing in a folder says where it renders, and a `permalink` a page writes is overwritten. Every file under
`_versions/<folder>/` renders at `/version/<url_segment>/<rest>` - a page as a folder with an index inside
(`api/v3.md` → `/version/2.1.1/api/v3/`, `index.md` → `/version/2.1.1/`), a static file under its own name
(`swagger/v3.json` → `/version/2.1.1/swagger/v3.json`). Static files get this through `VersionedFile`, a
`StaticFile` whose url can be set - Jekyll's own reads the collection template and ignores data.

## 🏷️ The stamps

On every document under `_versions/<folder>/`:

- `version` - the folder name. Nothing in the folder and no config block has to say it.
- `version_tag` - the tag listed beside that id in `_data/versions.yml`, for the pull commands on the page.
  A folder with no tag in the list stops the build: the page would print a pull command with nothing after
  the colon.
- `version_label` - the list entry's `label`, what the page calls its own version.
- `title_suffix` - `(v2.1.1)`, from the label, for whatever writes the page title.
- `version_urls` - a map from every version to the url of this same page in that version, or to that
  version's index where the page does not exist. For a version selector that lands on the same page.
- `robots` - `noindex, follow` on every version that is not `current`.

On every entry of the `list` in `_data/versions.yml`:

- `url` - where that version's index renders, so a template lists versions without building a url from an id.

On every page whose layout is `redirect`:

- `redirect_to` - the url of the current version's index, for the layout to send a reader to.
- `canonical` - the same url, so a crawler is told the redirect and its target are one destination.
- `robots` - `noindex`.

**A redirect page's canonical points at the page it redirects to, not at itself.** That is the whole
purpose of the page, and it is why the value cannot live in front matter: it moves when `current` moves.
Both keys carry one computed url, so the head and the redirect can never disagree.

**The current version has to have an index for a redirect to point at.** Where it does not and a redirect
page exists, the build stops.

`current` is the one knob. It names the version search engines may index; move it at release time and
nothing else changes. A document with no `version` is left alone, and so is a site with no versioned
documents at all.

**`current` has to name a version the site has.** Where it does not, the build stops and names both the
value it was given and the versions it found. A `current` nobody notices is wrong would put `noindex` on
every page of the site while the sitemap still lists them.

## 🛑 The collision check

Two files rendering at one url is a Jekyll warning, and a warning is how the wrong page ships. Here it stops the
build and names both files. It covers every page and document that writes output, so a root page claiming a
versioned url fails, and so would two pages in one version.

## 🗑️ The removed-page list

At build, the generator prints every page and file in the previous version that has no counterpart at the
same path in the current one. The previous version is the entry listed right after `current` in
`_data/versions.yml`. It is printed and written nowhere: it is the redirect list whoever opens a new major
has to write.

No key knows why it is set. Whatever renders them reads ordinary values and needs to know nothing about
versions.

## 🔗 The tag

```liquid
{% vlink /swagger/v3.json %}
{% vlink /swagger/{{ page.swagger }}.json %}
{% vlink v2.x /configuration/service-module/index.md %}
```

The path is resolved inside `_versions/<the page's version>/`, against documents and static files alike, and
comes back as a url. A first word that is a version id in `versions.yml` picks that version's folder instead -
the way a page links its counterpart in another line without knowing where that line renders. Liquid inside
the argument is rendered first, so a page can build the path from its own front matter. A path that resolves
to nothing fails the build rather than writing a link to a 404.

## ⚠️ Gotchas

- The generator never overwrites a key the page already set. That is how a swagger page keeps
  `noindex, nofollow` while the rest of its version is `noindex, follow`.
- Only `title_suffix` is order-sensitive. It runs at Jekyll's `:high` priority because a generator resolves
  the title from it; resolve the title first and the suffix is silently missing. `robots` is read at render,
  so nothing that reads it can run too early.
- A generator runs once, before render. A document another plugin creates later is not stamped.
- `{% vlink %}` walks every file in the site per call. Fine at this size; it is a linear scan.

## 🧪 Tests

```bash
bundle exec rspec
```

The specs build two real Jekyll sites from `spec/fixtures` - one with two versions, one whose only page
links at a file that is not there.
