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

**Merged from `packing-demo-bugs.md` and `show-me-the-request.md`.** They were filed as a bug list and an
idea, and they are neither: they are the next three pieces of work on the same surface, and the first two
were the last survivors of a ten-item review where the other eight are fixed.

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
`unpackedItemsTitle` and `unpackedItemText` are in `packages/binacle-net-ui/src/core/packingDemo.ts` with ten
tests behind them in `packages/binacle-net-ui/tests/components/packingDemo.test.ts`. Whoever builds the
tooltip starts from checked strings.

**Both hosts or neither.** The demo site and `Binacle.Net.UIModule` consume the same package; one host is
not the feature.

## 2. The submit button can stick disabled

**This one is a defect, and it is latent rather than live.**
`packages/binacle-net-ui/src/core/packingDemo.ts:183` sets `submitting = true` in `onSubmit`, and the only
thing that clears it is the `finally` at `:225` - which sits inside the thunk handed to
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
- [ ] The request panel shows the call that was actually sent, against the host the page is served from.
      **By eye.** Open the packing page on a running container, submit, and paste what the panel prints into
      a terminal. It answers.
- [ ] The five questions above are answered in the code, readable where the answer was taken.
      **By eye.** If an answer is only in this file, the box is open.
