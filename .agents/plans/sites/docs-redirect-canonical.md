---
description: sites/docs/_layouts/redirect.html is the last head on any site that writes its own metadata. It can only move onto the tag once something stamps the canonical it needs.
state: ready
waits-on: "nothing"
---

# The redirect layout writes its own head

**One layout and one page.** `sites/docs/_layouts/redirect.html` renders
`collections/_common_pages/version-latest.html` at `/version/latest/`, and it composes its own title,
canonical and `robots` inline. Every other head on all three sites is `{% page_meta %}`.

**It was left out of the switchover on purpose.** Its canonical does not point at itself - it points at
`/version/<versions.current>/`, the page it redirects to, which is what tells a crawler the two are one
destination. Front matter cannot carry that value, because it changes when `current` moves.

## What to build

**Stamp the two keys in `binacle-docs-versions`**, which already owns `versions.current` and already stamps
`robots` and `title_suffix`:

- `canonical` - the absolute url of `/version/<current>/`, on a page whose layout is `redirect`.
- `robots` - `noindex`, which the layout hardcodes today.

Then delete the inline logic from the layout and call `{% page_meta %}`, the way
`_layouts/versions/swagger.html` does. The redirect itself - the `<script>`, the `<meta http-equiv>` and the
visible link - stays exactly as it is.

**The generator must not overwrite a key the page set**, the same rule the other two stamps follow.

## What will bite

**The canonical is the whole point of the page.** Get it wrong and `/version/latest/` competes with
`/version/v3.0.x/` for the same content instead of pointing at it. Check the built page, not the spec.

**`{% page_meta %}` writes the OpenGraph and Twitter blocks too**, which this page has never had. Harmless on
a `noindex` redirect, and it will be in the diff.

**The layout has a `<base href=".">`.** Nothing in the tag emits a relative url, but any future change that
does would resolve against it.

## Done when

- [ ] Nothing under `sites/` composes its own title, description or canonical.
      `grep -rn "assign page_title" sites/` finds nothing
- [ ] The built `/version/latest/` page carries the same canonical it carries today.
      Build docs from the previous commit and compare the `<link rel="canonical">` on that one page
- [ ] Moving `versions.current` moves the canonical with it.
      A spec on the stamp, and by eye on a scratch build
- [ ] The page is still `noindex` and still redirects.
      **By eye** on the built page: the meta refresh, the script and the visible link are unchanged
