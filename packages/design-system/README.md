# design-system

`@binacle/design-system`. The look every Binacle site shares: colours, type, spacing, the page blocks and the
components, plus the fonts and the mark. Each block keeps its Sass and its behaviour in one folder, and its shared
markup in `_includes/`. A site builds its own CSS from the parts it needs. Nothing here is built on its own.

`www` reads it by `pkg:` and by import. `docs` and `demo` still pass this folder as a Sass load path and import
the behaviour through [`binacle-components`](../binacle-components), which forwards here.

## 📂 What is in it

| Path | What it is |
|---|---|
| `abstracts/` | Sass that outputs no CSS: the tier boundaries, and mixins for the deep ground, the strong line, and the panel and drawer |
| `base/` | the custom properties for both themes, and the bare elements |
| `layout/` | one folder per block that frames a page: the page width, header, footer, bands, hero, split, columns, the tool page, and the docs page with its sidebar and outline |
| `components/` | one folder per block that sits inside a page: buttons, code, tables, the drawing, the 3D view, forms, callouts and the rest |
| `utilities/` | the type styles - `label`, `muted`, `prose` and the rest - that go on any element, beside a block's class |
| `fonts/` | Familjen Grotesk and Martian Mono, one Latin subset file each, with their licences |
| `mark/` | the logo, and the small mark for icons |
| `_includes/components/` | a component's shared markup, for the Jekyll sites |
| `index.scss` | forwards every part that writes CSS, for a surface that wants all of them |
| `index.ts` | re-exports every init function |
| `binacle-design-system.gemspec` | makes the folder a Jekyll theme gem, so a site can read `_includes/` |
| `tests/` | the jest tests, in the same folders as the code |

A block's folder holds `_index.scss`, its look, and a `.ts` file when it has behaviour: `components/drawer/` holds
`_index.scss` and `drawer.ts`. The theme button's `themeInit.ts` is the script that sets the theme before the first
paint.

## 📂 Why it is not a top-level folder

**It is one package that several surfaces read** - the three sites and the UI module - so it sits with the other
packages, as monorepo tools have it: Turborepo's starter keeps its shared UI in `packages/ui`. A top-level folder
is for an area with several parts of its own, as `vipaq/` is.

**It is close.** It is an npm package and a Jekyll theme at once, and it carries the fonts and the mark. It gets a
top-level folder of its own if it grows past one package - the tokens, the icons and the components apart, or a
Razor library for the UI module - or if it is ever published on its own.

## 🧱 How a partial is written

**One folder per block, named after it.** `.drawer` is in `components/drawer/_index.scss`.

**A rule goes in the file of the last block its selector names.** `.doc > .callout` is in `callout/`, so
everything that can change a callout is in one place. A second class counts the same: `.doc__table` is in
`table/`. The page width, `container`, and the type styles - `label`, `prose`, `mono` - do not count:
`.site-header .container` is in `site-header/`.

**Every class is written out in full, never glued with `&`,** so a search for its name finds it - the
*Sass Guidelines* rule on selector nesting. `&--primary` is not used; `.button--primary` is.

**What is nested, and what is flat.** A block's `is-` states, its `:hover` and `:focus`, its media queries and
the bare HTML elements inside it are nested in the block. Each modifier and each `__element` is its own rule
after the block, one class, as BEM has it: `.band--tint`, never `.band.band--tint`. A rule that reaches into
another block is flat.

**A block inside another block.** If it changes what the block sets, or reaches its elements, it is a modifier
in the block's own file: `.close-button--bin`. If it only places the block, it is an element of the parent,
mixed on: `class="copy-button command__copy"`. Two blocks on one element never set the same property.

**A state a script sets is an `is-` class**, always written beside its block: `.drawer.is-open`, never
`.is-open` alone.

**Media queries use range syntax and the named boundaries:** `(width >= tiers.$desktop)`, never a number.
`abstracts/_tiers.scss` holds them: `$tablet`, `$desktop`, `$wide`, `$wider`, `$widest`.

## 🔀 Where it leaves a convention

On purpose, each for a reason:

- **The groups are 7-1's; `utilities/` is ITCSS's.** 7-1 puts the type styles in `base/`, which loads early.
  They share elements with blocks and have to win, so they sit in a last layer, as ITCSS has it.
- **States are `is-` classes, as in SMACSS and BEMIT; nothing else takes a prefix.** BEM writes a state as a
  modifier. BEMIT would also mark the type styles as utilities, `u-label`; here they are `label`.

## 🚀 How you use it

The Sass, through the `pkg:` importer. The site's `sass` command needs `--pkg-importer=node`. A site names the
parts it uses in its own `main.scss`:

```scss
@use "pkg:@binacle/design-system/base/tokens" with ($font-path: "/fonts/");
@use "pkg:@binacle/design-system/base/elements";
@use "pkg:@binacle/design-system/layout/band";
@use "pkg:@binacle/design-system/components/button";
@use "pkg:@binacle/design-system/utilities/typography";
```

A part named twice is built once.

The behaviour, from the package root. The pre-paint script has its own path, so its bundle holds nothing else:

```ts
import {initCopyButton, initDrawer, initThemeButton} from "@binacle/design-system";
import {initTheme} from "@binacle/design-system/theme-init";
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

**The fonts are not served from here.** A font URL resolves from the built CSS, so each site copies `fonts/` next
to its CSS and passes that path as `$font-path`.

**Load `base/tokens` first when you pass `with`.** Sass only takes settings on a module's first load. If
another partial loaded it earlier, the build stops with *already loaded, so it can't be configured*.

**Load `utilities/` last.** A type style shares its element with a block - `band__body prose` - and both set
`gap`. Written on purpose, the type style has to win, so it comes after every block.

**Inside a file, order matters.** A later rule beats an earlier one of the same weight. `drawing/` and `form/`
depend on it; moving a rule there can change the look.

**`drawing/` still writes its modifiers with the block,** `.drawing.drawing--painted`, and so does
`.band__body.band__body--code-left`. Their rules win by that extra class; taking it off changes which rule wins.
`.split .split__title` has one for the same reason: it has to beat `display-1`, which loads later.

**Between the other files order does not matter, today.** No two blocks set the same property at the same
weight on the same element, so their `@use` lines can come in any order. A rule placed in the wrong file is how
that breaks.

**A subpath import type-checks only with the newer module resolution.** The sites' `tsconfig.json` uses the
older one, which ignores `exports` in `package.json`. So a site imports the behaviour from the root, and only the
pre-paint bundle, whose webpack config sets `moduleResolution: bundler`, imports `theme-init`.

**The gem ships `_includes/` only.** Jekyll loads plugins only from gems a theme names as dependencies, so Ruby
code does not belong here.

**A site's own include of the same name wins** over the theme's.
