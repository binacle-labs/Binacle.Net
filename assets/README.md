# Assets

Static files the three sites share - the icons at the root, the third-party libraries they load, and the
logos. Edit the originals here; the copies in the sites are overwritten on every build.

## 📂 What is in it

| Path | What it is |
|---|---|
| `*.png`, `*.ico` | Favicons, the apple touch icon and the android chrome icons. Served from the site root, so the names matter |
| `lib/` | Vendored third-party front-end libraries, checked in rather than installed |
| `media/` | Logos and marks - `logo/` (ours, every size), `github/` and `docker/` (theirs) |
| `assets.proj` | A no-build project so these files show up in the solution. It compiles nothing |

## 📦 The vendored libraries

Downloaded, not installed - nothing in `package.json` pulls them, and nothing rebuilds them. Each folder is
the vendor's own output, dropped in as-is.

| Folder | What the sites use it for | Version | Licence |
|---|---|---|---|
| `lib/beercss/` | The Material-style CSS framework, plus the Material Symbols fonts | `version` file, `3.11.11` | MIT |
| `lib/swagger-ui/` | The Swagger UI bundle the docs site embeds | `5.11.0`, recorded here only | Apache-2.0 |
| `lib/material-dynamic-colors/` | Theme colour generation for beercss | not recorded | Apache-2.0 |

**Every folder carries the upstream `LICENSE`**, and `swagger-ui` its `NOTICE` too. The vendor output arrived
with no copyright header in it - beercss ships none and the swagger-ui bundle has none either - so the file
beside it is the only notice there is. **Do not delete it when upgrading.** `gulpfile.js` has a `licenses`
glob so it travels with the copy into every site and into the UI module's `wwwroot/`, which is how it reaches
the published image.

**To upgrade one, replace the files and update its `version` file.** Only beercss has one - if you upgrade
either of the others, add it, because the table above is then the only record of what shipped.

## 🚀 Copying them into the sites

```bash
just assets                      # after changing anything here
```

That runs the three gulp tasks in the root `gulpfile.js`, which copy every `.js`, `.css`, `.woff2`, image and
icon - and each vendored `LICENSE` - into [`sites/docs/`](../sites/docs), [`sites/demo/`](../sites/demo) and the UI module's `wwwroot/`. It is
also part of `just install`, so a fresh clone gets them without asking.

**Every target gets the same layout - `lib/`, `media/` and the icons at the root.** What differs is only what
each one skips, and `gulpfile.js` holds that in one `IGNORE` block with the measurement behind each line.
Today `lib/swagger-ui/` goes to the docs site alone; nothing else reads it.

The copy is one-way and does not delete. Renaming a file here leaves the old name behind in every target until
someone removes it by hand.

## 📦 The UI module reads this folder too

`api/src/Binacle.Net.UIModule/wwwroot/` is generated in full and gitignored - `just assets` fills its `lib/`
and `media/`, and the module's own build fills `js/` and `css/`. **Nothing under that `wwwroot/` is edited by
hand**; the module's sources are `_sass/` and `_js/` beside it.

So upgrading a library here upgrades the module as well, which was not true before: it used to keep its own
checked-in copy of beercss, and that copy sat three versions behind this one for as long as it existed.
