# design

The look every Binacle surface shares: colours, type, spacing, page blocks and components, written as Sass
partials, plus the fonts and the mark. A surface builds its own CSS from the partials it needs. Nothing here is
built on its own.

## 📂 What is in it

| Path | What it is |
|---|---|
| `abstracts/` | Sass that outputs no CSS: the two breakpoints, and mixins for the deep ground, the strong line, and the panel and drawer |
| `base/` | the custom properties for both themes, the bare elements, and the type styles that go on any element |
| `layout/` | the blocks that frame a page: the page width, header, footer, bands, hero, split, columns, the tool page, and the docs page with its sidebar and outline |
| `components/` | everything that sits inside a page: buttons, code, tables, the drawing, the 3D view, forms, callouts and the rest |
| `fonts/` | Familjen Grotesk and Martian Mono, one Latin subset file each, with their licences |
| `mark.svg`, `mark-small.svg` | the logo, and the small mark for icons |

## 🧱 How a partial is written

**One partial per block, named after it.** `.drawer` is in `components/_drawer.scss`.

**A rule goes in the file of the last block its selector names.** `.doc__body .table` is in `_table.scss`, so
everything that can change a table is in one place. The page width, `container`, and the type styles - `label`,
`prose`, `mono` - do not count: `.site-header .container` is in `_site-header.scss`.

**What is nested, and what is flat.** A block's modifiers, its `is-` states, its `:hover` and `:focus`, its
media queries and the bare HTML elements inside it are nested in the block. Each `__element` starts its own
rule at the top of the file, so a search for its name finds it. A rule that reaches into another block is flat.

**A state a script sets is an `is-` class**, always written beside its block: `.drawer.is-open`, never
`.is-open` alone.

**Media queries use range syntax and the two breakpoints:** `(width >= tiers.$desktop)`. The other widths -
1000, 1100, 1200 - each change one part of one page, so they are written as numbers where they are used.

## 🚀 How you use it

A surface points the `sass` command at this folder and names the partials it uses in its own `main.scss`:

```scss
@use "base/tokens" with ($font-path: "/fonts/");
@use "base/elements";
@use "layout/band";
@use "components/button";
@use "base/typography";
```

```bash
sass --load-path=../../design _sass/main.scss css/main.css
```

A partial named twice is built once.

## ⚠️ What will bite you

**The fonts are not served from here.** A font URL resolves from the built CSS, so each surface copies
`fonts/` next to its CSS and passes that path as `$font-path`.

**Load `base/tokens` first when you pass `with`.** Sass only takes settings on a module's first load. If
another partial loaded it earlier, the build stops with *already loaded, so it can't be configured*.

**Inside a file, order matters.** A later rule beats an earlier one of the same weight. `_drawing.scss` and
`_form.scss` depend on it; moving a rule there can change the look.

**Load `base/typography` last.** A type style shares its element with a block - `band__body prose` - and both
set `gap`. Written on purpose, the type style has to win, so it comes after every block.

**Between the other files order does not matter, today.** No two blocks set the same property at the same
weight on the same element, so their `@use` lines can come in any order. A rule placed in the wrong file is how
that breaks.
