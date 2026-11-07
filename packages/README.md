# Packages

The TypeScript packages that belong to no one slice: the sites' shared parts. A package that belongs to a
slice lives in that slice's own `packages/` folder - the API clients in [`api/packages`](../api/packages),
compact notation in [`shared/packages`](../shared/packages), ViPaq in [`vipaq/packages`](../vipaq/packages).

They are all npm workspaces; install once from the repo root with `npm install`. A package is imported by its
npm name, which is `@binacle/` and the folder name - `@binacle/theme-switcher` for `theme-switcher/`.
`binacle-components` and `binacle-net-ui` keep their old names: the first forwards to `design-system`, the second becomes `apps`.

## 📦 Packages

| Package | What it is |
|---|---|
| [`binacle-net-ui`](binacle-net-ui) | Alpine.js components and a Three.js visualizer for the interactive packing demo |
| [`theme-switcher`](theme-switcher) | `@binacle/theme-switcher`. Auto, light and dark theme button, as a `<theme-switcher>` custom element. Holds its own copy of js-cookie |
| [`binacle-components`](binacle-components) | Forwards to `design-system`, for the sites that still import from here |
| [`design-system`](design-system) | `@binacle/design-system`. The look the sites share: tokens, fonts, the mark, and each block's Sass, behaviour and markup in one folder |

Every package here but `binacle-components` has tests. From the repo root:

```
just test ts_binacle-net-ui_unit       # binacle-net-ui
just test ts_theme-switcher_unit       # theme-switcher
just test ts_binacle-design-system_unit   # design-system
```

## 🌐 Who imports them

The sites and the API's UI module pull them in by package name and webpack bundles them:
[`sites/demo`](../sites/demo) and [the UI module](../api/src/Binacle.Net.UIModule) use `binacle-net-ui` for
the packing demo and the ViPaq decoder, `docs` and `demo` use `theme-switcher` and `binacle-components`,
[`sites/www`](../sites/www) uses `design-system`, which uses `theme-switcher`, and the local-only [`sites/admin`](../sites/admin) uses `@binacle/service-client`.
Nothing is copied: the import resolves through the workspace. (`just assets` is a different job - it copies the static files in
[`assets/`](../assets), not these.)

**None of them has a build step.** Each host compiles the TypeScript from source with its own webpack and
ts-loader, so a change here lands on every host at once and none of them can be updated on its own. That is
also why an edit here is only proven by building the hosts, not by building the package.
