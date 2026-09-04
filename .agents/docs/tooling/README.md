---
id: tooling
description: "tooling/ — every task the repo can run, called by CI and by hand alike: the test, coverage, openapi, agents, regen, changelog, serve, build, check, image, smoke and ci modules for just, the benchmark/performance scripts, the wrangler configs, the local compose stacks, and emulator state"
verified: 2026-09-04
check: "Script list, tooling/agents/ holds generate-index.py and indexes.toml and the agents module calls the script, tests.just recipes and its four group recipes, coverage.just recipes, openapi.just, agents.just, regen.just, changelog.just, serve.just, build.just (the API publish/image pair and the three site builds), check.just and its lychee.toml, image.just (stacks and the four verify checks, whose certificate identity must match SECURITY.md) and smoke.just recipes, ci.just recipes and the tooling/ci/*.sh files they call, and the compose stack/file/service table match tooling/"
also_update:
  - commands
  - samples
paths:
  - "tooling/**"
---

# Tooling

`tooling/` holds **every task the repo can run** — the just modules for tests, coverage, OpenAPI, the
agent indexes, the build, the image stacks and the smoke suite, plus the benchmark scripts, local Docker
Compose and emulator state. CI calls these same recipes rather than keeping its own copy, so a workflow step
and a maintainer typing the command do the same thing. It is **not** a deployment template; user-facing
deployment starting points live in samples (`$samples`). For the quick "how do I run X" reference see
`$commands`; this doc describes what's in the directory.

## Scripts and `just` modules (run from the repo root)

