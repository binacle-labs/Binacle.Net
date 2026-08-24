# jekyll-breadcrumb-trail

Works out a page's breadcrumb trail once, publishes it as page data, and renders it as the markup the ARIA
authoring practices specify. It replaces the thirty lines of nested path-splitting Liquid that a Jekyll site
otherwise copies into an include - and then copies again, with a small difference, for one section.

This is not the published `jekyll-breadcrumbs` gem. This one publishes the trail as data before it renders
anything, so a structured data plugin can build a `BreadcrumbList` from the same values the visible trail
shows. Two implementations of one trail are free to disagree, and nothing fails when they do.

## 📂 What is in it

| Surface | Does |
|---|---|
| the generator | resolves `page.breadcrumb_trail` - a list of `name` and `url`, the first crumb first |
| `{% breadcrumbs %}` | renders the nav from that list, and works nothing out |

`name` and `url` are the words schema.org's `ListItem` uses, because that is what they usually become.

## 🚀 Quick start

Both lines are needed. Miss the first and bundler cannot find the gem. Miss the second and the tag is never
registered, so Liquid raises `Unknown tag 'breadcrumbs'` and the build fails.

```ruby
# Gemfile, inside group :jekyll_plugins
gem "jekyll-breadcrumb-trail", path: "path/to/jekyll-breadcrumb-trail"
```

```yaml
# _config.yml
plugins:
  - jekyll-breadcrumb-trail
```

Then put the tag where the trail goes, usually in a header include or a layout:

```liquid
{% breadcrumbs %}
```

Every setting has a default, so that is enough to get a working trail.

## ⚙️ Configuration

```yaml
breadcrumbs:
  exclude: []                        # globs, matched against one path segment
  title_from: [crumbtitle, title]    # front matter keys, in order, for the current page's label
  link_last: false                   # is the current page a link
  label: breadcrumb                  # the nav's aria-label
  class: ''                          # an extra class on the <nav>
  home:
    title: Home
    html: ''                         # replaces the title inside the home link when set
```

| Key | Default | What it does |
|---|---|---|
| `exclude` | `[]` | segments whose label is left out of the trail. Globs, matched one segment at a time |
| `title_from` | `[crumbtitle, title]` | front matter keys tried in order for the current page's label |
| `link_last` | `false` | when false the current page is text, not a link. Both are conformant |
| `label` | `breadcrumb` | the `aria-label` on the nav landmark. Change it for another language |
| `class` | none | an extra class on the `<nav>`. The list and item classes are always written |
| `home.title` | `Home` | the first crumb's label |
| `home.html` | none | raw html rendered inside the home link instead of the label - an icon, usually |

Front matter, per page:

| Key | Does |
|---|---|
| `breadcrumbs: false` | no trail on this page, and the tag renders nothing at all |
| `crumbtitle` | a label for this page's crumb, when its `title` is too long for one |

The config is closed at those keys on purpose. Wrapper elements, per-crumb attributes and aria overrides are
not settings - a site that wants them renders `page.breadcrumb_trail` in its own Liquid, which is the whole
reason the key is published.

## 🏷️ What it renders

```html
<nav aria-label="breadcrumb">
<ol class="breadcrumb">
<li class="breadcrumb-item"><a href="/guides/">Guides</a></li>
<li class="breadcrumb-item"><a href="/guides/getting-started/">Getting Started</a></li>
<li class="breadcrumb-item active" aria-current="page">Quick start</li>
</ol>
</nav>
```

A navigation landmark with a label, an ordered list, and the current page carrying `aria-current="page"`.
A run of anchors separated by slashes, with no list, reads to a screen reader as a row of unrelated links.

The class names are Bootstrap's. They are inert on a site that does not style them, and a site that does use
Bootstrap gets a working trail with no CSS of its own. There is no separator setting because the standard
markup draws the separator in CSS, not in the list.

## 🧭 How the trail is worked out

The trail comes from the page's url, one crumb per segment. Labels for the segments above the current page
are the segment with hyphens turned into spaces, any `.html` dropped, and every word capitalized.

**`exclude` suppresses a label, never a url.** The segment keeps its place in every crumb below it, so the
trail a crawler follows is real.

**The home crumb points at the deepest excluded segment above the current page**, not at the site root. On a
site whose sections live under `/section/v1.0.x/` with both segments excluded, a page inside that section
gets a trail starting at `/section/v1.0.x/`. Walking further up would offer a reader a link out of the
section they are reading.

| Page url | `exclude` | Home points at |
|---|---|---|
| `/section/v1.0.x/getting-started/quick-start.html` | `["section", "*.*"]` | `/section/v1.0.x/` |
| `/section/v1.0.x/` | `["section", "*.*"]` | `/section/` |
| `/getting-started/` | `["section", "*.*"]` | `/` |
| anything | `[]` | `/` |

**The current page is never excluded**, whatever it matches. Row two is that rule: `v1.0.x` matches `*.*`
and it is still the crumb you are standing on.

## ⚠️ What will bite

**A page with no path segments gets no trail.** The site root renders nothing rather than a lone home crumb.

**A crumb for a file carries no trailing slash.** The rebuilt path gets one only where the page's own url
has one. An include doing this by hand usually appends a slash to everything, which is invisible while the
last crumb is unlinked and a 404 the moment it goes into markup.

**Labels are capitalized one word at a time, so an acronym comes back mixed** - `api-reference` is
`Api Reference`. Set `crumbtitle` on the page, or name the folder what you want to read.

**The generator runs at a low priority**, so it sees keys another plugin stamped. Nothing needs to run after
it: `page.breadcrumb_trail` is read at render time.

**Everything is escaped except `home.html`.** That one is raw by definition - it is there to hold an icon.
