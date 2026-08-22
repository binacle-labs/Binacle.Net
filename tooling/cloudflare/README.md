# Cloudflare

One wrangler config per site - `docs.wrangler.jsonc`, `demo.wrangler.jsonc` and `www.wrangler.jsonc`. They are
the whole deployment configuration for [`sites/docs`](../../sites/docs), [`sites/demo`](../../sites/demo) and
[`sites/www`](../../sites/www).

**Nothing here is run by hand.** The `Deploy Docs Site`, `Deploy Demo Site` and `Deploy WWW Site` workflows
call `wrangler deploy --config` against them; all three are manual, and all three tag the commit they
published.

## ⚙️ What each one sets

| Key | Why |
|---|---|
| `assets.directory` | The built site to upload - `artifacts/docs`, `artifacts/demo` or `artifacts/www`. Relative to **this file**, and it has to match the `destination` in that site's `_config.yml` |
| `assets.not_found_handling` | `404-page`, which wants `404.html` in the site root. That is why each site's `pages/404.html` carries `permalink: /404.html` |
| `preview_urls` | Off. A deploy is meant to be the site, not a second copy of it on another URL |
| `observability` | On. Request metadata, 404s and exceptions - the only way to see a dead inbound link, because the link check runs offline against the built folder and cannot know what anyone typed |

## ⚠️ The Worker name is set once

`name` is the Worker. **Renaming it after the first deploy creates a second Worker**, leaves the first one
running, and detaches the custom domain from it. Get it right before the first run, not after.

## ⚠️ Change the path in two places

`assets.directory` here and `destination` in the site's `_config.yml` name the same folder. Change one alone
and the deploy uploads a folder the build never wrote - or worse, a stale one from an earlier build.
