---
id: shared/design-system
description: Why the design system is built and looks the way it is - each surface builds its own CSS, the Sass conventions, the tiers, colour, type, lines and space, how a script finds an element. What a later change would otherwise undo.
verified: 2026-11-08
check: DS1 against the build:css script in each site's package.json and the UI module's, which runs sass with --pkg-importer=node on that surface's own main.scss. DS2 to DS5 against shared/packages/design-system - its README, the folders, no @import and no "as *" in any .scss of the repo outside vendored lib/, every @media in range syntax naming a tiers. variable. DS6 against abstracts/_tiers.scss, and against every tier above desktop being used by a block that more than the docs site loads. DS7 against base/_tokens.scss, one @font-face per family, and fonts/, one woff2 each. DS8 against .field__input[aria-invalid] in components/field, a border colour and no background. DS9 against components/callout, a left edge and a label in --info, --warn or --error. DS10 against the two theme blocks in base/_tokens.scss, each setting its own --accent. DS11 against every var(--accent) in the package, each a background or a border. DS12 against a search for border-radius and box-shadow, and for --radius and --shadow, which must not exist. DS13 against a search of sites/, shared/ and the UI module for fonts.googleapis and fonts.gstatic, which must find nothing. DS14 against the --size- tokens and a search for a font-size given as a number outside them. DS15 against a search of every stylesheet for min-width and max-width, which must find none, and a search for mobile-only, tablet-only and desktop-only. DS16 against --step and --band-pad in base/_tokens.scss and their use in layout/. DS17 against every querySelector, closest and getElementsByClassName in the TypeScript of shared/packages, sites/*/_js and the UI module, none taking a class but an is- state.
paths:
  - "shared/packages/design-system/**"
---

# Design system - decisions ledger

Why `shared/packages/design-system` is the way it is. Its `README.md` says what is in it, how a partial is
written and where it leaves a convention; this says why, so a later change does not undo a choice made on purpose.
The values themselves are in `base/_tokens.scss` and `abstracts/`.

## Decided

### DS1 - each surface builds its own CSS from the partials

**Decided (the maintainer, 2026-10-31):** asked whether to build the CSS once and copy it to every surface, or
have each surface build from the shared partials: "lets ttry B".

Each surface's `build:css` runs `sass` on its own `main.scss`, which names the parts it uses. His reasons, given
2026-10-27: "css size optimize / sites required unique styles to them / sites conflict at a style and need other
ways to resolve it". A surface carries only the CSS it uses, and a rule only one site needs stays in that site.

**The build is the `sass` command, not Jekyll's Sass,** so the three sites and the UI module build the same way.
Leaning yes (the maintainer, 2026-10-31): "problably custom so not to rely on jekyll".

### DS2 - the folders are by kind, after the 7-1 pattern

**Decided (the maintainer, 2026-10-31):** asked one folder or folders by kind: "4 lets go with converion". The
fifth folder, `utilities/`, and the split of page frames (`layout/`) from what sits inside a page
(`components/`) are his answers of 2026-11-01.

The 7-1 pattern is the best-known Sass layout: anyone who knows it finds a rule by its kind. The README, *Where
it leaves a convention*, says where this differs and why.

### DS3 - nested and flat, each where it makes sense

**Decided (the maintainer, 2026-10-31):** asked whether to nest BEM names with `&` or write them flat: "no i was
thinking nested and flat where it makes sense".

Where the line falls is the agent's (2026-10-31), written in the README, *What is nested, and what is flat*.
The reason for it: every class is written out in full, so a search for a class name lands on its rule.

### DS4 - the Sass module system, with the file name in front

**Decided (the maintainer, 2026-10-31):** asked `tiers.$tablet`, `@use ... as *` with `$tier-tablet`, or
`tiers.$tier-tablet`: "prefix".

So every partial loads another with `@use`, never `@import` and never `as *`, and a name does not repeat its
file's name. `@import` put every file's names in one shared pile, where two files could overwrite each other's
`$width` without a warning. With the prefix, `tiers.$tablet` says where it comes from.

### DS5 - media queries in range syntax, and states as `is-` classes

**Decided (the maintainer, 2026-10-31):** asked range syntax or `min-width:` and `max-width:`: "range". Asked
`--` modifiers or `is-` classes for a state: "yeah i went back and forth lets do is-".

Range syntax is Media Queries Level 4: `(width >= tiers.$desktop)` and `(width < tiers.$desktop)` name one
boundary from both sides, where `min-width` and `max-width` need two numbers a pixel apart.

