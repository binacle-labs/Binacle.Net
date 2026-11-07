---
description: Restructure and rename the npm packages and Ruby gems by what each one knows and who uses it, not by language - and give each site component one home for its Sass, TypeScript and markup.
state: ready
waits-on: "nothing - the trial in Trying it passed; the rest is next. state and horizon written by an agent from his answers, strike them if wrong"
horizon: now
paths:
  - "packages/**"
  - "design/**"
  - "ruby/**"
  - "vipaq/packages/**"
  - "sites/www/**"
  - "sites/docs/**"
  - "sites/demo/**"
  - "api/src/Binacle.Net.UIModule/**"
---

# Packages and gems

**Leaning yes (the maintainer, 2026-11-07):** *"i feel the general direction you have it"*. The trial comes first:
*"iwant to do it before but also test it too"*. Everything below is a suggestion (agent, 2026-11-07) unless it quotes him.

His concern, 2026-11-07: *"there is no clear separation sometimes and... in the meantime there is separation when
it shouldnt"*.

## The problem

**Code is split by language.** The look is in `design/`, the behaviour in `packages/`, the Jekyll code in `ruby/`,
the markup in each site. That one habit causes both faults:

- **Split where it should be one.** The drawer's look is `design/components/_drawer.scss`, its behaviour
  `packages/binacle-components/src/drawer.ts`, its markup a copy in each site's `_includes`. The TypeScript builds
  markup with the Sass class names, such as `drawer__sites` and `code-block__line`, so the three are kept in step
  by hand.
- **One where it should be split.** A folder that means "the Ruby" takes any Ruby. So:
  - `doc_body` and `doc_sections`, which write the docs' own classes and phrases, sat in `ruby/jekyll-filters`, a
    generic gem every site loads, and brought Nokogiri to all four sites. Now moved to `binacle-docs`. The
    maintainer, 2026-11-06: *"i dont like that we pout site specific classes into ruby gems that are generic"*.
  - `binacle-net-ui` says it is *the UI module's implementation in TS*. It holds the apps, their Alpine helpers,
    the 3D view, which is a site component like the drawer, and *Everyday Items*, which only `demo` uses.

**Smaller faults found 2026-11-07:**

- **`cookies` is a real package on npm**, unrelated to ours. Ours resolves only because it is a workspace. Anyone
  reading `"cookies": "*"` takes it for the public one.
- **The names have no rule.** `binacle-*`, `binacle-net-*` and no prefix at all sit side by side.
- **The entry file moves around.** `theme-switcher`, `binacle-components` and `binacle-vipaq` export from a root
  `index.ts`; the two clients and `binacle-compact-notation` from `src/index.ts`.
- **`theme-switcher` has no README.**
- **The sites' npm names break the naming rule.** `binacle-net-www`, `binacle-net-docs` and `binacle-net-demo` are
  sites for the whole project, not the API.
- `initNavFade` in `binacle-components` is exported and tested, and no site calls it yet. It stays: the sites show
  a burger on a phone, but the UI module's header keeps the nav row that scrolls sideways, and its port calls it.
- The UI module's `package.json` lists `cookies`, `three` and `binacle-vipaq` and imports none of them; only
  `binacle-net-ui` does.

**What is right today and stays:**

- **The `jekyll-*` gems are generic.** Sitemap, page meta, structured data, GTM, breadcrumbs, the web manifest,
  resource tags, filters. Site plumbing, not components. They stay apart from the components.
- **`theme-switcher` is generic.** It holds no Binacle word: the storage key, the attribute and the button's class
  all come from options. The maintainer, 2026-11-07: *"themeswotcher is generioc and should cold generic componet
  used but the disign system"*. It stays its own package, MIT, and the design system uses it (*The brand package*).
- **The two API clients stay two.** They share a short response reader. They serve different APIs and different
  readers, and one is experimental. A shared package for that is the split this plan removes elsewhere.
