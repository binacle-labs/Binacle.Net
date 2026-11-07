# design-system

`@binacle/design-system`. A site component's three parts in one folder: its Sass, its behaviour and its shared
markup. Today it holds the copy button; the rest of the look is still in [`design/`](../../design).
`www` takes the copy button from here; `docs` and `demo` still reach it through `design/` and
[`binacle-components`](../binacle-components), which forward to this package.

## 📂 What is in it

| Path | What it is |
|---|---|
| `components/copy-button/` | `_index.scss`, the look, and `copyButton.ts`, which shows the buttons and makes them work |
| `_includes/components/copy-button.html` | The button's markup, for the Jekyll sites |
| `index.ts` | Re-exports every init function |
| `index.scss` | Forwards every part, for a surface that wants all of them |
| `binacle-design-system.gemspec` | Makes the folder a Jekyll theme gem, so a site can read `_includes/` |
| `tests/` | The jest tests, in the same folders as the code |

## 🚀 How you use it

The Sass, through the `pkg:` importer. The site's `sass` command needs `--pkg-importer=node`:

```scss
@use "pkg:@binacle/design-system/components/copy-button";
```

The behaviour, from the package root:

```ts
import {initCopyButton} from "@binacle/design-system";
```

The markup, once the site's `Gemfile` names the gem by path and its `_config.yml` sets
`theme: binacle-design-system`:

```liquid
{% include components/copy-button.html class="code-block__copy" %}
{% include components/copy-button.html copy="run-text" %}
```

From the repo root:

```
just test ts_binacle-design-system_unit
```

## ⚠️ What will bite you

**Import the TypeScript from the root, not a subpath.** The sites' `tsconfig.json` uses the older module
resolution, which ignores `exports` in `package.json`, so `@binacle/design-system/components/copy-button`
does not type-check there. Webpack would find it; ts-loader stops the build first.

**The gem ships `_includes/` only.** Jekyll loads plugins only from gems a theme names as dependencies, so Ruby
code does not belong here.

**A site's own include of the same name wins** over the theme's.
