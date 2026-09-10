---
description: The next three pieces of work on the packing demo - name the items that did not fit, stop the submit button sticking, and show the visitor the HTTP call that was just made
state: proposed
waits-on: "two answers - whether the request panel is a UI Module feature or a shared one, and which API version it prints. The other two need no decision. State chosen by an agent to make the file legible; strike it if it is wrong"
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "packages/binacle-net-ui/**"
  - "sites/demo/**"
---

# The packing demo - the next three

**These were filed as a bug list and an idea, and they are neither.** They are the next three pieces of work
on the same surface, and the first two were the last survivors of a ten-item review where the other eight
are fixed.

**Two of the three ship in the image, one is shared.** Say which before starting - it changes where the code
goes and how much of it there is.

---

## 1. Name the items that did not fit

**A work item, not a defect.** A partial result reports a percentage and never says which items were left
out, so the visitor is told the pack failed and not what failed. `unpackedItems` is on the response already
and is rendered nowhere.

**The shipped sample `02-packs-nowhere` exists to hit exactly this** - three 20x20x20 items into one
30x30x30 bin, where the volume fits and the geometry does not.

**Do not rebuild the inline version.** A heading plus one line per entry shipped on both hosts on
2026-08-27 and was pulled the same day, because it changes the height of the result row. **The answer is a
tooltip**, and it has to be reachable by hover, touch and keyboard - that is the real work here, not the
strings.

**The strings survived the revert and are tested.** `hasUnpackedItems`, `unpackedItemsOf`,
`unpackedItemsTitle` and `unpackedItemText` are in `packages/binacle-net-ui/src/apps/packingDemo/packingDemo.ts` with ten
tests behind them in `packages/binacle-net-ui/tests/apps/packingDemo/packingDemo.test.ts`. Whoever builds the
tooltip starts from checked strings.

**Both hosts or neither.** The demo site and `Binacle.Net.UIModule` consume the same package; one host is
not the feature.

### The UI module half landed 2026-09-10. The demo site half has not

**The shared package needs nothing.** The four helpers were already on the component. The tooltip is
beercss's own `.tooltip`, so there is no directive and no TypeScript to add - the demo site picks it up from
the stylesheet it already loads.

**A first attempt on 2026-09-10 built a native `popover` with an Alpine directive and was thrown away.**
It rendered at the top-left of the viewport instead of against its trigger: an open `popover` is painted in
the top layer, where `position: absolute` no longer resolves against the wrapper. **Do not rebuild that.**
beercss positions its own tooltip correctly and is already loaded.

**What is left is markup and two style rules**, to be written in a site session because `sites/` is off
limits to a coding session. Two files:

**`sites/demo/pages/packing.html`.** Its results block is the same markup as the UI module's, with `@click`
where the module writes `x-on:click`. Copy what `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` now
does, dropping the Razor `@* *@` comments. **Inside the existing `<a class="row wave padding max">`**, after
the `<div class="grid no-space max">` and before the anchor closes, add:

- a `<template x-if="hasUnpackedItems(result)">` holding a `<div style="position:relative;z-index:10">`,
  which carries two children: a `<button type="button" class="transparent circle"
  :aria-label="unpackedItemsTitle(result)" @click.prevent.stop="$el.focus()">` containing `<i>info</i>`, and
  a `<div class="tooltip max large-space">` holding a `<p x-text="unpackedItemsTitle(result)">` and an
  `x-for` over `unpackedItemsOf(result)` writing one `<p x-text="unpackedItemText(unpackedItem)">` per entry.

**Two things that were got wrong once each and cost a rebuild.** `position:relative` on the wrapper is not
optional - `.tooltip` is `position:absolute`, so without it the panel anchors to an ancestor up the page and
lands nowhere near the button. And **the click handler needs a non-empty expression**: Alpine compiles
`__self.result = <expression>`, so a bare `@click.prevent.stop` is a syntax error it logs once per row.
`$el.focus()` is the expression to use - it also makes a tap open the tooltip, which Safari does not do on
its own.

**The button goes inside the anchor**, which is what the ViPaq page already does with its delete button -
`z-index` lifts it over the row and `.prevent.stop` keeps the click off the row. **The wrapping `<div>` is
required**: beercss reveals a tooltip with `:hover > .tooltip`, so the tooltip has to be a direct child of
the element being hovered, and `<p>` is not allowed inside a `<button>`.

