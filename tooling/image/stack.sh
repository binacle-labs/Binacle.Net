#!/usr/bin/env bash
# Bring a local stack up or down. The image is binacle-net:local, built by `just build image`.
#   stack.sh up   <full|volume|bind> [compose args]
#   stack.sh down <full|volume|bind> [compose args]
set -euo pipefail

action="$1"
name="$2"
shift 2

# The one table. It is the only place an unknown name is rejected - a typo would otherwise fall through to
# a compose default and report success for a stack nobody started.
#
# Four columns, `-` for none. Paths are relative to tooling/, because compose resolves a relative bind source
# against the compose file's own directory.
#
#   stack     compose file      project name         /app/data   other folders it bind-mounts
case "$name" in
    full)   read -r file project data dirs <<<"image.full.yml  binacle-net-full   -       ./azurite" ;;
    volume) read -r file project data dirs <<<"image.local.yml binacle-net-volume -       -" ;;
    bind)   read -r file project data dirs <<<"image.local.yml binacle-net-bind   ./data  -" ;;
    *) echo "Unknown stack '${name}'. Use full, volume or bind." >&2; exit 1 ;;
esac

# Unset for a named-volume stack, or a value exported for `bind` turns the other two's volume into a bind.
if [ "$data" = "-" ]; then
    unset BINACLE_DATA_DIR
else
    export BINACLE_DATA_DIR="${BINACLE_DATA_DIR:-$data}"
fi

# The directory only, never `-R`: the container needs a writable mount point, and the files inside stay
# writable to whoever wrote them. A recursive chmod would fail on those and ask for a password every time.
ensure_writable() {
    mkdir -p "$1"
    [ "$(stat -c '%a' "$1")" = "777" ] && return 0

    # A directory docker made for us is owned by root, and only sudo reopens that.
    chmod 777 "$1" 2>/dev/null || sudo chmod 777 "$1"
}

# Without this, compose falls back to pulling binacle-net from Docker Hub and reports "pull access denied",
# which reads like a credentials problem.
require_image() {
    docker image inspect binacle-net:local >/dev/null 2>&1 && return 0
    echo "binacle-net:local not found. Build it first: just build image" >&2
    exit 1
}

if [ "$action" = "up" ]; then
    require_image
    # Same value compose mounts, so the folder prepared is the one it uses.
    ( cd tooling
      [ "$data" = "-" ] || ensure_writable "${BINACLE_DATA_DIR}"
      [ "$dirs" = "-" ] || ensure_writable "$dirs" )
fi

# -p is not optional: `volume` and `bind` come out of one file, so without it `up bind` recreates the
# `volume` container and reads its data as an empty database.
docker compose -f "tooling/$file" -p "$project" "$action" "$@"
