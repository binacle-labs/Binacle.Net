#!/usr/bin/env bash
# Which CHANGELOG section a version publishes. Prints `name=<section>`.
#   changelog-section.sh <version>
set -euo pipefail

version="$1"

# A prerelease publishes the in-progress section; a real release publishes its own. The heading is renamed
# from [Unreleased] to the version as the last edit before the release.
case "$version" in
    *-*) echo "name=Unreleased" ;;
    *)   echo "name=${version}" ;;
esac
