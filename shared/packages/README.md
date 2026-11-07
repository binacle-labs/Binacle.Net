# Shared packages

The TypeScript packages that more than one slice reads. Each is an npm workspace, imported by its npm name,
`@binacle/` and the folder name. Install once from the repo root with `npm install`.

## 📦 Packages

| Package | What it is | Read by |
|---|---|---|
| [`compact-notation`](compact-notation) | `@binacle/compact-notation`. Parses and writes the compact strings for bins and items, the twin of the C# parser in [`shared/src`](../src). Apache-2.0 | ViPaq, the apps' tests |
| [`design-system`](design-system) | `@binacle/design-system`. The look the sites share: tokens, fonts, the mark, the 3D view, and each block's Sass, behaviour and markup in one folder | `www`, `docs`, `demo`, the UI module |
| [`apps`](apps) | `@binacle/apps`. The interactive apps - *Everyday Items*, the packing demo and the ViPaq decoder - as Alpine.js plugins | `demo`, the UI module |

`apps` uses `design-system`; `design-system` uses nothing from `apps`.

From the repo root:

```
just test ts_binacle-compact-notation_unit
just test ts_binacle-design-system_unit
just test ts_binacle-apps_unit
```

## ⚠️ What will bite you

**None of them has a build step.** Each host compiles the TypeScript from source with its own webpack and
ts-loader, so an edit here is only proven by building the hosts that read it.
