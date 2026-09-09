---
id: packages/binacle-net-ui
description: packages/binacle-net-ui — Alpine.js apps and components plus a Three.js visualizer for the packing demo. The apps/components/shared split, the plugins, and the window.binacle global.
verified: 2026-09-10
check: Every Alpine.data name under src/apps/ and src/components/ appears in the table and vice versa; the two plugins register exactly what is listed; the apps/components/shared split matches src/ and no utils/ or core/ folder exists; apps/packingDemo/sampleData.ts still carries its generated-do-not-edit line and randomize still steps a sampleIndex rather than rolling; packingDemo.ts still reaches the API only through binacle-net-client; the suite/test/coverage figures still match `npx jest --selectProjects binacle-net-ui --coverage`
also_update:
  - packages
paths:
  - "packages/**"
---

# binacle-net-ui

TypeScript package implementing the interactive packing UI as Alpine.js components plus a Three.js 3D visualizer.
Private npm workspace package (`"private": true`, name `binacle-net-ui`).

## Build & consumers — important

This package has **no build step and no bundle of its own** — `"main": "index.ts"` points at raw TypeScript, and
its only script is `test`. Each host compiles it from source with its own webpack + ts-loader.

| Host | Entries | Config |
|---|---|---|
| `sites/demo/` (Jekyll) | `sites/demo/_js/packing_demo.js`, `protocol_decoder.js` | `sites/demo/webpack.config.js` |
| `api/src/Binacle.Net.UIModule` (Razor Pages) | `_js/packing_demo.js`, `_js/protocol_decoder.js` | the module's `webpack.config.js` |

**One implementation, two hosts. A change here lands on both** — that is the point, and it is the rule to test
any proposed feature against: pass data in, never fork the component.

Both configs give this package its own split chunk with the same name and priority, so the chunk set cannot
drift. `three` is bundled from `node_modules` in both, never a CDN.

**Every host must resolve exactly one copy of `three`.** This package imports it and so does each host; if the
two resolve to different directories, webpack bundles both and a mesh built by one fails `instanceof` against
the other. Resolving through the root workspace is what prevents it.

## Public entry points (`index.ts`)

Two aggregate Alpine plugins — these are the only public surface:

| Plugin | Registers |
|---|---|
| `packingDemoPlugin` | `fieldPlugin`, `loggerPlugin`, `packingDemoAppPlugin`, `packingVisualizerPlugin`, `errorsDialogPlugin` |
| `protocolDecoderPlugin` | `loggerPlugin`, `packingVisualizerPlugin`, `protocolDecoderAppPlugin`, `errorsDialogPlugin` |

A host page imports a plugin, calls `Alpine.plugin(...)`, then `Alpine.start()`, and uses the `x-data` names in HTML.

## Apps and components (`src/apps/`, `src/components/`)

| `x-data` name | Factory | Params | What it does |
|---|---|---|---|
| `packing_demo_app` | `packingDemoApp` | `({ baseUrl })` | Form model (bins/items/algorithm), validation, and the sample set. On submit calls **`pack/compare-bins`** through `binacle-net-client`; dispatches `update-scene` / `error-occurred`. Algorithms: FFD/BFD/WFD/Best |
| `protocol_decoder_app` | `protocolDecoderApp` | none | Decodes base64 ViPaq via `binacle-vipaq`'s `ViPaqSerializer.deserialize`; saves to `localStorage` key `ProtocolDecoderSavedResults` |
| `packing_visualizer` | `packingVisualizer` | none | The Three.js scene. Listens for `update-scene`; sets up the scene in `init()` and stores it on `window.binacle`. Playback controls drive items in/out |
| `errors_dialog` | `errorsDialog` | `(default_title)` | Error dialog; `onErrorOccurred(detail)` handles a `string[]` or an `Error` view-model |

Supporting (not `x-data`): `field` (Alpine directive `x-field-prefix` + magics `$fieldId`/`$fieldName` for
hierarchical field names), `logger` (magic `$logger`), `Binacle` (the interface shape of `window.binacle`),
`ControlsManager` (visualizer button state).

**The folder says what a thing is.** `apps/` holds what a page mounts, one folder each, owning the models that
page binds to. `components/` holds what any app may register, one folder each, and a component's folder is
self-contained - its own `index.ts` is its public surface. `shared/` is only what every layer needs.
**There is no `utils/`, and that absence is deliberate** - see `$packages/decisions`.

Cross-component messaging is via Alpine window events: **`update-scene`** (payload is a `() => Promise<{bin,items}>`
or a `DecodedPackingResult`) and **`error-occurred`** (payload `string[]` or `Error`). `packing_visualizer` and
`errors_dialog` are the listeners.

## window.binacle

