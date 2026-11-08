---
id: sites/www-design
description: Why the www site's templates are shaped the way they are - the traps that bite silently, and the constraints a rewrite would break without noticing.
verified: 2026-11-08
check: W1 against the expand_year call in _includes/footer.html carrying no argument; W2 against the theme-init script tag in _layouts/default.html being in head with no defer or async, the second webpack config in webpack.config.js, and _js/theme-init.ts importing @binacle/design-system/theme-init; W3 against _sass/main.scss and the build:css script in package.json; W4 against theme-switcher in _includes/header.html being defined by main.js; W5 against exchange.html still being an include and every page calling it; W7 against the scope table on the home and cartonization pages; W9 against the fit-row__verdicts lists holding the verdicts as text, and the header's img taking site.logo.src; W10 against sites/www/_includes/drawings/, whose SVGs carry no fill attribute outside a mask, and every page pulling its drawing in with an include
paths:
  - "sites/www/**"
---

# The www site - why the templates are shaped this way

**These were Liquid comments in the templates until 23 Aug 2026.** They are here because a comment in a
published site is a comment in a public repository.

## W1 - `expand_year` is called with no argument, and that is the whole point

`_includes/footer.html` was `{% assign %}` plus a `replace` until 24 Aug 2026, because **Liquid's variable
parser stops an output tag at the first `}` it meets**, and `{now}` has one. `{{ x | replace: "{now}", y }}`
fails the build with `Variable was not properly terminated`, at parse, with a message that does not name the
placeholder.

**The trap did not go away with the assign.** `expand_year` takes the placeholder as an optional argument,
and `{{ x | expand_year: "{now}" }}` fails exactly the same way. `{now}` is the filter's default so that the
placeholder never has to be written in a template at all - which is what lets the footer be one output tag.

**Leave the argument off.** If a site ever needs a different placeholder, either give it one with no brace
in it or set it in an `assign`, where the rule does not apply.

## W2 - the theme read is a separate bundle, loaded blocking in `<head>`

`js/theme-init.js` runs before first paint. **A deferred or module script cannot do this** - it runs after
the first paint, so a reader who chose light gets a dark flash on every navigation. It is `<script src>`
with no `defer` and no `async` on purpose.

**It cannot touch `document.body`** - the parser has not reached it - which is why the theme is `data-theme`
on `<html>` and not a class on `<body>`, on this site and on the other three.

**It is a second webpack config, not a second entry.** On demo the main config splits every entry into a
runtime chunk and a vendors chunk, and a head script has to be one self-contained file. www's and docs' main
configs do not split; they keep the second config so all three sites build the head script the same way, alone
and with no source map. Both write into `js/`, so the main config's `clean` keeps `theme-init.js` - without
that the two race and the file is deleted after it is written.

`_js/theme-init.ts` imports `@binacle/design-system/theme-init`, the package's entry for the head script, so the
switcher element stays out of it. What is in it and why is `$packages`.

**With nothing stored, the stylesheet's `prefers-color-scheme` query decides** - which is the toggle
defaulting to the machine. `data-default-theme` on `<html>` is the last resort, for a browser reporting no
preference at all.

## W3 - the stylesheet is built by the sass CLI, not by Jekyll

`npm run build:css`, the same way `api/src/Binacle.Net.UIModule` does it. **`_config.yml` carries no `sass:`
block for that reason**, and `css/` is gitignored because it is build output.

## W4 - the pages are complete with JavaScript off

The theme toggle in `_includes/header.html` is a `<theme-switcher>` element that `main.js` defines. Until then
it is empty, so with JavaScript off there is no button. **A control that does nothing is worse than no
control.** Anything added later follows the same rule.

## W5 - the exchange card is an include so its payloads cannot rot

`_includes/exchange.html` appears on all four pages and reads from one `_data/exchange.yml`. **Inline copies
would be re-run on one page and go stale on the other three.**

Every exchange in that file was really run. The JSON is escaped into a plain code block, with no highlighting.

```liquid
{% include exchange.html block=site.data.exchange.fit fold=true
   verdict="The medium locker holds all three items. The small one takes only one of them." %}
```

## W6 - every example is v3, and that is a contract question

v4 is marked experimental (`ApiV4Document.IsExperimental`). **`fit/bin`, `fit/smallest-bin` and `pack/smallest-bin` are v4
and must not appear on this site.** `parameters` is required on every fit and pack request - a body without
it returns 400.

The cartonization page uses `by-preset` rather than `by-custom` on purpose: the box set is configured once in
the image, so the request carries only the order, **which is the shape a real integration has**.

## W7 - two pieces of copy are load-bearing and one has an expiry

**`pages/index.html`, the coordinates sentence.** The page promises "where every item sits" and a `fit`
response does not carry coordinates. **Remove the sentence and the page promises something the card does not
show.**

**The scope table on `pages/index.html` and `pages/cartonization.html`**: "works one box at a time", and
"How do I split an order across boxes?" under what it does not answer. Both become false the day Binacle.Net
distributes an order across several boxes. **Whoever ships that feature changes them.**

## W8 - four schema types are rejected, so nobody adds them back

`{% structured_data %}` writes the graph, and `/` alone names a type, `SoftwareApplication` - `$sites/www`.
**Four more were considered and rejected**: `BreadcrumbList` (four flat pages), `FAQPage` (rich results
withdrawn, and it invites writing fake questions), `WebSite` + `SearchAction` (no site search), and `HowTo`
(deprecated).

## W9 - a verdict is a word, and the mark is the vector file

Under each drawing, the `fit-row__verdicts` list gives each box the API's own result, `AllItemsFit` or
`NotAllItemsFit`, as text. **No glyph stands in for it**, so a screen reader reads the same verdict a sighted
reader sees.

The mark in `_includes/header.html` is `site.logo.src`, the SVG, never a downscaled png - **the raster copies
turn to mush below 48px**, and the header draws it well below that.

## W10 - each drawing is an include file, not an image

Every picture on the site is an SVG file in `_includes/drawings/`, one per drawing, and each page includes its own:
`{% include drawings/hero.svg %}`. No site build draws them; the files are committed as they are.

**Decided (the maintainer, 2026-11-05):** "we do partials" - asked whether the drawings stay pasted into each page
or move into include files.

**The SVG carries no colour.** Every shape has a class and no `fill` - the only `fill` is a fade mask's - and the
design system's `drawing` block paints them, `drawing--painted` on the wrapping div. Included, the SVG is part of
the page, so the page's CSS reaches it - the built page is the same as one with the drawing pasted in. **The two
other ways to reuse an SVG both break it:** as an `<img>` the page's CSS cannot reach inside, and it draws a black
shape; through `<use>` the shapes sit in a shadow tree the page's selectors do not match, and the walls lose their
paint.

**The home hero is two files**, the whole locker bank and its crop for a phone. The `drawing-pair` that holds them
stays in the page, not in either file.
