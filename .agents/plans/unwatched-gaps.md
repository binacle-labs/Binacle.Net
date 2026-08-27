---
description: Three repository gaps nothing watches - a generated copy on no drift check, a v4 caller inside the image, and an error page that answers 200
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

## `/error/404` answers 200

`Pages/Error.cshtml` is routed at `/error/{errorCode?}` and `Error.cshtml.cs` never sets
`Response.StatusCode`, so a monitor hitting that address directly is told everything is fine. Re-executed
requests (`ModuleDefinition.cs:48`) keep their real status; a direct hit does not.

## Done when

- [ ] The docs site's swagger copies fail a check when they drift from the generator.
      `grep swagger tooling/regen.just` matches, or another recipe compares the two.
- [ ] The v4 dependency in the shipped image is deliberate.
      **By eye.** Either `_js/instance.js:52` is on v3, or a design record says why the image ships a v4
      caller.
- [ ] `/error/404` answers 404.
      `curl -o /dev/null -s -w '%{http_code}' http://localhost:8080/error/404` prints 404.
