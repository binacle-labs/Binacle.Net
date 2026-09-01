---
description: v4 - flip from experimental to stable
state: idea
waits-on: "an endpoint added to v4 that reshapes no existing contract - none has been chosen"
horizon: near
paths:
  - "api/**"
---

# v4 - flip from experimental to stable

v4 ships experimental and stays that way until it has run in a real deployment and at least one endpoint has
been added to it without reshaping an existing request or response. The second condition is the evidence that
the contract shape holds, which is the whole claim "stable" makes; if adding an endpoint forces an existing
contract to change, v4 is not ready and the right move is to make that change while it is still experimental.
Nobody knows what the contents will be beyond that, and the only candidate anyone has written up is
`pack/first-bin`.

## Done when

- [ ] `IsExperimental` is false and the comment above it is gone.
      `grep -n 'IsExperimental' api/src/Binacle.Net/v4/ApiV4Document.cs` returns `false`.
- [ ] The published spec carries no experimental banner.
      `grep -c experimental sites/docs/**/swagger/v4.json` returns 0, and the v4 docs pages drop the marking.
- [ ] The release notes say v4 is now stable and grows by adding endpoints.
      **By eye** in `CHANGELOG.md`.
- [ ] The v4 agent doc no longer records the experimental marking as current truth.
      **By eye.**

## Research

### Date not recorded - why it ships experimental first

v4 has never been called by a real user. Shipping it stable would lock its contracts on the strength of a
design nobody has used, and every later reshape becomes a breaking change against people who trusted the
document. Experimental costs one boolean and some adoption for one release; getting it wrong the other way
costs a breaking change in a version that promised none.

**The API declares this itself.** `ApiV4Document.IsExperimental` drives a warning banner into the published
OpenAPI description - "This API version is experimental and may change any time, introducing breaking changes".
v3 sets it false, v4 sets it true.
