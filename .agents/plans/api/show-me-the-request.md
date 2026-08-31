---
description: The packing demo shows the HTTP call it just made, against this host, ready to copy
state: idea
waits-on: "nobody - it is an idea"
horizon: near
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "packages/binacle-net-ui/**"
---

# Idea: show the request the demo just made

The packing demo already holds real numbers - the visitor's own boxes and items - and already builds a request
body from them. Show it: a panel beside the results with the exact call that was sent, ready to copy. The UI
module is the host worth having it on, because the module is served from the instance the visitor is running,
so the host in the snippet is one they can paste into their own code. A copy of the same panel on a public demo
site can only ever print a public host nobody will call.

```
POST http://localhost:8080/api/v3/pack/by-custom
Content-Type: application/json

{ "bins": [ ... ], "items": [ ... ], "parameters": { ... } }
```

## Research

### Date not recorded - nothing in the image shows a request today

Someone who has watched the visualizer work still has to open the API documentation and rebuild the same call
by hand.

### Date not recorded - five open questions, and the fourth decides the cost

- **What form.** Raw HTTP, a `curl` line, or a language snippet. `curl` pastes into a terminal; raw HTTP
  matches the documentation. Probably not both.
- **Where the URL comes from.** The demo's `baseUrl` is empty by default and the browser resolves it relative,
  so the panel has to read the page's own origin rather than the value handed to the component.
- **Which API version.** The component posts to `/api/v3/pack/by-custom` today. A panel that teaches people the
  call teaches whichever version it prints, so this interacts with moving the shipped clients off v3 - printing
  v3 while the documentation recommends v4 is worse than printing nothing.
- **Inside the tool or around it.** In the shared component it lands on both hosts, where it is worth much
  less. In the Razor page it has to read the component's state, which is a seam that does not exist yet.
  Answer this one first.
- **The response half.** Showing the response too doubles the panel and the visualizer already shows that
  result. It may be the request alone.
