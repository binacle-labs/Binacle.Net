---
id: sites/webmanifest
description: Why the three sites ship a web app manifest rather than dropping the two android icons, where its colours come from, and why the UI module gets neither.
verified: 2026-08-24
check: the theme_color in each site's webmanifest block against the dark --primary and --bg in that site's own sass; the android-chrome ignore line in gulpfile.js for the uimodule target; no site holding a pages/site.webmanifest of its own
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

## Every value traces to something, and two of them to a stylesheet

`name` is `display_title` where a site sets one and `title` otherwise, so the demo is "Binacle.Net Demo"
rather than a second thing called "Binacle.Net". `description` is the site description. `start_url` is the
site root through `relative_url`.

**There is no `short_name`.** Android truncates a home-screen label at about twelve characters and no site
config carries a shorter name. Inventing one is inventing a brand string, so the key is left out and the
name is truncated instead.

**`theme_color` is measured per site, and it is not the same on all three.** It colours the browser bar, so
it is the colour at the top of that site's page: `#3c5d8b`, the dark `--primary`, on demo and docs, which
run a primary-coloured header bar; `#101010` on www, whose header sits on the page background.
`background_color` is `#101010` everywhere - the dark `--bg` - because that is the splash behind the icon.

**Those two hexes now exist twice**, in `_sass` and in the `webmanifest:` block of `_config.yml`. A manifest
is JSON served to a browser and cannot read CSS, so there is no way to hold them once. **They are a pair:
change a theme colour and change the manifest in the same commit**, or the splash and the browser bar stop
matching the page. Nothing fails when they drift.

## It is generated, not written

**Each site wrote the file by hand for about an hour on 24 Aug 2026, then `jekyll-webmanifest` took it over.**
The gem could own the whole file where `binacle-robots` could not, because a manifest waits on no other
generator - every value is config, `site.title` or `site.description`. So the sites keep a `webmanifest:`
block and no page at all.

Nothing it says changed in the move. The only difference in the built file is that `JSON.pretty_generate`
puts each icon's keys on their own line.

## The link rides the icon list

The `rel="manifest"` link is one entry in each site's `_data/includes.yml` under `icons:`, which
`{% link_tags %}` already renders into every head. No layout was touched, and the link is on all 127 pages.
**The gem ships a link tag and none of these three uses it**, because that would write the link twice.

## The UI module gets neither

It links `favicon.ico` and nothing else, it is a module inside a self-hosted image rather than a site anyone
installs, and a manifest for it would need a name and colours nobody has chosen. So the two icons are in the
`uimodule` ignore list in `gulpfile.js` instead - the only place the delete answer was the right one.
