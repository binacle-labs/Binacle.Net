---
description: One-liners with a known answer - six of them, across the image, the sites and the shared UI package
state: ready
waits-on: "nothing"
---

# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## The image and the sites

- **A locally built image carries a source map the release image does not.**
  `api/src/Binacle.Net.UIModule/wwwroot/css/main.css.map` is left behind by `npm run watch:css`
  (`package.json:12`, which unlike `build:css` at `:9` passes no `--no-source-map`), and nothing cleans it -
  webpack's `clean` applies to `output.path`, which is `wwwroot/js` only. CI checks out fresh and has none.
- **`api_url` is declared on three sites and read by one.** All three set it in `_config.yml` and
  `_config.prod.yml`; the only reader is `sites/demo/pages/packing.html:26`. Drop it from the two that do not
  read it.
- **The demo pages carry two live flags and only one is documented.** `sites/demo/pages/packing.html` and
  `vipaq.html` set both `applet: true` and `demo: true`. Both are read - `applet` by
  `_includes/navbar/menu.html:1`, `demo` by `_layouts/page.html:11`. `_data/includes.yml:42` documents only
  `demo`. The `_apps` removal, half-done.
- **The instance page prints its runtime as a literal.** `Pages/Instance.cshtml:26` prints `<code>.NET 10</code>`
  as text, on the page whose job is reporting what the container runs. Every other row comes from `@Model`.

## Sites

- **Old-register prose left on the docs site.** Found 2026-08-25, checked again 2026-08-27.
  `sites/docs/collections/_versions/v3.0.x/configuration/ui-module/index.md:20-28` still reads "allows users
  to interact", "Users can navigate" and "enables users to decode"; `sites/docs/pages/index.md:29` still says
  "the algorithms and real-time strategies that power Binacle.Net's packing solutions". **A site session.**

## The shared UI package

- **An import that reads wrong.** `packages/binacle-net-ui/src/core/packingDemo.ts:170` tests
  `error instanceof Error` while `:11` imports `Error` from `../viewModels`. `viewModels/error.ts` declares an
  interface, so the import is erased and the branch tests the global `Error` - correct today. **If that
  interface ever becomes a class it breaks silently.**
