# Sites

Every site Binacle.Net publishes, one directory each. All three are [Jekyll](https://jekyllrb.com/) sites
built with webpack and TypeScript beside them.

## 📂 The sites

| Directory | What it is |
|---|---|
| [`docs/`](docs) | The documentation site - versioned API reference and guides |
| [`demo/`](demo) | The demo site - the packing demo, the ViPaq decoder, and the pages around them |
| [`www/`](www) | The marketing site - what Binacle.Net does, and who it is for |

Each has its own `README.md`, `Gemfile` and `package.json`. **All three `package.json` files are root npm
workspace members**, so one `npm install` at the root covers them and none has a lock file of its own. Ruby is
still per site. `just install` from the repo root runs the root install and then `bundle install` in each.

**`www` is the odd one out.** It runs no CSS framework and compiles its own stylesheet with the sass CLI
rather than through Jekyll, so `css/` is build output there and committed source on the other two. Its
[`README.md`](www) says what else differs.

## 🛠️ Building and serving

From the repo root, one pair of recipes per site:

```bash
just serve docs                  # jekyll serve + webpack watch, one Ctrl-C stops both
just build docs                  # the same site built once, into artifacts/docs
```

`serve demo`, `build demo`, `serve www` and `build www` are the same for the other two. A build is three
steps in a fixed order - copy the shared assets, compile the scripts (and on `www` the stylesheet too), then
`jekyll build` - and **skipping any of them still produces a site**, just one with no scripts and no logo.
Use the recipes rather than calling `jekyll` yourself.

Output goes to `artifacts/<site>` at the repo root, which is what gets deployed.

## ☁️ Deploying

All three go to Cloudflare, each from its own workflow - `Deploy Docs Site`, `Deploy Demo Site` and
`Deploy WWW Site`. All are **manual** (`workflow_dispatch`), all build the site fresh, check its links
offline, upload `artifacts/<site>`, and then tag the commit they published so a live site maps back to a
commit.

The wrangler config for each lives in [`tooling/cloudflare/`](../tooling/cloudflare). The `directory` it
uploads has to match the `destination` in that site's `_config.yml`.

Shared static assets live in [`assets/`](../assets) and are copied in by gulp; the custom Liquid filters and
Google Tag Manager tags come from the local gems in [`ruby/`](../ruby).
