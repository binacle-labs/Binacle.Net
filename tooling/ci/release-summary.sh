#!/usr/bin/env bash
# The run summary for a release: what shipped, under which digest and public tags.
#   release-summary.sh <version> <digest> <tag> <release url> '<one tag per line>' <ref>
set -euo pipefail

version="$1"
digest="$2"
tag="$3"
release_url="$4"
tags="$5"
ref="$6"

# A prerelease lives on GHCR only, signed under the branch it ran from, so the published one-liner - Docker
# Hub, main - would fail on it. The command printed is the one that passes.
case "$tag" in
    *-*) where="the one \`smoke\` passed, on GHCR only"
         verify="just image verify ${version} all ${ref} ${tags%%:*}" ;;
    *)   where="the one \`smoke\` passed, copied rather than rebuilt"
         verify="just image verify ${version}" ;;
esac

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

{
    echo "## Published \`${version}\`"
    echo
    echo "**Digest** \`${digest}\` - ${where}."
    echo
    echo '| Tag |'
    echo '|---|'
    while IFS= read -r public_tag; do
        [[ -n "$public_tag" ]] || continue
        echo "| \`${public_tag}\` |"
    done <<<"$tags"
    echo
    echo "[Release ${tag}](${release_url})"
    echo
    echo '```'
    echo "$verify"
    echo '```'
} >>"$summary"