| Script | What it does |
|---|---|
| `serve.just` | **Not a script** — the `serve` module for the root `justfile`, everything run **from source**. `just serve api [profile]` runs the API via `dotnet run -lp <profile>` (`Normal`/`WithServiceModuleOnly`/`WithUiModuleOnly`/`WithAllModules`, aliases `N/S/U/All`, default `Normal`); `just serve docs`, `just serve demo` and `just serve www` run jekyll + webpack watch together from `sites/<site>`; `just serve services-up [-d]` / `just serve services-down` bring up what the API talks to |
| `tests.just` | **Not a script** — the `test` module for the root `justfile`. One recipe per suite, run with `just test <name>`, plus four group recipes — `image`, `sites`, `all` and `all-with-services`. The tests are `[private]`, so completion offers the four groups; a bare `just test` prints every test name. The three test lists in it are the only copy of the set of tests, and `all` is written out as the sum of the other two with nothing run twice. A group is for a laptop; CI names every test as its own step. See `$commands` for the list |
| `performance.<slice>.sh` | `dotnet run -c Release` for the slice's `PerformanceTests`. Slices `lib`, `vipaq`. Writes to gitignored `PerformanceTests.Artifacts` |
| `benchmarks.<slice>.sh [alias]` | `dotnet run -c Release --filter <pattern>` from the slice's `Benchmarks` project. Slices `lib`, `vipaq`. No arg = all |
| `build.just` | **Not a script** — the `build` module for the root `justfile`, **everything this repo builds**. `just build publish` runs the asset copy and the UI module's `npm run build`, then publishes the API (`-c Release -o artifacts/binacle-net --no-self-contained --runtime linux-x64`) — the bundles first, because dotnet collects static web assets at publish time; `just build image [version]` publishes then `docker build -t binacle-net:<version>` (default `local`), applying the three per-build OCI labels. `just build docs`, `just build demo` and `just build www` build the three Jekyll sites under `sites/` into gitignored `artifacts/docs`, `artifacts/demo` and `artifacts/www` — asset copy, then webpack, then `jekyll build` with `_config.yml,_config.prod.yml`; they mirror `just serve <site>` and are the build half of that pair, and the deploy workflows hand what they produce straight to the host. None starts compose and none needs `sudo`, so CI calls them as they stand — see `$ci-cd` |
| `check.just` | **Not a script** — the `check` module for the root `justfile`. `just check links` checks the internal links in all three built sites with `lychee --offline`, `just check links <site>` one of them, `just check links-external <site>` every link including other people's servers, `just check workflows` runs actionlint over `.github/workflows`, and `just check actions` greps the `.github/actions` manifests for the `vars`/`secrets` expression actionlint cannot see, and `just check scripts` runs shellcheck over `tooling/*.sh` and `tooling/ci/*.sh`. All three list the files they were handed and end with a count, so a clean run reads differently from one that never started. Reads `artifacts/<site>`, so the site has to be built first — it stops with a pointer to `just build <site>` rather than checking nothing. `tooling/check.lychee.toml` holds the URLs it must never check and is named with `--config`, because lychee only finds a config in the working directory by itself |
| `coverage.just` | **Not a script** — the `coverage` module for the root `justfile`. Runs the tests with the collector attached and writes to gitignored `artifacts/tests/` + `artifacts/coverage/`; see `$commands` |
| `coverage.run.sh`, `coverage.table.sh` | The bodies behind `just coverage run` and `just coverage table`. Files rather than recipe bodies so `just check scripts` covers them. `coverage.run.sh` also merges the ten gem reports into one `ruby.json` for a `sonar` run; `coverage.table.sh` prints the per-suite table and the run's exit code. `tests.ruby-coverage.rb` beside them is what `RUBYOPT` loads to start SimpleCov |
| `openapi.just` | **Not a script** — the `openapi` module for the root `justfile`. `just openapi generate [dir]` builds the v3/v4 documents into gitignored `artifacts/openapi/`, `just openapi lint [dir]` generates then Spectral-lints them against `tooling/openapi.spectral.yaml`, named with `--ruleset` and run with `--fail-severity=warn` so a warning fails the run, and `just openapi check-site-copies` generates then fails if the docs site's hand-placed copies of the current version have drifted from the generator. `current_docs_version` at the top of the module says which version folder is compared; the frozen ones below it never are |
| `agents.just` | **Not a script** — the `agents` module for the root `justfile`. `just agents all` regenerates the `_index.md` manifest for `.agents/rules`, `.agents/docs`, `.agents/design`, `.agents/plans` and `.agents/memory` (grouped by area); `just agents generate-index <name>` does one. The recipes call `agents/generate-index.py` |
| `agents/generate-index.py` | The generator behind that module. Python, not shell: nothing in CI calls it, so it can use a real parser. Reads `indexes.toml` beside it, walks `.agents/<name>/` and writes the manifest. Sorts by byte order, so two machines produce the same file |
| `agents/indexes.toml` | What `generate-index.py` writes — one table per manifest with its heading and blurb, the group names whose capitalisation cannot be guessed, and which front matter keys become entry fields. A directory under `.agents/` is indexed only if it has a table here |
| `regen.just` | **Not a script** — the `regen` module for the root `justfile`. The four generators whose output is **committed**: `just regen or-lib-scenarios` (OR-Library text → `shared/data/bischoff-suite`), `just regen vipaq-packed-data` (that plus `custom-problems`, packed → `vipaq/data/packed`), `just regen vipaq-interop-vectors` (the C# and TS interop halves plus the header bytes → `vipaq/test-vectors`), `just regen demo-samples` (`shared/data/demo-samples` → the demo's sample set in `packages/binacle-net-ui`), `just regen all` in dependency order, and `just regen check` which runs `all` then fails if any generated `.json` moved. None takes an argument — each tool runs every generator in its list so it cannot half-run. **No workflow calls `check`**. This module covers data generated *into* the repository and nothing else — the docs site's OpenAPI copies are checked by `just openapi check-site-copies`, not here |
| `changelog.just` | **Not a script** — the `changelog` module for the root `justfile`. Reads `CHANGELOG.md` at the repo root. `just changelog extract <version\|Unreleased>` prints one release's section, with its headings promoted from `###` back to `##` for a release body; `just changelog check <version\|Unreleased>` exits 1 if that section is missing or empty. The release workflow calls both, so CI and a laptop parse the file the same way and the exact body can be previewed before the release is dispatched — see `$ci-cd/release-pipeline` |
| `image.just` | **Not a script** — the `image` module for the root `justfile`. Runs what `build.just` produced: `just image up [full\|volume\|bind]` (default `full`) and `just image down [name]`; extra arguments pass through to `docker compose`. `up` creates and opens the bind-mounted folders first, and every stack stops with a pointer to `just build image` if `binacle-net:local` is missing. **Two recipes are the odd ones out** — `just image verify <version> [check]` reads a *published* image off Docker Hub, and `just image dockerhub-overview <version>` renders the Docker Hub page; neither builds anything and neither logs in. See below |
| `smoke.just` | **Not a script** — the `smoke` module for the root `justfile`. Tests the image rather than the code. `just smoke test-structure [image]` runs `container-structure-test` against `tooling/smoke/structure.yaml`; `just smoke test <profile> [image]` does up → hurl → down for one profile; `just smoke up`/`down` are the manual halves; `just smoke all [image]` builds, checks the structure once, then runs every profile. Every recipe takes the image last, default `binacle-net:local`, so a published tag can be smoked too |
| `ci.just` + `ci/*.sh` | **The `ci` module plus one script per operation.** The shell a workflow runs, kept out of the YAML: a `.just` body can be neither run nor shellchecked on its own, and a `.sh` file is both - `just check scripts` is what actually checks them. `ci.just` is a door - two lines per recipe - and `tooling/ci/<name>.sh` is the code. Eighteen operations today: `changed-paths`, `gate`, `deploy-message`, `push-tag`, `deploy-summary`, `sonar-summary`, `check-release-ref`, `check-version`, `check-release-tag`, `changelog-section`, `moving-tags`, `copy-tags`, `github-release`, `release-summary`, `pull-image`, `smoke-summary`, `codeql-summary`, `dockerhub-version`. Four more scripts sit beside them with no recipe - `install-actionlint.sh`, `install-container-structure-test.sh`, `install-hurl.sh`, `install-lychee.sh` - each called by path from the matching composite action, because an action that installs a tool must not need `just` installed first. Every one takes its inputs as arguments and reads no `github.*` context, so all of them run on a laptop; the ones that print `key=value` are teed into `$GITHUB_OUTPUT` and the ones that write a summary fall back to `/dev/stdout`. See `$ci-cd` |
| `cloudflare/` | **Not a script** — one wrangler config per site: `docs.wrangler.jsonc`, `demo.wrangler.jsonc` and `www.wrangler.jsonc`, plus a `README.md`. They are the whole deployment configuration for the three sites. **Nothing here is run by hand** — the three deploy workflows call `wrangler deploy --config` against them; see `$ci-cd` |

The launch profiles live in `serve.just`; the benchmark filters live inside the per-slice `benchmarks.*`
scripts.

The five TS tests (`ts_binacle-compact-notation_unit`, `ts_binacle-vipaq_unit`, `ts_binacle-net-ui_unit`,
`ts_cookies_unit`, `ts_theme-switcher_unit`) run jest from the repo root. Run
`just install` first — it does the root `npm install` (the packages are npm workspaces, so one install covers
them all), `bundle install` for all three jekyll sites and for `ruby/`, and copies `assets/` into
`sites/docs/`, `sites/demo/`, `sites/www/` and the UI module's `wwwroot/`.

## Local Docker Compose

**Three files, each named after the module that runs it.** `serve.services.yml` brings up what the app talks
to and no binacle-net at all, so it belongs to `serve`, alongside `just serve api` (and it is what the
Postgres/AzureStorage tests need). The two `image.*.yml` files follow `just build image` and answer a
different question — does the shipped image work — so they are the `image` module's stacks, and that is why
only they check for `binacle-net:local`. The five under `smoke/` answer a narrower question again — does it
work *as configured* — and are driven entirely by `just smoke`, never by hand.

**Three stacks come out of two image files.** `volume` and `bind` are one container differing only in where
`/app/data` goes, so `image.local.yml` serves both and `_compose` picks with `-p` and `BINACLE_DATA_DIR` out
of the `_stack` table.
`image.full.yml` `include:`s that file and `serve.services.yml`, then overrides the app's storage and
telemetry — so postgres, azurite and the dashboard are declared once, in `serve`.

| File | Module | Command | Project name | Runs |
|---|---|---|---|---|
| `serve.services.yml` | `serve` | `just serve services-up` | `binacle-net-services` | **Backing services only** — `aspire-dashboard`, `azurite`, `postgres`. No API. The only place those three are declared |
| `image.full.yml` | `image` | `just image up full` | `binacle-net-full` | **Full** — `include:`s the other two files and overrides the app's storage and telemetry, so it is about twenty lines. Local image + `azurite` + `postgres` + `aspire-dashboard`, all modules on; injects `OpenTelemetry.Production.json` on top of the `JwtAuth.json` it inherits. All three storage backends run; Postgres wins on provider order, swap by moving the comment. The `image` module's default |
| `image.local.yml` | `image` | `just image up volume` | `binacle-net-volume` | **Simple** — the local image alone, ServiceModule on SQLite, data in the named volume `binacle-net-data` |
| `image.local.yml` | `image` | `just image up bind` | `binacle-net-bind` | **Simple** — the same file, with `BINACLE_DATA_DIR` set by the recipe so `/app/data` is a bind at `tooling/data`. Compose then drops the volume declaration, so this stack leaves none behind |
| `smoke/<profile>.yml` | `smoke` | `just smoke up <profile>` | `binacle-smoke-<profile>` | **Five throwaway stacks** — `minimal`, `quickstart`, `prod`, `service`, `full`, one per smoke profile, and each name is also a `samples/docker/` folder. Storage is a named volume dropped on teardown, so they need no `_prepare`. They take the image from `$BINACLE_IMAGE` (default `binacle-net:local`); `service`/`full` inline `JwtAuth.json` and raise `RateLimiter__ApiUsageAnonymous` so a second run inside the hour does not go red on 429s; `prod` mounts its own `Presets.json` so reading it back proves the config-mount path |

Each file carries its own `name:` as a fallback, but `image.just` passes `-p` — two stacks share one file, so
without it `up bind` would recreate the `volume` container. **One table in `image.just` holds the mapping** —
the private `_stack` recipe, four columns per stack: compose file, project name, `/app/data`, and the other
folders it bind-mounts. `_compose` and `_prepare` both read it, so `up`, `down` and the folder setup cannot
disagree about what a name means, and it is the only place an unknown name is rejected. `smoke.just` gets the
same guarantee for free, since the profile name **is** the filename.

**Both named volumes carry a fixed `name:`** — `binacle-net-postgres` and `binacle-net-data` — so compose does
not prefix them with the project. That is what makes `serve services-up` and `image up full` one database
rather than two that look alike, and it means `-v` in either place wipes it for both. The two also publish the
same 5432, so they cannot run at once; the second one fails on the port, loudly, and leaves the first alone
(checked 2026-08-15).

### What compose does here — tested 2026-08-15 against compose v5.4.0

Four behaviours the shape above rests on. They were **run, not reasoned**. Do not re-derive them.

- **`include:` resolves an included file's relative paths against that file's own directory.** `-f a.yml -f
  b.yml` resolves every path against the **first** file instead. Same two files, same `./azurite` source, two
  different answers. **This retires the subfolder trap**: the 2026-08-07 attempt to move these files into a
  subfolder was reverted because the shared azurite bind silently stopped being shared — that was an `-f`
  failure, and `include:` does not have it. A subfolder is safe now; it is simply not used, because flat
  beside the `.just` files reads better.
- **An including file overrides an included service key by key.** Everything it does not name carries through,
  which is why `image.full.yml` is twenty lines rather than a second copy of the app. An included `name:` is
  ignored in favour of the including file's.
- **`${BINACLE_DATA_DIR:-app_data}:/app/data/` is what switches a named volume for a bind.** Unset gives
  `type: volume`; `./data` gives `type: bind` at the resolved absolute path **and drops the top-level
  `volumes:` declaration**, so the bind stack leaves no orphan volume. A value with no leading `./` is read as
  a volume name and refuses to start — `refers to undefined volume data: invalid compose project` — which is
  the good outcome: it fails before anything writes data where nobody looks.
- **A fixed volume `name:` is shared across projects** with no `external: true` and no pre-creation step.
  `down -v` still removes it when nothing else references it, and prints `Resource is still in use` when
  something does. Both are legible; what compose does **not** do is warn that the volume being dropped is
  shared by another stack.

**A bare `just image up` means `full`.** Raised and settled on 2026-08-15: it stays the expensive stack,
because that is the one that exercises the whole image, which is what the module is for.

### Do not

- **Give postgres a bind mount**, in any file, for any reason. It chowns its data directory to its own user
  and locks it to 0700, leaving a folder in the repo nobody can read — and `docker build` walks the whole
  context, so the next build fails on it. The named volume is deliberate and survives every rename.
- **Compose these together with `-f a.yml -f b.yml`.** Path resolution, above. That is the exact failure that
  got the 2026-08-07 attempt reverted.
- **Have `image.just` call a recipe in `serve.just`, or the reverse.** The `mkdir` and `chmod` lines are
  copied on purpose.
- **Let `image.local.yml` default `BINACLE_DATA_DIR` to a path.** The recipe sets it for `bind` and unsets it
  for the other two. Inside the file, unset must stay the named volume — otherwise a bare `docker compose -f`
  run starts leaving container-owned folders in the repo, and one of those fails the next `docker build`.
- **Drop `-p`** and rely on the file's own `name:`. Two paragraphs up for why.

## Rendering the Docker Hub page

`just image dockerhub-overview <version>` in `image.just`. `.github/dockerhub-overview.md` is the page Docker Hub shows,
and it carries `{{VERSION}}` and `{{MINOR}}` rather than a version — so it is right for every release instead
of for the one it was written in. This recipe fills them in and prints the result; it writes nothing.

**Here for the same reason `changelog extract` is.** The release workflow calls it and pipes the output to the
Docker Hub API, so what you read locally is exactly what gets published.

Two guards, both of which fail the release step rather than publishing:

| Guard | Why |
|---|---|
| Rejects a version with a hyphen, or one that is not `x.y.z` | A prerelease moves neither the minor tag nor `latest`, so every tag the page would name is one that does not exist |
| Fails if any `{{...}}` survives the substitution | A raw placeholder on a public landing page is worse than a red job |

**The braces in the recipe are doubled.** The page's placeholders look exactly like `just` interpolation, and
doubling is how `just` is told they are not — halve them and the substitution silently stops matching.

## Verifying a published image

`just image verify <version> [check]` in `image.just`, with the four checks as private `_verify-*` recipes.
Each is one question and the order matters — every one answers something the next assumes.

**Docker Hub only.** GHCR is the release workflow's staging registry and nothing outside that workflow reads
it, so this recipe knows one repository. It carried a fifth check until 2026-08-15, `digest`, which compared
the tag across both registries.

| Check | What it proves |
|---|---|
| `tags` | The Docker Hub tag map, from the v2 API. Rows sharing a digest are one image under several names — how you see what `latest` resolves to. The **date** is the trap: it moves for reasons that are not a retag, so it is printed and never compared |
| `signature` | `cosign verify` against the Docker Hub tag. A signature is a referrer stored beside the image, not inside the index, so it does not survive `imagetools create` and the pipeline signs after the copy as well as before it. Needs `cosign`; fails with a pointer when it is missing |
| `attestations` | The SPDX SBOM package count and the SLSA provenance builder id. Both are manifests **inside** the index, so the index digest hashes them and the one signature already covers them — nothing extra to verify, this only reports what is attached |
| `metadata` | The three OCI labels, then a throwaway run: `BINACLE_VERSION`, the uid, `/app/data`'s owner, and the `System.*.dll` count in `/app`. That count is the **framework-dependent proof** — 4 on a framework-dependent build, ~170 on a self-contained one |

**The version argument is required and never defaults**; a default rots into a tag nobody meant to check.
**No `docker login` anywhere** — these are the commands a user runs, and a check that only passes with a
credential is not checking a public artifact. The aggregate does **not** use `set -e`: it runs all four, ORs
the exit codes and fails at the end, because the first failure otherwise hides the three answers explaining it.

**Two just traps live in this recipe** and both cost real time:

- **A Go template needs four braces open, two closed** — `{{{{ json .SBOM }}`. Two opening braces are just's
  own interpolation. Four closing braces emit a literal `}}` on the end of every value, which still looks
  right: piping into `jq` gives `parse error: Unmatched '}'` under a correct-looking answer.
- **A backtick in a recipe-body comment is executed by just**, before the shell ever sees the line. Found
  2026-08-15 by writing the brace explanation with backticks around the braces; the recipe died on
  `Backtick failed with exit code 2`. Explain punctuation in words down there, not in code spans.

**Only `3.0.0-beta.3` and later can pass.** The recipe matches the signature against the `binacle-labs`
certificate identity, so it accepts only images signed after the repository moved. `3.0.0-beta.2` **is**
signed, but under the old identity, and fails `signature` for that reason alone. `3.0.0-beta.1`, `2.1.1` and
everything earlier were never signed and fail both `signature` (`no signatures found`) and `attestations`
(no SBOM). Neither case is a broken check. It also binds every user-facing surface: an example naming a tag
must name one that passes **today**.

**`3.0.0` is the reference tag** — the one to re-run against and the one to name in an example. Green on all
four checks, 2026-09-01: `3.0.0`, `3.0` and `latest` on one digest, signed by the release workflow under the
new identity, a 166-package SBOM, and provenance naming the build run. **`latest` passes now**, which it could
not before v3.0.0 published. `2.1.1` is still the tag to watch it fail against — no signature, no SBOM, and
172 System dlls where a framework-dependent build carries 4.

The cosign invocation was proven against **cosign 3.1.3**, the version `DEVELOPMENT.md` pins. Both flags were
kept; dropping the identity would make the check ask only whether anyone signed the image.

The smoke stacks are separate files from `samples/` on purpose. They run the image under test and carry
test-only tweaks — a raised rate limit, disposable storage — that a sample a user copies must never have.

**`tooling/smoke/README.md` is the authority on that suite** — what each profile claims, the two rules that
decide whether a check belongs in it (`assert what the image contains and wires, never what the algorithm
computed`; `every check must be able to fail`), and the setup gotchas. It is written for a human, and it is
where the design rationale went when the smoke plan was deleted on 2026-08-07. Read it before changing an
assertion; do not re-derive any of it here.

Which folders `up` prepares: `serve services-up` needs `tooling/azurite`; `image up full` needs that same one
and nothing more, since its `/app/data` is the named volume; `image up bind` needs `BINACLE_DATA_DIR` (default
`tooling/data`); `image up volume` needs none. It opens the **directory** only, never `-R` — the files inside
belong to whoever wrote them (the app as `APP_UID`, azurite as root) and stay writable to that writer, so a
recursive `chmod` would fail on them while making nothing more writable. `sudo` is used only for a directory
docker created itself, which the daemon makes as root. The few lines that do this are **copied** into both
modules rather than shared: a module reaching into another one restores the coupling the split removed.

## Emulator state
- `tooling/azurite/` holds Azurite emulator state (`__azurite_db_*__.json`).
- `tooling/tooling.proj` is a `Microsoft.Build.NoTargets` content project (no compile) that includes the config
  files in the solution — see `$build-topology`.

## The folder itself

Renamed from `config/` on 2026-08-12, and `build/` became `artifacts/` in the same change. The old names were
each wrong in a different way: `config/` held recipes rather than configuration, while the API's own runtime
settings live in `Config_Files`; `build/` held output, which the .NET convention calls `artifacts/` and the Go
convention reads as the opposite — build *scripts*. `eng/` was considered and rejected as jargon, `tooling/`
being the plain word for what it holds. `artifacts/` is not `results/`: that one is committed measured
evidence, records that outlive a build.

**Every module sets `set working-directory := '..'`, which resolves relative to the module file.** The folder
therefore has to stay one level below the repo root. Moving it deeper or shallower breaks every path in every
recipe at once, and does it silently — the recipes still parse.

**References point one way: out of `tooling/`, never into it.** This folder may name anything; almost nothing
may name it. The exceptions are (a) whatever operates on it — the `justfile` `mod` lines, `Binacle.Net.slnx`,
the `.gitignore` state-dir patterns, the `/s:` argument in `sonar-analysis.yml`; (b) this `.agents/` layer,
whose job is describing the repo; (c) the repo's own top-level docs, `README.md`, `DEVELOPMENT.md` and
`CLAUDE.md`, because something has to say the folder exists. **Never a comment**, and never a file a user
copies — a sample that names a maintainer's path is handing a reader something they cannot use. A comment that
needs to talk about this folder is a briefing, not a trap, and belongs here or in design instead.
