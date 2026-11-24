---
id: packages/decisions
description: Packages decisions ledger - why the v4 client is hand-written rather than generated, why the 3D view owns its own internals instead of sharing a utils folder, and why the request fold is the UI module's and the component only hands over the request. Also why a package sits with its slice, why theme-switcher and cookies stay generic, what @binacle/apps holds, and the levels that decide where code goes.
verified: 2026-11-10
check: P1 against api/packages/api-client - no generator in its devDependencies, spec/v4.json present and byte-identical to the docs site's copy of the current version, tests/contract.test.ts still validating fixtures against it with ajv, and src/client.ts still the only routes the apps call; against tooling/openapi.just, where check-all-copies must diff that copy and sync-all-copies must write it, and against .github/workflows, where only shared-image-tests.yml runs check-all-copies and nothing runs sync-all-copies. P2 against shared/packages/apps/src - no utils/ or core/ folder - and shared/packages/design-system/index.ts, which exports createPackView and the 3D view's types and nothing else from components/pack-view/; no file under shared/packages/apps/src imports a path inside it. P3 against shared/packages/apps/src/packingDemo/packingDemo.ts, which sets lastRequest with a relative path and formats nothing, against api/src/Binacle.Net.UIModule/_js/packing_demo.js, which registers request_fold and passes window.location.origin to curlCommand, against src/packingDemo/curlCommand.ts, which reads no window, and against Pages/Packing.cshtml, which puts the fold under the results. P4 against shared/packages/apps/src/protocolDecoder/sampleData.ts, five entries, tests/protocolDecoder/sampleData.test.ts, which reads vipaq/data/packed/demo-samples, holds each entry to its packed bin and checks each fills three quarters of it, and the Samples drawer in sites/demo/pages/vipaq.html and the UI module's Pages/Vipaq.cshtml; P5 against the folder of every package.json - packages/ holds only cookies and theme-switcher, ruby/ only jekyll-* gems, and the root workspaces list names every package folder; P6 against git grep -niE 'binacle|__' packages/theme-switcher/src, which finds nothing, and cookies being its own folder; P7 against shared/packages/apps/src, which holds app folders, components, shared and types and nothing else; P8 against the level table in the packages doc and the imports of each package; P9 against shared/packages/apps/package.json, which has no sideEffects, and against the errors dialog importing @binacle/api-client; P10 against api/packages/api-client/src/response.ts and api/packages/service-client/src/response.ts, two files, and no package that holds both
paths:
  - "packages/**"
  - "shared/packages/**"
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
path, body - and formats nothing. `_js/packing_demo.js`, the module's own entry, registers a second Alpine
component, `request_fold`, that the page nests under the demo's scope. It gives the host to `curlCommand`, the
package's formatter, which only this host calls. The package reads `lastRequest` itself in one place: the API answers an unfit item by id only, so its size comes from
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
  formatting is `curlCommand`, in the package.
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

**Why the formatter is in the package and the fold is not.** The package's jest does not reach the module's
`_js/`. The formatter started there as a few lines checked by eye, and grew: it wraps the command word by word and
weights the JSON. So it is `src/packingDemo/curlCommand.ts`, a function of the request and the origin, with a test.
It reads no `window`, and a host that does not call it does not bundle it.

**What would change this.** The demo site wanting the fold after all - then it registers the same small component
and calls the same function.

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

### P5 - a package sits with its slice, and the root folders hold generic code only

**Decided (the maintainer, 2026-11-07):** "the rule now is packages /ruby for those not having a home or are not
part of a feature", and "compact notation is a feature small but a feature...shared feature". Earlier: "its by
slice so it has to stickto vipaq", "the idea is for each slice to hold relatied thiungs togeter". On what the root
folders hold: "the idea is yes", then "ruby at root conmtinues to hold generic struff jeykyll- / root packjages
cookies themeswitcher". His idea: "evetryhing binacle must not go in packages or ruby / but in shared or in a
slice/ruby /slice/pakcages".

A package of a slice is `<slice>/packages/<name>/`. `shared/` is a slice too. **`design-system` and `apps` are in
it.** Asked `shared` or `sites`, told `apps` is read by `sites/demo` and the UI module: "shared" (the maintainer,
2026-11-07). `design-system` follows for the same reason: three sites read it, and the UI module does too.

