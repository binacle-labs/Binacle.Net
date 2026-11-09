---
id: packages/decisions
description: Packages decisions ledger - why the v4 client is hand-written rather than generated, why the 3D view owns its own internals instead of sharing a utils folder, and why the request fold is the UI module's and the component only hands over the request.
verified: 2026-11-10
check: P1 against api/packages/api-client - no generator in its devDependencies, spec/v4.json present and byte-identical to the docs site's copy of the current version, tests/contract.test.ts still validating fixtures against it with ajv, and src/client.ts still the only routes the apps call; against tooling/openapi.just, where check-all-copies must diff that copy and sync-all-copies must write it, and against .github/workflows, where only shared-image-tests.yml runs check-all-copies and nothing runs sync-all-copies. P2 against shared/packages/apps/src - no utils/ or core/ folder - and shared/packages/design-system/index.ts, which exports createPackView and the 3D view's types and nothing else from components/pack-view/; no file under shared/packages/apps/src imports a path inside it. P3 against shared/packages/apps/src/packingDemo/packingDemo.ts, which sets lastRequest with a relative path and formats nothing, against api/src/Binacle.Net.UIModule/_js/packing_demo.js, which registers request_fold, holds the curl formatting and reads window.location.origin, and against Pages/Packing.cshtml, which puts the fold under the results. P4 against shared/packages/apps/src/protocolDecoder/sampleData.ts, five entries, tests/protocolDecoder/sampleData.test.ts, which reads vipaq/data/packed/demo-samples, holds each entry to its packed bin and checks each fills three quarters of it, and the Samples drawer in sites/demo/pages/vipaq.html and the UI module's Pages/Vipaq.cshtml
paths:
  - "api/packages/api-client/**"
  - "shared/packages/apps/**"
---

# Packages - decisions ledger

Why the TypeScript packages are shaped the way they are where the shape is not obvious from the code. What
they *are* is the docs under `$packages`; this file is the reasoning.

## Decided

### P1 - the v4 client is hand-written, and a test holds it to the spec

**Decided (the maintainer, 2026-09-09):** "however ido hate the generated dependency ... leaning on custom
written package that can be testable against generated spec?", and on the copies: "check-all-copies = runs in
CI / sync-all-copies = never runs in CI only humans run it once and the spec is commited".

`@binacle/api-client`, in `api/packages/api-client`, is written by hand. No generator, no codegen step, no
generator in the dependency tree. A test validates the hand-written types against a committed copy of the v4
OpenAPI document instead.

**Generating was the obvious alternative and it was measured.** The published documents are already
generation-quality and linted for it, so a generated client is one command. Three things argued against it:

- **The UI calls few endpoints.** When this was decided the demo called `pack/compare-bins` and nothing else,
  out of sixteen v4 endpoints. *Everyday Items* has since added `pack/bin/{preset}/{bin}`. A generator would
  maintain a short file of types.
- **v4's contracts share bases**, so those sixteen endpoints are roughly six distinct shapes. Adding
  `pack/smallest-bin` and `pack/best-bin` later is a method each and no new response type, because
  `PackBinResponse` is already the whole body of both.
- **The maintainer did not want a generator in the dependency tree**, and that is the decision the other two
  only support.

**What the contract test buys is the one real benefit of generating.** Fixtures are annotated with the
hand-written types and validated against `spec/v4.json` with ajv, so a field renamed in the API fails a test
rather than reaching a page. It catches a renamed or newly-required field. It does not catch a field we
declare that the API never had, because the spec sets no `additionalProperties: false`.

**The copy is kept honest by a recipe, not by discipline.** `just openapi check-all-copies` regenerates the
documents and diffs every committed copy, and it runs on every pull request and on release.
`just openapi sync-all-copies` writes them and **is called by no workflow** - a person runs it and commits
the result. That asymmetry is deliberate: a check that runs in CI and a write that never does.

**This does not reverse the no-published-SDK decision.** Nothing here is published, the package is
`private: true`, and an integrator still generates their own client from the document. What changed is only
that our own UI stopped hand-rolling `fetch` calls in a component.

**What would change this.** The UI calling four or more endpoints, or the OpenAPI documents becoming
committed source for some other reason - today they are build output, and the client's copy exists because
the package should not reach into the documentation site to find one.

### P3 - the request fold is the UI module's, and the component only hands over the request

**Decided (the maintainer, 2026-09-14):** "we will only do it for UiModule". The packing page inside the
image shows the call it just made; the demo site does not. `packing_demo_app` exposes `lastRequest` - method,
path, body - and formats nothing. The formatting and the host live in `_js/packing_demo.js`, the module's own
entry, as a second Alpine component, `request_fold`, that the page nests under the demo's scope. The package
reads `lastRequest` itself in one place: the API answers an unfit item by id only, so its size comes from
what was sent.

**A fold under the results.** `Pages/Packing.cshtml` puts a `Request` fold under the results, hidden until a
call has gone out, holding the command in a code block with the design system's Copy button. It is closed
until opened, so it does not push the page down. Two shapes came before it: an article under the results,
which pushed the page down by the height of the request every run, and a right-side dialog - the maintainer,
2026-09-15: "i was thinking a popover panel on the right". No quote here covers the move from the dialog to
the fold.