- **The docs-only scripts stay in `sites/docs/_js`** - sections, the version select, the Swagger theme.
- **The gems' inside layout** already follows Bundler's: `<name>.gemspec`, `lib/<name>.rb`, `lib/<name>/`, `spec/`.

## The rule: by what it knows

**Each piece of code sits at one level. A level may use the levels below it, never one above.** Language does not
decide where code goes.

| Level | Knows | Holds |
|---|---|---|
| 1 - generic | nothing about Binacle | the `jekyll-*` gems, `theme-switcher` with its js-cookie |
| 2 - API and data | the API or the shared formats, not the look | the two clients, compact notation, ViPaq |
| 3 - brand | the look, not the API | tokens, fonts, the mark, every site component, the 3D view |
| 4 - apps and sites | anything | the apps, each site, the docs markup filter, the robots.txt body |

**How to place a new file:** ask what it imports and what words it holds. A gem that writes `doc__text` knows the
docs, so it is level 4 whatever language it is in. A generic part a site component needs, as the theme switcher
is, stays at level 1 and the component uses it.

## The names

**The maintainer, 2026-11-07:** *"binacle is binacle binacle-net is the api"*.

**Decided (the maintainer, 2026-11-07):** *"i hate net-client go with api-client but thats if we can rename to
@binacle if not the bust go with binacle--net-client and binacle-net-service-cleint"*.

- **npm: every workspace package takes the scope `@binacle/`.** Scoped names come from npm, and Angular, Babel and
  Vue use them: a scope marks a family of packages as one owner's and cannot clash with a public name. All stay
  `private`, so nothing is registered and the scope works today. Checked 2026-11-07: npm's search finds no package
  published under `@binacle`; whether anyone owns the empty scope could not be seen. That matters only if one is
  ever published.
- **The API client is `@binacle/api-client`.** If the scope cannot be used, both clients keep today's names,
  `binacle-net-client` and `binacle-net-service-client`.
- **Gems: `jekyll-*` means generic, `binacle-*` means ours.** The gems already follow it; `doc_body` is the one that
  breaks it. Jekyll's own plugins use the same prefix, such as `jekyll-sitemap`.
- **One scope, and a site's name says it is a site.** **Decided (the maintainer, 2026-11-07):** *"i wat
  @binacle/design-system  @binacle/sites-www @binacle/api-client"*. Asked about a second scope, `@binacle-net/`, he
  chose this. Earlier that day: *"the sites names go with www not -net-www"*. So `@binacle/sites-docs`,
  `@binacle/sites-demo` and `@binacle/sites-admin` follow (agent, from the same pattern). The UI module's name is open.
  Told the sites' names would then differ from their Cloudflare names: *"i think its unmpotrrant change isnt it? go
  with sites-www if it si"*. It is: only the `name` line and `package-lock.json` hold a site's npm name.
- **The Cloudflare names stay** (agent, 2026-11-07). `tooling/cloudflare/*.wrangler.jsonc` carries `binacle-net-www` and the rest as
  the Cloudflare project name. Renaming one there makes a new project to deploy to, not a rename. Only the npm
  `name` changes.
- **The name is the folder's path, with `packages/` left out**: `packages/api-client/` is `@binacle/api-client`,
  `sites/www/` is `@binacle/sites-www` (agent, from his three names). Nx named projects from their path the same way.

## The structure

```
packages/
  theme-switcher/   @binacle/theme-switcher    1   MIT; takes packages/cookies in
  compact-notation/ @binacle/compact-notation  2   was binacle-compact-notation
  api-client/       @binacle/api-client        2   was binacle-net-client
  service-client/   @binacle/service-client    2   was binacle-net-service-client, experimental
  design-system/    @binacle/design-system     3   was design/ and binacle-components
  apps/             @binacle/apps              4   was binacle-net-ui
vipaq/packages/binacle-vipaq/  @binacle/vipaq  2   the folder stays

ruby/
  jekyll-*                                     1   unchanged; jekyll-filters loses doc_body,
                                                   doc_sections and Nokogiri
  binacle-robots                               4   unchanged
  binacle-docs                                 4   was binacle-docs-versions
```

