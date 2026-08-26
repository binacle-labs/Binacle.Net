---
description: Ten correctness and accessibility bugs in the shared packing demo component - most of them ship inside the image as well as on the demo site
state: deferred
waits-on: "the v3.0.0 tag - the maintainer deferred the unfitted-items tooltip, the one item left, on 2026-08-27"
paths:
  - "packages/binacle-net-ui/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "sites/demo/**"
---

# The packing demo's bugs

**Found 25 Aug 2026 by a four-way review of `sites/demo`.** None of them is styling. **One now waits on a
design decision** - the unfitted items, since the inline display was rejected on layout.

**Most live in `packages/binacle-net-ui/`, so they are in the image too.** The demo site and
`Binacle.Net.UIModule` both consume that package, which is why a fix in one place lands on both - and why
these were invisible for as long as they were.

**The items are separable.** Anything here can be pulled into its own file the moment someone starts it.

**Both hosts carry the `.number` fix - checked 27 Aug 2026.** Commit 70407e99 landed it.
`grep -c 'x-model.number' sites/demo/pages/packing.html api/src/Binacle.Net.UIModule/Pages/Packing.cshtml`
returns 7 for each, and `core/packingDemo.ts` casts as well. `resultStatusText`, the row `href` and
`aria-current` are on both hosts too. **The 27 Aug submit-button work is on both hosts** - the bindings and
the live region. **The unfitted-items markup landed on both hosts and was taken back out the same day** - see
below.

## The one that costs visitors

**A visitor who types their own box dimensions gets a .NET converter exception in a dialog.**

`x-model` without the `.number` modifier stores `event.target.value`, a string - Alpine has no special case
for `type="number"`. `Box.length` is declared `number` in `viewModels/box.ts:9`, so TypeScript never sees the
lie, and `getDimensions()` wraps each value in `Number(...)`, so the client validator passes. `onSubmit` at
`core/packingDemo.ts:153` and `:159` then sends `x.length` raw, and the API declares `required int` with no
`AllowReadingFromString` anywhere in `api/src`.

**It only breaks when someone uses it.** The seeded random sample is built from real integers, so the demo
works perfectly until a visitor edits a field.

**Fix it in `onSubmit`, not in the markup** - one edit covers both hosts. Adding `.number` in the templates as
well is cheap insurance.

## The two that return a 422

- **`addBin` builds a request the API refuses.** `core/packingDemo.ts:59` copies the previous bin verbatim,
  and `Box.id` (`viewModels/box.ts:19`) is `${length}x${width}x${height}`, so the copy collides. The API
  rejects duplicate ids. Two clicks from arrival to an error. The comment at `:57` says the copy is
  deliberate, and it is - what is not deliberate is the id.
- **`clearAllBins` and `clearAllItems` pass validation with nothing in them.** `isValid()` at
  `core/packingDemo.ts:42` uses `.every()`, which is `true` for an empty array, so zero bins reaches the
  server. No confirm and no undo either.

**Found 26 Aug 2026, and gone the same day.** Randomize used to roll its bins and items independently and
could produce two identical ones, which the API rejects as a duplicate id. **The hand-picked sample set
removed it** - Randomize now moves between checked samples and a test asserts no sample carries a duplicate
id. Recorded because the same trap is one careless roll away if anything ever generates a set again.

## The four the visitor cannot see past

- **A failed call leaves the previous packing drawn.** `onSubmit` returns `null` to the visualizer when the
  response is empty, which clears `results` and `selectedResult` but leaves the scene alone. The panel says
  nothing packed while the 3D view still shows the last result.
- **Pressing the one button appears to do nothing.** The button did not disable or change, the spinner is
  below the fold, and there was no `scrollIntoView`, no `focus()` and no `aria-live` in the site or the
  package. **Fixed in the package and both hosts on 2026-08-27.** `onSubmit` sets `submitting` and
  `submitStatus` before it dispatches, so the click changes the page in its own frame: the button disables,
  reads "Working...", carries `aria-busy`, and a live region says "Packing...".
  **The live region said too much, and it lingered. Both fixed 2026-08-27.** "Results ready." was removed - a
  result fills the panel and redraws the 3D view, so the line was saying what the page had already said.
  "No results." stays, because a failed request shows nothing else. And nothing used to clear `submitStatus`
  after a request settled, so any message outlived the result it described - Randomize was the case that got
  caught, but a sample, a keystroke in any field, an added bin and a cleared list all left it stale.
  **One deep `$watch('model')` in `init()` clears it**, which is the only place that catches a keystroke as
  well as a click.
- **The result prints the raw enum.** `FullyPacked`, or reachably
  `EarlyFail_ContainerDimensionExceeded` - a C# identifier on a public page. It needs mapping to plain
  English.
- **`unpackedItems` was on the response and rendered nowhere**, so a partial result reported a percentage and
  would not say which items were left out. **This stopped being optional on 26 Aug 2026.** The demo ships a
  hand-picked sample set, and `02-packs-nowhere` exists precisely to show a partial pack - three 20x20x20
  items into one 30x30x30 bin, where the volume fits and the geometry does not. **Still open, and the reason is
  new.** An inline block shipped on both hosts on 2026-08-27 - a heading, "Could not fit 2 items", and one line
  per entry, "2 x 20x20x20-3" - and it was **pulled the same day**: it changes the height of the result row.
  **The markup is out of both templates and the answer will be a tooltip instead.**
  **The four helpers in `core/packingDemo.ts` stayed** - `hasUnpackedItems`, `unpackedItemsOf`,
  `unpackedItemsTitle` and `unpackedItemText` (`:270` to `:282`), with ten unit tests under
  `describe("the items a result could not fit")`. Only the markup went, so whoever builds the tooltip starts
  from strings that are already checked, not from nothing. **Do not rebuild the inline version** - it was
  built once and it is what got rejected.

## The three about operating it

- **Result rows are mouse-only.** They are `<a>` elements with no `href`: not focusable, no role, dead to
  Enter. Selecting a result is the only way to redraw the 3D view for a different bin.
- **The error dialog never opens as one.** It is a native `<dialog>` toggled by an Alpine class binding;
  `showModal()` is never called. Focus stays behind it, Tab walks the page underneath, Escape does nothing and
  nothing is announced.
- **The 3D render is stretched.** `core/packingVisualizer.ts:45` and `:226` take the camera aspect from
  `window.innerWidth / window.innerHeight` while `:54` and `:247` size the renderer from the container's own
  box. Wrong everywhere, badly wrong on a phone. A `ResizeObserver` on the container fixes both.

## What is not a bug, so nobody chases it

**The module does not ship an old palette. Checked 26 Aug 2026 and the claim does not hold.** A review said
the module was still on a pre-contrast-pass dark palette while the sites had moved on. It is not.
`api/src/Binacle.Net.UIModule/_sass/_theme.scss` is byte identical to `sites/demo/_sass/_theme.scss` once
whitespace is stripped; `#3c5d8b` is also the dark `--primary` in `sites/www/_sass/_tokens.scss`; and the
module's `_components.scss` carries the same four contrast overrides the demo site's does. **There is no
palette work here.** If a contrast number is ever in doubt, measure it - do not compare hex strings across
files that were always meant to match.

**The pink and teal faces in the 3D view are not item colours.** `utils/_itemMaterial.ts` is a single shared
`MeshNormalMaterial`, which colours each face by the direction its normal points. There is one kind of item
and no categories anywhere in the API. Any change to how items are coloured is design work, not here.

## What is left

**Nine of the ten are closed and ticked below.** Four landed whole on 2026-08-26. The submit button landed on
both hosts on 2026-08-27 and its status line was trimmed the same day - it said "Results ready." over a panel
that had already filled, and it never cleared. **The four that only a browser could settle were loaded and
confirmed on 2026-08-27**: adding a bin, clearing all bins, a failed request clearing the scene, and reaching
a result row from the keyboard.

**One is left: the unfitted items. The maintainer deferred it on 2026-08-27** - see `waits-on:` for what
revives it. It is not unbuilt. The inline block was built, shipped on both hosts, and **rejected on layout** -
it changes the height of the result row. The markup is out of both templates and the answer will be a
tooltip. The four helpers behind it stayed, tested.

**What is left is one interaction nobody has designed yet.** The tooltip is not markup - hover, touch and
keyboard each need an answer, which is why it was not built on the way out.

## Done when

- [x] **No dimension reaches the API as a string from any host - 2026-08-26.** `onSubmit` casts, and both
      templates carry seven `x-model.number` lines.
- [x] **Adding a bin and submitting returns a result rather than a 422 - seen in a browser 2026-08-27.**
      **By eye.** Load the packing page, press Add bin, press Get results.
- [x] **Clearing all bins is caught before the request goes out - seen in a browser 2026-08-27.**
      **By eye.** Press Clear all, then Get results - the error is inline, not a 422 dialog.
- [x] **A failed request clears the scene - seen in a browser 2026-08-27.**
      **By eye.** Point the demo at a dead host, submit, and confirm the 3D view is empty rather than stale.
- [x] **Pressing the submit button visibly changes something within one frame - 2026-08-27.** `onSubmit`
      sets `submitting` and `submitStatus` before it dispatches, and both templates bind `:disabled`,
      `:aria-busy`, `submitButtonText()` and the live region.
      `grep -c 'submitButtonText\|submitStatus\|aria-busy' sites/demo/pages/packing.html api/src/Binacle.Net.UIModule/Pages/Packing.cshtml`
      returns 3 for each.
      **The live region only ever says "Packing..." or "No results.", and it clears - 2026-08-27.**
      "Results ready." went, and `init()` clears `submitStatus` on any change to `model`.
      `grep -c "Results ready" packages/binacle-net-ui/src/core/packingDemo.ts` returns 0, and
      `grep -cF "\$watch('model'" packages/binacle-net-ui/src/core/packingDemo.ts` returns 1.
      **`-F` is not decoration** - without it grep reads the `$` as an anchor and the count is always 0.
      **By eye:** press Get results and watch the button in the same frame - it disables, reads "Working...",
      and the line under it says "Packing..." and then goes empty when the panel fills. Press Randomize after
      a failed request and "No results." goes with it.
- [x] **No C# identifier appears in the results panel - 2026-08-26.** Both templates call
      `resultStatusText`, and the map covers all six statuses read from the v3 contract.
- [ ] A partial result names the items it could not fit **in a tooltip, so the result row keeps its height.**
      **Deferred by the maintainer, 2026-08-27** - `waits-on:` says what revives it. It is the only open box
      in this file.
      The inline block that shipped on 2026-08-27 did change the height and was pulled the same day. **This is
      not unbuilt work - it is work to be built again, differently.**
      **The strings survived the rollback and are tested.** `hasUnpackedItems`, `unpackedItemsOf`,
      `unpackedItemsTitle` and `unpackedItemText` are in
      `packages/binacle-net-ui/src/core/packingDemo.ts` with ten tests behind them. Only the markup went.
      `grep -c 'unpackedItemsTitle\|unpackedItemText\|hasUnpackedItems\|unpackedItemsOf'` returns 6 on
      `packages/binacle-net-ui/src/core/packingDemo.ts` and 10 on
      `packages/binacle-net-ui/tests/components/packingDemo.test.ts`. If either drops the tooltip is starting
      from nothing and this box got worse.
      `grep -c hasUnpackedItems sites/demo/pages/packing.html` and the same on
      `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` both return 0 until the tooltip lands, and must
      return the same number as each other after it - one host is not the feature.
      **By eye, and the whole clause:** Randomize to `02-packs-nowhere` - one 30x30x30 bin, three 20x20x20
      items - and reach `Could not fit 2 items` and `2 x 20x20x20-3` from the row without the row growing.
      **The interaction is the work, not the strings**: hover, touch and keyboard each need an answer. No grep
      settles any of that.
- [x] **Every result row can be reached and activated from the keyboard - seen in a browser 2026-08-27.**
      Both templates give the row an `href` and `aria-current`.
      **By eye.** Tab to a row and press Enter; the 3D view redraws.
- [x] **The error dialog traps focus and closes on Escape - 2026-08-26.** `core/errorsDialog.ts` calls
      `showModal()` and listens for the element's own `close` event. **Watch where it sits on screen the first
      time** - `showModal()` moves it into the browser's top layer and beercss positions `.modal.active`
      itself.
- [x] **The camera aspect comes from the renderer's container - 2026-08-26.** `utils/containerAspectRatio.ts`
      reads the container's box, driven by a `ResizeObserver`. No `innerWidth` left in the visualizer.
