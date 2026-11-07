# apps

The front end behind the interactive apps on the website - *Everyday Items*, the packing demo and the ViPaq
protocol decoder. Alpine.js components, written in TypeScript. The 3D view they drive is in
[`design-system`](../design-system); this package holds its Alpine wrapper.

It exports three Alpine plugins and nothing else:

```ts
import {everydayItemsPlugin} from '@binacle/apps';
import {packingDemoPlugin} from '@binacle/apps';
import {protocolDecoderPlugin} from '@binacle/apps';
```

Each one registers the set of Alpine components its page needs, so a page turns on with a single plugin call.

## 📂 What is in it

**Three kinds of thing.** An *app* is what a page mounts, and has its own folder in `src/`. A *component* is a
reusable Alpine component any app can register, in `src/components/`. A *plugin* is the bundle entry that puts
one app together with the components it needs; each app keeps its own, as `plugin.ts`.

| Folder | What it is |
|---|---|
| `src/everydayItems/`, `src/packingDemo/`, `src/protocolDecoder/` | One folder per app. Each owns its plugin, its own component, the models its page binds to, and anything only it uses |
| `src/components/` | The reusable components - `packView`, the 3D view's wrapper, then `errorsDialog`, `field`, `logger`. Each folder is self-contained and exports its public surface from its own `index.ts` |
| `src/shared/` | The few things every layer needs - `defineComponent` and `dictionary` |
| `src/types/` | Ambient declarations for Alpine and the globals the pages set |
| `tools/` | The sample-set generator. `just regen demo-samples` reads `shared/data/demo-samples` and writes `src/packingDemo/sampleData.ts`, which is generated - edit the data, not the file |

**An app talks to the 3D view through its wrapper**, `src/components/packView/`, never to the view itself. It
dispatches `update-scene` with a function that returns a promise of a scene:

| Scene | What the view draws |
|---|---|
| `{bin}` | The bin alone, before any answer |
| `{bin, items}` | An answer. Items are the API's shape, with an optional `id` and `kind` |
| `{bin, items: [], note}` | An answer with nothing in it. The note shows under the view |
| `null`, or no `bin` | Nothing |

`kindNames` goes beside `items`, and the readout names each item by it. The loader shows while the promise runs.
Dispatching `highlight-kind` with a kind shows only that kind in colour; `null` shows them all again.

The figure's markup and colours are the host's: [`design-system`](../design-system) says what the view needs.

## 🚀 Where it runs

**Two hosts, one implementation.** Both compile it from source with their own webpack and ts-loader; the
import resolves through the npm workspace, so nothing is copied and a change here shows up on the next
webpack pass of each.

| Host | Entries |
|---|---|
| [`sites/demo`](../../../sites/demo) | `_js/everyday_items.js`, `_js/packing_demo.js`, `_js/protocol_decoder.js` |
| [the API's UI module](../../../api/src/Binacle.Net.UIModule) | `_js/packing_demo.js`, `_js/protocol_decoder.js` |

`just serve demo` watches the site; `just serve api U` watches the module. **A change here lands on both, and
neither can be updated without the other** - so widen a signature first, move each host, then narrow.

*Everyday Items* and the packing demo call a live API through [`@binacle/api-client`](../../../api/packages/api-client), which owns the wire
shapes. The site takes the address from `api_url` in its `_config.yml`; the module passes an empty one, which
means fetch relative from whatever host served the page. The decoder calls nothing
- it decodes in the browser, through `@binacle/vipaq`.

## 🧪 Tests

```bash
just test ts_binacle-apps_unit
```

jsdom, because the components read `document` and `window` even where the logic under test does not.
`tests/` mirrors `src/`, so a test sits beside the thing it covers. The component tests call each Alpine
component factory directly with a stub `$dispatch`, rather than starting Alpine.
