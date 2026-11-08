---
id: sites/site-theming-css
description: The CSS side of the theme on every surface - why the attribute is on html and the system query lives in the stylesheet, how color-scheme follows the choice, and why the switcher element is not a box.
verified: 2026-11-08
check: T1 and T2 against the :root block, the -dark mixin, :root[data-theme="dark"] and the prefers-color-scheme block in shared/packages/design-system/base/_tokens.scss; T3 against theme-switcher { display: contents } in components/theme-button/_index.scss, and the element being empty in each surface's header
paths:
  - "shared/packages/design-system/base/_tokens.scss"
  - "shared/packages/design-system/components/theme-button/**"
---

# The theme, from the stylesheet's side

The design system's tokens hold the theme for all three sites and the UI module. The script half - why the
pre-paint read is a separate blocking bundle and why it cannot touch `document.body` - is `$sites/www-design#W2`.

## T1 - no attribute at all is a state, and only CSS can carry it

The theme is `data-theme` on the `html` element. **No attribute means follow the machine**, which is the one
state an attribute or a class cannot express, and that is why the `prefers-color-scheme` query lives in the
stylesheet rather than in JavaScript.

Dark applies in two cases: `data-theme="dark"`, or no `light` choice on a dark machine. Both include one
`-dark` mixin, so the two cannot drift apart.

## T2 - `color-scheme` moves with the theme

`color-scheme: light` sits in the `:root` block and `color-scheme: dark` in the dark mixin. **A reader who picks
light on a dark machine gets light scrollbars and form controls**, because the dark block never applies to
them. A separate light rule is not needed.

## T3 - the switcher element is not a box

**`theme-switcher { display: contents; }`** - the element only hosts the button. The button has to sit
directly in the header's row, where the header's layout expects it.

The element is empty in the markup; the script writes the button into it. **With JavaScript off there is no
button**, and a dead control is worse than no control.