An `is-` class tells a reader a script turns it on and off. **Where an ARIA attribute already carries the state**
- `aria-current`, `aria-expanded`, `aria-pressed`, `aria-invalid` - the CSS reads the attribute and no class is
added: the agent's (2026-10-31), and the package follows it.

### DS6 - a width a second site uses is a tier

**Decided (the maintainer, 2026-11-01):** asked whether the widths above desktop are tiers or the docs site's
own: "lets go with 1 and we see if it is only for docs then prepend a docs only ... if it turns we need them
for other sites they get bumped into normal tiers".

So a width only `docs` uses would take a `docs` name, and one a second site uses is a tier in
`abstracts/_tiers.scss`. Every boundary there today reaches more than one site.

### DS7 - one font file per face

**Decided (the maintainer, 2026-10-24):** "keep one file per font".

Each family is one variable `woff2`, Latin only, holding every weight the tokens use. A page fetches two font
files and no more. A new weight or script grows the file every visitor downloads, so check its size first.

### DS8 - an error is a red border and a word, never a fill

**Decided (the maintainer, 2026-10-24):** shown a red border and a 2px red left marker on a failing field: "i
like the boarder not the 2px left".

The field takes the red border and the message says what is wrong. The red is chroma-matched to the accent, so
the two differ in hue and not in loudness.

### DS9 - a callout is a coloured edge and label, the body in ink

**Decided (the maintainer, 2026-10-25):** "info = blue or bvlueish / warning yellow/orange / critical = red".
Of the palettes tried: "muted its nto strong or foolproof but lets go with that... a future might decide to
change".

A 2px left edge and the label in `--info`, `--warn` or `--error`, no fill, no border round it - drawn as
GitHub draws its alerts. The colour is never the only sign: the label says the level in words.

### DS17 - a script finds an element by a `data-` attribute, never by a class

**Decided (the maintainer, 2026-10-31):** asked `data-` attributes or a `js-` class prefix: "iver workw ith data
all my life sto data it is".

The class that styles and the hook a script looks for are never the same, so renaming a class to fix a look
cannot break a script without an error. A hook is named for its component, like its class: `data-code-block`,
`data-pack-view-stage`. The one class a script reads is an `is-` state it sets itself (DS5).

## Pending

The agent's (2026-10-31 to 2026-11-05). None of these quotes him.

### DS10 - both themes stand on their own

Each theme's values are chosen and measured for that theme; dark is not light inverted or computed from it.
A colour that works on one paper fails on the other: the accent is 2.18:1 on light paper and 8.03:1 on dark.
**So the accent is a pair**, one orange per theme. A token layer with one orange for both themes is wrong.

### DS11 - the accent is a fill, never text

On light paper the accent is 2.18:1, below WCAG's 4.5:1 for text and its 3:1 for large text. Ink on the
accent is 6.49:1. So the accent is a fill - the primary button, one per page - or the edge of a current
entry that already shows itself in ink, weight and ground.

### DS12 - one line weight, no radius, no shadow

Lines are 1px. Ink marks a boundary that means something - a page edge, a section top, a table head; `--rule`
separates entries inside a list. A 2px mark is the exception: a marker on a picked or current entry, a
callout's edge, the outline of a drawing's object.

Corners are square and nothing casts a CSS shadow, in both themes. An inset `box-shadow` used as a marker
line is a line, not a shadow. **There is no `--radius` and no `--shadow` token:** their absence is the rule,
and a token would invite one.

### DS13 - no font from a third party

Both families are subset files served by the surface itself. A request to a font host sends every visitor's
address to it, and a site that runs anywhere should not depend on another site answering.

### DS14 - type ships as roles, not sizes

A surface names a role - `--size-display-1`, `--size-label`, `--size-code` - never a size, so a size changes
in one place. Display is Familjen Grotesk in sentence case. Labels, numbers and code are Martian Mono; a
number that lines up with others is tabular.

### DS15 - a tier is a change of shape, and its name is never a class

A boundary is where a page changes shape: below `$tablet` a table becomes label and value, below `$desktop`
columns stack. A change of density - gutters, tap targets - happens inside a tier and is not a boundary.

The tier names live in the Sass and nowhere in the markup: no `.mobile-only`, no `s12` / `m6` column classes
under any name. The markup stays plain and the stylesheet responds.

A docs table stays a table on a phone: it is reference, read across a row. `components/table/` says so.

### DS16 - space is one step

The space between bands, sections and blocks is a multiple of `--step`, so they line up from page to page.
A small gap inside a component is its own. The page width, the gutter and the two reading measures - prose
and marketing - are one token each.
