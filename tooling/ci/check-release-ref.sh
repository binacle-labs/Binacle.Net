#!/usr/bin/env bash
# main may dispatch a release or a prerelease. A release/* branch may dispatch a prerelease only - it stops
# at staging. Nothing else may dispatch anything.
#   check-release-ref.sh <ref> <version>
set -euo pipefail

ref="$1"
version="$2"

case "$ref" in
    refs/heads/main) ;;
    refs/heads/release/*)
        if [[ "$version" != *-* ]]; then
            echo "Releases run from main only. This was dispatched on ${ref}." >&2
            exit 1
        fi
        ;;
    *)
        echo "A release runs from main, a prerelease from main or a release/* branch. This was dispatched on ${ref}." >&2
        exit 1
        ;;
esac

echo "Dispatched on ${ref}."
