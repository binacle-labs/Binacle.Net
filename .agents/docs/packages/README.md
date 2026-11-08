---
id: packages
description: The TypeScript npm workspaces - where each package lives, and the two generic ones under packages/, cookies and theme-switcher, in full.
verified: 2026-11-09
check: The package table matches the workspaces globs in the root package.json and each package.json name; packages/ holds only cookies and theme-switcher; theme-switcher's only importer is the design system; the head bundle reaches it through @binacle/design-system/theme-init; the jsdom and https notes match each jest.config.js; the test aliases match tooling/tests.just
also_update:
  - shared/apps
  - api/api-client
  - sites/demo
  - api/modules/ui
paths:
  - "packages/**"
---

# Packages

npm workspaces at the repo root. Every one is `private: true`, and every one is TypeScript with no build step
of its own: `main` points at an `index.ts` at the package root, and each host compiles the source with its own
webpack and ts-loader.

**`packages/` holds only the generic packages**, the ones that know nothing about Binacle. The others sit with
the slice they belong to and take the `@binacle/` scope:

| Package | Folder | Doc |
|---|---|---|
| `cookies` | `packages/` | below |
| `theme-switcher` | `packages/` | below |
| `@binacle/api-client` | `api/packages/` | `$api/api-client` |
| `@binacle/service-client` | `api/packages/` | **Experimental, local only.** Its one host is `sites/admin`, which nothing builds or deploys |
| `@binacle/compact-notation` | `shared/packages/` | its README |
| `@binacle/design-system` | `shared/packages/` | `$shared/design-system` |
| `@binacle/apps` | `shared/packages/` | `$shared/apps` |
| `@binacle/vipaq` | `vipaq/packages/` | `$vipaq` |

Which imports which is `$packages/dependencies`.

## cookies

A copy of js-cookie v3.0.5, kept close to upstream so a re-sync stays cheap. Its only importer is
`theme-switcher`. No dependencies.

`Cookies` is a static class, not the upstream factory. Defaults are `path=/`, `expires` 90 days,
`sameSite=Lax`, `secure` - **so a caller that takes the defaults on a plain http page cannot read back what it
writes.** That suits the sites, which are https; a caller that also runs inside the image has to decide, and
`theme-switcher` is the one that did.

## theme-switcher

The `<theme-switcher>` element and the pre-paint read. Depends on `cookies`. **Its only importer is the design
system**, which defines the element in `initThemeButton` and gives the head script its own export path,
`@binacle/design-system/theme-init`. The four hosts reach it only through that.

**`readStored` stands apart from `createStorage` on purpose, and it is the only thing in the package whose
shape is decided by the bundler.** The pre-paint script reaches `readStored` alone, and webpack drops the
write side and the `cookies` package with it. Route reading through `createStorage` and both come back.

**Two things let webpack drop them.** The host tsconfigs emit ES modules, and ts-loader compiles the package
with the host's config, not the package's own commonjs one. And `theme-switcher`, `cookies` and the design
system all declare `sideEffects: false`. The design system's `theme-init` path re-exports `initTheme` alone,
so the head bundle never reads the design system's index, which pulls in three.js.

**There is one reader, not two.** `createStorage`'s `read` calls `readStored`. A second cookie parser was
the thing worth avoiding. It does not URI-decode, because only the two theme names are ever accepted and
neither changes when encoded.

**`mergeOptions` merges `cookie` rather than replacing it.** A caller passing only a domain would otherwise
drop `expires` and silently get the cookies package's 90 days instead of a year.

**A host declares its settings once, as `data-` attributes on `<html>`**, and `optionsFromDocument()`
reads them: `data-default-theme` (`light`, `dark` or `system`), `data-theme-storage`
(`cookie`, `local` or `none`), `data-theme-key`, `data-theme-domain`. One place, or the pre-paint script
and the button disagree about the cookie and the page repaints. `ThemeSwitcherButtonElement.configure()`
takes the same shape in code, and its `storage` also accepts a `ThemeStorage` object.

**`data-default-theme` on the element still wins over the host setting**, but no host uses it: the pre-paint
script cannot read an attribute on an element the parser has not reached.

**A click goes auto, light, dark.** Auto removes `data-theme` and the stored choice, and no attribute is how
the stylesheet follows the system. The word `auto` is never stored, so a host default of `light` or `dark`
comes back on the next load.

**It sets `secure` only on an https page**, rather than taking the cookies default. The API image is
commonly served over plain http on a LAN, and a secure cookie is dropped there.

**`data-theme-domain` shares one cookie across `*.binacle.net`**, so a theme chosen on the docs is the theme
on the demo. It comes from `_config.prod.yml` and is absent from `_config.yml`: **a browser drops a cookie
whose domain the host does not match**, so a local build or the self-hosted image must set none. Writing a
domain cookie removes the host-only one first - otherwise both are sent under one name and `Cookies.get`
can return the stale one forever. Going to auto removes both.

**`data-swap` makes something CSS cannot reach follow the theme** - `data-swap="src"` with
`data-lighttheme` and `data-darktheme`. A missing value skips the element.

**The UIModule takes no pre-paint script.** `_Layout.cshtml` reads the cookie server-side and renders the
attribute, which needs no script and no request. Its default comes from `UIModuleOptions.DefaultTheme` so
the server and the browser cannot disagree.

## Related Tests

| Project | What it covers | Run |
|---|---|---|
| `packages/cookies` | the converter round trip, get/set/remove, attribute stringifying | `just test ts_cookies_unit` |
| `packages/theme-switcher` | connect, click, the button and its labels, `system`, the swap, the host settings, the pre-paint read, and the cookie over plain http | `just test ts_theme-switcher_unit` |

**Both run on jsdom and point it at an `https` URL**, because the cookies defaults include `secure` and jsdom
hides a secure cookie from a page on an insecure origin. **The origin is set per file**, which is why
`theme-switcher` has a second file on `http` - it is the one that proves the theme survives a reload on an
image served over plain http.

**Every TypeScript workspace's tests run through the root `jest.config.js`**, with `--selectProjects`, and
the coverage settings live only there: in multi-project mode jest ignores a project's own
`collectCoverageFrom`.
