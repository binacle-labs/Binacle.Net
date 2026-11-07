# Packages

The generic TypeScript packages: they know nothing about Binacle, so any site could use them. Code that knows
Binacle lives with the slice it belongs to - the API clients in [`api/packages`](../api/packages), ViPaq in
[`vipaq/packages`](../vipaq/packages), and what several slices share, the design system and the apps among it,
in [`shared/packages`](../shared/packages).

They are npm workspaces; install once from the repo root with `npm install`. A package here is imported by its
folder name, with no scope - `theme-switcher` for `theme-switcher/`.

## 📦 Packages

| Package | What it is |
|---|---|
| [`cookies`](cookies) | Read, write and remove cookies. A copy of js-cookie v3.0.5, changed a little |
| [`theme-switcher`](theme-switcher) | Auto, light and dark theme button, as a `<theme-switcher>` custom element, and the script that sets the theme before the first paint. Uses `cookies` |

Both are MIT. Every package here has tests. From the repo root:

```
just test ts_cookies_unit              # cookies
just test ts_theme-switcher_unit       # theme-switcher
```

## 🌐 Who imports them

The design system in [`shared/packages/design-system`](../shared/packages/design-system) uses `theme-switcher`
for its theme button. No site imports either one directly.

**None of them has a build step.** Each host compiles the TypeScript from source with its own webpack and
ts-loader, so a change here lands on every host at once. That is also why an edit here is only proven by
building the hosts, not by building the package.

## ⚠️ What will bite you

**Both names are taken on the public npm registry** by other libraries. They resolve here only because they are
workspaces. Do not publish either under its bare name.
