---
description: The guidance drift a third site left, and the repository gaps nothing watches. A findings list - each item is fixed, moved onto the plan it concerns, or written down as a decision not to act.
state: ready
waits-on: "nothing"
paths:
  - ".agents/**"
  - "sites/**"
  - ".github/workflows/**"
---

# The drift a third site left, and the gaps nothing watches

**A findings list, not a backlog.** Made on 23 Aug 2026 against `65b1aaaf`, which is now `main`. **An item
leaves this file when it is fixed, when it moves onto the plan it concerns, or when someone writes down that
it will not be acted on.** The file is deleted when it is empty.

Everything below was verified by reading the file named, unless it says otherwise.

---

## 1. The five entry-point docs - **fixed 2026-08-25, one part left**

**Fixed by reading the tree, not the list below.** `docs/README.md` now counts ten workflows, three site
deploys, and carries `sites/www/` in the layout table and a WWW row in Slice Docs. `build-topology.md` reads
46 projects (34 `.csproj`, seven `.proj`, five `.dcproj`), six workspace entries and seven content projects.
`tooling/README.md` gained a row for `tooling/cloudflare/` and names `just serve www`.
`rules/never-edit-published-sites.md` names all three sites and four README carve-outs.

**`commands.md` and `ci-cd/README.md` needed nothing** - both had already been brought up to three sites and
ten workflows. **The claim that `agents.just` loops over six indexes was wrong**: it loops over five - rules,
docs, design, plans, memory - and `commands.md` already lists all five including `rules`.

**The prose sweep is done too, 2026-08-25.** `README.md`, `DEVELOPMENT.md` (three rows, including the Ruby
one that listed two `.ruby-version` files), `assets/README.md` and `tooling/lychee.toml` all say three sites
now. `plans/architecture-checks.md` says nine `tsconfig.json` files, not seven.

**Two claims on this list were wrong.** `docs/packages/README.md` already said "all four hosts" - and `www`
does depend on `theme-switcher`, so the premise that it uses neither package was false. **`docs/ruby/README.md`
was genuinely wrong** and is fixed: all three sites set an empty container id in `_config.yml` and a real one
in `_config.prod.yml`, so GTM is off locally and on in every published build - the doc said off everywhere.

**Two of the three left were closed by deletion on 2026-08-25.** The root `architecture.yml` listed two sites
and had no `www` slice; `tooling/tmux.sh` had no `www` window. **Both files are gone** - the declaration
because nothing read it and this plan's own generator replaces it, the session script because it was a staging
layout one person used. **One left, and it is not a wording fix.**

- **`.github/dockerhub-overview.md:150` says "Website and demo" with one address.** **It is correct today** -
  the marketing host really does serve both. It becomes wrong when the demo moves to its own host, and that
  page republishes on every release, so it corrects itself then. **Recorded so nobody fixes it early and
  points readers at a host with no DNS.**

**Two historical sentences left alone.** `plans/ci-cd/ci-gates.md:176` and `plans/sonar-issue-triage.md:22`
describe a 2026-08-09 change, when there really were two sites.

## 2. Done - 2026-08-25

**`docs/sites/www.md`** said `_sass/` was four partials plus `main.scss` and listed five. There are five.
**The "five live defects" table it also carried is already gone** - nothing in the file mentions it.

## 3. The design record still deploys to DigitalOcean

`design/ci-cd/decisions.md`, **D17** - *"a stopped deploy leaves App Platform mid-rollout"*. All three deploys
use `cloudflare/wrangler-action` against `tooling/cloudflare/*.wrangler.jsonc`. **The `check:` line is fixed**
- it counted two deploy workflows and now counts three. **The reasoning is not**, and it should not be edited
blind: whether a stopped wrangler deploy leaves anything mid-rollout is a fact nobody here has established, so
D17's justification needs re-deriving rather than rewording.

**Nothing anywhere in `.agents/` names Cloudflare or wrangler** except two lines of `docs/sites/www.md`, one
row of the CI/CD secrets table, and the `tooling/cloudflare/` row added on 2026-08-25. The host moved and the
guidance layer did not follow it.

## 4. Layer violations - done, 2026-08-25

**All four are fixed.** `design/ci-cd/decisions.md` no longer cites a plan - it names the work in words.
`plans/ci-cd/dockerhub-overview.md` no longer cites `$ci-cd`, `$ci-cd/release-pipeline`, `$tooling` or
`$commands`. `rules/never-edit-published-sites.md` names all three sites and four README carve-outs.
**`plans/api/ui-clients-off-v3.md` already carried its blocker** - its `waits-on:` names the public API host
and splits the two halves. That finding was stale when it was written down.

## 5. Front matter and one missing type

