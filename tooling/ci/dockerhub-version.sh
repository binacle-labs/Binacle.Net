#!/usr/bin/env bash
# Which version the Docker Hub page describes. Prints `name=<version>`.
#   dockerhub-version.sh [version]        empty takes the latest release, and needs GH_TOKEN
set -euo pipefail

version="${1:-}"

# `gh release view` returns the latest NON-prerelease, which is the line this page describes - so a wording fix
# needs nothing typed and cannot name a version by mistake.
[[ -n "$version" ]] || version="$(gh release view --json tagName -q .tagName)"

# To stderr, so the only thing on stdout is the line below - the caller appends that to $GITHUB_OUTPUT.
echo "Page will describe ${version#v}" >&2

echo "name=${version#v}"