**`sites/demo/_sass/_components.scss`.** Copy the `:focus-within > .tooltip` rule and the two `.tooltip.max p`
rules added to the module's `_sass/_components.scss` on the same date. **The focus rule is not decoration** -
beercss reveals a tooltip on hover alone, so without it a keyboard never reaches one and a touch device has
no hover to give.

## 2. The submit button can stick disabled

**This one is a defect, and it is latent rather than live.**
`packages/binacle-net-ui/src/apps/packingDemo/packingDemo.ts:177` sets `submitting = true` in `onSubmit`, and the only
thing that clears it is the `finally` at `:220` - which sits inside the thunk handed to
`$dispatch('update-scene', ...)`. **Nothing runs that thunk unless a visualizer is listening.**

Both packing pages include the visualizer today, so nothing is broken in public. A page that embeds the
component without one gets a button stuck disabled and a status stuck on "Packing...".

**What has to be decided is what the button waits on.** Clearing `submitting` when the response arrives,
rather than when something renders it, is the obvious shape - but say it in the code, at the point it is
taken.

## 3. Show the visitor the request that was just made

The demo already holds real numbers - the visitor's own boxes and items - and already builds a request body
from them. Show it: a panel beside the results with the exact call that was sent, ready to copy.

```
POST http://localhost:8080/api/v3/pack/by-custom
Content-Type: application/json

{ "bins": [ ... ], "items": [ ... ], "parameters": { ... } }
```

**The UI Module is the host worth having it on**, because the module is served from the instance the visitor
is running, so the host in the snippet is one they can paste into their own code. The same panel on a public
demo site can only ever print a public host nobody will call.

**Five questions, and the fourth decides the cost:**

- **What form.** Raw HTTP, a `curl` line, or a language snippet. `curl` pastes into a terminal; raw HTTP
  matches the documentation. Probably not both.
- **Where the URL comes from.** The demo's `baseUrl` is empty by default and the browser resolves it
  relative, so the panel has to read the page's own origin rather than the value handed to the component.
- **Which API version.** The component posts to `/api/v3/pack/by-custom` today. A panel that teaches people
  the call teaches whichever version it prints, so this interacts with moving the shipped clients off v3 -
  printing v3 while the documentation recommends v4 is worse than printing nothing.
- **Inside the tool or around it. Answer this one first.** In the shared component it lands on both hosts,
  where it is worth much less. In the Razor page it has to read the component's state, which is a seam that
  does not exist yet.
- **The response half.** Showing the response too doubles the panel and the visualizer already shows that
  result. It may be the request alone.

---

## Two neighbours that are not in here

**The instance page's presets** are a separate file - that work deletes `_js/instance.js` rather than
changing it, and it touches `Kernel` rather than the demo component.

**Moving the shipped clients off v3** is also separate, and question 3 above waits on the same answer it
does: what the UI changes to.

## Done when

- [x] **2026-09-11, confirmed by the maintainer.** A partial result names the items it could not fit, in a tooltip, and the result row keeps its height.
      **By eye.** Randomize to `02-packs-nowhere` and reach `Could not fit 2 items` and `2 x 20x20x20-3`
      from the row without the row growing. Hover, touch and keyboard all reach it.
- [x] **2026-09-11.** The four helpers and their ten tests are still there and still drive the tooltip.
      `grep -c 'unpackedItemsTitle\|unpackedItemText\|hasUnpackedItems\|unpackedItemsOf'` returns 6 on
      `packages/binacle-net-ui/src/apps/packingDemo/packingDemo.ts` and 10 on
      `packages/binacle-net-ui/tests/apps/packingDemo/packingDemo.test.ts`.
- [x] **2026-09-11.** Both hosts render it, to the same count - 1 and 1.
      `grep -c hasUnpackedItems sites/demo/pages/packing.html` and the same on
      `api/src/Binacle.Net.UIModule/Pages/Packing.cshtml` return the same non-zero number.
- [x] **2026-09-11, confirmed by the maintainer.** The submit button cannot stay disabled when no visualizer is listening.
      **By eye.** What clears `submitting` runs whether or not anything handles `update-scene`. Render the
      demo component on a page with no visualizer, submit, and the button comes back.
- [ ] The request panel shows the call that was actually sent, against the host the page is served from.
      **By eye.** Open the packing page on a running container, submit, and paste what the panel prints into
      a terminal. It answers.
- [ ] The five questions above are answered in the code, readable where the answer was taken.
      **By eye.** If an answer is only in this file, the box is open.