`design/` is gone. A site depends on `design-system`, and on `apps` if it has an app. It no longer depends on
`theme-switcher` itself.

## Inside every npm package

The same skeleton, so a reader who knows one knows all:

```
<package>/
  package.json     name, "private": true, "main": "index.ts", "sideEffects": false
  README.md        what it is, who uses it
  LICENSE          only where the licence differs from the repository's
  index.ts         the public surface: re-exports only, no code
  src/             the code, one file per part
  tests/           mirrors src/, jest on jsdom
  tsconfig.json
  jest.config.js
```

- **`index.ts` at the root, never `src/index.ts`.** Most packages here do it already; the two clients and compact
  notation move theirs.
- **File names stay camelCase** (`copyButton.ts`), as every TypeScript file here is. Folder names that are a BEM
  block stay kebab-case (`copy-button/`), because the block's Sass name is.
- **Tests stay in `tests/`**, as every package here has them. Choose tests beside the code, `drawer.test.ts`, only
  if one folder must hold the whole component; Angular and Vite templates do that. It would change every package.

### `theme-switcher`

```
packages/theme-switcher/
  package.json  LICENSE (MIT)  README.md (new)  index.ts
  src/
    options.ts  storage.ts  theme.ts  themeInit.ts  themeSwitcher.ts
    vendor/js-cookie/     was packages/cookies, with its own licence note
  tests/
    vendor/js-cookie/
```

- **It stays generic.** Nothing in it may name a Binacle class, attribute value or site. A new option is how a site
  asks for something different.
- **js-cookie moves in.** It is its only reader, and both are MIT, so the licence does not change. `NOTICE` names
  `packages/cookies` and its upstream version; it moves with it. The UI module's stray `cookies` dependency goes.
  It comes back out as `@binacle/cookies` only if another package ever needs it.

### The brand package - `design-system`

**What goes in:** all of `design/`, all of `binacle-components`, and the framework-free part of the 3D view.

```
packages/design-system/
  package.json         the exports map; depends on @binacle/theme-switcher and three
  binacle-design-system.gemspec    the Jekyll theme gem: ships _includes/ only
  README.md
  index.scss           @forward every part, for a surface that wants all
  index.ts             re-exports every init function
  abstracts/           Sass helpers that write no CSS: grounds, lines, overlays, tiers
  base/                tokens, bare elements
  layout/              one folder per layout block
    site-header/  _index.scss
    container/    _index.scss
  components/          one folder per component block
    drawer/        _index.scss  drawer.ts
    copy-button/   _index.scss  copyButton.ts
    button/        _index.scss                      no .ts when there is no behaviour
    theme-button/  _index.scss  themeButton.ts  themeInit.ts
    pack-view/     _index.scss  packView.ts  colors.ts  palette.ts  cameraFit.ts ...
  utilities/
  fonts/  mark/
  _includes/components/    one .html per component that has shared markup
  tests/                   mirrors the folders
```

- **The groups are `design/`'s own** - abstracts, base, layout, components, utilities. They follow the 7-1 pattern
  from Hugo Giraudel's Sass Guidelines, the most used layout for a Sass project. They stay.
- **One folder per BEM block**, named after the block. A block's Sass and its TypeScript sit side by side; its
  shared markup is in `_includes/components/`, because a Jekyll theme gem reads includes from there only.
- **The theme button uses `theme-switcher`.** `themeButton.ts` defines `<theme-switcher>` with the design's button
  class and the site's options; `_index.scss` styles it. `themeInit.ts` is the pre-paint script. It gets its own
  path in the exports map, `@binacle/design-system/theme-init`, so its bundle holds only it and the switcher, not
  the whole package. Its size is checked against today's.
