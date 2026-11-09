---
id: sites/templates
description: How the three Jekyll sites name and place their template files - one base layout and wrappers, where a partial sits says how far it reaches, and when a partial takes a generic name. The class names follow BEM; the template files do not have to.
verified: 2026-11-10
check: T1 against _layouts/ in sites/www, sites/docs and sites/demo - a default.html with no layout of its own, a page.html with layout default, and the layout default in each _config.yml being page; T2 against each site's _includes/, where the site header and footer are header.html and footer.html at the root with no site- in the name
paths:
  - "sites/www/_layouts/**"
  - "sites/www/_includes/**"
  - "sites/docs/_layouts/**"
  - "sites/docs/_includes/**"
  - "sites/demo/_layouts/**"
  - "sites/demo/_includes/**"
---

# Sites - template files

How `www`, `docs` and `demo` name and place their layouts and partials. The same rules hold on every Jekyll site,
so a reader who knows one site finds their way in the others.

**The classes follow BEM; the template files do not have to.** The maintainer, 2026-11-05: "tne partials mainly
/ but i dont want them all bem". Page files keep their names.

## Decided

### T1 - one base layout, then wrappers

**Decided (the maintainer, 2026-11-05):** "i have 1 base the page.html or default.html then wrappers if the site
has pages with 2-3 layout sizes like full homescreen etc", and "that pattern must hold for every site iun jekyll
or have 1 consistent pattern". Asked which name is the bare one: "swap", so `default.html` is the base.

| Layout | What it is |
|---|---|
| `default.html` | the base: the bare document, `<head>` and `<body>`, and nothing a visitor sees |
| `page.html` | the wrapper every page gets: `layout: default`, then the header, the main area and the footer |
| anything else | a wrapper for another page size: `layout: default`, beside `page` |

The names are Jekyll's own, from its theme template. Each site's `_config.yml` sets `page` as the default layout.

**Two docs layouts stand alone** (agent, 2026-11-10): `swagger.html`, the API reference the docs show inside a
frame, and `redirect.html`. Neither is a page in the site frame, so neither wraps `default`.

### T2 - where a partial sits says how far it reaches

**Decided (the maintainer, 2026-11-05):** "a convention i have is includes root m,eans used throughotu like
includes/header = site header / includes/<smotheing>/header a specific header".

So `_includes/header.html` is the site header, and it takes no `site-` in its name. `_includes/<group>/header.html`
would be a header for that group only.

**A folder may also hold a group kept out of the root.** The maintainer, 2026-11-05: "we can add a foler in
includes to describe a specific group we need to hide from root". `drawings/` is one. A `components/` folder is
allowed, but not for partials used across the site: "but i could see components in a site just not in this case".

### T3 - a partial used in many ways takes the generic name

**Decided (the maintainer, 2026-11-05):** "if we can create an abstract partial that will be used in many places
with data passed into it then we name the generic name / otherwise the use", and "this new rule too persisted
across the sites".

*Many places* means on one site: each site has its own partials (agent, 2026-11-05).

## Pending

### T4 - the UI module keeps Razor's names

The UI module is Razor, not Jekyll, so its base stays `_Layout.cshtml` and `Pages/Shared/` is its root (agent,
2026-11-05).
