---
id: packages/decisions
description: Packages decisions ledger - why the v4 client is hand-written rather than generated, why the visualizer owns its own internals instead of sharing a utils folder, and why the request panel is the UI module's and the component only hands over the request.
verified: 2026-09-15
check: P1 against packages/binacle-net-client - no generator in its devDependencies, spec/v4.json present and byte-identical to the docs site's copy of the current version, and tests/contract.test.ts still validating fixtures against it; against tooling/openapi.just, where check-all-copies must diff that copy and sync-all-copies must write it. P2 against packages/binacle-net-ui/src - no utils/ or core/ folder, components/visualizer/index.ts exporting only the component and the Binacle type, and no file under apps/ importing a path inside components/visualizer/. P3 against packages/binacle-net-ui/src/apps/packingDemo/packingDemo.ts, which sets lastRequest with a relative path and formats nothing, and against api/src/Binacle.Net.UIModule/_js/packing_demo.js, which holds the curl formatting and reads window.location.origin. P4 against packages/binacle-net-ui/src/apps/protocolDecoder/sampleData.ts, five entries, and tests/apps/protocolDecoder/sampleData.test.ts, which reads vipaq/test-vectors and holds four of them to their rows
paths:
  - "packages/**"
---

# Packages - decisions ledger

Why the TypeScript packages are shaped the way they are where the shape is not obvious from the code. What
they *are* is the docs under `$packages`; this file is the reasoning.

## Locked

### P1 - the v4 client is hand-written, and a test holds it to the spec

`packages/binacle-net-client` is written by hand. No generator, no codegen step, no generator in the
dependency tree. A test validates the hand-written types against a committed copy of the v4 OpenAPI document
instead.

**Generating was the obvious alternative and it was measured.** The published documents are already
generation-quality and linted for it, so a generated client is one command. Three things argued against it:

- **The UI calls one endpoint.** After the demo moved to v4 it calls `pack/compare-bins` and nothing else,
  out of sixteen v4 endpoints. A generator would maintain about thirty lines of types.
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

### P2 - the visualizer owns its internals, and that is why there is no shared utils folder

`packages/binacle-net-ui` is `apps/`, `components/` and `shared/`. There is no `utils/`, and that absence is
the decision.

**The measurement that produced it, taken 2026-09-09: of the 25 files in the old `src/utils/`, 19 were used
only by the visualizer** or by another visualizer util - the scene helpers, camera maths, materials, origins
and loading state. Of the remaining six, three belonged to the packing demo's sample data, one to `field`,
one to the demo alone, and exactly one - `defineComponent` - was genuinely shared.

**So there was never a shared utility layer to keep.** There was a visualizer with its implementation spread
across a folder anything could import from, which is worse than a naming problem: nothing stopped an app
reaching into visualizer internals, because they looked like general helpers.

The visualizer now exposes its component and the `Binacle` contract type from its own `index.ts`, and
nothing else. A third app consumes it without being able to reach inside.

**The barrels went with the move.** The old `index.ts` in each of `core/`, `utils/`, `models/` and
`viewModels/` re-exported nearly everything, so most imports named a barrel rather than a file and the
dependency graph was invisible. A folder keeps a barrel only where it is that folder's public surface.

**The cost, paid once and worth knowing:** about 50 file moves and every import rewritten, with no behaviour
change. The test suite was the whole safety net - 348 tests passed before and after with no assertion edited.
A restructure of this kind that needs an assertion changed has moved something wrong.

### P3 - the request panel is the UI module's, and the component only hands over the request

**Decided 2026-09-14 by the maintainer: UI module only.** The packing page inside the image shows the call it
just made; the demo site does not. `packing_demo_app` exposes `lastRequest` - method, path, body - and nothing
else changed in the shared package. The formatting and the host live in `_js/packing_demo.js`, the module's
own entry, as a second Alpine component the page nests under the demo's scope.

**A panel on the right, not under the results - the maintainer's call on 2026-09-15.** A `Request` button on
the results card opens a beercss right `dialog`, the same overlay-and-dialog shape `_ErrorsDialog` already
uses, so nothing new is styled ahead of the rebrand. The first shape was an article under the results; it
pushed the page down by the height of the request every run, and the request is read once, not watched.

**Four questions were open, and each answer sits where it was taken.**

- **One `curl` line, not raw HTTP and not a language snippet.** The panel is checked by pasting what it
  prints into a terminal, and `curl` is what the Docker Hub quick start already hands a reader. The
  formatting is in the module's entry.
- **The host is `window.location.origin`.** The module always calls the API it is served from -
  `ModuleDefinition.cs` sets `ApiBaseUrl` to empty - so the page's own origin is the only host that is true,
  and it is one the reader can call. `lastRequest.path` stays relative for the same reason: which host
  answered is the page's to say, never the component's, and the test holds it relative even with a `baseUrl`.
- **The panel is around the component, not in it.** In the shared component it would land on the demo site
  too, printing a public host nobody will call from their own code. The seam it needed - reading the
  component's state - is one property, set on every valid submit and left standing whatever the answer was,
  so a failed call still shows what was sent.
- **Request only.** The visualizer and the result list already show the response; a second copy of it would
  double the panel for nothing.

**What the module's half costs.** `_js/packing_demo.js` has no test harness - the package's jest does not
reach it. The formatter is twelve lines, its output was parsed by `bash -n` once, and the rebuild plus the
by-eye paste is what proves it. If that half grows, it moves into a tested file.

**What would change this.** The demo site wanting the panel after all - then the formatting moves into the
package as a method on the component, and the origin becomes an option the host passes in.

### P4 - the decoder's samples are packed demo examples, held to the packed data by a test

**Decided 2026-09-15 by the maintainer: the ViPaq decoder offers known-good strings.** Five, in
`src/apps/protocolDecoder/sampleData.ts`, and the component exposes them as `samples` and nothing else. The
module page shows them in a right-side panel behind a full-width `Samples` button under the add button: each
name, its string in a selectable block, and a Copy button. **No add button, on purpose** - the visitor is meant
to copy the string and paste it, because seeing and handling the string is the point of the page. Three
earlier shapes were built and reversed on 2026-09-15: chips under the input, a menu that decoded on click, and
a menu that filled the input. The page's own script holds the open state and the copy; the component owns them, so the demo site gets the same
five from the bundle once its page carries the button.

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

**The chips were also rejected** - too big and in the wrong place. The menu takes no space until opened.
