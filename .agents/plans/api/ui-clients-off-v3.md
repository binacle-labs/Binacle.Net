---
description: Migrate the shipped UI clients off the v3 API
state: idea
waits-on: "the shape - what the UI changes to and how is not worked out yet"
horizon: near
paths:
  - "api/**"
  - "packages/binacle-net-ui/**"
---

# Migrate the shipped UI clients off the v3 API

The shipped packing demo calls `POST /api/v3/pack/by-custom`. v3 stays and is frozen, so nothing breaks by
leaving it - this is about not shipping our own UI on the version we tell users is the older one. What it moves
to is not worked out: check what the component does with the response before assuming it needs
`pack/compare-bins`, because if it only shows the winning bin then `pack/smallest-bin` already covers it and is
a smaller request and a smaller response than fetching every bin's result and throwing most of it away.

## Done when

- [ ] The packing demo calls v4, using the endpoint shaped like the answer it actually renders.
      **By eye** in `packages/binacle-net-ui/src/core/packingDemo.ts`, and both hosts have had their bundle
      rebuilt.

## Research

### Date not recorded - it is one line, in one place

`packages/binacle-net-ui/src/core/packingDemo.ts`. The UIModule rebuild deleted the second call site: the
module now serves the same TypeScript component the demo site does, so both hosts move on that one edit.

### 2026-08-22 - the two hosts were not equally ready, and now they are

The demo inside the image calls its own instance on a relative URL and was never affected. The demo site's
copy calls `api.binacle.net`, which served `2.1.1` and answered 404 on v4 when this was probed.

**That cleared. Probed 2026-09-02:** the host serves a 3.0.x image - AGPL in its OpenAPI document,
`/openapi/v4.json` answers 200, `/openapi/v2.json` is gone. **Neither host blocks this any more**, and what
is left is the open question in `waits-on:` - what the UI changes to.

### 2026-09-07 - the component keeps every bin's result, so it is `pack/compare-bins`

**The plan's own warning was to check this before assuming.** Checked: `getResults` stores the whole array -
`this.results = response.data` - and `selectResult` / `isSelected` let the visitor click between them, so the
page is a list of results and not just the winner. `selectedResult` merely defaults to the first entry with a
bin.

**So `pack/smallest-bin` does not cover it.** Dropping to it would delete the result list, which is a feature
of the page and not an accident. The v4 endpoint with the same shape is `pack/compare-bins` - one result per
bin, in the order the bins were sent.

### Date not recorded - expect to touch the call site again

v4 is experimental for the whole 3.0.x line and may change. Migrating our own UI to it is fine, and it is the
adoption that justifies calling v4 stable later, but the call site is not settled by this change.
