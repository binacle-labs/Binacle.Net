#!/usr/bin/env bash
# The version is semver shaped, with no leading v.
#   check-version.sh <version>
set -euo pipefail

version="$1"

# Build metadata is rejected as well as a leading v: a Docker tag cannot carry a `+`.
if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z.-]+)?$ ]]; then
    echo "Version '${version}' is not semver. Expected 3.0.0 or 3.0.0-beta.1, with no leading v." >&2
    exit 1
fi

echo "Version ${version} is semver shaped."
