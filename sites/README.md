# Sites

Every site Binacle.Net publishes, one directory each. All three are [Jekyll](https://jekyllrb.com/) sites
built with webpack and TypeScript beside them.

## 📂 The sites

| Directory | What it is |
|---|---|
| [`docs/`](docs) | The documentation site - versioned API reference and guides |
| [`demo/`](demo) | The demo site - the packing demo, the ViPaq decoder, and the pages around them |
| [`www/`](www) | The marketing site - what Binacle.Net does, and who it is for |
| [`admin/`](admin) | **Experimental, local only - not published.** A page to manage a local ServiceModule's accounts. No build recipe, no assets copy, no workflow names it |
| [`ruby/`](ruby) | Not a site: the Jekyll gems only these sites read - the docs' version scheme and markup, and the `robots.txt` body |

Each has its own `README.md`, `Gemfile` and `package.json`. **All three `package.json` files are root npm
workspace members**, so one `npm install` at the root covers them and none has a lock file of its own. Ruby is
still per site. `just install` from the repo root runs the root install and then `bundle install` in each.

**The look comes from [`shared/packages/design-system`](../shared/packages/design-system).** Each site's
`_sass/main.scss` names the parts it uses, and the sass CLI compiles it rather than Jekyll, so `css/` is build
output on all three.

## 🛠️ Building and serving

From the repo root, one pair of recipes per site:

```bash
just serve docs                  # jekyll serve + sass and webpack watches, one Ctrl-C stops all three
just build docs                  # the same site built once, into artifacts/docs
```

`serve demo`, `build demo`, `serve www` and `build www` are the same for the other two. A build is three
steps in a fixed order - copy the shared assets, compile the stylesheet and the scripts, then
`jekyll build` - and **skipping any of them still produces a site**, just one with no styles, no scripts and no logo.
Use the recipes rather than calling `jekyll` yourself.

Output goes to `artifacts/<site>` at the repo root, which is what gets deployed.

## ☁️ Deploying

All three go to Cloudflare from one workflow, `Deploy Site`, with the site picked at dispatch - `docs`, `demo`
or `www`. It is **manual** (`workflow_dispatch`), builds the site fresh, checks its links offline, uploads
`artifacts/<site>`, and then tags the commit it published so a live site maps back to a commit.

The wrangler config for each lives in [`tooling/cloudflare/`](../tooling/cloudflare). The `directory` it
uploads has to match the `destination` in that site's `_config.yml`.

Shared static assets live in [`assets/`](../assets) and are copied in by gulp; the custom Liquid filters and
Google Tag Manager tags come from the local gems in [`ruby/`](../ruby), and the gems only these sites read
from [`ruby/`](ruby) here.
