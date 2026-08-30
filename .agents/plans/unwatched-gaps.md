---
description: Two repository gaps nothing watches - a generated copy on no drift check, and a v4 caller inside the image
state: proposed
waits-on: "a yes or a no per gap. State chosen by an agent to make the file legible - strike it if it is wrong."
paths:
  - "sites/**"
  - "tooling/**"
  - "api/src/Binacle.Net.UIModule/**"
---

# What nothing is watching

**Each of these is a gap in coverage, not a defect, and each needs a yes or a no.** Re-checked against the
tree on 2026-08-27. The five that had a known answer and no decision in them moved to the TODO list.

## The frozen OpenAPI copies on the docs site drift with nothing watching

`sites/docs/collections/_versions/*/swagger/*.json` are hand-placed copies. `tooling/openapi.just` writes
only to `artifacts/openapi`, and `tooling/regen.just check` - the recipe that exists to fail on generated
data drifting - lists six paths and none is a swagger folder. The docs deploy row in the release plan is the
only control.

## The instance page is the only v4 consumer that ships in the image

`api/src/Binacle.Net.UIModule/_js/instance.js:52` fetches `/api/v4/presets`; everything else in the module
and both site demos are on v3. `sites/www/pages/how-it-works.html:134` says v4 "can change in a patch
release. Do not integrate against it by accident."

## Done when

- [ ] The docs site's swagger copies fail a check when they drift from the generator.
      `grep swagger tooling/regen.just` matches, or another recipe compares the two.
- [ ] The v4 dependency in the shipped image is deliberate.
      **By eye.** Either `_js/instance.js:52` is on v3, or a design record says why the image ships a v4
      caller.
- [x] **`/error/404` answers 404 - done 2026-08-31.** `Error.cshtml.cs` sets the status when the route names
      one in the 400 to 599 range, and both suites cover it. Outside that range the page falls back and stays
      200, because the re-execute cannot produce such a code and a typed one is not a status to answer with.
