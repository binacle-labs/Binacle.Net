---
description: The docs site's punctuation is inconsistent - en dashes used as list separators in four v3.0.x pages, two curly apostrophes, and a kramdown setting that rewrites straight quotes to curly at build
state: ready
waits-on: "nothing"
paths:
  - "sites/docs/**"
---

# Consistent punctuation on the docs site

**A site session.** `sites/docs/` is published, so a coding session does not touch it.

**This is about how the site looks, not about the plain-ASCII rule.** That rule's `paths:` are `api/src/**`,
`packages/**` and `**/Config_Files/**`, and its reason is consoles, log files, JSON and terminals where the
encoding is not ours to control. A rendered web page declares its encoding, so the rule does not reach here.
**The maintainer wants a consistent look across the site - 2026-08-27.** That is the whole reason this file
exists.

**Three curly apostrophes were fixed in the source on 2026-08-25 and the rendered pages did not change.** That
is why this is a file and not a one-liner: editing the markdown does not settle what the page shows.

## Settle the shape first

`sites/docs/_config.yml:123` sets `smart_quotes : lsquo,rsquo,ldquo,rdquo`. kramdown turns a straight `'`
into a curly one **at build**, so an ASCII source still renders a curly page. Verified 2026-08-27.

**That one line decides whether the site's punctuation is consistent by config or by hand.** Two shapes, and
the work below is the same either way:

- **Turn it off.** The source is then authoritative - what is typed is what renders, and consistency is kept
  by reading the markdown.
- **Leave it on.** kramdown normalises every quote and apostrophe on the site to the same curly forms, and
  the source stops being a reliable picture of the page.

**Pick one before starting.** It is not a blocker on the rest - the en dashes and the two apostrophes need
fixing under either shape - but it decides what the fix means and how it is kept.

## What the source carries

**Two curly apostrophes in `sites/docs/collections/_versions/v3.0.x/`** - counted 2026-08-27, and these two
are the whole set:

- `configuration/service-module/index.md:55` - "the Service Module's architecture"
- `configuration/core/presets.md:107` - "if you don't already have one"

**Thirty-seven en dashes across four files in the same tree**, almost all of them the separator in a
definition list - `` `ServiceNamespace` (_string_) – Defines the namespace ``:

| File | Count |
|---|---|
| `configuration/diagnostics-module/open-telemetry.md` | 15 |
| `configuration/diagnostics-module/packing-logs.md` | 10 |
| `configuration/diagnostics-module/health-checks.md` | 6 |
| `configuration/core/forwarded-headers.md` | 6 |

**`packing-logs.md` was not in the original count and is the same shape**, so a sweep that names three files
leaves a quarter of them behind.

**kramdown does not produce these.** `smart_quotes` covers quotes and apostrophes only; the en dashes are
typed into the source, so replacing them with `-` fixes both the source and the page whichever shape is
chosen.

**Em dashes are a separate list and are not in this count.** `api/v3.md` and `api/v4.md` carry 11 and 12, used
as the separator in term lists - the same job the en dashes do elsewhere, written with a different character.
**That is the inconsistency in one sentence**, and whichever character wins, those two files go with it.

## Done when

- [ ] The `smart_quotes` shape is chosen and written where a later reader will find it, not only here.
      **By eye.** If the answer lives only in this file, the box is open.
- [ ] `sites/docs/_config.yml:123` reflects that choice.
      `grep -n smart_quotes sites/docs/_config.yml` - the line is either gone or deliberately kept.
- [ ] No curly apostrophe is left in the v3.0.x source.
      `grep -rc $'’' sites/docs/collections/_versions/v3.0.x/` returns 0 for every file.
- [ ] One separator character is used across the v3.0.x pages, not three.
      `grep -rc $'–' sites/docs/collections/_versions/v3.0.x/` and the same for `—` - each returns 0 for every
      file, or the count matches a choice written down.
- [ ] The rendered pages match the source.
      **By eye.** Build the site and read `configuration/core/presets.md` and
      `configuration/diagnostics-module/open-telemetry.md` in a browser.
