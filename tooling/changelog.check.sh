#!/usr/bin/env bash
# Fails if a version's section is missing or empty.
#   changelog.check.sh <version|Unreleased> <changelog file>
set -euo pipefail

version="$1"
file="$2"

notes="$("$(dirname "$0")/changelog.extract.sh" "$version" "$file")"

if [ -z "${notes//[[:space:]]/}" ]; then
    echo "No '## ${version}' section in ${file}." >&2
    echo "Rename [Unreleased] to ${version} before tagging." >&2
    exit 1
fi

lines=$(printf '%s\n' "$notes" | grep -c '' || true)
echo "'## ${version}' found - ${lines} lines."
