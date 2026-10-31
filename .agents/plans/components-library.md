---
description: One package that holds each site component whole - its Sass, its TypeScript and its markup - and each surface builds only the components it uses.
state: deferred
waits-on: "the maintainer picking it up - he chose to try it later, 2026-10-31. horizon: undecided - written by an agent, strike it if wrong"
horizon: undecided
paths:
  - "packages/**"
  - "sites/www/**"
  - "sites/docs/**"
  - "sites/demo/**"
  - "api/src/Binacle.Net.UIModule/**"
  - "ruby/**"
---

# A components library

**The idea.** One workspace package, `binacle-components`, with a folder per component. A folder holds what
the component looks like, how it behaves and its markup. Each surface takes only the components it uses.

The maintainer, 2026-10-31: *"best scenario this turns into a components lib and each site build their own"*,
and on trying it: *"i dont know if any will work / I want to try it"*. Deferred, 2026-10-31: *"defer the shared
for later"*. It revives when he picks it up.

**It is not known to work here.** Nothing below has been tried in this repository.

## What a folder would hold

```
packages/binacle-components/
  package.json                 the Sass and TypeScript side
  binacle-components.gemspec   the Jekyll side
  index.scss                   every component
  index.ts                     every init function
  _includes/components/
    header.html
    drawer.html
  src/
    tokens/_index.scss
    drawer/_index.scss
    drawer/drawer.ts
  tests/
  fonts/
```

A component with no behaviour, such as the header or a button, has no `.ts`.

## How each part reaches a surface

**Sass.** Dart Sass has a package importer since 1.71; the repository is on 1.93. With `--pkg-importer=node`
on the `sass` command, `@use 'pkg:binacle-components/drawer'` is found through `node_modules`, the way Node
finds a package. npm workspaces link the package there, so nothing is published.

```scss
@use 'pkg:binacle-components/tokens';
@use 'pkg:binacle-components/header';
@use 'site';
```

**The map** is the `exports` field in the package's `package.json`. A `"sass"` condition tells Sass which
`.scss` a name means; `"default"` tells webpack which `.ts`. One name can serve both.

**TypeScript.** As `theme-switcher` does now. A surface imports the init functions it needs, and webpack
bundles only those.

```ts
import {initDrawer, initCopyButtons} from 'binacle-components';
```

**Markup on the three Jekyll sites - a theme gem.** A Jekyll theme is a gem that ships `_includes/`. A site
names it in `_config.yml`, and Jekyll reads its includes as the site's own. A file of the same name in the
site wins, so a site can override one component. The sites already load the gems under `ruby/` as local path
gems, so this is one more.

```ruby
gem "binacle-components", path: "../../packages/binacle-components"
```

```yaml
theme: binacle-components
```

```liquid
{% include components/code-block.html lang="bash" code=page.run_command %}
```

Inside the include, `{{ include.lang }}` reads what was passed.

**Markup in the UI module - written again.** It is Razor and cannot read Liquid. Its parts are Razor partials,
or later a Razor Class Library. The screenshot compare against the mockup keeps them in step with the Liquid
ones.

## Known before trying

- **Breakpoints written once.** Media queries cannot read custom properties. Sass variables in `tokens` -
  `$tier-tablet: 640px` - can be read by every component's media queries.
- **Fonts are still copied.** A `url()` in the built CSS resolves against the page's site, not the package,
  so each surface serves the font files itself.
- **Markup is not built from JavaScript.** Custom elements would reach all four surfaces, but the header,
  footer and code blocks would be missing until the script runs - less for a crawler and for a reader with
  scripts off. They suit small interactive parts, as `theme-switcher` already is.
- **Only one Jekyll theme per site.** A second shared set of includes would have to join this gem.

## What could stop it

- The `pkg:` importer and the `exports` conditions may not resolve the way the docs say through npm
  workspace links.
- A Jekyll theme gem from a local path may not find its includes, or may clash with `jekyll-include-cache`.
- Webpack may not drop unused init functions unless the package is ES modules and has no side effects.

## Trying it

**One component, one surface.** The copy button has all three parts - Sass, TypeScript and markup. Move it into
the package and have `www` take it by `pkg:`, by import and by include. If any of the three fails, that part
stays where it is.
