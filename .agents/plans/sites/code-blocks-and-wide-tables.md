---
description: Two framework defaults on the docs site - code samples had no named mono face (fixed), and wide tables are still clipped rather than scrolled
state: proposed
waits-on: "a yes or no on wrapping each table in a scroll box - the only route left. State picked to make the file legible; strike it if it is wrong."
paths:
  - "sites/docs/**"
---

# A beercss default is still clipping wide tables on the docs site

**Found on 22 Aug 2026.** **Not `www` work** - `www` drops the framework and inherits neither fault. These
bite `sites/docs`, which keeps it.

The mono stack is in and stays. The table half was tried, made things worse, and is out again. What follows
is what that ruled out and what is left.

## The mono stack - done 2026-08-27

`sites/docs/_sass/_typography.scss` gives `main pre` and `main code` a mono stack.

**It fixed no visible bug and was never going to.** Rouge puts every block's text inside `<code>` and the
browser's own stylesheet renders `code` monospace, so before-and-after screenshots of a fenced block are
identical. What the rule buys is a pinned face instead of each browser's default, and a match with `sites/www`.
**Do not re-measure this.**

## Wide tables - still clipped

`beer.min.css` sets `overflow-x: hidden` on **both** `body` and `main`:

```css
body{color:var(--on-surface);background-color:var(--surface);overflow-x:hidden}
main{flex:1;padding:.5rem;overflow-x:hidden}
```

Nothing overrides either. A table wider than the content column loses its right-hand columns, with no
scrollbar to say anything is missing. The four-column table at
`sites/docs/collections/_common_pages/configuration-basics.md:134` loses the column holding the
connection-string example - **the reason the table exists** - on a phone, and at 1280px too.

Only **8 files** under `sites/docs/collections/` have a table at all. At 390px four of them clip.

## The cheap fix does not work

`table { display: block; overflow-x: auto; }` scrolls the table, and shrinks it. `display: block` puts the
rows in an anonymous table box that sizes to its content instead of filling the column. Measured on the eight
pages that carry tables:

- `forwarded-headers`: 928px, down to 308px.
- `vipaq-protocol`, one table: down to 247px.
- `quick-start` and `health-checks`: down to 574px.

**Three CSS-only alternatives were tried and none works:**

- `min-inline-size: 100%` on the row groups does nothing - width does not apply to a table-row-group.
- `display: table; inline-size: 100%` on `thead`/`tbody` restores the width, but makes head and body two
  separate tables and misaligns every column.
- Nothing else reaches the anonymous table box.

**There is no CSS-only, no-wrapper answer.** It is shrink, or keep the clipping.

## What is left: a wrapper

The plan first floated a wrapper and dismissed it as too big. It is the only route left. A block-level
element around each table takes the `overflow-x: auto`; the table inside stays a table and keeps its width.

**What it costs, honestly:** these are kramdown-generated tables in **8 markdown files across 4 version
folders**, so it needs either markup written into each file or a Jekyll hook that wraps every `<table>` at
build time. Both are more than a line of sass, and the markup route repeats the wrapper 8+ times across
version folders that are otherwise frozen copies.

## What will bite

**Any scroll answer needs a visible resting scrollbar.** Beercss styles scrollbars nearly invisible -
`sites/docs/lib/beercss/beer.css:183` sets `background: none` on the track, thumb and buttons at 0.4rem, and
paints the thumb only on `:hover`/`:focus`. A table that scrolls but shows no scrollbar at rest still reads
as clipped.

**Do not remove the framework's `overflow-x: hidden`.** It is a default on `body` and something else on the
site may rely on it.

**Check the Pygments theme before touching faces.** `sites/docs/_sass/pygments/` styles the highlighted spans
and may assume metrics.

## A separate fault, not this one

**Four pages overflow the document horizontally at 390px for a different reason**, unchanged by anything
here: the breadcrumb `nav.tiny-space` refuses to wrap, and the fixed page-top FAB spills 2px. **Do not fix it
here** - it needs its own plan.

## The case against

**Nobody has complained.** The clipping has been live for the whole life of the site and has never been
reported. The fix now costs a wrapper in 8 files or a build hook, against a fault that bites 4 pages at phone
width and one page at 1280px.

## Done when

**What it takes: a yes or a no, then a Jekyll hook.** The CSS-only routes are all measured and all dead, so
the only remaining shape is a block-level wrapper around each `<table>`.

**Take the hook, not the markup.** A hook wraps every table at build time, in one file, and covers the four
frozen version folders without editing them. Writing the wrapper into the markdown is 8 files across 4
version folders that are otherwise frozen copies, and every new version adds a fifth.

**Whatever is chosen needs a visible resting scrollbar**, or the table reads as clipped exactly as it does
now - see *What will bite*. That is the part that makes this more than one line of sass.

**The no is a real answer.** Nobody has complained in the whole life of the site, it bites four pages at
phone width and one at 1280px, and *The case against* is written for that. **If the answer is no, record it
in the general decisions ledger and delete this file** - leaving it open is the outcome with no value.

- [x] `pre` and `code` render monospace on the built docs site.
      `grep -rn 'font-family' sites/docs/_sass/` returns the stack, and the built `css/main.css` carries
      `main pre,main code{font-family:ui-monospace,...}`. **By eye** on a built page with a fenced block:
      monospace, and unchanged from before - see **The mono stack** above.
- [ ] The configuration-basics table is reachable at phone width, and still fills the column when it fits.
      **By eye.** Built and opened at 390px: the table scrolls to the connection-string column, with a
      scrollbar visible at rest. On a page whose table is narrower than the column - `forwarded-headers` -
      the table still spans the full column width, not 308px.
