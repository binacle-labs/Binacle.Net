#!/usr/bin/env bash
# main may dispatch a release or a prerelease. A release/v<x>-<y>-<z> branch may dispatch a prerelease of
# x.y.z only. Nothing else may dispatch anything.
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
        # release/v3-1-0 names 3.1.0, so a beta of any other version is on the wrong branch.
        if [[ ! "$ref" =~ ^refs/heads/release/v([0-9]+)-([0-9]+)-([0-9]+)$ ]]; then
            echo "A release/ branch is named release/v<major>-<minor>-<patch>. This was dispatched on ${ref}." >&2
            exit 1
        fi
        branch_version="${BASH_REMATCH[1]}.${BASH_REMATCH[2]}.${BASH_REMATCH[3]}"
        if [[ "$branch_version" != "${version%%-*}" ]]; then
            echo "${ref} may dispatch ${branch_version}-* only, not ${version}." >&2
            exit 1
        fi
        ;;
    *)
        echo "A release runs from main, a prerelease from main or a release/* branch. This was dispatched on ${ref}." >&2
        exit 1
        ;;
esac

echo "Dispatched on ${ref}."
