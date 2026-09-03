---
id: packages
description: TypeScript packages under packages/ (npm workspaces) — UI components, compact-notation mirror, cookie utilities, and theme switching.
verified: 2026-09-04
check: The package list, their descriptions and the private flag match each packages/*/package.json; the Related Tests table names every package under packages/ that has a suite, with the alias tooling/tests.just gives it
also_update:
  - packages/binacle-net-ui
  - sites/demo
  - api/modules/ui
paths:
  - "packages/**"
---

# Packages

npm workspaces at the repo root. All four are `private: true` — none is published to npm, and all four are
TypeScript with no build step of their own: `main` points at a `.ts` entry and each host compiles the source
with its own webpack + ts-loader. `binacle-compact-notation` puts that entry at `src/index.ts`; the other
three keep an `index.ts` barrel at the package root.

| Package | Description |
|---|---|
| `binacle-net-ui` | Alpine.js + Three.js frontend for the packing demo and ViPaq decoder — see `$packages/binacle-net-ui` |
| `binacle-compact-notation` | Compact text notation for Binacle geometry — TS mirror of C# `Binacle.CompactNotation`; used by `binacle-vipaq` (tools/tests) and `binacle-net-ui` (its sample generator) |
| `cookies` | Cookie read/write utility (based on js-cookie v3.0.5, MIT) |
| `theme-switcher` | Light/dark theme switching — the custom element and the pre-paint read |

The ViPaq TypeScript mirror lives at `vipaq/packages/binacle-vipaq/` — see `$vipaq`.

## binacle-net-ui

Alpine.js components + Three.js visualizer for the packing demo and ViPaq decoder. Full reference —
components, plugins, model layers, the `window.binacle` global, and how to add a component — is in
`$packages/binacle-net-ui`. **Consumed as TS source by two hosts**, each with its own webpack config:
`sites/demo/` and the UIModule (`$api/modules/ui`). One implementation, two pages — a change lands on both.

## binacle-compact-notation

TypeScript mirror of the C# `Binacle.CompactNotation` — the shared compact text notation for Binacle geometry
(`"10x10x10 (0,0,0)"` style). A leaf with no dependencies. **Two packages reach it and neither at runtime**:
`binacle-vipaq` in its `tools/` and `tests/` (not runtime `src/`), to parse geometry when generating interop
artifacts and reading shared vectors; and `binacle-net-ui` as a devDependency, in one file —
`tools/generateSamples.ts`.

## cookies

Vendored fork of js-cookie v3.0.5, MIT, kept close to upstream so a re-sync stays cheap. Reached through
`theme-switcher` by all four hosts. No dependencies.

`Cookies` is a static class, not the upstream factory: there is no `withConverter` or `withAttributes`.
Defaults are `path=/`, `expires` 90 days, `sameSite=Lax`, `secure` — **so a caller that takes the defaults on
a plain http page cannot read back what it writes.** The default is right for the sites, which are https; a
caller that also runs inside the image has to decide, and `theme-switcher` is the one that did.

## theme-switcher

Custom HTML element (`<theme-switcher>`) plus the pre-paint read that goes with it. **Used by all four
hosts** — `sites/www`, `sites/demo`, `sites/docs` and the UIModule. Depends on the `cookies` workspace
package; no external dependencies.

**The theme is `data-theme` on the `<html>` element**, never a class on `<body>`. The script that avoids
the wrong-theme flash runs in `<head>`, where `document.body` does not exist yet.

**Five files, one job each.** `theme.ts` is the theme on the document, `options.ts` is what the settings
are and how they are read, `storage.ts` reads and writes one, `themeSwitcher.ts` is the element,
`themeInit.ts` is the pre-paint entry. One barrel, `index.ts`.

**`readStored` stands apart from `createStorage` on purpose, and it is the only thing in the package whose
shape is decided by the bundler.** The pre-paint script imports `readStored` alone; webpack drops
everything it does not reach, which is the whole write side and the `cookies` package with it. Route
reading through `createStorage` and that bundle quadruples. **1.21 KiB minified, 587 bytes gzipped.**

**Each host's pre-paint webpack config overrides ts-loader to `module: 'esnext'`.** The tsconfigs emit
commonjs, which webpack can neither tree-shake nor concatenate — without the override the same bundle is
2.1 KiB and carries a module registry. **It is also what makes a second entry point unnecessary:** the
head bundle imports the ordinary barrel and the switcher element is shaken out.

**There is one reader, not two.** `createStorage`'s `read` calls `readStored`. A second cookie parser was
the thing worth avoiding, not the kilobyte. It does not URI-decode, because only the two literals are ever
accepted and neither survives encoding.

**`mergeOptions` merges `cookie` rather than replacing it.** A caller passing only a domain would otherwise
drop `expires` and silently get the cookies package's 90 days instead of a year.

**A host declares its settings once, as `data-` attributes on `<html>`**, and `optionsFromDocument()`
reads them: `data-default-theme` (`light`, `dark` or `system`), `data-theme-storage`
(`cookie`, `local` or `none`), `data-theme-key`, `data-theme-domain`. One place, or the pre-paint script
and the switcher disagree about the cookie and the page repaints. `configure()` takes the same shape
programmatically, and `storage` also accepts a `ThemeStorage` object.

**`data-default-theme` on the element still wins over the host setting**, but no host uses it: the pre-paint
script cannot read an attribute on an element the parser has not reached.

**The element renders a real `<button>`.** A custom element takes no focus and answers no key, so before
2026-08-24 the theme could not be changed from a keyboard on any host. `data-button-class` puts the host's
class on it. A `[data-theme-label]` child is kept in step as words; an empty element gets the material
ligature the BeerCSS hosts expect.

**It sets `secure` only on an https page**, rather than taking the cookies default. The API image is
commonly served over plain http on a LAN, and a secure cookie is dropped there.

**`data-theme-domain` shares one cookie across `*.binacle.net`**, so a theme chosen on the docs is the theme
on the demo. It comes from `_config.prod.yml` and is absent from `_config.yml`: **a browser drops a cookie
whose domain the host does not match**, so a local build or the self-hosted image must set none. Writing a
domain cookie removes the host-only one first — otherwise both are sent under one name and `Cookies.get`
can return the stale one forever.

**`data-swap` makes something CSS cannot reach follow the theme** — `data-swap="src"` with
`data-lighttheme` and `data-darktheme`. It was `data-theme` until 2026-08-24, which is now the theme itself.
A missing value skips the element; it used to write the string `undefined`.

**The BeerCSS hosts double `:root` in their token selectors on purpose.** BeerCSS stamps `light` or `dark`
on `<body>` itself when it finds neither, from the machine's setting — so a reader who picks light on a dark
machine gets BeerCSS's `body.dark` tokens unless ours outrank them. `:root:root` does; `:root` does not.
`sites/www` carries no BeerCSS and needs none of this.

**The UIModule takes no pre-paint script.** `_Layout.cshtml` reads the cookie server-side and renders the
attribute, which needs no script and no request. Its default comes from `UIModuleOptions.DefaultTheme` so
the server and the browser cannot disagree.

## Related Tests

| Project | What it covers | Run |
|---|---|---|
| `packages/binacle-compact-notation` | the notation parser/formatter, `tests/compactNotation.test.ts` | `just test ts_binacle-compact-notation_unit` |
| `packages/binacle-net-ui` | the randomizer, the view models and every Alpine component bar the visualizer | `just test ts_binacle-net-ui_unit` |
| `packages/cookies` | the converter round trip, get/set/remove, attribute stringifying | `just test ts_cookies_unit` |
| `packages/theme-switcher` | connect, click, the control and its labels, `system`, the swap, the host settings, the pre-paint read, and the cookie over plain http | `just test ts_theme-switcher_unit` |
| `vipaq/packages/binacle-vipaq` | the ViPaq TS mirror, including the shared cross-language vectors | `just test ts_binacle-vipaq_unit` |

The compact-notation alias is filed under **shared**, not packages, because that package mirrors a
`shared/src` C# project; the other three are named after the folder they live in.

**All three new suites run on jsdom**, so their configs add `jest-environment-jsdom` (jest 29 does not
bundle it). `cookies` and `theme-switcher` also point jsdom at an `https` URL, because the cookies defaults
include `secure` and jsdom hides a secure cookie from a document on an insecure origin.

**The origin is set per file**, which is why `theme-switcher` has a second file on `http` — it is the one that
proves the theme survives a reload on an image served over plain http.

**Coverage settings live only in the root `jest.config.js`.** In multi-project mode jest ignores a project's
own `collectCoverageFrom`, and every test runs through the root config with `--selectProjects`. Five package
configs carried a copy until 2026-08-22; all five were dead and one disagreed with the root.

## Dependencies

Which package imports which — every workspace import declared in its `package.json` — is in
`$packages/dependencies`.
