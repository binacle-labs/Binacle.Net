---
description: The packing demo shows the visitor the HTTP call that was just made - a panel beside the results with the exact request, ready to copy
state: ready
waits-on: "a session of its own - the maintainer put it in v3.1.0 on 2026-09-11. One question is answered first in that session: inside the shared component or in the Razor page around it"
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "packages/binacle-net-ui/**"
  - "sites/demo/**"
---

# The packing demo - show the visitor the request that was just made

**This file used to hold three items.** The unpacked-items tooltip and the sticking submit button landed on
2026-09-10 and 2026-09-11 and were trimmed out on 2026-09-12; their reasoning is in the packages decisions
ledger. This is what is left.

The demo already holds real numbers - the visitor's own boxes and items - and already builds a request body
from them. Show it: a panel beside the results with the exact call that was sent, ready to copy.

```
POST http://localhost:8080/api/v4/pack/compare-bins
Content-Type: application/json

{ "bins": [ ... ], "items": [ ... ], "parameters": { ... } }
```

**The UI Module is the host worth having it on**, because the module is served from the instance the visitor
is running, so the host in the snippet is one they can paste into their own code. The same panel on a public
demo site can only ever print a public host nobody will call.

**Four questions, and the third decides the cost:**

- **What form.** Raw HTTP, a `curl` line, or a language snippet. `curl` pastes into a terminal; raw HTTP
  matches the documentation. Probably not both.
- **Where the URL comes from.** The demo's `baseUrl` is empty by default and the browser resolves it
  relative, so the panel has to read the page's own origin rather than the value handed to the component.
- **Inside the tool or around it. Answer this one first.** In the shared component it lands on both hosts,
  where it is worth much less. In the Razor page it has to read the component's state, which is a seam that
  does not exist yet.
- **The response half.** Showing the response too doubles the panel and the visualizer already shows that
  result. It may be the request alone.

**The version question is answered.** The component calls `pack/compare-bins` on v4 through
`packages/binacle-net-client` - `packCompareBinsPath` in `client.ts` is the one place the path is written -
so the panel prints v4, which is what the documentation recommends.

## Done when

- [ ] The request panel shows the call that was actually sent, against the host the page is served from.
      **By eye.** Open the packing page on a running container, submit, and paste what the panel prints into
      a terminal. It answers.
- [ ] The four questions above are answered in the code, readable where the answer was taken.
      **By eye.** If an answer is only in this file, the box is open.
