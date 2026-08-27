# Builds everything this repo builds.
#
#   just build publish            publish the API into artifacts/binacle-net
#   just build image [version]    publish, then docker build -t binacle-net:<version>
#   just build docs               the documentation site into artifacts/docs
#   just build demo               the demo site into artifacts/demo
#   just build www                the marketing site into artifacts/www
#
# The three site recipes are the build half of `just serve docs|demo|www` - the same site, built once
# instead of served and watched.
#
# Nothing here is interactive and nothing needs sudo, so CI calls these as they stand. Preparing the
# bind-mounted tooling/data and tooling/azurite is not here: that is a precondition of running a stack, not
# of building one.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# dotnet and docker both print why they failed, so just's own "Recipe failed" line adds nothing. The exit
# code still gets through, so CI still fails.
set no-exit-message := true

api_project := "api/src/Binacle.Net/Binacle.Net.csproj"

# Not an argument. The Dockerfile hardcodes COPY ["artifacts/binacle-net", "."] and .dockerignore lets only
# that one path through, so publishing anywhere else builds an image with no app in it.
publish_dir := "artifacts/binacle-net"

# List the recipes
default:
    @just --list build

# dotnet collects the static web assets at publish time, so the UI module's wwwroot has to be filled first.
# Nothing fails when it is not - the image ships pages with no styling and demos that do nothing.

# Publish the API framework-dependent for linux-x64, into artifacts/binacle-net
publish:
    npm run copy-assets-to-uimodule
    cd api/src/Binacle.Net.UIModule && npm run build
    dotnet publish {{ api_project }} -c Release -o {{ publish_dir }} --no-self-contained --runtime linux-x64

# Publish, then build the image [version]
image version="local": publish
    #!/usr/bin/env bash
    set -euo pipefail

    revision="$(git rev-parse HEAD)"
    created="$(date -u +%Y-%m-%dT%H:%M:%SZ)"

    # One value for the tag and for BINACLE_VERSION, which is what the running container reports.
    #
    # --label, never a Dockerfile LABEL fed by ARG: revision and created change every build, so as LABELs
    # they would invalidate the layer cache from that point down.
    docker build \
      --build-arg VERSION="{{ version }}" \
      --label "org.opencontainers.image.version={{ version }}" \
      --label "org.opencontainers.image.revision=${revision}" \
      --label "org.opencontainers.image.created=${created}" \
      -t "binacle-net:{{ version }}" .

# Three steps in a fixed order: the asset copy fills the site's gitignored lib/ and media/, webpack compiles
# _js/ into its gitignored js/, then jekyll assembles. `jekyll build` on its own succeeds and ships a site
# with no scripts and no logo.
#
# Both config files: _config.yml holds everything, _config.prod.yml overrides the three values that differ
# off localhost. Jekyll merges later files over earlier ones.
#
# The output directory is `destination` in each _config.yml, not passed here. The deploy workflows hand that
# directory straight to the host.

# The documentation site into artifacts/docs
docs: (_build-site 'docs' 'sites/docs')

# The demo site into artifacts/demo
demo: (_build-site 'demo' 'sites/demo')

# The marketing site into artifacts/www
www: (_build-site 'www' 'sites/www')

[private]
_build-site name dir:
    #!/usr/bin/env bash
    set -euo pipefail

    # Two arguments because they are two things: the gulp target is keyed on the name, jekyll needs the path.
    npm run copy-assets-to-{{ name }}

    cd "{{ dir }}"
    npm run build
    bundle exec jekyll build --config _config.yml,_config.prod.yml
