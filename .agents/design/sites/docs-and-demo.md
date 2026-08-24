---
id: sites/docs-and-demo-design
description: Why the docs and demo templates are shaped this way - the beercss and Alpine traps, the contrast measurements behind the component overrides, and the asset budget.
verified: 2026-08-24
check: D6 against sites/docs/_sass/_breadcrumbs.scss, which must stay one rule; D1 against the progress elements in sites/demo/pages/packing.html and vipaq.html, which must both still carry value="0"; D2 against the four overrides in sites/demo/_sass/_components.scss; D4 against the prefetch list in sites/demo/_data/includes.yml
paths:
  - "sites/demo/**"
  - "sites/docs/**"
---

# The docs and demo templates - why they are shaped this way

**These were template comments until 23 Aug 2026.** They are here because a comment in a published site is a
comment in a public repository.

**Two of them are not repeated here** because they are already decisions: the square `og_image` and
`twitter:card summary` pair is `$sites/decisions#S1`, and indexing only the `current` version is
`$sites/decisions#S5`.

## D1 - `value="0"` on a progress element is not a default

**It is what keeps the bar determinate.** Alpine skips its own assignment when `el.value` already equals the
bound one, and an indeterminate progress reads `0` - so **a 0% result would never get the attribute**, and
beercss animates every progress that lacks it.

**`pages/vipaq.html` did not carry it until 23 Aug 2026**, while the comment above that element claimed the
trap was handled the same way as `packing.html`. **The comment was wrong and nothing could see it** - which is
part of why these moved here.

**Removing the attribute looks harmless and breaks only the zero case**, which is the case a demo hits when
the answer is "nothing fits".

## D2 - the four beercss overrides are contrast fixes, and here are the numbers

Measured, not guessed. `sites/demo/_sass/_components.scss`:

| Override | Why |
|---|---|
| `.border` colour | beercss hardcodes `color: var(--primary)`, which lands on the panel colour, not on white - **1.53:1 in light, 1.01:1 on the dark items panel.** The on-container colour is 6.8:1 or better everywhere |
| destructive actions | the warning is the container fill, not coloured text - **raw `--error` on these panels measures 2.3-4.4:1**, the pair used is 8:1 or better. `.transparent` sets both properties `!important`, so the override has to outrank it |
| the one filled button | **white on `--primary` is 3.32:1 in light, black 6.33:1; in dark it is the other way round.** `--on-primary` cannot just flip - it also paints the header, whose GitHub and Docker marks are white PNGs. The pair used is already black and white in the right order |
| the whole-card link | beercss styles `article` as an element, so the anchor sits inside and fills it. Without `display: block`, beercss's `a { display: inline-flex; align-items: center }` puts the title beside the copy |

## D3 - a version's page title carries its version

Or it collides with the same page at the site root and with every other version of itself. **Keep the whole
title under 60 characters.**

The `<title>` value is emitted with no newlines inside the tag on both sites - **some scrapers keep the
whitespace and print it in the result.**

## D4 - three.js is the asset budget, and prefetch is why it costs nothing

**three ships as 3 pre-bundled modules, 566 KiB minified, no tree shaking** - 601 KiB before minification. It
gets its own webpack chunk so it stays cached when app code or Alpine change.

`demo: true` in a page's front matter loads the demo bundle; **every other page prefetches the same list**, so
arriving at a demo costs no download. **One list in `_data/includes.yml`, so the two can never drift.**

## D5 - the outdated-version notice is why the docs site is versioned at all

Shown on every page of a version that is not current. **The site exists so someone running an older image can
read that image's docs** - without the notice, an old page is indistinguishable from a current one.

It reuses `.block-warning` from `_sass/_blockquote.scss`, which supplies the label and the dark mode.

## D6 - the breadcrumb stylesheet is one rule because beercss already does the rest

`sites/docs/_sass/_breadcrumbs.scss` holds a single declaration block, and that is not an oversight.

The trail is `<nav class="tiny-space"><ol class="breadcrumb"><li class="breadcrumb-item">`, which is the
markup `jekyll-breadcrumb-trail` emits everywhere. beercss already carries rules for exactly that shape,
written for its drawer navigation: `nav>:is(ol,ul){all:inherit;flex:auto}` makes the list inherit the nav's
`display:flex` and `align-items:center`, and `nav>:is(ol,ul)>li{all:unset}` strips the list marker and the
list-item display. The crumbs lay out in a row with no help from us.

**`class: tiny-space` in the config block is load-bearing.** beercss sets `:is(nav,.row,li).tiny-space{gap:.5rem}`
on the nav, and `all:inherit` is what carries that gap down to the list. Drop the class and the crumbs run
together, because the gap is the only spacing between them.

**What is left is the separator**, and it is ours because the standard breadcrumb markup has none in the
list - Bootstrap, which the class names come from, draws it with `::before` and so do we. The old markup put
a literal `<span>/</span>` between the crumbs, which is a slash a screen reader reads aloud.

The Bootstrap class names are inert here. beercss has no `.breadcrumb` rule, so they cost nothing and they
are the selector this file needs.