This package's `packingVisualizer.init()` sets `window.binacle = { rendererContainer, visualizerContainer,
visualizerState }` — exactly the three members of the `Binacle` interface in
`src/components/visualizer/binacle.ts`, all
nullable, declared onto `Window` in `src/types/global.d.ts`. It is **event-driven** — there are no public
`initialize`/`redrawScene` methods.

**Both hosts run this one.** There is no second, imperative `window.binacle` anywhere.

## Model layers (`src/`)

| Folder | Nature | Key types |
|---|---|---|
| `apps/<app>/` | The stateful classes that app's page binds to, beside the component | `Box` (dimension validation, min 1 / max 65535), `Bin`, `Item`, `ErrorCollection` under `packingDemo`; `DecodedPackingResult` under `protocolDecoder` |
| `components/<name>/` | Whatever only that component needs | `Control` under `controls`, `Error` under `errorsDialog`, `VisualizerState` and `Binacle` under `visualizer` |
| `shared/` | Structural interfaces every layer needs | `Coordinates`, `Dimensions`, `Dictionary<T>`, `SceneData`, `defineComponent` |
| `types/` | Ambient declarations only | `alpine.d.ts` (adds `$logger`, `_x_fieldPrefix`), `global.d.ts` (`Window.binacle`) |

**The wire shapes are not here.** They moved to `binacle-net-client`, which owns the v4 request and response
types and the `fetch` that uses them - see `$packages/binacle-net-client`.

**Both files under `types/` are modules, so their `declare global` and `declare module` blocks only apply when
the file is in the compilation** - and nothing imports them. `index.ts` therefore carries a
`/// <reference path>` to each. **Delete those two lines and every host build reports `Window.binacle`,
`$logger` and `_x_fieldPrefix` as missing properties** - 32 errors, which webpack emits through anyway.

**This package's `tsconfig.json` is the one that governs, in every host.** ts-loader walks up from each `.ts`
file it compiles, so it lands here rather than on the host's config - `sites/demo` has no tsconfig at all and
compiles correctly, and an es5 target in a host's config changes nothing. This file's `target: es2016` is what
lets `theme-switcher` emit a real `class`; on es5 a class extending `HTMLElement` throws `Illegal constructor`
at load.

**Both host webpack configs cache to the filesystem, and the cache hides type errors.** Before measuring one,
delete `<host>/node_modules/.cache/webpack` - a warm cache reports success on source that fails cold.

`onSubmit` maps the app's classes into the client's plain request object before the call. **The Three.js scene
helpers are private to the visualizer** - `redrawScene`, `createBin`/`createItem`,
`addItemToScene`/`removeItemFromScene`, the camera helpers, `containerAspectRatio`, `getThemeColors`,
`itemMaterial` - and live inside `src/components/visualizer/` rather than anywhere an app can reach.

## The sample set

**Randomize moves between hand-picked samples; it does not roll new dimensions.**
`apps/packingDemo/sampleData.ts` is **generated** from `shared/data/demo-samples/` by
`just regen demo-samples` and carries a `Do not edit` line. `apps/packingDemo/samples.ts` turns an entry into
`Bin` and `Item` view models, and `packingDemo.ts` holds a `sampleIndex` and steps through them.

`samples.ts` also keeps the rolling helpers `randomBin` and `randomItemFor`, used where a fresh bin is needed
rather than a whole set. Why items are sized against the largest bin, and why `sizingBin` and `addBin` do not
pick the same one, is in the packing-demo design record (`$sites/packing-demo-set`).

## Conventions for adding / modifying a component

- Factory is camelCase (`packingDemoApp`); the `x-data` string is snake_case (`packing_demo_app`); wire them with
  `Alpine.data('snake_name', factory)` in a `*Plugin`. Wrap the factory body in `defineComponent(...)` for typing.
- To add a component to a page, add its `*Plugin` to `src/packingDemoPlugin.ts` or `src/protocolDecoderPlugin.ts`,
  and export it from that component's own `index.ts`.
- **The API call goes through `binacle-net-client`.** `packingDemo.ts` constructs a `BinacleClient` with the
  `baseUrl` it was handed and calls a method; no URL and no wire shape is written here.
- **`packing_demo_app` takes an options object** — `PackingDemoOptions`, `baseUrl` optional. Options rather than
  positional so a second value later is not a signature break.
- **A signature change here lands on both hosts**, and neither can be updated without the other. The way through
  is to widen first, move each host, then narrow: that is how the base URL went from positional to an object on
  2026-08-22 without either page breaking in between.
- No compile here — each host's webpack picks up changes via the workspace symlink.

## Tests

`just test ts_binacle-net-ui_unit`. jsdom, because the components read `document` and `window` even where the
logic under test does not. **20 suites, 350 tests, 70.62% of lines** — measured 2026-09-04.

`tests/model/` is the pure half — the samples, the view models, `ControlsManager`. `tests/components/` is
the Alpine half: each component factory is a plain object, so a test calls it directly with a stub `$dispatch`
and `$logger` rather than starting Alpine.

**What is uncovered is the Three.js half, and that is the intended answer** — `core/packingVisualizer.ts` and
the scene helpers in `utils/` need a WebGL context, so a test there could only assert that a call happened.
They stay in the coverage denominator rather than being excluded from it: the number is meant to show the gap,
not hide it. Only `.d.ts` files are excluded, in the root config, because they carry no runtime code.

**`three` ships `OrbitControls` as ESM only**, which the commonjs transform cannot load. Importing either
plugin barrel pulls the visualizer in and hits it, so `jest.config.js` maps it to `tests/stubs/orbitControls.ts`.
Nothing under test constructs one.
