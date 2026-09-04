---
id: sites/www-stylesheets
description: The www stylesheet system - the palette and its measured contrast numbers, the separation rhythm, the one-column hero and why the two-column version was dropped, and the exchange card's scroll behaviour.
verified: 2026-09-04
check: WS1 against the token block in sites/www/_sass/_tokens.scss, where --action and --accent must still be separate from --primary and --tertiary; WS2 against .band, .band.alt and .rule > .shell in _sass/_content.scss; WS3 against .hero-grid, which must stay a single-column grid; WS4 against the max-width 640px block in .scope; WS5 against the @supports (animation-timeline: scroll()) block in .exchange-code; WS6 against the max-width 719px grid block in _sass/_layout.scss
paths:
  - "sites/www/_sass/**"
  - "sites/www/_js/**"
---

# The www stylesheets - why they are shaped this way

**These were comments in `_sass/` and `_js/` until 4 Sep 2026.** They are here because a comment in a
published site is a comment in a public repository.

`_sass/main.scss` is the only entry: tokens, base, layout, code, content, in that order. It is compiled by
the sass CLI rather than by Jekyll, which is `$sites/www-design#W3`.

## WS1 - four hues, and two of them exist twice on purpose

**The brand hues are copied from `sites/demo/_sass/_theme.scss`, not shared with it.** Four duplicated hex
values are cheaper than a build step that can break three sites at once.

The neutrals are derived from `--bg` and `--fg` with `color-mix`, which is what gives a full text ramp
without adding a fifth hue.

**No brand hue is ever the colour of running text.** `#448aff` on `#fdfcff` measures about 3.4:1 - it passes
for large text and UI and fails for body copy, and the dark variants fail on `#101010` the same way.

**`--action` is not `--primary`, and `--accent` is not `--tertiary`.** The palette's dark `--primary` is
deliberately desaturated so it recedes, which is right for a rule or a marker and wrong for the one button
that has to be pressed - in dark it turned the call to action into a dead grey-blue. `--action` is full
strength in both themes and its label is near-black in both, because white on `#448aff` measures about 3.4:1
and fails. `--accent` is the same problem one hue over: the dark `--tertiary` `#5b2d70` is right for a 2px
rule and unreadable as text on `#101010`, so `--tertiary` stays the rule colour and `--accent` carries a word.

**`--surface` is a 6% mix, not the 4% first specified.** At 4% the alternating ground was invisible, and it
is the second-strongest separator the design has. Dark needs 9% - a dark surface needs a bigger lift than a
light one to read as a separate ground at all.

`--measure` is 62ch. Documentation runs 90-100ch, and the narrower measure is the cheapest signal separating
the two registers. The spacing scale is one 8px base. The font stack is system-only: a marketing site for a
self-hosted tool should not fetch a font from a third party.

## WS2 - sections are separated without imagery, in a fixed order

Space, then a 6% ground shift, then a hairline where two same-ground sections meet, then one blue rule on the
page's most important section. **No diagonal dividers, no wave separators, no gradient fades.**

Never more than two consecutive sections on the same ground. The hairline rule
(`.band:not(.alt) + .band:not(.alt)`) never fires today because the pages alternate strictly - it is there so
it stays correct if a section is added or moved.

**The blue rule is a real border on the content column, not an absolutely positioned bar on the band.** The
band is full-bleed, so `left: 0` put the rule against the window edge with the content floating away from it.
The padding gives back exactly what the border takes, so nothing else shifts.

At most one full-strength blue element per screen.

## WS3 - the hero is one column, and the payload is why

The design asked for two columns at roughly 55/45. **A real `fit` response is 757px of monospace at its
longest line and a 45% column is about 517px.** Shrinking the type to fit needs 11px; reflowing the JSON to
short lines doubles the card's height. The card was chopping mid-token, and the `h1` got a 179px sliver.

Full width, the response is complete and legible at 15px. **The two-column version is recoverable the day the
hero shows a shorter exchange.**

## WS4 - the scope table is real table markup and restacks rather than scrolls

Three rows. Scrolling three rows is worse than stacking them, so below 640px every part goes `display: block`,
`thead` is hidden and each cell prints its own label from `data-label` through `td::before`.

**Restacked, the six cells run together as one list and the three rows stop being three.** A hairline on
`tr + tr` says where one row ends, which the header row does on a wide screen.

The first cell has no left padding so it aligns with the prose above it. No vertical rules, no zebra striping,
no outer border.

## WS5 - the exchange card scrolls at every width, and the fade is conditional

Wrapping was tried and it is worse: **a wrapped line starts at a different indent from the line it continues,
so the JSON's own structure stops being readable, and the card grew to about 1900px tall.** A developer
scrolls a code block on a phone without thinking about it. What helps is more room and less type - below
719px the card goes gutter to gutter and the type drops to 13px.

**Gutter to gutter is worth about 100px.** The shell's padding and the card's own padding were costing the
code pane nearly 100px of the 390 a phone has, which is the difference between reading two thirds of a
response line and reading two fifths.

**The fade is inside `@supports (animation-timeline: scroll())` and nowhere else.** A static mask dims the
last word of a pane that fits, which is worse than no mask. An earlier version keyed it on `:hover`, so it
never appeared on the touch devices that need it most.

**Light mode may use one soft shadow, on this card and nowhere else. Dark uses none at all** - a dark page
held together only by borders reads as a TUI, so surfaces lift and lines do the edges.

## WS6 - the header is a grid below 720px, with everything placed explicitly

The flex row put the wordmark, the nav and the theme control on four separate rows - **about 200px of header
before a reader reaches anything.** The nav still wraps to two rows, which the design allows; a row of its own
for the toggle is what it does not need.

**Placement is explicit because source order gets it wrong.** Left to source order the nav spans both columns,
takes row 2 and pushes the toggle onto a row of its own, which is the four-row header this exists to stop.

Five text nav items, no hamburger. GitHub is not in the nav - it is in the hero and in the footer.

**The footer links are navigation, not prose, so the inline-link exemption does not cover them.** At 13px in a
tight row they were a 15px tap target; below 719px the type goes to 14px and the links carry their own
vertical padding.

No shields.io badge anywhere in the footer - that is a third-party request on every page.

## WS7 - the code card is two-tone, and orange means "you type this"

`_sass/_code.scss` maps the Rouge json classes to two colours and no more: keys recede (`--muted`), values
carry (`--text`), punctuation is lighter than either. **No syntax rainbow.** Which class is which is in
`$sites/www-design#W5`.

**Orange (`--secondary`) is what you type, and nothing else.** Roughly four appearances on the whole site: the
method chip and the `$` prompt. Never a button, a heading, a link, a hover state or a section background.

## WS8 - the copy button is the only script beyond the theme

A button marks what it copies with `data-copy="<id>"`. **With JavaScript off the text is still selectable**,
which is the fallback, and the button stays hidden until the script reveals it.

**The clipboard API is permission-gated and refuses outside a secure context.** The catch is empty on purpose:
leave the label alone rather than claim a copy that did not happen.