- **Licence: AGPL-3.0-only**, as `binacle-components` is. Using the MIT switcher from it is allowed.
- **Sass is reached by the `pkg:` importer**, from the Sass team, in Dart Sass since 1.71. A site writes
  `@use 'pkg:@binacle/design-system/components/drawer'` and Sass finds it through `node_modules`, as Node finds a
  package. This replaces `--load-path=../../design` in each site's `build:css` and `watch:css` scripts; they gain
  `--pkg-importer=node`.
- **The `exports` field** in `package.json`, from Node, maps each name: a `"sass"` condition to the `.scss`,
  `"default"` to the `.ts`. One name serves both.
- **Markup on the three Jekyll sites comes from the theme gem** - Jekyll's own way to share `_includes`. A site
  names it in `_config.yml` as `theme:`, and loads it from a path in its `Gemfile`. A file of the same name in the
  site wins, so a site can still override one include. One theme per site, so all shared markup is in this gem.
- **Markup in the UI module is written again**, as Razor partials. Razor cannot read Liquid.
- **Fonts are still copied to each site.** A `url()` in built CSS resolves against the page's site, not the
  package. `gulpfile.js` copies `design/fonts/*` today; it reads the new path.
- **The 3D view splits in two.** `packView.ts` and its pure parts - colours, palette, camera fit, prints,
  positions - import only three and the shared coordinate and dimension types, so they are level 3 and move here,
  with `shared/coordinates.ts` and `shared/dimensions.ts`. `packViewComponent.ts` is the Alpine wrapper; it stays
  with the apps. `docs` reads the view's Sass and not its TypeScript, which already works this way.
- **The tabs.** `tabs.ts` does not follow the ARIA tabs pattern from the W3C: no arrow keys, no `aria-controls`. It
  moves as it is; fixing it is its own work.

### The apps package - `apps`

```
packages/apps/
  package.json  README.md
  index.ts                 the three plugins
  src/
    packingDemo/           Custom Sizes, with its plugin.ts
    protocolDecoder/       the ViPaq Decoder, with its plugin.ts
    everydayItems/         Everyday Items, with its plugin.ts
    shared/
      errorsDialog/  field/  logger/
      packViewComponent.ts  defineComponent.ts  dictionary.ts
  tools/generateSamples.ts
  tests/
```

- **Each plugin moves into its app's folder.** Today they sit loose in `src/`.
- **The app folders keep their names.** `packingDemo` is the page now called *Custom Sizes*; renaming the folder
  changes no URL and no `x-data` name, so it is not worth it on its own.
- **Everyday Items stays here** though only `demo` uses it. It shares the helpers and the view wrapper, and the
  sites have no test setup. Choose to move it into `sites/demo/` only if `demo` gets one.
- **The errors dialog imports `@binacle/api-client`**, so it is level 4 and cannot go to the brand package.
- **`"sideEffects": false`** is missing today. Without it webpack may keep code a host does not call.
- **The built bundle's name is a URL.** The image serves `_content/Binacle.Net.UIModule/js/binacle-net-ui.js`, and
  `tooling/smoke/full.hurl` and `quickstart.hurl` check it. A rename of the package does not have to rename the
  bundle; rename both or neither.
- **The Alpine `x-data` names do not change** - `pack_view`, `errors_dialog` and the app names.
  `api/test/Binacle.Net.UIModule.IntegrationTests/Tests/PageContentTests.cs` checks them.

### The clients and the formats

```
packages/api-client/
  package.json  README.md  index.ts
  src/      client.ts  response.ts  types.ts
  spec/     the OpenAPI document the types are checked against
  tests/
```

`service-client` takes the same shape, with its own `spec/`. `compact-notation` takes it without `spec/`. `binacle-vipaq` keeps its own, with
`tools/`. `tooling/openapi.just` names the client's `spec/` folder by path.

### The gems

Every gem keeps Bundler's layout, the one `bundle gem` writes:

```
ruby/<gem>/
  <gem>.gemspec  README.md  LICENSE
  lib/<gem>.rb        requires the rest, nothing else
  lib/<gem>/          one file per part
  spec/               mirrors lib/
```