**Four questions were open, and each answer sits where it was taken.**

- **One `curl` command, not raw HTTP and not a language snippet.** The fold is checked by pasting what it
  prints into a terminal, and `curl` is what the Docker Hub quick start already hands a reader. The
  formatting is in the module's entry.
- **The host is `window.location.origin`.** The module always calls the API it is served from -
  `ModuleDefinition.cs` sets `ApiBaseUrl` to empty - so the page's own origin is the only host that is true,
  and it is one the reader can call. `lastRequest.path` stays relative for the same reason: which host
  answered is the page's to say, never the component's, and the test holds it relative even with a `baseUrl`.
- **The fold is around the component, not in it.** In the shared component it would land on the demo site
  too, printing a public host nobody will call from their own code. The seam it needed - reading the
  component's state - is one property, set on every valid submit and left standing whatever the answer was,
  so a failed call still shows what was sent.
- **Request only.** The 3D view and the result list already show the response; a second copy of it would
  double the fold for nothing.

**What the module's half costs.** `_js/packing_demo.js` has no test harness - the package's jest does not
reach it. The formatter started as a few lines checked once by `bash -n` and by eye. It has since grown: it
wraps the command word by word and weights the JSON. The rule here was that it moves into a tested file when
it grows; that has not happened yet.

**What would change this.** The demo site wanting the fold after all - then the formatting moves into the
package as a method on the component, and the origin becomes an option the host passes in.

### P4 - the decoder's samples are packed demo examples, held to the packed data by a test

**Decided (the maintainer, 2026-09-15):** "the samples of vipaq are ugl;y... i need properly filled bins not
too large dimensions", then "lets do a sidebar with a samples buton below the + a full lenth button ... a copy
button, and no add... we want to encourage the user to copy and paste it".

The ViPaq decoder offers known-good strings. Five, in `src/protocolDecoder/sampleData.ts`, and the component exposes them as `samples` and nothing else.
Both hosts show them in a drawer from the right, opened by a `Samples` button beside `Add result`: each name,
its string in a selectable block, a Copy button, and a **Use** button that adds the string as a result. **Use
goes against "no add" above**, and no quote here covers it. Three earlier shapes were built and reversed on
2026-09-15: chips under the input, a menu that decoded on click, and a menu that filled the input. Each
host's page script holds the open state, the copy and Use; the component owns the strings, so both hosts get
the same five from the bundle.

**Each is one placed bin out of `vipaq/data/packed/demo-samples`**, the packed results the demo's own worked
examples generate, and a test serializes that placement and holds the string to it. A string that drifts from
its source fails a test. A second test asserts every bin is at least three quarters full.

**Why packed results and not test vectors.** The first version took five strings from `vipaq/test-vectors`,
and the maintainer rejected them the same day: the vectors exist to cover the wire, not to look like anything -
a 1000x2000x3000 bin with one box, a 20000-sided bin with 45 boxes, an empty bin. A sample on the decoder page
is there to be drawn, so it has to look like a packing: a full bin with sides in the tens. The demo's packed
results are exactly that, already generated and already proved.

**Why a hand-written file and not a generator.** The choice and the visitor-facing names are curated; a
generator would need the same list. If five stops being enough, the list moves to a `regen` recipe over the
packed data.

**The chips were also rejected** - too big and in the wrong place. The drawer takes no space until opened.

## Pending

What the packages do and why. No quote from the maintainer covers these yet.

### P2 - the 3D view owns its internals, and that is why there is no shared utils folder

`@binacle/apps` is `components/`, `shared/`, `types/` and one folder per app. There is no `utils/`, and that
absence is the decision.

**The measurement that produced it, taken 2026-09-09: of the 25 files in the old `src/utils/`, 19 were used
only by the visualizer** - the 3D view before the rebrand - or by another visualizer util: the scene helpers,
camera maths, materials, origins and loading state. Of the remaining six, three belonged to the packing demo's
sample data, one to `field`, one to the demo alone, and exactly one - `defineComponent` - was genuinely
shared.

**So there was never a shared utility layer to keep.** There was a visualizer with its implementation spread
across a folder anything could import from, which is worse than a naming problem: nothing stopped an app
reaching into visualizer internals, because they looked like general helpers.

**The 3D view now lives in the design system**, in `components/pack-view/`, and the design system's `index.ts`
exports `createPackView` and its types and nothing else from that folder. `@binacle/apps` wraps it as the
`pack_view` Alpine component in its own `components/packView/`. No app source reaches inside; one test,
`tests/everydayItems/itemTypes.test.ts`, imports the item prints from inside it to check them.

**The barrels went with the move.** The old `index.ts` in each of `core/`, `utils/`, `models/` and
`viewModels/` re-exported nearly everything, so most imports named a barrel rather than a file and the
dependency graph was invisible. A folder keeps a barrel only where it is that folder's public surface.

**The cost, paid once and worth knowing:** about 50 file moves and every import rewritten, with no behaviour
change. The test suite was the whole safety net - 348 tests passed before and after with no assertion edited.
A restructure of this kind that needs an assertion changed has moved something wrong.
