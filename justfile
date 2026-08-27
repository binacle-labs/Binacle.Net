# Binacle.Net task runner. `just` with no arguments lists everything.
#
# Install: sudo apt install just
#
# Each module below is one file in tooling/. The benchmark and performance runs are still shell scripts in
# there, not recipes.

# List all tasks
default:
    @just --list

# Tests: `just test all`, one with `just test <name>`, every name with a bare `just test`.
mod test 'tooling/tests.just'

# Coverage for those tests: `just coverage all [cobertura|sonar]`, `just coverage report` for the HTML.
mod coverage 'tooling/coverage.just'

# OpenAPI documents: `just openapi generate [dir]`, `just openapi lint [dir]` to Spectral them too.
mod openapi 'tooling/openapi.just'

# The .agents/ manifests: `just agents all` after adding, renaming or re-describing a file there.
mod agents 'tooling/agents.just'

# The committed generated data: `just regen all`, `just regen check` to prove it is in step.
mod regen 'tooling/regen.just'

# CHANGELOG.md sections: `just changelog extract <version|Unreleased>`, `just changelog check <version>`.
mod changelog 'tooling/changelog.just'

# Run from source: `just serve api [profile]`, `just serve docs|demo|www`, `just serve services-up`.
mod serve 'tooling/serve.just'

# Make the API image: `just build publish` for the app, `just build image [version]` for the container.
mod build 'tooling/build.just'

# Run that image: `just image up [full|volume|bind]`, `just image down [name]`.
mod image 'tooling/image.just'

# Smoke the built image: `just smoke all`, `just smoke test-structure`, `just smoke test <profile>`.
mod smoke 'tooling/smoke.just'

# Check what was built: `just check links` for every site, `just check links <site>` for one.
mod check 'tooling/check.just'

# The shell a workflow runs: `just ci gate <json>`, `just ci changed-paths <base> <head>`, one file each.
mod ci 'tooling/ci.just'

# Two recipes rather than an `install` module: a fresh clone wants all of it, and the asset copy is the only
# part worth running on its own.

# Everything a fresh clone needs before `just serve` works
[group('dev')]
install:
    npm install
    cd sites/docs && bundle install
    cd sites/demo && bundle install
    cd sites/www && bundle install
    @just assets

# Copy assets/ into the three sites and the UI module - run it after changing anything under assets/
[group('dev')]
assets:
    npm run copy-assets-to-docs
    npm run copy-assets-to-demo
    npm run copy-assets-to-www
    npm run copy-assets-to-uimodule
