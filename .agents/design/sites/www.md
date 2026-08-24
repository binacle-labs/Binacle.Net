---
id: sites/www-design
description: Why the www site's templates are shaped the way they are - the traps that bite silently, and the constraints a rewrite would break without noticing.
verified: 2026-08-24
check: W1 against the assign in _includes/footer.html; W2 against the theme-init script tag in _layouts/default.html being in head with no defer or async, and the second webpack config in webpack.config.js; W3 against _sass/main.scss and the build:css script in package.json; W5 against exchange.html still being an include and every page calling it
paths:
  - "sites/www/**"
---

# The www site - why the templates are shaped this way

**These were Liquid comments in the templates until 23 Aug 2026.** They are here because a comment in a
published site is a comment in a public repository.

## W1 - the `{now}` replace has to be an assign

`_includes/footer.html` does its `replace` in an `assign` tag, never inside the output tag. **Liquid's
variable parser trips over the closing brace of the `{now}` placeholder when it sits inside `{{ }}`.** It
fails at build, not at render, and the message does not name the placeholder.

## W2 - the theme read is a separate bundle, loaded blocking in `<head>`

`js/theme-init.js` runs before first paint. **A deferred or module script cannot do this** - it runs after
the first paint, so a reader who chose light gets a dark flash on every navigation. It is `<script src>`
with no `defer` and no `async` on purpose.

**It cannot touch `document.body`** - the parser has not reached it - which is why the theme is `data-theme`
on `<html>` and not a class on `<body>`, on this site and on the other three.

**It is a second webpack config, not a second entry.** The main config splits every entry into a runtime
chunk and a vendors chunk, and a head script has to be one self-contained file. Both write into `js/`, so
the main config's `clean` keeps `theme-init.js` - without that the two race and the file is deleted after
it is written.

It overrides ts-loader to `module: 'esnext'` for this bundle alone - the tsconfigs emit commonjs, which
webpack can neither tree-shake nor concatenate. Without that the bundle is twice the size and the package
needs a second entry point to keep the switcher element out of it. **1.21 KiB minified, 587 gzipped.**
What is in it and why is `$packages`.

**Nothing stored, and the stylesheet's `prefers-color-scheme` query decides** - which is the toggle
defaulting to the machine. `data-default-theme` on `<html>` is the last resort, for a browser reporting no
preference at all.

## W3 - the stylesheet is built by the sass CLI, not by Jekyll

`npm run build:css`, the same way `api/src/Binacle.Net.UIModule` does it. **`_config.yml` carries no `sass:`
block for that reason**, and `css/` is gitignored because it is build output.

## W4 - the pages are complete with JavaScript off

The theme toggle in `_includes/header.html` is hidden until `main.js` reveals it. **A control that does
nothing is worse than no control.** Anything added later follows the same rule.

## W5 - the exchange card is an include so its payloads cannot rot

`_includes/exchange.html` appears on all four pages and reads from one `_data/exchange.yml`. **Inline copies
would be re-run on one page and go stale on the other three.**

Every exchange in that file was really run. The JSON goes through `markdownify` so Rouge marks it up; the
classes Rouge emits are mapped in `_sass/_code.scss`, confirmed against a real build on 23 Aug 2026 -
`.nl` key names, `.s2` strings, `.mi`/`.mf` numbers, `.p` punctuation, `.w` whitespace, `.kc` true/false/null.

```liquid
{% include exchange.html id="hero" block=site.data.exchange.fit
   verdict="The 40x30x20 locker holds all three items. The small one does not." %}
```

## W6 - every example is v3, and that is a contract question

v4 is experimental for the whole 3.0.x line. **`fit/bin`, `fit/smallest-bin` and `pack/smallest-bin` are v4
and must not appear on this site.** `parameters` is required on every fit and pack request - a body without
it returns 400.

The cartonization page uses `by-preset` rather than `by-custom` on purpose: the box set is configured once in
the image, so the request carries only the order, **which is the shape a real integration has**.

## W7 - two pieces of copy are load-bearing and one has an expiry

**`pages/index.html`, the coordinates sentence.** The page promises "where every item sits" and a `fit`
response does not carry coordinates. **Remove the sentence and the page promises something the card does not
show.**

**`pages/index.html`, the multi-box paragraph.** It becomes false the day Binacle.Net distributes an order
across several boxes. **Whoever ships that feature deletes it.**

## W8 - four schema types are rejected, so nobody adds them back

`_includes/schema.html` emits two blocks on `/` alone, where a page opts in with `structured_data: true`.
**Four more were considered and rejected**: `BreadcrumbList` (four flat pages), `FAQPage` (rich results
withdrawn, and it invites writing fake questions), `WebSite` + `SearchAction` (no site search), and `HowTo`
(deprecated).

## W9 - the glyphs are the whole image budget

A tick and a balance mark. Hand-drawn, `currentColor`, stroke only, defined once in `_includes/glyphs.html`
and referenced with `<use>`. **No icon font.**

`_includes/verdicts.html` carries a visually hidden word on each line. **The glyph carries the meaning, so
without it a screen reader gets two identical bullets.**

```liquid
{% include verdicts.html tick="..." scales="..." %}
```

The mark in `_includes/header.html` is the vector file, never a downscaled png - **the raster copies turn to
mush below 48px** and the header renders at 28.
