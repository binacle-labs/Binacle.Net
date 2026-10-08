#!/usr/bin/env bash
# Render the Docker Hub page for a version. Prints to stdout and writes nothing, so the exact page can be
# read before the release is dispatched.
#   dockerhub-overview.sh <version> <page>
set -euo pipefail

version="${1#v}"
page="$2"

case "$version" in
    # A prerelease never reaches Docker Hub, so every tag this page names would be missing.
    *-*)   echo "'$version' is a prerelease. This page describes the stable line only." >&2; exit 1 ;;
    *.*.*) ;;
    *)     echo "Expected a full version like 3.0.0, got '$version'." >&2; exit 1 ;;
esac

minor="${version%.*}"
out="$(sed -e "s/{{VERSION}}/$version/g" -e "s/{{MINOR}}/$minor/g" -e "s/{{MAJOR}}/${minor%.*}/g" "$page")"

if grep -n '{{' <<<"$out" >&2; then
    echo "^ unsubstituted placeholder in $page" >&2
    exit 1
fi
printf '%s\n' "$out"
