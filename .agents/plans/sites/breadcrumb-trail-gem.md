---
description: Build ruby/jekyll-breadcrumb-trail - a generator that computes a page's trail once, and a tag that renders it as standard accessible breadcrumb markup. The gem only.
state: ready
waits-on: "nothing"
---

# Build the breadcrumb trail gem

**This is the gem half only.** Nothing under `sites/` is touched.

The documentation site computes its breadcrumb trail in two includes that are the same thirty lines: split
the url on `/`, rebuild a cumulative path in a nested loop, title-case each segment. The versioned copy adds
two `continue` guards. **A third site does not have a trail and does not want one** - a breadcrumb on a flat
site is noise in the markup and a lie in a search result.

## It must carry into an unrelated site unchanged

**No product name in the gem code, in a spec or in the README.** Specs build a throwaway site at
`https://example.com`. `jekyll-breadcrumbs` is taken on rubygems, which is why this is
`jekyll-breadcrumb-trail`.

## A generator and a tag

**The generator computes the trail once and writes it to `page.breadcrumb_trail`. The tag renders it.**

```yaml
page.breadcrumb_trail:
  - name: Home
    url: /version/v3.0.x/
  - name: Getting started
    url: /version/v3.0.x/getting-started/
  - name: Quick start
    url: /version/v3.0.x/getting-started/quick-start.html
```

**`name` and `url`, because that is what they become** - schema.org's `ListItem` uses `name`, and a
structured data gem reads this key to write the `BreadcrumbList`. **The visible trail and the markup are one
computation**, which is the entire reason this is a generator and not logic inside a tag. Google's rule is
that the markup describes visible content, and two implementations of one trail are free to disagree while
nothing fails.

## The markup

Standard, accessible breadcrumb markup - a navigation landmark, an ordered list, and the current page
marked:

```html
<nav aria-label="Breadcrumb" class="tiny-space">
  <ol>
    <li><a href="/version/v3.0.x/"><i class="small">home</i></a></li>
    <li><a href="/version/v3.0.x/getting-started/">Getting started</a></li>
    <li><span aria-current="page">Quick start</span></li>
  </ol>
</nav>
```

**This is what the accessibility guidance specifies and what every design system emits.** A run of anchors
separated by slash characters, with no list and no `aria-current`, reads to a screen reader as a row of
unrelated links.

## The config, closed at these keys

```yaml
breadcrumbs:
  hide: ["version", "*.*"]      # globs. Default: hide nothing
  class: tiny-space             # goes on the <nav>
  link_last: false
  label_from: [menu_title, title]
  home:
    label: Home
    html: '<i class="small">home</i>'   # replaces the label when set
```

**Closed on purpose.** Wrapper elements, per-crumb attributes and aria overrides are not config - a site
wanting those renders `page.breadcrumb_trail` in its own Liquid. **That key is what keeps this config from
growing into a template language.**

**The gem ships no hide list.** A default naming `version` would be one site's vocabulary living in a gem
that other sites load.

**`label_from` names front matter keys in order**, the same shape the page meta gem uses for its
description chain. No site's front matter word is frozen into a shared gem.

## The home crumb points at the deepest hidden prefix

**Not the site root.** This is the rule that makes `hide` work at all, and it must not be "corrected" later.

| Page url | `hide` | Home crumb points at |
|---|---|---|
| `/version/v3.0.x/getting-started/quick-start.html` | `["version", "*.*"]` | **`/version/v3.0.x/`** |
| `/version/v3.0.x/` | `["version", "*.*"]` | `/version/` - the version segment is the current page, and the current page is never hidden |
| `/getting-started/` | `["version", "*.*"]` | `/` - nothing matched |
| any | `[]` | `/` |

**A versioned page's trail starts at its own version**, which is what the versioned include resolves today.
Walking up past it would offer a reader a link out of the version they are reading.

**Hidden segments stay in every url.** The trail a crawler follows must be real, even where the label for
that level is suppressed. Getting this backwards produces a trail of 404s in the markup.

## What will bite

**The current page is never hidden**, whatever it is called. A version index page is `v3.0.x` and matches
`*.*`, and it is still the crumb you are standing on.

**A page that is a file must not gain a trailing slash.** The rebuilt path always ends in `/`; a last
segment containing a dot is a file. Today's include builds `/version/v3.0.x/swagger/v3.html/`, which is
invisible only because the last crumb is never a link - and a 404 the moment it goes into markup.

**Label casing has to match the existing filter exactly.** The trail title-cases each segment the way
`capitalize_all` does. Change one and every label on the site shifts at once.

**`breadcrumbs: false` on a page means an empty trail**, and an empty trail renders nothing - not an empty
`<nav>`.

**Every label and url is escaped.** A page title containing `&` or a quote otherwise breaks the markup.

**Both halves are needed to load a path gem** - the `Gemfile` line resolves the path, the `plugins:` line
loads it. Miss the second and Jekyll does not fail; a tag renders as text.

## Done when

- [ ] `ruby/jekyll-breadcrumb-trail` exists with a gemspec, a `README.md`, a `Gemfile` and a passing spec suite.
      `test -f ruby/jekyll-breadcrumb-trail/jekyll-breadcrumb-trail.gemspec` and `rspec` passes there
- [ ] Nothing in it names the product.
      `grep -rni "binacle" ruby/jekyll-breadcrumb-trail` finds nothing
- [ ] The generator publishes `page.breadcrumb_trail` and the tag computes nothing.
      A spec reading the key after a build, and **by eye** that the tag only formats
- [ ] The home crumb is the deepest hidden prefix, with a spec per row of the table above.
      Four specs
- [ ] A hidden segment still appears in every crumb url.
      A spec
- [ ] The current page is never hidden, even when it matches a pattern.
      A spec on a page whose last segment contains a dot
- [ ] A page whose last segment is a file gets no trailing slash on its crumb url.
      A spec
- [ ] `breadcrumbs: false` renders nothing at all.
      A spec asserting an empty string, not an empty `<nav>`
- [ ] The markup is a nav landmark, an ordered list, and `aria-current` on the last crumb.
      A spec asserting all three
- [ ] `link_last: true` links the last crumb and `false` does not.
      A spec for each
- [ ] A label containing `&` and a quote cannot break the markup.
      A spec
- [ ] The README documents the config, the published key, and that anything beyond the five keys is the
      site's own Liquid.
      **By eye**, reading it as someone who has never seen this repository
