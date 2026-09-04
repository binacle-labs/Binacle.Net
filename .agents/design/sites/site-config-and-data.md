---
id: sites/site-config-and-data
description: What the three sites' _config.yml and _data files carry that a reader cannot recover - the version_tag trap, the cookie domain, the organisation block, and how the www exchange payloads are re-run.
verified: 2026-09-04
check: C1 against every version_tag in the defaults of sites/docs/_config.yml, each of which must be a tag that exists on Docker Hub; C2 against sites/docs/_data/versions.yml and the version scope blocks in that config, which must list the same folders; C3 against cookie_domain appearing in all three _config.prod.yml files and in no _config.yml; C4 against the structured_data.organization block being written out in full and identical in all three configs; C5 against the verified line on every block in sites/www/_data/exchange.yml; C6 against command.text in that file being a single line
paths:
  - "sites/www/_config.yml"
  - "sites/docs/_config.yml"
  - "sites/demo/_config.yml"
  - "sites/*/_config.prod.yml"
  - "sites/*/_data/**"
---

# The site configs and data files

**These were comments in `_config.yml`, `_config.prod.yml` and `_data/` until 4 Sep 2026.**

Two things here are already decisions and are not repeated: only the current docs version is indexable and
swagger pages are out of every sitemap, which is `$decisions#D7`; the square `og_image` and the small twitter
card are `$sites/decisions#S1`.

## C1 - `version_tag` is a docker tag, never the folder label

Every versioned docs page pulls with `page.version_tag`. **"2.1.x" is not a tag that exists**, and writing the
folder label there published a `docker run` command that failed for the reader.

**A closed line carries its newest patch** - `1.3.0`, `2.0.1`, `2.1.1`. It will get no further release, and no
moving tag was ever published for it.

**The current line carries the minor tag** - `3.0` - which the release workflow publishes and which resolves
to the newest patch, so it needs no edit when 3.0.1 ships.

## C2 - a docs version needs two edits or it is invisible

`_data/versions.yml` is the source of truth for the list. **The order in that file is the render order, so
keep it newest first** - Jekyll's own ordering sorts by path and would put v3.10.x before v3.2.x. `current`
names the current folder, points the `latest` redirect, and decides which version is indexable.

**A version also needs its own `defaults` scope block in `_config.yml`**, carrying `version` and
`version_tag`. Without it the pages have neither and the version does not appear in the selector.

## C3 - `cookie_domain` is production-only, and that is not an oversight

`.binacle.net` is shared across www, demo, docs and the API, so a theme chosen on one host is the theme on all
of them. **It is absent from `_config.yml` on purpose**: a local build is not on that domain and the browser
would drop the cookie.

## C4 - the organisation block is written out in full on all three sites

Identical in all three configs, and not derived from `site.url`. **Derived, it would be a different
organisation on each host** - three organisations instead of one.

## C5 - every exchange on www was really run, and here is how to run it again

`sites/www/_data/exchange.yml` holds every request and response the site shows, in one file.
**A response that looks plausible but is not what the API returns is the one lie this audience catches.**

```
just serve api            then POST the request to https://localhost:7194
or: docker run ... binacle/binacle-net:<tag>   and POST to http://localhost:8080
```

Paste the response back verbatim and update `verified` on the block that changed.

**Open at 4 Sep 2026: every block says "source build of the 3.0.x line".** They were run against the working
tree, not against a published tag. Re-run them against the tag `command.tag` names, so the page and the image
a reader pulls agree.

Two payload choices are not obvious. The parcel-locker example carries **one item on purpose**: a multi-item
order against a compartment nothing fits produces an empty `fittedItems` row that reads as a bug unless the
page explains the sort order. Its compartment heights are the shape a locker bank has, not anyone's published
figures, and the page says so.

The rest of the contract rules for these payloads - v3 only, `parameters` required, `by-preset` on the
cartonization page - are `$sites/www-design#W6`.

## C6 - the docker line is one line, and the tag is a variable

**No backslash continuations.** They were tried: `pre-wrap` keeps both the newlines and the continuation
indents and then wraps on top of them, which left a stray `-e` alone on its own line. A single line wraps at
its spaces.

`command.tag` is a variable so the tag changes in one place when the line moves. **It must name a tag that is
actually published.**

## C7 - www's description is a fallback and is never derived from the excerpt

Every page writes its own `description` in front matter. The `_config.yml` one is what the homepage uses and
what a page that forgot would fall back to. **Deriving it from the excerpt is what truncated the old site's
snippet mid-word.**

`url` is the one line the host is named on - every absolute URL comes from it through `absolute_url`.

## C8 - the icon list is copied in, not authored per site

The favicon and touch-icon files land in each site root from `assets/` through `just assets`. The `icons:`
list in `_data/includes.yml` is what renders the link tags, and the manifest link rides that same list -
`$sites/webmanifest`.
