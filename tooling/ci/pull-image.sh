#!/usr/bin/env bash
# Pull a published image and print its index digest as `digest=sha256:...`.
#   pull-image.sh <image, with tag>
set -euo pipefail

image="$1"

# To stderr, so the only thing on stdout is the line below - the caller appends that to $GITHUB_OUTPUT.
docker pull "$image" >&2

# The index digest, which is what the release copies - not the per-platform one `docker pull` prints.
echo "digest=$(docker buildx imagetools inspect "$image" --format '{{ .Manifest.Digest }}')"
