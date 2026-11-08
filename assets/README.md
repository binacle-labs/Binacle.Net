# Assets

Static files the three sites share - the icons at the root, the third-party libraries they load, and the
logos. Edit the originals here; the copies in the sites are overwritten on every build.

## 📂 What is in it

| Path | What it is |
|---|---|
| `*.png`, `*.ico` | Favicons, the apple touch icon and the android chrome icons. Served from the site root, so the names matter |
| `lib/` | Vendored third-party front-end libraries, checked in rather than installed |
| `media/` | Logos and marks - `logo/` (ours: the PNG sizes and the small SVG; the main mark is in [`shared/packages/design-system/mark`](../shared/packages/design-system/mark)), `github/` and `docker/` (theirs) |
| `assets.proj` | A no-build project so these files show up in the solution. It compiles nothing |

## 📦 The vendored libraries

Downloaded, not installed - nothing in `package.json` pulls them, and nothing rebuilds them. Each folder is
the vendor's own output, dropped in as-is.

| Folder | What the sites use it for | Version | Licence |
|---|---|---|---|
| `lib/swagger-ui/` | The Swagger UI bundle the docs site embeds | `5.34.1`, recorded here only | Apache-2.0 |

**Every folder carries the upstream `LICENSE`**, and `swagger-ui` its `NOTICE` too. The bundle arrived with no
copyright header in it, so the files beside it are the only notice there is. **Do not delete them when
upgrading.** `gulpfile.js` has a `licenses` glob so they travel with the copy.

**To upgrade one, replace the files and update its version in the table above.** The table is the only record
of what shipped.

## 🚀 Copying them into the sites

```bash
just assets                      # after changing anything here
```

That runs every gulp task in the root `gulpfile.js`, which copy every `.js`, `.css`, `.woff2`, image and
icon - and each vendored `LICENSE` - into [`sites/docs/`](../sites/docs) and the UI module's `wwwroot/`, and
only the icons and the logo files they read into [`sites/demo/`](../sites/demo) and [`sites/www/`](../sites/www). It is
also part of `just install`, so a fresh clone gets them without asking. It also copies the mark from the design
system into every target, as `media/logo/binacle-logo.svg`.

**Every target gets the same layout - `lib/`, `media/` and the icons at the root.** What differs is only what
each one skips, and `gulpfile.js` holds that in one `IGNORE` block with the measurement behind each line.
Today `lib/swagger-ui/` goes to the docs site alone; nothing else reads it.

The copy is one-way and does not delete. Renaming a file here leaves the old name behind in every target until
someone removes it by hand.

## 📦 The UI module reads this folder too

`api/src/Binacle.Net.UIModule/wwwroot/` is generated in full and gitignored - `just assets` fills its tab icon,
`fonts/` and the mark in `media/`, and the module's own build fills `js/` and `css/`. **Nothing under that `wwwroot/` is edited by
hand**; the module's sources are `_sass/` and `_js/` beside it.
