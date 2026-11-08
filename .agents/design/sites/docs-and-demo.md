---
id: sites/docs-and-demo-design
description: Why the docs and demo templates are shaped this way - the version in a page title, the demo scripts and their prefetch, the outdated-version notice, and the breadcrumb separator.
verified: 2026-11-08
check: D3 against binacle-docs stamping title_suffix and jekyll-page-meta writing the title element on one line; D4 against demo_scripts in sites/demo/_data/includes.yml, the page.demo branches in sites/demo/_layouts/default.html and the three chunk in sites/demo/webpack.config.js; D5 against sites/docs/_includes/outdated-notice.html; D6 against the breadcrumbs nav in sites/docs/_layouts/page.html and the ::before separator in the design system's breadcrumbs block
paths:
  - "sites/demo/**"
  - "sites/docs/**"
---

# The docs and demo templates - why they are shaped this way

**These were template comments until 23 Aug 2026.** They are here because a comment in a published site is a
comment in a public repository.

**Two of them are not repeated here** because a ledger holds them: the square `og_image` and
`twitter:card summary` pair is `$sites/decisions#S1`, and indexing only the `current` version is
`$decisions#D7`.

## D3 - a version's page title carries its version

Or it collides with the same page at the site root and with every other version of itself. **Keep the whole
title under 60 characters.**

The `<title>` value is emitted with no newlines inside the tag - **some scrapers keep the whitespace and print it
in the result.**

## D4 - three.js is the asset budget, and prefetch is why it costs nothing

**three ships as 3 pre-bundled modules, with no tree shaking** - the bundle size is in `$sites/demo`. It
gets its own webpack chunk so it stays cached when app code or Alpine change.

`demo: true` in a page's front matter loads the demo scripts; **every other page prefetches the same list**, so
arriving at a demo costs no download. **One list, `demo_scripts` in `_data/includes.yml`, so the two can never
drift.**

## D5 - the outdated-version notice is why the docs site is versioned at all

Shown on every page of a version that is not current. **The site exists so someone running an older image can
read that image's docs** - without the notice, an old page is indistinguishable from a current one.

It is the design system's warning callout, which supplies the colour in both themes.

## D6 - the breadcrumb separator is CSS, not a character in the markup

The trail is a `nav` of links, the last crumb a `span` with `aria-current="page"`. The design system's
`breadcrumbs` block draws the `/` between crumbs with `::before`. **A literal slash in the markup is one a
screen reader reads aloud**, so none is written there.