**Done (agent, 2026-11-07): `binacle-docs-versions` is `binacle-docs`**, everything Ruby that only `docs` uses.
`docs` builds the same HTML as before. He ran `just test sites`: it passes (the maintainer, 2026-11-07).

```
ruby/binacle-docs/
  binacle-docs.gemspec      depends on Nokogiri
  lib/binacle-docs.rb
  lib/binacle-docs/
    versions/          today's files: generator, pages, urls, versioned_file, vlink_tag, error
    pager.rb           Binacle::Docs::Pager
    markup/            doc_body.rb, sections.rb (was headings.rb)
  spec/                versions/, markup/, pager_spec.rb; fixtures/ stays at the top
```

- **The module becomes `Binacle::Docs`**, with `Binacle::Docs::Versions` and `Binacle::Docs::Markup`. The RubyGems
  naming guide: a dash in a gem name is a nested module. Today's `Binacle::DocsVersions` breaks that.
- **`jekyll-filters` goes back to generic** - plain text, title case, dates. Its gemspec drops Nokogiri; its README
  drops the two filters.
- **The class that rewrites a page is `Converter`**, inside `Binacle::Docs::Markup::DocBody`. It was `Markup`, which
  now names the module above it.
- **`doc_body`'s class list must match the design system's Sass.** That is level 4 using level 3 names, which the
  rule allows. A theme gem cannot hold plugins - Jekyll loads only plugin gems a theme names as dependencies - so
  the filter cannot live inside the design system's gem.
- **Not chosen: the site's `_plugins/` folder.** It is Jekyll's convention for one site's own code, but it has no
  specs or coverage, which every gem here has.

## What a move touches

The old names and paths are written in these places, beside each package's own files and the imports:

- the root `package.json` workspaces, and `package-lock.json`;
- each site's and the UI module's `package.json` dependencies and `build:css` / `watch:css`;
- `jest.config.js` roots;
- the test recipe names, `ts_<package>_unit` and `rb_<gem>_unit`, and the workflows that call them:
  `.github/workflows/shared-image-tests.yml`, `shared-site-tests.yml`;
- `tooling/regen.just` (the demo sample set), `tooling/openapi.just` (the client spec);
- `gulpfile.js` (the fonts, and the skip lists);
- `NOTICE`;
- `sites/docs/Gemfile` for the docs gem, and each site's `_config.yml` for the theme gem;
- each site's `_js/theme-init.ts` and main entry, which import `theme-switcher` today;
- comments that name a package path: `_Header.cshtml`, `sites/www/_js/main.ts`, `PageContentTests.cs`;
- the READMEs under `packages/` and `ruby/`.

## Ordering facts

- **The `pkg:` trial comes before the merge.** If the importer, the exports map or the theme gem does not work
  here, the brand package has no way in, and `design/` stays.
- **The gem move and the renames do not depend on the trial.** Each can land on its own.
- **The UI module's port builds on whichever layout exists.** Done after it, the UI module's new Sass and
  TypeScript move as well.

## His answers

1. **The brand package's name.** **Decided (the maintainer, 2026-11-07):** `design-system`, picked from
   `design-system`, `components`, `ui` and `theme`.
2. **Where the brand package lives.** **Decided (the maintainer, 2026-11-07):** in `packages/`, *"packages...
   since it vboroows from packages"*. He had asked whether it could leave `packages/` as a theme.
3. **The apps package's name.** **Decided (the maintainer, 2026-11-07):** `apps`, picked from `apps`, `ui` and
   `widgets`.
4. **The service client's name.** **Decided (the maintainer, 2026-11-07):** `@binacle/service-client`, picked
   from `service-client` and `admin-client`. Without the scope it stays `binacle-net-service-client`.
5. **The docs gem's name.** **Decided (the maintainer, 2026-11-07):** `binacle-docs`, module `Binacle::Docs`.
   His rule: *"the idea is binacle-net is the api... while the repo is called binacle.net it has binacle lib binacle
   vipaq binacle compact notation"*. The docs cover more than the API, so they are `binacle`. Asked to confirm:
   *"ok"*.

