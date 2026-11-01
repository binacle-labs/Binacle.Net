# design

The look every Binacle surface shares: colours, type, spacing, page blocks and components, written as Sass
partials, plus the fonts and the mark. A surface builds its own CSS from the partials it needs. Nothing here is
built on its own.

## 📂 What is in it

| Path | What it is |
|---|---|
| `abstracts/` | Sass that outputs no CSS: the tier boundaries, and mixins for the deep ground, the strong line, and the panel and drawer |
| `base/` | the custom properties for both themes, and the bare elements |
| `layout/` | the blocks that frame a page: the page width, header, footer, bands, hero, split, columns, the tool page, and the docs page with its sidebar and outline |
| `components/` | everything that sits inside a page: buttons, code, tables, the drawing, the 3D view, forms, callouts and the rest |
| `utilities/` | the type styles - `label`, `muted`, `prose` and the rest - that go on any element, beside a block's class |
| `fonts/` | Familjen Grotesk and Martian Mono, one Latin subset file each, with their licences |
| `mark.svg`, `mark-small.svg` | the logo, and the small mark for icons |

## 🧱 How a partial is written

**One partial per block, named after it.** `.drawer` is in `components/_drawer.scss`.

**A rule goes in the file of the last block its selector names.** `.doc__body .table` is in `_table.scss`, so
everything that can change a table is in one place. The page width, `container`, and the type styles - `label`,
`prose`, `mono` - do not count: `.site-header .container` is in `_site-header.scss`.

**Every class is written out in full, never glued with `&`,** so a search for its name finds it - the
*Sass Guidelines* rule on selector nesting. `&--primary` is not used; `.button--primary` is.

**What is nested, and what is flat.** A block's `is-` states, its `:hover` and `:focus`, its media queries and
the bare HTML elements inside it are nested in the block. Each modifier and each `__element` is its own rule
after the block, one class, as BEM has it: `.band--tint`, never `.band.band--tint`. A rule that reaches into
another block is flat.

**A state a script sets is an `is-` class**, always written beside its block: `.drawer.is-open`, never
`.is-open` alone.

**Media queries use range syntax and the named boundaries:** `(width >= tiers.$desktop)`, never a number.
`abstracts/_tiers.scss` holds them: `$tablet`, `$desktop`, `$wide`, `$wider`, `$widest`.

## 🔀 Where it leaves a convention

On purpose, each for a reason:

- **The folders are 7-1's; `utilities/` is ITCSS's.** 7-1 puts the type styles in `base/`, which loads early.
  They share elements with blocks and have to win, so they sit in a last layer, as ITCSS has it.
- **States are `is-` classes, as in SMACSS and BEMIT; nothing else takes a prefix.** BEM writes a state as a
  modifier. BEMIT would also mark the type styles as utilities, `u-label`; here they are `label`.
- **Content from markdown is styled by its bare tags and its context.** BEM wants a class on every element and
  a second class where a block sits inside another. Markdown gives a class only where one is written by hand,
  so `.doc__body > h2` styles a heading and `.doc__body .table` a table.

## 🚀 How you use it

A surface points the `sass` command at this folder and names the partials it uses in its own `main.scss`:

```scss
@use "base/tokens" with ($font-path: "/fonts/");
@use "base/elements";
@use "layout/band";
@use "components/button";
@use "utilities/typography";
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

**`_drawing.scss` still writes its modifiers with the block,** `.drawing.drawing--painted`, and so do
`.band__body.band__body--code-left` and the states a script sets as modifiers, `.field.field--invalid`. Their
rules win by that extra class; taking it off changes which rule wins.

**Load `utilities/` last.** A type style shares its element with a block - `band__body prose` - and both set
`gap`. Written on purpose, the type style has to win, so it comes after every block.

**Between the other files order does not matter, today.** No two blocks set the same property at the same
weight on the same element, so their `@use` lines can come in any order. A rule placed in the wrong file is how
that breaks.
