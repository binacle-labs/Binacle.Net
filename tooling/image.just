# Run the built image locally, loaded by the root justfile as the `image` module.
#
#   just image up                same as `up full`
#   just image up full           everything on: all modules, all three backends, the aspire dashboard
#   just image up volume         the image alone, SQLite, data in the named volume binacle-net-data
#   just image up bind           the image alone, SQLite, data in tooling/data, a folder you can open
#   just image down [name]       stop it; add -v to drop the named volumes too
#   just image verify <version>  check a published image: tags, signature, attestations, metadata
#   just image dockerhub-overview <version>   print the Docker Hub page as it would be published
#
# The stacks run binacle-net:local and check for it first. `verify` reads a published image off Docker Hub
# and `dockerhub-overview` only renders text, so neither needs a local build.
#
# Two files, three stacks: `volume` and `bind` share tooling/image.local.yml and differ only in where
# /app/data goes. `full` also brings up serve's services - same postgres, same volume, same port - so it and
# `just serve services-up` cannot run at once.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# docker prints why it failed, so just's own "Recipe failed" line adds nothing. The exit code still gets
# through.
set no-exit-message := true

# Where a release lands.
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
    #!/usr/bin/env bash
    set -euo pipefail

    just image::_prepare "{{ name }}"
    just image::_compose "{{ name }}" up {{ args }}

# Stop a stack [full|volume|bind]
down name="full" *args:
    @just image::_compose "{{ name }}" down {{ args }}

# Verify a published image [all|tags|signature|attestations|metadata]
verify version check="all":
    #!/usr/bin/env bash
    # No `set -e`: every check runs and the codes are OR-ed at the end, so the first failure cannot hide the
    # answers that explain it.
    set -uo pipefail

    if [ -z "{{ version }}" ]; then
        echo "A version is required: just image verify 3.0.0" >&2
        exit 1
    fi
    case "{{ check }}" in
        all) checks="tags signature attestations metadata" ;;
        tags|signature|attestations|metadata) checks="{{ check }}" ;;
        *)
            echo "Unknown check '{{ check }}'." >&2
            echo "Use all, tags, signature, attestations or metadata." >&2
            exit 1
            ;;
    esac

    echo "Verifying binacle-net {{ version }}"
    rc=0
    failed=""
    for c in $checks; do
        just image::_verify-$c "{{ version }}" || { rc=1; failed="$failed $c"; }
        echo
    done
    if [ "$rc" -eq 0 ]; then
        echo "PASS - everything checked came back clean."
    else
        echo "FAIL -$failed" >&2
    fi
    exit $rc

# Prints to stdout and writes nothing, so you can read the exact page before pushing the tag.

# Render the Docker Hub page for a version <version>
dockerhub-overview version:
    #!/usr/bin/env bash
    set -euo pipefail

    version='{{ version }}'
    version="${version#v}"

    case "$version" in
        # A prerelease has no minor tag and no latest, so every tag this page names would be missing.
        *-*)   echo "'$version' is a prerelease. This page describes the stable line only." >&2; exit 1 ;;
        *.*.*) ;;
        *)     echo "Expected a full version like 3.0.0, got '$version'." >&2; exit 1 ;;
    esac

    # Every brace is doubled because just would otherwise read the page's placeholders as its own
    # interpolation.
    out="$(sed -e "s/{{{{VERSION}}/$version/g" -e "s/{{{{MINOR}}/${version%.*}/g" '{{ overview_file }}')"

    if grep -n '{{{{' <<<"$out" >&2; then
        echo "^ unsubstituted placeholder in {{ overview_file }}" >&2
        exit 1
    fi
    printf '%s\n' "$out"


# The one table. Every other recipe reads it, and it is the only place an unknown name is rejected - a typo
# would otherwise fall through to a compose default and report success for a stack nobody started.
#
# Four columns, `-` for none. Paths are relative to tooling/, because compose resolves a relative bind source
# against the compose file's own directory.
#
#   stack     compose file      project name         /app/data   other folders it bind-mounts
[private]
_stack name:
    #!/usr/bin/env bash
    set -euo pipefail

    case "{{ name }}" in
        full)   echo "image.full.yml  binacle-net-full   -       ./azurite" ;;
        volume) echo "image.local.yml binacle-net-volume -       -" ;;
        bind)   echo "image.local.yml binacle-net-bind   ./data  -" ;;
        *) echo "Unknown stack '{{ name }}'. Use full, volume or bind." >&2; exit 1 ;;
    esac

# -p is not optional: `volume` and `bind` come out of one file, so without it `up bind` recreates the
# `volume` container and reads its data as an empty database.
[private]
_compose name *args:
    #!/usr/bin/env bash
    set -euo pipefail

    stack=$(just image::_stack "{{ name }}")
    read -r file project data _ <<<"$stack"

    # Unset for a named-volume stack, or a value exported for `bind` turns the other two's volume into a bind.
    if [ "$data" = "-" ]; then
        unset BINACLE_DATA_DIR
    else
        export BINACLE_DATA_DIR="${BINACLE_DATA_DIR:-$data}"
    fi

    docker compose -f "tooling/$file" -p "$project" {{ args }}

