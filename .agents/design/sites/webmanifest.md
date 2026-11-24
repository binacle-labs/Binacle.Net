---
id: sites/webmanifest
description: Why the three sites ship a web app manifest rather than dropping the two android icons, where its colours come from, and why the UI module gets neither.
verified: 2026-11-25
check: both colours in each site's webmanifest block against the light --paper in the design system's base/_tokens.scss; the android-chrome ignore line in gulpfile.js for the uimodule target; no site holding a pages/site.webmanifest of its own
paths:
  - "sites/**"
  - "gulpfile.js"
---

# The web app manifest

**Two files decided this.** `assets/android-chrome-192x192.png` and `assets/android-chrome-512x512.png` were
copied into all three sites and the UI module and referenced by nothing at all. They exist for a manifest,
and there was no manifest. The choice was write one or stop copying them.

**Written, 24 Aug 2026.** The files are made, shipped and already paid for in image size; deleting them
throws away work that is done, and a manifest is what makes a site installable.

## Every value traces to something, and the colours to a stylesheet

`name` is the block's own `name:` where a site sets one and `title` otherwise. The demo sets
`name: "Binacle.Net Demo"`, so it is not a second thing called "Binacle.Net". `description` is the site description. `start_url` is the
site root through `relative_url`.

**There is no `short_name`.** Android truncates a home-screen label at about twelve characters and no site
config carries a shorter name. Inventing one is inventing a brand string, so the key is left out and the
name is truncated instead.

**Both colours are the light theme's `--paper`, `#efece4`, on all three sites.** `theme_color` is the bar over an
installed site and `background_color` the splash behind its icon. Every header sits on the page background, so
one colour fits both. **Decided (the maintainer, 2026-11-25):** asked `light`, `dark` or `keep`: "light".

**One theme is always off.** A manifest holds one set of colours and the sites have two themes, so on a phone in
dark mode the bar and the splash are light. Light is the theme the tokens start from; dark overrides it.

**That hex now exists twice**, in `base/_tokens.scss` and in the `webmanifest:` block of each `_config.yml`. A
manifest is JSON served to a browser and cannot read CSS, so there is no way to hold it once. **They are a pair:
change the light `--paper` and change the manifests in the same commit**, or the splash and the bar stop matching
the page. Nothing fails when they drift.

## It is generated, not written

**Each site wrote the file by hand for about an hour on 24 Aug 2026, then `jekyll-webmanifest` took it over.**
The gem could own the whole file where `binacle-robots` could not, because a manifest waits on no other
generator - every value is config, `site.title` or `site.description`. So the sites keep a `webmanifest:`
block and no page at all.

Nothing it says changed in the move. The only difference in the built file is that `JSON.pretty_generate`
puts each icon's keys on their own line.

## The link rides the icon list

The `rel="manifest"` link is one entry in each site's `_data/includes.yml` under `icons:`, which
`{% link_tags %}` already renders into every head. No layout was touched, and the link is on every page.
**The gem ships a link tag and none of these three uses it**, because that would write the link twice.

## The UI module gets neither

It links `favicon.ico` and nothing else, it is a module inside a self-hosted image rather than a site anyone
installs, and a manifest for it would need a name and colours nobody has chosen. So the two icons are in the
`uimodule` ignore list in `gulpfile.js` instead - the only place the delete answer was the right one.
