# Composite actions

The setup steps the workflows share, one folder per action. Each is a plain composite action - a few shell
steps in an `action.yml`, no packaging and no publishing. A workflow uses one by path:

```yaml
- uses: ./.github/actions/setup-dotnet
  with:
    dotnet-version: ${{ vars.DONET_VERSION }}
```

## 🛠️ Installing a toolchain

| Action | What it does |
|---|---|
| `setup-dotnet` | Installs the .NET SDK and restores the NuGet cache. The version is an **input** - pass the repo variable, because an empty value quietly installs a default SDK |
| `setup-node` | Installs Node (default `22`) and restores the npm cache. It does **not** install packages; the caller runs `npm ci` |
| `setup-ruby` | Installs Ruby (default `3.4.7`) and, unlike the others, **does** install the gems. Takes the directory holding the `Gemfile` - a site for a build, `ruby/` for the gem specs |
| `setup-just` | Installs the `just` version this repo's module files need |

## 📥 Installing a pinned binary

Four one-step actions. Each calls `tooling/ci/install-<tool>.sh`, which downloads a release, checks it against
a **pinned SHA-256** and puts it on `PATH`. A changed checksum fails the step rather than running an unknown
binary.

**The version, the checksum and the download live in the script**, not in the action. One home each, and
shellcheck reads them - `just check scripts`.

| Action | Tool | Used by |
|---|---|---|
| `install-hurl` | `hurl` | The image smoke suite |
| `install-container-structure-test` | `container-structure-test` | The image structure check |
| `install-lychee` | `lychee` | The link check before a site deploy |
| `install-actionlint` | `actionlint` | The workflow lint on every pull request |

**The same scripts install these on a laptop** - run `tooling/ci/install-lychee.sh` and you get the bytes CI
gets. [`DEVELOPMENT.md`](../../DEVELOPMENT.md) tells a maintainer to run exactly these, so there is one home
for each version and nothing to keep in step.

## 🏗️ Building

`build-jekyll-site` builds one of the three sites as a pre-flight check - it takes the site name and its
directory, and calls the same `just build <site>` recipe you would run locally. **Deploying is the caller's
job**; this action only proves the site builds.
