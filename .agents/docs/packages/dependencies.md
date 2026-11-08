---
id: packages/dependencies
description: TypeScript packages dependency tree - the npm workspaces and which package imports (and declares) which.
verified: 2026-11-09
check: the workspaces globs in the root package.json still cover every package; each package.json dependencies block matches the graph; a grep for non-relative `from "..."` across every workspace package turns up nothing the graph does not name
paths:
  - "packages/**"
  - "api/packages/**"
  - "shared/packages/**"
  - "vipaq/packages/**"
---

# Packages - TypeScript dependencies

The npm workspaces. Root `package.json` covers `packages/*`, `api/packages/*`, `shared/packages/*`,
`vipaq/packages/*`, and the hosts - `api/src/Binacle.Net.UIModule`, `sites/docs`, `sites/demo`, `sites/www`
and `sites/admin`. Every package resolves the others from the workspace. Every workspace import is also
declared in its importer's `package.json` (all as `"*"`), so the graph is honest on its own, not only by
hoisting.

## The graph

Arrows point at what a package imports.

```
@binacle/apps                         + external: alpinejs
   ├──► @binacle/api-client           leaf
   ├──► @binacle/vipaq                mirror of the C# ViPaq
   │       └──► @binacle/compact-notation    in tools/ and tests/ only, not runtime src
   ├──► @binacle/compact-notation     dev only, in tools/ and tests/
   └──► @binacle/design-system        + external: three
           └──► theme-switcher
                   └──► cookies       leaf

@binacle/service-client               leaf; only sites/admin imports it
```

**Who imports the packages:** `www` and `docs` import the design system; `demo` and the UIModule import
the design system and the apps; `sites/admin` imports the service client.

## Packages at a glance

| Package | Folder | Imports | External deps |
|---|---|---|---|
| `cookies` | `packages/` | - | - |
| `theme-switcher` | `packages/` | `cookies` | - |
| `@binacle/compact-notation` | `shared/packages/` | - | - |
| `@binacle/design-system` | `shared/packages/` | `theme-switcher` | `three`, and its `three/examples/jsm` add-ons |
| `@binacle/apps` | `shared/packages/` | `@binacle/api-client`, `@binacle/design-system`, `@binacle/vipaq`; `@binacle/compact-notation` (dev) | `alpinejs` |
| `@binacle/vipaq` | `vipaq/packages/` | `@binacle/compact-notation` | - |
| `@binacle/api-client` | `api/packages/` | - | - |
| `@binacle/service-client` | `api/packages/` | - | - |

## Notes

1. **Every workspace import is declared** as `"*"` in the importer's `package.json`, so resolution never
   relies on workspace hoisting.

2. **Two packages reach `@binacle/compact-notation`, and neither does it at runtime.** `@binacle/vipaq`
   touches it only in `tools/` and `tests/`, not in runtime `src/` - it parses the shared compact-geometry
   notation when generating interop artifacts and reading vectors, and the format itself pulls in nothing
   (`$vipaq/typescript`). `@binacle/apps` declares it as a **devDependency** and imports it only in `tools/`
   and `tests/`.

   **`@binacle/vipaq` declares it as a runtime `dependency`, which is what makes npm's graph look cyclic.**
   Moving it to `devDependencies` is a known one-line change nobody has made.

3. **The apps reach three.js only through the design system**, and declare no `three` of their own. The 3D
   view is the design system's.

4. **`cookies`, `@binacle/compact-notation` and the two clients are leaves** - no workspace or runtime deps.
   The clients' only extra is `ajv`, a devDependency for the contract test.

5. **Only `@binacle/apps` and `@binacle/design-system` have runtime externals.** Everything else declares
   only devDependencies (jest, ts-jest, typescript, type packages) or nothing at all. The root `package.json`
   carries no runtime dependency either - its `concurrently` and `gulp` are build tooling (`$build-topology`).
