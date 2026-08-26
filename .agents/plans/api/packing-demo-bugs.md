---
description: Ten correctness and accessibility bugs in the shared packing demo component - most of them ship inside the image as well as on the demo site
state: ready
waits-on: "nothing"
paths:
  - "packages/binacle-net-ui/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "sites/demo/**"
---

# The packing demo's bugs

**Found 25 Aug 2026 by a four-way review of `sites/demo`.** None of them is styling and none waits on any
design decision.

**Most live in `packages/binacle-net-ui/`, so they are in the image too.** The demo site and
`Binacle.Net.UIModule` both consume that package, which is why a fix in one place lands on both - and why
these were invisible for as long as they were.

**The items are separable.** Anything here can be pulled into its own file the moment someone starts it.

**Neither host is fixed - checked 26 Aug 2026.** An earlier note said the demo site had already been
corrected and only the image was left. It had not. `sites/demo/pages/packing.html` and
`api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` each carry the same seven bare `x-model` lines, and
`core/packingDemo.ts` still sends the raw values. **Do not skip either host on the strength of that note.**

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
- **Pressing the one button appears to do nothing.** The button does not disable or change, the spinner is
  below the fold, and there is no `scrollIntoView`, no `focus()` and no `aria-live` in the site or the
  package.
- **The result prints the raw enum.** `FullyPacked`, or reachably
  `EarlyFail_ContainerDimensionExceeded` - a C# identifier on a public page. It needs mapping to plain
  English.
- **`unpackedItems` is on the response and rendered nowhere**, so a partial result reports a percentage and
  will not say which items were left out. **This stopped being optional on 26 Aug 2026.** The demo now ships a
  hand-picked sample set, and one of those samples exists precisely to show a partial pack - three items where
  the volume fits and the geometry does not. It is the one sample the page cannot currently explain.

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
and no categories anywhere in the API. Any change to how items are coloured is design work and belongs with
the rebrand, not here.

## What is left

**Four of the ten fixes shipped on 2026-08-26 and are ticked below.** Four more are fixed in code and
unit-tested but **have never been looked at in a browser** - they are the four `By eye` clauses, and they are
the same four the release set is holding its browser pass for.

**Two are untouched:** the submit button giving no sign it was pressed, and `unpackedItems` being rendered
nowhere. **The second stopped being optional** - see the note beside it above.

## Done when

- [x] **No dimension reaches the API as a string from any host - 2026-08-26.** `onSubmit` casts, and both
      templates carry seven `x-model.number` lines.
- [ ] Adding a bin and submitting returns a result rather than a 422.
      **By eye, and still unseen.** Fixed in code and unit-tested on 2026-08-26; nobody has loaded the page.
      Load the packing page, press Add bin, press Get results.
- [ ] Clearing all bins is caught before the request goes out.
      **By eye, and still unseen.** `formErrors` renders in both templates; nobody has watched it fire.
      Press Clear all, then Get results - the error is inline, not a 422 dialog.
- [ ] A failed request clears the scene.
      **By eye, and still unseen.** `clearScene()` exists and is called on a null result.
      Point the demo at a dead host, submit, and confirm the 3D view is empty rather than stale.
- [ ] Pressing the submit button visibly changes something within one frame.
      **By eye.** The button state, the results panel, or both.
- [x] **No C# identifier appears in the results panel - 2026-08-26.** Both templates call
      `resultStatusText`, and the map covers all six statuses read from the v3 contract.
- [ ] A partial result names the items it could not fit.
      `grep -rn "unpackedItems" sites/demo/pages api/src/Binacle.Net.UIModule/Pages` returns at least one hit.
- [ ] Every result row can be reached and activated from the keyboard.
      **By eye, and still unseen.** Both templates now give the row an `href` and `aria-current`.
      Tab to a row and press Enter; the 3D view redraws.
- [x] **The error dialog traps focus and closes on Escape - 2026-08-26.** `core/errorsDialog.ts` calls
      `showModal()` and listens for the element's own `close` event. **Watch where it sits on screen the first
      time** - `showModal()` moves it into the browser's top layer and beercss positions `.modal.active`
      itself.
- [x] **The camera aspect comes from the renderer's container - 2026-08-26.** `utils/containerAspectRatio.ts`
      reads the container's box, driven by a `ResizeObserver`. No `innerWidth` left in the visualizer.
