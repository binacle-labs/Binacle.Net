# Everything this repo builds, loaded by the root justfile as the `build` module.
#
#   just build publish            publish the API into artifacts/binacle-net
#   just build image [version]    publish, then docker build -t binacle-net:<version>
#   just build docs               the documentation site into artifacts/docs
#   just build demo               the demo site into artifacts/demo
#   just build www                the marketing site into artifacts/www
#
# The three site recipes mirror `just serve docs` / `demo` / `www`: same site, built once instead of served
# and watched. They are the build half of the pair, which is why they are here rather than in `serve`.
#
# Nothing here is interactive, uses sudo, or names a local-only path, so CI calls these as they stand.
# Preparing the bind-mounted tooling/data and tooling/azurite is not here - that is a precondition of running
# a compose stack, not of building.

# Module recipes run where the module file lives (tooling/), which would break every path below.
set working-directory := '..'

# dotnet and docker both print why they failed, so just's own "Recipe failed" line adds nothing. The exit code
# still propagates, so CI still fails.
set no-exit-message := true

api_project := "api/src/Binacle.Net/Binacle.Net.csproj"

# Not a parameter: the Dockerfile hardcodes COPY ["artifacts/binacle-net", "."] and .dockerignore allowlists that
# one path, so publishing anywhere else builds an image without the app in it. A `dir` argument would only
# offer a choice that does not exist.
publish_dir := "artifacts/binacle-net"

# List the recipes
default:
    @just --list build

# One command, not restore + publish --no-restore: publish restores on its own, and nothing caches the restore
# between the two, so the split step only adds a place for the two runtime identifiers to drift apart.
#
# --no-self-contained is spelled out rather than left to the default of a bare --runtime: the runtime comes
# from the base image, which is the whole point, and that has to be readable on the line.
#
# Keep this in step with release-docker-image.yml's publish step - the two must not drift.

# dotnet collects static web assets at publish time, so the UI module's wwwroot has to be filled first.
# Nothing fails when it is not - the image ships pages with no styling and demos that do nothing.

# Publish the API framework-dependent for linux-x64, into artifacts/binacle-net
publish:
    npm run copy-assets-to-uimodule
    cd api/src/Binacle.Net.UIModule && npm run build
    dotnet publish {{ api_project }} -c Release -o {{ publish_dir }} --no-self-contained --runtime linux-x64

# The publish is a dependency rather than a check on the folder: `docker build` reads whatever is in
# artifacts/binacle-net, so skipping it when the folder exists is how a stale image gets tagged and shipped.
#
# The version lands in the image twice - as the tag and, via the build arg, as BINACLE_VERSION, which is what
# the running container reports. They come from the same value here for the same reason the release workflow
# feeds both from the metadata step: an image whose reported version disagrees with its tag is unreportable.

# The per-build OCI labels live here, not in the Dockerfile: revision and created change every build, so as
# Dockerfile LABELs fed by ARG they would invalidate the layer cache from that point down. Applied with
# --label they set image-config metadata with no layer and no cache cost. This is the local counterpart to
# metadata-action, so a locally built image carries the same version/revision/created a pushed one does.

# Publish, then build the image [version]
image version="local": publish
    #!/usr/bin/env bash
    set -euo pipefail
    revision="$(git rev-parse HEAD)"
    created="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
    docker build \
      --build-arg VERSION="{{ version }}" \
      --label "org.opencontainers.image.version={{ version }}" \
      --label "org.opencontainers.image.revision=${revision}" \
      --label "org.opencontainers.image.created=${created}" \
      -t "binacle-net:{{ version }}" .

# The three Jekyll sites. Three steps in a fixed order: the asset copy fills the site's gitignored lib/ and
# media/, webpack compiles _js/ into its gitignored js/, then jekyll assembles. `jekyll build` alone succeeds
# and ships a site with no scripts and no logo.
#
# --config takes both files: _config.yml holds everything, _config.prod.yml overrides the three values that
# differ off localhost - site url, api url, analytics container. Jekyll merges later files over earlier ones.
#
# Output goes to artifacts/<site>, set as `destination` in each _config.yml rather than passed here. The
# deploy workflows hand that directory straight to the host.

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
    # They stopped being the same string when the sites moved under sites/.
    npm run copy-assets-to-{{ name }}
    cd "{{ dir }}"
    npm run build
    bundle exec jekyll build --config _config.yml,_config.prod.yml