## What could stop it

- Webpack keeps three.js in a site that never imports the 3D view while the bundles are CommonJS (*What the
  trial found*). Then the 3D view stays in the apps package.
- The pre-paint script may grow once it is bundled from a bigger package, even through its own export path.

## Trying it

**One component, one surface, all three parts.** The copy button has Sass, TypeScript and markup. Move it into the
design system and have `www` take it by `pkg:`, by import and by include. If a part fails, that part stays where
it is and this plan says why.

### What the trial found (agent, 2026-11-07)

All three parts work. `packages/design-system/` holds the copy button; `www` takes it. Built before and after:
`www`'s CSS is byte for byte the same, its HTML the same but for the sitemap's build time. `docs` and `demo` build
and their CSS is unchanged. The jest tests pass. He ran `just test sites` and looked at `www` served: *"ok"* (the maintainer, 2026-11-07).

- **`pkg:` and the `sass` condition resolve through the workspace link.** Sass 1.103 is installed.
- **The theme gem works from a path.** `www`'s `Gemfile` names it, `_config.yml` sets `theme:`. No site calls
  `include_cached`, so `jekyll-include-cache` has nothing to clash with.
- **An include's final newline lands in the page.** Inline in a `div`, it moved `</div>` to its own line. The
  include ends on a `{%- comment -%}` to eat it.
- **A subpath import does not type-check on the sites.** Their `tsconfig.json` uses the old module resolution,
  which ignores `exports`. `www` imports from the package root. With `moduleResolution: bundler` the subpath
  passes. The pre-paint script's own path, `@binacle/design-system/theme-init`, needs that setting in its
  webpack override.
- **Nothing is tree-shaken today.** `www`'s bundle already carries tabs, lightbox, sticky sidebar and nav fade,
  and calls none of them. The TypeScript compiles to CommonJS, so `"sideEffects": false` drops nothing. The
  3D view in the design system's `index.ts` would put three.js in every site, unless the main bundles compile
  to ES modules as the pre-paint script already does.
- **The jest name is the folder, `design-system`, not the npm name.** `tooling/tests/jest.sh` names report files
  after it, and a `/` makes a folder. The recipe is `ts_design-system_unit`.
- **Coverage looked in `src/` only.** The design system's code sits in its Sass groups, so the root
  `jest.config.js` now also collects `components/**`. Each group that gains a `.ts` needs its own line.
- **Two bridges stay until `docs` and `demo` move:** `design/components/_copy-button.scss` forwards to the
  package, and `binacle-components` re-exports `initCopyButton` from it.

## Done when

- [x] No `jekyll-*` gem names a class, a phrase or a path from one site. Check: `git grep -nE
      'doc__|code-block|callout|http-line' ruby/jekyll-*` finds nothing.
- [x] `jekyll-filters` has no Nokogiri: `grep -n nokogiri ruby/jekyll-filters/*.gemspec` finds nothing.
- [ ] Every workspace package's name starts `@binacle/`, or, without the scope, the clients keep their names.
      Check: the `name` of each workspace `package.json`.
- [ ] Every npm package has the skeleton in *Inside every npm package*: a root `index.ts`, a README.
- [ ] `theme-switcher` names no Binacle class, value or site: `git grep -niE 'binacle|__' packages/theme-switcher/src`
      finds nothing.
- [ ] No site lists `theme-switcher` as its own dependency; the design system does.
- [ ] No folder named `design/` at the root, and no `--load-path=../../design` in any `package.json`.
- [ ] Each component's Sass and TypeScript sit in one folder of the design system, and its shared include in its
      `_includes/components/`.
- [ ] A site that does not import the 3D view has no three.js in its bundle.
- [ ] The pre-paint script's bundle is no bigger than today's.
- [ ] The image still serves the UI module's bundle at the URL the smoke tests check.
- [ ] Every test recipe and workflow line names a package that exists.
- [ ] The READMEs under `packages/` and `ruby/` list what is there.