**Not a top-level folder, for now. Decided (the maintainer, 2026-11-07):** "we go shared for now / it seems its at
the threshold of becoming a root folder but maybe not yet lets keep an eye on it". The design system's README says
what would give it one.

**The gems only the sites read are in `sites/ruby/`. Decided (the maintainer, 2026-11-07):** "sites get
binacle-docs and binacle-robots which is only used by them". Asked whether it has its own bundle or shares
`ruby/Gemfile`: "own" (the maintainer, 2026-11-08), so it has its own `Gemfile` and lock, which is Bundler's
convention.

### P6 - `theme-switcher` stays generic and its own package, and `cookies` is separate from it

**Decided (the maintainer, 2026-11-07):** "themeswotcher is generioc and should cold generic componet used but the
disign system". On js-cookie: "leave js cookie outside separate from theme-swiotchet", and asked `js-cookie` or
`cookies`: "cookies".

Nothing in `theme-switcher` names a Binacle class, attribute value or site; the storage key, the attribute and
the button's class all come from options, and a new option is how a site asks for something different. The design
system's theme button uses it, so the generic part stays at level 1 and the component uses it. Both packages are
MIT, and the AGPL design system may use them.

### P7 - `@binacle/apps` holds apps only, and the apps sit straight under `src/`

**Decided (the maintainer, 2026-11-08):** "if the packge does hgold anything other that apps we need to rename the
package". Asked about the `apps/src/apps/` stutter: "ok drop". The package's name already says what it holds, and
`components/` stays apart from `shared/`, so a reusable Alpine component and a helper do not mix.

**The app folders keep their names** (agent, 2026-11-08). `packingDemo` is the page called *Custom Sizes*; renaming
the folder changes no URL and no `x-data` name, so it is not worth it on its own.

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

### P8 - code is placed by what it knows, not by its language

**Leaning yes (the maintainer, 2026-11-07):** "i feel the general direction you have it", and the trial came first:
"iwant to do it before but also test it too". His concern, 2026-11-07: "there is no clear separation sometimes
and... in the meantime there is separation when it shouldnt". The levels themselves are the agent's (2026-11-07),
and are in the packages doc.

**Splitting by language made two faults.** One thing sat in three places: the drawer's look in `design/`, its
behaviour in `packages/`, its markup copied into each site, with the TypeScript building markup out of the Sass
class names and the three kept in step by hand. And one folder took anything of its language: `doc_body` wrote the
docs' own classes inside a generic gem every site loads, and the package called the UI module's implementation
held the apps, the 3D view and *Everyday Items* alike. A level asks what the code imports and what words it holds,
so a component's Sass, behaviour and markup are one folder, and a gem that writes `doc__text` is the docs' whatever
its language.

**The `jekyll-*` gems stay apart from the components.** Sitemap, page meta, structured data, GTM, breadcrumbs, the
manifest, resource tags and filters are site plumbing, not components, and they are level 1.

### P9 - three things about what `@binacle/apps` holds

**It has no `"sideEffects": false`.** A cold build with and without the flag: webpack already leaves *Everyday
Items* out of the UI module, and with the flag it skips the package's `index.ts`, so the chunk splits into many
modules where it was one. It grew a little on the demo and the UI module, and nothing was dropped. Builds keep a
webpack disk cache, so a warm one can hide a cost like this until `node_modules/.cache/webpack` is deleted.

***Everyday Items* stays in the package though only `demo` uses it.** It shares the helpers and the view wrapper,
and the sites have no test setup. It moves into `sites/demo/` only if `demo` gets one.

**The errors dialog is in the package and not the design system.** It imports `@binacle/api-client`, so it knows
the API, and the design system may not.

### P10 - the two API clients stay two

`@binacle/api-client` and `@binacle/service-client` each have a short response reader, written alike. They serve
different APIs and different readers, and one is experimental. A package shared between them is the split by
language that P8 removes elsewhere.

### P11 - the same skeleton in every package

So a reader who knows one package knows all. `index.ts` is at the root and `src/index.ts` is not used, because most
packages already did it that way. File names are camelCase, as every TypeScript file here is, and a folder that is
a BEM block is kebab-case because the block's Sass name is. **Tests stay in `tests/`**, as every package already
has them. Tests beside the code, as Angular and Vite templates have them, would suit a package where one folder
must hold the whole component, and would change every package. The skeleton itself is in the packages doc.