- **`design/sites/decisions.md`** still says *"`sites/www` is not covered here. It is being built in its own
  session and owns its own record"*, and its `paths:` is demo-and-docs only. The site landed. Nothing checks
  `www`'s decisions against anything. **Whether `www` joins that record or gets its own is a decision, not a
  correction.**
- **`docs/api/kernel.md`** does not mention `ReservedPathOptions.cs`, added to `Kernel/` on 22 Aug and read by
  three modules and `Program.cs`. Two other docs point readers at it as a Kernel type. Its `check:` also asks
  for a section per folder, and `HealthChecks/` and `Serialization/` have none - that half predates the branch.
  **Writing that section means reading the type, not renaming a heading.**
- **Dates.** `docs/README.md`, `build-topology.md`, `tooling/README.md`, `docs/ruby/README.md`,
  `docs/sites/www.md` and `design/ci-cd/decisions.md` were re-read and their `verified:` dates bumped on
  2026-08-25. `commands.md` was read and needed no change, so **its date was deliberately left alone**.
- **`docs/sites/demo.md`** was read in full against the tree and **nothing in it is wrong**. Only the date is
  behind.
- **`memory/bulk-rename-traps.md`** - fixed. It named two UI module types that no longer exist as the
  collision that forced per-file usings; it now names the api's `v3`/`v4` `Contracts.Algorithm`, which does
  still collide, and records that the UI module pair is gone.

---

## What nothing is watching, in the repository itself

**None of this blocks the tag. All of it is a gap rather than a break.**

**No pull request builds a site, and `sites/` is excluded from the gate entirely.** `pull-request.yml`'s
`changes` job treats everything under `sites/` as not-code, and `test-suite`, `image` and `workflows` are all
gated on `code == 'true'`. **A pull request touching only `sites/www` runs zero jobs and passes green.** There
is no Jekyll build job in the workflow at all - the first time a site's build runs in CI is the deploy itself.
The filter predates the branch; what is new is that it now covers a third site with the most fragile build of
the three.

**Three shipped bundles and the framework stylesheet are on no smoke assertion.** `vendors.js`,
`binacle-net-ui.js` and `binacle-vipaq.js` are requested by every applet page; `lib/beercss/beer.min.css` is
the entire framework, and `build.just`'s own comment says skipping the asset copy "ships pages with no
styling". All return 200 on a running instance, so this is coverage, not a break. `structure.yaml` opens by
calling itself "a complete declaration ... it asserts every shipped file".

**The frozen OpenAPI copies on the docs site drift with nothing watching.** `tooling/openapi.just` writes only
to `artifacts/openapi`, and `tooling/regen.just check` - the recipe that exists to fail on generated data
drifting - lists five globs and none is the swagger folder. The docs deploy row in the release plan is
currently the only control.

**The instance page is the only v4 consumer that ships in the image.** `_js/instance.js:52` fetches
`/api/v4/presets`; everything else in the module and both site demos are on v3. The marketing page written on
the same branch says v4 "can change in a patch release. Do not integrate against it by accident."

**A locally built image carries a source map the release image does not.**
`wwwroot/_content/Binacle.Net.UIModule/css/main.css.map` is left behind by `npm run watch:css`, which
`just serve api U` runs, and nothing cleans it: webpack's `clean` covers `wwwroot/js` only. CI checks out
fresh and has none. `build.just` opens by saying the maintainer's image and the release image come from one
recipe; on any machine that has run the watch, they do not.

**Four smaller ones.** `api_url` is declared on all three sites and read by one - `sites/demo/pages/packing.html`.
The demo pages carry **both** `applet: true` and `demo: true`, both live, read by different includes, and
`_data/includes.yml:25` documents only `demo` - the `_apps` removal half-done. `/error/404` answers **200**,
so a monitor hitting it is told everything is fine. `Instance.cshtml:23` prints `<code>.NET 10</code>` as a
literal on the page whose job is reporting what the container runs.

**All three `robots.txt` files** carry ~25 lines of content-signal preamble above a `# Content-Signals:` line
that is commented out. The declaration is inert on every site. Pre-existing and identical across the three.

---

## One clean result worth keeping

**All six generated `_index.md` files match disk exactly**, descriptions included. Nothing to do.

## Done when

- [ ] Every finding above is gone from this file.
      Each one left as a fix, as front matter on the plan it concerns, or as a written decision not to act.
      **By eye** - a finding still written here is a finding still open.
- [ ] The five entry-point docs count three sites, and their own `check:` lines pass.
      Run each doc's `check:` line. `docs/README.md`'s compares its workflow count to `.github/workflows/`.
- [ ] The file is deleted.
      `test ! -f .agents/plans/agents-doc-drift.md`.
