---
description: Repository gaps nothing watches - a shipped file on no assertion, a generated copy on no drift check, a pull request that runs no job. Each is a gap, not a break.
state: proposed
waits-on: "a yes or a no per gap. State chosen by an agent to make the file legible - strike it if it is wrong."
paths:
  - "sites/**"
  - ".github/workflows/**"
  - "tooling/**"
  - "api/src/Binacle.Net.UIModule/**"
---

# What nothing is watching

**Every item here is a gap in coverage, not a defect.** Each was re-checked against the tree on 2026-08-27.

## No pull request builds a site, and `sites/` is outside the gate

`pull-request.yml:42` classes everything under `sites/` as not-code, and `test-suite`, `image` and `workflows`
are all gated on `code == 'true'`. **A pull request touching only `sites/www` runs zero jobs and passes green.**
There is no Jekyll build job in the workflow at all - the first time a site's build runs in CI is the deploy.

## Three shipped bundles and the framework stylesheet are on no smoke assertion

`vendors.js`, `binacle-net-ui.js` and `binacle-vipaq.js` are requested by every applet page; `lib/beercss/beer.min.css`
is the whole framework, and `build.just:45` says skipping the asset copy "ships pages with no styling". None is
in `tooling/smoke/structure.yaml`, whose opening comment calls itself "a complete declaration ... it asserts
every shipped file". All return 200 on a running instance, so this is coverage, not a break.

## The frozen OpenAPI copies on the docs site drift with nothing watching

`sites/docs/collections/_versions/*/swagger/*.json` are hand-placed copies. `tooling/openapi.just` writes only
to `artifacts/openapi`, and `tooling/regen.just check` - the recipe that exists to fail on generated data
drifting - lists six paths and none is a swagger folder. The docs deploy row in the release plan is the only
control.

## The instance page is the only v4 consumer that ships in the image

`api/src/Binacle.Net.UIModule/_js/instance.js:52` fetches `/api/v4/presets`; everything else in the module and
both site demos are on v3. `sites/www/pages/how-it-works.html:134` says v4 "can change in a patch release. Do
not integrate against it by accident."

## A locally built image carries a source map the release image does not

`wwwroot/css/main.css.map` is left behind by `npm run watch:css` (`package.json:12`, which unlike `build:css`
at `:9` passes no `--no-source-map`), and nothing cleans it: webpack's `clean` applies to `output.path`, which
is `wwwroot/js` only. CI checks out fresh and has none. `build.just:13` says the maintainer's image and the
release image come from one recipe; on any machine that has run the watch, they do not.

## `api_url` is declared on three sites and read by one

All three sites set it in `_config.yml` and `_config.prod.yml`. The only reader is
`sites/demo/pages/packing.html:26`.

## The demo pages carry two live flags and only one is documented

`sites/demo/pages/packing.html` and `vipaq.html` set both `applet: true` and `demo: true`. Both are read -
`applet` by `_includes/navbar/menu.html:1`, `demo` by `_layouts/page.html:11`. `_data/includes.yml:42`
documents only `demo`. The `_apps` removal, half-done.

## `/error/404` answers 200

`Pages/Error.cshtml` is routed at `/error/{errorCode?}` and `Error.cshtml.cs` never sets `Response.StatusCode`,
so a monitor hitting that address directly is told everything is fine. Re-executed requests
(`ModuleDefinition.cs:48`) keep their real status; a direct hit does not.

## The instance page prints its runtime as a literal

`Pages/Instance.cshtml:26` prints `<code>.NET 10</code>` as text, on the page whose job is reporting what the
container runs. Every other row on it comes from `@Model`.

## Done when

- [ ] A pull request touching only `sites/` runs at least one job.
      **By eye.** `pull-request.yml:42` no longer sends `sites/` to `code=false`, or a site build job exists
      that is not gated on `code`.
- [ ] The three bundles and `beer.min.css` are asserted in the image.
      `grep -c 'vendors.js\|binacle-net-ui.js\|binacle-vipaq.js\|beer.min.css' tooling/smoke/structure.yaml`
      returns 4 or more.
- [ ] The docs site's swagger copies fail a check when they drift from the generator.
      `grep swagger tooling/regen.just` matches, or another recipe compares the two.
- [ ] The v4 dependency in the shipped image is deliberate.
      **By eye.** Either `_js/instance.js:52` is on v3, or a design record says why the image ships a v4 caller.
- [ ] A local build and a CI build produce the same `wwwroot/`.
      `just build image` on a tree that has run `just serve api U`, then
      `test ! -f api/src/Binacle.Net.UIModule/wwwroot/css/main.css.map` inside the publish output.
- [ ] `api_url` is read by every site that declares it, or declared only by the site that reads it.
      `grep -rl api_url sites/*/_config*.yml` and `grep -rn 'site.api_url' sites/` name the same sites.
- [ ] One flag drives the demo pages, or both are documented.
      **By eye.** `sites/demo/_data/includes.yml` explains `applet` as well as `demo`, or one flag is gone
      from `pages/packing.html` and `pages/vipaq.html`.
- [ ] `/error/404` answers 404.
      `curl -o /dev/null -s -w '%{http_code}' http://localhost:8080/error/404` prints 404.
- [ ] The instance page reports its runtime from the runtime.
      `grep -n '\.NET 10' api/src/Binacle.Net.UIModule/Pages/Instance.cshtml` finds nothing.
