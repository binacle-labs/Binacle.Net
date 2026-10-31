# design

The look every Binacle surface shares: colours, type, spacing and page blocks, written as Sass partials, plus
the fonts and the mark. A surface builds its own CSS from the partials it needs. Nothing here is built on its
own.

## 📂 What is in it

| Path | What it is |
|---|---|
| `abstracts/` | Sass that outputs no CSS: the two breakpoints, shared mixins |
| `base/` | the custom properties for both themes, the bare elements, and the type styles that go on any element |
| `layout/` | the blocks that frame a page: the page width, bands, the footer |
| `fonts/` | Familjen Grotesk and Martian Mono, one Latin subset file each, with their licences |
| `mark.svg`, `mark-small.svg` | the logo, and the small mark for icons |

One partial per block, named after it. A block's elements and modifiers live in its file. A rule that reaches
into another block - `.band .drawing` - goes in the file of the block it styles.

## 🚀 How you use it

A surface points the `sass` command at this folder and names the partials it uses in its own `main.scss`:

```scss
@use "base/tokens" with ($font-path: "/fonts/");
@use "base/elements";
@use "layout/band";
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

**The order of `@use` lines is the order of the CSS.** A later rule beats an earlier one of the same weight, so
moving a line can change the look.
