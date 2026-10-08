# Run the built image locally, loaded by the root justfile as the `image` module.
#
#   just image up                same as `up full`
#   just image up full           everything on: all modules, all three backends, the aspire dashboard
#   just image up volume         the image alone, SQLite, data in the named volume binacle-net-data
#   just image up bind           the image alone, SQLite, data in tooling/data, a folder you can open
#   just image down [name]       stop it; add -v to drop the named volumes too
#   just image verify <version>  check a published image: tags, signature, attestations, metadata
#                                a tag-signed release needs its ref passed as the third argument, and a
#                                prerelease its registry as the fourth: ghcr.io/binacle-labs/binacle-net
#   just image dockerhub-overview <version>   print the Docker Hub page as it would be published
#
# Every recipe is a door onto one file in tooling/image/. Nothing is written in here, because a `.just` body
# is invisible to shellcheck and cannot be run on its own - a real script is both.
#
# The stacks run binacle-net:local and check for it first. `verify` reads a published image off Docker Hub,
# or off GHCR for a prerelease, and `dockerhub-overview` only renders text, so neither needs a local build.
#
# Two files, three stacks: `volume` and `bind` share tooling/image.local.yml and differ only in where
# /app/data goes. `full` also brings up serve's services - same postgres, same volume, same port - so it and
# `just serve services-up` cannot run at once. The stack table is in stack.sh.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# The scripts print why they failed, so just's own "Recipe failed" line adds nothing. The exit code still
# gets through.
set no-exit-message := true

# Where a release lands. `verify` takes it as a parameter so CI can check what it just pushed, and so a
# prerelease - which stops on GHCR - can be checked where it is.
hub_repo := 'binacle/binacle-net'

# The Docker Hub page. It carries placeholders and never a version - `dockerhub-overview` fills them in.
overview_file := '.github/dockerhub-overview.md'

# List the recipes
default:
    @just --list image

# The name is positional, so pass it whenever you pass compose flags - `just image up -d` reads -d as the
# stack name and is rejected.

# Bring a stack up [full|volume|bind]
up name="full" *args:
    @tooling/image/stack.sh up {{ quote(name) }} {{ args }}

# Stop a stack [full|volume|bind]
down name="full" *args:
    @tooling/image/stack.sh down {{ quote(name) }} {{ args }}

# Verify a published image [all|tags|signature|attestations|metadata]
verify version check="all" signed_from="refs/heads/main" repo=hub_repo:
    @tooling/image/verify.sh {{ quote(version) }} {{ quote(check) }} {{ quote(signed_from) }} {{ quote(repo) }}

# Render the Docker Hub page for a version <version>
dockerhub-overview version:
    @tooling/image/dockerhub-overview.sh {{ quote(version) }} {{ quote(overview_file) }}
