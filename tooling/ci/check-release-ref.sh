#!/usr/bin/env bash
# Only main may release.
#   check-release-ref.sh <ref>
set -euo pipefail

ref="$1"

if [[ "$ref" != "refs/heads/main" ]]; then
    echo "Releases run from main only. This was dispatched on ${ref}." >&2
    exit 1
fi

echo "Dispatched on ${ref}."
