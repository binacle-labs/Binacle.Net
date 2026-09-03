---
id: sites/site-theming-css
description: The CSS side of the theme on all three sites - why the attribute is on html, why every dark block also lands on body, and the two rules that keep the switcher element from being a box or a dead control.
verified: 2026-09-04
check: T1 against the @media (prefers-color-scheme: dark) block in sites/www/_sass/_tokens.scss and the when-dark mixin in sites/docs/_sass/_theme-modes.scss and sites/demo/_sass/_theme-modes.scss; T2 against the body selectors inside that mixin, which must stay paired with the :root ones; T3 against :root[data-theme="light"] { color-scheme: light } on all three sites; T4 against theme-switcher and theme-switcher:not(:defined) in sites/www/_sass/_layout.scss and both _theme.scss files
paths:
  - "sites/www/_sass/**"
  - "sites/docs/_sass/**"
  - "sites/demo/_sass/**"
---

# The theme, from the stylesheet's side

**These were comments in the three sites' sass until 4 Sep 2026.** The script half - why the pre-paint read is
a separate blocking bundle and why it cannot touch `document.body` - is `$sites/www-design#W2`.

## T1 - no attribute at all is a state, and only CSS can carry it

The theme is `data-theme` on the `html` element. **No attribute means follow the machine**, which is the one
state an attribute or a class cannot express, and that is why the `prefers-color-scheme` query lives in the
stylesheet rather than in JavaScript.

## T2 - on docs and demo every dark block lands on `body` as well as `html`

**BeerCSS declares its own tokens on the `body` element.** A declaration on `body` beats one inherited from
`html` whatever the specificity - they are different elements, so specificity never comes into it. BeerCSS
also stamps light or dark on `body` itself when it finds neither, which is now always.

The `when-dark` mixin exists so the pairing cannot be forgotten in one block and remembered in another. www
has no BeerCSS and needs no `body` selector.

## T3 - a stored light choice has to move `color-scheme` too

`:root[data-theme="light"] { color-scheme: light; }` on all three sites. Without it a reader who picks light
on a dark machine keeps dark scrollbars and dark form controls on a light page.

## T4 - two rules keep the switcher element honest

**`theme-switcher { display: contents; }`** - the element is a definition point, not a box. The button it
renders has to sit directly in the header's layout, where the host's classes expect it.

**`theme-switcher:not(:defined) { display: none; }`** - until the script defines it the element is not a
control, just its own contents. A dead control is worse than no control.
