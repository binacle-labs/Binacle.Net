---
id: commands
description: How to set up a clone, run the API and the three sites, run tests and benchmarks, and build the Docker image
verified: 2026-08-29
check: Tests match tooling/tests.just; coverage recipes match tooling/coverage.just; openapi recipes match tooling/openapi.just; agents recipes match tooling/agents.just; regen recipes match tooling/regen.just; serve recipes match tooling/serve.just; smoke recipes match tooling/smoke.just; build recipes match tooling/build.just; check recipes match tooling/check.just; ci recipes match tooling/ci.just and each names an existing tooling/ci/*.sh; install/assets match the root justfile; aliases and scripts match tooling/*.sh; compose service list matches tooling/serve.services.yml; the Prerequisites section still only points at DEVELOPMENT.md and repeats no versions or install commands
paths:
  - "justfile"
  - "tooling/**"
---

# Commands

Setup, running things, tests, coverage, the OpenAPI documents, the image build, the smoke suite, the agent
indexes and the committed generated data are `just` recipes; only the benchmarks and the performance runs are
still scripts in `tooling/`. All are run
from the repo root. `just` with no arguments lists everything. For the `tooling/` directory anatomy (scripts,
local compose, env, emulator state) see `$tooling`.

## Prerequisites

**`DEVELOPMENT.md` at the repo root is the single source for this** — the tools, their versions, their pin
files, and the commands that install each one, docker and the two smoke binaries included. It is written for a
human setting up a machine.

Do not repeat any of it here, and do not answer a setup question from memory: read that file, or point the user
at it. This section exists only to say where it is.

The short version, for judging whether a command in this doc can run at all: .NET SDK 10.x, Node 22 (`.nvmrc`),
Ruby 3.4.7 (`.ruby-version` in `sites/docs/`, `sites/demo/` and `sites/www/` — **all three** sites need it),
`just`, and docker for anything touching the image.

## Set up a fresh clone

```bash
just install                           # npm workspaces, all three jekyll sites' gems, then the asset copy
just assets                            # only the asset copy - after changing anything under assets/
```

`assets` copies `assets/**` into the three sites and into the UI module's `wwwroot/` via gulp. Each serves its
own copy, so a changed logo does not show up until this runs. **`sites/www` gets a smaller set** — the gulp
target skips `assets/lib/`, because that site runs no CSS framework; see `$sites/www`.

## Run from source

```bash
just serve api [N|S|U|All]             # the API
just serve docs                        # docs site: jekyll serve + webpack watch, one terminal
just serve demo                        # demo site: same
just serve www                         # marketing site: jekyll serve + sass and webpack watches
just serve services-up [-d]            # what the API talks to: aspire-dashboard, azurite, postgres
just serve services-down [-v]          # only needed after -d; Ctrl-C is enough otherwise
```

`services-up` runs **no** binacle-net — it is what `just serve api` talks to, and what the Postgres and
AzureStorage tests need. Running the *built image* is a different job; see "Run the image" below.

The API launch profiles:

- `N` / `Normal` — core API only (default)
- `S` / `WithServiceModuleOnly` — with ServiceModule (auth, rate limiting)
- `U` / `WithUiModuleOnly` — with UIModule
- `All` / `WithAllModules` — everything

`docs` and `demo` run both halves under `concurrently --kill-others`, so one Ctrl-C stops the pair. `www` runs
three, because its `npm run watch` is itself a sass and webpack pair. All of them need `just install` to have
run first.

## Run Tests

Tests are `just` recipes, not scripts — one recipe per suite, defined in `tooling/tests.just` and
loaded as the `test` module. The same recipes are what CI calls, so a red step is the line to paste here.

**A test name is derived, never chosen:** `<lang>_<project>_<kind>`. The project segment is the assembly,
package or gem name lowercased with dots turned to dashes, so `cs_binacle-net-kernel_unit` is
`Binacle.Net.Kernel.UnitTests` and nothing else. `_` separates the segments, `-` belongs inside a name.

**The tests are `[private]`**, so completion offers the three groups and not twenty-six tests. A private
recipe still runs by name. `just test` with no argument prints the whole list.

```bash
just test all       # every test that needs nothing brought up
just test image     # the seventeen the Docker image ships
just test sites     # the fifteen a Jekyll site ships
just test           # the three above, then every test name

# C#
just test cs_binacle-lib_unit
just test cs_binacle-compact-notation_unit
just test cs_binacle-flux-results_unit
just test cs_binacle-vipaq_unit
just test cs_binacle-net_unit
just test cs_binacle-net-kernel_unit
just test cs_binacle-net-diagnostics-module_unit
just test cs_binacle-net-ui-module_unit
just test cs_binacle-net-service-module_unit
just test cs_binacle-net_integration
just test cs_binacle-net-ui-module_integration
just test cs_binacle-net-service-module_integration [Sqlite|Postgres|AzureStorage]  # no arg falls back to SQLite

# TypeScript
just test ts_binacle-compact-notation_unit
just test ts_binacle-vipaq_unit
just test ts_binacle-net-ui_unit
just test ts_cookies_unit
just test ts_theme-switcher_unit

# Ruby - the ten gems. `just test sites` and `just test all` run them.
just test rb_binacle-docs-versions_unit
just test rb_binacle-robots_unit
just test rb_jekyll-breadcrumb-trail_unit
just test rb_jekyll-filters_unit
just test rb_jekyll-gtm_unit
just test rb_jekyll-multi-sitemap_unit
just test rb_jekyll-page-meta_unit
just test rb_jekyll-resource-tags_unit
just test rb_jekyll-structured-data_unit
just test rb_jekyll-webmanifest_unit
```

**Twenty-six tests, and `just test all` runs every one.** The ten Ruby ones go through `bundle exec rspec`
from the gem's own folder, which is the only place a `spec_helper` is on the load path.

**Three group recipes, and the two lists in `tooling/tests.just` are the only copy of the set of tests.**
`just test image` is what the Docker image ships, `just test sites` is what a Jekyll site ships, and
`just test all` is the two of them with nothing run twice — five javascript tests are in both. Each group
runs every test and reports all the failures, not just the first.

**A group is for a laptop. CI never calls one** — a workflow names every test as its own step, so a red check
names the suite. The step's `name:` is the assembly, package or gem in full; its `run:` is the derived recipe
name. The lists are written out by hand and nothing checks one against another.

`DOTNET_TEST_ARGS` is appended to every `dotnet test` recipe, which is how CI runs them all against one Release
build: `DOTNET_TEST_ARGS="--configuration Release --no-build" just test all`.

## Coverage

Coverage is not a second run — the collector rides along inside the test run, so these are the same tests
`just test all` runs, asked for extra output. Needs nothing brought up: the ServiceModule test uses SQLite.

```bash
just coverage all                      # every suite + the table (cobertura)
just coverage all sonar                # the formats Sonar imports
just coverage report                   # merge the last run into artifacts/coverage/html-report/index.html
just coverage table                    # re-print the table without re-running
```

The format names the consumer, not the file format — `cobertura` is what the table and the HTML report read,
`sonar` is Visual Studio xml for C#, lcov for TS and simplecov json for Ruby. Output is one flat file per
suite, named after the project, package or gem:

| Path | Holds |
|---|---|
| `artifacts/tests/<suite>.ctrf.json` | test results (jest writes `<package>.jest.json`, rspec `<gem>.rspec.json`) |
| `artifacts/tests/expected.txt` | every suite that started, one name per line |
| `artifacts/coverage/cobertura/<suite>.xml` | coverage for the table and the HTML report |
| `artifacts/coverage/sonar/<suite>.xml` | C# coverage for Sonar; TS is `<package>.info`, Ruby `<gem>.json` |
| `artifacts/coverage/html-report/` | the merged report, written by `just coverage report` |

The table prints a row per suite (`Passed`/`Failed`/`Skipped`/`Coverage`) and its exit code is the run's verdict.

**A suite that started and wrote nothing gets a `no report` row and fails the run.** Every test writes its
name into `expected.txt` before it runs, which is the only thing separating "the suite produced nothing" from
"there is no such suite" — without it a missing report is simply absent from the table and the run publishes.

## OpenAPI documents

```bash
just openapi generate                  # artifacts/openapi/Binacle.Net_v3.json + _v4.json
just openapi generate <dir>            # write them somewhere else (pass an absolute path)
just openapi lint [<dir>]              # generate, then lint with Spectral against tooling/openapi.spectral.yaml
```

Nothing needs to be brought up — the documents come out of the build, not out of a running server:
`Microsoft.Extensions.ApiDescription.Server` starts the app host itself and dumps every registered
`IOpenApiDocument` (`$api/openapi`). The host it starts has no launch profile, so **ServiceModule is off** and
the documents carry no `/api/auth/token` path — the shape the committed specs assume.

Generation is off by default (`-p:GenerateOpenApi=true`, set by the recipe) so an ordinary build doesn't start
the app host. The destination is `-p:OpenApiDir`; MSBuild resolves a relative one against the **project**
directory, which is why the recipe passes an absolute path.

`just openapi lint` is clean — no errors and no warnings. Both documents carry a `servers` entry with a single
relative `/`, set in the shared document transform, so a run that reports `oas3-api-servers` means something
removed it.

## Performance tests

Per slice; write reports to a gitignored scratch folder — see [results/README.md](../../results/README.md) for the
scratch-vs-curated convention:

```bash
./tooling/performance.lib.sh
./tooling/performance.vipaq.sh
```

## Benchmarks

Per slice; BenchmarkDotNet, markdown-only, output pinned next to the project:

```bash
./tooling/benchmarks.lib.sh [FastValidation|AlgorithmRacing|BischoffSuite|Parallelization|ResultSelection]
./tooling/benchmarks.vipaq.sh [Encode|Decode]
# No argument = all
```

## Run the image

Build it first with `just build image`, then:

```bash
just image up                          # same as `up full`
just image up full                     # all modules, all three backends + dashboard
just image up volume                   # the image alone, SQLite, data in a named volume
just image up bind                     # the image alone, SQLite, data in a folder you can open
just image down [name] [-v]            # -v drops the named volumes, postgres included
```

Extra arguments go straight through to `docker compose`. The name is positional, so pass it whenever you pass
a flag — `just image up -d` reads `-d` as the stack name and is rejected.

All three check for `binacle-net:local` and tell you to build it if it is missing. `up` also creates the
bind-mounted folders and opens their permissions, which docker will not do for you. See `$tooling` for which
stack needs which folder.

The backing services for an API run from source are a different thing — that is `just serve services-up`.

## Verify a published image

```bash
just image verify 3.0.0                 # all four checks
just image verify 3.0.0 signature       # one: tags, signature, attestations or metadata
just image verify 3.0.0 all refs/heads/main binacle/binacle-net   # the ref and the repo, both defaulted above
```

Reads Docker Hub, builds nothing, **never logs in**. The version is required and never defaults — a default
rots into a tag nobody meant to check. All four checks run even when one fails, so a failure comes with the
three answers that explain it; exit 1 if any failed.

Only `signature` needs `cosign` (`DEVELOPMENT.md`, pinned) and it fails with that pointer rather than passing
quietly. See `$tooling` for what each check proves.

## Render the Docker Hub page

```bash
just image dockerhub-overview 3.0.0               # prints the page as it would be published
just image dockerhub-overview 3.0.0 > page.md     # keep it
```

Fills the two placeholders in `.github/dockerhub-overview.md` and prints the result. Writes nothing and needs
no credential, so it is the way to read the exact page before a tag goes out — the release workflow runs this
same recipe and pipes it straight to Docker Hub.

Rejects a prerelease and a partial version, and fails if any placeholder survives. A page naming a tag that
does not exist is the one failure a reader sees first.

## Smoke the image

Tests the image rather than the code: what it contains, and what its HTTP surface does with the modules
switched on and off. Needs `container-structure-test` and `hurl` (see `DEVELOPMENT.md`) plus docker.

```bash
just smoke all                         # build binacle-net:local, check its structure, then every profile
just smoke test-structure [image]      # static content only — reads the image, no container, no stack
just smoke test <profile> [image]      # one profile end to end: up -> hurl -> down
just smoke up <profile> [image]        # bring one up and leave it   [minimal|quickstart|prod|service|full]
just smoke down <profile> [-v]         # stop it
```

Every recipe takes the image last and defaults to `binacle-net:local`, so the same suite runs against a local
build or a published tag — `just smoke all binacle/binacle-net:<tag>`. Given anything but the local tag,
`all` pulls instead of building. The stacks read it as `$BINACLE_IMAGE` with the same default.

The static check reads the image, not a container, so `all` runs it **once** rather than once per profile. The
five profiles are declared in one place, the `profiles` variable at the top of `tooling/smoke.just`.

While editing a `.hurl`, skip the up/down cycle: `just smoke up prod`, then `just smoke::_test_profile prod` as
many times as needed, then `just smoke down prod -v`.

## Regenerate the agent indexes

```bash
just agents all                        # all five
just agents generate-index plans       # one [rules|docs|design|plans|memory]
```

Rewrites the `_index.md` manifest for `.agents/rules`, `.agents/docs`, `.agents/design`, `.agents/plans` and
`.agents/memory` (grouped by area). Each entry's description comes from the file's `description:` frontmatter,
falling back to its first heading. Run it after adding, renaming, or re-describing any
`.agents/{rules,docs,design,plans,memory}/*.md` file. A name that isn't one of the five is rejected, so a typo
can't leave an index untouched and look like it worked.

## Regenerate committed data

```bash
just regen all                         # every generator, in dependency order
just regen or-lib-scenarios            # OR-Library text -> shared/data/bischoff-suite
just regen vipaq-packed-data           # those + custom-problems, packed -> vipaq/data/packed
just regen vipaq-interop-vectors       # the interop pair + header bytes -> vipaq/test-vectors
just regen demo-samples                # shared/data/demo-samples -> the demo's sample set
just regen check                       # regenerate, then fail if any of it changed
```

Their output is committed. None takes an argument: each runs every generator in its list, so it cannot
half-run and leave the data inconsistent.

`or-lib-scenarios` writes what `vipaq-packed-data` reads, which is why `all` exists — the ordering is the part
that is easy to get wrong by hand. `vipaq-interop-vectors` is one recipe because the C# and TS halves write
`interop/cs` and `interop/ts` from the same `input.json`, and regenerating one alone is the drift the interop
integrity tests catch.

Every run is deterministic, so `check` runs everything and then asks whether the tree moved; it diffs only the
`.json` the generators write, because those folders also hold their own README and `vipaq/test-vectors` holds
hand-authored vectors. **No workflow calls `check`** — it is for the maintainer who edited a tool or a source
problem.

## Build

```bash
just build publish                     # asset copy + UI bundles, then dotnet publish -> artifacts/binacle-net
just build image [version]             # publish, then docker build -t binacle-net:<version> (default local)
just build docs                        # the documentation site -> artifacts/docs
just build demo                        # the demo site -> artifacts/demo
just build www                         # the marketing site -> artifacts/www
```

`publish` runs the asset copy and the UI module's own `npm run build` before `dotnet publish`, because dotnet
collects static web assets at publish time and nothing fails when `wwwroot/` is empty — the image just ships
pages with no styling and demos that do nothing. `image` always re-publishes first — `docker build` copies
whatever is in `artifacts/binacle-net`, so skipping the publish is how a stale image gets tagged. The version
becomes both the image tag and `BINACLE_VERSION` inside the container, which is what the running app reports.

Then run it with `just image up`, which prepares the bind-mounted folders first.

`docs`, `demo` and `www` are the build half of `just serve <site>` — same site, built once instead of
served and watched. **The deploy workflows call these and hand `artifacts/<site>` straight to the host**, so
what they build is what gets served. Three steps in a fixed order: copy the assets, run webpack over `_js/`, then
`jekyll build` with `_config.yml,_config.prod.yml`. **Skipping any of them still produces a site**, just one
with no scripts and no logo, because `js/`, `lib/` and `media/` are gitignored and filled by the first two
steps. The prod config overrides the three values that differ off localhost: the site url, the api url and the
analytics container.

## Checks

```bash
just check links                       # internal links in all three built sites
just check links docs                  # one of them
just check links-external docs         # every link, other people's servers included
just check workflows                   # actionlint over .github/workflows
just check actions                     # the .github/actions manifests
just check scripts                     # shellcheck over tooling/ci/*.sh
```

All three print the files they were handed and end with a count, because actionlint, grep and shellcheck are
silent when they pass and silence alone does not say the run happened. **`workflows`** catches what a workflow file cannot be
tested for without running it: a bad expression, an undefined `needs`, an invalid runner label, an unquoted
shell variable. **It needs shellcheck as well as
actionlint** — actionlint hands every `run:` block to it when it can find one, and the runners already have it,
so a laptop without it checks less than CI does.

**`actions` is separate because actionlint cannot read a composite action** — hand it an `action.yml` and it
reports `"jobs" section is missing`. What it checks by hand is the one failure that has already happened: a
`vars` or `secrets` expression anywhere in a manifest, `description:` fields included, fails the action *load*
on the runner rather than resolving to empty.

**`scripts` is the one that makes `tooling/ci/` worth having.** Those files exist so the shell a workflow runs
can be run and checked on its own; without this recipe nothing checked them. shellcheck ships on the runners, so
the pull request installs nothing for it.

Needs `lychee` (pinned, installed from `DEVELOPMENT.md` like the smoke tools) and needs the site built first —
the recipe stops with `No artifacts/docs` rather than checking nothing and passing. It reads the built output
because a source `href` is still Liquid at that point.

`links` passes `--offline`, so it checks only the links that resolve inside the site — the ones a renamed page
breaks. Both sites together answer in about a fifth of a second, which is why the deploy workflows run it as a
pre-flight. `links-external` makes a real request per unique URL, takes ten seconds, and can fail on somebody
else's outage; that is why it is a separate recipe rather than a flag, and why it is not a gate.

`tooling/check.lychee.toml`, named with `--config`, holds the URLs it must never check — the `localhost` samples in
the quickstart pages, which are correct on the page and unreachable from anywhere else.

## CI scripts

```bash
just --list ci                              # every operation, with its arguments
just ci changed-paths HEAD~1 HEAD           # code=yes|no and site=yes|no
just ci gate '<json of every job and its result>'
just ci deploy-summary <commit> <tag> <url>
just ci moving-tags <immutable tag> '<one tag per line>'   # prints moving=<space separated>
just ci copy-tags <source@digest> '<tags>'                 # copy, then read every tag back
```

The shell a GitHub Actions workflow runs. `ci.just` is a door with two lines per recipe; the code is one file
per operation in `tooling/ci/`, because shellcheck cannot read a `.just` body or a `run:` block and neither
can be run on its own.

**Every script takes its inputs as arguments and reads no `github.*` context**, so all of them run here as
well as on a runner. The ones that print `key=value` are teed into `$GITHUB_OUTPUT` by the step that calls
them, and send their own chatter to stderr so it cannot land in that file; the ones that write a run summary
append to `$GITHUB_STEP_SUMMARY` and fall back to `/dev/stdout`.

Nothing here is a thing you would normally run by hand — it is here so a red step can be reproduced by pasting
its `run:` line into a terminal. See `$ci-cd` for which workflow calls which.

## JS Packages (npm workspaces at root)

`just install` covers them — it is the root `npm install`, so one install covers every workspace package.

## TypeScript packages

Five tests — `ts_binacle-compact-notation_unit`, `ts_binacle-vipaq_unit`, `ts_binacle-net-ui_unit`, `ts_cookies_unit` and
`ts_theme-switcher_unit`. They run jest from the repo root through the root `jest.config.js`, which is
what keeps the workspace folder in coverage paths and applies its `collectCoverageFrom`. Running `npm test`
inside a package works but drives the run from that package's own config, so its numbers are not the ones CI
or coverage report.

## Docker

<!-- sourced from docs site; verify against current code if behaviour changes -->

Image: `binacle/binacle-net:latest`. Default internal port: `8080`. `latest` is right for an ad-hoc run of the
newest image; a **sample** never uses it (`$samples#image-pin` has the pinning rule).

Basic run:

```bash
docker run -d --name binacle-net -p 8080:8080 binacle/binacle-net:latest
```

With all UIs and modules enabled:

```bash
docker run -d --name binacle-net -p 8080:8080 \
  -e SWAGGER_UI=True \
  -e SCALAR_UI=True \
  -e UI_MODULE=True \
  binacle/binacle-net:latest
```

Override preset file (read-only bind mount):

```bash
-v $(pwd)/Presets.json:/app/Config_Files/Presets.json:ro
```

Change the internal port (e.g. run on 80 inside the container, expose as 8080 on the host):

```bash
-e ASPNETCORE_HTTP_PORTS=80 -p 8080:80
```

Persist logs — bind a host path to `/app/data/logs` (or `/app/data` for all data):

```bash
-v $(pwd)/data/logs:/app/data/logs
```
