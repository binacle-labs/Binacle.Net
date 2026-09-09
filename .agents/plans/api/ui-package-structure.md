---
description: binacle-net-ui has no layers - 19 of its 25 utils are visualizer internals sitting in a shared bag. Split it into apps, components and plugins
state: ready
waits-on: "nothing. horizon: now - taken from the maintainer's instruction to start it, strike it if wrong"
horizon: now
paths:
  - "packages/binacle-net-ui/**"
---

# binacle-net-ui - split it into apps, components and plugins

`packages/binacle-net-ui` is three flat folders - `core/`, `utils/`, `models/` - and none of them is a layer.
A second app cannot be added without reading every file to find out what it is allowed to touch.

## The finding, measured 2026-09-09

**Of the 25 files in `src/utils/`, 19 are used only by `packingVisualizer.ts` or by another visualizer util.**
The scene helpers, camera maths, materials, origins and loading state are the visualizer's implementation,
sitting in a folder any file may import from.

The other six:

| File | Real owner |
|---|---|
| `defineComponent` | shared - all four components use it |
| `findClosestElement` | `field` only |
| `getResponseStatusText` | `packingDemo` only |
| `samples`, `sampleData`, `getRandomInt` | `packingDemo`'s sample data |

So there is no shared utility layer to keep. There is a visualizer with its guts spread where anything can
reach them, which is the real cost: nothing stops an app importing visualizer internals, because they look
like general helpers.

`src/core/` flattens the same way. It holds two apps, five components, and `binacle.ts`, which is not a
component at all - it is the contract type between an app and the visualizer.

## The three kinds

| Kind | What it is | Today |
|---|---|---|
| App | what a page mounts, one per bundle entry | `packingDemo`, `protocolDecoder` |
| Component | a reusable Alpine component any app registers | `packingVisualizer`, `errorsDialog`, `field`, `controlsManager`, `logger` |
| Plugin | the bundle entry - one app plus the components it needs | `packingDemoPlugin.ts`, `protocolDecoderPlugin.ts` |

**Keep the word "plugin" for the entry only.** Every component also exports a `*Plugin` function today, which
is why the word stopped carrying meaning. The Alpine registration function can keep its name inside the
component; what changes is that the folder layout no longer calls five different things a plugin.

## The target

```
src/
  apps/packingDemo/        the component, its view models, its samples
  apps/protocolDecoder/
  components/visualizer/   the component and the 19 files that are its internals
  components/errorsDialog/
  components/field/
  components/controls/
  components/logger/
  shared/                  defineComponent, the Binacle contract type, base models
  packingDemoPlugin.ts
  protocolDecoderPlugin.ts
```

**The visualizer is the point of the whole move.** Both apps use it and more will. Giving it a folder makes
its public surface two things - the component and the `Binacle` contract type - and makes the other 19 files
private to it. A third app then consumes it without being able to reach inside.

## What will bite

**The package's public surface must not change.** `index.ts` at the package root exports exactly
`packingDemoPlugin` and `protocolDecoderPlugin`, and carries two `/// <reference path=...>` lines for the
ambient declarations in `src/types/`. Both hosts import the package by name and get those two exports.
Change either and both hosts break at build time, not test time.

**The tests are the whole safety net.** 348 of them across 20 suites pass before the move and must pass
after. Nothing in this work changes behaviour, so any test that needs its assertions edited is a signal that
something was moved wrong - not a test to fix.

**`tests/` mirrors the old shape.** It splits into `components/` and `model/`, which stops matching the
source the moment the source moves.

**Barrel files hide the blast radius.** `src/utils/index.ts`, `src/core/index.ts`, `src/models/index.ts` and
`src/viewModels/index.ts` re-export almost everything, so most imports go through a barrel rather than naming
a file. Deleting a barrel and letting the compiler find every real importer is what makes the move
reviewable; keeping them hides which app depends on what, which is the thing being fixed.

**`_itemMaterial` is named for the folder it is escaping.** The underscore marks it private to `utils/`. Once
it sits inside the visualizer, the underscore says nothing.

## Done when

- [ ] No file in the package is imported by two apps unless it lives under `shared/` or `components/`.
      **By eye**, walking the import list of each app folder. A visualizer helper reachable from an app
      folder means the box is open.
- [ ] The visualizer's 19 internals live with it.
      `ls packages/binacle-net-ui/src/utils` does not exist as a path, and
      `ls packages/binacle-net-ui/src/components/visualizer` lists them.
- [ ] The package still exports exactly the two plugins, with the two ambient references intact.
      `cat packages/binacle-net-ui/index.ts` shows both `export` lines and both `/// <reference` lines.
- [ ] Every test still passes, with no assertion edited.
      `just test ts_binacle-net-ui_unit` reports 348 passed, and
      `git diff --stat packages/binacle-net-ui/tests` shows moves and import rewrites only.
- [ ] The type checker agrees.
      `npx tsc --noEmit -p packages/binacle-net-ui` prints nothing.
- [ ] Both hosts still build.
      **He runs it.** Neither bundle is rebuilt by this work, and the demo host's output is not ours to write.