[private]
_prepare name:
    #!/usr/bin/env bash
    set -euo pipefail
    cd tooling

    # The directory only, never `-R`: the container needs a writable mount point, and the files inside stay
    # writable to whoever wrote them. A recursive chmod would fail on those and ask for a password every time.
    ensure_writable() {
        mkdir -p "$1"
        [ "$(stat -c '%a' "$1")" = "777" ] && return 0

        # A directory docker made for us is owned by root, and only sudo reopens that.
        chmod 777 "$1" 2>/dev/null || sudo chmod 777 "$1"
    }

    # Without this, compose falls back to pulling binacle-net from Docker Hub and reports "pull access
    # denied", which reads like a credentials problem.
    require_image() {
        docker image inspect binacle-net:local >/dev/null 2>&1 && return 0
        echo "binacle-net:local not found. Build it first: just build image" >&2
        exit 1
    }

    stack=$(just image::_stack "{{ name }}")
    read -r _ _ data dirs <<<"$stack"

    require_image

    # Same default as _compose, so the folder prepared here is the one compose mounts.
    [ "$data" = "-" ] || ensure_writable "${BINACLE_DATA_DIR:-$data}"
    [ "$dirs" = "-" ] || ensure_writable "$dirs"

# Which docker hub tags are this same image
[private]
_verify-tags version:
    #!/usr/bin/env bash
    set -uo pipefail

    echo "== docker hub tags =="
    json=$(curl --proto '=https' --proto-redir '=https' -fsSL \
        'https://hub.docker.com/v2/repositories/{{ hub_repo }}/tags?page_size=100') || {
        echo "  could not read the tag list" >&2
        exit 1
    }
    digest=$(echo "$json" | jq -r --arg v '{{ version }}' '.results[] | select(.name==$v) | .digest')
    if [ -z "$digest" ] || [ "$digest" = "null" ]; then
        echo "  no '{{ version }}' tag on docker hub" >&2
        exit 1
    fi

    # Rows sharing a digest are one image under several names. The date is printed but never compared - it
    # moves for reasons that are not a retag.
    echo "$json" | jq -r --arg d "$digest" '
        .results[] | select(.digest==$d)
        | "  \(.name)   \(.digest[7:19])   \(.full_size)   \(.last_updated[0:10])"'
    echo "  every row above is the same image under another name"

# Signed by this repo's release workflow
[private]
_verify-signature version:
    #!/usr/bin/env bash
    set -uo pipefail

    echo "== signature =="
    if ! command -v cosign >/dev/null 2>&1; then
        echo "  cosign is not installed - DEVELOPMENT.md has the pinned install" >&2
        exit 1
    fi
    ref="{{ hub_repo }}:{{ version }}"

    # Never drop either flag. Without the identity this only asks whether anyone signed it, and anyone can.
    if out=$(cosign verify "$ref" \
        --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
        --certificate-oidc-issuer https://token.actions.githubusercontent.com 2>&1); then
        printf '  %-52s signed by the release workflow\n' "$ref"
    else
        printf '  %-52s NOT VERIFIED\n' "$ref" >&2
        echo "$out" | sed 's/^/      /' >&2
        exit 1
    fi

# What is attached: SPDX SBOM and SLSA provenance
[private]
_verify-attestations version:
    #!/usr/bin/env bash
    set -uo pipefail

    echo "== attestations =="
    ref="{{ hub_repo }}:{{ version }}"
    sbom=$(docker buildx imagetools inspect "$ref" --format '{{{{ json .SBOM }}' 2>/dev/null)
    prov=$(docker buildx imagetools inspect "$ref" --format '{{{{ json .Provenance }}' 2>/dev/null)
    rc=0

    # Both are manifests inside the index, so the signature above already covers them. This only reports
    # what is attached.
    packages=$(echo "$sbom" | jq -r '[.. | objects | select(has("packages")) | .packages | length] | add' 2>/dev/null)
    if [ -n "$packages" ] && [ "$packages" != "null" ]; then
        printf '  sbom        %s packages\n' "$packages"
    else
        echo "  sbom        MISSING" >&2
        rc=1
    fi
    builder=$(echo "$prov" | jq -r '[.. | objects | select(has("builder")) | .builder.id] | first' 2>/dev/null)
    if [ -n "$builder" ] && [ "$builder" != "null" ]; then
        printf '  provenance  %s\n' "$builder"
    else
        echo "  provenance  MISSING" >&2
        rc=1
    fi
    exit $rc

# Labels, and what the container says about itself
[private]
_verify-metadata version:
    #!/usr/bin/env bash
    set -uo pipefail

    echo "== metadata =="
    ref="{{ hub_repo }}:{{ version }}"
    docker pull -q "$ref" >/dev/null 2>&1 || {
        echo "  could not pull $ref" >&2
        exit 1
    }
    docker image inspect "$ref" --format '{{{{ json .Config.Labels }}' | jq -r '
        "  version     \(.["org.opencontainers.image.version"] // "-")",
        "  revision    \(.["org.opencontainers.image.revision"] // "-")",
        "  base        \(.["org.opencontainers.image.base.name"] // "-")"'

    # From inside a throwaway run: what the image says about itself, not what the registry says about it. A
    # framework-dependent build has a handful of System.*.dll in /app; a self-contained one has about 170.
    docker run --rm --entrypoint sh "$ref" -c '
        printf "  reports     %s\n" "${BINACLE_VERSION:--}"
        printf "  runs as     %s (%s)\n" "$(id -un)" "$(id -u)"
        printf "  /app/data   %s\n" "$(stat -c "%U:%G %a" /app/data 2>/dev/null || echo -)"
        printf "  framework   %s System.*.dll in /app\n" "$(ls /app/System.*.dll 2>/dev/null | wc -l)"
    '
