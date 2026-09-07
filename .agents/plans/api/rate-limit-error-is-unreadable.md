---
description: A rate-limited packing request tells the visitor the error response could not be parsed, which is not what happened and not what they need to know
state: idea
waits-on: "nobody - it is an idea"
paths:
  - "packages/binacle-net-ui/**"
---

# A 429 reads as a parse failure

The one error a visitor to the public demo is most likely to see is the one that reads as broken software.

## What happens, read in the code 2026-09-07

`handleErrorResponse` in `packages/binacle-net-ui/src/core/packingDemo.ts:122` builds a title from
`response.statusText`, falling back to `getResponseStatusText(response.status)` - which maps `429` to
`Too Many Requests`. That half is right.

It then calls `await response.json()` unconditionally. **The rate limiter returns no body**, so the parse
throws, the `catch` pushes `An error occurred, but the error response could not be parsed.` into the errors
list, and the dialog shows that sentence under the title.

So the visitor is told the software failed to read a reply, when what actually happened is that they were
asked to wait. **Nothing failed.**

## Why it reaches a real person

Anonymous callers to the public API share one bucket, so a 429 is a normal outcome on the demo host and not
an edge case. Inside the image the limiter only exists when the Service Module is on, so a self-hoster
running the shipped defaults never sees it.

## What to settle while fixing it

- **Whether an empty body is an error path at all.** A 429 with no body is the documented shape, not a
  malformed reply. The same is true of any status the API answers without a `ProblemDetails`. Decide whether
  the code branches on the body being absent or on the status.
- **What the 429 says.** It is the only status here where the visitor can do something - wait - and the
  message is the only place that can say so.
- **Whether the parse-failure sentence survives.** It is worth keeping for a genuinely malformed reply. It is
  wrong for every reply that simply has no body.

## Done when

- [ ] A 429 says the caller is rate limited and does not mention parsing.
      **By eye.** Force one against the public host, or stub a 429 with an empty body, and read the dialog.
- [ ] An empty body is not treated as a parse failure on any status.
      **By eye** in `handleErrorResponse`. If the only guard is a `try`/`catch` around `response.json()`, the
      box is open.
- [ ] The answer is readable where it was taken, not only here.
      **By eye.** A comment at the branch saying why an absent body is not an error.
