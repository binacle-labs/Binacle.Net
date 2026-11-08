---
id: shared/apps
description: shared/packages/apps (@binacle/apps) - the Alpine.js apps behind Everyday Items, the packing demo and the ViPaq decoder. The names the hosts write by hand, the events, the call seam, and what each host build must set.
verified: 2026-11-08
check: The x-data, directive and magic names in the table match what the three plugin.ts files register and tests/plugins.test.ts asserts; the same names appear in sites/demo/pages, sites/demo/_includes and the UI module's Pages; the event names match the listeners in sites/demo/_includes/pack-view.html, errors-dialog.html and the module's _PackView.cshtml and _ErrorsDialog.cshtml; both host webpack configs still override ts-loader's module and still name the binacle-net-ui chunk; jest.config.js still maps the three ESM-only add-ons to tests/stubs/
also_update:
  - api/modules/ui
  - sites/demo
paths:
  - "shared/packages/apps/**"
---

# apps

`@binacle/apps`, in `shared/packages/apps/`. The package README holds its layout, the scene shapes the 3D view
takes, its two hosts and its test command. This page holds what an edit can break without a test failing in
the package.

## The names the hosts write by hand

Each app's `plugin.ts` registers the app and the components its page needs. The hosts name them in markup, so
a rename here breaks a page and no TypeScript check sees it.

| Name | Kind | Registered by | Mounted in |
|---|---|---|---|
| `everyday_items_app` | `x-data`, takes `({baseUrl})` | `everydayItemsPlugin` | `sites/demo/pages/items.html` |
| `packing_demo_app` | `x-data`, takes `({baseUrl})` | `packingDemoPlugin` | `sites/demo/pages/packing.html`, the module's `Packing.cshtml` |
| `protocol_decoder_app` | `x-data` | `protocolDecoderPlugin` | `sites/demo/pages/vipaq.html`, the module's `Vipaq.cshtml` |
| `pack_view` | `x-data`, the 3D view's wrapper | all three | `sites/demo/_includes/pack-view.html`, the module's `_PackView.cshtml` |
| `errors_dialog` | `x-data`, takes the default title | all three | `sites/demo/_includes/errors-dialog.html`, the module's `_ErrorsDialog.cshtml` |
| `x-field-prefix`, `$fieldId`, `$fieldName` | directive and magics | `packingDemoPlugin` | the packing form's fields |
| `$logger` | magic | all three | inside the components |

`tests/plugins.test.ts` pins what each plugin registers. `PageContentTests` in the module's integration tests
checks the module's pages carry the `x-data` names.

**An app with options takes an object**, `{baseUrl}`, optional. A second value later is then not a signature
break on two hosts. The demo passes `site.api_url`; the module passes an empty string, which fetches relative
to whatever host served the page.

## The events

The apps never call the view or the dialog. They dispatch window events, and the host's markup wires each
listener.

| Event | Payload | Listener |
|---|---|---|
| `update-scene` | a function returning a promise of a scene, or of `null` | `pack_view.updateScene` |
| `error-occurred` | `string[]`, or `{title, errors}` | `errors_dialog.onErrorOccurred` |
| `highlight-item-type` | an item type's `id`, or `null` for all | `pack_view.highlight` - wired only in `demo`, as only *Everyday Items* sends it |

`pack_view` drops a scene whose promise resolves after a newer one was asked for, so a slow answer never draws
over a later edit.

## The API call

**Every call goes through `@binacle/api-client`** (`$api/api-client`). No URL and no wire shape is
written here.

- *Everyday Items* calls `packPresetBin` on the `sample` preset, with the box's name and `BFD`.
- The packing demo calls `packCompareBins`. Each bin and item gets its id from its place, `bin-1`, `item-1`,
  because the API rejects a repeated id and two rows may share a size.
- The decoder calls nothing. It decodes in the browser through `@binacle/vipaq`.

**`lastRequest` is the seam for a host that prints the call.** The packing demo sets it as the request goes
out: the method, the path - relative, from the client's `packCompareBinsPath` - and the body. The module's
`_js/packing_demo.js` turns it into a curl command with the page's own origin. `demo` does not print it.

## The sample set

**Randomize steps to the next hand-picked sample; it never rolls new sizes.** The page opens on sample zero.
`src/packingDemo/sampleData.ts` is generated and says so on its first line.

## What each host build must set

**Both hosts override ts-loader's `module` to `esnext`.** Their entries are `.js`, so ts-loader takes the
first `tsconfig.json` it meets above a `.ts` file - this package's, which emits commonjs. Commonjs cannot be
tree-shaken, and it loads three twice: once as its commonjs build, once as the module build its add-ons
import. Both configs say so beside the rule.

**The bundle keeps the package's old name, `binacle-net-ui.js`.** It is a URL: the demo's `_data/includes.yml`,
the module's `_AppletScripts.cshtml` and the smoke tests ask for it. Its chunk rule takes this package,
`api-client` and the design system's `components/pack-view/` together; without the last, the view lands in a
chunk no page loads. Rename the bundle and every one of those together, or none.

**A warm webpack cache hides type errors.** Both configs cache to disk. Delete
`<host>/node_modules/.cache/webpack` before trusting a clean build.

## Tests

**The 3D view is not tested here.** It is the design system's, with its own tests. Importing any plugin pulls
the view in, and three ships `OrbitControls` and its line add-ons as ESM only, which the commonjs transform
cannot load - so `jest.config.js` maps them to `tests/stubs/`. Nothing under test constructs one.

A component factory is a plain object. A test calls it with a stub `$dispatch` and `$logger` instead of starting
Alpine.

## Adding a component

- The factory is camelCase (`packingDemoApp`), the `x-data` name snake_case (`packing_demo_app`), joined by
  `Alpine.data` in a `*Plugin` function. Wrap the factory in `defineComponent` for its types.
- A reusable one goes in `src/components/<name>/`; one only an app uses stays in that app's folder.
- Add its plugin to each app's `plugin.ts` that needs it, and its name to `tests/plugins.test.ts`.
