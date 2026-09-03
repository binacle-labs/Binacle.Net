---
id: sites/site-build
description: The webpack and sass machinery behind the three sites - the clean rule that keeps a watch alive, the chunk split and the asset budget, and the cache that lies about a clean build.
verified: 2026-09-04
check: B1 against the clean option in the main config of all three webpack.config.js files, which must be a keep regex in production and false otherwise; B2 against the cacheGroups and performance blocks in sites/demo/webpack.config.js; B3 against sites/www/webpack.config.js, which must declare no splitChunks on its main config, and against the empty dependencies in sites/www/package.json; B4 against cache.type filesystem in sites/demo/webpack.config.js
paths:
  - "sites/www/webpack.config.js"
  - "sites/docs/webpack.config.js"
  - "sites/demo/webpack.config.js"
  - "sites/*/package.json"
---

# Building the three sites

**These were comments in the three `webpack.config.js` files until 4 Sep 2026.** Why there are two configs
rather than two entries, and why the theme bundle overrides ts-loader to `esnext`, is
`$sites/www-design#W2`; why www's stylesheet is built by the sass CLI is `$sites/www-design#W3`.

## B1 - `clean` is a keep regex in production and off in watch

**Watch mode shares `js/` with a running jekyll.** Deleting a file jekyll has already listed makes its next
`File.stat` raise `ENOENT` and kills the serve, so `clean` is `false` outside a production build.

**In production the two configs run in parallel into one directory**, so cleaning has to spare the other's
output - `{keep: /^theme-init\.js$/}`. Without it the two race and `theme-init.js` is deleted after it is
written.

## B2 - the demo's chunk split is an asset budget, not tidiness

`three` gets its own cache group at priority 30 so it stays cached when app code or Alpine change; the size
and the prefetch that pays for it are `$sites/docs-and-demo-design#D4`. `binacle-net-ui` and `binacle-vipaq`
sit at 20 so they beat the generic `vendors` group at 10.

**The performance budget is 300 KiB per asset and per entrypoint, and `three` is filtered out of it.** three
is unavoidable on the demo pages; the filter is there so the budget still means something for everything
else, rather than being permanently over and permanently ignored.

## B3 - www has one config with no split at all

**This site has no npm dependencies**, so there is nothing to split out. No `splitChunks`, no vendors group.
The whole main bundle is the copy button and the switcher registration in `_js/main.ts`.

## B4 - the filesystem cache reports success on source that fails a cold build

Type errors included. **Delete `node_modules/.cache/webpack` before trusting a clean run.**

## B5 - all three sites compile sass compressed

`style: compressed` in the docs and demo `_config.yml`, `--style=compressed` in www's `build:css`. **A `/* */`
comment in sass survives into expanded output and does not survive compressed output**, which is why the
loud-comment form was safe in these files and is not a reason to use it.
