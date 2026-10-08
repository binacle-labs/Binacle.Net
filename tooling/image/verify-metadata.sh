#!/usr/bin/env bash
# The OCI labels, and what the container says about itself from inside a throwaway run.
#   verify-metadata.sh <version> <repo>
set -uo pipefail

version="$1"
repo="$2"

echo "== metadata =="
ref="${repo}:${version}"
docker pull -q "$ref" >/dev/null 2>&1 || {
    echo "  could not pull $ref" >&2
    exit 1
}
docker image inspect "$ref" --format '{{ json .Config.Labels }}' | jq -r '
    "  version     \(.["org.opencontainers.image.version"] // "-")",
    "  revision    \(.["org.opencontainers.image.revision"] // "-")",
    "  base        \(.["org.opencontainers.image.base.name"] // "-")"'

# What the image says about itself, not what the registry says about it. A framework-dependent build has a
# handful of System.*.dll in /app; a self-contained one has about 170.
docker run --rm --entrypoint sh "$ref" -c '
    printf "  reports     %s\n" "${BINACLE_VERSION:--}"
    printf "  runs as     %s (%s)\n" "$(id -un)" "$(id -u)"
    printf "  /app/data   %s\n" "$(stat -c "%U:%G %a" /app/data 2>/dev/null || echo -)"
    printf "  framework   %s System.*.dll in /app\n" "$(ls /app/System.*.dll 2>/dev/null | wc -l)"
'
