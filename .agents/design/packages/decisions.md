---
id: packages/decisions
description: Packages decisions ledger - why the v4 client is hand-written rather than generated, and why the visualizer owns its own internals instead of sharing a utils folder.
verified: 2026-09-10
check: P1 against packages/binacle-net-client - no generator in its devDependencies, spec/v4.json present and byte-identical to the docs site's copy of the current version, and tests/contract.test.ts still validating fixtures against it; against tooling/openapi.just, where check-all-copies must diff that copy and sync-all-copies must write it. P2 against packages/binacle-net-ui/src - no utils/ or core/ folder, components/visualizer/index.ts exporting only the component and the Binacle type, and no file under apps/ importing a path inside components/visualizer/
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
