# Binacle.Net Docs

The documentation site, built with [Jekyll](https://jekyllrb.com/). It hosts the versioned API reference
(with an embedded Swagger UI) and the guides. Published to <https://docs.binacle.net>.

## 📂 What is in it

| Path | What it is |
|---|---|
| `collections/_versions/` | The versioned documentation, one folder per line. A closed line's folder is frozen |
| `collections/_common_pages/` | One page: the version list at `/version/`. Every other page belongs to a line |
| `pages/` | `404.html` and `robots.txt`. The landing page is the current line's index |
| `_data/` | Site data. `versions.yml` is the version list: which one is current, where each renders, what docker tag each pulls |
| `_redirects` | Old urls and where they went, one per line. Cloudflare reads it from the output root |
| `_layouts/`, `_includes/`, `_sass/`, `css/` | Templates, partials and styles |
| `_js/` | TypeScript and JavaScript sources. Webpack bundles them into `js/` |

`js/`, `lib/`, `media/` and the favicons at the root are **generated or copied in** and gitignored - webpack
writes the first, `just assets` writes the rest. Nothing there is edited by hand.

## 🚀 Develop

From the repo root:

```bash
just serve docs
```

That runs `jekyll serve` and the webpack watch together in one terminal, and one Ctrl-C stops both. **Use it
rather than `jekyll serve` on its own** - Jekyll alone does not rebuild the TypeScript or SCSS under `_js/`, so
a script or style change appears to do nothing.

Run `just install` once on a fresh clone, and `just assets` after changing anything under the repo-root
[`assets/`](../../assets) folder - the site serves its own copy, so a new logo does not show up until that runs.

## 🔢 Adding a version

`_data/versions.yml` is the single source of truth for the list, newest first - Jekyll's own ordering sorts by
path and would put `v3.10.x` before `v3.2.x`. Opening a new line takes two steps, and the file says so at the
top: add the folder to `list` with its four keys, and point `current` at it. The gem reads the version off the
folder name; no folder name appears in a url - a line renders under its `url_segment` and is called by its
`label`, both the highest version it shipped, and pulls with its `version_tag`.

**A folder that is not in the list, or an entry missing a key, fails the build** - a page would print a pull
command with no tag, or a selector row with no name.

## ⚙️ Two things that bite

- **`{% vlink %}`, not `{% link %}`**, for anything inside a version. It joins the path onto the page's own
  version, so the same guide can link its neighbour without naming a version number. The tag comes from the
  `binacle-docs-versions` gem in [`ruby/`](../../ruby), with the generator that stamps the version keys.
- **`pages/404.html` carries `permalink: /404.html`.** Cloudflare is configured with `not_found_handling:
  "404-page"`, which looks for that exact name in the site root.

Custom Liquid filters and the Google Tag Manager tags come from the local gems in [`ruby/`](../../ruby),
wired up through this site's `Gemfile`.
