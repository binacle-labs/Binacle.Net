#!/usr/bin/env bash
# The run summary for a release: what shipped, under which digest and public tags.
#   release-summary.sh <version> <digest> <tag> <release url> '<one tag per line>'
set -euo pipefail

version="$1"
digest="$2"
tag="$3"
release_url="$4"

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

{
    echo "## Published \`${version}\`"
    echo
    echo "**Digest** \`${digest}\` - the one \`smoke\` passed, copied rather than rebuilt."
    echo
    echo '| Tag |'
    echo '|---|'
    while IFS= read -r public_tag; do
        [[ -n "$public_tag" ]] || continue
        echo "| \`${public_tag}\` |"
    done <<<"$5"
    echo
    echo "[Release ${tag}](${release_url})"
    echo
    echo '```'
    echo "just image verify ${version}"
    echo '```'
} >>"$summary"
