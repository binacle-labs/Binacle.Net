---
description: Two framework defaults nobody overrode - code samples on the docs site render in the body sans-serif, and wide tables are clipped rather than scrolled
state: proposed
waits-on: "a yes or a no from the maintainer"
paths:
  - "sites/docs/**"
---

# Two beercss defaults are breaking the docs site

**Found on 22 Aug 2026.** **They are not `www` work** - `www` drops the framework and inherits neither. These
bite `sites/docs`, which keeps it.

## What it is

Two rules in `sites/docs/_sass/`, one line each.

1. **Give `pre` and `code` a mono stack.**
2. **Wrap wide tables in a container that scrolls**, rather than letting `overflow-x: hidden` cut them off.

## Why

**Code blocks are not monospace on any site.** `beer.min.css` sets

```css
pre{...;font-family:inherit}
```

and **no `font-family` declaration exists anywhere in the sass** - not in `sites/docs/_sass/`, not in
`sites/demo/_sass/`, not in `api/src/Binacle.Net.UIModule/_sass/`. So `pre` inherits the body sans-serif and
every fenced block renders in it. **78 files under `sites/docs/collections/` carry fenced code blocks.** It is
the documentation site for an HTTP API; the JSON is most of what a reader is there for.

`sites/demo` and the UI module have no `<pre>` content, so the fault shows on the docs site only.

**Wide tables are clipped, not scrolled.** `beer.min.css` sets `overflow-x: hidden` on **both** `body` and
`main`:

```css
body{color:var(--on-surface);background-color:var(--surface);overflow-x:hidden}
main{flex:1;padding:.5rem;overflow-x:hidden}
```

Nothing overrides either. The four-column table at
`sites/docs/collections/_common_pages/configuration-basics.md:134` loses its right-hand columns on a phone -
**the column holding the connection-string example, which is the reason the table exists** - and there is no
scrollbar to tell the reader anything is missing.

## What it touches

- `sites/docs/_sass/_typography.scss` or a new partial, for the mono stack
- the docs table markup or a wrapper, for the scroll container
- **`sites/docs/` is published**, so this is a site session, not a coding session

## What will bite

**Check the Pygments theme first.** `sites/docs/_sass/pygments/` styles the highlighted spans and may already
assume a face. Setting the stack on `pre` and `code` without reading it can change the colour rules' metrics.

**`overflow-x: hidden` on `body` is doing something.** It is a framework default and something on the docs
site may be relying on it to hide a horizontal overflow elsewhere. **Wrap the table; do not remove the
framework's rule.**

## The case against

**Nobody has complained.** Both faults have been live for the whole life of the site and neither has been
reported.

**And the second one is one table.** Fixing the general case means a wrapper on every table in 78 files or a
Jekyll hook, which is more work than the one table justifies.

## Done when

- [ ] `pre` and `code` render monospace on the built docs site.
      `grep -rn 'font-family' sites/docs/_sass/` returns a mono stack, and **by eye** on a built page with a
      fenced block.
- [ ] The configuration-basics table is reachable at phone width.
      **By eye.** Build the site, open that page at 390px, and scroll the table to its last column.
