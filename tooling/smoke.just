# Smoke tests a built image: the files it ships and the HTTP surface it serves.
#
#   just smoke all                     build binacle-net:local, check its structure, then every profile
#   just smoke test-structure [image]  the files the image ships - no container, no stack
#   just smoke test <profile> [image]  one profile end to end: up -> hurl -> down
#   just smoke up <profile> [image]    bring one up and leave it   [minimal|quickstart|prod|service|full]
#   just smoke down <profile>          stop it, add -v to drop the volume
#
# The image goes last on every recipe and defaults to binacle-net:local, so the same suite runs against a
# local build or a published tag - `just smoke test service binacle/binacle-net:3.0.0` smokes what is on
# Docker Hub.
#
# The files are checked by one container-structure-test file, tooling/smoke/structure.yaml, read straight
# from the image. That is why it runs once per `all` and not once per profile. The HTTP surface is one .hurl
# per profile, run against a stack that is up.
#
# The stacks here are not the ones under samples/. These raise the anonymous rate limit and throw their
# storage away, which a sample must never do.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# docker, container-structure-test and hurl all print why they failed, so just's own line adds nothing.
set no-exit-message := true

# Install these per DEVELOPMENT.md. `just install` does not.
cst := "container-structure-test"
hurl := "hurl"

# What `just build image` tags, and so what every recipe here smokes unless told otherwise.
local_image := "binacle-net:local"

# just has no list type, so this is a space-separated string - which is what the loop in `all` and the match
# in `_check` both want. Adding a profile is this line plus the two files named after it under tooling/smoke.
profiles := "minimal quickstart prod service full"

# List the recipes
# List the recipes
default:
    @just --list smoke

# The files the image ships, their permissions, the metadata and the OCI labels
test-structure image=local_image:
    {{ cst }} test --image {{ image }} --config tooling/smoke/structure.yaml

# One profile end to end: up -> hurl -> down [minimal|quickstart|prod|service|full]
test profile image=local_image:
    #!/usr/bin/env bash
    set -euo pipefail

    just smoke::_check "{{ profile }}"

    # Tears the stack down even when the hurl run fails. The teardown is not silenced: a failed `down` is how
    # a container leaks, which is the one thing this trap exists to prevent.
    trap 'just smoke::_compose "{{ profile }}" "{{ image }}" down -v || true' EXIT

    just smoke::_compose "{{ profile }}" "{{ image }}" up -d
    just smoke::_test_profile "{{ profile }}"

# Takes no image: it talks to whatever is listening, and `up` already decided which image that is. Private
# because `test` owns the lifecycle, but `just smoke::_test_profile <profile>` still works - which is how you
# re-run a .hurl you are editing without paying for an up and a down each time.
[private]
_test_profile profile:
    {{ hurl }} --test --retry 30 --retry-interval 1000 tooling/smoke/{{ profile }}.hurl

# Bring a profile up and leave it running [minimal|quickstart|prod|service|full]
up profile image=local_image:
    @just smoke::_compose "{{ profile }}" "{{ image }}" up -d

# Stop a profile [minimal|quickstart|prod|service|full]; add -v to drop the volume
down profile *args:
    @just smoke::_compose "{{ profile }}" "{{ local_image }}" down {{ args }}

# Build the image, check its files once, then every profile end to end
all image=local_image:
    #!/usr/bin/env bash
    set -euo pipefail

    # Only build when smoking the local tag. Building for a published one would tag binacle-net:local while
    # the stacks went on pulling the image that was asked for - a green run that says nothing about either.
    if [ "{{ image }}" = "{{ local_image }}" ]; then
        just build image
    else
        echo "smoking {{ image }} - not building"
        docker pull "{{ image }}"
    fi

    echo "===== smoke: image structure ====="
    just smoke test-structure "{{ image }}"

    for profile in {{ profiles }}; do
        echo "===== smoke: $profile ====="
        just smoke test "$profile" "{{ image }}"
    done

    echo "all profiles green: {{ image }}"

# Rejects an unknown profile, in one place
[private]
_check profile:
    #!/usr/bin/env bash

    # $want is quoted inside the pattern so it stays a literal. Unquoted it is a glob, and a profile named
    # `*` would match anything.
    want="{{ profile }}"
    case " {{ profiles }} " in
        *" $want "*) ;;
        *) echo "Unknown profile '$want'. Use one of: {{ profiles }}" >&2; exit 1 ;;
    esac

# A profile name maps to a compose file here and nowhere else, so `up` and `down` cannot disagree about which
# file a name means.
[private]
_compose profile image *args:
    #!/usr/bin/env bash
    set -euo pipefail

    just smoke::_check "{{ profile }}"

    # The stack reads $BINACLE_IMAGE with the same binacle-net:local default, so a bare
    # `docker compose -f tooling/smoke/<profile>.yml` still works.
    BINACLE_IMAGE="{{ image }}" docker compose -f "tooling/smoke/{{ profile }}.yml" {{ args }}
