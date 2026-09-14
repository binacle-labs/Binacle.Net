#!/usr/bin/env bash
# What is attached: the SPDX SBOM and the SLSA provenance. Both are manifests inside the index, so the
# signature already covers them - this only reports what is there.
#   verify-attestations.sh <version> <repo>
set -uo pipefail

version="$1"
repo="$2"

echo "== attestations =="

# Without this the inspect below fails, both variables come back empty, and the check reports the
# attestations missing. A tool I have not installed is not a fault in the image.
if ! docker buildx version >/dev/null 2>&1; then
    echo "  docker buildx is not installed - DEVELOPMENT.md has the Docker install" >&2
    exit 1
fi
ref="${repo}:${version}"
sbom=$(docker buildx imagetools inspect "$ref" --format '{{ json .SBOM }}' 2>/dev/null)
prov=$(docker buildx imagetools inspect "$ref" --format '{{ json .Provenance }}' 2>/dev/null)
rc=0

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
