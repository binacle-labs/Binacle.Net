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

### 2026-08-22 - the two hosts are not equally ready

The demo inside the image calls its own instance on a relative URL and is unaffected. The demo site's copy
calls `api.binacle.net`, which serves image `2.1.1` and answers 404 on v4 - probed that day. Moving that copy
to v4 breaks the live demo until that host serves a v3.0.x image.

### Date not recorded - expect to touch the call site again

v4 is experimental for the whole 3.0.x line and may change. Migrating our own UI to it is fine, and it is the
adoption that justifies calling v4 stable later, but the call site is not settled by this change.
