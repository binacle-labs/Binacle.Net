---
description: Move the documentation site onto the breadcrumb trail gem - two thirty-line includes and a layout branch out, one tag and one config block in, plus the stylesheet rule the list markup needs.
state: blocked
waits-on: "the gem existing at ruby/jekyll-breadcrumb-trail with a passing spec suite"
---

# Wire the documentation site onto the breadcrumb gem

**This is site-session work.** Everything it changes is under `sites/`, which the standing rule keeps out
of an ordinary coding session. The rule's own answer is a plan that says what must change - this file -
carried out by the session that owns that content.

**The gem is built and its specs pass before this starts. Nothing in `ruby/` is edited here.**

**Only the documentation site.** The other two are flat, have no trail to draw, and get neither the gem nor
the tag.

## Loading it

```ruby
# sites/docs/Gemfile, inside group :jekyll_plugins
gem "jekyll-breadcrumb-trail", path: "../../ruby/jekyll-breadcrumb-trail"
```

```yaml
# sites/docs/_config.yml, under plugins:
  - jekyll-breadcrumb-trail
```

Then `bundle install` in `sites/docs`. **Both halves are needed.**

## The config

```yaml
breadcrumbs:
  hide: ["version", "*.*"]
  class: tiny-space
  link_last: false
  label_from: [menu_title, title]
  home:
    label: Home
    html: '<i class="small">home</i>'
```

**Forgetting the `hide:` block is silent.** Every breadcrumb on the site gains a `Version` crumb and a
`V3.0.x` crumb, nothing fails, and the only thing that catches it is looking.

## The templates

**Delete** `_includes/breadcrumbs.html` and `_includes/versions/breadcrumbs.html`.

**`_includes/header.html`** loses its branch:

```liquid
-   {% if page.version != nil %}
-       {% include versions/breadcrumbs.html %}
-   {% else %}
-       {% include breadcrumbs.html %}
-   {% endif %}
+   {% breadcrumbs %}
```

**No page front matter changes.** `breadcrumbs: false` and `menu_title` are read exactly as they are today.

## The stylesheet, and this is the part with a real cost

The gem emits `<nav aria-label="Breadcrumb" class="tiny-space"><ol><li>...`. Today's markup has no list.

**BeerCSS lays out `nav.tiny-space` by putting a gap between its direct children** - the rule is
`:is(nav, .row, li).tiny-space`. With an `<ol>` inside, the nav has exactly one child, so **the crumbs
collapse into a default vertical list** unless the list gets its own rule.

**`sites/docs/_sass/_list.scss` styles ordered lists for page content** - markers, indents, margins. A
breadcrumb list inherits all of it.

So the site needs a small rule: lay the `ol` out inline with the same gap, drop the markers, and clear the
content-list indent. **Write it against the built page, not against this description** - it is a handful of
declarations and the only way to know it is right is to look.

## What changes on the page

**The markup shape.** A nav landmark with an ordered list, and `aria-current="page"` on the last crumb.
Today's is a row of anchors separated by `<span>/</span>`. **The separator is now the stylesheet's job**,
which is why `separator:` is not in the config block above.

**Three visible changes, all deliberate, all fixes:**

- **The four version index pages gain a crumb.** `/version/v3.0.x/` and its three siblings render a bare
  home icon today, because the old include drops the current page whenever the last segment has a dot. They
  now read `home / v3.0.x`, with home pointing at `/version/`.
- **`pages/404.html` loses its breadcrumb nav.** It sets `breadcrumbs: false`, which **only the versioned
  include ever honoured**. The gem honours it everywhere.
- **Seven swagger pages stop appending a slash to a file.** The old include builds
  `/version/v3.0.x/swagger/v3.html/`. Invisible today because the last crumb is never a link; a 404 the
  moment it goes into markup.

**The labels themselves do not change.** Same source keys, same casing.

## What will bite

**A versioned page's home crumb points at its own version**, `/version/v3.0.x/`, not at the site root.
That is the gem's rule and it is what the versioned include does today. **If a built page shows home
pointing at `/`, the `hide:` block is missing or wrong.**

**The breadcrumb nav sits inside the header's own `<nav>`.** Nesting a landmark inside a landmark is legal
and the `aria-label` is what keeps them distinguishable - do not remove it to tidy the markup.

**A `BreadcrumbList` appears in the structured data only once the structured data gem is also wired.** The
two read and write the same page key and neither requires the other; until both are in place, docs renders
the visible trail and no markup for it.

**Check it at both widths.** The header is a fixed bar with a mobile menu button; a breadcrumb list that
wraps differently from the old spans changes the bar's height on a phone.

## Done when

- [ ] Neither breadcrumb include exists and no Liquid splits a url to build a trail.
      `test ! -f sites/docs/_includes/breadcrumbs.html` and
      `test ! -f sites/docs/_includes/versions/breadcrumbs.html`; `grep -rn "split: '/'" sites/` finds nothing
- [ ] `_includes/header.html` calls the tag once, with no version branch.
      `grep -n "breadcrumb" sites/docs/_includes/header.html` shows one line
- [ ] The visible breadcrumb labels are the same strings they were before.
      Build from the previous commit and compare the label text on one page per level. **Markup differs;
      the words may not**
- [ ] A versioned page's home crumb points at its version, and a plain page's at the site root.
      **By eye** on `/version/v3.0.x/getting-started/` and on a page under `/pages/`
- [ ] The four version index pages read `home / v3.0.x`.
      **By eye** on one of them
- [ ] `pages/404.html` renders no breadcrumb nav.
      `grep -c 'aria-label="Breadcrumb"' artifacts/docs/404.html` is 0
- [ ] No crumb url ends in `.html/`.
      `grep -rn '\.html/"' artifacts/docs/version/*/swagger/` finds nothing
- [ ] The trail renders inline, with no list markers and no content indent, at desktop and phone widths.
      **By eye** in a browser at both. This is the whole point of the stylesheet rule
- [ ] docs builds and every versioned link still resolves.
      `just build docs`, then `just check links docs`
