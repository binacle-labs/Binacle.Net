#!/usr/bin/env bash
# The run summary for a deploy: the commit, the marker tag and the site.
#   deploy-summary.sh <commit> <tag> <site url>
set -euo pipefail

commit="$1"
tag="$2"
url="$3"

# Where the run writes its summary page. Falls back to the screen, so this also runs on a laptop.
summary="${GITHUB_STEP_SUMMARY:-/dev/stdout}"

# A `|` in the subject would end the table row.
subject=$(git log -1 --format=%s "$commit" | sed 's/|/\\|/g')

{
    echo "## ${url#https://} deployed"
    echo
    echo '| | |'
    echo '|---|---|'
    echo "| Commit | \`${commit:0:8}\` - ${subject} |"
    echo "| Marker tag | \`${tag}\` |"
    echo "| Site | ${url} |"
} >>"$summary"
