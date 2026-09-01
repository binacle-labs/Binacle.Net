---
description: Two open bugs in the shared packing demo - a partial result names no unfitted items, and the submit button can stick disabled on a page with no visualizer
state: idea
waits-on: "nobody - it is an idea"
horizon: near
paths:
  - "packages/binacle-net-ui/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "sites/demo/**"
---

# The packing demo's two open bugs

Two bugs are left in the shared packing demo. A partial result reports a percentage and never says which items
were left out, so the visitor is told the pack failed and not what failed. And the submit button can stick
disabled, with the status stuck on "Packing...", on a page that has no visualizer listening. Neither is a typo:
the first needs an interaction that works for hover, touch and keyboard, and the second needs a decision about
what the button should wait on.

## Done when

- [ ] A partial result names the items it could not fit, in a tooltip, and the result row keeps its height.
      **By eye.** Randomize to `02-packs-nowhere` and reach `Could not fit 2 items` and `2 x 20x20x20-3`
      from the row without the row growing. Hover, touch and keyboard all reach it.
- [ ] The four helpers and their ten tests are still there and still drive the tooltip.
      `grep -c 'unpackedItemsTitle\|unpackedItemText\|hasUnpackedItems\|unpackedItemsOf'` returns 6 on
      `packages/binacle-net-ui/src/core/packingDemo.ts` and 10 on
      `packages/binacle-net-ui/tests/components/packingDemo.test.ts`.
- [ ] Both hosts render it, to the same count.
      `grep -c hasUnpackedItems sites/demo/pages/packing.html` and the same on
      `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` return the same non-zero number.
- [ ] The submit button cannot stay disabled when no visualizer is listening.
      **By eye.** What clears `submitting` runs whether or not anything handles `update-scene`. Render the
      demo component on a page with no visualizer, submit, and the button comes back.

## Research

### 2026-08-25 - a four-way review found ten bugs; nine are fixed

The two below are what is left.

### The unfitted items - `unpackedItems` is on the response and rendered nowhere

The shipped sample `02-packs-nowhere` exists to hit exactly this: three 20x20x20 items into one 30x30x30 bin,
where the volume fits and the geometry does not.

**2026-08-27 - an inline block shipped on both hosts and was pulled the same day.** A heading, "Could not fit
2 items", one line per entry. It was pulled because it changes the height of the result row. **Do not rebuild
the inline version.** The answer is a tooltip.

**The strings survived and are tested.** `hasUnpackedItems`, `unpackedItemsOf`, `unpackedItemsTitle` and
`unpackedItemText` are in `packages/binacle-net-ui/src/core/packingDemo.ts` with ten tests behind them in
`packages/binacle-net-ui/tests/components/packingDemo.test.ts`. Only the markup went, so whoever builds the
tooltip starts from checked strings.

**Both hosts or neither.** The demo site and `Binacle.Net.UIModule` consume the same package; one host is not
the feature.

### The submit button - found independently by two reviews, date not recorded

`packages/binacle-net-ui/src/core/packingDemo.ts:183` sets `submitting = true` in `onSubmit`, and the only
thing that clears it is the `finally` at `:225`, which is inside the thunk handed to
`$dispatch('update-scene', ...)`. **Nothing runs that thunk unless a visualizer is listening.**

**Latent, not live** - both packing pages include the visualizer today.
